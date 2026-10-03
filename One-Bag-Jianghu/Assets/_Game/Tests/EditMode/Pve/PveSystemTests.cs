using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using UnityEngine;

namespace OneBagJianghu.Tests.Pve
{
    public sealed class PveSystemTests
    {
        private string json;
        private ItemCatalog items;
        [SetUp]
        public void SetUp()
        {
            json =
File.ReadAllText(Path.Combine(Application.dataPath,
"_Game/Resources/Data/Pve/chapters.json"));
            items =
ItemCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
"_Game/Resources/Data/items.json")));
        }

        [Test]
        public void PlaceholderCatalogIsValid()
        {
            PveCatalog c = PveCatalog.Parse(json, items);
            Assert.AreEqual(3, c.Chapters.Count);
            Assert.AreEqual(30,
c.Chapters[0].Stages.Count +
c.Chapters[1].Stages.Count +
c.Chapters[2].Stages.Count);
        }

        [Test]
        public void EnemyBattleUsesCatalogLevel()
        {
            PveStage s = PveCatalog.Parse(json, items).Chapters[0].Stages[0];
            string player = "{\"name\":\"p\",\"level\":1,\"cols\":5,\"rows\":4,\"items\":[]}";
            int first =
PveBattleService.RunBattle(player, s, 1).Result["events"][1].Value<int>("maxHp");
            s.Enemy.Level++;
            int second =
PveBattleService.RunBattle(player, s, 1).Result["events"][1].Value<int>("maxHp");
            Assert.AreEqual(first + 5, second);
        }

        [Test]
        public void RewardMaterialsExistInMaterialCatalog()
        {
            MaterialCatalog materials =
MaterialCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
"_Game/Resources/Data/materials.json")));
            foreach (PveChapter chapter in PveCatalog.Parse(json, items).Chapters)
            {
                foreach (PveStage stage in chapter.Stages)
                {
                    foreach (string id in stage.Rewards.FirstClear.Materials?
.Keys ?? new System.Collections.Generic.Dictionary<string,
int>().Keys)
                    {
                        Assert.DoesNotThrow(() =>
materials.Get(id),
"Stage '" +
stage.Id +
"' rewards unknown material id '" +
id +
"'.");
                    }
                }
            }
        }

        [Test]
        public void RejectsDuplicateIds()
        {
            JObject j = JObject.Parse(json);
            j["chapters"][1]["id"] = j["chapters"][0]["id"];
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [Test]
        public void RejectsUnknownItem()
        {
            JObject j = JObject.Parse(json);
            j["chapters"][0]["stages"][0]["enemy"]["items"][0]["id"] = "missing";
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [Test]
        public void RejectsBagOutsideGrid()
        {
            JObject j = JObject.Parse(json);
            j["chapters"][0]["stages"][0]["enemy"]["items"][0]["x"] = 5;
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [Test]
        public void RejectsBagOverlap()
        {
            JObject j = JObject.Parse(json);
            JArray a = (JArray)j["chapters"][0]["stages"][1]["enemy"]["items"];
            a[1]["x"] = 0;
            a[1]["y"] = 0;
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [Test]
        public void RejectsUnknownDialogueSpeaker()
        {
            JObject j = JObject.Parse(json);
            j["chapters"][0]["stages"][0]["dialogue"]["before"][0]["speaker"] = "missing";
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [Test]
        public void RejectsMissingThemeKey()
        {
            JObject j = JObject.Parse(json);
            j["chapters"][0]["theme"]["accent"] = "";
            Assert.Throws<FormatException>(() => PveCatalog.Parse(j.ToString(), items));
        }

        [TestCase(true, 840, 1200, 3)]
        [TestCase(true, 480, 1200, 2)]
        [TestCase(true, 1, 1200, 1)]
        [TestCase(false, 1200, 1200, 0)]
        public void StarsFollowRemainingHp(bool won, int hp, int max, int expected)
        {
            Assert.AreEqual(expected, PveRules.Stars(won, hp, max));
        }

        [Test]
        public void StaminaRegeneratesAndSpends()
        {
            var state = new PveProfileState
            {
                Stamina = 5,
                LastStaminaRegenUtc = "2026-01-01T00:00:00Z"
            };
            var service = new StaminaService(new StaminaConfig { Max = 10, RegenPerMinute = .5f });
            Assert.AreEqual(7,
service.Refresh(state,
DateTimeOffset.Parse("2026-01-01T00:04:01Z")));
            Assert.IsTrue(service.TrySpend(state,
6,
DateTimeOffset.Parse("2026-01-01T00:04:01Z")));
            Assert.AreEqual(1, state.Stamina);
            Assert.IsFalse(service.TrySpend(state,
2,
DateTimeOffset.Parse("2026-01-01T00:04:01Z")));
        }

        [Test]
        public void UnlockRequiresPreviousStageAndBoss()
        {
            PveCatalog c = PveCatalog.Parse(json, items);
            var p = new PlayerProfile();
            Assert.IsTrue(PveRules.IsStageUnlocked(c.Chapters[0].Stages[0], c, p));
            Assert.IsFalse(PveRules.IsStageUnlocked(c.Chapters[0].Stages[1], c, p));
            p.Pve.Stages[c.Chapters[0].Stages[0].Id] = new PveStageProgress
            {
                Cleared = true
            };
            Assert.IsTrue(PveRules.IsStageUnlocked(c.Chapters[0].Stages[1], c, p));
            Assert.IsFalse(PveRules.IsChapterUnlocked(c.Chapters[1], c, p));
            p.Pve.Stages[c.Chapters[0].Stages[c.Chapters[0].Stages.Count - 1].Id] = new PveStageProgress
            {
                Cleared = true
            };
            Assert.IsTrue(PveRules.IsChapterUnlocked(c.Chapters[1], c, p));
        }

        [Test]
        public void MigratesVersionFourWithSafePveDefaults()
        {
            var original = new PlayerProfile();
            JObject old = JObject.Parse(original.ToJson());
            old["version"] = 4;
            old.Remove("pve");
            PlayerProfile migrated = PlayerProfile.FromJson(old.ToString());
            Assert.AreEqual(PlayerProfile.CurrentVersion, migrated.Version);
            Assert.AreEqual(-1, migrated.Pve.Stamina);
            Assert.IsEmpty(migrated.Pve.Stages);
        }
    }
}