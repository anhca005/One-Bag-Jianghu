using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class UiBuild
    {
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

        public static Text Text(Transform parent, string name, string value, int size, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            RectTransform rect = Rect(parent, name);
            rect.gameObject.AddComponent<CanvasRenderer>();
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = size;
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
            Text text = Text(image.transform, "Label", label, 24);
            text.color = primary ? Paper : Ink;
            Stretch(text.rectTransform, 12f);
            UiButton uiButton = image.gameObject.AddComponent<UiButton>();
            Set(uiButton, "button", button);
            Set(uiButton, "label", text);
            return button;
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
