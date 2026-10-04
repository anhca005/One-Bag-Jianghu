# Giao thức client ↔ server (MVP)

Quyết định #18. Bản MVP dùng **WebSocket + JSON** (dễ đọc, dễ gỡ lỗi); chuyển sang Protobuf khi cần tối ưu băng thông. Server có quyền quyết định (quyết định #3): client chỉ gửi đội hình, server chọn đối thủ, seed và chạy trận.

## Kết nối
- URL mặc định: `ws://localhost:8765/ws` (cấu hình được ở cả hai phía).
- Mỗi tin nhắn là **một khung văn bản JSON** dạng phong bì:

```json
{ "type": "battle_request", "id": 7, "payload": { } }
```

`id` do client đặt, tăng dần; phản hồi của server lặp lại `id` của yêu cầu (`replyTo`). Tin server chủ động gửi có `replyTo: 0`.

## Tin nhắn

| Chiều | `type` | `payload` |
|---|---|---|
| C→S | `hello` | `{ "clientVersion": "0.1.0", "heroName": "..." }` |
| S→C | `hello_ok` | `{ "serverVersion": "0.1.0", "sessionId": "uuid" }` |
| C→S | `battle_request` | `{ "mode": "pve" \| "pvp", "loadout": <Loadout> }` |
| S→C | `battle_result` | `{ "opponent": { "name": "...", "sect": "kim_cang" }, "result": <BattleResult> }` |
| S→C | `error` | `{ "code": "invalid_loadout" \| "bad_request" \| "not_ready" \| "internal", "message": "..." }` |
| C→S / S→C | `ping` / `pong` | `{}` |

Phong bì phản hồi: `{ "type": "battle_result", "replyTo": 7, "payload": {…} }`.

- **`<Loadout>`**: đúng định dạng đội hình của battle-engine (`name, hp, cols, rows, sect, tamPhapLevel, items[{id,x,y,tier}]`, xem [items.md](../design/items.md#đội-hình-loadout)). MVP: server **ép `hp = 1200`, `cols = 5`, `rows = 4`** bất kể client gửi gì (client không được tự nâng máu/túi).
- **`<BattleResult>`**: đúng JSON mà `BattleCli --json` xuất ra (`seed, winner, durationMs, names, finalHp, bags, items[], events[]`). Người gửi yêu cầu luôn là **side 0**.
- Phải gửi `hello` trước; yêu cầu khác trước `hello` → `error not_ready`.

## Server chọn đối thủ (MVP)
- `pvp`: ngẫu nhiên một đội trong `server/battle-engine/samples/` (PvP ghép ngẫu nhiên, quyết định #13).
- `pve`: cũng lấy ngẫu nhiên trong samples cho tới khi có dữ liệu ải.
- Seed: ngẫu nhiên phía server. Trọng/Tâm Pháp của đối thủ giữ như file mẫu.
- Đội hình không hợp lệ (id lạ, chồng ô, ra ngoài túi, sect lạ…) → `error invalid_loadout` kèm thông báo của engine.

## Chưa làm
Tài khoản, lưu đồ trên server, bảng xếp hạng, xác thực, giới hạn tần suất, TLS.
