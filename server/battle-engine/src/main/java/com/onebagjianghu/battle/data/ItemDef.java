package com.onebagjianghu.battle.data;

import java.util.ArrayList;
import java.util.List;

public final class ItemDef {
    public String id;
    public String name;
    public ItemType type;
    /** Hệ binh khí (fist, sword, axe...), null for non-weapons. */
    public String weaponClass;
    /** Lowest rarity this item drops at; simulation rarity lives on each owned copy. */
    public Rarity rarity = Rarity.COMMON;
    /** [width, height] in bag cells. */
    public int[] size = {1, 1};
    /** Lực Đạo. */
    public int attack;
    /** Tốc Xuất Chiêu in seconds; weapons fire basic, relics/manuals fire active; 0 = passive. */
    public double cooldown;
    /** Hội Tâm chance, fraction. */
    public double crit;
    public List<EffectDef> basic = List.of();
    /** Relic/manual active effects fired on cooldown; empty for weapons and passive items. */
    public List<EffectDef> active = List.of();
    public UltimateDef ultimate;
    public List<TriggerDef> triggers = List.of();
    /** Đột Phá, sorted by tier (checked by {@link ItemCatalog}). */
    public List<UnlockDef> unlocks = List.of();

    /**
     * The definition as it plays at {@code tier}: base values plus every unlock with
     * {@code unlock.tier <= tier}. The result has no unlocks of its own.
     */
    public ItemDef atTier(int tier) {
        ItemDef r = copyStats();
        List<EffectDef> basicOut = new ArrayList<>(basic);
        List<EffectDef> activeOut = new ArrayList<>(active);
        List<TriggerDef> triggersOut = new ArrayList<>(triggers);
        List<EffectDef> ultEffects = ultimate == null ? new ArrayList<>() : new ArrayList<>(ultimate.effects);
        int ultCost = ultimate == null ? 0 : ultimate.cost;
        for (UnlockDef u : unlocks) {
            if (u.tier > tier) {
                continue;
            }
            r.attack += u.attack;
            r.cooldown += u.cooldown;
            r.crit += u.crit;
            ultCost += u.ultimateCost;
            basicOut.addAll(u.basic);
            activeOut.addAll(u.active);
            ultEffects.addAll(u.ultimate);
            triggersOut.addAll(u.triggers);
        }
        r.basic = List.copyOf(basicOut);
        r.active = List.copyOf(activeOut);
        r.triggers = List.copyOf(triggersOut);
        if (ultimate != null || ultCost != 0 || !ultEffects.isEmpty()) {
            r.ultimate = new UltimateDef();
            r.ultimate.cost = ultCost;
            r.ultimate.effects = List.copyOf(ultEffects);
        }
        return r;
    }

    /** Copy of this definition without unlocks; effect lists are shared (treated as immutable). */
    public ItemDef copy() {
        ItemDef r = copyStats();
        r.basic = basic;
        r.active = active;
        r.ultimate = ultimate;
        r.triggers = triggers;
        return r;
    }

    private ItemDef copyStats() {
        ItemDef r = new ItemDef();
        r.id = id;
        r.name = name;
        r.type = type;
        r.weaponClass = weaponClass;
        r.rarity = rarity;
        r.size = size;
        r.attack = attack;
        r.cooldown = cooldown;
        r.crit = crit;
        return r;
    }

    public int width() {
        return size[0];
    }

    public int height() {
        return size[1];
    }

    public int cooldownMs() {
        return (int) Math.round(cooldown * 1000);
    }

    public int critPermille() {
        return (int) Math.round(crit * 1000);
    }

    public boolean isActive() {
        return cooldown > 0;
    }
}
