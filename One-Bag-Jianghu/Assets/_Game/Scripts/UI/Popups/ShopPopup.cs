using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Audio;

namespace OneBagJianghu.UI.Popups
{
    public sealed class ShopRuntimeClock : IShopClock
    {
        private static TimeSpan offset;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow + offset;

        public static void AdvanceDays(int days) => offset += TimeSpan.FromDays(days);
        public static void Reset() => offset = TimeSpan.Zero;
    }

    public sealed class ShopPopup : BasePopup
    {
        [SerializeField]
        private Text headerText;
        [SerializeField]
        private Text silverText;
        [SerializeField]
        private Text reputationText;
        [SerializeField]
        private Text statusText;
        [SerializeField]
        private UiButton tradeTab;
        [SerializeField]
        private UiButton exchangeTab;
        [SerializeField]
        private UiButton reputationTab;
        [SerializeField]
        private GameObject tradePanel;
        [SerializeField]
        private GameObject exchangePanel;
        [SerializeField]
        private GameObject reputationPanel;
        [SerializeField]
        private RectTransform slotList;
        [SerializeField]
        private ShopCard cardPrefab;
        [SerializeField]
        private UiButton refreshButton;
        [SerializeField]
        private Dropdown inputDropdown;
        [SerializeField]
        private Dropdown targetDropdown;
        [SerializeField]
        private Text exchangePreview;
        [SerializeField]
        private UiButton exchangeButton;
        [SerializeField]
        private RectTransform reputationList;
        [SerializeField]
        private UiButton closeButton;
        [SerializeField]
        private ItemTooltip itemTooltip;
        private readonly List<GameObject> rows = new List<GameObject>();
        private readonly List<GameObject> tradeCards = new List<GameObject>();
        private readonly List<GameObject> reputationCards = new List<GameObject>();
        private readonly List<string> materialIds = new List<string>();
        private EconomyRuntime catalog;
        private MaterialCatalog materials;
        private ShopCatalog config;
        private ShopService shop;
        private ItemIconLibrary icons;
        private int loggedGeneration = -1;
        private Sprite primaryTabSprite;
        private Sprite secondaryTabSprite;
        private Color primaryTabColor;
        private Color secondaryTabColor;
        private Color primaryTabTextColor;
        private Color secondaryTabTextColor;
        protected override void Awake()
        {
            base.Awake();
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (shop != null)
                return;
            catalog = new EconomyRuntime(true);
            materials = EconomyRuntime.Load("materials", MaterialCatalog.Parse);
            config = EconomyRuntime.Load("shop", ShopCatalog.Parse);
            shop = new ShopService(config,
                materials,
                catalog,
                GameBootstrap.Session.HeroName.GetHashCode(),
                new ShopRuntimeClock());
            icons = ItemIconLibrary.Load();
            CaptureTabStyles();
            tradeTab.onClick.AddListener(() => ShowTab(0));
            exchangeTab.onClick.AddListener(() => ShowTab(1));
            reputationTab.onClick.AddListener(() => ShowTab(2));
            refreshButton.onClick.AddListener(RefreshShop);
            exchangeButton.onClick.AddListener(Exchange);
            inputDropdown.onValueChanged.AddListener(_ => UpdateExchange());
            targetDropdown.onValueChanged.AddListener(_ => UpdateExchange());
            closeButton.onClick.AddListener(Close);
            List<Dropdown.OptionData> options = BuildMaterialOptions(materials, icons);
            foreach (MaterialInfo value in materials.All)
            {
                if (!value.IsCatalyst)
                {
                    materialIds.Add(value.Id);
                }
            }
            inputDropdown.ClearOptions();
            inputDropdown.AddOptions(options);
            targetDropdown.ClearOptions();
            targetDropdown.AddOptions(options);
        }

        protected override void OnSetup(object data)
        {
            EnsureInitialized();
            tradePanel.SetActive(true);
            exchangePanel.SetActive(false);
            reputationPanel.SetActive(false);
            ShowTab(0);
            Refresh();
        }

        protected override void OnShown()
        {
            itemTooltip.EnsureAbove(GetComponent<Canvas>());
        }

        protected override void OnHidden()
        {
            itemTooltip.Hide();
        }

        private void ShowTab(int value)
        {
            itemTooltip?.Hide();
            tradePanel.SetActive(value == 0);
            exchangePanel.SetActive(value == 1);
            reputationPanel.SetActive(value == 2);
            SetTabStyle(tradeTab, value == 0);
            SetTabStyle(exchangeTab, value == 1);
            SetTabStyle(reputationTab, value == 2);
            if (shop == null)
                return;
            if (value == 0)
                BuildSlots();
            else if (value == 2)
                BuildReputation();
        }

        public override void Refresh()
        {
            itemTooltip?.Hide();
            EnsureInitialized();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            shop.Ensure(profile);
            profile.Currencies.TryGetValue("bac", out long silver);
            profile.Currencies.TryGetValue("danh_vong", out long reputation);
            TimeSpan left = shop.UntilFreeRefresh(profile);
            CurrencyArt.Apply(silverText, "icon_bac");
            CurrencyArt.Apply(reputationText, "icon_danh_vong");
            silverText.text = UiFormat.Number(silver);
            reputationText.text = UiFormat.Number(reputation);
            headerText.text = $"Miễn phí sau {left.Hours:00}:{left.Minutes:00}:{left.Seconds:00}";
            refreshButton.SetLabel(left == TimeSpan.Zero
                ? "Làm mới miễn phí"
                : $"Làm mới • {UiFormat.Number(shop.PaidRefreshCost(profile))} Bạc");
            BuildSlots();
            UpdateExchange();
            if (reputationPanel.activeSelf)
                BuildReputation();
        }

        private void BuildSlots()
        {
            Clear(tradeCards);
            IReadOnlyList<ShopSlotState> slots = shop.Ensure(GameBootstrap.Session.Profile);
            GameBootstrap.Session.Profile.Currencies.TryGetValue("bac", out long silver);
            if (loggedGeneration != GameBootstrap.Session.Profile.Shop.Generation)
            {
                var contents = new List<string>();
                foreach (ShopSlotState value in slots)
                    contents.Add(value.Kind + ":" + value.ContentId + "x" + value.Quantity + "@" + value.Price);
                loggedGeneration = GameBootstrap.Session.Profile.Shop.Generation;
                Debug.Log(
                    $"[Shop] Slots generated generation={loggedGeneration}, " +
                    $"contents={string.Join(",", contents)}");
            }

            for (int i = 0; i < slots.Count; i++)
            {
                ShopSlotState slot = slots[i];
                ShopCard card = Instantiate(cardPrefab, slotList);
                if (i == 0)
                {
                    TutorialTarget target = card.gameObject.AddComponent<TutorialTarget>();
                    target.Configure("shop_first_item");
                }
                card.gameObject.SetActive(true);
                tradeCards.Add(card.gameObject);
                rows.Add(card.gameObject);
                bool item = slot.Kind == "item";
                string name;
                string type;
                string detail;
                string stat;
                string tooltipText;
                Sprite icon = null;
                if (item)
                {
                    ItemInfo info = catalog.Get(slot.ContentId);
                    name = info.Name;
                    type = TypeName(info.Type);
                    const int tier = 1;
                    detail = BagViewStyle.RarityRichText(slot.Rarity) + " • " + tier + " Trọng";
                    stat = ItemDescriber.StatLine(slot.ContentId, slot.Rarity, tier);
                    int power = PowerCatalog.LoadFromResources().ItemPower(slot.ContentId, tier, slot.Rarity);
                    tooltipText = ItemTooltip.WithPower(
                        ItemDescriber.Describe(slot.ContentId, slot.Rarity, tier),
                        power);
                    if (icons != null)
                        icons.TryGet(slot.ContentId, out icon);
                }
                else
                {
                    MaterialInfo info = materials.Get(slot.ContentId);
                    name = info.Name;
                    type = TypeName(info.ItemType);
                    detail = "×" + slot.Quantity;
                    stat = string.Empty;
                    tooltipText = ItemDescriber.DescribeMaterial(info, slot.Quantity);
                    if (icons != null)
                        icons.TryGet(slot.ContentId, out icon);
                }

                ShopSlotRule rule = i < config.Slots.Count ? config.Slots[i] : null;
                int discount = rule == null ? 0 : rule.DiscountPercent;
                long oldPrice = discount > 0
                    && discount < 100 ? (long)Math.Round(slot.Price * 100d / (100 - discount)) : 0;
                bool space = shop.HasStorageSpace(GameBootstrap.Session.Profile, slot);
                int index = i;
                card.Setup(name,
                    detail,
                    stat,
                    type,
                    $"Bạc  {UiFormat.Number(slot.Price)}",
                    oldPrice > slot.Price ? UiFormat.Number(oldPrice) : null,
                    oldPrice > slot.Price ? $"-{discount}%" : null,
                    item ? slot.Rarity : null,
                    icon,
                    silver >= slot.Price
                    && space,
                    slot.Bought,
                    () => ConfirmBuy(index,
                    name,
                    slot.Price),
                    space ? null : "Tàng Bảo Các không đủ chỗ.");
                card.SetTooltip(itemTooltip, () => tooltipText);
            }
        }

        private void BuildReputation()
        {
            Clear(reputationCards);
            GameBootstrap.Session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            foreach (ReputationOffer offer in config.ReputationOffers)
            {
                ShopCard card = Instantiate(cardPrefab, reputationList);
                card.gameObject.SetActive(true);
                reputationCards.Add(card.gameObject);
                rows.Add(card.gameObject);
                int remaining = shop.ReputationRemaining(GameBootstrap.Session.Profile, offer.Id);
                string id = offer.Id;
                Sprite icon = FindIcon(id);
                card.Setup(offer.Name,
                    "×"
                    + offer.Quantity,
                    string.Empty,
                    $"còn {remaining}/{offer.Limit} mỗi tuần",
                    $"Danh Vọng  {UiFormat.Number(offer.Price)}",
                    null,
                    null,
                    null,
                    icon,
                    remaining > 0
                    && reputation >= offer.Price,
                    remaining <= 0,
                    () => ConfirmReputation(id,
                    offer.Name,
                    offer.Price));
                card.SetTooltip(itemTooltip, () => DescribeReputationOffer(offer));
            }
        }

        private void Clear(List<GameObject> cards)
        {
            foreach (GameObject value in cards)
            {
                rows.Remove(value);
                Destroy(value);
            }

            cards.Clear();
        }

        private void ConfirmBuy(int index, string itemName, long price)
        {
            itemTooltip.Hide();
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Xác nhận mua",
                $"Mua {itemName} với giá {UiFormat.Number(price)} Bạc?",
                "Mua",
                "Hủy",
                yes =>
                {
                    if (yes)
                    {
                        Complete(shop.Buy(GameBootstrap.Session.Profile,
                            index),
                            $"buy slot={index}, content={itemName}, price={price}");
                        if (GameBootstrap.Session.Profile.Shop.Slots[index].Bought)
                            OnboardingRuntime.Publish("shop_item_purchased");
                    }
                }));
        }

        private void ConfirmReputation(string id,
            string itemName,
            int price)
        {
            itemTooltip.Hide();
            PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Xác nhận mua",
                $"Mua {itemName} với giá {UiFormat.Number(price)} Danh Vọng?",
                "Mua",
                "Hủy",
                yes =>
                {
                    if (yes)
                        Complete(shop.BuyReputation(GameBootstrap.Session.Profile,
                            id),
                            $"reputation purchase id={id}, content={itemName}, price={price}");
                }));
        }
        private void RefreshShop()
        {
            itemTooltip.Hide();
            bool free = shop.UntilFreeRefresh(GameBootstrap.Session.Profile) == TimeSpan.Zero;
            int cost = free ? 0 : shop.PaidRefreshCost(GameBootstrap.Session.Profile);
            Complete(shop.Refresh(GameBootstrap.Session.Profile,
                free),
                $"{(free ? "free" : "paid")} refresh cost={cost}");
        }

        private void Exchange()
        {
            string input = SelectedMaterialId(inputDropdown);
            string target = SelectedMaterialId(targetDropdown);
            Complete(shop.Exchange(GameBootstrap.Session.Profile, input, target), $"exchange {input}->{target}");
        }

        private void UpdateExchange()
        {
            if (shop == null || inputDropdown.options.Count == 0)
                return;
            MaterialInfo a = materials.Get(SelectedMaterialId(inputDropdown));
            MaterialInfo b = materials.Get(SelectedMaterialId(targetDropdown));
            bool same = a.ItemType == b.ItemType
                && b.Tier == a.Tier
                + 1, cross = a.ItemType != b.ItemType
                && a.Tier == b.Tier;
            exchangePreview.text =
                same
                ? $"3 {a.Name} → 1 {b.Name}"
                : cross
                ? $"2 {a.Name} → 1 {b.Name}"
                : "Lựa chọn không hợp lệ";
            exchangeButton.GetComponent<Button>().interactable = same || cross;
        }

        private void Complete(EconomyResult result, string action)
        {
            itemTooltip.Hide();
            statusText.text = result.Success ? "Giao dịch thành công." : result.Error;
            if (result.Success)
            {
                AudioManager.Instance?.Play("coin");
                Debug.Log($"[Shop] {action}, result=success");
                GameBootstrap.Session.SaveProfile();
            }
            else
            {
                AudioManager.Instance?.Play("ui_error");
                Debug.LogWarning($"[Shop] {action}, result=failed, reason={result.Error}");
            }
            Refresh();
        }

        private static string TypeName(string type)
            => type == "weapon"
            ? "Binh Khí"
            : type == "armor"
            ? "Hộ Cụ"
            : type == "manual"
            ? "Công Pháp"
            : type == "pill"
                || type == "pill_relic" ? "Đan Dược / Kỳ Trân" : type == "relic" ? "Kỳ Trân" : "Nguyên liệu";

        private void CaptureTabStyles()
        {
            Image primaryImage = tradeTab.GetComponent<Image>();
            Image secondaryImage = exchangeTab.GetComponent<Image>();
            primaryTabSprite = primaryImage.sprite;
            secondaryTabSprite = secondaryImage.sprite;
            primaryTabColor = primaryImage.color;
            secondaryTabColor = secondaryImage.color;
            primaryTabTextColor = tradeTab.GetComponentInChildren<Text>().color;
            secondaryTabTextColor = exchangeTab.GetComponentInChildren<Text>().color;
        }

        private void SetTabStyle(UiButton tab, bool active)
        {
            Image image = tab.GetComponent<Image>();
            Text label = tab.GetComponentInChildren<Text>();
            image.sprite = active ? primaryTabSprite : secondaryTabSprite;
            image.color = active ? primaryTabColor : secondaryTabColor;
            label.color = active ? primaryTabTextColor : secondaryTabTextColor;
        }

        private Sprite FindIcon(string id)
        {
            if (icons != null && icons.TryGet(id, out Sprite sprite))
                return sprite;
            return null;
        }

        private string DescribeReputationOffer(ReputationOffer offer)
        {
            if (offer.Id == "chi_ton_ngoc_tuy")
                return ItemDescriber.DescribeMaterial(materials.Get(offer.Id), offer.Quantity);
            if (offer.Id == "catalyst_box_random")
                return $"<b>{offer.Name}</b>\nRương\nChứa {offer.Quantity} Chất Dẫn ngẫu nhiên.";
            if (offer.Id == "tier_3_material_box")
                return $"<b>{offer.Name}</b>\nRương\nChứa {offer.Quantity} nguyên liệu bậc 3 ngẫu nhiên.";
            return $"<b>{offer.Name}</b>\nVật phẩm Tiệm Danh Vọng";
        }

        private static List<Dropdown.OptionData> BuildMaterialOptions(
            MaterialCatalog catalog,
            ItemIconLibrary iconLibrary)
        {
            var options = new List<Dropdown.OptionData>();
            foreach (MaterialInfo material in catalog.All)
            {
                if (material.IsCatalyst)
                    continue;
                Sprite sprite = null;
                if (iconLibrary != null)
                    iconLibrary.TryGet(material.Id, out sprite);
                options.Add(new Dropdown.OptionData(material.Name, sprite));
            }

            return options;
        }

        private string SelectedMaterialId(Dropdown dropdown)
        {
            int index = Mathf.Clamp(dropdown.value, 0, materialIds.Count - 1);
            return materialIds[index];
        }
    }
}
