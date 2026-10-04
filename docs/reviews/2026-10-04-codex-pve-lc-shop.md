# Báo cáo dựng PvE theo túi mua ở Thương Hội (#52)

Quái được dựng từ bản sao túi `expected`, rồi chỉnh phẩm/Trọng/số món về LC mục tiêu. Mỗi tỷ lệ dưới đây là
50 trận, hòa tính nửa trận thắng; ngoài dải được thử lại đúng một lần với target LC lệch ±2%. Kết quả đạt
**13/30** ải trong dải. ⚠ là thời lượng trên 40 giây.

Mô hình shop dùng phẩm có trọng số cao nhất của từng ô làm kỳ vọng, chọn món tăng LC túi ngay lập tức nhiều
nhất, mua hết ô có ích rồi làm mới theo thang giá. Không mô phỏng thời gian làm mới miễn phí, ngẫu nhiên ID
món, hay reset phí theo ngày. Giá = giá phẩm × hệ số loại × giảm giá; vũ khí Phàm là 400 Bạc, giáp Phàm 200.

| Ải | Loại | Cấp | Bạc | Túi món/LC | Quái LC/món | Expected | Starter | Lucky | Giây |
|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | thường | 1 | 500 | 3/161 | 160/3 | 65% | 0% | 65% | 51,2 ⚠ |
| 2 | thường | 2 | 400 | 4/222 | 221/4 | 61% | 0% | 61% | 55,1 ⚠ |
| 3 | thường | 3 | 350 | 5/282 | 282/5 | 51% | 0% | 0% | 58,1 ⚠ |
| 4 | thường | 5 | 550 | 5/298 | 298/5 | 31% | 0% | 0% | 54,7 ⚠ |
| 5 | tinh anh | 6 | 600 | 6/317 | 316/6 | 61% | 0% | 61% | 54,5 ⚠ |
| 6 | thường | 7 | 600 | 6/317 | 317/6 | 50% | 0% | 58% | 55,0 ⚠ |
| 7 | thường | 8 | 700 | 7/329 | 314/6 | 40% | 0% | 89% | 54,7 ⚠ |
| 8 | thường | 9 | 650 | 9/401 | 386/8 | 48% | 0% | 48% | 56,0 ⚠ |
| 9 | thường | 10 | 650 | 10/413 | 397/9 | 64% | 0% | 96% | 54,7 ⚠ |
| 10 | Thủ Lĩnh | 11 | 700 | 12/485 | 485/12 | 44% | 0% | 44% | 55,9 ⚠ |
| 11 | thường | 12 | 1.200 | 12/571 | 555/11 | 65% | 0% | 72% | 57,2 ⚠ |
| 12 | thường | 13 | 800 | 12/656 | 640/11 | 63% | 0% | 7% | 58,1 ⚠ |
| 13 | thường | 13 | 950 | 15/741 | 710/13 | 56% | 0% | 8% | 56,7 ⚠ |
| 14 | thường | 14 | 850 | 18/824 | 809/17 | 59% | 0% | 74% | 55,3 ⚠ |
| 15 | tinh anh | 15 | 1.100 | 18/953 | 938/17 | 52% | 0% | 74% | 56,5 ⚠ |
| 16 | thường | 15 | 1.000 | 18/1.038 | 981/17 | 74% | 0% | 76% | 57,0 ⚠ |
| 17 | thường | 16 | 1.000 | 18/1.039 | 982/17 | 77% | 0% | 78% | 57,1 ⚠ |
| 18 | thường | 17 | 1.050 | 18/1.041 | 983/17 | 75% | 0% | 78% | 57,2 ⚠ |
| 19 | thường | 17 | 1.150 | 18/1.042 | 984/17 | 77% | 0% | 77% | 57,3 ⚠ |
| 20 | Thủ Lĩnh | 18 | 1.300 | 18/1.042 | 1.042/18 | 44% | 0% | 44% | 57,5 ⚠ |
| 21 | thường | 18 | 2.100 | 18/1.042 | 985/17 | 56% | 0% | 56% | 57,3 ⚠ |
| 22 | thường | 19 | 3.200 | 18/1.042 | 985/17 | 57% | 0% | 57% | 57,4 ⚠ |
| 23 | thường | 19 | 4.400 | 18/1.042 | 985/17 | 57% | 0% | 57% | 57,4 ⚠ |
| 24 | thường | 20 | 5.650 | 18/1.042 | 985/17 | 58% | 0% | 58% | 57,5 ⚠ |
| 25 | tinh anh | 20 | 6.950 | 18/1.042 | 1.042/18 | 49% | 0% | 49% | 57,6 ⚠ |
| 26 | thường | 21 | 8.550 | 18/1.042 | 985/17 | 57% | 0% | 57% | 57,5 ⚠ |
| 27 | thường | 21 | 9.950 | 18/1.042 | 985/17 | 57% | 0% | 57% | 57,5 ⚠ |
| 28 | thường | 22 | 11.450 | 18/1.042 | 985/17 | 55% | 0% | 55% | 57,6 ⚠ |
| 29 | thường | 22 | 13.050 | 18/1.042 | 985/17 | 55% | 0% | 55% | 57,6 ⚠ |
| 30 | Thủ Lĩnh | 23 | 14.750 | 18/1.042 | 1.042/18 | 47% | 0% | 47% | 57,8 ⚠ |

## Ngoài dải

17 ải: **2 (61%), 3 (51%), 4 (31%), 6 (50%), 7 (40%), 8 (48%), 13 (56%), 14 (59%),
21 (56%), 22 (57%), 23 (57%), 24 (58%), 25 (49%), 26 (57%), 27 (57%), 28 (55%), 29 (55%)**.

LC gần nhau không bảo đảm tỷ lệ gần target: thành phần/vị trí túi tạo các bước nhảy mà LC v1 không biểu diễn.
Đáng chú ý, túi lucky mạnh hơn theo LC nhưng chỉ thắng 0–8% ở ải 3, 4, 12, 13 do tương tác đội hình.

## Hình dạng đường Bạc

Bạc đủ làm túi tăng qua Hồi 1 và phần lớn Hồi 2: 3 món/LC 161 ở ải 1; 12/485 ở ải 10;
18/824 ở ải 14; 18/1.042 ở ải 19. Từ ải 19 túi **chững hẳn**: không còn offer phẩm kỳ vọng nào
cải thiện phép xếp LC/ô. Bạc trước ải vì vậy dồn từ 1.150 (ải 19) lên 2.100 (ải 21) và 14.750
(ải 30), trong khi túi vẫn 18 món/LC 1.042. Bạc không giữ được tăng trưởng qua Hồi 3 trong mô hình này.

## Tái tạo bằng PowerShell

```powershell
$env:PYTHONIOENCODING = 'utf-8'
py tools/balance/pve_progression.py --stages 1-30
py tools/balance/pve_lc_build.py --stages 1-30 --runs 50 --out tools/codex/out/pve-lc-shop/chapters.json
py tools/lint/check_style.py tools/balance/pve_progression.py tools/balance/pve_lc_build.py
```

Không chạy Maven. Log nguyên văn và bảng JSON nằm trong `tools/codex/out/pve-lc-shop/tables/`. Đối chiếu tự
động xác nhận tên, chân dung, môn phái, thoại, mô tả, phần thưởng và mọi trường ngoài cấp/đồ quái giữ nguyên.
