# Lệnh cho Gemini: vòng 4, cân bằng PvP + rút ngắn trận (giữ PvE)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Read `docs/decisions.md` #35–#39, your round-3 report `docs/reviews/2026-10-03-gemini-round-3-stats.md`, `docs/design/combat.md`, `tools/balance/README.md`.

## Status after round 3 (Claude verified)
- Your round-3 data is **now the game data**:
  - items: `server/battle-engine/src/main/resources/data/items.json`, the same as `items-v3-final.json`;
  - engine defaults: your `config.json` values;
  - 24 samples: `server/battle-engine/samples/`;
  - 30 stages: `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`.

  Rebuild first: `cd server && ./mvnw -q test`. No `--battle-config` is needed any more.
- **Accepted:** the PvE stage table (Claude re-ran all 30 from `chapters.json`: identical), rarity-step 80–89.5%, tier-step 58%.
- **Not met, and this round is about it:**
  1. **Matrix A: 131/192 cells outside 35–65%** (target ≤ ~58), basically unchanged from your baseline.
     - Builds averaging far below 35%: Thiết Phủ Liệt Diễm (4–34%), Băng Quy Tuyệt Hộ, Xích Xà Hỏa Liệp, Hỏa Hải Song Đao, Hàn Băng Thương Pháp, Xích Sa Chưởng Kình, Bá Khí Khai Sơn (5% at T6/TP9).
     - Builds far above 65%: Tiêu Dao Phiến Pháp (84–96%), Kiếm Đạo Hóa Kình (79–96%), Hàn Bích Hợp Bích, Bách Luyện Thuần Quyền, Kim Cang Thiết Phủ.
     - Your report did not say the target was missed. This time, state plainly what is and is not met.
  2. **Fights are too long:** 37–42 s on average, up to 60–75 s, past Kiệt Lực at 45 s. Target: average **15–35 s**, and most fights ending before 45 s.

## Hard rules
- Do not modify real data/code/docs. Create ONLY:
  - `tools/gemini/out/round-4/`
  - `docs/reviews/2026-10-04-gemini-round-4-pvp.md` (Vietnamese, ≤ 250 lines)
- No git. Tables verbatim from the tool. The patch must be exactly what produced the final tables.
- **Allowed `config:` keys:** `burn.damage`, `poison.damage`, `bleed.damage`, `bleed.maxHpPermille`, `exhaustionStep`, `maxRegen`. Anything else (`exhaustionStartMs`, caps, tier/rarity tables, `baseHp`, `hpPerLevel`) may only be **proposed** with evidence in the open questions.
- Reproduction commands must run as written. `balance.py` has no `tier-step` command: give the exact script path you used (e.g. `tools/gemini/out/round-4/scratch/run_tier_step.py`).

## Do
1. **Baseline:** matrix A (`--tiers 1,3,6,9 --tp 1,9 --runs 100`), matrix B (common/legendary, `--tiers 1,9 --tp 1`), rarity-step, tier-step, plus a **fight-length table** (average and share > 45 s per tier) on the current game data. Save them verbatim in `round-4/tables/baseline_*`.
2. **Fight length first:** find why fights drag (too much sustain/shield/Hộ Thân vs. damage, regen, low weapon damage after the ÷3.5 rescale). Fix it with item numbers and the allowed config keys.
3. **Then the matrix:** fix the outlier builds via their key items. Prefer targeted changes. Explain each change.
4. **Keep what passed:**
   - rarity-step 75–90% per column, no 100% cell;
   - tier-step 52–62%;
   - **re-run the 30 PvE stages** with the new items (Claude's method: each `balance/player/stage_NN.json` from round 3 vs. the stage enemy in `chapters.json`). Every stage must stay in its #34 band. If it drifts, adjust the enemy bags/levels in `round-4/chapters.json`.
   - **New starter kit (user decision #39): 1 sword + 1 armor** — `thanh_phong_kiem` T1 + `o_tam_y` T1, Phàm (see `One-Bag-Jianghu/Assets/_Game/Resources/Data/starter.json`). The old third item (Thiết Cốt Quyền Sáo) is gone. Claude measured: this kit wins 0% against the round-3 stage 1–3 enemies, so Claude temporarily set stage 1's enemy to `thiet_cot_quyen_sao` + `thiet_giap_y` (wins 100%). Rebuild the expected-player bags for the early stages from **the starter kit + first-clear rewards + what the starting 1000 Bạc and early rewards can buy**, and retune stages 1–5 to their bands with those bags. Stage 1 should be a gentle tutorial (normal band 80–90%).
   - User answers to your round-3 questions: no "enemy uses poison" hint in the UI; bosses one level above the player with Tuyệt gear are fine.
5. **Output:**
   - `round-4/patch.json` (on top of the current game `items.json`);
   - `round-4/config.json` (allowed keys only);
   - `round-4/items-final.json`;
   - `round-4/chapters.json` (only if changed);
   - `round-4/tables/final_*`.
6. **Report:**
   - a met / not-met summary at the top;
   - baseline vs. final tables;
   - every change with its reason;
   - PvE re-check table;
   - reproduction commands;
   - at most 4 open questions.
