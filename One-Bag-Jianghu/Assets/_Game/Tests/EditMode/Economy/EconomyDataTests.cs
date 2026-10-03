using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.Tests.Economy
{
    public sealed class EconomyDataTests
    {
        [Test]
        public void EconomyCatalogsParseAndReferencesExist()
        {
            MaterialCatalog materials = MaterialCatalog.Parse(Load("materials"));
            Assert.AreEqual(17, materials.All.Count);
            Assert.IsTrue(materials.Get("chi_ton_ngoc_tuy").PvpOnly);
            UpgradeCatalog upgrades = UpgradeCatalog.Parse(Load("upgrade_costs"));
            MergeCatalog merges = MergeCatalog.Parse(Load("merge"));
            RewardCatalog.Parse(Load("rewards"));
            var ids = new HashSet<string>();
            foreach (MaterialInfo material in materials.All)
            {
                ids.Add(material.Id);
            }

            foreach (MergeStep step in merges.Steps)
            {
                foreach (string id in step.ExtraMaterials.Keys)
                {
                    Assert.IsTrue(ids.Contains(id), id);
                }
            }

            foreach (string rarity in RarityRules.Ordered)
            {
                for (int tier = 1; tier <= 8; tier++)
                {
                    Assert.IsNotNull(upgrades.Get(rarity, tier));
                }
            }
        }

        private static string Load(string name)
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/" + name);
            Assert.IsNotNull(asset);
            return asset.text;
        }
    }
}