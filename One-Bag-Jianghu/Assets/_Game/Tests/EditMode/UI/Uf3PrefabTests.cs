using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class Uf3PrefabTests
    {
        [TestCase("Assets/_Game/Prefabs/UI/Bag/ItemView.prefab")]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/ReplayItemView.prefab")]
        public void ItemPrefabContainsRarityFrame(string path)
        {
            // Arrange
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // Act
            Transform frame = prefab.transform.Find("Rarity Frame");

            // Assert
            Assert.That(frame, Is.Not.Null, path);
            Assert.That(frame.GetComponent<Image>(), Is.Not.Null, path);
        }

        [Test]
        public void ReplayItemHidesNameWhenIconExists()
        {
            // Arrange
            ItemIconLibrary library = ItemIconLibrary.Load();
            ItemIconLibrary.Entry entry = library.Entries.Find(candidate => candidate.Sprite != null);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_Game/Prefabs/UI/Replay/ReplayItemView.prefab");
            GameObject instance = Object.Instantiate(prefab);
            ReplayItemView view = instance.GetComponent<ReplayItemView>();

            // Act
            view.Initialize(1, entry.ItemId, "Tên dài", 1, "rare");
            Text nameText = instance.transform.Find("Name").GetComponent<Text>();

            // Assert
            Assert.That(nameText.gameObject.activeSelf, Is.False);
            Assert.That(nameText.fontSize, Is.LessThanOrEqualTo(14));
            Object.DestroyImmediate(instance);
        }
    }
}
