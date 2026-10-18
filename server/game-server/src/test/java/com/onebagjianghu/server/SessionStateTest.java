package com.onebagjianghu.server;

import org.junit.jupiter.api.Test;

import java.time.Instant;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class SessionStateTest
{
    @Test
    void fiveFailuresRateLimitOnlyCurrentMinute()
    {
        // Arrange
        SessionState session = new SessionState();
        Instant now = Instant.parse("2026-10-18T00:00:00Z");

        // Act
        for (int i = 0; i < 5; i++)
        {
            session.loginFailed(now.plusSeconds(i));
        }

        // Assert
        assertFalse(session.loginAllowed(now.plusSeconds(5)));
        assertTrue(session.loginAllowed(now.plusSeconds(65)));
    }
}
