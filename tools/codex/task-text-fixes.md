# Codex task TX: on-screen text fixes from the text audit

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. **Follow `docs/tech/code-style.md`** (one statement per line, ≤120 chars, Allman braces; wrapped lines indented, never at column 0).

Source: `docs/reviews/2026-10-08-gemini-text-audit.md`, plus `tools/gemini/out/text-audit/code_findings.json` (Gemini's raw findings, which may contain false positives). Terms must follow `docs/design/glossary.md`.

## User decisions (binding)
1. **Number format:** Vietnamese thousands separator with a dot (`1.200`, `10.000`), everywhere numbers are shown to the player.
2. **Long names are kept.** Do not rename items, stages or buttons. Instead, make name and button texts shrink to fit on one line (see 4 below).
3. **Boss label:** "Thủ Lĩnh" instead of "BOSS".

## Do
1. **Number format helper.**
   - Add `Scripts/UI/Common/UiFormat.cs` with `public static string Number(long value)`, which formats with `.` as the thousands separator, using `CultureInfo("vi-VN")` or a fixed `NumberFormatInfo` (no culture lookup that can fail on some machines).
   - Replace every player-visible `ToString("N0")`, `:N0` and similar with it.
   - Do not touch `Debug.Log` strings.
2. **Raw English / ids on screen.** Every error string that can reach a popup must be Vietnamese and must not contain ids. Show the material **name** from the catalog when it is available. Known spots:
   - `StorageGrid.cs:~226` and `ShopService.cs:~169`: `Insufficient material '<id>'` → `Không đủ <Tên nguyên liệu>.`;
   - `PlayerProfile.cs:~436`: `Insufficient {label} '{key}': need …` → Vietnamese with the name and the amount;
   - scan `Scripts/Economy`, `Scripts/Pve` and `Scripts/Core` for other `EconomyResult.Fail("...")` / exception messages shown to the player that are English or contain ids, and fix them;
   - `DevCheatPanel.cs:~50,55`: dropdown options show the **name**, keeping the id mapping internally.
3. **Battle log and HUD terms** (`EventTextFormatter.cs`, `FighterPanelView.cs`, `ReplayPrefabBuilder.cs`):
   - "Sinh Lực" → "Khí Huyết" (HP label and log);
   - `REGEN` → "Điều Tức" (check the glossary; use the glossary's word);
   - every event type in the formatter's `default` branch gets a Vietnamese line: START, SHIELD, QI (Nội Lực), DOT, BURST, CHARGE, BUFF, MAX_HP, DMG_REDUCE, PARRY_COST and any others in `BattleEventType`. No enum name may ever reach the screen. The fallback prints a neutral Vietnamese phrase, not the raw type;
   - consistent capitalisation of glossary terms: Khí Huyết, Nội Lực, Cương Khí, Tuyệt Kỹ, Hiệp Khách (also in `ItemDescriber.cs`).
4. **Shrink-to-fit names and captions.** Item name texts (bag, storage, shop cards, tooltip title, replay item tooltips), stage and enemy names on the PvE map and info board, and button captions must stay on one line and shrink to fit. Use the existing `SingleLineFit` component.
   Apply it everywhere in your files, including the shop and storage cards.
5. **PvE node label:** `PveStageNode.cs:~56`, `"BOSS"` → `"Thủ Lĩnh"`.
6. **Glossary:** add "Thủ Lĩnh" (`boss`): "Đối thủ ải cuối mỗi Hồi" to `docs/design/glossary.md`, in the right table.

## Your files
Everything under:
- `Assets/_Game/Scripts/**`;
- `Assets/_Game/Editor/Building/**`, including the economy builder and the shop/storage popups, which were just restored by task UF2b. **Do not remove** anything UF2b restored: the detail ScrollRect, scrollbars, cost best fit, card frames, seal, reputation grid;
- `docs/design/glossary.md`;
- tests under `Assets/_Game/Tests/**` whose expected strings change (e.g. `ItemDescriberTests`, log formatter tests). Update the expectations; do not delete tests.

## Tests
- **EditMode `UiFormatTests`:** 0 → "0", 999 → "999", 1200 → "1.200", 1234567 → "1.234.567", negatives.
- **EditMode:** for every `BattleEventType` value, the formatter output contains no ASCII-uppercase word of 3+ letters (no raw enum names).
- **EditMode:** the economy failure messages for a missing material contain the material's Vietnamese name and no `_`.

## Check
- `py tools/lint/check_style.py <every file you changed>` → 0 problems for runtime scripts and tests.
  - Builders: no new problems in code you write.
  - `py tools/lint/fix_continuation_indent.py <files> --check` → 0.
- C# compiles.
- No git. Do not run Unity.

## Result
List the files changed and every replaced string (old → new).

## Also fix (UI regressions found by Claude's test run + screenshots)
Screenshots: `One-Bag-Jianghu/Logs/Snapshots/*.png` (made by `SceneSnapshot.CaptureAll`).
1. **Loading bar shows a full red fill at 0%** (`LoadingPanel.png`, `GameControllerPrefabBuilder`).
   - 0% must show no fill, and 100% fills exactly the inner channel.
   - Use an `Image.Type.Filled` horizontal fill driven by the loading script, or size the fill rect from progress. Check which field the loading script sets and wire it correctly.
2. **Shop header currency icons render as white squares** (`ShopPopup.png`). The icon Images have no sprite.
   - Load `icon_bac` / `icon_danh_vong`, or whatever keys `CurrencyArt` / the Home HUD uses, the same way the Home HUD and Tàng Bảo Các do; those show correctly.
   - If the sprite is missing, hide the Image instead of drawing a white box.
3. **PlayMode `TextOverflowTests.RealItemTextFitsStorageBagAndShop` fails on the shop card:**
   - `Shop Card Prefab/Buy/Label` text height 20.8 > rect 12.0: the buy button label rect is too small for its font. Give the label the button's inner area (`UiBuild` button helpers already do this for other buttons), with `SingleLineFit`;
   - `Shop Card Prefab/Disabled Reason` height 18.2 > 16.5: give it enough height, or `SingleLineFit` with a smaller minimum.
4. **PlayMode `DropdownPlayTests.ShowCreatesFiveVisibleRowsInsideViewport` fails at the "inside viewport" assertion** (line ~39).
   - Find out whether the rows really fall outside the viewport, for example because the template height (208) does not fit 5 rows × 40 plus padding, or Content is offset. Fix the template if so.
   - If the test geometry is wrong instead, fix the test. Explain which it was.
