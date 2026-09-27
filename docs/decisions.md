# Nhật ký quyết định

Mỗi quyết định ghi: ngày, nội dung, lý do. Quyết định mới thêm lên **trên cùng**. Nếu thay đổi quyết định cũ thì ghi quyết định mới và đánh dấu quyết định cũ là *[đã thay thế bởi #n]*.

---

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
