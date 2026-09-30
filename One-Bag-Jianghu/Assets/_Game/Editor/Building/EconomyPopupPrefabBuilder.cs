using System.Collections.Generic;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
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
            CommonUiPrefabBuilder.Build(false); UiBuild.EnsureFolder(Folder);
            BuildStorageViews(overwrite); BuildStorage(overwrite); BuildShopCard(overwrite); BuildShopGrid(overwrite); BuildDev(overwrite); AssetDatabase.SaveAssets();
        }
        private static void BuildStorageViews(bool overwrite)
        {
            string cellPath=Folder+"/StorageCell.prefab"; if(AssetGuard.ShouldBuild(cellPath,overwrite)){var root=new GameObject("StorageCell",typeof(RectTransform));root.GetComponent<RectTransform>().sizeDelta=new Vector2(76,76);StorageCell cell=root.AddComponent<StorageCell>();Image paper=UiBuild.Image(root.transform,"Paper",new Color(.18f,.15f,.12f,.1f));UiBuild.Stretch(paper.rectTransform);UiBuild.Set(cell,"paper",paper);PrefabUtility.SaveAsPrefabAsset(root,cellPath);Object.DestroyImmediate(root);}
            string itemPath=Folder+"/StorageItemView.prefab"; if(AssetGuard.ShouldBuild(itemPath,overwrite)){var root=new GameObject("StorageItemView",typeof(RectTransform),typeof(CanvasGroup));StorageItemView view=root.AddComponent<StorageItemView>();Image border=UiBuild.Image(root.transform,"Border",Color.white);UiBuild.Stretch(border.rectTransform);UiBuild.ApplyUiSprite(border,"frame_panel",UnityEngine.UI.Image.Type.Sliced);Image icon=UiBuild.Image(root.transform,"Icon",Color.white);Anchor(icon.rectTransform,.08f,.25f,.92f,.92f);icon.preserveAspect=true;Text name=Text(root.transform,"Name","Vật phẩm",14,.04f,.02f,.96f,.28f);Text badge=Text(root.transform,"Badge","1 Trọng",13,.5f,.72f,.98f,.98f);UiBuild.Set(view,"icon",icon);UiBuild.Set(view,"border",border);UiBuild.Set(view,"nameText",name);UiBuild.Set(view,"badgeText",badge);UiBuild.Set(view,"canvasGroup",root.GetComponent<CanvasGroup>());PrefabUtility.SaveAsPrefabAsset(root,itemPath);Object.DestroyImmediate(root);}
        }
        private static void BuildStorage(bool overwrite)
        {
            Build<StoragePopup>("StoragePopup",new Vector2(1740,980),overwrite,(popup,panel)=>{
                Text header=Text(panel,"Header","Tàng Bảo Các  0/200",40,.02f,.91f,.62f,.99f);Text currency=Text(panel,"Currencies","Bạc • Danh Vọng",20,.64f,.92f,.96f,.98f);Text warning=Text(panel,"Overflow Warning","",17,.02f,.86f,.62f,.91f);warning.color=UiBuild.Vermilion;
                Dropdown filter=Dropdown(panel,"Type Filter",new[]{"Tất cả","Binh Khí","Hộ Cụ","Công Pháp","Đan Dược","Kỳ Trân"},.02f,.79f,.22f,.86f);UiButton sort=Button(panel,"Sort","Sắp xếp",false,.37f,.825f);
                RectTransform viewport=UiBuild.Image(panel,"Grid Viewport",new Color(0,0,0,.04f)).rectTransform;Anchor(viewport,.02f,.08f,.62f,.78f);viewport.gameObject.AddComponent<Mask>().showMaskGraphic=false;RectTransform content=UiBuild.Rect(viewport,"Grid Content");content.anchorMin=content.anchorMax=new Vector2(0,1);content.pivot=new Vector2(0,1);ScrollRect scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.content=content;scroll.horizontal=false;scroll.vertical=true;
                StorageCell cell=UiBuild.InstantiateLinked<StorageCell>(Folder+"/StorageCell.prefab",panel);cell.gameObject.SetActive(false);StorageItemView item=UiBuild.InstantiateLinked<StorageItemView>(Folder+"/StorageItemView.prefab",panel);item.gameObject.SetActive(false);
                content.sizeDelta=new Vector2(800,1600);for(int y=0;y<20;y++)for(int x=0;x<10;x++){StorageCell authored=UiBuild.InstantiateLinked<StorageCell>(Folder+"/StorageCell.prefab",content);authored.name=$"Cell {x},{y}";RectTransform r=(RectTransform)authored.transform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x*80,-y*80);r.sizeDelta=new Vector2(76,76);}
                Text detail=Text(panel,"Detail","Chọn một vật phẩm",25,.65f,.48f,.97f,.78f);detail.supportRichText=true;detail.resizeTextForBestFit=true;detail.resizeTextMinSize=10;detail.verticalOverflow=VerticalWrapMode.Overflow;Text cost=Text(panel,"Cost","",20,.65f,.34f,.97f,.48f);UiButton up=Button(panel,"Upgrade","Nâng Trọng",true,.72f,.26f);UiButton merge=Button(panel,"Merge","Hợp Thành",true,.89f,.26f);UiButton sell=Button(panel,"Sell","Bán",false,.72f,.15f);UiButton close=Button(panel,"Close","Đóng",false,.89f,.15f);
                UiBuild.Set(popup,"headerText",header);UiBuild.Set(popup,"currencyText",currency);UiBuild.Set(popup,"overflowText",warning);UiBuild.Set(popup,"typeFilter",filter);UiBuild.Set(popup,"gridScroll",scroll);UiBuild.Set(popup,"gridContent",content);UiBuild.Set(popup,"cellPrefab",cell);UiBuild.Set(popup,"storageItemPrefab",item);UiBuild.Set(popup,"sortButton",sort);UiBuild.Set(popup,"detailText",detail);UiBuild.Set(popup,"costText",cost);UiBuild.Set(popup,"upgradeButton",up);UiBuild.Set(popup,"mergeButton",merge);UiBuild.Set(popup,"sellButton",sell);UiBuild.Set(popup,"closeButton",close);
            });
        }
        private static void BuildStorageLegacy(bool overwrite)
        {
            Build<StoragePopup>("StoragePopup", new Vector2(1740, 980), overwrite, (popup, panel) =>
            {
                Text(panel, "Title", "Tàng Bảo Các", 40, .02f, .91f, .98f, .99f);
                Text currency = Text(panel, "Currencies", "Bạc • Danh Vọng", 23, .03f, .84f, .65f, .91f);
                Text materials = Text(panel, "Material Bag", "Nguyên liệu", 18, .03f, .78f, .97f, .84f);
                Dropdown filter = Dropdown(panel, "Type Filter", new[] { "Tất cả", "Binh Khí", "Hộ Cụ", "Công Pháp", "Đan Dược", "Kỳ Trân" }, .03f, .7f, .24f, .77f);
                Dropdown sort = Dropdown(panel, "Sort", new[] { "Phẩm", "Trọng" }, .26f, .7f, .43f, .77f);
                RectTransform viewport = UiBuild.Image(panel, "Item Viewport", new Color(0, 0, 0, .04f)).rectTransform; Anchor(viewport, .03f, .12f, .54f, .68f); viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
                RectTransform content = UiBuild.Rect(viewport, "Content"); content.anchorMin = content.anchorMax = new Vector2(0, 1); content.pivot = new Vector2(0, 1); content.sizeDelta = new Vector2(850, 0);
                VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 8; layout.childControlHeight = false; layout.childControlWidth = true; content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport; scroll.content = content; scroll.horizontal = false;
                UiButton row = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/Button_Secondary.prefab", panel); row.name = "Item Row Prefab"; row.gameObject.SetActive(false); row.GetComponent<RectTransform>().sizeDelta = new Vector2(820, 60);
                Text detail = Text(panel, "Detail", "Chọn một vật phẩm", 26, .57f, .44f, .97f, .68f); Text cost = Text(panel, "Cost", "", 21, .57f, .33f, .97f, .44f);
                UiButton upgrade = Button(panel, "Upgrade", "Nâng Trọng", true, .65f, .25f); UiButton merge = Button(panel, "Merge", "Hợp Thành", true, .84f, .25f); UiButton sell = Button(panel, "Sell", "Bán", false, .65f, .15f); UiButton close = Button(panel, "Close", "Đóng", false, .84f, .15f);
                UiBuild.Set(popup, "currencyText", currency); UiBuild.Set(popup, "materialText", materials); UiBuild.Set(popup, "typeFilter", filter); UiBuild.Set(popup, "sortOrder", sort); UiBuild.Set(popup, "itemList", content); UiBuild.Set(popup, "itemButtonPrefab", row); UiBuild.Set(popup, "detailText", detail); UiBuild.Set(popup, "costText", cost); UiBuild.Set(popup, "upgradeButton", upgrade); UiBuild.Set(popup, "mergeButton", merge); UiBuild.Set(popup, "sellButton", sell); UiBuild.Set(popup, "closeButton", close);
            });
        }
        private static void BuildDev(bool overwrite)
        {
            Build<DevCheatPanel>("DevCheatPanel", new Vector2(980, 760), overwrite, (popup, panel) =>
            {
                Text(panel, "Title", "Công cụ phát triển (F1)", 38, .05f, .87f, .95f, .98f);
                Dropdown items = Dropdown(panel, "Items", new[] { "item" }, .05f, .74f, .55f, .83f); Dropdown rarity = Dropdown(panel, "Rarity", new[] { "Phàm", "Lương", "Tinh", "Tuyệt", "Thần Binh", "Chí Tôn" }, .57f, .74f, .76f, .83f); Dropdown tier = Dropdown(panel, "Tier", new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }, .78f, .74f, .94f, .83f);
                UiButton addItem = Button(panel, "Add Item", "Thêm vật phẩm", true, .5f, .66f); Dropdown material = Dropdown(panel, "Materials", new[] { "material" }, .05f, .51f, .55f, .6f); UiButton add10 = Button(panel, "Add 10", "+10", false, .67f, .555f); UiButton add100 = Button(panel, "Add 100", "+100", false, .86f, .555f); UiButton silver = Button(panel, "Silver", "+10.000 Bạc", false, .3f, .42f); UiButton reputation = Button(panel, "Reputation", "+1.000 Danh Vọng", false, .7f, .42f); UiButton refresh = Button(panel, "Refresh Shop", "Làm mới Thương Hội miễn phí", false, .3f, .31f); UiButton week = Button(panel, "Next Week", "Sang tuần mới", false, .7f, .31f); UiButton reset = Button(panel, "Reset", "Đặt lại hồ sơ", false, .3f, .16f); UiButton close = Button(panel, "Close", "Đóng", true, .7f, .16f);
                UiBuild.Set(popup, "itemDropdown", items); UiBuild.Set(popup, "rarityDropdown", rarity); UiBuild.Set(popup, "tierDropdown", tier); UiBuild.Set(popup, "materialDropdown", material); UiBuild.Set(popup, "addItemButton", addItem); UiBuild.Set(popup, "add10Button", add10); UiBuild.Set(popup, "add100Button", add100); UiBuild.Set(popup, "addSilverButton", silver); UiBuild.Set(popup, "addReputationButton", reputation); UiBuild.Set(popup, "refreshShopButton", refresh); UiBuild.Set(popup, "nextWeekButton", week); UiBuild.Set(popup, "resetButton", reset); UiBuild.Set(popup, "closeButton", close);
            });
        }
        private static void BuildShopCard(bool overwrite)
        {
            string path = Folder + "/ShopCard.prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var root = new GameObject("ShopCard", typeof(RectTransform), typeof(CanvasGroup)); root.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 300); ShopCard card = root.AddComponent<ShopCard>();
            Image paper = UiBuild.Image(root.transform, "Paper", UiBuild.Paper); UiBuild.Stretch(paper.rectTransform); UiBuild.ApplyUiSprite(paper, "frame_panel", UnityEngine.UI.Image.Type.Sliced);
            Image border = UiBuild.Image(root.transform, "Rarity Border", Color.white); UiBuild.Stretch(border.rectTransform, 3); border.raycastTarget = false; UiBuild.ApplyUiSprite(border, "frame_panel", UnityEngine.UI.Image.Type.Sliced);
            Image icon = UiBuild.Image(root.transform, "Icon", Color.white); Anchor(icon.rectTransform, .08f, .41f, .92f, .94f); icon.preserveAspect = true;
            Text placeholder = Text(root.transform, "Icon Placeholder", "◆\nVật phẩm", 24, .08f, .41f, .92f, .94f); placeholder.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .55f);
            Text itemName = Text(root.transform, "Name", "Tên vật phẩm", 24, .05f, .32f, .95f, .42f); itemName.fontStyle = FontStyle.Bold;
            Text detail = Text(root.transform, "Detail", "Phàm • 1 Trọng", 18, .05f, .25f, .62f, .33f); Text type = Text(root.transform, "Type", "Binh Khí", 17, .62f, .25f, .95f, .33f);
            Text price = Text(root.transform, "Price", "Bạc  1.000", 20, .05f, .14f, .48f, .24f); Text oldPrice = Text(root.transform, "Old Price", "1.200", 15, .47f, .14f, .68f, .24f); oldPrice.color = new Color(.45f, .4f, .36f);
            Text discount = Text(root.transform, "Discount", "-20%", 17, .68f, .14f, .93f, .24f); discount.color = UiBuild.Vermilion;
            UiButton buy = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/Button_Primary.prefab", root.transform); buy.name = "Buy"; buy.SetLabel("Mua"); Anchor(buy.GetComponent<RectTransform>(), .28f, .015f, .72f, .15f);
            Text reason = Text(root.transform, "Disabled Reason", "Không đủ Bạc", 16, .05f, .015f, .95f, .15f); reason.color = UiBuild.Vermilion;
            Image seal = UiBuild.Image(root.transform, "Bought Seal", Color.clear); seal.raycastTarget = false; RectTransform sealRect = seal.rectTransform; sealRect.anchorMin = sealRect.anchorMax = new Vector2(.78f, .16f); sealRect.sizeDelta = new Vector2(105, 105); UiBuild.ApplyUiSprite(seal, "seal_title", UnityEngine.UI.Image.Type.Simple);
            Text sealText = Text(root.transform, "Bought Seal Text", "Đã mua", 23, .64f, .08f, .93f, .25f); sealText.color = UiBuild.Vermilion; sealText.fontStyle = FontStyle.Bold;
            UiBuild.Set(card, "paperPanel", paper); UiBuild.Set(card, "rarityBorder", border); UiBuild.Set(card, "iconImage", icon); UiBuild.Set(card, "placeholderText", placeholder); UiBuild.Set(card, "nameText", itemName); UiBuild.Set(card, "detailText", detail); UiBuild.Set(card, "typeText", type); UiBuild.Set(card, "priceText", price); UiBuild.Set(card, "oldPriceText", oldPrice); UiBuild.Set(card, "discountText", discount); UiBuild.Set(card, "buyButton", buy); UiBuild.Set(card, "disabledReasonText", reason); UiBuild.Set(card, "boughtSeal", seal); UiBuild.Set(card, "boughtSealText", sealText);
            PrefabUtility.SaveAsPrefabAsset(root, path); Object.DestroyImmediate(root);
        }
        private static void BuildShopGrid(bool overwrite)
        {
            Build<ShopPopup>("ShopPopup", new Vector2(1600, 920), overwrite, (popup, panel) =>
            {
                Text(panel, "Title", "Thương Hội", 40, .03f, .92f, .97f, .99f); Text header = Text(panel, "Header", "Bạc • Danh Vọng", 21, .03f, .85f, .76f, .92f); Text status = Text(panel, "Status", "", 18, .03f, .015f, .65f, .065f);
                UiButton refresh = Button(panel, "Refresh", "Làm mới", true, .86f, .885f); refresh.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 58);
                UiButton trade = Button(panel, "Trade Tab", "Thương Hội", true, .18f, .795f); UiButton exchange = Button(panel, "Exchange Tab", "Đổi nguyên liệu", false, .5f, .795f); UiButton reputation = Button(panel, "Reputation Tab", "Tiệm Danh Vọng", false, .82f, .795f);
                RectTransform tradePanel = UiBuild.Rect(panel, "Trade Panel"); Anchor(tradePanel, .03f, .07f, .97f, .75f); RectTransform tradeList = UiBuild.Rect(tradePanel, "Slots"); Anchor(tradeList, 0, 0, 1, 1); GridLayoutGroup grid = tradeList.gameObject.AddComponent<GridLayoutGroup>(); grid.cellSize = new Vector2(420, 300); grid.spacing = new Vector2(28, 28); grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount; grid.constraintCount = 3; grid.childAlignment = TextAnchor.MiddleCenter;
                RectTransform exchangePanel = UiBuild.Rect(panel, "Exchange Panel"); Anchor(exchangePanel, .03f, .07f, .97f, .75f); Dropdown input = Dropdown(exchangePanel, "Input", new[] { "input" }, .08f, .65f, .44f, .78f); Dropdown target = Dropdown(exchangePanel, "Target", new[] { "target" }, .56f, .65f, .92f, .78f); Text preview = Text(exchangePanel, "Preview", "Xem trước", 25, .1f, .42f, .9f, .58f); UiButton exchangeGo = Button(exchangePanel, "Confirm Exchange", "Xác nhận đổi", true, .5f, .28f);
                RectTransform repPanel = UiBuild.Rect(panel, "Reputation Panel"); Anchor(repPanel, .03f, .07f, .97f, .75f); RectTransform repList = UiBuild.Rect(repPanel, "Offers"); Anchor(repList, 0, .25f, 1, .75f); GridLayoutGroup repGrid = repList.gameObject.AddComponent<GridLayoutGroup>(); repGrid.cellSize = new Vector2(420, 300); repGrid.spacing = new Vector2(28, 0); repGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount; repGrid.constraintCount = 1; repGrid.childAlignment = TextAnchor.MiddleCenter;
                ShopCard template = UiBuild.InstantiateLinked<ShopCard>(Folder + "/ShopCard.prefab", panel); template.name = "Shop Card Prefab"; template.gameObject.SetActive(false); UiButton close = Button(panel, "Close", "Đóng", false, .86f, .035f);
                UiBuild.Set(popup, "headerText", header); UiBuild.Set(popup, "statusText", status); UiBuild.Set(popup, "tradeTab", trade); UiBuild.Set(popup, "exchangeTab", exchange); UiBuild.Set(popup, "reputationTab", reputation); UiBuild.Set(popup, "tradePanel", tradePanel.gameObject); UiBuild.Set(popup, "exchangePanel", exchangePanel.gameObject); UiBuild.Set(popup, "reputationPanel", repPanel.gameObject); UiBuild.Set(popup, "slotList", tradeList); UiBuild.Set(popup, "cardPrefab", template); UiBuild.Set(popup, "refreshButton", refresh); UiBuild.Set(popup, "inputDropdown", input); UiBuild.Set(popup, "targetDropdown", target); UiBuild.Set(popup, "exchangePreview", preview); UiBuild.Set(popup, "exchangeButton", exchangeGo); UiBuild.Set(popup, "reputationList", repList); UiBuild.Set(popup, "closeButton", close);
            });
        }
        private static RectTransform List(Transform parent, string name) { RectTransform root = UiBuild.Rect(parent, name); Anchor(root, .03f, .13f, .97f, .96f); VerticalLayoutGroup layout = root.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 8; layout.childControlHeight = false; layout.childControlWidth = true; return root; }
        private static void Build<T>(string name, Vector2 size, bool overwrite, System.Action<T, RectTransform> content) where T : BasePopup
        {
            string path = Folder + "/" + name + ".prefab"; if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas), typeof(GraphicRaycaster)); UiBuild.Stretch(root.GetComponent<RectTransform>()); root.GetComponent<Canvas>().overrideSorting = true; T popup = root.AddComponent<T>(); Image image = UiBuild.InstantiateLinked<Image>(CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab", root.transform); RectTransform panel = image.rectTransform; panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f); panel.sizeDelta = size; UiBuild.Set(popup, "panel", panel); content(popup, panel); root.SetActive(false); PrefabUtility.SaveAsPrefabAsset(root, path); Object.DestroyImmediate(root);
        }
        private static Text Text(Transform p, string name, string value, int size, float x1, float y1, float x2, float y2) { Text text = UiBuild.Text(p, name, value, size); Anchor(text.rectTransform, x1, y1, x2, y2); return text; }
        private static UiButton Button(Transform p, string name, string label, bool primary, float x, float y) { UiButton b = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/Button_" + (primary ? "Primary" : "Secondary") + ".prefab", p); b.name = name; b.SetLabel(label); RectTransform r = b.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = new Vector2(x, y); r.sizeDelta = new Vector2(260, 62); return b; }
        private static Dropdown Dropdown(Transform p, string name, IEnumerable<string> options, float x1, float y1, float x2, float y2) { Dropdown d = UiBuild.InstantiateLinked<Dropdown>(CommonUiPrefabBuilder.Folder + "/Dropdown.prefab", p); d.name = name; d.options = new List<Dropdown.OptionData>(); foreach (string option in options) d.options.Add(new Dropdown.OptionData(option)); d.RefreshShownValue(); Anchor(d.GetComponent<RectTransform>(), x1, y1, x2, y2); return d; }
        private static void Anchor(RectTransform r, float x1, float y1, float x2, float y2) { r.anchorMin = new Vector2(x1, y1); r.anchorMax = new Vector2(x2, y2); r.offsetMin = r.offsetMax = Vector2.zero; }
    }
}
