using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OneBagJianghu.Economy
{
    public sealed class PowerCatalog
    {
        private const float UnknownItemPower = 25f;
        private static readonly string[] DefaultReferenceKit = { "thanh_phong_kiem", "o_tam_y" };
        private readonly PowerData data;
        private readonly HashSet<string> warnedItemIds = new HashSet<string>(StringComparer.Ordinal);

        private PowerCatalog(PowerData data)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.data.Items ??= new Dictionary<string, JToken>(StringComparer.Ordinal);
            this.data.TierFactor ??= new Dictionary<string, double>(StringComparer.Ordinal);
            this.data.RarityFactor ??= new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            this.data.TierMultiplier ??= new Dictionary<string, double>(StringComparer.Ordinal);
            this.data.RarityMultiplier ??= new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            this.data.ReferenceKit ??= DefaultReferenceKit;
        }

        public float RecommendedFactor => data.RecommendedFactor;

        public static PowerCatalog Parse(string json)
        {
            PowerData parsed = JsonConvert.DeserializeObject<PowerData>(json);
            if (parsed == null)
                throw new FormatException("Power catalog JSON is null.");
            return new PowerCatalog(parsed);
        }

        public static PowerCatalog LoadFromResources()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/power");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/power.json.");
            return Parse(asset.text);
        }

        public int ItemPower(OwnedItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return ItemPower(item.ItemId, item.Tier, item.Rarity);
        }

        public int ItemPower(string itemId, int tier, string rarity)
        {
            if (data.Version < 2)
                return Round(RawV1ItemPower(itemId, tier, rarity));
            var kit = ReferenceItems();
            int basePower = FighterV2(kit, 1);
            kit.Add(CreateItem(itemId, tier, rarity));
            return Math.Max(0, FighterV2(kit, 1) - basePower);
        }

        public int Fighter(IEnumerable<OwnedItem> items, int level)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            return data.Version < 2 ? FighterV1(items, level) : FighterV2(items, level);
        }

        private int FighterV1(IEnumerable<OwnedItem> items, int level)
        {
            double total = data.LevelBase + data.LevelPoints * (Mathf.Max(1, level) - 1);
            foreach (OwnedItem item in items)
                total += RawV1ItemPower(item.ItemId, item.Tier, item.Rarity);
            return Round(total);
        }

        private int FighterV2(IEnumerable<OwnedItem> items, int level)
        {
            double offense = 0;
            double defense = data.LevelBaseDefense + data.LevelDefensePerLevel * (Mathf.Max(1, level) - 1);
            foreach (OwnedItem item in items)
            {
                ItemStats stats = GetItemStats(item.ItemId);
                double multiplier = V2Multiplier(item.Tier, item.Rarity);
                offense += stats.Offense * multiplier;
                defense += stats.Defense * multiplier;
            }

            return Round(data.Scale * Math.Sqrt(Math.Max(0, offense * defense)));
        }

        private double RawV1ItemPower(string itemId, int tier, string rarity)
        {
            double basePower = V1ItemBase(itemId);
            return basePower * Factor(data.TierFactor, tier.ToString()) * Factor(data.RarityFactor, rarity);
        }

        private double V1ItemBase(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId) && data.Items.TryGetValue(itemId, out JToken value))
                return value.Value<double>();
            WarnUnknown(itemId, "using base power 25.");
            return UnknownItemPower;
        }

        private ItemStats GetItemStats(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId) && data.Items.TryGetValue(itemId, out JToken value))
                return value.ToObject<ItemStats>();
            WarnUnknown(itemId, "using zero offense and defense.");
            return new ItemStats();
        }

        private double V2Multiplier(int tier, string rarity)
        {
            return Factor(data.TierMultiplier, tier.ToString()) * Factor(data.RarityMultiplier, rarity);
        }

        private List<OwnedItem> ReferenceItems()
        {
            var items = new List<OwnedItem>();
            foreach (string itemId in data.ReferenceKit)
                items.Add(CreateItem(itemId, 1, "common"));
            return items;
        }

        private static OwnedItem CreateItem(string itemId, int tier, string rarity)
        {
            return new OwnedItem { ItemId = itemId, Tier = tier, Rarity = rarity };
        }

        private void WarnUnknown(string itemId, string behavior)
        {
            string warningKey = itemId ?? "<null>";
            if (warnedItemIds.Add(warningKey))
                Debug.LogWarning("[Power] Unknown item id '" + warningKey + "'; " + behavior);
        }

        private static double Factor(Dictionary<string, double> factors, string key)
        {
            return !string.IsNullOrEmpty(key) && factors.TryGetValue(key, out double value) ? value : 1;
        }

        private static int Round(double value)
        {
            return (int)Math.Round(value, MidpointRounding.ToEven);
        }

        private sealed class ItemStats
        {
            [JsonProperty("offense")]
            public double Offense;
            [JsonProperty("defense")]
            public double Defense;
        }

        private sealed class PowerData
        {
            [JsonProperty("version")]
            public int Version;
            [JsonProperty("levelBase")]
            public double LevelBase;
            [JsonProperty("levelPoints")]
            public double LevelPoints;
            [JsonProperty("recommendedFactor")]
            public float RecommendedFactor = 1.1f;
            [JsonProperty("tierFactor")]
            public Dictionary<string, double> TierFactor;
            [JsonProperty("rarityFactor")]
            public Dictionary<string, double> RarityFactor;
            [JsonProperty("tierMultiplier")]
            public Dictionary<string, double> TierMultiplier;
            [JsonProperty("rarityMultiplier")]
            public Dictionary<string, double> RarityMultiplier;
            [JsonProperty("levelBaseDefense")]
            public double LevelBaseDefense;
            [JsonProperty("levelDefensePerLevel")]
            public double LevelDefensePerLevel;
            [JsonProperty("scale")]
            public double Scale;
            [JsonProperty("referenceKit")]
            public string[] ReferenceKit;
            [JsonProperty("items")]
            public Dictionary<string, JToken> Items;
        }
    }
}
