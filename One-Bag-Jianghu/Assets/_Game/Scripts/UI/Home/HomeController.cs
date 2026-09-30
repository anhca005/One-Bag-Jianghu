using System.Collections;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Home
{
    public sealed class HomeController : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image heroZone;
        [SerializeField] private Text heroName;
        [SerializeField] private Slider expBar;
        [SerializeField] private Text levelText;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button pveButton;
        [SerializeField] private Button pvpButton;
        [SerializeField] private Text warningText;
        [SerializeField] private HomeHotspot pveHotspot;
        [SerializeField] private HomeHotspot pvpHotspot;
        [SerializeField] private HomeHotspot bagHotspot;
        [SerializeField] private HomeHotspot storageHotspot;
        [SerializeField] private HomeHotspot codexHotspot;
        [SerializeField] private HomeHotspot shopHotspot;
        [SerializeField] private HomeHotspot sectHotspot;

        private void Awake()
        {
            pveButton.onClick.AddListener(StartPve);
            pvpButton.onClick.AddListener(StartPvp);
            pveHotspot.OnClick.AddListener(StartPve);
            pvpHotspot.OnClick.AddListener(StartPvp);
            bagHotspot.OnClick.AddListener(OpenBag);
            storageHotspot.OnClick.AddListener(OpenStorage);
            codexHotspot.OnClick.AddListener(OpenComingSoon);
            shopHotspot.OnClick.AddListener(OpenShop);
            sectHotspot.OnClick.AddListener(OpenComingSoon);
            settingsButton.onClick.AddListener(OpenSettings);
        }

        private void Start()
        {
            GameSession session = GameBootstrap.Session;
            heroName.text = session.HeroName;
            levelText.text = $"Cấp {session.HeroLevel}";
            expBar.value = 0f;
            sectHotspot.SetAvailableForLevel(session.HeroLevel);
            RefreshBattleAvailability();
            SceneFlow.NotifyReady();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || PopupManager.Top != null) return;
            if (keyboard.escapeKey.wasPressedThisFrame) StartCoroutine(OpenSettingsNextFrame());
            else if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame) StartPve();
            else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame) StartPvp();
            else if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame) OpenBag();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else if (keyboard.f1Key.wasPressedThisFrame) PopupManager.Show<DevCheatPanel>();
#endif
        }

        private void OnDestroy()
        {
            pveButton.onClick.RemoveListener(StartPve);
            pvpButton.onClick.RemoveListener(StartPvp);
            pveHotspot.OnClick.RemoveListener(StartPve);
            pvpHotspot.OnClick.RemoveListener(StartPvp);
            bagHotspot.OnClick.RemoveListener(OpenBag);
            storageHotspot.OnClick.RemoveListener(OpenStorage);
            codexHotspot.OnClick.RemoveListener(OpenComingSoon);
            shopHotspot.OnClick.RemoveListener(OpenShop);
            sectHotspot.OnClick.RemoveListener(OpenComingSoon);
            settingsButton.onClick.RemoveListener(OpenSettings);
        }

        public void RefreshBattleAvailability()
        {
            bool valid = GameBootstrap.Session.TryValidateLoadout(out string warning);
            warningText.text = warning;
            warningText.gameObject.SetActive(!valid);
            pveButton.interactable = valid;
            pvpButton.interactable = valid;
        }

        private void StartPve() => TryStartGame(GameMode.Pve);
        private void StartPvp() => TryStartGame(GameMode.Pvp);

        private void TryStartGame(GameMode mode)
        {
            if (!GameBootstrap.Session.TryValidateLoadout(out string warning))
            {
                warningText.text = warning;
                warningText.gameObject.SetActive(true);
                return;
            }
            GameBootstrap.Session.SelectedMode = mode;
            SceneFlow.Go(GameScene.Gameplay);
        }

        private static void OpenComingSoon()
        {
            PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", "Sắp ra mắt", "Đóng"));
        }

        private static void OpenSettings() => PopupManager.Show<SettingsPopup>();
        private static void OpenStorage() => PopupManager.Show<StoragePopup>();
        private static void OpenShop() => PopupManager.Show<ShopPopup>();

        private static IEnumerator OpenSettingsNextFrame()
        {
            yield return null;
            if (PopupManager.Top == null) OpenSettings();
        }

        private void OpenBag()
        {
            BagPopup popup = PopupManager.Show<BagPopup>();
            if (popup != null) popup.Closed += HandleBagClosed;
        }

        private void HandleBagClosed(BasePopup popup)
        {
            popup.Closed -= HandleBagClosed;
            if (this == null) return; // Home was unloaded while the bag was still open.
            RefreshBattleAvailability();
        }
    }
}
