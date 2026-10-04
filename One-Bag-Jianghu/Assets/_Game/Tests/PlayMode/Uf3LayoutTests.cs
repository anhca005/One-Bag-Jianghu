using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.PlayMode
{
    public sealed class Uf3LayoutTests
    {
        [Test]
        public void BagStorageUsesClippedScrollViewport()
        {
            // Arrange
            GameObject prefab = Resources.Load<GameObject>("UI/Popups/BagPopup");
            Transform tray = prefab.transform.Find("Panel/BagHost/BagLoadoutPanel/ItemTray");

            // Act
            ScrollRect scroll = tray.GetComponent<ScrollRect>();
            RectMask2D mask = scroll.viewport.GetComponent<RectMask2D>();

            // Assert
            Assert.That(scroll.vertical, Is.True);
            Assert.That(mask, Is.Not.Null);
        }

        [Test]
        public void PveStageCopyStaysInsideBoardWithoutOverlap()
        {
            // Arrange
            GameObject prefab = Resources.Load<GameObject>("UI/Popups/StageMapPopup");
            RectTransform board = FindRect(prefab.transform, "Stage Detail");
            RectTransform title = FindRect(board, "Name");
            RectTransform enemy = FindRect(board, "Enemy");
            RectTransform description = FindRect(board, "Description");

            // Act
            Canvas.ForceUpdateCanvases();

            // Assert
            AssertInside(board, title);
            AssertInside(board, enemy);
            AssertInside(board, description);
            Assert.That(Overlaps(title, enemy), Is.False);
            Assert.That(Overlaps(enemy, description), Is.False);
        }

        private static RectTransform FindRect(Transform root, string name)
        {
            foreach (RectTransform rect in root.GetComponentsInChildren<RectTransform>(true))
            {
                if (rect.name == name)
                    return rect;
            }

            Assert.Fail("Missing rect " + name);
            return null;
        }

        private static void AssertInside(RectTransform outer, RectTransform inner)
        {
            var outerCorners = new Vector3[4];
            var innerCorners = new Vector3[4];
            outer.GetWorldCorners(outerCorners);
            inner.GetWorldCorners(innerCorners);
            for (int i = 0; i < innerCorners.Length; i++)
            {
                Assert.That(innerCorners[i].x, Is.InRange(outerCorners[0].x, outerCorners[2].x));
                Assert.That(innerCorners[i].y, Is.InRange(outerCorners[0].y, outerCorners[2].y));
            }
        }

        private static bool Overlaps(RectTransform left, RectTransform right)
        {
            var leftCorners = new Vector3[4];
            var rightCorners = new Vector3[4];
            left.GetWorldCorners(leftCorners);
            right.GetWorldCorners(rightCorners);
            Rect leftRect = Rect.MinMaxRect(
                leftCorners[0].x,
                leftCorners[0].y,
                leftCorners[2].x,
                leftCorners[2].y);
            Rect rightRect = Rect.MinMaxRect(
                rightCorners[0].x,
                rightCorners[0].y,
                rightCorners[2].x,
                rightCorners[2].y);
            return leftRect.Overlaps(rightRect);
        }
    }
}
