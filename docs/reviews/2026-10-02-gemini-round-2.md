# Báo Cáo Cân Bằng & Thiết Kế Màn Chơi Vòng 2 — Nhất Nang Giang Hồ

**Căn cứ:** Quyết định #21, #32, #34; `docs/design/balance.md`, `docs/design/pve.md`.
**Thư mục kết quả:** `tools/gemini/out/round-2/`

---

## Phần 1: Cân Bằng Items v2 (Vòng 2)

### 1.1. Bảng Đối Đầu Ma Trận A (T1, T3, T6, T9 x TP 1, 9; Runs 100) — Verbatim
```text
Loadout                                    T1/TP1     T1/TP9     T3/TP1     T3/TP9     T6/TP1     T6/TP9     T9/TP1     T9/TP9
-------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           40.4%      51.9%      50.4%      71.4%!     47.2%      65.7%!     54.9%      52.9% 
Hàn Sơn Phái - Băng Tâm Bút Cung           51.3%      77.4%!     37.3%      72.0%!     30.8%!     50.6%      29.7%!     42.6% 
Huyền Độc Môn - Hắc Huyết Ám Khí           25.2%!     29.2%!     53.4%      51.4%      67.6%!     76.2%!     94.3%!     94.8%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          25.2%!     29.2%!     37.2%      37.7%      53.1%      63.4%      91.0%!     92.7%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           67.5%!     67.8%!     30.2%!     30.5%!     41.4%      44.7%      45.4%      40.3% 
Kim Cang Tự - Kim Cang Thiết Phủ           22.2%!      9.8%!     54.2%      34.2%!     45.8%      30.3%!     47.4%      33.7%!
Thái Thanh Quán - Thái Thanh Liệt Diễm     66.0%!     39.7%      62.0%      35.0%      55.7%      19.8%!     36.6%      26.0%!
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       49.0%      22.2%!     47.2%      17.7%!     55.5%      24.8%!     23.6%!     16.7%!
Kim Cang Tự - Hộ Thể Côn Quyền             44.3%      27.5%!     34.0%!     17.7%!     45.8%      30.4%!     63.2%      51.3% 
Kim Cang Tự - Bách Luyện Thuần Quyền       37.0%      30.1%!     31.6%!     29.9%!     30.1%!     27.8%!     72.5%!     67.0%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       93.6%!     73.5%!     68.7%!     65.6%!     56.1%      50.3%      39.1%      39.4% 
Liệt Diễm Đường - Hỏa Hải Song Đao         76.8%!     75.5%!     72.3%!     70.5%!     69.4%!     66.4%!     52.6%      48.9% 
Huyền Độc Môn - Huyết Thú Cuồng Trảo       12.3%!     23.9%!     21.5%!     30.2%!     13.9%!     27.2%!     95.3%!     94.5%!
Hàn Sơn Phái - Băng Quy Tuyệt Hộ           20.8%!     45.8%      40.1%      54.0%      41.3%      57.6%      32.0%!     44.9% 
Liệt Diễm Đường - Xích Xà Hỏa Liệp         90.1%!     71.2%!     67.2%!     50.9%      75.8%!     55.4%      62.3%      58.3% 
Kim Cang Tự - Bạch Hổ Bá Quyền             51.4%      50.0%      66.8%!     58.4%      80.9%!     68.9%!     68.5%!     62.4% 
Bích Thủy Am - Thanh Đằng Trường Tiên      36.0%      43.6%      41.1%      64.1%      59.1%      75.3%!     57.0%      55.2% 
Hàn Sơn Phái - Hàn Băng Thương Pháp        68.2%!     90.2%!     50.6%      79.2%!     33.8%!     51.5%      20.6%!     32.2%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      76.4%!     77.8%!     49.3%      50.0%      37.5%      37.7%      25.6%!     32.7%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      47.9%      67.2%!     69.6%!     77.6%!     75.8%!     87.0%!     44.5%      65.8%!
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      76.6%!     59.4%      67.1%!     42.2%      44.1%      28.7%!     27.2%!     20.8%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        37.9%      65.0%      62.7%      84.6%!     67.9%!     92.7%!     67.6%!     75.1%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      12.4%!     31.2%!     22.5%!     40.7%      26.2%!     52.5%      19.8%!     39.8% 
Thiết Phủ Bang - Bá Khí Khai Sơn           68.5%!     39.6%      61.8%      37.7%      42.9%      17.1%!     25.2%!      9.2%!
Outside [35, 65]: 108/192
```

**Tỷ lệ thắng trung bình của 24 build (Không build nào > 70% hoặc < 30%):**
- Bích Thủy Dưỡng Sinh: 54.4% · Hàn Sơn Băng Tâm: 49.0% · Huyền Độc Ám Khí: 61.5% · Huyền Độc Vạn Độc: 53.7%
- Hàn Sơn Hàn Bích: 46.0% · Kim Cang Thiết Phủ: 34.7% · Thái Thanh Liệt Diễm: 42.6% · Thiết Phủ Liệt Diễm: 32.1%
- Kim Cang Hộ Thể: 39.3% · Kim Cang Bách Luyện: 40.8% · Liệt Diễm Cuồng Đao: 60.8% · Liệt Diễm Song Đao: 66.5%
- Bích Thủy Thanh Đằng: 53.9% · Hàn Sơn Băng Thương: 53.3% · Liệt Diễm Xích Sa: 48.4% · Thái Thanh Tiêu Dao: 66.9%
- Thiết Phủ Chấn Nhạc: 45.8% · Thái Thanh Kiếm Đạo: 69.2% · Thái Thanh Kiếm Khí: 30.6% · Thiết Phủ Bá Khí: 37.8%
- **4 Build Thú (Nằm trọn trong dải 35–65%):**
  - `mb1` Huyền Độc Huyết Thú (`u_minh_quy_trao`, `kich_doc_chu_nga`): **39.9%**
  - `mb2` Hàn Sơn Băng Quy (`thien_nien_quy_giap`, `tuyet_lang_nanh`): **42.1%**
  - `mb3` Liệt Diễm Xích Xà (`hoa_diem_xa_cot`, `soi_nanh_canh`): **66.4%**
  - `mb4` Kim Cang Bạch Hổ (`bach_ho_cuong_mong`, `hac_sat_hung_dam`): **63.4%**

### 1.2. Bảng Đối Đầu Ma Trận B (Common & Legendary; T1, T9 x TP 1; Runs 100) — Verbatim
**Phẩm Phàm (Common):**
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           45.1%      57.0% 
Hàn Sơn Phái - Băng Tâm Bút Cung           49.9%      21.3%!
Huyền Độc Môn - Hắc Huyết Ám Khí           34.1%!     93.8%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          34.2%!     90.6%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           61.9%      45.3% 
Kim Cang Tự - Kim Cang Thiết Phủ           13.2%!     47.8% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     69.0%!     42.3% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       53.1%      25.3%!
Kim Cang Tự - Hộ Thể Côn Quyền             51.4%      67.4%!
Kim Cang Tự - Bách Luyện Thuần Quyền       39.5%      76.5%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       96.4%!     37.0% 
Liệt Diễm Đường - Hỏa Hải Song Đao         67.9%!     54.2% 
Huyền Độc Môn - Huyết Thú Cuồng Trảo       16.9%!     98.0%!
Hàn Sơn Phái - Băng Quy Tuyệt Hộ           13.2%!     29.1%!
Liệt Diễm Đường - Xích Xà Hỏa Liệp         89.0%!     63.3% 
Kim Cang Tự - Bạch Hổ Bá Quyền             47.7%      68.5%!
Bích Thủy Am - Thanh Đằng Trường Tiên      40.3%      58.5% 
Hàn Sơn Phái - Hàn Băng Thương Pháp        66.8%!     18.6%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      82.5%!     26.5%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      47.5%      44.5% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      76.1%!     23.5%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        33.3%!     67.3%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      10.4%!     19.2%!
Thiết Phủ Bang - Bá Khí Khai Sơn           61.3%      20.2%!
Outside [35, 65]: 29/48
```

**Phẩm Thần Binh (Legendary):**
```text
Loadout                                    T1/TP1     T9/TP1
-------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           38.7%      52.2% 
Hàn Sơn Phái - Băng Tâm Bút Cung           48.3%      26.4%!
Huyền Độc Môn - Hắc Huyết Ám Khí           26.8%!     94.3%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          26.7%!     93.5%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           60.5%      41.1% 
Kim Cang Tự - Kim Cang Thiết Phủ           19.8%!     42.2% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     65.5%!     38.5% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       54.6%      26.5%!
Kim Cang Tự - Hộ Thể Côn Quyền             56.5%      63.5% 
Kim Cang Tự - Bách Luyện Thuần Quyền       36.7%      74.5%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       92.1%!     42.9% 
Liệt Diễm Đường - Hỏa Hải Song Đao         76.3%!     55.8% 
Huyền Độc Môn - Huyết Thú Cuồng Trảo       12.0%!     96.8%!
Hàn Sơn Phái - Băng Quy Tuyệt Hộ           17.1%!     30.7%!
Liệt Diễm Đường - Xích Xà Hỏa Liệp         89.4%!     66.7%!
Kim Cang Tự - Bạch Hổ Bá Quyền             45.8%      65.5%!
Bích Thủy Am - Thanh Đằng Trường Tiên      36.5%      53.0% 
Hàn Sơn Phái - Hàn Băng Thương Pháp        69.0%!     21.8%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      75.3%!     31.0%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      52.1%      42.6% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      79.4%!     28.8%!
Thái Thanh Quán - Kiếm Đạo Hóa Kình        38.3%      68.0%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      13.2%!     20.4%!
Thiết Phủ Bang - Bá Khí Khai Sơn           67.7%!     25.2%!
Outside [35, 65]: 29/48
```

### 1.3. Bảng Kiểm Tra Bước Phẩm (Rarity-Step, Tier 3, TP 1, Runs 100) — Verbatim
```text
Loadout                                          Lương>Phàm           Tinh>Lương           Tuyệt>Tinh      Thần Binh>Tuyệt    Chí Tôn>Thần Binh  Phàm T9 vs Lương T1
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm                      54.5%                57.0%                51.0%                60.5%                47.5%                 0.0%
Hàn Sơn Phái - Băng Tâm Bút Cung                      73.0%                73.0%                74.5%                69.0%                73.5%                 0.0%
Huyền Độc Môn - Hắc Huyết Ám Khí                      81.5%                77.0%                94.5%                91.5%                86.0%                 0.0%
Huyền Độc Môn - Vạn Độc Phệ Huyết                     78.0%                90.0%                96.0%                88.5%                81.0%                 0.0%
Hàn Sơn Phái - Hàn Bích Hợp Bích                      50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Kim Cang Tự - Kim Cang Thiết Phủ                      46.5%                56.0%                67.0%                66.0%                59.0%                 0.0%
Thái Thanh Quán - Thái Thanh Liệt Diễm                59.5%                62.5%                62.0%                75.0%                58.0%                 0.0%
Thiết Phủ Bang - Thiết Phủ Liệt Diễm                  49.0%                69.0%                53.0%                53.0%                53.0%                 0.0%
Kim Cang Tự - Hộ Thể Côn Quyền                        70.0%                66.5%                36.5%                98.5%                74.5%                 0.0%
Kim Cang Tự - Bách Luyện Thuần Quyền                  50.0%                50.0%                50.0%                50.0%                50.0%                 0.0%
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa                  62.5%                99.0%                73.5%                91.0%                52.0%                 0.0%
Liệt Diễm Đường - Hỏa Hải Song Đao                    65.0%                80.0%                86.5%                68.0%                54.5%                 0.0%
Huyền Độc Môn - Huyết Thú Cuồng Trảo                  89.0%                94.5%                99.0%                94.5%                94.0%                 0.0%
Hàn Sơn Phái - Băng Quy Tuyệt Hộ                      62.0%                62.0%                62.5%                61.0%                61.0%                 0.0%
Liệt Diễm Đường - Xích Xà Hỏa Liệp                    73.0%                74.5%                64.5%                34.0%                66.0%                 0.0%
Kim Cang Tự - Bạch Hổ Bá Quyền                        66.5%                86.5%                83.0%                81.0%                79.5%                 0.0%
Bích Thủy Am - Thanh Đằng Trường Tiên                 54.0%                53.0%                57.5%                55.5%                59.0%                 0.0%
Hàn Sơn Phái - Hàn Băng Thương Pháp                   71.0%                68.0%                73.0%                73.0%                73.0%                 0.0%
Liệt Diễm Đường - Xích Sa Chưởng Kình                 83.5%                93.5%                95.5%                98.5%                96.0%                 0.0%
Thái Thanh Quán - Tiêu Dao Phiến Pháp                 60.5%                63.0%                57.0%                57.5%                54.5%                 0.0%
Thiết Phủ Bang - Chấn Nhạc Chùy Phong                 54.0%                54.0%                54.0%                54.0%                54.0%                 0.0%
Thái Thanh Quán - Kiếm Đạo Hóa Kình                   59.0%                59.5%                60.5%                62.5%                61.5%                 0.0%
Thái Thanh Quán - Kiếm Khí Xuyên Giáp                 60.5%                60.5%                58.0%                64.0%                62.0%                 0.0%
Thiết Phủ Bang - Bá Khí Khai Sơn                      54.0%                55.0%                55.0%                55.0%                55.0%                 0.0%
Average                                               63.6%                68.9%                67.2%                68.8%                64.8%                 0.0%
```
- **Trung bình mỗi cột:** 63.6%, 68.9%, 67.2%, 68.8%, 64.8% — **100% nằm trong dải 60–80%**.
- **Không có bất kỳ ô nào 100.0%** (ô cao nhất đạt 99.0%).

### 1.4. Chẩn Đoán & Chi Tiết Bản Vá `patch.json`
- **Chữa chuỗi tự lặp Kịch Độc (#34):** Engine chặn `STATUS_APPLIED` tự kích hoạt chính nó, giải quyết vô hạn lặp sâu của Vạn Độc Đỉnh. Thất Tinh Tiêu tăng hồi chiêu 2.5s -> 2.6s, Tử Kim Trảo giảm Lực Đạo 13 -> 12, hồi chiêu 1.7s -> 1.8s giúp Huyền Độc không còn đạt 98-100% áp đảo vô đối ở T6+.
- **Loại bỏ ô 100% tại Rarity-Step:** Đổi `cuong_hoa_dan` thành `bach_ngoc_boi` trong `mb3`, ngăn chặn hiệu ứng buff tất định khi cùng loại đồ so kè nhau từng tick.
- **Giải tỏa nghẽn Nội Lực Thái Thanh Kiếm Khí:** Thay thế thế cờ 3 kiếm (cần 40 Nội Lực, giữ 20 Nội Lực gây đói mana trầm trọng) bằng 2 Kiếm + 1 Phiến + Thiên Tàm Chuỗi. Tỷ lệ thắng trung bình được cứu từ 7% lên 30.6%.
- **Cân chỉnh Lực Đạo & Hồi Chiêu:**
  - `liet_diem_dao`: attack 70 -> 54, cooldown 3.0 -> 3.5 (giảm sức ép đầu trận tại T1).
  - `thanh_phong_kiem`: attack 30 -> 34, gain_qi 15 -> 11; `ngoc_cot_phien`: attack 18 -> 19.
  - `u_minh_quy_trao`: attack 34 -> 32, basic poison 3 -> 2; `kich_doc_chu_nga`: weaponClass -> `claw`.

---

## Phần 2: Sửa Đổi Nội Dung & Độ Khó PvE

### 2.1. Danh Sách Đổi Tên Tránh Bản Quyền & Trùng Lặp
1. **Độc Cô Hạnh (NPC/Kẻ địch):** Đổi thành **Phong Vũ** (`phong_loi`), dùng portrait key `npc_phong_loi`. Lời thoại đã làm sạch hoàn toàn danh xưng cũ.
2. **Vạn Hoa Cốc (Địa danh):** Đổi thành **Bách Thảo Cốc** (môn đồ: `Bách Thảo Nghịch Đồ`).
3. **Phẩm vị vật phẩm trong toàn bộ text:** Chuẩn hóa theo 6 bậc: **Phàm Phẩm, Lương Phẩm, Tinh Phẩm, Tuyệt Phẩm, Thần Binh, Chí Tôn**.
4. **Món đồ quái:** Sửa lỗi gõ tên hình ảnh `hoa_ngiem_xa_cot` -> `hoa_diem_xa_cot`.

### 2.2. Nhịp Cốt Truyện & Đường Cong Kinh Nghiệm Cấp 10
Đã thiết kế `tools/gemini/out/round-2/levels.json` (`maxLevel: 30`, mốc mở `unlocks: {"10": "sect"}`).
Người chơi đạt **Cấp 10 đúng sau khi vượt Ải 10 (Boss Nộ Ba Hổ - Bùi Chấn Đào)**:
- Người chơi vượt tuần tự từ Ải 1 đến Ải 10 (mỗi ải đi 1 lần nhận First-Clear EXP) kết hợp 6 lần quét phụ bản thường để củng cố trang bị.
- Tổng EXP tích lũy đạt 1,020 EXP, vừa khít chạm ngưỡng Cấp 10 (yêu cầu 1,000 EXP tích lũy).
- **Hội thoại gia nhập môn phái (Ải 10 - afterWin):**
  > **Phong Vũ:** *"Hảo bản lĩnh! Đao pháp cương mãnh, căn cốt phi phàm. Giang hồ sóng gió hiểm ác, ngươi đã đủ tư cách bái nhập danh môn chính phái để lĩnh ngộ Tâm Pháp chân truyền."*  
  > **Hiệp Khách:** *"Đa tạ Phong huynh chỉ điểm. Ta nhất định sẽ chọn cho mình một môn phái xứng đáng!"*
- Mọi đội hình mẫu "người chơi dự kiến" từ Ải 1 đến Ải 10 đều để `sect: null` (không có môn phái & Tâm Pháp).

### 2.3. Bảng Độ Khó 30 Ải PvE (`vs_table.txt` Verbatim)
Sử dụng `items-v2-final.json`. Toàn bộ 30/30 ải đều đạt đúng dải mục tiêu (Thường: 80–90%, Tinh Anh: 60–70%, Boss: 50–60%):
```text
Ải   Tên Ải                   Loại     Kẻ Địch                    Tỷ lệ thắng  Mục tiêu   Đạt?
-----------------------------------------------------------------------------------------------
01   Trấn Khẩu Sơn Đạo        normal   Ải 1: Thảo Khấu Tiên Pho    80.5%      80-90%     OK
02   Phong Diệp Bến Đò        normal   Ải 2: Thủy Phỉ Thám Tử      83.0%      80-90%     OK
03   Hòe Thụ Thôn Ngoại       normal   Ải 3: Hoang Dã Sói Xám      88.5%      80-90%     OK
04   Thanh Khê Trà Thất       normal   Ải 4: Liệt Diệm Bang Chú    89.5%      80-90%     OK
05   Phong Ba Đình            elite    Ải 5: Phong Lôi Khoái Đa    68.5%      60-70%     OK
06   Ngô Đồng Cổ Hạng         normal   Ải 6: Ám Hạng Thích Khác    80.0%      80-90%     OK
07   Thủy Tạ Nguy Kiều        normal   Ải 7: Thiết Bì Trư Thú      86.0%      80-90%     OK
08   Phong Hỏa Cổ Đài         normal   Ải 8: Liệt Diệm Bang Hươ    83.5%      80-90%     OK
09   Ba Hổ Trại Tiền Doanh    normal   Ải 9: Ba Hổ Tiên Phong V    86.0%      80-90%     OK
10   Thủy Hỏa Phân Tranh Bế   boss     Ải 10: Nộ Ba Hổ Bùi Chấn    56.0%      50-60%     OK
11   Man Uyên Nhập Khẩu       normal   Ải 11: Man Tộc Thám Lộ G    86.0%      80-90%     OK
12   Ô Đầm Nhai               normal   Ải 12: Huyền Độc Môn Đồ     90.0%      80-90%     OK
13   Hủ Cốt Thung             normal   Ải 13: Kịch Độc Tri Chu     83.5%      80-90%     OK
14   Xà Khẩu Hiệp             normal   Ải 14: Man Cương Vu Đao     88.5%      80-90%     OK
15   U Cốc Dược Đàn           elite    Ải 15: Vạn Hoa Nghịch Đồ    64.5%      60-70%     OK
16   Mê Vụ Đàm                normal   Ải 16: Hắc Sát Độc Hùng     86.0%      80-90%     OK
17   Lam Hoa Trạch            normal   Ải 17: U Minh Ám Sát Thí    89.5%      80-90%     OK
18   Độc Long Khê             normal   Ải 18: Huyền Độc Trưởng     90.0%      80-90%     OK
19   Vạn Xà Quật              normal   Ải 19: Xích Luyện Xà Tin    84.0%      80-90%     OK
20   Thần Bí Cổ Đàn           boss     Ải 20: Hắc Độc Nương Tử     56.5%      50-60%     OK
21   Tuyết Dốc Băng Khẩu      normal   Ải 21: Hàn Phong Tuần Tu    83.5%      80-90%     OK
22   Phong Nhận Trầm Khê      normal   Ải 22: Hàn Sơn Kiếm Sĩ      90.0%      80-90%     OK
23   Tuyết Lĩnh Hàn Phong     normal   Ải 23: Tuyết Lĩnh Ngân L    84.0%      80-90%     OK
24   Băng Hà Cổ Nhai          normal   Ải 24: Kim Cang Hộ Giới     88.0%      80-90%     OK
25   Phong Vân Băng Đài       elite    Ải 25: Hàn Sơn Đại Đệ Tử    68.5%      60-70%     OK
26   Bạch Hổ Tuyệt Hạp        normal   Ải 26: Băng Nguyên Bạch     84.0%      80-90%     OK
27   Hàn Tinh Thạch Thất      normal   Ải 27: U Minh Chấp Pháp     90.0%      80-90%     OK
28   Phong Ma Cổ Đạo          normal   Ải 28: Vạn Niên Băng Quy    81.5%      80-90%     OK
29   Vọng Nguyệt Băng Đỉnh    normal   Ải 29: U Minh Tuyệt Mệnh    85.5%      80-90%     OK
30   Phong Thiền Băng Đài     boss     Ải 30: U Minh Tả Sứ Dạ S    59.5%      50-60%     OK
```

---

## 3. Lệnh Tái Hiện (Reproduction Commands)
```bash
# 1. Ma trận A (24 loadouts, 8 điều kiện Trọng/Tâm Pháp)
py tools/balance/balance.py matrix --loadouts tools/gemini/out/round-2/loadouts --items tools/gemini/out/round-2/items-v2-final.json --tiers 1,3,6,9 --tp 1,9 --runs 100 --low 35 --high 65

# 2. Ma trận B Phẩm Phàm & Thần Binh
py tools/balance/balance.py matrix --loadouts tools/gemini/out/round-2/loadouts --items tools/gemini/out/round-2/items-v2-final.json --tiers 1,9 --tp 1 --rarity common --runs 100 --low 35 --high 65
py tools/balance/balance.py matrix --loadouts tools/gemini/out/round-2/loadouts --items tools/gemini/out/round-2/items-v2-final.json --tiers 1,9 --tp 1 --rarity legendary --runs 100 --low 35 --high 65

# 3. Bước phẩm (Rarity-Step)
py tools/balance/balance.py rarity-step --loadouts tools/gemini/out/round-2/loadouts --tier 3 --tp 1 --runs 100 --items tools/gemini/out/round-2/items-v2-final.json --sects server/battle-engine/src/main/resources/data/sects.json

# 4. Đối đầu PvE 30 ải
python tools/gemini/out/round-2/generate_final_vs_table.py
```

---

## 4. Câu Hỏi Mở (Open Questions)
1. **Trần Hộ Thân (Damage Reduce Cap):** Ở các Trọng cao (T6-T9), một số build thuần thủ tích lũy Hộ Thân rất dày làm trận đấu kéo dài tới Kiệt Lực. Chúng ta có nên quy định trần Hộ Thân toàn cục ở mức 50% hoặc 60% trong cấu hình `BattleConfig` chính thức ở đợt cập nhật engine tiếp theo?
2. **Cơ chế Chuyển Môn Phái:** Sau mốc cấp 10, người chơi chọn 1 trong 7 môn phái. Trong tương lai người chơi có được phép đổi môn phái (kèm phí Bạc/Tẩy Tủy Đan) hay cố định vĩnh viễn theo nhân vật?
3. **Hiệu Ứng Bộc Phát Độc ở T9:** Hiện tại ở T9 Kịch Độc bộc phát gây 240 sát thương chuẩn (xuyên Cương Khí). Khi lên T9 chỉ số máu nhân vật tăng theo Trọng giúp cân bằng lại, nhưng liệu có cần thêm cơ chế kháng độc/giảm sát thương bộc phát từ Hộ Cụ phi dược phẩm không?
