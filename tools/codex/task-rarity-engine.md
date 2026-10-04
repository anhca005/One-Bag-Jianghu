# Task J (E2b): per-copy rarity in the battle engine (Java + C#), 61-item data, parity ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`. Read `docs/decisions.md` #24 and **#25**, `docs/design/combat.md` (Trọng scaling), `docs/design/items.md`, `tools/parity/make_golden.py`, `tools/balance/README.md`.

## Why
Decision #25: rarity is a property of each owned copy. `items.json` holds **Phàm (common) baseline** stats; a copy's rarity multiplies its stats **exactly where the Trọng multiplier already applies** (attack/Lực Đạo and effect `value`/`stacks`; never qi, durations, costs, pct/ratios, cooldowns, crit). Final = baseline × tierMultiplier(tier) × rarityMultiplier(rarity).

## Hard rules
- Modify only: `server/battle-engine/**` (Java src, tests, `src/main/resources/data/items.json`, `samples/`), `tools/parity/`, `tools/balance/`, new `tools/data/`, and in Unity `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/**`, `Resources/Data/items.json`, `Resources/Data/Samples/**`, the parity golden files and `Tests/EditMode/Engine/**`. Do NOT touch `Scripts/Economy`, `Scripts/Core`, `Scripts/UI`, `Editor/`, scenes/prefabs (another agent works there in parallel).
- English, existing style (Java 17; C# 9, integer permille math identical on both sides — the C# engine must stay bit-for-bit equal to Java). No git. You cannot run Unity; you can run Maven (`server/battle-engine/mvnw` or the wrapper the repo uses) and Python (`py`).
- Known C# pitfall: CS0165 with `out` inside `&&` — declare the variable first.

## Build
1. **Config**: `BattleConfig.rarityMultiplierPermille` (Java) / `rarityMultiplierPermille` + `RarityMultiplier(Rarity)` (C#) = `{common 1000, uncommon 1250, rare 1550, epic 1900, legendary 2300, supreme 2750}`. Expose it in `--dump-config` like the tier array.
2. **Loadout**: each loadout item accepts optional `"rarity": "rare"` (lowercase, same enum as `Rarity`); missing → `common`. Invalid value → clear validation error. Everywhere the engine scales by tier (ItemInstance attack, `Scale(...)`/`value * tierMultiplier / 1000` in Battle, unlock/sect application if they scale) use the combined factor. Define the integer order once and use it on both sides, e.g. `value * tierPermille / 1000 * rarityPermille / 1000` — pick one, document it in `combat.md`-style comment, same in Java and C#.
3. **ItemDef.rarity**: no longer required and not used by the simulation (keep the field optional, meaning "lowest rarity it drops at"; default common). Update validation in Java + C# ItemCatalog.
4. **Data conversion script** `tools/data/convert_rarity_baseline.py` (deterministic, re-runnable):
   - Input: current `server/battle-engine/src/main/resources/data/items.json` (21 items, each with its old fixed `rarity`) and `tools/gemini/out/rarity/items_all.json` (61 items).
   - For the **21 existing items**: take them from the CURRENT items.json (NOT Gemini's hand conversion), divide `attack` and every effect `value`/`stacks` that the engine scales (in basic, triggers, ultimate effects, unlocks) by the old rarity multiplier, round to nearest int with minimum 1 for non-zero values. Leave qi (`gain_qi`, `drain_qi` — check in the engine whether these are scaled by tier; follow the engine), durations, pct, costs, cooldowns, crit untouched. Set `rarity` to `common`.
   - For the **40 new items** (ids in items_all.json not in the old file): copy from items_all.json as-is.
   - Output: new items.json (61 items) written to the server data file AND `One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json` (keep the same formatting style as the current file). Print a table: id, old rarity, old attack → baseline attack, and a check column "baseline × old multiplier ≈ old" (flag differences > 5% caused by rounding).
   - Existing **sample loadouts** (`server/battle-engine/samples/*.json`, Unity `Resources/Data/Samples/**`): add `"rarity": "<old fixed rarity of that item>"` to each item entry so samples keep their current power.
5. **Engine validation**: the 61-item file must load in Java and C# (all ops/triggers already exist). Fix nothing in item designs; if an item fails validation, report it.
6. **Parity**: regenerate goldens with `tools/parity/make_golden.py` (include at least 4 cases with mixed rarities incl. `supreme`, and cases using several of the 40 new items). Unity parity tests must use the new goldens.
7. **Balance tool**: `balance.py` passes rarity through from loadouts; add `--rarity <r>` to force every item in every loadout to one rarity (for "all at X" matrices). Update README.
8. **Tests**: Java unit tests (rarity multiplies attack/shield/heal/stacks, not qi/duration; missing rarity = common; invalid rarity rejected; sample loadouts reproduce their pre-change results within rounding — compare win rates of `balance.py matrix` before/after on the samples at T1/T9 and report the numbers). C# EditMode tests mirroring the Java ones.

## Self-check
`mvnw test` green; run `py tools/balance/balance.py matrix --tiers 1,9 --runs 60` before and after (report both); run the conversion twice → identical output.

Finish with: files created/changed, the conversion table, before/after matrices, anything unsure.
