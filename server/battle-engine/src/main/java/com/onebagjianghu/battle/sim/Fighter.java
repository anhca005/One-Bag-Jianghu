package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.grid.BagGrid;
import com.onebagjianghu.battle.grid.Rect;

import java.util.ArrayList;
import java.util.List;

/** Mutable per-battle state of one side (Hiệp Khách + Bao Phục). */
final class Fighter {
    final int side;
    final String name;
    final List<ItemInstance> items = new ArrayList<>();
    Fighter enemy;

    int maxHp;
    int hp;
    int shield;
    int qi;
    int dmgReducePermille;
    int parryDiscount;
    int burn;
    int poison;
    int bleed;
    int regen;
    int stunUntil;

    private Fighter(int side, String name, int hp) {
        this.side = side;
        this.name = name;
        this.maxHp = hp;
        this.hp = hp;
    }

    static Fighter create(int side, Loadout loadout, ItemCatalog catalog, BattleConfig cfg) {
        if (loadout.hp <= 0) {
            throw new DataException("Loadout " + loadout.name + ": hp must be > 0");
        }
        Fighter f = new Fighter(side, loadout.name, loadout.hp);
        List<Rect> rects = new ArrayList<>();
        List<ItemDef> defs = new ArrayList<>();
        for (Loadout.PlacedItem p : loadout.items) {
            ItemDef def = catalog.get(p.id);
            if (p.tier < 1 || p.tier > cfg.tierMultiplierPermille.length) {
                throw new DataException("Loadout " + loadout.name + ": " + p.id + " has invalid tier " + p.tier);
            }
            defs.add(def);
            rects.add(new Rect(p.x, p.y, def.width(), def.height()));
        }
        List<String> errors = new BagGrid(loadout.cols, loadout.rows).validate(rects);
        if (!errors.isEmpty()) {
            throw new DataException("Loadout " + loadout.name + ": " + String.join("; ", errors));
        }
        for (int i = 0; i < defs.size(); i++) {
            int tier = loadout.items.get(i).tier;
            f.items.add(new ItemInstance(side * 100 + i, defs.get(i), f, tier, rects.get(i), cfg.tierMultiplier(tier)));
        }
        List<List<Integer>> adjacency = BagGrid.adjacency(rects);
        for (int i = 0; i < f.items.size(); i++) {
            for (int j : adjacency.get(i)) {
                f.items.get(i).adjacent.add(f.items.get(j));
            }
        }
        return f;
    }
}
