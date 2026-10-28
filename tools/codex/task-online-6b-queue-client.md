# Task: Phần client của hàng chờ ghép trận (tiếp task online-6)

Bạn là Codex. Lần trước (`tools/codex/task-online-6-matchmaking-queue.md`) bạn đã làm xong **phần server**: Claude đã
chạy test, qua hết. Phần client chưa làm được vì sandbox chặn ghi. Lần này thư mục làm việc là Unity project nên ghi
được.

Làm **mục "Client"** trong `tools/codex/task-online-6-matchmaking-queue.md` (đọc lại cả file đó cùng
`docs/tech/protocol.md` mục "Hàng chờ ghép trận (#70)"). Server đã có `arena_find`, `arena_searching`, `arena_cancel`
và gửi chủ động `replyTo: 0`. Xem `server/game-server/src/main/java/com/onebagjianghu/server/ArenaService.java` và
`MessageHandler.java` để khớp đúng tên trường.

Được phép sửa:
- `Assets/_Game/Scripts/Net/**`, `Assets/_Game/Scripts/UI/Arena/**`;
- `Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs`;
- `Assets/_Game/Tests/EditMode/Net/**`, `Assets/_Game/Tests/EditMode/UI/Arena*`.

Không sửa `ServerConfig.cs` và `ServerConfigTests.cs` (task khác vừa sửa).

**Cấm:** mọi lệnh git, chạy Unity, sửa scene và ảnh, đặt tên hàm trùng tên kiểu Unity.

Lint mọi file đã sửa bằng `py ../tools/lint/check_style.py <file>`.

Báo cáo ngắn bằng tiếng Việt ở file `-o`.
