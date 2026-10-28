using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
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
    /// <summary>End-to-end coverage for the PvE result popup actions.</summary>
    public sealed class PveResultFlowTests
    {
        private const float TimeoutSeconds = 10f;
        private const string HeroNameKey = "OneBagJianghu.HeroName";
        private const string SectIdKey = "OneBagJianghu.SectId";
        private static readonly string[] ProfileFiles =
        {
            Path.Combine(Application.persistentDataPath, "profile.json"),
            Path.Combine(Application.persistentDataPath, "profile.json.bak")
        };
        [UnityTest]
        public IEnumerator WinCanContinueToNextStageAndFinishAnotherBattle()
        {
            yield return StartFreshHome(CreateStageOneLoadout, 10);
            yield return StartStageFromHome();
            GameplayController gameplay =
                UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            Assert.IsNotNull(gameplay);
            Assert.That(GetField<string>(gameplay,
                "opponentName"),
                Is.EqualTo(PveCatalog.Load().GetStage("hoi_1_01").Enemy.Name));
            yield return FinishBattleAndOpenResult();
            ResultPopup result = FindPopup<ResultPopup>();
            UiButton continueButton = GetField<UiButton>(result, "playAgainButton");
            Assert.That(ButtonLabel(continueButton), Is.EqualTo("Ải tiếp theo"));
            Assert.IsTrue(continueButton.GetComponent<Button>().interactable);
            PveStage next = PveCatalog.Load().GetStage("hoi_1_02");
            int staminaBefore = GameBootstrap.Session.Profile.Pve.Stamina;
            continueButton.onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>(),
                TimeoutSeconds,
                "Continuing did not open the next stage's before-dialogue.");
            Assert.AreEqual(staminaBefore - next.StaminaCost,
                GameBootstrap.Session.Profile.Pve.Stamina);
            Assert.AreEqual("hoi_1_02", GameBootstrap.Session.CurrentPveStageId);
            GameplayController previous = gameplay;
            SkipTopDialogue();
            // GameplayScene is reloaded over itself: wait for the new controller, not just the scene name.
            yield return WaitUntil(() =>
            {
                GameplayController current =
                    UnityEngine.Object.FindFirstObjectByType<GameplayController>();
                return previous == null && current != null || current != null && current != previous;
            },
                TimeoutSeconds,
                "Continuing did not reload GameplayScene.");
            yield return WaitForReadyScene("GameplayScene",
                "Continuing did not load a ready GameplayScene.");
            gameplay = UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            Assert.IsNotNull(gameplay);
            Assert.That(GetField<string>(gameplay,
                "opponentName"),
                Is.EqualTo(next.Enemy.Name));
            yield return FinishBattleAndOpenResult();
            Assert.IsNotNull(FindPopup<ResultPopup>(),
                "The continued battle did not show another result popup.");
        }

        [UnityTest]
        public IEnumerator LossCanRetrySameStageThroughBeforeDialogue()
        {
            yield return StartFreshHome(CreateLosingLoadout, 1);
            yield return StartStageFromHome();
            yield return FinishBattleAndOpenResult();
            ResultPopup result = FindPopup<ResultPopup>();
            UiButton retryButton = GetField<UiButton>(result, "playAgainButton");
            Assert.That(ButtonLabel(retryButton),
                Is.EqualTo("Đánh lại"),
                "The deliberately underpowered one-item bag unexpectedly won hoi_1_01.");
            Assert.IsTrue(retryButton.GetComponent<Button>().interactable);
            retryButton.onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>(),
                TimeoutSeconds,
                "Retry did not reopen hoi_1_01's before-dialogue.");
            Assert.AreEqual("hoi_1_01", GameBootstrap.Session.CurrentPveStageId);
        }

        [UnityTest]
        public IEnumerator ContinueIsDisabledWhenStaminaIsInsufficient()
        {
            yield return StartFreshHome(CreateStageOneLoadout, 10);
            yield return StartStageFromHome();
            GameBootstrap.Session.Profile.Pve.Stamina = 0;
            yield return FinishBattleAndOpenResult();
            ResultPopup result = FindPopup<ResultPopup>();
            UiButton continueButton = GetField<UiButton>(result, "playAgainButton");
            Assert.That(ButtonLabel(continueButton), Is.EqualTo("Ải tiếp theo"));
            Assert.IsFalse(continueButton.GetComponent<Button>().interactable);
            Text reason = GetField<Text>(result, "disabledReason");
            Assert.IsTrue(reason.gameObject.activeInHierarchy);
            Assert.That(reason.text, Is.EqualTo("Không đủ Thể Lực"));
        }

        [UnityTest]
        public IEnumerator HomePveHotspotRejectsEmptyBagWithoutOpeningMapOrBattle()
        {
            yield return StartFreshHome(_ => string.Empty, 1);
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GetField<HomeHotspot>(home, "pveHotspot").OnPointerClick(null);
            yield return WaitUntil(() =>
                PopupManager.IsOpen<MessagePopup>(),
                TimeoutSeconds,
                "The PvE hotspot did not explain why an empty bag cannot battle.");
            MessagePopup message = FindPopup<MessagePopup>();
            Assert.That(GetField<Text>(message,
                "messageText").text,
                Does.Contain("chưa có binh khí"));
            Assert.IsFalse(PopupManager.IsOpen<StageMapPopup>());
            Assert.AreEqual("HomeScene", SceneManager.GetActiveScene().name);
            Assert.IsNull(UnityEngine.Object.FindFirstObjectByType<GameplayController>());
        }

        private static IEnumerator StartFreshHome(Func<string, string> makeLoadout, int heroLevel)
        {
            PlayerPrefs.DeleteKey(HeroNameKey);
            PlayerPrefs.DeleteKey(SectIdKey);
            PlayerPrefs.Save();
            foreach (string file in ProfileFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            GameBootstrap.Session?.LoadProfile();
            yield return LoadScene("Assets/_Game/Scenes/LoginScene.unity");
            yield return WaitUntil(() =>
                GameBootstrap.Session != null,
                TimeoutSeconds,
                "Game services did not initialize after loading LoginScene.");
            LoginController login = UnityEngine.Object.FindFirstObjectByType<LoginController>();
            InputField name = GetField<InputField>(login, "heroName");
            Button enter = GetField<Button>(login, "enterButton");
            name.text = "Kiểm Thử PvE";
            Assert.IsTrue(enter.interactable);
            enter.onClick.Invoke();
            yield return WaitForReadyScene("HomeScene", "Login did not reach a ready HomeScene.");
            ApplyProfileForBattle(makeLoadout(GameBootstrap.Session.HeroName), heroLevel);
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            Assert.IsNotNull(home);
            home.RefreshBattleAvailability();
            CloseSectPromptIfPresent();
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

        private static IEnumerator StartStageFromHome()
        {
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Network.Disconnect();
            GetField<HomeHotspot>(home, "pveHotspot").OnPointerClick(null);
            yield return WaitUntil(() =>
                PopupManager.IsOpen<StageMapPopup>(),
                TimeoutSeconds,
                "Home PvE entry did not open the stage map.");
            CloseSectPromptIfPresent();
            StageMapPopup map = UnityEngine.Object.FindFirstObjectByType<StageMapPopup>();
            Assert.IsNotNull(map);
            GetField<UiButton>(map, "challengeButton").onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>(),
                TimeoutSeconds,
                "The stage before-dialogue did not open.");
            SkipTopDialogue();
            yield return WaitForReadyScene("GameplayScene",
                "The stage did not load a ready GameplayScene.");
            Assert.IsNotNull(UnityEngine.Object.FindFirstObjectByType<GameplayController>());
        }

        private static IEnumerator FinishBattleAndOpenResult()
        {
            GameplayController gameplay =
                UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            ReplayScreen replay = GetField<ReplayScreen>(gameplay, "replayScreen");
            yield return WaitUntil(() =>
                replay.gameObject.activeInHierarchy,
                TimeoutSeconds,
                "Starting the battle did not reveal the replay screen.");
            GetField<Button>(replay, "speed4Button").onClick.Invoke();
            Slider scrub = GetField<Slider>(replay, "scrub");
            scrub.value = scrub.maxValue;
            yield return WaitUntil(() =>
                PopupManager.IsOpen<DialoguePopup>() ||
                PopupManager.IsOpen<ResultPopup>(),
                TimeoutSeconds,
                "The battle replay did not reach its result flow.");
            if (PopupManager.IsOpen<DialoguePopup>())
            {
                SkipTopDialogue();
            }

            yield return WaitUntil(() =>
                PopupManager.IsOpen<ResultPopup>(),
                TimeoutSeconds,
                "The result popup did not appear after the post-battle dialogue.");
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

        private static string CreateLosingLoadout(string name)
        {
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            var grid = new BagGrid(5, 4);
            grid.Place(catalog.Get("thiet_cot_quyen_sao"), 0, 0);
            return LoadoutSerializer.ToJson(grid, name, 1);
        }

        private static void SkipTopDialogue()
        {
            DialoguePopup dialogue = FindPopup<DialoguePopup>();
            Assert.IsNotNull(dialogue, "No dialogue popup was available to skip.");
            GetField<UiButton>(dialogue, "skipButton").onClick.Invoke();
        }

        private static void CloseSectPromptIfPresent()
        {
            MessagePopup message = PopupManager.Top as MessagePopup;
            if (message == null)
                return;
            Text title = GetField<Text>(message, "titleText");
            if (!title.text.Contains("Môn Phái"))
                return;
            GetField<UiButton>(message, "closeButton").onClick.Invoke();
        }

        private static T FindPopup<T>()
            where T : Component => UnityEngine.Object.FindFirstObjectByType<T>();
        private static string ButtonLabel(UiButton button) =>
            button.GetComponentInChildren<Text>(true).text;
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

        private static T GetField<T>(object target, string name)
            where T : class
        {
            Assert.IsNotNull(target, "Cannot read field '" + name + "' from a null target.");
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
