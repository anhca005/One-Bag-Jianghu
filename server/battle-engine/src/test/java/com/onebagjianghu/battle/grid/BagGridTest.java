package com.onebagjianghu.battle.grid;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BagGridTest {

    @Test
    void edgeTouchIsAdjacentCornerIsNot() {
        Rect a = new Rect(0, 0, 1, 1);
        assertTrue(a.isAdjacent(new Rect(1, 0, 1, 1)));
        assertTrue(a.isAdjacent(new Rect(0, 1, 1, 1)));
        assertFalse(a.isAdjacent(new Rect(1, 1, 1, 1)));
        assertFalse(a.isAdjacent(new Rect(2, 0, 1, 1)));
    }

    @Test
    void largeItemsAreAdjacentAlongAnySharedEdge() {
        Rect axe = new Rect(0, 0, 2, 2);
        assertTrue(axe.isAdjacent(new Rect(2, 1, 1, 1)));
        assertTrue(axe.isAdjacent(new Rect(1, 2, 1, 3)));
        assertFalse(axe.isAdjacent(new Rect(2, 2, 1, 1)));
    }

    @Test
    void validateReportsOverlapAndOutOfBounds() {
        BagGrid bag = new BagGrid(5, 4);
        assertTrue(bag.validate(List.of(new Rect(0, 0, 1, 3), new Rect(1, 0, 2, 2))).isEmpty());
        assertEquals(1, bag.validate(List.of(new Rect(0, 0, 2, 2), new Rect(1, 1, 1, 1))).size());
        assertEquals(1, bag.validate(List.of(new Rect(4, 2, 1, 3))).size());
    }

    @Test
    void adjacencyListIsSymmetric() {
        List<List<Integer>> adj = BagGrid.adjacency(List.of(
                new Rect(0, 0, 1, 1), new Rect(1, 0, 1, 1), new Rect(3, 3, 1, 1)));
        assertEquals(List.of(1), adj.get(0));
        assertEquals(List.of(0), adj.get(1));
        assertTrue(adj.get(2).isEmpty());
    }
}
