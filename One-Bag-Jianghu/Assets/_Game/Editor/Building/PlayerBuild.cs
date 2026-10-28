using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Building
{
    /// <summary>
    /// Builds the Windows player into Builds/Windows next to the Unity project.
    /// Batch: -executeMethod OneBagJianghu.EditorTools.Building.PlayerBuild.BuildWindows [-server ws://host:port/ws]
    /// With -server, the address is written to the build's StreamingAssets/server.txt.
    /// </summary>
    public static class PlayerBuild
    {
        private const string OutputFolder = "../Builds/Windows";
        private const string ExeName = "OneBagJianghu.exe";

        [MenuItem("One Bag Jianghu/Build/Windows Player")]
        public static void BuildWindows()
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            string output = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputFolder));
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = Path.Combine(output, ExeName),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Windows build failed: {report.summary.result}");

            WriteServerAddress(output);
            Debug.Log($"Windows build written to {output}");
        }

        private static void WriteServerAddress(string output)
        {
            string address = ReadArgument("-server");
            if (string.IsNullOrWhiteSpace(address))
                return;

            string folder = Path.Combine(output, "OneBagJianghu_Data", "StreamingAssets");
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "server.txt"), address.Trim());
            Debug.Log($"Server address set to {address}");
        }

        private static string ReadArgument(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, name);
            if (index < 0 || index + 1 >= args.Length)
                return null;

            return args[index + 1];
        }
    }
}
