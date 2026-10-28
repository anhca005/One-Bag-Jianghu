package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

/** Version-9 profile JSON model. It deliberately retains fields owned by the next E6 batch. */
public final class ProfileDocument
{
    public static final int VERSION = 9;
    private final ObjectNode root;

    public ProfileDocument(ObjectNode root)
    {
        this.root = root;
    }

    public ObjectNode json()
    {
        return root;
    }

    public ObjectNode item(String uid)
    {
        for (JsonNode value : array("items"))
        {
            if (uid != null && uid.equals(value.path("uid").asText()))
            {
                return (ObjectNode) value;
            }
        }
        return null;
    }

    public ArrayNode array(String name)
    {
        return (ArrayNode) root.get(name);
    }

    public ObjectNode storage()
    {
        return (ObjectNode) root.get("storage");
    }

    public ObjectNode currencies()
    {
        return (ObjectNode) root.get("currencies");
    }

    public ObjectNode materials()
    {
        return (ObjectNode) root.get("materials");
    }

    public void validate(GameData data)
    {
        if (root.path("version").asInt(-1) != VERSION)
        {
            invalid("Unsupported profile version; expected 9.");
        }
        if (root.path("heroLevel").asInt(0) < 1 || root.path("heroExp").asLong(-1) < 0)
        {
            invalid("Profile hero level or experience is invalid.");
        }
        int tamPhap = root.path("tamPhapLevel").asInt(0);
        if (tamPhap < 1 || tamPhap > 9)
        {
            invalid("Profile tamPhapLevel must be from 1 to 9.");
        }
        int cols = storage().path("cols").asInt(0);
        int rows = storage().path("rows").asInt(0);
        if (cols < 1 || rows < 1)
        {
            invalid("Profile storage dimensions must be positive.");
        }
        Set<String> owned = new HashSet<>();
        for (JsonNode value : array("items"))
        {
            String uid = value.path("uid").asText("");
            int tier = value.path("tier").asInt(0);
            if (uid.isBlank() || !owned.add(uid) || tier < 1 || tier > 9)
            {
                invalid("Profile contains an invalid or duplicate owned item.");
            }
            data.item(value.path("itemId").asText());
        }
        Set<String> placed = new HashSet<>();
        validatePlacements(array("loadout"), owned, placed, "Loadout");
        validatePlacements((ArrayNode) storage().get("items"), owned, placed, "Storage");
        Map<String, Integer> totals = new HashMap<>();
        Set<String> cells = new HashSet<>();
        for (JsonNode stack : storage().withArray("materialStacks"))
        {
            int count = stack.path("count").asInt(0);
            String id = stack.path("materialId").asText("");
            String cell = stack.path("x").asInt() + ":" + stack.path("y").asInt();
            if (id.isBlank() || data.material(id) == null || count < 1 || count > 999 || !cells.add(cell))
            {
                invalid("Storage contains an invalid material stack.");
            }
            totals.merge(id, count, Math::addExact);
        }
        Iterator<Map.Entry<String, JsonNode>> fields = materials().fields();
        while (fields.hasNext())
        {
            Map.Entry<String, JsonNode> value = fields.next();
            if (value.getValue().asInt(-1) != totals.getOrDefault(value.getKey(), 0))
            {
                invalid("Material total '" + value.getKey() + "' is not synchronized with storage stacks.");
            }
        }
        for (Map.Entry<String, Integer> value : totals.entrySet())
        {
            if (materials().path(value.getKey()).asInt(-1) != value.getValue())
            {
                invalid("Material total '" + value.getKey() + "' is not synchronized with storage stacks.");
            }
        }
    }

    private static void validatePlacements(ArrayNode values, Set<String> owned, Set<String> placed, String kind)
    {
        for (JsonNode value : values)
        {
            String uid = value.path("uid").asText("");
            if (!owned.contains(uid) || !placed.add(uid))
            {
                invalid(kind + " references an unknown or duplicate owned item.");
            }
        }
    }

    private static void invalid(String message)
    {
        throw new IllegalStateException(message);
    }
}
