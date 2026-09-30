using System;
using System.Collections.Generic;

namespace OneBagJianghu.Economy
{
    public sealed class SellPreview : EconomyResult
    {
        public long Silver { get; internal set; }
        public Dictionary<string, int> Materials { get; } = new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public sealed class SellService
    {
        private static readonly long[] BasePrices = { 200, 500, 1200, 3000, 8000, 15000 };
        private readonly UpgradeCatalog upgrades; private readonly MaterialCatalog materials; private readonly IUpgradeItemCatalog items;
        public SellService(UpgradeCatalog upgrades, MaterialCatalog materials, IUpgradeItemCatalog items)
        { this.upgrades = upgrades; this.materials = materials; this.items = items; }
        public SellPreview Preview(PlayerProfile profile, string uid)
        {
            OwnedItem item = profile?.FindItem(uid);
            if (item == null) return new SellPreview { Error = "Không tìm thấy vật phẩm để bán." };
            long investedSilver = 0; var result = new SellPreview { Success = true };
            string group = UpgradeService.Group(items.GetItemType(item.ItemId));
            for (int tier = 1; tier < item.Tier; tier++)
            {
                UpgradeStep step = upgrades.Get(item.Rarity, tier); investedSilver += step.Silver;
                AddMaterial(result.Materials, group, step.MaterialTier, false, step.MaterialCount / 2);
                AddMaterial(result.Materials, group, 0, true, step.CatalystCount / 2);
            }
            result.Silver = BasePrices[RarityRules.Index(item.Rarity)] * 30 / 100 + investedSilver * 15 / 100;
            return result;
        }
        public EconomyResult Apply(PlayerProfile profile, string uid)
        {
            SellPreview preview = Preview(profile, uid); if (!preview.Success) return EconomyResult.Fail(preview.Error);
            var storage = new StorageGrid(profile, items as IStorageItemCatalog);
            storage.RemoveItem(uid); profile.RemoveItem(uid); profile.AddCurrency("bac", preview.Silver);
            foreach (KeyValuePair<string, int> value in preview.Materials) storage.TryAddMaterial(value.Key, value.Value);
            return EconomyResult.Ok();
        }
        private void AddMaterial(IDictionary<string, int> target, string group, int tier, bool catalyst, int count)
        {
            if (count <= 0) return;
            foreach (MaterialInfo value in materials.All) if (value.ItemType == group && value.Tier == tier && value.IsCatalyst == catalyst)
            { MergeService.Add(target, value.Id, count); return; }
            throw new FormatException("Missing sell refund material for " + group + ".");
        }
    }
}
