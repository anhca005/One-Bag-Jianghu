# Lệnh cho Gemini: hệ phẩm theo từng bản vật phẩm + gộp nâng phẩm + ≥ 40 vật phẩm phi binh khí + chỉnh kinh tế

Dán toàn bộ phần dưới vào Antigravity. **Bản này thay bản trước** (nếu Gemini đang làm bản cũ, hãy dừng lại và dán bản này).

---

You are the game designer of "One Bag Jianghu" (2D wuxia backpack auto-battler, PC). The product owner changed the rarity rule (decision #24). Four parts: (A) rarity-as-instance system + merging, (B) convert the 21 existing items, (C) ≥ 40 new non-weapon items, (D) revise your economy and hero-level specs.

## Hard rules
- Create/modify ONLY: `docs/reviews/2026-09-29-gemini-rarity-items.md` (Vietnamese, ≤ 400 lines), `tools/gemini/out/rarity/rarity.json`, `tools/gemini/out/rarity/merge.json`, `tools/gemini/out/rarity/items_all.json` (all items, existing + new, in the new baseline format), and revise in place `docs/reviews/2026-09-29-gemini-economy.md`, `docs/reviews/2026-09-29-gemini-hero-level.md`, `tools/gemini/out/economy/*.json`. Nothing else (do NOT edit `server/` or the Unity project). No git.
- Game terms per `docs/design/glossary.md`; new names in a "Thuật ngữ đề xuất" table; wuxia names, **no names from Kim Dung novels or other games** (also avoid famous named artefacts from those novels).
- Items use ONLY the existing effect ops/triggers (see `docs/design/items.md` "Định dạng dữ liệu" and `server/battle-engine/src/main/resources/data/items.json`). The only format change allowed is what Part A needs for rarity. Sizes must fit a **fixed 5×4 bag**.

## Read first
`docs/decisions.md` #4, #12, #13, #14, #15, #21, #22, #23 and **#24** (rarity is a property of each owned copy; items.json holds **Phàm Phẩm baseline** stats; higher rarities multiply stats like Trọng does; Đột Phá stays at Trọng 3/6/9; **merge 2 copies of the same item, same rarity, both ≥ Trọng 3 → 1 copy one rarity higher, keeps about half the Trọng, refunds part of the upgrade materials**; normal merging (2 copies + materials + Bạc) goes up to Thần Binh (`legendary`); the final step **Thần Binh → Chí Tôn (`supreme`) is impossible with a normal merge** — it **mandatorily** requires an additional **special material obtainable only from Luận Kiếm Đài (PvP)**; all lower steps must NOT require it), `docs/design/items.md`, `docs/design/combat.md` (BattleConfig tier scaling), `docs/design/balance.md`, `server/battle-engine/src/main/resources/data/{items.json,sects.json}`, `server/battle-engine/samples/`, `tools/balance/README.md`, your specs `docs/reviews/2026-09-29-gemini-{economy,hero-level}.md`.

## Part A — rarity system & merging
1. **Rarity multipliers** (`rarity.json`): for each of common, uncommon, rare, epic, legendary, supreme — which stats scale (attack, heal, shield, status stacks, cooldown?, qi?) and by how much; how it **stacks with the Trọng multiplier** (multiply? add?). Show a table "effective power" for rarity × Trọng (6 × 9) and compare: e.g. is a Lương Trọng 1 weaker/stronger than a Phàm Trọng 9? The merge must never feel like a loss: justify.
2. **Merge rules** (`merge.json`): inputs (2 same item, same rarity, min Trọng), output rarity, output Trọng formula (e.g. floor(avg/2)+1 — propose), material refund %, Bạc cost per rarity step, whether Đột Phá unlocks carry over (they follow Trọng, so state the result). The **PvP-only special material** for the Thần Binh → Chí Tôn step: name/id (new term), counts per step, how it drops in Luận Kiếm Đài (per win, weekly/season rewards, Danh Vọng exchange) and expected time to afford one Chí Tôn merge. Edge cases: different Trọng inputs, supreme is the cap.
3. **Where copies come from** at each rarity (drops, Thương Hội pools by rarity, PvP) so merging is achievable; expected time to first Lương / Tinh / Tuyệt / Thần Binh copy of the main weapon.
4. Data format proposal: loadout item gets `"rarity": "rare"` next to `"tier"`; `ItemDef` no longer has a fixed rarity (or keeps a `dropMinRarity` if needed — justify). The engine change will be done by the programmer later — describe it precisely (fields, formula), do not implement.

## Part B — convert the 21 existing items
Rewrite their stats as Phàm baseline so that each item **at its current rarity** keeps roughly its current power (so existing balance results stay meaningful). Put them in `items_all.json`. List any item whose conversion is awkward.

## Part C — new non-weapon items
At least **10 armor, 10 manual (Công Pháp), 10 pill (Đan Dược), 10 relic (Kỳ Trân)**, baseline format, each with `unlocks` at Trọng 3/6/9, a clear build **role** (adjacency with weapon classes, burn/poison/bleed builds, shield/regen sustain, qi/parry, counters — decision #13), no pure stat sticks, no "always best". Per item: 1-line ChatGPT icon description (object only, Thủy Mặc style, size ratio matching its cells).

## Part D — revise the economy & hero-level specs
- Hero level (#23): bag fixed 5×4 (no growth), no stamina in the offline build, Tàng Bảo Các from level 1, keep PvP unlock at 5 + newbie protection; rework level rewards (features, Bạc, materials, catalysts, storage capacity, maybe a guaranteed higher-rarity copy at milestones); rename the level-up popup (no "Đột Phá"); Tâm Pháp upgrade cost with real material ids.
- Economy: upgrade cost table now indexed by the **copy's rarity** (you already have per-rarity columns — keep consistent); material drop groups weighted by item counts after Part C; every group has sources; Thương Hội sells copies at various rarities (pools/weights), material exchange (3 lower → 1 higher; cross-group at a loss) if needed; selling copies back (rarity + Trọng); no stamina costs. Re-check pacing (Trọng and rarity).

## Balance
The engine cannot read `rarity` yet, so you cannot run rarity simulations. Do this instead: run `tools/balance/balance.py` on the **current** engine with your Part C items at the existing tier levels (create 5–8 test loadouts under `tools/gemini/out/loadouts_non_weapon/`), report win rates at Trọng 1/3/6/9 (target #14: no build average > ~70%), and include the commands + raw numbers. State which checks must be re-run after the engine supports rarity.

Finish the report with the list of files changed and max 5 open questions.
