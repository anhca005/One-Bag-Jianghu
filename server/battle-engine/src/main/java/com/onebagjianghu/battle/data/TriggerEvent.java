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
    /**
     * Own side applied a status (optionally filtered by {@code status}). Effects executed by this
     * trigger may apply statuses, but those applications do not raise STATUS_APPLIED again.
     */
    STATUS_APPLIED,
    /** A status applied by own side reached its threshold and burst (optionally filtered by {@code status}). */
    BURST,
    /** After Cương Khí, own hero took >0 HP damage from enemy DAMAGE or status burst; from is the enemy source item. */
    DAMAGED,
    /** Immediately after own hero gains >0 HP from heal, regen, or lifesteal; from is the healing effect's source item. */
    HEALED,
    /** Immediately after own item gets SLOW/SEAL or own hero gets STUN; from is the enemy applying item. */
    CONTROLLED,
    /** After an enemy DAMAGE hit rolled Hội Tâm; from is the enemy attacking item. */
    CRIT_TAKEN,
    /** After the parry check when the enemy starts Tuyệt Kỹ; parried ultimates count; from is that enemy item. */
    ENEMY_ULTIMATE,
    /** Immediately when own Nội Lực changes from >0 to 0; from is the item whose effect caused the change, when any. */
    QI_EMPTY,
    /** Immediately when own side reduces enemy Cương Khí from >0 to 0; from is the own item that broke it. */
    SHIELD_BROKEN,
    /** After an enemy status burst damages own hero; optional status filter; from is the enemy applying item. */
    BURST_TAKEN
}
