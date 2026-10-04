# Prompt icon vật phẩm phi binh khí và nguyên liệu (ChatGPT)

Theo quyết định #23/#25: 40 món mới (10 Hộ Cụ, 10 Công Pháp, 10 Đan Dược, 10 Kỳ Trân) và 17 nguyên liệu nâng cấp. Mô tả gốc do Gemini viết (`docs/reviews/2026-09-29-gemini-rarity-items.md`, `tools/gemini/out/economy/materials.json`), Claude rút gọn phần phong cách vì tin mở đầu đã nói.

Icon **dùng chung cho mọi phẩm** (phẩm thể hiện bằng màu khung trong game), nên vẽ ở mức chi tiết vừa phải, không vẽ theo phẩm.

## Cách làm
1. Mở **một** cuộc trò chuyện ChatGPT mới, đính kèm `01-style-board-thuy-mac.png` và `02-icon-thanh-phong-kiem.png` như đợt binh khí.
2. Gửi **tin mở đầu** của [prompts-items-mvp.md](prompts-items-mvp.md), nhưng thay câu "Detail level follows rarity…" bằng: `Detail level: medium for every icon (a few ornaments, no ink swirls around the object); rarity is shown by the game frame, not by the drawing.`
3. Mỗi tin nhắn dán **một** dòng ở cột Prompt. Hình lệch phong cách thì trả lời như ghi chú số 4 của file binh khí.
4. Tải về, đặt tên theo cột **File**. Món đồ lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/Items/`, nguyên liệu vào `Textures2D/Materials/`. Nền caro giả thì báo mình tách bằng `tools/art/ink_to_alpha.py`.
5. ChatGPT hay từ chối nhầm những mô tả mơ hồ ("aura", "radiating", "glow", "smoke", "poison", "blood"). Prompt đã viết lại thành đồ vật cụ thể. Nếu vẫn bị từ chối: bấm thử lại một lần, rồi mở cuộc trò chuyện mới (lịch sử bị chặn làm các lần sau dễ bị chặn tiếp).

## Vật phẩm (40 hình)

### Hộ Cụ

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 1 | Thiết Giáp Y | 2x1 | `thiet_giap_y.png` | `Icon (1536x1024 landscape, lying horizontally): Heavy riveted iron breastplate with leather straps, worn steel texture.` |
| 2 | Ô Thiết Giáp | 2x2 | `o_thiet_giap.png` | `Icon (1024x1024 square): A massive dark-tempered iron plate armor, rugged strokes, dense black ink.` |
| 3 | Kim Tuyến Y | 2x2 | `kim_tuyen_y.png` | `Icon (1024x1024 square): A shimmering golden-thread woven chainmail vest, delicate brushwork.` |
| 4 | Dạ Hổ Y | 1x2 | `da_ho_y.png` | `Icon (1024x1536 portrait, standing vertically): A rugged dark tiger-pelt sleeveless armor with thick fur lining.` |
| 5 | Nho Sam Y | 2x1 | `nho_sam_y.png` | `Icon (1536x1024 landscape, lying horizontally): An elegant layered scholar robe with silk sash, flowing cyan brush strokes.` |
| 6 | Băng Tâm Giáp | 2x1 | `bang_tam_giap.png` | `Icon (1536x1024 landscape, lying horizontally): A frost-encrusted light mail armor, pale blue.` |
| 7 | Tuế Nguyệt Bào Y | 1x2 | `tue_nguyet_bao_y.png` | `Icon (1024x1536 portrait, standing vertically): A long sage-green flowing Daoist robe hanging on a stand, vertical.` |
| 8 | Phục Ma Giáp | 2x2 | `phuc_ma_giap.png` | `Icon (1024x1024 square): Imposing ceremonial bronze heavy armor with fierce beast emblem, ancient patina.` |
| 9 | Thanh Đồng Hộ Tâm Kính | 1x1 | `thanh_dong_ho_tam_kinh.png` | `Icon (1024x1024 square): A circular ancient bronze mirror with engraved dragon motifs, soft metallic reflection.` |
| 10 | Long Lân Giáp | 2x2 | `long_lan_giap.png` | `Icon (1024x1024 square): A heavy armor vest covered in overlapping dragon-scale plates, dark iron scales with ochre gold edges, a small vermilion cord at the collar.` |

### Công Pháp

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 11 | Thuần Dương Công | 1x1 | `thuan_duong_cong.png` | `Icon (1024x1024 square): A rolled bamboo-slip book tied with a vermilion cord, the slips slightly fanned open at one end, a small ochre gold sun emblem on the cord's tag.` |
| 12 | Quy Nguyên Quyết | 1x1 | `quy_nguyen_quyet.png` | `Icon (1024x1024 square): A small circular jade-embossed manual with yin-yang ink swirl.` |
| 13 | Bách Luyện Chân Kinh | 1x2 | `bach_luyen_chan_kinh.png` | `Icon (1024x1536 portrait, standing vertically): A tall stack of two bound martial manuals tied with dark cord, the top cover showing a hammer-and-anvil brush emblem, ochre gold corner caps.` |
| 14 | U Minh Bí Lục | 1x1 | `u_minh_bi_luc.png` | `Icon (1024x1024 square): A weathered dark purple grimoire bound in coarse cord, mysterious.` |
| 15 | Liệt Hỏa Tâm Kinh | 1x1 | `liet_hoa_tam_kinh.png` | `Icon (1024x1024 square): A scorched-edge paper manual with crimson flame brush marks.` |
| 16 | Vô Niệm Kinh | 1x2 | `vo_niem_kinh.png` | `Icon (1024x1536 portrait, standing vertically): A vertical hanging Buddhist sutra scroll with cursive calligraphy.` |
| 17 | Phi Yến Quyết | 1x1 | `phi_yen_quyet.png` | `Icon (1024x1024 square): A lightweight treatise with a flying swallow ink emblem, delicate brushwork.` |
| 18 | Thiên Địa Vô Cực Công | 2x1 | `thien_dia_vo_cuc_cong.png` | `Icon (1536x1024 landscape, lying horizontally): An open horizontal handscroll with a circular heaven-and-earth diagram painted in ink, wooden rollers with ochre gold ends.` |
| 19 | Dưỡng Sinh Quyết | 1x2 | `duong_sinh_quyet.png` | `Icon (1024x1536 portrait, standing vertically): An aged jade-inlaid scroll bound with evergreen hemp.` |
| 20 | Kiếm Tông Tâm Yếu | 1x1 | `kiem_tong_tam_yeu.png` | `Icon (1024x1024 square): A folded manuscript with a sword ink silhouette on the parchment.` |

### Đan Dược

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 21 | Kim Sáng Dược | 1x1 | `kim_sang_duoc.png` | `Icon (1024x1024 square): A small round wooden salve box with the lid slightly open showing ochre healing paste, a strip of cloth bandage beside it.` |
| 22 | Hồi Thiên Đan | 1x1 | `hoi_thien_dan.png` | `Icon (1024x1024 square): A glossy dark-brown herbal pill resting in an ornate porcelain saucer.` |
| 23 | Thanh Tâm Đan | 1x1 | `thanh_tam_dan.png` | `Icon (1024x1024 square): A translucent jade medicine vial with soothing mint-green ink tint.` |
| 24 | Cuồng Hỏa Đan | 1x1 | `cuong_hoa_dan.png` | `Icon (1024x1024 square): A small round clay pill jar with a vermilion wax seal and a crack running down its side, a single red pill resting next to it.` |
| 25 | Vận Khí Đan | 1x1 | `van_khi_dan.png` | `Icon (1024x1024 square): A slender glazed celadon flask tied with crimson cord.` |
| 26 | Giải Độc Đan | 1x1 | `giai_doc_dan.png` | `Icon (1024x1024 square): A small white porcelain jar sealed with yellow talismans.` |
| 27 | Hộ Mạch Đan | 1x1 | `ho_mach_dan.png` | `Icon (1024x1024 square): A carved cinnabar gourd bottle with gold lacquer trim.` |
| 28 | Ngưng Thần Đan | 1x1 | `ngung_than_dan.png` | `Icon (1024x1024 square): A blue-and-white porcelain jar with swirling cloud motifs, delicate.` |
| 29 | Cửu Chuyển Hoàn Hồn Đan | 1x1 | `cuu_chuyen_hoan_hon_dan.png` | `Icon (1024x1024 square): A single large golden-red pill resting on a small nine-petal porcelain lotus stand, faint flat ink wisps rising from it.` |
| 30 | Tỳ Khang Đan | 1x1 | `ti_khang_dan.png` | `Icon (1024x1024 square): A plain earthenware medicine jar with a cloth-tied lid and a paper label without text, a few pale green herb leaves beside it.` |

### Kỳ Trân

| # | Món | Ô | File | Prompt |
|---|---|---|---|---|
| 31 | Trầm Hương Lư | 1x1 | `tram_huong_lo.png` | `Icon (1024x1024 square): A miniature three-legged bronze incense burner with a pierced lid, two thin flat ink curls rising from the lid.` |
| 32 | Bạch Ngọc Bội | 1x1 | `bach_ngoc_boi.png` | `Icon (1024x1024 square): A carved white mutton-fat nephrite jade pendant with silk tassel, soft.` |
| 33 | Dạ Minh Châu | 1x1 | `da_minh_chau.png` | `Icon (1024x1024 square): A large pale pearl resting on a small carved dark wooden stand, a few flat ink rings drawn around the pearl.` |
| 34 | Chu Sa Hồ Lô | 1x1 | `chu_sa_ho_lo.png` | `Icon (1024x1024 square): A polished cinnabar-red lacquer gourd with engraved talismanic runes.` |
| 35 | Thiền Tâm Chuỗi | 1x2 | `thien_tam_chuoi.png` | `Icon (1024x1536 portrait, standing vertically): A strung strand of dark wooden Buddhist prayer beads with central tassel.` |
| 36 | Vạn Độc Đỉnh | 1x1 | `van_doc_dinh.png` | `Icon (1024x1024 square): A small three-legged ancient bronze cauldron with green verdigris patches and two ring handles, lid closed.` |
| 37 | Huyết Ngọc Thạch | 1x1 | `huyet_ngoc_thach.png` | `Icon (1024x1024 square): A polished oval stone of deep red jade with dark ink veins, set in a simple ochre gold clasp with a short vermilion cord.` |
| 38 | Hàn Băng Châu | 1x1 | `han_bang_chau.png` | `Icon (1024x1024 square): A pale blue-grey crystal bead with frost-like flat ink strokes inside, hanging from a thin silver cord.` |
| 39 | Tử Kim Bát | 2x1 | `tu_kim_bat.png` | `Icon (1536x1024 landscape, lying horizontally): A monk's alms bowl of dark purple-gold metal seen from the side, engraved cloud bands, resting on a small folded cloth.` |
| 40 | Âm Dương Ngọc | 1x1 | `am_duong_ngoc.png` | `Icon (1024x1024 square): A round jade disc split into black and white halves in a yin-yang swirl, with a short vermilion tassel.` |

## Nguyên liệu (17 hình, lưu vào `Textures2D/Materials/`)

| # | Nguyên liệu | File | Prompt |
|---|---|---|---|
| 1 | Hàn Thiết | `han_thiet.png` | `Icon (1024x1024 square, small object centered): A rough chunk of frost-covered cold iron ore, blue-grey metallic sheen with ice crystals.` |
| 2 | Ô Thiết | `o_thiet.png` | `Icon (1024x1024 square, small object centered): A heavy dark-ink metal ingot with dull golden veins, dense and rugged.` |
| 3 | Thiên Ngoại Vẫn Thiết | `thien_ngoai_van_thiet.png` | `Icon (1024x1024 square, small object centered): A jagged dark meteorite fragment with cracks showing vermilion lines inside a scorched crust.` |
| 4 | Binh Hồn Tinh Thạch | `binh_hon_tinh_thach.png` | `Icon (1024x1024 square, small object centered): A sharp rhomboid crystal enclosing a miniature swirling sword silhouette in vermilion ink.` |
| 5 | Thanh Đồng Ti | `thanh_dong_ti.png` | `Icon (1024x1024 square, small object centered): A spool of spun greenish-bronze metallic thread wrapped around a carved bamboo bobbin.` |
| 6 | Ô Kim Ti | `o_kim_ti.png` | `Icon (1024x1024 square, small object centered): A coil of shimmering dark gold and black braided silk rope, supple and tough.` |
| 7 | Cửu U Tàm Ti | `cuu_u_tam_ti.png` | `Icon (1024x1024 square, small object centered): A neat skein of silver-blue silk thread wound on a small wooden spool.` |
| 8 | Huyền Giáp Hồn Thạch | `huyen_giap_hon_thach.png` | `Icon (1024x1024 square, small object centered): A polished turtle-shell-shaped dark stone talisman with carved ochre-gold runes.` |
| 9 | Hoàng Tuyên Chỉ | `hoang_tuyen_chi.png` | `Icon (1024x1024 square, small object centered): A roll of aged yellowed rice paper tied with a simple vermilion cord.` |
| 10 | Mặc Trúc Trản | `mac_truc_tran.png` | `Icon (1024x1024 square, small object centered): A bundle of dark bamboo slips bound with leather cord, inscribed with tiny ink characters.` |
| 11 | Tằm Tơ Cổ Quyển | `tam_to_co_quyen.png` | `Icon (1024x1024 square, small object centered): An ornate brocade silk scroll with jade scroll-ends and golden embroidery.` |
| 12 | Vấn Đạo Tâm Tinh | `van_dao_tam_tinh.png` | `Icon (1024x1024 square, small object centered): A single round prayer bead of dark crystal on a short knotted cord, a small ochre gold cap.` |
| 13 | Chu Sa Thảo | `chu_sa_thao.png` | `Icon (1024x1024 square, small object centered): A delicate wild herb with bright vermilion root nodules and pale green leaves.` |
| 14 | Tử Linh Chi | `tu_linh_chi.png` | `Icon (1024x1024 square, small object centered): A fan-shaped purple lingzhi mushroom with a glossy lacquer-like rim, cut at the stem.` |
| 15 | Côn Lôn Tuyết Liên | `con_lon_tuyet_lien.png` | `Icon (1024x1024 square, small object centered): A pure white lotus flower blooming with crystalline icy petals.` |
| 16 | Cửu Chuyển Đan Sa | `cuu_chuyen_dan_sa.png` | `Icon (1024x1024 square, small object centered): A tiny open celadon porcelain dish containing sparkling golden-red powder.` |
| 17 | Chí Tôn Ngọc Tủy | `chi_ton_ngoc_tuy.png` | `Icon (1024x1024 square, small object centered): A flawless hexagonal jade crystal, vermilion and ochre gold veins inside, standing on a small carved wooden base, a few flat ink strokes around it.` |

## Bổ sung 2026-10-03: rương Tiệm Danh Vọng (2 hình)

Dùng chung cuộc trò chuyện và tin mở đầu như trên. Lưu vào `Textures2D/Materials/`. Codex UF2 nạp hình theo mã; nếu đổi thư mục thì kết quả của task sẽ ghi.

| # | Món | File | Prompt |
|---|---|---|---|
| 18 | Rương Chất Dẫn Ngẫu Nhiên | `catalyst_box_random.png` | `Icon (1024x1024 square, small object centered): A small lacquered black wooden chest with ochre gold corner fittings, lid slightly open, a few colorful jade and crystal shards visible inside, a tied vermilion silk cord with a tassel.` |
| 19 | Rương Nguyên Liệu Bậc 3 | `tier_3_material_box.png` | `Icon (1024x1024 square, small object centered): A sturdy closed wooden supply crate bound with dark iron bands, three vermilion seal stamps on the front, a bundle of herbs and an iron ingot tied on top with rope.` |
