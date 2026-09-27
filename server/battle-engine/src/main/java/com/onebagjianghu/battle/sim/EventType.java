package com.onebagjianghu.battle.sim;

/**
 * Event kinds in the battle log. {@code side} on an event is the side that is affected
 * (or acting, for TRIGGER), see {@link BattleEvent}.
 */
public enum EventType {
    /** One per side at t=0 with starting state. */
    START,
    /** An item activated. kind = BASIC | ULTIMATE | PASSIVE. */
    TRIGGER,
    /** Weapon damage. item = source item uid. */
    DAMAGE,
    /** Damage over time. kind = EXHAUSTION (Kiệt Lực). */
    DOT,
    /** A build-up status reached its threshold. kind = BURN | POISON | BLEED; value = damage; item = applying item. */
    BURST,
    HEAL,
    /** Cương Khí change (value may be negative for Phá Cương). */
    SHIELD,
    /** Nội Lực change. */
    QI,
    /** kind = status; item = target item uid for item statuses; value = stacks or ms; total = meter after. */
    STATUS,
    /** side = parrying side, item = the parried enemy item. */
    PARRY,
    CHARGE,
    /** kind = ATTACK | CRIT. */
    BUFF,
    MAX_HP,
    DMG_REDUCE,
    /** value = new Hóa Kình cost of {@code side}. */
    PARRY_COST,
    /** side = winner, -1 = draw. */
    END
}
