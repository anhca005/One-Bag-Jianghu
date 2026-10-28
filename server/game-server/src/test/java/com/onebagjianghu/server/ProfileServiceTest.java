package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.node.ObjectNode;
import com.fasterxml.jackson.databind.node.ArrayNode;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Path;
import java.security.SecureRandom;
import java.time.Clock;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

final class ProfileServiceTest
{
    @TempDir
    Path temporary;

    @Test
    void registrationCreatesStarterAndRoundTripsEveryField()
    {
        // Arrange
        Fixture fixture = fixture(false);

        // Act
        ProfileService.Snapshot created = fixture.profiles.ensure(fixture.accountId, "Thiếu Hiệp");
        ProfileService.Snapshot loaded = fixture.profiles.get(fixture.accountId);

        // Assert
        // Compare the JSON text: numbers built in memory and parsed from the database can differ in node type.
        assertEquals(created.profile().toString(), loaded.profile().toString());
        assertEquals(9, loaded.profile().path("version").asInt());
        assertEquals("Thiếu Hiệp", loaded.profile().path("heroName").asText());
        assertEquals(500, loaded.profile().path("currencies").path("bac").asLong());
        assertEquals(2, loaded.profile().path("items").size());
        assertNotNull(loaded.profile().get("quest"));
    }

    @Test
    void mutationIncrementsRevisionAndRejectedMutationDoesNot()
    {
        // Arrange
        Fixture fixture = fixture(false);
        fixture.profiles.ensure(fixture.accountId, "Thiếu Hiệp");

        // Act
        ObjectNode changed = fixture.profiles.mutate(fixture.accountId,
                fixture.profiles.storageMove(move("starter_sword", 4, 0)));
        ServiceException rejected = assertThrows(ServiceException.class, () -> fixture.profiles.mutate(
                fixture.accountId, fixture.profiles.upgrade("missing")));

        // Assert
        assertEquals(1, changed.path("rev").asLong());
        assertEquals("rejected", rejected.code());
        assertEquals("Không tìm thấy vật phẩm.", rejected.getMessage());
        assertEquals(1, fixture.profiles.get(fixture.accountId).rev());
    }

    @Test
    void concurrentMutationsDoNotLoseCurrency()
            throws Exception
    {
        // Arrange
        Fixture fixture = fixture(true);
        fixture.profiles.ensure(fixture.accountId, "Thiếu Hiệp");
        int requests = 12;
        ExecutorService executor = Executors.newFixedThreadPool(4);
        CountDownLatch start = new CountDownLatch(1);
        List<Runnable> work = new ArrayList<>();
        for (int i = 0; i < requests; i++)
        {
            work.add(() ->
            {
                await(start);
                fixture.profiles.mutate(fixture.accountId,
                        fixture.profiles.cheat(currencyCheat(1), "Thiếu Hiệp"));
            });
        }

        // Act
        work.forEach(executor::submit);
        start.countDown();
        executor.shutdown();
        assertTrue(executor.awaitTermination(10, TimeUnit.SECONDS));
        ProfileService.Snapshot result = fixture.profiles.get(fixture.accountId);

        // Assert
        assertEquals(500 + requests, result.profile().path("currencies").path("bac").asLong());
        assertEquals(requests, result.rev());
    }

    @Test
    void devCheatIsForbiddenOutsideDevMode()
    {
        // Arrange
        Fixture fixture = fixture(false);

        // Act
        ServiceException error = assertThrows(ServiceException.class,
                () -> fixture.profiles.cheat(currencyCheat(10), "Thiếu Hiệp"));

        // Assert
        assertEquals("forbidden", error.code());
    }

    @Test
    void playerPowerKeepsLegacyFormulaAndRounding()
    {
        // Arrange
        Fixture fixture = fixture(false);
        ProfileService.Snapshot created = fixture.profiles.ensure(fixture.accountId, "Power Hero");
        ObjectNode profile = created.profile().deepCopy();
        profile.put("heroLevel", 5);
        ArrayNode loadout = profile.putArray("loadout");
        loadout.addObject().put("uid", "starter_sword").put("x", 0).put("y", 0);
        loadout.addObject().put("uid", "starter_armor").put("x", 1).put("y", 0);

        // Act
        int power = fixture.profiles.power(new ProfileService.Snapshot(profile, created.rev()));

        // Assert
        assertEquals(112, power);
    }

    private Fixture fixture(boolean dev)
    {
        Database database = new Database(temporary.resolve(dev ? "dev.db" : "normal.db"));
        Clock clock = Clock.systemUTC();
        AccountService accounts = new AccountService(database, new SecureRandom(), clock);
        AccountService.Auth auth = accounts.register(dev ? "dev_user" : "normal_user", "secret1", "Thiếu Hiệp");
        ProfileService profiles = new ProfileService(database, new GameData(GameServer.defaultDataPath()), clock, dev);
        return new Fixture(profiles, auth.accountId());
    }

    private static ObjectNode move(String uid, int x, int y)
    {
        ObjectNode payload = com.onebagjianghu.battle.data.ItemCatalog.mapper().createObjectNode();
        payload.put("uid", uid);
        payload.put("toX", x);
        payload.put("toY", y);
        return payload;
    }

    private static ObjectNode currencyCheat(long amount)
    {
        ObjectNode payload = com.onebagjianghu.battle.data.ItemCatalog.mapper().createObjectNode();
        payload.put("action", "add_currency");
        payload.put("currencyId", "bac");
        payload.put("amount", amount);
        return payload;
    }

    private static void await(CountDownLatch latch)
    {
        try
        {
            latch.await();
        }
        catch (InterruptedException exception)
        {
            Thread.currentThread().interrupt();
            throw new IllegalStateException(exception);
        }
    }

    private record Fixture(ProfileService profiles, long accountId) { }
}
