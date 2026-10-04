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

        [Test]
        public void SlicedAutoFitLimitsSmallBordersAndKeepsLargeDefault()
        {
            // Arrange
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Assets/_Game/Textures2D/UI/button_primary.png");
            GameObject root = new GameObject("Sliced test", typeof(RectTransform), typeof(CanvasRenderer));
            RectTransform rect = root.GetComponent<RectTransform>();
            Image image = root.AddComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            SlicedAutoFit fit = root.AddComponent<SlicedAutoFit>();

            // Act
            rect.sizeDelta = new Vector2(300f, 40f);
            fit.Configure(6f);
            float smallMultiplier = image.pixelsPerUnitMultiplier;
            rect.sizeDelta = new Vector2(2000f, 1000f);
            fit.Configure(6f);

            // Assert
            float vertical = (sprite.border.y + sprite.border.w) * 100f;
            vertical /= sprite.pixelsPerUnit * smallMultiplier;
            Assert.That(vertical, Is.LessThanOrEqualTo(36.01f));
            Assert.That(image.pixelsPerUnitMultiplier, Is.EqualTo(6f).Within(0.001f));
            Object.DestroyImmediate(root);
        }
    }
}
