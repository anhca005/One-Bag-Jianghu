using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Economy
{
    public sealed class PveStageProgress
    {
        [JsonProperty("cleared")]
        public bool Cleared;
        [JsonProperty("bestStars")]
        // Legacy, unused. Retained so profiles written by older builds still deserialize.
        public int BestStars;
    }

    public sealed class PveProfileState
    {
        [JsonProperty("stages")]
        public Dictionary<string,
            PveStageProgress> Stages = new Dictionary<string,
            PveStageProgress>(StringComparer.Ordinal);
        [JsonProperty("stamina")]
        public int Stamina = -1;
        [JsonProperty("lastStaminaRegenUtc")]
        public string LastStaminaRegenUtc = string.Empty;
        [JsonProperty("dailyFirstClears")]
        public List<string> DailyFirstClears = new List<string>();
        [JsonProperty("dailyFirstClearDate")]
        public string DailyFirstClearDate = string.Empty;
    }

    public sealed class OwnedItem
    {
        [JsonProperty("uid")]
        public string Uid = string.Empty;
        [JsonProperty("itemId")]
        public string ItemId = string.Empty;
        [JsonProperty("tier")]
        public int Tier = 1;
        [JsonProperty("rarity")]
        public string Rarity = "common";
        [JsonProperty("acquiredAt")]
        public string AcquiredAt = string.Empty;
    }

    public sealed class LoadoutPlacement
    {
        [JsonProperty("uid")]
        public string Uid = string.Empty;
        [JsonProperty("x")]
        public int X;
        [JsonProperty("y")]
        public int Y;
    }

    public sealed class ShopSlotState
    {
        [JsonProperty("kind")]
        public string Kind = string.Empty;
        [JsonProperty("contentId")]
        public string ContentId = string.Empty;
        [JsonProperty("rarity")]
        public string Rarity = string.Empty;
        [JsonProperty("quantity")]
        public int Quantity = 1;
        [JsonProperty("price")]
        public long Price;
        [JsonProperty("bought")]
        public bool Bought;
    }

    public sealed class ShopState
    {
        [JsonProperty("slots")]
        public List<ShopSlotState> Slots = new List<ShopSlotState>();
        [JsonProperty("lastFreeRefreshUtc")]
        public string LastFreeRefreshUtc = string.Empty;
        [JsonProperty("paidRefreshesToday")]
        public int PaidRefreshesToday;
        [JsonProperty("starterFreeRefreshesUsed")]
        public int StarterFreeRefreshesUsed;
        [JsonProperty("dayKey")]
        public string DayKey = string.Empty;
        [JsonProperty("weekKey")]
        public string WeekKey = string.Empty;
        [JsonProperty("reputationPurchases")]
        public Dictionary<string, int> ReputationPurchases = new Dictionary<string, int>(StringComparer.Ordinal);
        [JsonProperty("generation")]
        public int Generation;
    }

    public sealed class QuestProfileState
    {
        [JsonProperty("activeIndex")]
        public int ActiveIndex;
        [JsonProperty("activeId")]
        public string ActiveId = string.Empty;
        [JsonProperty("claimedIds")]
        public List<string> ClaimedIds = new List<string>();
        [JsonProperty("actionCounters")]
        public Dictionary<string, int> ActionCounters = new Dictionary<string, int>(StringComparer.Ordinal);
        [JsonProperty("actionBaseline")]
        public Dictionary<string, int> ActionBaseline = new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public sealed class PlayerProfile
    {
        public static event Action ProgressChanged;
        public const int CurrentVersion = 9;
        [JsonProperty("version")]
        public int Version = CurrentVersion;
        [JsonProperty("heroName")]
        public string HeroName = string.Empty;
        [JsonProperty("heroLevel")]
        public int HeroLevel = 1;
        [JsonProperty("heroExp")]
        public long HeroExp;
        [JsonProperty("sectId")]
        public string SectId;
        [JsonProperty("tamPhapLevel")]
        public int TamPhapLevel = 1;
        [JsonProperty("sectPromptShown")]
        public bool SectPromptShown;
        [JsonProperty("introSeen")]
        public bool IntroSeen;
        [JsonProperty("completedTutorials")]
        public List<string> CompletedTutorials = new List<string>();
        [JsonProperty("seenStoryChapters")]
        public List<int> SeenStoryChapters = new List<int>();
        [JsonProperty("pendingStoryChapters")]
        public List<int> PendingStoryChapters = new List<int>();
        [JsonProperty("currencies")]
        public Dictionary<string, long> Currencies = new Dictionary<string, long>(StringComparer.Ordinal);
        [JsonProperty("materials")]
        public Dictionary<string, int> Materials = new Dictionary<string, int>(StringComparer.Ordinal);
        [JsonProperty("items")]
        public List<OwnedItem> Items = new List<OwnedItem>();
        [JsonProperty("loadout")]
        public List<LoadoutPlacement> Loadout = new List<LoadoutPlacement>();
        [JsonProperty("firstClears")]
        public List<string> FirstClears = new List<string>();
        [JsonProperty("shop")]
        public ShopState Shop = new ShopState();
        [JsonProperty("storage")]
        public StorageState Storage = new StorageState();
        [JsonProperty("pve")]
        public PveProfileState Pve = new PveProfileState();
        [JsonProperty("quest")]
        public QuestProfileState Quest = new QuestProfileState();
        public OwnedItem AddItem(string itemId,
            int tier,
            string acquiredAt = null,
            string uid = null,
            string rarity = "common")
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("Item id must not be empty.", nameof(itemId));
            if (tier < 1 || tier > 9)
                throw new ArgumentOutOfRangeException(nameof(tier), "Item tier must be from 1 to 9.");
            if (!RarityRules.IsValid(rarity))
                throw new ArgumentException("Unknown rarity '" + rarity + "'.", nameof(rarity));
            string resolvedUid = string.IsNullOrWhiteSpace(uid) ? Guid.NewGuid().ToString("N") : uid.Trim();
            if (FindItem(resolvedUid) != null)
                throw new InvalidOperationException($"Owned item uid '{resolvedUid}' already exists.");
            var item = new OwnedItem
            {
                Uid = resolvedUid,
                ItemId = itemId.Trim(),
                Tier = tier,
                Rarity = rarity,
                AcquiredAt = acquiredAt ?? DateTimeOffset.UtcNow.ToString("O")
            };
            Items.Add(item);
            return item;
        }

        public void RecordQuestAction(string action)
        {
            Quest.ActionCounters.TryGetValue(action, out int count);
            Quest.ActionCounters[action] = checked(count + 1);
            ProgressChanged?.Invoke();
        }

        public static void NotifyProgressChanged()
        {
            ProgressChanged?.Invoke();
        }

        public void RemoveItem(string uid)
        {
            OwnedItem item = RequireItem(uid);
            Items.Remove(item);
            Loadout.RemoveAll(placement => string.Equals(placement.Uid, uid, StringComparison.Ordinal));
            Storage?.Items?.RemoveAll(placement => string.Equals(placement.Uid, uid, StringComparison.Ordinal));
        }

        public void AddMaterial(string id, int amount)
        {
            ValidateResource(id, amount, "Material");
            if (!new StorageGrid(this, null).TryAddMaterial(id, amount))
                throw new InvalidOperationException("Tàng Bảo Các không đủ chỗ.");
        }

        public bool SpendMaterials(IReadOnlyDictionary<string, int> costs, out string error)
        {
            return new StorageGrid(this, null).SpendMaterials(costs, out error);
        }

        public void AddCurrency(string id, long amount)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Currency id must not be empty.", nameof(id));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Currency amount must be positive.");
            Currencies.TryGetValue(id, out long current);
            checked
            {
                Currencies[id] = current + amount;
            }
        }

        public bool TrySpend(IReadOnlyDictionary<string, long> costs, out string error)
        {
            if (!CanAfford(costs, Currencies, "currency", out error))
                return false;
            foreach (KeyValuePair<string, long> cost in costs)
                Currencies[cost.Key] -= cost.Value;
            return true;
        }

        public OwnedItem FindItem(string uid)
        {
            if (uid == null)
                return null;
            return Items.Find(item => string.Equals(item.Uid, uid, StringComparison.Ordinal));
        }

        public OwnedItem RequireItem(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("Owned item uid must not be empty.", nameof(uid));
            return FindItem(uid) ?? throw new KeyNotFoundException($"Unknown owned item uid '{uid}'.");
        }

        public void Validate()
        {
            if (Version != CurrentVersion)
                throw new FormatException($"Unsupported profile version {Version}; expected {CurrentVersion}.");
            if (HeroLevel < 1)
                throw new FormatException("Profile heroLevel must be at least 1.");
            if (HeroExp < 0)
                throw new FormatException("Profile heroExp must not be negative.");
            if (TamPhapLevel < 1 || TamPhapLevel > 9)
                throw new FormatException("Profile tamPhapLevel must be from 1 to 9.");
            Currencies ??= new Dictionary<string, long>(StringComparer.Ordinal);
            Materials ??= new Dictionary<string, int>(StringComparer.Ordinal);
            Items ??= new List<OwnedItem>();
            Loadout ??= new List<LoadoutPlacement>();
            FirstClears ??= new List<string>();
            CompletedTutorials ??= new List<string>();
            SeenStoryChapters ??= new List<int>();
            PendingStoryChapters ??= new List<int>();
            Shop ??= new ShopState();
            Storage ??= new StorageState();
            Pve ??= new PveProfileState();
            Quest ??= new QuestProfileState();
            Quest.ActiveId ??= string.Empty;
            Quest.ClaimedIds ??= new List<string>();
            Quest.ActionCounters ??= new Dictionary<string, int>(StringComparer.Ordinal);
            Quest.ActionBaseline ??= new Dictionary<string, int>(StringComparer.Ordinal);
            if (Quest.ActiveIndex < 0)
                throw new FormatException("Profile quest activeIndex must not be negative.");
            foreach (KeyValuePair<string, int> counter in Quest.ActionCounters)
                if (string.IsNullOrWhiteSpace(counter.Key) || counter.Value < 0)
                    throw new FormatException("Profile contains an invalid quest action counter.");
            Pve.Stages ??= new Dictionary<string, PveStageProgress>(StringComparer.Ordinal);
            Pve.DailyFirstClears ??= new List<string>();
            if (Pve.Stamina < -1)
                throw new FormatException("Profile PvE stamina must be -1 or non-negative.");
            foreach (KeyValuePair<string, PveStageProgress> value in Pve.Stages)
                if (string.IsNullOrWhiteSpace(value.Key)
                    || value.Value == null)
                    throw new FormatException("Profile contains invalid PvE stage progress.");
            Storage.Items ??= new List<StorageItemPlacement>();
            Storage.MaterialStacks ??= new List<MaterialStack>();
            Storage.Overflow ??= new List<string>();
            if (Storage.Cols < 1 || Storage.Rows < 1)
                throw new FormatException("Profile storage dimensions must be positive.");
            Shop.Slots ??= new List<ShopSlotState>();
            Shop.ReputationPurchases ??= new Dictionary<string, int>(StringComparer.Ordinal);
            if (Shop.PaidRefreshesToday < 0
                || Shop.StarterFreeRefreshesUsed < 0
                || Shop.Generation < 0)
                throw new FormatException("Profile shop counters must not be negative.");
            var uids = new HashSet<string>(StringComparer.Ordinal);
            foreach (OwnedItem item in Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Uid))
                    throw new FormatException("Profile contains an item with no uid.");
                if (!uids.Add(item.Uid))
                    throw new FormatException($"Duplicate owned item uid '{item.Uid}'.");
                if (string.IsNullOrWhiteSpace(item.ItemId))
                    throw new FormatException($"Owned item '{item.Uid}' has no itemId.");
                if (item.Tier < 1 || item.Tier > 9)
                    throw new FormatException($"Owned item '{item.Uid}' tier must be from 1 to 9.");
                if (!RarityRules.IsValid(item.Rarity))
                    throw new FormatException($"Owned item '{item.Uid}' has invalid rarity '{item.Rarity}'.");
            }

            foreach (KeyValuePair<string, long> value in Currencies)
                if (string.IsNullOrWhiteSpace(value.Key) || value.Value < 0)
                    throw new FormatException("Profile currencies must have non-empty ids and non-negative values.");
            foreach (KeyValuePair<string, int> value in Materials)
                if (string.IsNullOrWhiteSpace(value.Key) || value.Value < 0)
                    throw new FormatException("Profile materials must have non-empty ids and non-negative values.");
            var placed = new HashSet<string>(StringComparer.Ordinal);
            foreach (LoadoutPlacement placement in Loadout)
            {
                if (placement == null || !uids.Contains(placement.Uid))
                    throw new FormatException($"Loadout references unknown owned item uid '{placement?.Uid}'.");
                if (!placed.Add(placement.Uid))
                    throw new FormatException($"Loadout contains owned item uid '{placement.Uid}' more than once.");
            }

            var storageUids = new HashSet<string>(StringComparer.Ordinal);
            foreach (StorageItemPlacement placement in Storage.Items)
            {
                if (placement == null || !uids.Contains(placement.Uid))
                    throw new FormatException($"Storage references unknown owned item uid '{placement?.Uid}'.");
                if (!storageUids.Add(placement.Uid) || placed.Contains(placement.Uid))
                    throw new FormatException(
                        $"Owned item uid '{placement.Uid}' occurs in storage/loadout more than once.");
            }

            foreach (MaterialStack stack in Storage.MaterialStacks)
                if (stack == null
                    || string.IsNullOrWhiteSpace(stack.MaterialId)
                    || stack.Count < 1
                    || stack.Count > StorageGrid.MaxStack)
                    throw new FormatException("Storage contains an invalid material stack.");
            var stackedTotals = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (MaterialStack stack in Storage.MaterialStacks)
            {
                stackedTotals.TryGetValue(stack.MaterialId, out int count);
                stackedTotals[stack.MaterialId] = checked(count + stack.Count);
            }

            foreach (KeyValuePair<string, int> value in Materials)
                if (!stackedTotals.TryGetValue(value.Key, out int count) || count != value.Value)
                    throw new FormatException($"Material total '{value.Key}' is not synchronized with storage stacks.");
        }

        public string ToJson()
        {
            Validate();
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static PlayerProfile FromJson(string json, IStorageItemCatalog catalog = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new FormatException("Profile JSON is empty.");
            try
            {
                JObject root = JObject.Parse(json);
                int version = root.Value<int?>("version") ?? 1;
                if (version == 1)
                {
                    if (root["items"] is JArray items)
                        foreach (JToken item in items)
                            item["rarity"] = "common";
                    version = 2;
                }

                if (version == 2)
                {
                    root["shop"] = JObject.FromObject(new ShopState());
                    version = 3;
                }

                if (version == 3)
                {
                    root["storage"] = JObject.FromObject(new StorageState());
                    version = 4;
                }

                if (version == 4)
                {
                    root["pve"] = JObject.FromObject(new PveProfileState());
                    version = 5;
                }

                if (version == 5)
                {
                    root["sectPromptShown"] = false;
                    version = 6;
                }

                if (version == 6)
                {
                    root["introSeen"] = false;
                    root["completedTutorials"] = new JArray();
                    version = 7;
                }

                if (version == 7)
                {
                    root["seenStoryChapters"] = new JArray();
                    root["pendingStoryChapters"] = new JArray();
                    version = 8;
                }

                if (version == 8)
                {
                    root["quest"] = JObject.FromObject(new QuestProfileState());
                    version = 9;
                }

                if (version != CurrentVersion)
                    throw new FormatException(
                        $"Unsupported profile version {version}; expected 1 through {CurrentVersion}.");
                root["version"] = version;
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                PlayerProfile profile = JsonConvert.DeserializeObject<PlayerProfile>(root.ToString(),
                    settings) ?? throw new FormatException("Profile JSON is null.");
                profile.EnsureStorage(catalog ?? UnitSizeCatalog.Instance);
                profile.Validate();
                return profile;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid profile JSON: " + exception.Message, exception);
            }
        }

        public void EnsureStorage(IStorageItemCatalog catalog)
        {
            Materials ??= new Dictionary<string, int>(StringComparer.Ordinal);
            Items ??= new List<OwnedItem>();
            Loadout ??= new List<LoadoutPlacement>();
            Storage ??= new StorageState();
            Storage.Items ??= new List<StorageItemPlacement>();
            Storage.MaterialStacks ??= new List<MaterialStack>();
            Storage.Overflow ??= new List<string>();
            var grid = new StorageGrid(this, catalog);
            var inLoadout = new HashSet<string>(StringComparer.Ordinal);
            foreach (LoadoutPlacement placement in Loadout)
                inLoadout.Add(placement.Uid);
            foreach (OwnedItem item in Items)
            {
                if (inLoadout.Contains(item.Uid) || Storage.Items.Exists(p => p.Uid == item.Uid))
                    continue;
                if (!grid.TryPlaceItem(item) && !Storage.Overflow.Contains("item:" + item.Uid))
                    Storage.Overflow.Add("item:" + item.Uid);
            }

            if (Storage.MaterialStacks.Count == 0 && Materials.Count > 0)
            {
                var old = new Dictionary<string, int>(Materials, StringComparer.Ordinal);
                Materials.Clear();
                foreach (KeyValuePair<string, int> value in old)
                    if (!grid.TryAddMaterial(value.Key, value.Value))
                        Storage.Overflow.Add("material:" + value.Key + ":" + value.Value);
            }

            grid.MergeStacks();
        }

        private sealed class UnitSizeCatalog : IStorageItemCatalog
        {
            public static readonly UnitSizeCatalog Instance = new UnitSizeCatalog();
            public void GetItemSize(string itemId, out int width, out int height)
            {
                width = height = 1;
            }
        }

        private static void ValidateResource(string id, int amount, string label)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(label + " id must not be empty.", nameof(id));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), label + " amount must be positive.");
        }

        private static bool CanAfford<T>(IReadOnlyDictionary<string,
            T> costs,
            IReadOnlyDictionary<string,
            T> balances,
            string label,
            out string error)
            where T : struct, IComparable<T>
        {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));
            T zero = default;
            foreach (KeyValuePair<string, T> cost in costs)
            {
                if (string.IsNullOrWhiteSpace(cost.Key) || cost.Value.CompareTo(zero) < 0)
                    throw new ArgumentException($"Every {label} cost must have a non-empty id and non-negative amount.",
                        nameof(costs));
                balances.TryGetValue(cost.Key, out T balance);
                if (balance.CompareTo(cost.Value) < 0)
                {
                    string name = cost.Key == "bac"
                        ? "Bạc"
                        : cost.Key == "danh_vong"
                            ? "Danh Vọng"
                            : "tài nguyên";
                    error = $"Không đủ {name}: cần {cost.Value}, hiện có {balance}.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
