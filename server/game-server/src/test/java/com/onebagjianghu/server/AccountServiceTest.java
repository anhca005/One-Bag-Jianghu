package com.onebagjianghu.server;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Path;
import java.security.SecureRandom;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneOffset;
import java.sql.Connection;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class AccountServiceTest
{
    @TempDir
    Path directory;
    private AccountService accounts;
    private Database database;

    @BeforeEach
    void setUp()
    {
        Clock clock = Clock.fixed(Instant.parse("2026-10-18T00:00:00Z"), ZoneOffset.UTC);
        database = new Database(directory.resolve("test.db"));
        accounts = new AccountService(database, new SecureRandom(), clock);
    }

    @Test
    void registerLoginAndResume()
    {
        // Arrange
        AccountService.Auth registered = accounts.register("Player_1", "secret1", "  Hiệp Khách  ");

        // Act
        AccountService.Auth loggedIn = accounts.login("player_1", "secret1");
        AccountService.Auth resumed = accounts.resume(loggedIn.token());

        // Assert
        assertEquals("player_1", registered.username());
        assertEquals("Hiệp Khách", registered.heroName());
        assertEquals(registered.accountId(), resumed.accountId());
    }

    @Test
    void duplicateAndInvalidCredentialsUseProtocolCodes()
    {
        // Arrange
        accounts.register("player", "secret1", "Hero");

        // Act
        ServiceException duplicate = assertThrows(ServiceException.class,
                () -> accounts.register("PLAYER", "secret1", "Other"));
        ServiceException wrong = assertThrows(ServiceException.class, () -> accounts.login("player", "wrong1"));

        // Assert
        assertEquals("name_taken", duplicate.code());
        assertEquals("auth_failed", wrong.code());
    }

    @Test
    void validatesNameAndPassword()
    {
        // Act
        ServiceException name = assertThrows(ServiceException.class,
                () -> accounts.register("A!", "secret1", "Hero"));
        ServiceException password = assertThrows(ServiceException.class,
                () -> accounts.register("valid", "short", "Hero"));

        // Assert
        assertEquals("invalid_name", name.code());
        assertEquals("weak_password", password.code());
    }

    @Test
    void expiredTokenIsRejected() throws Exception
    {
        // Arrange
        AccountService.Auth auth = accounts.register("player", "secret1", "Hero");
        try (Connection connection = database.open())
        {
            connection.createStatement().executeUpdate(
                    "UPDATE sessions SET expires_at = '2020-01-01T00:00:00Z'");
        }

        // Act
        ServiceException exception = assertThrows(ServiceException.class, () -> accounts.resume(auth.token()));

        // Assert
        assertEquals("token_expired", exception.code());
    }
}
