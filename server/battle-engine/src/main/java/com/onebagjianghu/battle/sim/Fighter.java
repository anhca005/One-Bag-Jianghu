package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.data.SectDef;
import com.onebagjianghu.battle.grid.BagGrid;
import com.onebagjianghu.battle.grid.Rect;

import java.util.ArrayList;
import java.util.List;
import java.util.HashMap;
import java.util.Map;

/** Mutable per-battle state of one side (Hiệp Khách + Bao Phục). */
final class Fighter {
    final int side;
    final String name;
    final List<ItemInstance> items = new ArrayList<>();
    int cols;
    int rows;
    Fighter enemy;

    int maxHp;
    int hp;
    int shield;
    int qi;
    int dmgReducePermille;
    int cooldownReductionPermille;
    int parryDiscount;
    int burn;
    int poison;
    int bleed;
    int regen;
    int stunUntil;
    /** Hero-target BUFF_STACK counts, separate for every source item and trigger. */
    final Map<String, Integer> buffStacks = new HashMap<>();

    private Fighter(int side, String name) {
        this.side = side;
        this.name = name;
    }

    static Fighter create(int side, Loadout loadout, ItemCatalog catalog, SectCatalog sects, BattleConfig cfg) {
        if (loadout.level < 1) {
            throw new DataException("Loadout " + loadout.name + ": level must be >= 1");
        }
        if (loadout.tamPhapLevel < 1 || loadout.tamPhapLevel > SectCatalog.MAX_LEVEL) {
            throw new DataException("Loadout " + loadout.name + ": tamPhapLevel must be 1.." + SectCatalog.MAX_LEVEL);
        }
        SectDef sect = loadout.sect == null ? null : sects.get(loadout.sect);
        Fighter f = new Fighter(side, loadout.name);
        f.cols = loadout.cols;
        f.rows = loadout.rows;
        List<Rect> rects = new ArrayList<>();
        List<ItemDef> defs = new ArrayList<>();
        for (Loadout.PlacedItem p : loadout.items) {
            ItemDef def = catalog.get(p.id);
            if (p.tier < 1 || p.tier > cfg.tierMultiplierPermille.length) {
                throw new DataException("Loadout " + loadout.name + ": " + p.id + " has invalid tier " + p.tier);
            }
            if (p.rarity == null) throw new DataException("Loadout " + loadout.name + ": " + p.id + " has invalid rarity null");
            defs.add(def);
            rects.add(new Rect(p.x, p.y, def.width(), def.height()));
        }
        List<String> errors = new BagGrid(loadout.cols, loadout.rows).validate(rects);
        if (!errors.isEmpty()) {
            throw new DataException("Loadout " + loadout.name + ": " + String.join("; ", errors));
        }
        for (int i = 0; i < defs.size(); i++) {
            int tier = loadout.items.get(i).tier;
            ItemDef def = defs.get(i).atTier(tier);
            if (sect != null) {
                def = sect.applyTo(def, loadout.tamPhapLevel);
            }
            var rarity = loadout.items.get(i).rarity;
            f.items.add(new ItemInstance(side * 100 + i, def, f, tier, rarity, rects.get(i), cfg.tierMultiplier(tier), cfg.rarityMultiplier(rarity)));
        }
        long hpBonus = 0, dmgReduce = 0, cooldownReduction = 0;
        for (ItemInstance item : f.items) {
            hpBonus += item.hpBonus;
            dmgReduce += item.baseDmgReducePermille;
            cooldownReduction += item.cooldownReductionPermille;
        }
        f.maxHp = Math.toIntExact((long) cfg.baseHp + (long) cfg.hpPerLevel * (loadout.level - 1) + hpBonus);
        f.hp = f.maxHp;
        f.dmgReducePermille = (int) Math.min(cfg.maxDmgReducePermille, dmgReduce);
        f.cooldownReductionPermille = (int) Math.min(cfg.maxCooldownReducePermille, cooldownReduction);
        for (ItemInstance item : f.items) {
            if (item.cooldownMs > 0) item.cooldownMs = Math.max(cfg.tickMs,
                    item.cooldownMs * (1000 - f.cooldownReductionPermille) / 1000);
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
