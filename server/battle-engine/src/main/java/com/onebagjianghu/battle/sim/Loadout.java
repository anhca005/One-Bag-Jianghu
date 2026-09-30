package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.Rarity;

import java.io.IOException;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;

/** Một bên tham chiến: Hiệp Khách + Bao Phục đã xếp đồ. */
public final class Loadout {
    public String name = "?";
    /** Khí Huyết tối đa của Hiệp Khách. */
    public int hp;
    public int cols = 5;
    public int rows = 4;
    /** Môn phái của Hiệp Khách (id trong sects.json); null = không có Tâm Pháp. */
    public String sect;
    /** Cấp Tâm Pháp (1..9). */
    public int tamPhapLevel = 1;
    public List<PlacedItem> items = new ArrayList<>();

    public static final class PlacedItem {
        public String id;
        public int x;
        public int y;
        /** Trọng (1..9). */
        public int tier = 1;
        public Rarity rarity = Rarity.COMMON;

        public PlacedItem() {
        }

        public PlacedItem(String id, int x, int y) {
            this.id = id;
            this.x = x;
            this.y = y;
        }
    }

    public Loadout() {
    }

    public Loadout(String name, int hp, int cols, int rows) {
        this.name = name;
        this.hp = hp;
        this.cols = cols;
        this.rows = rows;
    }

    public Loadout add(String id, int x, int y) {
        items.add(new PlacedItem(id, x, y));
        return this;
    }

    public Loadout add(String id, int x, int y, int tier) {
        PlacedItem p = new PlacedItem(id, x, y);
        p.tier = tier;
        items.add(p);
        return this;
    }

    public static Loadout load(Path path) {
        try {
            return ItemCatalog.mapper().readValue(path.toFile(), Loadout.class);
        } catch (IOException e) {
            throw new DataException("Cannot read loadout " + path + ": " + e.getMessage(), e);
        }
    }
}
