package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Path;
import java.security.SecureRandom;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;
import java.time.Clock;
import java.time.Instant;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.List;
import java.util.Random;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ArenaHistoryTest
{
    private static final Clock CLOCK = Clock.fixed(Instant.parse("2026-10-18T12:00:00Z"), ZoneOffset.UTC);

    @TempDir
    Path temporaryDirectory;

    @Test
    void battleIsStoredAndCanBeReplayedOnlyByItsOwner() throws Exception
    {
        // Arrange
        Database database = new Database(temporaryDirectory.resolve("arena.db"));
        AccountService accounts = new AccountService(database, new SecureRandom(), CLOCK);
        long owner = accounts.register("owner_1", "secret1", "Owner").accountId();
        long stranger = accounts.register("other_1", "secret2", "Other").accountId();
        Loadout loadout = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        ArenaService arena = service(database, loadout);

        // Act
        ObjectNode battle = arena.battle(owner, loadout, 1000);
        long battleId = battle.path("battleId").asLong();
        ObjectNode history = arena.history(owner, 20);
        ObjectNode replay = arena.replay(owner, battleId);

        // Assert
        assertTrue(battleId > 0);
        assertEquals(275, battle.path("opponent").path("power").asInt());
        assertEquals(battleId, history.path("entries").get(0).path("battleId").asLong());
        assertEquals(battle.path("result").path("winner"), replay.path("result").path("winner"));
        assertEquals(275, replay.path("opponent").path("power").asInt());
        ServiceException exception = assertThrows(ServiceException.class, () -> arena.replay(stranger, battleId));
        assertEquals("not_found", exception.code());
    }

    @Test
    void ghostFightsUnderOwnerHeroNameAndTimesAreOffsetIso() throws Exception
    {
        // Arrange
        Database database = new Database(temporaryDirectory.resolve("ghost.db"));
        AccountService accounts = new AccountService(database, new SecureRandom(), CLOCK);
        long first = accounts.register("first_1", "secret1", "Kiếm Khách").accountId();
        long second = accounts.register("second_1", "secret2", "Đao Khách").accountId();
        Loadout loadout = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        ArenaService arena = service(database, loadout);
        arena.battle(first, loadout, 1000);

        // Act
        ObjectNode battle = arena.battle(second, loadout, 1000);
        String endsAt = battle.path("status").path("seasonEndsAt").asText();
        String at = arena.history(second, 20).path("entries").get(0).path("at").asText();

        // Assert
        assertFalse(battle.path("opponent").path("bot").asBoolean());
        assertEquals("Kiếm Khách", battle.path("opponent").path("name").asText());
        OffsetDateTime.parse(endsAt);
        assertFalse(endsAt.contains("["));
        assertFalse(at.contains("["));
        OffsetDateTime.parse(at);
    }

    @Test
    void drawChangesNeitherWinsNorLossesAndUsesLosingReward() throws Exception
    {
        // Arrange
        Database database = new Database(temporaryDirectory.resolve("draw.db"));
        AccountService accounts = new AccountService(database, new SecureRandom(), CLOCK);
        long accountId = accounts.register("drawer", "secret1", "Drawer").accountId();
        Loadout empty = new Loadout("Empty", 100, 5, 4);
        ArenaService arena = service(database, empty);

        // Act
        ObjectNode result = arena.battle(accountId, empty, 0);

        // Assert
        assertEquals(-1, result.path("result").path("winner").asInt());
        assertEquals(1000, result.path("ratingAfter").asInt());
        assertEquals(0, result.path("status").path("wins").asInt());
        assertEquals(0, result.path("status").path("losses").asInt());
        assertEquals(10, result.path("reward").path("reputation").asInt());
    }

    @Test
    void oldSchemaMigratesToVersionTwo() throws Exception
    {
        // Arrange
        Path path = temporaryDirectory.resolve("old.db");
        try (Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
                Statement statement = connection.createStatement())
        {
            statement.executeUpdate("CREATE TABLE schema_version(version INTEGER NOT NULL)");
            statement.executeUpdate("INSERT INTO schema_version(version) VALUES(1)");
        }

        // Act
        Database database = new Database(path);

        // Assert
        try (Connection connection = database.open(); Statement statement = connection.createStatement())
        {
            try (ResultSet version = statement.executeQuery("SELECT version FROM schema_version"))
            {
                assertTrue(version.next());
                assertEquals(4, version.getInt(1));
            }
        }
    }

    private static ArenaService service(Database database, Loadout bot)
    {
        ObjectMapper mapper = ItemCatalog.mapper();
        BattleSimulator simulator = new BattleSimulator(ItemCatalog.loadDefault(), SectCatalog.loadDefault(),
                BattleConfig.defaults());
        GameData gameData = new GameData(GameServer.defaultDataPath());
        ProfileService profiles = new ProfileService(database, gameData, CLOCK, false);
        return new ArenaService(database, mapper, simulator, List.of(bot), new Random(7), new ArenaCalendar(CLOCK),
                gameData, profiles);
    }
}
