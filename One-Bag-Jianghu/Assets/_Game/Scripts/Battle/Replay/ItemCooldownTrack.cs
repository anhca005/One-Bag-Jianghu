using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;

namespace OneBagJianghu.Battle.Replay
{
    public sealed class ItemCooldownTrack
    {
        private readonly int[] activations;
        private readonly int nominalCooldownMs;
        public ItemCooldownTrack(int uid, ItemInfo item, IReadOnlyList<BattleEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            List<int> times = new List<int>();
            for (int i = 0; i < events.Count; i++)
            {
                BattleEvent battleEvent = events[i];
                if (battleEvent.Item == uid && VfxResolver.IsItemAttackTrigger(battleEvent, item))
                {
                    times.Add(battleEvent.TimeMs);
                }
            }

            activations = times.ToArray();
            nominalCooldownMs =
                item == null ?
                0 :
                Math.Max(0,
                (int)Math.Round(item.CooldownSeconds * 1000f));
        }

        public bool HasActivations => activations.Length > 0;

        public float FillAt(int timeMs)
        {
            if (activations.Length == 0)
                return 0f;
            int next = UpperBound(timeMs);
            int start;
            int end;
            if (next == 0)
            {
                start = 0;
                end = activations[0];
            }
            else if (next < activations.Length)
            {
                start = activations[next - 1];
                end = activations[next];
            }
            else
            {
                start = activations[activations.Length - 1];
                int duration = nominalCooldownMs;
                if (duration <= 0 && activations.Length > 1)
                {
                    duration = activations[activations.Length - 1] - activations[activations.Length - 2];
                }

                if (duration <= 0)
                {
                    duration = activations[0];
                }

                if (duration <= 0)
                    return 0f;
                end = start + duration;
            }

            int cycle = end - start;
            if (cycle <= 0)
                return timeMs <= start ? 1f : 0f;
            float fill = (float)(end - timeMs) / cycle;
            return fill < 0f ? 0f : fill > 1f ? 1f : fill;
        }

        private int UpperBound(int timeMs)
        {
            int low = 0;
            int high = activations.Length;
            while (low < high)
            {
                int middle = low + ((high - low) / 2);
                if (activations[middle] <= timeMs)
                {
                    low = middle + 1;
                }
                else
                    high = middle;
            }

            return low;
        }
    }
}