# Codex task: build every PvE stage from Lực Chiến (decisions #50, #51)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #50, #51;
- `docs/reviews/2026-10-04-codex-pve-mirror.md`;
- `docs/reviews/2026-10-04-codex-power-calibration.md`;
- tools: `tools/balance/power.py` (`lc()`), `tools/balance/pve_progression.py` (`bags_for_stage`), `tools/balance/pve_mirror.py`;
- `docs/tech/code-style.md`.

**Data**
- Calibrated `power.json` is now in the game at `One-Bag-Jianghu/Assets/_Game/Resources/Data/power.json`; use that one.
- Do not run Maven.

## Facts from calibration
`P(player wins) ≈ sigmoid(30.68 × ln(LC_player / LC_enemy))`. The curve is steep: 3% more LC already gives about 72% wins.

The mirror build (#50) reached 16/30. Removing a whole item is too coarse for small early bags.

## Rule
For each stage N:
1. `bag = no-shop bag` at N. `LC_p = lc(bag)`.
2. **Target enemy LC** = `LC_p / r`, where r comes from the target win rate `w` through the calibrated model: `r = exp(logit(w) / 30.68)`.
   - normal `w = 0.72`;
   - elite `w = 0.62`;
   - boss `w = 0.50`.
3. **Start from the mirror** of `bag`: same items, positions, tier and rarity. Enemy level = player level; boss +1.
4. **Fine-tune toward the target LC** with the smallest moves, repeated until the enemy LC is within ±1.5% of the target or no move gets closer:
   - lower or raise one item's rarity one step;
   - then one item's tier one step;
   - remove or add one non-weapon only if the rarity and tier moves cannot get within ±1.5%.
   - Always pick the move that brings LC closest to the target.
   - Ties go to non-weapons, and to the lowest-base item first.
   - Keep at least the player's number of weapons.
5. **Verify** with **50 runs**: no-shop vs enemy. Bands:
   - normal 62–82%;
   - elite 52–72%;
   - boss 40–60%.

   If outside the band, shift the target LC by ±2% in the right direction, rebuild, and verify once more. Keep the closer result.
6. Also measure **shop-smart** vs the final enemy, 50 runs, for information.
7. **Fix the shop-smart model first:** when the 5×4 bag is full, purchased items may **replace** the weakest items by LC per cell. Use `power.lc` for the score instead of the old heuristic. This also changes the no-shop selection to "best LC that fits". Report the fix.

**Keep untouched:** enemy `name`, `portrait`, `sect`, `tamPhapLevel`, dialogue, description and rewards. Set `recommendedLevel` to the player level.

## Tool, outputs and report
- **Tool:** `tools/balance/pve_lc_build.py`, CLI `--stages 1-30 --runs 50 --out PATH`. It prints a line per stage and writes the output after every chapter.
- **Output:** `tools/codex/out/pve-lc/chapters.json` and `tables/` with verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-lc.md`, in Vietnamese, ≤ 120 lines:
  1. a per-stage table: stage, kind, level, LC no-shop, LC shop-smart, enemy LC, recommended LC (= enemy × 1.1), no-shop %, shop-smart %, seconds (flag > 40 s);
  2. the out-of-band list;
  3. the text-conflict list, the same check as the mirror report: dialogue or description naming items the enemy no longer carries;
  4. PowerShell reproduction commands.

## Hard rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_lc_build.py`, `tools/balance/pve_progression.py`;
  - `tools/codex/out/pve-lc/**`;
  - the report.
- **Do not modify** game data, Unity code, engine code or other docs.
- `py tools/lint/check_style.py` must be clean on your `.py` files.
- **Be honest.**

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The number of text conflicts.
- The reproduction commands.
