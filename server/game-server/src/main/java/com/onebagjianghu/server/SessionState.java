package com.onebagjianghu.server;

public final class SessionState {
    private String sessionId;

    public boolean isReady() {
        return sessionId != null;
    }

    public String sessionId() {
        return sessionId;
    }

    void ready(String sessionId) {
        this.sessionId = sessionId;
    }
}
