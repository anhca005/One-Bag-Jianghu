using System;
using System.Collections.Generic;
using System.IO;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Net;
using UnityEngine;

namespace OneBagJianghu.Core
{
    public enum GameMode
    {
        Pve,
        Pvp,
        ArenaReplay
    }

    [Obsolete("Profiles are server-authoritative. This type only removes obsolete local save files.")]
    public static class ProfileStore
    {
        public static string SaveDirectory => Application.persistentDataPath;

        public static List<string> DeleteSaveData()
        {
            string[] names = { "profile.json", "profile.json.bak", "profile.json.tmp", "loadout.json" };
            var deleted = new List<string>();
            foreach (string name in names)
            {
                string path = Path.Combine(SaveDirectory, name);
                if (!File.Exists(path))
                {
                    continue;
                }

                File.Delete(path);
                deleted.Add(name);
            }

            return deleted;
        }
    }

    public sealed class GameSession : MonoBehaviour
    {
        public PlayerProfile Profile
        {
            get;
            private set;
        }

        public string HeroName
        {
            get => Profile?.HeroName ?? string.Empty;
        }

        public string SectId
        {
            get => Profile?.SectId;
        }

        public int TamPhapLevel
        {
            get => Profile?.TamPhapLevel ?? 1;
        }

        public int HeroLevel
        {
            get => Profile?.HeroLevel ?? 1;
        }

        public string CurrentLoadoutJson
        {
            get => Profile == null ? string.Empty : BuildEngineLoadoutJson(Profile, ItemCatalog.LoadFromResources());
        }
        public long ProfileRevision
        {
            get;
            private set;
        }

        public GameMode SelectedMode
        {
            get;
            set;
        } = GameMode.Pve;
        public string CurrentPveStageId
        {
            get;
            set;
        }
        public string PendingPveMapStageId
        {
            get;
            set;
        }
        public bool OpenArenaOnHome
        {
            get;
            set;
        }
        public bool AutoFindArenaMatch
        {
            get;
            set;
        }
        public ArenaResult PendingArenaResult
        {
            get;
            private set;
        }
        public ArenaReplayResult PendingArenaReplay
        {
            get;
            private set;
        }
        public JObject PendingPveBattle
        {
            get;
            private set;
        }

        public void StorePveBattle(JObject payload)
        {
            PendingPveBattle = payload;
        }

        public JObject TakePveBattle()
        {
            JObject payload = PendingPveBattle;
            PendingPveBattle = null;
            return payload;
        }

        public void StoreArenaResult(ArenaResult result)
        {
            PendingArenaResult = result;
            PendingArenaReplay = null;
        }

        public ArenaResult TakeArenaResult()
        {
            ArenaResult result = PendingArenaResult;
            PendingArenaResult = null;
            return result;
        }

        public void StoreArenaReplay(ArenaReplayResult replay)
        {
            PendingArenaReplay = replay;
            PendingArenaResult = null;
        }

        public ArenaReplayResult TakeArenaReplay()
        {
            ArenaReplayResult replay = PendingArenaReplay;
            PendingArenaReplay = null;
            return replay;
        }

        public bool TryValidateLoadout(out string warning)
        {
            warning = "Bao Phục chưa có binh khí";
            if (Profile == null || Profile.Loadout.Count == 0)
                return false;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            foreach (LoadoutPlacement placement in Profile.Loadout)
            {
                OwnedItem owned = Profile.FindItem(placement.Uid);
                ItemInfo info;
                if (owned != null
                    && catalog.TryGet(owned.ItemId,
                    out info)
                    && string.Equals(info.Type,
                    "weapon",
                    StringComparison.OrdinalIgnoreCase))
                {
                    warning = string.Empty;
                    return true;
                }
            }

            warning = "Bao Phục chưa có binh khí";
            return false;
        }

        public void LoadProfile()
        {
            Profile = null;
            ProfileRevision = 0;
        }

        /// <summary>Replaces the in-memory profile with the authoritative server snapshot.</summary>
        public void ApplyServerProfile(JToken json, long revision)
        {
            if (json == null || json.Type == JTokenType.Null)
            {
                return;
            }

            Profile = PlayerProfile.FromJson(json.ToString(Newtonsoft.Json.Formatting.None),
                ItemCatalog.LoadFromResources());
            Profile.EnsureStorage(ItemCatalog.LoadFromResources());
            ProfileRevision = revision;
            PlayerProfile.NotifyProgressChanged();
        }

        /// <summary>Applies an in-memory profile snapshot for tests without writing local save data.</summary>
        public void ApplyProfileForTests(PlayerProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            ApplyServerProfile(JToken.Parse(profile.ToJson()), ProfileRevision);
        }

        public int PlayerPower()
        {
            EnsureProfile();
            var items = new List<OwnedItem>(Profile.Loadout.Count);
            foreach (LoadoutPlacement placement in Profile.Loadout)
                items.Add(Profile.RequireItem(placement.Uid));
            return PowerCatalog.LoadFromResources().Fighter(items, Profile.HeroLevel);
        }

        public static MaterialCatalog LoadMaterialCatalog()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/materials");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/materials.json.");
            return MaterialCatalog.Parse(asset.text);
        }

        public static string BuildEngineLoadoutJson(PlayerProfile profile, ItemCatalog catalog)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));
            var grid = new BagGrid(5, 4);
            foreach (LoadoutPlacement placement in profile.Loadout)
            {
                OwnedItem owned = profile.RequireItem(placement.Uid);
                ItemInfo info = catalog.Get(owned.ItemId);
                if (!grid.CanPlace(info, placement.X, placement.Y))
                    throw new FormatException(
                        $"Owned item '{owned.Uid}' cannot be placed at ({placement.X}, {placement.Y}).");
                grid.Place(info, placement.X, placement.Y, owned.Tier, owned.Uid);
            }

            JObject json = JObject.Parse(LoadoutSerializer.ToJson(grid,
                profile.HeroName,
                profile.HeroLevel,
                new LoadoutMeta
                {
                    Sect = profile.SectId,
                    TamPhapLevel = profile.TamPhapLevel
                }));
            JArray entries = (JArray)json["items"];
            for (int i = 0; i < profile.Loadout.Count; i++)
                entries[i]["rarity"] = profile.RequireItem(profile.Loadout[i].Uid).Rarity;
            return json.ToString();
        }

        private void EnsureProfile()
        {
            if (Profile == null)
                Profile = new PlayerProfile();
        }

    }
}
