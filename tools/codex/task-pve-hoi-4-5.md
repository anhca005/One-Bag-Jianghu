# Codex task: PvE Hồi 4 and Hồi 5 (stages 31–50): story, content and balanced enemies

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`.

**Read first:**
- `docs/design/pve.md`, which gives the data format;
- `docs/design/glossary.md`, which has the mandatory terms;
- `docs/reviews/2026-10-01-gemini-pve-content.md`, the story so far and the style of Hồi 1–3;
- the game's `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`;
- `docs/decisions.md` #47–#55;
- `docs/reviews/2026-10-04-codex-pve-lc-shop-v2.md`, your last stage rebuild;
- tools `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`, `tools/balance/power.py`;
- `Resources/Data/levels.json` (max level 30), `items.json`, `sects.json`;
- `docs/tech/code-style.md`.

Do not run Maven.

## Goal
Add two chapters, `hoi_4` (stages 31–40) and `hoi_5` (stages 41–50), in the same format and style as Hồi 1–3.

## 1. Story and content (you design it; the user will review)
- **Continue the open thread.** At the end of Hồi 3 the hero is heading toward Trung Nguyên and the imperial capital,
  and U Minh Giáo still hunts the scattered pieces of the Thần Nang.
- **Do not end the story at stage 50.** The game will continue after Hồi 5:
  - Hồi 5 closes its own arc with a boss;
  - it leaves the main conflict open, for example with a higher U Minh leader still at large.
- **Each chapter has its own identity:**
  - setting and season, distinct from autumn river, swamp and snow mountain;
  - the `theme` colors (accent, paper, ink), readable like the existing ones;
  - a sect mix for enemies from `sects.json`;
  - 2 beast or monster stages using the **existing** monster items only (`monsterItems`). **No new items.**
- **Stage list per chapter:** stage 5 is `elite`, stage 10 is `boss`, the rest `normal`.
  - Each stage gets: name, description, `mapPos` (spread across the map, readable path order), and `staminaCost`
    (follow the existing curve).
  - Each stage also gets dialogue `before` / `afterWin` / `afterLoss` (2–3 lines each, in the voice of Hồi 1–3).
- **Characters:**
  - at most 3 new characters in `characters`: 2 bosses, plus at most 1 recurring NPC or rival;
  - existing characters (Lục Tiều Phu, Bạch Linh Nhi, Phong Vũ) may return;
  - portrait names follow the existing pattern (`boss_<id>`, `npc_<id>`). The art does not exist yet: the game falls
    back to placeholders.
- **Rewards:**
  - continue the first-clear curve of Hồi 1–3 (Bạc, EXP, materials, including higher material tiers where Hồi 3
    uses them);
  - `items: []`, `drops: []`, `repeat: "default"` (decisions #47, #52).
- **Names:**
  - no Kim Dung names, no names from reference games;
  - no duplicate stage or character names;
  - Vietnamese Hán-Việt style like the existing ones.
- **Theme image names:** `map_hoi_4`, `bg_battle_hoi_4`, `node_hoi_4`, `node_boss_hoi_4`, and the same for 5.

## 2. Balance (stages 31–50)
- Extend the progression model to 50 stages: Bạc and EXP from the new first-clear rewards, and the Monte Carlo shop
  as in round 2.
- **Hero max level is 30. Do not change `levels.json`.** Report the stage where the expected player reaches level 30.
  - Enemy level may go up to 30 at most.
  - After the player hits the cap, difficulty grows through enemy items (rarity, tier, count) only.
- Build enemies with the same method and bands as round 2 (normal 62–82%, elite 52–72%, boss 40–60%), and write
  `recommendedPower`.
- **Fix the seed reuse found in round 2.**
  - The battle CLI with `--runs N` always uses seeds 1..N.
  - Tune on 20-run measurements, then confirm on **fresh seeds**: run 100 and use only seeds 51–100, computed as
    `(100 × rate100 − 50 × rate50) / 50`. Alternatively, pass a mirrored loadout order if that changes seeds; document
    which.
  - Report the fresh-seed rate.
- **Stages 1–30: do not touch them.** Copy them verbatim from the game's current `chapters.json`.
- **Also report the unspent Bạc.** Bạc piling up was already a problem at stage 30; say whether it gets worse.

## Output
- **`tools/codex/out/pve-hoi-4-5/chapters.json`:** the full file, with stages 1–30 unchanged plus `hoi_4` and `hoi_5`.
  - It must parse.
  - All item ids must exist.
  - All speakers must exist in `characters`.
  - It must have the same top-level keys.
- **`tools/codex/out/pve-hoi-4-5/tables/`:** verbatim output.
- **Report:** `docs/reviews/2026-10-04-codex-pve-hoi-4-5.md`, in Vietnamese, ≤ 150 lines:
  1. the chapter synopses and the new characters;
  2. a per-stage table for 31–50: name, kind, level, Bạc, bag items/LC, enemy items/LC, recommendedPower, tune-seed %,
     fresh-seed %, seconds;
  3. the out-of-band list;
  4. level cap and Bạc notes;
  5. the list of art needed (map, battle background, node icons, portraits) with one-line descriptions, so Claude can
     write ChatGPT prompts;
  6. PowerShell reproduction commands.

## Rules
- **No git.**
- **Write only:**
  - `tools/balance/pve_progression.py`, `tools/balance/pve_lc_build.py`;
  - `tools/codex/out/pve-hoi-4-5/**`;
  - the report.
- **Do not modify:** game data, Unity code, engine code or other docs.
- Keep the Python readable: ≤ 120 columns, one statement per line. Lint the two files clean.
- **Be honest.**

## Final message (`-o` output)
- The synopsis in 3 lines.
- In band X/20, on fresh seeds.
- The out-of-band list.
- The level-cap stage and the Bạc note.
- The art list.
