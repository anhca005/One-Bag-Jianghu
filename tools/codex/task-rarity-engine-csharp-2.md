# Task J3: apply the C# engine rarity mirror (files are writable now)

Repo root `D:\CONG VIEC\One-Bag-Jianghu`. Tasks J/J2 finished the Java side, data, goldens and `Tests/EditMode/Engine/RarityTests.cs`, but could not write `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/{Data.cs,Simulation.cs,Battle.cs}` (ACL lock). The files have been recreated and are writable now.

Do only the "Do" section of `tools/codex/task-rarity-engine-csharp.md` (skip the rounding part, it is done): mirror the Java rarity change exactly in those three files (read the Java sources in `server/battle-engine/src/main/java/.../sim/{BattleConfig,Loadout,Fighter,ItemInstance,Battle}.java` and `data/{ItemDef,ItemCatalog}.java`), so that `RarityTests.cs` and the existing parity tests (regenerated goldens) pass bit-for-bit. Modify only `Scripts/Engine/**` (and `Tests/EditMode/Engine/**` only if a test must be adjusted to the real API — say why). English, C# 9, existing style. No git. You cannot run Unity; re-read your code for compile errors (CS0165: never `out` inside `&&` when the variable is used later).

Finish with: files changed, anything unsure.
