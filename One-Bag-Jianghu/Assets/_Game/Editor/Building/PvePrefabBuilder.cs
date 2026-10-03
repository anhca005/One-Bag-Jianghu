using System;
using System.IO;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Pve;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class PvePrefabBuilder
    {
        private const string PopupFolder = "Assets/_Game/Resources/UI/Popups";
        private const string PrefabFolder = "Assets/_Game/Prefabs/UI/Pve";
        public const string MapPrefabFolder = PrefabFolder + "/Maps";
        private const string ArtPath = "Assets/_Game/Resources/PveArtLibrary.asset";

        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(PrefabFolder); RebuildArtLibrary(); BuildNode(overwrite); EnsureMapPrefabs(); BuildMap(overwrite); BuildDialogue(overwrite); AssetDatabase.SaveAssets();
        }

        /// <summary>Creates missing chapter maps and adds only newly introduced stage anchors.</summary>
        public static void EnsureMapPrefabs()
        {
            UiBuild.EnsureFolder(MapPrefabFolder);
            PveCatalog catalog = PveCatalog.Load(); PveArtLibrary library = AssetDatabase.LoadAssetAtPath<PveArtLibrary>(ArtPath);
            foreach (PveChapter chapter in catalog.Chapters) EnsureChapterMap(chapter, library, false);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("One Bag Jianghu/PvE/Reset Map Prefabs from chapters.json (discards manual edits)")]
        public static void ResetMapPrefabs()
        {
            if (!EditorUtility.DisplayDialog("Reset PvE map prefabs?", "This discards every hand-edited stage position and restores chapters.json mapPos values.", "Reset map prefabs", "Cancel")) return;
            UiBuild.EnsureFolder(MapPrefabFolder);
            PveCatalog catalog = PveCatalog.Load(); PveArtLibrary library = AssetDatabase.LoadAssetAtPath<PveArtLibrary>(ArtPath);
            foreach (PveChapter chapter in catalog.Chapters) EnsureChapterMap(chapter, library, true);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        }

        private static void EnsureChapterMap(PveChapter chapter, PveArtLibrary library, bool reset)
        {
            string path = MapPrefabFolder + "/Map_" + chapter.Id + ".prefab";
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing == null || reset)
            {
                GameObject root = CreateChapterMap(chapter, library);
                PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root); return;
            }

            GameObject contents = PrefabUtility.LoadPrefabContents(path); bool changed = false;
            try
            {
                PveMapLayout layout = contents.GetComponent<PveMapLayout>();
                if (layout == null || layout.NodesRoot == null) { Debug.LogWarning(path + " has no valid PveMapLayout; skipped to protect manual edits. Use the explicit reset command to rebuild it."); return; }
                foreach (PveStage stage in chapter.Stages) if (layout.FindAnchor(stage.Id) == null) { CreateAnchor(layout.NodesRoot, stage); changed = true; }
                if (changed) PrefabUtility.SaveAsPrefabAsset(contents, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }

        private static GameObject CreateChapterMap(PveChapter chapter, PveArtLibrary library)
        {
            GameObject root = new GameObject("Map_" + chapter.Id, typeof(RectTransform)); RectTransform rect = root.GetComponent<RectTransform>(); rect.sizeDelta = new Vector2(1196.8f, 784f);
            Image background = UiBuild.Image(root.transform, "Background", Color.white); UiBuild.Stretch(background.rectTransform); background.preserveAspect = true;
            if (library != null && library.TryGet(chapter.Theme.MapBackground, out Sprite sprite)) background.sprite = sprite;
            else background.color = UiBuild.Paper;
            RectTransform nodes = UiBuild.Rect(root.transform, "Nodes"); UiBuild.Stretch(nodes);
            PveMapLayout layout = root.AddComponent<PveMapLayout>(); UiBuild.Set(layout, "nodesRoot", nodes);
            var serialized = new SerializedObject(layout); serialized.FindProperty("chapterId").stringValue = chapter.Id; serialized.ApplyModifiedPropertiesWithoutUndo();
            foreach (PveStage stage in chapter.Stages) CreateAnchor(nodes, stage);
            return root;
        }

        private static void CreateAnchor(RectTransform parent, PveStage stage)
        {
            RectTransform anchor = UiBuild.Rect(parent, stage.Id); anchor.anchorMin = anchor.anchorMax = new Vector2(stage.MapPos[0], 1f - stage.MapPos[1]); anchor.sizeDelta = Vector2.zero; anchor.anchoredPosition = Vector2.zero;
            Image marker = UiBuild.Image(anchor, "Editor Marker", new Color(.75f, .12f, .08f, .62f)); marker.raycastTarget = false; marker.rectTransform.sizeDelta = new Vector2(58, 58);
            string suffix = stage.Kind == "boss" ? " B" : stage.Kind == "elite" ? " E" : string.Empty;
            Text label = UiBuild.Text(marker.transform, "Label", stage.Index + suffix, 22, TextAnchor.MiddleCenter); UiBuild.Stretch(label.rectTransform); label.raycastTarget = false; label.color = Color.white;
        }

        [MenuItem("One Bag Jianghu/Build/Rebuild PvE Art Library")]
        public static void RebuildArtLibrary()
        {
            AssetDatabase.ImportAsset("Assets/_Game/Textures2D/Pve", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset("Assets/_Game/Textures2D/UI", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            PveArtLibrary library = AssetDatabase.LoadAssetAtPath<PveArtLibrary>(ArtPath);
            if (library == null) { library = ScriptableObject.CreateInstance<PveArtLibrary>(); AssetDatabase.CreateAsset(library, ArtPath); }
            library.Entries.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Game/Textures2D/Pve", "Assets/_Game/Textures2D/UI" });
            Array.Sort(guids, (left, right) => string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(left), AssetDatabase.GUIDToAssetPath(right)));
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase)) continue;
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null) { AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate); sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path); }
                if (sprite == null) { Debug.LogWarning("PvE art is not imported as a Sprite: " + path); continue; }
                string key = Path.GetFileNameWithoutExtension(path); int duplicate = library.Entries.FindIndex(entry => entry.Key == key);
                if (duplicate >= 0) library.Entries[duplicate].Sprite = sprite;
                else library.Entries.Add(new PveArtLibrary.Entry { Key = key, Sprite = sprite });
            }
            EditorUtility.SetDirty(library); AssetDatabase.SaveAssets(); Debug.Log($"Rebuilt PvE art library with {library.Entries.Count} sprites.");
        }

        private static void BuildNode(bool overwrite)
        {
            string path = PrefabFolder + "/PveStageNode.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            GameObject root = new GameObject("PveStageNode", typeof(RectTransform)); root.GetComponent<RectTransform>().sizeDelta = new Vector2(116, 116);
            Image frame = UiBuild.Image(root.transform, "Frame", UiBuild.Paper); UiBuild.Stretch(frame.rectTransform); frame.preserveAspect = true; Button button = frame.gameObject.AddComponent<Button>();
            Text index = UiBuild.Text(frame.transform, "Index", "1", 34); UiBuild.Stretch(index.rectTransform);
            Text kind = UiBuild.Text(frame.transform, "Kind", "", 12); Anchor(kind.rectTransform, .05f, .68f, .95f, .95f);
            Text stars = UiBuild.Text(frame.transform, "Stars", "★★★", 18); Anchor(stars.rectTransform, .05f, -.22f, .95f, .04f); stars.color = UiBuild.Gold;
            Image locked = UiBuild.Image(frame.transform, "Lock", Color.white); Anchor(locked.rectTransform, .31f, .31f, .69f, .69f); locked.preserveAspect = true; locked.gameObject.SetActive(false);
            var starImages = new Image[3];
            for (int i = 0; i < 3; i++) { starImages[i] = UiBuild.Image(root.transform, "Star " + (i + 1), Color.white); RectTransform rect = starImages[i].rectTransform; rect.anchorMin = rect.anchorMax = new Vector2(.31f + i * .19f, -.08f); rect.sizeDelta = new Vector2(22, 22); starImages[i].preserveAspect = true; starImages[i].gameObject.SetActive(false); }
            PveStageNode node = root.AddComponent<PveStageNode>(); UiBuild.Set(node, "button", button); UiBuild.Set(node, "frame", frame); UiBuild.Set(node, "lockIcon", locked); UiBuild.Set(node, "indexText", index); UiBuild.Set(node, "starsText", stars); UiBuild.Set(node, "kindText", kind); SetArray(node, "starIcons", starImages);
            PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root);
        }

        private static void BuildMap(bool overwrite)
        {
            string path = PopupFolder + "/StageMapPopup.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            GameObject root = Root("StageMapPopup", out RectTransform panel); StageMapPopup popup = root.AddComponent<StageMapPopup>(); UiBuild.Set(popup, "panel", panel); UiBuild.SetBool(popup, "closeOnBackdrop", false);
            Image map = UiBuild.Image(panel, "Chapter Painting", UiBuild.Paper); Anchor(map.rectTransform, .02f, .08f, .70f, .88f); map.preserveAspect = true; RectTransform nodes = UiBuild.Rect(map.transform, "Stage Nodes"); UiBuild.Stretch(nodes);
            Image header = UiBuild.Image(panel, "Header", UiBuild.Vermilion); Anchor(header.rectTransform, .02f, .90f, .98f, .985f); Text title = UiBuild.Text(header.transform, "Chapter Title", "Hồi 1", 36, TextAnchor.MiddleCenter, UiFontRole.Display); Anchor(title.rectTransform, .03f, .02f, .68f, .98f);
            Image staminaIcon = Icon(header.transform, "Stamina Icon", .70f, .18f, .735f, .82f); Text stamina = UiBuild.Text(header.transform, "Stamina", "Thể Lực: 60", 24, TextAnchor.MiddleRight); Anchor(stamina.rectTransform, .73f, .05f, .97f, .95f);
            Image detail = UiBuild.Image(panel, "Stage Detail", UiBuild.Paper); Anchor(detail.rectTransform, .72f, .08f, .98f, .88f); Text name = T(detail.transform, "Name", "Tên ải", 30, .06f, .85f, .94f, .96f);
            Image portrait = UiBuild.Image(detail.transform, "Enemy Portrait", new Color(.2f, .2f, .2f, .2f)); Anchor(portrait.rectTransform, .22f, .57f, .78f, .83f); portrait.preserveAspect = true;
            Text enemy = T(detail.transform, "Enemy", "Địch", 20, .05f, .48f, .95f, .57f); Text desc = T(detail.transform, "Description", "Mô tả", 19, .07f, .34f, .93f, .48f); Text power = T(detail.transform, "Power", "Chiến lực đề nghị", 18, .06f, .27f, .94f, .34f);
            Image rewardIcon = Icon(detail.transform, "Reward Currency Icon", .05f, .205f, .12f, .255f); Text rewards = T(detail.transform, "Rewards", "Phần thưởng", 18, .12f, .19f, .94f, .27f); Text cost = T(detail.transform, "Cost", "Thể Lực", 18, .06f, .13f, .94f, .19f); Text reason = T(detail.transform, "Disabled Reason", "", 16, .06f, .03f, .94f, .09f); reason.color = UiBuild.Vermilion;
            UiButton prev = Btn(panel, "Previous", "‹ Hồi trước", false, new Vector2(.08f, .035f), new Vector2(220, 58)); UiButton next = Btn(panel, "Next", "Hồi sau ›", false, new Vector2(.61f, .035f), new Vector2(220, 58)); UiButton challenge = Btn(detail.transform, "Challenge", "Khiêu chiến", true, new Vector2(.5f, .10f), new Vector2(260, 62)); UiButton close = Btn(panel, "Close", "Đóng", false, new Vector2(.92f, .035f), new Vector2(180, 58));
            PveStageNode template = UiBuild.InstantiateLinked<PveStageNode>(PrefabFolder + "/PveStageNode.prefab", panel); template.name = "Stage Node Prefab"; template.gameObject.SetActive(false);
            Set(popup, map, header, detail, staminaIcon, rewardIcon, title, stamina, name, desc, enemy, power, rewards, cost, reason, portrait, nodes, template, prev, next, challenge, close);
            PveCatalog mapCatalog = PveCatalog.Load(); var mapPrefabs = new GameObject[mapCatalog.Chapters.Count];
            for (int i = 0; i < mapPrefabs.Length; i++) mapPrefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(MapPrefabFolder + "/Map_" + mapCatalog.Chapters[i].Id + ".prefab"); SetObjectArray(popup, "mapPrefabs", mapPrefabs);
            root.SetActive(false); PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root);
        }

        private static void BuildDialogue(bool overwrite)
        {
            string path = PopupFolder + "/DialoguePopup.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            GameObject root = Root("DialoguePopup", out RectTransform panel); DialoguePopup popup = root.AddComponent<DialoguePopup>(); UiBuild.Set(popup, "panel", panel); UiBuild.SetBool(popup, "closeOnBackdrop", false);
            RectTransform left = UiBuild.Rect(panel, "Portrait Left"); Anchor(left, .03f, .17f, .35f, .88f); RectTransform right = UiBuild.Rect(panel, "Portrait Right"); Anchor(right, .65f, .17f, .97f, .88f); Image portrait = UiBuild.Image(left, "Portrait", Color.white); UiBuild.Stretch(portrait.rectTransform); portrait.preserveAspect = true;
            Image box = UiBuild.Image(panel, "Dialogue Box", new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, .96f)); // frame_dialogue.png is a 3:1 scroll (rollers ~9% each side, paper band ~19-81% of its height): keep that aspect and keep text on the paper.
            RectTransform boxRect = box.rectTransform; boxRect.anchorMin = boxRect.anchorMax = new Vector2(.5f, .03f); boxRect.pivot = new Vector2(.5f, 0f); boxRect.sizeDelta = new Vector2(1266f, 422f); boxRect.anchoredPosition = Vector2.zero;
            Text speaker = T(box.transform, "Speaker", "Người nói", 30, .14f, .62f, .86f, .80f); Text body = T(box.transform, "Body", "Lời thoại", 24, .14f, .21f, .86f, .62f); body.alignment = TextAnchor.UpperLeft; Button advance = box.gameObject.AddComponent<Button>(); UiButton skip = Btn(panel, "Skip", "Bỏ qua", false, new Vector2(.91f, .93f), new Vector2(180, 56));
            UiBuild.Set(popup, "portrait", portrait); UiBuild.Set(popup, "dialogueFrame", box); UiBuild.Set(popup, "portraitLeft", left); UiBuild.Set(popup, "portraitRight", right); UiBuild.Set(popup, "speakerText", speaker); UiBuild.Set(popup, "bodyText", body); UiBuild.Set(popup, "advanceButton", advance); UiBuild.Set(popup, "skipButton", skip);
            root.SetActive(false); PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root);
        }

        private static Image Icon(Transform parent, string name, float x1, float y1, float x2, float y2) { Image image = UiBuild.Image(parent, name, Color.white); Anchor(image.rectTransform, x1, y1, x2, y2); image.preserveAspect = true; image.gameObject.SetActive(false); return image; }
        private static void SetArray(UnityEngine.Object target, string field, Image[] values) { var serialized = new SerializedObject(target); SerializedProperty property = serialized.FindProperty(field); property.arraySize = values.Length; for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i]; serialized.ApplyModifiedPropertiesWithoutUndo(); }
        private static void SetObjectArray(UnityEngine.Object target, string field, UnityEngine.Object[] values) { var serialized = new SerializedObject(target); SerializedProperty property = serialized.FindProperty(field); property.arraySize = values.Length; for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i]; serialized.ApplyModifiedPropertiesWithoutUndo(); }
        private static GameObject Root(string name, out RectTransform panel) { GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas), typeof(GraphicRaycaster)); UiBuild.Stretch(root.GetComponent<RectTransform>()); root.GetComponent<Canvas>().overrideSorting = true; Image paper = UiBuild.InstantiateLinked<Image>(CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab", root.transform); panel = paper.rectTransform; panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f); panel.sizeDelta = new Vector2(1760, 980); return root; }
        private static UiButton Btn(Transform parent, string name, string label, bool primary, Vector2 anchor, Vector2 size) { UiButton button = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/Button_" + (primary ? "Primary" : "Secondary") + ".prefab", parent); button.name = name; button.SetLabel(label); RectTransform rect = (RectTransform)button.transform; rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; return button; }
        private static Text T(Transform parent, string name, string value, int size, float x1, float y1, float x2, float y2) { UiFontRole role = name == "Name" || name == "Speaker" ? UiFontRole.Display : UiFontRole.Body; Text text = UiBuild.Text(parent, name, value, size, TextAnchor.MiddleCenter, role); Anchor(text.rectTransform, x1, y1, x2, y2); return text; }
        private static void Anchor(RectTransform rect, float x1, float y1, float x2, float y2) { rect.anchorMin = new Vector2(x1, y1); rect.anchorMax = new Vector2(x2, y2); rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void Set(StageMapPopup popup, Image map, Image header, Image detail, Image staminaIcon, Image rewardIcon, Text chapter, Text stamina, Text name, Text desc, Text enemy, Text power, Text rewards, Text cost, Text reason, Image portrait, RectTransform nodes, PveStageNode template, UiButton prev, UiButton next, UiButton challenge, UiButton close) { UiBuild.Set(popup, "mapBackground", map); UiBuild.Set(popup, "header", header); UiBuild.Set(popup, "detailFrame", detail); UiBuild.Set(popup, "staminaIcon", staminaIcon); UiBuild.Set(popup, "rewardCurrencyIcon", rewardIcon); UiBuild.Set(popup, "chapterText", chapter); UiBuild.Set(popup, "staminaText", stamina); UiBuild.Set(popup, "stageName", name); UiBuild.Set(popup, "descriptionText", desc); UiBuild.Set(popup, "enemyText", enemy); UiBuild.Set(popup, "powerText", power); UiBuild.Set(popup, "rewardsText", rewards); UiBuild.Set(popup, "costText", cost); UiBuild.Set(popup, "disabledReason", reason); UiBuild.Set(popup, "enemyPortrait", portrait); UiBuild.Set(popup, "nodeLayer", nodes); UiBuild.Set(popup, "nodePrefab", template); UiBuild.Set(popup, "previousButton", prev); UiBuild.Set(popup, "nextButton", next); UiBuild.Set(popup, "challengeButton", challenge); UiBuild.Set(popup, "closeButton", close); }
    }
}
