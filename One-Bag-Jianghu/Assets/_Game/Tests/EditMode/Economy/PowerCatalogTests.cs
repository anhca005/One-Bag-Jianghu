using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using UnityEngine;
using UnityEngine.TestTools;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class PowerCatalogTests
    {
        private const string SyntheticJson = @"{
            'version': 1,
            'levelBase': 10,
            'levelPoints': 3,
            'recommendedFactor': 1.1,
            'tierFactor': { '1': 1, '2': 1.5 },
            'rarityFactor': { 'common': 1, 'rare': 2 },
            'items': { 'sword': 40, 'armor': 20 }
        }";
        private const string SyntheticV2Json = @"{
            'version': 2,
            'tierMultiplier': { '1': 1 },
            'rarityMultiplier': { 'common': 1 },
            'levelBaseDefense': 100,
            'levelDefensePerLevel': 5,
            'scale': 1,
            'referenceKit': [ 'sword' ],
            'items': {
                'sword': { 'offense': 9, 'defense': 0 },
                'armor': { 'offense': 0, 'defense': 44 }
            }
        }";

        [Test]
        public void FighterUsesFormula()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticJson);
            var items = new[]
            {
                Item("sword", 1, "common"),
                Item("armor", 1, "common")
            };

            // Act
            int power = catalog.Fighter(items, 1);

            // Assert
            Assert.AreEqual(70, power);
        }

        [Test]
        public void ItemPowerAppliesTierAndRarityFactors()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticJson);

            // Act
            int power = catalog.ItemPower("sword", 2, "rare");

            // Assert
            Assert.AreEqual(120, power);
        }

        [Test]
        public void UnknownItemFallsBackToTwentyFive()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticJson);
            LogAssert.Expect(LogType.Warning, "[Power] Unknown item id 'missing'; using base power 25.");

            // Act
            int power = catalog.ItemPower("missing", 1, "common");

            // Assert
            Assert.AreEqual(25, power);
        }

        [Test]
        public void FighterAddsLevelPoints()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticJson);

            // Act
            int power = catalog.Fighter(new OwnedItem[0], 4);

            // Assert
            Assert.AreEqual(19, power);
        }

        [Test]
        public void VersionTwoFighterUsesHybridFormula()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticV2Json);
            var items = new[] { Item("sword", 1, "common"), Item("armor", 1, "common") };

            // Act
            int power = catalog.Fighter(items, 1);

            // Assert: round(1 * sqrt(9 offense * (100 + 44) defense)) = 36.
            Assert.AreEqual(36, power);
        }

        [Test]
        public void VersionTwoItemPowerIsMarginalOverReferenceKit()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.Parse(SyntheticV2Json);

            // Act
            int power = catalog.ItemPower("armor", 1, "common");

            // Assert: reference is 30, and reference plus armor is 36.
            Assert.AreEqual(6, power);
            Assert.Greater(power, 0);
        }

        [Test]
        public void RealPowerDataStarterKitIsOneHundred()
        {
            // Arrange
            PowerCatalog catalog = PowerCatalog.LoadFromResources();
            var items = new[]
            {
                Item("thanh_phong_kiem", 1, "common"),
                Item("o_tam_y", 1, "common")
            };

            // Act
            int power = catalog.Fighter(items, 1);

            // Assert
            Assert.AreEqual(100, power);
        }

        [Test]
        public void RealPowerDataContainsEveryCatalogItem()
        {
            // Arrange
            ItemCatalog items = ItemCatalog.LoadFromResources();
            TextAsset asset = Resources.Load<TextAsset>("Data/power");
            JObject data = JObject.Parse(asset.text);
            JObject powers = (JObject)data["items"];
            var missing = new List<string>();

            // Act
            foreach (ItemInfo item in items.All)
                if (powers[item.Id] == null)
                    missing.Add(item.Id);

            // Assert
            Assert.IsEmpty(missing, "Missing item ids: " + string.Join(", ", missing));
        }

        private static OwnedItem Item(string id, int tier, string rarity)
        {
            return new OwnedItem
            {
                ItemId = id,
                Tier = tier,
                Rarity = rarity
            };
        }
    }
}
