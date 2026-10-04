# Đưa dữ liệu game lên Google Sheets (dùng Gemini trong Chrome)

Gemini trong Chrome thao tác được trên trang đang mở nhưng **không tự chọn được file trên máy**, nên bước tải file do người dùng làm. Gemini lo phần kiểm tra và định dạng.

## Bước 1: người dùng tự nhập file

1. Trong Google Sheets: **Tệp → Nhập → Tải lên → Duyệt**.
2. Chọn `D:\CONG VIEC\One-Bag-Jianghu\exports\One-Bag-Jianghu-data.xlsx`.
3. Chọn **Thay thế bảng tính** → **Nhập dữ liệu**.
4. Chờ trang tải xong và thấy các tab ở dưới đáy.

## Bước 2: bấm “Ask Gemini” và dán lệnh dưới đây

---

Bạn đang ở Google Sheets “One-Bag-Jianghu”. Hãy thao tác trực tiếp trên trang này. Không mở trang khác, không chia sẻ sheet, không xóa tab.

**Việc 1: kiểm tra tab.** Phải có đủ **24 tab** theo đúng thứ tự: Hướng Dẫn, Hiệp Khách, Đội Hình Mẫu, Binh Khí, Đột Phá, Hộ Cụ, Công Pháp, Đan Dược, Kỳ Trân, Tất Cả Vật Phẩm, Nguyên Liệu, Nâng Trọng, Hợp Thành, Thương Hội, Phần Thưởng, Khởi Đầu, Hành Tẩu, Cấp Độ, Lực Chiến, Trạng Thái, Môn Phái, Hệ Binh Khí, Hằng Số, Đối Đầu.

**Việc 2: tìm ô lỗi.** Mở từng tab, tìm `#REF!`, `#NAME?`, `#VALUE!`, `#N/A`, `#DIV/0!`, `#ERROR!`. Ghi tên tab, địa chỉ ô và công thức. **Không sửa công thức.**

**Việc 3: kiểm tra dữ liệu chính.**

- Tất Cả Vật Phẩm có **73 dòng vật phẩm**; cột Độ hiếm hiển thị khi dữ liệu có `rarity`.
- Mọi tab vật phẩm và Tất Cả Vật Phẩm có “Chỉ số” và “Chỉ số T9 Chí Tôn”; Công Pháp/Kỳ Trân có “Chiêu chủ động” khi dữ liệu có `active`. Cột “% ngân sách” của Binh Khí là số phần trăm, không trống và không lỗi. Ghi món thấp nhất và cao nhất.
- Hằng Số có hệ số 6 phẩm, hệ số 9 Trọng, các trần, sát thương Bộc Phát, Kiệt Lực, Điều Tức, bảng Khí Huyết cấp 1–30 và bảng “Chỉ số cuối theo Trọng × Phẩm” 9×6.
- Môn Phái hiển thị `dual` dưới Liệt Diễm và `bow` dưới Hàn Sơn (qua tên hệ tương ứng).

**Việc 4: kiểm tra đội hình và đối đầu.**

- Hiệp Khách có **20 dòng**; Cấp, Khí Huyết tối đa, Hộ Thân, Giảm Hồi Chiêu, số ô, số binh khí và tỉ lệ thắng TB đều hợp lệ.
- Đội Hình Mẫu chứa đủ dữ liệu của **20 mẫu**.
- Đối Đầu là ma trận **20×20** có tô màu.

**Việc 5: kiểm tra 6 tab kinh tế.**

- Nguyên Liệu có ID, tên, loại, nhóm, bậc, Chất dẫn, PvP và mô tả.
- Nâng Trọng có từng bước theo phẩm và công thức tổng Bạc T1→T9.
- Hợp Thành có phẩm từ/đến, Bạc và nguyên liệu thêm bằng tên.
- Thương Hội có làm mới, giá cơ bản, 6 ô hàng, đổi nguyên liệu và đổi Danh Vọng.
- Phần Thưởng có Thắng, Thua, Vượt lần đầu.
- Khởi Đầu có tiền tệ, vật phẩm, tên vật phẩm và vị trí x/y trong Bao Phục.

**Việc 6: kiểm tra PvE.**

- Hành Tẩu có một dòng mỗi ải, phân biệt Cấp và Khí Huyết của file cũ, vật phẩm địch kèm Trọng/phẩm, phần thưởng; có Lực Chiến quái, Lực Chiến đề nghị, Số món quái, Đánh lại: Bạc và Đánh lại: EXP; có bảng chủ đề từng Hồi và danh sách vật phẩm quái.
- Cấp Độ có cấp 1–30, EXP tới cấp sau, EXP tích lũy bằng công thức, mở khóa và Khí Huyết cơ bản bằng công thức tham chiếu Hằng Số.
- Lực Chiến có công thức, `levelBase`, `levelPoints`, `recommendedFactor`, đủ 9 hệ số Trọng, 6 hệ số phẩm và 73 vật phẩm; LC T1 Phàm và LC T9 Chí Tôn là công thức tham chiếu các hệ số.

**Việc 7: làm đẹp (chỉ định dạng, không đổi nội dung).** Ở mọi tab: cố định hàng tiêu đề; tự chỉnh độ rộng cột; cột mô tả dài đặt rộng khoảng 300px và bật xuống dòng.

**Việc 8: báo cáo** bằng tiếng Việt theo khuôn:

```
SỐ TAB: x/24 (thiếu/sai tên/thứ tự: ...)
Ô LỖI: không có | [tab]!ô: mã lỗi — công thức
DỮ LIỆU CHÍNH: 73 vật phẩm; Binh Khí/Lực Chiến/Hằng Số/Môn Phái: ổn | vấn đề ...
BINH KHÍ % NGÂN SÁCH: thấp nhất ... / cao nhất ...
HIỆP KHÁCH / ĐỘI HÌNH / ĐỐI ĐẦU: 20 / 20 / 20×20 — ổn | vấn đề ...
KINH TẾ: 6/6 tab ổn | vấn đề ...
PVE: Hành Tẩu ... dòng ải / Cấp Độ 30 cấp — ổn | vấn đề ...
ĐÃ ĐỊNH DẠNG: ...
GHI CHÚ: ...
```

---

## Nếu Gemini không thao tác được trên trang

Dùng menu **Gemini** trên thanh menu Google Sheets và dán cùng lệnh. Phần định dạng có thể phải làm tay.
