using System.Collections;
using OneBagJianghu.Economy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    public sealed class ItemTooltip : MonoBehaviour
    {
        public const float HorizontalInset = 32f;
        public const float BottomInset = 28f;
        public const float TopInset = 72f;
        [SerializeField]
        private Canvas overlayCanvas;
        [SerializeField]
        private RectTransform panel;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private float delay = 0.3f;
        [SerializeField]
        private Vector2 cursorOffset = new Vector2(18f, -18f);
        private Coroutine pending;
        private Vector2 pointerPosition;
        public bool IsVisible => panel != null && panel.gameObject.activeSelf;
        public string Text => descriptionText == null ? string.Empty : descriptionText.text;
        public Canvas OverlayCanvas => overlayCanvas;

        public static ItemTooltip CreateRuntime(Transform parent)
        {
            // Prefer a copy of the shop's built tooltip, so every screen shows the same frame and clipping.
            ItemTooltip shared = SharedTooltip();
            if (shared != null)
            {
                ItemTooltip copy = Instantiate(shared, parent, false);
                copy.name = "Battle Item Tooltip";
                Stretch((RectTransform)copy.transform);
                copy.Hide();
                return copy;
            }

            var root = new GameObject("Battle Item Tooltip", typeof(RectTransform), typeof(Canvas));
            root.transform.SetParent(parent, false);
            RectTransform rootRect = root.GetComponent<RectTransform>();
            Stretch(rootRect);
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.overrideSorting = true;

            var paperObject = new GameObject("Paper", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            paperObject.transform.SetParent(root.transform, false);
            Image paper = paperObject.GetComponent<Image>();
            paper.color = new Color(.94f, .88f, .72f, .98f);
            RectTransform panelRect = paper.rectTransform;
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.zero;
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.sizeDelta = new Vector2(440f, 420f);

            RectTransform viewport = CreateRect(panelRect, "Viewport");
            Stretch(viewport);
            viewport.offsetMin = new Vector2(HorizontalInset, BottomInset);
            viewport.offsetMax = new Vector2(-HorizontalInset, -TopInset);
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = CreateRect(viewport, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(.5f, 1f);
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            ContentSizeFitter contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = paperObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            var textObject = new GameObject("Description", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(content, false);
            Text text = textObject.GetComponent<Text>();
            UiFonts.Apply(text, UiFontRole.Body);
            text.fontSize = 18;
            text.color = new Color(.13f, .1f, .07f);
            text.alignment = TextAnchor.UpperLeft;
            text.supportRichText = true;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            ContentSizeFitter textFitter = textObject.AddComponent<ContentSizeFitter>();
            textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ItemTooltip tooltip = root.AddComponent<ItemTooltip>();
            tooltip.overlayCanvas = canvas;
            tooltip.panel = panelRect;
            tooltip.descriptionText = text;
            SetRaycasts(root.transform, false);
            tooltip.Hide();
            return tooltip;
        }

        public static string WithPower(string description, OwnedItem item)
        {
            if (item == null)
                return description;
            int power = PowerCatalog.LoadFromResources().ItemPower(item);
            return WithPower(description, power);
        }

        public static string WithPower(string description, int power)
        {
            string line = "<color=#A33A2B>Lực Chiến +" + UiFormat.Number(power) + "</color>";
            int insertion = FindLineEnd(description, 3);
            return insertion < 0
                ? description + "\n" + line
                : description.Insert(insertion, "\n" + line);
        }

        private void Awake()
        {
            SetRaycasts(transform, false);
            Hide();
        }

        private void Update()
        {
            if (!IsVisible)
                return;
            if (Mouse.current != null)
                pointerPosition = Mouse.current.position.ReadValue();
            Position(pointerPosition);
        }

        public void EnsureAbove(Canvas owner)
        {
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = owner == null ? 100 : owner.sortingOrder + 1;
        }

        public void ShowDelayed(string text, PointerEventData eventData)
        {
            Hide();
            if (string.IsNullOrWhiteSpace(text))
                return;
            pointerPosition = eventData == null ? Vector2.zero : eventData.position;
            pending = StartCoroutine(ShowAfterDelay(text));
        }

        public void Hide()
        {
            if (pending != null)
                StopCoroutine(pending);
            pending = null;
            if (panel != null)
                panel.gameObject.SetActive(false);
        }

        private IEnumerator ShowAfterDelay(string text)
        {
            yield return new WaitForSecondsRealtime(delay);
            pending = null;
            descriptionText.text = text;
            panel.gameObject.SetActive(true);
            FitToContent();
            Canvas.ForceUpdateCanvases();
            Position(pointerPosition);
        }

        private void FitToContent()
        {
            const float verticalInsets = TopInset + BottomInset;
            // Panel sizes are in canvas units (CanvasScaler), not screen pixels.
            float maxHeight = Mathf.Max(160f, CanvasSize().y * .70f);
            descriptionText.fontSize = 18;
            descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
            ContentSizeFitter fitter = descriptionText.GetComponent<ContentSizeFitter>();
            if (fitter != null)
                fitter.enabled = true;
            RectTransform content = descriptionText.rectTransform.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            if (content != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionText.rectTransform);
            Canvas.ForceUpdateCanvases();
            float wanted = descriptionText.preferredHeight + verticalInsets;
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(wanted, maxHeight));
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            if (content != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            Canvas.ForceUpdateCanvases();
        }

        private Vector2 CanvasSize()
        {
            Canvas root = overlayCanvas != null ? overlayCanvas.rootCanvas : GetComponentInParent<Canvas>().rootCanvas;
            return ((RectTransform)root.transform).rect.size;
        }

        private float ScaleFactor()
        {
            Canvas root = overlayCanvas != null ? overlayCanvas.rootCanvas : GetComponentInParent<Canvas>().rootCanvas;
            return root.scaleFactor <= 0f ? 1f : root.scaleFactor;
        }

        private void Position(Vector2 screenPoint)
        {
            // Work in screen pixels: panel.position is a screen point for overlay canvases.
            Vector2 size = panel.rect.size * ScaleFactor();
            bool left = screenPoint.x + cursorOffset.x * ScaleFactor() + size.x > Screen.width;
            bool below = screenPoint.y - cursorOffset.y * ScaleFactor() - size.y >= 0f;
            panel.pivot = new Vector2(left ? 1f : 0f, below ? 1f : 0f);
            Vector2 offset = cursorOffset * ScaleFactor();
            Vector2 target = screenPoint + new Vector2(left ? -offset.x : offset.x, below ? -offset.y : offset.y);
            float x = Mathf.Clamp(target.x, left ? size.x : 0f, left ? Screen.width : Screen.width - size.x);
            float y = Mathf.Clamp(target.y, below ? size.y : 0f, below ? Screen.height : Screen.height - size.y);
            panel.position = new Vector2(x, y);
        }

        private static void SetRaycasts(Transform root, bool value)
        {
            foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = value;
        }

        private static ItemTooltip SharedTooltip()
        {
            GameObject shop = Resources.Load<GameObject>("UI/Popups/ShopPopup");
            return shop == null ? null : shop.GetComponentInChildren<ItemTooltip>(true);
        }

        private static RectTransform CreateRect(Transform parent, string objectName)
        {
            var value = new GameObject(objectName, typeof(RectTransform));
            value.transform.SetParent(parent, false);
            return value.GetComponent<RectTransform>();
        }

        private static void Stretch(RectTransform value)
        {
            value.anchorMin = Vector2.zero;
            value.anchorMax = Vector2.one;
            value.offsetMin = Vector2.zero;
            value.offsetMax = Vector2.zero;
        }

        private static int FindLineEnd(string value, int line)
        {
            if (string.IsNullOrEmpty(value))
                return -1;
            int index = -1;
            for (int current = 0; current < line; current++)
            {
                index = value.IndexOf('\n', index + 1);
                if (index < 0)
                    return -1;
            }

            return index;
        }
    }
}
