# Codex task: redesign the PvE stage curve (economy-aware)

You are doing the balance work originally written for Gemini. **The spec is `tools/gemini/task-round-7-pve-economy.md`; read it fully.** Everything in it applies, except the overrides below.

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Also read:**
- `docs/decisions.md` #34, #44, #46, #47;
- `docs/design/pve.md`;
- `tools/balance/README.md`, `tools/balance/balance.py`, `tools/balance/pve_check.py`, `tools/balance/pve_bags.py`.

## Data
- Game data: `One-Bag-Jianghu/Assets/_Game/Resources/Data/` (`Pve/chapters.json`, `shop.json`, `starter.json`, `levels.json`, `rewards.json`, `materials.json`).
- Engine items: `server/battle-engine/src/main/resources/data/items.json`.
- The engine is already compiled: `server/battle-engine/target/classes` and `target/cp.txt`. `balance.ensure_classpath()` returns the classpath. **Do not run Maven.**

## Overrides (the user asked for these)
1. **Fewer fights.**
   - Iterate with **20 runs** per matchup.
   - Final tables use **50 runs** per matchup.
   - Never use 200.
   - Keep the total number of simulator calls modest: change several stages, then measure them together. Do not measure after every single edit.
2. **Weapon audit:** 50 runs, stages 2, 10 and 12 only.
3. **Skip the matrix A run** at the end. Instead, say whether any item number changed. Item changes should be rare.
4. **Output folder:** `tools/codex/out/pve-redesign/` instead of `tools/gemini/out/round-7/`. Same file set:
   - `progression.py`;
   - `chapters.json`;
   - `shop.json` / `starter.json`, if changed;
   - `patch.json`;
   - `players/no_shop/stage_NN.json`, `players/shop_smart/stage_NN.json`;
   - `tables/*.txt`.
5. **Report:** `docs/reviews/2026-10-04-codex-pve-redesign.md`, in Vietnamese, ≤ 220 lines, same sections as the spec.
6. **Work in chapter order and save as you go.**
   - Finish chapter 1 (stages 1–10), write its files and a "Hồi 1" section in the report, then move on to chapter 2, then chapter 3.
   - If you run out of budget, what is already written must be usable.
   - Use `tools/codex/out/pve-redesign/chapters.json` as the working copy. Start it as a copy of the game file.
7. **Replay income (decision #47) is already implemented in the game.** Model it exactly as the spec describes, with K = 0 for the targets and K = 3 reported.

## Hard rules
- **No git.**
- **Do not modify** game data, engine code, Unity code or other docs. Write only under `tools/codex/out/pve-redesign/` and the one report file. Claude integrates.
- **Python must be readable:** follow `docs/tech/code-style.md` (one statement per line, ≤ 120 columns). Check with `py tools/lint/check_style.py` if it accepts `.py`. Otherwise, keep the style by hand.
- **Names:** no Kim Dung names; terms from `docs/design/glossary.md`. Keep existing stage names, enemy names and dialogue unless an enemy's items change so much that the name no longer fits. If so, note it in the report.
- **Be honest:** the met / not-met table gives the closest values reached.

## Final message (`-o` output)
- What was finished (which chapters).
- The met / not-met summary.
- The exact commands Claude should run to reproduce the final tables with 50 runs.
