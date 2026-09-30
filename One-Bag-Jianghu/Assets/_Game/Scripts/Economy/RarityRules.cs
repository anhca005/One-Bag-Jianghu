using System;
using System.Collections.Generic;

namespace OneBagJianghu.Economy
{
    public static class RarityRules
    {
        public static readonly string[] Ordered =
            { "common", "uncommon", "rare", "epic", "legendary", "supreme" };
        private static readonly int[] Multipliers = { 1000, 1060, 1120, 1190, 1260, 1340 };
        // Mirrors BattleConfig.tierMultiplierPermille (Trọng 1..9).
        private static readonly int[] TierMultipliers = { 1000, 1200, 1400, 1550, 1700, 1800, 1900, 1950, 2000 };
        public static bool IsValid(string rarity) => Array.IndexOf(Ordered, rarity) >= 0;
        public static int Index(string rarity)
        {
            int index = Array.IndexOf(Ordered, rarity);
            if (index < 0) throw new ArgumentException("Unknown rarity '" + rarity + "'.", nameof(rarity));
            return index;
        }
        public static int MultiplierPermille(string rarity) => Multipliers[Index(rarity)];
        public static int TierMultiplierPermille(int tier) => TierMultipliers[Math.Max(1, Math.Min(tier, TierMultipliers.Length)) - 1];
        /// <summary>Combined stat multiplier in permille, same truncation order as the engine (tier, then rarity).</summary>
        public static int PowerPermille(string rarity, int tier) => TierMultiplierPermille(tier) * MultiplierPermille(rarity) / 1000;
        public static string Next(string rarity)
        {
            int index = Index(rarity);
            return index == Ordered.Length - 1 ? null : Ordered[index + 1];
        }
    }

    public interface IUpgradeItemCatalog
    {
        string GetItemType(string itemId);
        IReadOnlyList<int> GetUnlockTiers(string itemId);
    }

    public class EconomyResult
    {
        public bool Success { get; internal set; }
        public string Error { get; internal set; } = string.Empty;
        public static EconomyResult Fail(string error) => new EconomyResult { Error = error };
        public static EconomyResult Ok() => new EconomyResult { Success = true };
    }
}
