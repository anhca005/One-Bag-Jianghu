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

## Tài khoản và Luận Kiếm Đài (#68)

Mọi tin dưới đây đều cần `hello` trước. Các tin `arena_*` và `leaderboard` cần đăng nhập trước, nếu chưa thì server
trả `error unauthorized`.

### Tài khoản

| Chiều | `type` | `payload` |
|---|---|---|
| C→S | `register` | `{ "username": "...", "password": "...", "heroName": "..." }` |
| C→S | `login` | `{ "username": "...", "password": "..." }` |
| C→S | `resume` | `{ "token": "..." }` |
| S→C | `auth_ok` | `{ "accountId": 12, "username": "...", "heroName": "...", "token": "...", "expiresAt": "ISO-8601" }` |
| C→S | `logout` | `{}`: xoá token đang dùng; server trả `logout_ok {}` |

- `username`: 3–20 ký tự `[a-z0-9_]`, server đổi về chữ thường. `password`: 6–64 ký tự. `heroName`: 1–24 ký tự, bỏ
  khoảng trắng hai đầu.
- Mật khẩu băm PBKDF2WithHmacSHA256, 120.000 vòng, salt 16 byte. Token là 32 byte ngẫu nhiên (base64url), sống 30 ngày.
- Một kết nối đăng nhập sai quá 5 lần trong 1 phút thì bị từ chối `rate_limited` tới hết phút đó.
- Lỗi: `invalid_name`, `weak_password`, `name_taken`, `auth_failed` (sai tên hoặc mật khẩu, không nói rõ cái nào),
  `token_expired`, `rate_limited`.

### Luận Kiếm Đài

| Chiều | `type` | `payload` |
|---|---|---|
| C→S | `arena_status` | `{}` |
| S→C | `arena_status_ok` | `<ArenaStatus>` |
| C→S | `arena_battle` | `{ "loadout": <Loadout>, "power": 1234 }` |
| S→C | `arena_result` | xem dưới |
| C→S | `leaderboard` | `{ "limit": 50 }` (tối đa 50) |
| S→C | `leaderboard_ok` | `{ "seasonId": 1, "entries": [ { "rank": 1, "heroName": "...", "rating": 1240, "wins": 9, "losses": 2 } ], "self": { "rank": 17, "rating": 1032 } }` |

`<ArenaStatus>`:

```json
{
  "seasonId": 1, "seasonEndsAt": "2026-11-15T00:00:00+07:00",
  "rating": 1000, "rank": 17, "wins": 3, "losses": 1,
  "rewardedLeftToday": 6, "weeklyWins": 3, "weeklyWinsTarget": 10, "weeklyJadeClaimed": false
}
```

`arena_result`:

```json
{
  "opponent": { "name": "...", "sect": "kim_cang", "rating": 1012, "power": 1180, "bot": false },
  "result": <BattleResult>,
  "ratingBefore": 1000, "ratingAfter": 1016,
  "reward": { "reputation": 30, "materials": { "chi_ton_ngoc_tuy": 1 } },
  "status": <ArenaStatus>
}
```

- Người chơi luôn là side 0. `reward` có thể rỗng (`reputation: 0`, `materials: {}`) khi đã hết 10 trận có thưởng.
- Client cộng `reward` vào hồ sơ trên máy ngay khi nhận `arena_result`, trước khi phát lại trận.
- Lỗi: `invalid_loadout` (kèm thông báo của engine), `unauthorized`.
- Ghép trận, Elo, mùa, mốc thưởng: theo #68. Điểm không bao giờ xuống dưới 1000, kể cả điểm của ghost bị đánh thua.

### Lịch sử và xem lại (#69)

| Chiều | `type` | `payload` |
|---|---|---|
| C→S | `arena_history` | `{ "limit": 20 }` (tối đa 20) |
| S→C | `arena_history_ok` | `{ "entries": [ { "battleId": 88, "at": "ISO-8601", "won": true, "opponentName": "...", "opponentSect": "kim_cang", "bot": false, "ratingBefore": 1000, "ratingAfter": 1016 } ] }` |
| C→S | `arena_replay` | `{ "battleId": 88 }` |
| S→C | `arena_replay_ok` | `{ "opponent": { …như arena_result… }, "result": <BattleResult> }` |

- Server lưu mỗi trận gồm hai đội hình đã chuẩn hoá, seed, kết quả và thay đổi điểm. Xem lại thì chạy lại engine từ
  hai đội hình và seed đó, nên không cần lưu nhật ký sự kiện. Người hỏi luôn là side 0.
- Chỉ xem lại được trận của chính mình. Trận của người khác → `error not_found`.

`battle_request` cũ vẫn giữ để thử engine, nhưng game không dùng cho Luận Kiếm Đài nữa.

## Chưa làm
Kho đồ và kinh tế trên server (E6), thưởng cuối mùa, chia bảng theo cảnh giới, TLS, đổi mật khẩu.
