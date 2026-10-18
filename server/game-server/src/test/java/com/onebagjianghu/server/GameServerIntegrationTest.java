package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.onebagjianghu.battle.data.ItemCatalog;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.net.ServerSocket;
import java.net.URI;
import java.nio.file.Path;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicReference;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class GameServerIntegrationTest
{
    @TempDir
    Path temporaryDirectory;

    @Test
    void helloThenBattleOverWebSocket() throws Exception
    {
        // Arrange
        int port;
        try (ServerSocket socket = new ServerSocket(0))
        {
            port = socket.getLocalPort();
        }
        Database database = new Database(temporaryDirectory.resolve("jianghu.db"));
        GameServer server = new GameServer(port, Path.of("../battle-engine/samples"), database);
        server.start();
        ObjectMapper mapper = ItemCatalog.mapper();
        CountDownLatch received = new CountDownLatch(1);
        AtomicReference<JsonNode> result = new AtomicReference<>();
        WebSocketClient client = client(port, mapper, received, result);

        try
        {
            // Act
            assertTrue(client.connectBlocking(10, TimeUnit.SECONDS));
            assertTrue(received.await(10, TimeUnit.SECONDS), "battle_result timed out");

            // Assert
            assertEquals("battle_result", result.get().get("type").asText());
            assertEquals(2, result.get().get("replyTo").asInt());
        }
        finally
        {
            client.closeBlocking();
            server.stop(1000);
        }
    }

    private static WebSocketClient client(int port, ObjectMapper mapper, CountDownLatch received,
            AtomicReference<JsonNode> result) throws Exception
    {
        return new WebSocketClient(new URI("ws://localhost:" + port + "/ws"))
        {
            @Override
            public void onOpen(ServerHandshake handshake)
            {
                send("{\"type\":\"hello\",\"id\":1,\"payload\":{}}");
            }

            @Override
            public void onMessage(String message)
            {
                try
                {
                    JsonNode envelope = mapper.readTree(message);
                    if ("hello_ok".equals(envelope.path("type").asText()))
                    {
                        sendBattle(mapper);
                    }
                    else if ("battle_result".equals(envelope.path("type").asText()))
                    {
                        result.set(envelope);
                        received.countDown();
                    }
                }
                catch (Exception exception)
                {
                    throw new RuntimeException(exception);
                }
            }

            private void sendBattle(ObjectMapper objectMapper) throws Exception
            {
                Path sample = Path.of("../battle-engine/samples/kim_cang.json");
                JsonNode loadout = objectMapper.readTree(sample.toFile());
                JsonNode payload = objectMapper.createObjectNode().put("mode", "pve").set("loadout", loadout);
                JsonNode request = objectMapper.createObjectNode().put("type", "battle_request").put("id", 2)
                        .set("payload", payload);
                send(objectMapper.writeValueAsString(request));
            }

            @Override
            public void onClose(int code, String reason, boolean remote)
            {
            }

            @Override
            public void onError(Exception error)
            {
            }
        };
    }
}
