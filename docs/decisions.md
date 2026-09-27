# Nhật ký quyết định

Mỗi quyết định ghi: ngày, nội dung, lý do. Quyết định mới thêm lên **trên cùng**. Nếu thay đổi quyết định cũ thì ghi quyết định mới và đánh dấu quyết định cũ là *[đã thay thế bởi #n]*.

---

### #10 · 2026-09-27 · Đánh đồng thời, binh khí ≥ 2 ô, trạng thái tích điểm bộc phát
Yêu cầu từ người dùng.
- **Hai bên đánh đồng thời:** trong mỗi tick 100ms, mọi đòn đã ra của cả hai bên đều được thực hiện (xen kẽ A–B, bên dẫn đầu chọn bằng random có seed); tử trận chỉ xét cuối tick. Đòn ra cùng lúc với cú kết liễu vẫn trúng; hai bên cùng gục → hòa; Định Thân gây trong tick không hủy được đòn đối thủ đã ra trong cùng tick.
- **Binh khí tối thiểu 2 ô** (`ItemCatalog.MIN_WEAPON_CELLS`). Quyền Sáo, Trảo, Ám Khí chuyển sang 1x2.
- **Liệt Hỏa / Kịch Độc / Xuất Huyết tích điểm**, không gây sát thương theo thời gian. Đủ 50 điểm → bộc phát rồi reset về 0 (điểm dư bị bỏ). Liệt Hỏa 300 (Cương Khí đỡ được), Kịch Độc 240 (xuyên Cương Khí), Xuất Huyết 150 + 10% khí huyết tối đa (xuyên Cương Khí). Thêm trigger `burst`. Tất cả nằm trong `BattleConfig.Burst`, có sẵn tùy chọn điểm tự giảm theo giây (đang tắt).

### #9 · 2026-09-27 · Luật và quy tắc sau cân bằng đợt 1
Chi tiết và số liệu ở [design/balance.md](design/balance.md).
- *[đã thay thế bởi #10]* **Xuất Huyết** mất một nửa số tầng sau mỗi lần bộc phát. **Lý do:** cộng dồn vô hạn khiến sát thương tăng theo cấp số nhân với số lần đối thủ ra đòn.
- **Hóa Kình** gây Định Thân 1 giây (trước là 2). Thêm hiệu ứng `parry_discount`, chi phí Hóa Kình tối thiểu 20. **Lý do:** đội không có Tuyệt Kỹ biến thành "máy Hóa Kình" khóa đối thủ liên tục.
- *[đã thay thế bởi #10]* **Bên đi trước mỗi tick** chọn bằng random có seed thay cho đổi theo tick chẵn/lẻ. **Lý do:** lỗi, bên A luôn thắng khi hai bên ra đòn cùng lúc.
- **Quy tắc thiết kế vật phẩm:** ngân sách sát thương/giây theo ô × hệ số độ hiếm; binh khí không có Tuyệt Kỹ chỉ cho ≤ 1 nội lực/giây; khống chế toàn bộ đồ địch ≤ 1 giây mỗi lần.

### #8 · 2026-09-27 · Thiết kế battle-engine
- Tick 100 ms, thời gian và chỉ số dùng số nguyên (tỉ lệ theo ‰) để kết quả tất định.
- Random dùng SplitMix64 tự viết thay vì `java.util.Random`: chuỗi số không đổi giữa các bản JDK và dễ port sang C#.
- Vật phẩm hoàn toàn data-driven: hiệu ứng ghép từ ~12 `op` + trigger, không viết code riêng cho từng món.
- Nội lực là quỹ chung của Hiệp Khách. Bên đi trước đổi luân phiên mỗi tick *[đã thay thế bởi #9: chọn bằng random có seed]*.
- Hai bên cùng gục → hòa.

### #7 · 2026-09-27 · Server dùng Java 17 (thay cho 21 trong kế hoạch)
Máy dev đang có JDK 17 (LTS). Build bằng Maven Wrapper (`server/mvnw`), không cần cài Maven. Có thể nâng lên 21 sau mà không phải sửa code.

### #6 · 2026-09-27 · Tên game: One Bag Jianghu (Nhất Nang Giang Hồ)
"Cả giang hồ gói trong một túi", nói đúng lối chơi xếp túi đồ.

### #5 · 2026-09-27 · Thuật ngữ võ hiệp riêng, không dùng tên của game tham chiếu
Bảng chuẩn ở [design/glossary.md](design/glossary.md). Môn phái và vật phẩm dùng tên tự đặt, tránh tên riêng từ truyện Kim Dung để không vướng bản quyền.

### #4 · 2026-09-27 · Vật phẩm giữ lâu dài (phương án B)
Đồ kiếm được ở PvE được giữ và mang vào PvP, không reset theo lượt chơi.
**Lý do:** tạo động lực cày PvE và sưu tầm đồ.
**Rủi ro:** pay-to-win và chênh lệch trong PvP. Cách giảm thiểu xem [design/gdd.md](design/gdd.md#pvp-công-bằng).

### #3 · 2026-09-27 · Server có quyền quyết định, client chỉ phát lại trận
Server Java chạy mô phỏng trận và trả nhật ký sự kiện; Unity phát lại.
**Lý do:** client (C#) và server (Java) không dùng chung được code; đồ có giá trị lâu dài nên cần chống hack; phát lại và xem nhật ký có sẵn.

### #2 · 2026-09-27 · Công nghệ: Unity (client PC) + Java WebSocket (server)
Hình ảnh do ChatGPT tạo.

### #1 · 2026-09-27 · Thể loại: auto-battler xếp túi đồ, 2D hình tĩnh, có PvE và PvP
PvE đi ải theo chương để lấy tài nguyên và kinh nghiệm; PvP đua top để nhận đồ hiếm và trang phục.
