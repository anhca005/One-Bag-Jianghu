package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Path;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.util.List;
import java.util.Random;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ArenaSeasonTest
{
    @TempDir
    Path temporaryDirectory;
    private final MutableClock clock = new MutableClock();
    private Database database;
    private ProfileService profiles;
    private ArenaService arena;

    @BeforeEach
    void setUp()
    {
        database = new Database(temporaryDirectory.resolve("season.db"));
        GameData gameData = new GameData(GameServer.defaultDataPath());
        profiles = new ProfileService(database, gameData, clock, false);
        ObjectMapper mapper = ItemCatalog.mapper();
        BattleSimulator simulator = new BattleSimulator(ItemCatalog.loadDefault(), SectCatalog.loadDefault(),
                BattleConfig.defaults());
        Loadout bot = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        arena = new ArenaService(database, mapper, simulator, List.of(bot), new Random(7),
                new ArenaCalendar(clock), gameData, profiles);
    }

    @Test
    void finalizationRewardsTopFiftyOnceAndStartsEveryoneAtOneThousand() throws Exception
    {
        // Arrange
        long[] accounts = createPlayers(51);
        arena.maintainSeasons();
        long oldSeason = scalar("SELECT id FROM seasons");
        try (Connection connection = database.open(); PreparedStatement update = connection.prepareStatement(
                "UPDATE arena_players SET rating = ?, peak_rating = ? WHERE season_id = ? AND account_id = ?"))
        {
            for (int index = 0; index < accounts.length; index++)
            {
                int rating = 2000 - index;
                update.setInt(1, rating);
                update.setInt(2, rating);
                update.setLong(3, oldSeason);
                update.setLong(4, accounts[index]);
                update.addBatch();
            }
            update.executeBatch();
        }

        // Act
        clock.advanceDays(28);
        arena.maintainSeasons();
        arena.maintainSeasons();

        // Assert
        assertReward(oldSeason, accounts[0], 1, 3000, 3);
        assertReward(oldSeason, accounts[1], 2, 2000, 2);
        assertReward(oldSeason, accounts[2], 3, 2000, 2);
        assertReward(oldSeason, accounts[3], 4, 1200, 1);
        assertReward(oldSeason, accounts[9], 10, 1200, 1);
        assertReward(oldSeason, accounts[10], 11, 600, 0);
        assertReward(oldSeason, accounts[49], 50, 600, 0);
        assertEquals(0, scalar("SELECT COUNT(*) FROM season_rewards WHERE account_id = " + accounts[50]));
        assertEquals(50, scalar("SELECT COUNT(*) FROM season_rewards"));
        JsonNode champion = profiles.get(accounts[0]).profile();
        assertEquals(3000, champion.path("currencies").path("danh_vong").asInt());
        assertEquals(3, champion.path("materials").path("chi_ton_ngoc_tuy").asInt());
        assertEquals(51, scalar("SELECT COUNT(*) FROM arena_players WHERE season_id <> " + oldSeason
                + " AND rating = 1000 AND peak_rating = 1000"));
    }

    @Test
    void peakRatingNeverFallsAndWeeklyRewardNeedsFifteenWins() throws Exception
    {
        // Arrange
        long account = createPlayers(1)[0];
        arena.maintainSeasons();
        long season = scalar("SELECT id FROM seasons");

        // Act
        try (Connection connection = database.open())
        {
            ArenaService.updatePlayer(connection, season, account, 1100, 0);
            ArenaService.updatePlayer(connection, season, account, 1050, 1);
            for (int win = 0; win < 14; win++)
            {
                assertFalse(arena.reward(connection, account, true).path("materials")
                        .has("chi_ton_ngoc_tuy"));
            }
            assertTrue(arena.reward(connection, account, true).path("materials").has("chi_ton_ngoc_tuy"));
        }

        // Assert
        assertEquals(1100, scalar("SELECT peak_rating FROM arena_players WHERE account_id = " + account));
        JsonNode status = arena.status(account);
        assertEquals(15, status.path("weeklyWinsTarget").asInt());
        assertEquals(15, status.path("weeklyWins").asInt());
    }

    @Test
    void rewardRemainsVisibleUntilSeenAndJadeUsesTwoWeekPeriod() throws Exception
    {
        // Arrange
        long account = createPlayers(1)[0];
        arena.maintainSeasons();
        long season = scalar("SELECT id FROM seasons");
        profiles.mutate(account, (profile, result) -> profile.currencies().put("danh_vong", 5000));
        profiles.mutate(account, profiles.shopBuyReputation("chi_ton_ngoc_tuy"));

        // Act
        ServiceException blocked = assertThrows(ServiceException.class,
                () -> profiles.mutate(account, profiles.shopBuyReputation("chi_ton_ngoc_tuy")));
        clock.advanceDays(14);
        profiles.mutate(account, profiles.shopBuyReputation("chi_ton_ngoc_tuy"));
        try (Connection connection = database.open())
        {
            connection.createStatement().executeUpdate("UPDATE seasons SET ends_at = '2026-10-01T00:00+07:00'");
        }
        arena.maintainSeasons();
        JsonNode first = arena.status(account).path("seasonReward");
        JsonNode second = arena.status(account).path("seasonReward");

        // Assert
        assertEquals("rejected", blocked.code());
        assertNotNull(first);
        assertEquals(season, first.path("seasonId").asLong());
        assertEquals(first, second);
        arena.markSeasonRewardSeen(account, season);
        assertNull(arena.unseenSeasonReward(account));
        assertFalse(arena.status(account).has("seasonReward"));
    }

    private long[] createPlayers(int count) throws Exception
    {
        long[] accounts = new long[count];
        try (Connection connection = database.open(); PreparedStatement insert = connection.prepareStatement(
                "INSERT INTO accounts(username, hero_name, salt, password_hash) VALUES(?, ?, X'00', X'00')",
                Statement.RETURN_GENERATED_KEYS))
        {
            for (int index = 0; index < count; index++)
            {
                insert.setString(1, "player" + index);
                insert.setString(2, "Player " + index);
                insert.executeUpdate();
                try (ResultSet keys = insert.getGeneratedKeys())
                {
                    keys.next();
                    accounts[index] = keys.getLong(1);
                }
            }
        }
        for (int index = 0; index < count; index++)
        {
            profiles.ensure(accounts[index], "Player " + index);
        }
        return accounts;
    }

    private void assertReward(long season, long account, int rank, int reputation, int jade) throws Exception
    {
        String sql = "SELECT rank, reputation, jade FROM season_rewards WHERE season_id = ? AND account_id = ?";
        try (Connection connection = database.open(); PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, season);
            query.setLong(2, account);
            try (ResultSet row = query.executeQuery())
            {
                assertTrue(row.next());
                assertEquals(rank, row.getInt("rank"));
                assertEquals(reputation, row.getInt("reputation"));
                assertEquals(jade, row.getInt("jade"));
            }
        }
    }

    private long scalar(String sql) throws Exception
    {
        try (Connection connection = database.open())
        {
            try (Statement query = connection.createStatement())
            {
                try (ResultSet row = query.executeQuery(sql))
                {
                    row.next();
                    return row.getLong(1);
                }
            }
        }
    }

    private static final class MutableClock extends Clock
    {
        private Instant now = Instant.parse("2026-10-19T00:00:00Z");

        void advanceDays(long days)
        {
            now = now.plusSeconds(days * 24 * 60 * 60);
        }

        @Override
        public ZoneId getZone()
        {
            return ZoneOffset.UTC;
        }

        @Override
        public Clock withZone(ZoneId zone)
        {
            return this;
        }

        @Override
        public Instant instant()
        {
            return now;
        }
    }
}
