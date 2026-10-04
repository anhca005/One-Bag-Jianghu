using NUnit.Framework;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class StorageGridTests
    {
        private sealed class Sizes : IStorageItemCatalog
        {
            public void GetItemSize(string id, out int width, out int height)
            {
                width = id == "wide" ? 2 : 1;
                height = id == "tall" ? 3 : 1;
            }
        }

        [Test]
        public void FirstFitUsesRealSizeRowMajorWithoutRotation()
        {
            var profile = new PlayerProfile();
            var grid = new StorageGrid(profile, new Sizes());
            OwnedItem wide = profile.AddItem("wide", 1, uid: "a");
            OwnedItem tall = profile.AddItem("tall", 1, uid: "b");
            Assert.That(grid.TryPlaceItem(wide), Is.True);
            Assert.That(grid.TryPlaceItem(tall), Is.True);
            Assert.That(profile.Storage.Items[0].X, Is.Zero);
            Assert.That(profile.Storage.Items[0].Y, Is.Zero);
            Assert.That(profile.Storage.Items[1].X, Is.EqualTo(2));
            Assert.That(profile.Storage.Items[1].Y, Is.Zero);
        }

        [Test]
        public void MaterialsSplitAt999AndSpendingRemovesEmptyStacks()
        {
            var profile = new PlayerProfile();
            var grid = new StorageGrid(profile, new Sizes());
            Assert.That(grid.TryAddMaterial("ore", 1000), Is.True);
            Assert.That(profile.Storage.MaterialStacks.Count, Is.EqualTo(2));
            Assert.That(profile.Materials["ore"], Is.EqualTo(1000));
            Assert.That(grid.SpendMaterials(new System.Collections.Generic.Dictionary<string,
                int> { { "ore",
                1 } },
                out _),
                Is.True);
            Assert.That(profile.Storage.MaterialStacks.Count, Is.EqualTo(1));
            Assert.That(profile.Materials["ore"], Is.EqualTo(999));
        }

        [Test]
        public void InvalidMoveDoesNotChangePlacement()
        {
            var profile = new PlayerProfile();
            var grid = new StorageGrid(profile, new Sizes());
            OwnedItem item = profile.AddItem("wide", 1, uid: "a");
            grid.TryPlaceItem(item);
            Assert.That(grid.TryMoveItem("a", 9, 0), Is.False);
            Assert.That(profile.Storage.Items[0].X, Is.Zero);
        }
    }
}