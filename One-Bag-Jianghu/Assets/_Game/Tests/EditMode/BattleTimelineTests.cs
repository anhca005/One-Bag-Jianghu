using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using OneBagJianghu.Battle.Replay;
using UnityEngine;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class BattleTimelineTests
    {
        [Test]
        public void CollectEvents_UsesExclusiveStartAndInclusiveEnd()
        {
            BattleTimeline timeline = new BattleTimeline(LoadSample());
            List<BattleEvent> buffer = new List<BattleEvent>();
            int count = timeline.CollectEvents(0, 1200, buffer);
            Assert.That(count, Is.EqualTo(6));
            Assert.That(buffer, Has.Count.EqualTo(6));
            for (int i = 0; i < buffer.Count; i++)
            {
                Assert.That(buffer[i].TimeMs, Is.EqualTo(1200));
            }

            buffer.Clear();
            Assert.That(timeline.CollectEvents(1200, 1200, buffer), Is.Zero);
            Assert.That(buffer, Is.Empty);
        }

        [TestCase(0, 0, 1200, 1200, 600, 0)]
        [TestCase(1, 0, 1200, 1200, 600, 0)]
        [TestCase(0, 15000, 824, 1200, 382, 12)]
        [TestCase(1, 15000, 685, 1200, 0, 46)]
        [TestCase(0, 30000, -32, 1200, 120, 20)]
        [TestCase(1, 30000, 1, 1200, 0, 34)]
        public void StateAt_MatchesLastSampleValues(int side, int timeMs, int hp, int maxHp, int shield, int qi)
        {
            BattleTimeline timeline = new BattleTimeline(LoadSample());
            FighterState state = timeline.StateAt(side, timeMs);
            Assert.That(state.Hp, Is.EqualTo(hp));
            Assert.That(state.MaxHp, Is.EqualTo(maxHp));
            Assert.That(state.Shield, Is.EqualTo(shield));
            Assert.That(state.Qi, Is.EqualTo(qi));
        }

        [Test]
        public void StateAt_AppliesMeterResetAfterBurst()
        {
            BattleTimeline timeline = new BattleTimeline(LoadSample());
            FighterState before = timeline.StateAt(0, 29199);
            FighterState after = timeline.StateAt(0, 29200);
            Assert.That(before.Bleed, Is.EqualTo(45));
            Assert.That(after.Bleed, Is.Zero);
        }

        [Test]
        public void Constructor_StableSortsEqualTimestampEvents()
        {
            BattleLog log = new BattleLog
            {
                DurationMs = 10
            };
            log.Events.Add(new BattleEvent { TimeMs = 10, RawType = "first" });
            log.Events.Add(new BattleEvent { TimeMs = 5, RawType = "earlier" });
            log.Events.Add(new BattleEvent { TimeMs = 10, RawType = "second" });
            BattleTimeline timeline = new BattleTimeline(log);
            Assert.That(timeline.Events[0].RawType, Is.EqualTo("earlier"));
            Assert.That(timeline.Events[1].RawType, Is.EqualTo("first"));
            Assert.That(timeline.Events[2].RawType, Is.EqualTo("second"));
        }

        private static BattleLog LoadSample()
        {
            string path =
Path.Combine(Application.dataPath,
"_Game/Tests/EditMode/Data/sample_battle.json");
            return BattleLogParser.Parse(File.ReadAllText(path));
        }
    }
}