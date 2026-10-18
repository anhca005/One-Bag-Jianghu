package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.Loadout;

import java.util.Set;
import java.util.Locale;
import java.util.stream.Collectors;
import java.util.stream.Stream;

/** Applies server-owned bag dimensions and validates client-owned progression fields. */
final class LoadoutNormalizer
{
    private static final Set<String> RARITIES = Stream.of(Rarity.values())
            .map(value -> value.name().toLowerCase(Locale.ROOT))
            .collect(Collectors.toUnmodifiableSet());
    private final ObjectMapper mapper;
    private final SectCatalog sects;

    LoadoutNormalizer(ObjectMapper mapper, SectCatalog sects)
    {
        this.mapper = mapper;
        this.sects = sects;
    }

    Loadout normalize(JsonNode supplied, boolean preserveLevel)
    {
        if (supplied == null || !supplied.isObject())
        {
            throw new DataException("loadout is required");
        }
        if (supplied.has("hp"))
        {
            throw new DataException("Loadout cannot contain hp; use level");
        }
        int level = preserveLevel ? supplied.path("level").asInt(0) : 1;
        if (level < 1 || level > 60)
        {
            throw new DataException("level must be in 1..60");
        }
        JsonNode tamPhap = supplied.get("tamPhapLevel");
        int tamPhapLevel = tamPhap == null || tamPhap.isNull() ? 1 : tamPhap.asInt(0);
        if (tamPhapLevel < 1 || tamPhapLevel > SectCatalog.MAX_LEVEL)
        {
            throw new DataException("tamPhapLevel must be in 1..9");
        }
        for (JsonNode item : supplied.path("items"))
        {
            int tier = item.path("tier").asInt(1);
            String rarity = item.path("rarity").asText("common").toLowerCase(Locale.ROOT);
            if (tier < 1 || tier > 9)
            {
                throw new DataException("item tier must be in 1..9");
            }
            if (!RARITIES.contains(rarity))
            {
                throw new DataException("Unknown item rarity: " + rarity);
            }
        }
        ObjectNode normalized = ((ObjectNode) supplied).deepCopy();
        normalized.put("level", level);
        normalized.put("cols", 5);
        normalized.put("rows", 4);
        normalized.put("tamPhapLevel", tamPhapLevel);
        try
        {
            Loadout loadout = mapper.treeToValue(normalized, Loadout.class);
            if (loadout.sect != null)
            {
                sects.get(loadout.sect);
            }
            return loadout;
        }
        catch (DataException exception)
        {
            throw exception;
        }
        catch (Exception exception)
        {
            throw new DataException("Cannot read loadout: " + exception.getMessage(), exception);
        }
    }
}
