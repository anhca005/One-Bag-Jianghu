using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Bag
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/Item Icon Library")]
    public sealed class ItemIconLibrary : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string ItemId;
            public Sprite Sprite;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();
        public List<Entry> Entries => entries;

        public static ItemIconLibrary Load()
        {
            return Resources.Load<ItemIconLibrary>("ItemIconLibrary");
        }

        public bool TryGet(string itemId, out Sprite sprite)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    Entry entry = entries[i];
                    if (entry != null && entry.ItemId == itemId && entry.Sprite != null)
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