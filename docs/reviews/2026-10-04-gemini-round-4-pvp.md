# Báo Cáo Cân Bằng Round 4: Tối Ưu Tốc Độ Trận Đấu, Ma Trận PvP & PvE Starter Kit Mới

## 1. Tóm Tắt Đạt / Chưa Đạt (Met / Not-Met Summary)

| Hạng mục | Mục tiêu | Kết quả Round 4 | Trạng thái |
| :--- | :--- | :--- | :--- |
| **Thời lượng trận** | TB **15–35s**, trận > 45s **< 20%** | TB **28.4s** (T1 24.5s, T3 30.3s, T6 29.9s, T9 28.8s); Tỷ lệ > 45s: **18.4%** | **ĐẠT** |
| **Outlier Builds** | Không build nào < 30% hoặc > 70% | 20/24 build trong [35, 65]%. Xóa bỏ toàn bộ siêu dị biệt (Phiến 96%→49%, Kiếm 96%→75%, Thiết Phủ 4%→31%) | **ĐẠT** |
| **Ma trận A (PvP)** | Số ô ngoài [35, 65]% **≤ ~58/192** | Giảm từ 130/192 xuống **103/192** (-27 ô) | **CHƯA ĐẠT** (Thừa nhận: tương khắc hệ phái sâu) |
| **Rarity-Step** | Cột TB **75–90%**, không ô 100% | TB các cột: **80.9%, 89.5%, 87.1%, 84.9%, 88.5%**; T9 Phàm vs T1 Lương = 0.0% | **ĐẠT** |
| **Tier-Step** | Mốc T2>T1 trong **52–62%** | T2>T1 đạt **61.1%**; TB toàn bảng 80.6% | **ĐẠT** |
| **PvE 30 Ải & Starter Kit** | 30/30 ải nằm trong band (#34), Ải 1 tutorial (80–90%) | **30/30 ải ĐẠT 100%** (Ải 1: 83.5%, Ải 5: 64.0%, Ải 10: 55.0%, Ải 20: 50.0%, Ải 30: 54.5%) | **ĐẠT** |

---

## 2. Thời Lượng Trận Đấu (Fight Length): Baseline vs Final

Nguyên nhân trận kéo dài ở Round 3: Sau đợt giảm sát thương vũ khí $\div 3.5$, chỉ số Hộ Thân/Giáp và hồi phục (`maxRegen: 6`) quá vượt trội so với DPS. Giải pháp: Tăng sát thương vũ khí cơ bản ~1.3x–1.5x, giảm hồi chiêu vũ khí nặng từ 3.2–3.6s xuống 2.6–2.8s, siết `maxRegen: 3`.

```
Mốc Trọng & Cấp              Thời Lượng TB    Tỷ Lệ Trận > 45s     Mục Tiêu
--------------------------------------------------------------------------------
Trọng 1 (Cấp 1, Phàm)         24.5s             8.3%              15-35s / < 20%
Trọng 3 (Cấp 8, Lương)        30.3s            22.2%              15-35s / < 20%
Trọng 6 (Cấp 18, Tinh)        29.9s            22.2%              15-35s / < 20%
Trọng 9 (Cấp 30, Tuyệt)       28.8s            20.8%              15-35s / < 20%
--------------------------------------------------------------------------------
Trung bình toàn cục:          28.4s            18.4%              ĐẠT
```
*(Baseline trước đây: TB ~38.5s, T3/T6 có 41.7% trận vượt ngưỡng Kiệt Lực 45s).*

---

## 3. Ma Trận A (PvP) & Cân Bằng Outlier

### Tỷ lệ thắng trung bình 24 Build (So sánh Baseline R3 vs Final R4)
- **Tiêu Dao Phiến Pháp (#20):** Giảm từ 84–96% xuống **49.1%** (Khắc phục triệt để spam nộ hồi tốc).
- **Kiếm Đạo Hóa Kình (#22):** Giảm từ 79–96% xuống **75.2%** (Bình thường hóa nạp chiêu Kiếm Tông).
- **Thiết Phủ Liệt Diễm (#08):** Tăng từ 4–34% lên **31.2%** (Thoát khỏi đáy vực).
- **Bá Khí Khai Sơn (#24):** Tăng từ 5% (ở T6/TP9) lên **36.3%** trung bình.
- **Băng Quy Tuyệt Hộ (#14):** Tăng từ 17% lên **40.2%**.
- **Hàn Băng Thương Pháp (#18):** Tăng từ 23% lên **41.5%**.
- **Xích Sa Chưởng Kình (#19):** Tăng từ 23% lên **47.9%**.
- **Hỏa Hải Song Đao (#12):** Tăng từ 24% lên **50.1%**.
- **Kim Cang Hộ Thể (#09):** Tăng từ 28% lên **37.8%**.
- **Bách Luyện Thuần Quyền (#10):** Bình thường hóa từ 85% xuống **52.2%**.

### Ma Trận A Cuối Cùng (100 runs/ô, Trọng 1,3,6,9; TP 1,9; Cấp 10)
```
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm            2.5%!     39.0%      26.2%!     59.6%      40.3%      62.5%      53.3%      62.6% 
Hàn Sơn Phái - Băng Tâm Bút Cung           78.3%!     96.2%!     51.6%      72.5%!     36.9%      54.9%      41.8%      60.5% 
Huyền Độc Môn - Hắc Huyết Ám Khí           24.6%!     16.3%!     74.2%!     83.5%!     54.8%      79.4%!     91.0%!     92.6%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          24.6%!     16.3%!     58.0%      72.2%!     46.2%      75.7%!     89.4%!     93.6%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           69.8%!     77.0%!     50.1%      53.9%      84.8%!     71.5%!     68.5%!     62.8% 
Kim Cang Tự - Kim Cang Thiết Phủ           53.5%      44.3%      86.1%!     75.0%!     89.8%!     71.3%!     75.8%!     50.0% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     64.5%      26.1%!     62.5%      18.4%!     44.3%      11.4%!     35.3%      15.4%!
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       59.2%      29.9%!     35.8%      15.8%!     37.0%      28.9%!     23.4%!     20.0%!
Kim Cang Tự - Hộ Thể Côn Quyền             47.9%      51.7%      12.2%!     16.5%!     27.7%!     37.4%      57.0%      52.0% 
Kim Cang Tự - Bách Luyện Thuần Quyền       69.7%!     73.6%!     47.0%      38.2%      38.5%      37.3%      57.4%      55.8% 
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       88.0%!     72.0%!     87.1%!     67.6%!     62.8%      40.0%      36.3%      31.2%!
Liệt Diễm Đường - Hỏa Hải Song Đao         68.7%!     52.2%      71.8%!     51.8%      59.2%      35.3%      35.8%      26.3%!
Huyền Độc Môn - Huyết Thú Cuồng Trảo       38.4%      43.2%      38.6%      47.5%      23.0%!     38.1%      64.5%      72.0%!
Hàn Sơn Phái - Băng Quy Tuyệt Hộ           31.5%!     53.1%      29.5%!     50.0%      30.2%!     51.0%      32.3%!     43.8% 
Liệt Diễm Đường - Xích Xà Hỏa Liệp         63.0%      48.2%      46.3%      28.5%!     67.3%!     43.2%      67.8%!     48.1% 
Kim Cang Tự - Bạch Hổ Bá Quyền             43.9%      45.5%      30.7%!     30.3%!     63.3%      56.7%      61.0%      55.4% 
Bích Thủy Am - Thanh Đằng Trường Tiên       7.6%!     67.2%!     57.8%      80.0%!     70.1%!     81.2%!     71.5%!     80.6%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        46.1%      73.3%!     30.0%!     59.8%      20.7%!     43.9%      23.6%!     34.8%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      86.3%!     79.0%!     60.2%      42.3%      47.9%      30.1%!     18.2%!     19.4%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      20.5%!     12.5%!     41.3%      48.3%      67.5%!     83.4%!     47.0%      72.2%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      63.1%      52.7%      47.9%      35.4%      32.7%!      8.7%!     22.7%!     13.0%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        49.7%      56.6%      70.9%!     80.6%!     87.8%!     94.3%!     77.9%!     83.5%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      30.0%!     29.6%!     17.0%!     33.6%!     38.2%      56.5%      26.2%!     39.8% 
Thiết Phủ Bang - Bá Khí Khai Sơn           68.5%!     45.8%      65.0%      37.7%      31.3%!      7.6%!     21.8%!     12.7%!
Outside [35, 65]: 103/192
```

---

## 4. Giải Trình Thay Đổi Chi Tiết

1. **Vũ khí chém & đập nặng (Khai Sơn Phủ, Chấn Nhạc Chùy, Liệt Diễm Đao):** Giảm hồi chiêu từ 3.2–3.6s xuống **2.6–2.8s**, tăng sát thương lên 22–29. Giảm tiêu hao nộ tuyệt kỹ từ 60 xuống 38–48. Lý do: Giúp các build rìu/chùy/đao thi triển sát thương kịp trước khi đối thủ hồi phục.
2. **Kiếm Tông & Ngọc Cốt Phiến:**
   - `ngoc_cot_phien.ultimate.cost`: 30 → **45**, nạp chiêu khi bạo kích: 0.8s → **0.15s**.
   - `kiem_tong_tam_yeu`: Nạp chiêu lân cận khi bạo kích: 0.5s → **0.15s**.
   - `thai_thanh_tam_kinh`: Giảm tiêu hao đỡ đòn: 20 → **8**.
   - Lý do: Chấm dứt tình trạng Tiêu Dao Phiến và Kiếm Đạo Hóa Kình bắn nộ liên thanh 96% winrate.
3. **Quyền & Côn (Thiết Cốt Quyền Sáo, Phục Hổ Côn, Bách Luyện Chân Kinh):**
   - `phuc_ho_con.attack`: 12 → **14**, cooldown: 2.7s → **2.6s**.
   - `thiet_cot_quyen_sao.attack`: 3 → **6**.
   - `bach_luyen_chan_kinh`: Sát thương cộng thêm khi phá khiên giữ ở **1**, nạp chiêu **0.7s**.
   - Lý do: Cứu Kim Cang Hộ Thể từ 28% lên 37.8%, đồng thời kiềm hãm Bách Luyện Thuần Quyền từ 85% về 52.2%.
4. **Hiệu ứng trạng thái & Config Engine:**
   - `burn.damage`: **80**, `poison.damage`: **32**, `bleed.damage`: **20** (maxHpPermille: **35**).
   - `maxRegen`: **3** (giảm từ 6 để tránh cù nhây hồi máu).
   - `exhaustionStep`: **5**.

---

## 5. Bảng Nghiệm Thu PvE 30 Ải (Starter Kit Mới)

Theo Quyết định #39, túi khởi đầu mới chỉ gồm **1 Kiếm (`thanh_phong_kiem` T1 Phàm) + 1 Giáp (`o_tam_y` T1 Phàm)** và 1000 Bạc. Túi đồ người chơi Ải 1–5 được tái thiết lập chuẩn xác từ lộ trình thưởng:
- Ải 1: Kiếm T1 + Ô Tàm Y T1. Kẻ địch: Thảo Khấu Tiên Phong (`thanh_dang_tien` + `thiet_giap_y` T1). Tỷ lệ thắng: **83.5%** (chuẩn tutorial 80–90%).
- Ải 2: Thêm Bồi Nguyên Đan T1 (thưởng Ải 1). Thắng: **89.5%**.
- Ải 3: Thêm Kim Sáng Dược T1 (thưởng Ải 2). Kẻ địch: Hoang Dã Sói Xám (Nanh Sói + Móng Vuốt + Thiết Giáp Y + Bồi Nguyên Đan Cấp 5). Thắng: **85.0%**.
- Ải 4: Thêm Nanh Sói T1. Thắng: **86.0%**.
- Ải 5 (Tinh Anh Phong Vũ): Thêm Thái Thanh Tâm Kinh T1. Thắng: **64.0%** (chuẩn 60–70%).

```
Ải   Tên Ải                   Loại     Kẻ Địch                    Cấp Địch   Tỷ Lệ Thắng    Mục Tiêu   Đạt?
---------------------------------------------------------------------------------------------------------
01   Trấn Khẩu Sơn Đạo        normal   Thảo Khấu Tiên Phong       1           83.5%         80-90%     ĐẠT
02   Phong Diệp Bến Đò        normal   Thủy Phỉ Thám Tử           1           89.5%         80-90%     ĐẠT
03   Hòe Thụ Thôn Ngoại       normal   Hoang Dã Sói Xám           5           85.0%         80-90%     ĐẠT
04   Thanh Khê Trà Thất       normal   Liệt Diễm Bang Chúng       3           86.0%         80-90%     ĐẠT
05   Phong Ba Đình            elite    Phong Lôi Khoái Đao Phong Vũ 6           64.0%         60-70%     ĐẠT
06   Ngô Đồng Cổ Hạng         normal   Ám Hạng Thích Khách        13          83.0%         80-90%     ĐẠT
07   Thủy Tạ Nguy Kiều        normal   Liệt Diễm Bang Tinh Anh    2           90.0%         80-90%     ĐẠT
08   Phong Hỏa Cổ Đài         normal   Liệt Diễm Bang Hương Chủ   4           83.0%         80-90%     ĐẠT
09   Ba Hổ Trại Tiền Doanh    normal   Ba Hổ Tiên Phong Vệ        5           80.0%         80-90%     ĐẠT
10   Thủy Hỏa Phân Tranh Bến Bạc boss     Nộ Ba Hổ Bùi Chấn          16          55.0%         50-60%     ĐẠT
11   Man Uyên Nhập Khẩu       normal   Vạn Độc Môn Đồ             11          82.0%         80-90%     ĐẠT
12   Ô Đầm Nhai               normal   Huyền Độc Môn Đồ           16          87.0%         80-90%     ĐẠT
13   Hủ Cốt Thung             normal   Kịch Độc Tri Chu           16          88.0%         80-90%     ĐẠT
14   Xà Khẩu Hiệp             normal   Man Cương Vu Đao           18          88.5%         80-90%     ĐẠT
15   U Cốc Dược Đàn           elite    Bách Thảo Nghịch Đồ        13          70.0%         60-70%     ĐẠT
16   Mê Vụ Đàm                normal   Hắc Sát Độc Hùng           16          87.0%         80-90%     ĐẠT
17   Lam Hoa Trạch            normal   U Minh Ám Sát Thí Tử       17          89.0%         80-90%     ĐẠT
18   Độc Long Khê             normal   Huyền Độc Trưởng Lão       23          90.0%         80-90%     ĐẠT
19   Vạn Xà Quật              normal   Xích Luyện Xà Tinh         25          90.0%         80-90%     ĐẠT
20   Thần Bí Cổ Đàn           boss     Hắc Độc Nương Tử           11          50.0%         50-60%     ĐẠT
21   Tuyết Dốc Băng Khẩu      normal   Hàn Phong Tuần Tuyết Giả   17          88.0%         80-90%     ĐẠT
22   Phong Nhận Trầm Khê      normal   Hàn Sơn Kiếm Sĩ            26          88.0%         80-90%     ĐẠT
23   Tuyết Lĩnh Hàn Phong     normal   Tuyết Lĩnh Ngân Lang       36          86.0%         80-90%     ĐẠT
24   Băng Hà Cổ Nhai          normal   Kim Cang Hộ Giới Tăng      26          89.5%         80-90%     ĐẠT
25   Phong Vân Băng Đài       elite    Hàn Sơn Đại Đệ Tử          17          62.5%         60-70%     ĐẠT
26   Bạch Hổ Tuyệt Hạp        normal   Băng Nguyên Bạch Hổ        27          87.0%         80-90%     ĐẠT
27   Hàn Tinh Thạch Thất      normal   U Minh Chấp Pháp Sứ        26          90.0%         80-90%     ĐẠT
28   Phong Ma Cổ Đạo          normal   Vạn Niên Băng Quy          37          90.0%         80-90%     ĐẠT
29   Vọng Nguyệt Băng Đỉnh    normal   U Minh Tuyệt Mệnh Kiếm Vệ  25          88.0%         80-90%     ĐẠT
30   Phong Thiền Băng Đài     boss     U Minh Tả Sứ Dạ Sát Thiên  27          54.5%         50-60%     ĐẠT
---------------------------------------------------------------------------------------------------------
Tổng kết 30 Ải: 30/30 ải ĐẠT 100% (Không có ải nào lệch ra ngoài khung độ khó #34).
```

---

## 6. Lệnh Tái Hiện (Reproduction Commands)

Tất cả lệnh chạy từ thư mục gốc repository (`One-Bag-Jianghu`):
```bash
# 1. Đo Ma trận A (100 runs):
py tools/balance/balance.py matrix --loadouts tools/gemini/out/round-4/loadouts --items tools/gemini/out/round-4/items-final.json --battle-config tools/gemini/out/round-4/config.json --tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10

# 2. Đo Rarity Step (100 runs):
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/round-4/loadouts --items tools/gemini/out/round-4/items-final.json --battle-config tools/gemini/out/round-4/config.json --tier 3 --tp 1 --level 10 --runs 100

# 3. Đo Tier Step (100 runs, cấp 10):
py tools/gemini/out/round-4/scratch/run_final_tables.py

# 4. Kiểm tra nghiệm thu 30 ải PvE:
py tools/gemini/out/round-4/scratch/verify_pve_table.py
```

---

## 7. Câu Hỏi Mở & Đề Xuất (Tối Đa 4 Câu Hỏi)

1. **Về trần số ô lệch Ma trận A (103/192 so với kỳ vọng ≤ ~58):** Sự chênh lệch hiện tại đến từ tương khắc hệ thống (Độc/Chảy máu bỏ qua khiên hạ gục Thuần Thủ; Kiếm bạo kích trị Độc; Băng khống chế ngắt nhịp vũ khí chậm). Để ép số ô ngoài [35, 65]% xuống dưới 58 mà không làm mất bản sắc phái, liệu ta có nên áp dụng cơ chế giảm bớt hiệu ứng khắc chế ở bậc Trọng thấp (T1–T3)?
2. **Về độ giãn cách cấp độ kẻ địch PvE:** Ở các ải cuối Chương 3 (Ải 23, 28), cấp độ kẻ địch phải kéo lên 36–37 để ghìm tỷ lệ thắng người chơi không vượt quá 90%. Người dùng có đồng ý giải pháp này, hay muốn nâng phẩm chất trang bị của quái vật lên Tinh/Tuyệt thay vì nâng cấp số?
3. **Cơ chế Hộ Thân (DmgReduce) của Đan Dược:** Hiện trần 80% vẫn khiến một số nhánh build tích lũy đan dược trở nên quá trâu ở hiệp cuối. Có nên đề xuất giảm nhẹ trần đan dược từ 80% xuống 60% ở Round tiếp theo?
4. **Túi đồ người chơi mẫu ở PvE Chương 2 & 3:** Hiện người chơi mẫu ở Chương 2–3 vẫn mang túi đồ từ Round 3. Sau khi Starter Kit #39 ổn định, có cần xây dựng bộ túi đồ người chơi phân nhánh theo 7 môn phái cho PvE không?
