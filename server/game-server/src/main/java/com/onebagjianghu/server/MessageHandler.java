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
import java.time.Duration;
import java.time.Instant;
import java.util.List;
import java.util.Objects;
import java.util.Random;
import java.util.UUID;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
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
    private final ProfileService profiles;
    private final Clock clock;
    private final Map<Long, Search> searches = new ConcurrentHashMap<>();
    private final ScheduledExecutorService searchTimer;

    private record Search(String id, long accountId, Loadout loadout, int power, Instant startedAt,
            SessionState session, int range) { }

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
        this(items, sects, opponents, random, sessionIds, database, clock, secureRandom,
                Executors.newSingleThreadScheduledExecutor(task ->
                {
                    Thread thread = new Thread(task, "arena-matchmaker");
                    thread.setDaemon(true);
                    return thread;
                }), new GameData(GameServer.defaultDataPath()), false);
    }

    MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random,
            Supplier<String> sessionIds, Database database, Clock clock, SecureRandom secureRandom,
            ScheduledExecutorService searchTimer)
    {
        this(items, sects, opponents, random, sessionIds, database, clock, secureRandom, searchTimer,
                new GameData(GameServer.defaultDataPath()), false);
    }

    MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random,
            Supplier<String> sessionIds, Database database, Clock clock, SecureRandom secureRandom,
            ScheduledExecutorService searchTimer, GameData gameData, boolean dev)
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
        profiles = new ProfileService(database, gameData, clock, dev);
        arena = new ArenaService(database, mapper, simulator, opponents, random, new ArenaCalendar(clock), gameData,
                profiles);
        this.searchTimer = searchTimer;
        arena.maintainSeasons();
        searchTimer.scheduleAtFixedRate(this::checkSearches, 5, 5, TimeUnit.SECONDS);
        searchTimer.scheduleAtFixedRate(this::maintainSeasons, 1, 1, TimeUnit.MINUTES);
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
            Envelope reply = dispatch(request, session);
            logRequest(session, request.type(), reply.type());
            return reply;
        }
        catch (ServiceException exception)
        {
            logRequest(session, request.type(), exception.code() + " (" + exception.getMessage() + ")");
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

    // One line per request so the console shows who did what; pings would drown everything else.
    private static void logRequest(SessionState session, String type, String outcome)
    {
        if ("ping".equals(type))
        {
            return;
        }
        String who = session.isAuthenticated() ? "account " + session.accountId() : "guest";
        LOG.info("{} {} -> {}", who, type, outcome);
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
            case "profile_get" -> profileGet(request, session);
            case "loadout_save" -> profileMutation(request, session, profiles.loadoutSave(payload(request)));
            case "storage_move" -> profileMutation(request, session, profiles.storageMove(payload(request)));
            case "storage_repack" -> profileMutation(request, session, profiles.storageRepack());
            case "item_upgrade" -> profileMutation(request, session,
                    profiles.upgrade(payload(request).path("uid").asText(null)));
            case "item_merge" -> profileMutation(request, session, profiles.merge(
                    payload(request).path("firstUid").asText(null),
                    payload(request).path("secondUid").asText(null)));
            case "item_sell" -> profileMutation(request, session,
                    profiles.sell(payload(request).path("uid").asText(null)));
            case "shop_refresh" -> profileMutation(request, session,
                    profiles.shopRefresh(payload(request).path("paid").asBoolean()));
            case "shop_buy" -> profileMutation(request, session,
                    profiles.shopBuy(payload(request).path("slot").asInt(-1)));
            case "shop_exchange" -> profileMutation(request, session, profiles.shopExchange(
                    payload(request).path("inputId").asText(null),
                    payload(request).path("targetId").asText(null)));
            case "shop_buy_reputation" -> profileMutation(request, session,
                    profiles.shopBuyReputation(payload(request).path("offerId").asText(null)));
            case "pve_battle" -> profileMutation(request, session,
                    profiles.pveBattle(payload(request).path("stageId").asText(null)));
            case "quest_claim" -> profileMutation(request, session,
                    profiles.questClaim(payload(request).path("questId").asText(null)));
            case "sect_choose" -> profileMutation(request, session,
                    profiles.sectChoose(payload(request).path("sectId").asText(null)));
            case "onboarding" -> profileMutation(request, session, profiles.onboarding(
                    payload(request).path("action").asText(null), payload(request).path("id")));
            case "dev_cheat" -> profileMutation(request, session,
                    profiles.cheat(payload(request), accounts.heroName(session.accountId())));
            case "arena_status" -> arenaStatus(request, session);
            case "season_reward_seen" -> seasonRewardSeen(request, session);
            case "arena_find" -> arenaFind(request, session);
            case "arena_cancel" -> arenaCancel(request, session);
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
        ProfileService.Snapshot profile = profiles.ensure(auth.accountId(), auth.heroName());
        payload.set("profile", profile.profile());
        payload.put("rev", profile.rev());
        payload.put("serverTime", clock.instant().toString());
        ObjectNode seasonReward = arena.unseenSeasonReward(auth.accountId());
        if (seasonReward != null)
        {
            payload.set("seasonReward", seasonReward);
        }
        return Envelope.reply("auth_ok", request.id(), payload);
    }

    private Envelope profileGet(Envelope request, SessionState session)
    {
        requireAuth(session);
        return Envelope.reply("profile_get_ok", request.id(), profiles.response(profiles.get(session.accountId())));
    }

    private Envelope profileMutation(Envelope request, SessionState session, ProfileService.Rule rule)
    {
        requireAuth(session);
        return Envelope.reply(request.type() + "_ok", request.id(), profiles.mutate(session.accountId(), rule));
    }

    private Envelope logout(Envelope request, SessionState session)
    {
        if (session.isAuthenticated())
        {
            searches.remove(session.accountId());
        }
        accounts.logout(session.token());
        session.logout();
        return Envelope.reply("logout_ok", request.id(), mapper.createObjectNode());
    }

    private Envelope arenaStatus(Envelope request, SessionState session)
    {
        requireAuth(session);
        return Envelope.reply("arena_status_ok", request.id(), arena.status(session.accountId()));
    }

    private Envelope seasonRewardSeen(Envelope request, SessionState session)
    {
        requireAuth(session);
        long seasonId = payload(request).path("seasonId").asLong(-1);
        return Envelope.reply("season_reward_seen_ok", request.id(),
                arena.markSeasonRewardSeen(session.accountId(), seasonId));
    }

    private void maintainSeasons()
    {
        try
        {
            arena.maintainSeasons();
        }
        catch (Exception exception)
        {
            LOG.error("Could not maintain arena seasons", exception);
        }
    }

    private Envelope leaderboard(Envelope request, SessionState session)
    {
        requireAuth(session);
        int limit = payload(request).path("limit").asInt(50);
        return Envelope.reply("leaderboard_ok", request.id(), arena.leaderboard(session.accountId(), limit));
    }

    private Envelope arenaFind(Envelope request, SessionState session)
    {
        requireAuth(session);
        if (searches.containsKey(session.accountId()))
        {
            throw new ServiceException("already_searching", "An arena search is already active");
        }
        payload(request);
        ProfileService.Snapshot profile = profiles.get(session.accountId());
        Loadout requester = loadouts.normalize(profiles.arenaLoadout(profile), true);
        int power = profiles.power(profile);
        String searchId = UUID.randomUUID().toString();
        ObjectNode result = arena.find(session.accountId(), requester, power, 50, searchId);
        if (result != null)
        {
            return Envelope.reply("arena_result", request.id(), profiles.applyArenaReward(session.accountId(), result));
        }
        Search search = new Search(searchId, session.accountId(), requester, power, clock.instant(), session, 50);
        searches.put(session.accountId(), search);
        return Envelope.reply("arena_searching", request.id(), searching(search));
    }

    private Envelope arenaCancel(Envelope request, SessionState session)
    {
        requireAuth(session);
        Search removed = searches.remove(session.accountId());
        ObjectNode payload = mapper.createObjectNode();
        if (removed == null)
        {
            payload.putNull("searchId");
        }
        else
        {
            payload.put("searchId", removed.id());
        }
        return Envelope.reply("arena_cancel_ok", request.id(), payload);
    }

    void disconnected(SessionState session)
    {
        if (session.isAuthenticated())
        {
            searches.remove(session.accountId());
        }
    }

    void checkSearches()
    {
        Instant now = clock.instant();
        for (Search oldSearch : List.copyOf(searches.values()))
        {
            long periods = Duration.between(oldSearch.startedAt(), now).toMinutes() / 2;
            int range = 50 + Math.toIntExact(periods) * 50;
            Search search = oldSearch;
            if (range != oldSearch.range())
            {
                search = new Search(oldSearch.id(), oldSearch.accountId(), oldSearch.loadout(), oldSearch.power(),
                        oldSearch.startedAt(), oldSearch.session(), range);
                if (!searches.replace(search.accountId(), oldSearch, search))
                {
                    continue;
                }
                search.session().send(Envelope.reply("arena_searching", 0L, searching(search)));
            }
            ObjectNode result = arena.find(search.accountId(), search.loadout(), search.power(), range, search.id());
            if (result != null && searches.remove(search.accountId(), search))
            {
                result = profiles.applyArenaReward(search.accountId(), result);
                search.session().send(Envelope.reply("arena_result", 0L, result));
            }
        }
    }

    private ObjectNode searching(Search search)
    {
        ObjectNode payload = mapper.createObjectNode();
        payload.put("searchId", search.id());
        payload.put("rating", arena.status(search.accountId()).path("rating").asInt());
        payload.put("range", search.range());
        payload.put("startedAt", search.startedAt().toString());
        payload.put("nextWidenAt", search.startedAt().plusSeconds(
                ((search.range() - 50) / 50 + 1L) * 120L).toString());
        return payload;
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
