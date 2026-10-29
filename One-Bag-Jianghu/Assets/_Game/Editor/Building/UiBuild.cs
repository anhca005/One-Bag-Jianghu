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
            /// <summary>Only adds single-line shrink-to-fit; keeps the label's current rect.</summary>
            None,
            OrnateButton,
            Plaque,
            InkBox
        }

        public static readonly Color Paper = UiKit.Paper;
        public static readonly Color Ink = UiKit.Ink;
        public static readonly Color Gold = UiKit.Gold;
        public static readonly Color Vermilion = UiKit.Vermilion;
        private const string UiTextureFolder = "Assets/_Game/Textures2D/UI/";

        public static bool ApplyUiSprite(
            Image image,
            string key,
            UnityEngine.UI.Image.Type type = UnityEngine.UI.Image.Type.Simple)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(UiTextureFolder + key + ".png");
            if (sprite == null)
            {
                Debug.LogError("Missing UI sprite: " + UiTextureFolder + key + ".png");
                image.sprite = null;
                return false;
            }
            image.sprite = sprite;
            image.type = type;
            image.color = Color.white;
            return true;
        }

        /// <summary>
        /// Turns a text button into the round close icon. The bar-shaped disabled art of Button_Primary would be
        /// squashed into the square icon while a popup waits for the server, so the icon only dims instead.
        /// </summary>
        public static bool ApplyCloseIcon(Button button)
        {
            if (!ApplyUiSprite(button.GetComponent<Image>(), "Kit/button_close"))
                return false;

            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.pressedColor = new Color(.85f, .85f, .85f, 1f);
            colors.disabledColor = new Color(1f, 1f, 1f, .45f);
            button.colors = colors;
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(56f, 56f);
            rect.anchorMin = rect.anchorMax = new Vector2(.965f, .965f);
            button.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
            return true;
        }

        public static Sprite LoadUiSprite(string key)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(UiTextureFolder + key + ".png");
        }

        // Item shapes in items.json; 2x2 shares the square frame. Baked from frame_panel by bake_nine_slice.py.
        private static readonly string[] ItemFrameShapes = { "1x1", "1x2", "2x1", "1x3", "1x4" };

        /// <summary>Item frame that swaps to the art drawn for the item's shape (see ShapeFrame).</summary>
        public static void ApplyItemFrame(Image image)
        {
            var frames = new Sprite[ItemFrameShapes.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = LoadUiSprite("Kit/item_frame_" + ItemFrameShapes[i]);
                if (frames[i] == null)
                    Debug.LogError("Missing UI sprite: " + UiTextureFolder + "Kit/item_frame_" + ItemFrameShapes[i]);
            }

            image.sprite = frames[0];
            image.type = UnityEngine.UI.Image.Type.Simple;
            ShapeFrame shapeFrame = image.GetComponent<ShapeFrame>();
            if (shapeFrame == null)
                shapeFrame = image.gameObject.AddComponent<ShapeFrame>();
            shapeFrame.Initialize(frames);
        }

        public static void AddLeadingIcon(Transform parent, string key, float size = 28f)
        {
            Image icon = Image(parent, "Action Icon", Color.white);
            if (!ApplyUiSprite(icon, "Kit/" + key))
            {
                Object.DestroyImmediate(icon.gameObject);
                return;
            }

            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.rectTransform.anchorMin = new Vector2(0f, .5f);
            icon.rectTransform.anchorMax = new Vector2(0f, .5f);
            icon.rectTransform.pivot = new Vector2(0f, .5f);
            icon.rectTransform.sizeDelta = new Vector2(size, size);
            Text ownText = parent.GetComponent<Text>();
            if (ownText != null)
            {
                // The parent is the label itself: hang the icon just left of it.
                icon.rectTransform.pivot = new Vector2(1f, .5f);
                icon.rectTransform.anchoredPosition = new Vector2(-4f, 0f);
                return;
            }

            icon.rectTransform.anchoredPosition = new Vector2(14f, 0f);
            Text label = parent.GetComponentInChildren<Text>();
            if (label != null)
            {
                RectTransform labelRect = label.rectTransform;
                labelRect.offsetMin = new Vector2(labelRect.offsetMin.x + size + 10f, labelRect.offsetMin.y);
            }
        }

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

        public static Text Text(
            Transform parent,
            string name,
            string value,
            int size,
            TextAnchor alignment = TextAnchor.MiddleCenter,
            UiFontRole role = UiFontRole.Body)
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
        public static void FitLabel(Text text, LabelFitPreset preset, int minimumSize = 14)
        {
            int authoredSize = text.fontSize;
            float left;
            float right;
            float vertical;
            switch (preset)
            {
                case LabelFitPreset.OrnateButton:
                    left = right = 0.27f;
                    vertical = 9f;
                    break;
                case LabelFitPreset.Plaque:
                    left = right = 0.16f;
                    vertical = 8f;
                    break;
                case LabelFitPreset.InkBox:
                    left = 0.18f;
                    right = 0.14f;
                    vertical = 7f;
                    break;
                default:
                    left = right = 0f;
                    vertical = 0f;
                    break;
            }

            RectTransform rect = text.rectTransform;
            if (preset != LabelFitPreset.None)
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
