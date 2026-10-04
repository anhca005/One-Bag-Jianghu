using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OneBagJianghu.Audio
{
    public sealed class SfxCatalogRecord
    {
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("event")]
        public string Event;
        [JsonProperty("durationSec")]
        public float DurationSec;
        [JsonProperty("volumeDb")]
        public float? VolumeDb;
    }

    public sealed class ItemSfxMap
    {
        [JsonProperty("attackKey")]
        public string AttackKey;
        [JsonProperty("hitKey")]
        public string HitKey;
        [JsonProperty("ultimateKey")]
        public string UltimateKey;
        [JsonProperty("statusKey")]
        public string StatusKey;
    }

    public sealed class SfxMapDocument
    {
        [JsonProperty("items")]
        public Dictionary<string, ItemSfxMap> Items = new Dictionary<string, ItemSfxMap>();
    }

    public static class SfxData
    {
        public static SfxMapDocument LoadMap()
        {
            return ParseMap(LoadText("Data/Audio/sfx-map"));
        }

        public static List<SfxCatalogRecord> LoadCatalog()
        {
            return ParseCatalog(LoadText("Data/Audio/sfx-catalog"));
        }

        public static SfxMapDocument ParseMap(string json)
        {
            return JsonConvert.DeserializeObject<SfxMapDocument>(json) ??
                throw new FormatException("SFX map JSON is null.");
        }

        public static List<SfxCatalogRecord> ParseCatalog(string json)
        {
            return JsonConvert.DeserializeObject<List<SfxCatalogRecord>>(json) ??
                throw new FormatException("SFX catalog JSON is null.");
        }

        private static string LoadText(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null)
                throw new InvalidOperationException("Missing Resources/" + path + ".json.");
            return asset.text;
        }
    }
}
