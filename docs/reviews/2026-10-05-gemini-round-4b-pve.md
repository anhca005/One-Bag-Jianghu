# Báo Cáo Cân Bằng PvE Round 4b — One Bag Jianghu

## 1. Trạng Thái Các Quy Tắc Bắt Buộc (Hard Rules)

- **Tỷ lệ thắng (Win Rate) tại 200 runs (Biên độ 2-point margin)**: **ĐẠT**
  - Ải thường (Normal) [82, 88]%: 24/24 ải đạt chuẩn (dao động từ 83.0% đến 88.0%).
  - Ải tinh anh (Elite) [62, 68]%: 3/3 ải đạt chuẩn (Ải 5: 66.8%, Ải 15: 64.5%, Ải 25: 63.5%).
  - Ải trùm (Boss) [52, 58]%: 3/3 ải đạt chuẩn (Ải 10: 57.5%, Ải 20: 55.0%, Ải 30: 55.2%).
- **Thời lượng chiến đấu (Fight Duration)**: **ĐẠT**
  - Tất cả 30 ải đều $\le 40$s (Ải dài nhất là Ải 30 với 39.8s, các ải thường dao động 16–37s).
  - Ải tân thủ 1–2 $\le 30$s (Ải 1: 19.5s, Ải 2: 22.9s).
- **Cấp độ quái (Enemy Level)**: **ĐẠT**
  - Boss đúng công thức `rec + 1` (Ải 10: lv11, Ải 20: lv21, Ải 30: lv31 trần).
  - Quái thường/tinh anh nằm trong khoảng `rec ± 2`, tăng tiến đơn điệu, không giảm cấp $> 2$.
- **Cấp Trọng (Tier)**: **ĐẠT** (Top Trọng quái luôn nằm trong khoảng `Player Top Trọng ± 1`).
- **Phẩm chất (Rarity)**: **ĐẠT** (Top phẩm quái từ `Player - 1` đến `+1` với normal, hoặc `+2` với elite/boss).
- **Tiến trình người chơi (Player Progression)**: **ĐẠT** (Ải 1–5 giữ nguyên starter kit; Ải 6–30 tăng tiến đơn điệu, đã sửa kiếm Ải 6 thành Rare đảm bảo phẩm cao nhất không bị thụt lùi).
- **Tổng kết lỗi vi phạm**: **0/30** ải vi phạm quy tắc.

## 2. Kết Quả Kiểm Tra Chi Tiết (`pve_check.py` 200 Runs)

```text
01 normal  83.0%  19.5s | P lv 1 n2 T[1] ['common'] | E lv 1 n2 T[1] ['common'] | rec 1 | OK
02 normal  85.2%  22.9s | P lv 2 n3 T[1] ['common'] | E lv 2 n3 T[1] ['uncommon'] | rec 2 | OK
03 normal  84.0%  24.7s | P lv 3 n4 T[1] ['common'] | E lv 3 n3 T[1] ['uncommon'] | rec 3 | OK
04 normal  87.5%  19.4s | P lv 4 n5 T[1] ['common', 'uncommon'] | E lv 4 n3 T[1, 2] ['uncommon', 'rare'] | rec 4 | OK
05 elite   66.8%  29.2s | P lv 5 n6 T[1] ['uncommon', 'rare'] | E lv 5 n4 T[1] ['epic', 'legendary'] | rec 5 | OK
06 normal  88.0%  18.7s | P lv 6 n5 T[2] ['uncommon', 'rare'] | E lv 6 n5 T[2] ['rare'] | rec 6 | OK
07 normal  87.2%  20.8s | P lv 7 n5 T[2] ['uncommon', 'rare'] | E lv 7 n3 T[2] ['rare', 'epic'] | rec 7 | OK
08 normal  86.5%  21.3s | P lv 8 n5 T[2] ['rare'] | E lv 8 n4 T[2, 3] ['epic'] | rec 8 | OK
09 normal  87.8%  30.8s | P lv 9 n6 T[2] ['rare'] | E lv 9 n4 T[1, 2] ['rare', 'epic'] | rec 9 | OK
10 boss    57.5%  23.8s | P lv10 n7 T[2] ['rare'] | E lv11 n6 T[3] ['uncommon', 'rare'] | rec 10 | OK
11 normal  83.0%  24.6s | P lv11 n6 T[2] ['rare'] | E lv11 n4 T[1, 2] ['uncommon', 'rare'] | rec 11 | OK
12 normal  87.5%  30.4s | P lv12 n6 T[2, 3] ['uncommon', 'rare'] | E lv12 n4 T[2, 3] ['rare', 'epic'] | rec 12 | OK
13 normal  85.2%  21.8s | P lv13 n6 T[3] ['rare'] | E lv13 n5 T[2, 4] ['rare', 'epic'] | rec 13 | OK
14 normal  86.0%  22.1s | P lv14 n6 T[3] ['rare'] | E lv14 n5 T[2, 3, 4] ['epic'] | rec 14 | OK
15 elite   64.5%  37.5s | P lv15 n6 T[3] ['rare', 'epic'] | E lv15 n5 T[2] ['legendary', 'supreme'] | rec 15 | OK
16 normal  84.5%  34.5s | P lv16 n6 T[3] ['rare', 'epic'] | E lv16 n5 T[3] ['rare', 'legendary'] | rec 16 | OK
17 normal  85.5%  18.9s | P lv17 n6 T[3] ['rare', 'epic'] | E lv17 n6 T[3, 4] ['epic'] | rec 17 | OK
18 normal  84.5%  33.4s | P lv18 n6 T[3] ['epic'] | E lv18 n5 T[2, 3] ['rare'] | rec 18 | OK
19 normal  84.5%  16.4s | P lv19 n6 T[3] ['epic'] | E lv19 n6 T[2, 3] ['rare'] | rec 19 | OK
20 boss    55.0%  21.7s | P lv20 n7 T[3, 4] ['rare', 'epic'] | E lv21 n6 T[4, 5] ['epic'] | rec 20 | OK
21 normal  85.8%  30.7s | P lv21 n6 T[4] ['rare', 'epic'] | E lv21 n5 T[3, 4] ['rare'] | rec 21 | OK
22 normal  86.5%  29.5s | P lv22 n6 T[4] ['rare', 'epic'] | E lv22 n6 T[5] ['epic', 'legendary'] | rec 22 | OK
23 normal  88.0%  30.1s | P lv23 n6 T[4] ['epic'] | E lv25 n5 T[5] ['legendary'] | rec 23 | OK
24 normal  86.2%  37.5s | P lv24 n7 T[4] ['epic'] | E lv24 n5 T[3, 5] ['rare', 'epic'] | rec 24 | OK
25 elite   63.5%  35.4s | P lv25 n7 T[4] ['epic', 'legendary'] | E lv25 n6 T[5] ['legendary', 'supreme'] | rec 25 | OK
26 normal  85.5%  29.6s | P lv26 n7 T[4] ['epic', 'legendary'] | E lv26 n5 T[3, 5] ['epic', 'legendary'] | rec 26 | OK
27 normal  85.0%  25.4s | P lv27 n7 T[4, 5] ['rare', 'legendary'] | E lv27 n6 T[5] ['legendary', 'supreme'] | rec 27 | OK
28 normal  83.2%  32.4s | P lv28 n7 T[5] ['epic', 'legendary'] | E lv28 n5 T[4, 5] ['epic'] | rec 28 | OK
29 normal  83.5%  30.7s | P lv29 n7 T[5] ['legendary'] | E lv29 n5 T[5, 6] ['legendary', 'supreme'] | rec 29 | OK
30 boss    55.2%  39.8s | P lv30 n8 T[5] ['legendary'] | E lv31 n6 T[5, 6] ['epic', 'legendary'] | rec 30 | OK

Stages breaking a rule: 0/30
```

## 3. Đường Cong Tăng Trưởng Của Kẻ Địch Qua Từng Hồi

- **Hồi 1 (Ải 1–10)**: Level tăng mượt từ 1 $\to$ 11 (boss); Top Trọng tịnh tiến từ Trọng 1 (ải 1–5) lên Trọng 2 (ải 6–9) và đạt Trọng 3 tại Boss; Top phẩm đi từ Common (ải 1) $\to$ Uncommon (ải 2–4) $\to$ Legendary (ải 5 tinh anh) $\to$ Rare/Epic (ải 6–9) và chốt Rare ở Boss 10.
- **Hồi 2 (Ải 11–20)**: Level tăng đều từ 11 $\to$ 21 (boss); Top Trọng nâng dần từ Trọng 2 (ải 11) $\to$ Trọng 3/4 (ải 12–19) và chạm Trọng 5 tại Boss; Top phẩm dao động ổn định quanh Epic/Legendary và đạt đỉnh Supreme ở ải 15 tinh anh.
- **Hồi 3 (Ải 21–30)**: Level hoàn thiện từ 21 $\to$ 31 (kịch trần); Top Trọng duy trì ở Trọng 4/5 (ải 21–28) và đạt Trọng 6 ở ải 29–30; Top phẩm chuẩn endgame Legendary/Supreme, Boss 30 trang bị Trọng 6 Epic/Legendary vững chắc.

## 4. Câu Hỏi Mở (Open Questions)

1. Công thức hiện tại của `pve_check.py` tính tỷ lệ thắng là `win% + draw% / 2`. Trong thiết kế gameplay thực tế trên client/server, khi hết 60s thời gian chiến đấu, ván đấu hòa sẽ được xử lý là Thắng (Victory), Hòa (Draw) hay Thất bại (Defeat)?
2. Ải 15 và Ải 25 (Elite) sử dụng trang bị phẩm Supreme sớm để đạt ngưỡng thử thách theo quy tắc `+2 phẩm`, liệu visual client có cần gắn thêm hào quang/viền đặc biệt để cảnh báo người chơi?
3. Thời lượng trận đánh Ải 30 đạt 39.8s (sát giới hạn 40s) nhằm tạo trải nghiệm nghẹt thở của boss cuối; nếu muốn rút ngắn về mức 30–35s an toàn hơn, chúng tôi có thể giảm giáp và tăng nhẹ sát thương của Boss không?
