# Sảnh Luận Kiếm Đài và Đăng nhập (thiết kế UI)

Quyết định #68 (online đợt 1) và #69 (sảnh, bậc). Kích thước theo canvas tham chiếu 1920x1080. Mọi ảnh để kiểu Simple
và đúng tỷ lệ (#66). Không có chữ vẽ sẵn trong ảnh. Chữ tự co và chừa khoảng 30% chỗ cho localize.

## 1. Luồng

```
Home ─ bấm lôi đài (pvp hotspot)
  ├─ chưa kết nối → thử kết nối 3 s → lỗi: "Luận Kiếm Đài cần kết nối máy chủ"
  ├─ chưa đăng nhập → Popup Đăng nhập → xong thì mở Sảnh
  └─ Sảnh Luận Kiếm Đài (popup toàn màn hình, nằm trên Home)
        └─ [Tìm Trận] → kiểm Bao Phục → lớp "Đang tìm đối thủ" → gửi arena_battle
              → thẻ "Đối đầu" (2,5 s hoặc bấm Vào trận) → Gameplay phát lại kết quả đã nhận
              → Bảng kết quả: [Đấu tiếp] (về Sảnh và tìm trận ngay) · [Về Sảnh]
```

- Trận được server chạy **ngay khi bấm Tìm Trận**. Gameplay chỉ phát lại `arena_result` đã nhận, cất trong
  `GameSession`. Như vậy lớp "Đang tìm đối thủ" là thời gian chờ thật, và vào Gameplay không còn bước mạng nào.
- Từ Gameplay về Home thì Sảnh tự mở lại (cờ trong `GameSession`).
- Rớt mạng trong Sảnh: hiện dải báo "Mất kết nối, đang thử lại…". Nút Tìm Trận bị khoá tới khi nối lại.

## 2. Sảnh Luận Kiếm Đài (`ArenaLobbyPopup`, toàn màn hình)

```
┌──────────────────────────────────────────────────────────────────────────────────────────┐
│ [←]  LUẬN KIẾM ĐÀI     Kỳ Luận Kiếm 1 · còn 12 ngày          (DV) 1.240    (NT) 2        │ 0–110
├────────────────┬──────────────────────────────────────┬──────────────────────────────────┤
│  THẺ HIỆP KHÁCH│                                      │ [Xếp Hạng][Lịch Sử][Thưởng][Tiệm]│
│   (huy hiệu    │         tranh nền lôi đài            │ ┌──────────────────────────────┐ │
│    bậc 160px)  │         (để trống ở giữa)            │ │ 1  ◆ Tên……………  1.420  31/8   │ │
│  Tên Hiệp Khách│                                      │ │ 2  ◆ Tên……………  1.388  27/9   │ │
│  Phái · TP 6   │                                      │ │ …                            │ │
│  Bậc Thiếu Hiệp│                                      │ │                              │ │
│  Điểm 1.132    │                                      │ │                              │ │
│  Hạng #17      │                                      │ │                              │ │
│  Thắng 12·Thua 7│   Lượt có thưởng hôm nay: 6/10       │ │                              │ │
│  Chiến Uy 1.180│   Thắng tuần ▓▓▓▓░░░░░░ 3/10 → 1 NT  │ ├──────────────────────────────┤ │
│  [Bao Phục mini│                                      │ │17  ◆ (mình) ……  1.132  12/7  │ │
│   lưới 5x4]    │          [  TÌM TRẬN  ]              │ └──────────────────────────────┘ │
│ [Chỉnh Bao Phục]│                                     │                                  │
└────────────────┴──────────────────────────────────────┴──────────────────────────────────┘
  x 40–460 (420)        x 480–1300 (820)                      x 1320–1880 (560)
  y 130–1040                                                   y 130–1040
```

**Thanh trên (cao 110):**
- Nút quay lại (`button_close` hoặc nút mũi tên nếu có ảnh), tiêu đề "Luận Kiếm Đài" bằng font Display.
- Dòng mùa: "Kỳ Luận Kiếm {n} · còn {d} ngày". Còn dưới 1 ngày thì ghi "còn {h} giờ".
- Bên phải là Danh Vọng và Chí Tôn Ngọc Tủy, có icon, lấy từ hồ sơ trên máy.

**Cột trái: Thẻ Hiệp Khách** (420x910):
- Huy hiệu bậc 160x160.
- Tên Hiệp Khách, môn phái và cấp Tâm Pháp.
- Bậc, điểm, hạng, thắng/thua, Chiến Uy.
- **Bao Phục thu nhỏ**: lưới 5x4, ô 56 px, hiện icon và viền phẩm của túi hiện tại. Đây là đội hình sẽ thành ghost.
- Nút "Chỉnh Bao Phục" mở BagPopup. Đóng BagPopup thì thẻ và Chiến Uy cập nhật.

**Giữa: tranh lôi đài + khu Tìm Trận** (820 rộng):
- Tranh nền phủ cả màn hình, nằm sau mọi cột. Phần giữa tranh để thoáng.
- "Lượt có thưởng hôm nay: {n}/10". Hết lượt thì đổi thành "Hết lượt có thưởng, vẫn được tính điểm".
- Thanh tiến độ thắng tuần (Filled) "{w}/10", bên cạnh là icon Ngọc Tủy. Đã nhận thì ghi "Đã nhận Ngọc Tủy tuần này".
- Nút **Tìm Trận** lớn (ảnh nút chính cỡ lớn, khoảng 420x100).
  - Bao Phục không hợp lệ thì hiện cảnh báo như Home.
  - Chưa có kết quả trận trước thì nút bị khoá.

**Cột phải: 4 tab** (560x910, tab cao 64):
1. **Xếp Hạng:**
   - top 50 cuộn được, mỗi dòng cao 64;
   - dòng gồm hạng (top 1–3 có dấu ấn vàng/bạc/đồng), huy hiệu bậc nhỏ 40 px, tên, điểm, thắng/thua;
   - dòng của mình **ghim ở đáy** và tô nổi; mình nằm trong top 50 thì dòng trong danh sách cũng tô nổi.
2. **Lịch Sử:**
   - 20 trận gần nhất, mỗi dòng gồm Thắng/Thua (dấu son đỏ / mực xám), tên đối thủ (thêm chữ "máy" nếu là ghost
     máy), điểm ±, thời gian ("2 giờ trước");
   - nút **Xem lại** phát lại trận ở Gameplay mà không đổi điểm. Server chạy lại trận từ hai đội hình và seed đã lưu.
3. **Thưởng:**
   - mốc tuần (10 thắng → 1 Chí Tôn Ngọc Tủy);
   - thưởng mỗi trận (thắng 30 / thua 10 Danh Vọng, 10 trận đầu mỗi ngày);
   - bảng bậc và thưởng cuối mùa, ghi "Trao cuối mùa, sắp có".
4. **Tiệm:**
   - thẻ giới thiệu Tiệm Danh Vọng, hiện số Danh Vọng;
   - nút "Vào tiệm" mở Thương Hội ở tab Tiệm Danh Vọng có sẵn. Không làm tiệm thứ hai.

**Lần đầu vào Sảnh:** một dòng hướng dẫn ngắn: "Đội hình trong Bao Phục sẽ thành bóng kiếm cho người khác khiêu
chiến." Dòng này nằm dưới nút Tìm Trận và chỉ hiện một lần.

## 3. Lớp "Đang tìm đối thủ" và thẻ "Đối đầu"

```
┌──────────── nền tối 60% ────────────┐      ┌──────────────── Đối đầu ────────────────┐
│                                     │      │  [huy hiệu]          ĐẤU        [huy hiệu] │
│       (dấu ấn mực xoay 360°/2 s)    │  →   │  Tên mình                       Tên đối thủ │
│       Đang tìm đối thủ…             │      │  Tân Tú · 1.132              Vô Danh · 1.040│
│       (ít nhất 1,2 s)               │      │  Chiến Uy 1.180               Chiến Uy 1.150│
│                                     │      │  Phái                          Phái (· máy) │
└─────────────────────────────────────┘      │              [ Vào trận ] (tự vào sau 2,5 s)│
                                             └─────────────────────────────────────────────┘
```

- Lớp tìm trận hiện ít nhất 1,2 giây cho đỡ giật. Lỗi thì đóng lớp và hiện lỗi bằng tiếng Việt.
- Thẻ Đối đầu cao 520. Chữ "ĐẤU" ở giữa, chia hai nửa trái và phải.

## 4. Bảng kết quả trận Luận Kiếm

- Dùng ResultPopup đang có, thêm dòng "Điểm Luận Kiếm 1.132 → 1.148 (+16)". Đổi bậc thì thêm dòng "Lên bậc Đại Hiệp!".
- Phần thưởng (Danh Vọng, Ngọc Tủy) hiện như thưởng PvE. Hết lượt có thưởng thì ghi "Hôm nay đã hết lượt có thưởng".
- Hai nút: **Đấu tiếp** (về Sảnh và bấm Tìm Trận luôn), **Về Sảnh**.

## 5. Popup Đăng nhập (`AccountPopup`)

```
┌──────────────── Danh Thiếp Giang Hồ ────────────────┐   khung popup vừa (~720x640)
│      [ Đăng nhập ]  [ Đăng ký ]          (2 tab)      │
│                                                       │
│  Tên đăng nhập    [______________________]           │
│  Mật khẩu         [______________________] [mắt]     │
│  Nhập lại         [______________________]  (Đăng ký)│
│  Tên Hiệp Khách   [______________________]  (Đăng ký)│
│                                                       │
│  [✓] Ghi nhớ đăng nhập                                │
│  (dòng lỗi màu son, 1 dòng)                           │
│                                                       │
│        [  Đăng nhập  ]        [  Để sau  ]            │
│  Tài khoản dùng cho Luận Kiếm Đài. Đồ đạc vẫn lưu trên máy này. │
└───────────────────────────────────────────────────────┘
```

- Tab Đăng ký thì hiện thêm hai ô, nút chính đổi thành "Đăng ký".
- Ô Tên Hiệp Khách mặc định là tên trong hồ sơ.
- Nút mắt bật/tắt hiện mật khẩu.
- Ô nhập: Tab và Enter chuyển ô, Enter ở ô cuối là bấm nút chính.
- Đang gửi thì khoá nút và ghi "Đang kết nối…".
- Kiểm ở client trước khi gửi: tên 3–20 ký tự `a-z 0-9 _`, mật khẩu 6–64, hai mật khẩu khớp.
- "Ghi nhớ đăng nhập" tắt thì token chỉ giữ trong phiên chơi, không lưu xuống `PlayerPrefs`.
- Câu lỗi:

| Mã | Câu hiện |
|---|---|
| `auth_failed` | Sai tên đăng nhập hoặc mật khẩu |
| `name_taken` | Tên đăng nhập đã có người dùng |
| `invalid_name` | Tên đăng nhập 3–20 ký tự, chỉ gồm chữ thường không dấu, số và _ |
| `weak_password` | Mật khẩu cần 6–64 ký tự |
| `rate_limited` | Thử sai nhiều lần, đợi một phút rồi thử lại |
| `token_expired` | Phiên đăng nhập đã hết hạn, mời đăng nhập lại |
| mất kết nối | Không kết nối được máy chủ |

**Tài khoản trong Cài đặt:**
- Mục Tài khoản trong bảng Cài đặt hiện "Đang đăng nhập: {tên}" và nút Đăng xuất.
- Chưa đăng nhập thì hiện nút Đăng nhập, mở popup trên.
- Không sửa LoginScene và HomeScene, vì người dùng sửa tay.

## 6. Bậc Luận Kiếm (theo điểm)

| Bậc | Điểm |
|---|---|
| Vô Danh | 1.000–1.099 |
| Tân Tú | 1.100–1.249 |
| Thiếu Hiệp | 1.250–1.399 |
| Đại Hiệp | 1.400–1.599 |
| Danh Túc | 1.600–1.799 |
| Tông Sư | từ 1.800 |

- Người mới bắt đầu 1.000 điểm, tức bậc Vô Danh.
- Thua bị trừ điểm nhưng không xuống dưới 1.000 (#68).
- Người hạng 1 lúc hết mùa nhận danh hiệu **Thiên Hạ Đệ Nhất** (gdd).
- Client tự tính bậc từ điểm. Ngưỡng nằm trong một file dữ liệu `Resources/Data/arena.json`, server không cần biết.

## 7. Ảnh cần có

Đặt ở `Assets/_Game/Textures2D/UI/Arena/`. Codex vẽ theo phong cách Thủy Mặc của bộ kit.

| Ảnh | Kích thước | Ghi chú |
|---|---|---|
| `arena_lobby_bg.png` | 1920x1080 | Lôi đài gỗ giữa sân đá, cờ, núi xa; giữa tranh thoáng để đặt nút |
| `arena_rank_{vo_danh,tan_tu,thieu_hiep,dai_hiep,danh_tuc,tong_su}.png` | 256x256 | Huy hiệu bậc, nền trong suốt, càng cao càng cầu kỳ (mực → đồng → bạc → vàng → son) |
| `arena_seal_spinner.png` | 256x256 | Dấu ấn mực tròn để xoay |
| `arena_medal_{1,2,3}.png` | 96x96 | Dấu hạng vàng/bạc/đồng |
| `arena_versus.png` | 256x160 | Chữ "ĐẤU" không được vẽ vào ảnh; ảnh chỉ là hai thanh kiếm chéo/vệt mực làm nền |

Khung các cột, ô tab và dòng danh sách: lấy trước ảnh có sẵn trong `Textures2D/UI/Kit` nếu tỷ lệ khớp. Chỗ nào không
khớp thì Claude bake bằng `tools/ui-art/bake_nine_slice.py` từ khung cùng bộ.
