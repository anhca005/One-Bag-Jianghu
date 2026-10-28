Đã hoàn tất đóng gói và cấu hình triển khai VPS.

- Fat JAR chạy độc lập và 89 test Java đều qua.
- Smoke test server tại cổng 18766 thành công.
- Thêm Docker, Caddy TLS/WSS, backup/restore và tài liệu tiếng Việt.
- Client hỗ trợ `server.txt` theo đúng thứ tự ưu tiên, kèm 4 test EditMode.
- Lint Java/C#: 0 lỗi.
- `docker compose config` hợp lệ; chưa build container vì sandbox không cho truy cập Docker Buildx.
- Không chạy Unity theo yêu cầu.

Báo cáo ngắn: [-o](</D:/CONG VIEC/One-Bag-Jianghu/-o>)  
Hướng dẫn triển khai: [deploy.md](</D:/CONG VIEC/One-Bag-Jianghu/docs/tech/deploy.md>)Đã hoàn tất đóng gói và triển khai server:

- Fat JAR `server/game-server/target/game-server.jar` chạy độc lập, chứa thư viện và JSON mặc định.
- Thêm `--host`, `--data` và đủ biến `JIANGHU_*`; log stdout có thời gian.
- Thêm Docker multi-stage, Caddy TLS/WSS, volume DB, giới hạn tài nguyên/kết nối, backup 14 ngày và restore.
- Client ưu tiên `-server` > PlayerPrefs > `StreamingAssets/server.txt` > localhost; thêm 4 test EditMode.
- Viết hướng dẫn VPS tiếng Việt tại `docs/tech/deploy.md` và cập nhật `server/README.md`.

Đã kiểm:

- `mvnw -o -pl game-server -am package -DskipTests`: PASS.
- Chạy JAR tại `127.0.0.1:18766` với DB tạm: PASS; log có timestamp.
- `mvnw -o test`: PASS, 89 test Java.
- Style Java/C#: PASS, 0 lỗi. Hai script shell qua `sh -n`.
- `docker compose config`: PASS. Không build container được vì sandbox từ chối truy cập
  `C:\Users\ADMIN\.docker\buildx\instances`; chưa chạy daemon/container thật.
- Không chạy Unity theo yêu cầu; test EditMode đã thêm nhưng chưa thực thi.

Shade plugin 3.6.0 đã có trong cache Maven nên không cần tải. Nếu máy khác chạy offline mà thiếu, tải trước bằng:
`./mvnw dependency:get -Dartifact=org.apache.maven.plugins:maven-shade-plugin:3.6.0`.
