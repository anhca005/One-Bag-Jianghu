# Codex task DV: "Chơi lại từ đầu" button in the dev cheat panel (full wipe)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Read first:
- `Scripts/UI/Popups/DevCheatPanel.cs`;
- `Scripts/Core/GameSession.cs`, `ProfileStore` (wherever it lives), `GameBootstrap`, `SceneFlow`;
- `Editor/Building/EconomyPopupPrefabBuilder.cs` or whichever builder makes `DevCheatPanel`;
- `Scripts/UI/Popups/EconomyRuntime.cs`, `ShopRuntimeClock`;
- the PlayMode tests and their profile backup/restore helper (`SmokeTestBase`).

**Builders only. No git. Do not run Unity**; Claude runs it.

## Problem
The user wants to restart the game from zero while testing:
- deleting PlayerPrefs did nothing, because the save is `Application.persistentDataPath/profile.json` (+ `profile.json.bak`);
- the existing "Đặt lại hồ sơ" button only rebuilds the starter kit. It keeps the hero name and sect and stays in Home.

## Do
1. Add a button **"Chơi lại từ đầu"** to `DevCheatPanel`. Style it as a dangerous action (vermilion) and keep it apart from "Đặt lại hồ sơ", which stays as it is. It opens a `ConfirmPopup`: "Xóa toàn bộ dữ liệu và quay về màn đăng nhập?" with "Xóa hết" / "Hủy".
2. **Confirm** triggers a new `GameSession.WipeAllData()` (or a small `SaveWipe` service) that deletes **everything the game persists**. Find every place first: grep `persistentDataPath`, `PlayerPrefs`, `File.Write`, and economy/shop runtime state.
   - `profile.json`, `profile.json.bak`, and any other save files in `persistentDataPath` the game writes (for example the shop runtime/clock state, if it is stored outside the profile, and the dev `loadout.json`);
   - the game's PlayerPrefs keys (hero name, sect…). **Keep `OneBagJianghu.ServerUrl`**, a connection setting rather than game progress; say so in a comment;
   - the in-memory session (`Profile`, `HeroName`, `SectId`, current PvE stage, any cached runtime), so nothing is re-saved from memory afterwards;
   - any static caches that hold profile-derived state.

   Then close all popups and go to the **Login scene** through `SceneFlow`, the same way a fresh start does. The next login must create a brand-new profile from `starter.json`: level 1, starter kit, no PvE progress, default currencies and stamina.
3. Log `[Cheat] Wipe all data: deleted=<files>`.
4. Only in the dev build, inside the existing `#if` guard that hides the panel in release.

## Tests (PlayMode, with the existing profile backup/restore so the user's real save survives)
- Make a progressed profile: level 5, a cleared stage, extra silver, a sect.
- Press the button and confirm. Then:
  - the files are gone;
  - the game PlayerPrefs keys are gone and `ServerUrl` remains;
  - the Login scene is active.
- Log in again → a fresh profile: level 1, the 2 starter items, no first-clears, no sect.
- **Cancel** in the confirm popup changes nothing.

## Rules
- Edit only:
  - `Assets/_Game/Scripts/**`, except `Scripts/Engine/**`;
  - `Assets/_Game/Editor/**`;
  - `Assets/_Game/Tests/**`.
- No git.

## Result
- Every file and key the wipe deletes, and where each one was found.
- The changed files.
