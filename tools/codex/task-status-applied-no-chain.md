# Codex task Q1: STATUS_APPLIED must not re-trigger itself, plus weaponClass validation

Repo root: `D:\CONG VIEC\One-Bag-Jianghu`. Read `docs/decisions.md` #32 and #34, then the engine (`server/battle-engine/src/main/java/com/onebagjianghu/battle/**`) and the C# engine (`One-Bag-Jianghu/Assets/_Game/Scripts/Engine/{Data,Simulation,Battle}.cs`).

## Rules
- Edit the Java engine and tests under `server/battle-engine/**`.
- The C# engine: your sandbox cannot write `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/**`. Write the full new files to `tools/codex/staging/Engine/` and list the copy commands in `tools/codex/staging/README.md`, replacing the old content of that README.
- You may write C# tests in `One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Engine/`.
- **No git writes.** Do not edit data JSON files or docs.

## Changes
1. **No self-chaining of STATUS_APPLIED (decision #34).** When effects run because a `STATUS_APPLIED` trigger fired, any status those effects apply must **not** fire `STATUS_APPLIED` triggers again, for any item on either side.
   - Other triggers (HIT, CRIT, BURST, …) fired from inside those effects still work normally.
   - A status applied by anything else (Phổ Công, Tuyệt Kỹ, other triggers, relic actives) still fires `STATUS_APPLIED` once, as today.
   - Implement it with explicit state in the trigger context (for example a flag carried in `Ctx`), not by depth tricks. Document it in the Javadoc of `TriggerEvent.STATUS_APPLIED`.
2. **weaponClass validation:**
   - The catalog must reject a weapon whose `weaponClass` is not one of the classes known to the game. Derive the known classes from the existing data (`axe, blade, bow, brush, claw, dual, fan, fist, flute, hammer, hidden, palm, spear, staff, sword, whip, zither`) as a constant, with a comment saying where to add new ones.
   - Non-weapons must not have a `weaponClass`.
   - Make C# reject the same cases.
3. Mirror both in C# exactly.

## Proof
- Java test: a catalog where item A has `on: status_applied → apply poison` and item B applies poison on hit. The hit fires A exactly once per application, never recursively. Assert the event count and the poison points.
- Java test: an unknown weaponClass is rejected with a clear message.
- Old goldens: `py tools/parity/make_golden.py --out <temp>` must be **identical** to `One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/Parity`. The rule must not change v1 data. If it does, stop and report which case and why.
- V2 goldens: regenerate with `py tools/parity/make_golden.py --items tools/gemini/out/items-v2/items-v2.normalized.json --out tools/codex/staging/ParityV2` (they may change: say which cases changed). Claude will copy them into `Tests/EditMode/Data/ParityV2`.
- `cd server && ./mvnw -q test` passes.
- Also run `py tools/balance/balance.py matrix --items tools/gemini/out/items-v2-balance-1/items-v2-balanced.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts --tiers 1,9 --tp 1 --runs 50` and paste its summary line, so the effect on Huyền Độc builds is visible.
