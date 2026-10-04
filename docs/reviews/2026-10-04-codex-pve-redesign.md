# Rà soát đường cong PvE theo kinh tế — Codex, 2026-10-04

## Đạt / chưa đạt

| Mục tiêu | Kết quả 50 trận/ải | Kết luận |
|---|---:|---|
| Không-shop ≥55%, Thủ Lĩnh ≥45% | thấp nhất 2%; Thủ Lĩnh thấp nhất 6% | Chưa đạt |
| Không có dốc >20 điểm | dốc lớn nhất 98 điểm | Chưa đạt |
| Shop-smart ≤90%, tinh anh/Thủ Lĩnh ≤75% | thường cao nhất 100%; tinh anh/Thủ Lĩnh 100% | Chưa đạt |
| Chênh trung bình khoảng 10–25 điểm | 4,4 điểm | Chưa đạt |
| Trung bình hai túi trong dải #34 | chỉ ải 1 nằm trong dải | Chưa đạt |
| Mọi trận trung bình ≤40 giây | cao nhất 48,2 giây | Chưa đạt |
| Hồi 2 không là vách | ải 17 chỉ 4% | Chưa đạt |

Đây là kết quả gần nhất đạt được trong ngân sách chạy rút gọn. Không nên tích hợp `chapters.json` này vào game.

## Phương pháp tiến trình và kinh tế

`progression.py` cộng Bạc, EXP, vật phẩm thưởng lần đầu của các ải trước; cấp người chơi lấy từ
`levels.json`. Túi không-shop chỉ dùng đồ khởi đầu và đồ thưởng. Túi shop-smart thử mua một Binh Khí
chưa sở hữu, theo món tốt nhất đủ ngân sách: Phàm Phẩm không làm mới; Lương/Tinh Phẩm tính kỳ vọng
hai lần làm mới. Bao Phục được đóng theo kích thước thật, tối đa 5x4.

Đánh lại dùng đúng 15% của ải khó nhất đã qua: Bạc/EXP làm tròn, tối thiểu 1; không lặp vật phẩm.
Mục tiêu dùng K=0. Bảng `progression.txt` báo cả Bạc K=0 và K=3. K=3 tăng Bạc trước ải từ 90 ở ải 2
đến 720 ở ải 30.

Shop-smart không còn khác không-shop từ ải 9 vì Bao Phục kín; đây là nguyên nhân chính khiến khoảng
cách hai túi không đạt. Mô hình tiếp theo cần chọn tập con tối ưu từ toàn bộ kho, không nhét mọi thưởng.

## Hồi 1

Đã xuất đủ ải 1–10 và đo 50 trận. Vách ban đầu ải 2 được bỏ, nhưng quá tay: ải 2 đạt 100%.
Ải tinh anh 5 lại rơi còn 4%, trong khi Thủ Lĩnh 10 đạt 100%; đường cong chưa dùng được.

## Hồi 2

Đã xuất đủ ải 11–20 và đo 50 trận. Ải 11–14 đều 100%, tinh anh 15 đạt 62% nhưng dài 47,5 giây;
ải 17 rơi còn 4%, ải 18 là 70%, rồi Thủ Lĩnh 20 trở lại 100%. Hồi 2 vẫn là vách không đều.

## Hồi 3

Đã xuất đủ ải 21–30 và đo 50 trận. Các vách rõ nhất là ải 25 = 8%, 27 = 54%, 29 = 2%,
Thủ Lĩnh 30 = 6%; xen giữa nhiều ải 100%. Không đạt yêu cầu kết thúc chiến dịch.

## Bảng từng ải

`Bạc` là K=0. Thời lượng lấy túi không-shop; shop-smart giống hệt từ ải 9 trở đi.

| Ải | Loại | Cấp địch/người | Bạc | Không-shop | Shop-smart | Giây |
|---:|---|---:|---:|---:|---:|---:|
| 1 | thường | 1/1 | 600 | 88% | 100% | 18,9 |
| 2 | thường | 2/2 | 800 | 100% | 100% | 25,1 |
| 3 | thường | 3/3 | 1.050 | 94% | 100% | 23,9 |
| 4 | thường | 4/5 | 1.350 | 77% | 100% | 19,8 |
| 5 | tinh anh | 5/6 | 1.700 | 4% | 95% | 29,6 |
| 6 | thường | 6/7 | 2.200 | 100% | 100% | 20,0 |
| 7 | thường | 7/8 | 2.600 | 100% | 100% | 17,4 |
| 8 | thường | 8/9 | 3.050 | 100% | 100% | 17,9 |
| 9 | thường | 9/10 | 3.550 | 100% | 100% | 26,1 |
| 10 | Thủ Lĩnh | 11/11 | 4.100 | 100% | 100% | 20,1 |
| 11 | thường | 11/12 | 5.100 | 100% | 100% | 17,9 |
| 12 | thường | 12/13 | 5.700 | 100% | 100% | 23,0 |
| 13 | thường | 13/13 | 6.350 | 100% | 100% | 17,0 |
| 14 | thường | 14/14 | 7.050 | 100% | 100% | 20,3 |
| 15 | tinh anh | 15/15 | 7.800 | 62% | 62% | 47,5 |
| 16 | thường | 16/15 | 8.700 | 100% | 100% | 39,7 |
| 17 | thường | 17/16 | 9.500 | 4% | 4% | 15,8 |
| 18 | thường | 18/17 | 10.350 | 70% | 70% | 48,2 |
| 19 | thường | 19/17 | 11.250 | 98% | 98% | 17,5 |
| 20 | Thủ Lĩnh | 21/18 | 12.200 | 100% | 100% | 23,0 |
| 21 | thường | 21/18 | 13.700 | 100% | 100% | 30,5 |
| 22 | thường | 22/19 | 14.700 | 98% | 98% | 25,9 |
| 23 | thường | 25/19 | 15.800 | 100% | 100% | 38,7 |
| 24 | thường | 24/20 | 16.950 | 100% | 100% | 37,4 |
| 25 | tinh anh | 25/20 | 18.150 | 8% | 8% | 32,5 |
| 26 | thường | 26/21 | 19.650 | 100% | 100% | 35,5 |
| 27 | thường | 27/21 | 20.950 | 54% | 54% | 21,6 |
| 28 | thường | 28/22 | 22.350 | 100% | 100% | 30,1 |
| 29 | thường | 29/22 | 23.850 | 2% | 2% | 21,0 |
| 30 | Thủ Lĩnh | 31/23 | 25.450 | 6% | 6% | 32,3 |

## Audit Binh Khí Phàm Nhất Trọng

Audit dùng bộ khởi đầu + Bồi Nguyên Đan + từng Binh Khí, 50 trận tại ải 2, 10, 12.
Mọi Binh Khí đều đạt 100% ở ải 2, nên phép đo bị bão hòa. Trung vị trung bình ba ải xấp xỉ 54%.
Băng Phách Thương (88,7%), Chấn Nhạc Chùy (87,3%), Tử Mẫu Song Đao (79,7%), Khai Sơn Phủ
(75,3%), Liệt Diễm Đao và Truy Hồn Cung (73,3%) cao hơn trung vị trên 15 điểm.

Không sửa chỉ số: có sáu món vượt ngưỡng và thứ hạng đổi mạnh theo ải, nên chưa đủ bằng chứng rằng một món
là ngoại lệ độc lập với đường cong địch. `patch.json` là `{}`; do đó không có số vật phẩm nào thay đổi.

## Thay đổi dữ liệu đề xuất

- Starter Bạc: 1.000 → 600, làm chậm mua sớm.
- Giá Phàm/Lương/Tinh/Tuyệt/Thần: 200/500/1.200/3.000/8.000 → 300/600/1.400/3.200/8.500.
- Phí làm mới: 100/200/400/800 → 150/250/500/1.000.
- Ô đầu: trọng số 45/40/15 → 60/35/5 cho Phàm/Lương/Tinh; giảm sức mua sớm.
- Thưởng ải và đội địch có thay đổi trong `chapters.json`, nhưng chưa nên tích hợp vì bảng mục tiêu thất bại.
- Tên ải, tên địch và thoại giữ nguyên. Không đổi vật phẩm, nên không cần đổi tên địch.

## Lệnh tái tạo bảng cuối

Chạy từ PowerShell ở gốc repo; không chạy Maven:

```powershell
py tools/codex/out/pve-redesign/progression.py | Set-Content -Encoding utf8 tools/codex/out/pve-redesign/tables/progression.txt
py tools/balance/pve_check.py --runs 50 --chapters tools/codex/out/pve-redesign/chapters.json --players tools/codex/out/pve-redesign/players/no_shop > tools/codex/out/pve-redesign/tables/no_shop_50.txt
py tools/balance/pve_check.py --runs 50 --chapters tools/codex/out/pve-redesign/chapters.json --players tools/codex/out/pve-redesign/players/shop_smart > tools/codex/out/pve-redesign/tables/shop_smart_50.txt
```

Audit nguyên văn nằm tại `tables/weapon_audit_50.txt`; lệnh PowerShell vòng lặp đã dùng được ghi trong lịch sử
task, vì tiện ích hiện tại chưa có chế độ audit ba ải.

## Câu hỏi mở

1. Có nên đổi mô hình shop-smart sang tối ưu tập con 5x4 từ toàn bộ kho trước khi cân lại địch?
2. Có nên giới hạn số vật phẩm thưởng lần đầu, thay một phần bằng nguyên liệu để tránh Bao Phục kín từ ải 9?
