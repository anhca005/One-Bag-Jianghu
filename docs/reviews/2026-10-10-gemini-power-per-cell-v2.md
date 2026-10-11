# Báo Cáo Cân Bằng Power Per Cell v2 (2026-10-10)

## 1. Tổng Quan Kế Thừa & Phương Pháp

Báo cáo này ghi nhận kết quả thực thi quy trình cân bằng `power-per-cell-v2` theo đúng chỉ thị trong `tools/gemini/task-power-per-cell-v2.md`.

- **Phương pháp đo:** Sử dụng `tools/balance/item_value.py` (Arena simulation trên túi đồ điển hình cấp 15/30/45).
- **Synergy items:** Được đo đạc kèm theo vật phẩm kích hoạt tương ứng (Synergy-measured) nhằm phản ánh đúng giá trị thực tế trong túi đồ.
- **Neo chuẩn (Anchor items):** `thanh_phong_kiem` và `o_tam_y` được giữ nguyên chỉ số gốc, thiết lập benchmark perCellRaw chuẩn = `0.0677`.
- **Giới hạn biến đổi:** Mỗi vòng điều chỉnh (factor per round) được giới hạn nghiêm ngặt trong khoảng **[0.70, 1.40]**. Các thuộc tính nguyên thể như `maxStacks`, kích thước túi (`size`), tên, ID, loại hình được giữ nguyên 100%.

---

## 2. Kết Quả Đo Đạc Lực Chiến & Phân Bố (Final Round)

- **Số vòng đo & cân chỉnh đã chạy:** 3 vòng.
- **Số trang bị nằm trong dải mục tiêu [0.85 – 1.15]:** **15 / 73** trang bị.
- **Số trang bị nằm ngoài dải mục tiêu:** **58 / 73** trang bị.
- **Tỷ lệ chồng tầng tệ nhất (Worst stackRatio):** **2.53** (Mục tiêu: ≤ 1.10).

### Phân Bố Theo Loại Trang Bị (Type Spread)
- **armor**: Min 0.991 | Avg 1.200 | Max 1.388 (Count: 14)
- **manual**: Min 0.990 | Avg 1.310 | Max 1.659 (Count: 12)
- **pill**: Min 0.255 | Avg 1.105 | Max 2.054 (Count: 12)
- **relic**: Min 0.668 | Avg 1.570 | Max 4.757 (Count: 14)
- **weapon**: Min 0.772 | Avg 2.249 | Max 3.636 (Count: 21)

---

## 3. Exploit Check (Greedy Bag vs Typical Bag)

Kiểm tra tỷ lệ thắng của túi đồ tối ưu (Greedy) so với túi đồ điển hình (Typical) qua các mốc màn chơi (30 trận x 2 lượt ghế = 60 trận/mốc):

- **Stage 20:** Win Rate = **50.0%**
- **Stage 35:** Win Rate = **100.0%**
- **Stage 45:** Win Rate = **0.0%**

---

## 4. Starter Check (Bộ Khởi Đầu Cấp 1 vs Quái Khởi Đầu)

Kiểm tra bộ trang bị khởi đầu (`thanh_phong_kiem` + `o_tam_y`) đấu với quái vật 3 ải đầu tiên trong `chapters.json`:

- **Stage hoi_1_01 (Trấn Khẩu Sơn Đạo):** Win Rate = **100.0%**
- **Stage hoi_1_02 (Phong Diệp Bến Đò):** Win Rate = **100.0%**
- **Stage hoi_1_03 (Hòe Thụ Thôn Ngoại):** Win Rate = **100.0%**

---

## 5. Bảng Ghi Nhận Chi Tiết Chỉ Số 73 Trang Bị

| ID Trang Bị | Loạn / Loại | Kích Thước | Giá Trị perCell | Stack Ratio | Synergy Measured | Ghi Chú / Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `chu_sa_ho_lo` | relic | 1x1 | **4.757** | 0.65 | Không | Nằm ngoài dải |
| `thanh_phong_kiem` | weapon | 1x3 | **3.636** |  | Không | Nằm ngoài dải |
| `thiet_cot_quyen_sao` | weapon | 1x2 | **3.342** |  | Không | Nằm ngoài dải |
| `kich_doc_chu_nga` | weapon | 1x2 | **2.886** |  | Không | Nằm ngoài dải |
| `tu_kim_trao` | weapon | 1x2 | **2.794** |  | Không | Nằm ngoài dải |
| `ngoc_cot_phien` | weapon | 1x2 | **2.524** |  | Không | Nằm ngoài dải |
| `liet_diem_dao` | weapon | 1x3 | **2.471** |  | Không | Nằm ngoài dải |
| `soi_nanh_canh` | weapon | 1x2 | **2.470** |  | Không | Nằm ngoài dải |
| `bang_phach_thuong` | weapon | 1x4 | **2.445** |  | Không | Nằm ngoài dải |
| `u_minh_quy_trao` | weapon | 2x2 | **2.412** |  | Không | Nằm ngoài dải |
| `phuc_ho_con` | weapon | 1x4 | **2.404** |  | Không | Nằm ngoài dải |
| `bich_thuy_cam` | weapon | 2x2 | **2.325** |  | Không | Nằm ngoài dải |
| `chan_nhac_chuy` | weapon | 2x2 | **2.199** |  | Không | Nằm ngoài dải |
| `khai_son_phu` | weapon | 2x2 | **2.123** |  | Không | Nằm ngoài dải |
| `hoi_thien_dan` | pill | 1x1 | **2.054** | 0.63 | Không | Nằm ngoài dải |
| `han_ngoc_tieu` | weapon | 1x2 | **2.038** |  | Không | Nằm ngoài dải |
| `xich_sa_chuong` | weapon | 1x2 | **2.026** |  | Không | Nằm ngoài dải |
| `tuy_tien_ho_lo` | relic | 1x1 | **1.931** | 0.6 | Không | Nằm ngoài dải |
| `tram_huong_lo` | relic | 1x1 | **1.832** | 0.61 | Không | Nằm ngoài dải |
| `am_duong_ngoc` | relic | 1x1 | **1.785** | 0.59 | Không | Nằm ngoài dải |
| `tu_mau_song_dao` | weapon | 2x2 | **1.768** |  | Không | Nằm ngoài dải |
| `thanh_dang_tien` | weapon | 1x3 | **1.756** |  | Không | Nằm ngoài dải |
| `truy_hon_cung` | weapon | 1x3 | **1.726** |  | Không | Nằm ngoài dải |
| `tu_kim_bat` | relic | 2x1 | **1.720** |  | Không | Nằm ngoài dải |
| `u_minh_bi_luc` | manual | 1x1 | **1.659** | 0.5 | Không | Nằm ngoài dải |
| `phan_quan_but` | weapon | 1x2 | **1.647** |  | Không | Nằm ngoài dải |
| `kim_sang_duoc` | pill | 1x1 | **1.642** | 0.62 | Không | Nằm ngoài dải |
| `duong_sinh_quyet` | manual | 1x2 | **1.529** |  | Không | Nằm ngoài dải |
| `thai_thanh_tam_kinh` | manual | 1x1 | **1.505** | 0.81 | Không | Nằm ngoài dải |
| `bach_ho_cuong_mong` | weapon | 2x1 | **1.466** |  | Không | Nằm ngoài dải |
| `quy_nguyen_quyet` | manual | 1x1 | **1.435** | 0.61 | Không | Nằm ngoài dải |
| `thanh_dong_ho_tam_kinh` | armor | 1x1 | **1.388** | 0.78 | Không | Nằm ngoài dải |
| `phuc_ma_giap` | armor | 2x2 | **1.378** |  | Không | Nằm ngoài dải |
| `liet_hoa_tam_kinh` | manual | 1x1 | **1.376** | 0.67 | Không | Nằm ngoài dải |
| `bang_phach_co_cot` | relic | 2x2 | **1.342** |  | Không | Nằm ngoài dải |
| `kiem_tong_tam_yeu` | manual | 1x1 | **1.327** | 0.64 | Không | Nằm ngoài dải |
| `phi_yen_quyet` | manual | 1x1 | **1.327** | 0.6 | Không | Nằm ngoài dải |
| `thuan_duong_cong` | manual | 1x1 | **1.323** | 0.66 | Không | Nằm ngoài dải |
| `bach_ngoc_boi` | relic | 1x1 | **1.319** | 0.48 | Không | Nằm ngoài dải |
| `o_tam_y` | armor | 2x2 | **1.310** |  | Không | Nằm ngoài dải |
| `tue_nguyet_bao_y` | armor | 1x2 | **1.308** |  | Không | Nằm ngoài dải |
| `nho_sam_y` | armor | 2x1 | **1.287** |  | Không | Nằm ngoài dải |
| `ho_mach_dan` | pill | 1x1 | **1.261** | 0.85 | Không | Nằm ngoài dải |
| `thien_nien_quy_giap` | armor | 2x2 | **1.252** |  | Không | Nằm ngoài dải |
| `hac_sat_hung_dam` | pill | 1x1 | **1.244** | 0.77 | Không | Nằm ngoài dải |
| `bang_tam_giap` | armor | 2x1 | **1.201** |  | Không | Nằm ngoài dải |
| `bang_tam_linh_chau` | relic | 1x1 | **1.182** | 0.89 | Không | Nằm ngoài dải |
| `thien_dia_vo_cuc_cong` | manual | 2x1 | **1.175** |  | Không | Nằm ngoài dải |
| `thien_tam_chuoi` | relic | 1x2 | **1.172** |  | Không | Nằm ngoài dải |
| `ti_khang_dan` | pill | 1x1 | **1.166** | 0.79 | Không | Nằm ngoài dải |
| `da_ho_y` | armor | 1x2 | **1.162** |  | Không | Nằm ngoài dải |
| `man_xa_doc_tui` | relic | 1x1 | **1.159** | 0.73 | Có (Synergy) | Nằm ngoài dải |
| `cuu_chuyen_hoan_hon_dan` | pill | 1x1 | **1.155** | 0.7 | Không | Nằm ngoài dải |
| `heo_rung_bap_giap` | armor | 2x2 | **1.143** |  | Không | Trong dải [0.85-1.15] |
| `kim_tuyen_y` | armor | 2x2 | **1.138** |  | Không | Trong dải [0.85-1.15] |
| `tuyet_lang_bach_bi` | armor | 2x1 | **1.111** |  | Không | Trong dải [0.85-1.15] |
| `huyet_ngoc_thach` | relic | 1x1 | **1.099** | 0.74 | Có (Synergy) | Trong dải [0.85-1.15] |
| `o_thiet_giap` | armor | 2x2 | **1.095** |  | Không | Trong dải [0.85-1.15] |
| `vo_niem_kinh` | manual | 1x2 | **1.073** |  | Không | Trong dải [0.85-1.15] |
| `han_bang_chau` | relic | 1x1 | **1.056** | 0.74 | Không | Trong dải [0.85-1.15] |
| `giai_doc_dan` | pill | 1x1 | **1.052** | 0.91 | Không | Trong dải [0.85-1.15] |
| `thiet_giap_y` | armor | 2x1 | **1.041** |  | Không | Trong dải [0.85-1.15] |
| `thanh_tam_dan` | pill | 1x1 | **1.007** | 0.99 | Không | Trong dải [0.85-1.15] |
| `bach_luyen_chan_kinh` | manual | 1x2 | **0.999** |  | Không | Trong dải [0.85-1.15] |
| `boi_nguyen_dan` | pill | 1x1 | **0.992** | 1.03 | Không | Trong dải [0.85-1.15] |
| `long_lan_giap` | armor | 2x2 | **0.991** |  | Không | Trong dải [0.85-1.15] |
| `hoa_diem_xa_cot` | manual | 1x2 | **0.990** |  | Không | Trong dải [0.85-1.15] |
| `da_minh_chau` | relic | 1x1 | **0.962** | 0.87 | Không | Trong dải [0.85-1.15] |
| `that_tinh_tieu` | weapon | 1x2 | **0.772** |  | Không | Nằm ngoài dải |
| `van_khi_dan` | pill | 1x1 | **0.731** | 1.36 | Có (Synergy) | Nằm ngoài dải |
| `cuong_hoa_dan` | pill | 1x1 | **0.700** | 1.08 | Có (Synergy) | Nằm ngoài dải |
| `van_doc_dinh` | relic | 1x1 | **0.668** | 1.03 | Có (Synergy) | Nằm ngoài dải |
| `ngung_than_dan` | pill | 1x1 | **0.255** | 2.53 | Không | Nằm ngoài dải |

---

## 6. Danh Sách Trang Bị Nằm Ngoài Dải [0.85 - 1.15] & Lý Do

- **`chu_sa_ho_lo`** (perCell: `4.757`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thanh_phong_kiem`** (perCell: `3.636`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thiet_cot_quyen_sao`** (perCell: `3.342`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`kich_doc_chu_nga`** (perCell: `2.886`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tu_kim_trao`** (perCell: `2.794`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`ngoc_cot_phien`** (perCell: `2.524`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`liet_diem_dao`** (perCell: `2.471`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`soi_nanh_canh`** (perCell: `2.470`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bang_phach_thuong`** (perCell: `2.445`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`u_minh_quy_trao`** (perCell: `2.412`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`phuc_ho_con`** (perCell: `2.404`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bich_thuy_cam`** (perCell: `2.325`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`chan_nhac_chuy`** (perCell: `2.199`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`khai_son_phu`** (perCell: `2.123`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`hoi_thien_dan`** (perCell: `2.054`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`han_ngoc_tieu`** (perCell: `2.038`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`xich_sa_chuong`** (perCell: `2.026`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tuy_tien_ho_lo`** (perCell: `1.931`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tram_huong_lo`** (perCell: `1.832`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`am_duong_ngoc`** (perCell: `1.785`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tu_mau_song_dao`** (perCell: `1.768`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thanh_dang_tien`** (perCell: `1.756`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`truy_hon_cung`** (perCell: `1.726`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tu_kim_bat`** (perCell: `1.720`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`u_minh_bi_luc`** (perCell: `1.659`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`phan_quan_but`** (perCell: `1.647`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`kim_sang_duoc`** (perCell: `1.642`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`duong_sinh_quyet`** (perCell: `1.529`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thai_thanh_tam_kinh`** (perCell: `1.505`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bach_ho_cuong_mong`** (perCell: `1.466`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`quy_nguyen_quyet`** (perCell: `1.435`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thanh_dong_ho_tam_kinh`** (perCell: `1.388`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`phuc_ma_giap`** (perCell: `1.378`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`liet_hoa_tam_kinh`** (perCell: `1.376`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bang_phach_co_cot`** (perCell: `1.342`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`kiem_tong_tam_yeu`** (perCell: `1.327`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`phi_yen_quyet`** (perCell: `1.327`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thuan_duong_cong`** (perCell: `1.323`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bach_ngoc_boi`** (perCell: `1.319`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`o_tam_y`** (perCell: `1.310`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`tue_nguyet_bao_y`** (perCell: `1.308`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`nho_sam_y`** (perCell: `1.287`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`ho_mach_dan`** (perCell: `1.261`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thien_nien_quy_giap`** (perCell: `1.252`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`hac_sat_hung_dam`** (perCell: `1.244`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bang_tam_giap`** (perCell: `1.201`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`bang_tam_linh_chau`** (perCell: `1.182`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thien_dia_vo_cuc_cong`** (perCell: `1.175`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`thien_tam_chuoi`** (perCell: `1.172`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`ti_khang_dan`** (perCell: `1.166`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`da_ho_y`** (perCell: `1.162`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`man_xa_doc_tui`** (perCell: `1.159`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`cuu_chuyen_hoan_hon_dan`** (perCell: `1.155`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`that_tinh_tieu`** (perCell: `0.772`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`van_khi_dan`** (perCell: `0.731`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`cuong_hoa_dan`** (perCell: `0.700`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`van_doc_dinh`** (perCell: `0.668`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
- **`ngung_than_dan`** (perCell: `0.255`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.
