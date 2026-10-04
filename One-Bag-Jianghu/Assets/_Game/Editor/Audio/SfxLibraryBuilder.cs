using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OneBagJianghu.Audio;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Audio
{
    public static class SfxLibraryBuilder
    {
        private const string AudioRoot = "Assets/_Game/Audio/Sfx";
        private const string CatalogPath = "Assets/_Game/Resources/Data/Audio/sfx-catalog.json";
        private const string LibraryPath = "Assets/_Game/Resources/Audio/SfxLibrary.asset";

        [InitializeOnLoadMethod]
        private static void EnsureCatalogEntries()
        {
            SfxLibrary library = AssetDatabase.LoadAssetAtPath<SfxLibrary>(LibraryPath);
            if (library == null || library.Entries.Count == 0)
            {
                Rebuild();
            }
        }

        [MenuItem("One Bag Jianghu/Audio/Rebuild Sfx Library")]
        public static void Rebuild()
        {
            SfxLibrary library = AssetDatabase.LoadAssetAtPath<SfxLibrary>(LibraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<SfxLibrary>();
                AssetDatabase.CreateAsset(library, LibraryPath);
            }

            Dictionary<string, List<AudioClip>> clips = GroupClips(FindAudioPaths());
            List<SfxCatalogRecord> catalog = ReadCatalog();
            library.Entries.Clear();
            for (int i = 0; i < catalog.Count; i++)
            {
                SfxCatalogRecord record = catalog[i];
                clips.TryGetValue(record.Key, out List<AudioClip> variants);
                library.Entries.Add(new SfxLibrary.Entry
                {
                    Key = record.Key,
                    Variants = variants == null ? Array.Empty<AudioClip>() : variants.ToArray(),
                    BaseVolume = DbToLinear(record.VolumeDb.GetValueOrDefault()),
                    PitchJitter = .05f,
                    MinimumInterval = .08f,
                    CatalogDuration = record.DurationSec
                });
            }

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
            Debug.Log("Rebuilt SFX library with " + library.Entries.Count + " catalog keys.");
        }

        public static Dictionary<string, List<string>> GroupFileNames(IEnumerable<string> paths)
        {
            var groups = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (string path in paths)
            {
                string key = KeyFromFileName(Path.GetFileNameWithoutExtension(path));
                if (!groups.TryGetValue(key, out List<string> group))
                {
                    group = new List<string>();
                    groups.Add(key, group);
                }

                group.Add(path);
            }

            foreach (List<string> group in groups.Values)
            {
                group.Sort(StringComparer.Ordinal);
            }

            return groups;
        }

        private static Dictionary<string, List<AudioClip>> GroupClips(IEnumerable<string> paths)
        {
            Dictionary<string, List<string>> groupedPaths = GroupFileNames(paths);
            var result = new Dictionary<string, List<AudioClip>>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, List<string>> pair in groupedPaths)
            {
                var clips = new List<AudioClip>();
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(pair.Value[i]);
                    if (clip != null)
                    {
                        clips.Add(clip);
                    }
                }

                result[pair.Key] = clips;
            }

            return result;
        }

        private static IEnumerable<string> FindAudioPaths()
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { AudioRoot });
            var paths = new List<string>(guids.Length);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string extension = Path.GetExtension(path);
                if (extension.Equals(".ogg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".wav", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    paths.Add(path);
                }
            }

            paths.Sort(StringComparer.Ordinal);
            return paths;
        }

        private static List<SfxCatalogRecord> ReadCatalog()
        {
            string json = File.ReadAllText(CatalogPath);
            return JsonConvert.DeserializeObject<List<SfxCatalogRecord>>(json) ??
                throw new FormatException("SFX catalog JSON is null.");
        }

        private static string KeyFromFileName(string fileName)
        {
            int separator = fileName.LastIndexOf('_');
            if (separator < 0 || separator == fileName.Length - 1)
                return fileName;
            return int.TryParse(fileName.Substring(separator + 1), out _) ?
                fileName.Substring(0, separator) :
                fileName;
        }

        private static float DbToLinear(float decibels)
        {
            return Mathf.Pow(10f, decibels / 20f);
        }
    }
}
