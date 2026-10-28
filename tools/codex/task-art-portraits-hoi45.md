# Task (Codex, image_gen): Vẽ 18 chân dung quái Hồi 4–5 và 4 chân dung đối thủ Luận Kiếm

Bạn là Codex. Dùng skill vẽ ảnh có sẵn (`image_gen`) để vẽ ảnh cho game One Bag Jianghu. Đây là game võ hiệp, phong
cách **Thủy Mặc**: tranh thuỷ mặc, nét mực, màu nhạt trên giấy cổ.

## Tham chiếu phong cách
Mở và bám sát các ảnh sau:
- `docs/art/reference/01-style-board-thuy-mac.png`: bảng phong cách;
- `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/boss_lo_tan_quang.png`: chân dung người, Hồi 4;
- `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/boss_tao_huyen_ky.png`: chân dung người, Hồi 5;
- `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/monster_ngan_lang.png`: chân dung quái thú;
- `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/enemy_portrait_1.png`: kẻ địch thường.

Quy chuẩn chân dung trong `docs/art/prompts-pve.md` (mục "Đợt 2"):
- nửa thân, khổ dọc **1024x1536**, **nền trong suốt**;
- kẻ địch **quay mặt sang trái** (three-quarter view facing left), từ thắt lưng trở lên;
- quái thú thấy đầu và nửa thân trước;
- **không có chữ** trong ảnh.

Bối cảnh từng Hồi:
- `docs/art/prompts-pve.md`, mục "Đợt 3: Hồi 4–5";
- `docs/reviews/2026-10-04-codex-pve-hoi-4-5.md`, nếu có;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: tên, phái, mô tả ải;
- `Resources/Data/sects.json`: phong cách từng phái.

## Danh sách cần vẽ
Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/<key>.png`.

| Key | Tên | Phái |
|---|---|---|
| enemy_hoi_4_01 | Dịch Lộ Đao Khách | thiet_phu |
| enemy_hoi_4_02 | Giả Lệnh Dịch Tốt | thai_thanh |
| enemy_hoi_4_03 | Mặc Ảnh Linh Miêu | quái thú (mèo linh bóng mực) |
| enemy_hoi_4_04 | Tế Vân Chưởng Quỹ | bich_thuy |
| enemy_hoi_4_05 | U Minh Vũ Y Vệ | huyen_doc |
| enemy_hoi_4_06 | Thiết Phủ Khai Lộ Sứ | thiet_phu |
| enemy_hoi_4_07 | Xích Giác Cuồng Ngưu | quái thú (trâu sừng đỏ cuồng nộ) |
| enemy_hoi_4_08 | Hàn Sơn Tín Tiêu Vệ | han_son |
| enemy_hoi_4_09 | U Minh Khóa Giang Vệ | kim_cang |
| enemy_hoi_5_01 | Giả Chiếu Cấm Vệ | kim_cang |
| enemy_hoi_5_02 | Chu Tước Ảnh Khách | thai_thanh |
| enemy_hoi_5_03 | Huyền Đồng Cơ Sư | người chế cơ quan (con rối đồng) |
| enemy_hoi_5_04 | Ngụy Học Kiếm Sinh | thai_thanh |
| enemy_hoi_5_05 | Thiên Cơ Chấp Ấn Quan | thiet_phu |
| enemy_hoi_5_06 | U Minh Ám Cừ Sứ | huyen_doc |
| enemy_hoi_5_07 | Xích Nhật Kim Sí Điểu | quái thú (chim cánh vàng lửa đỏ) |
| enemy_hoi_5_08 | Thái Dịch Cầm Sư | bich_thuy |
| enemy_hoi_5_09 | Thiên Cơ Hỏa Nỏ Vệ | liet_diem |

Thêm 4 chân dung đối thủ Luận Kiếm, lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Arena/pvp_avatar_<n>.png`:
- 4 hiệp khách khác nhau về giới tính, tuổi và vũ khí, quay sang trái;
- trông như người chơi khác chứ không phải quái, trang phục trung tính, không gắn một phái cụ thể.

Mỗi ảnh:
- viết prompt tiếng Anh theo mẫu trong `docs/art/prompts-pve.md`, ví dụ
  "Half-body portrait of …, three-quarter view facing left, from the waist up, transparent background, no text.";
- trang phục và vũ khí hợp phái;
- **không dùng tên hay hình nhân vật trong truyện Kim Dung**.

Sau khi vẽ:
- chép ảnh từ `~/.codex/generated_images/` sang đúng đường dẫn;
- kiểm ảnh **có kênh alpha** và nền trong suốt;
- ảnh ra nền trắng thì tách nền (Python + Pillow, ngưỡng màu nền), giữ viền mềm.

## Luật
- Chỉ tạo các file PNG trên. Ghi bảng prompt đã dùng vào `docs/art/prompts-pve.md`, mục mới "Đợt 4: chân dung Hồi 4–5 và
  Luận Kiếm".
- **Không** sửa code, prefab, `PveArtLibrary.asset` hay file `.meta`. Claude sẽ nối ảnh vào game.
- Không chạy git, không chạy Unity.
- Ảnh nào vẽ hỏng (sai hướng, có chữ, không trong suốt) thì vẽ lại tối đa 1 lần. Vẫn hỏng thì ghi vào báo cáo.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`: danh sách file đã tạo, ảnh nào cần xem lại.
