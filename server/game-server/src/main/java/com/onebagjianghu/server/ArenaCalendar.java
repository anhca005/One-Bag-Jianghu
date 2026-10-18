package com.onebagjianghu.server;

import java.time.Clock;
import java.time.DayOfWeek;
import java.time.LocalDate;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.temporal.TemporalAdjusters;

/** UTC+7 day, week and 28-day season boundaries. */
public final class ArenaCalendar
{
    public static final ZoneId ZONE = ZoneId.of("Asia/Ho_Chi_Minh");
    private final Clock clock;

    public ArenaCalendar(Clock clock)
    {
        this.clock = clock;
    }

    public LocalDate day()
    {
        return LocalDate.now(clock.withZone(ZONE));
    }

    public LocalDate week()
    {
        return day().with(TemporalAdjusters.previousOrSame(DayOfWeek.MONDAY));
    }

    public ZonedDateTime now()
    {
        return ZonedDateTime.now(clock.withZone(ZONE));
    }
}
