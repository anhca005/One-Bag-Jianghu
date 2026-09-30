using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class ShopRuntimeClock : IShopClock
    {
        private static TimeSpan offset;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow + offset;
        public static void AdvanceDays(int days) => offset += TimeSpan.FromDays(days);
    }

    public sealed class ShopPopup : BasePopup
    {
        [SerializeField] private Text headerText; [SerializeField] private Text statusText;
        [SerializeField] private UiButton tradeTab; [SerializeField] private UiButton exchangeTab; [SerializeField] private UiButton reputationTab;
        [SerializeField] private GameObject tradePanel; [SerializeField] private GameObject exchangePanel; [SerializeField] private GameObject reputationPanel;
        [SerializeField] private RectTransform slotList; [SerializeField] private ShopCard cardPrefab; [SerializeField] private UiButton refreshButton;
        [SerializeField] private Dropdown inputDropdown; [SerializeField] private Dropdown targetDropdown; [SerializeField] private Text exchangePreview; [SerializeField] private UiButton exchangeButton;
        [SerializeField] private RectTransform reputationList; [SerializeField] private UiButton closeButton;
        private readonly List<GameObject> rows = new List<GameObject>();
        private readonly List<GameObject> tradeCards = new List<GameObject>();
        private readonly List<GameObject> reputationCards = new List<GameObject>();
        private EconomyRuntime catalog; private MaterialCatalog materials; private ShopCatalog config; private ShopService shop; private ItemIconLibrary icons;
        private int loggedGeneration = -1;

        protected override void Awake() { base.Awake(); EnsureInitialized(); }
        private void EnsureInitialized()
        {
            if (shop != null) return;
            catalog = new EconomyRuntime(); materials = EconomyRuntime.Load("materials", MaterialCatalog.Parse); config = EconomyRuntime.Load("shop", ShopCatalog.Parse);
            shop = new ShopService(config, materials, catalog, GameBootstrap.Session.HeroName.GetHashCode(), new ShopRuntimeClock()); icons = ItemIconLibrary.Load();
            tradeTab.onClick.AddListener(() => ShowTab(0)); exchangeTab.onClick.AddListener(() => ShowTab(1)); reputationTab.onClick.AddListener(() => ShowTab(2)); refreshButton.onClick.AddListener(RefreshShop);
            exchangeButton.onClick.AddListener(Exchange); inputDropdown.onValueChanged.AddListener(_ => UpdateExchange()); targetDropdown.onValueChanged.AddListener(_ => UpdateExchange()); closeButton.onClick.AddListener(Close);
            var names = new List<string>(); foreach (MaterialInfo value in materials.All) if (!value.IsCatalyst) names.Add(value.Id); inputDropdown.ClearOptions(); inputDropdown.AddOptions(names); targetDropdown.ClearOptions(); targetDropdown.AddOptions(names);
        }
        protected override void OnSetup(object data) { EnsureInitialized(); tradePanel.SetActive(true); exchangePanel.SetActive(false); reputationPanel.SetActive(false); Refresh(); }
        private void ShowTab(int value) { tradePanel.SetActive(value == 0); exchangePanel.SetActive(value == 1); reputationPanel.SetActive(value == 2); if (shop == null) return; if (value == 0) BuildSlots(); else if (value == 2) BuildReputation(); }
        public override void Refresh()
        {
            EnsureInitialized(); PlayerProfile profile = GameBootstrap.Session.Profile; shop.Ensure(profile); profile.Currencies.TryGetValue("bac", out long silver); profile.Currencies.TryGetValue("danh_vong", out long reputation);
            TimeSpan left = shop.UntilFreeRefresh(profile); headerText.text = $"Bạc: {silver:N0}   Danh Vọng: {reputation:N0}   Miễn phí sau {left.Hours:00}:{left.Minutes:00}:{left.Seconds:00}";
            refreshButton.SetLabel(left == TimeSpan.Zero ? "Làm mới miễn phí" : $"Làm mới • {shop.PaidRefreshCost(profile):N0} Bạc"); BuildSlots(); UpdateExchange(); if (reputationPanel.activeSelf) BuildReputation();
        }
        private void BuildSlots()
        {
            Clear(tradeCards); IReadOnlyList<ShopSlotState> slots = shop.Ensure(GameBootstrap.Session.Profile); GameBootstrap.Session.Profile.Currencies.TryGetValue("bac", out long silver);
            if (loggedGeneration != GameBootstrap.Session.Profile.Shop.Generation)
            {
                var contents = new List<string>();
                foreach (ShopSlotState value in slots) contents.Add(value.Kind + ":" + value.ContentId + "x" + value.Quantity + "@" + value.Price);
                loggedGeneration = GameBootstrap.Session.Profile.Shop.Generation;
                Debug.Log($"[Shop] Slots generated generation={loggedGeneration}, contents={string.Join(",", contents)}");
            }
            for (int i = 0; i < slots.Count; i++)
            {
                ShopSlotState slot = slots[i]; ShopCard card = Instantiate(cardPrefab, slotList); card.gameObject.SetActive(true); tradeCards.Add(card.gameObject); rows.Add(card.gameObject);
                bool item = slot.Kind == "item"; string name; string type; string detail; Sprite icon = null;
                if (item) { ItemInfo info = catalog.Get(slot.ContentId); name = info.Name; type = TypeName(info.Type); detail = RarityName(slot.Rarity) + " • 1 Trọng"; if (icons != null) icons.TryGet(slot.ContentId, out icon); }
                else { MaterialInfo info = materials.Get(slot.ContentId); name = info.Name; type = TypeName(info.ItemType); detail = "×" + slot.Quantity; if (icons != null) icons.TryGet(slot.ContentId, out icon); }
                ShopSlotRule rule = i < config.Slots.Count ? config.Slots[i] : null; int discount = rule == null ? 0 : rule.DiscountPercent; long oldPrice = discount > 0 && discount < 100 ? (long)Math.Round(slot.Price * 100d / (100 - discount)) : 0;
                bool space = shop.HasStorageSpace(GameBootstrap.Session.Profile, slot);
                int index = i; card.Setup(name, detail, type, $"Bạc  {slot.Price:N0}", oldPrice > slot.Price ? $"{oldPrice:N0}" : null, oldPrice > slot.Price ? $"-{discount}%" : null, item ? slot.Rarity : null, icon, silver >= slot.Price && space, slot.Bought, () => ConfirmBuy(index, name, slot.Price), space ? null : "Tàng Bảo Các không đủ chỗ.");
            }
        }
        private void BuildReputation()
        {
            Clear(reputationCards); GameBootstrap.Session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            foreach (ReputationOffer offer in config.ReputationOffers)
            {
                ShopCard card = Instantiate(cardPrefab, reputationList); card.gameObject.SetActive(true); reputationCards.Add(card.gameObject); rows.Add(card.gameObject); int remaining = shop.ReputationRemaining(GameBootstrap.Session.Profile, offer.Id); string id = offer.Id;
                card.Setup(offer.Name, "×" + offer.Quantity, $"còn {remaining}/{offer.Limit} mỗi tuần", $"Danh Vọng  {offer.Price:N0}", null, null, null, null, remaining > 0 && reputation >= offer.Price, remaining <= 0, () => ConfirmReputation(id, offer.Name, offer.Price));
            }
        }
        private void Clear(List<GameObject> cards) { foreach (GameObject value in cards) { rows.Remove(value); Destroy(value); } cards.Clear(); }
        private void ConfirmBuy(int index, string itemName, long price) => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Xác nhận mua", $"Mua {itemName} với giá {price:N0} Bạc?", "Mua", "Hủy", yes => { if (yes) Complete(shop.Buy(GameBootstrap.Session.Profile, index), $"buy slot={index}, content={itemName}, price={price}"); }));
        private void ConfirmReputation(string id, string itemName, int price) => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Xác nhận mua", $"Mua {itemName} với giá {price:N0} Danh Vọng?", "Mua", "Hủy", yes => { if (yes) Complete(shop.BuyReputation(GameBootstrap.Session.Profile, id), $"reputation purchase id={id}, content={itemName}, price={price}"); }));
        private void RefreshShop() { bool free = shop.UntilFreeRefresh(GameBootstrap.Session.Profile) == TimeSpan.Zero; int cost = free ? 0 : shop.PaidRefreshCost(GameBootstrap.Session.Profile); Complete(shop.Refresh(GameBootstrap.Session.Profile, free), $"{(free ? "free" : "paid")} refresh cost={cost}"); }
        private void Exchange() { string input = inputDropdown.options[inputDropdown.value].text, target = targetDropdown.options[targetDropdown.value].text; Complete(shop.Exchange(GameBootstrap.Session.Profile, input, target), $"exchange {input}->{target}"); }
        private void UpdateExchange() { if (shop == null || inputDropdown.options.Count == 0) return; MaterialInfo a = materials.Get(inputDropdown.options[inputDropdown.value].text), b = materials.Get(targetDropdown.options[targetDropdown.value].text); bool same = a.ItemType == b.ItemType && b.Tier == a.Tier + 1, cross = a.ItemType != b.ItemType && a.Tier == b.Tier; exchangePreview.text = same ? $"3 {a.Name} → 1 {b.Name}" : cross ? $"2 {a.Name} → 1 {b.Name}" : "Lựa chọn không hợp lệ"; exchangeButton.GetComponent<Button>().interactable = same || cross; }
        private void Complete(EconomyResult result, string action) { statusText.text = result.Success ? "Giao dịch thành công." : result.Error; if (result.Success) { Debug.Log($"[Shop] {action}, result=success"); GameBootstrap.Session.SaveProfile(); } else Debug.LogWarning($"[Shop] {action}, result=failed, reason={result.Error}"); Refresh(); }
        private static string RarityName(string value) { string[] names = { "Phàm", "Lương", "Tinh", "Tuyệt", "Thần Binh", "Chí Tôn" }; return names[RarityRules.Index(value)]; }
        private static string TypeName(string type) => type == "weapon" ? "Binh Khí" : type == "armor" ? "Hộ Cụ" : type == "manual" ? "Công Pháp" : type == "pill" || type == "pill_relic" ? "Đan Dược / Kỳ Trân" : type == "relic" ? "Kỳ Trân" : "Nguyên liệu";
    }
}
