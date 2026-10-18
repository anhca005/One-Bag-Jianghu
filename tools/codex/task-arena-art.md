# Task: Vẽ ảnh cho Sảnh Luận Kiếm Đài

Bạn là Codex. Dùng công cụ tạo ảnh (image_gen) để vẽ ảnh cho game One Bag Jianghu, một game võ hiệp phong cách
**Thủy Mặc** (tranh mực nước, giấy dó ấm, điểm son đỏ, viền vàng đồng như bộ UI kit).

## Đọc / xem trước
- `docs/design/arena-lobby.md` mục 2, 3 và 7.
- Ảnh tham chiếu phong cách: `docs/art/reference/01-style-board-thuy-mac.png`.
- Bộ UI kit có sẵn để khớp màu và nét: `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/` (xem `popup_frame.png`,
  `quest_badge_main.png`, `button_primary_compact.png`, `quest_status_done.png`).
- Tranh nền Home để khớp không khí: tìm trong `One-Bag-Jianghu/Assets/_Game/Textures2D/` (ảnh nền Home/Login).

## Ảnh cần vẽ (lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Arena/`)

| File | Kích thước cuối | Nội dung |
|---|---|---|
| `arena_lobby_bg.png` | 1920x1080, không trong suốt | Lôi đài gỗ vuông giữa sân đá, cờ phướn, mái ngói, núi xa trong sương. Hai bên trái (x 40–460) và phải (x 1320–1880) sẽ bị panel che, nên chi tiết đẹp nhất đặt ở giữa. Vùng giữa-dưới (x 700–1100, y 760–960) để thoáng, chỗ đó đặt nút. Tông dịu, không quá tương phản để chữ đọc được. |
| `arena_rank_vo_danh.png` | 256x256, nền trong suốt | Huy hiệu bậc thấp nhất: vòng mực đơn giản, giấy dó |
| `arena_rank_tan_tu.png` | 256x256, trong suốt | Huy hiệu mực + viền đồng mảnh |
| `arena_rank_thieu_hiep.png` | 256x256, trong suốt | Viền đồng, có một thanh kiếm |
| `arena_rank_dai_hiep.png` | 256x256, trong suốt | Viền bạc, hai kiếm chéo |
| `arena_rank_danh_tuc.png` | 256x256, trong suốt | Viền vàng, kiếm + mây |
| `arena_rank_tong_su.png` | 256x256, trong suốt | Viền vàng son, rồng mây, cầu kỳ nhất |
| `arena_seal_spinner.png` | 256x256, trong suốt | Dấu ấn mực tròn, có hoạ tiết xoay được (không đối xứng gương hoàn toàn để thấy đang xoay) |
| `arena_medal_1.png`, `_2`, `_3` | 96x96, trong suốt | Dấu hạng vàng / bạc / đồng, hình tròn có viền |
| `arena_versus.png` | 256x160, trong suốt | Hai thanh kiếm chéo trên vệt mực, làm nền cho chữ ĐẤU |

## Luật bắt buộc
- **Không có chữ, số, ký tự** nào vẽ trong ảnh. Game sẽ localize.
- Sáu huy hiệu bậc cùng một bộ: cùng góc nhìn, cùng khung tròn, cùng kích cỡ phần hình. Phần hình chiếm khoảng 90%
  ảnh, căn giữa, không bị cắt mép.
- Ảnh trong suốt phải có alpha sạch. Không để nền trắng hay xám, không có viền răng cưa, không có quầng.
- Kích thước cuối đúng như bảng. Vẽ to hơn thì thu nhỏ lại bằng LANCZOS. **Không** kéo giãn lệch tỷ lệ, không độn
  viền trong suốt để ép tỷ lệ.
- Lưu PNG. Không tạo file `.meta`.

## Được phép
- Tạo thư mục và các file PNG ở trên.
- Viết script phụ, nếu cần, trong `tools/codex/out/arena-art/`.

**Cấm:** mọi lệnh git; sửa bất kỳ file nào khác.

## Tự kiểm tra
Viết một script Python kiểm từng file:
- đúng kích thước;
- ảnh trong suốt có alpha ở 4 góc bằng 0;
- bbox của phần hình nằm trong ảnh, không chạm mép quá 2 px.

Ghép một tấm xem trước `tools/codex/out/arena-art/preview.png`: tranh nền, rồi 6 huy hiệu xếp hàng trên nền giấy,
rồi medal và versus.

## Báo cáo
Viết ngắn bằng tiếng Việt, lưu ở file `-o`:
- danh sách file, kích thước, kết quả script kiểm;
- ảnh nào chưa ưng ý và lý do.
