# Task (Gemini): Trích luật C# của từng thao tác hồ sơ để làm chuẩn đối chiếu bản Java

Bạn là Gemini, làm việc trong repo One Bag Jianghu. Đây là việc **chỉ đọc và viết báo cáo**, không sửa code.

## Bối cảnh
- Quyết định #70 và #71 (`docs/decisions.md`): server Java sẽ quyết định mọi thay đổi hồ sơ người chơi, và phải cho kết
  quả **giống hệt** các lớp C# hiện có.
- Codex đang viết bản Java. Claude cần một bản luật chính xác, trích thẳng từ C#, để:
  - giao Codex làm đợt 2;
  - review code Java xem có lệch luật C# không.
- Bản kiểm kê trước của bạn là `docs/reviews/2026-10-21-gemini-e6-inventory.md`. Dùng nó để biết file nào cần đọc.

## Đọc
- `docs/tech/protocol.md`, mục "Hồ sơ trên server (E6, #70)".
- `One-Bag-Jianghu/Assets/_Game/Scripts/Economy/**`, `Pve/**`, `Quests/**`, `Onboarding/**`, `Bag/SectChoiceService.cs`,
  `Core/GameSession.cs`, `UI/Gameplay/GameplayController.cs` (`ShowPveResult`), `UI/Popups/DevCheatPanel.cs`.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/*.json`, `Data/Pve/*.json`.
- Test: `One-Bag-Jianghu/Assets/_Game/Tests/EditMode/**`.

## Viết báo cáo vào `docs/reviews/2026-10-21-gemini-e6-rules.md` (tiếng Việt)

Mỗi thao tác một mục, theo thứ tự:
1. Nâng Trọng
2. Hợp Thành
3. Bán
4. Xếp Tàng Bảo Các (di chuyển, dồn kho, thêm đồ/nguyên liệu vào kho)
5. Xếp Bao Phục
6. Thương Hội: làm mới (miễn phí, trả phí), mua, bốc hàng, giá, đổi nguyên liệu, Tiệm Danh Vọng, reset ngày/tuần
7. Thể Lực
8. Trận PvE: vào ải, quái, thưởng lần đầu, thưởng đánh lại, rơi đồ, thưởng ngày
9. EXP và lên cấp
10. Nhiệm vụ: đếm hành động, hoàn thành, nhận thưởng
11. Chọn phái
12. Hướng dẫn, intro, cốt truyện
13. Chiến Uy (`PowerCatalog`)
14. Bộ khởi đầu
15. Bảng cheat

Mỗi mục ghi:
- **Điều kiện từ chối** và **câu lỗi chép nguyên văn** (file:dòng).
- **Công thức** từng bước, chép đúng phép tính và kiểu số (`int`/`long`/`double`), cách làm tròn (`Math.Round` với
  `MidpointRounding` nào, ép kiểu cắt hay làm tròn), thứ tự các bước.
- **Trường hồ sơ bị đổi**, theo thứ tự đổi. Có ghi bộ đếm nhiệm vụ (`RecordQuestAction`) không, tên action là gì.
- **Ngẫu nhiên:** bốc gì, phân bố/trọng số, đọc từ JSON nào.
- **Ca biên:** kho đầy, đã tối đa, số lượng 0, đồ đang đeo trên Bao Phục, stack vượt 999…
- **Ca thử** lấy từ test C# có sẵn: đầu vào → kết quả mong đợi (dẫn tên test). Thiếu ca quan trọng thì tự tính tay
  thêm 1–2 ca, ghi rõ là ca tự tính.

Cuối báo cáo:
- **Chỗ dễ chép sai sang Java** (ví dụ `Math.Round` mặc định của .NET là làm tròn về số chẵn, còn `Math.round` của Java
  làm tròn lên; `Dictionary` không giữ thứ tự; `GetHashCode` khác nhau);
- **Chỗ luật C# có vẻ là lỗi** (nếu có). Chỉ nêu, không sửa.

## Luật
- Chỉ tạo `docs/reviews/2026-10-21-gemini-e6-rules.md`.
- Không sửa file khác, không chạy git.
- Dẫn file:dòng cho mọi khẳng định.
