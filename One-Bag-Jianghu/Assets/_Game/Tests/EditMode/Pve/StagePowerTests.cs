using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;

namespace OneBagJianghu.Tests.Pve
{
    public sealed class StagePowerTests
    {
        [Test]
        public void RecommendedUsesStageValueWhenPositive()
        {
            // Arrange
            PowerCatalog power = CreatePower();
            PveStage stage = CreateStage();
            stage.RecommendedPower = 123;

            // Act
            int recommended = StagePower.Recommended(stage, power);

            // Assert
            Assert.AreEqual(123, recommended);
        }

        [Test]
        public void RecommendedFallsBackWhenStageValueIsZero()
        {
            // Arrange
            PowerCatalog power = CreatePower();
            PveStage stage = CreateStage();

            // Act
            int recommended = StagePower.Recommended(stage, power);

            // Assert
            Assert.AreEqual(50, recommended);
        }

        private static PowerCatalog CreatePower()
        {
            return PowerCatalog.Parse(@"{
                'levelBase': 0,
                'levelPoints': 0,
                'recommendedFactor': 1.1,
                'tierFactor': { '1': 1 },
                'rarityFactor': { 'common': 1 },
                'items': { 'sword': 45 }
            }");
        }

        private static PveStage CreateStage()
        {
            return new PveStage
            {
                Enemy = new PveEnemy
                {
                    Level = 1,
                    Items = new List<PveEnemyItem>
                    {
                        new PveEnemyItem { Id = "sword", Tier = 1, Rarity = "common" }
                    }
                }
            };
        }
    }
}
