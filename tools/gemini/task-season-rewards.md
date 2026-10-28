# Task (Gemini): Thiết kế thưởng cuối mùa Luận Kiếm Đài và cách đặt lại điểm

Bạn là Gemini, nhà thiết kế số liệu cho game One Bag Jianghu (auto-battler xếp túi đồ, võ hiệp).

## Đọc
- `docs/decisions.md` #24, #25, #36, #68, #69, #70.
- `docs/design/arena-lobby.md`, `docs/design/gdd.md` (mục Luận Kiếm Đài, Tiền tệ), `docs/design/glossary.md`.
- Kinh tế hiện có: `One-Bag-Jianghu/Assets/_Game/Resources/Data/*.json` (Thương Hội, nguyên liệu, giá Danh Vọng ở Tiệm
  Danh Vọng, giá Hợp Thành lên Chí Tôn cần 5 Chí Tôn Ngọc Tủy).
- Luật hiện tại:
  - Elo K = 32, sàn 1000;
  - bậc Vô Danh 1000, Tân Tú 1100, Thiếu Hiệp 1250, Đại Hiệp 1400, Danh Túc 1600, Tông Sư 1800;
  - mỗi ngày 10 trận đầu có thưởng: thắng +30, thua +10 Danh Vọng;
  - 10 thắng/tuần thì nhận 1 Ngọc Tủy;
  - mùa dài 28 ngày.

## Cần thiết kế
1. **Bảng thưởng cuối mùa theo bậc** cao nhất lúc chốt (hoặc bậc lúc chốt, chọn một và nói lý do): Danh Vọng, Chí Tôn
   Ngọc Tủy, nguyên liệu khác nếu cần.
   - Có thưởng thêm cho top 1, top 2–3, top 4–10 không?
   - Danh hiệu **Thiên Hạ Đệ Nhất** cho hạng 1: giữ trong mùa sau, hiện ở đâu.
   - Trang phục/khung avatar trong gdd: đề xuất có làm ở đợt này không.
2. **Điều kiện nhận:** số trận tối thiểu trong mùa để nhận thưởng. Ví dụ ít nhất 10 trận, để chặn người tạo tài khoản
   rồi bỏ.
3. **Đặt lại điểm** khi sang mùa: công thức (ví dụ `1000 + (điểm - 1000) / 2`), có làm tròn không, sàn 1000 vẫn giữ.
   Thử với vài mức điểm.
4. **Kiểm tra kinh tế:**
   - một người chơi đều đặn (10 trận/ngày, tỷ lệ thắng 50%) nhận bao nhiêu Danh Vọng và Ngọc Tủy mỗi mùa;
   - người giỏi (thắng 65%) nhận bao nhiêu;
   - bao lâu thì đủ 5 Ngọc Tủy để lên Chí Tôn một món.
   Đánh giá có quá nhanh hay quá chậm so với #24/#25. Nếu cần thì đề xuất chỉnh thưởng 30/10, mốc tuần, K hoặc ngưỡng
   bậc, kèm lý do.
   - Có thể viết và chạy script Python mô phỏng (lưu ở `tools/gemini/season_sim.py`): nhiều người chơi với Elo ẩn
     khác nhau, ghép ±50 điểm nới dần theo #70, 28 ngày.
   - Báo cáo phân bố bậc cuối mùa và độ phình điểm do sàn 1000.

## Kết quả
Viết vào `docs/reviews/2026-10-21-gemini-season-rewards.md` bằng tiếng Việt. Có:
- bảng thưởng;
- công thức đặt lại điểm;
- số liệu mô phỏng;
- danh sách thay đổi đề xuất, đánh số để Claude chốt từng mục.

## Luật
- Chỉ tạo `docs/reviews/2026-10-21-gemini-season-rewards.md` và `tools/gemini/season_sim.py`.
- Không sửa file khác, không chạy git.
- Tên gọi theo `glossary.md`. Không dùng tên từ game khác hay tên riêng trong truyện Kim Dung.
