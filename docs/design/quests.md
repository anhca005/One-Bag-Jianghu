# Nhiệm vụ cốt truyện (Hành Tẩu Lục)

Chốt ngày 2026-10-11, quyết định #65.

## Luật chung
- Mỗi Hồi có **một chuỗi nhiệm vụ nối tiếp nhau**. Trong chuỗi:
  - **nhiệm vụ chính** đi theo cốt truyện: qua ải, đánh boss;
  - **việc phụ** dạy lối chơi, gắn vào lời của đồng minh: mua đồ, nâng Trọng, xếp Bao Phục, bái nhập Môn Phái…
- **Mỗi lúc chỉ có một nhiệm vụ đang làm.** Bấm **Nhận thưởng** xong thì nhiệm vụ kế tiếp mới mở. Hết chuỗi Hồi 1 thì nối sang Hồi 2.
- **Thưởng:** Bạc, EXP và nguyên liệu, nhận bằng tay. Nhiệm vụ cuối Hồi thưởng lớn hơn. Nguyên liệu vào Tàng Bảo Các. Nếu kho đầy thì không nhận được và báo "Tàng Bảo Các không đủ chỗ".
- **Xét tiến độ theo trạng thái:**
  - Mục tiêu đã đạt từ trước vẫn tính là xong ngay. Ví dụ ải đã qua, cấp đã đủ.
  - Riêng mục tiêu đếm hành động (mua, nâng Trọng, Hợp Thành…) chỉ tính từ lúc nhiệm vụ mở.
- **Giao diện:**
  - Ở Home có **khung theo dõi**: tên nhiệm vụ, tiến độ, nút Nhận khi xong. Bấm vào khung thì đi thẳng tới chỗ cần làm.
  - Có popup **Hành Tẩu Lục** liệt kê nhiệm vụ theo Hồi, mỗi nhiệm vụ có lời dẫn của người giao. Nhiệm vụ đã xong thì xem lại được, nhiệm vụ chưa mở thì hiện "???".

## Dữ liệu: `Resources/Data/quests.json`
```json
{
  "quests": [
    {
      "id": "q1_01",
      "chapter": 1,
      "kind": "main",
      "title": "Trấn Khẩu gặp nạn",
      "giver": "lao_tieu_phu",
      "text": "Thảo khấu chặn đường vào trấn. Bày kiếm giáp cho gọn rồi mở đường.",
      "objective": { "type": "stage_clear", "stage": "hoi_1_01" },
      "goto": "stage:hoi_1_01",
      "reward": { "silver": 80, "heroExp": 20, "materials": { "han_thiet": 1 } }
    }
  ]
}
```
- `kind` là `main` (chính) hoặc `side` (phụ). Thứ tự trong mảng là thứ tự của chuỗi.
- `giver` là speaker id trong [story-bible.md](story-bible.md).
- `text` là 1–2 câu, giọng của người giao.

### Kiểu mục tiêu (`objective.type`)
Nhãn tiến độ do game tự viết từ kiểu mục tiêu, không ghi trong JSON.

| type | Tham số | Đạt khi | Nhãn hiện ra |
|---|---|---|---|
| `stage_clear` | `stage` | Ải đã qua | Qua ải {tên ải} |
| `hero_level` | `level` | Cấp hiệp khách ≥ level | Đạt cấp {level} |
| `sect_joined` | – | Đã bái nhập Môn Phái | Bái nhập một Môn Phái |
| `tam_phap_level` | `level` | Tầng Tâm Pháp ≥ level | Tâm Pháp tầng {level} |

> **Chưa dùng `tam_phap_level`** cho tới khi có tính năng nâng Tâm Pháp. Hiện game không có cách nào nâng tầng Tâm Pháp, nên nhiệm vụ kiểu này sẽ không bao giờ xong và làm kẹt cả chuỗi.

| `equip_count` | `count` | Bao Phục có ≥ count món | Xếp {count} món vào Bao Phục |
| `item_tier` | `tier` | Có ít nhất một món ≥ tier Trọng | Có món đạt {tier} Trọng |
| `shop_buy` | `count` | Mua đủ count món ở Thương Hội kể từ khi nhận | Mua {count} món ở Thương Hội |
| `item_upgrade` | `count` | Nâng Trọng đủ count lần | Nâng Trọng {count} lần |
| `item_merge` | `count` | Hợp Thành đủ count lần | Hợp Thành {count} lần |
| `pve_win` | `count` | Thắng đủ count trận PvE (kể cả đánh lại) | Thắng {count} trận |

### `goto`
Bấm khung theo dõi thì đi tới chỗ cần làm:

| goto | Mở ra |
|---|---|
| `stage:<id>` | Bản đồ Hồi chứa ải đó |
| `shop` | Thương Hội |
| `storage` | Tàng Bảo Các |
| `bag` | Bao Phục |
| `sect` | Môn Phái |

### Cỡ thưởng tham khảo
Lần đầu qua một ải thường được 200 Bạc và 60 EXP.

| Loại nhiệm vụ | Bạc | EXP | Nguyên liệu |
|---|---|---|---|
| Việc phụ | 80–150 | 20–40 | 1–3 nguyên liệu bậc của Hồi |
| Nhiệm vụ chính | 120–250 | 30–60 | – |
| Nhiệm vụ boss cuối Hồi | 500–800 | 120–200 | Vài nguyên liệu, có thể kèm chất dẫn Đột Phá |

Bậc nguyên liệu theo Hồi:
- **Hồi 1–2:** bậc 1, tức Hàn Thiết, Thanh Đồng Ti, Hoàng Tuyên Chỉ, Chu Sa Thảo.
- **Hồi 3–4:** bậc 2.
- **Hồi 5:** bậc 3.

## Nội dung Hồi 1 (bản đầu, Gemini sẽ rà lại)

| id | Loại | Tên | Mục tiêu | goto | Thưởng |
|---|---|---|---|---|---|
| q1_01 | chính | Trấn Khẩu gặp nạn | Qua `hoi_1_01` | stage | 80 Bạc, 20 EXP, Hàn Thiết x1 |
| q1_02 | phụ | Sắm sửa hành trang | Mua 1 món ở Thương Hội | shop | 100 Bạc, 20 EXP, Thanh Đồng Ti x2 |
| q1_03 | chính | Sói hoang ngoài thôn | Qua `hoi_1_03` | stage | 120 Bạc, 30 EXP |
| q1_04 | phụ | Rèn thêm một Trọng | Nâng Trọng 1 lần | storage | 120 Bạc, 30 EXP, Hàn Thiết x3 |
| q1_05 | chính | Thử kiếm Phong Ba Đình | Qua `hoi_1_05` | stage | 200 Bạc, 50 EXP |
| q1_06 | phụ | Bày trận trong nang | Xếp 4 món vào Bao Phục | bag | 120 Bạc, 30 EXP, Chu Sa Thảo x2 |
| q1_07 | chính | Phá tiền doanh Ba Hổ | Qua `hoi_1_09` | stage | 220 Bạc, 50 EXP |
| q1_08 | chính | Nộ Ba Hổ | Qua `hoi_1_10` | stage | 600 Bạc, 150 EXP, Hàn Thiết x3, Thanh Đồng Ti x3, Binh Hồn Tinh Thạch x1 |
| q1_09 | phụ | Bái nhập sư môn | Bái nhập một Môn Phái | sect | 300 Bạc, 60 EXP |

## Hồi 2–5 (2026-10-11)
- Gemini soạn mỗi Hồi 9 nhiệm vụ, tổng cả 5 Hồi là 45 nhiệm vụ. Bảng chi tiết ở [reviews/2026-10-11-gemini-quests.md](../reviews/2026-10-11-gemini-quests.md).
- Claude kiểm tra: id ải và id nguyên liệu đều tồn tại, không trùng id nhiệm vụ.
- Claude đổi hai nhiệm vụ Tâm Pháp, vì chưa có cách nâng tầng Tâm Pháp:
  - `q3_02`: đổi thành Đạt cấp 20.
  - `q5_02`: đổi thành Đạt cấp 29.
- Tổng thưởng mỗi Hồi khoảng 1.900–2.500 Bạc và 440–610 EXP.
