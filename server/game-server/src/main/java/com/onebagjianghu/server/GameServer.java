package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.Loadout;
import org.java_websocket.WebSocket;
import org.java_websocket.handshake.ClientHandshake;
import org.java_websocket.server.WebSocketServer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.net.InetSocketAddress;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CountDownLatch;
import java.util.stream.Stream;

public final class GameServer extends WebSocketServer {
    private static final Logger LOG = LoggerFactory.getLogger(GameServer.class);
    private final ObjectMapper mapper = ItemCatalog.mapper();
    private final MessageHandler handler;
    private final Map<WebSocket, SessionState> sessions = new ConcurrentHashMap<>();

    public GameServer(int port, Path samples) { this(port, loadHandler(samples, new Random())); }
    public GameServer(int port, MessageHandler handler) {
        super(new InetSocketAddress(port));
        this.handler = handler;
    }

    private static MessageHandler loadHandler(Path samples, Random random) {
        ItemCatalog items = ItemCatalog.loadDefault();
        SectCatalog sects = SectCatalog.loadDefault();
        List<Loadout> loadouts;
        try (Stream<Path> files = Files.list(samples)) {
            loadouts = files.filter(p -> p.getFileName().toString().endsWith(".json"))
                    .sorted(Comparator.comparing(Path::toString)).map(Loadout::load).toList();
        } catch (Exception e) {
            if (e instanceof DataException data) throw data;
            throw new DataException("Cannot read samples directory " + samples + ": " + e.getMessage(), e);
        }
        return new MessageHandler(items, sects, loadouts, random);
    }

    @Override public void onOpen(WebSocket connection, ClientHandshake handshake) {
        if (!"/ws".equals(handshake.getResourceDescriptor())) {
            connection.close(1008, "WebSocket endpoint is /ws");
            return;
        }
        sessions.put(connection, new SessionState());
    }

    @Override public void onMessage(WebSocket connection, String message) {
        SessionState session = sessions.get(connection);
        if (session == null) return;
        try {
            Envelope request = mapper.readValue(message, Envelope.class);
            connection.send(mapper.writeValueAsString(handler.handle(request, session)));
        } catch (Exception e) {
            LOG.debug("Bad JSON message", e);
            try {
                var payload = mapper.createObjectNode().put("code", "bad_request").put("message", "Invalid JSON message");
                connection.send(mapper.writeValueAsString(Envelope.reply("error", null, payload)));
            } catch (Exception serializationFailure) {
                LOG.error("Could not serialize error response", serializationFailure);
            }
        }
    }

    @Override public void onClose(WebSocket connection, int code, String reason, boolean remote) { sessions.remove(connection); }
    @Override public void onError(WebSocket connection, Exception error) { LOG.error("WebSocket error", error); }
    @Override public void onStart() { LOG.info("Game server listening on port {} at /ws", getPort()); }

    public static void main(String[] args) throws Exception {
        int port = Integer.parseInt(option(args, "--port", env("PORT", "8765")));
        Path samples = Path.of(option(args, "--samples", env("SAMPLES", "battle-engine/samples")));
        GameServer server = new GameServer(port, samples);
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            try { server.stop(1000); } catch (InterruptedException e) { Thread.currentThread().interrupt(); }
        }));
        server.start();
        new CountDownLatch(1).await();
    }

    private static String env(String name, String fallback) {
        String value = System.getenv(name);
        return value == null || value.isBlank() ? fallback : value;
    }
    private static String option(String[] args, String name, String fallback) {
        for (int i = 0; i < args.length; i++) if (name.equals(args[i])) {
            if (i + 1 >= args.length) throw new IllegalArgumentException(name + " requires a value");
            return args[i + 1];
        }
        return fallback;
    }
}
