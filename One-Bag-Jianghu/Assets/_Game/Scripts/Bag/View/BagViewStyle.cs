using UnityEngine;

namespace OneBagJianghu.Bag.View
{
    public static class BagViewStyle
    {
        public const float CellSize = 96f;
        public const float Gap = 4f;
        public const float Pitch = CellSize + Gap;
        public static readonly Color Paper = Hex("#EFE6D2");
        public static readonly Color Ink = Hex("#1E1B18");
        public static readonly Color Gold = Hex("#C9A45C");
        public static readonly Color Valid = new Color(0.20f, 0.65f, 0.30f, 0.42f);
        public static readonly Color Invalid = new Color(0.78f, 0.18f, 0.16f, 0.42f);

        public static Color Rarity(string rarity)
        {
            return rarity switch
            {
                "uncommon" => Hex("#6F8F5E"),
                "rare" => Hex("#4F7A99"),
                "epic" => Hex("#7C5A8E"),
                "legendary" => Hex("#C9A045"),
                "supreme" => Hex("#B8322A"),
                _ => Hex("#9A958C")
            };
        }

        public static Color Fill(string rarity)
        {
            Color color = Rarity(rarity);
            color.a = 0.35f;
            return color;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out Color color);
            return color;
        }
    }
}
