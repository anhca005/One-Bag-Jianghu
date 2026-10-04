# Lệnh cho Gemini: cân bằng vòng 3 — sửa các đội hình lệch mạnh

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Round 2 was **accepted** (decision #30 in `docs/decisions.md`): the engine now uses rarity multipliers `[1000,1060,1120,1190,1260,1340]` **by default** (no `--battle-config` needed any more) and your 4-change patch is applied to the real data. Read `docs/decisions.md` #28–#30 and your report `docs/reviews/2026-09-30-gemini-balance-rarity-2.md`.

## Hard rules (same as round 2)
- Do NOT modify real data, samples, code or docs other than your outputs. No git.
- Create ONLY `tools/gemini/out/balance-rarity-3/` (loadouts, patch, raw tables) and the report `docs/reviews/2026-09-30-gemini-balance-rarity-3.md` (Vietnamese, ≤ 220 lines).
- Every table pasted **verbatim** from the tool; the patch file is exactly what produced the final tables; accept a patch only if the outside-35–65% count **drops in every matrix**; keep item roles (decision #13 counters), prefer Đột Phá / Tâm Pháp milestones, smooth numbers.

## Focus: the outliers left after round 2
Use your round-2 loadouts (`tools/gemini/out/balance-rarity-2/loadouts/`, copy to the new folder). With the patched real data they show:
- **Kim Cang Cương Khí** (`shield_sustain.json`) ≈ 86–92% everywhere → the new armour/shield items (and/or this bag) are too strong even with the 100% max-HP shield cap. Find which items carry it (try removing items one by one) and tune them, not the loadout.
- **Tử Mẫu Tốc Kích** (`crit_haste.json`) 0–25% and **Huyền Độc Môn** (`huyen_doc.json`) ≈ 10% → too weak; decide whether the loadout is badly built (then fix the loadout and say so) or the items are weak (then buff with Đột Phá/stats).
- **Huyền Độc Phệ Khí** (`poison_drain.json`) ≈ 5–20%.
- Also look at `rarity-step`: some loadouts jump to **100%** at a single step (Huyền Độc at Tuyệt>Tinh and Chí Tôn>Thần Binh, Kim Cang Bách Luyện) — this means a breakpoint (e.g. a stack count crossing a burst threshold). Identify the breakpoint and smooth it (smaller stacks with more frequent application, etc.).

## Steps
1. Baselines on the current data: A `whatif`-free `matrix --tiers 1,3,6,9 --tp 1,9 --runs 100`, B `--rarity common|legendary --tiers 1,9 --runs 100`, and `rarity-step --tier 3 --tp 1 --runs 100`.
2. Diagnose each outlier with numbers (item ablation, event logs from the CLI if useful: `server/README.md`).
3. Iterate with `whatif`/`tune`; final patch must reduce outside counts in A and both B, and must not make any `rarity-step` column average leave 60–80%.
4. Report: baseline vs final tables (verbatim), diagnosis per outlier, each change with reason, exact reproduction commands, max 4 open questions.
