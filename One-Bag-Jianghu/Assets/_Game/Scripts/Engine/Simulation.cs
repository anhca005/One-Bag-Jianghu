using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Engine
{
    public sealed class Loadout
    {
        public string name = "?";
        public int level = 1;
        public int cols = 5;
        public int rows = 4;
        public string sect;
        public int tamPhapLevel = 1;
        public List<PlacedItem> items = new List<PlacedItem>();
        public sealed class PlacedItem
        {
            public string id;
            public int x;
            public int y;
            public int tier = 1;
            public Rarity rarity = Rarity.COMMON;
            public PlacedItem()
            {
            }

            public PlacedItem(string id, int x, int y)
            {
                this.id = id;
                this.x = x;
                this.y = y;
            }
        }

        public static Loadout FromJson(string json)
        {
            try
            {
                JObject o = JObject.Parse(json);
                string n = (string)o["name"] ?? "?";
                if (o.Property("hp") != null)
                    throw new DataException("Loadout " + n + ": \"hp\" was removed (decision #35); use \"level\"");
                Loadout r = o.ToObject<Loadout>(JsonSerializer.Create(ItemCatalog.JsonSettings()));
                if (r.level < 1)
                    throw new DataException("Loadout " + r.name + ": level must be >= 1");
                return r;
            }
            catch (DataException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DataException("Cannot read loadout: " + e.Message, e);
            }
        }

        public Loadout Add(string id, int x, int y, int tier = 1)
        {
            items.Add(new PlacedItem(id, x, y) { tier = tier });
            return this;
        }
    }

    public sealed class Rng
    {
        private ulong state;
        public Rng(long seed)
        {
            state = unchecked((ulong)seed);
        }

        public long NextLong()
        {
            unchecked
            {
                ulong z = state += 0x9E3779B97F4A7C15UL;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return (long)(z ^ (z >> 31));
            }
        }

        public int NextInt(int bound)
        {
            if (bound <= 0)
                throw new ArgumentException("bound must be > 0");
            return (int)(unchecked((ulong)NextLong()) % (ulong)bound);
        }

        public bool Chance(int p)
        {
            if (p <= 0)
                return false;
            if (p >= 1000)
                return true;
            return NextInt(1000) < p;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType
    {
        START,
        TRIGGER,
        DAMAGE,
        DOT,
        BURST,
        HEAL,
        SHIELD,
        QI,
        STATUS,
        PARRY,
        CHARGE,
        BUFF,
        MAX_HP,
        DMG_REDUCE,
        PARRY_COST,
        END
    }

    public sealed class BattleEvent
    {
        public int t;
        public EventType type;
        public int side;
        public int? item;
        public string kind;
        public int? value;
        public int? absorbed;
        public int? total;
        public bool? crit;
        public int? hp;
        public int? maxHp;
        public int? shield;
        public int? qi;
        internal BattleEvent(int t, EventType type, int side)
        {
            this.t = t;
            this.type = type;
            this.side = side;
        }

        internal BattleEvent Item(int x)
        {
            item = x;
            return this;
        }

        internal BattleEvent Kind(string x)
        {
            kind = x;
            return this;
        }

        internal BattleEvent Value(int x)
        {
            value = x;
            return this;
        }

        internal BattleEvent Absorbed(int x)
        {
            absorbed = x;
            return this;
        }

        internal BattleEvent Total(int x)
        {
            total = x;
            return this;
        }

        internal BattleEvent Crit()
        {
            crit = true;
            return this;
        }

        internal BattleEvent State(Fighter f)
        {
            hp = Math.Max(0, f.hp);
            maxHp = f.maxHp;
            shield = f.shield;
            qi = f.qi;
            return this;
        }

        internal JObject ToJObject()
        {
            JObject o = new JObject
            {
                ["t"] = t,
                ["type"] = type.ToString(),
                ["side"] = side
            };
            if (item.HasValue)
                o["item"] = item;
            if (kind != null)
                o["kind"] = kind;
            if (value.HasValue)
                o["value"] = value;
            if (absorbed.HasValue)
                o["absorbed"] = absorbed;
            if (total.HasValue)
                o["total"] = total;
            if (crit.HasValue)
                o["crit"] = crit;
            if (hp.HasValue)
                o["hp"] = hp;
            if (maxHp.HasValue)
                o["maxHp"] = maxHp;
            if (shield.HasValue)
                o["shield"] = shield;
            if (qi.HasValue)
                o["qi"] = qi;
            return o;
        }
    }

    public sealed class BattleResult
    {
        public long seed;
        public int winner;
        public int durationMs;
        public string[] names;
        public int[] finalHp;
        public int[][] bags;
        public List<ItemRef> items;
        public List<BattleEvent> events;
        public sealed class ItemRef
        {
            public int uid;
            public int side;
            public int index;
            public string id;
            public int x;
            public int y;
            public int w;
            public int h;
            public int tier;
        }

        public JObject ToJObject()
        {
            JObject o = new JObject
            {
                ["seed"] = seed,
                ["winner"] = winner,
                ["durationMs"] = durationMs,
                ["names"] = JArray.FromObject(names),
                ["finalHp"] = JArray.FromObject(finalHp),
                ["bags"] = JArray.FromObject(bags)
            };
            JArray ir = new JArray();
            foreach (ItemRef i in items)
                ir.Add(JObject.FromObject(i));
            o["items"] = ir;
            JArray ev = new JArray();
            foreach (BattleEvent e in events)
                ev.Add(e.ToJObject());
            o["events"] = ev;
            return o;
        }
    }

    public sealed class BattleConfig
    {
        public int tickMs = 100,
            maxQi = 100,
            parryCost = 60,
            minParryCost = 20,
            parryStunMs = 1000,
            exhaustionStartMs = 45000,
            exhaustionStep = 5,
            maxDurationMs = 120000,
            maxTriggerDepth = 8,
            maxRegen = 3,
            maxDmgReducePermille = 800,
            baseHp = 100,
            hpPerLevel = 5,
            maxCooldownReducePermille = 400;
        public int[] tierMultiplierPermille =
        {
            1000,
            1050,
            1100,
            1150,
            1200,
            1250,
            1300,
            1350,
            1400
        };
        public Burst burn = new Burst(50, 80, 0, false, 0),
            poison = new Burst(50, 32, 0, true, 0),
            bleed = new Burst(50, 20, 35, true, 0);
        public sealed class Burst
        {
            public int threshold, damage, maxHpPermille, decayPerSecond;
            public bool ignoreShield;
            public Burst(int t, int d, int m, bool i, int f)
            {
                threshold = t;
                damage = d;
                maxHpPermille = m;
                ignoreShield = i;
                decayPerSecond = f;
            }
        }

        public static BattleConfig Defaults()
        {
            return new BattleConfig();
        }

        public int shieldCapPermille = 1000;
        public int[] rarityMultiplierPermille =
        {
            1000,
            1100,
            1210,
            1330,
            1460,
            1600
        };
        public int RarityMultiplier(Rarity r)
        {
            return rarityMultiplierPermille[(int)r];
        }

        public int TierMultiplier(int t)
        {
            return tierMultiplierPermille[Math.Max(1, Math.Min(t, tierMultiplierPermille.Length)) - 1];
        }

        public Burst BurstFor(StatusType s)
        {
            return s == StatusType.BURN ? burn : s == StatusType.POISON ? poison : s == StatusType.BLEED ? bleed : null;
        }
    }

    internal sealed class ItemInstance
    {
        internal readonly int uid, tier;
        internal int cooldownMs;
        internal readonly Rarity rarity;
        internal readonly ItemDef def;
        internal readonly Fighter owner;
        internal readonly Rect rect;
        internal readonly List<ItemInstance> adjacent = new List<ItemInstance>();
        internal readonly int[] triggerUses;
        internal readonly Dictionary<string, int> buffStacks = new Dictionary<string, int>();
        internal int attack, critPermille, progressMs, hasteUntil, slowUntil, sealUntil;
        internal ItemInstance(int uid, ItemDef d, Fighter o, int tier, Rarity rarity, Rect r, int mult, int rarityMult)
        {
            this.uid = uid;
            def = d;
            owner = o;
            this.tier = tier;
            this.rarity = rarity;
            rect = r;
            triggerUses = new int[d.triggers.Count];
            attack = unchecked(unchecked(d.attack * mult) / 1000 * rarityMult) / 1000;
            critPermille = d.CritPermille();
            cooldownMs = d.CooldownMs();
        }

        internal bool IsActive()
        {
            return cooldownMs > 0;
        }

        internal bool IsWeapon()
        {
            return def.type == ItemType.WEAPON;
        }
    }

    internal sealed class Fighter
    {
        internal readonly int side;
        internal readonly string name;
        internal readonly List<ItemInstance> items = new List<ItemInstance>();
        internal readonly Dictionary<string, int> buffStacks = new Dictionary<string, int>();
        internal int cols,
            rows,
            maxHp,
            hp,
            shield,
            qi,
            dmgReducePermille,
            cooldownReductionPermille,
            parryDiscount,
            burn,
            poison,
            bleed,
            regen,
            stunUntil;
        internal Fighter enemy;
        private Fighter(int s, string n, int hp)
        {
            side = s;
            name = n;
            maxHp = hp;
            this.hp = hp;
        }

        internal static Fighter Create(int side, Loadout l, ItemCatalog c, SectCatalog s, BattleConfig cfg)
        {
            if (l.level < 1)
                throw new DataException("Loadout " + l.name + ": level must be >= 1");
            if (l.tamPhapLevel < 1 || l.tamPhapLevel > 9)
                throw new DataException("Loadout " + l.name + ": tamPhapLevel must be 1..9");
            SectDef sect = l.sect == null ? null : s.Get(l.sect);
            Fighter f = new Fighter(side, l.name, 1)
            {
                cols = l.cols,
                rows = l.rows
            };
            List<Rect> rs = new List<Rect>();
            List<ItemDef> ds = new List<ItemDef>();
            foreach (Loadout.PlacedItem p in l.items)
            {
                ItemDef d = c.Get(p.id);
                if (p.tier < 1 || p.tier > cfg.tierMultiplierPermille.Length)
                    throw new DataException("Loadout " + l.name + ": " + p.id + " has invalid tier " + p.tier);
                ds.Add(d);
                rs.Add(new Rect(p.x, p.y, d.Width(), d.Height()));
            }

            List<string> errors = new BagGrid(l.cols, l.rows).Validate(rs);
            if (errors.Count != 0)
                throw new DataException("Loadout " + l.name + ": " + string.Join("; ", errors));
            for (int i = 0; i < ds.Count; i++)
            {
                int tier = l.items[i].tier;
                ItemDef d = ds[i].AtTier(tier);
                if (sect != null)
                    d = sect.ApplyTo(d, l.tamPhapLevel);
                Rarity rarity = l.items[i].rarity;
                f.items.Add(
                    new ItemInstance(
                        side * 100 + i,
                        d,
                        f,
                        tier,
                        rarity,
                        rs[i],
                        cfg.TierMultiplier(tier),
                        cfg.RarityMultiplier(rarity)));
            }

            long hpBonus = 0, dmgReduce = 0, cooldownReduction = 0;
            foreach (ItemInstance it in f.items)
            {
                ItemStats st = it.def.stats;
                if (st == null)
                    continue;
                hpBonus += Scale(st.hp, it.tier, it.rarity, cfg);
                dmgReduce += Scale(st.DmgReducePermille(), it.tier, it.rarity, cfg);
                cooldownReduction += Scale(st.CooldownReductionPermille(), it.tier, it.rarity, cfg);
            }

            f.maxHp = f.hp = unchecked(cfg.baseHp + cfg.hpPerLevel * (l.level - 1) + (int)hpBonus);
            f.dmgReducePermille = (int)Math.Min(cfg.maxDmgReducePermille, dmgReduce);
            f.cooldownReductionPermille = (int)Math.Min(cfg.maxCooldownReducePermille, cooldownReduction);
            foreach (ItemInstance it in f.items)
                if (it.cooldownMs > 0)
                    it.cooldownMs = Math.Max(
                        cfg.tickMs,
                        unchecked(it.cooldownMs * (1000 - f.cooldownReductionPermille)) / 1000);
            List<List<int>> adj = BagGrid.Adjacency(rs);
            for (int i = 0; i < f.items.Count; i++)
                foreach (int j in adj[i])
                    f.items[i].adjacent.Add(f.items[j]);
            return f;
        }

        internal static int ScaleStat(int value, int tier, Rarity rarity, BattleConfig cfg)
        {
            return unchecked(unchecked(value * cfg.TierMultiplier(tier)) / 1000 * cfg.RarityMultiplier(rarity)) / 1000;
        }

        private static int Scale(int value, int tier, Rarity rarity, BattleConfig cfg)
        {
            return ScaleStat(value, tier, rarity, cfg);
        }
    }

    public sealed class BattleSimulator
    {
        private readonly ItemCatalog catalog;
        private readonly SectCatalog sects;
        private readonly BattleConfig config;
        public BattleSimulator(ItemCatalog catalog, SectCatalog sects, BattleConfig config)
        {
            this.catalog = catalog;
            this.sects = sects;
            this.config = config;
        }

        public BattleSimulator(ItemCatalog catalog) : this(catalog, SectCatalog.Empty(), BattleConfig.Defaults())
        {
        }

        public FighterStats FighterStats(Loadout loadout)
        {
            Fighter fighter = Fighter.Create(0, loadout, catalog, sects, config);
            return new FighterStats(fighter.maxHp, fighter.dmgReducePermille, fighter.cooldownReductionPermille);
        }

        public BattleResult Run(Loadout a, Loadout b, long seed)
        {
            return new Battle(config, catalog, sects, a, b, seed).Run();
        }
    }

    public sealed class FighterStats
    {
        public readonly int maxHp;
        public readonly int dmgReducePermille;
        public readonly int cooldownReductionPermille;
        internal FighterStats(int maxHp, int dmgReducePermille, int cooldownReductionPermille)
        {
            this.maxHp = maxHp;
            this.dmgReducePermille = dmgReducePermille;
            this.cooldownReductionPermille = cooldownReductionPermille;
        }
    }
}
