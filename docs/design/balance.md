# Cân bằng

Quy tắc dùng để đặt chỉ số vật phẩm, cách đo, và kết quả các đợt cân bằng. Số liệu hiện hành luôn nằm trong `server/battle-engine/src/main/resources/data/items.json`.

## Quy tắc 1: ngân sách sức mạnh theo ô

**Sát thương/giây mỗi ô ≈ 6** ở chỉ số gốc Phàm (từ #24/#25, `items.json` chỉ chứa gốc Phàm; phẩm của từng bản nhân thêm 1.00 → 1.34, quyết định #30). Bảng dưới là quy tắc cũ khi mỗi món có phẩm cố định, giữ để tham khảo:

| Độ hiếm | Phàm | Lương | Tinh | Tuyệt | Thần Binh | Chí Tôn |
|---|---|---|---|---|---|---|
| Hệ số | 1.0 | 1.1 | 1.25 | 1.4 | 1.6 | 1.8 |

Ví dụ: Tinh Phẩm 1x3 → khoảng 22 sát thương/giây. Phần hiệu ứng phụ được **trừ vào ngân sách** theo quy đổi gần đúng:

| Thành phần | Quy đổi |
|---|---|
| Hội Tâm | × (1 + tỉ lệ) |
| Tuyệt Kỹ | cộng thêm (hệ số Tuyệt Kỹ − 1) × tần suất tung được |
| `ignoreShield` | × 1.2 |
| 1 điểm Liệt Hỏa | 300 / 50 = 6 sát thương (Cương Khí đỡ được) |
| 1 điểm Kịch Độc | 240 / 50 = 4.8, × 1.2 vì xuyên Cương Khí ≈ 5.8 |
| 1 điểm Xuất Huyết | (150 + 10% khí huyết tối đa) / 50 ≈ 5.4 với 1200 máu, × 1.2 ≈ 6.5 |
| Hao hụt khi tích điểm | trừ khoảng **25%**: điểm chưa đủ 50 khi trận kết thúc coi như mất |
| Khống chế (Định Thân, Điểm Huyệt, Trì Trệ) | đắt, tính theo % thời gian khóa được đối thủ |

Món nhỏ và món lớn phải **cùng mức theo ô**. Trước đợt 1, Quyền Sáo 1 ô mạnh gấp khoảng 4 lần Rìu 4 ô nếu tính theo ô. Từ đợt 2, binh khí tối thiểu 2 ô.

## Quy tắc 2: kinh tế Nội Lực

- Nội lực dùng cho hai việc: tung Tuyệt Kỹ và Hóa Kình. Đội hình **không có Tuyệt Kỹ sẽ tích nội lực để Hóa Kình miễn phí**. Vì vậy:
  - **Binh khí không có Tuyệt Kỹ chỉ cho ≤ 1 nội lực/giây** (khoảng 1–4 mỗi đòn).
  - Binh khí có Tuyệt Kỹ cho khoảng 2–4 nội lực/giây.
- Muốn một môn phái chuyên Hóa Kình thì dùng hiệu ứng `parry_discount` (Thái Thanh Tâm Kinh), kèm Tuyệt Kỹ có chi phí ≥ chi phí Hóa Kình. Nhờ vậy, mỗi khi đủ nội lực để tung Tuyệt Kỹ thì cũng đủ để Hóa Kình.

## Quy tắc 3: khống chế
- Khống chế nhắm vào **toàn bộ** đồ địch (Trì Trệ của Hàn Ngọc Tiêu) chỉ nên kéo dài ≤ 1 giây mỗi lần.
- Khống chế nhắm **ngẫu nhiên 1 món** (Điểm Huyệt) tự nhiên khắc chế đội ít món. Đó là ý đồ thiết kế, nhưng phải giới hạn: 1 giây mỗi lần với chu kỳ 4 giây.

## Cách đo

```bash
cd server
./mvnw -q -pl battle-engine exec:java -Dexec.args="--matrix battle-engine/samples --runs 1000"   # bảng đối đầu
./mvnw -q -pl battle-engine exec:java -Dexec.args="battle-engine/samples/A.json battle-engine/samples/B.json --runs 500"   # 1 cặp + nguồn sát thương
```

Chế độ `--runs` in ra **nguồn sát thương** của từng bên: mỗi món, Liệt Hỏa, Kịch Độc, Xuất Huyết, cùng hồi máu, Cương Khí và Kiệt Lực. Nên xem phần này trước khi đổi số.

Đội hình mẫu dùng khoảng 16–19/20 ô để so sánh công bằng.

**Mục tiêu:** tỉ lệ thắng trung bình của mỗi đội nằm trong khoảng 40–60%, trận kéo dài 20–35 giây.

## Đợt 1 · 2026-09-27

Kết quả cuối (1000 trận mỗi cặp, tỉ lệ thắng của hàng trước cột):

| | Huyền Độc | Kim Cang | Liệt Diễm | Thái Thanh | Thiết Phủ | **TB** |
|---|---|---|---|---|---|---|
| Huyền Độc Môn | - | 68.5% | 11.5% | 90.7% | 0.2% | **42.7%** |
| Kim Cang Tự | 29.9% | - | 84.0% | 24.2% | 48.1% | **46.6%** |
| Liệt Diễm Đường | 90.4% | 16.9% | - | 45.2% | 62.4% | **53.7%** |
| Thái Thanh Quán | 9.4% | 76.0% | 51.7% | - | 59.8% | **49.2%** |
| Thiết Phủ Bang | 99.7% | 51.8% | 36.8% | 40.6% | - | **57.2%** |

Trước đợt 1 thì từ 6% đến 100%.

**Thế khắc chế đã hình thành:** Thái Thanh (Hóa Kình) khắc các đội dựa vào Tuyệt Kỹ, nhưng thua Huyền Độc (không có Tuyệt Kỹ để bị Hóa Kình). Liệt Diễm (Liệt Hỏa đi qua Cương Khí trước) khắc Huyền Độc.

### Thay đổi luật (ghi ở decisions #9)
- Xuất Huyết mất một nửa số tầng sau mỗi lần bộc phát (trước đây cộng dồn mãi, tăng theo cấp số nhân). *Đã thay bằng cơ chế tích điểm ở đợt 2.*
- Hóa Kình gây Định Thân 1 giây (trước là 2).
- Thêm hiệu ứng `parry_discount`, chi phí Hóa Kình tối thiểu 20.
- Bên đi trước mỗi tick chọn bằng random có seed. Cách đổi theo tick chẵn/lẻ trước đây là **lỗi**, khiến bên A luôn thắng khi hai bên ra đòn cùng lúc.

### Thay đổi chỉ số

| Vật phẩm | Trước → Sau |
|---|---|
| Thiết Cốt Quyền Sáo | Lực Đạo 30 → 8, nội lực 8 → 2 |
| Phục Hổ Côn | Lực Đạo 90 → 130, 6s → 5.5s; Tuyệt Kỹ Định Thân 1.5s → 1s |
| Thanh Phong Kiếm | Lực Đạo 70 → 65, nội lực 14 → 12; Tuyệt Kỹ 30 → 40 nội lực, 3×0.8 → 3×0.9 |
| Liệt Diễm Đao | Lực Đạo 140 → 135; Liệt Hỏa 4 → 3; Tuyệt Kỹ 2.0x + 25 tầng → 1.6x + 15 tầng |
| Thất Tinh Tiêu | Lực Đạo 10 → 3, Kịch Độc 3 → 1, nội lực 4 → 1 |
| Tử Kim Trảo | Lực Đạo 35 → 10, 1.8s → 1.6s, nội lực 6 → 1 |
| Khai Sơn Phủ | Lực Đạo 180 → 240, 9s → 8s, nội lực 15 → 20; Tuyệt Kỹ Phá Cương 120 → 150, 3 nhát → 2 |
| Phán Quan Bút | Lực Đạo 40 → 44; Điểm Huyệt 1.5s → 1s; nội lực 8 → 4 |
| Hàn Ngọc Tiêu | Lực Đạo 50 → 38, 7s → 6s; Trì Trệ toàn bộ 2s → 1s; nội lực 10 → 4 |
| Bích Thủy Cầm | Lực Đạo 60 (×0.6) → 120 (×1.0), Điều Tức 3 → 6, nội lực 10 → 4 |
| Thái Thanh Tâm Kinh | thêm Hóa Kình −20 nội lực |

## Đợt 2 · 2026-09-27

Làm theo 3 yêu cầu thiết kế (decisions #10):
- Hai bên đánh đồng thời thật sự: đòn ra cùng tick đều trúng, chỉ xét tử trận cuối tick.
- Binh khí tối thiểu 2 ô.
- Liệt Hỏa / Kịch Độc / Xuất Huyết đổi sang **tích điểm → bộc phát ở 50 → reset**.

Kết quả cuối (1000 trận mỗi cặp):

| | Huyền Độc | Kim Cang | Liệt Diễm | Thái Thanh | Thiết Phủ | **TB** |
|---|---|---|---|---|---|---|
| Huyền Độc Môn | - | 67.8% | 5.3% | 94.6% | 0.0% | **41.9%** |
| Kim Cang Tự | 34.0% | - | 65.7% | 22.8% | 43.8% | **41.6%** |
| Liệt Diễm Đường | 95.1% | 34.5% | - | 32.1% | 60.4% | **55.5%** |
| Thái Thanh Quán | 5.8% | 78.9% | 64.2% | - | 56.0% | **51.2%** |
| Thiết Phủ Bang | 100.0% | 56.3% | 41.0% | 43.1% | - | **60.1%** |

Ngay sau khi đổi luật (chưa chỉnh số): từ 0.6% đến 82.7%.

### Thay đổi chỉ số

| Vật phẩm | Đợt 1 → Đợt 2 |
|---|---|
| Thiết Cốt Quyền Sáo | 1x1 → **1x2**, Lực Đạo 8 → 16 |
| Tử Kim Trảo | 1x1 → **1x2**, Lực Đạo 10 → 12, Xuất Huyết 2 → 3 điểm |
| Thất Tinh Tiêu | 1x1 → **1x2**, Lực Đạo 3 → 4, Kịch Độc 1 → 6 điểm |
| Liệt Diễm Đao | Lực Đạo 135 → 105, nội lực 16 → 12 |
| Phục Hổ Côn | Lực Đạo 130 → 160, Tuyệt Kỹ 36 → 30 nội lực |
| Khai Sơn Phủ | nội lực 20 → 15; Tuyệt Kỹ 2 nhát × 100% → 1 nhát 160% |

Đội hình mẫu được xếp lại cho vừa binh khí 1x2 (vẫn 18–19/20 ô).

## Đột Phá · 2026-09-27

Chạy 5 đội hình mẫu, cả hai bên cùng Trọng, 200 trận mỗi cặp. Cột là tỉ lệ thắng trung bình của từng đội.

| Trọng | Huyền Độc | Kim Cang | Liệt Diễm | Thái Thanh | Thiết Phủ |
|---|---|---|---|---|---|
| 1 (chưa có Đột Phá) | 40% | 40% | 56% | 52% | 63% |
| 3 | 28% | 71% | 47% | 56% | 48% |
| 6 | 57% | 44% | 30% | 53% | 66% |
| 9 | 46% | 60% | 33% | 64% | 48% |

Bài học rút ra khi chỉnh:
- **Bản đầu** cho Huyền Độc Môn thắng 94% ở Trọng 9. Có hai nguyên nhân:
  - Kích hoạt "khi bộc phát" chạy trên mọi bản sao của món trong túi → sửa bằng `from: self`.
  - Tiêu được +2 điểm Kịch Độc, làm số đòn cần để bộc phát giảm từ 5 xuống 4 → đổi sang giảm Tốc Xuất Chiêu.
- **Đã thử giữ điểm dư sau bộc phát:** không mượt hơn mà chỉ làm trạng thái mạnh lên, nên bỏ.
- Vòng sau giảm Côn (Cương Khí Trọng 3: 40 → 20) và Quyền Sáo (đòn phụ Trọng 9: 40% → 25%), tăng nhẹ Trảo.
- **Chưa đạt mục tiêu 35–65%:** Kim Cang ở Trọng 3 (71%, phần lớn do hệ số Trọng: không có Đột Phá vẫn 63%), Huyền Độc ở Trọng 3 (28%), Liệt Diễm ở Trọng 6/9 (30–33%). Dừng chỉnh tay ở đây, vì với 5 đội hình mẫu mỗi lần sửa một món lại làm đội khác lệch. Cần thêm đội hình mẫu hoặc công cụ dò tự động. Trong game thật hai bên thường khác Trọng, nên bảng này chỉ để phát hiện món quá mạnh.

## Đợt 3 · 2026-09-27

**Dữ liệu:** 15 đội hình mẫu (5 gốc + 10 do Gemini tạo, báo cáo ở [reviews/2026-09-27-gemini-loadouts.md](../reviews/2026-09-27-gemini-loadouts.md)). Trước đợt 3: Cầm + Tiêu 99%, 6 Quyền Sáo 77%, Vạn Độc 77%.

**Đổi luật:** Điều Tức tối đa 30/giây (`BattleConfig.maxRegen`). Trước đó cộng dồn mãi, đội có 2 Cầm hồi trung bình 812 máu mỗi trận.

**Cách làm:** chỉnh tay 4 vòng không hội tụ (sửa một món thì đội khác lệch), nên chuyển sang **bộ dò tự động**:
- Mỗi lần nhích một trong 18 thông số (Lực Đạo, Tốc Xuất Chiêu, chi phí Tuyệt Kỹ, nội lực, Điều Tức, Tâm Kinh), giữ thay đổi nếu tốt hơn.
- Mục tiêu đo trên bảng 15×15 ở cả Trọng 1, 3, 6, 9: phạt nặng đội vượt 62% (build áp đảo), phạt nhẹ đội dưới 38% (người chơi có thể tự xếp ra túi dở).
- Mỗi lần đo khoảng 3 giây; chạy tới khi không còn cải thiện.

**Kết quả** (tỉ lệ thắng trung bình trong bảng 15×15):

| Trọng | Thấp nhất | Cao nhất | Đội cao nhất |
|---|---|---|---|
| 1 | 12% (Huyền Độc gốc) | 64% | Kim Cang + Thiết Phủ |
| 3 | 11% | 69% | Thái Thanh Tuyệt Kỹ |
| 6 | 22% | 69% | Vạn Độc |
| 9 | 22% | 72% | 6 Quyền Sáo |

Không còn build áp đảo. Đội yếu nhất là đội Huyền Độc gốc (2 Tiêu + 2 Trảo + 2 Hàn Ngọc Tiêu): độc và huyết đều không đủ nhanh, coi như một cách xếp túi dở.

**Thay đổi chỉ số chính:** Quyền Sáo Lực Đạo 16 → 11; Trảo 12 → 8; Tiêu 2.0s → 2.1s; Kiếm 65 → 75; Đao 105 → 120, 6s → 4.5s; Côn 160 → 200, Tuyệt Kỹ 30 → 25; Rìu 240 → 280, 8s → 6.75s; Bút 44 → 28; Hàn Ngọc Tiêu 38 → 62, Trì Trệ 1s → 0.7s, nội lực 4 → 2; Cầm 120 → 90, Điều Tức 6 → 3, nội lực 4 → 2. Đột Phá: Rìu (Phá Cương 80 → 40, Lực Đạo +40 → +20), Cầm (hồi 250 → 150), Quyền Sáo Trọng 9 (đòn phụ 25% → 15%).

**Nhận xét:** bộ dò đẩy món nặng (Côn, Rìu, Đao) mạnh lên và món nhẹ nhiều bản sao (Quyền Sáo, Trảo, Bút) yếu đi, khớp với nhận định "món nhỏ bị nhân theo số bản sao". Bộ dò đã được Codex làm lại thành công cụ `tools/balance/balance.py` (`matrix`, `whatif`, `tune`), xem [README](../../tools/balance/README.md).

## 7 binh khí mới · 2026-09-27

Gemini thiết kế ([báo cáo](../reviews/2026-09-27-gemini-new-weapons.md)), Claude kiểm chứng và sửa 3 lỗi trước khi áp dụng: Chấn Nhạc Chùy Trọng 9 "Định Thân +0.5s" không có tác dụng (thời gian khống chế lấy mức lớn nhất, không cộng) → đổi thành Tuyệt Kỹ −5 nội lực; Thanh Đằng Tiên Trọng 9 "Trì Trệ +0.4s" thực chất là một lần Trì Trệ riêng → đổi thành Tốc Xuất Chiêu −0.2s; Truy Hồn Cung không có Tuyệt Kỹ nhưng cho 1.1 nội lực/giây → 4 nội lực mỗi đòn. Bộ mẫu thành 20 đội (thêm 5 đội dùng binh khí mới); không đội mới nào vượt 70%.

**Phát hiện mới:** khi **cả** Trọng 9 **và** Tâm Pháp cấp 9, vài đội vượt ngưỡng (6 Quyền Sáo ~77%, Kim Cang + Thiết Phủ ~71%, Kiếm Khí ~70%). Các đợt trước chỉ đo Trọng và Tâm Pháp riêng rẽ. Cần một vòng `tools/balance/balance.py tune` với `--tiers 1,9 --tp 1,9`.

## Vấn đề còn mở

- ~~Khắc chế quá cứng~~ → **chấp nhận** (quyết định #13). Mục tiêu cân bằng mới: tỉ lệ thắng **trung bình** của mỗi đội hình trong 35–65%, mỗi đội có ít nhất một cặp thắng và một cặp thua rõ; từng cặp riêng lẻ được phép 0–100%. Ghi chú cũ: Nhiều cặp ra 0–10% hoặc 90–100%, vì trận gần như không có ngẫu nhiên: chỉ có Hội Tâm và việc chọn mục tiêu. Chỉ cần vượt ngưỡng là kết quả lật hẳn, nên cân bằng rất nhạy (giảm 1 điểm Lực Đạo làm một cặp đổi từ 56% xuống 0.2%). Đây là vấn đề thiết kế cần bàn: khắc chế cứng có thể chấp nhận vì người chơi được đổi đội hình, hoặc có thể thêm yếu tố ngẫu nhiên (né tránh, dao động sát thương…).
- Mới cân bằng với 5 đội hình mẫu, chưa có hiệu ứng bộ môn phái. Đội hình mẫu còn ít nên mỗi con số rất nhạy với từng món.
