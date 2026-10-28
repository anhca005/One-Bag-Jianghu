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

namespace OneBagJianghu.Tests.Play
{
    public sealed class DragOutsideGridTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator BagPlacedItemDroppedOutsideReturnsToOldCellWithoutDuplication()
        {
            DeleteProfile();
            yield return Login("Bag Outside Drag");
            HomeController home;
            PlayerProfile profile;
            OwnedItem item;
            PrepareSingleStoredItem(out home, out profile, out item);
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(), "BagPopup did not open.");
            BagLoadoutPanel panel = GetField<BagLoadoutPanel>(PopupManager.Top, "bagPanel");
            Assert.IsTrue(panel.TryMoveStorageToBag(item.Uid, 0, 0));
            yield return null;
            BagGridView grid = GetField<BagGridView>(panel, "gridView");
            PlacedItem placed = grid.Grid.Items[0];
            int oldX = placed.X, oldY = placed.Y;
            ItemView view = FindItemView(panel.transform, item.ItemId);
            Drag(view.gameObject,
                Centre(view.RectTransform),
                new Vector2(-100f,
                -100f));
            yield return null;
            Assert.AreEqual(oldX, grid.Grid.Items[0].X);
            Assert.AreEqual(oldY, grid.Grid.Items[0].Y);
            Assert.IsEmpty(profile.Loadout, "The persisted loadout changed before Save.");
            Assert.AreEqual(1, CountViews(panel.transform, item.ItemId));
        }

        [UnityTest]
        public IEnumerator BagTrayItemDroppedOutsideReturnsAndStaysUnique()
        {
            DeleteProfile();
            yield return Login("Tray Outside Drag");
            HomeController home;
            PlayerProfile profile;
            OwnedItem item;
            PrepareSingleStoredItem(out home, out profile, out item);
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(), "BagPopup did not open.");
            BagLoadoutPanel panel = GetField<BagLoadoutPanel>(PopupManager.Top, "bagPanel");
            ItemView view = FindItemView(panel.StorageGridRect, item.ItemId);
            Vector2 before = view.RectTransform.anchoredPosition;
            Drag(view.gameObject,
                Centre(view.RectTransform),
                new Vector2(-100f,
                -100f));
            yield return null;
            Assert.Less(Vector2.Distance(view.RectTransform.anchoredPosition,
                before),
                .1f);
            Assert.AreEqual(1, CountViews(panel.transform, item.ItemId));
            Assert.IsEmpty(profile.Loadout);
        }

        [UnityTest]
        public IEnumerator StorageDropOutsideRestoresPlacementAndCount()
        {
            DeleteProfile();
            yield return Login("Storage Outside Drag");
            HomeController home;
            PlayerProfile profile;
            OwnedItem item;
            PrepareSingleStoredItem(out home, out profile, out item);
            StorageItemPlacement placement = profile.Storage.Items.Find(p => p.Uid == item.Uid);
            int x = placement.X, y = placement.Y, count = profile.Storage.Items.Count;
            GetField<HomeHotspot>(home, "storageHotspot").OnClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<StoragePopup>(),
                "StoragePopup did not open.");
            StoragePopup popup = PopupManager.Top as StoragePopup;
            StorageItemView view = FindStorageView(popup, placement);
            LogAssert.Expect(LogType.Warning,
                new System.Text.RegularExpressions.Regex("\\[Storage\\] Move refused: .* -> outside grid"));
            Drag(view.gameObject,
                Centre((RectTransform)view.transform),
                new Vector2(-100f,
                -100f));
            yield return null;
            Assert.AreEqual(x, placement.X);
            Assert.AreEqual(y, placement.Y);
            Assert.AreEqual(count, profile.Storage.Items.Count);
            Assert.AreEqual(1, CountStorageViews(popup, placement));
        }

        [UnityTest]
        public IEnumerator InterruptedDragRestoresAndDraggedImageIsCentredOnPointer()
        {
            DeleteProfile();
            yield return Login("Interrupted Drag");
            HomeController home;
            PlayerProfile profile;
            OwnedItem item;
            PrepareSingleStoredItem(out home, out profile, out item);
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(), "BagPopup did not open.");
            BagPopup popup = PopupManager.Top as BagPopup;
            BagLoadoutPanel panel = GetField<BagLoadoutPanel>(popup, "bagPanel");
            ItemView view = FindItemView(panel.StorageGridRect, item.ItemId);
            Vector2 pointer = new Vector2(Screen.width * .43f, Screen.height * .57f);
            var data = new PointerEventData(EventSystem.current)
            {
                position = pointer
            };
            ExecuteEvents.Execute(view.gameObject, data, ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(view.gameObject, data, ExecuteEvents.dragHandler);
            Assert.Less(Vector2.Distance(Centre(view.RectTransform),
                pointer),
                4f,
                "Dragged image was not centred on the cursor.");
            GetField<UiButton>(popup, "closeButton").onClick.Invoke();
            yield return WaitUntil(() =>
                !popup.gameObject.activeInHierarchy,
                "BagPopup did not finish closing.");
            yield return null;
            Assert.AreEqual(1, profile.Storage.Items.Count);
            Assert.AreEqual(0,
                UnityEngine.Object.FindObjectsByType<ItemView>(FindObjectsSortMode.None).Length,
                "A drag ghost survived popup disable.");
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(),
                "BagPopup did not reopen.");
            panel = GetField<BagLoadoutPanel>(PopupManager.Top, "bagPanel");
            Assert.AreEqual(1, CountViews(panel.transform, item.ItemId));
        }

        private static void PrepareSingleStoredItem(out HomeController home,
            out PlayerProfile profile,
            out OwnedItem item)
        {
            home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            profile = GameBootstrap.Session.Profile;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            OwnedItem kept = profile.Items[0];
            item = kept;
            profile.Loadout.Clear();
            profile.Storage.Items.Clear();
            profile.Storage.MaterialStacks.Clear();
            profile.Storage.Overflow.Clear();
            profile.Materials.Clear();
            profile.Items.RemoveAll(value => value.Uid != kept.Uid);
            Assert.IsTrue(new StorageGrid(profile, catalog).TryPlaceItem(kept));
            GameBootstrap.Session.ApplyProfileForTests(profile);
            profile = GameBootstrap.Session.Profile;
        }

        private static void Drag(GameObject target, Vector2 start, Vector2 end)
        {
            var data = new PointerEventData(EventSystem.current)
            {
                position = start
            };
            ExecuteEvents.Execute(target, data, ExecuteEvents.beginDragHandler);
            data.position = end;
            ExecuteEvents.Execute(target, data, ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(target, data, ExecuteEvents.endDragHandler);
        }

        private static Vector2 Centre(RectTransform rect) =>
            RectTransformUtility.WorldToScreenPoint(null,
            rect.TransformPoint(rect.rect.center));
        private static ItemView FindItemView(Transform root, string id)
        {
            foreach (ItemView view in root.GetComponentsInChildren<ItemView>())
            {
                if (view.Item != null && view.Item.Id == id)
                    return view;
            }

            Assert.Fail("ItemView not found: " + id);
            return null;
        }

        private static int CountViews(Transform root, string id)
        {
            int count = 0;
            foreach (ItemView view in root.GetComponentsInChildren<ItemView>())
            {
                if (view.Item != null && view.Item.Id == id)
                {
                    count++;
                }
            }

            return count;
        }

        private static StorageItemView FindStorageView(StoragePopup popup, StorageItemPlacement placement)
        {
            foreach (GameObject row in GetField<List<GameObject>>(popup, "rows"))
            {
                Vector2 p = ((RectTransform)row.transform).anchoredPosition;
                if (Mathf.Approximately(p.x,
                    placement.X * 80f) &&
                    Mathf.Approximately(p.y,
                    -placement.Y * 80f))
                    return row.GetComponent<StorageItemView>();
            }

            Assert.Fail("Storage view not found.");
            return null;
        }

        private static int CountStorageViews(StoragePopup popup, StorageItemPlacement placement)
        {
            int count = 0;
            foreach (GameObject row in GetField<List<GameObject>>(popup, "rows"))
            {
                Vector2 p = ((RectTransform)row.transform).anchoredPosition;
                if (Mathf.Approximately(p.x,
                    placement.X * 80f) &&
                    Mathf.Approximately(p.y,
                    -placement.Y * 80f))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
