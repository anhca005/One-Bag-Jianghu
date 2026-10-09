using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Core
{
    public interface ISettingsStore
    {
        float GetFloat(string key, float defaultValue);
        int GetInt(string key, int defaultValue);
        bool GetBool(string key, bool defaultValue);
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetBool(string key, bool value);
        void Save();
    }

    public sealed class PlayerPrefsSettingsStore : ISettingsStore
    {
        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
        public bool GetBool(string key, bool defaultValue) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public void Save() => PlayerPrefs.Save();
    }

    public sealed class MemorySettingsStore : ISettingsStore
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        public float GetFloat(string key, float defaultValue)
            => values.TryGetValue(key, out object value) ? (float)value : defaultValue;
        public int GetInt(string key, int defaultValue)
            => values.TryGetValue(key, out object value) ? (int)value : defaultValue;
        public bool GetBool(string key, bool defaultValue)
            => values.TryGetValue(key, out object value) ? (bool)value : defaultValue;
        public void SetFloat(string key, float value) => values[key] = value;
        public void SetInt(string key, int value) => values[key] = value;
        public void SetBool(string key, bool value) => values[key] = value;
        public void Save()
        {
        }
    }

    public sealed class GameSettings
    {
        private const string Prefix = "OneBagJianghu.Settings.";
        private static GameSettings current;
        private readonly ISettingsStore store;
        private float masterVolume;
        private float musicVolume;
        private float sfxVolume;
        private bool muted;
        private bool fullscreen;
        private int resolutionWidth;
        private int resolutionHeight;
        private bool vSync;
        private int frameRateLimit;

        private GameSettings(ISettingsStore store)
        {
            this.store = store;
        }

        public static GameSettings Current
            => current ??= Load(new PlayerPrefsSettingsStore());

        public float MasterVolume
        {
            get => masterVolume;
            set => Set(ref masterVolume, Mathf.Clamp01(value));
        }

        public float MusicVolume
        {
            get => musicVolume;
            set => Set(ref musicVolume, Mathf.Clamp01(value));
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set => Set(ref sfxVolume, Mathf.Clamp01(value));
        }

        public bool Muted
        {
            get => muted;
            set => Set(ref muted, value);
        }

        public bool Fullscreen
        {
            get => fullscreen;
            set => Set(ref fullscreen, value);
        }

        public int ResolutionWidth
        {
            get => resolutionWidth;
            set => Set(ref resolutionWidth, Mathf.Max(0, value));
        }

        public int ResolutionHeight
        {
            get => resolutionHeight;
            set => Set(ref resolutionHeight, Mathf.Max(0, value));
        }

        public bool VSync
        {
            get => vSync;
            set => Set(ref vSync, value);
        }

        public int FrameRateLimit
        {
            get => frameRateLimit;
            set => Set(ref frameRateLimit, NormalizeFrameRate(value));
        }

        public float EffectiveMusicVolume => Muted ? 0f : MasterVolume * MusicVolume;
        public float EffectiveSfxVolume => Muted ? 0f : MasterVolume * SfxVolume;
        public event Action Changed;

        public static GameSettings Load(ISettingsStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            var settings = new GameSettings(store);
            settings.masterVolume = Mathf.Clamp01(store.GetFloat(Prefix + "MasterVolume", 1f));
            settings.musicVolume = Mathf.Clamp01(store.GetFloat(Prefix + "MusicVolume", .8f));
            settings.sfxVolume = Mathf.Clamp01(store.GetFloat(Prefix + "SfxVolume", 1f));
            settings.muted = store.GetBool(Prefix + "Muted", false);
            settings.fullscreen = store.GetBool(Prefix + "Fullscreen", true);
            settings.resolutionWidth = Mathf.Max(0, store.GetInt(Prefix + "ResolutionWidth", 0));
            settings.resolutionHeight = Mathf.Max(0, store.GetInt(Prefix + "ResolutionHeight", 0));
            settings.vSync = store.GetBool(Prefix + "VSync", true);
            settings.frameRateLimit = NormalizeFrameRate(store.GetInt(Prefix + "FrameRateLimit", 60));
            return settings;
        }

        public void Save()
        {
            store.SetFloat(Prefix + "MasterVolume", MasterVolume);
            store.SetFloat(Prefix + "MusicVolume", MusicVolume);
            store.SetFloat(Prefix + "SfxVolume", SfxVolume);
            store.SetBool(Prefix + "Muted", Muted);
            store.SetBool(Prefix + "Fullscreen", Fullscreen);
            store.SetInt(Prefix + "ResolutionWidth", ResolutionWidth);
            store.SetInt(Prefix + "ResolutionHeight", ResolutionHeight);
            store.SetBool(Prefix + "VSync", VSync);
            store.SetInt(Prefix + "FrameRateLimit", FrameRateLimit);
            store.Save();
        }

        public void ResetToDefaults()
        {
            masterVolume = 1f;
            musicVolume = .8f;
            sfxVolume = 1f;
            muted = false;
            fullscreen = true;
            resolutionWidth = 0;
            resolutionHeight = 0;
            vSync = true;
            frameRateLimit = 60;
            Changed?.Invoke();
        }

        public void Apply()
        {
            AudioListener.volume = Muted ? 0f : MasterVolume;
            QualitySettings.vSyncCount = VSync ? 1 : 0;
            Application.targetFrameRate = VSync ? -1 : FrameRateLimit;
            if (Fullscreen)
            {
                // Fullscreen always uses the display's own resolution: a scaled borderless window
                // renders with a wrong gamma (yellowed colors). The chosen resolution applies to windowed mode.
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight,
                    FullScreenMode.FullScreenWindow);
                return;
            }

            int width = ResolutionWidth == 0 ? Screen.width : ResolutionWidth;
            int height = ResolutionWidth == 0 ? Screen.height : ResolutionHeight;
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }

        private static int NormalizeFrameRate(int value)
        {
            return value == 30 || value == 60 || value == 120 || value == -1 ? value : 60;
        }

        private void Set<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            Changed?.Invoke();
        }
    }
}
