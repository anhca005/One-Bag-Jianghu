using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace OneBagJianghu.Bag.View
{
    public sealed class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Outline outline;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text nameText;
        [SerializeField] private Text tierText;
        [SerializeField] private Text tamPhapText;
        private ItemInfo info; private PlacedItem placed; private BagGridView gridView; private ItemTrayView tray;
        private string ownedUid; private int itemTier = 1;
        private Canvas canvas; private int grabbedX; private int grabbedY; private bool dragging;
        private Transform dragOriginParent; private int dragOriginSibling;
        public RectTransform RectTransform => (RectTransform)transform;
        public ItemInfo Item => info;
        public Action<ItemInfo, string, int> Selected;
        private OneBagJianghu.UI.Common.ItemTooltip tooltip; private string tooltipText;
        public void SetTooltip(OneBagJianghu.UI.Common.ItemTooltip value, string text) { tooltip = value; tooltipText = text; }
        public void OnPointerEnter(PointerEventData e) => tooltip?.ShowDelayed(tooltipText, e);
        public void OnPointerExit(PointerEventData e) => tooltip?.Hide();

        public void Bind(ItemInfo item)
        {
            info = item; name = item.Id; background.color = BagViewStyle.Fill(item.Rarity);
            outline.effectColor = BagViewStyle.Rarity(item.Rarity); outline.effectDistance = new Vector2(2f, -2f);
            nameText.text = item.Name;
            ItemIconLibrary library = ItemIconLibrary.Load();
            Sprite sprite = null;
            bool hasIcon = library != null && library.TryGet(item.Id, out sprite);
            icon.gameObject.SetActive(hasIcon); icon.sprite = hasIcon ? sprite : null;
            nameText.rectTransform.anchorMax = hasIcon ? new Vector2(1f, .28f) : Vector2.one;
            nameText.fontSize = hasIcon ? 14 : 22; nameText.alignment = hasIcon ? TextAnchor.LowerCenter : TextAnchor.MiddleCenter;
        }
        public void InitializePlaced(BagGridView owner, ItemTrayView trayView, PlacedItem item)
        { Bind(item.Item); gridView = owner; tray = trayView; placed = item; tierText.gameObject.SetActive(true); tierText.text = "T" + item.Tier; }
        public void InitializeTray(BagGridView owner, ItemTrayView trayView, string uid = null, int tier = 1)
        { gridView = owner; tray = trayView; placed = null; ownedUid = uid; itemTier = tier; tierText.gameObject.SetActive(uid != null); tierText.text = "T" + tier; }
        public void OnBeginDrag(PointerEventData e)
        {
            tooltip?.Hide();
            canvas = GetComponentInParent<Canvas>(); RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, e.position, e.pressEventCamera, out Vector2 local);
            float pitch = placed == null && tray != null ? tray.CellPitch : BagViewStyle.Pitch;
            grabbedX = Mathf.Clamp(Mathf.FloorToInt(local.x / pitch), 0, info.Width - 1); grabbedY = Mathf.Clamp(Mathf.FloorToInt(-local.y / pitch), 0, info.Height - 1);
            dragOriginParent = transform.parent; dragOriginSibling = transform.GetSiblingIndex(); transform.SetParent(canvas.transform, true); transform.SetAsLastSibling(); canvasGroup.blocksRaycasts = false; dragging = true; Follow(e);
        }
        public void OnDrag(PointerEventData e) { if (!dragging) return; Follow(e); gridView.ScreenToCell(e.position, e.pressEventCamera, grabbedX, grabbedY, out int x, out int y); gridView.ShowTarget(info, x, y, placed?.InstanceId ?? -1); }
        public void OnEndDrag(PointerEventData e)
        {
            if (!dragging) return; dragging = false; canvasGroup.blocksRaycasts = true; gridView.ClearTarget();
            bool overGrid = gridView.ContainsScreenPoint(e.position, e.pressEventCamera); gridView.ScreenToCell(e.position, e.pressEventCamera, grabbedX, grabbedY, out int x, out int y);
            if (overGrid && gridView.Grid.CanPlace(info, x, y, placed?.InstanceId ?? -1)) { if (placed == null) gridView.Grid.Place(info, x, y, itemTier, ownedUid); else gridView.Grid.TryMove(placed.InstanceId, x, y); }
            else if (placed != null && tray.ContainsScreenPoint(e.position, e.pressEventCamera) && tray.TryAccept(placed, e.position, e.pressEventCamera)) gridView.Grid.Remove(placed.InstanceId); else gridView.Refresh();
            if (placed == null && this != null) { transform.SetParent(dragOriginParent, false); transform.SetSiblingIndex(dragOriginSibling); } else if (this != null) Destroy(gameObject);
        }
        public void OnPointerClick(PointerEventData e)
        {
            string uid = placed != null ? placed.OwnedUid : ownedUid;
            int tier = placed != null ? placed.Tier : itemTier;
            Selected?.Invoke(info, uid, tier);
            if (e != null && e.button == PointerEventData.InputButton.Right && placed != null && string.IsNullOrEmpty(placed.OwnedUid)) gridView.Grid.CycleTier(placed.InstanceId);
        }
        public void SetAdjacent(bool value) { outline.effectColor = value ? BagViewStyle.Gold : BagViewStyle.Rarity(info.Rarity); outline.effectDistance = value ? new Vector2(3f, -3f) : new Vector2(2f, -2f); }
        public void SetTamPhapBuffed(bool value) => tamPhapText.gameObject.SetActive(value);
        private void Follow(PointerEventData e) { RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, e.position, e.pressEventCamera, out Vector2 p); RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(.5f, .5f); RectTransform.pivot = new Vector2(0f, 1f); RectTransform.anchoredPosition = p - new Vector2(grabbedX * BagViewStyle.Pitch, -grabbedY * BagViewStyle.Pitch); }
    }
}
