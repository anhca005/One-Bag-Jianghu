package com.onebagjianghu.server;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Random;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ArenaRulesTest
{
    @Test
    void eloMovesInCorrectDirectionsAndIsZeroSum()
    {
        // Act
        int winnerChange = EloRating.change(1000, 1000, 1.0);
        int loserChange = EloRating.change(1000, 1000, 0.0);

        // Assert
        assertTrue(winnerChange > 0);
        assertTrue(loserChange < 0);
        assertEquals(0, winnerChange + loserChange);
    }

    @Test
    void losingCostsPointsButNeverBelowTheFloor()
    {
        // Act
        int aboveFloor = EloRating.apply(1100, EloRating.change(1100, 1100, 0.0));
        int atFloor = EloRating.apply(1000, EloRating.change(1000, 1000, 0.0));
        int nearFloor = EloRating.apply(1010, -16);

        // Assert
        assertEquals(1084, aboveFloor);
        assertEquals(1000, atFloor);
        assertEquals(1000, nearFloor);
    }

    @Test
    void matchmakerUsesRatingRangeAndReturnsNullWhenEmpty()
    {
        // Arrange
        Matchmaker matchmaker = new Matchmaker(new Random(1));
        Matchmaker.Ghost near = new Matchmaker.Ghost(2, "{}", 1100, 1000);
        Matchmaker.Ghost far = new Matchmaker.Ghost(3, "{}", 1400, 1000);

        // Act
        Matchmaker.Ghost selected = matchmaker.choose(List.of(far, near), 1000, 50);

        // Assert
        assertEquals(near, selected);
        assertNull(matchmaker.choose(List.of(), 1000, 50));
    }

    @Test
    void matchmakerIgnoresPowerAndRejectsRatingOutsideRange()
    {
        // Arrange
        Matchmaker matchmaker = new Matchmaker(new Random(0));
        List<Matchmaker.Ghost> ghosts = new java.util.ArrayList<>();
        for (int index = 0; index < 10; index++)
        {
            ghosts.add(new Matchmaker.Ghost(index, "{}", 1140, 1000 + index));
        }
        Matchmaker.Ghost closePowerButFarRating = new Matchmaker.Ghost(99, "{}", 1000, 3000);
        ghosts.add(closePowerButFarRating);

        // Act
        Matchmaker.Ghost selected = matchmaker.choose(ghosts, 1000, 50);

        // Assert
        assertTrue(selected.rating() < 3000);
    }
}
