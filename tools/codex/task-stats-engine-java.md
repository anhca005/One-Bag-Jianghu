# Codex task S1: Java engine, item stat lines and level-based HP (decision #35)

Repo root: `D:\CONG VIEC\One-Bag-Jianghu`. Read `docs/decisions.md` #35, `docs/design/combat.md` ("Chỉ số Hiệp Khách"), and **`tools/codex/spec-stats-35.md`, which is the exact contract**.

Task S2 ports the same spec to C# **in parallel** and does not read your code. Follow the spec literally: same order, same truncation. If the spec is ambiguous or conflicts with the code, choose the simplest reading, and write it down under "Spec interpretations" in your result, so Claude can align S2.

## Rules
- You may edit:
  - `server/**` (battle-engine sources, tests, `samples/`, test resources);
  - `tools/balance/**`;
  - `tools/parity/make_golden.py` (only if it builds loadouts with `hp`).
- Do not edit Unity files, docs, item data (`items.json` copies) or `tools/gemini/out/**`.
- **No git.**

## Build
1. Implement the spec in `ItemDef` and `ItemCatalog` (validation), `BattleConfig`, `Loadout`, `Fighter` and `ItemInstance`. Print the stats in `Breakdown` and the CLI.
2. Migrate the loadout JSONs: `server/battle-engine/samples/*.json`, server test resources, and the `tools/balance` loadouts if they have `hp`. `"hp": N` becomes `"level": 1`.
3. **`tools/balance/balance.py`:**
   - Add `--level N` to `matrix`, `vs`, `whatif`, `tune` and `rarity-step`. It overrides `level` on every loadout (both sides); without it, each loadout keeps its own level.
   - Add `--level-a` / `--level-b` to `vs`.
   - Update `tools/balance/README.md`.
4. Update `server/README.md` wherever it mentions `hp` in loadouts.

## Tests (JUnit)
- Stat parsing and validation: unknown key, float `hp`, values out of range, missing `stats` = 0.
- `maxHp` for level 1 and level 10, with and without Hộ Cụ `stats.hp`, at tier 1 vs tier 9 and Phàm vs Chí Tôn (check the scale truncation).
- The starting `dmgReduce` is capped at 800‰, and a temporary `dmg_reduce` stacks on top.
- `cooldownReduction` shortens a weapon's and a relic active's cooldown, capped at 400‰, never below `tickMs`. Vận Khí uses the reduced cooldown.
- A loadout with `hp` is rejected with the #35 message.
- Existing tests whose expectations depend on the old multiplier tables or 1200 HP: update the expected values. Do **not** weaken assertions; recompute them.

## Self-check
- Run `cd server && ./mvnw -q test` until green.
- Run `python tools/balance/balance.py vs --a <one sample> --b <another> --level 10 --runs 20` (adapt it to the real CLI) and paste the output.
- Expect balance to be broken: the items are still tuned for 1200 HP. That is fine; Gemini retunes next.
- Result: a list of changed files, test results, and "Spec interpretations".
