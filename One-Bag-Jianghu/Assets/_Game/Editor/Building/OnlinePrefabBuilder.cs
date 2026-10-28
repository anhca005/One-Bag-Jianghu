using OneBagJianghu.UI.Arena;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Pve;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class OnlinePrefabBuilder
    {
        private const string Folder = "Assets/_Game/Resources/UI/Popups";
        private const string Common = CommonUiPrefabBuilder.Folder + "/";

        public static void Build(bool overwrite)
        {
            UiBuild.EnsureFolder(Folder);
            LeaderboardRow leaderboard = BuildLeaderboardRow(overwrite);
            ArenaHistoryRow history = BuildHistoryRow(overwrite);
            BuildAccount(overwrite);
            BuildArena(leaderboard, history, overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildAccount(bool overwrite)
        {
            const string path = Folder + "/AccountPopup.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;

            GameObject root = PopupRoot("AccountPopup");
            AccountPopup popup = root.AddComponent<AccountPopup>();
            Image frame = UiBuild.Image(root.transform, "Panel", Color.white);
            Center(frame.rectTransform, 828f, 760f);
            UiBuild.ApplyUiSprite(frame, "Kit/popup_frame_square", Image.Type.Simple);
            RectTransform panel = frame.rectTransform;
            AddRibbonTitle(panel, "Danh Thiếp Giang Hồ");
            UiButton login = Button(panel, "Login Tab", "Đăng nhập", true, 138f, 132f, 260f, 62f);
            UiButton register = Button(panel, "Register Tab", "Đăng ký", false, 430f, 132f, 260f, 62f);

            RectTransform column = Group(panel, "Input Column", 199f, 196f, 430f, 334f);
            VerticalLayoutGroup layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            RectTransform usernameGroup = InputGroup(column, "Username Group");
            InputField username = Input(usernameGroup, "Username", "Tên đăng nhập", 430f);
            RectTransform passwordGroup = InputGroup(column, "Password Group");
            InputField password = Input(passwordGroup, "Password", "Mật khẩu", 368f);
            UiButton eye = Button(passwordGroup, "Password Eye", string.Empty, false, 374f, 10f, 56f, 56f);
            UiBuild.ApplyUiSprite(eye.GetComponent<Image>(), "Kit/button_square", Image.Type.Simple);
            Image eyeIcon = FixedImage(eye.transform, "Eye Icon", 8f, 8f, 40f, 40f);
            eyeIcon.enabled = UiBuild.ApplyUiSprite(eyeIcon, "Kit/icon_eye_closed", Image.Type.Simple);
            RectTransform repeatedGroup = InputGroup(column, "Repeated Password Group");
            InputField repeated = Input(repeatedGroup, "Repeated Password", "Nhập lại", 430f);
            RectTransform heroGroup = InputGroup(column, "Hero Name Group");
            InputField hero = Input(heroGroup, "Hero Name", "Tên Hiệp Khách", 430f);

            RectTransform footer = Group(panel, "Form Footer", 54f, 526f, 720f, 134f);
            Toggle remember = Toggle(footer, 145f, 0f);
            Text error = Label(footer, "Error", string.Empty, 17, 80f, 38f, 560f, 28f);
            error.color = UiBuild.Vermilion;
            UiButton confirm = Button(footer, "Confirm", "Đăng nhập", true, 94f, 72f, 260f, 62f);
            UiButton later = Button(footer, "Later", "Để sau", false, 346f, 72f, 260f, 62f);
            Label(panel, "Note", "Mọi hành trang và tiến trình được lưu an toàn trên máy chủ.", 14,
                99f, 716f, 630f, 24f);
            SetFields(popup,
                new[] { "panel", "loginTab", "registerTab", "usernameInput", "passwordInput" },
                new Object[] { panel, login, register, username, password });
            SetFields(popup,
                new[] { "repeatedPasswordGroup", "repeatedPasswordInput", "heroNameGroup", "heroNameInput" },
                new Object[] { repeatedGroup.gameObject, repeated, heroGroup.gameObject, hero });
            SetFields(popup,
                new[] { "rememberToggle", "passwordEyeButton", "confirmButton", "laterButton", "errorText" },
                new Object[] { remember, eye, confirm, later, error });
            SetFields(popup, new[] { "passwordEyeIcon", "eyeOpenSprite", "eyeClosedSprite" },
                new Object[] { eyeIcon, UiBuild.LoadUiSprite("Kit/icon_eye_open"),
                    UiBuild.LoadUiSprite("Kit/icon_eye_closed") });
            Assign(popup, "formFooter", footer);
            root.SetActive(false);
            Save(root, path);
        }

        private static void BuildArena(LeaderboardRow leaderboard, ArenaHistoryRow history, bool overwrite)
        {
            const string path = Folder + "/ArenaLobbyPopup.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;

            GameObject root = PopupRoot("ArenaLobbyPopup");
            ArenaLobbyPopup popup = root.AddComponent<ArenaLobbyPopup>();
            Image background = UiBuild.Image(root.transform, "Arena Background", Color.white);
            UiBuild.Stretch(background.rectTransform);
            UiBuild.ApplyUiSprite(background, "Arena/arena_lobby_bg", Image.Type.Simple);
            RectTransform panel = background.rectTransform;
            Image top = PlacedImage(panel, "Top Bar", "Arena/arena_top_bar", 0f, 0f, 1920f, 110f);
            UiButton back = Button(top.transform, "Back", "←", false, 100f, 23f, 64f, 64f);
            UiBuild.ApplyUiSprite(back.GetComponent<Image>(), "Kit/button_square", Image.Type.Simple);
            Text title = Label(top.transform, "Title", "LUẬN KIẾM ĐÀI", 36, 180f, 20f, 380f, 70f,
                TextAnchor.MiddleLeft, UiFontRole.Display);
            Text season = Label(top.transform, "Season", "Kỳ Luận Kiếm 1", 20, 560f, 20f, 620f, 70f,
                TextAnchor.MiddleLeft);
            Image reputationIcon = FixedImage(top.transform, "Reputation Icon", 1260f, 35f, 40f, 40f);
            SetCurrency(reputationIcon, "icon_danh_vong");
            Text reputation = Label(top.transform, "Reputation", "0", 22, 1310f, 20f, 220f, 70f,
                TextAnchor.MiddleLeft);
            Image jadeIcon = FixedImage(top.transform, "Jade Icon", 1580f, 35f, 40f, 40f);
            SetCurrency(jadeIcon, "chi_ton_ngoc_tuy");
            Text jade = Label(top.transform, "Jade", "0", 22, 1630f, 20f, 190f, 70f, TextAnchor.MiddleLeft);
            // The top bar art is dark lacquer: its text uses paper colour to stay readable.
            foreach (Text text in new[] { title, season, reputation, jade })
                text.color = UiBuild.Paper;

            Image heroCard = PlacedImage(panel, "Hero Card", "Arena/arena_panel_hero", 40f, 130f, 420f, 910f);
            Image portraitFrame = PlacedImage(heroCard.transform, "Portrait Frame", "Kit/hero_portrait_frame_wide",
                126f, 24f, 168f, 209f);
            Image portrait = FixedImage(portraitFrame.transform, "Hero Portrait", 14f, 17f, 140f, 176f);
            portrait.preserveAspect = true;
            SetPortrait(portrait);
            Image badge = FixedImage(heroCard.transform, "Rank Badge", 250f, 162f, 72f, 72f);
            UiBuild.ApplyUiSprite(badge, "Arena/arena_rank_vo_danh", Image.Type.Simple);
            badge.preserveAspect = true;
            Text hero = Label(heroCard.transform, "Hero", "Hiệp Khách\nVô Môn Phái · Tâm Pháp 1", 26,
                28f, 245f, 364f, 55f);
            Text status = Label(heroCard.transform, "Status", "Bậc Vô Danh\nĐiểm 1.000 · Hạng --\nThắng 0 · Thua 0",
                19, 28f, 305f, 364f, 110f);
            Text power = Label(heroCard.transform, "Power", "Chiến Uy 0", 19, 28f, 425f, 364f, 35f);
            RectTransform miniBag = MiniBag(heroCard.transform, out RectTransform miniBagItems);
            UiButton bag = Button(heroCard.transform, "Edit Bag", "Chỉnh Bao Phục", false,
                80f, 789f, 260f, 62f);

            Image match = PlacedImage(panel, "Match Card", "Arena/arena_panel_match", 480f, 680f, 820f, 360f);
            Text rewarded = Label(match.transform, "Rewarded", "Lượt có thưởng hôm nay: --", 22,
                40f, 30f, 740f, 40f);
            Text weekly = Label(match.transform, "Weekly", "Thắng tuần --/15 → 1 Chí Tôn Ngọc Tủy", 18,
                80f, 82f, 660f, 36f);
            Image track = FixedImage(match.transform, "Weekly Track", 100f, 126f, 580f, 24f);
            UiBuild.ApplyUiSprite(track, "Arena/arena_progress_track", Image.Type.Simple);
            Image fill = FixedImage(track.transform, "Fill", 0f, 0f, 580f, 24f);
            UiBuild.ApplyUiSprite(fill, "Arena/arena_progress_fill", Image.Type.Filled);
            fill.fillMethod = Image.FillMethod.Horizontal;
            Image rewardIcon = FixedImage(match.transform, "Weekly Reward Icon", 690f, 118f, 40f, 40f);
            SetCurrency(rewardIcon, "chi_ton_ngoc_tuy");
            UiButton find = Button(match.transform, "Find Match", "TÌM TRẬN", true, 200f, 184f, 420f, 100f);
            Text guide = Label(match.transform, "Guide",
                "Đội hình trong Bao Phục sẽ thành bóng kiếm cho người khác khiêu chiến.", 16,
                40f, 300f, 740f, 40f);

            Image tabs = PlacedImage(panel, "Tabs", "Arena/arena_panel_tabs", 1320f, 130f, 560f, 910f);
            UiButton rankingTab = Tab(tabs.transform, "Ranking Tab", "Xếp Hạng", true, 20f, 16f);
            UiButton historyTab = Tab(tabs.transform, "History Tab", "Lịch Sử", false, 288f, 16f);
            UiButton rewardsTab = Tab(tabs.transform, "Rewards Tab", "Thưởng", false, 20f, 84f);
            UiButton shopTab = Tab(tabs.transform, "Shop Tab", "Tiệm", false, 288f, 84f);
            RectTransform rankingPanel = Group(tabs.transform, "Ranking Panel", 20f, 160f, 520f, 720f);
            RectTransform leaderboardContent = Scroll(rankingPanel, "Leaderboard", 0f, 0f, 520f, 646f);
            LeaderboardRow selfRow = Object.Instantiate(leaderboard, rankingPanel);
            Place(selfRow.GetComponent<RectTransform>(), 0f, 656f, 520f, 64f);
            selfRow.gameObject.SetActive(true);
            RectTransform historyPanel = Group(tabs.transform, "History Panel", 20f, 160f, 520f, 730f);
            RectTransform historyContent = Scroll(historyPanel, "History", 0f, 0f, 520f, 730f);
            RectTransform rewardsPanel = Group(tabs.transform, "Rewards Panel", 20f, 160f, 520f, 730f);
            Text rewards = Label(rewardsPanel, "Rewards", ArenaLobbyPopup.BuildRewardsText(15),
                18, 20f, 30f, 480f, 670f);
            Text rankingEmpty = Label(rankingPanel, "Empty", "Chưa có dữ liệu xếp hạng mùa giải này.",
                18, 35f, 285f, 450f, 80f);
            Text historyEmpty = Label(historyPanel, "Empty", "Chưa có chiến tích luận kiếm nào.",
                18, 35f, 285f, 450f, 80f);
            rankingEmpty.gameObject.SetActive(false);
            historyEmpty.gameObject.SetActive(false);
            RectTransform shopPanel = Group(tabs.transform, "Shop Panel", 20f, 160f, 520f, 730f);
            // Only the ranking tab shows until the player picks another one.
            historyPanel.gameObject.SetActive(false);
            rewardsPanel.gameObject.SetActive(false);
            shopPanel.gameObject.SetActive(false);
            Label(shopPanel, "Shop", "TIỆM DANH VỌNG\nDùng Danh Vọng đổi kỳ trân.", 22,
                20f, 260f, 480f, 150f);
            UiButton enterShop = Button(shopPanel, "Enter Shop", "Vào tiệm", true, 130f, 450f, 260f, 62f);

            Image banner = PlacedImage(panel, "Connection Banner", "Arena/arena_connection_banner",
                480f, 130f, 820f, 56f);
            Label(banner.transform, "Text", "Mất kết nối, đang thử lại…", 19, 20f, 0f, 780f, 56f);
            banner.gameObject.SetActive(false);
            Text error = Label(panel, "Error", string.Empty, 18, 480f, 640f, 820f, 36f);
            error.color = UiBuild.Vermilion;
            GameObject matching = Matching(panel, out MatchingFields mf);
            GameObject versus = Versus(panel, out VersusFields vf);

            SetFields(popup,
                new[] { "panel", "seasonText", "reputationText", "jadeText", "heroText", "statusText" },
                new Object[] { panel, season, reputation, jade, hero, status });
            SetFields(popup,
                new[] { "powerText", "rewardedText", "weeklyText", "weeklyFill", "rankBadge", "miniBag" },
                new Object[] { power, rewarded, weekly, fill, badge, miniBag });
            SetFields(popup,
                new[] { "rewardsText", "leaderboardEmptyText", "historyEmptyText" },
                new Object[] { rewards, rankingEmpty, historyEmpty });
            SetFields(popup, new[] { "heroPortrait", "miniBagItems" }, new Object[] { portrait, miniBagItems });
            AssignRanks(popup);
            SetFields(popup,
                new[] { "connectionBanner", "errorText", "guideText", "backButton", "bagButton", "findButton" },
                new Object[] { banner.gameObject, error, guide, back, bag, find });
            SetFields(popup,
                new[] { "rankingTab", "historyTab", "rewardsTab", "shopTab", "rankingPanel", "historyPanel" },
                new Object[] { rankingTab, historyTab, rewardsTab, shopTab, rankingPanel.gameObject,
                    historyPanel.gameObject });
            SetFields(popup,
                new[] { "rewardsPanel", "shopPanel", "leaderboardContent", "historyContent" },
                new Object[] { rewardsPanel.gameObject, shopPanel.gameObject, leaderboardContent, historyContent });
            SetFields(popup,
                new[] { "leaderboardRowPrefab", "historyRowPrefab", "selfRow", "enterShopButton" },
                new Object[] { leaderboard, history, selfRow, enterShop });
            SetFields(popup,
                new[] { "matchingLayer", "spinner", "matchingTimeText", "matchingRangeText", "matchingWidenText",
                    "cancelMatchButton" },
                new Object[] { matching, mf.Spinner, mf.Time, mf.Range, mf.Widen, mf.Cancel });
            SetFields(popup, new[] { "versusCard", "versusPlayerText", "versusOpponentText" },
                new Object[] { versus, vf.PlayerName, vf.OpponentName });
            SetFields(popup,
                new[] { "versusPlayerBadge", "versusOpponentBadge", "versusPlayerPortrait",
                    "versusOpponentPortrait", "versusPlayerRankText",
                    "versusOpponentRankText", "versusPlayerPowerText", "versusOpponentPowerText" },
                new Object[] { vf.PlayerBadge, vf.OpponentBadge, vf.PlayerPortrait, vf.OpponentPortrait,
                    vf.PlayerRank, vf.OpponentRank,
                    vf.PlayerPower, vf.OpponentPower });
            SetFields(popup,
                new[] { "versusPlayerSectText", "versusOpponentSectText", "enterBattleButton" },
                new Object[] { vf.PlayerSect, vf.OpponentSect, vf.EnterButton });
            root.SetActive(false);
            Save(root, path);
        }

        private static GameObject Matching(Transform parent, out MatchingFields fields)
        {
            Image shade = UiBuild.Image(parent, "Matching Layer", new Color(0f, 0f, 0f, .6f));
            UiBuild.Stretch(shade.rectTransform);
            Image seal = FixedImage(shade.transform, "Spinner", 850f, 300f, 220f, 220f);
            seal.enabled = UiBuild.ApplyUiSprite(seal, "Arena/arena_seal_spinner", Image.Type.Simple);
            fields = new MatchingFields { Spinner = seal.rectTransform };
            Text label = Label(shade.transform, "Label", "Đang tìm đối thủ…", 30, 560f, 550f, 800f, 60f);
            label.color = Color.white;
            fields.Time = Label(shade.transform, "Wait Time", "Đã chờ 00:00", 22, 660f, 615f, 600f, 40f);
            fields.Range = Label(shade.transform, "Search Range", "Tìm đối thủ quanh 1.000 điểm (±50)", 22,
                560f, 660f, 800f, 40f);
            fields.Widen = Label(shade.transform, "Next Widen", "Nới khoảng sau 02:00", 19,
                660f, 705f, 600f, 36f);
            fields.Cancel = Button(shade.transform, "Cancel Match", "Huỷ", false, 830f, 765f, 260f, 62f);
            foreach (Text text in new[] { fields.Time, fields.Range, fields.Widen })
            {
                text.color = Color.white;
            }
            shade.gameObject.SetActive(false);
            return shade.gameObject;
        }

        private static GameObject Versus(Transform parent, out VersusFields fields)
        {
            Image card = PlacedImage(parent, "Versus Card", "Arena/arena_versus_card", 420f, 280f, 1080f, 520f);
            fields = new VersusFields {
                PlayerName = Label(card.transform, "Player Name", "Mình", 26, 40f, 250f, 420f, 42f),
                OpponentName = Label(card.transform, "Opponent Name", "Đối thủ", 26, 620f, 250f, 420f, 42f),
                PlayerRank = Label(card.transform, "Player Rank", "Vô Danh · 1.000", 19, 40f, 295f, 420f, 34f),
                OpponentRank = Label(card.transform, "Opponent Rank", "Vô Danh · 1.000", 19,
                    620f, 295f, 420f, 34f),
                PlayerPower = Label(card.transform, "Player Power", "Chiến Uy 0", 19, 40f, 335f, 420f, 34f),
                OpponentPower = Label(card.transform, "Opponent Power", "Chiến Uy 0", 19,
                    620f, 335f, 420f, 34f),
                PlayerSect = Label(card.transform, "Player Sect", "Vô Môn Phái", 18, 40f, 375f, 420f, 34f),
                OpponentSect = Label(card.transform, "Opponent Sect", "Vô Môn Phái", 18, 620f, 375f, 420f, 34f)
            };
            Image playerFrame = PlacedImage(card.transform, "Player Portrait Frame", "Kit/hero_portrait_frame_wide",
                126f, 25f, 168f, 209f);
            fields.PlayerPortrait = FixedImage(playerFrame.transform, "Portrait", 14f, 17f, 140f, 176f);
            SetPortrait(fields.PlayerPortrait);
            fields.PlayerPortrait.preserveAspect = true;
            fields.PlayerBadge = FixedImage(card.transform, "Player Badge", 250f, 163f, 72f, 72f);
            fields.PlayerBadge.enabled = UiBuild.ApplyUiSprite(fields.PlayerBadge,
                "Arena/arena_rank_vo_danh", Image.Type.Simple);
            Image opponentFrame = PlacedImage(card.transform, "Opponent Portrait Frame",
                "Kit/hero_portrait_frame_wide", 786f, 25f, 168f, 209f);
            fields.OpponentPortrait = FixedImage(opponentFrame.transform, "Portrait", 14f, 17f, 140f, 176f);
            SetPortrait(fields.OpponentPortrait);
            fields.OpponentPortrait.preserveAspect = true;
            fields.OpponentBadge = FixedImage(card.transform, "Opponent Badge", 910f, 163f, 72f, 72f);
            fields.OpponentBadge.enabled = UiBuild.ApplyUiSprite(fields.OpponentBadge,
                "Arena/arena_rank_vo_danh", Image.Type.Simple);
            Image art = FixedImage(card.transform, "Versus Art", 412f, 95f, 256f, 160f);
            art.enabled = UiBuild.ApplyUiSprite(art, "Arena/arena_versus", Image.Type.Simple);
            Label(card.transform, "Versus Text", "ĐẤU", 44, 412f, 125f, 256f, 80f,
                TextAnchor.MiddleCenter, UiFontRole.Display);
            fields.EnterButton = Button(card.transform, "Enter Battle", "Vào trận", true,
                410f, 440f, 260f, 62f);
            card.gameObject.SetActive(false);
            return card.gameObject;
        }

        private static LeaderboardRow BuildLeaderboardRow(bool overwrite)
        {
            const string path = Folder + "/LeaderboardRow.prefab";
            LeaderboardRow existing = AssetDatabase.LoadAssetAtPath<LeaderboardRow>(path);
            if (!overwrite && existing != null) return existing;
            Image image = UiBuild.Image(null, "LeaderboardRow", Color.white);
            image.rectTransform.sizeDelta = new Vector2(520f, 64f);
            SetRowLayout(image.gameObject, 64f);
            UiBuild.ApplyUiSprite(image, "Arena/arena_row_rank", Image.Type.Simple);
            LeaderboardRow row = image.gameObject.AddComponent<LeaderboardRow>();
            Image medal = FixedImage(image.transform, "Medal", 12f, 8f, 48f, 48f);
            Image rankBadge = FixedImage(image.transform, "Rank Badge", 84f, 12f, 40f, 40f);
            // Defaults until Bind runs: no medal (rank text shows), the starting Vô Danh badge.
            medal.preserveAspect = true;
            medal.gameObject.SetActive(false);
            rankBadge.enabled = UiBuild.ApplyUiSprite(rankBadge, "Arena/arena_rank_vo_danh", Image.Type.Simple);
            rankBadge.preserveAspect = true;
            SetFields(row, new[] { "background", "rankText", "nameText", "ratingText", "recordText" },
                new Object[] { image, Label(image.transform, "Rank", "#1", 16, 12f, 0f, 60f, 64f),
                    Label(image.transform, "Name", "Hiệp Khách", 16, 136f, 0f, 194f, 64f,
                        TextAnchor.MiddleLeft), Label(image.transform, "Rating", "1000", 16, 330f, 0f, 90f, 64f),
                    Label(image.transform, "Record", "0/0", 16, 420f, 0f, 88f, 64f) });
            SetFields(row, new[] { "medalImage", "rankBadge" }, new Object[] { medal, rankBadge });
            AssignArray(row, "medalSprites",
                new[] { "Arena/arena_medal_1", "Arena/arena_medal_2", "Arena/arena_medal_3" });
            AssignArray(row, "rankSprites", RankKeys());
            Assign(row, "normalSprite", UiBuild.LoadUiSprite("Arena/arena_row_rank"));
            Assign(row, "selectedSprite", UiBuild.LoadUiSprite("Arena/arena_row_self"));
            Save(image.gameObject, path);
            return AssetDatabase.LoadAssetAtPath<LeaderboardRow>(path);
        }

        private static ArenaHistoryRow BuildHistoryRow(bool overwrite)
        {
            const string path = Folder + "/ArenaHistoryRow.prefab";
            ArenaHistoryRow existing = AssetDatabase.LoadAssetAtPath<ArenaHistoryRow>(path);
            if (!overwrite && existing != null) return existing;
            Image image = UiBuild.Image(null, "ArenaHistoryRow", Color.white);
            image.rectTransform.sizeDelta = new Vector2(520f, 72f);
            SetRowLayout(image.gameObject, 72f);
            UiBuild.ApplyUiSprite(image, "Arena/arena_row_history", Image.Type.Simple);
            ArenaHistoryRow row = image.gameObject.AddComponent<ArenaHistoryRow>();
            UiButton replay = Button(image.transform, "Replay", "Xem lại", false, 378f, 16f, 132f, 32f);
            UiBuild.ApplyUiSprite(replay.GetComponent<Image>(), "Kit/button_small_gold_compact", Image.Type.Simple);
            SetFields(row, new[] { "resultText", "opponentText", "ratingText", "timeText", "replayButton" },
                new Object[] { Label(image.transform, "Result", "Thắng", 15, 12f, 0f, 68f, 72f),
                    Label(image.transform, "Opponent", "Đối thủ", 15, 80f, 0f, 170f, 72f),
                    Label(image.transform, "Rating", "+16", 15, 250f, 0f, 70f, 72f),
                    Label(image.transform, "Time", "2 giờ trước", 14, 320f, 0f, 90f, 72f), replay });
            Save(image.gameObject, path);
            return AssetDatabase.LoadAssetAtPath<ArenaHistoryRow>(path);
        }

        private static RectTransform MiniBag(Transform parent, out RectTransform items)
        {
            RectTransform grid = Group(parent, "Mini Bag 5x4", 62f, 470f, 296f, 236f);
            GridLayoutGroup layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(56f, 56f);
            layout.spacing = new Vector2(4f, 4f);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 5;
            for (int i = 0; i < 20; i++)
            {
                Image cell = UiBuild.Image(grid, "Cell " + i, new Color(1f, 1f, 1f, .28f));
                UiBuild.ApplyItemFrame(cell);
            }
            items = Group(parent, "Mini Bag Items", 62f, 470f, 296f, 236f);
            for (int i = 0; i < 20; i++)
            {
                Image frame = UiBuild.Image(items, "Item " + i, Color.white);
                Place(frame.rectTransform, 0f, 0f, 56f, 56f);
                UiBuild.ApplyItemFrame(frame);
                Image icon = UiBuild.Image(frame.transform, "Icon", Color.white);
                UiBuild.Stretch(icon.rectTransform, 5f);
                icon.preserveAspect = true;
                frame.gameObject.SetActive(false);
            }
            return grid;
        }

        private static void SetRowLayout(GameObject row, float height)
        {
            LayoutElement element = row.AddComponent<LayoutElement>();
            element.preferredWidth = 520f;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static RectTransform Scroll(Transform parent, string name, float x, float y, float w, float h)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Common + "ScrollList.prefab");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = name;
            Place((RectTransform)instance.transform, x, y, w, h);
            ScrollRect scroll = instance.GetComponent<ScrollRect>();
            VerticalLayoutGroup layout = scroll.content.GetComponent<VerticalLayoutGroup>();
            scroll.content.gameObject.SetActive(true);
            scroll.content.anchorMin = new Vector2(0f, 1f);
            scroll.content.anchorMax = new Vector2(1f, 1f);
            scroll.content.pivot = new Vector2(.5f, 1f);
            scroll.content.anchoredPosition = Vector2.zero;
            scroll.content.sizeDelta = Vector2.zero;
            if (layout == null) layout = scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = scroll.content.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = scroll.content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return scroll.content;
        }

        private static InputField Input(Transform parent, string name, string placeholder, float width)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Common + "InputField.prefab");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = name;
            // tooltip_field is 720x128: keep its aspect and centre a shorter field in the 76 px row.
            float height = Mathf.Min(76f, width * 128f / 720f);
            Place((RectTransform)instance.transform, 0f, (76f - height) / 2f, width, height);
            InputField field = instance.GetComponent<InputField>();
            field.placeholder.GetComponent<Text>().text = placeholder;
            field.textComponent.resizeTextForBestFit = true;
            field.textComponent.resizeTextMinSize = 12;
            return field;
        }

        private static RectTransform InputGroup(Transform parent, string name)
        {
            RectTransform group = UiBuild.Rect(parent, name);
            group.sizeDelta = new Vector2(430f, 76f);
            LayoutElement size = group.gameObject.AddComponent<LayoutElement>();
            size.preferredWidth = 430f;
            size.preferredHeight = 76f;
            return group;
        }

        private static Toggle Toggle(Transform parent, float x, float y)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Common + "Toggle.prefab");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = "Remember";
            Place((RectTransform)instance.transform, x, y, 32f, 32f);
            Label(parent, "Remember Label", "Ghi nhớ đăng nhập", 18, x + 44f, y, 276f, 32f,
                TextAnchor.MiddleLeft);
            return instance.GetComponent<Toggle>();
        }

        private static UiButton Tab(Transform parent, string name, string label, bool active, float x, float y)
        {
            UiButton tab = Button(parent, name, label, active, x, y, 252f, 60f);
            UiBuild.ApplyUiSprite(tab.GetComponent<Image>(), active ? "Kit/tab_active" : "Kit/tab_idle",
                Image.Type.Simple);
            if (!active) tab.GetComponentInChildren<Text>().color = UiBuild.Ink;
            Text text = tab.GetComponentInChildren<Text>();
            text.fontSize = 20;
            text.resizeTextForBestFit = true;
            return tab;
        }

        private static UiButton Button(Transform parent, string name, string label, bool primary,
            float x, float y, float w, float h)
        {
            UiButton button = UiBuild.InstantiateLinked<UiButton>(Common + "Button_"
                + (primary ? "Primary" : "Secondary") + ".prefab", parent);
            button.name = name;
            button.SetLabel(label);
            Place(button.GetComponent<RectTransform>(), x, y, w, h);
            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null) UiBuild.FitLabel(text, UiBuild.LabelFitPreset.None, 12);
            return button;
        }

        private static GameObject PopupRoot(string name)
        {
            GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            root.GetComponent<Canvas>().overrideSorting = true;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = .5f;
            return root;
        }

        private static Image PlacedImage(Transform parent, string name, string sprite,
            float x, float y, float w, float h)
        {
            Image image = UiBuild.Image(parent, name, Color.white);
            Place(image.rectTransform, x, y, w, h);
            image.enabled = UiBuild.ApplyUiSprite(image, sprite, Image.Type.Simple);
            return image;
        }

        private static Image FixedImage(Transform parent, string name, float x, float y, float w, float h)
        {
            Image image = UiBuild.Image(parent, name, Color.white);
            Place(image.rectTransform, x, y, w, h);
            image.type = Image.Type.Simple;
            image.raycastTarget = false;
            return image;
        }

        private static RectTransform Group(Transform parent, string name, float x, float y, float w, float h)
        {
            RectTransform group = UiBuild.Rect(parent, name);
            Place(group, x, y, w, h);
            return group;
        }

        private static Text Label(Transform parent, string name, string value, int size,
            float x, float y, float w, float h, TextAnchor alignment = TextAnchor.MiddleCenter,
            UiFontRole role = UiFontRole.Body)
        {
            Text text = UiBuild.Text(parent, name, value, size, alignment, role);
            Place(text.rectTransform, x, y, w, h);
            UiBuild.FitLabel(text, UiBuild.LabelFitPreset.None, 12);
            return text;
        }

        private static void Place(RectTransform rect, float x, float y, float w, float h)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
        }

        private static void Center(RectTransform rect, float w, float h)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(w, h);
        }

        private static void AddRibbonTitle(Transform parent, string value)
        {
            // Centred on the panel. The dark band of popup_title_ribbon spans 6%..51% of the art's height from the
            // top (tassels hang below), so the text is centred on that band, not on the whole 123 px ribbon.
            const float ribbonTop = -30f;
            float panelWidth = ((RectTransform)parent).sizeDelta.x;
            float ribbonLeft = (panelWidth - 520f) / 2f;
            Image ribbon = FixedImage(parent, "Title Ribbon", ribbonLeft, ribbonTop, 520f, 123f);
            UiBuild.ApplyUiSprite(ribbon, "Kit/popup_title_ribbon", Image.Type.Simple);
            float bandCentre = ribbonTop + 123f * (.06f + .51f) / 2f;
            Text title = Label(parent, "Title", value, 32, ribbonLeft + 60f, bandCentre - 22f, 400f, 44f,
                TextAnchor.MiddleCenter, UiFontRole.Display);
            title.color = UiBuild.Paper;
        }

        private static void SetCurrency(Image image, string key)
        {
            if (CurrencyArt.TryGetSprite(key, out Sprite sprite))
            {
                image.sprite = sprite;
                image.preserveAspect = true;
            }
            image.enabled = image.sprite != null;
        }

        private static void SetPortrait(Image image)
        {
            PveArtLibrary library = PveArtLibrary.Load();
            image.sprite = library != null && library.TryGet("hero_portrait", out Sprite sprite) ? sprite : null;
            image.enabled = image.sprite != null;
        }

        private static string[] RankKeys()
        {
            return new[] { "Arena/arena_rank_vo_danh", "Arena/arena_rank_tan_tu",
                "Arena/arena_rank_thieu_hiep", "Arena/arena_rank_dai_hiep",
                "Arena/arena_rank_danh_tuc", "Arena/arena_rank_tong_su" };
        }

        private static void AssignRanks(ArenaLobbyPopup popup)
        {
            string[] fields = { "voDanhBadge", "tanTuBadge", "thieuHiepBadge", "daiHiepBadge",
                "danhTucBadge", "tongSuBadge" };
            string[] keys = RankKeys();
            for (int i = 0; i < fields.Length; i++) Assign(popup, fields[i], UiBuild.LoadUiSprite(keys[i]));
        }

        private static void SetFields(Object target, string[] names, Object[] values)
        {
            for (int i = 0; i < names.Length; i++) Assign(target, names[i], values[i]);
        }

        private static void Assign(Object target, string property, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(property).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignArray(Object target, string property, string[] keys)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty values = serialized.FindProperty(property);
            values.arraySize = keys.Length;
            for (int i = 0; i < keys.Length; i++)
                values.GetArrayElementAtIndex(i).objectReferenceValue = UiBuild.LoadUiSprite(keys[i]);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Save(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private struct VersusFields
        {
            public Image PlayerBadge;
            public Image OpponentBadge;
            public Image PlayerPortrait;
            public Image OpponentPortrait;
            public Text PlayerName;
            public Text OpponentName;
            public Text PlayerRank;
            public Text OpponentRank;
            public Text PlayerPower;
            public Text OpponentPower;
            public Text PlayerSect;
            public Text OpponentSect;
            public UiButton EnterButton;
        }

        private struct MatchingFields
        {
            public RectTransform Spinner;
            public Text Time;
            public Text Range;
            public Text Widen;
            public UiButton Cancel;
        }
    }
}
