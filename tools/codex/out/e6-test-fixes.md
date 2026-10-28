Đã sửa đủ 3 lỗi EditMode:

- Test chọn môn phái đúng luồng server-authoritative, thêm kiểm tra transport `sect_choose`.
- Portrait bật `preserveAspect`.
- Sửa đúng tỷ lệ `LeaderboardRow` 520×64 và `ArenaHistoryRow` 520×72.
- Không sửa code game, prefab hoặc ảnh.

Tự kiểm tra:

- Cả hai `dotnet build --no-restore` thành công.
- Style check: `0 problem(s) in 4 file(s)`.
- Không chạy Unity.

Báo cáo: [-o](<D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\-o>)Đã sửa 3 test EditMode còn hỏng:

- Cập nhật test chọn môn phái: `TryChoose`/`TryJoin` chỉ xác thực và không đổi hồ sơ.
- Thêm test transport `sect_choose`, kiểm payload và hồ sơ server trả về được áp với `SectId = han_son`, `TamPhapLevel = 1`.
- Sửa builder: portrait giữ đúng tỷ lệ; `LeaderboardRow` dùng 520x64 theo ảnh 1040x128; `ArenaHistoryRow` dùng 520x72 theo ảnh 1040x144.
- Không sửa code game, prefab hay ảnh.

Kiểm tra:

- `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore`: thành công, 0 lỗi (4 cảnh báo obsolete có sẵn).
- `dotnet build OneBagJianghu.Editor.csproj --no-restore`: thành công, 0 lỗi.
- Style check: 0 lỗi trong 4 file.
- Không chạy Unity theo yêu cầu.
