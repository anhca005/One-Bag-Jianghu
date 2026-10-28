# Task: Sửa các chỗ client còn giữ hồ sơ cũ sau E6, cùng test ảnh méo ở Sảnh Luận Kiếm

Bạn là Codex, làm việc trong Unity project One Bag Jianghu (Unity 6000.3.9f1, UGUI, Input System).

## Bối cảnh
- Từ E6 (#70, #71), mỗi tin `*_ok` của server gửi kèm toàn bộ hồ sơ.
- `GameSession.ApplyServerProfile` tạo một object `PlayerProfile` **mới**, rồi phát `PlayerProfile.ProgressChanged`.
- Gemini đã rà soát và liệt kê 19 chỗ client còn giữ hồ sơ cũ, khoá nút chưa đủ, hoặc không vẽ lại:
  **`docs/reviews/2026-10-21-gemini-e6-stale-profile.md`**.
  - Đọc kỹ từng dòng và **kiểm lại bằng code** trước khi sửa: báo cáo có thể sai.
- Lưu ý `TutorialService`: Claude đã thêm tập `finished` để nó không chạy lại phần hướng dẫn vừa xong. Nếu sửa cho nó
  đọc `GameBootstrap.Session.Profile` mới nhất thì giữ lại tập này.

## Đọc trước
- `docs/decisions.md` #70, #71. `docs/tech/protocol.md`, mục "Hồ sơ trên server".
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.

## Việc cần làm
1. **Sửa mọi mục mức Cao và Vừa** trong báo cáo. Mục Thấp thì sửa nếu chỉ mất vài dòng.
   - Không giữ `PlayerProfile` lâu dài trong service hoặc popup. Mỗi lần dùng thì đọc `GameBootstrap.Session.Profile`,
     hoặc nhận lại hồ sơ khi có `ProgressChanged`.
   - Kiểm tra kết quả **trong callback `*_ok`**, không kiểm ngay sau khi gửi yêu cầu.
   - Màn đang mở phải vẽ lại khi nhận hồ sơ mới: nghe `ProgressChanged` (nhớ huỷ nghe khi đóng), hoặc làm mới trong
     callback.
   - Trong lúc chờ server: khoá **mọi** nút làm thay đổi hồ sơ trên màn đó (mua, đổi, làm mới, Khiêu chiến ải, nhận
     thưởng, xác nhận phái…).
     - Nhận `*_ok` hoặc lỗi thì mở lại.
     - Mất kết nối giữa chừng thì cũng mở lại, không được kẹt.
   - Chỉ sửa trên bản sao khi cần xem trước, không bao giờ sửa object hồ sơ thật.
2. **Test `UiPrefabsDoNotSliceOrDistortUiSprites` đang lỗi.** Ảnh bị méo trong `ArenaHistoryRow.prefab` và
   `ArenaLobbyPopup.prefab`: khung chân dung ở thẻ Đối đầu, dòng Lịch Sử…
   - Sửa trong `Editor/Building/OnlinePrefabBuilder.cs`: kích thước ô ảnh phải đúng tỉ lệ ảnh gốc (helper `Place`,
     theo pixel). Đọc test đó để biết sai số cho phép.
   - **Không** sửa prefab bằng tay. Claude sẽ chạy builder để dựng lại.
3. **Test EditMode** cho các chỗ đã sửa, dùng transport giả như `Tests/EditMode/Net/OnlineNetTests.cs`. Ít nhất có:
   - nút mua ở Thương Hội bị khoá trong lúc chờ, rồi mở lại sau khi nhận `_ok`;
   - nhận hồ sơ mới thì popup đang mở vẽ lại theo hồ sơ mới.

## Được phép sửa
- `One-Bag-Jianghu/Assets/_Game/Scripts/**`, `Tests/EditMode/**`, `Tests/PlayMode/**` (chỉ khi test cũ cần sửa theo),
  `Editor/Building/OnlinePrefabBuilder.cs`.
- **Cấm:**
  - mọi lệnh git; chạy Unity;
  - sửa `server/**` (một task khác đang làm);
  - sửa scene `*.unity`, nhất là `LoginScene.unity` (người dùng chỉnh tay); sửa prefab hay ảnh bằng tay;
  - đặt tên hàm trùng tên kiểu Unity (ví dụ `Image`); dùng `UnityEngine.Input`.

## Tự kiểm tra
- `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore`, `dotnet build OneBagJianghu.Game.PlayTests.csproj
  --no-restore` và `dotnet build OneBagJianghu.Editor.csproj --no-restore` (trong thư mục Unity) đều phải thành công.
- `py tools/lint/check_style.py <mọi file đã sửa>` (chạy ở gốc repo) phải sạch.
- Claude sẽ chạy Unity, test và build bản chơi.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- mục nào đã sửa, ghi số dòng trong báo cáo Gemini;
- mục nào bỏ qua và vì sao (ví dụ báo cáo sai);
- test đã thêm.
