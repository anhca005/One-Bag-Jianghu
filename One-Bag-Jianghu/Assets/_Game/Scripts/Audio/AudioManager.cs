using System.Collections.Generic;
using OneBagJianghu.Core;
using UnityEngine;

namespace OneBagJianghu.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        private const int PoolSize = 12;
        private const int MaximumSameKey = 3;
        private static AudioManager instance;
        private readonly List<Voice> voices = new List<Voice>(PoolSize);
        private readonly Dictionary<string, float> lastPlayed = new Dictionary<string, float>();
        private readonly HashSet<string> warnedKeys = new HashSet<string>();
        private SfxLibrary library;
        // The button-type cues all use the same click. Pressing "Đóng" fires the button click and the popup close
        // cue in the same frame; play one click, not two.
        private const float ButtonCueWindow = .08f;
        private static readonly HashSet<string> ButtonCues = new HashSet<string>
        {
            "ui_click",
            "ui_open",
            "ui_close",
            "dialogue_next",
            "stage_select"
        };
        private float lastButtonCue = float.NegativeInfinity;
        private MusicPlayer musicPlayer;

        public static AudioManager Instance => instance;
        public float MusicVolume
        {
            get;
            private set;
        }

        public static AudioManager EnsureExists()
        {
            if (instance != null)
                return instance;
            GameObject root = new GameObject("AudioManager");
            return root.AddComponent<AudioManager>();
        }

        public void Play(string key)
        {
            Play(key, 1f);
        }

        public void Play(string key, float volumeScale)
        {
            if (!TrySelectClip(key, out SfxLibrary.Entry entry, out AudioClip clip))
                return;
            float now = Time.unscaledTime;
            if (lastPlayed.TryGetValue(key, out float last) && now - last < entry.MinimumInterval)
                return;
            if (CountPlaying(key) >= MaximumSameKey)
                return;
            bool buttonCue = ButtonCues.Contains(key);
            if (buttonCue && now - lastButtonCue < ButtonCueWindow)
                return;
            Voice voice = FindVoice();
            if (voice == null)
                return;
            lastPlayed[key] = now;
            if (buttonCue)
                lastButtonCue = now;
            voice.Key = key;
            voice.VolumeScale = entry.BaseVolume * Mathf.Max(0f, volumeScale);
            voice.Source.clip = clip;
            voice.Source.volume = voice.VolumeScale * GameSettings.Current.EffectiveSfxVolume;
            voice.Source.pitch = 1f + Random.Range(-entry.PitchJitter, entry.PitchJitter);
            voice.Source.Play();
        }

        public void PlayMusic(string key)
        {
            musicPlayer.PlayMusic(key);
        }

        public void PlaySting(string key)
        {
            musicPlayer.PlaySting(key);
        }

        public void StopMusic(float fadeDuration = 1f)
        {
            musicPlayer.StopMusic(fadeDuration);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            library = Resources.Load<SfxLibrary>("Audio/SfxLibrary");
            musicPlayer = gameObject.AddComponent<MusicPlayer>();
            musicPlayer.Initialize();
            for (int i = 0; i < PoolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                voices.Add(new Voice(source));
            }

            GameSettings.Current.Changed += RefreshVolumes;
            RefreshVolumes();
        }

        private void OnDestroy()
        {
            if (instance != this)
                return;
            GameSettings.Current.Changed -= RefreshVolumes;
            instance = null;
        }

        private void RefreshVolumes()
        {
            MusicVolume = GameSettings.Current.EffectiveMusicVolume;
            musicPlayer.SetVolume(MusicVolume);
            float sfxVolume = GameSettings.Current.EffectiveSfxVolume;
            for (int i = 0; i < voices.Count; i++)
            {
                if (voices[i].Source.isPlaying)
                {
                    voices[i].Source.volume = voices[i].VolumeScale * sfxVolume;
                }
            }
        }

        private bool TrySelectClip(string key, out SfxLibrary.Entry entry, out AudioClip clip)
        {
            entry = null;
            clip = null;
            if (library != null && library.TryGet(key, out entry) && entry.Variants != null &&
                entry.Variants.Length > 0)
            {
                clip = entry.Variants[Random.Range(0, entry.Variants.Length)];
            }

            if (clip != null)
                return true;
            string warningKey = string.IsNullOrEmpty(key) ? "<empty>" : key;
            if (warnedKeys.Add(warningKey))
            {
                Debug.LogWarning("[Audio] Unknown SFX key or no clips: " + warningKey);
            }

            return false;
        }

        private int CountPlaying(string key)
        {
            int count = 0;
            for (int i = 0; i < voices.Count; i++)
            {
                if (voices[i].Source.isPlaying && voices[i].Key == key)
                {
                    count++;
                }
            }

            return count;
        }

        private Voice FindVoice()
        {
            for (int i = 0; i < voices.Count; i++)
            {
                if (!voices[i].Source.isPlaying)
                    return voices[i];
            }

            return null;
        }

        private sealed class Voice
        {
            public Voice(AudioSource source)
            {
                Source = source;
            }

            public AudioSource Source { get; }
            public string Key
            {
                get;
                set;
            }
            public float VolumeScale
            {
                get;
                set;
            }
        }
    }
}
