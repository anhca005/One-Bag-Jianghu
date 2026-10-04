# Task S: engine config override for balancing, shield cap, sect classes ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`. Read `docs/decisions.md` **#28**, #25, `docs/design/combat.md`, `server/battle-engine/src/main/java/.../sim/{BattleConfig,Battle,Fighter}.java`, `cli/BattleCli.java`, `tools/balance/{balance.py,README.md}`, `tools/parity/make_golden.py`, the C# mirror `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/{Simulation.cs,Battle.cs,Data.cs}` and `Tests/EditMode/Engine/*`.

## Do
1. **Config override for balancing**: `BattleCli --battle-config <json>` reads a JSON object whose keys are `BattleConfig` public fields (e.g. `rarityMultiplierPermille`, `tierMultiplierPermille`, `maxRegen`, `shieldCapPermille`) and overrides the defaults (unknown key → clear error). `balance.py` gets `--battle-config <json>` on `matrix`, `whatif`, `tune` and passes it through; `tune` config may also tune config fields with addresses `config:<field>[.<index>]` (e.g. `config:rarityMultiplierPermille.1`). Update the README with an example that sets rarity multipliers `[1000,1150,1320,1520,1750,2000]`.
2. **Rarity step check tool**: `balance.py rarity-step --loadouts <dir> --tier 3 --tp 1 --runs N [--battle-config f]` — for each loadout, fights the loadout with all items at rarity r+1 against itself at r, for r = common..legendary, and prints a table (rows: loadout, columns: Lương>Phàm … Chí Tôn>Thần Binh, value = win rate of the higher side, draws counted as half) plus the average per column. Also a column "Phàm T9 vs Lương T1".
3. **Shield cap (decision #28)**: Cương Khí can never exceed 100% of the fighter's current max HP: `BattleConfig.shieldCapPermille = 1000` (of maxHp); any shield gain above the cap is dropped (event value = actually gained). Java + C# identical.
4. **Sects**: in `sects.json` (server and `One-Bag-Jianghu/Assets/_Game/Resources/Data/sects.json`) add `dual` to Liệt Diễm's `weaponClasses` and `bow` to Hàn Sơn's.
5. Regenerate parity goldens (`tools/parity/make_golden.py`), add Java + C# tests (shield cap, config override parsing), `mvnw test` green. Report `balance.py matrix --tiers 1,9 --runs 100` on the default samples before/after (cap + sects change).

## Hard rules
- Modify only: `server/battle-engine/**` (src, tests, data), `tools/balance/**`, `tools/parity/**`, `One-Bag-Jianghu/Assets/_Game/Scripts/Engine/**`, `One-Bag-Jianghu/Assets/_Game/Resources/Data/sects.json`, `One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Engine/**` + the golden files. Another agent is working on Unity UI/Economy in parallel — do not touch anything else. The default `BattleConfig` values stay unchanged except the new shield cap.
- If a C# file in `Scripts/Engine` cannot be written (ACL "Access denied" happened before), stop and report it — do not work around it.
- English, Java 17 / C# 9, existing style. No git. You cannot run Unity.

Finish with: files changed, before/after matrices, anything unsure.
