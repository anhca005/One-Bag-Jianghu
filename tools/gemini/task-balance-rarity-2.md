# Lệnh cho Gemini: cân bằng theo phẩm — vòng 2

Chỉ dán **sau khi Claude báo công cụ đã sẵn sàng** (Codex đang thêm tùy chọn `--battle-config` và lệnh `rarity-step` cho `balance.py`). Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Your previous pass (`docs/reviews/2026-09-30-gemini-balance-rarity.md`) was **rejected**: the patch made balance worse (outside 35–65% at T1/T9 TP1 went from 23/52 to 31/52; your own table A went 107 → 116/208), and the patch file did not match the numbers in the report. Read `docs/decisions.md` **#28** first.

New since then (decision #28): shield (Cương Khí) is capped at 100% of max HP; Song Đao (`dual`) now belongs to Liệt Diễm and Cung (`bow`) to Hàn Sơn; `balance.py` can override engine constants with `--battle-config <json>` (e.g. `{"rarityMultiplierPermille":[1000,1150,1320,1520,1750,2000]}`) and has a `rarity-step` command. Read `tools/balance/README.md` again.

## Hard rules
- Do NOT modify real data, samples, code or docs other than your outputs. No git.
- Create ONLY: `tools/gemini/out/balance-rarity-2/` (loadouts, configs, patch, raw tables) and the report `docs/reviews/2026-09-30-gemini-balance-rarity-2.md` (Vietnamese, ≤ 250 lines).
- **Every table in the report must be pasted verbatim from the tool output**, and the final patch file must be exactly what produced the final tables. A patch is only acceptable if it **reduces** the outside-35–65% count versus baseline in every matrix you report; otherwise report "no patch" and explain.

## Steps
1. **Fix your 6 non-weapon test loadouts** (`tools/gemini/out/balance-rarity/loadouts/{burn_synergy,burst_heal,crit_haste,parry_counter,poison_drain,shield_sustain}.json`): they are under-built (e.g. Bích Thủy Hồi Xuân uses 11/20 cells with one weapon) and score 0–20%, which distorts every matrix. Rebuild each to a realistic full bag (≥ 16/20 cells, 2–4 weapons of its sect + supporting non-weapons, rarities like the samples). Put the 20 samples + fixed 6 in `tools/gemini/out/balance-rarity-2/loadouts/`.
2. **Rarity multipliers** — target: one rarity step ≈ **65–75%** win rate for the higher side (average over loadouts), Chí Tôn vs Thần Binh too. Run `rarity-step` (Trọng 3, TP 1, ≥ 100 runs) on the current multipliers, then search candidate sets via `--battle-config` (your 15% proposal, and smaller steps such as 8–10%). Report a table: candidate set → average win rate per step. Pick the set closest to the target; also report "Phàm T9 vs Lương T1" for it.
3. With the chosen multipliers (`--battle-config`), run baselines: A = native rarities `--tiers 1,3,6,9 --tp 1,9`; B = `--rarity common|rare|legendary|supreme --tiers 1,9`.
4. Balance items/Đột Phá/Tâm Pháp with `whatif`/`tune` (small parameter sets), prefer Đột Phá and Tâm Pháp milestones, keep item roles. Accept only if outside counts drop in A and in every B.
5. Report: chosen multipliers (as a `battle-config` JSON file), step table, baseline vs final tables (verbatim), each change with reason, exact reproduction commands, max 4 open questions.
