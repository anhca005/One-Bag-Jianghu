# Task: Online đợt 1, phía client Unity (tài khoản + Luận Kiếm Đài)

Bạn là Codex, làm việc trong Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Viết code C# cho client.

## Đọc trước
- `docs/decisions.md` mục **#68**, #3, #17 (popup), #66 (ảnh UI Simple, chữ tự co cho localize sau này).
- `docs/tech/protocol.md`, mục **"Tài khoản và Luận Kiếm Đài (#68)"**. Đây là hợp đồng với server, làm đúng tên trường.
  Server do một task khác viết song song, nên **đừng chờ server**. Test dùng transport giả (`IGameTransport`).
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Code hiện có:
  - `Assets/_Game/Scripts/Net/*`: `GameClient`, `NetworkService`, `ProtocolJson`, `ServerConfig`;
  - `Scripts/UI/Home/HomeController.cs`: `StartPvp`;
  - `Scripts/UI/Gameplay/GameplayController.cs`: nhánh PvP, `RequestBattle`;
  - `Scripts/UI/Popups/*`: `BasePopup`, `PopupManager`, `ResultPopup`;
  - `Scripts/Economy/PlayerProfile.cs` (Danh Vọng, nguyên liệu), `Scripts/Economy/PowerCatalog.cs` (Chiến Uy).
- Cách dựng prefab popup bằng code: xem `Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs`,
  `QuestUiBuilder.cs` và `UiBuild.cs`, đặc biệt `UiBuild.ApplyUiSprite`, cách chọn khung popup theo tỷ lệ, nút dùng prefab
  chung `Button_Primary`/`Button_Secondary`, `UiBuild.FitLabel`.

## Được phép sửa/tạo
- `Assets/_Game/Scripts/Net/**`
- `Assets/_Game/Scripts/UI/Popups/` (popup mới `AccountPopup`, `ArenaPopup` và các row; sửa `ResultPopup` nếu cần)
- `Assets/_Game/Scripts/UI/Home/HomeController.cs` (chỉ phần PvP)
- `Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs` (chỉ nhánh PvP)
- `Assets/_Game/Scripts/Core/GameSession.cs` (nếu cần chỗ giữ kết quả arena)
- `Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs` (mới), cùng một dòng menu trong `BuildMenu.cs`
- `Assets/_Game/Tests/EditMode/Net/**`
- `Assets/_Game/Scripts/Net/README.md`

**Cấm:**
- mọi lệnh git;
- sửa scene (`*.unity`), đặc biệt LoginScene và HomeScene, vì người dùng sửa tay;
- sửa prefab có sẵn;
- tạo hay sửa ảnh;
- chạy Unity: Claude sẽ build, chạy test và dựng prefab.

Tạo file `.cs` mới thì **không** tạo `.meta`, Unity sẽ tự tạo.

## Việc cần làm
1. **Lớp mạng:**
   - Thêm các lệnh `register`, `login`, `resume`, `logout`, `arena_status`, `arena_battle`, `leaderboard` vào
     `NetworkService` (hoặc một `AccountService`/`ArenaClient` mới). Callback về main thread như `RequestBattle` đang làm.
   - Kiểu dữ liệu C# cho `ArenaStatus`, `ArenaResult`, `LeaderboardEntry`, lỗi server (code + message).
   - Lưu token trong `PlayerPrefs` (khoá `jianghu.auth.token`).
   - Khi kết nối xong mà có token thì tự gửi `resume`. Token hỏng thì xoá đi, coi như chưa đăng nhập.
   - Có sự kiện báo trạng thái đăng nhập thay đổi.
2. **AccountPopup** (prefab ở `Assets/_Game/Resources/UI/Popups/AccountPopup.prefab`, dựng bằng `OnlinePrefabBuilder`):
   - hai chế độ Đăng nhập / Đăng ký;
   - ô tên đăng nhập, mật khẩu (ẩn ký tự), tên Hiệp Khách khi đăng ký (mặc định là tên trong hồ sơ);
   - nút xác nhận, nút chuyển chế độ, dòng lỗi màu đỏ.
   - Lỗi server hiện bằng tiếng Việt, ví dụ `auth_failed` thành "Sai tên đăng nhập hoặc mật khẩu". Kiểm luật tên và
     mật khẩu ở client trước khi gửi.
   - Đăng nhập xong thì đóng popup và mở tiếp ArenaPopup.
3. **ArenaPopup** (`Resources/UI/Popups/ArenaPopup.prefab`), tiêu đề "Luận Kiếm Đài":
   - Khối trạng thái: điểm, hạng, thắng/thua, "Còn N trận có thưởng hôm nay", tiến độ "Thắng tuần: x/10" (Ngọc Tủy),
     mùa kết thúc sau bao lâu.
   - Nút **Khiêu chiến**: kiểm Bao Phục như `TryStartGame` đang làm, đặt `SelectedMode = Pvp`, rồi vào Gameplay.
   - Tab hoặc phần **Bảng xếp hạng**: top 50 cuộn được, dòng của mình tô nổi.
   - Nút Đăng xuất nhỏ và nút đóng.
4. **Luồng từ Home** (`StartPvp`):
   - chưa kết nối thì thử kết nối (timeout 3 giây);
   - không được thì hiện "Luận Kiếm Đài cần kết nối máy chủ";
   - chưa đăng nhập thì mở AccountPopup, đã đăng nhập thì mở ArenaPopup.
5. **Gameplay PvP:**
   - Gửi `arena_battle` với loadout hiện tại (`CurrentLoadoutJson`) và Chiến Uy (`PowerCatalog`, cùng cách Home
     đang tính).
   - Nhận `arena_result` thì cộng ngay `reward` vào hồ sơ (Danh Vọng, nguyên liệu) và lưu hồ sơ, rồi mới phát lại trận.
   - Tên đối thủ lấy từ `opponent.name`.
   - Mất kết nối hoặc server báo lỗi thì báo lỗi rồi về Home. **Bỏ** đường PvP offline với đội mẫu.
   - PvE giữ nguyên, vẫn chạy offline.
6. **ResultPopup sau trận PvP:**
   - hiện thay đổi điểm, ví dụ "Điểm Luận Kiếm 1000 → 1016 (+16)";
   - hiện phần thưởng theo cách đang hiện thưởng PvE: Danh Vọng, Ngọc Tủy;
   - hết lượt có thưởng trong ngày thì ghi "Hôm nay đã hết lượt có thưởng".
   - Hai nút: "Đấu tiếp", đánh thêm trận PvP, và "Về Luận Kiếm Đài", về Home rồi mở lại ArenaPopup.
7. **UI và ảnh** (#66):
   - Chỉ dùng ảnh đã có trong `Assets/_Game/Textures2D/UI/Kit`, để Image kiểu Simple, chọn ảnh có tỷ lệ gần đúng.
   - Mọi Text có chỗ trống và tự co (`FitLabel`/best fit), vì sau này sẽ localize.
   - Thiếu ảnh hợp tỷ lệ cho chỗ nào thì **liệt kê trong báo cáo** (tên chỗ, kích thước). Không mượn tạm ảnh lệch
     tỷ lệ, không kéo giãn.
   - Ô nhập chữ dùng prefab InputField chung nếu có (xem `CommonUiPrefabBuilder`).
8. **Test EditMode** (`Tests/EditMode/Net/`) dùng transport giả:
   - đăng nhập lưu token, resume tự gửi khi có token, token hỏng thì bị xoá;
   - đọc `arena_result` và cộng thưởng đúng vào `PlayerProfile` (Danh Vọng và `chi_ton_ngoc_tuy`);
   - đọc `leaderboard_ok` và `arena_status_ok`;
   - đổi mã lỗi server sang câu tiếng Việt.

## Tự kiểm tra
Không chạy Unity. Đọc lại code cho khớp API thật: tên lớp, namespace, chữ ký hàm có sẵn. Mọi file đã sửa phải qua
`py tools/lint/check_style.py <file>`, chạy ở thư mục gốc repo.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt:
- file đã tạo/sửa;
- tên menu builder;
- chỗ còn thiếu ảnh;
- giả định đã đặt về API server;
- những gì chưa làm được.
