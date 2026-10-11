using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Audio;

namespace OneBagJianghu.EditorTools.Building
{
    public static class CommonUiPrefabBuilder
    {
        public const string Folder = "Assets/_Game/Prefabs/UI/Common";

        public static void BuildMissing()
        {
            Build(false);
        }

        public static void BuildOverwrite()
        {
            Build(true);
        }

        public static void Build(bool overwrite)
        {
            EnsureFontRegistry();
            UiBuild.EnsureFolder(Folder);
            BuildButton("Button_Primary", true, overwrite);
            BuildButton("Button_Secondary", false, overwrite);
            Save("Panel_Paper", overwrite, ConfigurePanel);
            Save("Tooltip", overwrite, ConfigureTooltip);
            BuildText("Text_Title", "Tiêu đề", 42, UiFontRole.Title, overwrite);
            BuildText("Text_Body", "Nội dung", 24, UiFontRole.Body, overwrite);
            Save("InputField", overwrite, ConfigureInput);
            Save("Toggle", overwrite, ConfigureToggle);
            Save("Slider", overwrite, ConfigureSlider);
            Save("ScrollList", overwrite, ConfigureScrollList);
            Save("Dropdown", overwrite, ConfigureDropdown);
            AssetDatabase.SaveAssets();
        }

        private static GameObject ConfigureDropdown(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            Image background = root.AddComponent<Image>();
            background.color = UiBuild.Paper;
            // Simple, not Sliced: fields keep roughly the art's 7:1 shape, so a plain stretch stays clean,
            // while the 9-slice cut ran through the painted ornaments and warped them.
            bool fieldArt = UiBuild.ApplyUiSprite(background, "Kit/dropdown_field_medium", Image.Type.Simple);
            if (!fieldArt)
            {
                Outline outline = root.AddComponent<Outline>();
                outline.effectColor = UiBuild.Ink;
                outline.effectDistance = new Vector2(2f, -2f);
            }

            Dropdown dropdown = root.AddComponent<Dropdown>();
            Text caption = UiBuild.Text(root.transform, "Label", "Chọn", 20, TextAnchor.MiddleLeft);
            UiBuild.FitLabel(caption, UiBuild.LabelFitPreset.InkBox);
            if (fieldArt)
            {
                // Keep the caption between the field's left cap and its painted chevron.
                caption.rectTransform.anchorMin = new Vector2(0f, caption.rectTransform.anchorMin.y);
                caption.rectTransform.anchorMax = new Vector2(1f, caption.rectTransform.anchorMax.y);
                caption.rectTransform.offsetMin = new Vector2(24f, caption.rectTransform.offsetMin.y);
                caption.rectTransform.offsetMax = new Vector2(-52f, caption.rectTransform.offsetMax.y);
            }

            Text arrow = UiBuild.Text(root.transform, "Arrow", "▼", 17);
            arrow.gameObject.SetActive(!fieldArt);
            arrow.raycastTarget = false;
            arrow.rectTransform.anchorMin = new Vector2(0.86f, 0f);
            arrow.rectTransform.anchorMax = Vector2.one;
            arrow.rectTransform.offsetMin = new Vector2(0f, 7f);
            arrow.rectTransform.offsetMax = new Vector2(-10f, -7f);

            // The list owns a sorting canvas so a parent popup cannot cover its rows.
            Image templateImage = UiBuild.Image(root.transform, "Template", UiBuild.Paper);
            RectTransform template = templateImage.rectTransform;
            template.anchorMin = Vector2.zero;
            template.anchorMax = new Vector2(1f, 0f);
            template.pivot = new Vector2(0.5f, 1f);
            template.sizeDelta = new Vector2(0f, 210f);
            // No Canvas here: Dropdown.Show() adds one with sorting order 30000 to the spawned list.
            ScrollRect scroll = templateImage.gameObject.AddComponent<ScrollRect>();
            Image viewport = UiBuild.Image(template, "Viewport", Color.white);
            // No inset: Dropdown.Show() shrinks the template to the content height and assumes the viewport fills it.
            UiBuild.Stretch(viewport.rectTransform);
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform content = UiBuild.Rect(viewport.transform, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = new Vector2(0f, 40f);
            Toggle item = BuildDropdownItem(content);
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            dropdown.targetGraphic = background;
            dropdown.captionText = caption;
            dropdown.template = template;
            dropdown.itemText = item.transform.Find("Item Label").GetComponent<Text>();
            template.gameObject.SetActive(false);
            return root;
        }

        private static Toggle BuildDropdownItem(RectTransform content)
        {
            Image background = UiBuild.Image(content, "Item", UiBuild.Paper);
            background.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            background.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            background.rectTransform.sizeDelta = new Vector2(0f, 40f);
            Toggle toggle = background.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = background;
            Image checkmark = UiBuild.Image(toggle.transform, "Item Checkmark", UiBuild.Vermilion);
            checkmark.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            checkmark.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            checkmark.rectTransform.anchoredPosition = new Vector2(20f, 0f);
            checkmark.rectTransform.sizeDelta = new Vector2(16f, 16f);
            toggle.graphic = checkmark;
            Text label = UiBuild.Text(toggle.transform, "Item Label", "Mục", 19, TextAnchor.MiddleLeft);
            label.color = UiBuild.Ink;
            UiBuild.Stretch(label.rectTransform);
            label.rectTransform.offsetMin = new Vector2(42f, 2f);
            label.rectTransform.offsetMax = new Vector2(-10f, -2f);
            return toggle;
        }

        private static void BuildButton(string name, bool primary, bool overwrite)
        {
            Save(name, overwrite, root => ConfigureButton(root, name, primary));
        }

        private static GameObject ConfigureButton(GameObject root, string name, bool primary)
        {
            Button button = UiBuild.Button(root.transform, "Visual", name, primary);
            button.gameObject.AddComponent<UiSfx>();
            UiBuild.ApplyUiSprite(
                button.GetComponent<Image>(),
                primary ? "Kit/button_primary_compact" : "Kit/button_secondary_compact",
                Image.Type.Simple);
            Sprite disabled = UiBuild.LoadUiSprite("Kit/button_disabled");
            if (disabled != null)
            {
                SpriteState state = button.spriteState;
                state.disabledSprite = disabled;
                button.spriteState = state;
                button.transition = Selectable.Transition.SpriteSwap;
            }
            button.GetComponentInChildren<Text>().color = UiBuild.Paper;
            button.transform.SetParent(root.transform.parent, false);
            // Popup actions share one compact 4.2:1 frame and let localized labels shrink within it.
            ((RectTransform)button.transform).sizeDelta = new Vector2(260f, 62f);
            Object.DestroyImmediate(root);
            return button.gameObject;
        }

        private static GameObject ConfigurePanel(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            root.AddComponent<Image>().color = UiBuild.Paper;
            Image frame = UiBuild.Image(root.transform, "Frame", Color.clear);
            UiBuild.Stretch(frame.rectTransform);
            frame.raycastTarget = false;
            frame.sprite = null;
            frame.gameObject.SetActive(false);
            Outline outline = root.AddComponent<Outline>();
            outline.effectColor = UiBuild.Gold;
            outline.effectDistance = new Vector2(3f, -3f);
            return root;
        }

        private static GameObject ConfigureTooltip(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            Image image = root.AddComponent<Image>();
            image.color = UiBuild.Paper;
            UiBuild.ApplyUiSprite(image, "Kit/tooltip_field", Image.Type.Simple);
            return root;
        }

        private static void BuildText(string name, string value, int size, UiFontRole role, bool overwrite)
        {
            Save(name, overwrite, root => ConfigureText(root, value, size, role));
        }

        private static GameObject ConfigureText(GameObject root, string value, int size, UiFontRole role)
        {
            root.AddComponent<CanvasRenderer>();
            Text text = root.AddComponent<Text>();
            UiFonts.Apply(text, role);
            text.text = value;
            text.fontSize = size;
            text.color = UiBuild.Ink;
            text.alignment = TextAnchor.MiddleCenter;
            return root;
        }

        private static GameObject ConfigureInput(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            Image image = root.AddComponent<Image>();
            image.color = Color.white;
            UiBuild.ApplyUiSprite(image, "Kit/tooltip_field", Image.Type.Simple);
            InputField input = root.AddComponent<InputField>();
            Text value = UiBuild.Text(root.transform, "Text", string.Empty, 24, TextAnchor.MiddleLeft);
            Text placeholder = UiBuild.Text(root.transform, "Placeholder", "Nhập nội dung", 24, TextAnchor.MiddleLeft);
            UiBuild.FitLabel(value, UiBuild.LabelFitPreset.InkBox);
            UiBuild.FitLabel(placeholder, UiBuild.LabelFitPreset.InkBox);
            placeholder.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.45f);
            input.targetGraphic = image;
            input.textComponent = value;
            input.placeholder = placeholder;
            return root;
        }

        private static GameObject ConfigureToggle(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            Image background = root.AddComponent<Image>();
            background.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.18f);
            bool boxArt = UiBuild.ApplyUiSprite(background, "Kit/toggle_off");
            if (!boxArt)
            {
                Outline outline = root.AddComponent<Outline>();
                outline.effectColor = UiBuild.Ink;
                outline.effectDistance = new Vector2(2f, -2f);
            }

            background.preserveAspect = true;
            Toggle toggle = root.AddComponent<Toggle>();
            Image checkmark = UiBuild.Image(root.transform, "Checkmark", UiBuild.Vermilion);
            bool checkArt = UiBuild.ApplyUiSprite(checkmark, "Kit/toggle_on");
            UiBuild.Stretch(checkmark.rectTransform, checkArt ? 0f : 8f);
            checkmark.preserveAspect = true;
            toggle.targetGraphic = background;
            toggle.graphic = checkmark;
            return root;
        }

        private static GameObject ConfigureSlider(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            Image track = root.AddComponent<Image>();
            track.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.18f);
            UiBuild.ApplyUiSprite(track, "Kit/slider_track", Image.Type.Simple);
            // The track art is 1536x97; keep the default size at that aspect.
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(348f, 22f);
            Slider slider = root.AddComponent<Slider>();
            Image fill = UiBuild.Image(root.transform, "Fill", UiBuild.Vermilion);
            UiBuild.Stretch(fill.rectTransform, 4f);
            UiBuild.ApplyUiSprite(fill, "Kit/slider_fill", Image.Type.Filled);
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            // The fill keeps the full track size; SliderFillAmount crops it, so the art is never stretched.
            root.AddComponent<SliderFillAmount>().Initialize(fill);
            RectTransform handleArea = UiBuild.Rect(root.transform, "Handle Slide Area");
            handleArea.anchorMin = Vector2.zero;
            handleArea.anchorMax = Vector2.one;
            handleArea.offsetMin = new Vector2(14f, 0f);
            handleArea.offsetMax = new Vector2(-14f, 0f);
            Image handle = UiBuild.Image(handleArea, "Handle", UiBuild.Vermilion);
            handle.rectTransform.sizeDelta = new Vector2(28f, 28f);
            UiBuild.ApplyUiSprite(handle, "Kit/slider_handle");
            handle.preserveAspect = true;
            slider.targetGraphic = track;
            slider.handleRect = handle.rectTransform;
            return root;
        }

        private static GameObject ConfigureScrollList(GameObject root)
        {
            root.AddComponent<CanvasRenderer>();
            root.AddComponent<Image>().color = UiBuild.Paper;
            ScrollRect scroll = root.AddComponent<ScrollRect>();
            Image viewport = UiBuild.Image(root.transform, "Viewport", Color.clear);
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            UiBuild.Stretch(viewport.rectTransform);
            VerticalLayoutGroup layout = UiBuild.Vertical(viewport.transform, "Content", 8f);
            RectTransform content = layout.GetComponent<RectTransform>();
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(0.5f, 1f);
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            AddVerticalScrollbar(root.transform, scroll);
            return root;
        }

        private static void AddVerticalScrollbar(Transform parent, ScrollRect scroll)
        {
            Color trackColor = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .16f);
            Image track = UiBuild.Image(parent, "Scrollbar", trackColor);
            track.rectTransform.anchorMin = new Vector2(.97f, 0f);
            track.rectTransform.anchorMax = Vector2.one;
            track.rectTransform.offsetMin = Vector2.zero;
            track.rectTransform.offsetMax = Vector2.zero;
            Scrollbar scrollbar = track.gameObject.AddComponent<Scrollbar>();
            Image handle = UiBuild.Image(track.transform, "Handle", UiBuild.Vermilion);
            UiBuild.Stretch(handle.rectTransform);
            handle.color = UiBuild.Ink;
            scrollbar.handleRect = handle.rectTransform;
            scrollbar.targetGraphic = handle;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scroll.verticalScrollbar = scrollbar;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        }

        private static void EnsureFontRegistry()
        {
            const string resources = "Assets/_Game/Resources";
            const string path = resources + "/UiFonts.asset";
            UiBuild.EnsureFolder(resources);
            UiFontRegistry registry = AssetDatabase.LoadAssetAtPath<UiFontRegistry>(path);
            if (registry == null)
            {
                registry = ScriptableObject.CreateInstance<UiFontRegistry>();
                AssetDatabase.CreateAsset(registry, path);
            }

            registry.display = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Fonts/Charm-Bold.ttf");
            registry.title = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Fonts/AlegreyaSC-Bold.ttf");
            registry.body = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Fonts/Alegreya-Regular.ttf");
            registry.emphasis = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Fonts/Alegreya-Bold.ttf");
            EditorUtility.SetDirty(registry);
            UiFonts.Reload();
        }

        private static void Save(string name, bool overwrite, System.Func<GameObject, GameObject> configure)
        {
            string path = Folder + "/" + name + ".prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
            {
                return;
            }

            GameObject root = new GameObject(name, typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 64f);
            root = configure(root);
            root.name = name;
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
