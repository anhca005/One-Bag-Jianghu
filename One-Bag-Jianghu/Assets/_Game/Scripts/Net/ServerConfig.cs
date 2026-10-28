using System;
using System.IO;
using UnityEngine;

namespace OneBagJianghu.Net
{
    public static class ServerConfig
    {
        public const string DefaultUrl = "ws://localhost:8765/ws";
        public const string PlayerPrefsKey = "OneBagJianghu.ServerUrl";

        public static string GetUrl()
        {
            string savedUrl = PlayerPrefs.HasKey(PlayerPrefsKey)
                ? PlayerPrefs.GetString(PlayerPrefsKey)
                : null;
            return ResolveUrl(
                Environment.GetCommandLineArgs(),
                savedUrl,
                Path.Combine(Application.streamingAssetsPath, "server.txt"));
        }

        public static string ResolveUrl(string[] arguments, string savedUrl, string serverFile)
        {
            for (int i = 0; i < arguments.Length - 1; i++)
            {
                if (string.Equals(arguments[i], "-server", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(arguments[i + 1]))
                {
                    return arguments[i + 1];
                }
            }

            if (!string.IsNullOrWhiteSpace(savedUrl))
            {
                return savedUrl.Trim();
            }

            if (File.Exists(serverFile))
            {
                string fileUrl = File.ReadAllText(serverFile).Trim();
                if (!string.IsNullOrWhiteSpace(fileUrl))
                {
                    return fileUrl;
                }
            }

            return DefaultUrl;
        }
    }
}
