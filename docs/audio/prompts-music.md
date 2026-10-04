# Prompt nhạc nền: Nhất Nang Giang Hồ

Tài liệu này dành cho người tạo nhạc bằng AI. Chọn công cụ, rồi dán từng prompt (tiếng Anh) vào công cụ đó.

## Chọn công cụ và giấy phép
| Công cụ | Ghi chú |
|---|---|
| **Suno** (suno.com) | Dễ dùng, có chế độ *Instrumental*. Dùng thương mại cần gói **Pro/Premier** lúc tạo bài (bài tạo bằng gói Free không được bán kèm game). |
| **Udio** (udio.com) | Chất lượng nhạc cụ dân tộc tốt. Điều khoản thương mại cũng theo gói trả phí. |
| **Runway** (đã kết nối) | Tạo nhạc cần gói trả phí. Tài khoản hiện đang ở gói Free. |
| Thư viện miễn phí | Pixabay Music, Freesound (CC0). Phải kiểm từng bài; khó tìm đúng chất võ hiệp. |

**Quy ước chung:**
- Bật chế độ **Instrumental**, không lời.
- Mỗi bài dài 1:30–3:00 và **lặp được** (loop).
- Tải bản WAV nếu có, không thì MP3 320 kbps.
- Đặt tên file theo cột "Khóa" và để vào `One-Bag-Jianghu/Assets/_Game/Audio/Music/`.
- **Không** dùng giai điệu có sẵn của phim hay game khác, và không nhắc tên tác phẩm trong prompt.

## Danh sách bài

| # | Khóa | Dùng ở | Cảm xúc |
|---|---|---|---|
| 1 | `bgm_home` | Màn chính (trấn nhỏ) | Thanh bình, phiêu lãng, mời gọi lên đường |
| 2 | `bgm_map_hoi_1` | Bản đồ Hồi 1 (Thanh Khê, lá phong) | Ấm, mùa thu, hơi bâng khuâng |
| 3 | `bgm_map_hoi_2` | Bản đồ Hồi 2 (đầm độc, Bách Thảo Cốc) | U ám, bí hiểm, ẩm ướt |
| 4 | `bgm_map_hoi_3` | Bản đồ Hồi 3 (tuyết sơn) | Lạnh, cô tịch, hùng vĩ |
| 5 | `bgm_battle` | Trận thường | Dồn dập, quyết liệt nhưng không ồn |
| 6 | `bgm_boss` | Trận boss | Nặng, hoành tráng, có cao trào |
| 7 | `bgm_pvp` | Luận Kiếm Đài | Trang nghiêm, đấu trường, khí thế |
| 8 | `sting_victory` | Thắng trận (5–8 giây, không lặp) | Hào sảng |
| 9 | `sting_defeat` | Thua trận (5–8 giây, không lặp) | Trầm, tiếc nuối |

Nên ưu tiên tạo 1, 5, 8, 9 trước: đó là bộ tối thiểu để chơi thử.

## Prompt

### 1. `bgm_home`
```
Instrumental, no vocals. Traditional Chinese wuxia ambient, peaceful mountain town at dawn. Guzheng plucked melody, soft dizi bamboo flute, light erhu harmony, gentle hand drum, warm string pad. Calm, adventurous, a feeling of setting out on a journey. Tempo 80 BPM, pentatonic scale. Seamless loop, no fade-out ending, about 2 minutes.
```

### 2. `bgm_map_hoi_1`
```
Instrumental, no vocals. Chinese traditional, autumn maple forest by a river, wandering swordsman travelling. Pipa and guzheng duet, xiao flute, light woodblock percussion, soft wind. Warm, nostalgic, slightly melancholic but hopeful. 90 BPM, pentatonic. Seamless loop, about 2 minutes.
```

### 3. `bgm_map_hoi_2`
```
Instrumental, no vocals. Dark Chinese traditional ambient, misty poison swamp and hidden herbal valley. Low erhu glissando, bawu flute, sparse guqin harmonics, deep frame drum, dripping water textures, insect-like percussion. Mysterious, tense, eerie but not horror. 70 BPM, minor pentatonic. Seamless loop, about 2 minutes.
```

### 4. `bgm_map_hoi_3`
```
Instrumental, no vocals. Chinese traditional, snowy mountain peaks and frozen temple. Guqin solo with long reverb, xiao flute, bowed cymbal shimmer, distant large drum, cold airy string pad. Lonely, majestic, solemn. 72 BPM. Seamless loop, about 2 minutes.
```

### 5. `bgm_battle`
```
Instrumental, no vocals. Chinese wuxia battle music. Fast guzheng tremolo and rapid pipa strums, driving taiko and Chinese war drums, sharp dizi flute lines, erhu lead, low strings ostinato. Intense, heroic, focused; leave space in the mid range for sound effects. 140 BPM, pentatonic minor. Seamless loop, about 2 minutes, no long intro.
```

### 6. `bgm_boss`
```
Instrumental, no vocals. Epic Chinese wuxia boss battle. Heavy war drums and gongs, low brass and strings, aggressive erhu and suona lead, choir-like pad without words, guzheng runs. Dark, powerful, building to a climax and returning to the main riff. 150 BPM. Seamless loop, about 2 minutes 30 seconds.
```

### 7. `bgm_pvp`
```
Instrumental, no vocals. Chinese martial arts tournament arena. Steady ceremonial drums, gong accents, bold dizi and suona fanfare motifs, guzheng rhythmic patterns, strings. Proud, competitive, dignified. 120 BPM. Seamless loop, about 2 minutes.
```

### 8. `sting_victory`
```
Instrumental, no vocals. Short Chinese traditional victory fanfare, 6 seconds. Bright suona and dizi phrase, gong hit and drum roll, guzheng glissando ending. Triumphant, heroic. Clear ending, no loop.
```

### 9. `sting_defeat`
```
Instrumental, no vocals. Short Chinese traditional defeat cue, 6 seconds. Slow descending erhu phrase, soft guqin notes, single low gong fading out. Melancholic, quiet. Clear ending, no loop.
```

## Gửi lại cho Claude
- Mình kiểm: độ dài, chỗ nối khi lặp (có hụt nhịp không), âm lượng (chuẩn hóa khoảng −16 LUFS cho nhạc nền, −14 cho sting).
- Mình nối vào game qua hệ thống âm thanh: nhạc chuyển mượt giữa các màn, nhạc bản đồ đổi theo Hồi, có chỉnh âm lượng trong Cài đặt.
- Cần cắt lại cho điểm lặp khớp thì mình làm được bằng công cụ.
