# Task J2: finish per-copy rarity in the C# engine (mirror of the Java change)

Repo root `D:\CONG VIEC\One-Bag-Jianghu`. Task J (`tools/codex/task-rarity-engine.md`) is done on the Java side: `server/battle-engine` (`BattleConfig`, `Loadout`, `ItemInstance`, `Fighter`, `Battle`, `ItemDef`, `ItemCatalog` + tests), 61-item data, samples with rarity, regenerated parity goldens. The C# files could not be written at the time (file lock), so the Unity engine is now behind the goldens.

## Do
- Mirror the Java change exactly (read the Java diff/source first) in `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/Data.cs`, `Simulation.cs`, `Battle.cs`: `rarityMultiplierPermille` {1000,1250,1550,1900,2300,2750} + `RarityMultiplier(Rarity)`, optional loadout item `rarity` (missing = common, invalid = validation error), same tier-first integer truncation order as Java, `ItemDef.rarity` optional (default common) and unused by the simulation, catalog validation updated.
- C# EditMode tests in `Tests/EditMode/Engine/` mirroring the Java rarity tests. Existing parity tests must pass against the regenerated goldens (bit-for-bit equal to Java).
- Modify only `Scripts/Engine/**` and `Tests/EditMode/Engine/**`. English, C# 9, existing style. No git. You cannot run Unity; check your code carefully by reading (CS0165: never `out` inside `&&` — declare first).

## Also: better baseline rounding (Java data side, then goldens)
Round-tripping the 21 old items (baseline × old rarity, truncated like the engine) loses a little: e.g. `liet_diem_dao.unlocks[0].basic[0].stacks` 2 → 1, `tu_kim_trao` attack 8 → 7, `phuc_ho_con` 200 → 199. Change `tools/data/convert_rarity_baseline.py` to pick, for each scaled number, the integer baseline `b ≥ 1` minimising `|floor(b × m / 1000) − old|` (m = old rarity permille; ties → the larger b). Re-run it (both items.json files), print the round-trip table again (old, baseline, back), re-run `mvnw test`, regenerate the parity goldens with `tools/parity/make_golden.py`, and report `py tools/balance/balance.py matrix --tiers 1,9 --runs 60`. For this part you may also modify `tools/data/`, both `items.json` files and the golden files.

Finish with: files changed, anything unsure.
