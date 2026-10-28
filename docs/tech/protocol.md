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
| S→C | `auth_ok` | `{ "accountId": 12, "username": "...", "heroName": "...", "token": "...", "expiresAt": "ISO-8601", "seasonReward": <SeasonReward>? }` |
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
| S→C | `arena_result` | xem dưới |
| C→S | `leaderboard` | `{ "limit": 50 }` (tối đa 50) |
| S→C | `leaderboard_ok` | `{ "seasonId": 1, "entries": [ { "rank": 1, "heroName": "...", "rating": 1240, "wins": 9, "losses": 2 } ], "self": { "rank": 17, "rating": 1032 } }` |
| C→S | `season_reward_seen` | `{ "seasonId": 1 }` |
| S→C | `season_reward_seen_ok` | `{ "seasonId": 1 }` |

`<ArenaStatus>`:

```json
{
  "seasonId": 1, "seasonEndsAt": "2026-11-15T00:00:00+07:00",
  "rating": 1000, "peakRating": 1048, "rank": 17, "wins": 3, "losses": 1,
  "rewardedLeftToday": 6, "weeklyWins": 3, "weeklyWinsTarget": 15, "weeklyJadeClaimed": false,
  "seasonReward": { "seasonId": 1, "rank": 17, "rating": 1032, "reputation": 600, "jade": 0 }
}
```

`seasonReward` là `<SeasonReward>` và chỉ có khi phần thưởng chưa được xem. Nó xuất hiện trong cả `auth_ok` và
`arena_status_ok` cho tới khi client gửi `season_reward_seen` đúng `seasonId`. Server chốt mùa 28 ngày theo UTC+7,
lưu hạng và điểm cuối mùa, rồi đặt điểm và `peakRating` mùa mới về 1.000. `peakRating` chỉ được lưu, chưa dùng tính thưởng.

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
- Mốc Ngọc Tủy tuần là 15 trận thắng; client phải đọc `weeklyWinsTarget`, không viết cứng con số này.

### Hàng chờ ghép trận (#70)

Thay `arena_battle`. Trận vẫn đánh với ghost, nhưng server chỉ ghép khi có ghost trong khoảng điểm đang tìm.

| Chiều | `type` | `payload` |
|---|---|---|
| C→S | `arena_find` | `{ "loadout": <Loadout>, "power": 1234 }` |
| S→C | `arena_searching` | `{ "searchId": "uuid", "rating": 1032, "range": 50, "startedAt": "ISO-8601", "nextWidenAt": "ISO-8601" }` |
| S→C | `arena_result` | như cũ, thêm `"searchId"` |
| C→S | `arena_cancel` | `{}` |
| S→C | `arena_cancel_ok` | `{ "searchId": "uuid" }` |

- Nhận `arena_find` thì server lưu ghost mới của người chơi, rồi tìm ngay các ghost người thật (không phải của mình)
  có điểm trong `rating ± range`, với `range` bắt đầu = 50:
  - **có:** chọn ngẫu nhiên một ghost, đánh luôn, trả `arena_result` (`replyTo` = id của `arena_find`);
  - **không có:** trả `arena_searching` (`replyTo` = id của `arena_find`), giữ lượt tìm trong bộ nhớ.
- Cứ **2 phút** tính từ `startedAt`, `range` tăng thêm 50. Server **chủ động gửi** `arena_searching` mới
  (`replyTo: 0`) mỗi khi nới, và kiểm lại hàng chờ mỗi 5 giây. Tìm được thì chủ động gửi `arena_result` (`replyTo: 0`).
- Cả mùa chưa có ghost người thật nào khác thì đánh ngay với ghost máy (`bot: true`), để thử một mình được.
- Mỗi tài khoản chỉ có một lượt tìm. Gửi `arena_find` khi đang tìm thì nhận `error already_searching`.
- `arena_cancel` hoặc mất kết nối thì huỷ lượt tìm. Huỷ khi không có lượt nào thì vẫn trả `arena_cancel_ok`.
- Không còn lọc theo Chiến Uy. `power` vẫn được lưu cùng ghost để hiện trên thẻ Đối đầu.

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

## Hồ sơ trên server (E6, #70)

Server giữ hồ sơ của mỗi tài khoản và quyết định mọi thay đổi. Client không lưu hồ sơ xuống đĩa. Nó chỉ giữ bản hồ sơ
mới nhất trong bộ nhớ, nhận từ server.

### Hồ sơ

- `<Profile>` có **đúng định dạng JSON** mà client đang lưu ở `profile.json`: `PlayerProfile.ToJson`, `version` 9,
  cùng tên trường (`heroName`, `currencies`, `items`, `loadout`, `storage`, `shop`, `pve`, `quest`…). Client đọc lại
  bằng `PlayerProfile.FromJson` có sẵn.
- Server lưu hồ sơ vào bảng `profiles` (`account_id`, `json`, `rev`, `updated_at`). `rev` tăng 1 sau mỗi thay đổi.
- `register` tạo hồ sơ mới từ bộ khởi đầu (`starter.json`), với `heroName` của tài khoản.
- `auth_ok` (từ `register`, `login`, `resume`) có thêm `"profile": <Profile>`, `"rev"` và `"serverTime"`.
- Mọi tin `*_ok` dưới đây đều có `"profile": <Profile>`, `"rev": n`, `"serverTime": "ISO-8601"` cùng các trường riêng.
  Client thay **toàn bộ** hồ sơ đang giữ bằng bản mới, rồi báo UI vẽ lại.
- `serverTime` cho client biết lệch giờ, để đếm ngược Thể Lực, Thương Hội và mùa. Client không tự đổi hồ sơ theo giờ máy.
- Thể Lực và các chu kỳ ngày/tuần của Thương Hội: server tự cập nhật mỗi khi đọc hồ sơ, theo đúng luật hiện có của
  client (`StaminaService`, `ShopService.ResetPeriods`).
- Offer Tiệm Danh Vọng có thể có `periodWeeks` trong `shop.json` (mặc định 1). Chu kỳ bắt đầu ở mốc thứ Hai UTC+7
  của Thương Hội; `chi_ton_ngoc_tuy` dùng `periodWeeks: 2`, giá 800 và giới hạn 1 lần trong mỗi chu kỳ.

### Tin nhắn

| Chiều | `type` | `payload` | Trường riêng trong `*_ok` |
|---|---|---|---|
| C→S | `profile_get` | `{}` | |
| C→S | `loadout_save` | `{ "placements": [ { "uid": "...", "x": 0, "y": 0 } ] }` | |
| C→S | `storage_move` | `{ "uid": "..." }` hoặc `{ "materialId": "...", "fromX": 0, "fromY": 0 }`, kèm `"toX"`, `"toY"` | |
| C→S | `storage_repack` | `{}` | |
| C→S | `item_upgrade` | `{ "uid": "..." }` | `"tier"` mới |
| C→S | `item_merge` | `{ "firstUid": "...", "secondUid": "..." }` | `"resultUid"` |
| C→S | `item_sell` | `{ "uid": "..." }` | `"silver"` nhận được |
| C→S | `shop_refresh` | `{ "paid": false }` | |
| C→S | `shop_buy` | `{ "slot": 0 }` | `"uid"` món vừa mua |
| C→S | `shop_exchange` | `{ "inputId": "...", "targetId": "..." }` | |
| C→S | `shop_buy_reputation` | `{ "offerId": "..." }` | |
| C→S | `sect_choose` | `{ "sectId": "kim_cang" }` | |
| C→S | `quest_claim` | `{ "questId": "..." }` | `"reward"` |
| C→S | `onboarding` | `{ "action": "intro_seen" \| "tutorial_done" \| "story_seen" \| "sect_prompt_shown", "id": "..." }` | |
| C→S | `pve_battle` | `{ "stageId": "hoi_1_01" }` | xem dưới |
| C→S | `dev_cheat` | `{ "action": "...", ... }` | |

- Mỗi tin `C→S` được trả bằng tin cùng tên thêm `_ok` (ví dụ `item_upgrade` → `item_upgrade_ok`), hoặc `error`.
- **Lỗi luật chơi** (thiếu Bạc, kho đầy, chưa đủ cấp…): `error` với `code: "rejected"` và `message` là câu tiếng Việt
  giống câu client đang hiện (ví dụ "Vật phẩm đã đạt Cửu Trọng."). Hồ sơ không đổi.
- Server xử lý **từng yêu cầu một** cho mỗi tài khoản. Client khoá nút hành động trong lúc chờ trả lời.
- Ngẫu nhiên (Thương Hội, rơi đồ, seed trận PvE) do server bốc bằng RNG của nó. Client không cần tính trùng.
- `pve_battle`:
  - server lấy Bao Phục trong hồ sơ, trừ Thể Lực, chạy trận với quái của ải từ seed của nó, rồi cộng thưởng (Bạc, EXP,
    nguyên liệu, vật phẩm, nhiệm vụ, cốt truyện) theo đúng luật hiện có (`PveRewardService`, `GameplayController.ShowPveResult`);
  - `pve_battle_ok`: `{ "stageId", "won", "firstClear", "result": <BattleResult>, "reward": { "silver", "heroExp",
    "materials": {}, "items": [ "uid" ], "levelUps": 0 }, "enemy": { "name", "sectId", "level" }, "profile", "rev",
    "serverTime" }`.
- `dev_cheat` chỉ chạy khi server bật cờ `--dev`, nếu không thì `error forbidden`. Các `action` giống nút trên
  `DevCheatPanel`.
- **Luận Kiếm:** `arena_find` lấy Bao Phục và Chiến Uy từ hồ sơ trên server (payload `{}`). `arena_result` có thêm
  `profile`, `rev`, `serverTime`, vì thưởng Danh Vọng và Ngọc Tủy đã được cộng trên server.

### Dữ liệu dùng chung

- Server đọc các file JSON trong `One-Bag-Jianghu/Assets/_Game/Resources/Data` qua tham số `--data`. Khi chạy dev,
  mặc định là thư mục đó. Ảnh Docker chép thư mục này vào lúc build, vì vậy build context là gốc repo.
- Logic Java phải cho kết quả **giống hệt** các lớp C# đang có (`UpgradeService`, `MergeService`, `SellService`,
  `ShopService`, `StorageGrid`, `LevelService`, `PveRewardService`, `QuestService`…), kể cả cách làm tròn. Client giữ
  các lớp C# đó để xem trước chi phí, nhưng không dùng chúng để đổi hồ sơ nữa.

## Chưa làm
Thưởng cuối mùa, chia bảng theo cảnh giới, đổi mật khẩu.
