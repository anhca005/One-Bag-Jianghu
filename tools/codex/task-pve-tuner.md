# Codex task: automatic PvE stage tuner (decision #48)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #34, #46, #47, #48;
- `docs/reviews/2026-10-04-codex-pve-redesign.md`: your previous attempt and why it failed;
- `tools/codex/out/pve-redesign/progression.py`: reuse what is good;
- `tools/balance/balance.py`, `tools/balance/pve_check.py`;
- `docs/tech/code-style.md`: Python must be readable too.

**Data**
- Game data is in `One-Bag-Jianghu/Assets/_Game/Resources/Data/`. `starter.json` now gives **500 Bạc** (decision #48).
- `shop.json` is **unchanged**. Do not propose shop changes.
- Engine items: `server/battle-engine/src/main/resources/data/items.json`.
- The engine is compiled; use `balance.ensure_classpath()`. **Do not run Maven.**

**Start from the game's `chapters.json`**, not from your previous output.

## Why last time failed
1. The "no-shop" bag stuffed every reward item, so the bag was full from stage 9 and "shop-smart" became identical.
2. Enemies were hand-edited with too few measurements, so stages jump between 2% and 100%.

## Part 1: realistic player bags (`tools/balance/pve_progression.py`, new reusable tool)

**For stage N, assume stages 1..N−1 were each cleared once (K = 0).**
- **Owned:** the starter items plus every first-clear reward item.
- **Bạc:** starter + first-clear Bạc of stages 1..N−1.
- **Level:** from `levels.json` with starter EXP + first-clear EXP.

**Bag selection.** From everything owned, choose what fits a **5×4 grid** using the real item shapes, with a deterministic greedy:
1. at least 1 weapon;
2. rank items by a simple power score: tier first, then rarity, then base price from `shop.json` `itemBasePrices`;
3. place greedily, largest shape first among equal scores.

Document the score in the module docstring.

**Two bags**
- **"no-shop":** the bag selection above, with no purchases.
- **"shop-smart":** before selecting, spend the available Bạc on the shop.
  - Use the expected best common/uncommon/rare item affordable from the slot rules in `shop.json`.
  - Keep it simple and documented, for example: each 200 Bạc buys one common T1 of the player's choice. Read prices from `shop.json`.
  - Then run the same bag selection.

**Expose `bags_for_stage(n) -> (no_shop, shop_smart, level, silver)`** and a CLI that prints a per-stage table.

## Part 2: tuner (`tools/balance/pve_tune.py`, new reusable tool)

**Target band for the no-shop bag,** with draws counting half:

| Stage kind | Band |
|---|---|
| normal | 65–80% |
| elite (5, 15, 25) | 55–65% |
| boss (10, 20, 30) | 45–55% |

**Lever ladder per stage.** Win rate drops as you climb the ladder.
- **Primary:** enemy `level`, from 1 to 35.
- **Fallback:** used only when the level bounds cannot reach the band, for example when the enemy is still too strong at level 1.
  - Step the enemy items down: lower rarity one step, then lower tier, then remove the weakest non-weapon item.
  - Or step them up: raise rarity, then add a matching common item that fits the 5×4 grid.
  - Keep the enemy's weapon and its theme. Never add items whose names clash with the enemy's identity; prefer items already in that stage's set, at a higher rarity.

**Search**
- Binary search on level with **20 runs** per probe.
- Then one **50-run** confirmation at the chosen setting.
- If the 50-run result falls outside the band by more than 5 points, do one more step and confirm again.

**Ordering constraints**
- Within a chapter, normal-stage enemy levels must be **non-decreasing**.
- An elite's level must be ≥ the previous normal's; a boss's level must be ≥ every level in its chapter.
- If a constraint and the band conflict, prefer the constraint. Then report the stage as "ngoài dải" with its value.
- Set `recommendedLevel` to the no-shop player level for that stage.

**Other limits**
- Average fight length must be ≤ 40 s. If the chosen setting exceeds it, note it in the report; do not loop on it.
- **Budget:** about 6 probes per stage at most. Print progress as you go.
- **Write `tools/codex/out/pve-tuner/chapters.json` after each chapter**, so partial work survives.

**CLI flags:** `--stages 1-30`, `--runs 20`, `--confirm-runs 50`, `--out PATH`.

## Part 3: run it and report
- Run the tuner on all 30 stages.
- Then measure the **shop-smart** bag on the tuned stages, 50 runs, and report it. It is information only; no target.
- Save verbatim tool output in `tools/codex/out/pve-tuner/tables/`.
- **Report** `docs/reviews/2026-10-04-codex-pve-tuner.md`, in Vietnamese, ≤ 150 lines:
  1. a per-stage table: stage, kind, player level, Bạc, no-shop bag size, enemy level old → new, item changes, no-shop %, shop-smart %, seconds;
  2. which stages are out of band and why;
  3. exact reproduction commands, in PowerShell.

## Hard rules
- **No git.**
- **Do not modify** game data, Unity code, engine code or docs, other than:
  - the two new tools in `tools/balance/`;
  - `tools/codex/out/pve-tuner/**`;
  - the one report file.
- Run `py tools/lint/check_style.py <your .py files>`; they must be clean.
- **Names:** no Kim Dung names; terms from `docs/design/glossary.md`. Keep stage names, enemy names and dialogue.
- **Be honest.**

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The reproduction commands.
