package com.onebagjianghu.server;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Random;

/** Filters by widening power bands, then samples among the ten nearest ratings. */
public final class Matchmaker
{
    public record Ghost(long accountId, String loadoutJson, int power, int rating) { }

    private final Random random;

    public Matchmaker(Random random)
    {
        this.random = random;
    }

    public Ghost choose(List<Ghost> ghosts, int power, int rating)
    {
        for (double band : new double[] { 0.15, 0.30, Double.POSITIVE_INFINITY })
        {
            List<Ghost> candidates = new ArrayList<>();
            for (Ghost ghost : ghosts)
            {
                if (band == Double.POSITIVE_INFINITY || Math.abs(ghost.power() - power) <= power * band)
                {
                    candidates.add(ghost);
                }
            }
            candidates.sort(Comparator.comparingInt(ghost -> Math.abs(ghost.rating() - rating)));
            if (!candidates.isEmpty())
            {
                int bound = Math.min(10, candidates.size());
                synchronized (random)
                {
                    return candidates.get(random.nextInt(bound));
                }
            }
        }
        return null;
    }
}
