# Lệnh cho Gemini: rà toàn bộ chữ hiển thị trong game (chỉ báo cáo)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity (chọn model nào cũng được), dán toàn bộ phần dưới dấu `---`.

---

You are the content editor of "One Bag Jianghu" (Nhất Nang Giang Hồ), a Vietnamese wuxia bag-packing auto-battler. The player sees only Vietnamese. The user found, while testing:
- raw ids on screen (`han_thiet` in a dropdown, `QI` in the battle log);
- names shown twice;
- text overflowing frames.

Code fixes for those specific cases are in progress elsewhere. Your job is a **full audit** of everything else the player can read.

## Read first
- `docs/design/glossary.md`: the official terms. They are binding, and no names from the reference games or Kim Dung novels may appear.
- `docs/design/gdd.md`, `docs/design/pve.md`, `docs/design/items.md` (skim).

## Audit scope (read only)
1. **Data:** every string field shown to players in:
   - `One-Bag-Jianghu/Assets/_Game/Resources/Data/*.json`: items, materials, sects, shop, rewards, levels, starter, merge, upgrade_costs;
   - `Resources/Data/Pve/chapters.json`: stage names, enemy names, descriptions, dialogue lines.
2. **Code strings:** Vietnamese or English string literals that reach the screen in `One-Bag-Jianghu/Assets/_Game/Scripts/**/*.cs`:
   - `Text.text = ...`, button labels, popup messages, `EconomyResult.Fail("...")`, formatters such as `ItemDescriber`, `EventTextFormatter`, `CurrencyArt`;
   - skip `Debug.Log` strings.
3. **Builder labels:** `One-Bag-Jianghu/Assets/_Game/Editor/Building/*.cs`, strings passed to `UiBuild.Text(...)` and button captions.

## Check for
- **Raw identifiers or English** that can reach the screen: snake_case ids, enum names (`QI`, `BASIC`, `WEAPON`), English words. Give the code path that shows them.
- **Terminology:**
  - terms that disagree with the glossary;
  - one concept with two names (e.g. "Sinh Lực" vs "Khí Huyết", "Cương Khí" vs "Hộ Thân", "Nộ" vs "Nội Lực");
  - mixed capitalisation of the same term.
- **Vietnamese quality:** typos, missing diacritics, wrong tone marks, unnatural phrasing, inconsistent punctuation (`:` spacing, `·`, `…`), number formats (1,200 vs 1.200: pick the one the game uses most and flag the rest).
- **Length risk:** strings likely to overflow their UI. Flag:
  - names over 18 characters;
  - button captions over 14 characters;
  - single-line descriptions over 90 characters.

  Give the file and line where each string is used.
- **Names:** any name that matches a Kim Dung character, sect or martial art, or a reference game's term. Use your general knowledge and list each suspect with the reason.
- **Story consistency in `chapters.json`:**
  - enemy names and descriptions match their chapter's theme (Hồi 1 Thanh Khê autumn town, Hồi 2 poison swamp / Bách Thảo Cốc, Hồi 3 snow mountain);
  - dialogue speakers exist;
  - no contradictions between stages.

## Rules
- **Create only:**
  - `docs/reviews/2026-10-08-gemini-text-audit.md` (Vietnamese, ≤ 250 lines);
  - `tools/gemini/out/text-audit/` for proposed replacement files.
- **Do not edit game data, code, docs or git.**
- **Proposed fixes:**
  - **data fixes:** one JSON file per data file in `tools/gemini/out/text-audit/<file>.patch.json`, as a list of `{ "path": "<json path, e.g. items[12].description>", "old": "...", "new": "..." }` entries. Claude applies them after review;
  - **code-string fixes:** a table in the report (file:line, old → new). Claude hands it to Codex.

## Report format
1. **Summary table:** count per category (raw id, terminology, typo, length, naming, story).
2. **Must-fix list** (things the player will notice), most visible first.
3. **Proposed glossary additions** for terms used in the game but missing from `glossary.md`.
4. **At most 3 open questions** that need the user's decision, e.g. which of two competing terms to keep.
