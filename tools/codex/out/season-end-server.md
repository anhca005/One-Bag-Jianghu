Đã hoàn tất phần server chốt mùa Luận Kiếm.

- Chốt mùa đúng một lần bằng transaction, `finalized_at` và khóa chính `season_rewards`.
- Thưởng top 1–50, hỗ trợ `overflow`, reset mùa mới về 1000 và tách ghost theo mùa.
- Thêm `peak_rating`, `seasonReward`, `season_reward_seen`.
- Ngọc Tủy: giá 800, chu kỳ 2 tuần; thưởng tuần cần 15 thắng.
- Đã cập nhật đặc tả protocol và thêm JUnit dùng đồng hồ giả.
- Maven: **97 test qua, BUILD SUCCESS**.
- Style lint: **0 lỗi**.
- Không sửa C#, không chạy Unity, không đụng `server/data/*.db`.

Báo cáo chi tiết ngắn nằm tại [`-o`](<D:/CONG VIEC/One-Bag-Jianghu/-o>).Đã sửa:
- Server: ArenaService, Database, ProfileService, E6Rules, MessageHandler; test ArenaSeasonTest và ArenaHistoryTest.
- Dữ liệu: shop.json (Ngọc Tủy giá 800, periodWeeks 2).
- Đặc tả: docs/tech/protocol.md.

Chốt đúng một lần:
- Toàn bộ xếp hạng, ghi season_rewards, cộng hồ sơ, đánh dấu finalized_at và tạo mùa mới chạy trong một transaction.
- Khóa chính (season_id, account_id) cùng INSERT OR IGNORE ngăn trao trùng khi chạy lại.
- Kiểm lúc server khởi động, mỗi phút, và trước các lần đọc mùa quan trọng.

Client cần làm:
- Hiện seasonReward trong auth_ok/arena_status_ok tới khi gửi season_reward_seen.
- Đọc periodWeeks (mặc định 1) để hiển thị đúng giới hạn Tiệm Danh Vọng; Ngọc Tủy dùng 2 tuần.
- Đọc weeklyWinsTarget từ server; mốc hiện tại là 15 thắng.

Kiểm tra:
- .\mvnw.cmd -o -Dmaven.repo.local=<repo>\.m2\repository test: BUILD SUCCESS, 97 test qua.
- check_style.py trên 7 file Java đã sửa: sạch.
