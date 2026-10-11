# Gemini task: realistic reference bag, exploit check, and enemy kits round 5 (Hồi 2–5)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. You run Python tools and edit balance data. Vietnamese for reports.

## Read first
- `CLAUDE.md`, `docs/tech/code-style.md` (Python: one statement per line, readable), `docs/decisions.md` #52–#56;
- `tools/balance/pve_progression.py` (`select_bag`, `_place`, `bags_for_stage`: the expected player bag per stage),
  `tools/balance/power.py`, `tools/balance/pve_tune.py`;
- Codex's round-4 work: `tools/codex/task-pve-kits-round4.md`, `tools/codex/pve_enemy_variety_round4.py` (tuner,
  rule checks, report writer), `docs/reviews/2026-10-10-codex-pve-kits-round4.md`,
  `tools/codex/out/pve-kits-r4/verification.json`;
- data: `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`, `items.json`, `power.json`, `shop.json`.

Simulations call the Java engine through the existing Python helpers. **Do not run Maven.** Keep run counts low:
probe 20, confirm 40. Each `bags_for_stage` call simulates the whole economy and is slow: cache its results.

## Problem (found by Claude)
`select_bag` packs by Lực Chiến per cell, so the "expected player bag" is unrealistic. The bag grid is 5×4 = 20 cells:
- stage 20: 1 weapon, 3 armor, 4 manuals, 4 relics, 1 pill (13 items, 18 cells);
- stage 45: 1 weapon (2 cells), 10 manuals, 4 relics, 4 pills, **no armor** (19 items, 20 cells).

Rounds 2–4 tuned enemies against this bag, so their win rates do not reflect real play, and enemies were pushed to
odd kits (19 items, or tier-7 items among tier-2 neighbours). Round 4 used plausible kits but with far fewer items than
this bag, so the player won ~100% (only 10/49 in band; not applied).

## Do
1. **Exploit check first** (report it, do not change items or engine):
   - At stages 20, 35 and 45, build two player bags from the same owned items: (a) today's `select_bag` result;
     (b) the typical bag from step 2. Fight each against the stage's current enemy, 40 runs. Also fight (a) vs (b)
     directly, 40 runs.
   - If (a) clearly beats (b) (≥65% head-to-head), stacking 1×1 manuals is a balance exploit. Report which item types
     and stats cause it and suggest a fix (e.g. power or effect values in `power.json`/`items.json`), but **do not
     apply** balance changes to items.
2. **Typical reference bag** in `pve_progression.py` (user decision):
   - at least **2 weapons** when the player owns 2 or more;
   - at least **1 armor** when owned;
   - at most **3 manuals** and at most **3 pills**;
   - fill the remaining cells by the current Lực Chiến-per-cell order; same 5×4 grid and `_place` rules.
   - Keep the old behaviour available behind a flag (e.g. `bag_style="greedy"`) so old reports can be reproduced; the
     default becomes `"typical"`.
   - `pve_progression.py` already reads both `paidRefreshCost` (fixed 50) and the legacy `paidRefreshCosts`; keep that.
3. **Round 5 enemy kits, stages 11–50 only** (Hồi 2–5). **Hồi 1 (stages 1–10) is final: do not change it.**
   - Anchor each enemy to the typical bag at that stage: item count within ±3 of the player's count; average tier
     within ±1 of the player's average tier; rarities from the player's rarity set (bosses may use one step higher).
   - Keep all round-4 rules: full kit (weapon of the themed class + armor + support item); themed weapons; a weapon in
     at most 3 stages per chapter; no weapon in two consecutive stages; at most 2 copies of one weapon per enemy;
     Thanh Phong Kiếm at most once per chapter; beasts use beast hides/claws where items exist; placements fit the
     enemy's grid with no overlaps; no tier/rarity spikes versus neighbours (average tier may not drop by more than 1
     from one stage to the next within a chapter).
   - Targets (decision #55): normal 72% (60–84), elite 62% (50–74), boss 50% (38–62).
   - You may reuse/extend `tools/codex/pve_enemy_variety_round4.py` or write `tools/gemini/pve_kits_round5.py`.
4. **Output and apply:**
   - Candidate: `tools/gemini/out/pve-kits-r5/chapters.json`.
   - Gate: all rules hold, at least 36 of the 40 tuned stages in band, every boss of Hồi 2–5 in band.
   - If the gate passes: back up the current game file to `tools/gemini/out/pve-kits-r5/chapters-before.json`, then
     **write only `enemy.items` of stages 11–50 into the current game file**
     `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`. Do **not** copy the candidate file over it: the
     game file has new dialogue (all stages) and new Hồi 1 kits that must stay. Keep its formatting (2-space indent,
     CRLF, UTF-8, `ensure_ascii=False`).
   - If the gate fails: do not touch the game file; report.
5. **Report** `docs/reviews/2026-10-10-gemini-pve-kits-round5.md` (Vietnamese):
   - the exploit check results and your conclusion;
   - the typical bag at stages 15, 25, 35, 45 (items, cells, types);
   - one row per stage 11–50: enemy, item count, tier range, rarities, weapon, win rate, target, in band;
   - a rule-check section and whether it was applied.

## Rules
- **No git.** Do not edit C# code, `items.json`, `power.json`, dialogue, or Hồi 1.
- Write only: `tools/balance/pve_progression.py`, scripts under `tools/gemini/`, outputs under `tools/gemini/out/`,
  the review file, and (only when the gate passes) the enemy items of stages 11–50 in `chapters.json`.

When done, reply with: exploit yes/no and why, in-band count, applied or not, and the stages out of band.
