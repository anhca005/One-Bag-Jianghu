package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.Loadout;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import java.nio.file.Path;
import java.util.List;
import java.util.Random;
import static org.junit.jupiter.api.Assertions.*;

class MessageHandlerTest {
    private final ObjectMapper mapper = ItemCatalog.mapper();
    private MessageHandler handler;
    private SessionState session;
    private JsonNode valid;

    @BeforeEach void setUp() {
        Loadout sample = Loadout.load(Path.of("../battle-engine/samples/kim_cang.json"));
        handler = new MessageHandler(ItemCatalog.loadDefault(), SectCatalog.loadDefault(), List.of(sample),
                new Random(7), () -> "fixed-session");
        session = new SessionState();
        valid = mapper.valueToTree(sample);
    }
    @Test void helloMakesSessionReady() {
        Envelope response = handler.handle(request("hello", mapper.createObjectNode()), session);
        assertEquals("hello_ok", response.type()); assertEquals(7L, response.replyTo());
        assertEquals("0.1.0", response.payload().get("serverVersion").asText());
        assertEquals("fixed-session", response.payload().get("sessionId").asText()); assertTrue(session.isReady());
    }
    @Test void requestBeforeHelloIsNotReady() { assertError(handler.handle(request("battle_request", battlePayload(valid)), session), "not_ready"); }
    @Test void invalidOverlapIsReported() {
        ready(); ObjectNode loadout = valid.deepCopy();
        ((ObjectNode) loadout.withArray("items").get(1)).put("x", 0).put("y", 0);
        assertError(handler.handle(request("battle_request", battlePayload(loadout)), session), "invalid_loadout");
    }
    @Test void unknownItemIsReported() {
        ready(); ObjectNode loadout = valid.deepCopy();
        ((ObjectNode) loadout.withArray("items").get(0)).put("id", "missing_item");
        assertError(handler.handle(request("battle_request", battlePayload(loadout)), session), "invalid_loadout");
    }
    @Test void unknownSectIsReported() {
        ready(); ObjectNode loadout = valid.deepCopy();
        loadout.put("sect", "missing_sect");
        assertError(handler.handle(request("battle_request", battlePayload(loadout)), session), "invalid_loadout");
    }
    @Test void battleReturnsRequesterFirstAndForcesServerStats() {
        ready(); ObjectNode loadout = valid.deepCopy();
        loadout.put("name", "Requester").put("level", 999999).put("cols", 50).put("rows", 40); loadout.remove("tamPhapLevel");
        Envelope response = handler.handle(request("battle_request", battlePayload(loadout)), session);
        assertEquals("battle_result", response.type()); JsonNode result = response.payload().get("result");
        assertEquals("Requester", result.get("names").get(0).asText());
        assertEquals(5, result.get("bags").get(0).get(0).asInt()); assertEquals(4, result.get("bags").get(0).get(1).asInt());
        JsonNode events = result.get("events"); assertEquals("END", events.get(events.size() - 1).get("type").asText());
        // The client's level 999999 would give ~5M Khí Huyết; the server forces level 1 (base 100 + item stats).
        assertTrue(events.findValues("maxHp").stream().allMatch(n -> n.asInt() < 2000));
    }
    @Test void unknownTypeIsBadRequest() { ready(); assertError(handler.handle(request("wat", mapper.createObjectNode()), session), "bad_request"); }
    private void ready() { handler.handle(request("hello", mapper.createObjectNode()), session); }
    private Envelope request(String type, JsonNode payload) { return new Envelope(type, 7L, null, payload); }
    private ObjectNode battlePayload(JsonNode loadout) { return mapper.createObjectNode().put("mode", "pve").set("loadout", loadout); }
    private static void assertError(Envelope response, String code) { assertEquals("error", response.type()); assertEquals(code, response.payload().get("code").asText()); }
}
