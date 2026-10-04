using NUnit.Framework;
using OneBagJianghu.Core;

namespace OneBagJianghu.Tests.Core
{
    public sealed class GameSettingsTests
    {
        [Test]
        public void LoadUsesDefaultsForEmptyStore()
        {
            // Arrange and Act
            GameSettings settings = GameSettings.Load(new MemorySettingsStore());

            // Assert
            Assert.That(settings.MasterVolume, Is.EqualTo(1f));
            Assert.That(settings.MusicVolume, Is.EqualTo(.8f));
            Assert.That(settings.SfxVolume, Is.EqualTo(1f));
            Assert.That(settings.Muted, Is.False);
            Assert.That(settings.Fullscreen, Is.True);
            Assert.That(settings.ResolutionWidth, Is.Zero);
            Assert.That(settings.ResolutionHeight, Is.Zero);
            Assert.That(settings.VSync, Is.True);
            Assert.That(settings.FrameRateLimit, Is.EqualTo(60));
        }

        [Test]
        public void VolumesAreClamped()
        {
            // Arrange
            GameSettings settings = GameSettings.Load(new MemorySettingsStore());

            // Act
            settings.MasterVolume = -2f;
            settings.MusicVolume = 2f;
            settings.SfxVolume = .4f;

            // Assert
            Assert.That(settings.MasterVolume, Is.Zero);
            Assert.That(settings.MusicVolume, Is.EqualTo(1f));
            Assert.That(settings.SfxVolume, Is.EqualTo(.4f));
        }

        [TestCase(30, 30)]
        [TestCase(60, 60)]
        [TestCase(120, 120)]
        [TestCase(-1, -1)]
        [TestCase(75, 60)]
        public void FrameRateAcceptsOnlySupportedValues(int value, int expected)
        {
            // Arrange
            GameSettings settings = GameSettings.Load(new MemorySettingsStore());

            // Act
            settings.FrameRateLimit = value;

            // Assert
            Assert.That(settings.FrameRateLimit, Is.EqualTo(expected));
        }

        [Test]
        public void SaveThenLoadRoundTripsAllValues()
        {
            // Arrange
            var store = new MemorySettingsStore();
            GameSettings saved = GameSettings.Load(store);
            saved.MasterVolume = .6f;
            saved.MusicVolume = .4f;
            saved.SfxVolume = .2f;
            saved.Muted = true;
            saved.Fullscreen = false;
            saved.ResolutionWidth = 1920;
            saved.ResolutionHeight = 1080;
            saved.VSync = false;
            saved.FrameRateLimit = 120;

            // Act
            saved.Save();
            GameSettings loaded = GameSettings.Load(store);

            // Assert
            Assert.That(loaded.MasterVolume, Is.EqualTo(.6f));
            Assert.That(loaded.MusicVolume, Is.EqualTo(.4f));
            Assert.That(loaded.SfxVolume, Is.EqualTo(.2f));
            Assert.That(loaded.Muted, Is.True);
            Assert.That(loaded.Fullscreen, Is.False);
            Assert.That(loaded.ResolutionWidth, Is.EqualTo(1920));
            Assert.That(loaded.ResolutionHeight, Is.EqualTo(1080));
            Assert.That(loaded.VSync, Is.False);
            Assert.That(loaded.FrameRateLimit, Is.EqualTo(120));
        }

        [Test]
        public void ResetToDefaultsRestoresEveryValue()
        {
            // Arrange
            GameSettings settings = GameSettings.Load(new MemorySettingsStore());
            settings.MasterVolume = .2f;
            settings.Muted = true;
            settings.Fullscreen = false;
            settings.ResolutionWidth = 1280;
            settings.ResolutionHeight = 720;
            settings.VSync = false;
            settings.FrameRateLimit = -1;

            // Act
            settings.ResetToDefaults();

            // Assert
            Assert.That(settings.MasterVolume, Is.EqualTo(1f));
            Assert.That(settings.MusicVolume, Is.EqualTo(.8f));
            Assert.That(settings.SfxVolume, Is.EqualTo(1f));
            Assert.That(settings.Muted, Is.False);
            Assert.That(settings.Fullscreen, Is.True);
            Assert.That(settings.ResolutionWidth, Is.Zero);
            Assert.That(settings.ResolutionHeight, Is.Zero);
            Assert.That(settings.VSync, Is.True);
            Assert.That(settings.FrameRateLimit, Is.EqualTo(60));
        }

        [Test]
        public void EffectiveVolumesRespectMuteAndMixLevels()
        {
            // Arrange
            GameSettings settings = GameSettings.Load(new MemorySettingsStore());
            settings.MasterVolume = .5f;
            settings.MusicVolume = .8f;
            settings.SfxVolume = .6f;

            // Act and Assert
            Assert.That(settings.EffectiveMusicVolume, Is.EqualTo(.4f).Within(.001f));
            Assert.That(settings.EffectiveSfxVolume, Is.EqualTo(.3f).Within(.001f));
            settings.Muted = true;
            Assert.That(settings.EffectiveMusicVolume, Is.Zero);
            Assert.That(settings.EffectiveSfxVolume, Is.Zero);
        }
    }
}
