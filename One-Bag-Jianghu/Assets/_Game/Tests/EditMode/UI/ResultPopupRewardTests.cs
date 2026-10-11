using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Popups;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ResultPopupRewardTests
    {
        [Test]
        public void PveRewardBuildsCurrencyExperienceAndMaterials()
        {
            // Arrange
            MaterialCatalog materials = MaterialCatalog.Parse(
                "[{\"id\":\"han_thiet\",\"name\":\"Hàn Thiết\",\"itemType\":\"weapon\"," +
                "\"band\":\"tier_1\",\"description\":\"Khoáng rèn.\"}]");
            var reward = new PveRewardResult
            {
                Success = true,
                Silver = 45,
                HeroExp = 8,
                OldLevel = 12,
                NewLevel = 13
            };
            reward.Materials["han_thiet"] = 2;

            // Act
            ResultPopupData data = ResultPopupData.FromPveReward("Thắng", "Lữ Khách", reward, materials);

            // Assert
            Assert.That(data.RewardEntries.Count, Is.EqualTo(3));
            Assert.That(data.RewardEntries[0].IconKey, Is.EqualTo("icon_bac"));
            Assert.That(data.RewardEntries[1].IconKey, Is.EqualTo("icon_exp"));
            Assert.That(data.RewardEntries[2].IconKey, Is.EqualTo("han_thiet"));
            Assert.That(data.RewardEntries[2].AmountText, Is.EqualTo("x 2"));
            Assert.That(data.LevelUpText, Is.EqualTo("Lên cấp! Cấp 13"));
        }

        [Test]
        public void PopupCreatesOneCardPerStructuredReward()
        {
            // Arrange
            GameObject prefab = Resources.Load<GameObject>("UI/Popups/ResultPopup");
            GameObject instance = Object.Instantiate(prefab);
            ResultPopup popup = instance.GetComponent<ResultPopup>();
            var data = new ResultPopupData("Thắng", "Lữ Khách");
            data.RewardEntries.Add(new ResultRewardEntry("icon_bac", "45", "Bạc"));
            data.RewardEntries.Add(new ResultRewardEntry("icon_exp", "8 EXP", "Kinh nghiệm"));
            data.RewardEntries.Add(new ResultRewardEntry("han_thiet", "x 2", "Hàn Thiết"));

            // Act
            popup.Setup(data);

            // Assert
            Assert.That(popup.RewardCardCount, Is.EqualTo(data.RewardEntries.Count));
            bool hasStarObjects = instance.GetComponentsInChildren<Transform>(true)
                .Any(child => child.name == "Stars" || child.name.StartsWith("Star "));
            Assert.That(hasStarObjects, Is.False);
            Object.DestroyImmediate(instance);
        }
    }
}
