# Gemini task: rebalance item stats so power scales with cell count (decision #61)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. You run Python balance tools and produce data patches. Reports in Vietnamese.

## Read first
- `CLAUDE.md`, `docs/tech/code-style.md` (Python readable, one statement per line);
- `docs/decisions.md` **#61** (this task), #51 and #55 (how Lực Chiến is measured), `docs/design/items.md`,
  `docs/design/combat.md`, `docs/design/glossary.md`;
- your report `docs/reviews/2026-10-10-gemini-pve-kits-round5.md` (the 1×1 Công Pháp exploit);
- tools: `tools/balance/power.py` (per-item measurement and calibration), `tools/balance/power_hybrid.py`,
  `tools/balance/balance.py`, `tools/balance/pve_progression.py` (now with the "typical" bag);
- data: `One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json` (73 items: stats, `size`, `basic`, `triggers`,
  `unlocks` per tier), `power.json`, and the engine copy `server/battle-engine/src/main/resources/data/items.json`.

Simulations use the Java engine through the existing helpers. **Do not run Maven.** Keep run counts low (≤30 per pair).

## Goal (user decision #61)
At the same Trọng and rarity, an item's real combat power is proportional to its cell count: a 4-cell item ≈ 4× a
1-cell item, for every type. Today 1×1 Công Pháp are far stronger per cell than weapons and armor.

## Do
1. **Measure today:** for every item at Trọng 1 and Trọng 6, common, measure its power with the existing method
   (`power.py`) and compute **power per cell**. Table by type and size. Show the outliers.
2. **Budget:** choose one power-per-cell budget (the median of weapons and armor is a good anchor, so the starter kit
   Thanh Phong Kiếm + Ô Tàm Y stays about where it is). Each item's target = budget × cells. Type roles stay
   (weapons deal damage, armor defends, pills heal or cleanse, manuals buff), only their strength changes.
3. **Rescale stats** to hit each item's target within ±10%:
   - change numbers only: `attack`, `cooldown`, crit, effect `value`/`pct`/`duration`, `maxStacks`, the numbers in
     `unlocks` for higher Trọng. Do not add or remove effects, do not change sizes, ids, names, types or text meaning;
   - prefer the fewest changes; one scale factor per item where possible;
   - re-measure after the change and iterate.
4. **Recalibrate `power.json`** with the existing calibration so Lực Chiến matches the new stats. Check that
   "Lực Chiến +N" of an item is now ≈ proportional to its cells (4-cell ≈ 4× 1-cell at equal Trọng/rarity).
5. **Verify:**
   - the round-5 exploit check again: greedy bag vs typical bag at stages 20, 35, 45 (≤30 runs). Goal: the
     head-to-head is between 35% and 65% (no single best packing);
   - the starter kit at level 1 still beats PvE stage 1–3 enemies (Hồi 1 is meant to be easy);
   - a few mixed bags of equal Lực Chiến land near 50% against each other.
6. **Output (do not overwrite game files):**
   - `tools/gemini/out/power-per-cell/items.json` (full file, same format and key order as the original, UTF-8,
     2-space indent as the original), `tools/gemini/out/power-per-cell/power.json`;
   - `tools/gemini/out/power-per-cell/changes.csv`: item id, field path, old value, new value, factor;
   - your scripts under `tools/gemini/`.
7. **Report** `docs/reviews/2026-10-10-gemini-power-per-cell.md`: the before/after power-per-cell table by type and
   size, the budget chosen, the biggest changes, the verification results, and anything that could not reach the
   target and why.

## Rules
- **No git.** Do not edit C# code. Do not write into `Resources/Data/` or `server/.../data/`: Claude reviews and
  applies the files from `tools/gemini/out/power-per-cell/` to both copies and syncs Google Sheets.
- Do not change enemy kits or `chapters.json`.

When done, reply with: the budget per cell, the before/after per-cell spread (min–max) by type, the exploit
head-to-head after the change, and how many items changed.
