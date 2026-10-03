using System;
using System.Collections.Generic;

namespace OneBagJianghu.Bag
{
    public sealed class PlacedItem
    {
        public int InstanceId
        {
            get;
        }
        public string OwnedUid
        {
            get;
        }
        public ItemInfo Item
        {
            get;
        }
        public int X
        {
            get;
            internal set;
        }
        public int Y
        {
            get;
            internal set;
        }
        public int Tier
        {
            get;
            internal set;
        }
        public GridRect Rect => new GridRect(X, Y, Item.Width, Item.Height);

        internal PlacedItem(int instanceId, ItemInfo item, int x, int y, int tier, string ownedUid)
        {
            InstanceId = instanceId;
            OwnedUid = ownedUid;
            Item = item;
            X = x;
            Y = y;
            Tier = tier;
        }
    }

    public sealed class BagGrid
    {
        private readonly List<PlacedItem> items = new List<PlacedItem>();
        private int nextInstanceId = 1;
        public int Cols
        {
            get;
        }
        public int Rows
        {
            get;
        }
        public IReadOnlyList<PlacedItem> Items => items;
        public int UsedCells
        {
            get;
            private set;
        }

        public event Action Changed;
        public BagGrid(int cols, int rows)
        {
            if (cols < 1 || rows < 1)
                throw new ArgumentOutOfRangeException(nameof(cols), "Bag dimensions must be positive.");
            Cols = cols;
            Rows = rows;
        }

        public bool CanPlace(ItemInfo item, int x, int y, int ignoreInstanceId = -1)
        {
            if (item == null)
                return false;
            var candidate = new GridRect(x, y, item.Width, item.Height);
            if (!candidate.FitsIn(Cols, Rows))
                return false;
            foreach (PlacedItem placed in items)
            {
                if (placed.InstanceId != ignoreInstanceId && candidate.Overlaps(placed.Rect))
                    return false;
            }

            return true;
        }

        public PlacedItem Place(ItemInfo item, int x, int y, int tier = 1, string ownedUid = null)
        {
            if (tier < 1 || tier > 9)
                throw new ArgumentOutOfRangeException(nameof(tier), "Tier must be from 1 to 9.");
            if (!CanPlace(item, x, y))
                throw new InvalidOperationException($"Cannot place item '{item?.Id ?? "<null>"}' at ({x}, {y}).");
            if (!string.IsNullOrEmpty(ownedUid) && items.Exists(value => value.OwnedUid == ownedUid))
                throw new InvalidOperationException($"Owned item uid '{ownedUid}' is already in the bag.");
            var placed = new PlacedItem(nextInstanceId++, item, x, y, tier, ownedUid);
            items.Add(placed);
            UsedCells += item.Width * item.Height;
            Changed?.Invoke();
            return placed;
        }

        public bool TryMove(int instanceId, int x, int y)
        {
            PlacedItem placed = Find(instanceId);
            if (placed == null || !CanPlace(placed.Item, x, y, instanceId))
                return false;
            placed.X = x;
            placed.Y = y;
            Changed?.Invoke();
            return true;
        }

        public bool Remove(int instanceId)
        {
            PlacedItem placed = Find(instanceId);
            if (placed == null)
                return false;
            items.Remove(placed);
            UsedCells -= placed.Item.Width * placed.Item.Height;
            Changed?.Invoke();
            return true;
        }

        public PlacedItem ItemAt(int x, int y)
        {
            foreach (PlacedItem placed in items)
            {
                GridRect rect = placed.Rect;
                if (x >= rect.X && x < rect.X + rect.W && y >= rect.Y && y < rect.Y + rect.H)
                    return placed;
            }

            return null;
        }

        public IReadOnlyList<PlacedItem> Neighbours(int instanceId)
        {
            PlacedItem source = Find(instanceId);
            var result = new List<PlacedItem>();
            if (source == null)
                return result;
            foreach (PlacedItem placed in items)
            {
                if (placed.InstanceId != instanceId && source.Rect.IsAdjacent(placed.Rect))
                    result.Add(placed);
            }

            return result;
        }

        public bool CycleTier(int instanceId)
        {
            PlacedItem placed = Find(instanceId);
            if (placed == null)
                return false;
            placed.Tier = placed.Tier == 9 ? 1 : placed.Tier + 1;
            Changed?.Invoke();
            return true;
        }

        public void Clear()
        {
            if (items.Count == 0)
                return;
            items.Clear();
            UsedCells = 0;
            Changed?.Invoke();
        }

        private PlacedItem Find(int instanceId) => items.Find(item => item.InstanceId == instanceId);
    }
}