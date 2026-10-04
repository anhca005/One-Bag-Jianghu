# Lệnh cho Gemini (Antigravity): tạo thêm đội hình mẫu

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, rồi dán toàn bộ phần dưới dấu `---`.

---

You are helping on "One Bag Jianghu", a 2D wuxia backpack auto-battler. Battles are simulated by a Java engine in `server/battle-engine`. We need MORE sample loadouts (bag layouts) so balance testing is not overfitted to the current 5.

## Hard rules
- Only create files inside `server/battle-engine/samples/gemini/` (create the folder) plus one report file `docs/reviews/2026-09-27-gemini-loadouts.md`.
- Do NOT modify any other file (especially `items.json`, Java code, existing samples). Do NOT run git.
- Do not stop to ask questions; make reasonable choices and note them in the report.

## Read first
- `server/battle-engine/src/main/resources/data/items.json`: all 14 items. Only these ids exist. `size` is [width, height] in cells (default [1,1]). Items cannot be rotated.
- `server/battle-engine/samples/*.json`: the 5 existing loadouts (format reference).
- `docs/design/combat.md`, `docs/design/items.md`, `docs/design/balance.md`: rules, the item table (including Đột Phá at tiers 3/6/9) and the balance goals.

## Loadout format
```json
{ "name": "Kim Cang Tự - Thiết Bích (Côn + Hộ Cụ)", "hp": 1200, "cols": 5, "rows": 4,
  "items": [ { "id": "phuc_ho_con", "x": 0, "y": 0 } ] }
```
- Bag is always 5 columns x 4 rows, hp always 1200. `x, y` = top-left cell, origin top-left, y grows down.
- An item covers [x, x+w) x [y, y+h); it must stay inside the bag and must not overlap any other item.
- Adjacency (shared edge, not corner) matters: `tuy_tien_ho_lo` only buffs weapons touching it.
- Duplicates of the same item are allowed.
- Use 16–20 of the 20 cells.
- Leave `tier` out (defaults to 1).

## What to create
**10 new loadouts**, file names `<sect>_<variant>.json` (e.g. `kim_cang_thiet_bich.json`):
- At least one new loadout for each of the 5 sects already present (kim_cang, thai_thanh, liet_diem, huyen_doc, thiet_phu), each with a DIFFERENT play pattern than the existing sample of that sect (e.g. defensive vs aggressive, ultimate-heavy vs no-ultimate, status build vs raw damage).
- At least 3 hybrid loadouts that mix two sects' items.
- At least 1 loadout with NO ultimate weapons (parry-heavy, relying on free Hóa Kình), and 1 that is all-in on ultimates.
- Names in Vietnamese, format `"<Môn phái> - <tên lối chơi> (<vũ khí chính>)"`. Invent short wuxia-style play-pattern names; do not use names from Kim Dung novels.

## Validate
From the `server` folder, in a terminal (Git Bash or PowerShell; on PowerShell use `.\mvnw.cmd` and run `chcp 65001` first for Vietnamese output):
```
./mvnw -q -pl battle-engine exec:java -Dexec.args="--matrix battle-engine/samples/gemini --runs 200"
```
An invalid loadout makes the command print "Lỗi: ..." with the reason; fix and rerun until the matrix prints.
Then build the full comparison: create `server/battle-engine/samples/gemini/all/`, copy the 5 original samples and your 10 new files into it, and run:
```
./mvnw -q -pl battle-engine exec:java -Dexec.args="--matrix battle-engine/samples/gemini/all --runs 200"
```

## Report (`docs/reviews/2026-09-27-gemini-loadouts.md`, in Vietnamese)
1. Table: file, name, cells used, weapons, play pattern in one sentence.
2. Both matrix outputs (new only, and all 15) as code blocks, and each loadout's average win rate in the 15x15 run.
3. Observations: which items appear in almost every strong loadout, which items never help, any loadout with average > 65% or < 35%, and your suspected cause (cite numbers from items.json).
4. Anything you were unsure about.

Do not propose changes to items.json in files; only describe suggestions in the report.
