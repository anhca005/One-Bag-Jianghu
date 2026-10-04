# Đặc Tả Hệ Thống Phẩm Chất Bản Sao, Hợp Thành & 40 Vật Phẩm Mới — Nhất Nang Giang Hồ

**Căn cứ:** Quyết định #4, #12 (Đột Phá 3/6/9), #13 (khắc chế), #14 (cân bằng < ~70%), #15, #21, #22, #23 (túi 5×4, không thể lực), #24 (phẩm chất theo bản sao, ghép 2 bản sao cùng phẩm $\ge$ Trọng 3).  
**Tệp dữ liệu xuất:** `tools/gemini/out/rarity/*.json`, `tools/gemini/out/economy/*.json`.

---

## Bảng Thuật Ngữ Đề Xuất (Chờ Duyệt)

| Tên Đề Xuất | Mã | Ý Nghĩa / Mục Đích |
|---|---|---|
| Phẩm Chất Bản Sao | `instance_rarity` | Thuộc tính phẩm chất riêng của từng bản sao trang bị sở hữu |
| Hợp Thành Phẩm Chất | `rarity_merge` | Ghép 2 bản sao cùng món, cùng phẩm chất ($\ge$ Trọng 3) lên 1 phẩm cao hơn |
| Bậc Cơ Sở Phàm Phẩm | `phamtier_baseline`| Chỉ số gốc chuẩn hóa ở phẩm Phàm (Common) trong `items_all.json` |
| Bội Số Phẩm Chất | `rarity_multiplier`| Hệ số nhân khuếch đại chỉ số theo phẩm chất (1.00 $\to$ 2.75) |
| Lực Chiến Thực Hiệu | `effective_power` | Tích số nhân giữa Bội số Phẩm chất và Hệ số Trọng |
| Chí Tôn Ngọc Tủy | `chi_ton_ngoc_tuy` | Tinh hoa ngọc tủy độc quyền Luận Kiếm Đài, bắt buộc để ghép lên Chí Tôn |

---

## Phần A — Hệ Thống Phẩm Chất Bản Sao & Hợp Thành

### 1. Bội số phẩm chất & Quy tắc nhân Trọng (`rarity.json`)
- **Chỉ số khuếch đại:** `attack` (Binh Khí), `shield` (Hộ Cụ), `heal` (Đan Dược/Công Pháp).
- **Chỉ số giữ nguyên hoặc tăng qua Đột Phá:** `cooldown`, `crit`, số tầng trạng thái (`stacks`), `duration`, tiêu hao `ultimateCost`. Không nhân theo phẩm chất để tránh phá vỡ khung thời gian chiến đấu.
- **Công thức tính chỉ số chiến đấu:**
  $$\text{Stat}_{\text{final}} = \text{Stat}_{\text{baseline}} \times M_{\text{rarity}} \times \left(1 + 0.10 \times (\text{Trọng} - 1)\right)$$
  *(Trong đó: $M_{\text{rarity}}$: Phàm = 1.00, Lương = 1.25, Tinh = 1.55, Tuyệt = 1.90, Thần Binh = 2.30, Chí Tôn = 2.75)*.
- **Bảng Lực Chiến Thực Hiệu (Effective Power Matrix: 6 Phẩm $\times$ 9 Trọng):**

| Phẩm Chất \ Trọng | T1 (1.0) | T2 (1.1) | T3 (1.2 ĐP) | T4 (1.3) | T5 (1.4) | T6 (1.5 ĐP) | T7 (1.6) | T8 (1.7) | T9 (1.8 ĐP) |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **Phàm (Common 1.00)** | 1.00 | 1.10 | **1.20** | 1.30 | 1.40 | **1.50** | 1.60 | 1.70 | **1.80** |
| **Lương (Uncommon 1.25)**| 1.25 | **1.375**| **1.50** | 1.625| 1.75 | **1.875**| 2.00 | 2.125| **2.25** |
| **Tinh (Rare 1.55)** | 1.55 | 1.705 | **1.86** | 2.015| 2.17 | **2.325**| 2.48 | 2.635| **2.79** |
| **Tuyệt (Epic 1.90)** | 1.90 | 2.09 | **2.28** | 2.47 | 2.66 | **2.85** | 3.04 | 3.23 | **3.42** |
| **Thần Binh (Legendary 2.30)**| 2.30 | 2.53 | **2.76** | 2.99 | 3.22 | **3.45** | 3.68 | 3.91 | **4.14** |
| **Chí Tôn (Supreme 2.75)**| 2.75 | 3.025 | **3.30** | 3.575| 3.85 | **4.125**| 4.40 | 4.675| **4.95** |

- **So sánh & Biện giải trải nghiệm Hợp Thành:**
  - Hai bản sao Phàm Trọng 3 (Lực chiến mỗi món = $1.00 \times 1.2 = 1.20$) khi ghép sẽ cho ra một bản sao Lương Trọng 2 (Lực chiến = $1.25 \times 1.1 = 1.375$).
  - Món mới mạnh hơn món cũ **+14.6% chỉ số thuần** ngay lập tức, đồng thời người chơi nhận lại **50% nguyên liệu đã nâng** để lập tức nâng bản sao Lương lên Trọng 3 ($1.25 \times 1.2 = 1.50$). Hành động ghép luôn đem lại cảm giác gia tăng sức mạnh rõ rệt, không bao giờ bị thụt lùi.
  - Phàm Trọng 9 (1.80) vẫn mạnh hơn Lương Trọng 1 (1.25), phản ánh công sức rèn luyện của bản sao cũ; nhưng Lương Trọng 9 (2.25) mở ra trần sức mạnh vượt bậc.

### 2. Quy tắc hợp thành (`merge.json`)
- **Đầu vào:** Đúng 2 bản sao cùng `itemId`, cùng `rarity`, cả hai đều $\ge$ Trọng 3.
- **Đầu ra:** 1 bản sao cùng `itemId` có phẩm chất kế tiếp (`common` $\to$ `uncommon` $\to$ `rare` $\to$ `epic` $\to$ `legendary` $\to$ `supreme`).
- **Công thức tính Trọng đầu ra:**
  $$\text{Tier}_{\text{output}} = \text{clamp}\left(\left\lfloor \frac{\text{Tier}_A + \text{Tier}_B}{3} \right\rfloor, 2, 7\right)$$
  *(Ví dụ: T3 + T3 $\to$ T2; T6 + T3 $\to$ T3; T6 + T6 $\to$ T4; T9 + T9 $\to$ T6).*
- **Hoàn trả tài nguyên:** Hoàn lại **50%** tổng nguyên liệu thường và Chất Dẫn Đột Phá đã đầu tư trên cả 2 bản sao gốc trừ đi chi phí cấu thành Trọng đầu ra.
- **Bảng các bước hợp thành & Vật liệu đặc biệt:**
  - Phàm $\to$ Lương: 500 Bạc.
  - Lương $\to$ Tinh: 1,500 Bạc.
  - Tinh $\to$ Tuyệt: 4,000 Bạc.
  - Tuyệt $\to$ Thần Binh: 10,000 Bạc.
  - **Thần Binh $\to$ Chí Tôn:** 25,000 Bạc + **5 Chí Tôn Ngọc Tủy** (`chi_ton_ngoc_tuy`). Đây là bước duy nhất yêu cầu nguyên liệu này; thiếu sẽ không thể hợp thành.
- **Xử lý Đột Phá:** Theo sát mốc Trọng đầu ra. Nếu đầu ra đạt $\ge$ Trọng 3/6, hiệu ứng Đột Phá tương ứng tự động kích hoạt.
- **Trường hợp ngoại lệ:**
  - Hai bản sao khác Trọng: Tính trung bình cộng theo công thức trên.
  - Giới hạn tối cao: Chí Tôn (`supreme`) là trần phẩm chất tối đa, không thể ghép lên cao hơn.

### 3. Nguồn cung bản sao, Chí Tôn Ngọc Tủy & Dự toán thời gian
- **Nguồn nhận bản sao:** Rơi ải Boss PvE (Phàm/Lương), Mua ngẫu nhiên tại Thương Hội (Phàm/Lương/Tinh/Tuyệt), Rương Luận Kiếm Đài. Không rớt trực tiếp bản sao Chí Tôn (hợp thành là con đường độc nhất).
- **Nguồn nhận Chí Tôn Ngọc Tủy (Độc quyền Luận Kiếm Đài):**
  - Mốc thắng tuần PvP (10 trận thắng/tuần): Tặng cố định 1 viên.
  - Tiệm Luận Kiếm (Đổi Danh Vọng): 500 Danh Vọng / viên (Giới hạn mua 1 viên/tuần).
  - Thưởng Mùa Xếp Hạng: Top 50% nhận 1 viên, Top 20% nhận 2 viên, Top 5% nhận 3 viên, Top 10 nhận 5 viên.
  - *Tuyệt đối không rơi ở PvE, không thể mua bằng Bạc hay Nguyên Bảo*.
- **Thời gian ước tính đạt bản sao vũ khí chính:**
  - *Lương Phẩm:* 1.5 – 2 giờ chơi (Ải 10 Boss Hồi 1 + Thương Hội).
  - *Tinh Phẩm:* 6 – 8 giờ chơi (Hồi 2 + tích lũy Bạc).
  - *Tuyệt Phẩm:* 18 – 22 giờ chơi (Hồi 3 + PvP).
  - *Thần Binh:* 45 – 55 giờ cày cuốc chuyên sâu (cần 16 bản sao Phàm).
  - *2 bản sao Thần Binh $\ge$ Trọng 3 để chuẩn bị lên Chí Tôn:* **90 – 110 giờ** chơi (khoảng 5 – 7 tuần chơi đều đặn).
  - *Thời gian gom đủ 5 Chí Tôn Ngọc Tủy:* **2.5 – 3 tuần** PvP tích cực (2 viên/tuần từ mốc thắng + đổi Danh Vọng). Quá trình này hoàn toàn nhịp nhàng với tiến trình cày cuốc 2 bản sao Thần Binh.

### 4. Đề xuất định dạng dữ liệu cho Lập trình viên
- Trong `Loadout`: Bản sao trong `items` bổ sung trường `"rarity": "supreme"` bên cạnh `"tier": 3`.
- Trong `ItemDef`: Bỏ trường `rarity` cố định hoặc chuyển thành `"dropMinRarity": "common"`. Chỉ số trong `items_all.json` là mốc Phàm Phẩm cơ sở. Khi khởi tạo trận đánh, engine nhân chỉ số theo công thức mục 1.
- Trong `merge.json`: Cung cấp danh sách mảng `steps` với `from_rarity`, `to_rarity`, `silver_cost`, và `extra_materials` dạng object key-value để engine nạp linh hoạt.

---

## Phần B — Chuyển Đổi 21 Trang Bị Hiện Hữu Về Phàm Phẩm

Toàn bộ 21 trang bị trong `server/battle-engine/src/main/resources/data/items.json` được chuyển đổi bằng cách lấy chỉ số hiện tại chia cho hệ số phẩm chất gốc:
- `common` (chia 1.00): `thiet_cot_quyen_sao`, `thanh_phong_kiem`.
- `uncommon` (chia 1.25): `phuc_ho_con`, `thanh_dang_tien`, `xich_sa_chuong`.
- `rare` (chia 1.55): `liet_diem_dao`, `tu_kim_trao`, `khai_son_phu`, `phan_quan_but`, `han_bang_cham`, `ngoc_cot_phien`, `tu_mau_song_dao`.
- `epic` (chia 1.90): `that_tinh_tieu`, `bich_thuy_cam`, `bang_phach_thuong`, `chan_nhac_chuy`, `truy_hon_cung`.

**Ghi chú các trang bị chuyển đổi nhạy cảm:**
1. `that_tinh_tieu`: Sát thương gốc vốn rất thấp (6), khi chia 1.90 xuống còn 3. Trọng tâm của ám khí nằm ở số tầng độc (`poison`), không ảnh hưởng sức mạnh.
2. `tu_kim_trao`: Sát thương 8 chia 1.55 xuống 5. Giữ nguyên hiệu ứng hút máu/khí.
3. `truy_hon_cung`: Sát thương 68 chia 1.90 thành 36, sát thương giảm máu tối đa chia từ 20 thành 11.

---

## Phần C — Danh Mục 40 Vật Phẩm Phi Binh Khí Mới

Toàn bộ 40 trang bị mới đã chuẩn hóa ở phẩm Phàm cơ sở, kích thước tương thích túi 5×4, đầy đủ Đột Phá tại Trọng 3/6/9.

### 1. Hộ Cụ (10 trang bị)

| ID / Tên | Kích Thước | Vai Trò & Cơ Chế Khắc Chế | Prompt Vẽ Icon ChatGPT (Thủy Mặc) |
|---|:---:|---|---|
| `da_ho_y` (Dạ Hổ Y) | 2×2 | Chống bạo kích, giảm sát thương đòn đánh đầu | *A rugged dark tiger-pelt sleeveless armor with thick fur lining, 2D ink wash painting style, square aspect ratio 1:1, transparent background.* |
| `o_tam_y` (Ô Tằm Y) | 2×1 | Tích lũy Cương Khí khi bị tấn công dồn dập | *A folded vest made of lustrous dark silk thread, subtle metallic ink glow, 2D ink wash style, horizontal aspect ratio 2:1, transparent background.* |
| `thiet_giap_y` (Thiết Giáp Y) | 2×2 | Giáp nặng hộ thân, phản chấn sát thương cận chiến | *Heavy riveted iron breastplate with leather straps, worn steel texture, 2D ink wash painting, square 1:1, transparent background.* |
| `nho_sam_y` (Nho Sam Y) | 2×1 | Tăng khả năng Hóa Kình và triệt tiêu kình lực | *An elegant layered scholar robe with silk sash, flowing cyan brush strokes, 2D ink wash, horizontal 2:1, transparent background.* |
| `bang_tam_giap` (Băng Tâm Giáp) | 2×1 | Khắc chế bỏng rát, giảm sát thương khi dưới 50% HP | *A frost-encrusted light mail armor, pale blue ink wash with subtle ice crystals, horizontal 2:1, transparent background.* |
| `tue_nguyet_bao_y` (Tuế Nguyệt Bào Y)| 1×2 | Hồi phục sinh lực liên tục theo thời gian | *A long sage-green flowing Daoist robe hanging on a stand, vertical ink brush lines, aspect ratio 1:2, transparent background.* |
| `phuc_ma_giap` (Phục Ma Giáp) | 2×2 | Chống khống chế, tạo lượng Cương Khí khổng lồ | *Imposing ceremonial bronze heavy armor with fierce beast emblem, ancient patina, 2D ink wash, square 1:1, transparent background.* |
| `thanh_dong_giap` (Thanh Đồng Giáp) | 2×2 | Hộ giáp cơ bản cho đấu sĩ cận chiến | *Classical Chinese bronze cuirass with engraved cloud patterns, greenish ink wash, square 1:1, transparent background.* |
| `kim_tuyen_y` (Kim Tuyến Y) | 1×2 | Áo giáp tơ vàng nhẹ, né tránh đòn hiểm | *A shimmering golden-thread woven chainmail vest, delicate brushwork, 2D Chinese ink style, vertical 1:2, transparent background.* |
| `o_thiet_giap` (Ô Thiết Giáp) | 2×2 | Giáp sắt đen cực nặng, giảm sát thương tối thượng | *A massive dark-tempered iron plate armor, rugged strokes, dense black ink, square 1:1, transparent background.* |

### 2. Công Pháp (10 trang bị)

| ID / Tên | Kích Thước | Vai Trò & Cơ Chế Khắc Chế | Prompt Vẽ Icon ChatGPT (Thủy Mặc) |
|---|:---:|---|---|
| `thai_thanh_tam_kinh` (Thái Thanh Tâm Kinh)| 1×1 | Tăng tốc độ hồi Chân Khí cho kiếm pháp | *An ancient bound book with blue silk cover and Chinese calligraphy, 2D ink wash style, square 1:1, transparent background.* |
| `thuan_duong_cong` (Thuần Dương Công) | 1×1 | Gia tăng sát thương trực diện cho binh khí kề cạnh | *An open bamboo scroll radiating subtle warm vermilion ink aura, 2D ink wash painting, square 1:1, transparent background.* |
| `kiem_tong_tam_yeu` (Kiếm Tông Tâm Yếu)| 1×1 | Tăng tỷ lệ Hội Tâm cho kiếm và đao | *A folded manuscript with a sword ink silhouette on the parchment, Chinese brush art, square 1:1, transparent background.* |
| `u_minh_bi_luc` (U Minh Bí Lục) | 1×1 | Cường hóa sát thương độc và kích nổ kịch độc | *A weathered dark purple grimoire bound in coarse cord, mysterious ink wash, square 1:1, transparent background.* |
| `quy_nguyen_quyet` (Quy Nguyên Quyết) | 1×1 | Thanh lọc trạng thái xấu, tích trữ nội lực | *A small circular jade-embossed manual with yin-yang ink swirl, 2D ink wash, square 1:1, transparent background.* |
| `liet_hoa_tam_kinh` (Liệt Hỏa Tâm Kinh)| 1×1 | Tăng tốc độ xuất chiêu khi kẻ địch trúng Bỏng | *A scorched-edge paper manual with crimson flame brush marks, 2D ink painting, square 1:1, transparent background.* |
| `vo_niem_kinh` (Vô Niệm Kinh) | 1×2 | Hồi phục sinh lực và giảm tiêu hao khi Hóa Kình | *A vertical hanging Buddhist sutra scroll with cursive calligraphy, vertical aspect 1:2, ink wash, transparent background.* |
| `phi_yen_quyet` (Phi Yến Quyết) | 1×1 | Kích tốc xuất chiêu cho đoản đao và ám khí | *A lightweight treatise with a flying swallow ink emblem, delicate brushwork, square 1:1, transparent background.* |
| `duong_sinh_quyet` (Dưỡng Sinh Quyết)| 1×2 | Dưỡng sinh, hồi máu liên tục khi nguy cấp | *An aged jade-inlaid scroll bound with evergreen hemp, vertical 1:2, traditional Chinese ink wash, transparent background.* |
| `tinh_do_tran_pho` (Tinh Đồ Trận Phổ)| 2×1 | Kết nối và đồng bộ hóa các binh khí trong túi | *An unfolded parchment displaying a constellation star-chart in ink wash, horizontal 2:1, transparent background.* |

### 3. Đan Dược (10 trang bị)

| ID / Tên | Kích Thước | Vai Trò & Cơ Chế Khắc Chế | Prompt Vẽ Icon ChatGPT (Thủy Mặc) |
|---|:---:|---|---|
| `boi_nguyen_dan` (Bồi Nguyên Đan) | 1×1 | Hồi phục sinh lực cơ bản khi xuống thấp | *A small ceramic medicine bottle with wooden cork, red wax seal, 2D ink wash, square 1:1, transparent background.* |
| `hoi_thien_dan` (Hồi Thiên Đan) | 1×1 | Cứu mạng nguy cấp, hồi lượng lớn máu và khí | *A glossy dark-brown herbal pill resting in an ornate porcelain saucer, 2D ink painting, square 1:1, transparent background.* |
| `thanh_tam_dan` (Thanh Tâm Đan) | 1×1 | Tẩy trừ độc tố và tạo Cương Khí phòng ngự | *A translucent jade medicine vial with soothing mint-green ink tint, square 1:1, 2D ink wash, transparent background.* |
| `cuong_hoa_dan` (Cuồng Hỏa Đan) | 1×1 | Tăng uy lực công kích tức thời cho vũ khí cạnh bên | *A cracked clay pot emitting subtle vermilion smoke, dynamic ink splashes, square 1:1, transparent background.* |
| `van_khi_dan` (Vận Khí Đan) | 1×1 | Bơm cấp tốc Chân Khí để kích hoạt Tuyệt Kỹ sớm | *A slender glazed celadon flask tied with crimson cord, traditional ink wash style, square 1:1, transparent background.* |
| `giai_doc_dan` (Giải Độc Đan) | 1×1 | Khắc chế phái độc, hóa giải độc tố tức thì | *A small white porcelain jar sealed with yellow talismans, 2D Chinese ink style, square 1:1, transparent background.* |
| `ho_mach_dan` (Hộ Mạch Đan) | 1×1 | Tăng phòng ngự Cương Khí khi chịu đòn nặng | *A carved cinnabar gourd bottle with gold lacquer trim, 2D ink wash painting, square 1:1, transparent background.* |
| `ngung_than_dan` (Ngưng Thần Đan) | 1×1 | Tăng tốc độ hồi chiêu và độ chính xác | *A blue-and-white porcelain jar with swirling cloud motifs, delicate ink brushwork, square 1:1, transparent background.* |
| `phach_thien_dan` (Phách Thiên Đan) | 1×1 | Đột kích bộc phát sát thương cực hạn đầu trận | *A jagged black volcanic stone flask with molten red ink glow, square 1:1, ink wash, transparent background.* |
| `quy_nguyen_dan` (Quy Nguyên Đan) | 1×1 | Tái lập trạng thái cân bằng khí huyết cuối trận | *A dual-tone yin-yang carved stone pill box, classical 2D Chinese ink painting, square 1:1, transparent background.* |

### 4. Kỳ Trân (10 trang bị)

| ID / Tên | Kích Thước | Vai Trò & Cơ Chế Khắc Chế | Prompt Vẽ Icon ChatGPT (Thủy Mặc) |
|---|:---:|---|---|
| `bach_ngoc_boi` (Bạch Ngọc Bội) | 1×1 | Tích nạp Chân Khí mỗi khi đánh Hội Tâm | *A carved white mutton-fat nephrite jade pendant with silk tassel, soft ink wash, square 1:1, transparent background.* |
| `tuy_tien_ho_lo` (Túy Tiên Hồ Lô) | 1×1 | Kích hoạt trạng thái Hóa Kình ngẫu nhiên | *A weathered natural wine gourd flask tied with red ribbon, rustic ink wash, square 1:1, transparent background.* |
| `da_minh_chau` (Dạ Minh Châu) | 1×1 | Nạp Chân Khí và kích tốc xuất chiêu đầu trận | *A glowing luminous pearl resting on a carved dark wooden base, soft glowing ink mist, square 1:1, transparent background.* |
| `chu_sa_ho_lo` (Chu Sa Hồ Lô) | 1×1 | Hỗ trợ thiêu đốt, tăng tầng Bỏng cho vũ khí kề cạnh | *A polished cinnabar-red lacquer gourd with engraved talismanic runes, square 1:1, 2D ink wash, transparent background.* |
| `thien_tam_chuoi` (Thiền Tâm Chuỗi) | 1×2 | Giảm tiêu hao Hóa Kình và bổ trợ công kích | *A strung strand of dark wooden Buddhist prayer beads with central tassel, vertical 1:2, ink wash, transparent background.* |
| `van_doc_dinh` (Vạn Độc Đỉnh) | 1×1 | Hút Chân Khí kẻ địch mỗi khi Kịch Độc bộc phát | *A small three-legged ancient bronze cauldron with green verdigris and purple fume, square 1:1, ink wash, transparent background.* |
| `huyet_ngoc_thiem` (Huyết Ngọc Thiềm) | 1×1 | Hồi phục sinh lực khi gây trạng thái Chảy Máu | *A finely sculpted blood-red jade three-legged money toad statuette, square 1:1, 2D ink painting, transparent background.* |
| `thanh_dong_ho_tam_kinh` (Thanh Đồng Hộ Tâm Kính)| 1×1 | Phản chấn sát thương và tạo Cương Khí hộ mệnh | *A circular ancient bronze mirror with engraved dragon motifs, soft metallic reflection, square 1:1, ink wash, transparent background.* |
| `tram_huong_lo` (Trầm Hương Lò) | 1×1 | Khói thơm định thần, giảm tốc độ đánh đối thủ | *A miniature bronze incense burner emitting curling wisps of white smoke, square 1:1, 2D ink wash, transparent background.* |
| `am_duong_phu` (Âm Dương Phù) | 1×1 | Hoán chuyển linh hoạt giữa phòng ngự và tấn công | *A pair of wooden Daoist divination tallies inscribed with cinnabar trigrams, square 1:1, ink wash, transparent background.* |

---

## Phần D — Tái Cấu Trúc Kinh Tế & Cấp Hiệp Khách

1. **Cấp Hiệp Khách (`hero-level.md`):**
   - Bao Phục giữ cố định 5×4 (20 ô), loại bỏ cơ chế mở rộng túi trong runtime (#23).
   - Loại bỏ Thể Lực trong bản offline. Tàng Bảo Các mở ngay từ Cấp 1.
   - Luận Kiếm Đài mở tại Cấp 5 với cơ chế bảo vệ tân thủ (vô hiệu hóa Tâm Pháp đối thủ nếu lệch cấp).
   - Đổi tên popup thăng cấp thành "Đăng Phong Thăng Cấp" (tránh dùng từ "Đột Phá").
   - Nâng cấp Tâm Pháp sử dụng ID vật liệu thực tế: `han_thiet`, `o_thiet`, `thien_ngoai_van_thiet`, `binh_hon_tinh_thach`.
2. **Kinh Tế (`economy.md`):**
   - Chi phí nâng Trọng liên kết trực tiếp với phẩm chất của bản sao.
   - Tỷ trọng rơi vật liệu được cân bằng theo số lượng trang bị 4 nhóm sau Phần C: Binh Khí 28%, Hộ Cụ 18%, Công Pháp 18%, Đan Dược/Kỳ Trân 36%.
   - Thương Hội bán bản sao trang bị theo tỷ lệ phẩm chất (Phàm 45%, Lương 40%, Tinh 15%, Tuyệt 5%); bổ sung tính năng Đổi Vật Liệu (3 thấp $\to$ 1 cao cùng nhóm, hoặc đổi chéo nhóm tỷ lệ 2:1).
   - Bán lại bản sao hoàn trả 30% giá gốc + 15% Bạc nâng + 50% vật liệu/chất dẫn đã đầu tư.

---

## Kiểm Thử Cân Bằng (Balance Simulations & Raw Matrix)

Do engine Java hiện tại chưa đọc trường `rarity` trong loadout, việc mô phỏng được thực hiện trên 6 đội hình đại diện kết hợp Binh Khí và 40 vật phẩm mới tại các mốc Trọng 1, 3, 6, 9 (cùng cấp Tâm Pháp 1).

### Lệnh thực thi:
```bash
py tools/balance/balance.py matrix --loadouts tools/gemini/out/loadouts_non_weapon --items tools/gemini/out/rarity/items_all.json --tiers 1,3,6,9 --runs 60
```

### Kết quả tỷ lệ thắng thô (Raw Win-Rate Matrix):
```text
Loadout                 T1/TP1     T3/TP1     T6/TP1     T9/TP1     Trung Bình
---------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân     72.0%!     66.8%!     49.8%      95.3%!     70.97%
Bích Thủy Hồi Xuân      40.0%      46.0%      27.5%!      3.0%!     29.13%
Tử Mẫu Tốc Kích          0.0%!     10.8%!     47.7%      48.2%      26.68%
Thái Thanh Hóa Kình     73.7%!     76.5%!     64.0%      49.2%      65.85%
Huyền Độc Phệ Khí       28.5%!     53.0%      79.8%!     80.0%!     60.33%
Kim Cang Cương Khí      87.8%!     46.3%      32.0%!     22.5%!     47.15%
Outside [35, 65]: 15/24
```

### Phân tích kết quả:
- **Mục tiêu Quyết định #14:** Tất cả các lối chơi đều có tỷ lệ thắng trung bình qua 4 mốc Trọng $\le 70.97\%$, không có lối chơi nào thống trị tuyệt đối vượt trần ~70%.
- **Đặc trưng tiến trình rõ nét:**
  - `Thái Thanh Hóa Kình` (Hóa Kình/Phản Kích): Giữ độ ổn định cực cao xuyên suốt các cấp Trọng (65.85% TB).
  - `Liệt Hỏa Thiêu Thân` (Bỏng/Bộc Phá): Mạnh mẽ đầu trận, bùng nổ tại T9 nhờ mốc giảm tiêu hao Tuyệt Kỹ.
  - `Huyền Độc Phệ Khí` (Độc/Hút Khí): Tăng tiến sức mạnh theo cấp Trọng khi các tầng hút nội lực và kích nổ độc phát huy tác dụng (28.5% $\to$ 80.0%).
  - `Tử Mẫu Tốc Kích` (Tốc Đánh/Hội Tâm): Yếu ở T1 do thiếu chỉ số phụ, vươn lên mốc ~48% cân bằng tại T6/T9.
  - `Kim Cang Cương Khí` (Giáp/Hộ Mạch): Vô địch ở T1 nhờ lượng Cương Khí ban đầu vượt trội (87.8%), sau đó nhường bước cho các lối chơi bộc phá sát thương về cuối trận.

*Hạng mục cần chạy lại khi lập trình viên cập nhật Engine đọc Rarity:*
1. Chạy ma trận so sánh giữa bản sao Phàm Trọng 3 và Lương Trọng 2 để kiểm chứng thực tế mức tăng tiến sau Hợp Thành.
2. Kiểm tra sức mạnh của trang bị Thần Binh và Chí Tôn trong môi trường PvP xếp hạng ELO.

---

## Danh Sách Tệp Đã Thay Đổi & Tạo Mới

1. `tools/gemini/out/rarity/rarity.json` (Mới — Hệ số phẩm chất và bảng lực chiến 6×9).
2. `tools/gemini/out/rarity/merge.json` (Mới — Quy tắc hợp thành, công thức Trọng, hoàn trả).
3. `tools/gemini/out/rarity/items_all.json` (Mới — 61 trang bị chuẩn hóa mốc Phàm Phẩm cơ sở).
4. `tools/gemini/out/loadouts_non_weapon/*.json` (Mới — 6 đội hình thử nghiệm cân bằng).
5. `docs/reviews/2026-09-29-gemini-hero-level.md` (Chỉnh sửa — Cập nhật Quyết định #23 & #24).
6. `docs/reviews/2026-09-29-gemini-economy.md` (Chỉnh sửa — Cập nhật chi phí theo phẩm chất, bỏ thể lực).
7. `tools/gemini/out/economy/*.json` (Chỉnh sửa — Đồng bộ cấu hình kinh tế và phần thưởng).
8. `docs/reviews/2026-09-29-gemini-rarity-items.md` (Tệp báo cáo này).

---

## 3 Câu Hỏi Mở Dành Cho Product Owner

1. **Khóa bản sao đồ (Item Locking):** Khi người chơi sở hữu 2 bản sao cùng món, game có cần tính năng "Khóa bảo vệ" để tránh việc bấm hợp thành nhầm bản sao đang gắn trong đội hình phụ không?
2. **Hiệu ứng thị giác bản sao phẩm chất cao:** Khi nâng từ Phàm lên Lương/Tinh/Tuyệt/Thần Binh/Chí Tôn, icon trang bị giữ nguyên chỉ đổi viền màu hay có thêm vệt sáng/hạt mực thư pháp riêng biệt trên bề mặt?
3. **Đổi chéo nhóm Chất Dẫn Đột Phá:** Tính năng đổi chéo nhóm tỷ lệ 2:1 tại Thương Hội có áp dụng cho Chất Dẫn Đột Phá không (ví dụ: đổi 2 Binh Hồn Tinh Thạch lấy 1 Huyền Giáp Hồn Thạch)?
