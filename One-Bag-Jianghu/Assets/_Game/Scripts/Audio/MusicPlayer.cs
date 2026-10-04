using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Audio
{
    public sealed class MusicPlayer : MonoBehaviour
    {
        private const float DefaultCrossfadeDuration = 1f;
        private const float DuckScale = .2f;
        private readonly HashSet<string> warnedKeys = new HashSet<string>();
        private readonly float[] musicGains = { 0f, 0f };
        private AudioSource[] musicSources;
        private AudioSource stingSource;
        private MusicLibrary library;
        private Coroutine fadeRoutine;
        private Coroutine stingRoutine;
        private int activeSource;
        private string currentKey;
        private float musicVolume = 1f;
        private float duckScale = 1f;

        public string CurrentKey => currentKey;

        public void Initialize()
        {
            if (musicSources != null)
                return;
            library = Resources.Load<MusicLibrary>("Audio/MusicLibrary");
            musicSources = new[] { CreateSource(), CreateSource() };
            stingSource = CreateSource();
        }

        public void SetVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
        }

        public void PlayMusic(string key)
        {
            Initialize();
            if (key == currentKey && musicSources[activeSource].isPlaying)
                return;
            if (!TryGetClip(key, out AudioClip clip))
                return;
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(Crossfade(key, clip, DefaultCrossfadeDuration));
        }

        public void PlaySting(string key)
        {
            Initialize();
            if (!TryGetClip(key, out AudioClip clip))
                return;
            if (stingRoutine != null)
                StopCoroutine(stingRoutine);
            stingRoutine = StartCoroutine(PlayStingRoutine(clip));
        }

        public void StopMusic(float fadeDuration = DefaultCrossfadeDuration)
        {
            Initialize();
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOut(Mathf.Max(0f, fadeDuration)));
        }

        private AudioSource CreateSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private bool TryGetClip(string key, out AudioClip clip)
        {
            clip = null;
            if (library != null && library.TryGet(key, out clip))
                return true;
            string warningKey = string.IsNullOrEmpty(key) ? "<empty>" : key;
            if (warnedKeys.Add(warningKey))
            {
                Debug.LogWarning("[Audio] Unknown music key or no clip: " + warningKey);
            }

            return false;
        }

        private IEnumerator Crossfade(string key, AudioClip clip, float duration)
        {
            int previousIndex = activeSource;
            int nextIndex = 1 - activeSource;
            AudioSource previous = musicSources[previousIndex];
            AudioSource next = musicSources[nextIndex];
            next.clip = clip;
            next.loop = true;
            next.volume = 0f;
            musicGains[nextIndex] = 0f;
            next.Play();
            currentKey = key;
            activeSource = nextIndex;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                musicGains[nextIndex] = progress;
                musicGains[previousIndex] = 1f - progress;
                ApplyVolumes();
                yield return null;
            }

            previous.Stop();
            musicGains[previousIndex] = 0f;
            musicGains[nextIndex] = 1f;
            ApplyVolumes();
            fadeRoutine = null;
        }

        private IEnumerator FadeOut(float duration)
        {
            AudioSource source = musicSources[activeSource];
            float startVolume = source.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicGains[activeSource] = Mathf.Clamp01(1f - elapsed / duration);
                source.volume = startVolume * musicGains[activeSource];
                yield return null;
            }

            source.Stop();
            musicGains[activeSource] = 0f;
            currentKey = null;
            fadeRoutine = null;
        }

        private IEnumerator PlayStingRoutine(AudioClip clip)
        {
            duckScale = DuckScale;
            ApplyVolumes();
            stingSource.clip = clip;
            stingSource.loop = false;
            stingSource.volume = musicVolume;
            stingSource.Play();
            float elapsed = 0f;
            while (elapsed < clip.length)
            {
                elapsed += Time.unscaledDeltaTime;
                stingSource.volume = musicVolume;
                yield return null;
            }

            duckScale = 1f;
            ApplyVolumes();
            stingRoutine = null;
        }

        private void ApplyVolumes()
        {
            if (musicSources == null)
                return;
            for (int i = 0; i < musicSources.Length; i++)
            {
                if (musicSources[i].isPlaying)
                {
                    musicSources[i].volume = musicGains[i] * musicVolume * duckScale;
                }
            }

            if (stingSource.isPlaying)
            {
                stingSource.volume = musicVolume;
            }
        }
    }
}
