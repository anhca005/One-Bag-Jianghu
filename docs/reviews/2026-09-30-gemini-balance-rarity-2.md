# Báo Cáo Cân Bằng Phẩm Chất Đồ & Trang Bị (Lần 2)

## 1. Hệ Số Phẩm Chất & Kiểm Thử Rarity-Step
Hệ số cũ `[1.0, 1.25, 1.55, 1.90, 2.30, 2.75]` tạo bước nhảy quá dốc (thắng trung bình 87.7% khi hơn 1 phẩm). Đã thử nghiệm 7 bộ bước nhảy (15%, 12%, 10%, 8%, 6%).
Bộ tối ưu đạt khoảng 65–75%: `[1000, 1060, 1120, 1190, 1260, 1340]` (bước nhảy ~6%, TB toàn bộ các bước đạt **70.1%**).
File config: `tools/gemini/out/balance-rarity-2/battle-config.json`.

Bảng `rarity-step` nguyên văn:
```
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                                   65.0%                57.5%                67.0%                78.5%                85.0%                 0.0%
Bích Thủy Hồi Xuân                                    87.5%                91.5%                93.5%                90.5%                92.5%                 0.0%
Tử Mẫu Tốc Kích                                       84.0%                86.0%                83.5%                80.5%                85.5%                 0.0%
Huyền Độc Môn (Ám Khí + Trảo)                         48.5%                48.5%               100.0%                50.0%               100.0%                 0.0%
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo                46.0%                46.0%               100.0%                50.5%               100.0%                 0.0%
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti                86.5%                82.5%                88.0%                98.5%                96.5%                 0.0%
Kim Cang - Thiết Phủ Hợp Bích (Côn + R                60.0%                60.0%                61.5%                63.0%                70.5%                 0.0%
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm                 56.0%                56.5%                55.0%                58.5%                58.5%                 0.0%
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +                 68.0%                74.5%                50.0%                52.5%                49.0%                 0.0%
Kim Cang Tự (Quyền + Côn)                             80.0%                88.0%                87.5%                79.0%                67.0%                 0.0%
Kim Cang Tự - Bách Luyện (Thuần Quyền)                72.0%                98.5%               100.0%                96.0%                97.5%                 0.0%
Liệt Diễm Đường (Đao lửa)                             57.0%                64.5%                66.5%                66.0%                77.0%                 0.0%
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)                    44.5%                45.5%                44.0%                44.0%                44.0%                 0.0%
Bích Thủy Am (Thanh Đằng Tiên + Cung C                81.5%                86.5%                93.0%                84.0%                92.0%                 0.0%
Hàn Sơn Phái (Băng Phách Thương + Tiêu                83.0%                84.0%                86.5%                89.5%                83.0%                 0.0%
Liệt Diễm Đường (Xích Sa Chưởng + Liệt                70.5%                77.0%                75.5%                74.0%                82.5%                 0.0%
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm                61.5%                63.0%                63.5%                66.0%                62.5%                 0.0%
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai                 67.5%                64.0%                71.0%                69.5%                74.5%                 0.0%
Thái Thanh Hóa Kình                                   53.0%                53.0%                54.0%                54.0%                54.0%                 0.0%
Huyền Độc Phệ Khí                                     50.0%                50.0%               100.0%               100.0%               100.0%                 0.0%
Kim Cang Cương Khí                                    81.5%                89.5%                93.5%                78.5%                79.0%                 0.0%
Thái Thanh Quán (Kiếm + Hóa Kình)                     61.0%                59.0%                61.5%                60.5%                60.0%                 0.0%
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T                49.0%                47.0%                45.5%                48.0%                49.0%                 0.0%
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm                 47.0%                46.0%                48.0%                45.0%                45.0%                 0.0%
Thiết Phủ Bang (Rìu + Cầm)                            58.0%                58.5%                80.0%                87.0%                88.0%                 0.0%
Thiết Phủ Bang - Khai Thiên (Tam Phủ)                 58.0%                57.0%                59.0%                59.0%                59.0%                 0.0%
Average                                               64.5%                66.7%                74.1%                70.1%                75.1%                 0.0%
(draws count as half a win; the last column reports the Lương T1 side)
```

## 2. Kết Quả Cân Bằng & Bảng Ma Trận
Baseline (với config mới): A = 103/208, B-Common = 29/52, B-Rare = 26/52, B-Legendary = 28/52, B-Supreme = 28/52.
Sau Patch (`tools/gemini/out/balance-rarity-2/balance-rarity-2.patch.json`):
- **Matrix A**: giảm từ 103 xuống **98/208** (-5 ô lệch).
- **Matrix B Common**: giảm từ 29 xuống **24/52** (-5 ô lệch).
- **Matrix B Rare**: giảm từ 26 xuống **24/52** (-2 ô lệch).
- **Matrix B Legendary**: giảm từ 28 xuống **23/52** (-5 ô lệch).
- **Matrix B Supreme**: giảm từ 28 xuống **25/52** (-3 ô lệch).
Toàn bộ 5 ma trận đều giảm nghiêm ngặt số ô ngoài ngưỡng [35%, 65%].

### Ma Trận A (Nguyên văn sau patch)
```
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        65.3%!     51.9%      65.4%!     49.9%      53.4%      47.4%      72.6%!     62.9% 
Bích Thủy Hồi Xuân                         20.0%!     55.6%      43.7%      75.1%!     74.2%!     70.5%!     75.8%!     70.6%!
Tử Mẫu Tốc Kích                             1.0%!      6.5%!      4.0%!     16.5%!      6.7%!     24.3%!     23.5%!     42.3% 
Huyền Độc Môn (Ám Khí + Trảo)              14.4%!      9.5%!     10.2%!      2.9%!     14.9%!     13.6%!     10.7%!      7.9%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     38.3%      36.3%      25.1%!     32.4%!     29.4%!     40.1%      32.8%!     35.9% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     60.5%      41.4%      59.5%      40.3%      65.5%!     52.6%      67.0%!     57.9% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     76.0%!     76.6%!     69.0%!     66.2%!     69.3%!     72.7%!     78.5%!     78.4%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      55.8%      47.9%      48.1%      41.8%      44.3%      31.7%!     34.4%!     24.2%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      63.4%      36.9%      49.2%      33.0%!     45.1%      32.9%!     41.0%      26.4%!
Kim Cang Tự (Quyền + Côn)                  47.3%      64.9%      49.9%      56.3%      38.6%      53.3%      45.3%      62.1% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     52.8%      75.8%!     54.6%      75.5%!     59.8%      72.5%!     71.0%!     77.1%!
Liệt Diễm Đường (Đao lửa)                  68.7%!     53.3%      67.5%!     57.8%      53.3%      56.5%      38.9%      42.8% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         58.3%      59.1%      47.1%      45.4%      36.3%      36.5%      19.9%!     29.1%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     40.6%      85.4%!     45.5%      83.7%!     58.9%      79.2%!     40.7%      52.1% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     35.8%      69.3%!     46.3%      75.1%!     48.5%      75.0%!     43.7%      70.8%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     53.5%      62.0%      68.1%!     67.7%!     66.3%!     70.7%!     61.8%      66.1%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     46.8%      37.5%      46.0%      44.7%      43.9%      45.7%      51.3%      52.5% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      62.0%      39.2%      69.1%!     37.5%      60.4%      40.1%      45.0%      26.9%!
Thái Thanh Hóa Kình                        34.3%!     51.5%      50.1%      64.1%      53.0%      59.5%      65.3%!     70.6%!
Huyền Độc Phệ Khí                          11.5%!      5.9%!      7.5%!      4.7%!     19.3%!     16.5%!     21.5%!     18.2%!
Kim Cang Cương Khí                         91.8%!     87.5%!     92.4%!     91.3%!     88.9%!     87.1%!     88.5%!     88.4%!
Thái Thanh Quán (Kiếm + Hóa Kình)          38.5%      36.2%      44.5%      45.5%      44.2%      44.9%      53.4%      54.8% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     61.9%      67.0%!     60.9%      65.2%!     50.9%      57.4%      61.2%      67.7%!
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      62.9%      52.1%      56.5%      40.0%      47.0%      34.6%!     56.9%      37.9% 
Thiết Phủ Bang (Rìu + Cầm)                 45.1%      24.6%!     51.2%      29.4%!     55.0%      37.2%      34.1%!     20.5%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      90.3%!     66.6%!     66.6%!     57.2%      71.9%!     50.2%      64.5%      56.6% 
Outside [35, 65]: 98/208
```

### Ma Trận B (Nguyên văn sau patch)
```
[Common]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        66.0%!     75.6%!
Bích Thủy Hồi Xuân                         20.4%!     80.5%!
Tử Mẫu Tốc Kích                             0.3%!     24.3%!
Huyền Độc Môn (Ám Khí + Trảo)              14.6%!     14.3%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     55.2%      43.5% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     58.9%      60.4% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     76.1%!     79.1%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      55.4%      37.9% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      66.5%!     45.6% 
Kim Cang Tự (Quyền + Côn)                  42.3%      45.5% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     60.1%      74.5%!
Liệt Diễm Đường (Đao lửa)                  66.1%!     41.3% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         59.4%      23.7%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     43.2%      43.3% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     24.9%!     35.5% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     61.5%      63.0% 
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     40.9%      43.3% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      61.1%      40.8% 
Thái Thanh Hóa Kình                        31.4%!     66.1%!
Huyền Độc Phệ Khí                          14.5%!     32.3%!
Kim Cang Cương Khí                         86.5%!     90.4%!
Thái Thanh Quán (Kiếm + Hóa Kình)          34.2%!     47.4% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     62.2%      58.4% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      60.3%      51.7% 
Thiết Phủ Bang (Rìu + Cầm)                 54.0%      23.3%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      82.8%!     58.4% 
Outside [35, 65]: 24/52

[Rare]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        67.9%!     72.6%!
Bích Thủy Hồi Xuân                         20.1%!     78.4%!
Tử Mẫu Tốc Kích                             1.2%!     25.0%!
Huyền Độc Môn (Ám Khí + Trảo)               9.3%!     10.4%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     43.9%      33.6%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     54.2%      60.0% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     77.1%!     79.9%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      56.5%      37.9% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      63.9%      43.4% 
Kim Cang Tự (Quyền + Côn)                  44.1%      46.4% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     65.0%      74.3%!
Liệt Diễm Đường (Đao lửa)                  71.1%!     42.6% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         59.0%      22.8%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     43.1%      44.4% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     25.9%!     37.3% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     60.3%      66.7%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     46.6%      48.6% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      62.9%      45.2% 
Thái Thanh Hóa Kình                        32.6%!     66.2%!
Huyền Độc Phệ Khí                          11.2%!     21.9%!
Kim Cang Cương Khí                         90.6%!     90.2%!
Thái Thanh Quán (Kiếm + Hóa Kình)          38.1%      51.1% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     60.1%      58.5% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      58.6%      53.1% 
Thiết Phủ Bang (Rìu + Cầm)                 45.9%      25.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      88.3%!     61.8% 
Outside [35, 65]: 24/52

[Legendary]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        58.0%      73.3%!
Bích Thủy Hồi Xuân                         18.4%!     73.6%!
Tử Mẫu Tốc Kích                             2.6%!     23.1%!
Huyền Độc Môn (Ám Khí + Trảo)              10.8%!     20.4%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     52.1%      60.7% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     52.7%      57.3% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     77.2%!     74.8%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      55.8%      38.5% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      64.6%      41.7% 
Kim Cang Tự (Quyền + Côn)                  48.9%      34.0%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     63.2%      71.7%!
Liệt Diễm Đường (Đao lửa)                  71.8%!     46.9% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         54.5%      20.9%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     38.7%      39.3% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     30.6%!     32.4%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     66.3%!     71.7%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     49.4%      43.1% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      63.0%      42.1% 
Thái Thanh Hóa Kình                        31.7%!     63.5% 
Huyền Độc Phệ Khí                          14.8%!     43.3% 
Kim Cang Cương Khí                         90.0%!     84.7%!
Thái Thanh Quán (Kiếm + Hóa Kình)          40.8%      46.8% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     58.8%      51.4% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      59.8%      46.5% 
Thiết Phủ Bang (Rìu + Cầm)                 37.1%      32.8%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      85.0%!     64.3% 
Outside [35, 65]: 23/52

[Supreme]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        61.9%      77.5%!
Bích Thủy Hồi Xuân                         18.6%!     75.5%!
Tử Mẫu Tốc Kích                             3.3%!     23.9%!
Huyền Độc Môn (Ám Khí + Trảo)              10.6%!     15.7%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     39.0%      53.2% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     56.2%      51.8% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     80.3%!     74.8%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      53.7%      40.6% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      65.2%!     45.0% 
Kim Cang Tự (Quyền + Côn)                  50.1%      31.8%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     56.8%      69.1%!
Liệt Diễm Đường (Đao lửa)                  71.7%!     46.3% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         51.9%      24.0%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     42.4%      39.5% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     33.7%!     36.1% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     69.5%!     70.3%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     51.9%      44.5% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      68.8%!     42.7% 
Thái Thanh Hóa Kình                        30.4%!     63.4% 
Huyền Độc Phệ Khí                          13.9%!     34.3%!
Kim Cang Cương Khí                         88.9%!     87.2%!
Thái Thanh Quán (Kiếm + Hóa Kình)          40.7%      47.4% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     57.0%      55.1% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      57.8%      50.4% 
Thiết Phủ Bang (Rìu + Cầm)                 40.7%      33.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      84.5%!     63.1% 
Outside [35, 65]: 25/52
```

## 3. Chi Tiết Patch & Lý Do Thiết Kế
1. `thiet_cot_quyen_sao.attack: 9` (gốc 11): Giảm sát thương quyền cơ bản, kiềm chế Kim Cang thuần quyền đầu/giữa game mà không phá hủy lối đánh thủ phản.
2. `that_tinh_tieu.basic.poison: 4` (gốc 5) & `cooldown: 2.3` (gốc 2.0): Giảm tốc độ tích stack độc bộc phát ở cấp cao, giúp các phái khác có cửa thở.
3. `liet_diem_dao.attack: 70` (gốc 75): Giảm nhẹ sát thương đao để tránh Liệt Diễm bùng nổ quá đà sau khi Kim Cang bị nerf.

## 4. Lệnh Tái Hiện
```bash
# Rarity-step
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/balance-rarity-2/loadouts --tier 3 --tp 1 --runs 100 --battle-config tools/gemini/out/balance-rarity-2/battle-config.json

# Matrix A
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-2/balance-rarity-2.patch.json --loadouts tools/gemini/out/balance-rarity-2/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100 --battle-config tools/gemini/out/balance-rarity-2/battle-config.json

# Matrix B (common, rare, legendary, supreme)
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-2/balance-rarity-2.patch.json --loadouts tools/gemini/out/balance-rarity-2/loadouts --rarity <common|rare|legendary|supreme> --tiers 1,9 --tp 1 --runs 100 --battle-config tools/gemini/out/balance-rarity-2/battle-config.json
```

## 5. Câu Hỏi Mở
1. Có nên giữ nguyên bước nhảy phẩm chất 6% cho toàn bộ game hay tăng nhẹ ở 2 bậc cuối (Thần Binh / Chí Tôn)?
2. Đội hình Ám Khí (Huyền Độc) phụ thuộc lớn vào tốc độ dồn stack độc; có nên xem xét cơ chế giải độc / kháng độc theo thời gian?
3. Khi áp dụng vào thực tế, người chơi có thể phối hợp nhiều item non-weapon phòng thủ hơn; ta có cần một bộ test bench riêng cho meta full thủ?
