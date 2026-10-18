Đã hoàn tất đợt 2 cho `server/game-server`.

- Sửa: `Database`, `ArenaService`, `MessageHandler`, `Matchmaker`, `AccountService`, `GameServer`, README và các test liên quan.
- Schema nâng từ v1 lên v2, tự migrate và thêm bảng `arena_battles`.
- Thêm `arena_history`, `arena_replay`, `battleId`; replay kiểm tra quyền sở hữu và tính tất định.
- Matcher chọn ngẫu nhiên trong 10 ghost gần rating nhất sau khi lọc Chiến Uy.
- Trận hòa không tăng thắng/thua, Elo dùng 0.5, thưởng như thua.
- Toàn bộ JDBC resource dùng try-with-resources.
- DB mặc định được neo theo thư mục chứa `mvnw`.
- Đã xóa `server/game-server/data/`; test không tái tạo thư mục này.
- Test: **84/84 qua** (`./mvnw -o test`).
- Lint: **0 lỗi trên 20 file Java**.