Đã hoàn tất quest UI và xác nhận build thành công.

| Ảnh | Kích thước | Alpha | Redrawn |
|---|---:|---|---|
| `quest_tracker.png` | 440×128 | Có | Có |
| `quest_badge_main.png` | 72×30 | Có | Có |
| `quest_badge_side.png` | 72×30 | Có | Có |
| `quest_icon_scroll.png` | 56×56 | Có | Có |
| `quest_log_row.png` | 820×150 | Có | Có |
| `quest_status_done.png` | 56×56 | Có | Có |
| `quest_status_active.png` | 56×56 | Có | Có |
| `quest_status_locked.png` | 56×56 | Có | Có |
| `quest_glow_spark.png` | 48×48 | Có | Có |

Ảnh được tạo bằng built-in `image_gen`, trim/resize đúng tỉ lệ và lưu tại [Kit](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit).

Các file code đã thay đổi/thêm:

- [QuestTracker.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/QuestTracker.cs)
- [QuestLogPopup.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs)
- [QuestLogRow.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogRow.cs)
- [UiBorderRunner.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/UiBorderRunner.cs)
- [QuestUiBuilder.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/QuestUiBuilder.cs)
- [QuestUiTests.cs](/D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/UI/QuestUiTests.cs)
- [prompts-ui-kit.md](/D:/CONG%20VIEC/One-Bag-Jianghu/docs/art/prompts-ui-kit.md) — thêm nhóm I, #110–118.

Runner dùng một spark chính và ba trail giảm alpha, chạy theo chu vi theo chiều kim đồng hồ bằng `Time.unscaledTime`, một vòng 2,5 giây. Frame đồng thời pulse màu vàng ấm nhẹ. Component không cấp phát trong `Update` và ẩn toàn bộ khi nhiệm vụ chưa hoàn thành.

Kiểm tra:

- Style lint: `0 problem(s)` trên cả 6 file C#.
- Build bằng bộ `.csproj` tạm có chứa file mới: thành công, 0 lỗi; chỉ còn 2 cảnh báo obsolete có sẵn ngoài phạm vi task.
- Các project tạm và cache `obj` tương ứng đã xóa.
- Không mở Unity, không chạy git, không sửa scene/prefab hiện hữu.

Trong Unity, chạy:

`One Bag Jianghu → Build → Quest UI (Overwrite)`