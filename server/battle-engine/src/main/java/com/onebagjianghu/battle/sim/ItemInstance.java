package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.data.ItemType;
import com.onebagjianghu.battle.grid.Rect;

import java.util.ArrayList;
import java.util.List;
import java.util.HashMap;
import java.util.Map;

/** Mutable per-battle state of one placed item. */
final class ItemInstance {
    final int uid;
    final ItemDef def;
    final Fighter owner;
    final int tier;
    final com.onebagjianghu.battle.data.Rarity rarity;
    final Rect rect;
    final List<ItemInstance> adjacent = new ArrayList<>();
    /** Uses per trigger index. */
    final int[] triggerUses;
    /** BUFF_STACK counts keyed by source uid + trigger identity + effect index/stat. */
    final Map<String, Integer> buffStacks = new HashMap<>();

    int attack;
    int critPermille;
    final int cooldownMs;
    int progressMs;
    int hasteUntil;
    int slowUntil;
    int sealUntil;

    ItemInstance(int uid, ItemDef def, Fighter owner, int tier, com.onebagjianghu.battle.data.Rarity rarity, Rect rect, int tierMultiplier, int rarityMultiplier) {
        this.uid = uid;
        this.def = def;
        this.owner = owner;
        this.tier = tier;
        this.rarity = rarity;
        this.rect = rect;
        this.triggerUses = new int[def.triggers.size()];
        // Parity contract: apply tier, truncate, then rarity, truncate.
        this.attack = def.attack * tierMultiplier / 1000 * rarityMultiplier / 1000;
        this.critPermille = def.critPermille();
        this.cooldownMs = def.cooldownMs();
    }

    boolean isActive() {
        return cooldownMs > 0;
    }

    boolean isWeapon() {
        return def.type == ItemType.WEAPON;
    }
}
