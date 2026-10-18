Đã hoàn thành phía Java cho tài khoản và Luận Kiếm Đài.

Các phần chính:

- Thêm SQLite JDBC và các bảng:
  `schema_version`, `accounts`, `sessions`, `seasons`, `arena_players`, `ghosts`, `arena_daily`, `arena_weekly`.
- Thêm các lớp:
  `Database`, `AccountService`, `PasswordHasher`, `ArenaService`, `ArenaCalendar`, `Matchmaker`, `EloRating`, `LoadoutNormalizer`.
- Hỗ trợ `register`, `login`, `resume`, `logout`.
- PBKDF2-SHA256 120.000 vòng, salt 16 byte; token 32 byte và chỉ lưu SHA-256.
- Giới hạn 5 lần đăng nhập sai/phút trên mỗi kết nối.
- Hỗ trợ `arena_status`, `arena_battle`, `leaderboard`.
- Ghost được lưu trước ghép trận, loại chính mình, ghép theo ±15%/±30%/không giới hạn rồi fallback bot.
- Elo K=32, mùa 28 ngày, thưởng ngày/tuần theo UTC+7.
- Trận đấu và cập nhật điểm/thưởng nằm chung transaction.
- Thêm `--db <path>` và `JIANGHU_DB`; mặc định `server/data/jianghu.db`.
- Cập nhật [server/README.md](D:/CONG%20VIEC/One-Bag-Jianghu/server/README.md) và bỏ qua `server/data/` trong `.gitignore`.
- Không sửa `battle-engine` hoặc Unity.

Kiểm tra:

- Maven offline: **80 test qua, 0 fail** — 64 test engine và 16 test game-server.
- WebSocket integration test vẫn qua.
- Style lint các file mới/sửa chính: **0 lỗi**.
- Do sandbox chặn truy cập trực tiếp JAR SQLite trong `C:\Users\ADMIN\.m2`, lần chạy xác nhận dùng bản sao cache tạm trong workspace rồi đã xóa sạch.

Không có chủ ý lệch khỏi `protocol.md` hoặc quyết định #68. Giới hạn cấp dùng phạm vi dự phòng **1–60** vì engine không công bố mức cấp tối đa riêng.