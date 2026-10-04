using System.Collections.Generic;

namespace OneBagJianghu.Battle.Replay
{
    public sealed class BattleItemRef
    {
        public int Uid
        {
            get;
            set;
        }
        public int Side
        {
            get;
            set;
        }
        public int Index
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public int? X
        {
            get;
            set;
        }
        public int? Y
        {
            get;
            set;
        }
        public int? W
        {
            get;
            set;
        }
        public int? H
        {
            get;
            set;
        }
        public int? Tier
        {
            get;
            set;
        }
        public string Rarity
        {
            get;
            set;
        }
    }

    public static class BattleLogRarityMapper
    {
        public static void Apply(BattleLog log,
            IReadOnlyList<string> player,
            IReadOnlyList<string> enemy = null)
        {
            if (log == null)
                return;
            foreach (BattleItemRef item in log.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Rarity))
                    continue;
                IReadOnlyList<string> source = item.Side == 0 ? player : enemy;
                int index = item.Uid - item.Side * 100;
                if (source != null && index >= 0 && index < source.Count)
                {
                    item.Rarity = source[index];
                }
            }
        }
    }

    public sealed class BattleLog
    {
        public long Seed
        {
            get;
            set;
        }
        public int Winner
        {
            get;
            set;
        }
        public int DurationMs
        {
            get;
            set;
        }
        public List<string> Names
        {
            get;
            set;
        } = new List<string>();
        public List<int> FinalHp
        {
            get;
            set;
        } = new List<int>();
        public List<int[]> Bags
        {
            get;
            set;
        } = new List<int[]>
        {
            new[]
            {
                5,
                4
            },
            new[]
            {
                5,
                4
            }
        };
        public List<BattleItemRef> Items
        {
            get;
            set;
        } = new List<BattleItemRef>();
        public List<BattleEvent> Events
        {
            get;
            set;
        } = new List<BattleEvent>();
    }
}