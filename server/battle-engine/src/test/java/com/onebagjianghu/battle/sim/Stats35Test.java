package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.*;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

import static com.onebagjianghu.battle.sim.TestItems.*;
import static org.junit.jupiter.api.Assertions.*;

class Stats35Test {
    private static ItemDef stats(String id, int hp, double dmgReduce, double cdr) {
        ItemDef d = passive(id, ItemType.ARMOR);
        d.stats.hp = hp;
        d.stats.dmgReduce = dmgReduce;
        d.stats.cooldownReduction = cdr;
        return d;
    }

    private static Loadout loadout(int level, String id, int tier, Rarity rarity) {
        Loadout l = new Loadout();
        l.name = "hero";
        l.level = level;
        l.items.add(new Loadout.PlacedItem(id, 0, 0));
        l.items.get(0).tier = tier;
        l.items.get(0).rarity = rarity;
        return l;
    }

    @Test
    void maxHpUsesLevelAndScaledItemHpWithStepwiseTruncation() {
        BattleSimulator plain = new BattleSimulator(ItemCatalog.of(List.of()));
        assertEquals(100, plain.fighterStats(new Loadout()).maxHp());
        Loadout level10 = new Loadout();
        level10.level = 10;
        assertEquals(145, plain.fighterStats(level10).maxHp());

        ItemDef armor = stats("armor", 101, 0, 0);
        BattleSimulator sim = new BattleSimulator(ItemCatalog.of(List.of(armor)));
        assertEquals(201, sim.fighterStats(loadout(1, "armor", 1, Rarity.COMMON)).maxHp());
        assertEquals(306, sim.fighterStats(loadout(10, "armor", 1, Rarity.SUPREME)).maxHp());
        // 101 * 1400 / 1000 = 141, then 141 * 1600 / 1000 = 225.
        assertEquals(370, sim.fighterStats(loadout(10, "armor", 9, Rarity.SUPREME)).maxHp());
    }

    @Test
    void startingDamageReductionCapsAndTemporaryReductionStacksToSameCap() {
        ItemDef defense = stats("defense", 0, .7, 0);
        defense.triggers = List.of(on(TriggerEvent.BATTLE_START, pct(EffectOp.DMG_REDUCE, .2)));
        Loadout a = loadout(1, "defense", 1, Rarity.COMMON);
        BattleSimulator sim = new BattleSimulator(ItemCatalog.of(List.of(defense)));
        assertEquals(700, sim.fighterStats(a).dmgReducePermille());
        BattleResult result = sim.run(a, new Loadout(), 1);
        BattleEvent event = result.events().stream().filter(e -> e.type == EventType.DMG_REDUCE).findFirst().orElseThrow();
        assertEquals(800, event.value);

        ItemDef capped = stats("capped", 0, 1, 0);
        assertEquals(800, new BattleSimulator(ItemCatalog.of(List.of(capped)))
                .fighterStats(loadout(1, "capped", 9, Rarity.SUPREME)).dmgReducePermille());
    }

    @Test
    void cooldownReductionAffectsWeaponRelicAndChargeAndIsCapped() {
        ItemDef weapon = weapon("weapon", 1, 1);
        ItemDef relic = passive("relic", ItemType.RELIC);
        relic.cooldown = 1;
        relic.active = List.of(op(EffectOp.GAIN_QI, 1));
        ItemDef manual = stats("manual", 0, 0, 1);
        manual.type = ItemType.MANUAL;
        Loadout a = new Loadout();
        a.items.add(new Loadout.PlacedItem("weapon", 0, 0));
        a.items.add(new Loadout.PlacedItem("relic", 1, 0));
        a.items.add(new Loadout.PlacedItem("manual", 2, 0));
        BattleSimulator sim = new BattleSimulator(ItemCatalog.of(List.of(weapon, relic, manual)));
        assertEquals(400, sim.fighterStats(a).cooldownReductionPermille());
        BattleResult normal = sim.run(a, new Loadout(), 1);
        assertEquals(600, normal.events().stream().filter(e -> e.type == EventType.DAMAGE).findFirst().orElseThrow().t);
        assertEquals(600, normal.events().stream().filter(e -> e.type == EventType.QI).findFirst().orElseThrow().t);

        manual.triggers = List.of(on(TriggerEvent.BATTLE_START, charge(.5)));
        BattleResult charged = new BattleSimulator(ItemCatalog.of(List.of(weapon, manual)))
                .run(new Loadout("a", 100, 5, 4).add("weapon", 0, 0).add("manual", 1, 0), new Loadout(), 1);
        assertEquals(100, charged.events().stream().filter(e -> e.type == EventType.DAMAGE).findFirst().orElseThrow().t);

        weapon.cooldown = .1;
        assertEquals(100, new BattleSimulator(ItemCatalog.of(List.of(weapon, manual)))
                .run(new Loadout("a", 100, 5, 4).add("weapon", 0, 0).add("manual", 1, 0), new Loadout(), 1)
                .events().stream().filter(e -> e.type == EventType.DAMAGE).findFirst().orElseThrow().t);
    }

    @Test
    void rejectsRemovedHpAndInvalidLevel(@TempDir Path temp) throws Exception {
        Path old = temp.resolve("old.json");
        Files.writeString(old, "{\"name\":\"Old\",\"hp\":1200,\"items\":[]}");
        DataException removed = assertThrows(DataException.class, () -> Loadout.load(old));
        assertTrue(removed.getMessage().contains("decision #35"));
        Loadout invalid = new Loadout();
        invalid.name = "Bad";
        invalid.level = 0;
        DataException level = assertThrows(DataException.class,
                () -> new BattleSimulator(ItemCatalog.of(List.of())).run(invalid, new Loadout(), 1));
        assertTrue(level.getMessage().contains("level must be >= 1"));
    }

    private static EffectDef pct(EffectOp op, double value) {
        EffectDef e = new EffectDef();
        e.op = op;
        e.pct = value;
        return e;
    }

    private static EffectDef charge(double seconds) {
        EffectDef e = new EffectDef();
        e.op = EffectOp.CHARGE;
        e.duration = seconds;
        e.target = Target.ADJACENT_ITEMS;
        return e;
    }
}
