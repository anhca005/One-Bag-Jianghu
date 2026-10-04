using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace OneBagJianghu.Bag.View
{
    public sealed class BagGridView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform cellsLayer;
        [SerializeField]
        private RectTransform highlightLayer;
        [SerializeField]
        private RectTransform itemLayer;
        [SerializeField]
        private ItemView itemPrefab;
        [SerializeField]
        private Image highlightPrefab;
        private readonly List<Image> highlights = new List<Image>();
        private readonly Dictionary<int, ItemView> itemViews = new Dictionary<int, ItemView>();
        private RectTransform rectTransform;
        private ItemTrayView tray;
        private SectInfo sect;
        public BagGrid Grid
        {
            get;
            private set;
        }

        public Action<ItemInfo, string, int> ItemSelected;
        public Func<PlacedItem, string> TooltipText;
        public Func<PlacedItem, string> Rarity;
        public OneBagJianghu.UI.Common.ItemTooltip Tooltip;
        public void Initialize(BagGrid grid, ItemTrayView trayView)
        {
            tray = trayView;
            rectTransform = GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.sizeDelta = new Vector2(grid.Cols * BagViewStyle.Pitch - BagViewStyle.Gap,
                grid.Rows * BagViewStyle.Pitch - BagViewStyle.Gap);
            // Legacy demo scenes can still initialize an empty component. Production prefabs
            // have all three layers and the 5x4 cells serialized by BagPrefabBuilder.
            if (cellsLayer == null)
            {
                cellsLayer = CreateLayer("Cells");
                BuildCells(grid.Cols, grid.Rows, cellsLayer);
                highlightLayer = CreateLayer("Highlights");
                itemLayer = CreateLayer("Items");
                itemPrefab = Resources.Load<ItemView>("UI/Bag/ItemView");
            }

            SetGrid(grid);
        }

        public void SetGrid(BagGrid grid)
        {
            if (Grid != null)
                Grid.Changed -= Refresh;
            Grid = grid;
            Grid.Changed += Refresh;
            Refresh();
        }

        public bool ScreenToCell(Vector2 screenPoint,
            Camera camera,
            int itemWidth,
            int itemHeight,
            out int x,
            out int y)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                screenPoint,
                camera,
                out Vector2 local))
            {
                x = y = -1;
                return false;
            }

            x = DragGridMath.OriginCell(local.x, BagViewStyle.Pitch, itemWidth);
            y = DragGridMath.OriginCell(-local.y, BagViewStyle.Pitch, itemHeight);
            return true;
        }

        public bool ContainsScreenPoint(Vector2 point,
            Camera camera) => RectTransformUtility.RectangleContainsScreenPoint(rectTransform,
            point,
            camera);
        public void ShowTarget(ItemInfo item, int x, int y, int ignoreInstanceId)
        {
            ClearTarget();
            bool valid = Grid.CanPlace(item, x, y, ignoreInstanceId);
            Color color = valid ? BagViewStyle.Valid : BagViewStyle.Invalid;
            for (int row = 0; row < item.Height; row++)
                for (int col = 0; col < item.Width; col++)
                {
                    // Cells hanging past the bag edge get no highlight, so nothing is drawn outside the grid.
                    int cellX = x + col;
                    int cellY = y + row;
                    if (cellX < 0 || cellY < 0 || cellX >= Grid.Cols || cellY >= Grid.Rows)
                        continue;
                    Image image = Instantiate(highlightPrefab, highlightLayer);
                    image.color = color;
                    SetCellRect(image.rectTransform, cellX, cellY, 1, 1);
                    highlights.Add(image);
                }

            var candidate = new GridRect(x, y, item.Width, item.Height);
            if (!valid)
                return;
            foreach (PlacedItem placed in Grid.Items)
            {
                if (placed.InstanceId != ignoreInstanceId
                    && candidate.IsAdjacent(placed.Rect)
                    && itemViews.TryGetValue(placed.InstanceId,
                    out ItemView view))
                    view.SetAdjacent(true);
            }
        }

        public void ClearTarget()
        {
            foreach (Image image in highlights)
                if (image != null)
                    Destroy(image.gameObject);
            highlights.Clear();
            foreach (ItemView view in itemViews.Values)
                if (view != null)
                    view.SetAdjacent(false);
        }

        public void Refresh()
        {
            ClearTarget();
            foreach (Transform child in itemLayer)
                Destroy(child.gameObject);
            itemViews.Clear();
            foreach (PlacedItem placed in Grid.Items)
            {
                ItemView view = Instantiate(itemPrefab, itemLayer);
                view.InitializePlaced(this, tray, placed, Rarity?.Invoke(placed));
                view.SetTooltip(Tooltip, TooltipText?.Invoke(placed));
                view.Selected = ItemSelected;
                view.SetTamPhapBuffed(sect != null && sect.Buffs(placed.Item.WeaponClass));
                SetCellRect(view.RectTransform, placed.X, placed.Y, placed.Item.Width, placed.Item.Height);
                itemViews.Add(placed.InstanceId, view);
            }
        }

        public void SetSect(SectInfo selectedSect)
        {
            sect = selectedSect;
            Refresh();
        }

        private void BuildCells(int cols, int rows, RectTransform layer)
        {
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                {
                    Image cell = CreateImage(layer,
                        $"Cell {x},{y}",
                        new Color(BagViewStyle.Ink.r,
                        BagViewStyle.Ink.g,
                        BagViewStyle.Ink.b,
                        0.10f));
                    SetCellRect(cell.rectTransform, x, y, 1, 1);
                }
        }

        private RectTransform CreateLayer(string layerName)
        {
            var go = new GameObject(layerName, typeof(RectTransform));
            var layer = go.GetComponent<RectTransform>();
            layer.SetParent(rectTransform, false);
            layer.anchorMin = new Vector2(0f, 1f);
            layer.anchorMax = new Vector2(0f, 1f);
            layer.pivot = new Vector2(0f, 1f);
            layer.anchoredPosition = Vector2.zero;
            layer.sizeDelta = rectTransform.sizeDelta;
            return layer;
        }

        public static void SetCellRect(RectTransform rect, int x, int y, int width, int height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x * BagViewStyle.Pitch, -y * BagViewStyle.Pitch);
            rect.sizeDelta = new Vector2(width * BagViewStyle.Pitch - BagViewStyle.Gap,
                height * BagViewStyle.Pitch - BagViewStyle.Gap);
        }

        internal static Image CreateImage(Transform parent, string objectName, Color color)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }
    }
}