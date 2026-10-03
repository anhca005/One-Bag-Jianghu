using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public sealed class RewardRule
    {
        [JsonProperty("silver")]
        public long Silver;
        [JsonProperty("heroExp")]
        public long? HeroExp;
        [JsonProperty("materialTier")]
        public int MaterialTier;
        [JsonProperty("materialCount")]
        public int MaterialCount;
        [JsonProperty("materialChancePermille")]
        public int MaterialChancePermille = 1000;
        [JsonProperty("catalystChancePermille")]
        public int CatalystChancePermille;
    }

    public sealed class RewardCatalog
    {
        [JsonProperty("win")]
        public RewardRule Win;
        [JsonProperty("loss")]
        public RewardRule Loss;
        [JsonProperty("firstClear")]
        public RewardRule FirstClear;
        public static RewardCatalog Parse(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                RewardCatalog value = JsonConvert.DeserializeObject<RewardCatalog>(json,
settings) ?? throw new FormatException("Reward catalog JSON is null.");
                if (value.Win == null || value.Loss == null || value.FirstClear == null)
                    throw new FormatException("Reward catalog requires win, loss and firstClear.");
                return value;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid reward JSON: " + exception.Message, exception);
            }
        }
    }

    public sealed class BattleReward : EconomyResult
    {
        public long Silver
        {
            get;
            internal set;
        }
        public long HeroExp
        {
            get;
            internal set;
        }
        public int OldLevel
        {
            get;
            internal set;
        }
        public int NewLevel
        {
            get;
            internal set;
        }
        public Dictionary<string, int> Materials
        {
            get;
        } = new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public sealed class RewardService
    {
        private readonly RewardCatalog rewards;
        private readonly MaterialCatalog materials;
        private readonly IStorageItemCatalog itemCatalog;
        public RewardService(RewardCatalog rewards, MaterialCatalog materials, IStorageItemCatalog itemCatalog = null)
        {
            this.rewards = rewards;
            this.materials = materials;
            this.itemCatalog = itemCatalog;
        }

        public BattleReward Grant(PlayerProfile profile, bool won, bool firstClear, int seed)
        {
            if (profile == null)
                return new BattleReward
                {
                    Error = "Không có hồ sơ Hiệp Khách."
                };
            RewardRule rule = won ? (firstClear ? rewards.FirstClear : rewards.Win) : rewards.Loss;
            var result = new BattleReward
            {
                Success = true,
                Silver = rule.Silver,
                HeroExp = rule.HeroExp ?? 0,
                OldLevel = profile.HeroLevel
            };
            uint state = unchecked((uint)seed) ^ 0x9E3779B9u;
            if (rule.MaterialCount > 0 && Next(ref state) % 1000 < rule.MaterialChancePermille)
            {
                List<MaterialInfo> pool = Pool(rule.MaterialTier, false);
                MaterialInfo chosen = pool[(int)(Next(ref state) % (uint)pool.Count)];
                result.Materials[chosen.Id] = rule.MaterialCount;
            }

            if (rule.CatalystChancePermille > 0 && Next(ref state) % 1000 < rule.CatalystChancePermille)
            {
                List<MaterialInfo> pool = Pool(0, true);
                MaterialInfo chosen = pool[(int)(Next(ref state) % (uint)pool.Count)];
                MergeService.Add(result.Materials, chosen.Id, 1);
            }

            var storage = new StorageGrid(profile, itemCatalog);
            foreach (KeyValuePair<string, int> value in result.Materials)
                if (!storage.CanAddMaterial(value.Key, value.Value))
                    return new BattleReward
                    {
                        Error = "Tàng Bảo Các không đủ chỗ."
                    };
            if (result.Silver > 0)
                profile.AddCurrency("bac", result.Silver);
            new LevelService(LevelConfig.Load()).Grant(profile, result.HeroExp);
            result.NewLevel = profile.HeroLevel;
            foreach (KeyValuePair<string, int> value in result.Materials)
                storage.TryAddMaterial(value.Key, value.Value);
            return result;
        }

        private List<MaterialInfo> Pool(int tier, bool catalyst)
        {
            var result = new List<MaterialInfo>();
            foreach (MaterialInfo item in materials.All)
                if (item.Tier == tier && item.IsCatalyst == catalyst && !item.PvpOnly)
                    result.Add(item);
            if (result.Count == 0)
                throw new FormatException("Reward material pool is empty.");
            return result;
        }

        private static uint Next(ref uint state)
        {
            state = unchecked(state * 1664525u + 1013904223u);
            return state;
        }
    }
}