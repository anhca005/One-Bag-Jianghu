package com.onebagjianghu.battle.data;

public final class EffectDef {
    public EffectOp op;
    /** Null = default target of the op / status. */
    public Target target;
    /** DAMAGE: multiple of attack. DMG_REDUCE / ADD_CRIT: fraction (0.08 = 8%). */
    public double pct = 1.0;
    public int hits = 1;
    /** Flat amount: qi, shield, heal, shield break, max hp, attack; for DAMAGE overrides pct when > 0. */
    public int value;
    public StatusType status;
    public int stacks;
    /** Seconds: timed statuses, CHARGE. */
    public double duration;
    public boolean ignoreShield;

    public Target resolvedTarget() {
        if (target != null) {
            return target;
        }
        if (op == EffectOp.APPLY) {
            return status == null ? null : status.defaultTarget;
        }
        return op == null ? null : op.defaultTarget;
    }

    public int durationMs() {
        return (int) Math.round(duration * 1000);
    }

    public int pctPermille() {
        return (int) Math.round(pct * 1000);
    }
}
