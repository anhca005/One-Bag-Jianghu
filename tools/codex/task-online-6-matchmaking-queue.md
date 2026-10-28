# Task: Hàng chờ ghép trận Luận Kiếm (±50 điểm, nới dần) cho server và client

Bạn là Codex, làm việc trong repo One Bag Jianghu.

## Đọc trước
- `docs/decisions.md` #70 (mục Ghép trận), #68, #69.
- `docs/tech/protocol.md`, mục **"Hàng chờ ghép trận (#70)"**. Đây là hợp đồng, làm đúng tên trường.
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Server: `server/game-server/src/main/java/com/onebagjianghu/server/` (`ArenaService`, `Matchmaker`, `MessageHandler`,
  `GameServer`, `SessionState`).
- Client: `One-Bag-Jianghu/Assets/_Game/Scripts/Net/` (`NetworkService`, `OnlineModels`),
  `Scripts/UI/Arena/ArenaLobbyPopup.cs` (lớp "Đang tìm đối thủ", thẻ Đối đầu),
  `Editor/Building/OnlinePrefabBuilder.cs`.

## Server
1. Bỏ `arena_battle` và lọc Chiến Uy. Thêm `arena_find` và `arena_cancel`, cùng hàng chờ trong bộ nhớ: một lượt tìm cho
   mỗi tài khoản, gắn với kết nối.
2. Khoảng tìm = 50 + 50 × số lần đủ 2 phút đã chờ. Có một bộ hẹn giờ kiểm hàng chờ mỗi 5 giây. Server gửi
   `arena_searching` khi nới khoảng và `arena_result` khi ghép được (`replyTo: 0`).
   - Cần cách để `MessageHandler`/`ArenaService` **chủ động gửi** tin cho đúng kết nối: thêm vào `SessionState` một
     hàm gửi, `GameServer` cài đặt hàm đó.
   - Trận và ghi DB vẫn nằm trong một transaction như `battle()` hiện tại. Thêm `searchId` vào kết quả.
3. Ghost máy chỉ dùng khi cả mùa chưa có ghost người thật nào khác ngoài người đang tìm.
4. Mất kết nối thì huỷ lượt tìm. Hai người cùng lúc ghép trúng nhau thì vẫn đúng: mỗi lượt tìm đánh với ghost của
   người kia, kết quả độc lập; điểm của bên ghost cập nhật như hiện tại.
5. Tách đồng hồ (`Clock`) và bộ hẹn giờ ra để test không phải chờ 2 phút thật.
6. **Test JUnit:**
   - có ghost trong ±50 thì ghép ngay;
   - không có thì trả `arena_searching`, sau 2 phút (đồng hồ giả) khoảng lên 100 và ghép được ghost lệch 90;
   - huỷ thì không ghép nữa;
   - tìm khi đang tìm thì `already_searching`;
   - một mình trong mùa thì đánh ghost máy;
   - mất kết nối thì xoá lượt tìm.
   Test cũ dùng `arena_battle` đổi sang luồng mới.

## Client
1. `NetworkService`:
   - thêm `FindArenaMatch(loadout, power, onSearching, onResult, onError)` và `CancelArenaSearch()`;
   - xử lý tin server chủ động gửi (`arena_searching`, `arena_result` có `replyTo: 0`);
   - bỏ `RequestArenaBattle`.
2. Lớp "Đang tìm đối thủ" ở Sảnh:
   - hiện thời gian chờ `mm:ss` (đếm ở client từ `startedAt`);
   - hiện khoảng đang tìm: "Tìm đối thủ quanh 1.032 điểm (±50)";
   - hiện "Nới khoảng sau 1:23";
   - có nút **Huỷ** (nút phụ 260x62).
   Bỏ thời gian chờ tối thiểu 1,2 giây khi phải đợi thật, nhưng vẫn giữ khi ghép được ngay.
3. Đóng Sảnh khi đang tìm thì gửi huỷ. Mất kết nối thì đóng lớp tìm và báo "Mất kết nối, đã huỷ tìm trận".
4. **Builder:** thêm nút Huỷ và các dòng chữ trên vào lớp tìm trận (`OnlinePrefabBuilder`, helper `Place`, theo pixel).
5. **Test EditMode** (transport giả):
   - tìm được ngay;
   - nhận `arena_searching` rồi sau đó nhận `arena_result` chủ động;
   - huỷ gửi đúng `arena_cancel`.

## Được phép sửa
- `server/game-server/**`, trừ `pom.xml` (một task khác đang sửa `pom.xml` để đóng gói; cần thêm thư viện thì ghi vào
  báo cáo).
- **Không** sửa `GameServer.main`/phần đọc tham số dòng lệnh. Task đóng gói đang sửa chỗ đó. Phần nối "gửi chủ động"
  trong `GameServer` thì được sửa.
- `One-Bag-Jianghu/Assets/_Game/Scripts/Net/**`, `Scripts/UI/Arena/**`, `Editor/Building/OnlinePrefabBuilder.cs`,
  `Tests/EditMode/Net/**`, `Tests/EditMode/UI/Arena*`.
- `docs/tech/protocol.md` chỉ khi phát hiện chỗ đặc tả thiếu hoặc sai. Ghi rõ đã đổi gì.

**Cấm:** mọi lệnh git, chạy Unity, sửa scene và ảnh, đặt tên hàm trùng tên kiểu Unity (ví dụ `Image`).

## Tự kiểm tra
- Trong `server/`, `./mvnw -o test` phải qua. Sandbox chặn đọc `~/.m2` thì copy tạm vào workspace rồi xoá như các lần
  trước.
- Mọi file đã sửa phải qua `py tools/lint/check_style.py <file>`, chạy ở thư mục gốc repo.
- Claude sẽ build Unity, chạy test và chụp ảnh.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`: file đã sửa, cách gửi chủ động, kết quả test.
