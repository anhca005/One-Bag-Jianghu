# Codex task SU: show the #35 stat lines and Items v2 mechanics in the Unity UI

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read `docs/decisions.md` #35, #36 and #32, `docs/design/combat.md` ("Chỉ số Hiệp Khách"), `docs/design/glossary.md`, and `tools/codex/spec-stats-35.md`.

The game data is now Items v3 (Gemini round 3): `Resources/Data/items.json`, 73 items. Every non-weapon has `stats` (`hp`, `dmgReduce` or `cooldownReduction`). Relics and manuals may have `active` + `cooldown`. There are new triggers and ops.

## Problems
1. **`ItemDescriber`** (`Scripts/UI/Common/ItemDescriber.cs`) prints raw op names. Seen: "Phổ Công: Sát thương 100% Lực Đạo, +2 nội lực, **BUFF_STACK**".
   - Render every Items v2 element in Vietnamese, in the same wording as the sheet exporter `tools/sheets/export_xlsx.py` (`describe_effect` / `describe_trigger`; read it and mirror its phrases so the sheet and the game say the same thing):
     - ops `buff_stack` (`stat`, `maxStacks`, `target`), `cleanse`, `lifesteal`, `raise_max_hp`, `minQiAfter`;
     - triggers `damaged`, `healed`, `controlled`, `crit_taken`, `enemy_ultimate`, `qi_empty`, `shield_broken`, `burst_taken`, `from: enemy`;
     - `active` + `cooldown` ("Chiêu chủ động (mỗi X giây): …").
   - No raw identifiers may reach the UI: add a test that describes all 73 items and asserts no uppercase `SNAKE_CASE` or unknown op names appear.
2. **The stat line is not shown.** In the tooltip (Bao Phục hover), the Tàng Bảo Các detail panel and the Thương Hội card/detail, show the item's stat line, **scaled by its tier and rarity** with the engine's truncation (tier then rarity):
   - Binh Khí: "Công: N" (replaces/merges the current "Lực Đạo" line; glossary: Công = Lực Đạo của món).
   - Hộ Cụ / Kỳ Trân: "Khí Huyết +N".
   - Công Pháp: "Giảm Hồi Chiêu +N%".
   - Đan Dược: "Hộ Thân +N%".

   Read the multipliers from the engine `BattleConfig` defaults (as `RarityRules` does now). Do not hard-code tables.
3. **Bao Phục has no character totals.** Add a small stats panel to Bao Phục (builder: `BagPrefabBuilder` / `BagLoadoutPanel`):
   - Cấp;
   - Khí Huyết tối đa (base 100 + 5 × (cấp − 1) + the bag's `stats.hp`);
   - Hộ Thân %, with the 80% cap note when hit;
   - Giảm Hồi Chiêu %, with the 40% cap note.

   Compute with the C# engine itself: `Fighter` creation, or a small public helper in the engine that mirrors Java's `BattleSimulator.fighterStats`. If the helper needs an engine change, write it to `tools/codex/staging/Engine/` (you cannot write `Scripts/Engine/**`), and also update `tools/codex/staging/README.md`. The panel updates live while items are moved.
4. Use the font roles (`UiFonts`) and the text-fit presets already in `UiBuild`. Builders only, no prefab edits.

## Rules
- Edit `Assets/_Game/Scripts/**` (not `Scripts/Engine/**`), `Assets/_Game/Editor/**`, `Assets/_Game/Tests/**`, and `tools/codex/staging/**`. **No git.** Do not run Unity: the license fails in your sandbox. Claude runs it.

## Tests
- Describer: all 73 items, no raw identifiers; a `buff_stack` and an `active` line read correctly.
- Stat line scaling: e.g. an armor `hp 140` at T3 Tinh → 140*1100/1000=154 → 154*1210/1000=186.
- Bao Phục totals for a known bag match the engine's `maxHp` at battle start.

## Result
The changed files, the staged engine files (if any) with copy commands, and screenshots to check (which popup, which item).
