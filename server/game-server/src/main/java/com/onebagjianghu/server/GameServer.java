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
import java.security.SecureRandom;
import java.time.Clock;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ScheduledExecutorService;
import java.util.stream.Stream;

public final class GameServer extends WebSocketServer {
    private static final Logger LOG = LoggerFactory.getLogger(GameServer.class);
    private final ObjectMapper mapper = ItemCatalog.mapper();
    private final MessageHandler handler;
    private final Map<WebSocket, SessionState> sessions = new ConcurrentHashMap<>();

    public GameServer(int port, Path samples)
    {
        this(port, samples, new Database(defaultDatabasePath()));
    }

    GameServer(int port, Path samples, Database database)
    {
        this(port, loadHandler(samples, new Random(), database));
    }
    public GameServer(int port, MessageHandler handler) {
        this("0.0.0.0", port, handler);
    }

    private GameServer(String host, int port, MessageHandler handler) {
        super(new InetSocketAddress(host, port));
        this.handler = handler;
    }

    private static MessageHandler loadHandler(Path samples, Random random, Database database) {
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
        return new MessageHandler(items, sects, loadouts, random, () -> java.util.UUID.randomUUID().toString(),
                database, Clock.systemUTC(), new SecureRandom());
    }

    @Override public void onOpen(WebSocket connection, ClientHandshake handshake) {
        if (!"/ws".equals(handshake.getResourceDescriptor())) {
            connection.close(1008, "WebSocket endpoint is /ws");
            return;
        }
        SessionState session = new SessionState();
        session.sender(envelope -> send(connection, envelope));
        sessions.put(connection, session);
        LOG.info("Client connected from {} ({} online)", connection.getRemoteSocketAddress(), sessions.size());
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
                var payload = mapper.createObjectNode()
                        .put("code", "bad_request")
                        .put("message", "Invalid JSON message");
                connection.send(mapper.writeValueAsString(Envelope.reply("error", null, payload)));
            } catch (Exception serializationFailure) {
                LOG.error("Could not serialize error response", serializationFailure);
            }
        }
    }

    @Override public void onClose(WebSocket connection, int code, String reason, boolean remote)
    {
        SessionState session = sessions.remove(connection);
        if (session != null)
        {
            handler.disconnected(session);
            LOG.info("Client disconnected ({} online)", sessions.size());
        }
    }
    @Override public void onError(WebSocket connection, Exception error) { LOG.error("WebSocket error", error); }
    @Override public void onStart() { LOG.info("Game server listening on port {} at /ws", getPort()); }

    private void send(WebSocket connection, Envelope envelope)
    {
        try
        {
            connection.send(mapper.writeValueAsString(envelope));
        }
        catch (Exception exception)
        {
            LOG.warn("Could not send proactive message", exception);
        }
    }

    public static void main(String[] args) throws Exception {
        System.setProperty("org.slf4j.simpleLogger.showDateTime", "true");
        System.setProperty("org.slf4j.simpleLogger.dateTimeFormat", "yyyy-MM-dd HH:mm:ss.SSS");
        int port = Integer.parseInt(option(args, "--port", env("JIANGHU_PORT", "8765")));
        String host = option(args, "--host", env("JIANGHU_HOST", "0.0.0.0"));
        Path samples = Path.of(option(args, "--samples", env("JIANGHU_SAMPLES", "battle-engine/samples")));
        String configuredData = option(args, "--data", env("JIANGHU_DATA", defaultDataPath().toString()));
        String configuredDatabase = option(args, "--db", env("JIANGHU_DB", null));
        Path databasePath = configuredDatabase == null ? defaultDatabasePath() : Path.of(configuredDatabase);
        GameData data = new GameData(Path.of(configuredData));
        boolean dev = flag(args, "--dev") || "1".equals(System.getenv("JIANGHU_DEV"));
        MessageHandler handler = loadHandler(samples, data, new Random(), new Database(databasePath), dev);
        GameServer server = new GameServer(host, port, handler);
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            try
            {
                server.stop(1000);
            }
            catch (InterruptedException exception)
            {
                Thread.currentThread().interrupt();
            }
        }));
        server.start();
        new CountDownLatch(1).await();
    }

    private static MessageHandler loadHandler(Path samples, GameData data, Random random, Database database,
            boolean dev)
    {
        List<Loadout> loadouts;
        try (Stream<Path> files = Files.list(samples))
        {
            loadouts = files.filter(path -> path.getFileName().toString().endsWith(".json"))
                    .sorted(Comparator.comparing(Path::toString))
                    .map(Loadout::load)
                    .toList();
        }
        catch (Exception exception)
        {
            if (exception instanceof DataException dataException) throw dataException;
            throw new DataException("Cannot read samples directory " + samples + ": " + exception.getMessage(),
                    exception);
        }
        ScheduledExecutorService timer = java.util.concurrent.Executors.newSingleThreadScheduledExecutor();
        return new MessageHandler(data.itemCatalog(), data.sectCatalog(), loadouts, random,
                () -> java.util.UUID.randomUUID().toString(), database, Clock.systemUTC(), new SecureRandom(), timer,
                data, dev);
    }

    private static String env(String name, String fallback) {
        String value = System.getenv(name);
        return value == null || value.isBlank() ? fallback : value;
    }
    static Path defaultDatabasePath()
    {
        Path directory = Path.of("").toAbsolutePath().normalize();
        while (directory != null)
        {
            if (Files.exists(directory.resolve("mvnw")) || Files.exists(directory.resolve("mvnw.cmd")))
            {
                return directory.resolve("data/jianghu.db");
            }
            directory = directory.getParent();
        }
        return Path.of("server/data/jianghu.db").toAbsolutePath().normalize();
    }
    static Path defaultDataPath()
    {
        Path directory = Path.of("").toAbsolutePath().normalize();
        while (directory != null)
        {
            Path direct = directory.resolve("One-Bag-Jianghu/Assets/_Game/Resources/Data");
            if (Files.isDirectory(direct))
            {
                return direct.normalize();
            }
            directory = directory.getParent();
        }
        return Path.of("../One-Bag-Jianghu/Assets/_Game/Resources/Data").toAbsolutePath().normalize();
    }
    private static boolean flag(String[] args, String name)
    {
        for (String value : args)
        {
            if (name.equals(value))
            {
                return true;
            }
        }
        return false;
    }
    private static String option(String[] args, String name, String fallback) {
        for (int i = 0; i < args.length; i++) if (name.equals(args[i])) {
            if (i + 1 >= args.length) throw new IllegalArgumentException(name + " requires a value");
            return args[i + 1];
        }
        return fallback;
    }
}
