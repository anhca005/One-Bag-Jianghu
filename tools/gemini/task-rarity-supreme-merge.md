# Lệnh cho Gemini (bổ sung): gộp Thần Binh → Chí Tôn bằng nguyên liệu Luận Kiếm Đài

Gemini đã làm xong `task-items-non-weapon.md` theo luật cũ ("Chí Tôn chỉ có ở Luận Kiếm Đài, gộp dừng ở Thần Binh"). Người dùng đổi luật (quyết định #24). Lệnh này chỉ sửa phần liên quan, giữ nguyên phần còn lại.

Dán toàn bộ phần dưới vào Antigravity.

---

You are the game designer of "One Bag Jianghu". You just finished the rarity/merge/items task. The product owner **changed one rule** in decision #24 (re-read it in `docs/decisions.md`):

- Normal merging (2 copies of the same item, same rarity, both ≥ Trọng 3, + materials + Bạc) goes up to **Thần Binh (`legendary`)** — unchanged.
- **New:** a player CAN merge **2 Thần Binh → 1 Chí Tôn (`supreme`)**, but this step **mandatorily** requires an additional **special material obtainable only from Luận Kiếm Đài (PvP)**. Without it the merge is impossible. No lower step may require this material.
- Chí Tôn is no longer "PvP exclusive" as a copy: remove or rethink "Chí Tôn copies as PvP season rewards" (you may keep a rare Chí Tôn reward only if you justify it; the main path is the merge).

## Hard rules
- Modify ONLY (in place): `docs/reviews/2026-09-29-gemini-rarity-items.md` (stay ≤ 400 lines), `tools/gemini/out/rarity/merge.json`, `tools/gemini/out/rarity/rarity.json` (only if needed), `docs/reviews/2026-09-29-gemini-economy.md`, `docs/reviews/2026-09-29-gemini-hero-level.md`, `tools/gemini/out/economy/*.json`. Do not touch items, loadouts, `server/` or the Unity project. No git.
- Keep everything else you delivered (multipliers, Part B/C items, balance results) unless this rule forces a change; list every change you make.
- New names go in the "Thuật ngữ đề xuất" table; wuxia flavour, **no Kim Dung names or famous artefacts from those novels, no names from other games**.

## Do
1. **The PvP-only special material**: Vietnamese name + snake_case id, meaning, 1-line ChatGPT icon description (object only, Thủy Mặc style, 1:1). It is one material for all item types (or justify per type). Add it to `tools/gemini/out/economy/materials.json` with a flag/field making clear it only drops in Luận Kiếm Đài.
2. **`merge.json`**: add the `legendary → supreme` step: count of the special material, Bạc cost, output Trọng (same formula as other steps or justify a different one), refund %. Replace `max_merge_rarity` / `supreme_mergeable` with a data shape that lets each step declare extra required materials (e.g. `steps: [{from, to, silver, extra_materials: {id: count}}]`) so the programmer can read it generically. `supreme` is the cap.
3. **Where it comes from** (`rewards.json` + economy doc): per PvP win/loss/draw (chance or count), weekly/season rewards by rank, Danh Vọng exchange in Thương Hội (price, weekly limit). Must not drop in PvE, must not be bought with Bạc/Nguyên Bảo.
4. **Pacing**: expected PvP matches/weeks for an active player to afford ONE Chí Tôn merge (target: a long-term goal, roughly several weeks of regular PvP, not days, not months). Also the time to have 2 Thần Binh copies of the same item at ≥ Trọng 3.
5. **Update** the rarity report (Part A.2, A.3, the "Chí Tôn độc quyền" wording, open questions — drop the question about Chí Tôn season copies at Trọng 1 or 3 if it no longer applies), the economy doc (upgrade costs for `supreme` copies must exist), and the hero-level doc if it mentions Chí Tôn.

Finish with the list of changed files, what changed in each, and max 3 open questions.
