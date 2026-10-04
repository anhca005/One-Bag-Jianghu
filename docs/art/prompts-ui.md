# Prompt hình giao diện (ChatGPT)

Nền màn hình và bộ khung UI cho luồng Login → Loading → Home → Gameplay ([ui-flow.md](../tech/ui-flow.md)). Độ phân giải chuẩn 1920×1080.

## Quy ước
- Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/`, **đúng tên cột File**.
- Nền màn hình: ChatGPT chỉ ra được tối đa 1536×1024 (3:2). Game sẽ cắt giữa và phóng lên 16:9, nên **đặt nội dung quan trọng ở giữa**, hai mép trên và dưới khoảng 8% có thể bị cắt.
- Khung, nút: nền trong suốt; nếu ra ô caro thì chạy `tools/art/remove_checker.py`.
- Đính kèm `docs/art/reference/01-style-board-thuy-mac.png` khi gửi.

## Tin mở đầu

```
We will create UI art for a 2D wuxia game, one image per message, in exactly the style of the attached reference.
Style: 2D Chinese ink-brush (shuimo) on aged rice paper, confident black brush outlines, flat ink washes with dry-brush texture, vermilion and ochre-gold accents. Palette only: rice paper #EFE6D2, deep ink #1E1B18, light ink #6B6760, misty blue-grey #8A9A94, vermilion #B8322A, ochre gold #C9A45C.
Strictly 2D: no 3D, no glow, no lens effects, no depth of field. No text, no letters, no numbers, no watermark, no logo unless I ask.
Reply "ready" and wait.
```

## Danh sách

| # | File | Kích thước | Prompt |
|---|---|---|---|
| 1 | `bg_login.png` | 1536x1024 | `Login screen background: a lone wanderer's silhouette on a cliff edge seen small from behind, facing a vast sea of misty ink mountains, huge vermilion sun, flying cranes. Keep the center-left area calm and empty for a login panel. Landscape 1536x1024.` |
| 2 | `bg_loading.png` | 1536x1024 | `Loading screen background: an empty rice-paper scroll with ink-wash mountains only along the bottom edge and a few bamboo leaves in the top right corner; the whole center is plain paper for text and a progress bar. Landscape 1536x1024.` |
| 3 | `bg_home.png` | 1536x1024 | `Main hall background (panoramic landscape of a mountain-pass martial arts waystation). Center foreground: a broad clean stone courtyard with empty space reserved for a standing character. Left side: a distant mountain archway gate in misty clouds (top-left), a merchant tea-stall with a hanging blank shop flag (mid-left), an open wooden pavilion with scrolls on a stone desk (bottom-left). Right side: a raised stone duel platform on a cliff edge with blank banners (top-right), a winding paved stone path leading down the pass (mid-right), a sturdy wooden storehouse with a brass-locked door (bottom-right). Lower center, slightly left: a low stone bench holding a traveller's cloth pack. Keep the top 8% and bottom 12% calm and low-contrast for UI bars. No people, no readable text. Landscape 1536x1024.` *(bản mới theo bố cục Gemini 2026-09-29: mỗi địa điểm là một điểm bấm)* |
| 4 | `bg_battle.png` | 1536x1024 | `Battle arena background: a flat stone duel platform seen from the front under a stormy ink sky, two stone lanterns at the far left and far right edges, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024.` |
| 5 | `frame_panel.png` | 1024x1024 | `A square decorative panel frame: thin double ink border with ochre-gold cloud-pattern corners, the inside completely empty and transparent, fully transparent background outside the border. Designed to be 9-sliced (corners detailed, straight edges plain).` |
| 6 | `button_primary.png` | 1024x384 | `A wide horizontal button shape: a vermilion lacquer plate with a thin ochre-gold border and small cloud ornaments at both ends, flat, no text, transparent background. Designed to be 9-sliced.` |
| 7 | `button_secondary.png` | 1024x384 | `Same wide button shape as before but deep ink black with a thin light-ink border, flat, no text, transparent background.` |
| 8 | `progress_bar.png` | 1536x256 | `A horizontal progress bar frame: a long empty bamboo tube split lengthwise, ochre-gold binding rings at both ends, the inside empty and transparent, transparent background.` |
| 9 | `seal_title.png` | 1024x1024 | `A large square vermilion seal stamp (chop) with carved-looking abstract ornamental pattern and no readable characters, slightly uneven ink edges, transparent background.` |
| 10 | `hero_portrait_frame.png` | 1024x1536 | `A tall portrait frame for a character card: ink-brush border with a vermilion ribbon tied at the top and ochre-gold corner ornaments, empty transparent inside, transparent background.` |

| 11 | `plaque_label.png` | 1536x512 | `A blank horizontal wooden hanging signboard for a calligraphy label: weathered dark ink-stained wood frame with small ochre-gold corner brackets, smooth pale rice-paper inner surface, completely empty inside, flat 2D, transparent background, no text.` (Unity: 9-slice) |
| 12 | `icon_lock.png` | 1024x1024 | `An ancient Chinese horizontal brass padlock, flat ochre-gold and deep ink wash tones, bold dry-brush outline, centered, transparent background, no text.` |
| 13 | `badge_new.png` | 1024x1024 | `A small round vermilion seal stamp with a rough carved edge and an abstract ornamental pattern inside, no readable characters, centered, transparent background.` |
| 14 | `hero_idle.png` | 1024x1536 | `Full-body wandering swordsman with no sect insignia, standing calmly in a three-quarter view facing the viewer, plain grey-blue travelling robe, bamboo hat hanging on the back, cloth bundle on the shoulder, a sheathed plain sword at the waist, face calm with simple ink features; portrait 1024x1536, the figure fills the height, transparent background, no text.` (đứng giữa sân ở Home) |
| 15 | `frame_tooltip.png` | 1024x1024 | `A simple rectangular tooltip frame: pale rice-paper inside with a thin deep-ink brush border and a tiny vermilion seal dot in the top-left corner, the inside empty, flat 2D, transparent background, no text.` (Unity: 9-slice, viền đổi màu theo độ hiếm) |

## Biểu tượng trạng thái (1024x1024, dùng ở 32×32)

Lưu vào `Textures2D/Status/`. Hình phải **rất đơn giản, một hình khối đậm**, vì chỉ hiện 32×32 px. Tin mở đầu như trên, thêm câu: `These are tiny status icons: one bold simple symbol in a round ink-brush circle, readable at 32x32 pixels, transparent background, no text.`

| # | File | Trạng thái | Prompt |
|---|---|---|---|
| 16 | `status_burn.png` | Liệt Hỏa | `Status icon: a single vermilion flame inside a round ink-brush circle.` |
| 17 | `status_poison.png` | Kịch Độc | `Status icon: a single dark green-grey drop with a small bubble inside a round ink-brush circle.` |
| 18 | `status_bleed.png` | Xuất Huyết | `Status icon: a single vermilion teardrop shape with a thin diagonal ink stroke through it, inside a round ink-brush circle.` |
| 19 | `status_internal_injury.png` | Nội Thương | `Status icon: a simple ink heart shape with a cracked line across it, inside a round ink-brush circle.` |
| 20 | `status_slow.png` | Trì Trệ | `Status icon: a small snail-shell spiral in light ink inside a round ink-brush circle.` |
| 21 | `status_seal.png` | Điểm Huyệt | `Status icon: a single acupuncture needle pointing down onto a small dot, inside a round ink-brush circle.` |
| 22 | `status_stun.png` | Định Thân | `Status icon: three small ochre-gold stars circling in a ring inside a round ink-brush circle.` |
| 23 | `status_haste.png` | Khinh Công | `Status icon: a light feather with two short speed lines inside a round ink-brush circle.` |
| 24 | `status_charge.png` | Vận Khí | `Status icon: a single upward ochre-gold swirl of qi inside a round ink-brush circle.` |
| 25 | `status_regen.png` | Điều Tức | `Status icon: a small green-grey leaf with a circular arrow inside a round ink-brush circle.` |
| 26 | `status_heal.png` | Liệu Thương | `Status icon: a small medicine gourd inside a round ink-brush circle.` |

## Chân dung trong màn trận (quyết định #26)

Avatar hai bên màn trận nằm trong khung `hero_portrait_frame`, chỉ lộ **nửa thân trên**. Đối thủ quay mặt sang **trái** (về phía người chơi). Lưu vào `Textures2D/UI/`. Chưa có hình thì game tạm dùng `hero_idle`.

| # | File | Kích thước | Prompt |
|---|---|---|---|
| 27 | `hero_portrait.png` | 1024x1536 | `Half-body portrait of the same wandering swordsman as the hero image (grey-blue travelling robe, bamboo hat on the back, calm face), three-quarter view turned slightly to the right, from the waist up, the figure fills the frame, transparent background, no text.` |
| 28 | `enemy_portrait_1.png` | 1024x1536 | `Half-body portrait of a stern rival swordsman in a dark ink-black robe with a vermilion sash, hair tied high, three-quarter view facing left, from the waist up, the figure fills the frame, transparent background, no text.` |
| 29 | `enemy_portrait_2.png` | 1024x1536 | `Half-body portrait of a burly martial monk-like fighter in an ochre robe with prayer beads, bald head, three-quarter view facing left, from the waist up, the figure fills the frame, transparent background, no text.` |
| 30 | `enemy_portrait_3.png` | 1024x1536 | `Half-body portrait of a cold female swordswoman in a pale blue-grey robe with a veil hat, three-quarter view facing left, from the waist up, the figure fills the frame, transparent background, no text.` |

## Banner trên cùng của Home (2026-10-04)

Dải banner cao khoảng 110 px chạy suốt chiều ngang màn Home. Bên trái là ảnh đại diện, tên, cấp và thanh EXP; bên phải là 4 ô tiền tệ và nút Cài đặt. Lưu vào `Textures2D/UI/`. Chưa có hình thì game dùng tạm dải giấy hiện tại.

| # | File | Kích thước | Prompt |
|---|---|---|---|
| 31 | `home_banner.png` | 1536x256 | `A long horizontal top banner strip for a game menu bar: a rolled-open rice-paper scroll band spanning the full width, thin deep-ink brush border along the bottom edge with a slightly torn dry-brush edge, faint ink-wash mountains very low-contrast inside, small ochre-gold cloud ornaments at the far left and far right ends, the middle area plain and calm for UI on top, flat 2D, transparent background outside the strip, no text.` (Unity: 9-slice ngang, hai đầu giữ nguyên) |
| 32 | `currency_slot.png` | 1024x256 | `A small horizontal pill-shaped plaque for showing a number: dark ink-wash lacquer inside, thin ochre-gold border, a round empty socket on the left end for an icon, the rest empty, flat 2D, transparent background, no text.` (Unity: 9-slice) |
| 33 | `avatar_frame.png` | 1024x1024 | `A round character avatar frame: a circular ink-brush ring with ochre-gold cloud ornaments at the top and bottom and a small vermilion tassel hanging from the bottom right, the inside of the circle completely empty and transparent, transparent background, no text.` |
| 34 | `icon_settings.png` | 1024x1024 | `Settings icon: a bronze bagua-style octagonal disc with a small round hole in the center, ochre-gold and deep ink flat washes, bold dry-brush outline, centered, transparent background, no text, no symbols that look like letters.` |

## Huy hiệu phẩm (Thương Hội, Bao Phục, Tàng Bảo Các)

Lưu vào `Textures2D/UI/`, 1024x1024, nền trong suốt. Gửi **tin 1** trước (kèm ảnh mẫu `01-style-board-thuy-mac.png`), rồi gửi lần lượt **tin 2–7**, mỗi tin một hình, trong cùng cuộc trò chuyện để giữ cùng một kiểu.

**Tin 1 (dùng chung):** `I need a set of 6 rarity badge icons for a wuxia ink-wash game UI. All 6 must share the exact same shape and size: a small round jade-and-metal medallion seen straight on, with a simple cloud-scroll ornament around the rim. Only the color, material and the amount of ornament change, getting richer with each rank. Centered, transparent background, no text, no letters, no numbers, no symbols that look like characters, no square or angular fret patterns. Readable when shrunk to 32x32 pixels: bold silhouette, strong color, little fine detail. I will ask for them one at a time.`

| # | File | Tin |
|---|---|---|
| 35 | `rarity_common.png` | `Rank 1 (Phàm Phẩm): plain grey iron medallion, matte, a thin plain rim, no gems, very simple.` |
| 36 | `rarity_uncommon.png` | `Rank 2 (Lương Phẩm): green jade medallion with a bronze rim, one small leaf-shaped cloud scroll at the top.` |
| 37 | `rarity_rare.png` | `Rank 3 (Tinh Phẩm): deep blue lapis medallion with a silver rim, cloud scrolls at top and bottom, a soft blue sheen.` |
| 38 | `rarity_epic.png` | `Rank 4 (Tuyệt Phẩm): purple amethyst medallion with a dark gold rim, cloud scrolls on four sides, faint violet glow.` |
| 39 | `rarity_legendary.png` | `Rank 5 (Thần Binh): bright gold medallion with an amber gem center, ornate cloud scrolls all around, warm golden glow and a few light rays.` |
| 40 | `rarity_supreme.png` | `Rank 6 (Chí Tôn): crimson-red jade medallion with a gold rim and a small flame-like crest on top, ornate cloud scrolls, strong red-gold glow.` |

Màu theo bảng thuật ngữ: Phàm xám · Lương lục · Tinh lam · Tuyệt tím · Thần Binh vàng kim · Chí Tôn đỏ.

## Để sau (chưa cần vẽ)
- 7 huy hiệu môn phái (triện vuông, 1024x1024): khi làm hệ thống cấp Hiệp Khách + Môn Phái (quyết định #21).
- Bản đồ chọn ải Hành Tẩu Giang Hồ, sảnh Luận Kiếm Đài: khi làm hai màn đó.
- Chữ thư pháp "Thắng/Bại", logo game: AI vẽ chữ Việt không chuẩn, dùng font trong game thay vì hình.
