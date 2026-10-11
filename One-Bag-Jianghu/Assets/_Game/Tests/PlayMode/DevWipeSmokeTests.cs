using System.Collections;
using System.IO;
using NUnit.Framework;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Login;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Play
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public sealed class DevWipeSmokeTests : SmokeTestBase
    {
        private static readonly string[] SaveFiles =
        {
            Path.Combine(Application.persistentDataPath, "profile.json"),
            Path.Combine(Application.persistentDataPath, "profile.json.bak"),
            Path.Combine(Application.persistentDataPath, "profile.json.tmp"),
            Path.Combine(Application.persistentDataPath, "loadout.json")
        };

        [TearDown]
        public void ResetShopClock() => ShopRuntimeClock.Reset();

        [UnityTest]
        public IEnumerator WipeDeletesProgressReturnsToLoginAndNextLoginIsFresh()
        {
            DeleteProfile();
            yield return Login("Progressed Hero");
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.HeroLevel = 5;
            profile.SectId = "thiet_phu";
            profile.AddCurrency("bac", 50000);
            profile.FirstClears.Add("stage_1");
            profile.Pve.Stages["stage_1"] = new PveStageProgress { Cleared = true };
            GameBootstrap.Session.CurrentPveStageId = "stage_1";
            GameBootstrap.Session.SaveProfile();
            File.WriteAllText(SaveFiles[3], "dev loadout");
            PlayerPrefs.SetString(ProfileStore.HeroNameKey, profile.HeroName);
            PlayerPrefs.SetString(ProfileStore.SectIdKey, profile.SectId);
            PlayerPrefs.SetString("OneBagJianghu.ServerUrl", "ws://wipe-test");
            PlayerPrefs.Save();
            ShopRuntimeClock.AdvanceDays(7);

            PopupManager.Show<DevCheatPanel>();
            yield return WaitUntil(() => PopupManager.IsOpen<DevCheatPanel>(), "Dev cheat panel did not open.");
            GetField<UiButton>(PopupManager.Top, "wipeAllButton").onClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ConfirmPopup>(), "Wipe confirmation did not open.");
            GetField<UiButton>(PopupManager.Top, "confirmButton").onClick.Invoke();
            yield return WaitUntil(() => SceneManager.GetActiveScene().name == "LoginScene" &&
                !SceneFlow.IsTransitioning && !LoadingPanel.IsVisible, "Wipe did not return to LoginScene.");

            foreach (string path in SaveFiles) Assert.IsFalse(File.Exists(path), path + " survived wipe.");
            Assert.IsFalse(PlayerPrefs.HasKey(ProfileStore.HeroNameKey));
            Assert.IsFalse(PlayerPrefs.HasKey(ProfileStore.SectIdKey));
            Assert.AreEqual("ws://wipe-test", PlayerPrefs.GetString("OneBagJianghu.ServerUrl"));
            Assert.IsNull(GameBootstrap.Session.CurrentPveStageId);
            Assert.Less(
                System.Math.Abs((new ShopRuntimeClock().UtcNow - System.DateTimeOffset.UtcNow).TotalSeconds),
                2d);

            LoginController login = Object.FindFirstObjectByType<LoginController>();
            GetField<InputField>(login, "heroName").text = "Fresh Hero";
            GetField<Button>(login, "enterButton").onClick.Invoke();
            yield return WaitUntil(() => SceneManager.GetActiveScene().name == "HomeScene" &&
                !SceneFlow.IsTransitioning && !LoadingPanel.IsVisible, "Fresh login did not reach HomeScene.");

            PlayerProfile fresh = GameBootstrap.Session.Profile;
            Assert.AreEqual(1, fresh.HeroLevel);
            Assert.AreEqual(2, fresh.Items.Count);
            Assert.AreEqual(0, fresh.FirstClears.Count);
            Assert.AreEqual(0, fresh.Pve.Stages.Count);
            Assert.IsNull(fresh.SectId);
            Assert.AreEqual(500, fresh.Currencies["bac"]); // starter.json (decision #48)
            Assert.AreEqual(0, fresh.Currencies["nguyen_bao"]);
            Assert.AreEqual(0, fresh.Currencies["danh_vong"]);
            Assert.AreEqual(OneBagJianghu.Pve.StaminaConfig.Load().Max, fresh.Pve.Stamina);
        }

        [UnityTest]
        public IEnumerator CancelWipeChangesNothing()
        {
            DeleteProfile();
            yield return Login("Cancel Hero");
            GameBootstrap.Session.Profile.HeroLevel = 5;
            GameBootstrap.Session.Profile.AddCurrency("bac", 1234);
            GameBootstrap.Session.SaveProfile();
            string before = File.ReadAllText(SaveFiles[0]);

            PopupManager.Show<DevCheatPanel>();
            GetField<UiButton>(PopupManager.Top, "wipeAllButton").onClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ConfirmPopup>(), "Wipe confirmation did not open.");
            GetField<UiButton>(PopupManager.Top, "cancelButton").onClick.Invoke();
            yield return WaitUntil(() => !PopupManager.IsOpen<ConfirmPopup>(), "Wipe confirmation did not close.");

            Assert.AreEqual("HomeScene", SceneManager.GetActiveScene().name);
            Assert.AreEqual(5, GameBootstrap.Session.Profile.HeroLevel);
            Assert.AreEqual(before, File.ReadAllText(SaveFiles[0]));
        }
    }
#endif
}
