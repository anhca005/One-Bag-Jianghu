package com.onebagjianghu.battle.data;

import java.util.List;

/**
 * One milestone of a sect's Tâm Pháp. Everything is added on top of lower milestones and applies
 * only to weapons whose class is listed in {@link SectDef#weaponClasses}.
 */
public final class TamPhapLevelDef {
    public int level;
    /** Fraction added to Lực Đạo (0.1 = +10%), before the tier multiplier. */
    public double attackPct;
    /** Fraction removed from Tốc Xuất Chiêu (0.05 = 5% faster). */
    public double cooldownPct;
    /** Added to Hội Tâm chance. */
    public double crit;
    /** Added to the ultimate cost (weapons without an ultimate ignore it). */
    public int ultimateCost;
    /** Appended to Phổ Công. */
    public List<EffectDef> basic = List.of();
    /** Appended to Tuyệt Kỹ (weapons without an ultimate ignore it). */
    public List<EffectDef> ultimate = List.of();
    /** Appended to the weapon's triggers. */
    public List<TriggerDef> triggers = List.of();
}
