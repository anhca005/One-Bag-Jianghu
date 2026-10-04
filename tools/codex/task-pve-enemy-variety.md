# Codex task: make PvE enemies varied (same strength, different kits)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/tech/code-style.md`;
- `docs/decisions.md` #52–#56;
- your report `docs/reviews/2026-10-04-codex-pve-upgrade.md`;
- `tools/balance/pve_lc_build.py`, `tools/balance/pve_progression.py`, `tools/balance/power.py`;
- data under `One-Bag-Jianghu/Assets/_Game/Resources/Data/`:
  - `Pve/chapters.json`: 50 stages, each with an enemy name, title and loadout;
  - `items.json`: weapon class, type, size and sect of every item;
  - `power.json`.

Do not run Maven. Keep simulation counts low (`--runs` 50 at most).

## Problem
- **The player's complaint:** while playing Hồi 1, almost every enemy carries Thanh Phong Kiếm plus an armor, the
  same as the starter kit, so the enemies feel identical.
- **The data shows it:**
  - Thanh Phong Kiếm is the weapon in 7 of 10 stages in Hồi 1 and in several stages of Hồi 2.
  - A wolf (Sói Xám) fights with a sword.

## Do
1. **Re-skin each enemy loadout (stages 2–50)** so the kit fits the enemy's name and theme:
   - **Weapon by theme:**
     - beasts get claws or fists;
     - poison sects get hidden weapons, poison pills and gourds;
     - blade gangs get blades;
     - archers get bows;
     - musicians get the flute or zither;
     - scholars get the brush;
     - and so on.
     Use the `weaponClass` values that exist in `items.json`.
   - **Weapon variety:**
     - no single weapon in more than 3 stages of a chapter;
     - never the same weapon in two consecutive stages;
     - Thanh Phong Kiếm at most once per chapter after stage 1.
   - **Armor and support items:** vary them as well. Do not give every enemy the same armor.
   - **Stage 1 stays exactly as it is:** only Thanh Phong Kiếm, as the user decided.
   - **Use only existing items and rarities.** Placements must fit the enemy bag grid with no overlaps, the same rules
     the current data follows.
2. **Keep the difficulty:**
   - Each stage's enemy LC (from `power.json`, same method as `pve_lc_build.py`) stays within ±5% of its current value.
     Adjust tier or rarity to compensate.
   - Keep `recommendedPower` unchanged.
   - Verify with `pve_lc_build.py --measure-existing`, or the equivalent, that the win rates of the expected player bag
     per stage stay within ±10 points of the current ones.
3. **Output:**
   - First write the result to `tools/codex/out/pve-variety/chapters.json`.
   - When the checks pass, copy it over `Resources/Data/Pve/chapters.json`. Back up the old file next to the new one
     first, as `tools/codex/out/pve-variety/chapters-before.json`.
   - Change only the enemy `items`. Do not touch names, dialogue, rewards, map positions or any other field.
4. **Report:**
   - Write `docs/reviews/2026-10-05-codex-pve-enemy-variety.md` in Vietnamese.
   - Per stage, one row: enemy name, old weapon → new weapon, old LC → new LC, old win rate → new win rate.
   - Add the per-chapter weapon counts.

## Rules
- **No git.**
- Do not edit C# code.
- If you write or change a script, it must be clean under `py tools/lint/check_style.py` when it is C#. Python scripts
  stay readable: one statement per line.

## Report (`-o` output)
- The files changed.
- The per-chapter weapon counts.
- The biggest LC and win-rate changes.
