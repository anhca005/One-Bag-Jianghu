# Task: E6 server đợt 2 — Thương Hội, PvE, lên cấp, nhiệm vụ, phái, hướng dẫn; test đối chiếu luật C#

Bạn là Codex, làm việc trong repo One Bag Jianghu. Đợt 1 đã xong: hồ sơ trên server, khung `ProfileService.mutate`,
kho đồ, Bao Phục, nâng Trọng, Hợp Thành, bán, cheat, Luận Kiếm. Báo cáo ở `tools/codex/out/e6-server-1.md`.

## Đọc trước
- `docs/decisions.md` #70, #71. `docs/tech/protocol.md`, mục **"Hồ sơ trên server (E6, #70)"**. Đây là hợp đồng.
- **`docs/reviews/2026-10-21-gemini-e6-rules.md`**: luật C# chép chi tiết cho từng thao tác, gồm công thức, cách làm
  tròn, câu lỗi nguyên văn và ca thử. Đây là chuẩn đối chiếu. Chỗ nào nó khác code C# thì **code C# đúng**: đọc lại C#
  ở file:dòng nó dẫn.
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Server: `server/game-server/src/main/java/com/onebagjianghu/server/**` (`ProfileService`, `ProfileGrid`,
  `ProfileDocument`, `GameData`, `MessageHandler`).
- C# (chỉ đọc): `Scripts/Economy/ShopService.cs`, `LevelService.cs`, `Pve/**`, `Quests/**`, `Onboarding/**`,
  `Bag/SectChoiceService.cs`, `UI/Gameplay/GameplayController.cs` (`ShowPveResult`), `UI/Popups/DevCheatPanel.cs`.

## Việc cần làm
1. **Thêm thao tác** vào khung `ProfileService.mutate`, đúng luật C#:
   - `shop_refresh` (miễn phí/trả phí), `shop_buy`, `shop_exchange`, `shop_buy_reputation`, reset ngày/tuần;
   - Thể Lực: hồi theo giờ server mỗi khi đọc hồ sơ, giữ múi giờ như C# (UTC);
   - `pve_battle`:
     - lấy Bao Phục trên hồ sơ, trừ Thể Lực, dựng quái từ `Data/Pve/chapters.json`, chạy `BattleSimulator` với seed
       server bốc;
     - cộng thưởng lần đầu, đánh lại và rơi đồ, EXP và lên cấp, bộ đếm nhiệm vụ, cốt truyện chờ xem, thưởng ngày;
     - trả đúng các trường trong protocol;
   - `quest_claim`, `sect_choose`, `onboarding` (4 action);
   - `dev_cheat` thêm: `refresh_shop`, `unlock_pve`, `refill_stamina`, `clear_stage {stageId}`, `next_week`. Đọc
     `DevCheatPanel.cs`.
   - Bộ đếm nhiệm vụ (`RecordQuestAction`) phải tăng ở mọi thao tác mà C# có tăng, **kể cả các thao tác đợt 1** (nâng
     Trọng, Hợp Thành, mua…). Kiểm lại đợt 1 cho đủ.
   - Ngẫu nhiên: dùng RNG của server, không cần tính trùng C#. Nhưng phân bố và trọng số phải đúng.
2. **Làm tròn:** chép đúng từng chỗ, theo mục 16 bản luật:
   - `Math.Round` mặc định (về số chẵn);
   - `AwayFromZero`;
   - ép kiểu cắt.
   Viết helper rõ tên, ví dụ `roundHalfEven` / `roundHalfAwayFromZero`. Không dùng `Math.round` của Java.
3. **Nạp dữ liệu:** `shop`, `quests`, `Pve/chapters`, `Pve/stamina`, `rewards`, `intro`, `story`, `tutorial` qua
   `GameData`.
4. **Test JUnit đối chiếu luật** (đây là phần đợt 1 còn thiếu):
   - mỗi thao tác của **cả hai đợt** (nâng Trọng, Hợp Thành, bán, xếp kho, dồn kho, Bao Phục, Thương Hội, PvE, lên
     cấp, nhiệm vụ, phái, hướng dẫn) có ít nhất:
     - một ca đúng, với số liệu lấy từ phần "Ca thử" của bản luật;
     - một ca bị từ chối, kiểm câu lỗi **nguyên văn**;
   - riêng làm tròn: một test ca 0,5 cho từng helper;
   - PvE:
     - thắng lần đầu: thưởng lần đầu, EXP, lên cấp, cốt truyện;
     - đánh lại: tỉ lệ thưởng;
     - thiếu Thể Lực thì bị từ chối;
     - Thể Lực hồi đúng theo đồng hồ giả.
5. **Mục 17 bản luật** ("luật C# có vẻ là lỗi"): **không tự sửa luật**. Chép đúng như C#, liệt kê vào báo cáo để Claude
   quyết.

## Được phép sửa/tạo
- `server/game-server/**`.
- `server/battle-engine/**` chỉ khi thật cần (ghi lý do).
- `docs/tech/protocol.md` chỉ khi đặc tả thiếu hoặc sai. Ghi rõ đã đổi gì.

**Cấm:** mọi lệnh git; sửa file trong `One-Bag-Jianghu/` (một task khác đang sửa client); chạy Unity; xoá hoặc sửa
`server/data/jianghu.db`. Test dùng DB tạm.

## Tự kiểm tra
- Trong `server/`: `./mvnw -o test` phải qua. Sandbox cần thì thêm `-Dmaven.repo.local=../.m2/repository` như đợt 1.
- `py tools/lint/check_style.py <mọi file Java đã sửa/tạo>` (chạy ở gốc repo) phải sạch.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- file đã sửa;
- thao tác đã thêm;
- số test mới cho từng nhóm;
- chỗ C# có vẻ là lỗi (mục 17) và chỗ Java buộc phải khác C#;
- kết quả test.
