using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/PvE Art Library")]
    public sealed class PveArtLibrary : ScriptableObject
    {
        private const int ArenaAvatarCount = 4;

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

        /// <summary>Resolves requested enemy art, then a stable generic portrait so UI never shows a blank tile.</summary>
        public Sprite ResolveEnemyPortrait(string requestedKey, string stableId)
        {
            if (TryGet(requestedKey, out Sprite sprite))
            {
                return sprite;
            }

            int variant = StableVariant(stableId ?? requestedKey, 3) + 1;
            return TryGet("enemy_portrait_" + variant, out sprite) ? sprite : null;
        }

        /// <summary>Picks one of the arena opponent avatars, falling back to the generic enemy portraits.</summary>
        public Sprite ResolveArenaPortrait(string stableId)
        {
            int variant = StableVariant(stableId, ArenaAvatarCount) + 1;
            if (TryGet("pvp_avatar_" + variant, out Sprite sprite))
            {
                return sprite;
            }

            return ResolveEnemyPortrait(null, stableId);
        }

        public static int StableVariant(string value, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int hash = 17;
            string source = value ?? string.Empty;
            for (int i = 0; i < source.Length; i++)
            {
                hash = unchecked(hash * 31 + source[i]);
            }

            return (hash & int.MaxValue) % count;
        }
    }
}
