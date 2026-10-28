# Task: E6 server đợt 1 — hồ sơ trên server, kho đồ, Bao Phục, Luận Kiếm

Bạn là Codex, làm việc trong repo One Bag Jianghu. Đây là **đợt 1** của việc đưa hồ sơ người chơi lên server Java.
Đợt 2 (Thương Hội, PvE, nhiệm vụ, hướng dẫn, chọn phái) làm sau. Vẫn phải dựng nền để đợt 2 cắm vào dễ.

## Đọc trước
- `docs/decisions.md` #70, #71. `docs/tech/protocol.md`, mục **"Hồ sơ trên server (E6, #70)"**. Đây là hợp đồng:
  làm đúng tên tin và tên trường.
- `docs/reviews/2026-10-21-gemini-e6-inventory.md`: kiểm kê mọi thao tác, có dẫn file:dòng C#.
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Server: `server/game-server/src/main/java/com/onebagjianghu/server/**`, `server/battle-engine/**`.
- Luật C# cần chép (chỉ đọc, **không sửa**): `One-Bag-Jianghu/Assets/_Game/Scripts/Economy/` (`PlayerProfile`,
  `StorageGrid`, `UpgradeService`, `MergeService`, `SellService`, `StarterKit`, `MaterialCatalog`, `PowerCatalog`,
  `LevelService`), `Scripts/Core/GameSession.cs`, `Scripts/UI/Common/BagLoadoutPanel.cs` (luật xếp Bao Phục).
- Dữ liệu: `One-Bag-Jianghu/Assets/_Game/Resources/Data/*.json`.

## Việc cần làm
1. **Nạp dữ liệu dùng chung:**
   - `GameServer` có sẵn `--data`/`JIANGHU_DATA`;
   - khi chạy dev, mặc định là `../One-Bag-Jianghu/Assets/_Game/Resources/Data` (tính từ `server/`);
   - đọc `materials`, `upgrade_costs`, `merge`, `starter`, `levels`, `power`, `items`, `sects` vào các lớp catalog Java;
   - thiếu file thì báo lỗi rõ khi khởi động.
2. **Mô hình hồ sơ Java:**
   - các lớp ánh xạ đúng JSON của `PlayerProfile` version 9 (Jackson, cùng tên trường);
   - đọc rồi ghi lại không mất trường nào;
   - có hàm kiểm tra tính hợp lệ giống `PlayerProfile.Validate` (tổng `materials` khớp các stack trong kho…).
3. **Lưu trữ:**
   - bảng `profiles` (`account_id` PK, `json`, `rev`, `updated_at`), migration schema tăng phiên bản như cách `Database`
     đang làm;
   - `register` tạo hồ sơ từ bộ khởi đầu, giống `StarterKit.CreateProfile`, với `heroName` của tài khoản;
   - tài khoản cũ chưa có hồ sơ thì tạo khi đăng nhập;
   - `auth_ok` thêm `profile`, `rev`, `serverTime`.
4. **Khung xử lý thao tác:**
   - một chỗ chung làm việc này: khoá theo tài khoản, đọc hồ sơ, gọi luật, kiểm hợp lệ, ghi trong transaction, tăng
     `rev`, trả `<type>_ok` với `profile`/`rev`/`serverTime`;
   - lỗi luật chơi trả `error rejected` với câu tiếng Việt **giống hệt** câu C#;
   - đợt 2 chỉ việc thêm luật mới vào khung này.
5. **Thao tác đợt 1** (chép đúng luật C#, kể cả làm tròn `MidpointRounding.AwayFromZero` nếu C# dùng):
   - `profile_get`;
   - `loadout_save`: kiểm uid thuộc người chơi, vừa lưới 5x4, không chồng nhau, theo đúng luật `BagLoadoutPanel` /
     `GameSession`;
   - `storage_move`, `storage_repack` (`StorageGrid`: thứ tự quét ô, gộp stack, trần 999);
   - `item_upgrade`, `item_merge`, `item_sell`;
   - `dev_cheat` (chỉ khi `--dev`/`JIANGHU_DEV=1`, nếu không thì `error forbidden`). Đợt này có các action:
     `add_item {itemId, rarity}`, `add_material {amount}`, `add_currency {id, amount}`, `add_level`, `add_exp`,
     `reset_sect`, `reset_profile`. Đọc `DevCheatPanel.cs` để biết từng nút làm gì.
6. **Luận Kiếm dùng hồ sơ:**
   - `arena_find` nhận `{}`. Đội hình và Chiến Uy lấy từ hồ sơ: chép cách `GameSession` dựng loadout cho trận, cùng
     `PowerCatalog`;
   - client cũ còn gửi `loadout`/`power` thì bỏ qua;
   - thưởng Danh Vọng/Ngọc Tủy cộng thẳng vào hồ sơ trong cùng transaction với trận;
   - `arena_result` thêm `profile`, `rev`, `serverTime`.
7. **Test JUnit:**
   - đọc rồi ghi lại hồ sơ mẫu không đổi;
   - đăng ký thì có bộ khởi đầu;
   - mỗi thao tác có một ca đúng và một ca bị từ chối (đúng câu lỗi);
   - `rev` tăng;
   - hai yêu cầu đồng thời cùng tài khoản không làm mất dữ liệu;
   - `dev_cheat` bị chặn khi không `--dev`;
   - `arena_find` dùng Bao Phục trên server và cộng thưởng vào hồ sơ.
   Với nâng Trọng, Hợp Thành, bán và xếp kho: lấy vài ca từ test C# (`Tests/EditMode/Economy/*`) rồi viết lại bằng Java
   cho cùng kết quả.

## Được phép sửa/tạo
- `server/game-server/**` (kể cả `pom.xml` nếu cần thêm thư viện; thiếu trong kho Maven thì ghi lệnh tải vào báo cáo).
- `server/battle-engine/**` chỉ khi thật cần (ghi lý do).
- `server/deploy/Dockerfile`, `server/deploy/docker-compose.yml`: build context đổi sang gốc repo để chép
  `Resources/Data` vào ảnh. Cập nhật `docs/tech/deploy.md` nếu lệnh build đổi.
- `server/README.md`.
- `docs/tech/protocol.md` chỉ khi thấy đặc tả thiếu hoặc sai. Ghi rõ đã đổi gì trong báo cáo.

**Cấm:** mọi lệnh git; sửa bất kỳ file nào trong `One-Bag-Jianghu/` (một task khác đang sửa client); chạy Unity; xoá
`server/data/jianghu.db` (DB dev của người dùng). Test dùng DB tạm.

## Tự kiểm tra
- Trong `server/`: `./mvnw -o test` phải qua. Sandbox chặn đọc `~/.m2` thì copy tạm vào workspace rồi xoá như các lần
  trước.
- `py tools/lint/check_style.py <mọi file Java đã sửa>` (chạy ở gốc repo) phải sạch.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- file đã sửa;
- khung xử lý thao tác hoạt động thế nào (để đợt 2 dùng);
- chỗ luật Java khác C# (nếu có) và lý do;
- kết quả test.
