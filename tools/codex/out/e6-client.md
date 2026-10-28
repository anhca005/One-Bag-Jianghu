Đã hoàn tất E6 client và ghi báo cáo tại [-o](</D:/CONG VIEC/One-Bag-Jianghu/-o>).

Kết quả kiểm tra:

- `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore`: thành công.
- Lint 26 file đã sửa: 0 lỗi.
- Không chạy Unity, không sửa scene, prefab hay `server/**`.
- Còn 4 cảnh báo obsolete, không ảnh hưởng build; đã ghi rõ giới hạn test scene Login trong báo cáo.# Báo cáo E6 client

- Đã sửa lõi hồ sơ/mạng: `GameSession`, `NetworkService`, `OnlineModels`; hồ sơ chỉ còn trong RAM, mọi
  `auth_ok`/`*_ok`/`arena_result` áp toàn bộ `profile` + `rev` trước callback, và `ServerClock` dùng `serverTime`.
- Đã sửa đăng nhập: `LoginController`, `AccountPopup`; vào Login tự kết nối/resume, bắt đăng nhập nếu không có phiên,
  có Thử lại khi mất server, khóa tên Hiệp Khách, và tự resume khi rớt mạng đang chơi.
- Đã chuyển lên server: xếp Bao Phục; kéo thả/dồn kho/nâng Trọng/Hợp Thành/bán; làm mới/mua/đổi nguyên liệu/Tiệm
  Danh Vọng; chọn phái; nhận nhiệm vụ; intro/hướng dẫn/cốt truyện/cờ nhắc phái; PvE; Luận Kiếm; Dev Cheat.
  Home/bản đồ ải chỉ đếm giờ bằng `ServerClock`; client không tự hồi/trừ Thể Lực hoặc cộng thưởng/cấp nữa.
- Các lớp luật C# và test luật được giữ để xem trước/kiểm luật. UI kinh tế chỉ chạy luật trên bản sao khi cần preview.
- Không còn đường ghi/đọc `profile.json`; `ProfileStore` chỉ còn shim xóa file save cũ cho menu Editor hiện hữu.
- Test thêm trong `OnlineNetTests`: `auth_ok` áp hồ sơ, action gửi đúng payload và áp snapshot, `rejected` giữ nguyên
  hồ sơ. Test arena đổi sang xác nhận client không cộng thưởng. Đã xoá hai test lưu đĩa không còn ý nghĩa:
  `StoreMigratesLegacyPlayerPrefsValuesAndKeepsStarter`, `SecondSaveKeepsOneLoadableBackup`; test `FromJson` giữ lại.
- Chưa thêm test scene đầy đủ cho popup Login vì task cấm sửa/chạy scene; luồng token/resume được kiểm bằng transport giả,
  còn việc popup được mở bằng code nằm trong `LoginController`.
- Tự kiểm tra: `dotnet build OneBagJianghu.Game.Tests.csproj --no-restore` thành công; lint 26 file: 0 lỗi.
