using System;
using System.Collections.Generic;

namespace OneBagJianghu.Engine
{
    internal sealed class Battle
    {
        private readonly BattleConfig cfg;
        private readonly long seed;
        private readonly Rng rng;
        private readonly Fighter[] fighters = new Fighter[2];
        private readonly List<BattleEvent> events = new List<BattleEvent>();
        private int now;
        private bool over;
        private int winner = -1;
        private sealed class Ctx
        {
            internal Fighter owner;
            internal ItemInstance source;
            internal ItemInstance eventSource;
            internal int depth;
            internal string stackKey;
            internal bool suppressStatusApplied;
            internal Ctx(Fighter o, ItemInstance s, ItemInstance e, int d, string k, bool suppress = false)
            {
                owner = o;
                source = s;
                eventSource = e;
                depth = d;
                stackKey = k;
                suppressStatusApplied = suppress;
            }
        }

        internal Battle(BattleConfig cfg, ItemCatalog cat, SectCatalog sects, Loadout a, Loadout b, long seed)
        {
            this.cfg = cfg;
            this.seed = seed;
            rng = new Rng(seed);
            fighters[0] = Fighter.Create(0, a, cat, sects, cfg);
            fighters[1] = Fighter.Create(1, b, cat, sects, cfg);
            fighters[0].enemy = fighters[1];
            fighters[1].enemy = fighters[0];
        }

        internal BattleResult Run()
        {
            foreach (Fighter f in fighters)
                Emit(Ev(EventType.START, f).State(f));
            int first = rng.NextInt(2);
            Fire(TriggerEvent.BATTLE_START, fighters[first], null, null, 0);
            Fire(TriggerEvent.BATTLE_START, fighters[1 - first], null, null, 0);
            CheckDeath();
            int tick = 0;
            while (!over)
            {
                tick++;
                now = unchecked(tick * cfg.tickMs);
                if (now > cfg.maxDurationMs)
                {
                    Finish(-1);
                    break;
                }

                List<ItemInstance> a = Advance(fighters[0]);
                List<ItemInstance> b = Advance(fighters[1]);
                bool lead = rng.NextInt(2) == 0;
                List<ItemInstance> x = lead ? a : b;
                List<ItemInstance> y = lead ? b : a;
                for (int i = 0; i < Math.Max(x.Count, y.Count); i++)
                {
                    if (i < x.Count)
                        Activate(x[i]);
                    if (i < y.Count)
                        Activate(y[i]);
                }

                if (now % 1000 == 0)
                {
                    foreach (Fighter f in fighters)
                        Second(f);
                    if (now >= cfg.exhaustionStartMs)
                        Exhaustion();
                }

                CheckDeath();
            }

            List<BattleResult.ItemRef> refs = new List<BattleResult.ItemRef>();
            foreach (Fighter f in fighters)
                for (int i = 0; i < f.items.Count; i++)
                {
                    ItemInstance it = f.items[i];
                    refs.Add(new BattleResult.ItemRef
                    {
                        uid = it.uid,
                        side = f.side,
                        index = i,
                        id = it.def.id,
                        x = it.rect.x,
                        y = it.rect.y,
                        w = it.rect.w,
                        h = it.rect.h,
                        tier = it.tier
                    });
                }

            return new BattleResult
            {
                seed = seed,
                winner = winner,
                durationMs = now,
                names = new[]
                {
                    fighters[0].name,
                    fighters[1].name
                },
                finalHp = new[]
                {
                    Math.Max(0, fighters[0].hp),
                    Math.Max(0, fighters[1].hp)
                },
                bags = new[]
                {
                    new[]
                    {
                        fighters[0].cols,
                        fighters[0].rows
                    },
                    new[]
                    {
                        fighters[1].cols,
                        fighters[1].rows
                    }
                },
                items = refs,
                events = events
            };
        }

        private List<ItemInstance> Advance(Fighter f)
        {
            List<ItemInstance> ready = new List<ItemInstance>();
            foreach (ItemInstance it in f.items)
            {
                if (!it.IsActive())
                    continue;
                int rate = Rate(it);
                if (rate == 0)
                    continue;
                it.progressMs = unchecked(it.progressMs + cfg.tickMs * rate / 100);
                if (it.progressMs >= it.cooldownMs)
                {
                    it.progressMs = Math.Min(it.progressMs - it.cooldownMs, it.cooldownMs);
                    ready.Add(it);
                }
            }

            return ready;
        }

        private int Rate(ItemInstance it)
        {
            if (it.owner.stunUntil >= now || it.sealUntil >= now)
                return 0;
            int r = 100;
            if (it.hasteUntil >= now)
                r *= 2;
            if (it.slowUntil >= now)
                r /= 2;
            return r;
        }

        private static int Meter(Fighter f, StatusType s)
        {
            return s == StatusType.BURN ? f.burn : s == StatusType.POISON ? f.poison : f.bleed;
        }

        private static void SetMeter(Fighter f, StatusType s, int v)
        {
            if (s == StatusType.BURN)
                f.burn = v;
            else if (s == StatusType.POISON)
                f.poison = v;
            else
                f.bleed = v;
        }

        private void Second(Fighter f)
        {
            if (f.regen > 0 && f.hp > 0)
                Heal(f, f.regen, null, 0);
            foreach (StatusType s in new[]
            {
                StatusType.BURN,
                StatusType.POISON,
                StatusType.BLEED
            }

            )
            {
                int d = cfg.BurstFor(s).decayPerSecond;
                int m = Meter(f, s);
                if (d > 0 && m > 0)
                {
                    int n = Math.Max(0, m - d);
                    SetMeter(f, s, n);
                    Emit(Ev(EventType.STATUS, f).Kind(s.ToString()).Value(n - m).Total(n).State(f));
                }
            }
        }

        private void Exhaustion()
        {
            int n = (now - cfg.exhaustionStartMs) / 1000 + 1;
            int d = unchecked(n * cfg.exhaustionStep);
            foreach (Fighter f in fighters)
                LoseHp(f, d, true, EventType.DOT, "EXHAUSTION", null, false, 0);
        }

        private void Activate(ItemInstance it)
        {
            Fighter self = it.owner;
            Fighter enemy = self.enemy;
            Ctx ctx = new Ctx(self, it, it, 0, "active");
            UltimateDef ult = it.def.ultimate;
            if (ult != null && self.qi >= ult.cost)
            {
                SpendQi(self, ult.cost, it, 0);
                Emit(Ev(EventType.TRIGGER, self).Item(it.uid).Kind("ULTIMATE").State(self));
                int pc = Math.Max(cfg.minParryCost, cfg.parryCost - enemy.parryDiscount);
                bool parried = enemy.qi >= pc;
                Fire(TriggerEvent.ENEMY_ULTIMATE, enemy, it, null, 0);
                if (parried)
                {
                    SpendQi(enemy, pc, it, 0);
                    self.stunUntil = Math.Max(self.stunUntil, now + cfg.parryStunMs);
                    Emit(Ev(EventType.PARRY, enemy).Item(it.uid).Value(cfg.parryStunMs).State(enemy));
                    Emit(
                        Ev(EventType.STATUS, self)
                            .Kind(StatusType.STUN.ToString())
                            .Value(cfg.parryStunMs)
                            .State(self));
                    Fire(TriggerEvent.PARRY_SUCCESS, enemy, null, null, 0);
                    Fire(TriggerEvent.PARRIED, self, it, null, 0);
                    return;
                }

                ExecAll(ult.effects, ctx, true);
                Fire(TriggerEvent.ULTIMATE, self, it, null, 0);
            }
            else
            {
                bool nonWeapon = !it.IsWeapon();
                Emit(Ev(EventType.TRIGGER, self).Item(it.uid).Kind(nonWeapon ? "ACTIVE" : "BASIC").State(self));
                ExecAll(nonWeapon ? it.def.active : it.def.basic, ctx, false);
            }
        }

        private void Fire(TriggerEvent e, Fighter side, ItemInstance source, StatusType? status, int depth)
        {
            if (over || depth >= cfg.maxTriggerDepth)
                return;
            foreach (ItemInstance it in side.items)
                for (int i = 0; i < it.def.triggers.Count; i++)
                {
                    TriggerDef t = it.def.triggers[i];
                    if (t.on != e)
                        continue;
                    if (!SourceMatches(t.from, it, source))
                        continue;
                    if ((e == TriggerEvent.STATUS_APPLIED || e == TriggerEvent.BURST ||
                         e == TriggerEvent.BURST_TAKEN) && t.status.HasValue && t.status != status)
                        continue;
                    if (e == TriggerEvent.HP_BELOW &&
                        (side.hp <= 0 || (long)side.hp * 1000 >= (long)side.maxHp * t.HpBelowPermille()))
                        continue;
                    if (t.maxUses > 0 && it.triggerUses[i] >= t.maxUses)
                        continue;
                    it.triggerUses[i]++;
                    Emit(Ev(EventType.TRIGGER, side).Item(it.uid).Kind("PASSIVE").State(side));
                    ExecAll(
                        t.effects,
                        new Ctx(side, it, source, depth + 1, "trigger:" + i, e == TriggerEvent.STATUS_APPLIED),
                        false);
                }
        }

        private static bool SourceMatches(TriggerSource from, ItemInstance listener, ItemInstance source)
        {
            if (source == null || from == TriggerSource.ANY)
                return true;
            if (from == TriggerSource.SELF)
                return source == listener;
            if (from == TriggerSource.ENEMY)
                return source.owner != listener.owner;
            return source.owner == listener.owner && listener.adjacent.Contains(source);
        }

        private void ExecAll(List<EffectDef> effects, Ctx ctx, bool ultimate)
        {
            int precedingDamage = 0;
            for (int i = 0; i < effects.Count; i++)
            {
                EffectDef e = effects[i];
                if (ultimate && e.minQiAfter.HasValue && ctx.owner.qi < e.minQiAfter.Value)
                    continue;
                if (e.op == EffectOp.DAMAGE)
                    precedingDamage = Damage(e, ctx, FighterOf(e.ResolvedTarget().Value, ctx));
                else if (e.op == EffectOp.LIFESTEAL)
                    Heal(
                        FighterOf(e.ResolvedTarget().Value, ctx),
                        unchecked(precedingDamage * e.PctPermille()) / 1000,
                        ctx.source,
                        ctx.depth);
                else
                    Exec(e, ctx, i);
            }
        }

        private void Exec(EffectDef e, Ctx c, int effectIndex)
        {
            Target target = e.ResolvedTarget().Value;
            EffectOp op = e.op.Value;
            if (op == EffectOp.DAMAGE || op == EffectOp.LIFESTEAL)
                throw new InvalidOperationException("handled by effect list");
            else if (op == EffectOp.GAIN_QI)
                ChangeQi(FighterOf(target, c), e.value, c.source, c.depth);
            else if (op == EffectOp.DRAIN_QI)
                ChangeQi(FighterOf(target, c), -e.value, c.source, c.depth);
            else if (op == EffectOp.SHIELD)
            {
                Fighter f = FighterOf(target, c);
                int cap = (int)((long)f.maxHp * cfg.shieldCapPermille / 1000);
                int v = Math.Max(0, Math.Min(Scale(e.value, c), cap - f.shield));
                f.shield = unchecked(f.shield + v);
                Emit(Ev(EventType.SHIELD, f).Value(v).State(f));
            }
            else if (op == EffectOp.HEAL)
                Heal(FighterOf(target, c), Scale(e.value, c), c.source, c.depth);
            else if (op == EffectOp.SHIELD_BREAK)
            {
                Fighter f = FighterOf(target, c);
                int v = Math.Min(f.shield, Scale(e.value, c));
                if (v > 0)
                {
                    int before = f.shield;
                    f.shield -= v;
                    Emit(Ev(EventType.SHIELD, f).Item(c.source.uid).Value(-v).State(f));
                    if (before > 0 && f.shield == 0)
                        Fire(TriggerEvent.SHIELD_BROKEN, c.owner, c.source, null, c.depth);
                }
            }
            else if (op == EffectOp.DMG_REDUCE)
            {
                Fighter f = FighterOf(target, c);
                f.dmgReducePermille = Math.Min(cfg.maxDmgReducePermille, f.dmgReducePermille + e.PctPermille());
                Emit(Ev(EventType.DMG_REDUCE, f).Value(f.dmgReducePermille).State(f));
            }
            else if (op == EffectOp.PARRY_DISCOUNT)
            {
                Fighter f = FighterOf(target, c);
                f.parryDiscount += e.value;
                Emit(Ev(EventType.PARRY_COST, f).Value(Math.Max(cfg.minParryCost, cfg.parryCost - f.parryDiscount)));
            }
            else if (op == EffectOp.REDUCE_MAX_HP)
            {
                Fighter f = FighterOf(target, c);
                int v = Math.Min(f.maxHp - 1, Scale(e.value, c));
                f.maxHp -= v;
                f.hp = Math.Min(f.hp, f.maxHp);
                Emit(Ev(EventType.MAX_HP, f).Value(-v).State(f));
            }
            else if (op == EffectOp.APPLY)
                Apply(e, c, target);
            else if (op == EffectOp.CHARGE)
                foreach (ItemInstance it in ItemsOf(target, c))
                {
                    it.progressMs = Math.Min(it.cooldownMs, it.progressMs + e.DurationMs());
                    Emit(Ev(EventType.CHARGE, it.owner).Item(it.uid).Value(e.DurationMs()));
                }
            else if (op == EffectOp.ADD_ATTACK)
            {
                int v = Scale(e.value, c);
                foreach (ItemInstance it in ItemsOf(target, c))
                {
                    it.attack += v;
                    Emit(Ev(EventType.BUFF, it.owner).Item(it.uid).Kind("ATTACK").Value(v));
                }
            }
            else if (op == EffectOp.ADD_CRIT)
                foreach (ItemInstance it in ItemsOf(target, c))
                {
                    it.critPermille += e.PctPermille();
                    Emit(Ev(EventType.BUFF, it.owner).Item(it.uid).Kind("CRIT").Value(e.PctPermille()));
                }
            else if (op == EffectOp.BUFF_STACK)
                BuffStack(e, c, target);
            else if (op == EffectOp.CLEANSE)
                Cleanse(e, c);
            else if (op == EffectOp.RAISE_MAX_HP)
            {
                Fighter f = FighterOf(target, c);
                int v = Scale(e.value, c);
                f.maxHp = unchecked(f.maxHp + v);
                f.hp = unchecked(f.hp + v);
                Emit(Ev(EventType.MAX_HP, f).Item(c.source.uid).Kind("RAISE").Value(v).State(f));
            }
        }

        private int Damage(EffectDef e, Ctx c, Fighter target)
        {
            int dealt = 0;
            for (int h = 0; h < e.hits; h++)
            {
                int b = e.value > 0 ? Scale(e.value, c) : unchecked(c.source.attack * e.PctPermille()) / 1000;
                bool crit = rng.Chance(c.source.critPermille);
                if (crit)
                    b = unchecked(b * 2);
                int amount = unchecked(b * (1000 - target.dmgReducePermille)) / 1000;
                dealt = unchecked(
                    dealt + LoseHp(target, amount, e.ignoreShield, EventType.DAMAGE, null, c.source, crit, c.depth));
                Fire(TriggerEvent.HIT, c.owner, c.source, null, c.depth);
                if (crit)
                    Fire(TriggerEvent.CRIT, c.owner, c.source, null, c.depth);
            }

            return dealt;
        }

        private void BuffStack(EffectDef e, Ctx c, Target target)
        {
            string key = c.source.uid + ":" + c.stackKey;
            if (e.stat == "dmg_reduce")
            {
                Fighter f = FighterOf(target, c);
                int count = f.buffStacks.TryGetValue(key, out int n) ? n : 0;
                if (count >= e.maxStacks)
                    return;
                f.buffStacks[key] = count + 1;
                f.dmgReducePermille = Math.Min(
                    cfg.maxDmgReducePermille,
                    f.dmgReducePermille + e.PctPermille());
                Emit(
                    Ev(EventType.BUFF, f)
                        .Item(c.source.uid)
                        .Kind("DMG_REDUCE")
                        .Value(e.PctPermille())
                        .Total(count + 1)
                        .State(f));
                return;
            }

            foreach (ItemInstance it in ItemsOf(target, c))
            {
                int count = it.buffStacks.TryGetValue(key, out int n) ? n : 0;
                if (count >= e.maxStacks)
                    continue;
                it.buffStacks[key] = count + 1;
                int value;
                if (e.stat == "attack")
                {
                    value = Scale(e.value, c);
                    it.attack = unchecked(it.attack + value);
                }
                else
                {
                    value = e.PctPermille();
                    it.critPermille = unchecked(it.critPermille + value);
                }

                Emit(
                    Ev(EventType.BUFF, it.owner)
                        .Item(it.uid)
                        .Kind(e.stat.ToUpperInvariant())
                        .Value(value)
                        .Total(count + 1));
            }
        }

        private void Cleanse(EffectDef e, Ctx c)
        {
            Fighter f = c.owner;
            if (!e.status.HasValue)
            {
                f.stunUntil = Math.Min(f.stunUntil, now - 1);
                foreach (ItemInstance it in f.items)
                {
                    it.slowUntil = Math.Min(it.slowUntil, now - 1);
                    it.sealUntil = Math.Min(it.sealUntil, now - 1);
                }

                Emit(Ev(EventType.STATUS, f).Item(c.source.uid).Kind("CLEANSE_CONTROL").Value(0).State(f));
                return;
            }

            int before = Meter(f, e.status.Value);
            int removed = e.value == 0 ? before : Math.Min(before, e.value);
            SetMeter(f, e.status.Value, before - removed);
            Emit(
                Ev(EventType.STATUS, f)
                    .Item(c.source.uid)
                    .Kind("CLEANSE_" + e.status.Value)
                    .Value(-removed)
                    .Total(before - removed)
                    .State(f));
        }

        private void Apply(EffectDef e, Ctx c, Target target)
        {
            StatusType s = e.status.Value;
            bool applied = false;
            if (s == StatusType.BURN || s == StatusType.POISON || s == StatusType.BLEED)
            {
                Fighter f = FighterOf(target, c);
                int points = Scale(e.stacks, c);
                int meter = Meter(f, s) + points;
                BattleConfig.Burst b = cfg.BurstFor(s);
                bool bursts = meter >= b.threshold;
                SetMeter(f, s, bursts ? 0 : meter);
                Emit(
                    Ev(EventType.STATUS, f)
                        .Kind(s.ToString())
                        .Value(points)
                        .Total(bursts ? b.threshold : meter)
                        .State(f));
                if (!c.suppressStatusApplied)
                    Fire(TriggerEvent.STATUS_APPLIED, c.owner, c.source, s, c.depth);
                if (bursts)
                {
                    int dmg = b.damage + unchecked((int)((long)f.maxHp * b.maxHpPermille / 1000));
                    LoseHp(f, dmg, b.ignoreShield, EventType.BURST, s.ToString(), c.source, false, c.depth);
                    Emit(Ev(EventType.STATUS, f).Kind(s.ToString()).Value(-b.threshold).Total(0).State(f));
                    Fire(TriggerEvent.BURST, c.owner, c.source, s, c.depth);
                }

                return;
            }

            if (s == StatusType.REGEN)
            {
                Fighter f = FighterOf(target, c);
                int stacks = Scale(e.stacks, c);
                f.regen = Math.Min(f.regen + stacks, cfg.maxRegen);
                Emit(Ev(EventType.STATUS, f).Kind(s.ToString()).Value(stacks).Total(f.regen).State(f));
                applied = true;
            }
            else if (s == StatusType.STUN)
            {
                Fighter f = FighterOf(target, c);
                f.stunUntil = Math.Max(f.stunUntil, now + e.DurationMs());
                Emit(Ev(EventType.STATUS, f).Kind(s.ToString()).Value(e.DurationMs()).State(f));
                applied = true;
                if (f != c.owner)
                    Fire(TriggerEvent.CONTROLLED, f, c.source, s, c.depth);
            }
            else
                foreach (ItemInstance it in ItemsOf(target, c))
                {
                    int until = now + e.DurationMs();
                    if (s == StatusType.SLOW)
                        it.slowUntil = Math.Max(it.slowUntil, until);
                    else if (s == StatusType.SEAL)
                        it.sealUntil = Math.Max(it.sealUntil, until);
                    else
                        it.hasteUntil = Math.Max(it.hasteUntil, until);
                    Emit(Ev(EventType.STATUS, it.owner).Item(it.uid).Kind(s.ToString()).Value(e.DurationMs()));
                    applied = true;
                    if ((s == StatusType.SLOW || s == StatusType.SEAL) && it.owner != c.owner)
                        Fire(TriggerEvent.CONTROLLED, it.owner, c.source, s, c.depth);
                }

            if (applied && !c.suppressStatusApplied)
                Fire(TriggerEvent.STATUS_APPLIED, c.owner, c.source, s, c.depth);
        }

        private int LoseHp(
            Fighter f,
            int amount,
            bool ignore,
            EventType type,
            string kind,
            ItemInstance src,
            bool crit,
            int depth)
        {
            int hpBefore = Math.Max(0, f.hp);
            int shieldBefore = f.shield;
            int absorbed = ignore ? 0 : Math.Min(f.shield, amount);
            f.shield -= absorbed;
            int rest = amount - absorbed;
            f.hp -= rest;
            BattleEvent e = Ev(type, f).Value(amount).Absorbed(absorbed);
            if (kind != null)
                e.Kind(kind);
            if (src != null)
                e.Item(src.uid);
            if (crit)
                e.Crit();
            Emit(e.State(f));
            if (shieldBefore > 0 && f.shield == 0 && src != null)
                Fire(TriggerEvent.SHIELD_BROKEN, src.owner, src, null, depth);
            if (rest > 0)
            {
                Fire(TriggerEvent.HP_BELOW, f, null, null, depth);
                if (src != null && src.owner != f &&
                    (type == EventType.BURST || type == EventType.DAMAGE && src.IsWeapon()))
                    Fire(
                        TriggerEvent.DAMAGED,
                        f,
                        src,
                        kind == null ? (StatusType? )null : (StatusType)Enum.Parse(typeof(StatusType), kind),
                        depth);
            }

            if (crit && src != null && src.owner != f)
                Fire(TriggerEvent.CRIT_TAKEN, f, src, null, depth);
            if (type == EventType.BURST && src != null && src.owner != f)
                Fire(TriggerEvent.BURST_TAKEN, f, src, (StatusType)Enum.Parse(typeof(StatusType), kind), depth);
            return unchecked(absorbed + Math.Min(rest, hpBefore));
        }

        private void Heal(Fighter f, int amount, ItemInstance src, int depth)
        {
            if (f.hp <= 0)
                return;
            int actual = Math.Min(amount, f.maxHp - f.hp);
            if (actual <= 0)
                return;
            f.hp += actual;
            Emit(Ev(EventType.HEAL, f).Value(actual).State(f));
            Fire(TriggerEvent.HEALED, f, src, null, depth);
        }

        private void ChangeQi(Fighter f, int delta, ItemInstance src, int depth)
        {
            int before = f.qi;
            int next = Math.Max(0, Math.Min(cfg.maxQi, f.qi + delta));
            if (next == f.qi)
                return;
            int actual = next - f.qi;
            f.qi = next;
            Emit(Ev(EventType.QI, f).Value(actual).State(f));
            if (before > 0 && next == 0)
                Fire(TriggerEvent.QI_EMPTY, f, src, null, depth);
        }

        private void SpendQi(Fighter f, int amount, ItemInstance src, int depth)
        {
            int before = f.qi;
            f.qi = Math.Max(0, f.qi - amount);
            if (before > 0 && f.qi == 0)
                Fire(TriggerEvent.QI_EMPTY, f, src, null, depth);
        }

        private void CheckDeath()
        {
            if (over)
                return;
            bool a = fighters[0].hp <= 0;
            bool b = fighters[1].hp <= 0;
            if (a && b)
                Finish(-1);
            else if (a)
                Finish(1);
            else if (b)
                Finish(0);
        }

        private void Finish(int side)
        {
            over = true;
            winner = side;
            Emit(new BattleEvent(now, EventType.END, side));
        }

        private static Fighter FighterOf(Target t, Ctx c)
        {
            return t == Target.ENEMY ? c.owner.enemy : c.owner;
        }

        private List<ItemInstance> ItemsOf(Target t, Ctx c)
        {
            if (t == Target.SELF_ITEM)
                return new List<ItemInstance>
                {
                    c.source
                };
            if (t == Target.EVENT_SOURCE)
                return new List<ItemInstance>
                {
                    c.eventSource ?? c.source
                };
            if (t == Target.SELF_ALL_ITEMS)
                return Active(c.owner.items, false, null);
            if (t == Target.SELF_OTHER_ITEMS)
                return Active(c.owner.items, false, c.source);
            if (t == Target.SELF_ALL_WEAPONS)
                return Active(c.owner.items, true, null);
            if (t == Target.ADJACENT_ITEMS)
                return Active(c.source.adjacent, false, null);
            if (t == Target.ADJACENT_WEAPONS)
                return Active(c.source.adjacent, true, null);
            if (t == Target.ENEMY_ALL_ITEMS)
                return Active(c.owner.enemy.items, false, null);
            if (t == Target.ENEMY_RANDOM_ITEM)
                return Pick(Active(c.owner.enemy.items, false, null));
            if (t == Target.ENEMY_RANDOM_WEAPON)
                return Pick(Active(c.owner.enemy.items, true, null));
            throw new InvalidOperationException("Not an item target: " + t);
        }

        private static List<ItemInstance> Active(List<ItemInstance> items, bool weapons, ItemInstance exclude)
        {
            List<ItemInstance> r = new List<ItemInstance>();
            foreach (ItemInstance it in items)
                if (it.IsActive() && it != exclude && (!weapons || it.IsWeapon()))
                    r.Add(it);
            return r;
        }

        private List<ItemInstance> Pick(List<ItemInstance> c)
        {
            return c.Count == 0 ? new List<ItemInstance>() : new List<ItemInstance>
            {
                c[rng.NextInt(c.Count)]
            };
        }

        // Parity contract with Java: apply tier, truncate, then rarity, truncate.
        private int Scale(int value, Ctx c)
        {
            return unchecked(
                unchecked(value * cfg.TierMultiplier(c.source.tier)) / 1000 *
                cfg.RarityMultiplier(c.source.rarity)) / 1000;
        }

        private BattleEvent Ev(EventType type, Fighter f)
        {
            return new BattleEvent(now, type, f.side);
        }

        private void Emit(BattleEvent e)
        {
            events.Add(e);
        }
    }
}
