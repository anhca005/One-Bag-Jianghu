# Thiết Kế Hệ Thống Cấp Độ Hiệp Khách (Hero Level System) — Nhất Nang Giang Hồ

**Căn cứ:** Quyết định #12 (Đột Phá), #15 (Tâm Pháp), #20 (dựng sẵn), #21 (không phái khi tạo tài khoản, mở Môn Phái ở cấp 10), #23 (túi cố định 5×4, không thể lực offline, Tàng Bảo Các từ cấp 1), #24 (phẩm chất theo bản sao).  
**Nhân vật mẫu:** "Vân Du". Cấp tối đa: **Cấp 30**. Kích thước Bao Phục: **Cố định 5×4 (20 ô)** xuyên suốt trò chơi.

---

## Bảng Thuật Ngữ Đề Xuất (Chờ Duyệt)

| Tên Đề Xuất | Mã | Ý Nghĩa / Mục Đích |
|---|---|---|
| Cấp Hiệp Khách | `hero_level` | Cấp độ trưởng thành nhân vật, mở khóa tính năng và phần thưởng tài nguyên |
| Điểm Kinh Nghiệm | `exp` | Điểm tích lũy qua thực chiến PvE/PvP để thăng cấp Hiệp Khách |
| Thăng Cấp Hiệp Khách | `hero_level_up` | Đạt đủ kinh nghiệm sang cấp mới (thay thế cụm "Đột Phá Cảnh Giới") |
| Sức Chứa Kho | `storage_capacity`| Dung lượng lưu trữ trang bị trong Tàng Bảo Các |
| Bái Sư | `join_sect` | Chọn gia nhập 1 trong 7 môn phái tại Cấp 10 |
| Xuất Sư | `change_sect`| Đổi sang môn phái khác (tốn Bạc và thời gian chờ 24h) |

---

## 1. Nguồn Tích Lũy Điểm Kinh Nghiệm (EXP Sources)

### 1.1. Hành Tẩu Giang Hồ (PvE)
Không sử dụng Thể Lực (bản offline tự do trải nghiệm). EXP gắn liền với vượt ải:

| Khu Vực | Thắng Lần Đầu (3 Sao) | Đánh Lại Thắng (Replay) | Thua Trận (Khuyến Khích) |
|---|---|---|---|
| **Hồi 1: Ải 1–9** | 60 EXP | 20 EXP | 5 EXP |
| **Hồi 1: Ải 10 (Boss)** | 150 EXP | 40 EXP | 10 EXP |
| **Hồi 2: Ải 1–9** | 100 EXP | 35 EXP | 10 EXP |
| **Hồi 2: Ải 10 (Boss)** | 250 EXP | 70 EXP | 15 EXP |
| **Hồi 3+ (Các Hồi sau)** | +25% EXP / Hồi | +25% EXP / Hồi | Cố định 15 EXP |

### 1.2. Luận Kiếm Đài (PvP)
Tích lũy qua giao đấu bất đồng bộ (mở tại Cấp 5):
- **Thắng trận:** `35 EXP`.
- **Hòa trận:** `20 EXP`.
- **Bại trận:** `10 EXP` (an ủi, tránh ức chế).

### 1.3. Vật phẩm tăng EXP
**Quy tắc:** `Không có Đan Dược tăng EXP`. Giữ nhịp độ thăng tiến thuần túy qua thực chiến và xếp túi.

---

## 2. Đường Cong Cấp Độ (Level Curve 1 → 30)

- Giả định: 20–25 trận/giờ. Đạt **Cấp 10** sau khoảng **1.5 giờ** chơi thực tế (~30–35 trận).

| Cấp Độ | EXP Lên Cấp Kế | EXP Tích Lũy | Ước Tính Trận | Thời Gian Ước Tính |
|:---:|:---:|:---:|:---:|:---:|
| **1** | 50 | 0 | Khởi đầu | 0 phút |
| **2** | 70 | 50 | 1 trận (Ải 1) | 3 phút |
| **3** | 100 | 120 | 2 trận | 8 phút |
| **4** | 130 | 220 | 4 trận | 15 phút |
| **5** | 160 | 350 | 6 trận | 25 phút |
| **6** | 200 | 510 | 9 trận | 38 phút |
| **7** | 240 | 710 | 13 trận | 52 phút |
| **8** | 290 | 950 | 18 trận | 1 giờ 10 phút |
| **9** | 350 | 1,240 | 24 trận | 1 giờ 25 phút |
| **10** | 420 | 1,590 | 32 trận (Mở Phái) | **1 giờ 45 phút** |
| **15** | 800 | 4,500 | ~75 trận | 4 giờ |
| **20** | 1,400 | 10,200 | ~150 trận | 8 giờ |
| **25** | 2,200 | 19,500 | ~260 trận | 14 giờ |
| **30 (Max)** | — | 32,000 | ~420 trận | 22–25 giờ |

---

## 3. Phần Thưởng & Tính Năng Mở Khóa Theo Cấp

Bao Phục giữ **cố định 5×4 (20 ô)**. Thăng cấp thưởng tính năng, Bạc, kho chứa đồ và bản sao trang bị phẩm chất cao tại cột mốc:

| Cấp | Tính Năng Mở Khóa | Sức Chứa Kho | Phần Thưởng Tài Nguyên & Trang Bị Cột Mốc |
|:---:|---|:---:|---|
| **1** | Hành Tẩu Giang Hồ, **Tàng Bảo Các** | 30 ô | Khởi đầu giang hồ, cấp sẵn túi 5×4 cố định |
| **3** | Vạn Bảo Phổ (`codex`) | 35 ô | 500 Bạc, 10 `han_thiet` |
| **5** | **Luận Kiếm Đài** (PvP tân thủ) | 40 ô | 1,000 Bạc, 1 `dot_pha_thach_so` |
| **7** | Thương Hội (`shop`) | 45 ô | 1,500 Bạc, 15 `han_thiet`, 15 `tam_to` |
| **10**| **Gia Nhập Môn Phái (Tâm Pháp 1)** | 50 ô | **Rương Binh Khí Lương Phẩm** (uncommon), 3,000 Bạc |
| **12**| Mở tính năng Đổi Vật Liệu Thương Hội | 55 ô | 3,500 Bạc, 10 `o_thiet`, 1 `dot_pha_thach_trung` |
| **15**| Cho phép nâng Tâm Pháp Cấp 3 | 60 ô | 5,000 Bạc, 20 `o_thiet`, 20 `linh_chi_phan` |
| **20**| Cho phép nâng Tâm Pháp Cấp 6 | 70 ô | **Rương Binh Khí Tinh Phẩm** (rare), 8,000 Bạc, 2 `dot_pha_thach_cao` |
| **25**| Mở tầng cao Luận Kiếm Đài | 80 ô | 12,000 Bạc, 15 `thien_ngoai_van_thiet` |
| **30**| Cho phép nâng Tâm Pháp Cấp 9 | 100 ô | **Rương Tuyệt Phẩm Tự Chọn** (epic), Danh hiệu "Đại Tông Sư" |

---

## 4. Cơ Chế Bái Sư (Cấp 10) & Nâng Cấp Tâm Pháp

### 4.1. Quy trình bái nhập môn phái tại Cấp 10
1. Tại Cấp 10, biểu tượng Hotspot **Môn Phái** mở khóa, phát sáng son đỏ.
2. Mở giao diện **Bái Nhập Sơn Môn**: Hiển thị 7 đại môn phái, xem trước hệ vũ khí, 4 tầng Tâm Pháp (1/3/6/9) và lối đánh.
3. Xác nhận bái nhập: Kích hoạt ngay **Tâm Pháp Cấp 1** (Miễn phí).
4. Sau khi gia nhập, Hotspot trở thành **Tâm Pháp Đường** quản lý nâng cấp và đổi phái.

### 4.2. Luật đổi môn phái (Xuất Sư)
- Tiêu hao `5,000 Bạc` + Thời gian giãn cách `24 giờ`.
- Cấp Tâm Pháp của môn phái cũ được **bảo lưu nguyên vẹn** khi quay lại.

### 4.3. Bảng tài nguyên nâng mốc Tâm Pháp (ID vật liệu chuẩn)

| Mốc Tâm Pháp | Yêu Cầu Cấp Hiệp Khách | Chi Phí Bạc | Vật Liệu Bắt Buộc (Mã Chuẩn) | Hiệu Quả Nhận Được |
|:---:|:---:|:---:|---|---|
| **Cấp 1** | Cấp 10 (Bái sư) | Miễn phí | Không tốn | +10% Lực Đạo cho hệ binh khí phái |
| **Cấp 3** | Cấp 15 | 3,000 Bạc | 15 `han_thiet` | Bước tăng chỉ số phụ (Hội Tâm/Tốc) |
| **Cấp 6** | Cấp 20 | 10,000 Bạc | 25 `o_thiet` + 5 `thien_ngoai_van_thiet` | Mở cơ chế kích hoạt riêng của phái |
| **Cấp 9** | Cấp 30 | 30,000 Bạc | 50 `thien_ngoai_van_thiet` + 10 `binh_hon_tinh_thach` | Tuyệt kỹ tối thượng của môn phái |

---

## 5. Cân Bằng & Bảo Vệ PvP Dưới Cấp 10

- **Mở Luận Kiếm Đài ở Cấp 5:** Người chơi làm quen PvP sớm trước khi bái phái.
- **Quy tắc bảo vệ tân thủ (Cấp 5–9):**
  1. *Bộ lọc đối thủ:* Chỉ ghép với ghost của người chơi Cấp 5–9 (chưa gia nhập môn phái nào).
  2. *Bộ ngắt dự phòng:* Nếu thiếu ghost cùng cấp, hệ thống tự động **vô hiệu hóa Tâm Pháp** của đối thủ trong trận đấu.
  3. Từ Cấp 10 trở lên: Ghép chuẩn ELO và biên độ Chiến Uy $\pm 15\%$.

---

## 6. Cấu Trúc Dữ Liệu (Data Format JSON)

### 6.1. Bảng cấp độ: `tools/gemini/out/economy/hero_levels.json`
```json
[
  { "level": 1,  "expToNext": 50,  "bagCols": 5, "bagRows": 4, "storageSlots": 30, "unlock": "pve_and_storage" },
  { "level": 3,  "expToNext": 100, "bagCols": 5, "bagRows": 4, "storageSlots": 35, "unlock": "codex" },
  { "level": 5,  "expToNext": 160, "bagCols": 5, "bagRows": 4, "storageSlots": 40, "unlock": "arena" },
  { "level": 7,  "expToNext": 240, "bagCols": 5, "bagRows": 4, "storageSlots": 45, "unlock": "shop" },
  { "level": 10, "expToNext": 420, "bagCols": 5, "bagRows": 4, "storageSlots": 50, "unlock": "sect" },
  { "level": 15, "expToNext": 800, "bagCols": 5, "bagRows": 4, "storageSlots": 60, "unlock": "tam_phap_3" },
  { "level": 20, "expToNext": 1400,"bagCols": 5, "bagRows": 4, "storageSlots": 70, "unlock": "tam_phap_6" },
  { "level": 25, "expToNext": 2200,"bagCols": 5, "bagRows": 4, "storageSlots": 80, "unlock": "arena_high" },
  { "level": 30, "expToNext": 0,   "bagCols": 5, "bagRows": 4, "storageSlots": 100,"unlock": "tam_phap_9" }
]
```

### 6.2. Trường lưu trữ trong hồ sơ người chơi (`GameSession`)
- `heroLevel` (int, mặc định 1)
- `heroExp` (int, mặc định 0)
- `storageCapacity` (int, mặc định 30)
- `sectId` (string, rỗng khi cấp < 10)
- `tamPhapLevel` (int, mặc định 0; khi bái sư lên 1)
- `sectProgress` (Dictionary `<string, int>`: lưu cấp Tâm Pháp từng phái)
- `lastSectChangeTime` (long: timestamp lần cuối đổi phái)

---

## 7. Đặc Tả Giao Diện (UI Specifications)

### 7.1. Cụm thanh kinh nghiệm Header
- **Vị trí:** Cạnh tên Hiệp Khách trên thanh Header sảnh chính.
- **Nội dung hiển thị:** `[Vân Du • Cấp 1] [======== 35/50 ========] (70%)`.
- **Hiệu ứng:** Mực vàng óng khi nhận EXP; nhấp nháy ánh son đỏ khi thăng cấp.

### 7.2. Popup Thăng Cấp Hiệp Khách (`HeroLevelUpPopup` — 600×450)
- **Tựa đề đại tự:** "ĐĂNG PHONG THĂNG CẤP" (Thư pháp son đỏ — tuyệt đối không dùng từ "Đột Phá").
- **Nội dung:**
  - Chữ lớn: `Cấp [X - 1] → Cấp [X]`.
  - Hàng quyền lợi mở khóa: Ví dụ *"Sức chứa kho: 35 ô"*, *"Mở khóa: Luận Kiếm Đài"*, *"Phần thưởng: 1,000 Bạc, 1 Đột Phá Thạch"*.
  - Nút bấm: "Lĩnh Hội" (Son đỏ chính giữa).

### 7.3. Tooltip Hotspot Môn Phái khi còn bị khóa (< Cấp 10)
- **Nội dung:**
  - Tiêu đề: `Môn Phái [Chưa Mở]` (Chữ xám mực).
  - Thân bài: *"Sơn môn mây mù che lối. Hiệp Khách cần đạt Cấp 10 để xuất sư hạ sơn, chọn bái nhập Môn Phái lĩnh hội Tâm Pháp."*

---

## 8. Ghi Nhận Quyết Định & Đóng Câu Hỏi Mở Cũ

1. **Thể Lực:** Đã loại bỏ hoàn toàn theo Quyết định #23; người chơi offline không bị giới hạn số lượt chơi.
2. **Kích thước Bao Phục:** Giữ cố định 5×4 (20 ô); loại bỏ hoàn toàn cơ chế mở rộng túi trong runtime.
3. **Thuật ngữ Đột Phá:** Giữ nguyên tắc độc quyền của Quyết định #12/24 — "Đột Phá" chỉ dành cho mốc Trọng 3/6/9 của trang bị; việc lên cấp nhân vật dùng từ "Đăng Phong Thăng Cấp".
