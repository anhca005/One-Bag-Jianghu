# Báo Cáo Cân Bằng Hệ Thống Phẩm Chất Bản Sao & 40 Vật Phẩm Mới — Nhất Nang Giang Hồ

**Căn cứ:** Quyết định #12–#15, #24, #25; `docs/design/balance.md`, `combat.md`, `glossary.md`.  
**Tệp xuất:** `tools/gemini/out/balance-rarity.patch.json`, `tools/gemini/out/balance-rarity/loadouts/`.

---

## 1. Bảng Kết Quả Cơ Sở (Baseline) & Kết Quả Sau Patch (Final)

### 1.1. Ma Trận A (Native Rarities: 20 mẫu giữ phẩm gốc + 6 đội phi binh khí phối phẩm)

#### Cơ Sở A (Baseline A):
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                        12.1%!     12.2%!     12.1%!     12.7%!     10.9%!     14.5%!     90.2%!     83.4%!
Bích Thủy Hồi Xuân                          8.0%!      8.5%!     10.9%!      8.1%!      8.5%!      8.0%!      0.0%!      0.9%!
Tử Mẫu Tốc Kích                             0.0%!      0.0%!      0.0%!      0.0%!      2.1%!      0.0%!     17.3%!      5.9%!
Huyền Độc Môn (Ám Khí + Trảo)              31.4%!     31.4%!     28.6%!     33.7%!     31.0%!     33.1%!     35.4%      43.7% 
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     63.5%      64.4%      37.2%      60.5%      63.8%      78.0%!     64.1%      75.1%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     55.4%      56.1%      63.2%      59.2%      67.2%!     67.6%!     62.5%      61.1% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     81.6%!     75.3%!     75.3%!     84.1%!     79.2%!     76.2%!     77.3%!     74.4%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      67.3%!     56.9%      64.8%      51.7%      53.6%      39.4%      43.2%      34.9%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      70.3%!     52.6%      58.4%      43.0%      59.8%      41.0%      41.8%      32.5%!
Kim Cang Tự (Quyền + Côn)                  69.0%!     80.7%!     65.1%!     70.7%!     43.6%      66.1%!     58.8%      66.3%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     59.1%      67.6%!     60.6%      73.0%!     59.0%      72.9%!     63.3%      77.3%!
Liệt Diễm Đường (Đao lửa)                  83.7%!     75.8%!     80.5%!     79.7%!     67.8%!     69.5%!     58.5%      62.6% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         70.9%!     69.6%!     52.3%      51.3%      52.9%      61.6%      43.3%      47.1% 
Bích Thủy Am (Thanh Đằng Tiên + Cung C     37.5%      75.4%!     44.3%      71.8%!     55.4%      71.4%!     38.8%      54.6% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     50.2%      66.0%!     68.3%!     69.0%!     62.8%      66.3%!     62.4%      63.3% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     55.4%      52.3%      61.2%      53.3%      63.6%      61.6%      61.8%      54.3% 
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     50.3%      54.6%      57.0%      59.5%      54.8%      60.1%      60.1%      58.1% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      68.9%!     48.4%      70.9%!     51.5%      72.9%!     52.7%      48.2%      34.6%!
Thái Thanh Hóa Kình                        20.8%!     25.6%!     26.6%!     32.5%!     39.8%      47.4%      54.9%      66.9%!
Huyền Độc Phệ Khí                           4.0%!      4.0%!      7.7%!      5.0%!     18.5%!     18.1%!     19.7%!     19.5%!
Kim Cang Cương Khí                         17.5%!     18.8%!     14.8%!     32.1%!     21.6%!     37.1%      42.6%      45.1% 
Thái Thanh Quán (Kiếm + Hóa Kình)          46.1%      51.4%      54.7%      57.9%      55.0%      55.9%      55.9%      57.6% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     67.7%!     77.2%!     65.3%!     65.3%!     58.3%      59.1%      54.0%      63.1% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      73.3%!     63.7%      73.2%!     53.6%      63.0%      38.1%      53.2%      39.9% 
Thiết Phủ Bang (Rìu + Cầm)                 64.0%      41.6%      67.2%!     48.7%      56.5%      46.3%      31.1%!     26.3%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      71.0%!     72.4%!     77.5%!     70.7%!     78.3%!     61.2%      61.6%      50.6% 
Outside [35, 65]: 107/208
```

#### Sau Patch A (Final A):
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Liệt Hỏa Thiêu Thân                         8.2%!      4.6%!      5.4%!      2.3%!      3.6%!      5.8%!     89.3%!     83.0%!
Bích Thủy Hồi Xuân                         10.7%!     17.4%!     16.5%!     15.6%!     12.9%!     13.7%!      3.3%!      0.6%!
Tử Mẫu Tốc Kích                             6.0%!      1.5%!      8.9%!      2.9%!     28.6%!     10.2%!     63.5%      36.4% 
Huyền Độc Môn (Ám Khí + Trảo)              35.9%      39.7%      36.7%      41.4%      35.7%      43.8%      36.7%      50.2% 
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     69.7%!     70.8%!     56.4%      70.2%!     70.2%!     83.1%!     71.0%!     74.3%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     69.7%!     64.2%      74.4%!     62.9%      72.9%!     70.3%!     66.9%!     71.4%!
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     74.8%!     80.5%!     73.2%!     78.4%!     72.7%!     71.2%!     74.3%!     74.0%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      56.8%      53.3%      50.5%      45.7%      45.1%      36.0%      32.7%!     24.0%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      70.7%!     50.9%      45.5%      38.1%      51.5%      37.8%      44.6%      29.9%!
Kim Cang Tự (Quyền + Côn)                  66.1%!     82.0%!     66.2%!     75.3%!     50.8%      64.9%      50.2%      66.1%!
Kim Cang Tự - Bách Luyện (Thuần Quyền)     65.5%!     81.4%!     73.5%!     81.8%!     67.9%!     81.0%!     71.0%!     80.1%!
Liệt Diễm Đường (Đao lửa)                  73.2%!     66.3%!     68.4%!     71.7%!     55.3%      60.4%      52.9%      50.4% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         73.7%!     70.7%!     54.0%      58.0%      49.7%      61.5%      30.4%!     35.5% 
Bích Thủy Am (Thanh Đằng Tiên + Cung C     45.8%      89.8%!     57.4%      92.1%!     75.7%!     89.5%!     49.9%      74.2%!
Hàn Sơn Phái (Băng Phách Thương + Tiêu     56.2%      64.7%      65.8%!     68.2%!     68.1%!     67.8%!     58.0%      61.2% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     66.9%!     56.7%      78.6%!     66.3%!     77.9%!     61.3%      62.4%      55.7% 
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     34.0%!     38.4%      39.7%      43.3%      38.4%      45.1%      35.3%      44.0% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      80.4%!     58.4%      81.3%!     65.9%!     83.1%!     61.8%      62.9%      47.9% 
Thái Thanh Hóa Kình                        17.6%!     22.0%!     22.7%!     30.3%!     34.3%!     47.9%      51.4%      64.8% 
Huyền Độc Phệ Khí                           0.6%!      7.8%!      5.9%!      9.8%!     17.3%!     17.5%!     27.6%!     28.8%!
Kim Cang Cương Khí                         19.8%!     20.0%!     23.0%!     44.3%      21.2%!     41.3%      41.4%      49.9% 
Thái Thanh Quán (Kiếm + Hóa Kình)          32.6%!     33.7%!     34.1%!     38.5%      33.2%!     39.4%      27.9%!     35.8% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     74.4%!     60.3%      65.1%!     53.7%      57.6%      45.6%      50.2%      53.8% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      76.0%!     70.0%!     72.6%!     61.9%      63.8%      50.2%      50.8%      51.1% 
Thiết Phủ Bang (Rìu + Cầm)                 53.2%      38.2%      46.7%      32.3%!     56.5%      43.9%      27.7%!     18.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      62.6%      56.3%      78.1%!     49.1%      57.1%      46.5%      62.4%      39.5% 
Outside [35, 65]: 116/208
```

---

### 1.2. Ma Trận B (Fair PvP: Cùng Phẩm Chất Mọi Món Đồ)

#### B Phàm (Common) — Trước vs Sau Patch:
```text
Loadout                                    Trước T1   Trước T9   |   Sau T1     Sau T9
-----------------------------------------------------------------+--------------------
Liệt Hỏa Thiêu Thân                        13.2%!     80.7%!     |    9.5%!     81.5%!
Bích Thủy Hồi Xuân                          8.0%!      0.0%!     |    6.9%!      2.6%!
Tử Mẫu Tốc Kích                             0.0%!     22.3%!     |    8.8%!     71.3%!
Huyền Độc Môn (Ám Khí + Trảo)              27.8%!     38.8%      |   40.3%      38.2% 
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     74.2%!     92.7%!     |   92.5%!     86.4%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     57.0%      62.4%      |   68.1%!     72.0%!
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     79.7%!     81.7%!     |   72.9%!     82.4%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      70.3%!     49.8%      |   60.9%      41.4% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      77.9%!     50.7%      |   72.7%!     48.1% 
Kim Cang Tự (Quyền + Côn)                  57.9%      55.0%      |   58.4%      47.0% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     87.0%!     80.8%!     |   88.3%!     83.4%!
Liệt Diễm Đường (Đao lửa)                  83.1%!     58.4%      |   77.0%!     56.5% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         80.7%!     38.3%      |   79.0%!     39.2% 
Bích Thủy Am (Thanh Đằng Tiên + Cung C     46.0%      40.8%      |   60.0%      56.1% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     29.5%!     35.7%      |   32.2%!     38.4% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     55.6%      62.3%      |   63.5%      70.3%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     45.5%      49.7%      |   30.0%!     21.5%!
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      62.6%      49.3%      |   75.1%!     67.1%!
Thái Thanh Hóa Kình                        19.0%!     49.8%      |   15.0%!     45.7% 
Huyền Độc Phệ Khí                           4.5%!     20.2%!     |    2.6%!     30.4%!
Kim Cang Cương Khí                         16.9%!     17.1%!     |   19.0%!     26.4%!
Thái Thanh Quán (Kiếm + Hóa Kình)          40.8%      50.5%      |   28.0%!     23.5%!
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     65.0%      61.6%      |   67.8%!     52.3% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      66.1%!     55.8%      |   73.8%!     53.9% 
Thiết Phủ Bang (Rìu + Cầm)                 50.2%      30.2%!     |   42.4%      21.1%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      79.0%!     65.2%!     |   53.3%      43.5% 
Outside [35, 65]                           27/52                 |   33/52
```

#### B Tinh (Rare) — Trước vs Sau Patch:
```text
Loadout                                    Trước T1   Trước T9   |   Sau T1     Sau T9
-----------------------------------------------------------------+--------------------
Liệt Hỏa Thiêu Thân                        13.4%!     89.5%!     |   10.0%!     86.7%!
Bích Thủy Hồi Xuân                          8.0%!      0.0%!     |    6.4%!      0.4%!
Tử Mẫu Tốc Kích                             0.0%!     25.4%!     |    9.0%!     65.8%!
Huyền Độc Môn (Ám Khí + Trảo)              38.1%      38.5%      |   33.5%!     39.2% 
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     66.6%!     86.7%!     |   76.2%!     89.1%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     50.9%      57.5%      |   53.9%      65.7%!
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     84.8%!     77.6%!     |   79.7%!     77.3%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      63.8%      43.7%      |   60.7%      37.7% 
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      70.6%!     46.1%      |   73.6%!     45.2% 
Kim Cang Tự (Quyền + Côn)                  63.2%      42.4%      |   65.9%!     49.3% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     81.1%!     82.1%!     |   95.5%!     88.1%!
Liệt Diễm Đường (Đao lửa)                  86.7%!     64.1%      |   80.0%!     60.5% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         69.4%!     41.8%      |   74.1%!     36.6% 
Bích Thủy Am (Thanh Đằng Tiên + Cung C     45.0%      39.9%      |   52.6%      54.8% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     31.2%!     39.8%      |   36.9%      42.8% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     67.7%!     75.4%!     |   72.9%!     72.4%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     50.3%      50.9%      |   33.1%!     25.6%!
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      71.5%!     48.5%      |   78.8%!     64.6% 
Thái Thanh Hóa Kình                        19.9%!     48.9%      |   15.5%!     45.5% 
Huyền Độc Phệ Khí                           4.0%!     20.8%!     |    0.6%!     31.5%!
Kim Cang Cương Khí                         16.8%!     25.8%!     |   19.4%!     37.3% 
Thái Thanh Quán (Kiếm + Hóa Kình)          46.1%      51.0%      |   31.6%!     22.7%!
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     57.6%      50.7%      |   67.0%!     45.2% 
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      61.3%      49.0%      |   68.4%!     48.4% 
Thiết Phủ Bang (Rìu + Cầm)                 53.9%      33.6%!     |   43.6%      23.5%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      76.3%!     65.9%!     |   58.9%      42.0% 
Outside [35, 65]                           27/52                 |   32/52
```

#### B Thần Binh (Legendary) — Trước vs Sau Patch:
```text
Loadout                                    Trước T1   Trước T9   |   Sau T1     Sau T9
-----------------------------------------------------------------+--------------------
Liệt Hỏa Thiêu Thân                        13.0%!     91.9%!     |   11.8%!     93.7%!
Bích Thủy Hồi Xuân                          8.0%!      0.2%!     |    7.0%!      0.8%!
Tử Mẫu Tốc Kích                             0.0%!     31.1%!     |    3.2%!     66.4%!
Huyền Độc Môn (Ám Khí + Trảo)              43.8%      59.9%      |   38.6%      48.7% 
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     81.6%!     92.2%!     |   75.1%!     88.7%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     58.6%      55.0%      |   61.9%      57.2% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     86.8%!     71.8%!     |   78.9%!     73.8%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      58.0%      37.8%      |   52.4%      33.0%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      57.8%      32.2%!     |   57.7%      38.5% 
Kim Cang Tự (Quyền + Côn)                  65.4%!     44.1%      |   66.2%!     45.3% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     80.0%!     81.0%!     |   94.9%!     85.4%!
Liệt Diễm Đường (Đao lửa)                  84.4%!     56.3%      |   88.7%!     52.3% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         49.8%      33.1%!     |   56.3%      33.9%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     38.6%      42.8%      |   54.3%      53.9% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     41.7%      47.4%      |   48.1%      43.0% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     77.3%!     77.7%!     |   78.2%!     77.1%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     55.8%      49.5%      |   40.5%      27.2%!
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      77.8%!     49.9%      |   81.5%!     63.7% 
Thái Thanh Hóa Kình                        22.6%!     49.1%      |   15.8%!     44.5% 
Huyền Độc Phệ Khí                           4.8%!     37.9%      |    5.7%!     50.2% 
Kim Cang Cương Khí                         16.7%!     39.4%      |   20.0%!     44.4% 
Thái Thanh Quán (Kiếm + Hóa Kình)          51.8%      44.8%      |   36.8%      26.5%!
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     49.7%      29.7%!     |   61.5%      27.1%!
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      53.8%      28.5%!     |   55.1%      33.2%!
Thiết Phủ Bang (Rìu + Cầm)                 44.9%      36.8%      |   41.8%      27.9%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      75.6%!     78.3%!     |   67.2%!     62.3% 
Outside [35, 65]                           26/52                 |   28/52
```

#### B Chí Tôn (Supreme) — Trước vs Sau Patch:
```text
Loadout                                    Trước T1   Trước T9   |   Sau T1     Sau T9
-----------------------------------------------------------------+--------------------
Liệt Hỏa Thiêu Thân                        13.3%!     95.2%!     |   12.0%!     95.4%!
Bích Thủy Hồi Xuân                          8.0%!      0.8%!     |    4.7%!      1.4%!
Tử Mẫu Tốc Kích                             0.0%!     32.8%!     |    4.8%!     64.3% 
Huyền Độc Môn (Ám Khí + Trảo)              51.7%      82.9%!     |   38.1%      73.7%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     93.6%!     96.0%!     |   97.5%!     95.3%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     54.7%      55.2%      |   62.9%      61.0% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     87.7%!     66.2%!     |   78.6%!     71.6%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      52.2%      26.4%!     |   52.0%      29.7%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      47.9%      24.3%!     |   64.0%      28.4%!
Kim Cang Tự (Quyền + Côn)                  60.8%      42.3%      |   61.2%      41.6% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     84.1%!     79.2%!     |   94.8%!     81.8%!
Liệt Diễm Đường (Đao lửa)                  79.1%!     50.1%      |   82.6%!     44.5% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         38.2%      21.8%!     |   57.7%      33.6%!
Bích Thủy Am (Thanh Đằng Tiên + Cung C     40.3%      45.2%      |   48.0%      47.5% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     42.1%      56.1%      |   42.4%      47.1% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     74.0%!     77.6%!     |   76.3%!     77.1%!
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     56.5%      45.0%      |   41.9%      27.4%!
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      69.8%!     44.2%      |   77.2%!     57.2% 
Thái Thanh Hóa Kình                        22.2%!     49.5%      |   15.8%!     44.2% 
Huyền Độc Phệ Khí                          10.5%!     64.2%      |   11.2%!     64.3% 
Kim Cang Cương Khí                         14.4%!     44.6%      |   16.8%!     49.2% 
Thái Thanh Quán (Kiếm + Hóa Kình)          53.7%      44.9%      |   36.7%      26.7%!
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     43.4%      25.5%!     |   58.6%      23.1%!
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      47.9%      24.2%!     |   53.6%      27.5%!
Thiết Phủ Bang (Rìu + Cầm)                 76.0%!     35.0%      |   37.3%      26.9%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      79.0%!     69.6%!     |   57.8%      56.7% 
Outside [35, 65]                           28/52                 |   27/52
```

---

### 1.3. Kiểm Tra Bước Nhảy Phẩm Chất (Rarity Step Check)
Chạy trên 4 archetype đại diện (`Trọng 3, TP 1, 100 trận`):

| Archetype | Lương vs Phàm | Tinh vs Lương | Tuyệt vs Tinh | Thần Binh vs Tuyệt | Chí Tôn vs Thần Binh | Phàm T9 vs Lương T1 |
|---|:---:|:---:|:---:|:---:|:---:|:---:|
| **Liệt Diễm (Đao lửa)** | 100% | 98.0% | 89.0% | 54.0% (hòa 43%) | 93.0% | 100% (Phàm T9 thắng) |
| **Thái Thanh (Kiếm/Hóa Kình)** | 89.0% | 88.0% | 82.0% | 80.0% | 81.0% | 100% (Phàm T9 thắng) |
| **Huyền Độc (Ám khí/Trảo)** | 100% | 100% | 100% | 100% | 100% | 100% (Phàm T9 thắng) |
| **Kim Cang (Quyền/Côn)** | 99.0% | 100% | 99.0% | 99.0% | 84.0% | 100% (Phàm T9 thắng) |

**Nhận định & Đề xuất hệ số nhân phẩm:**  
- **Vấn đề:** Trong các trận đấu đối xứng (mirror match), chiến đấu gần như tất định (deterministic), độ lệch chỉ số +25% giữa 2 phẩm liên tiếp bị tích lũy qua từng đòn đánh khiến bên phẩm cao thắng gần như 90–100% (ngoại trừ Thái Thanh có Hóa Kình tạo biến số rơi về 80–89%). Đồng thời, Phàm Trọng 9 có hệ số Trọng 2.0x kèm 3 mốc Đột Phá (T3, T6, T9) nên hoàn toàn áp đảo Lương Trọng 1 (1.25x, 0 Đột Phá).
- **Đề xuất hệ số phẩm mới (bước nhảy mượt 15%):**  
  Phàm **1.00** · Lương **1.15** · Tinh **1.32** · Tuyệt **1.52** · Thần Binh **1.75** · Chí Tôn **2.00**.  
  *Lợi ích:* Đưa tỉ lệ thắng 1 bước nhảy về vùng lý tưởng 65–75%; kiềm chế lượng khiên/hồi máu tối đa tại Chí Tôn T9 từ 5.5x xuống 4.0x.

---

## 2. Chi Tiết Các Thay Đổi Trong Patch (`balance-rarity.patch.json`)

### 2.1. Nhóm Binh Khí Cũ & Môn Phái
1. `thiet_cot_quyen_sao`: Giảm Lực Đạo cơ sở từ 16/11 về `10`, giữ `gain_qi: 1`. Hạn chế việc xếp 6 Quyền Sáo tích nạp quá nhiều Chân Khí và sát thương đầu trận.
2. `thanh_phong_kiem`: Lực Đạo 49 → `36`, tiêu hao Tuyệt Kỹ 40 → `45`. Giảm bớt nhịp xả Tuyệt Kỹ dồn dập của Tam Kiếm Thái Thanh.
3. `liet_diem_dao`: Lực Đạo 78 → `68`, hồi chiêu 4.5s → `4.8s`. Kéo giãn nhịp kích nổ Liệt Hỏa đầu trận.
4. `khai_son_phu`: Lực Đạo 181 → `145`, hồi chiêu 6.75s → `7.0s`. Kiềm chế Tam Phủ Khai Thiên đập vỡ Cương Khí quá sớm.
5. `phuc_ho_con`: Lực Đạo 106 → `95`, tiêu hao Tuyệt Kỹ 25 → `30`.
6. `chan_nhac_chuy`: Lực Đạo 126 → `95`.
7. `bang_phach_thuong`: Lực Đạo 72 → `62` (do có sẵn hiệu ứng xuyên giáp).
8. `truy_hon_cung`: Lực Đạo 36 → `30`.
9. `xich_sa_chuong`: Lực Đạo 27 → `22`.
10. `that_tinh_tieu`: Tầng Kịch Độc 5 → `4` (tránh việc nhân phẩm Chí Tôn nổ độc liên tục).
11. `Tâm Pháp Môn Phái`: Giảm `levels.1.attackPct` của Kim Cang, Thiết Phủ, Liệt Diễm, Thái Thanh từ 0.10 về `0.06 – 0.08` để tránh khuếch đại sát thương binh khí quá lớn ở cấp Trọng thấp.

### 2.2. Nhóm 40 Vật Phẩm Mới (Đặc biệt xử lý Khiên & Hồi Phục ở Chí Tôn)
Tại mốc Chí Tôn T9, chỉ số bị nhân tới 5.5x ($2.0 \times 2.75$). Khiên và Hồi Phục cơ sở ở Phàm cần chuẩn hóa để không vượt quá trần Sinh Lực người chơi (1,200 HP):
1. `bang_tam_giap.triggers.shield`: 400 → `180` (tối đa Chí Tôn T9: 990 Cương Khí thay vì 2,200).
2. `o_tam_y.triggers.shield`: 480 → `260`.
3. `o_thiet_giap.triggers.shield`: 500 → `300`.
4. `kim_tuyen_y.triggers.shield`: 800 → `380`.
5. `phuc_ma_giap.triggers.shield`: 700 → `380`.
6. `thiet_giap_y.triggers.shield`: 250 → `190`.
7. `da_ho_y.triggers.shield`: 200 → `170`.
8. `nho_sam_y.triggers.shield`: 300 → `190`.
9. `tue_nguyet_bao_y.triggers.shield`: 180 → `150`.
10. `thanh_dong_ho_tam_kinh.triggers.shield`: 160 → `90`.
11. `boi_nguyen_dan.triggers.heal`: 240 → `150`.
12. `hoi_thien_dan.triggers.heal`: 220 → `170`.
13. `ho_mach_dan.triggers.shield`: 350 → `200`.
14. `giai_doc_dan`: hồi máu 200 → `130`, khiên 150 → `110`.
15. `bich_thuy_cam`: Lực Đạo 59 → `60`, hồi chiêu 5.0s → `4.6s`.
16. `tu_mau_song_dao`: Lực Đạo 22 → `28`, Tỉ lệ Hội Tâm 0.10 → `0.18`. Giúp đội hình `Tử Mẫu Tốc Kích` đạt tỉ lệ thắng 59–66% ở T9 (đạt chuẩn viable ≥ 35%).

---

## 3. Lệnh Tái Hiện Kết Quả Báo Cáo

```bash
# 1. Chạy ma trận A (Native rarities, T1,3,6,9 x TP1,9):
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity.patch.json --loadouts tools/gemini/out/balance-rarity/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100

# 2. Chạy 4 ma trận B (Fair PvP, cùng phẩm chất):
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity.patch.json --loadouts tools/gemini/out/balance-rarity/loadouts --rarity common --tiers 1,9 --runs 100
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity.patch.json --loadouts tools/gemini/out/balance-rarity/loadouts --rarity rare --tiers 1,9 --runs 100
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity.patch.json --loadouts tools/gemini/out/balance-rarity/loadouts --rarity legendary --tiers 1,9 --runs 100
py tools/balance/balance.py whatif --patch tools/gemini/out/balance-rarity.patch.json --loadouts tools/gemini/out/balance-rarity/loadouts --rarity supreme --tiers 1,9 --runs 100
```

---

## 4. 4 Câu Hỏi Mở Dành Cho Ban Thiết Kế & Lập Trình

1. **Điều chỉnh hệ số phẩm chất (Rarity Multipliers):** Có chấp thuận hạ bậc số nhân phẩm từ `[1.0, 1.25, 1.55, 1.90, 2.30, 2.75]` xuống `[1.0, 1.15, 1.32, 1.52, 1.75, 2.00]` trong engine để giải quyết triệt để bài toán bước nhảy phẩm chất 65–75% không?
2. **Kích thước & vai trò của Bích Thủy Hồi Xuân:** Đội hình `burst_heal` hiện chỉ chiếm 11/20 ô với 1 đàn duy nhất nên thiếu sát thương dứt điểm trận đấu; có nên bổ sung 1 vũ khí phụ (như Tiên hoặc Tiêu) vào mẫu thử này không?
3. **Môn phái cho Song Đao & Cung:** Hiện `tu_mau_song_dao` (dual) và `truy_hon_cung` (bow) không thuộc danh mục vũ khí của môn phái nào trong `sects.json`; có nên mở rộng hoặc ghép vào Thái Thanh / Hàn Sơn không?
4. **Trần Cương Khí tối đa (Max Shield Cap):** Tương tự như trần Điều Tức 30/s (Quyết định #14), game có nên áp dụng trần Cương Khí tối đa không vượt quá 100% Khí Huyết tối đa của Hiệp Khách để ngăn hiện tượng "khiên vô tận" ở Chí Tôn không?
