Đã triển khai hệ thống Hành Tẩu Lục.

Các phần chính:

- Dữ liệu 9 nhiệm vụ Hồi 1: [quests.json](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/quests.json>)
- Catalog và validation: [QuestCatalog.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestCatalog.cs>)
- Tiến độ, nhận thưởng, baseline và objective labels: [QuestService.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs>)
- Home tracker: [QuestTracker.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/QuestTracker.cs>)
- Popup Hành Tẩu Lục: [QuestLogPopup.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs>)
- Builder UI: [QuestUiBuilder.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/QuestUiBuilder.cs>)
- EditMode tests: [QuestSystemTests.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Quests/QuestSystemTests.cs>)

Profile được nâng lên version 9 với:

- `quest.activeIndex`
- `quest.activeId`
- `quest.claimedIds`
- `quest.actionCounters`
- `quest.actionBaseline`

Save version 1–8 tiếp tục được migration tự động.

Bộ đếm được tăng sau khi thành công tại:

- `shop_buy`: `ShopService.Buy`
- `item_upgrade`: `UpgradeService.Apply`
- `item_merge`: cả hai nhánh thành công của `MergeService.Apply`
- `pve_win`: `GameplayController.ShowPveResult`

Tracker được `HomeController` tạo dưới Home canvas lúc runtime; không sửa HomeScene. Sau khi chạy builder, dùng menu:

`One Bag Jianghu/Build/Quest UI (Overwrite)`

Kiểm tra:

- Style lint: `0 problem(s)` trên toàn bộ 12 file C# thay đổi/tạo.
- Temporary-project builds cho Game, Editor và EditMode tests: thành công, không lỗi.
- Không mở Unity, không sửa scene hoặc prefab hiện hữu.
- EditMode tests đã được viết và compile, nhưng chưa chạy bằng Unity Test Runner do yêu cầu không khởi động Unity.

Giới hạn còn lại: reward trong QuestLog hiện trình bày bằng danh sách chữ thay vì các icon riêng cho từng phần thưởng. Claim feedback dùng `MessagePopup`, đúng phương án fallback trong đặc tả.