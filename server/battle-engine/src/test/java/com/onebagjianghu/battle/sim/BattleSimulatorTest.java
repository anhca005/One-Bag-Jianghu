package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.EffectDef;
import com.onebagjianghu.battle.data.EffectOp;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.data.ItemType;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.data.StatusType;
import com.onebagjianghu.battle.data.Target;
import com.onebagjianghu.battle.data.TriggerEvent;
import com.onebagjianghu.battle.data.TriggerSource;
import com.onebagjianghu.battle.data.UnlockDef;
import org.junit.jupiter.api.Test;

import java.nio.file.Path;
import java.util.List;
import java.util.function.Predicate;

import static com.onebagjianghu.battle.sim.TestItems.damage;
import static com.onebagjianghu.battle.sim.TestItems.from;
import static com.onebagjianghu.battle.sim.TestItems.hpBelow;
import static com.onebagjianghu.battle.sim.TestItems.on;
import static com.onebagjianghu.battle.sim.TestItems.op;
import static com.onebagjianghu.battle.sim.TestItems.passive;
import static com.onebagjianghu.battle.sim.TestItems.selfDamage;
import static com.onebagjianghu.battle.sim.TestItems.stacks;
import static com.onebagjianghu.battle.sim.TestItems.timed;
import static com.onebagjianghu.battle.sim.TestItems.weapon;
import static com.onebagjianghu.battle.sim.TestItems.withTriggers;
import static com.onebagjianghu.battle.sim.TestItems.withUltimate;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BattleSimulatorTest {

    private static BattleResult fight(List<ItemDef> defs, Loadout a, Loadout b) {
        return new BattleSimulator(ItemCatalog.of(defs)).run(a, b, 42);
    }

    /** A target that never acts: no items, lots of HP. */
    private static Loadout dummy(int hp) {
        return new Loadout("dummy", hp, 5, 4);
    }

    private static List<BattleEvent> events(BattleResult r, EventType type, int side) {
        return r.events().stream().filter(e -> e.type == type && e.side == side).toList();
    }

    private static BattleEvent first(BattleResult r, Predicate<BattleEvent> p) {
        return r.events().stream().filter(p).findFirst().orElseThrow();
    }

    @Test
    void sameSeedGivesIdenticalLog() {
        ItemCatalog catalog = ItemCatalog.loadDefault();
        BattleSimulator sim = new BattleSimulator(catalog);
        Loadout a = Loadout.load(Path.of("samples/thai_thanh.json"));
        Loadout b = Loadout.load(Path.of("samples/liet_diem.json"));

        List<String> first = sim.run(a, b, 123).events().stream().map(Object::toString).toList();
        List<String> second = sim.run(a, b, 123).events().stream().map(Object::toString).toList();
        assertEquals(first, second);

        boolean anyDifferent = false;
        for (long seed = 1; seed <= 20 && !anyDifferent; seed++) {
            anyDifferent = !sim.run(a, b, seed).events().stream().map(Object::toString).toList().equals(first);
        }
        assertTrue(anyDifferent, "different seeds should eventually change crit rolls");
    }

    @Test
    void weaponHitsOnCooldown() {
        ItemDef sword = weapon("sword", 10, 1.0);
        BattleResult r = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0), dummy(1000));

        List<BattleEvent> hits = events(r, EventType.DAMAGE, 1);
        assertEquals(1000, hits.get(0).t);
        assertEquals(2000, hits.get(1).t);
        assertEquals(10, hits.get(0).value);
        assertEquals(990, hits.get(0).hp);
    }

    @Test
    void shieldAbsorbsWeaponDamage() {
        ItemDef armor = passive("armor", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, op(EffectOp.SHIELD, 50)));
        ItemDef sword = weapon("sword", 30, 1.0);
        BattleResult r = fight(List.of(armor, sword),
                new Loadout("a", 100, 5, 4).add("sword", 0, 0),
                new Loadout("b", 1000, 5, 4).add("armor", 0, 0));

        BattleEvent hit = first(r, e -> e.type == EventType.DAMAGE && e.side == 1);
        assertEquals(30, hit.absorbed);
        assertEquals(1000, hit.hp);
        assertEquals(20, hit.shield);
    }

    @Test
    void shieldGainIsCappedAtCurrentMaxHpAndReportsActualGain() {
        ItemDef armor = passive("armor", ItemType.ARMOR, on(TriggerEvent.BATTLE_START,
                op(EffectOp.SHIELD, 800), op(EffectOp.SHIELD, 500), op(EffectOp.SHIELD, 100)));
        BattleResult r = fight(List.of(armor),
                new Loadout("a", 1000, 5, 4).add("armor", 0, 0), dummy(1000));

        List<BattleEvent> shields = events(r, EventType.SHIELD, 0);
        assertEquals(List.of(800, 200, 0), shields.stream().map(e -> e.value).toList());
        assertEquals(List.of(800, 1000, 1000), shields.stream().map(e -> e.shield).toList());
    }

    @Test
    void statusBuildsUpSilentlyThenBurstsAndResets() {
        ItemDef armor = passive("armor", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, op(EffectOp.SHIELD, 500)));
        ItemDef needle = weapon("needle", 1, 1.0, stacks(StatusType.POISON, 20));
        BattleResult r = fight(List.of(armor, needle),
                new Loadout("a", 100, 5, 4).add("needle", 0, 0),
                new Loadout("b", 1000, 5, 4).add("armor", 0, 0));

        List<Integer> meter = events(r, EventType.STATUS, 1).stream()
                .filter(e -> "POISON".equals(e.kind)).map(e -> e.total).limit(5).toList();
        assertEquals(List.of(20, 40, 50, 0, 20), meter); // 60 ≥ 50 → burst, overflow discarded

        List<BattleEvent> bursts = events(r, EventType.BURST, 1);
        assertEquals(3000, bursts.get(0).t);
        int poisonBurst = BattleConfig.defaults().poison.damage;
        assertEquals(poisonBurst, bursts.get(0).value);
        assertEquals(0, bursts.get(0).absorbed, "poison ignores Cương Khí");
        assertEquals(1000 - poisonBurst, bursts.get(0).hp);
        // Kiệt Lực also emits DOT; only the window before it shows the meter stays silent.
        int exhaustion = BattleConfig.defaults().exhaustionStartMs;
        assertTrue(r.events().stream().noneMatch(e -> e.type == EventType.DOT && e.side == 1 && e.t < exhaustion),
                "no damage before the threshold");
    }

    @Test
    void burnBurstHitsShieldAndBleedBurstScalesWithMaxHp() {
        BattleConfig cfg = BattleConfig.defaults();
        ItemDef armor = passive("armor", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, op(EffectOp.SHIELD, 1000)));
        // A weapon (fires at 1s) rather than battle_start, so the target's shield is already up.
        ItemDef torch = weapon("torch", 1, 1.0, stacks(StatusType.BURN, 50), stacks(StatusType.BLEED, 50));
        BattleResult r = fight(List.of(armor, torch),
                new Loadout("a", 100, 5, 4).add("torch", 0, 0),
                new Loadout("b", 2000, 5, 4).add("armor", 0, 0));

        BattleEvent burn = first(r, e -> e.type == EventType.BURST && "BURN".equals(e.kind));
        assertEquals(cfg.burn.damage, burn.absorbed);
        BattleEvent bleed = first(r, e -> e.type == EventType.BURST && "BLEED".equals(e.kind));
        assertEquals(cfg.bleed.damage + 2000 * cfg.bleed.maxHpPermille / 1000, bleed.value);
        assertEquals(0, bleed.absorbed);
    }

    @Test
    void bothSidesHitInTheSameTickSoDoubleKnockoutIsDraw() {
        ItemDef sword = weapon("sword", 100, 1.0);
        for (long seed = 1; seed <= 10; seed++) {
            BattleResult r = new BattleSimulator(ItemCatalog.of(List.of(sword))).run(
                    new Loadout("a", 100, 5, 4).add("sword", 0, 0),
                    new Loadout("b", 100, 5, 4).add("sword", 0, 0), seed);
            assertTrue(r.isDraw(), "seed " + seed);
            assertEquals(1000, r.durationMs());
        }
    }

    @Test
    void stunCannotCancelAnAttackReleasedInTheSameTick() {
        ItemDef stunner = weapon("stunner", 1, 1.0, timed(StatusType.STUN, 5, Target.ENEMY));
        ItemDef sword = weapon("sword", 10, 1.0);
        for (long seed = 1; seed <= 10; seed++) {
            BattleResult r = new BattleSimulator(ItemCatalog.of(List.of(stunner, sword))).run(
                    new Loadout("a", 1000, 5, 4).add("stunner", 0, 0),
                    new Loadout("b", 1000, 5, 4).add("sword", 0, 0), seed);
            List<BattleEvent> swordHits = events(r, EventType.DAMAGE, 0);
            assertEquals(1000, swordHits.get(0).t, "seed " + seed);
        }
    }

    @Test
    void parryCancelsUltimateAndStunsAttacker() {
        ItemDef blade = withTriggers(
                withUltimate(weapon("blade", 100, 1.0), 30, damage(5.0)),
                on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 30)));
        ItemDef manual = passive("manual", ItemType.MANUAL, on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 60)));
        BattleResult r = fight(List.of(blade, manual),
                new Loadout("a", 1000, 5, 4).add("blade", 0, 0),
                new Loadout("b", 1000, 5, 4).add("manual", 0, 0));

        BattleEvent parry = first(r, e -> e.type == EventType.PARRY);
        assertEquals(1000, parry.t);
        assertEquals(1, parry.side);
        assertEquals(0, parry.qi);
        // No 500-damage ultimate landed; the next swing waits for the stun, then a full cooldown.
        List<BattleEvent> hits = events(r, EventType.DAMAGE, 1);
        assertEquals(1000 + BattleConfig.defaults().parryStunMs + 1000, hits.get(0).t);
        assertEquals(100, hits.get(0).value);
    }

    @Test
    void shieldBreakStripsShieldBeforeTheHit() {
        ItemDef armor = passive("armor", ItemType.ARMOR, on(TriggerEvent.BATTLE_START, op(EffectOp.SHIELD, 100)));
        ItemDef axe = weapon("axe", 50, 1.0, op(EffectOp.SHIELD_BREAK, 80), damage(1.0));
        BattleResult r = fight(List.of(armor, axe),
                new Loadout("a", 100, 5, 4).add("axe", 0, 0),
                new Loadout("b", 1000, 5, 4).add("armor", 0, 0));

        BattleEvent hit = first(r, e -> e.type == EventType.DAMAGE && e.side == 1);
        assertEquals(20, hit.absorbed); // 100 shield - 80 broken
        assertEquals(970, hit.hp);
    }

    @Test
    void multiHitDamageLandsEveryHit() {
        EffectDef triple = damage(0.5);
        triple.hits = 3;
        ItemDef sword = weapon("sword", 40, 1.0, triple);
        BattleResult r = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0), dummy(1000));

        List<Integer> hits = events(r, EventType.DAMAGE, 1).stream().filter(e -> e.t == 1000).map(e -> e.value).toList();
        assertEquals(List.of(20, 20, 20), hits);
    }

    @Test
    void regenHealsOncePerSecond() {
        ItemDef monk = passive("monk", ItemType.MANUAL, on(TriggerEvent.BATTLE_START,
                selfDamage(50), stacks(StatusType.REGEN, 3)));
        BattleResult r = fight(List.of(monk), new Loadout("a", 100, 5, 4).add("monk", 0, 0), dummy(1000));

        List<BattleEvent> heals = events(r, EventType.HEAL, 0);
        assertEquals(1000, heals.get(0).t);
        assertEquals(3, heals.get(0).value);
        assertEquals(2000, heals.get(1).t);
        assertEquals(56, heals.get(1).hp);
    }

    @Test
    void slowHalvesAndChargeAdvancesCooldown() {
        ItemDef slowed = withTriggers(weapon("slowed", 1, 1.0),
                on(TriggerEvent.BATTLE_START, timed(StatusType.SLOW, 10, Target.SELF_ITEM)));
        BattleResult slow = fight(List.of(slowed), new Loadout("a", 100, 5, 4).add("slowed", 0, 0), dummy(1000));
        assertEquals(2000, events(slow, EventType.DAMAGE, 1).get(0).t);

        EffectDef charge = new EffectDef();
        charge.op = EffectOp.CHARGE;
        charge.duration = 0.5;
        ItemDef charged = withTriggers(weapon("charged", 1, 1.0), on(TriggerEvent.BATTLE_START, charge));
        BattleResult fast = fight(List.of(charged), new Loadout("a", 100, 5, 4).add("charged", 0, 0), dummy(1000));
        assertEquals(500, events(fast, EventType.DAMAGE, 1).get(0).t);
    }

    @Test
    void parryCostAndDamageReductionHaveLimits() {
        BattleConfig cfg = BattleConfig.defaults();
        ItemDef blade = withTriggers(
                withUltimate(weapon("blade", 100, 1.0), 30, damage(5.0)),
                on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 30)));
        EffectDef bigReduce = new EffectDef();
        bigReduce.op = EffectOp.DMG_REDUCE;
        bigReduce.pct = 1.0;
        ItemDef manual = passive("manual", ItemType.MANUAL, on(TriggerEvent.BATTLE_START,
                op(EffectOp.GAIN_QI, 30), op(EffectOp.PARRY_DISCOUNT, 100), bigReduce, bigReduce));
        BattleResult r = fight(List.of(blade, manual),
                new Loadout("a", 1000, 5, 4).add("blade", 0, 0),
                new Loadout("b", 1000, 5, 4).add("manual", 0, 0));

        assertEquals(30 - cfg.minParryCost, first(r, e -> e.type == EventType.PARRY).qi);
        assertEquals(cfg.maxDmgReducePermille, events(r, EventType.DMG_REDUCE, 1).get(1).value);
        BattleEvent hit = first(r, e -> e.type == EventType.DAMAGE && e.side == 1);
        assertEquals(100 * (1000 - cfg.maxDmgReducePermille) / 1000, hit.value);
    }

    @Test
    void fighterAtZeroCannotBeHealedInTheSameTickAndLogNeverShowsNegativeHp() {
        ItemDef sword = weapon("sword", 100, 1.0);
        ItemDef herb = weapon("herb", 1, 1.0, op(EffectOp.HEAL, 200));
        for (long seed = 1; seed <= 10; seed++) {
            BattleResult r = new BattleSimulator(ItemCatalog.of(List.of(sword, herb))).run(
                    new Loadout("a", 1000, 5, 4).add("sword", 0, 0),
                    new Loadout("b", 60, 5, 4).add("herb", 0, 0), seed);
            assertEquals(0, r.winner(), "seed " + seed);
            assertTrue(r.events().stream().allMatch(e -> e.hp == null || e.hp >= 0), "seed " + seed);
        }
    }

    @Test
    void parryDiscountLowersParryCost() {
        ItemDef blade = withTriggers(
                withUltimate(weapon("blade", 100, 1.0), 30, damage(5.0)),
                on(TriggerEvent.BATTLE_START, op(EffectOp.GAIN_QI, 30)));
        ItemDef manual = passive("manual", ItemType.MANUAL, on(TriggerEvent.BATTLE_START,
                op(EffectOp.GAIN_QI, 45), op(EffectOp.PARRY_DISCOUNT, 20)));
        BattleResult r = fight(List.of(blade, manual),
                new Loadout("a", 1000, 5, 4).add("blade", 0, 0),
                new Loadout("b", 1000, 5, 4).add("manual", 0, 0));

        BattleEvent parry = first(r, e -> e.type == EventType.PARRY);
        assertEquals(5, parry.qi); // 45 - (60 - 20)
    }

    @Test
    void hasteDoublesAndSealPausesCooldown() {
        ItemDef quick = withTriggers(weapon("quick", 1, 1.0),
                on(TriggerEvent.BATTLE_START, timed(StatusType.HASTE, 10, Target.SELF_ITEM)));
        BattleResult hasted = fight(List.of(quick), new Loadout("a", 100, 5, 4).add("quick", 0, 0), dummy(1000));
        assertEquals(500, events(hasted, EventType.DAMAGE, 1).get(0).t);

        ItemDef sealer = passive("sealer", ItemType.RELIC,
                on(TriggerEvent.BATTLE_START, timed(StatusType.SEAL, 3, Target.ENEMY_ALL_ITEMS)));
        ItemDef plain = weapon("plain", 1, 1.0);
        BattleResult sealed = fight(List.of(sealer, plain),
                new Loadout("a", 100, 5, 4).add("sealer", 0, 0),
                new Loadout("b", 1000, 5, 4).add("plain", 0, 0));
        assertEquals(4000, events(sealed, EventType.DAMAGE, 0).get(0).t);
    }

    @Test
    void pillHealsOnceWhenBelowThreshold() {
        ItemDef pill = passive("pill", ItemType.PILL, hpBelow(0.5, 1, op(EffectOp.HEAL, 30)));
        ItemDef sword = weapon("sword", 30, 1.0);
        BattleResult r = fight(List.of(pill, sword),
                new Loadout("a", 1000, 5, 4).add("sword", 0, 0),
                new Loadout("b", 100, 5, 4).add("pill", 0, 0));

        List<BattleEvent> heals = events(r, EventType.HEAL, 1);
        assertEquals(1, heals.size());
        assertEquals(2000, heals.get(0).t); // 100 → 70 → 40 (< 50%) → heal to 70
        assertEquals(70, heals.get(0).hp);
        assertEquals(0, r.winner());
    }

    @Test
    void adjacentTriggerOnlyFiresForNeighbours() {
        ItemDef sword = weapon("sword", 10, 1.0);
        ItemDef relic = passive("relic", ItemType.RELIC,
                from(on(TriggerEvent.HIT, op(EffectOp.GAIN_QI, 5)), TriggerSource.ADJACENT));
        List<ItemDef> defs = List.of(sword, relic);

        BattleResult near = fight(defs, new Loadout("a", 100, 5, 4).add("sword", 0, 0).add("relic", 1, 0), dummy(1000));
        BattleResult far = fight(defs, new Loadout("a", 100, 5, 4).add("sword", 0, 0).add("relic", 1, 2), dummy(1000));

        assertFalse(events(near, EventType.QI, 0).isEmpty());
        assertTrue(events(far, EventType.QI, 0).isEmpty(), "diagonal is not adjacent");
    }

    @Test
    void triggerChainsStopAtMaxDepth() {
        // Every hit triggers another hit: would loop forever without the depth limit.
        ItemDef echo = withTriggers(weapon("echo", 1, 1.0),
                from(on(TriggerEvent.HIT, damage(1.0)), TriggerSource.SELF));
        BattleResult r = fight(List.of(echo), new Loadout("a", 100, 5, 4).add("echo", 0, 0), dummy(100_000));

        long hitsAtFirstSecond = events(r, EventType.DAMAGE, 1).stream().filter(e -> e.t == 1000).count();
        assertEquals(1 + BattleConfig.defaults().maxTriggerDepth, hitsAtFirstSecond);
    }

    @Test
    void exhaustionEndsStalemateAsDraw() {
        BattleResult r = fight(List.of(), dummy(1000), dummy(1000));
        assertTrue(r.isDraw());
        assertTrue(r.durationMs() < BattleConfig.defaults().maxDurationMs);
        assertEquals(EventType.END, r.events().get(r.events().size() - 1).type);
    }

    @Test
    void higherTierScalesAttack() {
        ItemDef sword = weapon("sword", 100, 1.0);
        BattleResult r = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0, 3), dummy(10_000));
        assertEquals(110, events(r, EventType.DAMAGE, 1).get(0).value);
    }

    @Test
    void rarityScalesAttackShieldHealAndStacksButNotQiOrDuration() {
        ItemDef item = weapon("rare", 100, 1.0,
                op(EffectOp.SHIELD, 20), selfDamage(100), op(EffectOp.HEAL, 20),
                stacks(StatusType.POISON, 10), op(EffectOp.GAIN_QI, 7),
                timed(StatusType.HASTE, 2, Target.SELF_ITEM), damage(1.0));
        Loadout a = new Loadout("a", 1000, 5, 4).add("rare", 0, 0);
        a.items.get(0).rarity = Rarity.SUPREME;
        BattleResult r = fight(List.of(item), a, dummy(10000));
        assertEquals(160, first(r, e -> e.type == EventType.DAMAGE && e.side == 1).value);
        assertEquals(32, first(r, e -> e.type == EventType.SHIELD).value);
        assertEquals(32, first(r, e -> e.type == EventType.HEAL).value);
        assertEquals(16, first(r, e -> e.type == EventType.STATUS && "POISON".equals(e.kind)).value);
        assertEquals(7, first(r, e -> e.type == EventType.QI).value);
        assertEquals(2000, first(r, e -> e.type == EventType.STATUS && "HASTE".equals(e.kind)).value);
    }

    @Test
    void rejectsOverlappingItems() {
        ItemDef sword = TestItems.size(weapon("sword", 10, 1.0), 1, 3);
        Loadout bad = new Loadout("bad", 100, 5, 4).add("sword", 0, 0).add("sword", 0, 2);
        DataException e = assertThrows(DataException.class, () -> fight(List.of(sword), bad, dummy(100)));
        assertTrue(e.getMessage().contains("overlaps"));
    }

    @Test
    void sampleLoadoutsAreValidAndFinish() {
        ItemCatalog catalog = ItemCatalog.loadDefault();
        BattleSimulator sim = new BattleSimulator(catalog);
        List<Loadout> samples = List.of("kim_cang", "thai_thanh", "liet_diem", "huyen_doc", "thiet_phu").stream()
                .map(n -> Loadout.load(Path.of("samples/" + n + ".json"))).toList();
        for (Loadout a : samples) {
            for (Loadout b : samples) {
                BattleResult r = sim.run(a, b, 7);
                assertNotEquals(0, r.durationMs());
                assertEquals(EventType.END, r.events().get(r.events().size() - 1).type);
            }
        }
    }

    @Test
    void unlocksApplyFromTheirTier() {
        UnlockDef qi = new UnlockDef();
        qi.tier = 3;
        qi.basic = List.of(op(EffectOp.GAIN_QI, 5));
        ItemDef sword = weapon("sword", 10, 1.0);
        sword.unlocks = List.of(qi);

        BattleResult t2 = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0, 2), dummy(100000));
        BattleResult t3 = fight(List.of(sword), new Loadout("a", 100, 5, 4).add("sword", 0, 0, 3), dummy(100000));
        assertTrue(events(t2, EventType.QI, 0).isEmpty());
        BattleEvent firstQi = events(t3, EventType.QI, 0).get(0);
        assertEquals(1000, firstQi.t);
        // Qi does not scale with tier, so the unlock gives exactly its listed value.
        assertEquals(5, firstQi.qi);
    }

    @Test
    void tamPhapBuffsOnlyItsWeaponClasses() {
        ItemDef sword = weapon("sword", 100, 1.0);
        sword.weaponClass = "sword";
        ItemDef axe = weapon("axe", 100, 1.0);
        axe.weaponClass = "axe";
        com.onebagjianghu.battle.data.SectDef sect = new com.onebagjianghu.battle.data.SectDef();
        sect.id = "s";
        sect.name = "S";
        sect.tamPhap = "T";
        sect.weaponClasses = List.of("sword");
        com.onebagjianghu.battle.data.TamPhapLevelDef l1 = new com.onebagjianghu.battle.data.TamPhapLevelDef();
        l1.level = 1;
        l1.attackPct = 0.5;
        sect.levels = List.of(l1);
        BattleSimulator sim = new BattleSimulator(ItemCatalog.of(List.of(sword, axe)),
                com.onebagjianghu.battle.data.SectCatalog.of(List.of(sect)), BattleConfig.defaults());

        Loadout a = new Loadout("a", 100, 5, 4).add("sword", 0, 0).add("axe", 1, 0);
        a.sect = "s";
        BattleResult r = sim.run(a, dummy(100000), 42);
        BattleEvent swordHit = first(r, e -> e.type == EventType.DAMAGE && e.item != null && e.item == 0 && !Boolean.TRUE.equals(e.crit));
        BattleEvent axeHit = first(r, e -> e.type == EventType.DAMAGE && e.item != null && e.item == 1 && !Boolean.TRUE.equals(e.crit));
        assertEquals(150, swordHit.value);
        assertEquals(100, axeHit.value);

        Loadout bad = new Loadout("b", 100, 5, 4).add("sword", 0, 0);
        bad.sect = "nope";
        assertThrows(DataException.class, () -> sim.run(bad, dummy(100), 1));
    }
}
