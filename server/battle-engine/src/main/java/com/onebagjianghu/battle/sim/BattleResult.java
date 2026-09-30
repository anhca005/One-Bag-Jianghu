package com.onebagjianghu.battle.sim;

import java.util.List;

/**
 * @param winner 0 or 1; -1 = draw
 * @param bags   [cols, rows] of each side's bag
 * @param items  uid → item id and bag position per side, so a replay can draw both bags
 */
public record BattleResult(
        long seed,
        int winner,
        int durationMs,
        String[] names,
        int[] finalHp,
        int[][] bags,
        List<ItemRef> items,
        List<BattleEvent> events) {

    public record ItemRef(int uid, int side, int index, String id, int x, int y, int w, int h, int tier) {
    }

    public boolean isDraw() {
        return winner < 0;
    }
}
