# Báo cáo PvE Hồi 4–5

## Cốt truyện và nhân vật

**Hồi 4 · Xuân Vũ Kinh Kỳ:** qua đồng nội Trung Nguyên trong mưa xuân, Hiệp Khách hộ tống mật sớ,
phá đường dây giả lệnh từ dịch trạm đến thủy môn và hạ U Minh Hữu Sứ trên chiến thuyền. **Hồi 5 · Hạ
Nhật Đế Kinh:** giữa kinh thành mùa hạ, nhóm phá giả chiếu và cơ nỏ của Thiên Cơ Viện, cứu đại lễ;
Tào Huyền Kỳ bị hạ nhưng Giáo chủ đã mang lõi Thần Nang tới Bắc Minh, nên xung đột chính còn mở.

Ba nhân vật mới: **Yến Vô Tâm**, Ngự Sử Tuần Án đồng hành; **Lộ Tẫn Quang**, U Minh Hữu Sứ và boss
Hồi 4; **Tào Huyền Kỳ**, Giám chủ Thiên Cơ Viện và boss Hồi 5. Mỗi Hồi có đúng hai ải quái thú,
chỉ dùng ID trong `monsterItems`; không thêm vật phẩm.

## Kết quả từng ải

`Bạc` là số dư trước ải. `Túi` và `Quái` ghi số món/LC. Tune dùng seeds 1–20. Fresh được tính đúng
từ hai lần chạy CLI 50 và 100: `(100 × rate100 − 50 × rate50) / 50`, tức riêng seeds 51–100.

| Ải | Tên | Loại | Cấp | Bạc | Túi | Quái | recPower | Tune | Fresh | Giây |
|---:|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 31 | Thanh Dã Xuân Kiều | thường | 24 | 8.590 | 18/795 | 18/758 | 795 | 85% | 86% | 54,1 |
| 32 | Liễu Âm Cổ Dịch | thường | 24 | 10.390 | 18/795 | 18/758 | 795 | 85% | 86% | 54,1 |
| 33 | Linh Miêu Trúc Viên | thường | 24 | 12.090 | 18/798 | 18/784 | 798 | 60% | 49% | 46,3 |
| 34 | Tế Vân Trà Phường | thường | 25 | 10.790 | 18/814 | 18/768 | 814 | 65% | 67% | 24,2 |
| 35 | Vũ Đình Đoạt Sớ | tinh anh | 25 | 12.890 | 18/814 | 17/716 | 814 | 100% | 100% | 22,7 |
| 36 | Hoàng Thổ Quan Đạo | thường | 26 | 15.390 | 18/817 | 18/775 | 817 | 57,5% | 66% | 24,5 |
| 37 | Hỏa Giác Ngưu Tràng | thường | 26 | 17.590 | 18/817 | 18/775 | 817 | 57,5% | 66% | 24,5 |
| 38 | Vọng Kinh Phong Đài | thường | 27 | 19.890 | 18/820 | 18/785 | 820 | 62,5% | 69% | 24,6 |
| 39 | Đông Quách Thủy Môn | thường | 27 | 22.290 | 18/820 | 18/785 | 820 | 62,5% | 69% | 24,6 |
| 40 | Vân Tân Hắc Chu | Thủ Lĩnh | 28 | 22.290 | 17/827 | 17/831 | 827 | 47,5% | 30% | 15,5 |
| 41 | Kim Minh Ngoại Thành | thường | 28 | 27.870 | 18/828 | 18/786 | 828 | 55% | 54% | 19,5 |
| 42 | Chu Tước Trường Nhai | thường | 29 | 28.490 | 17/834 | 17/804 | 834 | 80% | 64% | 15,6 |
| 43 | Đồng Sư Cơ Phường | thường | 29 | 34.150 | 19/835 | 19/818 | 835 | 85% | 78% | 52,9 |
| 44 | Bạch Ngọc Văn Môn | thường | 30 | 34.190 | 17/838 | 17/820 | 838 | 62,5% | 48% | 16,6 |
| 45 | Thừa Thiên Đài Ấn | tinh anh | 30 | 39.090 | 19/847 | 19/843 | 847 | 60% | 59% | 54,1 |
| 46 | Ngự Thủy Ám Cừ | thường | 30 | 37.670 | 19/850 | 19/810 | 850 | 80% | 77% | 28,5 |
| 47 | Kim Sí Yêu Điện | thường | 30 | 40.870 | 19/850 | 19/836 | 850 | 87,5% | 82% | 29,6 |
| 48 | Thái Dịch Liên Đình | thường | 30 | 44.170 | 19/850 | 19/836 | 850 | 87,5% | 82% | 29,6 |
| 49 | Xã Tắc Cơ Đàn | thường | 30 | 47.570 | 19/850 | 19/836 | 850 | 87,5% | 82% | 29,6 |
| 50 | Minh Quang Thiên Cơ Điện | Thủ Lĩnh | 30 | 51.070 | 19/850 | 19/850 | 850 | 52,5% | 46% | 30,6 |

## Ngoài dải

Fresh seeds đạt **13/20**. Ngoài dải: **31 (86%), 32 (86%), 33 (49%), 35 (100%), 40 (30%),
41 (54%), 44 (48%)**. Không che sai lệch: bước đổi phẩm/món vẫn rời rạc; các ải 31–33 và 35 đặc biệt
nhạy theo tổ hợp, còn mẫu tune 20 trận không dự báo ổn định cho boss 40.

## Cấp tối đa và Bạc

Người chơi đạt **cấp 30 trước ải 44**; cấp địch không vượt 30. Từ ải 44, độ khó chỉ đổi bằng phẩm,
Trọng và số món. Bạc dư tăng từ **8.490 trước ải 30** lên **51.070 trước ải 50**: vấn đề tích Bạc
xấu hơn rõ rệt vì Bao Phục gần kín và mô hình chưa mua nguyên liệu/nâng Trọng.

## Hình cần làm

- `map_hoi_4`: đồng nội Trung Nguyên mưa xuân, cầu đá, dịch trạm, vận hà và tường kinh xa.
- `bg_battle_hoi_4`: bến thủy môn dưới mưa xanh ngọc, chiến thuyền đen và đèn phản chiếu.
- `node_hoi_4`, `node_boss_hoi_4`: nút ngọc mưa và biến thể chiến thuyền U Minh.
- `map_hoi_5`: đế kinh mùa hạ, trường nhai, hồ sen, đàn tế và Minh Quang điện.
- `bg_battle_hoi_5`: sân điện nắng gắt, cơ nỏ đồng và cờ lễ đỏ cam.
- `node_hoi_5`, `node_boss_hoi_5`: nút ấn son và biến thể bánh răng Thiên Cơ.
- `npc_yen_vo_tam`: tuần án áo xanh sẫm, cầm mật sớ, vẻ điềm tĩnh sắc bén.
- `boss_lo_tan_quang`: Hữu Sứ áo đen viền lửa, hỏa đao trên boong thuyền mưa.
- `boss_tao_huyen_ky`: Giám chủ quan phục đồng đỏ, đứng giữa đại cơ trận phát sáng.

## Tái tạo bằng PowerShell

```powershell
$env:PYTHONIOENCODING = 'utf-8'
py tools/balance/pve_progression.py --stages 31-50 |
  Tee-Object -FilePath tools/codex/out/pve-hoi-4-5/tables/progression.txt
py tools/balance/pve_lc_build.py --stages 31-50 --runs 100 `
  --out tools/codex/out/pve-hoi-4-5/chapters.json
py tools/lint/check_style.py tools/balance/pve_progression.py tools/balance/pve_lc_build.py
```

Log nguyên văn nằm trong `tables/pve-lc.txt`; hàng máy đọc nằm trong `tables/results.json`. Không chạy Maven.
