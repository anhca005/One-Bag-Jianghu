namespace OneBagJianghu.Audio
{
    public static class MusicKeys
    {
        public const string Home = "bgm_home";
        public const string Battle = "bgm_battle";
        public const string Boss = "bgm_boss";
        public const string Pvp = "bgm_pvp";
        public const string Intro = "bgm_intro";
        public const string Victory = "sting_victory";
        public const string Defeat = "sting_defeat";

        public static string ForChapter(int chapter)
        {
            int safeChapter = chapter >= 1 && chapter <= 5 ? chapter : 1;
            return "bgm_map_hoi_" + safeChapter;
        }

        public static string ForStage(bool bossStage)
        {
            return bossStage ? Boss : Battle;
        }

        public static string ForResult(bool victory)
        {
            return victory ? Victory : Defeat;
        }
    }
}
