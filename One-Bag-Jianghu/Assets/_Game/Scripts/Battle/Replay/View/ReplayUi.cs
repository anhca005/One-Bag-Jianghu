using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View
{
    internal static class ReplayUi
    {
        public static RectTransform Rect(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        public static Image Image(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>(); image.color = color;
            return image;
        }

        public static Text Text(Transform parent, string name, string value, int size, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value; text.fontSize = size; text.color = ReplayStyle.Ink; text.alignment = alignment;
            return text;
        }

        public static Button Button(Transform parent, string label, UnityAction action, float width = 120f)
        {
            Image image = Image(parent, label, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .14f));
            image.gameObject.AddComponent<LayoutElement>().preferredWidth = width;
            Button button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image; button.onClick.AddListener(action);
            Text text = Text(image.transform, "Text", label, ReplayStyle.BodyFont);
            Stretch(text.rectTransform);
            return button;
        }

        public static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
    }
}
