# Lệnh cho Gemini: vòng 5, ma trận A ≤ ~58 + đo trần Hộ Thân 60%

**Dán SAU khi vòng 4b xong và Claude đã nhận.** Vòng này đổi vật phẩm, nên PvE vòng 4b phải được giữ.

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Read:
- `docs/decisions.md` #35–#41;
- your reports `docs/reviews/2026-10-04-gemini-round-4-pvp.md` and `docs/reviews/2026-10-05-gemini-round-4b-pve.md`;
- `docs/design/combat.md`;
- `tools/balance/README.md`;
- `tools/balance/pve_check.py`.

Rebuild first: `cd server && ./mvnw -q test`. The game data (items, engine defaults, `chapters.json`) already holds your round-4 and 4b results.

## User decisions (#41)
1. **The matrix A target stays:** ≤ ~58/192 cells outside 35–65% (`--tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10`). Today it is 103/192. Sect counters may exist, but no build may be a hard counter (no cell < 15% or > 85% in a build's row average).
2. **Measure lowering the Hộ Thân cap from 80% to 60%** (`maxDmgReducePermille` 800 → 600). This is an experiment only. Claude decides based on your evidence.

## Hard rules
- Create ONLY:
  - `tools/gemini/out/round-5/`;
  - `docs/reviews/2026-10-06-gemini-round-5-matrix.md` (Vietnamese, ≤ 200 lines).
- No git. Do not modify game data, code or docs. Tables must be verbatim tool output. The patch must be exactly what produced the final tables.
- **Allowed `config:` keys:** `burn.damage`, `poison.damage`, `bleed.damage`, `bleed.maxHpPermille`, `exhaustionStep`, `maxRegen`. For the experiment in step 2 only, you may also set `maxDmgReducePermille`.
- **Keep what passed:**
  - fight length: average 15–35 s per tier, and ≤ 20% of individual fights over 45 s. Measure **per fight, not per matchup**; give the script path;
  - rarity-step: column averages 75–90%. Also report the number of 100% cells (today 31) and **do not increase it**;
  - tier-step: T2 > T1 in 52–62%;
  - PvE: `py tools/balance/pve_check.py --items <your items> [--battle-config <your config>] --runs 200` must print `Stages breaking a rule: 0/30`. If it does not, you may adjust enemy bags in `round-5/chapters.json` under the round-4b rules.

## Do
1. **Baseline:**
   - matrix A;
   - rarity-step (with the count of 100% cells);
   - tier-step;
   - fight length;
   - `pve_check`.

   Save each in `round-5/tables/baseline_*`.
2. **Hộ Thân cap experiment:**
   - Run the same five measurements with only `maxDmgReducePermille: 600` and save them in `round-5/tables/cap60_*`.
   - Say which builds move, by how much, and whether it helps the matrix or only shifts it.
   - Recommend keep-80 or switch-to-60. Base the recommendation on numbers.
3. **Matrix:**
   - Reduce cells out to ≤ ~58 with targeted item changes on the outlier builds' key items.
   - Use cap 80 unless step 2 showed that 60 is clearly better; in that case say so and use it.
   - Explain each change in one line.
4. **Output:**
   - `round-5/patch.json`;
   - `round-5/config.json`;
   - `round-5/items-final.json`;
   - `round-5/chapters.json` (only if changed);
   - `round-5/tables/final_*`.

## Report
- A met / not-met table at the top. Be honest: a target that is not met is written as NOT MET, with how close it got.
- The cap-60 experiment, with evidence and a recommendation.
- Baseline vs. final tables.
- Every change with its reason.
- Reproduction commands that run as written.
- At most 3 open questions.
