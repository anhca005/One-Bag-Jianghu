using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public sealed class StarterKit
    {
        [JsonProperty("currencies")]
        public Dictionary<string, long> Currencies = new Dictionary<string, long>();
        [JsonProperty("items")]
        public List<StarterItem> Items = new List<StarterItem>();
        [JsonProperty("loadout")]
        public List<StarterPlacement> Loadout = new List<StarterPlacement>();
        public static StarterKit Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                return JsonConvert.DeserializeObject<StarterKit>(json,
                    settings) ?? throw new FormatException("Starter kit JSON is null.");
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid starter kit JSON: " + exception.Message, exception);
            }
        }

        public PlayerProfile CreateProfile(string heroName = "", string sectId = null)
        {
            var profile = new PlayerProfile
            {
                HeroName = heroName ?? string.Empty,
                SectId = sectId
            };
            foreach (KeyValuePair<string, long> currency in Currencies)
            {
                if (currency.Value < 0)
                    throw new FormatException($"Starter currency '{currency.Key}' is negative.");
                if (currency.Value > 0)
                    profile.AddCurrency(currency.Key, currency.Value);
                else
                    profile.Currencies[currency.Key] = 0;
            }

            var keys = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (StarterItem entry in Items)
            {
                if (string.IsNullOrWhiteSpace(entry.Key))
                    throw new FormatException("A starter item has no key.");
                if (keys.ContainsKey(entry.Key))
                    throw new FormatException($"Duplicate starter item key '{entry.Key}'.");
                OwnedItem item = profile.AddItem(entry.ItemId,
                    entry.Tier,
                    "starter",
                    "starter_"
                    + entry.Key,
                    entry.Rarity);
                keys.Add(entry.Key, item.Uid);
            }

            foreach (StarterPlacement entry in Loadout)
            {
                if (!keys.TryGetValue(entry.ItemKey, out string uid))
                    throw new FormatException($"Starter loadout references unknown itemKey '{entry.ItemKey}'.");
                profile.Loadout.Add(new LoadoutPlacement { Uid = uid, X = entry.X, Y = entry.Y });
            }

            profile.Validate();
            return profile;
        }
    }

    public sealed class StarterItem
    {
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("itemId")]
        public string ItemId;
        [JsonProperty("tier")]
        public int Tier = 1;
        [JsonProperty("rarity")]
        public string Rarity = "common";
    }

    public sealed class StarterPlacement
    {
        [JsonProperty("itemKey")]
        public string ItemKey;
        [JsonProperty("x")]
        public int X;
        [JsonProperty("y")]
        public int Y;
    }
}