# Task: Online đợt 1, phía server Java (tài khoản + Luận Kiếm Đài)

Bạn là Codex, làm việc trong repo One Bag Jianghu. Viết code Java cho module `server/game-server`.

## Đọc trước
- `docs/decisions.md` mục **#68** (luật Luận Kiếm Đài, các con số) và #3, #18, #35.
- `docs/tech/protocol.md`, đặc biệt mục **"Tài khoản và Luận Kiếm Đài (#68)"**. Đây là hợp đồng với client Unity, phải
  làm đúng từng tên trường.
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Code hiện có: `server/game-server/src/main/java/com/onebagjianghu/server/*` và test của nó; engine ở
  `server/battle-engine`.

## Được phép sửa
- `server/game-server/**` (code, test, `pom.xml` của module).
- `server/README.md`: thêm mục chạy server có DB, cách chỉ đường dẫn file DB.
- `server/.gitignore` hoặc dòng thêm vào `.gitignore` gốc để bỏ qua `server/data/`.

**Cấm:** mọi lệnh git; sửa `server/battle-engine` (nếu thật sự cần thì dừng lại và ghi lý do vào báo cáo); sửa
thư mục Unity.

## Việc cần làm
1. **SQLite:** thêm `org.xerial:sqlite-jdbc:3.46.1.3` (đã có trong `~/.m2`, chạy Maven với `-o`). JDBC thuần, không
   thêm ORM. File DB mặc định `server/data/jianghu.db`, đổi được bằng tham số dòng lệnh `--db <path>` và biến môi trường
   `JIANGHU_DB`. Tạo bảng khi khởi động (`CREATE TABLE IF NOT EXISTS`), có bảng `schema_version`.
   Bảng gợi ý: `accounts`, `sessions` (token), `arena_players` (theo mùa: rating, wins, losses), `ghosts` (ghost mới nhất
   mỗi người: loadout JSON, power, rating lúc lưu, thời gian), `arena_daily` (ngày, số trận có thưởng), `arena_weekly`
   (tuần, số trận thắng, đã nhận Ngọc Tủy chưa), `seasons`.
2. **Tài khoản** (`register`, `login`, `resume`, `logout`): đúng luật trong protocol.md. PBKDF2WithHmacSHA256 120.000
   vòng, salt 16 byte, so sánh hằng thời gian. Token ngẫu nhiên `SecureRandom`, lưu **băm SHA-256** của token trong DB
   chứ không lưu token gốc. Giới hạn đăng nhập sai 5 lần/phút mỗi kết nối. `SessionState` giữ accountId sau khi
   đăng nhập.
3. **Luận Kiếm Đài** (`arena_status`, `arena_battle`, `leaderboard`): đúng #68.
   - Kiểm loadout: dùng cách chuẩn hoá đang có ở `battle_request` (ép `cols 5`, `rows 4`, cấm `hp`), nhưng **giữ
     `level` client gửi** và chỉ chặn ngoài khoảng hợp lệ (tìm mức cấp tối đa trong engine/dữ liệu; nếu không có thì
     1–60). Thêm kiểm `rarity` thuộc tập phẩm của engine, `tier` 1–9, `tamPhapLevel` hợp lệ. Lỗi → `invalid_loadout`.
   - Lưu ghost mới **trước** khi ghép trận; không ghép với ghost của chính mình.
   - Ghép trận, Elo (K = 32, khởi điểm 1000), ghost máy từ `battle-engine/samples` khi không có người, phần thưởng 10
     trận/ngày, mốc 10 thắng/tuần → 1 `chi_ton_ngoc_tuy`, mùa 28 ngày, ngày và tuần theo UTC+7. Ghost máy: tên lấy
     từ file mẫu, `bot: true`, rating = rating người chơi, không cập nhật điểm cho máy.
   - Trận và cập nhật DB của một lần khiêu chiến nằm trong **một transaction**.
   - Seed ngẫu nhiên phía server. Tách đồng hồ (`java.time.Clock`) và nguồn ngẫu nhiên ra để test được.
4. Tách code thành các lớp nhỏ, dễ đọc (ví dụ `Database`, `AccountService`, `PasswordHasher`, `ArenaService`,
   `Matchmaker`, `EloRating`, `ArenaCalendar`). `MessageHandler` chỉ điều phối. Giữ `battle_request` và `hello` như cũ.
5. **Test JUnit** (dùng DB SQLite tạm trong thư mục test hoặc `:memory:`):
   - đăng ký, đăng nhập sai/đúng, trùng tên, tên/mật khẩu sai luật, resume token, token hết hạn, rate limit;
   - Elo: thắng/thua làm điểm đổi đúng chiều và tổng gần bằng 0 giữa hai người thật;
   - ghép trận: không gặp chính mình, ưu tiên ±15% Chiến Uy, rơi về ghost máy khi trống;
   - thưởng: trận thứ 11 trong ngày không có thưởng, qua ngày mới (UTC+7) có lại; thắng trận thứ 10 trong tuần nhận
     đúng 1 Ngọc Tủy, không nhận lần hai;
   - leaderboard sắp đúng, có `self`;
   - chưa đăng nhập gọi `arena_*` → `unauthorized`.
   Test tích hợp WebSocket đang có phải vẫn qua.

## Tự kiểm tra
Trong `server/`: `./mvnw -o test` (hoặc `mvnw.cmd -o test`) phải qua hết. Nếu Maven không chạy được trong sandbox,
ghi rõ lệnh và lỗi vào báo cáo, không đoán là đã qua.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt: các file đã tạo/sửa, bảng DB, kết quả test (số test qua/fail), chỗ nào lệch khỏi
protocol.md hoặc #68 và lý do.
