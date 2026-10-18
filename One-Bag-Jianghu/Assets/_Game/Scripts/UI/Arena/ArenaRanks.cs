using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OneBagJianghu.UI.Arena
{
    public sealed class ArenaRankDefinition
    {
        [JsonProperty("minimumRating")]
        public int MinimumRating
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("imageKey")]
        public string ImageKey
        {
            get;
            set;
        }
    }

    public sealed class ArenaRanks
    {
        [JsonProperty("ranks")]
        public List<ArenaRankDefinition> Ranks
        {
            get;
            set;
        } = new List<ArenaRankDefinition>();

        public static ArenaRanks Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/arena");
            if (asset == null)
            {
                throw new InvalidOperationException("Could not load Resources/Data/arena.json.");
            }

            return Parse(asset.text);
        }

        public static ArenaRanks Parse(string json)
        {
            ArenaRanks value = JsonConvert.DeserializeObject<ArenaRanks>(json);
            if (value == null || value.Ranks.Count == 0)
            {
                throw new FormatException("Arena rank data is empty.");
            }

            value.Ranks.Sort((left, right) => left.MinimumRating.CompareTo(right.MinimumRating));
            return value;
        }

        public ArenaRankDefinition ForRating(int rating)
        {
            ArenaRankDefinition result = Ranks[0];
            foreach (ArenaRankDefinition rank in Ranks)
            {
                if (rating < rank.MinimumRating)
                {
                    break;
                }

                result = rank;
            }

            return result;
        }
    }
}


