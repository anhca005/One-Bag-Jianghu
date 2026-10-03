using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Battle.Replay.View
{
    [CreateAssetMenu(menuName = "One Bag Jianghu/VFX Tuning", fileName = "VfxTuning")]
    public sealed class VfxTuning : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string key;
            [Range(-180f, 180f)]
            public float rotation;
            [Range(.2f, 4f)]
            public float scale = 1f;
            public Vector2 offset;
            public bool flipX;
            public bool flipY;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();
        private static VfxTuning cached;
        public static VfxTuning Load()
        {
            if (cached == null)
            {
                cached = Resources.Load<VfxTuning>("VfxTuning");
                if (cached == null)
                {
                    cached = CreateInstance<VfxTuning>();
                    cached.hideFlags = HideFlags.HideAndDontSave;
                }
            }

            return cached;
        }

        public bool TryGet(string key, out Entry entry)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].key, key, StringComparison.Ordinal))
                {
                    entry = entries[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }

        public Entry GetOrCreate(string key)
        {
            if (TryGet(key, out Entry entry))
                return entry;
            entry = new Entry
            {
                key = key
            };
            entries.Add(entry);
            return entry;
        }

        public void Reset(string key)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (string.Equals(entries[i].key, key, StringComparison.Ordinal))
                {
                    entries.RemoveAt(i);
                }
            }
        }

        public static bool IsIdentity(Entry entry)
        {
            return entry == null ||
(Mathf.Approximately(entry.rotation,
0f) &&
Mathf.Approximately(entry.scale,
1f) &&
entry.offset == Vector2.zero &&
!entry.flipX &&
!entry.flipY);
        }

        public static VfxTransform Compose(Entry entry, bool mirrored)
        {
            float scale = entry == null ? 1f : Mathf.Clamp(entry.scale, .2f, 4f);
            float x = scale * (entry != null && entry.flipX ? -1f : 1f);
            float y = scale * (entry != null && entry.flipY ? -1f : 1f);
            float rotation = entry == null ? 0f : Mathf.Clamp(entry.rotation, -180f, 180f);
            Vector2 offset = entry == null ? Vector2.zero : entry.offset;
            if (mirrored)
            {
                x = -x;
                rotation = -rotation;
                offset.x = -offset.x;
            }

            return new VfxTransform(new Vector3(x, y, 1f), rotation, offset);
        }
    }

    public readonly struct VfxTransform
    {
        public VfxTransform(Vector3 localScale, float rotation, Vector2 offset)
        {
            LocalScale = localScale;
            Rotation = rotation;
            Offset = offset;
        }

        public Vector3 LocalScale { get; }
        public float Rotation { get; }
        public Vector2 Offset { get; }
    }
}