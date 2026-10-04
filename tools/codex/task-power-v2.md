# Codex task: Lực Chiến v2 — offense × defense model, per-item measurement

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #51, #53;
- `docs/reviews/2026-10-04-codex-power-calibration.md` (v1);
- `tools/balance/power.py` (v1), `tools/balance/balance.py`;
- `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/BattleConfig.java`;
- `docs/tech/code-style.md`.

Do not run Maven.

## Why v1 is wrong (Claude measured it)
The user's real bag at level 9: Thanh Phong Kiếm, Ô Tàm Y, Tử Mẫu Song Đao, Bồi Nguyên Đan and Kim Sáng Dược, all T1 common.

It fought stage `hoi_1_07` (Thiết Bì Trư Thú, level 8, 7 items):

| | Player | Enemy |
|---|---|---|
| v1 LC | 144 | 175 |
| v1 prediction | about 1% player win | |
| Engine, 100 runs | **player wins 57%** | |

Causes:
1. **Song Đao is underrated:** base 30.6, below Thanh Phong Kiếm's 37.8. In this fight it dealt 304 of the player's 423 damage. It rarely appeared in the v1 calibration bags.
2. **The additive sum overrates many weak items.** The enemy's 3 weak weapons plus small pills add up to a higher LC, yet deal less damage. Fights are a race of time-to-kill, so offense and defense should **multiply**.
3. **Fitted rarity factors are implausible:** uncommon 1.01 and rare 1.02, while the engine multiplies item stats by 1.10 and 1.21. Tier and rarity should follow the engine multipliers, not be fitted freely.

## Model (v2)
```
O = Σ offense[id] × statMult(tier, rarity)      # damage value of the bag
D = baseD(level) + Σ defense[id] × statMult(tier, rarity)   # survivability value
LC = round(scale × sqrt(O × D))
statMult = tierMultiplier[tier] × rarityMultiplier[rarity] (from BattleConfig, /1000)
```
- Every item has an `offense` and a `defense` value, both ≥ 0. Weapons are mostly offense; armor, pills and heals are mostly defense; some items have both.
- `baseD(level)` comes from the engine's base HP: 100 + 5 per level, unless the config says otherwise. Read it.
- Add a `synergy`/sect term **only** if the data clearly needs it, and document it. Sect is still ignored in v2.
- `scale` is chosen so that the starter bag (Thanh Phong Kiếm + Ô Tàm Y, T1 common, level 1) = 100.
- `power.json` gets a new shape. Keep `version: 2`, `recommendedFactor: 1.1`, and add `tierMultiplier`, `rarityMultiplier`, `levelBaseDefense`, `levelDefensePerLevel`, `scale`, `items: {id: {offense, defense}}`.

## Measuring per-item values
Measure every item, including monster items, directly. Keep the fight budget modest: **about 15,000 fights in total**.
1. **Reference fighters:** a few standard bags of different sizes, for example 2, 5 and 9 items, made from mid-value items at T1 common, at levels 1, 10 and 20.
2. **For each item X:** add X to (or swap it into) a reference bag, fight a fixed reference opponent at 30 runs, and record the win-rate change and the average fight length. Use these to estimate offense and defense: wins gained through shorter fights point to offense, wins gained through longer survival point to defense. A simple decomposition is fine; document it.
3. **Fit** `offense`, `defense` and `scale` so `sigmoid(k × ln(LC_A / LC_B))` matches a mixed matchup set. Include random bags with every item appearing at least 8 times. Measure with 20 runs.

## Validation (must report)
- **Held-out set:** 100 random matchups at 30 runs each, including user-style bags (random shop purchases at mixed rarities and levels). Report accuracy, log-loss and a calibration table by LC-ratio bucket, side by side with v1 on the same set.
- **The user's case** above must be predicted within ±15 points of the engine's 57%.
- **Outlier list:** items whose measured value per cell is more than 40% above the median of their type. Song Đao is suspected. Report them for a balance decision; **do not change items**.

## Output
- **`tools/balance/power.py` v2**, with a `lc()` that reads either version. CLI `--calibrate` and `--validate`, plus small run flags.
- **`tools/codex/out/power-v2/power.json`** and `tables/`. **Do not** overwrite the game's `power.json`; Claude integrates.
- **The C# side must follow:** write `tools/codex/out/power-v2/csharp-notes.md` describing exactly what `Scripts/Economy/PowerCatalog.cs` must change to read v2: formula, fields and rounding. Claude or a later Codex task applies it. **Do not edit Unity code.**
- **Report:** `docs/reviews/2026-10-04-codex-power-v2.md`, in Vietnamese, ≤ 120 lines:
  1. the method;
  2. the v1-vs-v2 validation;
  3. the user case;
  4. the top 10 and bottom 10 items by LC at T1 common;
  5. the outliers;
  6. the LC of the starter kit at levels 1, 10 and 20;
  7. PowerShell reproduction commands.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/power.py`;
  - `tools/codex/out/power-v2/**`;
  - the report.
- Keep the Python readable: ≤ 120 columns, one statement per line.
- **Be honest:** if v2 is not better than v1 on the held-out set, say so.

## Final message (`-o` output)
- v1 vs v2 accuracy and log-loss.
- The user-case prediction.
- The outliers.
- The reproduction commands.
