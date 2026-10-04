using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Popups;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class Uf2EconomyPrefabTests
    {
        private const string CommonDropdown = "Assets/_Game/Prefabs/UI/Common/Dropdown.prefab";

        [Test]
        public void ExchangeOptionsUseMaterialNames()
        {
            // Arrange
            TextAsset data = Resources.Load<TextAsset>("Data/materials");
            MaterialCatalog catalog = MaterialCatalog.Parse(data.text);
            MethodInfo method = typeof(ShopPopup).GetMethod(
                "BuildMaterialOptions",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Act
            var options = method.Invoke(null, new object[] { catalog, ItemIconLibrary.Load() })
                as List<Dropdown.OptionData>;

            // Assert
            Assert.That(options, Is.Not.Null);
            Assert.That(options.Count, Is.GreaterThan(0));
            Assert.That(options[0].text, Is.EqualTo(FirstExchangeMaterial(catalog).Name));
            Assert.That(options[0].text, Is.Not.EqualTo(FirstExchangeMaterial(catalog).Id));
        }

        [TestCase("UI/Popups/SectSelectPopup")]
        [TestCase("UI/Popups/StoragePopup")]
        [TestCase("UI/Popups/ShopPopup")]
        [TestCase("UI/Popups/DevCheatPanel")]
        public void EconomyDropdownsAreLinkedCommonPrefabInstances(string resourcePath)
        {
            // Arrange
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            GameObject common = AssetDatabase.LoadAssetAtPath<GameObject>(CommonDropdown);

            // Act
            Dropdown[] dropdowns = prefab.GetComponentsInChildren<Dropdown>(true);

            // Assert
            Assert.That(dropdowns.Length, Is.GreaterThan(0));
            foreach (Dropdown dropdown in dropdowns)
            {
                Object source = PrefabUtility.GetCorrespondingObjectFromSource(dropdown.gameObject);
                Assert.That(source, Is.EqualTo(common), dropdown.name);
            }
        }

        private static MaterialInfo FirstExchangeMaterial(MaterialCatalog catalog)
        {
            foreach (MaterialInfo material in catalog.All)
            {
                if (!material.IsCatalyst)
                    return material;
            }

            Assert.Fail("The material catalog has no exchange material.");
            return null;
        }
    }
}
