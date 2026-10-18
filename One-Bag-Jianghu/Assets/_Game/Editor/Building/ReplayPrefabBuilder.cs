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
        [MenuItem("One Bag Jianghu/Build/Replay Prefabs (Overwrite)")] public static void
            BuildReplayPrefabsOverwrite() => Build(true);
        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(overwrite);
            UiBuild.EnsureFolder(Folder);
            BuildItem(overwrite);
            BuildFighter(overwrite);
            BuildEffect(overwrite);
            BuildLogRow(overwrite);
            BuildScreen(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildItem(bool overwrite)
        {
            string path = Folder + "/ReplayItemView.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image bg = UiBuild.Image(null, "ReplayItemView", Color.white);
            Outline outline = bg.gameObject.AddComponent<Outline>();
            ReplayItemView view = bg.gameObject.AddComponent<ReplayItemView>();
            Image icon = UiBuild.Image(bg.transform, "Icon", Color.white);
            UiBuild.Stretch(icon.rectTransform, 4);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Image cooldown = UiBuild.Image(bg.transform, "CooldownOverlay", ReplayStyle.CooldownOverlay);
            UiBuild.Stretch(cooldown.rectTransform);
            cooldown.type = Image.Type.Filled;
            cooldown.fillMethod = Image.FillMethod.Vertical;
            cooldown.fillOrigin = (int)Image.OriginVertical.Bottom;
            cooldown.raycastTarget = false;
            Image flash = UiBuild.Image(bg.transform, "FlashOverlay", Color.clear);
            UiBuild.Stretch(flash.rectTransform);
            flash.raycastTarget = false;
            flash.gameObject.SetActive(false);
            Text label = UiBuild.Text(bg.transform, "Name", "Vật phẩm", ReplayStyle.SmallFont);
            UiBuild.Stretch(label.rectTransform, 4);
            label.gameObject.AddComponent<SingleLineFit>().Configure(ReplayStyle.SmallFont, 9);
            Text status = UiBuild.Text(bg.transform, "Status", "", 12);
            status.color = Color.white;
            status.rectTransform.anchorMin = new Vector2(0, 0);
            status.rectTransform.anchorMax = new Vector2(1, 0);
            status.rectTransform.pivot = new Vector2(.5f, 0);
            status.rectTransform.sizeDelta = new Vector2(0, 22);
            Image rarityFrame = UiBuild.Image(bg.transform, "Rarity Frame", Color.white);
            UiBuild.Stretch(rarityFrame.rectTransform, 3f);
            rarityFrame.raycastTarget = false;
            UiBuild.ApplyItemFrame(rarityFrame);
            UiBuild.Set(view, "background", bg);
            UiBuild.Set(view, "rarityOutline", outline);
            UiBuild.Set(view, "icon", icon);
            UiBuild.Set(view, "cooldownOverlay", cooldown);
            UiBuild.Set(view, "rarityFrame", rarityFrame);
            UiBuild.Set(view, "flashOverlay", flash);
            UiBuild.Set(view, "nameText", label);
            UiBuild.Set(view, "statusText", status);
            Save(bg.gameObject, path);
        }

        private static void BuildEffect(bool overwrite)
        {
            string path = Folder + "/VfxEffect.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image image = UiBuild.Image(null, "VfxEffect", Color.white);
            image.raycastTarget = false;
            image.gameObject.SetActive(false);
            Save(image.gameObject, path);
        }

        private static void BuildLogRow(bool overwrite)
        {
            string path = Folder + "/ReplayLogRow.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Text row = UiBuild.Text(null, "ReplayLogRow", "Sự kiện", 18, TextAnchor.MiddleLeft);
            row.color = ReplayStyle.Paper;
            row.horizontalOverflow = HorizontalWrapMode.Wrap;
            row.verticalOverflow = VerticalWrapMode.Truncate;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 44f;
            AddShadow(row);
            Save(row.gameObject, path);
        }

        private static void BuildFighter(bool overwrite)
        {
            string path = Folder + "/FighterPanel.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            Image root = UiBuild.Image(null, "Avatar", ReplayStyle.Paper);
            UiBuild.ApplyUiSprite(root, "Kit/fighter_panel_replay", Image.Type.Simple);
            // The replay avatar size, which is also the art's aspect.
            root.rectTransform.sizeDelta = new Vector2(250f, 360f);
            root.color = new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .88f);
            FighterPanelView view = root.gameObject.AddComponent<FighterPanelView>();
            Image titleStrip = UiBuild.Image(root.transform, "TitleStrip", new Color(ReplayStyle.Ink.r,
                ReplayStyle.Ink.g, ReplayStyle.Ink.b, .72f));
            // 240x48 is the name_plate art aspect (5:1); centred in the 250 px panel.
            SetRect(titleStrip.rectTransform, 5, 302, 240, 48);
            UiBuild.ApplyUiSprite(titleStrip, "Kit/name_plate", Image.Type.Simple);
            Text name = UiBuild.Text(titleStrip.transform, "Name", "Hiệp Khách", 24, TextAnchor.MiddleCenter,
                UiFontRole.Title);
            name.color = ReplayStyle.Paper;
            UiBuild.Stretch(name.rectTransform, 6);
            UiBuild.FitLabel(name, UiBuild.LabelFitPreset.Plaque);
            // Long sect build names wrap to two lines and shrink inside the strip.
            Object.DestroyImmediate(name.GetComponent<SingleLineFit>());
            name.horizontalOverflow = HorizontalWrapMode.Wrap;
            name.verticalOverflow = VerticalWrapMode.Truncate;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 11;
            name.resizeTextMaxSize = 22;
            AddShadow(name);
            Image frame = UiBuild.Image(root.transform, "Portrait", Color.white);
            SetRect(frame.rectTransform, 14, 18, 222, 276);
            UiBuild.ApplyUiSprite(frame, "Kit/hero_portrait_frame_wide");
            Image backing = UiBuild.Image(frame.transform, "Backing", ReplayStyle.Ink);
            UiBuild.Stretch(backing.rectTransform, 22);
            backing.raycastTarget = false;
            Image portrait = UiBuild.Image(frame.transform, "HeroArt", Color.white);
            UiBuild.Stretch(portrait.rectTransform, 22);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
            portrait.sprite = LoadArt("hero_portrait") ?? LoadArt("hero_idle") ?? frame.sprite;
            UiBuild.Set(view, "title", name);
            UiBuild.Set(view, "portraitFrame", frame);
            UiBuild.Set(view, "portrait", portrait);
            Save(root.gameObject, path);
        }

        private static void Bar(FighterPanelView view, Transform parent, string name, Rect rect, Color color,
            string frameSprite, string fillSprite, string fillField, string textField, string label, bool mirrored,
            int fontSize)
        {
            // Battle resource bar: ornate frame with a solid-colour fallback fill.
            Image root = UiBuild.Image(parent, name,
                new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .28f));
            SetRect(root.rectTransform, rect.x, rect.y, rect.width, rect.height);
            UiBuild.ApplyUiSprite(root, "Kit/" + frameSprite, Image.Type.Simple);

            Image fill = UiBuild.Image(root.transform, "Fill", color);
            float horizontalInset = frameSprite == "gauge_frame_large" ? 13f : 8f;
            float verticalInset = frameSprite == "gauge_frame_large" ? 8f : 4f;
            UiBuild.Stretch(fill.rectTransform);
            fill.rectTransform.offsetMin = new Vector2(horizontalInset, verticalInset);
            fill.rectTransform.offsetMax = new Vector2(-horizontalInset, -verticalInset);
            // The "_wide" art is the tapered-off "_core" brush tiled out to this fill's own aspect, so the
            // fill covers its rect undistorted and fillAmount only crops it.
            UiBuild.ApplyUiSprite(fill, "Kit/" + fillSprite + "_wide", Image.Type.Filled);
            fill.fillMethod = Image.FillMethod.Horizontal;
            // The enemy's bars drain toward the centre of the screen, so they fill from the right.
            fill.fillOrigin = mirrored ? (int)Image.OriginHorizontal.Right : (int)Image.OriginHorizontal.Left;
            Text text = UiBuild.Text(
                root.transform,
                "Value",
                label,
                fontSize,
                TextAnchor.MiddleCenter,
                UiFontRole.Emphasis);
            text.color = Color.white;
            text.lineSpacing = 1f;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            UiBuild.Stretch(text.rectTransform, 6);
            text.gameObject.AddComponent<SingleLineFit>().Configure(fontSize, 11);
            AddShadow(text);
            AddOutline(text);
            UiBuild.Set(view, fillField, fill);
            UiBuild.Set(view, textField, text);
        }

        private static void Status(FighterPanelView view, Transform parent, string name, string iconField,
            string textField, string art)
        {
            Image socket = UiBuild.Image(parent, name, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g,
                ReplayStyle.Ink.b, .75f));
            UiBuild.ApplyUiSprite(socket, "Kit/status_slot", Image.Type.Simple);

            Image icon = UiBuild.Image(socket.transform, "Icon", Color.white);
            UiBuild.Stretch(icon.rectTransform, 5f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.sprite = LoadStatus(art);
            Image badge = UiBuild.Image(socket.transform, "CountBadge", ReplayStyle.Ink);
            badge.rectTransform.anchorMin = new Vector2(1f, 0f);
            badge.rectTransform.anchorMax = new Vector2(1f, 0f);
            badge.rectTransform.pivot = new Vector2(1f, 0f);
            badge.rectTransform.anchoredPosition = new Vector2(3f, -3f);
            badge.rectTransform.sizeDelta = new Vector2(20f, 14f);
            UiBuild.ApplyUiSprite(badge, "Kit/count_badge_compact", Image.Type.Simple);
            Text value = UiBuild.Text(badge.transform, "Count", "", 11, TextAnchor.MiddleCenter);
            value.color = Color.white;
            // The badge is tiny: never truncate the digits.
            value.horizontalOverflow = HorizontalWrapMode.Overflow;
            value.verticalOverflow = VerticalWrapMode.Overflow;
            UiBuild.Stretch(value.rectTransform, 2f);
            AddShadow(value);
            UiBuild.Set(view, iconField, icon);
            UiBuild.Set(view, textField, value);
        }

        private static void FighterHud(FighterPanelView view, Transform parent, float x, bool mirrored)
        {
            const float hudWidth = 780f;

            // Each paper HUD owns one fighter's resources while the component remains on the avatar panel.
            Image hud = UiBuild.Image(parent, mirrored ? "EnemyHud" : "PlayerHud", Color.white);
            SetRect(hud.rectTransform, x, 8f, hudWidth, 148f);
            UiBuild.ApplyUiSprite(hud, "Kit/hud_panel_simple", Image.Type.Simple);

            hud.color = new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .94f);
            TextAnchor nameAlignment = mirrored ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            Text fighterName = UiBuild.Text(
                hud.transform,
                "Name",
                "Hiệp Khách",
                25,
                nameAlignment,
                UiFontRole.Title);
            fighterName.color = ReplayStyle.Ink;
            // Clear the ~50 px gold corner ornament of hud_panel_simple at the outer top corner.
            float nameX = mirrored ? 286f : 64f;
            float barX = mirrored ? 138f : 22f;
            float statusX = mirrored ? 18f : 654f;
            SetRect(fighterName.rectTransform, nameX, 108f, 430f, 34f);
            fighterName.gameObject.AddComponent<SingleLineFit>().Configure(25, 13);
            UiBuild.Set(view, "bannerTitle", fighterName);
            Bar(view, hud.transform, "HpBar", new Rect(barX, 67f, 620f, 39f), ReplayStyle.Hp,
                "gauge_frame_large", "bar_fill_hp", "hpFill", "hpText", "Khí Huyết  0/0", mirrored, 20);
            Bar(view, hud.transform, "ShieldBar", new Rect(barX, 44f, 620f, 21f), ReplayStyle.Shield,
                "gauge_frame_small", "bar_fill_shield", "shieldFill", "shieldText", "Cương Khí  0/0", mirrored, 13);
            Bar(view, hud.transform, "QiBar", new Rect(barX, 20f, 620f, 21f), ReplayStyle.Qi,
                "gauge_frame_small", "bar_fill_qi", "qiFill", "qiText", "Nội Lực  0/100", mirrored, 13);

            // Statuses sit on the outer edge, away from the central timer.
            RectTransform chips = UiBuild.Rect(hud.transform, "StatusChips");
            SetRect(chips, statusX, 12f, 90f, 128f);
            GridLayoutGroup grid = chips.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(40f, 40f);
            grid.spacing = new Vector2(4f, 4f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            Status(view, chips, "Burn", "burnIcon", "burnText", "status_burn");
            Status(view, chips, "Poison", "poisonIcon", "poisonText", "status_poison");
            Status(view, chips, "Bleed", "bleedIcon", "bleedText", "status_bleed");
            Status(view, chips, "Stun", "stunIcon", "stunText", "status_stun");
            Status(view, chips, "Regen", "regenIcon", "regenText", "status_regen");
            Status(view, chips, "Haste", "hasteIcon", "hasteText", "status_haste");
        }

        private static void BuildScreen(bool overwrite)
        {
            string path = Folder + "/ReplayScreen.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            RectTransform root = UiBuild.Rect(null, "ReplayScreen");
            ReplayScreen screen = root.gameObject.AddComponent<ReplayScreen>();
            UiBuild.Stretch(root);
            Image top = DarkPanel(root, "TopBar", 0, 916, 1920, 164);

            // Clock plate: one readable centre for both the mode title and timer.
            Image clockPlate = UiBuild.Image(top.transform, "ClockPlate", Color.white);
            SetRect(clockPlate.rectTransform, 820f, 20f, 280f, 112f);
            UiBuild.ApplyUiSprite(clockPlate, "Kit/clock_plate", Image.Type.Simple);

            // Emblem sits on the plate (not under it), faint, behind the outlined timer and mode title.
            Image versus = UiBuild.Image(top.transform, "Vs Emblem", Color.white);
            versus.raycastTarget = false;
            versus.preserveAspect = true;
            SetRect(versus.rectTransform, 915f, 31f, 90f, 90f);
            if (UiBuild.ApplyUiSprite(versus, "Kit/vs_emblem"))
            {
                versus.color = new Color(1f, 1f, 1f, .4f);
            }
            else
            {
                versus.gameObject.SetActive(false);
            }

            Text stage = UiBuild.Text(
                clockPlate.transform,
                "StageOpponentName",
                "Hành Tẩu Giang Hồ",
                16,
                TextAnchor.MiddleCenter,
                UiFontRole.Title);
            stage.color = Color.white;
            SetRect(stage.rectTransform, 20f, 10f, 240f, 28f);
            stage.gameObject.AddComponent<SingleLineFit>().Configure(16, 11);
            AddShadow(stage);
            AddOutline(stage);
            Text clock = UiBuild.Text(
                clockPlate.transform,
                "Clock",
                "00:00",
                30,
                TextAnchor.MiddleCenter,
                UiFontRole.Emphasis);
            clock.color = Color.white;
            SetRect(clock.rectTransform, 30f, 50f, 220f, 48f);
            AddShadow(clock);
            AddOutline(clock);

            // Round exit seal just below the banner's right end (the enemy HUD covers the banner itself).
            Button exit = CommonButton(top.transform, "ExitButton", "Thoát", 1832f, -96f, 64f, 64f);
            Image exitImage = exit.GetComponent<Image>();
            if (UiBuild.ApplyUiSprite(exitImage, "Kit/button_exit"))
            {
                exitImage.preserveAspect = true;
            }

            Text exitLabel = exit.GetComponentInChildren<Text>();
            SetRect(exitLabel.rectTransform, -22f, -24f, 108f, 22f);
            exitLabel.fontSize = 15;
            AddShadow(exitLabel);
            RectTransform band = UiBuild.Rect(root, "MiddleBand");
            SetRect(band, 40f, 135f, 1840f, 765f);
            RectTransform player = UiBuild.Rect(band, "Player");
            SetRect(player, 0f, 0f, 810f, 765f);
            FighterPanelView left = Fighter(player, 0, false);
            RectTransform leftBag = Bag(player, 270f, 190f, false);
            RectTransform gap = UiBuild.Rect(band, "TravelGap");
            SetRect(gap, 810f, 220f, 220f, 470f);
            RectTransform enemy = UiBuild.Rect(band, "Enemy");
            SetRect(enemy, 1030f, 0f, 810f, 765f);
            RectTransform rightBag = Bag(enemy, 0f, 190f, true);
            FighterPanelView right = Fighter(enemy, 580f, true);
            FighterHud(left, top.transform, 20f, false);
            FighterHud(right, top.transform, 1120f, true);
            Image bottom = DarkPanel(root, "BottomBar", 0, 0, 1920, 120);
            RectTransform speeds = UiBuild.Rect(bottom.transform, "SpeedButtons");
            SetRect(speeds, 20, 24, 520, 72);
            Button play = CommonButton(speeds, "PauseResume", "Tạm dừng", 0, 4, 170, 64);
            UiBuild.ApplyUiSprite(play.GetComponent<Image>(), "Kit/button_replay_control", Image.Type.Simple);
            Image playIcon = AddControlIcon(play, "icon_pause");
            Button s1 = SquareButton(speeds, "Speed x1", "x1", 184f);
            Button s2 = SquareButton(speeds, "Speed x2", "x2", 254f);
            Button s4 = SquareButton(speeds, "Speed x4", "x4", 324f);

            // Collapsed log: the panel itself remains clickable and clips to its quiet paper centre.
            Button logToggle = CommonButton(bottom.transform, "LogToggle", "", 550, 12, 1300, 96);
            Image logToggleImage = logToggle.GetComponent<Image>();
            UiBuild.ApplyUiSprite(logToggleImage, "Kit/log_panel_wide", Image.Type.Simple);
            Image collapsedViewport = UiBuild.Image(logToggle.transform, "Log Viewport", Color.clear);
            UiBuild.Stretch(collapsedViewport.rectTransform);
            collapsedViewport.rectTransform.offsetMin = new Vector2(72f, 16f);
            collapsedViewport.rectTransform.offsetMax = new Vector2(-72f, -16f);
            collapsedViewport.gameObject.AddComponent<RectMask2D>();
            Text collapsed = UiBuild.Text(collapsedViewport.transform, "LastThreeLines", "Nhật ký trận", 17,
                TextAnchor.MiddleLeft);
            collapsed.color = Color.white;
            collapsed.horizontalOverflow = HorizontalWrapMode.Wrap;
            collapsed.verticalOverflow = VerticalWrapMode.Truncate;
            UiBuild.Stretch(collapsed.rectTransform);
            AddShadow(collapsed);
            Image expanded = UiBuild.Image(root, "ExpandedBattleLog", ReplayStyle.Paper);
            SetRect(expanded.rectTransform, 300f, 120f, 1320f, 440f);
            UiBuild.ApplyUiSprite(expanded, "Kit/log_panel_expanded", Image.Type.Simple);

            Image viewport = UiBuild.Image(expanded.transform, "Viewport",
                new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .78f));
            UiBuild.Stretch(viewport.rectTransform);
            viewport.rectTransform.offsetMin = new Vector2(76f, 42f);
            viewport.rectTransform.offsetMax = new Vector2(-76f, -42f);
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport.transform, "Content");
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(.5f, 1);
            content.sizeDelta = new Vector2(-24f, 356f);
            VerticalLayoutGroup logs = content.gameObject.AddComponent<VerticalLayoutGroup>();
            logs.childControlHeight = true;
            logs.childForceExpandHeight = false;
            logs.childControlWidth = true;
            logs.childForceExpandWidth = true;
            logs.spacing = 4f;
            logs.padding = new RectOffset(18, 18, 12, 12);
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = expanded.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            expanded.gameObject.SetActive(false);
            Image timeline = UiBuild.Image(bottom.transform, "Timeline",
                new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .3f));
            SetRect(timeline.rectTransform, 550, 98, 1300, 20);
            UiBuild.ApplyUiSprite(timeline, "Kit/timeline_bar_thin", Image.Type.Simple);
            Slider slider = timeline.gameObject.AddComponent<Slider>();
            Image fill = UiBuild.Image(timeline.transform, "Fill", ReplayStyle.Gold);
            UiBuild.Stretch(fill.rectTransform, 5f);
            slider.fillRect = fill.rectTransform;
            slider.targetGraphic = timeline;
            Text banner = UiBuild.Text(root, "EndBanner", "", 56, TextAnchor.MiddleCenter, UiFontRole.Display);
            banner.color = ReplayStyle.Gold;
            UiBuild.Anchors(banner.rectTransform, new Vector2(.25f, .40f), new Vector2(.75f, .64f));
            banner.gameObject.SetActive(false);
            RectTransform vfxRect = UiBuild.Rect(root, "ReplayVfxLayer");
            UiBuild.Stretch(vfxRect);
            ReplayVfxLayer vfx = vfxRect.gameObject.AddComponent<ReplayVfxLayer>();
            UiBuild.Set(vfx, "effectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder +
                "/VfxEffect.prefab").GetComponent<Image>());
            UiBuild.Set(screen, "leftFighter", left);
            UiBuild.Set(screen, "rightFighter", right);
            UiBuild.Set(screen, "leftBagItems", leftBag);
            UiBuild.Set(screen, "rightBagItems", rightBag);
            UiBuild.Set(screen, "itemPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder +
                "/ReplayItemView.prefab").GetComponent<ReplayItemView>());
            UiBuild.Set(screen, "scrub", slider);
            UiBuild.Set(screen, "playLabel", play.GetComponentInChildren<Text>());
            UiBuild.Set(screen, "playIcon", playIcon);
            UiBuild.Set(screen, "playSprite", UiBuild.LoadUiSprite("Kit/icon_play"));
            UiBuild.Set(screen, "pauseSprite", UiBuild.LoadUiSprite("Kit/icon_pause"));
            UiBuild.Set(screen, "timeLabel", clock);
            UiBuild.Set(screen, "stageLabel", stage);
            UiBuild.Set(screen, "eventLog", collapsed);
            UiBuild.Set(screen, "logContent", content);
            UiBuild.Set(screen, "collapsedLogContent", collapsed.rectTransform);
            UiBuild.Set(screen, "expandedLogContent", content);
            UiBuild.Set(screen, "logRowPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Folder +
                "/ReplayLogRow.prefab").GetComponent<Text>());
            UiBuild.Set(screen, "banner", banner);
            UiBuild.Set(screen, "vfxLayer", vfx);
            UiBuild.Set(screen, "playButton", play);
            UiBuild.Set(screen, "speed1Button", s1);
            UiBuild.Set(screen, "speed2Button", s2);
            UiBuild.Set(screen, "speed4Button", s4);
            UiBuild.Set(screen, "exitButton", exit);
            UiBuild.Set(screen, "logToggleButton", logToggle);
            UiBuild.Set(screen, "expandedLogPanel", expanded.gameObject);
            Save(root.gameObject, path);
        }

        private static FighterPanelView Fighter(Transform parent, float x, bool enemy)
        {
            FighterPanelView fighter = UiBuild.InstantiateLinked<FighterPanelView>(Folder +
                "/FighterPanel.prefab", parent);
            fighter.name = "Avatar";
            SetRect((RectTransform)fighter.transform, x, 238f, 250f, 360f);
            UiBuild.SetBool(fighter, "enemy", enemy);
            return fighter;
        }

        private static RectTransform Bag(Transform parent, float x, float y, bool mirrored)
        {
            Image bag = UiBuild.Image(parent, "Bag", new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g,
                ReplayStyle.Ink.b, .13f));
            float pitch = ReplayStyle.CellSize + ReplayStyle.CellGap;
            SetRect(bag.rectTransform, x, y, 5 * pitch, 4 * pitch);
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    int visualColumn = mirrored ? 4 - column : column;
                    Image cell = UiBuild.Image(bag.transform, $"Cell {column},{row}",
                        new Color(ReplayStyle.Paper.r, ReplayStyle.Paper.g, ReplayStyle.Paper.b, .67f));
                    UiBuild.ApplyUiSprite(cell, "Kit/slot_cell_square", Image.Type.Simple);
                    cell.rectTransform.anchorMin = new Vector2(0f, 1f);
                    cell.rectTransform.anchorMax = new Vector2(0f, 1f);
                    cell.rectTransform.pivot = new Vector2(0f, 1f);
                    cell.rectTransform.anchoredPosition = new Vector2(visualColumn * pitch, -row * pitch);
                    cell.rectTransform.sizeDelta = new Vector2(ReplayStyle.CellSize, ReplayStyle.CellSize);
                }
            }
            RectTransform items = UiBuild.Rect(bag.transform, "Items");
            UiBuild.Stretch(items);
            return items;
        }

        private static Image DarkPanel(Transform parent, string name, float x, float y, float w, float h) {
            Image image = UiBuild.Image(parent, name, new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g,
            ReplayStyle.Ink.b, .86f));
        SetRect(image.rectTransform, x, y, w, h);
        string spriteKey = name == "TopBar" ? "Kit/replay_top_bar" : "Kit/replay_bottom_bar";
        UiBuild.ApplyUiSprite(image, spriteKey, UnityEngine.UI.Image.Type.Simple);
        image.color = new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .86f);
        return image;
        }
        private static Image AddControlIcon(Button button, string key)
        {
            Image icon = UiBuild.Image(button.transform, "Control Icon", Color.white);
            if (!UiBuild.ApplyUiSprite(icon, "Kit/" + key))
            {
                Object.DestroyImmediate(icon.gameObject);
                return null;
            }
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = new Vector2(0f, .5f);
            icon.rectTransform.pivot = new Vector2(0f, .5f);
            icon.rectTransform.anchoredPosition = new Vector2(30f, 0f);
            icon.rectTransform.sizeDelta = new Vector2(24f, 24f);
            Text label = button.GetComponentInChildren<Text>();
            label.rectTransform.offsetMin = new Vector2(58f, label.rectTransform.offsetMin.y);
            return icon;
        }

        private static Button SquareButton(Transform parent, string name, string label, float x)
        {
            Button button = CommonButton(parent, name, label, x, 4f, 64f, 64f);
            Image image = button.GetComponent<Image>();
            if (UiBuild.ApplyUiSprite(image, "Kit/button_square", Image.Type.Simple))
            {
                image.preserveAspect = true;
            }

            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = 18;
            UiBuild.Stretch(text.rectTransform, 8f);
            return button;
        }
        private static Button CommonButton(Transform parent, string name, string label, float x, float y, float
            w, float h) { UiButton ui = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder +
            "/Button_Secondary.prefab", parent);
        ui.name = name;
        SetRect((RectTransform)ui.transform, x, y, w, h);
        Text text = ui.GetComponentInChildren<Text>();
        text.fontSize = 22;
        UiBuild.FitLabel(text, UiBuild.LabelFitPreset.OrnateButton);
        ui.SetLabel(label);
        return ui.GetComponent<Button>();
        }
        private static Sprite LoadArt(string key) =>
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Textures2D/UI/" + key + ".png");
        private static Sprite LoadStatus(string key) =>
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Textures2D/Status/" + key + ".png");
        private static void AddShadow(Text text) { Shadow shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, .8f);
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }
        private static void AddOutline(Text text)
        {
            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, .9f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
        }
        private static void SetRect(RectTransform rect, float x, float y, float w, float h) { rect.anchorMin =
            rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(w, h);
        }
        private static void Save(GameObject root, string path) { PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        }
    }
}
