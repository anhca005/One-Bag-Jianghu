using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private UiButton closeButton;
        private EconomyRuntime catalog; private MaterialCatalog materials;
        protected override void Awake()
        {
            base.Awake(); EnsureInitialized();
        }
        private void EnsureInitialized()
        {
            if (catalog != null) return; catalog = new EconomyRuntime(); materials = GameSession.LoadMaterialCatalog();
            itemDropdown.ClearOptions(); var ids = new System.Collections.Generic.List<string>(); foreach (OneBagJianghu.Bag.ItemInfo item in catalog.All) ids.Add(item.Id); itemDropdown.AddOptions(ids);
            materialDropdown.ClearOptions(); ids.Clear(); foreach (MaterialInfo item in materials.All) ids.Add(item.Id); materialDropdown.AddOptions(ids);
            addItemButton.onClick.AddListener(AddItem); add10Button.onClick.AddListener(() => AddMaterial(10)); add100Button.onClick.AddListener(() => AddMaterial(100));
            addSilverButton.onClick.AddListener(() => AddCurrency("bac", 10000)); addReputationButton.onClick.AddListener(() => AddCurrency("danh_vong", 1000));
            refreshShopButton.onClick.AddListener(RefreshShop); nextWeekButton.onClick.AddListener(() => { ShopRuntimeClock.AdvanceDays(7); Debug.Log("[Cheat] Advance shop clock by 7 days"); Save(); });
            resetButton.onClick.AddListener(ConfirmReset); closeButton.onClick.AddListener(Close);
        }
        protected override void OnSetup(object data) { EnsureInitialized(); }
        private void RefreshShop() { var runtime = new EconomyRuntime(); var service = new ShopService(EconomyRuntime.Load("shop", ShopCatalog.Parse), materials, runtime, GameBootstrap.Session.HeroName.GetHashCode(), new ShopRuntimeClock()); service.ForceFreeRefresh(GameBootstrap.Session.Profile); Debug.Log("[Cheat] Force free shop refresh"); Save(); }
        private void AddItem() { PlayerProfile profile = GameBootstrap.Session.Profile; OwnedItem item = profile.AddItem(itemDropdown.options[itemDropdown.value].text, tierDropdown.value + 1, rarity: RarityRules.Ordered[rarityDropdown.value]); if (!new StorageGrid(profile, catalog).TryPlaceItem(item)) { profile.RemoveItem(item.Uid); Debug.LogWarning("[Cheat] Add item failed: storage full"); ShowFull(); return; } Debug.Log($"[Cheat] Add item id={item.ItemId}, rarity={item.Rarity}, Trọng={item.Tier}"); Save(); }
        private void AddMaterial(int amount) { string id = materialDropdown.options[materialDropdown.value].text; if (!new StorageGrid(GameBootstrap.Session.Profile, catalog).TryAddMaterial(id, amount)) { Debug.LogWarning($"[Cheat] Add material failed: id={id}, amount={amount}, storage full"); ShowFull(); return; } Debug.Log($"[Cheat] Add material id={id}, amount={amount}"); Save(); }
        private static void ShowFull() => PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", "Tàng Bảo Các không đủ chỗ."));
        private void AddCurrency(string id, long amount) { GameBootstrap.Session.Profile.AddCurrency(id, amount); Debug.Log($"[Cheat] Add currency id={id}, amount={amount}"); Save(); }
        private static void Save() => GameBootstrap.Session.SaveProfile();
        private void ConfirmReset() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Đặt lại hồ sơ", "Khôi phục bộ vật phẩm khởi đầu?", "Đặt lại", "Hủy", yes => { if (yes) { Debug.Log("[Cheat] Reset profile"); GameBootstrap.Session.ResetProfile(); Close(); } }));
    }
#else
    public sealed class DevCheatPanel : BasePopup { protected override void OnSetup(object data) { } }
#endif
}
