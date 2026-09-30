using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OneBagJianghu.Bag
{
    public sealed class SectInfo
    {
        public string Id { get; }
        public string Name { get; }
        public string TamPhap { get; }
        public IReadOnlyList<string> WeaponClasses { get; }

        public SectInfo(string id, string name, string tamPhap, IReadOnlyList<string> weaponClasses)
        {
            Id = id;
            Name = name;
            TamPhap = tamPhap;
            WeaponClasses = weaponClasses;
        }

        public bool Buffs(string weaponClass)
        {
            if (string.IsNullOrEmpty(weaponClass))
                return false;
            for (int i = 0; i < WeaponClasses.Count; i++)
            {
                if (string.Equals(WeaponClasses[i], weaponClass, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }
    }

    public sealed class SectCatalog
    {
        private readonly List<SectInfo> all;
        private readonly Dictionary<string, SectInfo> byId;

        private SectCatalog(List<SectInfo> sects)
        {
            all = sects;
            byId = new Dictionary<string, SectInfo>(StringComparer.Ordinal);
            foreach (SectInfo sect in sects)
            {
                if (string.IsNullOrWhiteSpace(sect.Id))
                    throw new FormatException("A sect definition has no id.");
                if (!byId.TryAdd(sect.Id, sect))
                    throw new FormatException($"Duplicate sect id '{sect.Id}'.");
            }
        }

        public IReadOnlyList<SectInfo> All => all;

        public static SectCatalog Parse(string json)
        {
            List<SectDto> data = JsonConvert.DeserializeObject<List<SectDto>>(json)
                ?? throw new FormatException("Sect catalog JSON is null.");
            var sects = new List<SectInfo>(data.Count);
            foreach (SectDto dto in data)
            {
                sects.Add(new SectInfo(dto.Id, dto.Name, dto.TamPhap,
                    dto.WeaponClasses ?? Array.Empty<string>()));
            }
            return new SectCatalog(sects);
        }

        public static SectCatalog LoadFromResources()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/sects");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/sects.json.");
            return Parse(asset.text);
        }

        public bool TryGet(string id, out SectInfo sect)
        {
            if (id != null)
                return byId.TryGetValue(id, out sect);
            sect = null;
            return false;
        }

        private sealed class SectDto
        {
            [JsonProperty("id")] public string Id;
            [JsonProperty("name")] public string Name;
            [JsonProperty("tamPhap")] public string TamPhap;
            [JsonProperty("weaponClasses")] public string[] WeaponClasses;
        }
    }
}
