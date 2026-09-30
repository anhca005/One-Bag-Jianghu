using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Battle.Replay.View.VfxLab;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Gameplay;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Login;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Scenes
{
    public sealed class DemoSceneTests
    {
        private string previousScene;
        [SetUp] public void RememberScene() => previousScene = SceneManager.GetActiveScene().path;
        [TearDown] public void RestoreScene()
        {
            if (!string.IsNullOrEmpty(previousScene)) EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Single);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [TestCase("Assets/_Game/Scenes/BagDemo.unity", typeof(BagDemoBootstrap))]
        [TestCase("Assets/_Game/Scenes/ReplayDemo.unity", typeof(ReplayDemoBootstrap))]
        [TestCase("Assets/_Game/Scenes/VfxLab.unity", typeof(VfxLabBootstrap))]
        public void DemoSceneIsCompleteAndWired(string path, System.Type bootstrapType)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(canvases, Has.Length.EqualTo(1));
            CanvasScaler scaler = canvases[0].GetComponent<CanvasScaler>();
            Assert.That(scaler.uiScaleMode, Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize));
            Assert.That(scaler.referenceResolution, Is.EqualTo(new Vector2(1920f, 1080f)));
            Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(.5f));
            MonoBehaviour bootstrap = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                bootstrap = root.GetComponentInChildren(bootstrapType, true) as MonoBehaviour;
                if (bootstrap != null) break;
            }
            Assert.That(bootstrap, Is.Not.Null);
            foreach (FieldInfo field in bootstrapType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() == null) continue;
                object value = field.GetValue(bootstrap);
                if (typeof(Object).IsAssignableFrom(field.FieldType))
                    Assert.That(value, Is.Not.Null, bootstrapType.Name + "." + field.Name);
                else if (field.FieldType.IsArray && typeof(Object).IsAssignableFrom(field.FieldType.GetElementType()))
                {
                    var array = (System.Array)value;
                    Assert.That(array, Is.Not.Null.And.Length.GreaterThan(0), bootstrapType.Name + "." + field.Name);
                    foreach (object element in array) Assert.That(element, Is.Not.Null, bootstrapType.Name + "." + field.Name);
                }
            }
        }

        [TestCase("Assets/_Game/Scenes/LoginScene.unity", typeof(LoginController))]
        [TestCase("Assets/_Game/Scenes/HomeScene.unity", typeof(HomeController))]
        [TestCase("Assets/_Game/Scenes/GameplayScene.unity", typeof(GameplayController))]
        [TestCase("Assets/_Game/Scenes/BagDemo.unity", typeof(BagDemoBootstrap))]
        [TestCase("Assets/_Game/Scenes/ReplayDemo.unity", typeof(ReplayDemoBootstrap))]
        [TestCase("Assets/_Game/Scenes/VfxLab.unity", typeof(VfxLabBootstrap))]
        public void SceneHasFlatStandardRoots(string path, System.Type controllerType)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            GameObject[] roots = scene.GetRootGameObjects();
            bool isLogin = scene.name == "LoginScene";
            Assert.That(System.Array.FindAll(roots, root => root.GetComponent<GameController>() != null), Has.Length.EqualTo(isLogin ? 1 : 0));
            Assert.That(System.Array.Find(roots, root => root.name == scene.name), Is.Null);
            Assert.That(System.Array.Find(roots, root => root.name == scene.name + "Scene"), Is.Null);

            GameObject cameraRoot = SingleRoot(roots, "Main Camera");
            Assert.That(cameraRoot.CompareTag("MainCamera"), Is.True);
            Camera camera = cameraRoot.GetComponent<Camera>();
            Assert.That(camera, Is.Not.Null);
            Assert.That(camera.orthographic, Is.True);
            Assert.That(camera.clearFlags, Is.EqualTo(CameraClearFlags.SolidColor));
            Assert.That(camera.backgroundColor, Is.EqualTo(UiKit.Paper));
            Assert.That(camera.depth, Is.EqualTo(-1f));
            Assert.That(cameraRoot.GetComponents<AudioListener>(), Has.Length.EqualTo(1));

            GameObject eventSystemRoot = SingleRoot(roots, "EventSystem");
            Assert.That(eventSystemRoot.GetComponent<EventSystem>(), Is.Not.Null);
            Assert.That(eventSystemRoot.GetComponent("InputSystemUIInputModule"), Is.Not.Null);
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));

            GameObject controllerRoot = System.Array.Find(roots, root => root.GetComponent(controllerType) != null);
            Assert.That(controllerRoot, Is.Not.Null, controllerType.Name + " must be a scene root.");
            Canvas canvas = SingleRoot(roots, "Canvas").GetComponent<Canvas>();
            Assert.That(canvas, Is.Not.Null);
            Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
        }

        [Test]
        public void DemoScenesUseLinkedTaskCPrefabs()
        {
            AssertLinked("Assets/_Game/Scenes/BagDemo.unity", "Assets/_Game/Prefabs/UI/Bag/BagLoadoutPanel.prefab");
            AssertLinked("Assets/_Game/Scenes/ReplayDemo.unity", "Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab");
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/VfxLab.unity", OpenSceneMode.Single);
            FighterPanelView[] fighters = Object.FindObjectsByType<FighterPanelView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(fighters, Has.Length.EqualTo(2));
            foreach (FighterPanelView fighter in fighters) Assert.That(PrefabUtility.IsPartOfPrefabInstance(fighter), Is.True);
        }

        private static void AssertLinked(string scenePath, string prefabPath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            bool found = false;
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                    if (PrefabUtility.GetCorrespondingObjectFromSource(transform.gameObject) == source) found = true;
            Assert.That(found, Is.True, prefabPath);
        }

        private static GameObject SingleRoot(GameObject[] roots, string name)
        {
            GameObject[] matches = System.Array.FindAll(roots, root => root.name == name);
            Assert.That(matches, Has.Length.EqualTo(1), "Expected exactly one root named " + name + ".");
            return matches[0];
        }
    }
}
