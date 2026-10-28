Đã hoàn tất Task UI polish A và rà lại phần dang dở.

- Tách `VanBaoPhoItemCard` sang file MonoBehaviour riêng, kèm `.meta`.
- Vạn Bảo Phổ có lưới 73 vật phẩm, icon đầy đủ, tooltip chỉ số, trạng thái sở hữu, bộ lọc loại và bộ đếm `Đã có X/Y`.
- Hoàn thiện builder, menu Overwrite và `SceneSnapshot.CaptureAll`.
- Esc ở `ResultPopup` trở về đúng đích an toàn; Esc ở `IntroPopup` vẫn gửi `intro_seen` và trả nhạc Home.
- Tên người giao nhiệm vụ được đọc từ `Pve/chapters.json`; toàn bộ giver trong `quests.json` đều có tên Việt.
- Home dùng “Chiến Uy”; Hành Tẩu Lục có thông báo khi trống.
- Hoàn thiện trạng thái rỗng Bách Khoa, Cài đặt và nhãn popup chung.
- Bổ sung chống null cho hồ sơ chưa có danh sách vật phẩm.

Tự kiểm:

- Ba project kiểm tra tạm tương ứng Game Tests, PlayTests và Editor đều build thành công.
- Style checker: `0 problem(s)`.
- Không chạy Unity hoặc lệnh git theo quy định.
- `.csproj` gốc không bị sửa; các project kiểm tra tạm đã được xóa.

Builder cần chạy lại:

- `CommonUiPrefabBuilder`
- `QuestUiBuilder`
- `PopupPrefabBuilder`
- `VanBaoPhoPrefabBuilder`
- Builder Home/scene đang gọi các prefab trên

Không cần vẽ ảnh mới để chạy bản hiện tại; huy hiệu kết quả và nút Hành Tẩu Lục đang dùng tài nguyên sẵn có làm phương án hoàn thiện.