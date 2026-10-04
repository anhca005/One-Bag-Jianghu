# Lệnh cho Gemini: vòng 3 (tiếp), hoàn thành cân bằng + PvE + báo cáo

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are continuing **round 3** (`tools/gemini/task-stats-round-3.md`; read it again, every rule there still applies). Your previous session stopped before finishing. Claude reviewed what you left in `tools/gemini/out/round-3/`:

**Done, and accepted as a base:**
- `items-base.json`, `patch.json` and `items-v3-final.json`: the patch reproduces the final file exactly, and every non-weapon has a stat line.
- `config.json` (only the allowed keys).
- The 24 loadouts.

**Not done:**
- No report (`docs/reviews/2026-10-03-gemini-round-3-stats.md` does not exist).
- No `round-3/chapters.json`.
- No PvE `vs` tables (`balance/player` and `balance/enemy` exist, but there is no result).
- Part 1 numbers are missing: max HP and fight length per sample.

**Targets missed**, from your own verbatim tables in `scratch/`:
- Matrix A: **131/192** cells outside 35–65% (target ≤ ~58).
  - Builds far off: Huyền Độc ×2 (2% at T1), Huyết Thú Cuồng Trảo (0–12%), Hàn Băng Thương Pháp (10–28%), Thiết Phủ Liệt Diễm, Bá Khí Khai Sơn (4% at T6/TP9).
  - On the high side: Kim Cang Thiết Phủ (85–96%), Hàn Bích Hợp Bích, Bạch Hổ Bá Quyền.
- Matrix B: 31/48 and 32/48 outside.

## New decision #36 (read `docs/decisions.md`)
The **rarity table changed**: Phàm 1.0, Lương 1.1, Tinh 1.21, Tuyệt 1.33, Thần Binh 1.46, Chí Tôn 1.6. It is already the engine default (rebuild with `cd server && ./mvnw -q test` first). Your rarity-step table (97–99%) is obsolete.
- **New rarity-step target:** every column average within **75–90%**, no cell at 100%.
- The tier-step target stays 52–62%. Your current tier-step table mixes level changes into tier changes: run tier-step with the **same `--level`** on both sides, so it isolates Trọng.

## Do now
1. **Measure first:** re-run the baseline A, B, rarity-step and tier-step with the current engine and `items-v3-final.json` + `config.json`. Save the outputs verbatim in `round-3/tables/`.
2. **Part 1 table:** max Khí Huyết and the average fight length for the samples at (level 1, T1 Phàm), (level 10, T3 Lương), (level 30, T9 Chí Tôn). If fights mostly hit Kiệt Lực (45 s) or end in under 10 s, fix the scale before tuning.
3. **Part 2: tune until the targets hold.** Change items via the patch; regenerate `items-v3-final.json` from `items-base.json` + `patch.json`. Prefer fixing the outlier builds' key items over global changes. If a build cannot reach the band without breaking its identity, say so in the report instead of distorting it.
4. **Part 3: PvE.** Finish `round-3/chapters.json` (30 stages, `level` instead of `hp`) and the `vs` tables at the expected player power, inside the #34 bands. The curve must be smooth.
5. **Write the report** as specified in round 3 (Vietnamese, ≤ 300 lines). Every table verbatim, plus the reproduction commands.

Keep your scratch scripts in `round-3/scratch/`. Do not touch anything outside `tools/gemini/out/round-3/` and the report file. No git.
