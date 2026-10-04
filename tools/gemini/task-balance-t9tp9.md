# Lệnh cho Gemini (Antigravity): cân bằng tổ hợp Trọng 9 + Tâm Pháp 9

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Previous passes balanced item tiers (Trọng) and Tâm Pháp levels separately. When BOTH are at 9, some loadouts exceed the 70% target (6 Quyền Sáo ≈ 77%, Kim Cang + Thiết Phủ ≈ 71%, Kiếm Khí ≈ 70%). Fix it without breaking the other combinations.

## Hard rules
- Do NOT modify real data files (`server/battle-engine/src/main/resources/data/*.json`), Java code, samples or docs. Do NOT run git.
- Create ONLY: `tools/gemini/out/balance-t9tp9.patch.json` (the final patch in the tool's patch format), optional helper files under `tools/gemini/out/`, and the report `docs/reviews/2026-09-28-gemini-balance-t9tp9.md` (Vietnamese, ≤ 250 lines).
- Game terms per `docs/design/glossary.md`.

## Read first
`docs/design/balance.md` (all passes and lessons, especially pass 3 and "7 binh khí mới"), `docs/decisions.md` #12–#15, `tools/balance/README.md` (tool usage and **patch format**, including `sect:<id>.levels.<n>.<field>`), `server/battle-engine/src/main/resources/data/items.json` and `sects.json`, the 20 loadouts in `server/battle-engine/samples/`.

## Method
1. Baseline: `py tools/balance/balance.py matrix --tiers 1,3,6,9 --tp 1,9 --runs 200` (record which cells exceed 70% or fall under 30%).
2. Diagnose each offender (which Đột Phá / Tâm Pháp milestone stacks up; use `--runs` breakdown of the CLI if useful: `server/README.md`).
3. Prefer changing **Đột Phá (unlocks) and Tâm Pháp milestones** over base stats, so tier-1/level-1 balance (already tuned) stays intact. Prefer smooth stats; avoid build-up stack breakpoints; triggers with `from: self`.
4. Iterate with `whatif` and/or `tune` (a small parameter set only, e.g. ≤ 10 params). Target for **every** column of `--tiers 1,3,6,9 --tp 1,9`: no loadout average above 70%; the tier-1/TP1 column must not move by more than ~3 points per loadout versus baseline.

## Report
1. Baseline table and final table (both as code blocks from the tool).
2. Each change: what, why (diagnosis with numbers), effect.
3. Remaining outliers and why they are acceptable (e.g. deliberately weak layouts like "Huyền Độc gốc").
4. Open questions (max 5).
