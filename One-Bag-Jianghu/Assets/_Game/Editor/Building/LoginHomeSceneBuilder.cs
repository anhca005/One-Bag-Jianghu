using System;
using System.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class LoginHomeSceneBuilder
    {
        private const string Common = "Assets/_Game/Prefabs/UI/Common/";
        private const string LoginPath = "Assets/_Game/Scenes/LoginScene.unity";
        private const string HomePath = "Assets/_Game/Scenes/HomeScene.unity";

        [MenuItem("One Bag Jianghu/Build/Login Scene")] public static void BuildLoginScene() => BuildLoginScene(false);
        [MenuItem("One Bag Jianghu/Build/Login Scene (Overwrite)")] public static void BuildLoginSceneOverwrite() => BuildLoginScene(true);
        [MenuItem("One Bag Jianghu/Build/Home Scene")] public static void BuildHomeScene() => BuildHomeScene(false);
        [MenuItem("One Bag Jianghu/Build/Home Scene (Overwrite)")] public static void BuildHomeSceneOverwrite() => BuildHomeScene(true);

        public static void Build(bool overwrite)
        {
            BuildLoginScene(overwrite);
            BuildHomeScene(overwrite);
        }

        public static void BuildLoginScene(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            GameControllerPrefabBuilder.Build(false);
            if (!ShouldBuild(LoginPath, overwrite, typeof(LoginController))) return;
            Scene scene = CreateScene("LoginScene", typeof(LoginController), out Component component, out Canvas canvas);
            GameController controllerRoot = UiBuild.InstantiateLinked<GameController>(GameControllerPrefabBuilder.Path, null);
            controllerRoot.gameObject.name = "GameController";
            var controller = (LoginController)component;
            Image background = canvas.transform.Find("Background").GetComponent<Image>();
            if (UiBuild.ApplyUiSprite(background, "bg_login"))
            {
                AspectRatioFitter backgroundFit = background.gameObject.AddComponent<AspectRatioFitter>();
                backgroundFit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; backgroundFit.aspectRatio = 1.5f;
            }
            Text title = Linked<Text>("Text_Title", background.transform, "Title");
            Place(title.rectTransform, new Vector2(0.27f, 0.69f), new Vector2(0.73f, 0.85f));
            title.text = "Nhất Nang Giang Hồ"; title.fontSize = 58;
            UiFonts.Apply(title, UiFontRole.Display); title.lineSpacing = 1.05f;
            RectTransform content = UiBuild.Rect(canvas.transform, "Content");
            Place(content, new Vector2(0.34f, 0.27f), new Vector2(0.66f, 0.68f));
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 24f; layout.childAlignment = TextAnchor.MiddleCenter; layout.padding = new RectOffset(24, 24, 24, 24);
            Text prompt = Linked<Text>("Text_Body", content, "HeroNameLabel"); prompt.text = "Tên Hiệp Khách";
            InputField heroName = Linked<InputField>("InputField", content, "HeroNameInput"); heroName.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 64f);
            Button enter = Linked<Button>("Button_Primary", content, "Btn_Enter"); enter.GetComponent<OneBagJianghu.UI.Common.UiButton>().SetLabel("Vào Giang Hồ");
            Preferred(heroName.gameObject, 620f, 64f); Preferred(enter.gameObject, 360f, 76f);
            UiBuild.Set(controller, "heroName", heroName); UiBuild.Set(controller, "enterButton", enter);
            EditorSceneManager.SaveScene(scene, LoginPath); EnsureBuildSettings();
        }

        public static void BuildHomeScene(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            HomeHotspotPrefabBuilder.Build(overwrite);
            if (!ShouldBuild(HomePath, overwrite, typeof(HomeController))) return;
            Scene scene = CreateScene("HomeScene", typeof(HomeController), out Component component, out Canvas canvas);
            var controller = (HomeController)component;
            Image background = canvas.transform.Find("Background").GetComponent<Image>();
            background.color = UiBuild.Paper;
            UiBuild.ApplyUiSprite(background, "bg_home");
            AspectRatioFitter backgroundFit = background.gameObject.AddComponent<AspectRatioFitter>();
            backgroundFit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; backgroundFit.aspectRatio = 1.5f;

            Image hero = UiBuild.Image(canvas.transform, "HeroZone", new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.12f));
            PixelRect(hero.rectTransform, 810f, 320f, 300f, 600f, new Vector2(0.5f, 0f)); hero.preserveAspect = true; hero.raycastTarget = false;
            if (UiBuild.ApplyUiSprite(hero, "hero_idle"))
            {
                hero.rectTransform.pivot = new Vector2(0.5f, 0f);
                hero.rectTransform.anchoredPosition = new Vector2(0f, 160f);
                hero.rectTransform.sizeDelta = new Vector2(300f, 450f);
            }

            Image header = UiBuild.Image(canvas.transform, "Header", new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, 0.88f));
            TopStretch(header.rectTransform, 80f);
            Text heroName = UiBuild.Text(header.transform, "HeroName", "Vân Du", 28, TextAnchor.MiddleLeft, UiFontRole.Title); Rect(heroName.rectTransform, 32f, 10f, 260f, 60f);
            Slider exp = Slider(header.transform, "ExpBar"); Rect(exp.GetComponent<RectTransform>(), 300f, 20f, 280f, 40f);
            Text expText = UiBuild.Text(exp.transform, "ExpText", "EXP 0/100", 16); UiBuild.Stretch(expText.rectTransform, 5f); expText.raycastTarget = false;
            Text level = UiBuild.Text(header.transform, "LevelText", "Cấp 1", 20, TextAnchor.MiddleCenter, UiFontRole.Title); Rect(level.rectTransform, 590f, 10f, 100f, 60f);
            Text stamina;
            Text silver;
            Button settings = LinkedButton("Button_Secondary", header.transform, "Btn_Settings", "Cài đặt"); Rect(settings.GetComponent<RectTransform>(), 1700f, 10f, 180f, 60f); settings.GetComponent<Image>().pixelsPerUnitMultiplier = 10f; UiBuild.FitLabel(settings.GetComponentInChildren<Text>(), UiBuild.LabelFitPreset.OrnateButton); settings.GetComponent<OneBagJianghu.UI.Common.UiButton>().SetLabel("Cài đặt");

            Text premium;
            Text reputation;
            RectTransform currencyRow = UiBuild.Rect(header.transform, "Currency Row"); Rect(currencyRow, 990f, 10f, 700f, 60f);
            HorizontalLayoutGroup currencyLayout = currencyRow.gameObject.AddComponent<HorizontalLayoutGroup>(); currencyLayout.spacing = 10f; currencyLayout.childAlignment = TextAnchor.MiddleLeft; currencyLayout.childControlWidth = true; currencyLayout.childControlHeight = true; currencyLayout.childForceExpandWidth = false;
            stamina = CurrencyEntry(currencyRow, "StaminaText", "Thể Lực 120/120", 165f);
            silver = CurrencyEntry(currencyRow, "SilverText", "Bạc 8,800", 125f);
            premium = CurrencyEntry(currencyRow, "PremiumText", "Nguyên Bảo 0", 185f);
            reputation = CurrencyEntry(currencyRow, "ReputationText", "Danh Vọng 0", 165f);
            RectTransform hotspots = UiBuild.Rect(canvas.transform, "Hotspots"); UiBuild.Stretch(hotspots);
            HomeHotspot pve = Hotspot(hotspots, "Hotspot_Pve", "Hành Tẩu Giang Hồ", "Vào ải hành tẩu", 1380, 380, 260, 200, new Vector2(1f, 0.5f), false);
            HomeHotspot pvp = Hotspot(hotspots, "Hotspot_Pvp", "Luận Kiếm Đài", "Tỷ thí cùng quần hùng", 1600, 140, 240, 180, Vector2.one, false);
            HomeHotspot bag = Hotspot(hotspots, "Hotspot_Bag", "Bao Phục", "Sắp xếp hành trang", 430, 790, 240, 140, new Vector2(0.5f, 0f), false);
            HomeHotspot storage = Hotspot(hotspots, "Hotspot_Storage", "Tàng Bảo Các", "Kho báu và vật phẩm", 1640, 560, 240, 220, new Vector2(1f, 0f), false);
            HomeHotspot codex = Hotspot(hotspots, "Hotspot_Codex", "Vạn Bảo Phổ", "Xem đồ giám giang hồ", 80, 680, 220, 160, Vector2.zero, false);
            HomeHotspot shop = Hotspot(hotspots, "Hotspot_Shop", "Thương Hội", "Giao dịch vật phẩm", 380, 400, 300, 200, new Vector2(0f, 0.5f), false);
            HomeHotspot sect = Hotspot(hotspots, "Hotspot_Sect", "Môn Phái", "Đạt cấp 10 mở khóa gia nhập Môn Phái", 250, 90, 240, 180, new Vector2(0f, 1f), true);

            UiBuild.Set(controller, "background", background); UiBuild.Set(controller, "heroZone", hero); UiBuild.Set(controller, "heroName", heroName);
            UiBuild.Set(controller, "expBar", exp); UiBuild.Set(controller, "expText", expText); UiBuild.Set(controller, "levelText", level); UiBuild.Set(controller, "settingsButton", settings);
            UiBuild.Set(controller, "staminaText", stamina); UiBuild.Set(controller, "silverText", silver); UiBuild.Set(controller, "premiumText", premium); UiBuild.Set(controller, "reputationText", reputation);
            UiBuild.Set(controller, "pveHotspot", pve); UiBuild.Set(controller, "pvpHotspot", pvp); UiBuild.Set(controller, "bagHotspot", bag);
            UiBuild.Set(controller, "storageHotspot", storage); UiBuild.Set(controller, "codexHotspot", codex); UiBuild.Set(controller, "shopHotspot", shop); UiBuild.Set(controller, "sectHotspot", sect);
            EditorSceneManager.SaveScene(scene, HomePath); EnsureBuildSettings();
        }

        private static HomeHotspot Hotspot(Transform parent, string name, string label, string tooltip, float x, float y, float width, float height, Vector2 anchor, bool locked)
        {
            HomeHotspot hotspot = UiBuild.InstantiateLinked<HomeHotspot>(HomeHotspotPrefabBuilder.Path, parent); hotspot.gameObject.name = name;
            PixelRect((RectTransform)hotspot.transform, x, y, width, height, anchor); hotspot.Configure(label, tooltip, locked, locked ? 10 : 0, false); return hotspot;
        }

        private static Slider Slider(Transform parent, string name)
        {
            Image root = UiBuild.Image(parent, name, new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, 0.18f)); Slider slider = root.gameObject.AddComponent<Slider>();
            Image fill = UiBuild.Image(root.transform, "Fill", UiBuild.Gold); UiBuild.Stretch(fill.rectTransform, 3f); slider.fillRect = fill.rectTransform; slider.targetGraphic = fill; slider.interactable = false; return slider;
        }

        private static Scene CreateScene(string name, Type controllerType, out Component controller, out Canvas canvas)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneBuild.CreateMainCamera();
            SceneBuild.CreateEventSystem();
            controller = new GameObject(controllerType.Name, controllerType).GetComponent(controllerType);
            canvas = UiBuild.Canvas(null, "Canvas", 0); Image background = UiBuild.Image(canvas.transform, "Background", UiBuild.Paper); UiBuild.Stretch(background.rectTransform); return scene;
        }

        private static T Linked<T>(string prefabName, Transform parent, string name) where T : Component { T value = UiBuild.InstantiateLinked<T>(Common + prefabName + ".prefab", parent); value.gameObject.name = name; return value; }
        private static void Place(RectTransform rect, Vector2 min, Vector2 max) { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero; }
        private static void Rect(RectTransform rect, float x, float y, float width, float height) { rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f); rect.pivot = new Vector2(0f, 1f); rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height); }
        private static void TopStretch(RectTransform rect, float height) { rect.anchorMin = new Vector2(0f, 1f); rect.anchorMax = Vector2.one; rect.pivot = new Vector2(0.5f, 1f); rect.offsetMin = new Vector2(0f, -height); rect.offsetMax = Vector2.zero; }
        private static Button LinkedButton(string prefab, Transform parent, string name, string label)
        {
            Button button = Linked<Button>(prefab, parent, name);
            button.GetComponent<OneBagJianghu.UI.Common.UiButton>().SetLabel(label);
            return button;
        }

        private static void Preferred(GameObject gameObject, float width, float height)
        {
            LayoutElement element = gameObject.GetComponent<LayoutElement>();
            if (element == null) element = gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = width; element.preferredHeight = height;
            ((RectTransform)gameObject.transform).sizeDelta = new Vector2(width, height);
        }

        private static Text CurrencyEntry(Transform parent, string name, string value, float width)
        {
            RectTransform entry = UiBuild.Rect(parent, name + " Entry");
            LayoutElement entrySize = entry.gameObject.AddComponent<LayoutElement>(); entrySize.preferredWidth = width; entrySize.preferredHeight = 60f;
            HorizontalLayoutGroup layout = entry.gameObject.AddComponent<HorizontalLayoutGroup>(); layout.spacing = 6f; layout.childAlignment = TextAnchor.MiddleLeft; layout.childControlWidth = true; layout.childControlHeight = true; layout.childForceExpandWidth = false; layout.childForceExpandHeight = false;
            Image icon = UiBuild.Image(entry, "Icon", Color.white); icon.preserveAspect = true; icon.raycastTarget = false;
            LayoutElement iconSize = icon.gameObject.AddComponent<LayoutElement>(); iconSize.minWidth = iconSize.preferredWidth = 26f; iconSize.minHeight = iconSize.preferredHeight = 26f;
            Text text = UiBuild.Text(entry, name, value, 22, TextAnchor.MiddleLeft, UiFontRole.Body); text.horizontalOverflow = HorizontalWrapMode.Overflow;
            ContentSizeFitter fitter = text.gameObject.AddComponent<ContentSizeFitter>(); fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            return text;
        }

        private static void PixelRect(RectTransform rect, float x, float y, float width, float height, Vector2 anchor)
        {
            rect.anchorMin = rect.anchorMax = anchor; rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x - anchor.x * 1920f, -(y - (1f - anchor.y) * 1080f)); rect.sizeDelta = new Vector2(width, height);
        }

        private static bool ShouldBuild(string path, bool overwrite, Type controllerType)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) return true;
            if (!overwrite && IsLegacyShell(path, controllerType)) return true;
            return AssetGuard.ShouldBuild(path, overwrite);
        }

        private static bool IsLegacyShell(string path, Type controllerType)
        {
            Scene scene = SceneManager.GetSceneByPath(path); bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            GameObject[] roots = scene.GetRootGameObjects(); bool hasController = roots.SelectMany(root => root.GetComponentsInChildren(controllerType, true)).Any();
            bool hasCanvas = roots.SelectMany(root => root.GetComponentsInChildren<Canvas>(true)).Any(); bool incomplete = hasController && !hasCanvas;
            if (opened) EditorSceneManager.CloseScene(scene, true); return incomplete;
        }

        private static void EnsureBuildSettings()
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Login), true), new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Home), true), new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Gameplay), true) };
        }
    }

    public static class HomeHotspotPrefabBuilder
    {
        public const string Path = "Assets/_Game/Prefabs/UI/Home/Hotspot.prefab";
        [MenuItem("One Bag Jianghu/Build/Home Hotspot Prefab")] public static void BuildHomeHotspotPrefab() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Home Hotspot Prefab (Overwrite)")] public static void BuildHomeHotspotPrefabOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder("Assets/_Game/Prefabs/UI/Home"); if (!AssetGuard.ShouldBuild(Path, overwrite)) return;
            Image area = UiBuild.Image(null, "Hotspot", Color.clear); area.raycastTarget = true; HomeHotspot hotspot = area.gameObject.AddComponent<HomeHotspot>();
            Image plaque = UiBuild.Image(area.transform, "Plaque", new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, 0.94f));
            UiBuild.ApplyUiSprite(plaque, "plaque_label", Image.Type.Sliced);
            plaque.rectTransform.anchorMin = new Vector2(0f, 0.36f); plaque.rectTransform.anchorMax = new Vector2(1f, 0.82f); plaque.rectTransform.offsetMin = Vector2.zero; plaque.rectTransform.offsetMax = Vector2.zero;
            Outline outline = plaque.gameObject.AddComponent<Outline>(); outline.effectColor = UiBuild.Gold; outline.effectDistance = new Vector2(2f, -2f); outline.enabled = false;
            Text label = UiBuild.Text(plaque.transform, "Label", "Hotspot", 22, TextAnchor.MiddleCenter, UiFontRole.Title); UiBuild.FitLabel(label, UiBuild.LabelFitPreset.Plaque);
            Text tooltip = UiBuild.Text(area.transform, "Tooltip", "Tooltip", 16); tooltip.rectTransform.anchorMin = new Vector2(0f, 0.12f); tooltip.rectTransform.anchorMax = new Vector2(1f, 0.34f); tooltip.rectTransform.offsetMin = Vector2.zero; tooltip.rectTransform.offsetMax = Vector2.zero; tooltip.gameObject.SetActive(false);
            RectTransform lockContainer = UiBuild.Rect(plaque.transform, "LockContainer"); UiBuild.Stretch(lockContainer);
            Image lockIcon = UiBuild.Image(lockContainer, "LockIcon", UiBuild.Gold); Rect(lockIcon.rectTransform, 10, 10, 36, 36); lockIcon.preserveAspect = true;
            UiBuild.ApplyUiSprite(lockIcon, "icon_lock");
            Text lockText = UiBuild.Text(lockContainer, "LockText", "Cấp 10", 17, TextAnchor.MiddleRight); lockText.rectTransform.anchorMin = new Vector2(0.52f, 0f); lockText.rectTransform.anchorMax = Vector2.one; lockText.rectTransform.offsetMin = new Vector2(0f, 4f); lockText.rectTransform.offsetMax = new Vector2(-8f, -4f);
            Image badge = UiBuild.Image(plaque.transform, "Badge", UiBuild.Vermilion); badge.rectTransform.anchorMin = badge.rectTransform.anchorMax = Vector2.one; badge.rectTransform.pivot = new Vector2(0.5f, 0.5f); badge.rectTransform.anchoredPosition = Vector2.zero; badge.rectTransform.sizeDelta = new Vector2(52f, 52f);
            UiBuild.ApplyUiSprite(badge, "badge_new"); badge.preserveAspect = true;
            Text badgeText = UiBuild.Text(badge.transform, "Label", "Mới", 15); badgeText.color = UiBuild.Paper; UiBuild.Stretch(badgeText.rectTransform);
            UiBuild.Set(hotspot, "area", area); UiBuild.Set(hotspot, "plaque", plaque.rectTransform); UiBuild.Set(hotspot, "plaqueImage", plaque); UiBuild.Set(hotspot, "plaqueOutline", outline); UiBuild.Set(hotspot, "label", label); UiBuild.Set(hotspot, "tooltip", tooltip);
            UiBuild.Set(hotspot, "lockContainer", lockContainer.gameObject); UiBuild.Set(hotspot, "lockIcon", lockIcon); UiBuild.Set(hotspot, "lockText", lockText); UiBuild.Set(hotspot, "badge", badge.gameObject); UiBuild.Set(hotspot, "badgeImage", badge);
            lockContainer.gameObject.SetActive(false); badge.gameObject.SetActive(false); PrefabUtility.SaveAsPrefabAsset(area.gameObject, Path); UnityEngine.Object.DestroyImmediate(area.gameObject); AssetDatabase.SaveAssets();
        }

        private static void Rect(RectTransform rect, float x, float y, float width, float height) { rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f); rect.pivot = new Vector2(0f, 1f); rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height); }
    }
}
