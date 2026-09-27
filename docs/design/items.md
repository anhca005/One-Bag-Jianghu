# Vật phẩm

Thuật ngữ và mã xem [glossary.md](glossary.md).

## Hệ binh khí

**Binh khí tối thiểu 2 ô**, không có binh khí 1x1 (kiểm tra khi nạp dữ liệu). Món 1x1 chỉ dành cho Tâm Pháp, Đan Dược, Kỳ Trân.

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

## Môn phái (tên tự đặt)
Vật phẩm cùng môn phái đặt **liền kề** nhau thì kích hoạt hiệu ứng bộ khi đủ 2 / 4 / 6 món.

| Môn phái | Mã | Hệ chủ đạo | Lối chơi |
|---|---|---|---|
| Kim Cang Tự | `kim_cang` | Quyền, Côn | Cương Khí, Định Thân |
| Thái Thanh Quán | `thai_thanh` | Kiếm, Phiến | Nội lực, Hóa Kình |
| Liệt Diễm Đường | `liet_diem` | Đao, Chưởng | Liệt Hỏa |
| Huyền Độc Môn | `huyen_doc` | Ám Khí, Trảo | Kịch Độc, Xuất Huyết |
| Hàn Sơn Phái | `han_son` | Tiêu, Thương | Điểm Huyệt, Trì Trệ |
| Thiết Phủ Bang | `thiet_phu` | Rìu, Chùy | Phá Cương, sát thương lớn |
| Bích Thủy Am | `bich_thuy` | Tiên, Cầm | Liệu Thương, Điều Tức |

## Vật phẩm mẫu (MVP)

Chỉ số sau cân bằng đợt 2 (xem [balance.md](balance.md)). Số liệu gốc nằm ở `server/battle-engine/src/main/resources/data/items.json`. "+3 Liệt Hỏa" nghĩa là đối thủ tích thêm 3 điểm (đủ 50 thì bộc phát).

| Tên | Loại | Kích thước | Độ hiếm | Hiệu ứng |
|---|---|---|---|---|
| Thiết Cốt Quyền Sáo | Quyền | 1x2 | Phàm | 1.2s. Phổ Công 16, +2 nội lực. Hội Tâm 5% |
| Tử Kim Trảo | Trảo | 1x2 | Tinh | 1.6s. Phổ Công 12, +3 Xuất Huyết, +1 nội lực. Hội Tâm 10% |
| Thất Tinh Tiêu | Ám Khí | 1x2 | Lương | 2s. Phổ Công 4, +6 Kịch Độc, +1 nội lực |
| Thanh Phong Kiếm | Kiếm | 1x3 | Tinh | 3.5s. Phổ Công 65, +12 nội lực. Hội Tâm 20%. Tuyệt Kỹ (40): 3 nhát × 90% |
| Liệt Diễm Đao | Đao | 1x3 | Tinh | 6s. Phổ Công 105, +3 Liệt Hỏa, +12 nội lực. Tuyệt Kỹ (40): 160% + 15 Liệt Hỏa |
| Phục Hổ Côn | Côn | 1x4 | Tuyệt | 5.5s. Phổ Công 160, +12 nội lực. Tuyệt Kỹ (30): 150% + Định Thân 1s |
| Khai Sơn Phủ | Rìu | 2x2 | Tinh | 8s. Phổ Công 240, +15 nội lực. Tuyệt Kỹ (40): Phá Cương 150 + 160% |
| Phán Quan Bút | Bút | 1x2 | Tinh | 4s. Phổ Công 44, Điểm Huyệt ngẫu nhiên 1 món địch 1s, +4 nội lực |
| Hàn Ngọc Tiêu | Tiêu | 1x2 | Tuyệt | 6s. Phổ Công 38 xuyên Cương Khí, Trì Trệ toàn bộ đồ địch 1s, +4 nội lực |
| Bích Thủy Cầm | Cầm | 2x2 | Tinh | 5s. Phổ Công 120 xuyên Cương Khí, +6 Điều Tức, +4 nội lực |
| Thái Thanh Tâm Kinh | Tâm Pháp | 1x1 | Tinh | Vào trận +20 nội lực, Hóa Kình tốn ít hơn 20; Hóa Kình thành công → Vận Khí mọi binh khí 1.5s |
| Ô Tàm Y | Hộ Cụ | 2x2 | Lương | Vào trận +600 Cương Khí, Hộ Thân 8% |
| Bồi Nguyên Đan | Đan Dược | 1x1 | Lương | Khí huyết < 40% → hồi 300 (1 lần) |
| Túy Tiên Hồ Lô | Kỳ Trân | 1x1 | Tuyệt | Binh khí liền kề được Khinh Công 2s sau mỗi lần Hội Tâm |

## Định dạng dữ liệu

File JSON là một mảng vật phẩm. Tên enum viết chữ thường. Trường sai tên sẽ bị báo lỗi khi nạp (chống gõ nhầm).

```json
{
  "id": "khai_son_phu", "name": "Khai Sơn Phủ",
  "type": "weapon", "weaponClass": "axe", "sect": "thiet_phu", "rarity": "rare", "size": [2, 2],
  "attack": 180, "cooldown": 9.0, "crit": 0.05,
  "basic": [ { "op": "damage" }, { "op": "gain_qi", "value": 15 } ],
  "ultimate": { "cost": 40, "effects": [
    { "op": "shield_break", "value": 120 },
    { "op": "damage", "hits": 3 }
  ] },
  "triggers": []
}
```

| Trường | Ý nghĩa |
|---|---|
| `type` | `weapon` · `armor` · `manual` · `pill` · `relic` |
| `rarity` | `common` · `uncommon` · `rare` · `epic` · `legendary` · `supreme` |
| `size` | `[rộng, cao]` theo ô |
| `attack` | Lực Đạo |
| `cooldown` | Tốc Xuất Chiêu (giây). 0 = món bị động (chỉ dùng `triggers`). Binh khí bắt buộc > 0 |
| `crit` | Tỉ lệ Hội Tâm (0.2 = 20%) |
| `basic` | Hiệu ứng Phổ Công |
| `ultimate` | `{cost, effects}`: Tuyệt Kỹ, tiêu `cost` nội lực |
| `triggers` | `[{on, from, hpBelow, status, maxUses, effects}]`, xem [combat.md](combat.md#hệ-thống-sự-kiện-trigger) |

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
{ "name": "Kim Cang Tự", "hp": 1200, "cols": 5, "rows": 4,
  "items": [ { "id": "phuc_ho_con", "x": 0, "y": 0, "tier": 1 } ] }
```
`x, y` là ô góc trên-trái. Hai món liền kề khi chung một cạnh (chéo góc không tính). Chưa hỗ trợ xoay.
