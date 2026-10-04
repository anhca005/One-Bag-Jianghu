# Thiết Kế Kinh Tế: Nguyên Liệu Nâng Trọng, Chi Phí, Thương Hội & Phần Thưởng

**Hệ máy:** PC (Auto-battler 2D Thủy Mặc).  
**Căn cứ:** Quyết định #4 (vật phẩm giữ lâu dài), #12 (Đột Phá 3/6/9), #14 (cân bằng), #21 (mở phái cấp 10, túi ngoài trận), #22 (shop NPC, nâng Trọng không hỏng), #23 (túi 5×4 cố định, không thể lực), #24 (phẩm chất theo bản sao, ghép 2 bản sao cùng phẩm tăng phẩm chất, ghép 2 Thần Binh lên Chí Tôn cần nguyên liệu PvP độc quyền).  
**Tệp dữ liệu liên kết:** `tools/gemini/out/economy/*.json`.

---

## Bảng Thuật Ngữ Đề Xuất (Chờ Duyệt)

| Tên Đề Xuất | Mã | Ý Nghĩa / Mục Đích |
|---|---|---|
| Hàn Thiết | `han_thiet` | Quặng sắt lạnh, nguyên liệu Binh Khí Bậc 1 (Trọng 1→3) |
| Ô Thiết | `o_thiet` | Kim loại đen nặng, nguyên liệu Binh Khí Bậc 2 (Trọng 4→6) |
| Thiên Ngoại Vẫn Thiết | `thien_ngoai_van_thiet` | Thiên thạch rơi, nguyên liệu Binh Khí Bậc 3 (Trọng 7→9) |
| Binh Hồn Tinh Thạch | `binh_hon_tinh_thach` | Chất dẫn Đột Phá Binh Khí (mốc Trọng 3, 6, 9) |
| Thanh Đồng Ti | `thanh_dong_ti` | Tơ đồng mềm dệt giáp, nguyên liệu Hộ Cụ Bậc 1 |
| Ô Kim Ti | `o_kim_ti` | Tơ vàng đen dẻo dai, nguyên liệu Hộ Cụ Bậc 2 |
| Cửu U Tàm Ti | `cuu_u_tam_ti` | Tơ tằm hàn đầm, nguyên liệu Hộ Cụ Bậc 3 |
| Huyền Giáp Hồn Thạch | `huyen_giap_hon_thach` | Chất dẫn Đột Phá Hộ Cụ (mốc Trọng 3, 6, 9) |
| Hoàng Tuyên Chỉ | `hoang_tuyen_chi` | Giấy dó vàng cổ truyền thư, nguyên liệu Công Pháp Bậc 1 |
| Mặc Trúc Trản | `mac_truc_tran` | Thẻ trúc khắc mực, nguyên liệu Công Pháp Bậc 2 |
| Tằm Tơ Cổ Quyển | `tam_to_co_quyen` | Cuộn lụa thư họa bí kíp, nguyên liệu Công Pháp Bậc 3 |
| Vấn Đạo Tâm Tinh | `van_dao_tam_tinh` | Xá lợi đốn ngộ, chất dẫn Đột Phá Công Pháp |
| Chu Sa Thảo | `chu_sa_thao` | Dược thảo mỏ son, nguyên liệu Đan Dược & Kỳ Trân Bậc 1 |
| Tử Linh Chi | `tu_linh_chi` | Nấm linh chi tím vách đá, nguyên liệu Đan Dược & Kỳ Trân Bậc 2 |
| Côn Lôn Tuyết Liên | `con_lon_tuyet_lien` | Sen tuyết băng giá ngàn năm, nguyên liệu Đan Dược & Kỳ Trân Bậc 3 |
| Cửu Chuyển Đan Sa | `cuu_chuyen_dan_sa` | Đan sa chín lần luyện lò, chất dẫn Đột Phá Đan Dược & Kỳ Trân |
| Chất Dẫn Đột Phá | `catalyst` | Loại nguyên liệu quý hiếm bắt buộc dùng khi nâng vào mốc Đột Phá 3, 6, 9 |
| Chí Tôn Ngọc Tủy | `chi_ton_ngoc_tuy` | Tinh hoa ngọc tủy độc quyền Luận Kiếm Đài, bắt buộc để ghép lên Chí Tôn |
| Chuyển Đổi Vật Liệu | `material_exchange` | Đổi 3 nguyên liệu cấp thấp lên 1 cấp cao, hoặc đổi chéo nhóm tại Thương Hội |

---

## 1. Hệ Thống Nguyên Liệu & Tỷ Trọng Rơi Theo 4 Nhóm

Toàn bộ 61 vật phẩm trong trò chơi (17 Binh Khí, 11 Hộ Cụ, 11 Công Pháp, 22 Đan Dược & Kỳ Trân) được quy về 4 nhóm nguyên liệu riêng biệt. Tỷ trọng rơi tài nguyên phân bổ tương ứng:

| Nhóm Vật Phẩm | Số Lượng Vật Phẩm | Tỷ Trọng Rơi | Bậc 1 (Trọng 1→3) | Bậc 2 (Trọng 4→6) | Bậc 3 (Trọng 7→9) | Chất Dẫn (Mốc 3/6/9) |
|---|:---:|:---:|---|---|---|---|
| **Binh Khí** | 17 | **28%** | Hàn Thiết | Ô Thiết | Thiên Ngoại Vẫn Thiết | Binh Hồn Tinh Thạch |
| **Hộ Cụ** | 11 | **18%** | Thanh Đồng Ti | Ô Kim Ti | Cửu U Tàm Ti | Huyền Giáp Hồn Thạch |
| **Công Pháp** | 11 | **18%** | Hoàng Tuyên Chỉ | Mặc Trúc Trản | Tằm Tơ Cổ Quyển | Vấn Đạo Tâm Tinh |
| **Đan Dược / Kỳ Trân** | 22 | **36%** | Chu Sa Thảo | Tử Linh Chi | Côn Lôn Tuyết Liên | Cửu Chuyển Đan Sa |

- **Nguyên liệu đặc biệt tối cao:** **Chí Tôn Ngọc Tủy** (`chi_ton_ngoc_tuy`) là nguyên liệu phổ quát chung cho mọi loại trang bị, **độc quyền rơi tại Luận Kiếm Đài (PvP)**. Tuyệt đối không rớt ở PvE, không bán bằng Bạc hay Nguyên Bảo.
- **Nguồn rơi của từng bậc nguyên liệu:**
  - *Bậc 1:* Hành Tẩu Giang Hồ Hồi 1, Rương Thắng PvP sơ cấp, Thương Hội.
  - *Bậc 2:* Hành Tẩu Giang Hồ Hồi 2, Luận Kiếm Đài (PvP), Thương Hội.
  - *Bậc 3:* Hành Tẩu Giang Hồ Hồi 3+, Thưởng Mùa Luận Kiếm, Thương Hội.
  - *Chất Dẫn Đột Phá thường:* 100% rơi lần đầu tại Boss Ải 10 mỗi Hồi, rương thưởng cột mốc PvP, và ô đặc biệt Thương Hội.

---

## 2. Bảng Chi Phí Nâng Trọng Theo Phẩm Chất Bản Sao

Quy tắc Quyết định #22 & #24:
- **Nâng không bao giờ thất bại**.
- Chi phí nâng phụ thuộc trực tiếp vào **Phẩm Chất hiện tại của chính bản sao đó** (Phàm $\to$ Lương $\to$ Tinh $\to$ Tuyệt $\to$ Thần Binh $\to$ Chí Tôn).
- Bản sao Chí Tôn sau khi hợp thành vẫn có thể tiếp tục nâng Trọng 1 $\to$ 9 với chi phí tương ứng ở cột Chí Tôn.

| Bước Nâng Trọng | Nguyên Liệu (Phàm / Lương / Tinh) | Nguyên Liệu (Tuyệt / Thần / Chí Tôn) | Chất Dẫn | Bạc (Phàm / Lương / Tinh) | Bạc (Tuyệt / Thần / Chí Tôn) |
|:---:|:---:|:---:|:---:|:---:|:---:|
| **Trọng 1 → 2** | 3 / 4 / 6 Bậc 1 | 9 / 14 / 20 Bậc 1 | 0 | 100 / 140 / 200 | 300 / 450 / 650 |
| **Trọng 2 → 3 (ĐP)** | 6 / 8 / 12 Bậc 1 | 18 / 27 / 39 Bậc 1 | **1** (Thần: 2, Chí: 2) | 250 / 350 / 500 | 750 / 1,125 / 1,625 |
| **Trọng 3 → 4** | 4 / 6 / 8 Bậc 2 | 12 / 18 / 26 Bậc 2 | 0 | 500 / 700 / 1,000 | 1,500 / 2,250 / 3,250 |
| **Trọng 4 → 5** | 7 / 10 / 14 Bậc 2 | 21 / 32 / 46 Bậc 2 | 0 | 900 / 1,260 / 1,800 | 2,700 / 4,050 / 5,850 |
| **Trọng 5 → 6 (ĐP)** | 10 / 14 / 20 Bậc 2 | 30 / 45 / 65 Bậc 2 | **2** (Thần: 3, Chí: 4) | 1,500 / 2,100 / 3,000 | 4,500 / 6,750 / 9,750 |
| **Trọng 6 → 7** | 6 / 8 / 12 Bậc 3 | 18 / 27 / 39 Bậc 3 | 0 | 2,500 / 3,500 / 5,000 | 7,500 / 11,250 / 16,250 |
| **Trọng 7 → 8** | 10 / 14 / 20 Bậc 3 | 30 / 45 / 65 Bậc 3 | 0 | 4,000 / 5,600 / 8,000 | 12,000 / 18,000 / 26,000 |
| **Trọng 8 → 9 (ĐP)** | 15 / 21 / 30 Bậc 3 | 45 / 68 / 98 Bậc 3 | **3** (Thần: 5, Chí: 6) | 6,500 / 9,100 / 13,000 | 19,500 / 29,250 / 42,250 |

---

## 3. Nhịp Độ Tiến Trình & Chu Kỳ Ghép Lên Chí Tôn

- **Tốc độ chơi trung bình:** 20 trận/giờ (tự do không giới hạn Thể Lực).
- **Mốc Lương Phẩm (Uncommon):** Sau ~1.5 – 2 giờ chơi (ghép 2 bản sao Phàm $\ge$ Trọng 3).
- **Mốc Tinh Phẩm (Rare):** Sau ~6 – 8 giờ chơi (Cấp 18–20).
- **Mốc Tuyệt Phẩm (Epic):** Sau ~18 – 22 giờ chơi (Cấp 25–28).
- **Mốc Thần Binh (Legendary):** ~45 – 55 giờ chơi chuyên sâu cho 1 bản sao.
- **Hợp thành Chí Tôn (Supreme):**
  - Điều kiện: 2 bản sao Thần Binh cùng món $\ge$ Trọng 3 + **5 Chí Tôn Ngọc Tủy** + 25,000 Bạc.
  - *Thời gian sở hữu 2 bản sao Thần Binh:* ~90 – 110 giờ chơi (khoảng 5 – 7 tuần chơi đều đặn).
  - *Thời gian gom đủ 5 Chí Tôn Ngọc Tủy:* Khoảng **2.5 – 3 tuần** PvP (đạt 10 trận thắng/tuần tặng 1 viên + đổi 1 viên/tuần từ Tiệm Danh Vọng). Người chơi có đủ nguyên liệu PvP trước khi gom đủ 2 Thần Binh, tạo động lực thi đấu liên tục mà không bị nghẽn cổ chai.

---

## 4. Bảng Phần Thưởng Rơi (Battle Rewards — Không Thể Lực)

### 4.1. Hành Tẩu Giang Hồ (PvE)

| Phân Loại Ải | Kết Quả | Bạc | EXP | Nguyên Liệu Rơi (Theo Tỷ Trọng Nhóm) | Tỷ Lệ Chất Dẫn |
|---|---|---|---|---|---|
| **Hồi 1 (Ải 1–9)** | Thắng Lần Đầu | 200 | 60 | 3 Bậc 1 (100%) | 20% |
| | Đánh Lại Thắng | 60 | 20 | 1 Bậc 1 (100%) | 5% |
| | Thua Trận | 15 | 5 | 1 Bậc 1 (30%) | 0% |
| **Hồi 1: Ải 10 (Boss)** | Thắng Lần Đầu | 600 | 150 | 8 Bậc 1 (100%) | **100% (1 Viên)** |
| | Đánh Lại Thắng | 150 | 40 | 3 Bậc 1 (100%) | 15% |
| | Thua Trận | 30 | 10 | 1 Bậc 1 (50%) | 0% |
| **Hồi 2 (Ải 1–9)** | Thắng Lần Đầu | 400 | 100 | 3 Bậc 2 (100%) | 25% |
| | Đánh Lại Thắng | 120 | 35 | 1 Bậc 2 (100%) | 8% |
| | Thua Trận | 25 | 10 | 1 Bậc 2 (30%) | 0% |
| **Hồi 2: Ải 10 (Boss)** | Thắng Lần Đầu | 1,200 | 250 | 8 Bậc 2 (100%) | **100% (1 Viên)** |
| | Đánh Lại Thắng | 300 | 70 | 3 Bậc 2 (100%) | 20% |
| | Thua Trận | 50 | 15 | 1 Bậc 2 (50%) | 0% |

### 4.2. Luận Kiếm Đài (PvP)

- **Thưởng từng trận:**
  - *Thắng:* 100 Bạc, 25 Danh Vọng, 35 EXP, 50% rơi 1 nguyên liệu Bậc 1/2, 10% rơi Chất Dẫn thường.
  - *Hòa:* 50 Bạc, 10 Danh Vọng, 20 EXP, 25% rơi 1 nguyên liệu Bậc 1.
  - *Bại:* 25 Bạc, 5 Danh Vọng, 10 EXP, 10% rơi 1 nguyên liệu Bậc 1.
- **Mốc thưởng tuần Luận Kiếm:**
  - 10 trận thắng/tuần: **1 Chí Tôn Ngọc Tủy** + 1,000 Bạc + 100 Danh Vọng.
- **Thưởng Mùa Xếp Hạng (Season Ranking):**
  - Top 10 (Tông Sư Đỉnh Phong): **5 Chí Tôn Ngọc Tủy**, 20,000 Bạc, Danh hiệu độc quyền.
  - Top 5%: **3 Chí Tôn Ngọc Tủy**, 12,000 Bạc.
  - Top 20%: **2 Chí Tôn Ngọc Tủy**, 8,000 Bạc.
  - Top 50%: **1 Chí Tôn Ngọc Tủy**, 5,000 Bạc.

---

## 5. Thương Hội (NPC Shop) & Tiệm Luận Kiếm

### 5.1. Bán bản sao trang bị theo phẩm chất (Ô 1 & Ô 2)
- **Ô 1:** Bản sao Phàm (45%), Lương (40%), Tinh (15%).
- **Ô 2:** Bản sao Lương (50%), Tinh (35%), Tuyệt (15%).
- Giá niêm yết: Phàm: `200 Bạc` · Lương: `500 Bạc` · Tinh: `1,200 Bạc` · Tuyệt: `3,000 Bạc`.

### 5.2. Tiệm Luận Kiếm (`pvp_reputation_shop`)
Dùng tiền tệ **Danh Vọng** (`reputation`) tích lũy qua thi đấu PvP:
- **Chí Tôn Ngọc Tủy:** `500 Danh Vọng` (Giới hạn mua: **1 viên / tuần**).
- **Rương Chất Dẫn Ngẫu Nhiên:** `250 Danh Vọng` (Giới hạn: 3 rương / tuần).
- **Rương Nguyên Liệu Bậc 3:** `100 Danh Vọng` (Giới hạn: 5 rương / tuần).

### 5.3. Tính năng Chuyển Đổi Vật Liệu (`material_exchange`)
Mở khóa tại Hiệp Khách Cấp 12:
- **Nâng bậc cùng nhóm:** `3 nguyên liệu Bậc N → 1 nguyên liệu Bậc N+1`. Phí gia công: 50 Bạc (Bậc 1→2), 150 Bạc (Bậc 2→3).
- **Đổi chéo nhóm cùng bậc (chấp nhận lỗ):** `2 nguyên liệu nhóm A → 1 nguyên liệu nhóm B`. Phí: 25 / 75 / 200 Bạc theo bậc.

### 5.4. Bán lại bản sao trang bị (Sell Back)
- **Giá Bạc thu hồi:** `30% giá gốc theo phẩm chất` + `15% tổng Bạc đã đầu tư nâng Trọng`.
- **Hoàn trả vật liệu:** Hoàn lại **50%** tổng số nguyên liệu thường và Chất Dẫn Đột Phá đã tiêu hao nâng Trọng cho bản sao đó.

---

## 6. Dung Lượng Tàng Bảo Các (Kho Chứa Đồ)

- Mở khóa tại Hiệp Khách **Cấp 1**.
- Sức chứa khởi đầu: **30 ô chứa**.
- Mở rộng thông qua phần thưởng Thăng Cấp Hiệp Khách (lên đến 100 ô tại Cấp 30). Người chơi cũng có thể chi `2,000 Bạc` để mua thêm từng cụm 5 ô nếu cần kíp.

---

## 7. Cấu Trúc Tệp Dữ Liệu Liên Kết

1. `tools/gemini/out/economy/materials.json`: Danh mục 17 nguyên liệu (16 nguyên liệu thường/chất dẫn theo 4 nhóm + 1 `chi_ton_ngoc_tuy` phổ quát độc quyền PvP).
2. `tools/gemini/out/economy/upgrade_costs.json`: Chi phí 1→9 theo 6 cột phẩm chất (từ Phàm đến Chí Tôn).
3. `tools/gemini/out/economy/shop.json`: Tỷ lệ xuất hiện bản sao theo phẩm chất, bảng giá, chuyển đổi vật liệu và Tiệm Luận Kiếm.
4. `tools/gemini/out/economy/rewards.json`: Tỷ trọng 4 nhóm vật liệu, phần thưởng ải PvE/PvP, mốc tuần và thưởng mùa PvP.

---

## 8. 3 Câu Hỏi Mở Dành Cho Product Owner

1. **Khóa bảo vệ nguyên liệu Chí Tôn:** Khi sở hữu `chi_ton_ngoc_tuy`, có cần cơ chế xác nhận 2 bước khi tiến hành hợp thành Chí Tôn để tránh người chơi thao tác nhầm không?
2. **Hiệu ứng thông báo toàn server khi xuất hiện Chí Tôn:** Khi một người chơi hợp thành thành công 1 món đồ phẩm Chí Tôn, game có nên hiện dòng chữ chạy (triện vàng) vinh danh trên sảnh chính không?
3. **Reset Danh Vọng theo Mùa:** Điểm Danh Vọng PvP tích lũy có bị reset theo từng mùa giải Luận Kiếm (1 tháng/lần) hay được bảo lưu tích lũy vĩnh viễn?
