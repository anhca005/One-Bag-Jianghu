# Lực Chiến v3 — đo offense và defense từ engine

## Phương pháp

- Đọc trực tiếp output nhiều trận của CLI: thời lượng, sát thương, maxHp, Hộ Thân, hồi máu, Cương Khí.
- Offense vũ khí: DPS T1 common trong 60 giây trước dummy 100.000 HP, tắt Kiệt Lực.
- Món không gây sát thương: chênh DPS khi ghép với Thanh Phong Kiếm so với kiếm đứng một mình.
- Defense: chênh sức chịu đựng trước attacker cố định gồm Băng Phách Thương, Chấn Nhạc Chùy và Song Đao.
  Sức chịu đựng gồm maxHp, hồi máu, Cương Khí và sát thương Hộ Thân tránh được trong cửa sổ 30 giây.
- Mỗi phép đo dùng 10 seed. Fit `k` trên 50 cặp túi hỗn hợp × 20 trận; chỉ `scale` và `k` được fit.
- `scale = 1,67755168` để bộ khởi đầu cấp 1 bằng 100; `k = 1,5`.
- Không có số riêng nào được đặt tay. Tổng cấu hình chuẩn là 5.460 trận nếu chạy đo, fit và held-out mới;
  validation lần này tái dùng đúng file 3.000 trận của v2 nên không chạy lại các trận đó.

## Held-out — cùng 100 cặp × 30 trận của v2

| Mô hình | Accuracy | Log-loss |
|---|---:|---:|
| v1 | 80,0% | 0,4120 |
| v2 | 87,0% | 0,3443 |
| v3 | 82,0% | 0,5971 |

V3 không tốt hơn v2: accuracy thấp hơn 5 điểm và log-loss xấu hơn rõ rệt. So với v1, v3 tăng 2 điểm
accuracy nhưng log-loss xấu hơn. V1/v2 dùng `k=5` đã hiệu chỉnh của v2; v3 dùng `k=1,5` fit riêng.

## Ca sáu vũ khí của người dùng

| Vũ khí thêm | Thực tế | V3 dự đoán | Sai lệch điểm |
|---|---:|---:|---:|
| Băng Phách Thương | 63% | 29,1% | -33,9 |
| Tử Mẫu Song Đao | 60% | 31,1% | -28,9 |
| Chấn Nhạc Chùy | 60% | 28,8% | -31,2 |
| Khai Sơn Phủ | 47% | 27,6% | -19,4 |
| Liệt Diễm Đao | 23% | 27,9% | +4,9 |
| Không thêm | 0% | 17,0% | +17,0 |

Spearman = **0,8407**, đạt yêu cầu xếp hạng ≥ 0,8. Chỉ 3/6 dự đoán đạt ±20 điểm; ba vũ khí mạnh
không đạt. Song Đao đứng cùng nhóm vũ khí 4 ô, không có override và không còn giá trị 140 đặt tay.

## Top 10 / bottom 10 ở T1 common, cấp 1

| # | Top | LC | Bottom | LC |
|---:|---|---:|---|---:|
| 1 | Tử Kim Trảo | 148 | Ô Tàm Y | 0 |
| 2 | Sói Nanh Bộc Phá | 136 | Bồi Nguyên Đan | 0 |
| 3 | U Minh Quỷ Trảo | 83 | Thiết Giáp Y | 0 |
| 4 | Tử Mẫu Song Đao | 75 | Ô Thiết Giáp | 0 |
| 5 | Băng Phách Thương | 67 | Kim Tuyến Y | 0 |
| 6 | Chấn Nhạc Chùy | 66 | Dạ Hổ Y | 0 |
| 7 | Liệt Diễm Đao | 63 | Nho Sam Y | 0 |
| 8 | Khai Sơn Phủ | 62 | Băng Tâm Giáp | 0 |
| 9 | Truy Hồn Cung | 56 | Tuế Nguyệt Bào Y | 0 |
| 10 | Kịch Độc Chu Ngà | 56 | Phục Ma Giáp | 0 |

LC của món phòng thủ đứng riêng bằng 0 là hệ quả trực tiếp của `sqrt(O × D)` khi offense đo được bằng 0;
đây là một hạn chế của công thức, không được che bằng prior hay số đặt tay.

## Median theo loại

| Loại | Median LC |
|---|---:|
| weapon | 55 |
| manual | 17 |
| armor | 0 |
| pill | 0 |
| relic | 0 |

## Outlier >40% so với median cùng kích thước và loại

- Tử Kim Trảo (`tu_kim_trao`): LC 148, median 47, cao hơn 214,9%.
- Sói Nanh Bộc Phá (`soi_nanh_canh`): LC 136, median 47, cao hơn 189,4%.
- Thuần Dương Công (`thuan_duong_cong`): LC 50, median 22, cao hơn 127,3%.
- Hỏa Diệm Xà Cốt (`hoa_diem_xa_cot`): LC 29, median 15, cao hơn 93,3%.

## Tái lập

```powershell
py tools/balance/power.py --measure
py tools/balance/power.py --calibrate
py tools/balance/power.py --validate
py tools/balance/power.py path/to/loadout.json --power tools/codex/out/power-v3/power.json
py tools/lint/check_style.py tools/balance/power.py
```
