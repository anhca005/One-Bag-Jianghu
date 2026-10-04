# Codex task P1: Hành Tẩu Giang Hồ system (chapters, stage map, dialogue, progress)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Repo root: `D:\CONG VIEC\One-Bag-Jianghu`.

Read first:
- `CLAUDE.md`
- `docs/design/pve.md`: the plan and the **data format**, which is the contract
- `docs/decisions.md` #16, #26, #29, #33
- `docs/tech/ui-flow.md`
- The existing UI code (`Scripts/UI/**`, `PopupManager`, `SceneFlow`, `GameSession`, `PlayerProfile`, `RewardService` or whatever grants rewards today)
- The scene and prefab builders in `Editor/Building/`

## Rules
- You may edit/create:
  - `Assets/_Game/Scripts/**` except `Scripts/Engine/**`
  - `Assets/_Game/Editor/**`
  - `Assets/_Game/Tests/**`
  - `Assets/_Game/Resources/Data/Pve/**`
  - `tools/balance/balance.py` and `tools/balance/README.md`
- `Scripts/Engine/**` is read-only for you. If you need a change there (for example `LocalBattleService` taking a stage enemy instead of a random sample), write the full new file to `tools/codex/staging/Engine/Unity/<file>.cs`. Another task (X2) is staging `Data.cs`, `Simulation.cs` and `Battle.cs` there, so do **not** touch those three. Explain in your result.
- Do not edit `items.json`, `sects.json`, the samples or docs. **No git.**
- Generated prefabs and scenes come from the builders: change the builders. UI text is Vietnamese with glossary terms; code is English. Log with `Debug.Log` tags, adding `[Pve]`.

## Build
1. **Data**
   - `PveCatalog` loads `Resources/Data/Pve/chapters.json` in the pve.md format and validates it: unique ids; valid item ids against the item catalog; enemy bags inside 5×4 with no overlaps; dialogue speakers exist; theme keys present.
   - Provide a small **placeholder** `chapters.json`:
     - 3 chapters × 3 stages, using existing items and the existing `enemy_portrait_*` and `bg_battle` art;
     - Vietnamese placeholder names marked "(tạm)".

     Gemini will replace it with the real content later, so the format must be exact.
2. **Profile** (bump the version and migrate safely): per stage, cleared and best stars; Thể Lực current value and last regen time; the daily first-clear tracking you need. Thể Lực values (max, regen per minute) go in `Resources/Data/Pve/stamina.json`.
3. **Stage map screen.** Choose a scene or a full-screen popup and explain why. It shows:
   - The chapter painting from `theme.mapBackground`, with a fallback when the image is missing.
   - Stage nodes at `mapPos`, with different visuals for normal, elite and boss, and for locked / available / cleared (with 1–3 stars).
   - A chapter switcher: tabs or arrows, with locked chapters greyed out.
   - Colours from `theme.accent`, `theme.paper` and `theme.ink` applied to the header, frames and buttons, so each Hồi looks different.
   - A Thể Lực display.
   - Clicking a node opens a stage panel: name, description, enemy portrait, enemy name and sect, recommended power, rewards preview, Thể Lực cost, and a "Khiêu chiến" button that is disabled with a reason when locked or out of Thể Lực.
4. **Dialogue.**
   - A `DialoguePopup` with portrait, speaker name and text (typewriter effect, click to skip or advance), plus a "Bỏ qua" button.
   - The portrait sits on the left for the hero and on the right for others.
   - Flow: the `before` dialogue, then the battle, then `afterWin` or `afterLoss` on the result screen, then rewards.
5. **Battle integration.**
   - Home's PvE entry (the path hotspot and the PvE button) opens the stage map instead of starting a random fight.
   - The battle uses the stage enemy: its loadout, hp, sect, Tâm Pháp, portrait key and name.
   - The battle screen uses `theme.battleBackground`, with a fallback to `bg_battle`.
   - The top bar shows "Hồi N · <stage name>".
   - PvP keeps the old random-sample behaviour.
6. **Results and rewards.**
   - Stars come from the hero's remaining HP %, as in pve.md.
   - First-clear rewards (silver, EXP, materials, items) go into storage. Respect the "Tàng Bảo Các không đủ chỗ" overflow rules that already exist.
   - Repeat clears use `rewards.json`. Drops are rolled with a seeded RNG.
   - Unlock the next stage or chapter.
   - Thể Lực is spent when a battle starts, not refunded on a loss.
7. **Shop.** Thương Hội must not offer the item ids listed in `monsterItems`.
8. **Dev cheat panel (F1):** unlock all stages, refill Thể Lực, and clear the current stage with 3 stars.
9. **Balance tool:** add `py tools/balance/balance.py vs --a <folder> --b <folder> [--items ...] [--sects ...] --runs N`.
   - It runs every loadout in A against every loadout in B, **keeping each item's own `tier` and `rarity` from the JSON** (the `matrix` command overrides them).
   - It prints a win-rate table with draws counted as half.
   - Document it in `tools/balance/README.md`. Reuse the Java CLI the tool already calls.

## Tests
- EditMode:
  - `PveCatalog` validation (good file, plus each error case)
  - star calculation
  - Thể Lực regen and spend
  - unlock rules
  - profile migration from the previous version
- PlayMode:
  - Home → stage map opens; locked nodes cannot start.
  - Start stage 1: the before-dialogue shows; skip it; the battle runs with the stage enemy (assert the opponent name).
  - After the result, the progress is saved and stage 2 is unlocked.
  - The chapter theme colour is applied (compare the header colour for chapter 1 and chapter 2).
- `vs` command: a quick self-run on two small folders, pasted into your result.

## Self-check
Claude runs Unity afterwards. Your sandbox probably cannot run Unity, so write the tests anyway and list which are unverified. Run the `vs` command and `cd server && ./mvnw -q test` yourself.
