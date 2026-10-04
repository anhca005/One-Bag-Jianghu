# Lệnh cho Gemini: cân bằng lại sau khi engine đọc phẩm từng bản (thay `task-balance-t9tp9.md`)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`. Bản này **thay** `task-balance-t9tp9.md` (chưa từng chạy, dữ liệu đã đổi).

---

You are the balance designer of "One Bag Jianghu". The engine now reads **per-copy rarity** (decisions #24, #25): `items.json` holds 61 items at Phàm (common) baseline; a copy's stats = baseline × tier multiplier (Trọng 1→9: 1.0…2.0) × rarity multiplier (common 1.00, uncommon 1.25, rare 1.55, epic 1.90, legendary 2.30, supreme 2.75), applied exactly where Trọng applies (attack, effect value/stacks, incl. Tâm Pháp effects added to a weapon), never to qi, durations, costs, pct. The 20 sample loadouts carry the rarity their items used to have; the 40 new non-weapon items (yours) were only tested all-at-common.

## Hard rules
- Do NOT modify real data (`server/battle-engine/src/main/resources/data/*.json`, `server/battle-engine/samples/`, the Unity project), Java code or docs other than your report. No git.
- Create ONLY: `tools/gemini/out/balance-rarity.patch.json` (final patch in the tool's patch format), helper loadouts/files under `tools/gemini/out/balance-rarity/`, and the report `docs/reviews/2026-09-30-gemini-balance-rarity.md` (Vietnamese, ≤ 300 lines).
- Game terms per `docs/design/glossary.md`. Keep item identities/roles; prefer changing Đột Phá (unlocks) and Tâm Pháp milestones over base stats; smooth numbers, no build-up breakpoints.

## Read first
`docs/decisions.md` #12–#15, #24, #25, `docs/design/balance.md`, `docs/design/combat.md`, `tools/balance/README.md` (commands, **`--rarity`** to force every item to one rarity, patch format incl. `sect:<id>.levels.<n>.<field>`), `server/battle-engine/src/main/resources/data/{items.json,sects.json}`, `server/battle-engine/samples/`, your loadouts in `tools/gemini/out/loadouts_non_weapon/`, your report `docs/reviews/2026-09-29-gemini-rarity-items.md`.

## Method
1. **Loadout pool**: copy the 20 samples + your 6 non-weapon loadouts into `tools/gemini/out/balance-rarity/loadouts/` (the samples keep their per-item rarity; give your 6 loadouts sensible mixed rarities, e.g. weapons rare/epic like the samples). Use this folder for every run (`--loadouts`).
2. **Baseline** (record, 200 runs where time allows, at least 100):
   - A. native rarities: `matrix --tiers 1,3,6,9 --tp 1,9`.
   - B. same rarity for everyone: `matrix --rarity common --tiers 1,9`, and the same for `rare`, `legendary`, `supreme` — this is the "fair" PvP case where both sides own copies of equal rarity.
3. **Targets** (#14): in every column of A and B no loadout average > 70% and none < 30%; count of cells outside 35–65% should drop clearly versus baseline (baseline A today ≈ 10/40 at T1/T9). The 6 non-weapon loadouts must be viable (≥ 35% in B at common and legendary).
4. **Rarity step check**: build a helper pair per archetype (at least 4 archetypes): the same loadout at rarity r vs at r+1 (all items), Trọng 3, TP 1; report the win rate of the higher side. Target: one rarity step ≈ 65–80% (a meaningful upgrade, not an auto-win); Phàm Trọng 9 vs Lương Trọng 1 should be close to 50–60% for the Trọng 9 side. If the multipliers themselves look wrong, propose new multipliers with numbers — do not change them in the patch (engine constant).
5. Diagnose offenders (which Đột Phá / Tâm Pháp / new item stacks up), iterate with `whatif` / `tune` (small parameter sets). Pay special attention to your new items at `supreme` (flat shields/heals × 2.75 × 2.0).

## Report
1. Baseline and final tables (code blocks straight from the tool) for A, B (4 rarities) and the rarity-step check.
2. Each change: what, why (numbers), effect; list of new-item changes separately.
3. Exact commands to reproduce the final tables with the patch (`whatif --patch tools/gemini/out/balance-rarity.patch.json ...`).
4. Max 5 open questions.
