using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
            Assert.AreEqual(5, c.Chapters.Count);
            int stageCount = 0;
            foreach (PveChapter chapter in c.Chapters)
                stageCount += chapter.Stages.Count;
            Assert.AreEqual(50, stageCount);
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

        [Test]
        public void CatalogParsesRepeatRewardPercent()
        {
            // Arrange
            JObject data = JObject.Parse(json);
            data["repeatRewardPercent"] = 27;

            // Act
            PveCatalog catalog = PveCatalog.Parse(data.ToString(), items);

            // Assert
            Assert.AreEqual(27, catalog.RepeatRewardPercent);
        }

        [Test]
        public void CatalogDefaultsRepeatRewardPercentToFifteen()
        {
            // Arrange
            JObject data = JObject.Parse(json);
            data.Remove("repeatRewardPercent");

            // Act
            PveCatalog catalog = PveCatalog.Parse(data.ToString(), items);

            // Assert
            Assert.AreEqual(15, catalog.RepeatRewardPercent);
        }

        [TestCase(-1)]
        [TestCase(101)]
        public void CatalogRejectsInvalidRepeatRewardPercent(int percent)
        {
            // Arrange
            JObject data = JObject.Parse(json);
            data["repeatRewardPercent"] = percent;

            // Act
            TestDelegate parse = () => PveCatalog.Parse(data.ToString(), items);

            // Assert
            Assert.Throws<FormatException>(parse);
        }

        [Test]
        public void ReplayUsesFifteenPercentOfStageFirstClearReward()
        {
            // Arrange
            PveCatalog catalog = PveCatalog.Parse(json, items);
            PveStage stage = catalog.GetStage("hoi_1_01");
            var profile = new PlayerProfile();
            var service = new PveRewardService(items, catalog.RepeatRewardPercent);

            // Act
            PveRewardResult result = service.Grant(profile, stage, false, 1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(45, result.Silver);
            Assert.AreEqual(8, result.HeroExp);
            Assert.That(MaterialCount(result, "han_thiet"), Is.InRange(0, 1));
            Assert.IsFalse(result.Items.Exists(item => item.Id == "boi_nguyen_dan"));
        }

        [Test]
        public void ReplayMaterialAverageMatchesFractionalReward()
        {
            // Arrange
            PveCatalog catalog = PveCatalog.Parse(json, items);
            PveStage stage = catalog.GetStage("hoi_1_01");
            var service = new PveRewardService(items, catalog.RepeatRewardPercent);
            const int samples = 2000;
            int total = 0;

            // Act
            for (int seed = 0; seed < samples; seed++)
            {
                PveRewardResult result = service.Grant(new PlayerProfile(), stage, false, seed);
                total += MaterialCount(result, "han_thiet");
            }

            // Assert
            Assert.That((double)total / samples, Is.EqualTo(0.45d).Within(0.05d));
        }

        [Test]
        public void ReplayRewardIsDeterministicForSeed()
        {
            // Arrange
            PveCatalog catalog = PveCatalog.Parse(json, items);
            PveStage stage = catalog.GetStage("hoi_1_01");
            var service = new PveRewardService(items, catalog.RepeatRewardPercent);

            // Act
            PveRewardResult first = service.Grant(new PlayerProfile(), stage, false, 739);
            PveRewardResult second = service.Grant(new PlayerProfile(), stage, false, 739);

            // Assert
            Assert.AreEqual(RewardSignature(first), RewardSignature(second));
        }

        [Test]
        public void ReplayRoundsPositiveSilverUpToMinimumOne()
        {
            // Arrange
            PveStage stage = SyntheticStage(3, 0);
            var service = new PveRewardService(items, 15);

            // Act
            PveRewardResult result = service.Grant(new PlayerProfile(), stage, false, 1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Silver);
        }

        [Test]
        public void FirstClearRewardRemainsUnscaled()
        {
            // Arrange
            PveCatalog catalog = PveCatalog.Parse(json, items);
            PveStage stage = catalog.GetStage("hoi_1_01");
            var service = new PveRewardService(items, catalog.RepeatRewardPercent);

            // Act
            PveRewardResult result = service.Grant(new PlayerProfile(), stage, true, 1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(300, result.Silver);
            Assert.AreEqual(50, result.HeroExp);
            Assert.AreEqual(3, MaterialCount(result, "han_thiet"));
            // Stages no longer reward items (decision #52).
            Assert.AreEqual(0, result.Items.Count);
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

        [TestCase(10, 0, 0d, 0d)]
        [TestCase(5, 4, 120d, 1560d)]
        [TestCase(0, 0, 360d, 3600d)]
        public void StaminaCountdownsCoverFullPartialAndEmpty(
            int stamina,
            int elapsedMinutes,
            double expectedNext,
            double expectedFull)
        {
            // Arrange
            DateTimeOffset last = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
            var state = new PveProfileState
            {
                Stamina = stamina,
                LastStaminaRegenUtc = last.ToString("O")
            };
            var service = new StaminaService(new StaminaConfig { Max = 10, RegenPerMinute = 1f / 6f });

            // Act
            double next = service.SecondsToNext(state, last.AddMinutes(elapsedMinutes));
            double full = service.SecondsToFull(state, last.AddMinutes(elapsedMinutes));

            // Assert
            Assert.AreEqual(expectedNext, next, .001d);
            Assert.AreEqual(expectedFull, full, .001d);
        }

        [Test]
        public void TrySpendKeepsPartialRegenProgress()
        {
            // Arrange
            DateTimeOffset last = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
            var state = new PveProfileState
            {
                Stamina = 50,
                LastStaminaRegenUtc = last.ToString("O")
            };
            var service = new StaminaService(new StaminaConfig { Max = 60, RegenPerMinute = 1f / 6f });

            // Act
            bool spent = service.TrySpend(state, 6, last.AddMinutes(4));
            double next = service.SecondsToNext(state, last.AddMinutes(4));

            // Assert
            Assert.IsTrue(spent);
            Assert.AreEqual(44, state.Stamina);
            Assert.AreEqual(120d, next, .001d);
        }

        [TestCase(252d, "04:12")]
        [TestCase(6480d, "1:48:00")]
        public void StaminaDurationFormattingUsesMinutesOrHours(double seconds, string expected)
        {
            // Act
            string result = StaminaService.FormatDuration(seconds);

            // Assert
            Assert.AreEqual(expected, result);
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

        private static int MaterialCount(PveRewardResult reward, string id)
        {
            return reward.Materials.TryGetValue(id, out int count) ? count : 0;
        }

        private static string RewardSignature(PveRewardResult reward)
        {
            return JsonConvert.SerializeObject(new
            {
                reward.Success,
                reward.Silver,
                reward.HeroExp,
                reward.Materials,
                reward.Items
            });
        }

        private static PveStage SyntheticStage(long silver, long heroExp)
        {
            return new PveStage
            {
                Rewards = new PveRewards
                {
                    FirstClear = new PveFirstClearReward
                    {
                        Silver = silver,
                        HeroExp = heroExp,
                        Materials = new Dictionary<string, int>(),
                        Items = new List<PveRewardItem>()
                    },
                    Repeat = "default",
                    Drops = new List<PveDrop>()
                }
            };
        }
    }
}
