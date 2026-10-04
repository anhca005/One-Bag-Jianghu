# Codex task V: update the Google Sheets exporter to the current data

Repo root: `D:\CONG VIEC\One-Bag-Jianghu`. Read `CLAUDE.md`, `docs/decisions.md` (#25, #27, #28, #30, #31) and `docs/design/glossary.md` (section "Nâng cấp và nguyên liệu") first.

## Rules
- Edit ONLY `tools/sheets/export_xlsx.py` and `tools/sheets/gemini-import-prompt.md`; write the output workbook to `exports/One-Bag-Jianghu-data.xlsx`.
- Do not touch game data, engine code, Unity, or docs. **No git commands.**
- Sheet text is Vietnamese; code and identifiers are English. Keep the existing style: Arial, blue font = values copied from data, black = formulas, header fill, notes under titles. Game terms must follow the glossary.
- The sheet is a VIEW: every number must be read from the data/config files, never hardcoded in the script.

## What is out of date
1. The rarity table in "Hằng Số" is hardcoded (`RARITIES` list with 1.0/1.1/1.25/1.4/1.6/1.8). The engine now uses `BattleConfig.rarityMultiplierPermille = {1000,1060,1120,1190,1260,1340}` (decision #30). Read it from the engine config the script already loads through the CLI (`load_config`). If that JSON does not contain the field, read it from `BattleConfig.java` with a regex as a fallback. Keep the name/colour columns. Also show `shieldCapPermille` (Cương Khí cap = % of max Sinh Lực) and `maxDmgReducePermille` in "Hằng Số".
2. The final stat is baseline × Trọng multiplier × rarity multiplier, truncating at each step. Add a small table "Chỉ số cuối theo Trọng × Phẩm" (rows = Trọng 1..9, columns = 6 rarities) with formulas referencing the two multiplier tables. In the "Binh Khí" tab, add a column "Lực Đạo Chí Tôn T9" computed by formula from those tables.
3. There are no tabs yet for the economy data in `One-Bag-Jianghu/Assets/_Game/Resources/Data/`. Add these tabs, each with a title and a one-line note naming its source file:
   - **Nguyên Liệu** (`materials.json`): id, name, item type, band, tier, catalyst?, PvP only?, description.
   - **Nâng Trọng** (`upgrade_costs.json`): one row per rarity × fromTier with material tier, material count, catalyst count and Bạc; a total Bạc per rarity from T1 to T9 as a formula.
   - **Hợp Thành** (`merge.json`): from/to rarity, Bạc, extra materials (use material names from `materials.json`).
   - **Thương Hội** (`shop.json`): refresh settings, base prices per rarity, and one row per slot (type, rarities with weights, tier, quantity, price, discount). Include any other top-level sections the file has (for example the Danh Vọng shop or material exchange) as extra small tables.
   - **Phần Thưởng** (`rewards.json`): win / loss / first clear rows.
   - **Khởi Đầu** (`starter.json`): starting currencies, items (with names from items.json) and bag positions.
   Read the actual file structure. If a field appears that is not listed here, show it too rather than dropping it.
4. Check the other tabs against the current data: the "Đội Hình Mẫu" tab, "Hiệp Khách" and the "Đối Đầu" matrix should work with 20 samples. "Môn Phái" and "Hệ Binh Khí" should show `dual` under Liệt Diễm and `bow` under Hàn Sơn (from sects.json). Items have an optional `rarity`; show it in "Tất Cả Vật Phẩm" if present.
5. Update the "Hướng Dẫn" tab: its tab list and description rows must match the new tabs.
6. Update `tools/sheets/gemini-import-prompt.md`: the tab count and order, the row counts it asks Gemini to check (61 items, 20 samples, a 20×20 matchup table), and checks for the new tabs.

## Self-check (required)
- Run: `cd server && ./mvnw -q compile` then `py tools/sheets/export_xlsx.py --runs 200` from the repo root. It must finish without errors.
- Recalculate with LibreOffice if it is available (`soffice --headless --convert-to xlsx` into a temp folder), or otherwise load the workbook with openpyxl and verify that every formula references an existing sheet and cell range. Report any error cells. There must be zero `#REF!`, `#NAME?`, `#VALUE!` or `#DIV/0!` cells.
- Print the final tab list and the row count of each new tab in your result.
