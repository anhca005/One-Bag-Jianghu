using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Bag;

namespace OneBagJianghu.Tests.EditMode.Economy
{
    public sealed class LevelServiceTests
    {
        private static LevelService Service() =>
            new LevelService(LevelConfig.Parse(
                "{\"maxLevel\":4,\"expToNext\":[100,200,300],\"unlocks\":{\"10\":\"sect\"}}"));
        [Test]
        public void Grant_CarriesAcrossSeveralLevels()
        {
            var profile = new PlayerProfile();
            LevelGrantResult result = Service().Grant(profile, 350);
            Assert.That(profile.HeroLevel, Is.EqualTo(3));
            Assert.That(profile.HeroExp, Is.EqualTo(50));
            Assert.That(result.OldLevel, Is.EqualTo(1));
        }

        [Test]
        public void Grant_AtCap_DiscardsRemainingExp()
        {
            var profile = new PlayerProfile();
            Service().Grant(profile, 10000);
            Assert.That(profile.HeroLevel, Is.EqualTo(4));
            Assert.That(profile.HeroExp, Is.Zero);
            Service().Grant(profile, 50);
            Assert.That(profile.HeroExp, Is.Zero);
        }

        [TestCase("{\"maxLevel\":3,\"expToNext\":[1]}")]
        [TestCase("{\"maxLevel\":2,\"expToNext\":[0]}")]
        public void Config_RejectsInvalidCurve(string json)
        {
            Assert.Throws<System.FormatException>(() => LevelConfig.Parse(json));
        }

        [Test]
        public void ResourceConfig_IsValid()
        {
            LevelConfig value = LevelConfig.Load();
            Assert.That(value.ExpToNext.Count, Is.EqualTo(value.MaxLevel - 1));
        }

        [Test]
        public void Sect_CannotBeChosenBelowTenOrTwice()
        {
            var profile = new PlayerProfile();
            var service = new SectChoiceService(SectCatalog.LoadFromResources());
            Assert.That(service.TryChoose(profile, "kim_cang", out _), Is.False);
            profile.HeroLevel = 10;
            Assert.That(service.TryChoose(profile, "kim_cang", out _), Is.True);
            Assert.That(profile.TamPhapLevel, Is.EqualTo(1));
            Assert.That(service.TryChoose(profile, "thai_thanh", out _), Is.False);
            Assert.That(profile.SectId, Is.EqualTo("kim_cang"));
        }
    }
}