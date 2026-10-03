using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace OneBagJianghu.Tests.Play
{
    [SetUpFixture]
    public sealed class PlayerDataGuard
    {
        private static readonly string[] FileNames =
        {
            "profile.json",
            "profile.json.bak",
            "profile.json.tmp",
            "loadout.json"
        };

        private static readonly string[] PreferenceKeys =
        {
            "OneBagJianghu.HeroName",
            "OneBagJianghu.SectId",
            "OneBagJianghu.ServerUrl"
        };

        private readonly Dictionary<string, byte[]> fileSnapshots = new Dictionary<string, byte[]>();
        private readonly Dictionary<string, string> preferenceSnapshots = new Dictionary<string, string>();
        private readonly HashSet<string> existingPreferences = new HashSet<string>();

        [OneTimeSetUp]
        public void SnapshotPlayerData()
        {
            string guardDirectory = Path.Combine(Application.persistentDataPath, "test-guard");
            Directory.CreateDirectory(guardDirectory);

            foreach (string fileName in FileNames)
            {
                SnapshotFile(fileName, guardDirectory);
            }

            foreach (string key in PreferenceKeys)
            {
                SnapshotPreference(key);
            }

            WritePreferenceRecoveryCopy(guardDirectory);
        }

        [OneTimeTearDown]
        public void RestorePlayerData()
        {
            foreach (string fileName in FileNames)
            {
                RestoreFile(fileName);
            }

            foreach (string key in PreferenceKeys)
            {
                RestorePreference(key);
            }

            PlayerPrefs.Save();
        }

        private void SnapshotFile(string fileName, string guardDirectory)
        {
            string sourcePath = Path.Combine(Application.persistentDataPath, fileName);
            string recoveryPath = Path.Combine(guardDirectory, fileName);
            string missingPath = recoveryPath + ".missing";
            byte[] contents = File.Exists(sourcePath) ? File.ReadAllBytes(sourcePath) : null;
            fileSnapshots[fileName] = contents;

            if (contents != null)
            {
                File.WriteAllBytes(recoveryPath, contents);
                DeleteIfPresent(missingPath);
                return;
            }

            File.WriteAllBytes(recoveryPath, new byte[0]);
            File.WriteAllText(missingPath, string.Empty);
        }

        private void SnapshotPreference(string key)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }

            existingPreferences.Add(key);
            preferenceSnapshots[key] = PlayerPrefs.GetString(key, string.Empty);
        }

        private void WritePreferenceRecoveryCopy(string guardDirectory)
        {
            var lines = new List<string>();
            foreach (string key in PreferenceKeys)
            {
                bool existed = existingPreferences.Contains(key);
                string value = existed ? preferenceSnapshots[key] : string.Empty;
                lines.Add(key + "\t" + existed + "\t" + value);
            }

            File.WriteAllLines(Path.Combine(guardDirectory, "player-prefs.tsv"), lines);
        }

        private void RestoreFile(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            byte[] contents = fileSnapshots[fileName];
            if (contents == null)
            {
                DeleteIfPresent(path);
                return;
            }

            File.WriteAllBytes(path, contents);
        }

        private void RestorePreference(string key)
        {
            if (existingPreferences.Contains(key))
            {
                PlayerPrefs.SetString(key, preferenceSnapshots[key]);
                return;
            }

            PlayerPrefs.DeleteKey(key);
        }

        private static void DeleteIfPresent(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
