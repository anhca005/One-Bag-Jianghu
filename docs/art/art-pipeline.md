# Quy trình làm hình (ChatGPT)

**Phong cách đã chốt: Thủy Mặc** (quyết định #11). Hình gốc: [reference/01-style-board-thuy-mac.png](reference/01-style-board-thuy-mac.png).

## Nguyên tắc
1. **ChatGPT chỉ vẽ vật thể trên nền trong suốt.** Khung, màu độ hiếm, dấu Trọng do Unity ghép lên để đồng bộ.
2. **Mọi prompt bắt đầu bằng khóa phong cách** dưới đây, chỉ thay phần mô tả.
3. **Luôn đính kèm hình tham chiếu** trong `reference/` (tối đa 3–5 hình) và ghi "match the attached reference style exactly".
4. Mỗi nhóm hình (icon, nhân vật, nền…) làm trong **một cuộc trò chuyện riêng** để ChatGPT giữ nét đồng đều. Khi hình bắt đầu lệch thì mở cuộc mới.
5. Hình nào ưng ý thì thêm vào `reference/` (đánh số tiếp: `02-...png`), để bộ tham chiếu có đủ icon, nhân vật và nền.

## Bảng màu

| Vai trò | Màu |
|---|---|
| Nền giấy dó | `#EFE6D2` |
| Mực đậm (viền) | `#1E1B18` |
| Mực nhạt (bóng) | `#6B6760` |
| Xám xanh núi | `#8A9A94` |
| Son đỏ (nhấn) | `#B8322A` |
| Vàng đất | `#C9A45C` |

Màu khung độ hiếm (Unity dùng): Phàm `#9A958C` · Lương `#6F8F5E` · Tinh `#4F7A99` · Tuyệt `#7C5A8E` · Thần Binh `#C9A045` · Chí Tôn `#B8322A`.

## Khóa phong cách

Viết lại theo hình đã chốt: hình có vệt mực và nét cọ khô, không phẳng tuyệt đối như bản thử. Vẫn cấm các dấu hiệu 3D.

```
Match the attached reference style exactly.
Style: 2D Chinese ink-brush (shuimo) illustration with a clean anime-influenced wuxia look. Confident black brush-stroke outlines of varying thickness, flat ink washes in 2-3 tones with visible dry-brush texture, generous negative space, vermilion red accents on ribbons, tassels and seals.
Palette only: rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
Strictly 2D: not a 3D render, no CGI, no glossy or realistic metal reflections, no glow, no bloom, no rim light, no lens effects, no depth of field. Shading only with flat ink washes and brush strokes.
No text, no letters, no numbers, no watermark, no logo.
```

## Prompt mẫu theo nhóm

Dán **khóa phong cách** lên đầu, rồi dán mẫu tương ứng và thay phần `[...]`.

### Icon vật phẩm
```
Single game item icon: [MÔ TẢ VẬT THỂ BẰNG TIẾNG ANH].
Seen from a flat side view, standing vertically [blade/tip pointing up], centered, filling about 85% of the canvas height.
Fully transparent background: no paper texture, no scenery, no ground shadow, no frame.
Canvas: [1024x1536 portrait | 1024x1024 square].
```
Chọn khung theo kích thước món:

| Kích thước ô | Canvas | Cắt sau khi tải về |
|---|---|---|
| 1x1, 2x2 | 1024x1024 | giữ nguyên |
| 1x2 | 1024x1536 | cắt về tỉ lệ 1:2 (768x1536) |
| 1x3, 1x4 | 1024x1536 | cắt về 1:3 hoặc 1:4; vật thể phải thật mảnh và dài |

Ví dụ mô tả: `a straight jian sword with a dark lacquered scabbard wrapped around the hilt and a long vermilion tassel` · `a pair of iron tiger claws worn on the hand, three curved blades` · `a small round jade pill glowing faintly green, resting on a tiny ochre dish` (pill 1x1).

Gợi ý theo độ hiếm (không vẽ khung, chỉ đổi độ cầu kỳ):
- Phàm, Lương: đơn giản, ít hoa văn, màu mực chủ đạo.
- Tinh, Tuyệt: thêm hoa văn vàng đất, tua son đỏ.
- Thần Binh, Chí Tôn: hoa văn cầu kỳ, nhiều vàng đất, có vệt khí mực bay quanh (vẫn phẳng, không phát sáng).

### Hiệp Khách (toàn thân)
```
Full-body character art of [MÔ TẢ: e.g. a stern Buddhist monk warrior with a long iron staff, bare right shoulder, prayer beads].
Dynamic but readable standing pose, facing slightly to the right, whole body visible from head to feet.
Fully transparent background, no ground, no scenery.
Canvas: 1024x1536 portrait.
```

### Boss
Giống mẫu Hiệp Khách, thêm: `imposing silhouette, facing slightly to the left, [chi tiết đặc trưng: mask, scars, demonic ink smoke drawn as flat brush strokes]`.

### Nền màn hình
```
Background painting for a game screen: [CẢNH: e.g. misty mountain pass with pine trees and a vermilion sun].
Composition: scenery only along the left and right edges and the top; the large central area is plain rice paper with very faint ink, so UI panels can sit on top. Low contrast, calm.
No characters. Canvas: 1536x1024 landscape.
```

### Icon trạng thái
```
Small status icon: [BIỂU TƯỢNG: e.g. a single flame for burning / a dripping green drop for poison / a red blood splash for bleeding].
Drawn as a bold brush-stroke symbol inside a round vermilion seal-stamp shape, readable at 32x32 pixels, very simple, thick strokes.
Fully transparent background. Canvas: 1024x1024.
```

### Biểu tượng môn phái
```
Emblem for a martial arts sect called [TÊN TIẾNG ANH: e.g. Iron Axe Guild], symbol: [BIỂU TƯỢNG: e.g. a crossed pair of axes over a mountain].
Style of a square red Chinese seal stamp (chop) with a carved look, ink brush symbol, no readable characters.
Fully transparent background. Canvas: 1024x1024.
```

## Xử lý sau khi tải về
1. ChatGPT hay vẽ luôn ô caro xám trắng thay vì nền trong suốt thật. Chạy `py tools/art/remove_checker.py <thư mục hoặc file> --backup <thư mục sao lưu>`: tách nền từ mép ảnh vào, dừng ở viền mực; hình đã trong suốt thật thì bỏ qua.
2. Cắt theo tỉ lệ ở bảng trên, rồi thu nhỏ về **256 px mỗi ô** (1x3 → 256x768, 2x2 → 512x512).
3. Đặt tên theo `id` trong items.json: `thanh_phong_kiem.png`.
4. Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/<nhóm>/`: `Weapons/`, `Items/` (Hộ Cụ, Tâm Pháp, Đan Dược, Kỳ Trân), `Heroes/`, `Backgrounds/`, `Status/`, `Sects/`.

## Quy ước kích thước
- 1 ô lưới = **256 px**.
- Binh khí vẽ **dựng đứng** vì món 1xN trong túi nằm dọc (chưa hỗ trợ xoay).
- PNG nền trong suốt.

## Danh sách hình cho MVP

| Nhóm | Số lượng |
|---|---|
| Icon vật phẩm | ~50 |
| Biểu tượng môn phái | 7 |
| Hiệp Khách | 3 |
| Boss | 6 |
| Icon trạng thái | 12 |
| Nền màn hình | 5 |
| Bộ khung UI (khung độ hiếm, nút, panel) | 1 bộ |

## Lưu ý
Kiểm tra điều khoản sử dụng thương mại của OpenAI trước khi phát hành.
