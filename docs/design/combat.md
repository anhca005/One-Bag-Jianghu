# Hệ thống chiến đấu

> Các con số trong file này là **đề xuất ban đầu**, chưa chốt. Khi chốt thì ghi vào [decisions.md](../decisions.md).

## Nguyên tắc
- Mô phỏng **tất định**: cùng đội hình + cùng seed thì luôn ra cùng kết quả.
- Chạy trên **server Java**, xuất ra nhật ký sự kiện để client phát lại.
- Tick cố định: **10 tick/giây** (100 ms).

## Vòng lặp mỗi tick

```
mỗi tick:
  với mỗi vật phẩm có Tốc Xuất Chiêu:
    hệ số = 1 × (Khinh Công ? 2 : 1) × (Trì Trệ ? 0.5 : 1) × (Điểm Huyệt/Định Thân ? 0 : 1)
    tiến độ += dt × hệ số
    nếu tiến độ >= Tốc Xuất Chiêu → kích hoạt(), tiến độ = 0
  mỗi 1 giây: xử lý Liệt Hỏa, Kịch Độc, Điều Tức
  sau 45 giây: Kiệt Lực — cả hai bên mất máu tăng dần
```

## Luật binh khí
- Nếu nội lực ≥ chi phí Tuyệt Kỹ của món → tung **Tuyệt Kỹ**, trừ nội lực.
- Nếu không → **Phổ Công**, cộng nội lực theo chỉ số của món.
- Nội lực tối đa **100**.

## Hóa Kình
Khi đối thủ tung Tuyệt Kỹ mà bên phòng thủ có nội lực ≥ **60**:
- Tự động trừ 60 nội lực.
- Tuyệt Kỹ bị hóa giải (không gây hiệu ứng), nội lực đối thủ đã tiêu vẫn mất.
- Đối thủ bị **Định Thân 2 giây**.

## Thứ tự tính sát thương

```
sát thương gốc (Lực Đạo × hệ số chiêu)
→ Hội Tâm (x2 nếu trúng)
→ Hộ Thân (giảm %)
→ Phá Cương (phá thêm cương khí)
→ trừ vào Cương Khí
→ phần còn lại trừ vào Khí Huyết
```
Kịch Độc bỏ qua Cương Khí. Liệt Hỏa và Kịch Độc không bị giảm bởi Hộ Thân.

## Hệ thống sự kiện (event bus)
Mỗi vật phẩm đăng ký nghe sự kiện, ví dụ:
`onBattleStart`, `onTrigger`, `onHit`, `onCrit`, `onUltimate`, `onParry`, `onParried`, `onStatusApplied`, `onHpBelow`, `onAdjacentTrigger`.

**Chống vòng lặp vô hạn:** giới hạn độ sâu chuỗi sự kiện (ví dụ tối đa 8 tầng trong một tick).

## Nhật ký sự kiện (gửi cho client)

```json
{ "t": 1200, "type": "TRIGGER", "side": 0, "item": "u17", "skill": "ULTIMATE" }
{ "t": 1200, "type": "DAMAGE", "target": 1, "amount": 540, "crit": true, "shieldHit": 200 }
{ "t": 1200, "type": "STATUS", "target": 1, "status": "burn", "stacks": 30 }
```
