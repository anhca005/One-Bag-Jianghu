using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    public sealed class ItemTooltip : MonoBehaviour
    {
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private RectTransform panel;
        [SerializeField] private Text descriptionText;
        [SerializeField] private float delay = 0.3f;
        [SerializeField] private Vector2 cursorOffset = new Vector2(18f, -18f);
        private Coroutine pending;
        private Vector2 pointerPosition;

        public bool IsVisible => panel != null && panel.gameObject.activeSelf;
        public string Text => descriptionText == null ? string.Empty : descriptionText.text;
        public Canvas OverlayCanvas => overlayCanvas;

        private void Awake() { SetRaycasts(transform, false); Hide(); }
        private void Update()
        {
            if (!IsVisible) return;
            if (Mouse.current != null) pointerPosition = Mouse.current.position.ReadValue();
            Position(pointerPosition);
        }
        public void EnsureAbove(Canvas owner)
        { overlayCanvas.overrideSorting = true; overlayCanvas.sortingOrder = owner == null ? 100 : owner.sortingOrder + 1; }
        public void ShowDelayed(string text, PointerEventData eventData)
        {
            Hide(); if (string.IsNullOrWhiteSpace(text)) return;
            pointerPosition = eventData == null ? Vector2.zero : eventData.position;
            pending = StartCoroutine(ShowAfterDelay(text));
        }
        public void Hide()
        {
            if (pending != null) StopCoroutine(pending); pending = null;
            if (panel != null) panel.gameObject.SetActive(false);
        }
        private IEnumerator ShowAfterDelay(string text)
        {
            yield return new WaitForSecondsRealtime(delay); pending = null; descriptionText.text = text;
            panel.gameObject.SetActive(true); Canvas.ForceUpdateCanvases(); Position(pointerPosition);
        }
        private void Position(Vector2 screenPoint)
        {
            Vector2 size = panel.rect.size;
            bool left = screenPoint.x + cursorOffset.x + size.x > Screen.width;
            bool up = screenPoint.y + cursorOffset.y - size.y < 0f;
            panel.pivot = new Vector2(left ? 1f : 0f, up ? 0f : 1f);
            panel.position = screenPoint + new Vector2(left ? -cursorOffset.x : cursorOffset.x,
                up ? -cursorOffset.y : cursorOffset.y);
        }
        private static void SetRaycasts(Transform root, bool value)
        { foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = value; }
    }
}
