using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Bag
{
    // Lives in its own file: Unity binds a ScriptableObject asset to its script only when the names match.
    [CreateAssetMenu(menuName = "One Bag Jianghu/Sect Art Library")]
    public sealed class SectArtLibrary : ScriptableObject
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

        public static SectArtLibrary Load()
        {
            return Resources.Load<SectArtLibrary>("SectArtLibrary");
        }

        public bool TryGet(string key, out Sprite sprite)
        {
            if (!string.IsNullOrEmpty(key))
            {
                foreach (Entry entry in entries)
                {
                    if (entry != null && entry.Key == key && entry.Sprite != null)
                    {
                        sprite = entry.Sprite;
                        return true;
                    }
                }
            }

            sprite = null;
            return false;
        }
    }
}
