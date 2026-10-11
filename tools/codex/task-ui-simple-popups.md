# Codex task: decision #66, wave 2 (popups under Resources)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/`. **Read `docs/tech/code-style.md`**,
decision #66 in `docs/decisions.md`, and wave 1's `docs/art/ui-simple-plan.md`.

## Context
Wave 1 converted the prefabs under `Assets/_Game/Prefabs` and the scenes. The audit now also scans
`Assets/_Game/Resources`, where the popups live. It found 305 distorted uses. Read the new audit:
- `docs/art/ui-image-audit.md`;
- `tools/codex/out/ui-image-audit.csv`;
- the screenshots in `One-Bag-Jianghu/Logs/Snapshots/*Popup.png`. `ShopPopup.png` and `SettingsPopup.png` show the
  problems well.

Summary by sprite (canvas units at 1920x1080):

| Count | Sprite (art px) | Rect sizes | Where |
|---|---|---|---|
| 202 | `Kit/slot_cell` (1138x1070) | 76x76 | `StoragePopup`, `StorageCell` |
| 28 | `button_secondary` (2048x401) | 260x62, 280x64, 220x54, 220x58 | `DevCheatPanel`, `SettingsPopup`, `StageMapPopup` |
| 11 | `button_primary` (2048x393) | 260x62, 280x64, 185x45, 230x48 | `Shop`, `Storage`, `Confirm`… |
| 12 | `Kit/panel_paper_strip` (720x128) | popup-sized, e.g. 1760x980 | `Panel_Paper` instances used as popup bodies |
| 11 | `Kit/popup_frame` (1024x999) | 1760x980, 1600x920, 1700x960, 760x440 | most popups (`Settings` 960x900 passes) |
| 10 | `Kit/popup_title_ribbon` (1536x363) | 510x89 … 991x78 | every popup title |
| 9 | `Kit/dropdown_field_medium` (720x128) | 490x81, 403x54, 541x81, 186x81 | `DevCheat`, `Settings`, `Shop` |
| 4 | `frame_panel` (1221x1160) | 414x294, 100x100 | `ShopCard`, `StorageItemView` |
| 4 | `Kit/panel_inset` (1024x996) | 307x615, 330x615, 624x210, 557x402 | `Encyclopedia`, `Result`, `Storage` |
| 3 | `Kit/divider_brush` (1427x37) | 480x3 | `Settings` |
| 3 | `Kit/slider_track` (1536x97) | 403x22 | `Settings` |
| 2 | `frame_tooltip` (1677x597) | 440x420 | `Bag`, `Shop` |
| 2 | `Kit/price_tag` (2048x432) | 256x36 | `ShopCard` |
| 2 | `Kit/shop_card_frame` (885x1326) | 420x300 | `ShopCard` |
| 2 | `Kit/tier_pill` (2048x349) | 34x22 | `StorageItemView` |
| 1 each | scrollbar handle/track, `button_small_gold` 260x62, `list_row` 0x48 | | `Storage`, `Encyclopedia` |

## Rules (same as wave 1, with its lessons)
Pick the first option that works, per use:
1. **Use existing wave 1 art.** For example `slot_cell_square` for the storage cells, `tier_pill_compact` (then
   resize to 42x24 or a size at its aspect), and flat scrollbars like `SettingsPopup`.
2. **Resize the rect to the art's aspect** when the element is fixed-size and a change of about 12% fits.
   Examples: `divider_brush` 480x3 → 480x12; `price_tag`; `slider_track` 403x22 → 403x25.
3. **Fixed size.** Popup title ribbons must be one or two fixed sizes at the ribbon art's aspect; the title text
   shrinks to fit (best fit). Buttons: pick a few fixed sizes per aspect group.
4. **New art at the exact aspect**, built like wave 1:
   - caps + middle / 9-piece composition from drawn sources;
   - uniform scale only;
   - 2x resolution;
   - no text.
   Uses within 5% aspect share one image. Expected:
   - `popup_frame_wide` (about 1.77:1, for the 1760x980, 1700x960, 1600x920 and 760x440 popups; the frame
     ornaments should be about as thick as `SettingsPopup`'s frame looks now, not the chunky look in
     `ShopPopup.png`);
   - button art at about 4.2:1 (`button_primary_compact`, `button_secondary_compact`, `button_small_gold_compact`);
   - `panel_inset` variants;
   - `tooltip_tall` (about 1.05:1);
   - dropdown variants if resizing cannot fit.
5. **`Panel_Paper` used as a popup body:** the popup already has `popup_frame`. Remove the duplicate frame there:
   the `Frame` child gets no sprite and is hidden. Keep the paper colour fill only if the popup needs it.
6. **Fallback sprites:** none. `ShopCard` must use only `shop_card_wide`. Remove the `shop_card_frame` and
   `frame_panel` layers if they are still drawn.

## Art acceptance (required; reuse `tools/codex/out/check_ui_simple_art.py`)
- The content bounding box fills ≥ 97% on both axes. **No padding** to fake an aspect.
- **No cover-cropping into a frame.** The rim check must pass on all 4 edges, and the corner check must match the
  source corners.
- Aspect within 1% of the target; ≥ 2x resolution, or the 2048 cap.
- No text.
- Make a review sheet: `tools/codex/out/ui-simple-popups-review.png`.

## Code
Change only builders in `Assets/_Game/Editor/Building/*.cs`. In this sandbox `Assets/_Game/Scripts` is not
writable; list any runtime change you need in the report instead. Text in fixed frames uses best fit.

## Check
- `py tools/lint/check_style.py` on the changed C#: clean.
- `dotnet build` the Game, Editor and Tests projects with temporary csproj copies; delete the copies and their
  `obj/` caches afterwards.
- Do not launch Unity. No git commands that change the repo. No `.prefab`, `.unity` or `.meta` edits by hand.

## Report (`-o` output)
- a decision table (element → option → art or size);
- the new art files with their target rects;
- the checker output;
- the builder menus the leader must run, in order. They must never include LoginScene, HomeScene, the
  GameController loading frame or `Map_hoi_1`;
- the runtime changes you could not make.
