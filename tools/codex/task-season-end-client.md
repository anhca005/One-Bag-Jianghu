# Task: Client cho thưởng cuối mùa và Tiệm Danh Vọng 2 tuần (#72)

Bạn là Codex, làm việc trong Unity project One Bag Jianghu (Unity 6000.3.9f1, UGUI, Input System).

## Đọc trước
- `docs/decisions.md` #72.
- `docs/tech/protocol.md`: `seasonReward`, `season_reward_seen`, `peakRating`, `weeklyWinsTarget`. Server đã làm xong,
  báo cáo ở `tools/codex/out/season-end-server.md`.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json`: offer `chi_ton_ngoc_tuy` có thêm `"periodWeeks": 2`.
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.

## Lỗi đang có
- `ShopCatalog` (C#) đọc JSON rất chặt, nên báo `Could not find member 'periodWeeks' on object of type
  'ReputationOffer'`.
- Hậu quả: 13 test Thương Hội hỏng, và Thương Hội trong game không mở được.

## Việc cần làm
1. **`ReputationOffer`:**
   - thêm `PeriodWeeks`, mặc định 1, phải ≥ 1;
   - Tiệm Danh Vọng hiện giới hạn đúng, ví dụ "1 lần / 2 tuần" thay vì "1 lần / tuần".
   - Luật mua vẫn do server quyết. Phần xem trước phía client (nếu có) phải tính chu kỳ giống server: chu kỳ tuần
     theo UTC+7, gộp 2 tuần.
2. **Mốc tuần Luận Kiếm:** Sảnh dùng `weeklyWinsTarget` từ `arena_status`, không viết cứng 10 ở đâu cả. Tìm và sửa
   mọi chỗ còn viết cứng.
3. **Thưởng cuối mùa:**
   - đọc `seasonReward` trong `auth_ok` và `arena_status_ok`;
   - có thưởng chưa xem thì hiện `MessagePopup` có sẵn, ví dụ: "Kỳ Luận Kiếm 1 đã khép lại. Bạn đứng hạng 3 với 1.420
     điểm, nhận 2.000 Danh Vọng và 2 Chí Tôn Ngọc Tủy.";
   - đóng popup thì gửi `season_reward_seen`;
   - hiện ở Home sau khi đăng nhập, hoặc khi mở Sảnh nếu Home chưa hiện. **Chỉ hiện một lần.**
   - Hồ sơ đã được server cộng sẵn, client không cộng gì.
4. **`peakRating`:** đọc vào model. Thẻ Hiệp Khách ở Sảnh hiện thêm dòng "Cao nhất mùa: 1.420" nếu còn chỗ, không
   thì bỏ qua.
5. **Test EditMode:**
   - `ShopCatalog` đọc được `periodWeeks`, và báo lỗi khi giá trị < 1;
   - `seasonReward` trong `auth_ok` được đọc, đóng popup thì gửi `season_reward_seen` (transport giả như
     `Tests/EditMode/Net/OnlineNetTests.cs`);
   - 13 test Thương Hội qua lại.

## Được phép sửa
- `One-Bag-Jianghu/Assets/_Game/Scripts/**`, `Tests/EditMode/**`, `Editor/Building/OnlinePrefabBuilder.cs` (nếu cần
  thêm dòng chữ, dùng helper `Place`).
- **Cấm:**
  - mọi lệnh git; chạy Unity;
  - sửa `server/**`, `shop.json`;
  - sửa scene `*.unity`; sửa prefab hay ảnh bằng tay;
  - đặt tên hàm trùng tên kiểu Unity; dùng `UnityEngine.Input`;
  - chạy coroutine trong `Setup` của popup.

## Tự kiểm tra
- Trong thư mục Unity, các lệnh sau phải thành công:
  - `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore`;
  - `dotnet build OneBagJianghu.Game.PlayTests.csproj --no-restore`;
  - `dotnet build OneBagJianghu.Editor.csproj --no-restore`.
- `py tools/lint/check_style.py <mọi file đã sửa>` ở gốc repo phải sạch.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`: file đã sửa, popup hiện ở đâu, test đã thêm.
