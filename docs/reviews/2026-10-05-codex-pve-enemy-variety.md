# Rà soát độ đa dạng trang bị địch PvE

## Round 1 — giữ LC

Round 1 giữ được LC của 50/50 ải trong ±5%, nhưng chỉ 10/50 ải giữ được tỉ lệ thắng cũ trong ±10 điểm. Kết quả cho thấy LC không dự báo đúng khi đổi lớp vũ khí; điển hình ải 4 đổi sang đao, LC 203 → 210 nhưng tỉ lệ thắng 92% → 0%. Vì vậy bản Round 1 không được áp dụng.

## Round 2 — dò trực tiếp theo tỉ lệ thắng

Kết quả xác nhận 50 trận: **36/49 ải chỉnh sửa nằm trong dải**. Điều kiện áp dụng cần ít nhất 45/49, nên **không áp dụng** ứng viên vào dữ liệu game và không tạo `chapters-before.json`. Tất cả luật cứng về vũ khí, vị trí túi, ải 1 và trường dữ liệu không phải `enemy.items` đều đạt.

Dải mục tiêu: thường 72% (60–84%), tinh anh 62% (50–74%), Thủ Lĩnh 50% (38–62%). `recommendedPower` được giữ nguyên.

### Theo từng ải

| Ải | Địch | Vũ khí cũ → mới | LC cũ → mới | Mục tiêu | Thắng cũ → mới | Kết quả |
|---:|---|---|---:|---:|---:|---|
| 1 | Thảo Khấu Tiên Phong | Thanh Phong Kiếm → Thanh Phong Kiếm | 82 → 82 | 72% | 100.0% → 100.0% | miễn |
| 2 | Thủy Phỉ Thám Tử | Thanh Phong Kiếm → Phục Hổ Côn | 144 → 107 | 72% | 3.0% → 82.0% | đạt |
| 3 | Hoang Dã Sói Xám | Thanh Phong Kiếm → Kịch Độc Chu Ngà | 152 → 126 | 72% | 44.0% → 66.0% | đạt |
| 4 | Liệt Diễm Bang Chúng | Thanh Phong Kiếm → Liệt Diễm Đao | 203 → 121 | 72% | 92.0% → 68.0% | đạt |
| 5 | Phong Lôi Khoái Đao Phong Vũ | Thanh Phong Kiếm, Thanh Phong Kiếm → Tử Mẫu Song Đao, Tử Mẫu Song Đao | 215 → 201 | 62% | 22.0% → 54.0% | đạt |
| 6 | Ám Hạng Thích Khách | Thanh Phong Kiếm, Khai Sơn Phủ → Thất Tinh Tiêu | 279 → 241 | 72% | 0.0% → 60.0% | đạt |
| 7 | Thiết Bì Trư Thú | Thiết Cốt Quyền Sáo, Thanh Phong Kiếm → Thiết Cốt Quyền Sáo | 294 → 181 | 72% | 0.0% → 65.0% | đạt |
| 8 | Liệt Diễm Bang Hương Chủ | Thanh Phong Kiếm, Tử Mẫu Song Đao → Liệt Diễm Đao | 316 → 225 | 72% | 0.0% → 64.0% | đạt |
| 9 | Ba Hổ Tiên Phong Vệ | Xích Sa Chưởng, Ngọc Cốt Phiến, Thanh Phong Kiếm, Chấn Nhạc Chùy → Thanh Phong Kiếm, Thanh Phong Kiếm | 325 → 342 | 72% | 77.0% → 59.0% | không đạt |
| 10 | Nộ Ba Hổ Bùi Chấn | Truy Hồn Cung, Thanh Phong Kiếm, Tử Mẫu Song Đao → Liệt Diễm Đao | 376 → 299 | 50% | 0.0% → 58.0% | đạt |
| 11 | Vạn Độc Môn Đồ | Thanh Phong Kiếm → Kịch Độc Chu Ngà | 376 → 304 | 72% | 100.0% → 77.0% | đạt |
| 12 | Huyền Độc Môn Đồ | Thanh Phong Kiếm → Thất Tinh Tiêu | 432 → 384 | 72% | 10.0% → 72.0% | đạt |
| 13 | Kịch Độc Tri Chu | Hàn Ngọc Tiêu → Kịch Độc Chu Ngà | 468 → 156 | 72% | 45.0% → 64.0% | đạt |
| 14 | Man Cương Vu Đao | Tử Kim Trảo, Thất Tinh Tiêu, Phán Quan Bút → Liệt Diễm Đao, Liệt Diễm Đao | 478 → 368 | 72% | 100.0% → 74.0% | đạt |
| 15 | Bách Thảo Nghịch Đồ | Tử Kim Trảo, Thanh Phong Kiếm → Thất Tinh Tiêu, Thất Tinh Tiêu | 422 → 345 | 62% | 54.0% → 58.0% | đạt |
| 16 | Hắc Sát Độc Hùng | Liệt Diễm Đao → U Minh Quỷ Trảo | 428 → 382 | 72% | 50.0% → 56.0% | không đạt |
| 17 | U Minh Ám Sát Thí Tử | Liệt Diễm Đao → Thất Tinh Tiêu | 446 → 467 | 72% | 68.0% → 75.0% | đạt |
| 18 | Huyền Độc Trưởng Lão | Xích Sa Chưởng, Ngọc Cốt Phiến → Ngọc Cốt Phiến, Ngọc Cốt Phiến | 467 → 537 | 72% | 65.0% → 88.0% | không đạt |
| 19 | Xích Luyện Xà Tinh | Xích Sa Chưởng, Ngọc Cốt Phiến → Thanh Đằng Tiên, Thanh Đằng Tiên | 469 → 468 | 72% | 66.0% → 70.0% | đạt |
| 20 | Hắc Độc Nương Tử | Thanh Phong Kiếm → Xích Sa Chưởng | 528 → 494 | 50% | 45.0% → 34.0% | không đạt |
| 21 | Hàn Phong Tuần Tuyết Giả | Thanh Phong Kiếm → Băng Phách Thương | 502 → 510 | 72% | 68.0% → 65.0% | đạt |
| 22 | Hàn Sơn Kiếm Sĩ | Thanh Phong Kiếm → Thanh Phong Kiếm | 523 → 579 | 72% | 61.0% → 74.0% | đạt |
| 23 | Tuyết Lĩnh Ngân Lang | Tử Kim Trảo, Xích Sa Chưởng → U Minh Quỷ Trảo, U Minh Quỷ Trảo | 555 → 442 | 72% | 64.0% → 54.0% | không đạt |
| 24 | Kim Cang Hộ Giới Tăng | Tử Kim Trảo, Xích Sa Chưởng → Phục Hổ Côn, Phục Hổ Côn | 573 → 481 | 72% | 70.0% → 82.0% | đạt |
| 25 | Hàn Sơn Đại Đệ Tử | Thanh Phong Kiếm → Tử Mẫu Song Đao | 589 → 581 | 62% | 60.0% → 61.0% | đạt |
| 26 | Băng Nguyên Bạch Hổ | Tử Kim Trảo, Xích Sa Chưởng → U Minh Quỷ Trảo, U Minh Quỷ Trảo | 588 → 485 | 72% | 75.0% → 69.0% | đạt |
| 27 | U Minh Chấp Pháp Sứ | Tử Kim Trảo, Xích Sa Chưởng → Thất Tinh Tiêu, Thất Tinh Tiêu | 559 → 667 | 72% | 100.0% → 51.0% | không đạt |
| 28 | Vạn Niên Băng Quy | Tử Kim Trảo, Xích Sa Chưởng → Chấn Nhạc Chùy, Chấn Nhạc Chùy | 596 → 435 | 72% | 70.0% → 80.0% | đạt |
| 29 | U Minh Tuyệt Mệnh Kiếm Vệ | Xích Sa Chưởng → Tử Mẫu Song Đao | 603 → 515 | 72% | 74.0% → 80.0% | đạt |
| 30 | U Minh Tả Sứ Dạ Sát Thiên | Xích Sa Chưởng → U Minh Quỷ Trảo | 648 → 538 | 50% | 42.0% → 38.0% | đạt |
| 31 | Dịch Lộ Đao Khách | Tử Kim Trảo, Tử Kim Trảo → Liệt Diễm Đao, Liệt Diễm Đao | 687 → 532 | 72% | 56.0% → 88.0% | không đạt |
| 32 | Giả Lệnh Dịch Tốt | Tử Kim Trảo, Tử Kim Trảo → Băng Phách Thương, Băng Phách Thương | 666 → 426 | 72% | 70.0% → 79.0% | đạt |
| 33 | Mặc Ảnh Linh Miêu | Thiết Cốt Quyền Sáo → Bạch Hổ Cuồng Móng | 721 → 695 | 72% | 52.0% → 73.0% | đạt |
| 34 | Tế Vân Chưởng Quỹ | Thiết Cốt Quyền Sáo → Ngọc Cốt Phiến | 727 → 698 | 72% | 50.0% → 98.0% | không đạt |
| 35 | U Minh Vũ Y Vệ | Tử Kim Trảo, Tử Kim Trảo → Thất Tinh Tiêu, Thất Tinh Tiêu | 666 → 682 | 62% | 100.0% → 54.0% | đạt |
| 36 | Thiết Phủ Khai Lộ Sứ | Tử Kim Trảo → Khai Sơn Phủ | 735 → 625 | 72% | 74.0% → 93.0% | không đạt |
| 37 | Xích Giác Cuồng Ngưu | Tử Kim Trảo → Thiết Cốt Quyền Sáo | 732 → 657 | 72% | 91.0% → 64.0% | đạt |
| 38 | Hàn Sơn Tín Tiêu Vệ | Tử Kim Trảo → Truy Hồn Cung | 720 → 701 | 72% | 56.0% → 78.0% | đạt |
| 39 | U Minh Khóa Giang Vệ | Tử Kim Trảo → Thanh Đằng Tiên | 744 → 737 | 72% | 84.0% → 63.0% | đạt |
| 40 | U Minh Hữu Sứ Lộ Tẫn Quang | Tử Kim Trảo → U Minh Quỷ Trảo | 792 → 716 | 50% | 34.0% → 41.0% | đạt |
| 41 | Giả Chiếu Cấm Vệ | Tử Kim Trảo → Băng Phách Thương | 767 → 737 | 72% | 63.0% → 54.0% | không đạt |
| 42 | Chu Tước Ảnh Khách | Tử Kim Trảo → Thất Tinh Tiêu | 754 → 851 | 72% | 65.0% → 85.0% | không đạt |
| 43 | Huyền Đồng Cơ Sư | Tử Kim Trảo → Chấn Nhạc Chùy | 753 → 710 | 72% | 72.0% → 62.0% | đạt |
| 44 | Ngụy Học Kiếm Sinh | Tử Kim Trảo → Phán Quan Bút | 834 → 671 | 72% | 58.0% → 100.0% | không đạt |
| 45 | Thiên Cơ Chấp Ấn Quan | Tử Kim Trảo → Ngọc Cốt Phiến | 836 → 802 | 62% | 69.0% → 64.0% | đạt |
| 46 | U Minh Ám Cừ Sứ | Tử Kim Trảo → U Minh Quỷ Trảo, U Minh Quỷ Trảo | 776 → 577 | 72% | 95.0% → 77.0% | đạt |
| 47 | Xích Nhật Kim Sí Điểu | Tử Kim Trảo → Tử Kim Trảo | 838 → 854 | 72% | 58.0% → 68.0% | đạt |
| 48 | Thái Dịch Cầm Sư | Tử Kim Trảo → Bích Thủy Cầm | 838 → 746 | 72% | 58.0% → 81.0% | đạt |
| 49 | Thiên Cơ Hỏa Nỏ Vệ | Tử Kim Trảo → Truy Hồn Cung | 827 → 733 | 72% | 75.0% → 69.0% | đạt |
| 50 | Giám Chủ Thiên Cơ Tào Huyền Kỳ | Tử Kim Trảo → Chấn Nhạc Chùy | 863 → 619 | 50% | 41.0% → 100.0% | không đạt |

### Số lần dùng vũ khí theo hồi

- hoi_1: Thanh Phong Kiếm: 2; Phục Hổ Côn: 1; Kịch Độc Chu Ngà: 1; Liệt Diễm Đao: 3; Tử Mẫu Song Đao: 1; Thất Tinh Tiêu: 1; Thiết Cốt Quyền Sáo: 1.
- hoi_2: Kịch Độc Chu Ngà: 2; Thất Tinh Tiêu: 3; Liệt Diễm Đao: 1; U Minh Quỷ Trảo: 1; Ngọc Cốt Phiến: 1; Thanh Đằng Tiên: 1; Xích Sa Chưởng: 1.
- hoi_3: Băng Phách Thương: 1; Thanh Phong Kiếm: 1; U Minh Quỷ Trảo: 3; Phục Hổ Côn: 1; Tử Mẫu Song Đao: 2; Thất Tinh Tiêu: 1; Chấn Nhạc Chùy: 1.
- hoi_4: Liệt Diễm Đao: 1; Băng Phách Thương: 1; Bạch Hổ Cuồng Móng: 1; Ngọc Cốt Phiến: 1; Thất Tinh Tiêu: 1; Khai Sơn Phủ: 1; Thiết Cốt Quyền Sáo: 1; Truy Hồn Cung: 1; Thanh Đằng Tiên: 1; U Minh Quỷ Trảo: 1.
- hoi_5: Băng Phách Thương: 1; Thất Tinh Tiêu: 1; Chấn Nhạc Chùy: 2; Phán Quan Bút: 1; Ngọc Cốt Phiến: 1; U Minh Quỷ Trảo: 1; Tử Kim Trảo: 1; Bích Thủy Cầm: 1; Truy Hồn Cung: 1.

### Các ải ngoài dải

- Ải 9: 59.0% (dải 60–84%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 16: 56.0% (dải 60–84%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 18: 88.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 20: 34.0% (dải 38–62%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 23: 54.0% (dải 60–84%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 27: 51.0% (dải 60–84%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 31: 88.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 34: 98.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 36: 93.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 41: 54.0% (dải 60–84%), quá khó; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 42: 85.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 44: 100.0% (dải 60–84%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.
- Ải 50: 100.0% (dải 38–62%), quá dễ; các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả.

### Biến động lớn nhất

- LC: ải 13, 468 → 156 (-312). LC chỉ được báo cáo, không dùng làm ràng buộc.
- Tỉ lệ thắng: ải 2, 3.0% → 82.0% (+79.0 điểm).

### Tệp

- Ứng viên chưa áp dụng: `tools/codex/out/pve-variety/chapters.json`.
- Số đo xác nhận: `tools/codex/out/pve-variety/tables/verification.json`.
- Script dò/tinh chỉnh Round 2: `tools/codex/pve_enemy_variety_round2.py`, `tools/codex/pve_enemy_variety_refine.py`.
- Dữ liệu chính `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: không đổi.
- `tools/codex/out/pve-variety/chapters-before.json`: không tạo vì chưa đạt 45/49.

## Round 3 — tinh chỉnh 13 ải ngoài dải và áp dụng

Round 3 chỉ thay `enemy.items` của 13 ải đã chỉ định; 36 ải còn lại giữ nguyên đúng dữ liệu ứng viên
Round 2 và không đo lại. Mỗi phương án được dò với 20 trận, sau đó xác nhận đúng 50 trận. Kết quả cuối là
**45/49 ải trong dải**, đạt ngưỡng áp dụng; ải 50 đạt **52%**, nằm trong dải Thủ Lĩnh 38–62%.

| Ải | Địch | Vũ khí Round 2 → Round 3 | LC Round 2 → Round 3 | Mục tiêu | Thắng Round 2 → Round 3 | Kết quả |
|---:|---|---|---:|---:|---:|---|
| 9 | Ba Hổ Tiên Phong Vệ | Thanh Phong Kiếm ×2 → Thanh Phong Kiếm ×2 | 342 → 342 | 72% | 59% → 70% | đạt |
| 16 | Hắc Sát Độc Hùng | U Minh Quỷ Trảo → U Minh Quỷ Trảo | 382 → 355 | 72% | 56% → 64% | đạt |
| 18 | Huyền Độc Trưởng Lão | Ngọc Cốt Phiến ×2 → Ngọc Cốt Phiến ×2 | 537 → 539 | 72% | 88% → 85% | không đạt |
| 20 | Hắc Độc Nương Tử | Xích Sa Chưởng → Xích Sa Chưởng | 494 → 493 | 50% | 34% → 41% | đạt |
| 23 | Tuyết Lĩnh Ngân Lang | U Minh Quỷ Trảo ×2 → U Minh Quỷ Trảo ×2 | 442 → 440 | 72% | 54% → 62% | đạt |
| 27 | U Minh Chấp Pháp Sứ | Thất Tinh Tiêu ×2 → Thất Tinh Tiêu ×2 | 667 → 664 | 72% | 51% → 55% | không đạt |
| 31 | Dịch Lộ Đao Khách | Liệt Diễm Đao ×2 → Liệt Diễm Đao ×2 | 532 → 534 | 72% | 88% → 79% | đạt |
| 34 | Tế Vân Chưởng Quỹ | Ngọc Cốt Phiến → Xích Sa Chưởng | 698 → 738 | 72% | 98% → 63% | đạt |
| 36 | Thiết Phủ Khai Lộ Sứ | Khai Sơn Phủ → Chấn Nhạc Chùy | 625 → 580 | 72% | 93% → 84% | đạt |
| 41 | Giả Chiếu Cấm Vệ | Băng Phách Thương → Băng Phách Thương | 737 → 736 | 72% | 54% → 58% | không đạt |
| 42 | Chu Tước Ảnh Khách | Thất Tinh Tiêu → Thất Tinh Tiêu | 851 → 853 | 72% | 85% → 80% | đạt |
| 44 | Ngụy Học Kiếm Sinh | Phán Quan Bút → Thanh Phong Kiếm ×2 | 671 → 743 | 72% | 100% → 98% | không đạt |
| 50 | Giám Chủ Thiên Cơ Tào Huyền Kỳ | Chấn Nhạc Chùy → Chấn Nhạc Chùy ×2 | 619 → 660 | 50% | 100% → 52% | đạt |

Bốn ải còn ngoài dải là 18 (85%, cao hơn 1 điểm), 27 (55%, thấp hơn 5 điểm), 41 (58%, thấp hơn
2 điểm) và 44 (98%, cao hơn 14 điểm). Điều kiện Round 3 chỉ yêu cầu ít nhất 45/49 và bắt buộc ải 50 đạt;
cả hai điều kiện đều thỏa.

### Số lần dùng vũ khí cuối cùng theo hồi

- hoi_1: Thanh Phong Kiếm: 2; Phục Hổ Côn: 1; Kịch Độc Chu Nga: 1; Liệt Diễm Đao: 3; Tử Mẫu Song Đao: 1; Thất Tinh Tiêu: 1; Thiết Cốt Quyền Sáo: 1.
- hoi_2: Kịch Độc Chu Nga: 2; Thất Tinh Tiêu: 3; Liệt Diễm Đao: 1; U Minh Quỷ Trảo: 1; Ngọc Cốt Phiến: 1; Thanh Đằng Tiên: 1; Xích Sa Chưởng: 1.
- hoi_3: Băng Phách Thương: 1; Thanh Phong Kiếm: 1; U Minh Quỷ Trảo: 3; Phục Hổ Côn: 1; Tử Mẫu Song Đao: 2; Thất Tinh Tiêu: 1; Chấn Nhạc Chùy: 1.
- hoi_4: Liệt Diễm Đao: 1; Băng Phách Thương: 1; Bạch Hổ Cuồng Móng: 1; Xích Sa Chưởng: 1; Thất Tinh Tiêu: 1; Chấn Nhạc Chùy: 1; Thiết Cốt Quyền Sáo: 1; Truy Hồn Cung: 1; Thanh Đằng Tiên: 1; U Minh Quỷ Trảo: 1.
- hoi_5: Băng Phách Thương: 1; Thất Tinh Tiêu: 1; Chấn Nhạc Chùy: 2; Thanh Phong Kiếm: 1; Ngọc Cốt Phiến: 1; U Minh Quỷ Trảo: 1; Tử Kim Trảo: 1; Bích Thủy Cầm: 1; Truy Hồn Cung: 1.

Không hồi nào dùng một vũ khí quá 3 ải; không có hai ải liên tiếp dùng cùng vũ khí; mỗi địch có tối đa
2 bản sao của một vũ khí; ải 1 giữ nguyên.

### Biến động lớn nhất và tệp áp dụng

- LC Round 2 → Round 3 lớn nhất: ải 44, 671 → 743 (+72); chiều giảm lớn nhất: ải 36, 625 → 580 (-45).
- Tỉ lệ thắng Round 2 → Round 3 lớn nhất: ải 50, 100% → 52% (-48 điểm); kế tiếp là ải 34, 98% → 63% (-35 điểm).
- Ứng viên cuối: `tools/codex/out/pve-variety/chapters.json`.
- Bản sao dữ liệu game trước khi áp dụng: `tools/codex/out/pve-variety/chapters-before.json`.
- Số đo xác nhận Round 3: `tools/codex/out/pve-variety/tables/round3-verification.json`.
- Ứng viên đã được chép vào `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`; SHA-256 của bản game và ứng viên trùng nhau.
