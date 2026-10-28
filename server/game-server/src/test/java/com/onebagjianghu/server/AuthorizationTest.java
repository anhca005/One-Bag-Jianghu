package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.Loadout;
import org.junit.jupiter.api.Test;

import java.nio.file.Path;
import java.util.List;
import java.util.Random;

import static org.junit.jupiter.api.Assertions.assertEquals;

class AuthorizationTest
{
    @Test
    void allArenaMessagesRequireAuthentication()
    {
        // Arrange
        ObjectMapper mapper = ItemCatalog.mapper();
        Loadout sample = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        MessageHandler handler = new MessageHandler(ItemCatalog.loadDefault(), SectCatalog.loadDefault(),
                List.of(sample), new Random(1));
        SessionState session = new SessionState();
        handler.handle(new Envelope("hello", 1L, null, mapper.createObjectNode()), session);

        // Act and assert
        assertUnauthorized(handler.handle(request("arena_status", mapper), session));
        assertUnauthorized(handler.handle(request("arena_find", mapper), session));
        assertUnauthorized(handler.handle(request("arena_cancel", mapper), session));
        assertUnauthorized(handler.handle(request("arena_history", mapper), session));
        assertUnauthorized(handler.handle(request("arena_replay", mapper), session));
        assertUnauthorized(handler.handle(request("leaderboard", mapper), session));
    }

    private static Envelope request(String type, ObjectMapper mapper)
    {
        return new Envelope(type, 2L, null, mapper.createObjectNode());
    }

    private static void assertUnauthorized(Envelope response)
    {
        assertEquals("error", response.type());
        assertEquals("unauthorized", response.payload().path("code").asText());
    }
}
