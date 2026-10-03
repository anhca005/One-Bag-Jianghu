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
        private Text staminaText;
        [SerializeField]
        private Text silverText;
        [SerializeField]
        private Text premiumText;
        [SerializeField]
        private Text reputationText;
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
        private int displayedLevel = -1;
        private long displayedExp = -1;
        private void Awake()
        {
            pveHotspot.OnClick.AddListener(StartPve);
            pvpHotspot.OnClick.AddListener(StartPvp);
            bagHotspot.OnClick.AddListener(OpenBag);
            storageHotspot.OnClick.AddListener(OpenStorage);
            codexHotspot.OnClick.AddListener(OpenComingSoon);
            shopHotspot.OnClick.AddListener(OpenShop);
            sectHotspot.OnClick.AddListener(OpenSect);
            settingsButton.onClick.AddListener(OpenSettings);
        }

        private void Start()
        {
            GameSession session = GameBootstrap.Session;
            session.Profile.Currencies.TryGetValue("bac", out long silver);
            session.Profile.Currencies.TryGetValue("nguyen_bao", out long premium);
            session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            bool staminaArt = CurrencyArt.Apply(staminaText,
"icon_stamina"), silverArt = CurrencyArt.Apply(silverText,
"icon_bac"), premiumArt = CurrencyArt.Apply(premiumText,
"icon_nguyen_bao"), reputationArt = CurrencyArt.Apply(reputationText,
"icon_danh_vong");
            if (staminaText != null)
                staminaText.text = "       Thể Lực " + session.Profile.Pve.Stamina.ToString("N0");
            if (silverText != null)
                silverText.text = "       Bạc " + silver.ToString("N0");
            if (premiumText != null)
                premiumText.text = "       Nguyên Bảo " + premium.ToString("N0");
            if (reputationText != null)
                reputationText.text = "       Danh Vọng " + reputation.ToString("N0");
            if (staminaText != null)
                staminaText.text = staminaText.text.TrimStart();
            if (silverText != null)
                silverText.text = silverText.text.TrimStart();
            if (premiumText != null)
                premiumText.text = premiumText.text.TrimStart();
            if (reputationText != null)
                reputationText.text = reputationText.text.TrimStart();
            heroName.text = session.HeroName;
            RefreshProgress();
            RefreshBattleAvailability();
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
                PopupManager.Show<MessagePopup>(new MessagePopupData("Mở khóa Môn Phái",
"Bạn đã đạt cấp 10. Hãy đến điểm Môn Phái trên trang chủ để gia nhập.",
"Đã hiểu"));
            }
        }

        private void Update()
        {
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
            settingsButton.onClick.RemoveListener(OpenSettings);
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
            levelText.text = $"Cấp {displayedLevel}";
            expBar.value = levels.Progress(session.Profile);
            if (expText != null)
                expText.text =
                    displayedLevel >= levels.MaxLevel
                    ? "EXP Tối đa"
                    : $"EXP {displayedExp:N0}/{levels.ExpToNext(displayedLevel):N0}";
            sectHotspot.SetAvailableForLevel(displayedLevel);
        }

        public void OpenSect()
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            if (profile.HeroLevel < LevelService.SectUnlockLevel)
                PopupManager.Show<MessagePopup>(new MessagePopupData("Môn Phái",
"Đạt cấp 10 để gia nhập môn phái",
"Đóng"));
            else if (!string.IsNullOrWhiteSpace(profile.SectId))
                PopupManager.Show<MessagePopup>(new MessagePopupData("Môn Phái",
"Bạn đã gia nhập môn phái. Lựa chọn hiện tại là vĩnh viễn.",
"Đóng"));
            else
                PopupManager.Show<SectSelectPopup>();
        }

        private static void OpenSettings() => PopupManager.Show<SettingsPopup>();
        private static void OpenStorage() => PopupManager.Show<StoragePopup>();
        private static void OpenShop() => PopupManager.Show<ShopPopup>();
        private static IEnumerator OpenSettingsNextFrame()
        {
            yield return null;
            if (PopupManager.Top == null)
                OpenSettings();
        }

        private void OpenBag()
        {
            BagPopup popup = PopupManager.Show<BagPopup>();
            if (popup != null)
                popup.Closed += HandleBagClosed;
        }

        private void HandleBagClosed(BasePopup popup)
        {
            popup.Closed -= HandleBagClosed;
            if (this == null)
                return; // Home was unloaded while the bag was still open.
            RefreshBattleAvailability();
        }
    }
}