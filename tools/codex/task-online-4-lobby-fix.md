# Task: Sửa bố cục prefab Sảnh Luận Kiếm Đài và Đăng nhập

Bạn là Codex, sửa `One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs`. Code này do bạn viết ở đợt
trước. Claude đã chạy builder trong Unity và chụp ảnh prefab. Bố cục đang hỏng nặng, xem hai ảnh:
- `One-Bag-Jianghu/Logs/Snapshots/ArenaLobbyPopup.png`
- `One-Bag-Jianghu/Logs/Snapshots/AccountPopup.png`

## Đọc trước
- `docs/design/arena-lobby.md`: bố cục đích, mục 2, 3 và 5.
- `docs/tech/code-style.md`.
- Cách các popup khác dựng đúng: `Editor/Building/QuestUiBuilder.cs` (`BuildLog`, gốc có Canvas + GraphicRaycaster,
  được kéo giãn) và `EconomyPopupPrefabBuilder.cs`.

## Lỗi Claude đã tìm ra
1. **Chữ dựng đứng thành cột dọc:** `Label()` gọi `Set(rect, min, max, 0f)`, nên mọi khung chữ rộng 0 px. `Set()`
   neo x ở 0.5 với width cố định, nên không dùng được cho chữ cần rộng.
2. **Sảnh dồn vào giữa:**
   - gốc `ArenaLobbyPopup` không được kéo giãn, nên chỉ to 100x100 và mọi neo theo tỷ lệ co lại theo;
   - gốc thiếu `Canvas` + `GraphicRaycaster` như các popup khác;
   - `AccountPopup` cũng vậy.
3. **Lớp "Matching Layer" và "Versus Card"** đang bật sẵn trong prefab. Hai lớp này phải tắt mặc định.
4. **Popup Đăng nhập:**
   - khung chọn `popup_frame_square` nhưng kích thước 720x640 không khớp tỷ lệ ảnh (1960x1800, tỷ lệ 1.09);
   - ô "Ghi nhớ" là ô trắng trơn;
   - nút mắt ghi chữ "Mắt";
   - dòng ghi chú bị đặt ngoài panel (neo -0.01..0.04).

## Ảnh đã có (Claude bake, đúng tỷ lệ)
Mọi ảnh nằm trong `Textures2D/UI/Arena/`, kiểu **Simple**, đặt **đúng kích thước** dưới đây:

| Ảnh | Kích thước (canvas 1920x1080) |
|---|---|
| `arena_top_bar` | 1920x110 |
| `arena_panel_hero` | 420x910 |
| `arena_panel_match` | 820x360 |
| `arena_panel_tabs` | 560x910 |
| `arena_versus_card` | 1080x520 |
| `arena_row_rank`, `arena_row_self` | 520x64 |
| `arena_row_history` | 520x72 |
| `arena_progress_track`, `arena_progress_fill` (Filled ngang) | 580x24 |
| `arena_connection_banner` | 820x56 |

Ảnh do task vẽ đang làm (có thể chưa có file):
- `arena_lobby_bg` 1920x1080;
- `arena_rank_*` 256 (hiển thị 160 ở thẻ, 40 ở dòng);
- `arena_seal_spinner` 256;
- `arena_medal_1..3` 96 (hiển thị 48);
- `arena_versus` 256x160.

## Bố cục đích (pixel, gốc toạ độ góc trên trái màn 1920x1080)
- **Gốc:**
  - kéo giãn toàn màn hình;
  - có `Canvas` (override sorting như popup khác) + `GraphicRaycaster` + `CanvasGroup`;
  - `CanvasScaler` theo cách các popup khác đang làm (xem QuestLogPopup hoặc PopupManager).
- **Nền:** `arena_lobby_bg` kéo giãn đúng 1920x1080.
- **Thanh trên** 1920x110 ở y 0:
  - nút quay lại 64x64 ở x 24;
  - tiêu đề "Luận Kiếm Đài" font Display 36, x 110–520;
  - dòng mùa x 560–1180;
  - Danh Vọng và Ngọc Tủy x 1260–1880, mỗi mục là icon 32 + số (icon `icon_danh_vong`/`chi_ton_ngoc_tuy` lấy qua
    `CurrencyArt` như Shop).
- **Thẻ Hiệp Khách** 420x910 ở (40, 130). Toạ độ trong thẻ, tính từ góc trên:
  - huy hiệu 160 ở tâm (210, 120);
  - tên y 215–255 (cỡ 26);
  - phái · Tâm Pháp y 255–285;
  - bậc y 295–330;
  - điểm/hạng y 335–370;
  - thắng/thua y 370–405;
  - Chiến Uy y 405–440;
  - Bao Phục mini 5x4 ô 56 + cách 4 (rộng 296, cao 236) căn giữa ở y 470–706;
  - nút "Chỉnh Bao Phục" 300x62 ở tâm (210, 820).
  - Chữ căn giữa, mọi chữ có lề trái/phải 28.
- **Khu Tìm Trận** 820x360 ở (480, 680):
  - "Lượt có thưởng" y 30–70;
  - thanh tuần 580x24 ở y 100, căn giữa, kèm icon Ngọc Tủy 40 bên phải và chữ "x/10" bên trái;
  - nút Tìm Trận 420x100 ở tâm y 220;
  - dòng hướng dẫn y 300–340, cỡ 16.
- **Bảng tab** 560x910 ở (1320, 130):
  - 4 nút tab 124x56 cách nhau 8, hàng ở y 24 trong bảng (dùng `tab_active`/`tab_idle`, tỷ lệ 4.24, nên kích thước
    là 124x29 hoặc chữ trên nền tab; chọn cách đúng tỷ lệ ảnh, có thể dùng ảnh tab ở 236x56 xếp 2x2 nếu đẹp hơn);
  - vùng nội dung x 20–540, y 100–890;
  - Xếp Hạng: danh sách cuộn y 100–810 (dòng 520x64, cách 6), dòng của mình ghim y 820–884.
- **Dải mất kết nối** 820x56 ở (480, 130), mặc định tắt.
- **Lớp tìm trận:**
  - nền tối kéo giãn;
  - dấu ấn 220 ở tâm màn hình (lệch lên 60);
  - chữ trắng 30 bên dưới, rộng 800.
  - Mặc định tắt.
- **Thẻ Đối đầu:**
  - 1080x520 ở giữa màn hình;
  - hai nửa trái/phải rộng 420, mỗi nửa gồm huy hiệu 120, tên, bậc · điểm, Chiến Uy, phái;
  - giữa là `arena_versus` 256x160 với chữ "ĐẤU" cỡ 44 đè lên;
  - nút Vào trận 300x62 ở đáy y 440.
  - Mặc định tắt.
  - Thêm field cho huy hiệu, bậc, Chiến Uy của hai bên nếu `ArenaLobbyPopup` chưa có; sửa script cho khớp.
- **Dòng xếp hạng** 520x64:
  - medal 48 hoặc số hạng ở x 12–72;
  - huy hiệu bậc 40 ở x 84;
  - tên x 136–330;
  - điểm x 330–420;
  - thắng/thua x 420–508.
- **Dòng lịch sử** 520x72:
  - kết quả x 12–80;
  - đối thủ x 80–250;
  - điểm ± x 250–320;
  - thời gian x 320–410;
  - nút Xem lại 100x36 (`button_small_gold_compact` hoặc ảnh nút có tỷ lệ gần 2.8, xem kit) ở x 410–510.
- **Popup Đăng nhập:**
  - khung `popup_frame_square` ở **720x660** (tỷ lệ 1.09);
  - tiêu đề nằm trên dải ribbon như các popup khác (xem `PopupPrefabBuilder` phần ribbon `popup_title_ribbon`);
  - ô nhập dùng prefab `InputField` chung, cỡ 430x56 (ảnh `tooltip_field` 5.62 → **430x76** nếu giữ tỷ lệ; chọn
    76 cao);
  - nút mắt 56x56 dùng `button_square`, không có chữ (icon thiếu thì để trống và ghi vào báo cáo);
  - Ghi nhớ dùng prefab `Toggle` chung trong `Prefabs/UI/Common/Toggle.prefab`;
  - ghi chú cỡ 14 nằm **trong** panel, ngay trên mép dưới.
  - Khi ở tab Đăng nhập, hai ô của tab Đăng ký bị ẩn, nên các phần bên dưới phải dồn lên. Dùng VerticalLayoutGroup
    cho cột ô nhập là cách gọn.

## Luật
- Viết một helper rõ ràng, ví dụ `Place(RectTransform rect, float x, float y, float w, float h)`, theo pixel từ góc
  trên trái của cha (anchor và pivot 0,1). Dùng nó thay cho các neo tỷ lệ lẫn lộn.
- Chữ dùng `UiBuild.FitLabel` hoặc best fit, không dùng width 0.
- Không đổi tên field mà `ArenaLobbyPopup`, `AccountPopup`, `LeaderboardRow`, `ArenaHistoryRow` đang dùng, trừ khi
  sửa luôn cả script.

## Được phép sửa
- `Editor/Building/OnlinePrefabBuilder.cs`
- `Scripts/UI/Arena/**`, `Scripts/UI/Popups/AccountPopup.cs`, `LeaderboardRow.cs` (chỉ khi cần field mới)
- `Tests/EditMode/**` liên quan

**Cấm:** mọi lệnh git, chạy Unity, sửa ảnh, sửa scene.

## Tự kiểm tra
Mọi file đã sửa phải qua `py tools/lint/check_style.py <file>`, chạy ở thư mục gốc repo. Claude sẽ chạy builder và
chụp lại ảnh.

## Báo cáo
Viết ngắn bằng tiếng Việt: đã sửa gì, field mới, chỗ chưa chắc.
