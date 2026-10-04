using NUnit.Framework;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class MaterialStackTests
    {
        [Test]
        public void MergeStacksCombinesPartialStacksOfTheSameMaterial()
        {
            var profile = new PlayerProfile();
            var grid = new StorageGrid(profile, null);
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId =
                    "han_thiet",
                Count =
                    5,
                X =
                    0,
                Y =
                    0
            });
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId =
                    "o_kim_ti",
                Count =
                    2,
                X =
                    1,
                Y =
                    0
            });
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId =
                    "han_thiet",
                Count =
                    7,
                X =
                    2,
                Y =
                    0
            });
            grid.MergeStacks();
            Assert.AreEqual(2, profile.Storage.MaterialStacks.Count);
            Assert.AreEqual(12, grid.MaterialCount("han_thiet"));
            Assert.AreEqual(12, profile.Materials["han_thiet"]);
            MaterialStack first = profile.Storage.MaterialStacks[0];
            Assert.AreEqual("han_thiet", first.MaterialId);
            Assert.AreEqual(0, first.X);
        }

        [Test]
        public void MergeStacksSplitsAtTheStackLimit()
        {
            var profile = new PlayerProfile();
            var grid = new StorageGrid(profile, null);
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId =
                    "han_thiet",
                Count =
                    600,
                X =
                    0,
                Y =
                    0
            });
            profile.Storage.MaterialStacks.Add(new MaterialStack
            {
                MaterialId =
                    "han_thiet",
                Count =
                    600,
                X =
                    1,
                Y =
                    0
            });
            grid.MergeStacks();
            Assert.AreEqual(2, profile.Storage.MaterialStacks.Count);
            Assert.AreEqual(StorageGrid.MaxStack, profile.Storage.MaterialStacks[0].Count);
            Assert.AreEqual(1200 - StorageGrid.MaxStack,
                profile.Storage.MaterialStacks[1].Count);
        }

        [Test]
        public void ClearingStacksAndMaterialsBeforeRebuildDoesNotDuplicate()
        {
            // Mirrors Tàng Bảo Các "Sắp xếp": totals are re-added after clearing both lists.
            var profile = new PlayerProfile();
            new StorageGrid(profile, null).TryAddMaterial("han_thiet", 5);
            int total = new StorageGrid(profile, null).MaterialCount("han_thiet");
            profile.Storage.MaterialStacks.Clear();
            profile.Materials.Clear();
            var rebuilt = new StorageGrid(profile, null);
            rebuilt.TryAddMaterial("han_thiet", total);
            Assert.AreEqual(5, rebuilt.MaterialCount("han_thiet"));
            Assert.AreEqual(1, profile.Storage.MaterialStacks.Count);
        }
    }
}