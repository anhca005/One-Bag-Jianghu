package com.onebagjianghu.battle.data;

/** Which item caused the event, relative to the listening item. */
public enum TriggerSource {
    ANY,
    SELF,
    ADJACENT,
    /** The event source is an item owned by the enemy side. */
    ENEMY
}
