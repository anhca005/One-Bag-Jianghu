# Server (Java)

Maven đa module, Java 17. Dùng Maven Wrapper nên không cần cài Maven (lần đầu chạy sẽ tự tải về `~/.m2`).

## Items v2

Kỳ Trân và Công Pháp có thể dùng `cooldown` + `active`; `cooldown: 0` vẫn là nội tại.
Các trường hiệu ứng mới là `stat`, `maxStacks`, `minQiAfter`; Đột Phá có thể nối thêm `active`.
Trigger mới: `damaged`, `healed`, `controlled`, `crit_taken`, `enemy_ultimate`, `qi_empty`,
`shield_broken`, `burst_taken`; nguồn mới: `enemy`. Op mới: `buff_stack`, `cleanse`,
`lifesteal`, `raise_max_hp`.

Không có lượt gọi RNG mới cố định: mỗi nhát `damage` (kể cả `hits`) vẫn gọi đúng một lần Hội Tâm,
và target ngẫu nhiên vẫn dùng lượt RNG hiện có. Các cơ chế mới chỉ thêm lượt RNG nếu dữ liệu của chính
chúng chọn target ngẫu nhiên. Vì vậy catalog cũ không đổi thứ tự RNG.

Chuẩn hóa catalog Items v2: `py tools/items/convert_items_v2.py`.

| Module | Trạng thái | Nội dung |
|---|---|---|
| `battle-engine` | 🟨 đang làm | Mô phỏng trận tất định, Java thuần, không phụ thuộc framework |

## Lệnh thường dùng

Chạy trong thư mục `server/` (Git Bash; trên PowerShell/cmd dùng `mvnw.cmd`):

```bash
./mvnw test                                   # chạy toàn bộ unit test

# Chạy 1 trận, in nhật ký tiếng Việt (thêm --verbose để thấy cả Phổ Công và nội lực)
./mvnw -q -pl battle-engine exec:java -Dexec.args="battle-engine/samples/thai_thanh.json battle-engine/samples/liet_diem.json --seed 7"

# Ghi nhật ký trận ra JSON (định dạng client sẽ phát lại)
./mvnw -q -pl battle-engine exec:java -Dexec.args="battle-engine/samples/kim_cang.json battle-engine/samples/huyen_doc.json --json battle.json"

# Tỉ lệ thắng 2 đội hình qua N seed + nguồn sát thương của từng bên
./mvnw -q -pl battle-engine exec:java -Dexec.args="battle-engine/samples/kim_cang.json battle-engine/samples/huyen_doc.json --runs 1000"

# Bảng đối đầu mọi đội hình trong thư mục (dùng để cân bằng)
./mvnw -q -pl battle-engine exec:java -Dexec.args="--matrix battle-engine/samples --runs 500"
```

Tuỳ chọn chung: `--items <file.json>` để dùng file vật phẩm khác thay cho `battle-engine/src/main/resources/data/items.json`.

> Trên PowerShell, nếu tiếng Việt bị lỗi font thì chạy `chcp 65001` trước.

## battle-engine

```
battle-engine/
├── samples/                     đội hình mẫu (Bao Phục 5x4)
└── src/main/
    ├── resources/data/items.json    dữ liệu vật phẩm
    └── java/com/onebagjianghu/battle/
        ├── data/    ItemDef, EffectDef, TriggerDef... + ItemCatalog (nạp & kiểm tra JSON)
        ├── grid/    BagGrid, Rect: đặt đồ, chồng lấn, liền kề
        ├── sim/     BattleSimulator (API), Battle (vòng lặp), Loadout, BattleEvent, BattleResult
        └── cli/     BattleCli, LogFormatter, Breakdown (công cụ dev)
```

API dùng từ các module khác:

```java
ItemCatalog catalog = ItemCatalog.loadDefault();
BattleResult r = new BattleSimulator(catalog).run(loadoutA, loadoutB, seed);
// r.winner(): 0 / 1 / -1 (hòa); r.events(): nhật ký gửi client
```

Cơ chế chi tiết: [docs/design/combat.md](../docs/design/combat.md). Cách viết vật phẩm: [docs/design/items.md](../docs/design/items.md).

## Chạy game server

Từ thư mục `server/`, chạy WebSocket game server (mặc định `ws://localhost:8765/ws`):

```bash
./mvnw -q -pl game-server -am exec:java
```

Có thể đổi cổng và thư mục đội hình mẫu:

```bash
./mvnw -q -pl game-server -am exec:java -Dexec.args="--port 9000 --samples battle-engine/samples"
```
