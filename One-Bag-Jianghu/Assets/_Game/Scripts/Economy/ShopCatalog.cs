using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public sealed class ShopSlotRule
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("rarities")]
        public string[] Rarities;
        [JsonProperty("weights")]
        public int[] Weights;
        [JsonProperty("tier")]
        public int Tier;
        [JsonProperty("quantity")]
        public int Quantity;
        [JsonProperty("price")]
        public long Price;
        [JsonProperty("discountPercent")]
        public int DiscountPercent;
    }

    public sealed class ExchangeRules
    {
        [JsonProperty("sameInput")]
        public int SameInput;
        [JsonProperty("sameFees")]
        public int[] SameFees;
        [JsonProperty("crossInput")]
        public int CrossInput;
        [JsonProperty("crossFees")]
        public int[] CrossFees;
    }

    public sealed class ReputationOffer
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("price")]
        public int Price;
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("quantity")]
        public int Quantity = 1;
    }

    public sealed class ShopCatalog
    {
        [JsonProperty("freeRefreshHours")]
        public int FreeRefreshHours;
        [JsonProperty("starterFreeRefreshes")]
        public int StarterFreeRefreshes;
        [JsonProperty("paidRefreshCost")]
        public int PaidRefreshCost;
        [JsonProperty("paidRefreshCosts")]
        public int[] PaidRefreshCosts;
        [JsonProperty("itemBasePrices")]
        public Dictionary<string, int> ItemBasePrices;
        [JsonProperty("itemTypePriceMultipliers")]
        public Dictionary<string, double> ItemTypePriceMultipliers = new Dictionary<string, double>();
        [JsonProperty("slots")]
        public List<ShopSlotRule> Slots;
        [JsonProperty("exchange")]
        public ExchangeRules Exchange;
        [JsonProperty("reputationOffers")]
        public List<ReputationOffer> ReputationOffers;
        public static ShopCatalog Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                ShopCatalog value = JsonConvert.DeserializeObject<ShopCatalog>(json,
                    settings) ?? throw new FormatException("Shop catalog JSON is null.");
                bool hasPaidRefreshCost = value.PaidRefreshCost > 0
                    || value.PaidRefreshCosts != null
                    && value.PaidRefreshCosts.Length > 0
                    && value.PaidRefreshCosts[0] > 0;
                if (value.FreeRefreshHours < 0
                    || value.StarterFreeRefreshes < 0
                    || !hasPaidRefreshCost
                    || value.Slots == null
                    || value.Slots.Count != 6
                    || value.Exchange == null
                    || value.ReputationOffers == null)
                    throw new FormatException("Shop catalog is incomplete.");
                if (value.PaidRefreshCost <= 0)
                    value.PaidRefreshCost = value.PaidRefreshCosts[0];
                value.ItemTypePriceMultipliers = value.ItemTypePriceMultipliers
                    ?? new Dictionary<string, double>();
                foreach (double multiplier in value.ItemTypePriceMultipliers.Values)
                {
                    if (!(multiplier > 0))
                        throw new FormatException("Shop item type price multipliers must be greater than zero.");
                }

                return value;
            }
            catch (JsonException e)
            {
                throw new FormatException("Invalid shop catalog JSON: " + e.Message, e);
            }
        }
    }
}
