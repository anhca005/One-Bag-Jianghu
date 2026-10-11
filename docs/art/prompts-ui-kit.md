# Bộ UI chỉn chu (đợt 2): danh sách và prompt ChatGPT

Đợt 1 ([prompts-ui.md](prompts-ui.md), #1–43) đã có nền màn hình, khung panel, 2 kiểu nút, banner Home, huy hiệu phẩm và icon trạng thái. Đợt 2 thay những phần trong game **còn là mảng màu phẳng** hoặc dùng tạm hình khác: tab, nút đóng, ô túi, thanh máu, thanh cuộn, công tắc, thẻ shop, màn thắng/thua, icon thao tác…

## Cách gửi
- Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/`, **đúng tên cột File**.
- Mỗi **nhóm** gửi trong **một cuộc trò chuyện riêng**: tin mở đầu (kèm `docs/art/reference/01-style-board-thuy-mac.png` và một hình đợt 1 cùng loại, ví dụ `frame_panel.png`, `button_primary.png`), rồi mỗi tin một hình. Cùng cuộc trò chuyện thì các hình sẽ đồng bộ nét và màu.
- ChatGPT chỉ ra 1024x1024, 1536x1024 hoặc 1024x1536. Hình dạng **thanh mảnh** (thanh máu, tab, dải tiêu đề) cứ vẽ ở khổ được ghi, đặt vật ở giữa; Claude sẽ cắt sát.
- Nền ra ô caro thì để nguyên, Claude chạy `tools/art/remove_checker.py`.
- Hình **9-slice** (co giãn được): bốn góc có hoa văn, **các cạnh thẳng phải trơn, đều**, phần giữa trơn. Đã ghi rõ trong prompt.

### Tin mở đầu (dùng cho mọi nhóm)
```
We will create a consistent UI kit for a 2D wuxia game, one image per message, in exactly the style of the attached references.
Style: 2D Chinese ink-brush (shuimo) on aged rice paper, confident black brush outlines, flat ink washes with dry-brush texture, vermilion and ochre-gold accents. Palette only: rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
Strictly 2D and flat: no 3D, no bevel, no glow, no lens effects, no drop shadow outside the shape. No text, no letters, no numbers, no watermark.
UI pieces must be centered with a transparent background. Pieces marked "9-slice" have ornate corners but perfectly straight, plain, even edges and a plain center so they can be stretched.
Reply "ready" and wait.
```

---

## Nhóm A: Khung popup và điều khiển chung (ưu tiên 1, dùng ở mọi popup)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 44 | `popup_frame.png` | 1024x1024 | Khung mọi popup lớn (Thương Hội, Tàng Bảo Các, Bao Phục, Bách Khoa, Môn Phái…), thay `frame_panel` | `9-slice popup window frame: a pale rice-paper sheet mounted on a thin dark-lacquer wooden border, ochre-gold cloud-scroll brackets on all four corners, a faint light-ink wash vignette toward the edges, the center plain paper. Square, fills the image.` |
| 45 | `popup_title_ribbon.png` | 1536x1024 | Dải tiêu đề trên đầu popup (chữ do game viết) | `A wide horizontal title ribbon for a popup header: a dark ink-black lacquer plaque with a thin ochre-gold inner line, small vermilion tassels hanging from both ends, swallow-tail cut ends, the middle long and plain. Place it centered, about one fifth of the image height.` (9-slice ngang) |
| 46 | `button_close.png` | 1024x1024 | Nút đóng ở góc phải trên popup | `A round close button: a vermilion seal-like disc with a rough carved rim and two crossed bold ink brush strokes forming an X in its center, ochre-gold thin ring outside. Centered, readable at 48x48 pixels.` |
| 47 | `tab_active.png` | 1536x1024 | Tab đang chọn (Bách Khoa, Tàng Bảo Các, Thương Hội) | `A single horizontal tab shaped like a hanging cloth bookmark: vermilion silk with a thin ochre-gold border and a small knot at the top, slightly larger and brighter. Centered, about one third of the image width, plain middle.` (9-slice) |
| 48 | `tab_idle.png` | 1536x1024 | Tab chưa chọn | `The same tab shape as before but in muted light-ink grey silk with a thin dark border, flatter and quieter.` |
| 49 | `panel_inset.png` | 1024x1024 | Khung lõm cho vùng chi tiết, danh sách, mô tả trong popup | `9-slice inset panel: a slightly darker rice-paper area with a thin light-ink brush border that looks pressed into the page, tiny ink dots at the corners, plain center. Square.` |
| 50 | `list_row.png` | 1536x1024 | Nền một dòng trong danh sách (cốt truyện, bách khoa, phần thưởng) | `9-slice list row: a long thin strip of pale paper with a faint light-ink underline brush stroke and tiny cloud marks at both ends, centered, plain middle.` |
| 51 | `list_row_selected.png` | 1536x1024 | Dòng đang chọn | `The same list row strip, but with a soft ochre-gold wash and a small vermilion marker at the left end.` |
| 52 | `divider_brush.png` | 1536x1024 | Nét chia ngang giữa các mục | `A single long horizontal dry-brush ink stroke that tapers at both ends, with a tiny vermilion dot in the middle. Thin, centered.` |
| 53 | `button_disabled.png` | 1536x1024 | Nút bị khóa (vd "Nâng Tâm Pháp – Sắp mở") | `The same wide button shape as the vermilion lacquer button, but faded light-ink grey with a thin worn border, looking inactive. Centered, no text.` (9-slice) |
| 54 | `button_small_gold.png` | 1536x1024 | Nút phụ nhỏ (x1/x2/x4, Sắp xếp, Làm mới) | `A small rounded rectangular button: ochre-gold lacquer plate with a thin deep-ink border and tiny cloud marks at the ends, centered, plain middle, no text.` (9-slice) |

## Nhóm B: Điều khiển trong Cài đặt và danh sách cuộn (ưu tiên 2)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 55 | `slider_track.png` | 1536x1024 | Thanh âm lượng | `A thin horizontal slider track: a split bamboo groove with dark ink inside and small ochre-gold caps at both ends, centered, long and thin.` (9-slice ngang) |
| 56 | `slider_fill.png` | 1536x1024 | Phần đã kéo của thanh trượt | `A thin horizontal fill bar matching the bamboo groove: a smooth vermilion ink wash with a soft dry-brush edge, centered, long and thin.` |
| 57 | `slider_handle.png` | 1024x1024 | Núm kéo | `A round slider knob: a small jade bead with an ochre-gold ring and a dark ink outline, centered, readable at 40x40 pixels.` |
| 58 | `toggle_off.png` | 1024x1024 | Ô bật/tắt (Toàn màn hình…) | `An empty square checkbox drawn as a small paper square with a thin deep-ink brush border and rounded corners, centered.` |
| 59 | `toggle_on.png` | 1024x1024 | Ô đã bật | `The same paper checkbox with a bold vermilion brush check mark inside.` |
| 60 | `scrollbar_track.png` | 1024x1536 | Rãnh thanh cuộn dọc | `A thin vertical scrollbar track: a narrow groove of light ink on paper with tiny caps at top and bottom, centered, tall and thin.` (9-slice dọc) |
| 61 | `scrollbar_handle.png` | 1024x1536 | Tay cuộn | `A thin vertical scrollbar handle: a slim dark-lacquer rod with ochre-gold bands near both ends, centered, tall and thin.` (9-slice dọc) |
| 62 | `dropdown_field.png` | 1536x1024 | Ô chọn (độ phân giải) | `9-slice dropdown field: a pale paper strip with a thin deep-ink border and a small downward ink-brush chevron at the right end, centered, plain middle.` |

## Nhóm C: Túi đồ, Thương Hội, Tàng Bảo Các (ưu tiên 1)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 63 | `slot_cell.png` | 1024x1024 | Một ô lưới túi 5x4 và lưới kho | `A single square inventory cell: a recessed square of darker aged paper with a thin light-ink border and tiny corner marks, plain inside. Must tile cleanly side by side. Centered, fills the image.` |
| 64 | `slot_cell_highlight.png` | 1024x1024 | Ô hợp lệ khi kéo đồ vào | `The same inventory cell with a soft jade-green ink wash inside and a brighter border.` |
| 65 | `slot_cell_invalid.png` | 1024x1024 | Ô không đặt được | `The same inventory cell with a soft vermilion ink wash inside.` |
| 66 | `bag_board.png` | 1536x1024 | Nền dưới lưới túi (tấm vải tay nải trải ra) | `An unfolded square-ish travelling cloth (a wanderer's bundle spread open) seen from above, deep indigo-ink cloth with a woven texture and an ochre-gold stitched border, corners knotted, plain flat center for a grid.` (9-slice) |
| 67 | `shop_card.png` | 1024x1536 | Thẻ hàng ở Thương Hội | `9-slice vertical merchant item card: a pale paper card on a thin dark-wood frame, a small hanging cord at the top, a plain upper area for an item picture and a slightly darker band at the bottom for a price. No text.` |
| 68 | `tier_pill.png` | 1536x1024 | Nhãn "Trọng 3" trên vật phẩm | `A tiny horizontal pill-shaped tag: deep ink lacquer with a thin ochre-gold rim, plain inside. Centered, small.` (9-slice) |
| 69 | `price_tag.png` | 1536x1024 | Ô giá tiền trên thẻ | `A small horizontal wooden price tag with a round socket on the left for a coin icon and a plain area for a number, thin ochre-gold edge.` (9-slice) |

## Nhóm D: Icon thao tác (ưu tiên 2; một biểu tượng đậm, đọc được ở 32x32)

Thêm vào tin mở đầu: `These are small action icons: one bold simple symbol, ink and ochre-gold, readable at 32x32 pixels, transparent background.`

| # | File | Nghĩa | Prompt |
|---|---|---|---|
| 70 | `icon_upgrade.png` | Nâng Trọng | `Icon: an upward ochre-gold arrow made of a single brush stroke over a small anvil.` |
| 71 | `icon_merge.png` | Hợp Thành | `Icon: two small jade pieces joining into one with a spark between them.` |
| 72 | `icon_sell.png` | Bán | `Icon: a single round coin with a square hole sliding off an open palm.` |
| 73 | `icon_refresh.png` | Làm mới shop | `Icon: a circular brush stroke forming a loop arrow.` |
| 74 | `icon_power.png` | Lực Chiến | `Icon: a clenched fist inside a small flame of ochre-gold qi.` |
| 75 | `icon_exp.png` | EXP | `Icon: a small rolled scroll with a rising wisp of light.` |
| 76 | `icon_sort.png` | Sắp xếp | `Icon: three stacked horizontal brush strokes of decreasing length.` |
| 77 | `icon_info.png` | Bách Khoa / thông tin | `Icon: an open book with a small vermilion bookmark ribbon.` |
| 78 | `icon_play.png` | Phát trận | `Icon: a right-pointing triangle drawn with one bold ink stroke.` |
| 79 | `icon_pause.png` | Tạm dừng | `Icon: two vertical bold ink strokes.` |
| 80 | `icon_speed.png` | Tua nhanh | `Icon: two right-pointing triangles side by side drawn in bold ink.` |
| 81 | `icon_skip.png` | Bỏ qua | `Icon: a right-pointing triangle followed by a vertical bar, bold ink.` |

## Nhóm E: Màn trận (ưu tiên 1)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 82 | `bar_frame.png` | 1536x1024 | Khung thanh Khí Huyết / Cương Khí / Nội Lực | `A long thin horizontal gauge frame: a dark lacquer tube with ochre-gold caps at both ends and a thin gold inner line, the inside empty and transparent. Centered, long and thin.` (9-slice ngang) |
| 83 | `bar_fill_hp.png` | 1536x1024 | Phần máu | `A long thin horizontal fill: rich vermilion ink wash with a dry-brush texture and a slightly lighter top edge. Centered, long and thin, no frame.` |
| 84 | `bar_fill_shield.png` | 1536x1024 | Cương Khí | `The same long thin fill in pale ochre-gold with a soft shimmer-free texture.` |
| 85 | `bar_fill_qi.png` | 1536x1024 | Nội Lực | `The same long thin fill in misty blue-grey ink.` |
| 86 | `fighter_panel.png` | 1024x1536 | Nền khung mỗi bên (chân dung + túi) | `9-slice tall side panel: aged paper with a thin dark-wood border, an ochre-gold bracket at the top center, plain center.` |
| 87 | `vs_emblem.png` | 1024x1024 | Giữa hai bên lúc vào trận | `Two crossed swords over a round vermilion seal disc with rough edges, bold silhouette, no text.` |
| 88 | `timeline_bar.png` | 1536x1024 | Thanh thời gian trận | `A long thin horizontal track like a stretched silk cord with small knots at regular intervals and tassels at both ends. Centered, long and thin.` (9-slice ngang) |

## Nhóm F: Kết quả trận và phần thưởng (ưu tiên 1)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 89 | `result_victory.png` | 1536x1024 | Nền tiêu đề khi thắng (chữ "Thắng" do game viết) | `A victory emblem: a large burst of vermilion ink splash behind a pair of crossed ochre-gold swords, a few flying cranes and cloud scrolls around it, the center calm enough for a large title on top. No text.` |
| 90 | `result_defeat.png` | 1536x1024 | Nền tiêu đề khi thua | `A defeat emblem: a broken sword lying on grey ink-washed ground under falling withered leaves, muted light-ink tones with only a hint of vermilion, the center calm for a title. No text.` |
| 91 | `reward_slot.png` | 1024x1024 | Ô phần thưởng (Bạc, EXP, nguyên liệu) | `A small square reward slot: a dark lacquer tray with an ochre-gold rim and tiny cloud corners, plain inside.` |
| 92 | `star_burst.png` | 1024x1024 | Sau ngôi sao đánh giá ải | `A soft round ochre-gold ink-wash halo with short radiating brush strokes, very flat, centered.` |

## Nhóm G: Hướng dẫn và Bách Khoa (ưu tiên 3)

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 93 | `tutorial_pointer.png` | 1024x1024 | Mũi chỉ vào chỗ cần bấm | `A pointing brush-drawn hand with the index finger extended, wide sleeve cuff in vermilion, bold outline, pointing down-left.` |
| 94 | `tutorial_ring.png` | 1024x1024 | Vòng khoanh quanh mục tiêu (thay khung vàng vuông) | `A hand-drawn ochre-gold brush circle loop, open slightly at the top right where the stroke starts and ends, thin and confident, empty transparent inside.` |
| 95 | `encyclopedia_icon_items.png` | 1024x1024 | Mục Vật phẩm | `Icon: a sword and a small gourd crossed, bold ink, readable at 48x48.` |
| 96 | `encyclopedia_icon_status.png` | 1024x1024 | Mục Trạng thái | `Icon: an acupuncture needle over a small swirl, bold ink.` |
| 97 | `encyclopedia_icon_story.png` | 1024x1024 | Mục Cốt truyện | `Icon: a half-unrolled scroll with a tiny mountain painting, bold ink.` |
| 98 | `encyclopedia_icon_rules.png` | 1024x1024 | Mục Luật chơi | `Icon: an open travelling bundle showing a small grid of squares, bold ink.` |

## Nhóm H: Mảnh riêng cho màn trận và nút nhỏ (2026-10-10)

Lý do: các lỗi méo/tràn trên màn trận đến từ việc kéo giãn hình nút, khung panel và thanh loading làm nền cho thứ khác. Nhóm này vẽ **đúng tỉ lệ dùng thật**, nên chỉ phải co giãn ít.

| # | File | Khổ | Dùng ở | Prompt |
|---|---|---|---|---|
| 99 | `hud_panel.png` | 1536x1024 | Nền khung thông tin mỗi bên trên top banner (khoảng 780x150) | `9-slice wide horizontal plaque, about 5:1 proportion, centered: smooth pale rice paper inside a thin dark-lacquer border with small ochre-gold corner brackets, a faint ink-wash vignette inside the edges, plain even center for bars and text.` |
| 100 | `gauge_frame_large.png` | 1536x1024 | Khung thanh Khí Huyết (khoảng 620x40) | `A long slim horizontal gauge frame drawn for a health bar: a recessed dark-ink channel with a thin ochre-gold rim and small cloud-scroll caps at both ends, the inner channel empty and transparent, edges straight and even. Very long and thin (about 16:1), centered.` |
| 101 | `gauge_frame_small.png` | 1536x1024 | Khung thanh Cương Khí và Nội Lực (khoảng 620x20) | `The same gauge frame style but thinner and simpler: a narrow recessed channel with a hairline gold rim and tiny round caps, the inside empty and transparent. Very long and thin (about 30:1), centered.` |
| 102 | `status_slot.png` | 1024x1024 | Ô đặt icon trạng thái (khoảng 40x40) | `A small round socket: a dark ink disc with a thin ochre-gold ring and a slightly lighter inner rim, plain inside so an icon can sit on it, centered, readable at 40x40 pixels.` |
| 103 | `count_badge.png` | 1024x1024 | Số lớp trạng thái ở góc ô | `A tiny rounded pill badge: deep vermilion lacquer with a thin paper-white rim, plain inside for one or two digits, centered.` |
| 104 | `log_panel.png` | 1536x1024 | Nền ô nhật ký trận (khoảng 920x96 khi thu gọn, 1200x430 khi mở) | `9-slice wide low panel for a text log: a deep ink-wash paper slip with a thin ochre-gold inner line and tiny cloud marks only at the four corners, the straight edges plain and even, a large plain dark center for light text.` |
| 105 | `button_exit.png` | 1024x1024 | Nút Thoát trận (khoảng 64x64, có chữ nhỏ bên dưới do game viết) | `A round exit button: a bright vermilion seal disc with a thick paper-white rim and a bold white brush-stroke arrow leaving through a simple open gate shape, high contrast, readable at 48x48 pixels, centered.` |
| 106 | `name_plate.png` | 1536x1024 | Bảng tên trên khung chân dung (khoảng 230x48) | `9-slice small horizontal name plate: dark lacquer board with a thin ochre-gold rim and tiny knots at both ends, plain center for light text, about 5:1 proportion, centered.` |
| 107 | `clock_plate.png` | 1024x1024 | Nền sau đồng hồ trận và tên chế độ (khoảng 280x110) | `A rounded oval medallion plate: deep ink lacquer with a thin ochre-gold ring and faint cloud scrolls at the left and right ends, plain dark center for light numbers, centered, wider than tall (about 5:2).` |
| 108 | `button_square.png` | 1024x1024 | Nút nhỏ hình vuông chỉ có icon hoặc 1 ký tự (−, +, x2…) | `A small square button with rounded corners: dark lacquer plate with a thin ochre-gold rim and tiny corner dots, plain center, centered. Designed to be 9-slice.` |
| 109 | `shop_card_wide.png` | 1536x1024 (cắt về đúng tỉ lệ 7:5) | Thẻ hàng ở Thương Hội (420x300). Thay `shop_card` (vẽ dọc, kéo ngang bị méo). Không vẽ nút dây vì game đã có `shop_card_knot` riêng | `A landscape item card, exactly 7:5 proportion, centered, filling the frame: pale rice-paper face inside a thin dark-lacquer border with small ochre-gold fret (key-pattern) corner brackets; along the bottom a separate footer band about one fifth of the card height, slightly darker parchment, divided from the face by a thin gold line with small fret corners; plain even surfaces for an item picture, text and a button; no cord, no knot, no text.` |

---

## Thứ tự nên vẽ
1. **Nhóm A, C, E, F** (≈ 37 hình): thay đổi lớn nhất về cảm giác chỉn chu.
2. Nhóm B, D.
3. Nhóm G.

Vẽ xong nhóm nào thì báo, Claude cắt, tách nền, đặt 9-slice và giao Codex gắn vào các builder (mọi chỗ đều giữ dự phòng khi thiếu hình).

## Nhóm I: Nhiệm vụ — khung theo dõi và Hành Tẩu Lục (2026-10-11)

Tin mở đầu dùng đúng phần “Tin mở đầu” ở đầu tài liệu. Các ảnh dưới đây được tạo bằng image generation tích hợp,
tham chiếu style board và các ảnh `hud_panel`, `shop_card_wide`, `price_tag`, `popup_frame` phù hợp.

| # | File | Khổ dùng | Prompt |
|---|---|---|---|
| 110 | `quest_tracker.png` | 880x256 | `Redraw as one extra-wide 55:16 wuxia quest-tracker plaque filling at least 94% of the transparent canvas: a warm pale aged rice-paper face spanning at least 80% of the plaque height, with extremely subtle even texture and a clean unobstructed field on the right for two lines of game-rendered text. Use a thin dark brown-black lacquer border, restrained ochre-gold linework, compact cloud-scroll corner ornaments, and tiny vermilion accents that stay inside the silhouette and outside the paper face. Integrate one perfectly round empty dark slot on the left, vertically centered near 12% of the width, with an outer gold circle about 70% of total image height. Match the hand-painted ink-and-rice-paper UI family. Genuine transparent alpha outside; no more than 3% transparent padding per edge. No text, letters, numbers, symbols, watermark, checkerboard, scenery, dangling tassels, oversized ornaments, or empty top/bottom bands. Used whole, not 9-sliced.` |
| 111 | `quest_badge_main.png` | 72x30 | `A compact horizontal vermilion lacquer tag, about 12:5 proportion, thin ochre-gold rim, tiny restrained ink-brush end caps, completely plain center for a game-rendered label.` |
| 112 | `quest_badge_side.png` | 72x30 | `A very simple dark ink-black lacquer rounded tag, exactly about 12:5 proportion, thin ochre-gold outline, plain uninterrupted center for a game-rendered label. No icon socket, circle, tassels or extensions.` |
| 113 | `quest_icon_scroll.png` | 56x56 | `One small tied mission scroll, rolled aged rice paper with a vermilion cord and tiny ochre-gold clasp, bold simple silhouette readable at 56 pixels.` |
| 114 | `quest_log_row.png` | 820x150 | `A wide aged rice-paper strip at about 11:2 proportion with a thin dark lacquer outline and one thin ochre-gold rule, restrained tiny corner brackets, a clearly reserved square empty slot on the left for a separate status icon, and a large plain paper region for text and rewards. Used whole, not 9-sliced.` |
| 115 | `quest_status_done.png` | 56x56 | `A bold vermilion check mark painted inside a rough-edged round vermilion seal disc, with a very thin ochre-gold rim; purely pictorial, no written character.` |
| 116 | `quest_status_active.png` | 56x56 | `One open ochre-gold ensō brush circle, bold simple single stroke with a small natural gap, faint deep-ink outline, empty transparent center.` |
| 117 | `quest_status_locked.png` | 56x56 | `One compact closed dark-ink padlock with a simple rounded shackle, tiny ochre-gold edge highlights, bold flat silhouette readable at 56 pixels.` |
| 118 | `quest_glow_spark.png` | 48x48 | `One compact warm ochre-gold light spark with a bright rice-paper-white core and a short tapered tail pointing left, soft transparent feathered edges, simple enough at 48 pixels. This is the only asset in the kit allowed to look softly luminous.` |

Ràng buộc chung đã dùng cho #110–118: vật thể giữa khung, khoảng 2% đệm, nền alpha thật; không chữ, số, ký tự,
triện có chữ, watermark, 3D hay bevel. Riêng #118 có ánh sáng mềm; các ảnh còn lại không glow hoặc bóng ngoài.

## Nhóm J: Ảnh đúng tỷ lệ cho UI Simple (2026-10-11)

Các prompt dưới đây dùng ảnh cùng vai trò hiện có đặt cạnh prompt làm style reference. Ràng buộc chung:
Thủy Mặc mực và sơn mài, giấy gạo cũ ấm, mực nâu-đen, nét vàng đất tiết chế, điểm son rất nhỏ;
alpha thật ngoài silhouette; hoa văn nằm trọn trong canvas; vùng giữa phẳng và yên cho nội dung do game vẽ;
không chữ, số, ký tự, triện chữ, watermark, cảnh vật, người, động vật, checkerboard, glow, bóng đổ, 3D hay bevel.
Ảnh được dùng nguyên tấm bằng `Image.Type.Simple`, không 9-slice.

Round 2: với #119–125, #127, #131, #133 và #136, nguồn sinh phải là thanh ngang chiếm gần
trọn chiều rộng ảnh. Khi khổ đích dài hơn tỷ lệ mà image generator hỗ trợ, cắt hai đầu hoa
văn và một đoạn giữa phẳng để lặp; chỉ scale đều hai chiều theo chiều cao.

Round 3: nguồn #134 phải là một frame kín nhìn chính diện, đủ bốn góc hoa văn, bốn cạnh dài
phẳng và tâm tối yên để ghép 2D. Nguồn #135 là pill sơn mài 7:4 có viền vàng và end caps đậm,
silhouette alpha thật, chiếm gần trọn canvas và vẫn đọc rõ ở rect 42×24. Mọi nguồn được ghép
ở pixel size cuối; middle ngang rộng ≥1,5× chiều cao và nối bằng cross-fade khoảng 15%.

| # | File | Khổ dùng | Ảnh tham chiếu | Prompt |
|---|---|---|---|---|
| 119 | `replay_top_bar.png` | 1920×164 | `frame_panel.png` | `One extremely wide 11.707:1 replay top bar; dark lacquer face, thin ochre-gold rules and compact cloud-scroll end ornaments; calm uninterrupted center.` |
| 120 | `replay_bottom_bar.png` | 1920×120 | `frame_panel.png` | `One ultra-wide 16:1 replay bottom bar; open pale-paper center, thin dark lacquer and gold border, very compact end ornaments.` |
| 121 | `home_banner_simple.png` | 1920×132 | `home_banner.png` | `One ultra-wide 14.545:1 home header scroll; pale rice-paper center, restrained ink edge and compact gold cloud ornaments only at the ends.` |
| 122 | `hud_panel_simple.png` | 780×148 | `hud_panel.png` | `One 5.270:1 battle HUD plaque; pale rice-paper center, thin dark-lacquer border, tiny gold corner scrolls and faint grey ink wash at the outer edges.` |
| 123 | `button_replay_control.png` | 170×64 | `button_secondary.png` | `One compact 2.656:1 dark lacquer replay control button; thin gold rim, tiny cloud ends, bold simple silhouette and plain center.` |
| 124 | `panel_paper_strip.png` | 360×64 | `frame_panel.png` | `One 5.625:1 pale paper panel strip; thin dark lacquer border, compact gold corner brackets and a completely clear center.` |
| 125 | `tooltip_field.png` | 360×64 | `frame_tooltip.png` | `One 5.625:1 low paper tooltip field; dry-brush ink outline, tiny restrained corner curls and a plain light center.` |
| 126 | `tooltip_panel.png` | 440×260 | `frame_tooltip.png` | `One 1.692:1 paper tooltip panel; irregular dry-brush ink frame, tiny vermilion corner dots and a large quiet center.` |
| 127 | `input_field_wide.png` | 620×64 | `frame_tooltip.png` | `One very wide 9.688:1 paper input field; fine dry-brush ink edge, minimal curls at both ends and a clear uninterrupted center.` |
| 128 | `hero_portrait_frame_wide.png` | 222×276 | `hero_portrait_frame.png` | `One portrait frame exactly 0.804:1; black lacquer and gold structure wrapped by restrained vermilion ribbons; fully transparent empty interior.` |
| 129 | `bag_board_landscape.png` | 540×450 | `bag_board.png` | `One landscape 1.2:1 unfolded indigo travelling cloth; woven ink texture, gold stitched edge, four compact knotted corners and a flat plain center.` |
| 130 | `count_badge_compact.png` | 20×14 | `count_badge.png` | `One tiny 10:7 vermilion pill badge; paper-white inner rim and hairline dark outline; bold, simple and readable at 20×14.` |
| 131 | `dropdown_field_medium.png` | 360×64 | `dropdown_field.png` | `One 5.625:1 paper dropdown field; thin ink and gold edge, plain center and one small pictorial down chevron at the right.` |
| 132 | `fighter_panel_replay.png` | 250×360 | `fighter_panel.png` | `One 25:36 tall fighter panel; pale rice paper, slim black lacquer rails, small gold brackets and one restrained top crest.` |
| 133 | `log_panel_wide.png` | 1300×96 | `log_panel.png` | `One 13.542:1 dark battle-log strip; near-black ink-wash center, hairline gold inner rule and tiny cloud marks only at corners.` |
| 134 | `log_panel_expanded.png` | 1320×440 | `log_panel.png` | `One 3:1 expanded dark battle-log panel; near-black ink-wash center, thin gold rule and compact corner clouds.` |
| 135 | `tier_pill_compact.png` | 42×24 | `tier_pill.png` | `One compact 7:4 black lacquer tier pill; slim ochre-gold rim, tiny end curls and a plain center readable at 42×24.` |
| 136 | `timeline_bar_thin.png` | 1300×20 | `timeline_bar.png` | `One ultra-thin 65:1 battle timeline cord; straight gold silk line, tiny regular knots and compact tasselled end caps entirely inside the canvas.` |
| 137 | `plaque_label_compact.png` | 100×46 | `plaque_label.png` | `One compact 50:23 hanging paper plaque; black wood frame, gold corner caps and very short vermilion cords contained inside the canvas.` |
| 138 | `plaque_label_medium.png` | 260×92 | `plaque_label.png` | `One 65:23 hanging paper plaque; black wood frame, thin gold corners, short vermilion ties and a wide clear paper face.` |
| 139 | `plaque_label_tall.png` | 240×101.25 | `plaque_label.png` | `One 64:27 taller hanging paper plaque; black wood frame, gold corner caps, short vermilion ties and a clear paper face.` |
