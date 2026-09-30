package com.onebagjianghu.battle.data;

import java.util.List;

public final class TriggerDef {
    public TriggerEvent on;
    public TriggerSource from = TriggerSource.ANY;
    /** HP_BELOW: fraction of max HP (0.4 = 40%). */
    public double hpBelow;
    /** STATUS_APPLIED, BURST, or BURST_TAKEN: only this status; null = any. */
    public StatusType status;
    /** 0 = unlimited. */
    public int maxUses;
    public List<EffectDef> effects = List.of();

    public int hpBelowPermille() {
        return (int) Math.round(hpBelow * 1000);
    }
}
