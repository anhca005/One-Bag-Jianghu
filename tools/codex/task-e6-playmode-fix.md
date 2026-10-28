# Task: Sửa 22 lỗi biên dịch trong test PlayMode sau E6 client

Bạn là Codex. Task E6 client (`tools/codex/out/e6-client.md`) đã bỏ `GameSession.SaveProfile`, `ResetProfile`,
các setter `HeroLevel`/`CurrentLoadoutJson` và `ProfileStore.HeroNameKey`/`SectIdKey`. Vì vậy test PlayMode trong
`One-Bag-Jianghu/Assets/_Game/Tests/PlayMode/` không biên dịch được nữa. Lần trước bạn chỉ build project EditMode.

## Lỗi (Unity log)
Assets\_Game\Tests\PlayMode\BagTooltipSmokeTests.cs(129,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\BagTooltipSmokeTests.cs(27,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\BagTooltipSmokeTests.cs(42,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\DemoSmokeTests.cs(119,13): error CS0200: Property or indexer 'GameSession.HeroLevel' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\DemoSmokeTests.cs(120,13): error CS0200: Property or indexer 'GameSession.CurrentLoadoutJson' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\DemoSmokeTests.cs(229,13): error CS0200: Property or indexer 'GameSession.CurrentLoadoutJson' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\DemoSmokeTests.cs(244,13): error CS0200: Property or indexer 'GameSession.CurrentLoadoutJson' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(43,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(45,48): error CS0117: 'ProfileStore' does not contain a definition for 'HeroNameKey'
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(46,48): error CS0117: 'ProfileStore' does not contain a definition for 'SectIdKey'
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(60,60): error CS0117: 'ProfileStore' does not contain a definition for 'HeroNameKey'
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(61,60): error CS0117: 'ProfileStore' does not contain a definition for 'SectIdKey'
Assets\_Game\Tests\PlayMode\DevWipeSmokeTests.cs(93,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\DragOutsideGridTests.cs(150,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\DragOutsideGridTests.cs(162,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\PveResultFlowTests.cs(168,13): error CS0200: Property or indexer 'GameSession.HeroLevel' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\PveResultFlowTests.cs(169,13): error CS0200: Property or indexer 'GameSession.CurrentLoadoutJson' cannot be assigned to -- it is read only
Assets\_Game\Tests\PlayMode\ShopSmokeTests.cs(23,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\StorageSmokeTests.cs(105,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\StorageSmokeTests.cs(119,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\TextOverflowTests.cs(29,35): error CS1061: 'GameSession' does not contain a definition for 'ResetProfile' and no accessible extension method 'ResetProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)
Assets\_Game\Tests\PlayMode\TextOverflowTests.cs(54,35): error CS1061: 'GameSession' does not contain a definition for 'SaveProfile' and no accessible extension method 'SaveProfile' accepting a first argument of type 'GameSession' could be found (are you missing a using directive or an assembly reference?)

## Cách sửa
- Test cần hồ sơ dựng sẵn thì thêm vào `GameSession` **một** hàm cho test: ví dụ `ApplyProfileForTests(PlayerProfile)`,
  tái dùng đường `ApplyServerProfile`, ghi chú rõ là chỉ cho test. Không mở lại đường ghi đĩa.
- Test kiểm đường lưu/xoá file trên máy (ví dụ `DevWipeSmokeTests`) mà không còn ý nghĩa thì xoá ca đó và ghi vào báo cáo.
- Test thao tác (Shop/Storage smoke) mà giờ phải đi qua mạng thì dùng transport giả như `Tests/EditMode/Net/OnlineNetTests`,
  hoặc chỉ giữ phần kiểm giao diện. Chọn cách ít đổi nhất.
- Đọc `docs/tech/code-style.md`.

## Được phép sửa
`One-Bag-Jianghu/Assets/_Game/Tests/PlayMode/**`, `Scripts/Core/GameSession.cs` (chỉ thêm hàm cho test).

**Cấm:** mọi lệnh git, chạy Unity, sửa `server/**`, scene, prefab.

## Tự kiểm tra
- Build **cả hai** project test: `OneBagJianghu.Game.PlayTests.csproj` và `OneBagJianghu.Game.Tests.csproj` (cùng `OneBagJianghu.Editor.csproj`), chạy
  `dotnet build <csproj> --no-restore` cho từng file. Cả hai phải thành công.
- `py tools/lint/check_style.py <file đã sửa>` ở gốc repo.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`: file đã sửa, test nào đã xoá và vì sao.
