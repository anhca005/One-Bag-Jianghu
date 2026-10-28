package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.Loadout;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.nio.file.Path;
import java.security.SecureRandom;
import java.sql.Connection;
import java.sql.Statement;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ArenaQueueTest
{
    @TempDir
    Path temporaryDirectory;
    private final ObjectMapper mapper = ItemCatalog.mapper();
    private final MutableClock clock = new MutableClock();
    private final ScheduledExecutorService timer = Executors.newSingleThreadScheduledExecutor();
    private Database database;
    private MessageHandler handler;
    private ObjectNode loadout;

    @BeforeEach
    void setUp()
    {
        database = new Database(temporaryDirectory.resolve("queue.db"));
        Loadout sample = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        loadout = mapper.valueToTree(sample);
        handler = new MessageHandler(ItemCatalog.loadDefault(), SectCatalog.loadDefault(), List.of(sample),
                new Random(4), () -> "session", database, clock, new SecureRandom(), timer);
    }

    @AfterEach
    void tearDown()
    {
        timer.shutdownNow();
    }

    @Test
    void ghostInsideFiftyMatchesImmediatelyAndSoloPlayerGetsBot()
    {
        SessionState first = registered("first");
        Envelope solo = find(first);
        SessionState second = registered("second");
        Envelope matched = find(second);

        assertEquals("arena_result", solo.type());
        assertTrue(solo.payload().path("opponent").path("bot").asBoolean());
        assertEquals("arena_result", matched.type());
        assertEquals(false, matched.payload().path("opponent").path("bot").asBoolean());
    }

    @Test
    void widensAfterTwoMinutesThenMatchesGhostNinetyPointsAway() throws Exception
    {
        SessionState distant = registered("distant");
        find(distant);
        try (Connection connection = database.open(); Statement statement = connection.createStatement())
        {
            statement.executeUpdate("UPDATE arena_players SET rating = 1090 WHERE account_id = 1");
            statement.executeUpdate("UPDATE ghosts SET rating = 1090 WHERE account_id = 1");
        }
        SessionState seeker = registered("seeker");
        List<Envelope> pushed = new ArrayList<>();
        seeker.sender(pushed::add);

        Envelope waiting = find(seeker);
        clock.advanceSeconds(120);
        handler.checkSearches();

        assertEquals("arena_searching", waiting.type());
        assertEquals(100, pushed.get(0).payload().path("range").asInt());
        assertEquals("arena_result", pushed.get(1).type());
        assertEquals(0L, pushed.get(1).replyTo());
    }

    @Test
    void cancelDuplicateAndDisconnectRemoveSearch() throws Exception
    {
        SessionState distant = registered("far");
        find(distant);
        try (Connection connection = database.open(); Statement statement = connection.createStatement())
        {
            statement.executeUpdate("UPDATE ghosts SET rating = 1200 WHERE account_id = 1");
        }
        SessionState seeker = registered("waiting");
        assertEquals("arena_searching", find(seeker).type());
        assertEquals("already_searching", find(seeker).payload().path("code").asText());
        assertEquals("arena_cancel_ok", request("arena_cancel", mapper.createObjectNode(), seeker).type());
        assertEquals("arena_searching", find(seeker).type());
        handler.disconnected(seeker);
        assertEquals("arena_searching", find(seeker).type());
    }

    private SessionState registered(String username)
    {
        SessionState session = new SessionState();
        request("hello", mapper.createObjectNode(), session);
        ObjectNode payload = mapper.createObjectNode();
        payload.put("username", username);
        payload.put("password", "secret1");
        payload.put("heroName", username);
        assertEquals("auth_ok", request("register", payload, session).type());
        return session;
    }

    private Envelope find(SessionState session)
    {
        ObjectNode payload = mapper.createObjectNode();
        payload.set("loadout", loadout);
        payload.put("power", 1000);
        return request("arena_find", payload, session);
    }

    private Envelope request(String type, ObjectNode payload, SessionState session)
    {
        return handler.handle(new Envelope(type, 7L, null, payload), session);
    }

    private static final class MutableClock extends Clock
    {
        private Instant now = Instant.parse("2026-10-21T00:00:00Z");

        void advanceSeconds(long seconds)
        {
            now = now.plusSeconds(seconds);
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
