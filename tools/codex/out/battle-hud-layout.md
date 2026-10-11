Đã hoàn tất phần redesign bằng builder, không chạy Unity.

Files changed:

- [ReplayPrefabBuilder.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/ReplayPrefabBuilder.cs)
- [FighterPanelView.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs)
- [GameplayPrefabTests.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/UI/GameplayPrefabTests.cs)

Layout mới:

- Top banner: `1920 × 164`, bắt đầu tại `y=916`.
- Player HUD: `x=20`, enemy HUD: `x=1120`, mỗi bên `780 × 148`.
- Khí Huyết `620 × 40`; Cương Khí `620 × 20`; Nội Lực `620 × 24`.
- Enemy bar fill từ phải sang trái.
- Sáu status icon vẫn giữ đủ reference và hành vi.
- Timer, `vs_emblem`, tên chế độ nằm giữa.
- Nút Thoát thu gọn tại góc trên phải.
- Avatar panel tăng thành `250 × 360`, chỉ còn tên và portrait.
- Bag giữ nguyên pitch/cách đặt item; chuyển xuống `y=190`.
- Cell dùng `Kit/slot_cell`; màu translucent hiện tại vẫn là fallback.
- Bottom controls/log không đổi.

Binding:

- Mỗi bên vẫn chỉ dùng một `FighterPanelView`.
- Component nằm trên avatar panel nên hit-shake, VFX target và floating number tiếp tục bám portrait.
- Các field bar/status cũ được nối sang HUD trong top banner.
- Thêm `bannerTitle`; `Initialize()` đồng bộ tên giữa banner và avatar.

Tests:

- Cập nhật hierarchy assertions sang `TopBar/PlayerHud` và `TopBar/EnemyHud`.
- Kiểm tra VS emblem.
- Kiểm tra bar không còn trong avatar panel.
- Kiểm tra hướng fill trái/phải của hai HUD.

Kiểm tra:

- Style: `0 problem(s)` trên cả ba file.
- `OneBagJianghu.Game.Tests.csproj`: build thành công, `0 warnings`, `0 errors`.
- Generated Editor project có reference mồ côi tới `BatchUiKitRebuild.cs`; mình dùng stub tạm để xác nhận compile rồi đã xóa stub. Không để lại file ngoài phạm vi task.
- Không chạy Unity.

Menu cần chạy:

1. `One Bag Jianghu/Build/Replay Prefabs (Overwrite)`
2. Test Runner → EditMode để chạy các test UI/prefab.