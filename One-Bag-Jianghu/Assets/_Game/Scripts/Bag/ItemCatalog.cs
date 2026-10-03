using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Bag
{
    public sealed class ItemCatalog : IStorageItemCatalog
    {
        private readonly List<ItemInfo> all;
        private readonly Dictionary<string, ItemInfo> byId;
        private ItemCatalog(List<ItemInfo> items)
        {
            all = items;
            byId = new Dictionary<string, ItemInfo>(StringComparer.Ordinal);
            foreach (ItemInfo item in items)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                    throw new FormatException("An item definition has no id.");
                if (!byId.TryAdd(item.Id, item))
                    throw new FormatException($"Duplicate item id '{item.Id}'.");
            }
        }

        public IReadOnlyList<ItemInfo> All => all;

        public static ItemCatalog Parse(string json)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<ItemDto> data = JsonConvert.DeserializeObject<List<ItemDto>>(json,
settings) ?? throw new FormatException("Item catalog JSON is null.");
            var items = new List<ItemInfo>(data.Count);
            foreach (ItemDto dto in data)
            {
                int width = dto.Size != null && dto.Size.Length > 0 ? dto.Size[0] : 1;
                int height = dto.Size != null && dto.Size.Length > 1 ? dto.Size[1] : 1;
                if (width < 1 || height < 1)
                    throw new FormatException($"Item '{dto.Id}' has an invalid size.");
                items.Add(new ItemInfo(dto.Id,
dto.Name,
dto.Type,
dto.WeaponClass,
dto.Sect,
dto.Rarity,
width,
height,
dto.Cooldown,
dto.Active != null
&& dto.Active.Length > 0));
            }

            return new ItemCatalog(items);
        }

        public static ItemCatalog LoadFromResources()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/items");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/items.json.");
            return Parse(asset.text);
        }

        public ItemInfo Get(string id)
        {
            if (!TryGet(id, out ItemInfo item))
                throw new KeyNotFoundException($"Unknown item id '{id}'.");
            return item;
        }

        public bool TryGet(string id, out ItemInfo item) => byId.TryGetValue(id, out item);
        public void GetItemSize(string itemId, out int width, out int height)
        {
            ItemInfo item = Get(itemId);
            width = item.Width;
            height = item.Height;
        }

        private sealed class ItemDto
        {
            [JsonProperty("id")]
            public string Id;
            [JsonProperty("name")]
            public string Name;
            [JsonProperty("type")]
            public string Type;
            [JsonProperty("weaponClass")]
            public string WeaponClass;
            [JsonProperty("sect")]
            public string Sect;
            [JsonProperty("rarity")]
            public string Rarity;
            [JsonProperty("size")]
            public int[] Size;
            [JsonProperty("cooldown")]
            public float Cooldown;
            [JsonProperty("active")]
            public object[] Active;
        }
    }
}