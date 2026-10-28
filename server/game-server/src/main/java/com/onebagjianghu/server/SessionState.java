package com.onebagjianghu.server;

import java.time.Instant;
import java.util.ArrayDeque;
import java.util.Deque;
import java.util.function.Consumer;

/** Per-WebSocket readiness, authentication, token and failed-login budget. */
public final class SessionState
{
    private String sessionId;
    private Long accountId;
    private String token;
    private final Deque<Instant> failedLogins = new ArrayDeque<>();
    private Consumer<Envelope> sender = ignored -> { };

    public boolean isReady()
    {
        return sessionId != null;
    }

    public boolean isAuthenticated()
    {
        return accountId != null;
    }

    public String sessionId()
    {
        return sessionId;
    }

    public long accountId()
    {
        if (accountId == null)
        {
            throw new IllegalStateException("Not authenticated");
        }
        return accountId;
    }

    public String token()
    {
        return token;
    }

    void ready(String value)
    {
        sessionId = value;
    }

    void authenticated(long id, String value)
    {
        accountId = id;
        token = value;
        failedLogins.clear();
    }

    void logout()
    {
        accountId = null;
        token = null;
    }

    void sender(Consumer<Envelope> value)
    {
        sender = value;
    }

    void send(Envelope envelope)
    {
        sender.accept(envelope);
    }

    boolean loginAllowed(Instant now)
    {
        trimFailures(now);
        return failedLogins.size() < 5;
    }

    void loginFailed(Instant now)
    {
        trimFailures(now);
        failedLogins.addLast(now);
    }

    private void trimFailures(Instant now)
    {
        Instant cutoff = now.minusSeconds(60);
        while (!failedLogins.isEmpty() && failedLogins.peekFirst().isBefore(cutoff))
        {
            failedLogins.removeFirst();
        }
    }
}
