namespace OneBagJianghu.Battle.Replay
{
    public sealed class BattleEvent
    {
        public int TimeMs
        {
            get;
            set;
        }
        public BattleEventType Type
        {
            get;
            set;
        }
        public string RawType
        {
            get;
            set;
        }
        public int Side
        {
            get;
            set;
        }
        public int? Item
        {
            get;
            set;
        }
        public string Kind
        {
            get;
            set;
        }
        public int? Value
        {
            get;
            set;
        }
        public int? Absorbed
        {
            get;
            set;
        }
        public int? Total
        {
            get;
            set;
        }
        public bool? Crit
        {
            get;
            set;
        }
        public int? Hp
        {
            get;
            set;
        }
        public int? MaxHp
        {
            get;
            set;
        }
        public int? Shield
        {
            get;
            set;
        }
        public int? Qi
        {
            get;
            set;
        }
    }
}