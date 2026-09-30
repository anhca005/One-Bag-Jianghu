# Trình dựng scene và prefab
Các menu nằm dưới `One Bag Jianghu/Build/`.
`Common UI Prefabs` tạo bộ điều khiển và khung dùng chung.
`GameController Prefab` tạo `Resources/Core/GameController.prefab`.
`Popup Prefabs` tạo năm popup đầy đủ.
`Build All Missing` chạy toàn bộ builder đã đăng ký.

Menu thường chỉ tạo asset khi còn thiếu; asset đã có sẽ được giữ nguyên.
Menu có hậu tố `(Overwrite)` hỏi xác nhận trước khi ghi đè.
Batch mode tự xác nhận để dùng được với `-executeMethod`.

Task B–D thêm builder dạng `Action<bool>` vào danh sách `Builders`
trong `BuildMenu.cs`; tham số `bool` là chế độ ghi đè.
Builder nên gọi `AssetGuard.ShouldBuild` trước khi lưu asset.
UI tĩnh chỉ được tạo bằng `UiBuild` trong Editor assembly.
Prefab dùng chung phải được gắn bằng `UiBuild.InstantiateLinked`.

## UI art slicing

Every generated scene has a flat root hierarchy: `Main Camera`, `EventSystem`, its
controller/bootstrap, and `Canvas`. LoginScene additionally owns the linked persistent
`GameController` prefab instance. The persistent GameController prefab has no
EventSystem or AudioListener; each loaded scene owns the single active instances.

`ItemIconImporter` applies these source-pixel borders as `(left, bottom, right, top)`:

- `button_primary`, `button_secondary`: `(420, 170, 420, 170)`
- `frame_panel`: `(260, 260, 260, 260)`
- `frame_tooltip`: `(180, 150, 180, 150)`
- `plaque_label`: `(300, 160, 300, 160)`
- `progress_bar`: `(330, 150, 330, 150)`

The values isolate the generated cloud, corner, seal, and bamboo-end ornaments so only
the plain middle segments stretch. UI textures import at max size 2048; status icons at 256.
