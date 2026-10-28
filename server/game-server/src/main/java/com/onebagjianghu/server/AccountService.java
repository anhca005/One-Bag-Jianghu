package com.onebagjianghu.server;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.SecureRandom;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.util.Base64;
import java.util.Locale;
import java.util.regex.Pattern;

/** Registration, authentication and hashed persistent session tokens. */
public final class AccountService
{
    public record Auth(long accountId, String username, String heroName, String token, Instant expiresAt) { }

    private static final Pattern USERNAME = Pattern.compile("[a-z0-9_]{3,20}");
    private final Database database;
    private final PasswordHasher passwords;
    private final SecureRandom random;
    private final Clock clock;

    public AccountService(Database database, SecureRandom random, Clock clock)
    {
        this.database = database;
        this.passwords = new PasswordHasher(random);
        this.random = random;
        this.clock = clock;
    }

    public Auth register(String suppliedUsername, String password, String suppliedHeroName)
    {
        String username = normalizeUsername(suppliedUsername);
        validatePassword(password);
        String heroName = suppliedHeroName == null ? "" : suppliedHeroName.trim();
        if (heroName.isEmpty() || heroName.length() > 24)
        {
            throw new ServiceException("invalid_name", "heroName must contain 1-24 characters");
        }
        byte[] salt = passwords.newSalt();
        byte[] hash = passwords.hash(password.toCharArray(), salt);
        try (Connection connection = database.open())
        {
            try (PreparedStatement insert = connection.prepareStatement(
                    "INSERT INTO accounts(username, hero_name, salt, password_hash) VALUES(?, ?, ?, ?)"))
            {
                insert.setString(1, username);
                insert.setString(2, heroName);
                insert.setBytes(3, salt);
                insert.setBytes(4, hash);
                insert.executeUpdate();
            }
            return issue(connection, accountId(connection, username), username, heroName);
        }
        catch (Exception exception)
        {
            if (exception instanceof java.sql.SQLException && exception.getMessage().contains("UNIQUE"))
            {
                throw new ServiceException("name_taken", "Username is already registered");
            }
            throw new IllegalStateException(exception);
        }
    }

    public Auth login(String suppliedUsername, String password)
    {
        String username = suppliedUsername == null ? "" : suppliedUsername.toLowerCase(Locale.ROOT);
        String suppliedPassword = password == null ? "" : password;
        try (Connection connection = database.open())
        {
            try (PreparedStatement query = connection.prepareStatement(
                    "SELECT id, username, hero_name, salt, password_hash FROM accounts WHERE username = ?"))
            {
                query.setString(1, username);
                try (ResultSet row = query.executeQuery())
                {
                    if (!row.next() || !passwords.matches(suppliedPassword.toCharArray(), row.getBytes("salt"),
                            row.getBytes("password_hash")))
                    {
                        throw new ServiceException("auth_failed", "Invalid username or password");
                    }
                    return issue(connection, row.getLong("id"), row.getString("username"),
                            row.getString("hero_name"));
                }
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

    public Auth resume(String token)
    {
        try (Connection connection = database.open())
        {
            try (PreparedStatement query = connection.prepareStatement(
                    "SELECT a.id, a.username, a.hero_name, s.expires_at "
                            + "FROM sessions s JOIN accounts a ON a.id = s.account_id WHERE s.token_hash = ?"))
            {
                query.setBytes(1, tokenHash(token));
                try (ResultSet row = query.executeQuery())
                {
                    if (!row.next())
                    {
                        throw new ServiceException("token_expired", "Session token is invalid or expired");
                    }
                    Instant expiresAt = Instant.parse(row.getString("expires_at"));
                    if (!expiresAt.isAfter(clock.instant()))
                    {
                        deleteToken(connection, token);
                        throw new ServiceException("token_expired", "Session token is invalid or expired");
                    }
                    return new Auth(row.getLong("id"), row.getString("username"), row.getString("hero_name"), token,
                            expiresAt);
                }
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

    public void logout(String token)
    {
        if (token == null)
        {
            return;
        }
        try (Connection connection = database.open())
        {
            deleteToken(connection, token);
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }

    public String heroName(long accountId)
    {
        try (Connection connection = database.open(); PreparedStatement query = connection.prepareStatement(
                "SELECT hero_name FROM accounts WHERE id = ?"))
        {
            query.setLong(1, accountId);
            try (ResultSet row = query.executeQuery())
            {
                if (!row.next())
                {
                    throw new ServiceException("unauthorized", "Login is required");
                }
                return row.getString(1);
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

    private Auth issue(Connection connection, long id, String username, String heroName) throws Exception
    {
        byte[] bytes = new byte[32];
        random.nextBytes(bytes);
        String token = Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
        Instant expiresAt = clock.instant().plus(Duration.ofDays(30));
        try (PreparedStatement insert = connection.prepareStatement(
                "INSERT INTO sessions(token_hash, account_id, expires_at) VALUES(?, ?, ?)"))
        {
            insert.setBytes(1, tokenHash(token));
            insert.setLong(2, id);
            insert.setString(3, expiresAt.toString());
            insert.executeUpdate();
        }
        return new Auth(id, username, heroName, token, expiresAt);
    }

    private static long accountId(Connection connection, String username) throws Exception
    {
        try (PreparedStatement query = connection.prepareStatement("SELECT id FROM accounts WHERE username = ?"))
        {
            query.setString(1, username);
            try (ResultSet row = query.executeQuery())
            {
                row.next();
                return row.getLong(1);
            }
        }
    }

    private static void deleteToken(Connection connection, String token) throws Exception
    {
        try (PreparedStatement delete = connection.prepareStatement("DELETE FROM sessions WHERE token_hash = ?"))
        {
            delete.setBytes(1, tokenHash(token));
            delete.executeUpdate();
        }
    }

    private static byte[] tokenHash(String token) throws Exception
    {
        return MessageDigest.getInstance("SHA-256").digest(token.getBytes(StandardCharsets.UTF_8));
    }

    private static String normalizeUsername(String supplied)
    {
        String username = supplied == null ? "" : supplied.toLowerCase(Locale.ROOT);
        if (!USERNAME.matcher(username).matches())
        {
            throw new ServiceException("invalid_name", "username must match [a-z0-9_]{3,20}");
        }
        return username;
    }

    private static void validatePassword(String password)
    {
        if (password == null || password.length() < 6 || password.length() > 64)
        {
            throw new ServiceException("weak_password", "password must contain 6-64 characters");
        }
    }
}
