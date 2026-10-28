package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;

import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.time.LocalDate;
import java.time.temporal.TemporalAdjusters;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Random;
import java.util.UUID;

/** Rules moved from the Unity economy, progression and onboarding services. */
final class E6Rules
{
    private static final String NO_SPACE = "Tàng Bảo Các không đủ chỗ.";
    private final GameData data;
    private final Clock clock;
    private final ObjectMapper mapper;
    private final Random random;
    private final BattleSimulator simulator;

    E6Rules(GameData data, Clock clock, ObjectMapper mapper)
    {
        this.data = data;
        this.clock = clock;
        this.mapper = mapper;
        random = new Random();
        simulator = new BattleSimulator(data.itemCatalog(), data.sectCatalog(), BattleConfig.defaults());
    }

    void refreshPassive(ProfileDocument profile)
    {
        refreshStamina(profile, clock.instant(), false);
        resetShop(profile, clock.instant());
        ensureShop(profile);
        activateQuest(profile);
    }

    // Same as ShopService.Ensure: a profile always shows a full shop, so the client never rolls one itself.
    private void ensureShop(ProfileDocument profile)
    {
        ObjectNode shop = (ObjectNode) profile.json().path("shop");
        if (shop.path("slots").size() == data.root("shop.json").path("slots").size())
        {
            return;
        }
        generateShop(profile);
        if (shop.path("lastFreeRefreshUtc").asText().isEmpty())
        {
            shop.put("lastFreeRefreshUtc", clock.instant().toString());
        }
    }

    ProfileService.Rule shopRefresh(boolean paid)
    {
        return (profile, result) ->
        {
            resetShop(profile, clock.instant());
            ObjectNode shop = (ObjectNode) profile.json().path("shop");
            JsonNode config = data.root("shop.json");
            if (paid)
            {
                long cost = config.path("paidRefreshCost").asLong();
                requireCurrency(profile, "bac", cost, "Bạc");
                profile.currencies().put("bac", profile.currencies().path("bac").asLong() - cost);
                shop.put("paidRefreshesToday", shop.path("paidRefreshesToday").asInt() + 1);
            }
            else
            {
                int used = shop.path("starterFreeRefreshesUsed").asInt();
                int starter = config.path("starterFreeRefreshes").asInt();
                if (used < starter)
                {
                    shop.put("starterFreeRefreshesUsed", used + 1);
                }
                else
                {
                    Instant last = instant(shop.path("lastFreeRefreshUtc").asText());
                    long hours = config.path("freeRefreshHours").asLong();
                    if (last != null && clock.instant().isBefore(last.plus(Duration.ofHours(hours))))
                    {
                        reject("Chưa đến lượt làm mới miễn phí.");
                    }
                }
                shop.put("lastFreeRefreshUtc", clock.instant().toString());
            }
            generateShop(profile);
        };
    }

    ProfileService.Rule shopBuy(int index)
    {
        return (profile, result) ->
        {
            ArrayNode slots = profile.json().with("shop").withArray("slots");
            if (index < 0 || index >= slots.size())
            {
                reject("Ô hàng không hợp lệ.");
            }
            ObjectNode slot = (ObjectNode) slots.get(index);
            if (slot.path("bought").asBoolean())
            {
                reject("Ô hàng này đã mua.");
            }
            long price = slot.path("price").asLong();
            requireCurrency(profile, "bac", price, "Bạc");
            ProfileGrid grid = new ProfileGrid(profile, data);
            if ("item".equals(slot.path("kind").asText()))
            {
                String uid = addItem(profile, slot.path("contentId").asText(), 1,
                        slot.path("rarity").asText("common"));
                if (!grid.placeItem(uid))
                {
                    removeItem(profile, uid);
                    reject(NO_SPACE);
                }
                result.put("uid", uid);
            }
            else if (!grid.addMaterial(slot.path("contentId").asText(), slot.path("quantity").asInt()))
            {
                reject(NO_SPACE);
            }
            profile.currencies().put("bac", profile.currencies().path("bac").asLong() - price);
            slot.put("bought", true);
            record(profile, "shop_buy");
        };
    }

    ProfileService.Rule exchange(String inputId, String targetId)
    {
        return (profile, result) ->
        {
            JsonNode input = data.material(inputId);
            JsonNode target = data.material(targetId);
            if (input == null || target == null)
            {
                reject("Nguyên liệu không hợp lệ.");
            }
            if (forbiddenMaterial(input) || forbiddenMaterial(target))
            {
                reject("Không thể đổi chất dẫn hoặc Chí Tôn Ngọc Tủy.");
            }
            boolean same = input.path("itemType").asText().equals(target.path("itemType").asText())
                    && target.path("tier").asInt() == input.path("tier").asInt() + 1;
            boolean cross = !input.path("itemType").asText().equals(target.path("itemType").asText())
                    && target.path("tier").asInt() == input.path("tier").asInt();
            if (!same && !cross)
            {
                reject("Chỉ đổi lên một bậc cùng nhóm hoặc ngang bậc khác nhóm.");
            }
            JsonNode config = data.root("shop.json").path("exchange");
            int count = config.path(same ? "sameInput" : "crossInput").asInt();
            int tier = input.path("tier").asInt();
            long fee = config.path(same ? "sameFees" : "crossFees").path(tier - 1).asLong();
            int owned = profile.materials().path(inputId).asInt();
            if (owned < count)
            {
                reject("Không đủ " + input.path("name").asText() + " (" + owned + "/" + count + ").");
            }
            requireCurrency(profile, "bac", fee, "Bạc");
            ProfileGrid grid = new ProfileGrid(profile, data);
            grid.spend(java.util.Map.of(inputId, count));
            if (!grid.addMaterial(targetId, 1))
            {
                reject(NO_SPACE);
            }
            profile.currencies().put("bac", profile.currencies().path("bac").asLong() - fee);
        };
    }

    ProfileService.Rule buyReputation(String offerId)
    {
        return (profile, result) ->
        {
            resetShop(profile, clock.instant());
            JsonNode offer = find(data.root("shop.json").path("reputationOffers"), offerId);
            if (offer == null)
            {
                reject("Món Danh Vọng không hợp lệ.");
            }
            resetReputationOffer(profile, offer, clock.instant());
            ObjectNode purchases = (ObjectNode) profile.json().path("shop").path("reputationPurchases");
            if (purchases.path(offerId).asInt() >= offer.path("limit").asInt())
            {
                reject("Đã hết lượt mua trong tuần.");
            }
            requireCurrency(profile, "danh_vong", offer.path("price").asLong(), "Danh Vọng");
            List<JsonNode> choices = materialsForOffer(offerId);
            ProfileGrid grid = new ProfileGrid(profile, data);
            int quantity = offer.path("quantity").asInt();
            for (int i = 0; i < quantity; i++)
            {
                JsonNode reward = choices.get(random.nextInt(choices.size()));
                if (!grid.addMaterial(reward.path("id").asText(), 1))
                {
                    reject(NO_SPACE);
                }
            }
            long price = offer.path("price").asLong();
            profile.currencies().put("danh_vong", profile.currencies().path("danh_vong").asLong() - price);
            purchases.put(offerId, purchases.path(offerId).asInt() + 1);
        };
    }

    ProfileService.Rule sectChoose(String sectId)
    {
        return (profile, result) ->
        {
            if (profile.json().path("heroLevel").asInt() < 10)
            {
                reject("Đạt cấp 10 để gia nhập môn phái");
            }
            if (!profile.json().path("sectId").isNull() && !profile.json().path("sectId").asText().isBlank())
            {
                reject("Môn phái đã chọn không thể thay đổi.");
            }
            if (find(data.root("sects.json"), sectId) == null)
            {
                reject("Môn phái không hợp lệ.");
            }
            profile.json().put("sectId", sectId);
            profile.json().put("tamPhapLevel", 1);
        };
    }

    ProfileService.Rule onboarding(String action, JsonNode id)
    {
        return (profile, result) ->
        {
            switch (action)
            {
                case "intro_seen" -> profile.json().put("introSeen", true);
                case "sect_prompt_shown" -> profile.json().put("sectPromptShown", true);
                case "tutorial_done" -> addUnique(profile.array("completedTutorials"), id.asText());
                case "story_seen" ->
                {
                    int chapter = id.asInt();
                    removeValue(profile.array("pendingStoryChapters"), chapter);
                    addUnique(profile.array("seenStoryChapters"), chapter);
                }
                default -> throw new IllegalArgumentException("Unknown onboarding action: " + action);
            }
        };
    }

    ProfileService.Rule questClaim(String questId)
    {
        return (profile, result) ->
        {
            activateQuest(profile);
            ObjectNode questState = (ObjectNode) profile.json().path("quest");
            JsonNode quest = activeQuest(profile);
            if (quest == null || !quest.path("id").asText().equals(questId) || !questDone(profile, quest))
            {
                reject("Nhiệm vụ chưa hoàn thành.");
            }
            JsonNode reward = quest.path("reward");
            ProfileGrid grid = new ProfileGrid(profile, data);
            var fields = reward.path("materials").fields();
            while (fields.hasNext())
            {
                var entry = fields.next();
                if (!grid.addMaterial(entry.getKey(), entry.getValue().asInt()))
                {
                    reject(NO_SPACE);
                }
            }
            addCurrency(profile, "bac", reward.path("silver").asLong());
            grantExp(profile, reward.path("heroExp").asLong());
            addUnique((ArrayNode) questState.path("claimedIds"), questId);
            questState.put("activeIndex", questState.path("activeIndex").asInt() + 1);
            setQuestBaseline(profile);
            result.set("reward", reward.deepCopy());
        };
    }

    ProfileService.Rule pveBattle(String stageId)
    {
        return (profile, response) ->
        {
            JsonNode stage = stage(stageId);
            if (stage == null || !stageUnlocked(profile, stageId))
            {
                reject("Ải chưa mở.");
            }
            if (!spendStamina(profile, stage.path("staminaCost").asInt(5)))
            {
                reject("Không đủ Thể Lực.");
            }
            long seed = random.nextLong();
            BattleResult battle = simulator.run(playerLoadout(profile), enemyLoadout(stage.path("enemy")), seed);
            boolean won = battle.winner() == 0;
            boolean firstClear = won && !isCleared(profile, stageId);
            ObjectNode reward = mapper.createObjectNode();
            reward.put("silver", 0);
            reward.put("heroExp", 0);
            reward.putObject("materials");
            reward.putArray("items");
            if (won)
            {
                reward = reward(stage, firstClear, seed);
                applyPveReward(profile, stage, reward, firstClear);
            }
            response.put("stageId", stageId);
            response.put("won", won);
            response.put("firstClear", firstClear);
            response.put("seed", seed);
            response.set("result", mapper.valueToTree(battle));
            response.set("reward", reward);
            JsonNode enemy = stage.path("enemy");
            ObjectNode opponent = response.putObject("enemy");
            opponent.put("name", enemy.path("name").asText());
            opponent.set("sect", enemy.path("sect"));
            opponent.put("level", enemy.path("level").asInt());
            opponent.put("portrait", enemy.path("portrait").asText());
        };
    }

    void forceShopRefresh(ProfileDocument profile)
    {
        generateShop(profile);
    }

    void unlockPve(ProfileDocument profile)
    {
        ObjectNode stages = (ObjectNode) profile.json().path("pve").path("stages");
        for (JsonNode chapter : data.root("Pve/chapters.json").path("chapters"))
        {
            for (JsonNode stage : chapter.path("stages"))
            {
                stages.with(stage.path("id").asText()).put("cleared", true);
            }
        }
    }

    void refillStamina(ProfileDocument profile)
    {
        ObjectNode pve = (ObjectNode) profile.json().path("pve");
        pve.put("stamina", data.root("Pve/stamina.json").path("max").asInt());
        pve.put("lastStaminaRegenUtc", clock.instant().toString());
    }

    void clearStage(ProfileDocument profile, String stageId)
    {
        JsonNode value = stage(stageId);
        if (value == null)
        {
            throw new IllegalArgumentException("Unknown stage: " + stageId);
        }
        profile.json().with("pve").with("stages").with(stageId).put("cleared", true);
    }

    void nextWeek(ProfileDocument profile)
    {
        ObjectNode shop = (ObjectNode) profile.json().path("shop");
        shop.put("dayKey", "");
        shop.put("weekKey", "");
        resetShop(profile, clock.instant());
    }

    private void generateShop(ProfileDocument profile)
    {
        ObjectNode shop = (ObjectNode) profile.json().path("shop");
        int generation = shop.path("generation").asInt();
        shop.put("generation", generation + 1);
        ArrayNode slots = shop.putArray("slots");
        JsonNode configs = data.root("shop.json").path("slots");
        for (int i = 0; i < configs.size(); i++)
        {
            JsonNode config = configs.get(i);
            if (generation == 0 && i == 0)
            {
                addShopItem(slots, "thai_thanh_tam_kinh", "common", 0);
            }
            else
            {
                createSlot(slots, config);
            }
        }
    }

    private void createSlot(ArrayNode slots, JsonNode config)
    {
        String type = config.path("type").asText();
        if ("item".equals(type) || "discount".equals(type) && random.nextInt(100) >= 30)
        {
            String rarity = weighted(config.path("rarities"), config.path("weights"));
            List<JsonNode> choices = new ArrayList<>();
            for (JsonNode item : data.root("items.json"))
            {
                if (!monsterItem(item.path("id").asText()))
                {
                    choices.add(item);
                }
            }
            JsonNode item = choices.get(random.nextInt(choices.size()));
            int discount = config.path("discountPercent").asInt();
            long base = data.root("shop.json").path("itemBasePrices").path(rarity).asLong();
            double multiplier = data.root("shop.json").path("itemTypePriceMultipliers")
                    .path(item.path("type").asText()).asDouble(1);
            long price = RuleMath.roundHalfEven(base * multiplier) * (100 - discount) / 100;
            addShopItem(slots, item.path("id").asText(), rarity, price);
            return;
        }
        JsonNode material;
        int quantity = config.path("quantity").asInt();
        long price = config.path("price").asLong();
        if ("catalyst_or_tier3".equals(type) && random.nextInt(100) < 40)
        {
            material = randomMaterial(0, true, null);
            quantity = 1;
            price = 1200;
        }
        else
        {
            int tier = config.path("tier").asInt();
            String group = null;
            if ("catalyst_or_tier3".equals(type))
            {
                tier = 3;
                quantity = 2;
                price = 800;
            }
            if ("discount".equals(type))
            {
                // ShopService.CreateSlot: a discount slot that rolls a material sells five tier-1 pills.
                tier = 1;
                group = "pill_relic";
                quantity = 5;
                price = 250 * (100 - config.path("discountPercent").asInt()) / 100;
            }
            material = randomMaterial(tier, false, group);
        }
        ObjectNode slot = slots.addObject();
        slot.put("kind", "material");
        slot.put("contentId", material.path("id").asText());
        slot.put("quantity", quantity);
        slot.put("price", price);
        slot.put("bought", false);
    }

    private void addShopItem(ArrayNode slots, String id, String rarity, long price)
    {
        ObjectNode slot = slots.addObject();
        slot.put("kind", "item");
        slot.put("contentId", id);
        slot.put("rarity", rarity);
        slot.put("quantity", 1);
        slot.put("price", price);
        slot.put("bought", false);
    }

    private void resetShop(ProfileDocument profile, Instant now)
    {
        ObjectNode shop = (ObjectNode) profile.json().path("shop");
        LocalDate date = now.atZone(ArenaCalendar.ZONE).toLocalDate();
        String day = date.toString();
        if (!day.equals(shop.path("dayKey").asText()))
        {
            shop.put("dayKey", day);
            shop.put("paidRefreshesToday", 0);
        }
        String week = date.with(TemporalAdjusters.previousOrSame(java.time.DayOfWeek.MONDAY)).toString();
        if (!week.equals(shop.path("weekKey").asText()))
        {
            shop.put("weekKey", week);
        }
        for (JsonNode offer : data.root("shop.json").path("reputationOffers"))
        {
            resetReputationOffer(profile, offer, now);
        }
    }

    private void resetReputationOffer(ProfileDocument profile, JsonNode offer, Instant now)
    {
        ObjectNode shop = (ObjectNode) profile.json().path("shop");
        ObjectNode periods = shop.with("reputationPurchasePeriods");
        ObjectNode purchases = shop.with("reputationPurchases");
        String offerId = offer.path("id").asText();
        int periodWeeks = Math.max(1, offer.path("periodWeeks").asInt(1));
        LocalDate monday = LocalDate.ofInstant(now, ArenaCalendar.ZONE)
                .with(TemporalAdjusters.previousOrSame(java.time.DayOfWeek.MONDAY));
        long weekNumber = ChronoUnit.WEEKS.between(LocalDate.of(1970, 1, 5), monday);
        String period = Long.toString(Math.floorDiv(weekNumber, periodWeeks));
        if (!period.equals(periods.path(offerId).asText()))
        {
            periods.put(offerId, period);
            purchases.put(offerId, 0);
        }
    }

    // StaminaService.Refresh moves the regen clock to now while stamina is full. Only spending needs that, so a
    // plain read leaves it alone and does not rewrite the profile on every request.
    private void refreshStamina(ProfileDocument profile, Instant now, boolean restartClockWhenFull)
    {
        ObjectNode pve = (ObjectNode) profile.json().path("pve");
        int max = data.root("Pve/stamina.json").path("max").asInt(60);
        int stamina = pve.path("stamina").asInt(-1);
        if (stamina < 0)
        {
            pve.put("stamina", max);
            pve.put("lastStaminaRegenUtc", now.toString());
            return;
        }
        if (stamina >= max)
        {
            pve.put("stamina", max);
            if (restartClockWhenFull)
            {
                pve.put("lastStaminaRegenUtc", now.toString());
            }
            return;
        }
        Instant last = instant(pve.path("lastStaminaRegenUtc").asText());
        if (last == null)
        {
            last = now;
            pve.put("lastStaminaRegenUtc", now.toString());
        }
        double rate = data.root("Pve/stamina.json").path("regenPerMinute").asDouble();
        double perPointSeconds = 60d / rate;
        int gained = (int) (Duration.between(last, now).toMillis() / 1000d / perPointSeconds);
        if (gained > 0)
        {
            pve.put("stamina", Math.min(max, stamina + gained));
            pve.put("lastStaminaRegenUtc", last.plusMillis((long) (gained * perPointSeconds * 1000)).toString());
        }
    }

    private boolean spendStamina(ProfileDocument profile, int amount)
    {
        refreshStamina(profile, clock.instant(), true);
        ObjectNode pve = (ObjectNode) profile.json().path("pve");
        if (pve.path("stamina").asInt() < amount)
        {
            return false;
        }
        pve.put("stamina", pve.path("stamina").asInt() - amount);
        return true;
    }

    private ObjectNode reward(JsonNode stage, boolean first, long seed)
    {
        JsonNode base = stage.path("rewards").path("firstClear");
        ObjectNode reward = mapper.createObjectNode();
        int percent = data.root("Pve/chapters.json").path("repeatRewardPercent").asInt();
        reward.put("silver", first ? base.path("silver").asLong() : repeat(base.path("silver").asLong(), percent));
        reward.put("heroExp", first ? base.path("heroExp").asLong()
                : repeat(base.path("heroExp").asLong(), percent));
        ObjectNode materials = reward.putObject("materials");
        List<String> ids = new ArrayList<>();
        base.path("materials").fieldNames().forEachRemaining(ids::add);
        ids.sort(Comparator.naturalOrder());
        int state = (int) seed ^ 0xA5A5DE40;
        for (String id : ids)
        {
            long count = base.path("materials").path(id).asLong();
            if (!first)
            {
                state = state * 1664525 + 1013904223;
                long scaled = count * percent;
                count = scaled / 100 + ((state >>> 16) % 100 < scaled % 100 ? 1 : 0);
            }
            if (count > 0)
            {
                materials.put(id, count);
            }
        }
        ArrayNode items = reward.putArray("items");
        state = (int) seed ^ 0x85EBCA6B;
        for (JsonNode drop : stage.path("rewards").path("drops"))
        {
            state = state * 1664525 + 1013904223;
            if (Integer.toUnsignedLong(state) % 1000 < drop.path("chancePermille").asInt())
            {
                ObjectNode item = items.addObject();
                item.put("id", drop.path("id").asText());
                item.put("tier", 1);
                item.put("rarity", drop.path("rarity").asText("common"));
            }
        }
        return reward;
    }

    private void applyPveReward(ProfileDocument profile, JsonNode stage, ObjectNode reward, boolean first)
    {
        ProfileGrid grid = new ProfileGrid(profile, data);
        List<String> newItems = new ArrayList<>();
        var materials = reward.path("materials").fields();
        while (materials.hasNext())
        {
            var entry = materials.next();
            if (!grid.addMaterial(entry.getKey(), entry.getValue().asInt()))
            {
                reject(NO_SPACE);
            }
        }
        for (JsonNode item : reward.path("items"))
        {
            String uid = addItem(profile, item.path("id").asText(), 1, item.path("rarity").asText());
            if (!grid.placeItem(uid))
            {
                reject(NO_SPACE);
            }
            newItems.add(uid);
        }
        ObjectNode progress = (ObjectNode) profile.json().path("pve").path("stages");
        progress.with(stage.path("id").asText()).put("cleared", true);
        record(profile, "pve_win");
        resetDaily(profile);
        addUnique((ArrayNode) profile.json().path("pve").path("dailyFirstClears"), stage.path("id").asText());
        addCurrency(profile, "bac", reward.path("silver").asLong());
        grantExp(profile, reward.path("heroExp").asLong());
        if (first && stage.path("index").asInt() == 10 && "boss".equals(stage.path("kind").asText()))
        {
            addUnique(profile.array("pendingStoryChapters"), stage.path("chapterIndex").asInt());
        }
    }

    private Loadout playerLoadout(ProfileDocument profile)
    {
        Loadout value = new Loadout();
        value.name = profile.json().path("heroName").asText();
        value.level = profile.json().path("heroLevel").asInt();
        value.sect = profile.json().path("sectId").isNull() ? null : profile.json().path("sectId").asText();
        value.tamPhapLevel = profile.json().path("tamPhapLevel").asInt();
        for (JsonNode placement : profile.json().path("loadout"))
        {
            ObjectNode owned = profile.item(placement.path("uid").asText());
            addPlaced(value, owned.path("itemId").asText(), placement, owned);
        }
        return value;
    }

    private Loadout enemyLoadout(JsonNode enemy)
    {
        Loadout value = new Loadout();
        value.name = enemy.path("name").asText();
        value.level = enemy.path("level").asInt();
        value.sect = enemy.path("sect").isNull() ? null : enemy.path("sect").asText();
        value.tamPhapLevel = enemy.path("tamPhapLevel").asInt(1);
        for (JsonNode item : enemy.path("items"))
        {
            addPlaced(value, item.path("id").asText(), item, item);
        }
        return value;
    }

    private static void addPlaced(Loadout loadout, String id, JsonNode placement, JsonNode item)
    {
        Loadout.PlacedItem value = new Loadout.PlacedItem(id, placement.path("x").asInt(), placement.path("y").asInt());
        value.tier = item.path("tier").asInt(1);
        value.rarity = Rarity.valueOf(item.path("rarity").asText("common").toUpperCase());
        loadout.items.add(value);
    }

    private boolean stageUnlocked(ProfileDocument profile, String id)
    {
        for (JsonNode chapter : data.root("Pve/chapters.json").path("chapters"))
        {
            JsonNode stages = chapter.path("stages");
            for (int i = 0; i < stages.size(); i++)
            {
                if (id.equals(stages.get(i).path("id").asText()))
                {
                    if (i > 0)
                    {
                        return isCleared(profile, stages.get(i - 1).path("id").asText());
                    }
                    int chapterIndex = chapter.path("index").asInt();
                    return chapterIndex == 1 || previousChapterCleared(profile, chapterIndex);
                }
            }
        }
        return false;
    }

    private boolean previousChapterCleared(ProfileDocument profile, int chapterIndex)
    {
        for (JsonNode chapter : data.root("Pve/chapters.json").path("chapters"))
        {
            if (chapter.path("index").asInt() == chapterIndex - 1)
            {
                JsonNode stages = chapter.path("stages");
                return isCleared(profile, stages.get(stages.size() - 1).path("id").asText());
            }
        }
        return false;
    }

    private JsonNode stage(String id)
    {
        for (JsonNode chapter : data.root("Pve/chapters.json").path("chapters"))
        {
            for (JsonNode value : chapter.path("stages"))
            {
                if (id.equals(value.path("id").asText()))
                {
                    ObjectNode copy = value.deepCopy();
                    copy.put("chapterIndex", chapter.path("index").asInt());
                    return copy;
                }
            }
        }
        return null;
    }

    private boolean questDone(ProfileDocument profile, JsonNode quest)
    {
        JsonNode objective = quest.path("objective");
        String type = objective.path("type").asText();
        int current = switch (type)
        {
            case "stage_clear" -> isCleared(profile, objective.path("stage").asText()) ? 1 : 0;
            case "hero_level" -> profile.json().path("heroLevel").asInt();
            case "sect_joined" -> profile.json().path("sectId").isNull() ? 0 : 1;
            case "tam_phap_level" -> profile.json().path("tamPhapLevel").asInt();
            case "equip_count" -> profile.json().path("loadout").size();
            case "item_tier" -> highestTier(profile);
            default -> actionProgress(profile, type);
        };
        int target = objective.path("count").asInt(objective.path("level").asInt(
                objective.path("tier").asInt(1)));
        return current >= target;
    }

    private int actionProgress(ProfileDocument profile, String action)
    {
        JsonNode quest = profile.json().path("quest");
        return quest.path("actionCounters").path(action).asInt()
                - quest.path("actionBaseline").path(action).asInt();
    }

    private void activateQuest(ProfileDocument profile)
    {
        ObjectNode state = (ObjectNode) profile.json().path("quest");
        if (state.path("activeId").asText().isBlank())
        {
            setQuestBaseline(profile);
        }
    }

    private void setQuestBaseline(ProfileDocument profile)
    {
        ObjectNode state = (ObjectNode) profile.json().path("quest");
        int index = state.path("activeIndex").asInt();
        JsonNode quests = data.root("quests.json").path("quests");
        state.put("activeId", index < quests.size() ? quests.get(index).path("id").asText() : "");
        state.set("actionBaseline", state.path("actionCounters").deepCopy());
    }

    private JsonNode activeQuest(ProfileDocument profile)
    {
        return find(data.root("quests.json").path("quests"), profile.json().path("quest").path("activeId").asText());
    }

    private void resetDaily(ProfileDocument profile)
    {
        ObjectNode pve = (ObjectNode) profile.json().path("pve");
        String today = LocalDate.ofInstant(clock.instant(), ArenaCalendar.ZONE).toString();
        if (!today.equals(pve.path("dailyFirstClearDate").asText()))
        {
            pve.put("dailyFirstClearDate", today);
            pve.putArray("dailyFirstClears");
        }
    }

    private void grantExp(ProfileDocument profile, long amount)
    {
        int level = profile.json().path("heroLevel").asInt();
        int max = data.root("levels.json").path("maxLevel").asInt();
        long experience = Math.addExact(profile.json().path("heroExp").asLong(), amount);
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
        return data.root("levels.json").path("expToNext").path(level - 1).asLong();
    }

    private List<JsonNode> materialsForOffer(String offerId)
    {
        List<JsonNode> result = new ArrayList<>();
        for (JsonNode material : data.root("materials.json"))
        {
            boolean match = "chi_ton_ngoc_tuy".equals(offerId)
                    ? "chi_ton_ngoc_tuy".equals(material.path("id").asText())
                    : "catalyst_box_random".equals(offerId)
                    ? material.path("isCatalyst").asBoolean()
                            && !"chi_ton_ngoc_tuy".equals(material.path("id").asText())
                    : material.path("tier").asInt() == 3 && !material.path("isCatalyst").asBoolean();
            if (match)
            {
                result.add(material);
            }
        }
        return result;
    }

    private JsonNode randomMaterial(int tier, boolean catalyst, String group)
    {
        List<JsonNode> choices = new ArrayList<>();
        for (JsonNode material : data.root("materials.json"))
        {
            if (material.path("tier").asInt() == tier && material.path("isCatalyst").asBoolean() == catalyst
                    && !"chi_ton_ngoc_tuy".equals(material.path("id").asText())
                    && (group == null || group.equals(material.path("itemType").asText())))
            {
                choices.add(material);
            }
        }
        return choices.get(random.nextInt(choices.size()));
    }

    private boolean monsterItem(String id)
    {
        for (JsonNode value : data.root("Pve/chapters.json").path("monsterItems"))
        {
            if (id.equals(value.asText()))
            {
                return true;
            }
        }
        return false;
    }

    private String weighted(JsonNode values, JsonNode weights)
    {
        int roll = random.nextInt(100);
        for (int i = 0; i < weights.size(); i++)
        {
            roll -= weights.get(i).asInt();
            if (roll < 0)
            {
                return values.get(i).asText();
            }
        }
        return values.get(values.size() - 1).asText();
    }

    private static long repeat(long amount, int percent)
    {
        return amount <= 0 ? 0 : Math.max(1, RuleMath.roundHalfAwayFromZero(amount * percent / 100d));
    }

    private static boolean forbiddenMaterial(JsonNode material)
    {
        return material.path("isCatalyst").asBoolean()
                || "chi_ton_ngoc_tuy".equals(material.path("id").asText());
    }

    private static JsonNode find(JsonNode values, String id)
    {
        for (JsonNode value : values)
        {
            if (id != null && id.equals(value.path("id").asText()))
            {
                return value;
            }
        }
        return null;
    }

    private static boolean isCleared(ProfileDocument profile, String id)
    {
        return profile.json().path("pve").path("stages").path(id).path("cleared").asBoolean();
    }

    private static int highestTier(ProfileDocument profile)
    {
        int highest = 0;
        for (JsonNode item : profile.json().path("items"))
        {
            highest = Math.max(highest, item.path("tier").asInt());
        }
        return highest;
    }

    private static String addItem(ProfileDocument profile, String id, int tier, String rarity)
    {
        String uid = UUID.randomUUID().toString().replace("-", "");
        ObjectNode item = profile.array("items").addObject();
        item.put("uid", uid);
        item.put("itemId", id);
        item.put("tier", tier);
        item.put("rarity", rarity);
        item.put("acquiredAt", Instant.now().toString());
        return uid;
    }

    private static void removeItem(ProfileDocument profile, String uid)
    {
        ArrayNode items = profile.array("items");
        for (int i = items.size() - 1; i >= 0; i--)
        {
            if (uid.equals(items.get(i).path("uid").asText()))
            {
                items.remove(i);
            }
        }
    }

    private static void record(ProfileDocument profile, String action)
    {
        ObjectNode counters = (ObjectNode) profile.json().path("quest").path("actionCounters");
        counters.put(action, Math.addExact(counters.path(action).asInt(), 1));
    }

    private static void addCurrency(ProfileDocument profile, String id, long amount)
    {
        profile.currencies().put(id, Math.addExact(profile.currencies().path(id).asLong(), amount));
    }

    private static void requireCurrency(ProfileDocument profile, String id, long cost, String name)
    {
        long value = profile.currencies().path(id).asLong();
        if (value < cost)
        {
            reject("Không đủ " + name + " (" + value + "/" + cost + ").");
        }
    }

    private static Instant instant(String value)
    {
        try
        {
            return value == null || value.isBlank() ? null : Instant.parse(value);
        }
        catch (Exception ignored)
        {
            return null;
        }
    }

    private static void addUnique(ArrayNode values, String value)
    {
        for (JsonNode item : values)
        {
            if (value.equals(item.asText()))
            {
                return;
            }
        }
        values.add(value);
    }

    private static void addUnique(ArrayNode values, int value)
    {
        for (JsonNode item : values)
        {
            if (value == item.asInt())
            {
                return;
            }
        }
        values.add(value);
    }

    private static void removeValue(ArrayNode values, int value)
    {
        for (int i = values.size() - 1; i >= 0; i--)
        {
            if (values.get(i).asInt() == value)
            {
                values.remove(i);
            }
        }
    }

    private static void reject(String message)
    {
        throw new ServiceException("rejected", message);
    }
}
