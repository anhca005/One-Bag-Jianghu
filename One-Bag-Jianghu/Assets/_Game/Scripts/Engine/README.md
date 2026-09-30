# Offline battle engine

This is the C# port of the authoritative Java battle engine under
`server/battle-engine/src/main/java/com/onebagjianghu/battle`.

`OneBagJianghu.Engine` has no UnityEngine dependency. Unity resource loading and
opponent selection live under `Unity/` in the game assembly.

Parity is the contract: identical data, loadouts, config, and seed must produce
identical JSON events.

Any change to the Java engine must be mirrored here and the golden cases regenerated:
`py tools/parity/make_golden.py`

Run Unity EditMode tests and select `OneBagJianghu.Tests.Engine.ParityTests`.
It reads every `Tests/EditMode/Data/Parity/case_*.json` and compares all fields.
