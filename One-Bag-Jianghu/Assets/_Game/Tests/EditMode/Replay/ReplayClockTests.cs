using NUnit.Framework;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class ReplayClockTests
    {
        [Test]
        public void Advance_UsesSpeedAndStopsAtDuration()
        {
            var clock = new ReplayClock(1000);
            clock.SetSpeed(2f);
            clock.Play();
            Assert.That(clock.Advance(.2f), Is.Zero);
            Assert.That(clock.TimeMs, Is.EqualTo(400));
            clock.Advance(.5f);
            Assert.That(clock.TimeMs, Is.EqualTo(1000));
            Assert.That(clock.IsPlaying, Is.False);
        }

        [Test]
        public void Seek_ClampsAndPausePreventsAdvance()
        {
            var clock = new ReplayClock(1000);
            clock.Seek(-10);
            Assert.That(clock.TimeMs, Is.Zero);
            clock.Play();
            clock.Pause();
            clock.Advance(1f);
            Assert.That(clock.TimeMs, Is.Zero);
            clock.Seek(1200);
            Assert.That(clock.TimeMs, Is.EqualTo(1000));
        }
    }
}