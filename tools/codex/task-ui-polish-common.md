# Phần chung cho hai task UI polish (A và B)

## Bối cảnh
- Game sắp có bản demo cho người chơi thật. Người dùng muốn UI "chỉn chu nhất có thể".
- **Gemini đã rà soát** 56 mục: `docs/reviews/2026-10-27-gemini-ui-audit.md`.
  - **Kiểm lại từng mục bằng code** trước khi sửa: báo cáo có thể sai, và một số mục đã được sửa ở đợt trước.
- **Claude xem ảnh chụp** và thấy thêm:
  - Chỉ số sức mạnh phải gọi là **"Chiến Uy"** ở mọi nơi (glossary). Hiện Home ghi "Lực Chiến", Bản đồ ải ghi
    "Chiến lực đề nghị" / "Lực Chiến của bạn".
  - Popup Đăng nhập: dòng chú thích cuối "Tài khoản dùng cho Luận Kiếm Đài. Đồ đạc vẫn lưu trên máy này." đã **sai**,
    vì mọi dữ liệu nằm trên server (#70). Icon mắt mật khẩu nằm trong ô vuông tối, nhỏ và khó thấy.
  - Thương Hội:
    - số tiền góc trái (1.200 và 0) không có icon tiền tệ;
    - chữ tab "Đổi nguyên liệu" và "Tiệm Danh Vọng" nhỏ hơn tab "Thương Hội".
  - Tàng Bảo Các: chữ ở khung chi tiết bên phải và dòng Bạc/Danh Vọng ở góc trên quá nhỏ và nhạt, khó đọc.
  - Sảnh Luận Kiếm:
    - thanh mốc tuần tô đầy khi chưa có dữ liệu;
    - bảng xếp hạng và lịch sử không có dòng báo khi trống.
  - Hành Tẩu Lục: không có dòng báo khi chưa có nhiệm vụ.

## Việc chung
1. Sửa **mọi mục mức Cao và Vừa** thuộc các màn hình của task mình, cùng các mục Claude liệt kê ở trên. Mục Thấp thì
   sửa nếu chỉ mất vài dòng.
2. Quy ước:
   - Chữ hiển thị là tiếng Việt, đúng `docs/design/glossary.md`.
   - Số định dạng bằng `UiFormat` (1.000).
   - Chữ nằm gọn trong khung, chừa khoảng 30% cho bản dịch.
   - Ảnh kiểu Simple, đúng tỉ lệ (#66). Không để Image không có sprite hiện ô trắng: tắt nó, hoặc dùng ảnh dự phòng.
   - Danh sách trống phải có một dòng báo, ví dụ "Chưa có trận nào".
   - Nút gửi server phải khoá trong lúc chờ và mở lại khi có kết quả hoặc lỗi. Lỗi thì hiện lý do.
   - Thao tác thành công phải có phản hồi: âm thanh sẵn có (`AudioManager`, SFX key có sẵn), hoặc một dòng thông báo
     ngắn.
3. **Không có ảnh thì không bịa file ảnh.** Dùng ảnh có sẵn trong `Textures2D/**` / `PveArtLibrary` / `ItemIconLibrary`
   làm dự phòng. Ghi vào báo cáo những chỗ cần vẽ ảnh mới.
4. Code dễ đọc theo `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for`
   cùng dòng.
5. Prefab phải dựng bằng builder. **Không sửa prefab hay scene bằng tay.**
   - `LoginScene.unity` người dùng chỉnh tay: không sửa.
   - Replay prefab có chỉnh tay: không bắt buộc chạy lại `ReplayPrefabBuilder`. Cần đổi gì ở màn trận thì làm bằng
     code lúc chạy, hoặc ghi vào báo cáo.

**Cấm:**
- mọi lệnh git; chạy Unity; sửa `server/**`;
- sửa file thuộc phạm vi của task kia (xem mục "Được phép sửa" của mình). Cần đụng tới thì ghi vào báo cáo để Claude
  làm;
- đặt tên hàm trùng tên kiểu Unity (ví dụ `Image`); dùng `UnityEngine.Input`;
- chạy coroutine trong `Setup` của popup.

## Tự kiểm tra
- Trong thư mục Unity, các lệnh sau phải thành công:
  - `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore`;
  - `dotnet build OneBagJianghu.Game.PlayTests.csproj --no-restore`;
  - `dotnet build OneBagJianghu.Editor.csproj --no-restore`.
- `py tools/lint/check_style.py <mọi file đã sửa>` ở gốc repo phải sạch.
- Claude sẽ chạy builder, test, chụp ảnh và build bản chơi.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- mục nào đã sửa, ghi số dòng trong báo cáo Gemini hoặc "Claude";
- mục nào bỏ qua và vì sao;
- builder nào cần chạy lại;
- ảnh cần vẽ thêm.
