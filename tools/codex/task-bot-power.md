# Task: Chiến Uy thật cho ghost máy ở Luận Kiếm Đài

Bạn là Codex, làm việc trong repo One Bag Jianghu.

## Lỗi
- `server/game-server/src/main/java/com/onebagjianghu/server/ArenaService.java:166` đang viết
  `int opponentPower = bot ? power : ghost.power();`.
- Nghĩa là ghost máy luôn hiện đúng Chiến Uy của người chơi. Thẻ Đối đầu và lịch sử vì thế hiện sai.

## Việc cần làm
1. Tính Chiến Uy của ghost máy từ chính đội hình của nó, cùng công thức `ProfileService.power` (đọc `power.json`:
   `levelBase`, `levelPoints`, `items`, `tierFactor`, `rarityFactor`…).
   - Tách phần tính từ danh sách món đồ (`itemId`, `tier`, `rarity`) và cấp ra một hàm dùng chung. Chiến Uy của người
     chơi và của ghost máy đều gọi hàm này, để không có hai bản công thức.
   - Đội hình ghost máy là `Loadout` lấy từ `battle-engine/samples`. Đọc `Loadout` để biết trường cấp và món đồ.
     Món không có `tier`/`rarity` thì coi là Trọng 1, phẩm `common`.
   - Kết quả của người chơi phải giữ nguyên như hiện tại, kể cả cách làm tròn.
2. Lịch sử trận (`opponent_power`) lưu đúng số mới.
3. **Test JUnit:**
   - Chiến Uy của ghost máy bằng số tính tay từ một đội hình mẫu;
   - Chiến Uy của người chơi không đổi so với trước (lấy một hồ sơ, so với số cũ).

## Đọc trước
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.

## Được phép sửa
- `server/game-server/**`.

**Cấm:** mọi lệnh git; sửa file trong `One-Bag-Jianghu/` (một task khác đang sửa client); chạy Unity; sửa
`server/data/*.db`.

## Tự kiểm tra
- Trong `server/`: `./mvnw -o test` phải qua. Sandbox cần thì thêm `-Dmaven.repo.local=../.m2/repository`.
- `py tools/lint/check_style.py <file Java đã sửa>` ở gốc repo phải sạch.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`: file đã sửa, công thức dùng chung nằm ở đâu, kết quả test.
