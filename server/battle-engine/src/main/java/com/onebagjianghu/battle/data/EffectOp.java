package com.onebagjianghu.battle.data;

public enum EffectOp {
    /** Sát thương = Lực Đạo × pct (hoặc value cố định), lặp {@code hits} lần. */
    DAMAGE(Target.ENEMY),
    GAIN_QI(Target.SELF),
    DRAIN_QI(Target.ENEMY),
    /** Cộng Cương Khí. */
    SHIELD(Target.SELF),
    /** Liệu Thương. */
    HEAL(Target.SELF),
    /** Phá Cương: phá ngay {@code value} Cương Khí. */
    SHIELD_BREAK(Target.ENEMY),
    /** Hộ Thân: cộng {@code pct} giảm sát thương binh khí. */
    DMG_REDUCE(Target.SELF),
    /** Nội Thương: giảm khí huyết tối đa. */
    REDUCE_MAX_HP(Target.ENEMY),
    /** Hóa Kình của bên này tốn ít hơn {@code value} nội lực. */
    PARRY_DISCOUNT(Target.SELF),
    /** Gây trạng thái {@code status}; target mặc định theo trạng thái. */
    APPLY(null),
    /** Vận Khí: đẩy thanh hồi chiêu thêm {@code duration} giây. */
    CHARGE(Target.SELF_ITEM),
    ADD_ATTACK(Target.SELF_ITEM),
    /** Cộng {@code pct} tỉ lệ Hội Tâm. */
    ADD_CRIT(Target.SELF_ITEM);

    /** Null for APPLY: depends on the status. */
    public final Target defaultTarget;

    EffectOp(Target defaultTarget) {
        this.defaultTarget = defaultTarget;
    }
}
