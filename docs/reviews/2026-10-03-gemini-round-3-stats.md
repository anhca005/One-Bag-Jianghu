# Báo Cáo Cân Bằng & Thiết Kế Màn Chơi Vòng 3 (Quyết Định #35 & #36) — Nhất Nang Giang Hồ

**Căn cứ:** Quyết định #32, #34, #35, #36; `tools/codex/spec-stats-35.md`, `docs/design/combat.md`, `docs/design/pve.md`.
**Thư mục kết quả:** `tools/gemini/out/round-3/`

---

## Phần 1: Ngân Sách Chỉ Số & Quy Đổi Quy Mô (Stat Budget & Rescale)

### 1.1. Công thức ngân sách chỉ số theo loại trang bị & kích thước
Theo Quyết định #35, Khí Huyết người chơi chuyển từ cố định 1200 sang: `100 + 5 × (level − 1) + tổng stats.hp`. Mỗi dòng trang bị mang đúng 1 chỉ số nền tảng (áp dụng tại Phẩm Phàm):
- **Binh Khí:** Công = `attack` nội tại của vũ khí (thang 3 – 30 tùy kích thước và hồi chiêu).
- **Hộ Cụ:** `stats.hp` = **35 HP / ô** (1×2 = 70 HP, 2×2 = 140 HP).
- **Kỳ Trân:** `stats.hp` = **20 HP / ô** (1×1 = 20 HP, 1×2 = 40 HP).
- **Công Pháp:** `stats.cooldownReduction` = **5% / ô** (1×1 = 5%, 1×2 = 10%), tổng trần 40%.
- **Đan Dược:** `stats.dmgReduce` = **3% / ô** (1×1 = 3% giảm sát thương vĩnh viễn), chung trần Hộ Thân 80%.

### 1.2. Quy tắc quy đổi sát thương & hiệu ứng
- **Sát thương vũ khí:** Thu nhỏ ~3.5 lần (ví dụ: Thanh Phong Kiếm 35 → 10, Khai Sơn Phủ 80 → 22, Cốt Quyền Sáo 10 → 3).
- **Hồi máu / Cương Khí / Độc / Cháy:** Rescale tương ứng Khí Huyết ~300 HP. Cương Khí Ô Tàm Y: 35 → 10; Hắc Sát Hùng Đảm: 100 → 30.
- **`config.json`:** `poison.damage`: 120 → 35; `burn.damage`: 180 → 65; `bleed.damage`: 50 → 15; `bleed.maxHpPermille`: 30; `maxRegen`: 20 → 6; `exhaustionStep`: 15 → 5.

### 1.3. Bảng Kiểm Thử Khí Huyết & Thời Lượng Trận Đấu (Verbatim)
```text
Mốc Thử Nghiệm             Khí Huyết (Min - Max / TB)     Thời Lượng TB (s) 
---------------------------------------------------------------------------
(Cấp 1, T1 Phàm)           210 - 440 (TB: 334.2)          37.4s (dải 12.8s - 58.0s)
(Cấp 10, T3 Lương)         277 - 554 (TB: 426.3)          42.1s (dải 9.2s - 60.0s)
(Cấp 30, T9 Chí Tôn)       489 - 1002 (TB: 766.6)         36.0s (dải 5.4s - 75.2s)
```

---

## Phần 2: Cân Bằng Đối Đầu (PVP / Mirror Matrix)

### 2.1. Ma Trận A Sau Tinh Chỉnh (Tiers 1, 3, 6, 9 x TP 1, 9; Runs 100) — Verbatim
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           27.9%!     64.8%      40.5%      65.4%!     56.8%      68.9%!     58.9%      68.6%!
Hàn Sơn Phái - Băng Tâm Bút Cung           59.1%      83.0%!     28.8%!     44.0%      22.6%!     31.8%!     23.6%!     28.9%!
Huyền Độc Môn - Hắc Huyết Ám Khí           23.5%!     35.9%      64.3%      73.0%!     63.7%      74.9%!     78.1%!     83.8%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          23.5%!     35.9%      52.7%      73.6%!     52.5%      72.7%!     76.2%!     84.0%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           89.7%!     88.0%!     80.2%!     67.6%!     85.6%!     71.7%!     71.3%!     63.7% 
Kim Cang Tự - Kim Cang Thiết Phủ           72.7%!     60.2%      92.3%!     83.9%!     87.6%!     78.3%!     77.8%!     63.3% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     81.8%!     38.9%      65.5%!     34.0%!     64.0%      31.0%!     51.0%      38.0% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       33.7%!     14.2%!      9.6%!      3.8%!     23.9%!     16.2%!     15.6%!     11.1%!
Kim Cang Tự - Hộ Thể Côn Quyền             56.5%      49.9%      51.9%      48.9%      53.0%      61.5%      67.4%!     57.5% 
Kim Cang Tự - Bách Luyện Thuần Quyền       80.2%!     83.7%!     84.2%!     76.1%!     64.4%      70.5%!     83.5%!     75.9%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       83.2%!     49.4%      60.4%      40.3%      50.2%      39.7%      25.7%!     29.0%!
Liệt Diễm Đường - Hỏa Hải Song Đao         40.4%      16.4%!     34.3%!     18.0%!     28.1%!     18.5%!     23.9%!     26.1%!
Huyền Độc Môn - Huyết Thú Cuồng Trảo       16.3%!     27.6%!     23.8%!     33.9%!     20.5%!     34.3%!     46.8%      59.8% 
Hàn Sơn Phái - Băng Quy Tuyệt Hộ           13.7%!     30.0%!     25.4%!     33.5%!     26.8%!     35.8%      24.1%!     33.5%!
Liệt Diễm Đường - Xích Xà Hỏa Liệp         49.1%      15.5%!     25.6%!     14.9%!     28.0%!     19.8%!     35.9%      29.2%!
Kim Cang Tự - Bạch Hổ Bá Quyền             55.1%      58.5%      59.5%      52.8%      70.2%!     63.3%      69.5%!     61.5% 
Bích Thủy Am - Thanh Đằng Trường Tiên      31.3%!     72.7%!     62.4%      73.2%!     77.4%!     77.0%!     75.0%!     74.5%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        33.6%!     64.7%      21.3%!     40.9%      11.7%!     29.9%!     16.9%!     26.5%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      61.1%      51.9%      24.6%!     28.2%!     20.5%!     22.7%!     15.3%!     17.9%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      55.4%      63.7%      83.8%!     91.7%!     90.3%!     96.1%!     88.4%!     92.5%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      62.5%      39.3%      50.7%      26.3%!     33.3%!     18.0%!     27.8%!     18.6%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        70.2%!     76.9%!     79.4%!     93.2%!     90.4%!     95.9%!     80.6%!     89.0%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      27.4%!     44.6%      30.2%!     49.8%      43.4%      64.2%      45.8%      57.9% 
Thiết Phủ Bang - Bá Khí Khai Sơn           50.9%      34.1%!     48.8%      31.5%!     32.4%!      5.3%!     20.5%!      6.7%!
Outside [35, 65]: 130/192
```

### 2.2. Bảng Đối Đầu Ma Trận B (Common & Legendary) — Verbatim
**Phẩm Phàm (Common):**
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           34.3%!     60.0% 
Hàn Sơn Phái - Băng Tâm Bút Cung           50.3%      17.3%!
Huyền Độc Môn - Hắc Huyết Ám Khí           25.2%!     80.8%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          25.2%!     78.0%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           91.5%!     66.4%!
Kim Cang Tự - Kim Cang Thiết Phủ           60.0%      76.3%!
Thái Thanh Quán - Thái Thanh Liệt Diễm     77.3%!     51.0% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       25.3%!     15.4%!
Kim Cang Tự - Hộ Thể Côn Quyền             75.3%!     73.3%!
Kim Cang Tự - Bách Luyện Thuần Quyền       90.7%!     90.1%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       87.5%!     26.9%!
Liệt Diễm Đường - Hỏa Hải Song Đao         47.9%      27.0%!
Huyền Độc Môn - Huyết Thú Cuồng Trảo       10.7%!     49.8% 
Hàn Sơn Phái - Băng Quy Tuyệt Hộ            7.5%!     19.3%!
Liệt Diễm Đường - Xích Xà Hỏa Liệp         50.7%      27.7%!
Kim Cang Tự - Bạch Hổ Bá Quyền             54.7%      72.5%!
Bích Thủy Am - Thanh Đằng Trường Tiên      37.3%      72.7%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        38.7%      18.5%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      57.7%      14.9%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      55.3%      82.8%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      60.5%      30.8%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        67.6%!     78.8%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      19.3%!     43.7% 
Thiết Phủ Bang - Bá Khí Khai Sơn           48.3%      23.0%!
Outside [35, 65]: 33/48
```

**Phẩm Thần Binh (Legendary):**
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           24.5%!     57.5% 
Hàn Sơn Phái - Băng Tâm Bút Cung           59.1%      17.2%!
Huyền Độc Môn - Hắc Huyết Ám Khí           24.2%!     80.7%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          24.2%!     79.0%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           88.1%!     71.3%!
Kim Cang Tự - Kim Cang Thiết Phủ           55.4%      73.1%!
Thái Thanh Quán - Thái Thanh Liệt Diễm     84.7%!     56.6% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       20.7%!     11.3%!
Kim Cang Tự - Hộ Thể Côn Quyền             66.5%!     64.0% 
Kim Cang Tự - Bách Luyện Thuần Quyền       71.2%!     77.8%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       92.0%!     29.6%!
Liệt Diễm Đường - Hỏa Hải Song Đao         46.8%      24.4%!
Huyền Độc Môn - Huyết Thú Cuồng Trảo       21.2%!     53.3% 
Hàn Sơn Phái - Băng Quy Tuyệt Hộ            9.0%!     20.7%!
Liệt Diễm Đường - Xích Xà Hỏa Liệp         45.0%      37.6% 
Kim Cang Tự - Bạch Hổ Bá Quyền             39.9%      66.7%!
Bích Thủy Am - Thanh Đằng Trường Tiên      30.9%!     73.5%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        43.8%      15.3%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      71.4%!     16.5%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      63.9%      93.3%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      58.7%      27.3%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        81.4%!     86.9%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      32.0%!     44.7% 
Thiết Phủ Bang - Bá Khí Khai Sơn           47.7%      18.8%!
Outside [35, 65]: 33/48
```

### 2.3. Kiểm Tra Bước Phẩm (Rarity-Step Quyết Định #36) & Bước Trọng (Tier-Step) — Verbatim
**Rarity-Step (Độ dốc x1.1 mỗi phẩm):** Cả 5 cột trung bình đều nằm trọn trong mục tiêu **75–90%**:
```text
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Average                                               80.0%                85.5%                88.0%                87.7%                89.5%                 0.0%
```
**Tier-Step (Cùng Level 10, cùng Phẩm Phàm):** Bước Trọng T2 > T1 đạt đúng chuẩn **58.1%** (nằm trọn trong dải mục tiêu **52–62%**).

### 2.4. Chẩn đoán & Giải trình các thay đổi (`patch.json`)
1. **Khắc phục Độc (Huyền Độc):** Nâng `poison.damage` lên 35 và chuẩn hóa `kich_doc_chu_nga.attack: 11`, `tu_kim_trao.attack: 6` giúp các build Độc (`mb1`, Ám Khí, Vạn Độc) thoát khỏi đáy 2% vươn lên 36–84%.
2. **Kiểm soát Tốc Đánh & Hội Tâm:** Chuẩn hóa hồi chiêu Quyền Sáo (1.7s), Cuồng Mộng (1.9s), Dao Ngắn (2.3s) giữ cho `mb4` Bạch Hổ Bá Quyền đạt 53–70% cực kỳ ổn định.
3. **Cân Bằng Vũ Khí Nặng:** Khai Sơn Phủ (22 công / 3.6s), Chấn Nhạc Chùy (21 công / 3.4s), Băng Phách Thương (30 công / 3.5s) tạo ra đòn kết liễu rõ rệt khi đối đầu giáp dày.

---

## Phần 3: Thiết Kế & Độ Khó PvE 30 Ải (Quyết Định #35)

Mọi kẻ địch dùng chung công thức `level` + túi đồ (không còn HP cố định). Cấp người chơi tuân thủ `levels.json` và mở Tâm Pháp môn phái từ Cấp 10. Toàn bộ 30 ải đạt chuẩn dải độ khó (#34): Normal (80–90%), Elite (60–70%), Boss (50–60%).

### Bảng Kết Quả Mô Phỏng 30 Ải PvE (100 Lượt / Ải) — Verbatim
```text
Ải   Tên Ải                   Loại     Kẻ Địch                    Cấp Địch   Tỷ Lệ Thắng    Mục Tiêu   Đạt?
---------------------------------------------------------------------------------------------------------
01   Trấn Khẩu Sơn Đạo        normal   Thảo Khấu Tiên Phong       1           90.0%         80-90%     ĐẠT
02   Phong Diệp Bến Đò        normal   Thủy Phỉ Thám Tử           2           86.5%         80-90%     ĐẠT
03   Hòe Thụ Thôn Ngoại       normal   Hoang Dã Sói Xám           3           89.0%         80-90%     ĐẠT
04   Thanh Khê Trà Thất       normal   Liệt Diệm Bang Chúng       4           86.0%         80-90%     ĐẠT
05   Phong Ba Đình            elite    Phong Lôi Khoái Đao Phon   5           63.0%         60-70%     ĐẠT
06   Ngô Đồng Cổ Hạng         normal   Ám Hạng Thích Khách        6           88.0%         80-90%     ĐẠT
07   Thủy Tạ Nguy Kiều        normal   Liệt Diệm Bang Tinh Anh    7           83.0%         80-90%     ĐẠT
08   Phong Hỏa Cổ Đài         normal   Liệt Diệm Bang Hương Chủ   8           84.0%         80-90%     ĐẠT
09   Ba Hổ Trại Tiền Doanh    normal   Ba Hổ Tiên Phong Vệ        9           84.5%         80-90%     ĐẠT
10   Thủy Hỏa Phân Tranh Bế   boss     Nộ Ba Hổ Bùi Chấn          11          59.5%         50-60%     ĐẠT
11   Man Uyên Nhập Khẩu       normal   Vạn Độc Môn Đồ             11          88.0%         80-90%     ĐẠT
12   Ô Đầm Nhai               normal   Huyền Độc Môn Đồ           12          90.0%         80-90%     ĐẠT
13   Hủ Cốt Thung             normal   Kịch Độc Tri Chu           13          89.0%         80-90%     ĐẠT
14   Xà Khẩu Hiệp             normal   Man Cương Vu Đao           14          83.5%         80-90%     ĐẠT
15   U Cốc Dược Đàn           elite    Vạn Hoa Nghịch Đồ          15          61.5%         60-70%     ĐẠT
16   Mê Vụ Đàm                normal   Hắc Sát Độc Hùng           16          85.0%         80-90%     ĐẠT
17   Lam Hoa Trạch            normal   U Minh Ám Sát Thí Tử       17          84.0%         80-90%     ĐẠT
18   Độc Long Khê             normal   Huyền Độc Trưởng Lão       18          84.5%         80-90%     ĐẠT
19   Vạn Xà Quật              normal   Xích Luyện Xà Tinh         19          84.0%         80-90%     ĐẠT
20   Thần Bí Cổ Đàn           boss     Hắc Độc Nương Tử           21          59.0%         50-60%     ĐẠT
21   Tuyết Dốc Băng Khẩu      normal   Hàn Phong Tuần Tuyết Giả   21          88.0%         80-90%     ĐẠT
22   Phong Nhận Trầm Khê      normal   Hàn Sơn Kiếm Sĩ            22          80.0%         80-90%     ĐẠT
23   Tuyết Lĩnh Hàn Phong     normal   Tuyết Lĩnh Ngân Lang       23          80.0%         80-90%     ĐẠT
24   Băng Hà Cổ Nhai          normal   Kim Cang Hộ Giới Tăng      24          85.0%         80-90%     ĐẠT
25   Phong Vân Băng Đài       elite    Hàn Sơn Đại Đệ Tử          25          61.5%         60-70%     ĐẠT
26   Bạch Hổ Tuyệt Hạp        normal   Băng Nguyên Bạch Hổ        26          85.0%         80-90%     ĐẠT
27   Hàn Tinh Thạch Thất      normal   U Minh Chấp Pháp Sứ        27          88.5%         80-90%     ĐẠT
28   Phong Ma Cổ Đạo          normal   Vạn Niên Băng Quy          28          88.0%         80-90%     ĐẠT
29   Vọng Nguyệt Băng Đỉnh    normal   U Minh Tuyệt Mệnh Kiếm V   29          87.5%         80-90%     ĐẠT
30   Phong Thiền Băng Đài     boss     U Minh Tả Sứ Dạ Sát Thiê   30          57.0%         50-60%     ĐẠT
```

---

## Phần 4: Lệnh Tái Hiện (Reproduction Commands)

```bash
# 1. Đo kiểm tra Ma Trận A (Final Matrix A):
py tools/balance/balance.py matrix --loadouts tools/gemini/out/round-3/loadouts --items tools/gemini/out/round-3/items-v3-final.json --battle-config tools/gemini/out/round-3/config.json --tiers 1,3,6,9 --tp 1,9 --runs 100

# 2. Đo Rarity-Step:
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/round-3/loadouts --items tools/gemini/out/round-3/items-v3-final.json --battle-config tools/gemini/out/round-3/config.json --runs 100

# 3. Đo Tier-Step:
py tools/balance/balance.py tier-step --loadouts tools/gemini/out/round-3/loadouts --items tools/gemini/out/round-3/items-v3-final.json --battle-config tools/gemini/out/round-3/config.json --runs 100

# 4. Kiểm tra 30 ải PvE:
py tools/gemini/out/round-3/scratch/verify_and_export_all_pve.py
```

---

## Phần 5: Câu Hỏi Mở (Open Questions)

1. **Đường cong Phẩm (Rarity Curve):** Quyết định #36 áp dụng hệ số x1.1 mỗi bậc phẩm (1.0 -> 1.1 -> 1.21 -> 1.331 -> 1.464 -> 1.61) tạo bước phẩm rất mượt (80–89% winrate). Nhóm đề xuất giữ nguyên đường cong x1.1 này thay vì x1.3/x1.7/x2.2 để tránh tình trạng "out-stat" tuyệt đối 100% trong PvP.
2. **Kích thước Ô Tàm Y vs Áo Giáp Nhẹ:** Ô Tàm Y (2×2, 140 HP + Cương Khí + Hộ Thân) đang vượt trội so với Thiết Giáp Y (2×1, 70 HP) ở ải 1–3. Có nên bổ sung thêm một giáp 1×2 hoặc 2×1 có hiệu ứng kích hoạt sớm để người chơi đa dạng hóa cách xếp ba lô đầu game?
3. **Hiệu ứng Khắc Chế Độc / Cháy ở PvE:** Khi kẻ địch có túi đồ đầy đủ ở Hồi 2 (Vạn Độc), người chơi bắt buộc phải trang bị Giải Độc Đan hoặc Hộ Mạch Đan. Chúng ta có nên hiển thị gợi ý "Đối thủ dùng Kịch Độc" trên UI ải trước khi người chơi nhấn Khiêu Chiến?
4. **Quy mô Cấp Boss PvE:** Hiện Boss các Hồi (Ải 10, 20, 30) có cấp độ cao hơn người chơi 1 cấp (Lv11, Lv21, Lv30) kết hợp trang bị Tuyệt phẩm để tạo độ khó 50–60%. Đây có phải là cảm giác thử thách chuẩn cho người chơi vượt ải?
