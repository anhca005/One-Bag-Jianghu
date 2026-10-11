using System;
using System.Collections;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using OneBagJianghu.UI.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Audio;

namespace OneBagJianghu.UI.Home
{
    public sealed class HomeController : MonoBehaviour
    {
        [SerializeField]
        private Image background;
        [SerializeField]
        private Image heroZone;
        [SerializeField]
        private Text heroName;
        [SerializeField]
        private Slider expBar;
        [SerializeField]
        private Text expText;
        [SerializeField]
        private Text levelText;
        [SerializeField]
        private Text powerText;
        [SerializeField]
        private Text staminaText;
        [SerializeField]
        private Text silverText;
        [SerializeField]
        private Text premiumText;
        [SerializeField]
        private Text reputationText;
        [SerializeField]
        private ItemTooltip staminaTooltip;
        [SerializeField]
        private HoverTooltipTarget staminaTooltipTarget;
        [SerializeField]
        private Button settingsButton;
        [SerializeField]
        private HomeHotspot pveHotspot;
        [SerializeField]
        private HomeHotspot pvpHotspot;
        [SerializeField]
        private HomeHotspot bagHotspot;
        [SerializeField]
        private HomeHotspot storageHotspot;
        [SerializeField]
        private HomeHotspot codexHotspot;
        [SerializeField]
        private HomeHotspot shopHotspot;
        [SerializeField]
        private HomeHotspot sectHotspot;
        [SerializeField]
        private HomeHotspot encyclopediaHotspot;
        private int displayedLevel = -1;
        private long displayedExp = -1;
        private StaminaConfig staminaConfig;
        private StaminaService staminaService;
        private float nextStaminaRefresh;
        private QuestTracker questTracker;
        private void Awake()
        {
            pveHotspot.OnClick.AddListener(StartPve);
            pvpHotspot.OnClick.AddListener(StartPvp);
            bagHotspot.OnClick.AddListener(OpenBag);
            storageHotspot.OnClick.AddListener(OpenStorage);
            codexHotspot.OnClick.AddListener(OpenComingSoon);
            shopHotspot.OnClick.AddListener(OpenShop);
            sectHotspot.OnClick.AddListener(OpenSect);
            encyclopediaHotspot.OnClick.AddListener(OpenEncyclopedia);
            settingsButton.onClick.AddListener(OpenSettings);
            settingsButton.onClick.AddListener(PlayClick);
        }

        private void Start()
        {
            AudioManager.EnsureExists();
            GameSession session = GameBootstrap.Session;
            // Coming back to the stage map after a battle: the map popup plays its chapter music, skip Home's.
            if (string.IsNullOrEmpty(session.PendingPveMapStageId))
                AudioManager.Instance.PlayMusic(MusicKeys.Home);
            staminaConfig = StaminaConfig.Load();
            staminaService = new StaminaService(staminaConfig);
            staminaTooltipTarget?.Initialize(staminaTooltip, BuildStaminaTooltip);
            RefreshStamina();
            RefreshCurrencies();
            heroName.text = session.HeroName;
            RefreshProgress();
            RefreshPower();
            RefreshBattleAvailability();
            CreateQuestTracker();
            SceneFlow.NotifyReady();
            if (!string.IsNullOrEmpty(session.PendingPveMapStageId))
            {
                string stageId = session.PendingPveMapStageId;
                session.PendingPveMapStageId = null;
                PopupManager.Show<StageMapPopup>(stageId);
            }

            if (session.HeroLevel >= LevelService.SectUnlockLevel
                && string.IsNullOrWhiteSpace(session.SectId)
                && !session.Profile.SectPromptShown)
            {
                session.Profile.SectPromptShown = true;
                session.SaveProfile();
                // The sect tutorial says the same thing and points at the hotspot; only show this without it.
                bool tutorialWillGuide = session.Profile.CompletedTutorials == null
                    || !session.Profile.CompletedTutorials.Contains("sect");
                if (!tutorialWillGuide)
                    PopupManager.Show<MessagePopup>(new MessagePopupData("Mở khóa Môn Phái",
                        "Bạn đã đạt cấp 10. Hãy đến điểm Môn Phái trên trang chủ để gia nhập.",
                        "Đã hiểu"));
            }
        }

        private void Update()
        {
            if (Time.unscaledTime >= nextStaminaRefresh)
            {
                nextStaminaRefresh = Time.unscaledTime + 1f;
                RefreshStamina();
            }
            if (GameBootstrap.Session != null
                && (displayedLevel != GameBootstrap.Session.HeroLevel
                || displayedExp != GameBootstrap.Session.Profile.HeroExp))
                RefreshProgress();
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || PopupManager.Top != null)
                return;
            if (keyboard.escapeKey.wasPressedThisFrame)
                StartCoroutine(OpenSettingsNextFrame());
            else if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
                StartPve();
            else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
                StartPvp();
            else if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
                OpenBag();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else if (keyboard.f1Key.wasPressedThisFrame)
                PopupManager.Show<DevCheatPanel>();
#endif
        }

        private void OnDestroy()
        {
            pveHotspot.OnClick.RemoveListener(StartPve);
            pvpHotspot.OnClick.RemoveListener(StartPvp);
            bagHotspot.OnClick.RemoveListener(OpenBag);
            storageHotspot.OnClick.RemoveListener(OpenStorage);
            codexHotspot.OnClick.RemoveListener(OpenComingSoon);
            shopHotspot.OnClick.RemoveListener(OpenShop);
            sectHotspot.OnClick.RemoveListener(OpenSect);
            encyclopediaHotspot.OnClick.RemoveListener(OpenEncyclopedia);
            settingsButton.onClick.RemoveListener(OpenSettings);
            settingsButton.onClick.RemoveListener(PlayClick);
        }

        public void RefreshBattleAvailability()
        {
            // Battle hotspots intentionally remain clickable so validation can explain the problem.
            pveHotspot.SetLocked(false, 0);
            pvpHotspot.SetLocked(false, 0);
        }

        private void StartPve()
        {
            if (!ValidateBattleLoadout())
                return;
            PopupManager.Show<StageMapPopup>();
            OnboardingRuntime.Publish("stage_map_opened");
        }

        private void StartPvp() => TryStartGame(GameMode.Pvp);
        private void TryStartGame(GameMode mode)
        {
            if (!GameBootstrap.Session.TryValidateLoadout(out string warning))
            {
                ShowLoadoutWarning(warning);
                return;
            }

            GameBootstrap.Session.SelectedMode = mode;
            SceneFlow.Go(GameScene.Gameplay);
        }

        private static void OpenComingSoon()
        {
            PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", "Sắp ra mắt", "Đóng"));
        }

        private static bool ValidateBattleLoadout()
        {
            if (GameBootstrap.Session.TryValidateLoadout(out string warning))
                return true;
            ShowLoadoutWarning(warning);
            return false;
        }

        private static void ShowLoadoutWarning(string warning)
            => PopupManager.Show<MessagePopup>(new MessagePopupData("Không thể xuất chiêu",

            warning,
                "Đóng"));
        private void RefreshProgress()
        {
            GameSession session = GameBootstrap.Session;
            if (session == null || session.Profile == null)
                return;
            LevelService levels = new LevelService(LevelConfig.Load());
            displayedLevel = session.HeroLevel;
            displayedExp = session.Profile.HeroExp;
            levelText.text = displayedLevel.ToString();
            expBar.value = levels.Progress(session.Profile);
            if (expText != null)
                expText.text =
                    displayedLevel >= levels.MaxLevel
                    ? $"Cấp {displayedLevel} · EXP Tối đa"
                    : $"Cấp {displayedLevel} · EXP {UiFormat.Number(displayedExp)}/"
                        + UiFormat.Number(levels.ExpToNext(displayedLevel));
            sectHotspot.SetAvailableForLevel(displayedLevel);
            RefreshPower();
        }

        /// <summary>Refreshes the combat power displayed on Home.</summary>
        public void RefreshPower()
        {
            GameSession session = GameBootstrap.Session;
            if (powerText != null && session?.Profile != null)
                powerText.text = "Lực Chiến " + UiFormat.Number(session.PlayerPower());
        }

        public void OpenSect()
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            if (profile.HeroLevel < LevelService.SectUnlockLevel)
                PopupManager.Show<MessagePopup>(new MessagePopupData("Môn Phái",
                    "Đạt cấp 10 để gia nhập môn phái",
                    "Đóng"));
            else
                PopupManager.Show<SectSelectPopup>();
        }

        private static void OpenSettings() => PopupManager.Show<SettingsPopup>();

        // The settings icon is a plain Button without UiSfx.
        private static void PlayClick() => AudioManager.Instance?.Play("ui_click");
        private static void OpenEncyclopedia() => PopupManager.Show<EncyclopediaPopup>();
        public void OpenStorage()
        {
            WatchCurrencyPopup(PopupManager.Show<StoragePopup>());
            OnboardingRuntime.Publish("storage_opened");
        }

        public void OpenShop()
        {
            WatchCurrencyPopup(PopupManager.Show<ShopPopup>());
            OnboardingRuntime.Publish("shop_opened");
        }
        private static IEnumerator OpenSettingsNextFrame()
        {
            yield return null;
            if (PopupManager.Top == null)
                OpenSettings();
        }

        public void OpenBag()
        {
            BagPopup popup = PopupManager.Show<BagPopup>();
            OnboardingRuntime.Publish("bag_opened");
            if (popup != null)
                popup.Closed += HandleBagClosed;
        }

        private void HandleBagClosed(BasePopup popup)
        {
            popup.Closed -= HandleBagClosed;
            OnboardingRuntime.Publish("bag_closed");
            if (this == null)
                return; // Home was unloaded while the bag was still open.
            RefreshBattleAvailability();
            RefreshPower();
            questTracker?.Refresh();
        }

        private void WatchCurrencyPopup(BasePopup popup)
        {
            if (popup != null)
                popup.Closed += HandleCurrencyPopupClosed;
        }

        private void HandleCurrencyPopupClosed(BasePopup popup)
        {
            popup.Closed -= HandleCurrencyPopupClosed;
            if (this == null)
                return;
            RefreshCurrencies();
            RefreshPower();
            questTracker?.Refresh();
        }

        public void RefreshQuestAndResources()
        {
            RefreshCurrencies();
            RefreshProgress();
            questTracker?.Refresh();
        }

        private void CreateQuestTracker()
        {
            QuestTracker prefab = Resources.Load<QuestTracker>("UI/Home/QuestTracker");
            // HomeController sits on a root object outside any Canvas; the HUD's canvas is found from its texts.
            Canvas canvas = heroName != null && heroName.canvas != null ? heroName.canvas.rootCanvas : null;
            if (prefab == null || canvas == null)
            {
                Debug.LogWarning("[Quest] Tracker not created: prefab="
                    + (prefab != null)
                    + ", canvas="
                    + (canvas != null));
                return;
            }

            questTracker = Instantiate(prefab, canvas.transform, false);
            questTracker.Initialize(this);
        }

        private void RefreshCurrencies()
        {
            GameSession session = GameBootstrap.Session;
            if (session == null || session.Profile == null)
                return;
            session.Profile.Currencies.TryGetValue("bac", out long silver);
            session.Profile.Currencies.TryGetValue("nguyen_bao", out long premium);
            session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            int staminaMax = staminaConfig != null ? staminaConfig.Max : StaminaConfig.Load().Max;
            string staminaValue = staminaService != null
                ? BuildStaminaLabel(session.Profile.Pve, DateTimeOffset.UtcNow)
                : UiFormat.Number(session.Profile.Pve.Stamina) + "/" + UiFormat.Number(staminaMax);
            SetCurrency(staminaText, "icon_stamina", staminaValue);
            SetCurrency(silverText, "icon_bac", UiFormat.Number(silver));
            SetCurrency(premiumText, "icon_nguyen_bao", UiFormat.Number(premium));
            SetCurrency(reputationText, "icon_danh_vong", UiFormat.Number(reputation));
        }

        private void RefreshStamina()
        {
            GameSession session = GameBootstrap.Session;
            if (session?.Profile == null || staminaService == null)
                return;
            int previous = session.Profile.Pve.Stamina;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            int current = staminaService.Refresh(session.Profile.Pve, now);
            SetCurrency(staminaText, "icon_stamina", BuildStaminaLabel(session.Profile.Pve, now));
            if (current != previous)
                session.SaveProfile();
        }

        private string BuildStaminaTooltip()
        {
            if (staminaService == null || GameBootstrap.Session?.Profile == null)
                return string.Empty;
            PveProfileState state = GameBootstrap.Session.Profile.Pve;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            int minutesPerPoint = Math.Max(1, (int)Math.Round(1d / staminaConfig.RegenPerMinute));
            string header = $"Thể Lực: {state.Stamina}/{staminaConfig.Max}";
            string rate = $"Hồi 1 điểm mỗi {minutesPerPoint} phút";
            if (state.Stamina >= staminaConfig.Max)
                return header + "\n" + rate + "\nĐã đầy";
            string next = StaminaService.FormatDuration(staminaService.SecondsToNext(state, now));
            string full = StaminaService.FormatDuration(staminaService.SecondsToFull(state, now));
            return header + "\n" + rate + "\n+1 sau " + next + "\nĐầy sau " + full;
        }

        private string BuildStaminaLabel(PveProfileState state, DateTimeOffset now)
        {
            string value = UiFormat.Number(state.Stamina) + "/" + UiFormat.Number(staminaConfig.Max);
            if (state.Stamina >= staminaConfig.Max)
                return value;
            string next = StaminaService.FormatDuration(staminaService.SecondsToNext(state, now));
            return value + "  " + next;
        }

        private static void SetCurrency(Text label, string iconKey, string value)
        {
            if (label == null)
                return;
            Transform iconTransform = label.transform.parent.Find("Icon");
            Image icon = iconTransform != null ? iconTransform.GetComponent<Image>() : null;
            if (icon != null)
            {
                icon.sprite = CurrencyArt.TryGetSprite(iconKey, out Sprite sprite) ? sprite : null;
                icon.gameObject.SetActive(icon.sprite != null);
            }
            label.text = value;
        }
    }
}
