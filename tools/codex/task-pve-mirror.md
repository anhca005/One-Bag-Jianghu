# Codex task: build PvE enemies as mirrors of the expected player bag (decision #50)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/decisions.md` #48, #49, #50;
- `tools/balance/pve_progression.py`, which provides `bags_for_stage(n)`, and `tools/balance/pve_tune.py`. Reuse their helpers: the measure function and output writing;
- `docs/tech/code-style.md`.

Do not run Maven; `balance.ensure_classpath()` works.

**Why:** Claude measured that the no-shop bag against an exact copy of itself wins 30–65%. With one non-weapon removed from the copy it wins 75–90%, and with two removed 90–100%. One item is worth about 30–40 points, so ladder searches on tier and rarity jump from 0% to 100%. We now **construct** enemies instead of searching.

## Rule (user decision #50)
Start from the **game's** `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`. For each stage N, let `bag = no-shop bag at N` from `pve_progression.bags_for_stage(N)`.

- **Enemy items** = a copy of `bag["items"]`: same ids, tier, rarity and x/y.
  - Enemy `level` = the player level; boss = player level + 1. `recommendedLevel` = the player level.
- **Keep untouched:** enemy `name`, `portrait`, `sect`, `tamPhapLevel`, stage `name`, `description`, `dialogue`, `rewards` and every other field.
- **Kind adjustment, starting point:**

  | Kind | Change from the copy |
  |---|---|
  | normal | remove 1 non-weapon (the lowest power score as defined in `pve_progression`) |
  | elite | lower the rarity of the 2 highest-score non-weapons by one step; if there are none, lower one weapon |
  | boss | exact copy |

- **One correction pass.** Measure the no-shop bag against the enemy with **50 runs**, draws counting half. Target bands:
  - normal 65–85%;
  - elite 55–70%;
  - boss 40–55%.

  If the result is outside its band, apply **one** step toward the band, measure 50 runs again, and keep whichever of the two results is closer to the band:

  | Kind | Too easy (above band) | Too hard (below band) |
  |---|---|---|
  | normal | remove 0 items instead of 1 | remove 2 items |
  | elite | lower 1 rarity step instead of 2 | lower 3, or also remove 1 non-weapon |
  | boss | raise the rarity of the highest-score non-weapon by one step | lower 1 non-weapon one rarity step |

- **Stage 1:** if the bag has only 2 items, a normal enemy with 1 removed would have just a weapon. That is fine; measure it.
- Then measure the **shop-smart** bag against the final enemy, 50 runs, for information.

## Tool
`tools/balance/pve_mirror.py`:
- CLI `--stages 1-30 --runs 50 --out PATH`;
- prints one line per stage as it goes;
- writes the output `chapters.json` after every chapter.

## Run and report
- Run all 30 stages.
- **Output:** `tools/codex/out/pve-mirror/chapters.json`, plus `tables/` with verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-mirror.md`, in Vietnamese, ≤ 120 lines:
  1. a per-stage table: stage, kind, player level, items/weapons, adjustment applied, no-shop %, shop-smart %, seconds (flag > 40 s);
  2. the out-of-band list;
  3. **text conflicts:** stages whose `description` or `dialogue` mentions a weapon or item the enemy no longer carries. Quote the phrase. Claude will rewrite those lines, so do not edit the text yourself;
  4. PowerShell reproduction commands.

## Hard rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_mirror.py`;
  - `tools/codex/out/pve-mirror/**`;
  - the report.
- **Do not modify** game data, Unity code, engine code or other docs.
- `py tools/lint/check_style.py tools/balance/pve_mirror.py` must be clean.
- **Be honest.**

## Final message (`-o` output)
- In band: X/30.
- The out-of-band list.
- The number of text conflicts.
- The reproduction commands.
