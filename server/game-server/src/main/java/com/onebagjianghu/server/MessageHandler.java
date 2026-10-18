package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.nio.file.Files;
import java.security.SecureRandom;
import java.time.Clock;
import java.util.List;
import java.util.Objects;
import java.util.Random;
import java.util.UUID;
import java.util.function.Supplier;

/** Pure request dispatcher; socket and persistence work live in services. */
public final class MessageHandler
{
    private static final Logger LOG = LoggerFactory.getLogger(MessageHandler.class);
    private static final String VERSION = "0.1.0";
    private final ObjectMapper mapper;
    private final BattleSimulator simulator;
    private final List<Loadout> opponents;
    private final Random random;
    private final Supplier<String> sessionIds;
    private final LoadoutNormalizer loadouts;
    private final AccountService accounts;
    private final ArenaService arena;
    private final Clock clock;

    public MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random)
    {
        this(items, sects, opponents, random, () -> UUID.randomUUID().toString());
    }

    MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random,
            Supplier<String> sessionIds)
    {
        this(items, sects, opponents, random, sessionIds, temporaryDatabase(), Clock.systemUTC(), new SecureRandom());
    }

    public MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random,
            Supplier<String> sessionIds, Database database, Clock clock, SecureRandom secureRandom)
    {
        mapper = ItemCatalog.mapper();
        this.opponents = List.copyOf(opponents);
        if (this.opponents.isEmpty())
        {
            throw new IllegalArgumentException("At least one sample loadout is required");
        }
        this.random = Objects.requireNonNull(random);
        this.sessionIds = Objects.requireNonNull(sessionIds);
        this.clock = clock;
        simulator = new BattleSimulator(Objects.requireNonNull(items), Objects.requireNonNull(sects),
                BattleConfig.defaults());
        loadouts = new LoadoutNormalizer(mapper, sects);
        accounts = new AccountService(database, secureRandom, clock);
        arena = new ArenaService(database, mapper, simulator, opponents, random, new ArenaCalendar(clock));
    }

    public Envelope handle(Envelope request, SessionState session)
    {
        Long replyTo = request == null ? null : request.id();
        try
        {
            if (request == null || request.type() == null || request.type().isBlank())
            {
                return error(replyTo, "bad_request", "Message type is required");
            }
            if (!session.isReady() && !"hello".equals(request.type()))
            {
                return error(replyTo, "not_ready", "Send hello before other requests");
            }
            return dispatch(request, session);
        }
        catch (ServiceException exception)
        {
            return error(replyTo, exception.code(), exception.getMessage());
        }
        catch (DataException exception)
        {
            return error(replyTo, "invalid_loadout", exception.getMessage());
        }
        catch (IllegalArgumentException exception)
        {
            return error(replyTo, "bad_request", exception.getMessage());
        }
        catch (Exception exception)
        {
            LOG.error("Unhandled request failure", exception);
            return error(replyTo, "internal", "Internal server error");
        }
    }

    private Envelope dispatch(Envelope request, SessionState session)
    {
        return switch (request.type())
        {
            case "hello" -> hello(request, session);
            case "ping" -> Envelope.reply("pong", request.id(), mapper.createObjectNode());
            case "battle_request" -> oldBattle(request);
            case "register" -> register(request, session);
            case "login" -> login(request, session);
            case "resume" -> resume(request, session);
            case "logout" -> logout(request, session);
            case "arena_status" -> arenaStatus(request, session);
            case "arena_battle" -> arenaBattle(request, session);
            case "arena_history" -> arenaHistory(request, session);
            case "arena_replay" -> arenaReplay(request, session);
            case "leaderboard" -> leaderboard(request, session);
            default -> error(request.id(), "bad_request", "Unknown message type: " + request.type());
        };
    }

    private Envelope hello(Envelope request, SessionState session)
    {
        String id = session.isReady() ? session.sessionId() : sessionIds.get();
        session.ready(id);
        ObjectNode payload = mapper.createObjectNode();
        payload.put("serverVersion", VERSION);
        payload.put("sessionId", id);
        return Envelope.reply("hello_ok", request.id(), payload);
    }

    private Envelope register(Envelope request, SessionState session)
    {
        JsonNode payload = payload(request);
        AccountService.Auth auth = accounts.register(payload.path("username").asText(null),
                payload.path("password").asText(null), payload.path("heroName").asText(null));
        return authenticated(request, session, auth);
    }

    private Envelope login(Envelope request, SessionState session)
    {
        if (!session.loginAllowed(clock.instant()))
        {
            throw new ServiceException("rate_limited", "Too many failed login attempts");
        }
        JsonNode payload = payload(request);
        try
        {
            AccountService.Auth auth = accounts.login(payload.path("username").asText(""),
                    payload.path("password").asText(""));
            return authenticated(request, session, auth);
        }
        catch (ServiceException exception)
        {
            if ("auth_failed".equals(exception.code()))
            {
                session.loginFailed(clock.instant());
            }
            throw exception;
        }
    }

    private Envelope resume(Envelope request, SessionState session)
    {
        AccountService.Auth auth = accounts.resume(payload(request).path("token").asText(""));
        return authenticated(request, session, auth);
    }

    private Envelope authenticated(Envelope request, SessionState session, AccountService.Auth auth)
    {
        session.authenticated(auth.accountId(), auth.token());
        ObjectNode payload = mapper.createObjectNode();
        payload.put("accountId", auth.accountId());
        payload.put("username", auth.username());
        payload.put("heroName", auth.heroName());
        payload.put("token", auth.token());
        payload.put("expiresAt", auth.expiresAt().toString());
        return Envelope.reply("auth_ok", request.id(), payload);
    }

    private Envelope logout(Envelope request, SessionState session)
    {
        accounts.logout(session.token());
        session.logout();
        return Envelope.reply("logout_ok", request.id(), mapper.createObjectNode());
    }

    private Envelope arenaStatus(Envelope request, SessionState session)
    {
        requireAuth(session);
        return Envelope.reply("arena_status_ok", request.id(), arena.status(session.accountId()));
    }

    private Envelope leaderboard(Envelope request, SessionState session)
    {
        requireAuth(session);
        int limit = payload(request).path("limit").asInt(50);
        return Envelope.reply("leaderboard_ok", request.id(), arena.leaderboard(session.accountId(), limit));
    }

    private Envelope arenaBattle(Envelope request, SessionState session)
    {
        requireAuth(session);
        JsonNode payload = payload(request);
        Loadout requester = loadouts.normalize(payload.get("loadout"), true);
        ObjectNode result = arena.battle(session.accountId(), requester, payload.path("power").asInt(-1));
        return Envelope.reply("arena_result", request.id(), result);
    }

    private Envelope arenaHistory(Envelope request, SessionState session)
    {
        requireAuth(session);
        int limit = payload(request).path("limit").asInt(20);
        return Envelope.reply("arena_history_ok", request.id(), arena.history(session.accountId(), limit));
    }

    private Envelope arenaReplay(Envelope request, SessionState session)
    {
        requireAuth(session);
        long battleId = payload(request).path("battleId").asLong(-1);
        if (battleId < 1)
        {
            throw new IllegalArgumentException("battleId must be positive");
        }
        return Envelope.reply("arena_replay_ok", request.id(), arena.replay(session.accountId(), battleId));
    }

    private Envelope oldBattle(Envelope request)
    {
        JsonNode payload = payload(request);
        String mode = payload.path("mode").asText("");
        if (!mode.equals("pve") && !mode.equals("pvp"))
        {
            throw new IllegalArgumentException("mode must be pve or pvp");
        }
        Loadout requester = loadouts.normalize(payload.get("loadout"), false);
        Loadout opponent;
        long seed;
        synchronized (random)
        {
            opponent = opponents.get(random.nextInt(opponents.size()));
            seed = random.nextLong();
        }
        BattleResult result = simulator.run(requester, opponent, seed);
        ObjectNode opponentJson = mapper.createObjectNode();
        opponentJson.put("name", opponent.name);
        if (opponent.sect == null)
        {
            opponentJson.putNull("sect");
        }
        else
        {
            opponentJson.put("sect", opponent.sect);
        }
        ObjectNode response = mapper.createObjectNode();
        response.set("opponent", opponentJson);
        response.set("result", mapper.valueToTree(result));
        return Envelope.reply("battle_result", request.id(), response);
    }

    private static void requireAuth(SessionState session)
    {
        if (!session.isAuthenticated())
        {
            throw new ServiceException("unauthorized", "Login is required");
        }
    }

    private static JsonNode payload(Envelope request)
    {
        if (request.payload() == null || !request.payload().isObject())
        {
            throw new IllegalArgumentException("payload is required");
        }
        return request.payload();
    }

    private Envelope error(Long replyTo, String code, String message)
    {
        ObjectNode payload = mapper.createObjectNode();
        payload.put("code", code);
        payload.put("message", message == null ? "" : message);
        return Envelope.reply("error", replyTo, payload);
    }

    private static Database temporaryDatabase()
    {
        try
        {
            return new Database(Files.createTempFile("jianghu-handler-", ".db"));
        }
        catch (Exception exception)
        {
            throw new IllegalStateException(exception);
        }
    }
}
