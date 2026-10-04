# Lệnh cho Gemini: cân bằng vòng 4 — Huyền Độc, Cương Khí T3+, bước nhảy 100%

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Round 3 was **accepted** (decision #31 in `docs/decisions.md`): your 5 changes are now in the real data. Read decisions #28–#31 and your report `docs/reviews/2026-09-30-gemini-balance-rarity-3.md`.

## Hard rules
- Do NOT modify real data, samples, code or docs other than your outputs. No git.
- Create ONLY `tools/gemini/out/balance-rarity-4/` (loadouts, patch, raw tables, proposed samples) and the report `docs/reviews/2026-09-30-gemini-balance-rarity-4.md` (Vietnamese, ≤ 220 lines).
- Every table pasted **verbatim** from the tool; the patch file is exactly what produced the final tables. Accept a patch only if the outside-35–65% count **drops in every matrix**; keep item roles (decision #13 counters); prefer Đột Phá / Tâm Pháp milestones; smooth numbers.
- **If your diagnosis says a loadout is badly built, you must actually fix it.** Put the fixed loadout in your `loadouts/` folder, and also as a proposed replacement sample in `tools/gemini/out/balance-rarity-4/samples/<same file name>`. Say in the report exactly which items changed. Last round you diagnosed `huyen_doc.json` (2 `han_ngoc_tieu` from Hàn Sơn) but left it unchanged, so the problem stayed at 2–12%.

## Setup
Copy `tools/gemini/out/balance-rarity-3/loadouts/` to `tools/gemini/out/balance-rarity-4/loadouts/`. There is no need for `--items`: the real data already contains round 3.

## Focus
1. **Huyền Độc Môn** (`huyen_doc.json`, 2–12%): rebuild the bag with Huyền Độc items only (poison/bleed hidden weapons + claws + a fitting Tâm Pháp item), same bag size and HP. It is a real in-game opponent sample, so it must look like a sensible Huyền Độc build. Then check whether the items still need buffs.
2. **Huyền Độc Phệ Khí** (`poison_drain.json`, 5–20%): this build cannot finish enemies. The mirror match lasts about 51 s and ends in Kiệt Lực with both sides dying in the same tick. Give it a way to kill: poison bursts, drain turning into damage, or better item choice. Do **not** propose rule changes to Kiệt Lực or draws: the user decided to keep the current rule (both sides dying in the same tick = draw), so the fix must come from items and loadouts.
3. **Kim Cang Cương Khí** (`shield_sustain.json`): 69–73% at T1, but **80–87% from T3 upward**. Find what grows with Trọng (ablation per tier: shield values scale with tier and rarity, Đột Phá T3/T6/T9 effects, Tâm Pháp milestones) and tune it so T3–T9 are ≤ 65–70%.
4. **Rarity-step jumps to 100%**: Kim Cang Bách Luyện (Tinh>Lương onward ≈ 100%), Hàn Sơn–Bích Thủy Hợp Bích (98.5%), Huyền Độc Vạn Độc (97.5%). Find the breakpoint (stack count crossing a burst threshold, a kill landing one tick earlier, etc.) and smooth it.

## Steps
1. Baselines on the current data: A `matrix --tiers 1,3,6,9 --tp 1,9 --runs 100`; B `--rarity common` and `--rarity legendary` with `--tiers 1,9 --tp 1 --runs 100`; `rarity-step --tier 3 --tp 1 --runs 100`. Use the round-3 loadouts for the baseline (before your loadout fixes), then show the same tables again after loadout fixes only, then after the item patch.
2. Diagnose each focus point with numbers (item ablation, per-tier ablation, event logs from the CLI: see `server/README.md`).
3. Iterate with `whatif` / `tune`. The final patch must reduce outside counts in A and in both B, and every `rarity-step` column average must stay within 60–80%.
4. Report: baseline, loadout-fix and final tables (verbatim); diagnosis per focus point; each change with its reason; the list of changed samples; exact reproduction commands; at most 4 open questions.
