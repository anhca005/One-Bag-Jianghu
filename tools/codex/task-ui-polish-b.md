# Task UI polish B: Thương Hội, Tàng Bảo Các, Bao Phục, Bản đồ ải, Chọn phái, Trận đấu, Luận Kiếm, Đăng nhập

Bạn là Codex, làm việc trong Unity project One Bag Jianghu. **Đọc trước `tools/codex/task-ui-polish-common.md`**: bối
cảnh, quy ước, điều cấm, cách tự kiểm và báo cáo đều ở đó.

## Màn hình của task này
Theo các mục trong báo cáo Gemini:
- 1 Login & Tài khoản;
- 3 Bao Phục;
- 4 Tàng Bảo Các;
- 5 Thương Hội;
- 6 Bản đồ ải;
- 7 Trận đấu;
- 10 Chọn phái;
- 11 Sảnh Luận Kiếm.
Kèm theo mọi mục Claude ghi cho Đăng nhập, Thương Hội, Tàng Bảo Các, Sảnh Luận Kiếm, Bản đồ ải ("Chiến Uy").

Việc cụ thể, ngoài các mục trong báo cáo:
- **Tàng Bảo Các:** lỗi server không được bị `Refresh()` xoá mất. Hiện lỗi ở chỗ riêng, đủ lâu để đọc.
- **Chân dung quái Hồi 4–5** (`enemy_hoi_4_*`, `enemy_hoi_5_*`) chưa có ảnh:
  - dùng ảnh dự phòng bằng code, ví dụ chọn ổn định một ảnh `enemy_portrait_*` có sẵn theo id ải;
  - không để ô xám;
  - dùng ở Bản đồ ải và ở màn trận.
- **Bản đồ ải:** ải đã vượt thì hiện thưởng đánh lại (`repeatRewardPercent` trong `Pve/chapters.json`), không ghi
  "Lần đầu: 100%".
- **Luận Kiếm:**
  - Đối thủ không dùng chung chân dung người chơi. Chọn ổn định một ảnh `enemy_portrait_*` có sẵn theo tên hoặc phái
    đối thủ, cả ở thẻ Đối đầu lẫn trong trận.
  - Tab Thưởng ghi đúng #72: mốc tuần lấy từ `weeklyWinsTarget`; thưởng cuối mùa theo thứ hạng (1 / 2–3 / 4–10 /
    11–50, cùng số Danh Vọng và Ngọc Tủy); đặt lại 1.000 điểm. Bỏ chữ "sắp có".
  - Thanh mốc tuần rỗng khi chưa có dữ liệu.
  - Bảng xếp hạng và lịch sử có dòng báo khi trống.
- **Popup Đăng nhập:**
  - đổi dòng chú thích cuối cho đúng (#70), ví dụ "Mọi hành trang được lưu trên máy chủ.";
  - icon mắt to và rõ, nền hợp kit UI;
  - nút "Để sau" không hiện khi bắt buộc đăng nhập (kiểm lại).
- **Chọn phái:** nút "Nâng Tâm Pháp (Sắp mở)" thì khoá hẳn, có dòng giải thích ngắn. Không để chữ "Sắp mở" trên nút.

## Được phép sửa
- `Scripts/UI/Popups/`: `ShopPopup`, `ShopCard` và các file Thương Hội, `StoragePopup`, `BagPopup`, `SectSelectPopup`,
  `AccountPopup`, `AccountFormLogic`, `DevCheatPanel`.
- `Scripts/UI/Pve/**`, `Scripts/UI/Gameplay/**`, `Scripts/UI/Arena/**`, `Scripts/Battle/Replay/View/**`,
  `Scripts/Bag/View/**`, `Scripts/Pve/**` (chỉ phần hiển thị).
- `Editor/Building/`: `OnlinePrefabBuilder.cs`, `EconomyPopupPrefabBuilder.cs`, `PvePrefabBuilder.cs`,
  `BagPrefabBuilder.cs`.
- `Tests/EditMode/**`, cho các test của phần mình. Riêng test `AuthOk_SeasonRewardClosesAndSendsSeen` đang lỗi
  `NullReferenceException` (popup cần `PopupManager`/`GameController`): sửa test cho chạy được trong EditMode, hoặc
  sửa code nếu đó là lỗi thật.

Task A đang sửa: Home, nhiệm vụ, kết quả, cài đặt, hướng dẫn, Bách Khoa, Vạn Bảo Phổ, `Scripts/UI/Common/**`,
`PopupPrefabBuilder`, `QuestUiBuilder`, `CommonUiPrefabBuilder`, `BuildMenu`, `SceneSnapshot`.
