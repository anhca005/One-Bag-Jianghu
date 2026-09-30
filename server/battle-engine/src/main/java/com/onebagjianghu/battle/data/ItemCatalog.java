package com.onebagjianghu.battle.data;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.json.JsonMapper;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.EnumSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

/** Validated, read-only set of item definitions keyed by id. */
public final class ItemCatalog {
    public static final String DEFAULT_RESOURCE = "/data/items.json";
    /** Binh khí không có loại 1x1. */
    public static final int MIN_WEAPON_CELLS = 2;
    /** Trọng at which Đột Phá may unlock new effects. */
    public static final List<Integer> UNLOCK_TIERS = List.of(3, 6, 9);
    /** Events raised by a specific item, the only ones where a trigger's {@code from} filter means anything. */
    private static final Set<TriggerEvent> ITEM_EVENTS = EnumSet.of(
            TriggerEvent.HIT, TriggerEvent.CRIT, TriggerEvent.ULTIMATE, TriggerEvent.PARRIED,
            TriggerEvent.STATUS_APPLIED, TriggerEvent.BURST, TriggerEvent.DAMAGED,
            TriggerEvent.HEALED, TriggerEvent.CONTROLLED, TriggerEvent.CRIT_TAKEN,
            TriggerEvent.ENEMY_ULTIMATE, TriggerEvent.QI_EMPTY, TriggerEvent.SHIELD_BROKEN,
            TriggerEvent.BURST_TAKEN);
    /** Events for which the causal item may belong to the opposing side. */
    private static final Set<TriggerEvent> ENEMY_SOURCE_EVENTS = EnumSet.of(
            TriggerEvent.DAMAGED, TriggerEvent.CONTROLLED, TriggerEvent.CRIT_TAKEN,
            TriggerEvent.ENEMY_ULTIMATE, TriggerEvent.QI_EMPTY, TriggerEvent.BURST_TAKEN);

    private final Map<String, ItemDef> items;

    private ItemCatalog(Map<String, ItemDef> items) {
        this.items = Collections.unmodifiableMap(items);
    }

    public static ObjectMapper mapper() {
        return JsonMapper.builder()
                .enable(MapperFeature.ACCEPT_CASE_INSENSITIVE_ENUMS)
                .enable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES)
                .build();
    }

    public static ItemCatalog of(Collection<ItemDef> defs) {
        Map<String, ItemDef> map = new LinkedHashMap<>();
        for (ItemDef def : defs) {
            validate(def);
            if (map.put(def.id, def) != null) {
                throw new DataException("Duplicate item id: " + def.id);
            }
        }
        return new ItemCatalog(map);
    }

    public static ItemCatalog load(InputStream in) {
        try {
            List<ItemDef> defs = mapper().readValue(in, new TypeReference<List<ItemDef>>() { });
            return of(defs);
        } catch (IOException e) {
            throw new DataException("Cannot read item data: " + e.getMessage(), e);
        }
    }

    public static ItemCatalog load(Path path) {
        try (InputStream in = Files.newInputStream(path)) {
            return load(in);
        } catch (IOException e) {
            throw new DataException("Cannot open " + path + ": " + e.getMessage(), e);
        }
    }

    public static ItemCatalog loadDefault() {
        try (InputStream in = ItemCatalog.class.getResourceAsStream(DEFAULT_RESOURCE)) {
            if (in == null) {
                throw new DataException("Missing resource " + DEFAULT_RESOURCE);
            }
            return load(in);
        } catch (IOException e) {
            throw new DataException("Cannot read " + DEFAULT_RESOURCE, e);
        }
    }

    public ItemDef get(String id) {
        ItemDef def = items.get(id);
        if (def == null) {
            throw new DataException("Unknown item id: " + id);
        }
        return def;
    }

    public boolean contains(String id) {
        return items.containsKey(id);
    }

    public Collection<ItemDef> all() {
        return items.values();
    }

    private static void validate(ItemDef def) {
        List<String> errors = new ArrayList<>();
        String where = def.id == null ? "<no id>" : def.id;
        validateCore(def, errors);
        int lastTier = 0;
        for (int i = 0; i < def.unlocks.size(); i++) {
            UnlockDef u = def.unlocks.get(i);
            if (!UNLOCK_TIERS.contains(u.tier)) errors.add("unlocks[" + i + "].tier must be one of " + UNLOCK_TIERS);
            else if (u.tier <= lastTier) errors.add("unlocks must be sorted by tier without duplicates");
            lastTier = Math.max(lastTier, u.tier);
        }
        // Check the item as it plays at each Đột Phá; skipped when the base is already invalid to avoid duplicates.
        if (errors.isEmpty()) {
            for (UnlockDef u : def.unlocks) {
                List<String> tierErrors = new ArrayList<>();
                validateCore(def.atTier(u.tier), tierErrors);
                tierErrors.forEach(e -> errors.add("at tier " + u.tier + ": " + e));
            }
        }
        if (!errors.isEmpty()) {
            throw new DataException("Item " + where + ": " + String.join("; ", errors));
        }
    }

    private static void validateCore(ItemDef def, List<String> errors) {
        if (def.id == null || def.id.isBlank()) errors.add("id is required");
        if (def.name == null || def.name.isBlank()) errors.add("name is required");
        if (def.type == null) errors.add("type is required");
        if (def.rarity == null) errors.add("rarity must be valid when specified");
        if (def.size == null || def.size.length != 2 || def.size[0] < 1 || def.size[1] < 1) {
            errors.add("size must be [w, h] with w, h >= 1");
        }
        if (def.cooldown < 0) errors.add("cooldown must be >= 0");
        if (def.attack < 0) errors.add("attack must be >= 0");
        if (def.crit < 0 || def.crit > 1) errors.add("crit must be in [0, 1]");
        if (def.type == ItemType.WEAPON && !def.isActive()) errors.add("weapon needs cooldown > 0");
        if (def.type == ItemType.WEAPON && def.attack <= 0) errors.add("weapon needs attack > 0");
        if (def.type == ItemType.WEAPON && def.size != null && def.size.length == 2
                && def.size[0] * def.size[1] < MIN_WEAPON_CELLS) {
            errors.add("weapon must occupy at least " + MIN_WEAPON_CELLS + " cells");
        }
        if (!def.isActive() && !def.basic.isEmpty()) errors.add("basic effects need cooldown > 0");
        if (!def.active.isEmpty() && (def.type != ItemType.RELIC && def.type != ItemType.MANUAL)) {
            errors.add("active effects are only valid for relic or manual");
        }
        if (!def.active.isEmpty() && !def.isActive()) errors.add("active effects need cooldown > 0");
        if (def.isActive() && (def.type == ItemType.RELIC || def.type == ItemType.MANUAL) && def.active.isEmpty()) {
            errors.add("active relic/manual needs active effects");
        }
        if (def.ultimate != null) {
            if (!def.isActive()) errors.add("ultimate needs cooldown > 0");
            if (def.ultimate.cost <= 0) errors.add("ultimate.cost must be > 0");
            validateEffects(def.ultimate.effects, "ultimate", true, errors);
        }
        validateEffects(def.basic, "basic", false, errors);
        validateEffects(def.active, "active", false, errors);
        validateTriggers(def.triggers, "triggers", errors);
    }

    static void validateTriggers(List<TriggerDef> triggers, String where, List<String> errors) {
        for (int i = 0; i < triggers.size(); i++) {
            TriggerDef t = triggers.get(i);
            String tw = where + "[" + i + "]";
            if (t.on == null) errors.add(tw + ".on is required");
            if (t.on == TriggerEvent.HP_BELOW && (t.hpBelow <= 0 || t.hpBelow >= 1)) {
                errors.add(tw + ".hpBelow must be in (0, 1)");
            }
            if (t.maxUses < 0) errors.add(tw + ".maxUses must be >= 0");
            // "from" only filters events raised by a specific item; on side-level events it would be ignored.
            if (t.on != null && t.from != TriggerSource.ANY && !ITEM_EVENTS.contains(t.on)) {
                errors.add(tw + ": from=" + t.from + " is only valid for " + ITEM_EVENTS);
            }
            if (t.on != null && t.from == TriggerSource.ENEMY && !ENEMY_SOURCE_EVENTS.contains(t.on)) {
                errors.add(tw + ": from=ENEMY is not meaningful for " + t.on);
            }
            validateEffects(t.effects, tw, false, errors);
        }
    }

    static void validateEffects(List<EffectDef> effects, String where, List<String> errors) {
        validateEffects(effects, where, false, errors);
    }

    static void validateEffects(List<EffectDef> effects, String where, boolean ultimate, List<String> errors) {
        for (int i = 0; i < effects.size(); i++) {
            EffectDef e = effects.get(i);
            String ew = where + "[" + i + "]";
            if (e.op == null) {
                errors.add(ew + ".op is required");
                continue;
            }
            if (e.op == EffectOp.APPLY) {
                if (e.status == null) {
                    errors.add(ew + ": APPLY needs status");
                    continue;
                }
                if (e.status.timed && e.duration <= 0) errors.add(ew + ": " + e.status + " needs duration");
                if (!e.status.timed && e.stacks <= 0) errors.add(ew + ": " + e.status + " needs stacks");
            }
            if (e.op == EffectOp.CHARGE && e.duration <= 0) errors.add(ew + ": CHARGE needs duration");
            if (e.minQiAfter != null && !ultimate) errors.add(ew + ": minQiAfter is only valid inside ultimate effects");
            if (e.minQiAfter != null && e.minQiAfter < 0) errors.add(ew + ": minQiAfter must be >= 0");
            if (e.hits < 1) errors.add(ew + ": hits must be >= 1");
            if (e.hits > 1 && e.op != EffectOp.DAMAGE) errors.add(ew + ": hits only applies to DAMAGE");
            switch (e.op) {
                case DAMAGE -> {
                    if (e.value < 0 || (e.value == 0 && e.pct <= 0)) errors.add(ew + ": DAMAGE needs pct > 0 or value > 0");
                }
                case GAIN_QI, DRAIN_QI, SHIELD, HEAL, SHIELD_BREAK, REDUCE_MAX_HP, PARRY_DISCOUNT, ADD_ATTACK -> {
                    if (e.value <= 0) errors.add(ew + ": " + e.op + " needs value > 0");
                }
                case DMG_REDUCE, ADD_CRIT -> {
                    if (e.pct <= 0 || e.pct > 1) errors.add(ew + ": " + e.op + " needs pct in (0, 1]");
                }
                case BUFF_STACK -> {
                    if (!Set.of("attack", "crit", "dmg_reduce").contains(e.stat)) errors.add(ew + ": BUFF_STACK stat must be attack, crit, or dmg_reduce");
                    if (e.maxStacks <= 0) errors.add(ew + ": BUFF_STACK needs maxStacks > 0");
                    if ("attack".equals(e.stat) && e.value <= 0) errors.add(ew + ": attack BUFF_STACK needs value > 0");
                    if (("crit".equals(e.stat) || "dmg_reduce".equals(e.stat)) && (e.pct <= 0 || e.pct > 1)) errors.add(ew + ": " + e.stat + " BUFF_STACK needs pct in (0, 1]");
                }
                case CLEANSE -> {
                    if (e.status != null && !Set.of(StatusType.BURN, StatusType.POISON, StatusType.BLEED).contains(e.status)) errors.add(ew + ": CLEANSE status must be burn, poison, or bleed");
                    if (e.value < 0) errors.add(ew + ": CLEANSE value must be >= 0");
                }
                case LIFESTEAL -> {
                    if (e.pct <= 0 || e.pct > 1) errors.add(ew + ": LIFESTEAL needs pct in (0, 1]");
                    boolean hasPrecedingDamage = false;
                    for (int j = i - 1; j >= 0; j--) {
                        if (effects.get(j).op == EffectOp.DAMAGE) { hasPrecedingDamage = true; break; }
                    }
                    if (!hasPrecedingDamage) errors.add(ew + ": LIFESTEAL needs a preceding DAMAGE in the same list");
                }
                case RAISE_MAX_HP -> { if (e.value <= 0) errors.add(ew + ": RAISE_MAX_HP needs value > 0"); }
                default -> { }
            }
            Target expected = e.op == EffectOp.APPLY ? (e.status == null ? null : e.status.defaultTarget)
                    : e.op == EffectOp.BUFF_STACK ? ("dmg_reduce".equals(e.stat) ? Target.SELF : Target.SELF_ITEM)
                    : e.op.defaultTarget;
            Target actual = e.resolvedTarget();
            if (actual == null || expected == null || actual.kind != expected.kind) {
                errors.add(ew + ": target " + actual + " is not valid for " + e.op
                        + (e.op == EffectOp.APPLY ? " " + e.status : ""));
            }
        }
    }
}
