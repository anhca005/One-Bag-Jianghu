Đã hoàn tất sửa các mục Cao/Vừa và mục Thấp `SectChoiceService`.

- Sửa luồng callback, khóa/mở nút và cập nhật UI theo `ProgressChanged`.
- Giữ nguyên tập `finished` của `TutorialService`.
- Chỉnh tỉ lệ ảnh trong `OnlinePrefabBuilder`; không sửa prefab thủ công.
- Thêm test transport giả cho nút mua và popup Luận Kiếm tự vẽ lại.
- Cập nhật báo cáo tại [-o](<D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\-o>).

Kiểm tra:

- Cả 3 `dotnet build --no-restore` thành công.
- Lint sạch: `0 problem(s) in 19 file(s)`.
- Không chạy Unity theo yêu cầu.
- Còn 4 cảnh báo obsolete có sẵn, không có lỗi biên dịch.Đã sửa

- ShopPopup.cs:344-345, 427-432: chỉ phát tutorial sau shop_buy_ok; khóa nút mua, đóng, tab, đổi,
  làm mới và dropdown khi chờ; lỗi cũng mở khóa và vẽ lại.
- QuestLogPopup.cs:98-100, 114-118 và QuestService.cs:101-103: preflight không sửa/phát sự kiện;
  khóa nhận thưởng, chỉ hoàn tất và vẽ lại trong callback; nghe ProgressChanged khi popup mở.
- TutorialService.cs:9, 31-36, 90, 108 và OnboardingRuntime.cs:24: đọc profile mới qua provider động,
  giữ tập finished, đánh giá lại tutorial khi ProgressChanged và hủy nghe khi runtime bị hủy.
- StageMapPopup.cs:120-204, 346-355, 418-428: nghe profile mới để dựng lại node; khóa Khiêu chiến;
  lỗi server khôi phục leavingForBattle, nút và nhạc nền.
- SectSelectPopup.cs:32, 55, 221-232: không giữ profile; preview từ Session.Profile mới nhất; khóa nút
  gia nhập, đóng và thẻ phái tới khi callback.
- BagLoadoutPanel.cs:51, 137, 203-211; ItemTrayView.cs:23-24, 58, 85-101; BagPopup.cs:70-85:
  không ghi Storage/Loadout thật, bind lại snapshot khi profile đổi hoặc lưu lỗi.
- StoragePopup.cs:62-65, 105, 542-557 và StorageGrid.cs:54, 68: lưu UID thay object item, dựng lại grid
  khi profile đổi/lỗi, khóa đóng và bộ lọc; bỏ migration sửa Materials trong constructor.
- HomeController.cs:96-102, 148-151, 351-359: HUD nghe ProgressChanged; bỏ polling cấp/EXP.
- ArenaLobbyPopup.cs:108, 196-207, 458-462: sảnh đang mở tự bind lại profile và hủy nghe khi đóng.
- SectChoiceService.cs:33-34 (Thấp): TryChoose nay chỉ kiểm tra, không sửa profile.
- OnlinePrefabBuilder.cs: khung chân dung dùng tỉ lệ pixel 444:552 (168x209); dòng lịch sử dùng tỉ lệ
  1040:144 (520x72). Không sửa prefab bằng tay.

Bỏ qua

- ArenaLobbyPopup.cs:422-425, SettingsPopup.cs:262, DevCheatPanel.cs:168-174 và StoryProgress.cs:16,22-24:
  các mục Thấp không phải sửa vài dòng an toàn (cần thêm trạng thái/lifecycle hoặc đổi API và test liên quan).
- Đề xuất StorageGrid không được giữ instance được áp dụng tại các UI bằng cách dựng lại khi profile đổi;
  bản thân StorageGrid vẫn cần giữ snapshot trong thời gian một thao tác đồng bộ, nên không xóa trường profile.

Test thêm/cập nhật

- OnlineNetTests: nút mua bị khóa trong lúc transport giả giữ phản hồi và mở lại sau shop_buy_ok.
- OnlineNetTests: ArenaLobbyPopup đang mở vẽ lại tên từ snapshot profile mới qua transport giả.
- QuestSystemTests: xác nhận preflight nhận thưởng không sửa profile.
- LevelServiceTests: xác nhận kiểm tra chọn phái không sửa profile.
- OnboardingTests: truyền provider để kiểm tra TutorialService với profile hiện hành.
