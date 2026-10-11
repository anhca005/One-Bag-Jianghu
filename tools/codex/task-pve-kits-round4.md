# Codex task: redo enemy kit distribution (plausible kits), easier Hồi 1

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Data and tools only.

**Read first:**
- `docs/tech/code-style.md`; `docs/decisions.md` #52–#56; `docs/reviews/2026-10-05-codex-pve-enemy-variety.md`;
- `tools/codex/task-pve-enemy-variety.md` (rounds 1–3) and your scripts `tools/codex/pve_enemy_variety*.py`;
- `tools/balance/pve_progression.py` (expected player bag per stage), `power.py`, `pve_tune.py`;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`, `items.json`, `power.json`.

Do not run Maven. Keep simulation counts low: probe 20 runs, confirm 40 runs.

## Problem (user playtest + Claude review)
Round 3 hit the win-rate band by pushing tiers wherever the search led, so many kits are implausible:
- stage 3: a wolf with a single tier-9 legendary claw and nothing else; stage 5: two tier-9 twin blades, nothing else;
- stages 18, 27, 35: tier 7–8 items while their neighbours are tier 1–3; stages 4 and 13 carry only 2–3 items;
- the user found Hồi 1 too hard. Claude hand-set stages 3 and 5 to low-tier kits (keep them as a starting point).

## Do
1. **Plausibility rules (hard):**
   - Each enemy carries a **full kit**: at least one weapon of its themed class, at least one armor, and at least one
     support item (pill, manual or relic). Item count grows with progression: Hồi 1 3–6 items, Hồi 2 5–9, Hồi 3 7–12,
     Hồi 4 9–15, Hồi 5 10–18 (bosses may take the top of the range).
   - **Tier band per chapter:** Hồi 1 T1–2, Hồi 2 T1–3, Hồi 3 T2–4, Hồi 4 T3–6, Hồi 5 T4–7. Bosses may go +1 above the
     band. Within a chapter, the average tier must not decrease from one stage to the next by more than 1.
   - **Rarity band:** Hồi 1 common/uncommon, Hồi 2 up to rare, Hồi 3 up to rare (boss epic), Hồi 4 up to epic, Hồi 5 up
     to epic (final boss may use one legendary).
   - Keep the round-1 variety rules: themed weapon by enemy name; a weapon in at most 3 stages per chapter; no weapon in
     two consecutive stages; at most 2 copies of one weapon per enemy; Thanh Phong Kiếm at most once per chapter after
     stage 1. Avoid giving every enemy the same armor; beasts use beast hides/claws where items exist
     (`tuyet_lang_bach_bi`, `heo_rung_bap_giap`, claws).
   - Stage 1 stays exactly as it is. Placements fit the enemy grid with no overlaps.
2. **Difficulty targets** (player win rate against the expected bag from `pve_progression.py`):
   - **Hồi 1 (easier, user decision):** normal 85% (75–95), elite 75% (65–88), boss 65% (55–78).
   - **Hồi 2–5:** keep decision #55: normal 72% (60–84), elite 62% (50–74), boss 50% (38–62).
   - Tune inside the plausibility rules: item count, which support items, tier/rarity within the band, mixed tiers.
     If a stage cannot reach its band inside the rules, keep the rules and report the stage.
3. **Output and apply:**
   - Write the candidate to `tools/codex/out/pve-kits-r4/chapters.json`.
   - Apply only if all hard rules hold and at least 44/49 stages are in band, with every boss in band. Then back up the
     current game file to `tools/codex/out/pve-kits-r4/chapters-before.json` and copy the candidate over
     `Resources/Data/Pve/chapters.json`. Change only `enemy.items`; keep the file's formatting (2-space indent, CRLF).
   - Otherwise do not apply; report.
4. **Report:** write `docs/reviews/2026-10-10-codex-pve-kits-round4.md` in Vietnamese: per stage one row with enemy,
   item count, tier range, rarity range, weapon, win rate, target, in band. Add a "rule check" section.

## Rules
- **No git.** Do not edit C# code. Python scripts stay readable (one statement per line).
- Other Codex tasks run at the same time; only touch `tools/codex/`, `tools/codex/out/pve-kits-r4/`, the review file and
  `chapters.json` (enemy items only).

## Report (`-o` output)
- Applied or not, in-band count, stages out of band and why, the biggest changes, files changed.
