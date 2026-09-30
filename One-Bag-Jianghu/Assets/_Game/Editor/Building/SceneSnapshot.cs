using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Building
{
    /// <summary>
    /// Renders the edit-time state of the main scenes to PNG (1920x1080) so layouts can be reviewed without
    /// opening the Editor. Batch: -executeMethod OneBagJianghu.EditorTools.Building.SceneSnapshot.CaptureAll
    /// (needs a graphics device: do not pass -nographics). Scenes are not saved.
    /// </summary>
    public static class SceneSnapshot
    {
        private const string OutputFolder = "Logs/Snapshots";
        private static readonly string[] Scenes =
        {
            "Assets/_Game/Scenes/LoginScene.unity",
            "Assets/_Game/Scenes/HomeScene.unity",
            "Assets/_Game/Scenes/GameplayScene.unity",
            "Assets/_Game/Scenes/VfxLab.unity"
        };

        [MenuItem("One Bag Jianghu/Tools/Capture Scene Snapshots")]
        public static void CaptureAll()
        {
            Directory.CreateDirectory(OutputFolder);
            foreach (string path in Scenes)
            {
                if (!File.Exists(path)) continue;
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                // The replay only switches on during a battle; show its authored layout for review.
                foreach (OneBagJianghu.Battle.Replay.View.ReplayScreen replay in Object.FindObjectsByType<OneBagJianghu.Battle.Replay.View.ReplayScreen>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    replay.gameObject.SetActive(true);
                Capture(Path.Combine(OutputFolder, Path.GetFileNameWithoutExtension(path) + ".png"));
            }

            GameObject gameController = Resources.Load<GameObject>("Core/GameController");
            if (gameController != null)
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GameObject instance = Object.Instantiate(gameController);
                foreach (Canvas canvas in instance.GetComponentsInChildren<Canvas>(true))
                {
                    canvas.gameObject.SetActive(canvas.sortingOrder >= 1000);
                }

                Capture(Path.Combine(OutputFolder, "LoadingPanel.png"));
            }

            foreach (string name in new[] { "StoragePopup", "BagPopup", "ShopPopup", "DevCheatPanel", "ResultPopup" })
            {
                GameObject prefab = Resources.Load<GameObject>("UI/Popups/" + name);
                if (prefab == null) continue;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var holder = new GameObject("Holder", typeof(Canvas));
                holder.SetActive(false);
                GameObject popup = Object.Instantiate(prefab, holder.transform);
                // Popup scripts need the runtime session; strip them so only the authored layout renders.
                foreach (MonoBehaviour behaviour in popup.GetComponentsInChildren<MonoBehaviour>(true))
                    if (behaviour is OneBagJianghu.UI.Popups.BasePopup) Object.DestroyImmediate(behaviour);
                popup.SetActive(true);
                holder.SetActive(true);
                Capture(Path.Combine(OutputFolder, name + ".png"));
            }

            Debug.Log("Scene snapshots written to " + Path.GetFullPath(OutputFolder));
        }

        private static void Capture(string file)
        {
            const int width = 1920;
            const int height = 1080;
            var cameraObject = new GameObject("Snapshot Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = target;

            foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (!canvas.isRootCanvas) continue;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 10f + canvas.sortingOrder * 0.001f;
            }

            Canvas.ForceUpdateCanvases();
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            RenderTexture.active = previous;
            File.WriteAllBytes(file, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            camera.targetTexture = null;
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(cameraObject);
        }
    }
}
