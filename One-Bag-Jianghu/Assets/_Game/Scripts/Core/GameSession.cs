using System;
using System.Collections.Generic;
using System.IO;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OneBagJianghu.Core
{
    public enum GameMode { Pve, Pvp }

    public sealed class ProfileStore
    {
        public const string HeroNameKey = "OneBagJianghu.HeroName";
        public const string SectIdKey = "OneBagJianghu.SectId";
        private readonly string path;
        private readonly Func<string, string> readLegacy;
        private readonly Action clearLegacy;
        private readonly StarterKit starter;
        public string FilePath => path;
        public int? LastMigrationFrom { get; private set; }

        public ProfileStore(string path, StarterKit starter, Func<string, string> readLegacy,
            Action clearLegacy = null)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.starter = starter ?? throw new ArgumentNullException(nameof(starter));
            this.readLegacy = readLegacy ?? throw new ArgumentNullException(nameof(readLegacy));
            this.clearLegacy = clearLegacy;
        }

        public static ProfileStore CreateDefault()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/starter");
            if (asset == null) throw new InvalidOperationException("Could not load Resources/Data/starter.json.");
            return new ProfileStore(Path.Combine(Application.persistentDataPath, "profile.json"),
                StarterKit.Parse(asset.text), key => PlayerPrefs.GetString(key, string.Empty), () =>
                {
                    PlayerPrefs.DeleteKey(HeroNameKey);
                    PlayerPrefs.DeleteKey(SectIdKey);
                    PlayerPrefs.Save();
                });
        }

        public PlayerProfile LoadOrCreate()
        {
            string backup = path + ".bak";
            if (File.Exists(path))
            {
                try { return Read(path); }
                catch (FormatException) when (File.Exists(backup))
                {
                    PlayerProfile recovered = Read(backup);
                    File.Copy(backup, path, true);
                    return recovered;
                }
            }
            if (File.Exists(backup))
            {
                PlayerProfile recovered = Read(backup);
                Save(recovered);
                return recovered;
            }
            PlayerProfile profile = starter.CreateProfile(readLegacy(HeroNameKey), EmptyToNull(readLegacy(SectIdKey)));
            Save(profile);
            clearLegacy?.Invoke();
            return profile;
        }

        public void Save(PlayerProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, profile.ToJson());
            try
            {
                if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
                else File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        private PlayerProfile Read(string source)
        {
            try
            {
                string json = File.ReadAllText(source);
                int version = JObject.Parse(json)["version"]?.Value<int>() ?? 1;
                LastMigrationFrom = version < PlayerProfile.CurrentVersion ? version : null;
                return PlayerProfile.FromJson(json, ItemCatalog.LoadFromResources());
            }
            catch (Newtonsoft.Json.JsonException exception)
            { throw new FormatException($"Could not parse player profile '{source}': {exception.Message}", exception); }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            { throw new IOException($"Could not read player profile '{source}': {exception.Message}", exception); }
        }

        private static string EmptyToNull(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public sealed class GameSession : MonoBehaviour
    {
        private ProfileStore store;
        public PlayerProfile Profile { get; private set; }
        public string HeroName { get => Profile?.HeroName ?? string.Empty; set { EnsureProfile(); Profile.HeroName = value ?? string.Empty; } }
        public string SectId { get => Profile?.SectId; set { EnsureProfile(); Profile.SectId = value; } }
        public int TamPhapLevel { get => Profile?.TamPhapLevel ?? 1; set { EnsureProfile(); Profile.TamPhapLevel = value; } }
        public int HeroLevel { get => Profile?.HeroLevel ?? 1; set { EnsureProfile(); Profile.HeroLevel = value; } }
        public string CurrentLoadoutJson
        {
            get => Profile == null ? string.Empty : BuildEngineLoadoutJson(Profile, ItemCatalog.LoadFromResources());
            set { EnsureProfile(); ImportEngineLoadout(value); }
        }
        public GameMode SelectedMode { get; set; } = GameMode.Pve;

        public bool TryValidateLoadout(out string warning)
        {
            warning = "Bao Phục đang trống";
            if (Profile == null || Profile.Loadout.Count == 0) return false;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            foreach (LoadoutPlacement placement in Profile.Loadout)
            {
                OwnedItem owned = Profile.FindItem(placement.Uid);
                ItemInfo info;
                if (owned != null && catalog.TryGet(owned.ItemId, out info) &&
                    string.Equals(info.Type, "weapon", StringComparison.OrdinalIgnoreCase))
                { warning = string.Empty; return true; }
            }
            warning = "Bao Phục chưa có binh khí";
            return false;
        }

        public void LoadProfile()
        {
            store = ProfileStore.CreateDefault();
            Profile = store.LoadOrCreate();
            Profile.EnsureStorage(ItemCatalog.LoadFromResources());
            Profile.Currencies.TryGetValue("bac", out long silver);
            string migration = store.LastMigrationFrom.HasValue ? ", migratedFrom=v" + store.LastMigrationFrom.Value : string.Empty;
            Debug.Log($"[Profile] Load path={store.FilePath}, version={Profile.Version}{migration}, items={Profile.Items.Count}, materialStacks={Profile.Storage.MaterialStacks.Count}, Bạc={silver}");
            if (Profile.Storage.Overflow.Count > 0)
                Debug.LogWarning($"[Profile] Migration overflow items={Profile.Storage.Overflow.Count}: {string.Join(",", Profile.Storage.Overflow)}");
        }

        public void SaveProfile()
        {
            EnsureProfile();
            store ??= ProfileStore.CreateDefault();
            store.Save(Profile);
            Debug.Log("[Profile] Save path=" + store.FilePath);
        }

        public void ResetProfile()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/starter");
            Profile = StarterKit.Parse(asset.text).CreateProfile(HeroName, SectId);
            Profile.EnsureStorage(ItemCatalog.LoadFromResources());
            SaveProfile();
            Debug.Log($"[Profile] Reset to starter kit: items={Profile.Items.Count}, materialStacks={Profile.Storage.MaterialStacks.Count}");
        }

        public static MaterialCatalog LoadMaterialCatalog()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/materials");
            if (asset == null) throw new InvalidOperationException("Could not load Resources/Data/materials.json.");
            return MaterialCatalog.Parse(asset.text);
        }

        public static string BuildEngineLoadoutJson(PlayerProfile profile, ItemCatalog catalog)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            var grid = new BagGrid(5, 4);
            foreach (LoadoutPlacement placement in profile.Loadout)
            {
                OwnedItem owned = profile.RequireItem(placement.Uid);
                ItemInfo info = catalog.Get(owned.ItemId);
                if (!grid.CanPlace(info, placement.X, placement.Y))
                    throw new FormatException($"Owned item '{owned.Uid}' cannot be placed at ({placement.X}, {placement.Y}).");
                grid.Place(info, placement.X, placement.Y, owned.Tier, owned.Uid);
            }
            JObject json = JObject.Parse(LoadoutSerializer.ToJson(grid, profile.HeroName, 1200,
                new LoadoutMeta { Sect = profile.SectId, TamPhapLevel = profile.TamPhapLevel }));
            JArray entries = (JArray)json["items"];
            for (int i = 0; i < profile.Loadout.Count; i++)
                entries[i]["rarity"] = profile.RequireItem(profile.Loadout[i].Uid).Rarity;
            return json.ToString();
        }

        private void EnsureProfile() { if (Profile == null) Profile = new PlayerProfile(); }

        private void ImportEngineLoadout(string json)
        {
            Profile.Loadout.Clear();
            if (string.IsNullOrWhiteSpace(json)) return;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            BagGrid grid = LoadoutSerializer.FromJson(json, catalog, out LoadoutMeta meta);
            Profile.SectId = meta.Sect;
            Profile.TamPhapLevel = meta.TamPhapLevel;
            var used = new HashSet<string>(StringComparer.Ordinal);
            foreach (PlacedItem placed in grid.Items)
            {
                OwnedItem owned = Profile.Items.Find(item => item.ItemId == placed.Item.Id &&
                    item.Tier == placed.Tier && !used.Contains(item.Uid));
                owned ??= Profile.AddItem(placed.Item.Id, placed.Tier);
                used.Add(owned.Uid);
                Profile.Loadout.Add(new LoadoutPlacement { Uid = owned.Uid, X = placed.X, Y = placed.Y });
            }
        }
    }
}
