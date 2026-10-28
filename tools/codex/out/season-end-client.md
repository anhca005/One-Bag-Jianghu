Đã hoàn tất task #72.

- Tiệm Danh Vọng hỗ trợ `periodWeeks`, kiểm tra giá trị ≥ 1 và preview đúng chu kỳ UTC+7.
- Mốc tuần lấy hoàn toàn từ `weeklyWinsTarget`.
- Thưởng cuối mùa hiện một lần tại Home hoặc Sảnh; đóng popup mới gửi `season_reward_seen`.
- Đã đọc và hiển thị `peakRating`.
- Đã thêm/cập nhật EditMode tests theo yêu cầu.
- Cả ba lệnh `dotnet build` thành công.
- Style checker: 0 lỗi.
- Không chạy Unity, không dùng git, không sửa server/scene/prefab/shop.json.

Báo cáo nằm tại [-o](</D:/CONG VIEC/One-Bag-Jianghu/-o>).Đã hoàn tất client thưởng cuối mùa và Tiệm Danh Vọng 2 tuần (#72).

File đã sửa:
- Scripts Economy: ShopCatalog.cs, PlayerProfile.cs, ShopService.cs.
- Scripts UI: ShopPopup.cs, HomeController.cs, ArenaLobbyPopup.cs.
- Scripts Net: OnlineModels.cs, NetworkService.cs.
- Tests EditMode: ShopServiceTests.cs, OnlineNetTests.cs.

Popup thưởng cuối mùa:
- Hiện tại Home sau đăng nhập; nếu chưa hiện thì arena_status khi mở Sảnh sẽ hiện.
- Chỉ hiện một lần trong phiên. Đóng MessagePopup mới gửi season_reward_seen; client không cộng thưởng.

Test đã thêm/cập nhật:
- Đọc periodWeeks = 2, từ chối periodWeeks < 1, và preview chu kỳ 2 tuần theo thứ Hai UTC+7.
- Đọc seasonReward từ auth_ok; đóng popup kiểm tra transport gửi season_reward_seen đúng seasonId.
- Mốc tuần dùng weeklyWinsTarget = 15 từ payload; model đọc peakRating.

Tự kiểm tra:
- Ba lệnh dotnet build theo yêu cầu đều thành công.
- check_style.py sạch trên 10 file đã sửa.
