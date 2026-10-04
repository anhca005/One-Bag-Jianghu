# Codex task PW-A: calibrate the "Lực Chiến" (combat power) formula with the engine

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #50, #51;
- `tools/balance/balance.py`, `tools/balance/pve_progression.py`, `tools/balance/pve_tune.py` (measure helper);
- `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/BattleConfig.java`;
- `docs/tech/code-style.md`.

Do not run Maven; `balance.ensure_classpath()` works.

## Goal
Produce one number, **Lực Chiến (LC)**, for a fighter (bag items + level), such that **equal LC ≈ 50% win**, and a higher LC wins more often in a predictable way. It is a display indicator only; it never changes combat.

## Formula (fixed shape; you fit the numbers)
```
itemPower(item) = base[item.id] × tierFactor[item.tier] × rarityFactor[item.rarity]
LC = round( Σ itemPower(items in bag) + levelPoints × (level − 1) + levelBase )
```
- `base[id]`: one value per item id in `server/battle-engine/src/main/resources/data/items.json`, covering **all** items including monster items.
- `tierFactor` covers tiers 1..9 and `rarityFactor` covers the six rarities. Both must be monotonic non-decreasing; tier 1 = 1.0 and common = 1.0.
- Sect and Tâm Pháp are **ignored** in v1. Note in the report how much the enemy sect field shifts results, if you can see it cheaply.

## Calibration method (keep the fight budget modest)
1. **Matchups.** Generate about **400 matchups** of plausible bags:
   - start from `pve_progression.bags_for_stage(n)` for n = 1..30, both bags;
   - make variants: swap one item for another of the same type, change one item's rarity or tier by one step, remove or add one item, change the level by ±2;
   - mix pairs from nearby stages so that LC differences cover roughly −40%..+40%.
   - Use deterministic seeds.
2. **Measure** each matchup with **20 runs**: about 8,000 fights, one JVM call per matchup.
3. **Fit:** `P(A wins) = sigmoid(k × ln(LC_A / LC_B))`, minimising log-loss.
   - Use simple coordinate descent or `scipy` if available. Pure Python is fine.
   - Regularise each `base` toward its type mean, so items seen rarely stay sensible.
   - Start from a stat-based initial guess: weapons from attack ÷ cooldown, and so on.
4. **Validate** on 80 **new** matchups at 20 runs each (held out):
   - log-loss;
   - accuracy of "higher LC wins";
   - a calibration table with LC-ratio buckets (≤0.8, 0.8–0.9, 0.9–1.0, 1.0–1.1, 1.1–1.25, ≥1.25) → predicted vs actual win %.
5. **Scale:** a starter bag (Thanh Phong Kiếm + Ô Tàm Y, T1 common, level 1) should come out at about **100 LC**. Pick `levelBase` and scale everything so this holds.

## Output: data file (exact format; the Unity task depends on it)
Write `tools/codex/out/power/power.json`; Claude copies it into the game:
```json
{
  "version": 1,
  "levelBase": 0,
  "levelPoints": 0.0,
  "recommendedFactor": 1.1,
  "tierFactor": { "1": 1.0, "2": 0.0, "...": 0.0, "9": 0.0 },
  "rarityFactor": { "common": 1.0, "uncommon": 0.0, "rare": 0.0, "epic": 0.0, "legendary": 0.0, "supreme": 0.0 },
  "items": { "thanh_phong_kiem": 0.0, "...": 0.0 }
}
```
- Numbers are rounded to 2 decimals.
- `recommendedFactor` stays 1.1. It is a display multiplier, documented in the report.

Also write:
- `tools/balance/power.py`: a reusable module plus CLI.
  - `lc(fighter_dict, power_json) -> int`.
  - CLI `py tools/balance/power.py <loadout.json>` prints the LC.
  - CLI `--calibrate` reruns the fit; flags `--matchups 400 --runs 20`.
- `tools/codex/out/power/tables/`: verbatim fit and validation output.

## Report
`docs/reviews/2026-10-04-codex-power-calibration.md`, in Vietnamese, ≤ 120 lines:
- the method;
- the fitted factors;
- the top 10 and bottom 10 item bases;
- the validation table;
- **the LC of the expected no-shop and shop-smart bags for every stage 1–30.** This is the "Lực Chiến curve";
- PowerShell reproduction commands.

## Hard rules
- **No git.**
- **Write only:**
  - `tools/balance/power.py`;
  - `tools/codex/out/power/**`;
  - the report.
- **Do not modify** game data, Unity code, engine code or other docs.
- Another Codex job is writing `tools/balance/pve_mirror.py` and `tools/codex/out/pve-mirror/`. **Do not touch them.**
- `py tools/lint/check_style.py tools/balance/power.py` must be clean.

## Final message (`-o` output)
- Validation accuracy and log-loss.
- The calibration table.
- The reproduction commands.
