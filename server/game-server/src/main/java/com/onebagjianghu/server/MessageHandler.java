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

import java.util.List;
import java.util.Objects;
import java.util.Random;
import java.util.UUID;
import java.util.function.Supplier;

/** Pure request processing; it has no socket or threading responsibilities. */
public final class MessageHandler {
    private static final Logger LOG = LoggerFactory.getLogger(MessageHandler.class);
    private static final String VERSION = "0.1.0";

    private final ObjectMapper mapper;
    private final BattleSimulator simulator;
    private final SectCatalog sects;
    private final List<Loadout> opponents;
    private final Random random;
    private final Supplier<String> sessionIds;

    public MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random) {
        this(items, sects, opponents, random, () -> UUID.randomUUID().toString());
    }

    MessageHandler(ItemCatalog items, SectCatalog sects, List<Loadout> opponents, Random random,
                   Supplier<String> sessionIds) {
        this.mapper = ItemCatalog.mapper();
        this.sects = Objects.requireNonNull(sects);
        this.simulator = new BattleSimulator(Objects.requireNonNull(items), sects, BattleConfig.defaults());
        this.opponents = List.copyOf(opponents);
        if (this.opponents.isEmpty()) throw new IllegalArgumentException("At least one sample loadout is required");
        this.random = Objects.requireNonNull(random);
        this.sessionIds = Objects.requireNonNull(sessionIds);
    }

    public Envelope handle(Envelope request, SessionState session) {
        Long replyTo = request == null ? null : request.id();
        try {
            if (request == null || request.type() == null || request.type().isBlank()) {
                return error(replyTo, "bad_request", "Message type is required");
            }
            if (!session.isReady() && !"hello".equals(request.type())) {
                return error(replyTo, "not_ready", "Send hello before other requests");
            }
            return switch (request.type()) {
                case "hello" -> hello(request, session);
                case "ping" -> Envelope.reply("pong", replyTo, mapper.createObjectNode());
                case "battle_request" -> battle(request);
                default -> error(replyTo, "bad_request", "Unknown message type: " + request.type());
            };
        } catch (DataException e) {
            return error(replyTo, "invalid_loadout", e.getMessage());
        } catch (IllegalArgumentException e) {
            return error(replyTo, "bad_request", e.getMessage());
        } catch (Exception e) {
            LOG.error("Unhandled request failure", e);
            return error(replyTo, "internal", "Internal server error");
        }
    }

    private Envelope hello(Envelope request, SessionState session) {
        String id = session.isReady() ? session.sessionId() : sessionIds.get();
        session.ready(id);
        ObjectNode payload = mapper.createObjectNode();
        payload.put("serverVersion", VERSION);
        payload.put("sessionId", id);
        return Envelope.reply("hello_ok", request.id(), payload);
    }

    private Envelope battle(Envelope request) {
        JsonNode payload = request.payload();
        if (payload == null || !payload.isObject()) throw new IllegalArgumentException("payload is required");
        String mode = payload.path("mode").asText("");
        if (!mode.equals("pve") && !mode.equals("pvp")) {
            throw new IllegalArgumentException("mode must be pve or pvp");
        }
        JsonNode supplied = payload.get("loadout");
        if (supplied == null || !supplied.isObject()) throw new IllegalArgumentException("loadout is required");
        if (supplied.has("hp")) {
            throw new DataException("Loadout " + supplied.path("name").asText("?")
                    + ": \"hp\" was removed (decision #35); use \"level\"");
        }
        ObjectNode normalized = ((ObjectNode) supplied).deepCopy();
        normalized.put("level", 1);
        normalized.put("cols", 5);
        normalized.put("rows", 4);
        if (!normalized.has("tamPhapLevel") || normalized.get("tamPhapLevel").isNull()) {
            normalized.put("tamPhapLevel", 1);
        }
        Loadout requester;
        try {
            requester = mapper.treeToValue(normalized, Loadout.class);
        } catch (Exception e) {
            throw new DataException("Cannot read loadout: " + e.getMessage(), e);
        }
        if (requester.sect != null) sects.get(requester.sect);

        Loadout opponent;
        long seed;
        synchronized (random) {
            opponent = opponents.get(random.nextInt(opponents.size()));
            seed = random.nextLong();
        }
        BattleResult result = simulator.run(requester, opponent, seed);
        ObjectNode opponentJson = mapper.createObjectNode();
        opponentJson.put("name", opponent.name);
        if (opponent.sect == null) opponentJson.putNull("sect"); else opponentJson.put("sect", opponent.sect);
        ObjectNode response = mapper.createObjectNode();
        response.set("opponent", opponentJson);
        response.set("result", mapper.valueToTree(result));
        return Envelope.reply("battle_result", request.id(), response);
    }

    private Envelope error(Long replyTo, String code, String message) {
        ObjectNode payload = mapper.createObjectNode();
        payload.put("code", code);
        payload.put("message", message == null ? "" : message);
        return Envelope.reply("error", replyTo, payload);
    }
}
