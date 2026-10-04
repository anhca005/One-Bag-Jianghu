using OneBagJianghu.Audio;
using OneBagJianghu.UI.Popups;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Audio
{
    public static class UiSfxPrefabBuilder
    {
        private const string PopupRoot = "Assets/_Game/Resources/UI/Popups";

        [InitializeOnLoadMethod]
        private static void ScheduleBuild()
        {
            EditorApplication.delayCall += AddToPopupPrefabs;
        }

        [MenuItem("One Bag Jianghu/Audio/Add UI SFX To Prefabs")]
        public static void AddToPopupPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PopupRoot });
            for (int i = 0; i < guids.Length; i++)
            {
                AddToPopup(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            AssetDatabase.SaveAssets();
        }

        private static void AddToPopup(string path)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (root.GetComponent<BasePopup>() == null || root.GetComponent<UiSfx>() != null)
                    return;
                root.AddComponent<UiSfx>().ConfigureAsPopup();
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }

    public sealed class UiSfxPopupPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i].StartsWith("Assets/_Game/Resources/UI/Popups/") &&
                    importedAssets[i].EndsWith(".prefab"))
                {
                    EditorApplication.delayCall += UiSfxPrefabBuilder.AddToPopupPrefabs;
                    return;
                }
            }
        }
    }
}
