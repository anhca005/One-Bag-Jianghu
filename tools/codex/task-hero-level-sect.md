# Codex task Q3: Hiệp Khách level system + sect choice at level 10

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read `docs/decisions.md` #21 (sect unlock at level 10), #33 and #34, `docs/design/gdd.md`, and the profile, reward and PvE code.

**Run after task Q2 (PvE art wiring) is done:** both touch `Scripts/UI`.

## Rules
- Edit `Assets/_Game/Scripts/**` except `Scripts/Engine/**`, plus `Assets/_Game/Editor/**`, `Assets/_Game/Tests/**` and `Assets/_Game/Resources/Data/levels.json` (new).
- Change the builders, not the prefabs. **No git.**

## Build
1. **Data:** `Resources/Data/levels.json`:

   ```json
   {"maxLevel": 30, "expToNext": [...], "unlocks": {"10": "sect"}}
   ```

   Use a placeholder curve until Gemini's `tools/gemini/out/round-2/levels.json` arrives. Validate it: the array length is `maxLevel - 1`, and every value is > 0.
2. **Leveling:**
   - All EXP grants go through one method, `PlayerProfile` or a `LevelService`: PvE first clear, repeat rewards, battle rewards. It adds EXP and levels up, possibly several levels at once.
   - At max level, EXP stops accumulating (or is capped; document which).
   - Log `[Profile] Level up 9 -> 10`.
   - Bump and migrate the profile version only if new fields are needed.
3. **UI:**
   - The Home top bar shows "Cấp N" and an EXP bar (it currently shows "Cấp 1" statically).
   - A level-up toast or popup appears after rewards.
4. **Sect choice:**
   - At level ≥ 10 with no sect chosen, the Home sect hotspot (it currently opens "Sắp ra mắt") opens a `SectSelectPopup`. It lists the 7 sects from `sects.json`: name, short description, weapon classes buffed, and Tâm Pháp level-1 effect. The user picks one and confirms; the choice is **permanent** for now.
   - It sets `profile.SectId` and `TamPhapLevel = 1`.
   - Below level 10, the hotspot shows "Đạt cấp 10 để gia nhập môn phái".
   - When the player reaches level 10, show a one-time prompt that points to the sect hotspot.
   - The battle uses the chosen sect and Tâm Pháp, as `sect` in the loadout. Before choosing, there is no sect.
5. **Cheat panel (F1):** +1 level, +EXP, reset sect.

## Tests
- EditMode:
  - EXP → level math, including multi-level jumps and the cap;
  - `levels.json` validation;
  - the sect cannot be chosen below 10 or twice.
- PlayMode: cheat to level 10 → the sect hotspot opens the popup → choose a sect → the next battle's player loadout has that `sect`.

## Self-check
Claude runs Unity afterwards. List anything you could not verify.
