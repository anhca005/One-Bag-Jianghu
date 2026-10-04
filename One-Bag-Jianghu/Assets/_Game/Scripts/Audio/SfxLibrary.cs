using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Audio
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/Audio/SFX Library")]
    public sealed class SfxLibrary : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string Key;
            public AudioClip[] Variants = Array.Empty<AudioClip>();
            [Range(0f, 1f)]
            public float BaseVolume = 1f;
            [Range(0f, .5f)]
            public float PitchJitter = .05f;
            [Min(0f)]
            public float MinimumInterval = .08f;
            [Min(0f)]
            public float CatalogDuration;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();
        private Dictionary<string, Entry> byKey;

        public List<Entry> Entries => entries;

        public bool TryGet(string key, out Entry entry)
        {
            EnsureIndex();
            entry = null;
            return !string.IsNullOrEmpty(key) && byKey.TryGetValue(key, out entry);
        }

        private void OnEnable()
        {
            byKey = null;
        }

        private void EnsureIndex()
        {
            if (byKey != null)
                return;
            byKey = new Dictionary<string, Entry>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Count; i++)
            {
                Entry entry = entries[i];
                if (entry != null && !string.IsNullOrEmpty(entry.Key))
                {
                    byKey[entry.Key] = entry;
                }
            }
        }
    }
}
