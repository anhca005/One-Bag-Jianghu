# Codex task UF3: item borders (Bao Phục + battle), battle item names and log, PvE stage info panel

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. **Follow `docs/tech/code-style.md`** (one statement per line, ≤120 chars, Allman braces, no inline if/for bodies, named helpers). This also applies to builder code you touch.

Parallel tasks:
- UF1 owns `UiBuild.cs`, `CommonUiPrefabBuilder.cs`, `GameControllerPrefabBuilder.cs`. It adds `Scripts/UI/Common/SlicedAutoFit.cs`, a component that fits 9-slice borders to the rect, and makes `UiBuild.ApplyUiSprite(..., Sliced)` add it.
- UF2 owns the economy builder, shop and Home.

**Do not edit their files.** You may *call* `UiBuild` helpers.

## Your files (only these)
- `Assets/_Game/Editor/Building/PopupPrefabBuilder.cs` (Bao Phục popup parts only)
- `Assets/_Game/Editor/Building/BagPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/ReplayPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/PvePrefabBuilder.cs`
- `Assets/_Game/Scripts/Bag/View/**`
- `Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs`
- `Assets/_Game/Scripts/UI/Popups/BagPopup.cs`
- `Assets/_Game/Scripts/Battle/Replay/View/**`
- `Assets/_Game/Scripts/UI/Pve/**`
- tests for these screens under `Assets/_Game/Tests/`

## Bugs (user screenshots)

### 1. Items in Bao Phục have no border
Items in the bag grid (left) and in the storage side (right) of the Bao Phục popup sit on a plain grey cell with no frame. Tàng Bảo Các already draws a rarity border around each item: see `StorageItemView` and the "Rarity Border" built in `EconomyPopupPrefabBuilder` (read only).
- Give Bao Phục item views the same look: a frame around the item's full footprint (all its cells), coloured by rarity via `BagViewStyle` / the existing rarity colours, plus the T badge as now.
- Reuse one style source so the two screens stay identical. If the colours are duplicated, move them into `BagViewStyle` and leave `EconomyPopupPrefabBuilder` alone; UF2 owns it, so mention it in your result.

Also in that popup:
- "Tâm Pháp cấp 1" floats alone at the top-left, far from the bag.
- The stats block ("Cấp 2 / Khí Huyết tối đa / Hộ Thân / Giảm Hồi Chiêu") sits in the middle gap.
- The right-hand storage grid is cut off at the bottom: its last row is half visible.

Lay the popup out cleanly:
- **left column:** sect dropdown + Tâm Pháp level, then the bag grid;
- **middle column:** stats block, top-aligned;
- **right column:** the storage grid, fully visible or in a ScrollRect that shows whole rows.

### 2. Items in battle have no border, and item names are drawn huge over the icon
In the battle replay, items sit on translucent grey cells without a frame. Two examples:
- "Thanh Phong Kiếm" (1×2, icon visible) also shows its name in very large letters across the whole item;
- the opponent's "Thanh Đằng Tiên" does the same.

`ReplayItemView` switches the name style on `hasIcon`. Find out why it renders large text when an icon is shown; for example, `hasIcon` may be computed before the sprite loads, or tall items may take the wrong branch.
- When an icon exists, show either no name or a small single-line name at the bottom inside the frame. Make it `SingleLineFit`, ≤ 14 px. Prefer no name, since the tooltip and log carry it.
- Add the same rarity frame as in Bao Phục, drawn above the cooldown overlay so the dark cooldown fill does not hide the frame.

### 3. Battle log overflows its frame
The bottom log box (the collapsed one with the last 3 lines, `LogToggle` / `LastThree…` in `ReplayPrefabBuilder`) draws its text past the left edge of the dark frame. Lines start outside the frame.
- Pad the text inside the frame's inner area.
- Clip with `RectMask2D`.
- Truncate long lines with an ellipsis or `SingleLineFit`.
- Check the expanded log (`ExpandedBattleLog`) the same way.
- Also seen: a raw event label `QI` in the log ("6.0s QI"). Use the Vietnamese term (Nội Lực) via the existing event text formatter, if that formatter lives in `Scripts/Battle/Replay/` (yours). Otherwise just report it.

### 4. PvE stage info panel misaligned with its art
On the Hồi map, the right-hand stage info panel uses a hanging signboard image (wooden board with red ribbons, a paper area in the middle, a wooden plank at the bottom). The texts are placed without regard to the art:
- the stage title "Trấn Khẩu Sơn Đạo" overlaps the top ribbons;
- the enemy portrait sits over the paper top;
- the enemy name and description run across the wooden posts;
- a small currency icon floats at the left outside the board;
- a reward line is hidden behind the plank text;
- "Khiêu chiến" sits on the plank but cuts the plank art.

Fix it the same way the dialogue box was fixed in `PvePrefabBuilder.BuildDialogue`:
- open the board PNG and measure its paper area and plank area as fractions of the image;
- keep the board at its native aspect (`preserveAspect`, fixed aspect box);
- anchor each element inside those fractions;
- leave a comment with the measured fractions.

Suggested layout:
- **title:** on the paper top, or above the board;
- **portrait:** in the paper's upper half;
- **enemy name:** under the portrait;
- **description:** wrapped, best-fit with a max size;
- **recommended level and rewards:** each with an icon next to its value;
- **button:** centred on the plank.

`StageMapPopup.cs:233` sets `Image.Type.Sliced` at runtime. Add `SlicedAutoFit` to that image once UF1's component exists; if it is not there yet, add a `// TODO(UF1)` and say so.

## Tests
- **EditMode:**
  - `ReplayItemView.prefab` and the Bao Phục item view prefab both contain a rarity frame image;
  - the name text is inactive or ≤ 14 px when an icon is set (call the bind method with a sprite).
- **PlayMode (screenshot-free assertions):**
  - in the replay, log row texts lie inside the log frame's rect (corners via `GetWorldCorners`);
  - on the PvE map, the info panel's title, name and description rects lie inside the board image rect and do not overlap each other;
  - the Bao Phục storage grid's last row is fully visible inside its viewport, or the grid is in a ScrollRect.

## Check
- `py tools/lint/check_style.py <every file you changed>` → 0 problems.
- C# compiles.
- No git. Do not run Unity; Claude rebuilds, tests and takes screenshots.

## Result
List the files changed, why the big name text appeared, and the measured board fractions.
