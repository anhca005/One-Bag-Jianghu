package com.onebagjianghu.battle.data;

/**
 * Trạng thái. Stack statuses live on the hero; timed statuses use {@code duration}.
 * BURN / POISON / BLEED are build-up meters that burst at a threshold (see BattleConfig.Burst).
 */
public enum StatusType {
    /** Liệt Hỏa: tích điểm, đủ ngưỡng thì bộc phát (Cương Khí đỡ được). */
    BURN(Target.ENEMY, false),
    /** Kịch Độc: tích điểm, đủ ngưỡng thì bộc phát xuyên Cương Khí. */
    POISON(Target.ENEMY, false),
    /** Xuất Huyết: tích điểm, đủ ngưỡng thì bộc phát xuyên Cương Khí + % khí huyết tối đa. */
    BLEED(Target.ENEMY, false),
    /** Điều Tức: hồi máu mỗi giây. */
    REGEN(Target.SELF, false),
    /** Định Thân: dừng mọi vật phẩm. */
    STUN(Target.ENEMY, true),
    /** Trì Trệ: tốc xuất chiêu còn 50%. */
    SLOW(Target.ENEMY_RANDOM_ITEM, true),
    /** Điểm Huyệt: dừng thời gian hồi của vật phẩm. */
    SEAL(Target.ENEMY_RANDOM_ITEM, true),
    /** Khinh Công: tốc xuất chiêu x2. */
    HASTE(Target.SELF_ITEM, true);

    public final Target defaultTarget;
    public final boolean timed;

    StatusType(Target defaultTarget, boolean timed) {
        this.defaultTarget = defaultTarget;
        this.timed = timed;
    }

    public TargetKind kind() {
        return defaultTarget.kind;
    }
}
