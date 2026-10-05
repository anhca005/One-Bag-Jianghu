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

## Note (rerun)
- A previous run was interrupted.
- It left a draft script, `tools/codex/pve_enemy_variety.py`, and a draft output in `tools/codex/out/pve-variety/`.
- Reuse them if they are sound, or replace them. Check everything again: nothing in the draft was verified.

## Round 2: tune by win rate, not by LC

**Your round-1 finding:** LC does not predict the result when the weapon class changes. One example is stage 4: the
blade swap kept LC 203 → 210, but the win rate fell 92% → 0%. Holding LC within ±5% is therefore the wrong constraint.
**The old win rates are not a good target either:** many stages sit at 0% or 100%.

New rules:
1. **Keep your themed weapon choice and the variety rules from round 1.**
   - Stage 1 stays untouched.
   - Hard rule: no weapon in more than 3 stages per chapter. **Count every stage that has the weapon, not just how many
     copies a stage carries.**
   - Hard rule: never the same weapon in two consecutive stages.
   - Avoid filling one enemy with 3–4 copies of the same weapon. At most 2 copies of one weapon per enemy.
2. **Target the design band from decision #55**, against the expected player bag at each stage (same model as
   `pve_lc_build.py` and the upgrade model):
   - win about 72% on normal stages;
   - about 62% on elite stages, if the data has them;
   - about 50% on boss stages (Thủ Lĩnh, stage 10 of each chapter).
   - Tolerance: ±12 points at 50 runs.
   - Stage 1 is exempt.
3. **Tune each enemy by simulation**, the way `tools/balance/pve_tune.py` does:
   - Probe with ~20 runs, then confirm with 50 runs.
   - Adjust tier, rarity, the support and armor items, and the item count. Keep the themed weapon class (another weapon
     of the same class is fine).
   - Avoid stacking the same armor on every enemy.
   - **LC may drift.** Report old → new LC, but do not constrain it.
4. **`recommendedPower`:** keep it unchanged. It is the LC of the player bag that beats the stage, not the enemy's LC.
5. **Apply and back up:** if at least 45 of the 49 tuned stages land in the band and all hard rules hold, back up the old
   file and apply it, as in round 1. If not, do not apply. Report which stages failed and why.
6. **Report:** update `docs/reviews/2026-10-05-codex-pve-enemy-variety.md` with a round-2 section that uses the same
   table plus a "target" column.

## Round 3: fix only the 13 out-of-band stages, then apply

**Starting point:** the round-2 candidate in `tools/codex/out/pve-variety/chapters.json`. 36/49 stages are in the band.
The user chose to tune only the stages that are out of band, then apply.

1. **Retune only these stages:** 9, 16, 18, 20, 23, 27, 31, 34, 36, 41, 42, 44, 50.
   - Leave the other 36 untouched, and do not re-measure them. Their round-2 confirmations stand.
2. **When tier and rarity steps jump over the band,** you may also:
   - swap the themed weapon for another weapon of the same class, or for a different class that still fits the enemy
     (for example Phán Quan Bút → another scholar-like weapon);
   - change a support or armor item to one with a finer effect;
   - add or remove one small item;
   - use a mixed tier across items (e.g. weapon T3, armor T2) for finer steps.
   - Keep every hard rule:
     - variety caps per chapter (re-check after each swap);
     - no consecutive repeat;
     - at most 2 copies of one weapon per enemy;
     - stage 1 untouched.
3. **Stage 50 is the final boss** (Tào Huyền Kỳ) and must land in 38–62%. Treat it as the top priority; bosses may carry
   more and better items.
4. **Measure:** probe with ~20 runs and confirm with 50 runs, as in round 2.
5. **Apply:** if at least 45/49 are in band (with stage 50 among them), apply exactly as round 1 describes:
   - back up the current game file to `tools/codex/out/pve-variety/chapters-before.json`;
   - copy the candidate over `Resources/Data/Pve/chapters.json`;
   - change only `enemy.items`.
   Otherwise do not apply, and report.
6. **Report:** add a "Round 3" section to the review with the 13 rows (round 2 → round 3) and the final in-band count.
