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
        private const string PveRoot = TextureRoot + "Pve/";
        private const string SectRoot = TextureRoot + "Sects/";
        private const string StatusRoot = TextureRoot + "Status/";
        // Borders are in source-image pixels (left, bottom, right, top) of the art cropped to its visible content. They keep the
        // generated corner/end ornaments intact while stretching only the quiet center.
        private static readonly IReadOnlyDictionary<string,
            Vector4> UiBorders = new Dictionary<string,
            Vector4>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "button_primary",
                new Vector4(400f, 120f, 400f, 120f)
            },
            {
                "button_secondary",
                new Vector4(400f, 120f, 400f, 120f)
            },
            {
                "frame_panel",
                new Vector4(240f, 240f, 240f, 240f)
            },
            {
                "frame_tooltip",
                new Vector4(130f, 110f, 130f, 110f)
            },
            {
                "plaque_label",
                // Bottom 102 px = lower frame; top 277 px = the hanging ties plus the upper frame, so only the
                // paper stretches and labels can centre on it.
                new Vector4(300f, 102f, 300f, 277f)
            },
            {
                "progress_bar",
                new Vector4(310f, 100f, 310f, 100f)
            },
            {
                "home_banner",
                new Vector4(520f, 80f, 540f, 80f)
            },
            {
                "currency_slot",
                new Vector4(860f, 100f, 300f, 100f)
            },
            // Round-corner popup frame; the wide quiet centre is the only stretchable area.
            { "popup_frame", new Vector4(120f, 120f, 120f, 120f) },
            // Horizontal ribbon: preserve swallow tails and tassels.
            { "popup_title_ribbon", new Vector4(310f, 175f, 310f, 60f) },
            // Tabs preserve the hanging knot and shaped lower corners.
            { "tab_active", new Vector4(150f, 80f, 150f, 80f) },
            { "tab_idle", new Vector4(150f, 80f, 150f, 80f) },
            // Inset and row corners are deliberately small at their target sizes.
            { "panel_inset", new Vector4(90f, 90f, 90f, 90f) },
            { "list_row", new Vector4(190f, 35f, 190f, 35f) },
            { "list_row_selected", new Vector4(190f, 35f, 190f, 35f) },
            // Buttons retain their end clouds while the plain lacquer centre stretches.
            { "button_disabled", new Vector4(330f, 40f, 330f, 40f) },
            { "button_small_gold", new Vector4(250f, 45f, 250f, 45f) },
            // Horizontal control caps and vertical scrollbar caps remain unstretched.
            { "slider_track", new Vector4(180f, 20f, 180f, 20f) },
            { "slider_fill", new Vector4(150f, 20f, 150f, 20f) },
            { "scrollbar_track", new Vector4(15f, 180f, 15f, 180f) },
            { "scrollbar_handle", new Vector4(18f, 190f, 18f, 190f) },
            { "dropdown_field", new Vector4(145f, 50f, 300f, 50f) },
            // Inventory and merchant surfaces preserve their stitched/corner ornaments.
            { "slot_cell", new Vector4(95f, 95f, 95f, 95f) },
            { "slot_cell_highlight", new Vector4(95f, 95f, 95f, 95f) },
            { "slot_cell_invalid", new Vector4(95f, 95f, 95f, 95f) },
            { "bag_board", new Vector4(180f, 150f, 180f, 150f) },
            { "shop_card", new Vector4(100f, 170f, 100f, 170f) },
            // shop_card split for landscape cards: frame without the cord (footer band in the bottom border).
            { "shop_card_frame", new Vector4(135f, 290f, 135f, 120f) },
            { "tier_pill", new Vector4(250f, 100f, 250f, 100f) },
            { "price_tag", new Vector4(640f, 60f, 360f, 60f) },
            // Battle/result entries are imported now for the part-two builders.
            { "bar_frame", new Vector4(260f, 60f, 260f, 60f) },
            { "bar_fill_hp", new Vector4(180f, 50f, 180f, 50f) },
            { "bar_fill_shield", new Vector4(180f, 50f, 180f, 50f) },
            { "bar_fill_qi", new Vector4(180f, 50f, 180f, 50f) },
            { "fighter_panel", new Vector4(110f, 170f, 110f, 170f) },
            { "timeline_bar", new Vector4(260f, 50f, 260f, 50f) },
            { "reward_slot", new Vector4(110f, 110f, 110f, 110f) },
            // Group H battle art: borders match the measured corner and end-cap ornaments.
            { "hud_panel", new Vector4(145f, 42f, 145f, 42f) },
            { "gauge_frame_large", new Vector4(150f, 24f, 150f, 24f) },
            { "gauge_frame_small", new Vector4(150f, 13f, 150f, 13f) },
            { "status_slot", new Vector4(230f, 230f, 230f, 230f) },
            { "count_badge", new Vector4(235f, 105f, 235f, 105f) },
            { "log_panel", new Vector4(150f, 45f, 150f, 45f) },
            { "button_exit", new Vector4(230f, 230f, 230f, 230f) },
            { "name_plate", new Vector4(190f, 55f, 190f, 55f) },
            { "clock_plate", new Vector4(215f, 90f, 215f, 90f) },
            { "button_square", new Vector4(230f, 230f, 230f, 230f) }
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
            importer.maxTextureSize = assetPath.StartsWith(UiRoot,
                StringComparison.OrdinalIgnoreCase)
                || assetPath.StartsWith(PveRoot,
                StringComparison.OrdinalIgnoreCase)
                || assetPath.StartsWith(SectRoot,
                StringComparison.OrdinalIgnoreCase) ? 2048 : assetPath.StartsWith(StatusRoot,
                StringComparison.OrdinalIgnoreCase) ? 256 : 1024;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            if (assetPath.StartsWith(UiRoot,
                StringComparison.OrdinalIgnoreCase)
                || assetPath.StartsWith(PveRoot,
                StringComparison.OrdinalIgnoreCase)
                || assetPath.StartsWith(SectRoot,
                StringComparison.OrdinalIgnoreCase))
            {
                // Borders are seeded only on a texture's first import. After that the .meta owns them, so
                // edits made in the Sprite Editor survive reimports instead of snapping back to this table.
                if (!importer.importSettingsMissing)
                    return;
                string key = Path.GetFileNameWithoutExtension(assetPath);
                Vector4 border;
                if (UiBorders.TryGetValue(key, out border))
                    importer.spriteBorder = border;
                else if (key.StartsWith("frame_", StringComparison.OrdinalIgnoreCase))
                {
                    importer.GetSourceTextureWidthAndHeight(out int width, out int height);
                    importer.spriteBorder = new Vector4(Mathf.Round(width * .12f),
                        Mathf.Round(height * .12f),
                        Mathf.Round(width * .12f),
                        Mathf.Round(height * .12f));
                }
                else
                    importer.spriteBorder = Vector4.zero;
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
            AssetDatabase.ImportAsset("Assets/_Game/Textures2D",
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
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
