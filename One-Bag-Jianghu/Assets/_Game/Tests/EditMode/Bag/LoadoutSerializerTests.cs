using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class LoadoutSerializerTests
    {
        private ItemCatalog catalog;
        [SetUp]
        public void SetUp()
        {
            string catalogPath =
                Path.Combine(Application.dataPath,
                "_Game/Resources/Data/items.json");
            catalog = ItemCatalog.Parse(File.ReadAllText(catalogPath));
        }

        [Test]
        public void ReadsServerSample()
        {
            string path =
                Path.Combine(Application.dataPath,
                "_Game/Tests/EditMode/Data/loadout_thiet_phu.json");
            BagGrid grid = LoadoutSerializer.FromJson(File.ReadAllText(path), catalog);
            Assert.That(grid.Items.Count, Is.EqualTo(6));
            Assert.That(grid.UsedCells, Is.EqualTo(18));
            Assert.That(grid.Items.All(item => item.Tier == 1), Is.True);
        }

        [Test]
        public void RoundTripKeepsIdsPositionsAndTiers()
        {
            var original = new BagGrid(5, 4);
            original.Place(catalog.Get("khai_son_phu"), 0, 0, 3);
            original.Place(catalog.Get("boi_nguyen_dan"), 3, 1, 9);
            string json = LoadoutSerializer.ToJson(original, "Round Trip", 1);
            BagGrid restored = LoadoutSerializer.FromJson(json, catalog);
            Assert.That(restored.Items.Select(Value),
                Is.EqualTo(original.Items.Select(Value)));
        }

        [Test]
        public void InvalidLoadoutNamesProblemItem()
        {
            const string json =
                "{\"name\":\"bad\",\"level\":1,\"cols\":5,\"rows\":4," +
                "\"items\":[{\"id\":\"khai_son_phu\",\"x\":4,\"y\":3,\"tier\":1}]}";
            FormatException exception =
                Assert.Throws<FormatException>(() =>
                LoadoutSerializer.FromJson(json,
                catalog));
            StringAssert.Contains("khai_son_phu", exception.Message);
        }

        [Test]
        public void RoundTripKeepsSectAndTamPhapLevel()
        {
            var original = new BagGrid(5, 4);
            var meta = new LoadoutMeta
            {
                Sect = "thai_thanh",
                TamPhapLevel = 7
            };
            string json = LoadoutSerializer.ToJson(original, "Sect", 1, meta);
            LoadoutSerializer.FromJson(json, catalog, out LoadoutMeta restored);
            Assert.That(restored.Sect, Is.EqualTo("thai_thanh"));
            Assert.That(restored.TamPhapLevel, Is.EqualTo(7));
        }

        [Test]
        public void OldJsonDefaultsToNoSectAndLevelOne()
        {
            const string json = "{\"name\":\"old\",\"level\":1,\"cols\":5,\"rows\":4,\"items\":[]}";
            LoadoutSerializer.FromJson(json, catalog, out LoadoutMeta meta);
            Assert.That(meta.Sect, Is.Null);
            Assert.That(meta.TamPhapLevel, Is.EqualTo(1));
        }

        private static string Value(PlacedItem item) =>
            $"{item.Item.Id}:{item.X}:{item.Y}:{item.Tier}";
    }
}