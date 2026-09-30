using System;
using System.Collections.Generic;
using System.Text;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class StoragePopup : BasePopup
    {
        private const float Pitch = 80f;

        [SerializeField] private Text headerText;
        [SerializeField] private Text currencyText;
        [SerializeField] private Text overflowText;
        [SerializeField] private Text detailText;
        [SerializeField] private Text costText;
        [SerializeField] private Dropdown typeFilter;
        [SerializeField] private ScrollRect gridScroll;
        [SerializeField] private RectTransform gridContent;
        [SerializeField] private StorageCell cellPrefab;
        [SerializeField] private StorageItemView storageItemPrefab;
        [SerializeField] private UiButton sortButton;
        [SerializeField] private UiButton upgradeButton;
        [SerializeField] private UiButton mergeButton;
        [SerializeField] private UiButton sellButton;
        [SerializeField] private UiButton closeButton;

        private readonly List<GameObject> rows = new List<GameObject>();
        private readonly List<StorageCell> cells = new List<StorageCell>();
        private EconomyRuntime catalog;
        private MaterialCatalog materials;
        private UpgradeService upgrade;
        private MergeService merge;
        private SellService sell;
        private ItemIconLibrary icons;
        private StorageGrid grid;
        private OwnedItem selected;
        private MaterialStack selectedStack;
        private string mergeSourceUid;

        protected override void Awake()
        {
            base.Awake();
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (catalog != null) return;
            catalog = new EconomyRuntime();
            icons = ItemIconLibrary.Load();
            materials = EconomyRuntime.Load("materials", MaterialCatalog.Parse);
            UpgradeCatalog costs = EconomyRuntime.Load("upgrade_costs", UpgradeCatalog.Parse);
            upgrade = new UpgradeService(costs, materials, catalog);
            merge = new MergeService(costs, EconomyRuntime.Load("merge", MergeCatalog.Parse), materials, catalog);
            sell = new SellService(costs, materials, catalog);
            typeFilter.onValueChanged.AddListener(_ => Refresh());
            sortButton.onClick.AddListener(Repack);
            upgradeButton.onClick.AddListener(Upgrade);
            mergeButton.onClick.AddListener(BeginMerge);
            sellButton.onClick.AddListener(Sell);
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            EnsureInitialized();
            Refresh();
        }

        public override void Refresh()
        {
            EnsureInitialized();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.EnsureStorage(catalog);
            grid = new StorageGrid(profile, catalog);
            EnsureCells(profile.Storage);
            foreach (GameObject row in rows) Destroy(row);
            rows.Clear();

            string filter = typeFilter.options[typeFilter.value].text;
            foreach (StorageItemPlacement placement in profile.Storage.Items)
            {
                OwnedItem item = profile.FindItem(placement.Uid);
                if (item == null) continue;
                ItemInfo info = catalog.Get(item.ItemId);
                StorageItemView view = Instantiate(storageItemPrefab, gridContent);
                view.gameObject.SetActive(true);
                rows.Add(view.gameObject);
                Sprite sprite = null;
                if (icons != null) icons.TryGet(item.ItemId, out sprite);
                float alpha = filter == "Tất cả" || filter == TypeName(info.Type) ? 1f : .25f;
                view.Bind(this, item.Uid, false, Short(info.Name), item.Tier + " Trọng", sprite,
                    RarityColor(item.Rarity), alpha);
                SetRect((RectTransform)view.transform, placement.X, placement.Y, info.Width, info.Height);
            }

            foreach (MaterialStack stack in profile.Storage.MaterialStacks)
            {
                StorageItemView view = Instantiate(storageItemPrefab, gridContent);
                view.gameObject.SetActive(true);
                rows.Add(view.gameObject);
                Sprite materialSprite = null;
                if (icons != null) icons.TryGet(stack.MaterialId, out materialSprite);
                view.Bind(this, Key(stack), true, MaterialName(stack.MaterialId), "×" + stack.Count, materialSprite,
                    new Color(.58f, .43f, .18f), filter == "Tất cả" ? 1f : .25f);
                SetRect((RectTransform)view.transform, stack.X, stack.Y, 1, 1);
            }

            profile.Currencies.TryGetValue("bac", out long silver);
            profile.Currencies.TryGetValue("danh_vong", out long reputation);
            currencyText.text = $"Bạc: {silver:N0}   Danh Vọng: {reputation:N0}";
            headerText.text = $"Tàng Bảo Các  {grid.UsedCells}/{profile.Storage.Cols * profile.Storage.Rows}";
            overflowText.gameObject.SetActive(profile.Storage.Overflow.Count > 0);
            overflowText.text = profile.Storage.Overflow.Count > 0
                ? "Cảnh báo: có dữ liệu đang chờ trong overflow." : string.Empty;
            if (selected != null) Select(selected.Uid);
            else if (selectedStack != null) SelectView(Key(selectedStack), true);
        }

        public void SelectView(string key, bool isMaterial)
        {
            if (!isMaterial)
            {
                if (!string.IsNullOrEmpty(mergeSourceUid) && key != mergeSourceUid)
                {
                    ChooseMergeCandidate(key);
                    return;
                }
                selectedStack = null;
                Select(key);
                return;
            }

            mergeSourceUid = null;
            selected = null;
            selectedStack = FindStack(key);
            if (selectedStack == null) return;
            MaterialInfo info = materials.Get(selectedStack.MaterialId);
            detailText.text = ItemDescriber.DescribeMaterial(info, selectedStack.Count);
            costText.text = string.Empty;
            SetActions(false);
        }

        public void PreviewDrop(string key, bool isMaterial, Vector2 point, Camera camera)
        {
            ClearDropPreview();
            if (!ToCell(point, camera, out int x, out int y)) return;
            int width = 1;
            int height = 1;
            string ignoredUid = null;
            MaterialStack ignoredStack = null;
            if (isMaterial) ignoredStack = FindStack(key);
            else
            {
                ignoredUid = key;
                OwnedItem item = GameBootstrap.Session.Profile.FindItem(key);
                if (item == null) return;
                catalog.GetItemSize(item.ItemId, out width, out height);
            }
            bool valid = grid.CanPlace(x, y, width, height, ignoredUid, ignoredStack);
            StorageState storage = GameBootstrap.Session.Profile.Storage;
            for (int row = y; row < y + height; row++)
            for (int column = x; column < x + width; column++)
                if (column >= 0 && row >= 0 && column < storage.Cols && row < storage.Rows)
                    cells[row * storage.Cols + column].SetHighlight(valid);
        }

        public bool CommitDrop(string key, bool isMaterial, Vector2 point, Camera camera)
        {
            if (!ToCell(point, camera, out int x, out int y))
            {
                Debug.LogWarning($"[Storage] Move refused: {key} -> outside grid");
                return false;
            }
            bool moved = isMaterial
                ? grid.TryMoveStack(FindStack(key), x, y)
                : grid.TryMoveItem(key, x, y);
            if (moved)
            {
                GameBootstrap.Session.SaveProfile();
                Debug.Log($"[Storage] Move {key} -> ({x},{y})");
                Refresh();
            }
            else Debug.LogWarning($"[Storage] Move refused: {key} -> ({x},{y}), reason=occupied or invalid");
            return moved;
        }

        public void ClearDropPreview()
        {
            foreach (StorageCell cell in cells) cell.Clear();
        }

        private void Select(string uid)
        {
            mergeSourceUid = null;
            selected = GameBootstrap.Session.Profile.FindItem(uid);
            if (selected == null) return;
            ItemInfo info = catalog.Get(selected.ItemId);
            UpgradePreview preview = upgrade.Preview(GameBootstrap.Session.Profile, uid);
            string currentPower = FormatPower(selected.Rarity, selected.Tier);
            string nextPower = preview.NextTier > 0
                ? FormatPower(selected.Rarity, preview.NextTier) : currentPower;
            detailText.text = ItemDescriber.Describe(selected.ItemId, selected.Rarity, selected.Tier) +
                $"\nKích thước: {info.Width}×{info.Height}";
            costText.text = BuildUpgradeCost(preview);
            upgradeButton.GetComponent<Button>().interactable = preview.Success;
            mergeButton.GetComponent<Button>().interactable = true;
            sellButton.GetComponent<Button>().interactable = true;
        }

        private string BuildUpgradeCost(UpgradePreview preview)
        {
            if (preview.NextTier <= 0) return preview.Error;
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.Currencies.TryGetValue("bac", out long silver);
            var text = new StringBuilder();
            text.Append("Nâng lên ").Append(preview.NextTier).Append(" Trọng\n");
            text.Append("Bạc ").Append(silver.ToString("N0")).Append('/').Append(preview.Silver.ToString("N0"));
            foreach (KeyValuePair<string, int> cost in preview.Materials)
            {
                profile.Materials.TryGetValue(cost.Key, out int owned);
                text.Append('\n').Append(MaterialName(cost.Key)).Append(' ')
                    .Append(owned.ToString("N0")).Append('/').Append(cost.Value.ToString("N0"));
            }
            if (preview.UnlocksBreakthrough) text.Append("\n• mở Đột Phá");
            if (!preview.Success && !string.IsNullOrEmpty(preview.Error)) text.Append('\n').Append(preview.Error);
            return text.ToString();
        }

        private bool ToCell(Vector2 point, Camera camera, out int x, out int y)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridContent, point, camera,
                out Vector2 localPoint))
            {
                x = y = -1;
                return false;
            }
            x = Mathf.FloorToInt(localPoint.x / Pitch);
            y = Mathf.FloorToInt(-localPoint.y / Pitch);
            StorageState storage = GameBootstrap.Session.Profile.Storage;
            return x >= 0 && y >= 0 && x < storage.Cols && y < storage.Rows;
        }

        private void EnsureCells(StorageState storage)
        {
            if (cells.Count > 0) return;
            gridContent.sizeDelta = new Vector2(storage.Cols * Pitch, storage.Rows * Pitch);
            foreach (StorageCell cell in gridContent.GetComponentsInChildren<StorageCell>(true)) cells.Add(cell);
            if (cells.Count == storage.Cols * storage.Rows) return;
            cells.Clear();
            for (int y = 0; y < storage.Rows; y++)
            for (int x = 0; x < storage.Cols; x++)
            {
                StorageCell cell = Instantiate(cellPrefab, gridContent);
                cell.gameObject.SetActive(true);
                SetRect((RectTransform)cell.transform, x, y, 1, 1);
                cells.Add(cell);
            }
        }

        private void Repack()
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            var items = new List<OwnedItem>();
            foreach (StorageItemPlacement placement in profile.Storage.Items)
            {
                OwnedItem item = profile.FindItem(placement.Uid);
                if (item != null) items.Add(item);
            }
            items.Sort(CompareItems);
            var stacks = new List<MaterialStack>(profile.Storage.MaterialStacks);
            profile.Storage.Items.Clear();
            profile.Storage.MaterialStacks.Clear();
            grid = new StorageGrid(profile, catalog);
            foreach (OwnedItem item in items) grid.TryPlaceItem(item);
            foreach (MaterialStack stack in stacks)
            {
                if (!grid.TryFindFirst(1, 1, out int x, out int y)) continue;
                stack.X = x;
                stack.Y = y;
                profile.Storage.MaterialStacks.Add(stack);
            }
            GameBootstrap.Session.SaveProfile();
            Debug.Log($"[Storage] Repack items={items.Count}, materialStacks={stacks.Count}");
            Refresh();
        }

        private int CompareItems(OwnedItem first, OwnedItem second)
        {
            ItemInfo a = catalog.Get(first.ItemId);
            ItemInfo b = catalog.Get(second.ItemId);
            int comparison = string.Compare(a.Type, b.Type, StringComparison.Ordinal);
            if (comparison != 0) return comparison;
            comparison = RarityRules.Index(second.Rarity).CompareTo(RarityRules.Index(first.Rarity));
            return comparison != 0 ? comparison : second.Tier.CompareTo(first.Tier);
        }

        private void Upgrade()
        {
            if (selected == null) return;
            OwnedItem item = selected;
            int fromTier = item.Tier;
            UpgradePreview preview = upgrade.Preview(GameBootstrap.Session.Profile, item.Uid);
            Complete(upgrade.Apply(GameBootstrap.Session.Profile, item.Uid),
                $"upgrade item={item.ItemId}, rarity={item.Rarity}, Trọng={fromTier}->{preview.NextTier}, cost={preview.Silver}");
        }

        private void BeginMerge()
        {
            if (selected == null) return;
            if (merge.Candidates(GameBootstrap.Session.Profile, selected.Uid).Count == 0)
            {
                costText.text = "Không có bản sao hợp lệ để Hợp Thành.";
                Debug.LogWarning($"[Storage] merge item={selected.ItemId}, result=failed, reason=no valid candidate");
                return;
            }
            mergeSourceUid = selected.Uid;
            costText.text = "Chọn một bản sao hợp lệ trong Tàng Bảo Các để Hợp Thành.";
        }

        private void ChooseMergeCandidate(string candidateUid)
        {
            string sourceUid = mergeSourceUid;
            OwnedItem candidate = null;
            foreach (OwnedItem item in merge.Candidates(GameBootstrap.Session.Profile, sourceUid))
                if (item.Uid == candidateUid) candidate = item;
            if (candidate == null)
            {
                costText.text = "Bản sao đã chọn không hợp lệ để Hợp Thành.";
                Debug.LogWarning($"[Storage] merge inputs={sourceUid}+{candidateUid}, result=failed, reason=invalid candidate");
                return;
            }
            mergeSourceUid = null;
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Hợp Thành",
                "Dùng bản sao " + candidate.Tier + " Trọng?", "Hợp Thành", "Hủy", yes =>
                {
                    if (yes)
                    {
                        OwnedItem source = GameBootstrap.Session.Profile.FindItem(sourceUid);
                        MergePreview preview = merge.Preview(GameBootstrap.Session.Profile, sourceUid, candidateUid);
                        Complete(merge.Apply(GameBootstrap.Session.Profile, sourceUid, candidateUid),
                            $"merge inputs={sourceUid}+{candidateUid}, output={source?.ItemId}/{preview.OutputRarity}/{preview.OutputTier} Trọng, refund={FormatMaterials(preview.RefundMaterials)}");
                    }
                }));
        }

        private void Sell()
        {
            if (selected == null) return;
            SellPreview preview = sell.Preview(GameBootstrap.Session.Profile, selected.Uid);
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Bán vật phẩm",
                $"Nhận {preview.Silver:N0} Bạc?", "Bán", "Hủy", yes =>
                {
                    if (yes) Complete(sell.Apply(GameBootstrap.Session.Profile, selected.Uid),
                        $"sell item={selected.ItemId}, price={preview.Silver}");
                }));
        }

        private void Complete(EconomyResult result, string action)
        {
            if (!result.Success)
            {
                costText.text = result.Error;
                Debug.LogWarning($"[Storage] {action}, result=failed, reason={result.Error}");
                return;
            }
            Debug.Log($"[Storage] {action}, result=success");
            selected = null;
            selectedStack = null;
            mergeSourceUid = null;
            GameBootstrap.Session.SaveProfile();
            Refresh();
        }

        private void SetActions(bool value)
        {
            upgradeButton.GetComponent<Button>().interactable = value;
            mergeButton.GetComponent<Button>().interactable = value;
            sellButton.GetComponent<Button>().interactable = value;
        }

        private static string FormatMaterials(IReadOnlyDictionary<string, int> values)
        {
            var result = new List<string>();
            foreach (KeyValuePair<string, int> value in values) result.Add(value.Key + "x" + value.Value);
            return string.Join(",", result);
        }

        private MaterialStack FindStack(string key)
        {
            foreach (MaterialStack stack in GameBootstrap.Session.Profile.Storage.MaterialStacks)
                if (Key(stack) == key) return stack;
            return null;
        }

        private string MaterialName(string id)
        {
            try { return materials.Get(id).Name; }
            catch (KeyNotFoundException) { return id; }
        }

        private static string Key(MaterialStack stack) => stack.MaterialId + "@" + stack.X + "," + stack.Y;
        private static string Short(string value) => value.Length <= 15 ? value : value.Substring(0, 14) + "…";
        private static string FormatPower(string rarity, int tier) =>
            (RarityRules.PowerPermille(rarity, tier) / 10f).ToString("0.#");

        private static void SetRect(RectTransform rect, int x, int y, int width, int height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x * Pitch, -y * Pitch);
            rect.sizeDelta = new Vector2(width * Pitch - 4f, height * Pitch - 4f);
        }

        private static string TypeName(string type) => type == "weapon" ? "Binh Khí" :
            type == "armor" ? "Hộ Cụ" : type == "manual" ? "Công Pháp" :
            type == "pill" ? "Đan Dược" : "Kỳ Trân";

        private static string RarityName(string value)
        {
            string[] names = { "Phàm", "Lương", "Tinh", "Tuyệt", "Thần Binh", "Chí Tôn" };
            return names[RarityRules.Index(value)];
        }

        private static Color RarityColor(string value)
        {
            Color[] colors =
            {
                new Color(.72f, .72f, .68f), new Color(.35f, .68f, .4f),
                new Color(.3f, .55f, .85f), new Color(.63f, .38f, .78f),
                new Color(.86f, .62f, .2f), new Color(.75f, .16f, .12f)
            };
            return colors[RarityRules.Index(value)];
        }
    }
}
