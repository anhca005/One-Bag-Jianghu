# Task: Sửa UI Sảnh Luận Kiếm Đài và Đăng nhập sau khi người dùng chơi thử

Bạn là Codex, làm trong Unity project `One-Bag-Jianghu/`. Người dùng chơi thử và chụp màn hình. Claude đã tìm ra
nguyên nhân phần lớn lỗi, xem danh sách dưới.

## Đọc trước
- `docs/design/arena-lobby.md`, `docs/decisions.md` #66, #69.
- `docs/tech/code-style.md`.
- Code:
  - `Editor/Building/OnlinePrefabBuilder.cs` (helper `Place` theo pixel, góc trên trái);
  - `Scripts/UI/Arena/*`, `Scripts/UI/Popups/AccountPopup.cs`, `LeaderboardRow.cs`.
- Ảnh chụp hiện tại: `One-Bag-Jianghu/Logs/Snapshots/ArenaLobbyPopup.png`, `AccountPopup.png`.

## Lỗi cần sửa
1. **Huy hiệu bậc ở dòng xếp hạng là ô trắng:**
   - Nguyên nhân: `LeaderboardRow.Bind` gọi `ArenaRanks.Load()` hai lần, nên `IndexOf` so hai danh sách khác nhau và
     ra -1.
   - Cách sửa: lấy bậc một lần và tìm sprite theo `ImageKey` thay vì theo index.
   - Kiểm mọi chỗ khác có cùng kiểu lỗi (ArenaLobbyPopup `RankSprite`, thẻ Đối đầu).
2. **Danh sách xếp hạng trống** dù dòng của mình vẫn hiện đúng.
   - Kiểm `BindLeaderboard`: row được tạo trong `leaderboardContent` của prefab `ScrollList` chung, nhưng không thấy.
   - Tìm nguyên nhân: layout của content, anchor/pivot của row, row bị tắt vì template inactive, `Clear()` xoá nhầm,
     hoặc request lỗi bị nuốt.
   - Mỗi dòng 520x64, cách 6, xếp từ trên xuống. Content tự cao theo số dòng (ContentSizeFitter).
   - Áp dụng tương tự cho danh sách Lịch Sử.
   - Thêm test EditMode: bind 3 entry thì content có 3 row đang bật.
3. **Ảnh đại diện Hiệp Khách:**
   - Thẻ Hiệp Khách hiện chỉ có huy hiệu Vô Danh, là một vòng mực rỗng, nên người chơi tưởng ảnh chưa tải.
   - Cần thêm chân dung: khung `Kit/hero_portrait_frame_wide` (444x552, tỷ lệ 0.8), kích thước 168x210, đặt căn giữa
     thẻ ở y 24.
   - Bên trong khung là ảnh `hero_portrait` lấy từ `PveArtLibrary` (xem `ReplayPrefabBuilder` dòng ~126–136 và
     `FighterPanelView`).
   - Huy hiệu bậc thu về 72x72, đè lên góc dưới phải của khung.
   - Dời các dòng chữ bên dưới xuống cho khớp: tên y 245, các dòng sau cách nhau 30.
   - Thẻ Đối đầu cũng dùng chân dung + huy hiệu nhỏ như vậy cho cả hai bên.
4. **Tên phái đang hiện id** (`liet_diem`):
   - Ở thẻ Hiệp Khách, thẻ Đối đầu và mọi chỗ khác, hiện tên phái theo dữ liệu, ví dụ "Liệt Diễm Đường". Xem cách
     Home/SectSelectPopup lấy tên phái.
   - Không có phái thì ghi "Vô Môn Phái".
5. **Tab quá nhỏ:**
   - Ảnh `tab_active`/`tab_idle` có tỷ lệ 4.24, đặt 124x29 nên chữ bé tí.
   - Đổi thành lưới 2x2, mỗi tab 252x60, cách 8, ở y 16 trong bảng. Chữ cỡ 20, tự co.
   - Vùng nội dung bắt đầu từ y 160:
     - Xếp Hạng: danh sách y 160–806, dòng của mình y 816–880;
     - các tab khác dùng y 160–890.
6. **Tiến độ thắng tuần:**
   - Chữ "0/10 → 1 Ngọc Tủy" quá nhỏ và nằm sát trái.
   - Đổi thành một dòng chữ cỡ 18 nằm **trên** thanh: "Thắng tuần 0/10 → 1 Chí Tôn Ngọc Tủy". Đã nhận thì ghi "Đã nhận
     Ngọc Tủy tuần này".
   - Thanh 580x24 căn giữa bên dưới dòng chữ, icon Ngọc Tủy 40 ở cuối thanh.
   - Dời các phần khác của khu Tìm Trận cho cân.
7. **Thanh trên:**
   - Nút quay lại đang đè lên hoa văn đầu thanh. Hoa văn trái của `arena_top_bar` rộng khoảng 96 px.
   - Đặt nút quay lại 64x64 ở x 100, tiêu đề từ x 180.
   - Icon Danh Vọng và Ngọc Tủy 40x40, số cỡ 22.
   - Mọi chữ phải nằm trong phần giữa của thanh (x 100–1820).
8. **Popup Đăng nhập:**
   - Tua rua của dải tên đè lên hàng tab. Dời hàng tab xuống ngay dưới tua rua (khoảng +40 px) và dời các phần khác
     theo. Nếu cần thì tăng chiều cao panel lên 760, giữ tỷ lệ 1.09 của `popup_frame_square` (tức khoảng 828x760).
   - Nút mắt dùng icon `Kit/icon_eye_open` / `icon_eye_closed` (task khác đang vẽ), đặt 40x40 giữa nút
     `button_square` 56x56, đổi icon theo trạng thái ẩn/hiện. Chưa có file thì builder báo thiếu, không sao.
9. **Bao Phục thu nhỏ:**
   - Món dài (1x2, 1x3, 2x1…) hiện chỉ trong một ô. Vẽ icon phủ đúng số ô của món như túi đồ thật: lấy kích thước món
     từ `ItemCatalog`, vị trí từ placement.
   - Icon nằm trên lớp riêng, phủ đúng khung. Viền ô theo hình dạng món dùng `ShapeFrame` (`UiBuild.ApplyItemFrame`).
     Có thể tô viền theo phẩm như `ItemView`.
   - Ô trống chỉ còn viền mờ.
10. Rà toàn bộ Sảnh, popup Đăng nhập, thẻ Đối đầu, dòng xếp hạng và dòng lịch sử:
    - không còn Image nào hiện ô trắng vì thiếu sprite: không có sprite thì tắt Image;
    - không còn chữ nào tràn khỏi khung.

## Được phép sửa
- `Editor/Building/OnlinePrefabBuilder.cs`
- `Scripts/UI/Arena/**`, `Scripts/UI/Popups/AccountPopup.cs`, `LeaderboardRow.cs`
- `Tests/EditMode/**` liên quan

**Cấm:** mọi lệnh git, chạy Unity, tạo/sửa ảnh, sửa scene, đặt tên hàm trùng tên kiểu (lần trước có hàm `Image()`
làm hỏng compile).

## Tự kiểm tra
- Mọi file đã sửa phải qua `py tools/lint/check_style.py <file>`, chạy ở thư mục gốc repo.
- Đọc lại để chắc compile được: đủ `using`, đúng tên kiểu và namespace.
- Claude sẽ build, chạy test và chụp ảnh.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt: nguyên nhân danh sách trống, các thay đổi, field mới.
