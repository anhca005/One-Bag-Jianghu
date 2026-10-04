# Codex task: update the Google Sheets export (Lực Chiến, new PvE stages, replay rewards)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `tools/sheets/export_xlsx.py`, the tool to extend;
- `tools/sheets/gemini-import-prompt.md`;
- `docs/decisions.md` #47, #48, #51;
- `docs/tech/code-style.md`.

**Data**
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/power.json`;
- `Resources/Data/Pve/chapters.json`, which has new enemies plus `repeatRewardPercent`;
- `Resources/Data/starter.json`, now 500 Bạc;
- `tools/balance/power.py`, which has `lc()`.

## Changes to `export_xlsx.py`
Follow the file's existing conventions: Arial; blue font for values copied from data; black for formulas; a header fill; a note row explaining the tab.

1. **New tab "Lực Chiến"**, placed after "Cấp Độ".
   - Header block:
     - the formula in words;
     - `levelBase`, `levelPoints`, `recommendedFactor`, each in its own labelled cell;
     - a `tierFactor` table (T1..T9) and a `rarityFactor` table (6 rarities), as blue inputs.
   - Item table, one row per item in `power.json`: id, Vietnamese name, type (Vietnamese type name as in the other tabs), base (blue).
   - Formula columns for "LC T1 Phàm" and "LC T9 Chí Tôn" that reference the factor cells (`=base × tierFactor × rarityFactor`), so edits recalculate.
   - Sort by type, then base descending.
   - The note row says the numbers are calibrated by simulation (decision #51) and that the game reads `power.json`. The sheet is a view.
2. **"Hành Tẩu" tab:** add these columns, keeping the existing ones:
   - "Lực Chiến quái": a value computed in Python with `power.lc`, written blue with a note that it is computed by the tool;
   - "Lực Chiến đề nghị": a formula, quái × `recommendedFactor` from the Lực Chiến tab, rounded;
   - "Số món quái";
   - "Đánh lại: Bạc" and "Đánh lại: EXP": formulas `=ROUND(firstClear × percent / 100, 0)` with a minimum of 1 when first clear > 0. Use `MAX`. The percent comes from one labelled input cell holding `repeatRewardPercent`.
3. **"Khởi Đầu" tab:** make sure it reads `starter.json` (500 Bạc), not a hard-coded value.
4. **"Hướng Dẫn" tab:** update the tab list and counts.
5. **`gemini-import-prompt.md`:**
   - update the tab count and order;
   - update the item count. Count the items in `items.json`; it is probably 73, not 61;
   - add checks for the new tab and the new columns.
   - Keep the rest of the prompt.

**Do not add new spill or array functions.** Use only `ROUND`, `MAX`, `INDEX`, `MATCH`, `SUMPRODUCT`, `IFERROR` and plain arithmetic, so LibreOffice recalc works.

## Run it
The engine is compiled; do not run Maven. If the tool insists on Maven, reuse `server/battle-engine/target/cp.txt` the way `tools/balance/balance.py` `ensure_classpath()` does.

```
py tools/sheets/export_xlsx.py --runs 100 --out exports/One-Bag-Jianghu-data.xlsx
```
Use `--runs 100` for the matchup tab, as the user asked to keep fight counts low.

Then recalculate and check for formula errors:
```
py "C:/Users/ADMIN/.claude/skills/synced/fab78c6b-c6d1-4953-82b1-053e10e22f93_633faca3-1609-4aae-8504-610cb3fdc922/xlsx/scripts/recalc.py" exports/One-Bag-Jianghu-data.xlsx 90
```
- If `recalc.py` cannot run (no LibreOffice), say so.
- Otherwise there must be **0 formula errors**.
- Check 2–3 formula cells by hand against expected values. For example, stage 1-01 replay Bạc = 30, and starter LC = 100.

## Rules
- **No git.**
- **Write only:**
  - `tools/sheets/export_xlsx.py`;
  - `tools/sheets/gemini-import-prompt.md`;
  - `exports/One-Bag-Jianghu-data.xlsx`.
- **Do not modify** game data, code or docs.
- If `check_style.py` does not check `.py` files, keep the style by hand: ≤ 120 columns, one statement per line.

## Report (`-o` output)
- The tab list.
- The new columns.
- The recalc result: formula count and errors.
- The hand-checked cells and their values.
