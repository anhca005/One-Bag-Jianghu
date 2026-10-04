# Codex task UF1: 9-slice images distorted, dropdown lists open empty, loading bar

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. **Follow `docs/tech/code-style.md`** (one statement per line, ≤120 chars, Allman braces, no inline if/for bodies, named helpers). This applies to builder code you touch too.

Two other Codex tasks run in parallel:
- UF2: shop and Home;
- UF3: Bao Phục, battle replay and PvE map.

## Your files (only these)
- `Assets/_Game/Editor/Building/UiBuild.cs`
- `Assets/_Game/Editor/Building/CommonUiPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/GameControllerPrefabBuilder.cs` (loading panel)
- new `Assets/_Game/Scripts/UI/Common/SlicedAutoFit.cs` (+ any small helper in `Scripts/UI/Common/` you create)
- new tests under `Assets/_Game/Tests/EditMode/UI/` and `Assets/_Game/Tests/PlayMode/`

Other tasks may call `UiBuild` helpers, but must not edit `UiBuild`. Keep existing public helper signatures working.

## Bugs (the user reported these with screenshots)

### 1. Every Sliced image looks distorted
The 9-slice UI sprites are huge source images with thick borders:

| Sprite | Size | Border (L, B, R, T) |
|---|---|---|
| `button_primary` / `button_secondary` | 2157×414 | 400, 120, 400, 120 |
| `plaque_label` | 2172×662 | 300, 160, 300, 160 |
| `progress_bar` | 2144×293 | 310, 100, 310, 100 |
| `frame_panel` | 1221×1160 | 240 all sides |
| `frame_tooltip` | 1677×597 | 130, 110, 130, 110 |

`UiBuild.ApplyUiSprite` sets one fixed `pixelsPerUnitMultiplier` per sprite (`SlicedPixelsPerUnit`, 6–12). Whenever an element is shorter or narrower than its scaled borders, Unity squashes the corners, which happens often. The same sprite is used at many sizes, so a fixed number cannot fit.

**Fix:** add a small runtime component `SlicedAutoFit : UIBehaviour` with `[ExecuteAlways]` and `[RequireComponent(typeof(Image))]`.
- On enable and on `OnRectTransformDimensionsChange`, set `pixelsPerUnitMultiplier` so that:
  - the scaled top + bottom borders take at most **~90% of the rect height**;
  - the scaled left + right borders take at most **~90% of the width**;
  - **corners stay square**: one uniform multiplier, the larger of the two needed values;
  - never go below the sprite's tuned default. Keep `SlicedPixelsPerUnit` as the "preferred" value and only increase it when needed.
- Use `sprite.border` and `sprite.pixelsPerUnit`, and the canvas reference pixels per unit (100 by default). Explain that formula in a short comment.

`UiBuild.ApplyUiSprite(..., Sliced)` must add or configure this component, so every builder that uses it gets the fix after a rebuild.

`Scripts/UI/Pve/StageMapPopup.cs:233` sets Sliced at runtime. Do **not** edit that file (UF3 owns it). Note in your result that UF3 should add `SlicedAutoFit` there.

### 2. Loading bar broken (`bg_loading` screen, `GameControllerPrefabBuilder`)
The bar shows as a flat grey strip with a thin red sliver at the left at 0%.
- Rebuild it as a proper bar:
  - frame: the `progress_bar` sprite with `SlicedAutoFit`;
  - fill: inside the frame's inner area. Use a `Filled` horizontal image, or a masked fill, so 0% shows nothing and 100% fills exactly to the inner edges;
  - the percentage text stays readable below it.
- Look at `progress_bar.png` to see where its inner channel is, and place the fill there.

### 3. Every dropdown opens to an empty box with only a scrollbar
This affects every dropdown: Thương Hội exchange, Bao Phục sect, Tàng Bảo Các filter and sort, and others. The common prefab `Prefabs/UI/Common/Dropdown.prefab` is built in `CommonUiPrefabBuilder.BuildDropdown`. Its template is broken, so the item rows are invisible. Likely causes, all to be checked:
- zero-height `Item`;
- `Content` without a height or with the wrong pivot or anchors;
- item label with no font, a zero rect, or the same colour as the background;
- `Toggle` without a graphic;
- the template `Viewport` mask hiding everything;
- the template `Canvas` sort order.

Rebuild the template the way Unity's default `DefaultControls.CreateDropdown` does:
- Template = ScrollRect + Viewport (Mask) + Content (top-anchored, pivot (0.5, 1), height of one item);
- Item = Toggle with a background, a checkmark and the label;
- item height about 40;
- label uses `UiFonts` Body, dark ink colour on a paper-coloured background;
- the caption shows the selected option.

Every popup must reuse this prefab. UF2 switches the shop popups to it, so the fix lands everywhere.

## Tests
- **EditMode:** load `Dropdown.prefab`; template has Viewport/Content/Item; item height > 0; item label has a font and a non-transparent colour different from the item background.
- **PlayMode:**
  - instantiate the dropdown on a test canvas with 5 options and call `Show()`;
  - after a frame, the spawned list has 5 visible item labels with the right texts, each with a rect height > 0 and inside the viewport.
- **EditMode:** `SlicedAutoFit` on a 300×40 rect with `button_primary`: the scaled vertical border is ≤ 90% of the height and corners stay uniform. With a large rect, it keeps the default multiplier.

## Check
- `py tools/lint/check_style.py <every file you changed>` → 0 problems.
- C# compiles.
- No git. Do not run Unity. Claude rebuilds prefabs, runs the tests and takes screenshots.

## Result
Files changed, root cause of the empty dropdown (what exactly was wrong), and the formula used for the multiplier.
