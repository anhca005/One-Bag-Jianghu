using System.Collections;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
namespace OneBagJianghu.Tests.Play
{
    public sealed class LevelSectFlowTests
    {
        [UnityTest]
        public IEnumerator CheatLevelTen_SectHotspot_SelectsSect_ForNextBattleLoadout()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                "Assets/_Game/Scenes/HomeScene.unity",
                new LoadSceneParameters(LoadSceneMode.Single));
#else
            yield return SceneManager.LoadSceneAsync("HomeScene");
#endif
            yield return null;
            yield return null;
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.HeroLevel = 9;
            profile.HeroExp = 0;
            profile.SectId = null;
            profile.TamPhapLevel = 1;
            profile.SectPromptShown = true;
            LevelService levels = new LevelService(LevelConfig.Load());
            levels.Grant(profile, levels.ExpToNext(9));
            HomeHotspot hotspot = null;
            foreach (HomeHotspot value in Object.FindObjectsByType<HomeHotspot>(FindObjectsSortMode.None))
            {
                if (value.name == "Hotspot_Sect")
                {
                    hotspot = value;
                }
            }

            Assert.That(hotspot, Is.Not.Null);
            hotspot.OnClick.Invoke();
            yield return null;
            SectSelectPopup popup = Object.FindFirstObjectByType<SectSelectPopup>();
            Assert.That(popup, Is.Not.Null);
            Assert.That(popup.IsShowing, Is.True);
            popup.Confirm();
            yield return null;
            Assert.That(profile.SectId, Is.Not.Empty);
            Assert.That(profile.TamPhapLevel, Is.EqualTo(1));
            JObject nextBattle =
                JObject.Parse(GameSession.BuildEngineLoadoutJson(profile,
                ItemCatalog.LoadFromResources()));
            Assert.That(nextBattle.Value<string>("sect"), Is.EqualTo(profile.SectId));
        }
    }
}
