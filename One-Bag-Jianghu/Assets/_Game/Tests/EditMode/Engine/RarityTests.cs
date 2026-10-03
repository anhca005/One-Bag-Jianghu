using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Engine;

namespace OneBagJianghu.Tests.Engine
{
    public sealed class RarityTests
    {
        private const string ItemJson =
"[{\"id\":\"item\",\"name\":\"Item\",\"type\":\"weapon\"," +
"\"weaponClass\":\"sword\",\"size\":[1,2],\"attack\":100,\"cooldown\":1," +
"\"stats\":{\"hp\":900}," +
"\"basic\":[{\"op\":\"shield\",\"value\":20},{\"op\":\"damage\"," +
"\"target\":\"self\",\"value\":100},{\"op\":\"heal\",\"value\":20}," +
"{\"op\":\"apply\",\"status\":\"poison\",\"stacks\":10},{\"op\":\"gain_qi\"," +
"\"value\":7},{\"op\":\"apply\",\"status\":\"haste\",\"target\":\"self_item\"," +
"\"duration\":2},{\"op\":\"damage\"}]}]";
        private static Loadout LoadoutWith(string rarity = null, int tier = 1)
        {
            string rarityJson = rarity == null ? "" : ",\"rarity\":\"" + rarity + "\"";
            return Loadout.FromJson(
"{\"name\":\"a\",\"level\":1,\"items\":[{\"id\":\"item\",\"x\":0,\"y\":0,\"tier\":" +
tier +
rarityJson +
"}]}");
        }

        private static BattleResult Fight(Loadout a)
        {
            Loadout b = new Loadout
            {
                name = "b",
                level = 2000
            };
            return new BattleSimulator(ItemCatalog.Load(ItemJson)).Run(a, b, 1);
        }

        [Test]
        public void RarityScalesAttackShieldHealAndStacksButNotQiOrDuration()
        {
            BattleResult result = Fight(LoadoutWith("supreme"));
            Assert.That(result.events.First(e =>
e.type == EventType.DAMAGE &&
e.side == 1).value,
Is.EqualTo(160));
            Assert.That(result.events.First(e => e.type == EventType.SHIELD).value,
Is.EqualTo(32));
            Assert.That(result.events.First(e => e.type == EventType.HEAL).value,
Is.EqualTo(32));
            Assert.That(result.events.First(e =>
e.type == EventType.STATUS &&
e.kind == "POISON").value,
Is.EqualTo(16));
            Assert.That(result.events.First(e => e.type == EventType.QI).value,
Is.EqualTo(7));
            Assert.That(result.events.First(e =>
e.type == EventType.STATUS &&
e.kind == "HASTE").value,
Is.EqualTo(2000));
        }

        [Test]
        public void MissingRarityDefaultsToCommonAndDefinitionRarityIsOptional()
        {
            Assert.That(ItemCatalog.Load("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"relic\"}]").Get("x").rarity,
Is.EqualTo(Rarity.COMMON));
            Assert.That(Fight(LoadoutWith()).events.First(e =>
e.type == EventType.DAMAGE &&
e.side == 1).value,
Is.EqualTo(100));
        }

        [Test]
        public void InvalidRarityIsRejected()
        {
            Assert.Throws<DataException>(() => LoadoutWith("mythic"));
        }

        [Test]
        public void ScalingTruncatesTierBeforeRarity()
        {
            const string json =
"[{\"id\":\"item\",\"name\":\"Item\",\"type\":\"weapon\"," +
"\"weaponClass\":\"sword\",\"size\":[1,2],\"attack\":19,\"cooldown\":1," +
"\"basic\":[{\"op\":\"damage\"}]}]";
            Loadout b = new Loadout
            {
                name = "b",
                level = 2000
            };
            BattleResult result =
new BattleSimulator(ItemCatalog.Load(json)).Run(LoadoutWith("rare",
2),
b,
1);
            Assert.That(result.events.First(e =>
e.type == EventType.DAMAGE &&
e.side == 1).value,
Is.EqualTo(22)); // 19*1050/1000=19, then 19*1210/1000=22 (untruncated 19*1.05*1.21 would be 24)
        }
    }
}
