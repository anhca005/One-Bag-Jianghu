using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/PvE Art Library")]
    public sealed class PveArtLibrary : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string Key;
            public Sprite Sprite;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();
        public List<Entry> Entries => entries;

        public static PveArtLibrary Load() => Resources.Load<PveArtLibrary>("PveArtLibrary");
        public bool TryGet(string key, out Sprite sprite)
        {
            if (!string.IsNullOrEmpty(key))
                foreach (Entry e in entries)
                    if (e != null && e.Key == key && e.Sprite != null)
                    {
                        sprite = e.Sprite;
                        return true;
                    }

            sprite = null;
            return false;
        }
    }
}