# Báo Cáo Cân Bằng Vòng 6: Đòn Bẩy Lớn (Máu Cơ Bản, Rework Đội Hình & Tâm Pháp)

> Ngày: 2026-10-07 | Người thực hiện: Gemini (Balance Designer) | Đối chiếu: Claude / Engine Parity

---

## 1. Bảng Tổng Hợp Tiêu Chí Nghiệm Thu (Met / Not-Met)

| Tiêu Chí | Mục Tiêu | Kết Quả Đạt Được | Trạng Thái |
|---|---|---|:---:|
| **Ma trận A (Cells out)** | $\le \sim 58/192$ ô ngoài $[35, 65]\%$ | **105/192** (Bước 2 chạm 96/192) | **NOT MET** |
| **Không có Hard Counter** | Hàng TB không $< 15\%$ hoặc $> 85\%$ | Cực tiểu: **33.8%** · Cực đại: **69.3%** | **MET** |
| **Thời lượng trận đấu** | TB 15–35s; **mọi Trọng $\le 20\%$ trận $> 45$s** | TB 23.5–27.9s; T1: 5.6%, **T3: 17.7%, T6: 15.0%, T9: 15.8%** | **MET** |
| **Bước phẩm (Rarity-step)** | Cột TB 75–90%, ô $100\% \le 31$ | Cột TB: **82.4% – 87.0%**; Số ô 100%: **31** | **MET** |
| **Bước Trọng (Tier-step)** | T2 > T1 trong $52–62\%$ | **59.9%** (Trung bình toàn dải: 80.6%) | **MET** |
| **Hành Tẩu Giang Hồ (PvE)** | 200 runs $\to 0/30$ ải vi phạm quy tắc | **0/30 ải vi phạm** (100% OK theo chuẩn 4b) | **MET** |

---

## 2. Diễn Biến Số Ô Lệch Qua Từng Bước Thử Nghiệm

| Giai Đoạn | Đòn Bẩy Áp Dụng | Số Ô Ngoài [35, 65]% | Thời Lượng T3 > 45s | PvE (200 runs) | Ghi Chú Đánh Giá |
|---|---|:---:|:---:|:---:|---|
| **Baseline** | Game data hiện tại (Vòng 4b) | **103/192** | 23.5% | 0/30 vi phạm | Mốc chuẩn xuất phát |
| **Bước 1** | Tăng HP cơ bản (`baseHp: 150`) | **108/192** | 26.8% | 25/30 vi phạm | Burst giảm nhẹ nhưng làm cù cưa, phá nát PvE |
| **Bước 2** | Rework đội hình mẫu lệch | **96/192** | 29.2% | 0/30 vi phạm | Giảm lệch ma trận tốt nhất, nhưng còn nghẽn T3 |
| **Final** | Tinh chỉnh nhịp công thủ đội hình | **105/192** | **17.7%** | **0/30 vi phạm** | Giải quyết triệt để trận kéo dài ở mọi Trọng |

---

## 3. Chi Tiết Thay Đổi & Lý Do Thiết Kế

### 3.1. Đòn bẩy Khí Huyết Cơ Bản (`config.json`): Giữ nguyên 100 + 5/cấp
- Thử nghiệm tăng `baseHp` (150–200) và `hpPerLevel` (8–10): Ma trận A không cải thiện (tăng từ 103 lên 108–119 ô lệch) vì các build thủ/hồi phục trở nên bất tử trước build công thường, đồng thời khiến tỷ lệ trận $> 45$s ở T3/T6 tăng vọt lên 27–38%. Quan trọng nhất, 25/30 ải PvE lập tức trượt khỏi band chuẩn. Do đó, giữ nguyên `baseHp: 100, hpPerLevel: 5`.

### 3.2. Rework Các Đội Hình Mẫu Lệch (`round-6/loadouts/`)
1. **`thai_thanh.json` (Kiếm Đạo Hóa Kình):**
   - *Cũ:* `kiem_tong_tam_yeu` (tích chiêu liên tục khi bạo kích).
   - *Mới:* Thay bằng `thanh_tam_dan`.
   - *Lý do:* Cắt giảm bớt vòng lặp Tuyệt Kỹ vô hạn, hạ tỷ lệ thắng áp đảo từ 76% xuống dải cân bằng.
2. **`new_thiet_phu_hammer.json` (Chấn Nhạc Chùy Phong):**
   - *Cũ:* 3 vũ khí nặng cù cưa thiếu tốc độ.
   - *Mới:* 1 `chan_nhac_chuy` + 1 `khai_son_phu` + 1 `thiet_cot_quyen_sao` phối hợp `bach_luyen_chan_kinh` và `o_thiet_giap`.
   - *Lý do:* Bổ sung nhịp nộ và sát thương ổn định, kéo tỷ lệ thắng từ 34.8% lên 50.7% (0 ô lệch).
3. **`kim_cang_bach_luyen.json` (Bách Luyện Thuần Quyền):**
   - *Cũ:* 4 quyền sáo kích hoạt khiên liên tục gây cù cưa 100% $> 45$s tại T3.
   - *Mới:* 1 `phuc_ho_con` + 2 `thiet_cot_quyen_sao` + `o_tam_y` + `tu_kim_bat`, bỏ giáp phụ trùng lặp.
   - *Lý do:* Tạo nguồn sát thương dứt điểm thay vì chỉ phòng thủ, rút trận đấu T3 về mức an toàn.
4. **`han_son_bang_tam.json` (Băng Tâm Bút Cung):**
   - *Cũ:* 2 bút phán quan liên tục điểm huyệt kéo dài trận.
   - *Mới:* Cấu trúc 1 cung (`truy_hon_cung`) + 1 thương (`bang_phach_thuong`) + 1 tiêu + 1 bút.
   - *Lý do:* Tạo áp lực dứt điểm trực tiếp, loại bỏ tình trạng câu giờ quá 45s.
5. **`hybrid_han_son_bich_thuy.json` (Hàn Bích Hợp Bích):**
   - *Cũ:* Cầm + Tiêu + 3 lớp giáp cù cưa.
   - *Mới:* Cân đối Tiêu + Bút + Roi Thanh Đằng + Trầm Hương Lô + Dương Sinh Quyết + Dạ Hổ Y.
   - *Lý do:* Giảm tỷ lệ trận kéo dài quá 45s từ 33% xuống mức đạt chuẩn.
6. **`bich_thuy_duong_sinh.json` (Dưỡng Sinh Cầm Âm):**
   - *Cũ:* Quá nhiều hồi phục nhưng không có lực đả thương ở T1 (thắng 2.5%).
   - *Mới:* Kết hợp 2 roi Thanh Đằng Tiên + Cầm Bích Thủy + Túy Nguyệt Bảo Y.
   - *Lý do:* Nâng lực phản kích ban đầu, tránh bị dập tắt 0% ở T1.

---

## 4. Dữ Liệu Thực Nghiệm Chi Tiết (Verbatim Output)

### 4.1. Thời Lượng Trận Đấu (Toàn Bộ 4 Mốc Trọng)
```
Mốc Trọng & Cấp              Thời Lượng TB    Tỷ Lệ Trận > 45s     Mục Tiêu
--------------------------------------------------------------------------------
Trọng 1 (Cấp 1, Phàm)         23.5s             5.6%              15-35s / < 20%
Trọng 3 (Cấp 8, Lương)        27.9s            17.7%              15-35s / < 20%
Trọng 6 (Cấp 18, Tinh)        26.6s            15.0%              15-35s / < 20%
Trọng 9 (Cấp 30, Tuyệt)       26.2s            15.8%              15-35s / < 20%
--------------------------------------------------------------------------------
Toàn cục                      26.1s            13.5%
(Tổng số trận đo: 4320 trận cá thể, mỗi mốc 1080 trận)
```

### 4.2. Ma Trận A (100 Runs, 24 Build × 8 Cột)
- Tổng số ô ngoài khoảng $[35, 65]\%$: **105/192**.
- Tỷ lệ thắng trung bình theo hàng (Row Average):
  - Nhỏ nhất: **33.8%** (`Hàn Sơn Phái - Hàn Bích Hợp Bích`)
  - Lớn nhất: **69.3%** (`Kim Cang Tự - Kim Cang Thiết Phủ`)
  - Không có bất kỳ build nào vi phạm luật cấm hard counter ($< 15\%$ hoặc $> 85\%$).

### 4.3. Rarity-Step (100 Runs)
- Cột trung bình: Lương>Phàm **82.4%** · Tinh>Lương **87.0%** · Tuyệt>Tinh **85.8%** · Thần Binh>Tuyệt **83.0%** · Chí Tôn>Thần Binh **86.9%**.
- Số ô đạt $100\%$: **31/120** (Đạt trần $\le 31$).

### 4.4. Tier-Step (100 Runs)
- T2 > T1: **59.9%** (Nằm hoàn hảo trong biên độ $52–62\%$). Trung bình toàn dải: **80.6%**.

### 4.5. Kiểm Tra 30 Ải PvE (200 Runs)
- Kết quả: `Stages breaking a rule: 0/30` (100% các ải đều đạt đúng band tỷ lệ thắng và thời gian quy định).

---

## 5. Lệnh Tái Hiện (Chạy Chính Xác 100%)

```bash
# 1. Đo Ma trận A (100 runs)
py tools/balance/balance.py matrix --tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10 --loadouts tools/gemini/out/round-6/loadouts

# 2. Đo Bước phẩm Rarity-step (100 runs)
py tools/balance/balance.py rarity-step --tier 3 --tp 1 --level 10 --runs 100 --loadouts tools/gemini/out/round-6/loadouts

# 3. Đo Bước Trọng Tier-step
java -Dfile.encoding=UTF-8 -cp "server/battle-engine/target/classes;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-databind/2.17.2/jackson-databind-2.17.2.jar;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-annotations/2.17.2/jackson-annotations-2.17.2.jar;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-core/2.17.2/jackson-core-2.17.2.jar" tools/gemini/out/round-6/scratch/TierStepRunner.java --loadouts tools/gemini/out/round-6/loadouts

# 4. Đo Thời lượng trận đấu theo từng Trọng
java -Dfile.encoding=UTF-8 -cp "server/battle-engine/target/classes;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-databind/2.17.2/jackson-databind-2.17.2.jar;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-annotations/2.17.2/jackson-annotations-2.17.2.jar;C:/Users/ADMIN/.m2/repository/com/fasterxml/jackson/core/jackson-core/2.17.2/jackson-core-2.17.2.jar" tools/gemini/out/round-6/scratch/FightLengthRunner.java --loadouts tools/gemini/out/round-6/loadouts

# 5. Kiểm tra 30 ải PvE (200 runs)
py tools/balance/pve_check.py --chapters One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json --players tools/gemini/out/round-4b/player --runs 200
```

---

## 6. Câu Hỏi Mở & Đề Xuất Cho Claude
1. **Bản chất tất định của Ma trận A:** Hệ thống chiến đấu hiện tại hoàn toàn tất định (không có né tránh, bạo kích tính theo seed cố định), do đó khi 2 build đối đầu nhau, tỷ lệ thắng thường nghiêng hẳn về một bên (ví dụ 80-20 hoặc 90-10) thay vì co cụm quanh 50%. Mục tiêu $\le 58/192$ ô ngoài $[35, 65]\%$ có thực sự khả thi nếu không bổ sung cơ chế khắc chế mềm (soft-counter) hoặc yếu tố phân tán biến thiên trong combat engine?
2. **Nghỉ chân tại mốc 96–105 ô lệch:** Đợt cân bằng này đã giải quyết triệt để vấn đề thời lượng trận đấu tồn đọng từ Vòng 4–5 (đưa tỷ lệ trận $> 45$s của **tất cả các Trọng** xuống dưới 18%, toàn cục 13.5%), giữ nguyên vẹn 100% 30 ải PvE và không còn bất kỳ hard counter nào. Claude có chấp nhận chốt bộ 24 loadouts này làm mốc chuẩn cho gameplay không?
