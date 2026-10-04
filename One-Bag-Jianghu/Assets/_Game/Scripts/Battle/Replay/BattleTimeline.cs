using System;
using System.Collections.Generic;

namespace OneBagJianghu.Battle.Replay
{
    public sealed class BattleTimeline
    {
        private const int CheckpointInterval = 64;
        private readonly List<BattleEvent> _events;
        private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
        public BattleTimeline(BattleLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (log.Events == null)
                throw new ArgumentException("Battle log events cannot be null.", nameof(log));
            DurationMs = log.DurationMs;
            List<IndexedEvent> indexed = new List<IndexedEvent>(log.Events.Count);
            for (int i = 0; i < log.Events.Count; i++)
            {
                indexed.Add(new IndexedEvent(log.Events[i], i));
            }

            indexed.Sort(CompareIndexedEvents);
            _events = new List<BattleEvent>(indexed.Count);
            for (int i = 0; i < indexed.Count; i++)
            {
                _events.Add(indexed[i].Event);
            }

            BuildCheckpoints();
        }

        public int DurationMs { get; }
        public IReadOnlyList<BattleEvent> Events => _events;

        public int CollectEvents(int fromMsExclusive,
            int toMsInclusive,
            List<BattleEvent> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (toMsInclusive <= fromMsExclusive)
                return 0;
            int start = UpperBound(fromMsExclusive);
            int count = 0;
            for (int i = start; i < _events.Count && _events[i].TimeMs <= toMsInclusive; i++)
            {
                buffer.Add(_events[i]);
                count++;
            }

            return count;
        }

        public FighterState StateAt(int side, int timeMs)
        {
            if (side != 0 && side != 1)
                throw new ArgumentOutOfRangeException(nameof(side), "Side must be 0 or 1.");
            int checkpointIndex = FindCheckpoint(timeMs);
            Checkpoint checkpoint = _checkpoints[checkpointIndex];
            FighterState state = checkpoint.States[side].Clone();
            for (int i = checkpoint.NextEventIndex; i < _events.Count && _events[i].TimeMs <= timeMs; i++)
            {
                BattleEvent battleEvent = _events[i];
                if (battleEvent.Side == side)
                {
                    Apply(state, battleEvent);
                }
            }

            return state;
        }

        private void BuildCheckpoints()
        {
            FighterState[] states =
            {
                new FighterState(),
                new FighterState()
            };
            _checkpoints.Add(new Checkpoint(int.MinValue, 0, states));
            int sinceCheckpoint = 0;
            int index = 0;
            while (index < _events.Count)
            {
                int time = _events[index].TimeMs;
                do
                {
                    BattleEvent battleEvent = _events[index];
                    if (battleEvent.Side == 0 || battleEvent.Side == 1)
                    {
                        Apply(states[battleEvent.Side], battleEvent);
                    }

                    index++;
                    sinceCheckpoint++;
                }
                while (index < _events.Count && _events[index].TimeMs == time);
                if (sinceCheckpoint >= CheckpointInterval || index == _events.Count)
                {
                    _checkpoints.Add(new Checkpoint(time, index, states));
                    sinceCheckpoint = 0;
                }
            }
        }

        private static void Apply(FighterState state, BattleEvent battleEvent)
        {
            if (battleEvent.Hp.HasValue)
            {
                state.Hp = battleEvent.Hp.Value;
            }

            if (battleEvent.MaxHp.HasValue)
            {
                state.MaxHp = battleEvent.MaxHp.Value;
            }

            if (battleEvent.Shield.HasValue)
            {
                state.Shield = battleEvent.Shield.Value;
            }

            if (battleEvent.Qi.HasValue)
            {
                state.Qi = battleEvent.Qi.Value;
            }

            if (battleEvent.Type != BattleEventType.Status || string.IsNullOrEmpty(battleEvent.Kind))
                return;
            if (battleEvent.Total.HasValue)
            {
                switch (battleEvent.Kind)
                {
                    case "BURN":
                        state.Burn = battleEvent.Total.Value;
                        break;
                    case "POISON":
                        state.Poison = battleEvent.Total.Value;
                        break;
                    case "BLEED":
                        state.Bleed = battleEvent.Total.Value;
                        break;
                    case "REGEN":
                        state.Regen = battleEvent.Total.Value;
                        break;
                }
            }

            if (battleEvent.Kind == "STUN" && battleEvent.Value.HasValue)
            {
                state.StunUntilMs =
                    Math.Max(state.StunUntilMs,
                    battleEvent.TimeMs +
                    battleEvent.Value.Value);
            }
        }

        private int UpperBound(int timeMs)
        {
            int low = 0;
            int high = _events.Count;
            while (low < high)
            {
                int middle = low + ((high - low) / 2);
                if (_events[middle].TimeMs <= timeMs)
                {
                    low = middle + 1;
                }
                else
                    high = middle;
            }

            return low;
        }

        private int FindCheckpoint(int timeMs)
        {
            int low = 0;
            int high = _checkpoints.Count;
            while (low < high)
            {
                int middle = low + ((high - low) / 2);
                if (_checkpoints[middle].TimeMs <= timeMs)
                {
                    low = middle + 1;
                }
                else
                    high = middle;
            }

            return low - 1;
        }

        private static int CompareIndexedEvents(IndexedEvent left, IndexedEvent right)
        {
            int timeComparison = left.Event.TimeMs.CompareTo(right.Event.TimeMs);
            return timeComparison != 0 ?
                timeComparison :
                left.SourceIndex.CompareTo(right.SourceIndex);
        }

        private readonly struct IndexedEvent
        {
            public IndexedEvent(BattleEvent battleEvent, int sourceIndex)
            {
                Event = battleEvent;
                SourceIndex = sourceIndex;
            }

            public BattleEvent Event { get; }
            public int SourceIndex { get; }
        }

        private sealed class Checkpoint
        {
            public Checkpoint(int timeMs, int nextEventIndex, FighterState[] states)
            {
                TimeMs = timeMs;
                NextEventIndex = nextEventIndex;
                States = new[]
                {
                    states[0].Clone(),
                    states[1].Clone()
                };
            }

            public int TimeMs { get; }
            public int NextEventIndex { get; }
            public FighterState[] States { get; }
        }
    }
}