# Codex task X2: Items v2, port the Java engine changes to C#

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Repo root: `D:\CONG VIEC\One-Bag-Jianghu`.

Task X1 (`tools/codex/task-items-v2-engine-java.md`, already done) added the decision #32 mechanics to the Java engine. The C# engine in `Assets/_Game/Scripts/Engine/` (`Data.cs`, `Simulation.cs`, `Battle.cs`) must match it **bit for bit**: the client replays and the parity goldens depend on it.

## Read first
- `docs/decisions.md` #32
- The Java diff of commit `26391d8` against `dce4c40` under `server/battle-engine/src/main/java` (`git diff dce4c40 26391d8 -- server/battle-engine/src/main/java`). Use git only to **read**.
- The current Java sources: `EffectDef`, `EffectOp`, `TriggerDef`, `TriggerEvent`, `TriggerSource`, `UnlockDef`, `ItemDef`, `ItemCatalog`, `Battle`, `Fighter`, `ItemInstance`, `BattleEvent`, `EventType`.
- The C# engine files and `Assets/_Game/Scripts/Engine/README.md`.

## Important: where to write
Your sandbox **cannot write** `Assets/_Game/Scripts/Engine/**`. Write the full new versions of the changed C# engine files to:

```
tools/codex/staging/Engine/Data.cs
tools/codex/staging/Engine/Simulation.cs
tools/codex/staging/Engine/Battle.cs
(any other engine file you change, same relative path)
```

Claude copies them over. Start from the current files and keep their style. They are deliberately compact, so match the existing density and naming; do not reformat untouched code.

You **may** write tests directly in `Assets/_Game/Tests/EditMode/Engine/` (C# NUnit, EditMode). Do not edit any other Unity file, and do not edit the Java engine, the data JSON files or docs. **No git writes.**

## What to port
Everything X1 added:
- `ItemDef.active`, `UnlockDef.active`, and active cooldowns for relics and manuals.
- The new triggers `DAMAGED`, `HEALED`, `CONTROLLED`, `CRIT_TAKEN`, `ENEMY_ULTIMATE`, `QI_EMPTY`, `SHIELD_BROKEN`, `BURST_TAKEN`, and `TriggerSource.ENEMY`.
- The new ops `BUFF_STACK` (`stat`, `maxStacks`), `CLEANSE`, `LIFESTEAL`, `RAISE_MAX_HP`, and `minQiAfter`.
- The catalog validation rules (unknown fields rejected, the same error cases).
- The new event types and fields.

Keep the **exact same order of operations, RNG calls and integer rounding** as Java. Where Java uses `long` or `Math.floorDiv`, mirror it. Where the existing C# uses `unchecked(...)` for Java int-overflow parity, keep doing so.

## Proof of parity (required)
1. Write `tools/codex/staging/README.md`. It lists every staged file and gives the exact copy commands.
2. Generate extra golden cases with the **Items v2** catalog, so the new mechanics are exercised.
   - Read `tools/parity/make_golden.py`. It may need an `--items <path>` and `--out <dir>` option. You may edit `tools/parity/make_golden.py` to add them (keep the default behaviour unchanged).
   - Write about 12 cases into `tools/codex/staging/ParityV2/` using `tools/gemini/out/items-v2/items-v2.normalized.json`. Pick loadouts that use relic actives, `buff_stack`, `cleanse`, `lifesteal`, `raise_max_hp`, `minQiAfter` and each new trigger. Build small loadouts if the samples do not cover them.
3. Add an EditMode test (for example `Tests/EditMode/Engine/ParityV2Tests.cs`) that runs the C# engine on those cases with that catalog and compares every event, exactly like the existing `ParityTests`. Reuse its helpers. The test may read the files from `tools/codex/staging/ParityV2/` through a path relative to the project for now: Claude will move them into `Tests/EditMode/Data/ParityV2/` when applying.
4. Add focused C# unit tests mirroring the Java tests for each new op and trigger.

## Self-check
Your sandbox probably cannot run Unity. Still:
- Make sure the staged C# compiles logically: no Java-only APIs, the same enum member order as Java where the ordinal matters, the Newtonsoft JSON attributes like the existing code.
- Run `cd server && ./mvnw -q test` and the golden generation, and report the results.
- In your result, list the risky spots: anything where the Java and C# semantics could differ (integer division of negatives, `Math.round`, list ordering, dictionary iteration order).
