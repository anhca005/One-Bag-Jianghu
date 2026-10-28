#!/usr/bin/env sh
set -eu

if [ "$#" -ne 1 ]; then
    echo "Cách dùng: ./restore.sh /app/state/backups/jianghu-YYYY-MM-DD.db" >&2
    exit 2
fi

case "$1" in
    /app/state/backups/jianghu-*.db) ;;
    *)
        echo "Chỉ được khôi phục file trong /app/state/backups." >&2
        exit 2
        ;;
esac

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
cd "$SCRIPT_DIR"

BACKUP_PATH=$1
docker compose exec -T game test -f "$BACKUP_PATH"
docker compose stop game
docker compose run --rm --no-deps game \
    sqlite3 /app/state/jianghu.db ".restore '${BACKUP_PATH}'"
docker compose up -d game
echo "Đã khôi phục ${BACKUP_PATH} và khởi động lại game."
