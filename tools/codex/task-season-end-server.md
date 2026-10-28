# Task: Chốt mùa Luận Kiếm, thưởng top, đặt lại điểm, siết Chí Tôn Ngọc Tủy (server)

Bạn là Codex, làm việc trong repo One Bag Jianghu.

## Đọc trước
- `docs/decisions.md` **#72** (vừa chốt), #68, #69, #70, #71.
- `docs/tech/protocol.md`: các mục Luận Kiếm và "Hồ sơ trên server".
- Server: `server/game-server/src/main/java/com/onebagjianghu/server/` (`ArenaService`, `ArenaCalendar`, `Database`,
  `ProfileService`, `E6Rules`, `MessageHandler`).
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.

## Việc cần làm
1. **Chốt mùa:**
   - Mùa dài 28 ngày theo `ArenaCalendar` (UTC+7). Khi một mùa đã hết, server chốt **đúng một lần**, kể cả khi
     server tắt lúc hết mùa rồi bật lại sau:
     - kiểm khi khởi động, và kiểm định kỳ (ví dụ mỗi phút);
     - lưu trạng thái đã chốt trong DB.
   - Lấy bảng xếp hạng cuối mùa: cùng thứ tự với `leaderboard`.
   - Trao thưởng theo bảng ở #72: hạng 1, 2–3, 4–10, 11–50 nhận Danh Vọng (`danh_vong`) và `chi_ton_ngoc_tuy`.
     - Cộng thẳng vào hồ sơ qua khung `ProfileService.mutate`, có kiểm Tàng Bảo Các còn chỗ cho Ngọc Tủy.
     - Kho đầy thì Ngọc Tủy vào `overflow`, hoặc theo đúng cách code hiện có xử lý khi không còn chỗ. Không được làm
       mất thưởng.
   - Ghi mỗi phần thưởng vào bảng `season_rewards` (`season_id`, `account_id`, `rank`, `rating`, `reputation`,
     `jade`, `seen`).
   - Mùa mới: mọi người về **1000 điểm**. Ghost của mùa cũ không dùng để ghép trận mùa mới.
   - Mọi việc chốt mùa nằm trong transaction, chạy lại không trao trùng.
2. **`peak_rating`:**
   - thêm cột vào dữ liệu người chơi trong mùa, cập nhật sau mỗi trận (của cả hai bên);
   - đưa vào `arena_status`.
   - Chưa dùng để thưởng, chỉ lưu (#72).
3. **Báo thưởng cho client:**
   - `auth_ok` và `arena_status` có thêm
     `"seasonReward": { "seasonId", "rank", "rating", "reputation", "jade" }` khi người chơi có phần thưởng chưa xem;
   - tin `season_reward_seen` `{ "seasonId" }` → `season_reward_seen_ok`, đánh dấu đã xem.
   - Ghi đặc tả vào `docs/tech/protocol.md`.
4. **Siết Ngọc Tủy:**
   - `One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json`: offer `chi_ton_ngoc_tuy` giá **800**, giới hạn **1 lần
     mỗi 2 tuần**.
     - Thêm trường cấu hình, ví dụ `"periodWeeks": 2`, mặc định 1.
     - Server tính chu kỳ 2 tuần theo cùng mốc tuần UTC+7 của Thương Hội.
   - Mốc tuần Luận Kiếm: **15 trận thắng** mới nhận 1 viên (trước là 10).
     - Đưa số này ra cấu hình, không viết cứng hai chỗ; `weeklyWinsTarget` trong `arena_status` dùng cùng số.
   - Client đang đọc `shop.json` để hiện giá/giới hạn. Nếu client cần hiểu `periodWeeks` thì ghi vào báo cáo, **đừng
     sửa client**.
5. **Test JUnit** (dùng đồng hồ giả):
   - hết mùa thì top 1/2–3/4–10/11–50 nhận đúng thưởng, người hạng 51 không nhận;
   - chốt hai lần không trao trùng;
   - mùa mới mọi người về 1000;
   - `peak_rating` tăng và không giảm khi thua;
   - `seasonReward` hiện cho tới khi gửi `season_reward_seen`;
   - Tiệm Danh Vọng chặn viên thứ hai trong cùng chu kỳ 2 tuần và cho mua ở chu kỳ sau;
   - mốc tuần cần đúng 15 thắng.

## Được phép sửa
- `server/game-server/**`.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json` (chỉ offer Ngọc Tủy).
- `docs/tech/protocol.md`.

**Cấm:** mọi lệnh git; sửa file C# trong `One-Bag-Jianghu/` (một task khác đang sửa client); chạy Unity; sửa
`server/data/*.db`.

## Tự kiểm tra
- Trong `server/`: `./mvnw -o test` phải qua. Sandbox cần thì thêm `-Dmaven.repo.local=../.m2/repository`.
- `py tools/lint/check_style.py <file Java đã sửa>` ở gốc repo phải sạch.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- file đã sửa;
- cách bảo đảm chỉ chốt một lần;
- client cần làm gì: hiện thưởng mùa, `periodWeeks`, mốc 15 thắng;
- kết quả test.
