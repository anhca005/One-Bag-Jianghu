namespace OneBagJianghu.Battle.Replay
{
    public sealed class FighterState
    {
        public int Hp
        {
            get;
            set;
        }
        public int MaxHp
        {
            get;
            set;
        }
        public int Shield
        {
            get;
            set;
        }
        public int Qi
        {
            get;
            set;
        }
        public int Burn
        {
            get;
            set;
        }
        public int Poison
        {
            get;
            set;
        }
        public int Bleed
        {
            get;
            set;
        }
        public int Regen
        {
            get;
            set;
        }
        public int StunUntilMs
        {
            get;
            set;
        }

        public FighterState Clone()
        {
            return (FighterState)MemberwiseClone();
        }
    }
}