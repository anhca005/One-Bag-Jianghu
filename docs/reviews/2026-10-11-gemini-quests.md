# Báo cáo Thiết Kế Nhiệm Vụ Cốt Truyện Hồi 2–5 (Hành Tẩu Lục)

- **Ngày lập:** 2026-10-11
- **Người thực hiện:** Gemini (Pair-programming assistant)
- **Tập tin liên quan:** 
  - [`quests.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/quests.json)
  - [`quests.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/quests.md)
  - [`story-bible.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/story-bible.md)
  - [`chapters.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json)

---

## 1. Kết Quả Rà Soát Hồi 1 (`quests.json`)

Giữ nguyên toàn bộ 9 nhiệm vụ (`q1_01` → `q1_09`), không thay đổi thứ tự và mã định danh (`id`). Tiến hành tinh chỉnh lời văn và hoàn thiện cơ cấu thưởng:

1. **Chuẩn hóa thuật ngữ (`q1_07`, `q1_08`):**
   - Trước: Sử dụng cụm từ `"tàn nang"`, `"mảnh tàn nang"`.
   - Sửa: Chuẩn hóa viết hoa danh từ báu vật `"Mảnh Tàn Nang"` (và `"Mảnh Tàn Nang thứ nhất"` ở `q1_08`) theo đúng quy ước tài liệu [story-bible.md](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/story-bible.md).
2. **Cân đối thưởng nguyên liệu môn phái (`q1_09`):**
   - Trước: `materials: {}`.
   - Sửa: Bổ sung thưởng `hoang_tuyen_chi: 2` (Hoàng Tuyên Chỉ - nguyên liệu tu luyện Công Pháp bậc 1). Việc này giúp người chơi sau khi hoàn thành Hồi 1 và bái nhập Môn Phái sở hữu trọn vẹn đủ cả 4 loại nguyên liệu bậc 1 của Cửu Châu (Hàn Thiết, Thanh Đồng Ti, Chu Sa Thảo, Hoàng Tuyên Chỉ) để bước vào Hồi 2.

---

## 2. Bảng Tóm Tắt Nhiệm Vụ Từng Hồi

### Hồi 1: Thanh Khê Trấn (Người giao: Lục Tiều Phu `lao_tieu_phu` & Phong Vũ `phong_loi`)

| id | Loại | Tên nhiệm vụ | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| `q1_01` | Chính | Trấn Khẩu gặp nạn | Qua ải `hoi_1_01` | `stage:hoi_1_01` | 80 Bạc, 20 EXP, Hàn Thiết x1 |
| `q1_02` | Phụ | Sắm sửa hành trang | Mua 1 món ở Thương Hội | `shop` | 100 Bạc, 20 EXP, Thanh Đồng Ti x2 |
| `q1_03` | Chính | Sói hoang ngoài thôn | Qua ải `hoi_1_03` | `stage:hoi_1_03` | 120 Bạc, 30 EXP |
| `q1_04` | Phụ | Rèn thêm một Trọng | Nâng Trọng 1 lần | `storage` | 120 Bạc, 30 EXP, Hàn Thiết x3 |
| `q1_05` | Chính | Thử kiếm Phong Ba Đình | Qua ải `hoi_1_05` | `stage:hoi_1_05` | 200 Bạc, 50 EXP |
| `q1_06` | Phụ | Bày trận trong nang | Xếp 4 món vào Bao Phục | `bag` | 120 Bạc, 30 EXP, Chu Sa Thảo x2 |
| `q1_07` | Chính | Phá tiền doanh Ba Hổ | Qua ải `hoi_1_09` | `stage:hoi_1_09` | 220 Bạc, 50 EXP |
| `q1_08` | Chính | Nộ Ba Hổ | Qua ải `hoi_1_10` | `stage:hoi_1_10` | 600 Bạc, 150 EXP, Hàn Thiết x3, Thanh Đồng Ti x3, Binh Hồn Tinh Thạch x1 |
| `q1_09` | Phụ | Bái nhập sư môn | Bái nhập một Môn Phái | `sect` | 300 Bạc, 60 EXP, Hoàng Tuyên Chỉ x2 |

---

### Hồi 2: Thập Vạn Man Uyên (Người giao: Bạch Linh Nhi `tieu_linh_nhi`)
*Trọng tâm việc phụ:* Đan Dược, khắc chế độc chướng và giải độc.

| id | Loại | Tên nhiệm vụ | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| `q2_01` | Chính | Vào chốn độc chướng | Qua ải `hoi_2_01` | `stage:hoi_2_01` | 150 Bạc, 40 EXP, Chu Sa Thảo x2 |
| `q2_02` | Phụ | Túi gấm trừ tà | Xếp 5 món vào Bao Phục | `bag` | 120 Bạc, 30 EXP, Chu Sa Thảo x2 |
| `q2_03` | Chính | Chém nhện Hủ Cốt | Qua ải `hoi_2_03` | `stage:hoi_2_03` | 160 Bạc, 40 EXP, Hàn Thiết x2 |
| `q2_04` | Phụ | Tìm mua dược thảo | Mua 2 món ở Thương Hội | `shop` | 140 Bạc, 30 EXP, Thanh Đồng Ti x2 |
| `q2_05` | Chính | Thu hồi bí dược | Qua ải `hoi_2_05` | `stage:hoi_2_05` | 220 Bạc, 50 EXP, Hoàng Tuyên Chỉ x2 |
| `q2_06` | Phụ | Luyện khí cường thân | Nâng Trọng 2 lần | `storage` | 140 Bạc, 35 EXP, Chu Sa Thảo x3 |
| `q2_07` | Chính | Chặn trảo Độc Long | Qua ải `hoi_2_08` | `stage:hoi_2_08` | 240 Bạc, 55 EXP, Hàn Thiết x3 |
| `q2_08` | Chính | Phá trận Vạn Xà | Qua ải `hoi_2_09` | `stage:hoi_2_09` | 250 Bạc, 60 EXP, Thanh Đồng Ti x3 |
| `q2_09` | Chính | Trảm Hắc Độc Nương Tử | Qua ải `hoi_2_10` | `stage:hoi_2_10` | 650 Bạc, 160 EXP, Chu Sa Thảo x3, Hàn Thiết x3, Cửu Chuyển Đan Sa x1 |

---

### Hồi 3: Hàn Nguyệt Cô Phong (Người giao: Phong Vũ `phong_loi`)
*Trọng tâm việc phụ:* Tâm Pháp tầng cao (tầng 3), Đột Phá mốc Trọng 3/6.

| id | Loại | Tên nhiệm vụ | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| `q3_01` | Chính | Hội ngộ tuyết sơn | Qua ải `hoi_3_01` | `stage:hoi_3_01` | 180 Bạc, 45 EXP, Ô Thiết x2 |
| `q3_02` | Phụ | Tâm Pháp hộ thể | Tâm Pháp tầng 3 | `sect` | 140 Bạc, 35 EXP, Mặc Trúc Trản x2 |
| `q3_03` | Chính | Phá kiếm trận Hám Tâm | Qua ải `hoi_3_02` | `stage:hoi_3_02` | 200 Bạc, 45 EXP, Ô Kim Ti x2 |
| `q3_04` | Phụ | Đột phá Tam Trọng | Có món đạt 3 Trọng | `storage` | 150 Bạc, 40 EXP, Ô Thiết x3 |
| `q3_05` | Chính | Đoạt Phong Vân Băng Đài | Qua ải `hoi_3_05` | `stage:hoi_3_05` | 240 Bạc, 55 EXP, Tử Linh Chi x2 |
| `q3_06` | Phụ | Tôi rèn Ô Thiết | Nâng Trọng 2 lần | `storage` | 150 Bạc, 35 EXP, Ô Kim Ti x3 |
| `q3_07` | Chính | Chém Băng Quy phong ma | Qua ải `hoi_3_08` | `stage:hoi_3_08` | 250 Bạc, 60 EXP, Ô Thiết x3 |
| `q3_08` | Chính | Huyết chiến Băng Đỉnh | Qua ải `hoi_3_09` | `stage:hoi_3_09` | 250 Bạc, 60 EXP, Mặc Trúc Trản x3 |
| `q3_09` | Chính | Đoạt mạng Dạ Sát Thiên | Qua ải `hoi_3_10` | `stage:hoi_3_10` | 700 Bạc, 180 EXP, Ô Thiết x3, Ô Kim Ti x3, Binh Hồn Tinh Thạch x1 |

---

### Hồi 4: Xuân Vũ Kinh Kỳ (Người giao: Yến Vô Tâm `yen_vo_tam`)
*Trọng tâm việc phụ:* Hợp Thành vật phẩm, mở rộng phối hợp Bao Phục.

| id | Loại | Tên nhiệm vụ | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| `q4_01` | Chính | Mưa xuân hộ sớ | Qua ải `hoi_4_01` | `stage:hoi_4_01` | 180 Bạc, 45 EXP, Ô Kim Ti x2 |
| `q4_02` | Phụ | Bày trận sáu món | Xếp 6 món vào Bao Phục | `bag` | 130 Bạc, 30 EXP, Tử Linh Chi x2 |
| `q4_03` | Chính | Lấy lại Cổ Dịch | Qua ải `hoi_4_02` | `stage:hoi_4_02` | 200 Bạc, 50 EXP, Ô Thiết x2 |
| `q4_04` | Phụ | Thu thập quân nhu | Mua 2 món ở Thương Hội | `shop` | 140 Bạc, 35 EXP, Mặc Trúc Trản x2 |
| `q4_05` | Chính | Vũ Đình đoạt sớ | Qua ải `hoi_4_05` | `stage:hoi_4_05` | 240 Bạc, 55 EXP, Ô Kim Ti x3 |
| `q4_06` | Phụ | Hợp Thành tăng phẩm | Hợp Thành 1 lần | `storage` | 150 Bạc, 40 EXP, Ô Thiết x3 |
| `q4_07` | Chính | Hạ Phong Đài tín tiêu | Qua ải `hoi_4_08` | `stage:hoi_4_08` | 250 Bạc, 60 EXP, Mặc Trúc Trản x3 |
| `q4_08` | Chính | Phá Thủy Môn ngoại kinh | Qua ải `hoi_4_09` | `stage:hoi_4_09` | 250 Bạc, 60 EXP, Tử Linh Chi x3 |
| `q4_09` | Chính | Hỏa chiến Lộ Tẫn Quang | Qua ải `hoi_4_10` | `stage:hoi_4_10` | 750 Bạc, 180 EXP, Ô Thiết x3, Ô Kim Ti x3, Vấn Đạo Tâm Tinh x1 |

---

### Hồi 5: Hạ Nhật Đế Kinh (Người giao: Yến Vô Tâm `yen_vo_tam`)
*Trọng tâm việc phụ:* Tổng hợp võ học (Tâm Pháp tầng 6, nâng Trọng đỉnh cao, thắng liên tiếp PvE).

| id | Loại | Tên nhiệm vụ | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| `q5_01` | Chính | Đột nhập Kim Minh môn | Qua ải `hoi_5_01` | `stage:hoi_5_01` | 200 Bạc, 50 EXP, Thiên Ngoại Vẫn Thiết x2 |
| `q5_02` | Phụ | Tâm Pháp viên dung | Tâm Pháp tầng 6 | `sect` | 160 Bạc, 40 EXP, Tằm Tơ Cổ Quyển x2 |
| `q5_03` | Chính | Chém vỡ Đồng Sư | Qua ải `hoi_5_03` | `stage:hoi_5_03` | 220 Bạc, 55 EXP, Cửu U Tàm Ti x2 |
| `q5_04` | Phụ | Bách luyện thần binh | Nâng Trọng 3 lần | `storage` | 160 Bạc, 40 EXP, Thiên Ngoại Vẫn Thiết x3 |
| `q5_05` | Chính | Đoạt Thừa Thiên Đài Ấn | Qua ải `hoi_5_05` | `stage:hoi_5_05` | 250 Bạc, 60 EXP, Côn Lôn Tuyết Liên x2 |
| `q5_06` | Phụ | Quét sạch cấm thành | Thắng 5 trận | `stage:hoi_5_01` | 180 Bạc, 45 EXP, Cửu U Tàm Ti x3 |
| `q5_07` | Chính | Giải trận Liên Đình | Qua ải `hoi_5_08` | `stage:hoi_5_08` | 250 Bạc, 60 EXP, Tằm Tơ Cổ Quyển x3 |
| `q5_08` | Chính | Cứu Xã Tắc Cơ Đàn | Qua ải `hoi_5_09` | `stage:hoi_5_09` | 260 Bạc, 60 EXP, Côn Lôn Tuyết Liên x3 |
| `q5_09` | Chính | Trừ diệt Tào Huyền Kỳ | Qua ải `hoi_5_10` | `stage:hoi_5_10` | 800 Bạc, 200 EXP, Thiên Ngoại Vẫn Thiết x3, Cửu U Tàm Ti x3, Huyền Giáp Hồn Thạch x1 |

---

## 3. Tổng Bạc, EXP và Nguyên Liệu Mỗi Hồi

| Hồi | Tổng Bạc | Tổng EXP | Nguyên liệu thưởng chi tiết |
|---|---|---|---|
| **Hồi 1** | 1.860 Bạc | 440 EXP | • Hàn Thiết: 7<br>• Thanh Đồng Ti: 5<br>• Hoàng Tuyên Chỉ: 2<br>• Chu Sa Thảo: 2<br>• Binh Hồn Tinh Thạch: 1 |
| **Hồi 2** | 2.070 Bạc | 500 EXP | • Chu Sa Thảo: 10<br>• Hàn Thiết: 8<br>• Thanh Đồng Ti: 5<br>• Hoàng Tuyên Chỉ: 2<br>• Cửu Chuyển Đan Sa: 1 |
| **Hồi 3** | 2.260 Bạc | 555 EXP | • Ô Thiết: 11<br>• Ô Kim Ti: 8<br>• Mặc Trúc Trản: 5<br>• Tử Linh Chi: 2<br>• Binh Hồn Tinh Thạch: 1 |
| **Hồi 4** | 2.290 Bạc | 555 EXP | • Ô Kim Ti: 8<br>• Ô Thiết: 8<br>• Mặc Trúc Trản: 5<br>• Tử Linh Chi: 5<br>• Vấn Đạo Tâm Tinh: 1 |
| **Hồi 5** | 2.480 Bạc | 610 EXP | • Thiên Ngoại Vẫn Thiết: 8<br>• Cửu U Tàm Ti: 8<br>• Tằm Tơ Cổ Quyển: 5<br>• Côn Lôn Tuyết Liên: 5<br>• Huyền Giáp Hồn Thạch: 1 |

### Nhận xét phân bổ kinh tế:
1. **Mức tăng lũy tiến hợp lý:** Tài nguyên Bạc và EXP tăng từ tốn qua từng Hồi (+10% đến +15% mỗi Hồi), tương xứng với độ khó và tiêu hao stamina khi đi ải.
2. **Nguyên liệu theo bậc:**
   - Hồi 1 & Hồi 2: 100% nguyên liệu Bậc 1, kèm chất dẫn Đột Phá Binh Khí (`binh_hon_tinh_thach`) và Đan Dược (`cuu_chuyen_dan_sa`).
   - Hồi 3 & Hồi 4: 100% nguyên liệu Bậc 2, kèm chất dẫn Đột Phá Binh Khí (`binh_hon_tinh_thach` tiếp theo) và Công Pháp (`van_dao_tam_tinh`).
   - Hồi 5: 100% nguyên liệu Bậc 3, kèm chất dẫn Đột Phá Hộ Cụ (`huyen_giap_hon_thach`).
3. **Phân bổ theo 4 nhánh:** Đảm bảo cả 4 nhánh võ học (Binh Khí, Hộ Cụ, Công Pháp, Đan Dược/Kỳ Trân) đều được cấp nguyên liệu cân đối để người chơi xây dựng bộ trang bị hoàn chỉnh.

---

## 4. Các Chỗ Còn Phân Vân & Kiến Nghị

1. **Khả năng đạt mốc Hợp Thành ở Hồi 4 (`q4_06`):**
   - *Vấn đề:* Mục tiêu `item_merge` đòi hỏi người chơi có **2 bản cùng món, cùng phẩm chất, đều từ Tam Trọng trở lên**. Nếu người chơi chỉ tập trung nâng cấp một bộ đồ duy nhất mà không giữ bản trùng trong kho, nhiệm vụ này có thể yêu cầu người chơi quay lại Thương Hội mua thêm phôi hoặc đi ải farm lại để kiếm món trùng.
   - *Cách giải quyết:* Đã bố trí nhiệm vụ phụ `q4_04` đi Thương Hội sắm phôi ngay trước nhiệm vụ `q4_06`, đồng thời lượng nguyên liệu cấp từ các ải Hồi 1–3 hoàn toàn đủ để nâng bản thứ hai lên Tam Trọng. Nếu muốn nới lỏng hơn nữa trong tương lai, có thể xem xét tặng trực tiếp một cặp vật phẩm cùng loại qua phần thưởng ải PvE trước đó.
2. **Mục tiêu `tam_phap_level` ở Hồi 3 và Hồi 5:**
   - *Hồi 3 (`q3_02` - Tâm Pháp tầng 3):* Yêu cầu nhân vật đạt Cấp 15 (mở khóa tính năng `tam_phap_3` theo bảng cấp độ nhân vật). Ở Hồi 3 ải 1–2, quái đã ở cấp 18–19 nên người chơi chắc chắn đã vượt cấp 15 từ trước.
   - *Hồi 5 (`q5_02` - Tâm Pháp tầng 6):* Yêu cầu nhân vật đạt Cấp 20 (mở khóa tính năng `tam_phap_6`). Ở Hồi 5 quái cấp 28–30, người chơi chắc chắn đã đủ điều kiện kích hoạt.
3. **Mục tiêu `equip_count` (Xếp số món vào Bao Phục):**
   - Lưới Bao Phục hiện tại có kích thước $5 \times 4 = 20$ ô.
   - Hồi 1 đòi 4 món (`q1_06`), Hồi 2 đòi 5 món (`q2_02`), Hồi 4 đòi 6 món (`q4_02`). Với kích thước đồ đan dược/kỳ trân chiếm 1 ô, binh khí 2–3 ô, hộ cụ 4 ô, việc xếp 5–6 món là hoàn toàn vừa vặn trong 20 ô mà không lo bị chật.
4. **Điểm đến `goto` của `q5_06` (`pve_win`):**
   - Do kiểu mục tiêu `pve_win` có thể thắng bất kỳ trận PvE nào, `goto` được gán về `stage:hoi_5_01` (đầu bản đồ Hồi 5) để người chơi dễ dàng bấm vào tham chiến ngay.
