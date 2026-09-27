# Hệ thống chiến đấu

> Các con số trong file này đã qua **cân bằng đợt 2** ([balance.md](balance.md)) nhưng chưa chốt. Chúng nằm trong `BattleConfig` (hằng số chung) và `items.json` (từng món), đổi được mà không sửa logic. Khi chốt thì ghi vào [decisions.md](../decisions.md).
>
> Code: `server/battle-engine` ([hướng dẫn chạy](../../server/README.md)).

## Nguyên tắc
- **Hai bên đánh đồng thời theo thời gian thực**, không phải theo lượt. Mỗi món có thanh hồi chiêu riêng chạy song song; đầy thì ra đòn.
- Mô phỏng **tất định**: cùng đội hình + cùng seed thì luôn ra cùng kết quả. Random dùng SplitMix64 (dễ port sang C# nếu cần).
- Chạy trên **server Java**, xuất ra nhật ký sự kiện để client phát lại.
- Thời gian chia thành bước **100 ms** (tick) để tính toán. Mọi thời gian tính bằng mili giây (số nguyên), chỉ số dùng số nguyên (tỉ lệ theo phần nghìn ‰).

## Hằng số (BattleConfig)

| Hằng số | Giá trị đề xuất |
|---|---|
| Tick | 100 ms |
| Nội lực tối đa | 100 |
| Chi phí Hóa Kình | 60 (giảm được bằng `parry_discount`, tối thiểu 20) |
| Định Thân khi bị Hóa Kình | 1 giây |
| Liệt Hỏa bộc phát | đủ **50** điểm → **300** sát thương, Cương Khí đỡ được |
| Kịch Độc bộc phát | đủ **50** điểm → **240** sát thương, xuyên Cương Khí |
| Xuất Huyết bộc phát | đủ **50** điểm → **150 + 10% khí huyết tối đa**, xuyên Cương Khí |
| Điểm trạng thái tự giảm mỗi giây | 0 (không giảm; có thể bật riêng cho từng loại) |
| Kiệt Lực bắt đầu | 45 giây, giây thứ n mất n × 20 máu (bỏ qua Cương Khí) |
| Hết giờ (hòa) | 120 giây |
| Độ sâu chuỗi sự kiện tối đa | 8 |
| Hộ Thân tối đa | 80% |
| Hệ số theo Trọng 1→9 | 1.0 · 1.2 · 1.4 · 1.55 · 1.7 · 1.8 · 1.9 · 1.95 · 2.0 |

Hệ số Trọng nhân vào Lực Đạo, và vào `value`/`stacks` của hiệu ứng (khiên, hồi máu, số điểm trạng thái…). Không nhân vào thời gian, nội lực, chi phí.

## Vòng lặp mỗi tick

```
bắt đầu trận: kích hoạt các trigger BATTLE_START của hai bên

mỗi tick (now += 100ms):
  1. Cả hai bên cùng chạy thanh hồi chiêu:
       tốc độ = 0 nếu Định Thân / Điểm Huyệt, ngược lại 100% × (Khinh Công ? 2) × (Trì Trệ ? 0.5)
       tiến độ += 100ms × tốc độ; đầy → món đó "ra đòn trong tick này" (giữ phần dư)
  2. Mọi đòn ra trong tick này đều được thực hiện, xen kẽ A–B (bên dẫn đầu chọn bằng random có seed)
  3. Mỗi 1 giây: hồi Điều Tức; từ 45s thêm Kiệt Lực
  4. Kiểm tra tử trận (chỉ ở cuối tick)
```

- **Đồng thời thật sự:** đòn đã ra trong một tick luôn trúng, kể cả khi bên kia vừa bị hạ gục hoặc vừa bị Định Thân trong cùng tick. Hai bên cùng gục trong một tick → **hòa**.
- Thứ tự xen kẽ trong tick chỉ ảnh hưởng tài nguyên dùng chung (nội lực, Hóa Kình), và được chọn ngẫu nhiên có seed để không bên nào được ưu tiên cố định.
- Trạng thái có thời hạn kéo dài đến mốc T thì vẫn có hiệu lực ở tick kết thúc tại T (mỗi tick phủ khoảng (now−100, now]).
- Một vật phẩm ra đòn tối đa **1 lần mỗi tick**.
- Vận Khí đẩy tiến độ lên nhưng không kích hoạt ngay; món đó ra đòn ở tick kế tiếp.

## Trạng thái tích điểm (Liệt Hỏa, Kịch Độc, Xuất Huyết)
- Mỗi lần trúng, đối thủ **tích thêm điểm**. Điểm tích **không gây sát thương**.
- Khi đủ **50 điểm** → **bộc phát** một lượng sát thương lớn (xem bảng hằng số), rồi điểm **reset về 0**. Điểm dư vượt 50 bị bỏ.
- Mỗi loại tích riêng, có bản sắc riêng:
  - Liệt Hỏa: sát thương cao nhất nhưng Cương Khí đỡ được.
  - Kịch Độc: xuyên Cương Khí.
  - Xuất Huyết: xuyên Cương Khí, mạnh hơn khi đối thủ nhiều máu.
- Sát thương bộc phát không bị Hộ Thân giảm.
- Điểm tích chưa đủ khi trận kết thúc coi như mất, nên khi định giá vật phẩm phải tính hao hụt này ([balance.md](balance.md)).

## Kết thúc trận
- Khí huyết ≤ 0 ở cuối tick → thua. Hai bên cùng ≤ 0 → **hòa**.
- Quá 120 giây → hòa (hiếm khi xảy ra vì Kiệt Lực).

## Luật ra đòn
1. Nếu món có Tuyệt Kỹ và nội lực ≥ chi phí → trừ nội lực, tung **Tuyệt Kỹ**:
   - Nếu đối thủ đủ nội lực Hóa Kình (60, trừ `parry_discount`) → **Hóa Kình** (xem dưới), dừng.
   - Ngược lại chạy hiệu ứng Tuyệt Kỹ, rồi phát sự kiện `ULTIMATE`.
2. Ngược lại → **Phổ Công** (chạy danh sách `basic`, thường gồm sát thương + cộng nội lực).

Nội lực là **quỹ chung của Hiệp Khách**, mọi binh khí cùng tích và cùng tiêu.

## Hóa Kình
Khi đối thủ tung Tuyệt Kỹ mà bên phòng thủ có nội lực ≥ **chi phí Hóa Kình** (60, trừ `parry_discount`, tối thiểu 20):
- Bên phòng thủ tự động trừ chi phí đó.
- Tuyệt Kỹ bị hóa giải (không gây hiệu ứng), nội lực đối thủ đã tiêu vẫn mất.
- Đối thủ bị **Định Thân 1 giây**.
- Phát sự kiện `PARRY_SUCCESS` cho bên phòng thủ, `PARRIED` cho bên bị hóa giải.

> Đội không có Tuyệt Kỹ sẽ tích nội lực để Hóa Kình miễn phí, vì vậy binh khí không có Tuyệt Kỹ chỉ cho rất ít nội lực. Xem [balance.md](balance.md#quy-tắc-2-kinh-tế-nội-lực).

## Thứ tự tính sát thương (mỗi nhát)

```
sát thương gốc = Lực Đạo × pct   (hoặc value cố định nếu có)
→ Hội Tâm: tung xác suất theo từng nhát, trúng thì x2
→ Hộ Thân: giảm theo % của bên nhận
→ trừ vào Cương Khí (trừ khi ignoreShield)
→ phần còn lại trừ vào Khí Huyết
```
- **Phá Cương** là hiệu ứng riêng (`shield_break`), đặt trước `damage` trong danh sách để phá khiên trước khi chém.
- Bộc phát trạng thái và Kiệt Lực không bị Hộ Thân giảm.

## Hệ thống sự kiện (trigger)

| Sự kiện | Khi nào | Bên nhận |
|---|---|---|
| `battle_start` | Đầu trận | Mỗi bên |
| `hit` | Một nhát `damage` của bên mình trúng | Bên ra đòn |
| `crit` | Nhát đó Hội Tâm | Bên ra đòn |
| `ultimate` | Tuyệt Kỹ của bên mình thành công | Bên ra đòn |
| `parry_success` | Bên mình Hóa Kình thành công | Bên phòng thủ |
| `parried` | Tuyệt Kỹ của bên mình bị hóa giải | Bên bị hóa giải |
| `hp_below` | Khí huyết bên mình giảm và đang dưới ngưỡng `hpBelow` (còn sống) | Bên bị đánh |
| `status_applied` | Bên mình gây một trạng thái (lọc theo `status` nếu có) | Bên gây |
| `burst` | Trạng thái do bên mình tích đã bộc phát (lọc theo `status` nếu có) | Bên gây |

- `from`: `any` (mặc định) / `self` (chính món này gây ra) / `adjacent` (món liền kề gây ra). Chỉ có ý nghĩa với sự kiện do một món cụ thể gây ra.
- `maxUses`: số lần tối đa mỗi trận (0 = không giới hạn). Đan Dược thường là 1.
- **Chống vòng lặp vô hạn:** trigger gây ra trigger khác tăng độ sâu; quá 8 tầng thì dừng.

## Nhật ký sự kiện (gửi cho client)

Mỗi sự kiện có `t` (ms), `type`, `side` (bên bị tác động, hoặc bên hành động với `TRIGGER`), và trạng thái sau sự kiện của bên đó (`hp`, `maxHp`, `shield`, `qi`) khi liên quan. `item` là uid vật phẩm = `side × 100 + thứ tự trong túi`; bảng `items` trong kết quả map uid → id.

```json
{ "t": 6000, "type": "TRIGGER", "side": 1, "item": 100, "kind": "ULTIMATE", "hp": 1200, "maxHp": 1200, "shield": 224, "qi": 0 }
{ "t": 6000, "type": "DAMAGE", "side": 0, "item": 100, "value": 168, "absorbed": 168, "crit": false, "hp": 1200, "maxHp": 1200, "shield": 432, "qi": 24 }
{ "t": 6000, "type": "STATUS", "side": 0, "kind": "BURN", "value": 15, "total": 30, "hp": 1200, "maxHp": 1200, "shield": 432, "qi": 24 }
{ "t": 20000, "type": "BURST", "side": 0, "item": 101, "kind": "BURN", "value": 300, "absorbed": 0, "hp": 0, "maxHp": 1200, "shield": 0, "qi": 44 }
{ "t": 23500, "type": "PARRY", "side": 1, "item": 0, "value": 1000, "hp": 576, "maxHp": 1200, "shield": 0, "qi": 10 }
{ "t": 30000, "type": "END", "side": 1 }
```

Với `STATUS` của trạng thái tích điểm: `value` là số điểm vừa thêm, `total` là điểm sau khi thêm (khi bộc phát thì `total` = 50, tiếp theo là một `STATUS` với `total` = 0). Client dùng `total` để vẽ thanh tích điểm.

Các loại: `START, TRIGGER (kind BASIC/ULTIMATE/PASSIVE), DAMAGE, BURST (kind BURN/POISON/BLEED), DOT (kind EXHAUSTION), HEAL, SHIELD, QI, STATUS, PARRY, CHARGE, BUFF, MAX_HP, DMG_REDUCE, PARRY_COST, END (side = bên thắng, -1 = hòa)`.

## Chưa làm
- Xoay vật phẩm trong túi.
- Côn "cắt ngang Tuyệt Kỹ", Cung "phá Đan Dược", bộ môn phái (hiệu ứng khi đủ 2/4/6 món cùng phái).
- Kỹ năng bị động của Hiệp Khách.
