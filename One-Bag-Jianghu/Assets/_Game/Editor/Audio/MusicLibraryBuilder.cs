using System;
using System.Collections.Generic;
using System.IO;
using OneBagJianghu.Audio;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Audio
{
    public static class MusicLibraryBuilder
    {
        private const string AudioRoot = "Assets/_Game/Audio/Music";
        private const string LibraryPath = "Assets/_Game/Resources/Audio/MusicLibrary.asset";

        [InitializeOnLoadMethod]
        private static void EnsureLibraryEntries()
        {
            MusicLibrary library = AssetDatabase.LoadAssetAtPath<MusicLibrary>(LibraryPath);
            if (library == null || library.Entries.Count == 0)
            {
                Rebuild();
            }
        }

        [MenuItem("One Bag Jianghu/Audio/Rebuild Music Library")]
        public static void Rebuild()
        {
            MusicLibrary library = AssetDatabase.LoadAssetAtPath<MusicLibrary>(LibraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<MusicLibrary>();
                AssetDatabase.CreateAsset(library, LibraryPath);
            }

            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { AudioRoot });
            var paths = new List<string>(guids.Length);
            for (int i = 0; i < guids.Length; i++)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            paths.Sort(StringComparer.Ordinal);
            library.Entries.Clear();
            for (int i = 0; i < paths.Count; i++)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(paths[i]);
                if (clip == null)
                    continue;
                library.Entries.Add(new MusicLibrary.Entry
                {
                    Key = Path.GetFileNameWithoutExtension(paths[i]),
                    Clip = clip
                });
            }

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
            Debug.Log("Rebuilt music library with " + library.Entries.Count + " keys.");
        }
    }
}
