using System.Collections;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneBagJianghu.Core
{
    public sealed class SceneFlow : MonoBehaviour
    {
        private static SceneFlow instance;
        [SerializeField]
        private LoadingPanel loadingPanel;
        private SceneTransition transition;
        private bool ready;
        public static bool IsTransitioning => instance != null
&& instance.transition != null
&& instance.transition.IsRunning;

        public void Initialize()
        {
            instance = this;
            transition = new SceneTransition();
        }

        public static void Go(GameScene target)
        {
            if (instance == null || instance.transition.IsRunning)
            {
                return;
            }

            instance.StartCoroutine(instance.TransitionTo(target));
        }

        public static void NotifyReady()
        {
            if (instance != null)
            {
                instance.ready = true;
            }
        }

        private IEnumerator TransitionTo(GameScene target)
        {
            string from = SceneManager.GetActiveScene().name;
            string to = GameScenePaths.GetName(target);
            Debug.Log($"[Scene] Change requested: {from} -> {to}");
            PopupManager.CloseAll();
            ready = false;
            transition.Begin(Time.unscaledTimeAsDouble);
            yield return loadingPanel.Show();
            transition.Update(Time.unscaledTimeAsDouble, 0f, false, true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(GameScenePaths.GetName(target));
            if (operation == null)
            {
                Debug.LogError("[Scene] Could not load scene: " + GameScenePaths.GetPath(target));
                yield return loadingPanel.Hide();
                transition.Cancel();
                yield break;
            }

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingPanel.SetProgress(progress);
                yield return null;
            }

            Debug.Log($"[Scene] Loaded: {from} -> {to}");
            loadingPanel.SetProgress(1f);
            transition.Update(Time.unscaledTimeAsDouble, 1f, ready, false);
            while (transition.State == SceneTransitionState.WaitingReady)
            {
                transition.Update(Time.unscaledTimeAsDouble, 1f, ready, false);
                yield return null;
            }

            yield return loadingPanel.Hide();
            transition.Update(Time.unscaledTimeAsDouble, 1f, ready, true);
        }
    }
}