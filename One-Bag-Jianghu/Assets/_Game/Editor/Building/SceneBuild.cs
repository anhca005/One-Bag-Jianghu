using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace OneBagJianghu.EditorTools.Building
{
    public static class SceneBuild
    {
        public static Scene CreateStandard(string sceneName, string assetPath, System.Type controllerType)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateMainCamera();
            CreateEventSystem();
            new GameObject(controllerType.Name, controllerType);
            Canvas canvas = UiBuild.Canvas(null, "Canvas", 0);
            UiBuild.Image(canvas.transform, "Background", UiBuild.Paper);
            UiBuild.Rect(canvas.transform, "Header");
            UiBuild.Rect(canvas.transform, "Content");
            UiBuild.Rect(canvas.transform, "Footer");
            foreach (RectTransform child in canvas.GetComponentsInChildren<RectTransform>())
                if (child != canvas.transform) UiBuild.Stretch(child);
            EditorSceneManager.SaveScene(scene, assetPath);
            return scene;
        }

        public static Camera CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = UiBuild.Paper;
            camera.depth = -1f;
            return camera;
        }

        public static EventSystem CreateEventSystem()
        {
            return new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule))
                .GetComponent<EventSystem>();
        }
    }
}
