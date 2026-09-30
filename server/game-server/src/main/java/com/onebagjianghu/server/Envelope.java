package com.onebagjianghu.server;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.databind.JsonNode;

@JsonInclude(JsonInclude.Include.NON_NULL)
public record Envelope(String type, Long id, Long replyTo, JsonNode payload) {
    public static Envelope reply(String type, Long replyTo, JsonNode payload) {
        return new Envelope(type, null, replyTo, payload);
    }
}
