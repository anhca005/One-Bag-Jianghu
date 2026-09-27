package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.ItemDef;
import com.onebagjianghu.battle.data.ItemType;
import com.onebagjianghu.battle.grid.Rect;

import java.util.ArrayList;
import java.util.List;

/** Mutable per-battle state of one placed item. */
final class ItemInstance {
    final int uid;
    final ItemDef def;
    final Fighter owner;
    final int tier;
    final Rect rect;
    final List<ItemInstance> adjacent = new ArrayList<>();
    /** Uses per trigger index. */
    final int[] triggerUses;

    int attack;
    int critPermille;
    final int cooldownMs;
    int progressMs;
    int hasteUntil;
    int slowUntil;
    int sealUntil;

    ItemInstance(int uid, ItemDef def, Fighter owner, int tier, Rect rect, int tierMultiplier) {
        this.uid = uid;
        this.def = def;
        this.owner = owner;
        this.tier = tier;
        this.rect = rect;
        this.triggerUses = new int[def.triggers.size()];
        this.attack = def.attack * tierMultiplier / 1000;
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
