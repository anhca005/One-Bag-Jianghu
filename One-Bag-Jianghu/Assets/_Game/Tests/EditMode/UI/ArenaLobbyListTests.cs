using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Arena;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ArenaLobbyListTests
    {
        [Test]
        public void BindLeaderboardCreatesThreeActiveRows()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_Game/Resources/UI/Popups/ArenaLobbyPopup.prefab");
            GameObject instance = Object.Instantiate(prefab);
            ArenaLobbyPopup popup = instance.GetComponent<ArenaLobbyPopup>();
            FieldInfo contentField = typeof(ArenaLobbyPopup).GetField("leaderboardContent",
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo bind = typeof(ArenaLobbyPopup).GetMethod("BindLeaderboard",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var result = new LeaderboardResult
            {
                Entries = new List<LeaderboardEntry>
                {
                    Entry(1, 1500),
                    Entry(2, 1400),
                    Entry(3, 1300)
                }
            };

            bind.Invoke(popup, new object[] { result });

            var content = (RectTransform)contentField.GetValue(popup);
            Assert.That(content.childCount, Is.EqualTo(3));
            for (int i = 0; i < content.childCount; i++)
            {
                Assert.That(content.GetChild(i).gameObject.activeSelf, Is.True);
            }

            Object.DestroyImmediate(instance);
        }

        private static LeaderboardEntry Entry(int rank, int rating)
        {
            return new LeaderboardEntry
            {
                Rank = rank,
                Rating = rating,
                HeroName = "Hiệp Khách " + rank,
                Wins = rank,
                Losses = 0
            };
        }
    }
}
