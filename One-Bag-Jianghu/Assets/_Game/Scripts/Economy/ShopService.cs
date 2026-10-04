using System;
using System.Collections.Generic;

namespace OneBagJianghu.Economy
{
    public interface IShopItemCatalog : IUpgradeItemCatalog
    {
        IReadOnlyList<string> AllItemIds
        {
            get;
        }
    }

    public interface IShopClock
    {
        DateTimeOffset UtcNow
        {
            get;
        }
    }

    public sealed class SystemShopClock : IShopClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public sealed class ShopService
    {
        private readonly ShopCatalog config;
        private readonly MaterialCatalog materials;
        private readonly IShopItemCatalog items;
        private readonly IShopClock clock;
        private readonly int seed;
        public ShopService(ShopCatalog config,
            MaterialCatalog materials,
            IShopItemCatalog items,
            int seed,
            IShopClock clock = null)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.materials = materials ?? throw new ArgumentNullException(nameof(materials));
            this.items = items ?? throw new ArgumentNullException(nameof(items));
            this.seed = seed;
            this.clock = clock ?? new SystemShopClock();
        }

        public IReadOnlyList<ShopSlotState> Ensure(PlayerProfile profile)
        {
            ResetPeriods(profile);
            if (profile.Shop.Slots.Count != 6)
                Generate(profile, false);
            return profile.Shop.Slots;
        }

        public TimeSpan UntilFreeRefresh(PlayerProfile profile)
        {
            Ensure(profile);
            DateTimeOffset last = Parse(profile.Shop.LastFreeRefreshUtc, clock.UtcNow);
            TimeSpan left = last.AddHours(config.FreeRefreshHours) - clock.UtcNow;
            return left > TimeSpan.Zero ? left : TimeSpan.Zero;
        }

        public int PaidRefreshCost(PlayerProfile profile)
        {
            ResetPeriods(profile);
            int index = Math.Min(profile.Shop.PaidRefreshesToday, config.PaidRefreshCosts.Length - 1);
            return config.PaidRefreshCosts[index];
        }

        public EconomyResult Refresh(PlayerProfile profile, bool free)
        {
            Ensure(profile);
            if (free)
            {
                if (UntilFreeRefresh(profile) > TimeSpan.Zero)
                    return EconomyResult.Fail("Chưa đến lượt làm mới miễn phí.");
                Generate(profile, true);
                return EconomyResult.Ok();
            }

            int cost = PaidRefreshCost(profile);
            profile.Currencies.TryGetValue("bac", out long silver);
            if (silver < cost)
                return EconomyResult.Fail($"Không đủ Bạc ({silver}/{cost}).");
            profile.Currencies["bac"] = silver - cost;
            profile.Shop.PaidRefreshesToday++;
            Generate(profile, false);
            return EconomyResult.Ok();
        }

        public void ForceFreeRefresh(PlayerProfile profile)
        {
            Ensure(profile);
            Generate(profile, true);
        }

        public EconomyResult Buy(PlayerProfile profile, int index)
        {
            Ensure(profile);
            if (index < 0 || index >= profile.Shop.Slots.Count)
                return EconomyResult.Fail("Ô hàng không hợp lệ.");
            ShopSlotState slot = profile.Shop.Slots[index];
            if (slot.Bought)
                return EconomyResult.Fail("Ô hàng này đã mua.");
            profile.Currencies.TryGetValue("bac", out long silver);
            if (silver < slot.Price)
                return EconomyResult.Fail($"Không đủ Bạc ({silver}/{slot.Price}).");
            var storage = new StorageGrid(profile, items as IStorageItemCatalog, materials);
            if (slot.Kind == "item")
            {
                items.GetItemSizeOrDefault(slot.ContentId, out int width, out int height);
                if (!storage.TryFindFirst(width, height, out _, out _))
                    return EconomyResult.Fail("Tàng Bảo Các không đủ chỗ.");
            }
            else if (!storage.CanAddMaterial(slot.ContentId, slot.Quantity))
                return EconomyResult.Fail("Tàng Bảo Các không đủ chỗ.");
            profile.Currencies["bac"] = silver - slot.Price;
            if (slot.Kind == "item")
                storage.TryPlaceItem(profile.AddItem(slot.ContentId, 1, rarity: slot.Rarity));
            else
                storage.TryAddMaterial(slot.ContentId, slot.Quantity);
            slot.Bought = true;
            return EconomyResult.Ok();
        }

        public bool HasStorageSpace(PlayerProfile profile, ShopSlotState slot)
        {
            var storage = new StorageGrid(profile, items as IStorageItemCatalog, materials);
            if (slot.Kind != "item")
                return storage.CanAddMaterial(slot.ContentId, slot.Quantity);
            items.GetItemSizeOrDefault(slot.ContentId, out int width, out int height);
            return storage.TryFindFirst(width, height, out _, out _);
        }

        public EconomyResult Exchange(PlayerProfile profile, string inputId, string targetId)
        {
            MaterialInfo input, target;
            try
            {
                input = materials.Get(inputId);
                target = materials.Get(targetId);
            }
            catch (KeyNotFoundException)
            {
                return EconomyResult.Fail("Nguyên liệu không hợp lệ.");
            }

            if (input.IsCatalyst
                || target.IsCatalyst
                || input.Id == "chi_ton_ngoc_tuy"
                || target.Id == "chi_ton_ngoc_tuy")
                return EconomyResult.Fail("Không thể đổi chất dẫn hoặc Chí Tôn Ngọc Tủy.");
            bool same = input.ItemType == target.ItemType && target.Tier == input.Tier + 1;
            bool cross = input.ItemType != target.ItemType && target.Tier == input.Tier;
            if (!same && !cross)
                return EconomyResult.Fail("Chỉ đổi lên một bậc cùng nhóm hoặc ngang bậc khác nhóm.");
            int count = same ? config.Exchange.SameInput : config.Exchange.CrossInput;
            int fee = same ? config.Exchange.SameFees[input.Tier - 1] : config.Exchange.CrossFees[input.Tier - 1];
            profile.Materials.TryGetValue(input.Id, out int owned);
            profile.Currencies.TryGetValue("bac", out long silver);
            if (owned < count)
                return EconomyResult.Fail($"Không đủ {input.Name} ({owned}/{count}).");
            if (silver < fee)
                return EconomyResult.Fail($"Không đủ Bạc ({silver}/{fee}).");
            // Spend through the storage stacks; writing profile.Materials directly was overwritten
            // by the next stack sync, so the input was never deducted.
            var storage = new StorageGrid(profile, items as IStorageItemCatalog, materials);
            if (!storage.SpendMaterials(new Dictionary<string, int> { [input.Id] = count }, out string error))
                return EconomyResult.Fail(error);
            if (!storage.TryAddMaterial(target.Id, 1))
            {
                storage.TryAddMaterial(input.Id, count);
                return EconomyResult.Fail("Tàng Bảo Các không đủ chỗ.");
            }

            profile.Currencies["bac"] = silver - fee;
            return EconomyResult.Ok();
        }

        public int ReputationRemaining(PlayerProfile profile, string offerId)
        {
            ResetPeriods(profile);
            ReputationOffer offer = config.ReputationOffers.Find(x => x.Id == offerId);
            if (offer == null)
                return 0;
            profile.Shop.ReputationPurchases.TryGetValue(offerId, out int bought);
            return Math.Max(0, offer.Limit - bought);
        }

        public EconomyResult BuyReputation(PlayerProfile profile, string offerId)
        {
            ResetPeriods(profile);
            ReputationOffer offer = config.ReputationOffers.Find(x => x.Id == offerId);
            if (offer == null)
                return EconomyResult.Fail("Món Danh Vọng không hợp lệ.");
            if (ReputationRemaining(profile, offerId) <= 0)
                return EconomyResult.Fail("Đã hết lượt mua trong tuần.");
            profile.Currencies.TryGetValue("danh_vong", out long value);
            if (value < offer.Price)
                return EconomyResult.Fail($"Không đủ Danh Vọng ({value}/{offer.Price}).");
            var random = Rng(profile.Shop.Generation * 397 + StableHash(offerId));
            string reward = offer.Id;
            if (offer.Id == "catalyst_box_random")
                reward = PickMaterial(random, 0, true).Id;
            else if (offer.Id == "tier_3_material_box")
                reward = PickMaterial(random, 3, false).Id;
            profile.Currencies["danh_vong"] = value - offer.Price;
            profile.AddMaterial(reward, offer.Quantity);
            profile.Shop.ReputationPurchases.TryGetValue(offerId, out int bought);
            profile.Shop.ReputationPurchases[offerId] = bought + 1;
            return EconomyResult.Ok();
        }

        // The very first shop a player sees offers this beginner Công Pháp in slot 1 (the tutorial buys it).
        public const string FirstShopItemId = "thai_thanh_tam_kinh";
        public const string FirstShopRarity = "common";

        private void Generate(PlayerProfile profile, bool markFree)
        {
            bool firstShop = profile.Shop.Generation == 0;
            var random = Rng(profile.Shop.Generation++);
            profile.Shop.Slots.Clear();
            foreach (ShopSlotRule rule in config.Slots)
                profile.Shop.Slots.Add(CreateSlot(rule, random));
            if (firstShop && profile.Shop.Slots.Count > 0 && profile.Shop.Slots[0].Kind == "item")
                profile.Shop.Slots[0] = ItemSlot(FirstShopItemId, FirstShopRarity, 0);
            if (markFree || string.IsNullOrEmpty(profile.Shop.LastFreeRefreshUtc))
                profile.Shop.LastFreeRefreshUtc = clock.UtcNow.ToString("O");
        }

        private ShopSlotState CreateSlot(ShopSlotRule rule, Random random)
        {
            if (rule.Type == "material")
                return MaterialSlot(PickMaterial(random, rule.Tier, false), rule.Quantity, rule.Price);
            if (rule.Type == "catalyst_or_tier3")
                return random.Next(100) < 40 ? MaterialSlot(PickMaterial(random,
                    0,
                    true),
                    1,
                    1200) : MaterialSlot(PickMaterial(random,
                    3,
                    false),
                    2,
                    800);
            string rarity = Weighted(rule.Rarities, rule.Weights, random);
            bool item = rule.Type == "item" || (rule.Type == "discount" && random.Next(100) >= 30);
            if (!item)
                return MaterialSlot(PickMaterial(random,
                    1,
                    false,
                    "pill_relic"),
                    5,
                    250 * (100 - rule.DiscountPercent) / 100);
            IReadOnlyList<string> all = items.AllItemIds;
            string selected = all[random.Next(all.Count)];
            return ItemSlot(selected, rarity, rule.DiscountPercent);
        }

        private ShopSlotState ItemSlot(string itemId, string rarity, int discountPercent)
        {
            string itemType = items.GetItemType(itemId);
            config.ItemTypePriceMultipliers.TryGetValue(itemType, out double multiplier);
            multiplier = multiplier > 0 ? multiplier : 1.0;
            long price = (long)Math.Round(config.ItemBasePrices[rarity] * multiplier);
            if (discountPercent > 0)
                price = price * (100 - discountPercent) / 100;
            return new ShopSlotState
            {
                Kind = "item",
                ContentId = itemId,
                Rarity = rarity,
                Quantity = 1,
                Price = price
            };
        }

        private ShopSlotState MaterialSlot(MaterialInfo value, int quantity, long price) => new ShopSlotState
        {
            Kind = "material",
            ContentId = value.Id,
            Quantity = quantity,
            Price = price
        };
        private MaterialInfo PickMaterial(Random random, int tier, bool catalyst, string group = null)
        {
            var pool = new List<MaterialInfo>();
            foreach (MaterialInfo x in materials.All)
                if (x.Id != "chi_ton_ngoc_tuy"
                    && x.Tier == tier
                    && x.IsCatalyst == catalyst
                    && (group == null
                    || x.ItemType == group))
                    pool.Add(x);
            if (pool.Count == 0)
                throw new FormatException("No matching shop material.");
            return pool[random.Next(pool.Count)];
        }

        private static string Weighted(string[] values, int[] weights, Random random)
        {
            int total = 0;
            foreach (int x in weights)
                total += x;
            int roll = random.Next(total);
            for (int i = 0; i < values.Length; i++)
            {
                roll -= weights[i];
                if (roll < 0)
                    return values[i];
            }

            return values[values.Length - 1];
        }

        private Random Rng(int salt) => new Random(unchecked(seed * 486187739 + salt));
        private static int StableHash(string value)
        {
            unchecked
            {
                int hash = 17;
                foreach (char c in value)
                    hash = hash * 31 + c;
                return hash;
            }
        }

        private void ResetPeriods(PlayerProfile profile)
        {
            string day = clock.UtcNow.UtcDateTime.ToString("yyyy-MM-dd");
            if (profile.Shop.DayKey != day)
            {
                profile.Shop.DayKey = day;
                profile.Shop.PaidRefreshesToday = 0;
            }

            DateTime date = clock.UtcNow.UtcDateTime.Date;
            int back = ((int)date.DayOfWeek + 6) % 7;
            string week = date.AddDays(-back).ToString("yyyy-MM-dd");
            if (profile.Shop.WeekKey != week)
            {
                profile.Shop.WeekKey = week;
                profile.Shop.ReputationPurchases.Clear();
            }
        }

        private static DateTimeOffset Parse(string value,
            DateTimeOffset fallback) => DateTimeOffset.TryParse(value,
            out DateTimeOffset parsed) ? parsed : fallback;
    }

    internal static class StorageCatalogExtensions
    {
        public static void GetItemSizeOrDefault(this IUpgradeItemCatalog catalog,
            string id,
            out int width,
            out int height)
        {
            if (catalog is IStorageItemCatalog sized)
                sized.GetItemSize(id, out width, out height);
            else
            {
                width = height = 1;
            }
        }
    }
}
