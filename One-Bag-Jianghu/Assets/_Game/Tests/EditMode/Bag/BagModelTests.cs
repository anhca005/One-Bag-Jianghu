using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class BagModelTests
    {
        private ItemCatalog catalog;
        [SetUp]
        public void SetUp()
        {
            string path = Path.Combine(Application.dataPath, "_Game/Resources/Data/items.json");
            catalog = ItemCatalog.Parse(File.ReadAllText(path));
        }

        [Test]
        public void CatalogParsesRealFileAndIgnoresExtraFields()
        {
            Assert.That(catalog.All.Count, Is.GreaterThan(0));
            Assert.That(catalog.Get("khai_son_phu").Width, Is.EqualTo(2));
            Assert.That(catalog.Get("khai_son_phu").Height, Is.EqualTo(2));
            Assert.That(catalog.Get("phuc_ho_con").Width, Is.EqualTo(1));
            Assert.That(catalog.Get("phuc_ho_con").Height, Is.EqualTo(4));
        }

        [Test]
        public void RejectsOutOfBoundsAndOverlap()
        {
            var grid = new BagGrid(5, 4);
            ItemInfo axe = catalog.Get("khai_son_phu");
            grid.Place(axe, 0, 0);
            Assert.That(grid.CanPlace(axe, -1, 0), Is.False);
            Assert.That(grid.CanPlace(axe, 4, 0), Is.False);
            Assert.That(grid.CanPlace(axe, 1, 1), Is.False);
            Assert.Throws<InvalidOperationException>(() => grid.Place(axe, 1, 1));
        }

        [Test]
        public void MoveIgnoresTheMovingItem()
        {
            var grid = new BagGrid(5, 4);
            PlacedItem item = grid.Place(catalog.Get("khai_son_phu"), 0, 0);
            Assert.That(grid.TryMove(item.InstanceId, 0, 0), Is.True);
            Assert.That(grid.TryMove(item.InstanceId, 3, 2), Is.True);
            Assert.That(item.X, Is.EqualTo(3));
            Assert.That(item.Y, Is.EqualTo(2));
        }

        [Test]
        public void AdjacencyRequiresSharedEdge()
        {
            var source = new GridRect(0, 0, 2, 2);
            Assert.That(source.IsAdjacent(new GridRect(2,
                2,
                1,
                1)),
                Is.False,
                "Corner contact");
            Assert.That(source.IsAdjacent(new GridRect(2,
                1,
                1,
                2)),
                Is.True,
                "Right edge");
            Assert.That(source.IsAdjacent(new GridRect(1,
                2,
                2,
                1)),
                Is.True,
                "Bottom edge");
        }

        [Test]
        public void NeighboursUsesRectangleAdjacency()
        {
            var grid = new BagGrid(5, 4);
            var oneByOne = new ItemInfo("test", "Test", "relic", null, null, "common", 1, 1);
            PlacedItem source = grid.Place(oneByOne, 1, 1);
            PlacedItem edge = grid.Place(oneByOne, 2, 1);
            grid.Place(oneByOne, 2, 2);
            Assert.That(grid.Neighbours(source.InstanceId).Select(item =>
                item.InstanceId),
                Is.EquivalentTo(new[] { edge.InstanceId }));
        }
    }
}