using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace OneBagJianghu.Tests.Play
{
    public abstract class SmokeTestBase
    {
        protected const float TimeoutSeconds = 10f;
        private const string HeroKey = "OneBagJianghu.HeroName";
        private const string SectKey = "OneBagJianghu.SectId";
        private bool hadHero, hadSect;
        private string hero, sect;
        private static readonly string[] Files =
        {
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json"),
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json.bak")
        };

        [SetUp]
        public void BackUpProfile()
        {
            hadHero = PlayerPrefs.HasKey(HeroKey);
            hadSect = PlayerPrefs.HasKey(SectKey);
            hero = PlayerPrefs.GetString(HeroKey, string.Empty);
            sect = PlayerPrefs.GetString(SectKey, string.Empty);
            foreach (string file in Files)
            {
                string backup = file + ".testbackup";
                if (System.IO.File.Exists(backup)) System.IO.File.Delete(backup);
                if (System.IO.File.Exists(file)) System.IO.File.Copy(file, backup);
            }
        }

        [TearDown]
        public void RestoreProfile()
        {
            RestorePref(HeroKey, hadHero, hero);
            RestorePref(SectKey, hadSect, sect);
            PlayerPrefs.Save();
            foreach (string file in Files)
            {
                string backup = file + ".testbackup";
                if (System.IO.File.Exists(backup)) System.IO.File.Copy(backup, file, true);
                else if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                if (System.IO.File.Exists(backup)) System.IO.File.Delete(backup);
            }
            if (GameBootstrap.Session == null) return;
            GameBootstrap.Session.HeroName = hadHero ? hero : string.Empty;
            GameBootstrap.Session.SectId = hadSect ? sect : string.Empty;
            GameBootstrap.Session.LoadProfile();
        }

        protected static void DeleteProfile()
        {
            PlayerPrefs.DeleteKey(HeroKey);
            PlayerPrefs.DeleteKey(SectKey);
            foreach (string file in Files)
                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        }

        protected static IEnumerator Login(string name)
        {
#if UNITY_EDITOR
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(
                "Assets/_Game/Scenes/LoginScene.unity", new LoadSceneParameters(LoadSceneMode.Single));
            while (!operation.isDone) yield return null;
#else
            Assert.Fail("These PlayMode tests require the Unity Editor.");
#endif
            yield return WaitUntil(() => GameBootstrap.Session != null, "Game services did not initialize.");
            LoginController login = UnityEngine.Object.FindFirstObjectByType<LoginController>();
            GetField<InputField>(login, "heroName").text = name;
            GetField<Button>(login, "enterButton").onClick.Invoke();
            yield return WaitUntil(() => SceneManager.GetActiveScene().name == "HomeScene" &&
                !SceneFlow.IsTransitioning && !LoadingPanel.IsVisible, "Login did not reach HomeScene.");
        }

        protected static IEnumerator WaitUntil(Func<bool> condition, string message)
        {
            float deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsTrue(condition(), message);
        }

        protected static T GetField<T>(object target, string name) where T : class
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, target.GetType().Name + "." + name + " was not found.");
            T value = field.GetValue(target) as T;
            Assert.IsNotNull(value, target.GetType().Name + "." + name + " was not assigned.");
            return value;
        }

        private static void RestorePref(string key, bool existed, string value)
        {
            if (existed) PlayerPrefs.SetString(key, value);
            else PlayerPrefs.DeleteKey(key);
        }
    }

    public sealed class StorageSmokeTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator StorageUpgradesMergesAndMovesStarterWeapon()
        {
            DeleteProfile();
            yield return Login("Storage Smoke");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Session.ResetProfile();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            OwnedItem weapon = profile.Items.Find(item =>
                string.Equals(catalog.Get(item.ItemId).Type, "weapon", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(weapon);
            profile.Loadout.RemoveAll(value => value.Uid == weapon.Uid);
            Assert.IsTrue(new StorageGrid(profile, catalog).TryPlaceItem(weapon));
            profile.AddCurrency("bac", 10000);
            profile.AddMaterial("han_thiet", 100);
            profile.AddMaterial("binh_hon_tinh_thach", 10);
            GameBootstrap.Session.SaveProfile();

            GetField<HomeHotspot>(home, "storageHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<StoragePopup>(), "StoragePopup did not open.");
            StoragePopup popup = PopupManager.Top as StoragePopup;
            StorageItemView view = ViewFor(popup, profile, weapon.Uid);
            Assert.IsNotNull(view, "No view existed at the weapon's placed cells.");

            for (int tier = 2; tier <= 3; tier++)
            {
                view.OnPointerClick(null);
                UpgradePreview preview = GetField<UpgradeService>(popup, "upgrade").Preview(profile, weapon.Uid);
                long silver = profile.Currencies["bac"];
                var before = new Dictionary<string, int>();
                foreach (KeyValuePair<string, int> cost in preview.Materials)
                    before[cost.Key] = profile.Materials[cost.Key];
                GetField<UiButton>(popup, "upgradeButton").onClick.Invoke();
                Assert.AreEqual(tier, weapon.Tier);
                Assert.AreEqual(silver - preview.Silver, profile.Currencies["bac"]);
                foreach (KeyValuePair<string, int> cost in preview.Materials)
                    Assert.AreEqual(before[cost.Key] - cost.Value, profile.Materials[cost.Key]);
                view = ViewFor(popup, profile, weapon.Uid);
            }

            OwnedItem copy = profile.AddItem(weapon.ItemId, 3, rarity: weapon.Rarity);
            Assert.IsTrue(new StorageGrid(profile, catalog).TryPlaceItem(copy));
            popup.Refresh();
            ViewFor(popup, profile, weapon.Uid).OnPointerClick(null);
            GetField<UiButton>(popup, "mergeButton").onClick.Invoke();
            ViewFor(popup, profile, copy.Uid).OnPointerClick(null);
            yield return WaitUntil(() => PopupManager.IsOpen<ConfirmPopup>(), "Merge confirmation did not open.");
            GetField<UiButton>(PopupManager.Top, "confirmButton").onClick.Invoke();
            yield return WaitUntil(() => !PopupManager.IsOpen<ConfirmPopup>(), "Merge confirmation did not close.");
            List<OwnedItem> merged = profile.Items.FindAll(item => item.ItemId == weapon.ItemId);
            Assert.AreEqual(1, merged.Count);
            Assert.AreEqual("uncommon", merged[0].Rarity);
            Assert.IsNotNull(profile.Storage.Items.Find(p => p.Uid == merged[0].Uid));
            Assert.AreEqual(UsedCells(profile, catalog), new StorageGrid(profile, catalog).UsedCells);

            StorageItemPlacement placement = profile.Storage.Items.Find(p => p.Uid == merged[0].Uid);
            StorageGrid grid = new StorageGrid(profile, catalog);
            catalog.GetItemSize(merged[0].ItemId, out int width, out int height);
            Assert.IsTrue(FindFree(grid, placement, width, height, out int x, out int y));
            RectTransform content = GetField<RectTransform>(popup, "gridContent");
            Vector3 world = content.TransformPoint(new Vector3(x * 80f + 1f, -y * 80f - 1f));
            Assert.IsTrue(popup.CommitDrop(merged[0].Uid, false,
                RectTransformUtility.WorldToScreenPoint(null, world), null));
            Assert.AreEqual(x, placement.X);
            Assert.AreEqual(y, placement.Y);
            GetField<UiButton>(popup, "closeButton").onClick.Invoke();
            yield return WaitUntil(() => !PopupManager.IsOpen<StoragePopup>(), "StoragePopup did not close.");
            // Errors and exceptions already fail the test; tagged Debug.Log lines are expected (decision #29).
        }

        private static StorageItemView ViewFor(StoragePopup popup, PlayerProfile profile, string uid)
        {
            StorageItemPlacement p = profile.Storage.Items.Find(value => value.Uid == uid);
            Assert.IsNotNull(p);
            foreach (GameObject row in GetField<List<GameObject>>(popup, "rows"))
            {
                Vector2 position = ((RectTransform)row.transform).anchoredPosition;
                if (Mathf.Approximately(position.x, p.X * 80f) && Mathf.Approximately(position.y, -p.Y * 80f))
                    return row.GetComponent<StorageItemView>();
            }
            return null;
        }

        private static bool FindFree(StorageGrid grid, StorageItemPlacement p, int width, int height,
            out int resultX, out int resultY)
        {
            for (int y = 0; y <= 20 - height; y++)
            for (int x = 0; x <= 10 - width; x++)
                if ((x != p.X || y != p.Y) && grid.CanPlace(x, y, width, height, p.Uid))
                { resultX = x; resultY = y; return true; }
            resultX = resultY = -1;
            return false;
        }

        private static int UsedCells(PlayerProfile profile, ItemCatalog catalog)
        {
            int result = profile.Storage.MaterialStacks.Count;
            foreach (StorageItemPlacement p in profile.Storage.Items)
            {
                catalog.GetItemSize(profile.RequireItem(p.Uid).ItemId, out int width, out int height);
                result += width * height;
            }
            return result;
        }
    }
}
