# Task M: PlayMode smoke test for Tàng Bảo Các

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Read `Tests/PlayMode/DemoSmokeTests.cs` (scene loading, profile backup/restore), `Scripts/UI/Popups/{StoragePopup,PopupManager,BasePopup}.cs`, `Scripts/UI/Home/HomeController.cs`, `Scripts/Economy/*`, `Scripts/Core/GameSession.cs`.

A bug slipped through: `PopupManager` calls `Setup` (→ `StoragePopup.Refresh`) before the popup is active, so `Awake` had not run (fixed with `EnsureInitialized`). Add a PlayMode test that would have caught it.

Create ONLY `One-Bag-Jianghu/Assets/_Game/Tests/PlayMode/StorageSmokeTests.cs` (+ nothing else): back up / restore `profile.json` exactly like DemoSmokeTests (never overwrite the real profile), load Login → Home via the real scenes, reset the profile to the starter kit, give materials + Bạc through `PlayerProfile` APIs, open the storage popup through the Home storage hotspot (invoke its click), assert no error was logged (`LogAssert.NoUnexpectedReceived`), assert one row per owned copy, select a starter weapon, press Nâng Trọng, assert its Trọng went 1 → 2 and the materials/Bạc decreased by the previewed amounts, add a second copy of the same item at the same rarity at Trọng 3 plus raise the first to 3, press Hợp Thành, confirm, assert one copy of `uncommon` rarity remains. Close the popup. Use reflection only if a needed field is private; prefer existing public API.

English, C# 9, existing test style. No git. You cannot run Unity; re-read for compile errors. Finish with: the file, anything unsure.
