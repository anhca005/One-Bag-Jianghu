Đã sửa UI Sảnh Luận Kiếm Đài và popup Đăng nhập.

- Nguyên nhân danh sách trống: `ScrollList` dùng cấu hình layout mặc định không kiểm soát kích thước con, row thiếu `LayoutElement`, content giữ kích thước mặc định và không được rebuild sau bind. Đã chuẩn hóa row 520×64, spacing 6, `ContentSizeFitter`, bật row sau instantiate và ép rebuild. Áp dụng cả Lịch Sử.
- Sửa huy hiệu bậc bằng `ImageKey`, chỉ gọi `ArenaRanks.Load()` một lần; Image thiếu sprite sẽ bị tắt.
- Thêm chân dung và huy hiệu 72×72 cho thẻ Hiệp Khách/Đối đầu.
- Tên phái lấy từ `SectCatalog`; không có phái hiện “Vô Môn Phái”.
- Bao Phục mini hiển thị đúng footprint vật phẩm, `ShapeFrame` và màu phẩm.
- Đổi tab thành lưới 2×2, chỉnh vùng nội dung, thanh tuần, top bar và kích thước chữ.
- Popup đăng nhập tăng thành 828×760, dời form xuống và thêm icon mắt đóng/mở.
- Thêm EditMode test bind 3 leaderboard entry và kiểm tra đủ 3 row đang bật.

Field mới:

- `heroPortrait`, `miniBagItems`
- `versusPlayerPortrait`, `versusOpponentPortrait`
- `passwordEyeIcon`, `eyeOpenSprite`, `eyeClosedSprite`

Cả 5 file C# đã sửa đều qua `check_style.py` với `0 problem(s)`. Không chạy Unity theo yêu cầu.