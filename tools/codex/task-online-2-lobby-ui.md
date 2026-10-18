# Task: Sảnh Luận Kiếm Đài + Popup Đăng nhập (client Unity)

Bạn là Codex, làm việc trong Unity project `One-Bag-Jianghu/`. Đợt trước, một task Codex khác đã viết lớp mạng
cho tài khoản và Luận Kiếm Đài, cùng một `ArenaPopup` nhỏ. Đợt này thay `ArenaPopup` bằng **Sảnh toàn màn hình**
và làm lại `AccountPopup` theo thiết kế mới.

## Đọc trước
- **Thiết kế:** `docs/design/arena-lobby.md`. Làm đúng bố cục, luồng, chữ, lỗi và bậc trong file này.
- `docs/decisions.md` mục #66, #68, #69. `docs/tech/protocol.md` mục "Tài khoản và Luận Kiếm Đài", "Lịch sử và xem lại".
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- Báo cáo đợt trước: `tools/codex/out/online-1-client.md`. Code đợt trước:
  - `Scripts/Net/OnlineModels.cs`, `Scripts/Net/NetworkService.cs`;
  - `Scripts/UI/Popups/AccountPopup.cs`, `ArenaPopup.cs`, `LeaderboardRow.cs`;
  - `Editor/Building/OnlinePrefabBuilder.cs`.
- Cách dựng prefab bằng code: `Editor/Building/EconomyPopupPrefabBuilder.cs`, `QuestUiBuilder.cs`, `UiBuild.cs`.
  Nút dùng prefab chung `Button_Primary`/`Button_Secondary`; ảnh dùng `UiBuild.ApplyUiSprite`, kiểu Simple; chữ dùng
  `UiBuild.FitLabel` hoặc best fit.
- Ảnh mới của sảnh: `Assets/_Game/Textures2D/UI/Arena/`. Một task Codex khác có thể đang vẽ, nên file chưa có thì vẫn
  dùng đúng tên trong thiết kế mục 7, builder sẽ báo thiếu.

## Được phép sửa/tạo
- `Assets/_Game/Scripts/Net/**`
- `Assets/_Game/Scripts/UI/Arena/**` (thư mục mới cho sảnh: `ArenaLobbyPopup`, các tab, row, lớp tìm trận, thẻ Đối đầu,
  `ArenaRanks`)
- `Assets/_Game/Scripts/UI/Popups/AccountPopup.cs`, `ArenaPopup.cs` (xoá nếu đã thay hết), `LeaderboardRow.cs`,
  `ResultPopup.cs`, `SettingsPopup.cs` (mục Tài khoản), `ShopPopup.cs` (chỉ thêm cách mở thẳng tab Tiệm Danh Vọng)
- `Assets/_Game/Scripts/UI/Home/HomeController.cs` (luồng PvP, mở lại sảnh sau trận)
- `Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs` (PvP chỉ phát lại `arena_result` đã có trong session; xem
  lại trận từ `arena_replay_ok`)
- `Assets/_Game/Scripts/Core/GameSession.cs`
- `Assets/_Game/Resources/Data/arena.json` (mới: ngưỡng bậc, tên bậc, key ảnh)
- `Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs`, `BuildMenu.cs` (menu), `PopupPrefabBuilder.cs` (chỉ phần
  Settings: mục Tài khoản)
- `Assets/_Game/Tests/EditMode/Net/**`, `Assets/_Game/Tests/EditMode/UI/Arena*`

**Cấm:**
- mọi lệnh git;
- sửa scene `*.unity`, đặc biệt LoginScene và HomeScene;
- sửa prefab có sẵn bằng tay;
- tạo hay sửa ảnh;
- chạy Unity.

Tạo file mới thì không tạo `.meta`.

## Việc cần làm
1. **ArenaLobbyPopup** (`Resources/UI/Popups/ArenaLobbyPopup.prefab`): đúng mục 2 thiết kế.
   - Thanh trên: quay lại, tiêu đề, dòng mùa, Danh Vọng, Ngọc Tủy.
   - Thẻ Hiệp Khách có Bao Phục thu nhỏ 5x4. Dùng icon và viền phẩm như các chỗ khác (`ItemIconLibrary`, khung
     `ShapeFrame`/`UiBuild.ApplyItemFrame` nếu hợp).
   - Khu Tìm Trận: lượt có thưởng, thanh thắng tuần (Image Filled), nút Tìm Trận.
   - 4 tab Xếp Hạng / Lịch Sử / Thưởng / Tiệm. Dòng của mình ghim ở đáy. Lịch Sử có nút Xem lại.
   - Dòng hướng dẫn hiện một lần. Lưu cờ trong hồ sơ hoặc PlayerPrefs.
   - Dải "Mất kết nối…" khi rớt mạng; Tìm Trận bị khoá.
2. **Tìm trận** (mục 3): lớp tìm trận (xoay `arena_seal_spinner`, ít nhất 1,2 s) → gửi `arena_battle` → cộng thưởng và
   lưu hồ sơ → cất `ArenaResult` vào `GameSession` → thẻ Đối đầu (2,5 s hoặc bấm Vào trận) → Gameplay.
3. **Gameplay:**
   - Chế độ PvP lấy kết quả có sẵn trong session, không gọi mạng nữa.
   - Chế độ xem lại (`arena_replay`) phát lại mà không cộng thưởng; bảng kết quả chỉ có nút "Về Sảnh".
   - Về Home thì sảnh tự mở lại.
4. **ResultPopup** (mục 4): dòng đổi điểm, dòng lên bậc, phần thưởng, nút Đấu tiếp / Về Sảnh. "Đấu tiếp" về Home, mở
   sảnh và tự bấm Tìm Trận.
5. **AccountPopup** (mục 5):
   - 2 tab, các ô nhập, nút mắt, ô Ghi nhớ đăng nhập, dòng lỗi, nút Để sau;
   - Tab và Enter chuyển ô;
   - bảng câu lỗi tiếng Việt như thiết kế.
   - Ghi nhớ tắt thì không lưu token xuống PlayerPrefs.
6. **Cài đặt → Tài khoản**: hiện tên đang đăng nhập và nút Đăng xuất, hoặc nút Đăng nhập. Sửa `PopupPrefabBuilder`
   phần Settings và `SettingsPopup.cs`.
7. **Bậc:** `ArenaRanks` đọc `arena.json`, đổi điểm → tên bậc + key ảnh. Lên bậc thì ResultPopup báo.
8. **Mạng:** thêm `arena_history` và `arena_replay` theo protocol.md. Server sẽ được bổ sung sau, nên test dùng
   transport giả.
9. **Ảnh và chữ (#66):**
   - Image dùng Simple, chọn ảnh kit có tỷ lệ gần nhất (sai lệch dưới 6%).
   - Chỗ nào không có ảnh hợp tỷ lệ thì vẫn dựng `Image`, gọi `UiBuild.ApplyUiSprite` với tên ảnh dự kiến (ví dụ
     `Arena/arena_panel_card`) để builder báo thiếu, và ghi vào báo cáo: tên, kích thước, nên bake từ ảnh nào. Không
     mượn ảnh lệch tỷ lệ.
   - Mọi Text tự co, chừa khoảng 30% chỗ.
10. **Test EditMode:**
    - `ArenaRanks` đúng ngưỡng;
    - đọc `arena_history_ok` và `arena_replay_ok`;
    - luồng tìm trận cất kết quả vào session và cộng thưởng đúng một lần;
    - AccountPopup kiểm luật nhập và tắt Ghi nhớ thì không lưu token (tách logic ra lớp thuần để test được);
    - xem lại không cộng thưởng.

## Tự kiểm tra
Không chạy Unity. Mọi file đã sửa phải qua `py tools/lint/check_style.py <file>`, chạy ở thư mục gốc repo. Nếu
`dotnet build` các `.csproj` trong `One-Bag-Jianghu/` chạy được thì chạy. File `.cs` mới chưa có trong csproj thì ghi
rõ là chưa compile được.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt, lưu ở file `-o`:
- file đã tạo/sửa/xoá;
- tên menu builder và thứ tự chạy;
- **danh sách ảnh còn thiếu** kèm kích thước;
- giả định về server;
- việc chưa làm.
