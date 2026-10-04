using NUnit.Framework;
using OneBagJianghu.Bag.View;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class RarityBadgeTests
    {
        [TestCase("common", "rarity_common")]
        [TestCase("uncommon", "rarity_uncommon")]
        [TestCase("rare", "rarity_rare")]
        [TestCase("epic", "rarity_epic")]
        [TestCase("legendary", "rarity_legendary")]
        [TestCase("supreme", "rarity_supreme")]
        [TestCase("unknown", null)]
        public void SpriteNameMapsSupportedRarities(string rarity, string expected)
        {
            // Act
            string result = RarityBadge.SpriteName(rarity);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("Assets/_Game/Prefabs/UI/Bag/ItemView.prefab")]
        [TestCase("Assets/_Game/Resources/UI/Popups/StorageItemView.prefab")]
        [TestCase("Assets/_Game/Resources/UI/Popups/ShopCard.prefab")]
        public void BuiltPrefabBadgeDoesNotReceiveRaycasts(string path)
        {
            // Arrange
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // Act
            Transform badge = prefab == null ? null : prefab.transform.Find("Rarity Badge");

            // Assert
            Assert.That(prefab, Is.Not.Null, path);
            Assert.That(badge, Is.Not.Null, path);
            Assert.That(badge.GetComponent<Image>().raycastTarget, Is.False, path);
        }
    }
}
