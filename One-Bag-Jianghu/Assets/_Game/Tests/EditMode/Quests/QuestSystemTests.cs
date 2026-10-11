using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using OneBagJianghu.Quests;
using UnityEngine;

namespace OneBagJianghu.Tests.Quests
{
    public sealed class QuestSystemTests
    {
        private QuestCatalog catalog;
        private QuestService service;
        private PlayerProfile profile;

        [SetUp]
        public void SetUp()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/quests");
            PveCatalog pve = PveCatalog.Load();
            MaterialCatalog materials = GameSession.LoadMaterialCatalog();
            catalog = QuestCatalog.Parse(asset.text, pve, materials);
            service = new QuestService(catalog, pve, materials, ItemCatalog.LoadFromResources());
            profile = NewProfile();
        }

        [Test]
        public void Catalog_LoadsNineUniqueQuestsPerHoi()
        {
            // Arrange
            var ids = new HashSet<string>();
            var perChapter = new Dictionary<int, int>();

            // Act
            foreach (QuestDefinition quest in catalog.Quests)
            {
                ids.Add(quest.Id);
                perChapter.TryGetValue(quest.Chapter, out int count);
                perChapter[quest.Chapter] = count + 1;
            }

            // Assert
            Assert.That(ids, Has.Count.EqualTo(catalog.Quests.Count));
            for (int chapter = 1; chapter <= 5; chapter++)
                Assert.That(perChapter[chapter], Is.EqualTo(9), "Hoi " + chapter);
        }

        [TestCase("objective", "type", "unknown")]
        [TestCase("objective", "stage", "missing_stage")]
        [TestCase("reward", "materials", "missing_material")]
        public void Catalog_RejectsUnknownReferences(string section, string field, string value)
        {
            // Arrange
            JObject root = JObject.Parse(Resources.Load<TextAsset>("Data/quests").text);
            JObject quest = (JObject)root["quests"][0];
            if (field == "materials")
                quest[section][field] = new JObject { [value] = 1 };
            else
                quest[section][field] = value;

            // Act / Assert
            Assert.Throws<System.FormatException>(() => QuestCatalog.Parse(root.ToString(),
                PveCatalog.Load(), GameSession.LoadMaterialCatalog()));
        }

        [TestCase("hero_level", 7, 0, 7)]
        [TestCase("tam_phap_level", 4, 0, 4)]
        [TestCase("equip_count", 0, 3, 3)]
        [TestCase("item_tier", 0, 5, 5)]
        public void StateObjective_UsesCurrentProfileState(string type, int level, int count, int target)
        {
            // Arrange
            QuestDefinition quest = MakeQuest(type, target);
            profile.HeroLevel = level > 0 ? level : 1;
            profile.TamPhapLevel = level > 0 ? level : 1;
            for (int i = 0; i < count; i++)
            {
                OwnedItem item = profile.AddItem("thiet_kiem", type == "item_tier" ? target : 1);
                if (type == "equip_count")
                    profile.Loadout.Add(new LoadoutPlacement { Uid = item.Uid, X = i, Y = 0 });
            }

            // Act
            QuestProgress progress = service.Progress(profile, quest);

            // Assert
            Assert.That(progress.Done, Is.True);
        }

        [Test]
        public void SectAndStageObjectives_AlreadyMetCompleteImmediately()
        {
            // Arrange
            profile.SectId = "thai_thanh_quan";
            profile.Pve.Stages["hoi_1_01"] = new PveStageProgress { Cleared = true };

            // Act
            QuestProgress sect = service.Progress(profile, MakeQuest("sect_joined", 1));
            QuestProgress stage = service.Progress(profile, MakeQuest("stage_clear", 1));

            // Assert
            Assert.That(sect.Done, Is.True);
            Assert.That(stage.Done, Is.True);
        }

        [TestCase("shop_buy")]
        [TestCase("item_upgrade")]
        [TestCase("item_merge")]
        [TestCase("pve_win")]
        public void ActionObjective_CountsOnlyAfterActivation(string action)
        {
            // Arrange
            profile.RecordQuestAction(action);
            service.Active(profile);

            // Act
            QuestProgress before = service.Progress(profile, MakeQuest(action, 1));
            profile.RecordQuestAction(action);
            QuestProgress after = service.Progress(profile, MakeQuest(action, 1));

            // Assert
            Assert.That(before.Current, Is.Zero);
            Assert.That(after.Done, Is.True);
        }

        [Test]
        public void Claim_PaysAndAdvances()
        {
            // Arrange
            profile.Pve.Stages["hoi_1_01"] = new PveStageProgress { Cleared = true };
            profile.Currencies["bac"] = 0;

            // Act
            QuestClaimResult result = service.Claim(profile);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(profile.Currencies["bac"], Is.EqualTo(80));
            Assert.That(profile.Quest.ActiveIndex, Is.EqualTo(1));
            Assert.That(profile.Quest.ClaimedIds, Does.Contain("q1_01"));
        }

        [Test]
        public void Claim_WhenStorageIsFullPaysNothing()
        {
            // Arrange
            profile.Storage.Cols = 1;
            profile.Storage.Rows = 1;
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId = "han_thiet",
                Count = StorageGrid.MaxStack,
                X = 0,
                Y = 0
            });
            profile.Materials["han_thiet"] = StorageGrid.MaxStack;
            profile.Pve.Stages["hoi_1_01"] = new PveStageProgress { Cleared = true };
            profile.Currencies["bac"] = 0;

            // Act
            QuestClaimResult result = service.Claim(profile);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.EqualTo("Tàng Bảo Các không đủ chỗ."));
            Assert.That(profile.Currencies["bac"], Is.Zero);
            Assert.That(profile.Quest.ActiveIndex, Is.Zero);
        }

        [Test]
        public void OldProfileJsonWithoutQuestFields_LoadsDefaults()
        {
            // Arrange
            JObject old = JObject.Parse(profile.ToJson());
            old["version"] = 8;
            old.Remove("quest");

            // Act
            PlayerProfile loaded = PlayerProfile.FromJson(old.ToString(), ItemCatalog.LoadFromResources());

            // Assert
            Assert.That(loaded.Quest, Is.Not.Null);
            Assert.That(loaded.Quest.ActiveIndex, Is.Zero);
            Assert.That(loaded.Quest.ClaimedIds, Is.Empty);
        }

        private static QuestDefinition MakeQuest(string type, int target)
        {
            return new QuestDefinition
            {
                Id = "test",
                Objective = new QuestObjective
                {
                    Type = type,
                    Stage = "hoi_1_01",
                    Level = target,
                    Count = target,
                    Tier = target
                }
            };
        }

        private static PlayerProfile NewProfile()
        {
            var result = new PlayerProfile();
            result.Storage = new StorageState { Cols = 12, Rows = 20 };
            result.EnsureStorage(ItemCatalog.LoadFromResources());
            return result;
        }
    }
}
