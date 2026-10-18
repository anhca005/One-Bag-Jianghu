using UnityEngine;
using UnityEngine.EventSystems;
using OneBagJianghu.Audio;
using UnityEngine.UI;
using System;
using OneBagJianghu.Onboarding;

namespace OneBagJianghu.Bag.View
{
    public sealed class ItemView
        : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField]
        private Image background;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image rarityFrame;
        [SerializeField]
        private Image rarityBadge;
        [SerializeField]
        private Outline outline;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text tamPhapText;
        private ItemInfo info;
        private PlacedItem placed;
        private BagGridView gridView;
        private ItemTrayView tray;
        private string ownedUid;
        private int itemTier = 1;
        private string displayedRarity;
        private Canvas canvas;
        private bool dragging;
        private Transform dragOriginParent;
        private int dragOriginSibling;
        private Vector2 dragOriginPosition, dragOriginAnchorMin, dragOriginAnchorMax, dragOriginPivot, dragOriginSize;
        public RectTransform RectTransform => (RectTransform)transform;
        public ItemInfo Item => info;

        public Action<ItemInfo, string, int> Selected;
        private OneBagJianghu.UI.Common.ItemTooltip tooltip;
        private string tooltipText;
        public void SetTooltip(OneBagJianghu.UI.Common.ItemTooltip value, string text)
        {
            tooltip = value;
            tooltipText = text;
        }

        public void OnPointerEnter(PointerEventData e) => tooltip?.ShowDelayed(tooltipText, e);
        public void OnPointerExit(PointerEventData e) => tooltip?.Hide();
        public void Bind(ItemInfo item, string rarity = null)
        {
            info = item;
            TutorialTarget tutorialTarget = GetComponent<TutorialTarget>();
            string tutorialId = item.Id == "thanh_phong_kiem"
                ? "starter_sword"
                : item.Id == "o_tam_y"
                    ? "starter_armor"
                    : null;
            if (tutorialId != null)
            {
                tutorialTarget ??= gameObject.AddComponent<TutorialTarget>();
                tutorialTarget.Configure(tutorialId);
            }
            else if (tutorialTarget != null)
            {
                Destroy(tutorialTarget);
            }
            displayedRarity = string.IsNullOrWhiteSpace(rarity) ? item.Rarity : rarity;
            name = item.Id;
            background.color = BagViewStyle.Fill(displayedRarity);
            if (rarityFrame != null)
            {
                rarityFrame.color = BagViewStyle.Rarity(displayedRarity);
            }
            RarityBadge.Apply(rarityBadge, displayedRarity);

            outline.effectColor = BagViewStyle.Rarity(displayedRarity);
            outline.effectDistance = new Vector2(2f, -2f);
            nameText.text = item.Name;
            ItemIconLibrary library = ItemIconLibrary.Load();
            Sprite sprite = null;
            bool hasIcon = library != null && library.TryGet(item.Id, out sprite);
            icon.gameObject.SetActive(hasIcon);
            icon.sprite = hasIcon ? sprite : null;
            nameText.gameObject.SetActive(!hasIcon);
            nameText.rectTransform.anchorMin = new Vector2(.12f, .14f);
            nameText.rectTransform.anchorMax = new Vector2(.88f, .86f);
            nameText.fontSize = 18;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.horizontalOverflow = HorizontalWrapMode.Wrap;
            nameText.verticalOverflow = VerticalWrapMode.Truncate;
            nameText.resizeTextForBestFit = true;
            nameText.resizeTextMinSize = 12;
            nameText.resizeTextMaxSize = 18;
        }

        public void InitializePlaced(BagGridView owner, ItemTrayView trayView, PlacedItem item, string rarity = null)
        {
            Bind(item.Item, rarity);
            gridView = owner;
            tray = trayView;
            placed = item;
            ownedUid = item.OwnedUid;
        }

        public void InitializeTray(BagGridView owner, ItemTrayView trayView, string uid = null, int tier = 1)
        {
            gridView = owner;
            tray = trayView;
            placed = null;
            ownedUid = uid;
            itemTier = tier;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            AudioManager.Instance?.Play("item_pick");
            tooltip?.Hide();
            canvas = GetComponentInParent<Canvas>();
            dragOriginParent = transform.parent;
            dragOriginSibling = transform.GetSiblingIndex();
            dragOriginPosition = RectTransform.anchoredPosition;
            dragOriginAnchorMin = RectTransform.anchorMin;
            dragOriginAnchorMax = RectTransform.anchorMax;
            dragOriginPivot = RectTransform.pivot;
            dragOriginSize = RectTransform.sizeDelta;
            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            dragging = true;
            Follow(e);
        }

        public void OnDrag(PointerEventData e)
        {
            if (!dragging)
                return;
            Follow(e);
            // Only preview a drop spot while the pointer is over the bag; outside it there is nothing to show.
            if (!gridView.ContainsScreenPoint(e.position, e.pressEventCamera))
            {
                gridView.ClearTarget();
                return;
            }

            gridView.ScreenToCell(e.position, e.pressEventCamera, info.Width, info.Height, out int x, out int y);
            gridView.ShowTarget(info, x, y, placed?.InstanceId ?? -1);
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (!dragging)
                return;
            dragging = false;
            canvasGroup.blocksRaycasts = true;
            gridView.ClearTarget();
            bool overGrid = gridView.ContainsScreenPoint(e.position, e.pressEventCamera);
            gridView.ScreenToCell(e.position, e.pressEventCamera, info.Width, info.Height, out int x, out int y);
            if (overGrid && gridView.Grid.CanPlace(info, x, y, placed?.InstanceId ?? -1))
            {
                if (placed == null)
                    gridView.Grid.Place(info, x, y, itemTier, ownedUid);
                else
                    gridView.Grid.TryMove(placed.InstanceId, x, y);
            }
            else if (placed != null
                && tray != null
                && tray.ContainsScreenPoint(e.position,
                e.pressEventCamera)
                && tray.TryAccept(placed,
                e.position,
                e.pressEventCamera))
                gridView.Grid.Remove(placed.InstanceId);
            else if (placed == null
                && tray != null
                && tray.ContainsScreenPoint(e.position, e.pressEventCamera)
                && tray.TryMoveWithin(ownedUid, info.Width, info.Height, e.position, e.pressEventCamera))
            {
                // Rearranged inside the storage: the tray rebuilt its views, this one is stale.
                AudioManager.Instance?.Play("item_drop");
                Destroy(gameObject);
                return;
            }
            else
            {
                AudioManager.Instance?.Play("item_drop_bad");
                RestoreOrigin();
                return;
            }

            AudioManager.Instance?.Play("item_drop");
            if (placed == null && info.Id == "thanh_phong_kiem")
                OnboardingRuntime.Publish("starter_sword_placed");
            else if (placed == null && info.Id == "o_tam_y")
                OnboardingRuntime.Publish("starter_armor_placed");

            if (placed == null && this != null)
                RestoreOrigin();
            else if (this != null)
                Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData e)
        {
            string uid = placed != null ? placed.OwnedUid : ownedUid;
            int tier = placed != null ? placed.Tier : itemTier;
            Selected?.Invoke(info, uid, tier);
            if (e != null
                && e.button == PointerEventData.InputButton.Right
                && placed != null
                && string.IsNullOrEmpty(placed.OwnedUid))
                gridView.Grid.CycleTier(placed.InstanceId);
        }

        public void SetAdjacent(bool value)
        {
            outline.effectColor = value ? BagViewStyle.Gold : BagViewStyle.Rarity(displayedRarity);
            outline.effectDistance = value ? new Vector2(3f, -3f) : new Vector2(2f, -2f);
            if (rarityFrame != null)
            {
                rarityFrame.color = value ? BagViewStyle.Gold : BagViewStyle.Rarity(displayedRarity);
            }
        }

        public void SetTamPhapBuffed(bool value) => tamPhapText.gameObject.SetActive(value);
        private void Follow(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform,
                e.position,
                e.pressEventCamera,
                out Vector2 p);
            RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(.5f, .5f);
            RectTransform.pivot = new Vector2(.5f, .5f);
            RectTransform.anchoredPosition = p;
        }

        private void RestoreOrigin()
        {
            if (dragOriginParent == null)
                return;
            transform.SetParent(dragOriginParent, false);
            transform.SetSiblingIndex(Mathf.Min(dragOriginSibling, dragOriginParent.childCount - 1));
            RectTransform.anchorMin = dragOriginAnchorMin;
            RectTransform.anchorMax = dragOriginAnchorMax;
            RectTransform.pivot = dragOriginPivot;
            RectTransform.anchoredPosition = dragOriginPosition;
            RectTransform.sizeDelta = dragOriginSize;
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = true;
        }

        private void LateUpdate()
        {
            // A dragged view is temporarily parented to the root canvas.  If its popup is
            // closed, it therefore does not receive OnDisable with the rest of the popup.
            // Destroy the transient view instead of trying to reparent into a hierarchy
            // while that hierarchy is being deactivated.
            if (dragging && (dragOriginParent == null || !dragOriginParent.gameObject.activeInHierarchy))
            {
                dragging = false;
                gridView?.ClearTarget();
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            if (!dragging)
                return;
            dragging = false;
            gridView?.ClearTarget();
        }
    }
}
