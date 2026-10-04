using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class HoverTooltipTargetTests
    {
        [Test]
        public void PointerEnterRequestsText()
        {
            // Arrange
            var root = new GameObject("Tooltip Target");
            HoverTooltipTarget target = root.AddComponent<HoverTooltipTarget>();
            bool requested = false;
            target.Initialize(null, () =>
            {
                requested = true;
                return "Chi tiết";
            });

            // Act
            target.OnPointerEnter(null);

            // Assert
            Assert.That(requested, Is.True);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void ShopCardSetTooltipAddsTarget()
        {
            // Arrange
            var root = new GameObject("Shop Card");
            ShopCard card = root.AddComponent<ShopCard>();

            // Act
            card.SetTooltip(null, () => "Chi tiết");

            // Assert
            Assert.That(root.GetComponent<HoverTooltipTarget>(), Is.Not.Null);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void PointerExitHidesTooltipPanel()
        {
            // Arrange
            var root = new GameObject("Tooltip Target");
            HoverTooltipTarget target = root.AddComponent<HoverTooltipTarget>();
            var tooltipRoot = new GameObject("Tooltip");
            ItemTooltip tooltip = tooltipRoot.AddComponent<ItemTooltip>();
            var panelRoot = new GameObject("Panel", typeof(RectTransform));
            panelRoot.transform.SetParent(tooltipRoot.transform);
            SetField(tooltip, "panel", panelRoot.GetComponent<RectTransform>());
            panelRoot.SetActive(true);
            target.Initialize(tooltip, () => "Chi tiết");

            // Act
            target.OnPointerExit(null);

            // Assert
            Assert.That(panelRoot.activeSelf, Is.False);
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(tooltipRoot);
        }

        private static void SetField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
