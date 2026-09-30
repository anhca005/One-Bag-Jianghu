using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneBagJianghu.Core
{
    public static class GameBootstrap
    {
        private const string ResourcePath = "Core/GameController";

        public static GameSession Session => GameController.Instance == null ? null : GameController.Instance.Session;
        public static OneBagJianghu.Net.NetworkService Network => GameController.Instance == null ? null : GameController.Instance.Network;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            if (GameController.Instance != null) return;

            GameObject prefab = Resources.Load<GameObject>(ResourcePath);
            if (prefab == null)
            {
                Debug.LogError("[Game] GameController prefab is missing at Resources/Core/GameController. Run One Bag Jianghu/Build/GameController Prefab (Overwrite).");
                return;
            }

            GameObject root = Object.Instantiate(prefab);
            root.name = "GameController";
            if (SceneManager.GetActiveScene().name != GameScenePaths.GetName(GameScene.Login))
                Debug.Log("[Game] Bootstrap GameController from non-Login scene: " + SceneManager.GetActiveScene().name);
        }
    }
}
