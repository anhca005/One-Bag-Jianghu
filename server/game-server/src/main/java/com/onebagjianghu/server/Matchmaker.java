package com.onebagjianghu.server;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

/** Selects a random real ghost inside the current rating range. */
public final class Matchmaker
{
    public record Ghost(long accountId, String loadoutJson, int power, int rating) { }

    private final Random random;

    public Matchmaker(Random random)
    {
        this.random = random;
    }

    public Ghost choose(List<Ghost> ghosts, int rating, int range)
    {
        List<Ghost> candidates = new ArrayList<>();
        for (Ghost ghost : ghosts)
        {
            if (Math.abs(ghost.rating() - rating) <= range)
            {
                candidates.add(ghost);
            }
        }
        if (!candidates.isEmpty())
        {
            synchronized (random)
            {
                return candidates.get(random.nextInt(candidates.size()));
            }
        }
        return null;
    }
}
