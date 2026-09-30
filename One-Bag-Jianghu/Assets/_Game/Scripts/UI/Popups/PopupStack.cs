using System.Collections.Generic;

namespace OneBagJianghu.UI.Popups
{
    public sealed class PopupStack
    {
        public const int BaseSortingOrder = 100;
        private readonly List<BasePopup> items = new List<BasePopup>();

        public int Count => items.Count;
        public BasePopup Top => items.Count == 0 ? null : items[items.Count - 1];
        public IReadOnlyList<BasePopup> Items => items;

        public void Push(BasePopup popup)
        {
            Remove(popup);
            items.Add(popup);
        }

        public bool Remove(BasePopup popup)
        {
            return items.Remove(popup);
        }

        public void Clear()
        {
            items.Clear();
        }

        public static int PopupSortingOrder(int index)
        {
            return BaseSortingOrder + 2 * index + 1;
        }

        public static int BackdropSortingOrder(int index)
        {
            return BaseSortingOrder + 2 * index;
        }
    }
}
