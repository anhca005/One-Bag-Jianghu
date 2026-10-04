import sys
from pathlib import Path

content = """# Báo Cáo Thiết Kế Nội Dung PvE: Hành Tẩu Giang Hồ (3 Hồi × 10 Ải)

## 1. Cốt Truyện Tổng Thể & Nhân Vật Định Kỳ
- **Thế giới:** Cửu Châu Giang Hồ phân tranh giữa 7 đại phái (Kim Cang, Thái Thanh, Liệt Diễm, Huyền Độc, Hàn Sơn, Thiết Phủ, Bích Thủy). Cổ vật chí bảo *Cửu Cực Càn Khôn Nang* (Huyền Hoàng Thần Nang) bị phân tách thành các mảnh linh phù phong ấn rải rác.
- **Xung đột chính:** U Minh Giáo đồ sát sơn trang, mưu đồ đoạt thần nang độc bá võ lâm.
- **Hành trình nhân vật:** Thiếu niên hiệp khách mang chiếc túi gấm gia truyền (tàn nang) tầm thù, dần thấu triệt chân lý "Tâm tại càn khôn", hợp nhất linh nang. Cốt truyện mở đường vượt thoát Hồi 3 tiến vào Trung Nguyên hoàng cung.
- **Nhân vật định kỳ:**
  1. `hero` (Hiệp Khách): Thiếu niên kiên nghị, mang tàn nang tầm thù cứu thế.
  2. `lao_tieu_phu` (Lục Tiều Phu): Hộ nang ẩn sĩ bến Thanh Khê, dẫn dắt cơ bản Hóa Kình & Cương Khí.
  3. `tieu_linh_nhi` (Bạch Linh Nhi): Thiếu nữ Vạn Hoa Cốc, giải trừ chướng độc Man Uyên.
  4. `doc_co_hanh` (Phong Lôi Khoái Đao): Đao khách kiêu hùng, từ kình địch hóa tri kỷ sinh tử.
  5. `bui_chen` (Nộ Ba Hổ): Trại chủ thủy phỉ Ba Hổ Trại, mãnh tướng bị tà giáo lợi dụng (Boss Hồi 1).
  6. `hac_doc_nuong_tu` (Hắc Độc Nương Tử): Ma nữ vạn xà quật Thập Vạn Man Uyên (Boss Hồi 2).
  7. `da_sat_thien` (U Minh Tả Sứ): Đại phản diện tuyết sơn, chấp chưởng Băng Phách Thương (Boss Hồi 3).

## 2. Bản Sắc 3 Hồi
| Hồi | Bối Cảnh & Tiết Khí | Màu Sắc UI (Accent / Paper / Ink) | Phân Bổ Kẻ Địch | Cơ Chế Giáo Dục |
|---|---|---|---|---|
| **Hồi 1: Thanh Khê Trấn** | Sông nước mùa thu phong đỏ, cầu gỗ quán trà sương khói | `#8B2E1F` / `#EFE4CC` / `#2B2420` | Thảo khấu, Liệt Diễm, Thái Thanh (7); Sói, Heo rừng (2); Boss Bùi Chấn | Cơ bản: Hóa Kình & Cương Khí |
| **Hồi 2: Thập Vạn Man Uyên** | Đầm lầy nhiệt đới, rêu phong cổ thụ, sương độc tím | `#1B5E3B` / `#E5EAD8` / `#1A241C` | Man tộc, Huyền Độc, Bích Thủy (7); Nhện, Gấu, Xà tinh (2); Boss Độc Nương Tử | Trạng thái (Độc/Chảy Máu) & Bộc Phát |
| **Hồi 3: Hàn Nguyệt Cô Phong** | Núi tuyết mùa đông, vực băng nghìn trượng, hàn phong | `#2C5282` / `#E6EDF5` / `#16202E` | Hàn Sơn, Kim Cang, U Minh (7); Bạch Lang, Bạch Hổ, Băng Quy (2); Boss Dạ Sát Thiên | Khống chế (Choáng/Làm chậm) & Bảo Vật |

## 3. Danh Sách 30 Ải & Kiểm Chứng Cân Bằng Độ Khó
- Độ khó mô phỏng chuẩn xác qua lệnh `balance.py vs --runs 100`.
- Ải 1–3 đánh bại hoàn toàn bằng túi khởi đầu (`starter.json`).
- Boss Hồi 3 đòi hỏi trang bị Trọng 4–5 Tinh Phẩm (T4: 26.5%, T5: 95.0%).

| Ải | Tên Ải | Loại | Kẻ Địch Chính | Sức Mạnh Người Chơi Dự Kiến | Tỷ Lệ Thắng |
|---|---|---|---|---|---|
| 01 | Trấn Khẩu Sơn Đạo | Thường | Thảo Khấu Tiên Phong (HP 750) | Khởi đầu: 3 món T1 Phổ Thông (starter) | 100.0% |
| 02 | Phong Diệp Bến Đò | Thường | Thủy Phỉ Thám Tử (HP 850) | Khởi đầu: 3 món T1 Phổ Thông (starter) | 100.0% |
| 03 | Hòe Thụ Thôn Ngoại | Thường | Hoang Dã Sói Xám (HP 900) | Khởi đầu: 3 món T1 Phổ Thông (starter) | 100.0% |
| 04 | Thanh Khê Trà Thất | Thường | Liệt Diệm Bang Chúng (HP 980) | T1 Lương Phẩm (T1 Uncommon) | 100.0% |
| 05 | Phong Ba Đình | Tinh Anh | Độc Cô Hạnh (HP 1100, TP 1) | T1 Lương/Vi Phẩm hỗn hợp | 78.0% |
| 06 | Ngô Đồng Cổ Hạng | Thường | Ám Hạng Thích Khách (HP 1050) | T2 Lương Phẩm (T2 Uncommon) | 100.0% |
| 07 | Thủy Tạ Nguy Kiều | Thường | Thiết Bì Trư Thú (HP 1150) | T2 Lương Phẩm tối ưu | 100.0% |
| 08 | Phong Hỏa Cổ Đài | Thường | Liệt Diệm Hương Chủ (HP 1180) | T2 Vi Phẩm (T2 Rare) | 100.0% |
| 09 | Ba Hổ Trại Tiền Doanh | Thường | Ba Hổ Tiên Phong Vệ (HP 1220) | T2 Vi Phẩm (T2 Rare) | 83.0% |
| 10 | Bến Bạc Phân Tranh | Boss | Nộ Ba Hổ Bùi Chấn (HP 1350) | T2 Vi Phẩm tối ưu (T2 Rare full build) | 57.0% |
| 11 | Man Uyên Nhập Khẩu | Thường | Man Tộc Thám Lộ Giả (HP 1250) | T2/T3 Hỗn hợp (TP 2) | 100.0% |
| 12 | Ô Đầm Nhai | Thường | Huyền Độc Môn Đồ (HP 1300) | T3 Lương Phẩm (T3 Uncommon) | 100.0% |
| 13 | Hủ Cốt Thung | Thường | Kịch Độc Tri Chu (HP 1350) | T3 Vi Phẩm (T3 Rare) | 100.0% |
| 14 | Xà Khẩu Hiệp | Thường | Man Cương Vu Đao (HP 1250) | T3 Vi Phẩm (T3 Rare) | 100.0% |
| 15 | U Cốc Dược Đàn | Tinh Anh | Vạn Hoa Nghịch Đồ (HP 1380, TP 2) | T3 Vi/Cực Phẩm hỗn hợp | 54.0% |
| 16 | Mê Vụ Đàm | Thường | Hắc Sát Độc Hùng (HP 1500) | T3 Vi/Cực Phẩm (T3 Rare/Epic) | 100.0% |
| 17 | Lam Hoa Trạch | Thường | U Minh Thí Tử (HP 1450) | T3 Cực Phẩm (T3 Epic) | 100.0% |
| 18 | Độc Long Khê | Thường | Huyền Độc Trưởng Lão (HP 1550) | T3 Cực Phẩm (T3 Epic) | 93.5% |
| 19 | Vạn Xà Quật | Thường | Xích Luyện Xà Tinh (HP 1600) | T3 Cực Phẩm tối ưu | 100.0% |
| 20 | Thần Bí Cổ Đàn | Boss | Hắc Độc Nương Tử (HP 1750) | T3/T4 Cực Phẩm (T3/T4 Epic full build) | 89.0% |
| 21 | Tuyết Dốc Băng Khẩu | Thường | Hàn Phong Tuần Tuyết (HP 1600) | T4 Vi/Cực Phẩm (TP 3) | 100.0% |
| 22 | Phong Nhận Trầm Khê | Thường | Hàn Sơn Kiếm Sĩ (HP 1650) | T4 Cực Phẩm (T4 Epic) | 100.0% |
| 23 | Tuyết Lĩnh Hàn Phong | Thường | Tuyết Lĩnh Ngân Lang (HP 1700) | T4 Cực Phẩm (T4 Epic) | 100.0% |
| 24 | Băng Hà Cổ Nhai | Thường | Kim Cang Hộ Giới Tăng (HP 1850) | T4 Cực Phẩm kiên cố | 100.0% |
| 25 | Phong Vân Băng Đài | Tinh Anh | Hàn Sơn Đại Đệ Tử (HP 1950, TP 3) | T4 Cực/Tinh Phẩm (T4 Epic/Legendary) | 99.5% |
| 26 | Bạch Hổ Tuyệt Hạp | Thường | Băng Nguyên Bạch Hổ (HP 1950) | T4 Tinh Phẩm (T4 Legendary, TP 4) | 100.0% |
| 27 | Hàn Tinh Thạch Thất | Thường | U Minh Chấp Pháp Sứ (HP 2000) | T4/T5 Tinh Phẩm (T4/T5 Legendary) | 100.0% |
| 28 | Phong Ma Cổ Đạo | Thường | Vạn Niên Băng Quy (HP 2150) | T5 Tinh Phẩm (T5 Legendary) | 100.0% |
| 29 | Vọng Nguyệt Băng Đỉnh | Thường | U Minh Tuyệt Mệnh Vệ (HP 2100) | T5 Tinh Phẩm (T5 Legendary) | 100.0% |
| 30 | Phong Thiền Băng Đài | Boss | U Minh Tả Sứ Dạ Sát Thiên (HP 2350) | Trọng 5 Tinh Phẩm (T5 Legendary full build) | 95.0% |

## 4. Vật Phẩm Quái Thú (12 Món) & Kết Quả Mô Phỏng Ma Trận
- Danh mục 12 vật phẩm quái: `soi_nanh_canh` (vũ khí), `heo_rung_bap_giap` (áo giáp), `man_xa_doc_tui` (bảo vật), `u_minh_quy_trao` (vũ khí), `kich_doc_chu_nga` (vũ khí), `bang_tam_linh_chau` (bảo vật), `tuyet_lang_bach_bi` (áo giáp), `hac_sat_hung_dam` (đan dược), `bach_ho_cuong_mong` (vũ khí), `hoa_ngiem_xa_cot` (bí kíp), `bang_phach_co_cot` (bảo vật), `thien_nien_quy_giap` (áo giáp).
- **2 Đội hình mẫu người chơi:**
  - Build 1 (`monster_build_1`): Huyền Độc Môn - Huyết Thú Cuồng Trảo (`u_minh_quy_trao`, `kich_doc_chu_nga`, `hac_sat_hung_dam`, `man_xa_doc_tui`, `heo_rung_bap_giap`).
  - Build 2 (`monster_build_2`): Hàn Sơn Phái - Băng Quy Tuyệt Hộ (`thien_nien_quy_giap`, `bang_phach_co_cot`, `bang_tam_linh_chau`, `tuyet_lang_bach_bi`, `han_ngoc_tieu`).

### Bảng Ma Trận Cân Bằng (Verbatim 22 Đội Hình)
```
Loadout                                    T1/TP1     T3/TP1     T6/TP1     T9/TP1
-----------------------------------------------------------------------------------
Bích Thủy Am - Dưỡng Sinh Cầm Âm           63.5%      62.9%      57.5%      64.2% 
Hàn Sơn Phái - Băng Tâm Bút Cung           50.4%      40.6%      35.1%      38.5% 
Huyền Độc Môn - Hắc Huyết Ám Khí           32.1%!     79.6%!     90.4%!     98.0%!
Huyền Độc Môn - Vạn Độc Phệ Huyết          32.1%!     61.0%      84.3%!     96.6%!
Hàn Sơn Phái - Hàn Bích Hợp Bích           67.6%!     38.9%      49.1%      56.0% 
Kim Cang Tự - Kim Cang Thiết Phủ           21.1%!     63.9%      53.8%      58.9% 
Thái Thanh Quán - Thái Thanh Liệt Diễm     54.1%      54.4%      44.8%      41.5% 
Thiết Phủ Bang - Thiết Phủ Liệt Diễm       55.0%      37.6%      53.3%      32.1%!
Kim Cang Tự - Hộ Thể Côn Quyền             36.4%      44.0%      56.5%      67.0%!
Kim Cang Tự - Bách Luyện Thuần Quyền       28.2%!     36.0%      37.4%      76.9%!
Liệt Diễm Đường - Cuồng Đao Liệt Hỏa       76.6%!     62.4%      45.0%      45.6% 
Liệt Diễm Đường - Hỏa Hải Song Đao         73.0%!     65.4%!     64.0%      57.3% 
Huyền Độc Môn - Huyết Thú Cuồng Trảo       60.8%      20.6%!     11.8%!     11.2%!
Hàn Sơn Phái - Băng Quy Tuyệt Hộ            8.9%!     13.7%!     17.1%!     13.7%!
Bích Thủy Am - Thanh Đằng Trường Tiên      59.8%      66.1%!     78.2%!     70.4%!
Hàn Sơn Phái - Hàn Băng Thương Pháp        57.1%      41.1%      27.3%!     24.6%!
Liệt Diễm Đường - Xích Sa Chưởng Kình      57.9%      28.7%!     27.2%!     28.0%!
Thái Thanh Quán - Tiêu Dao Phiến Pháp      45.3%      72.1%!     74.3%!     59.0% 
Thiết Phủ Bang - Chấn Nhạc Chùy Phong      85.6%!     68.5%!     54.9%      39.6% 
Thái Thanh Quán - Kiếm Đạo Hóa Kình        41.3%      66.0%!     73.8%!     73.4%!
Thái Thanh Quán - Kiếm Khí Xuyên Giáp      25.9%!      9.6%!     12.7%!     10.8%!
Thiết Phủ Bang - Bá Khí Khai Sơn           70.7%!     63.4%      52.4%      37.6% 
Outside [35, 65]: 43/88
```

## 5. Thống Kê Tài Nguyên Mỹ Thuật (Art Assets)
Tổng cộng: **39 tài nguyên hình ảnh** độc bản:
- **3 Bản đồ chọn ải:** `map_hoi_1`, `map_hoi_2`, `map_hoi_3` (1920×1080).
- **3 Nền trận chiến:** `bg_battle_hoi_1`, `bg_battle_hoi_2`, `bg_battle_hoi_3` (1920×1080).
- **6 Nút ải bản đồ:** 3 nút ải thường (`node_hoi_1..3`) và 3 nút boss (`node_boss_hoi_1..3`).
- **15 Chân dung nửa người (1024×1536):**
  - 4 Đồng minh / NPC: `hero_portrait`, `npc_lao_tieu_phu`, `npc_tieu_linh_nhi`, `npc_doc_co_hanh`.
  - 3 Boss chính: `boss_bui_chen`, `boss_hac_doc_nuong_tu`, `boss_da_sat_thien` (quay trái).
  - 8 Quái thú dị vật: `monster_soi_xam`, `monster_heo_rung`, `monster_tri_chu`, `monster_gau_den`, `monster_xich_xa`, `monster_ngan_lang`, `monster_bach_ho`, `monster_bang_quy` (quay trái).
  - *(Tái sử dụng 3 chân dung generic `enemy_portrait_1..3` cho võ nhân giang hồ).*
- **12 Icon vật phẩm quái:** `soi_nanh_canh` .. `thien_nien_quy_giap` (512×512).

## 6. Câu Hỏi Mở (Tối Đa 5 Câu)
1. **Cơ chế Thể Lực:** Mức tiêu hao 6 Thể Lực (ải thường), 8 (tinh anh), 10 (boss) đã tối ưu với giới hạn hồi Thể Lực hàng ngày của GDD chưa?
2. **Tỷ lệ rơi vật phẩm quái:** Hiện thiết lập 10–20% ở ải thường/tinh anh và 25% ở boss; cần điều chỉnh để kiểm soát tiến độ cày cuốc không?
3. **Mở khóa Môn Phái người chơi:** Người chơi nên bắt đầu không phái (Tâm Pháp trung lập) ở Hồi 1 rồi gia nhập môn phái ở Hồi 2, hay chọn phái ngay từ đầu?
4. **Hệ thống 3 sao:** Tiêu chí giữ ≥ 70% Khí Huyết để đạt 3 sao có quá khắc nghiệt trước các boss dồn sát thương bộc phát như Bùi Chấn hay Dạ Sát Thiên?
5. **Độ khó Boss Hồi 3:** Dạ Sát Thiên hiện chặn đứng hoàn toàn người chơi dưới Trọng 4 (thắng 0–5%), người chơi bắt buộc phải rèn đủ Trọng 5 Tinh Phẩm (thắng 95%) — độ khó này có cần nới lỏng thêm cho người chơi casual?
"""

report_path = Path("docs/reviews/2026-10-01-gemini-pve-content.md")
report_path.parent.mkdir(parents=True, exist_ok=True)
lines = content.strip().splitlines()
print(f"Total lines: {len(lines)}")
assert len(lines) <= 300, f"Report line count exceeds 300: {len(lines)}"

with open(report_path, "w", encoding="utf-8") as f:
    f.write(content.strip() + "\n")
print(f"Successfully wrote {report_path} with {len(lines)} lines.")
