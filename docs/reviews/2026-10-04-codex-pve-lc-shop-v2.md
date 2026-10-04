# Báo cáo dựng lại PvE theo Thương Hội v2 (#52, #55)

Mô hình roll đúng ba ô item của `shop.json` (kể cả 30% ô giảm giá thành vật liệu), pool item đã loại đồ
quái như `EconomyRuntime(true)`, trọng số phẩm, giá loại, giảm giá và phí làm mới. Mỗi ải chọn LC trung vị
của 9 seed; Lucky là phân vị 75% với K=3 lượt đánh lại. Nâng tier không được mô phỏng: luật cần đúng vật
liệu theo loại/bậc, chất dẫn và đổi vật liệu; cột Bạc là số còn lại sau mua/làm mới. Kết quả đạt **15/30**.

| Ải | Loại | Cấp | Bạc dư | Túi món/LC | Quái món/LC | recommendedPower | Expected | Starter | Lucky | Giây | Sửa |
|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | thường | 1 | 100 | 2/100 | 1/82 | 100 | 100% | 100% | 100% | 14,0 | 5 |
| 2 | thường | 2 | 100 | 4/146 | 4/144 | 146 | 54% | 0% | 100% | 49,2 | 5 |
| 3 | thường | 3 | 50 | 5/168 | 4/152 | 168 | 75% | 0% | 100% | 35,8 | 2 |
| 4 | thường | 5 | 450 | 6/206 | 6/203 | 206 | 56% | 0% | 95% | 50,7 | 5 |
| 5 | tinh anh | 6 | 300 | 7/239 | 6/215 | 239 | 42% | 0% | 100% | 25,4 | 2 |
| 6 | thường | 7 | 0 | 7/279 | 7/279 | 279 | 44% | 0% | 0% | 18,6 | 5 |
| 7 | thường | 8 | 0 | 9/310 | 9/294 | 310 | 72% | 0% | 31% | 18,2 | 1 |
| 8 | thường | 9 | 50 | 11/334 | 10/316 | 334 | 47% | 0% | 88% | 31,5 | 5 |
| 9 | thường | 10 | 150 | 9/355 | 8/325 | 355 | 100% | 0% | 0% | 7,6 | 5 |
| 10 | Thủ Lĩnh | 11 | 0 | 11/380 | 11/376 | 380 | 41% | 0% | 8% | 11,4 | 1 |
| 11 | thường | 12 | 140 | 14/413 | 13/376 | 413 | 72% | 0% | 100% | 47,6 | 2 |
| 12 | thường | 13 | 440 | 15/449 | 15/432 | 449 | 67% | 0% | 100% | 49,8 | 1 |
| 13 | thường | 13 | 90 | 15/478 | 15/468 | 478 | 72% | 0% | 100% | 52,6 | 0 |
| 14 | thường | 14 | 1.390 | 12/488 | 12/478 | 488 | 86% | 0% | 100% | 30,7 | 0 |
| 15 | tinh anh | 15 | 940 | 17/529 | 17/526 | 529 | 59% | 0% | 100% | 54,9 | 0 |
| 16 | thường | 15 | 1.540 | 14/542 | 14/533 | 542 | 70% | 0% | 100% | 25,9 | 0 |
| 17 | thường | 16 | 2.440 | 14/545 | 14/523 | 545 | 96% | 0% | 100% | 25,3 | 5 |
| 18 | thường | 17 | 690 | 15/556 | 15/570 | 556 | 61% | 0% | 100% | 53,3 | 1 |
| 19 | thường | 17 | 790 | 16/593 | 16/581 | 593 | 78% | 0% | 0% | 54,3 | 0 |
| 20 | Thủ Lĩnh | 18 | 6.000 | 17/631 | 16/598 | 631 | 44% | 0% | 100% | 58,0 | 1 |
| 21 | thường | 18 | 240 | 18/672 | 18/687 | 672 | 50% | 0% | 98% | 46,0 | 1 |
| 22 | thường | 19 | 3.000 | 18/687 | 18/673 | 687 | 73% | 0% | 100% | 59,0 | 0 |
| 23 | thường | 19 | 2.300 | 19/715 | 19/703 | 715 | 62% | 0% | 48% | 56,0 | 0 |
| 24 | thường | 20 | 3.550 | 19/718 | 19/705 | 718 | 100% | 0% | 100% | 56,1 | 5 |
| 25 | tinh anh | 20 | 1.390 | 17/739 | 17/733 | 739 | 60% | 0% | 84% | 43,8 | 0 |
| 26 | thường | 21 | 3.390 | 19/769 | 19/756 | 769 | 72% | 0% | 100% | 52,5 | 0 |
| 27 | thường | 21 | 4.790 | 19/769 | 19/750 | 769 | 57% | 0% | 100% | 52,0 | 5 |
| 28 | thường | 22 | 5.790 | 19/774 | 19/758 | 774 | 100% | 0% | 100% | 53,0 | 5 |
| 29 | thường | 22 | 3.330 | 16/778 | 16/772 | 778 | 65% | 0% | 0% | 43,8 | 4 |
| 30 | Thủ Lĩnh | 23 | 8.490 | 19/783 | 19/776 | 783 | 69% | 0% | 0% | 36,0 | 5 |

## Ngoài dải

15 ải: **1 (100%), 2 (54%), 4 (56%), 5 (42%), 6 (44%), 8 (47%), 9 (100%), 14 (86%),
17 (96%), 18 (61%), 21 (50%), 24 (100%), 27 (57%), 28 (100%), 30 (69%)**. Các bước đổi một món/phẩm
vẫn gây bước nhảy lớn; vòng dò 20 trận cũng có thể lệch đáng kể so với xác nhận 50 trận. Không che kết quả.

## Đường Bạc

Túi **tiếp tục lớn qua Hồi 3**: 18 món/LC 672 ở ải 21 lên 19 món/LC 783 ở ải 30, không còn đóng băng
LC 1.042 từ ải 19 như vòng 1. Tuy vậy LC chỉ tăng 14 điểm từ ải 26–30 và còn 8.490 Bạc ở ải 30: offer
ngẫu nhiên thường không tăng LC túi đã gần kín, còn nhánh mua vật liệu/nâng tier đã được bỏ qua như trên.

## Thời lượng

Trung bình expected là **40,4 giây**, 12/30 trận trên 50 giây; không còn việc mọi trận đều trên 50 giây.
Túi ≤10 món trung bình 27,4 giây, túi ≥17 món 51,6 giây. Vì quái là mirror gần cùng số món, cả túi lớn
và hai bên có nhiều hiệu ứng/phòng thủ cùng làm trận dài; dữ liệu không cho phép quy hết cho riêng một phía.

## Tái tạo bằng PowerShell

```powershell
$env:PYTHONIOENCODING = 'utf-8'
py tools/balance/pve_progression.py --stages 1-30
py tools/balance/pve_lc_build.py --stages 1-30 --runs 50 --out tools/codex/out/pve-lc-shop-v2/chapters.json
py tools/lint/check_style.py tools/balance/pve_progression.py tools/balance/pve_lc_build.py
```

Log nguyên văn và JSON kết quả nằm trong `tools/codex/out/pve-lc-shop-v2/tables/`. Không chạy Maven.
