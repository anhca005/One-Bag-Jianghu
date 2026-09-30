using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Battle.Replay.View.VfxLab;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Gameplay;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace OneBagJianghu.Tests.Play
{
    /// <summary>Exercises the authored scenes. Any unexpected error log fails a test.</summary>
    public sealed class DemoSmokeTests
    {
        private const float TimeoutSeconds = 10f;
        private const string HeroNameKey = "OneBagJianghu.HeroName";
        private const string SectIdKey = "OneBagJianghu.SectId";
        private bool hadHeroName, hadSectId;
        private string heroName, sectId;
        // The flow test logs in and saves: keep the developer's real profile.json (+ .bak) untouched.
        private static readonly string[] ProfileFiles =
        {
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json"),
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json.bak")
        };

        [SetUp]
        public void BackUpProfile()
        {
            hadHeroName = PlayerPrefs.HasKey(HeroNameKey);
            hadSectId = PlayerPrefs.HasKey(SectIdKey);
            heroName = PlayerPrefs.GetString(HeroNameKey, string.Empty);
            sectId = PlayerPrefs.GetString(SectIdKey, string.Empty);
            foreach (string file in ProfileFiles)
            {
                string backup = file + ".testbackup";
                if (System.IO.File.Exists(backup)) System.IO.File.Delete(backup);
                if (System.IO.File.Exists(file)) System.IO.File.Copy(file, backup);
            }
        }

        [TearDown]
        public void RestoreProfile()
        {
            RestorePreference(HeroNameKey, hadHeroName, heroName);
            RestorePreference(SectIdKey, hadSectId, sectId);
            PlayerPrefs.Save();
            foreach (string file in ProfileFiles)
            {
                string backup = file + ".testbackup";
                if (System.IO.File.Exists(backup)) System.IO.File.Copy(backup, file, true);
                else if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                if (System.IO.File.Exists(backup)) System.IO.File.Delete(backup);
            }

            if (GameBootstrap.Session != null)
            {
                GameBootstrap.Session.HeroName = hadHeroName ? heroName : string.Empty;
                GameBootstrap.Session.SectId = hadSectId ? sectId : string.Empty;
                GameBootstrap.Session.LoadProfile();
            }
        }

        [UnityTest]
        public IEnumerator BagDemoHasPopulatedGrid()
        {
            yield return LoadScene("Assets/_Game/Scenes/BagDemo.unity");
            yield return Frames(10);
            BagGridView grid = UnityEngine.Object.FindFirstObjectByType<BagGridView>();
            Assert.IsNotNull(grid, "BagDemo did not contain an active BagGridView.");
            Assert.IsTrue(grid.gameObject.activeInHierarchy, "BagDemo's bag grid was inactive.");
            AddFirstWeapon(UnityEngine.Object.FindFirstObjectByType<BagLoadoutPanel>());
            Assert.Greater(UnityEngine.Object.FindObjectsByType<ItemView>(FindObjectsSortMode.None).Length, 0,
                "BagDemo's bag grid did not create any item views.");
        }

        [UnityTest]
        public IEnumerator ReplayDemoPlaysToEnd()
        {
            yield return LoadScene("Assets/_Game/Scenes/ReplayDemo.unity");
            yield return Frames(10);
            ReplayScreen replay = UnityEngine.Object.FindFirstObjectByType<ReplayScreen>();
            Assert.IsNotNull(replay, "ReplayDemo did not contain an active ReplayScreen.");
            Button play = GetField<Button>(replay, "playButton");
            Button speed4 = GetField<Button>(replay, "speed4Button");
            bool completed = false;
            replay.Completed += _ => completed = true;
            speed4.onClick.Invoke();
            play.onClick.Invoke();
            yield return WaitUntil(() => completed, TimeoutSeconds,
                "ReplayDemo did not finish within 10 seconds at 4x speed.");
        }

        [UnityTest]
        public IEnumerator VfxLabPlaysAllForTwoSeconds()
        {
            yield return LoadScene("Assets/_Game/Scenes/VfxLab.unity");
            yield return Frames(10);
            VfxLabBootstrap lab = UnityEngine.Object.FindFirstObjectByType<VfxLabBootstrap>();
            Assert.IsNotNull(lab, "VfxLab did not contain an active VfxLabBootstrap.");
            Button playAll = GetField<Button>(lab, "playAllButton");
            Assert.IsTrue(playAll.gameObject.activeInHierarchy, "VfxLab's Play All button was inactive.");
            playAll.onClick.Invoke();
            yield return WaitForRealtime(2f);
        }

        [UnityTest]
        public IEnumerator LoginBattleResultAndReturnHome()
        {
            PlayerPrefs.DeleteKey(HeroNameKey);
            PlayerPrefs.DeleteKey(SectIdKey);
            PlayerPrefs.Save();
            foreach (string file in ProfileFiles)
                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            yield return LoadScene("Assets/_Game/Scenes/LoginScene.unity");
            yield return WaitUntil(() => GameBootstrap.Session != null, TimeoutSeconds,
                "Game services did not initialize after loading LoginScene.");

            LoginController login = UnityEngine.Object.FindFirstObjectByType<LoginController>();
            Assert.IsNotNull(login, "LoginScene did not contain an active LoginController.");
            InputField name = GetField<InputField>(login, "heroName");
            Button enter = GetField<Button>(login, "enterButton");
            name.text = "Kiểm Thử";
            Assert.IsTrue(enter.interactable, "Entering a hero name did not enable the login button.");
            enter.onClick.Invoke();

            yield return WaitForReadyScene("HomeScene", "Login did not reach a ready HomeScene.");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            Assert.IsNotNull(home, "HomeScene did not contain an active HomeController.");
            GameBootstrap.Session.CurrentLoadoutJson = CreateWeaponLoadout(GameBootstrap.Session.HeroName);
            home.RefreshBattleAvailability();
            GameBootstrap.Network.Disconnect();
            GetField<Button>(home, "pveButton").onClick.Invoke();

            yield return WaitForReadyScene("GameplayScene", "Home did not reach a ready GameplayScene.");
            GameplayController gameplay = UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            Assert.IsNotNull(gameplay, "GameplayScene did not contain an active GameplayController.");
            ReplayScreen replay = GetField<ReplayScreen>(gameplay, "replayScreen");
            yield return WaitUntil(() => replay.gameObject.activeInHierarchy, TimeoutSeconds,
                "Starting the battle did not reveal the replay screen.");
            GetField<Button>(replay, "speed4Button").onClick.Invoke();
            Slider scrub = GetField<Slider>(replay, "scrub");
            scrub.value = scrub.maxValue;

            yield return WaitUntil(() => PopupManager.IsOpen<ResultPopup>(), TimeoutSeconds,
                "The battle replay did not finish with a result popup within 10 seconds.");
            ResultPopup result = PopupManager.Top as ResultPopup;
            Assert.IsNotNull(result, "The open popup was not ResultPopup.");
            GetField<UiButton>(result, "homeButton").onClick.Invoke();
            yield return WaitForReadyScene("HomeScene", "The result popup did not return to a ready HomeScene.");
            Assert.IsNotNull(UnityEngine.Object.FindFirstObjectByType<HomeController>(),
                "The final HomeScene did not contain an active HomeController.");
        }

        [UnityTest]
        public IEnumerator DirectHomeCreatesOneGameControllerAndItSurvivesSceneChange()
        {
            yield return LoadScene("Assets/_Game/Scenes/HomeScene.unity");
            yield return WaitUntil(() => GameController.Instance != null, TimeoutSeconds,
                "Direct HomeScene start did not bootstrap GameController.");
            GameController original = GameController.Instance;
            Assert.That(UnityEngine.Object.FindObjectsByType<GameController>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));

            yield return LoadScene("Assets/_Game/Scenes/LoginScene.unity");
            yield return Frames(2);
            Assert.That(GameController.Instance, Is.SameAs(original));
            Assert.That(UnityEngine.Object.FindObjectsByType<GameController>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator HomeWarnsUntilBagHasWeapon()
        {
            yield return LoadScene("Assets/_Game/Scenes/HomeScene.unity");
            yield return WaitUntil(() => GameBootstrap.Session != null, TimeoutSeconds,
                "Game services did not initialize after loading HomeScene.");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            Text warning = GetField<Text>(home, "warningText");
            Button pve = GetField<Button>(home, "pveButton");
            Button pvp = GetField<Button>(home, "pvpButton");

            GameBootstrap.Session.CurrentLoadoutJson = string.Empty;
            home.RefreshBattleAvailability();
            Assert.IsTrue(warning.gameObject.activeSelf);
            Assert.AreEqual("Bao Phục đang trống", warning.text);
            Assert.IsFalse(pve.interactable); Assert.IsFalse(pvp.interactable);

            GameBootstrap.Session.CurrentLoadoutJson = CreateWeaponLoadout("Test");
            home.RefreshBattleAvailability();
            Assert.IsFalse(warning.gameObject.activeSelf);
            Assert.IsTrue(pve.interactable); Assert.IsTrue(pvp.interactable);
        }

        private static string CreateWeaponLoadout(string name)
        {
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            var grid = new BagGrid(5, 4);
            foreach (ItemInfo item in catalog.All)
            {
                if (string.Equals(item.Type, "weapon", StringComparison.OrdinalIgnoreCase) && grid.CanPlace(item, 0, 0))
                {
                    grid.Place(item, 0, 0);
                    return LoadoutSerializer.ToJson(grid, name, 1200);
                }
            }
            Assert.Fail("The item catalog had no weapon that fits the empty 5x4 bag.");
            return string.Empty;
        }

        private static void AddFirstWeapon(BagLoadoutPanel panel)
        {
            Assert.IsNotNull(panel, "The scene did not contain an active BagLoadoutPanel.");
            BagGridView gridView = GetField<BagGridView>(panel, "gridView");
            ItemInfo weapon = null;
            foreach (ItemInfo item in ItemCatalog.LoadFromResources().All)
            {
                if (string.Equals(item.Type, "weapon", StringComparison.OrdinalIgnoreCase) &&
                    gridView.Grid.CanPlace(item, 0, 0))
                {
                    weapon = item;
                    break;
                }
            }
            Assert.IsNotNull(weapon, "The item catalog had no weapon that fits the empty 5x4 bag.");
            gridView.Grid.Place(weapon, 0, 0);
            Assert.IsTrue(panel.HasWeapon, "The test weapon was not present in the gameplay loadout.");
        }

        private static IEnumerator LoadScene(string path)
        {
#if UNITY_EDITOR
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(path,
                new LoadSceneParameters(LoadSceneMode.Single));
            Assert.IsNotNull(operation, "Unity did not start loading scene: " + path);
            float deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            while (!operation.isDone && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsTrue(operation.isDone, "Scene did not load within 10 seconds: " + path);
#else
            Assert.Fail("These PlayMode tests require the Unity Editor.");
            yield break;
#endif
        }

        private static IEnumerator WaitForReadyScene(string sceneName, string failureMessage)
        {
            yield return WaitUntil(() => SceneManager.GetActiveScene().name == sceneName &&
                !SceneFlow.IsTransitioning && !LoadingPanel.IsVisible, TimeoutSeconds, failureMessage);
        }

        private static IEnumerator WaitUntil(Func<bool> condition, float timeout, string failureMessage)
        {
            float deadline = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsTrue(condition(), failureMessage);
        }

        private static IEnumerator WaitForRealtime(float seconds)
        {
            float deadline = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < deadline) yield return null;
        }

        private static IEnumerator Frames(int count)
        {
            for (int i = 0; i < count; i++) yield return null;
        }

        private static T GetField<T>(object target, string name) where T : class
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, target.GetType().Name + "." + name + " field was not found.");
            T value = field.GetValue(target) as T;
            Assert.IsNotNull(value, target.GetType().Name + "." + name + " was not assigned.");
            return value;
        }

        private static void RestorePreference(string key, bool existed, string value)
        {
            if (existed) PlayerPrefs.SetString(key, value);
            else PlayerPrefs.DeleteKey(key);
        }
    }
}
