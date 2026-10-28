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

```powershell
./run-server.ps1          # PowerShell
./run-server.sh           # Git Bash
```

Script build `game-server.jar` (kèm battle-engine) rồi chạy bằng `java -jar`. Log in ra console, có giờ:
kết nối, ngắt kết nối, và mỗi yêu cầu kèm kết quả (`account 1 pve_battle -> pve_battle_ok`). Không chạy bằng
`mvn exec:java`, vì khi đó server dùng chung bộ ghi log của Maven và bị ẩn hết log thường. Tham số thêm được
chuyển cho server, ví dụ `./run-server.ps1 --port 8765 --db data/demo.db --dev`.

Cơ sở dữ liệu SQLite mặc định nằm ở `server/data/jianghu.db`; schema được tạo khi server khởi động.
Server tìm thư mục chứa `mvnw` để neo đường dẫn mặc định, nên chạy từ `server/` hay `server/game-server/`
đều dùng đúng cùng một file mà không phụ thuộc thư mục hiện tại.
Có thể đổi file bằng `--db` hoặc biến môi trường `JIANGHU_DB`, đồng thời đổi cổng và thư mục đội hình mẫu:

```bash
./run-server.sh --port 9000 --samples battle-engine/samples --db data/dev.db
```

PowerShell dùng biến môi trường: `$env:JIANGHU_DB = "data/dev.db"`.

## Đóng gói và triển khai

Tạo một fat JAR tự chứa thư viện và dữ liệu JSON mặc định:

```bash
./mvnw -pl game-server -am package
java -jar game-server/target/game-server.jar --port 8765 --db data/jianghu.db \
  --samples battle-engine/samples
```

Các tham số là `--host`, `--port`, `--db`, `--samples`, `--data`, `--dev`. Biến môi trường tương ứng là
`JIANGHU_HOST`, `JIANGHU_PORT`, `JIANGHU_DB`, `JIANGHU_SAMPLES`, `JIANGHU_DATA`. Nếu không truyền `--data`,
server dùng mặc định `../One-Bag-Jianghu/Assets/_Game/Resources/Data`. Thư mục dữ liệu phải có `materials`,
`upgrade_costs`, `merge`, `starter`, `levels`, `power`, `items` và `sects`; thiếu file nào server dừng với đường dẫn rõ.
Cheat chỉ được mở bằng `--dev` hoặc `JIANGHU_DEV=1`.

Hướng dẫn đưa lên VPS bằng Docker, TLS và sao lưu: [docs/tech/deploy.md](../docs/tech/deploy.md).
