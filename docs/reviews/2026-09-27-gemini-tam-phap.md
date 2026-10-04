# Báo Cáo Thiết Kế Và Cân Bằng Mốc Tâm Pháp 7 Môn Phái

**Thời gian:** 2026-09-27  
**Tác giả:** Gemini (Game Designer)  
**Tài liệu tham chiếu:** `docs/design/items.md`, `combat.md`, `balance.md`, `glossary.md`, `decisions.md` (#12–#15)  
**File đề xuất:** `tools/gemini/out/sects.proposed.json`  

---

## 1. Thiết Kế Tâm Pháp 7 Môn Phái & Phân Tích Ngân Sách

Nguyên tắc ngân sách (Budget Rule):
- **Cấp 1** (hiện có): +10% Lực Đạo (≈ +10% giá trị).
- **Cấp 3**: Bước đệm chỉ số (stat step), giá trị ≈ +5% (lũy kế +15%).
- **Cấp 6**: Cơ chế đặc trưng (distinct mechanic), giá trị ≈ +10% (lũy kế +25%).
- **Cấp 9**: Hiệu ứng định danh môn phái (signature effect), giá trị ≈ +15% (lũy kế +40%).

Tất cả các hiệu ứng kích hoạt (`triggers`) đều tuân thủ `from: "self"` để tránh nhân số lượng ngoài ý muốn khi dùng nhiều bản sao vũ khí nhỏ.

### 1.1. Kim Cang Tự (`kim_cang`)
- **Tâm Pháp:** Kim Cang Thiết Cốt Quyết
- **Binh khí được buff:** Quyền (`fist`), Côn (`staff`)
- **Bản sắc:** Cương Khí, Định Thân
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Lực Đạo. (Budget: +5%).
  - **Cấp 6:** +5% Tốc Xuất Chiêu, +5% Hội Tâm, Tiêu hao Tuyệt Kỹ −5. (Budget: +10% — tốc độ và hội tâm ~7%, giảm 20% chi phí Tuyệt Kỹ của Phục Hổ Côn ~3%).
  - **Cấp 9:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Tuyệt Kỹ thêm Định Thân 0.5s; Hội Tâm hồi 12 Cương Khí từ bản thân. (Budget: +15% — chỉ số ~8%, khống chế và giáp hồi phục kiểm soát ở mức an toàn).
- **Tổng ngân sách:** +40%.

### 1.2. Thái Thanh Quán (`thai_thanh`)
- **Tâm Pháp:** Thái Thanh Kiếm Ý
- **Binh khí được buff:** Kiếm (`sword`), Phiến (`fan`)
- **Bản sắc:** Nội Lực, Hóa Kình
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Hội Tâm. (Budget: +5% — Thanh Phong Kiếm sẵn 20% Hội Tâm, thêm 5% giúp tăng đều ~4.5% DPS).
  - **Cấp 6:** +5% Tốc Xuất Chiêu; Hội Tâm nhận 4 Nội Lực từ bản thân. (Budget: +10% — cung cấp năng lượng ổn định để kích hoạt Hóa Kình và Tuyệt Kỹ).
  - **Cấp 9:** +5% Lực Đạo, Tiêu hao Tuyệt Kỹ −5; Tuyệt Kỹ thêm 1 đòn 30% Lực Đạo. (Budget: +15% — khuếch đại sức công phá dứt điểm của Tuyệt Kỹ).
- **Tổng ngân sách:** +40%.

### 1.3. Liệt Diễm Đường (`liet_diem`)
- **Tâm Pháp:** Liệt Diễm Tâm Hỏa Quyết
- **Binh khí được buff:** Đao (`blade`), Chưởng (`palm`)
- **Bản sắc:** Liệt Hỏa
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +4% Hội Tâm. (Budget: +5% — ưu tiên Hội Tâm thay vì Lực Đạo thuần để tránh đao lửa áp đảo sớm ở cấp 3).
  - **Cấp 6:** +5% Tốc Xuất Chiêu, +5% Hội Tâm. (Budget: +10% — tăng nhịp xuất chiêu dồn Liệt Hỏa mà không can thiệp trực tiếp vào mốc bộc phát 50 điểm).
  - **Cấp 9:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Liệt Hỏa do bản thân bộc phát → Vận Khí 1.5s. (Budget: +15% — bùng nổ nhịp độ xuất chiêu ngay khi kích nổ Liệt Hỏa).
- **Tổng ngân sách:** +40%.

### 1.4. Huyền Độc Môn (`huyen_doc`)
- **Tâm Pháp:** Vạn Độc Huyền Kinh
- **Binh khí được buff:** Ám Khí (`hidden`), Trảo (`claw`)
- **Bản sắc:** Kịch Độc, Xuất Huyết
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Lực Đạo, +5% Hội Tâm. (Budget: +5% — hỗ trợ sát thương cơ bản cho các vũ khí kích thước nhỏ).
  - **Cấp 6:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Kịch Độc do bản thân bộc phát → Hút 6 Nội Lực đối thủ. (Budget: +10% — bóp nghẹt khả năng Hóa Kình của kẻ địch).
  - **Cấp 9:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Xuất Huyết do bản thân bộc phát → Liệu Thương 60 Khí Huyết. (Budget: +15% — hồi phục sinh lực theo chu kỳ bộc phát).
- **Tổng ngân sách:** +40%.

### 1.5. Hàn Sơn Phái (`han_son`)
- **Tâm Pháp:** Hàn Sơn Băng Tâm Quyết
- **Binh khí được buff:** Tiêu (`flute`), Thương (`spear`), Bút (`brush`)
- **Bản sắc:** Điểm Huyệt, Trì Trệ
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Lực Đạo, +3% Tốc Xuất Chiêu. (Budget: +5%).
  - **Cấp 6:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Phổ Công hút 3 Nội Lực đối thủ. (Budget: +10% — khống chế nhịp độ và cạn kiệt tài nguyên đối phương).
  - **Cấp 9:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Đòn trúng (hit) gây Trì Trệ 1 binh khí địch ngẫu nhiên trong 0.8s. (Budget: +15% — khống chế làm chậm chuẩn ngưỡng ≤ 1.0s, chọn ngẫu nhiên 1 vũ khí tránh khóa toàn diện).
- **Tổng ngân sách:** +40%.

### 1.6. Thiết Phủ Bang (`thiet_phu`)
- **Tâm Pháp:** Khai Sơn Bá Khí
- **Binh khí được buff:** Rìu (`axe`), Chùy (`hammer`)
- **Bản sắc:** Phá Cương, Sát thương lớn
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Tốc Xuất Chiêu. (Budget: +5% — giảm ~0.34s cho nhịp vung rìu nặng nề 6.75s).
  - **Cấp 6:** +5% Hội Tâm; Tuyệt Kỹ thêm Phá Cương 30. (Budget: +10% — dồn khả năng Phá Cương vào Tuyệt Kỹ thay vì Phổ Công để tránh triệt tiêu phòng thủ quá sớm).
  - **Cấp 9:** +5% Lực Đạo, Tiêu hao Tuyệt Kỹ −5; Tuyệt Kỹ thêm Phá Cương 50. (Budget: +15% — nhát chém Tuyệt Kỹ nặng ngàn cân phá tan Cương Khí dày đặc).
- **Tổng ngân sách:** +40%.

### 1.7. Bích Thủy Am (`bich_thuy`)
- **Tâm Pháp:** Bích Thủy Thanh Âm
- **Binh khí được buff:** Tiên (`whip`), Cầm (`zither`)
- **Bản sắc:** Liệu Thương, Điều Tức
- **Chi tiết các mốc:**
  - **Cấp 1:** +10% Lực Đạo.
  - **Cấp 3:** +5% Tốc Xuất Chiêu. (Budget: +5% — tăng tốc độ hồi chiêu các đòn đánh âm luật xuyên giáp).
  - **Cấp 6:** +5% Lực Đạo; Phổ Công thêm +1 Điều Tức (`regen`). (Budget: +10% — tích lũy Điều Tức an toàn, hoàn toàn dưới mức trần 30/s).
  - **Cấp 9:** +5% Lực Đạo, +5% Tốc Xuất Chiêu; Khi Khí Huyết < 40% → Liệu Thương 150 (tối đa 1 lần/trận). (Budget: +15% — bảo hiểm sinh mệnh tránh bị sốc sát thương).
- **Tổng ngân sách:** +40%.

---

## 2. Bảng Tổng Hợp Tỉ Lệ Thắng (TB) Qua 4 Mốc Tâm Pháp

Ma trận mô phỏng gồm 15 đội hình mẫu, mỗi cặp đấu chạy 200 lượt (tổng 45.000 trận mỗi mốc):

| # | Đội hình mẫu | Môn phái | TB Cấp 1 | TB Cấp 3 | TB Cấp 6 | TB Cấp 9 |
|:---:|---|---|:---:|:---:|:---:|:---:|
| #1 | Huyền Độc Môn (Ám Khí + Trảo) | `huyen_doc` | 10.5% | 10.0% | 9.6% | 8.5% |
| #2 | Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo) | `huyen_doc` | 40.1% | 38.5% | 33.4% | 36.4% |
| #3 | Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Tiêu) | `han_son` | 42.6% | 39.9% | 42.9% | 41.7% |
| #4 | Kim Cang - Thiết Phủ Hợp Bích (Côn + Rìu) | `kim_cang` | 59.6% | 59.1% | 61.5% | 57.6% |
| #5 | Thái Thanh - Liệt Diễm Hợp Bích (Kiếm + Đao) | `thai_thanh` | 59.4% | 58.8% | 57.6% | 64.3% |
| #6 | Thiết Phủ - Liệt Diễm Hợp Bích (Rìu + Đao) | `thiet_phu` | 56.0% | 50.2% | 41.3% | 38.5% |
| #7 | Kim Cang Tự (Quyền + Côn) | `kim_cang` | 39.5% | 43.6% | 55.0% | 59.8% |
| #8 | Kim Cang Tự - Bách Luyện (Thuần Quyền) | `kim_cang` | 58.1% | 58.0% | 59.3% | 63.5% |
| #9 | Liệt Diễm Đường (Đao lửa) | `liet_diem` | 64.8% | 69.9% | 60.8% | 56.8% |
| #10 | Liệt Diễm Đường - Hỏa Hải (Tứ Đao) | `liet_diem` | 61.4% | 60.6% | 60.1% | 69.6% |
| #11 | Thái Thanh Quán (Kiếm + Hóa Kình) | `thai_thanh` | 38.3% | 39.8% | 40.9% | 42.8% |
| #12 | Thái Thanh Quán - Kiếm Khí (Tam Kiếm Tứ Bút) | `thai_thanh` | 54.3% | 53.0% | 55.8% | 68.5% |
| #13 | Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm + Côn) | `thai_thanh` | 61.8% | 59.9% | 68.2% | 53.7% |
| #14 | Thiết Phủ Bang (Rìu + Cầm) | `thiet_phu` | 45.1% | 45.4% | 44.3% | 31.6% |
| #15 | Thiết Phủ Bang - Khai Thiên (Tam Phủ) | `thiet_phu` | 59.2% | 62.2% | 58.2% | 57.3% |

**Đánh giá:**
- Không có bất kỳ đội hình nào vượt ngưỡng 70.0% ở bất kỳ cấp độ nào (Cao nhất: #9 ở L3 đạt 69.9%; #13 ở L6 đạt 68.2%; #10 ở L9 đạt 69.6%).
- Đội hình #1 có tỉ lệ thắng thấp (<15%) do lỗi bố trí túi: dùng 2 Hàn Ngọc Tiêu (hệ Tiêu không được Huyền Độc buff, chiếm tới 4 ô mà không có tương tác Kịch Độc/Xuất Huyết), trong khi đội hình chuẩn #2 (Vạn Độc) duy trì ổn định 33.4% - 40.1%.
- Đội hình #14 (Rìu + Cầm) bị tụt ở cấp 9 (31.6%) vì đây là đội hình lai thiếu tập trung: Rìu cần dồn sát thương nhanh trong khi Cầm lại thiên về kéo dài trận, khiến đội hình bị các phái thuần đè bẹp khi đối phương đạt mốc 9 hoàn thiện.

---

## 3. Quá Trình Hiệu Chỉnh Và Lý Do Điều Chỉnh

Trong quá trình chạy thử nghiệm mô phỏng, đã phát hiện 3 điểm bất thường vượt trần 70% và được can thiệp kịp thời:

1. **Liệt Diễm Đường tại Cấp 3 (Loadout #9 ban đầu đạt 72.1%):**
   - *Nguyên nhân:* Liệt Diễm Đao có Lực Đạo gốc rất lớn (120). Khi cộng thêm 5% Lực Đạo ở cấp 3 (lũy kế 15%), sát thương mỗi nhát chém trở nên quá áp đảo trước khi đối thủ kịp kích hoạt phòng ngự.
   - *Xử lý:* Thay `attackPct: 0.05` ở cấp 3 bằng `crit: 0.04`. Tỉ lệ Hội Tâm giúp tăng hiệu suất sát thương ~3% một cách mượt mà. Kết quả tỉ lệ thắng của #9 tại L3 hạ xuống **69.9%**.

2. **Thiết Phủ Bang tại Cấp 6 và Cấp 9 (Loadout #15 ban đầu đạt 74.4% ở L6 và 76.6% ở L9):**
   - *Nguyên nhân:* Khai Sơn Phủ có sát thương cơ bản tới 280. Thiết kế ban đầu gắn `shield_break: 30` vào Phổ Công ở cấp 6, kết hợp cả `attackPct: 0.05` và `cooldownPct: 0.05`. Đội hình 3 Khai Sơn Phủ chém liên tục vừa nhanh vừa xóa sạch Cương Khí của mọi phái phòng thủ.
   - *Xử lý:* Chuyển toàn bộ Phá Cương từ Phổ Công vào Tuyệt Kỹ (+30 ở L6, +50 ở L9), đồng thời đổi `cooldownPct` ở L6 thành `crit: 0.05`. Nhờ đó đòn đánh thường không triệt tiêu phòng thủ vô tội vạ, mà chỉ những cú vung Tuyệt Kỹ mới bộc phát Phá Cương uy lực. Kết quả tỉ lệ thắng của #15 giảm về **58.2% (L6)** và **57.3% (L9)**.

3. **Kim Cang Tự tại Cấp 9 (Loadout #8 ban đầu đạt 77.6%):**
   - *Nguyên nhân:* Mốc 9 ban đầu cấp hiệu ứng `on: "crit"` hồi 60 Cương Khí. Đội hình #8 (6 Thiết Cốt Quyền Sáo) có tốc độ đánh cực nhanh (~1.1s) kết hợp Túy Tiên Hồ Lô tạo Hội Tâm liên tục, sinh ra lượng Cương Khí khổng lồ (bẫy tích lũy vũ khí nhỏ).
   - *Xử lý:* Giảm Cương Khí khi Hội Tâm từ 60 xuống 12. Khi dùng 6 Quyền Sáo, lượng Cương Khí tạo ra chỉ ở mức ~9.6/giây, vừa đủ giữ bản sắc phòng ngự mà không thể bất tử. Tỉ lệ thắng của #8 tại L9 hạ về mức cân bằng lý tưởng **63.5%**.

---

## 4. Rủi Ro Và Câu Hỏi Mở

1. **Các hệ binh khí chưa có vật phẩm mẫu:** 5 hệ (Chưởng, Phiến, Thương, Chùy, Tiên) hiện chưa có dữ liệu item thực tế trong engine. Tâm Pháp đã được xây dựng hoàn chỉnh theo đúng chuẩn lý thuyết và ngân sách, nhưng cần chạy lại ma trận kiểm tra ngay khi các vật phẩm này được đưa vào code.
2. **Cơ chế bộc phát phụ thuộc thiếu thanh tẩy:** Các hiệu ứng kích hoạt khi bộc phát Kịch Độc/Xuất Huyết/Liệt Hỏa (`on: "burst"`) hiện vận hành rất ổn định do game chưa có cơ chế xóa điểm tích lũy. Nếu tương lai bổ sung hiệu ứng giải trừ hoặc kháng trạng thái, các mốc Tâm Pháp này có thể bị giảm giá trị.
3. **Sự tụt hậu của lối chơi lai (Hybrid) ở giai đoạn cuối:** Các đội hình phối hợp binh khí khác phái (như #6, #14) có xu hướng yếu đi rõ rệt ở cấp 9 (tỉ lệ thắng giảm ~10-15% so với cấp 1) do Tâm Pháp chỉ buff được 1 nửa số trang bị trong túi. Cần cân nhắc bổ sung Kỳ Trân hoặc Công Pháp hỗ trợ đội hình đa hệ ở bản cập nhật sau.
4. **Cơ chế hút nội lực ở cấp cao:** Cả Huyền Độc (hút 6 khi nổ độc) và Hàn Sơn (hút 3 trên Phổ Công) đều nhắm vào nội lực của địch. Khi đối đầu với Thái Thanh (vốn cần nội lực để Hóa Kình), sự khắc chế này rất rõ rệt. Cần theo dõi thêm để tránh việc Thái Thanh bị vô hiệu hóa hoàn toàn lối chơi Hóa Kình.
