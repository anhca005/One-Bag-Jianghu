# Server (Java)

Maven đa module, Java 17. Dùng Maven Wrapper nên không cần cài Maven (lần đầu chạy sẽ tự tải về `~/.m2`).

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
