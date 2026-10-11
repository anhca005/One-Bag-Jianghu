# Prompt hình Môn Phái và Tâm Pháp

Dùng cho giao diện Môn Phái (chọn phái, xem và sau này nâng Tâm Pháp). Lối vẽ chung với tranh cốt truyện: thủy mặc trên giấy dó cũ, tham chiếu `docs/art/reference/01-style-board-thuy-mac.png` và `Textures2D/Pve/intro_1.png`.

**Quy tắc chung (mọi hình):** không chữ, không số, không ấn triện có chữ, không chữ ký, không watermark. Không có nhân vật chính diện rõ mặt (hình là cảnh/vật biểu tượng). Màu nền giấy ngà, mực đen xám, một màu nhấn riêng của phái.

## A. Tranh sơn môn: `Textures2D/Sects/sect_<id>.png`, 1536x1024 (ngang)

Hiện lớn ở khung chi tiết, và cắt giữa làm thẻ nhỏ trong danh sách: chủ thể đặt **ở giữa**, hai mép trái/phải thoáng.

Khung prompt (thay phần trong ngoặc):
> Traditional Chinese ink-wash painting (shui-mo) on aged cream rice paper, wuxia sect headquarters: {cảnh}. Accent color: {màu}. Misty layered mountains, soft ink gradients, dry-brush texture, generous empty paper space, the main subject centered. No text, no letters, no seals with characters, no signature, no watermark.

| id | Phái | Cảnh | Màu nhấn |
|---|---|---|---|
| `kim_cang` | Kim Cang Tự | a stern Buddhist mountain monastery with heavy stone halls, a giant bronze bell and iron prayer staffs leaning by the gate, monks' training ground of packed earth | warm gold and saffron |
| `thai_thanh` | Thái Thanh Quán | a serene Taoist temple on a high cliff above a sea of clouds, a lone sword planted before the gate, cranes in flight, a folding fan resting on a stone table | pale jade green |
| `liet_diem` | Liệt Diễm Đường | a fierce hall at the foot of a smoking volcano, forges glowing, broad sabres racked under red banners, sparks and heat haze | vermilion red and ember orange |
| `huyen_doc` | Huyền Độc Môn | a hidden stilted lair in a misty swamp, twisted mangroves, poison flowers, darts and steel claws hung on bamboo racks, coiled snake silhouettes in the reeds | sickly violet and dark green |
| `han_son` | Hàn Sơn Phái | a dignified sect on a snowy peak, frozen pines, a long spear and a jade flute on an ice terrace, a bow and calligraphy brush beside an inkstone | icy pale blue |
| `thiet_phu` | Thiết Phủ Bang | a rugged gang fortress of timber and iron in a quarry canyon, giant axes and war hammers, chains and stone blocks, dust in the air | rust brown and iron grey |
| `bich_thuy` | Bích Thủy Am | a quiet nunnery by a clear green lake, a lotus pond, a guqin zither on a pavilion veranda, a silk whip-ribbon drifting in the breeze | teal and lotus pink |

## B. Hình Tâm Pháp: `Textures2D/Sects/tam_phap_<id>.png`, 1024x1024 (vuông)

Một cuốn bí kíp/cuộn trục đặt giữa khung, có biểu tượng của phái quanh nó. Hiện làm ô Tâm Pháp (khoảng 160–220 px), nên hình phải **đọc rõ khi nhỏ**: một vật chính, viền đậm, nền giấy trơn.

Khung prompt:
> Ink-wash illustration on plain aged cream rice paper: an ancient martial arts manual {vật}, centered, bold ink outline, readable as a small icon. Accent color: {màu}. Plain paper background with soft vignette. No text or characters on the cover or anywhere, no seals, no signature, no watermark.

| id | Tâm Pháp | Vật | Màu nhấn |
|---|---|---|---|
| `kim_cang` | Kim Cang Thiết Cốt Quyết | a thick bound book with iron corner plates, wrapped in prayer beads, a faint golden aura | gold |
| `thai_thanh` | Thái Thanh Kiếm Ý | an unrolled scroll with a slim sword lying across it and a wisp of cloud rising | jade green |
| `liet_diem` | Liệt Diễm Tâm Hỏa Quyết | a scorched book with flames licking its edges, a sabre's edge behind it | vermilion |
| `huyen_doc` | Vạn Độc Huyền Kinh | a dark leather book bound with snakeskin, a small poison vial and violet smoke | violet |
| `han_son` | Hàn Sơn Băng Tâm Quyết | a scroll frosted with ice crystals, a spear tip and a jade flute crossing behind it | icy blue |
| `thiet_phu` | Khai Sơn Bá Khí | a heavy stone-cover book chained shut, a cracked rock and an axe head behind it | rust brown |
| `bich_thuy` | Bích Thủy Thanh Âm | a slim book resting on a guqin, ripples of water and a lotus petal | teal |
