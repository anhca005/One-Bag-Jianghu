# Đặc Tả Giao Diện & Trải Nghiệm Người Dùng (UI/UX Spec) — Nhất Nang Giang Hồ

**Hệ máy:** PC (chuẩn 1920×1080, hỗ trợ co giãn 16:10). **Phong cách mỹ thuật:** Thủy Mặc (Quyết định #11).  
**Đối tượng sử dụng:** Lập trình viên Unity (triển khai UGUI/Code) và Họa sĩ 2D (tạo tài nguyên/prompt ChatGPT).  
**Nguyên tắc thuật ngữ:** Chuẩn hóa tuyệt đối theo `docs/design/glossary.md`.

---

## 1. Hệ Thống Giao Diện Dùng Chung (UI Kit)

### 1.1. Bảng màu & Bậc độ hiếm
- **Chủ đạo Thủy Mặc:** Nền giấy dó `#EFE6D2` · Mực đậm `#1E1B18` · Mực nhạt `#6B6760` · Xám xanh núi `#8A9A94` · Son đỏ `#B8322A` · Vàng đất `#C9A45C`.
- **Màu khung độ hiếm (Border/Glow):** Phàm Phẩm `#9A958C` · Lương Phẩm `#6F8F5E` · Tinh Phẩm `#4F7A99` · Tuyệt Phẩm `#7C5A8E` · Thần Binh `#C9A045` · Chí Tôn `#B8322A`.

### 1.2. Kiểu dáng thành phần cơ bản
- **Nút bấm chính (Primary Button):** Nền Son đỏ `#B8322A`, chữ giấy dó `#EFE6D2`, viền chỉ Vàng đất `#C9A45C` (2px). Hover: tăng 15% độ sáng; Pressed: thụt xuống 2px; Disabled: xám mực `#6B6760` (alpha 40%).
- **Nút bấm phụ (Secondary Button):** Nền Mực mờ `#1E1B18` (alpha 12%), chữ Mực đậm, viền Mực nhạt 1.5px. Hover: nền mực alpha 25%.
- **Khung giao diện (Panel Frames):** Nền giấy dó có họa tiết xơ giấy mờ, viền brush mực loang góc vuông, góc đóng con dấu triện son nhỏ.
- **Lưới khoảng cách (Spacing Grid):** Bội số 8 px (8, 16, 24, 32, 48, 64 px). Căn lề padding mặc định 24 px.
- **Cỡ chữ (Typography - 1920×1080):** Tiêu đề lớn (Title) `48–56px` · Đề mục (Heading) `28–34px` · Thân bài (Body) `20–24px` · Chú thích/Chỉ số phụ (Caption) `14–16px`. Font thư pháp có chân có dấu tiếng Việt.
- **Thứ tự lớp hiển thị (Z-order sortingOrder):** Nội dung Scene `0–99` · Ngăn xếp Popups `100–199` (mỗi cấp +2) · Thông báo/Toasts `200–299` · LoadingPanel `1000`.

### 1.3. Khung chú giải vật phẩm (Item Tooltip)
- **Kích thước & Bố cục:** Rộng cố định `360px`, cao tự co giãn. Nền giấy dó ép viền màu theo độ hiếm món đồ.
- **Nội dung hiển thị:**
  1. *Đầu khung:* Tên vật phẩm (chữ 24px, màu theo độ hiếm) + Kích thước ô (vd: `1×3`) + Phân loại (`Binh Khí`, `Hộ Cụ`, `Công Pháp`, `Đan Dược`, `Kỳ Trân`).
  2. *Cấp Trọng:* Nhất Trọng → Cửu Trọng (kèm chỉ số tăng theo cấp).
  3. *Chỉ số chiến đấu:* Lực Đạo, Tốc Xuất Chiêu, Hội Tâm, Cương Khí, Phổ Công, Tuyệt Kỹ (kèm mức tiêu hao Nội Lực).
  4. *Đột Phá (Mốc Trọng 3 / 6 / 9):* Liệt kê hiệu ứng mở khóa. Sáng rõ nếu đã đạt, xám mờ nếu chưa mở.
  5. *Tâm Pháp tương thích:* Tên Tâm Pháp môn phái buff trực tiếp cho hệ binh khí này (sáng vàng nếu trùng phái người chơi).

---

## 2. Màn Hình Nạp Dữ Liệu (LoadingPanel)

- **Mục đích & Chuyển cảnh:** Không phải Scene riêng. Nằm tại Canvas `sortingOrder = 1000` (DontDestroyOnLoad). Fade-in `0.25s`, hiển thị tiến độ tải dữ liệu khi chuyển scene, chặn toàn bộ raycast thao tác, fade-out `0.25s` khi xong.
- **Wireframe ASCII (1920×1080) & Pixel Rects:**
```text
+-----------------------------------------------------------------------------------+
|                                                                                   |
|                                [ĐẠI TỰ THƯ PHÁP]                                  |
|                             Nhất Nang Nhập Giang Hồ                               |
|                                                                                   |
|                   [============= THANH TIẾN ĐỘ =============]                     |
|                                       68%                                         |
|                                                                                   |
|                  * Mẹo: Hóa Kình tiêu hao 60 Nội Lực để hóa giải...               |
+-----------------------------------------------------------------------------------+
```
- **Neo & 16:10:** Nền che toàn màn hình `[0, 0, 1920, 1080]` (Stretch Full). Thanh tiến độ `[560, 560, 800, 24]` neo Center. Khi tỷ lệ 16:10 (1920×1200), nền giấy dó tự mở rộng viền trên/dưới, cụm nạp giữ nguyên trung tâm.
- **Phần tử UI & Nguồn dữ liệu:** Nền giấy dó tối giản; Slider `progress` (0..1 từ `SceneFlow`); Text `percentage` (tỷ lệ %); Text `tip` (lấy ngẫu nhiên từ danh sách mẹo giang hồ).
- **Tương tác & Phím tắt:** Chặn mọi tương tác chuột và bàn phím.
- **Phản hồi & Trạng thái:** Tiếng bút lướt nhẹ trên giấy khi bắt đầu hiện; thanh nạp chuyển động mượt mà.
- **Nhu cầu Art (ChatGPT prompt):** Nền giấy dó thủy mặc vệt khói mực tối giản (`1920×1080`) · Thanh nạp tạo hình vệt mực loang cuộn dần (`800×24`).

---

## 3. Màn Hình Khởi Động & Đăng Nhập (LoginScene)

- **Mục đích & Chuyển cảnh:** Scene index 0. Tiếp nhận danh xưng Hiệp Khách, chọn môn phái gia nhập, lưu thông tin phiên (`GameSession`), kết nối máy chủ dịch vụ. Thoát cảnh bằng `SceneFlow.Go(GameScene.Home)`.
- **Wireframe ASCII (1920×1080) & Pixel Rects:**
```text
+-----------------------------------------------------------------------------------+
|                                                                                   |
|                               [LOGO THƯ PHÁP GAME]                                |
|                                 NHẤT NANG GIANG HỒ                                |
|                                                                                   |
|                         +------------------------------+                          |
|                         | Tên Hiệp Khách: [..........] |                          |
|                         | Môn Phái:       [v Thái Thanh] |                        |
|                         |                              |                          |
|                         |     [ VÀO GIANG HỒ ]         |                          |
|                         +------------------------------+                          |
|                                                                                   |
| Phiên bản 0.1.0-alpha                                                Thoát [Esc]  |
+-----------------------------------------------------------------------------------+
```
- **Neo & 16:10:** Khung đăng nhập `[650, 340, 620, 480]` neo Center. Logo `[660, 180, 600, 140]` neo Top-Center. Nút Thoát neo Bottom-Right. Tỷ lệ 16:10 mở rộng không gian giấy dó nền.
- **Phần tử UI & Nguồn dữ liệu:**
  1. *Logo Game:* Hình đại tự "Nhất Nang Giang Hồ".
  2. *Ô nhập danh xưng:* `InputField` (Placeholder: "Nhập tôn tính đại danh..."), nguồn `GameSession.HeroName`.
  3. *Chọn môn phái:* `Dropdown` gồm 7 phái từ `sects.json` (Kim Cang, Thái Thanh, Liệt Diễm, Huyền Độc, Hàn Sơn, Thiết Phủ, Bích Thủy).
  4. *Nút vào game:* `Button` "Vào Giang Hồ" (Chính son đỏ). Disable khi tên rỗng hoặc chứa ký tự cấm.
- **Tương tác & Phím tắt:** Phím `Enter` thực hiện vào game nếu tên hợp lệ; `Esc` mở bảng xác nhận thoát trò chơi.
- **Phản hồi & Trạng thái:** Tiếng đóng dấu triện son cộp đanh thép khi bấm vào game; viền đỏ nhấp nháy nếu chưa nhập tên.
- **Nhu cầu Art:** Tranh nền thủy mặc đỉnh núi mù sương tĩnh lặng (`1920×1080`) · Logo thư pháp mực đậm (`600×200`) · Hộp khung thư họa (`620×480`) · Nút son đỏ sơn mài (`360×64`).

---

## 4. Sảnh Chính (HomeScene) & Các Phân Hệ Quản Lý

- **Mục đích & Chuyển cảnh:** Trung tâm điều hướng mọi tính năng. Vào từ LoginScene hoặc khi kết thúc trận đấu. Chuyển đổi giữa các phân hệ dạng Panel/Popup trong cùng scene để tối ưu hiệu năng.

### 4.1. Bố cục tổng thể HomeScene
- **Wireframe ASCII (1920×1080) & Pixel Rects:**
```text
+-----------------------------------------------------------------------------------+
| [Avatar] Vân Du • Liệt Diễm Đường [Tâm Hỏa Cấp 3]      [Thể Lực: 120/120] [V] |
|-----------------------------------------------------------------------------------|
|                      |                                                            |
|                      |   [ HÀNH TẨU GIANG HỒ (PvE) ]                              |
|   [ TRANH HIỆP KHÁCH |                                                            |
|      TOÀN THÂN       |   [ LUẬN KIẾM ĐÀI (PvP) ]                                  |
|      THEO PHÁI ]     |                                                            |
|                      |   [ BAO PHỤC (SẮP XẾP TÚI) ]                               |
|                      |                                                            |
|                      |   [ Tàng Bảo Các ]   [ Vạn Bảo Phổ ]   [ Thương Hội ]      |
|                      |                                                            |
|                      |   [ Cài Đặt ]                          [ Đăng Xuất ]       |
+-----------------------------------------------------------------------------------+
```
- **Neo & 16:10:**
  - Header `[0, 0, 1920, 80]` neo Top-Stretch.
  - Cột nhân vật `[60, 120, 560, 900]` neo Left-Center.
  - Cột thực đơn trung tâm `[700, 160, 1100, 840]` neo Center/Right-Center.
  - Tỷ lệ 16:10: Giữ nguyên độ rộng cột, dãn khoảng trống phía trên/dưới nhẹ nhàng.
- **Phần tử UI:** Cụm Header (Tên, Phái, Tâm Pháp, Thể Lực, Bạc, Mạng Online/Offline); Hệ thống nút điều hướng lớn dẫn sang PvE, PvP, Bao Phục; Hàng nút phụ bên dưới.

### 4.2. Phân hệ Bao Phục (Bag Loadout Panel)
- **Mục đích:** Sắp xếp lưới túi đồ, bố trí vũ khí kích hoạt hiệu ứng liền kề, chuyển đồ từ khay dự phòng.
- **Wireframe & Pixel Rects:**
  - Lưới Bao Phục `[80, 200, 720, 720]` (Ô chuẩn 80×80px, khoảng cách 4px).
  - Khay đồ dự trữ (Item Tray) `[900, 200, 940, 720]` (Thanh cuộn dọc, chứa vật phẩm chưa dùng).
  - Thanh chọn phái & cấp Tâm Pháp thử nghiệm `[80, 100, 720, 70]`.
- **Tương tác:** Nhấp giữ chuột trái kéo vật phẩm từ khay thả vào lưới (kèm khung xanh hợp lệ / đỏ va chạm); Nhấp chuột phải cất đồ về khay; Hover hiện Tooltip chi tiết vật phẩm.
- **Phản hồi:** Tiếng lách cách vũ khí kim loại khi nhấc đồ; tiếng vải bố ma sát khi đặt vào túi; rung nhẹ khi đặt sai vị trí.

### 4.3. Phân hệ Hành Tẩu Giang Hồ (PvE Chapter & Stage Select Panel)
- **Mục đích:** Bản đồ chọn Hồi (chương) và Ải chiến đấu PvE.
- **Bố cục:**
  - Tiêu đề Hồi (vd: "Hồi 1: Rừng Trúc Loạn Đao") ở đỉnh.
  - Tuyến bản đồ nét vẽ mực thủy mặc nối từ Ải 1 đến Ải 10 (Ải 10 vẽ ấn đầu trâu/mặt nạ Boss).
  - Dưới mỗi ải hiển thị đánh giá 1–3 đóa Mai Đỏ (sao).
  - Khung phụ bên phải `[1360, 180, 480, 780]`: Thông tin Ải chọn, tiêu hao Thể Lực (vd: 5 Thể Lực), phần thưởng tiềm năng (Bạc, Trọng liệu), nút "Khiêu Chiến" (Son đỏ).
- **Phản hồi:** Tiếng trống trận thúc giục khi bấm nút Khiêu Chiến; cảnh báo popup "Không đủ Thể Lực" nếu thiếu.

### 4.4. Phân hệ Luận Kiếm Đài (PvP Arena Lobby Panel)
- **Mục đích:** Đấu võ bất đồng bộ với bản lưu túi của người chơi khác.
- **Bố cục:**
  - Thẻ thông tin Hiệp Khách: Bậc xếp hạng (Sơ Nhập → Cửu Thiên), Điểm Luận Kiếm, Chiến Uy hiện tại.
  - Kỳ Luận Kiếm: Thời gian đếm ngược mùa giải (vd: "Còn 12 ngày").
  - Nút chính: "Tìm Kiếm Đối Thủ" (Ghép đối thủ lệch Chiến Uy trong biên độ ±15%).
  - Bảng vinh danh: Top 3 Hiệp Khách dẫn đầu kỳ trước kèm danh hiệu "Thiên Hạ Đệ Nhất".
- **Phản hồi:** Tiếng kiếm rút rít gió khi ấn tìm đối thủ; hiệu ứng lật thẻ bài thủy mặc hiện thông tin đối phương.

### 4.5. Hộp Thoại Cài Đặt (SettingsPopup)
- **Kích thước & Vị trí:** `[660, 290, 600, 500]` neo Center (Z-order 100).
- **Phần tử:** Slider âm lượng Nhạc nền (BGM); Slider âm lượng Võ thuật (SFX); Toggle Toàn màn hình/Cửa sổ; Nút "Đăng xuất" (về LoginScene). Phím `Esc` đóng bảng.

### 4.6. Nhu cầu Art sảnh Home
- Tranh toàn thân 3 Hiệp Khách thủy mặc (`1024×1536`) · 7 Huy hiệu triện đỏ môn phái (`1024×1024`) · Bản đồ giang hồ cuộn giấy cổ PvE (`1400×700`) · Bộ khung viền thẻ bài ải & huy chương PvP (`512×512`).

---

## 5. Màn Hình Trận Đấu (GameplayScene)

- **Mục đích:** Nơi diễn ra chu trình thi đấu tự động 3 giai đoạn: Chuẩn bị đội hình → Giao đấu tự động → Bảng kết quả.

### 5.1. Giai đoạn 1: Chuẩn Bị (Preparation Phase)
- **Bố cục:**
  - Lưới Bao Phục nằm trung tâm `[560, 140, 800, 800]` cho phép tinh chỉnh nhanh vật phẩm.
  - Phía trên là tên đối thủ và cảnh giới dự kiến.
  - Phía dưới là Nút "Xuất Chiêu" (Son đỏ cực đại `[760, 960, 400, 80]`).
- **Ràng buộc & Báo lỗi (Error States):**
  - Bao Phục trống → Báo Popup: *"Bao phục đang trống."*
  - Bao Phục không có Binh Khí → Báo Popup: *"Bao phục phải có ít nhất một vũ khí."* (Không được vào trận).
  - Đang gửi dữ liệu lên server → Khóa nút "Xuất Chiêu" tránh spam lệnh.

### 5.2. Giai đoạn 2: Giao Đấu (Combat Replay Phase)
- **Wireframe ASCII (1920×1080) & Pixel Rects:**
```text
+-----------------------------------------------------------------------------------+
| [ Phe Ta: Vân Du ]                                    [ Đối Thủ: Hắc Y Nhân ] |
| Khí Huyết: [====    ] 850/1000                    Khí Huyết: [======= ] 920/1000  |
| Cương Khí: [==      ] 150/400                     Cương Khí: [        ] 0/300     |
| Nội Lực:   [======  ] 60/100                      Nội Lực:   [====    ] 40/100    |
| Buff/Debuff: [Lửa][Độc]                           Buff/Debuff: [Trì Trệ]          |
|-----------------------------------------------------------------------------------|
|                                                                                   |
|      +---------------------+                +---------------------+               |
|      |    BAO PHỤC TA      |                |   BAO PHỤC ĐỐI THỦ  |               |
|      | (Vũ khí sáng theo   |   VS (VFX)     | (Vũ khí sáng theo   |               |
|      |  thời gian hồi)     |                |  thời gian hồi)     |               |
|      +---------------------+                +---------------------+               |
|                                                                                   |
|-----------------------------------------------------------------------------------|
| [> Tạm dừng] [x1] [x2] [x4] [Chơi lại]  [======== Thanh Thời Gian ========] 12.4s |
| [Nhật Ký Chiến Đấu: Vân Du kích hoạt Hóa Kình tiêu 60 Nội Lực!             ^] |
+-----------------------------------------------------------------------------------+
```
- **Neo & Pixel Rects:**
  - Sàn đấu trên (Arena) `[28, 20, 1864, 730]` (neo Top-Stretch).
  - Hai bảng chỉ số đấu thủ `[28, 20, 340, 700]` và `[1552, 20, 340, 700]`.
  - Hai lưới túi đồ đối diện ở giữa, mỗi bên rộng `540×540px`.
  - Bảng điều khiển đáy `[28, 760, 1864, 290]` (neo Bottom-Stretch).
- **Phần tử & Cơ chế Replay:**
  - Thanh máu/khiên/nội lực cập nhật thời gian thực theo tick dữ liệu.
  - Ô trang bị có thanh tiến độ nạp Tốc Xuất Chiêu phủ sáng viền; khi ra chiêu giật nhẹ phát xung mực.
  - Bảng nút tua: Tạm dừng / Phát, Chơi lại, Đổi tốc độ `1x`, `2x`, `4x`, Thanh trượt Timeline scrub.
  - Bảng Nhật Ký Giao Đấu (cuộn tự động): Liệt kê từng chiêu thức Phổ Công, Tuyệt Kỹ, Hóa Kình, Bộc Phát.
- **Tương tác & Phím tắt:**
  - `Phím Space`: Tạm dừng / Tiếp tục phát.
  - `Phím 1`, `2`, `4`: Đổi tốc độ trận đấu.
  - Kéo thanh trượt scrub: Tua trận đến thời điểm tùy ý.
- **Phản hồi âm thanh & Hình ảnh:**
  - Tiếng đao kiếm xé gió chém đanh gọn; tiếng "Boong" rền vang khi kích hoạt Hóa Kình cắt Tuyệt Kỹ.
  - Tiếng nổ trầm khi Liệt Hỏa đạt 50 điểm Bộc Phát.
  - Số sát thương nhảy nổi trên đầu túi (Vàng: Hội Tâm, Đỏ: Liệt Hỏa, Xanh lục: Kịch Độc).

### 5.3. Giai đoạn 3: Bảng Kết Quả (Result Phase / ResultPopup)
- **Mục đích:** Thông báo Thắng / Bại / Hòa, hiển thị phần thưởng và điều hướng.
- **Bố cục:**
  - Khung đại triện mực giữa màn hình `[560, 240, 800, 600]`.
  - Tiêu đề đại tự Thủy Mặc: "ĐẠI THẮNG" (Son đỏ viền vàng) hoặc "KỲ PHÙNG ĐỊCH THỦ" / "THẤT BẠI" (Mực lạnh).
  - PvE: Số sao đạt được (1–3 Mai Đỏ) + Bạc + Nguyên liệu rơi.
  - PvP: Điểm Luận Kiếm tăng/giảm (+25 ELO).
  - 2 nút bấm: "Khiêu Chiến Tiếp" (Chính son đỏ) và "Về Sảnh" (Phụ viền mực).
- **Phản hồi:** Nhạc đàn tranh hào hùng khi chiến thắng; tiếng chiêng buồn lắng đọng khi bại trận.

### 5.4. Nhu cầu Art Gameplay
- Tranh nền Đấu Trường Thủy Mặc (Hẻm núi mây mù cuộn trào) (`1920×1080`) · 12 Icon trạng thái (Liệt Hỏa, Kịch Độc, Xuất Huyết, Định Thân, Điểm Huyệt...) (`1024×1024` thu về `32×32`) · Đại tự thư pháp Thắng/Bại (`800×300`).

---

## 6. Danh Mục Tài Nguyên Hình Ảnh Cần Sản Xuất (Art Needs)

| Nhóm Tài Nguyên | Danh Mục Hình Ảnh | Kích Thước Canvas (ChatGPT) | Quy Cách Cắt & Render Trong Unity |
|---|---|---|---|
| **Nền Màn Hình** | Nền giấy dó chung, Nền Login, Nền Sảnh Home, Nền Đấu Trường Gameplay, Nền Bản đồ PvE | 1536×1024 landscape | Cắt phủ 1920×1080 (giữ vùng giữa sáng nhạt đặt UI) |
| **Hiệp Khách** | Toàn thân 3 phái khởi đầu: Thiền tăng (Kim Cang), Kiếm hiệp (Thái Thanh), Đao khách (Liệt Diễm) | 1024×1536 portrait | Tách nền trong suốt, hiển thị tại HomeScene |
| **Huy Hiệu Phái** | 7 Biểu tượng dấu triện vuông khắc gỗ cho 7 môn phái | 1024×1024 square | Scale về 96×96 và 48×48 trong UI |
| **Biểu Tượng Trạng Thái** | 12 Icon: Liệt Hỏa, Kịch Độc, Xuất Huyết, Cương Khí, Định Thân, Trì Trệ, Điểm Huyệt, Khinh Công, Vận Khí... | 1024×1024 square | Nét vẽ cọ son đỏ/mực đen, scale về 32×32 |
| **Bộ Khung & Nút UI** | Khung hộp đăng nhập, khung Tooltip, Khung Popup, Nút Son đỏ, Nút Mực xám, Dấu triện sao Mai Đỏ | 1024×1024 / 512×512 | Sliced 9-slice Sprite cho Unity UGUI |

---

## 7. Phân Kỳ Triển Khai: Bản Thử Nghiệm (MVP) và Bản Đầy Đủ

| Màn hình / Tính năng | Phiên Bản MVP (Chơi Ngay Được) | Phiên Bản Hoàn Thiện (Giai đoạn sau) |
|---|---|---|
| **LoadingPanel** | Nền giấy đơn sắc, thanh tiến độ tuyến tính, mẹo text tĩnh | Diễn hoạt vệt mực chảy loang lỏng, chuyển cảnh rèm khói mờ dần |
| **LoginScene** | Nhập tên cục bộ + chọn phái trong danh sách, lưu `GameSession` | Tích hợp xác thực tài khoản qua API máy chủ, lưu đám mây |
| **HomeScene** | Menu nút thẳng hàng: Vào trận PvE/PvP nhanh, Mở xếp túi | Bản đồ Hành Tẩu Giang Hồ chọn ải phân nhánh, giao diện Luận Kiếm xếp hạng chi tiết |
| **Bao Phục (Túi)** | Lưới ô kéo thả chuột cơ bản, hiển thị cấp Trọng và Tooltip đơn giản | Hiệu ứng hào quang liên kết ô liền kề, xoay vật phẩm 90°, so sánh đồ |
| **GameplayScene** | Xem diễn biến tự động, tạm dừng, đổi tốc độ 1x/2x/4x, thanh scrub cơ bản | Hiệu ứng đường kiếm mực tung hoành (VFX Lab), camera rung lắc chấn động khi Tuyệt Kỹ |
| **Tính Năng Phụ** | Báo popup "Sắp ra mắt" cho Tàng Bảo Các, Vạn Bảo Phổ, Thương Hội | Mở hoàn chỉnh hệ thống rèn đúc, nâng Trọng vật phẩm, chợ mua bán |

---

## 8. Câu Hỏi Mở Dành Cho Product Owner (PO)

1. **Cơ chế xoay vật phẩm:** Ở bản MVP có cho phép xoay ngang món đồ `1×3` hay `1×4` bằng phím tắt `R` không, hay giữ nguyên tắc cố định dọc như thiết kế ban đầu trong `art-pipeline.md`?
2. **Quy tắc hiển thị khiếu nại Bao Phục:** Khi túi đồ thiếu Binh Khí, hệ thống nên khóa xám mờ nút "Xuất Chiêu" kèm dòng chú thích cảnh báo hay để người chơi bấm vào rồi mới nảy Popup cảnh báo?
3. **Màn hình chọn Ải PvE:** Tiến trình qua ải là tuyến tính 100% (ải sau nối tiếp ải trước) hay có các nhánh rẽ bí cảnh/thử thách phụ để chọn đường đi?
4. **Hiển thị nhật ký chiến đấu:** Bảng nhật ký ở đáy màn Gameplay có cần cho phép lọc xem riêng từng loại sự kiện (chỉ xem Tuyệt Kỹ, chỉ xem Hóa Kình, hoặc chỉ xem sát thương Bộc Phát) không?
5. **Giới hạn số lượng bản sao:** Người chơi có thể trang bị nhiều bản sao của cùng một Binh Khí trong Bao Phục không, và nếu có thì việc kích hoạt Đột Phá mốc Trọng tính độc lập từng món hay gộp chung?
