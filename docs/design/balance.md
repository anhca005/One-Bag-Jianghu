# Cân bằng

Quy tắc dùng để đặt chỉ số vật phẩm, cách đo, và kết quả các đợt cân bằng. Số liệu hiện hành luôn nằm trong `server/battle-engine/src/main/resources/data/items.json`.

## Quy tắc 1: ngân sách sức mạnh theo ô

**Sát thương/giây mỗi ô ≈ 6 × hệ số độ hiếm**

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

## Vấn đề còn mở

- **Khắc chế quá cứng** (vẫn còn sau đợt 2; cơ chế bộc phát theo ngưỡng cũng góp phần). Nhiều cặp ra 0–10% hoặc 90–100%, vì trận gần như không có ngẫu nhiên: chỉ có Hội Tâm và việc chọn mục tiêu. Chỉ cần vượt ngưỡng là kết quả lật hẳn, nên cân bằng rất nhạy (giảm 1 điểm Lực Đạo làm một cặp đổi từ 56% xuống 0.2%). Đây là vấn đề thiết kế cần bàn: khắc chế cứng có thể chấp nhận vì người chơi được đổi đội hình, hoặc có thể thêm yếu tố ngẫu nhiên (né tránh, dao động sát thương…).
- Mới cân bằng với 5 đội hình mẫu, chưa tính hệ số Trọng và hiệu ứng bộ môn phái.
