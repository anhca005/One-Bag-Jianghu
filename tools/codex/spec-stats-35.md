# Spec: item stat lines and level-based HP (decision #35)

Shared by task S1 (Java) and task S2 (C#). Both engines must implement this **exactly**, with the same order of operations and the same integer truncation, because the client replays server logs and the parity goldens compare every event.

## Data: `ItemDef.stats` (optional)
```json
"stats": {"hp": 120, "dmgReduce": 0.05, "cooldownReduction": 0.10}
```
- Every key is optional. Missing keys and a missing `stats` object both mean 0. Unknown keys are rejected, like the other item fields.
- `hp`: an int ≥ 0. Floats are rejected, as for other int fields.
- `dmgReduce`, `cooldownReduction`: doubles in [0, 1]. They are converted to permille with the **same helper/rounding as `EffectDef.pctPermille()`**: `(int) Math.round(x * 1000)`. In C#, use the same expression the existing C# `pctPermille` uses.
- Values outside the ranges throw `DataException` at catalog load, with the item id in the message.
- `stats` is **not** part of `unlocks`; Đột Phá does not change stats.
- Data convention (not enforced by the engine): Hộ Cụ and Kỳ Trân use `hp`; Công Pháp uses `cooldownReduction`; Đan Dược uses `dmgReduce`. Binh Khí has no `stats`: its stat is its `attack` (Công).

## Config (`BattleConfig`), new defaults
| Field | Value |
|---|---|
| `tierMultiplierPermille` | `{1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400}` |
| `rarityMultiplierPermille` | `{1000, 1100, 1210, 1330, 1460, 1600}` |
| `baseHp` (new) | `100` |
| `hpPerLevel` (new) | `5` |
| `maxCooldownReducePermille` (new) | `400` |

All of these can be overridden with the existing battle-config JSON override.

## Loadout
- New field `level` (int, default 1). If `level < 1`, throw `DataException`.
- **The `hp` field is removed.** A loadout JSON that still has `hp` must fail with a clear message, for example `Loadout X: "hp" was removed (decision #35); use "level"`.

## Scaling
Per item instance, with `tm = tierMultiplier(tier)` and `rm = rarityMultiplier(rarity)`:

```
scale(x) = x * tm / 1000 * rm / 1000      // int math, truncate after each division (same as attack today)
```

Apply `scale` to:
- `stats.hp`;
- the permille value of `stats.dmgReduce`;
- the permille value of `stats.cooldownReduction`.

Attack, effect `value` and `stacks` keep using the same tier×rarity multipliers as today; only the tables change.

## Fighter creation (after all item instances exist, in bag order)
```
hpBonus  = Σ scale(stats.hp)                      over items in loadout order (int; use long if needed, then int)
maxHp    = cfg.baseHp + cfg.hpPerLevel * (level - 1) + hpBonus
hp       = maxHp
dmgReducePermille (starting value) = min(cfg.maxDmgReducePermille, Σ scale(dmgReducePermille))
cdr      = min(cfg.maxCooldownReducePermille, Σ scale(cooldownReductionPermille))
for each item with cooldownMs > 0:
    cooldownMs = max(cfg.tickMs, cooldownMs * (1000 - cdr) / 1000)
```

- `cooldownMs` is the item's cooldown after `atTier` and the sect `applyTo`, which is the value the instance gets today. The reduced value replaces it for the whole battle: progress, `charge` (Vận Khí) and the replay all use the reduced cooldown.
- Temporary `dmg_reduce` effects and `buff_stack dmg_reduce` keep adding on top of the starting value, capped by `maxDmgReducePermille` exactly as today.
- No new event types. The existing battle-start events and states already carry `maxHp`. If the result or log has a per-fighter summary, add `maxHp`, `dmgReducePermille` and `cooldownReductionPermille` there as optional output (the Java CLI `Breakdown` should print them).

## Loadout files in the repo
Rewrite every loadout JSON the task owns: `"hp": N` becomes `"level": 1`. Do not touch replay logs, where `hp` is event state, or files under `tools/gemini/out/`.
