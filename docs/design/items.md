# Vật phẩm

Thuật ngữ và mã xem [glossary.md](glossary.md).

## Hệ binh khí

| Hệ | Mã | Kích thước | Tốc độ | Cơ chế đặc trưng |
|---|---|---|---|---|
| Quyền Sáo | `fist` | 1x1 | Rất nhanh (1–1.5s) | Liên Kích, tích nội lực nhanh nhất |
| Chưởng | `palm` | 1x2 | Trung bình | Nội Thương |
| Trảo | `claw` | 1x1 | Nhanh | Xuất Huyết |
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
| Ám Khí | `hidden` | 1x1 | Nhanh | Kịch Độc cộng dồn |
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

| Tên | Loại | Kích thước | Độ hiếm | Hiệu ứng |
|---|---|---|---|---|
| Thiết Cốt Quyền Sáo | Quyền | 1x1 | Phàm | 1.2s. Phổ Công gây 30 sát thương, +8 nội lực |
| Khai Sơn Phủ | Rìu | 2x2 | Tinh | 9s. Tuyệt Kỹ (40 nội lực): 3 lần sát thương, 120 Phá Cương |
| Phán Quan Bút | Bút | 1x2 | Tinh | 4s. Phổ Công Điểm Huyệt ngẫu nhiên 1 món địch 1.5 giây |
| Thất Tinh Tiêu | Ám Khí | 1x1 | Lương | 2s. +3 tầng Kịch Độc |
| Phục Hổ Côn | Côn | 1x4 | Tuyệt | 6s. Tuyệt Kỹ: Định Thân toàn bộ đối thủ 1.5 giây |
| Thái Thanh Tâm Kinh | Tâm Pháp | 1x1 | Tinh | Vào trận +20 nội lực; Hóa Kình thành công → Vận Khí mọi binh khí 1.5 giây |
| Ô Tàm Y | Hộ Cụ | 2x2 | Lương | Vào trận +600 Cương Khí, Hộ Thân 8% |
| Bồi Nguyên Đan | Đan Dược | 1x1 | Lương | Khí huyết < 40% → hồi 300 (1 lần) |
| Túy Tiên Hồ Lô | Kỳ Trân | 1x1 | Tuyệt | Binh khí liền kề được Khinh Công 2 giây sau mỗi lần Hội Tâm |

## Định dạng dữ liệu (dự kiến)

```json
{
  "id": "khai_son_phu",
  "type": "weapon", "class": "axe", "sect": "thiet_phu",
  "rarity": "rare", "size": [2, 2],
  "attack": 180, "cooldown": 9.0,
  "basic":    [{ "do": "damage", "pct": 1.0 }, { "do": "gain_qi", "value": 15 }],
  "ultimate": { "cost": 40, "effects": [
      { "do": "damage", "pct": 1.0, "hits": 3 },
      { "do": "shield_break", "value": 120 } ] },
  "tier_scaling": { "attack": [1.0, 1.25, 1.45, 1.6] }
}
```
