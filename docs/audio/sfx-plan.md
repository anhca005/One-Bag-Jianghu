# Kế hoạch hiệu ứng âm thanh (SFX)

> Trạng thái (2026-10-04): **bộ A đã tạo và gắn vào game** (28 tiếng, 50 bản, 51 credit Runway); hệ thống phát đã làm (Codex). Bộ B, C và âm thanh nền chưa làm.
> **Bản chốt danh mục:** Gemini mở rộng lên 79 tiếng, gán tiếng cho cả 73 vật phẩm và 10 Tuyệt Kỹ có tiếng riêng, kèm quy tắc âm lượng và tua nhanh. Xem `tools/gemini/out/audio/sfx-catalog.json`, `sfx-map.json` và [báo cáo](../reviews/2026-10-08-gemini-sfx-map.md). Khi bắt đầu làm âm thanh, dùng các file đó thay cho bảng mục 3 bên dưới. Claude đã kiểm: mọi khóa được dùng đều có trong danh mục.
> Nhạc nền nằm ở [prompts-music.md](prompts-music.md) (đang gác).

## 1. Công cụ và cách chia việc

| Bước | Ai | Việc |
|---|---|---|
| 1 | Claude | Chốt danh mục (mục 3), tạo tiếng bằng **Runway** (hiệu ứng âm thanh, MP3), mỗi tiếng 2–3 bản để chọn. |
| 2 | Claude | Chuẩn hóa: cắt khoảng lặng đầu/cuối, cân âm lượng (~−16 LUFS cho tiếng trận, −20 LUFS cho giao diện), chuyển sang `.ogg` mono 44,1 kHz. Đặt vào `Assets/_Game/Audio/Sfx/<nhóm>/`. |
| 3 | Codex | Hệ thống phát âm thanh (mục 4). |
| 4 | Người dùng | Nghe thử trong game, đánh dấu tiếng cần làm lại. |
| 5 | Claude | Tạo lại những tiếng bị loại, cập nhật bảng trạng thái ở cuối file. |

**Vì sao chọn Runway:**
- Claude, ChatGPT và Gemini không có công cụ tạo hiệu ứng âm thanh riêng; ChatGPT và Gemini chỉ ra tiếng kèm theo video.
- Runway đã kết nối và tạo thử được trên gói Free (còn 499 credit lúc 2026-10-03).
- Giá 1 credit cho mỗi giây; cả bộ dự kiến khoảng 120–180 credit, kể cả bản dự phòng.

**Cần kiểm trước khi phát hành:** điều khoản dùng thương mại cho âm thanh tạo bằng gói Free. Nếu không cho phép thì tạo lại bằng gói trả phí lúc làm bản chính thức (giữ nguyên prompt).

**Quy ước:**
- Prompt viết tiếng Anh, tả *nguồn → hành động → không gian*.
- Luôn ghi "dry, close, no music"; trừ âm thanh nền thì ghi "loop".
- Tiếng trận đánh lặp lại rất nhiều lần: giữ **ngắn (≤ 0,8 s), gọn, không vang dài**.
- Mỗi tiếng đánh thường cần **2–3 biến thể**, phát ngẫu nhiên kèm lệch cao độ ±5% để nghe không nhàm.

## 2. Phong cách
Võ hiệp cổ trang, chất liệu thật: thép, gỗ, tre, vải, da, đá. Tiếng "phép" như Tuyệt Kỹ hay trạng thái phải lấy từ chất liệu tự nhiên (gió, lửa, nước, chuông đồng, dây đàn), không dùng âm điện tử hay sci-fi. Giao diện dùng gỗ, giấy, ngọc, chuông nhỏ, hợp nền giấy Thủy Mặc.

## 3. Danh mục

Ưu tiên: **A** = bộ tối thiểu để chơi thử; **B** = đủ trải nghiệm; **C** = trau chuốt.

### 3.1 Đòn đánh theo nhóm binh khí (sự kiện `Trigger` BASIC; `Damage` khi trúng)

Mỗi nhóm có 1 tiếng **ra đòn** (`atk_*`) phát lúc `Trigger`. Tiếng **trúng** dùng chung theo chất liệu (mục 3.2).

| Khóa | Nhóm | Ưu tiên | Prompt |
|---|---|---|---|
| `atk_sword` | Kiếm | A | `Light steel jian sword slash, fast thin metallic whoosh with a faint ring. Dry, close, no music. 0.5s` |
| `atk_blade` | Đao | A | `Heavy curved dao saber swing, thick whoosh with a rough metal edge. Dry, close, no music. 0.6s` |
| `atk_dual` | Song Đao / Song Kiếm | B | `Two quick blade slashes in rapid succession, crossing whooshes, light metal scrape. Dry, close, no music. 0.6s` |
| `atk_spear` | Thương | B | `Spear thrust: sharp forward air stab with wooden shaft creak and a red tassel flutter. Dry, close, no music. 0.5s` |
| `atk_staff` | Côn / Bổng | B | `Long wooden staff spin and swing, deep hollow whoosh, bamboo knock at the end. Dry, close, no music. 0.6s` |
| `atk_axe` | Rìu (Phủ) | A | `Heavy iron axe chop, slow powerful whoosh with weight, metal head wobble. Dry, close, no music. 0.7s` |
| `atk_hammer` | Chùy | B | `Massive iron mace swung hard, very low heavy whoosh, chain rattle. Dry, close, no music. 0.7s` |
| `atk_fist` | Quyền Sáo | A | `Fast martial arts punch, cloth sleeve snap and a short air burst, leather knuckle guard. Dry, close, no music. 0.4s` |
| `atk_palm` | Chưởng | B | `Open palm strike pushing internal energy, soft deep air pulse, wind push, robe flutter. Dry, close, no music. 0.6s` |
| `atk_claw` | Trảo | B | `Tiger claw swipe, three sharp scratching tearing whooshes, metal claw tips. Dry, close, no music. 0.5s` |
| `atk_whip` | Tiên (Roi) | B | `Leather whip crack, quick swish then a sharp snap. Dry, close, no music. 0.5s` |
| `atk_fan` | Phiến | B | `Iron folding fan snapping open with a flick, paper and steel ribs, quick airy swipe. Dry, close, no music. 0.5s` |
| `atk_brush` | Phán Quan Bút | C | `Iron calligraphy brush jab, short pointed stab of air with a tiny metallic tick. Dry, close, no music. 0.4s` |
| `atk_zither` | Cầm | B | `Guqin string plucked hard as an attack, sharp twang releasing a wave of sound, short decay. Dry, close, no music. 0.7s` |
| `atk_flute` | Tiêu | C | `Bamboo xiao flute sharp piercing note as a sonic attack, breathy attack, quick fade. Dry, close, no music. 0.6s` |
| `atk_hidden` | Ám Khí | A | `Small throwing darts flicked, two or three tiny fast whistling whizzes. Dry, close, no music. 0.4s` |
| `atk_bow` | Cung / Nỏ | B | `Bow string release twang and an arrow zipping away. Dry, close, no music. 0.6s` |

### 3.2 Trúng đòn và phòng thủ

| Khóa | Sự kiện | Ưu tiên | Prompt |
|---|---|---|---|
| `hit_metal` | `Damage` từ đao, kiếm, thương, rìu, ám khí, cung | A | `Blade hit on padded cloth armor, short meaty thud with a light metal tick. Dry, close, no music. 0.3s` |
| `hit_blunt` | `Damage` từ quyền, chưởng, côn, chùy | A | `Blunt martial arts impact on body, deep punchy thud, cloth. Dry, close, no music. 0.3s` |
| `hit_crit` | `Damage` bạo kích | A | `Powerful critical strike impact, sharp crack followed by a heavy thump, brief metallic ring. Dry, close, no music. 0.5s` |
| `parry` | `Parry` | A | `Two steel blades clash and deflect, bright metallic clang with short ring. Dry, close, no music. 0.4s` |
| `shield_gain` | `Shield` | A | `Protective internal energy barrier forming, soft whoosh rising into a low bronze hum. Dry, close, no music. 0.6s` |
| `shield_hit` | `Damage` vào khiên | B | `Strike absorbed by an energy barrier, muffled thump with a dull bronze bell undertone. Dry, close, no music. 0.3s` |
| `dmg_reduce` | `DmgReduce` (Hộ Thân) | C | `Iron skin hardening, low stone grind and a firm thump. Dry, close, no music. 0.5s` |

### 3.3 Tuyệt Kỹ, Nội Lực, hồi phục

| Khóa | Sự kiện | Ưu tiên | Prompt |
|---|---|---|---|
| `ultimate` | `Trigger` ULTIMATE (dùng chung; phát kèm `atk_*` của món) | A | `Martial arts ultimate technique unleashed: deep drum hit, rising wind gust, sharp release burst. Dry, close, no music. 1.0s` |
| `charge` | `Charge` (tích Nộ đầy) | B | `Energy gathering, short rising breathy swell with a faint chime at the peak. Dry, close, no music. 0.6s` |
| `qi_gain` | `Qi` (+Nội Lực) | C | `Soft inhale of internal energy, gentle airy rise, tiny jade chime. Dry, close, no music. 0.4s` |
| `heal` | `Heal` | A | `Gentle healing, soft water trickle and a warm rising chime. Dry, close, no music. 0.6s` |
| `buff` | `Buff` | B | `Power up, quick rising whoosh with a bright gong tap. Dry, close, no music. 0.5s` |
| `max_hp` | `MaxHp` (Nội Thương) | C | `Internal injury, dull inward thump with a low groan of strain. Dry, close, no music. 0.5s` |

### 3.4 Trạng thái (sự kiện `Status` khi gây; `Dot` / `Burst` khi gây sát thương theo thời gian)

| Khóa | Sự kiện | Ưu tiên | Prompt |
|---|---|---|---|
| `burn_apply` | Gây Bỏng | A | `Flames catching on cloth, quick whoosh of fire ignition. Dry, close, no music. 0.5s` |
| `burn_tick` | `Dot` / `Burst` bỏng | B | `Short fire crackle and sizzle. Dry, close, no music. 0.3s` |
| `poison_apply` | Gây Độc | A | `Poison spreading, wet bubbling hiss, sickly. Dry, close, no music. 0.5s` |
| `poison_tick` | `Dot` / `Burst` độc | B | `Short acidic sizzle and bubble pop. Dry, close, no music. 0.3s` |
| `bleed_apply` | Gây Chảy Máu | A | `Deep cutting slice through cloth and flesh, wet tear. Dry, close, no music. 0.4s` |
| `bleed_tick` | `Dot` / `Burst` chảy máu | B | `Small wet drip and faint cut sting. Dry, close, no music. 0.3s` |
| `stun` | Choáng | A | `Dazed stun, blunt knock then a short ringing bell tone wobbling. Dry, close, no music. 0.7s` |
| `exhaustion` | Bắt đầu Kiệt Lực (45 s) | B | `Exhaustion sets in, heavy tired breath and a low ominous gong. Dry, close, no music. 1.2s` |

### 3.5 Trận đấu

| Khóa | Khi nào | Ưu tiên | Prompt |
|---|---|---|---|
| `battle_start` | `Start` | A | `Battle begins, single large war drum hit followed by a short gong. Dry, no music. 1.2s` |
| `win` | `End` thắng (thay `sting_victory` nếu chưa có nhạc) | A | `Victory, triumphant gong strike with a bright cymbal shimmer. Dry, no music. 1.5s` |
| `lose` | `End` thua | A | `Defeat, low muffled gong and a fading wind gust. Dry, no music. 1.5s` |
| `ko` | Một bên hết máu | B | `Fighter falls, body collapse on wooden floor with cloth rustle. Dry, close, no music. 0.8s` |

### 3.6 Giao diện

| Khóa | Khi nào | Ưu tiên | Prompt |
|---|---|---|---|
| `ui_click` | Bấm nút | A | `Soft wooden UI click, small tap on lacquered wood. Dry, close, no music. 0.15s` |
| `ui_hover` | Rê chuột lên nút (âm rất nhỏ) | C | `Very soft paper brush tick, subtle. Dry, close, no music. 0.1s` |
| `ui_open` | Mở popup | A | `Paper scroll unrolling quickly, crisp parchment rustle. Dry, close, no music. 0.4s` |
| `ui_close` | Đóng popup | A | `Paper scroll rolling up quickly, short rustle. Dry, close, no music. 0.3s` |
| `ui_error` | Thao tác không hợp lệ, thiếu tiền | A | `Dull wooden block knock, low and short, negative feedback. Dry, close, no music. 0.2s` |
| `item_pick` | Nhấc món đồ (kéo) | A | `Picking up an object from a cloth bag, short leather and cloth rustle. Dry, close, no music. 0.25s` |
| `item_drop` | Đặt món vào ô | A | `Placing an object on a wooden shelf, solid soft clack. Dry, close, no music. 0.2s` |
| `item_drop_bad` | Thả sai chỗ (trả về) | B | `Object slides back, quick soft scrape. Dry, close, no music. 0.25s` |
| `coin` | Mua hoặc bán, nhận Bạc | A | `A few ancient copper coins dropping into a pouch, jingling. Dry, close, no music. 0.5s` |
| `upgrade` | Nâng Trọng | B | `Blacksmith hammer strikes hot metal on anvil, one ringing hit with sizzle. Dry, close, no music. 0.8s` |
| `merge` | Gộp phẩm | B | `Two jade pieces fuse, bright chime rising with a soft magical shimmer of wind chimes. Dry, close, no music. 0.9s` |
| `level_up` | Lên cấp | B | `Level up, ascending bronze bell chimes and a soft gong. Dry, no music. 1.2s` |
| `reward` | Nhận thưởng ải, sao | B | `Reward reveal, sparkle of small bells with a coin jingle. Dry, no music. 0.8s` |
| `stage_select` | Chọn ải trên bản đồ | C | `Ink brush stroke on paper, quick swish. Dry, close, no music. 0.3s` |
| `dialogue_next` | Sang câu thoại | C | `Turning a page of an old book, soft paper flip. Dry, close, no music. 0.3s` |
| `stamina_low` | Không đủ Thể Lực | C | `Tired sigh with a dull wooden knock. Dry, close, no music. 0.5s` |

### 3.7 Âm thanh nền (loop, ưu tiên C)

| Khóa | Dùng ở | Prompt |
|---|---|---|
| `amb_home` | Màn chính (trấn) | `Quiet ancient Chinese town morning, distant birds, light wind, faint market murmur, soft wooden wind chimes. Seamless loop. 20s` |
| `amb_map_hoi_1` | Bản đồ Hồi 1 | `Autumn forest by a stream, rustling maple leaves, flowing water, light wind. Seamless loop. 20s` |
| `amb_map_hoi_2` | Bản đồ Hồi 2 | `Misty poison swamp, bubbling mud, insects, distant frogs, damp eerie air. Seamless loop. 20s` |
| `amb_map_hoi_3` | Bản đồ Hồi 3 | `Snowy mountain peak, cold howling wind, occasional ice creak. Seamless loop. 20s` |

**Tổng:** khoảng 60 tiếng.
- Bộ A gồm 27 tiếng, tốn khoảng 30–40 credit với 2 bản mỗi tiếng.
- Cả bộ khoảng 120–180 credit, tính cả bản dự phòng và 4 âm thanh nền 20 giây (80 credit).

## 4. Hệ thống phát (giao Codex sau)

Đề bài dự kiến: `tools/codex/task-audio-sfx.md`.

### 4.1 `SfxLibrary` (ScriptableObject, `Resources/Audio/SfxLibrary.asset`)
- Mỗi mục gồm: khóa, danh sách biến thể `AudioClip[]`, âm lượng gốc, độ lệch cao độ, thời gian chờ tối thiểu giữa hai lần phát (chống dồn tiếng khi nhiều sự kiện trùng tick).
- Có menu Editor quét `Audio/Sfx/**` và tự điền theo tên file (`atk_sword_1.ogg`, `atk_sword_2.ogg` → khóa `atk_sword`). Cách làm giống `ItemIconLibrary`.

### 4.2 `AudioManager`
- Singleton `DontDestroyOnLoad`, có pool khoảng 12 `AudioSource`.
- Ba kênh: Master, Sfx, Ui (Music để sẵn cho nhạc nền sau này). Âm lượng lưu vào `PlayerPrefs` (khóa `OneBagJianghu.Volume.*`).
  - Nhớ thêm các khóa này vào `PlayerDataGuard` của test PlayMode.
- `Play(string key)`; khóa không tồn tại thì bỏ qua và log 1 lần, không lỗi.
- Giới hạn số tiếng cùng khóa phát đồng thời (ví dụ tối đa 3).

### 4.3 Ánh xạ sự kiện trận → tiếng (`BattleSfxResolver`, tách khỏi VFX)
- Ánh xạ:
  - `Trigger` BASIC → `atk_<weaponClass>`;
  - `Trigger` ULTIMATE → `ultimate` + `atk_<weaponClass>`;
  - `Damage` → `hit_metal` hoặc `hit_blunt` theo nhóm binh khí nguồn; `hit_crit` nếu bạo kích; `shield_hit` nếu bị khiên chặn hết;
  - `Status` → `<status>_apply`;
  - `Dot` / `Burst` → `<status>_tick`;
  - các loại còn lại → bảng ở mục 3.
- Phát theo thời gian của phát lại trận. Khi tua nhanh hoặc tua lại thì **không** phát dồn: chỉ phát sự kiện trong khoảng thời gian vừa chạy qua khi tốc độ ≤ 2×.
- Ở tốc độ cao, chỉ phát `Trigger`, `Damage` bạo kích và `End`.

### 4.4 Giao diện
- `UiSfx`: component gắn vào nút và popup (click, open/close). Builder thêm component này vào prefab dùng chung, không sửa tay từng prefab.
- Kéo thả (`ItemView`, Tàng Bảo Các), Thương Hội, nâng cấp/gộp, lên cấp, bản đồ PvE và hội thoại gọi `AudioManager.Play(...)` tại đúng chỗ.

### 4.5 Cài đặt
- Thêm 3 thanh trượt âm lượng (Tổng / Hiệu ứng / Giao diện) vào popup Cài đặt.

### 4.6 Test
- **EditMode:**
  - mọi khóa trong bảng ánh xạ có trong `SfxLibrary` hoặc nằm trong danh sách "chưa có tiếng";
  - resolver trả đúng khóa cho từng loại sự kiện;
  - không phát dồn khi tua.
- **PlayMode:** một trận phát lại không lỗi khi thiếu file âm thanh.

## 5. Thứ tự làm (khi bắt đầu)
1. Claude tạo bộ **A** (27 tiếng), chuẩn hóa, đưa vào `Audio/Sfx/`.
2. Codex làm mục 4. Có thể chạy song song với bước 1, vì hệ thống chạy được cả khi thiếu file.
3. Claude build, chạy test và quay video ngắn một trận để người dùng nghe.
4. Người dùng duyệt. Claude làm bộ B, rồi bộ C.

## 6. Trạng thái tiếng

- **2026-10-04, bỏ bộ Runway:** người dùng nghe thấy "cực kì khó nghe, không phù hợp". 50 file MP3 Runway đã gỡ (lưu ở scratchpad `sfx-runway-removed/`).
- **Thay bằng bộ thu âm thật CC0 của Kenney** (Impact Sounds, Interface Sounds, RPG Audio, UI Audio; giấy phép ở `Assets/_Game/Audio/Licenses/kenney-cc0-license.txt`): 30 khóa, 44 file `.ogg`, đặt theo `Audio/Sfx/<nhóm>/<khóa>_<n>.ogg`.
  - Giao diện: click, mở/đóng (sách), lỗi, nhấc/thả đồ (vải, đặt sách), tiền xu, lật trang hội thoại, chọn ải, lên cấp, nâng Trọng (kim loại), gộp (ngọc), sao.
  - Trận: rút dao (kiếm/đao/thương), chặt (rìu/chùy), trúng đòn (dao cắt, đấm), bạo kích, đỡ đòn (kim loại), khiên bị đánh, bắt đầu (chuông), thắng/thua.
  - **Cố ý để im** (Kenney không có tiếng hợp): Tuyệt Kỹ, hồi máu, bỏng, độc, chảy máu, choáng, Cương Khí, các binh khí nhạc khí/ám khí/quyền. Bổ sung sau bằng bộ CC0 khác nếu người dùng muốn.
- Người dùng nghe duyệt bộ Kenney; tiếng nào không hợp thì đổi file khác cùng bộ.
- **2026-10-04, thay tiếng giao diện:** người dùng thấy tiếng lật sách ồn. Đổi sang tiếng click ngắn (dưới 0,15 giây), giữ nguyên tên file và `.meta` nên không cần dựng lại thư viện:
  - `ui_click` ← UI Audio `switch2` (người dùng chọn 2026-10-06, cả hai biến thể); `ui_open`/`ui_close` ← Interface `open_001`/`close_001`; `dialogue_next` ← Interface `tick_004`, `tick_002`.
  - Kéo thả: `item_pick` ← UI Audio `mouseclick1`, `click5`; `item_drop` ← `mouserelease1`, `click2`; `item_drop_bad` ← Interface `back_002`.
- **2026-10-06:** người dùng chọn tiếng gạt công tắc (UI Audio `switch2`) cho mọi tiếng bấm: `ui_click`, `ui_open`, `ui_close`, `dialogue_next`, `stage_select`.
- **2026-10-06:** bỏ hẳn tiếng mở/đóng popup (`ui_open`, `ui_close` không còn được gọi; file vẫn giữ trong thư viện).
