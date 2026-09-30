package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.DataException;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BattleConfigTest {
    @TempDir
    Path temp;

    @Test
    void partialOverrideUpdatesPublicFieldsOverDefaults() throws IOException {
        Path file = temp.resolve("config.json");
        Files.writeString(file, """
                {"rarityMultiplierPermille":[1000,1150,1320,1520,1750,2000],"maxRegen":17}
                """);

        BattleConfig config = BattleConfig.loadOverride(file);

        assertArrayEquals(new int[]{1000, 1150, 1320, 1520, 1750, 2000},
                config.rarityMultiplierPermille);
        assertEquals(17, config.maxRegen);
        assertEquals(BattleConfig.defaults().tickMs, config.tickMs);
        assertEquals(1000, config.shieldCapPermille);
    }

    @Test
    void overrideRejectsUnknownFieldsWithClearError() throws IOException {
        Path file = temp.resolve("bad.json");
        Files.writeString(file, "{\"notAField\":1}");

        DataException error = assertThrows(DataException.class, () -> BattleConfig.loadOverride(file));
        assertTrue(error.getMessage().contains("notAField"), error.getMessage());
    }
}
