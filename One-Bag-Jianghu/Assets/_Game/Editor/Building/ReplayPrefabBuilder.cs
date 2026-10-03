using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class ReplayPrefabBuilder
    {
        public const string Folder = "Assets/_Game/Prefabs/UI/Replay";
        [MenuItem("One Bag Jianghu/Build/Replay Prefabs")] public static void BuildReplayPrefabs() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Replay Prefabs (Overwrite)")] public static void BuildReplayPrefabsOverwrite() => Build(true);
        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(overwrite); UiBuild.EnsureFolder(Folder);
            BuildItem(overwrite); BuildFighter(overwrite); BuildEffect(overwrite); BuildLogRow(overwrite); BuildScreen(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildItem(bool overwrite)
        {
            string path = Folder + "/ReplayItemView.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image bg = UiBuild.Image(null, "ReplayItemView", Color.white); Outline outline = bg.gameObject.AddComponent<Outline>(); ReplayItemView view = bg.gameObject.AddComponent<ReplayItemView>();
            Image icon = UiBuild.Image(bg.transform, "Icon", Color.white); UiBuild.Stretch(icon.rectTransform, 4); icon.preserveAspect = true; icon.raycastTarget = false;
            Image cooldown = UiBuild.Image(bg.transform, "CooldownOverlay", ReplayStyle.CooldownOverlay); UiBuild.Stretch(cooldown.rectTransform); cooldown.type = Image.Type.Filled; cooldown.fillMethod = Image.FillMethod.Vertical; cooldown.fillOrigin = (int)Image.OriginVertical.Bottom; cooldown.raycastTarget = false;
            Image flash = UiBuild.Image(bg.transform, "FlashOverlay", Color.clear); UiBuild.Stretch(flash.rectTransform); flash.raycastTarget = false; flash.gameObject.SetActive(false);
            Text label = UiBuild.Text(bg.transform, "Name", "Vật phẩm", ReplayStyle.SmallFont); UiBuild.Stretch(label.rectTransform, 4); label.resizeTextForBestFit = true; label.resizeTextMinSize = 9;
            Text status = UiBuild.Text(bg.transform, "Status", "", 12); status.color = Color.white; status.rectTransform.anchorMin = new Vector2(0, 0); status.rectTransform.anchorMax = new Vector2(1, 0); status.rectTransform.pivot = new Vector2(.5f, 0); status.rectTransform.sizeDelta = new Vector2(0, 22);
            UiBuild.Set(view, "background", bg); UiBuild.Set(view, "rarityOutline", outline); UiBuild.Set(view, "icon", icon); UiBuild.Set(view, "cooldownOverlay", cooldown); UiBuild.Set(view, "flashOverlay", flash); UiBuild.Set(view, "nameText", label); UiBuild.Set(view, "statusText", status); Save(bg.gameObject, path);
        }

        private static void BuildEffect(bool overwrite)
        {
            string path = Folder + "/VfxEffect.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image image = UiBuild.Image(null, "VfxEffect", Color.white); image.raycastTarget = false; image.gameObject.SetActive(false); Save(image.gameObject, path);
        }

        private static void BuildLogRow(bool overwrite)
        {
            string path = Folder + "/ReplayLogRow.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Text row = UiBuild.Text(null, "ReplayLogRow", "Sự kiện", 18, TextAnchor.MiddleLeft); row.color = ReplayStyle.Paper; row.horizontalOverflow = HorizontalWrapMode.Wrap; row.gameObject.AddComponent<LayoutElement>().preferredHeight = 27; AddShadow(row); Save(row.gameObject, path);
        }

        private static void BuildFighter(bool overwrite)
        {
            string path = Folder + "/FighterPanel.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image root = UiBuild.Image(null, "Avatar", ReplayStyle.Paper);
            UiBuild.ApplyUiSprite(root, "frame_panel", UnityEngine.UI.Image.Type.Sliced);
            root.color = new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .88f);
            FighterPanelView view = root.gameObject.AddComponent<FighterPanelView>();
            Image titleStrip = UiBuild.Image(root.transform, "TitleStrip", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .72f)); SetRect(titleStrip.rectTransform, 10, 424, 200, 38);
            Text name = UiBuild.Text(titleStrip.transform, "Name", "Hiệp Khách", 24, TextAnchor.MiddleCenter, UiFontRole.Title); name.color = ReplayStyle.Paper; UiBuild.Stretch(name.rectTransform, 6); UiBuild.FitLabel(name, UiBuild.LabelFitPreset.Plaque); AddShadow(name);
            Image frame = UiBuild.Image(root.transform, "Portrait", Color.white); SetRect(frame.rectTransform, 14, 220, 192, 198); UiBuild.ApplyUiSprite(frame, "hero_portrait_frame");
            Image backing = UiBuild.Image(frame.transform, "Backing", ReplayStyle.Ink); UiBuild.Stretch(backing.rectTransform, 12); backing.raycastTarget = false;
            Image portrait = UiBuild.Image(frame.transform, "HeroArt", Color.white); UiBuild.Stretch(portrait.rectTransform, 12); portrait.preserveAspect = true; portrait.raycastTarget = false;
            portrait.sprite = LoadArt("hero_portrait") ?? LoadArt("hero_idle") ?? frame.sprite;
            UiBuild.Set(view, "title", name); UiBuild.Set(view, "portraitFrame", frame); UiBuild.Set(view, "portrait", portrait);
            Bar(view, root.transform, "HpBar", 172, ReplayStyle.Hp, "hpFill", "hpText", "Sinh Lực  0/0");
            Bar(view, root.transform, "ShieldBar", 126, ReplayStyle.Shield, "shieldFill", "shieldText", "Cương Khí  0/0");
            Bar(view, root.transform, "QiBar", 80, ReplayStyle.Qi, "qiFill", "qiText", "Nội Lực  0/100");
            RectTransform chips = UiBuild.Rect(root.transform, "StatusChips"); SetRect(chips, 12, 4, 196, 72);
            GridLayoutGroup grid = chips.gameObject.AddComponent<GridLayoutGroup>(); grid.cellSize = new Vector2(58, 32); grid.spacing = new Vector2(7, 4); grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount; grid.constraintCount = 3;
            Status(view, chips, "Burn", "burnIcon", "burnText", "status_burn"); Status(view, chips, "Poison", "poisonIcon", "poisonText", "status_poison");
            Status(view, chips, "Bleed", "bleedIcon", "bleedText", "status_bleed"); Status(view, chips, "Stun", "stunIcon", "stunText", "status_stun");
            Status(view, chips, "Regen", "regenIcon", "regenText", "status_regen"); Status(view, chips, "Haste", "hasteIcon", "hasteText", "status_haste");
            Save(root.gameObject, path);
        }

        private static void Bar(FighterPanelView view, Transform parent, string name, float y, Color color, string fillField, string textField, string label)
        {
            Image root = UiBuild.Image(parent, name, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .28f)); SetRect(root.rectTransform, 10, y, 200, 38); UiBuild.ApplyUiSprite(root, "progress_bar", UnityEngine.UI.Image.Type.Sliced);
            Image fill = UiBuild.Image(root.transform, "Fill", color); UiBuild.Stretch(fill.rectTransform, 5); fill.type = UnityEngine.UI.Image.Type.Filled; fill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            Text text = UiBuild.Text(root.transform, "Value", label, 18, TextAnchor.MiddleCenter, UiFontRole.Emphasis); text.color = Color.white; text.lineSpacing = 1f; text.verticalOverflow = VerticalWrapMode.Overflow; text.raycastTarget = false; UiBuild.Stretch(text.rectTransform, 6); text.gameObject.AddComponent<SingleLineFit>().Configure(18, 12); AddShadow(text); UiBuild.Set(view, fillField, fill); UiBuild.Set(view, textField, text);
        }

        private static void Status(FighterPanelView view, Transform parent, string name, string iconField, string textField, string art)
        {
            Image icon = UiBuild.Image(parent, name, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .75f)); icon.preserveAspect = true; icon.raycastTarget = false; icon.sprite = LoadStatus(art);
            Text value = UiBuild.Text(icon.transform, "Count", "", 18, TextAnchor.LowerRight); value.color = Color.white; UiBuild.Stretch(value.rectTransform, 2); AddShadow(value); UiBuild.Set(view, iconField, icon); UiBuild.Set(view, textField, value);
        }

        private static void BuildScreen(bool overwrite)
        {
            string path = Folder + "/ReplayScreen.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            RectTransform root = UiBuild.Rect(null, "ReplayScreen"); ReplayScreen screen = root.gameObject.AddComponent<ReplayScreen>(); UiBuild.Stretch(root);
            Image top = DarkPanel(root, "TopBar", 0, 990, 1920, 90); Text stage = UiBuild.Text(top.transform, "StageOpponentName", "Hành Tẩu Giang Hồ", 26, TextAnchor.MiddleLeft, UiFontRole.Title); stage.color = Color.white; SetRect(stage.rectTransform, 28, 15, 680, 60); AddShadow(stage);
            Text clock = UiBuild.Text(top.transform, "Clock", "00:00", 30); clock.color = Color.white; SetRect(clock.rectTransform, 850, 15, 220, 60); AddShadow(clock); Button exit = CommonButton(top.transform, "ExitButton", "Thoát", 1660, 14, 190, 62);
            RectTransform band = UiBuild.Rect(root, "MiddleBand"); SetRect(band, 50, 135, 1820, 820);
            RectTransform player = UiBuild.Rect(band, "Player"); SetRect(player, 0, 0, 780, 820); FighterPanelView left = Fighter(player, 0, false); RectTransform leftBag = Bag(player, 220, 205, false);
            RectTransform gap = UiBuild.Rect(band, "TravelGap"); SetRect(gap, 780, 220, 260, 470);
            RectTransform enemy = UiBuild.Rect(band, "Enemy"); SetRect(enemy, 1040, 0, 780, 820); RectTransform rightBag = Bag(enemy, 0, 205, true); FighterPanelView right = Fighter(enemy, 560, true);
            Image bottom = DarkPanel(root, "BottomBar", 0, 0, 1920, 120); RectTransform speeds = UiBuild.Rect(bottom.transform, "SpeedButtons"); SetRect(speeds, 20, 24, 900, 72);
            Button play = CommonButton(speeds, "PauseResume", "Tạm dừng", 0, 4, 170, 64); Button s1 = CommonButton(speeds, "Speed x1", "x1", 180, 4, 170, 64); Button s2 = CommonButton(speeds, "Speed x2", "x2", 360, 4, 170, 64); Button s4 = CommonButton(speeds, "Speed x4", "x4", 540, 4, 170, 64); Button restart = CommonButton(speeds, "Restart", "Chơi lại", 720, 4, 170, 64);
            Button logToggle = CommonButton(bottom.transform, "LogToggle", "", 930, 12, 920, 96); Text collapsed = UiBuild.Text(logToggle.transform, "LastThreeLines", "Nhật ký trận", 18, TextAnchor.MiddleLeft); collapsed.color = Color.white; collapsed.horizontalOverflow = HorizontalWrapMode.Wrap; UiBuild.Stretch(collapsed.rectTransform, 16); AddShadow(collapsed);
            Image expanded = DarkPanel(root, "ExpandedBattleLog", 360, 120, 1200, 432); Image viewport = UiBuild.Image(expanded.transform, "Viewport", new Color(0, 0, 0, .08f)); UiBuild.Stretch(viewport.rectTransform, 18); viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Content"); content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(-16, 396); VerticalLayoutGroup logs = content.gameObject.AddComponent<VerticalLayoutGroup>(); logs.childControlHeight = false; logs.childForceExpandHeight = false; ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>(); fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = expanded.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport.rectTransform; scroll.content = content; scroll.horizontal = false; scroll.vertical = true; expanded.gameObject.SetActive(false);
            Image timeline = UiBuild.Image(bottom.transform, "Timeline", new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .3f)); SetRect(timeline.rectTransform, 920, 104, 930, 8); Slider slider = timeline.gameObject.AddComponent<Slider>(); Image fill = UiBuild.Image(timeline.transform, "Fill", ReplayStyle.Gold); UiBuild.Stretch(fill.rectTransform); slider.fillRect = fill.rectTransform; slider.targetGraphic = timeline;
            Text banner = UiBuild.Text(root, "EndBanner", "", 56, TextAnchor.MiddleCenter, UiFontRole.Display); banner.color = ReplayStyle.Gold; UiBuild.Anchors(banner.rectTransform, new Vector2(.25f, .40f), new Vector2(.75f, .64f)); banner.gameObject.SetActive(false);
            RectTransform vfxRect = UiBuild.Rect(root, "ReplayVfxLayer"); UiBuild.Stretch(vfxRect); ReplayVfxLayer vfx = vfxRect.gameObject.AddComponent<ReplayVfxLayer>(); UiBuild.Set(vfx, "effectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/VfxEffect.prefab").GetComponent<Image>());
            UiBuild.Set(screen, "leftFighter", left); UiBuild.Set(screen, "rightFighter", right); UiBuild.Set(screen, "leftBagItems", leftBag); UiBuild.Set(screen, "rightBagItems", rightBag); UiBuild.Set(screen, "itemPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ReplayItemView.prefab").GetComponent<ReplayItemView>()); UiBuild.Set(screen, "scrub", slider); UiBuild.Set(screen, "playLabel", play.GetComponentInChildren<Text>()); UiBuild.Set(screen, "timeLabel", clock); UiBuild.Set(screen, "stageLabel", stage); UiBuild.Set(screen, "eventLog", collapsed); UiBuild.Set(screen, "logContent", content); UiBuild.Set(screen, "collapsedLogContent", collapsed.rectTransform); UiBuild.Set(screen, "expandedLogContent", content); UiBuild.Set(screen, "logRowPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/ReplayLogRow.prefab").GetComponent<Text>()); UiBuild.Set(screen, "banner", banner); UiBuild.Set(screen, "vfxLayer", vfx); UiBuild.Set(screen, "playButton", play); UiBuild.Set(screen, "speed1Button", s1); UiBuild.Set(screen, "speed2Button", s2); UiBuild.Set(screen, "speed4Button", s4); UiBuild.Set(screen, "restartButton", restart); UiBuild.Set(screen, "exitButton", exit); UiBuild.Set(screen, "logToggleButton", logToggle); UiBuild.Set(screen, "expandedLogPanel", expanded.gameObject); Save(root.gameObject, path);
        }

        private static FighterPanelView Fighter(Transform parent, float x, bool enemy)
        {
            FighterPanelView fighter = UiBuild.InstantiateLinked<FighterPanelView>(Folder + "/FighterPanel.prefab", parent); fighter.name = "Avatar"; SetRect((RectTransform)fighter.transform, x, 170, 220, 470); UiBuild.SetBool(fighter, "enemy", enemy);
            return fighter;
        }

        private static RectTransform Bag(Transform parent, float x, float y, bool mirrored)
        {
            Image bag = UiBuild.Image(parent, "Bag", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .13f)); float pitch = ReplayStyle.CellSize + ReplayStyle.CellGap; SetRect(bag.rectTransform, x, y, 5 * pitch, 4 * pitch);
            for (int row = 0; row < 4; row++) for (int col = 0; col < 5; col++) { int visual = mirrored ? 4 - col : col; Image cell = UiBuild.Image(bag.transform, $"Cell {col},{row}", new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .67f)); cell.rectTransform.anchorMin = cell.rectTransform.anchorMax = new Vector2(0, 1); cell.rectTransform.pivot = new Vector2(0, 1); cell.rectTransform.anchoredPosition = new Vector2(visual * pitch, -row * pitch); cell.rectTransform.sizeDelta = new Vector2(ReplayStyle.CellSize, ReplayStyle.CellSize); }
            RectTransform items = UiBuild.Rect(bag.transform, "Items"); UiBuild.Stretch(items); return items;
        }

        private static Image DarkPanel(Transform parent, string name, float x, float y, float w, float h) { Image image = UiBuild.Image(parent, name, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .86f)); SetRect(image.rectTransform, x, y, w, h); UiBuild.ApplyUiSprite(image, "frame_panel", UnityEngine.UI.Image.Type.Sliced); image.color = new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .86f); return image; }
        private static Button CommonButton(Transform parent, string name, string label, float x, float y, float w, float h) { UiButton ui = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/Button_Secondary.prefab", parent); ui.name = name; SetRect((RectTransform)ui.transform, x, y, w, h); ui.GetComponent<Image>().pixelsPerUnitMultiplier = 10f; Text text = ui.GetComponentInChildren<Text>(); text.fontSize = 22; UiBuild.FitLabel(text, UiBuild.LabelFitPreset.OrnateButton); ui.SetLabel(label); return ui.GetComponent<Button>(); }
        private static Sprite LoadArt(string key) => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Textures2D/UI/" + key + ".png");
        private static Sprite LoadStatus(string key) => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Textures2D/Status/" + key + ".png");
        private static void AddShadow(Text text) { Shadow shadow = text.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, .8f); shadow.effectDistance = new Vector2(1.5f, -1.5f); }
        private static void SetRect(RectTransform rect, float x, float y, float w, float h) { rect.anchorMin = rect.anchorMax = Vector2.zero; rect.pivot = Vector2.zero; rect.anchoredPosition = new Vector2(x, y); rect.sizeDelta = new Vector2(w, h); }
        private static void Save(GameObject root, string path) { PrefabUtility.SaveAsPrefabAsset(root, path); Object.DestroyImmediate(root); }
    }
}
