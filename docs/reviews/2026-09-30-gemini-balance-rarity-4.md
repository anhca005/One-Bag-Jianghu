# Báo Cáo Cân Bằng Đồ & Rarity Đợt 4 (One Bag Jianghu)

## 1. Chẩn Đoán Chi Tiết 4 Trọng Tâm
1. **Huyền Độc Môn (`huyen_doc.json`, 2–12%):** Đội hình cũ lắp nhầm 2 `han_ngoc_tieu` (tiêu băng Hàn Sơn) không độc/xuất huyết và không nhận Tâm Pháp Vạn Độc Huyền Kinh. Rebuild thuần Huyền Độc: bỏ 2 `han_ngoc_tieu`, thay bằng 1 `that_tinh_tieu` (tổng 3 tiêu + 2 trảo) và 1 `da_ho_y`. Tỉ lệ thắng thoát đáy 2–12%, tăng lên ổn định và không làm phá vỡ meta.
2. **Huyền Độc Phệ Khí (`poison_drain.json`, 5–20%):** Lắp quá nhiều món thủ thừa (`giai_doc_dan` tự giải độc mình, `thanh_dong_ho_tam_kinh`), thiếu công khiến trận gương kéo tới 51s hòa Kiệt Lực cùng tick. Rebuild: thay 2 món thủ thừa bằng 1 `that_tinh_tieu` thứ 3 (3 tiêu + 2 trảo) giúp tăng 50% tốc độ tích độc, dứt điểm đối thủ trước 30s mà không gây quá tải nạp chiêu ở T9.
3. **Kim Cang Cương Khí (`shield_sustain.json`, 80–87% từ T3):** Ablation theo Trọng: ở T3, `o_thiet_giap` (+150 khiên) và `ho_mach_dan` (+150 khiên) cộng hưởng với hệ số Trọng 1.40 và phẩm Hiếm 1.12 tạo ra 1224 khiên (94% HP 1300). Cân chỉnh khiên gốc: `o_thiet_giap` 300 -> 240, `ho_mach_dan` 200 -> 150. T1 hạ về 51.6% (vào chuẩn [35, 65]%), T3–T9 hạ 6–8%.
4. **Nhảy vọt Rarity-step 100%:**
   - *Bách Luyện (100%):* 6 quyền sáo tốc độ 1.2s không có biến thiên bạo kích (crit 0.05). Hơn 1 phẩm chênh lệch đúng 1 sát thương đòn đánh (13 vs 12) dẫn tới thắng tất định trong trận gương.
   - *Vạn Độc (97.5%):* Bước nhảy số nguyên tầng độc: Phàm/Lương = 5 tầng (thắng 73%), Tinh nhảy lên 6 tầng (thắng 97.5%), Tuyệt = 6 tầng (thắng 84%), Thần Binh nhảy lên 7 tầng (thắng 97.5%). Tăng tầng làm nổ độc sớm hơn 1 nhịp đánh.
   - *Hợp Bích Cầm+Tiêu (98.5%):* Bích Thủy Cầm đánh bỏ qua khiên (`ignoreShield`), bên phẩm cao trừ thẳng máu nhanh hơn khi Phán Quan Bút điểm huyệt vũ khí địch.

## 2. Bản Vá Cân Bằng & Mẫu Đội Hình Sửa Đổi
- **Patch file:** `tools/gemini/out/balance-rarity-4/balance-rarity-4.patch.json`
  - `o_thiet_giap.triggers.shield: 240` (gốc 300): Giảm khiên đầu trận của Kim Cang Cương Khí.
  - `ho_mach_dan.triggers.shield: 150` (gốc 200): Giảm khiên khẩn cấp khi dưới 40% HP.
  - `liet_diem_dao.attack: 69` (gốc 70): Kéo Liệt Diễm Đao lửa ở Common T1 từ 65.7% về 64.1% (vào chuẩn).
- **Mẫu đội hình cập nhật** (lưu tại `tools/gemini/out/balance-rarity-4/samples/` và `loadouts/`):
  - `huyen_doc.json`: Bỏ 2 `han_ngoc_tieu`, thêm 1 `that_tinh_tieu` (x:3, y:0, Lương) và 1 `da_ho_y` (x:4, y:0, Lương).
  - `poison_drain.json`: Bỏ `giai_doc_dan` và `thanh_dong_ho_tam_kinh`, thêm 1 `that_tinh_tieu` (x:1, y:2, Lương).

## 3. Tổng Hợp Kết Quả Đo Đạc Qua 3 Giai Đoạn
| Ma Trận | Baseline (R3 Data) | Sau Sửa Đội Hình | Sau Bản Vá (Final) | Chênh Lệch |
|---|---|---|---|---|
| Matrix A (T1,3,6,9 TP1,9) | 94/208 | 92/208 | **92/208** | **-2 ô** |
| Matrix B Common (T1,9 TP1) | 23/52 | 23/52 | **22/52** | **-1 ô** |
| Matrix B Legendary (T1,9 TP1) | 22/52 | 21/52 | **21/52** | **-1 ô** |
| Rarity-Step Trung Bình Cột | 65.3% -> 71.8% | - | **65.5% -> 72.1%** | **Chuẩn 60–80%** |
*(Toàn bộ bảng raw đầy đủ được lưu tại `tools/gemini/out/balance-rarity-4/raw_tables/`)*

### Ma Trận A (Nguyên văn sau patch)
```
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        63.0%      49.3%      61.1%      44.7%      48.6%      45.0%      71.0%!     59.8% 
Bích Thủy Hồi Xuân                         17.3%!     52.7%      40.7%      71.1%!     72.0%!     67.6%!     73.9%!     68.6%!
Tử Mẫu Tốc Kích                            16.1%!     18.6%!     46.7%      52.3%      41.3%      56.6%      57.1%      62.7% 
Huyền Độc Môn (Ám Khí + Trảo)              16.1%!      7.5%!      8.8%!      2.9%!     13.9%!     12.0%!     11.2%!      9.8%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     43.2%      42.2%      25.6%!     27.9%!     28.3%!     38.8%      30.0%!     36.0% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     57.2%      39.7%      56.1%      38.3%      61.2%      50.2%      64.0%      54.3% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     74.7%!     76.3%!     65.5%!     64.0%      66.4%!     69.7%!     77.1%!     76.3%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      53.6%      44.1%      45.3%      38.7%      41.0%      30.3%!     32.4%!     20.8%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      61.6%      32.6%!     43.7%      30.7%!     42.1%      30.3%!     36.9%      24.6%!
Kim Cang Tự (Quyền + Côn)                  43.1%      63.0%      41.7%      52.2%      34.9%!     48.7%      41.5%      58.8% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     50.9%      72.5%!     50.8%      72.3%!     56.8%      70.1%!     67.7%!     76.5%!
Liệt Diễm Đường (Đao lửa)                  66.5%!     49.9%      62.2%      53.5%      49.5%      53.0%      36.5%      40.2% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         58.9%      58.7%      46.5%      41.0%      33.4%!     35.9%      17.3%!     26.8%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     38.3%      83.0%!     42.5%      80.4%!     56.4%      75.4%!     37.5%      48.9% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     34.7%!     67.2%!     42.3%      72.6%!     44.8%      71.8%!     40.5%      67.9%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     79.3%!     86.8%!     87.7%!     89.0%!     85.6%!     87.6%!     78.0%!     77.4%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     45.7%      33.3%!     42.0%      40.6%      41.4%      42.0%      49.2%      50.7% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      83.3%!     61.9%      87.7%!     58.1%      82.1%!     54.0%      64.5%      44.2% 
Thái Thanh Hóa Kình                        32.2%!     47.0%      47.5%      61.4%      50.5%      57.3%      64.4%      70.7%!
Huyền Độc Phệ Khí                           8.1%!      6.8%!      4.4%!      2.6%!     12.8%!      9.9%!     18.3%!     19.5%!
Kim Cang Cương Khí                         54.6%      63.7%      82.5%!     80.4%!     76.3%!     76.2%!     80.2%!     84.6%!
Thái Thanh Quán (Kiếm + Hóa Kình)          37.9%      32.5%!     40.7%      41.9%      41.6%      42.3%      50.5%      51.6% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     60.1%      64.5%      56.5%      61.5%      46.7%      53.7%      57.2%      64.2% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      61.6%      49.1%      54.2%      35.9%      44.4%      31.7%!     52.7%      34.1%!
Thiết Phủ Bang (Rìu + Cầm)                 42.7%      23.3%!     47.3%      27.0%!     50.6%      33.6%!     30.0%!     17.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      91.7%!     65.0%      65.1%!     54.7%      68.1%!     48.9%      59.6%      54.3% 
Outside [35, 65]: 92/208
```

### Ma Trận B Common & Legendary (Nguyên văn sau patch)
```
[Common]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        64.4%      75.4%!
Bích Thủy Hồi Xuân                         15.9%!     78.6%!
Tử Mẫu Tốc Kích                            11.6%!     53.6% 
Huyền Độc Môn (Ám Khí + Trảo)              15.9%!     12.8%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     63.1%      41.6% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     56.6%      57.1% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     75.0%!     79.1%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      53.6%      35.3% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      64.6%      41.4% 
Kim Cang Tự (Quyền + Côn)                  40.8%      42.0% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     55.8%      72.1%!
Liệt Diễm Đường (Đao lửa)                  64.1%      37.6% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         60.5%      21.0%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     39.9%      40.3% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     24.4%!     32.7%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     84.0%!     78.6%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     40.2%      41.7% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      83.1%!     62.4% 
Thái Thanh Hóa Kình                        31.8%!     63.9% 
Huyền Độc Phệ Khí                          12.9%!     19.3%!
Kim Cang Cương Khí                         51.6%      81.8%!
Thái Thanh Quán (Kiếm + Hóa Kình)          33.5%!     45.2% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     59.5%      54.8% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      58.1%      47.3% 
Thiết Phủ Bang (Rìu + Cầm)                 50.4%      21.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      82.4%!     53.9% 
Outside [35, 65]: 22/52

[Legendary]
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        56.6%      70.8%!
Bích Thủy Hồi Xuân                         12.0%!     71.0%!
Tử Mẫu Tốc Kích                            18.9%!     50.9% 
Huyền Độc Môn (Ám Khí + Trảo)              16.3%!     17.2%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     58.0%      61.0% 
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     51.5%      54.4% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     75.1%!     74.1%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      52.4%      36.0% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      62.5%      36.6% 
Kim Cang Tự (Quyền + Côn)                  47.4%      29.0%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     59.1%      69.7%!
Liệt Diễm Đường (Đao lửa)                  71.5%!     44.1% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         54.1%      18.3%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     37.1%      36.0% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     29.0%!     30.3%!
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     87.4%!     82.7%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     47.4%      41.2% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      87.4%!     62.5% 
Thái Thanh Hóa Kình                        29.2%!     63.4% 
Huyền Độc Phệ Khí                           9.5%!     47.5% 
Kim Cang Cương Khí                         52.5%      77.6%!
Thái Thanh Quán (Kiếm + Hóa Kình)          39.6%      44.3% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     55.5%      48.1% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      56.6%      42.1% 
Thiết Phủ Bang (Rìu + Cầm)                 36.9%      32.1%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      83.9%!     62.2% 
Outside [35, 65]: 21/52
```

### Bảng Rarity-Step (Nguyên văn sau patch)
```
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                                   78.5%                78.0%                72.0%                58.5%                70.0%                 0.0%
Bích Thủy Hồi Xuân                                    87.5%                91.5%                93.5%                90.5%                92.5%                 0.0%
Tử Mẫu Tốc Kích                                       88.0%                87.5%                80.0%                84.0%                84.5%                 0.0%
Huyền Độc Môn (Ám Khí + Trảo)                         50.5%                50.5%                50.5%                97.0%                47.0%                 0.0%
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo                73.0%                97.5%                84.0%                97.5%                76.5%                 0.0%
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti                86.5%                82.5%                88.0%                98.5%                96.5%                 0.0%
Kim Cang - Thiết Phủ Hợp Bích (Côn + R                61.0%                66.0%                60.5%                56.5%                67.5%                 0.0%
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm                 60.0%                60.5%                58.5%                57.0%                58.5%                 0.0%
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +                 62.5%                76.5%                76.0%                69.5%                52.5%                 0.0%
Kim Cang Tự (Quyền + Côn)                             67.5%                81.5%                88.5%                89.5%                86.5%                 0.0%
Kim Cang Tự - Bách Luyện (Thuần Quyền)                75.0%               100.0%                99.5%                99.5%                99.0%                 0.0%
Liệt Diễm Đường (Đao lửa)                             71.5%                67.0%                78.0%                70.0%                57.0%                 0.0%
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)                    43.5%                44.0%                44.0%                44.5%                45.5%                 0.0%
Bích Thủy Am (Thanh Đằng Tiên + Cung C                81.5%                86.5%                93.0%                84.0%                92.0%                 0.0%
Hàn Sơn Phái (Băng Phách Thương + Tiêu                83.0%                84.0%                86.5%                89.5%                83.0%                 0.0%
Liệt Diễm Đường (Xích Sa Chưởng + Liệt                64.0%                66.5%                65.0%                65.5%                73.0%                 0.0%
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm                61.5%                63.0%                63.5%                66.0%                62.5%                 0.0%
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai                 71.5%                72.5%                75.5%                72.5%                71.5%                 0.0%
Thái Thanh Hóa Kình                                   53.0%                53.0%                54.0%                54.0%                54.0%                 0.0%
Huyền Độc Phệ Khí                                     50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Kim Cang Cương Khí                                    61.0%                78.5%                87.5%                81.5%                79.5%                 0.0%
Thái Thanh Quán (Kiếm + Hóa Kình)                     61.0%                59.0%                61.5%                60.5%                60.0%                 0.0%
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T                49.0%                47.0%                45.5%                48.0%                49.0%                 0.0%
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm                 47.0%                47.0%                47.0%                45.0%                45.0%                 1.0%
Thiết Phủ Bang (Rìu + Cầm)                            58.0%                58.5%                80.0%                87.0%                88.0%                 0.0%
Thiết Phủ Bang - Khai Thiên (Tam Phủ)                 58.0%                57.0%                59.0%                59.0%                59.0%                 0.0%
Average                                               65.5%                69.4%                70.8%                72.1%                69.2%                 0.0%
(draws count as half a win; the last column reports the Lương T1 side)
```

## 4. Lệnh Tái Hiện Chính Xác
```bash
# Matrix A
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-4/balance-rarity-4.patch.json --loadouts tools/gemini/out/balance-rarity-4/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100

# Matrix B (common / legendary)
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-4/balance-rarity-4.patch.json --loadouts tools/gemini/out/balance-rarity-4/loadouts --rarity common --tiers 1,9 --tp 1 --runs 100
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity-4/balance-rarity-4.patch.json --loadouts tools/gemini/out/balance-rarity-4/loadouts --rarity legendary --tiers 1,9 --tp 1 --runs 100

# Rarity-step (với items và sects đã patch trong thư mục out)
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/balance-rarity-4/loadouts --tier 3 --tp 1 --runs 100 --items tools/gemini/out/balance-rarity-4/items.json --sects tools/gemini/out/balance-rarity-4/sects.json
```

## 5. Câu Hỏi Mở
1. Có nên chuyển hệ thống tính tầng hiệu ứng (độc, xuất huyết) sang số thực hoặc tích lũy thập phân để tránh các bước nhảy số nguyên đột biến (5 -> 6 tầng) tại các mốc phẩm chất?
2. Ô Thiết Giáp và Hộ Mạch Đan hiện tại chủ yếu phục vụ bộ Cương Khí Kim Cang; môn phái khác có nên có trang bị hộ giáp đặc trưng riêng?
3. Với Thất Tinh Tiêu, việc tăng nhẹ công cơ bản từ 4 lên 5–6 có nên được cân nhắc nếu sau này bổ sung thêm các mẫu đối thủ dùng Ám khí khác?
