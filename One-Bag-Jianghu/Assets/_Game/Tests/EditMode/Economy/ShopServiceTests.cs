using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class ShopServiceTests
    {
        private sealed class Clock : IShopClock
        {
            public DateTimeOffset Now;
            public DateTimeOffset UtcNow => Now;
        }

        private sealed class Items : IShopItemCatalog
        {
            private readonly string itemType;

            public Items(string itemType = "weapon")
            {
                this.itemType = itemType;
            }

            public IReadOnlyList<string> AllItemIds { get; } = new[]
            {
                "a",
                "b",
                "c"
            };

            public string GetItemType(string id) => itemType;
            public IReadOnlyList<int> GetUnlockTiers(string id) => Array.Empty<int>();
        }

        private ShopCatalog config;
        private MaterialCatalog materials;
        private Clock clock;
        [SetUp]
        public void SetUp()
        {
            config =
                ShopCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
                "_Game/Resources/Data/shop.json")));
            materials =
                MaterialCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
                "_Game/Resources/Data/materials.json")));
            clock = new Clock
            {
                Now = new DateTimeOffset(2026, 9, 30, 1, 0, 0, TimeSpan.Zero)
            };
        }

        private ShopService Service(int seed = 7,
            string itemType = "weapon") =>
            new ShopService(config,
            materials,
            new Items(itemType),
            seed,
            clock);

        private void ConfigureCommonItemSlots(int discountPercent = 0)
        {
            foreach (ShopSlotRule slot in config.Slots)
            {
                slot.Type = "item";
                slot.Rarities = new[] { "common" };
                slot.Weights = new[] { 1 };
                slot.DiscountPercent = discountPercent;
            }
        }
        private static PlayerProfile Profile(long silver = 10000, long reputation = 2000)
        {
            var p = new PlayerProfile();
            if (silver > 0)
            {
                p.AddCurrency("bac", silver);
            }

            if (reputation > 0)
            {
                p.AddCurrency("danh_vong", reputation);
            }

            return p;
        }

        [Test]
        public void SlotGenerationIsDeterministicForSeedAndClock()
        {
            PlayerProfile a = Profile(), b = Profile();
            Service(42).Ensure(a);
            Service(42).Ensure(b);
            Assert.That(a.Shop.Slots.Select(x =>
                x.ContentId +
                x.Rarity +
                x.Price),
                Is.EqualTo(b.Shop.Slots.Select(x => x.ContentId + x.Rarity + x.Price)));
        }

        [Test]
        public void PaidRefreshAlwaysCostsFifty()
        {
            // Arrange
            PlayerProfile profile = Profile();
            ShopService shop = Service();

            // Act
            EconomyResult first = shop.Refresh(profile, false);
            EconomyResult second = shop.Refresh(profile, false);
            EconomyResult third = shop.Refresh(profile, false);

            // Assert
            Assert.That(first.Success, Is.True);
            Assert.That(second.Success, Is.True);
            Assert.That(third.Success, Is.True);
            Assert.That(profile.Currencies["bac"], Is.EqualTo(9850));
            Assert.That(shop.PaidRefreshCost(profile), Is.EqualTo(50));
        }

        [Test]
        public void FreshProfileGetsExactlyOneStarterFreeRefresh()
        {
            // Arrange
            PlayerProfile profile = Profile();
            ShopService shop = Service();

            // Act
            bool freeAtStart = shop.CanRefreshFree(profile);
            EconomyResult free = shop.Refresh(profile, true);
            TimeSpan untilNextFree = shop.UntilFreeRefresh(profile);
            EconomyResult secondFree = shop.Refresh(profile, true);
            EconomyResult paid = shop.Refresh(profile, false);

            // Assert: the starter refresh is ready at once, then the timed one counts from its use.
            Assert.That(freeAtStart, Is.True);
            Assert.That(free.Success, Is.True);
            Assert.That(untilNextFree, Is.EqualTo(TimeSpan.FromHours(3)));
            Assert.That(secondFree.Success, Is.False);
            Assert.That(paid.Success, Is.True);
            Assert.That(profile.Shop.StarterFreeRefreshesUsed, Is.EqualTo(1));
            Assert.That(profile.Currencies["bac"], Is.EqualTo(9950));
        }

        [Test]
        public void BuyIsAtomicAndEachSlotCanOnlyBeBoughtOnce()
        {
            PlayerProfile p = Profile(0);
            ShopService s = Service();
            s.Ensure(p);
            Assert.That(s.Buy(p, 0).Success, Is.False);
            Assert.That(p.Items, Is.Empty);
            p.AddCurrency("bac", 10000);
            Assert.That(s.Buy(p, 0).Success, Is.True);
            int count = p.Items.Count;
            Assert.That(s.Buy(p, 0).Success, Is.False);
            Assert.That(p.Items.Count, Is.EqualTo(count));
        }

        [Test]
        public void ExchangeSupportsSameAndCrossGroupOnly()
        {
            PlayerProfile p = Profile();
            p.AddMaterial("han_thiet", 3);
            p.AddMaterial("o_thiet", 2);
            ShopService s = Service();
            Assert.That(s.Exchange(p, "han_thiet", "o_thiet").Success, Is.True);
            Assert.That(s.Exchange(p, "o_thiet", "o_kim_ti").Success, Is.True);
            Assert.That(s.Exchange(p, "han_thiet", "cuu_u_tam_ti").Success, Is.False);
        }

        [Test]
        public void ReputationLimitsResetOnMonday()
        {
            PlayerProfile p = Profile();
            ShopService s = Service();
            Assert.That(s.BuyReputation(p, "chi_ton_ngoc_tuy").Success, Is.True);
            Assert.That(s.BuyReputation(p, "chi_ton_ngoc_tuy").Success, Is.False);
            clock.Now = new DateTimeOffset(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);
            Assert.That(s.ReputationRemaining(p, "chi_ton_ngoc_tuy"), Is.EqualTo(1));
        }

        [Test]
        public void VersionTwoMigratesToShopState()
        {
            JObject json = JObject.FromObject(Profile());
            json["version"] = 2;
            json.Remove("shop");
            PlayerProfile restored = PlayerProfile.FromJson(json.ToString());
            Assert.That(restored.Version, Is.EqualTo(PlayerProfile.CurrentVersion));
            Assert.That(restored.Shop, Is.Not.Null);
        }

        [Test]
        public void CurrentProfileWithoutStarterCounterGetsOneFreeRefresh()
        {
            // Arrange
            JObject json = JObject.Parse(Profile().ToJson());
            ((JObject)json["shop"]).Remove("starterFreeRefreshesUsed");
            PlayerProfile restored = PlayerProfile.FromJson(json.ToString());

            // Act
            EconomyResult result = Service().Refresh(restored, true);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(restored.Shop.StarterFreeRefreshesUsed, Is.EqualTo(1));
        }

        [Test]
        public void ItemPriceUsesMultiplierForWeaponOnly()
        {
            // Arrange
            ConfigureCommonItemSlots();
            PlayerProfile weaponProfile = Profile();
            PlayerProfile armorProfile = Profile();

            // Act
            Service(itemType: "weapon").Ensure(weaponProfile);
            Service(itemType: "armor").Ensure(armorProfile);

            // Assert
            Assert.That(weaponProfile.Shop.Slots[0].Price, Is.EqualTo(400));
            Assert.That(armorProfile.Shop.Slots[0].Price, Is.EqualTo(200));
        }

        [Test]
        public void ItemDiscountAppliesAfterWeaponMultiplier()
        {
            // Arrange
            ConfigureCommonItemSlots(20);
            PlayerProfile profile = Profile();

            // Act
            Service().Ensure(profile);

            // Assert: slot 1 (index 0) of the very first shop is the fixed beginner offer, so check slot 2.
            Assert.That(profile.Shop.Slots[1].Price, Is.EqualTo(320));
        }

        [Test]
        public void FirstShopOffersBeginnerManualOnlyOnce()
        {
            // Arrange
            PlayerProfile profile = Profile();
            ShopService shop = Service(itemType: "manual");

            // Act
            shop.Ensure(profile);
            ShopSlotState first = profile.Shop.Slots[0];
            shop.ForceFreeRefresh(profile);

            // Assert
            Assert.That(first.ContentId, Is.EqualTo(ShopService.FirstShopItemId));
            Assert.That(first.Rarity, Is.EqualTo("common"));
            Assert.That(first.Price, Is.EqualTo(200));
            Assert.That(profile.Shop.Generation, Is.EqualTo(2));
        }

        [Test]
        public void MissingItemTypeMultipliersUsesBasePrice()
        {
            // Arrange
            JObject json = JObject.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
                "_Game/Resources/Data/shop.json")));
            json.Remove("itemTypePriceMultipliers");
            config = ShopCatalog.Parse(json.ToString());
            ConfigureCommonItemSlots();
            PlayerProfile profile = Profile();

            // Act
            Service().Ensure(profile);

            // Assert
            Assert.That(profile.Shop.Slots[0].Price, Is.EqualTo(200));
        }

        [Test]
        public void CatalogRejectsNonPositiveItemTypeMultiplier()
        {
            // Arrange
            JObject json = JObject.Parse(File.ReadAllText(Path.Combine(Application.dataPath,
                "_Game/Resources/Data/shop.json")));
            json["itemTypePriceMultipliers"]["weapon"] = 0;

            // Act
            TestDelegate parse = () => ShopCatalog.Parse(json.ToString());

            // Assert
            Assert.Throws<FormatException>(parse);
        }
    }
}
