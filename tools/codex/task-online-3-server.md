# Task: Server Luận Kiếm Đài đợt 2 (Lịch Sử, Xem lại, sửa review)

Bạn là Codex, sửa module `server/game-server`. Đợt trước bạn đã làm tài khoản và Luận Kiếm Đài
(`tools/codex/out/online-1-server.md`). Claude đã review và đã chạy lại test, 80 test qua.

## Đọc trước
- `docs/tech/protocol.md`: mục **"Lịch sử và xem lại (#69)"** (mới thêm).
- `docs/decisions.md` mục #68, #69.
- `docs/tech/code-style.md`.

## Được phép sửa
`server/game-server/**`, `server/README.md`. **Cấm:** mọi lệnh git, sửa `battle-engine`, sửa thư mục Unity.

## Việc cần làm
1. **Lịch sử và xem lại:**
   - thêm bảng `arena_battles` lưu id, account, thời gian, hai đội hình đã chuẩn hoá (JSON), tên/phái/bot của đối thủ,
     seed, thắng/thua, rating trước/sau; ghi trong cùng transaction của `arena_battle`;
   - nâng `schema_version` và tự migrate DB cũ;
   - thêm `arena_history` (tối đa 20, mới nhất trước) và `arena_replay` (chạy lại engine từ hai đội hình và seed đã
     lưu, kiểm kết quả `winner` khớp bản đã lưu, trận của người khác → `not_found`);
   - `arena_result` trả thêm `battleId`.
2. **Sửa ghép trận theo #68:**
   - lọc theo khoảng Chiến Uy (±15%, rồi ±30%, rồi không giới hạn) như đang làm;
   - nhưng trong các ghost lọt khoảng thì chọn ngẫu nhiên trong **10 ghost gần điểm (rating) nhất**, không phải gần
     Chiến Uy nhất;
   - thêm test chứng minh điều này.
3. **Đường dẫn DB:** hiện chạy test sinh ra file thật `server/game-server/data/jianghu.db`.
   - Test phải dùng DB tạm (`@TempDir`) hoặc `:memory:` và không tạo file nào trong cây mã nguồn.
   - Đường dẫn mặc định khi không có `--db`/`JIANGHU_DB` phải ra đúng `server/data/jianghu.db` dù chạy từ thư mục
     `server/` hay `server/game-server/`. Ví dụ: tìm thư mục chứa `mvnw`, hoặc ghi rõ trong README lệnh luôn truyền
     `--db`. Chọn cách đơn giản và ghi lý do.
   - Xoá thư mục `server/game-server/data/` do test cũ tạo ra.
4. **JDBC:** mọi `PreparedStatement`/`ResultSet` dùng try-with-resources.
5. **Trận hoà:** hiện `score 0.5` có thể bị tính là thua. Hoà thì không cộng thắng lẫn thua, Elo tính 0.5, thưởng
   tính như thua. Thêm test.
6. **Lint:** `MessageHandlerTest.java` và `GameServerIntegrationTest.java` đang có 31 lỗi style có từ trước. Sửa cho
   sạch.

## Tự kiểm tra
- `./mvnw -o test` trong `server/` qua hết. Sandbox chặn đọc `~/.m2` thì làm như lần trước: copy tạm rồi xoá.
- `py tools/lint/check_style.py` chạy trên mọi file Java của game-server: 0 lỗi.
- Sau khi test xong, `server/game-server/data` không tồn tại.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt: file đã sửa, migrate, kết quả test và lint.
