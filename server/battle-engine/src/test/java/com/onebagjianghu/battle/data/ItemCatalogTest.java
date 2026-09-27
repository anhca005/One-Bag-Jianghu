package com.onebagjianghu.battle.data;

import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ItemCatalogTest {

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
}
