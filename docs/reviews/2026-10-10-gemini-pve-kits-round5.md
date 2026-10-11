# Báo cáo Cân bằng PvE Round 5 (Hồi 2–5): Exploits, Bag Chuẩn và Enemy Kits

## 1. Kiểm tra Lỗ hổng Cân bằng (Exploit Check)
- **Phát hiện exploit:** CÓ
- **Chi tiết kết quả đối đầu (40 trận mỗi cặp):**
  - **Ải 20:**
    - Greedy bag (13 món) vs Enemy: 0.0%
    - Typical bag (12 món) vs Enemy: 0.0%
    - Greedy vs Typical head-to-head: 78.8%
  - **Ải 35:**
    - Greedy bag (17 món) vs Enemy: 0.0%
    - Typical bag (16 món) vs Enemy: 0.0%
    - Greedy vs Typical head-to-head: 0.0%
  - **Ải 45:**
    - Greedy bag (19 món) vs Enemy: 2.5%
    - Typical bag (17 món) vs Enemy: 0.0%
    - Greedy vs Typical head-to-head: 97.5%

**Phân tích nguyên nhân Exploit:**
Thuật toán `select_bag` cũ (`greedy`) ưu tiên xếp các món Công Pháp 1x1 (như *Thái Thanh Tâm Kinh*, *U Minh Bí Lục*, *Bạch Luyện Chân Kinh*) vì chỉ số Lực Chiến trên mỗi ô cực kỳ cao. Khi xếp chồng 10+ Công Pháp 1x1, người chơi tích lũy lượng lớn Giảm Hồi Chiêu và Công mà không tốn ô Bao Phục hay cần trang bị Hộ Cụ, dẫn đến sức mạnh áp đảo (đạt tỉ lệ thắng 78.8% ở Ải 20 và 97.5% ở Ải 45 so với túi đồ chuẩn).
**Đề xuất hướng chỉnh sửa (không áp dụng trong round này):** Điều chỉnh lại trọng số Lực Chiến của Công Pháp trong `power.json` hoặc tăng kích thước ô của các Công Pháp 1x1 lên 2x1/2x2.

## 2. Thông số Túi đồ Chuẩn (Typical Reference Bag)
Bảng thống kê túi đồ chuẩn của người chơi tại các mốc ải tiêu biểu:
| Ải | Số món | Số ô chiếm | Binh Khí | Hộ Cụ | Công Pháp | Kỳ Trân | Đan Dược |
|---|---|---|---|---|---|---|---|
| 15 | 12 | 20/20 | 2 | 1 | 3 | 3 | 3 |
| 25 | 15 | 20/20 | 2 | 2 | 3 | 5 | 3 |
| 35 | 16 | 20/20 | 2 | 1 | 3 | 7 | 3 |
| 45 | 17 | 20/20 | 2 | 1 | 3 | 8 | 3 |

## 3. Bảng Kết quả Cân bằng Enemy Kits (Ải 11–50)
| Ải | Loại | Tên ải | Tên địch | Cấp người | Món người | Món địch | Trọng địch | Độ hiếm | Binh khí chính | Tỉ lệ thắng (%) | Mục tiêu (%) | Đạt chuẩn |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 11 | normal | Man Uyên Nhập Khẩu | Vạn Độc Môn Đồ | 12 | 10 | 11 | 2-3 | common/uncommon | `kich_doc_chu_nga` | 65.0% | 72% (60-84) | Đạt |
| 12 | normal | Ô Đầm Nhai | Huyền Độc Môn Đồ | 13 | 10 | 12 | 2-3 | common | `that_tinh_tieu` | 61.2% | 72% (60-84) | Đạt |
| 13 | normal | Hủ Cốt Thung | Kịch Độc Tri Chu | 13 | 10 | 11 | 2-3 | rare/uncommon | `kich_doc_chu_nga` | 65.0% | 72% (60-84) | Đạt |
| 14 | normal | Xà Khẩu Hiệp | Man Cương Vu Đao | 14 | 11 | 12 | 2-3 | common | `liet_diem_dao` | 80.0% | 72% (60-84) | Đạt |
| 15 | elite | U Cốc Dược Đàn | Bách Thảo Nghịch Đồ | 15 | 12 | 10 | 1-2 | common | `that_tinh_tieu` | 100.0% | 62% (50-74) | KHÔNG |
| 16 | normal | Mê Vụ Đàm | Hắc Sát Độc Hùng | 15 | 11 | 9 | 1-2 | common | `u_minh_quy_trao` | 100.0% | 72% (60-84) | KHÔNG |
| 17 | normal | Lam Hoa Trạch | U Minh Ám Sát Thí Tử | 16 | 12 | 10 | 1-2 | common | `that_tinh_tieu` | 100.0% | 72% (60-84) | KHÔNG |
| 18 | normal | Độc Long Khê | Huyền Độc Trưởng Lão | 17 | 12 | 12 | 1-2 | common/uncommon | `ngoc_cot_phien` | 65.0% | 72% (60-84) | Đạt |
| 19 | normal | Vạn Xà Quật | Xích Luyện Xà Tinh | 17 | 12 | 10 | 1-2 | common | `thanh_dang_tien` | 100.0% | 72% (60-84) | KHÔNG |
| 20 | boss | Thần Bí Cổ Đàn | Hắc Độc Nương Tử | 18 | 12 | 12 | 1-2 | rare/uncommon | `xich_sa_chuong` | 55.0% | 50% (38-62) | Đạt |
| 21 | normal | Tuyết Dốc Băng Khẩu | Hàn Phong Tuần Tuyết Giả | 18 | 12 | 10 | 2-3 | common | `bang_phach_thuong` | 46.2% | 72% (60-84) | KHÔNG |
| 22 | normal | Phong Nhận Trầm Khê | Hàn Sơn Kiếm Sĩ | 19 | 12 | 11 | 2-3 | common/uncommon | `thanh_phong_kiem` | 75.0% | 72% (60-84) | Đạt |
| 23 | normal | Tuyết Lĩnh Hàn Phong | Tuyết Lĩnh Ngân Lang | 19 | 14 | 12 | 1-2 | uncommon | `u_minh_quy_trao` | 100.0% | 72% (60-84) | KHÔNG |
| 24 | normal | Băng Hà Cổ Nhai | Kim Cang Hộ Giới Tăng | 20 | 15 | 13 | 4-5 | uncommon | `phuc_ho_con` | 72.5% | 72% (60-84) | Đạt |
| 25 | elite | Phong Vân Băng Đài | Hàn Sơn Đại Đệ Tử | 20 | 15 | 15 | 2-3 | uncommon | `tu_mau_song_dao` | 0.0% | 62% (50-74) | KHÔNG |
| 26 | normal | Bạch Hổ Tuyệt Hạp | Băng Nguyên Bạch Hổ | 21 | 13 | 11 | 2-3 | common | `bach_ho_cuong_mong` | 100.0% | 72% (60-84) | KHÔNG |
| 27 | normal | Hàn Tinh Thạch Thất | U Minh Chấp Pháp Sứ | 21 | 15 | 16 | 7-8 | uncommon | `that_tinh_tieu` | 0.0% | 72% (60-84) | KHÔNG |
| 28 | normal | Phong Ma Cổ Đạo | Vạn Niên Băng Quy | 22 | 14 | 13 | 1-2 | common | `chan_nhac_chuy` | 0.0% | 72% (60-84) | KHÔNG |
| 29 | normal | Vọng Nguyệt Băng Đỉnh | U Minh Tuyệt Mệnh Kiếm Vệ | 22 | 12 | 14 | 1-1 | common | `tu_mau_song_dao` | 100.0% | 72% (60-84) | KHÔNG |
| 30 | boss | Phong Thiền Băng Đài | U Minh Tả Sứ Dạ Sát Thiên | 23 | 12 | 14 | 1-1 | uncommon | `u_minh_quy_trao` | 100.0% | 50% (38-62) | KHÔNG |
| 31 | normal | Thanh Dã Xuân Kiều | Dịch Lộ Đao Khách | 24 | 12 | 10 | 2-3 | common | `liet_diem_dao` | 100.0% | 72% (60-84) | KHÔNG |
| 32 | normal | Liễu Âm Cổ Dịch | Giả Lệnh Dịch Tốt | 24 | 16 | 12 | 2-2 | common | `bang_phach_thuong` | 100.0% | 72% (60-84) | KHÔNG |
| 33 | normal | Linh Miêu Trúc Viên | Mặc Ảnh Linh Miêu | 24 | 16 | 19 | 1-1 | uncommon | `bach_ho_cuong_mong` | 33.8% | 72% (60-84) | KHÔNG |
| 34 | normal | Tế Vân Trà Phường | Tế Vân Chưởng Quỹ | 25 | 16 | 19 | 2-2 | common | `xich_sa_chuong` | 0.0% | 72% (60-84) | KHÔNG |
| 35 | elite | Vũ Đình Đoạt Sớ | U Minh Vũ Y Vệ | 25 | 16 | 18 | 7-8 | common | `that_tinh_tieu` | 0.0% | 62% (50-74) | KHÔNG |
| 36 | normal | Hoàng Thổ Quan Đạo | Thiết Phủ Khai Lộ Sứ | 26 | 14 | 16 | 1-1 | common | `khai_son_phu` | 60.0% | 72% (60-84) | Đạt |
| 37 | normal | Hỏa Giác Ngưu Tràng | Xích Giác Cuồng Ngưu | 26 | 14 | 18 | 3-3 | common | `thiet_cot_quyen_sao` | 100.0% | 72% (60-84) | KHÔNG |
| 38 | normal | Vọng Kinh Phong Đài | Hàn Sơn Tín Tiêu Vệ | 27 | 14 | 17 | 3-3 | uncommon | `truy_hon_cung` | 0.0% | 72% (60-84) | KHÔNG |
| 39 | normal | Đông Quách Thủy Môn | U Minh Khóa Giang Vệ | 27 | 15 | 17 | 7-7 | common | `thanh_dang_tien` | 6.2% | 72% (60-84) | KHÔNG |
| 40 | boss | Vân Tân Hắc Chu | U Minh Hữu Sứ Lộ Tẫn Quang | 28 | 16 | 16 | 5-6 | common | `u_minh_quy_trao` | 0.0% | 50% (38-62) | KHÔNG |
| 41 | normal | Kim Minh Ngoại Thành | Giả Chiếu Cấm Vệ | 28 | 17 | 16 | 6-7 | common | `bang_phach_thuong` | 0.0% | 72% (60-84) | KHÔNG |
| 42 | normal | Chu Tước Trường Nhai | Chu Tước Ảnh Khách | 29 | 16 | 18 | 5-6 | rare | `that_tinh_tieu` | 0.0% | 72% (60-84) | KHÔNG |
| 43 | normal | Đồng Sư Cơ Phường | Huyền Đồng Cơ Sư | 29 | 17 | 16 | 7-7 | common | `chan_nhac_chuy` | 0.0% | 72% (60-84) | KHÔNG |
| 44 | normal | Bạch Ngọc Văn Môn | Ngụy Học Kiếm Sinh | 30 | 17 | 16 | 4-4 | uncommon | `thanh_phong_kiem` | 0.0% | 72% (60-84) | KHÔNG |
| 45 | elite | Thừa Thiên Đài Ấn | Thiên Cơ Chấp Ấn Quan | 30 | 17 | 19 | 7-7 | common | `ngoc_cot_phien` | 0.0% | 62% (50-74) | KHÔNG |
| 46 | normal | Ngự Thủy Ám Cừ | U Minh Ám Cừ Sứ | 30 | 17 | 14 | 1-1 | common | `u_minh_quy_trao` | 97.5% | 72% (60-84) | KHÔNG |
| 47 | normal | Kim Sí Yêu Điện | Xích Nhật Kim Sí Điểu | 30 | 17 | 19 | 4-4 | uncommon | `tu_kim_trao` | 0.0% | 72% (60-84) | KHÔNG |
| 48 | normal | Thái Dịch Liên Đình | Thái Dịch Cầm Sư | 30 | 17 | 17 | 7-7 | common | `bich_thuy_cam` | 1.2% | 72% (60-84) | KHÔNG |
| 49 | normal | Xã Tắc Cơ Đàn | Thiên Cơ Hỏa Nỏ Vệ | 30 | 16 | 18 | 5-5 | common | `truy_hon_cung` | 63.8% | 72% (60-84) | Đạt |
| 50 | boss | Minh Quang Thiên Cơ Điện | Giám Chủ Thiên Cơ Tào Huyền Kỳ | 30 | 16 | 14 | 5-5 | common | `chan_nhac_chuy` | 0.0% | 50% (38-62) | KHÔNG |

- **Tổng số ải đạt chuẩn:** 10/40 (Yêu cầu: ≥ 36)
- **Toàn bộ Boss Hồi 2–5 đạt chuẩn:** KHÔNG

## 4. Kiểm tra Quy tắc (Rule Check) & Cập nhật Dữ liệu
- **Vi phạm quy tắc:**
  - Stage 23: Missing full kit (weapon/armor/support)
  - Stage 24: Missing full kit (weapon/armor/support)
  - Stage 27: Missing full kit (weapon/armor/support)
  - Stage 28: Missing full kit (weapon/armor/support)
  - Stage 29: Missing full kit (weapon/armor/support)
  - Stage 30: Missing full kit (weapon/armor/support)
  - Stage 32: Missing full kit (weapon/armor/support)
  - Stage 33: Missing full kit (weapon/armor/support)
  - Stage 34: Missing full kit (weapon/armor/support)
  - Stage 35: Missing full kit (weapon/armor/support)
  - Stage 38: Missing full kit (weapon/armor/support)
  - Stage 39: Missing full kit (weapon/armor/support)
  - Stage 40: Missing full kit (weapon/armor/support)
  - Stage 41: Missing full kit (weapon/armor/support)
  - Stage 43: Missing full kit (weapon/armor/support)
  - Stage 44: Missing full kit (weapon/armor/support)
  - Stage 45: Missing full kit (weapon/armor/support)
  - Stage 46: Missing full kit (weapon/armor/support)
  - Stage 47: Missing full kit (weapon/armor/support)
  - Stage 48: Missing full kit (weapon/armor/support)
  - Stage 49: Missing full kit (weapon/armor/support)
  - Stage 50: Missing full kit (weapon/armor/support)
- **Cổng kiểm duyệt (Gate Pass):** KHÔNG ĐẠT
- **Trạng thái áp dụng game file (`chapters.json`):** KHÔNG SỬA ĐỔI
