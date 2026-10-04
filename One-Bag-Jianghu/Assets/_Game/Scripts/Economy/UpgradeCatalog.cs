using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public sealed class UpgradeStep
    {
        [JsonProperty("fromTier")]
        public int FromTier;
        [JsonProperty("materialTier")]
        public int MaterialTier;
        [JsonProperty("materialCount")]
        public int MaterialCount;
        [JsonProperty("catalystCount")]
        public int CatalystCount;
        [JsonProperty("silver")]
        public long Silver;
    }

    public sealed class UpgradeCatalog
    {
        [JsonProperty("steps")]
        public Dictionary<string,
            List<UpgradeStep>> Steps = new Dictionary<string,
            List<UpgradeStep>>(StringComparer.Ordinal);
        public UpgradeStep Get(string rarity, int fromTier)
        {
            if (!Steps.TryGetValue(rarity, out List<UpgradeStep> steps))
                throw new KeyNotFoundException("Unknown upgrade rarity '" + rarity + "'.");
            UpgradeStep step = steps.Find(value => value.FromTier == fromTier);
            return step ?? throw new KeyNotFoundException($"No upgrade step for {rarity} Trọng {fromTier}.");
        }

        public static UpgradeCatalog Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                UpgradeCatalog value = JsonConvert.DeserializeObject<UpgradeCatalog>(json,
                    settings) ?? throw new FormatException("Upgrade catalog JSON is null.");
                foreach (string rarity in RarityRules.Ordered)
                {
                    if (!value.Steps.TryGetValue(rarity, out List<UpgradeStep> steps) || steps.Count != 8)
                        throw new FormatException("Upgrade catalog must contain eight steps for " + rarity + ".");
                    for (int tier = 1; tier <= 8; tier++)
                        value.Get(rarity, tier);
                }

                return value;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid upgrade catalog JSON: " + exception.Message, exception);
            }
        }
    }

    public sealed class MergeStep
    {
        [JsonProperty("fromRarity")]
        public string FromRarity;
        [JsonProperty("toRarity")]
        public string ToRarity;
        [JsonProperty("silver")]
        public long Silver;
        [JsonProperty("extraMaterials")]
        public Dictionary<string, int> ExtraMaterials = new Dictionary<string, int>();
    }

    public sealed class MergeCatalog
    {
        [JsonProperty("steps")]
        public List<MergeStep> Steps = new List<MergeStep>();
        public MergeStep Get(string rarity)
            => Steps.Find(step
            => step.FromRarity == rarity) ?? throw new KeyNotFoundException("No merge step for rarity '"
                + rarity
                + "'.");
        public static MergeCatalog Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                MergeCatalog value = JsonConvert.DeserializeObject<MergeCatalog>(json,
                    settings) ?? throw new FormatException("Merge catalog JSON is null.");
                if (value.Steps.Count != 5)
                    throw new FormatException("Merge catalog must contain five steps.");
                foreach (MergeStep step in value.Steps)
                    if (RarityRules.Next(step.FromRarity) != step.ToRarity)
                        throw new FormatException("Invalid merge rarity step " + step.FromRarity + ".");
                return value;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid merge catalog JSON: " + exception.Message, exception);
            }
        }
    }
}