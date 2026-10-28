#!/usr/bin/env sh
# Starts the game server (ws://localhost:8765/ws). Extra arguments go to the server, e.g. --db, --port, --dev.
# It builds game-server.jar and runs it with plain java: under "mvn exec:java" the server shares Maven's logger,
# which hides its info lines.
set -e
cd "$(dirname "$0")"
./mvnw -q -pl game-server -am package -DskipTests
exec java -Dorg.slf4j.simpleLogger.showDateTime=true \
    "-Dorg.slf4j.simpleLogger.dateTimeFormat=yyyy-MM-dd HH:mm:ss" \
    -jar game-server/target/game-server.jar "$@"
