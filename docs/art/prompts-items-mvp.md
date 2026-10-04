# Prompt icon vật phẩm MVP (ChatGPT)

**Đã xong cả 14 món** (2026-09-27), lưu ở `One-Bag-Jianghu/Assets/_Game/Textures2D/Weapons/`, đã tách nền bằng `tools/art/remove_checker.py` (bản mới xóa được cả ô caro trong vùng kín) và nạp vào `ItemIconLibrary`.

## Cách làm
1. Mở **một** cuộc trò chuyện ChatGPT mới, đính kèm 2 hình: `01-style-board-thuy-mac.png` và `02-icon-thanh-phong-kiem.png`.
2. Gửi **tin mở đầu** bên dưới, để ChatGPT nhớ phong cách cho cả cuộc trò chuyện.
3. Sau đó mỗi tin nhắn dán **một** dòng từ bảng. Làm lần lượt, không gộp nhiều món một lần.
4. Hình nào lệch phong cách thì trả lời: `Too detailed / too realistic. Match the sword icon exactly: same outline weight, same ink texture, same level of detail.`
5. Tải về, đặt tên theo cột **File**, gửi mình khi xong vài món để kiểm tra.

## Tin mở đầu

```
We will create a set of game item icons, one per message, all in exactly the same style as the attached images (the sword icon is the reference for icons).

Style for every icon: 2D Chinese ink-brush (shuimo) illustration with a clean anime-influenced wuxia look. Confident black brush-stroke outlines of varying thickness, flat ink washes in 2-3 tones with visible dry-brush texture, vermilion red accents on ribbons and tassels, ochre gold ornaments.
Palette only: rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
Strictly 2D: no 3D render, no glossy reflections, no glow, no bloom, no rim light, no depth of field.
Every icon: single object only, seen from a flat front or side view, centered, filling about 85% of the canvas, fully transparent background (no paper texture, no scenery, no ground shadow, no frame), no text.
Detail level follows rarity: common/uncommon = simple, few ornaments; rare/epic = gold ornaments and a tassel; legendary/supreme = rich ornaments and flat ink-swirl strokes around it.

Reply "ready" and wait for my first item.
```

## Danh sách

Mỗi dòng trong cột **Prompt** là một tin nhắn.

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 1 | Thiết Cốt Quyền Sáo | 1x2 | `thiet_cot_quyen_sao.png` | `Icon (common, 1024x1536 portrait, standing vertically): a pair of plain iron knuckle-dusters stacked one above the other, wrapped with worn dark cloth, simple and sturdy.` |
| 2 | Phục Hổ Côn | 1x4 | `phuc_ho_con.png` | `Icon (epic, 1024x1536 portrait, perfectly vertical and very thin, the staff must span the full height): a long monk's iron-capped wooden staff, ochre gold bands at both ends, a small tiger head engraved on one cap, vermilion prayer-bead tassel near the top.` |
| 3 | Liệt Diễm Đao | 1x3 | `liet_diem_dao.png` | `Icon (rare, 1024x1536 portrait, standing vertically, tip up, slender): a broad curved dao sabre, dark blade with vermilion flame-shaped ink strokes along the back edge, ochre gold guard, red tassel.` |
| 4 | Thất Tinh Tiêu | 1x2 | `that_tinh_tieu.png` | `Icon (uncommon, 1024x1536 portrait, standing vertically): a small bundle of seven slim throwing darts tied together with a dark cord, tips pointing up, the tips stained with a subtle green-grey ink wash (poison).` |
| 5 | Tử Kim Trảo | 1x2 | `tu_kim_trao.png` | `Icon (rare, 1024x1536 portrait, standing vertically): a single iron tiger-claw gauntlet with three long curved blades pointing up, dark purple-grey metal drawn in ink, ochre gold wrist band, a drop of vermilion on one blade tip.` |
| 6 | Khai Sơn Phủ | 2x2 | `khai_son_phu.png` | `Icon (rare, 1024x1024 square): a heavy two-handed battle axe with a wide crescent blade, dark iron with ink texture, ochre gold engraving on the blade, short thick wooden haft wrapped in red cord, placed diagonally.` |
| 7 | Phán Quan Bút | 1x2 | `phan_quan_but.png` | `Icon (rare, 1024x1536 portrait, standing vertically, tip up): an oversized iron calligraphy brush used as a weapon, sharp metal tip instead of hair, black lacquered shaft with ochre gold rings, small vermilion tassel.` |
| 8 | Hàn Ngọc Tiêu | 1x2 | `han_ngoc_tieu.png` | `Icon (epic, 1024x1536 portrait, standing vertically): a pale blue-grey jade flute (xiao) with finger holes, faint frost-like flat ink strokes around it, ochre gold end caps, long vermilion tassel.` |
| 9 | Bích Thủy Cầm | 2x2 | `bich_thuy_cam.png` | `Icon (rare, 1024x1024 square): a guqin zither seen from above at a slight diagonal, dark wood body with blue-grey wave patterns painted in ink, seven thin strings, ochre gold tuning pegs.` |
| 10 | Thái Thanh Tâm Kinh | 1x1 | `thai_thanh_tam_kinh.png` | `Icon (rare, 1024x1024 square): a closed martial arts manual, thread-bound Chinese book with a blue-grey cover, a blank vermilion label strip on the cover (no characters), a yin-yang ink motif.` |
| 11 | Ô Tàm Y | 2x2 | `o_tam_y.png` | `Icon (uncommon, 1024x1024 square): a dark silk inner armor vest laid flat, front view, subtle woven scale pattern in light ink, simple ochre ties at the front.` |
| 12 | Bồi Nguyên Đan | 1x1 | `boi_nguyen_dan.png` | `Icon (uncommon, 1024x1024 square): a small round medicine pill, ochre gold with a single vermilion dot, resting on a tiny blue-grey porcelain dish.` |
| 13 | Túy Tiên Hồ Lô | 1x1 | `tuy_tien_ho_lo.png` | `Icon (epic, 1024x1024 square): a calabash wine gourd, ochre with ink cloud patterns, a red cord tied around its waist with a long vermilion tassel, cork stopper on top.` |

## Ghi chú
- Món 1x3 và 1x4 rất mảnh; nếu ChatGPT vẽ quá to bề ngang, trả lời: `Make it much thinner so it fits a 1:4 strip, keep it perfectly vertical.`
- ChatGPT có thể không cho nền trong suốt thật. Khi đó cứ tải về, mình sẽ tách nền bằng script.

## Đợt 2: 7 binh khí mới (xong 2026-09-29)

Cùng cuộc trò chuyện với đợt 1 (hoặc mở mới và gửi lại tin mở đầu + đính kèm 2–3 icon đã duyệt). Lưu vào `Textures2D/Weapons/`.

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 14 | Xích Sa Chưởng | 1x2 | `xich_sa_chuong.png` | `Icon (uncommon, 1024x1536 portrait, standing vertically): ONE single empty fingerless palm wrap (not a pair, no hand, no fingers inside), shown upright with the wrist cuff at the bottom and the open palm at the top, dark red cloth bands wound around it and tied with a cord, a few small iron studs on the cuff, faint vermilion heat-shimmer brush strokes rising from the palm. Ink-brush shuimo style like the other icons: confident black brush outline, flat ink washes, dry-brush texture, muted colors, no glossy shading, no cartoon highlights. Transparent background.` |
| 15 | Ngọc Cốt Phiến | 1x2 | `ngoc_cot_phien.png` | `Icon (rare, 1024x1536 portrait, standing vertically, folded fan held closed with the tip up): a war fan with pale jade ribs and sharp steel tips, ink landscape painted on the silk, small vermilion tassel.` |
| 16 | Băng Phách Thương | 1x4 | `bang_phach_thuong.png` | `Icon (epic, 1024x1536 portrait, perfectly vertical and very thin, spanning the full height): a long spear with a pale blue-grey leaf-shaped blade, frost-like flat strokes around the head, red horsehair tassel below the blade, dark shaft with ochre gold bands.` |
| 17 | Chấn Nhạc Chùy | 2x2 | `chan_nhac_chuy.png` | `Icon (rare, 1024x1024 square, placed diagonally): a heavy melon-shaped war hammer with a ribbed iron head, ochre gold rings, short thick handle wrapped in red cord.` |
| 18 | Thanh Đằng Tiên | 1x3 | `thanh_dang_tien.png` | `Icon (uncommon, 1024x1536 portrait, coiled vertically, slender): a green-grey rattan whip coiled in loose loops with a wooden handle and a small vermilion knot, dry-brush texture.` |
| 19 | Tử Mẫu Song Đao | 2x2 | `tu_mau_song_dao.png` | `Icon (rare, 1024x1024 square): a matched pair of short curved twin sabres crossed in an X, one slightly larger than the other, dark blades with thin gold edge lines, red tassels on both hilts.` |
| 20 | Truy Hồn Cung | 1x3 | `truy_hon_cung.png` | `Icon (epic, 1024x1536 portrait, bow standing vertically, slender): a recurve bow of dark lacquered wood with ochre gold tips and a single black-feathered arrow nocked, vermilion string wrap at the grip.` |
