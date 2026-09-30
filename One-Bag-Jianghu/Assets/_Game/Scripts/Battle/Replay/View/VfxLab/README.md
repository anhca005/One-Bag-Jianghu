# VFX Lab

1. Trong Unity, chọn **One Bag Jianghu → Create VFX Lab Scene**.
2. Mở `Assets/_Game/Scenes/VfxLab.unity`, rồi bấm Play.
3. Chọn một dòng bên trái để xem và chỉnh hiệu ứng đó.

Chỉnh hiệu ứng cho phía bên **TRÁI**; khi bên phải tấn công, battle tự lật toàn bộ kết quả theo chiều ngang.

- Kéo Rotation, Scale, Offset X/Y hoặc bật Flip X/Y; thay đổi hiện ngay và tự lặp.
- **Reset** đưa hiệu ứng đang chọn về mặc định.
- **Save** ghi dữ liệu xuống asset; `● chưa lưu` báo có thay đổi chưa ghi, `Đã lưu` báo thành công.
- Trong Editor, thay đổi ScriptableObject ở Play mode vẫn tồn tại; Save ép Unity ghi ngay xuống đĩa.
- Dữ liệu nằm tại `Assets/_Game/Resources/VfxTuning.asset`; menu **Create VFX Tuning Asset** không ghi đè asset cũ.

Phím tắt:

- **Space** phát; **←/→** đổi hiệu ứng; **L** bật/tắt loop.
- **Q/E** xoay −5°/+5°; giữ **Shift** để xoay −1°/+1°.
- **Z/X** giảm/tăng scale 0,05.
- **W/A/S/D** dịch lên/trái/xuống/phải 5 px.

Các nút Side, tốc độ, Hit after và BG giúp kiểm tra trong nhiều tình huống.
`✓` là có sprite, `✗` là fallback; `✎` là hiệu ứng có tuning khác mặc định.
Fallback không nhận tuning. Inspector dùng đúng kích thước battle và hiển thị rotation/scale/flip đã chỉnh.
