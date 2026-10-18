# Starts the game server (ws://localhost:8765/ws). Extra arguments go to the server, e.g. --db, --port.
# The battle engine is installed first because exec:java cannot run with -am (it would also run on the parent).
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
& .\mvnw.cmd -q -pl battle-engine -am install -DskipTests
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$serverArgs = $args -join " "
& .\mvnw.cmd -q -pl game-server exec:java "-Dexec.args=$serverArgs"
