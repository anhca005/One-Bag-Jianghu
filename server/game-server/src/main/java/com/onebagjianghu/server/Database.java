package com.onebagjianghu.server;

import java.nio.file.Files;
import java.nio.file.Path;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.sql.Statement;
import org.sqlite.SQLiteConfig;

/** Owns SQLite connections and schema creation. */
public final class Database
{
    private static final int SCHEMA_VERSION = 4;
    private final String url;

    public Database(Path path)
    {
        try
        {
            Path parent = path.toAbsolutePath().getParent();
            if (parent != null)
            {
                Files.createDirectories(parent);
            }
        }
        catch (Exception exception)
        {
            throw new IllegalStateException("Cannot create database directory", exception);
        }
        url = "jdbc:sqlite:" + path;
        migrate();
    }

    public Database(String jdbcUrl)
    {
        url = jdbcUrl;
        migrate();
    }

    public Connection open() throws SQLException
    {
        // Transactions start with BEGIN IMMEDIATE. A deferred transaction reads first and asks for the write lock
        // later; when two of them do that at once SQLite answers SQLITE_BUSY at once instead of waiting
        // busy_timeout, which the load test hit with 10+ concurrent clients.
        // WAL lets readers keep reading while one writer commits, so writers no longer queue behind them.
        SQLiteConfig config = new SQLiteConfig();
        config.setTransactionMode(SQLiteConfig.TransactionMode.IMMEDIATE);
        config.setJournalMode(SQLiteConfig.JournalMode.WAL);
        config.setBusyTimeout(5000);
        config.enforceForeignKeys(true);
        return DriverManager.getConnection(url, config.toProperties());
    }

    private void migrate()
    {
        try (Connection connection = open(); Statement statement = connection.createStatement())
        {
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS schema_version (version INTEGER NOT NULL)");
            statement.executeUpdate("INSERT INTO schema_version(version) "
                    + "SELECT 1 WHERE NOT EXISTS (SELECT 1 FROM schema_version)");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS accounts ("
                    + "id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT NOT NULL UNIQUE, "
                    + "hero_name TEXT NOT NULL, salt BLOB NOT NULL, password_hash BLOB NOT NULL)");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS sessions ("
                    + "token_hash BLOB PRIMARY KEY, account_id INTEGER NOT NULL, expires_at TEXT NOT NULL, "
                    + "FOREIGN KEY(account_id) REFERENCES accounts(id) ON DELETE CASCADE)");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS seasons ("
                    + "id INTEGER PRIMARY KEY AUTOINCREMENT, starts_at TEXT NOT NULL, ends_at TEXT NOT NULL, "
                    + "finalized_at TEXT)");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS arena_players ("
                    + "season_id INTEGER NOT NULL, account_id INTEGER NOT NULL, rating INTEGER NOT NULL DEFAULT 1000, "
                    + "peak_rating INTEGER NOT NULL DEFAULT 1000, wins INTEGER NOT NULL DEFAULT 0, "
                    + "losses INTEGER NOT NULL DEFAULT 0, "
                    + "PRIMARY KEY(season_id, account_id))");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS ghosts ("
                    + "season_id INTEGER NOT NULL, account_id INTEGER NOT NULL, loadout_json TEXT NOT NULL, "
                    + "power INTEGER NOT NULL, rating INTEGER NOT NULL, saved_at TEXT NOT NULL, "
                    + "PRIMARY KEY(season_id, account_id))");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS arena_daily ("
                    + "account_id INTEGER NOT NULL, day TEXT NOT NULL, rewarded_battles INTEGER NOT NULL DEFAULT 0, "
                    + "PRIMARY KEY(account_id, day))");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS arena_weekly ("
                    + "account_id INTEGER NOT NULL, week TEXT NOT NULL, wins INTEGER NOT NULL DEFAULT 0, "
                    + "jade_claimed INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(account_id, week))");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS arena_battles ("
                    + "id INTEGER PRIMARY KEY AUTOINCREMENT, account_id INTEGER NOT NULL, fought_at TEXT NOT NULL, "
                    + "requester_loadout_json TEXT NOT NULL, opponent_loadout_json TEXT NOT NULL, "
                    + "opponent_name TEXT NOT NULL, opponent_sect TEXT, opponent_bot INTEGER NOT NULL, "
                    + "opponent_rating INTEGER NOT NULL, opponent_power INTEGER NOT NULL, seed INTEGER NOT NULL, "
                    + "winner INTEGER NOT NULL, rating_before INTEGER NOT NULL, rating_after INTEGER NOT NULL, "
                    + "FOREIGN KEY(account_id) REFERENCES accounts(id) ON DELETE CASCADE)");
            statement.executeUpdate("CREATE INDEX IF NOT EXISTS arena_battles_account_time "
                    + "ON arena_battles(account_id, fought_at DESC, id DESC)");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS profiles ("
                    + "account_id INTEGER PRIMARY KEY, json TEXT NOT NULL, rev INTEGER NOT NULL DEFAULT 0, "
                    + "updated_at TEXT NOT NULL, FOREIGN KEY(account_id) REFERENCES accounts(id) ON DELETE CASCADE)");
            addColumn(connection, "seasons", "finalized_at", "TEXT");
            addColumn(connection, "arena_players", "peak_rating", "INTEGER NOT NULL DEFAULT 1000");
            statement.executeUpdate("CREATE TABLE IF NOT EXISTS season_rewards ("
                    + "season_id INTEGER NOT NULL, account_id INTEGER NOT NULL, rank INTEGER NOT NULL, "
                    + "rating INTEGER NOT NULL, reputation INTEGER NOT NULL, jade INTEGER NOT NULL, "
                    + "seen INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(season_id, account_id))");
            statement.executeUpdate("UPDATE schema_version SET version = " + SCHEMA_VERSION);
        }
        catch (SQLException exception)
        {
            throw new IllegalStateException("Cannot initialize SQLite database", exception);
        }
    }

    private static void addColumn(Connection connection, String table, String column, String definition)
            throws SQLException
    {
        try (var query = connection.prepareStatement("SELECT 1 FROM pragma_table_info('" + table
                + "') WHERE name = ?"))
        {
            query.setString(1, column);
            try (var row = query.executeQuery())
            {
                if (row.next())
                {
                    return;
                }
            }
        }
        try (Statement statement = connection.createStatement())
        {
            statement.executeUpdate("ALTER TABLE " + table + " ADD COLUMN " + column + " " + definition);
        }
    }
}
