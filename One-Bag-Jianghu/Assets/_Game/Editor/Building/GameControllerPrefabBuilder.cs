using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class GameControllerPrefabBuilder
    {
        public const string Path = "Assets/_Game/Resources/Core/GameController.prefab";

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            if (!AssetGuard.ShouldBuild(Path, overwrite)) return;
            UiBuild.EnsureFolder("Assets/_Game/Resources/Core");
            var root = new GameObject("GameController");
            GameController controller = root.AddComponent<GameController>();
            GameSession session = Child<GameSession>(root.transform, "GameSession");
            SceneFlow flow = Child<SceneFlow>(root.transform, "SceneFlow");
            NetworkService network = Child<NetworkService>(root.transform, "NetworkService");

            Canvas popupCanvas = UiBuild.Canvas(root.transform, "PopupManager", PopupStack.BaseSortingOrder);
            PopupManager popups = popupCanvas.gameObject.AddComponent<PopupManager>();
            Image backdropImage = UiBuild.Image(popupCanvas.transform, "Backdrop", new Color(0f, 0f, 0f, 0.55f));
            UiBuild.Stretch(backdropImage.rectTransform);
            Canvas backdropCanvas = backdropImage.gameObject.AddComponent<Canvas>();
            backdropCanvas.overrideSorting = true;
            backdropImage.gameObject.AddComponent<GraphicRaycaster>();
            Button backdropButton = backdropImage.gameObject.AddComponent<Button>();
            backdropButton.targetGraphic = backdropImage;
            backdropButton.transition = Selectable.Transition.None;
            UiBuild.Set(popups, "canvas", popupCanvas);
            UiBuild.Set(popups, "backdropCanvas", backdropCanvas);
            UiBuild.Set(popups, "backdrop", backdropImage.gameObject);
            UiBuild.Set(popups, "backdropButton", backdropButton);

            Canvas loadingCanvas = UiBuild.Canvas(root.transform, "LoadingPanel", 1000);
            LoadingPanel loading = loadingCanvas.gameObject.AddComponent<LoadingPanel>();
            CanvasGroup group = loadingCanvas.gameObject.AddComponent<CanvasGroup>();
            Image background = UiBuild.Image(loadingCanvas.transform, "Background", UiBuild.Paper);
            UiBuild.Stretch(background.rectTransform);
            if (UiBuild.ApplyUiSprite(background, "bg_loading"))
            {
                AspectRatioFitter fitter = background.gameObject.AddComponent<AspectRatioFitter>();
                fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                fitter.aspectRatio = 1.5f;
            }
            Text title = UiBuild.Text(background.transform, "Title", "Nhất Nang Giang Hồ", 60, TextAnchor.MiddleCenter, UiFontRole.Display);
            UiBuild.Anchors(title.rectTransform, new Vector2(0.16f, 0.61f), new Vector2(0.84f, 0.77f));
            Slider progress = UiBuild.InstantiateLinked<Slider>(CommonUiPrefabBuilder.Folder + "/Slider.prefab", background.transform);
            progress.name = "Progress Bar";
            progress.interactable = false;
            UiBuild.ApplyUiSprite(progress.GetComponent<Image>(), "progress_bar", Image.Type.Sliced);
            UiBuild.Anchors(progress.GetComponent<RectTransform>(), new Vector2(0.25f, 0.45f), new Vector2(0.75f, 0.49f));
            Text percentage = UiBuild.Text(background.transform, "Percentage", "0%", 24);
            UiBuild.Anchors(percentage.rectTransform, new Vector2(0.4f, 0.37f), new Vector2(0.6f, 0.43f));
            Text tip = UiBuild.Text(background.transform, "Tip", "Mỗi ô trong bao phục đều đáng giá.", 22);
            UiBuild.Anchors(tip.rectTransform, new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.3f));
            UiBuild.Set(loading, "canvas", loadingCanvas);
            UiBuild.Set(loading, "group", group);
            UiBuild.Set(loading, "panelRoot", loadingCanvas.gameObject);
            UiBuild.Set(loading, "progress", progress);
            UiBuild.Set(loading, "percentage", percentage);
            UiBuild.Set(loading, "tip", tip);
            UiBuild.Set(flow, "loadingPanel", loading);
            UiBuild.Set(controller, "session", session);
            UiBuild.Set(controller, "flow", flow);
            UiBuild.Set(controller, "popups", popups);
            UiBuild.Set(controller, "loading", loading);
            UiBuild.Set(controller, "network", network);

            PrefabUtility.SaveAsPrefabAsset(root, Path);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        private static T Child<T>(Transform parent, string name) where T : Component
        {
            var child = new GameObject(name, typeof(T));
            child.transform.SetParent(parent, false);
            return child.GetComponent<T>();
        }
    }
}
