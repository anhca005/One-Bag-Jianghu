# Kế hoạch ảnh UI Simple — pha 2 quyết định #66

Đơn vị kích thước trong bảng là canvas 1920×1080. Một dòng là một nhóm duy nhất theo
`sprite + rect`; các scene chỉ là bản sao của prefab được ghi cùng dòng. Mọi ảnh mới dùng
`Image.Type.Simple`. Text trong nút/nhãn dùng best fit và vùng trống đã chừa khoảng 30% cho
localization. Ba dòng mẫu 100×100 (`Button_Primary`, `Button_Secondary`, `FighterPanel`) không
được đưa vào bảng vì là prefab chưa định cỡ và không có use thực ở 100×100.

| Element (prefab/scene :: object) | Sprite hiện tại | Rect hiện tại | Quyết định | Rect mới nếu resize | Art file |
|---|---|---:|---|---:|---|
| LoginScene :: `Canvas/Content/Btn_Enter` | `button_primary` | 360×76 | resize, chốt cỡ nút có 30% dư chữ | 396×76 | `button_primary.png` |
| ReplayScreen + GameplayScene + ReplayDemo :: `BottomBar/SpeedButtons/PauseResume` | `button_secondary` | 170×64 | new art | — | `button_replay_control.png` |
| ItemView + ReplayItemView :: `Rarity Frame` | `frame_panel` | 94×94 | crop/pad vuông, không cắt góc | — | `frame_panel_square.png` |
| Panel_Paper :: `Frame` | `frame_panel` | 360×64 | new art | — | `panel_paper_strip.png` |
| ReplayScreen + GameplayScene + ReplayDemo :: `TopBar` | `frame_panel` | 1920×164 | new art | — | `replay_top_bar.png` |
| ReplayScreen + GameplayScene + ReplayDemo :: `BottomBar` | `frame_panel` | 1920×120 | new art | — | `replay_bottom_bar.png` |
| InputField + Tooltip :: root | `frame_tooltip` | 360×64 | new art, cỡ field nhỏ cố định | — | `tooltip_field.png` |
| HomeScene :: `Canvas/Stamina Tooltip/Paper` | `frame_tooltip` | 440×260 | new art, tooltip cố định | — | `tooltip_panel.png` |
| LoginScene :: `Canvas/Content/HeroNameInput` | `frame_tooltip` | 620×64 | new art, field rộng cố định | — | `input_field_wide.png` |
| FighterPanel/Replay/VfxLab :: `Portrait` | `hero_portrait_frame` | 222×276 | new art | — | `hero_portrait_frame_wide.png` |
| HomeScene :: `Canvas/Header` | `home_banner` | 1920×132 | new art | — | `home_banner_simple.png` |
| BagLoadoutPanel + BagDemo :: `Bag Board` | `Kit/bag_board` | 540×450 | new art | — | `bag_board_landscape.png` |
| Replay prefab/scenes :: mọi `StatusChips/*/CountBadge` | `Kit/count_badge` | 20×14 | new art; size-driven cũ bỏ, chốt một cỡ | — | `count_badge_compact.png` |
| BagLoadoutPanel + BagDemo :: `Sect` | `Kit/dropdown_field` | 500×64 | resize theo tỷ lệ art hiện có | 500×72 | `dropdown_field.png` |
| Dropdown prefab :: root | `Kit/dropdown_field` | 360×64 | new art, cỡ medium cố định | — | `dropdown_field_medium.png` |
| Replay prefab/scenes :: `MiddleBand/*/Avatar` | `Kit/fighter_panel` | 250×360 | new art | — | `fighter_panel_replay.png` |
| VfxLab :: `Stage/FighterPanel` | `Kit/fighter_panel` | 290×429.76 | resize, không va lân cận | 257×429.76 | `fighter_panel.png` |
| Replay prefab/scenes :: `TopBar/*Hud` | `Kit/hud_panel` | 780×148 | new art | — | `hud_panel_simple.png` |
| Replay prefab/scenes :: `BottomBar/LogToggle` | `Kit/log_panel` | 1300×96 | new art | — | `log_panel_wide.png` |
| Replay prefab/scenes :: `ExpandedBattleLog` | `Kit/log_panel` | 1320×440 | new art | — | `log_panel_expanded.png` |
| SettingsPopup :: `Scroll View/Scrollbar/Sliding Area/Handle` | `Kit/scrollbar_handle` | 10.8×64 | flat, màu mực; handle dài theo content | — | không sprite / white 1×1 tint |
| SettingsPopup :: `Scroll View/Scrollbar` | `Kit/scrollbar_track` | 10.8×64 | flat, màu mực nhạt | — | không sprite / white 1×1 tint |
| FighterPanel/Replay/GamePlay/ReplayDemo :: mọi `Bag/Cell *` | `Kit/slot_cell` | 109×109 | crop/pad vuông | — | `slot_cell_square.png` |
| BagLoadoutPanel/BagDemo :: mọi `Grid/Cell *` | `Kit/slot_cell` | 96×96 | crop/pad vuông, dùng chung ảnh vuông | — | `slot_cell_square.png` |
| BagHighlight :: root | `Kit/slot_cell_highlight` | 100×100 | crop/pad vuông | — | `slot_cell_highlight_square.png` |
| BagHighlightInvalid :: root | `Kit/slot_cell_invalid` | 100×100 | crop/pad vuông | — | `slot_cell_invalid_square.png` |
| ItemView :: `Tier Pill` | `Kit/tier_pill` | 42×24 | new art | — | `tier_pill_compact.png` |
| Replay prefab/scenes :: `BottomBar/Timeline` | `Kit/timeline_bar` | 1300×20 | new art | — | `timeline_bar_thin.png` |
| Hotspot prefab :: `Plaque` | `plaque_label` | 100×46 | new art | — | `plaque_label_compact.png` |
| HomeScene :: `Hotspot_Pve/Plaque` | `plaque_label` | 260×92 | new art; dùng chung nhóm tỷ lệ gần | — | `plaque_label_medium.png` |
| HomeScene :: `Hotspot_Pvp`, `Hotspot_Sect/Plaque` | `plaque_label` | 240×82.8 | resize về tỷ lệ ảnh medium | 240×84.9 | `plaque_label_medium.png` |
| HomeScene :: `Hotspot_Bag/Plaque` | `plaque_label` | 240×64.4 | resize theo art hiện có | 211×64.4 | `plaque_label.png` |
| HomeScene :: `Hotspot_Storage/Plaque` | `plaque_label` | 240×101.2 | new art | — | `plaque_label_tall.png` |
| HomeScene :: `Hotspot_Codex/Plaque` | `plaque_label` | 220×73.6 | resize theo art hiện có | 220×67 | `plaque_label.png` |
| HomeScene :: `Hotspot_Encyclopedia/Plaque` | `plaque_label` | 220×59.8 | resize theo art hiện có | 196×59.8 | `plaque_label.png` |

Các thanh HP, khiên, khí và slider không nằm trong class C nhưng vẫn phải theo quyết định #66:
giữ `Image.Type.Filled`, horizontal; `bar_fill_*_core` và `slider_fill` được vẽ ở tỷ lệ đầy đủ,
Unity chỉ crop theo fill amount, tuyệt đối không stretch. Không phát sinh ảnh mới ở pha này.

Fallback: bỏ toàn bộ nhánh fallback khi triển khai. `shop_card_frame`, `result_victory`,
`fighter_panel`, `gauge_frame_large`, `gauge_frame_small`, `bar_frame`, `bar_fill_hp_core`,
`bar_fill_shield_core`, `bar_fill_qi_core`, `status_slot` và `clock_plate` đều đã tồn tại.
Hai use `log_panel` được thay thẳng bằng `log_panel_wide` và `log_panel_expanded`; HUD dùng
`hud_panel_simple`; không còn mượn `frame_panel`, `fighter_panel`, `home_banner`,
`progress_bar`, `slot_cell` hay `plaque_label` làm dự phòng.

Kết quả quyết định theo 35 nhóm có use thực: **resize 7, crop/pad 5, new art 21,
Filled 0, flat 2**. Ba template 100×100 được bỏ qua đúng yêu cầu.

Round 2: 14 ảnh bị trắng viền đã được cắt sát alpha. Thanh dài dùng hai đầu + đoạn giữa
lặp, mỗi phần chỉ scale đều hai chiều theo chiều cao đích; ảnh vừa phải dùng crop phủ kín
sau khi scale đều. `timeline_bar_thin` giữ 1300×20 vì hoa văn đầu đã được giảm gọn và vẫn
đọc được ở chiều cao này. Không có ảnh nào bị scale riêng một trục.

Round 3: mọi file xuất ở ít nhất 2× rect (4× cho badge/pill nhỏ), riêng file vượt giới hạn
được lấy tỷ lệ lớn nhất có cạnh dài ≤2048. Thanh caps + middle dùng đoạn giữa rộng tối thiểu
1,5× chiều cao và cross-fade 15%; `log_panel_expanded` ghép 2D từ bốn góc, bốn cạnh và tâm.
Inset thân khung so với outer bounding box (left/top/right/bottom, % canvas) để builder canh
nội dung: `tooltip_panel` 4/5/4/6; `hero_portrait_frame_wide` 12/10/12/8;
`bag_board_landscape` 10/12/10/10; `fighter_panel_replay` 8/7/8/5;
`plaque_label_compact` 9/20/9/8; `plaque_label_medium` 7/18/7/8;
`plaque_label_tall` 8/17/8/7; `frame_panel_square` 7/7/7/7; ba file
`slot_cell_*_square` cùng geometry 8/8/8/8 và chỉ khác fill/trạng thái.

Round 4: bảy ảnh bị crop viền được dựng lại từ source untouched đã tight-crop alpha > 16.
Năm khung dùng chín mảnh, giữ nguyên bốn góc và scale đều; ba slot dùng chung corner 260×260.
Hai badge/pill dùng source image-gen mới, scale đều theo chiều cao rồi ghép caps + middle để rim vàng
hiện đủ bốn phía. Checker đối chiếu RGBA bốn góc (MAD < 6) và kiểm rim trong dải ngoài 4%.
