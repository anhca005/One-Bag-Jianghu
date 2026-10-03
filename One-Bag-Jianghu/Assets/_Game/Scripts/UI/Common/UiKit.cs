using OneBagJianghu.Bag.View;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    public static class UiKit
    {
        public static readonly Color Paper = BagViewStyle.Paper;
        public static readonly Color Ink = BagViewStyle.Ink;
        public static readonly Color Gold = BagViewStyle.Gold;
        public static readonly Color Vermilion = Hex("#B8322A");
        public static Canvas CreateCanvas(Transform parent, string name, int sortingOrder = 0)
        {
            var go = new GameObject(name,
typeof(RectTransform),
typeof(Canvas),
typeof(CanvasScaler),
typeof(GraphicRaycaster));
            go.transform.SetParent(parent, false);
            Canvas canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            CanvasScaler scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static Image Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text Text(Transform parent,
string name,
string value,
int size,
TextAnchor alignment = TextAnchor.MiddleCenter,
UiFontRole role = UiFontRole.Body)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            UiFonts.Apply(text, role);
            text.text = value;
            text.fontSize = size;
            text.lineSpacing = role == UiFontRole.Display ? 1.05f : 1.1f;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = Ink;
            text.alignment = alignment;
            return text;
        }

        public static Button Button(Transform parent, string label, UnityAction action, bool primary = false)
        {
            Image image = Panel(parent, label, primary ? Vermilion : new Color(Ink.r, Ink.g, Ink.b, 0.12f));
            image.rectTransform.sizeDelta = new Vector2(360f, 64f);
            Outline outline = image.gameObject.AddComponent<Outline>();
            outline.effectColor = Gold;
            outline.effectDistance = new Vector2(2f, -2f);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            Text text = Text(image.transform, "Text", label, 24, TextAnchor.MiddleCenter, UiFontRole.Title);
            Stretch(text.rectTransform);
            text.color = primary ? Paper : Ink;
            SingleLineFit fit = text.gameObject.AddComponent<SingleLineFit>();
            fit.Configure(24, 14);
            return button;
        }

        public static InputField InputField(Transform parent, string placeholder)
        {
            Image image = Panel(parent, "Hero Name", Color.white);
            image.rectTransform.sizeDelta = new Vector2(500f, 64f);
            InputField field = image.gameObject.AddComponent<InputField>();
            Text value = Text(image.transform, "Text", string.Empty, 24, TextAnchor.MiddleLeft);
            Stretch(value.rectTransform, 18f);
            Text hint = Text(image.transform, "Placeholder", placeholder, 24, TextAnchor.MiddleLeft);
            Stretch(hint.rectTransform, 18f);
            Color hintColor = Ink;
            hintColor.a = 0.45f;
            hint.color = hintColor;
            field.textComponent = value;
            field.placeholder = hint;
            field.targetGraphic = image;
            return field;
        }

        public static Dropdown Dropdown(Transform parent)
        {
            Image image = Panel(parent, "Sect", Color.white);
            image.rectTransform.sizeDelta = new Vector2(500f, 64f);
            Dropdown dropdown = image.gameObject.AddComponent<Dropdown>();
            Text caption = Text(image.transform, "Label", string.Empty, 23, TextAnchor.MiddleLeft);
            Stretch(caption.rectTransform, 18f);
            caption.gameObject.AddComponent<SingleLineFit>().Configure(23, 14);
            Image templateImage = Panel(image.transform, "Template", Paper);
            RectTransform template = templateImage.rectTransform;
            template.anchorMin = new Vector2(0f, 0f);
            template.anchorMax = new Vector2(1f, 0f);
            template.pivot = new Vector2(0.5f, 1f);
            template.anchoredPosition = new Vector2(0f, -2f);
            template.sizeDelta = new Vector2(0f, 280f);
            ScrollRect scroll = templateImage.gameObject.AddComponent<ScrollRect>();
            Image viewportImage = Panel(template, "Viewport", Color.clear);
            viewportImage.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            Stretch(viewportImage.rectTransform);
            var contentObject = new GameObject("Content",
typeof(RectTransform),
typeof(VerticalLayoutGroup),
typeof(ContentSizeFitter));
            contentObject.transform.SetParent(viewportImage.transform, false);
            RectTransform content = contentObject.GetComponent<RectTransform>();
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = contentObject.GetComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            contentObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Image itemImage = Panel(content, "Item", Paper);
            itemImage.rectTransform.sizeDelta = new Vector2(0f, 48f);
            Toggle toggle = itemImage.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = itemImage;
            Text itemText = Text(itemImage.transform, "Item Label", "Option", 22, TextAnchor.MiddleLeft);
            Stretch(itemText.rectTransform, 16f);
            scroll.viewport = viewportImage.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            dropdown.captionText = caption;
            dropdown.template = template;
            dropdown.itemText = itemText;
            dropdown.targetGraphic = image;
            templateImage.gameObject.SetActive(false);
            return dropdown;
        }

        public static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        public static RectTransform Vertical(Transform parent, string name, float spacing)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            go.transform.SetParent(parent, false);
            VerticalLayoutGroup layout = go.GetComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            return go.GetComponent<RectTransform>();
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out Color color);
            return color;
        }
    }

    public static class CurrencyArt
    {
        public static bool Apply(Text label, string key, string childName = "Currency Icon", float anchorX = 0f)
        {
            if (label == null)
                return false;
            Sprite sprite = null;
            OneBagJianghu.Pve.PveArtLibrary pve = OneBagJianghu.Pve.PveArtLibrary.Load();
            OneBagJianghu.Bag.ItemIconLibrary items = OneBagJianghu.Bag.ItemIconLibrary.Load();
            if (pve != null)
                pve.TryGet(key, out sprite);
            if (sprite == null && items != null)
                items.TryGet(key, out sprite);
            Transform layoutIcon = label.transform.parent != null
&& label.transform.parent.GetComponent<HorizontalLayoutGroup>() != null ? label.transform.parent.Find("Icon") : null;
            Transform existing = layoutIcon != null ? layoutIcon : label.transform.Find(childName);
            Image image;
            if (existing == null)
            {
                var child = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                child.transform.SetParent(label.transform, false);
                image = child.GetComponent<Image>();
                image.raycastTarget = false;
                image.preserveAspect = true;
            }
            else
                image = existing.GetComponent<Image>();
            image.sprite = sprite;
            image.gameObject.SetActive(sprite != null);
            if (layoutIcon == null)
            {
                RectTransform rect = image.rectTransform;
                rect.anchorMin = rect.anchorMax = new Vector2(anchorX, .5f);
                rect.pivot = new Vector2(0f, .5f);
                rect.anchoredPosition = new Vector2(4f, 0f);
                rect.sizeDelta = new Vector2(26f, 26f);
            }

            return sprite != null;
        }
    }
}