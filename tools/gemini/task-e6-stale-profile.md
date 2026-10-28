# Task (Gemini): Rà soát chỗ client còn giữ hồ sơ cũ sau khi server gửi hồ sơ mới

Bạn là Gemini, làm việc trong repo One Bag Jianghu. Đây là việc **chỉ đọc và viết báo cáo**, không sửa code.

## Bối cảnh
- Từ E6 (#70, #71), mỗi tin `*_ok` từ server gửi kèm toàn bộ hồ sơ.
- `GameSession.ApplyServerProfile` (`One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs`) tạo **một object
  `PlayerProfile` mới** rồi gán vào `GameSession.Profile`, sau đó phát `PlayerProfile.ProgressChanged`.
- Mọi lớp còn giữ tham chiếu tới object cũ (trường, tham số constructor, closure, biến cục bộ sống qua một lần chờ
  server) sẽ đọc hoặc ghi vào dữ liệu cũ.
  - Ví dụ đã thấy: `TutorialService` nhận `PlayerProfile` trong constructor.
- Hệ quả: UI hiện sai, hoặc client sửa vào object cũ mà không gửi lên server.

## Việc cần làm
Đọc `One-Bag-Jianghu/Assets/_Game/Scripts/**`. Liệt kê **mọi** chỗ:
1. **Giữ `PlayerProfile` lâu dài:** trường, thuộc tính, constructor của service hoặc popup, `StorageGrid`, các
   `*Service`, UI panel, closure…
   - Với mỗi chỗ ghi rõ: có làm mới khi `ProgressChanged` hay khi mở lại popup không, và có đọc dữ liệu cũ sau một lần
     chờ server không.
2. **Vẫn tự sửa hồ sơ ở client** (gọi `Apply`, `Buy`, `AddItem`, `AddCurrency`, `RecordQuestAction`, gán trường…) trên
   object thật thay vì trên bản sao để xem trước.
3. **UI không vẽ lại** sau khi nhận hồ sơ mới: không nghe `ProgressChanged` và không làm mới sau callback `*_ok`.
4. **Gọi thao tác mạng mà không khoá nút** hoặc không xử lý lỗi `rejected`.

## Báo cáo
Ghi vào `docs/reviews/2026-10-21-gemini-e6-stale-profile.md` bằng tiếng Việt, dạng bảng:
- file:dòng;
- vấn đề;
- mức độ: Cao (sai dữ liệu hoặc mất thao tác) / Vừa (hiện sai đến khi mở lại) / Thấp;
- cách sửa gợi ý, 1–2 câu.
Xếp mức Cao lên đầu. Không liệt kê chỗ đã đúng.

## Luật
- Chỉ tạo file báo cáo trên.
- Không sửa file khác, không chạy git.
- Dẫn file:dòng cho mọi khẳng định.
