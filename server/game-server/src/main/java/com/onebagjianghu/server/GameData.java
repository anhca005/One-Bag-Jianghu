package com.onebagjianghu.server;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/** Immutable shared game data used by profile rules. */
public final class GameData
{
    private static final List<String> REQUIRED = List.of("materials.json", "upgrade_costs.json", "merge.json",
            "starter.json", "levels.json", "power.json", "items.json", "sects.json", "shop.json", "quests.json",
            "rewards.json", "intro.json", "story.json", "tutorial.json", "Pve/chapters.json", "Pve/stamina.json");
    private final ObjectMapper mapper = ItemCatalog.mapper();
    private final Path directory;
    private final Map<String, JsonNode> roots = new HashMap<>();
    private final Map<String, JsonNode> items = new HashMap<>();
    private final Map<String, JsonNode> materials = new HashMap<>();

    public GameData(Path directory)
    {
        this.directory = directory.toAbsolutePath().normalize();
        for (String file : REQUIRED)
        {
            Path path = this.directory.resolve(file);
            if (!Files.isRegularFile(path))
            {
                throw new IllegalStateException("Missing required game data file: " + path);
            }
            try
            {
                roots.put(file, mapper.readTree(path.toFile()));
            }
            catch (Exception exception)
            {
                throw new IllegalStateException("Cannot read game data file " + path + ": " + exception.getMessage(),
                        exception);
            }
        }
        index((ArrayNode) roots.get("items.json"), items);
        index((ArrayNode) roots.get("materials.json"), materials);
    }

    public Path directory()
    {
        return directory;
    }

    public JsonNode root(String file)
    {
        return roots.get(file);
    }

    public JsonNode item(String id)
    {
        JsonNode value = items.get(id);
        if (value == null)
        {
            throw new ServiceException("rejected", "Không tìm thấy vật phẩm.");
        }
        return value;
    }

    public JsonNode material(String id)
    {
        return materials.get(id);
    }

    public ItemCatalog itemCatalog()
    {
        return ItemCatalog.load(directory.resolve("items.json"));
    }

    public SectCatalog sectCatalog()
    {
        return SectCatalog.load(directory.resolve("sects.json"));
    }

    private static void index(ArrayNode source, Map<String, JsonNode> target)
    {
        for (JsonNode value : source)
        {
            target.put(value.path("id").asText(), value);
        }
    }
}
