using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OneBagJianghu.Economy
{
    public sealed class LevelConfig
    {
        [JsonProperty("maxLevel")]
        public int MaxLevel;
        [JsonProperty("expToNext")]
        public List<long> ExpToNext;
        [JsonProperty("unlocks")]
        public Dictionary<string, string> Unlocks;
        public static LevelConfig Parse(string json)
        {
            LevelConfig value;
            try
            {
                value = JsonConvert.DeserializeObject<LevelConfig>(json);
            }
            catch (JsonException e)
            {
                throw new FormatException("Invalid levels JSON: " + e.Message, e);
            }

            if (value == null || value.MaxLevel < 1)
                throw new FormatException("levels maxLevel must be positive.");
            if (value.ExpToNext == null || value.ExpToNext.Count != value.MaxLevel - 1)
                throw new FormatException("levels expToNext length must equal maxLevel - 1.");
            for (int i = 0; i < value.ExpToNext.Count; i++)
                if (value.ExpToNext[i] <= 0)
                    throw new FormatException("Every levels expToNext value must be positive.");
            value.Unlocks ??= new Dictionary<string, string>();
            return value;
        }

        public static LevelConfig Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/levels");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/levels.json.");
            return Parse(asset.text);
        }
    }

    public readonly struct LevelGrantResult
    {
        public LevelGrantResult(int oldLevel, int newLevel, long awarded)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
            AwardedExp = awarded;
        }

        public int OldLevel
        {
            get;
        }
        public int NewLevel
        {
            get;
        }
        public long AwardedExp
        {
            get;
        }
        public bool LeveledUp => NewLevel > OldLevel;
    }

    public sealed class LevelService
    {
        public const int SectUnlockLevel = 10;
        private readonly LevelConfig config;
        public LevelService(LevelConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public int MaxLevel => config.MaxLevel;

        public long ExpToNext(int level) => level >= config.MaxLevel ? 0 : config.ExpToNext[level - 1];
        public float Progress(PlayerProfile profile)
            => profile.HeroLevel >= config.MaxLevel
            ? 1f
            : (float)profile.HeroExp / ExpToNext(profile.HeroLevel);
        // EXP is relative to the current level. At max level it is discarded and stored EXP is zero.
        public LevelGrantResult Grant(PlayerProfile profile, long amount)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            int old = profile.HeroLevel;
            if (old >= config.MaxLevel)
            {
                profile.HeroLevel = config.MaxLevel;
                profile.HeroExp = 0;
                return new LevelGrantResult(old, profile.HeroLevel, 0);
            }

            profile.HeroExp = checked(profile.HeroExp + amount);
            while (profile.HeroLevel < config.MaxLevel && profile.HeroExp >= ExpToNext(profile.HeroLevel))
            {
                int before = profile.HeroLevel;
                profile.HeroExp -= ExpToNext(before);
                profile.HeroLevel++;
                Debug.Log($"[Profile] Level up {before} -> {profile.HeroLevel}");
            }

            if (profile.HeroLevel == config.MaxLevel)
                profile.HeroExp = 0;
            return new LevelGrantResult(old, profile.HeroLevel, amount);
        }
    }
}