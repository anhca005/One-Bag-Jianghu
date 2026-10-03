using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Battle.Replay.View.VfxLab;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class DemoSceneBuilder
    {
        private const string SceneFolder = "Assets/_Game/Scenes";
        private const string ToolFolder = "Assets/_Game/Prefabs/UI/Tools";
        private const string BagPrefab = "Assets/_Game/Prefabs/UI/Bag/BagLoadoutPanel.prefab";
        private const string ReplayPrefab = "Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab";
        private const string FighterPrefab = "Assets/_Game/Prefabs/UI/Replay/FighterPanel.prefab";
        private const string EffectPrefab = "Assets/_Game/Prefabs/UI/Replay/VfxEffect.prefab";

        [MenuItem("One Bag Jianghu/Build/Demo Scenes")]
        public static void BuildDemoScenes() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Demo Scenes (Overwrite)")]
        public static void BuildDemoScenesOverwrite() => Build(true);
        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);
        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(SceneFolder); UiBuild.EnsureFolder(ToolFolder);
            BuildCatalogRow(overwrite); BuildBag(overwrite); BuildReplay(overwrite); BuildVfxLab(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildCatalogRow(bool overwrite)
        {
            string path = ToolFolder + "/VfxCatalogRow.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image root = UiBuild.Image(null, "VfxCatalogRow", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .06f));
            root.rectTransform.sizeDelta = new Vector2(410f, 58f); Button button = root.gameObject.AddComponent<Button>(); button.targetGraphic = root;
            Text status = UiBuild.Text(root.transform, "Status", "✓", 24); Place(status.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(8f, 0f), new Vector2(42f, 0f));
            Text label = UiBuild.Text(root.transform, "Label", "VFX label", 18, TextAnchor.UpperLeft); Place(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(50f, 25f), new Vector2(-38f, 0f));
            Text key = UiBuild.Text(root.transform, "Key", "vfx_key", 14, TextAnchor.LowerLeft); Place(key.rectTransform, Vector2.zero, Vector2.one, new Vector2(50f, 0f), new Vector2(-38f, -26f));
            Text tuned = UiBuild.Text(root.transform, "Tuned", "", 20, TextAnchor.MiddleRight); Place(tuned.rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-36f, 0f), new Vector2(-6f, 0f));
            VfxCatalogRow row = root.gameObject.AddComponent<VfxCatalogRow>(); Set(row, "background", root); Set(row, "status", status); Set(row, "label", label); Set(row, "key", key); Set(row, "tuned", tuned); Set(row, "button", button);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, path); Object.DestroyImmediate(root.gameObject);
        }

        private static void BuildBag(bool overwrite)
        {
            string path = SceneFolder + "/BagDemo.unity"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); SceneBuild.CreateMainCamera(); SceneBuild.CreateEventSystem();
            BagDemoBootstrap bootstrap = new GameObject("BagDemoBootstrap", typeof(BagDemoBootstrap)).GetComponent<BagDemoBootstrap>();
            Canvas canvas = UiBuild.Canvas(null, "Canvas", 0); Image background = UiBuild.Image(canvas.transform, "Background", UiBuild.Paper); UiBuild.Stretch(background.rectTransform);
            Text title = UiBuild.Text(canvas.transform, "Header", "BAG DEMO", 38, TextAnchor.MiddleCenter, UiFontRole.Title); Place(title.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(50f, -90f), new Vector2(-50f, -20f));
            BagLoadoutPanel panel = UiBuild.InstantiateLinked<BagLoadoutPanel>(BagPrefab, canvas.transform); Place((RectTransform)panel.transform, Vector2.zero, Vector2.one, new Vector2(20f, 90f), new Vector2(-20f, -100f));
            RectTransform footer = UiBuild.Rect(canvas.transform, "Footer"); Place(footer, Vector2.zero, new Vector2(1f, 0f), new Vector2(50f, 18f), new Vector2(-50f, 82f)); HorizontalLayoutGroup layout = footer.gameObject.AddComponent<HorizontalLayoutGroup>(); layout.spacing = 14f; layout.childControlWidth = false;
            Button save = Button(footer, "Save", "Save", 150f), load = Button(footer, "Load", "Load", 150f), clear = Button(footer, "Clear", "Clear", 150f);
            Set(bootstrap, "loadoutPanel", panel); Set(bootstrap, "saveButton", save); Set(bootstrap, "loadButton", load); Set(bootstrap, "clearButton", clear);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void BuildReplay(bool overwrite)
        {
            string path = SceneFolder + "/ReplayDemo.unity"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); SceneBuild.CreateMainCamera(); SceneBuild.CreateEventSystem();
            ReplayDemoBootstrap bootstrap = new GameObject("ReplayDemoBootstrap", typeof(ReplayDemoBootstrap)).GetComponent<ReplayDemoBootstrap>();
            Canvas canvas = UiBuild.Canvas(null, "Canvas", 0); ReplayScreen screen = UiBuild.InstantiateLinked<ReplayScreen>(ReplayPrefab, canvas.transform); UiBuild.Stretch((RectTransform)screen.transform);
            Set(bootstrap, "log", AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/_Game/Resources/Replays/replay_sample.json")); Set(bootstrap, "replayScreen", screen);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void BuildVfxLab(bool overwrite)
        {
            string path = SceneFolder + "/VfxLab.unity"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); SceneBuild.CreateMainCamera(); SceneBuild.CreateEventSystem();
            VfxLabBootstrap bootstrap = new GameObject("VfxLabBootstrap", typeof(VfxLabBootstrap)).GetComponent<VfxLabBootstrap>();
            Canvas canvas = UiBuild.Canvas(null, "Canvas", 0); Image background = UiBuild.Image(canvas.transform, "Background", ReplayStyle.Paper); UiBuild.Stretch(background.rectTransform);
            Image catalog = UiBuild.Image(background.transform, "Catalog", ReplayStyle.Panel); Place(catalog.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(22f, 158f), new Vector2(468f, -22f));
            Text title = UiBuild.Text(catalog.transform, "Title", "VFX LAB", 34, TextAnchor.MiddleLeft, UiFontRole.Title); Place(title.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(18f, -64f), new Vector2(-150f, -12f));
            Text count = UiBuild.Text(catalog.transform, "Count", "0/39 có hình", 18, TextAnchor.MiddleRight); Place(count.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(250f, -64f), new Vector2(-18f, -12f));
            Image viewport = UiBuild.Image(catalog.transform, "Viewport", new Color(1f, 1f, 1f, .12f)); Place(viewport.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 12f), new Vector2(-12f, -72f)); viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Rows"); content.anchorMin = new Vector2(0f, 1f); content.anchorMax = Vector2.one; content.pivot = new Vector2(.5f, 1f); content.offsetMin = content.offsetMax = Vector2.zero;
            VerticalLayoutGroup vertical = content.gameObject.AddComponent<VerticalLayoutGroup>(); vertical.spacing = 3f; vertical.padding = new RectOffset(6, 6, 6, 6); vertical.childControlHeight = false; vertical.childControlWidth = true; content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport.rectTransform; scroll.content = content; scroll.horizontal = false;

            Image stage = UiBuild.Image(background.transform, "Stage", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .035f)); Place(stage.rectTransform, Vector2.zero, Vector2.one, new Vector2(486f, 426f), new Vector2(-22f, -22f));
            Text stageLabel = UiBuild.Text(stage.transform, "Current VFX", "VFX", 30, TextAnchor.MiddleCenter, UiFontRole.Title); Place(stageLabel.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(12f, -92f), new Vector2(-12f, -12f));
            FighterPanelView leftPanel = UiBuild.InstantiateLinked<FighterPanelView>(FighterPrefab, stage.transform), rightPanel = UiBuild.InstantiateLinked<FighterPanelView>(FighterPrefab, stage.transform);
            Place((RectTransform)leftPanel.transform, new Vector2(0f, .1f), new Vector2(0f, .78f), new Vector2(40f, 0f), new Vector2(330f, 0f)); Place((RectTransform)rightPanel.transform, new Vector2(1f, .1f), new Vector2(1f, .78f), new Vector2(-330f, 0f), new Vector2(-40f, 0f));
            RectTransform layerRect = UiBuild.Rect(stage.transform, "Replay VFX Layer"); UiBuild.Stretch(layerRect); ReplayVfxLayer layer = layerRect.gameObject.AddComponent<ReplayVfxLayer>(); Set(layer, "effectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(EffectPrefab).GetComponent<Image>());

            Image inspector = UiBuild.Image(background.transform, "Inspector", ReplayStyle.Panel); Place(inspector.rectTransform, new Vector2(0f, 0f), Vector2.one, new Vector2(486f, 158f), new Vector2(-22f, 414f));
            RectTransform sizeBox = UiBuild.Rect(inspector.transform, "In-game size"); sizeBox.anchorMin = sizeBox.anchorMax = new Vector2(0f, .5f); sizeBox.pivot = new Vector2(0f, .5f); sizeBox.anchoredPosition = new Vector2(22f, 0f); sizeBox.sizeDelta = new Vector2(180f, 90f);
            Image raw = UiBuild.Image(sizeBox, "Raw sprite", Color.white); UiBuild.Stretch(raw.rectTransform); raw.preserveAspect = true; raw.raycastTarget = false;
            Text metrics = UiBuild.Text(inspector.transform, "Metrics", "Metrics", 20, TextAnchor.MiddleLeft); Place(metrics.rectTransform, Vector2.zero, Vector2.one, new Vector2(230f, 8f), new Vector2(-520f, -8f));
            RectTransform tuning = UiBuild.Rect(inspector.transform, "Tuning"); Place(tuning, new Vector2(1f, 0f), Vector2.one, new Vector2(-500f, 8f), new Vector2(-8f, -8f));
            Text tuningTitle = UiBuild.Text(tuning, "Title", "TUNING", 22, TextAnchor.UpperLeft, UiFontRole.Title); Place(tuningTitle.rectTransform, new Vector2(0f, 1f), Vector2.one, Vector2.zero, new Vector2(-300f, -30f));
            Slider rotation = Slider(tuning, "Rotation", -180f, 180f, 45f, out Text rotationValue), scale = Slider(tuning, "Scale", .2f, 4f, 82f, out Text scaleValue), offsetX = Slider(tuning, "Offset X", -200f, 200f, 119f, out Text offsetXValue), offsetY = Slider(tuning, "Offset Y", -200f, 200f, 156f, out Text offsetYValue);
            Toggle flipX = Toggle(tuning, "Flip X", 0f), flipY = Toggle(tuning, "Flip Y", 110f); Button reset = ButtonAt(tuning, "Reset", "Reset", 220f, 90f), save = ButtonAt(tuning, "Save", "Save", 320f, 90f); Text unsaved = UiBuild.Text(tuning, "Unsaved", "", 16, TextAnchor.MiddleLeft); Place(unsaved.rectTransform, Vector2.zero, new Vector2(1f, 0f), new Vector2(410f, 8f), new Vector2(0f, 42f));

            Image controls = UiBuild.Image(background.transform, "Controls", ReplayStyle.Panel); Place(controls.rectTransform, Vector2.zero, new Vector2(1f, 0f), new Vector2(22f, 18f), new Vector2(-22f, 146f));
            RectTransform first = Row(controls.transform, 69f, 12f); Button play = Button(first, "Play", "Play", 90f), loop = Button(first, "Loop", "Loop: Off", 120f), side = Button(first, "Side", "Side: Left", 135f);
            Button[] speeds = { Button(first, "Speed .25", ".25×", 70f), Button(first, "Speed .5", ".5×", 70f), Button(first, "Speed 1", "1×", 70f), Button(first, "Speed 2", "2×", 70f) };
            Button playAll = Button(first, "Play All", "Play all", 115f), previous = Button(first, "Previous", "Prev", 85f), next = Button(first, "Next", "Next", 85f);
            RectTransform second = Row(controls.transform, 8f, -59f); Button bg = Button(second, "Background", "BG: Paper", 150f), hit = Button(second, "Hit After", "Hit after: On", 160f); Text hint = UiBuild.Text(second, "Keyboard", "Space play | ←/→ prev/next | L loop | Q/E rotate | Z/X scale | W/A/S/D offset | Shift: 1°", 16, TextAnchor.MiddleLeft); hint.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            Set(bootstrap, "rootBackground", background); Set(bootstrap, "catalogContent", content); Set(bootstrap, "catalogRowPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(ToolFolder + "/VfxCatalogRow.prefab").GetComponent<VfxCatalogRow>()); Set(bootstrap, "countLabel", count);
            Set(bootstrap, "leftFighter", leftPanel.GetComponent<RectTransform>()); Set(bootstrap, "rightFighter", rightPanel.GetComponent<RectTransform>()); Set(bootstrap, "vfxLayer", layer); Set(bootstrap, "stageLabel", stageLabel); Set(bootstrap, "inspectorBox", sizeBox); Set(bootstrap, "inspectorImage", raw); Set(bootstrap, "inspectorText", metrics);
            Set(bootstrap, "playButton", play); Set(bootstrap, "loopButton", loop); Set(bootstrap, "sideButton", side); SetArray(bootstrap, "speedButtons", speeds); Set(bootstrap, "playAllButton", playAll); Set(bootstrap, "previousButton", previous); Set(bootstrap, "nextButton", next); Set(bootstrap, "backgroundButton", bg); Set(bootstrap, "hitButton", hit);
            Set(bootstrap, "rotationSlider", rotation); Set(bootstrap, "scaleSlider", scale); Set(bootstrap, "offsetXSlider", offsetX); Set(bootstrap, "offsetYSlider", offsetY); Set(bootstrap, "flipXToggle", flipX); Set(bootstrap, "flipYToggle", flipY); Set(bootstrap, "rotationValue", rotationValue); Set(bootstrap, "scaleValue", scaleValue); Set(bootstrap, "offsetXValue", offsetXValue); Set(bootstrap, "offsetYValue", offsetYValue); Set(bootstrap, "resetButton", reset); Set(bootstrap, "saveButton", save); Set(bootstrap, "unsavedLabel", unsaved);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static Button Button(Transform parent, string name, string label, float width) { Button value = UiBuild.Button(parent, name, label, false); value.gameObject.AddComponent<LayoutElement>().preferredWidth = width; return value; }
        private static Button ButtonAt(Transform parent, string name, string label, float left, float width) { Button value = UiBuild.Button(parent, name, label, false); RectTransform rect = value.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = Vector2.zero; rect.pivot = Vector2.zero; rect.anchoredPosition = new Vector2(left, 8f); rect.sizeDelta = new Vector2(width, 34f); return value; }
        private static Toggle Toggle(Transform parent, string label, float left) { Image box = UiBuild.Image(parent, label, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .15f)); box.rectTransform.anchorMin = box.rectTransform.anchorMax = Vector2.zero; box.rectTransform.pivot = Vector2.zero; box.rectTransform.anchoredPosition = new Vector2(left, 8f); box.rectTransform.sizeDelta = new Vector2(100f, 34f); Toggle toggle = box.gameObject.AddComponent<Toggle>(); toggle.targetGraphic = box; Image check = UiBuild.Image(box.transform, "Checkmark", ReplayStyle.Gold); Place(check.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(5f, 5f), new Vector2(27f, -5f)); toggle.graphic = check; UiBuild.Stretch(UiBuild.Text(box.transform, "Label", label, 16, TextAnchor.MiddleRight).rectTransform); return toggle; }
        private static Slider Slider(Transform parent, string label, float min, float max, float top, out Text value) { Text caption = UiBuild.Text(parent, label, label, 17, TextAnchor.MiddleLeft); Place(caption.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -top - 30f), new Vector2(-390f, -top)); Image track = UiBuild.Image(parent, label + " Slider", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .18f)); Place(track.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(90f, -top - 25f), new Vector2(-70f, -top - 5f)); Slider slider = track.gameObject.AddComponent<Slider>(); slider.minValue = min; slider.maxValue = max; slider.targetGraphic = track; Image fill = UiBuild.Image(track.transform, "Fill", ReplayStyle.Gold); UiBuild.Stretch(fill.rectTransform); slider.fillRect = fill.rectTransform; value = UiBuild.Text(parent, label + " Value", "", 16, TextAnchor.MiddleRight); Place(value.rectTransform, new Vector2(1f, 1f), Vector2.one, new Vector2(-68f, -top - 30f), new Vector2(0f, -top)); return slider; }
        private static RectTransform Row(Transform parent, float top, float bottom) { RectTransform rect = UiBuild.Rect(parent, "Row"); Place(rect, Vector2.zero, Vector2.one, new Vector2(12f, top), new Vector2(-12f, bottom)); HorizontalLayoutGroup layout = rect.gameObject.AddComponent<HorizontalLayoutGroup>(); layout.spacing = 8f; layout.childControlHeight = true; layout.childForceExpandHeight = true; layout.childControlWidth = false; return rect; }
        private static void Place(RectTransform rect, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax) { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = offsetMin; rect.offsetMax = offsetMax; }
        private static void Set(Object target, string name, Object value) => UiBuild.Set(target, name, value);
        private static void SetArray(Object target, string name, Object[] values) { SerializedObject serialized = new SerializedObject(target); SerializedProperty property = serialized.FindProperty(name); property.arraySize = values.Length; for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i]; serialized.ApplyModifiedPropertiesWithoutUndo(); }
    }
}
