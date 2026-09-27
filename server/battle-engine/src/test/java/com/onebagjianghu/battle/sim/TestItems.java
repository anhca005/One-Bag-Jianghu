package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.EffectDef;
import com.onebagjianghu.battle.data.EffectOp;
import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.data.ItemType;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.data.StatusType;
import com.onebagjianghu.battle.data.Target;
import com.onebagjianghu.battle.data.TriggerDef;
import com.onebagjianghu.battle.data.TriggerEvent;
import com.onebagjianghu.battle.data.TriggerSource;
import com.onebagjianghu.battle.data.UltimateDef;

import java.util.ArrayList;
import java.util.List;

/** Small builders so tests can describe items in code instead of JSON. */
final class TestItems {
    private TestItems() {
    }

    static ItemDef weapon(String id, int attack, double cooldown, EffectDef... basic) {
        ItemDef d = new ItemDef();
        d.id = id;
        d.name = id;
        d.type = ItemType.WEAPON;
        d.rarity = Rarity.COMMON;
        d.size = new int[] {1, 2};
        d.attack = attack;
        d.cooldown = cooldown;
        d.basic = basic.length == 0 ? List.of(damage(1.0)) : List.of(basic);
        return d;
    }

    static ItemDef passive(String id, ItemType type, TriggerDef... triggers) {
        ItemDef d = new ItemDef();
        d.id = id;
        d.name = id;
        d.type = type;
        d.rarity = Rarity.COMMON;
        d.triggers = List.of(triggers);
        return d;
    }

    static ItemDef withUltimate(ItemDef d, int cost, EffectDef... effects) {
        UltimateDef u = new UltimateDef();
        u.cost = cost;
        u.effects = List.of(effects);
        d.ultimate = u;
        return d;
    }

    static ItemDef withTriggers(ItemDef d, TriggerDef... triggers) {
        List<TriggerDef> all = new ArrayList<>(d.triggers);
        all.addAll(List.of(triggers));
        d.triggers = all;
        return d;
    }

    static ItemDef size(ItemDef d, int w, int h) {
        d.size = new int[] {w, h};
        return d;
    }

    static EffectDef damage(double pct) {
        EffectDef e = new EffectDef();
        e.op = EffectOp.DAMAGE;
        e.pct = pct;
        return e;
    }

    static EffectDef op(EffectOp op, int value) {
        EffectDef e = new EffectDef();
        e.op = op;
        e.value = value;
        return e;
    }

    static EffectDef stacks(StatusType status, int stacks) {
        EffectDef e = new EffectDef();
        e.op = EffectOp.APPLY;
        e.status = status;
        e.stacks = stacks;
        return e;
    }

    static EffectDef timed(StatusType status, double seconds, Target target) {
        EffectDef e = new EffectDef();
        e.op = EffectOp.APPLY;
        e.status = status;
        e.duration = seconds;
        e.target = target;
        return e;
    }

    static TriggerDef on(TriggerEvent event, EffectDef... effects) {
        TriggerDef t = new TriggerDef();
        t.on = event;
        t.effects = List.of(effects);
        return t;
    }

    static TriggerDef from(TriggerDef t, TriggerSource source) {
        t.from = source;
        return t;
    }

    static TriggerDef hpBelow(double fraction, int maxUses, EffectDef... effects) {
        TriggerDef t = on(TriggerEvent.HP_BELOW, effects);
        t.hpBelow = fraction;
        t.maxUses = maxUses;
        return t;
    }
}
