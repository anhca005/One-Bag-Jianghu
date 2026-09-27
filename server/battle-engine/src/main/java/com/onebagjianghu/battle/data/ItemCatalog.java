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
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/** Validated, read-only set of item definitions keyed by id. */
public final class ItemCatalog {
    public static final String DEFAULT_RESOURCE = "/data/items.json";
    /** Binh khí không có loại 1x1. */
    public static final int MIN_WEAPON_CELLS = 2;

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
        if (def.id == null || def.id.isBlank()) errors.add("id is required");
        if (def.name == null || def.name.isBlank()) errors.add("name is required");
        if (def.type == null) errors.add("type is required");
        if (def.rarity == null) errors.add("rarity is required");
        if (def.size == null || def.size.length != 2 || def.size[0] < 1 || def.size[1] < 1) {
            errors.add("size must be [w, h] with w, h >= 1");
        }
        if (def.cooldown < 0) errors.add("cooldown must be >= 0");
        if (def.crit < 0 || def.crit > 1) errors.add("crit must be in [0, 1]");
        if (def.type == ItemType.WEAPON && !def.isActive()) errors.add("weapon needs cooldown > 0");
        if (def.type == ItemType.WEAPON && def.size != null && def.size.length == 2
                && def.size[0] * def.size[1] < MIN_WEAPON_CELLS) {
            errors.add("weapon must occupy at least " + MIN_WEAPON_CELLS + " cells");
        }
        if (!def.isActive() && !def.basic.isEmpty()) errors.add("basic effects need cooldown > 0");
        if (def.ultimate != null) {
            if (!def.isActive()) errors.add("ultimate needs cooldown > 0");
            if (def.ultimate.cost <= 0) errors.add("ultimate.cost must be > 0");
            validateEffects(def.ultimate.effects, "ultimate", errors);
        }
        validateEffects(def.basic, "basic", errors);
        for (int i = 0; i < def.triggers.size(); i++) {
            TriggerDef t = def.triggers.get(i);
            String tw = "triggers[" + i + "]";
            if (t.on == null) errors.add(tw + ".on is required");
            if (t.on == TriggerEvent.HP_BELOW && (t.hpBelow <= 0 || t.hpBelow >= 1)) {
                errors.add(tw + ".hpBelow must be in (0, 1)");
            }
            if (t.maxUses < 0) errors.add(tw + ".maxUses must be >= 0");
            validateEffects(t.effects, tw, errors);
        }
        if (!errors.isEmpty()) {
            throw new DataException("Item " + where + ": " + String.join("; ", errors));
        }
    }

    private static void validateEffects(List<EffectDef> effects, String where, List<String> errors) {
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
            if (e.hits < 1) errors.add(ew + ": hits must be >= 1");
            Target expected = e.op == EffectOp.APPLY ? e.status.defaultTarget : e.op.defaultTarget;
            Target actual = e.resolvedTarget();
            if (actual.kind != expected.kind) {
                errors.add(ew + ": target " + actual + " is not valid for " + e.op
                        + (e.op == EffectOp.APPLY ? " " + e.status : ""));
            }
        }
    }
}
