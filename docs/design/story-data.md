# Dữ liệu cutscene cốt truyện

Cutscene nằm tại `Assets/_Game/Resources/Data/story.json`. Mỗi phần tử trong `chapters` có:

- `chapter`: số hồi; `0` là mở đầu;
- `title`: tiêu đề hiện trên title card;
- `panels`: danh sách `{ "image": "story_<chapter>_<n>", "text": "..." }` theo thứ tự phát.

Ảnh đặt trong `Assets/_Game/Textures2D/Pve/` và được đăng ký vào `PveArtLibrary`. Khi ảnh chưa có, player
dùng `map_hoi_<chapter>`, sau đó `intro_1` làm ảnh dự phòng. Có thể thay riêng `text` và file ảnh mà không cần
đổi code hoặc prefab.
