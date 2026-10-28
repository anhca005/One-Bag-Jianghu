package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/** C# StorageGrid scan order and stack behavior. */
final class ProfileGrid
{
    private final ProfileDocument profile;
    private final GameData data;
    private final ObjectNode storage;

    ProfileGrid(ProfileDocument profile, GameData data)
    {
        this.profile = profile;
        this.data = data;
        storage = profile.storage();
    }

    boolean moveItem(String uid, int x, int y)
    {
        ObjectNode placement = findItemPlacement(uid);
        ObjectNode owned = profile.item(uid);
        if (placement == null || owned == null)
        {
            return false;
        }
        int[] size = size(owned.path("itemId").asText());
        if (!canPlace(x, y, size[0], size[1], uid, null))
        {
            return false;
        }
        placement.put("x", x);
        placement.put("y", y);
        return true;
    }

    boolean moveStack(String materialId, int fromX, int fromY, int toX, int toY)
    {
        ObjectNode found = null;
        for (JsonNode value : storage.withArray("materialStacks"))
        {
            if (materialId.equals(value.path("materialId").asText()) && value.path("x").asInt() == fromX
                    && value.path("y").asInt() == fromY)
            {
                found = (ObjectNode) value;
                break;
            }
        }
        if (found == null || !canPlace(toX, toY, 1, 1, null, found))
        {
            return false;
        }
        found.put("x", toX);
        found.put("y", toY);
        return true;
    }

    boolean addMaterial(String id, int amount)
    {
        if (amount <= 0 || data.material(id) == null || !canAddMaterial(id, amount))
        {
            return false;
        }
        int remaining = amount;
        ArrayNode stacks = storage.withArray("materialStacks");
        for (JsonNode value : stacks)
        {
            ObjectNode stack = (ObjectNode) value;
            if (!id.equals(stack.path("materialId").asText()) || stack.path("count").asInt() >= 999)
            {
                continue;
            }
            int add = Math.min(remaining, 999 - stack.path("count").asInt());
            stack.put("count", stack.path("count").asInt() + add);
            remaining -= add;
            if (remaining == 0)
            {
                break;
            }
        }
        while (remaining > 0)
        {
            int[] cell = first(1, 1);
            ObjectNode stack = stacks.addObject();
            stack.put("materialId", id);
            stack.put("count", Math.min(remaining, 999));
            stack.put("x", cell[0]);
            stack.put("y", cell[1]);
            remaining -= stack.path("count").asInt();
        }
        syncMaterials();
        return true;
    }

    boolean spend(Map<String, Integer> costs)
    {
        for (Map.Entry<String, Integer> cost : costs.entrySet())
        {
            if (profile.materials().path(cost.getKey()).asInt() < cost.getValue())
            {
                return false;
            }
        }
        ArrayNode stacks = storage.withArray("materialStacks");
        for (Map.Entry<String, Integer> cost : costs.entrySet())
        {
            int remaining = cost.getValue();
            for (int i = stacks.size() - 1; i >= 0 && remaining > 0; i--)
            {
                ObjectNode stack = (ObjectNode) stacks.get(i);
                if (!cost.getKey().equals(stack.path("materialId").asText()))
                {
                    continue;
                }
                int take = Math.min(remaining, stack.path("count").asInt());
                stack.put("count", stack.path("count").asInt() - take);
                remaining -= take;
                if (stack.path("count").asInt() == 0)
                {
                    stacks.remove(i);
                }
            }
        }
        syncMaterials();
        return true;
    }

    void repack()
    {
        mergeStacks();
        List<ObjectNode> itemCopies = copies(storage.withArray("items"));
        List<ObjectNode> stackCopies = copies(storage.withArray("materialStacks"));
        storage.withArray("items").removeAll();
        storage.withArray("materialStacks").removeAll();
        for (ObjectNode placement : itemCopies)
        {
            ObjectNode owned = profile.item(placement.path("uid").asText());
            int[] itemSize = size(owned.path("itemId").asText());
            int[] cell = first(itemSize[0], itemSize[1]);
            placement.put("x", cell[0]);
            placement.put("y", cell[1]);
            storage.withArray("items").add(placement);
        }
        for (ObjectNode stack : stackCopies)
        {
            int[] cell = first(1, 1);
            stack.put("x", cell[0]);
            stack.put("y", cell[1]);
            storage.withArray("materialStacks").add(stack);
        }
        syncMaterials();
    }

    boolean placeItem(String uid)
    {
        ObjectNode owned = profile.item(uid);
        int[] itemSize = size(owned.path("itemId").asText());
        int[] cell = first(itemSize[0], itemSize[1]);
        if (cell == null)
        {
            return false;
        }
        ObjectNode placement = storage.withArray("items").addObject();
        placement.put("uid", uid);
        placement.put("x", cell[0]);
        placement.put("y", cell[1]);
        return true;
    }

    void removeItemPlacement(String uid)
    {
        ArrayNode values = storage.withArray("items");
        for (int i = values.size() - 1; i >= 0; i--)
        {
            if (uid.equals(values.get(i).path("uid").asText()))
            {
                values.remove(i);
            }
        }
    }

    int[] first(int width, int height)
    {
        for (int row = 0; row <= storage.path("rows").asInt() - height; row++)
        {
            for (int col = 0; col <= storage.path("cols").asInt() - width; col++)
            {
                if (canPlace(col, row, width, height, null, null))
                {
                    return new int[] { col, row };
                }
            }
        }
        return null;
    }

    private boolean canAddMaterial(String id, int amount)
    {
        int free = 0;
        for (JsonNode stack : storage.withArray("materialStacks"))
        {
            if (id.equals(stack.path("materialId").asText()))
            {
                free += 999 - stack.path("count").asInt();
            }
        }
        int cells = Math.max(0, amount - free + 998) / 999;
        int available = storage.path("cols").asInt() * storage.path("rows").asInt() - usedCells();
        return cells <= available;
    }

    private int usedCells()
    {
        int used = storage.withArray("materialStacks").size();
        for (JsonNode placement : storage.withArray("items"))
        {
            ObjectNode owned = profile.item(placement.path("uid").asText());
            int[] itemSize = size(owned.path("itemId").asText());
            used += itemSize[0] * itemSize[1];
        }
        return used;
    }

    private boolean canPlace(int x, int y, int width, int height, String ignoreUid, ObjectNode ignoreStack)
    {
        if (x < 0 || y < 0 || x + width > storage.path("cols").asInt()
                || y + height > storage.path("rows").asInt())
        {
            return false;
        }
        for (JsonNode value : storage.withArray("items"))
        {
            if (ignoreUid != null && ignoreUid.equals(value.path("uid").asText()))
            {
                continue;
            }
            ObjectNode owned = profile.item(value.path("uid").asText());
            int[] other = size(owned.path("itemId").asText());
            if (overlaps(x, y, width, height, value.path("x").asInt(), value.path("y").asInt(), other[0], other[1]))
            {
                return false;
            }
        }
        for (JsonNode value : storage.withArray("materialStacks"))
        {
            if (value == ignoreStack)
            {
                continue;
            }
            if (overlaps(x, y, width, height, value.path("x").asInt(), value.path("y").asInt(), 1, 1))
            {
                return false;
            }
        }
        return true;
    }

    private void mergeStacks()
    {
        Map<String, Integer> totals = new LinkedHashMap<>();
        Map<String, List<ObjectNode>> positions = new LinkedHashMap<>();
        for (JsonNode value : storage.withArray("materialStacks"))
        {
            String id = value.path("materialId").asText();
            totals.merge(id, value.path("count").asInt(), Integer::sum);
            positions.computeIfAbsent(id, ignored -> new ArrayList<>()).add((ObjectNode) value);
        }
        ArrayNode merged = storage.putArray("materialStacks");
        for (Map.Entry<String, Integer> entry : totals.entrySet())
        {
            int remaining = entry.getValue();
            int position = 0;
            while (remaining > 0)
            {
                ObjectNode old = positions.get(entry.getKey()).get(position++);
                ObjectNode stack = merged.addObject();
                stack.put("materialId", entry.getKey());
                stack.put("count", Math.min(remaining, 999));
                stack.put("x", old.path("x").asInt());
                stack.put("y", old.path("y").asInt());
                remaining -= stack.path("count").asInt();
            }
        }
    }

    private void syncMaterials()
    {
        ObjectNode totals = profile.json().putObject("materials");
        for (JsonNode stack : storage.withArray("materialStacks"))
        {
            String id = stack.path("materialId").asText();
            totals.put(id, totals.path(id).asInt() + stack.path("count").asInt());
        }
    }

    private ObjectNode findItemPlacement(String uid)
    {
        for (JsonNode value : storage.withArray("items"))
        {
            if (uid.equals(value.path("uid").asText()))
            {
                return (ObjectNode) value;
            }
        }
        return null;
    }

    private int[] size(String itemId)
    {
        JsonNode value = data.item(itemId).path("size");
        return new int[] { value.path(0).asInt(1), value.path(1).asInt(1) };
    }

    private static boolean overlaps(int x, int y, int width, int height, int bx, int by, int bw, int bh)
    {
        return x < bx + bw && x + width > bx && y < by + bh && y + height > by;
    }

    private static List<ObjectNode> copies(ArrayNode source)
    {
        List<ObjectNode> result = new ArrayList<>();
        for (JsonNode value : source)
        {
            result.add((ObjectNode) value.deepCopy());
        }
        return result;
    }
}
