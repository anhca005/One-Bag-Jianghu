# Nhất Nang Giang Hồ (One Bag Jianghu)

Game 2D auto-battler xếp túi đồ, đề tài võ hiệp. Người chơi sắp binh khí, hộ cụ, đan dược vào túi dạng lưới, hai bên tự giao đấu. Game có đánh ải (PvE), cửa hàng, nâng cấp vật phẩm và đấu xếp hạng Luận Kiếm Đài (PvP).

Dự án gồm hai phần: **server Java** (WebSocket, SQLite) và **client Unity**. Server giữ toàn bộ hồ sơ và chạy mô phỏng trận; client chỉ gửi yêu cầu và phát lại kết quả. Muốn chơi cần chạy server trước, rồi mở client.

Tài liệu kỹ thuật: [kiến trúc](docs/tech/architecture.md), [giao thức mạng](docs/tech/protocol.md), [triển khai](docs/tech/deploy.md).

Hướng dẫn dưới đây viết cho Windows. Nhánh dùng cho bản PC là `One-Bag-Jianghu-PC`.

## 1. Cần cài

| Công cụ | Phiên bản | Dùng để |
|---|---|---|
| Git | Bản mới | Tải mã nguồn |
| JDK | 17 trở lên | Chạy server. Không cần cài Maven, dự án có sẵn Maven Wrapper |
| Unity Hub và Unity Editor | **6000.3.9f1** | Mở project và build client |
| Python | 3.x, kèm `aiohttp` | Chỉ cần nếu chạy công cụ đo mạng (mục 7) |

Kiểm tra JDK:

```powershell
java -version
```

## 2. Tải mã nguồn

```powershell
git clone -b One-Bag-Jianghu-PC https://github.com/anhca005/One-Bag-Jianghu.git
cd One-Bag-Jianghu
```

Thư mục chính:

| Thư mục | Nội dung |
|---|---|
| `server/` | Server Java: `battle-engine` (mô phỏng trận), `game-server` (WebSocket, tài khoản, hồ sơ) |
| `One-Bag-Jianghu/` | Project Unity |
| `docs/` | Tài liệu thiết kế và kỹ thuật |
| `tools/` | Công cụ phụ trợ, gồm công cụ đo mạng |

## 3. Chạy server

```powershell
cd server
./run-server.ps1
```

Git Bash thì chạy `./run-server.sh`. Lần đầu Maven phải tải thư viện nên cần có Internet và mất vài phút.

Server sẵn sàng khi log hiện:

```
Game server listening on port 8765 at /ws
```

Địa chỉ mặc định là `ws://localhost:8765/ws`. Cơ sở dữ liệu `server/data/jianghu.db` tự tạo ở lần chạy đầu, nên không cần cài hay cấu hình gì thêm.

Tham số thêm được truyền thẳng cho server:

```powershell
./run-server.ps1 --port 9000 --db data/dev.db
```

| Tham số | Mặc định | Ý nghĩa |
|---|---|---|
| `--port` | `8765` | Cổng lắng nghe |
| `--host` | `0.0.0.0` | Địa chỉ lắng nghe |
| `--db` | `data/jianghu.db` | File SQLite |
| `--dev` | Tắt | Bật lệnh thử nghiệm |

## 4. Chạy client trong Unity

1. Mở Unity Hub, chọn **Add project from disk**, trỏ tới thư mục `One-Bag-Jianghu/One-Bag-Jianghu`.
2. Mở bằng Unity **6000.3.9f1**. Lần đầu Unity import toàn bộ tài nguyên nên có thể mất khá lâu.
3. Mở scene `Assets/_Game/Scenes/LoginScene.unity`.
4. Bấm **Play**, đăng ký tài khoản mới rồi vào game.

Trong Editor, client tự kết nối tới `ws://localhost:8765/ws`, nên server phải đang chạy trên cùng máy.

## 5. Build bản Windows

Trong Unity, chọn menu **One Bag Jianghu > Build > Windows Player**. Bản build nằm ở `Builds/Windows/OneBagJianghu.exe`, ngang hàng với thư mục project Unity.

Build bằng dòng lệnh (phải đóng Unity trước) và ghi sẵn địa chỉ server vào bản build:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.9f1\Editor\Unity.exe" -batchmode -nographics -quit `
  -projectPath "<đường dẫn>\One-Bag-Jianghu\One-Bag-Jianghu" -buildTarget StandaloneWindows64 `
  -executeMethod OneBagJianghu.EditorTools.Building.PlayerBuild.BuildWindows `
  -server ws://<host>:8765/ws -logFile build.log
```

## 6. Chơi từ máy khác

Client chọn địa chỉ server theo thứ tự ưu tiên:

1. Tham số dòng lệnh `-server <url>`.
2. Địa chỉ đã lưu trên máy.
3. File `OneBagJianghu_Data/StreamingAssets/server.txt` trong bản build.
4. Mặc định `ws://localhost:8765/ws`.

**Trong cùng mạng LAN:** mở cổng 8765 trên tường lửa của máy chạy server, rồi chạy client:

```powershell
OneBagJianghu.exe -server ws://<IP máy chạy server>:8765/ws
```

**Qua Internet bằng ngrok:** cài [ngrok](https://ngrok.com/), đăng nhập authtoken, giữ server đang chạy và mở thêm một cửa sổ:

```powershell
ngrok http 8765
```

ngrok in ra địa chỉ `https://<tên>.ngrok-free.dev`. Đổi `https` thành `wss` và thêm `/ws`:

```powershell
OneBagJianghu.exe -server wss://<tên>.ngrok-free.dev/ws
```

**Trên VPS:** dùng Docker Compose và Caddy (có TLS), xem [docs/tech/deploy.md](docs/tech/deploy.md).

## 7. Kiểm thử

Test server, chạy trong `server/`:

```powershell
./mvnw.cmd test
```

Test client: trong Unity mở **Window > General > Test Runner**, chọn **EditMode**, rồi **Run All**. Hiện có 4/433 test client đang hỏng từ trước: `StarterKitHasWeaponAndExpectedCurrencies`, kiểm tra tham chiếu của 2 prefab `ReplayScreen` và `FighterPanel`, và kiểm tra dropdown của `SectSelectPopup`.

Đo tải (nên dùng server và DB riêng để không ghi tài khoản test vào dữ liệu thật):

```powershell
py -m pip install aiohttp
cd server
./run-server.ps1 --port 18765 --db data/bench.db
```

Mở cửa sổ khác ở thư mục gốc của repo:

```powershell
py tools/net/ws_load_test.py --url ws://localhost:18765/ws --clients 1 10 25 50 100 --repeats 3 --out out.csv
```

Đo độ trễ, chỉ dùng tin của khách nên không ghi vào DB:

```powershell
py tools/net/ws_rtt_test.py --url ws://localhost:8765/ws --label local `
  --loadout server/battle-engine/samples/kim_cang.json --out rtt.csv
```

## 8. Lỗi thường gặp

| Hiện tượng | Cách xử lý |
|---|---|
| Client không vào được, báo lỗi kết nối | Kiểm tra server đã hiện dòng `listening`, và đúng địa chỉ, đúng cổng |
| `Address already in use` khi chạy server | Cổng đang bị chiếm. Tắt server cũ hoặc đổi `--port` |
| `java` không nhận lệnh, hoặc lỗi phiên bản | Cài JDK 17 và thêm vào `PATH` |
| Máy khác trong LAN không kết nối được | Mở cổng 8765 trên tường lửa, kiểm tra hai máy cùng mạng |
| Đổi `server.txt` nhưng client vẫn vào địa chỉ cũ | Máy đã lưu địa chỉ khác. Chạy client với `-server <url>` |
| Địa chỉ ngrok không còn dùng được | Kiểm tra ngrok còn chạy và địa chỉ có đổi không, rồi cập nhật lại client |
