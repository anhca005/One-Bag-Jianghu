# Lệnh cho Gemini: vòng 6, ma trận A với đòn bẩy lớn hơn

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Read:
- `docs/decisions.md` #35–#42;
- your reports for rounds 4, 4b and 5 in `docs/reviews/`;
- `docs/design/combat.md`;
- `tools/balance/README.md`;
- `tools/balance/pve_check.py`.

Rebuild first: `cd server && ./mvnw -q test`. The game data holds rounds 4 + 4b:
- items;
- engine defaults;
- `chapters.json`;
- the expected player bags in `tools/gemini/out/round-4b/player/`.

## Situation (Claude verified)
- **Matrix A** (`--tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10`) is stuck at **103/192** cells outside 35–65%. Round 5 changed nothing.
- **The cap-60 experiment had no effect.** It reproduced exactly; no build exceeds ~57% Hộ Thân. The cap stays at 80%.
- **The user keeps the target ≤ ~58/192** and chose to unlock **bigger levers** this round.

## Levers now allowed
1. **Hero base HP** (your round-5 question 3):
   - `config: baseHp` (now 100) and `config: hpPerLevel` (now 5);
   - the T1 columns are the worst, which suggests burst decides fights before sustain matters.
2. **The 24 sample loadouts** (`server/battle-engine/samples/`):
   - you may **rework** outlier builds. Change item choice and placement; keep each build's sect theme and name;
   - they represent "a reasonable player build of that sect", not a sacred list;
   - deliver them in `round-6/loadouts/`.
   - The worst rows today:
     - Kiếm Đạo Hóa Kình averages 75%;
     - Kim Cang Thiết Phủ 68%;
     - Hàn Bích Hợp Bích 67%;
     - Thiết Phủ Liệt Diễm 31%;
     - Kiếm Khí Xuyên Giáp 34%;
     - Chấn Nhạc Chùy Phong 35%;
     - Thái Thanh Liệt Diễm 35%;
   - plus build rows that swing across tiers, such as Dưỡng Sinh Cầm Âm (2.5% at T1/TP1, 62% at T9) and the Huyền Độc rows (16% at T1, 93% at T9).
3. **Tâm Pháp coefficients** in `sects.json` (via `balance.py`'s sect parameters), if TP9 vs TP1 is what makes the columns diverge.
4. **Item numbers**, as before, plus the allowed status/regen `config:` keys.

## Hard rules
- Create ONLY:
  - `tools/gemini/out/round-6/`;
  - `docs/reviews/2026-10-07-gemini-round-6-matrix.md` (Vietnamese, ≤ 200 lines).
- No git. Do not modify game data, code or docs. Tables are verbatim tool output. The outputs must be exactly what produced the final tables.
- **Keep what passed** (measure the final state, with the same tools as round 5):
  - **fight length:** average 15–35 s per tier, and **≤ 20% of individual fights over 45 s in every tier**, not only globally. Today T3/T6/T9 are 22–24% by your own table, so this round must fix them too;
  - **rarity-step:** column averages 75–90%, and 100% cells **≤ 31**;
  - **tier-step:** T2 > T1 in 52–62%;
  - **PvE:** `py tools/balance/pve_check.py --items <items> --battle-config <config> --runs 200` → `Stages breaking a rule: 0/30`.
    - A base-HP change affects PvE, so re-check it.
    - You may adjust **enemy** blocks in `round-6/chapters.json` under the 4b rules.
    - Keep the expected player bags from `round-4b/player/` unchanged.
  - **no hard counters:** no build row average below 15% or above 85%.
- Work in steps and show the effect of each lever separately in the report:
  - base HP alone;
  - then the loadout rework;
  - then item and TP tuning.

  This way Claude can see which lever actually moves the matrix.

## Output
- `round-6/config.json`, which may now include `baseHp` / `hpPerLevel`.
- `round-6/patch.json` (items).
- `round-6/sects-final.json`, only if changed.
- `round-6/items-final.json`.
- `round-6/loadouts/`: all 24.
- `round-6/chapters.json`, only if changed.
- `round-6/tables/{baseline,step1_hp,step2_loadouts,final}_*`.

## Report
- **A met / not-met table at the top.** Be honest: write NOT MET with the closest value reached.
- **One table:** cells out after each step.
- **Every change with one line of reason.** For reworked loadouts, show old vs. new items.
- **Reproduction commands that run as written.**
- **At most 3 open questions.**
