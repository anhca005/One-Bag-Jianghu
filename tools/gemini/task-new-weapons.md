# Lệnh cho Gemini (Antigravity): thiết kế binh khí cho các hệ còn trống

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the game designer on "One Bag Jianghu" (2D wuxia backpack auto-battler). Several weapon classes exist in the design and are buffed by a sect's Tâm Pháp, but have **no items yet**: Chưởng (`palm`), Phiến (`fan`), Thương (`spear`), Chùy (`hammer`), Tiên (`whip`). Design one weapon for each, plus one for Song Đao/Kiếm (`dual`) and Cung/Nỏ (`bow`), and validate them with the simulator.

## Hard rules
- Create ONLY: `tools/gemini/out/items.proposed.json` (the FULL items file = current 14 items unchanged + your 7 new ones), test loadouts under `tools/gemini/out/loadouts-new/`, and the report `docs/reviews/2026-09-27-gemini-new-weapons.md` (Vietnamese, ≤ 300 lines).
- Do NOT modify existing files (items.json, sects.json, Java, samples, docs). Do NOT run git.
- Wuxia names in Vietnamese, original (no Kim Dung, no other games). Game terms per `docs/design/glossary.md`.

## Read first
`docs/design/items.md` (weapon class table with size/speed/mechanic, data format, validation rules, Đột Phá, Tâm Pháp), `docs/design/combat.md`, `docs/design/balance.md` (budget rules + ALL lessons, pass 3), `docs/decisions.md` #10–#15, `server/battle-engine/src/main/resources/data/items.json` and `sects.json`, your earlier reports in `docs/reviews/`.

## Requirements per weapon
- Follow the class table in items.md (size, speed, signature mechanic). Weapons are ≥ 2 cells. Must be expressible with existing ops/triggers/statuses. If a signature mechanic is impossible with the current engine (e.g. bow "destroys pills", staff "interrupts ultimate"), design the closest possible version and list the missing mechanic as an open question.
- Include `unlocks` (Đột Phá) at tiers 3/6/9 following the rules in items.md (prefer smooth stats; triggers with `from: self`; no extra build-up stacks that create breakpoints).
- Rarity spread: mix of uncommon/rare/epic.
- Budget: DPS per cell ≈ 6 × rarity multiplier (balance.md rule 1); no-ultimate weapons give ≤ 1 qi/s; control on all enemy items ≤ 1 s.
- No `sect` field (items are sect-neutral).

## Validate
1. Build 5 new loadouts (5x4 bag, hp 1200, 16–20 cells) that feature the new weapons, each with the `sect` whose Tâm Pháp buffs them (palm→liet_diem, fan→thai_thanh, spear→han_son, hammer→thiet_phu, whip→bich_thuy; dual/bow have no sect yet, use any). Put them together with copies of the 15 samples in `tools/gemini/out/loadouts-new/` (20 loadouts).
2. Run from the repo root (Python `py`, see `tools/balance/README.md`):
```
py tools/balance/balance.py matrix --loadouts tools/gemini/out/loadouts-new --items tools/gemini/out/items.proposed.json --tiers 1,9 --tp 1,9 --runs 200
```
3. Target: no loadout average above 70% in any column, and the 15 old loadouts should move by at most ~5 points compared to running the same command with the real items (omit `--items`). Iterate until true.

## Report
1. Table of the 7 weapons: name, class, size, rarity, cooldown, attack, effects, ultimate, Đột Phá, budget arithmetic.
2. The matrix output (both runs) as code blocks.
3. Iteration notes, missing mechanics, open questions (max 5).
