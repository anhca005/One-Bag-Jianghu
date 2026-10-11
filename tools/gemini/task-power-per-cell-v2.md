# Gemini task (v2): rebalance item stats per cell, using Claude's in-bag measurement tool

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. You run Python tools and write candidate data. Reports in Vietnamese.

## Why a v2
Your v1 (`docs/reviews/2026-10-10-gemini-power-per-cell.md`) measured items **alone**. Support items look useless
alone, so the tool multiplied 1×1 Công Pháp by ~17× and some values by 100×: the opposite of the goal, since 1×1
Công Pháp were already too strong. The report's conclusions also contradicted its own numbers. **Do not reuse v1's
method or its candidate files.**

## Read first
- `docs/decisions.md` **#61**; `docs/tech/code-style.md` (Python readable, one statement per line);
- **the measurement tool you must use:** `tools/balance/item_value.py` (read its docstring). It adds one item to the
  typical player bag of stages 15/30/45 on a large grid, fights the unchanged bag from both seats, and reports
  `perCell` (anchor items Thanh Phong Kiếm + Ô Tàm Y average 1.0) and, for 1-cell items, `stackRatio`
  (per-copy value of 3 copies ÷ value of 1 copy).
- **the baseline:** `tools/balance/out/item-value/baseline.csv` (all 73 items, today's data).
- data: `server/battle-engine/src/main/resources/data/items.json` (the engine reads this one; the Unity copy
  `One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json` is identical).

Usage:
```
py tools/balance/item_value.py --items <candidate items.json> --out <csv> [--only id1,id2] [--runs 500]
```
Each full run takes ~10 minutes; use `--only` while iterating.

## Do
1. **Targets:** every item's `perCell` within **0.85–1.15**. For 1-cell items also `stackRatio` ≤ 1.1 (stacking must
   not grow faster than linear).
2. **Adjust numbers only**, in a copy at `tools/gemini/out/power-per-cell-v2/items.json`:
   - one scale factor per item, applied to its strength numbers: `attack`, effect `value` / `pct`, heal/shield
     amounts, status `duration`, stat bonuses; for `cooldown`, divide by the factor instead. Apply the same factor
     to that item's `unlocks` so higher Trọng keeps its shape;
   - round to the precision the field already uses (ints stay ints);
   - **per round, a factor stays within 0.7–1.4.** Re-measure, then adjust again. Expect 2–4 rounds;
   - never add/remove effects; never change `size`, ids, names, types, `maxStacks` caps or text.
   - If an item cannot reach the band without breaking its role, stop at the closest value and list it.
   - **Synergy items:** some items score near zero only because the base bags lack what they boost (e.g. poison
     relics such as Vạn Độc Đỉnh / Mãng Xà Độc Túi with no poison weapon; several share the exact same low value,
     e.g. 0.278 or 0.156 in the baseline). Do **not** buff those blindly. Measure them in a bag that contains their
     partner (add a fitting weapon to a copy of the base bag in your own script, reusing `Arena` from
     `item_value.py`), scale them on that number, and list them in the report as "synergy-measured".
3. **Exploit check** after the last round: rebuild the round-5 comparison with your candidate (greedy bag vs
   typical bag at stages 20, 35, 45, 30 runs each way, both seats). Goal 35–65%. Use `pve_progression.Progression`
   with `bag_style="greedy"` and `"typical"`, and pass your candidate items file to the engine.
4. **Starter check:** level-1 starter kit (Thanh Phong Kiếm + Ô Tàm Y) vs stage 1–3 enemies in
   `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`; report the win rates honestly.
5. **Lực Chiến:** write `tools/gemini/out/power-per-cell-v2/power.json`, a copy of today's
   `One-Bag-Jianghu/Assets/_Game/Resources/Data/power.json` where each item's `offense + defense` total equals
   `budget × cells` (budget = the anchors' average total per cell today), keeping each item's offense/defense split.
   Then "Lực Chiến +N" grows with cells by construction.
6. **Outputs:** the two JSON files, `changes.csv` (item, field path, old, new, factor), the final measurement CSV,
   and `docs/reviews/2026-10-10-gemini-power-per-cell-v2.md`: before/after `perCell` and `stackRatio` per item,
   per-type spread, the exploit and starter checks with the real numbers, and the items left outside the band.
   **Report numbers exactly as measured; do not claim a target is met unless the numbers show it.**

## Rules
- **No git.** Do not edit C# code. Do not write into `Resources/Data/` or `server/.../data/`: Claude reviews and
  applies. Do not change `tools/balance/item_value.py`; if it has a bug, describe it in the report.

When done, reply with: rounds run, items inside/outside 0.85–1.15 (count), worst stackRatio, the exploit numbers, and
the starter numbers.
