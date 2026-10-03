package com.onebagjianghu.battle.data;

/** Optional, independently scaled stat line on an item. */
public final class ItemStats {
    public int hp;
    public double dmgReduce;
    public double cooldownReduction;

    public int dmgReducePermille() { return (int) Math.round(dmgReduce * 1000); }
    public int cooldownReductionPermille() { return (int) Math.round(cooldownReduction * 1000); }
}
