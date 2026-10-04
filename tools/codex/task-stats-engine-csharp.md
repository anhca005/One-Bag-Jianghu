# Codex task S2: C# engine + Unity wiring, item stat lines and level-based HP (decision #35)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Repo root: `D:\CONG VIEC\One-Bag-Jianghu`.

Read:
- `docs/decisions.md` #35;
- `docs/design/combat.md`, section "Chỉ số Hiệp Khách";
- **`tools/codex/spec-stats-35.md`, the exact contract.**

Task S1 implements the same spec in Java **in parallel**. Do not read or wait for S1. Follow the spec literally, mirroring the existing Java↔C# parity style (`Rules.Round`, `unchecked`, truncating integer division). Claude regenerates the parity goldens after both tasks, so any difference shows up there. Under "Spec interpretations" in your result, list every choice the spec did not pin down.

## Where to write
- **Engine:** your sandbox cannot write `Assets/_Game/Scripts/Engine/**`. Write the full new versions of the changed engine files to `tools/codex/staging/Engine/` (same file names: `Data.cs`, `Simulation.cs`, `Battle.cs`, …). Start from the current files and keep their compact style. Also update `tools/codex/staging/README.md` with the copy commands.
- You may edit:
  - `Assets/_Game/Scripts/**` except `Scripts/Engine/**`;
  - `Assets/_Game/Tests/**`, **except** `Tests/EditMode/Data/Parity/` and `Tests/EditMode/Data/ParityV2/` (Claude regenerates those);
  - `Assets/_Game/Resources/Data/Samples/*.json`;
  - `Assets/_Game/Resources/Data/Pve/chapters.json`.
- Do not edit the Java server, docs, or the `items.json` copies. **No git.**

## Build
1. **Engine (staging):** the spec (`ItemDef.stats`, `BattleConfig` tables and new fields, `Loadout.level` replacing `hp` and rejecting `hp`, the `Fighter` creation math, the cooldown reduction).
2. **Unity wiring** (every hard-coded 1200 goes):
   - `GameSession.BuildEngineLoadoutJson` and `BagLoadoutPanel.ToJson` use `LoadoutSerializer`, which takes an `hp` argument today. Change it to `level` and pass `profile.HeroLevel`.
   - `LocalBattleService`, `PveBattleService`: the player `level` = `HeroLevel`.
   - PvE enemy:
     - `PveCatalog.PveEnemy` replaces `hp` with `level` (int, default 1).
     - The enemy loadout gets `level`.
     - Placeholder `chapters.json`: `"hp": N` becomes `"level"`, rising gently across stages (Hồi 1: 1–5, Hồi 2: 8–15, Hồi 3: 18–25).
     - Update the chapters.json format note in `PveCatalog` validation if any.
   - `GameplayController` stars: compute from the player's real `maxHp` in the battle log (the first-event state, or whatever the log carries), not 1200.
   - `Economy/RarityRules.cs` duplicates the tier multiplier table. Make it read the engine `BattleConfig` defaults, or match them exactly (tier and rarity), so the UI shows the same scaled numbers as the battle. Search for any other copy of the old tables (`1060`, `1340`, `1550`) outside the engine and fix it.
3. **Samples:** `Resources/Data/Samples/*.json` and `Tests/EditMode/Data/loadout_*.json`: `"hp": N` becomes `"level": 1`. Leave the replay logs alone.

## Tests (EditMode)
Mirror the Java tests the spec implies:
- stat parsing and validation;
- `maxHp` by level, tier and rarity, including truncation;
- the starting Hộ Thân cap, plus a temporary `dmg_reduce` on top;
- cooldown reduction: cap, `tickMs` floor, Vận Khí;
- `hp` in a loadout is rejected.

Also cover:
- `BuildEngineLoadoutJson` writes `level` = `HeroLevel` and no `hp`;
- a PvE enemy loadout gets its level from `chapters.json`.

Existing tests that hard-code 1200 or the old multipliers: recompute the expected values. Do not delete or weaken them.

## Self-check
- You cannot run Unity. Make sure the staged code compiles logically: no Java APIs, Newtonsoft attributes like the existing code, and enum order unchanged.
- Result:
  - changed files (staging vs direct);
  - "Spec interpretations";
  - the parity risk spots: rounding, the order of the Σ, the cooldown floor;
  - anything not verified.
