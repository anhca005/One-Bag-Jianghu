package com.onebagjianghu.battle.sim;

import com.fasterxml.jackson.annotation.JsonInclude;

/**
 * One entry of the battle log sent to the client for replay.
 * hp / maxHp / shield / qi are the state of {@code side} after the event.
 */
@JsonInclude(JsonInclude.Include.NON_NULL)
public final class BattleEvent {
    public int t;
    public EventType type;
    public int side;
    public Integer item;
    public String kind;
    public Integer value;
    public Integer absorbed;
    /** STATUS: build-up meter after the change. */
    public Integer total;
    public Boolean crit;
    public Integer hp;
    public Integer maxHp;
    public Integer shield;
    public Integer qi;

    public BattleEvent() {
    }

    BattleEvent(int t, EventType type, int side) {
        this.t = t;
        this.type = type;
        this.side = side;
    }

    BattleEvent item(int uid) {
        this.item = uid;
        return this;
    }

    BattleEvent kind(String kind) {
        this.kind = kind;
        return this;
    }

    BattleEvent value(int value) {
        this.value = value;
        return this;
    }

    BattleEvent absorbed(int absorbed) {
        this.absorbed = absorbed;
        return this;
    }

    BattleEvent total(int total) {
        this.total = total;
        return this;
    }

    BattleEvent crit(boolean crit) {
        this.crit = crit;
        return this;
    }

    BattleEvent state(Fighter f) {
        // Overkill is not shown: the simulation may go below 0 inside a tick, the log never does.
        this.hp = Math.max(0, f.hp);
        this.maxHp = f.maxHp;
        this.shield = f.shield;
        this.qi = f.qi;
        return this;
    }

    @Override
    public String toString() {
        return t + " " + type + " side=" + side
                + (item != null ? " item=" + item : "")
                + (kind != null ? " kind=" + kind : "")
                + (value != null ? " value=" + value : "")
                + (absorbed != null ? " absorbed=" + absorbed : "")
                + (total != null ? " total=" + total : "")
                + (Boolean.TRUE.equals(crit) ? " CRIT" : "")
                + (hp != null ? " hp=" + hp + "/" + maxHp + " shield=" + shield + " qi=" + qi : "");
    }
}
