using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Engine;

namespace OneBagJianghu.Tests.Engine
{
    public sealed class ItemsV2MechanicsTests
    {
        private static JObject Run(string items, string aItems, string bItems = "")
        {
            ItemCatalog c = ItemCatalog.Load("[" + items + "]");
            Loadout a =
                Loadout.FromJson("{\"name\":\"a\",\"level\":1,\"cols\":10,\"rows\":4,\"items\":[" +
                aItems +
                "]}");
            Loadout b =
                Loadout.FromJson("{\"name\":\"b\",\"level\":181,\"cols\":10,\"rows\":4,\"items\":[" +
                bItems +
                "]}");
            return new BattleSimulator(c).Run(a, b, 42).ToJObject();
        }

        private const string Weapon =
            "{\"id\":\"w\",\"name\":\"w\",\"type\":\"weapon\",\"weaponClass\":\"sword\"," +
            "\"size\":[2,1],\"attack\":10,\"cooldown\":0.1,\"basic\":[{\"op\":\"damage\"}]}";
        [Test]
        public void StatusAppliedEffectsDoNotFireStatusAppliedAgain()
        {
            string listener =
                "{\"id\":\"a\",\"name\":\"a\",\"type\":\"pill\"," +
                "\"triggers\":[{\"on\":\"status_applied\",\"effects\":[{\"op\":\"apply\"," +
                "\"status\":\"poison\",\"stacks\":7}]}]}";
            string poisoner =
                "{\"id\":\"b\",\"name\":\"b\",\"type\":\"weapon\",\"weaponClass\":\"sword\"," +
                "\"size\":[2,1],\"attack\":1000,\"cooldown\":0.1,\"basic\":[{\"op\":\"damage\"}]," +
                "\"triggers\":[{\"on\":\"hit\",\"from\":\"self\",\"effects\":[{\"op\":\"apply\"," +
                "\"status\":\"poison\",\"stacks\":5}]}]}";
            JArray events =
                (JArray)Run(listener +
                "," +
                poisoner,
                "{\"id\":\"a\",\"x\":0,\"y\":0},{\"id\":\"b\",\"x\":1,\"y\":0}")["events"];
            JToken[] poison =
                events.Where(x =>
                x.Value<string>("type") == "STATUS" &&
                x.Value<string>("kind") == "POISON").ToArray();
            Assert.That(poison.Select(x => x.Value<int>("value")),
                Is.EqualTo(new[] { 5,
                7 }));
            Assert.That(poison[1].Value<int>("total"), Is.EqualTo(12));
            Assert.That(events.Count(x =>
                x.Value<string>("type") == "TRIGGER" &&
                x.Value<int?
                >("item") == 0 &&
                x.Value<string>("kind") == "PASSIVE"),
                Is.EqualTo(1));
        }

        [Test]
        public void CatalogRejectsInvalidWeaponClasses()
        {
            DataException unknown =
                Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"x\",\"name\":\"x\",\"type\":\"weapon\",\"weaponClass\":\"dagger\"," +
                "\"size\":[2,1],\"attack\":1,\"cooldown\":1}]"));
            StringAssert.Contains("weaponClass must be one of", unknown.Message);
            DataException nonWeapon =
                Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"x\",\"name\":\"x\",\"type\":\"armor\",\"weaponClass\":\"sword\"}]"));
            StringAssert.Contains("weaponClass is only valid for weapons", nonWeapon.Message);
        }

        [Test]
        public void CatalogAcceptsActiveAndRejectsInvalidNewFields()
        {
            Assert.DoesNotThrow(() =>
                ItemCatalog.Load("[{\"id\":\"r\",\"name\":\"r\",\"type\":\"relic\",\"cooldown\":1," +
                "\"active\":[{\"op\":\"gain_qi\",\"value\":1}]}]"));
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"r\",\"name\":\"r\",\"type\":\"relic\",\"cooldown\":1}]"));
            Assert.Throws<DataException>(() =>
                ItemCatalog.Load("[{\"id\":\"x\",\"name\":\"x\",\"type\":\"pill\"," +
                "\"triggers\":[{\"on\":\"battle_start\",\"effects\":[{\"op\":\"lifesteal\"," +
                "\"pct\":0.5}]}]}]"));
        }

        [Test]
        public void BuffStackCapsRaiseHpAndMinQiGate()
        {
            string item =
                "{\"id\":\"x\",\"name\":\"x\",\"type\":\"pill\"," +
                "\"triggers\":[{\"on\":\"battle_start\",\"effects\":[{\"op\":\"raise_max_hp\"," +
                "\"value\":25},{\"op\":\"buff_stack\",\"stat\":\"dmg_reduce\",\"pct\":0.1," +
                "\"maxStacks\":1},{\"op\":\"buff_stack\",\"stat\":\"dmg_reduce\",\"pct\":0.1," +
                "\"maxStacks\":1}]}]}";
            JArray events = (JArray)Run(item, "{\"id\":\"x\",\"x\":0,\"y\":0}")["events"];
            Assert.That(events.Count(x =>
                x.Value<string>("type") == "MAX_HP" &&
                x.Value<int>("maxHp") == 125),
                Is.EqualTo(1));
            Assert.That(events.Count(x => x.Value<string>("kind") == "DMG_REDUCE"),
                Is.EqualTo(1));
        }

        [Test]
        public void LifestealHealedAndNewCauseTriggersChain()
        {
            string w =
                "{\"id\":\"w\",\"name\":\"w\",\"type\":\"weapon\",\"weaponClass\":\"sword\"," +
                "\"size\":[2,1],\"attack\":20,\"cooldown\":0.1,\"crit\":1," +
                "\"basic\":[{\"op\":\"damage\"},{\"op\":\"lifesteal\",\"pct\":0.5}]," +
                "\"triggers\":[{\"on\":\"healed\",\"effects\":[{\"op\":\"gain_qi\"," +
                "\"value\":1}]}]}";
            string hurt =
                "{\"id\":\"h\",\"name\":\"h\",\"type\":\"armor\"," +
                "\"triggers\":[{\"on\":\"battle_start\",\"effects\":[{\"op\":\"damage\"," +
                "\"value\":30,\"target\":\"self\"}]}]}";
            JArray events =
                (JArray)Run(w +
                "," +
                hurt,
                "{\"id\":\"w\",\"x\":0,\"y\":0},{\"id\":\"h\",\"x\":2,\"y\":0}")["events"];
            Assert.That(events.Any(x => x.Value<string>("type") == "HEAL"), Is.True);
            Assert.That(events.Any(x =>
                x.Value<string>("type") == "QI" &&
                x.Value<int>("value") == 1),
                Is.True);
        }

        [Test]
        public void CleanseAndEveryNewTriggerDeserialize()
        {
            foreach (string on in new[]
            {
                "damaged",
                "healed",
                "controlled",
                "crit_taken",
                "enemy_ultimate",
                "qi_empty",
                "shield_broken",
                "burst_taken"
            }

            )
            {
                Assert.DoesNotThrow(() =>
                    ItemCatalog.Load("[{\"id\":\"x\",\"name\":\"x\",\"type\":\"pill\",\"triggers\":[{\"on\":\"" +
                    on +
                    "\",\"effects\":[{\"op\":\"cleanse\"}]}]}]"),
                    on);
            }
        }
    }
}