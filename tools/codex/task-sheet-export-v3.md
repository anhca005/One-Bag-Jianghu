# Codex task SH: update the data sheet exporter for Items v2 + the #35 stat system + PvE

Repo root: `D:\CONG VIEC\One-Bag-Jianghu`. Read `CLAUDE.md`, `docs/decisions.md` **#35**, #33, #34 and #32, `docs/design/combat.md` ("Chỉ số Hiệp Khách"), `tools/codex/spec-stats-35.md`, `docs/design/pve.md` and `docs/design/glossary.md`. Then read `tools/sheets/export_xlsx.py` and the previous task `tools/codex/task-sheet-export-v2.md` (the conventions still hold).

## Rules
- Edit ONLY `tools/sheets/export_xlsx.py` and `tools/sheets/gemini-import-prompt.md`. Write the workbooks to `exports/`.
- Do not touch game data, engine code, Unity or docs. **No git.**
- The sheet is a VIEW: every number comes from data/config files or from formulas, never hardcoded. Keep the style:
  - Arial;
  - blue = values copied from data, black = formulas;
  - header fill, notes under titles;
  - Vietnamese text, glossary terms.

## What is out of date
1. **It breaks on the new loadouts.** The samples now have `level` instead of `hp` (#35), and "Hiệp Khách" reads `lo["hp"]`.
   - Show `level` and the **computed max Khí Huyết**. Get it from the engine, not a reimplementation: the Java `BattleSimulator.fighterStats(loadout)` returns maxHp, dmgReducePermille and cooldownReductionPermille, and the CLI `Breakdown` prints them. Use whichever the script can call; add the smallest CLI hook in `tools/sheets` only (not in `server/`). If impossible, compute with formulas in the sheet from the "Hằng Số" tables, and say so in your result.
   - Also show Hộ Thân % and Giảm Hồi Chiêu %.
2. **"Hằng Số":**
   - Drop "Khí huyết tham chiếu 1200".
   - Add `baseHp`, `hpPerLevel`, `maxCooldownReducePermille`, `maxDmgReducePermille`, `shieldCapPermille`, burst damages, `exhaustionStep` and `maxRegen`, all read from the engine config.
   - The tier and rarity tables must come from the config: they are now 1.00→1.40 and 1.0/1.3/1.7/2.2/2.8/3.5.
   - Add a small "Khí Huyết cơ bản theo cấp" table (levels 1–30) using formulas `= baseHp + hpPerLevel × (cấp − 1)` that reference the constant cells.
3. **Stat line (#35)**, in every item tab and in "Tất Cả Vật Phẩm":
   - Add a "Chỉ số" column with the type's stat: Công = `attack` for Binh Khí; `stats.hp` for Hộ Cụ and Kỳ Trân; `stats.cooldownReduction` % for Công Pháp; `stats.dmgReduce` % for Đan Dược. Leave it empty when the data has none.
   - Add "Chỉ số T9 Chí Tôn" as a formula over the multiplier tables, with the same truncation as the engine (`INT` after each step).
   - Rename/replace the old "Lực Đạo Chí Tôn T9" column accordingly.
4. **Items v2 support.** Item JSON may have:
   - `active` + `cooldown` on relics and manuals;
   - triggers `damaged, healed, controlled, crit_taken, enemy_ultimate, qi_empty, shield_broken, burst_taken`, and `from: enemy`;
   - ops `buff_stack` (`stat`, `maxStacks`), `cleanse`, `lifesteal`, `raise_max_hp`, and `minQiAfter`.

   `describe_effect` / `describe_trigger` must render all of them in Vietnamese, in the same wording style as the existing ones and as the Unity `ItemDescriber` (`One-Bag-Jianghu/Assets/_Game/Scripts/UI/**/ItemDescriber.cs`; read it for wording). No raw op names in the sheet. Add an "Chiêu chủ động" column (active + cooldown) where relevant.
5. **New CLI options**, each defaulting to the current game files:
   - `--items` (default: the game `items.json`)
   - `--sects`
   - `--battle-config` (a partial override JSON, like the balance tool)
   - `--loadouts` (a folder)
   - `--chapters` (default `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`)
   - `--levels` (default `.../Resources/Data/levels.json`)

   When `--items` is not the game file, the "Hướng Dẫn" tab must state which files were used.
6. **New PvE tabs:**
   - **Hành Tẩu** (`chapters.json`):
     - one row per stage: Hồi, ải, name, type (normal/elite/boss), Thể Lực cost, enemy name, enemy `level` (or `hp` for an old file), enemy sect, the enemy items with tier/rarity, and the rewards;
     - a small table per chapter with its theme keys;
     - the `monsterItems` list with item names.
   - **Cấp Độ** (`levels.json`):
     - level, EXP to next, cumulative EXP (formula), unlock;
     - base Khí Huyết at that level (formula referencing "Hằng Số").
7. Update "Hướng Dẫn" (tab list and descriptions) and `tools/sheets/gemini-import-prompt.md` (tab count and order, row checks).

## Run (required)
1. `cd server && ./mvnw -q compile` (or `package -DskipTests` if the script needs the jar).
2. Default run, game data: `py tools/sheets/export_xlsx.py --runs 200` → `exports/One-Bag-Jianghu-data.xlsx`.
3. Preview run with the Items v2 catalog and Gemini's round-2 PvE content:

   ```
   py tools/sheets/export_xlsx.py --runs 100 \
     --items tools/gemini/out/round-2/items-v2-final.json \
     --chapters tools/gemini/out/round-2/chapters.json \
     --levels tools/gemini/out/round-2/levels.json \
     --out exports/One-Bag-Jianghu-data-v2-preview.xlsx
   ```

   The round-2 chapters use `hp` and the round-2 loadouts may too. The exporter must show `hp` columns gracefully for old files, not crash. For the matchup it uses the game samples (already `level`).
4. Check both workbooks:
   - Recalculate with LibreOffice if available, or verify with openpyxl that every formula references an existing sheet and range.
   - Zero `#REF!`, `#NAME?`, `#VALUE!` or `#DIV/0!`.
5. Result:
   - the tab list and row counts per tab for both files;
   - any value you could not get from the engine;
   - any op or trigger still rendered raw.
