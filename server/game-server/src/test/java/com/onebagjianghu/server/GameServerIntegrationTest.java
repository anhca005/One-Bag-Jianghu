package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.onebagjianghu.battle.data.ItemCatalog;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;
import org.junit.jupiter.api.Test;
import java.net.ServerSocket;
import java.net.URI;
import java.nio.file.Path;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicReference;
import static org.junit.jupiter.api.Assertions.*;

class GameServerIntegrationTest {
    @Test void helloThenBattleOverWebSocket() throws Exception {
        int port; try (ServerSocket socket = new ServerSocket(0)) { port = socket.getLocalPort(); }
        GameServer server = new GameServer(port, Path.of("../battle-engine/samples")); server.start();
        ObjectMapper mapper = ItemCatalog.mapper(); CountDownLatch received = new CountDownLatch(1); AtomicReference<JsonNode> result = new AtomicReference<>();
        WebSocketClient client = new WebSocketClient(new URI("ws://localhost:" + port + "/ws")) {
            @Override public void onOpen(ServerHandshake handshake) { send("{\"type\":\"hello\",\"id\":1,\"payload\":{\"clientVersion\":\"0.1.0\",\"heroName\":\"Test\"}}"); }
            @Override public void onMessage(String message) {
                try {
                    JsonNode envelope = mapper.readTree(message);
                    if ("hello_ok".equals(envelope.path("type").asText())) {
                        JsonNode loadout = mapper.readTree(Path.of("../battle-engine/samples/kim_cang.json").toFile());
                        var payload = mapper.createObjectNode().put("mode", "pve").set("loadout", loadout);
                        send(mapper.writeValueAsString(mapper.createObjectNode().put("type", "battle_request").put("id", 2).set("payload", payload)));
                    } else if ("battle_result".equals(envelope.path("type").asText())) { result.set(envelope); received.countDown(); }
                } catch (Exception e) { throw new RuntimeException(e); }
            }
            @Override public void onClose(int code, String reason, boolean remote) { }
            @Override public void onError(Exception error) { }
        };
        try {
            assertTrue(client.connectBlocking(10, TimeUnit.SECONDS)); assertTrue(received.await(10, TimeUnit.SECONDS), "battle_result timed out");
            assertEquals("battle_result", result.get().get("type").asText()); assertEquals(2, result.get().get("replyTo").asInt());
        } finally { client.closeBlocking(); server.stop(1000); }
    }
}
