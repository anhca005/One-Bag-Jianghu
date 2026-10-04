# Lệnh cho Gemini: vòng 2, cân bằng Items v2 (kèm vật phẩm quái) + sửa nội dung PvE

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

**Chỉ dán khi Claude báo luật engine mới (#34) đã vào Java.**

---

You are the balance and level designer of "One Bag Jianghu". Read `docs/decisions.md` **#34** first: it is Claude's review of your two round-1 reports. Then read:
- `docs/reviews/2026-10-01-gemini-items-v2-balance-1.md`
- `docs/reviews/2026-10-01-gemini-pve-content.md`
- `docs/design/pve.md`, `docs/design/balance.md`, `docs/design/glossary.md`
- `server/README.md` and `tools/balance/README.md`

## Hard rules
- Do NOT modify real data, code, docs or files outside your output folders. In round 1 the file `tools/gemini/out/items-v2/items-v2.normalized.json` was rewritten during your session. Treat it as **read-only**.
- No git.
- **No `config:` keys in patches.** Global rules such as the Hộ Thân cap are decisions for the user, not balance knobs. If you think a rule must change, ask in the open questions.
- Create ONLY:
  - `tools/gemini/out/round-2/`
  - `docs/reviews/2026-10-02-gemini-round-2.md` (Vietnamese, ≤ 300 lines)
- Every table must be **verbatim** from the tool. Patches must be exactly what produced the final tables. Use only approved mechanics (#32) and valid `weaponClass` values: `axe, blade, bow, brush, claw, dual, fan, fist, flute, hammer, hidden, palm, spear, staff, sword, whip, zither`.
- Original names only: nothing from Kim Dung or other famous works. Use our rarity names: Phàm, Lương, Tinh, Tuyệt, Thần Binh, Chí Tôn. Do not use "Vi Phẩm" or "Cực Phẩm".

## Part 1: Items v2 balance, round 2
- **Base:** `tools/gemini/out/items-v2/items-v2.normalized.json` + your round-1 patch **without** the `config:maxDmgReducePermille` line, + your 12 monster items (`tools/gemini/out/pve/monster-items.json`) after fixing them:
  - `kich_doc_chu_nga` must use a valid weaponClass;
  - rename `hoa_ngiem_xa_cot` → `hoa_diem_xa_cot`.

  Write this combined base as `round-2/items-base.json`.
- **Loadouts:** your 20 round-1 samples + the 2 monster-item builds + 2 more monster-item builds, so that each beast theme has a build. 24 in total, in `round-2/loadouts/`.
- **The engine now blocks the STATUS_APPLIED self-chain (#34).** Re-check the Huyền Độc builds (round 1: 90–98% at T6+) and every `rarity-step` 100% cell.
- **Targets:**
  - A (`--tiers 1,3,6,9 --tp 1,9 --runs 100`): outside 35–65% at most about 30% of the cells.
  - B (common and legendary, `--tiers 1,9 --tp 1`): the same target.
  - No build averages above 70% or below 30% in A.
  - Every `rarity-step` column average within 60–80%, with no single cell at 100%.
  - The monster-item builds sit inside 35–65% on average, like every other build.
- **Output:** `round-2/patch.json` (on top of `items-base.json`) and `round-2/items-v2-final.json` (the full catalog with the patch applied).

## Part 2: PvE content fixes (on top of `tools/gemini/out/pve/chapters.json`)
Use `items-v2-final.json`.
1. **Names:**
   - Rename "Độc Cô Hạnh" (character id and name) to an original name. Use the portrait key `npc_phong_loi`; its art prompt is already written.
   - Rename "Vạn Hoa Cốc" to an original place name.
   - Check every other name the same way.
2. **Sect per decision #21:** players have **no sect before Hiệp Khách level 10**.
   - The game has **no EXP curve yet**: the profile only stores `heroLevel` and `heroExp`. Design one as `round-2/levels.json`:

     ```json
     {"maxLevel": 30, "expToNext": [/* 29 integers, level 1→2 … 29→30 */], "unlocks": {"10": "sect"}}
     ```

     Codex will implement it.
   - Using `heroExp` from first-clear rewards, normal repeat rewards (`rewards.json`) and a sensible number of replays, land level 10 at a story beat. Suggested: end of Hồi 1 or early Hồi 2. Show the cumulative EXP per stage in a table.
   - Add a short dialogue where the hero is invited to choose a sect. It is only story: the actual sect choice is a separate UI.
   - Expected-player loadouts below level 10 have no `sect`.
3. **Difficulty (#34):** at the expected player power, normal stages win 80–90%, elite stages 60–70%, bosses 50–60%.
   - Retune the enemy bags, Trọng, phẩm and HP until every stage is inside its band, and paste the `vs` table verbatim.
   - The curve must be smooth: no step where the needed power jumps (round 1 had Hồi 3's boss at 26% with T4 and 95% with T5).
4. Fix the rarity names in all text.
5. **Output:**
   - `round-2/chapters.json`, in the exact pve.md format, with `monsterItems` listing the 12 ids;
   - `round-2/balance/`: the expected-player and enemy loadouts, and the `vs` tables.

## Report
- Part 1: baseline and final tables, diagnosis, every change with its reason.
- Part 2:
  - the rename list;
  - the level 10 beat;
  - the stage table: stage, enemy, expected player power, win rate, band OK?
- Reproduction commands.
- At most 4 open questions.
