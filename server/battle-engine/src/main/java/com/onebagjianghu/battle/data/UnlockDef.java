package com.onebagjianghu.battle.data;

import java.util.List;

/**
 * Đột Phá: bonus an item gains once it reaches {@code tier}. Every field is added on top of the
 * base definition (and of lower unlocks); see {@link ItemDef#atTier(int)}.
 */
public final class UnlockDef {
    public int tier;
    /** Added to Lực Đạo (before the tier multiplier). */
    public int attack;
    /** Added to Tốc Xuất Chiêu, seconds (negative = faster). */
    public double cooldown;
    /** Added to Hội Tâm chance. */
    public double crit;
    /** Added to the ultimate cost; for an item without an ultimate, this is the new ultimate's cost. */
    public int ultimateCost;
    /** Appended to Phổ Công. */
    public List<EffectDef> basic = List.of();
    /** Appended to a relic/manual's active cooldown effects. */
    public List<EffectDef> active = List.of();
    /** Appended to Tuyệt Kỹ. */
    public List<EffectDef> ultimate = List.of();
    /** Appended to triggers. */
    public List<TriggerDef> triggers = List.of();
}
