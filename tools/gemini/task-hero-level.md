# Lệnh cho Gemini: thiết kế hệ thống cấp Hiệp Khách (mở Môn Phái ở cấp 10)

Dán toàn bộ phần dưới vào Antigravity.

---

You are the game designer of "One Bag Jianghu" (2D wuxia backpack auto-battler, PC). Design the **hero level system**.

## Hard rules
- Create ONLY `docs/reviews/2026-09-29-gemini-hero-level.md` (Vietnamese, ≤ 250 lines). Do not modify any other file. Do not run git.
- Game terms exactly as in `docs/design/glossary.md`; any NEW term must be listed in a "Thuật ngữ đề xuất" table (name, code id, meaning) for the product owner to approve — do not use it as if it were already official. No names from Kim Dung novels or other games (sample hero name: "Vân Du").
- Numbers must be concrete (tables), not "tùy chỉnh sau".

## Read first
`docs/decisions.md` (especially #12 Đột Phá, #13, #14, #15 Tâm Pháp, #21: new accounts have NO sect; the sect UI unlocks at hero level 10; the bag is arranged outside battle), `docs/design/gdd.md`, `docs/design/items.md` (Trọng 1–9, Đột Phá at 3/6/9, Tâm Pháp levels 1/3/6/9), `docs/design/balance.md`, `docs/design/combat.md`, `docs/reviews/2026-09-29-gemini-home-layout.md` (header shows name + level + EXP bar; Môn Phái hotspot locked until level 10), `server/battle-engine/src/main/resources/data/sects.json`.

## Answer the open questions from your Home layout spec, and deliver
1. **EXP sources**: how much EXP per PvE stage (by chapter/stage, first clear vs replay, win vs loss) and per PvP match (win/loss/draw). Whether any item gives EXP (if yes, justify; default: no).
2. **Level curve 1 → max** (propose the max, e.g. 30): table level, EXP to next, cumulative EXP, expected play time / number of battles to reach it. Target: level 10 reached after roughly the first 1–2 hours of play (state your assumption about battles per hour).
3. **Rewards per level**: what each level unlocks (features, bag size?, stamina cap?, item slots?) — keep it small and readable; mark which ones need new systems. Level 10 = choose a sect.
4. **Sect choice at level 10**: the flow (screen steps), can the player preview each sect's Tâm Pháp before choosing, is the choice permanent, cost/conditions to change later (answer your own Q3: after joining, the Môn Phái hotspot becomes the Tâm Pháp upgrade screen or not?). How Tâm Pháp level 1/3/6/9 is raised (resource, cost table).
5. **Balance impact**: a player without sect (levels 1–9) fights opponents that may have a sect in PvP — propose matchmaking rules or protections (e.g. PvP unlocks at level X, or opponents without Tâm Pháp below level 10). Check against `docs/design/balance.md`: Tâm Pháp alone should not dominate.
6. **Data format**: proposed JSON for the level table and EXP rewards (so the server/engine can load it; data-driven, no hardcoded numbers), and which fields `GameSession` / the profile must store.
7. **UI**: what the header EXP bar shows, level-up feedback (popup content), the locked Môn Phái hotspot tooltip text.
8. Max 3 open questions for the product owner.
