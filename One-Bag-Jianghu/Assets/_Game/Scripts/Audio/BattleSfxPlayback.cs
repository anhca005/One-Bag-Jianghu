using System;
using System.Collections.Generic;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Audio
{
    public sealed class BattleSfxPlayback
    {
        private readonly BattleTimeline timeline;
        private readonly BattleSfxResolver resolver;
        private readonly List<BattleEvent> events = new List<BattleEvent>(32);

        public BattleSfxPlayback(BattleTimeline timeline, BattleSfxResolver resolver)
        {
            this.timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public void PlayAdvancedWindow(int previousMs, int currentMs, float speed, Action<string> play)
        {
            if (play == null || currentMs <= previousMs)
                return;
            events.Clear();
            timeline.CollectEvents(previousMs, currentMs, events);
            for (int i = 0; i < events.Count; i++)
            {
                BattleEvent battleEvent = events[i];
                if (speed > 2f && !AllowedAtHighSpeed(battleEvent))
                    continue;
                IReadOnlyList<string> keys = resolver.Resolve(battleEvent);
                for (int keyIndex = 0; keyIndex < keys.Count; keyIndex++)
                {
                    play(keys[keyIndex]);
                }
            }
        }

        private static bool AllowedAtHighSpeed(BattleEvent battleEvent)
        {
            return battleEvent.Type == BattleEventType.Trigger ||
                battleEvent.Type == BattleEventType.End ||
                battleEvent.Type == BattleEventType.Damage && battleEvent.Crit == true;
        }
    }
}
