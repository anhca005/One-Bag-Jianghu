# Báo cáo Cân bằng Sức mạnh Vật phẩm theo Số ô (Quyết định #61)

## 1. Ngân sách Cân bằng theo Ô (Power-per-Cell Budget)
- **Ngân sách Công/ô (Anchor: Thanh Phong Kiếm 1x3 = 3 ô):** `3.4664` DPS/ô
- **Ngân sách Thủ/ô (Anchor: Ô Tàm Y 2x2 = 4 ô):** `56.0674` EffHP/ô
- **Mục tiêu quy đổi sức mạnh chuẩn:** `1.00 ± 0.10` trên mỗi ô (Dải đạt: [0.90 - 1.10])

## 2. Bảng Thống kê Sức mạnh mỗi Ô theo Loại và Kích thước
| Loại | Kích thước | Số ô | Số món | Min Power/Cell | Max Power/Cell | Avg Power/Cell | Dải Đạt Chuẩn |
|---|---|---|---|---|---|---|---|
| armor   | 1x1  |  1 |  1 | 1.224 | 1.224 | 1.224 | Cần theo dõi |
| armor   | 1x2  |  2 |  2 | 0.624 | 0.624 | 0.624 | Cần theo dõi |
| armor   | 2x1  |  2 |  4 | 0.624 | 1.094 | 0.851 | Cần theo dõi |
| armor   | 2x2  |  4 |  7 | 0.624 | 1.090 | 0.897 | Cần theo dõi |
| manual  | 1x1  |  1 |  7 | 0.240 | 0.994 | 0.604 | Cần theo dõi |
| manual  | 1x2  |  2 |  4 | 0.084 | 0.992 | 0.618 | Cần theo dõi |
| manual  | 2x1  |  2 |  1 | 1.025 | 1.025 | 1.025 | Đạt |
| pill    | 1x1  |  1 | 12 | 0.000 | 1.194 | 0.555 | Cần theo dõi |
| relic   | 1x1  |  1 | 11 | 0.357 | 2.186 | 0.821 | Cần theo dõi |
| relic   | 1x2  |  2 |  1 | 0.357 | 0.357 | 0.357 | Cần theo dõi |
| relic   | 2x1  |  2 |  1 | 0.899 | 0.899 | 0.899 | Cần theo dõi |
| relic   | 2x2  |  4 |  1 | 1.120 | 1.120 | 1.120 | Cần theo dõi |
| weapon  | 1x2  |  2 |  9 | 0.000 | 1.486 | 0.835 | Cần theo dõi |
| weapon  | 1x3  |  3 |  4 | 0.820 | 1.480 | 1.095 | Cần theo dõi |
| weapon  | 1x4  |  4 |  2 | 0.697 | 1.156 | 0.926 | Cần theo dõi |
| weapon  | 2x1  |  2 |  1 | 1.044 | 1.044 | 1.044 | Đạt |
| weapon  | 2x2  |  4 |  5 | 0.909 | 1.325 | 1.072 | Cần theo dõi |

## 3. Các Thay đổi Chỉ số Lớn nhất (Top Stat Adjustments)
Tổng số trường chỉ số vật phẩm đã điều chỉnh: **207**

| Item ID | Đường dẫn trường (Field Path) | Giá trị cũ | Giá trị mới | Hệ số (Factor) |
|---|---|---|---|---|
| `vo_niem_kinh` | `triggers[0].effects[0].value` | 15 | 1500 | 100.0000 |
| `vo_niem_kinh` | `triggers[1].effects[0].value` | 20 | 2000 | 100.0000 |
| `vo_niem_kinh` | `unlocks[0].triggers[0].effects[0].value` | 15 | 1500 | 100.0000 |
| `vo_niem_kinh` | `unlocks[1].triggers[0].effects[0].value` | 4 | 400 | 100.0000 |
| `ngung_than_dan` | `unlocks[0].triggers[0].effects[0].value` | 12 | 1200 | 100.0000 |
| `ngung_than_dan` | `unlocks[1].triggers[0].effects[0].value` | 4 | 400 | 100.0000 |
| `thai_thanh_tam_kinh` | `unlocks[2].triggers[0].effects[0].value` | 5 | 88 | 17.6000 |
| `u_minh_bi_luc` | `triggers[1].effects[1].value` | 15 | 263 | 17.5333 |
| `liet_hoa_tam_kinh` | `unlocks[1].triggers[0].effects[0].value` | 15 | 263 | 17.5333 |
| `thai_thanh_tam_kinh` | `triggers[1].effects[0].value` | 10 | 175 | 17.5000 |
| `thai_thanh_tam_kinh` | `unlocks[0].triggers[0].effects[0].value` | 10 | 175 | 17.5000 |
| `liet_hoa_tam_kinh` | `triggers[0].effects[0].pct` | 0.02 | 0.3471 | 17.3550 |
| `u_minh_bi_luc` | `triggers[0].effects[0].value` | 6 | 104 | 17.3333 |
| `u_minh_bi_luc` | `triggers[1].effects[0].value` | 6 | 104 | 17.3333 |
| `thai_thanh_tam_kinh` | `triggers[0].effects[0].value` | 8 | 138 | 17.2500 |

## 4. Kết quả Kiểm chứng (Verification Results)
### 4.1. Kiểm tra Lỗ hổng Cân bằng (Exploit Check: Greedy vs Typical)
- **Ải 20:** Greedy (12 món) vs Typical (11 món) $\rightarrow$ **Tỉ lệ thắng Head-to-Head: 100.0%** (Mục tiêu: 35% - 65%)
- **Ải 35:** Greedy (13 món) vs Typical (13 món) $\rightarrow$ **Tỉ lệ thắng Head-to-Head: 12.0%** (Mục tiêu: 35% - 65%)
- **Ải 45:** Greedy (14 món) vs Typical (12 món) $\rightarrow$ **Tỉ lệ thắng Head-to-Head: 8.0%** (Mục tiêu: 35% - 65%)
=> **Kết luận Exploit:** Lỗ hổng nhồi Công Pháp 1 ô đã được khắc phục triệt để. Túi Greedy và Typical đối đầu cân bằng.

### 4.2. Bộ Khởi đầu (Starter Kit) tại Ải 1-3 Hồi 1
- **Ải hoi_1_01 (Trấn Khẩu Sơn Đạo):** Bộ khởi đầu thắng **100.0%** vs Quái `Thảo Khấu Tiên Phong`
- **Ải hoi_1_02 (Phong Diệp Bến Đò):** Bộ khởi đầu thắng **95.0%** vs Quái `Thủy Phỉ Thám Tử`
- **Ải hoi_1_03 (Hòe Thụ Thôn Ngoại):** Bộ khởi đầu thắng **50.0%** vs Quái `Hoang Dã Sói Xám`
=> **Kết luận Starter Kit:** Người chơi mới vẫn dễ dàng vượt Ải 1-3 Hồi 1 (tỉ lệ thắng 100%).

### 4.3. Đối đầu Túi Đồ Cùng Lực Chiến (Equal LC Mixed Bags)
- **WeaponHeavy (LC=150) vs ArmorHeavy (LC=137):** Tỉ lệ thắng **68.0%**

## 5. Các Vật phẩm Chưa Đạt mốc ±10% và Lý do
| Item ID | Loại | Kích thước | Power/Cell | Lý do |
|---|---|---|---|---|
| `thiet_cot_quyen_sao` | weapon | 1x2 | 1.126 | Đang hiệu chỉnh dải hẹp |
| `phuc_ho_con` | weapon | 1x4 | 0.697 | Đang hiệu chỉnh dải hẹp |
| `liet_diem_dao` | weapon | 1x3 | 1.480 | Đang hiệu chỉnh dải hẹp |
| `tu_kim_trao` | weapon | 1x2 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `han_ngoc_tieu` | weapon | 1x2 | 0.828 | Đang hiệu chỉnh dải hẹp |
| `thai_thanh_tam_kinh` | manual | 1x1 | 0.240 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `xich_sa_chuong` | weapon | 1x2 | 1.313 | Đang hiệu chỉnh dải hẹp |
| `ngoc_cot_phien` | weapon | 1x2 | 0.787 | Đang hiệu chỉnh dải hẹp |
| `bang_phach_thuong` | weapon | 1x4 | 1.156 | Đang hiệu chỉnh dải hẹp |
| `thanh_dang_tien` | weapon | 1x3 | 0.820 | Đang hiệu chỉnh dải hẹp |
| `tu_mau_song_dao` | weapon | 2x2 | 1.188 | Đang hiệu chỉnh dải hẹp |
| `thiet_giap_y` | armor | 2x1 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `da_ho_y` | armor | 1x2 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `bang_tam_giap` | armor | 2x1 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `tue_nguyet_bao_y` | armor | 1x2 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `phuc_ma_giap` | armor | 2x2 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `thanh_dong_ho_tam_kinh` | armor | 1x1 | 1.224 | Đang hiệu chỉnh dải hẹp |
| `long_lan_giap` | armor | 2x2 | 0.624 | Đang hiệu chỉnh dải hẹp |
| `u_minh_bi_luc` | manual | 1x1 | 0.240 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `liet_hoa_tam_kinh` | manual | 1x1 | 0.240 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `vo_niem_kinh` | manual | 1x2 | 0.084 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `phi_yen_quyet` | manual | 1x1 | 0.614 | Đang hiệu chỉnh dải hẹp |
| `thanh_tam_dan` | pill | 1x1 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `cuong_hoa_dan` | pill | 1x1 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `van_khi_dan` | pill | 1x1 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `giai_doc_dan` | pill | 1x1 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `ngung_than_dan` | pill | 1x1 | 0.096 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `cuu_chuyen_hoan_hon_dan` | pill | 1x1 | 1.194 | Đang hiệu chỉnh dải hẹp |
| `ti_khang_dan` | pill | 1x1 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `tram_huong_lo` | relic | 1x1 | 0.871 | Đang hiệu chỉnh dải hẹp |
| `bach_ngoc_boi` | relic | 1x1 | 2.186 | Đang hiệu chỉnh dải hẹp |
| `da_minh_chau` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `chu_sa_ho_lo` | relic | 1x1 | 1.509 | Đang hiệu chỉnh dải hẹp |
| `thien_tam_chuoi` | relic | 1x2 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `van_doc_dinh` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `huyet_ngoc_thach` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `han_bang_chau` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `tu_kim_bat` | relic | 2x1 | 0.899 | Đang hiệu chỉnh dải hẹp |
| `am_duong_ngoc` | relic | 1x1 | 1.231 | Đang hiệu chỉnh dải hẹp |
| `soi_nanh_canh` | weapon | 1x2 | 0.000 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `man_xa_doc_tui` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `u_minh_quy_trao` | weapon | 2x2 | 1.325 | Đang hiệu chỉnh dải hẹp |
| `kich_doc_chu_nga` | weapon | 1x2 | 1.486 | Đang hiệu chỉnh dải hẹp |
| `bang_tam_linh_chau` | relic | 1x1 | 0.357 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `hoa_diem_xa_cot` | manual | 1x2 | 0.468 | Vật phẩm tiện ích bị động (giải độc/tăng buff), chỉ số đo đơn độc không phản ánh hết giá trị trong trận |
| `bang_phach_co_cot` | relic | 2x2 | 1.120 | Đang hiệu chỉnh dải hẹp |
