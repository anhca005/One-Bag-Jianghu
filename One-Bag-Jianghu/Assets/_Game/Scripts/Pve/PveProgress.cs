using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    public sealed class StaminaConfig
    {
        [JsonProperty("max")]
        public int Max;
        [JsonProperty("regenPerMinute")]
        public float RegenPerMinute;
        public static StaminaConfig Load()
        {
            TextAsset a = Resources.Load<TextAsset>("Data/Pve/stamina");
            if (a == null)
                throw new InvalidOperationException("Missing PvE stamina config.");
            return JsonConvert.DeserializeObject<StaminaConfig>(a.text);
        }

        public void Validate()
        {
            if (Max <= 0 || RegenPerMinute <= 0)
                throw new FormatException("Stamina max and regenPerMinute must be positive.");
        }
    }

    public static class PveRules
    {
        public static bool IsChapterUnlocked(PveChapter chapter, PveCatalog catalog, PlayerProfile profile)
        {
            if (string.IsNullOrEmpty(chapter.Unlock?.AfterChapter))
                return true;
            PveChapter prerequisite = catalog.Chapters.Find(c => c.Id == chapter.Unlock.AfterChapter);
            return prerequisite != null
                && prerequisite.Stages.Count > 0
                && IsCleared(profile,
                prerequisite.Stages[prerequisite.Stages.Count - 1].Id);
        }

        public static bool IsStageUnlocked(PveStage stage, PveCatalog catalog, PlayerProfile profile)
        {
            if (!IsChapterUnlocked(stage.Chapter, catalog, profile))
                return false;
            int i = stage.Chapter.Stages.IndexOf(stage);
            return i <= 0 || IsCleared(profile, stage.Chapter.Stages[i - 1].Id);
        }

        public static bool IsCleared(PlayerProfile profile,
            string id) => profile.Pve.Stages.TryGetValue(id,
            out PveStageProgress p)
            && p.Cleared;
    }

    public sealed class StaminaService
    {
        private readonly StaminaConfig config;
        public StaminaService(StaminaConfig config)
        {
            this.config = config;
            config.Validate();
        }

        public int Refresh(PveProfileState state, DateTimeOffset now)
        {
            if (state.Stamina < 0)
                state.Stamina = config.Max;
            if (!DateTimeOffset.TryParse(state.LastStaminaRegenUtc, out DateTimeOffset last))
                last = now;
            if (state.Stamina >= config.Max)
            {
                state.Stamina = config.Max;
                state.LastStaminaRegenUtc = now.ToString("O");
                return state.Stamina;
            }

            double perPoint = 1d / config.RegenPerMinute;
            int gained = (int)((now - last).TotalMinutes / perPoint);
            if (gained > 0)
            {
                state.Stamina = Math.Min(config.Max, state.Stamina + gained);
                state.LastStaminaRegenUtc = last.AddMinutes(gained * perPoint).ToString("O");
            }
            else if (string.IsNullOrEmpty(state.LastStaminaRegenUtc))
                state.LastStaminaRegenUtc = last.ToString("O");
            return state.Stamina;
        }

        public bool TrySpend(PveProfileState state, int amount, DateTimeOffset now)
        {
            Refresh(state, now);
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (state.Stamina < amount)
                return false;
            // Refresh already moved the regen clock to now when stamina was full; otherwise keep the partial
            // progress toward the next point instead of restarting it.
            state.Stamina -= amount;
            return true;
        }

        public double SecondsToNext(PveProfileState state, DateTimeOffset now)
        {
            double perPoint = SecondsPerPoint();
            double elapsed = ElapsedSeconds(state, now);
            int pending = (int)(elapsed / perPoint);
            if (state.Stamina + pending >= config.Max)
                return 0d;
            return perPoint - elapsed % perPoint;
        }

        public double SecondsToFull(PveProfileState state, DateTimeOffset now)
        {
            if (state.Stamina >= config.Max)
                return 0d;
            double remaining = (config.Max - state.Stamina) * SecondsPerPoint();
            return Math.Max(0d, remaining - ElapsedSeconds(state, now));
        }

        public static string FormatDuration(double seconds)
        {
            int totalSeconds = Math.Max(0, (int)Math.Ceiling(seconds));
            TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);
            return totalSeconds >= 3600
                ? $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}"
                : $"{duration.Minutes:00}:{duration.Seconds:00}";
        }

        public void Refill(PveProfileState state, DateTimeOffset now)
        {
            state.Stamina = config.Max;
            state.LastStaminaRegenUtc = now.ToString("O");
        }

        private double SecondsPerPoint() => 60d / config.RegenPerMinute;

        private static double ElapsedSeconds(PveProfileState state, DateTimeOffset now)
        {
            if (!DateTimeOffset.TryParse(state.LastStaminaRegenUtc, out DateTimeOffset last))
                return 0d;
            return Math.Max(0d, (now - last).TotalSeconds);
        }
    }
}
