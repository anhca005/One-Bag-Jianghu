using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class StorageItemView : MonoBehaviour, IBeginDragHandler, IDragHandler,
        IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image border;
        [SerializeField] private Text nameText;
        [SerializeField] private Text badgeText;
        [SerializeField] private CanvasGroup canvasGroup;

        private StoragePopup owner;
        private string key;
        private bool material;
        private Vector2 origin;

        public void Bind(StoragePopup popup, string id, bool isMaterial, string label, string badge,
            Sprite sprite, Color color, float alpha)
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
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            owner.SelectView(key, material);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            origin = ((RectTransform)transform).anchoredPosition;
            canvasGroup.blocksRaycasts = false;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            ((RectTransform)transform).position = eventData.position;
            owner.PreviewDrop(key, material, eventData.position, eventData.pressEventCamera);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            if (!owner.CommitDrop(key, material, eventData.position, eventData.pressEventCamera))
                ((RectTransform)transform).anchoredPosition = origin;
            owner.ClearDropPreview();
        }
    }
}
