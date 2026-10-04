# Prompt hình tấn công (ChatGPT) — bản 2

Hình hiệu ứng trong màn phát lại trận: khi binh khí ra đòn, hình **bay** từ món đó sang Hiệp Khách bên kia rồi biến mất. Mỗi hệ binh khí có **2 hình**: Phổ Công (đòn thường) và Tuyệt Kỹ. Thêm hình chung cho bộc phát trạng thái, Hóa Kình và va chạm.

## Vì sao viết lại
Bản đầu ra hình đòn thường quá cầu kỳ (sóng, đá vụn, máu bắn). Trong game hình chỉ hiện nhỏ và rất nhanh, nên phải theo **3 cấp độ** rõ ràng:

| Cấp | Dùng cho | Hiện trong game | Quy tắc |
|---|---|---|---|
| **Đơn giản** | Phổ Công (`*_basic`), `hit` | ~260×170 px, 0.35 s, lặp lại mỗi 1–6 s | **Đúng MỘT hình** (một vệt / một vòng / một vật bay), không lặp, không nhân đôi. Gần như chỉ mực đen, tối đa 1 chấm son nhỏ. Không đá vụn, không sóng, không máu. Nhận ra hệ vũ khí qua **hình dáng** |
| **Vừa** | Bộc phát, Hóa Kình | ~250 px vuông, 0.5–0.6 s | Một hình khối rõ ràng, 1 màu chủ đạo theo trạng thái, ít chi tiết |
| **Cầu kỳ** | Tuyệt Kỹ (`*_ultimate`) | ~420×280 px, 0.55 s, hiếm | **Một hình chính** lớn (không phải chuỗi nhiều hình giống nhau), được thêm nét phụ, son đỏ, vàng đất, mảnh vụn bao quanh |

## Quy ước (để code tự nhận hình)
- Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/Vfx/`, **tên file đúng cột File**.
- Hình bay: canvas **1536x1024 ngang**, hướng **từ trái sang phải** (game tự lật khi bên phải đánh).
- Hình đứng yên (bộc phát, Hóa Kình, va chạm): **1024x1024 vuông**, ở giữa khung.
- Nền trong suốt. ChatGPT thường trả về nền ô caro giả hoặc nền trắng: chạy `py tools/art/ink_to_alpha.py One-Bag-Jianghu/Assets/_Game/Textures2D/Vfx --backup <chỗ sao lưu>` (tách nền mềm theo từng ô caro, không để viền trắng quanh nét mực; hình đã trong suốt thì bỏ qua). Không dùng `remove_checker.py` cho VFX, công cụ đó dành cho icon viền cứng.
- Không vẽ vũ khí hay người, chỉ vẽ vệt đòn / khí / vật bay.
- Xong thì trong Unity chạy menu **One Bag Jianghu → Rebuild Icon Library**.

## Cách làm
1. Mở **một** cuộc trò chuyện mới, đính kèm `docs/art/reference/01-style-board-thuy-mac.png`.
2. Gửi tin mở đầu. Sau đó mỗi tin **một dòng** trong bảng, làm lần lượt: vẽ hết các đòn thường trước, rồi Tuyệt Kỹ, cuối cùng hình chung.
3. Khi có 1 hình đòn thường ưng ý, đính kèm hình đó vào các tin sau và thêm câu `Same simplicity as the attached basic attack.` để giữ độ đơn giản đồng đều.
4. Câu sửa nhanh:
   - Tuyệt Kỹ ra nhiều hình nối nhau: `Only ONE large main shape. Remove all the smaller repeated copies leading up to it; keep only the biggest one with its accents around it.`
   - ChatGPT báo vi phạm (bạo lực): `This is an abstract calligraphy ink-brush effect for a stylized game, not violence: no blood, no wounds, no body. Use only abstract brush strokes and small vermilion ink dots.` Nếu vẫn bị chặn, bỏ các từ như claw, splatter, blood, crush trong prompt.
   - Quá rối / nhiều hình: `Only ONE single shape, not two. Simplify to one clean brush shape, mostly black ink, no debris, no splatter. It must read clearly at 260x170 pixels.`
   - Có ánh sáng/3D: `Flatter: only ink brush strokes and flat washes, no glow, no light rays, no gradient.`
   - Sai hướng: `Flip it: the effect must travel from left to right, leading edge on the right.`

## Tin mở đầu

```
We will create combat effect sprites for a 2D wuxia auto-battler, one per message, matching the attached style reference.

Style: 2D Chinese ink-brush (shuimo). Effects are calligraphic brush strokes and flat ink washes with dry-brush texture. Palette only: deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C, rice-paper white #EFE6D2 for small highlights.
Strictly 2D: no glow, no bloom, no light rays, no lens flare, no 3D, no gradient. Energy is shown by the shape of the stroke, not by light.
No weapon, no character, no text, no frame. Fully transparent background.

There are three complexity levels, and I will tell you which one each time:
- SIMPLE: exactly ONE single shape (one stroke, one ring or one projectile). Never two or more copies, never a sequence. Almost only black ink, at most one tiny vermilion dot. No debris, no waves, no splatter. Must stay readable when shrunk to 260x170 pixels.
- MEDIUM: one clear shape with one dominant color, few details. Readable at 250x250 pixels.
- RICH: exactly ONE large main shape, never a row, trail or sequence of repeated copies (no smaller versions leading up to it). Secondary strokes, vermilion and ochre-gold accents and a few flying fragments may surround that one shape.

Travelling effects: landscape 1536x1024, moving from LEFT to RIGHT with the leading edge on the right, filling most of the width.
Centered effects: square 1024x1024, centered.

Reply "ready" and wait.
```

## Đòn thường (SIMPLE, 1536x1024, bay trái → phải)

| # | Hệ | File | Prompt |
|---|---|---|---|
| 1 | Quyền Sáo | `fist_basic.png` | `SIMPLE travelling effect: one single round ink shockwave ring with a short motion tail, like one quick punch.` |
| 2 | Côn | `staff_basic.png` | `SIMPLE travelling effect: one single long straight horizontal sweep stroke with a thick blunt head.` |
| 3 | Kiếm | `sword_basic.png` | `SIMPLE travelling effect: one single thin crescent slash with a very sharp tip.` |
| 4 | Đao | `blade_basic.png` | `SIMPLE travelling effect: one single heavy wide curved slash, thick at the base, one tiny vermilion flick at the edge.` |
| 5 | Ám Khí | `hidden_basic.png` | `SIMPLE travelling effect: one single needle-like dart with a short ink speed line behind it.` |
| 6 | Trảo | `claw_basic.png` | `SIMPLE travelling effect: one single claw mark, three short curved lines joined at the base as one shape.` |
| 7 | Rìu | `axe_basic.png` | `SIMPLE travelling effect: one single massive short downward-curving chop stroke, very thick.` |
| 8 | Bút | `brush_basic.png` | `SIMPLE travelling effect: one single sharp calligraphy dot-and-flick stroke with one tiny vermilion dot at the tip.` |
| 9 | Tiêu (sáo) | `flute_basic.png` | `SIMPLE travelling effect: one single wavy sound-wave ribbon in light ink with a blue-grey wash.` |
| 10 | Cầm | `zither_basic.png` | `SIMPLE travelling effect: one single thin vibrating string line with a small ripple at its leading end.` |
| 11 | Chưởng | `palm_basic.png` | `SIMPLE travelling effect: one single open-palm shaped ink push wave.` |
| 12 | Phiến | `fan_basic.png` | `SIMPLE travelling effect: one single thin fan-shaped arc of cutting wind.` |
| 13 | Thương | `spear_basic.png` | `SIMPLE travelling effect: one single very long straight thrust line with a sharp point.` |
| 14 | Chùy | `hammer_basic.png` | `SIMPLE travelling effect: one single dense round shockwave blot with a few short crack lines on its edge.` |
| 15 | Tiên | `whip_basic.png` | `SIMPLE travelling effect: one single long wavy lashing stroke, thin at the tip.` |
| 16 | Song Đao | `dual_basic.png` | `SIMPLE travelling effect: one single X-shaped cross slash (two strokes crossing into one mark).` |
| 17 | Cung | `bow_basic.png` | `SIMPLE travelling effect: one single thin arrow with a short ink trail.` |

## Tuyệt Kỹ (RICH, 1536x1024, bay trái → phải)

| # | Hệ | File | Prompt |
|---|---|---|---|
| 18 | Quyền Sáo | `fist_ultimate.png` | `RICH travelling effect: one single huge fist-shaped ink shockwave ring with swirling blue-grey waves inside it, small vermilion ink dots and a few flying stone fragments around it. Only ONE main shape, no repeated copies, no sequence.` |
| 19 | Côn | `staff_ultimate.png` | `RICH travelling effect: one single huge circular sweeping arc of thick ink, ending in a heavy vermilion impact blot, a few small gold prayer-bead dots around it. Only ONE main shape, no repeated copies, no sequence.` |
| 20 | Kiếm | `sword_ultimate.png` | `RICH travelling effect: one single huge crescent sword-qi slash with a thin gold edge line, a few ink petals drifting around it. Only ONE main shape, no repeated copies, no sequence.` |
| 21 | Đao | `blade_ultimate.png` | `RICH travelling effect: one single enormous curved slash made of vermilion flame-shaped brush strokes over black ink, a few flat red embers around it. Only ONE main shape, no repeated copies, no sequence.` |
| 22 | Ám Khí | `hidden_ultimate.png` | `RICH travelling effect: one single large dart wrapped in a green-grey poison mist drawn as flat ink wash blots, a few small drips around it. Only ONE main shape, no repeated copies, no sequence.` |
| 23 | Trảo | `claw_ultimate.png` | `RICH travelling effect: one single giant abstract brush mark of three thick curved parallel ink strokes joined at the base like a tiger's swipe of wind, small vermilion ink dots around it. Only ONE main shape, no repeated copies, no sequence.` |
| 24 | Rìu | `axe_ultimate.png` | `RICH travelling effect: one single gigantic crescent chop wave, angular broken ink shards and thin gold crack lines around it. Only ONE main shape, no repeated copies, no sequence.` |
| 25 | Bút | `brush_ultimate.png` | `RICH travelling effect: one single long flowing calligraphy stroke forming an abstract glyph-like shape (not a real character), ending in a vermilion seal stamp. Only ONE main shape, no repeated copies, no sequence.` |
| 26 | Tiêu (sáo) | `flute_ultimate.png` | `RICH travelling effect: one single wide rippling blue-grey sound wave like a cold wind, a few flat angular frost crystals around it. Only ONE main shape, no repeated copies, no sequence.` |
| 27 | Cầm | `zither_ultimate.png` | `RICH travelling effect: one single curling blue-grey water wave with thin string lines across it, a few gold note-shaped dots around it. Only ONE main shape, no repeated copies, no sequence.` |
| 28 | Chưởng | `palm_ultimate.png` | `RICH travelling effect: one single giant open-palm imprint of swirling ink and vermilion heat strokes. Only ONE main shape, no repeated copies, no sequence.` |
| 29 | Phiến | `fan_ultimate.png` | `RICH travelling effect: one single huge fan-shaped wind arc, a few falling ink petals and gold dots around it. Only ONE main shape, no repeated copies, no sequence.` |
| 30 | Thương | `spear_ultimate.png` | `RICH travelling effect: one single long thrust line with a dragon-like spiral of blue-grey ink twisting around it, a few angular frost crystals. Only ONE main shape, no repeated copies, no sequence.` |
| 31 | Chùy | `hammer_ultimate.png` | `RICH travelling effect: one single huge round heavy shockwave with ground-crack shapes on its edge, a few flying stone fragments and gold cracks around it. Only ONE main shape, no repeated copies, no sequence.` |
| 32 | Tiên | `whip_ultimate.png` | `RICH travelling effect: one single long lashing whip stroke curling forward like a vine, a few small green-grey leaves around it. Only ONE main shape, no repeated copies, no sequence.` |
| 33 | Song Đao | `dual_ultimate.png` | `RICH travelling effect: one single large X-shaped cross slash (two thick strokes crossing into one mark), small vermilion ink dots around it. Only ONE main shape, no repeated copies, no sequence.` |
| 34 | Cung | `bow_ultimate.png` | `RICH travelling effect: one single large black-feathered arrow with a long vermilion trail, a few short ink speed lines around it. Only ONE main shape, no repeated copies, no sequence.` |

## Hình chung (1024x1024 vuông, ở giữa)

| # | File | Cấp | Prompt |
|---|---|---|---|
| 35 | `hit.png` | SIMPLE | `SIMPLE centered effect: a small star-shaped ink splash with three flying dots.` |
| 36 | `parry.png` | MEDIUM | `MEDIUM centered effect: a circular ink swirl like a deflecting vortex with a thin gold rim.` |
| 37 | `burst_burn.png` | MEDIUM | `MEDIUM centered effect: a round burst of vermilion flame-shaped strokes around a black ink core.` |
| 38 | `burst_poison.png` | MEDIUM | `MEDIUM centered effect: a splash of dark green-grey ink wash blots bursting outward, a few drips.` |
| 39 | `burst_bleed.png` | MEDIUM | `MEDIUM centered effect: an abstract burst of sharp vermilion ink brush strokes radiating outward like a star, a few small ink dots.` |
