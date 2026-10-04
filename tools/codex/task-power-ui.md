# Codex task PW-B: "Lực Chiến" in the Unity client

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity; Claude builds, runs tests and takes snapshots.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `docs/decisions.md` #45, #51;
- `docs/design/glossary.md`.

## What it is (decision #51)
**Lực Chiến (LC)** is a single number that estimates a fighter's strength from the items in their bag and their level. It is a **display indicator only**: it never changes combat and never blocks entering a stage.

**Formula (data-driven):**
```
itemPower = items[id] × tierFactor[tier] × rarityFactor[rarity]
LC = round(Σ itemPower over the bag + levelPoints × (level − 1) + levelBase)
```

**Data file** `Resources/Data/power.json`:
```json
{
  "version": 1,
  "levelBase": 0,
  "levelPoints": 0.0,
  "recommendedFactor": 1.1,
  "tierFactor": { "1": 1.0, "2": 1.0 },
  "rarityFactor": { "common": 1.0, "uncommon": 1.0 },
  "items": { "thanh_phong_kiem": 50.0 }
}
```
- Another job is calibrating the real numbers; Claude will drop them in later.
- **Create a placeholder `power.json` now:**
  - every item id from `Resources/Data/items.json` with base 40 for weapons and 25 for everything else;
  - tierFactor `1 + 0.1 × (tier − 1)` for tiers 1..9;
  - rarityFactor 1.0 / 1.1 / 1.21 / 1.33 / 1.46 / 1.6;
  - levelBase 30, levelPoints 3, recommendedFactor 1.1.
- Unknown item ids count as base 25 and log one warning.
- Rarity string keys must match how rarity is serialised in the client data. Check `ItemCatalog` and the JSON converters.

## Part 1: logic
- `Scripts/Economy/PowerCatalog.cs`: loads `power.json` (pattern of the other catalogs) and computes `int ItemPower(OwnedItem or id/tier/rarity)` and `int Fighter(IEnumerable<items>, int level)`.
- `Scripts/Pve/`: helper `StagePower.Enemy(PveStage)` = LC of the stage enemy (items + level), and `StagePower.Recommended(PveStage) = round(Enemy × recommendedFactor)`.
- **Player LC** = items currently in the **battle loadout** (the bag used for fights; find how `GameSession` / the loadout stores it) + hero level.
  - Expose `GameSession.PlayerPower()` or a small service; do not duplicate loadout logic.

## Part 2: UI (number format `UiFormat.Number`; label text "Lực Chiến")
1. **Home banner** (`Editor/Building/LoginHomeSceneBuilder.cs` `BuildHomeScene`, `Scripts/UI/Home/HomeController.cs`):
   - a "Lực Chiến 1.234" label under or next to the hero name, without overlapping the EXP bar or the level seal. There is room at x≈180–500, y≈92–124 inside the 132 px header; check the current layout and keep it clean;
   - Title font, 20 pt, Vermilion;
   - refresh it in `Start`, after the bag popup closes, and when the level changes.
2. **Bao Phục** (`BagPopup` / `BagLoadoutPanel` and their builders):
   - show "Lực Chiến: N" near the bag title, updating live whenever the loadout changes (drag, drop, remove, swap);
   - find the existing change event; add one if there is none.
3. **Item tooltip** (`Scripts/UI/Common/ItemTooltip.cs` and its builder): one line "Lực Chiến +N" with that item's power, placed near the rarity/tier line.
4. **Stage info board** (`Scripts/UI/Pve/StageMapPopup.cs` and `Editor/Building/PvePrefabBuilder.cs`):
   - two lines, "Lực Chiến đề nghị: X" and "Lực Chiến của bạn: Y";
   - Y is coloured by ratio Y / X: **≥ 1.0 green** (an ink-friendly green, about `#4F7A3A`), **0.85–1.0 ochre gold** `UiKit.Gold`, **< 0.85 Vermilion**;
   - when Y / X < 0.85, the start button stays enabled; add a small warning line "Lực Chiến thấp hơn đề nghị".

Keep every existing serialized reference working. New labels get `SingleLineFit` (via `UiBuild.FitLabel(..., UiBuild.LabelFitPreset.None)` in builders) so they shrink rather than overflow.

## Part 3: tests (EditMode)
- **`PowerCatalogTests`:**
  - formula on a synthetic catalog;
  - tier and rarity factors applied;
  - unknown id falls back to 25;
  - level points.
- **`StagePowerTests`:** recommended = round(enemy × factor) for a synthetic stage.
- **Real-data test:** `power.json` contains every item id in `items.json`.
- Update existing UI tests if hierarchy assertions change.

## Allowed files
- New: `Scripts/Economy/PowerCatalog.cs`, `Scripts/Pve/StagePower.cs`, `Resources/Data/power.json`, tests under `Tests/EditMode/`.
- Edit:
  - `Scripts/Core/GameSession.cs` (a power accessor only);
  - `Scripts/UI/Home/HomeController.cs`;
  - `Scripts/UI/Popups/BagPopup.cs`;
  - `Scripts/UI/Common/BagLoadoutPanel.cs`;
  - `Scripts/UI/Common/ItemTooltip.cs`;
  - `Scripts/UI/Pve/StageMapPopup.cs`;
  - builders: `LoginHomeSceneBuilder.cs` (Home only), `PopupPrefabBuilder.cs` (Bag only), `BagPrefabBuilder.cs`, `CommonUiPrefabBuilder.cs` (tooltip only, if it is built there), `PvePrefabBuilder.cs`.
- **Do not touch:**
  - `Resources/Data/Pve/chapters.json`;
  - `tools/**`;
  - the LoginScene path;
  - any `.unity`, `.prefab` or `.asset` file by hand.

## Rules
- **No git.**
- UI text Vietnamese, code English.
- Run `py tools/lint/check_style.py <changed .cs files>` from the repo root. New and changed code must be clean; pre-existing violations in the big builders are out of scope.

## Report (`-o` output)
- The files changed.
- Where the loadout LC is computed from.
- Any assumption.
- The lint result.
