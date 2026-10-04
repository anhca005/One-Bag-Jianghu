using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    /// <summary>
    /// Puts the title logo into the open LoginScene and the loading panel of the GameController prefab.
    /// Both are hand-edited, so this only adds or updates a "TitleLogo" image next to the existing "Title" text and
    /// hides that text; nothing else changes. Running it again just updates the logo.
    /// </summary>
    public static class LoginTitleLogo
    {
        public const string LogoKey = "logo_title";
        public const string LogoName = "TitleLogo";
        private const string LogoPath = "Assets/_Game/Textures2D/UI/" + LogoKey + ".png";

        // Login: centered in the upper part of the screen. The image keeps its 3:2 aspect inside the area.
        private static readonly Vector2 LoginMin = new Vector2(.30f, .60f);
        private static readonly Vector2 LoginMax = new Vector2(.70f, .97f);

        // Loading: above the progress bar (centered at y .47).
        public static readonly Vector2 LoadingMin = new Vector2(.30f, .56f);
        public static readonly Vector2 LoadingMax = new Vector2(.70f, .93f);

        [MenuItem("One Bag Jianghu/UI/Put Title Logo In Login And Loading")]
        public static void Apply()
        {
            Sprite logo = AssetDatabase.LoadAssetAtPath<Sprite>(LogoPath);
            if (logo == null)
            {
                EditorUtility.DisplayDialog("Title logo", "Sprite not found: " + LogoPath, "OK");
                return;
            }

            bool login = ApplyToOpenLoginScene(logo);
            bool loading = ApplyToLoadingPanel(logo);
            string report = "Login scene: " + (login ? "done, save the scene (Ctrl+S)" : "open LoginScene first")
                + "\nLoading panel: " + (loading ? "done (prefab saved)" : "Title not found in GameController");
            EditorUtility.DisplayDialog("Title logo", report, "OK");
        }

        private static bool ApplyToOpenLoginScene(Sprite logo)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject title = FindInScene(scene, "Title");
            if (title == null)
                return false;

            Image image = FindOrCreateLogo(title.transform.parent, true);
            Place(image, logo, LoginMin, LoginMax, title.transform.GetSiblingIndex());
            Undo.RecordObject(title, "Hide text title");
            title.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            return true;
        }

        private static bool ApplyToLoadingPanel(Sprite logo)
        {
            string path = GameControllerPrefabBuilder.Path;
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Transform title = FindChild(contents.transform, "LoadingPanel/Background/Title");
                if (title == null)
                    return false;

                Image image = FindOrCreateLogo(title.parent, false);
                Place(image, logo, LoadingMin, LoadingMax, title.GetSiblingIndex());
                title.gameObject.SetActive(false);
                PrefabUtility.SaveAsPrefabAsset(contents, path);
                return true;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }

        /// <summary>Sets sprite and layout of a logo image; also used by the GameController builder.</summary>
        public static void Place(Image image, Sprite logo, Vector2 anchorMin, Vector2 anchorMax, int siblingIndex)
        {
            image.sprite = logo;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = Color.white;

            RectTransform rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            rect.SetSiblingIndex(siblingIndex);
        }

        private static Image FindOrCreateLogo(Transform parent, bool withUndo)
        {
            Transform existing = parent.Find(LogoName);
            if (existing != null && existing.TryGetComponent(out Image found))
            {
                if (withUndo)
                {
                    Undo.RecordObject(found, "Update title logo");
                    Undo.RecordObject(found.rectTransform, "Update title logo");
                }

                return found;
            }

            var logoObject = new GameObject(LogoName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (withUndo)
                Undo.RegisterCreatedObjectUndo(logoObject, "Add title logo");
            logoObject.transform.SetParent(parent, false);
            logoObject.layer = parent.gameObject.layer;
            return logoObject.GetComponent<Image>();
        }

        private static Transform FindChild(Transform root, string path)
        {
            Transform direct = root.Find(path);
            if (direct != null)
                return direct;
            string leaf = path.Substring(path.LastIndexOf('/') + 1);
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == leaf && child.parent != null && child.parent.name == "Background")
                    return child;
            }

            return null;
        }

        private static GameObject FindInScene(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                {
                    // The GameController instance in the scene has its own loading "Title"; skip it.
                    if (child.name == name && child.GetComponentInParent<Canvas>(true)?.name != "LoadingPanel")
                        return child.gameObject;
                }
            }

            return null;
        }
    }
}
