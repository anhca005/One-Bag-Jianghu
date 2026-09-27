package com.onebagjianghu.battle.sim;

/**
 * SplitMix64. Chosen over java.util.Random so the exact sequence is easy to port
 * (e.g. to C# for an offline preview) and never changes between JDK versions.
 */
public final class Rng {
    private long state;

    public Rng(long seed) {
        this.state = seed;
    }

    public long nextLong() {
        long z = (state += 0x9E3779B97F4A7C15L);
        z = (z ^ (z >>> 30)) * 0xBF58476D1CE4E5B9L;
        z = (z ^ (z >>> 27)) * 0x94D049BB133111EBL;
        return z ^ (z >>> 31);
    }

    /** Uniform-enough int in [0, bound). */
    public int nextInt(int bound) {
        if (bound <= 0) {
            throw new IllegalArgumentException("bound must be > 0");
        }
        return (int) Long.remainderUnsigned(nextLong(), bound);
    }

    /** True with probability permille / 1000. */
    public boolean chance(int permille) {
        if (permille <= 0) return false;
        if (permille >= 1000) return true;
        return nextInt(1000) < permille;
    }
}
