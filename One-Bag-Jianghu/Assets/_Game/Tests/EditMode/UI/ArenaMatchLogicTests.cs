using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Arena;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ArenaMatchLogicTests
    {
        [Test]
        public void StoreBattle_DoesNotApplyRewardOnClient()
        {
            // Arrange
            var profile = new PlayerProfile();
            var result = new ArenaResult
            {
                Reward = new ArenaReward { Reputation = 30 }
            };
            var root = new UnityEngine.GameObject("Session");
            GameSession session = root.AddComponent<GameSession>();

            // Act
            session.ApplyServerProfile(Newtonsoft.Json.Linq.JObject.Parse(profile.ToJson()), 1);
            ArenaMatchLogic.StoreBattle(session, result);

            // Assert
            session.Profile.Currencies.TryGetValue("danh_vong", out long reputation);
            Assert.That(reputation, Is.EqualTo(0));
            Assert.That(session.PendingArenaResult, Is.SameAs(result));
            UnityEngine.Object.DestroyImmediate(root);
        }

        [Test]
        public void ReplayModel_DoesNotContainReward()
        {
            // Arrange
            var replay = new ArenaReplayResult();

            // Act
            var rewardProperty = replay.GetType().GetProperty("Reward");

            // Assert
            Assert.That(rewardProperty, Is.Null);
        }
    }
}
