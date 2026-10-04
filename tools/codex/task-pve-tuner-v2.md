# Codex task: PvE tuner v2 — enemy bags grow with the player (decision #49)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #48, #49;
- `docs/reviews/2026-10-04-codex-pve-tuner.md`: your v1 result;
- your tools `tools/balance/pve_progression.py` and `tools/balance/pve_tune.py`;
- `tools/balance/balance.py`: `--dump-config` gives the engine's tier and rarity multipliers;
- `docs/tech/code-style.md`.

**Do not run Maven;** `balance.ensure_classpath()` works.

## What Claude found in v1
- **Level is a near-useless lever:** +5 HP per level. A level-35 enemy is about 100 HP above a level-7 player.
- **The real gap is item count and weapon count.** By stage 15 the player bag has about 12 items with 3 weapons; the enemy has 5 items with 1 weapon.
  - Raising the rarity of an enemy's defensive items cannot compensate: stage 15 enemy damage was 256 per fight against the player's 1,065.

## New design (user decision #49)
For every stage N:

1. **Player bags:** keep `pve_progression.py` as the source of the no-shop and shop-smart bags. Fix it if you see bugs; report the fixes.
2. **Enemy bag size** = the number of items in the **no-shop** bag at stage N.
   - **Enemy weapon count** = the player's weapon count at stage N, minimum 1.
   - Elite and boss stages may add +1 item.
   - Everything must fit 5×4 using the real shapes; if it does not fit, drop the lowest-value non-weapon.
3. **Enemy item choice (theme):**
   - Keep all items the enemy has now.
   - Add from a theme pool, in this order:
     1. items the same enemy already uses;
     2. items used by other enemies **in the same chapter**;
     3. any item of the needed type (weapon or non-weapon) from `items.json`, preferring types already in the enemy's bag.
   - Deterministic choice: a seeded RNG on the stage id, or a stable sort.
   - Keep the enemy name, sect, portrait and dialogue unchanged.
4. **Enemy level** = the player level at stage N; boss = player level + 1. `recommendedLevel` = the player level.
5. **Lever = a power ladder over (tier, rarity) for the whole enemy bag.**
   - Build the ladder from the engine config multipliers (`--dump-config`): every valid (tier, rarity) pair, sorted by combined strength.
   - Document how strength is combined. If tier and rarity multiply the same stats, use their product.
   - All enemy items take the ladder step's tier and rarity. You may keep the weapons one rarity step above the rest if that helps granularity.
   - Binary-search the ladder index with **20 runs** per probe, then one **50-run** confirmation. If the confirmation is outside the band by more than 5 points, do one adjacent step and confirm again.
   - **Fallback, only at the ladder ends:**
     - bottom still too strong → remove one non-weapon and retry;
     - top still too weak → add one item.
6. **Bands** for the no-shop bag (same as v1), with draws counting half:
   - normal 65–80%;
   - elite 55–65%;
   - boss 45–55%.
7. **Ordering constraint:** within a chapter, the ladder index of normal stages is non-decreasing. Elite and boss stages are ≥ the previous normal stage's index.
   - If the constraint conflicts with the band, keep the constraint and report the stage as "ngoài dải".
8. **Fight length:** report the average; flag anything over 40 s, but do not loop on it.
9. **Budget:** at most about 7 probes per stage. Print progress. Write the output `chapters.json` after each chapter.

Implement this as `tools/balance/pve_tune.py` v2. Replace the v1 logic; keep its CLI flags.

## Run and report
- Run all 30 stages.
- Measure **shop-smart** on the result: 50 runs, for information.
- **Output folder:** `tools/codex/out/pve-tuner-v2/`, containing `chapters.json` and `tables/` with verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-tuner-v2.md`, in Vietnamese, ≤ 150 lines:
  1. a per-stage table: stage, kind, player level, player items/weapons, enemy items/weapons, enemy tier and rarity, no-shop %, shop-smart %, seconds;
  2. the out-of-band list with reasons;
  3. a list of the items added to each enemy (one line per stage);
  4. PowerShell reproduction commands.

## Hard rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_tune.py`, `tools/balance/pve_progression.py`;
  - `tools/codex/out/pve-tuner-v2/**`;
  - the report.
- **Do not modify** game data, Unity code, engine code or other docs.
- `py tools/lint/check_style.py` must be clean on your `.py` files.
- **Names:** no Kim Dung names; terms from `docs/design/glossary.md`.
- **Be honest** about results.

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The reproduction commands.
