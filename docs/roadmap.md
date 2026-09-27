# Lộ trình

Trạng thái: ⬜ chưa làm · 🟨 đang làm · ✅ xong

| # | Giai đoạn | Nội dung | Ước lượng | Trạng thái |
|---|---|---|---|---|
| 0 | Tài liệu thiết kế | Chốt thuật ngữ, công thức sát thương, 50 vật phẩm MVP trên Sheets, file `.proto` | 2 tuần | 🟨 |
| 1 | battle-engine (Java) | Mô phỏng trận tất định, event bus, unit test, công cụ tự đấu để cân bằng | 3 tuần | 🟨 |
| 2 | Unity: phát lại trận + Bao Phục | Kết nối WebSocket, lưới kéo thả, trình phát lại nhật ký trận | 3–4 tuần | ⬜ |
| 3 | Hành Tẩu Giang Hồ (PvE) | Hồi 1–3, phần thưởng, cường hóa, Tàng Bảo Các, Thương Hội | 4 tuần | ⬜ |
| 4 | Luận Kiếm Đài (PvP) | Ghost, ghép trận theo điểm + Chiến Uy, mùa giải, bảng xếp hạng Redis | 3–4 tuần | ⬜ |
| 5 | Nội dung + hình ảnh | 100+ vật phẩm, 7 môn phái, chơi thử kín | liên tục | ⬜ |

## Giai đoạn 0: việc cụ thể

- [x] Chốt thể loại, công nghệ, phương án đồ giữ lâu dài
- [x] Chốt bộ thuật ngữ võ hiệp
- [x] Đặt tên game, tạo Unity project
- [x] Tạo thư mục docs và quy trình ghi worklog
- [x] Khởi tạo git cho repo
- [ ] Chốt công thức sát thương và các con số cơ bản (nội lực tối đa, chi phí Hóa Kình, thời gian Kiệt Lực…). Đã có đề xuất chạy được trong engine
- [ ] Danh sách 50 vật phẩm MVP (Sheets). Hiện có 14 món trong `items.json`
- [ ] Thiết kế file `.proto` cho giao thức client–server

## Giai đoạn 1: việc cụ thể

- [x] Project Maven `server/` + Maven Wrapper, Java 17
- [x] Mô hình dữ liệu vật phẩm (op, trigger, target) + nạp và kiểm tra JSON
- [x] Lưới Bao Phục: đặt đồ, chồng lấn, liền kề
- [x] Vòng lặp trận: tick, Phổ Công / Tuyệt Kỹ, Hóa Kình, 8 trạng thái, Kiệt Lực, Trọng
- [x] Nhật ký sự kiện (JSON) cho client phát lại
- [x] CLI: chạy 1 trận, tỉ lệ thắng N seed, bảng đối đầu
- [x] 25 unit test
- [x] CLI phân tích nguồn sát thương (`--runs`)
- [x] Cân bằng đợt 1: 5 đội hình mẫu đều trong khoảng 42–57% ([balance.md](design/balance.md))
- [x] Đánh đồng thời thật sự, binh khí ≥ 2 ô, trạng thái tích điểm bộc phát (decisions #10)
- [x] Cân bằng đợt 2: 5 đội hình mẫu trong khoảng 42–60%; 28 unit test
- [ ] Hiệu ứng bộ môn phái (2/4/6 món)
- [ ] Cơ chế còn thiếu: Côn cắt Tuyệt Kỹ, Cung phá Đan Dược, xoay vật phẩm
- [ ] Kỹ năng bị động của Hiệp Khách
- [ ] Quyết định về khắc chế cứng / độ ngẫu nhiên trong trận (xem vấn đề mở trong balance.md)
