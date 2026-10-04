using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Engine;

namespace OneBagJianghu.Tests.Engine
{
    public sealed class Decision35StatsTests
    {
        private const string Weapon =
            "{\"id\":\"w\",\"name\":\"W\",\"type\":\"weapon\",\"weaponClass\":\"sword\"," +
            "\"size\":[1,2],\"attack\":100,\"cooldown\":1,\"basic\":[{\"op\":\"damage\"}]}";
        private static Loadout L(string items,
            int level =
            1) =>
            Loadout.FromJson("{\"name\":\"x\",\"level\":" + level +
            ",\"items\":[" +
            items +
            "]}");
        [Test]
        public void StatsParseAndValidate()
        {
            ItemDef d =
                ItemCatalog.Load("[{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"hp\":12," +
                "\"dmgReduce\":0.125,\"cooldownReduction\":0.1}}]").Get("a");
            Assert.AreEqual(12, d.stats.hp);
            Assert.AreEqual(125, d.stats.DmgReducePermille());
            Assert.AreEqual(100, d.stats.CooldownReductionPermille());
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"hp\":-1}}]"));
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"hp\":1.5}}]"));
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"dmgReduce\":1.01}}]"));
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"unknown\":1}}]"));
        }

        [Test]
        public void MaxHpUsesLevelAndTruncatedTierThenRarityScaling()
        {
            string item = "{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"hp\":3}}";
            BattleResult r =
                new BattleSimulator(ItemCatalog.Load("[" + item + "]")).Run(
                L("{\"id\":\"a\",\"x\":0,\"y\":0,\"tier\":2,\"rarity\":\"rare\"}",
                10),
                L("",
                1),
                1);
            // 100 + 45 + ((3*1050/1000)*1210/1000) = 148
            Assert.AreEqual(148,
                r.events.First(e => e.type == EventType.START && e.side == 0).maxHp);
        }

        [Test]
        public void StartingDamageReductionCapsAndTemporaryReductionAddsToIt()
        {
            string pill =
                "{\"id\":\"p\",\"name\":\"P\",\"type\":\"pill\",\"stats\":{\"dmgReduce\":0.75}," +
                "\"triggers\":[{\"on\":\"battle_start\",\"effects\":[{\"op\":\"dmg_reduce\"," +
                "\"pct\":0.2}]}]}";
            BattleResult r =
                new BattleSimulator(ItemCatalog.Load("[" + pill + "," + Weapon + "]"))
                    .Run(L("{\"id\":\"p\",\"x\":0,\"y\":0}"),
                L("{\"id\":\"w\",\"x\":0,\"y\":0}",
                100),
                1);
            Assert.AreEqual(800,
                r.events.First(e =>
                e.type == EventType.DMG_REDUCE && e.side == 0).value);
            Assert.AreEqual(20,
                r.events.First(e => e.type == EventType.DAMAGE && e.side == 0).value);
        }

        [Test]
        public void CooldownReductionCapsAndUsesTickFloor()
        {
            string manual =
                "{\"id\":\"m\",\"name\":\"M\",\"type\":\"manual\",\"stats\":{\"cooldownReduction\":1.0}}";
            string fast = Weapon.Replace("\"cooldown\":1", "\"cooldown\":0.1");
            BattleResult r =
                new BattleSimulator(ItemCatalog.Load("[" + manual + "," + fast + "]")).Run(
                L("{\"id\":\"m\",\"x\":0,\"y\":0},{\"id\":\"w\",\"x\":1,\"y\":0}"),
                L("",
                100),
                1);
            Assert.AreEqual(100,
                r.events.First(e =>
                e.type == EventType.TRIGGER && e.item == 0 + 1).t);
        }

        [Test]
        public void ChargeUsesReducedCooldown()
        {
            string manual =
                "{\"id\":\"m\",\"name\":\"M\",\"type\":\"manual\"," +
                "\"stats\":{\"cooldownReduction\":0.4},\"triggers\":[{\"on\":\"battle_start\"," +
                "\"effects\":[{\"op\":\"charge\",\"target\":\"self_all_items\"," +
                "\"duration\":0.5}]}]}";
            BattleResult r =
                new BattleSimulator(ItemCatalog.Load("[" + manual + "," + Weapon + "]")).Run(
                L("{\"id\":\"m\",\"x\":0,\"y\":0},{\"id\":\"w\",\"x\":1,\"y\":0}"),
                L("",
                100),
                1);
            Assert.AreEqual(100,
                r.events.First(e => e.type == EventType.TRIGGER && e.item == 1).t);
        }

        [Test]
        public void RemovedHpFieldIsRejected()
        {
            DataException e =
                Assert.Throws<DataException>(() =>
                Loadout.FromJson("{\"name\":\"old\",\"hp\":100,\"items\":[]}"));
            StringAssert.Contains("was removed", e.Message);
        }

        [Test]
        public void FighterStatsHelperMatchesBattleStartForKnownBag()
        {
            string armor = "{\"id\":\"a\",\"name\":\"A\",\"type\":\"armor\",\"stats\":{\"hp\":140}}";
            string manual =
                "{\"id\":\"m\",\"name\":\"M\",\"type\":\"manual\",\"stats\":{\"cooldownReduction\":0.1}}";
            ItemCatalog catalog = ItemCatalog.Load("[" + armor + "," + manual + "]");
            Loadout bag =
                L("{\"id\":\"a\",\"x\":0,\"y\":0,\"tier\":3,\"rarity\":\"rare\"},{\"id\":\"m\"," +
                "\"x\":1,\"y\":0}",
                8);
            var simulator = new BattleSimulator(catalog);
            FighterStats stats = simulator.FighterStats(bag);
            BattleResult battle = simulator.Run(bag, L("", 1), 1);
            Assert.AreEqual(battle.events.First(e =>
                e.type == EventType.START && e.side == 0).maxHp,
                stats.maxHp);
            Assert.AreEqual(321, stats.maxHp); // 100 + 35 + (140*1100/1000*1210/1000)
            Assert.AreEqual(100, stats.cooldownReductionPermille);
        }
    }
}
