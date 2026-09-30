using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OneBagJianghu.Battle.Replay
{
    public static class BattleLogParser
    {
        public static BattleLog Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("Battle log JSON is empty.");
            }

            try
            {
                RawBattleLog raw = JsonConvert.DeserializeObject<RawBattleLog>(json);
                if (raw == null)
                {
                    throw new FormatException("Battle log JSON did not contain an object.");
                }

                Validate(raw);
                BattleLog result = new BattleLog
                {
                    Seed = raw.Seed,
                    Winner = raw.Winner,
                    DurationMs = raw.DurationMs,
                    Names = raw.Names,
                    FinalHp = raw.FinalHp,
                    Bags = NormalizeBags(raw.Bags),
                    Items = raw.Items
                };

                for (int i = 0; i < raw.Events.Count; i++)
                {
                    RawBattleEvent source = raw.Events[i];
                    result.Events.Add(new BattleEvent
                    {
                        TimeMs = source.TimeMs,
                        Type = ParseType(source.Type),
                        RawType = source.Type,
                        Side = source.Side,
                        Item = source.Item,
                        Kind = source.Kind,
                        Value = source.Value,
                        Absorbed = source.Absorbed,
                        Total = source.Total,
                        Crit = source.Crit,
                        Hp = source.Hp,
                        MaxHp = source.MaxHp,
                        Shield = source.Shield,
                        Qi = source.Qi
                    });
                }

                return result;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Malformed battle log JSON: " + exception.Message, exception);
            }
        }

        private static List<int[]> NormalizeBags(List<int[]> bags)
        {
            var result = new List<int[]>(2);
            for (int side = 0; side < 2; side++)
            {
                int[] source = bags != null && side < bags.Count ? bags[side] : null;
                result.Add(source != null && source.Length >= 2 && source[0] > 0 && source[1] > 0
                    ? new[] { source[0], source[1] }
                    : new[] { 5, 4 });
            }
            return result;
        }

        private static void Validate(RawBattleLog log)
        {
            if (log.Names == null || log.Names.Count != 2)
                throw new FormatException("Battle log must contain exactly two names.");
            if (log.FinalHp == null || log.FinalHp.Count != 2)
                throw new FormatException("Battle log must contain exactly two finalHp values.");
            if (log.Items == null)
                throw new FormatException("Battle log is missing items.");
            if (log.Events == null)
                throw new FormatException("Battle log is missing events.");
            if (log.Winner < -1 || log.Winner > 1)
                throw new FormatException("Battle log winner must be -1, 0, or 1.");

            for (int i = 0; i < log.Events.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(log.Events[i].Type))
                    throw new FormatException("Battle event at index " + i + " is missing type.");
            }
        }

        private static BattleEventType ParseType(string value)
        {
            switch (value)
            {
                case "START": return BattleEventType.Start;
                case "TRIGGER": return BattleEventType.Trigger;
                case "DAMAGE": return BattleEventType.Damage;
                case "DOT": return BattleEventType.Dot;
                case "BURST": return BattleEventType.Burst;
                case "HEAL": return BattleEventType.Heal;
                case "SHIELD": return BattleEventType.Shield;
                case "QI": return BattleEventType.Qi;
                case "STATUS": return BattleEventType.Status;
                case "PARRY": return BattleEventType.Parry;
                case "CHARGE": return BattleEventType.Charge;
                case "BUFF": return BattleEventType.Buff;
                case "MAX_HP": return BattleEventType.MaxHp;
                case "DMG_REDUCE": return BattleEventType.DmgReduce;
                case "PARRY_COST": return BattleEventType.ParryCost;
                case "END": return BattleEventType.End;
                default: return BattleEventType.Unknown;
            }
        }

        private sealed class RawBattleLog
        {
            [JsonProperty("seed", Required = Required.Always)] public long Seed { get; set; }
            [JsonProperty("winner", Required = Required.Always)] public int Winner { get; set; }
            [JsonProperty("durationMs", Required = Required.Always)] public int DurationMs { get; set; }
            [JsonProperty("names", Required = Required.Always)] public List<string> Names { get; set; }
            [JsonProperty("finalHp", Required = Required.Always)] public List<int> FinalHp { get; set; }
            [JsonProperty("bags")] public List<int[]> Bags { get; set; }
            [JsonProperty("items", Required = Required.Always)] public List<BattleItemRef> Items { get; set; }
            [JsonProperty("events", Required = Required.Always)] public List<RawBattleEvent> Events { get; set; }
        }

        private sealed class RawBattleEvent
        {
            [JsonProperty("t", Required = Required.Always)] public int TimeMs { get; set; }
            [JsonProperty("type", Required = Required.Always)] public string Type { get; set; }
            [JsonProperty("side", Required = Required.Always)] public int Side { get; set; }
            [JsonProperty("item")] public int? Item { get; set; }
            [JsonProperty("kind")] public string Kind { get; set; }
            [JsonProperty("value")] public int? Value { get; set; }
            [JsonProperty("absorbed")] public int? Absorbed { get; set; }
            [JsonProperty("total")] public int? Total { get; set; }
            [JsonProperty("crit")] public bool? Crit { get; set; }
            [JsonProperty("hp")] public int? Hp { get; set; }
            [JsonProperty("maxHp")] public int? MaxHp { get; set; }
            [JsonProperty("shield")] public int? Shield { get; set; }
            [JsonProperty("qi")] public int? Qi { get; set; }
        }
    }
}
