# Codex task: rebuild PvE stages around the shop-bought bag (decision #52)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #47, #48, #51, #52;
- `docs/reviews/2026-10-04-codex-pve-lc.md`;
- tools: `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`, `tools/balance/power.py`;
- `docs/tech/code-style.md`.

Do not run Maven.

## What changed
Stages no longer give items: `rewards.firstClear.items` and `rewards.drops` are now empty in the game's `chapters.json`, and first-clear Bạc is +100. **Items come only from Thương Hội.**

The current enemy curve was built for a bag that grew by one reward item per stage, so it is now far too hard: about 12 items at stage 15, against a player who only has the starter kit plus purchases.

**Shop prices also changed:** weapons now cost 2× the rarity base price, through `shop.json` `"itemTypePriceMultipliers": {"weapon": 2.0}`. Types not listed use 1.0, and the slot discount applies after the multiplier. The model **must read this field**, so a common weapon costs 400 and common armor 200.

## 1. Progression model (`pve_progression.py`)
- **Owned items:** the starter kit plus purchases only. No reward items.
- **Bạc before stage N:** starter (500) + first-clear Bạc of stages 1..N−1 (K = 0 replays).
- **Shopping:** before each stage the player spends their Bạc in the shop, using the same shop model you already have (`shop.json` prices, slot rarities, refresh costs; expected best buy).
  - Purchases **accumulate** across stages.
  - When the 5×4 bag is full, a better item replaces the weakest by LC per cell.
  - The player keeps a reserve of 0.
  - Document any simplification in the docstring.
- **Bags returned by `bags_for_stage(n)`:**
  - **"expected" bag:** the shop bag above. This is now the main bag for the targets.
  - **"starter-only" bag:** starter kit, for information.
  - **"lucky" bag:** the expected bag plus Bạc from K = 3 replays of the previous stage (15% rule), also for information.
- Keep the CLI table and update its columns.

## 2. Rebuild enemies (`pve_lc_build.py`)
- Same method as before (mirror, then fine-tune to a target LC), but using the **expected (shop) bag**.
- **Target win rates for the expected bag:** normal 72%, elite 62%, boss 50%.
- **Verify bands** (50 runs): normal 62–82%, elite 52–72%, boss 40–60%. One ±2% retry, as before.
- **Also measure, 50 runs each, for information:**
  - the starter-only bag;
  - the lucky bag.
- **Keep untouched:** names, portraits, sect, dialogue, description and rewards. Rewards are already final.
- Start from the game's current `chapters.json`.

## Output and report
- **Output:** `tools/codex/out/pve-lc-shop/chapters.json`, plus `tables/` with verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-lc-shop.md`, in Vietnamese, ≤ 120 lines:
  1. a per-stage table: stage, kind, level, Bạc available, expected-bag items/LC, enemy LC and items, expected %, starter-only %, lucky %, seconds (flag > 40 s);
  2. the out-of-band list;
  3. a short note on the shape of the Bạc curve: is Bạc enough to keep the bag growing through chapter 3, or does it stall? Give the numbers;
  4. PowerShell reproduction commands.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`;
  - `tools/codex/out/pve-lc-shop/**`;
  - the report.
- **Do not modify** game data, Unity code, engine code or other docs.
- Keep the Python readable: ≤ 120 columns, one statement per line.
- **Be honest.**

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The Bạc-curve note.
- The reproduction commands.
