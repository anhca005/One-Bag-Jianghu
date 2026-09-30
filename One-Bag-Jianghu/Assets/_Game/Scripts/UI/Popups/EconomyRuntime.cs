using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.UI.Popups
{
    internal sealed class EconomyRuntime : IShopItemCatalog, IStorageItemCatalog
    {
        private readonly ItemCatalog catalog = ItemCatalog.LoadFromResources();
        private readonly Dictionary<string, List<int>> unlocks = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        private readonly List<string> allItemIds = new List<string>();
        public EconomyRuntime()
        {
            TextAsset data = Resources.Load<TextAsset>("Data/items");
            foreach (JToken item in JArray.Parse(data.text))
            {
                var tiers = new List<int>();
                if (item["unlocks"] is JArray values)
                    foreach (JToken unlock in values)
                        if (unlock["tier"] != null) tiers.Add(unlock.Value<int>("tier"));
                unlocks[item.Value<string>("id")] = tiers;
                allItemIds.Add(item.Value<string>("id"));
            }
        }
        public string GetItemType(string itemId) => catalog.Get(itemId).Type;
        public IReadOnlyList<int> GetUnlockTiers(string itemId) => unlocks.TryGetValue(itemId, out List<int> value) ? value : Array.Empty<int>();
        public ItemInfo Get(string itemId) => catalog.Get(itemId);
        public void GetItemSize(string itemId, out int width, out int height) => catalog.GetItemSize(itemId, out width, out height);
        public IReadOnlyList<ItemInfo> All => catalog.All;
        public IReadOnlyList<string> AllItemIds => allItemIds;
        public static T Load<T>(string name, Func<string, T> parse)
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/" + name);
            if (asset == null) throw new InvalidOperationException("Không tìm thấy Data/" + name + ".json");
            return parse(asset.text);
        }
    }
}
