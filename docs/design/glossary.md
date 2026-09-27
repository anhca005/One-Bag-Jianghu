# Bảng thuật ngữ

Đây là chuẩn đặt tên trong game. Cột **Mã** dùng làm tên trong code và dữ liệu.

## Hệ thống và giao diện

| Tên trong game | Mã | Mô tả |
|---|---|---|
| Bao Phục | `bag` | Túi đồ dạng lưới ô |
| Tàng Bảo Các | `storage` | Kho chứa đồ chưa dùng |
| Thương Hội | `shop` | Cửa hàng, làm mới hàng bằng bạc |
| Vạn Bảo Phổ | `codex` | Bộ sưu tập |
| Chiến Uy | `power` | Điểm sức mạnh tổng |
| Hiệp Khách | `hero` | Nhân vật, thuộc một môn phái |
| Thể Lực | `stamina` | Giới hạn lượt chơi |
| Hành Tẩu Giang Hồ | `pve` | Chế độ đi ải; mỗi chương gọi là **Hồi** (`chapter`) |
| Luận Kiếm Đài | `arena` | PvP xếp hạng; mỗi mùa là **Kỳ Luận Kiếm** (`season`) |
| Trọng | `tier` | Cấp cường hóa, Nhất Trọng → Cửu Trọng |

## Loại vật phẩm

| Tên | Mã | Vai trò |
|---|---|---|
| Binh Khí | `weapon` | Gây sát thương, tự kích hoạt |
| Hộ Cụ | `armor` | Cương khí, giảm sát thương |
| Tâm Pháp | `manual` | Bị động, kích hoạt khi vào trận hoặc theo sự kiện |
| Đan Dược | `pill` | Dùng một lần khi thỏa điều kiện |
| Kỳ Trân | `relic` | Hỗ trợ, tác động ô liền kề |

## Độ hiếm

| Tên | Mã | Màu khung |
|---|---|---|
| Phàm Phẩm | `common` | Xám |
| Lương Phẩm | `uncommon` | Lục |
| Tinh Phẩm | `rare` | Lam |
| Tuyệt Phẩm | `epic` | Tím |
| Thần Binh | `legendary` | Vàng kim |
| Chí Tôn | `supreme` | Đỏ (chỉ có ở Luận Kiếm Đài) |

## Chỉ số và cơ chế

| Tên | Mã | Mô tả |
|---|---|---|
| Khí Huyết | `hp` | Máu |
| Nội Lực | `qi` | Tài nguyên chiến đấu, tối đa 100 |
| Phổ Công | `basic` | Đòn thường, thường cộng nội lực |
| Tuyệt Kỹ | `ultimate` | Đòn mạnh, tiêu hao nội lực |
| Hóa Kình | `parry` | Tự tiêu 60 nội lực (giảm được bằng vật phẩm) để hóa giải Tuyệt Kỹ của đối thủ, đối thủ bị Định Thân 1 giây |
| Tốc Xuất Chiêu | `cooldown` | Thời gian giữa hai lần kích hoạt |
| Lực Đạo | `attack` | Sát thương gốc của binh khí |
| Cương Khí | `shield` | Lớp máu ảo, bị trừ trước khí huyết |
| Hộ Thân | `dmg_reduce` | Giảm % sát thương nhận vào |
| Hội Tâm | `crit` | Chí mạng, x2 sát thương |
| Phá Cương | `shield_break` | Phá thêm cương khí, cộng dồn |
| Liên Kích | `multi_hit` | Kích hoạt nhiều lần |

## Trạng thái

| Tên | Mã | Hiệu ứng |
|---|---|---|
| Liệt Hỏa | `burn` | Tích điểm; đủ 50 thì bộc phát sát thương lớn (Cương Khí đỡ được) rồi reset |
| Kịch Độc | `poison` | Tích điểm; đủ 50 thì bộc phát xuyên Cương Khí rồi reset |
| Xuất Huyết | `bleed` | Tích điểm; đủ 50 thì bộc phát xuyên Cương Khí + % khí huyết tối đa rồi reset |
| Nội Thương | `internal_injury` | Giảm khí huyết tối đa |
| Trì Trệ | `slow` | Tốc xuất chiêu còn 50% |
| Điểm Huyệt | `seal` | Dừng hẳn thời gian hồi của một món |
| Định Thân | `stun` | Dừng mọi món |
| Khinh Công | `haste` | Tốc xuất chiêu x2 |
| Vận Khí | `charge` | Đẩy thanh hồi chiêu lên ngay |
| Bộc Phát | `burst` | Trạng thái tích điểm đạt ngưỡng, gây sát thương một lần rồi reset |
| Điều Tức | `regen` | Hồi máu mỗi giây |
| Liệu Thương | `heal` | Hồi máu một lần |
| Kiệt Lực | `exhaustion` | Trận quá 45 giây thì cả hai bên mất máu tăng dần |
