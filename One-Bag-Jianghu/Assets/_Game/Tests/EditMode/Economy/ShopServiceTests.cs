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
        private sealed class Clock : IShopClock { public DateTimeOffset Now; public DateTimeOffset UtcNow => Now; }
        private sealed class Items : IShopItemCatalog
        {
            public IReadOnlyList<string> AllItemIds { get; } = new[] { "a", "b", "c" };
            public string GetItemType(string id) => "weapon";
            public IReadOnlyList<int> GetUnlockTiers(string id) => Array.Empty<int>();
        }
        private ShopCatalog config; private MaterialCatalog materials; private Clock clock;
        [SetUp] public void SetUp() { config = ShopCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath, "_Game/Resources/Data/shop.json"))); materials = MaterialCatalog.Parse(File.ReadAllText(Path.Combine(Application.dataPath, "_Game/Resources/Data/materials.json"))); clock = new Clock { Now = new DateTimeOffset(2026, 9, 30, 1, 0, 0, TimeSpan.Zero) }; }
        private ShopService Service(int seed = 7) => new ShopService(config, materials, new Items(), seed, clock);
        private static PlayerProfile Profile(long silver = 10000, long reputation = 2000) { var p = new PlayerProfile(); if (silver > 0) p.AddCurrency("bac", silver); if (reputation > 0) p.AddCurrency("danh_vong", reputation); return p; }

        [Test] public void SlotGenerationIsDeterministicForSeedAndClock()
        { PlayerProfile a = Profile(), b = Profile(); Service(42).Ensure(a); Service(42).Ensure(b); Assert.That(a.Shop.Slots.Select(x => x.ContentId + x.Rarity + x.Price), Is.EqualTo(b.Shop.Slots.Select(x => x.ContentId + x.Rarity + x.Price))); }

        [Test] public void RefreshUsesSixHoursAndDailyCostLadder()
        { PlayerProfile p = Profile(); ShopService s = Service(); s.Ensure(p); Assert.That(s.Refresh(p, true).Success, Is.False); Assert.That(s.PaidRefreshCost(p), Is.EqualTo(100)); Assert.That(s.Refresh(p, false).Success, Is.True); Assert.That(s.PaidRefreshCost(p), Is.EqualTo(200)); clock.Now = clock.Now.AddHours(6); Assert.That(s.Refresh(p, true).Success, Is.True); clock.Now = clock.Now.AddDays(1); Assert.That(s.PaidRefreshCost(p), Is.EqualTo(100)); }

        [Test] public void BuyIsAtomicAndEachSlotCanOnlyBeBoughtOnce()
        { PlayerProfile p = Profile(0); ShopService s = Service(); s.Ensure(p); Assert.That(s.Buy(p, 0).Success, Is.False); Assert.That(p.Items, Is.Empty); p.AddCurrency("bac", 10000); Assert.That(s.Buy(p, 0).Success, Is.True); int count = p.Items.Count; Assert.That(s.Buy(p, 0).Success, Is.False); Assert.That(p.Items.Count, Is.EqualTo(count)); }

        [Test] public void ExchangeSupportsSameAndCrossGroupOnly()
        { PlayerProfile p = Profile(); p.AddMaterial("han_thiet", 3); p.AddMaterial("o_thiet", 2); ShopService s = Service(); Assert.That(s.Exchange(p, "han_thiet", "o_thiet").Success, Is.True); Assert.That(s.Exchange(p, "o_thiet", "o_kim_ti").Success, Is.True); Assert.That(s.Exchange(p, "han_thiet", "cuu_u_tam_ti").Success, Is.False); }

        [Test] public void ReputationLimitsResetOnMonday()
        { PlayerProfile p = Profile(); ShopService s = Service(); Assert.That(s.BuyReputation(p, "chi_ton_ngoc_tuy").Success, Is.True); Assert.That(s.BuyReputation(p, "chi_ton_ngoc_tuy").Success, Is.False); clock.Now = new DateTimeOffset(2026, 10, 5, 0, 0, 0, TimeSpan.Zero); Assert.That(s.ReputationRemaining(p, "chi_ton_ngoc_tuy"), Is.EqualTo(1)); }

        [Test] public void VersionTwoMigratesToShopState()
        { JObject json = JObject.FromObject(Profile()); json["version"] = 2; json.Remove("shop"); PlayerProfile restored = PlayerProfile.FromJson(json.ToString()); Assert.That(restored.Version, Is.EqualTo(PlayerProfile.CurrentVersion)); Assert.That(restored.Shop, Is.Not.Null); }
    }
}
