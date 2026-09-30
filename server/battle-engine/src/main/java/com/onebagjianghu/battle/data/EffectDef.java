package com.onebagjianghu.battle.data;

public final class EffectDef {
    public EffectOp op;
    /** Null = default target of the op / status. */
    public Target target;
    /** DAMAGE: multiple of attack. DMG_REDUCE / ADD_CRIT: fraction (0.08 = 8%). */
    public double pct = 1.0;
    /** DAMAGE: number of hits; every hit rolls Hội Tâm and fires HIT/CRIT independently. */
    public int hits = 1;
    /** Flat amount: qi, shield, heal, shield break, max hp, attack; for DAMAGE overrides pct when > 0. */
    public int value;
    public StatusType status;
    public int stacks;
    /** Seconds: timed statuses, CHARGE. */
    public double duration;
    public boolean ignoreShield;
    /** BUFF_STACK stat: attack, crit, or dmg_reduce. */
    public String stat;
    /** BUFF_STACK: maximum applications for this source item + trigger key. */
    public int maxStacks;
    /** Ultimate-only Hậu Kình gate: resolve only when owner qi after paying cost is at least this value. */
    public Integer minQiAfter;

    public Target resolvedTarget() {
        if (target != null) {
            return target;
        }
        if (op == EffectOp.APPLY) {
            return status == null ? null : status.defaultTarget;
        }
        if (op == EffectOp.BUFF_STACK) {
            return "dmg_reduce".equals(stat) ? Target.SELF : Target.SELF_ITEM;
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
