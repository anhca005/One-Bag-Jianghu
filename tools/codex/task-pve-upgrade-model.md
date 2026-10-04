# Codex task: progression model with tier upgrades (Bạc → materials → Trọng), then rebuild stages 15–50

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #52–#55;
- your reports `docs/reviews/2026-10-04-codex-pve-lc-shop-v2.md` and `docs/reviews/2026-10-04-codex-pve-hoi-4-5.md`;
- `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`, `tools/balance/power.py`;
- game data under `One-Bag-Jianghu/Assets/_Game/Resources/Data/`:
  - `upgrade_costs.json`: per rarity and tier, materials, catalysts and Bạc;
  - `materials.json`, `merge.json`, `shop.json` (material slots, `catalyst_or_tier3` slot, exchange, reputation
    offers), `items.json`;
  - `Pve/chapters.json`: first-clear materials per stage. **It now has 50 stages.**
- the upgrade and exchange code in `One-Bag-Jianghu/Assets/_Game/Scripts/Economy/` (read only), so the model matches
  the real rules;
- `docs/tech/code-style.md`.

Do not run Maven.

## Why
- **What both of your reports showed:** from about stage 20 the expected bag freezes at 18–19 items, LC about 850, and
  unspent Bạc climbs to 51.070 by stage 50.
- **What the user decided:** turning Bạc into power through Trọng upgrades is the answer.
- **The upgrade path already exists in the game:** upgrades cost Bạc plus materials plus catalysts, and the shop
  sells materials for Bạc. Your model skipped it. **Model it faithfully. Do not change game data.**

## 1. Upgrade model (`pve_progression.py`)
- **Inventory:** track materials by tier, and catalysts. Sources:
  - first-clear rewards;
  - shop material slots (Monte Carlo rolls as now);
  - the `catalyst_or_tier3` slot;
  - exchange, if the code allows converting materials.
- **Before each stage**, after or interleaved with buying items, spend Bạc on the action with the best LC gain per Bạc
  among:
  - buying an item offer;
  - buying a material pack;
  - upgrading one owned bag item one Trọng (needs its materials, catalysts and Bạc);
  - a paid refresh.
  - The value of a material pack is the LC of the upgrades it enables. A simple lookahead is fine; document it.
- **Respect the real rules:** the max Trọng, `upgrade_costs.json` per rarity, material tier per step, catalysts, and
  the shop's refresh limits/costs.
- **Expected bag:** the median of 9 seeds, as before. Report per stage:
  - the bag LC;
  - the average Trọng of the bag;
  - Bạc spent on items, materials, upgrades and refresh;
  - unspent Bạc;
  - the materials held.
- **Diagnosis if Bạc still piles up:** name the bottleneck (material supply per refresh, catalysts, tier-3 materials,
  max Trọng) with numbers. **Do not fix it by changing data.** Propose the smallest data change and Claude will ask
  the user.

## 2. Rebuild enemies for stages 15–50
- Stages 1–14 stay as in the game's current `chapters.json`. Copy them verbatim.
- For stages 15–50, rebuild enemy items with the same method:
  - bands: normal 62–82%, elite 52–72%, boss 40–60%;
  - tune on 20 runs, confirm on fresh seeds 51–100;
  - write `recommendedPower` from the expected bag.
- Enemy level: as now, capped at 30.
- **Keep untouched:** names, portraits, sect, dialogue, description and rewards.
- **Budget:** about 12,000 fights.

## Output
- `tools/codex/out/pve-upgrade/chapters.json`: the full 50-stage file.
- `tools/codex/out/pve-upgrade/tables/`.
- **Report:** `docs/reviews/2026-10-04-codex-pve-upgrade.md`, in Vietnamese, ≤ 150 lines:
  1. the upgrade model and its simplifications;
  2. a per-stage table for 1–50: bag items/LC/average Trọng, Bạc split (items / materials / upgrades / refresh /
     unspent), enemy LC, fresh-seed %, seconds;
  3. the out-of-band list;
  4. the bottleneck diagnosis and the proposed data change, if any;
  5. PowerShell reproduction commands.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`;
  - `tools/codex/out/pve-upgrade/**`;
  - the report.
- **Do not modify:** game data, Unity code, engine code or other docs.
- Keep the Python readable: ≤ 120 columns, one statement per line. Lint clean.
- **Be honest.**

## Final message (`-o` output)
- In band X/36 (stages 15–50), on fresh seeds.
- Unspent Bạc at stages 20, 30, 40 and 50.
- Bag LC and average Trọng at stages 20, 30, 40 and 50.
- The bottleneck and the proposed change.
- The reproduction commands.
