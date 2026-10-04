# Đặc Tả Bố Cục Sảnh Chính (Home Layout Spec) — Nhất Nang Giang Hồ

**Hệ máy:** PC (chuẩn 1920×1080, co giãn 16:10). **Phong cách:** Thủy Mặc (Quyết định #11).  
**Căn cứ:** Quyết định #20 (dựng sẵn trong scene), #21 (môn phái mở cấp 10, túi xếp ngoài trận, lối vào rải quanh tranh nền, cảnh báo trên nút xuất trận), `docs/design/glossary.md`.

---

## 1. Ý Niệm Thiết Kế & Ẩn Dụ Không Gian (Concept & Metaphors)

Sảnh chính là một bức tranh toàn cảnh thủy mặc vẽ **Tiêu Cục Sơn Trấn** trên đỉnh đèo mây phủ. Mỗi tính năng là một **điểm bấm (Hotspot)** gắn vào cảnh vật kèm **biển hiệu thư pháp** (`Plaque`):
- **Hành Tẩu Giang Hồ (`pve`):** *Con đường mòn lát đá men sườn núi* dẫn ra cổng trấn. Ẩn dụ bước chân ngao du, vượt ải giang hồ.
- **Luận Kiếm Đài (`arena`):** *Lôi đài đá cổ kính trên đỉnh mỏm núi cao* với cờ phướn bay trong gió. Ẩn dụ nơi anh hào tỷ thí võ nghệ.
- **Bao Phục (`bag`):** *Chiếc túi vải du hiệp đặt trên bàn đá* cạnh nơi Hiệp Khách dừng chân. Ẩn dụ sắp xếp hành trang binh khí bên mình.
- **Tàng Bảo Các (`storage`):** *Gian tháp gỗ kiên cố có khóa đồng* bên đình viện. Ẩn dụ kho cất giữ bảo vật chưa dùng.
- **Vạn Bảo Phổ (`codex`):** *Cuộn thư tịch cổ trải trên án thư* trong lương đình. Ẩn dụ tra cứu điển tích vũ khí giang hồ.
- **Thương Hội (`shop`):** *Quầy hàng lưu động mái che cờ hiệu "Thương"*. Ẩn dụ trao đổi mua bán bằng bạc.
- **Môn Phái (`sect`):** *Cổng tam quan sơn môn mây mù che phủ* (khóa xích + biển "Cấp 10"). Ẩn dụ nơi bái sư lĩnh hội Tâm Pháp.
- **Cài đặt (`settings`):** *Chuông gió đồng treo hiên quán*. Ẩn dụ tinh chỉnh phong thổ, âm thanh.

---

## 2. Bố Cục Trên Lưới 1920×1080 (Wireframe & Rects)

### 2.1. ASCII Wireframe
```text
+-----------------------------------------------------------------------------------+
| [Vân Du • Cấp 1] [====== EXP ======]         [Thể Lực: 120/120] [Bạc: 8800] [Cài đặt] |
|-----------------------------------------------------------------------------------|
|  [MÔN PHÁI]                                                      [LUẬN KIẾM ĐÀI]  |
|  (Cổng sơn môn - Cấp 10)                                         (Lôi đài trên núi)|
|                                                                                   |
|  [THƯƠNG HỘI]                                                    [HÀNH TẨU GIANG HỒ|
|  (Quầy hàng sầm uất)               [ HÌNH BÓNG HIỆP KHÁCH ]      (Đường mòn xuất quan)
|                                           "VÂN DU"                                |
|  [VẠN BẢO PHỔ]                    (Toàn thân đứng/tọa thiền)      [TÀNG BẢO CÁC]  |
|  (Án thư thiền trà)                                              (Kho báu kiên cố)|
|                         [BAO PHỤC]                                                |
|                       (Túi vải bàn đá)                                            |
|-----------------------------------------------------------------------------------|
|                       * Cảnh báo: Bao Phục chưa có binh khí *                     |
|                 [ HÀNH TẨU GIANG HỒ ]        [ LUẬN KIẾM ĐÀI ]                    |
+-----------------------------------------------------------------------------------+
```

### 2.2. Bảng tọa độ chi tiết & Hành vi co giãn
- Tranh nền `1536×1024` khi phóng lên `1920×1080` bị xén ~8% trên/dưới; vùng an toàn đặt Hotspot nằm cách mép tối thiểu 64 px.
- Màn hình 16:10 (`1920×1200`): CanvasScaler (match 0.5) giữ nguyên tỉ lệ; Header bám sát mép trên, Battle Area bám sát đáy, nền tranh lộ thêm phần trên/dưới mà không bị kéo giãn.

| Thành phần UI | Tọa độ [x, y, w, h] (px) | Neo (Anchor) | Hành vi 16:10 & Xén tranh |
|---|---|---|---|
| **Header Bar** | `[0, 0, 1920, 80]` | Top-Stretch | Neo đỉnh, trải rộng ngang |
| **Vân Du (Hiệp Khách)** | `[810, 320, 300, 600]` | Bottom-Center | Giữ thông thoáng trung tâm, không bị che |
| **Hotspot_Sect (Môn Phái)** | `[80, 140, 240, 180]` | Top-Left | Neo góc trên trái sườn núi |
| **Hotspot_Shop (Thương Hội)** | `[80, 420, 220, 180]` | Left-Center | Neo bờ trái, tránh che chân nhân vật |
| **Hotspot_Codex (Vạn Bảo Phổ)** | `[80, 680, 220, 160]` | Bottom-Left | Neo góc dưới trái |
| **Hotspot_Pvp (Luận Kiếm Đài)** | `[1600, 140, 240, 180]` | Top-Right | Neo vách núi cao bên phải |
| **Hotspot_Pve (Hành Tẩu)** | `[1600, 420, 240, 180]` | Right-Center | Neo lối xuất quan bên phải |
| **Hotspot_Storage (Tàng Bảo Các)**| `[1620, 680, 220, 160]` | Bottom-Right | Neo góc dưới phải cạnh kho báu |
| **Hotspot_Bag (Bao Phục)** | `[540, 680, 220, 150]` | Bottom-Center | Bàn đá đặt túi cạnh nhân vật |
| **BattleArea (Khu Xuất Trận)** | `[560, 930, 800, 130]` | Bottom-Center | Neo chính giữa đáy màn hình |
| ├ *WarningText (Cảnh báo)* | `[0, 0, 800, 30]` | Top-Stretch nội bộ | Chữ son đỏ nhấp nháy ngay trên nút |
| ├ *Btn_Pve (Hành Tẩu)* | `[60, 40, 320, 70]` | Left-Bottom nội bộ | Nút chính Son đỏ (`#B8322A`) |
| └ *Btn_Pvp (Luận Kiếm)* | `[420, 40, 320, 70]` | Right-Bottom nội bộ | Nút chính Vàng đất (`#C9A45C`) |

---

## 3. Trạng Thái & Tương Tác Hotspot

Mỗi Hotspot là một thực thể prefab tương tác bao gồm: Vùng cảm ứng (Image trong suốt), Biển hiệu (`Plaque`), Biểu tượng Khóa (`LockIcon`), và Triện son báo mới (`Badge`).
- **Bình thường (Normal):** Biển gỗ viền mực sẫm, chữ Mực đậm `#1E1B18`, không hào quang.
- **Rà chuột (Hover):** Biển phóng to 1.06x, viền mực loang vàng kim `#C9A45C`, hiện Tooltip 1 dòng tóm tắt mục đích bên dưới biển hiệu.
- **Nhấp chuột (Pressed):** Biển thụt xuống 2 px, phát âm gõ mõ giang hồ giòn nhẹ.
- **Khóa (Locked - Môn Phái):** Biển ngả xám mực `#6B6760` (độ mờ 40%), hiện ổ khóa đồng + nhãn chữ thư pháp `"Cấp 10"`. Khi hover hiện Tooltip: *"Đạt cấp 10 mở khóa gia nhập Môn Phái"*. Bấm vào rung lắc nhẹ và phát tiếng xích sắt va chạm.
- **Báo mới (Badge):** Dấu triện son đỏ tròn `"Mới"` gắn góc trên bên phải của biển.
- **Phím tắt:** `Esc` mở hộp thoại Cài Đặt (`SettingsPopup`). Phím `1` (Hành Tẩu), `2` (Luận Kiếm), `3` (Bao Phục).

---

## 4. Cây Phân Cấp Đối Tượng Unity (Scene Hierarchy)

Tuân thủ Quyết định #20 (dựng sẵn toàn bộ trong Editor, không sinh code lúc chạy):
```text
HomeScene
├── Main Camera
├── EventSystem
├── HomeController (Tham chiếu các component qua [SerializeField])
└── Canvas (Screen Space Overlay, 1920×1080, match 0.5, sort 0)
    ├── Background (Image: bg_home.png, Stretch full)
    ├── HeroZone (Image: hình ảnh nhân vật Vân Du, neo Bottom-Center)
    ├── Header (Image panel mờ)
    │   ├── HeroName (Text: "Vân Du")
    │   ├── ExpBar (Slider cấp + Text: "Cấp 1")
    │   ├── StaminaText (Text: "Thể Lực 120/120")
    │   ├── SilverText (Text: "Bạc 8,800")
    │   └── Btn_Settings (Button + Image icon chuông gió)
    ├── Hotspots
    │   ├── Hotspot_Pve (Prefab Hotspot: Area, Plaque, Badge)
    │   ├── Hotspot_Pvp
    │   ├── Hotspot_Bag
    │   ├── Hotspot_Storage
    │   ├── Hotspot_Codex
    │   ├── Hotspot_Shop
    │   └── Hotspot_Sect (Prefab Hotspot + LockContainer: LockIcon, LockText)
    └── BattleArea (RectTransform neo Bottom-Center)
        ├── WarningText (Text: "Bao Phục chưa có binh khí" / Son đỏ #B8322A)
        ├── Btn_QuickPve (Button: "Hành Tẩu Giang Hồ")
        └── Btn_QuickPvp (Button: "Luận Kiếm Đài")
```

---

## 5. Yêu Cầu Tài Nguyên & Prompt ChatGPT (Art Brief)

Tuân thủ khóa phong cách Thủy Mặc trong `docs/art/art-pipeline.md`: *2D Chinese ink-brush (shuimo) on aged rice paper, rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C. Strictly 2D, no 3D, no text*.

### 5.1. Tranh nền sảnh chính (`bg_home.png` — Canvas 1536×1024 landscape)
```text
Match the attached reference style exactly. Style: 2D Chinese ink-brush (shuimo) illustration on aged rice paper.
A panoramic landscape of a mountain-pass martial arts waystation (sơn trấn dịch trạm).
Center foreground: a broad, clean stone courtyard with empty rice paper space reserved for a standing character.
Left side: a distant mountain archway gate in misty clouds (top-left), a lively merchant tea-stall with hanging shop flag (mid-left), and an open wooden pavilion with scroll manuscripts on a stone desk (bottom-left).
Right side: a raised stone martial duel platform on a jagged cliff edge with banners (top-right), a winding paved stone path leading down the pass (mid-right), and a sturdy wooden storehouse building (bottom-right).
Mid-center: a low stone tea-bench holding a lone traveller's cloth pack.
Keep the very top 8% and bottom 10% calm and low-contrast for UI bars. High negative space in center. No people, no characters, no readable text.
```

### 5.2. Biển hiệu thư pháp (`plaque_label.png` — Canvas 512×160, 9-slice)
```text
Match the attached reference style exactly. Style: 2D Chinese ink-brush (shuimo) illustration.
A blank horizontal wooden hanging signboard plaque for calligraphy text. Weathered dark ink-stained wood frame with subtle ochre-gold corner brackets, smooth pale aged rice-paper inner surface, visible brush-stroke outlines, completely empty inside, transparent background, flat 2D, no text.
```

### 5.3. Ổ khóa đồng cổ (`icon_lock.png` — Canvas 256×256)
```text
Match the attached reference style exactly. Style: 2D Chinese ink-brush (shuimo) illustration.
An ancient Chinese horizontal brass padlock (khóa đồng cổ), flat ochre-gold and deep ink wash tones, bold dry-brush outlines, rustic engraved texture, fully transparent background, no text.
```

### 5.4. Dấu triện son báo mới (`badge_new.png` — Canvas 256×256)
```text
Match the attached reference style exactly. Style: 2D Chinese ink-brush (shuimo) illustration.
A small circular vermilion red seal stamp (chop), rough carved ink stone texture, solid vermilion #B8322A, fully transparent background, no letters, no text.
```

---

## 6. Câu Hỏi Mở Dành Cho Product Owner (PO)

1. **Hành vi hai nút xuất trận ở đáy:** Nút "Hành Tẩu Giang Hồ" và "Luận Kiếm Đài" ở đáy màn hình sẽ trực tiếp đưa người chơi vào trận đấu gần nhất hay mở bảng chọn ải / sảnh ghép đội?
2. **Nguồn nhận điểm kinh nghiệm Hiệp Khách:** Để đạt Cấp 10 mở Môn Phái, Hiệp Khách tích lũy EXP qua từng ải Hành Tẩu hoàn thành hay thông qua cắn Đan Dược tu vi?
3. **Cơ chế khi đã gia nhập Môn Phái (>= Cấp 10):** Sau khi đạt Cấp 10 và chọn phái, Hotspot "Môn Phái" sẽ chuyển thành giao diện tăng cấp Tâm Pháp hay cho phép phản sư đổi phái?
