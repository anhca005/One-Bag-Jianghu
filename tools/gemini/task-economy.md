# Lệnh cho Gemini: thiết kế kinh tế — nguyên liệu nâng Trọng, bảng chi phí, Thương Hội (NPC), phần thưởng

Dán toàn bộ phần dưới vào Antigravity.

---

You are the economy designer of "One Bag Jianghu" (2D wuxia backpack auto-battler, PC). Design the upgrade materials, upgrade costs, the NPC shop and battle rewards.

## Hard rules
- Create ONLY `docs/reviews/2026-09-29-gemini-economy.md` (Vietnamese, ≤ 350 lines) and the draft data files `tools/gemini/out/economy/materials.json`, `tools/gemini/out/economy/upgrade_costs.json`, `tools/gemini/out/economy/shop.json`, `tools/gemini/out/economy/rewards.json`. Do not modify any other file. Do not run git.
- Game terms exactly as in `docs/design/glossary.md`. Every NEW name (materials, currencies…) goes in a "Thuật ngữ đề xuất" table (Vietnamese name, code id in snake_case ASCII, meaning) for the product owner to approve. Wuxia-flavoured names, **no names from Kim Dung novels or other games**. Example the owner gave: "Hàn Thiết" (cold iron) for weapons.
- Concrete numbers in tables. JSON must be valid, lowercase snake_case ids, no comments.

## Read first
`docs/decisions.md` (#4 items kept long-term, #12 Đột Phá, #14 balance goal, #21, **#22: NPC shop only — no player market; upgrading = Trọng 1→9, rarity fixed; upgrades never fail; materials split by item type**), `docs/design/gdd.md` (currencies: Bạc, Nguyên Bảo, Danh Vọng), `docs/design/items.md` (item types, rarities, 21 items), `docs/design/balance.md`, `server/battle-engine/src/main/resources/data/items.json`, and if present `docs/reviews/2026-09-29-gemini-hero-level.md` (EXP/level pacing — keep consistent).

## Deliver
1. **Materials by item type**: Binh Khí, Hộ Cụ, Công Pháp, Đan Dược + Kỳ Trân (you may merge the last two). For each type 3 tiers matching Trọng bands (1→3, 4→6, 7→9), plus one special **Đột Phá catalyst** per type used only for the steps into Trọng 3, 6, 9. Name, id, rarity look, icon description (1 line, for ChatGPT), where it drops.
2. **Upgrade cost table**: for each rarity (Phàm → Chí Tôn present in items.json) and each step Trọng n→n+1: material ids + counts + Bạc. Justify the curve (diminishing returns per GDD: "cường hóa có lợi ích giảm dần"). No failure (#22).
3. **Pacing**: expected battles/time to bring one epic weapon to Trọng 3 / 6 / 9 and a full bag of ~8 items to Trọng 3; state assumptions (battles per hour, win rate). Target: first Đột Phá (Trọng 3) on the main weapon within the first 1–2 hours.
4. **Rewards**: per PvE stage (by chapter, first clear vs repeat, win/loss) and per PvP match (win/loss/draw): Bạc, materials (drop tables with chances), Danh Vọng for PvP. EXP only if the hero-level doc exists (reuse its numbers).
5. **Thương Hội (NPC shop)**: number of slots, what can appear (items by rarity, materials, catalysts), weights, prices by rarity/tier, refresh (free timer + paid refresh cost in Bạc, rising per refresh per day?), purchase limits, **selling items/materials back to the shop** (price ratio) and what happens to the Trọng of a sold item. Anti-exploit notes (offline first: the client computes it; later the Java server is authoritative).
6. **Duplicates & storage**: what happens when the player gets an item they already own (e.g. converts to materials? kept as a second copy?), Tàng Bảo Các capacity.
7. **Data files** (the 4 JSON above) with a short schema description for each; ids must match the tables.
8. Max 4 open questions for the product owner.
