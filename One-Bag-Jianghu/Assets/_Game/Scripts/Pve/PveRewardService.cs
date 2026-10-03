using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Pve
{
    public sealed class PveRewardResult
    {
        public bool Success;
        public string Error = string.Empty;
        public long Silver;
        public long HeroExp;
        public int OldLevel;
        public int NewLevel;
        public readonly Dictionary<string, int> Materials = new Dictionary<string, int>(StringComparer.Ordinal);
        public readonly List<PveRewardItem> Items = new List<PveRewardItem>();
    }

    public sealed class PveRewardService
    {
        private readonly RewardCatalog repeatCatalog;
        private readonly MaterialCatalog materials;
        private readonly ItemCatalog items;
        public PveRewardService(RewardCatalog repeatCatalog, MaterialCatalog materials, ItemCatalog items)
        {
            this.repeatCatalog = repeatCatalog;
            this.materials = materials;
            this.items = items;
        }

        public PveRewardResult Grant(PlayerProfile profile, PveStage stage, bool firstClear, int seed)
        {
            PveRewardResult plan = firstClear ? First(stage) : Repeat(profile, seed);
            plan.OldLevel = profile.HeroLevel;
            if (!string.IsNullOrEmpty(plan.Error))
                return plan;
            RollDrops(plan, stage, seed);
            PlayerProfile trial = PlayerProfile.FromJson(profile.ToJson(), items);
            if (!Apply(trial, plan))
                return new PveRewardResult
                {
                    Error = "Tàng Bảo Các không đủ chỗ."
                };
            if (!Apply(profile, plan))
                throw new InvalidOperationException("PvE reward preflight did not match the live profile.");
            plan.NewLevel = profile.HeroLevel;
            plan.Success = true;
            return plan;
        }

        private PveRewardResult First(PveStage stage)
        {
            PveFirstClearReward r = stage.Rewards.FirstClear;
            var result = new PveRewardResult
            {
                Silver = r.Silver,
                HeroExp = r.HeroExp
            };
            foreach (KeyValuePair<string, int> value in r.Materials ?? new Dictionary<string, int>())
                result.Materials[value.Key] = value.Value;
            result.Items.AddRange(r.Items ?? new List<PveRewardItem>());
            return result;
        }

        private PveRewardResult Repeat(PlayerProfile profile, int seed)
        {
            PlayerProfile trial = PlayerProfile.FromJson(profile.ToJson(), items);
            BattleReward reward = new RewardService(repeatCatalog, materials, items).Grant(trial, true, false, seed);
            if (!reward.Success)
                return new PveRewardResult
                {
                    Error = reward.Error
                };
            var result = new PveRewardResult
            {
                Silver = reward.Silver,
                HeroExp = reward.HeroExp
            };
            foreach (KeyValuePair<string, int> value in reward.Materials)
                result.Materials[value.Key] = value.Value;
            return result;
        }

        private static void RollDrops(PveRewardResult result, PveStage stage, int seed)
        {
            uint state = unchecked((uint)seed) ^ 0x85EBCA6Bu;
            foreach (PveDrop drop in stage.Rewards.Drops ?? new List<PveDrop>())
            {
                state = unchecked(state * 1664525u + 1013904223u);
                if (state % 1000 < drop.ChancePermille)
                    result.Items.Add(new PveRewardItem { Id = drop.Id, Tier = 1, Rarity = drop.Rarity });
            }
        }

        private bool Apply(PlayerProfile profile, PveRewardResult reward)
        {
            var storage = new StorageGrid(profile, items);
            foreach (KeyValuePair<string, int> value in reward.Materials)
                if (!storage.CanAddMaterial(value.Key, value.Value))
                    return false;
            var added = new List<OwnedItem>();
            foreach (PveRewardItem value in reward.Items)
            {
                OwnedItem item = profile.AddItem(value.Id, value.Tier, rarity: value.Rarity);
                added.Add(item);
                if (!storage.TryPlaceItem(item))
                {
                    foreach (OwnedItem rollback in added)
                        profile.RemoveItem(rollback.Uid);
                    return false;
                }
            }

            foreach (KeyValuePair<string, int> value in reward.Materials)
                storage.TryAddMaterial(value.Key, value.Value);
            if (reward.Silver > 0)
                profile.AddCurrency("bac", reward.Silver);
            new LevelService(LevelConfig.Load()).Grant(profile, reward.HeroExp);
            return true;
        }
    }
}