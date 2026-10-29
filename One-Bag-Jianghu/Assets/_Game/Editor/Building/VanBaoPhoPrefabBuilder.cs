using System.Collections.Generic;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class VanBaoPhoPrefabBuilder
    {
        private const string Path = "Assets/_Game/Resources/UI/Popups/VanBaoPhoPopup.prefab";

        [MenuItem("One Bag Jianghu/Build/Vạn Bảo Phổ Popup (Overwrite)")]
        public static void BuildOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            if (!AssetGuard.ShouldBuild(Path, overwrite))
            {
                return;
            }

            CommonUiPrefabBuilder.Build(false);
            var root = new GameObject("VanBaoPhoPopup", typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas),
                typeof(GraphicRaycaster));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            VanBaoPhoPopup popup = root.AddComponent<VanBaoPhoPopup>();
            Image panelImage = UiBuild.InstantiateLinked<Image>(
                CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab",
                root.transform);
            panelImage.name = "Panel";
            RectTransform panel = panelImage.rectTransform;
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f);
            // Matches the 1.8 aspect of popup_frame_wide so the frame is not stretched.
            panel.sizeDelta = new Vector2(1500f, 834f);
            UiBuild.ApplyUiSprite(panelImage, "Kit/popup_frame_wide", Image.Type.Simple);

            // Header keeps collection progress and filtering visible while the grid scrolls.
            Text title = AddText(panel, "Title", "Vạn Bảo Phổ", 42, new Vector2(.08f, .89f),
                new Vector2(.92f, .98f), TextAnchor.MiddleCenter);
            UiFonts.Apply(title, UiFontRole.Display);
            Text count = AddText(panel, "Owned Count", "Đã có 0/0", 23, new Vector2(.07f, .81f),
                new Vector2(.3f, .88f), TextAnchor.MiddleLeft);
            Dropdown filter = AddFilter(panel);
            RectTransform content = AddGrid(panel);
            VanBaoPhoItemCard template = AddCard(content);
            template.gameObject.SetActive(false);
            UiButton close = AddButton(panel, "Đóng", new Vector2(.5f, .065f));
            ItemTooltip tooltip = ItemTooltip.CreateRuntime(root.transform);

            UiBuild.Set(popup, "panel", panel);
            UiBuild.Set(popup, "content", content);
            UiBuild.Set(popup, "cardTemplate", template);
            UiBuild.Set(popup, "typeFilter", filter);
            UiBuild.Set(popup, "ownedCount", count);
            UiBuild.Set(popup, "closeButton", close);
            UiBuild.Set(popup, "tooltip", tooltip);
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, Path);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        private static Dropdown AddFilter(Transform panel)
        {
            Dropdown dropdown = UiBuild.InstantiateLinked<Dropdown>(
                CommonUiPrefabBuilder.Folder + "/Dropdown.prefab",
                panel);
            dropdown.name = "Type Filter";
            dropdown.options = new List<Dropdown.OptionData>
            {
                new Dropdown.OptionData("Tất cả loại"),
                new Dropdown.OptionData("Binh Khí"),
                new Dropdown.OptionData("Hộ Cụ"),
                new Dropdown.OptionData("Công Pháp"),
                new Dropdown.OptionData("Đan Dược"),
                new Dropdown.OptionData("Kỳ Trân")
            };
            RectTransform rect = dropdown.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(.82f, .845f);
            rect.sizeDelta = new Vector2(320f, 57f);
            return dropdown;
        }

        private static RectTransform AddGrid(Transform panel)
        {
            Image viewport = UiBuild.Image(panel, "Items Viewport", new Color(0f, 0f, 0f, .04f));
            UiBuild.Anchors(viewport.rectTransform, new Vector2(.06f, .14f), new Vector2(.94f, .8f));
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Items Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(.5f, 1f);
            // A new RectTransform keeps a 100x100 sizeDelta, which made the content 100px wider than the viewport.
            content.sizeDelta = Vector2.zero;
            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.cellSize = new Vector2(238f, 170f);
            grid.spacing = new Vector2(12f, 12f);
            grid.padding = new RectOffset(12, 12, 12, 12);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            return content;
        }

        private static VanBaoPhoItemCard AddCard(Transform parent)
        {
            Image background = UiBuild.Image(parent, "Item Card Template", UiBuild.Paper);
            UiBuild.ApplyUiSprite(background, "Kit/shop_card_wide", Image.Type.Simple);
            background.rectTransform.sizeDelta = new Vector2(238f, 170f);
            CanvasGroup group = background.gameObject.AddComponent<CanvasGroup>();
            VanBaoPhoItemCard card = background.gameObject.AddComponent<VanBaoPhoItemCard>();
            HoverTooltipTarget target = background.gameObject.AddComponent<HoverTooltipTarget>();
            Image icon = UiBuild.Image(background.transform, "Icon", Color.white);
            // The bottom 20% of shop_card_wide is a decorative strip, so the content sits above it.
            UiBuild.Anchors(icon.rectTransform, new Vector2(.05f, .26f), new Vector2(.39f, .9f));
            icon.preserveAspect = true;
            Text name = AddText(background.transform, "Name", "Tên vật phẩm", 18, new Vector2(.42f, .6f),
                new Vector2(.95f, .94f), TextAnchor.MiddleLeft);
            Text rarity = AddText(background.transform, "Rarity", "Phàm Phẩm", 15, new Vector2(.42f, .42f),
                new Vector2(.95f, .6f), TextAnchor.MiddleLeft);
            Text type = AddText(background.transform, "Type", "Binh Khí", 14, new Vector2(.42f, .25f),
                new Vector2(.95f, .42f), TextAnchor.MiddleLeft);
            UiBuild.Set(card, "group", group);
            UiBuild.Set(card, "icon", icon);
            UiBuild.Set(card, "itemName", name);
            UiBuild.Set(card, "rarity", rarity);
            UiBuild.Set(card, "type", type);
            UiBuild.Set(card, "tooltipTarget", target);
            return card;
        }

        private static Text AddText(Transform parent, string name, string value, int size, Vector2 min,
            Vector2 max, TextAnchor alignment)
        {
            Text text = UiBuild.Text(parent, name, value, size, alignment);
            UiBuild.Anchors(text.rectTransform, min, max);
            text.color = UiBuild.Ink;
            return text;
        }

        private static UiButton AddButton(Transform parent, string label, Vector2 anchor)
        {
            UiButton button = UiBuild.InstantiateLinked<UiButton>(
                CommonUiPrefabBuilder.Folder + "/Button_Primary.prefab",
                parent);
            button.name = "Close Button";
            button.SetLabel(label);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(260f, 60f);
            return button;
        }
    }
}
