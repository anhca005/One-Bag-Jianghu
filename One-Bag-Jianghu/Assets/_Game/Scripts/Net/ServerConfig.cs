using System;
using UnityEngine;

namespace OneBagJianghu.Net
{
    public static class ServerConfig
    {
        public const string DefaultUrl = "ws://localhost:8765/ws";
        public const string PlayerPrefsKey = "OneBagJianghu.ServerUrl";
        public static string GetUrl()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length - 1; i++)
            {
                if (string.Equals(arguments[i],
"-server",
StringComparison.OrdinalIgnoreCase)
&& !string.IsNullOrWhiteSpace(arguments[i
+ 1]))
                {
                    return arguments[i + 1];
                }
            }

            return PlayerPrefs.GetString(PlayerPrefsKey, DefaultUrl);
        }
    }
}