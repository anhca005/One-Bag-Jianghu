using OneBagJianghu.Core;
using OneBagJianghu.UI.Gameplay;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Login;
using OneBagJianghu.EditorTools.Building;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OneBagJianghu.EditorTools
{
    public static class GameSceneBuilder
    {
        private const string Folder = "Assets/_Game/Scenes";

        [MenuItem("One Bag Jianghu/Create Game Scenes")]
        public static void CreateGameScenes()
        {
            PopupPrefabBuilder.BuildPopupPrefabs();
            GameplaySceneBuilder.Build(false);
            if (!AssetDatabase.IsValidFolder(Folder))
            {
                AssetDatabase.CreateFolder("Assets/_Game", "Scenes");
            }

            Create<LoginController>(GameScene.Login);
            Create<HomeController>(GameScene.Home);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Login), true),
                new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Home), true),
                new EditorBuildSettingsScene(GameScenePaths.GetPath(GameScene.Gameplay), true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("Game scenes created and Build Settings updated.");
        }

        private static void Create<T>(GameScene gameScene) where T : Component
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject(GameScenePaths.GetName(gameScene), typeof(T));
            EditorSceneManager.SaveScene(scene, GameScenePaths.GetPath(gameScene));
        }
    }
}
