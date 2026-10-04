# Báo Cáo Kiểm Tra Toàn Diện Văn Bản (Text Audit) - Nhất Nang Giang Hồ

**Ngày:** 2026-10-08  
**Biên tập viên:** Gemini (Content Editor)  
**Tình trạng:** Hoàn tất rà soát toàn bộ Data (`.json`), Code (`Scripts/**/*.cs`), và Builder (`Editor/Building/*.cs`).

---

## 1. Bảng Tổng Hợp Lỗi Theo Phân Loại

| Phân loại | Số lượng phát hiện | Mức độ nghiêm trọng | Ghi chú chính |
|---|:---:|:---:|---|
| **Raw ID / Tiếng Anh lọt màn hình** | 5 nguồn (15+ biểu hiện) | Cực cao | Lọt id `han_thiet`, enum `QI`, `SHIELD`, `START`, chữ `BOSS`, thông báo lỗi tiếng Anh |
| **Bất nhất thuật ngữ (Terminology)** | 6 nhóm (24+ vị trí) | Cao | Vi phạm glossary: "Sinh Lực" vs "Khí Huyết", "Hồi Phục" vs "Điều Tức", lẫn lộn hoa-thường |
| **Rủi ro tràn khung UI (Length)** | 43 chuỗi | Trung bình - Cao | 7 trang bị, 3 nguyên liệu, 3 tâm pháp, 23 ải/quái PVE (>18 ký tự); 4 nút bấm (>14 ký tự) |
| **Nghi vấn tên Kim Dung (Naming)** | 3 trường hợp | Cần quyết định | `Côn Lôn Tuyết Liên` (phái Côn Lôn), `Phong Ba Đình`, `Xích Luyện Xà Tinh` |
| **Mâu thuẫn cốt truyện (Story)** | 2 trường hợp | Cao | Ải 1-7 tên địch là `Liệt Diễm Bang Tinh Anh` nhưng thoại/mô tả là quái trư `Thiết Bì Trư Thú` |
| **Chất lượng tiếng Việt / Format** | 4 lỗi | Trung bình | Định dạng số lẫn lộn `.` và `,`; câu thoại lủng củng `sức trâu húc... tích nộ` |

---

## 2. Danh Sách Lỗi Nghiêm Trọng Cần Sửa Ngay (Must-Fix)
*Sắp xếp theo thứ tự hiển thị đập vào mắt người chơi nhất:*

1. **Thông báo lỗi Tiếng Anh kèm ID thô khi thiếu nguyên liệu:**
   - Tại `StorageGrid.cs:226` & `ShopService.cs:169`: trả về chuỗi `Insufficient material 'han_thiet'.` hiển thị thẳng lên popup giao dịch của người chơi.
   - Tại `PlayerProfile.cs:436`: lỗi `Insufficient {label} '{cost.Key}': need {cost.Value}...`.
2. **Enum tiếng Anh và trạng thái chưa dịch trong Nhật Ký Chiến Đấu:**
   - Tại `EventTextFormatter.cs:53`: nhánh `default` in thẳng chuỗi enum gốc `START`, `SHIELD`, `QI`, `DOT`, `CHARGE`, `MAX_HP`, `DMG_REDUCE` lên battle log thay vì tiếng Việt wuxia.
   - User đã trực tiếp thấy chữ `QI` xuất hiện trên log thi đấu vì thiếu định dạng cho các sự kiện này.
3. **Hiển thị sai thuật ngữ cốt lõi trên thanh máu võ hiệp:**
   - Tại `FighterPanelView.cs:90` và `ReplayPrefabBuilder.cs:62`: hiển thị `"Sinh Lực 0/0"` thay vì `"Khí Huyết"` (Glossary #70 cấm tiệt từ Sinh Lực, Máu).
4. **Mâu thuẫn nghiêm trọng giữa tên quái và bối cảnh ở Hồi 1 Ải 7:**
   - Tên địch trong data ghi `Liệt Diễm Bang Tinh Anh`, nhưng mô tả ải là `"con quái trư da sắt án ngữ đầu cầu"` và toàn bộ thoại nhân vật đều khẳng định là `"Thiết Bì Trư Thú"`.
5. **Chữ Tiếng Anh `BOSS` trên bản đồ PVE:**
   - Tại `PveStageNode.cs:56`: gán cứng nhãn `kindText.text = "BOSS"`. Cần đổi sang chuẩn kiếm hiệp `"THỦ LĨNH"`.
6. **Lọt ID thô dạng snake_case trong Dropdown:**
   - Tại `DevCheatPanel.cs:50, 55`: danh sách nạp thẳng `item.Id` (`han_thiet`, `o_thiet`) thay vì tên hiển thị `item.Name`.
7. **Sai lệch trạng thái hồi máu giữa giao diện và glossary:**
   - `EventTextFormatter.cs:76` dịch `REGEN` là `"Hồi Phục"` trong khi Glossary #99 và `ItemDescriber.cs:176` chuẩn hóa là `"Điều Tức"`.

---

## 3. Bảng Đề Xuất Sửa Chuỗi Trong Mã Nguồn (Code-String Fixes)

| File : Dòng | Chuỗi hiện tại (Old) | Chuỗi đề xuất (New) | Lý do |
|---|---|---|---|
| `StorageGrid.cs:226` | `$"Insufficient material '{cost.Key}'."` | `$"Không đủ nguyên liệu."` | Chặn tiếng Anh & raw snake_case id lọt ra popup |
| `PlayerProfile.cs:436` | `$"Insufficient {label} '{cost.Key}'..."` | `$"Không đủ tài nguyên: cần {cost.Value}."` | Chặn câu lỗi tiếng Anh lọt ra UI |
| `PveStageNode.cs:56` | `"BOSS"` | `"THỦ LĨNH"` | Tránh tiếng Anh trên icon bản đồ |
| `FighterPanelView.cs:90` | `"Sinh Lực  " + ...` | `"Khí Huyết  " + ...` | Chuẩn hóa theo Glossary #70 |
| `ReplayPrefabBuilder.cs:62` | `"Sinh Lực  0/0"` | `"Khí Huyết  0/0"` | Đồng bộ text prefab theo Glossary #70 |
| `EventTextFormatter.cs:20` | `"Võ hiệp"` | `"Hiệp Khách"` | Chuẩn hóa danh xưng theo Glossary #28 |
| `EventTextFormatter.cs:25` | `"... tuyệt kỹ"` | `"... Tuyệt Kỹ"` | Viết hoa đồng bộ thuật ngữ Tuyệt Kỹ |
| `EventTextFormatter.cs:38` | `time + item + " hồi " + value + " sinh lực"` | `time + item + " hồi " + value + " Khí Huyết"` | Chuẩn hóa theo Glossary #70 |
| `EventTextFormatter.cs:53` | `return time + (battleEvent.RawType...);` | Bổ sung switch format cho Shield, Start, v.v. | Triệt tiêu chữ tiếng Anh `QI`, `SHIELD`, `START` |
| `EventTextFormatter.cs:76` | `case "REGEN": return "Hồi Phục";` | `case "REGEN": return "Điều Tức";` | Chuẩn hóa theo Glossary #99 |
| `ItemDescriber.cs:74,154` | `" nội lực"` (chữ thường) | `" Nội Lực"` (viết hoa) | Nhất quán viết hoa danh từ tài nguyên chiến đấu |
| `ItemDescriber.cs:160,201`| `" khí huyết"` (chữ thường) | `" Khí Huyết"` (viết hoa) | Nhất quán viết hoa chỉ số theo glossary |
| `ShopPopup.cs:161` | `"Làm mới miễn phí"` (17 kt) | `"Miễn phí"` (8 kt) | Chống tràn nút bấm (giới hạn 14 ký tự) |
| `ShopPopup.cs:162` | `$"Làm mới • {cost:N0} Bạc"` (17+ kt) | `$"Làm mới ({cost:N0})"` (12 kt) | Chống tràn nút bấm |
| `EconomyPopupPrefabBuilder.cs:35` | `"Xác nhận gia nhập"` (17 kt) | `"Gia nhập"` (8 kt) | Chống tràn nút bấm |
| `EconomyPopupPrefabBuilder.cs:341`| `"Đổi nguyên liệu"` (15 kt) | `"Đổi đồ"` (6 kt) | Chống tràn nút bấm |

---

## 4. Tóm Tắt Các Bản Vá Dữ Liệu Đã Tạo (Data Patches)
Toàn bộ các file patch đã được tạo tại `tools/gemini/out/text-audit/` và kiểm tra khớp 100% đường dẫn:
1. `items.patch.json` (7 mục): Rút gọn tên trang bị vượt 18 ký tự (ví dụ: `Thanh Đồng Hộ Tâm Kính` (22) → `Đồng Hộ Tâm Kính` (16), `Cửu Chuyển Hoàn Hồn Đan` (23) → `Cửu Chuyển Đan` (14)).
2. `materials.patch.json` (4 mục): Rút gọn 3 nguyên liệu dài (`Thiên Ngoại Vẫn Thiết` → `Thiên Ngoại Thiết`, `Binh Hồn Tinh Thạch` → `Binh Hồn Thạch`, `Huyền Giáp Hồn Thạch` → `Huyền Giáp Thạch`) và khử tên Kim Dung: `Côn Lôn Tuyết Liên` → `Băng Sơn Tuyết Liên`.
3. `sects.patch.json` (3 mục): Rút gọn tâm pháp > 18 ký tự (`Kim Cang Thiết Cốt Quyết` → `Kim Cang Thiết Cốt`, `Liệt Diễm Tâm Hỏa Quyết` → `Liệt Diễm Tâm Hỏa`, `Hàn Sơn Băng Tâm Quyết` → `Hàn Sơn Băng Tâm`).
4. `shop.patch.json` (2 mục): Rút gọn tên hàng danh vọng (`Rương Chất Dẫn Ngẫu Nhiên` → `Rương Chất Dẫn`, `Rương Nguyên Liệu Bậc 3` → `Rương Liệu Bậc 3`).
5. `chapters.patch.json` (32 mục): Sửa mâu thuẫn quái ải 1-7 (`Thiết Bì Trư Thú`), sửa Lão Tiều Phu → Lục Tiều Phu, khử `sinh lực`, `cương khí`, `tích nộ` trong hội thoại, rút gọn 23 tên ải/địch > 18 ký tự và 1 mô tả > 90 ký tự.

---

## 5. Đề Xuất Bổ Sung Glossary (Chưa có trong `glossary.md`)

1. **Thủ Lĩnh** (`boss`): Hiện nhãn PVE đang để `"BOSS"`. Cần bổ sung vào glossary: `Thủ Lĩnh` đại diện cho Boss ải cuối mỗi hồi, không dùng Boss hay Trùm.
2. **Chất Dẫn** (`catalyst`): Nhóm vật phẩm dùng để Đột Phá Binh Khí / Hộ Cụ / Công Pháp / Đan Dược (Binh Hồn Thạch, Huyền Giáp Thạch, Vấn Đạo Tâm Tinh, Cửu Chuyển Đan Sa).
3. **Bậc** (`tier`): Định danh cấp bậc của nguyên liệu và rương kho báu (`Bậc 1, Bậc 2, Bậc 3`).
4. **Thiên Khí** (`stamina`): Thể lực dùng để khiêu chiến các ải PVE (`stamina.json`).
5. **Rương** (`box`): Vật phẩm chứa phần thưởng ngẫu nhiên trong tiệm Danh Vọng (`Rương Chất Dẫn`, `Rương Liệu Bậc 3`).

---

## 6. Ba Câu Hỏi Mở Cần Quyết Định (Open Questions)

1. **Định dạng phân cách hàng nghìn cho tiền tệ/con số:**  
   Trong code hiện tại, builder tự gán dấu chấm kiểu Việt Nam (`+10.000 Bạc`, `+1.000 Danh Vọng`), nhưng các bộ định dạng chuẩn C# (`cost:N0`) lại xuất ra dấu phẩy kiểu quốc tế (`10,000`). Dự án muốn chốt thống nhất theo định dạng nào?  
   - *Lựa chọn A:* Chuẩn Việt Nam (`10.000 Bạc`, `1.200 Khí Huyết`).  
   - *Lựa chọn B:* Chuẩn quốc tế (`10,000 Bạc`, `1,200 Khí Huyết`).

2. **Hướng xử lý tên danh từ dài hơn 18 ký tự:**  
   Các bản vá trong `items.patch.json` và `sects.patch.json` đã chủ động đề xuất rút gọn tên (ví dụ: `Thanh Đồng Hộ Tâm Kính` → `Đồng Hộ Tâm Kính`, `Liệt Diễm Tâm Hỏa Quyết` → `Liệt Diễm Tâm Hỏa`). User muốn áp dụng tên rút gọn này hay giữ nguyên tên dài 4-5 chữ và cấu hình UI tự thu nhỏ kích cỡ chữ (Auto-fit/shrink text)?

3. **Từ ngữ phân loại Boss trên bản đồ PVE:**  
   Node ải 10 ở mỗi hồi đang hiển thị chữ `BOSS`. User muốn thống nhất đổi thành `Thủ Lĩnh` (trang trọng, phong vị kiếm hiệp chính thống) hay `Trùm` (ngắn gọn, hiện đại)?
