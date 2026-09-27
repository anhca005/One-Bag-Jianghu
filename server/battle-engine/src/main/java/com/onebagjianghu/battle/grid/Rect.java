package com.onebagjianghu.battle.grid;

/** Axis-aligned cell rectangle in the bag grid. (x, y) is the top-left cell. */
public record Rect(int x, int y, int w, int h) {

    public int right() {
        return x + w;
    }

    public int bottom() {
        return y + h;
    }

    public boolean overlaps(Rect o) {
        return x < o.right() && o.x < right() && y < o.bottom() && o.y < bottom();
    }

    /** True when the two rectangles share an edge (corners do not count). */
    public boolean isAdjacent(Rect o) {
        boolean rowsOverlap = y < o.bottom() && o.y < bottom();
        boolean colsOverlap = x < o.right() && o.x < right();
        boolean touchX = right() == o.x || o.right() == x;
        boolean touchY = bottom() == o.y || o.bottom() == y;
        return (touchX && rowsOverlap) || (touchY && colsOverlap);
    }

    public boolean fitsIn(int cols, int rows) {
        return x >= 0 && y >= 0 && right() <= cols && bottom() <= rows;
    }
}
