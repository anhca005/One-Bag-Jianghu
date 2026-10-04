using System.Collections;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneBagJianghu.Core
{
    public sealed class SceneFlow : MonoBehaviour
    {
        // Scenes load in a fraction of a second; keep the loading screen up long enough to read the tip.
        public const float MinimumLoadingSeconds = 1.8f;
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
            // Mark the transition as running before closing popups, so their close handlers can tell the scene is
            // being left (the stage map must not switch back to Home music on its way into a battle).
            transition.Begin(Time.unscaledTimeAsDouble);
            PopupManager.CloseAll();
            ready = false;
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

            float shownAt = Time.unscaledTime;
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingPanel.SetProgress(DisplayedProgress(progress, Time.unscaledTime - shownAt));
                yield return null;
            }

            Debug.Log($"[Scene] Loaded: {from} -> {to}");
            while (DisplayedProgress(1f, Time.unscaledTime - shownAt) < 1f)
            {
                loadingPanel.SetProgress(DisplayedProgress(1f, Time.unscaledTime - shownAt));
                yield return null;
            }

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

        /// <summary>
        /// The bar never runs ahead of the real load, and never fills faster than MinimumLoadingSeconds.
        /// The timed part eases out, quick at first and slowing near the end, like a real load.
        /// </summary>
        public static float DisplayedProgress(float realProgress, float elapsedSeconds)
        {
            float t = Mathf.Clamp01(elapsedSeconds / MinimumLoadingSeconds);
            float timed = 1f - (1f - t) * (1f - t);
            return Mathf.Min(Mathf.Clamp01(realProgress), timed);
        }
    }
}