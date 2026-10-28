Đã sửa trong `server/game-server/**`.

- Thêm `PowerCalculator.java`: công thức Chiến Uy dùng chung từ cấp và danh sách `(itemId, tier, rarity)`.
- `ProfileService` dùng công thức chung, giữ nguyên `Math.rint`.
- `ArenaService` tính Chiến Uy bot từ `Loadout`; mặc định Trọng 1, phẩm `common`.
- `opponent_power` lưu đúng Chiến Uy mới.
- Cập nhật `MessageHandler` và JUnit.

Kiểm tra:

- Bot mẫu `kim_cang.json`: `275`.
- Hồ sơ người chơi hồi quy: `112`.
- Maven: `94` test qua, `BUILD SUCCESS`.
- Style checker: `0 problem(s)`.