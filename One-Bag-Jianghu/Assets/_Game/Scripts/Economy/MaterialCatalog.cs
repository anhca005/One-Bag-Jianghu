using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public sealed class MaterialInfo
    {
        public string Id
        {
            get;
        }
        public string Name
        {
            get;
        }
        public string ItemType
        {
            get;
        }
        public string Band
        {
            get;
        }
        public string Description
        {
            get;
        }
        public int Tier
        {
            get;
        }
        public bool IsCatalyst
        {
            get;
        }
        public bool PvpOnly
        {
            get;
        }

        public MaterialInfo(string id,
string name,
string itemType,
string band,
string description,
int tier = 0,
bool isCatalyst = false,
bool pvpOnly = false)
        {
            Id = id;
            Name = name;
            ItemType = itemType;
            Band = band;
            Description = description;
            Tier = tier;
            IsCatalyst = isCatalyst;
            PvpOnly = pvpOnly;
        }
    }

    public sealed class MaterialCatalog
    {
        private readonly List<MaterialInfo> all;
        private readonly Dictionary<string, MaterialInfo> byId;
        private MaterialCatalog(List<MaterialInfo> materials)
        {
            all = materials;
            byId = new Dictionary<string, MaterialInfo>(StringComparer.Ordinal);
            foreach (MaterialInfo material in materials)
            {
                if (string.IsNullOrWhiteSpace(material.Id))
                    throw new FormatException("A material definition has no id.");
                if (!byId.TryAdd(material.Id, material))
                    throw new FormatException($"Duplicate material id '{material.Id}'.");
                if (string.IsNullOrWhiteSpace(material.Name))
                    throw new FormatException($"Material '{material.Id}' has no name.");
                if (string.IsNullOrWhiteSpace(material.ItemType))
                    throw new FormatException($"Material '{material.Id}' has no itemType.");
                if (string.IsNullOrWhiteSpace(material.Band))
                    throw new FormatException($"Material '{material.Id}' has no band.");
            }
        }

        public IReadOnlyList<MaterialInfo> All => all;

        public static MaterialCatalog Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<MaterialDto> data = JsonConvert.DeserializeObject<List<MaterialDto>>(json,
settings) ?? throw new FormatException("Material catalog JSON is null.");
                var materials = new List<MaterialInfo>(data.Count);
                foreach (MaterialDto dto in data)
                    materials.Add(new MaterialInfo(dto.Id,
dto.Name,
dto.ItemType,
dto.Band,
dto.Description,
dto.Tier,
dto.IsCatalyst,
dto.PvpOnly));
                return new MaterialCatalog(materials);
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid material catalog JSON: " + exception.Message, exception);
            }
        }

        public MaterialInfo Get(string id) => byId.TryGetValue(id,
out MaterialInfo value) ? value : throw new KeyNotFoundException($"Unknown material id '{id}'.");
        private sealed class MaterialDto
        {
            [JsonProperty("id")]
            public string Id;
            [JsonProperty("name")]
            public string Name;
            [JsonProperty("itemType")]
            public string ItemType;
            [JsonProperty("band")]
            public string Band;
            [JsonProperty("description")]
            public string Description;
            [JsonProperty("tier")]
            public int Tier;
            [JsonProperty("isCatalyst")]
            public bool IsCatalyst;
            [JsonProperty("pvpOnly")]
            public bool PvpOnly;
        }
    }
}