# Lệnh cho Gemini: vòng 3, hệ chỉ số trang bị (#35) + cân bằng lại vật phẩm và ải PvE

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

**Chỉ dán khi Claude báo engine Java đã có hệ chỉ số #35** (`stats`, `level`, `--level` trong balance tool).

---

You are the balance and level designer of "One Bag Jianghu". The combat rules changed. Read:
- `docs/decisions.md` **#35** (new), plus #34 and #32;
- `docs/design/combat.md`, section "Chỉ số Hiệp Khách";
- `tools/codex/spec-stats-35.md`, the exact engine contract;
- your round-2 report `docs/reviews/2026-10-02-gemini-round-2.md` and outputs in `tools/gemini/out/round-2/`;
- `docs/design/pve.md`, `docs/design/glossary.md`, `server/README.md`, `tools/balance/README.md`.

## What changed
- Player HP is no longer 1200. It is **100 + 5 × (level − 1)** plus the items' stat lines.
- Every item has one stat line by type:
  - Binh Khí: **Công** = its own `attack`;
  - Hộ Cụ: `stats.hp`, the main HP source;
  - Kỳ Trân: `stats.hp`, smaller than Hộ Cụ;
  - Công Pháp: `stats.cooldownReduction`, capped at 40% in total;
  - Đan Dược: `stats.dmgReduce`, a permanent Hộ Thân sharing the 80% cap.
- Multipliers apply to stats and to effects:
  - Trọng is gentle: 1.00 → 1.40.
  - Phẩm is strong: Phàm 1.0, Lương 1.3, Tinh 1.7, Tuyệt 2.2, Thần Binh 2.8, Chí Tôn 3.5.
- Loadouts use `level` instead of `hp`. PvE monsters use the **same formula**: no per-stage HP, only the enemy `level` and their bag.

Your round-2 **numbers** were tuned for 1200 HP and are superseded. **Keep** from round 2:
- the renames: Phong Vũ / `npc_phong_loi`, Bách Thảo Cốc;
- the story and dialogue, including the level-10 sect beat after stage 10;
- `levels.json` (you may adjust it if your new reward pacing needs it);
- the monster item fixes.

## Hard rules
- Do NOT modify real data, code, docs or files outside your output folders. Inputs are **read-only**.
- No git.
- Create ONLY:
  - `tools/gemini/out/round-3/`
  - `docs/reviews/2026-10-03-gemini-round-3-stats.md` (Vietnamese, ≤ 300 lines)
- Every table must be **verbatim** from the tool. Patches must be exactly what produced the final tables.
- Use only approved mechanics (#32) and valid `weaponClass` values.
- **`config:` keys:** this round you MAY change only these, because their absolute values were sized for 1200 HP:
  - `burn.damage`, `poison.damage`, `bleed.damage`, `bleed.maxHpPermille`;
  - `exhaustionStep`, `maxRegen`.

  The tier/rarity tables, `baseHp`, `hpPerLevel` and the caps are the user's decision: you may **propose** different values in the open questions with evidence, but the final tables must use the defaults.
- Original names only. Our rarity names: Phàm, Lương, Tinh, Tuyệt, Thần Binh, Chí Tôn.

## Part 1: stat budget + rescale
1. Design a **stat budget** table: the stat value per item type × size (cells) at the item's base rarity. Hộ Cụ gives more HP per cell than Kỳ Trân. Write the rule (formula) and apply it to every non-weapon item in `round-2/items-v2-final.json`, monster items included.
2. **Rescale** every absolute number to the new HP scale: weapon `attack`, effect `value` for shield, heal, `raise_max_hp`, flat damage, and so on. Rates, durations, cooldowns and qi do not need rescaling unless balance asks for it.
   - Target: at level 1, all T1 Phàm, a typical sample build has **about 250–400 max HP**, and a mirror-power fight lasts **15–35 s** (mostly ending before Kiệt Lực at 45 s).
   - Report the max HP and the average fight length for the samples at (level 1, T1 Phàm), (level 10, T3 Lương), (level 30, T9 Chí Tôn).
3. Write `round-3/items-base.json`, the rescaled catalog before tuning.

## Part 2: balance
- **Loadouts:** your 24 round-2 loadouts converted to `level`, in `round-3/loadouts/`.
- **Targets** (use `--level` matching the tier, e.g. T1→1, T3→8, T6→18, T9→30, and say which):
  - A (`--tiers 1,3,6,9 --tp 1,9 --runs 100`): outside 35–65% at most about 30% of the cells; no build averages above 70% or below 30%.
  - B (common and legendary, `--tiers 1,9 --tp 1`): the same target.
  - `rarity-step`: one rarity step up should now matter a lot. Every column average within **70–85%**, no single cell at 100%.
  - A one-Trọng step should be a small edge: report a tier-step check (the same build at T(n+1) vs T(n), same rarity). Target **52–62%**.
  - The monster-item builds sit inside 35–65% on average.
- **Output:**
  - `round-3/patch.json`, on top of `items-base.json`;
  - `round-3/config.json`, only the allowed keys;
  - `round-3/items-v3-final.json`, the full catalog with the patch applied.

## Part 3: PvE
On top of `round-2/chapters.json`, using `items-v3-final.json`:
- Replace every enemy `hp` with `level`. Enemy levels rise smoothly through the 30 stages.
- The expected player at each stage has:
  - the level from `levels.json` and your reward pacing table;
  - a bag of tier, rarity and items a player could plausibly own by then;
  - no `sect` below level 10.
- Difficulty bands (#34): normal 80–90%, elite 60–70%, boss 50–60%. Paste the `vs` table verbatim. The curve must be smooth, with no step where the needed power jumps.
- **Output:**
  - `round-3/chapters.json` (exact pve.md format, `level` instead of `hp`, `monsterItems` listing the 12 ids);
  - `round-3/balance/`: the expected-player and enemy loadouts, and the `vs` tables;
  - `round-3/levels.json`, only if changed.

## Report
- Part 1: the budget table and formula, the rescale rule, and the HP and fight-length table.
- Part 2: the baseline and final tables, the diagnosis, and every change with its reason.
- Part 3: the stage table: stage, enemy level, player level/tier/rarity, win rate, band OK?
- Reproduction commands.
- At most 4 open questions, including any proposal for the tier/rarity tables.
