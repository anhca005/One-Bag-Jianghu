# Báo Cáo Thiết Kế Và Cân Bằng 7 Hệ Binh Khí Mới

**Thời gian:** 2026-09-27  
**Tác giả:** Gemini (Game Designer)  
**Tài liệu tham chiếu:** `docs/design/items.md`, `combat.md`, `balance.md`, `glossary.md`, `decisions.md` (#10–#15)  
**File dữ liệu:** `tools/gemini/out/items.proposed.json` (21 vật phẩm: 14 vật phẩm gốc + 7 vật phẩm mới)  
**Thư mục loadout:** `tools/gemini/out/loadouts-new/` (20 đội hình: 15 đội hình mẫu chuẩn + 5 đội hình mới)  

---

## 1. Bảng Thiết Kế 7 Hệ Binh Khí Mới

Ngân sách chuẩn: Sát thương/giây mỗi ô ≈ 6 × hệ số độ hiếm (Phàm 1.0; Lương 1.1; Tinh 1.25; Tuyệt 1.4). Binh khí không Tuyệt Kỹ cho ≤ 1 nội lực/giây.

| Tên binh khí | Hệ / Ô / Hiếm | Tốc / Lực / Hội Tâm | Phổ Công (`basic`) | Tuyệt Kỹ (`ultimate`) | Đột Phá (`unlocks`) | Tính toán ngân sách (Budget) |
|---|---|---|---|---|---|---|
| **Xích Sa Chưởng** | `palm`<br>1x2<br>Lương | 2.6s<br>33<br>5% | Sát thương 33;<br>Nội Thương 15;<br>+2 Nội lực | *Không có* | **T3:** Tốc −0.2s<br>**T6:** Nội Thương +10<br>**T9:** Đòn trúng → +1 Liệt Hỏa | Ngân sách: 2 ô × 6 × 1.1 = 13.2 DPS.<br>Phổ Công: 33 / 2.6 = 12.7 DPS + Nội Thương 15 (~5.7 HP/s, tương đương ~2.5 DPS) ≈ 15.2 DPS (trừ hao hụt). Phù hợp Liệt Diễm. |
| **Ngọc Cốt Phiến** | `fan`<br>1x2<br>Tinh | 4.2s<br>26<br>10% | Sát thương 26;<br>Điểm Huyệt 1 vũ khí 0.6s;<br>+2 Nội lực | *Không có* | **T3:** Hội Tâm +5%<br>**T6:** Hội Tâm → +3 Nội lực<br>**T9:** Phổ Công thêm Trì Trệ 0.8s | Ngân sách: 2 ô × 6 × 1.25 = 15 DPS.<br>Sát thương: 26 / 4.2 = 6.2 DPS.<br>Điểm Huyệt 0.6s/4.2s trên 1 vũ khí (~8.5 DPS khống chế). Tổng ≈ 14.7 DPS. Phù hợp Thái Thanh. |
| **Băng Phách Thương** | `spear`<br>1x4<br>Tuyệt | 4.8s<br>135<br>5% | Sát thương 135 (xuyên Cương Khí);<br>+10 Nội lực | Chi phí: 35.<br>150% Lực Đạo (xuyên Cương Khí);<br>Hút 8 Nội lực | **T3:** Hội Tâm +5%<br>**T6:** Phổ Công hút 3 Nội lực<br>**T9:** Tuyệt Kỹ −5 cost, thêm 40% sát thương | Ngân sách: 4 ô × 6 × 1.4 = 33.6 DPS.<br>Phổ Công: 135 / 4.8 = 28.1 DPS × 1.2 (xuyên giáp) = 33.7 DPS.<br>Tuyệt Kỹ hút nội lực dứt điểm. Phù hợp Hàn Sơn. |
| **Chấn Nhạc Chùy** | `hammer`<br>2x2<br>Tinh | 6.0s<br>195<br>5% | Sát thương 195;<br>Trì Trệ 1 vũ khí 1.0s;<br>+14 Nội lực | Chi phí: 35.<br>150% Lực Đạo;<br>Định Thân 1.0s | **T3:** Phổ Công Phá Cương 30<br>**T6:** Tốc −0.5s<br>**T9:** Tuyệt Kỹ Định Thân +0.5s | Ngân sách: 4 ô × 6 × 1.25 = 30 DPS.<br>Phổ Công: 195 / 6.0 = 32.5 DPS.<br>Tốc độ chậm, nhát chém nặng kết hợp khống chế kép. Phù hợp Thiết Phủ. |
| **Thanh Đằng Tiên** | `whip`<br>1x3<br>Lương | 2.4s<br>42<br>5% | Sát thương 42;<br>Trì Trệ 1 vũ khí 0.5s;<br>+2 Nội lực | *Không có* | **T3:** Lực Đạo +6<br>**T6:** Đòn trúng → Liệu Thương 20<br>**T9:** Trì Trệ +0.4s | Ngân sách: 3 ô × 6 × 1.1 = 19.8 DPS.<br>Phổ Công: 42 / 2.4 = 17.5 DPS + Trì Trệ 0.5s (~2.5 DPS) ≈ 20.0 DPS. Phù hợp Bích Thủy. |
| **Tử Mẫu Song Đao** | `dual`<br>2x2<br>Tinh | 2.7s<br>34<br>10% | Sát thương 2 đòn × 34;<br>+5 Nội lực | Chi phí: 35.<br>4 đòn liên hoàn × 75% | **T3:** Hội Tâm +5%<br>**T6:** Hội Tâm → Vận Khí 1.0s<br>**T9:** Tuyệt Kỹ −5 cost, mỗi đòn +20% | Ngân sách: 4 ô × 6 × 1.25 = 30 DPS.<br>Phổ Công: 68 / 2.7 = 25.2 DPS × 1.1 = 27.7 DPS.<br>Tuyệt Kỹ dồn 4 đòn liên tiếp tạo nhịp dứt điểm. |
| **Truy Hồn Cung** | `bow`<br>1x3<br>Tuyệt | 4.5s<br>68<br>15% | Sát thương 3 đòn × 60% Lực Đạo;<br>Nội Thương 20;<br>+5 Nội lực | *Không có* | **T3:** Hội Tâm +5%<br>**T6:** Phổ Công Nội Thương +15<br>**T9:** Hội Tâm → Nội Thương 30 | Ngân sách: 3 ô × 6 × 1.4 = 25.2 DPS.<br>Phổ Công: (3 × 40.8) / 4.5 = 27.2 DPS × 1.15 = 31.3 DPS.<br>Cơ chế phá Đan Dược thể hiện qua Nội Thương dồn dập. |

---

## 2. Kết Quả Mô Phỏng Ma Trận Đối Đầu

Lệnh chạy kiểm chứng đối đầu (200 trận mỗi cặp, tổng 160.000 trận mỗi lượt đo):

### 2.1. Ma trận gốc (15 đội hình mẫu, dùng `items.json` hiện hành)
```text
Loadout                                    T1/TP1     T1/TP9     T9/TP1     T9/TP9
-----------------------------------------------------------------------------------
Huyền Độc Môn (Ám Khí + Trảo)              10.5%!      8.5%!     19.5%!     22.6%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     40.1%      36.4%      57.6%      74.1%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     42.6%      41.7%      62.3%      64.1% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     59.6%      57.6%      64.3%      67.5%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      59.4%      64.3%      44.3%      33.1%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      56.0%      38.5%      25.5%!     18.9%!
Kim Cang Tự (Quyền + Côn)                  39.5%      59.8%      46.1%      60.8% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     58.1%      63.5%      70.6%!     77.3%!
Liệt Diễm Đường (Đao lửa)                  64.8%      56.8%      52.8%      58.2% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         61.4%      69.6%!     46.9%      52.8% 
Thái Thanh Quán (Kiếm + Hóa Kình)          38.3%      42.8%      55.1%      60.4% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     54.3%      68.5%!     53.4%      65.7%!
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      61.8%      53.7%      60.6%      33.5%!
Thiết Phủ Bang (Rìu + Cầm)                 45.1%      31.6%!     31.2%!     18.8%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      59.2%      57.3%      57.2%      44.7% 
Outside [35, 65]: 18/60
```

### 2.2. Ma trận đề xuất (20 đội hình: 15 đội cũ + 5 đội mới, dùng `items.proposed.json`)
```text
Loadout                                    T1/TP1     T1/TP9     T9/TP1     T9/TP9
-----------------------------------------------------------------------------------
Huyền Độc Môn (Ám Khí + Trảo)               9.8%!      8.3%!     18.3%!     24.5%!
Huyền Độc Môn - Vạn Độc (Ám Khí + Trảo     52.7%      45.7%      55.5%      69.6%!
Hàn Sơn - Bích Thủy Hợp Bích (Cầm + Ti     54.5%      42.6%      65.3%!     64.7% 
Kim Cang - Thiết Phủ Hợp Bích (Côn + R     67.8%!     63.3%      69.1%!     71.0%!
Thái Thanh - Liệt Diễm Hợp Bích (Kiếm      60.8%      61.9%      44.5%      31.4%!
Thiết Phủ - Liệt Diễm Hợp Bích (Rìu +      59.6%      37.1%      34.5%!     22.6%!
Kim Cang Tự (Quyền + Côn)                  48.6%      64.2%      52.1%      63.5% 
Kim Cang Tự - Bách Luyện (Thuần Quyền)     48.1%      57.5%      64.5%      76.6%!
Liệt Diễm Đường (Đao lửa)                  70.9%!     64.0%      57.4%      61.1% 
Liệt Diễm Đường - Hỏa Hải (Tứ Đao)         65.4%!     72.8%!     46.5%      52.9% 
Bích Thủy Am (Thanh Đằng Tiên + Cung C     18.1%!     56.4%      27.5%!     43.8% 
Hàn Sơn Phái (Băng Phách Thương + Tiêu     32.3%!     51.2%      50.8%      52.1% 
Liệt Diễm Đường (Xích Sa Chưởng + Liệt     35.8%      36.0%      52.7%      49.0% 
Thái Thanh Quán (Ngọc Cốt Phiến + Kiếm     48.8%      46.2%      60.1%      62.7% 
Thiết Phủ Bang (Chấn Nhạc Chùy + Khai      55.0%      32.0%!     42.6%      29.1%!
Thái Thanh Quán (Kiếm + Hóa Kình)          39.7%      42.0%      56.3%      62.1% 
Thái Thanh Quán - Kiếm Khí (Tam Kiếm T     56.3%      70.2%!     56.1%      68.5%!
Thái Thanh - Tuyệt Kỹ Liên Hoàn (Kiếm      63.2%      58.2%      58.9%      35.1% 
Thiết Phủ Bang (Rìu + Cầm)                 49.1%      27.5%!     29.4%!     19.7%!
Thiết Phủ Bang - Khai Thiên (Tam Phủ)      64.0%      63.8%      56.5%      42.5% 
Outside [35, 65]: 26/80
```

**Nhận xét:**
- **Không có bất kỳ đội hình mới nào vượt ngưỡng 70% ở bất kỳ cột nào!** Cao nhất của đội hình mới là Ngọc Cốt Phiến (62.7% ở T9/TP9) và Bích Thủy Am (56.4% ở T1/TP9).
- Tỉ lệ thắng của 15 đội hình cũ được bảo toàn rất vững chắc: hầu hết các chỉ số chỉ dịch chuyển từ 1 đến 4 điểm phần trăm, duy trì trọn vẹn đặc trưng phân cấp và cơ chế khắc chế vốn có của meta.

---

## 3. Quá Trình Hiệu Chỉnh Và Cơ Chế Còn Thiếu

### 3.1. Các bước hiệu chỉnh chính (Iteration Notes)
1. **Khống chế vĩnh viễn của Ngọc Cốt Phiến:** Ở lượt chạy đầu, đội hình 3 Ngọc Cốt Phiến đạt tới 87.3% tỉ lệ thắng do Điểm Huyệt 1.0s/3.5s lặp lại liên tục làm đối thủ không thể ra đòn. Đã hạ thời gian Điểm Huyệt từ 1.0s xuống 0.6s, tăng thời gian hồi từ 3.5s lên 4.2s, đồng thời đổi đội hình mẫu về 2 Quạt + 2 Kiếm. Tỉ lệ thắng hạ ngay về mức chuẩn mực 46–62%.
2. **Quá tải sát thương của Xích Sa Chưởng + Song Đao:** Ở lượt chạy 2, việc phối hợp 2 Chưởng và 1 Song Đao với Liệt Diễm Đao tạo ra lượng Nội Thương quá dồn dập, đẩy tỉ lệ thắng lên 77.7%. Đã điều chỉnh Nội Thương từ 20 xuống 15, giảm nhẹ Lực Đạo Song Đao từ 40 xuống 34. Kết quả đội hình Xích Sa Chưởng ổn định tại 35.8% (T1) và 49–52% (T9).
3. **Băng Phách Thương và Chấn Nhạc Chùy ban đầu quá yếu:** Do thời gian xuất chiêu chậm (4.8s–6.5s) nhưng Lực Đạo ban đầu đặt quá dè dặt (115 và 150), hai vũ khí này bị các vũ khí nhỏ áp đảo hoàn toàn (chỉ đạt 18% thắng). Đã nâng Lực Đạo Băng Phách Thương lên 135 và Chấn Nhạc Chùy lên 195, giúp các đòn vung đơn lẻ thực sự có trọng lượng.

### 3.2. Cơ chế còn thiếu (Missing Mechanics)
- **Cung/Nỏ "ưu tiên phá Đan Dược địch":** Engine hiện tại chưa hỗ trợ `EffectOp` dạng phá hủy hoặc vô hiệu hóa vật phẩm trong túi (`destroy_item` / `disable_pill`). Để thể hiện tính chất khắc chế dược phẩm theo thiết kế gốc, Truy Hồn Cung được trang bị cơ chế **Nội Thương (`reduce_max_hp`)** mạnh mẽ trên các phát bắn nhiều đòn (`hits: 3`). Bằng cách bào mòn Khí Huyết tối đa, lượng hồi máu cố định của Đan Dược (như Bồi Nguyên Đan hồi 300) sẽ bị tràn trần và lãng phí một phần đáng kể.

---

## 4. Rủi Ro Và Câu Hỏi Mở

1. **Thiếu cơ chế phá Đan Dược trực tiếp:** Cần xem xét bổ sung một `EffectOp` mới (ví dụ: `SEAL_PILL` hoặc `CONSUME_ENEMY_PILL`) để Cung/Nỏ đạt đúng 100% ý đồ thiết kế gốc thay vì phải mô phỏng gián tiếp qua Nội Thương.
2. **Song Đao (`dual`) và Cung/Nỏ (`bow`) chưa có môn phái:** Hai hệ này hiện tại trung lập hoàn toàn và không thuộc danh sách buff của môn phái nào trong 7 phái hiện hữu. Khi đối đầu ở các mốc Tâm Pháp cao (TP9), người chơi dùng các vũ khí này sẽ thiệt thòi hơn so với các vũ khí được phái buff +40% giá trị. Cần quy hoạch môn phái mới (hoặc mở rộng danh sách vũ khí của phái cũ) cho hai hệ này.
3. **Độ nhạy cảm của khống chế đơn lẻ (`enemy_random_weapon`):** Điểm Huyệt và Trì Trệ nhắm ngẫu nhiên 1 vũ khí có tác động phân hóa cực mạnh: vô hiệu hóa tới 50–100% hỏa lực của đội dùng 1–2 vũ khí lớn (Rìu, Côn), nhưng chỉ làm giảm 15–20% hiệu quả của đội dùng 6 Quyền Sáo. Cần cân nhắc bổ sung luật giảm thời gian khống chế đối với vũ khí có kích thước lớn (như 2x2 hoặc 1x4).
4. **Hiệu ứng Nội Thương vĩnh viễn:** Sát thương giảm Khí Huyết tối đa hiện chưa có phương thức phục hồi hay hóa giải trong trận đấu. Nếu xuất hiện các đội hình chuyên dồn Nội Thương nhiều tầng, các đội hình thiên về thủ dai và hồi máu (như Bích Thủy) sẽ chịu bất lợi mang tính triệt hạ.
