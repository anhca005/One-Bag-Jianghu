using OneBagJianghu.Bag.View;
using UnityEngine;

namespace OneBagJianghu.Battle.Replay.View
{
    public static class ReplayStyle
    {
        public const float CellSize = 109f;
        public const float CellGap = 3f;
        public const int BodyFont = 20;
        public const int SmallFont = 15;
        public const int VfxBasicDurationMs = 350;
        public const int VfxUltimateDurationMs = 550;
        public const int VfxHitDurationMs = 250;
        public const int VfxBurstDurationMs = 600;
        public const int VfxParryDurationMs = 500;
        public static readonly Vector2 VfxBasicSize = new Vector2(260f, 170f);
        public static readonly Vector2 VfxUltimateSize = new Vector2(420f, 280f);
        public static readonly Vector2 VfxBurstSize = new Vector2(250f, 250f);
        public static readonly Vector2 VfxParrySize = new Vector2(220f, 220f);
        public static readonly Vector2 VfxHitSize = new Vector2(180f, 180f);
        public static readonly Color Paper = BagViewStyle.Paper;
        public static readonly Color Ink = BagViewStyle.Ink;
        public static readonly Color Gold = BagViewStyle.Gold;
        public static readonly Color Panel = Hex("#D8CBB3");
        public static readonly Color Hp = Hex("#A83C35");
        public static readonly Color Shield = Hex("#8796A5");
        public static readonly Color Qi = Hex("#D0A642");
        public static readonly Color Heal = Hex("#438A55");
        public static readonly Color Burn = Hex("#B8322A");
        public static readonly Color Poison = Hex("#6F8F5E");
        public static readonly Color Bleed = Hex("#7A1F1F");

        public static Color Status(string kind) => kind == "BURN" ? Burn : kind == "POISON" ? Poison : Bleed;
        private static Color Hex(string value) { ColorUtility.TryParseHtmlString(value, out Color color); return color; }
    }
}
