# Codex staging handoff

The complete C# engine files are staged under `Engine/`. From the repository root,
copy them into the Unity project with PowerShell:

```powershell
Copy-Item -LiteralPath tools/codex/staging/Engine/Data.cs -Destination One-Bag-Jianghu/Assets/_Game/Scripts/Engine/Data.cs
Copy-Item -LiteralPath tools/codex/staging/Engine/Simulation.cs -Destination One-Bag-Jianghu/Assets/_Game/Scripts/Engine/Simulation.cs
Copy-Item -LiteralPath tools/codex/staging/Engine/Battle.cs -Destination One-Bag-Jianghu/Assets/_Game/Scripts/Engine/Battle.cs
Copy-Item -LiteralPath tools/codex/staging/Engine/Unity/LocalBattleService.cs -Destination One-Bag-Jianghu/Assets/_Game/Scripts/Engine/Unity/LocalBattleService.cs
```

`Simulation.cs` also exposes `BattleSimulator.FighterStats(loadout)`, used by the
Bao Phục UI to show the exact battle-start totals (including caps and truncation).

The regenerated Items v2 parity fixtures are staged under `ParityV2/`. Copy them
with:

```powershell
New-Item -ItemType Directory -Force One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/ParityV2 | Out-Null
Copy-Item -Path tools/codex/staging/ParityV2/*.json -Destination One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/ParityV2
```
