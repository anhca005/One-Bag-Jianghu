using System;
using System.Collections.Generic;
using System.IO;
using OneBagJianghu.Bag;
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
            RebuildSectArtLibrary();
            BuildPopup<SectSelectPopup>("SectSelectPopup", new Vector2(1600f, 920f), overwrite, BuildSect);
            BuildPopup<DevCheatPanel>("DevCheatPanel", new Vector2(980f, 900f), overwrite, BuildDev);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("One Bag Jianghu/Build/Sect Popup (Overwrite)")]
        public static void BuildSectOverwrite()
        {
            CommonUiPrefabBuilder.Build(false);
            UiBuild.EnsureFolder(Folder);
            RebuildSectArtLibrary();
            BuildPopup<SectSelectPopup>("SectSelectPopup", new Vector2(1600f, 920f), true, BuildSect);
            AssetDatabase.SaveAssets();
        }

        private static void RebuildSectArtLibrary()
        {
            const string textureFolder = "Assets/_Game/Textures2D/Sects";
            const string libraryPath = "Assets/_Game/Resources/SectArtLibrary.asset";
            AssetDatabase.ImportAsset(
                textureFolder,
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            SectArtLibrary library = AssetDatabase.LoadAssetAtPath<SectArtLibrary>(libraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<SectArtLibrary>();
                AssetDatabase.CreateAsset(library, libraryPath);
            }

            library.Entries.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { textureFolder });
            Array.Sort(guids, CompareSectArtPaths);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }

                if (sprite != null)
                {
                    library.Entries.Add(new SectArtLibrary.Entry
                    {
                        Key = Path.GetFileNameWithoutExtension(path),
                        Sprite = sprite
                    });
                }
            }

            EditorUtility.SetDirty(library);
        }

        private static int CompareSectArtPaths(string leftGuid, string rightGuid)
        {
            string left = AssetDatabase.GUIDToAssetPath(leftGuid);
            string right = AssetDatabase.GUIDToAssetPath(rightGuid);
            return string.CompareOrdinal(left, right);
        }

        private static void BuildSect(SectSelectPopup popup, RectTransform panel)
        {
            // Header and the seven-card navigation column.
            Text(panel, "Title", "Môn Phái", 40, .04f, .91f, .96f, .985f);
            RectTransform cardColumn = UiBuild.Rect(panel, "Sect Choices");
            Anchor(cardColumn, .035f, .105f, .35f, .895f);
            cardColumn.gameObject.AddComponent<TutorialTarget>().Configure("sect_choices");
            var cardButtons = new Button[7];
            var cardBackgrounds = new Image[7];
            var cardArts = new Image[7];
            var cardNames = new Text[7];
            var cardWeapons = new Text[7];
            var cardBadges = new Text[7];
            for (int i = 0; i < cardButtons.Length; i++)
            {
                BuildSectCard(
                    cardColumn,
                    i,
                    out cardButtons[i],
                    out cardBackgrounds[i],
                    out cardArts[i],
                    out cardNames[i],
                    out cardWeapons[i],
                    out cardBadges[i]);
            }

            // Large art banner with an ink veil so the overlaid name remains readable.
            Image heroArt = UiBuild.Image(panel, "Sect Art", new Color(.18f, .14f, .10f, .9f));
            Anchor(heroArt.rectTransform, .375f, .595f, .965f, .895f);
            heroArt.preserveAspect = false;
            heroArt.gameObject.AddComponent<RectMask2D>();
            Image veil = UiBuild.Image(heroArt.transform, "Soft Ink Veil", new Color(.05f, .035f, .025f, .42f));
            Anchor(veil.rectTransform, 0f, 0f, 1f, .34f);
            veil.raycastTarget = false;
            Text sectName = Text(heroArt.transform, "Sect Name", "Tên môn phái", 38, .04f, .03f, .96f, .30f);
            sectName.color = Color.white;
            sectName.gameObject.AddComponent<SingleLineFit>().Configure(38, 22);

            // Lore and weapon specialty sit directly beneath the banner.
            Text lore = Text(panel, "Lore", "Giai thoại môn phái", 19, .39f, .52f, .95f, .585f);
            lore.alignment = TextAnchor.MiddleLeft;
            ShrinkToFit(lore, 19, 14);
            // "Label", not "Title": names containing "Title" get the popup title ribbon.
            Text weaponLabel = Text(panel, "Weapon Label", "Binh khí sở trường:", 19, .39f, .465f, .56f, .515f);
            weaponLabel.alignment = TextAnchor.MiddleLeft;
            weaponLabel.color = UiBuild.Vermilion;
            ShrinkToFit(weaponLabel, 19, 14);
            Text weapons = Text(panel, "Weapon Classes", "Binh khí", 19, .56f, .465f, .95f, .515f);
            weapons.alignment = TextAnchor.MiddleLeft;
            ShrinkToFit(weapons, 19, 13);

            // Tâm Pháp portrait and four compact milestone rows.
            Image tamArt = UiBuild.Image(panel, "Tam Phap Art", new Color(.18f, .14f, .10f, .9f));
            Anchor(tamArt.rectTransform, .39f, .18f, .555f, .445f);
            tamArt.preserveAspect = false;
            Text tamName = Text(panel, "Tam Phap Name", "Tâm Pháp", 24, .39f, .115f, .72f, .175f);
            tamName.alignment = TextAnchor.MiddleLeft;
            tamName.gameObject.AddComponent<SingleLineFit>().Configure(24, 15);
            Text tamLevel = Text(panel, "Tam Phap Level", string.Empty, 20, .72f, .115f, .94f, .175f);
            tamLevel.alignment = TextAnchor.MiddleRight;
            var milestoneTexts = new Text[4];
            var milestoneBackgrounds = new Image[4];
            for (int i = 0; i < milestoneTexts.Length; i++)
            {
                float top = .445f - i * .082f;
                milestoneBackgrounds[i] = UiBuild.Image(
                    panel,
                    "Milestone " + i,
                    new Color(.25f, .20f, .15f, .1f));
                Anchor(milestoneBackgrounds[i].rectTransform, .575f, top - .074f, .965f, top);
                milestoneTexts[i] = Text(
                    milestoneBackgrounds[i].transform,
                    "Description",
                    "Tầng",
                    16,
                    .025f,
                    .05f,
                    .975f,
                    .95f);
                milestoneTexts[i].alignment = TextAnchor.MiddleLeft;
                ShrinkToFit(milestoneTexts[i], 16, 11);
                milestoneTexts[i].supportRichText = true;
            }

            // Actions share the same linked button prefabs as the other economy popups.
            UiButton primary = Button(panel, "Primary Action", "Gia Nhập", true, .75f, .065f);
            UiButton close = Button(panel, "Close", "Để sau", false, .92f, .065f);
            SetObjectArray(popup, "cardButtons", cardButtons);
            SetObjectArray(popup, "cardBackgrounds", cardBackgrounds);
            SetObjectArray(popup, "cardArts", cardArts);
            SetObjectArray(popup, "cardNames", cardNames);
            SetObjectArray(popup, "cardWeapons", cardWeapons);
            SetObjectArray(popup, "cardBadges", cardBadges);
            Set(popup, "heroArt", heroArt);
            Set(popup, "sectName", sectName);
            Set(popup, "loreText", lore);
            Set(popup, "weaponText", weapons);
            Set(popup, "tamPhapArt", tamArt);
            Set(popup, "tamPhapName", tamName);
            Set(popup, "tamPhapLevel", tamLevel);
            SetObjectArray(popup, "milestoneTexts", milestoneTexts);
            SetObjectArray(popup, "milestoneBackgrounds", milestoneBackgrounds);
            Set(popup, "primaryButton", primary);
            Set(popup, "closeButton", close);
        }

        private static void BuildSectCard(
            RectTransform parent,
            int index,
            out Button button,
            out Image background,
            out Image art,
            out Text name,
            out Text weapons,
            out Text badge)
        {
            float top = 1f - index / 7f;
            float bottom = 1f - (index + 1) / 7f + .008f;
            background = UiBuild.Image(parent, "Sect Card " + index, new Color(.12f, .09f, .07f, .28f));
            Anchor(background.rectTransform, 0f, bottom, 1f, top - .008f);
            background.gameObject.AddComponent<RectMask2D>();
            button = background.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            art = UiBuild.Image(background.transform, "Art", new Color(.25f, .20f, .15f, .9f));
            Anchor(art.rectTransform, 0f, 0f, .31f, 1f);
            art.preserveAspect = false;
            name = Text(background.transform, "Name", "Môn phái", 21, .34f, .48f, .82f, .94f);
            name.alignment = TextAnchor.MiddleLeft;
            name.gameObject.AddComponent<SingleLineFit>().Configure(21, 14);
            weapons = Text(background.transform, "Weapons", "Binh khí", 14, .34f, .06f, .97f, .48f);
            weapons.alignment = TextAnchor.MiddleLeft;
            ShrinkToFit(weapons, 14, 10);
            badge = Text(background.transform, "Badge", string.Empty, 13, .80f, .55f, .98f, .92f);
            badge.alignment = TextAnchor.MiddleRight;
            badge.color = UiBuild.Vermilion;
        }

        // Cord and ring hung on the top edge, at the art's own aspect (571x248, ring centre 34 px above its
        // bottom), so the ring sits on the frame line.
        private static void AddCardKnot(Transform card)
        {
            Image knot = UiBuild.Image(card, "Knot", Color.white);
            knot.raycastTarget = false;
            if (!UiBuild.ApplyUiSprite(knot, "Kit/shop_card_knot"))
            {
                UnityEngine.Object.DestroyImmediate(knot.gameObject);
                return;
            }

            RectTransform rect = knot.rectTransform;
            rect.anchorMin = new Vector2(.5f, 1f);
            rect.anchorMax = new Vector2(.5f, 1f);
            rect.pivot = new Vector2(.5f, 34f / 248f);
            rect.sizeDelta = new Vector2(92f, 92f * 248f / 571f);
            rect.anchoredPosition = new Vector2(0f, -8f);
        }

        // Four vermilion rules around a rect: the border of an ink stamp.
        private static void AddStampFrame(RectTransform stamp, float thickness)
        {
            var inward = new Vector2(thickness, thickness);
            AddStampRule(stamp, "Rule Top", new Vector2(0f, 1f), Vector2.one, -inward * Vector2.up);
            AddStampRule(stamp, "Rule Bottom", Vector2.zero, new Vector2(1f, 0f), inward * Vector2.up);
            AddStampRule(stamp, "Rule Left", Vector2.zero, new Vector2(0f, 1f), inward * Vector2.right);
            AddStampRule(stamp, "Rule Right", new Vector2(1f, 0f), Vector2.one, -inward * Vector2.right);
        }

        // A rule hugging one edge; "reach" is how far it extends inward from that edge.
        private static void AddStampRule(
            RectTransform stamp,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 reach)
        {
            Image rule = UiBuild.Image(stamp, name, UiBuild.Vermilion);
            rule.raycastTarget = false;
            rule.rectTransform.anchorMin = anchorMin;
            rule.rectTransform.anchorMax = anchorMax;
            rule.rectTransform.offsetMin = Vector2.Min(reach, Vector2.zero);
            rule.rectTransform.offsetMax = Vector2.Max(reach, Vector2.zero);
        }

        // Best fit only shrinks up to the design size; Unity's default maximum (40) would blow the text up.
        private static void ShrinkToFit(Text text, int maxSize, int minSize)
        {
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = maxSize;
            text.resizeTextMinSize = minSize;
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
            UiBuild.ApplyUiSprite(paper, "Kit/slot_cell_square", Image.Type.Simple);
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
            // Opaque paper backing: inset so it stays within the rounded frame corners.
            Image background = UiBuild.Image(root.transform, "Background", UiBuild.Paper);
            UiBuild.Stretch(background.rectTransform, 4f);
            background.raycastTarget = false;
            Image border = UiBuild.Image(root.transform, "Border", Color.white);
            UiBuild.Stretch(border.rectTransform);
            UiBuild.ApplyUiSprite(border, "Kit/frame_panel_square", Image.Type.Simple);
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
            UiBuild.ApplyUiSprite(pill, "Kit/tier_pill_compact", Image.Type.Simple);
            Text badge = Text(pill.transform, "Badge", "T1", 13, 0f, 0f, 1f, 1f);
            badge.color = UiBuild.Paper;
            badge.alignment = TextAnchor.MiddleCenter;
            badge.gameObject.AddComponent<SingleLineFit>().Configure(13, 11);
            Set(view, "icon", icon);
            Set(view, "background", background);
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
            // tier_pill_compact is 1.75:1.
            rect.sizeDelta = new Vector2(38.5f, 22f);
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
            UiBuild.ApplyUiSprite(sort.GetComponent<Image>(), "Kit/button_small_gold_compact", Image.Type.Simple);
            UiBuild.AddLeadingIcon(sort.transform, "icon_sort");

            // Storage grid.
            RectTransform viewport = UiBuild.Image(panel, "Grid Viewport", new Color(0f, 0f, 0f, .04f)).rectTransform;
            Anchor(viewport, .02f, .08f, .62f, .64f);
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

            // Equipped items: a compact read-only row above the storage grid.
            Text equippedLabel = Text(panel, "Equipped Label", "Đang trang bị", 18, .02f, .65f, .14f, .78f);
            equippedLabel.alignment = TextAnchor.MiddleLeft;
            RectTransform equippedViewport = UiBuild.Image(
                panel,
                "Equipped Viewport",
                new Color(0f, 0f, 0f, .04f)).rectTransform;
            Anchor(equippedViewport, .14f, .65f, .62f, .78f);
            equippedViewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform equippedContent = UiBuild.Rect(equippedViewport, "Equipped Content");
            PrepareEquippedContent(equippedContent);
            ScrollRect equippedScroll = equippedViewport.gameObject.AddComponent<ScrollRect>();
            equippedScroll.viewport = equippedViewport;
            equippedScroll.content = equippedContent;
            equippedScroll.horizontal = true;
            equippedScroll.vertical = false;

            // The description grows inside a clipped, independently scrollable viewport.
            RectTransform detailViewport = UiBuild.Image(panel, "Detail Viewport", Color.clear).rectTransform;
            UiBuild.ApplyUiSprite(detailViewport.GetComponent<Image>(), "Kit/panel_inset_detail", Image.Type.Simple);
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
            UiBuild.AddLeadingIcon(upgrade.transform, "icon_upgrade");
            UiBuild.AddLeadingIcon(merge.transform, "icon_merge");
            TutorialTarget upgradeTarget = upgrade.gameObject.AddComponent<TutorialTarget>();
            upgradeTarget.Configure("storage_upgrade");
            TutorialTarget mergeTarget = merge.gameObject.AddComponent<TutorialTarget>();
            mergeTarget.Configure("storage_merge");
            UiButton sell = Button(panel, "Sell", "Bán", false, .72f, .15f);
            UiBuild.AddLeadingIcon(sell.transform, "icon_sell");
            UiButton close = Button(panel, "Close", "Đóng", false, .89f, .15f);
            BindStorage(popup, header, silver, reputation, warning, filter, sort, scroll, content, cell, item);
            Set(popup, "equippedContent", equippedContent);
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

        private static void PrepareEquippedContent(RectTransform content)
        {
            content.anchorMin = new Vector2(0f, .5f);
            content.anchorMax = new Vector2(0f, .5f);
            content.pivot = new Vector2(0f, .5f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(80f, 0f);
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
            // Flat ink like the common scrollbars; no kit art can fit a handle whose length follows content.
            Image handle = UiBuild.Image(scrollbar.transform, "Handle", UiBuild.Ink);
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
            UiButton clear = Button(panel, "Clear Stage", "Qua ải", false, .8f, .27f);
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
            // shop_card_wide is drawn at the card's own 7:5 shape, so it stretches whole (Simple) without warping.
            // Its footer band is the bottom 21% (button row); the face above holds art, text and price.
            UiBuild.ApplyUiSprite(paper, "Kit/shop_card_wide", Image.Type.Simple);
            Image border = UiBuild.Image(root.transform, "Rarity Border", Color.white);
            UiBuild.Stretch(border.rectTransform, 3f);
            border.raycastTarget = false;
            // ShopCard still tints this serialized Image for rarity, but the wide art owns the only visible frame.
            border.sprite = null;
            border.gameObject.SetActive(false);
            AddCardKnot(root.transform);
            Image icon = UiBuild.Image(root.transform, "Icon", Color.white);
            Anchor(icon.rectTransform, .08f, .6f, .92f, .91f);
            icon.preserveAspect = true;
            // Rarity medallion: top-left of the item-art region.
            Image rarityBadge = UiBuild.Image(root.transform, "Rarity Badge", Color.white);
            SetTopLeftBadgeRect(rarityBadge.rectTransform, 34f, 24f);
            rarityBadge.preserveAspect = true;
            rarityBadge.raycastTarget = false;
            Text placeholder = Text(root.transform, "Icon Placeholder", "◆", 24, .08f, .6f, .92f, .91f);
            placeholder.color = new Color(UiBuild.Ink.r, UiBuild.Ink.g, UiBuild.Ink.b, .55f);
            Text name = Text(root.transform, "Name", "Tên vật phẩm", 21, .05f, .51f, .95f, .6f);
            name.fontStyle = FontStyle.Bold;
            name.gameObject.AddComponent<SingleLineFit>().Configure(21, 14);
            Text detail = Text(root.transform, "Detail", "Phàm • 1 Trọng", 16, .05f, .44f, .66f, .51f);
            detail.alignment = TextAnchor.MiddleLeft;
            detail.gameObject.AddComponent<SingleLineFit>().Configure(16, 12);
            Text type = Text(root.transform, "Type", "Binh Khí", 16, .66f, .44f, .95f, .51f);
            Text stat = Text(root.transform, "Stat", "Khí Huyết +20", 16, .05f, .375f, .95f, .44f);
            stat.alignment = TextAnchor.MiddleLeft;
            stat.gameObject.AddComponent<SingleLineFit>().Configure(16, 12);
            // Price plate: the coin ring (left 640 px of the art, hole centred at x 381) holds the silver icon;
            // the plate is drawn at its native height (464 px / 12.9 = 36) so the ring is not squashed.
            Image priceTag = UiBuild.Image(root.transform, "Price Tag", new Color(UiBuild.Ink.r, UiBuild.Ink.g,
                UiBuild.Ink.b, .12f));
            Anchor(priceTag.rectTransform, .03f, .17f, .64f, .35f);
            bool hasPlate = UiBuild.ApplyUiSprite(priceTag, "Kit/price_tag", Image.Type.Simple);
            priceTag.raycastTarget = false;
            float ring = hasPlate ? 50f : 0f;
            if (hasPlate)
            {
                Image coin = UiBuild.Image(priceTag.transform, "Coin", Color.white);
                RectTransform coinRect = coin.rectTransform;
                coinRect.anchorMin = new Vector2(0f, .5f);
                coinRect.anchorMax = new Vector2(0f, .5f);
                coinRect.anchoredPosition = new Vector2(ring * .595f, 1f);
                coinRect.sizeDelta = new Vector2(ring * .38f, ring * .38f);
                coin.preserveAspect = true;
                coin.raycastTarget = false;
                // icon_bac lives with the PvE art, outside the UI folder ApplyUiSprite reads from.
                coin.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Textures2D/Pve/icon_bac.png");
                if (coin.sprite == null)
                    UnityEngine.Object.DestroyImmediate(coin.gameObject);
            }

            Text price = Text(priceTag.transform, "Price", "Bạc 1.000", 20, 0f, 0f, 1f, 1f);
            price.rectTransform.offsetMin = new Vector2(ring + 6f, 0f);
            price.rectTransform.offsetMax = new Vector2(-14f, 0f);
            price.color = hasPlate ? UiBuild.Paper : UiBuild.Ink;
            Shadow priceShadow = price.gameObject.AddComponent<Shadow>();
            priceShadow.effectColor = new Color(0f, 0f, 0f, .6f);
            Text oldPrice = Text(root.transform, "Old Price", "1.200", 15, .65f, .23f, .79f, .32f);
            oldPrice.color = new Color(.45f, .4f, .36f);
            Text discount = Text(root.transform, "Discount", "-20%", 17, .79f, .23f, .95f, .32f);
            discount.color = UiBuild.Vermilion;
            price.gameObject.AddComponent<SingleLineFit>().Configure(20, 12);
            oldPrice.gameObject.AddComponent<SingleLineFit>().Configure(15, 10);
            discount.gameObject.AddComponent<SingleLineFit>().Configure(17, 11);
            type.gameObject.AddComponent<SingleLineFit>().Configure(17, 11);
            Text reason = Text(root.transform, "Disabled Reason", "Không đủ Bạc", 15, .05f, .32f, .95f, .375f);
            reason.color = UiBuild.Vermilion;
            reason.gameObject.AddComponent<SingleLineFit>().Configure(15, 10);
            UiButton buy = Linked<UiButton>(
                CommonUiPrefabBuilder.Folder + "/Button_Primary.prefab",
                root.transform,
                "Buy");
            buy.SetLabel("Mua");
            // Centred in the footer band (4-19% of the card height, ~45 px).
            Anchor(buy.GetComponent<RectTransform>(), .28f, .035f, .72f, .185f);
            Text buyLabel = buy.GetComponentInChildren<Text>();
            UiBuild.Stretch(buyLabel.rectTransform, 4f);
            SingleLineFit buyFit = buyLabel.GetComponent<SingleLineFit>();
            if (buyFit == null)
            {
                buyFit = buyLabel.gameObject.AddComponent<SingleLineFit>();
            }

            buyFit.Configure(18, 11);
            // "Đã mua" stamp in the footer where the buy button was. It ignores the card's faded
            // CanvasGroup so it stays crisp; a plain ruled frame keeps the label readable.
            Image seal = UiBuild.Image(root.transform, "Bought Seal", new Color(1f, 1f, 1f, .55f));
            seal.raycastTarget = false;
            RectTransform sealRect = seal.rectTransform;
            sealRect.anchorMin = new Vector2(.5f, .11f);
            sealRect.anchorMax = new Vector2(.5f, .11f);
            sealRect.sizeDelta = new Vector2(140f, 38f);
            sealRect.localEulerAngles = new Vector3(0f, 0f, 6f);
            CanvasGroup sealGroup = seal.gameObject.AddComponent<CanvasGroup>();
            sealGroup.ignoreParentGroups = true;
            sealGroup.blocksRaycasts = false;
            AddStampFrame(sealRect, 3f);
            Text sealText = Text(seal.transform, "Bought Seal Text", "Đã mua", 26, 0f, 0f, 1f, 1f);
            sealText.color = UiBuild.Vermilion;
            sealText.fontStyle = FontStyle.Bold;
            sealText.gameObject.AddComponent<SingleLineFit>().Configure(26, 16);
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
            UiBuild.ApplyUiSprite(refresh.GetComponent<Image>(), "Kit/button_small_gold_compact", Image.Type.Simple);
            UiBuild.AddLeadingIcon(refresh.transform, "icon_refresh");
            refresh.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 62f);
            UiButton trade = Button(panel, "Trade Tab", "Thương Hội", true, .18f, .795f);
            UiButton exchange = Button(panel, "Exchange Tab", "Đổi nguyên liệu", false, .5f, .795f);
            UiButton reputationTab = Button(panel, "Reputation Tab", "Tiệm Danh Vọng", false, .82f, .795f);
            UiBuild.ApplyUiSprite(trade.GetComponent<Image>(), "Kit/tab_active", Image.Type.Simple);
            UiBuild.ApplyUiSprite(exchange.GetComponent<Image>(), "Kit/tab_idle", Image.Type.Simple);
            UiBuild.ApplyUiSprite(reputationTab.GetComponent<Image>(), "Kit/tab_idle", Image.Type.Simple);
            // Idle tabs are pale silk: ink labels stay readable (ShopPopup copies these colors per state).
            exchange.GetComponentInChildren<Text>().color = UiBuild.Ink;
            reputationTab.GetComponentInChildren<Text>().color = UiBuild.Ink;
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
            UiBuild.ApplyUiSprite(paper, "Kit/tooltip_tall", Image.Type.Simple);
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
            ApplyPopupFrame(paper, size);
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

        private static void ApplyPopupFrame(Image image, Vector2 size)
        {
            float aspect = size.x / size.y;
            string sprite = aspect >= 1.6f
                ? "Kit/popup_frame_wide"
                : aspect >= 1.37f
                    ? "Kit/popup_frame_medium"
                    : aspect >= 1.18f
                        ? "Kit/popup_frame_result"
                        : aspect >= 1.06f
                            ? "Kit/popup_frame_square"
                            : "Kit/popup_frame";
            UiBuild.ApplyUiSprite(image, sprite, Image.Type.Simple);
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
            if (isDisplay)
            {
                Image ribbon = UiBuild.Image(parent, name + " Ribbon", Color.clear);
                Vector2 center = new Vector2((xMin + xMax) * .5f, (yMin + yMax) * .5f);
                ribbon.rectTransform.anchorMin = center;
                ribbon.rectTransform.anchorMax = center;
                ribbon.rectTransform.sizeDelta = new Vector2(520f, 123f);
                text.rectTransform.anchorMin = center;
                text.rectTransform.anchorMax = center;
                // Keep the title on the ribbon's dark band (about 45% of its 123 px height).
                text.rectTransform.sizeDelta = new Vector2(400f, 54f);
                // The dark band sits in the ribbon's upper half (tassels hang below), so lift the title onto it.
                text.rectTransform.anchoredPosition = new Vector2(0f, 10f);
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 18;
                text.resizeTextMaxSize = size;
                ribbon.raycastTarget = false;
                if (UiBuild.ApplyUiSprite(ribbon, "Kit/popup_title_ribbon", Image.Type.Simple))
                {
                    // Nudge down without resizing: the 520x123 rect is the ribbon art's own aspect.
                    ribbon.rectTransform.anchoredPosition = new Vector2(0f, -6f);
                    ribbon.transform.SetSiblingIndex(text.transform.GetSiblingIndex());
                    text.transform.SetAsLastSibling();
                    text.color = UiBuild.Paper;
                }
                else
                    UnityEngine.Object.DestroyImmediate(ribbon.gameObject);
            }
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
            // The currency icons live with the PvE art, outside the UI folder ApplyUiSprite reads from.
            string iconKey = name == "Silver" ? "icon_bac" : "icon_danh_vong";
            string iconPath = "Assets/_Game/Textures2D/Pve/" + iconKey + ".png";
            icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (icon.sprite == null)
                Debug.LogError("Missing currency icon: " + iconPath);
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
            if (label == "Đóng" && UiBuild.ApplyUiSprite(button.GetComponent<Image>(), "Kit/button_close"))
            {
                rect.sizeDelta = new Vector2(56f, 56f);
                rect.anchorMin = rect.anchorMax = new Vector2(.965f, .965f);
                button.GetComponentInChildren<Text>().gameObject.SetActive(false);
            }
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
            RectTransform rect = dropdown.GetComponent<RectTransform>();
            Anchor(rect, xMin, yMin, xMax, yMax);
            float parentWidth = ((RectTransform)parent).rect.width;
            float parentHeight = ((RectTransform)parent).rect.height;
            float aspect = (xMax - xMin) * parentWidth / ((yMax - yMin) * parentHeight);
            // Art aspects: wide 7.46, full 6.94, popup 6.05, midwide 5.04, narrow 2.1.
            string sprite = aspect >= 7.2f
                ? "Kit/dropdown_field_wide"
                : aspect >= 6.5f
                    ? "Kit/dropdown_field"
                    : aspect >= 5.5f
                        ? "Kit/dropdown_field_popup"
                        : aspect >= 4f
                            ? "Kit/dropdown_field_midwide"
                            : "Kit/dropdown_field_narrow";
            Image field = dropdown.GetComponent<Image>();
            UiBuild.ApplyUiSprite(field, sprite, Image.Type.Simple);
            // The narrow fields (dev panel filters) vary in width; keep the art's shape rather than stretch it.
            field.preserveAspect = sprite == "Kit/dropdown_field_narrow";
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

        private static void SetObjectArray(
            UnityEngine.Object target,
            string field,
            UnityEngine.Object[] values)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Save(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
