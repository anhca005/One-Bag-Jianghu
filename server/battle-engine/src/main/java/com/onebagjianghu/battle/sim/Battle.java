package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.EffectDef;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.data.StatusType;
import com.onebagjianghu.battle.data.Target;
import com.onebagjianghu.battle.data.TriggerDef;
import com.onebagjianghu.battle.data.TriggerEvent;
import com.onebagjianghu.battle.data.TriggerSource;
import com.onebagjianghu.battle.data.UltimateDef;

import java.util.ArrayList;
import java.util.List;

/** One battle run. Not reusable, not thread-safe. See docs/design/combat.md. */
final class Battle {
    private final BattleConfig cfg;
    private final long seed;
    private final Rng rng;
    private final Fighter[] fighters = new Fighter[2];
    private final List<BattleEvent> events = new ArrayList<>();
    private int now;
    private boolean over;
    private int winner = -1;

    /** source = item owning the effect; eventSource = item that raised the triggering event. */
    private record Ctx(Fighter owner, ItemInstance source, ItemInstance eventSource, int depth, String stackKey,
                       boolean suppressStatusApplied) {
    }

    Battle(BattleConfig cfg, ItemCatalog catalog, SectCatalog sects, Loadout a, Loadout b, long seed) {
        this.cfg = cfg;
        this.seed = seed;
        this.rng = new Rng(seed);
        fighters[0] = Fighter.create(0, a, catalog, sects, cfg);
        fighters[1] = Fighter.create(1, b, catalog, sects, cfg);
        fighters[0].enemy = fighters[1];
        fighters[1].enemy = fighters[0];
    }

    BattleResult run() {
        for (Fighter f : fighters) {
            emit(ev(EventType.START, f).state(f));
        }
        // Same seeded coin flip as inside a tick: start-of-battle effects that touch shared
        // resources (e.g. drain_qi vs gain_qi) must not always favour side 0.
        int startFirst = rng.nextInt(2);
        fire(TriggerEvent.BATTLE_START, fighters[startFirst], null, null, 0);
        fire(TriggerEvent.BATTLE_START, fighters[1 - startFirst], null, null, 0);
        checkDeath();

        int tick = 0;
        while (!over) {
            tick++;
            now = tick * cfg.tickMs;
            if (now > cfg.maxDurationMs) {
                finish(-1);
                break;
            }
            // Both sides fight at the same time. Everything that becomes ready in this tick is
            // collected first (so a stun applied this tick cannot cancel an attack already released),
            // every collected activation resolves, and only then is death checked: a hit released in
            // the same tick as the killing blow still lands, and a double KO is a draw.
            List<ItemInstance> readyA = advanceItems(fighters[0]);
            List<ItemInstance> readyB = advanceItems(fighters[1]);
            // Resolution order inside the tick only matters for shared resources (qi, Hóa Kình);
            // interleave both sides and let a seeded coin flip pick who leads.
            boolean aLeads = rng.nextInt(2) == 0;
            List<ItemInstance> first = aLeads ? readyA : readyB;
            List<ItemInstance> second = aLeads ? readyB : readyA;
            for (int i = 0; i < Math.max(first.size(), second.size()); i++) {
                if (i < first.size()) activate(first.get(i));
                if (i < second.size()) activate(second.get(i));
            }
            if (now % 1000 == 0) {
                for (Fighter f : fighters) {
                    secondTick(f);
                }
                if (now >= cfg.exhaustionStartMs) {
                    exhaustion();
                }
            }
            checkDeath();
        }

        List<BattleResult.ItemRef> refs = new ArrayList<>();
        for (Fighter f : fighters) {
            for (int i = 0; i < f.items.size(); i++) {
                ItemInstance it = f.items.get(i);
                refs.add(new BattleResult.ItemRef(it.uid, f.side, i, it.def.id,
                        it.rect.x(), it.rect.y(), it.rect.w(), it.rect.h(), it.tier));
            }
        }
        return new BattleResult(seed, winner, now,
                new String[] {fighters[0].name, fighters[1].name},
                new int[] {Math.max(0, fighters[0].hp), Math.max(0, fighters[1].hp)},
                new int[][] {{fighters[0].cols, fighters[0].rows}, {fighters[1].cols, fighters[1].rows}},
                List.copyOf(refs), List.copyOf(events));
    }

    // ---- time ----

    /** Advances every cooldown of one side and returns the items that fire this tick. */
    private List<ItemInstance> advanceItems(Fighter f) {
        List<ItemInstance> ready = new ArrayList<>();
        for (ItemInstance it : f.items) {
            if (!it.isActive()) continue;
            int rate = rate(it);
            if (rate == 0) continue;
            it.progressMs += cfg.tickMs * rate / 100;
            if (it.progressMs >= it.cooldownMs) {
                // Keep the overflow (haste precision) but never bank more than one extra activation.
                it.progressMs = Math.min(it.progressMs - it.cooldownMs, it.cooldownMs);
                ready.add(it);
            }
        }
        return ready;
    }

    /**
     * Percent of normal cooldown speed. The tick at {@code now} covers (now - tickMs, now],
     * so a status lasting until T still applies to the tick ending at T.
     */
    private int rate(ItemInstance it) {
        if (it.owner.stunUntil >= now || it.sealUntil >= now) return 0;
        int r = 100;
        if (it.hasteUntil >= now) r *= 2;
        if (it.slowUntil >= now) r /= 2;
        return r;
    }

    /** Once per second: Điều Tức heals, build-up meters fade if configured. */
    private void secondTick(Fighter f) {
        if (f.regen > 0 && f.hp > 0) {
            heal(f, f.regen, null, 0);
        }
        for (StatusType s : BUILD_UP) {
            int decay = cfg.burst(s).decayPerSecond;
            int meter = meter(f, s);
            if (decay > 0 && meter > 0) {
                int next = Math.max(0, meter - decay);
                setMeter(f, s, next);
                emit(ev(EventType.STATUS, f).kind(s.name()).value(next - meter).total(next).state(f));
            }
        }
    }

    private static final StatusType[] BUILD_UP = {StatusType.BURN, StatusType.POISON, StatusType.BLEED};

    private static int meter(Fighter f, StatusType s) {
        return switch (s) {
            case BURN -> f.burn;
            case POISON -> f.poison;
            default -> f.bleed;
        };
    }

    private static void setMeter(Fighter f, StatusType s, int value) {
        switch (s) {
            case BURN -> f.burn = value;
            case POISON -> f.poison = value;
            default -> f.bleed = value;
        }
    }

    private void exhaustion() {
        int n = (now - cfg.exhaustionStartMs) / 1000 + 1;
        int dmg = n * cfg.exhaustionStep;
        for (Fighter f : fighters) {
            loseHp(f, dmg, true, EventType.DOT, "EXHAUSTION", null, false, 0);
        }
    }

    // ---- activation ----

    private void activate(ItemInstance it) {
        Fighter self = it.owner;
        Fighter enemy = self.enemy;
        Ctx ctx = new Ctx(self, it, it, 0, "active", false);
        UltimateDef ult = it.def.ultimate;
        if (ult != null && self.qi >= ult.cost) {
            spendQi(self, ult.cost, it, 0);
            emit(ev(EventType.TRIGGER, self).item(it.uid).kind("ULTIMATE").state(self));
            int parryCost = Math.max(cfg.minParryCost, cfg.parryCost - enemy.parryDiscount);
            boolean parried = enemy.qi >= parryCost;
            fire(TriggerEvent.ENEMY_ULTIMATE, enemy, it, null, 0);
            if (parried) {
                // Hóa Kình: the ultimate is cancelled, its qi is still spent.
                spendQi(enemy, parryCost, it, 0);
                self.stunUntil = Math.max(self.stunUntil, now + cfg.parryStunMs);
                emit(ev(EventType.PARRY, enemy).item(it.uid).value(cfg.parryStunMs).state(enemy));
                emit(ev(EventType.STATUS, self).kind(StatusType.STUN.name()).value(cfg.parryStunMs).state(self));
                fire(TriggerEvent.PARRY_SUCCESS, enemy, null, null, 0);
                fire(TriggerEvent.PARRIED, self, it, null, 0);
                return;
            }
            execAll(ult.effects, ctx, true);
            fire(TriggerEvent.ULTIMATE, self, it, null, 0);
        } else {
            boolean nonWeaponActive = !it.isWeapon();
            emit(ev(EventType.TRIGGER, self).item(it.uid).kind(nonWeaponActive ? "ACTIVE" : "BASIC").state(self));
            execAll(nonWeaponActive ? it.def.active : it.def.basic, ctx, false);
        }
    }

    private void fire(TriggerEvent event, Fighter side, ItemInstance eventSource, StatusType status, int depth) {
        if (over || depth >= cfg.maxTriggerDepth) return;
        for (ItemInstance it : side.items) {
            List<TriggerDef> triggers = it.def.triggers;
            for (int i = 0; i < triggers.size(); i++) {
                TriggerDef t = triggers.get(i);
                if (t.on != event) continue;
                if (!sourceMatches(t.from, it, eventSource)) continue;
                if ((event == TriggerEvent.STATUS_APPLIED || event == TriggerEvent.BURST || event == TriggerEvent.BURST_TAKEN)
                        && t.status != null && t.status != status) continue;
                // A fighter already at 0 HP this tick cannot be saved by a pill.
                if (event == TriggerEvent.HP_BELOW && (side.hp <= 0
                        || (long) side.hp * 1000 >= (long) side.maxHp * t.hpBelowPermille())) continue;
                if (t.maxUses > 0 && it.triggerUses[i] >= t.maxUses) continue;
                it.triggerUses[i]++;
                emit(ev(EventType.TRIGGER, side).item(it.uid).kind("PASSIVE").state(side));
                execAll(t.effects, new Ctx(side, it, eventSource, depth + 1, "trigger:" + i,
                        event == TriggerEvent.STATUS_APPLIED), false);
            }
        }
    }

    private static boolean sourceMatches(TriggerSource from, ItemInstance listener, ItemInstance eventSource) {
        if (eventSource == null || from == TriggerSource.ANY) return true;
        if (from == TriggerSource.SELF) return eventSource == listener;
        if (from == TriggerSource.ENEMY) return eventSource.owner != listener.owner;
        return eventSource.owner == listener.owner && listener.adjacent.contains(eventSource);
    }

    // ---- effects ----

    private void execAll(List<EffectDef> effects, Ctx ctx) {
        execAll(effects, ctx, false);
    }

    private void execAll(List<EffectDef> effects, Ctx ctx, boolean ultimate) {
        int precedingDamage = 0;
        for (int i = 0; i < effects.size(); i++) {
            EffectDef e = effects.get(i);
            if (ultimate && e.minQiAfter != null && ctx.owner.qi < e.minQiAfter) continue;
            if (e.op == com.onebagjianghu.battle.data.EffectOp.DAMAGE) {
                precedingDamage = damage(e, ctx, fighterOf(e.resolvedTarget(), ctx));
            } else if (e.op == com.onebagjianghu.battle.data.EffectOp.LIFESTEAL) {
                heal(fighterOf(e.resolvedTarget(), ctx), precedingDamage * e.pctPermille() / 1000, ctx.source, ctx.depth);
            } else {
                exec(e, ctx, i);
            }
        }
    }

    private void exec(EffectDef e, Ctx ctx, int effectIndex) {
        Target target = e.resolvedTarget();
        switch (e.op) {
            case DAMAGE, LIFESTEAL -> throw new IllegalStateException("handled by effect list");
            case GAIN_QI -> changeQi(fighterOf(target, ctx), e.value, ctx.source, ctx.depth);
            case DRAIN_QI -> changeQi(fighterOf(target, ctx), -e.value, ctx.source, ctx.depth);
            case SHIELD -> {
                Fighter f = fighterOf(target, ctx);
                int cap = (int) ((long) f.maxHp * cfg.shieldCapPermille / 1000);
                int v = Math.max(0, Math.min(scale(e.value, ctx), cap - f.shield));
                f.shield += v;
                emit(ev(EventType.SHIELD, f).value(v).state(f));
            }
            case HEAL -> heal(fighterOf(target, ctx), scale(e.value, ctx), ctx.source, ctx.depth);
            case SHIELD_BREAK -> {
                Fighter f = fighterOf(target, ctx);
                int v = Math.min(f.shield, scale(e.value, ctx));
                if (v > 0) {
                    int before = f.shield;
                    f.shield -= v;
                    emit(ev(EventType.SHIELD, f).item(ctx.source.uid).value(-v).state(f));
                    if (before > 0 && f.shield == 0) fire(TriggerEvent.SHIELD_BROKEN, ctx.owner, ctx.source, null, ctx.depth);
                }
            }
            case DMG_REDUCE -> {
                Fighter f = fighterOf(target, ctx);
                f.dmgReducePermille = Math.min(cfg.maxDmgReducePermille, f.dmgReducePermille + e.pctPermille());
                emit(ev(EventType.DMG_REDUCE, f).value(f.dmgReducePermille).state(f));
            }
            case PARRY_DISCOUNT -> {
                Fighter f = fighterOf(target, ctx);
                f.parryDiscount += e.value;
                emit(ev(EventType.PARRY_COST, f).value(Math.max(cfg.minParryCost, cfg.parryCost - f.parryDiscount)));
            }
            case REDUCE_MAX_HP -> {
                Fighter f = fighterOf(target, ctx);
                int v = Math.min(f.maxHp - 1, scale(e.value, ctx));
                f.maxHp -= v;
                f.hp = Math.min(f.hp, f.maxHp);
                emit(ev(EventType.MAX_HP, f).value(-v).state(f));
            }
            case APPLY -> apply(e, ctx, target);
            case CHARGE -> {
                for (ItemInstance it : itemsOf(target, ctx)) {
                    it.progressMs = Math.min(it.cooldownMs, it.progressMs + e.durationMs());
                    emit(ev(EventType.CHARGE, it.owner).item(it.uid).value(e.durationMs()));
                }
            }
            case ADD_ATTACK -> {
                int v = scale(e.value, ctx);
                for (ItemInstance it : itemsOf(target, ctx)) {
                    it.attack += v;
                    emit(ev(EventType.BUFF, it.owner).item(it.uid).kind("ATTACK").value(v));
                }
            }
            case ADD_CRIT -> {
                for (ItemInstance it : itemsOf(target, ctx)) {
                    it.critPermille += e.pctPermille();
                    emit(ev(EventType.BUFF, it.owner).item(it.uid).kind("CRIT").value(e.pctPermille()));
                }
            }
            case BUFF_STACK -> buffStack(e, ctx, target, effectIndex);
            case CLEANSE -> cleanse(e, ctx);
            case RAISE_MAX_HP -> {
                Fighter f = fighterOf(target, ctx);
                int v = scale(e.value, ctx);
                f.maxHp += v;
                f.hp += v;
                emit(ev(EventType.MAX_HP, f).item(ctx.source.uid).kind("RAISE").value(v).state(f));
            }
        }
    }

    private int damage(EffectDef e, Ctx ctx, Fighter target) {
        ItemInstance src = ctx.source;
        int dealt = 0;
        for (int h = 0; h < e.hits; h++) {
            int base = e.value > 0 ? scale(e.value, ctx) : src.attack * e.pctPermille() / 1000;
            boolean crit = rng.chance(src.critPermille);
            if (crit) base *= 2;
            int amount = base * (1000 - target.dmgReducePermille) / 1000;
            dealt += loseHp(target, amount, e.ignoreShield, EventType.DAMAGE, null, src, crit, ctx.depth);
            fire(TriggerEvent.HIT, ctx.owner, src, null, ctx.depth);
            if (crit) fire(TriggerEvent.CRIT, ctx.owner, src, null, ctx.depth);
        }
        return dealt;
    }

    private void buffStack(EffectDef e, Ctx ctx, Target target, int effectIndex) {
        String key = ctx.source.uid + ":" + ctx.stackKey;
        if ("dmg_reduce".equals(e.stat)) {
            Fighter f = fighterOf(target, ctx);
            int count = f.buffStacks.getOrDefault(key, 0);
            if (count >= e.maxStacks) return;
            f.buffStacks.put(key, count + 1);
            f.dmgReducePermille = Math.min(cfg.maxDmgReducePermille, f.dmgReducePermille + e.pctPermille());
            emit(ev(EventType.BUFF, f).item(ctx.source.uid).kind("DMG_REDUCE").value(e.pctPermille()).total(count + 1).state(f));
            return;
        }
        for (ItemInstance it : itemsOf(target, ctx)) {
            int count = it.buffStacks.getOrDefault(key, 0);
            if (count >= e.maxStacks) continue;
            it.buffStacks.put(key, count + 1);
            int value;
            if ("attack".equals(e.stat)) {
                value = scale(e.value, ctx);
                it.attack += value;
            } else {
                value = e.pctPermille();
                it.critPermille += value;
            }
            emit(ev(EventType.BUFF, it.owner).item(it.uid).kind(e.stat.toUpperCase()).value(value).total(count + 1));
        }
    }

    private void cleanse(EffectDef e, Ctx ctx) {
        Fighter f = ctx.owner;
        if (e.status == null) {
            f.stunUntil = Math.min(f.stunUntil, now - 1);
            for (ItemInstance it : f.items) {
                it.slowUntil = Math.min(it.slowUntil, now - 1);
                it.sealUntil = Math.min(it.sealUntil, now - 1);
            }
            emit(ev(EventType.STATUS, f).item(ctx.source.uid).kind("CLEANSE_CONTROL").value(0).state(f));
            return;
        }
        int before = meter(f, e.status);
        int removed = e.value == 0 ? before : Math.min(before, e.value);
        setMeter(f, e.status, before - removed);
        emit(ev(EventType.STATUS, f).item(ctx.source.uid).kind("CLEANSE_" + e.status.name()).value(-removed).total(before - removed).state(f));
    }

    private void apply(EffectDef e, Ctx ctx, Target target) {
        StatusType s = e.status;
        boolean applied = false;
        switch (s) {
            case BURN, POISON, BLEED -> {
                Fighter f = fighterOf(target, ctx);
                int points = scale(e.stacks, ctx);
                int meter = meter(f, s) + points;
                BattleConfig.Burst b = cfg.burst(s);
                boolean bursts = meter >= b.threshold;
                // Overflow past the threshold is discarded: the meter resets to 0.
                setMeter(f, s, bursts ? 0 : meter);
                emit(ev(EventType.STATUS, f).kind(s.name()).value(points).total(bursts ? b.threshold : meter).state(f));
                if (!ctx.suppressStatusApplied) {
                    fire(TriggerEvent.STATUS_APPLIED, ctx.owner, ctx.source, s, ctx.depth);
                }
                if (bursts) {
                    int dmg = b.damage + (int) ((long) f.maxHp * b.maxHpPermille / 1000);
                    loseHp(f, dmg, b.ignoreShield, EventType.BURST, s.name(), ctx.source, false, ctx.depth);
                    emit(ev(EventType.STATUS, f).kind(s.name()).value(-b.threshold).total(0).state(f));
                    fire(TriggerEvent.BURST, ctx.owner, ctx.source, s, ctx.depth);
                }
                return;
            }
            case REGEN -> {
                Fighter f = fighterOf(target, ctx);
                int stacks = scale(e.stacks, ctx);
                f.regen = Math.min(f.regen + stacks, cfg.maxRegen);
                emit(ev(EventType.STATUS, f).kind(s.name()).value(stacks).total(f.regen).state(f));
                applied = true;
            }
            case STUN -> {
                Fighter f = fighterOf(target, ctx);
                f.stunUntil = Math.max(f.stunUntil, now + e.durationMs());
                emit(ev(EventType.STATUS, f).kind(s.name()).value(e.durationMs()).state(f));
                applied = true;
                if (f != ctx.owner) fire(TriggerEvent.CONTROLLED, f, ctx.source, s, ctx.depth);
            }
            case SLOW, SEAL, HASTE -> {
                for (ItemInstance it : itemsOf(target, ctx)) {
                    int until = now + e.durationMs();
                    switch (s) {
                        case SLOW -> it.slowUntil = Math.max(it.slowUntil, until);
                        case SEAL -> it.sealUntil = Math.max(it.sealUntil, until);
                        default -> it.hasteUntil = Math.max(it.hasteUntil, until);
                    }
                    emit(ev(EventType.STATUS, it.owner).item(it.uid).kind(s.name()).value(e.durationMs()));
                    applied = true;
                    if ((s == StatusType.SLOW || s == StatusType.SEAL) && it.owner != ctx.owner) {
                        fire(TriggerEvent.CONTROLLED, it.owner, ctx.source, s, ctx.depth);
                    }
                }
            }
        }
        if (applied) {
            if (!ctx.suppressStatusApplied) {
                fire(TriggerEvent.STATUS_APPLIED, ctx.owner, ctx.source, s, ctx.depth);
            }
        }
    }

    // ---- state changes ----

    private int loseHp(Fighter f, int amount, boolean ignoreShield, EventType type, String kind,
                        ItemInstance src, boolean crit, int depth) {
        int hpBefore = Math.max(0, f.hp);
        int shieldBefore = f.shield;
        int absorbed = ignoreShield ? 0 : Math.min(f.shield, amount);
        f.shield -= absorbed;
        int rest = amount - absorbed;
        f.hp -= rest;
        BattleEvent ev = ev(type, f).value(amount).absorbed(absorbed);
        if (kind != null) ev.kind(kind);
        if (src != null) ev.item(src.uid);
        if (crit) ev.crit(true);
        emit(ev.state(f));
        if (shieldBefore > 0 && f.shield == 0 && src != null) {
            fire(TriggerEvent.SHIELD_BROKEN, src.owner, src, null, depth);
        }
        if (rest > 0) {
            fire(TriggerEvent.HP_BELOW, f, null, null, depth);
            if (src != null && src.owner != f && (type == EventType.BURST || (type == EventType.DAMAGE && src.isWeapon()))) {
                fire(TriggerEvent.DAMAGED, f, src, kind == null ? null : StatusType.valueOf(kind), depth);
            }
        }
        if (crit && src != null && src.owner != f) fire(TriggerEvent.CRIT_TAKEN, f, src, null, depth);
        if (type == EventType.BURST && src != null && src.owner != f) {
            fire(TriggerEvent.BURST_TAKEN, f, src, StatusType.valueOf(kind), depth);
        }
        return absorbed + Math.min(rest, hpBefore);
    }

    private void heal(Fighter f, int amount, ItemInstance src, int depth) {
        // Same rule as pills and Điều Tức: a fighter at 0 HP cannot be healed back within the tick.
        if (f.hp <= 0) return;
        int actual = Math.min(amount, f.maxHp - f.hp);
        if (actual <= 0) return;
        f.hp += actual;
        emit(ev(EventType.HEAL, f).value(actual).state(f));
        fire(TriggerEvent.HEALED, f, src, null, depth);
    }

    private void changeQi(Fighter f, int delta, ItemInstance src, int depth) {
        int before = f.qi;
        int next = Math.max(0, Math.min(cfg.maxQi, f.qi + delta));
        if (next == f.qi) return;
        int actual = next - f.qi;
        f.qi = next;
        emit(ev(EventType.QI, f).value(actual).state(f));
        if (before > 0 && next == 0) fire(TriggerEvent.QI_EMPTY, f, src, null, depth);
    }

    /** Pays an existing combat cost without adding a new log event (parity contract). */
    private void spendQi(Fighter f, int amount, ItemInstance src, int depth) {
        int before = f.qi;
        f.qi = Math.max(0, f.qi - amount);
        if (before > 0 && f.qi == 0) fire(TriggerEvent.QI_EMPTY, f, src, null, depth);
    }

    private void checkDeath() {
        if (over) return;
        boolean down0 = fighters[0].hp <= 0;
        boolean down1 = fighters[1].hp <= 0;
        if (down0 && down1) finish(-1);
        else if (down0) finish(1);
        else if (down1) finish(0);
    }

    private void finish(int winnerSide) {
        over = true;
        winner = winnerSide;
        emit(new BattleEvent(now, EventType.END, winnerSide));
    }

    // ---- targets ----

    private static Fighter fighterOf(Target target, Ctx ctx) {
        return target == Target.ENEMY ? ctx.owner.enemy : ctx.owner;
    }

    private List<ItemInstance> itemsOf(Target target, Ctx ctx) {
        Fighter self = ctx.owner;
        Fighter enemy = self.enemy;
        return switch (target) {
            case SELF_ITEM -> List.of(ctx.source);
            case EVENT_SOURCE -> List.of(ctx.eventSource != null ? ctx.eventSource : ctx.source);
            case SELF_ALL_ITEMS -> active(self.items, false, null);
            case SELF_OTHER_ITEMS -> active(self.items, false, ctx.source);
            case SELF_ALL_WEAPONS -> active(self.items, true, null);
            case ADJACENT_ITEMS -> active(ctx.source.adjacent, false, null);
            case ADJACENT_WEAPONS -> active(ctx.source.adjacent, true, null);
            case ENEMY_ALL_ITEMS -> active(enemy.items, false, null);
            case ENEMY_RANDOM_ITEM -> pick(active(enemy.items, false, null));
            case ENEMY_RANDOM_WEAPON -> pick(active(enemy.items, true, null));
            default -> throw new IllegalStateException("Not an item target: " + target);
        };
    }

    private static List<ItemInstance> active(List<ItemInstance> items, boolean weaponsOnly, ItemInstance exclude) {
        List<ItemInstance> out = new ArrayList<>();
        for (ItemInstance it : items) {
            if (it.isActive() && it != exclude && (!weaponsOnly || it.isWeapon())) {
                out.add(it);
            }
        }
        return out;
    }

    private List<ItemInstance> pick(List<ItemInstance> candidates) {
        return candidates.isEmpty() ? List.of() : List.of(candidates.get(rng.nextInt(candidates.size())));
    }

    // ---- helpers ----

    private int scale(int value, Ctx ctx) {
        // Parity contract: apply tier, truncate, then rarity, truncate.
        return value * cfg.tierMultiplier(ctx.source.tier) / 1000 * cfg.rarityMultiplier(ctx.source.rarity) / 1000;
    }

    private BattleEvent ev(EventType type, Fighter f) {
        return new BattleEvent(now, type, f.side);
    }

    private void emit(BattleEvent e) {
        events.add(e);
    }
}
