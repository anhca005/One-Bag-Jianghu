# Codex task: PvE replay reward = 15% of first clear (decision #47)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity; Claude builds and runs the tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `docs/decisions.md` #47;
- `Scripts/Pve/PveRewardService.cs`;
- `Scripts/Pve/PveCatalog.cs`;
- `Resources/Data/Pve/chapters.json`;
- `Scripts/UI/Gameplay/GameplayController.cs` (around the `PveRewardService.Grant` call);
- the existing PvE tests under `Tests/EditMode/`.

## Rule
Winning a stage that was already cleared grants **15% of that stage's `rewards.firstClear`**.

- **Bạc and EXP:** `round(value * percent / 100)`, using `MidpointRounding.AwayFromZero`. The minimum is 1 when the first-clear value is greater than 0.
- **Materials:** for each entry, `exact = count * percent / 100`.
  - The player gets `floor(exact)`.
  - They also get +1 with probability `exact - floor(exact)`.
  - Roll it with a deterministic RNG derived from `seed`, using the same LCG style as `RollDrops` but a different salt. Use one roll per material, in a **stable order** (sort keys ordinally).
  - Entries that roll 0 are omitted.
- **First-clear `items`:** not granted on replay.
- **Stage `drops`:** unchanged; `RollDrops` still runs for first clear and replay alike.
- **Percent comes from data:** a new top-level field `"repeatRewardPercent": 15` in `chapters.json`, parsed by `PveCatalog`.
  - Default to 15 if absent.
  - Validate 0..100.
  - Keep the per-stage `"repeat": "default"` field and its validation as they are.
- **The PvE replay no longer uses `RewardCatalog` / `rewards.json`.**
  - If the `repeatCatalog` constructor parameter becomes unused, remove it and update every caller (`GameplayController`, tests).
  - Do **not** change `rewards.json` or `RewardService`; PvP still uses them.
- **Storage-full preflight and rollback** must behave exactly as before.

## Allowed files
- `Scripts/Pve/PveRewardService.cs`, `Scripts/Pve/PveCatalog.cs`;
- `Resources/Data/Pve/chapters.json`: **only** add the top-level field. Edit the text minimally and keep the existing formatting;
- `Scripts/UI/Gameplay/GameplayController.cs`: only the constructor call, if the signature changes;
- tests under `Tests/EditMode/` (new or existing PvE reward tests).

Another Codex job is editing the Settings/Home files at the same time. **Do not touch** `SettingsPopup`, `HomeController`, `GameSettings`, `GameBootstrap`, `PopupPrefabBuilder`, `LoginHomeSceneBuilder`, `ItemIconImporter` or `SceneSnapshot`.

**No git.** Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`; it must be clean.

## Tests (EditMode)
1. **Stage 1-01** (first clear 200 Bạc / 50 EXP / `han_thiet` 3): a replay gives 30 Bạc and 8 EXP (7.5 rounds away from zero). `han_thiet` is 0 or 1, never more, and the first-clear item is not given.
2. **Material expectation:** over 2000 seeds, a 3-count material averages 0.45 ± 0.05.
3. **Determinism:** the same seed gives an identical result.
4. **Minimum 1:** a first clear of 3 Bạc gives 1 on replay. Use a synthetic stage object, not the real data.
5. **The first-clear path** is unchanged.
6. **Catalog:** parses `repeatRewardPercent`, defaults to 15 when absent, and rejects -1 and 101.

## Report
Write to the `-o` output:
- the files changed;
- whether the constructor changed;
- the lint result;
- anything you were unsure about.
