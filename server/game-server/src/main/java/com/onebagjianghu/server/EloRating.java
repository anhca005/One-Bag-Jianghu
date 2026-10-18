package com.onebagjianghu.server;

/** Elo rating updates with K=32; ratings never fall below the 1000 starting rating (#68). */
public final class EloRating
{
    public static final int FLOOR = 1000;
    private static final int K = 32;

    private EloRating()
    {
    }

    public static int change(int rating, int opponentRating, double score)
    {
        double expected = 1.0 / (1.0 + Math.pow(10.0, (opponentRating - rating) / 400.0));
        return (int) Math.round(K * (score - expected));
    }

    /** Losing still costs points, but never below the floor, so throwing matches cannot farm a low rating. */
    public static int apply(int rating, int change)
    {
        return Math.max(FLOOR, rating + change);
    }
}
