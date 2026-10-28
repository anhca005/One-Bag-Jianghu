# Task (Gemini): Kiểm kê mọi chỗ client tự sửa dữ liệu người chơi

Bạn là Gemini, làm việc trong repo One Bag Jianghu. Đây là việc **chỉ đọc và viết báo cáo**, không sửa code.

## Bối cảnh
Quyết định #70 (`docs/decisions.md`): mọi dữ liệu người chơi chuyển lên server Java. Server quyết định mọi thay đổi,
client chỉ gửi yêu cầu và nhận hồ sơ mới. Trước khi viết giao thức, Claude cần danh sách **đầy đủ** mọi chỗ client
đang tự đọc/sửa hồ sơ trên máy.

## Đọc
- `docs/decisions.md` #68, #70.
- `docs/tech/protocol.md` (giao thức hiện có).
- `One-Bag-Jianghu/Assets/_Game/Scripts/**` (C#). Trọng tâm:
  - `Economy/` (`PlayerProfile`, `MergeService`, `RewardService`, `UpgradeService`, Shop…);
  - `Core/GameSession.cs`, `Pve/`, `Quests/`, `Onboarding/`, `UI/**`.
- `server/game-server/src/main/java/**` để biết server đang có gì.

## Viết báo cáo vào `docs/reviews/2026-10-21-gemini-e6-inventory.md` (tiếng Việt)
1. **Bảng dữ liệu hồ sơ:** mọi trường trong `PlayerProfile` và các lớp con (tên, kiểu, ý nghĩa, ai đọc, ai ghi).
   Đánh dấu trường nào chỉ là trạng thái giao diện trên máy, có thể giữ ở client (cờ đã xem hướng dẫn, tab đang mở…),
   và trường nào phải lên server.
2. **Bảng thao tác làm thay đổi hồ sơ:** với mỗi thao tác ghi:
   - tên thao tác, ví dụ "Nâng Trọng";
   - lớp/hàm thực hiện (đường dẫn file:dòng);
   - màn hình gọi tới;
   - đầu vào, trường hồ sơ bị đổi;
   - có dùng ngẫu nhiên không (seed lấy từ đâu);
   - phụ thuộc file dữ liệu JSON nào.
   Gồm cả: thưởng sau trận PvE, Thể Lực hồi theo thời gian, Thương Hội làm mới theo giờ/tuần, đổi nguyên liệu, bán,
   Hợp Thành, xếp Bao Phục, chọn phái, lên cấp/EXP, Tâm Pháp, nhiệm vụ, hướng dẫn, cốt truyện, bảng cheat (DevCheatPanel),
   bộ đồ khởi đầu, chuyển dữ liệu cũ, thưởng Luận Kiếm.
3. **Logic thời gian:** mọi chỗ dùng giờ máy (`DateTime.Now`, `UtcNow`…), cho biết cái nào phải chuyển sang giờ server.
4. **Dữ liệu tĩnh client và server cần dùng chung:** liệt kê file JSON trong `Resources/Data` và lớp C# đọc nó.
5. **Đề xuất nhóm tin nhắn giao thức:** gom các thao tác thành nhóm tin `C→S`/`S→C` (chỉ tên và payload chính, chưa
   cần đầy đủ). Đề xuất server trả **toàn bộ hồ sơ** hay **phần thay đổi** sau mỗi thao tác, kèm lý do.
6. **Rủi ro:** chỗ logic phức tạp khó chuyển sang Java, chỗ client đang giả định có hồ sơ ngay (đồng bộ), test nào sẽ
   vỡ.

## Luật
- **Không sửa** file nào ngoài file báo cáo.
- Không chạy git.
- Dẫn file:dòng cho mọi khẳng định.
