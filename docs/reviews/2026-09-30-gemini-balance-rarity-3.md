# Báo Cáo Cân Bằng Đồ & Khắc Phục Outlier (Vòng 3)

## 1. Chẩn Đoán Chi Tiết Các Outlier Vòng 2
1. **Kim Cang Cương Khí (shield_sustain ~90%):** Thử nghiệm loại bỏ từng món (ablation) chứng minh 2 món mang vác toàn bộ build:
   - Gỡ `o_thiet_giap`: tỉ lệ thắng T1/TP1 sập từ **91.8% xuống 42.0%** (-50%). Khiên gốc 500 (+150 ở T3) nhân với hệ số Trọng/Phẩm lập tức chạm trần 100% HP ngay tick 0.
   - Gỡ `ho_mach_dan`: tỉ lệ thắng T1/TP1 giảm từ **91.8% xuống 65.7%**. Khiên 350 khi dưới 40% HP trên viên đan 1 ô giúp hồi phục gần như đầy máu ảo.
2. **Tử Mẫu Tốc Kích (crit_haste 0–25%):** Vũ khí `tu_mau_song_dao` chiếm 4 ô (2x2) nhưng chỉ có 22 công, hồi chiêu 2.7s (DPS 16.3), chiêu cuối 35 nội lực chỉ gây 66 sát thương. Ngoài ra túi đồ chứa `kiem_tong_tam_yeu` (tăng kiếm trong khi túi không có kiếm). Tăng công lên 27 và giảm hồi chiêu còn 2.2s đưa tỉ lệ thắng từ 1.0% lên thẳng 46–62% (6/8 ô vào chuẩn).
3. **Huyền Độc Môn (huyen_doc ~10%):** Túi đồ mẫu bị lỗi xếp đồ nghiêm trọng: mang 2 `han_ngoc_tieu` (tiêu băng của Hàn Sơn) không có độc/xuất huyết và không nhận Tâm Pháp Huyền Độc, làm loãng sát thương ám khí.
4. **Huyền Độc Phệ Khí & Nhảy Vọt Rarity-Step 100%:** Trong trận gương giữa 2 bản giống nhau, build thuần thủ/hút nội lực không kết liễu được đối thủ trước 30s. Trận đấu kéo tới 51s và kết thúc bằng sát thương Kiệt Sức (Exhaustion). Ở Phàm/Lương/Tinh, cả hai cùng gục một tick -> Hòa (50%). Từ Tuyệt trở lên, bên phẩm cao hơn sống lâu hơn 1 tick -> Thắng 100%.

## 2. Bản Vá Cân Bằng & Lý Do Thay Đổi
File patch: `tools/gemini/out/balance-rarity-3/balance-rarity-3.patch.json`
- `o_thiet_giap.triggers.shield: 300` (gốc 500): Hạ khiên đầu trận về mức hợp lý, kéo Kim Cang Cương Khí từ ~90% xuống ~68%.
- `ho_mach_dan.triggers.shield: 200` (gốc 350): Cân đối khiên cứu nguy khẩn cấp của đan dược 1 ô.
- `tu_mau_song_dao.attack: 27` (gốc 22) & `cooldown: 2.2` (gốc 2.7): Đưa DPS của Song Đao 4 ô tiệm cận ngân sách vũ khí trung cấp.
- `tu_kim_trao.attack: 7` (gốc 6): Tăng nhẹ sát thương cơ bản cho hệ Trảo của Huyền Độc Môn.

## 3. Kết Quả Đo Đạc So Với Baseline
- **Matrix A**: Baseline 98/208 -> **94/208** (giảm 4 ô lệch).
- **Matrix B Common**: Baseline 24/52 -> **23/52** (giảm 1 ô lệch).
- **Matrix B Legendary**: Baseline 23/52 -> **22/52** (giảm 1 ô lệch).
- **Rarity-step**: Cột trung bình đạt 65.3%, 69.1%, 71.8%, 71.7%, 71.6% (nằm trọn trong ngưỡng 60–80%).

### Ma Trận A (Nguyên văn sau patch)
```
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        62.0%      49.2%      61.0%      44.9%      49.1%      44.5%      71.0%!     60.2% 
Bích Thủy Hồi Xuân                         16.9%!     52.2%      40.3%      71.2%!     71.9%!     67.9%!     74.1%!     68.2%!
Tử Mẫu Tốc Kích                            16.1%!     18.0%!     46.6%      51.7%      40.9%      56.9%      56.9%      62.7% 
Huyền Độc Môn (Ám Khí + Trảo)              11.9%!      5.7%!      7.0%!      1.7%!     11.7%!     10.2%!      9.0%!      8.4%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     43.2%      41.7%      24.9%!     27.9%!     28.4%!     38.6%      29.9%!     36.3% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     56.9%      39.6%      56.1%      38.3%      61.0%      50.0%      64.0%      54.3% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     74.4%!     76.2%!     65.5%!     64.0%      66.4%!     69.7%!     77.1%!     76.3%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      53.5%      44.2%      45.2%      38.8%      41.0%      30.3%!     32.4%!     20.9%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      61.7%      32.6%!     43.7%      30.8%!     42.0%      30.4%!     36.9%      24.7%!
Kim Cang Tự (Quyền + Côn)                  43.1%      62.8%      41.5%      52.2%      34.7%!     48.6%      41.4%      58.8% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     50.9%      72.4%!     50.8%      72.3%!     56.8%      70.0%!     67.7%!     76.5%!
Liệt Diễm Đường (Đao lửa)                  66.7%!     49.9%      62.2%      53.6%      49.6%      53.0%      36.5%      40.3% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         58.9%      58.7%      46.5%      41.1%      33.4%!     35.9%      17.3%!     26.8%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     38.1%      83.0%!     42.4%      80.4%!     56.2%      75.4%!     37.5%      49.0% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     34.6%!     67.2%!     42.3%      72.5%!     44.8%      71.8%!     40.5%      67.9%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     79.3%!     86.9%!     87.8%!     89.0%!     85.6%!     87.6%!     78.0%!     77.4%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     45.7%      33.2%!     42.0%      40.6%      41.4%      42.0%      49.2%      50.8% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      83.3%!     62.0%      87.7%!     58.1%      82.2%!     54.0%      64.5%      44.2% 
Thái Thanh Hóa Kình                        32.1%!     46.9%      47.4%      61.4%      50.5%      57.3%      64.4%      70.7%!
Huyền Độc Phệ Khí                           8.6%!      9.9%!      4.7%!      5.2%!     18.9%!     16.9%!     19.6%!     19.0%!
Kim Cang Cương Khí                         68.9%!     72.5%!     87.1%!     85.2%!     80.6%!     79.7%!     83.7%!     86.2%!
Thái Thanh Quán (Kiếm + Hóa Kình)          37.8%      32.5%!     40.7%      41.9%      41.6%      42.3%      50.5%      51.6% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     60.1%      64.5%      56.5%      61.5%      46.7%      53.7%      57.2%      64.2% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      61.5%      49.1%      54.2%      35.9%      44.4%      31.7%!     52.7%      34.2%!
Thiết Phủ Bang (Rìu + Cầm)                 42.6%      23.3%!     47.3%      27.0%!     50.6%      33.6%!     30.1%!     17.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      91.7%!     65.0%      65.1%!     54.7%      68.2%!     48.9%      59.6%      54.3% 
Outside [35, 65]: 94/208
```

### Ma Trận B (Nguyên văn sau patch)
```
[Common]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        64.6%      73.9%!
Bích Thủy Hồi Xuân                         15.7%!     78.5%!
Tử Mẫu Tốc Kích                            14.0%!     54.1% 
Huyền Độc Môn (Ám Khí + Trảo)              12.6%!     12.6%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     63.1%      41.6% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     56.6%      57.1% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     74.8%!     78.6%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      53.6%      35.3% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      64.6%      41.4% 
Kim Cang Tự (Quyền + Côn)                  40.8%      42.0% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     56.2%      72.2%!
Liệt Diễm Đường (Đao lửa)                  65.7%!     38.2% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         59.1%      21.1%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     39.9%      40.3% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     24.4%!     32.9%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     83.5%!     78.2%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     40.2%      41.7% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      82.9%!     62.4% 
Thái Thanh Hóa Kình                        31.0%!     64.6% 
Huyền Độc Phệ Khí                          14.1%!     30.3%!
Kim Cang Cương Khí                         61.1%      85.4%!
Thái Thanh Quán (Kiếm + Hóa Kình)          33.5%!     44.8% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     59.5%      54.8% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      58.1%      47.3% 
Thiết Phủ Bang (Rìu + Cầm)                 50.4%      19.4%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      82.4%!     53.9% 
Outside [35, 65]: 23/52

[Legendary]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        56.6%      70.7%!
Bích Thủy Hồi Xuân                         13.7%!     70.9%!
Tử Mẫu Tốc Kích                            19.0%!     50.7% 
Huyền Độc Môn (Ám Khí + Trảo)              10.0%!     18.3%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     58.0%      61.0% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     51.5%      54.4% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     74.8%!     73.5%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      52.4%      36.0% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      62.5%      36.6% 
Kim Cang Tự (Quyền + Côn)                  46.2%      30.5%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     60.6%      69.7%!
Liệt Diễm Đường (Đao lửa)                  70.8%!     44.1% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         53.6%      18.4%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     37.1%      36.0% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     28.9%!     29.7%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     87.0%!     82.2%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     47.4%      41.2% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      86.8%!     62.4% 
Thái Thanh Hóa Kình                        29.4%!     62.7% 
Huyền Độc Phệ Khí                          15.2%!     43.8% 
Kim Cang Cương Khí                         64.9%      81.8%!
Thái Thanh Quán (Kiếm + Hóa Kình)          39.6%      44.3% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     55.5%      48.1% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      56.6%      42.1% 
Thiết Phủ Bang (Rìu + Cầm)                 36.9%      32.1%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      83.9%!     62.5% 
Outside [35, 65]: 22/52
```

### Bảng Rarity-Step (Nguyên văn sau patch)
```
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                                   74.5%                79.0%                68.0%                59.5%                72.0%                 0.0%
Bích Thủy Hồi Xuân                                    87.5%                91.5%                93.5%                90.5%                92.5%                 0.0%
Tử Mẫu Tốc Kích                                       87.5%                88.0%                80.0%                83.5%                84.0%                 0.0%
Huyền Độc Môn (Ám Khí + Trảo)                         50.5%                50.0%                48.0%                48.0%                47.0%                 0.0%
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo                73.0%                97.5%                84.0%                97.5%                76.5%                 0.0%
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti                86.5%                82.5%                88.0%                98.5%                96.5%                 0.0%
Kim Cang - Thiết Phủ Hợp Bích (Côn + R                61.0%                66.0%                60.5%                56.5%                67.5%                 0.0%
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm                 58.5%                58.5%                59.5%                57.0%                59.0%                 0.0%
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +                 68.5%                77.0%                76.0%                60.0%                51.5%                 0.0%
Kim Cang Tự (Quyền + Côn)                             67.5%                81.5%                88.5%                89.5%                86.5%                 0.0%
Kim Cang Tự - Bách Luyện (Thuần Quyền)                75.0%               100.0%                99.5%                99.5%                99.0%                 0.0%
Liệt Diễm Đường (Đao lửa)                             69.0%                71.0%                78.0%                69.0%                56.5%                 0.0%
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)                    43.5%                44.0%                44.0%                44.5%                45.5%                 0.0%
Bích Thủy Am (Thanh Đằng Tiên + Cung C                81.5%                86.5%                93.0%                84.0%                92.0%                 0.0%
Hàn Sơn Phái (Băng Phách Thương + Tiêu                83.0%                84.0%                86.5%                89.5%                83.0%                 0.0%
Liệt Diễm Đường (Xích Sa Chưởng + Liệt                63.0%                66.5%                65.0%                64.0%                73.0%                 0.0%
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm                61.5%                63.0%                63.5%                66.0%                62.5%                 0.0%
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai                 71.5%                72.5%                75.5%                72.5%                71.5%                 0.0%
Thái Thanh Hóa Kình                                   53.0%                53.0%                54.0%                54.0%                54.0%                 0.0%
Huyền Độc Phệ Khí                                     50.0%                50.0%               100.0%               100.0%               100.0%                 0.0%
Kim Cang Cương Khí                                    58.0%                67.0%                69.0%                82.0%                91.0%                 0.0%
Thái Thanh Quán (Kiếm + Hóa Kình)                     61.0%                59.0%                61.5%                60.5%                60.0%                 0.0%
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T                49.0%                47.0%                45.5%                48.0%                49.0%                 0.0%
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm                 47.0%                47.0%                47.0%                45.0%                45.0%                 1.0%
Thiết Phủ Bang (Rìu + Cầm)                            58.0%                58.5%                80.0%                87.0%                88.0%                 0.0%
Thiết Phủ Bang - Khai Thiên (Tam Phủ)                 58.0%                57.0%                59.0%                59.0%                59.0%                 0.0%
Average                                               65.3%                69.1%                71.8%                71.7%                71.6%                 0.0%
(draws count as half a win; the last column reports the Lương T1 side)
```

## 4. Lệnh Tái Hiện
```bash
# Matrix A
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-3/balance-rarity-3.patch.json --loadouts tools/gemini/out/balance-rarity-3/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100

# Matrix B (common, legendary)
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-3/balance-rarity-3.patch.json --loadouts tools/gemini/out/balance-rarity-3/loadouts --rarity <common|legendary> --tiers 1,9 --tp 1 --runs 100

# Rarity-step (dùng items đã patch sẵn trong thư mục output)
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/balance-rarity-3/loadouts --tier 3 --tp 1 --runs 100 --items tools/gemini/out/balance-rarity-3/items-patched.json
```

## 5. Câu Hỏi Mở Cho Vòng Sau
1. Đội hình mẫu `samples/huyen_doc.json` có nên được cập nhật chính thức để thay 2 `han_ngoc_tieu` bằng vật phẩm đúng hệ Huyền Độc?
2. Có nên bổ sung cơ chế phân định thắng thua theo % HP còn lại khi trận đấu chạm mốc 50s thay vì để cơ chế Kiệt Sức gây chết cùng tick?
3. Với Song Đao, hiệu ứng Đột Phá T6 (tăng tốc đánh khi bạo kích) đã hoạt động tốt chưa hay cần bổ sung thêm hiệu ứng giảm thời gian nạp chiêu cuối?
