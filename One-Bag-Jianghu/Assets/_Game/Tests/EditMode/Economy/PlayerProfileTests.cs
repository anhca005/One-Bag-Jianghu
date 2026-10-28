using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class PlayerProfileTests
    {
        private string temporaryDirectory;
        private StarterKit starter;
        [SetUp]
        public void SetUp()
        {
            temporaryDirectory =
                Path.Combine(Path.GetTempPath(),
                "OneBagJianghuTests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temporaryDirectory);
            starter = StarterKit.Parse(File.ReadAllText(DataPath("starter.json")));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, true);
            }
        }

        [Test]
        public void SpendingIsAtomicWhenAnyBalanceIsInsufficient()
        {
            var profile = new PlayerProfile();
            profile.AddCurrency("bac", 100);
            profile.AddCurrency("danh_vong", 5);
            profile.AddMaterial("han_thiet", 4);
            bool paid =
                profile.TrySpend(new Dictionary<string,
                long>
                {
                    ["bac"] =
                        50,
                    ["danh_vong"] =
                        6
                },
                    out string currencyError);
            bool consumed =
                profile.SpendMaterials(new Dictionary<string,
                int>
                {
                    ["han_thiet"] =
                        3,
                    ["missing"] =
                        1
                },
                    out string materialError);
            Assert.That(paid, Is.False);
            Assert.That(currencyError, Does.Contain("Danh Vọng"));
            Assert.That(profile.Currencies["bac"], Is.EqualTo(100));
            Assert.That(profile.Currencies["danh_vong"], Is.EqualTo(5));
            Assert.That(consumed, Is.False);
            Assert.That(materialError, Does.Contain("Không đủ"));
            Assert.That(profile.Materials["han_thiet"], Is.EqualTo(4));
        }

        [Test]
        public void AddAndRemoveItemAlsoRemovesItsPlacement()
        {
            var profile = new PlayerProfile();
            OwnedItem item = profile.AddItem("thanh_phong_kiem", 3, "test", "item-1");
            profile.Loadout.Add(new LoadoutPlacement { Uid = item.Uid, X = 0, Y = 0 });
            profile.RemoveItem(item.Uid);
            Assert.That(profile.Items, Is.Empty);
            Assert.That(profile.Loadout, Is.Empty);
            Assert.Throws<KeyNotFoundException>(() => profile.RemoveItem("missing"));
        }

        [Test]
        public void JsonRoundTripKeepsProfileState()
        {
            PlayerProfile original = starter.CreateProfile("Round Trip", "thai_thanh");
            original.HeroLevel = 12;
            original.HeroExp = 3456;
            original.TamPhapLevel = 6;
            original.AddMaterial("han_thiet", 9);
            PlayerProfile restored = PlayerProfile.FromJson(original.ToJson());
            Assert.That(restored.HeroName, Is.EqualTo("Round Trip"));
            Assert.That(restored.HeroLevel, Is.EqualTo(12));
            Assert.That(restored.Items.Select(item =>
                item.Uid),
                Is.EqualTo(original.Items.Select(item => item.Uid)));
            Assert.That(restored.Loadout.Select(item =>
                item.Uid),
                Is.EqualTo(original.Loadout.Select(item => item.Uid)));
            Assert.That(restored.Materials["han_thiet"], Is.EqualTo(9));
        }

        [Test]
        public void StarterKitHasWeaponAndExpectedCurrencies()
        {
            PlayerProfile profile = starter.CreateProfile();
            ItemCatalog catalog = LoadItems();
            Assert.That(profile.Currencies.Keys,
                Is.EquivalentTo(new[] { "bac",
                "nguyen_bao",
                "danh_vong" }));
            Assert.That(profile.Loadout.Select(place =>
                profile.RequireItem(place.Uid)).Any(item =>
                catalog.Get(item.ItemId).Type == "weapon"),
                Is.True);
        }

        [Test]
        public void EngineLoadoutUsesTierFromOwnedItem()
        {
            PlayerProfile profile = starter.CreateProfile("Tier Owner");
            // The starter bag is empty (the tutorial has the player place the kit), so equip the sword here.
            OwnedItem first = profile.Items.First(value => value.ItemId == "thanh_phong_kiem");
            profile.Loadout.Add(new LoadoutPlacement { Uid = first.Uid, X = 0, Y = 0 });
            first.Tier = 7;
            JObject json = JObject.Parse(GameSession.BuildEngineLoadoutJson(profile, LoadItems()));
            JToken item = json["items"].First(value => (string)value["id"] == first.ItemId);
            Assert.That((int)item["tier"], Is.EqualTo(7));
            Assert.That((string)item["rarity"], Is.EqualTo("common"));
            Assert.That((int)json["level"], Is.EqualTo(profile.HeroLevel));
            Assert.That(json["hp"], Is.Null);
            Assert.That(json["items"].Any(value =>
                value["uid"] != null),
                Is.False,
                "Engine input schema must not expose owned uids.");
        }

        [Test]
        public void MaterialCatalogRejectsUnknownFields()
        {
            string valid = File.ReadAllText(DataPath("materials.json"));
            Assert.That(MaterialCatalog.Parse(valid).All.Count, Is.EqualTo(17));
            const string invalid =
                "[{\"id\":\"x\",\"name\":\"X\",\"itemType\":\"weapon\"," +
                "\"band\":\"tier_1_3\",\"description\":\"x\",\"unexpected\":true}]";
            Assert.Throws<FormatException>(() => MaterialCatalog.Parse(invalid));
        }

        [Test]
        public void ProfileRejectsUnknownFields()
        {
            string json = starter.CreateProfile().ToJson();
            JObject document = JObject.Parse(json);
            document["unexpected"] = true;
            Assert.Throws<FormatException>(() => PlayerProfile.FromJson(document.ToString()));
        }

        private static string DataPath(string file) =>
            Path.Combine(Application.dataPath,
            "_Game/Resources/Data",
            file);
        private static ItemCatalog LoadItems() =>
            ItemCatalog.Parse(File.ReadAllText(DataPath("items.json")));
    }
}
