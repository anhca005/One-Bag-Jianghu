using System;
using System.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Common;
using OneBagJianghu.Onboarding;
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
            // LoginScene is edited by hand. "Build All (Overwrite)" runs without a dialog in batch mode, so it only
            // creates the scene when missing; use the explicit Login Scene (Overwrite) menu to rebuild it.
            BuildLoginScene(false);
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
            controller.gameObject.AddComponent<OnboardingRuntime>();
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

            // The banner is 9-sliced so the rolled scroll ends keep their shape.
            Color headerFallback = new Color(UiBuild.Paper.r, UiBuild.Paper.g, UiBuild.Paper.b, .88f);
            Image header = UiBuild.Image(canvas.transform, "Header", headerFallback);
            TopStretch(header.rectTransform, 132f);
            if (UiBuild.ApplyUiSprite(header, "home_banner", Image.Type.Sliced))
            {
                header.color = Color.white;
                // Scale the ends with the banner height (art is 390 px tall) instead of stretching them sideways.
                header.pixelsPerUnitMultiplier = 390f / 132f;
            }

            // Crop the half-body portrait to the frame's inner circle, anchored at the top so the face stays visible.
            RectTransform avatar = UiBuild.Rect(header.transform, "Avatar");
            Rect(avatar, 76f, 34f, 76f, 76f);
            Image avatarMask = avatar.gameObject.AddComponent<Image>();
            avatarMask.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            avatar.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            Color portraitFallback = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .15f);
            Image portrait = UiBuild.Image(avatar, "Portrait", portraitFallback);
            portrait.rectTransform.anchorMin = portrait.rectTransform.anchorMax = new Vector2(.5f, 1f);
            portrait.rectTransform.pivot = new Vector2(.5f, 1f);
            portrait.rectTransform.sizeDelta = new Vector2(84f, 126f);
            portrait.rectTransform.anchoredPosition = new Vector2(0f, 4f);
            portrait.preserveAspect = true;
            if (UiBuild.ApplyUiSprite(portrait, "hero_portrait", Image.Type.Simple))
                portrait.color = Color.white;
            Image avatarFrame = UiBuild.Image(header.transform, "Avatar Frame", Color.clear);
            Rect(avatarFrame.rectTransform, 64f, 22f, 100f, 100f);
            avatarFrame.preserveAspect = true;
            bool hasAvatarFrame = UiBuild.ApplyUiSprite(avatarFrame, "avatar_frame", Image.Type.Simple);
            if (!hasAvatarFrame)
                hasAvatarFrame = UiBuild.ApplyUiSprite(avatarFrame, "hero_portrait_frame", Image.Type.Simple);
            if (hasAvatarFrame)
                avatarFrame.color = Color.white;

            // Name, level seal and EXP bar on the left.
            Text heroName = UiBuild.Text(header.transform, "HeroName", "Vân Du", 28,
                TextAnchor.MiddleLeft, UiFontRole.Title);
            Rect(heroName.rectTransform, 184f, 34f, 236f, 34f);
            UiBuild.FitLabel(heroName, UiBuild.LabelFitPreset.None);
            // Layout follows the banner's paper band (about y 30-115 of 132): name and combat power on the
            // top row, EXP bar below, and the level seal pinned to the avatar's lower-right corner.
            Text power = UiBuild.Text(header.transform, "Power", "Lực Chiến 100", 20,
                TextAnchor.MiddleLeft, UiFontRole.Title);
            Rect(power.rectTransform, 430f, 37f, 230f, 28f);
            power.color = UiBuild.Vermilion;
            UiBuild.AddLeadingIcon(power.transform, "icon_power", 24f);
            UiBuild.FitLabel(power, UiBuild.LabelFitPreset.None);
            Image levelSeal = UiBuild.Image(header.transform, "Level Badge", Color.clear);
            Rect(levelSeal.rectTransform, 130f, 82f, 40f, 40f);
            levelSeal.preserveAspect = true;
            if (UiBuild.ApplyUiSprite(levelSeal, "seal_title", Image.Type.Simple))
                levelSeal.color = Color.white;
            Text level = UiBuild.Text(levelSeal.transform, "LevelText", "1", 20,
                TextAnchor.MiddleCenter, UiFontRole.Title);
            level.color = UiBuild.Paper;
            level.fontStyle = FontStyle.Bold;
            // The seal art is busy, so the number needs an ink outline to stay readable.
            Outline levelOutline = level.gameObject.AddComponent<Outline>();
            levelOutline.effectColor = UiBuild.Ink;
            levelOutline.effectDistance = new Vector2(2f, -2f);
            UiBuild.Stretch(level.rectTransform, 2f);
            UiBuild.FitLabel(level, UiBuild.LabelFitPreset.None, 12);
            Slider exp = BuildHomeExpBar(header.transform);
            Rect(exp.GetComponent<RectTransform>(), 184f, 72f, 300f, 41f);
            Text expText = UiBuild.Text(exp.transform, "ExpText", "Cấp 1 · EXP 0/100", 15);
            UiBuild.AddLeadingIcon(exp.transform, "icon_exp", 20f);
            expText.color = UiBuild.Ink;
            UiBuild.Stretch(expText.rectTransform);
            UiBuild.FitLabel(expText, UiBuild.LabelFitPreset.None, 10);
            expText.raycastTarget = false;

            // Four currency slots holding only an icon and a number.
            RectTransform currencyRow = UiBuild.Rect(header.transform, "Currency Row");
            Rect(currencyRow, 770f, 44f, 1010f, 56f);
            HorizontalLayoutGroup currencyLayout = currencyRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            currencyLayout.spacing = 10f;
            currencyLayout.childAlignment = TextAnchor.MiddleLeft;
            currencyLayout.childControlWidth = false;
            currencyLayout.childControlHeight = false;
            Text stamina = CurrencyEntry(currencyRow, "StaminaText", "120/120", "icon_stamina");
            Text silver = CurrencyEntry(currencyRow, "SilverText", "8.800", "icon_bac");
            Text premium = CurrencyEntry(currencyRow, "PremiumText", "0", "icon_nguyen_bao");
            Text reputation = CurrencyEntry(currencyRow, "ReputationText", "0", "icon_danh_vong");
            HoverTooltipTarget staminaTarget = stamina.transform.parent.gameObject.AddComponent<HoverTooltipTarget>();
            ItemTooltip staminaTooltip = BuildHomeTooltip(canvas.transform);
            Button settings = BuildSettingsButton(header.transform);
            RectTransform hotspots = UiBuild.Rect(canvas.transform, "Hotspots"); UiBuild.Stretch(hotspots);
            HomeHotspot pve = Hotspot(hotspots, "Hotspot_Pve", "Hành Tẩu Giang Hồ", "Vào ải hành tẩu", 1380, 380, 260, 200, new Vector2(1f, 0.5f), false);
            HomeHotspot pvp = Hotspot(hotspots, "Hotspot_Pvp", "Luận Kiếm Đài", "Tỷ thí cùng quần hùng", 1600, 140, 240, 180, Vector2.one, false);
            HomeHotspot bag = Hotspot(hotspots, "Hotspot_Bag", "Bao Phục", "Sắp xếp hành trang", 430, 790, 240, 140, new Vector2(0.5f, 0f), false);
            HomeHotspot storage = Hotspot(hotspots, "Hotspot_Storage", "Tàng Bảo Các", "Kho báu và vật phẩm", 1640, 560, 240, 220, new Vector2(1f, 0f), false);
            HomeHotspot codex = Hotspot(hotspots, "Hotspot_Codex", "Vạn Bảo Phổ", "Xem đồ giám giang hồ", 80, 680, 220, 160, Vector2.zero, false);
            HomeHotspot shop = Hotspot(hotspots, "Hotspot_Shop", "Thương Hội", "Giao dịch vật phẩm", 380, 400, 300, 200, new Vector2(0f, 0.5f), false);
            HomeHotspot sect = Hotspot(hotspots, "Hotspot_Sect", "Môn Phái", "Đạt cấp 10 mở khóa gia nhập Môn Phái", 250, 90, 240, 180, new Vector2(0f, 1f), true);
            HomeHotspot encyclopedia = Hotspot(hotspots, "Hotspot_Encyclopedia", "Bách Khoa",
                "Tra cứu thuật ngữ giang hồ", 80, 870, 220, 130, Vector2.zero, false);
            UiBuild.AddLeadingIcon(encyclopedia.transform, "icon_info", 30f);
            AddTarget(pve.gameObject, "home_pve");
            AddTarget(bag.gameObject, "home_bag");
            AddTarget(storage.gameObject, "home_storage");
            AddTarget(shop.gameObject, "home_shop");
            AddTarget(sect.gameObject, "home_sect");

            UiBuild.Set(controller, "background", background); UiBuild.Set(controller, "heroZone", hero); UiBuild.Set(controller, "heroName", heroName);
            UiBuild.Set(controller, "expBar", exp); UiBuild.Set(controller, "expText", expText); UiBuild.Set(controller, "levelText", level); UiBuild.Set(controller, "settingsButton", settings);
            UiBuild.Set(controller, "powerText", power);
            UiBuild.Set(controller, "staminaText", stamina); UiBuild.Set(controller, "silverText", silver); UiBuild.Set(controller, "premiumText", premium); UiBuild.Set(controller, "reputationText", reputation);
            UiBuild.Set(controller, "staminaTooltip", staminaTooltip);
            UiBuild.Set(controller, "staminaTooltipTarget", staminaTarget);
            UiBuild.Set(controller, "pveHotspot", pve); UiBuild.Set(controller, "pvpHotspot", pvp); UiBuild.Set(controller, "bagHotspot", bag);
            UiBuild.Set(controller, "storageHotspot", storage); UiBuild.Set(controller, "codexHotspot", codex); UiBuild.Set(controller, "shopHotspot", shop); UiBuild.Set(controller, "sectHotspot", sect);
            UiBuild.Set(controller, "encyclopediaHotspot", encyclopedia);
            EditorSceneManager.SaveScene(scene, HomePath); EnsureBuildSettings();
        }

        private static ItemTooltip BuildHomeTooltip(Transform parent)
        {
            // The stamina slot uses the same framed, cursor-following tooltip as the shop.
            var root = new GameObject("Stamina Tooltip", typeof(RectTransform), typeof(Canvas), typeof(ItemTooltip));
            root.transform.SetParent(parent, false);
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            Image paper = UiBuild.Image(root.transform, "Paper", UiBuild.Paper);
            UiBuild.ApplyUiSprite(paper, "frame_tooltip", Image.Type.Sliced);
            RectTransform panel = paper.rectTransform;
            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.zero;
            panel.pivot = new Vector2(0f, 1f);
            panel.sizeDelta = new Vector2(440f, 260f);
            RectTransform viewport = UiBuild.Image(paper.transform, "Viewport", Color.clear).rectTransform;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(ItemTooltip.HorizontalInset, ItemTooltip.BottomInset);
            viewport.offsetMax = new Vector2(-ItemTooltip.HorizontalInset, -ItemTooltip.TopInset);
            viewport.gameObject.AddComponent<RectMask2D>();
            Text description = UiBuild.Text(viewport, "Description", string.Empty, 18, TextAnchor.UpperLeft);
            UiBuild.Stretch(description.rectTransform);
            description.color = UiBuild.Ink;
            description.horizontalOverflow = HorizontalWrapMode.Wrap;
            description.verticalOverflow = VerticalWrapMode.Overflow;
            ItemTooltip tooltip = root.GetComponent<ItemTooltip>();
            UiBuild.Set(tooltip, "overlayCanvas", canvas);
            UiBuild.Set(tooltip, "panel", panel);
            UiBuild.Set(tooltip, "descriptionText", description);
            return tooltip;
        }

        private static HomeHotspot Hotspot(Transform parent, string name, string label, string tooltip, float x, float y, float width, float height, Vector2 anchor, bool locked)
        {
            HomeHotspot hotspot = UiBuild.InstantiateLinked<HomeHotspot>(HomeHotspotPrefabBuilder.Path, parent); hotspot.gameObject.name = name;
            PixelRect((RectTransform)hotspot.transform, x, y, width, height, anchor); hotspot.Configure(label, tooltip, locked, locked ? 10 : 0, false); return hotspot;
        }

        private static void AddTarget(GameObject owner, string id)
        {
            TutorialTarget target = owner.AddComponent<TutorialTarget>();
            target.Configure(id);
        }

        private static Slider BuildHomeExpBar(Transform parent)
        {
            // Same layering as the loading bar: fill first, bamboo frame drawn on top of it.
            RectTransform bar = UiBuild.Rect(parent, "ExpBar");
            RectTransform fillArea = UiBuild.Rect(bar, "Fill Area");
            fillArea.anchorMin = new Vector2(.105f, .28f);
            fillArea.anchorMax = new Vector2(.895f, .72f);
            fillArea.offsetMin = Vector2.zero;
            fillArea.offsetMax = Vector2.zero;
            Image fill = UiBuild.Image(fillArea, "Fill", UiBuild.Vermilion);
            fill.type = Image.Type.Simple;
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = new Vector2(0f, 1f);
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            Color frameFallback = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .18f);
            Image frame = UiBuild.Image(bar, "Frame", frameFallback);
            if (UiBuild.ApplyUiSprite(frame, "progress_bar", Image.Type.Simple))
                frame.color = Color.white;
            frame.raycastTarget = false;
            UiBuild.Stretch(frame.rectTransform);
            Slider slider = bar.gameObject.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;
            return slider;
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

        private static Text CurrencyEntry(Transform parent, string name, string value, string iconKey)
        {
            // The slot art has a round icon socket on the left, so it keeps its aspect instead of 9-slicing.
            RectTransform entry = UiBuild.Rect(parent, name + " Entry");
            entry.sizeDelta = new Vector2(242f, 56f);
            Image slot = entry.gameObject.AddComponent<Image>();
            slot.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .92f);
            bool hasSlotArt = UiBuild.ApplyUiSprite(slot, "currency_slot", Image.Type.Simple);
            if (hasSlotArt)
            {
                slot.color = Color.white;
                slot.preserveAspect = true;
            }
            else
                UiBuild.ApplyUiSprite(slot, "plaque_label", Image.Type.Sliced);
            Image icon = UiBuild.Image(entry, "Icon", Color.white);
            if (hasSlotArt)
                Rect(icon.rectTransform, 24f, 11f, 34f, 34f); // socket centre measured at x 16.8%, y 49% of the art
            else
                Rect(icon.rectTransform, 12f, 8f, 40f, 40f);
            icon.preserveAspect = true;
            // preserveAspect aligns the sprite by the pivot; centre it so wide icons sit in the middle of the socket.
            RectTransform iconRect = icon.rectTransform;
            iconRect.pivot = new Vector2(.5f, .5f);
            iconRect.anchoredPosition += new Vector2(iconRect.sizeDelta.x / 2f, -iconRect.sizeDelta.y / 2f);
            icon.raycastTarget = false;
            icon.sprite = CurrencyArt.TryGetSprite(iconKey, out Sprite sprite) ? sprite : null;
            icon.gameObject.SetActive(icon.sprite != null);
            Text text = UiBuild.Text(entry, name, value, 22, TextAnchor.MiddleRight, UiFontRole.Body);
            text.color = UiBuild.Paper;
            text.rectTransform.anchorMin = new Vector2(.3f, .1f);
            text.rectTransform.anchorMax = new Vector2(.86f, .9f);
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            UiBuild.FitLabel(text, UiBuild.LabelFitPreset.None);
            return text;
        }

        private static Button BuildSettingsButton(Transform parent)
        {
            Image icon = UiBuild.Image(parent, "Btn_Settings", Color.clear);
            Rect(icon.rectTransform, 1776f, 34f, 76f, 76f);
            icon.preserveAspect = true;
            if (UiBuild.ApplyUiSprite(icon, "icon_settings", Image.Type.Simple))
            {
                icon.color = Color.white;
                Button iconButton = icon.gameObject.AddComponent<Button>();
                iconButton.targetGraphic = icon;
                return iconButton;
            }

            UnityEngine.Object.DestroyImmediate(icon.gameObject);
            Button fallback = LinkedButton("Button_Secondary", parent, "Btn_Settings", "Cài đặt");
            Rect(fallback.GetComponent<RectTransform>(), 1710f, 26f, 180f, 60f);
            fallback.GetComponent<Image>().pixelsPerUnitMultiplier = 10f;
            UiBuild.FitLabel(fallback.GetComponentInChildren<Text>(), UiBuild.LabelFitPreset.OrnateButton);
            return fallback;
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
