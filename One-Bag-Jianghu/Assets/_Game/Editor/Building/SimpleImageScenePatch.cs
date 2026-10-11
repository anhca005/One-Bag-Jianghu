using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class SimpleImageScenePatch
    {
        private const string UiFolder = "Assets/_Game/Textures2D/UI/";

        private static readonly Dictionary<string, string> SpriteByObject = new Dictionary<string, string>
        {
            ["Canvas/Content/Btn_Enter"] = "button_primary",
            ["Canvas/Content/HeroNameInput"] = "Kit/input_field_wide",
            ["Canvas/Header"] = "Kit/home_banner_simple",
            ["Canvas/Stamina Tooltip/Paper"] = "Kit/tooltip_panel",
            ["Canvas/Hotspots/Hotspot_Pve/Plaque"] = "Kit/plaque_label_medium",
            ["Canvas/Hotspots/Hotspot_Pvp/Plaque"] = "Kit/plaque_label_medium",
            ["Canvas/Hotspots/Hotspot_Sect/Plaque"] = "Kit/plaque_label_medium",
            ["Canvas/Hotspots/Hotspot_Bag/Plaque"] = "plaque_label",
            ["Canvas/Hotspots/Hotspot_Storage/Plaque"] = "Kit/plaque_label_tall",
            ["Canvas/Hotspots/Hotspot_Codex/Plaque"] = "plaque_label",
            ["Canvas/Hotspots/Hotspot_Encyclopedia/Plaque"] = "plaque_label",
            ["Canvas/Hotspots/Hotspot_Shop/Plaque"] = "plaque_label"
        };

        // Only Btn_Enter has its own size; the plaques stretch to their hand-placed hotspots, so each gets the art
        // whose aspect matches that hotspot instead of a new size.
        private static readonly Dictionary<string, Vector2> SizeByObject = new Dictionary<string, Vector2>
        {
            ["Canvas/Content/Btn_Enter"] = new Vector2(396f, 76f)
        };

        private const string HotspotPrefabPath = "Assets/_Game/Prefabs/UI/Home/Hotspot.prefab";

        private static readonly string[] ScenePaths =
        {
            "Assets/_Game/Scenes/LoginScene.unity",
            "Assets/_Game/Scenes/HomeScene.unity",
            "Assets/_Game/Scenes/GameplayScene.unity",
            "Assets/_Game/Scenes/ReplayDemo.unity",
            "Assets/_Game/Scenes/BagDemo.unity",
            "Assets/_Game/Scenes/VfxLab.unity"
        };

        [MenuItem("One Bag Jianghu/Patch Scenes To Simple Images")]
        public static void RunBatch()
        {
            PatchHotspotPrefab();
            foreach (string scenePath in ScenePaths)
            {
                PatchScene(scenePath);
            }
        }

        private static void PatchScene(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool changed = false;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Image image in root.GetComponentsInChildren<Image>(true))
                {
                    string objectPath = GetPath(image.transform);
                    changed |= PatchNamedImage(scenePath, objectPath, image);
                    changed |= PatchRemainingSimpleImage(scenePath, objectPath, image);
                }
            }

            if (changed)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static bool PatchNamedImage(string scenePath, string objectPath, Image image)
        {
            if (!SpriteByObject.TryGetValue(objectPath, out string spriteKey))
            {
                return false;
            }

            bool changed = SetSprite(scenePath, objectPath, image, spriteKey);
            // Plaques keep their aspect, so a hotspot a few percent off the art shrinks it instead of stretching.
            if (objectPath.EndsWith("/Plaque") && !image.preserveAspect)
            {
                image.preserveAspect = true;
                Log(scenePath, objectPath, "preserveAspect off", "on");
                changed = true;
            }

            if (SizeByObject.TryGetValue(objectPath, out Vector2 size) && image.rectTransform.sizeDelta != size)
            {
                Vector2 oldSize = image.rectTransform.sizeDelta;
                image.rectTransform.sizeDelta = size;
                Log(scenePath, objectPath, oldSize.ToString(), size.ToString());
                changed = true;
            }

            RecordOverrides(image, changed);
            return changed;
        }

        private static bool PatchRemainingSimpleImage(string scenePath, string objectPath, Image image)
        {
            if (image.type != Image.Type.Sliced && image.type != Image.Type.Tiled)
            {
                return false;
            }

            string assetPath = image.sprite == null ? string.Empty : AssetDatabase.GetAssetPath(image.sprite);
            if (!assetPath.StartsWith(UiFolder))
            {
                return false;
            }

            Image.Type oldType = image.type;
            image.type = Image.Type.Simple;
            Log(scenePath, objectPath, oldType.ToString(), image.type.ToString());
            RecordOverrides(image, true);
            return true;
        }

        // Without this, edits on a prefab instance are not stored as overrides and are lost when the scene saves.
        private static void RecordOverrides(Image image, bool changed)
        {
            if (!changed || !PrefabUtility.IsPartOfPrefabInstance(image))
            {
                return;
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
            PrefabUtility.RecordPrefabInstancePropertyModifications(image.rectTransform);
        }

        private static void PatchHotspotPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(HotspotPrefabPath);
            try
            {
                foreach (Image image in root.GetComponentsInChildren<Image>(true))
                {
                    if (image.name != "Plaque" || image.type == Image.Type.Simple)
                    {
                        continue;
                    }

                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                    image.pixelsPerUnitMultiplier = 1f;
                    Log(HotspotPrefabPath, image.name, "Sliced", "Simple, preserveAspect");
                }

                PrefabUtility.SaveAsPrefabAsset(root, HotspotPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static bool SetSprite(string scenePath, string objectPath, Image image, string spriteKey)
        {
            string assetPath = UiFolder + spriteKey + ".png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite == null)
            {
                Debug.LogError("Missing UI sprite: " + assetPath);
                return false;
            }

            bool changed = false;
            if (image.sprite != sprite)
            {
                string oldName = image.sprite == null ? "none" : image.sprite.name;
                image.sprite = sprite;
                Log(scenePath, objectPath, oldName, sprite.name);
                changed = true;
            }

            if (image.type != Image.Type.Simple)
            {
                Image.Type oldType = image.type;
                image.type = Image.Type.Simple;
                Log(scenePath, objectPath, oldType.ToString(), image.type.ToString());
                changed = true;
            }

            return changed;
        }

        private static string GetPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        private static void Log(string scenePath, string objectPath, string oldValue, string newValue)
        {
            Debug.Log(scenePath + " :: " + objectPath + " : " + oldValue + " -> " + newValue);
        }
    }
}
