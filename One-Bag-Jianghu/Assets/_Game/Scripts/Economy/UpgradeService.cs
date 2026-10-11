using System;
using System.Collections.Generic;

namespace OneBagJianghu.Economy
{
    public sealed class UpgradePreview : EconomyResult
    {
        public long Silver
        {
            get;
            internal set;
        }
        public Dictionary<string, int> Materials
        {
            get;
        } = new Dictionary<string, int>(StringComparer.Ordinal);
        public int NextTier
        {
            get;
            internal set;
        }
        public bool UnlocksBreakthrough
        {
            get;
            internal set;
        }
    }

    public sealed class UpgradeService
    {
        private readonly UpgradeCatalog costs;
        private readonly MaterialCatalog materials;
        private readonly IUpgradeItemCatalog items;
        public UpgradeService(UpgradeCatalog costs, MaterialCatalog materials, IUpgradeItemCatalog items)
        {
            this.costs = costs;
            this.materials = materials;
            this.items = items;
        }

        public UpgradePreview Preview(PlayerProfile profile, string uid)
        {
            OwnedItem item = profile?.FindItem(uid);
            if (item == null)
                return new UpgradePreview
                {
                    Error = "Không tìm thấy vật phẩm."
                };
            if (item.Tier >= 9)
                return new UpgradePreview
                {
                    Error = "Vật phẩm đã đạt Cửu Trọng."
                };
            string group = Group(items.GetItemType(item.ItemId));
            UpgradeStep step = costs.Get(item.Rarity, item.Tier);
            var result = new UpgradePreview
            {
                Success = true,
                Silver = step.Silver,
                NextTier = item.Tier + 1
            };
            MaterialInfo normal = FindMaterial(group, step.MaterialTier, false);
            result.Materials[normal.Id] = step.MaterialCount;
            if (step.CatalystCount > 0)
                result.Materials[FindMaterial(group, 0, true).Id] = step.CatalystCount;
            IReadOnlyList<int> unlocks = items.GetUnlockTiers(item.ItemId);
            if (unlocks != null)
                for (int i = 0; i < unlocks.Count; i++)
                    if (unlocks[i] == result.NextTier)
                        result.UnlocksBreakthrough = true;
            if (!CanAfford(profile, result, out string error))
            {
                result.Success = false;
                result.Error = error;
            }

            return result;
        }

        public EconomyResult Apply(PlayerProfile profile, string uid)
        {
            UpgradePreview preview = Preview(profile, uid);
            if (!preview.Success)
                return EconomyResult.Fail(preview.Error);
            profile.Currencies["bac"] -= preview.Silver;
            new StorageGrid(profile, items as IStorageItemCatalog).SpendMaterials(preview.Materials, out _);
            profile.RequireItem(uid).Tier++;
            profile.RecordQuestAction("item_upgrade");
            return EconomyResult.Ok();
        }

        private bool CanAfford(PlayerProfile profile, UpgradePreview preview, out string error)
        {
            profile.Currencies.TryGetValue("bac", out long silver);
            if (silver < preview.Silver)
            {
                error = $"Không đủ Bạc ({silver}/{preview.Silver}).";
                return false;
            }

            foreach (KeyValuePair<string, int> cost in preview.Materials)
            {
                profile.Materials.TryGetValue(cost.Key, out int owned);
                if (owned < cost.Value)
                {
                    error = $"Không đủ {materials.Get(cost.Key).Name} ({owned}/{cost.Value}).";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        private MaterialInfo FindMaterial(string group, int tier, bool catalyst)
        {
            foreach (MaterialInfo material in materials.All)
                if (material.ItemType == group && material.Tier == tier && material.IsCatalyst == catalyst)
                    return material;
            throw new FormatException($"No material for group {group}, tier {tier}, catalyst {catalyst}.");
        }

        public static string Group(string type) => type == "pill" || type == "relic" ? "pill_relic" : type;
    }
}
