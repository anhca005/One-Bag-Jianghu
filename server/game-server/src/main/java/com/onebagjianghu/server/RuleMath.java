package com.onebagjianghu.server;

import java.math.BigDecimal;
import java.math.RoundingMode;

/** C#-compatible rounding used by economy rules. */
public final class RuleMath
{
    private RuleMath() { }

    public static long roundHalfEven(double value)
    {
        return BigDecimal.valueOf(value).setScale(0, RoundingMode.HALF_EVEN).longValueExact();
    }

    public static long roundHalfAwayFromZero(double value)
    {
        return BigDecimal.valueOf(value).setScale(0, RoundingMode.HALF_UP).longValueExact();
    }
}
