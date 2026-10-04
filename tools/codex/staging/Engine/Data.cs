using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Engine
{
    public sealed class DataException : Exception
    {
        public DataException(string message) : base(message)
        {
        }

        public DataException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemType
    {
        WEAPON,
        ARMOR,
        MANUAL,
        PILL,
        RELIC
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY,
        SUPREME
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetKind
    {
        FIGHTER,
        ITEM
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EffectOp
    {
        DAMAGE,
        GAIN_QI,
        DRAIN_QI,
        SHIELD,
        HEAL,
        SHIELD_BREAK,
        DMG_REDUCE,
        REDUCE_MAX_HP,
        PARRY_DISCOUNT,
        APPLY,
        CHARGE,
        ADD_ATTACK,
        ADD_CRIT,
        BUFF_STACK,
        CLEANSE,
        LIFESTEAL,
        RAISE_MAX_HP
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerEvent
    {
        BATTLE_START,
        HIT,
        CRIT,
        ULTIMATE,
        PARRY_SUCCESS,
        PARRIED,
        HP_BELOW,
        STATUS_APPLIED,
        BURST,
        DAMAGED,
        HEALED,
        CONTROLLED,
        CRIT_TAKEN,
        ENEMY_ULTIMATE,
        QI_EMPTY,
        SHIELD_BROKEN,
        BURST_TAKEN
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerSource
    {
        ANY,
        SELF,
        ADJACENT,
        ENEMY
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Target
    {
        SELF,
        ENEMY,
        SELF_ITEM,
        EVENT_SOURCE,
        SELF_ALL_ITEMS,
        SELF_OTHER_ITEMS,
        SELF_ALL_WEAPONS,
        ADJACENT_ITEMS,
        ADJACENT_WEAPONS,
        ENEMY_RANDOM_ITEM,
        ENEMY_RANDOM_WEAPON,
        ENEMY_ALL_ITEMS
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatusType
    {
        BURN,
        POISON,
        BLEED,
        REGEN,
        STUN,
        SLOW,
        SEAL,
        HASTE
    }

    internal static class Rules
    {
        internal static long Round(double value)
        {
            return (long)Math.Floor(value + 0.5d);
        }

        internal static TargetKind Kind(Target target)
        {
            return target == Target.SELF || target == Target.ENEMY ? TargetKind.FIGHTER : TargetKind.ITEM;
        }

        internal static bool Timed(StatusType status)
        {
            return status == StatusType.STUN || status == StatusType.SLOW ||
                   status == StatusType.SEAL || status == StatusType.HASTE;
        }

        internal static Target StatusTarget(StatusType status)
        {
            if (status == StatusType.REGEN)
                return Target.SELF;
            if (status == StatusType.HASTE)
                return Target.SELF_ITEM;
            if (status == StatusType.SLOW || status == StatusType.SEAL)
                return Target.ENEMY_RANDOM_ITEM;
            return Target.ENEMY;
        }

        internal static Target? OpTarget(EffectOp op)
        {
            if (op == EffectOp.APPLY || op == EffectOp.BUFF_STACK)
                return null;
            if (op == EffectOp.GAIN_QI || op == EffectOp.SHIELD || op == EffectOp.HEAL ||
                op == EffectOp.DMG_REDUCE || op == EffectOp.PARRY_DISCOUNT)
                return Target.SELF;
            if (op == EffectOp.CLEANSE || op == EffectOp.LIFESTEAL || op == EffectOp.RAISE_MAX_HP)
                return Target.SELF;
            if (op == EffectOp.CHARGE || op == EffectOp.ADD_ATTACK || op == EffectOp.ADD_CRIT)
                return Target.SELF_ITEM;
            return Target.ENEMY;
        }
    }

    public sealed class EffectDef
    {
        public EffectOp? op;
        public Target? target;
        public double pct = 1d;
        public int hits = 1;
        public int value;
        public StatusType? status;
        public int stacks;
        public double duration;
        public bool ignoreShield;
        public string stat;
        public int maxStacks;
        public int? minQiAfter;
        public Target? ResolvedTarget()
        {
            return target ??
                   (op == EffectOp.APPLY
                       ? (status.HasValue ? Rules.StatusTarget(status.Value) : (Target? )null)
                       : op == EffectOp.BUFF_STACK
                           ? (stat == "dmg_reduce" ? Target.SELF : Target.SELF_ITEM)
                           : (op.HasValue ? Rules.OpTarget(op.Value) : null));
        }

        public int DurationMs()
        {
            return unchecked((int)Rules.Round(duration * 1000d));
        }

        public int PctPermille()
        {
            return unchecked((int)Rules.Round(pct * 1000d));
        }
    }

    public sealed class UltimateDef
    {
        public int cost;
        public List<EffectDef> effects = new List<EffectDef>();
    }

    public sealed class ItemStats
    {
        public int hp;
        public double dmgReduce;
        public double cooldownReduction;
        public int DmgReducePermille()
        {
            return unchecked((int)Rules.Round(dmgReduce * 1000d));
        }

        public int CooldownReductionPermille()
        {
            return unchecked((int)Rules.Round(cooldownReduction * 1000d));
        }
    }

    public sealed class TriggerDef
    {
        public TriggerEvent? on;
        public TriggerSource from = TriggerSource.ANY;
        public double hpBelow;
        public StatusType? status;
        public int maxUses;
        public List<EffectDef> effects = new List<EffectDef>();
        public int HpBelowPermille()
        {
            return unchecked((int)Rules.Round(hpBelow * 1000d));
        }
    }

    public sealed class UnlockDef
    {
        public int tier;
        public int attack;
        public double cooldown;
        public double crit;
        public int ultimateCost;
        public List<EffectDef> basic = new List<EffectDef>();
        public List<EffectDef> active = new List<EffectDef>();
        public List<EffectDef> ultimate = new List<EffectDef>();
        public List<TriggerDef> triggers = new List<TriggerDef>();
    }

    public sealed class ItemDef
    {
        public string id;
        public string name;
        public ItemType? type;
        public string weaponClass;
        /// <summary>Lowest rarity this item drops at; simulation rarity lives on each owned copy.</summary>
        public Rarity? rarity = Rarity.COMMON;
        public int[] size =
        {
            1,
            1
        };
        public int attack;
        public double cooldown;
        public double crit;
        public ItemStats stats;
        public List<EffectDef> basic = new List<EffectDef>();
        public List<EffectDef> active = new List<EffectDef>();
        public UltimateDef ultimate;
        public List<TriggerDef> triggers = new List<TriggerDef>();
        public List<UnlockDef> unlocks = new List<UnlockDef>();
        public int Width()
        {
            return size[0];
        }

        public int Height()
        {
            return size[1];
        }

        public int CooldownMs()
        {
            return unchecked((int)Rules.Round(cooldown * 1000d));
        }

        public int CritPermille()
        {
            return unchecked((int)Rules.Round(crit * 1000d));
        }

        public bool IsActive()
        {
            return cooldown > 0d;
        }

        public ItemDef Copy()
        {
            ItemDef r = CopyStats();
            r.basic = basic;
            r.active = active;
            r.ultimate = ultimate;
            r.triggers = triggers;
            return r;
        }

        private ItemDef CopyStats()
        {
            return new ItemDef
            {
                id = id,
                name = name,
                type = type,
                weaponClass = weaponClass,
                rarity = rarity,
                size = size,
                attack = attack,
                cooldown = cooldown,
                crit = crit,
                stats = stats
            };
        }

        public ItemDef AtTier(int tier)
        {
            ItemDef r = CopyStats();
            List<EffectDef> b = new List<EffectDef>(basic);
            List<EffectDef> a = new List<EffectDef>(active);
            List<TriggerDef> t = new List<TriggerDef>(triggers);
            List<EffectDef> u = ultimate == null ? new List<EffectDef>() : new List<EffectDef>(ultimate.effects);
            int cost = ultimate == null ? 0 : ultimate.cost;
            foreach (UnlockDef x in unlocks)
            {
                if (x.tier > tier)
                    continue;
                r.attack = unchecked(r.attack + x.attack);
                r.cooldown += x.cooldown;
                r.crit += x.crit;
                cost = unchecked(cost + x.ultimateCost);
                b.AddRange(x.basic);
                a.AddRange(x.active);
                u.AddRange(x.ultimate);
                t.AddRange(x.triggers);
            }

            r.basic = b;
            r.active = a;
            r.triggers = t;
            if (ultimate != null || cost != 0 || u.Count != 0)
                r.ultimate = new UltimateDef
                {
                    cost = cost,
                    effects = u
                };
            return r;
        }
    }

    public sealed class TamPhapLevelDef
    {
        public int level;
        public double attackPct;
        public double cooldownPct;
        public double crit;
        public int ultimateCost;
        public List<EffectDef> basic = new List<EffectDef>();
        public List<EffectDef> ultimate = new List<EffectDef>();
        public List<TriggerDef> triggers = new List<TriggerDef>();
    }

    public sealed class SectDef
    {
        public string id;
        public string name;
        public string tamPhap;
        public List<string> weaponClasses = new List<string>();
        public List<TamPhapLevelDef> levels = new List<TamPhapLevelDef>();
        public bool Buffs(ItemDef d)
        {
            return d.weaponClass != null && weaponClasses.Contains(d.weaponClass);
        }

        public ItemDef ApplyTo(ItemDef d, int level)
        {
            if (!Buffs(d))
                return d;
            ItemDef r = d.Copy();
            double ap = 0d;
            double cp = 0d;
            int cost = r.ultimate == null ? 0 : r.ultimate.cost;
            List<EffectDef> b = new List<EffectDef>(r.basic);
            List<EffectDef> u = r.ultimate == null ? null : new List<EffectDef>(r.ultimate.effects);
            List<TriggerDef> t = new List<TriggerDef>(r.triggers);
            foreach (TamPhapLevelDef x in levels)
            {
                if (x.level > level)
                    continue;
                ap += x.attackPct;
                cp += x.cooldownPct;
                r.crit += x.crit;
                cost = unchecked(cost + x.ultimateCost);
                b.AddRange(x.basic);
                if (u != null)
                    u.AddRange(x.ultimate);
                t.AddRange(x.triggers);
            }

            r.attack = unchecked((int)Rules.Round(r.attack * (1d + ap)));
            r.cooldown *= 1d - cp;
            r.basic = b;
            r.triggers = t;
            if (u != null)
                r.ultimate = new UltimateDef
                {
                    cost = Math.Max(1, cost),
                    effects = u
                };
            return r;
        }
    }

    public sealed class ItemCatalog
    {
        // Add newly introduced weapon classes here when the game data gains one.
        public static readonly HashSet<string> WeaponClasses = new HashSet<string>
        {
            "axe",
            "blade",
            "bow",
            "brush",
            "claw",
            "dual",
            "fan",
            "fist",
            "flute",
            "hammer",
            "hidden",
            "palm",
            "spear",
            "staff",
            "sword",
            "whip",
            "zither"
        };
        private readonly Dictionary<string, ItemDef> items;
        private ItemCatalog(Dictionary<string, ItemDef> items)
        {
            this.items = items;
        }

        public static JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Converters =
                {
                    new StringEnumConverter()
                }
            };
        }

        public static ItemCatalog Load(string json)
        {
            try
            {
                JArray raw = JArray.Parse(json);
                foreach (JObject o in raw.OfType<JObject>())
                {
                    JToken hp = o["stats"]?["hp"];
                    if (hp != null && hp.Type != JTokenType.Integer)
                        throw new DataException(
                            "Item " + ((string)o["id"] ?? "<no id>") +
                            ": stats.hp must be an integer >= 0");
                }

                return Of(raw.ToObject<List<ItemDef>>(JsonSerializer.Create(JsonSettings())));
            }
            catch (DataException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DataException("Cannot read item data: " + e.Message, e);
            }
        }

        public static ItemCatalog Of(IEnumerable<ItemDef> defs)
        {
            Dictionary<string, ItemDef> map = new Dictionary<string, ItemDef>();
            foreach (ItemDef d in defs)
            {
                Validate(d);
                if (map.ContainsKey(d.id))
                    throw new DataException("Duplicate item id: " + d.id);
                map.Add(d.id, d);
            }

            return new ItemCatalog(map);
        }

        public ItemDef Get(string id)
        {
            if (!items.TryGetValue(id, out ItemDef d))
                throw new DataException("Unknown item id: " + id);
            return d;
        }

        public bool Contains(string id)
        {
            return items.ContainsKey(id);
        }

        public IEnumerable<ItemDef> All()
        {
            return items.Values;
        }

        private static void Validate(ItemDef d)
        {
            List<string> e = new List<string>();
            string where = d.id ?? "<no id>";
            if (string.IsNullOrWhiteSpace(d.id))
                e.Add("id is required");
            if (string.IsNullOrWhiteSpace(d.name))
                e.Add("name is required");
            if (!d.type.HasValue)
                e.Add("type is required");
            if (!d.rarity.HasValue)
                e.Add("rarity must be valid when specified");
            if (d.size == null || d.size.Length != 2 || d.size[0] < 1 || d.size[1] < 1)
                e.Add("size must be [w, h] with w, h >= 1");
            if (d.stats != null)
            {
                if (d.stats.hp < 0)
                    e.Add("stats.hp must be >= 0");
                if (double.IsNaN(d.stats.dmgReduce) || d.stats.dmgReduce < 0 || d.stats.dmgReduce > 1)
                    e.Add("stats.dmgReduce must be in [0, 1]");
                if (double.IsNaN(d.stats.cooldownReduction) || d.stats.cooldownReduction < 0 ||
                    d.stats.cooldownReduction > 1)
                    e.Add("stats.cooldownReduction must be in [0, 1]");
            }

            ValidateCore(d, e);
            int last = 0;
            for (int i = 0; i < d.unlocks.Count; i++)
            {
                UnlockDef u = d.unlocks[i];
                if (u.tier != 3 && u.tier != 6 && u.tier != 9)
                    e.Add("unlocks[" + i + "].tier must be one of [3, 6, 9]");
                else if (u.tier <= last)
                    e.Add("unlocks must be sorted by tier without duplicates");
                last = Math.Max(last, u.tier);
            }

            if (e.Count == 0)
                foreach (UnlockDef u in d.unlocks)
                {
                    List<string> te = new List<string>();
                    ValidateCore(d.AtTier(u.tier), te);
                    foreach (string x in te)
                        e.Add("at tier " + u.tier + ": " + x);
                }

            if (e.Count != 0)
                throw new DataException("Item " + where + ": " + string.Join("; ", e));
        }

        private static void ValidateCore(ItemDef d, List<string> e)
        {
            if (d.cooldown < 0)
                e.Add("cooldown must be >= 0");
            if (d.attack < 0)
                e.Add("attack must be >= 0");
            if (d.crit < 0 || d.crit > 1)
                e.Add("crit must be in [0, 1]");
            if (d.type == ItemType.WEAPON && !d.IsActive())
                e.Add("weapon needs cooldown > 0");
            if (d.type == ItemType.WEAPON && d.attack <= 0)
                e.Add("weapon needs attack > 0");
            if (d.type == ItemType.WEAPON && !WeaponClasses.Contains(d.weaponClass))
                e.Add("weaponClass must be one of " + string.Join(", ", WeaponClasses));
            if (d.type.HasValue && d.type != ItemType.WEAPON && d.weaponClass != null)
                e.Add("weaponClass is only valid for weapons");
            if (d.type == ItemType.WEAPON && d.size != null && d.size.Length == 2 && d.size[0] * d.size[1] < 2)
                e.Add("weapon must occupy at least 2 cells");
            if (!d.IsActive() && d.basic.Count != 0)
                e.Add("basic effects need cooldown > 0");
            if (d.active.Count != 0 && d.type != ItemType.RELIC && d.type != ItemType.MANUAL)
                e.Add("active effects are only valid for relic or manual");
            if (d.active.Count != 0 && !d.IsActive())
                e.Add("active effects need cooldown > 0");
            if (d.IsActive() && (d.type == ItemType.RELIC || d.type == ItemType.MANUAL) && d.active.Count == 0)
                e.Add("active relic/manual needs active effects");
            if (d.ultimate != null)
            {
                if (!d.IsActive())
                    e.Add("ultimate needs cooldown > 0");
                if (d.ultimate.cost <= 0)
                    e.Add("ultimate.cost must be > 0");
                ValidateEffects(d.ultimate.effects, "ultimate", true, e);
            }

            ValidateEffects(d.basic, "basic", false, e);
            ValidateEffects(d.active, "active", false, e);
            ValidateTriggers(d.triggers, "triggers", e);
        }

        private static void ValidateTriggers(List<TriggerDef> ts, string where, List<string> e)
        {
            TriggerEvent[] itemEvents =
            {
                TriggerEvent.HIT,
                TriggerEvent.CRIT,
                TriggerEvent.ULTIMATE,
                TriggerEvent.PARRIED,
                TriggerEvent.STATUS_APPLIED,
                TriggerEvent.BURST,
                TriggerEvent.DAMAGED,
                TriggerEvent.HEALED,
                TriggerEvent.CONTROLLED,
                TriggerEvent.CRIT_TAKEN,
                TriggerEvent.ENEMY_ULTIMATE,
                TriggerEvent.QI_EMPTY,
                TriggerEvent.SHIELD_BROKEN,
                TriggerEvent.BURST_TAKEN
            };
            TriggerEvent[] enemyEvents =
            {
                TriggerEvent.DAMAGED,
                TriggerEvent.CONTROLLED,
                TriggerEvent.CRIT_TAKEN,
                TriggerEvent.ENEMY_ULTIMATE,
                TriggerEvent.QI_EMPTY,
                TriggerEvent.BURST_TAKEN
            };
            for (int i = 0; i < ts.Count; i++)
            {
                TriggerDef t = ts[i];
                string w = where + "[" + i + "]";
                if (!t.on.HasValue)
                    e.Add(w + ".on is required");
                if (t.on == TriggerEvent.HP_BELOW && (t.hpBelow <= 0 || t.hpBelow >= 1))
                    e.Add(w + ".hpBelow must be in (0, 1)");
                if (t.maxUses < 0)
                    e.Add(w + ".maxUses must be >= 0");
                if (t.on.HasValue && t.from != TriggerSource.ANY && !itemEvents.Contains(t.on.Value))
                    e.Add(w + ": from=" + t.from + " is only valid for item events");
                if (t.on.HasValue && t.from == TriggerSource.ENEMY && !enemyEvents.Contains(t.on.Value))
                    e.Add(w + ": from=ENEMY is not meaningful for " + t.on);
                ValidateEffects(t.effects, w, false, e);
            }
        }

        private static void ValidateEffects(List<EffectDef> xs, string where, bool ultimate, List<string> e)
        {
            for (int i = 0; i < xs.Count; i++)
            {
                EffectDef x = xs[i];
                string w = where + "[" + i + "]";
                if (!x.op.HasValue)
                {
                    e.Add(w + ".op is required");
                    continue;
                }

                if (x.op == EffectOp.APPLY)
                {
                    if (!x.status.HasValue)
                    {
                        e.Add(w + ": APPLY needs status");
                        continue;
                    }

                    if (Rules.Timed(x.status.Value) ? x.duration <= 0 : x.stacks <= 0)
                        e.Add(w + ": " + x.status + " needs " + (Rules.Timed(x.status.Value) ? "duration" : "stacks"));
                }

                if (x.op == EffectOp.CHARGE && x.duration <= 0)
                    e.Add(w + ": CHARGE needs duration");
                if (x.minQiAfter.HasValue && !ultimate)
                    e.Add(w + ": minQiAfter is only valid inside ultimate effects");
                if (x.minQiAfter < 0)
                    e.Add(w + ": minQiAfter must be >= 0");
                if (x.hits < 1)
                    e.Add(w + ": hits must be >= 1");
                if (x.hits > 1 && x.op != EffectOp.DAMAGE)
                    e.Add(w + ": hits only applies to DAMAGE");
                if (x.op == EffectOp.DAMAGE && (x.value < 0 || x.value == 0 && x.pct <= 0))
                    e.Add(w + ": DAMAGE needs pct > 0 or value > 0");
                if (new[]
                {
                    EffectOp.GAIN_QI,
                    EffectOp.DRAIN_QI,
                    EffectOp.SHIELD,
                    EffectOp.HEAL,
                    EffectOp.SHIELD_BREAK,
                    EffectOp.REDUCE_MAX_HP,
                    EffectOp.PARRY_DISCOUNT,
                    EffectOp.ADD_ATTACK,
                    EffectOp.RAISE_MAX_HP
                }.Contains(x.op.Value) && x.value <= 0)
                    e.Add(w + ": " + x.op + " needs value > 0");
                if ((x.op == EffectOp.DMG_REDUCE || x.op == EffectOp.ADD_CRIT ||
                     x.op == EffectOp.LIFESTEAL) && (x.pct <= 0 || x.pct > 1))
                    e.Add(w + ": " + x.op + " needs pct in (0, 1]");
                if (x.op == EffectOp.BUFF_STACK)
                {
                    if (x.stat != "attack" && x.stat != "crit" && x.stat != "dmg_reduce")
                        e.Add(w + ": BUFF_STACK stat must be attack, crit, or dmg_reduce");
                    if (x.maxStacks <= 0)
                        e.Add(w + ": BUFF_STACK needs maxStacks > 0");
                    if (x.stat == "attack" && x.value <= 0)
                        e.Add(w + ": attack BUFF_STACK needs value > 0");
                    if ((x.stat == "crit" || x.stat == "dmg_reduce") && (x.pct <= 0 || x.pct > 1))
                        e.Add(w + ": " + x.stat + " BUFF_STACK needs pct in (0, 1]");
                }

                if (x.op == EffectOp.CLEANSE && x.status.HasValue && x.status != StatusType.BURN &&
                    x.status != StatusType.POISON && x.status != StatusType.BLEED)
                    e.Add(w + ": CLEANSE status must be burn, poison, or bleed");
                if (x.op == EffectOp.CLEANSE && x.value < 0)
                    e.Add(w + ": CLEANSE value must be >= 0");
                if (x.op == EffectOp.LIFESTEAL && !xs.Take(i).Any(y => y.op == EffectOp.DAMAGE))
                    e.Add(w + ": LIFESTEAL needs a preceding DAMAGE in the same list");
                Target? expected = x.op == EffectOp.APPLY
                    ? (x.status.HasValue ? Rules.StatusTarget(x.status.Value) : (Target? )null)
                    : x.op == EffectOp.BUFF_STACK
                        ? (x.stat == "dmg_reduce" ? Target.SELF : Target.SELF_ITEM)
                        : Rules.OpTarget(x.op.Value);
                Target? actual = x.ResolvedTarget();
                if (!actual.HasValue || !expected.HasValue || Rules.Kind(actual.Value) != Rules.Kind(expected.Value))
                    e.Add(
                        w + ": target " + actual + " is not valid for " + x.op +
                        (x.op == EffectOp.APPLY ? " " + x.status : ""));
            }
        }
    }

    public sealed class SectCatalog
    {
        public const int MAX_LEVEL = 9;
        private readonly Dictionary<string, SectDef> sects;
        private SectCatalog(Dictionary<string, SectDef> sects)
        {
            this.sects = sects;
        }

        public static SectCatalog Empty()
        {
            return new SectCatalog(new Dictionary<string, SectDef>());
        }

        public static SectCatalog Load(string json)
        {
            try
            {
                return Of(JsonConvert.DeserializeObject<List<SectDef>>(json, ItemCatalog.JsonSettings()));
            }
            catch (DataException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DataException("Cannot read sect data: " + e.Message, e);
            }
        }

        public static SectCatalog Of(IEnumerable<SectDef> defs)
        {
            Dictionary<string, SectDef> map = new Dictionary<string, SectDef>();
            foreach (SectDef d in defs)
            {
                if (string.IsNullOrWhiteSpace(d.id))
                    throw new DataException("Sect <no id>: id is required");
                if (map.ContainsKey(d.id))
                    throw new DataException("Duplicate sect id: " + d.id);
                map.Add(d.id, d);
            }

            return new SectCatalog(map);
        }

        public SectDef Get(string id)
        {
            if (!sects.TryGetValue(id, out SectDef d))
                throw new DataException("Unknown sect id: " + id);
            return d;
        }

        public IEnumerable<SectDef> All()
        {
            return sects.Values;
        }
    }
}
