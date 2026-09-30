using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class CommonUiPrefabBuilder
    {
        public const string Folder = "Assets/_Game/Prefabs/UI/Common";

        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(Folder);
            BuildButton("Button_Primary", true, overwrite);
            BuildButton("Button_Secondary", false, overwrite);
            BuildPanel(overwrite);
            BuildTooltip(overwrite);
            BuildText("Text_Title", "Tiêu đề", 42, overwrite);
            BuildText("Text_Body", "Nội dung", 24, overwrite);
            BuildInput(overwrite);
            BuildToggle(overwrite);
            BuildSlider(overwrite);
            BuildScrollList(overwrite);
            BuildDropdown(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildDropdown(bool overwrite)
        {
            Save("Dropdown", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>(); Image background = root.AddComponent<Image>(); background.color = UiBuild.Paper; UiBuild.ApplyUiSprite(background, "frame_tooltip", Image.Type.Sliced);
                Outline outline = root.AddComponent<Outline>(); outline.effectColor = UiBuild.Ink; outline.effectDistance = new Vector2(2, -2);
                Dropdown dropdown = root.AddComponent<Dropdown>(); Text caption = UiBuild.Text(root.transform, "Label", "Chọn", 20, TextAnchor.MiddleLeft); UiBuild.Stretch(caption.rectTransform, 14);
                Image templateImage = UiBuild.Image(root.transform, "Template", UiBuild.Paper); RectTransform template = templateImage.rectTransform; template.anchorMin = new Vector2(0, 0); template.anchorMax = new Vector2(1, 0); template.pivot = new Vector2(.5f, 1); template.sizeDelta = new Vector2(0, 240);
                ScrollRect scroll = templateImage.gameObject.AddComponent<ScrollRect>(); Image viewport = UiBuild.Image(template, "Viewport", Color.clear); UiBuild.Stretch(viewport.rectTransform, 4); viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
                RectTransform content = UiBuild.Rect(viewport.transform, "Content"); content.anchorMin = new Vector2(0, 1); content.anchorMax = Vector2.one; content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(0, 40);
                Toggle item = UiBuild.Image(content, "Item", Color.clear).gameObject.AddComponent<Toggle>(); item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40); Text itemLabel = UiBuild.Text(item.transform, "Item Label", "Mục", 19, TextAnchor.MiddleLeft); UiBuild.Stretch(itemLabel.rectTransform, 12);
                scroll.viewport = viewport.rectTransform; scroll.content = content; scroll.horizontal = false; dropdown.targetGraphic = background; dropdown.captionText = caption; dropdown.template = template; dropdown.itemText = itemLabel; template.gameObject.SetActive(false); return root;
            });
        }

        private static void BuildButton(string name, bool primary, bool overwrite)
        {
            Save(name, overwrite, root =>
            {
                Button button = UiBuild.Button(root.transform, "Visual", name, primary);
                Image image = button.GetComponent<Image>();
                UiBuild.ApplyUiSprite(image, primary ? "button_primary" : "button_secondary", Image.Type.Sliced);
                Text label = button.GetComponentInChildren<Text>();
                label.color = primary ? UiBuild.Paper : UiBuild.Paper;
                button.transform.SetParent(root.transform.parent, false);
                Object.DestroyImmediate(root);
                return button.gameObject;
            });
        }

        private static void BuildPanel(bool overwrite)
        {
            Save("Panel_Paper", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image image = root.AddComponent<Image>();
                image.color = UiBuild.Paper;
                Image frame = UiBuild.Image(root.transform, "Frame", Color.clear);
                UiBuild.Stretch(frame.rectTransform);
                frame.raycastTarget = false;
                UiBuild.ApplyUiSprite(frame, "frame_panel", Image.Type.Sliced);
                Outline outline = root.AddComponent<Outline>();
                outline.effectColor = UiBuild.Gold;
                outline.effectDistance = new Vector2(3f, -3f);
                return root;
            });
        }

        private static void BuildTooltip(bool overwrite)
        {
            Save("Tooltip", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image image = root.AddComponent<Image>();
                image.color = UiBuild.Paper;
                UiBuild.ApplyUiSprite(image, "frame_tooltip", Image.Type.Sliced);
                return root;
            });
        }

        private static void BuildText(string name, string value, int size, bool overwrite)
        {
            Save(name, overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Text text = root.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.text = value;
                text.fontSize = size;
                text.color = UiBuild.Ink;
                text.alignment = TextAnchor.MiddleCenter;
                return root;
            });
        }

        private static void BuildInput(bool overwrite)
        {
            Save("InputField", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image image = root.AddComponent<Image>();
                image.color = Color.white;
                InputField input = root.AddComponent<InputField>();
                Text value = UiBuild.Text(root.transform, "Text", string.Empty, 24, TextAnchor.MiddleLeft);
                Text placeholder = UiBuild.Text(root.transform, "Placeholder", "Nhập nội dung", 24, TextAnchor.MiddleLeft);
                UiBuild.Stretch(value.rectTransform, 16f);
                UiBuild.Stretch(placeholder.rectTransform, 16f);
                placeholder.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.45f);
                input.targetGraphic = image;
                input.textComponent = value;
                input.placeholder = placeholder;
                return root;
            });
        }

        private static void BuildToggle(bool overwrite)
        {
            Save("Toggle", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image background = root.AddComponent<Image>();
                background.color = Color.white;
                Toggle toggle = root.AddComponent<Toggle>();
                Image checkmark = UiBuild.Image(root.transform, "Checkmark", UiBuild.Vermilion);
                UiBuild.Stretch(checkmark.rectTransform, 8f);
                toggle.targetGraphic = background;
                toggle.graphic = checkmark;
                return root;
            });
        }

        private static void BuildSlider(bool overwrite)
        {
            Save("Slider", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image track = root.AddComponent<Image>();
                track.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.18f);
                Slider slider = root.AddComponent<Slider>();
                Image fill = UiBuild.Image(root.transform, "Fill", UiBuild.Vermilion);
                UiBuild.Stretch(fill.rectTransform, 4f);
                slider.targetGraphic = track;
                slider.fillRect = fill.rectTransform;
                return root;
            });
        }

        private static void BuildScrollList(bool overwrite)
        {
            Save("ScrollList", overwrite, root =>
            {
                root.AddComponent<CanvasRenderer>();
                Image background = root.AddComponent<Image>();
                background.color = UiBuild.Paper;
                ScrollRect scroll = root.AddComponent<ScrollRect>();
                Image viewportImage = UiBuild.Image(root.transform, "Viewport", Color.clear);
                Mask mask = viewportImage.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;
                UiBuild.Stretch(viewportImage.rectTransform);
                VerticalLayoutGroup layout = UiBuild.Vertical(viewportImage.transform, "Content", 8f);
                RectTransform content = layout.GetComponent<RectTransform>();
                content.anchorMin = new Vector2(0f, 1f);
                content.anchorMax = Vector2.one;
                content.pivot = new Vector2(0.5f, 1f);
                ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                scroll.viewport = viewportImage.rectTransform;
                scroll.content = content;
                scroll.horizontal = false;
                return root;
            });
        }

        private static void Save(string name, bool overwrite, System.Func<GameObject, GameObject> configure)
        {
            string path = Folder + "/" + name + ".prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var root = new GameObject(name, typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 64f);
            root = configure(root);
            root.name = name;
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
