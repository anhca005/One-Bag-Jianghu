# Lệnh cho Gemini: vòng 7, đường cong PvE tính cả kinh tế (người dùng chọn hướng c)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity (chọn model nào cũng được), dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Read first:
- `docs/decisions.md` #34, #39, #40, #42, #44, #45;
- `docs/design/pve.md`, `docs/design/combat.md`;
- your reports for rounds 4b and 6 in `docs/reviews/`;
- `tools/balance/README.md`, `tools/balance/pve_check.py`, `tools/balance/pve_bags.py`.

Rebuild first: `cd server && ./mvnw -q test`.

## The problem (the user played it; Claude measured it)
The user started with the starter kit (Thanh Phong Kiếm + Ô Tàm Y). They got stuck at **stage 2**. Then they bought **2 common tier-1 items** in the shop (Tử Mẫu Song Đao + Bồi Nguyên Đan, about 400 Bạc) and **cleared all of chapter 1** easily.

`py tools/balance/pve_bags.py --runs 200 --stages 20` shows:

| Stage | A: starter only | B: + Song Đao + Bồi Nguyên Đan | C: round-4b expected bag |
|---|---|---|---|
| 1 | 83% | 100% | 83% (2 items) |
| 2 | **6%** | 100% | 85% (3) |
| 3–9 | 0–7% | 92–100% | 84–88% (5–6) |
| 5 elite | 0% | 82% | 67% |
| 10 boss | 0% | 16% | 58% (7) |
| 12 | 0% | 38% | 87% |
| 14–20 | 0% | 0–8% | 84–87% |

Diagnosis:
1. **Stage 2 is a cliff.** The curve assumes the player already holds items they may not have.
2. **Shop power is too cheap.** The player starts with 1.000 Bạc and a common item costs 200, so they buy 2–5 items at once. The round-4b curve assumes items arrive slowly from stage rewards. A 4-item common T1 bag beats the 5–6-item bags the stages were tuned for.
3. **The curve is uneven.** Chapter 1 is trivial with shop items, while chapter 2 is a wall (stage 12 38%, 14+ ≈ 0%).
4. **Possible outlier:** Tử Mẫu Song Đao (`dual`, 2×2, cooldown 2.0) may be stronger than other T1 weapons. Measure it.

## New rule: replay rewards (decision #47, 2026-10-04)
Winning a stage that is already cleared pays **15% of that stage's first-clear reward**:
- **Bạc and EXP:** 15%, rounded, minimum 1.
- **Materials:** 15% of each count. The fractional part is the chance of one extra; for example, 3 → 45% chance of 1.
- **First-clear items:** not repeated.
- **Stage drops:** unchanged.

This replaces the flat `rewards.json` "win" pack for PvE. The percentage is `repeatRewardPercent` in `chapters.json`.

In the progression model, add an optional **"farming" assumption**: the player replays the hardest cleared stage K times before attempting stage N.
- Report Bạc available with K = 0 and with K = 3.
- Targets apply to K = 0. For K = 3, just report the numbers.

Do not change the 15% value. If you think it is wrong, say so in an open question.

## User decision: (c) combine both levers
- Make the stages respect what the player can **actually** own.
- Also slow down shop power somewhat.

You propose the balance between the two levers.

## Task
1. **Build a realistic progression model**, `tools/gemini/out/round-7/progression.py`, a script Claude can rerun.
   - **For each stage N it computes:**
     - Bạc available: starter currencies (`Resources/Data/starter.json`), plus first-clear rewards of stages 1..N−1 (`chapters.json`), plus win rewards (`rewards.json`, if PvE uses it);
     - items and materials owned from rewards.
   - **It produces two bags per stage:**
     - **"no-shop":** starter + reward items only;
     - **"shop-smart":** also spends the available Bạc in the shop on the best value it can buy. Use the shop rules in `shop.json` and `ShopService`: slot rarities and weights, base prices, refresh cost. Model this as the expected best buy over a few refreshes; document the method.
   - **Player level per stage:** from `levels.json` EXP plus the stage EXP rewards, assuming each stage is cleared once.
2. **Per-weapon audit at T1 common:** starter + Bồi Nguyên Đan + each player weapon, on stages 2, 5, 9, 10 and 12, 200 runs each.
   - List the outliers: any weapon more than 15 points above the median.
   - Propose number fixes **only** for real outliers.
3. **Targets** (measure every one with ≥ 200 runs per stage):
   - **"no-shop" bag:** wins every stage at **≥ 55%** (boss ≥ 45%). No cliff: stage-to-stage drop ≤ 20 points.
   - **"shop-smart" bag:** wins at **≤ 90%** on normal stages and **≤ 75%** on elite/boss stages. Buying helps but never trivialises.
   - **Gap between the two bags:** about 10–25 points on average.
   - **Keep the #34 bands:**
     - **normal:** 80–90% for the intended bag, which is now the **average of "no-shop" and "shop-smart"**;
     - **elite:** 60–70%;
     - **boss:** 50–60%.

     If this conflicts with the two limits above, say so and propose which to relax.
   - **Average fight length:** ≤ 40 s on every stage.
   - **Chapter 2 must not be a wall** for a player who followed the curve through chapter 1.
4. **Allowed levers:**
   - **Enemy blocks** in `chapters.json`: items, tier, rarity, level within the round-4b rules (boss = recommendedLevel + 1; others within rec ± 2, ≤ 31).
   - **Stage first-clear rewards:** Bạc, items, materials.
   - **Starter currencies and starter items.**
   - **Shop:**
     - `itemBasePrices`;
     - slot rarity weights;
     - refresh costs;
     - the first slot's rarity pool. For example, the cheapest slot may offer only common items early.
   - **Item numbers:** only for outliers found in task 2.

   **Do not** change engine code, `config` constants or other items. PvP balance (matrix A) is paused (#44), but report if your item changes move it: run matrix A once at the end, 100 runs.

## Hard rules
- **Create only:**
  - `tools/gemini/out/round-7/`;
  - `docs/reviews/2026-10-09-gemini-round-7-pve-economy.md` (Vietnamese, ≤ 220 lines).
- **No git.** Do not modify game data, code or docs; Claude integrates.
- **Outputs that must be exactly what produced the final tables:**
  - `round-7/chapters.json`;
  - `round-7/shop.json` (if changed);
  - `round-7/starter.json` (if changed);
  - `round-7/patch.json` (item changes, or `{}`);
  - `round-7/players/no_shop/stage_NN.json` and `round-7/players/shop_smart/stage_NN.json` for all 30 stages;
  - `round-7/tables/*.txt`: verbatim tool output.
- **Names:** no Kim Dung names; terms from `docs/design/glossary.md`.

## Report
- **Met / not-met table** at the top. Be honest and give the closest value reached.
- **Per-stage table for all 30 stages:** stage, kind, enemy level, player level, Bạc available, "no-shop" win%, "shop-smart" win%, average fight length.
- **Weapon audit table** and any outlier fixes, with one line of reason each.
- **Every change to shop, starter or rewards** with old → new values and one line of reason.
- **Reproduction commands** that run as written. On Windows the Java classpath must be run from PowerShell, because Git Bash breaks `;`.
- **At most 3 open questions.**
