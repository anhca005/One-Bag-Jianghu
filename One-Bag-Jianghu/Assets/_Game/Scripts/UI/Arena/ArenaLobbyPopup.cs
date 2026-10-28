using System;
using System.Collections;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Net;
using OneBagJianghu.Pve;
using OneBagJianghu.Bag.View;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Arena
{
    public sealed class ArenaLobbyPopup : BasePopup
    {
        private const string GuideKey = "jianghu.arena.guide.seen";
        [SerializeField] private Text seasonText;
        [SerializeField] private Text reputationText;
        [SerializeField] private Text jadeText;
        [SerializeField] private Text heroText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text powerText;
        [SerializeField] private Text rewardedText;
        [SerializeField] private Text weeklyText;
        [SerializeField] private Text rewardsText;
        [SerializeField] private Text leaderboardEmptyText;
        [SerializeField] private Text historyEmptyText;
        [SerializeField] private Image weeklyFill;
        [SerializeField] private Image rankBadge;
        [SerializeField] private Image heroPortrait;
        [SerializeField] private Sprite voDanhBadge;
        [SerializeField] private Sprite tanTuBadge;
        [SerializeField] private Sprite thieuHiepBadge;
        [SerializeField] private Sprite daiHiepBadge;
        [SerializeField] private Sprite danhTucBadge;
        [SerializeField] private Sprite tongSuBadge;
        [SerializeField] private RectTransform miniBag;
        [SerializeField] private RectTransform miniBagItems;
        [SerializeField] private GameObject connectionBanner;
        [SerializeField] private Text errorText;
        [SerializeField] private Text guideText;
        [SerializeField] private UiButton backButton;
        [SerializeField] private UiButton bagButton;
        [SerializeField] private UiButton findButton;
        [SerializeField] private UiButton rankingTab;
        [SerializeField] private UiButton historyTab;
        [SerializeField] private UiButton rewardsTab;
        [SerializeField] private UiButton shopTab;
        [SerializeField] private GameObject rankingPanel;
        [SerializeField] private GameObject historyPanel;
        [SerializeField] private GameObject rewardsPanel;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private RectTransform leaderboardContent;
        [SerializeField] private RectTransform historyContent;
        [SerializeField] private LeaderboardRow leaderboardRowPrefab;
        [SerializeField] private ArenaHistoryRow historyRowPrefab;
        [SerializeField] private LeaderboardRow selfRow;
        [SerializeField] private UiButton enterShopButton;
        [SerializeField] private GameObject matchingLayer;
        [SerializeField] private RectTransform spinner;
        [SerializeField] private Text matchingTimeText;
        [SerializeField] private Text matchingRangeText;
        [SerializeField] private Text matchingWidenText;
        [SerializeField] private UiButton cancelMatchButton;
        [SerializeField] private GameObject versusCard;
        [SerializeField] private Text versusPlayerText;
        [SerializeField] private Text versusOpponentText;
        [SerializeField] private Image versusPlayerBadge;
        [SerializeField] private Image versusOpponentBadge;
        [SerializeField] private Image versusPlayerPortrait;
        [SerializeField] private Image versusOpponentPortrait;
        [SerializeField] private Text versusPlayerRankText;
        [SerializeField] private Text versusOpponentRankText;
        [SerializeField] private Text versusPlayerPowerText;
        [SerializeField] private Text versusOpponentPowerText;
        [SerializeField] private Text versusPlayerSectText;
        [SerializeField] private Text versusOpponentSectText;
        [SerializeField] private UiButton enterBattleButton;
        private ArenaStatus arenaStatus;
        private ArenaResult pendingResult;
        private float matchingStartedAt;
        private ArenaSearch activeSearch;
        private bool awaitingSearch;
        private bool findWhenShown;
        private Coroutine versusRoutine;

        protected override void Awake()
        {
            base.Awake();
            backButton.onClick.AddListener(Close);
            bagButton.onClick.AddListener(OpenBag);
            findButton.onClick.AddListener(FindMatch);
            rankingTab.onClick.AddListener(() => ShowTab(0));
            historyTab.onClick.AddListener(() => ShowTab(1));
            rewardsTab.onClick.AddListener(() => ShowTab(2));
            shopTab.onClick.AddListener(() => ShowTab(3));
            enterShopButton.onClick.AddListener(OpenReputationShop);
            enterBattleButton.onClick.AddListener(EnterBattle);
            cancelMatchButton.onClick.AddListener(CancelMatch);
        }

        protected override void OnSetup(object data)
        {
            closeOnBackdrop = false;
            errorText.text = string.Empty;
            matchingLayer.SetActive(false);
            versusCard.SetActive(false);
            weeklyFill.fillAmount = 0f;
            weeklyText.text = "Đang tải mốc tuần…";
            leaderboardEmptyText?.gameObject.SetActive(false);
            historyEmptyText?.gameObject.SetActive(false);
            // The popup is reused: only a match the session still holds may show the versus card again.
            pendingResult = GameBootstrap.Session?.PendingArenaResult;
            ShowTab(0);
            BindProfile();
            SetGuide();
            SubscribeNetwork();
            RequestAll();
            // Setup runs before the popup is active, so coroutines wait for OnShown.
            if (GameBootstrap.Session.AutoFindArenaMatch)
            {
                GameBootstrap.Session.AutoFindArenaMatch = false;
                findWhenShown = true;
            }
        }

        protected override void OnShown()
        {
            PlayerProfile.ProgressChanged += HandleProgressChanged;
            if (findWhenShown)
            {
                findWhenShown = false;
                FindMatch();
            }
            else if (pendingResult != null && !versusCard.activeSelf)
            {
                // The match finished while the lobby was closed: show the versus card now.
                StartCoroutine(ShowVersusAfter(0f));
            }
        }

        private void Update()
        {
            if (matchingLayer.activeSelf)
            {
                spinner.Rotate(0f, 0f, -180f * Time.unscaledDeltaTime);
                UpdateMatchingText(DateTimeOffset.UtcNow);
            }
        }

        protected override void OnHidden()
        {
            PlayerProfile.ProgressChanged -= HandleProgressChanged;
            CancelMatch();
            UnsubscribeNetwork();
        }


        private void SubscribeNetwork()
        {
            if (GameBootstrap.Network != null)
            {
                GameBootstrap.Network.StateChanged += HandleNetworkState;
            }
        }

        private void UnsubscribeNetwork()
        {
            if (GameBootstrap.Network != null)
            {
                GameBootstrap.Network.StateChanged -= HandleNetworkState;
            }
        }

        private void HandleNetworkState(NetworkState state)
        {
            bool disconnected = state != NetworkState.Online;
            connectionBanner.SetActive(disconnected);
            findButton.GetComponent<Button>().interactable = !disconnected && pendingResult == null;
            if (disconnected && awaitingSearch)
            {
                awaitingSearch = false;
                activeSearch = null;
                matchingLayer.SetActive(false);
                ShowError("Mất kết nối, đã huỷ tìm trận");
            }
        }

        private void RequestAll()
        {
            NetworkService network = GameBootstrap.Network;
            HandleNetworkState(network == null ? NetworkState.Offline : network.State);
            if (network == null || !network.IsAuthenticated)
            {
                ShowError("Bạn cần đăng nhập để vào Luận Kiếm Đài");
                return;
            }

            network.RequestArenaStatus(BindStatus, ShowError);
            network.RequestLeaderboard(BindLeaderboard, ShowError);
            network.RequestArenaHistory(BindHistory, ShowError);
        }

        private void BindProfile()
        {
            GameSession session = GameBootstrap.Session;
            session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            session.Profile.Materials.TryGetValue("chi_ton_ngoc_tuy", out int jade);
            reputationText.text = UiFormat.Number(reputation);
            jadeText.text = UiFormat.Number(jade);
            heroText.text = session.HeroName + "\n" + SectName(session.SectId)
                + " · Tâm Pháp " + session.TamPhapLevel;
            powerText.text = "Chiến Uy " + UiFormat.Number(session.PlayerPower());
            BindPortrait(heroPortrait, false, null);
            BindMiniBag();
        }

        private void HandleProgressChanged()
        {
            if (IsShowing)
            {
                BindProfile();
            }
        }

        private void BindMiniBag()
        {
            ItemIconLibrary icons = ItemIconLibrary.Load();
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            for (int i = 0; i < miniBagItems.childCount; i++)
            {
                miniBagItems.GetChild(i).gameObject.SetActive(false);
            }

            int itemIndex = 0;
            foreach (LoadoutPlacement placement in GameBootstrap.Session.Profile.Loadout)
            {
                OwnedItem item = GameBootstrap.Session.Profile.RequireItem(placement.Uid);
                if (itemIndex >= miniBagItems.childCount || !catalog.TryGet(item.ItemId, out ItemInfo info))
                {
                    continue;
                }

                RectTransform itemView = (RectTransform)miniBagItems.GetChild(itemIndex++);
                itemView.gameObject.SetActive(true);
                itemView.anchoredPosition = new Vector2(placement.X * 60f, -placement.Y * 60f);
                itemView.sizeDelta = new Vector2(info.Width * 56f + (info.Width - 1) * 4f,
                    info.Height * 56f + (info.Height - 1) * 4f);
                Image frame = itemView.GetComponent<Image>();
                frame.color = BagViewStyle.Rarity(item.Rarity);
                Image icon = itemView.Find("Icon").GetComponent<Image>();
                icon.sprite = icons != null && icons.TryGet(item.ItemId, out Sprite sprite) ? sprite : null;
                icon.enabled = icon.sprite != null;
            }
        }

        private void BindStatus(ArenaStatus value)
        {
            arenaStatus = value;
            ArenaRankDefinition rank = ArenaRanks.Load().ForRating(value.Rating);
            statusText.text = "Bậc " + rank.Name + "\nĐiểm " + UiFormat.Number(value.Rating) + "\nHạng "
                + (value.Rank > 0 ? "#" + UiFormat.Number(value.Rank) : "Chưa xếp hạng") + "\nThắng "
                + UiFormat.Number(value.Wins)
                + " · Thua " + UiFormat.Number(value.Losses) + "\nCao nhất mùa: " + UiFormat.Number(value.PeakRating);
            rankBadge.sprite = RankSprite(rank.ImageKey);
            rankBadge.enabled = rankBadge.sprite != null;
            TimeSpan remaining = value.SeasonEndsAt - DateTimeOffset.Now;
            seasonText.text = "Kỳ Luận Kiếm " + value.SeasonId + " · " + FormatRemaining(remaining);
            rewardedText.text = value.RewardedLeftToday > 0
                ? "Lượt có thưởng hôm nay: " + value.RewardedLeftToday + "/10"
                : "Hết lượt có thưởng, vẫn được tính điểm";
            weeklyFill.fillAmount = value.WeeklyWinsTarget <= 0
                ? 0f
                : Mathf.Clamp01(value.WeeklyWins / (float)value.WeeklyWinsTarget);
            weeklyText.text = value.WeeklyJadeClaimed
                ? "Đã nhận Ngọc Tủy tuần này"
                : "Thắng tuần " + UiFormat.Number(value.WeeklyWins) + "/" + UiFormat.Number(value.WeeklyWinsTarget)
                    + " → 1 Chí Tôn Ngọc Tủy";
            if (rewardsText != null)
            {
                rewardsText.text = BuildRewardsText(value.WeeklyWinsTarget);
            }
            findButton.GetComponent<Button>().interactable = GameBootstrap.Network.State == NetworkState.Online;
            GameBootstrap.Network.TryShowSeasonReward();
        }

        private void BindLeaderboard(LeaderboardResult value)
        {
            Clear(leaderboardContent);
            leaderboardEmptyText?.gameObject.SetActive(value.Entries == null || value.Entries.Count == 0);
            foreach (LeaderboardEntry entry in value.Entries ?? new List<LeaderboardEntry>())
            {
                LeaderboardRow row = Instantiate(leaderboardRowPrefab, leaderboardContent);
                row.gameObject.SetActive(true);
                row.Bind(entry, value.Self != null && entry.Rank == value.Self.Rank);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(leaderboardContent);

            if (value.Self != null)
            {
                selfRow.Bind(new LeaderboardEntry
                {
                    Rank = value.Self.Rank,
                    Rating = value.Self.Rating,
                    HeroName = GameBootstrap.Session.HeroName,
                    Wins = arenaStatus?.Wins ?? 0,
                    Losses = arenaStatus?.Losses ?? 0
                }, true);
            }
        }

        private void BindHistory(ArenaHistoryResult value)
        {
            Clear(historyContent);
            historyEmptyText?.gameObject.SetActive(value.Entries == null || value.Entries.Count == 0);
            foreach (ArenaHistoryEntry entry in value.Entries ?? new List<ArenaHistoryEntry>())
            {
                ArenaHistoryRow row = Instantiate(historyRowPrefab, historyContent);
                row.gameObject.SetActive(true);
                row.Bind(entry, RequestReplay);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(historyContent);
        }

        private void FindMatch()
        {
            if (pendingResult != null)
            {
                return;
            }

            if (!GameBootstrap.Session.TryValidateLoadout(out string warning))
            {
                PopupManager.Show<MessagePopup>(new MessagePopupData("Không thể xuất chiêu", warning, "Đóng"));
                return;
            }

            errorText.text = string.Empty;
            matchingLayer.SetActive(true);
            matchingStartedAt = Time.unscaledTime;
            awaitingSearch = true;
            activeSearch = null;
            matchingTimeText.text = "Đã chờ 00:00";
            matchingRangeText.text = "Đang bắt đầu tìm đối thủ…";
            matchingWidenText.text = string.Empty;
            findButton.GetComponent<Button>().interactable = false;
            GameBootstrap.Network.FindArenaMatch(GameBootstrap.Session.CurrentLoadoutJson,
                GameBootstrap.Session.PlayerPower(),
                HandleSearching,
                HandleMatch,
                HandleMatchError);
        }

        private void HandleSearching(ArenaSearch search)
        {
            activeSearch = search;
            UpdateMatchingText(DateTimeOffset.UtcNow);
        }

        private void HandleMatch(ArenaResult result)
        {
            bool matchedImmediately = activeSearch == null;
            awaitingSearch = false;
            activeSearch = null;
            pendingResult = result;
            ArenaMatchLogic.StoreBattle(GameBootstrap.Session, result);
            if (!isActiveAndEnabled)
            {
                return;
            }

            float delay = matchedImmediately ? Mathf.Max(0f, 1.2f - (Time.unscaledTime - matchingStartedAt)) : 0f;
            StartCoroutine(ShowVersusAfter(delay));
        }

        private void CancelMatch()
        {
            if (!awaitingSearch)
            {
                return;
            }

            awaitingSearch = false;
            activeSearch = null;
            matchingLayer.SetActive(false);
            GameBootstrap.Network?.CancelArenaSearch();
            HandleNetworkState(GameBootstrap.Network == null ? NetworkState.Offline : GameBootstrap.Network.State);
        }

        private IEnumerator ShowVersusAfter(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            matchingLayer.SetActive(false);
            versusCard.SetActive(true);
            ArenaRankDefinition ownRank = ArenaRanks.Load().ForRating(pendingResult.RatingBefore);
            ArenaRankDefinition enemyRank = ArenaRanks.Load().ForRating(pendingResult.Opponent.Rating);
            versusPlayerText.text = GameBootstrap.Session.HeroName;
            versusOpponentText.text = pendingResult.Opponent.Name;
            versusPlayerBadge.sprite = RankSprite(ownRank.ImageKey);
            versusOpponentBadge.sprite = RankSprite(enemyRank.ImageKey);
            versusPlayerBadge.enabled = versusPlayerBadge.sprite != null;
            versusOpponentBadge.enabled = versusOpponentBadge.sprite != null;
            BindPortrait(versusPlayerPortrait, false, null);
            BindPortrait(versusOpponentPortrait, true, pendingResult.Opponent.Name + ":" + pendingResult.Opponent.Sect);
            versusPlayerRankText.text = ownRank.Name + " · " + UiFormat.Number(pendingResult.RatingBefore);
            versusOpponentRankText.text = enemyRank.Name + " · " + UiFormat.Number(pendingResult.Opponent.Rating);
            versusPlayerPowerText.text = "Chiến Uy " + UiFormat.Number(GameBootstrap.Session.PlayerPower());
            versusOpponentPowerText.text = "Chiến Uy " + UiFormat.Number(pendingResult.Opponent.Power);
            versusPlayerSectText.text = SectName(GameBootstrap.Session.SectId);
            versusOpponentSectText.text = SectName(pendingResult.Opponent.Sect)
                + (pendingResult.Opponent.Bot ? " · máy" : string.Empty);
            versusRoutine = StartCoroutine(AutoEnterBattle());
        }

        private IEnumerator AutoEnterBattle()
        {
            yield return new WaitForSecondsRealtime(2.5f);
            EnterBattle();
        }

        private void EnterBattle()
        {
            if (pendingResult == null)
            {
                return;
            }

            if (versusRoutine != null)
            {
                StopCoroutine(versusRoutine);
            }

            pendingResult = null;
            versusRoutine = null;
            GameBootstrap.Session.SelectedMode = GameMode.Pvp;
            Close();
            SceneFlow.Go(GameScene.Gameplay);
        }

        private void HandleMatchError(string message)
        {
            awaitingSearch = false;
            activeSearch = null;
            pendingResult = null;
            matchingLayer.SetActive(false);
            ShowError(message);
        }

        private void RequestReplay(long battleId)
        {
            GameBootstrap.Network.RequestArenaReplay(battleId, HandleReplay, ShowError);
        }

        private void HandleReplay(ArenaReplayResult replay)
        {
            ArenaMatchLogic.StoreReplay(GameBootstrap.Session, replay);
            GameBootstrap.Session.SelectedMode = GameMode.ArenaReplay;
            Close();
            SceneFlow.Go(GameScene.Gameplay);
        }

        private void ShowTab(int tab)
        {
            rankingPanel.SetActive(tab == 0);
            historyPanel.SetActive(tab == 1);
            rewardsPanel.SetActive(tab == 2);
            shopPanel.SetActive(tab == 3);
        }

        private void OpenBag()
        {
            BagPopup popup = PopupManager.Show<BagPopup>();
            if (popup != null)
            {
                popup.Closed += HandleBagClosed;
            }
        }

        private void HandleBagClosed(BasePopup popup)
        {
            popup.Closed -= HandleBagClosed;
            BindProfile();
        }

        private static void OpenReputationShop()
        {
            PopupManager.Show<ShopPopup>(ShopPopup.OpenTab.Reputation);
        }

        private void SetGuide()
        {
            bool seen = PlayerPrefs.GetInt(GuideKey, 0) != 0;
            guideText.gameObject.SetActive(!seen);
            if (!seen)
            {
                PlayerPrefs.SetInt(GuideKey, 1);
                PlayerPrefs.Save();
            }
        }

        private void ShowError(string message)
        {
            errorText.text = string.IsNullOrWhiteSpace(message) ? "Không kết nối được máy chủ" : message;
            findButton.GetComponent<Button>().interactable = false;
        }

        private static string FormatRemaining(TimeSpan remaining)
        {
            if (remaining <= TimeSpan.Zero)
            {
                return "đã kết thúc";
            }

            if (remaining.TotalDays < 1d)
            {
                return "còn " + Math.Max(1, (int)Math.Ceiling(remaining.TotalHours)) + " giờ";
            }

            return "còn " + Math.Max(1, (int)Math.Ceiling(remaining.TotalDays)) + " ngày";
        }

        private Sprite RankSprite(string key)
        {
            switch (key)
            {
                case "tan_tu":
                    return tanTuBadge;
                case "thieu_hiep":
                    return thieuHiepBadge;
                case "dai_hiep":
                    return daiHiepBadge;
                case "danh_tuc":
                    return danhTucBadge;
                case "tong_su":
                    return tongSuBadge;
                default:
                    return voDanhBadge;
            }
        }

        private void UpdateMatchingText(DateTimeOffset now)
        {
            if (activeSearch == null)
            {
                return;
            }

            TimeSpan waited = now - activeSearch.StartedAt;
            TimeSpan untilWiden = activeSearch.NextWidenAt - now;
            matchingTimeText.text = "Đã chờ " + FormatClock(waited);
            matchingRangeText.text = "Tìm đối thủ quanh " + UiFormat.Number(activeSearch.Rating)
                + " điểm (±" + activeSearch.Range + ")";
            matchingWidenText.text = "Nới khoảng sau " + FormatClock(untilWiden);
        }

        private static string FormatClock(TimeSpan value)
        {
            int seconds = Math.Max(0, (int)value.TotalSeconds);
            return (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }

        private static string SectName(string sectId)
        {
            if (string.IsNullOrWhiteSpace(sectId))
            {
                return "Vô Môn Phái";
            }

            SectCatalog catalog = SectCatalog.LoadFromResources();
            return catalog.TryGet(sectId, out SectInfo sect) ? sect.Name : "Vô Môn Phái";
        }

        public static string BuildRewardsText(int weeklyWinsTarget)
        {
            string target = weeklyWinsTarget > 0 ? UiFormat.Number(weeklyWinsTarget) : "—";
            return target + " thắng tuần → 1 Chí Tôn Ngọc Tủy\n\n"
                + "Thắng 30 / thua 10 Danh Vọng\n10 trận đầu mỗi ngày\n\n"
                + "Thưởng cuối mùa theo hạng:\n"
                + "Hạng 1: 3.000 Danh Vọng · 3 Ngọc Tủy\n"
                + "Hạng 2–3: 2.000 Danh Vọng · 2 Ngọc Tủy\n"
                + "Hạng 4–10: 1.200 Danh Vọng · 1 Ngọc Tủy\n"
                + "Hạng 11–50: 600 Danh Vọng · 0 Ngọc Tủy\n"
                + "Sang mùa mới đặt lại 1.000 điểm.";
        }

        private static void BindPortrait(Image target, bool opponent, string stableId)
        {
            PveArtLibrary art = PveArtLibrary.Load();
            target.sprite = opponent
                ? art?.ResolveArenaPortrait(stableId)
                : art != null && art.TryGet("hero_portrait", out Sprite sprite) ? sprite : null;
            target.enabled = target.sprite != null;
        }

        private static void Clear(RectTransform content)
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
}
