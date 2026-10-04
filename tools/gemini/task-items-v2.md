# Lệnh cho Gemini: làm lại nội tại vật phẩm (Items v2)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the game designer of "One Bag Jianghu" (wuxia backpack auto-battler). The user reviewed the item sheet and found the **item passives too samey**. The numbers confirm it: across 61 items, triggers are 86× `battle_start` and 40× `hp_below`, versus 17 `crit`, 14 `burst`, 12 `parry_success`, 10 `hit`, 7 `ultimate`, 2 `status_applied`. Effects are dominated by `gain_qi` 55, `shield` 53 and `heal` 30. Only weapons act on a cooldown; armour, manuals, pills and relics are pure passives.

The user collected a reference sheet from another game: `docs/art/tham_khao/tong_hop_vat_pham_tham_khao.xlsx` (73 items: 6 weapon classes, armour, soul jades, consumables, active relics). **Use it only for mechanics ideas.** Never copy its item names, its terms ("Thế kiếm", "Tụ lực", "Đỡ đòn", "Hồn Ngọc"…) or its exact numbers. Our terms are in `docs/design/glossary.md` (Nội Lực, Tuyệt Kỹ, Hóa Kình, Cương Khí, Hội Tâm, Vận Khí, Khinh Công, Trì Trệ, Điểm Huyệt, Định Thân…). No names from Kim Dung novels.

**Be creative. The reference is only a springboard; do not copy it.** The user explicitly asked for this. Concretely:
- Do not port reference items one-to-one onto ours. A Part B design that is just a reference item with renamed terms is a failure.
- At least half of the Part A mechanics and at least half of the Part B designs must be **original ideas** that are not in the reference sheet. Good sources are wuxia themes and our own systems: Nội Lực flow, Hóa Kình mind-games, Tâm Pháp, bag positions, and the burst-threshold statuses.
- In `changes.csv`, add a column `origin` with one of three values:
  - `original`: your own idea.
  - `inspired`: a reference idea changed in a meaningful way. Say what you changed.
  - `adapted`: close to a reference idea.

  `adapted` may be at most about 20% of the items.

Read first: `CLAUDE.md`, `docs/design/combat.md`, `docs/design/balance.md`, `docs/design/glossary.md`, `docs/decisions.md` (especially #13 counters and #25–#31), the engine data model (`server/battle-engine/src/main/java/com/onebagjianghu/battle/data/*.java`: `EffectOp`, `TriggerEvent`, `TriggerSource`, `Target`, `StatusType`, `ItemDef`, `UnlockDef`), `server/battle-engine/src/main/resources/data/items.json` and `sects.json`.

## Hard constraints
1. **No new art.** Keep all 61 items: same `id`, `name`, `type`, `size` and the same visual concept as the art prompts in `docs/art/prompts-items-mvp.md` and `docs/art/prompts-items-nonweapon.md`. A new passive must fit the item's picture and name. Example: a jade pendant can calm the qi, but it cannot suddenly shoot fire. Weapon class (`weaponClass`) and sect ties stay. If you are convinced an item's concept does not work, list it as an open question. Do not rename it.
2. Keep the core rules: server-authoritative deterministic simulation, Nội Lực / Tuyệt Kỹ / Hóa Kình, and the burst-threshold statuses (Liệt Hỏa / Kịch Độc / Xuất Huyết accumulate points and burst at the threshold). **Do not** switch to the reference's "damage per second = stacks" model.
3. Tier and rarity scaling (decisions #25 and #30) and Đột Phá milestones T3/T6/T9 stay. Every item keeps (or gets) meaningful Đột Phá unlocks.
4. Do NOT modify real data, code or docs. No git. Create ONLY:
   - `docs/reviews/2026-09-30-gemini-items-v2.md` (Vietnamese report, ≤ 300 lines)
   - `tools/gemini/out/items-v2/` (JSON drafts and tables)

## Part A: new engine mechanics (proposal; keep the list short)
Compare the reference mechanics with our engine and propose **at most 10** new mechanics that give the most variety per unit of engine work. Candidates, not a mandate:
- **Item-level control:** freeze an enemy item's cooldown (Điểm Huyệt already exists; decide whether it is enough), slow and haste on specific items.
- **Multicast:** the weapon triggers its Phổ Công N times.
- **Lifesteal** on weapon damage.
- **Stacking buffs** with a cap ("+X Lực Đạo, up to N stacks") and **timed buffs** ("+X for 3 s").
- **New triggers:** on being healed, on taking damage, on an own item being controlled (frozen or slowed), on a Phổ Công hit vs a Tuyệt Kỹ hit, on applying a specific status.
- **Conditional Tuyệt Kỹ:** a bonus if Nội Lực is still ≥ N after paying.
- **Enemy Nội Lực reduction** (we have `drain_qi`), **raise max Sinh Lực**, **cleanse**.
- **Active non-weapons:** relics and manuals with their own cooldown that fire an effect, instead of only passive triggers.

For each mechanic give: an English identifier in the style of the engine enums, the exact semantics (targets, timing within a tick, rounding, caps, interaction with Hóa Kình and Định Thân), the Vietnamese UI wording, and why it adds variety. Mark each one **Required** (the redesign depends on it) or **Nice-to-have**. The user must approve this list before any code is written.

## Part B: redesign all 61 items
For every item write a new passive design using existing mechanics plus Part A mechanics. Goals:
- Each category gets its own identity:
  - Binh Khí: active damage and attack patterns.
  - Hộ Cụ: defence with a reactive twist.
  - Công Pháp: rule-bending passives or Tâm Pháp synergies.
  - Đan Dược: one-shot conditional bursts.
  - Kỳ Trân: active cooldown relics or build-around engines.
- **Trigger variety:** at most about 25% of all triggers may be `battle_start`, and `hp_below` at most about 12%. Report the before/after trigger and op distribution tables.
- Build-arounds: every sect (7) must get at least 2 non-weapon items that reward its playstyle, and the adjacency/position puzzle should matter more (targets such as `ADJACENT_*`, "items to the left/right" only if cheap).
- Keep the counters of decision #13 intact.
- Budget: use `docs/design/balance.md` rules. Give each item an estimated budget % (a rough estimate is fine; Claude will simulate later).

## Outputs
1. `tools/gemini/out/items-v2/mechanics.md`: Part A as a table plus details.
2. `tools/gemini/out/items-v2/items-v2.json`: the full catalog in the **current** `items.json` format. New mechanics use the proposed identifiers, and each item containing one is flagged with `"_needs": ["MECH_ID", ...]`. Items using only existing mechanics must pass the current engine: run `py tools/balance/balance.py matrix --items tools/gemini/out/items-v2/items-v2-existing-only.json ...` on a copy that strips/skips `_needs` items, or explain why this is not possible.
3. `tools/gemini/out/items-v2/changes.csv`: `id, name, type, old passive (short), new passive (Vietnamese UI text), mechanics used, budget %`.
4. The report: summary, Part A table, before/after distributions, per-category identity, sect coverage table, list of items whose concept felt forced, and **at most 5 open questions** for the user.
