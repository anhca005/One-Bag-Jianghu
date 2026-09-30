package com.onebagjianghu.battle.data;

import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertSame;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class SectCatalogTest {

    private static SectCatalog parse(String json) {
        return SectCatalog.load(new ByteArrayInputStream(json.getBytes(StandardCharsets.UTF_8)));
    }

    private static String error(String json) {
        return assertThrows(DataException.class, () -> parse(json)).getMessage();
    }

    private static final String SWORD_SECT = """
            [{"id":"s","name":"S","tamPhap":"T","weaponClasses":["sword"],
              "levels":[
                {"level":1,"attackPct":0.1},
                {"level":3,"cooldownPct":0.2,"crit":0.05},
                {"level":6,"ultimateCost":-10,"ultimate":[{"op":"gain_qi","value":5}]},
                {"level":9,"basic":[{"op":"shield","value":20}]}
              ]}]""";

    private static ItemDef sword() {
        return ItemCatalog.load(new ByteArrayInputStream("""
                [{"id":"w","name":"W","type":"weapon","weaponClass":"sword","rarity":"rare","size":[1,3],
                  "attack":100,"cooldown":5,"crit":0.1,"basic":[{"op":"damage"}],
                  "ultimate":{"cost":40,"effects":[{"op":"damage","pct":2.0}]}}]""".getBytes(StandardCharsets.UTF_8))).get("w");
    }

    @Test
    void bundledSectsLoad() {
        SectCatalog sects = SectCatalog.loadDefault();
        assertEquals(7, sects.all().size());
        assertTrue(sects.get("kim_cang").weaponClasses.contains("fist"));
    }

    @Test
    void appliesCumulativeLevelsToMatchingClassOnly() {
        SectDef sect = parse(SWORD_SECT).get("s");
        ItemDef w = sword();

        ItemDef l1 = sect.applyTo(w, 1);
        assertEquals(110, l1.attack);
        assertEquals(5000, l1.cooldownMs());

        ItemDef l6 = sect.applyTo(w, 6);
        assertEquals(4000, l6.cooldownMs());
        assertEquals(150, l6.critPermille());
        assertEquals(30, l6.ultimate.cost);
        assertEquals(2, l6.ultimate.effects.size());
        assertEquals(1, l6.basic.size());

        assertEquals(2, sect.applyTo(w, 9).basic.size());
        // The original definition is untouched.
        assertEquals(100, w.attack);
        assertEquals(1, w.ultimate.effects.size());

        ItemDef axe = w.copy();
        axe.weaponClass = "axe";
        assertSame(axe, sect.applyTo(axe, 9));
    }

    @Test
    void rejectsBadSects() {
        assertTrue(error("""
                [{"id":"s","name":"S","tamPhap":"T","weaponClasses":[],"levels":[]}]""").contains("weaponClasses"));
        assertTrue(error("""
                [{"id":"s","name":"S","tamPhap":"T","weaponClasses":["sword"],"levels":[{"level":2}]}]""").contains("level must be one of"));
        assertTrue(error("""
                [{"id":"s","name":"S","tamPhap":"T","weaponClasses":["sword"],"levels":[{"level":3}]}]""").contains("first level must be 1"));
        assertTrue(error("""
                [{"id":"s","name":"S","tamPhap":"T","weaponClasses":["sword"],"levels":[{"level":1,"cooldownPct":0.6}]}]""").contains("cooldownPct"));
        assertTrue(error("""
                [{"id":"s","name":"S","tamPhap":"T","weaponClasses":["sword"],
                  "levels":[{"level":1,"basic":[{"op":"heal","value":0}]}]}]""").contains("levels[0].basic[0]"));
    }
}
