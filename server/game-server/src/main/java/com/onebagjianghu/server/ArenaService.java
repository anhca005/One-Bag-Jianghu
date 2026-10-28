package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.time.Duration;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.Random;

/** Transactional asynchronous arena battles, rewards, seasons and leaderboard. */
public final class ArenaService
{
    public static final int WEEKLY_WINS_TARGET = 15;
    private record Season(long id, ZonedDateTime endsAt) { }

    private final Database database;
    private final ObjectMapper mapper;
    private final BattleSimulator simulator;
    private final List<Loadout> bots;
    private final Random random;
    private final Matchmaker matchmaker;
    private final ArenaCalendar calendar;
    private final PowerCalculator powerCalculator;
    private final ProfileService profiles;

    public ArenaService(Database database, ObjectMapper mapper, BattleSimulator simulator, List<Loadout> bots,
            Random random, ArenaCalendar calendar, GameData gameData, ProfileService profiles)
    {
        this.database = database;
        this.mapper = mapper;
        this.simulator = simulator;
        this.bots = List.copyOf(bots);
        this.random = random;
        this.matchmaker = new Matchmaker(random);
        this.calendar = calendar;
        this.profiles = profiles;
        powerCalculator = new PowerCalculator(gameData.root("power.json"));
    }

    public void maintainSeasons()
    {
        try (Connection connection = database.open())
        {
            connection.setAutoCommit(false);
            try
            {
                currentSeason(connection);
                connection.commit();
            }
            catch (Exception exception)
            {
                connection.rollback();
                throw exception;
            }
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public ObjectNode status(long accountId)
    {
        maintainSeasons();
        try (Connection connection = database.open())
        {
            return status(connection, accountId, currentSeason(connection));
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public ObjectNode leaderboard(long accountId, int requestedLimit)
    {
        maintainSeasons();
        int limit = Math.max(1, Math.min(50, requestedLimit));
        try (Connection connection = database.open())
        {
            Season season = currentSeason(connection);
            ensurePlayer(connection, season.id(), accountId);
            ObjectNode response = mapper.createObjectNode();
            response.put("seasonId", season.id());
            ArrayNode entries = response.putArray("entries");
            try (PreparedStatement query = connection.prepareStatement("SELECT a.hero_name, p.rating, p.wins, "
                    + "p.losses "
                    + "FROM arena_players p JOIN accounts a ON a.id = p.account_id "
                    + "WHERE p.season_id = ? ORDER BY p.rating DESC, p.wins DESC, p.account_id ASC LIMIT ?"))
            {
                query.setLong(1, season.id());
                query.setInt(2, limit);
                try (ResultSet rows = query.executeQuery())
                {
                    int rank = 1;
                    while (rows.next())
                    {
                        ObjectNode entry = entries.addObject();
                        entry.put("rank", rank++);
                        entry.put("heroName", rows.getString("hero_name"));
                        entry.put("rating", rows.getInt("rating"));
                        entry.put("wins", rows.getInt("wins"));
                        entry.put("losses", rows.getInt("losses"));
                    }
                }
            }
            ObjectNode self = response.putObject("self");
            self.put("rank", rank(connection, season.id(), accountId));
            self.put("rating", playerRating(connection, season.id(), accountId));
            return response;
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public ObjectNode find(long accountId, Loadout requester, int power, int range, String searchId)
    {
        if (power < 0)
        {
            throw new ServiceException("invalid_loadout", "power must be non-negative");
        }
        try (Connection connection = database.open())
        {
            connection.setAutoCommit(false);
            try
            {
                Season season = currentSeason(connection);
                ensurePlayer(connection, season.id(), accountId);
                int rating = playerRating(connection, season.id(), accountId);
                saveGhost(connection, season.id(), accountId, requester, power, rating);
                List<Matchmaker.Ghost> ghosts = ghosts(connection, season.id(), accountId);
                Matchmaker.Ghost ghost = matchmaker.choose(ghosts, rating, range);
                if (ghost == null && !ghosts.isEmpty())
                {
                    connection.commit();
                    return null;
                }
                ObjectNode response = battleTransaction(connection, accountId, requester, power, ghost);
                response.put("searchId", searchId);
                connection.commit();
                return response;
            }
            catch (Exception exception)
            {
                connection.rollback();
                throw exception;
            }
        }
        catch (ServiceException exception)
        {
            throw exception;
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    ObjectNode battle(long accountId, Loadout requester, int power)
    {
        return find(accountId, requester, power, Integer.MAX_VALUE, "legacy-test");
    }

    private ObjectNode battleTransaction(Connection connection, long accountId, Loadout requester, int power,
            Matchmaker.Ghost selectedGhost)
            throws Exception
    {
        Season season = currentSeason(connection);
        ensurePlayer(connection, season.id(), accountId);
        int ratingBefore = playerRating(connection, season.id(), accountId);
        saveGhost(connection, season.id(), accountId, requester, power, ratingBefore);
        Matchmaker.Ghost ghost = selectedGhost;
        boolean bot = ghost == null;
        Loadout opponent = bot ? randomBot() : mapper.readValue(ghost.loadoutJson(), Loadout.class);
        if (!bot)
        {
            // A ghost fights under its owner's hero name, not the name inside the saved loadout.
            opponent.name = heroName(connection, ghost.accountId());
        }
        int opponentRating = bot ? ratingBefore : ghost.rating();
        int opponentPower = bot ? botPower(opponent) : ghost.power();
        long seed;
        synchronized (random)
        {
            seed = random.nextLong();
        }
        BattleResult result = simulator.run(requester, opponent, seed);
        double score = result.winner() == 0 ? 1.0 : result.winner() == 1 ? 0.0 : 0.5;
        int delta = EloRating.change(ratingBefore, opponentRating, score);
        int ratingAfter = EloRating.apply(ratingBefore, delta);
        updatePlayer(connection, season.id(), accountId, ratingAfter, result.winner());
        if (!bot)
        {
            updateOpponent(connection, season.id(), ghost.accountId(), -delta, oppositeWinner(result.winner()));
        }
        ObjectNode reward = reward(connection, accountId, score == 1.0);
        long battleId = saveBattle(connection, accountId, requester, opponent, opponentRating, opponentPower, bot,
                seed, result.winner(), ratingBefore, ratingAfter);
        ObjectNode response = mapper.createObjectNode();
        response.put("battleId", battleId);
        ObjectNode opponentJson = response.putObject("opponent");
        opponentJson.put("name", opponent.name);
        if (opponent.sect == null)
        {
            opponentJson.putNull("sect");
        }
        else
        {
            opponentJson.put("sect", opponent.sect);
        }
        opponentJson.put("rating", opponentRating);
        opponentJson.put("power", opponentPower);
        opponentJson.put("bot", bot);
        response.set("result", mapper.valueToTree(result));
        response.put("ratingBefore", ratingBefore);
        response.put("ratingAfter", ratingAfter);
        response.set("reward", reward);
        response.set("status", status(connection, accountId, season));
        return response;
    }

    private int botPower(Loadout loadout)
    {
        List<PowerCalculator.Item> items = new ArrayList<>();
        for (Loadout.PlacedItem item : loadout.items)
        {
            String rarity = item.rarity == null ? "common" : item.rarity.name().toLowerCase(Locale.ROOT);
            int tier = item.tier < 1 ? 1 : item.tier;
            items.add(new PowerCalculator.Item(item.id, tier, rarity));
        }
        return powerCalculator.calculate(loadout.level, items);
    }

    public ObjectNode history(long accountId, int requestedLimit)
    {
        int limit = Math.max(1, Math.min(20, requestedLimit));
        String sql = "SELECT id, fought_at, winner, opponent_name, opponent_sect, opponent_bot, "
                + "rating_before, rating_after FROM arena_battles WHERE account_id = ? "
                + "ORDER BY fought_at DESC, id DESC LIMIT ?";
        try (Connection connection = database.open(); PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, accountId);
            query.setInt(2, limit);
            ObjectNode response = mapper.createObjectNode();
            ArrayNode entries = response.putArray("entries");
            try (ResultSet rows = query.executeQuery())
            {
                while (rows.next())
                {
                    ObjectNode entry = entries.addObject();
                    entry.put("battleId", rows.getLong("id"));
                    entry.put("at", isoOffset(ZonedDateTime.parse(rows.getString("fought_at"))));
                    entry.put("won", rows.getInt("winner") == 0);
                    entry.put("opponentName", rows.getString("opponent_name"));
                    putNullable(entry, "opponentSect", rows.getString("opponent_sect"));
                    entry.put("bot", rows.getInt("opponent_bot") != 0);
                    entry.put("ratingBefore", rows.getInt("rating_before"));
                    entry.put("ratingAfter", rows.getInt("rating_after"));
                }
            }
            return response;
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public ObjectNode replay(long accountId, long battleId)
    {
        String sql = "SELECT * FROM arena_battles WHERE id = ? AND account_id = ?";
        try (Connection connection = database.open(); PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, battleId);
            query.setLong(2, accountId);
            try (ResultSet row = query.executeQuery())
            {
                if (!row.next())
                {
                    throw new ServiceException("not_found", "Arena battle was not found");
                }
                Loadout requester = mapper.readValue(row.getString("requester_loadout_json"), Loadout.class);
                Loadout opponent = mapper.readValue(row.getString("opponent_loadout_json"), Loadout.class);
                BattleResult result = simulator.run(requester, opponent, row.getLong("seed"));
                if (result.winner() != row.getInt("winner"))
                {
                    throw new IllegalStateException("Stored arena battle no longer replays deterministically");
                }
                ObjectNode response = mapper.createObjectNode();
                response.set("opponent", opponentJson(row));
                response.set("result", mapper.valueToTree(result));
                return response;
            }
        }
        catch (ServiceException exception)
        {
            throw exception;
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    ObjectNode reward(Connection connection, long accountId, boolean won) throws Exception
    {
        String day = calendar.day().toString();
        String week = calendar.week().toString();
        boolean rewarded;
        try (PreparedStatement daily = connection.prepareStatement(
                "INSERT INTO arena_daily(account_id, day, rewarded_battles) "
                        + "VALUES(?, ?, 1) ON CONFLICT(account_id, day) DO UPDATE SET "
                        + "rewarded_battles = rewarded_battles + 1 WHERE rewarded_battles < 10"))
        {
            daily.setLong(1, accountId);
            daily.setString(2, day);
            rewarded = daily.executeUpdate() == 1;
        }
        boolean jade = false;
        if (won)
        {
            try (PreparedStatement weekly = connection.prepareStatement(
                    "INSERT INTO arena_weekly(account_id, week, wins, jade_claimed) "
                            + "VALUES(?, ?, 1, 0) ON CONFLICT(account_id, week) DO UPDATE SET wins = wins + 1"))
            {
                weekly.setLong(1, accountId);
                weekly.setString(2, week);
                weekly.executeUpdate();
            }
            try (PreparedStatement claim = connection.prepareStatement("UPDATE arena_weekly SET jade_claimed = 1 "
                    + "WHERE account_id = ? AND week = ? AND wins >= ? AND jade_claimed = 0"))
            {
                claim.setLong(1, accountId);
                claim.setString(2, week);
                claim.setInt(3, WEEKLY_WINS_TARGET);
                jade = claim.executeUpdate() == 1;
            }
        }
        ObjectNode reward = mapper.createObjectNode();
        reward.put("reputation", rewarded ? (won ? 30 : 10) : 0);
        ObjectNode materials = reward.putObject("materials");
        if (jade)
        {
            materials.put("chi_ton_ngoc_tuy", 1);
        }
        return reward;
    }

    private ObjectNode status(Connection connection, long accountId, Season season) throws Exception
    {
        ensurePlayer(connection, season.id(), accountId);
        try (PreparedStatement player = connection.prepareStatement(
                "SELECT rating, peak_rating, wins, losses FROM arena_players WHERE season_id = ? AND account_id = ?"))
        {
            player.setLong(1, season.id());
            player.setLong(2, accountId);
            try (ResultSet row = player.executeQuery())
            {
                row.next();
                int daily = scalar(connection,
                        "SELECT rewarded_battles FROM arena_daily WHERE account_id = ? AND day = ?", accountId,
                        calendar.day().toString());
                int weeklyWins = scalar(connection,
                        "SELECT wins FROM arena_weekly WHERE account_id = ? AND week = ?", accountId,
                        calendar.week().toString());
                int claimed = scalar(connection,
                        "SELECT jade_claimed FROM arena_weekly WHERE account_id = ? AND week = ?", accountId,
                        calendar.week().toString());
                ObjectNode status = mapper.createObjectNode();
                status.put("seasonId", season.id());
                status.put("seasonEndsAt", isoOffset(season.endsAt()));
                status.put("rating", row.getInt("rating"));
                status.put("peakRating", row.getInt("peak_rating"));
                status.put("rank", rank(connection, season.id(), accountId));
                status.put("wins", row.getInt("wins"));
                status.put("losses", row.getInt("losses"));
                status.put("rewardedLeftToday", Math.max(0, 10 - daily));
                status.put("weeklyWins", weeklyWins);
                status.put("weeklyWinsTarget", WEEKLY_WINS_TARGET);
                status.put("weeklyJadeClaimed", claimed != 0);
                putSeasonReward(connection, accountId, status);
                return status;
            }
        }
    }

    private Season currentSeason(Connection connection) throws Exception
    {
        ZonedDateTime now = calendar.now();
        finalizeExpiredSeasons(connection, now);
        try (PreparedStatement query = connection.prepareStatement("SELECT id, ends_at FROM seasons "
                + "WHERE ends_at > ? ORDER BY id DESC LIMIT 1"))
        {
            query.setString(1, now.toString());
            try (ResultSet row = query.executeQuery())
            {
                if (row.next())
                {
                    return new Season(row.getLong("id"), ZonedDateTime.parse(row.getString("ends_at")));
                }
            }
        }
        ZonedDateTime starts = latestSeasonEnd(connection);
        if (starts == null)
        {
            starts = now.toLocalDate().atStartOfDay(ArenaCalendar.ZONE);
        }
        while (!starts.plusDays(28).isAfter(now))
        {
            starts = starts.plusDays(28);
        }
        ZonedDateTime ends = starts.plus(Duration.ofDays(28));
        try (PreparedStatement insert = connection.prepareStatement(
                "INSERT INTO seasons(starts_at, ends_at) VALUES(?, ?)", Statement.RETURN_GENERATED_KEYS))
        {
            insert.setString(1, starts.toString());
            insert.setString(2, ends.toString());
            insert.executeUpdate();
            try (ResultSet keys = insert.getGeneratedKeys())
            {
                keys.next();
                long seasonId = keys.getLong(1);
                try (PreparedStatement players = connection.prepareStatement(
                        "INSERT INTO arena_players(season_id, account_id) SELECT ?, id FROM accounts"))
                {
                    players.setLong(1, seasonId);
                    players.executeUpdate();
                }
                return new Season(seasonId, ends);
            }
        }
    }

    private ZonedDateTime latestSeasonEnd(Connection connection) throws Exception
    {
        try (PreparedStatement query = connection.prepareStatement(
                "SELECT ends_at FROM seasons ORDER BY ends_at DESC LIMIT 1"); ResultSet row = query.executeQuery())
        {
            return row.next() ? ZonedDateTime.parse(row.getString(1)) : null;
        }
    }

    private void finalizeExpiredSeasons(Connection connection, ZonedDateTime now) throws Exception
    {
        String sql = "SELECT id FROM seasons WHERE ends_at <= ? AND finalized_at IS NULL ORDER BY id";
        List<Long> seasons = new ArrayList<>();
        try (PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setString(1, now.toString());
            try (ResultSet rows = query.executeQuery())
            {
                while (rows.next())
                {
                    seasons.add(rows.getLong(1));
                }
            }
        }
        for (long seasonId : seasons)
        {
            finalizeSeason(connection, seasonId, now);
        }
    }

    private void finalizeSeason(Connection connection, long seasonId, ZonedDateTime now) throws Exception
    {
        String ranking = "SELECT account_id, rating FROM arena_players WHERE season_id = ? "
                + "ORDER BY rating DESC, wins DESC, account_id ASC";
        try (PreparedStatement query = connection.prepareStatement(ranking))
        {
            query.setLong(1, seasonId);
            try (ResultSet rows = query.executeQuery())
            {
                int rank = 0;
                while (rows.next())
                {
                    rank++;
                    if (rank <= 50)
                    {
                        grantSeasonReward(connection, seasonId, rows.getLong(1), rank, rows.getInt(2));
                    }
                }
            }
        }
        try (PreparedStatement update = connection.prepareStatement(
                "UPDATE seasons SET finalized_at = ? WHERE id = ? AND finalized_at IS NULL"))
        {
            update.setString(1, now.toString());
            update.setLong(2, seasonId);
            update.executeUpdate();
        }
    }

    private void grantSeasonReward(Connection connection, long seasonId, long accountId, int rank, int rating)
            throws Exception
    {
        int reputation = rank == 1 ? 3000 : rank <= 3 ? 2000 : rank <= 10 ? 1200 : 600;
        int jade = rank == 1 ? 3 : rank <= 3 ? 2 : rank <= 10 ? 1 : 0;
        try (PreparedStatement insert = connection.prepareStatement("INSERT OR IGNORE INTO season_rewards "
                + "(season_id, account_id, rank, rating, reputation, jade, seen) VALUES(?, ?, ?, ?, ?, ?, 0)"))
        {
            insert.setLong(1, seasonId);
            insert.setLong(2, accountId);
            insert.setInt(3, rank);
            insert.setInt(4, rating);
            insert.setInt(5, reputation);
            insert.setInt(6, jade);
            if (insert.executeUpdate() == 1)
            {
                profiles.mutate(connection, accountId, profiles.seasonReward(reputation, jade));
            }
        }
    }

    public ObjectNode unseenSeasonReward(long accountId)
    {
        maintainSeasons();
        try (Connection connection = database.open())
        {
            return seasonReward(connection, accountId);
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public ObjectNode markSeasonRewardSeen(long accountId, long seasonId)
    {
        try (Connection connection = database.open(); PreparedStatement update = connection.prepareStatement(
                "UPDATE season_rewards SET seen = 1 WHERE account_id = ? AND season_id = ?"))
        {
            update.setLong(1, accountId);
            update.setLong(2, seasonId);
            update.executeUpdate();
            ObjectNode result = mapper.createObjectNode();
            result.put("seasonId", seasonId);
            return result;
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    private void putSeasonReward(Connection connection, long accountId, ObjectNode target) throws Exception
    {
        ObjectNode reward = seasonReward(connection, accountId);
        if (reward != null)
        {
            target.set("seasonReward", reward);
        }
    }

    private ObjectNode seasonReward(Connection connection, long accountId) throws Exception
    {
        String sql = "SELECT season_id, rank, rating, reputation, jade FROM season_rewards "
                + "WHERE account_id = ? AND seen = 0 ORDER BY season_id DESC LIMIT 1";
        try (PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, accountId);
            try (ResultSet row = query.executeQuery())
            {
                if (!row.next())
                {
                    return null;
                }
                ObjectNode reward = mapper.createObjectNode();
                reward.put("seasonId", row.getLong("season_id"));
                reward.put("rank", row.getInt("rank"));
                reward.put("rating", row.getInt("rating"));
                reward.put("reputation", row.getInt("reputation"));
                reward.put("jade", row.getInt("jade"));
                return reward;
            }
        }
    }

    private void saveGhost(Connection connection, long season, long account, Loadout loadout, int power, int rating)
            throws Exception
    {
        try (PreparedStatement statement = connection.prepareStatement("INSERT INTO ghosts "
                + "(season_id, account_id, loadout_json, power, rating, saved_at) VALUES(?, ?, ?, ?, ?, ?) "
                + "ON CONFLICT(season_id, account_id) DO UPDATE SET loadout_json = excluded.loadout_json, "
                + "power = excluded.power, rating = excluded.rating, saved_at = excluded.saved_at"))
        {
            statement.setLong(1, season);
            statement.setLong(2, account);
            statement.setString(3, mapper.writeValueAsString(loadout));
            statement.setInt(4, power);
            statement.setInt(5, rating);
            statement.setString(6, calendar.now().toString());
            statement.executeUpdate();
        }
    }

    private List<Matchmaker.Ghost> ghosts(Connection connection, long season, long ownAccount) throws Exception
    {
        List<Matchmaker.Ghost> result = new ArrayList<>();
        try (PreparedStatement query = connection.prepareStatement(
                "SELECT account_id, loadout_json, power, rating FROM ghosts "
                        + "WHERE season_id = ? AND account_id <> ?"))
        {
            query.setLong(1, season);
            query.setLong(2, ownAccount);
            try (ResultSet rows = query.executeQuery())
            {
                while (rows.next())
                {
                    result.add(new Matchmaker.Ghost(rows.getLong(1), rows.getString(2), rows.getInt(3),
                            rows.getInt(4)));
                }
            }
        }
        return result;
    }

    private Loadout randomBot()
    {
        synchronized (random)
        {
            return bots.get(random.nextInt(bots.size()));
        }
    }

    private static String heroName(Connection connection, long account) throws Exception
    {
        try (PreparedStatement query = connection.prepareStatement("SELECT hero_name FROM accounts WHERE id = ?"))
        {
            query.setLong(1, account);
            try (ResultSet row = query.executeQuery())
            {
                return row.next() ? row.getString("hero_name") : "Hiệp Khách";
            }
        }
    }

    /** The protocol sends plain ISO-8601 with an offset; ZonedDateTime.toString adds a "[zone]" C# cannot read. */
    private static String isoOffset(ZonedDateTime time)
    {
        return time.toOffsetDateTime().truncatedTo(ChronoUnit.SECONDS).format(DateTimeFormatter.ISO_OFFSET_DATE_TIME);
    }

    private long saveBattle(Connection connection, long accountId, Loadout requester, Loadout opponent,
            int opponentRating, int opponentPower, boolean bot, long seed, int winner, int ratingBefore,
            int ratingAfter) throws Exception
    {
        String sql = "INSERT INTO arena_battles(account_id, fought_at, requester_loadout_json, "
                + "opponent_loadout_json, opponent_name, opponent_sect, opponent_bot, opponent_rating, "
                + "opponent_power, seed, winner, rating_before, rating_after) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, "
                + "?, ?, ?, ?)";
        try (PreparedStatement insert = connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS))
        {
            insert.setLong(1, accountId);
            insert.setString(2, calendar.now().toString());
            insert.setString(3, mapper.writeValueAsString(requester));
            insert.setString(4, mapper.writeValueAsString(opponent));
            insert.setString(5, opponent.name);
            insert.setString(6, opponent.sect);
            insert.setInt(7, bot ? 1 : 0);
            insert.setInt(8, opponentRating);
            insert.setInt(9, opponentPower);
            insert.setLong(10, seed);
            insert.setInt(11, winner);
            insert.setInt(12, ratingBefore);
            insert.setInt(13, ratingAfter);
            insert.executeUpdate();
            try (ResultSet keys = insert.getGeneratedKeys())
            {
                keys.next();
                return keys.getLong(1);
            }
        }
    }

    private ObjectNode opponentJson(ResultSet row) throws Exception
    {
        ObjectNode opponent = mapper.createObjectNode();
        opponent.put("name", row.getString("opponent_name"));
        putNullable(opponent, "sect", row.getString("opponent_sect"));
        opponent.put("rating", row.getInt("opponent_rating"));
        opponent.put("power", row.getInt("opponent_power"));
        opponent.put("bot", row.getInt("opponent_bot") != 0);
        return opponent;
    }

    private static void putNullable(ObjectNode node, String field, String value)
    {
        if (value == null)
        {
            node.putNull(field);
        }
        else
        {
            node.put(field, value);
        }
    }

    private static int oppositeWinner(int winner)
    {
        return winner < 0 ? -1 : 1 - winner;
    }

    private static void ensurePlayer(Connection connection, long season, long account) throws Exception
    {
        try (PreparedStatement insert = connection.prepareStatement(
                "INSERT OR IGNORE INTO arena_players(season_id, account_id) VALUES(?, ?)"))
        {
            insert.setLong(1, season);
            insert.setLong(2, account);
            insert.executeUpdate();
        }
    }

    private static int playerRating(Connection connection, long season, long account) throws Exception
    {
        try (PreparedStatement query = connection.prepareStatement(
                "SELECT rating FROM arena_players WHERE season_id = ? AND account_id = ?"))
        {
            query.setLong(1, season);
            query.setLong(2, account);
            try (ResultSet row = query.executeQuery())
            {
                row.next();
                return row.getInt(1);
            }
        }
    }

    static void updatePlayer(Connection connection, long season, long account, int rating, int winner)
            throws Exception
    {
        String counter = winner == 0 ? ", wins = wins + 1" : winner == 1 ? ", losses = losses + 1" : "";
        try (PreparedStatement update = connection.prepareStatement("UPDATE arena_players SET rating = ?, "
                + "peak_rating = MAX(peak_rating, ?)" + counter
                + " WHERE season_id = ? AND account_id = ?"))
        {
            update.setInt(1, rating);
            update.setInt(2, rating);
            update.setLong(3, season);
            update.setLong(4, account);
            update.executeUpdate();
        }
    }

    static void updateOpponent(Connection connection, long season, long account, int ratingChange, int winner)
            throws Exception
    {
        ensurePlayer(connection, season, account);
        String counter = winner == 0 ? ", wins = wins + 1" : winner == 1 ? ", losses = losses + 1" : "";
        String sql = "UPDATE arena_players SET rating = MAX(" + EloRating.FLOOR + ", rating + ?), "
                + "peak_rating = MAX(peak_rating, MAX(" + EloRating.FLOOR + ", rating + ?))"
                + counter + " WHERE season_id = ? AND account_id = ?";
        try (PreparedStatement update = connection.prepareStatement(sql))
        {
            update.setInt(1, ratingChange);
            update.setInt(2, ratingChange);
            update.setLong(3, season);
            update.setLong(4, account);
            update.executeUpdate();
        }
    }

    private static int rank(Connection connection, long season, long account) throws Exception
    {
        String sql = "SELECT COUNT(*) + 1 FROM arena_players ranked JOIN arena_players own "
                + "ON own.season_id = ranked.season_id WHERE own.season_id = ? AND own.account_id = ? AND "
                + "(ranked.rating > own.rating OR (ranked.rating = own.rating AND ranked.wins > own.wins) OR "
                + "(ranked.rating = own.rating AND ranked.wins = own.wins AND ranked.account_id < own.account_id))";
        try (PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, season);
            query.setLong(2, account);
            try (ResultSet row = query.executeQuery())
            {
                row.next();
                return row.getInt(1);
            }
        }
    }

    private static int scalar(Connection connection, String sql, long account, String period) throws Exception
    {
        try (PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, account);
            query.setString(2, period);
            try (ResultSet row = query.executeQuery())
            {
                return row.next() ? row.getInt(1) : 0;
            }
        }
    }
}
