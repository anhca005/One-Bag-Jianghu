using System.IO;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools
{
    public static class DataSyncMenu
    {
        [MenuItem("One Bag Jianghu/Sync Data From Server")]
        public static void Sync()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            string sourceFolder = Path.GetFullPath(Path.Combine(projectRoot ?? string.Empty,
                "../server/battle-engine/src/main/resources/data"));
            if (!Directory.Exists(sourceFolder))
            {
                Debug.LogError("Server data folder was not found: " + sourceFolder);
                return;
            }

            string targetFolder = Path.Combine(Application.dataPath, "_Game/Resources/Data");
            Directory.CreateDirectory(targetFolder);
            SyncFile(sourceFolder, targetFolder, "items.json");
            SyncFile(sourceFolder, targetFolder, "sects.json");
            string samplesSource = Path.GetFullPath(Path.Combine(projectRoot ?? string.Empty,
                "../server/battle-engine/samples"));
            string samplesTarget = Path.Combine(targetFolder, "Samples");
            if (Directory.Exists(samplesSource))
            {
                Directory.CreateDirectory(samplesTarget);
                foreach (string sample in Directory.GetFiles(samplesSource, "*.json"))
                    SyncFile(samplesSource, samplesTarget, Path.GetFileName(sample));
            }
            else
                Debug.LogError("Server samples folder was not found: " + samplesSource);
            AssetDatabase.Refresh();
        }

        private static void SyncFile(string sourceFolder, string targetFolder, string fileName)
        {
            string source = Path.Combine(sourceFolder, fileName);
            string target = Path.Combine(targetFolder, fileName);
            if (!File.Exists(source))
            {
                Debug.LogError("Server data file was not found: " + source);
                return;
            }

            byte[] sourceBytes = File.ReadAllBytes(source);
            bool changed = !File.Exists(target) || !BytesEqual(sourceBytes, File.ReadAllBytes(target));
            if (changed)
                File.WriteAllBytes(target, sourceBytes);
            Debug.Log($"Data sync {(changed ? "updated" : "unchanged")}: {fileName}");
        }

        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
                return false;
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                    return false;
            }

            return true;
        }
    }
}