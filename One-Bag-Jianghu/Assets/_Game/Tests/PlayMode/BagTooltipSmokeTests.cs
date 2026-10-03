using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Play
{
    public sealed class BagTooltipSmokeTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator BagTransfersPersistAndTooltipBehaves()
        {
            DeleteProfile();
            yield return Login("Bag Tooltip Smoke");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Session.ResetProfile();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            profile.Loadout.Clear();
            profile.EnsureStorage(catalog);
            OwnedItem item =
profile.Items.Find(value =>
profile.Storage.Items.Exists(p => p.Uid == value.Uid));
            if (item == null)
            {
                item = profile.Items.Find(value => !profile.Loadout.Exists(p => p.Uid == value.Uid));
                Assert.IsNotNull(item, "The starter profile had no item available for storage.");
                Assert.IsTrue(new StorageGrid(profile, catalog).TryPlaceItem(item));
            }

            GameBootstrap.Session.SaveProfile();
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(), "BagPopup did not open.");
            BagPopup popup = PopupManager.Top as BagPopup;
            BagLoadoutPanel panel = GetField<BagLoadoutPanel>(popup, "bagPanel");
            Assert.GreaterOrEqual(panel.StorageGridRect.rect.width, 400f);
            ItemTrayView tray = GetField<ItemTrayView>(panel, "trayView");
            List<ItemView> storageViews = GetField<List<ItemView>>(tray, "itemViews");
            Assert.AreEqual(profile.Storage.Items.Count,
storageViews.Count,
"The storage grid did not show one item view per stored owned item.");
            ItemView hovered = storageViews.Find(view => view != null && view.Item.Id == item.ItemId);
            Assert.IsNotNull(hovered);
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(640f, 420f)
            };
            ExecuteEvents.Execute(hovered.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return new WaitForSecondsRealtime(0.35f);
            ItemTooltip tooltip = GetField<ItemTooltip>(popup, "itemTooltip");
            Assert.IsTrue(tooltip.IsVisible);
            Assert.That(tooltip.Text, Does.Contain(catalog.Get(item.ItemId).Name));
            Assert.Greater(tooltip.OverlayCanvas.sortingOrder,
popup.GetComponent<Canvas>().sortingOrder);
            ExecuteEvents.Execute(hovered.gameObject, pointer, ExecuteEvents.pointerExitHandler);
            Assert.IsFalse(tooltip.IsVisible);
            ExecuteEvents.Execute(hovered.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return new WaitForSecondsRealtime(0.35f);
            ExecuteEvents.Execute(hovered.gameObject, pointer, ExecuteEvents.beginDragHandler);
            Assert.IsFalse(tooltip.IsVisible);
            Assert.IsTrue(panel.TryMoveStorageToBag(item.Uid, 0, 0));
            GetField<UiButton>(popup, "saveButton").onClick.Invoke();
            yield return WaitUntil(() =>
!PopupManager.IsOpen<BagPopup>(),
"BagPopup did not close after save.");
            Assert.IsNotNull(profile.Loadout.Find(p => p.Uid == item.Uid));
            Assert.IsNull(profile.Storage.Items.Find(p => p.Uid == item.Uid));
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(),
"BagPopup did not reopen.");
            popup = PopupManager.Top as BagPopup;
            panel = GetField<BagLoadoutPanel>(popup, "bagPanel");
            StorageGrid freeGrid = new StorageGrid(profile, catalog);
            catalog.GetItemSize(item.ItemId, out int width, out int height);
            Assert.IsTrue(FindFree(freeGrid,
width,
height,
out int storageX,
out int storageY));
            Assert.IsTrue(panel.TryMoveBagToStorage(item.Uid, storageX, storageY));
            GetField<UiButton>(popup, "saveButton").onClick.Invoke();
            yield return WaitUntil(() =>
!PopupManager.IsOpen<BagPopup>(),
"BagPopup did not close after reverse save.");
            Assert.IsNull(profile.Loadout.Find(p => p.Uid == item.Uid));
            Assert.IsNotNull(profile.Storage.Items.Find(p => p.Uid == item.Uid));
        }

        private static bool FindFree(StorageGrid grid,
int width,
int height,
out int resultX,
out int resultY)
        {
            for (int y = 0; y <= 20 - height; y++)
            {
                for (int x = 0; x <= 10 - width; x++)
                {
                    if (grid.CanPlace(x, y, width, height))
                    {
                        resultX = x;
                        resultY = y;
                        return true;
                    }
                }
            }

            resultX = resultY = -1;
            return false;
        }

        [UnityTest]
        public IEnumerator StorageWeaponDetailUsesSharedDescription()
        {
            DeleteProfile();
            yield return Login("Storage Description Smoke");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Session.ResetProfile();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            OwnedItem weapon =
profile.Items.Find(value =>
string.Equals(catalog.Get(value.ItemId).Type,
"weapon",
StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(weapon);
            profile.Loadout.RemoveAll(p => p.Uid == weapon.Uid);
            if (profile.Storage.Items.Find(p => p.Uid == weapon.Uid) == null)
            {
                Assert.IsTrue(new StorageGrid(profile, catalog).TryPlaceItem(weapon));
            }

            GetField<HomeHotspot>(home, "storageHotspot").OnClick.Invoke();
            yield return WaitUntil(() =>
PopupManager.IsOpen<StoragePopup>(),
"StoragePopup did not open.");
            StoragePopup popup = PopupManager.Top as StoragePopup;
            foreach (GameObject row in GetField<List<GameObject>>(popup, "rows"))
            {
                StorageItemView view = row.GetComponent<StorageItemView>();
                if (view != null)
                {
                    view.OnPointerClick(null);
                }

                Text detail = GetField<Text>(popup, "detailText");
                if (detail.text.Contains("Lực Đạo"))
                {
                    Assert.That(detail.text, Does.Contain("Phổ Công"));
                    yield break;
                }
            }

            Assert.Fail("No weapon detail containing Lực Đạo was found.");
        }
    }
}