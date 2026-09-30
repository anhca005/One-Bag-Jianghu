using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Building
{
    public static class BuildMenu
    {
        // Order matters: prefabs before the popups/scenes that nest them.
        private static readonly IReadOnlyList<Action<bool>> Builders = new Action<bool>[]
        {
            CommonUiPrefabBuilder.Build,
            GameControllerPrefabBuilder.Build,
            BagPrefabBuilder.Build,
            ReplayPrefabBuilder.Build,
            PopupPrefabBuilder.Build,
            EconomyPopupPrefabBuilder.Build,
            LoginHomeSceneBuilder.Build,
            GameplaySceneBuilder.Build,
            DemoSceneBuilder.Build
        };

        [MenuItem("One Bag Jianghu/Build/Common UI Prefabs")]
        public static void BuildCommonUiPrefabs() => CommonUiPrefabBuilder.Build(false);

        [MenuItem("One Bag Jianghu/Build/Common UI Prefabs (Overwrite)")]
        public static void BuildCommonUiPrefabsOverwrite() => CommonUiPrefabBuilder.Build(true);

        [MenuItem("One Bag Jianghu/Build/GameController Prefab")]
        public static void BuildGameControllerPrefab() => GameControllerPrefabBuilder.Build(false);

        [MenuItem("One Bag Jianghu/Build/GameController Prefab (Overwrite)")]
        public static void BuildGameControllerPrefabOverwrite() => GameControllerPrefabBuilder.Build(true);

        [MenuItem("One Bag Jianghu/Build/Popup Prefabs")]
        public static void BuildPopupPrefabs() => PopupPrefabBuilder.Build(false);

        [MenuItem("One Bag Jianghu/Build/Popup Prefabs (Overwrite)")]
        public static void BuildPopupPrefabsOverwrite() => PopupPrefabBuilder.Build(true);

        [MenuItem("One Bag Jianghu/Build/Economy Popup Prefabs")]
        public static void BuildEconomyPopupPrefabs() => EconomyPopupPrefabBuilder.Build(false);

        [MenuItem("One Bag Jianghu/Build/Economy Popup Prefabs (Overwrite)")]
        public static void BuildEconomyPopupPrefabsOverwrite() => EconomyPopupPrefabBuilder.Build(true);

        [MenuItem("One Bag Jianghu/Build/Build All Missing")]
        public static void BuildAllMissing() => BuildAll(false);

        [MenuItem("One Bag Jianghu/Build/Build All Missing (Overwrite)")]
        public static void BuildAllOverwrite() => BuildAll(true);

        private static void BuildAll(bool overwrite)
        {
            foreach (Action<bool> builder in Builders) builder(overwrite);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("One Bag Jianghu UI build completed.");
        }
    }
}
