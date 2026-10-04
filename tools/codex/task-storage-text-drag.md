# Codex task ST: text overflow in Tàng Bảo Các (and item displays) + drag-outside-grid tests

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Read first:
- `Scripts/UI/Popups/StoragePopup.cs`, `StorageItemView.cs`, `StorageCell.cs`;
- `Scripts/UI/Common/ItemTooltip.cs`, `ItemDescriber.cs`, `SingleLineFit.cs`, `BagLoadoutPanel.cs`;
- `Scripts/Bag/**` (`ItemView.OnEndDrag`, `DragGridMath`);
- `Editor/Building/EconomyPopupPrefabBuilder.cs`, `BagPrefabBuilder.cs`, `UiBuild.cs` (`FitLabel` presets), `SceneSnapshot.cs`;
- `Tests/PlayMode/StorageSmokeTests.cs`, `BagTooltipSmokeTests.cs`, `DemoSmokeTests.cs` (profile backup/restore pattern).

Builders only, never edit prefabs by hand. **No git. Do not run Unity** (the license fails in your sandbox); Claude runs it.

## Context
The user reports: in **Tàng Bảo Các** (StoragePopup), text in the item display spills outside its box. Empty-layout snapshots look fine, so the overflow happens with **real item data**:
- long names;
- the detail panel with the full Items v2 description and stat line (task SU added more lines);
- price/action labels;
- the cell labels (Trọng, count) on small items.

Also in Bao Phục, the stats panel wraps "Giảm Hồi Chiêu: 0% (tối đa 40%)" onto 2 lines.

## 1. An automated overflow check (do this first: it finds the bugs)
Add a PlayMode test `Tests/PlayMode/TextOverflowTests.cs`, with the profile backup/restore as in `DemoSmokeTests`:
- Grant the profile **every item in `items.json`** (73) at a mix of Trọng/phẩm, including T9 Chí Tôn.
- Open StoragePopup. For **each item**:
  - select it so the detail panel fills;
  - then check every **active** `UnityEngine.UI.Text` under the popup:
    - when `horizontalOverflow == Overflow`, or the text has no `SingleLineFit`/`BestFit`, `preferredWidth` must be ≤ the rect width + 1;
    - `preferredHeight` must be ≤ the rect height + 1, unless it sits inside a `ScrollRect` viewport (then the content must grow, i.e. a `ContentSizeFitter`/layout, and the viewport must mask).
  - Collect every failure as "item id · GameObject path · preferred vs rect" and fail once with the full list.
- Do the same for:
  - the Bao Phục tooltip (`ItemTooltip`) for every item;
  - the Bao Phục stats panel at a high level/stat bag;
  - the Thương Hội cards/detail if they show the same describer text.
- Add a helper that saves a screenshot of the storage popup with the longest-description item selected to `Logs/Snapshots/StoragePopup_filled.png`, plus the same for the tooltip (`Tooltip_filled.png`), so Claude can look. Use `ScreenCapture.CaptureScreenshotAsTexture` at the end of a frame; skip quietly in `-nographics`.

## 2. Fix every overflow the check finds (in the builders/scripts)
Decide per element:
- **Long free text** (description, effects): put it in a `ScrollRect` with a mask and a `ContentSizeFitter` on the text; keep the font size readable (≥ 18 on the detail panel).
- **Single-line labels** (names, prices, button text, cell badges): use the `UiBuild.FitLabel` presets / `SingleLineFit`, with a sensible minimum size; truncate with an ellipsis only as a last resort.
- **Bao Phục stats panel:** widen it or shorten the wording so each stat stays on one line (for example "Giảm Hồi Chiêu: 0% (≤ 40%)"). Keep the same wording in `BagLoadoutPanel.cs` and `BagPrefabBuilder.cs`.
- Fonts come from the `UiFonts` roles. Do not change the visual style (ornate buttons, colours, rarity frames).

## 3. Drag outside the grid (tests missing from task DR)
Add PlayMode tests (new file `Tests/PlayMode/DragOutsideGridTests.cs`):
- **Bao Phục:**
  - drag a placed item and drop it outside the grid and outside the tray → the item is back at its old cell, the loadout is unchanged, and there is exactly one view for it (no destroyed or duplicated view);
  - repeat on the tray.
- **Tàng Bảo Các:** drag an item and drop it outside the grid → it is back where it was, and the storage count is unchanged.
- **Interrupted drag:** start a drag, then close the popup (`OnDisable`) → reopening shows the item in place, with no stuck ghost image.
- **Centre-on-cursor:** while dragging, the dragged image's centre is within a few pixels of the pointer.

Drive the drags with `PointerEventData` and the real `IBeginDragHandler`/`IDragHandler`/`IEndDragHandler`, as `StorageSmokeTests` does. If a test reveals a real bug, fix it in the drag code and say so.

## Rules
- Edit only:
  - `Assets/_Game/Scripts/**` (not `Scripts/Engine/**`);
  - `Assets/_Game/Editor/**`;
  - `Assets/_Game/Tests/**`.
- No git. Do not touch the data JSON, `Scripts/Engine/**` or `tools/gemini/**` (Gemini is working there).

## Result
- The overflow list your check would have reported before your fixes (reason it out from the layout code), and what you changed for each.
- The new tests.
- Any drag bug found.
- What Claude should look at in the screenshots.
