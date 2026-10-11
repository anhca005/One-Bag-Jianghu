Implemented UI Kit round 2, part 1. Unity was not launched; PNG files, gameplay rules, LoginScene, loading frame, `Map_hoi_1`, replay/result/tutorial builders, and encyclopedia icons were not modified.

## Files changed

- `Assets/_Game/Editor/ItemIconImporter.cs`
- `Assets/_Game/Editor/Building/UiBuild.cs`
- `Assets/_Game/Editor/Building/CommonUiPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/BagPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/PopupPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs` — Home only
- `Assets/_Game/Editor/Building/PvePrefabBuilder.cs`
- `Assets/_Game/Scripts/Bag/View/BagGridView.cs`
- `Assets/_Game/Scripts/Onboarding/EncyclopediaPopup.cs`

## Sprite usage

Group A:

- `popup_frame`: regular, economy, sect, story, encyclopedia, bag, settings, and PvE popup panels.
- `popup_title_ribbon`: generated behind popup titles.
- `button_close`: replaces visual of existing Đóng buttons while retaining their click handlers and Esc/back behavior.
- `tab_active`, `tab_idle`: shop tabs.
- `panel_inset`: encyclopedia columns and storage details.
- `list_row`, `list_row_selected`: encyclopedia/category/story rows, including runtime selection swapping.
- `divider_brush`: settings section dividers.
- `button_disabled`: disabled sprite state on common buttons.
- `button_small_gold`: storage sort and shop refresh.

Group B:

- Settings/common slider: `slider_track`, `slider_fill`, `slider_handle`.
- Common toggle: `toggle_off`, `toggle_on`.
- Common dropdown: `dropdown_field`.
- Common and storage scrollbars: `scrollbar_track`, `scrollbar_handle`.

Group C:

- `slot_cell`: bag and storage cells.
- `slot_cell_highlight`, `slot_cell_invalid`: valid/invalid drag previews; old color tints remain as fallback.
- `bag_board`: behind the bag grid.
- `shop_card`: merchant item card.
- `tier_pill`: bag/storage tier labels.
- `price_tag`: shop price area.

Group D:

- `icon_upgrade`, `icon_merge`, `icon_sell`: storage actions.
- `icon_refresh`: shop refresh.
- `icon_sort`: storage sorting.
- `icon_power`: Home and stage detail.
- `icon_exp`: Home EXP.
- `icon_info`: Home Bách Khoa entry.
- Icons are positioned left of their labels; missing art removes only the icon.

## 9-slice table

All Kit files import as single transparent sprites. Explicit borders, in source pixels `(left, bottom, right, top)`:

```text
popup_frame          120,120,120,120
popup_title_ribbon   310,90,310,90
tab_active           150,80,150,80
tab_idle             150,80,150,80
panel_inset           90,90,90,90
list_row             190,35,190,35
list_row_selected    190,35,190,35
button_disabled      330,40,330,40
button_small_gold    250,45,250,45
slider_track         180,20,180,20
scrollbar_track       15,180,15,180
scrollbar_handle      18,190,18,190
dropdown_field       145,50,300,50
slot_cell             95,95,95,95
slot_cell_highlight   95,95,95,95
slot_cell_invalid     95,95,95,95
bag_board            180,150,180,150
shop_card            100,170,100,170
tier_pill            250,100,250,100
price_tag            310,100,150,100
bar_frame            260,60,260,60
bar_fill_hp          180,50,180,50
bar_fill_shield      180,50,180,50
bar_fill_qi          180,50,180,50
fighter_panel        110,170,110,170
timeline_bar         260,50,260,50
reward_slot          110,110,110,110
```

The corresponding `SlicedPixelsPerUnit` values are centralized in `UiBuild`. Borders were checked against Pillow-reported trimmed dimensions.

## Unity menus to run

Run these in order:

1. `One Bag Jianghu/Reimport Art And Rebuild Icon Library`
2. `One Bag Jianghu/Build/Common UI Prefabs (Overwrite)`
3. `One Bag Jianghu/Build/Bag Prefabs (Overwrite)`
4. `One Bag Jianghu/Build/Popup Prefabs (Overwrite)`
5. `One Bag Jianghu/Build/Economy Popup Prefabs (Overwrite)`
6. `One Bag Jianghu/Build/Home Hotspot Prefab (Overwrite)`
7. `One Bag Jianghu/Build/Home Scene (Overwrite)`

The PvE popup is included through `Build All Missing (Overwrite)`. Do not run the destructive “Reset Map Prefabs” command.

## Verification

- All generated project builds succeeded, including Editor, Game, Economy, Engine, EditMode tests, and PlayMode tests.
- Final Editor build: zero errors and zero warnings.
- Style checks pass for the newly clean shared/economy/bag/runtime files.
- Three touched legacy builders (`PopupPrefabBuilder`, `LoginHomeSceneBuilder`, `PvePrefabBuilder`) retain pre-existing compact-line style violations outside the edited regions.
- E/F/G received shared importer and slicing plumbing only, as requested.
- No storage filter tabs exist; its existing control is a dropdown.
- Story is sequential rather than a chapter-list UI, so there was no chapter-row builder to skin.