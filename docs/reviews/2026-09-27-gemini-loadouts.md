# Báo cáo Đánh giá và Cân bằng Đội hình Mẫu (Loadouts Review)

**Ngày thực hiện:** 2026-09-27  
**Phạm vi:** Tạo 10 đội hình mẫu mới (`server/battle-engine/samples/gemini/`) nhằm mở rộng tập kiểm thử cân bằng game từ 5 lên 15 đội hình; phân tích ma trận đối đầu chạy 200 trận mỗi cặp.

---

## 1. Bảng danh sách 10 Đội hình Mẫu mới

| File | Tên đội hình | Số ô dùng | Danh sách binh khí | Ý đồ lối chơi |
|---|---|---|---|---|
| `kim_cang_bach_luyen.json` | Kim Cang Tự - Bách Luyện (Thuần Quyền) | 19/20 | 6x Thiết Cốt Quyền Sáo | Tốc độ đánh cực nhanh (1.2s), tích nội lực liên tục (10 NL/s) để Hóa Kình miễn phí vô hiệu hóa mọi Tuyệt Kỹ địch. |
| `thai_thanh_kiem_khi.json` | Thái Thanh Quán - Kiếm Khí (Tam Kiếm Tứ Bút) | 20/20 | 3x Thanh Phong Kiếm, 4x Phán Quan Bút | 4 Bút liên tục Điểm Huyệt khóa thời gian hồi đối thủ kết hợp 3 Kiếm Hội Tâm cao xả Tuyệt Kỹ 3 nhát liên hoàn. |
| `liet_diem_hoa_hai.json` | Liệt Diễm Đường - Hỏa Hải (Tứ Đao) | 19/20 | 4x Liệt Diễm Đao, 2x Thiết Cốt Quyền Sáo | Tối đa hóa Liệt Hỏa với 4 Đao tích dồn dập (12 điểm/nhịp), liên tục bộc phát 300 sát thương kết hợp Tuyệt Kỹ diện rộng. |
| `huyen_doc_van_doc.json` | Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo) | 20/20 | 4x Thất Tinh Tiêu, 3x Tử Kim Trảo | Tích cực nhanh Kịch Độc (12 điểm/s) và Xuất Huyết (5.6 điểm/s) bộc phát dồn dập xuyên hoàn toàn Cương Khí, không dùng Tuyệt Kỹ. |
| `thiet_phu_khai_thien.json` | Thiết Phủ Bang - Khai Thiên (Tam Phủ) | 19/20 | 3x Khai Sơn Phủ | Đòn đánh vật lý cực nặng (240 Lực Đạo) kết hợp Phá Cương 150 và Tuyệt Kỹ 160% chém gục đối thủ sau khi vỡ khiên. |
| `hybrid_kim_cang_thiet_phu.json` | Kim Cang - Thiết Phủ Hợp Bích (Côn + Rìu) | 20/20 | 1x Phục Hổ Côn, 1x Khai Sơn Phủ, 2x Thiết Cốt Quyền Sáo, 1x Phán Quan Bút | Khống chế kép (Định Thân từ Côn, Điểm Huyệt từ Bút) tạo khoảng trống cho Rìu tung đòn Phá Cương kết liễu. |
| `hybrid_thai_thanh_liet_diem.json` | Thái Thanh - Liệt Diễm Hợp Bích (Kiếm + Đao) | 19/20 | 2x Thanh Phong Kiếm, 2x Liệt Diễm Đao, 2x Thiết Cốt Quyền Sáo | Kiếm Hội Tâm kích hoạt Khinh Công cho Đao chém nhanh gấp đôi, dồn Liệt Hỏa và Tuyệt Kỹ đa kích hạ gục mục tiêu. |
| `hybrid_han_son_bich_thuy.json` | Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Tiêu) | 20/20 | 2x Bích Thủy Cầm, 2x Hàn Ngọc Tiêu, 1x Phán Quan Bút | Âm công xuyên giáp kết hợp Trì Trệ toàn bộ đồ địch 1s và Điều Tức hồi 12 máu/s, bào mòn đối thủ mà bỏ qua Cương Khí. |
| `thai_thanh_tuyet_ky.json` | Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm + Côn) | 19/20 | 2x Phục Hổ Côn, 2x Thanh Phong Kiếm, 1x Thiết Cốt Quyền Sáo | All-in Tuyệt Kỹ: dồn toàn bộ nội lực tung Tuyệt Kỹ Định Thân (Côn 30 NL) và Trảm Kích 3 nhát (Kiếm 40 NL) áp đảo. |
| `hybrid_thiet_phu_liet_diem.json` | Thiết Phủ - Liệt Diễm Hợp Bích (Rìu + Đao) | 20/20 | 2x Khai Sơn Phủ, 2x Liệt Diễm Đao, 2x Thiết Cốt Quyền Sáo | Rìu phá sạch Cương Khí để sát thương bộc phát Liệt Hỏa (300 điểm) và nhát chém Đao đâm thẳng vào Khí Huyết đối phương. |

---

## 2. Kết quả Ma trận Đối đầu (200 trận mỗi cặp)

### 2.1. Ma trận 10 Đội hình Mới (10x10)

```
Tỉ lệ thắng của hàng (A) trước cột (B), 200 trận mỗi cặp

                                          #1      #2      #3      #4      #5      #6      #7      #8      #9     #10       TB
#1 Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo)       -    6.8%   86.3%   87.5%   82.8%  100.0%   99.3%   25.3%   10.0%   97.0%    66.1%
#2 Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Tiêu)   93.3%       -  100.0%  100.0%  100.0%  100.0%  100.0%   99.5%   99.0%  100.0%    99.1%
#3 Kim Cang - Thiết Phủ Hợp Bích (Côn + Rìu)   14.0%    0.0%       -   82.8%   92.0%    0.0%   41.8%   42.0%   16.8%   97.0%    42.9%
#4 Thái Thanh - Liệt Diễm Hợp Bích (Kiếm + Đao)   11.5%    0.0%   17.0%       -   78.8%    0.0%   85.0%   36.3%   14.8%   13.0%    28.5%
#5 Thiết Phủ - Liệt Diễm Hợp Bích (Rìu + Đao)   18.3%    0.0%    6.5%   21.5%       -    0.0%    4.0%    6.3%    3.0%   14.3%     8.2%
#6 Kim Cang Tự - Bách Luyện (Thuần Quyền)    0.0%    0.0%  100.0%  100.0%  100.0%       -  100.0%  100.0%  100.0%  100.0%    77.8%
#7 Liệt Diễm Đường - Hỏa Hải (Tứ Đao)    0.3%    0.0%   60.0%    9.5%   96.5%    0.0%       -   18.0%   15.8%   88.5%    32.1%
#8 Thái Thanh Quán - Kiếm Khí (Tam Kiếm Tứ Bút)   72.3%    0.5%   55.5%   68.3%   95.0%    0.0%   81.5%       -    8.5%   93.8%    52.8%
#9 Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm + Côn)   91.5%    0.0%   81.5%   83.0%   97.5%    0.0%   84.5%   93.8%       -   94.5%    69.6%
#10 Thiết Phủ Bang - Khai Thiên (Tam Phủ)    2.0%    0.0%    4.0%   87.0%   84.0%    0.0%   11.5%    6.8%    5.5%       -    22.3%

(hòa tính nửa trận thắng)
```

---

### 2.2. Ma trận Toàn bộ 15 Đội hình (5 gốc + 10 mới)

```
Tỉ lệ thắng của hàng (A) trước cột (B), 200 trận mỗi cặp

                                          #1      #2      #3      #4      #5      #6      #7      #8      #9     #10     #11     #12     #13     #14     #15       TB
#1 Huyền Độc Môn (Ám Khí + Trảo) [Gốc]     -    0.0%    0.0%   11.3%    4.5%    0.0%   63.0%  100.0%    3.0%   31.0%   92.5%    0.8%    0.0%    0.0%    0.0%    21.9%
#2 Huyền Độc Môn - Vạn Độc [Mới]        100.0%       -    6.8%   86.3%   87.5%   82.8%   99.0%  100.0%  100.0%   99.3%  100.0%   25.3%   10.0%   87.0%   97.0%    77.2%
#3 Hàn Sơn - Bích Thủy Hợp Bích [Mới]   100.0%   93.3%       -  100.0%  100.0%  100.0%  100.0%  100.0%  100.0%  100.0%  100.0%   99.5%   99.0%  100.0%  100.0%    99.4%
#4 Kim Cang - Thiết Phủ Hợp Bích [Mới]   88.5%   14.0%    0.0%       -   82.8%   92.0%   66.0%    0.0%   92.3%   41.8%   63.8%   42.0%   16.8%   98.8%   97.0%    56.8%
#5 Thái Thanh - Liệt Diễm Hợp Bích [Mới] 94.5%   11.5%    0.0%   17.0%       -   78.8%   56.5%    0.0%   76.8%   85.0%   38.8%   36.3%   14.8%   74.5%   13.0%    42.7%
#6 Thiết Phủ - Liệt Diễm Hợp Bích [Mới] 100.0%   18.3%    0.0%    6.5%   21.5%       -    0.0%    0.0%   71.8%    4.0%   36.8%    6.3%    3.0%   82.5%   14.3%    26.1%
#7 Kim Cang Tự (Quyền + Côn) [Gốc]       36.5%    1.0%    0.0%   36.0%   39.5%  100.0%       -    0.0%   63.3%   31.8%   20.0%   27.3%   27.3%   41.0%   72.0%    35.4%
#8 Kim Cang Tự - Bách Luyện [Mới]         0.0%    0.0%    0.0%  100.0%  100.0%  100.0%  100.0%       -  100.0%  100.0%  100.0%  100.0%  100.0%   77.8%  100.0%    77.0%
#9 Liệt Diễm Đường (Đao lửa) [Gốc]       97.5%    0.0%    0.0%   11.3%   24.8%   31.8%   32.8%    0.0%       -   46.8%   40.5%    1.0%    1.5%   54.3%   56.5%    28.5%
#10 Liệt Diễm Đường - Hỏa Hải [Mới]      71.5%    0.3%    0.0%   60.0%    9.5%   96.5%   63.3%    0.0%   49.5%       -   28.8%   18.0%   15.8%   96.8%   88.5%    42.7%
#11 Thái Thanh Quán (Kiếm + Hóa Kình) [Gốc] 7.3% 0.0%    0.0%   29.5%   62.0%   61.5%   81.5%    0.0%   66.8%   67.0%       -   54.5%   70.8%   51.0%   60.5%    43.7%
#12 Thái Thanh Quán - Kiếm Khí [Mới]     99.3%   72.3%    0.5%   55.5%   68.3%   95.0%   72.0%    0.0%   97.3%   81.5%   48.3%       -    8.5%   96.5%   93.8%    63.5%
#13 Thái Thanh - Tuyệt Kỹ Liên Hoàn [Mới]100.0%  91.5%    0.0%   81.5%   83.0%   97.5%   71.3%    0.0%   99.0%   84.5%   28.3%   93.8%       -   94.8%   94.5%    72.8%
#14 Thiết Phủ Bang (Rìu + Cầm) [Gốc]    100.0%   13.3%    0.0%    0.0%   25.5%   16.0%   59.5%   22.3%   44.0%    0.5%   48.5%    4.5%    5.5%       -   14.3%    25.3%
#15 Thiết Phủ Bang - Khai Thiên [Mới]   100.0%    2.0%    0.0%    4.0%   87.0%   84.0%   27.5%    0.0%   43.5%   11.5%   38.3%    6.8%    5.5%   89.5%       -    35.7%

(hòa tính nửa trận thắng)
```

### 2.3. Bảng tổng hợp Tỉ lệ Thắng Trung bình trong ma trận 15x15

| STT | Đội hình | Loại | Tỉ lệ thắng TB | Nhóm đánh giá |
|---|---|---|---|---|
| #3 | **Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Tiêu)** | Mới | **99.4%** | Quá mạnh (> 65%) |
| #2 | **Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo)** | Mới | **77.2%** | Quá mạnh (> 65%) |
| #8 | **Kim Cang Tự - Bách Luyện (Thuần Quyền)** | Mới | **77.0%** | Quá mạnh (> 65%) |
| #13 | **Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm + Côn)** | Mới | **72.8%** | Quá mạnh (> 65%) |
| #12 | Thái Thanh Quán - Kiếm Khí (Tam Kiếm Tứ Bút) | Mới | **63.5%** | Cân bằng (35%–65%) |
| #4 | Kim Cang - Thiết Phủ Hợp Bích (Côn + Rìu) | Mới | **56.8%** | Cân bằng (35%–65%) |
| #11 | Thái Thanh Quán (Kiếm + Hóa Kình) | Gốc | **43.7%** | Cân bằng (35%–65%) |
| #5 | Thái Thanh - Liệt Diễm Hợp Bích (Kiếm + Đao) | Mới | **42.7%** | Cân bằng (35%–65%) |
| #10 | Liệt Diễm Đường - Hỏa Hải (Tứ Đao) | Mới | **42.7%** | Cân bằng (35%–65%) |
| #15 | Thiết Phủ Bang - Khai Thiên (Tam Phủ) | Mới | **35.7%** | Cân bằng (35%–65%) |
| #7 | Kim Cang Tự (Quyền + Côn) | Gốc | **35.4%** | Cân bằng (35%–65%) |
| #9 | **Liệt Diễm Đường (Đao lửa)** | Gốc | **28.5%** | Quá yếu (< 35%) |
| #6 | **Thiết Phủ - Liệt Diễm Hợp Bích (Rìu + Đao)** | Mới | **26.1%** | Quá yếu (< 35%) |
| #14 | **Thiết Phủ Bang (Rìu + Cầm)** | Gốc | **25.3%** | Quá yếu (< 35%) |
| #1 | **Huyền Độc Môn (Ám Khí + Trảo)** | Gốc | **21.9%** | Quá yếu (< 35%) |

---

## 3. Phân tích Chi tiết & Quan sát Cân bằng

### 3.1. Vật phẩm xuất hiện ở hầu hết các đội hình mạnh
1. **`o_tam_y` (Ô Tàm Y):**
   - Cung cấp **600 Cương Khí** ngay giây thứ 0 và **8% Hộ Thân**. Với lượng máu cơ bản là 1200 HP, 600 Cương Khí tương đương việc tăng thêm **50% lượng máu hiệu dụng** trước mọi đòn đánh thường. Đội hình thiếu Ô Tàm Y rất dễ bị sốc sát thương trong 5–8 giây đầu.
2. **`boi_nguyen_dan` (Bồi Nguyên Đan):**
   - Hồi ngay **300 HP** khi máu rơi xuống dưới 40% (480 HP). Chỉ tốn đúng 1 ô (1x1) nhưng mang lại giá trị tương đương 25% lượng máu tối đa, biến nó thành món đồ phòng thủ bắt buộc ở mọi đội hình.
3. **`thai_thanh_tam_kinh` (Thái Thanh Tâm Kinh):**
   - Cực kỳ mạnh vì cấp **20 nội lực đầu trận** và giảm chi phí Hóa Kình đi 20 (chỉ còn 40 thay vì 60). Điều này cho phép người chơi Hóa Kình ngay lần đầu tiên địch tung Tuyệt Kỹ, trực tiếp khóa đòn của địch và phản đòn.

### 3.2. Vật phẩm kém hiệu quả hoặc khó sử dụng
1. **`khai_son_phu` (Khai Sơn Phủ):**
   - Tốc xuất chiêu quá chậm (**8.0 giây**). Mặc dù có Lực Đạo cao (240) và Phá Cương (150), nhưng trong meta có nhiều hiệu ứng khống chế (Điểm Huyệt 1s từ Bút, Trì Trệ 1s từ Tiêu, hoặc Định Thân 1s khi bị Hóa Kình), chu kỳ của Rìu thường bị kéo dài lên **10–14 giây**. Rìu gần như chỉ chém được 1–2 nhát trong cả trận trước khi chết.
2. **`liet_diem_dao` (Liệt Diễm Đao):**
   - Tốc xuất chiêu **6.0 giây** và Tuyệt Kỹ tốn tới **40 nội lực**. Mặc dù có hiệu ứng Liệt Hỏa (3 điểm thường, 15 điểm Tuyệt Kỹ), nhưng Liệt Hỏa bộc phát (300 sát thương) lại **bị Cương Khí đỡ được** (theo `combat.md`). Khi gặp đối thủ có Ô Tàm Y (600 khiên), sát thương bộc phát Liệt Hỏa bị khiên hấp thụ hoàn toàn, làm giảm mạnh độ đe dọa.

### 3.3. Phân tích nguyên nhân các đội hình có tỉ lệ thắng bất thường

#### A. Nhóm Quá Mạnh (> 65%):
- **`hybrid_han_son_bich_thuy` (99.4%):**
  - *Nguyên nhân:* Bộ đôi Bích Thủy Cầm + Hàn Ngọc Tiêu đều có thuộc tính `"ignoreShield": true` (bỏ qua hoàn toàn 600 Cương Khí của Ô Tàm Y). Cầm tích dồn vĩnh viễn 6 Điều Tức mỗi 5s (2 Cầm = 12 Điều Tức/nhịp), giúp nhân vật hồi máu liên tục từ 12 đến 36 HP mỗi giây. Đồng thời, 2 Hàn Ngọc Tiêu liên tục áp trạng thái Trì Trệ lên **toàn bộ đồ địch** (`enemy_all_items`), làm chậm tốc độ xuất chiêu và tích nội lực của địch xuống 50%. Địch bị giảm nửa DPS, trong khi bên này vừa đánh thẳng vào máu vừa tự hồi phục cực nhanh.
- **`huyen_doc_van_doc` (77.2%):**
  - *Nguyên nhân:* 4 Thất Tinh Tiêu (2.0s, +6 Kịch Độc) tạo ra 12 điểm Kịch Độc/giây. Chỉ sau 4.2 giây đã bộc phát 240 sát thương. Cùng lúc 3 Tử Kim Trảo (1.6s, +3 Xuất Huyết) kích hoạt bộc phát 270 sát thương. Cả Kịch Độc và Xuất Huyết bộc phát đều **xuyên Cương Khí**, khiến đối thủ bốc hơi lượng máu 1200 HP chỉ sau 3–4 chu kỳ bộc phát (khoảng 15s) mà không cần phá vỡ 600 Cương Khí.
- **`kim_cang_bach_luyen` (77.0%):**
  - *Nguyên nhân:* Thuần Quyền Sáo (6 chiếc) ra đòn mỗi 1.2s tạo ra 10 nội lực/giây. Do không có Tuyệt Kỹ để tiêu hao nội lực, nhân vật luôn tràn đầy 100 nội lực. Khi đối thủ tung Tuyệt Kỹ (Kiếm, Côn, Đao, Rìu), Tâm Kinh giúp Hóa Kình với chi phí rẻ (40 nội lực), bẻ gãy Tuyệt Kỹ và Định Thân địch 1s. Chỉ mất 4s là Quyền hồi lại 40 nội lực, biến đội hình này thành cỗ máy phản đòn tuyệt đối trước mọi đội hình dùng Tuyệt Kỹ (thắng 100% trước 9/14 đội). Chỉ chịu thua trước Huyền Độc và Cầm/Tiêu vì các đội này không dùng Tuyệt Kỹ và đánh xuyên giáp.
- **`thai_thanh_tuyet_ky` (72.8%):**
  - *Nguyên nhân:* Phối hợp Côn (Tuyệt Kỹ tốn 30 nội lực, rẻ nhất game) gây Định Thân 1s và Kiếm (Tuyệt Kỹ 3 nhát x 90% = 270% sát thương, Hội Tâm 20%). Nhờ Tâm Kinh cấp 20 nội lực đầu trận, Côn tung Tuyệt Kỹ gần như ngay lập tức, khóa chân địch và tạo tiền đề cho Kiếm dồn sát thương kết liễu.

#### B. Nhóm Quá Yếu (< 35%):
- **`hybrid_thiet_phu_liet_diem` (26.1%) & `thiet_phu.json` gốc (25.3%):**
  - *Nguyên nhân:* Tốc độ của Rìu (8s) và Đao (6s) quá chậm. Khi không có các món khống chế hoặc tăng tốc, đội hình bị động hoàn toàn trước các đội hình đánh nhanh, bị cướp lượt và gục ngã trước khi tung được đòn đánh thứ hai.
- **`huyen_doc.json` gốc (21.9%):**
  - *Nguyên nhân:* Đội hình gốc chia vụn tài nguyên: chỉ có 2 Tiêu và 2 Trảo (quá ít để tích đủ 50 điểm bộc phát nhanh chóng trước khi đối thủ đập vỡ khiên), lại thêm 2 Hàn Ngọc Tiêu (sát thương cơ bản chỉ 38). Sát thương dàn trải không đủ dứt điểm đối thủ trong meta mới.
- **`liet_diem.json` gốc (28.5%):**
  - *Nguyên nhân:* Dùng 3 Đao nhưng Liệt Hỏa bị cản bởi Cương Khí. Khi đối phương có Ô Tàm Y + Bồi Nguyên Đan, lượng sát thương của Đao không đủ xuyên qua để kết liễu trước khi bị các đòn xuyên giáp hoặc phản đòn đánh bại.

---

## 4. Các điểm Gợi ý Cân bằng cho Đội ngũ Thiết kế

1. **Điều chỉnh cơ chế Bỏ qua Cương Khí (`ignoreShield`):**
   - Thuộc tính `ignoreShield` trên Bích Thủy Cầm và Hàn Ngọc Tiêu hiện đang quá mạnh vì vô hiệu hóa hoàn toàn giá trị của Ô Tàm Y (600 giáp bị vô hiệu hóa). Nên cân nhắc đổi `ignoreShield` thành sát thương chuẩn một phần (ví dụ: xuyên 50% Cương Khí) hoặc giảm Lực Đạo của Bích Thủy Cầm (từ 120 xuống 80–90).
2. **Giới hạn số tầng cộng dồn Điều Tức (`regen`):**
   - Hiện tại Điều Tức cộng dồn vĩnh viễn không giới hạn và không suy giảm (mỗi đòn Cầm +6 tầng). Cần đặt trần tối đa (ví dụ tối đa 12 hoặc 18 tầng) hoặc cho Điều Tức tự giảm 1–2 tầng mỗi giây để tránh việc đấu thủ hồi máu tới 30–40 HP/s ở nửa sau trận đấu.
3. **Cơ chế Hóa Kình đối với đội không dùng Tuyệt Kỹ:**
   - Đội thuần vũ khí đánh nhanh (như 6 Quyền) đang tích nội lực quá dư thừa và Hóa Kình liên tục không tốn tài nguyên. Nên cân nhắc tăng thời gian hồi giữa các lần Hóa Kình (parry cooldown, ví dụ tối thiểu cách nhau 3–4 giây) để các đội hình Tuyệt Kỹ vẫn có cơ hội ra đòn.
4. **Tăng tốc độ cho Binh khí Nặng (Rìu, Đao):**
   - Khai Sơn Phủ (8.0s) và Liệt Diễm Đao (6.0s) cần được hỗ trợ thêm về khả năng chống khống chế hoặc giảm nhẹ thời gian hồi chiêu cơ bản (ví dụ Rìu xuống 7.0s, Đao xuống 5.0s) để có thể cạnh tranh trong nhịp độ trận đấu 20–30 giây.

---

## 5. Những điểm Chưa Chắc Chắn (Unsure Notes)

1. **Hiệu ứng Bộ Môn Phái (Sect Set Bonuses):**
   - Theo thiết kế, game sẽ có hiệu ứng bộ khi xếp 2/4/6 món cùng phái liền kề (`combat.md`). Hiện tại engine chưa cài đặt cơ chế này, nên các đội hình thuần phái (như 6 Quyền Kim Cang, 4 Đao Liệt Diễm) chưa nhận được sức mạnh cộng hưởng môn phái như thiết kế dự kiến.
2. **Trọng (Tier):**
   - Toàn bộ 10 đội hình mới đều được thiết lập ở Trọng 1 (`tier = 1`) theo đúng yêu cầu đề bài. Ở các mốc Trọng cao hơn (Trọng 3, 6, 9 với hệ số nhân Lực Đạo và Đột Phá), tương quan sức mạnh giữa vũ khí đơn kích lớn (Rìu) và vũ khí đa kích (Kiếm, Quyền) có thể sẽ có sự dịch chuyển đáng kể.
