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
        [MenuItem("One Bag Jianghu/Build/Bag Prefabs")] public static void BuildBagPrefabs() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Bag Prefabs (Overwrite)")] public static void BuildBagPrefabsOverwrite() => Build(true);
        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);
        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(Folder); BuildItem(overwrite); BuildHighlight(overwrite); BuildGrid(overwrite); BuildTray(overwrite); BuildPanel(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildItem(bool overwrite)
        {
            string path = Folder + "/ItemView.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image bg = UiBuild.Image(null, "ItemView", Color.white); bg.gameObject.AddComponent<Outline>(); bg.gameObject.AddComponent<CanvasGroup>(); bg.gameObject.AddComponent<LayoutElement>();
            ItemView view = bg.gameObject.AddComponent<ItemView>();
            Image icon = UiBuild.Image(bg.transform, "Icon", Color.white); UiBuild.Stretch(icon.rectTransform, 5f); icon.preserveAspect = true; icon.raycastTarget = false;
            Text label = UiBuild.Text(bg.transform, "Name", "Item", 18); UiBuild.Stretch(label.rectTransform, 12f);
            Text tier = UiBuild.Text(bg.transform, "Tier", "T1", 14, TextAnchor.MiddleCenter); Anchor(tier.rectTransform, new Vector2(1f, 1f), new Vector2(-8f, -8f), new Vector2(34f, 22f), new Vector2(1f, 1f)); tier.color = UiBuild.Ink; tier.fontStyle = FontStyle.Bold;
            Text mark = UiBuild.Text(bg.transform, "Tam Phap", "TP", 14, TextAnchor.UpperLeft); Anchor(mark.rectTransform, new Vector2(0f, 1f), new Vector2(4f, -3f), new Vector2(30f, 20f), new Vector2(0f, 1f)); mark.color = UiBuild.Gold; mark.fontStyle = FontStyle.Bold; mark.gameObject.SetActive(false);
            UiBuild.Set(view, "background", bg); UiBuild.Set(view, "icon", icon); UiBuild.Set(view, "outline", bg.GetComponent<Outline>()); UiBuild.Set(view, "canvasGroup", bg.GetComponent<CanvasGroup>()); UiBuild.Set(view, "nameText", label); UiBuild.Set(view, "tierText", tier); UiBuild.Set(view, "tamPhapText", mark);
            Save(bg.gameObject, path);
        }

        private static void BuildGrid(bool overwrite)
        {
            string path = Folder + "/BagGrid.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            RectTransform root = UiBuild.Rect(null, "BagGrid"); root.sizeDelta = new Vector2(5 * BagViewStyle.Pitch - BagViewStyle.Gap, 4 * BagViewStyle.Pitch - BagViewStyle.Gap); root.pivot = new Vector2(0f, 1f);
            BagGridView view = root.gameObject.AddComponent<BagGridView>(); RectTransform cells = Layer(root, "Cells");
            for (int y = 0; y < 4; y++) for (int x = 0; x < 5; x++) { Image cell = UiBuild.Image(cells, $"Cell {x},{y}", new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .1f)); BagGridView.SetCellRect(cell.rectTransform, x, y, 1, 1); }
            RectTransform highlights = Layer(root, "Highlights"); RectTransform items = Layer(root, "Items");
            UiBuild.Set(view, "cellsLayer", cells); UiBuild.Set(view, "highlightLayer", highlights); UiBuild.Set(view, "itemLayer", items); UiBuild.Set(view, "itemPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ItemView.prefab").GetComponent<ItemView>()); UiBuild.Set(view,"highlightPrefab",AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/BagHighlight.prefab").GetComponent<Image>()); Save(root.gameObject, path);
        }
        private static void BuildHighlight(bool overwrite) { string path=Folder+"/BagHighlight.prefab";if(!AssetGuard.ShouldBuild(path,overwrite))return;Image image=UiBuild.Image(null,"BagHighlight",BagViewStyle.Valid);image.raycastTarget=false;Save(image.gameObject,path); }

        private static void BuildTray(bool overwrite)
        {
            string path = Folder + "/ItemTray.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image root = UiBuild.Image(null, "ItemTray", new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, .92f)); ItemTrayView view = root.gameObject.AddComponent<ItemTrayView>(); ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();
            Image viewport = UiBuild.Image(root.transform, "Viewport", Color.clear); UiBuild.Stretch(viewport.rectTransform, 8f); viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Content"); content.anchorMin = content.anchorMax = new Vector2(0f, 1f); content.pivot = new Vector2(.5f, 1f); content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 8f; layout.childAlignment = TextAnchor.UpperCenter; layout.childControlWidth = false; layout.childControlHeight = false;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize; scroll.viewport = viewport.rectTransform; scroll.content = content; scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
            UiBuild.Set(view, "content", content); UiBuild.Set(view, "itemPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ItemView.prefab").GetComponent<ItemView>()); Save(root.gameObject, path);
        }

        private static void BuildPanel(bool overwrite)
        {
            string path = Folder + "/BagLoadoutPanel.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            RectTransform root = UiBuild.Rect(null, "BagLoadoutPanel"); BagLoadoutPanel panel = root.gameObject.AddComponent<BagLoadoutPanel>();
            Dropdown dropdown = Dropdown(root); RectTransform dr = dropdown.GetComponent<RectTransform>(); Anchor(dr, new Vector2(0f, 1f), new Vector2(70f, -80f), new Vector2(500f, 64f), new Vector2(0f, 1f));
            Button minus = UiBuild.Button(root, "Decrease Level", "−", false); Anchor(minus.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(70f, -160f), new Vector2(80f, 60f), new Vector2(0f, 1f));
            Text level = UiBuild.Text(root, "Level", "Tâm Pháp cấp 1", 22); Anchor(level.rectTransform, new Vector2(0f, 1f), new Vector2(170f, -160f), new Vector2(300f, 60f), new Vector2(0f, 1f));
            Button plus = UiBuild.Button(root, "Increase Level", "+", false); Anchor(plus.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(490f, -160f), new Vector2(80f, 60f), new Vector2(0f, 1f));
            Text stats = UiBuild.Text(root, "Hero Stats", "Cấp 1\nKhí Huyết tối đa: 100\nHộ Thân: 0% (≤ 80%)\nGiảm Hồi Chiêu: 0% (≤ 40%)", 20, TextAnchor.UpperLeft, UiFontRole.Body);
            // Free column right of the 5x4 bag grid, so the totals never cover the grid.
            Anchor(stats.rectTransform, new Vector2(0f, 1f), new Vector2(600f, -250f), new Vector2(390f, 220f), new Vector2(0f, 1f)); stats.resizeTextForBestFit = false; stats.horizontalOverflow = HorizontalWrapMode.Overflow; stats.verticalOverflow = VerticalWrapMode.Truncate; stats.lineSpacing = 1.15f;
            BagGridView grid = UiBuild.InstantiateLinked<BagGridView>(Folder + "/BagGrid.prefab", root); Anchor((RectTransform)grid.transform, new Vector2(0f, 1f), new Vector2(70f, -365f), ((RectTransform)grid.transform).sizeDelta, new Vector2(0f, 1f));
            ItemTrayView tray = UiBuild.InstantiateLinked<ItemTrayView>(Folder + "/ItemTray.prefab", root); RectTransform tr = (RectTransform)tray.transform; tr.anchorMin = new Vector2(1f, .28f); tr.anchorMax = new Vector2(1f, 1f); tr.pivot = new Vector2(1f, .5f); tr.anchoredPosition = new Vector2(-70f, -30f); tr.sizeDelta = new Vector2(620f, -100f);
            UiBuild.Set(panel, "sectDropdown", dropdown); UiBuild.Set(panel, "levelText", level); UiBuild.Set(panel, "decreaseLevelButton", minus); UiBuild.Set(panel, "increaseLevelButton", plus); UiBuild.Set(panel, "statsText", stats); UiBuild.Set(panel, "gridView", grid); UiBuild.Set(panel, "trayView", tray); Save(root.gameObject, path);
        }

        private static Dropdown Dropdown(Transform parent)
        {
            Dropdown dropdown = UiBuild.InstantiateLinked<Dropdown>(CommonUiPrefabBuilder.Folder + "/Dropdown.prefab", parent);
            dropdown.name = "Sect";
            dropdown.captionText.fontSize = 23;
            dropdown.options = new System.Collections.Generic.List<Dropdown.OptionData> { new Dropdown.OptionData("Môn phái") };
            dropdown.RefreshShownValue();
            return dropdown;
        }
        private static RectTransform Layer(RectTransform root, string name) { RectTransform layer = UiBuild.Rect(root, name); layer.anchorMin = layer.anchorMax = new Vector2(0f, 1f); layer.pivot = new Vector2(0f, 1f); layer.sizeDelta = root.sizeDelta; return layer; }
        private static void Anchor(RectTransform r, Vector2 anchor, Vector2 position, Vector2 size, Vector2 pivot) { r.anchorMin = r.anchorMax = anchor; r.pivot = pivot; r.anchoredPosition = position; r.sizeDelta = size; }
        private static void Save(GameObject root, string path) { PrefabUtility.SaveAsPrefabAsset(root, path); Object.DestroyImmediate(root); }
    }
}
