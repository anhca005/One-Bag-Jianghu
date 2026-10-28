# Task: Sửa 3 test EditMode còn hỏng sau đợt sửa hồ sơ cũ

Bạn là Codex. Đợt trước (`tools/codex/out/e6-client-fixes.md`) bạn đã sửa client theo báo cáo Gemini. Claude chạy Unity
và còn 3 test hỏng. Lỗi thứ tư (`ArenaLobbyPopup.cs:108` null `Session`) Claude đã tự sửa.

## Lỗi (Unity EditMode)
== OneBagJianghu.Tests.EditMode.Economy.LevelServiceTests.Sect_CannotBeChosenBelowTenOrTwice
System.ArgumentException : The actual value must be a string or a non-null IEnumerable or DirectoryInfo
Parameter name: actual
at NUnit.Framework.Constraints.EmptyConstraint.ApplyTo (System.Object actual) [0x00018] in <f577cd9686154d6ab10efd7e65e64366>:0 
  at NUnit.Framework.Assert.That[TActual] (TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, System.String message, System.Object[] args) [0x0000b] in <f577cd9686154d6ab10efd7e65e64366>:0 
  at NUnit.Framework.Assert.That[TActual] (TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression) [0x00000] in <f577cd9686154d6ab10efd7e65e64366>:0 
  at OneBagJianghu.Tests.EditMode.Economy.LevelServiceTests.Sect_CannotBeChosenBelowTenOrTwice () [0x00048] in D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\Assets\_Game\Tests\EditMode\Econ
== OneBagJianghu.Tests.EditMode.SectPopupTests.ConfirmedPopupJoinSetsSectAndTamPhapLevel
Expected: "han_son"
  But was:  null
at OneBagJianghu.Tests.EditMode.SectPopupTests.ConfirmedPopupJoinSetsSectAndTamPhapLevel () [0x00031] in D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\Assets\_Game\Tests\EditMode\SectPopupTests.cs:51
System.NullReferenceException : Object reference not set to an instance of an object
== OneBagJianghu.Tests.UI.SimpleImagePrefabTests.UiPrefabsDoNotSliceOrDistortUiSprites
Assets/_Game/Resources/UI/Popups/ArenaHistoryRow.prefab :: ArenaHistoryRow aspect error 11.76%
Assets/_Game/Resources/UI/Popups/ArenaLobbyPopup.prefab :: ArenaLobbyPopup/Arena Background/Versus Card/Player Portrait Frame/Portrait aspect error 17.62%
Assets/_Game/Resources/UI/Popups/ArenaLobbyPopup.prefab :: ArenaLobbyPopup/Arena Background/Versus Card/Opponent Portrait Frame/Portrait aspect error 17.62%
Assets/_Game/Resources/UI/Popups/LeaderboardRow.prefab :: LeaderboardRow aspect error 11.76%
  Expected: <empty>
  But was:  < "Assets/_Game/Resources/UI/Popups/ArenaHistoryRow.prefab :: ArenaHistoryRow aspect error 11.76%", "Assets/_Game/Resources/UI/Popups/ArenaLobbyPopup.prefab :: ArenaLobbyPopup/Arena Background/Versus Card/Player Portrait Frame/Portrait aspect error 17.62%", "Assets/_Game/Resources/UI/Popups/ArenaLobbyPopup.prefab :: ArenaLobbyPopup/Arena Background/Versus Card/Opponent Portrait Frame/Portrait aspect error 17.62%", "Assets/_Game/Resources/UI/Popups/LeaderboardRow.prefab :: LeaderboardRow aspect error 11.76%" >
at OneBagJianghu.Tests.UI.SimpleImagePrefabTests.UiPrefabsDoNotSliceOrDistortUiSprites () [0x0004b] in D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\Assets\_Game\Tests\EditMode\UI\SimpleImagePrefabTests.cs:29

## Cách sửa
1. **`Sect_CannotBeChosenBelowTenOrTwice` và `ConfirmedPopupJoinSetsSectAndTamPhapLevel`.**
   - `SectChoiceService.TryChoose` giờ chỉ kiểm tra, còn việc chọn phái do server làm (`sect_choose`).
   - Sửa test cho đúng luồng mới: kiểm `TryChoose` trả đúng và không đổi hồ sơ; chọn phái qua transport giả như
     `Tests/EditMode/Net/OnlineNetTests.cs`, áp hồ sơ server trả về, rồi kiểm `SectId` và `TamPhapLevel`.
   - Đừng sửa code game cho test qua, trừ khi thấy đó là lỗi thật. Nếu là lỗi thật thì ghi rõ.
2. **`UiPrefabsDoNotSliceOrDistortUiSprites`.** Đọc `Tests/EditMode/UI/SimpleImagePrefabTests.cs` để biết cách đo.
   - `Versus Card/.../Portrait`: ảnh `hero_portrait` (1024x1536) nằm trong khung 444:552, nên bị kéo méo 17,6%.
     Ô `Portrait` phải đúng tỉ lệ ảnh của chính nó, hoặc bật `preserveAspect` nếu test chấp nhận.
   - Ảnh nền gốc của `ArenaHistoryRow` và `LeaderboardRow` lệch 11,76%. Mở ảnh thật để lấy kích thước pixel đúng.
   - Sửa trong `Editor/Building/OnlinePrefabBuilder.cs`, không sửa prefab bằng tay. Claude sẽ chạy builder.

## Được phép sửa
`One-Bag-Jianghu/Assets/_Game/Tests/EditMode/**`, `Editor/Building/OnlinePrefabBuilder.cs`. Sửa `Scripts/**` chỉ khi đó
là lỗi thật, và phải ghi lý do.

**Cấm:** mọi lệnh git, chạy Unity, sửa `server/**`, sửa scene, sửa prefab hay ảnh bằng tay.

## Tự kiểm tra
- `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore` và `dotnet build OneBagJianghu.Editor.csproj
  --no-restore` (trong thư mục Unity) phải thành công.
- `py tools/lint/check_style.py <file đã sửa>` ở gốc repo phải sạch.

## Báo cáo
Ngắn gọn, tiếng Việt, ở file `-o`.
