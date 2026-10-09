using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools
{
    /// <summary>
    /// Sets the game's application icon (the .exe and taskbar icon) from the chosen artwork.
    /// Runs once automatically when no default icon is set, and is also available from the menu.
    /// </summary>
    [InitializeOnLoad]
    public static class AppIconSetup
    {
        private const string IconPath = "Assets/_Game/Textures2D/UI/app_icon_alt.png";
        private const string ProductName = "One Bag Jianghu";

        static AppIconSetup()
        {
            EditorApplication.delayCall += ApplyIfMissing;
            EditorApplication.delayCall += ApplyProductName;
        }

        [MenuItem("One Bag Jianghu/Build/Apply App Icon")]
        public static void Apply()
        {
            PrepareImport();
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            if (icon == null)
            {
                Debug.LogWarning($"[AppIcon] Icon not found at {IconPath}");
                return;
            }

            PlayerSettings.SetIcons(UnityEditor.Build.NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AppIcon] Default icon set to {IconPath}");
        }

        // The window title and build name; the old project name had dashes.
        private static void ApplyProductName()
        {
            if (PlayerSettings.productName == ProductName)
                return;
            PlayerSettings.productName = ProductName;
            AssetDatabase.SaveAssets();
            Debug.Log($"[AppIcon] Product name set to {ProductName}");
        }

        private static void ApplyIfMissing()
        {
            Texture2D[] icons = PlayerSettings.GetIcons(UnityEditor.Build.NamedBuildTarget.Unknown, IconKind.Any);
            if (icons.Length > 0 && icons[0] != null)
                return;
            Apply();
        }

        // Icons must keep full quality and a clean transparent edge.
        private static void PrepareImport()
        {
            var importer = AssetImporter.GetAtPath(IconPath) as TextureImporter;
            if (importer == null)
                return;
            bool changed = importer.textureType != TextureImporterType.Default
                || !importer.alphaIsTransparency
                || importer.maxTextureSize < 1024
                || importer.textureCompression != TextureImporterCompression.Uncompressed;
            if (!changed)
                return;
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
