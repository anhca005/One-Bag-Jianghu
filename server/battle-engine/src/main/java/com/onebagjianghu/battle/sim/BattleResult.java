package com.onebagjianghu.battle.sim;

import java.util.List;

/**
 * @param winner 0 or 1; -1 = draw
 * @param items  uid → item id per side, so a replay can map events to bag slots
 */
public record BattleResult(
        long seed,
        int winner,
        int durationMs,
        String[] names,
        int[] finalHp,
        List<ItemRef> items,
        List<BattleEvent> events) {

    public record ItemRef(int uid, int side, int index, String id) {
    }

    public boolean isDraw() {
        return winner < 0;
    }
}
