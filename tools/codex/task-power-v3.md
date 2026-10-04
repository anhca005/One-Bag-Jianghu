# Codex task: Lực Chiến v3 — measure offense and defense directly from engine stats

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #51;
- `docs/reviews/2026-10-04-codex-power-v2.md`;
- `tools/balance/power.py` (current v2 code), `tools/balance/balance.py`;
- the battle CLI output format: run one fight and read what it prints;
- `server/battle-engine/src/main/java/com/onebagjianghu/battle/cli/BattleCli.java`;
- `docs/tech/code-style.md`.

Do not run Maven.

## Why v2 was rejected (Claude checked `tools/codex/out/power-v2/power.json`)
- **16 weapons share identical values** (offense 32.40, defense 1.71). The "add one item and watch the win rate" measurement saturated, so v2 treats almost all weapons as equal.
- **Song Đao was set by hand to 140** to fit the user's fight. No code produces that value. **Never hand-set per-item numbers.** Every value must come out of the measurement code.

## Ground truth Claude measured (use it as a validation case, not for fitting)
- **Setup:** the user bag at level 9 is Thanh Phong Kiếm, Ô Tàm Y, Bồi Nguyên Đan, Kim Sáng Dược, plus one extra weapon in the 2×2 slot at (3,0), all T1 common. It fights `hoi_1_07` from the game's `chapters.json`, 60 runs.
- **Results:**

  | Extra weapon | Player win % |
  |---|---|
  | Băng Phách Thương | 63 |
  | Tử Mẫu Song Đao | 60 |
  | Chấn Nhạc Chùy | 60 |
  | Khai Sơn Phủ | 47 |
  | Liệt Diễm Đao | 23 |
  | none | 0 |

- **Song Đao is not an outlier** among 4-cell weapons.
- **v3 must rank these six correctly** (Spearman ≥ 0.8) and predict each within ±20 points.

## Model
```
LC = round(scale × sqrt(O × D))
```
- **O (offense):** the bag's damage per second against a standard target. Measure it from the engine's per-item damage output, not from win rates.
  - **Item offense value:** for each item, at T1 common, fight a fixed "training dummy" opponent: high HP, no damage or very low, so fights last a fixed long time. Record the average damage the item deals per second, including its status bursts and ultimate. The CLI already prints per-item damage per fight and the fight length.
  - For items with no direct damage (manuals, relics that buff), measure the bag's total DPS change when the item is added to a standard weapon bag.
- **D (defense):** effective survivability against a standard attacker. Use engine stats where printed (maxHp, Hộ Thân, heal per fight, Cương Khí per fight).
  - **Item defense value:** for each item, fight a fixed "standard attacker" with a known DPS, and record the extra damage absorbed, healed or shielded per fight compared with a base bag.
  - Level contributes base HP (from config).
- **Tier and rarity:** multiply per-item O and D by the engine multipliers (`BattleConfig`), as v2 does.
- **Fitted:** only `scale` and the logistic slope `k` in `P = sigmoid(k × ln(LC_A / LC_B))`, on a mixed matchup set. **No per-item fitting.**
- The starter bag at level 1 = 100.

## Budget
Keep it modest: **at most about 8,000 fights in total.** Dummy and attacker measurements need few runs, since they are close to deterministic; 10–20 runs each is fine.

## Validation (must report)
- **Held-out set:** 100 random user-style bags at 30 runs each, reusing v2's validation set file if it exists so the comparison is apples to apples. Report v1, v2 and v3 accuracy and log-loss.
- **The six-weapon ground-truth table above:** predicted vs actual for each, and the Spearman correlation.
- **Top and bottom items:** the 10 highest and 10 lowest items by LC at T1 common, plus a per-type median table.
- **Outliers:** items more than 40% above their **same size and type** median. Report them only.

## Output
- **`tools/balance/power.py`:** v3 support (`version: 3`). `lc()` must read v1, v2 and v3. The CLI keeps `--calibrate` and `--validate`, plus `--measure` to rerun only the per-item measurements.
- **`tools/codex/out/power-v3/power.json`**, with the same top-level fields as v2 and `version: 3`, plus `tables/`.
- **`tools/codex/out/power-v3/csharp-notes.md`:** the exact changes `Scripts/Economy/PowerCatalog.cs` needs to compute v3.
- **Report:** `docs/reviews/2026-10-04-codex-power-v3.md`, in Vietnamese, ≤ 120 lines.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/power.py`;
  - `tools/codex/out/power-v3/**`;
  - the report.
- **Do not modify** game data, items, Unity code or engine code.
- Keep the Python readable: ≤ 120 columns, one statement per line.
- **Be honest:** if v3 is not better than v1 and v2, say so.

## Final message (`-o` output)
- v1, v2 and v3 accuracy and log-loss.
- The six-weapon table and the Spearman correlation.
- The outliers.
- The reproduction commands.
