using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Pve;

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
            var ids = new System.Collections.Generic.List<string>();
            foreach (OneBagJianghu.Bag.ItemInfo item in catalog.All)
                ids.Add(item.Id);
            itemDropdown.AddOptions(ids);
            materialDropdown.ClearOptions();
            ids.Clear();
            foreach (MaterialInfo item in materials.All)
                ids.Add(item.Id);
            materialDropdown.AddOptions(ids);
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
                ShopRuntimeClock.AdvanceDays(7);
                Debug.Log("[Cheat] Advance shop clock by 7 days");
                Save();
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
            var runtime = new EconomyRuntime();
            var service = new ShopService(EconomyRuntime.Load("shop",
            ShopCatalog.Parse),
            materials,
            runtime,
            GameBootstrap.Session.HeroName.GetHashCode(),
            new ShopRuntimeClock());
            service.ForceFreeRefresh(GameBootstrap.Session.Profile);
            Debug.Log("[Cheat] Force free shop refresh");
            Save();
        }
        private void AddItem()
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            OwnedItem item = profile.AddItem(itemDropdown.options[itemDropdown.value].text,
            tierDropdown.value
            + 1,
            rarity: RarityRules.Ordered[rarityDropdown.value]);
            if (!new StorageGrid(profile,
            catalog).TryPlaceItem(item))
            {
                profile.RemoveItem(item.Uid);
                Debug.LogWarning("[Cheat] Add item failed: storage full");
                ShowFull();
                return;
            }
            Debug.Log($"[Cheat] Add item id={item.ItemId}, rarity={item.Rarity}, Trọng={item.Tier}");
            Save();
        }
        private void AddMaterial(int amount)
        {
            string id = materialDropdown.options[materialDropdown.value].text;
            if (!new StorageGrid(GameBootstrap.Session.Profile,
            catalog).TryAddMaterial(id,
            amount))
            {
                Debug.LogWarning($"[Cheat] Add material failed: id={id}, amount={amount}, storage full");
                ShowFull();
                return;
            }
            Debug.Log($"[Cheat] Add material id={id}, amount={amount}");
            Save();
        }
        private static void ShowFull() => PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo",
"Tàng Bảo Các không đủ chỗ."));
        private void AddCurrency(string id,
long amount)
        {
            GameBootstrap.Session.Profile.AddCurrency(id,
amount);
            Debug.Log($"[Cheat] Add currency id={id}, amount={amount}");
            Save();
        }
        private static void Save() => GameBootstrap.Session.SaveProfile();
        private static void UnlockPve()
        {
            PveCatalog c = PveCatalog.Load();
            foreach (PveChapter chapter in c.Chapters)
                foreach (PveStage stage in chapter.Stages)
                    GameBootstrap.Session.Profile.Pve.Stages[stage.Id] = new PveStageProgress
                    {
                        Cleared = true,
                        BestStars = GameBootstrap.Session.Profile.Pve.Stages.TryGetValue(stage.Id,
                out PveStageProgress old) ? old.BestStars : 0
                    };
            Debug.Log("[Pve] [Cheat] Unlock all stages");
            Save();
        }
        private static void RefillStamina()
        {
            new StaminaService(StaminaConfig.Load()).Refill(GameBootstrap.Session.Profile.Pve,
System.DateTimeOffset.UtcNow);
            Debug.Log("[Pve] [Cheat] Refill stamina");
            Save();
        }
        private static void ClearStage()
        {
            string id = GameBootstrap.Session.CurrentPveStageId;
            if (string.IsNullOrEmpty(id))
                id = PveCatalog.Load().Chapters[0].Stages[0].Id;
            GameBootstrap.Session.Profile.Pve.Stages[id] = new PveStageProgress
            {
                Cleared = true,
                BestStars = 3
            };
            Debug.Log("[Pve] [Cheat] Clear stage="
            + id
            + " stars=3");
            Save();
        }
        private static void AddLevel()
        {
            PlayerProfile p = GameBootstrap.Session.Profile;
            LevelService s = new LevelService(LevelConfig.Load());
            if (p.HeroLevel < s.MaxLevel)
                s.Grant(p,
            s.ExpToNext(p.HeroLevel) - p.HeroExp);
            Debug.Log("[Cheat] Add 1 level");
            Save();
        }
        private static void AddExp()
        {
            new LevelService(LevelConfig.Load()).Grant(GameBootstrap.Session.Profile,
100);
            Debug.Log("[Cheat] Add EXP amount=100");
            Save();
        }
        private static void ResetSect()
        {
            PlayerProfile p = GameBootstrap.Session.Profile;
            p.SectId = null;
            p.TamPhapLevel = 1;
            p.SectPromptShown = false;
            Debug.Log("[Cheat] Reset sect");
            Save();
        }
        private void ConfirmReset() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Đặt lại hồ sơ",
"Khôi phục bộ vật phẩm khởi đầu?",
"Đặt lại",
"Hủy",
yes =>
{
    if (yes)
    {
        Debug.Log("[Cheat] Reset profile");
        GameBootstrap.Session.ResetProfile();
        Close();
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
            GameBootstrap.Session.WipeAllData();
            PopupManager.ClearCacheAfterDataWipe();
            SceneFlow.Go(GameScene.Login);
        }));
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