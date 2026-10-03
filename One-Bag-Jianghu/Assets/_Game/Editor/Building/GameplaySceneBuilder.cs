using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.UI.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class GameplaySceneBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/GameplayScene.unity";
        [MenuItem("One Bag Jianghu/Build/Gameplay Scene")] public static void BuildGameplayScene() => Build(false);
        [MenuItem("One Bag Jianghu/Build/Gameplay Scene (Overwrite)")] public static void BuildGameplaySceneOverwrite() => Build(true);
        public static void BuildMissing() => Build(false); public static void BuildOverwrite() => Build(true);
        public static void Build(bool overwrite)
        {
            ReplayPrefabBuilder.Build(overwrite);
            if (!AssetGuard.ShouldBuild(ScenePath, overwrite)) return;
            Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            SceneBuild.CreateMainCamera();SceneBuild.CreateEventSystem();
            GameplayController controller=new GameObject("GameplayController",typeof(GameplayController)).GetComponent<GameplayController>();
            Canvas canvas=UiBuild.Canvas(null,"Canvas",0); Image bg=UiBuild.Image(canvas.transform,"Background",UiBuild.Paper);UiBuild.Stretch(bg.rectTransform);
            if (UiBuild.ApplyUiSprite(bg,"bg_battle")) { AspectRatioFitter fit=bg.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.EnvelopeParent;fit.aspectRatio=1.5f; }
            ReplayScreen replay=UiBuild.InstantiateLinked<ReplayScreen>(ReplayPrefabBuilder.Folder+"/ReplayScreen.prefab",canvas.transform); replay.name="Replay";UiBuild.Stretch((RectTransform)replay.transform);replay.gameObject.SetActive(false);
            UiBuild.Set(controller,"replayRoot",replay.gameObject);UiBuild.Set(controller,"replayScreen",replay);UiBuild.Set(controller,"battleBackground",bg);
            EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();
        }
    }
}
