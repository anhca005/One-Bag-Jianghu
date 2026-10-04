# Lệnh cho Gemini: Items v2, cân bằng vòng 1 + dựng lại đội hình mẫu

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Your Items v2 redesign was **approved** with changes: see `docs/decisions.md` #32. The Java engine now supports every approved mechanic.

Codex converted your draft into the exact engine schema: `tools/gemini/out/items-v2/items-v2.normalized.json`. Read the converter's notes in `tools/items/convert_items_v2.py`; it printed what it changed per item. Three changes matter:
- `phuc_ma_giap`: retaliate → `damage value 12` on `damaged`.
- `giai_doc_dan`: → `burst_taken`.
- `thuan_duong_cong`: → `add_attack 25`, because a timed % buff is not an approved mechanic.

**The normalized catalog is now the design baseline.** Read your report `docs/reviews/2026-09-30-gemini-items-v2.md`, `docs/design/balance.md`, decisions #13, #25–#32, and `server/README.md` for the CLI and the new trigger/op names.

## Hard rules
- Do NOT modify real data, samples, code or docs. No git.
- Create ONLY:
  - `tools/gemini/out/items-v2-balance-1/`: loadouts, patch, raw tables, candidate samples.
  - The report `docs/reviews/2026-10-01-gemini-items-v2-balance-1.md` (Vietnamese, ≤ 250 lines).
- Every table must be pasted **verbatim** from the tool. The patch file must be exactly what produced the final tables. Smooth numbers. Keep each item's identity and its decision #13 counter role. Prefer Đột Phá or Tâm Pháp milestones over raw stats.
- No new mechanics: use only what the engine accepts. If the catalog fails to load, report the error.

## Part 1: new sample loadouts for v2
The 20 current samples (`server/battle-engine/samples/`) were built for v1 item designs. Build a **fresh set of 20 sample loadouts** under `tools/gemini/out/items-v2-balance-1/loadouts/`:
- Same JSON format and `hp: 1200`, 5×4 bag, `sect`, `tamPhapLevel`.
- Every sect gets at least 2 builds that use its v2 build-around items (your sect coverage table).
- Include at least 3 hybrid builds and at least 2 builds centred on active relics.
- Builds must be plausible for a player: correct sect weapons and sensible adjacency, with no filler items of another sect's identity. The old `huyen_doc.json` mistake (two Hàn Sơn hidden weapons in a Huyền Độc bag) must not happen again.
- Keep the old file names where a build keeps the same idea, so the game's opponent list stays recognisable.
- These loadouts will become the real samples (players fight them in PvE), so each `name` must be "<Sect name> - <short build name>" **without** a weapon list in brackets.

## Part 2: balance round 1
Use `--items tools/gemini/out/items-v2/items-v2.normalized.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts` everywhere.
1. Baselines:
   - A: `matrix --tiers 1,3,6,9 --tp 1,9 --runs 100`
   - B: `--rarity common` and `--rarity legendary` with `--tiers 1,9 --tp 1 --runs 100`
   - `rarity-step --tier 3 --tp 1 --runs 100`
2. Diagnose the outliers with ablation and event logs. Also check the new mechanics for abuse:
   - infinite or near-infinite loops (for example a `healed` → heal chain, or `damaged` → shield);
   - `buff_stack` builds that snowball;
   - relic actives that are too frequent;
   - `cleanse` hard-countering a whole sect.
3. Iterate with `whatif` / `tune`. Targets:
   - outside 35–65% at most about 30% of the cells in A and in both B matrices;
   - every `rarity-step` column average within 60–80%;
   - no single build above 75% or below 25% average in A.
4. Write `tools/gemini/out/items-v2-balance-1/patch.json`, a patch on top of the normalized catalog in the same format as earlier rounds, and `items-v2-balanced.json`, the full catalog with the patch applied.

## Report
- Baseline and final tables (verbatim).
- The new samples list (file, name, sect, idea).
- The diagnosis per outlier.
- Every change with its reason.
- Any mechanic abuse found and how it was capped.
- Exact reproduction commands.
- At most 4 open questions.
