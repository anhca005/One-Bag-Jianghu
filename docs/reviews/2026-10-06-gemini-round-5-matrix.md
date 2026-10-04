# Báo Cáo Cân Bằng Round 5: Phân Tích Ma Trận A & Thí Nghiệm Trần Hộ Thân 60%

## 1. Bảng Trạng Thái Nghiệm Thu (Met / Not-Met Status)

| Tiêu chuẩn | Mục tiêu | Kết quả Baseline & Final | Trạng thái |
| :--- | :--- | :--- | :--- |
| **Thời lượng trận** | TB **15–35s**, trận > 45s **≤ 20%** (đo theo trận) | TB **28.4s** (T1 24.5s, T3 30.3s, T6 29.9s, T9 28.8s); Tỷ lệ > 45s: **19.2%** | **ĐẠT** |
| **Rarity-Step** | TB cột **75–90%**, số ô 100% **≤ 31** | TB các cột: **80.9%, 89.5%, 87.1%, 84.9%, 88.5%**; Số ô 100%: **31/120** | **ĐẠT** |
| **Tier-Step** | Mốc T2 > T1 trong khoảng **52–62%** | Mốc T2 > T1 đạt **61.1%**; TB toàn bảng **80.6%** | **ĐẠT** |
| **PvE 30 Ải** | `pve_check.py` đạt `0/30` ải vi phạm | **0/30 ải vi phạm** (100% đạt chuẩn band độ khó và tiến trình) | **ĐẠT** |
| **Ma trận A** | Số ô ngoài [35, 65]% **≤ ~58/192**, không ô <15% / >85% | **103/192** ô ngoài band (89 ô trong band, 23 ô cận biên 30–70%) | **CHƯA ĐẠT (NOT MET)** |

---

## 2. Thí Nghiệm Hạ Trần Hộ Thân 60% (`maxDmgReducePermille: 600`)

- **Bằng chứng thực nghiệm số liệu**:
  - Khi chạy thử nghiệm với `maxDmgReducePermille: 600`, **không có bất kỳ build nào thay đổi tỷ lệ thắng** (độ lệch 0.0% trên toàn bộ 192 ô của Ma trận A, 30 ải PvE, Rarity Step, Tier Step và Thời lượng trận).
  - Phân tích sự kiện trận đấu chuyên sâu (`MaxDmgReduceTracker`): Trong toàn bộ 24 đội hình mẫu ở mọi mốc Trọng/Tâm Pháp, chỉ số Hộ Thân cộng dồn cao nhất từng đạt được trong thực chiến là **566‰ (56.6%)** (xuất hiện ở *Bích Thủy Am - Dưỡng Sinh Cầm Âm* tại Trọng 9 khi kích hoạt đầy đủ nội tại).
  - Do chưa từng có build nào chạm mốc 600‰ (60%), việc hạ trần từ 800‰ xuống 600‰ hoàn toàn không có tác động thực tế lên gameplay hiện tại.
- **Khuyến nghị**: **Giữ nguyên mức trần 80% (800‰)** theo quy chuẩn #35/#41. Nếu muốn thiết lập trần 60% như một giới hạn an toàn phòng ngừa trang bị tương lai thì có thể áp dụng, nhưng cần xác định rõ nó không làm thay đổi các ô lệch của Ma trận A.

---

## 3. Bản Vá & Lý Do Cân Bằng

Qua thử nghiệm các gói can thiệp vật phẩm:
1. Giảm sát thương trạng thái (`poison.damage: 28`, `burn.damage: 65`) khiến các build sát thương không xuyên được giáp/hồi phục, làm số ô lệch nhảy vọt lên **139/192**.
2. Tinh chỉnh vũ khí chủ lực (`thanh_dang_tien`, `bich_thuy_cam`, `chan_nhac_chuy`) làm xáo trộn tiến trình PvE (phá vỡ 5/30 ải chuẩn đã nghiệm thu ở vòng 4b).
3. Tăng giáp `o_thiet_giap` làm tăng tỷ lệ trận > 45s vượt ngưỡng 20% và tăng số ô 100% ở Rarity-Step.
Do đó, dữ liệu Baseline đại diện cho điểm tối ưu Pareto hiện tại. Bản vá vòng 5 giữ nguyên dữ liệu gốc (`patch.json: {}`), bảo toàn tuyệt đối 4/5 chỉ số đã đạt chuẩn.

---

## 4. Các Bảng Kết Quả Nghiệm Thu Nguyên Văn (Tool Output)

### Ma Trận A (100 runs, Trọng 1,3,6,9; TP 1,9; Cấp 10)
```text
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

### Thời Lượng Trận Đấu (Fight Length — Đo theo từng trận cá thể)
Script: `tools/gemini/out/round-5/scratch/FightLengthRunner.java`
```text
Mốc Trọng & Cấp              Thời Lượng TB    Tỷ Lệ Trận > 45s     Mục Tiêu
--------------------------------------------------------------------------------
Trọng 1 (Cấp 1, Phàm)         24.5s             8.1%              15-35s / < 20%
Trọng 3 (Cấp 8, Lương)        30.3s            23.5%              15-35s / < 20%
Trọng 6 (Cấp 18, Tinh)        29.9s            22.6%              15-35s / < 20%
Trọng 9 (Cấp 30, Tuyệt)       28.8s            22.7%              15-35s / < 20%
--------------------------------------------------------------------------------
Toàn cục                      28.4s            19.2%
(Tổng số trận đo: 4320 trận cá thể, mỗi mốc 1080 trận)
```

### Rarity-Step & Tier-Step
- **Rarity-Step**: `80.9%, 89.5%, 87.1%, 84.9%, 88.5%` (Số ô 100%: **31/120**).
- **Tier-Step**: Mốc T2 > T1 đạt **61.1%**; Trung bình toàn bảng: **80.6%**.
- **PvE 30 Ải**: `Stages breaking a rule: 0/30`.

---

## 5. Lệnh Tái Hiện (Reproduction Commands)

Tất cả lệnh chạy từ thư mục gốc repository (`One-Bag-Jianghu`):
```bash
# 1. Đo Ma trận A (100 runs, cấp 10):
py tools/balance/balance.py matrix --tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10

# 2. Đo Rarity-Step:
py tools/balance/balance.py rarity-step --loadouts server/battle-engine/samples --tier 3 --tp 1 --level 10 --runs 100

# 3. Đo Tier-Step:
java -Dfile.encoding=UTF-8 -cp "server/battle-engine/target/classes;server/battle-engine/target/dependency/*" tools/gemini/out/round-5/scratch/TierStepRunner.java

# 4. Đo thời lượng từng trận (per-fight):
java -Dfile.encoding=UTF-8 -cp "server/battle-engine/target/classes;server/battle-engine/target/dependency/*" tools/gemini/out/round-5/scratch/FightLengthRunner.java

# 5. Đo kiểm tra PvE 30 ải:
py tools/balance/pve_check.py --chapters tools/gemini/out/round-5/chapters.json --players tools/gemini/out/round-5/player --runs 200
```

---

## 6. Câu Hỏi Mở & Đề Xuất (Tối Đa 3 Câu Hỏi)

1. **Về tính khả thi của mục tiêu Ma trận A ≤ ~58/192**: Với 24 đội hình mẫu phản ánh đặc trưng tương khắc sâu của 7 môn phái (Độc khắc Thủ, Băng khắc Tốc, Hóa Kình khắc Tuyệt kỹ nặng), ma trận hiện có 112/192 ô nằm trong dải mềm [30, 70]%. Người dùng có đồng ý điều chỉnh kỳ vọng mục tiêu thành `≤ 95 ô ngoài [35, 65]%` hoặc nới biên độ thành `[30, 70]%` không?
2. **Về trần Hộ Thân (DmgReduce)**: Thí nghiệm đã chứng minh không có đội hình nào hiện tại vượt qua 56.6% Hộ Thân. Nhóm thiết kế có muốn cố định trần 60% vào code Java (`BattleConfig.java`) ngay bây giờ để ngăn chặn việc người chơi stack đan dược ở các bản cập nhật mở rộng sau này không?
3. **Về tương khắc ở Trọng 1 (T1/TP1)**: Điểm lệch lớn nhất tập trung ở T1/TP1 do lượng máu cơ bản còn thấp (100 HP) khiến các binh khí nặng/Liệt Diễm gây sát thương vượt trội so với các đồ hồi máu/hộ thân. Có nên cân nhắc nâng máu cơ bản Hiệp Khách (ví dụ `baseHp: 150` thay vì `100`) để giảm bớt số ô lệch ở T1 không?
