# Báo cáo client Sảnh Luận Kiếm Đài

## Đã làm

- Thay `ArenaPopup` bằng `ArenaLobbyPopup` toàn màn hình: thanh mùa/tiền, thẻ Hiệp Khách, Bao Phục 5x4,
  tìm trận, tiến độ tuần, bốn tab, dòng bản thân ghim, lịch sử/xem lại, thưởng, lối vào Tiệm Danh Vọng,
  hướng dẫn một lần và dải mất kết nối.
- Tìm trận gọi `arena_battle` tại Sảnh, chờ ít nhất 1,2 giây, lưu và cộng thưởng đúng một lần, hiện thẻ Đối đầu
  tối đa 2,5 giây rồi mới sang Gameplay.
- Gameplay PvP chỉ phát kết quả trong `GameSession`; xem lại dùng `arena_replay_ok` và không cộng thưởng.
- ResultPopup hiện đổi điểm, lên bậc, thưởng, Đấu tiếp/Về Sảnh. Đấu tiếp tự tìm trận sau khi Home mở lại Sảnh.
- AccountPopup mới có hai tab, nhập lại mật khẩu, tên Hiệp Khách, mắt mật khẩu, ghi nhớ, lỗi tiếng Việt và Enter.
  Khi tắt Ghi nhớ, token chỉ nằm trong phiên.
- Cài đặt hiện tài khoản server và cho đăng nhập/đăng xuất. ShopPopup nhận tham số mở thẳng Tiệm Danh Vọng.
- Thêm `arena_history`, `arena_replay`, dữ liệu bậc `Resources/Data/arena.json` và test EditMode.

## File

- Tạo: `Scripts/UI/Arena/ArenaLobbyPopup.cs`, `ArenaHistoryRow.cs`, `ArenaMatchLogic.cs`, `ArenaRanks.cs`;
  `Scripts/UI/Popups/AccountFormLogic.cs`; `Resources/Data/arena.json`; ba test UI Arena và
  `Tests/EditMode/Net/ArenaProtocolTests.cs`.
- Sửa: `Net/OnlineModels.cs`, `Net/NetworkService.cs`, `Core/GameSession.cs`, `HomeController.cs`,
  `GameplayController.cs`, `AccountPopup.cs`, `LeaderboardRow.cs`, `ResultPopup.cs`, `SettingsPopup.cs`,
  `ShopPopup.cs`, `OnlinePrefabBuilder.cs`, `BuildMenu.cs`.
- Xóa: `Scripts/UI/Popups/ArenaPopup.cs`.

## Builder

- Menu: `One Bag Jianghu/Build/Arena Lobby And Account Prefabs`.
- Thứ tự: chạy `Common UI Prefabs` trước, rồi menu trên. Trong builder: LeaderboardRow → ArenaHistoryRow →
  AccountPopup → ArenaLobbyPopup.

## Ảnh còn thiếu

Thư mục `Textures2D/UI/Arena` hiện chưa có. Builder sẽ báo thiếu các ảnh sau; tất cả dùng Simple, trừ fill tiến độ:

- `arena_lobby_bg` 1920x1080: tranh lôi đài mới theo thiết kế.
- `arena_rank_{vo_danh,tan_tu,thieu_hiep,dai_hiep,danh_tuc,tong_su}` 256x256: huy hiệu mới.
- `arena_seal_spinner` 256x256; `arena_medal_{1,2,3}` 96x96; `arena_versus` 256x160: ảnh mới mục 7.
- `arena_top_bar` 1920x110: bake từ khung ngang cùng phong cách Kit, không có chữ.
- `arena_panel_hero` 420x910, `arena_panel_tabs` 560x910: bake từ khung panel dọc Kit gần nhất.
- `arena_panel_match` 820x346: bake từ khung panel ngang Kit gần nhất.
- `arena_progress_track` và `arena_progress_fill` 580x24: bake từ thanh tiến độ Kit; fill dùng Image Filled.
- `arena_connection_banner` 820x65: bake từ dải thông báo Kit.
- `arena_versus_card` 1075x540: bake từ khung popup rộng Kit.
- `arena_row_rank`, `arena_row_self` 500x64; `arena_row_history` 500x72: bake từ list row Kit.

## Giả định server

- Payload và mã lỗi đúng `protocol.md`; `arena_history` trả tối đa 20 dòng, `arena_replay` chỉ trả trận của tài khoản.
- `arena_result.reward` là phần thưởng nguồn sự thật và client vẫn lưu kinh tế cục bộ ngay khi nhận.
- Server chưa có hai endpoint mới; test đọc protocol dùng envelope giả.

## Tự kiểm và việc chưa làm

- Lint toàn bộ 21 file đã chạm: 0 lỗi.
- Không chạy Unity theo yêu cầu, nên chưa tạo prefab thực tế và chưa chạy EditMode test.
- `dotnet build` không xác nhận được code mới: các `.csproj` Unity hiện chưa chứa `OnlineModels.cs` và thư mục
  `Scripts/UI/Arena`, nên báo thiếu type. Cần Unity refresh project files trước khi build lại.
- Chưa kiểm tra trực quan tỷ lệ/anchor trong Game view; cần chạy builder sau khi ảnh Arena được thêm.
