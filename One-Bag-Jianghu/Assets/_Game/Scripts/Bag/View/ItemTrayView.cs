using System.Collections.Generic;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Bag.View
{
    public sealed class ItemTrayView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private ItemView itemPrefab;
        private RectTransform rectTransform;
        private BagGridView gridView;
        private readonly List<ItemView> itemViews = new List<ItemView>();
        private readonly List<GameObject> decorationViews = new List<GameObject>();
        private ItemIconLibrary materialIcons;
        private readonly List<TrayItem> source = new List<TrayItem>();
        private SectInfo sect;
        private bool spatial;
        private PlayerProfile draftProfile;
        private StorageGrid storageGrid;
        private float spatialPitch = 52f;
        public float CellPitch => spatial ? spatialPitch : BagViewStyle.Pitch;
        public bool IsSpatial => spatial;
        public System.Action<ItemInfo, string, int> ItemSelected;
        public System.Func<TrayItem, string> TooltipText;
        public OneBagJianghu.UI.Common.ItemTooltip Tooltip;

        public void Initialize(ItemCatalog catalog, BagGridView owner)
        { ResetSource(); gridView = owner; rectTransform = GetComponent<RectTransform>(); foreach (ItemInfo item in catalog.All) source.Add(new TrayItem(item, null, 1)); Rebuild(); }

        public void Initialize(IReadOnlyList<TrayItem> ownedItems, BagGridView owner)
        { ResetSource(); gridView = owner; rectTransform = GetComponent<RectTransform>(); source.AddRange(ownedItems); gridView.Grid.Changed += Rebuild; Rebuild(); }

        public void InitializeStorage(PlayerProfile profile, ItemCatalog catalog, BagGridView owner)
        {
            ResetSource(); spatial = true; gridView = owner; rectTransform = GetComponent<RectTransform>();
            draftProfile = new PlayerProfile { Storage = new StorageState { Cols = profile.Storage.Cols, Rows = profile.Storage.Rows } };
            draftProfile.Items.AddRange(profile.Items);
            foreach (StorageItemPlacement p in profile.Storage.Items) draftProfile.Storage.Items.Add(new StorageItemPlacement { Uid = p.Uid, X = p.X, Y = p.Y });
            foreach (MaterialStack s in profile.Storage.MaterialStacks) draftProfile.Storage.MaterialStacks.Add(new MaterialStack { MaterialId = s.MaterialId, Count = s.Count, X = s.X, Y = s.Y });
            storageGrid = new StorageGrid(draftProfile, catalog);
            foreach (OwnedItem owned in profile.Items) if (catalog.TryGet(owned.ItemId, out ItemInfo info)) source.Add(new TrayItem(info, owned.Uid, owned.Tier));
            gridView.Grid.Changed += ReconcileStorage; ReconcileStorage();
        }

        public void ApplyStorage(StorageState target)
        {
            if (!spatial) return; target.Items.Clear();
            foreach (StorageItemPlacement p in draftProfile.Storage.Items) target.Items.Add(new StorageItemPlacement { Uid = p.Uid, X = p.X, Y = p.Y });
            target.MaterialStacks.Clear();
            foreach (MaterialStack s in draftProfile.Storage.MaterialStacks) target.MaterialStacks.Add(new MaterialStack { MaterialId = s.MaterialId, Count = s.Count, X = s.X, Y = s.Y });
        }

        public bool TryAccept(PlacedItem placed, Vector2 point, Camera camera)
        {
            if (!spatial || placed == null || string.IsNullOrEmpty(placed.OwnedUid)) return true;
            if (ToCell(point, camera, out int x, out int y) && storageGrid.CanPlace(x, y, placed.Item.Width, placed.Item.Height))
            { draftProfile.Storage.Items.Add(new StorageItemPlacement { Uid = placed.OwnedUid, X = x, Y = y }); return true; }
            OwnedItem owned = draftProfile.FindItem(placed.OwnedUid);
            if (owned != null && storageGrid.TryPlaceItem(owned)) return true;
            const string error = "Tàng Bảo Các không đủ chỗ.";
            Debug.LogWarning($"[Bag] Bag to storage transfer refused: uid={placed.OwnedUid}, reason={error}");
            PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", error));
            return false;
        }

        public bool TryAcceptAt(PlacedItem placed, int x, int y)
        {
            if (!spatial || placed == null || string.IsNullOrEmpty(placed.OwnedUid)) return false;
            if (!storageGrid.CanPlace(x, y, placed.Item.Width, placed.Item.Height)) return false;
            draftProfile.Storage.Items.Add(new StorageItemPlacement { Uid = placed.OwnedUid, X = x, Y = y });
            return true;
        }

        private void ResetSource()
        { if (gridView != null && gridView.Grid != null) { gridView.Grid.Changed -= Rebuild; gridView.Grid.Changed -= ReconcileStorage; } source.Clear(); spatial = false; }
        private void OnDestroy() => ResetSource();
        private void ReconcileStorage()
        { if (!spatial) return; var inBag = new HashSet<string>(); foreach (PlacedItem p in gridView.Grid.Items) if (!string.IsNullOrEmpty(p.OwnedUid)) inBag.Add(p.OwnedUid); draftProfile.Storage.Items.RemoveAll(p => inBag.Contains(p.Uid)); Rebuild(); }

        private void Rebuild()
        {
            foreach (ItemView view in itemViews) if (view != null) Destroy(view.gameObject);
            foreach (GameObject view in decorationViews) if (view != null) Destroy(view);
            itemViews.Clear(); decorationViews.Clear(); var placedUids = new HashSet<string>();
            foreach (PlacedItem p in gridView.Grid.Items) if (!string.IsNullOrEmpty(p.OwnedUid)) placedUids.Add(p.OwnedUid);
            if (spatial) BuildCells();
            foreach (TrayItem entry in source)
            {
                if (entry.OwnedUid != null && placedUids.Contains(entry.OwnedUid)) continue;
                StorageItemPlacement placement = spatial ? draftProfile.Storage.Items.Find(p => p.Uid == entry.OwnedUid) : null; if (spatial && placement == null) continue;
                ItemView view = Instantiate(itemPrefab, content); view.Bind(entry.Item); float pitch = CellPitch;
                view.RectTransform.sizeDelta = new Vector2(entry.Item.Width * pitch - BagViewStyle.Gap, entry.Item.Height * pitch - BagViewStyle.Gap);
                LayoutElement element = view.GetComponent<LayoutElement>();
                if (spatial) { element.ignoreLayout = true; SetRect(view.RectTransform, placement.X, placement.Y, entry.Item.Width, entry.Item.Height); }
                else { element.preferredHeight = view.RectTransform.sizeDelta.y; element.preferredWidth = view.RectTransform.sizeDelta.x; }
                view.InitializeTray(gridView, this, entry.OwnedUid, entry.Tier); view.Selected = ItemSelected; view.SetTooltip(Tooltip, TooltipText?.Invoke(entry)); view.SetTamPhapBuffed(sect != null && sect.Buffs(view.Item.WeaponClass)); itemViews.Add(view);
            }
            if (spatial) BuildMaterials();
        }

        private void BuildCells()
        {
            spatialPitch = Mathf.Min(80f, Mathf.Max(40f, (rectTransform.rect.width - 20f) / draftProfile.Storage.Cols));
            content.anchorMin = content.anchorMax = new Vector2(0, 1); content.pivot = new Vector2(0, 1); content.sizeDelta = new Vector2(draftProfile.Storage.Cols * spatialPitch, draftProfile.Storage.Rows * spatialPitch);
            VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>(); if (layout != null) layout.enabled = false; ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>(); if (fitter != null) fitter.enabled = false;
            for (int y = 0; y < draftProfile.Storage.Rows; y++) for (int x = 0; x < draftProfile.Storage.Cols; x++)
            { Image cell = BagGridView.CreateImage(content, $"Storage Cell {x},{y}", new Color(BagViewStyle.Ink.r, BagViewStyle.Ink.g, BagViewStyle.Ink.b, .1f)); cell.raycastTarget = false; SetRect(cell.rectTransform, x, y, 1, 1); decorationViews.Add(cell.gameObject); }
        }

        private void BuildMaterials()
        {
            foreach (MaterialStack stack in draftProfile.Storage.MaterialStacks)
            {
                Image tile = BagGridView.CreateImage(content, "Material " + stack.MaterialId, new Color(.42f, .32f, .18f, .42f)); SetRect(tile.rectTransform, stack.X, stack.Y, 1, 1); tile.raycastTarget = true;
                Text label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>(); label.transform.SetParent(tile.transform, false); label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.color = new Color(1, 1, 1, .7f); label.alignment = TextAnchor.MiddleCenter; label.resizeTextForBestFit = true; label.resizeTextMinSize = 8; label.raycastTarget = false;
                RectTransform lr = label.rectTransform; lr.anchorMin = Vector2.zero; lr.anchorMax = Vector2.one; lr.offsetMin = lr.offsetMax = Vector2.zero;
                MaterialInfo material = OneBagJianghu.Core.GameSession.LoadMaterialCatalog().Get(stack.MaterialId);
                if (materialIcons == null) materialIcons = ItemIconLibrary.Load();
                if (materialIcons != null && materialIcons.TryGet(stack.MaterialId, out Sprite sprite))
                {
                    // Dimmed icon with the count in the corner, like Tàng Bảo Các.
                    Image icon = BagGridView.CreateImage(tile.transform, "Icon", new Color(1f, 1f, 1f, .55f)); icon.sprite = sprite; icon.preserveAspect = true; icon.raycastTarget = false;
                    RectTransform ir = icon.rectTransform; ir.anchorMin = Vector2.zero; ir.anchorMax = Vector2.one; ir.offsetMin = ir.offsetMax = Vector2.zero;
                    label.transform.SetAsLastSibling(); label.alignment = TextAnchor.LowerRight; label.text = "×" + stack.Count;
                }
                else label.text = (material != null ? material.Name : stack.MaterialId) + "\n×" + stack.Count;
                MaterialTooltipTarget target = tile.gameObject.AddComponent<MaterialTooltipTarget>();
                target.Initialize(Tooltip, OneBagJianghu.UI.Common.ItemDescriber.DescribeMaterial(OneBagJianghu.Core.GameSession.LoadMaterialCatalog().Get(stack.MaterialId), stack.Count)); decorationViews.Add(tile.gameObject);
            }
        }

        private bool ToCell(Vector2 point, Camera camera, out int x, out int y)
        { if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(content, point, camera, out Vector2 local)) { x = y = -1; return false; } x = Mathf.FloorToInt(local.x / spatialPitch); y = Mathf.FloorToInt(-local.y / spatialPitch); return x >= 0 && y >= 0 && x < draftProfile.Storage.Cols && y < draftProfile.Storage.Rows; }
        private void SetRect(RectTransform rect, int x, int y, int width, int height)
        { rect.anchorMin = rect.anchorMax = new Vector2(0, 1); rect.pivot = new Vector2(0, 1); rect.anchoredPosition = new Vector2(x * spatialPitch, -y * spatialPitch); rect.sizeDelta = new Vector2(width * spatialPitch - BagViewStyle.Gap, height * spatialPitch - BagViewStyle.Gap); }
        public void SetSect(SectInfo value) { sect = value; foreach (ItemView view in itemViews) view.SetTamPhapBuffed(sect != null && sect.Buffs(view.Item.WeaponClass)); }
        public bool ContainsScreenPoint(Vector2 point, Camera camera) => RectTransformUtility.RectangleContainsScreenPoint(rectTransform, point, camera);
    }

    public sealed class MaterialTooltipTarget : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
    {
        private OneBagJianghu.UI.Common.ItemTooltip tooltip; private string text;
        public void Initialize(OneBagJianghu.UI.Common.ItemTooltip value, string description) { tooltip = value; text = description; }
        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData e) => tooltip?.ShowDelayed(text, e);
        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData e) => tooltip?.Hide();
    }

    public sealed class TrayItem
    { public ItemInfo Item { get; } public string OwnedUid { get; } public int Tier { get; } public TrayItem(ItemInfo item, string ownedUid, int tier) { Item = item; OwnedUid = ownedUid; Tier = tier; } }
}
