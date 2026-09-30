package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.*;
import org.junit.jupiter.api.Test;

import java.util.List;

import static com.onebagjianghu.battle.sim.TestItems.*;
import static org.junit.jupiter.api.Assertions.*;

class ItemsV2MechanicsTest {
    private static BattleResult fight(List<ItemDef> defs, Loadout a, Loadout b) {
        return new BattleSimulator(ItemCatalog.of(defs)).run(a, b, 42);
    }

    private static ItemDef active(String id, double cooldown, EffectDef... effects) {
        ItemDef d = passive(id, ItemType.RELIC);
        d.cooldown = cooldown;
        d.active = List.of(effects);
        return d;
    }

    private static List<BattleEvent> kind(BattleResult r, EventType type, int side) {
        return r.events().stream().filter(e -> e.type == type && e.side == side).toList();
    }

    @Test
    void relicCooldownUsesSlowAndHasteLikeWeapon() {
        ItemDef relic = active("relic", 1, op(EffectOp.GAIN_QI, 1));
        ItemDef slow = passive("slow", ItemType.ARMOR,
                on(TriggerEvent.BATTLE_START, timed(StatusType.SLOW, 10, Target.ENEMY_RANDOM_ITEM)));
        BattleResult slowed = fight(List.of(relic, slow),
                new Loadout("a", 100, 5, 4).add("relic", 0, 0),
                new Loadout("b", 100, 5, 4).add("slow", 0, 0));
        assertEquals(2000, kind(slowed, EventType.QI, 0).get(0).t);

        ItemDef haste = passive("haste", ItemType.ARMOR,
                on(TriggerEvent.BATTLE_START, timed(StatusType.HASTE, 10, Target.ADJACENT_ITEMS)));
        BattleResult both = fight(List.of(relic, slow, haste),
                new Loadout("a", 100, 5, 4).add("relic", 0, 0).add("haste", 1, 0),
                new Loadout("b", 100, 5, 4).add("slow", 0, 0));
        assertEquals(1000, kind(both, EventType.QI, 0).get(0).t);
    }

    @Test
    void buffStackCapsAndSourcesRemainSeparate() {
        EffectDef stack = op(EffectOp.BUFF_STACK, 5);
        stack.stat = "attack";
        stack.maxStacks = 2;
        ItemDef a = withTriggers(weapon("a", 10, 1), from(on(TriggerEvent.HIT, stack), TriggerSource.SELF));
        EffectDef stack2 = op(EffectOp.BUFF_STACK, 5);
        stack2.stat = "attack";
        stack2.maxStacks = 2;
        ItemDef b = withTriggers(weapon("b", 10, 1), from(on(TriggerEvent.HIT, stack2), TriggerSource.SELF));
        BattleResult r = fight(List.of(a, b), new Loadout("x", 100, 5, 4).add("a", 0, 0).add("b", 1, 0),
                new Loadout("y", 10000, 5, 4));
        assertEquals(List.of(1, 1, 2, 2), kind(r, EventType.BUFF, 0).stream().limit(4).map(e -> e.total).toList());
        assertTrue(kind(r, EventType.BUFF, 0).stream().allMatch(e -> e.total <= 2));
    }

    @Test
    void cleanseSupportsControlAndBuildUpMeters() {
        EffectDef poison = stacks(StatusType.POISON, 20);
        poison.target = Target.SELF;
        EffectDef cleanPoison = op(EffectOp.CLEANSE, 7);
        cleanPoison.status = StatusType.POISON;
        EffectDef control = timed(StatusType.STUN, 10, Target.SELF);
        EffectDef cleanControl = op(EffectOp.CLEANSE, 0);
        ItemDef item = passive("cleanser", ItemType.PILL,
                on(TriggerEvent.BATTLE_START, poison, cleanPoison, control, cleanControl));
        BattleResult r = fight(List.of(item), new Loadout("a", 100, 5, 4).add("cleanser", 0, 0), new Loadout("b", 100, 5, 4));
        assertTrue(r.events().stream().anyMatch(e -> "CLEANSE_POISON".equals(e.kind) && e.total == 13));
        assertTrue(r.events().stream().anyMatch(e -> "CLEANSE_CONTROL".equals(e.kind)));
    }

    @Test
    void lifestealUsesActualDamageAndFiresHealedChain() {
        EffectDef steal = new EffectDef();
        steal.op = EffectOp.LIFESTEAL;
        steal.pct = .5;
        ItemDef sword = weapon("sword", 40, 1, damage(1), steal);
        ItemDef listener = passive("listener", ItemType.PILL, on(TriggerEvent.HEALED, op(EffectOp.GAIN_QI, 3)));
        ItemDef wound = passive("wound", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, selfDamage(30)));
        BattleResult r = fight(List.of(sword, listener, wound),
                new Loadout("a", 100, 5, 4).add("sword", 0, 0).add("listener", 1, 0).add("wound", 2, 0),
                new Loadout("b", 15, 5, 4));
        assertEquals(7, kind(r, EventType.HEAL, 0).get(0).value);
        assertEquals(3, kind(r, EventType.QI, 0).get(0).value);
    }

    @Test
    void raiseMaxHpRaisesCurrentHpBySameScaledValue() {
        ItemDef pill = passive("pill", ItemType.PILL, on(TriggerEvent.BATTLE_START, op(EffectOp.RAISE_MAX_HP, 25)));
        BattleEvent e = fight(List.of(pill), new Loadout("a", 100, 5, 4).add("pill", 0, 0), new Loadout("b", 100, 5, 4))
                .events().stream().filter(x -> x.type == EventType.MAX_HP).findFirst().orElseThrow();
        assertEquals(125, e.hp);
        assertEquals(125, e.maxHp);
    }

    @Test
    void minQiAfterTrueFalseAndParried() {
        EffectDef gated = op(EffectOp.SHIELD, 10);
        gated.minQiAfter = 20;
        ItemDef sword = withUltimate(weapon("sword", 1, 1), 30, gated);
        sword = withTriggers(sword, on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 50)));
        BattleResult yes = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0), new Loadout("b", 1000, 5, 4));
        assertFalse(kind(yes, EventType.SHIELD, 0).isEmpty());
        gated.minQiAfter = 21;
        BattleResult no = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0), new Loadout("b", 1000, 5, 4));
        assertTrue(kind(no, EventType.SHIELD, 0).isEmpty());
        ItemDef parryQi = passive("parry", ItemType.PILL, on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 60)));
        gated.minQiAfter = 0;
        BattleResult parried = fight(List.of(sword, parryQi), new Loadout("a", 100, 5, 4).add("sword", 0, 0),
                new Loadout("b", 1000, 5, 4).add("parry", 0, 0));
        assertTrue(kind(parried, EventType.SHIELD, 0).isEmpty());
    }

    @Test
    void hitsRollCritAndFireHitAndCritPerHit() {
        EffectDef triple = damage(1);
        triple.hits = 3;
        ItemDef sword = weapon("sword", 10, 1, triple);
        sword.crit = 1;
        sword = withTriggers(sword, from(on(TriggerEvent.HIT, op(EffectOp.GAIN_QI, 1)), TriggerSource.SELF),
                from(on(TriggerEvent.CRIT, op(EffectOp.GAIN_QI, 1)), TriggerSource.SELF));
        BattleResult r = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0), new Loadout("b", 50, 5, 4));
        assertEquals(3, kind(r, EventType.DAMAGE, 1).size());
        assertTrue(kind(r, EventType.DAMAGE, 1).stream().allMatch(e -> Boolean.TRUE.equals(e.crit)));
        assertEquals(6, kind(r, EventType.QI, 0).size());
    }

    @Test
    void everyNewCauseTriggerFiresExactlyOnce() {
        ItemDef sword = weapon("sword", 100, 1);
        sword.crit = 1;
        ItemDef hurt = passive("hurt", ItemType.ARMOR,
                from(on(TriggerEvent.DAMAGED, op(EffectOp.GAIN_QI, 1)), TriggerSource.ENEMY),
                from(on(TriggerEvent.CRIT_TAKEN, op(EffectOp.GAIN_QI, 2)), TriggerSource.ENEMY));
        BattleResult hit = fight(List.of(sword, hurt), new Loadout("a", 100, 5, 4).add("sword", 0, 0),
                new Loadout("b", 100, 5, 4).add("hurt", 0, 0));
        assertEquals(List.of(1, 2), kind(hit, EventType.QI, 1).stream().map(e -> e.value).toList());

        ItemDef controller = passive("controller", ItemType.ARMOR,
                on(TriggerEvent.BATTLE_START, timed(StatusType.STUN, 1, Target.ENEMY)));
        ItemDef controlled = passive("controlled", ItemType.PILL,
                from(on(TriggerEvent.CONTROLLED, op(EffectOp.GAIN_QI, 3)), TriggerSource.ENEMY));
        BattleResult ctl = fight(List.of(controller, controlled), new Loadout("a", 100, 5, 4).add("controller", 0, 0),
                new Loadout("b", 100, 5, 4).add("controlled", 0, 0));
        assertEquals(List.of(3), kind(ctl, EventType.QI, 1).stream().map(e -> e.value).toList());

        EffectDef drainSelf = op(EffectOp.DRAIN_QI, 5);
        drainSelf.target = Target.SELF;
        ItemDef empty = passive("empty", ItemType.PILL,
                on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 5), drainSelf),
                on(TriggerEvent.QI_EMPTY, op(EffectOp.SHIELD, 4)));
        BattleResult qi = fight(List.of(empty), new Loadout("a", 100, 5, 4).add("empty", 0, 0), new Loadout("b", 100, 5, 4));
        assertEquals(List.of(4), kind(qi, EventType.SHIELD, 0).stream().map(e -> e.value).toList());

        ItemDef shield = passive("shield", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, op(EffectOp.SHIELD, 10)));
        ItemDef breaker = withTriggers(weapon("breaker", 100, 1),
                from(on(TriggerEvent.SHIELD_BROKEN, op(EffectOp.GAIN_QI, 6)), TriggerSource.SELF));
        BattleResult broken = fight(List.of(shield, breaker), new Loadout("a", 100, 5, 4).add("breaker", 0, 0),
                new Loadout("b", 50, 5, 4).add("shield", 0, 0));
        assertEquals(List.of(6), kind(broken, EventType.QI, 0).stream().map(e -> e.value).toList());

        ItemDef poisoner = weapon("poisoner", 1, 1, stacks(StatusType.POISON, 50));
        TriggerDef burstTaken = from(on(TriggerEvent.BURST_TAKEN, op(EffectOp.GAIN_QI, 7)), TriggerSource.ENEMY);
        burstTaken.status = StatusType.POISON;
        ItemDef antidote = passive("antidote", ItemType.PILL, burstTaken);
        BattleResult burst = fight(List.of(poisoner, antidote), new Loadout("a", 100, 5, 4).add("poisoner", 0, 0),
                new Loadout("b", 1000, 5, 4).add("antidote", 0, 0));
        assertEquals(7, kind(burst, EventType.QI, 1).get(0).value);

        ItemDef ult = withUltimate(weapon("ult", 1, 1), 30, op(EffectOp.DAMAGE, 5));
        ult = withTriggers(ult, on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 30)));
        ItemDef watcher = passive("watcher", ItemType.PILL,
                from(on(TriggerEvent.ENEMY_ULTIMATE, op(EffectOp.SHIELD, 8)), TriggerSource.ENEMY),
                on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 60)));
        BattleResult enemyUlt = fight(List.of(ult, watcher), new Loadout("a", 100, 5, 4).add("ult", 0, 0),
                new Loadout("b", 100, 5, 4).add("watcher", 0, 0));
        assertEquals(List.of(8), kind(enemyUlt, EventType.SHIELD, 1).stream().map(e -> e.value).toList());
        assertEquals(1, kind(enemyUlt, EventType.PARRY, 1).size(), "parried ultimate still counts");
    }
}
