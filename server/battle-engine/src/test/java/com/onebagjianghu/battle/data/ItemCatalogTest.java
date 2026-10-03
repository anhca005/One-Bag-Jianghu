package com.onebagjianghu.battle.data;

import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ItemCatalogTest {

    @Test
    void loadsNormalizedItemsV2Catalog() {
        ItemCatalog catalog = ItemCatalog.load(java.nio.file.Path.of("..", "..", "tools", "gemini", "out", "items-v2", "items-v2.normalized.json"));
        assertEquals(61, catalog.all().size());
    }

    private static ItemCatalog parse(String json) {
        return ItemCatalog.load(new ByteArrayInputStream(json.getBytes(StandardCharsets.UTF_8)));
    }

    private static String error(String json) {
        return assertThrows(DataException.class, () -> parse(json)).getMessage();
    }

    @Test
    void bundledDataLoads() {
        ItemCatalog catalog = ItemCatalog.loadDefault();
        assertTrue(catalog.all().size() >= 10);
        // Only structure here: balance numbers change often and must not break tests.
        ItemDef axe = catalog.get("khai_son_phu");
        assertEquals(ItemType.WEAPON, axe.type);
        assertEquals(2, axe.width());
        assertEquals(Math.round(axe.cooldown * 1000), axe.cooldownMs());
    }

    @Test
    void lowercaseEnumsAreAccepted() {
        ItemCatalog c = parse("""
                [{"id":"x","name":"X","type":"relic","rarity":"rare",
                  "triggers":[{"on":"battle_start","effects":[{"op":"gain_qi","value":5}]}]}]""");
        assertEquals(TriggerEvent.BATTLE_START, c.get("x").triggers.get(0).on);
    }

    @Test
    void definitionRarityIsOptionalAndDefaultsToCommon() {
        ItemCatalog c = parse("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"relic\"}]");
        assertEquals(Rarity.COMMON, c.get("x").rarity);
    }

    @Test
    void rejectsWeaponWithoutCooldown() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","rarity":"common","attack":5}]""")
                .contains("cooldown"));
    }

    @Test
    void rejectsOneCellWeapon() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","rarity":"common","attack":5,"cooldown":1,"size":[1,1]}]""")
                .contains("at least 2 cells"));
    }

    @Test
    void rejectsUnknownWeaponClassAndWeaponClassOnNonWeapon() {
        String unknown = error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"dagger","rarity":"common",
                  "attack":5,"cooldown":1,"size":[1,2]}]""");
        assertTrue(unknown.contains("weaponClass must be one of"));
        assertTrue(unknown.contains("sword"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"armor","weaponClass":"sword","rarity":"common"}]""")
                .contains("weaponClass is only valid for weapons"));
    }

    @Test
    void rejectsNegativeOrMeaninglessNumbers() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","rarity":"common","attack":-5,"cooldown":1,"size":[1,2]}]""")
                .contains("attack"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"pill","rarity":"common",
                  "triggers":[{"on":"battle_start","effects":[{"op":"heal","value":-100}]}]}]""")
                .contains("HEAL needs value > 0"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"pill","rarity":"common",
                  "triggers":[{"on":"battle_start","effects":[{"op":"heal","value":50,"hits":3}]}]}]""")
                .contains("hits only applies to DAMAGE"));
    }

    @Test
    void rejectsFromFilterOnSideLevelEvents() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"pill","rarity":"common",
                  "triggers":[{"on":"hp_below","hpBelow":0.4,"from":"self","effects":[{"op":"heal","value":50}]}]}]""")
                .contains("only valid for"));
    }

    @Test
    void rejectsApplyWithoutStatus() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","rarity":"common","attack":5,"cooldown":1,
                  "basic":[{"op":"apply"}]}]""")
                .contains("needs status"));
    }

    @Test
    void rejectsTargetOfWrongKind() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","rarity":"common","attack":5,"cooldown":1,
                  "basic":[{"op":"damage","target":"enemy_all_items"}]}]""")
                .contains("not valid"));
    }

    @Test
    void rejectsUnknownFieldsAndDuplicates() {
        error("""
                [{"id":"x","name":"X","type":"relic","rarity":"rare","atack":5}]""");
        assertTrue(error("""
                [{"id":"x","name":"X","type":"relic","rarity":"rare"},
                 {"id":"x","name":"Y","type":"relic","rarity":"rare"}]""")
                .contains("Duplicate"));
    }

    private static final String UNLOCK_WEAPON = """
            [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"rare","size":[1,3],"attack":50,"cooldown":3,"crit":0.1,
              "basic":[{"op":"damage"}],
              "unlocks":[
                {"tier":3,"crit":0.05,"cooldown":-0.5,"basic":[{"op":"gain_qi","value":4}]},
                {"tier":6,"ultimateCost":40,"ultimate":[{"op":"damage","pct":2.0}]},
                {"tier":9,"attack":10,"ultimateCost":-10,"triggers":[{"on":"crit","effects":[{"op":"shield","value":20}]}]}
              ]}]""";

    @Test
    void atTierAddsUnlocksUpToThatTier() {
        ItemDef base = parse(UNLOCK_WEAPON).get("x");

        ItemDef t2 = base.atTier(2);
        assertEquals(1, t2.basic.size());
        assertEquals(3000, t2.cooldownMs());
        assertEquals(null, t2.ultimate);

        ItemDef t3 = base.atTier(3);
        assertEquals(2, t3.basic.size());
        assertEquals(2500, t3.cooldownMs());
        assertEquals(150, t3.critPermille());
        assertEquals(null, t3.ultimate);

        ItemDef t6 = base.atTier(6);
        assertEquals(40, t6.ultimate.cost);
        assertEquals(1, t6.ultimate.effects.size());
        assertEquals(0, t6.triggers.size());

        ItemDef t9 = base.atTier(9);
        assertEquals(60, t9.attack);
        assertEquals(30, t9.ultimate.cost);
        assertEquals(1, t9.triggers.size());
        assertTrue(t9.unlocks.isEmpty());
        // The base definition is not modified.
        assertEquals(1, base.basic.size());
        assertEquals(50, base.attack);
    }

    @Test
    void rejectsBadUnlocks() {
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"common","size":[1,2],"attack":5,"cooldown":1,
                  "unlocks":[{"tier":4,"attack":1}]}]""").contains("tier must be one of"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"common","size":[1,2],"attack":5,"cooldown":1,
                  "unlocks":[{"tier":6,"attack":1},{"tier":3,"attack":1}]}]""").contains("sorted"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"common","size":[1,2],"attack":5,"cooldown":1,
                  "unlocks":[{"tier":3,"cooldown":-1}]}]""").contains("at tier 3: weapon needs cooldown > 0"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"common","size":[1,2],"attack":5,"cooldown":1,
                  "unlocks":[{"tier":3,"ultimate":[{"op":"damage"}]}]}]""").contains("at tier 3: ultimate.cost must be > 0"));
        assertTrue(error("""
                [{"id":"x","name":"X","type":"weapon","weaponClass":"sword","rarity":"common","size":[1,2],"attack":5,"cooldown":1,
                  "unlocks":[{"tier":9,"basic":[{"op":"heal","value":0}]}]}]""").contains("at tier 9: basic[0]"));
    }

    @Test
    void parsesOptionalStatsAndRejectsInvalidStats() {
        ItemDef missing = parse("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"armor\"}]").get("x");
        assertEquals(0, missing.stats.hp);
        assertEquals(0, missing.stats.dmgReducePermille());
        assertEquals(0, missing.stats.cooldownReductionPermille());

        ItemDef stats = parse("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"armor\","
                + "\"stats\":{\"hp\":120,\"dmgReduce\":0.0504,\"cooldownReduction\":0.1005}}]").get("x");
        assertEquals(120, stats.stats.hp);
        assertEquals(50, stats.stats.dmgReducePermille());
        assertEquals(101, stats.stats.cooldownReductionPermille());

        assertTrue(error("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"armor\",\"stats\":{\"mystery\":1}}]")
                .contains("mystery"));
        assertTrue(error("[{\"id\":\"x\",\"name\":\"X\",\"type\":\"armor\",\"stats\":{\"hp\":1.5}}]")
                .contains("hp"));
        for (String bad : List.of("{\"hp\":-1}", "{\"dmgReduce\":-0.1}",
                "{\"dmgReduce\":1.1}", "{\"cooldownReduction\":-0.1}",
                "{\"cooldownReduction\":1.1}")) {
            String message = error("[{\"id\":\"stat_item\",\"name\":\"X\",\"type\":\"armor\",\"stats\":" + bad + "}]");
            assertTrue(message.contains("stat_item"));
        }
    }
}
