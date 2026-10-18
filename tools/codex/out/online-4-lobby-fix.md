Đã sửa:

- Chuyển toàn bộ bố cục Arena/Account sang helper `Place()` theo pixel từ góc trên-trái.
- Root popup kéo giãn toàn màn hình, thêm `Canvas`, `CanvasScaler`, `GraphicRaycaster`, `CanvasGroup`.
- Dựng lại thanh trên, thẻ Hiệp Khách, khu Tìm Trận, bảng tab, xếp hạng và lịch sử đúng kích thước thiết kế.
- Thêm icon Danh Vọng/Ngọc Tủy qua `CurrencyArt`.
- Tắt mặc định dải mất kết nối, lớp tìm trận và thẻ đối đầu.
- Thêm field huy hiệu, bậc, Chiến Uy, phái cho hai bên đối đầu.
- Popup đăng nhập dùng khung 720×660, ribbon tiêu đề, InputField/Toggle chung và cột nhập co lại theo tab.
- Nút mắt 56×56 không chữ; chưa có icon mắt nên để trống.
- Dòng xếp hạng dùng huy chương thay số cho top 3 và rút gọn thắng/thua.
- Dòng lịch sử và nút xem lại được đặt lại đúng tọa độ.

Các file sửa đều qua `check_style.py` với `0 problem(s)`. Không chạy Unity theo yêu cầu.