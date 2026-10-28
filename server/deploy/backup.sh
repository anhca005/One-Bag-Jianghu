#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
cd "$SCRIPT_DIR"

STAMP=$(date +%Y-%m-%d)
TARGET="/app/state/backups/jianghu-${STAMP}.db"
docker compose exec -T game sqlite3 /app/state/jianghu.db ".backup '${TARGET}'"

docker compose exec -T game sh -c \
    'ls -1t /app/state/backups/jianghu-*.db 2>/dev/null | tail -n +15 | xargs -r rm --'
echo "Đã sao lưu: ${TARGET}"
