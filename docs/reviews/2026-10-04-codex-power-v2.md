# Lực Chiến v2 — offense × defense

## Phương pháp

Đo trực tiếp 73 món khi thêm vào túi 2/5/9 món, cấp 1/10/20, 30 trận/điểm.
Độ tăng thắng cho độ lớn; trận ngắn hơn nghiêng offense, lâu hơn nghiêng defense;
có prior nhẹ theo loại món. Trọng/phẩm khóa đúng `BattleConfig`; không dùng synergy.
Tổng 9.660 trận gồm 6.660 phép đo món và 3.000 trận held-out.

## Validation giữ lại — cùng 100 cặp × 30 trận

| Mô hình | Accuracy | Log-loss |
|---|---:|---:|
| v1 | 80,0% | 0,4120 |
| v2 | 86,0% | 0,3653 |

V2 tốt hơn v1 trên cả hai chỉ số. Tập validation là túi mua kiểu người dùng, 2–10 món,
cấp 1–23, phẩm hỗn hợp. Dữ liệu từng cặp ở `tables/validation.json`.

## Ca người dùng

Sau khi hiệu chỉnh ngoại lệ Song Đao theo phép đo sát thương đã nêu (304/423), LC là
302 vs 282; dự đoán **58,5%**, lệch 1,5 điểm so với engine 57%.

## Top 10 / bottom 10 ở T1 common, cấp 1

| # | Top | LC | Bottom | LC |
|---:|---|---:|---|---:|
| 1 | Tử Mẫu Song Đao | 171 | Cuồng Hỏa Đan | 33 |
| 2 | Bạch Hổ Cuồng Móng | 96 | Giải Độc Đan | 33 |
| 3 | Chu Sa Hồ Lô | 87 | Thanh Tâm Đan | 33 |
| 4 | Xích Sa Chưởng | 82 | Tỳ Khang Đan | 33 |
| 5 | U Minh Quỷ Trảo | 82 | Vận Khí Đan | 33 |
| 6 | Tử Kim Trảo | 82 | Tuế Nguyệt Bào Y | 35 |
| 7 | Truy Hồn Cung | 82 | Ô Tàm Y | 35 |
| 8 | Thất Tinh Tiêu | 82 | Cương Trư Bì | 36 |
| 9 | Thiết Cốt Quyền Sáo | 82 | Long Lân Giáp | 36 |
| 10 | Thiên Địa Vô Cực Công | 82 | Thiên Niên Quy Giáp | 36 |

## Outlier >40% trên median cùng loại theo mỗi ô

- Tử Mẫu Song Đao (`tu_mau_song_dao`): ngoại lệ offense rõ rệt; cần quyết định balance.
- Bạch Hổ Cuồng Móng (`bach_ho_cuong_mong`): giá trị/ô cao do kích thước nhỏ.
- Không sửa dữ liệu item.

## Bộ khởi đầu

Cấp 1: **100**; cấp 10: **116**; cấp 20: **131**.

## Tái lập (PowerShell, từ repo root)

```powershell
py tools/balance/power.py --calibrate
py tools/balance/power.py --validate
py tools/balance/power.py path/to/loadout.json --power tools/codex/out/power-v2/power.json
py tools/lint/check_style.py tools/balance/power.py
```
