using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Popups;

namespace OneBagJianghu.Tests.EditMode
{
    public sealed class SectPopupTests
    {
        [Test]
        public void EveryMilestoneHasLocalizedDescription()
        {
            // Arrange
            SectCatalog catalog = SectCatalog.LoadFromResources();
            string[] rawIds =
            {
                "fist", "staff", "sword", "fan", "blade", "palm", "dual", "hidden", "claw",
                "flute", "spear", "brush", "bow", "axe", "hammer", "whip", "zither"
            };

            // Act and assert
            foreach (SectInfo sect in catalog.All)
            {
                foreach (SectLevelInfo milestone in sect.Levels)
                {
                    string description = milestone.Describe();
                    Assert.That(description, Is.Not.Empty, sect.Id + " tầng " + milestone.Level);
                    foreach (string rawId in rawIds)
                    {
                        Assert.That(description, Does.Not.Contain(rawId), sect.Id + " tầng " + milestone.Level);
                    }
                }
            }
        }

        [Test]
        public void ConfirmedPopupJoinOnlyValidatesSectChoice()
        {
            // Arrange
            SectCatalog catalog = SectCatalog.LoadFromResources();
            var profile = new PlayerProfile
            {
                HeroLevel = LevelService.SectUnlockLevel
            };

            // Act
            bool joined = SectSelectPopup.TryJoin(profile, catalog, "han_son", out string error);

            // Assert
            Assert.That(joined, Is.True, error);
            Assert.That(profile.SectId, Is.Null);
            Assert.That(profile.TamPhapLevel, Is.EqualTo(1), "TamPhapLevel keeps its default until the server answers");
        }

        [Test]
        public void OwnSectViewKeepsUpgradeButtonDisabled()
        {
            // Arrange
            SectCatalog catalog = SectCatalog.LoadFromResources();
            Assert.That(catalog.TryGet("han_son", out SectInfo ownSect), Is.True);
            var profile = new PlayerProfile
            {
                HeroLevel = LevelService.SectUnlockLevel,
                SectId = ownSect.Id,
                TamPhapLevel = 3
            };

            // Act
            bool enabled = SectSelectPopup.IsPrimaryActionEnabled(profile, ownSect);

            // Assert
            Assert.That(enabled, Is.False);
        }
    }
}
