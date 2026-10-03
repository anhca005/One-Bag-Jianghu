using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class StorageItemView
        : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image border;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text badgeText;
        [SerializeField]
        private CanvasGroup canvasGroup;
        private StoragePopup owner;
        private string key;
        private bool material;
        private bool dragging;
        private Vector2 origin, originPivot;
        private int originSibling;
        public void Bind(StoragePopup popup,
string id,
bool isMaterial,
string label,
string badge,
Sprite sprite,
Color color,
float alpha)
        {
            owner = popup;
            key = id;
            material = isMaterial;
            nameText.text = label;
            badgeText.text = badge;
            border.color = color;
            canvasGroup.alpha = alpha;
            icon.sprite = sprite;
            icon.gameObject.SetActive(sprite != null);
            nameText.gameObject.SetActive(sprite == null);
            nameText.horizontalOverflow = HorizontalWrapMode.Wrap;
            nameText.verticalOverflow = VerticalWrapMode.Overflow;
            nameText.resizeTextForBestFit = true;
            nameText.resizeTextMinSize = 10;
            nameText.resizeTextMaxSize = 14;
            nameText.fontSize = 14;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            owner.SelectView(key, material);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            origin = ((RectTransform)transform).anchoredPosition;
            originPivot = ((RectTransform)transform).pivot;
            originSibling = transform.GetSiblingIndex();
            canvasGroup.blocksRaycasts = false;
            ((RectTransform)transform).pivot = new Vector2(.5f, .5f);
            transform.SetAsLastSibling();
            dragging = true;
            ((RectTransform)transform).position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            ((RectTransform)transform).position = eventData.position;
            owner.PreviewDrop(key, material, eventData.position, eventData.pressEventCamera);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            dragging = false;
            canvasGroup.blocksRaycasts = true;
            if (!owner.CommitDrop(key, material, eventData.position, eventData.pressEventCamera))
                RestoreOrigin();
            owner.ClearDropPreview();
        }

        private void RestoreOrigin()
        {
            RectTransform rect = (RectTransform)transform;
            rect.pivot = originPivot;
            rect.anchoredPosition = origin;
            transform.SetSiblingIndex(Mathf.Min(originSibling, transform.parent.childCount - 1));
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = true;
        }

        private void OnDisable()
        {
            if (!dragging)
                return;
            // Unity forbids changing hierarchy/transform state while a parent is being
            // deactivated. The popup is about to discard this view, so only clear drag
            // state here; normal cancelled drops still restore synchronously in OnEndDrag.
            dragging = false;
            owner?.ClearDropPreview();
        }
    }
}