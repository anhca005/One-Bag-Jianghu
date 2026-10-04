# Chọn phong cách hình ảnh

> **Đã chốt: Thủy Mặc** (quyết định #11). Prompt dùng cho sản xuất ở [art-pipeline.md](art-pipeline.md). File này giữ lại để tham khảo.

Game là **2D phẳng**. Lần thử đầu (3 phong cách Thủy Mặc, Công Bút, Bán Thực) ra hình trông **giống 3D**: đổ bóng mềm có khối, kim loại phản sáng như thật, ánh sáng viền và hào quang, hậu cảnh có chiều sâu. Vì vậy mọi prompt dưới đây đều kèm **khối "bắt buộc 2D"**, và phong cách Bán Thực được thay bằng bản 2D cel-shade.

Mỗi phong cách có:
- **Bảng màu chính** (6 màu) để mọi hình ảnh sau này đồng nhất.
- **Bảng màu độ hiếm** (6 màu khung: Phàm → Chí Tôn), giữ đúng ý nghĩa màu trong [glossary](../design/glossary.md).
- **Prompt thử phong cách:** 1 tấm "style board" gồm nhân vật, 4 icon vật phẩm, khung túi đồ, dải màu.
- **Khóa phong cách:** dán vào đầu *mọi* prompt sau này khi đã chọn phong cách.

## Cách dùng

1. Mỗi phong cách mở **một cuộc trò chuyện ChatGPT mới**, dán prompt thử phong cách. Nếu hình vẫn "có khối", trả lời thêm câu [sửa nhanh](#sửa-nhanh-khi-hình-còn-giống-3d).
2. Chọn 1–2 phong cách, dùng [prompt màn hình trận](#prompt-chung-màn-hình-trận) để xem khi thành game.
3. Chốt 1 phong cách, ghi vào `decisions.md`. Từ đó mọi prompt đều bắt đầu bằng **khóa phong cách** + **khối bắt buộc 2D**.

Lưu ý: ChatGPT không luôn khớp mã màu hay số ô lưới. Mã hex để định hướng; lưới túi đồ và khung độ hiếm sẽ do Unity dựng. Prompt yêu cầu **không có chữ** vì AI hay vẽ chữ sai.

## Khối bắt buộc 2D (đã có sẵn trong mọi prompt dưới đây)

```
STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.
```

### Sửa nhanh khi hình còn giống 3D
Nếu ChatGPT vẫn vẽ có khối, trả lời tiếp trong cùng cuộc trò chuyện:
```
Redo it flatter: remove all soft shading, gradients, reflections and glow. Use flat color fills, thick clean outlines and hard 2-tone cel shadows only, like a 2D mobile card game.
```

---

## 1. Thủy Mặc Phẳng

Nét cọ mực, mảng màu loang phẳng trên giấy dó, nhiều khoảng trống, son đỏ điểm nhấn. Lần thử đầu: nhân vật đạt, icon vật phẩm quá thật → bản này ép icon phẳng.

| Vai trò | Màu |
|---|---|
| Nền giấy | `#EFE6D2` |
| Mực đậm (viền) | `#1E1B18` |
| Mực nhạt (bóng) | `#6B6760` |
| Xám xanh núi | `#8A9A94` |
| Son đỏ (nhấn) | `#B8322A` |
| Vàng đất | `#C9A45C` |

Độ hiếm: Phàm `#9A958C` · Lương `#6F8F5E` · Tinh `#4F7A99` · Tuyệt `#7C5A8E` · Thần Binh `#C9A045` · Chí Tôn `#B8322A`

**Khóa phong cách:**
```
Style: flat 2D Chinese ink-brush illustration. Bold black brush-stroke outlines, flat ink washes in 2 tones, textured rice paper background, generous negative space, vermilion accents. Palette: rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: flat 2D Chinese ink-brush illustration. Bold black brush-stroke outlines, flat ink washes in 2 tones, textured rice paper background, generous negative space, vermilion accents.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.

PALETTE (use only these colors and their tints/shades): rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
Rarity frame colors: common #9A958C, uncommon #6F8F5E, rare #4F7A99, epic #7C5A8E, legendary #C9A045, supreme #B8322A.

LAYOUT on a plain rice-paper background, every element clearly separated:
1. Left third: full-body hero, a young wandering swordsman in flowing robes holding a straight jian sword, calm confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 2. Công Bút Phẳng

Nét mảnh đều, tô màu khoáng phẳng từng mảng (chu sa, thạch lục, thạch thanh), điểm vàng. Lần thử đầu: gần 2D nhất trong 3 bản, nhưng vật phẩm còn bóng bẩy.

| Vai trò | Màu |
|---|---|
| Nền lụa | `#F3E9D2` |
| Mực nét | `#2B2320` |
| Chu sa | `#C0392B` |
| Thạch lục | `#3A8C6E` |
| Thạch thanh | `#2E5E8C` |
| Vàng | `#D4AF37` |

Độ hiếm: Phàm `#A8A29A` · Lương `#3A8C6E` · Tinh `#2E5E8C` · Tuyệt `#7B4B94` · Thần Binh `#D4AF37` · Chí Tôn `#C0392B`

**Khóa phong cách:**
```
Style: flat 2D Chinese gongbi-inspired illustration. Thin even ink outlines, flat mineral color fills (cinnabar, malachite green, azurite blue) with no blending, flat gold pattern details, decorative flat cloud motifs, silk-textured flat background. Palette: silk #F3E9D2, ink line #2B2320, cinnabar #C0392B, malachite #3A8C6E, azurite #2E5E8C, gold #D4AF37.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: flat 2D Chinese gongbi-inspired illustration. Thin even ink outlines, flat mineral color fills (cinnabar, malachite green, azurite blue) with no blending, flat gold pattern details, decorative flat cloud motifs, silk-textured flat background.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.

PALETTE (use only these colors and their tints/shades): silk #F3E9D2, ink line #2B2320, cinnabar #C0392B, malachite #3A8C6E, azurite #2E5E8C, gold #D4AF37.
Rarity frame colors: common #A8A29A, uncommon #3A8C6E, rare #2E5E8C, epic #7B4B94, legendary #D4AF37, supreme #C0392B.

LAYOUT on a plain silk background, every element clearly separated:
1. Left third: full-body hero, a young wandering swordsman in flowing robes holding a straight jian sword, calm confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 3. Game 2D Cel-shade (thay cho Bán Thực)

Giữ không khí gỗ tối + vàng kim của bản Bán Thực, nhưng vẽ theo kiểu game thẻ bài 2D: viền đậm, bóng 2 tầng cắt gọn, không phát sáng.

| Vai trò | Màu |
|---|---|
| Nền gỗ tối | `#2A1D15` |
| Nâu da | `#5A3E2B` |
| Giấy da | `#E8D5B0` |
| Vàng kim | `#D9A441` |
| Đồng | `#A8672F` |
| Ngọc bích | `#2F7D6B` |

Độ hiếm: Phàm `#8C8C8C` · Lương `#4E9A4A` · Tinh `#3D7BC2` · Tuyệt `#8A4FC7` · Thần Binh `#E0A526` · Chí Tôn `#C8322B`

**Khóa phong cách:**
```
Style: flat 2D cel-shaded game card art. Thick dark-brown outlines, flat color fills with one hard-edged shadow tone, simple flat gold ornament shapes, warm dark wood panels drawn as flat graphic planks. Palette: dark wood #2A1D15, leather brown #5A3E2B, parchment #E8D5B0, gold #D9A441, bronze #A8672F, jade #2F7D6B.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: flat 2D cel-shaded game card art. Thick dark-brown outlines, flat color fills with one hard-edged shadow tone, simple flat gold ornament shapes, warm dark wood panels drawn as flat graphic planks.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.

PALETTE (use only these colors and their tints/shades): dark wood #2A1D15, leather brown #5A3E2B, parchment #E8D5B0, gold #D9A441, bronze #A8672F, jade #2F7D6B.
Rarity frame colors: common #8C8C8C, uncommon #4E9A4A, rare #3D7BC2, epic #8A4FC7, legendary #E0A526, supreme #C8322B.

LAYOUT on a plain flat dark wood background, every element clearly separated:
1. Left third: full-body hero, a young wandering swordsman in flowing robes holding a straight jian sword, calm confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 4. Chibi Giang Hồ

Nhân vật đầu to, nét tròn, màu tươi, viền nâu mềm. Thân thiện, dễ tiếp cận; ít chất kiếm hiệp nghiêm túc.

| Vai trò | Màu |
|---|---|
| Nền kem | `#FFF4E0` |
| Viền nâu | `#5B3A29` |
| Cam quýt | `#FF9F43` |
| Đỏ son | `#E8505B` |
| Xanh ngọc | `#2EC4B6` |
| Vàng nắng | `#FFD166` |

Độ hiếm: Phàm `#B8B8B8` · Lương `#7BC67E` · Tinh `#5DADE2` · Tuyệt `#B388EB` · Thần Binh `#FFC93C` · Chí Tôn `#FF5E5B`

**Khóa phong cách:**
```
Style: flat 2D chibi cartoon. Big heads and small bodies, rounded chunky shapes, thick soft-brown outlines, flat bright color fills with one hard shadow tone, toy-like props. Palette: cream #FFF4E0, soft brown outline #5B3A29, tangerine #FF9F43, rouge red #E8505B, turquoise #2EC4B6, sunny yellow #FFD166.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: flat 2D chibi cartoon. Big heads and small bodies, rounded chunky shapes, thick soft-brown outlines, flat bright color fills with one hard shadow tone, toy-like props.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.

PALETTE (use only these colors and their tints/shades): cream #FFF4E0, soft brown outline #5B3A29, tangerine #FF9F43, rouge red #E8505B, turquoise #2EC4B6, sunny yellow #FFD166.
Rarity frame colors: common #B8B8B8, uncommon #7BC67E, rare #5DADE2, epic #B388EB, legendary #FFC93C, supreme #FF5E5B.

LAYOUT on a plain cream background, every element clearly separated:
1. Left third: full-body chibi hero, a young wandering swordsman in robes holding a straight jian sword, cheerful confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 5. Anime Kiếm Hiệp Phẳng

Nét viền đậm, đổ bóng phẳng 2 tầng, tương phản mạnh, kiếm khí vẽ thành mảng hình học phẳng. Năng động, dễ giữ đồng nhất.

| Vai trò | Màu |
|---|---|
| Chàm đêm | `#1B2340` |
| Trắng ngà | `#F5F1E8` |
| Đỏ thẫm | `#C1272D` |
| Lam kiếm khí | `#3FA7D6` |
| Vàng | `#F2C14E` |
| Tím mộng | `#6C4AB6` |

Độ hiếm: Phàm `#9EA3AD` · Lương `#4CAF6A` · Tinh `#3FA7D6` · Tuyệt `#8E5BD6` · Thần Binh `#F2C14E` · Chí Tôn `#E0303A`

**Khóa phong cách:**
```
Style: flat 2D anime wuxia illustration. Bold clean outlines, two-tone cel shading with hard edges, high contrast, sword-qi effects drawn as flat sharp graphic shapes, flat geometric background shapes. Palette: night indigo #1B2340, ivory #F5F1E8, crimson #C1272D, sword-qi blue #3FA7D6, gold #F2C14E, dream violet #6C4AB6.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: flat 2D anime wuxia illustration. Bold clean outlines, two-tone cel shading with hard edges, high contrast, sword-qi effects drawn as flat sharp graphic shapes, flat geometric background shapes.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Clear consistent outlines around every shape.
- Flat color fills; at most 2 shade tones per color with hard-edged cel shadows. No soft gradients, no airbrush.
- Metal, jade and wood are drawn as flat graphic shapes with at most one simple flat highlight shape. No realistic reflections, no specular glare, no glossy shine.
- No glow, no bloom, no rim light, no volumetric light, no lens effects, no depth of field, no atmospheric haze.
- Background is flat or made of simple flat graphic shapes, no perspective depth.
- Item icons: simple readable silhouettes seen from a flat side or front view, like 2D game sticker icons.

PALETTE (use only these colors and their tints/shades): night indigo #1B2340, ivory #F5F1E8, crimson #C1272D, sword-qi blue #3FA7D6, gold #F2C14E, dream violet #6C4AB6.
Rarity frame colors: common #9EA3AD, uncommon #4CAF6A, rare #3FA7D6, epic #8E5BD6, legendary #F2C14E, supreme #E0303A.

LAYOUT on a plain night indigo background, every element clearly separated:
1. Left third: full-body hero, a young wandering swordsman in flowing robes holding a straight jian sword, dynamic confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 6. Pixel Art Giang Hồ

Pixel art 32-bit, bảng màu giới hạn, không khử răng cưa. Dễ đồng nhất nhất, file nhẹ. Nhập vào Unity với chế độ lọc Point.

| Vai trò | Màu |
|---|---|
| Nền tối | `#1A1423` |
| Nâu đậm | `#3B2C35` |
| Nâu gỗ | `#7A4E3B` |
| Đồng nhạt | `#C9A26B` |
| Giấy sáng | `#F4E3C1` |
| Rêu ngọc | `#3E7C5A` |

Độ hiếm: Phàm `#8B8B8B` · Lương `#4FA85B` · Tinh `#3B82C4` · Tuyệt `#9152C4` · Thần Binh `#E7B437` · Chí Tôn `#D23B3B`

**Khóa phong cách:**
```
Style: 2D 32-bit pixel art. Crisp hard pixel edges, no anti-aliasing, no blur, limited palette, clean 1-pixel dark outlines, simple 2-tone pixel shading, readable silhouettes. Palette: deep night #1A1423, dark umber #3B2C35, wood brown #7A4E3B, tan bronze #C9A26B, parchment #F4E3C1, jade moss #3E7C5A.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: 2D 32-bit pixel art. Crisp hard pixel edges, no anti-aliasing, no blur, limited palette, clean 1-pixel dark outlines, simple 2-tone pixel shading, readable silhouettes.

STRICTLY FLAT 2D PIXEL ART. Not 3D, not a 3D render, not voxel, not photorealistic. No gradients, no glow, no reflections, no depth of field.

PALETTE (use only these colors and their tints/shades): deep night #1A1423, dark umber #3B2C35, wood brown #7A4E3B, tan bronze #C9A26B, parchment #F4E3C1, jade moss #3E7C5A.
Rarity frame colors: common #8B8B8B, uncommon #4FA85B, rare #3B82C4, epic #9152C4, legendary #E7B437, supreme #D23B3B.

LAYOUT on a plain deep night background, every element clearly separated:
1. Left third: full-body pixel art hero, a young wandering swordsman in robes holding a straight jian sword, confident stance.
2. Top right: four pixel art item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a pixel art inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## 7. Tiễn Chỉ (cắt giấy) — mới

Lấy cảm hứng từ nghệ thuật cắt giấy Trung Hoa: các lớp giấy màu phẳng xếp chồng, viền cắt sắc, hoa văn đục lỗ. **Phẳng tự nhiên**, rất khác biệt, icon rõ ràng. Hạn chế: khó thể hiện chi tiết nhỏ.

| Vai trò | Màu |
|---|---|
| Nền giấy kem | `#F2E8D5` |
| Đỏ giấy cắt | `#B3261E` |
| Đen mực | `#1F1A17` |
| Xanh ngọc thạch | `#2E7D6F` |
| Lam chàm | `#34506E` |
| Vàng nghệ | `#E0A43A` |

Độ hiếm: Phàm `#9C958A` · Lương `#5E8F55` · Tinh `#34506E` · Tuyệt `#7A4D8C` · Thần Binh `#E0A43A` · Chí Tôn `#B3261E`

**Khóa phong cách:**
```
Style: 2D Chinese paper-cut art. Layered flat colored paper shapes with crisp cut edges, decorative cut-out patterns (clouds, waves, lattice), a very subtle flat drop shadow between paper layers, no painting texture. Palette: cream paper #F2E8D5, paper-cut red #B3261E, ink black #1F1A17, jade #2E7D6F, indigo #34506E, turmeric gold #E0A43A.
```

**Prompt thử phong cách:**
```
Create a 16:9 style exploration board for a 2D wuxia (Chinese martial arts fantasy) backpack auto-battler game.

STYLE: 2D Chinese paper-cut art. Layered flat colored paper shapes with crisp cut edges, decorative cut-out patterns (clouds, waves, lattice), a very subtle flat drop shadow between paper layers, no painting texture.

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic.
- Every shape is a flat piece of colored paper with crisp edges.
- No gradients, no glossy reflections, no glow, no depth of field.
- Item icons: simple readable silhouettes seen from a flat side or front view.

PALETTE (use only these colors): cream paper #F2E8D5, paper-cut red #B3261E, ink black #1F1A17, jade #2E7D6F, indigo #34506E, turmeric gold #E0A43A.
Rarity frame colors: common #9C958A, uncommon #5E8F55, rare #34506E, epic #7A4D8C, legendary #E0A43A, supreme #B3261E.

LAYOUT on a plain cream paper background, every element clearly separated:
1. Left third: full-body hero, a young wandering swordsman in flowing robes holding a straight jian sword, confident stance.
2. Top right: four item icons in a 2x2 grid, each inside its own square frame whose border uses a rarity color: a two-handed battle axe (rare), a straight jian sword (rare), a round jade elixir pill on a small dish (uncommon), a calabash wine gourd with a red tassel (epic).
3. Bottom right: a game inventory panel with exactly 5 columns and 4 rows of empty square slots.
4. Bottom strip: six plain color swatches of the main palette.

No text, no letters, no numbers, no watermark, no logo.
```

---

## Prompt chung: màn hình trận

Dùng sau khi đã lọc còn 1–2 phong cách. Thay `[KHÓA PHONG CÁCH]` bằng đoạn khóa của phong cách đó; khối bắt buộc 2D đã có sẵn.

```
[KHÓA PHONG CÁCH]

STRICTLY FLAT 2D ILLUSTRATION. Not 3D, not a 3D render, not CGI, not photorealistic. Clear outlines, flat color fills with at most 2 hard-edged shade tones, no gradients, no reflections, no glow, no depth of field, flat background.

Create a 16:9 mock-up of the battle screen of a 2D wuxia backpack auto-battler game, in exactly the style and palette above.

Composition:
- Top: two health bars facing each other, one per fighter, with a small shield icon and a thin qi (inner energy) bar under each.
- Left edge: the player's hero portrait in a tall ornate frame. Right edge: the enemy portrait (a masked demonic swordsman) in a matching frame.
- Center: two backpack grids side by side, each exactly 5 columns x 4 rows, separated by a decorative vertical divider. Each grid holds items of different sizes: a long spear over 1x4 cells, a sword over 1x3, an axe over 2x2, a pair of claws over 1x2, small pills and trinkets over 1x1. Items sit inside rarity-colored frames.
- A few items are highlighted as if activating now; a flat graphic burst of fire energy flies from the left grid toward the right.
- Bottom right: three small round buttons (speed, replay, log) without text.

No text, no letters, no numbers, no watermark, no logo.
```

## Kết quả thử

| Lần | Phong cách | Nhận xét |
|---|---|---|
| 1 | Thủy Mặc, Công Bút, Bán Thực (bản cũ, chưa có khối 2D) | Cả 3 trông giống 3D: đổ bóng mềm, kim loại phản sáng, hào quang, hậu cảnh có chiều sâu. Thủy Mặc: nhân vật đạt, icon quá thật. Công Bút: gần 2D nhất. Bán Thực: nặng 3D nhất → thay bằng Cel-shade 2D. ChatGPT vẽ sai số ô lưới (6x3, 6x4) |
| 2 | Thủy Mặc Phẳng (có khối 2D) và vài phong cách khác | Thủy Mặc đạt: nét cọ, giấy dó, icon rõ, đúng lưới 5x4. **Chốt** → [reference/01-style-board-thuy-mac.png](reference/01-style-board-thuy-mac.png) |

## Sau khi chốt

- Ghi phong cách đã chọn vào `decisions.md`.
- Cập nhật "Prompt gốc" trong [art-pipeline.md](art-pipeline.md) bằng khóa phong cách + khối bắt buộc 2D.
- Lưu 5 hình ưng nhất làm **bảng mẫu tham chiếu**, đính kèm mỗi lần tạo hình mới.
