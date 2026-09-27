package com.onebagjianghu.battle.data;

import java.util.List;

public final class ItemDef {
    public String id;
    public String name;
    public ItemType type;
    /** Hệ binh khí (fist, sword, axe...), null for non-weapons. */
    public String weaponClass;
    public String sect;
    public Rarity rarity;
    /** [width, height] in bag cells. */
    public int[] size = {1, 1};
    /** Lực Đạo. */
    public int attack;
    /** Tốc Xuất Chiêu in seconds; 0 = passive item. */
    public double cooldown;
    /** Hội Tâm chance, fraction. */
    public double crit;
    public List<EffectDef> basic = List.of();
    public UltimateDef ultimate;
    public List<TriggerDef> triggers = List.of();

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
