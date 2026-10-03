using System;
using System.Collections.Generic;
using OneBagJianghu.Engine;
using Newtonsoft.Json;

namespace OneBagJianghu.Economy
{
    public interface IStorageItemCatalog
    {
        void GetItemSize(string itemId, out int width, out int height);
    }

    public sealed class StorageItemPlacement
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("x")]
        public int X;
        [JsonProperty("y")]
        public int Y;
    }

    public sealed class MaterialStack
    {
        [JsonProperty("materialId")]
        public string MaterialId;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("x")]
        public int X;
        [JsonProperty("y")]
        public int Y;
    }

    public sealed class StorageState
    {
        [JsonProperty("cols")]
        public int Cols = 10;
        [JsonProperty("rows")]
        public int Rows = 20;
        [JsonProperty("items")]
        public List<StorageItemPlacement> Items = new List<StorageItemPlacement>();
        [JsonProperty("materialStacks")]
        public List<MaterialStack> MaterialStacks = new List<MaterialStack>();
        [JsonProperty("overflow")]
        public List<string> Overflow = new List<string>();
    }

    /// <summary>Single spatial placement model shared by storage migration, economy and UI.</summary>
    public sealed class StorageGrid
    {
        public const int MaxStack = 999;
        private readonly StorageState state;
        private readonly PlayerProfile profile;
        private readonly IStorageItemCatalog catalog;
        public StorageGrid(PlayerProfile profile, IStorageItemCatalog catalog)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.catalog = catalog ?? UnitCatalog.Instance;
            state = profile.Storage ??= new StorageState();
            if (state.MaterialStacks.Count == 0 && profile.Materials.Count > 0)
            {
                var legacy = new Dictionary<string, int>(profile.Materials, StringComparer.Ordinal);
                profile.Materials.Clear();
                foreach (KeyValuePair<string, int> value in legacy)
                    if (!TryAddMaterial(value.Key, value.Value))
                        state.Overflow.Add("material:" + value.Key + ":" + value.Value);
            }
        }

        public int UsedCells
        {
            get
            {
                int used = state.MaterialStacks.Count;
                foreach (StorageItemPlacement placement in state.Items)
                {
                    OwnedItem item = profile.FindItem(placement.Uid);
                    if (item == null)
                        continue;
                    catalog.GetItemSize(item.ItemId, out int width, out int height);
                    used += width * height;
                }

                return used;
            }
        }

        public bool TryFindFirst(int width,
int height,
out int x,
out int y,
string ignoreUid = null,
MaterialStack ignoreStack = null)
        {
            for (int row = 0; row <= state.Rows - height; row++)
                for (int col = 0; col <= state.Cols - width; col++)
                    if (CanPlace(col, row, width, height, ignoreUid, ignoreStack))
                    {
                        x = col;
                        y = row;
                        return true;
                    }

            x = y = -1;
            return false;
        }

        public bool CanPlace(int x,
int y,
int width,
int height,
string ignoreUid = null,
MaterialStack ignoreStack = null)
        {
            var candidate = new Rect(x, y, width, height);
            if (!candidate.FitsIn(state.Cols, state.Rows))
                return false;
            foreach (StorageItemPlacement placement in state.Items)
            {
                if (placement.Uid == ignoreUid)
                    continue;
                OwnedItem item = profile.FindItem(placement.Uid);
                if (item == null)
                    continue;
                catalog.GetItemSize(item.ItemId, out int w, out int h);
                if (candidate.Overlaps(new Rect(placement.X, placement.Y, w, h)))
                    return false;
            }

            foreach (MaterialStack stack in state.MaterialStacks)
                if (!ReferenceEquals(stack, ignoreStack) && candidate.Overlaps(new Rect(stack.X, stack.Y, 1, 1)))
                    return false;
            return true;
        }

        public bool TryPlaceItem(OwnedItem item)
        {
            if (item == null || state.Items.Exists(x => x.Uid == item.Uid))
                return false;
            catalog.GetItemSize(item.ItemId, out int width, out int height);
            if (!TryFindFirst(width, height, out int x, out int y))
                return false;
            state.Items.Add(new StorageItemPlacement { Uid = item.Uid, X = x, Y = y });
            state.Overflow.Remove("item:" + item.Uid);
            return true;
        }

        public bool TryMoveItem(string uid, int x, int y)
        {
            StorageItemPlacement placement = state.Items.Find(p => p.Uid == uid);
            OwnedItem item = profile.FindItem(uid);
            if (placement == null || item == null)
                return false;
            catalog.GetItemSize(item.ItemId, out int width, out int height);
            if (!CanPlace(x, y, width, height, uid))
                return false;
            placement.X = x;
            placement.Y = y;
            return true;
        }

        public bool RemoveItem(string uid) => state.Items.RemoveAll(x => x.Uid == uid) > 0;
        public bool TryMoveStack(MaterialStack stack, int x, int y)
        {
            if (stack == null || !state.MaterialStacks.Contains(stack) || !CanPlace(x, y, 1, 1, null, stack))
                return false;
            stack.X = x;
            stack.Y = y;
            return true;
        }

        public bool CanAddMaterial(string id, int amount)
        {
            if (amount <= 0)
                return false;
            int freeInStacks = 0;
            foreach (MaterialStack stack in state.MaterialStacks)
                if (stack.MaterialId == id)
                    freeInStacks += MaxStack - stack.Count;
            int cells = Math.Max(0, amount - freeInStacks + MaxStack - 1) / MaxStack;
            var reserved = new List<MaterialStack>();
            for (int i = 0; i < cells; i++)
            {
                if (!TryFindFirstConsidering(reserved, out int x, out int y))
                    return false;
                reserved.Add(new MaterialStack { X = x, Y = y });
            }

            return true;
        }

        public bool TryAddMaterial(string id, int amount)
        {
            if (!CanAddMaterial(id, amount))
                return false;
            int remaining = amount;
            foreach (MaterialStack stack in state.MaterialStacks)
            {
                if (stack.MaterialId != id || stack.Count >= MaxStack)
                    continue;
                int add = Math.Min(remaining, MaxStack - stack.Count);
                stack.Count += add;
                remaining -= add;
                if (remaining == 0)
                    break;
            }

            while (remaining > 0)
            {
                TryFindFirst(1, 1, out int x, out int y);
                int add = Math.Min(remaining, MaxStack);
                state.MaterialStacks.Add(new MaterialStack { MaterialId = id, Count = add, X = x, Y = y });
                remaining -= add;
            }

            SyncMaterials();
            return true;
        }

        public bool SpendMaterials(IReadOnlyDictionary<string, int> costs, out string error)
        {
            foreach (KeyValuePair<string, int> cost in costs)
                if (MaterialCount(cost.Key) < cost.Value)
                {
                    error = $"Insufficient material '{cost.Key}'.";
                    return false;
                }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                int remaining = cost.Value;
                for (int i = state.MaterialStacks.Count - 1; i >= 0 && remaining > 0; i--)
                {
                    MaterialStack stack = state.MaterialStacks[i];
                    if (stack.MaterialId != cost.Key)
                        continue;
                    int take = Math.Min(remaining, stack.Count);
                    stack.Count -= take;
                    remaining -= take;
                    if (stack.Count == 0)
                        state.MaterialStacks.RemoveAt(i);
                }
            }

            SyncMaterials();
            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Merges partial stacks of the same material (each up to MaxStack), keeping the
        /// positions of the earliest stacks and freeing the cells of the rest.
        /// </summary>
        public void MergeStacks()
        {
            var order = new List<string>();
            var totals = new Dictionary<string, long>(StringComparer.Ordinal);
            var positions = new Dictionary<string, List<MaterialStack>>(StringComparer.Ordinal);
            foreach (MaterialStack stack in state.MaterialStacks)
            {
                if (!totals.ContainsKey(stack.MaterialId))
                {
                    order.Add(stack.MaterialId);
                    totals[stack.MaterialId] = 0;
                    positions[stack.MaterialId] = new List<MaterialStack>();
                }

                totals[stack.MaterialId] += stack.Count;
                positions[stack.MaterialId].Add(stack);
            }

            var merged = new List<MaterialStack>();
            foreach (string id in order)
            {
                long remaining = totals[id];
                int i = 0;
                while (remaining > 0)
                {
                    MaterialStack at = positions[id][i++];
                    int count = (int)Math.Min(remaining, MaxStack);
                    merged.Add(new MaterialStack { MaterialId = id, Count = count, X = at.X, Y = at.Y });
                    remaining -= count;
                }
            }

            state.MaterialStacks.Clear();
            state.MaterialStacks.AddRange(merged);
            SyncMaterials();
        }

        public int MaterialCount(string id)
        {
            int count = 0;
            foreach (MaterialStack stack in state.MaterialStacks)
                if (stack.MaterialId == id)
                    count += stack.Count;
            return count;
        }

        public void SyncMaterials()
        {
            profile.Materials.Clear();
            foreach (MaterialStack stack in state.MaterialStacks)
            {
                profile.Materials.TryGetValue(stack.MaterialId, out int count);
                profile.Materials[stack.MaterialId] = checked(count + stack.Count);
            }
        }

        private bool TryFindFirstConsidering(List<MaterialStack> reserved, out int x, out int y)
        {
            for (int row = 0; row < state.Rows; row++)
                for (int col = 0; col < state.Cols; col++)
                {
                    if (!CanPlace(col, row, 1, 1))
                        continue;
                    bool occupied = reserved.Exists(s => s.X == col && s.Y == row);
                    if (!occupied)
                    {
                        x = col;
                        y = row;
                        return true;
                    }
                }

            x = y = -1;
            return false;
        }

        private sealed class UnitCatalog : IStorageItemCatalog
        {
            public static readonly UnitCatalog Instance = new UnitCatalog();
            public void GetItemSize(string itemId, out int width, out int height)
            {
                width = height = 1;
            }
        }
    }
}