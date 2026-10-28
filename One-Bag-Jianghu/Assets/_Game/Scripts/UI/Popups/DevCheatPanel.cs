using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Pve;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.UI.Popups
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public sealed class DevCheatPanel : BasePopup
    {
        [SerializeField] private Dropdown itemDropdown;
        [SerializeField] private Dropdown rarityDropdown;
        [SerializeField] private Dropdown tierDropdown;
        [SerializeField] private Dropdown materialDropdown;
        [SerializeField] private UiButton addItemButton;
        [SerializeField] private UiButton add10Button;
        [SerializeField] private UiButton add100Button;
        [SerializeField] private UiButton addSilverButton;
        [SerializeField] private UiButton addReputationButton;
        [SerializeField] private UiButton refreshShopButton;
        [SerializeField] private UiButton nextWeekButton;
        [SerializeField] private UiButton resetButton;
        [SerializeField] private UiButton wipeAllButton;
        [SerializeField] private UiButton closeButton;
        [SerializeField] private UiButton unlockPveButton;
        [SerializeField] private UiButton refillStaminaButton;
        [SerializeField] private UiButton clearStageButton;
        [SerializeField] private UiButton addLevelButton;
        [SerializeField] private UiButton addExpButton;
        [SerializeField] private UiButton resetSectButton;
        private EconomyRuntime catalog;
        private MaterialCatalog materials;
        private readonly System.Collections.Generic.List<string> itemIds =
            new System.Collections.Generic.List<string>();
        private readonly System.Collections.Generic.List<string> materialIds =
            new System.Collections.Generic.List<string>();
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
            materials = GameSession.LoadMaterialCatalog();
            itemDropdown.ClearOptions();
            var names = new System.Collections.Generic.List<string>();
            foreach (OneBagJianghu.Bag.ItemInfo item in catalog.All)
            {
                itemIds.Add(item.Id);
                names.Add(item.Name);
            }
            itemDropdown.AddOptions(names);
            materialDropdown.ClearOptions();
            names.Clear();
            foreach (MaterialInfo item in materials.All)
            {
                materialIds.Add(item.Id);
                names.Add(item.Name);
            }
            materialDropdown.AddOptions(names);
            addItemButton.onClick.AddListener(AddItem);
            add10Button.onClick.AddListener(() => AddMaterial(10));
            add100Button.onClick.AddListener(() => AddMaterial(100));
            addSilverButton.onClick.AddListener(() => AddCurrency("bac",
                10000));
            addReputationButton.onClick.AddListener(() => AddCurrency("danh_vong",
            1000));
            refreshShopButton.onClick.AddListener(RefreshShop);
            nextWeekButton.onClick.AddListener(() =>
            {
                SendCheat(new JObject { ["action"] = "advance_week" });
            });
            resetButton.onClick.AddListener(ConfirmReset);
            wipeAllButton.onClick.AddListener(ConfirmWipeAll);
            closeButton.onClick.AddListener(Close);
            unlockPveButton.onClick.AddListener(UnlockPve);
            refillStaminaButton.onClick.AddListener(RefillStamina);
            clearStageButton.onClick.AddListener(ClearStage);
            addLevelButton.onClick.AddListener(AddLevel);
            addExpButton.onClick.AddListener(AddExp);
            resetSectButton.onClick.AddListener(ResetSect);
        }
        protected override void OnSetup(object data)
        {
            EnsureInitialized();
        }
        private void RefreshShop()
        {
            SendCheat(new JObject { ["action"] = "refresh_shop" });
        }
        private void AddItem()
        {
            SendCheat(new JObject
            {
                ["action"] = "add_item",
                ["itemId"] = itemIds[itemDropdown.value],
                ["tier"] = tierDropdown.value + 1,
                ["rarity"] = RarityRules.Ordered[rarityDropdown.value]
            });
        }
        private void AddMaterial(int amount)
        {
            string id = materialIds[materialDropdown.value];
            SendCheat(new JObject { ["action"] = "add_material", ["materialId"] = id, ["amount"] = amount });
        }
        private static void ShowFull() => PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo",
            "Tàng Bảo Các không đủ chỗ."));
        private void AddCurrency(string id,
            long amount)
        {
            SendCheat(new JObject { ["action"] = "add_currency", ["currencyId"] = id, ["amount"] = amount });
        }
        private static void UnlockPve()
        {
            SendCheat(new JObject { ["action"] = "unlock_pve" });
        }
        private static void RefillStamina()
        {
            SendCheat(new JObject { ["action"] = "refill_stamina" });
        }
        private static void ClearStage()
        {
            string id = GameBootstrap.Session.CurrentPveStageId;
            if (string.IsNullOrEmpty(id))
                id = PveCatalog.Load().Chapters[0].Stages[0].Id;
            SendCheat(new JObject { ["action"] = "clear_stage", ["stageId"] = id });
        }
        private static void AddLevel()
        {
            SendCheat(new JObject { ["action"] = "add_level" });
        }
        private static void AddExp()
        {
            SendCheat(new JObject { ["action"] = "add_exp", ["amount"] = 100 });
        }
        private static void ResetSect()
        {
            SendCheat(new JObject { ["action"] = "reset_sect" });
        }
        private void ConfirmReset() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Đặt lại hồ sơ",
            "Khôi phục bộ vật phẩm khởi đầu?",
            "Đặt lại",
            "Hủy",
            yes =>
            {
                if (yes)
                {
                    SendCheat(new JObject { ["action"] = "reset_profile" });
                }
            }));
        private void ConfirmWipeAll() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Chơi lại từ đầu",
            "Xóa toàn bộ dữ liệu và quay về màn đăng nhập?",
            "Xóa hết",
            "Hủy",
            yes =>
        {
            if (!yes)
                return;
            SendCheat(new JObject { ["action"] = "reset_profile" });
        }));

        private static void SendCheat(JObject payload)
        {
            GameBootstrap.Network.SendDevCheat(payload,
                _ => { },
                message => PopupManager.Show<MessagePopup>(
                    new MessagePopupData("Dev Cheat", message, "Đóng")));
        }
    }
#else
    public sealed class DevCheatPanel : BasePopup
    {
        protected override void OnSetup(object data)
        {
        }
    }
#endif
}
