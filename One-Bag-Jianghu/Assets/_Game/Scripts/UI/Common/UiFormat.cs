using System.Globalization;

namespace OneBagJianghu.UI.Common
{
    public static class UiFormat
    {
        private static readonly NumberFormatInfo VietnameseNumbers = new NumberFormatInfo
        {
            NumberGroupSeparator = ".",
            NumberDecimalDigits = 0
        };

        /// <summary>Formats a whole number for player-visible Vietnamese UI.</summary>
        public static string Number(long value)
        {
            return value.ToString("N0", VietnameseNumbers);
        }
    }
}
