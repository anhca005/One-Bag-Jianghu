# Đưa game server lên VPS

Tài liệu này đưa server lên Ubuntu 24.04 tại `wss://game.example.com/ws`. Thay tên miền mẫu bằng tên miền của bạn.

## 1. Chuẩn bị VPS và tên miền

Thuê VPS Ubuntu 24.04 có ít nhất 1 GB RAM và một địa chỉ IPv4 công cộng. Trong trang quản lý DNS, tạo bản ghi
`A` cho tên miền (ví dụ `game.example.com`) trỏ tới IPv4 của VPS. Nếu VPS có IPv6, có thể thêm bản ghi `AAAA`.
DNS thường cần vài phút đến vài giờ để cập nhật.

Trong tường lửa của nhà cung cấp VPS và UFW, mở SSH, HTTP và HTTPS:

```bash
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 443/udp
sudo ufw enable
```

Không mở cổng 8765 ra Internet; chỉ Caddy trong mạng Docker được nối tới game server.

## 2. Cài Docker

Đăng nhập VPS bằng SSH rồi cài Docker từ kho chính thức:

```bash
sudo apt-get update
sudo apt-get install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] \
https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" \
| sudo tee /etc/apt/sources.list.d/docker.list >/dev/null
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo usermod -aG docker "$USER"
```

Đăng xuất SSH rồi đăng nhập lại để dùng `docker` không cần `sudo`.

## 3. Chép mã nguồn và cấu hình

Cách thuận tiện nhất là clone toàn bộ dự án rồi vào thư mục triển khai:

```bash
git clone <DIA_CHI_KHO_MA> one-bag-jianghu
cd one-bag-jianghu/server/deploy
cp .env.example .env
nano .env
```

Nếu không dùng Git, chép nguyên thư mục `server/` lên VPS bằng SCP/SFTP rồi vào `server/deploy`. Điền `DOMAIN` bằng
tên miền, không có `https://` hoặc `/ws`; điền `EMAIL` để Let's Encrypt gửi cảnh báo chứng chỉ. File `.env` không nên
đưa lên kho mã.

## 4. Khởi động và xem log

```bash
docker compose up -d --build
docker compose ps
docker compose logs -f game caddy
```

Build context là gốc repo để ảnh chép cùng bộ `Assets/_Game/Resources/Data` mà client dùng. Vì vậy khi chép nguồn
thủ công lên VPS phải chép cả `server/` và `One-Bag-Jianghu/Assets/_Game/Resources/Data`, không chỉ riêng `server/`.

Caddy tự xin và gia hạn chứng chỉ Let's Encrypt. Khi log không còn lỗi DNS/TLS, địa chỉ của game là
`wss://<DOMAIN>/ws`.

Cấu hình nhận tối đa 64 KiB cho phần HTTP trước khi nâng cấp WebSocket, header tối đa 16 KiB và tối đa 256 kết nối
tới game server. Giới hạn file descriptor của container game là 1.024 và heap Java là 512 MiB. Lưu ý giới hạn
64 KiB của Caddy không áp dụng cho frame sau khi kết nối đã nâng cấp; client tin cậy cần giữ mỗi tin JSON dưới
64 KiB. Muốn chống client thù địch phải bổ sung giới hạn frame trong ứng dụng trước khi mở server công cộng.

## 5. Kiểm tra kết nối WSS

Cài `wscat` trên máy cá nhân có Node.js, rồi kết nối:

```bash
npx wscat -c wss://game.example.com/ws
```

Thấy dòng `Connected` là TLS và WebSocket hoạt động. Có thể xem lỗi đồng thời bằng `docker compose logs -f`.

## 6. Trỏ bản build game tới server

Trước khi build Unity, tạo file `Assets/StreamingAssets/server.txt` với đúng một dòng:

```text
wss://game.example.com/ws
```

Không có file này trong kho mã. Thứ tự ưu tiên là tham số chạy `-server`, địa chỉ đã lưu trong PlayerPrefs, file
`StreamingAssets/server.txt`, rồi `ws://localhost:8765/ws`. Vì vậy có thể đổi máy chủ của một lần chạy bằng
`TenGame -server wss://game.example.com/ws` mà không build lại.

## 7. Sao lưu tự động

Script dùng lệnh SQLite `.backup`, an toàn khi server đang ghi DB, và giữ 14 file mới nhất theo ngày:

```bash
chmod +x backup.sh restore.sh
./backup.sh
docker compose exec game ls -lh /app/state/backups
```

Đặt cron chạy lúc 03:15 hằng ngày (đường dẫn phải là đường dẫn tuyệt đối thật trên VPS):

```bash
crontab -e
15 3 * * * /home/ubuntu/one-bag-jianghu/server/deploy/backup.sh >> /home/ubuntu/jianghu-backup.log 2>&1
```

Nên định kỳ chép thư mục backup sang máy khác; volume trên cùng VPS không bảo vệ khỏi hỏng hoặc mất VPS.

Khôi phục sẽ kiểm tra file, dừng game, thay DB rồi khởi động lại:

```bash
./restore.sh /app/state/backups/jianghu-2026-10-21.db
docker compose logs -f game
```

## 8. Cập nhật phiên bản

Sao lưu trước, lấy mã mới rồi build lại:

```bash
./backup.sh
cd ../..
git pull
cd server/deploy
docker compose up -d --build
docker compose logs -f game
```

Nếu chép file thủ công, thay thư mục `server/` nhưng giữ lại `.env`, rồi chạy lệnh `docker compose up -d --build`.
DB và chứng chỉ nằm trong Docker volume nên không mất khi dựng lại container. Không dùng `docker compose down -v`
trừ khi thực sự muốn xóa dữ liệu.

## Chạy thử trên máy

Không cần TLS khi chỉ thử trên chính máy đó. Từ thư mục `server/`:

```bash
./mvnw -pl game-server -am package
java -jar game-server/target/game-server.jar --host 127.0.0.1 --port 8765 \
  --db data/jianghu.db --samples battle-engine/samples
```

Game mặc định nối tới `ws://localhost:8765/ws`. Trên Windows PowerShell dùng `mvnw.cmd` thay cho `./mvnw`.
