using System;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Login;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class LoginHomeSceneTests
    {
        private const string LoginPath = "Assets/_Game/Scenes/LoginScene.unity";
        private const string HomePath = "Assets/_Game/Scenes/HomeScene.unity";
        private string previousScenePath;

        [SetUp]
        public void RememberScene()
        {
            previousScenePath = SceneManager.GetActiveScene().path;
        }

        [TearDown]
        public void RestoreScene()
        {
            if (!string.IsNullOrEmpty(previousScenePath))
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
            else
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void LoginSceneIsCompleteAndWired()
        {
            Scene scene = EditorSceneManager.OpenScene(LoginPath, OpenSceneMode.Single);
            AssertStandardScene(scene);
            LoginController controller = FindOne<LoginController>();
            AssertAssignedReferences(controller);
            Assert.That(Find("Canvas/Content/Btn_Enter"), Is.Not.Null);
            Assert.That(Find("Canvas/Content/HeroNameInput"), Is.Not.Null);
            Assert.That(Find("Canvas/Content/SectDropdown"), Is.Null);
            GameObject gameController = System.Array.Find(scene.GetRootGameObjects(), root => root.name == "GameController");
            Assert.That(gameController, Is.Not.Null);
            Assert.That(PrefabUtility.IsPartOfPrefabInstance(gameController), Is.True);
            Assert.That(PrefabUtility.GetCorrespondingObjectFromSource(gameController), Is.EqualTo(Resources.Load<GameObject>("Core/GameController")));
            Assert.That(gameController.transform.childCount, Is.EqualTo(5));
            AssertSpriteWhenArtExists("Assets/_Game/Textures2D/UI/bg_login.png", Find("Canvas/Background").GetComponent<Image>());
        }

        [Test]
        public void HomeSceneIsCompleteAndWired()
        {
            Scene scene = EditorSceneManager.OpenScene(HomePath, OpenSceneMode.Single);
            AssertStandardScene(scene);
            Assert.That(System.Array.Exists(scene.GetRootGameObjects(), root => root.GetComponent<GameController>() != null), Is.False);
            HomeController controller = FindOne<HomeController>();
            AssertAssignedReferences(controller);
            string[] hotspots = { "Hotspot_Pve", "Hotspot_Pvp", "Hotspot_Bag", "Hotspot_Storage", "Hotspot_Codex", "Hotspot_Shop", "Hotspot_Sect" };
            Assert.That(Find("Canvas/Background"), Is.Not.Null);
            Assert.That(Find("Canvas/HeroZone"), Is.Not.Null);
            Assert.That(Find("Canvas/Header/HeroName"), Is.Not.Null);
            Assert.That(Find("Canvas/Header/LevelText"), Is.Not.Null);
            Assert.That(Find("Canvas/Header/Btn_Settings"), Is.Not.Null);
            Assert.That(Find("Canvas/Header/StaminaText").activeSelf, Is.False);
            Assert.That(Find("Canvas/Header/SilverText").activeSelf, Is.False);
            Assert.That(Find("Canvas/BattleArea/WarningText"), Is.Not.Null);
            Assert.That(Find("Canvas/BattleArea/Btn_Pve"), Is.Not.Null);
            Assert.That(Find("Canvas/BattleArea/Btn_Pvp"), Is.Not.Null);
            foreach (string hotspot in hotspots)
            {
                GameObject value = Find("Canvas/Hotspots/" + hotspot);
                Assert.That(value, Is.Not.Null, hotspot);
                Assert.That(value.GetComponent<HomeHotspot>(), Is.Not.Null, hotspot);
            }
            Assert.That(UnityEngine.Object.FindObjectsByType<HomeHotspot>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(7));
            Assert.That(UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(3));
            AssertSpriteWhenArtExists("Assets/_Game/Textures2D/UI/bg_home.png", Find("Canvas/Background").GetComponent<Image>());
        }

        [TestCase(1, true)]
        [TestCase(9, true)]
        [TestCase(10, false)]
        [TestCase(20, false)]
        public void SectHotspotLockStateFollowsHeroLevel(int heroLevel, bool expectedLocked)
        {
            Scene scene = EditorSceneManager.OpenScene(HomePath, OpenSceneMode.Single);
            AssertStandardScene(scene);
            HomeHotspot sect = Find("Canvas/Hotspots/Hotspot_Sect").GetComponent<HomeHotspot>();
            var sessionObject = new GameObject("TestSession");
            var session = sessionObject.AddComponent<GameSession>();
            session.HeroLevel = heroLevel;

            sect.SetAvailableForLevel(session.HeroLevel);

            Assert.That(sect.IsLocked, Is.EqualTo(expectedLocked));
            Assert.That(sect.RequiredLevel, Is.EqualTo(10));
            UnityEngine.Object.DestroyImmediate(sessionObject);
        }

        private static void AssertStandardScene(Scene scene)
        {
            // Flat root (decision #26): no wrapper named after the scene; Main Camera, EventSystem, controller and Canvas sit at the root.
            Assert.That(System.Array.Exists(scene.GetRootGameObjects(), root => root.name == scene.name), Is.False);
            Assert.That(System.Array.Exists(scene.GetRootGameObjects(), root => root.name == "Main Camera"), Is.True);
            Assert.That(UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));
            GameObject canvasRoot = System.Array.Find(scene.GetRootGameObjects(), root => root.name == "Canvas");
            Assert.That(canvasRoot, Is.Not.Null);
            Canvas canvas = canvasRoot.GetComponent<Canvas>();
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            Assert.That(scaler, Is.Not.Null);
            Assert.That(scaler.uiScaleMode, Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize));
            Assert.That(scaler.referenceResolution, Is.EqualTo(new Vector2(1920f, 1080f)));
            Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(0.5f));
        }

        private static T FindOne<T>() where T : UnityEngine.Object
        {
            T[] values = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(values, Has.Length.EqualTo(1), typeof(T).Name);
            return values[0];
        }

        private static void AssertAssignedReferences(MonoBehaviour controller)
        {
            foreach (FieldInfo field in controller.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() != null && typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                    Assert.That(field.GetValue(controller), Is.Not.Null, controller.GetType().Name + "." + field.Name);
            }
        }

        private static GameObject Find(string path) => GameObject.Find(path);

        private static void AssertSpriteWhenArtExists(string path, Image image)
        {
            if (File.Exists(path)) Assert.That(image.sprite, Is.Not.Null, path);
        }
    }
}
