#!/usr/bin/env sh
# Starts the game server (ws://localhost:8765/ws). Extra arguments go to the server, e.g. --db, --port.
# The battle engine is installed first because exec:java cannot run with -am (it would also run on the parent).
set -e
cd "$(dirname "$0")"
./mvnw -q -pl battle-engine -am install -DskipTests
./mvnw -q -pl game-server exec:java -Dexec.args="$*"
