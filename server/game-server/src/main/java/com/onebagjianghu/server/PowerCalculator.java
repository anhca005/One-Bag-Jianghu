package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.List;

/** Calculates combat power from a hero level and equipped item attributes. */
final class PowerCalculator
{
    record Item(String itemId, int tier, String rarity) { }

    private final JsonNode config;

    PowerCalculator(JsonNode config)
    {
        this.config = config;
    }

    int calculate(int requestedLevel, List<Item> items)
    {
        int level = Math.max(1, requestedLevel);
        double total = config.path("levelBase").asDouble()
                + config.path("levelPoints").asDouble() * (level - 1);
        for (Item item : items)
        {
            total += config.path("items").path(item.itemId()).asDouble(25)
                    * config.path("tierFactor").path(Integer.toString(item.tier())).asDouble(1)
                    * config.path("rarityFactor").path(item.rarity()).asDouble(1);
        }
        return (int) Math.rint(total);
    }
}
