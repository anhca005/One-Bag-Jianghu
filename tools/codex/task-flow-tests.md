# Codex task FL: PlayMode tests for the PvE result flows (missing from task HF)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Task HF (`tools/codex/task-home-result-flow.md`) implemented the flows, but section 3 (tests) was only partly done. Only the "Về bản đồ" flow is covered, in `DemoSmokeTests.LoginBattleResultAndReturnHome`.

Claude already fixed that test: at hero level 10, the one-time sect prompt opens on top of the map, so find the map with `FindFirstObjectByType<StageMapPopup>()`, not `PopupManager.Top`. Keep that in mind for every new test.

## Write (PlayMode, new file `Tests/PlayMode/PveResultFlowTests.cs`)
Reuse the helpers and the profile backup/restore pattern of `DemoSmokeTests`: copy the needed private helpers, or move them to a shared static test helper class. Use the deterministic winning bag (`CreateStageOneLoadout` + `HeroLevel = 10`).

1. **Continue (Ải tiếp theo):**
   - win `hoi_1_01`;
   - the result popup's second button reads "Ải tiếp theo" and is interactable;
   - press it → the before-dialogue of `hoi_1_02` opens;
   - Thể Lực has dropped by `hoi_1_02`'s `staminaCost`;
   - `GameBootstrap.Session.CurrentPveStageId == "hoi_1_02"`;
   - the gameplay opponent name equals the catalog enemy name of `hoi_1_02`;
   - let the battle finish (speed x4 / scrub to the end, as the existing test does) → a `ResultPopup` appears again.
2. **Retry (Đánh lại):**
   - force a loss on `hoi_1_01`, e.g. a 1-item bag at level 1 against the stage (check that it really loses; pick something that does);
   - the second button reads "Đánh lại";
   - press it → the stage is `hoi_1_01` again and its before-dialogue opens.
3. **Disabled when Thể Lực is short:**
   - after a win, set `Profile.Pve.Stamina = 0` before the result popup is set up, or win with exactly enough stamina;
   - "Ải tiếp theo" is not interactable and the reason "Không đủ Thể Lực" is visible.
4. **Home hotspot without a weapon:**
   - an empty bag;
   - clicking the PvE hotspot shows the "chưa có binh khí" message (`MessagePopup` or whatever HF uses) and does **not** open the map or start a battle.

Make the tests robust against the one-time level-10 sect prompt and the dialogue popups: skip dialogues via their skip button, and close the sect prompt if it is on top.

## Rules
- Edit only `Assets/_Game/Tests/**`. If a test reveals a real bug in the flow, **do not** change game code. Describe the bug in the result.
- No git. Do not run Unity; Claude runs it.

## Result
The test list, what each asserts, and any bug you suspect.
