using System;
using System.Collections.Generic;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Onboarding;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class EconomyPopupPrefabBuilder
    {
        private const string Folder = "Assets/_Game/Resources/UI/Popups";

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            UiBuild.EnsureFolder(Folder);
            BuildStorageViews(overwrite);
            BuildPopup<StoragePopup>("StoragePopup", new Vector2(1740f, 980f), overwrite, BuildStorage);
            BuildShopCard(overwrite);
            BuildPopup<ShopPopup>("ShopPopup", new Vector2(1600f, 920f), overwrite, BuildShop);
            BuildPopup<SectSelectPopup>("SectSelectPopup", new Vector2(1050f, 760f), overwrite, BuildSect);
            BuildPopup<DevCheatPanel>("DevCheatPanel", new Vector2(980f, 900f), overwrite, BuildDev);
            AssetDatabase.SaveAssets();
        }

        private static void BuildSect(SectSelectPopup popup, RectTransform panel)
        {
            // Sect selector.
            Text(panel, "Title", "Chọn Môn Phái", 40, .05f, .86f, .95f, .97f);
            Dropdown sect = Dropdown(panel, "Sects", new[] { "Môn phái" }, .08f, .70f, .92f, .82f);
            TutorialTarget sectTarget = sect.gameObject.AddComponent<TutorialTarget>();
            sectTarget.Configure("sect_choices");
            Text detail = Text(panel, "Description", "Thông tin môn phái", 24, .08f, .25f, .92f, .67f);
            detail.alignment = TextAnchor.UpperLeft;
            UiButton confirm = Button(panel, "Confirm", "Xác nhận gia nhập", true, .35f, .12f);
            UiButton cancel = Button(panel, "Cancel", "Để sau", false, .70f, .12f);
            Set(popup, "sectDropdown", sect);
            Set(popup, "descriptionText", detail);
            Set(popup, "confirmButton", confirm);
            Set(popup, "cancelButton", cancel);
        }

        private static void BuildStorageViews(bool overwrite)
        {
            BuildStorageCell(overwrite);
            BuildStorageItem(overwrite);
        }

        private static void BuildStorageCell(bool overwrite)
        {
            string path = Folder + "/StorageCell.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            var root = new GameObject("StorageCell", typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(76f, 76f);
            StorageCell cell = root.AddComponent<StorageCell>();
            Image paper = UiBuild.Image(root.transform, "Paper", new Color(.18f, .15f, .12f, .1f));
            UiBuild.Stretch(paper.rectTransform);
            Set(cell, "paper", paper);
            Save(root, path);
        }

        private static void BuildStorageItem(bool overwrite)
        {
            string path = Folder + "/StorageItemView.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            var root = new GameObject("StorageItemView", typeof(RectTransform), typeof(CanvasGroup));
            StorageItemView view = root.AddComponent<StorageItemView>();
            Image border = UiBuild.Image(root.transform, "Border", Color.white);
            UiBuild.Stretch(border.rectTransform);
            UiBuild.ApplyUiSprite(border, "frame_panel", Image.Type.Sliced);
            Image icon = UiBuild.Image(root.transform, "Icon", Color.white);
            Anchor(icon.rectTransform, .10f, .10f, .90f, .90f);
            icon.preserveAspect = true;
            // Rarity medallion: top-left, clear of the tier pill at bottom-left.
            Image rarityBadge = UiBuild.Image(root.transform, "Rarity Badge", Color.white);
            SetTopLeftBadgeRect(rarityBadge.rectTransform, 20f, 4f);
            rarityBadge.preserveAspect = true;
            rarityBadge.raycastTarget = false;
            Text name = Text(root.transform, "Name", "Vật phẩm", 14, .08f, .38f, .92f, .92f);
            name.gameObject.AddComponent<SingleLineFit>().Configure(14, 9);
            name.alignment = TextAnchor.MiddleCenter;
            name.horizontalOverflow = HorizontalWrapMode.Wrap;
            name.verticalOverflow = VerticalWrapMode.Overflow;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 10;
            name.resizeTextMaxSize = 14;
            Image pill = UiBuild.Image(root.transform, "Badge Pill", new Color(.10f, .08f, .06f, .86f));
            SetBadgeRect(pill.rectTransform);
            pill.raycastTarget = false;
            UiBuild.ApplyUiSprite(pill, "plaque_label", Image.Type.Sliced);
            Text badge = Text(pill.transform, "Badge", "T1", 13, 0f, 0f, 1f, 1f);
            badge.color = UiBuild.Paper;
            badge.alignment = TextAnchor.MiddleCenter;
            badge.gameObject.AddComponent<SingleLineFit>().Configure(13, 11);
            Set(view, "icon", icon);
            Set(view, "border", border);
            Set(view, "nameText", name);
            Set(view, "badgeText", badge);
            Set(view, "rarityBadge", rarityBadge);
            Set(view, "canvasGroup", root.GetComponent<CanvasGroup>());
            Save(root, path);
        }

        private static void SetBadgeRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(7f, 7f);
            rect.sizeDelta = new Vector2(34f, 22f);
        }

        private static void SetTopLeftBadgeRect(RectTransform rect, float size, float inset)
        {
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.pivot = Vector2.up;
            rect.anchoredPosition = new Vector2(inset, -inset);
            rect.sizeDelta = new Vector2(size, size);
        }

        private static void BuildStorage(StoragePopup popup, RectTransform panel)
        {
            // Header, currency row, and controls.
            Text header = Text(panel, "Header", "Tàng Bảo Các  0/200", 40, .02f, .91f, .62f, .99f);
            RectTransform currencies = UiBuild.Rect(panel, "Currency Row");
            Anchor(currencies, .66f, .92f, .96f, .98f);
            Horizontal(currencies, 20f);
            Text silver = CurrencyEntry(currencies, "Silver", "Bạc 0", 190f);
            Text reputation = CurrencyEntry(currencies, "Reputation", "Danh Vọng 0", 210f);
            Text warning = Text(panel, "Overflow Warning", string.Empty, 17, .02f, .86f, .62f, .91f);
            warning.color = UiBuild.Vermilion;
            string[] types = { "Tất cả", "Binh Khí", "Hộ Cụ", "Công Pháp", "Đan Dược", "Kỳ Trân" };
            Dropdown filter = Dropdown(panel, "Type Filter", types, .02f, .79f, .22f, .86f);
            UiButton sort = Button(panel, "Sort", "Sắp xếp", false, .37f, .825f);

            // Storage grid.
            RectTransform viewport = UiBuild.Image(panel, "Grid Viewport", new Color(0f, 0f, 0f, .04f)).rectTransform;
            Anchor(viewport, .02f, .08f, .62f, .78f);
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform content = UiBuild.Rect(viewport, "Grid Content");
            PrepareGrid(content);
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            BuildCells(content);
            StorageCell cell = Linked<StorageCell>(Folder + "/StorageCell.prefab", panel, "Cell Prefab");
            StorageItemView item = Linked<StorageItemView>(
                Folder + "/StorageItemView.prefab",
                panel,
                "Item Prefab");
            cell.gameObject.SetActive(false);
            item.gameObject.SetActive(false);

            // The description grows inside a clipped, independently scrollable viewport.
            RectTransform detailViewport = UiBuild.Image(panel, "Detail Viewport", Color.clear).rectTransform;
            Anchor(detailViewport, .65f, .47f, .97f, .88f);
            detailViewport.gameObject.AddComponent<RectMask2D>();
            RectTransform detailContent = UiBuild.Rect(detailViewport, "Content");
            PrepareDetailContent(detailContent);
            ScrollRect detailScroll = detailViewport.gameObject.AddComponent<ScrollRect>();
            ConfigureDetailScroll(detailScroll, detailViewport, detailContent);
            AddDetailScrollbar(panel, detailScroll);
            Text detail = Text(detailContent, "Detail", "Chọn một vật phẩm", 18, 0f, 0f, 1f, 1f);
            UiFonts.Apply(detail, UiFontRole.Body);
            detail.alignment = TextAnchor.UpperLeft;
            detail.supportRichText = true;
            detail.horizontalOverflow = HorizontalWrapMode.Wrap;
            detail.verticalOverflow = VerticalWrapMode.Overflow;
            detail.resizeTextForBestFit = false;
            detail.fontStyle = FontStyle.Normal;
            detail.lineSpacing = 1.1f;
            detail.transform.localScale = Vector3.one;
            detail.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Upgrade costs stay within the fixed band above the action buttons.
            Text cost = Text(panel, "Cost", string.Empty, 20, .65f, .34f, .97f, .46f);
            cost.alignment = TextAnchor.UpperLeft;
            cost.supportRichText = true;
            cost.resizeTextForBestFit = true;
            cost.resizeTextMinSize = 16;
            cost.resizeTextMaxSize = 20;
            UiButton upgrade = Button(panel, "Upgrade", "Nâng Trọng", true, .72f, .26f);
            UiButton merge = Button(panel, "Merge", "Hợp Thành", true, .89f, .26f);
            TutorialTarget upgradeTarget = upgrade.gameObject.AddComponent<TutorialTarget>();
            upgradeTarget.Configure("storage_upgrade");
            TutorialTarget mergeTarget = merge.gameObject.AddComponent<TutorialTarget>();
            mergeTarget.Configure("storage_merge");
            UiButton sell = Button(panel, "Sell", "Bán", false, .72f, .15f);
            UiButton close = Button(panel, "Close", "Đóng", false, .89f, .15f);
            BindStorage(popup, header, silver, reputation, warning, filter, sort, scroll, content, cell, item);
            Set(popup, "detailText", detail);
            Set(popup, "costText", cost);
            Set(popup, "upgradeButton", upgrade);
            Set(popup, "mergeButton", merge);
            Set(popup, "sellButton", sell);
            Set(popup, "closeButton", close);
        }

        private static void PrepareDetailContent(RectTransform content)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void ConfigureDetailScroll(
            ScrollRect scroll,
            RectTransform viewport,
            RectTransform content)
        {
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 28f;
        }

        private static void AddDetailScrollbar(RectTransform panel, ScrollRect scroll)
        {
            Color trackColor = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .16f);
            Image track = UiBuild.Image(panel, "Detail Scrollbar", trackColor);
            Scrollbar scrollbar = track.gameObject.AddComponent<Scrollbar>();
            Anchor(scrollbar.GetComponent<RectTransform>(), .972f, .47f, .982f, .88f);
            Image handle = UiBuild.Image(scrollbar.transform, "Handle", UiBuild.Vermilion);
            UiBuild.Stretch(handle.rectTransform);
            scrollbar.handleRect = handle.rectTransform;
            scrollbar.targetGraphic = handle;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scroll.verticalScrollbar = scrollbar;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        }

        private static void PrepareGrid(RectTransform content)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 1f);
            content.sizeDelta = new Vector2(800f, 1600f);
        }

        private static void BuildCells(RectTransform content)
        {
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    StorageCell cell = Linked<StorageCell>(Folder + "/StorageCell.prefab", content, $"Cell {x},{y}");
                    RectTransform rect = (RectTransform)cell.transform;
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.pivot = new Vector2(0f, 1f);
                    rect.anchoredPosition = new Vector2(x * 80f, -y * 80f);
                    rect.sizeDelta = new Vector2(76f, 76f);
                }
            }
        }

        private static void BindStorage(
            StoragePopup popup,
            Text header,
            Text silver,
            Text reputation,
            Text warning,
            Dropdown filter,
            UiButton sort,
            ScrollRect scroll,
            RectTransform content,
            StorageCell cell,
            StorageItemView item)
        {
            Set(popup, "headerText", header);
            Set(popup, "currencyText", silver);
            Set(popup, "reputationText", reputation);
            Set(popup, "overflowText", warning);
            Set(popup, "typeFilter", filter);
            Set(popup, "sortButton", sort);
            Set(popup, "gridScroll", scroll);
            Set(popup, "gridContent", content);
            Set(popup, "cellPrefab", cell);
            Set(popup, "storageItemPrefab", item);
        }

        private static void BuildDev(DevCheatPanel popup, RectTransform panel)
        {
            // Linked common dropdowns and development actions.
            Text(panel, "Title", "Công cụ phát triển (F1)", 38, .05f, .87f, .95f, .98f);
            Dropdown items = Dropdown(panel, "Items", new[] { "item" }, .05f, .74f, .55f, .83f);
            string[] rarities = { "Phàm", "Lương", "Tinh", "Tuyệt", "Thần Binh", "Chí Tôn" };
            Dropdown rarity = Dropdown(panel, "Rarity", rarities, .57f, .74f, .76f, .83f);
            string[] tiers = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Dropdown tier = Dropdown(panel, "Tier", tiers, .78f, .74f, .94f, .83f);
            Dropdown material = Dropdown(panel, "Materials", new[] { "material" }, .05f, .51f, .55f, .6f);
            UiButton addItem = Button(panel, "Add Item", "Thêm vật phẩm", true, .5f, .66f);
            UiButton add10 = Button(panel, "Add 10", "+10", false, .67f, .555f);
            UiButton add100 = Button(panel, "Add 100", "+100", false, .86f, .555f);
            UiButton silver = Button(panel, "Silver", "+10.000 Bạc", false, .3f, .46f);
            UiButton reputation = Button(panel, "Reputation", "+1.000 Danh Vọng", false, .7f, .46f);
            UiButton refresh = Button(panel, "Refresh Shop", "Làm mới Thương Hội miễn phí", false, .3f, .37f);
            UiButton week = Button(panel, "Next Week", "Sang tuần mới", false, .7f, .37f);
            UiButton unlock = Button(panel, "Unlock Pve", "Mở toàn bộ ải", false, .2f, .27f);
            UiButton refill = Button(panel, "Refill Stamina", "Hồi đầy Thể Lực", false, .5f, .27f);
            UiButton clear = Button(panel, "Clear Stage", "Qua ải 3 sao", false, .8f, .27f);
            UiButton level = Button(panel, "Add Level", "+1 cấp", false, .2f, .20f);
            UiButton exp = Button(panel, "Add Exp", "+100 EXP", false, .5f, .20f);
            UiButton sect = Button(panel, "Reset Sect", "Đặt lại môn phái", false, .8f, .20f);
            UiButton reset = Button(panel, "Reset", "Đặt lại hồ sơ", false, .3f, .13f);
            UiButton close = Button(panel, "Close", "Đóng", true, .7f, .13f);
            UiButton wipe = Button(panel, "Wipe All", "Chơi lại từ đầu", false, .5f, .06f);
            wipe.GetComponent<Image>().color = UiBuild.Vermilion;
            wipe.GetComponentInChildren<Text>().color = UiBuild.Paper;
            BindDev(popup, items, rarity, tier, material, addItem, add10, add100);
            Set(popup, "addSilverButton", silver);
            Set(popup, "addReputationButton", reputation);
            Set(popup, "refreshShopButton", refresh);
            Set(popup, "nextWeekButton", week);
            Set(popup, "unlockPveButton", unlock);
            Set(popup, "refillStaminaButton", refill);
            Set(popup, "clearStageButton", clear);
            Set(popup, "addLevelButton", level);
            Set(popup, "addExpButton", exp);
            Set(popup, "resetSectButton", sect);
            Set(popup, "resetButton", reset);
            Set(popup, "closeButton", close);
            Set(popup, "wipeAllButton", wipe);
        }

        private static void BindDev(
            DevCheatPanel popup,
            Dropdown items,
            Dropdown rarity,
            Dropdown tier,
            Dropdown material,
            UiButton addItem,
            UiButton add10,
            UiButton add100)
        {
            Set(popup, "itemDropdown", items);
            Set(popup, "rarityDropdown", rarity);
            Set(popup, "tierDropdown", tier);
            Set(popup, "materialDropdown", material);
            Set(popup, "addItemButton", addItem);
            Set(popup, "add10Button", add10);
            Set(popup, "add100Button", add100);
        }

        private static void BuildShopCard(bool overwrite)
        {
            string path = Folder + "/ShopCard.prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            var root = new GameObject("ShopCard", typeof(RectTransform), typeof(CanvasGroup));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(420f, 300f);
            ShopCard card = root.AddComponent<ShopCard>();
            Image paper = UiBuild.Image(root.transform, "Paper", UiBuild.Paper);
            UiBuild.Stretch(paper.rectTransform);
            UiBuild.ApplyUiSprite(paper, "frame_panel", Image.Type.Sliced);
            Image border = UiBuild.Image(root.transform, "Rarity Border", Color.white);
            UiBuild.Stretch(border.rectTransform, 3f);
            border.raycastTarget = false;
            UiBuild.ApplyUiSprite(border, "frame_panel", Image.Type.Sliced);
            Image icon = UiBuild.Image(root.transform, "Icon", Color.white);
            Anchor(icon.rectTransform, .08f, .55f, .92f, .94f);
            icon.preserveAspect = true;
            // Rarity medallion: top-left of the item-art region.
            Image rarityBadge = UiBuild.Image(root.transform, "Rarity Badge", Color.white);
            SetTopLeftBadgeRect(rarityBadge.rectTransform, 34f, 24f);
            rarityBadge.preserveAspect = true;
            rarityBadge.raycastTarget = false;
            Text placeholder = Text(root.transform, "Icon Placeholder", "◆", 24, .08f, .55f, .92f, .94f);
            placeholder.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .55f);
            Text name = Text(root.transform, "Name", "Tên vật phẩm", 21, .05f, .45f, .95f, .55f);
            name.fontStyle = FontStyle.Bold;
            name.gameObject.AddComponent<SingleLineFit>().Configure(21, 14);
            Text detail = Text(root.transform, "Detail", "Phàm • 1 Trọng", 16, .05f, .36f, .66f, .45f);
            detail.alignment = TextAnchor.MiddleLeft;
            detail.gameObject.AddComponent<SingleLineFit>().Configure(16, 12);
            Text type = Text(root.transform, "Type", "Binh Khí", 16, .66f, .36f, .95f, .45f);
            Text stat = Text(root.transform, "Stat", "Khí Huyết +20", 16, .05f, .28f, .95f, .36f);
            stat.alignment = TextAnchor.MiddleLeft;
            stat.gameObject.AddComponent<SingleLineFit>().Configure(16, 12);
            Text price = Text(root.transform, "Price", "Bạc 1.000", 20, .05f, .20f, .48f, .28f);
            Text oldPrice = Text(root.transform, "Old Price", "1.200", 15, .47f, .20f, .68f, .28f);
            oldPrice.color = new Color(.45f, .4f, .36f);
            Text discount = Text(root.transform, "Discount", "-20%", 17, .68f, .20f, .93f, .28f);
            discount.color = UiBuild.Vermilion;
            price.gameObject.AddComponent<SingleLineFit>().Configure(20, 12);
            oldPrice.gameObject.AddComponent<SingleLineFit>().Configure(15, 10);
            discount.gameObject.AddComponent<SingleLineFit>().Configure(17, 11);
            type.gameObject.AddComponent<SingleLineFit>().Configure(17, 11);
            Text reason = Text(root.transform, "Disabled Reason", "Không đủ Bạc", 15, .05f, .14f, .95f, .20f);
            reason.color = UiBuild.Vermilion;
            reason.gameObject.AddComponent<SingleLineFit>().Configure(15, 10);
            UiButton buy = Linked<UiButton>(
                CommonUiPrefabBuilder.Folder + "/Button_Primary.prefab",
                root.transform,
                "Buy");
            buy.SetLabel("Mua");
            // The ornate button needs ~38 px of card height for an 18 px label to fit inside its band.
            Anchor(buy.GetComponent<RectTransform>(), .28f, .005f, .72f, .14f);
            Text buyLabel = buy.GetComponentInChildren<Text>();
            UiBuild.Stretch(buyLabel.rectTransform, 4f);
            SingleLineFit buyFit = buyLabel.GetComponent<SingleLineFit>();
            if (buyFit == null)
            {
                buyFit = buyLabel.gameObject.AddComponent<SingleLineFit>();
            }

            buyFit.Configure(18, 11);
            Image seal = UiBuild.Image(root.transform, "Bought Seal", Color.clear);
            seal.raycastTarget = false;
            RectTransform sealRect = seal.rectTransform;
            sealRect.anchorMin = new Vector2(.78f, .16f);
            sealRect.anchorMax = new Vector2(.78f, .16f);
            sealRect.sizeDelta = new Vector2(105f, 105f);
            UiBuild.ApplyUiSprite(seal, "seal_title", Image.Type.Simple);
            Text sealText = Text(root.transform, "Bought Seal Text", "Đã mua", 23, .64f, .08f, .93f, .25f);
            sealText.color = UiBuild.Vermilion;
            sealText.fontStyle = FontStyle.Bold;
            BindCard(card, paper, border, icon, rarityBadge, placeholder, name, detail, type);
            Set(card, "statText", stat);
            Set(card, "priceText", price);
            Set(card, "oldPriceText", oldPrice);
            Set(card, "discountText", discount);
            Set(card, "buyButton", buy);
            Set(card, "disabledReasonText", reason);
            Set(card, "boughtSeal", seal);
            Set(card, "boughtSealText", sealText);
            Save(root, path);
        }

        private static void BindCard(
            ShopCard card,
            Image paper,
            Image border,
            Image icon,
            Image rarityBadge,
            Text placeholder,
            Text name,
            Text detail,
            Text type)
        {
            Set(card, "paperPanel", paper);
            Set(card, "rarityBorder", border);
            Set(card, "iconImage", icon);
            Set(card, "rarityBadge", rarityBadge);
            Set(card, "placeholderText", placeholder);
            Set(card, "nameText", name);
            Set(card, "detailText", detail);
            Set(card, "typeText", type);
        }

        private static void BuildShop(ShopPopup popup, RectTransform panel)
        {
            // Header with icon/value pairs controlled by layout.
            Text(panel, "Title", "Thương Hội", 40, .03f, .92f, .97f, .99f);
            RectTransform header = UiBuild.Rect(panel, "Header Row");
            Anchor(header, .03f, .85f, .76f, .92f);
            Horizontal(header, 22f);
            Text silver = CurrencyEntry(header, "Silver", "1.200", 150f);
            Text reputation = CurrencyEntry(header, "Reputation", "0", 150f);
            Text timer = UiBuild.Text(header, "Timer", "Miễn phí sau 05:59:59", 21, TextAnchor.MiddleLeft);
            timer.gameObject.AddComponent<LayoutElement>().preferredWidth = 300f;
            Text status = Text(panel, "Status", string.Empty, 18, .03f, .015f, .65f, .065f);
            UiButton refresh = Button(panel, "Refresh", "Làm mới", true, .86f, .885f);
            refresh.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 58f);
            UiButton trade = Button(panel, "Trade Tab", "Thương Hội", true, .18f, .795f);
            UiButton exchange = Button(panel, "Exchange Tab", "Đổi nguyên liệu", false, .5f, .795f);
            UiButton reputationTab = Button(panel, "Reputation Tab", "Tiệm Danh Vọng", false, .82f, .795f);
            RectTransform tradePanel = Panel(panel, "Trade Panel");
            RectTransform slots = UiBuild.Rect(tradePanel, "Slots");
            Anchor(slots, 0f, 0f, 1f, 1f);
            Grid(slots, 3, new Vector2(28f, 28f));
            RectTransform exchangePanel = Panel(panel, "Exchange Panel");
            Text(exchangePanel, "Input Label", "Đưa vào", 19, .08f, .79f, .44f, .86f);
            Text(exchangePanel, "Target Label", "Nhận về", 19, .56f, .79f, .92f, .86f);
            Dropdown input = Dropdown(exchangePanel, "Input", new[] { "Nguyên liệu" }, .08f, .65f, .44f, .78f);
            Dropdown target = Dropdown(exchangePanel, "Target", new[] { "Nguyên liệu" }, .56f, .65f, .92f, .78f);
            Text preview = Text(exchangePanel, "Preview", "Xem trước", 25, 0f, .42f, 1f, .58f);
            UiButton exchangeGo = Button(exchangePanel, "Confirm Exchange", "Xác nhận đổi", true, .5f, .28f);
            RectTransform reputationPanel = Panel(panel, "Reputation Panel");
            RectTransform offers = UiBuild.Rect(reputationPanel, "Offers");
            Anchor(offers, 0f, .25f, 1f, .75f);
            ReputationGrid(offers);
            ShopCard template = Linked<ShopCard>(Folder + "/ShopCard.prefab", panel, "Shop Card Prefab");
            template.gameObject.SetActive(false);
            ItemTooltip tooltip = BuildItemTooltip(popup.transform);
            UiButton close = Button(panel, "Close", "Đóng", false, .86f, .035f);
            BindShop(popup, timer, silver, reputation, status, trade, exchange, reputationTab);
            Set(popup, "tradePanel", tradePanel.gameObject);
            Set(popup, "exchangePanel", exchangePanel.gameObject);
            Set(popup, "reputationPanel", reputationPanel.gameObject);
            Set(popup, "slotList", slots);
            Set(popup, "cardPrefab", template);
            Set(popup, "refreshButton", refresh);
            Set(popup, "inputDropdown", input);
            Set(popup, "targetDropdown", target);
            Set(popup, "exchangePreview", preview);
            Set(popup, "exchangeButton", exchangeGo);
            Set(popup, "reputationList", offers);
            Set(popup, "closeButton", close);
            Set(popup, "itemTooltip", tooltip);
        }

        private static ItemTooltip BuildItemTooltip(Transform parent)
        {
            // Tooltip overlay shared by every shop card.
            var root = new GameObject("Item Tooltip", typeof(RectTransform), typeof(Canvas), typeof(ItemTooltip));
            root.transform.SetParent(parent, false);
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            Image paper = UiBuild.Image(root.transform, "Paper", UiBuild.Paper);
            UiBuild.ApplyUiSprite(paper, "frame_tooltip", Image.Type.Sliced);
            RectTransform panel = paper.rectTransform;
            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.zero;
            panel.pivot = new Vector2(0f, 1f);
            panel.sizeDelta = new Vector2(440f, 420f);
            RectTransform viewport = UiBuild.Image(paper.transform, "Viewport", Color.clear).rectTransform;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(ItemTooltip.HorizontalInset, ItemTooltip.BottomInset);
            viewport.offsetMax = new Vector2(-ItemTooltip.HorizontalInset, -ItemTooltip.TopInset);
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = UiBuild.Rect(viewport, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(.5f, 1f);
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            ContentSizeFitter contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = paper.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            Text description = UiBuild.Text(content, "Description", string.Empty, 18, TextAnchor.UpperLeft);
            description.color = UiBuild.Ink;
            description.supportRichText = true;
            description.horizontalOverflow = HorizontalWrapMode.Wrap;
            description.verticalOverflow = VerticalWrapMode.Overflow;
            ContentSizeFitter descriptionFitter = description.gameObject.AddComponent<ContentSizeFitter>();
            descriptionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ItemTooltip tooltip = root.GetComponent<ItemTooltip>();
            Set(tooltip, "overlayCanvas", canvas);
            Set(tooltip, "panel", panel);
            Set(tooltip, "descriptionText", description);
            return tooltip;
        }

        private static void BindShop(
            ShopPopup popup,
            Text timer,
            Text silver,
            Text reputation,
            Text status,
            UiButton trade,
            UiButton exchange,
            UiButton reputationTab)
        {
            Set(popup, "headerText", timer);
            Set(popup, "silverText", silver);
            Set(popup, "reputationText", reputation);
            Set(popup, "statusText", status);
            Set(popup, "tradeTab", trade);
            Set(popup, "exchangeTab", exchange);
            Set(popup, "reputationTab", reputationTab);
        }

        private static RectTransform Panel(RectTransform parent, string name)
        {
            RectTransform panel = UiBuild.Rect(parent, name);
            Anchor(panel, .03f, .07f, .97f, .75f);
            return panel;
        }

        private static void Grid(RectTransform parent, int columns, Vector2 spacing)
        {
            GridLayoutGroup grid = parent.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(420f, 300f);
            grid.spacing = spacing;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.MiddleCenter;
        }

        private static void ReputationGrid(RectTransform parent)
        {
            GridLayoutGroup grid = parent.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(420f, 300f);
            grid.spacing = new Vector2(28f, 0f);
            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 1;
            grid.childAlignment = TextAnchor.MiddleCenter;
        }

        private static void BuildPopup<T>(
            string name,
            Vector2 size,
            bool overwrite,
            Action<T, RectTransform> build)
            where T : BasePopup
        {
            string path = Folder + "/" + name + ".prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite))
                return;
            var root = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(Canvas),
                typeof(GraphicRaycaster));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            root.GetComponent<Canvas>().overrideSorting = true;
            T popup = root.AddComponent<T>();
            Image paper = Linked<Image>(CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab", root.transform, "Panel");
            RectTransform panel = paper.rectTransform;
            panel.anchorMin = new Vector2(.5f, .5f);
            panel.anchorMax = new Vector2(.5f, .5f);
            panel.pivot = new Vector2(.5f, .5f);
            panel.sizeDelta = size;
            Set(popup, "panel", panel);
            build(popup, panel);
            root.SetActive(false);
            Save(root, path);
        }

        private static Text Text(
            Transform parent,
            string name,
            string value,
            int size,
            float xMin,
            float yMin,
            float xMax,
            float yMax)
        {
            bool isDisplay = name.IndexOf("Title", StringComparison.OrdinalIgnoreCase) >= 0
                || name == "Header" && size >= 34;
            UiFontRole role = isDisplay
                ? UiFontRole.Display
                : name.IndexOf("Value", StringComparison.OrdinalIgnoreCase) >= 0
                    ? UiFontRole.Emphasis
                    : UiFontRole.Body;
            Text text = UiBuild.Text(parent, name, value, size, TextAnchor.MiddleCenter, role);
            Anchor(text.rectTransform, xMin, yMin, xMax, yMax);
            return text;
        }

        private static Text CurrencyEntry(Transform parent, string name, string value, float width)
        {
            RectTransform entry = UiBuild.Rect(parent, name + " Entry");
            LayoutElement entrySize = entry.gameObject.AddComponent<LayoutElement>();
            entrySize.preferredWidth = width;
            entrySize.preferredHeight = 44f;
            Horizontal(entry, 6f);
            Image icon = UiBuild.Image(entry, "Icon", Color.white);
            string iconKey = name == "Silver" ? "icon_bac" : "icon_danh_vong";
            if (!UiBuild.ApplyUiSprite(icon, iconKey))
                icon.gameObject.SetActive(false);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            LayoutElement iconSize = icon.gameObject.AddComponent<LayoutElement>();
            iconSize.minWidth = 26f;
            iconSize.preferredWidth = 26f;
            iconSize.minHeight = 26f;
            iconSize.preferredHeight = 26f;
            Text text = UiBuild.Text(entry, name, value, 20, TextAnchor.MiddleLeft);
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            return text;
        }

        private static void Horizontal(RectTransform parent, float spacing)
        {
            HorizontalLayoutGroup layout = parent.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        private static UiButton Button(
            Transform parent,
            string name,
            string label,
            bool primary,
            float x,
            float y)
        {
            string style = primary ? "Primary" : "Secondary";
            UiButton button = Linked<UiButton>(
                CommonUiPrefabBuilder.Folder + "/Button_" + style + ".prefab",
                parent,
                name);
            button.SetLabel(label);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(x, y);
            rect.anchorMax = new Vector2(x, y);
            rect.sizeDelta = new Vector2(260f, 62f);
            return button;
        }

        private static Dropdown Dropdown(
            Transform parent,
            string name,
            IEnumerable<string> options,
            float xMin,
            float yMin,
            float xMax,
            float yMax)
        {
            Dropdown dropdown = Linked<Dropdown>(
                CommonUiPrefabBuilder.Folder + "/Dropdown.prefab",
                parent,
                name);
            dropdown.options = new List<Dropdown.OptionData>();
            foreach (string option in options)
            {
                dropdown.options.Add(new Dropdown.OptionData(option));
            }

            dropdown.RefreshShownValue();
            Anchor(dropdown.GetComponent<RectTransform>(), xMin, yMin, xMax, yMax);
            return dropdown;
        }

        private static T Linked<T>(string path, Transform parent, string name)
            where T : Component
        {
            T component = UiBuild.InstantiateLinked<T>(path, parent);
            component.name = name;
            return component;
        }

        private static void Anchor(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Set(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            UiBuild.Set(target, field, value);
        }

        private static void Save(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
