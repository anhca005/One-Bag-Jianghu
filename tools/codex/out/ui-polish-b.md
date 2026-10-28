Đã hoàn tất UI polish Task B và kiểm tra lại phần dang dở.

Các điểm chính:

- Hoàn thiện fallback chân dung quái Hồi 4–5 và đối thủ Luận Kiếm.
- Sửa thưởng đánh lại, “Chiến Uy”, thưởng mùa/tuần Luận Kiếm và trạng thái danh sách rỗng.
- Giữ lỗi Tàng Bảo Các trong nhãn riêng 8 giây, không bị `Refresh()` xoá.
- Hoàn thiện trạng thái chờ/khoá thao tác cho Bao Phục, shop, chọn phái và đăng nhập.
- Thêm số Khí Huyết/Cương Khí được định dạng và phím Esc xác nhận thoát trận.
- Chuẩn hóa builder Bao Phục sang “Chiến Uy”.
- Giữ `VanBaoPhoItemCard` trong file riêng đúng quy ước MonoBehaviour.

Tự kiểm:

- Game Tests build: thành công.
- PlayTests build: thành công.
- Editor build: thành công.
- Style lint: 0 lỗi.
- Còn 4 cảnh báo API Unity deprecated có sẵn, không ảnh hưởng build.

Báo cáo đầy đủ: [ui-polish-b.md](</D:/CONG VIEC/One-Bag-Jianghu/tools/codex/out/ui-polish-b.md>)

Các builder cần chạy lại: `OnlinePrefabBuilder`, `EconomyPopupPrefabBuilder`, `PvePrefabBuilder`, `BagPrefabBuilder`.