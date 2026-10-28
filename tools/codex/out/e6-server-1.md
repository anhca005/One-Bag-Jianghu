Đã hoàn thành E6 server đợt 1:

- Hồ sơ v9, migration schema `profiles`, starter kit và `auth_ok`.
- Khung thao tác khóa theo tài khoản, transaction, validate và tăng `rev`.
- Kho đồ, Bao Phục, nâng Trọng, Hợp Thành, bán đồ và dev cheat.
- Luận Kiếm lấy đội hình/Chiến Uy từ hồ sơ, bỏ qua dữ liệu client và cộng thưởng vào hồ sơ.
- Docker dùng build context gốc repo và đóng gói dữ liệu dùng chung.
- Không sửa client Unity hoặc battle-engine.

Kiểm tra:

- Maven offline: **93 test qua**.
- Style: **0 lỗi trên 10 file Java đã sửa/tạo**.
- Không tải thêm dependency.
- Không tác động `server/data/jianghu.db`.

Báo cáo nằm tại [`-o`](<D:/CONG VIEC/One-Bag-Jianghu/-o>).# Báo cáo E6 server đợt 1

## File đã sửa/tạo

- Server: `GameData.java`, `ProfileDocument.java`, `ProfileGrid.java`, `ProfileService.java`, `Database.java`,
  `AccountService.java`, `MessageHandler.java`, `GameServer.java`.
- Test: `ProfileServiceTest.java`, cập nhật kỳ vọng schema trong `ArenaHistoryTest.java`.
- Đóng gói/tài liệu: `server/deploy/Dockerfile`, `server/deploy/docker-compose.yml`, `server/README.md`,
  `docs/tech/deploy.md`.
- Không sửa `server/battle-engine/**`, `docs/tech/protocol.md` hay bất kỳ file nào trong cây Unity.

## Khung thao tác cho đợt 2

`ProfileService.mutate` khóa theo `account_id`, đọc JSON và `rev`, chạy một `Rule`, validate toàn hồ sơ, ghi trong
transaction rồi tăng `rev`. Kết quả chung tự gắn `profile`, `rev`, `serverTime`; lỗi luật ném `ServiceException` mã
`rejected` trước commit. Đợt 2 chỉ cần thêm hàm tạo `Rule` và đăng ký tin trong `MessageHandler`.

## Khác với C#

- Không đổi luật kinh tế. Lực Chiến v1 dùng `Math.rint`, tương ứng `Math.Round` mặc định ToEven trong C# hiện tại.
- Java giữ hồ sơ dưới cây JSON có kiểu và validate các bất biến thay vì các POJO cho từng nhánh; nhờ vậy các trường
  của shop/PvE/nhiệm vụ thuộc đợt 2 được round-trip nguyên vẹn.

## Kết quả kiểm tra

- `mvnw -o test`: qua 93 test (64 battle-engine, 29 game-server). Sandbox cần chỉ kho Maven cục bộ bằng
  `-Dmaven.repo.local=../.m2/repository`; không phải tải thêm thư viện.
- `py tools/lint/check_style.py` trên toàn bộ Java đã sửa/tạo: 0 lỗi.
- Test mới kiểm tra starter/round-trip v9, tăng và giữ `rev` khi từ chối, cập nhật đồng thời không mất dữ liệu và
  chặn `dev_cheat` ngoài chế độ dev.
