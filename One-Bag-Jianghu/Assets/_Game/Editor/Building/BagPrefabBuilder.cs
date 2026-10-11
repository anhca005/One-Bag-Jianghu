using OneBagJianghu.Bag.View;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class BagPrefabBuilder
    {
        public const string Folder = "Assets/_Game/Prefabs/UI/Bag";
        [MenuItem("One Bag Jianghu/Build/Bag Prefabs")]
        public static void BuildBagPrefabs() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Bag Prefabs (Overwrite)")]
        public static void BuildBagPrefabsOverwrite() => Build(true);
        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);
        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(Folder);
            BuildItem(overwrite);
            BuildHighlight(overwrite);
            BuildGrid(overwrite);
            BuildTray(overwrite);
            BuildPanel(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildItem(bool overwrite)
        {
            string path = Folder + "/ItemView.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            Image bg = UiBuild.Image(null, "ItemView", Color.white);
            bg.gameObject.AddComponent<Outline>();
            bg.gameObject.AddComponent<CanvasGroup>();
            bg.gameObject.AddComponent<LayoutElement>();
            ItemView view = bg.gameObject.AddComponent<ItemView>();
            bg.color = Color.clear;
            // Opaque paper backing: inset so it stays within the rounded frame corners.
            Image backing = UiBuild.Image(bg.transform, "Background", UiBuild.Paper);
            UiBuild.Stretch(backing.rectTransform, 5f);
            backing.raycastTarget = false;
            Image icon = UiBuild.Image(bg.transform, "Icon", Color.white);
            UiBuild.Stretch(icon.rectTransform, 5f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text label = UiBuild.Text(bg.transform, "Name", "Item", 18);
            UiBuild.Stretch(label.rectTransform, 12f);
            label.gameObject.AddComponent<SingleLineFit>().Configure(18, 10);
            Image rarityFrame = UiBuild.Image(bg.transform, "Rarity Frame", Color.white);
            UiBuild.Stretch(rarityFrame.rectTransform, 3f);
            rarityFrame.raycastTarget = false;
            UiBuild.ApplyUiSprite(rarityFrame, "Kit/frame_panel_square", Image.Type.Simple);
            // Rarity medallion: top-left, opposite the tier label at top-right.
            Image rarityBadge = UiBuild.Image(bg.transform, "Rarity Badge", Color.white);
            Anchor(rarityBadge.rectTransform, Vector2.up, new Vector2(4f, -4f), new Vector2(20f, 20f), Vector2.up);
            rarityBadge.preserveAspect = true;
            rarityBadge.raycastTarget = false;
            Text tier = UiBuild.Text(bg.transform, "Tier", "T1", 14, TextAnchor.MiddleCenter);
            Anchor(tier.rectTransform, new Vector2(1f, 1f), new Vector2(-8f, -8f), new Vector2(34f, 22f),
                   new Vector2(1f, 1f));
            tier.color = UiBuild.Ink;
            tier.fontStyle = FontStyle.Bold;
            Image tierPill = UiBuild.Image(bg.transform, "Tier Pill",
                new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .86f));
            Anchor(tierPill.rectTransform, new Vector2(1f, 1f), new Vector2(-8f, -8f), new Vector2(42f, 24f),
                new Vector2(1f, 1f));
            UiBuild.ApplyUiSprite(tierPill, "Kit/tier_pill_compact", Image.Type.Simple);
            tier.transform.SetAsLastSibling();
            Text mark = UiBuild.Text(bg.transform, "Tam Phap", "TP", 14, TextAnchor.UpperLeft);
            Anchor(mark.rectTransform, new Vector2(0f, 1f), new Vector2(4f, -3f), new Vector2(30f, 20f),
                   new Vector2(0f, 1f));
            mark.color = UiBuild.Gold;
            mark.fontStyle = FontStyle.Bold;
            mark.gameObject.SetActive(false);
            UiBuild.Set(view, "background", backing);
            UiBuild.Set(view, "icon", icon);
            UiBuild.Set(view, "rarityFrame", rarityFrame);
            UiBuild.Set(view, "rarityBadge", rarityBadge);
            UiBuild.Set(view, "outline", bg.GetComponent<Outline>());
            UiBuild.Set(view, "canvasGroup", bg.GetComponent<CanvasGroup>());
            UiBuild.Set(view, "nameText", label);
            UiBuild.Set(view, "tierText", tier);
            UiBuild.Set(view, "tamPhapText", mark);
            Save(bg.gameObject, path);
        }

        private static void BuildGrid(bool overwrite)
        {
            string path = Folder + "/BagGrid.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            RectTransform root = UiBuild.Rect(null, "BagGrid");
            root.sizeDelta =
                new Vector2(5 * BagViewStyle.Pitch - BagViewStyle.Gap, 4 * BagViewStyle.Pitch - BagViewStyle.Gap);
            root.pivot = new Vector2(0f, 1f);
            BagGridView view = root.gameObject.AddComponent<BagGridView>();
            RectTransform cells = Layer(root, "Cells");
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 5; x++)
                {
                    Image cell = UiBuild.Image(cells, $"Cell {x},{y}",
                                               new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .1f));
                    UiBuild.ApplyUiSprite(cell, "Kit/slot_cell_square", Image.Type.Simple);
                    BagGridView.SetCellRect(cell.rectTransform, x, y, 1, 1);
                }
            RectTransform highlights = Layer(root, "Highlights");
            RectTransform items = Layer(root, "Items");
            UiBuild.Set(view, "cellsLayer", cells);
            UiBuild.Set(view, "highlightLayer", highlights);
            UiBuild.Set(view, "itemLayer", items);
            UiBuild.Set(
                view, "itemPrefab",
                AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ItemView.prefab").GetComponent<ItemView>());
            UiBuild.Set(
                view, "highlightPrefab",
                AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/BagHighlight.prefab").GetComponent<Image>());
            UiBuild.Set(
                view, "invalidHighlightPrefab",
                AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/BagHighlightInvalid.prefab")
                    .GetComponent<Image>());
            Save(root.gameObject, path);
        }
        private static void BuildHighlight(bool overwrite)
        {
            string path = Folder + "/BagHighlight.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            Image image = UiBuild.Image(null, "BagHighlight", BagViewStyle.Valid);
            UiBuild.ApplyUiSprite(image, "Kit/slot_cell_highlight_square", Image.Type.Simple);
            image.raycastTarget = false;
            Save(image.gameObject, path);

            string invalidPath = Folder + "/BagHighlightInvalid.prefab";
            if (!AssetGuard.ShouldBuild(invalidPath, overwrite))
                return;
            Image invalid = UiBuild.Image(null, "BagHighlightInvalid", BagViewStyle.Invalid);
            UiBuild.ApplyUiSprite(invalid, "Kit/slot_cell_invalid_square", Image.Type.Simple);
            invalid.raycastTarget = false;
            Save(invalid.gameObject, invalidPath);
        }

        private static void BuildTray(bool overwrite)
        {
            string path = Folder + "/ItemTray.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            Image root =
                UiBuild.Image(null, "ItemTray", new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, .92f));
            ItemTrayView view = root.gameObject.AddComponent<ItemTrayView>();
            ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();
            Image viewport = UiBuild.Image(root.transform, "Viewport", Color.clear);
            UiBuild.Stretch(viewport.rectTransform, 8f);
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Content");
            content.anchorMin = content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(.5f, 1f);
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            UiBuild.Set(view, "content", content);
            UiBuild.Set(
                view, "itemPrefab",
                AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ItemView.prefab").GetComponent<ItemView>());
            Save(root.gameObject, path);
        }

        private static void BuildPanel(bool overwrite)
        {
            string path = Folder + "/BagLoadoutPanel.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            RectTransform root = UiBuild.Rect(null, "BagLoadoutPanel");
            BagLoadoutPanel panel = root.gameObject.AddComponent<BagLoadoutPanel>();
            Dropdown dropdown = Dropdown(root);
            RectTransform dr = dropdown.GetComponent<RectTransform>();
            Anchor(dr, new Vector2(0f, 1f), new Vector2(30f, -20f), new Vector2(500f, 72f), new Vector2(0f, 1f));
            Button minus = UiBuild.Button(root, "Decrease Level", "−", false);
            Anchor(minus.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(30f, -100f),
                   new Vector2(80f, 60f), new Vector2(0f, 1f));
            Text level = UiBuild.Text(root, "Level", "Tâm Pháp cấp 1", 22);
            Anchor(level.rectTransform, new Vector2(0f, 1f), new Vector2(130f, -100f), new Vector2(300f, 60f),
                   new Vector2(0f, 1f));
            Button plus = UiBuild.Button(root, "Increase Level", "+", false);
            Anchor(plus.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(450f, -100f),
                   new Vector2(80f, 60f), new Vector2(0f, 1f));
            Text stats = UiBuild.Text(root, "Hero Stats",
                                      "Cấp 1\nKhí Huyết tối đa: 100\nHộ Thân: 0% (≤ 80%)\nGiảm Hồi Chiêu: 0% (≤ 40%)",
                                      20, TextAnchor.UpperLeft, UiFontRole.Body);
            // Live combat power occupies the top of the free column beside the bag grid.
            Text power = UiBuild.Text(root, "Power", "Lực Chiến: 100", 22, TextAnchor.MiddleLeft, UiFontRole.Title);
            Anchor(power.rectTransform, new Vector2(0f, 1f), new Vector2(565f, -20f), new Vector2(360f, 42f),
                   new Vector2(0f, 1f));
            power.color = UiBuild.Vermilion;
            UiBuild.FitLabel(power, UiBuild.LabelFitPreset.None);
            // Free column right of the 5x4 bag grid, so the totals never cover the grid.
            Anchor(stats.rectTransform, new Vector2(0f, 1f), new Vector2(565f, -68f), new Vector2(360f, 172f),
                   new Vector2(0f, 1f));
            stats.resizeTextForBestFit = false;
            stats.horizontalOverflow = HorizontalWrapMode.Wrap;
            stats.verticalOverflow = VerticalWrapMode.Truncate;
            stats.lineSpacing = 1.15f;
            BagGridView grid = UiBuild.InstantiateLinked<BagGridView>(Folder + "/BagGrid.prefab", root);
            Image board = UiBuild.Image(root, "Bag Board",
                new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .13f));
            Anchor(board.rectTransform, new Vector2(0f, 1f), new Vector2(18f, -168f),
                new Vector2(540f, 450f), new Vector2(0f, 1f));
            UiBuild.ApplyUiSprite(board, "Kit/bag_board_landscape", Image.Type.Simple);
            board.transform.SetSiblingIndex(grid.transform.GetSiblingIndex());
            Anchor((RectTransform)grid.transform, new Vector2(0f, 1f), new Vector2(30f, -180f),
                   ((RectTransform)grid.transform).sizeDelta, new Vector2(0f, 1f));
            ItemTrayView tray = UiBuild.InstantiateLinked<ItemTrayView>(Folder + "/ItemTray.prefab", root);
            RectTransform tr = (RectTransform)tray.transform;
            tr.anchorMin = new Vector2(1f, 0f);
            tr.anchorMax = new Vector2(1f, 1f);
            tr.pivot = new Vector2(1f, .5f);
            tr.anchoredPosition = new Vector2(-20f, 0f);
            tr.sizeDelta = new Vector2(620f, -20f);
            UiBuild.Set(panel, "sectDropdown", dropdown);
            UiBuild.Set(panel, "levelText", level);
            UiBuild.Set(panel, "decreaseLevelButton", minus);
            UiBuild.Set(panel, "increaseLevelButton", plus);
            UiBuild.Set(panel, "statsText", stats);
            UiBuild.Set(panel, "powerText", power);
            UiBuild.Set(panel, "gridView", grid);
            UiBuild.Set(panel, "trayView", tray);
            Save(root.gameObject, path);
        }

        private static Dropdown Dropdown(Transform parent)
        {
            Dropdown dropdown =
                UiBuild.InstantiateLinked<Dropdown>(CommonUiPrefabBuilder.Folder + "/Dropdown.prefab", parent);
            dropdown.name = "Sect";
            // The Sect field is 500x72, the aspect of the full-size dropdown art (1512x218), not the medium one.
            UiBuild.ApplyUiSprite(dropdown.GetComponent<Image>(), "Kit/dropdown_field", Image.Type.Simple);
            dropdown.captionText.fontSize = 23;
            dropdown.options =
                new System.Collections.Generic.List<Dropdown.OptionData> { new Dropdown.OptionData("Môn phái") };
            dropdown.RefreshShownValue();
            return dropdown;
        }
        private static RectTransform Layer(RectTransform root, string name)
        {
            RectTransform layer = UiBuild.Rect(root, name);
            layer.anchorMin = layer.anchorMax = new Vector2(0f, 1f);
            layer.pivot = new Vector2(0f, 1f);
            layer.sizeDelta = root.sizeDelta;
            return layer;
        }
        private static void Anchor(RectTransform r, Vector2 anchor, Vector2 position, Vector2 size, Vector2 pivot)
        {
            r.anchorMin = r.anchorMax = anchor;
            r.pivot = pivot;
            r.anchoredPosition = position;
            r.sizeDelta = size;
        }
        private static void Save(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
