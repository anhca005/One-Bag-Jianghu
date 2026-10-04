using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Audio
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/Audio/Music Library")]
    public sealed class MusicLibrary : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string Key;
            public AudioClip Clip;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();
        private Dictionary<string, AudioClip> byKey;

        public List<Entry> Entries => entries;

        public bool TryGet(string key, out AudioClip clip)
        {
            EnsureIndex();
            clip = null;
            return !string.IsNullOrEmpty(key) && byKey.TryGetValue(key, out clip) && clip != null;
        }

        private void OnEnable()
        {
            byKey = null;
        }

        private void EnsureIndex()
        {
            if (byKey != null)
                return;
            byKey = new Dictionary<string, AudioClip>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Count; i++)
            {
                Entry entry = entries[i];
                if (entry != null && !string.IsNullOrEmpty(entry.Key))
                {
                    byKey[entry.Key] = entry.Clip;
                }
            }
        }
    }
}
