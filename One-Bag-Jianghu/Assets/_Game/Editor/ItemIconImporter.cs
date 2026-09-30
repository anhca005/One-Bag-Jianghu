using System;
using System.IO;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools
{
    public sealed class ItemIconImporter : AssetPostprocessor
    {
        private const string TextureRoot = "Assets/_Game/Textures2D/";
        private const string UiRoot = TextureRoot + "UI/";
        private const string StatusRoot = TextureRoot + "Status/";

        // Borders are in source-image pixels (left, bottom, right, top) of the art cropped to its visible content. They keep the
        // generated corner/end ornaments intact while stretching only the quiet center.
        private static readonly IReadOnlyDictionary<string, Vector4> UiBorders =
            new Dictionary<string, Vector4>(StringComparer.OrdinalIgnoreCase)
            {
                { "button_primary", new Vector4(400f, 120f, 400f, 120f) },
                { "button_secondary", new Vector4(400f, 120f, 400f, 120f) },
                { "frame_panel", new Vector4(240f, 240f, 240f, 240f) },
                { "frame_tooltip", new Vector4(130f, 110f, 130f, 110f) },
                { "plaque_label", new Vector4(300f, 160f, 300f, 160f) },
                { "progress_bar", new Vector4(310f, 100f, 310f, 100f) }
            };

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(TextureRoot, StringComparison.OrdinalIgnoreCase))
                return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = assetPath.StartsWith(UiRoot, StringComparison.OrdinalIgnoreCase) ? 2048 :
                assetPath.StartsWith(StatusRoot, StringComparison.OrdinalIgnoreCase) ? 256 : 1024;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            if (assetPath.StartsWith(UiRoot, StringComparison.OrdinalIgnoreCase))
            {
                string key = Path.GetFileNameWithoutExtension(assetPath);
                Vector4 border;
                importer.spriteBorder = UiBorders.TryGetValue(key, out border) ? border : Vector4.zero;
            }
        }
    }

    public static class ItemIconLibraryBuilder
    {
        private const string LibraryPath = "Assets/_Game/Resources/ItemIconLibrary.asset";

        [MenuItem("One Bag Jianghu/Rebuild Icon Library")]
        [MenuItem("One Bag Jianghu/Reimport Art And Rebuild Icon Library")]
        public static void ReimportAndRebuild()
        {
            // Import settings (9-slice borders, max sizes) only apply when textures are reimported.
            AssetDatabase.ImportAsset("Assets/_Game/Textures2D", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            Rebuild();
        }

        public static void Rebuild()
        {
            ItemIconLibrary library = AssetDatabase.LoadAssetAtPath<ItemIconLibrary>(LibraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<ItemIconLibrary>();
                AssetDatabase.CreateAsset(library, LibraryPath);
            }
            library.Entries.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Game/Textures2D" });
            Array.Sort(guids, CompareAssetPaths);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
                    continue;
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null)
                {
                    // Imported before this postprocessor existed: reimport so it becomes a Sprite.
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
                if (sprite == null)
                {
                    Debug.LogWarning("Icon is not imported as a Sprite: " + path);
                    continue;
                }
                library.Entries.Add(new ItemIconLibrary.Entry
                {
                    ItemId = Path.GetFileNameWithoutExtension(path),
                    Sprite = sprite
                });
            }
            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
            Debug.Log($"Rebuilt item icon library with {library.Entries.Count} icons.");
        }

        private static int CompareAssetPaths(string leftGuid, string rightGuid)
        {
            return string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(leftGuid),
                AssetDatabase.GUIDToAssetPath(rightGuid));
        }
    }
}
