using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Building
{
    public static class AssetGuard
    {
        public static bool ShouldBuild(string assetPath, bool overwrite)
        {
            if (AssetDatabase.LoadMainAssetAtPath(assetPath) == null) return true;
            if (!overwrite)
            {
                Debug.Log(assetPath + " exists, skipped");
                return false;
            }

            return Application.isBatchMode || EditorUtility.DisplayDialog(
                "Overwrite asset?", "Replace the hand-edited asset?\n" + assetPath, "Overwrite", "Cancel");
        }
    }
}
