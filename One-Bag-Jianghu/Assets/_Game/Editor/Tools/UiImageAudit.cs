using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneBagJianghu.Editor.Tools
{
    public static class UiImageAudit
    {
        private const string MenuPath = "One Bag Jianghu/Audit UI Images";
        // Popups and the GameController live under Resources, so both prefab roots are scanned.
        private static readonly string[] PrefabFolders = { "Assets/_Game/Prefabs", "Assets/_Game/Resources" };
        private const string SceneFolder = "Assets/_Game/Scenes";
        private const string TextureFolder = "Assets/_Game/Textures2D/";
        private const float AspectErrorThreshold = 3f;
        private static readonly List<UseRecord> Uses = new List<UseRecord>();
        private static readonly List<string> Errors = new List<string>();

        [MenuItem(MenuPath)]
        public static void RunBatch()
        {
            Uses.Clear();
            Errors.Clear();
            AuditPrefabs();
            AuditScenes();
            WriteCsv();
            WriteReport();
            Debug.Log($"UI image audit complete: {Uses.Count} uses, {Errors.Count} errors.");
        }

        private static void AuditPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", PrefabFolders);
            foreach (string assetPath in guids.Select(AssetDatabase.GUIDToAssetPath).OrderBy(path => path))
            {
                try
                {
                    Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    RectTransform canvas = CreateAuditCanvas(scene);
                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    GameObject instance = PrefabUtility.InstantiatePrefab(asset, scene) as GameObject;
                    if (instance == null)
                        throw new InvalidOperationException("Prefab could not be instantiated.");
                    instance.transform.SetParent(canvas, false);
                    StretchToFill(instance.transform as RectTransform);
                    ForceLayout(instance);
                    CollectImages("Prefab", assetPath, instance);
                }
                catch (Exception exception)
                {
                    RecordError("Prefab", assetPath, exception);
                }
            }
        }

        private static void AuditScenes()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { SceneFolder });
            foreach (string assetPath in guids.Select(AssetDatabase.GUIDToAssetPath).OrderBy(path => path))
            {
                try
                {
                    // Opened only to measure; the scene is never saved, so pinning its canvases is harmless.
                    Scene scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Single);
                    foreach (GameObject root in scene.GetRootGameObjects())
                    {
                        foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
                        {
                            if (canvas.isRootCanvas)
                                PinToReferenceSize(canvas);
                        }
                    }
                    foreach (GameObject root in scene.GetRootGameObjects())
                        ForceLayout(root);
                    foreach (GameObject root in scene.GetRootGameObjects())
                        CollectImages("Scene", assetPath, root);
                }
                catch (Exception exception)
                {
                    RecordError("Scene", assetPath, exception);
                }
            }
        }

        private static RectTransform CreateAuditCanvas(Scene scene)
        {
            GameObject canvasObject = new GameObject(
                "UiImageAuditCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            SceneManager.MoveGameObjectToScene(canvasObject, scene);
            PinToReferenceSize(canvasObject.GetComponent<Canvas>());
            return canvasObject.GetComponent<RectTransform>();
        }

        // Batch mode has no real screen, so an overlay canvas would take an arbitrary size. A world-space canvas
        // of exactly 1920x1080 gives the same canvas units the game's 1920x1080 reference scaler produces.
        private static void PinToReferenceSize(Canvas canvas)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(1920f, 1080f);
        }

        // Only screens and popups that are authored stretched fill the canvas; component prefabs
        // (buttons, sliders, cards) keep their own size.
        private static void StretchToFill(RectTransform rect)
        {
            if (rect == null || rect.anchorMin == rect.anchorMax)
                return;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ForceLayout(GameObject root)
        {
            for (int pass = 0; pass < 2; pass++)
            {
                Canvas.ForceUpdateCanvases();
                RectTransform rect = root.transform as RectTransform;
                if (rect != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        private static void CollectImages(string sourceKind, string sourcePath, GameObject root)
        {
            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                string spritePath = image.sprite == null ? string.Empty : AssetDatabase.GetAssetPath(image.sprite);
                if (!spritePath.StartsWith(TextureFolder, StringComparison.Ordinal))
                    continue;
                Rect rect = image.rectTransform.rect;
                float width = Mathf.Abs(rect.width);
                float height = Mathf.Abs(rect.height);
                Rect spriteRect = image.sprite.rect;
                float error = CalculateAspectError(width, height, spriteRect.width, spriteRect.height);
                bool wrongSimple = image.type == Image.Type.Simple && !image.preserveAspect;
                if (image.type != Image.Type.Sliced && image.type != Image.Type.Tiled &&
                    (!wrongSimple || error <= AspectErrorThreshold))
                    continue;
                Uses.Add(new UseRecord
                {
                    SourceKind = sourceKind,
                    SourcePath = sourcePath,
                    ObjectPath = GetObjectPath(image.transform, root.transform),
                    SpritePath = spritePath,
                    ImageType = image.type.ToString(),
                    PreserveAspect = image.preserveAspect,
                    PixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier,
                    RectWidth = width,
                    RectHeight = height,
                    SpriteWidth = spriteRect.width,
                    SpriteHeight = spriteRect.height,
                    AspectError = error,
                    Driven = IsSizeDriven(image.rectTransform),
                    Inactive = !image.gameObject.activeInHierarchy
                });
            }
        }

        private static float CalculateAspectError(float width, float height, float spriteWidth, float spriteHeight)
        {
            if (width <= 0f || height <= 0f || spriteWidth <= 0f || spriteHeight <= 0f)
                return float.PositiveInfinity;
            double rectAspect = width / height;
            double spriteAspect = spriteWidth / spriteHeight;
            return (float)(Math.Abs(Math.Log(rectAspect / spriteAspect)) * 100d);
        }

        private static bool IsSizeDriven(RectTransform rect)
        {
            Transform current = rect;
            while (current != null)
            {
                if (current.GetComponent<ContentSizeFitter>() != null)
                    return true;
                if (current != rect && current.GetComponent<LayoutGroup>() != null)
                    return true;
                current = current.parent;
            }
            return false;
        }

        private static string GetObjectPath(Transform target, Transform auditRoot)
        {
            List<string> names = new List<string>();
            Transform current = target;
            while (current != null)
            {
                names.Add(current.name);
                if (current == auditRoot)
                    break;
                current = current.parent;
            }
            names.Reverse();
            return string.Join("/", names);
        }

        private static void RecordError(string kind, string path, Exception exception)
        {
            string message = $"{kind} {path}: {exception.GetType().Name}: {exception.Message}";
            Errors.Add(message);
            Debug.LogError(message);
        }

        private static void WriteCsv()
        {
            string outputPath = GetRepositoryPath("tools/codex/out/ui-image-audit.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("source_kind,source_path,game_object_path,sprite_asset_path,image_type,preserve_aspect," +
                "pixels_per_unit_multiplier,rect_width,rect_height,sprite_width,sprite_height,rect_aspect," +
                "sprite_aspect,aspect_error_percent,size_driven,inactive,class");
            foreach (UseRecord use in Uses.OrderBy(item => item.SpritePath).ThenBy(item => item.SourcePath))
                csv.AppendLine(ToCsvRow(use));
            File.WriteAllText(outputPath, csv.ToString(), new UTF8Encoding(false));
        }

        private static string ToCsvRow(UseRecord use)
        {
            string[] values =
            {
                use.SourceKind,
                use.SourcePath,
                use.ObjectPath,
                use.SpritePath,
                use.ImageType,
                use.PreserveAspect.ToString(),
                Format(use.PixelsPerUnitMultiplier),
                Format(use.RectWidth),
                Format(use.RectHeight),
                Format(use.SpriteWidth),
                Format(use.SpriteHeight),
                Format(use.RectAspect),
                Format(use.SpriteAspect),
                Format(use.AspectError),
                use.Driven.ToString(),
                use.Inactive.ToString(),
                Classify(use)
            };
            return string.Join(",", values.Select(EscapeCsv));
        }

        private static void WriteReport()
        {
            string outputPath = GetRepositoryPath("docs/art/ui-image-audit.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            StringBuilder report = new StringBuilder();
            report.AppendLine("# UI image distortion audit");
            report.AppendLine();
            report.AppendLine("Generated at 1920×1080. Aspect error is `abs(ln(rect aspect / sprite aspect)) × 100`. ");
            report.AppendLine("Only Sliced/Tiled uses and distorted Simple uses above 3% are included.");
            report.AppendLine();
            AppendSummary(report);
            AppendGroupedUses(report);
            AppendRuntimeSites(report);
            AppendFallbacks(report);
            AppendErrors(report);
            AppendSpriteSummary(report);
            File.WriteAllText(outputPath, report.ToString(), new UTF8Encoding(false));
        }

        private static void AppendSummary(StringBuilder report)
        {
            report.AppendLine("## Summary");
            report.AppendLine();
            report.AppendLine("| Class | Uses |");
            report.AppendLine("|---|---:|");
            foreach (string className in new[] { "A", "B", "C", "D" })
                report.AppendLine($"| {className} | {Uses.Count(use => Classify(use) == className)} |");
            report.AppendLine();
            report.AppendLine($"Proposed new art files: **{CountNewArtFiles()}**.");
            report.AppendLine();
            report.AppendLine("### Five worst distortions");
            report.AppendLine();
            foreach (UseRecord use in Uses.OrderByDescending(item => item.AspectError).Take(5))
                report.AppendLine($"- {use.AspectError:0.##}% — `{use.SpritePath}` at " +
                    $"`{use.SourcePath} :: {use.ObjectPath}` ({use.RectWidth:0.##}×{use.RectHeight:0.##}).");
            report.AppendLine();
        }

        private static void AppendGroupedUses(StringBuilder report)
        {
            report.AppendLine("## Measured uses by sprite");
            report.AppendLine();
            IEnumerable<IGrouping<string, UseRecord>> groups = Uses.GroupBy(use => use.SpritePath)
                .OrderBy(group => group.Key);
            foreach (IGrouping<string, UseRecord> group in groups)
            {
                report.AppendLine($"### `{group.Key}`");
                report.AppendLine();
                report.AppendLine("| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | " +
                    "Aspects | Error | Driven | Inactive |");
                report.AppendLine("|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|");
                foreach (UseRecord use in group.OrderBy(item => item.SourcePath).ThenBy(item => item.ObjectPath))
                    AppendUseRow(report, use);
                report.AppendLine();
                AppendProposal(report, group.ToList());
            }
        }

        private static void AppendUseRow(StringBuilder report, UseRecord use)
        {
            report.AppendLine($"| {Classify(use)} | `{use.SourcePath} :: {use.ObjectPath}` | {use.ImageType} | " +
                $"{use.PreserveAspect} | {use.PixelsPerUnitMultiplier:0.###} | " +
                $"{use.RectWidth:0.##}×{use.RectHeight:0.##} | {use.SpriteWidth:0}×{use.SpriteHeight:0} | " +
                $"{use.RectAspect:0.###} / {use.SpriteAspect:0.###} | {use.AspectError:0.##}% | " +
                $"{use.Driven} | {use.Inactive} |");
        }

        private static void AppendProposal(StringBuilder report, List<UseRecord> uses)
        {
            List<UseRecord> newArtUses = uses.Where(use => Classify(use) == "C").ToList();
            if (newArtUses.Count == 0)
                return;
            report.AppendLine("Proposal: new art for these target aspect groups:");
            foreach (List<UseRecord> cluster in ClusterByAspect(newArtUses))
            {
                UseRecord sample = cluster[0];
                Vector2Int size = TargetPixelSize(cluster);
                string name = SuggestedFileName(sample, size);
                string driven = cluster.Any(use => use.Driven)
                    ? " Driven size: fix to the representative canvas size shown, " +
                        "or define explicit compact/regular sizes."
                    : string.Empty;
                report.AppendLine($"- `{name}` at {size.x}×{size.y}px for about " +
                    $"{sample.RectWidth:0}×{sample.RectHeight:0} canvas units.{driven}");
            }
            report.AppendLine();
        }

        private static string Classify(UseRecord use)
        {
            if (use.AspectError <= 5f)
                return "A";
            string text = (use.SpritePath + " " + use.ObjectPath).ToLowerInvariant();
            // Whole words only: "hp" would also match "shop" and "qi" would match "equip".
            if (Regex.IsMatch(text, "fill|progress|slider|cooldown|\\bhp\\b|\\bqi\\b|health"))
                return "B";
            if (float.IsNaN(use.AspectError) || float.IsInfinity(use.AspectError) ||
                use.RectWidth < 1f || use.RectHeight < 1f)
                return "D";
            return "C";
        }

        private static List<List<UseRecord>> ClusterByAspect(List<UseRecord> uses)
        {
            List<List<UseRecord>> clusters = new List<List<UseRecord>>();
            foreach (UseRecord use in uses.OrderBy(item => item.RectAspect))
            {
                List<UseRecord> cluster = clusters.FirstOrDefault(items =>
                    Math.Abs(Math.Log(use.RectAspect / items[0].RectAspect)) * 100d <= 5d);
                if (cluster == null)
                {
                    cluster = new List<UseRecord>();
                    clusters.Add(cluster);
                }
                cluster.Add(use);
            }
            return clusters;
        }

        private static Vector2Int TargetPixelSize(List<UseRecord> uses)
        {
            float width = uses.Max(use => use.RectWidth);
            float height = uses.Max(use => use.RectHeight);
            float scale = Mathf.Min(3f, 2048f / Mathf.Max(width, height));
            scale = Mathf.Max(2f, scale);
            if (Mathf.Max(width, height) * scale > 2048f)
                scale = 2048f / Mathf.Max(width, height);
            return new Vector2Int(Mathf.Max(1, Mathf.RoundToInt(width * scale)),
                Mathf.Max(1, Mathf.RoundToInt(height * scale)));
        }

        private static string SuggestedFileName(UseRecord use, Vector2Int size)
        {
            string stem = Path.GetFileNameWithoutExtension(use.SpritePath).ToLowerInvariant();
            return $"{stem}_{size.x}x{size.y}.png";
        }

        private static int CountNewArtFiles()
        {
            return Uses.Where(use => Classify(use) == "C")
                .GroupBy(use => use.SpritePath)
                .Sum(group => ClusterByAspect(group.ToList()).Count);
        }

        private static void AppendRuntimeSites(StringBuilder report)
        {
            report.AppendLine("## Runtime");
            report.AppendLine();
            report.AppendLine("Code-created or code-mutated Images found by the requested source scan. " +
                "The final field is the nearest visible sprite key or size expression when present.");
            report.AppendLine();
            foreach (SourceHit hit in ScanRuntimeSites())
                report.AppendLine($"- `{hit.Path}:{hit.Line}` — `{hit.Text}`");
            report.AppendLine();
        }

        private static List<SourceHit> ScanRuntimeSites()
        {
            string scriptsRoot = GetProjectPath("Assets/_Game/Scripts");
            Regex pattern = new Regex(
                @"Image\.Type\.(Sliced|Tiled)|\.type\s*=|pixelsPerUnitMultiplier|SlicedAutoFit",
                RegexOptions.Compiled);
            return ScanSources(scriptsRoot, pattern);
        }

        private static void AppendFallbacks(StringBuilder report)
        {
            report.AppendLine("## Fallbacks");
            report.AppendLine();
            List<FallbackHit> fallbacks = ScanFallbacks();
            if (fallbacks.Count == 0)
                report.AppendLine("No adjacent `ApplyUiSprite` fallback chains were found.");
            foreach (FallbackHit fallback in fallbacks)
            {
                string exists = SpriteKeyExists(fallback.Wanted) ? "yes" : "no";
                report.AppendLine($"- `{fallback.Path}:{fallback.Line}` — wanted `{fallback.Wanted}`, " +
                    $"fallback `{fallback.Fallback}`; wanted file exists: **{exists}**.");
            }
            report.AppendLine();
        }

        private static List<FallbackHit> ScanFallbacks()
        {
            string root = GetProjectPath("Assets/_Game");
            Regex call = new Regex("ApplyUiSprite\\s*\\([^\\n]*?\"([^\"]+)\"", RegexOptions.Compiled);
            List<FallbackHit> hits = new List<FallbackHit>();
            foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(file);
                for (int index = 0; index < lines.Length; index++)
                {
                    if (!lines[index].Contains("!UiBuild.ApplyUiSprite") &&
                        !lines[index].Contains("!ApplyUiSprite"))
                        continue;
                    Match wanted = call.Match(lines[index]);
                    for (int next = index + 1; next < Math.Min(lines.Length, index + 5); next++)
                    {
                        Match fallback = call.Match(lines[next]);
                        if (!wanted.Success || !fallback.Success)
                            continue;
                        hits.Add(new FallbackHit
                        {
                            Path = ToProjectRelative(file),
                            Line = index + 1,
                            Wanted = wanted.Groups[1].Value,
                            Fallback = fallback.Groups[1].Value
                        });
                        break;
                    }
                }
            }
            return hits;
        }

        private static bool SpriteKeyExists(string key)
        {
            string fileName = Path.GetFileName(key);
            string[] guids = AssetDatabase.FindAssets(fileName + " t:Sprite", new[] { "Assets/_Game/Textures2D" });
            return guids.Select(AssetDatabase.GUIDToAssetPath)
                .Any(path => string.Equals(Path.GetFileNameWithoutExtension(path), fileName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static List<SourceHit> ScanSources(string root, Regex pattern)
        {
            List<SourceHit> hits = new List<SourceHit>();
            foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(file);
                for (int index = 0; index < lines.Length; index++)
                {
                    if (!pattern.IsMatch(lines[index]))
                        continue;
                    hits.Add(new SourceHit
                    {
                        Path = ToProjectRelative(file),
                        Line = index + 1,
                        Text = lines[index].Trim().Replace("|", "\\|")
                    });
                }
            }
            return hits.OrderBy(hit => hit.Path).ThenBy(hit => hit.Line).ToList();
        }

        private static void AppendErrors(StringBuilder report)
        {
            report.AppendLine("## Could not be measured");
            report.AppendLine();
            if (Errors.Count == 0)
                report.AppendLine("None.");
            foreach (string error in Errors)
                report.AppendLine($"- {error}");
            report.AppendLine();
        }

        private static void AppendSpriteSummary(StringBuilder report)
        {
            report.AppendLine("## Sprite proposal summary");
            report.AppendLine();
            report.AppendLine("| Sprite | Class | New art files needed |");
            report.AppendLine("|---|---|---|");
            IEnumerable<IGrouping<string, UseRecord>> groups = Uses.GroupBy(use => use.SpritePath)
                .OrderBy(group => group.Key);
            foreach (IGrouping<string, UseRecord> group in groups)
            {
                string classes = string.Join("/", group.Select(Classify).Distinct().OrderBy(value => value));
                List<string> files = new List<string>();
                foreach (List<UseRecord> cluster in ClusterByAspect(group.Where(use => Classify(use) == "C").ToList()))
                    files.Add(SuggestedFileName(cluster[0], TargetPixelSize(cluster)));
                string fileText = files.Count == 0 ? "—" : string.Join("<br>", files.Select(file => $"`{file}`"));
                report.AppendLine($"| `{group.Key}` | {classes} | {fileText} |");
            }
        }

        private static string GetRepositoryPath(string relativePath)
        {
            string repositoryRoot = Directory.GetParent(GetProjectPath(string.Empty)).FullName;
            return Path.Combine(repositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string GetProjectPath(string relativePath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string ToProjectRelative(string path)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar;
            return path.Replace(projectRoot, string.Empty).Replace('\\', '/');
        }

        private static string EscapeCsv(string value)
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static string Format(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private sealed class UseRecord
        {
            public string SourceKind;
            public string SourcePath;
            public string ObjectPath;
            public string SpritePath;
            public string ImageType;
            public bool PreserveAspect;
            public float PixelsPerUnitMultiplier;
            public float RectWidth;
            public float RectHeight;
            public float SpriteWidth;
            public float SpriteHeight;
            public float AspectError;
            public bool Driven;
            public bool Inactive;
            public float RectAspect => RectHeight > 0f ? RectWidth / RectHeight : float.PositiveInfinity;
            public float SpriteAspect => SpriteHeight > 0f ? SpriteWidth / SpriteHeight : float.PositiveInfinity;
        }

        private sealed class SourceHit
        {
            public string Path;
            public int Line;
            public string Text;
        }

        private sealed class FallbackHit
        {
            public string Path;
            public int Line;
            public string Wanted;
            public string Fallback;
        }
    }
}
