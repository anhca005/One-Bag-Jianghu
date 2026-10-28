# Task (Gemini): Rà soát UI còn dở hoặc chưa chỉn chu

Bạn là Gemini, làm việc trong repo One Bag Jianghu: game auto-battler xếp túi đồ, võ hiệp, Unity UGUI, PC. Đây là việc
**chỉ đọc và viết báo cáo**, không sửa code.

## Bối cảnh
- Game sắp có bản demo cho người chơi thật. Người dùng muốn mọi màn hình "chỉn chu nhất có thể".
- Từ #70/#71, mọi thao tác đi qua server. Mỗi nút cần có trạng thái chờ và cách báo lỗi.
- Quy ước UI:
  - ảnh hiển thị kiểu Simple, đúng tỉ lệ (#66);
  - chữ nằm gọn trong khung, chừa khoảng 30% cho bản dịch sau này;
  - ảnh không chứa chữ;
  - thuật ngữ theo `docs/design/glossary.md`.

## Đọc
- `One-Bag-Jianghu/Assets/_Game/Scripts/UI/**`, `Scripts/Onboarding/**`, `Scripts/Battle/Replay/View/**`.
- `One-Bag-Jianghu/Assets/_Game/Editor/Building/*Builder.cs`: đây là nơi dựng prefab, gồm kích thước, chữ, ảnh.
- `docs/design/arena-lobby.md`, `docs/design/gdd.md` (các màn hình), `docs/design/glossary.md`, `docs/decisions.md`
  (#66 trở đi).
- Danh sách ảnh có sẵn: `One-Bag-Jianghu/Assets/_Game/Textures2D/**`, `Resources/PveArtLibrary.asset`,
  `ItemIconLibrary`.

## Tìm và liệt kê
1. **Tính năng hoặc nút chưa làm:** chữ "Sắp mở", "TODO", nút không gắn hàm, hàm rỗng, tab trống, màn thiết kế trong
   docs nhưng chưa có trong code.
2. **Thiếu trạng thái:**
   - danh sách không có thông báo khi trống (bảng xếp hạng, lịch sử, kho, nhiệm vụ…);
   - không có trạng thái đang tải khi chờ server;
   - không báo lỗi khi server từ chối hoặc mất mạng;
   - nút không khoá khi không bấm được (thiếu tiền, thiếu cấp…) mà không giải thích vì sao.
3. **Ảnh:**
   - key ảnh được gọi trong code nhưng không có trong thư viện ảnh, nên sẽ hiện ô trắng;
   - ảnh tạm, silhouette, màu trơn đứng thay cho ảnh thật;
   - chỗ dùng cùng một ảnh cho nhiều thứ khác nhau (ví dụ mọi đối thủ đều dùng chân dung người chơi).
4. **Chữ:**
   - chữ có thể tràn khung: tên dài, số lớn, câu dài trong ô nhỏ, theo kích thước trong builder;
   - chữ tiếng Anh lọt vào UI;
   - thuật ngữ sai glossary;
   - số không định dạng (1000 thay vì 1.000).
5. **Nhất quán:** cùng loại nút nhưng khác kích thước hoặc kiểu; tiêu đề popup không đồng bộ; thiếu nút đóng hoặc
   không đóng được bằng Esc.
6. **Phản hồi thao tác:** mua, nâng Trọng, nhận thưởng… thành công mà không có thông báo, âm thanh hay hiệu ứng.

## Báo cáo
Ghi vào `docs/reviews/2026-10-27-gemini-ui-audit.md` bằng tiếng Việt.
- Nhóm theo **màn hình**: Login, Home, Bao Phục, Tàng Bảo Các, Thương Hội, Bản đồ ải, Trận đấu, Kết quả, Nhiệm vụ,
  Chọn phái, Sảnh Luận Kiếm, Cài đặt, Bách Khoa, Hướng dẫn…
- Mỗi mục ghi:
  - file:dòng;
  - vấn đề;
  - mức độ: **Cao** (người chơi thấy rõ là lỗi hoặc thiếu), **Vừa** (thiếu chỉn chu), **Thấp**;
  - đề xuất sửa 1–2 câu;
  - nếu cần ảnh mới thì ghi "cần ảnh" kèm mô tả ngắn.
- Cuối báo cáo:
  - danh sách **ảnh cần vẽ thêm**;
  - **10 việc nên làm trước nhất**.
- Không liệt kê chỗ đã ổn. Dẫn file:dòng cho mọi khẳng định.

## Luật
- Chỉ tạo file báo cáo trên.
- Không sửa file khác, không chạy git, không chạy Unity.
