using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Arena;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ArenaMatchLogicTests
    {
        [Test]
        public void ApplyRewardOnce_DoesNotDuplicateReward()
        {
            // Arrange
            var profile = new PlayerProfile();
            var result = new ArenaResult
            {
                Reward = new ArenaReward { Reputation = 30 }
            };
            var receipt = new ArenaMatchReceipt();

            // Act
            receipt.ApplyRewardOnce(profile, result);
            receipt.ApplyRewardOnce(profile, result);

            // Assert
            Assert.That(profile.Currencies["danh_vong"], Is.EqualTo(30));
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
