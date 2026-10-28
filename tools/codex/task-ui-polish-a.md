# Task UI polish A: Home, nhiệm vụ, kết quả trận, cài đặt, hướng dẫn, Bách Khoa, Vạn Bảo Phổ

Bạn là Codex, làm việc trong Unity project One Bag Jianghu. **Đọc trước `tools/codex/task-ui-polish-common.md`**: bối
cảnh, quy ước, điều cấm, cách tự kiểm và báo cáo đều ở đó.

## Màn hình của task này
Theo các mục trong báo cáo Gemini:
- 2 Home;
- 8 Kết quả;
- 9 Nhiệm vụ;
- 12 Cài đặt;
- 13 Bách Khoa;
- 14 Hướng dẫn & dẫn truyện;
- 15 Popup chung.
Kèm theo các mục Claude ghi cho Home ("Chiến Uy") và Hành Tẩu Lục (dòng báo khi trống).

Việc cụ thể, ngoài các mục trong báo cáo:
- **`ResultPopup`:** bấm Esc thì về như nút "Về trấn" (hoặc "Về Sảnh" với Luận Kiếm), không được kẹt màn trống.
- **`IntroPopup`:** Esc vẫn gửi `intro_seen` và trả nhạc nền về `MusicKeys.Home`.
- **Tên người giao nhiệm vụ:**
  - mọi id trong `quests.json` phải có tên tiếng Việt;
  - nếu có dữ liệu nhân vật (ví dụ `characters` trong `Pve/chapters.json`) thì đọc tên từ đó, không viết cứng.
- **Vạn Bảo Phổ ở Home** (glossary: bộ sưu tập vật phẩm) đang báo "Sắp ra mắt". Làm bản đơn giản `VanBaoPhoPopup`:
  - lưới mọi vật phẩm trong `items.json`, có icon (`ItemIconLibrary`), tên, phẩm, loại, cùng tooltip chỉ số có sẵn
    (`ItemTooltip`/`ItemDescriber`);
  - món người chơi đang sở hữu (trong hồ sơ) hiện rõ, món chưa có thì mờ;
  - lọc theo loại; đếm "Đã có X/Y";
  - dựng prefab bằng builder mới hoặc `PopupPrefabBuilder`, có thêm menu Overwrite trong `BuildMenu`;
  - thêm popup vào `SceneSnapshot.CaptureAll`.

## Được phép sửa
- `Scripts/UI/Home/**`, `Scripts/UI/Login/**` (chỉ khi cần cho trạng thái chờ đăng nhập), `Scripts/UI/Loading/**`,
  `Scripts/UI/Common/**`, `Scripts/Onboarding/**`, `Scripts/Quests/**`.
- `Scripts/UI/Popups/`: `ResultPopup`, `QuestLogPopup`, `SettingsPopup`, `MessagePopup`, `ConfirmPopup`,
  `EncyclopediaPopup`, `StoryPopup`, `BasePopup`, `PopupManager`, cùng popup mới `VanBaoPhoPopup`.
- `Editor/Building/`: `PopupPrefabBuilder.cs`, `QuestUiBuilder.cs`, `CommonUiPrefabBuilder.cs`, `BuildMenu.cs`,
  `SceneSnapshot.cs`, builder mới cho Vạn Bảo Phổ.
- `Tests/EditMode/**`, cho các test của phần mình.

Task B đang sửa: Thương Hội, Tàng Bảo Các, Bao Phục, Bản đồ ải, Chọn phái, Trận đấu, Sảnh Luận Kiếm, popup Đăng nhập,
`OnlinePrefabBuilder`, `EconomyPopupPrefabBuilder`, `PvePrefabBuilder`, `BagPrefabBuilder`.
