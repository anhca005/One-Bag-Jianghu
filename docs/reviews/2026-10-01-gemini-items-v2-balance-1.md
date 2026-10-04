# Báo Cáo Cân Bằng Đợt 1 Items v2 — Nhất Nang Giang Hồ
**Căn cứ:** Quyết định #13, #25–#32; `docs/design/balance.md`. Patch: `tools/gemini/out/items-v2-balance-1/patch.json`.
## 1. 20 Đội Hình Mẫu v2 (Loadouts)
1. `bich_thuy_duong_sinh.json`: Bích Thủy Am - Dưỡng Sinh Cầm Âm (bich_thuy). Dưỡng Sinh Quyết + Tuế Nguyệt Bảo Y.
2. `han_son_bang_tam.json`: Hàn Sơn Phái - Băng Tâm Bút Cung (han_son). Băng Tâm Giáp + Hàn Băng Châu + Bút + Cung.
3. `huyen_doc.json`: Huyền Độc Môn - Hắc Huyết Ám Khí (huyen_doc). Thất Tinh Tiêu + Tử Kim Trảo + U Minh Bí Lục + Vạn Độc Đỉnh.
4. `huyen_doc_van_doc.json`: Huyền Độc Môn - Vạn Độc Phệ Huyết (huyen_doc). Thất Tinh Tiêu + Tử Kim Trảo + U Minh Bí Lục.
5. `hybrid_han_son_bich_thuy.json`: Hàn Sơn Phái - Hàn Bích Hợp Bích (han_son). Hàn Ngọc Tiêu + Bích Thủy Cầm (Hybrid).
6. `hybrid_kim_cang_thiet_phu.json`: Kim Cang Tự - Kim Cang Thiết Phủ (kim_cang). Côn + Rìu + Bách Luyện Chân Kinh (Hybrid).
7. `hybrid_thai_thanh_liet_diem.json`: Thái Thanh Quán - Thái Thanh Liệt Diễm (thai_thanh). Kiếm + Đao + Thái Thanh Kinh (Hybrid).
8. `hybrid_thiet_phu_liet_diem.json`: Thiết Phủ Bang - Thiết Phủ Liệt Diễm (thiet_phu). Khai Sơn Phủ x2 + Liệt Diễm Đao (Hybrid).
9. `kim_cang.json`: Kim Cang Tự - Hộ Thể Côn Quyền (kim_cang). Côn + Quyền + Thiết Giáp Y + Tử Kim Bát phản kích.
10. `kim_cang_bach_luyen.json`: Kim Cang Tự - Bách Luyện Thuần Quyền (kim_cang). Quyền sáo x4 + Bách Luyện Chân Kinh.
11. `liet_diem.json`: Liệt Diễm Đường - Cuồng Đao Liệt Hỏa (liet_diem). Liệt Diễm Đao + Chu Sa Hồ Lô + Liệt Hỏa Tâm Kinh.
12. `liet_diem_hoa_hai.json`: Liệt Diễm Đường - Hỏa Hải Song Đao (liet_diem). Tử Mẫu Song Đao + Liệt Diễm Đao + Chu Sa Hồ Lô.
13. `new_bich_thuy_whip.json`: Bích Thủy Am - Thanh Đằng Trường Tiên (bich_thuy). Thanh Đằng Tiên x2 + Trầm Hương Lô.
14. `new_han_son_spear.json`: Hàn Sơn Phái - Hàn Băng Thương Pháp (han_son). Băng Phách Thương + Hàn Băng Châu + Băng Tâm Giáp.
15. `new_liet_diem_palm.json`: Liệt Diễm Đường - Xích Sa Chưởng Kình (liet_diem). Xích Sa Chưởng + Chu Sa Hồ Lô + Dạ Hổ Y.
16. `new_thai_thanh_fan.json`: Thái Thanh Quán - Tiêu Dao Phiến Pháp (thai_thanh). Ngọc Cốt Phiến + Bạch Ngọc Bội + Vô Niệm Kinh.
17. `new_thiet_phu_hammer.json`: Thiết Phủ Bang - Chấn Nhạc Chùy Phong (thiet_phu). Chấn Nhạc Chùy + Khai Sơn Phủ + Ô Thiết Giáp.
18. `thai_thanh.json`: Thái Thanh Quán - Kiếm Đạo Hóa Kình (thai_thanh). Thanh Phong Kiếm + Bạch Ngọc Bội + Vô Niệm Kinh.
19. `thai_thanh_kiem_khi.json`: Thái Thanh Quán - Kiếm Khí Xuyên Giáp (thai_thanh). Thanh Phong Kiếm x3 + Kiếm Tông Tâm Yếu.
20. `thiet_phu.json`: Thiết Phủ Bang - Bá Khí Khai Sơn (thiet_phu). Khai Sơn Phủ x3 + Ô Thiết Giáp + Dạ Hổ Y.
## 2. Bảng Kết Quả Đối Đầu (Verbatim)
### 2.1. Ma trận A: Baseline vs Final (Tiers 1, 3, 6, 9 x TP 1, 9; Runs 100)
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           93.1%!     98.7%!     83.5%!     79.0%!     80.8%!     76.6%!     75.7%!     69.5%!
Hàn Sơn Phái - Băng Tâm Bút Cung           15.4%!     36.8%       6.2%!     26.2%!      4.5%!     18.8%!      9.1%!     20.4%!
Huyền Độc Môn - Hắc Huyết Ám Khí            0.0%!      3.2%!     17.3%!     21.4%!     45.7%      57.9%      94.7%!     94.7%!
Huyền Độc Môn - Vạn Độc Phệ Huyết           5.3%!      2.4%!    100.0%!    100.0%!    100.0%!    100.0%!    100.0%!     99.7%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           77.8%!     77.1%!     58.3%      59.1%      52.1%      53.8%      64.2%      57.0% 
Kim Cang Tự - Kim Cang Thiết Phủ           76.2%!     74.1%!     68.3%!     63.6%      61.5%      52.3%      56.8%      47.4% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     37.5%      23.2%!     41.4%      27.1%!     40.7%      28.2%!     41.4%      30.4%!
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       27.5%!     17.9%!     16.9%!      4.9%!     15.6%!      5.5%!     10.4%!      5.4%!
Kim Cang Tự - Hộ Thể Côn Quyền             99.8%!     90.3%!     94.7%!     74.9%!     93.4%!     71.9%!     84.1%!     70.2%!
Kim Cang Tự - Bách Luyện Thuần Quyền       79.8%!     76.9%!     63.1%      50.8%      76.8%!     57.6%      73.3%!     67.1%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       49.8%      43.3%      41.3%      40.7%      31.8%!     22.4%!     28.2%!     24.7%!
Liệt Diễm Đường - Hỏa Hải Song Đao         32.4%!     35.8%      50.3%      45.9%      47.5%      45.4%      43.9%      47.1% 
Bích Thủy Am - Thanh Đằng Trường Tiên      89.4%!     95.4%!     82.1%!     79.2%!     77.3%!     76.3%!     71.9%!     72.2%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        23.6%!     41.6%      14.7%!     29.7%!     10.8%!     25.3%!     16.8%!     25.9%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      35.1%      36.6%      37.7%      37.2%      30.8%!     24.0%!     28.8%!     24.0%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      59.6%      59.4%      53.6%      79.4%!     60.8%      86.0%!     56.7%      76.1%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      39.1%      27.4%!     25.2%!     16.1%!     25.3%!     23.8%!     27.7%!     22.9%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        64.7%      66.9%!     71.4%!     84.1%!     70.6%!     87.7%!     60.9%      74.1%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      59.8%      63.6%      48.1%      60.5%      44.2%      60.9%      27.7%!     40.4% 
Thiết Phủ Bang - Bá Khí Khai Sơn           34.8%!     29.1%!     26.6%!     20.6%!     31.1%!     25.1%!     30.2%!     29.9%!
Outside [35, 65]: 108/160
```
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           59.7%      61.7%      59.3%      78.4%!     53.5%      63.8%      60.3%      61.0% 
Hàn Sơn Phái - Băng Tâm Bút Cung           48.4%      76.3%!     34.4%!     68.9%!     30.6%!     55.6%      34.1%!     49.7% 
Huyền Độc Môn - Hắc Huyết Ám Khí           30.2%!     39.7%      77.9%!     71.6%!     90.6%!     89.7%!     98.0%!     98.4%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          30.1%!     39.6%      56.9%      56.3%      82.8%!     84.5%!     96.6%!     96.9%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           64.0%      72.5%!     36.5%      37.4%      46.5%      46.5%      50.8%      45.1% 
Kim Cang Tự - Kim Cang Thiết Phủ           17.4%!     12.2%!     59.0%      37.7%      49.6%      32.1%!     53.1%      39.9% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     51.9%      16.2%!     50.1%      18.8%!     38.8%      10.3%!     35.2%      17.0%!
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       56.1%      26.3%!     36.6%      20.1%!     49.4%      37.9%      27.5%!     27.1%!
Kim Cang Tự - Hộ Thể Côn Quyền             35.4%      16.1%!     38.7%      18.7%!     51.5%      32.4%!     63.8%      54.0% 
Kim Cang Tự - Bách Luyện Thuần Quyền       24.9%!     22.1%!     29.9%!     26.9%!     30.3%!     28.2%!     75.0%!     67.9%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       73.5%!     60.1%      57.9%      46.8%      39.2%      31.7%!     40.0%      33.1%!
Liệt Diễm Đường - Hỏa Hải Song Đao         71.1%!     70.7%!     63.2%      59.2%      60.1%      57.8%      53.3%      53.2% 
Bích Thủy Am - Thanh Đằng Trường Tiên      55.8%      64.6%      63.0%      74.8%!     75.9%!     77.9%!     65.9%!     66.2%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        56.0%      83.5%!     37.5%      68.5%!     25.0%!     48.8%      19.9%!     34.8%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      55.6%      46.2%      21.5%!     22.6%!     19.2%!     22.8%!     20.8%!     20.4%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      45.4%      66.3%!     73.7%!     78.5%!     74.7%!     82.6%!     56.1%      75.8%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      88.6%!     61.4%      66.6%!     43.6%      49.5%      35.6%      34.2%!     28.5%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        37.6%      62.6%      63.3%      80.8%!     72.9%!     86.3%!     71.3%!     78.2%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      25.2%!     53.6%       8.1%!     44.5%      14.9%!     53.5%      10.5%!     38.5% 
Thiết Phủ Bang - Bá Khí Khai Sơn           73.6%!     51.2%      63.9%      46.2%      47.3%      22.1%!     30.8%!     14.5%!
Outside [35, 65]: 85/160
```
### 2.2. Ma trận B Phẩm Phàm: Baseline vs Final (Common; Tiers 1, 9 x TP 1; Runs 100)
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           92.7%!     76.7%!
Hàn Sơn Phái - Băng Tâm Bút Cung           13.8%!      7.4%!
Huyền Độc Môn - Hắc Huyết Ám Khí            0.0%!     94.7%!
Huyền Độc Môn - Vạn Độc Phệ Huyết           5.3%!    100.0%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           75.4%!     63.4% 
Kim Cang Tự - Kim Cang Thiết Phủ           76.2%!     58.7% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     36.8%      40.4% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       29.4%!     13.0%!
Kim Cang Tự - Hộ Thể Côn Quyền             99.9%!     85.6%!
Kim Cang Tự - Bách Luyện Thuần Quyền       82.9%!     72.9%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       50.6%      29.1%!
Liệt Diễm Đường - Hỏa Hải Song Đao         25.1%!     46.5% 
Bích Thủy Am - Thanh Đằng Trường Tiên      89.6%!     72.8%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        29.1%!     15.7%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      35.0%      27.8%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      60.2%      60.2% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      37.6%      16.9%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        64.6%      62.9% 
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      64.9%      32.4%!
Thiết Phủ Bang - Bá Khí Khai Sơn           30.9%!     24.4%!
Outside [35, 65]: 27/40
```
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           63.9%      62.1% 
Hàn Sơn Phái - Băng Tâm Bút Cung           40.3%      25.3%!
Huyền Độc Môn - Hắc Huyết Ám Khí           42.2%      98.4%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          42.1%      96.4%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           59.0%      49.2% 
Kim Cang Tự - Kim Cang Thiết Phủ           10.7%!     53.9% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     56.5%      39.9% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       57.2%      24.1%!
Kim Cang Tự - Hộ Thể Côn Quyền             39.7%      69.2%!
Kim Cang Tự - Bách Luyện Thuần Quyền       27.1%!     78.0%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       72.0%!     35.6% 
Liệt Diễm Đường - Hỏa Hải Song Đao         51.3%      59.6% 
Bích Thủy Am - Thanh Đằng Trường Tiên      61.7%      65.8%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        56.7%      18.5%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      58.8%      20.8%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      42.9%      58.4% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      87.5%!     31.3%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        31.1%!     71.2%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      23.1%!     12.2%!
Thiết Phủ Bang - Bá Khí Khai Sơn           75.1%!     27.1%!
Outside [35, 65]: 20/40
```
### 2.3. Ma trận B Thần Binh: Baseline vs Final (Legendary; Tiers 1, 9 x TP 1; Runs 100)
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           93.2%!     71.5%!
Hàn Sơn Phái - Băng Tâm Bút Cung           14.3%!      9.3%!
Huyền Độc Môn - Hắc Huyết Ám Khí            2.5%!     94.7%!
Huyền Độc Môn - Vạn Độc Phệ Huyết           2.8%!    100.0%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           74.9%!     59.9% 
Kim Cang Tự - Kim Cang Thiết Phủ           76.2%!     55.6% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     37.4%      41.6% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       28.0%!     11.2%!
Kim Cang Tự - Hộ Thể Côn Quyền             98.6%!     86.3%!
Kim Cang Tự - Bách Luyện Thuần Quyền       81.6%!     73.1%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       51.4%      27.1%!
Liệt Diễm Đường - Hỏa Hải Song Đao         35.2%      54.1% 
Bích Thủy Am - Thanh Đằng Trường Tiên      91.3%!     71.3%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        25.4%!     20.3%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      31.7%!     22.6%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      60.4%      54.2% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      38.3%      31.5%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        65.4%!     59.8% 
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      56.9%      27.1%!
Thiết Phủ Bang - Bá Khí Khai Sơn           35.5%      28.7%!
Outside [35, 65]: 27/40
```
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           54.9%      57.7% 
Hàn Sơn Phái - Băng Tâm Bút Cung           44.4%      30.8%!
Huyền Độc Môn - Hắc Huyết Ám Khí           29.8%!     97.4%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          29.2%!     97.4%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           53.9%      45.7% 
Kim Cang Tự - Kim Cang Thiết Phủ           12.3%!     49.8% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     50.2%      37.2% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       66.5%!     34.1%!
Kim Cang Tự - Hộ Thể Côn Quyền             40.6%      67.1%!
Kim Cang Tự - Bách Luyện Thuần Quyền       20.5%!     76.7%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       74.2%!     37.1% 
Liệt Diễm Đường - Hỏa Hải Song Đao         78.9%!     58.7% 
Bích Thủy Am - Thanh Đằng Trường Tiên      54.0%      63.7% 
Hàn Sơn Phái - Hàn Băng Thương Pháp        56.1%      21.2%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      48.7%      24.4%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      50.9%      56.4% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      85.6%!     33.6%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        42.6%      70.4%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      28.1%!     10.1%!
Thiết Phủ Bang - Bá Khí Khai Sơn           77.9%!     29.6%!
Outside [35, 65]: 22/40
```
### 2.4. Bậc phẩm chất: Baseline vs Final (Rarity Step; Tier 3, TP 1; Runs 100)
```text
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm                      60.5%                65.5%                54.0%                55.0%                55.5%                 0.0%
Hàn Sơn Phái - Băng Tâm Bút Cung                      70.0%                73.5%                70.5%                68.0%                71.5%                 0.0%
Huyền Độc Môn - Hắc Huyết Ám Khí                      55.5%                94.0%                91.0%                71.5%               100.0%                 0.0%
Huyền Độc Môn - Vạn Độc Phệ Huyết                     55.5%                70.5%                70.5%                99.5%                81.0%                 0.0%
Hàn Sơn Phái - Hàn Bích Hợp Bích                      62.0%                62.0%                69.5%                69.5%                69.5%                 0.0%
Kim Cang Tự - Kim Cang Thiết Phủ                      85.0%                87.5%                88.5%                85.0%                89.0%                 0.0%
Thái Thanh Quán - Thái Thanh Liệt Diễm                59.5%                60.5%                60.0%                63.5%                53.5%                 0.0%
Thiết Phủ Bang - Thiết Phủ Liệt Diễm                  60.5%                65.5%                67.0%                66.5%                67.0%                 0.0%
Kim Cang Tự - Hộ Thể Côn Quyền                        50.0%                50.0%                32.0%                78.0%                58.0%                 0.0%
Kim Cang Tự - Bách Luyện Thuần Quyền                  50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa                  91.0%                81.0%                85.0%               100.0%                17.0%                 0.0%
Liệt Diễm Đường - Hỏa Hải Song Đao                    78.5%                99.5%                67.5%               100.0%                97.5%                 0.0%
Bích Thủy Am - Thanh Đằng Trường Tiên                 55.0%                62.5%                64.5%                62.0%                67.5%                 0.0%
Hàn Sơn Phái - Hàn Băng Thương Pháp                   73.0%                75.0%                73.5%                73.0%                72.5%                 0.0%
Liệt Diễm Đường - Xích Sa Chưởng Kình                 41.5%                70.5%                98.5%                99.5%                96.0%                 0.0%
Thái Thanh Quán - Tiêu Dao Phiến Pháp                 55.0%                54.0%                57.0%                60.0%                62.0%                 0.0%
Thiết Phủ Bang - Chấn Nhạc Chùy Phong                 48.5%                50.5%                52.5%                53.5%                51.0%                 0.0%
Thái Thanh Quán - Kiếm Đạo Hóa Kình                   61.0%                61.0%                63.0%                64.0%                60.0%                 0.0%
Thái Thanh Quán - Kiếm Khí Xuyên Giáp                 70.0%                73.0%                70.0%                66.0%                64.5%                 0.0%
Thiết Phủ Bang - Bá Khí Khai Sơn                      74.5%                55.0%                55.0%                89.5%                86.0%                 0.0%
Average                                               62.8%                68.0%                67.0%                73.7%                68.5%                 0.0%
(draws count as half a win; the last column reports the Lương T1 side)
```
```text
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm                      61.0%                38.5%                46.0%                42.0%                39.5%                 0.0%
Hàn Sơn Phái - Băng Tâm Bút Cung                      73.0%                73.0%                74.5%                69.0%                73.5%                 0.0%
Huyền Độc Môn - Hắc Huyết Ám Khí                      95.0%               100.0%                98.5%               100.0%               100.0%                 0.0%
Huyền Độc Môn - Vạn Độc Phệ Huyết                     74.0%               100.0%               100.0%               100.0%               100.0%                 0.0%
Hàn Sơn Phái - Hàn Bích Hợp Bích                      50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Kim Cang Tự - Kim Cang Thiết Phủ                      56.0%                59.0%                64.0%                75.0%                72.0%                 0.0%
Thái Thanh Quán - Thái Thanh Liệt Diễm                59.5%                68.0%                70.5%                75.0%                67.0%                 0.0%
Thiết Phủ Bang - Thiết Phủ Liệt Diễm                  56.0%                54.5%                54.0%               100.0%                55.0%                 0.0%
Kim Cang Tự - Hộ Thể Côn Quyền                        70.0%                66.5%                36.5%                98.5%                74.5%                 0.0%
Kim Cang Tự - Bách Luyện Thuần Quyền                  50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa                  97.0%                91.5%                65.5%                86.0%                82.0%                 0.0%
Liệt Diễm Đường - Hỏa Hải Song Đao                    61.0%                61.5%                81.5%                69.0%                74.0%                 0.0%
Bích Thủy Am - Thanh Đằng Trường Tiên                 61.5%                59.0%                65.5%                64.5%                62.5%                 0.0%
Hàn Sơn Phái - Hàn Băng Thương Pháp                   77.0%                82.5%                84.5%                79.0%                75.5%                 0.0%
Liệt Diễm Đường - Xích Sa Chưởng Kình                 84.0%                81.5%                97.0%                98.0%               100.0%                 0.0%
Thái Thanh Quán - Tiêu Dao Phiến Pháp                 54.5%                53.0%                59.0%                58.5%                58.5%                 0.0%
Thiết Phủ Bang - Chấn Nhạc Chùy Phong                 64.0%                79.5%                66.5%                53.0%                53.0%                 0.0%
Thái Thanh Quán - Kiếm Đạo Hóa Kình                   57.0%                58.5%                61.5%                62.5%                66.0%                 0.0%
Thái Thanh Quán - Kiếm Khí Xuyên Giáp                 71.5%                70.5%                73.0%                75.5%                76.5%                 0.0%
Thiết Phủ Bang - Bá Khí Khai Sơn                      52.0%                52.0%                53.0%                53.0%                53.0%                 0.0%
Average                                               66.2%                67.5%                67.5%                72.9%                69.1%                 0.0%
(draws count as half a win; the last column reports the Lương T1 side)
```
## 3. Chẩn Đoán Lệch Điểm & Cơ Chế Lạm Dụng
- **Lặp vô hạn (Infinite Loop):** `van_doc_dinh` kích hoạt `status_applied` gây độc -> tự kích hoạt lặp đệ quy tới kịch trần `depth=8`, kéo theo `u_minh_bi_luc` nạp 0.5s liên tục làm vũ khí xuất chiêu vô tận. Capping: Tách liền kề, hạ nạp `u_minh_bi_luc`.
- **Lỗi schema Hộ Thân (Immortal DR):** `thiet_giap_y` và `duong_sinh_quyet` để `value: 0.02` thay vì `pct`, engine gán 100% giảm thương, chạm trần `maxDmgReducePermille=800` (bất tử). Khắc phục: siết trần Hộ Thân toàn cục `config:maxDmgReducePermille: 400` (40%).
- **Cỗ máy hồi phục Bích Thủy:** `bich_thuy_cam` nhận nạp 1.0s mỗi nhịp Điều Tức. Khắc phục: hạ nạp `triggers.charge` từ 1.0s -> 0.2s, hồi chiêu 4.6s, gọt khiên `duong_sinh_quyet` (8) và `tue_nguyet_bao_y` (10).
- **Rìu chùy Thiết Phủ:** Rìu chùy hồi chiêu dài lép vế T6/T9. Khắc phục: tăng công Khai Sơn Phủ (88), Chấn Nhạc Chùy (82), hồi chiêu 4.8s-5.0s; hạ khiên mở đầu `o_thiet_giap` (160) tránh áp đảo T1.
- **Ám khí Huyền Độc:** `that_tinh_tieu` (công 4) và `tu_kim_trao` (công 7) không phá nổi giáp T1. Khắc phục: nâng công lên 7 và 11, độc cơ bản lên 3.
## 4. Lệnh Tái Hiện & Câu Hỏi Mở
- `whatif --patch tools/gemini/out/items-v2-balance-1/patch.json --items tools/gemini/out/items-v2/items-v2.normalized.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100`
- `whatif ... --tiers 1,9 --tp 1 --runs 100 --rarity common` / `--rarity legendary`
- `rarity-step --items tools/gemini/out/items-v2-balance-1/items-v2-balanced.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts --tier 3 --tp 1 --runs 100`
1. Có nên bổ sung `internal_cooldown` (~0.5s) cho trigger `status_applied` để chặn triệt để lặp đệ quy độc/cháy?
2. `Thanh Đồng Hộ Tâm Kính` (Hộ Cụ 1x1) và `Xích Sa Chưởng` (vải quấn tay) có nên quy chuẩn lại thành Kỳ Trân và Bí Kíp ở v3?
3. Kịch Độc ở T9 áp đảo các lối chơi thuần Cương Khí; có nên tăng cường dược phẩm giải độc (`giai_doc_dan`) thành trang bị thường trực cho các phái đối đầu?
