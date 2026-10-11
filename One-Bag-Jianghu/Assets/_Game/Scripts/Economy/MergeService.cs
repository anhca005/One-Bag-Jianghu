using System;
using System.Collections.Generic;

namespace OneBagJianghu.Economy
{
    public sealed class MergePreview : EconomyResult
    {
        public string OutputRarity
        {
            get;
            internal set;
        }
        public int OutputTier
        {
            get;
            internal set;
        }
        public long Silver
        {
            get;
            internal set;
        }
        public bool WeakerThanBestInput
        {
            get;
            internal set;
        }
        public Dictionary<string, int> ExtraMaterials
        {
            get;
        } = new Dictionary<string, int>(StringComparer.Ordinal);
        public Dictionary<string, int> RefundMaterials
        {
            get;
        } = new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public sealed class MergeService
    {
        private readonly UpgradeCatalog upgrades;
        private readonly MergeCatalog merges;
        private readonly MaterialCatalog materials;
        private readonly IUpgradeItemCatalog items;
        public MergeService(UpgradeCatalog upgrades,
            MergeCatalog merges,
            MaterialCatalog materials,
            IUpgradeItemCatalog items)
        {
            this.upgrades = upgrades;
            this.merges = merges;
            this.materials = materials;
            this.items = items;
        }

        public MergePreview Preview(PlayerProfile profile, string firstUid, string secondUid)
        {
            string validation = Validate(profile, firstUid, secondUid, out OwnedItem first, out OwnedItem second);
            if (validation != null)
                return new MergePreview
                {
                    Error = validation
                };
            MergeStep step = merges.Get(first.Rarity);
            int outputTier = Math.Min(7, Math.Max(2, (first.Tier + second.Tier) / 3));
            var result = new MergePreview
            {
                Success = true,
                OutputRarity = step.ToRarity,
                OutputTier = outputTier,
                Silver = step.Silver,
                WeakerThanBestInput = Strength(step.ToRarity,
                    outputTier) < Math.Max(Strength(first.Rarity,
                    first.Tier),
                    Strength(second.Rarity,
                    second.Tier))
            };
            foreach (KeyValuePair<string, int> extra in step.ExtraMaterials)
                result.ExtraMaterials.Add(extra.Key, extra.Value);
            Dictionary<string, int> input = InvestedMaterials(first, second);
            Dictionary<string, int> output = InvestedMaterials(first.ItemId, step.ToRarity, outputTier);
            foreach (KeyValuePair<string, int> value in input)
            {
                int refund = value.Value / 2 - (output.TryGetValue(value.Key, out int spent) ? spent : 0);
                if (refund > 0)
                    result.RefundMaterials[value.Key] = refund;
            }

            profile.Currencies.TryGetValue("bac", out long silver);
            if (silver < result.Silver)
            {
                result.Success = false;
                result.Error = $"Không đủ Bạc ({silver}/{result.Silver}).";
                return result;
            }

            foreach (KeyValuePair<string, int> extra in result.ExtraMaterials)
            {
                profile.Materials.TryGetValue(extra.Key, out int owned);
                if (owned < extra.Value)
                {
                    result.Success = false;
                    result.Error = $"Không đủ {materials.Get(extra.Key).Name} ({owned}/{extra.Value}).";
                    return result;
                }
            }

            return result;
        }

        public IReadOnlyList<OwnedItem> Candidates(PlayerProfile profile, string selectedUid)
        {
            var result = new List<OwnedItem>();
            OwnedItem selected = profile?.FindItem(selectedUid);
            if (selected == null
                || selected.Tier < 3
                || selected.Rarity == "supreme")
                return result;
            foreach (OwnedItem value in profile.Items)
                if (value.Uid != selectedUid
                    && value.ItemId == selected.ItemId
                    && value.Rarity == selected.Rarity
                    && value.Tier >= 3)
                    result.Add(value);
            return result;
        }

        public EconomyResult Apply(PlayerProfile profile, string firstUid, string secondUid)
        {
            MergePreview preview = Preview(profile, firstUid, secondUid);
            if (!preview.Success)
                return EconomyResult.Fail(preview.Error);
            OwnedItem first = profile.RequireItem(firstUid);
            OwnedItem second = profile.RequireItem(secondUid);
            string itemId = first.ItemId;
            var storage = new StorageGrid(profile, items as IStorageItemCatalog);
            OwnedItem equippedResult = IsEquipped(profile, firstUid)
                ? first
                : IsEquipped(profile, secondUid)
                    ? second
                    : null;
            if (equippedResult != null)
            {
                OwnedItem consumed = ReferenceEquals(equippedResult, first) ? second : first;
                storage.RemoveItem(firstUid);
                storage.RemoveItem(secondUid);
                profile.RemoveItem(consumed.Uid);
                equippedResult.Rarity = preview.OutputRarity;
                equippedResult.Tier = preview.OutputTier;
                SpendAndRefund(profile, storage, preview);
                profile.RecordQuestAction("item_merge");
                return EconomyResult.Ok();
            }

            var removed = new List<StorageItemPlacement>();
            foreach (StorageItemPlacement p in profile.Storage.Items)
                if (p.Uid == firstUid || p.Uid == secondUid)
                    removed.Add(new StorageItemPlacement { Uid = p.Uid, X = p.X, Y = p.Y });
            storage.RemoveItem(firstUid);
            storage.RemoveItem(secondUid);
            items.GetItemSizeOrDefault(itemId, out int width, out int height);
            if (!storage.TryFindFirst(width, height, out _, out _))
            {
                profile.Storage.Items.AddRange(removed);
                return EconomyResult.Fail("Tàng Bảo Các không đủ chỗ.");
            }

            SpendAndRefund(profile, storage, preview);
            profile.RemoveItem(firstUid);
            profile.RemoveItem(secondUid);
            storage.TryPlaceItem(profile.AddItem(itemId, preview.OutputTier, rarity: preview.OutputRarity));
            profile.RecordQuestAction("item_merge");
            return EconomyResult.Ok();
        }

        private static string Validate(PlayerProfile profile,
            string a,
            string b,
            out OwnedItem first,
            out OwnedItem second)
        {
            first = profile?.FindItem(a);
            second = profile?.FindItem(b);
            if (first == null || second == null)
                return "Không tìm thấy bản sao để Hợp Thành.";
            if (a == b)
                return "Phải chọn hai bản sao khác nhau.";
            if (first.ItemId != second.ItemId)
                return "Hai bản sao phải là cùng một món.";
            if (first.Rarity != second.Rarity)
                return "Hai bản sao phải cùng phẩm.";
            if (first.Tier < 3 || second.Tier < 3)
                return "Cả hai bản sao phải đạt ít nhất Tam Trọng.";
            if (first.Rarity == "supreme")
                return "Phẩm Chí Tôn không thể Hợp Thành cao hơn.";
            return null;
        }

        private static bool IsEquipped(PlayerProfile profile, string uid)
        {
            return profile.Loadout.Exists(placement =>
                string.Equals(placement.Uid, uid, StringComparison.Ordinal));
        }

        private static void SpendAndRefund(PlayerProfile profile, StorageGrid storage, MergePreview preview)
        {
            profile.Currencies["bac"] -= preview.Silver;
            storage.SpendMaterials(preview.ExtraMaterials, out _);
            foreach (KeyValuePair<string, int> refund in preview.RefundMaterials)
                storage.TryAddMaterial(refund.Key, refund.Value);
        }

        private Dictionary<string, int> InvestedMaterials(OwnedItem a, OwnedItem b)
        {
            Dictionary<string, int> result = InvestedMaterials(a.ItemId, a.Rarity, a.Tier);
            foreach (KeyValuePair<string, int> value in InvestedMaterials(b.ItemId, b.Rarity, b.Tier))
                Add(result, value.Key, value.Value);
            return result;
        }

        private Dictionary<string, int> InvestedMaterials(string itemId, string rarity, int tier)
        {
            var result = new Dictionary<string, int>(StringComparer.Ordinal);
            string group = UpgradeService.Group(items.GetItemType(itemId));
            for (int current = 1; current < tier; current++)
            {
                UpgradeStep step = upgrades.Get(rarity, current);
                Add(result, Find(group, step.MaterialTier, false).Id, step.MaterialCount);
                if (step.CatalystCount > 0)
                    Add(result, Find(group, 0, true).Id, step.CatalystCount);
            }

            return result;
        }

        private MaterialInfo Find(string group, int tier, bool catalyst)
        {
            foreach (MaterialInfo value in materials.All)
                if (value.ItemType == group && value.Tier == tier && value.IsCatalyst == catalyst)
                    return value;
            throw new FormatException("Missing upgrade material for " + group + ".");
        }

        private static int Strength(string rarity, int tier) => RarityRules.PowerPermille(rarity, tier);
        internal static void Add(IDictionary<string, int> target, string id, int amount)
        {
            target.TryGetValue(id, out int current);
            target[id] = checked(current + amount);
        }
    }
}
