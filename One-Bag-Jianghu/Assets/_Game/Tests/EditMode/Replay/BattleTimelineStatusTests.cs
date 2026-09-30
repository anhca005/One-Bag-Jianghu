using System.Collections.Generic;
using NUnit.Framework;

namespace OneBagJianghu.Battle.Replay.Tests
{
    public sealed class BattleTimelineStatusTests
    {
        [Test]
        public void StateAt_ReconstructsRegenAfterSeek()
        {
            BattleLog log = new BattleLog
            {
                DurationMs = 5000,
                Names = new List<string> { "A", "B" },
                FinalHp = new List<int> { 100, 100 }
            };
            log.Events.Add(new BattleEvent { TimeMs = 0, Type = BattleEventType.Start, Side = 0, Hp = 100, MaxHp = 100 });
            log.Events.Add(new BattleEvent { TimeMs = 1000, Type = BattleEventType.Status, Side = 0, Kind = "REGEN", Value = 3, Total = 3, Hp = 100, MaxHp = 100 });
            log.Events.Add(new BattleEvent { TimeMs = 2000, Type = BattleEventType.Heal, Side = 0, Value = 3, Hp = 93, MaxHp = 100 });
            BattleTimeline timeline = new BattleTimeline(log);
            Assert.That(timeline.StateAt(0, 2500).Regen, Is.EqualTo(3));
            Assert.That(timeline.StateAt(0, 500).Regen, Is.Zero);
            Assert.That(timeline.StateAt(0, 2500).Regen, Is.EqualTo(3), "Regen must survive backward/forward seeks.");
        }
    }
}
