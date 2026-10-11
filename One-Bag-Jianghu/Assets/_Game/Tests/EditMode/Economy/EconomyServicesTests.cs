using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class EconomyServicesTests
    {
        private sealed class Items : IUpgradeItemCatalog
        {
            public string GetItemType(string itemId) => "weapon";
            public IReadOnlyList<int> GetUnlockTiers(string itemId) => new[]
            {
                3,
                6,
                9
            };
        }

        private static MaterialCatalog Materials() =>
            MaterialCatalog.Parse("[" +
            "{\"id\":\"m1\",\"name\":\"M1\",\"itemType\":\"weapon\",\"band\":\"t1\"," +
            "\"tier\":1,\"isCatalyst\":false,\"pvpOnly\":false,\"description\":\"\"}," +
            "{\"id\":\"m2\",\"name\":\"M2\",\"itemType\":\"weapon\",\"band\":\"t2\"," +
            "\"tier\":2,\"isCatalyst\":false,\"pvpOnly\":false,\"description\":\"\"}," +
            "{\"id\":\"m3\",\"name\":\"M3\",\"itemType\":\"weapon\",\"band\":\"t3\"," +
            "\"tier\":3,\"isCatalyst\":false,\"pvpOnly\":false,\"description\":\"\"}," +
            "{\"id\":\"cat\",\"name\":\"Cat\",\"itemType\":\"weapon\",\"band\":\"cat\"," +
            "\"tier\":0,\"isCatalyst\":true,\"pvpOnly\":false,\"description\":\"\"}," +
            "{\"id\":\"chi_ton_ngoc_tuy\",\"name\":\"Ngọc\",\"itemType\":\"universal\"," +
            "\"band\":\"pvp\",\"tier\":0,\"isCatalyst\":true,\"pvpOnly\":true," +
            "\"description\":\"\"}]");
        private static UpgradeCatalog Upgrades()
        {
            var catalog = new UpgradeCatalog();
            foreach (string rarity in RarityRules.Ordered)
            {
                var steps = new List<UpgradeStep>();
                for (int tier = 1; tier <= 8; tier++)
                {
                    steps.Add(new UpgradeStep
                    {
                        FromTier =
                            tier,
                        MaterialTier =
                            tier < 3 ?
                            1 :
                            tier < 6 ?
                            2 :
                            3,
                        MaterialCount =
                            tier,
                        CatalystCount =
                            tier == 2 ?
                            1 :
                            0,
                        Silver =
                            tier * 100
                    });
                }

                catalog.Steps[rarity] = steps;
            }

            return catalog;
        }

        private static MergeCatalog Merges() =>
            MergeCatalog.Parse("{\"steps\":[{\"fromRarity\":\"common\",\"toRarity\":\"uncommon\",\"silver\":500," +
            "\"extraMaterials\":{}},{\"fromRarity\":\"uncommon\",\"toRarity\":\"rare\"," +
            "\"silver\":1500,\"extraMaterials\":{}},{\"fromRarity\":\"rare\"," +
            "\"toRarity\":\"epic\",\"silver\":4000,\"extraMaterials\":{}}," +
            "{\"fromRarity\":\"epic\",\"toRarity\":\"legendary\",\"silver\":10000," +
            "\"extraMaterials\":{}},{\"fromRarity\":\"legendary\",\"toRarity\":\"supreme\"," +
            "\"silver\":25000,\"extraMaterials\":{\"chi_ton_ngoc_tuy\":5}}]}");
        private static PlayerProfile Rich()
        {
            var profile = new PlayerProfile();
            profile.Currencies["bac"] = 100000;
            foreach (string id in new[]
            {
                "m1",
                "m2",
                "m3",
                "cat",
                "chi_ton_ngoc_tuy"
            }

            )
            {
                profile.Materials[id] = 1000;
            }

            return profile;
        }

        [Test]
        public void UpgradeIsAtomicAndStopsAtNine()
        {
            PlayerProfile profile = Rich();
            OwnedItem item = profile.AddItem("sword", 2, uid: "a");
            var service = new UpgradeService(Upgrades(), Materials(), new Items());
            UpgradePreview preview = service.Preview(profile, "a");
            Assert.IsTrue(preview.Success);
            Assert.IsTrue(preview.UnlocksBreakthrough);
            Assert.IsTrue(service.Apply(profile, "a").Success);
            Assert.AreEqual(3, item.Tier);
            item.Tier = 9;
            Assert.IsFalse(service.Apply(profile, "a").Success);
            Assert.AreEqual(9, item.Tier);
        }

        [Test]
        public void MergeValidatesFormulaAndRemovesInputs()
        {
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 4, uid: "a");
            profile.AddItem("sword", 8, uid: "b");
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());
            MergePreview preview = service.Preview(profile, "a", "b");
            Assert.AreEqual(4, preview.OutputTier);
            Assert.IsTrue(service.Apply(profile, "a", "b").Success);
            Assert.IsNull(profile.FindItem("a"));
            Assert.IsNull(profile.FindItem("b"));
            Assert.AreEqual("uncommon", profile.Items[0].Rarity);
        }

        [Test]
        public void UpgradeKeepsItemInLoadout()
        {
            // Arrange
            PlayerProfile profile = Rich();
            OwnedItem equipped = profile.AddItem("sword", 2, uid: "equipped");
            profile.Loadout.Add(new LoadoutPlacement { Uid = equipped.Uid, X = 2, Y = 3 });
            var service = new UpgradeService(Upgrades(), Materials(), new Items());

            // Act
            EconomyResult result = service.Apply(profile, equipped.Uid);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, equipped.Tier);
            Assert.AreEqual(equipped.Uid, profile.Loadout[0].Uid);
            Assert.AreSame(equipped, profile.FindItem(equipped.Uid));
        }

        [Test]
        public void MergeCandidatesIncludeEquippedUids()
        {
            // Arrange
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "source");
            profile.AddItem("sword", 3, uid: "stored-copy");
            profile.AddItem("sword", 3, uid: "equipped-copy");
            profile.Loadout.Add(new LoadoutPlacement { Uid = "equipped-copy" });
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());

            // Act
            IReadOnlyList<OwnedItem> candidates = service.Candidates(profile, "source");

            // Assert
            Assert.AreEqual(2, candidates.Count);
            Assert.AreEqual("stored-copy", candidates[0].Uid);
            Assert.AreEqual("equipped-copy", candidates[1].Uid);
        }

        [Test]
        public void MergeEquippedSourceKeepsItsLoadoutPosition()
        {
            // Arrange
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "equipped");
            profile.AddItem("sword", 3, uid: "stored");
            profile.Loadout.Add(new LoadoutPlacement { Uid = "equipped", X = 2, Y = 4 });
            profile.Storage.Items.Add(new StorageItemPlacement { Uid = "stored", X = 0, Y = 0 });
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());

            // Act
            EconomyResult result = service.Apply(profile, "equipped", "stored");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("equipped", profile.Loadout[0].Uid);
            Assert.AreEqual(2, profile.Loadout[0].X);
            Assert.AreEqual(4, profile.Loadout[0].Y);
            Assert.AreEqual("uncommon", profile.RequireItem("equipped").Rarity);
            Assert.IsNull(profile.FindItem("stored"));
            Assert.IsEmpty(profile.Storage.Items);
        }

        [Test]
        public void MergeEquippedCandidateKeepsItsLoadoutPosition()
        {
            // Arrange
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "stored");
            profile.AddItem("sword", 3, uid: "equipped");
            profile.Storage.Items.Add(new StorageItemPlacement { Uid = "stored", X = 0, Y = 0 });
            profile.Loadout.Add(new LoadoutPlacement { Uid = "equipped", X = 1, Y = 3 });
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());

            // Act
            EconomyResult result = service.Apply(profile, "stored", "equipped");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("equipped", profile.Loadout[0].Uid);
            Assert.AreEqual(1, profile.Loadout[0].X);
            Assert.AreEqual(3, profile.Loadout[0].Y);
            Assert.AreEqual("uncommon", profile.RequireItem("equipped").Rarity);
            Assert.IsNull(profile.FindItem("stored"));
            Assert.IsEmpty(profile.Storage.Items);
        }

        [Test]
        public void MergeTwoEquippedCopiesKeepsSourceAndFreesOneSlot()
        {
            // Arrange
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "first");
            profile.AddItem("sword", 3, uid: "second");
            profile.Loadout.Add(new LoadoutPlacement { Uid = "first", X = 2, Y = 2 });
            profile.Loadout.Add(new LoadoutPlacement { Uid = "second", X = 5, Y = 5 });
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());

            // Act
            EconomyResult result = service.Apply(profile, "first", "second");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, profile.Loadout.Count);
            Assert.AreEqual("first", profile.Loadout[0].Uid);
            Assert.AreEqual(2, profile.Loadout[0].X);
            Assert.AreEqual(2, profile.Loadout[0].Y);
            Assert.AreEqual("uncommon", profile.RequireItem("first").Rarity);
            Assert.IsNull(profile.FindItem("second"));
        }

        [Test]
        public void MergeEquippedSourceSucceedsWhenStorageIsFull()
        {
            // Arrange
            PlayerProfile profile = Rich();
            profile.Storage.Cols = 1;
            profile.Storage.Rows = 1;
            profile.AddItem("sword", 3, uid: "first");
            profile.AddItem("sword", 3, uid: "second");
            profile.AddItem("shield", 1, uid: "blocker");
            profile.Loadout.Add(new LoadoutPlacement { Uid = "first", X = 0, Y = 0 });
            profile.Loadout.Add(new LoadoutPlacement { Uid = "second", X = 1, Y = 0 });
            profile.Storage.Items.Add(new StorageItemPlacement { Uid = "blocker", X = 0, Y = 0 });
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());

            // Act
            EconomyResult result = service.Apply(profile, "first", "second");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("first", profile.Loadout[0].Uid);
            Assert.AreEqual("uncommon", profile.RequireItem("first").Rarity);
            Assert.AreEqual("blocker", profile.Storage.Items[0].Uid);
        }

        [Test]
        public void MergeRejectsEveryStructuralMismatch()
        {
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "a");
            profile.AddItem("axe", 3, uid: "b");
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());
            Assert.IsFalse(service.Preview(profile, "a", "a").Success);
            Assert.IsFalse(service.Preview(profile, "a", "b").Success);
            profile.RequireItem("b").ItemId = "sword";
            profile.RequireItem("b").Rarity = "rare";
            Assert.IsFalse(service.Preview(profile, "a", "b").Success);
            profile.RequireItem("b").Rarity = "common";
            profile.RequireItem("b").Tier = 2;
            Assert.IsFalse(service.Preview(profile, "a", "b").Success);
            profile.RequireItem("a").Rarity = profile.RequireItem("b").Rarity = "supreme";
            profile.RequireItem("b").Tier = 3;
            Assert.IsFalse(service.Preview(profile, "a", "b").Success);
        }

        [Test]
        public void LegendaryMergeRequiresFivePvpJadeAndRefundNeverNegative()
        {
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "a", rarity: "legendary");
            profile.AddItem("sword", 3, uid: "b", rarity: "legendary");
            profile.Materials["chi_ton_ngoc_tuy"] = 4;
            var service = new MergeService(Upgrades(), Merges(), Materials(), new Items());
            Assert.IsFalse(service.Preview(profile, "a", "b").Success);
            profile.Materials["chi_ton_ngoc_tuy"] = 5;
            MergePreview preview = service.Preview(profile, "a", "b");
            Assert.IsTrue(preview.Success);
            foreach (int value in preview.RefundMaterials.Values)
            {
                Assert.GreaterOrEqual(value, 0);
            }
        }

        [Test]
        public void SellRemovesEquippedCopy()
        {
            PlayerProfile profile = Rich();
            profile.AddItem("sword", 3, uid: "a");
            profile.Loadout.Add(new LoadoutPlacement { Uid = "a" });
            Assert.IsTrue(new SellService(Upgrades(),
                Materials(),
                new Items()).Apply(profile,
                "a").Success);
            Assert.IsNull(profile.FindItem("a"));
            Assert.IsEmpty(profile.Loadout);
        }

        [Test]
        public void RewardIsDeterministic()
        {
            RewardCatalog rules =
                RewardCatalog.Parse("{\"win\":{\"silver\":1,\"materialTier\":1,\"materialCount\":1}," +
                "\"loss\":{\"silver\":0,\"materialTier\":1,\"materialCount\":1}," +
                "\"firstClear\":{\"silver\":2,\"materialTier\":1,\"materialCount\":1}}");
            BattleReward a =
                new RewardService(rules,
                Materials()).Grant(new PlayerProfile(),
                true,
                false,
                42);
            BattleReward b =
                new RewardService(rules,
                Materials()).Grant(new PlayerProfile(),
                true,
                false,
                42);
            CollectionAssert.AreEquivalent(a.Materials, b.Materials);
        }

        [Test]
        public void VersionOneMigratesAndLoadoutRarityCanBeRepresented()
        {
            PlayerProfile profile =
                PlayerProfile.FromJson("{\"version\":1,\"heroName\":\"A\",\"heroLevel\":1,\"heroExp\":0," +
                "\"sectId\":null," +
                "\"tamPhapLevel\":1,\"currencies\":{},\"materials\":{},\"items\":[{\"uid\":\"a\"," +
                "\"itemId\":\"sword\",\"tier\":1,\"acquiredAt\":\"x\"}],\"loadout\":[]}");
            Assert.AreEqual(PlayerProfile.CurrentVersion, profile.Version);
            Assert.AreEqual("common", profile.Items[0].Rarity);
            Assert.AreEqual("common",
                JObject.Parse("{\"rarity\":\"common\"}").Value<string>("rarity"));
        }
    }
}
