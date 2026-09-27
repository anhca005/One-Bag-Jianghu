package com.onebagjianghu.battle.data;

public enum TriggerEvent {
    BATTLE_START,
    /** An own item hit the enemy with DAMAGE. */
    HIT,
    /** An own DAMAGE hit was a Hội Tâm. */
    CRIT,
    /** An own Tuyệt Kỹ resolved (not parried). */
    ULTIMATE,
    /** Own side parried (Hóa Kình) an enemy Tuyệt Kỹ. */
    PARRY_SUCCESS,
    /** Own Tuyệt Kỹ was parried. */
    PARRIED,
    /** Own HP dropped below {@code hpBelow}. */
    HP_BELOW,
    /** Own side applied a status (optionally filtered by {@code status}). */
    STATUS_APPLIED,
    /** A status applied by own side reached its threshold and burst (optionally filtered by {@code status}). */
    BURST
}
