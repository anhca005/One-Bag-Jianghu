using System;
using System.Collections.Generic;
using System.Text;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Audio;

namespace OneBagJianghu.UI.Popups
{
    public sealed class StoragePopup : BasePopup
    {
        private const float Pitch = 80f;
        [SerializeField]
        private Text headerText;
        [SerializeField]
        private Text currencyText;
        [SerializeField]
        private Text reputationText;
        [SerializeField]
        private Text overflowText;
        [SerializeField]
        private Text detailText;
        [SerializeField]
        private Text costText;
        [SerializeField]
        private Dropdown typeFilter;
        [SerializeField]
        private ScrollRect gridScroll;
        [SerializeField]
        private RectTransform gridContent;
        [SerializeField]
        private RectTransform equippedContent;
        [SerializeField]
        private StorageCell cellPrefab;
        [SerializeField]
        private StorageItemView storageItemPrefab;
        [SerializeField]
        private UiButton sortButton;
        [SerializeField]
        private UiButton upgradeButton;
        [SerializeField]
        private UiButton mergeButton;
        [SerializeField]
        private UiButton sellButton;
        [SerializeField]
        private UiButton closeButton;
        private readonly List<GameObject> rows = new List<GameObject>();
        private readonly List<GameObject> equippedRows = new List<GameObject>();
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
            if (catalog != null)
                return;
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
            CurrencyArt.Apply(currencyText, "icon_bac");
            CurrencyArt.Apply(reputationText, "icon_danh_vong");
            EnsureInitialized();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.EnsureStorage(catalog);
            grid = new StorageGrid(profile, catalog);
            EnsureCells(profile.Storage);
            foreach (GameObject row in rows)
                Destroy(row);
            rows.Clear();
            foreach (GameObject row in equippedRows)
                Destroy(row);
            equippedRows.Clear();
            string filter = typeFilter.options[typeFilter.value].text;
            foreach (StorageItemPlacement placement in profile.Storage.Items)
            {
                OwnedItem item = profile.FindItem(placement.Uid);
                if (item == null)
                    continue;
                ItemInfo info = catalog.Get(item.ItemId);
                StorageItemView view = Instantiate(storageItemPrefab, gridContent);
                view.gameObject.SetActive(true);
                rows.Add(view.gameObject);
                Sprite sprite = null;
                if (icons != null)
                    icons.TryGet(item.ItemId, out sprite);
                float alpha = filter == "Tất cả" || filter == TypeName(info.Type) ? 1f : .25f;
                view.Bind(this,
                    item.Uid,
                    false,
                    info.Name,
                    "T"
                    + item.Tier,
                    sprite,
                    BagViewStyle.Rarity(item.Rarity),
                    alpha,
                    item.Rarity);
                SetRect((RectTransform)view.transform, placement.X, placement.Y, info.Width, info.Height);
            }

            RefreshEquippedItems(profile);

            foreach (MaterialStack stack in profile.Storage.MaterialStacks)
            {
                StorageItemView view = Instantiate(storageItemPrefab, gridContent);
                view.gameObject.SetActive(true);
                rows.Add(view.gameObject);
                Sprite materialSprite = null;
                if (icons != null)
                    icons.TryGet(stack.MaterialId, out materialSprite);
                view.Bind(this,
                    Key(stack),
                    true,
                    MaterialName(stack.MaterialId),
                    "×"
                    + stack.Count,
                    materialSprite,
                    new Color(.58f,
                    .43f,
                    .18f),
                    filter == "Tất cả" ? 1f : .25f);
                SetRect((RectTransform)view.transform, stack.X, stack.Y, 1, 1);
            }

            profile.Currencies.TryGetValue("bac", out long silver);
            profile.Currencies.TryGetValue("danh_vong", out long reputation);
            currencyText.text = $"       Bạc {UiFormat.Number(silver)}";
            reputationText.text = $"       Danh Vọng {UiFormat.Number(reputation)}";
            headerText.text = $"Tàng Bảo Các  {grid.UsedCells}/{profile.Storage.Cols * profile.Storage.Rows}";
            currencyText.text = currencyText.text.TrimStart();
            reputationText.text = reputationText.text.TrimStart();
            overflowText.gameObject.SetActive(profile.Storage.Overflow.Count > 0);
            overflowText.text =
                profile.Storage.Overflow.Count > 0
                ? "Cảnh báo: có dữ liệu đang chờ trong overflow."
                : string.Empty;
            if (selected != null)
                Select(selected.Uid);
            else if (selectedStack != null)
                SelectView(Key(selectedStack), true);
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
            if (selectedStack == null)
                return;
            MaterialInfo info = materials.Get(selectedStack.MaterialId);
            detailText.text = ItemDescriber.DescribeMaterial(info, selectedStack.Count);
            costText.text = string.Empty;
            SetActions(false);
        }

        public void PreviewDrop(string key, bool isMaterial, Vector2 point, Camera camera)
        {
            ClearDropPreview();
            int width = 1;
            int height = 1;
            string ignoredUid = null;
            MaterialStack ignoredStack = null;
            if (isMaterial)
                ignoredStack = FindStack(key);
            else
            {
                ignoredUid = key;
                OwnedItem item = GameBootstrap.Session.Profile.FindItem(key);
                if (item == null)
                    return;
                catalog.GetItemSize(item.ItemId, out width, out height);
            }

            if (!ToCell(point, camera, width, height, out int x, out int y))
                return;
            bool valid = grid.CanPlace(x, y, width, height, ignoredUid, ignoredStack);
            StorageState storage = GameBootstrap.Session.Profile.Storage;
            for (int row = y; row < y + height; row++)
                for (int column = x; column < x + width; column++)
                    if (column >= 0 && row >= 0 && column < storage.Cols && row < storage.Rows)
                        cells[row * storage.Cols + column].SetHighlight(valid);
        }

        public bool CommitDrop(string key, bool isMaterial, Vector2 point, Camera camera)
        {
            int width = 1, height = 1;
            if (!isMaterial)
            {
                OwnedItem item = GameBootstrap.Session.Profile.FindItem(key);
                if (item == null)
                    return false;
                catalog.GetItemSize(item.ItemId, out width, out height);
            }

            if (!ToCell(point, camera, width, height, out int x, out int y))
            {
                Debug.LogWarning($"[Storage] Move refused: {key} -> outside grid");
                return false;
            }

            bool moved = isMaterial ? grid.TryMoveStack(FindStack(key), x, y) : grid.TryMoveItem(key, x, y);
            if (moved)
            {
                GameBootstrap.Session.SaveProfile();
                Debug.Log($"[Storage] Move {key} -> ({x},{y})");
                Refresh();
            }
            else
                Debug.LogWarning($"[Storage] Move refused: {key} -> ({x},{y}), reason=occupied or invalid");
            return moved;
        }

        public void ClearDropPreview()
        {
            foreach (StorageCell cell in cells)
                cell.Clear();
        }

        private void Select(string uid)
        {
            mergeSourceUid = null;
            selected = GameBootstrap.Session.Profile.FindItem(uid);
            if (selected == null)
                return;
            ItemInfo info = catalog.Get(selected.ItemId);
            UpgradePreview preview = upgrade.Preview(GameBootstrap.Session.Profile, uid);
            string currentPower = FormatPower(selected.Rarity, selected.Tier);
            string nextPower = preview.NextTier > 0 ? FormatPower(selected.Rarity, preview.NextTier) : currentPower;
            detailText.text = ItemDescriber.Describe(selected.ItemId,
                selected.Rarity,
                selected.Tier)
                + $"\nKích thước: {info.Width}×{info.Height}";
            costText.text = BuildUpgradeCost(preview);
            upgradeButton.GetComponent<Button>().interactable = preview.Success;
            mergeButton.GetComponent<Button>().interactable = true;
            sellButton.GetComponent<Button>().interactable = true;
        }

        private void RefreshEquippedItems(PlayerProfile profile)
        {
            int index = 0;
            foreach (LoadoutPlacement placement in profile.Loadout)
            {
                OwnedItem item = profile.FindItem(placement.Uid);
                if (item == null)
                    continue;
                ItemInfo info = catalog.Get(item.ItemId);
                StorageItemView view = Instantiate(storageItemPrefab, equippedContent);
                view.gameObject.SetActive(true);
                equippedRows.Add(view.gameObject);
                Sprite sprite = null;
                if (icons != null)
                    icons.TryGet(item.ItemId, out sprite);
                view.Bind(this,
                    item.Uid,
                    false,
                    info.Name,
                    "T" + item.Tier,
                    sprite,
                    BagViewStyle.Rarity(item.Rarity),
                    1f,
                    item.Rarity,
                    false);
                SetEquippedRect((RectTransform)view.transform, index);
                index++;
            }

            equippedContent.sizeDelta = new Vector2(Mathf.Max(index * Pitch, Pitch), 0f);
        }

        private string BuildUpgradeCost(UpgradePreview preview)
        {
            if (preview.NextTier <= 0)
                return preview.Error;
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.Currencies.TryGetValue("bac", out long silver);
            var text = new StringBuilder();
            text.Append("Nâng lên ").Append(preview.NextTier).Append(" Trọng\n");
            bool silverEnough = silver >= preview.Silver;
            if (!silverEnough)
                text.Append("<color=#B8322A>");
            text.Append("Bạc ").Append(UiFormat.Number(silver)).Append('/').Append(UiFormat.Number(preview.Silver));
            if (!silverEnough)
                text.Append("</color>");
            foreach (KeyValuePair<string, int> cost in preview.Materials)
            {
                profile.Materials.TryGetValue(cost.Key, out int owned);
                text.Append('\n');
                if (owned < cost.Value)
                    text.Append("<color=#B8322A>");
                text
                    .Append(MaterialName(cost
                    .Key))
                    .Append(' ')
                    .Append(UiFormat.Number(owned))
                    .Append('/')
                    .Append(UiFormat.Number(cost.Value));
                if (owned < cost.Value)
                    text.Append("</color>");
            }

            if (preview.UnlocksBreakthrough)
                text.Append("\n• mở Đột Phá");
            if (!preview.Success && !string.IsNullOrEmpty(preview.Error))
                text.Append('\n').Append(preview.Error);
            return text.ToString();
        }

        private bool ToCell(Vector2 point, Camera camera, int width, int height, out int x, out int y)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridContent,
                point,
                camera,
                out Vector2 localPoint))
            {
                x = y = -1;
                return false;
            }

            x = DragGridMath.OriginCell(localPoint.x, Pitch, width);
            y = DragGridMath.OriginCell(-localPoint.y, Pitch, height);
            StorageState storage = GameBootstrap.Session.Profile.Storage;
            return x >= 0 && y >= 0 && x < storage.Cols && y < storage.Rows;
        }

        private void EnsureCells(StorageState storage)
        {
            if (cells.Count > 0)
                return;
            gridContent.sizeDelta = new Vector2(storage.Cols * Pitch, storage.Rows * Pitch);
            foreach (StorageCell cell in gridContent.GetComponentsInChildren<StorageCell>(true))
                cells.Add(cell);
            if (cells.Count == storage.Cols * storage.Rows)
                return;
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
                if (item != null)
                    items.Add(item);
            }

            items.Sort(CompareItems);
            // Total each material once, then re-add: this also merges partial stacks. Clear
            // profile.Materials too, or the new StorageGrid would treat the empty stack list as a
            // legacy profile and recreate every material a second time (the old duplication bug).
            var order = new List<string>();
            var totals = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (MaterialStack stack in profile.Storage.MaterialStacks)
            {
                if (!totals.ContainsKey(stack.MaterialId))
                {
                    order.Add(stack.MaterialId);
                    totals[stack.MaterialId] = 0;
                }

                totals[stack.MaterialId] += stack.Count;
            }

            int stacks = profile.Storage.MaterialStacks.Count;
            profile.Storage.Items.Clear();
            profile.Storage.MaterialStacks.Clear();
            profile.Materials.Clear();
            grid = new StorageGrid(profile, catalog);
            foreach (OwnedItem item in items)
                grid.TryPlaceItem(item);
            foreach (string id in order)
                if (!grid.TryAddMaterial(id, totals[id]))
                    profile.Storage.Overflow.Add("material:" + id + ":" + totals[id]);
            GameBootstrap.Session.SaveProfile();
            Debug
                .Log(
                    $"[Storage] Repack items={items.Count}, " +
                    $"materialStacks={stacks}->{profile.Storage.MaterialStacks.Count}");
            Refresh();
        }

        private int CompareItems(OwnedItem first, OwnedItem second)
        {
            ItemInfo a = catalog.Get(first.ItemId);
            ItemInfo b = catalog.Get(second.ItemId);
            int comparison = string.Compare(a.Type, b.Type, StringComparison.Ordinal);
            if (comparison != 0)
                return comparison;
            comparison = RarityRules.Index(second.Rarity).CompareTo(RarityRules.Index(first.Rarity));
            return comparison != 0 ? comparison : second.Tier.CompareTo(first.Tier);
        }

        private void Upgrade()
        {
            if (selected == null)
                return;
            OwnedItem item = selected;
            int fromTier = item.Tier;
            UpgradePreview preview = upgrade.Preview(GameBootstrap.Session.Profile, item.Uid);
            Complete(upgrade.Apply(GameBootstrap.Session.Profile,
                item.Uid),
                $"upgrade item={item.ItemId}, rarity={item.Rarity}, "
                + $"Trọng={fromTier}->{preview.NextTier}, cost={preview.Silver}");
        }

        private void BeginMerge()
        {
            if (selected == null)
                return;
            if (merge.Candidates(GameBootstrap.Session.Profile, selected.Uid).Count == 0)
            {
                AudioManager.Instance?.Play("ui_error");
                costText.text = "Không có bản sao hợp lệ để Hợp Thành.";
                Debug.LogWarning($"[Storage] merge item={selected.ItemId}, result=failed, reason=no valid candidate");
                return;
            }

            mergeSourceUid = selected.Uid;
            costText.text = "Chọn một bản sao hợp lệ để Hợp Thành.";
        }

        private void ChooseMergeCandidate(string candidateUid)
        {
            string sourceUid = mergeSourceUid;
            OwnedItem candidate = null;
            foreach (OwnedItem item in merge.Candidates(GameBootstrap.Session.Profile, sourceUid))
                if (item.Uid == candidateUid)
                    candidate = item;
            if (candidate == null)
            {
                costText.text = "Bản sao đã chọn không hợp lệ để Hợp Thành.";
                Debug.LogWarning(
                    $"[Storage] merge inputs={sourceUid}+{candidateUid}, " +
                    "result=failed, reason=invalid candidate");
                return;
            }

            mergeSourceUid = null;
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Hợp Thành",
                "Dùng bản sao "
                + candidate.Tier
                + " Trọng?",
                "Hợp Thành",
                "Hủy",
                yes =>
            {
                if (yes)
                {
                    OwnedItem source = GameBootstrap.Session.Profile.FindItem(sourceUid);
                    MergePreview preview = merge.Preview(GameBootstrap.Session.Profile, sourceUid, candidateUid);
                    Complete(
                        merge.Apply(
                            GameBootstrap.Session.Profile,
                            sourceUid,
                            candidateUid),
                        $"merge inputs={sourceUid}+{candidateUid}, " +
                        $"output={source?.ItemId}/{preview.OutputRarity}/{preview.OutputTier} Trọng, " +
                        $"refund={FormatMaterials(preview.RefundMaterials)}");
                }
            }));
        }

        private void Sell()
        {
            if (selected == null)
                return;
            SellPreview preview = sell.Preview(GameBootstrap.Session.Profile, selected.Uid);
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Bán vật phẩm",
                $"Nhận {UiFormat.Number(preview.Silver)} Bạc?",
                "Bán",
                "Hủy",
                yes =>
            {
                if (yes)
                    Complete(sell.Apply(GameBootstrap.Session.Profile,
                        selected.Uid),
                        $"sell item={selected.ItemId}, price={preview.Silver}");
            }));
        }

        private void Complete(EconomyResult result, string action)
        {
            if (!result.Success)
            {
                AudioManager.Instance?.Play("ui_error");
                costText.text = result.Error;
                Debug.LogWarning($"[Storage] {action}, result=failed, reason={result.Error}");
                return;
            }

            if (action.StartsWith("upgrade", StringComparison.Ordinal))
            {
                AudioManager.Instance?.Play("upgrade");
                HomeController home = FindObjectOfType<HomeController>();
                home?.RefreshPower();
            }
            else if (action.StartsWith("merge", StringComparison.Ordinal))
                AudioManager.Instance?.Play("merge");
            else if (action.StartsWith("sell", StringComparison.Ordinal))
                AudioManager.Instance?.Play("coin");
            Debug.Log($"[Storage] {action}, result=success");
            // Keep the upgraded item selected so the detail refreshes and it can be upgraded again.
            bool keepSelection = action.StartsWith("upgrade", StringComparison.Ordinal);
            if (!keepSelection)
                selected = null;
            selectedStack = null;
            mergeSourceUid = null;
            GameBootstrap.Session.SaveProfile();
            if (selected == null)
                ClearSelection();
            Refresh();
        }

        private void ClearSelection()
        {
            detailText.text = string.Empty;
            costText.text = string.Empty;
            SetActions(false);
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
            foreach (KeyValuePair<string, int> value in values)
                result.Add(value.Key + "x" + value.Value);
            return string.Join(",", result);
        }

        private MaterialStack FindStack(string key)
        {
            foreach (MaterialStack stack in GameBootstrap.Session.Profile.Storage.MaterialStacks)
                if (Key(stack) == key)
                    return stack;
            return null;
        }

        private string MaterialName(string id)
        {
            try
            {
                return materials.Get(id).Name;
            }
            catch (KeyNotFoundException)
            {
                return id;
            }
        }

        private static string Key(MaterialStack stack) => stack.MaterialId + "@" + stack.X + "," + stack.Y;
        private static bool IsEquipped(PlayerProfile profile, string uid)
        {
            return profile.Loadout.Exists(placement =>
                string.Equals(placement.Uid, uid, StringComparison.Ordinal));
        }
        private static void SetEquippedRect(RectTransform rect, int index)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, .5f);
            rect.pivot = new Vector2(0f, .5f);
            rect.anchoredPosition = new Vector2(index * Pitch, 0f);
            rect.sizeDelta = new Vector2(Pitch - 4f, Pitch - 4f);
        }
        private static string Short(string value) => value.Length <= 15 ? value : value.Substring(0, 14) + "…";
        private static string FormatPower(string rarity,
            int tier) => (RarityRules.PowerPermille(rarity,
            tier) / 10f).ToString("0.#");
        private static void SetRect(RectTransform rect, int x, int y, int width, int height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x * Pitch, -y * Pitch);
            rect.sizeDelta = new Vector2(width * Pitch - 4f, height * Pitch - 4f);
        }

        private static string TypeName(string type)
            => type == "weapon"
            ? "Binh Khí"
            : type == "armor"
            ? "Hộ Cụ"
            : type == "manual"
            ? "Công Pháp"
            : type == "pill"
            ? "Đan Dược"
            : "Kỳ Trân";
    }
}
