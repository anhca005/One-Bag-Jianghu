using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Battle.Replay.View.VfxLab;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Gameplay;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Pve;
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
        private static readonly string[] ProfileFiles =
        {
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json"),
            System.IO.Path.Combine(Application.persistentDataPath, "profile.json.bak")
        };
        [UnityTest]
        public IEnumerator BagDemoHasPopulatedGrid()
        {
            yield return LoadScene("Assets/_Game/Scenes/BagDemo.unity");
            yield return Frames(10);
            BagGridView grid = UnityEngine.Object.FindFirstObjectByType<BagGridView>();
            Assert.IsNotNull(grid, "BagDemo did not contain an active BagGridView.");
            Assert.IsTrue(grid.gameObject.activeInHierarchy,
                "BagDemo's bag grid was inactive.");
            AddFirstWeapon(UnityEngine.Object.FindFirstObjectByType<BagLoadoutPanel>());
            Assert.Greater(UnityEngine.Object.FindObjectsByType<ItemView>(FindObjectsSortMode.None).Length,
                0,
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
            yield return WaitUntil(() =>
                completed,
                TimeoutSeconds,
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
            Assert.IsTrue(playAll.gameObject.activeInHierarchy,
                "VfxLab's Play All button was inactive.");
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
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            // A session left over from earlier tests still holds the developer profile in memory.
            GameBootstrap.Session?.LoadProfile();
            yield return LoadScene("Assets/_Game/Scenes/LoginScene.unity");
            yield return WaitUntil(() =>
                GameBootstrap.Session != null,
                TimeoutSeconds,
                "Game services did not initialize after loading LoginScene.");
            LoginController login = UnityEngine.Object.FindFirstObjectByType<LoginController>();
            Assert.IsNotNull(login, "LoginScene did not contain an active LoginController.");
            InputField name = GetField<InputField>(login, "heroName");
            Button enter = GetField<Button>(login, "enterButton");
            name.text = "Kiểm Thử";
            Assert.IsTrue(enter.interactable,
                "Entering a hero name did not enable the login button.");
            enter.onClick.Invoke();
            yield return WaitForReadyScene("HomeScene", "Login did not reach a ready HomeScene.");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            Assert.IsNotNull(home, "HomeScene did not contain an active HomeController.");
            // Gemini's expected stage-1 bag wins ~90% at level 1 (round 3); level 10 (+45 Khí Huyết, #35) makes it certain.
            ApplyProfileForBattle(CreateStageOneLoadout(GameBootstrap.Session.HeroName), 10);
            home.RefreshBattleAvailability();
            GameBootstrap.Network.Disconnect();
            GetField<HomeHotspot>(home, "pveHotspot").OnPointerClick(null);
            yield return WaitUntil(() =>
                PopupManager.IsOpen<StageMapPopup>(),
                TimeoutSeconds,
                "Home PvE entry did not open the stage map.");
            StageMapPopup map = PopupManager.Top as StageMapPopup;
            Assert.IsFalse(GetField<Button>(GetField<System.Collections.Generic.List<GameObject>>(map,
                "nodes")[1].GetComponent<PveStageNode>(),
                "button").interactable,
                "Locked stage node was interactable.");
            GetField<UiButton>(map, "challengeButton").onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>(),
                TimeoutSeconds,
                "Stage before-dialogue did not open.");
            GetField<UiButton>(PopupManager.Top, "skipButton").onClick.Invoke();
            yield return WaitForReadyScene("GameplayScene",
                "Home did not reach a ready GameplayScene.");
            GameplayController gameplay =
                UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            Assert.IsNotNull(gameplay,
                "GameplayScene did not contain an active GameplayController.");
            Assert.That(GetField<string>(gameplay,
                "opponentName"),
                Is.EqualTo(OneBagJianghu.Pve.PveCatalog.Load().GetStage("hoi_1_01").Enemy.Name));
            ReplayScreen replay = GetField<ReplayScreen>(gameplay, "replayScreen");
            yield return WaitUntil(() =>
                replay.gameObject.activeInHierarchy,
                TimeoutSeconds,
                "Starting the battle did not reveal the replay screen.");
            GetField<Button>(replay, "speed4Button").onClick.Invoke();
            Slider scrub = GetField<Slider>(replay, "scrub");
            scrub.value = scrub.maxValue;
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>(),
                TimeoutSeconds,
                "The battle replay did not finish within 10 seconds.");
            GetField<UiButton>(PopupManager.Top, "skipButton").onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.Top is ResultPopup,
                TimeoutSeconds,
                "Result did not remain after after-dialogue.");
            ResultPopup result = PopupManager.Top as ResultPopup;
            Assert.IsNotNull(result, "The open popup was not ResultPopup.");
            Assert.AreEqual("Về bản đồ",
                GetField<UiButton>(result,
                "homeButton").GetComponentInChildren<Text>().text);
            GetField<UiButton>(result, "homeButton").onClick.Invoke();
            yield return WaitForReadyScene("HomeScene",
                "The result popup did not return to a ready HomeScene.");
            Assert.IsNotNull(UnityEngine.Object.FindFirstObjectByType<HomeController>(),
                "The final HomeScene did not contain an active HomeController.");
            Assert.IsTrue(GameBootstrap.Session.Profile.Pve.Stages["hoi_1_01"].Cleared);
            Assert.IsTrue(OneBagJianghu.Pve.PveRules.IsStageUnlocked(
                OneBagJianghu.Pve.PveCatalog.Load().GetStage("hoi_1_02"),
                OneBagJianghu.Pve.PveCatalog.Load(),
                GameBootstrap.Session.Profile));
            yield return WaitUntil(() =>
                PopupManager.IsOpen<StageMapPopup>(),
                TimeoutSeconds,
                "Returning from PvE did not reopen the stage map.");
            // Level 10 also shows the one-time sect prompt on top.
            map = UnityEngine.Object.FindFirstObjectByType<StageMapPopup>();
            Assert.AreEqual("hoi_1_01",
                GetField<OneBagJianghu.Pve.PveStage>(map,
                "selected").Id);
            Assert.IsTrue(GetField<Button>(GetField<System.Collections.Generic.List<GameObject>>(map,
                "nodes")[1].GetComponent<PveStageNode>(),
                "button").interactable,
                "Stage 2 was not unlocked on the reopened map.");
        }

        [UnityTest]
        public IEnumerator DirectHomeCreatesOneGameControllerAndItSurvivesSceneChange()
        {
            yield return LoadScene("Assets/_Game/Scenes/HomeScene.unity");
            yield return WaitUntil(() =>
                GameController.Instance != null,
                TimeoutSeconds,
                "Direct HomeScene start did not bootstrap GameController.");
            GameController original = GameController.Instance;
            Assert.That(UnityEngine.Object.FindObjectsByType<GameController>(FindObjectsInactive.Include,
                FindObjectsSortMode.None),
                Has.Length.EqualTo(1));
            yield return LoadScene("Assets/_Game/Scenes/LoginScene.unity");
            yield return Frames(2);
            Assert.That(GameController.Instance, Is.SameAs(original));
            Assert.That(UnityEngine.Object.FindObjectsByType<GameController>(FindObjectsInactive.Include,
                FindObjectsSortMode.None),
                Has.Length.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator HomeWarnsUntilBagHasWeapon()
        {
            yield return LoadScene("Assets/_Game/Scenes/HomeScene.unity");
            yield return WaitUntil(() =>
                GameBootstrap.Session != null,
                TimeoutSeconds,
                "Game services did not initialize after loading HomeScene.");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            HomeHotspot pve = GetField<HomeHotspot>(home, "pveHotspot");
            HomeHotspot pvp = GetField<HomeHotspot>(home, "pvpHotspot");
            Assert.IsNull(GameObject.Find("Btn_Pve"));
            Assert.IsNull(GameObject.Find("Btn_Pvp"));
            ApplyProfileForBattle(string.Empty, GameBootstrap.Session.HeroLevel);
            home.RefreshBattleAvailability();
            Assert.IsFalse(pve.IsLocked);
            Assert.IsFalse(pvp.IsLocked);
            pve.OnPointerClick(null);
            yield return WaitUntil(() =>
                PopupManager.IsOpen<MessagePopup>(),
                TimeoutSeconds,
                "The PvE hotspot did not explain the invalid loadout.");
            Assert.IsFalse(PopupManager.IsOpen<StageMapPopup>());
            PopupManager.Close<MessagePopup>();
            yield return WaitUntil(() =>
                !PopupManager.IsOpen<MessagePopup>(),
                TimeoutSeconds,
                "Loadout warning did not close.");
            ApplyProfileForBattle(CreateWeaponLoadout("Test"), GameBootstrap.Session.HeroLevel);
            home.RefreshBattleAvailability();
            Assert.IsFalse(pve.IsLocked);
            Assert.IsFalse(pvp.IsLocked);
        }

        private static string CreateStageOneLoadout(string name)
        {
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            var grid = new BagGrid(5, 4);
            grid.Place(catalog.Get("thiet_cot_quyen_sao"), 0, 0);
            grid.Place(catalog.Get("thanh_phong_kiem"), 1, 0);
            grid.Place(catalog.Get("o_tam_y"), 3, 0);
            return LoadoutSerializer.ToJson(grid, name, 1);
        }

        private static void ApplyProfileForBattle(string loadoutJson, int heroLevel)
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.HeroLevel = heroLevel;
            profile.Items.Clear();
            profile.Loadout.Clear();
            profile.Storage.Items.Clear();
            profile.Storage.Overflow.Clear();
            if (!string.IsNullOrEmpty(loadoutJson))
            {
                BagGrid grid = LoadoutSerializer.FromJson(loadoutJson, ItemCatalog.LoadFromResources());
                foreach (PlacedItem placed in grid.Items)
                {
                    OwnedItem item = profile.AddItem(placed.Item.Id, placed.Tier);
                    profile.Loadout.Add(new LoadoutPlacement
                    {
                        Uid = item.Uid,
                        X = placed.X,
                        Y = placed.Y
                    });
                }
            }

            GameBootstrap.Session.ApplyProfileForTests(profile);
        }

        private static string CreateWeaponLoadout(string name)
        {
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            var grid = new BagGrid(5, 4);
            foreach (ItemInfo item in catalog.All)
            {
                if (string.Equals(item.Type,
                    "weapon",
                    StringComparison.OrdinalIgnoreCase) &&
                    grid.CanPlace(item,
                    0,
                    0))
                {
                    grid.Place(item, 0, 0);
                    return LoadoutSerializer.ToJson(grid, name, 1);
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
                if (string.Equals(item.Type,
                    "weapon",
                    StringComparison.OrdinalIgnoreCase) &&
                    gridView.Grid.CanPlace(item,
                    0,
                    0))
                {
                    weapon = item;
                    break;
                }
            }

            Assert.IsNotNull(weapon,
                "The item catalog had no weapon that fits the empty 5x4 bag.");
            gridView.Grid.Place(weapon, 0, 0);
            Assert.IsTrue(panel.HasWeapon,
                "The test weapon was not present in the gameplay loadout.");
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
            yield return WaitUntil(() =>
                SceneManager.GetActiveScene().name == sceneName &&
                !SceneFlow.IsTransitioning &&
                !LoadingPanel.IsVisible,
                TimeoutSeconds,
                failureMessage);
        }

        private static IEnumerator WaitUntil(Func<bool> condition, float timeout, string failureMessage)
        {
            float deadline = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.IsTrue(condition(), failureMessage);
        }

        private static IEnumerator WaitForRealtime(float seconds)
        {
            float deadline = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }
        }

        private static IEnumerator Frames(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return null;
            }
        }

        private static T GetField<T>(object target, string name)
            where T : class
        {
            FieldInfo field =
                target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field,
                target.GetType().Name + "." + name + " field was not found.");
            T value = field.GetValue(target) as T;
            Assert.IsNotNull(value,
                target.GetType().Name + "." + name + " was not assigned.");
            return value;
        }

    }
}
