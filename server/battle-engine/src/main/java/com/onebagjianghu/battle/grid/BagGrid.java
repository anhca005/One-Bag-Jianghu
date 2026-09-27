package com.onebagjianghu.battle.grid;

import java.util.ArrayList;
import java.util.List;

/** Bao Phục: checks placement and computes adjacency. Rotation is not supported yet. */
public final class BagGrid {
    private final int cols;
    private final int rows;

    public BagGrid(int cols, int rows) {
        if (cols < 1 || rows < 1) {
            throw new IllegalArgumentException("Bag must be at least 1x1");
        }
        this.cols = cols;
        this.rows = rows;
    }

    public int cols() {
        return cols;
    }

    public int rows() {
        return rows;
    }

    /** Returns a list of problems; empty means the placement is valid. */
    public List<String> validate(List<Rect> rects) {
        List<String> errors = new ArrayList<>();
        for (int i = 0; i < rects.size(); i++) {
            Rect r = rects.get(i);
            if (!r.fitsIn(cols, rows)) {
                errors.add("item #" + i + " at " + r + " is outside the " + cols + "x" + rows + " bag");
            }
            for (int j = 0; j < i; j++) {
                if (r.overlaps(rects.get(j))) {
                    errors.add("item #" + i + " overlaps item #" + j);
                }
            }
        }
        return errors;
    }

    /** adjacency.get(i) = indexes of items sharing an edge with item i. */
    public static List<List<Integer>> adjacency(List<Rect> rects) {
        List<List<Integer>> result = new ArrayList<>();
        for (int i = 0; i < rects.size(); i++) {
            List<Integer> neighbours = new ArrayList<>();
            for (int j = 0; j < rects.size(); j++) {
                if (i != j && rects.get(i).isAdjacent(rects.get(j))) {
                    neighbours.add(j);
                }
            }
            result.add(neighbours);
        }
        return result;
    }
}
