# Codex task: Lực Chiến catalog v2 formula + recommended power from the standard bag (decision #55)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/Economy/PowerCatalog.cs`, `Scripts/Pve/StagePower.cs`, `Scripts/Pve/PveCatalog.cs` (`PveStage.RecommendedPower`);
- the new catalog `tools/codex/out/power-hybrid/power.json` (read only) and the reference formula `lc()` in
  `tools/balance/power.py`;
- tests `Tests/EditMode/Economy/PowerCatalogTests.cs`, `Tests/EditMode/Pve/StagePowerTests.cs`.

## 1. PowerCatalog reads version 2 (and 3, same shape)
The new catalog has this shape (version 2):
```
version, recommendedFactor, tierMultiplier{tier: f}, rarityMultiplier{rarity: f},
levelBaseDefense, levelDefensePerLevel, scale, items{id: {offense, defense}}
```
- Keep the version-1 path working exactly as now (existing tests must still pass).
- Version ≥ 2, fighter power, same as `power.py lc()`:
  ```
  offense = Σ items[id].offense × tierMultiplier[tier] × rarityMultiplier[rarity]
  defense = levelBaseDefense + levelDefensePerLevel × (max(1, level) − 1) + Σ items[id].defense × same multiplier
  LC = round(scale × sqrt(max(0, offense × defense)))
  ```
  Rounding must match Python `round()` for normal values (use `MidpointRounding.ToEven` or document the difference).
- Unknown item ids: count as 0 offense/defense (same tolerance as v1 has now; check what v1 does and match it).
- **`ItemPower` in version ≥ 2** (shown in tooltips as "Lực Chiến +N"): the LC an item adds to the reference kit at
  level 1. The kit is data: add an optional top-level field `"referenceKit": ["thanh_phong_kiem", "o_tam_y"]`
  (T1 common). If missing, use that default.
  ```
  ItemPower(item) = Fighter(referenceKit + item, level 1) − Fighter(referenceKit, level 1), minimum 0
  ```
  If the item is itself in the kit, still add it a second time (the result is still the marginal value).
- Split into small private methods; the file should stay under about 200 lines.

## 2. StagePower.Recommended uses the stage's data value
- `PveStage.RecommendedPower` already exists in the model. New rule:
  `Recommended = stage.RecommendedPower if > 0, else round(Enemy × RecommendedFactor)` (old behaviour as fallback).
- Claude will write `recommendedPower` into `chapters.json` later. **Do not edit `chapters.json`.**

## 3. Data
- Copy `tools/codex/out/power-hybrid/power.json` to `Resources/Data/power.json`, adding
  `"referenceKit": ["thanh_phong_kiem", "o_tam_y"]`. No other value changes.

## Tests (EditMode)
- v2 synthetic catalog: fighter LC matches a hand-computed value (write the arithmetic in a comment).
- v2 `ItemPower` = marginal value over the reference kit; an armor gets > 0.
- v1 synthetic tests still pass unchanged.
- `StagePower.Recommended`: uses `RecommendedPower` when > 0; falls back to enemy × factor when 0.
- The real `Resources/Data/power.json` parses and the starter kit (Thanh Phong Kiếm + Ô Tàm Y, T1 common, level 1)
  = 100.

## Allowed files
- `Scripts/Economy/PowerCatalog.cs`, `Scripts/Pve/StagePower.cs`;
- `Resources/Data/power.json`;
- `Tests/EditMode/Economy/PowerCatalogTests.cs`, `Tests/EditMode/Pve/StagePowerTests.cs`.

**Do not touch `tools/**`, `chapters.json` or any other file**; another job is running there.

## Rules
- **No git.**
- Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`; it must be clean.

## Report (`-o` output)
- The files changed, the rounding choice, the lint result.
