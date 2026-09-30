using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Bag
{
    public sealed class LoadoutData
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("hp")] public int Hp;
        [JsonProperty("cols")] public int Cols;
        [JsonProperty("rows")] public int Rows;
        [JsonProperty("sect", NullValueHandling = NullValueHandling.Ignore)] public string Sect;
        [JsonProperty("tamPhapLevel", DefaultValueHandling = DefaultValueHandling.Populate)]
        [System.ComponentModel.DefaultValue(1)] public int TamPhapLevel = 1;
        [JsonProperty("items")] public List<LoadoutItemData> Items = new List<LoadoutItemData>();
    }

    public sealed class LoadoutMeta
    {
        public string Sect;
        public int TamPhapLevel = 1;
    }

    public sealed class LoadoutItemData
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("x")] public int X;
        [JsonProperty("y")] public int Y;
        [JsonProperty("tier", DefaultValueHandling = DefaultValueHandling.Populate)]
        [System.ComponentModel.DefaultValue(1)] public int Tier = 1;
    }

    public static class LoadoutSerializer
    {
        public static string ToJson(BagGrid grid, string name, int hp)
        {
            return ToJson(grid, name, hp, null);
        }

        public static string ToJson(BagGrid grid, string name, int hp, LoadoutMeta meta)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            var data = new LoadoutData
            {
                Name = name,
                Hp = hp,
                Cols = grid.Cols,
                Rows = grid.Rows,
                Sect = meta?.Sect,
                TamPhapLevel = meta?.TamPhapLevel ?? 1
            };
            foreach (PlacedItem placed in grid.Items)
            {
                data.Items.Add(new LoadoutItemData
                {
                    Id = placed.Item.Id, X = placed.X, Y = placed.Y, Tier = placed.Tier
                });
            }
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static BagGrid FromJson(string json, ItemCatalog catalog)
        {
            return FromJson(json, catalog, out _);
        }

        public static BagGrid FromJson(string json, ItemCatalog catalog, out LoadoutMeta meta)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));
            LoadoutData data;
            try
            {
                data = JsonConvert.DeserializeObject<LoadoutData>(json);
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid loadout JSON: " + exception.Message, exception);
            }
            if (data == null)
                throw new FormatException("Invalid loadout: document is null.");
            if (data.Cols < 1 || data.Rows < 1)
                throw new FormatException("Invalid loadout dimensions.");
            if (data.TamPhapLevel < 1 || data.TamPhapLevel > 9)
                throw new FormatException("Invalid tamPhapLevel: expected 1..9.");

            meta = new LoadoutMeta
            {
                Sect = data.Sect,
                TamPhapLevel = data.TamPhapLevel
            };

            var grid = new BagGrid(data.Cols, data.Rows);
            var problems = new List<string>();
            if (data.Items == null)
                data.Items = new List<LoadoutItemData>();
            for (int i = 0; i < data.Items.Count; i++)
            {
                LoadoutItemData entry = data.Items[i];
                string label = string.IsNullOrWhiteSpace(entry?.Id) ? $"item #{i + 1}" : $"'{entry.Id}'";
                if (entry?.Id == null || !catalog.TryGet(entry.Id, out ItemInfo info))
                {
                    problems.Add($"{label}: unknown item id");
                    continue;
                }
                if (entry.Tier < 1 || entry.Tier > 9)
                {
                    problems.Add($"{label}: tier {entry.Tier} is outside 1..9");
                    continue;
                }
                if (!grid.CanPlace(info, entry.X, entry.Y))
                {
                    problems.Add($"{label}: cannot place at ({entry.X}, {entry.Y})");
                    continue;
                }
                grid.Place(info, entry.X, entry.Y, entry.Tier);
            }
            if (problems.Count > 0)
                throw new FormatException("Invalid loadout items: " + string.Join("; ", problems));
            return grid;
        }
    }
}
