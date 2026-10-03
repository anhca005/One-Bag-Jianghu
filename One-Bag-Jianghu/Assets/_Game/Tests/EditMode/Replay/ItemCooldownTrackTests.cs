using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class ItemCooldownTrackTests
    {
        [TestCase(0, 1f)]
        [TestCase(500, .5f)]
        [TestCase(1000, 1f)]
        [TestCase(2000, .5f)]
        [TestCase(4500, .5f)]
        [TestCase(6000, 1f)]
        [TestCase(7000, .5f)]
        [TestCase(8000, 0f)]
        public void FillAt_UsesRealCyclesThenNominalCooldown(int timeMs, float expected)
        {
            ItemCooldownTrack track =
Track(2000,
Trigger(7,
1000),
Trigger(7,
3000),
Trigger(7,
6000));
            Assert.That(track.FillAt(timeMs), Is.EqualTo(expected).Within(.0001f));
        }

        [Test]
        public void FillAt_IsIndependentOfSeekDirection()
        {
            ItemCooldownTrack track =
Track(2000,
Trigger(7,
1000),
Trigger(7,
3000),
Trigger(7,
6000));
            float forward = track.FillAt(4500);
            track.FillAt(6500);
            Assert.That(track.FillAt(4500), Is.EqualTo(forward));
            Assert.That(track.FillAt(500), Is.EqualTo(.5f));
        }

        [Test]
        public void PassiveAndOtherItemTriggers_DoNotResetCycle()
        {
            ItemCooldownTrack track =
Track(2000,
Trigger(7,
1000),
Trigger(7,
1500,
"ON_HIT"),
Trigger(8,
2000),
Trigger(7,
3000));
            Assert.That(track.FillAt(2000), Is.EqualTo(.5f).Within(.0001f));
        }

        [Test]
        public void NoActivations_HasZeroFillAndIsInactive()
        {
            ItemCooldownTrack track = Track(2000, Trigger(8, 1000), Trigger(7, 1500, "ON_HIT"));
            Assert.That(track.HasActivations, Is.False);
            Assert.That(track.FillAt(0), Is.Zero);
            Assert.That(track.FillAt(5000), Is.Zero);
        }

        [Test]
        public void MissingNominalCooldown_UsesLastMeasuredCycle()
        {
            ItemCooldownTrack track = Track(0, Trigger(7, 1000), Trigger(7, 3000), Trigger(7, 6000));
            Assert.That(track.FillAt(7500), Is.EqualTo(.5f).Within(.0001f));
        }

        [Test]
        public void ActiveRelic_WithCooldownUsesTheSameTriggerRule()
        {
            ItemInfo relic =
new ItemInfo("relic",
"Relic",
"relic",
null,
null,
"common",
1,
1,
4f,
true);
            ItemCooldownTrack track =
new ItemCooldownTrack(7,
relic,
new List<BattleEvent> { Trigger(7, 1000) });
            Assert.That(track.HasActivations, Is.True);
            Assert.That(track.FillAt(3000), Is.EqualTo(.5f).Within(.0001f));
        }

        private static ItemCooldownTrack Track(int cooldownMs, params BattleEvent[] events)
        {
            ItemInfo item =
new ItemInfo("weapon",
"Weapon",
"weapon",
"sword",
null,
"common",
1,
1,
cooldownMs / 1000f);
            return new ItemCooldownTrack(7, item, new List<BattleEvent>(events));
        }

        private static BattleEvent Trigger(int uid, int timeMs, string kind = "BASIC")
        {
            return new BattleEvent
            {
                TimeMs = timeMs,
                Type = BattleEventType.Trigger,
                Item = uid,
                Kind = kind
            };
        }
    }
}