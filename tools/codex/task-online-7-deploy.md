# Task: Đóng gói và đưa game server lên VPS (wss, sao lưu)

Bạn là Codex. Mục tiêu: người dùng thuê một VPS Linux (Ubuntu 24.04), làm theo tài liệu là có server chạy ở
`wss://<tên miền>/ws` cho bạn bè chơi thử (quyết định #70).

## Đọc trước
- `docs/decisions.md` #68, #70. `server/README.md`, `server/pom.xml`, `server/game-server/pom.xml`,
  `server/game-server/src/main/java/com/onebagjianghu/server/GameServer.java` (đọc tham số `--db`, `--port`,
  `--samples`).
- Client: `One-Bag-Jianghu/Assets/_Game/Scripts/Net/ServerConfig.cs` (địa chỉ server: PlayerPrefs, `-server`).
- `docs/tech/code-style.md`.

## Việc cần làm
1. **Gói chạy được một mình:**
   - thêm `maven-shade-plugin` (hoặc assembly) vào `server/game-server/pom.xml` để `./mvnw -pl game-server -am package`
     ra một file `game-server.jar` chạy được bằng `java -jar`;
   - đóng kèm thư mục dữ liệu cần thiết (samples, data JSON) hoặc cho chỉ đường dẫn bằng tham số.
   - Plugin thiếu trong kho Maven trên máy thì ghi lệnh tải vào báo cáo. Claude sẽ tải giúp, vì sandbox không có mạng.
2. **Tham số và cấu hình:**
   - `GameServer.main` nhận thêm `--host` (mặc định `0.0.0.0`) và `--data <thư mục dữ liệu game>`;
   - mọi tham số đều đọc được từ biến môi trường: `JIANGHU_PORT`, `JIANGHU_HOST`, `JIANGHU_DB`, `JIANGHU_SAMPLES`,
     `JIANGHU_DATA`;
   - log ra stdout có thời gian.
   - Chỉ sửa phần `main`/đọc tham số. Một task khác đang sửa phần xử lý tin nhắn trong `GameServer`, nên đừng đụng.
3. **Docker:** viết `server/deploy/`:
   - `Dockerfile`: build nhiều tầng, chạy bằng JRE 17, user không phải root;
   - `docker-compose.yml`: dịch vụ `game` cùng volume cho DB, và dịch vụ `caddy` làm reverse proxy cấp TLS tự động
     (Let's Encrypt) cho `wss://$DOMAIN/ws`;
   - `Caddyfile` và `.env.example` (`DOMAIN`, `EMAIL`);
   - giới hạn kích thước tin nhắn và số kết nối ở mức hợp lý, ghi rõ trong tài liệu.
4. **Sao lưu:**
   - script `server/deploy/backup.sh` dùng `sqlite3 .backup` (an toàn khi DB đang chạy), giữ 14 bản theo ngày;
   - hướng dẫn đặt cron;
   - script khôi phục `restore.sh`.
5. **Client:**
   - `ServerConfig` đọc địa chỉ mặc định theo thứ tự: tham số `-server`, PlayerPrefs, rồi file
     `StreamingAssets/server.txt`, và cuối cùng là `ws://localhost:8765/ws`. Có file thì bản build không cần sửa code;
   - thêm test EditMode cho thứ tự ưu tiên này;
   - **không** tạo `StreamingAssets/server.txt` thật. Chỉ ghi cách tạo trong tài liệu.
6. **Tài liệu `docs/tech/deploy.md`** (tiếng Việt), từng bước cho người không rành Linux:
   - thuê VPS, trỏ tên miền;
   - cài Docker;
   - `git clone` hoặc copy thư mục `server/`;
   - điền `.env`, `docker compose up -d`, xem log;
   - cập nhật phiên bản mới, sao lưu và khôi phục;
   - mở cổng 80/443;
   - trỏ game tới server (`server.txt`);
   - kiểm tra kết nối bằng `wss`.
   Thêm mục "Chạy thử trên máy" (không cần TLS).

## Được phép sửa/tạo
- `server/pom.xml`, `server/game-server/pom.xml`, `server/deploy/**`, `server/README.md`
- `GameServer.java` (chỉ `main` và đọc tham số)
- `One-Bag-Jianghu/Assets/_Game/Scripts/Net/ServerConfig.cs`, `Tests/EditMode/Net/ServerConfigTests.cs`
- `docs/tech/deploy.md`

**Cấm:** mọi lệnh git; sửa các file Java khác trong game-server (task hàng chờ ghép trận đang sửa); chạy Unity; tạo
file bí mật thật (khoá, mật khẩu).

## Tự kiểm tra
- `./mvnw -o -pl game-server -am package -DskipTests` ra jar.
- `java -jar … --port 18765 --db <tạm>` chạy lên được rồi tắt.
- `./mvnw -o test` qua.
- Không chạy được Docker trong sandbox thì nói rõ đã kiểm đến đâu.
- Lint các file Java/C# đã sửa bằng `py tools/lint/check_style.py`.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`.
