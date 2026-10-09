using NUnit.Framework;
using OneBagJianghu.Battle.Replay.View;
using UnityEngine;

namespace OneBagJianghu.Tests.Replay
{
    public sealed class ReplayItemTooltipTests
    {
        [Test]
        public void BoundItemBuildsTooltipContainingCatalogName()
        {
            // Arrange
            var root = new GameObject("Replay item");
            ReplayItemView view = root.AddComponent<ReplayItemView>();

            // Act
            view.BindItemData("thanh_phong_kiem", "rare", 2);
            string value = view.TooltipText;

            // Assert
            Assert.IsNotEmpty(value);
            StringAssert.Contains("Thanh Phong Kiếm", value);
            Object.DestroyImmediate(root);
        }
    }
}
