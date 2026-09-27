package com.onebagjianghu.battle.data;

public enum Target {
    /** Own hero. */
    SELF(TargetKind.FIGHTER),
    /** Enemy hero. */
    ENEMY(TargetKind.FIGHTER),

    /** The item that owns the effect. */
    SELF_ITEM(TargetKind.ITEM),
    /** The item that raised the event (e.g. the adjacent weapon that crit). Falls back to SELF_ITEM. */
    EVENT_SOURCE(TargetKind.ITEM),
    SELF_ALL_ITEMS(TargetKind.ITEM),
    SELF_OTHER_ITEMS(TargetKind.ITEM),
    SELF_ALL_WEAPONS(TargetKind.ITEM),
    ADJACENT_ITEMS(TargetKind.ITEM),
    ADJACENT_WEAPONS(TargetKind.ITEM),
    ENEMY_RANDOM_ITEM(TargetKind.ITEM),
    ENEMY_RANDOM_WEAPON(TargetKind.ITEM),
    ENEMY_ALL_ITEMS(TargetKind.ITEM);

    public final TargetKind kind;

    Target(TargetKind kind) {
        this.kind = kind;
    }
}
