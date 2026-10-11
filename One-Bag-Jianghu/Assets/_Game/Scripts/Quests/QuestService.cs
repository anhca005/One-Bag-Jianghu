using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;

namespace OneBagJianghu.Quests
{
    public readonly struct QuestProgress
    {
        public readonly int Current;
        public readonly int Target;
        public bool Done => Current >= Target;
        public QuestProgress(int current, int target)
        {
            Current = Math.Min(current, target);
            Target = target;
        }
    }

    public sealed class QuestClaimResult
    {
        public bool Success;
        public string Error = string.Empty;
        public QuestReward Reward;
        public int OldLevel;
        public int NewLevel;
    }

    public sealed class QuestService
    {
        public static event Action Changed;
        private readonly QuestCatalog catalog;
        private readonly PveCatalog pve;
        private readonly MaterialCatalog materials;
        private readonly ItemCatalog items;

        public QuestService(QuestCatalog catalog, PveCatalog pve, MaterialCatalog materials, ItemCatalog items)
        {
            this.catalog = catalog;
            this.pve = pve;
            this.materials = materials;
            this.items = items;
        }

        public QuestDefinition Active(PlayerProfile profile)
        {
            EnsureActive(profile);
            return profile.Quest.ActiveIndex < catalog.Quests.Count
                ? catalog.Quests[profile.Quest.ActiveIndex]
                : null;
        }

        public QuestProgress Progress(PlayerProfile profile, QuestDefinition quest)
        {
            QuestObjective objective = quest.Objective;
            int target = ObjectiveTarget(objective);
            int current;
            switch (objective.Type)
            {
                case "stage_clear":
                    current = PveRules.IsCleared(profile, objective.Stage) ? 1 : 0;
                    break;
                case "hero_level":
                    current = profile.HeroLevel;
                    break;
                case "sect_joined":
                    current = string.IsNullOrWhiteSpace(profile.SectId) ? 0 : 1;
                    break;
                case "tam_phap_level":
                    current = profile.TamPhapLevel;
                    break;
                case "equip_count":
                    current = profile.Loadout.Count;
                    break;
                case "item_tier":
                    current = HighestTier(profile);
                    break;
                default:
                    current = Counter(profile, objective.Type) - Baseline(profile, objective.Type);
                    break;
            }
            return new QuestProgress(Math.Max(0, current), target);
        }

        public QuestClaimResult Claim(PlayerProfile profile)
        {
            QuestDefinition quest = Active(profile);
            if (quest == null || !Progress(profile, quest).Done)
                return new QuestClaimResult { Error = "Nhiệm vụ chưa hoàn thành." };
            PlayerProfile trial = PlayerProfile.FromJson(profile.ToJson(), items);
            if (!ApplyReward(trial, quest.Reward))
                return new QuestClaimResult { Error = "Tàng Bảo Các không đủ chỗ." };
            int oldLevel = profile.HeroLevel;
            if (!ApplyReward(profile, quest.Reward))
                throw new InvalidOperationException("Quest reward preflight did not match the live profile.");
            profile.Quest.ClaimedIds.Add(quest.Id);
            profile.Quest.ActiveIndex++;
            SetBaseline(profile);
            PlayerProfile.NotifyProgressChanged();
            Changed?.Invoke();
            OnboardingRuntime.Publish("quest_claimed");
            return new QuestClaimResult
            {
                Success = true,
                Reward = quest.Reward,
                OldLevel = oldLevel,
                NewLevel = profile.HeroLevel
            };
        }

        public string ObjectiveLabel(QuestDefinition quest)
        {
            QuestObjective objective = quest.Objective;
            switch (objective.Type)
            {
                case "stage_clear": return "Qua ải " + pve.GetStage(objective.Stage).Name;
                case "hero_level": return "Đạt cấp " + objective.Level;
                case "sect_joined": return "Bái nhập một Môn Phái";
                case "tam_phap_level": return "Tâm Pháp tầng " + objective.Level;
                case "equip_count": return "Xếp " + objective.Count + " món vào Bao Phục";
                case "item_tier": return "Có món đạt " + objective.Tier + " Trọng";
                case "shop_buy": return "Mua " + objective.Count + " món ở Thương Hội";
                case "item_upgrade": return "Nâng Trọng " + objective.Count + " lần";
                case "item_merge": return "Hợp Thành " + objective.Count + " lần";
                default: return "Thắng " + objective.Count + " trận";
            }
        }

        public static void RecordAction(PlayerProfile profile, string action)
        {
            profile?.RecordQuestAction(action);
            Changed?.Invoke();
        }

        public static void NotifyChanged()
        {
            Changed?.Invoke();
        }

        private void EnsureActive(PlayerProfile profile)
        {
            if (profile.Quest.ActiveIndex < 0)
                profile.Quest.ActiveIndex = 0;
            if (string.IsNullOrEmpty(profile.Quest.ActiveId) && profile.Quest.ActiveIndex < catalog.Quests.Count)
                SetBaseline(profile);
        }

        private void SetBaseline(PlayerProfile profile)
        {
            profile.Quest.ActiveId = profile.Quest.ActiveIndex < catalog.Quests.Count
                ? catalog.Quests[profile.Quest.ActiveIndex].Id
                : string.Empty;
            profile.Quest.ActionBaseline = new Dictionary<string, int>(profile.Quest.ActionCounters,
                StringComparer.Ordinal);
        }

        private bool ApplyReward(PlayerProfile profile, QuestReward reward)
        {
            var storage = new StorageGrid(profile, items, materials);
            foreach (KeyValuePair<string, int> value in reward.Materials)
                if (!storage.CanAddMaterial(value.Key, value.Value))
                    return false;
            foreach (KeyValuePair<string, int> value in reward.Materials)
                storage.TryAddMaterial(value.Key, value.Value);
            if (reward.Silver > 0)
                profile.AddCurrency("bac", reward.Silver);
            new LevelService(LevelConfig.Load()).Grant(profile, reward.HeroExp);
            return true;
        }

        private static int ObjectiveTarget(QuestObjective objective)
        {
            if (objective.Type == "hero_level" || objective.Type == "tam_phap_level") return objective.Level;
            if (objective.Type == "equip_count") return objective.Count;
            if (objective.Type == "item_tier") return objective.Tier;
            if (objective.Type == "stage_clear" || objective.Type == "sect_joined") return 1;
            return objective.Count;
        }

        private static int HighestTier(PlayerProfile profile)
        {
            int highest = 0;
            foreach (OwnedItem item in profile.Items)
                highest = Math.Max(highest, item.Tier);
            return highest;
        }

        private static int Counter(PlayerProfile profile, string key)
        {
            profile.Quest.ActionCounters.TryGetValue(key, out int value);
            return value;
        }

        private static int Baseline(PlayerProfile profile, string key)
        {
            profile.Quest.ActionBaseline.TryGetValue(key, out int value);
            return value;
        }
    }
}
