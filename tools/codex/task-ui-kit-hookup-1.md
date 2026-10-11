# Codex task: hook the UI kit (round 2) into the game, part 1: shared pieces, popups, bag, shop, storage

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Read `One-Bag-Jianghu/Assets/_Game/Editor/Building/README.md`.

## The art
`Assets/_Game/Textures2D/UI/Kit/*.png` (55 files, transparent, trimmed). What each one is for:
`docs/art/prompts-ui-kit.md` (column "Dùng ở"). Part 2 (a later run) will do the battle screen, result popup,
tutorial and encyclopedia icons (groups E, F, G); **you do groups A, B, C, D plus the shared plumbing for all 55.**

## 1. Shared plumbing (for all Kit files, including E/F/G)
- Import: make every `Textures2D/UI/Kit` file import as a Sprite (see `Editor/ItemIconImporter.cs`), with sensible
  **9-slice borders** for the pieces marked 9-slice in the prompts file (frames, ribbon, tabs, rows, buttons, inset,
  slot cells, bag board, shop card, tier pill, price tag, slider/scrollbar parts, dropdown, bar frame/fills,
  fighter panel, timeline, reward slot). Measure the ornament size on each image (Pillow) to choose borders; keep
  the table in one place with a comment per entry.
- `UiBuild.ApplyUiSprite(image, "Kit/<name>", type)` already resolves the subfolder; add pixels-per-unit multipliers
  to its `SlicedPixelsPerUnit` table so ornaments keep their proportion at UI size (look at the existing entries).
- **Fallback:** every place you switch to a Kit sprite must still look acceptable when the file is missing (keep
  today's sprite or color if `ApplyUiSprite` returns false).

## 2. Apply groups A–D
- **A (popups):** popup window frame (`popup_frame`) for the large popups built by `PopupPrefabBuilder` /
  `EconomyPopupPrefabBuilder` / Story / Encyclopedia / Sect; a title ribbon behind each popup title; a round close
  button (`button_close`) top-right on popups that have a "Đóng" button (keep the existing button working too, or
  replace it if the popup has no other action; Esc/back still closes); tabs (`tab_active`/`tab_idle`) wherever tabs
  exist (Encyclopedia categories, Storage/Shop filters if any); `panel_inset` for detail/description areas;
  `list_row`/`list_row_selected` for list entries (story chapters, encyclopedia entries); `divider_brush` between
  sections; `button_disabled` for non-interactable buttons (e.g. "Nâng Tâm Pháp (Sắp mở)"); `button_small_gold` for
  small buttons (sort, refresh, speed x1/x2/x4 if they are built in your builders).
- **B (controls):** Settings slider (`slider_track`/`slider_fill`/`slider_handle`), toggle (`toggle_off`/`toggle_on`),
  dropdown (`dropdown_field`), vertical scrollbars (`scrollbar_track`/`scrollbar_handle`) in the common prefabs
  (`CommonUiPrefabBuilder`) and wherever scroll views are built.
- **C (bag/shop/storage):** `slot_cell` for each bag grid and storage grid cell; `slot_cell_highlight` /
  `slot_cell_invalid` for the drag preview (find how the bag grid colors valid/invalid cells today and swap sprites
  instead of tints, keeping tints as fallback); `bag_board` behind the bag grid; `shop_card` for Thương Hội cards;
  `tier_pill` for the Trọng pill; `price_tag` for prices.
- **D (icons):** next to the matching buttons/labels: `icon_upgrade` (Nâng Trọng), `icon_merge` (Hợp Thành),
  `icon_sell` (Bán), `icon_refresh` (Làm mới), `icon_power` (Lực Chiến on Home and stage map), `icon_exp` (EXP on Home
  and rewards), `icon_sort` (Sắp xếp), `icon_info` (Bách Khoa entry point if there is a button). Small, left of the
  label, label still readable.

## Must not touch
`LoginScene` and its builder parts, the GameController loading frame, `Map_hoi_1` (hand-edited by the user). Do not
change gameplay logic. Do not touch `ReplayPrefabBuilder.cs`, the result popup, the tutorial overlay and the
encyclopedia **icons** (part 2), except adding their 9-slice/import entries.

## Check
- Text must still fit at 1920x1080 (use the existing `UiBuild.FitLabel` presets).
- `py tools/lint/check_style.py <every C# file you changed>` until clean.
- Build the C# projects (`dotnet build` on the generated `.csproj` like previous runs) to catch compile errors.
- **Do not launch Unity** (the user has it open). No git. Do not edit PNG files.

## Report (`-o` output)
Files changed; for each group, where each sprite is used; the 9-slice table; which builder menus the user must run
(Overwrite) to see the result; anything skipped and why.
