# Codex task HF: Home without the centre buttons + PvE result choices (back to map / continue)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read:
- `docs/decisions.md` #21, #33, #39;
- `Scripts/UI/Home/HomeController.cs`, `HomeHotspot.cs`;
- `Scripts/UI/Gameplay/GameplayController.cs`;
- `Scripts/UI/Popups/ResultPopup.cs`;
- `Scripts/UI/Pve/StageMapPopup.cs`, `DialoguePopup.cs`;
- `Scripts/Pve/*`;
- the builders `Editor/Building/LoginHomeSceneBuilder.cs`, `PopupPrefabBuilder.cs`, `GameplaySceneBuilder.cs`;
- `Tests/PlayMode/DemoSmokeTests.cs`.

Builders only, never prefabs. **No git. Do not run Unity** (the license fails in your sandbox); Claude runs it.

## 1. Home: remove the two centre buttons
- Remove the big "Hành Tẩu Giang Hồ" and "Luận Kiếm Đài" buttons at the bottom centre of HomeScene (`pveButton`, `pvpButton`) from the builder and from `HomeController`.
- The scene hotspots already do the same job (`pveHotspot` → `StartPve`, `pvpHotspot` → `StartPvp`) and become the only entry points.
- The warning "Bao Phục chưa có binh khí" (currently under the centre buttons):
  - show it as a short message when the player clicks the PvE/PvP hotspot without a valid loadout (for example a toast or a `MessagePopup`), instead of a permanent label;
  - the hotspots stay clickable, so the player learns why.
- Remove any dead references, fields and tests tied to the removed buttons. `RefreshBattleAvailability` should still exist if tests or other code use it; adapt it to the hotspots.

## 2. PvE stage end: choose "Về bản đồ" or continue
Today the PvE result offers Home / Play again (`ResultPopupChoice`). For PvE stages, change it to:

| Result | Button 1 | Button 2 |
|---|---|---|
| Win | **Về bản đồ** | **Ải tiếp theo** |
| Loss | **Về bản đồ** | **Đánh lại** |

- **Về bản đồ:** return to HomeScene and **reopen the `StageMapPopup`** on the chapter of the stage just played, with the stage selected. Not just Home.
- **Ải tiếp theo:** start the next stage directly, with the same flow as pressing "Khiêu Chiến" on the map:
  - spend Thể Lực;
  - show the before-dialogue;
  - run the battle;
  - show the after-dialogue, and the rewards when won.

  The next stage is the next index in the chapter. After a chapter boss, it is stage 1 of the next Hồi when that Hồi is unlocked.
  - Disable the button, with a reason shown on it or under it, when:
    - there is no next stage (end of content): "Đã hết ải";
    - Thể Lực is not enough: "Không đủ Thể Lực";
    - the Bao Phục has no weapon.
- **Đánh lại:** retry the same stage. Same rules: Thể Lực, dialogue and so on.
- The after-win dialogue and the reward display still happen **before** this choice. Keep the current order of popups, and make sure two popups are never stacked.
- PvP (Luận Kiếm Đài) results keep the current Home / Play again choice. Rename the labels to Vietnamese if they are not already ("Về Home" is not OK; use "Về trấn" / "Đấu tiếp").

## 3. Tests (PlayMode): verify both flows
Extend or add PlayMode tests (style of `DemoSmokeTests`, profile backup/restore as there).

Use a deterministic winning bag: Gemini's stage-1 expected bag + hero level 10, as in `DemoSmokeTests.CreateStageOneLoadout`.

- **Flow A, back to map:**
  - Login → the Home PvE hotspot opens the map;
  - challenge stage 1 → dialogue → battle → win;
  - press "Về bản đồ" → back in HomeScene with the `StageMapPopup` open on Hồi 1, stage 2 now unlocked.
- **Flow B, continue:**
  - win stage 1;
  - press "Ải tiếp theo" → stage 2's before-dialogue opens, Thể Lực decreased by stage 2's cost, and the battle runs for `hoi_1_02` (check `CurrentPveStageId` and the opponent name from the catalog);
  - let it finish → a result popup appears again.
- **Loss path:** force a loss (an empty-ish bag, or a weapon only against a strong stage). "Đánh lại" restarts the same stage id.
- **Disabled case:** set Thể Lực to 0 after a win → "Ải tiếp theo" is not interactable and shows "Không đủ Thể Lực".
- **Home:** HomeScene has no `pveButton`/`pvpButton`; clicking the PvE hotspot with an empty bag shows the "chưa có binh khí" message and does not start a battle.

## Result
The changed files, the flow description, and anything that needs a visual check.
