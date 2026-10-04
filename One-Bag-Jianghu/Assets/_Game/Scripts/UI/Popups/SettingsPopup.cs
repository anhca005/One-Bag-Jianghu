using System.Collections.Generic;
using System.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OneBagJianghu.UI.Popups
{
    public sealed class SettingsPopup : BasePopup
    {
        private static readonly int[] FrameRates = { 30, 60, 120, -1 };
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Text masterVolumeText;
        [SerializeField] private Text musicVolumeText;
        [SerializeField] private Text sfxVolumeText;
        [SerializeField] private Toggle mutedToggle;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Dropdown frameRateDropdown;
        [SerializeField] private Text heroNameText;
        [SerializeField] private UiButton loginButton;
        [SerializeField] private UiButton quitButton;
        [SerializeField] private UiButton defaultsButton;
        [SerializeField] private UiButton closeButton;
        private readonly List<Vector2Int> resolutions = new List<Vector2Int>();
        private GameSettings settings;

        protected override void Awake()
        {
            base.Awake();
            AddCreditsLine();
            masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolume);
            mutedToggle.onValueChanged.AddListener(HandleMuted);
            fullscreenToggle.onValueChanged.AddListener(HandleFullscreen);
            resolutionDropdown.onValueChanged.AddListener(HandleResolution);
            vSyncToggle.onValueChanged.AddListener(HandleVSync);
            frameRateDropdown.onValueChanged.AddListener(HandleFrameRate);
            loginButton.onClick.AddListener(ConfirmLogin);
            quitButton.onClick.AddListener(ConfirmQuit);
            defaultsButton.onClick.AddListener(ResetDefaults);
            closeButton.onClick.AddListener(Close);
        }

        private void AddCreditsLine()
        {
            Text credits = Instantiate(heroNameText, panel);
            credits.name = "AudioCredits";
            credits.text = "Nhạc: Richard Guo · Tiếng động: Kenney (CC0)";
            credits.fontSize = Mathf.Min(heroNameText.fontSize, 14);
            credits.alignment = TextAnchor.LowerCenter;
            credits.raycastTarget = false;
            RectTransform rect = credits.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 8f);
            rect.sizeDelta = new Vector2(-32f, 24f);
        }

        protected override void OnSetup(object data)
        {
            settings = GameSettings.Current;
            BuildResolutionOptions();
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(new List<string> { "30", "60", "120", "Không giới hạn" });
            RefreshControls();
            if (GameBootstrap.Session != null)
                heroNameText.text = GameBootstrap.Session.HeroName;
        }

        private void OnDestroy()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolume);
            musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolume);
            sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolume);
            mutedToggle.onValueChanged.RemoveListener(HandleMuted);
            fullscreenToggle.onValueChanged.RemoveListener(HandleFullscreen);
            resolutionDropdown.onValueChanged.RemoveListener(HandleResolution);
            vSyncToggle.onValueChanged.RemoveListener(HandleVSync);
            frameRateDropdown.onValueChanged.RemoveListener(HandleFrameRate);
            loginButton.onClick.RemoveListener(ConfirmLogin);
            quitButton.onClick.RemoveListener(ConfirmQuit);
            defaultsButton.onClick.RemoveListener(ResetDefaults);
            closeButton.onClick.RemoveListener(Close);
        }

        private void BuildResolutionOptions()
        {
            resolutions.Clear();
            resolutions.AddRange(Screen.resolutions
                .Where(value => value.width >= 1280 && value.height >= 720)
                .Select(value => new Vector2Int(value.width, value.height))
                .Distinct()
                .OrderByDescending(value => value.x * value.y)
                .ThenByDescending(value => value.x));
            if (resolutions.Count == 0)
                resolutions.Add(new Vector2Int(Screen.width, Screen.height));
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutions
                .Select(value => value.x + " × " + value.y)
                .ToList());
        }

        private void RefreshControls()
        {
            masterVolumeSlider.SetValueWithoutNotify(settings.MasterVolume);
            musicVolumeSlider.SetValueWithoutNotify(settings.MusicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(settings.SfxVolume);
            mutedToggle.SetIsOnWithoutNotify(settings.Muted);
            fullscreenToggle.SetIsOnWithoutNotify(settings.Fullscreen);
            vSyncToggle.SetIsOnWithoutNotify(settings.VSync);
            resolutionDropdown.SetValueWithoutNotify(FindResolutionIndex());
            frameRateDropdown.SetValueWithoutNotify(System.Array.IndexOf(FrameRates, settings.FrameRateLimit));
            masterVolumeText.text = FormatPercent(settings.MasterVolume);
            musicVolumeText.text = FormatPercent(settings.MusicVolume);
            sfxVolumeText.text = FormatPercent(settings.SfxVolume);
            SetSliderAlpha(settings.Muted ? .5f : 1f);
            frameRateDropdown.interactable = !settings.VSync;
        }

        private int FindResolutionIndex()
        {
            int width = settings.ResolutionWidth == 0 ? Screen.width : settings.ResolutionWidth;
            int height = settings.ResolutionWidth == 0 ? Screen.height : settings.ResolutionHeight;
            int exact = resolutions.FindIndex(value => value.x == width && value.y == height);
            if (exact >= 0)
                return exact;
            long bestDistance = long.MaxValue;
            int bestIndex = 0;
            for (int i = 0; i < resolutions.Count; i++)
            {
                long deltaWidth = resolutions[i].x - width;
                long deltaHeight = resolutions[i].y - height;
                long distance = deltaWidth * deltaWidth + deltaHeight * deltaHeight;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static string FormatPercent(float value) => Mathf.RoundToInt(value * 100f) + "%";

        private void SetSliderAlpha(float alpha)
        {
            SetAlpha(masterVolumeSlider, alpha);
            SetAlpha(musicVolumeSlider, alpha);
            SetAlpha(sfxVolumeSlider, alpha);
        }

        private static void SetAlpha(Slider slider, float alpha)
        {
            CanvasGroup group = slider.GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = alpha;
        }

        private void HandleMasterVolume(float value)
        {
            settings.MasterVolume = value;
            Commit();
        }

        private void HandleMusicVolume(float value)
        {
            settings.MusicVolume = value;
            Commit();
        }

        private void HandleSfxVolume(float value)
        {
            settings.SfxVolume = value;
            Commit();
        }

        private void HandleMuted(bool value)
        {
            settings.Muted = value;
            Commit();
        }

        private void HandleFullscreen(bool value)
        {
            settings.Fullscreen = value;
            Commit();
        }

        private void HandleResolution(int index)
        {
            if (index < 0 || index >= resolutions.Count)
                return;
            settings.ResolutionWidth = resolutions[index].x;
            settings.ResolutionHeight = resolutions[index].y;
            Commit();
        }

        private void HandleVSync(bool value)
        {
            settings.VSync = value;
            Commit();
        }

        private void HandleFrameRate(int index)
        {
            if (index < 0 || index >= FrameRates.Length)
                return;
            settings.FrameRateLimit = FrameRates[index];
            Commit();
        }

        private void Commit()
        {
            settings.Apply();
            settings.Save();
            RefreshControls();
        }

        private void ResetDefaults()
        {
            settings.ResetToDefaults();
            Commit();
        }


        private void ConfirmLogin()
        {
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData(
                "Đăng xuất",
                "Tiến trình đã được lưu. Bạn muốn đăng xuất?",
                "Đồng ý",
                "Hủy",
                HandleLoginConfirmation));
        }

        private void HandleLoginConfirmation(bool confirmed)
        {
            if (!confirmed)
                return;
            GameBootstrap.Session?.SaveProfile();
            Close();
            SceneFlow.Go(GameScene.Login);
        }

        private void ConfirmQuit()
        {
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData(
                "Thoát game",
                "Bạn muốn rời giang hồ?",
                "Thoát",
                "Hủy",
                HandleQuitConfirmation));
        }

        private static void HandleQuitConfirmation(bool confirmed)
        {
            if (!confirmed)
                return;
            GameBootstrap.Session?.SaveProfile();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
