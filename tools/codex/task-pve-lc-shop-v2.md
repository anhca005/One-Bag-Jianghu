# Codex task: PvE shop-bag rebuild, round 2 (decisions #52, #55)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #52, #53, #55;
- your round-1 report `docs/reviews/2026-10-04-codex-pve-lc-shop.md` and `tools/codex/out/pve-lc-shop/`;
- `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`, `tools/balance/power.py`;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json` and the shop/upgrade code in
  `One-Bag-Jianghu/Assets/_Game/Scripts/Economy/` (read only), to model what Bạc can actually buy;
- `docs/tech/code-style.md`.

Do not run Maven.

## Problems Claude found in round 1
1. **Only 13/30 in band.** Most misses are 55–58% on normal stages, just below the band. LC is only a starting point:
   one ±2% retry is not enough.
2. **The shop model stalls at stage 19.** It always takes each slot's most likely rarity, so it never sees the rare or
   epic offers and never upgrades. The bag freezes at 18 items / LC 1.042 while Bạc piles up to 14.750 at stage 30.
   A real player would buy higher rarities and upgrades with that Bạc.
3. **Stage 1 is 0% for the starter kit.** A new player who goes straight to stage 1 cannot win.

## Changes
### 1. Shop model (`pve_progression.py`)
- **Monte Carlo instead of the most likely rarity.**
  - Roll the real slots from `shop.json`: rarity weights, the discount slot, paid refresh costs, and type price
    multipliers. Random item ids come from the item pool the game uses for shop offers; find it in the code.
  - Use a seeded RNG.
  - Before each stage, the player shops and refreshes while there is something worth buying and Bạc allows it.
    Reserve stays 0.
  - **Buy rule:** buy an offer when it raises bag LC. When the 5×4 bag is full, it may replace the item with the lowest
    LC per cell.
- **Upgrades:** if the game lets Bạc buy tier upgrades (material slots, exchange), include them in the same "best LC
  gain per Bạc" choice. If the upgrade rules are too complex to model faithfully, skip them, report how much Bạc
  stays unspent per stage, and say so.
- **The expected bag** for stage N is the run with the **median** bag LC out of 9 seeds.
  - Keep "starter-only" and "lucky" (75th-percentile seed plus K = 3 replays) for information.
- **LC everywhere** uses `tools/codex/out/power-hybrid/power.json` (version 2, read with `power.lc()`).
  - "LC per cell" for replacement = the item's marginal LC, by `power.lc()` difference, divided by its cells.
  - Remove any v1-only code path that reads `power["items"][id]` as a number.

### 2. Stage 1 is for the starter kit
- **Stage 1:** the target bag is the starter kit at level 1. Target 80%, band 70–90%.
- **Stages 2–30:** use the expected shop bag. Targets and bands are unchanged:

  | Kind | Target | Band |
  |---|---|---|
  | Normal | 72% | 62–82% |
  | Elite | 62% | 52–72% |
  | Boss | 50% | 40–60% |

### 3. Feedback loop (`pve_lc_build.py`)
For each stage:
1. Build the mirror enemy at the target LC, as now.
2. Measure 20 runs. If the result is outside the band, correct the enemy toward the target and measure again.
   - Correct by adjusting the target LC ratio from the measured win-rate error, or by swapping or upgrading one item.
   - Allow up to 5 correction steps.
3. Confirm with 50 runs.

- Keep the total under about 9,000 fights.
- **Keep untouched:** names, portraits, sect, dialogue, description and rewards.

### 4. Write `recommendedPower`
In the output `chapters.json`, set every stage's `recommendedPower` to the hybrid LC of its target bag:
- stage 1: the starter kit;
- other stages: the expected bag.

### 5. Fight length
Every fight in round 1 lasted over 50 s. **Report it only; do not change engine or config.** Add a column for the
expected-bag average seconds, and include a short note on whether bag size or mirror enemies cause the long fights.

## Output and report
- **Files:**
  - `tools/codex/out/pve-lc-shop-v2/chapters.json`;
  - `tables/` with verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-lc-shop-v2.md`, in Vietnamese, ≤ 120 lines:
  1. the per-stage table, with the same columns as round 1 plus `recommendedPower`, unspent Bạc and correction steps;
  2. the out-of-band list;
  3. the Bạc-curve note: does the bag keep growing through chapter 3 now?
  4. the fight-length note;
  5. PowerShell reproduction commands.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`;
  - `tools/codex/out/pve-lc-shop-v2/**`;
  - the report.
- **Do not modify:** game data, Unity code, engine code, `power.py`, `power_hybrid.py` or other docs.
- Keep the Python readable: ≤ 120 columns, one statement per line.
- Run `py tools/lint/check_style.py` on the two files; they must be clean.
- **Be honest.**

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The Bạc-curve note.
- The fight-length note.
- The reproduction commands.
