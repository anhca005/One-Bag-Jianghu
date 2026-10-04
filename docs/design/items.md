# Vật phẩm

Thuật ngữ và mã xem [glossary.md](glossary.md).

## Hệ binh khí

**Binh khí tối thiểu 2 ô**, không có binh khí 1x1 (kiểm tra khi nạp dữ liệu). Món 1x1 chỉ dành cho Công Pháp, Đan Dược, Kỳ Trân.

| Hệ | Mã | Kích thước | Tốc độ | Cơ chế đặc trưng |
|---|---|---|---|---|
| Quyền Sáo | `fist` | 1x2 | Rất nhanh (1–1.5s) | Liên Kích, ra đòn nhanh nhất |
| Chưởng | `palm` | 1x2 | Trung bình | Nội Thương |
| Trảo | `claw` | 1x2 | Nhanh | Xuất Huyết |
| Kiếm | `sword` | 1x3 | Trung bình | Hội Tâm cao, Tuyệt Kỹ nhiều nhát |
| Đao | `blade` | 1x3 | Chậm | Sát thương đơn lớn, Liệt Hỏa |
| Song Đao / Song Kiếm | `dual` | 2x2 | Nhanh | Liên Kích x2 |
| Thương | `spear` | 1x4 | Trung bình | Xuyên Cương Khí |
| Côn / Bổng | `staff` | 1x4 | Trung bình | Định Thân, cắt Tuyệt Kỹ |
| Rìu (Phủ) | `axe` | 2x2 | Rất chậm (9s) | Phá Cương cực mạnh |
| Chùy | `hammer` | 2x2 | Chậm | Định Thân, hạ Tốc Xuất Chiêu địch |
| Tiên (roi) | `whip` | 1x3 | Nhanh | Trì Trệ binh khí địch |
| Phiến / Phán Quan Bút | `fan` / `brush` | 1x2 | Trung bình | Điểm Huyệt |
| Cầm / Tiêu (âm công) | `zither` / `flute` | 2x2 / 1x2 | Chậm | Bỏ qua Cương Khí, Trì Trệ toàn bộ |
| Ám Khí | `hidden` | 1x2 | Nhanh | Tích Kịch Độc |
| Cung / Nỏ | `bow` | 1x3 | Chậm | Nhiều phát, ưu tiên phá Đan Dược địch |

## Môn phái và Tâm Pháp

Quyết định #15. **Vật phẩm trung lập:** không thuộc môn phái nào, Hiệp Khách phái nào cũng dùng được mọi món. Môn phái chỉ tác động vào trận qua **Tâm Pháp**: một nội tại buff các hệ binh khí của phái, mạnh dần theo cấp.

- Mỗi phái có **1 Tâm Pháp**, các mốc cấp **1 / 3 / 6 / 9** cộng dồn (giống Đột Phá của vật phẩm).
- Tâm Pháp chỉ áp lên binh khí có hệ nằm trong danh sách của phái. Binh khí hệ khác vẫn dùng được nhưng không được buff.
- Mỗi mốc có thể cộng: `attackPct` (% Lực Đạo), `cooldownPct` (% nhanh hơn), `crit`, `ultimateCost`, thêm hiệu ứng vào Phổ Công / Tuyệt Kỹ, thêm `triggers`.
- Dữ liệu: `server/battle-engine/src/main/resources/data/sects.json`. Đội hình khai báo `"sect"` và `"tamPhapLevel"` (1..9).
- Các mốc do Gemini thiết kế và Claude kiểm chứng ([báo cáo](../reviews/2026-09-27-gemini-tam-phap.md)): mốc 1 = +10% Lực Đạo cho mọi phái; mốc 3 là bước chỉ số, mốc 6 là cơ chế riêng, mốc 9 là hiệu ứng đặc trưng; tổng khoảng +40% giá trị các binh khí được buff. Bảng đối đầu (Trọng 1, 15 đội) ở cả 4 cấp: không đội nào vượt 70%.
- Việc tồn đọng: Bích Thủy mốc 9 "khí huyết < 40% → hồi 150" gắn vào từng cây Cầm nên 2 Cầm hồi 2 lần; Phá Cương của Thiết Phủ mốc 6/9 được nối vào cuối Tuyệt Kỹ nên phá khiên sau nhát chém.

| Môn phái | Mã | Tâm Pháp | Hệ được buff | Lối chơi |
|---|---|---|---|---|
| Kim Cang Tự | `kim_cang` | Kim Cang Thiết Cốt Quyết | Quyền, Côn | Cương Khí, Định Thân |
| Thái Thanh Quán | `thai_thanh` | Thái Thanh Kiếm Ý | Kiếm, Phiến | Nội lực, Hóa Kình |
| Liệt Diễm Đường | `liet_diem` | Liệt Diễm Tâm Hỏa Quyết | Đao, Chưởng | Liệt Hỏa |
| Huyền Độc Môn | `huyen_doc` | Vạn Độc Huyền Kinh | Ám Khí, Trảo | Kịch Độc, Xuất Huyết |
| Hàn Sơn Phái | `han_son` | Hàn Sơn Băng Tâm Quyết | Tiêu, Thương, Bút | Điểm Huyệt, Trì Trệ |
| Thiết Phủ Bang | `thiet_phu` | Khai Sơn Bá Khí | Rìu, Chùy | Phá Cương, sát thương lớn |
| Bích Thủy Am | `bich_thuy` | Bích Thủy Thanh Âm | Tiên, Cầm | Liệu Thương, Điều Tức |

## Vật phẩm mẫu (MVP)

Chỉ số sau cân bằng đợt 3 (xem [balance.md](balance.md)). Số liệu gốc nằm ở `server/battle-engine/src/main/resources/data/items.json`. "+3 Liệt Hỏa" nghĩa là đối thủ tích thêm 3 điểm (đủ 50 thì bộc phát).

| Tên | Loại | Kích thước | Độ hiếm | Hiệu ứng |
|---|---|---|---|---|
| Thiết Cốt Quyền Sáo | Quyền | 1x2 | Phàm | 1.2s. Phổ Công 11, +2 nội lực. Hội Tâm 5% |
| Tử Kim Trảo | Trảo | 1x2 | Tinh | 1.6s. Phổ Công 8, +3 Xuất Huyết, +1 nội lực. Hội Tâm 10% |
| Thất Tinh Tiêu | Ám Khí | 1x2 | Lương | 2.1s. Phổ Công 4, +6 Kịch Độc, +1 nội lực |
| Thanh Phong Kiếm | Kiếm | 1x3 | Tinh | 3.5s. Phổ Công 75, +12 nội lực. Hội Tâm 20%. Tuyệt Kỹ (40): 3 nhát × 90% |
| Liệt Diễm Đao | Đao | 1x3 | Tinh | 4.5s. Phổ Công 120, +3 Liệt Hỏa, +12 nội lực. Tuyệt Kỹ (40): 160% + 15 Liệt Hỏa |
| Phục Hổ Côn | Côn | 1x4 | Tuyệt | 5.5s. Phổ Công 200, +12 nội lực. Tuyệt Kỹ (25): 150% + Định Thân 1s |
| Khai Sơn Phủ | Rìu | 2x2 | Tinh | 6.75s. Phổ Công 280, +15 nội lực. Tuyệt Kỹ (40): Phá Cương 150 + 160% |
| Phán Quan Bút | Bút | 1x2 | Tinh | 4s. Phổ Công 28, Điểm Huyệt ngẫu nhiên 1 món địch 1s, +4 nội lực |
| Hàn Ngọc Tiêu | Tiêu | 1x2 | Tuyệt | 6s. Phổ Công 62 xuyên Cương Khí, Trì Trệ toàn bộ đồ địch 0.7s, +2 nội lực |
| Bích Thủy Cầm | Cầm | 2x2 | Tinh | 5s. Phổ Công 90 xuyên Cương Khí, +3 Điều Tức, +2 nội lực |
| Xích Sa Chưởng | Chưởng | 1x2 | Lương | 2.6s. Phổ Công 33, Nội Thương 15, +2 nội lực |
| Ngọc Cốt Phiến | Phiến | 1x2 | Tinh | 4.2s. Phổ Công 26, Điểm Huyệt 1 binh khí địch 0.6s, +2 nội lực. Hội Tâm 10% |
| Băng Phách Thương | Thương | 1x4 | Tuyệt | 4.8s. Phổ Công 135 xuyên Cương Khí, +10 nội lực. Tuyệt Kỹ (35): 150% xuyên Cương Khí + hút 8 nội lực |
| Chấn Nhạc Chùy | Chùy | 2x2 | Tinh | 6s. Phổ Công 195, Trì Trệ 1 binh khí địch 1s, +14 nội lực. Tuyệt Kỹ (35): 150% + Định Thân 1s |
| Thanh Đằng Tiên | Tiên | 1x3 | Lương | 2.4s. Phổ Công 42, Trì Trệ 1 binh khí địch 0.5s, +2 nội lực |
| Tử Mẫu Song Đao | Song Đao | 2x2 | Tinh | 2.7s. Phổ Công 2 nhát × 34, +5 nội lực. Hội Tâm 10%. Tuyệt Kỹ (35): 4 nhát × 75% |
| Truy Hồn Cung | Cung | 1x3 | Tuyệt | 4.5s. Phổ Công 3 phát × 60%, Nội Thương 20, +4 nội lực. Hội Tâm 15% |
| Thái Thanh Tâm Kinh | Công Pháp | 1x1 | Tinh | Vào trận +20 nội lực, Hóa Kình tốn ít hơn 20; Hóa Kình thành công → Vận Khí mọi binh khí 1.5s |
| Ô Tàm Y | Hộ Cụ | 2x2 | Lương | Vào trận +600 Cương Khí, Hộ Thân 8% |
| Bồi Nguyên Đan | Đan Dược | 1x1 | Lương | Khí huyết < 40% → hồi 300 (1 lần) |
| Túy Tiên Hồ Lô | Kỳ Trân | 1x1 | Tuyệt | Binh khí liền kề được Khinh Công 2s sau mỗi lần Hội Tâm |

## Đột Phá

Mỗi vật phẩm có 9 **Trọng**, nâng bằng tài nguyên PvE (quyết định #12). Mỗi Trọng tăng chỉ số theo hệ số chung ([combat.md](combat.md#hằng-số-battleconfig)). Riêng **Trọng 3, 6, 9 là Đột Phá**: món đó mở thêm hiệu ứng, cộng dồn các mốc trước. Tăng Trọng không làm đổi độ hiếm hay tên món.

Nguyên tắc thiết kế:
- Đột Phá làm **rõ bản sắc** của món (Kiếm thêm Hội Tâm, Đao thêm Liệt Hỏa, Côn thêm Cương Khí…), không chỉ cộng số.
- Mỗi mốc đáng khoảng +10% giá trị món. Món nhỏ thường được mang nhiều bản sao, nên Đột Phá của món nhỏ phải nhẹ hơn.
- **Tránh cộng điểm trạng thái tích:** ngưỡng 50 điểm làm số đòn cần để bộc phát nhảy bậc (12 → 14 điểm mỗi đòn thì từ 5 đòn còn 4 đòn), khiến sức mạnh tăng vọt. Ưu tiên giảm Tốc Xuất Chiêu hoặc tăng Hội Tâm, vì hai chỉ số này tăng từ từ.
- Kích hoạt "khi bộc phát" phải dùng `from: self`, nếu không mọi bản sao của món trong túi đều kích hoạt cùng lúc.

| Binh khí | Trọng 3 | Trọng 6 | Trọng 9 |
|---|---|---|---|
| Thiết Cốt Quyền Sáo | Tốc Xuất Chiêu −0.1s | Hội Tâm → +30 Cương Khí | Phổ Công thêm 1 đòn 15% |
| Phục Hổ Côn | Phổ Công +20 Cương Khí | Tuyệt Kỹ hút 6 nội lực | Tuyệt Kỹ tốn −5, thêm 50% sát thương |
| Thanh Phong Kiếm | Hội Tâm +5% | Hội Tâm → +4 nội lực | Tuyệt Kỹ thêm nhát thứ 4 (90%) |
| Liệt Diễm Đao | Phổ Công +2 Liệt Hỏa | Liệt Hỏa do món này bộc phát → Vận Khí 2s | Tuyệt Kỹ tốn −10 |
| Thất Tinh Tiêu | Tốc Xuất Chiêu −0.1s | Tốc Xuất Chiêu −0.1s | Kịch Độc do món này bộc phát → hút 8 nội lực |
| Tử Kim Trảo | Hội Tâm +5%, Tốc Xuất Chiêu −0.1s | Hội Tâm → +2 Xuất Huyết | Xuất Huyết do món này bộc phát → hồi 60 |
| Khai Sơn Phủ | Phổ Công thêm Phá Cương 40 | Lực Đạo +20 | Tuyệt Kỹ thêm Định Thân 1s |
| Phán Quan Bút | Phổ Công hút 2 nội lực | Tốc Xuất Chiêu −0.5s | Điểm Huyệt thêm 1 món |
| Hàn Ngọc Tiêu | Lực Đạo +4 | Phổ Công hút 3 nội lực | Điểm Huyệt 1 binh khí địch 1s |
| Bích Thủy Cầm | Phổ Công +3 Điều Tức | Khí huyết < 50% → hồi 150 (1 lần) | Tốc Xuất Chiêu −0.5s |

Hộ Cụ, Công Pháp, Đan Dược, Kỳ Trân chưa có Đột Phá (engine đã hỗ trợ).

## Định dạng dữ liệu

File JSON là một mảng vật phẩm. Tên enum viết chữ thường. Trường sai tên sẽ bị báo lỗi khi nạp (chống gõ nhầm).

```json
{
  "id": "khai_son_phu", "name": "Khai Sơn Phủ",
  "type": "weapon", "weaponClass": "axe", "rarity": "rare", "size": [2, 2],
  "attack": 180, "cooldown": 9.0, "crit": 0.05,
  "basic": [ { "op": "damage" }, { "op": "gain_qi", "value": 15 } ],
  "ultimate": { "cost": 40, "effects": [
    { "op": "shield_break", "value": 120 },
    { "op": "damage", "hits": 3 }
  ] },
  "triggers": [],
  "unlocks": [
    { "tier": 3, "basic": [ { "op": "shield_break", "value": 80 } ] },
    { "tier": 6, "attack": 40 },
    { "tier": 9, "ultimate": [ { "op": "apply", "status": "stun", "duration": 1.0 } ] }
  ]
}
```

| Trường | Ý nghĩa |
|---|---|
| `type` | `weapon` (Binh Khí) · `armor` (Hộ Cụ) · `manual` (Công Pháp) · `pill` (Đan Dược) · `relic` (Kỳ Trân) |
| `rarity` | `common` · `uncommon` · `rare` · `epic` · `legendary` · `supreme` |
| `size` | `[rộng, cao]` theo ô |
| `attack` | Lực Đạo |
| `cooldown` | Tốc Xuất Chiêu (giây). 0 = món bị động (chỉ dùng `triggers`). Binh khí bắt buộc > 0 |
| `crit` | Tỉ lệ Hội Tâm (0.2 = 20%) |
| `basic` | Hiệu ứng Phổ Công |
| `ultimate` | `{cost, effects}`: Tuyệt Kỹ, tiêu `cost` nội lực |
| `triggers` | `[{on, from, hpBelow, status, maxUses, effects}]`, xem [combat.md](combat.md#hệ-thống-sự-kiện-trigger) |
| `unlocks` | Đột Phá: `[{tier, attack, cooldown, crit, ultimateCost, basic, ultimate, triggers}]`. `tier` là 3, 6 hoặc 9, xếp tăng dần. Các số là **lượng cộng thêm** (cooldown âm = nhanh hơn); `basic`/`ultimate`/`triggers` được **nối thêm** vào danh sách gốc. Món chưa có Tuyệt Kỹ có thể nhận Tuyệt Kỹ mới: khi đó `ultimateCost` là chi phí |

### Hiệu ứng (`op`)

| op | Tham số | Đích mặc định |
|---|---|---|
| `damage` | `pct` (× Lực Đạo, mặc định 1.0) hoặc `value` cố định; `hits`; `ignoreShield` | `enemy` |
| `gain_qi` / `drain_qi` | `value` | `self` / `enemy` |
| `shield` | `value` (Cương Khí) | `self` |
| `heal` | `value` | `self` |
| `shield_break` | `value` (Phá Cương) | `enemy` |
| `dmg_reduce` | `pct` (Hộ Thân, 0.08 = 8%) | `self` |
| `reduce_max_hp` | `value` (Nội Thương) | `enemy` |
| `parry_discount` | `value`: Hóa Kình tốn ít nội lực hơn (tối thiểu còn 20) | `self` |
| `apply` | `status` + `stacks`: số điểm tích (burn, poison, bleed) hoặc lượng hồi mỗi giây (regen); hoặc `duration` giây (stun, slow, seal, haste) | theo trạng thái |
| `charge` | `duration` giây (Vận Khí) | `self_item` |
| `add_attack` | `value` | `self_item` |
| `add_crit` | `pct` | `self_item` |

**Đích (`target`)**: cho Hiệp Khách thì dùng `self` / `enemy`; cho vật phẩm thì dùng `self_item`, `event_source` (món gây ra sự kiện), `self_all_items`, `self_other_items`, `self_all_weapons`, `adjacent_items`, `adjacent_weapons`, `enemy_random_item`, `enemy_random_weapon`, `enemy_all_items`. Chỉ các món có Tốc Xuất Chiêu mới bị tính là đích vật phẩm (trừ `self_item`, `event_source`).

Trạng thái mặc định nhắm vào: `burn/poison/bleed/stun` → `enemy`, `regen` → `self`, `slow/seal` → `enemy_random_item`, `haste` → `self_item`.

**Trọng** không khai báo trong vật phẩm mà nằm ở đội hình (`"tier": 3`). Hệ số chung xem [combat.md](combat.md#hằng-số-battleconfig).

### Đội hình (loadout)

```json
{ "name": "Kim Cang Tự", "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
  "items": [ { "id": "phuc_ho_con", "x": 0, "y": 0, "tier": 1 } ] }
```
`x, y` là ô góc trên-trái. Hai món liền kề khi chung một cạnh (chéo góc không tính). Chưa hỗ trợ xoay.

### Kiểm tra khi nạp dữ liệu
`ItemCatalog` từ chối file nếu: sai tên trường; binh khí < 2 ô, không có `cooldown` hoặc `attack ≤ 0`; `attack` âm; hiệu ứng thiếu tham số hoặc tham số ≤ 0 (`value` của qi/khiên/hồi máu…, `pct` của `dmg_reduce`/`add_crit` phải trong (0, 1]); `hits > 1` ở hiệu ứng khác `damage`; đích sai loại (Hiệp Khách ↔ vật phẩm); `from` khác `any` ở sự kiện không do một món gây ra; Đột Phá sai mốc Trọng hoặc không xếp tăng dần. Món cũng được kiểm tra lại ở từng mốc Đột Phá (ví dụ Tốc Xuất Chiêu sau khi cộng phải > 0).
