# Gemini: soạn nhiệm vụ cốt truyện Hồi 2–5 (Hành Tẩu Lục)

Dự án: `D:/CONG VIEC/One-Bag-Jianghu`. Viết tiếng Việt, giọng võ hiệp. Không dùng tên riêng từ truyện Kim Dung, không dùng tên của game tham chiếu.

## Đọc trước
- `docs/design/quests.md`: luật, định dạng JSON, các kiểu mục tiêu, `goto`, cỡ thưởng, và bảng Hồi 1 làm mẫu.
- `docs/design/story-bible.md`: nhân vật, speaker id, diễn biến 50 ải.
- `docs/design/story.md`: mạch truyện.
- `docs/design/glossary.md`: thuật ngữ và id nguyên liệu.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: id và tên ải `hoi_<n>_<nn>`, ải nào là tinh anh, ải nào là boss.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/quests.json`: Hồi 1 đã có, nếu file đã được tạo. Giữ giọng văn giống vậy.

## Việc cần làm
1. **Soạn Hồi 2, 3, 4, 5, mỗi Hồi 8–10 nhiệm vụ**, theo đúng định dạng JSON trong `quests.md`.
   - **Nhiệm vụ chính:** bám diễn biến ải trong story-bible. Không cần mỗi ải một nhiệm vụ, chọn các ải có bước ngoặt. Ải boss luôn là nhiệm vụ chính cuối chuỗi.
   - **Việc phụ:** 3–4 nhiệm vụ mỗi Hồi, dạy hoặc ôn lối chơi hợp với giai đoạn đó, và gắn vào lời của đồng minh Hồi đó.
     - Hồi 2: Đan Dược, giải độc.
     - Hồi 3: Tâm Pháp tầng cao, Đột Phá ở mốc Trọng 3/6.
     - Hồi 4: Hợp Thành.
     - Hồi 5: tổng hợp.
     - Chỉ dùng các kiểu mục tiêu có trong bảng.
   - `giver` dùng đúng speaker id. `text` dài 1–2 câu, đúng giọng người giao.
   - **Thưởng** theo bảng cỡ thưởng và bậc nguyên liệu theo Hồi. Chỉ dùng id nguyên liệu có trong glossary.
   - Mục tiêu phải làm được ở thời điểm đó:
     - không đòi cấp, tầng Tâm Pháp hay Trọng cao hơn mức người chơi có thể đạt;
     - `equip_count` không vượt số ô Bao Phục.
2. **Rà Hồi 1** trong `quests.json`: sửa lời văn, nhịp và thưởng nếu cần. Ghi rõ đã đổi gì.
3. **Không đổi** id hay thứ tự nhiệm vụ Hồi 1. Không sửa file nào khác.

## Kết quả
- Ghi các nhiệm vụ mới vào `quests.json`: nối sau Hồi 1, đúng thứ tự chuỗi.
- Viết báo cáo `docs/reviews/2026-10-11-gemini-quests.md`, gồm:
  - bảng tóm tắt từng Hồi: id, loại, tên, mục tiêu, thưởng;
  - tổng Bạc, EXP và nguyên liệu mỗi Hồi;
  - các chỗ còn phân vân.
