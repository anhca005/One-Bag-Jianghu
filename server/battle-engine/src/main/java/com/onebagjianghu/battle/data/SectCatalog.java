package com.onebagjianghu.battle.data;

import com.fasterxml.jackson.core.type.TypeReference;

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

/** Validated set of sects keyed by id. */
public final class SectCatalog {
    public static final String DEFAULT_RESOURCE = "/data/sects.json";
    /** Levels at which a Tâm Pháp may gain something; 1 is the base. */
    public static final List<Integer> LEVELS = List.of(1, 3, 6, 9);
    public static final int MAX_LEVEL = 9;

    private final Map<String, SectDef> sects;

    private SectCatalog(Map<String, SectDef> sects) {
        this.sects = Collections.unmodifiableMap(sects);
    }

    public static SectCatalog empty() {
        return new SectCatalog(new LinkedHashMap<>());
    }

    public static SectCatalog of(Collection<SectDef> defs) {
        Map<String, SectDef> map = new LinkedHashMap<>();
        for (SectDef def : defs) {
            validate(def);
            if (map.put(def.id, def) != null) {
                throw new DataException("Duplicate sect id: " + def.id);
            }
        }
        return new SectCatalog(map);
    }

    public static SectCatalog load(InputStream in) {
        try {
            return of(ItemCatalog.mapper().readValue(in, new TypeReference<List<SectDef>>() { }));
        } catch (IOException e) {
            throw new DataException("Cannot read sect data: " + e.getMessage(), e);
        }
    }

    public static SectCatalog load(Path path) {
        try (InputStream in = Files.newInputStream(path)) {
            return load(in);
        } catch (IOException e) {
            throw new DataException("Cannot open " + path + ": " + e.getMessage(), e);
        }
    }

    public static SectCatalog loadDefault() {
        try (InputStream in = SectCatalog.class.getResourceAsStream(DEFAULT_RESOURCE)) {
            if (in == null) {
                throw new DataException("Missing resource " + DEFAULT_RESOURCE);
            }
            return load(in);
        } catch (IOException e) {
            throw new DataException("Cannot read " + DEFAULT_RESOURCE, e);
        }
    }

    public SectDef get(String id) {
        SectDef def = sects.get(id);
        if (def == null) {
            throw new DataException("Unknown sect id: " + id);
        }
        return def;
    }

    public Collection<SectDef> all() {
        return sects.values();
    }

    private static void validate(SectDef def) {
        List<String> errors = new ArrayList<>();
        if (def.id == null || def.id.isBlank()) errors.add("id is required");
        if (def.name == null || def.name.isBlank()) errors.add("name is required");
        if (def.tamPhap == null || def.tamPhap.isBlank()) errors.add("tamPhap is required");
        if (def.weaponClasses.isEmpty()) errors.add("weaponClasses must not be empty");
        int last = 0;
        for (int i = 0; i < def.levels.size(); i++) {
            TamPhapLevelDef l = def.levels.get(i);
            String w = "levels[" + i + "]";
            if (!LEVELS.contains(l.level)) errors.add(w + ".level must be one of " + LEVELS);
            else if (l.level <= last) errors.add("levels must be sorted by level without duplicates");
            last = Math.max(last, l.level);
            if (l.attackPct < 0) errors.add(w + ".attackPct must be >= 0");
            if (l.cooldownPct < 0 || l.cooldownPct >= 0.5) errors.add(w + ".cooldownPct must be in [0, 0.5)");
            if (l.crit < 0 || l.crit > 1) errors.add(w + ".crit must be in [0, 1]");
            ItemCatalog.validateEffects(l.basic, w + ".basic", errors);
            ItemCatalog.validateEffects(l.ultimate, w + ".ultimate", errors);
            ItemCatalog.validateTriggers(l.triggers, w + ".triggers", errors);
        }
        if (!def.levels.isEmpty() && def.levels.get(0).level != 1) errors.add("the first level must be 1");
        if (!errors.isEmpty()) {
            throw new DataException("Sect " + (def.id == null ? "<no id>" : def.id) + ": " + String.join("; ", errors));
        }
    }
}
