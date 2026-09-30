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
    ADD_CRIT(Target.SELF_ITEM),
    /** Tụ Kình: add one capped stack, keyed by source item and trigger; attack uses value, crit/dmg_reduce use pct. */
    BUFF_STACK(null),
    /** Tẩy Trừ: without status clears own timed control; with burn/poison/bleed removes value points (0 = all). */
    CLEANSE(Target.SELF),
    /** Hút Huyết: heal pct of actual shield + HP damage from the preceding DAMAGE in this effect list. */
    LIFESTEAL(Target.SELF),
    /** Cố Bản: raise maximum and current Khí Huyết by scaled value. */
    RAISE_MAX_HP(Target.SELF);

    /** Null for APPLY: depends on the status. */
    public final Target defaultTarget;

    EffectOp(Target defaultTarget) {
        this.defaultTarget = defaultTarget;
    }
}
