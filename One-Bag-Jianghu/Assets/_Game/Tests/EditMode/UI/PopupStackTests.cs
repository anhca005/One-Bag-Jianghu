using NUnit.Framework;
using OneBagJianghu.UI.Popups;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class PopupStackTestPopup : BasePopup
    {
    }

    public sealed class PopupStackTests
    {
        [Test]
        public void PushMovesExistingPopupToTop()
        {
            GameObject firstObject = new GameObject("First", typeof(RectTransform), typeof(CanvasGroup), typeof(PopupStackTestPopup));
            GameObject secondObject = new GameObject("Second", typeof(RectTransform), typeof(CanvasGroup), typeof(PopupStackTestPopup));
            try
            {
                PopupStackTestPopup first = firstObject.GetComponent<PopupStackTestPopup>();
                PopupStackTestPopup second = secondObject.GetComponent<PopupStackTestPopup>();
                var stack = new PopupStack();
                stack.Push(first);
                stack.Push(second);
                stack.Push(first);
                Assert.That(stack.Count, Is.EqualTo(2));
                Assert.That(stack.Top, Is.SameAs(first));
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
            }
        }

        [Test]
        public void RemoveAndClearUpdateTop()
        {
            GameObject popupObject = new GameObject("Popup", typeof(RectTransform), typeof(CanvasGroup), typeof(PopupStackTestPopup));
            try
            {
                PopupStackTestPopup popup = popupObject.GetComponent<PopupStackTestPopup>();
                var stack = new PopupStack();
                stack.Push(popup);
                Assert.That(stack.Remove(popup), Is.True);
                Assert.That(stack.Top, Is.Null);
                stack.Push(popup);
                stack.Clear();
                Assert.That(stack.Count, Is.Zero);
            }
            finally
            {
                Object.DestroyImmediate(popupObject);
            }
        }

        [TestCase(0, 101, 100)]
        [TestCase(1, 103, 102)]
        [TestCase(4, 109, 108)]
        public void SortingOrdersUseTwoSlotsPerLevel(int index, int popupOrder, int backdropOrder)
        {
            Assert.That(PopupStack.PopupSortingOrder(index), Is.EqualTo(popupOrder));
            Assert.That(PopupStack.BackdropSortingOrder(index), Is.EqualTo(backdropOrder));
        }
    }
}
