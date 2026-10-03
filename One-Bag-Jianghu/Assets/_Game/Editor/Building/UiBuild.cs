using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class UiBuild
    {
        public enum LabelFitPreset
        {
            OrnateButton,
            Plaque,
            InkBox
        }

        public static readonly Color Paper = UiKit.Paper;
        public static readonly Color Ink = UiKit.Ink;
        public static readonly Color Gold = UiKit.Gold;
        public static readonly Color Vermilion = UiKit.Vermilion;
        private const string UiTextureFolder = "Assets/_Game/Textures2D/UI/";

        public static bool ApplyUiSprite(Image image, string key, UnityEngine.UI.Image.Type type = UnityEngine.UI.Image.Type.Simple)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(UiTextureFolder + key + ".png");
            if (sprite == null) return false;
            image.sprite = sprite;
            image.type = type;
            image.color = Color.white;
            // The 9-slice art is drawn large (borders of 150-420 px); shrink the borders so the ornaments
            // keep their proportions on small UI elements instead of being squashed by Unity.
            if (type == UnityEngine.UI.Image.Type.Sliced && SlicedPixelsPerUnit.TryGetValue(key, out float multiplier))
                image.pixelsPerUnitMultiplier = multiplier;
            return true;
        }

        // Source pixels per UI unit for each 9-slice sprite (tuned for 1920x1080 UI sizes).
        private static readonly System.Collections.Generic.Dictionary<string, float> SlicedPixelsPerUnit =
            new System.Collections.Generic.Dictionary<string, float>
            {
                ["button_primary"] = 6f,
                ["button_secondary"] = 6f,
                ["plaque_label"] = 12f,
                ["progress_bar"] = 12f,
                ["frame_panel"] = 6.5f,
                ["frame_tooltip"] = 6f
            };

        public static RectTransform Rect(Transform parent, string name)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        public static Image Image(Transform parent, string name, Color color)
        {
            RectTransform rect = Rect(parent, name);
            CanvasRenderer renderer = rect.gameObject.AddComponent<CanvasRenderer>();
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text Text(Transform parent, string name, string value, int size, TextAnchor alignment = TextAnchor.MiddleCenter, UiFontRole role = UiFontRole.Body)
        {
            RectTransform rect = Rect(parent, name);
            rect.gameObject.AddComponent<CanvasRenderer>();
            Text text = rect.gameObject.AddComponent<Text>();
            UiFonts.Apply(text, role);
            text.text = value;
            text.fontSize = size;
            text.lineSpacing = role == UiFontRole.Display ? 1.05f : 1.1f;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = Ink;
            text.alignment = alignment;
            return text;
        }

        public static Button Button(Transform parent, string name, string label, bool primary)
        {
            Image image = Image(parent, name, primary ? Vermilion : new Color(Ink.r, Ink.g, Ink.b, 0.12f));
            Outline outline = image.gameObject.AddComponent<Outline>();
            outline.effectColor = Gold;
            outline.effectDistance = new Vector2(2f, -2f);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            Text text = Text(image.transform, "Label", label, 24, TextAnchor.MiddleCenter, UiFontRole.Title);
            text.color = primary ? Paper : Ink;
            FitLabel(text, LabelFitPreset.OrnateButton);
            UiButton uiButton = image.gameObject.AddComponent<UiButton>();
            Set(uiButton, "button", button);
            Set(uiButton, "label", text);
            return button;
        }

        // Normalized horizontal padding keeps ornament clearance proportional as controls resize.
        // Vertical values are pixels because the generated controls have a narrow, stable height range.
        /// <summary>Share of the sliced ornament border the label must stay clear of (the inner part is plain bar).</summary>
        public const float OrnateBorderClearance = 0.8f;

        public static void FitLabel(Text text, LabelFitPreset preset, int minimumSize = 14)
        {
            int authoredSize = text.fontSize;
            float left;
            float right;
            float vertical;
            switch (preset)
            {
                case LabelFitPreset.OrnateButton: left = right = 0.27f; vertical = 9f; break;
                case LabelFitPreset.Plaque: left = right = 0.16f; vertical = 8f; break;
                case LabelFitPreset.InkBox: left = 0.18f; right = 0.14f; vertical = 7f; break;
                default: left = right = 0f; vertical = 0f; break;
            }

            RectTransform rect = text.rectTransform;
            // Ornate button art is 9-sliced: its cloud scrolls keep a fixed pixel width whatever the
            // button width, so the label must clear the slice border in pixels, not a percentage.
            UnityEngine.UI.Image frame = preset == LabelFitPreset.OrnateButton && rect.parent != null ? rect.parent.GetComponent<UnityEngine.UI.Image>() : null;
            if (frame != null && frame.type == UnityEngine.UI.Image.Type.Sliced && frame.sprite != null && frame.hasBorder)
            {
                float scale = 1f / (frame.pixelsPerUnit * frame.pixelsPerUnitMultiplier);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMin = new Vector2(frame.sprite.border.x * scale * OrnateBorderClearance, vertical);
                rect.offsetMax = new Vector2(-frame.sprite.border.z * scale * OrnateBorderClearance, -vertical);
            }
            else
            {
                rect.anchorMin = new Vector2(left, 0f);
                rect.anchorMax = new Vector2(1f - right, 1f);
                rect.offsetMin = new Vector2(0f, vertical);
                rect.offsetMax = new Vector2(0f, -vertical);
            }
            SingleLineFit fit = text.GetComponent<SingleLineFit>();
            if (fit == null) fit = text.gameObject.AddComponent<SingleLineFit>();
            fit.Configure(authoredSize, Mathf.Min(minimumSize, authoredSize));
        }

        public static Canvas Canvas(Transform parent, string name, int sortingOrder)
        {
            RectTransform rect = Rect(parent, name);
            Canvas canvas = rect.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            CanvasScaler scaler = rect.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            rect.gameObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static T InstantiateLinked<T>(string assetPath, Transform parent) where T : Component
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            return instance.GetComponent<T>();
        }

        public static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        public static void Anchors(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static VerticalLayoutGroup Vertical(Transform parent, string name, float spacing)
        {
            RectTransform rect = Rect(parent, name);
            VerticalLayoutGroup layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            return layout;
        }

        public static void Set(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetBool(Object target, string propertyName, bool value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).boolValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
