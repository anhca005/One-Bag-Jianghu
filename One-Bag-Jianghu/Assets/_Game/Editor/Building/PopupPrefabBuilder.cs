using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Onboarding;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class PopupPrefabBuilder
    {
        private const string Folder = "Assets/_Game/Resources/UI/Popups";

        [MenuItem("One Bag Jianghu/Build Popup Prefabs")]
        public static void BuildPopupPrefabs() => Build(false);

        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);

        private const string PveArtFolder = "Assets/_Game/Textures2D/Pve/";

        [MenuItem("One Bag Jianghu/Build/Intro, Tutorial, Settings And Bag Popups (Overwrite)")]
        public static void BuildOnboardingOverwrite()
        {
            UiBuild.EnsureFolder(Folder);
            BuildSettings(true);
            BuildBag(true);
            BuildIntro(true);
            BuildTutorial(true);
            AssetDatabase.SaveAssets();
        }

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            BagPrefabBuilder.Build(false);
            UiBuild.EnsureFolder(Folder);
            BuildMessage(overwrite);
            BuildConfirm(overwrite);
            BuildSettings(overwrite);
            BuildBag(overwrite);
            BuildResult(overwrite);
            BuildIntro(overwrite);
            BuildTutorial(overwrite);
            BuildEncyclopedia(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildMessage(bool overwrite)
        {
            Build<MessagePopup>(new Vector2(720f, 420f), overwrite, (popup, panel) =>
            {
                Text title = AddText(panel, "Title", "Thông báo", 40, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.92f));
                Text message = AddText(panel, "Message", "Nội dung thông báo", 25, new Vector2(0.08f, 0.3f), new Vector2(0.92f, 0.7f));
                UiButton close = AddButton(panel, "Close Button", "Đóng", true, new Vector2(0.5f, 0.14f));
                UiBuild.Set(popup, "titleText", title); UiBuild.Set(popup, "messageText", message); UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildConfirm(bool overwrite)
        {
            Build<ConfirmPopup>(new Vector2(760f, 440f), overwrite, (popup, panel) =>
            {
                Text title = AddText(panel, "Title", "Xác nhận", 40, new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.92f));
                Text message = AddText(panel, "Message", "Bạn có chắc chắn?", 25, new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.7f));
                UiButton cancel = AddButton(panel, "Cancel Button", "Hủy", false, new Vector2(0.28f, 0.14f));
                UiButton confirm = AddButton(panel, "Confirm Button", "Đồng ý", true, new Vector2(0.72f, 0.14f));
                UiBuild.Set(popup, "titleText", title); UiBuild.Set(popup, "messageText", message);
                UiBuild.Set(popup, "cancelButton", cancel); UiBuild.Set(popup, "confirmButton", confirm);
            });
        }

        private static void BuildSettings(bool overwrite)
        {
            Build<SettingsPopup>(new Vector2(960f, 900f), overwrite, (popup, panel) =>
            {
                // A single grid keeps labels and controls aligned across all three sections.
                AddText(panel, "Title", "Cài đặt", 42, new Vector2(.1f, .9467f), new Vector2(.9f, 1f));
                AddSectionHeader(panel, "Audio Header", "Âm thanh", 60f);
                Slider master = AddSettingsSlider(panel, "Master Volume", "Âm lượng chung", 108f,
                    out Text masterText);
                Slider music = AddSettingsSlider(panel, "Music Volume", "Nhạc nền", 162f, out Text musicText);
                Slider sfx = AddSettingsSlider(panel, "Sfx Volume", "Hiệu ứng", 216f, out Text sfxText);
                Toggle muted = AddSettingsToggle(panel, "Muted Toggle", "Tắt tiếng", 270f);
                AddSectionHeader(panel, "Display Header", "Hiển thị", 336f);
                Toggle fullscreen = AddSettingsToggle(panel, "Fullscreen Toggle", "Toàn màn hình", 384f);
                Dropdown resolution = AddSettingsDropdown(panel, "Resolution Dropdown", "Độ phân giải", 438f);
                Toggle vSync = AddSettingsToggle(panel, "VSync Toggle", "Đồng bộ dọc (VSync)", 492f);
                Dropdown frameRate = AddSettingsDropdown(panel, "Frame Rate Dropdown", "Giới hạn FPS", 546f);
                AddSectionHeader(panel, "Account Header", "Tài khoản", 612f);
                AddSettingsLabel(panel, "Hero Name Label", "Hiệp khách", 660f);
                Text heroName = AddText(panel, "Hero Name", "Vân Du", 24,
                    SettingsRowMin(660f, .44f), SettingsRowMax(660f, .86f), TextAnchor.MiddleLeft);
                UiBuild.FitLabel(heroName, UiBuild.LabelFitPreset.None);
                // The four buttons form a 2x2 grid: same columns (34% / 66%) and width, so rows line up.
                UiButton login = AddSettingsButton(panel, "Login Button", "Đăng xuất", false,
                    .34f, 770f, 220f, 54f);
                UiButton quit = AddSettingsButton(panel, "Quit Button", "Thoát game", false,
                    .66f, 770f, 220f, 54f);
                UiButton defaults = AddSettingsButton(panel, "Defaults Button", "Mặc định", false,
                    .34f, 836f, 220f, 54f);
                UiButton close = AddSettingsButton(panel, "Close Button", "Đóng", true,
                    .66f, 836f, 220f, 54f);
                UiBuild.Set(popup, "masterVolumeSlider", master);
                UiBuild.Set(popup, "musicVolumeSlider", music);
                UiBuild.Set(popup, "sfxVolumeSlider", sfx);
                UiBuild.Set(popup, "masterVolumeText", masterText);
                UiBuild.Set(popup, "musicVolumeText", musicText);
                UiBuild.Set(popup, "sfxVolumeText", sfxText);
                UiBuild.Set(popup, "mutedToggle", muted);
                UiBuild.Set(popup, "fullscreenToggle", fullscreen);
                UiBuild.Set(popup, "resolutionDropdown", resolution);
                UiBuild.Set(popup, "vSyncToggle", vSync);
                UiBuild.Set(popup, "frameRateDropdown", frameRate);
                UiBuild.Set(popup, "heroNameText", heroName);
                UiBuild.Set(popup, "loginButton", login);
                UiBuild.Set(popup, "quitButton", quit);
                UiBuild.Set(popup, "defaultsButton", defaults);
                UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildIntro(bool overwrite)
        {
            Build<IntroPopup>(new Vector2(1536f, 1024f), overwrite, (popup, panel) =>
            {
                MakeFullScreen(panel, Color.black);
                panel.gameObject.AddComponent<RectMask2D>();

                // The art covers the whole screen at its own 3:2 aspect; the edges are cropped, never letterboxed.
                Image artwork = UiBuild.Image(panel, "Artwork", UiBuild.Paper);
                AspectRatioFitter fitter = artwork.gameObject.AddComponent<AspectRatioFitter>();
                fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                fitter.aspectRatio = 1.5f;
                // IntroPopup wires the click in Awake; listeners added here would not be saved in the prefab.
                Button advance = artwork.gameObject.AddComponent<Button>();
                advance.transition = Selectable.Transition.None;

                // Story text sits on the same scroll frame as the stage dialogue.
                Image scroll = ScrollFrame(panel, "Story Scroll", new Vector2(1140f, 380f));
                RectTransform scrollRect = scroll.rectTransform;
                scrollRect.anchorMin = scrollRect.anchorMax = new Vector2(.5f, .015f);
                scrollRect.pivot = new Vector2(.5f, 0f);
                scrollRect.anchoredPosition = Vector2.zero;
                Text body = AddText(scroll.transform, "Story Text", string.Empty, 30,
                    new Vector2(.13f, .27f), new Vector2(.87f, .73f));
                body.color = UiBuild.Ink;
                body.raycastTarget = false;
                Text hint = AddText(scroll.transform, "Advance Hint", "Bấm để tiếp tục ▸", 18,
                    new Vector2(.6f, .2f), new Vector2(.87f, .29f), TextAnchor.MiddleRight);
                hint.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .6f);
                hint.raycastTarget = false;
                UiButton skip = AddButton(panel, "Skip Button", "Bỏ qua", false, new Vector2(.9f, .93f));
                UiBuild.Set(popup, "artwork", artwork);
                UiBuild.Set(popup, "bodyText", body);
                UiBuild.Set(popup, "skipButton", skip);
            });
        }

        private static void BuildTutorial(bool overwrite)
        {
            Build<TutorialOverlay>(new Vector2(1536f, 1024f), overwrite, (popup, panel) =>
            {
                UiBuild.SetBool(popup, "dimBackground", false);
                UiBuild.SetBool(popup, "closeOnBackdrop", false);
                UiBuild.SetBool(popup, "closeOnEscape", false);
                MakeFullScreen(panel, Color.clear);

                // Four shades frame a hole over the target; the hole itself has nothing that catches clicks.
                Color shade = new Color(0f, 0f, 0f, .6f);
                Image top = UiBuild.Image(panel, "Top Shade", shade);
                Image bottom = UiBuild.Image(panel, "Bottom Shade", shade);
                Image left = UiBuild.Image(panel, "Left Shade", shade);
                Image right = UiBuild.Image(panel, "Right Shade", shade);
                // Hollow gold ring: four thin edges, the middle stays clear so the target shows through.
                Image ring = UiBuild.Image(panel, "Highlight Ring", Color.clear);
                ring.raycastTarget = false;
                Color gold = new Color(1f, .82f, .35f, .95f);
                AddRingEdge(ring.rectTransform, "Top", gold, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -4f));
                AddRingEdge(ring.rectTransform, "Bottom", gold, Vector2.zero, new Vector2(1f, 0f), new Vector2(0f, 4f));
                AddRingEdge(ring.rectTransform, "Left", gold, Vector2.zero, new Vector2(0f, 1f), new Vector2(4f, 0f));
                AddRingEdge(ring.rectTransform, "Right", gold, new Vector2(1f, 0f), Vector2.one, new Vector2(-4f, 0f));

                // Guide box: a scroll moved to the top or bottom edge at runtime, away from the hole.
                Image guideScroll = ScrollFrame(panel, "Guide Box", new Vector2(1100f, 366f));
                RectTransform guide = guideScroll.rectTransform;
                Image portrait = UiBuild.Image(guide, "Guide Portrait", new Color(.2f, .2f, .2f, .3f));
                UiBuild.Anchors(portrait.rectTransform, new Vector2(.11f, .2f), new Vector2(.23f, .8f));
                // Lục Tiều Phu's portrait from the PvE NPC art (same key as in chapters.json).
                Sprite guideArt = AssetDatabase.LoadAssetAtPath<Sprite>(PveArtFolder + "npc_lao_tieu_phu.png");
                if (guideArt != null)
                {
                    portrait.sprite = guideArt;
                    portrait.color = Color.white;
                }

                portrait.preserveAspect = true;
                portrait.raycastTarget = false;
                Text line = AddText(guide, "Guide Line", "Lục Tiều Phu", 26,
                    new Vector2(.25f, .24f), new Vector2(.89f, .76f), TextAnchor.MiddleLeft);
                line.color = UiBuild.Ink;
                line.raycastTarget = false;

                // The skip button rides on the guide box, so it never covers the highlighted target.
                UiButton skip = AddButton(guide, "Skip Tutorial Button", "Bỏ qua hướng dẫn", false,
                    new Vector2(1f, .5f));
                RectTransform skipRect = skip.GetComponent<RectTransform>();
                skipRect.pivot = new Vector2(0f, .5f);
                skipRect.anchoredPosition = new Vector2(24f, 0f);
                UiBuild.Set(popup, "topShade", top);
                UiBuild.Set(popup, "bottomShade", bottom);
                UiBuild.Set(popup, "leftShade", left);
                UiBuild.Set(popup, "rightShade", right);
                UiBuild.Set(popup, "highlight", ring);
                UiBuild.Set(popup, "guideBox", guide);
                UiBuild.Set(popup, "guideLine", line);
                UiBuild.Set(popup, "portrait", portrait);
                UiBuild.Set(popup, "skipButton", skip);
            });
        }

        /// <summary>One edge of the highlight ring; <paramref name="inset"/> gives its thickness inward.</summary>
        private static void AddRingEdge(RectTransform ring, string name, Color color, Vector2 min, Vector2 max,
            Vector2 inset)
        {
            Image edge = UiBuild.Image(ring, name, color);
            edge.raycastTarget = false;
            RectTransform rect = edge.rectTransform;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = new Vector2(Mathf.Min(0f, inset.x), Mathf.Min(0f, inset.y));
            rect.offsetMax = new Vector2(Mathf.Max(0f, inset.x), Mathf.Max(0f, inset.y));
        }

        /// <summary>
        /// The PvE dialogue scroll (frame_dialogue.png, 3:1, paper band 19-81% of its height). Falls back to a plain
        /// paper box when the art is missing. Never catches clicks.
        /// </summary>
        private static Image ScrollFrame(Transform parent, string name, Vector2 size)
        {
            Image image = UiBuild.Image(parent, name, UiBuild.Paper);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PveArtFolder + "frame_dialogue.png");
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
                image.color = Color.white;
            }

            image.raycastTarget = false;
            image.rectTransform.sizeDelta = size;
            return image;
        }

        /// <summary>Turns the shared paper panel into a plain full-screen layer (no frame, no fixed size).</summary>
        private static void MakeFullScreen(RectTransform panel, Color color)
        {
            PrefabUtility.UnpackPrefabInstance(
                panel.gameObject,
                PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction);
            for (int i = panel.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(panel.GetChild(i).gameObject);
            Image image = panel.GetComponent<Image>();
            image.sprite = null;
            image.color = color;
            image.raycastTarget = color.a > 0f;
            UiBuild.Stretch(panel);
        }

        private static void BuildEncyclopedia(bool overwrite)
        {
            Build<EncyclopediaPopup>(new Vector2(1180f, 820f), overwrite, (popup, panel) =>
            {
                AddText(panel, "Title", "Giang Hồ Bách Khoa", 40,
                    new Vector2(.08f, .9f), new Vector2(.92f, .98f));
                RectTransform categories = AddScrollColumn(panel, "Categories", .05f, .31f);
                RectTransform entries = AddScrollColumn(panel, "Entries", .34f, .61f);
                Text selectedName = AddText(panel, "Selected Name", string.Empty, 32,
                    new Vector2(.64f, .73f), new Vector2(.95f, .85f));
                Text description = AddText(panel, "Description", string.Empty, 25,
                    new Vector2(.64f, .2f), new Vector2(.95f, .72f), TextAnchor.UpperLeft);
                UiButton close = AddButton(panel, "Close Button", "Đóng", true, new Vector2(.8f, .1f));
                UiBuild.Set(popup, "categoryContent", categories);
                UiBuild.Set(popup, "entryContent", entries);
                UiBuild.Set(popup, "selectedName", selectedName);
                UiBuild.Set(popup, "selectedDescription", description);
                UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static RectTransform AddScrollColumn(Transform parent, string name, float minX, float maxX)
        {
            Image viewport = UiBuild.Image(parent, name + " Viewport", new Color(1f, 1f, 1f, .08f));
            UiBuild.Anchors(viewport.rectTransform, new Vector2(minX, .12f), new Vector2(maxX, .87f));
            viewport.gameObject.AddComponent<RectMask2D>();
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
            RectTransform content = UiBuild.Rect(viewport.transform, name + " Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(.5f, 1f);
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = content;
            scroll.viewport = viewport.rectTransform;
            scroll.horizontal = false;
            return content;
        }

        private static void AddSectionHeader(Transform panel, string name, string label, float top)
        {
            Text header = AddText(panel, name, label, 24,
                SettingsRectMin(top, 48f, .08f), SettingsRectMax(top, 48f, .4f), TextAnchor.MiddleLeft);
            UiFonts.Apply(header, UiFontRole.Title);
            Color dividerColor = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .35f);
            Image divider = UiBuild.Image(panel, name + " Divider", dividerColor);
            float dividerY = 1f - (top + 24f) / 900f;
            UiBuild.Anchors(divider.rectTransform, new Vector2(.44f, dividerY - .0015f),
                new Vector2(.94f, dividerY + .0015f));
        }

        private static Slider AddSettingsSlider(Transform panel,
            string name,
            string label,
            float top,
            out Text valueText)
        {
            AddSettingsLabel(panel, name + " Label", label, top);
            Slider slider = UiBuild.InstantiateLinked<Slider>(CommonUiPrefabBuilder.Folder + "/Slider.prefab", panel);
            slider.name = name + " Slider";
            slider.gameObject.AddComponent<CanvasGroup>();
            UiBuild.Anchors(slider.GetComponent<RectTransform>(),
                SettingsRectMin(top + 16f, 22f, .44f), SettingsRectMax(top + 16f, 22f, .86f));
            valueText = AddText(panel, name + " Value", "100%", 20, SettingsRowMin(top, .87f),
                SettingsRowMax(top, .94f));
            return slider;
        }

        private static Toggle AddSettingsToggle(Transform panel, string name, string label, float top)
        {
            AddSettingsLabel(panel, name + " Label", label, top);
            Toggle toggle = UiBuild.InstantiateLinked<Toggle>(CommonUiPrefabBuilder.Folder + "/Toggle.prefab", panel);
            toggle.name = name;
            RectTransform toggleRect = toggle.GetComponent<RectTransform>();
            toggleRect.anchorMin = toggleRect.anchorMax = new Vector2(.44f, 1f - (top + 27f) / 900f);
            toggleRect.pivot = new Vector2(0f, .5f);
            toggleRect.sizeDelta = new Vector2(40f, 40f);
            return toggle;
        }

        private static Dropdown AddSettingsDropdown(Transform panel, string name, string label, float top)
        {
            AddSettingsLabel(panel, name + " Label", label, top);
            Dropdown dropdown = UiBuild.InstantiateLinked<Dropdown>(
                CommonUiPrefabBuilder.Folder + "/Dropdown.prefab", panel);
            dropdown.name = name;
            UiBuild.Anchors(dropdown.GetComponent<RectTransform>(),
                SettingsRowMin(top, .44f), SettingsRowMax(top, .86f));
            return dropdown;
        }

        private static void AddSettingsLabel(Transform panel, string name, string label, float top)
        {
            Text rowLabel = AddText(panel, name, label, 24, SettingsRowMin(top, .08f),
                SettingsRowMax(top, .4f), TextAnchor.MiddleLeft);
            UiBuild.FitLabel(rowLabel, UiBuild.LabelFitPreset.None);
        }

        private static UiButton AddSettingsButton(Transform panel, string name, string label, bool primary,
            float centerX, float centerY, float width, float height)
        {
            UiButton button = AddButton(panel, name, label, primary, new Vector2(centerX, 1f - centerY / 900f));
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            UiBuild.FitLabel(button.GetComponentInChildren<Text>(), UiBuild.LabelFitPreset.OrnateButton);
            return button;
        }

        private static Vector2 SettingsRowMin(float top, float x) => SettingsRectMin(top, 54f, x);
        private static Vector2 SettingsRowMax(float top, float x) => SettingsRectMax(top, 54f, x);
        private static Vector2 SettingsRectMin(float top, float height, float x)
            => new Vector2(x, 1f - (top + height) / 900f);
        private static Vector2 SettingsRectMax(float top, float height, float x)
            => new Vector2(x, 1f - top / 900f);

        private static UiButton AddSmallButton(Transform panel,
            string name,
            string label,
            bool primary,
            float x,
            float y)
        {
            UiButton button = AddButton(panel, name, label, primary, new Vector2(x, y));
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 56f);
            UiBuild.FitLabel(button.GetComponentInChildren<Text>(), UiBuild.LabelFitPreset.OrnateButton);
            return button;
        }

        private static void BuildBag(bool overwrite)
        {
            Build<BagPopup>(new Vector2(1700f, 960f), overwrite, (popup, panel) =>
            {
                UiBuild.SetBool(popup, "closeOnBackdrop", false);
                AddText(panel, "Title", "Bao Phục", 40, new Vector2(0.35f, 0.9f), new Vector2(0.65f, 0.98f));
                RectTransform host = UiBuild.Rect(panel, "BagHost");
                UiBuild.Anchors(host, new Vector2(0.03f, 0.1f), new Vector2(0.97f, 0.89f));
                BagLoadoutPanel bagPanel = UiBuild.InstantiateLinked<BagLoadoutPanel>(BagPrefabBuilder.Folder + "/BagLoadoutPanel.prefab", host);
                UiBuild.Stretch((RectTransform)bagPanel.transform);
                UiBuild.SetBool(bagPanel, "showSectSelector", true);
                TutorialTarget gridTarget = bagPanel.gameObject.AddComponent<TutorialTarget>();
                gridTarget.Configure("bag_grid");
                GameObject tooltipRoot = new GameObject("Item Tooltip", typeof(RectTransform), typeof(Canvas), typeof(ItemTooltip));
                tooltipRoot.transform.SetParent(popup.transform, false); UiBuild.Stretch(tooltipRoot.GetComponent<RectTransform>());
                Canvas tooltipCanvas = tooltipRoot.GetComponent<Canvas>(); tooltipCanvas.overrideSorting = true;
                Image tooltipPanel = UiBuild.Image(tooltipRoot.transform, "Paper", UiBuild.Paper);
                UiBuild.ApplyUiSprite(tooltipPanel, "frame_tooltip", Image.Type.Sliced);
                RectTransform tooltipRect = tooltipPanel.rectTransform; tooltipRect.anchorMin = tooltipRect.anchorMax = Vector2.zero;
                tooltipRect.pivot = new Vector2(0f, 1f); tooltipRect.sizeDelta = new Vector2(440f, 420f);
                RectTransform tooltipViewport = UiBuild.Image(tooltipPanel.transform, "Viewport", Color.clear).rectTransform; tooltipViewport.anchorMin=Vector2.zero;tooltipViewport.anchorMax=Vector2.one;tooltipViewport.offsetMin=new Vector2(ItemTooltip.HorizontalInset,ItemTooltip.BottomInset);tooltipViewport.offsetMax=new Vector2(-ItemTooltip.HorizontalInset,-ItemTooltip.TopInset);tooltipViewport.gameObject.AddComponent<RectMask2D>();
                RectTransform tooltipContent = UiBuild.Rect(tooltipViewport, "Content"); tooltipContent.anchorMin = new Vector2(0f, 1f); tooltipContent.anchorMax = new Vector2(1f, 1f); tooltipContent.pivot = new Vector2(.5f, 1f); tooltipContent.sizeDelta = Vector2.zero;
                VerticalLayoutGroup tooltipLayout = tooltipContent.gameObject.AddComponent<VerticalLayoutGroup>(); tooltipLayout.childControlHeight = true; tooltipLayout.childControlWidth = true; tooltipLayout.childForceExpandHeight = false; tooltipLayout.childForceExpandWidth = true;
                ContentSizeFitter tooltipFitter = tooltipContent.gameObject.AddComponent<ContentSizeFitter>(); tooltipFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                ScrollRect tooltipScroll = tooltipPanel.gameObject.AddComponent<ScrollRect>(); tooltipScroll.viewport = tooltipViewport; tooltipScroll.content = tooltipContent; tooltipScroll.horizontal = false; tooltipScroll.vertical = true; tooltipScroll.movementType = ScrollRect.MovementType.Clamped;
                Text tooltipText = UiBuild.Text(tooltipContent, "Description", string.Empty, 18, TextAnchor.UpperLeft);
                tooltipText.color = UiBuild.Ink; tooltipText.supportRichText = true; tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap; tooltipText.verticalOverflow = VerticalWrapMode.Overflow;
                tooltipText.resizeTextForBestFit = false; tooltipText.resizeTextMinSize = 16; tooltipText.resizeTextMaxSize = 18;
                tooltipText.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                ItemTooltip tooltip = tooltipRoot.GetComponent<ItemTooltip>(); UiBuild.Set(tooltip, "overlayCanvas", tooltipCanvas); UiBuild.Set(tooltip, "panel", tooltipRect); UiBuild.Set(tooltip, "descriptionText", tooltipText);
                // No save button: closing saves the layout (BagPopup.SaveAndClose).
                UiButton close = AddButton(panel, "Close Button", "Đóng", true, new Vector2(0.5f, 0.045f));
                TutorialTarget closeTarget = close.gameObject.AddComponent<TutorialTarget>();
                closeTarget.Configure("bag_close");
                UiBuild.Set(popup, "bagHost", host); UiBuild.Set(popup, "bagPanel", bagPanel); UiBuild.Set(popup, "itemTooltip", tooltip); UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildResult(bool overwrite)
        {
            Build<ResultPopup>(new Vector2(760f, 500f), overwrite, (popup, panel) =>
            {
                UiBuild.SetBool(popup, "closeOnEscape", false); UiBuild.SetBool(popup, "closeOnBackdrop", false);
                Image seal = UiBuild.Image(panel, "Title Seal", Color.clear);
                UiBuild.Anchors(seal.rectTransform, new Vector2(0.39f, 0.61f), new Vector2(0.61f, 0.95f));
                seal.preserveAspect = true; seal.raycastTarget = false; UiBuild.ApplyUiSprite(seal, "seal_title");
                Text result = AddText(panel, "Result Text", "Thắng", 58, new Vector2(0.05f, 0.64f), new Vector2(0.95f, 0.91f));
                Text winner = AddText(panel, "Winner", "Người thắng", 26, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.65f));
                Text rewards = AddText(panel, "Rewards", "", 22, new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.54f));
                Text reason = AddText(panel, "Disabled Reason", "", 18, new Vector2(0.08f, 0.34f), new Vector2(0.92f, 0.42f)); reason.color = UiBuild.Vermilion;
                UiButton home = AddButton(panel, "Home Button", "Về trấn", false, new Vector2(0.5f, 0.3f));
                UiButton again = AddButton(panel, "Play Again Button", "Đấu tiếp", true, new Vector2(0.5f, 0.13f));
                UiBuild.Set(popup, "resultText", result); UiBuild.Set(popup, "winnerText", winner); UiBuild.Set(popup, "rewardText", rewards);
                UiBuild.Set(popup, "homeButton", home); UiBuild.Set(popup, "playAgainButton", again); UiBuild.Set(popup, "disabledReason", reason);
            });
        }

        private static void Build<T>(Vector2 size, bool overwrite, System.Action<T, RectTransform> content) where T : BasePopup
        {
            string path = Folder + "/" + typeof(T).Name + ".prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var root = new GameObject(typeof(T).Name, typeof(RectTransform), typeof(CanvasGroup));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            root.AddComponent<GraphicRaycaster>();
            T popup = root.AddComponent<T>();
            Image panelImage = UiBuild.InstantiateLinked<Image>(CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab", root.transform);
            panelImage.name = "Panel";
            RectTransform panel = panelImage.rectTransform;
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = size;
            UiBuild.Set(popup, "panel", panel);
            content(popup, panel);
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static Text AddText(Transform parent, string name, string value, int size, Vector2 min, Vector2 max, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            UiFontRole role = name.IndexOf("Title", System.StringComparison.OrdinalIgnoreCase) >= 0 || name == "Result Text"
                ? UiFontRole.Display : UiFontRole.Body;
            Text text = UiBuild.Text(parent, name, value, size, alignment, role);
            UiBuild.Anchors(text.rectTransform, min, max);
            return text;
        }

        private static UiButton AddButton(Transform parent, string name, string label, bool primary, Vector2 anchor)
        {
            string prefab = primary ? "Button_Primary.prefab" : "Button_Secondary.prefab";
            UiButton button = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/" + prefab, parent);
            button.name = name;
            button.SetLabel(label);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(280f, 64f);
            return button;
        }
    }
}
