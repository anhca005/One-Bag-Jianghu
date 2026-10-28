package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.time.Clock;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

/** Transaction boundary and rules for every server-owned profile mutation. */
public final class ProfileService
{
    @FunctionalInterface
    public interface Rule
    {
        void apply(ProfileDocument profile, ObjectNode result) throws Exception;
    }

    public record Snapshot(ObjectNode profile, long rev) { }

    private static final String NO_SPACE = "Tàng Bảo Các không đủ chỗ.";
    private static final long[] BASE_PRICES = { 200, 500, 1200, 3000, 8000, 15000 };
    private static final String[] RARITIES = { "common", "uncommon", "rare", "epic", "legendary", "supreme" };
    private final Database database;
    private final GameData data;
    private final ObjectMapper mapper;
    private final Clock clock;
    private final boolean dev;
    private final E6Rules e6;
    private final ConcurrentHashMap<Long, Object> locks = new ConcurrentHashMap<>();

    public ProfileService(Database database, GameData data, Clock clock, boolean dev)
    {
        this.database = database;
        this.data = data;
        this.clock = clock;
        this.dev = dev;
        mapper = ItemCatalogAccess.mapper();
        e6 = new E6Rules(data, clock, mapper);
    }

    public Snapshot ensure(long accountId, String heroName)
    {
        synchronized (lock(accountId))
        {
            try (Connection connection = database.open())
            {
                connection.setAutoCommit(false);
                Snapshot snapshot = read(connection, accountId);
                if (snapshot == null)
                {
                    // A new profile is stored with its shop, stamina and first quest already filled in.
                    ProfileDocument created = create(heroName);
                    e6.refreshPassive(created);
                    insert(connection, accountId, created);
                    connection.commit();
                    return new Snapshot(created.json(), 0);
                }
                snapshot = refreshOnRead(connection, accountId, snapshot);
                connection.commit();
                return snapshot;
            }
            catch (Exception exception)
            {
                throw wrap(exception);
            }
        }
    }

    public Snapshot get(long accountId)
    {
        synchronized (lock(accountId))
        {
            try (Connection connection = database.open())
            {
                connection.setAutoCommit(false);
                Snapshot result = read(connection, accountId);
                if (result == null)
                {
                    throw new IllegalStateException("Profile does not exist for account " + accountId);
                }
                result = refreshOnRead(connection, accountId, result);
                connection.commit();
                return result;
            }
            catch (Exception exception)
            {
                throw wrap(exception);
            }
        }
    }

    public ObjectNode mutate(long accountId, Rule rule)
    {
        synchronized (lock(accountId))
        {
            try (Connection connection = database.open())
            {
                connection.setAutoCommit(false);
                Snapshot old = read(connection, accountId);
                if (old == null)
                {
                    throw new IllegalStateException("Profile does not exist for account " + accountId);
                }
                ProfileDocument profile = new ProfileDocument(old.profile().deepCopy());
                e6.refreshPassive(profile);
                ObjectNode response = mapper.createObjectNode();
                rule.apply(profile, response);
                profile.validate(data);
                long revision = old.rev() + 1;
                update(connection, accountId, profile, revision);
                connection.commit();
                response.set("profile", profile.json());
                response.put("rev", revision);
                response.put("serverTime", clock.instant().toString());
                return response;
            }
            catch (ServiceException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw wrap(exception);
            }
        }
    }

    ObjectNode mutate(Connection connection, long accountId, Rule rule) throws Exception
    {
        Snapshot old = read(connection, accountId);
        if (old == null)
        {
            throw new IllegalStateException("Profile does not exist for account " + accountId);
        }
        ProfileDocument profile = new ProfileDocument(old.profile().deepCopy());
        e6.refreshPassive(profile);
        ObjectNode response = mapper.createObjectNode();
        rule.apply(profile, response);
        profile.validate(data);
        long revision = old.rev() + 1;
        update(connection, accountId, profile, revision);
        response.set("profile", profile.json());
        response.put("rev", revision);
        response.put("serverTime", clock.instant().toString());
        return response;
    }

    Rule seasonReward(long reputation, int jade)
    {
        return (profile, result) ->
        {
            addCurrency(profile, "danh_vong", reputation);
            if (jade <= 0)
            {
                return;
            }
            ProfileGrid grid = new ProfileGrid(profile, data);
            if (!grid.addMaterial("chi_ton_ngoc_tuy", jade))
            {
                profile.storage().withArray("overflow").add("material:chi_ton_ngoc_tuy:" + jade);
            }
        };
    }

    public ObjectNode response(Snapshot snapshot)
    {
        ObjectNode response = mapper.createObjectNode();
        response.set("profile", snapshot.profile());
        response.put("rev", snapshot.rev());
        response.put("serverTime", clock.instant().toString());
        return response;
    }

    public ObjectNode arenaLoadout(Snapshot snapshot)
    {
        ObjectNode profile = snapshot.profile();
        ObjectNode loadout = mapper.createObjectNode();
        loadout.put("name", profile.path("heroName").asText());
        loadout.put("cols", 5);
        loadout.put("rows", 4);
        if (profile.path("sectId").isNull())
        {
            loadout.putNull("sect");
        }
        else
        {
            loadout.put("sect", profile.path("sectId").asText());
        }
        loadout.put("tamPhapLevel", profile.path("tamPhapLevel").asInt());
        loadout.put("level", profile.path("heroLevel").asInt());
        ArrayNode values = loadout.putArray("items");
        ProfileDocument document = new ProfileDocument(profile);
        for (JsonNode placement : profile.path("loadout"))
        {
            ObjectNode owned = document.item(placement.path("uid").asText());
            ObjectNode item = values.addObject();
            item.put("id", owned.path("itemId").asText());
            item.put("x", placement.path("x").asInt());
            item.put("y", placement.path("y").asInt());
            item.put("tier", owned.path("tier").asInt());
            item.put("rarity", owned.path("rarity").asText());
        }
        return loadout;
    }

    public int power(Snapshot snapshot)
    {
        List<PowerCalculator.Item> items = new ArrayList<>();
        ProfileDocument profile = new ProfileDocument(snapshot.profile());
        for (JsonNode placement : snapshot.profile().path("loadout"))
        {
            JsonNode item = profile.item(placement.path("uid").asText());
            items.add(new PowerCalculator.Item(item.path("itemId").asText(), item.path("tier").asInt(),
                    item.path("rarity").asText()));
        }
        return new PowerCalculator(data.root("power.json"))
                .calculate(snapshot.profile().path("heroLevel").asInt(), items);
    }

    public ObjectNode applyArenaReward(long accountId, ObjectNode arenaResult)
    {
        JsonNode reward = arenaResult.path("reward");
        ObjectNode profileResult = mutate(accountId, (profile, ignored) ->
        {
            long reputation = reward.path("reputation").asLong();
            if (reputation > 0)
            {
                addCurrency(profile, "danh_vong", reputation);
            }
            ProfileGrid grid = new ProfileGrid(profile, data);
            reward.path("materials").fields().forEachRemaining(entry ->
            {
                if (!grid.addMaterial(entry.getKey(), entry.getValue().asInt()))
                {
                    reject(NO_SPACE);
                }
            });
        });
        arenaResult.set("profile", profileResult.path("profile"));
        arenaResult.set("rev", profileResult.path("rev"));
        arenaResult.set("serverTime", profileResult.path("serverTime"));
        return arenaResult;
    }

    public Rule loadoutSave(JsonNode payload)
    {
        return (profile, result) ->
        {
            ArrayNode requested = requireArray(payload, "placements");
            boolean[][] occupied = new boolean[4][5];
            Map<String, Boolean> used = new HashMap<>();
            for (JsonNode placement : requested)
            {
                String uid = placement.path("uid").asText("");
                ObjectNode owned = profile.item(uid);
                if (owned == null)
                {
                    reject("Không tìm thấy vật phẩm.");
                }
                if (used.put(uid, true) != null)
                {
                    reject("Một vật phẩm không thể xếp hai lần trong Bao Phục.");
                }
                JsonNode size = data.item(owned.path("itemId").asText()).path("size");
                int x = placement.path("x").asInt(-1);
                int y = placement.path("y").asInt(-1);
                int width = size.path(0).asInt();
                int height = size.path(1).asInt();
                if (x < 0 || y < 0 || x + width > 5 || y + height > 4)
                {
                    reject("Vật phẩm không vừa Bao Phục.");
                }
                for (int row = y; row < y + height; row++)
                {
                    for (int col = x; col < x + width; col++)
                    {
                        if (occupied[row][col])
                        {
                            reject("Vật phẩm bị chồng lên nhau trong Bao Phục.");
                        }
                        occupied[row][col] = true;
                    }
                }
            }
            ProfileGrid grid = new ProfileGrid(profile, data);
            ArrayNode previous = profile.array("loadout").deepCopy();
            for (JsonNode placement : requested)
            {
                grid.removeItemPlacement(placement.path("uid").asText());
            }
            profile.json().set("loadout", requested.deepCopy());
            for (JsonNode placement : previous)
            {
                String uid = placement.path("uid").asText();
                if (!used.containsKey(uid) && !grid.placeItem(uid))
                {
                    reject(NO_SPACE);
                }
            }
        };
    }

    public Rule storageMove(JsonNode payload)
    {
        return (profile, result) ->
        {
            ProfileGrid grid = new ProfileGrid(profile, data);
            boolean moved;
            if (payload.hasNonNull("uid"))
            {
                moved = grid.moveItem(payload.path("uid").asText(), payload.path("toX").asInt(-1),
                        payload.path("toY").asInt(-1));
            }
            else
            {
                moved = grid.moveStack(payload.path("materialId").asText(), payload.path("fromX").asInt(-1),
                        payload.path("fromY").asInt(-1), payload.path("toX").asInt(-1),
                        payload.path("toY").asInt(-1));
            }
            if (!moved)
            {
                reject("Không thể đặt vào ô này.");
            }
        };
    }

    public Rule storageRepack()
    {
        return (profile, result) -> new ProfileGrid(profile, data).repack();
    }

    public Rule shopRefresh(boolean paid)
    {
        return e6.shopRefresh(paid);
    }

    public Rule shopBuy(int slot)
    {
        return e6.shopBuy(slot);
    }

    public Rule shopExchange(String inputId, String targetId)
    {
        return e6.exchange(inputId, targetId);
    }

    public Rule shopBuyReputation(String offerId)
    {
        return e6.buyReputation(offerId);
    }

    public Rule pveBattle(String stageId)
    {
        return e6.pveBattle(stageId);
    }

    public Rule questClaim(String questId)
    {
        return e6.questClaim(questId);
    }

    public Rule sectChoose(String sectId)
    {
        return e6.sectChoose(sectId);
    }

    public Rule onboarding(String action, JsonNode id)
    {
        return e6.onboarding(action, id);
    }

    public Rule upgrade(String uid)
    {
        return (profile, result) ->
        {
            ObjectNode item = profile.item(uid);
            if (item == null)
            {
                reject("Không tìm thấy vật phẩm.");
            }
            int tier = item.path("tier").asInt();
            if (tier >= 9)
            {
                reject("Vật phẩm đã đạt Cửu Trọng.");
            }
            JsonNode step = upgradeStep(item.path("rarity").asText(), tier);
            Map<String, Integer> costs = upgradeMaterials(item, step);
            requireSilver(profile, step.path("silver").asLong());
            requireMaterials(profile, costs);
            profile.currencies().put("bac", profile.currencies().path("bac").asLong() - step.path("silver").asLong());
            new ProfileGrid(profile, data).spend(costs);
            item.put("tier", tier + 1);
            record(profile, "item_upgrade");
            result.put("tier", tier + 1);
        };
    }

    public Rule merge(String firstUid, String secondUid)
    {
        return (profile, result) ->
        {
            ObjectNode first = profile.item(firstUid);
            ObjectNode second = profile.item(secondUid);
            if (first == null || second == null)
            {
                reject("Không tìm thấy bản sao để Hợp Thành.");
            }
            if (firstUid.equals(secondUid))
            {
                reject("Phải chọn hai bản sao khác nhau.");
            }
            if (!first.path("itemId").asText().equals(second.path("itemId").asText()))
            {
                reject("Hai bản sao phải là cùng một món.");
            }
            if (!first.path("rarity").asText().equals(second.path("rarity").asText()))
            {
                reject("Hai bản sao phải cùng phẩm.");
            }
            if ("supreme".equals(first.path("rarity").asText()))
            {
                reject("Phẩm Chí Tôn không thể Hợp Thành cao hơn.");
            }
            JsonNode step = mergeStep(first.path("rarity").asText());
            Map<String, Integer> extras = fields(step.path("extraMaterials"));
            requireSilver(profile, step.path("silver").asLong());
            requireMaterials(profile, extras);
            ProfileGrid grid = new ProfileGrid(profile, data);
            Map<String, Integer> refund = invested(second);
            refund.replaceAll((key, value) -> value / 2);
            refund.entrySet().removeIf(entry -> entry.getValue() == 0);
            boolean firstEquipped = contains(profile.array("loadout"), firstUid);
            boolean secondEquipped = contains(profile.array("loadout"), secondUid);
            String kept = firstEquipped ? firstUid : secondEquipped ? secondUid : null;
            String outputUid = kept == null ? UUID.randomUUID().toString().replace("-", "") : kept;
            grid.removeItemPlacement(firstUid);
            grid.removeItemPlacement(secondUid);
            removeItem(profile, kept == null ? firstUid : kept.equals(firstUid) ? secondUid : firstUid);
            if (kept == null)
            {
                removeItem(profile, secondUid);
                ObjectNode output = profile.array("items").addObject();
                output.put("uid", outputUid);
                output.put("itemId", first.path("itemId").asText());
                output.put("tier", first.path("tier").asInt());
                output.put("rarity", step.path("toRarity").asText());
                output.put("acquiredAt", clock.instant().toString());
                if (!grid.placeItem(outputUid))
                {
                    reject(NO_SPACE);
                }
            }
            else
            {
                ObjectNode output = profile.item(kept);
                output.put("tier", first.path("tier").asInt());
                output.put("rarity", step.path("toRarity").asText());
            }
            profile.currencies().put("bac", profile.currencies().path("bac").asLong() - step.path("silver").asLong());
            grid.spend(extras);
            for (Map.Entry<String, Integer> entry : refund.entrySet())
            {
                if (!grid.addMaterial(entry.getKey(), entry.getValue()))
                {
                    reject(NO_SPACE);
                }
            }
            record(profile, "item_merge");
            result.put("resultUid", outputUid);
        };
    }

    public Rule sell(String uid)
    {
        return (profile, result) ->
        {
            ObjectNode item = profile.item(uid);
            if (item == null)
            {
                reject("Không tìm thấy vật phẩm để bán.");
            }
            long investedSilver = 0;
            Map<String, Integer> refund = new HashMap<>();
            for (int tier = 1; tier < item.path("tier").asInt(); tier++)
            {
                JsonNode step = upgradeStep(item.path("rarity").asText(), tier);
                investedSilver += step.path("silver").asLong();
                Map<String, Integer> one = upgradeMaterials(item, step);
                one.forEach((key, value) -> refund.merge(key, value / 2, Integer::sum));
            }
            int rarity = rarityIndex(item.path("rarity").asText());
            long silver = BASE_PRICES[rarity] * 30 / 100 + investedSilver * 15 / 100;
            ProfileGrid grid = new ProfileGrid(profile, data);
            grid.removeItemPlacement(uid);
            removeItem(profile, uid);
            profile.currencies().put("bac", Math.addExact(profile.currencies().path("bac").asLong(), silver));
            for (Map.Entry<String, Integer> entry : refund.entrySet())
            {
                if (entry.getValue() > 0 && !grid.addMaterial(entry.getKey(), entry.getValue()))
                {
                    reject(NO_SPACE);
                }
            }
            result.put("silver", silver);
        };
    }

    public Rule cheat(JsonNode payload, String heroName)
    {
        if (!dev)
        {
            throw new ServiceException("forbidden", "Dev cheat is disabled");
        }
        return (profile, result) ->
        {
            String action = payload.path("action").asText();
            ProfileGrid grid = new ProfileGrid(profile, data);
            switch (action)
            {
                case "add_item" ->
                {
                    String itemId = payload.path("itemId").asText();
                    data.item(itemId);
                    String uid = UUID.randomUUID().toString().replace("-", "");
                    ObjectNode item = profile.array("items").addObject();
                    item.put("uid", uid);
                    item.put("itemId", itemId);
                    int tier = payload.path("tier").asInt(1);
                    if (tier < 1 || tier > 9)
                    {
                        throw new IllegalArgumentException("tier must be from 1 to 9");
                    }
                    item.put("tier", tier);
                    item.put("rarity", payload.path("rarity").asText("common"));
                    item.put("acquiredAt", clock.instant().toString());
                    if (!grid.placeItem(uid))
                    {
                        reject(NO_SPACE);
                    }
                }
                case "add_material" ->
                {
                    String materialId = payload.path("materialId").asText();
                    if (!grid.addMaterial(materialId, payload.path("amount").asInt()))
                    {
                        reject(NO_SPACE);
                    }
                }
                case "add_currency" -> addCurrency(profile, payload.path("currencyId").asText(),
                        payload.path("amount").asLong());
                case "add_silver" -> addCurrency(profile, "bac", payload.path("amount").asLong(10000));
                case "add_reputation" -> addCurrency(profile, "danh_vong", payload.path("amount").asLong(1000));
                case "refresh_shop" -> e6.forceShopRefresh(profile);
                case "unlock_pve" -> e6.unlockPve(profile);
                case "refill_stamina" -> e6.refillStamina(profile);
                case "clear_stage" -> e6.clearStage(profile, payload.path("stageId").asText("hoi_1_01"));
                case "next_week", "advance_days" -> e6.nextWeek(profile);
                case "add_level" -> grantExp(profile, expToNext(profile.json().path("heroLevel").asInt()));
                case "add_exp" -> grantExp(profile, payload.path("amount").asLong(100));
                case "reset_sect" ->
                {
                    profile.json().putNull("sectId");
                    profile.json().put("tamPhapLevel", 1);
                    profile.json().put("sectPromptShown", false);
                }
                case "reset_profile" -> profile.json().setAll(create(heroName).json());
                default -> throw new IllegalArgumentException("Unknown dev_cheat action: " + action);
            }
        };
    }

    public ProfileDocument create(String heroName)
    {
        ObjectNode profile = mapper.createObjectNode();
        profile.put("version", 9);
        profile.put("heroName", heroName == null ? "" : heroName);
        profile.put("heroLevel", 1);
        profile.put("heroExp", 0);
        profile.putNull("sectId");
        profile.put("tamPhapLevel", 1);
        profile.put("sectPromptShown", false);
        profile.put("introSeen", false);
        profile.putArray("completedTutorials");
        profile.putArray("seenStoryChapters");
        profile.putArray("pendingStoryChapters");
        profile.set("currencies", data.root("starter.json").path("currencies").deepCopy());
        profile.putObject("materials");
        ArrayNode owned = profile.putArray("items");
        Map<String, String> keys = new HashMap<>();
        for (JsonNode value : data.root("starter.json").path("items"))
        {
            String uid = "starter_" + value.path("key").asText();
            keys.put(value.path("key").asText(), uid);
            ObjectNode item = owned.addObject();
            item.put("uid", uid);
            item.put("itemId", value.path("itemId").asText());
            item.put("tier", value.path("tier").asInt(1));
            item.put("rarity", value.path("rarity").asText("common"));
            item.put("acquiredAt", "starter");
        }
        ArrayNode loadout = profile.putArray("loadout");
        for (JsonNode value : data.root("starter.json").path("loadout"))
        {
            ObjectNode placement = loadout.addObject();
            placement.put("uid", keys.get(value.path("itemKey").asText()));
            placement.put("x", value.path("x").asInt());
            placement.put("y", value.path("y").asInt());
        }
        profile.putArray("firstClears");
        ObjectNode shop = profile.putObject("shop");
        shop.putArray("slots");
        shop.put("lastFreeRefreshUtc", "");
        shop.put("paidRefreshesToday", 0);
        shop.put("starterFreeRefreshesUsed", 0);
        shop.put("dayKey", "");
        shop.put("weekKey", "");
        shop.putObject("reputationPurchases");
        shop.put("generation", 0);
        ObjectNode storage = profile.putObject("storage");
        storage.put("cols", 10);
        storage.put("rows", 20);
        storage.putArray("items");
        storage.putArray("materialStacks");
        storage.putArray("overflow");
        ObjectNode pve = profile.putObject("pve");
        pve.putObject("stages");
        pve.put("stamina", -1);
        pve.put("lastStaminaRegenUtc", "");
        pve.putArray("dailyFirstClears");
        pve.put("dailyFirstClearDate", "");
        ObjectNode quest = profile.putObject("quest");
        quest.put("activeIndex", 0);
        quest.put("activeId", "");
        quest.putArray("claimedIds");
        quest.putObject("actionCounters");
        quest.putObject("actionBaseline");
        ProfileDocument result = new ProfileDocument(profile);
        ProfileGrid grid = new ProfileGrid(result, data);
        for (JsonNode item : owned)
        {
            if (!contains(loadout, item.path("uid").asText()))
            {
                grid.placeItem(item.path("uid").asText());
            }
        }
        result.validate(data);
        return result;
    }

    private Snapshot read(Connection connection, long accountId) throws Exception
    {
        String sql = "SELECT json, rev FROM profiles WHERE account_id = ?";
        try (PreparedStatement query = connection.prepareStatement(sql))
        {
            query.setLong(1, accountId);
            try (ResultSet row = query.executeQuery())
            {
                if (!row.next())
                {
                    return null;
                }
                ObjectNode value = (ObjectNode) mapper.readTree(row.getString("json"));
                new ProfileDocument(value).validate(data);
                return new Snapshot(value, row.getLong("rev"));
            }
        }
    }

    private Snapshot refreshOnRead(Connection connection, long accountId, Snapshot snapshot) throws Exception
    {
        ProfileDocument profile = new ProfileDocument(snapshot.profile().deepCopy());
        e6.refreshPassive(profile);
        if (profile.json().equals(snapshot.profile()))
        {
            return snapshot;
        }
        long revision = snapshot.rev() + 1;
        update(connection, accountId, profile, revision);
        return new Snapshot(profile.json(), revision);
    }

    private void insert(Connection connection, long accountId, ProfileDocument profile) throws Exception
    {
        try (PreparedStatement insert = connection.prepareStatement(
                "INSERT INTO profiles(account_id, json, rev, updated_at) VALUES(?, ?, 0, ?)"))
        {
            insert.setLong(1, accountId);
            insert.setString(2, mapper.writeValueAsString(profile.json()));
            insert.setString(3, clock.instant().toString());
            insert.executeUpdate();
        }
    }

    private void update(Connection connection, long accountId, ProfileDocument profile, long revision) throws Exception
    {
        try (PreparedStatement update = connection.prepareStatement(
                "UPDATE profiles SET json = ?, rev = ?, updated_at = ? WHERE account_id = ?"))
        {
            update.setString(1, mapper.writeValueAsString(profile.json()));
            update.setLong(2, revision);
            update.setString(3, clock.instant().toString());
            update.setLong(4, accountId);
            update.executeUpdate();
        }
    }

    private Map<String, Integer> upgradeMaterials(ObjectNode item, JsonNode step)
    {
        String type = data.item(item.path("itemId").asText()).path("type").asText();
        String group = "pill".equals(type) || "relic".equals(type) ? "pill_relic" : type;
        Map<String, Integer> result = new HashMap<>();
        result.put(findMaterial(group, step.path("materialTier").asInt(), false), step.path("materialCount").asInt());
        if (step.path("catalystCount").asInt() > 0)
        {
            result.put(findMaterial(group, 0, true), step.path("catalystCount").asInt());
        }
        return result;
    }

    private Map<String, Integer> invested(ObjectNode item)
    {
        Map<String, Integer> result = new HashMap<>();
        for (int tier = 1; tier < item.path("tier").asInt(); tier++)
        {
            upgradeMaterials(item, upgradeStep(item.path("rarity").asText(), tier))
                    .forEach((key, value) -> result.merge(key, value, Integer::sum));
        }
        return result;
    }

    private JsonNode upgradeStep(String rarity, int tier)
    {
        for (JsonNode step : data.root("upgrade_costs.json").path("steps").path(rarity))
        {
            if (step.path("fromTier").asInt() == tier)
            {
                return step;
            }
        }
        throw new IllegalStateException("Missing upgrade step for " + rarity + " tier " + tier);
    }

    private JsonNode mergeStep(String rarity)
    {
        for (JsonNode step : data.root("merge.json").path("steps"))
        {
            if (rarity.equals(step.path("fromRarity").asText()))
            {
                return step;
            }
        }
        throw new IllegalStateException("Missing merge step for " + rarity);
    }

    private String findMaterial(String group, int tier, boolean catalyst)
    {
        for (JsonNode value : data.root("materials.json"))
        {
            if (group.equals(value.path("itemType").asText()) && tier == value.path("tier").asInt()
                    && catalyst == value.path("isCatalyst").asBoolean())
            {
                return value.path("id").asText();
            }
        }
        throw new IllegalStateException("Missing upgrade material for " + group);
    }

    private void requireMaterials(ProfileDocument profile, Map<String, Integer> costs)
    {
        for (Map.Entry<String, Integer> cost : costs.entrySet())
        {
            int owned = profile.materials().path(cost.getKey()).asInt();
            if (owned < cost.getValue())
            {
                reject("Không đủ " + data.material(cost.getKey()).path("name").asText() + " (" + owned + "/"
                        + cost.getValue() + ").");
            }
        }
    }

    private static void requireSilver(ProfileDocument profile, long cost)
    {
        long silver = profile.currencies().path("bac").asLong();
        if (silver < cost)
        {
            reject("Không đủ Bạc (" + silver + "/" + cost + ").");
        }
    }

    private void grantExp(ProfileDocument profile, long amount)
    {
        int level = profile.json().path("heroLevel").asInt();
        int max = data.root("levels.json").path("maxLevel").asInt();
        long experience = Math.addExact(profile.json().path("heroExp").asLong(), Math.max(0, amount));
        while (level < max && experience >= expToNext(level))
        {
            experience -= expToNext(level);
            level++;
        }
        profile.json().put("heroLevel", level);
        profile.json().put("heroExp", level == max ? 0 : experience);
    }

    private long expToNext(int level)
    {
        int max = data.root("levels.json").path("maxLevel").asInt();
        return level >= max ? 0 : data.root("levels.json").path("expToNext").path(level - 1).asLong();
    }

    private static void addCurrency(ProfileDocument profile, String id, long amount)
    {
        if (id.isBlank() || amount <= 0)
        {
            throw new IllegalArgumentException("Currency id and amount are required");
        }
        profile.currencies().put(id, Math.addExact(profile.currencies().path(id).asLong(), amount));
    }

    private static void record(ProfileDocument profile, String action)
    {
        ObjectNode counters = (ObjectNode) profile.json().path("quest").path("actionCounters");
        counters.put(action, Math.addExact(counters.path(action).asInt(), 1));
    }

    private static void removeItem(ProfileDocument profile, String uid)
    {
        remove(profile.array("items"), uid);
        remove(profile.array("loadout"), uid);
        remove(profile.storage().withArray("items"), uid);
    }

    private static void remove(ArrayNode values, String uid)
    {
        for (int i = values.size() - 1; i >= 0; i--)
        {
            if (uid.equals(values.get(i).path("uid").asText()))
            {
                values.remove(i);
            }
        }
    }

    private static boolean contains(ArrayNode values, String uid)
    {
        for (JsonNode value : values)
        {
            if (uid.equals(value.path("uid").asText()))
            {
                return true;
            }
        }
        return false;
    }

    private static Map<String, Integer> fields(JsonNode source)
    {
        Map<String, Integer> result = new HashMap<>();
        source.fields().forEachRemaining(entry -> result.put(entry.getKey(), entry.getValue().asInt()));
        return result;
    }

    private static int rarityIndex(String rarity)
    {
        for (int i = 0; i < RARITIES.length; i++)
        {
            if (RARITIES[i].equals(rarity))
            {
                return i;
            }
        }
        throw new IllegalStateException("Unknown rarity " + rarity);
    }

    private static ArrayNode requireArray(JsonNode payload, String name)
    {
        if (!payload.path(name).isArray())
        {
            throw new IllegalArgumentException(name + " must be an array");
        }
        return (ArrayNode) payload.path(name);
    }

    private Object lock(long accountId)
    {
        return locks.computeIfAbsent(accountId, ignored -> new Object());
    }

    private static void reject(String message)
    {
        throw new ServiceException("rejected", message);
    }

    private static RuntimeException wrap(Exception exception)
    {
        if (exception instanceof RuntimeException runtime)
        {
            return runtime;
        }
        return new IllegalStateException(exception);
    }

    /** Keeps the Jackson configuration in one place without exposing battle-engine internals in tests. */
    private static final class ItemCatalogAccess
    {
        private static ObjectMapper mapper()
        {
            return com.onebagjianghu.battle.data.ItemCatalog.mapper();
        }
    }
}
