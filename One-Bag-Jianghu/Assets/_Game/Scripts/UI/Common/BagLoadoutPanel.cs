using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Economy;
using UnityEngine;
using UnityEngine.UI;
using BattleConfig = OneBagJianghu.Engine.BattleConfig;
using BattleSimulator = OneBagJianghu.Engine.BattleSimulator;
using EngineItemCatalog = OneBagJianghu.Engine.ItemCatalog;
using EnginePlacedItem = OneBagJianghu.Engine.Loadout.PlacedItem;
using FighterStats = OneBagJianghu.Engine.FighterStats;
using Loadout = OneBagJianghu.Engine.Loadout;
using Rarity = OneBagJianghu.Engine.Rarity;

namespace OneBagJianghu.UI.Common
{
    public sealed class BagLoadoutPanel : MonoBehaviour
    {
        public enum ItemSource
        {
            OwnedItems,
            FullCatalogDeveloper
        }

        [SerializeField]
        private Dropdown sectDropdown;
        [SerializeField]
        private Text levelText;
        [SerializeField]
        private Button decreaseLevelButton;
        [SerializeField]
        private Button increaseLevelButton;
        [SerializeField]
        private Text statsText;
        [SerializeField]
        private bool showSectSelector = true;
        [SerializeField]
        private BagGridView gridView;
        [SerializeField]
        private ItemTrayView trayView;
        [SerializeField]
        private ItemSource itemSource = ItemSource.OwnedItems;
        private ItemCatalog itemCatalog;
        private SectCatalog sectCatalog;
        private int tamPhapLevel;
        private string selectedSectId;
        private PlayerProfile boundProfile;
        private BagGrid statsGrid;
        private EngineItemCatalog engineItems;
        private OneBagJianghu.Engine.SectCatalog engineSects;
        public string SelectedSectId => selectedSectId;
        public int TamPhapLevel => tamPhapLevel;

        public event Action<ItemInfo, OwnedItem> ItemSelected;
        public bool IsEmpty => gridView.Grid.Items.Count == 0;

        public bool HasWeapon
        {
            get
            {
                foreach (PlacedItem placed in gridView.Grid.Items)
                    if (string.Equals(placed.Item.Type, "weapon", StringComparison.OrdinalIgnoreCase))
                        return true;
                return false;
            }
        }

        public RectTransform StorageGridRect => (RectTransform)trayView.transform;

        public void SetTooltip(ItemTooltip tooltip)
        {
            gridView.Tooltip = tooltip;
            trayView.Tooltip = tooltip;
            gridView.TooltipText = placed => Describe(placed.Item, placed.OwnedUid, placed.Tier);
            trayView.TooltipText = entry => Describe(entry.Item, entry.OwnedUid, entry.Tier);
        }

        private string Describe(ItemInfo info, string uid, int tier)
        {
            OwnedItem owned = boundProfile?.FindItem(uid);
            return ItemDescriber.Describe(info.Id, owned?.Rarity ?? info.Rarity, owned?.Tier ?? tier);
        }

        private string OwnedRarity(string uid, string fallback) => boundProfile?.FindItem(uid)?.Rarity ?? fallback;
        public bool TryMoveStorageToBag(string uid, int x, int y)
        {
            if (boundProfile == null)
                return false;
            OwnedItem owned = boundProfile.FindItem(uid);
            if (owned == null
|| !itemCatalog.TryGet(owned.ItemId,
out ItemInfo info)
|| !gridView.Grid.CanPlace(info,
x,
y))
                return false;
            gridView.Grid.Place(info, x, y, owned.Tier, uid);
            return true;
        }

        public bool TryMoveBagToStorage(string uid, int x, int y)
        {
            foreach (PlacedItem placed in gridView.Grid.Items)
                if (placed.OwnedUid == uid && trayView.TryAcceptAt(placed, x, y))
                    return gridView.Grid.Remove(placed.InstanceId);
            return false;
        }

        private void Awake()
        {
            sectDropdown.onValueChanged.AddListener(OnSectChanged);
            decreaseLevelButton.onClick.AddListener(() => ChangeLevel(-1));
            increaseLevelButton.onClick.AddListener(() => ChangeLevel(1));
            sectDropdown.gameObject.SetActive(showSectSelector);
            levelText.gameObject.SetActive(showSectSelector);
            decreaseLevelButton.gameObject.SetActive(showSectSelector);
            increaseLevelButton.gameObject.SetActive(showSectSelector);
        }

        public void Bind(string sectId, int level, string loadoutJson)
        {
            boundProfile = null;
            itemCatalog = ItemCatalog.LoadFromResources();
            BindCore(sectId, level, LoadGrid(loadoutJson), null, ItemSource.FullCatalogDeveloper);
        }

        public void Bind(PlayerProfile profile)
        {
            boundProfile = profile ?? throw new ArgumentNullException(nameof(profile));
            itemCatalog = ItemCatalog.LoadFromResources();
            profile.EnsureStorage(itemCatalog);
            BindCore(profile.SectId, profile.TamPhapLevel, LoadOwnedGrid(profile), null, itemSource);
            sectDropdown.gameObject.SetActive(false);
            levelText.gameObject.SetActive(!string.IsNullOrWhiteSpace(profile.SectId));
            decreaseLevelButton.gameObject.SetActive(false);
            increaseLevelButton.gameObject.SetActive(false);
            if (itemSource == ItemSource.OwnedItems)
                trayView.InitializeStorage(profile, itemCatalog, gridView);
        }

        private void BindCore(string sectId, int level, BagGrid grid, IReadOnlyList<TrayItem> owned, ItemSource source)
        {
            itemCatalog = ItemCatalog.LoadFromResources();
            sectCatalog = SectCatalog.LoadFromResources();
            selectedSectId = string.IsNullOrWhiteSpace(sectId) ? showSectSelector
&& sectCatalog.All.Count > 0 ? sectCatalog.All[0].Id : null : sectId;
            tamPhapLevel = Mathf.Clamp(level, 1, 9);
            var options = new List<Dropdown.OptionData>();
            foreach (SectInfo sect in sectCatalog.All)
                options.Add(new Dropdown.OptionData(sect.Name));
            sectDropdown.options = options;
            gridView.Initialize(grid, trayView);
            if (statsGrid != null)
                statsGrid.Changed -= RefreshStats;
            statsGrid = grid;
            statsGrid.Changed += RefreshStats;
            gridView.Rarity = placed => OwnedRarity(placed.OwnedUid, placed.Item.Rarity);
            gridView.ItemSelected = OnItemSelected;
            trayView.ItemSelected = OnItemSelected;
            if (source == ItemSource.FullCatalogDeveloper)
                trayView.Initialize(itemCatalog, gridView);
            else
                trayView.Initialize(owned ?? Array.Empty<TrayItem>(), gridView);
            int selected = FindSectIndex(sectId);
            sectDropdown.SetValueWithoutNotify(selected);
            sectDropdown.RefreshShownValue();
            ApplySect(selectedSectId);
            RefreshLevel();
            RefreshStats();
        }

        public string ToJson(string heroName) => LoadoutSerializer.ToJson(gridView.Grid,
heroName,
boundProfile?.HeroLevel ?? 1,
new LoadoutMeta
{
    Sect = selectedSectId,
    TamPhapLevel = tamPhapLevel
});
        public void ApplyToProfile()
        {
            if (!TryApplyToProfile(out string error))
                throw new InvalidOperationException(error);
        }

        public bool TryApplyToProfile(out string error)
        {
            if (boundProfile == null)
                throw new InvalidOperationException("The bag panel is not bound to a player profile.");
            trayView.ApplyStorage(boundProfile.Storage);
            // Sect choice is owned by SectSelectPopup and is permanent; the bag only consumes it.
            boundProfile.Loadout.Clear();
            foreach (PlacedItem placed in gridView.Grid.Items)
            {
                if (string.IsNullOrWhiteSpace(placed.OwnedUid))
                    throw new InvalidOperationException($"Placed item '{placed.Item.Id}' has no owned uid.");
                boundProfile.Loadout.Add(new LoadoutPlacement { Uid = placed.OwnedUid, X = placed.X, Y = placed.Y });
            }

            boundProfile.Validate();
            var summary = new List<string>();
            foreach (PlacedItem placed in gridView.Grid.Items)
            {
                OwnedItem owned = boundProfile.RequireItem(placed.OwnedUid);
                summary.Add($"{owned.ItemId}({owned.Tier} Trọng/{owned.Rarity})");
            }

            Debug.Log("[Bag] Loadout saved: " + string.Join(", ", summary));
            error = string.Empty;
            return true;
        }

        private BagGrid LoadOwnedGrid(PlayerProfile profile)
        {
            var grid = new BagGrid(5, 4);
            foreach (LoadoutPlacement placement in profile.Loadout)
            {
                OwnedItem owned = profile.RequireItem(placement.Uid);
                if (!itemCatalog.TryGet(owned.ItemId, out ItemInfo info))
                    throw new FormatException($"Owned item '{owned.Uid}' has unknown catalog id '{owned.ItemId}'.");
                if (!grid.CanPlace(info, placement.X, placement.Y))
                    throw new FormatException(
                        $"Owned item '{owned.Uid}' cannot be placed at ({placement.X}, {placement.Y}).");
                grid.Place(info, placement.X, placement.Y, owned.Tier, owned.Uid);
            }

            return grid;
        }

        private BagGrid LoadGrid(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new BagGrid(5, 4);
            try
            {
                BagGrid grid = LoadoutSerializer.FromJson(json, itemCatalog, out LoadoutMeta meta);
                tamPhapLevel = meta.TamPhapLevel;
                return grid.Cols == 5 && grid.Rows == 4 ? grid : new BagGrid(5, 4);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not restore session loadout: " + exception.Message);
                return new BagGrid(5, 4);
            }
        }

        private int FindSectIndex(string id)
        {
            for (int i = 0; i < sectCatalog.All.Count; i++)
                if (sectCatalog.All[i].Id == id)
                    return i;
            return 0;
        }

        private void OnItemSelected(ItemInfo info, string uid, int tier)
        {
            ItemSelected?.Invoke(info, boundProfile?.FindItem(uid));
        }

        private void OnSectChanged(int index)
        {
            if (sectCatalog == null || sectCatalog.All.Count == 0)
                return;
            selectedSectId = sectCatalog.All[index].Id;
            ApplySect(selectedSectId);
        }

        private void ApplySect(string id)
        {
            SectInfo sect = null;
            if (!string.IsNullOrWhiteSpace(id))
                sectCatalog.TryGet(id, out sect);
            trayView.SetSect(sect);
            gridView.SetSect(sect);
        }

        private void ChangeLevel(int amount)
        {
            tamPhapLevel = Mathf.Clamp(tamPhapLevel + amount, 1, 9);
            RefreshLevel();
        }

        private void RefreshLevel() => levelText.text = "Tâm Pháp cấp " + tamPhapLevel;
        private void RefreshStats()
        {
            if (statsText == null || statsGrid == null)
                return;
            if (engineItems == null)
            {
                engineItems = EngineItemCatalog.Load(Resources.Load<TextAsset>("Data/items").text);
                engineSects = OneBagJianghu.Engine.SectCatalog.Load(Resources.Load<TextAsset>("Data/sects").text);
            }

            int level = boundProfile?.HeroLevel ?? 1;
            FighterStats stats = new BattleSimulator(engineItems,
engineSects,
BattleConfig.Defaults()).FighterStats(BuildStatsLoadout(statsGrid,
level,
selectedSectId,
tamPhapLevel,
boundProfile));
            statsText.text = "Cấp "
+ level
+ "\nKhí Huyết tối đa: "
+ stats.maxHp
+ "\nHộ Thân: "
+ (stats.dmgReducePermille / 10f).ToString("0.#")
+ "% (≤ 80%)"
+ "\nGiảm Hồi Chiêu: "
+ (stats.cooldownReductionPermille / 10f).ToString("0.#")
+ "% (≤ 40%)";
        }

        public static Loadout BuildStatsLoadout(BagGrid grid,
int level,
string sectId,
int tamPhapLevel,
PlayerProfile profile = null)
        {
            var loadout = new Loadout
            {
                name = "Bao Phục",
                level = level,
                cols = grid.Cols,
                rows = grid.Rows,
                sect = string.IsNullOrWhiteSpace(sectId) ? null : sectId,
                tamPhapLevel = tamPhapLevel
            };
            foreach (PlacedItem placed in grid.Items)
            {
                OwnedItem owned = profile?.FindItem(placed.OwnedUid);
                string rarityName = owned?.Rarity ?? placed.Item.Rarity;
                if (!Enum.TryParse(rarityName, true, out Rarity rarity))
                    rarity = Rarity.COMMON;
                loadout.items.Add(new EnginePlacedItem
                {
                    id = placed.Item.Id,
                    x = placed.X,
                    y = placed.Y,
                    tier = owned?.Tier ?? placed.Tier,
                    rarity = rarity
                });
            }

            return loadout;
        }

        private void OnDestroy()
        {
            if (statsGrid != null)
                statsGrid.Changed -= RefreshStats;
        }
    }
}
