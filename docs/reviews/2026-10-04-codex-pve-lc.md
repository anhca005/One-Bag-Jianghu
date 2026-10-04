# Báo cáo dựng PvE theo Lực Chiến (#50, #51)

Đã sửa mô hình túi trước khi dựng ải: cả no-shop và shop-smart nay chọn đồ theo LC/ô từ `power.json` trong game. Khi túi 5×4 đầy, đồ mua cạnh tranh trực tiếp và thay món yếu nếu có LC/ô tốt hơn; không còn dùng heuristic Trọng/phẩm/giá cũ. Kết quả dưới đây là 50 trận/lần, hòa tính nửa trận thắng. Có **16/30** ải trong dải; ⚠ là trên 40 giây.

| Ải | Loại | Cấp | LC no-shop | LC mua khôn | LC quái | LC đề nghị | No-shop | Mua khôn | Giây |
|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | thường | 1 | 100 | 182 | 100 | 110 | 54% | 100% | 42,2 ⚠ |
| 2 | thường | 2 | 109 | 215 | 104 | 114 | 94% | 100% | 43,3 ⚠ |
| 3 | thường | 3 | 113 | 273 | 113 | 124 | 57% | 100% | 48,7 ⚠ |
| 4 | thường | 5 | 135 | 329 | 131 | 144 | 96% | 100% | 29,3 |
| 5 | tinh anh | 6 | 144 | 334 | 139 | 153 | 77% | 100% | 30,3 |
| 6 | thường | 7 | 168 | 377 | 164 | 180 | 81% | 100% | 39,6 |
| 7 | thường | 8 | 180 | 377 | 175 | 193 | 86% | 100% | 27,2 |
| 8 | thường | 9 | 231 | 377 | 218 | 240 | 76% | 100% | 47,4 ⚠ |
| 9 | thường | 10 | 232 | 381 | 219 | 241 | 75% | 100% | 50,4 ⚠ |
| 10 | Thủ Lĩnh | 11 | 236 | 381 | 236 | 260 | 54% | 100% | 51,6 ⚠ |
| 11 | thường | 12 | 286 | 401 | 270 | 297 | 76% | 100% | 28,4 |
| 12 | thường | 13 | 280 | 401 | 268 | 295 | 74% | 100% | 40,2 ⚠ |
| 13 | thường | 13 | 280 | 401 | 268 | 295 | 74% | 100% | 40,2 ⚠ |
| 14 | thường | 14 | 307 | 421 | 294 | 323 | 52% | 100% | 28,7 |
| 15 | tinh anh | 15 | 322 | 426 | 321 | 353 | 46% | 100% | 12,9 |
| 16 | thường | 15 | 332 | 438 | 326 | 359 | 50% | 100% | 26,9 |
| 17 | thường | 16 | 358 | 457 | 341 | 375 | 68% | 100% | 37,4 |
| 18 | thường | 17 | 358 | 457 | 341 | 375 | 64% | 100% | 37,4 |
| 19 | thường | 17 | 391 | 461 | 374 | 411 | 72% | 100% | 29,0 |
| 20 | Thủ Lĩnh | 18 | 401 | 472 | 393 | 432 | 56% | 100% | 12,4 |
| 21 | thường | 18 | 401 | 541 | 393 | 432 | 87% | 100% | 12,9 |
| 22 | thường | 19 | 413 | 543 | 405 | 446 | 84% | 94% | 13,2 |
| 23 | thường | 19 | 411 | 553 | 395 | 435 | 62% | 100% | 16,5 |
| 24 | thường | 20 | 430 | 553 | 422 | 464 | 68% | 100% | 22,1 |
| 25 | tinh anh | 20 | 430 | 553 | 424 | 466 | 62% | 100% | 22,1 |
| 26 | thường | 21 | 430 | 553 | 410 | 451 | 84% | 100% | 22,1 |
| 27 | thường | 21 | 430 | 553 | 410 | 451 | 46% | 100% | 21,7 |
| 28 | thường | 22 | 462 | 549 | 447 | 492 | 70% | 10% | 17,7 |
| 29 | thường | 22 | 462 | 549 | 447 | 492 | 70% | 10% | 17,7 |
| 30 | Thủ Lĩnh | 23 | 462 | 606 | 462 | 508 | 33% | 15% | 17,4 |

## Ngoài dải

14 ải: **1 (54%), 2 (94%), 3 (57%), 4 (96%), 5 (77%), 7 (86%), 14 (52%), 15 (46%), 16 (50%), 21 (87%), 22 (84%), 26 (84%), 27 (46%), 30 (33%)**.

Mỗi ải ngoài dải đã được dựng và đo lại đúng một lần với target LC dịch ±2%; bảng giữ kết quả gần dải hơn. Log nguyên văn và hai phép đo nằm trong `tools/codex/out/pve-lc/tables/pve-lc.txt`. Đường cong LC không bảo đảm thứ tự thắng trong mọi kèo: nổi bật ải 28–30, túi mua khôn có LC cao hơn nhưng chỉ thắng 10–15% do tương tác đội hình mà mô hình LC v1 không biểu diễn.

## Xung đột văn bản

Có **21 dòng tại 16 ải**, không có xung đột trong `description`; mỗi dòng dưới đây nhắc món quái cuối không mang:

- Ải 8: `afterLoss` — Cuồng Hỏa Đan.
- Ải 11: `before` — Giải Độc Đan.
- Ải 12: `afterWin` — U Minh Bí Lục.
- Ải 13: `before` — Kịch Độc Chu Ngà.
- Ải 14: `before` — Dưỡng Sinh Quyết.
- Ải 16: `before` — Hắc Sát Hùng Đảm.
- Ải 18: `afterWin` — Tử Kim Trảo.
- Ải 19: `before`, `afterWin` — Hỏa Diệm Xà Cốt.
- Ải 20: `before` — Mãng Xà Độc Túi.
- Ải 22: `before` — Hàn Ngọc Tiêu.
- Ải 23: `afterWin` — Tuyết Lang Bạch Bì.
- Ải 24: `afterWin` — Phục Ma Giáp.
- Ải 25: `before`, `afterWin` — Băng Tàm Linh Châu.
- Ải 26: `before`, `afterWin` — Bạch Hổ Cuồng Móng.
- Ải 27: `before`, `afterWin` — U Minh Quỷ Trảo.
- Ải 28: `before` — Thiên Niên Quy Giáp và Băng Phách Cổ Cốt (2 xung đột).

## Tái tạo bằng PowerShell

```powershell
$env:PYTHONIOENCODING = 'utf-8'
py tools/balance/pve_lc_build.py --stages 1-30 --runs 50 --out tools/codex/out/pve-lc/chapters.json
py tools/lint/check_style.py tools/balance/pve_lc_build.py tools/balance/pve_progression.py
```

Không chạy Maven; công cụ dùng classpath có sẵn. Đối chiếu tự động xác nhận mọi trường ngoài `recommendedLevel`, `enemy.level` và `enemy.items` giống hệt nguồn game.
