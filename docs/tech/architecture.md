# Kiến trúc kỹ thuật

## Tổng quan

```
Unity client (C#) ──WebSocket + Protobuf──► Java server
                                              ├─ battle-engine (mô phỏng trận)
                                              ├─ PostgreSQL (tài khoản, vật phẩm, tiến độ)
                                              └─ Redis (session, bảng xếp hạng)
```

**Server có quyền quyết định** (quyết định #3): client gửi hành động, server kiểm tra và chạy mọi logic, client chỉ hiển thị.

Luồng một trận:
```
Client: C2S_PveStart { stageId }
Server: lấy đội hình → battle-engine chạy (seed) → nhật ký sự kiện
Server: S2C_BattleResult { seed, events[], result, rewards }
Client: phát lại events[] theo thời gian (x1/x2, tua, xem nhật ký)
```

## Client Unity

- Unity **6000.3.9f1**, URP, UGUI (kéo thả lưới), Input System.
- Project nằm ở `One-Bag-Jianghu/` (thư mục con của repo).
- WebSocket: `System.Net.WebSockets.ClientWebSocket`. Protobuf: `Google.Protobuf`.
- Hình ảnh: Addressables (dự kiến).

Cấu trúc code dự kiến trong `Assets/_Game/`:
```
Scripts/Net      ← kết nối, đóng gói/giải gói tin
Scripts/Data     ← model, cấu hình vật phẩm nhận từ server
Scripts/Bag      ← lưới Bao Phục, kéo thả, kiểm tra liền kề
Scripts/Battle   ← trình phát lại nhật ký, hiệu ứng số nhảy
Scripts/UI       ← Thương Hội, Tàng Bảo Các, Vạn Bảo Phổ, Luận Kiếm Đài
Art/             ← sprite, khung độ hiếm
Scenes/
```

## Server Java (`server/`, xem [server/README.md](../../server/README.md))

- Java 17 (quyết định #7), Maven đa module + Maven Wrapper. Netty hoặc Spring Boot WebSocket cho gateway (chưa chọn).
- Module:

| Module | Việc |
|---|---|
| `gateway` | Nhận kết nối WebSocket, định tuyến gói tin |
| `auth` | Đăng nhập, phiên |
| `player` | Hồ sơ, Hiệp Khách, tiến trình |
| `inventory` | Vật phẩm, Bao Phục, Tàng Bảo Các, cường hóa |
| `shop` | Thương Hội |
| `pve` | Hồi, ải, phần thưởng, thể lực |
| `arena` | Ghost, ghép trận, mùa giải |
| `leaderboard` | Bảng xếp hạng (Redis ZSET) |
| `config` | Nạp dữ liệu JSON (xuất từ Sheets) |
| **`battle-engine`** ✅ đã có | Java thuần, không phụ thuộc framework, có unit test |

## Giao thức
- Gói tin định nghĩa bằng file `.proto` dùng chung, sinh code cho cả C# và Java.
- Quy ước tên: `C2S_*` (client → server), `S2C_*` (server → client).

## Dữ liệu cân bằng
Google Sheets → xuất JSON → server nạp khi khởi động (có thể nạp lại khi đang chạy). Client nhận cấu hình từ server lúc đăng nhập.
