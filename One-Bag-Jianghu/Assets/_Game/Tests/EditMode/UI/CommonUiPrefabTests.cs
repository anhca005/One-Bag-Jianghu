using NUnit.Framework;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class CommonUiPrefabTests
    {
        [Test]
        public void DropdownTemplateHasVisibleSizedItem()
        {
            // Arrange
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_Game/Prefabs/UI/Common/Dropdown.prefab");
            Dropdown dropdown = prefab.GetComponent<Dropdown>();
            Transform viewport = dropdown.template.Find("Viewport");
            Transform content = viewport.Find("Content");
            Toggle item = content.Find("Item").GetComponent<Toggle>();
            Text label = item.transform.Find("Item Label").GetComponent<Text>();

            // Act
            Color background = item.targetGraphic.color;

            // Assert
            Assert.That(item.GetComponent<RectTransform>().sizeDelta.y, Is.GreaterThan(0f));
            Assert.That(label.font, Is.Not.Null);
            Assert.That(label.color.a, Is.GreaterThan(0f));
            Assert.That(label.color, Is.Not.EqualTo(background));
            Assert.That(item.graphic, Is.Not.Null);
        }

    }
}
