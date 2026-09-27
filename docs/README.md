# Tài liệu dự án: One Bag Jianghu (Nhất Nang Giang Hồ)

## Mục lục

| Thư mục / file | Nội dung |
|---|---|
| [roadmap.md](roadmap.md) | Lộ trình theo giai đoạn và trạng thái hiện tại |
| [decisions.md](decisions.md) | Nhật ký các quyết định đã chốt (kèm lý do) |
| **design/** | |
| [design/gdd.md](design/gdd.md) | Tổng quan game: ý tưởng, vòng lặp, chế độ chơi, tiến trình, PvP |
| [design/glossary.md](design/glossary.md) | Bảng thuật ngữ võ hiệp (chuẩn dùng trong game và code) |
| [design/combat.md](design/combat.md) | Hệ thống chiến đấu: tick, nội lực, tuyệt kỹ, trạng thái, công thức |
| [design/items.md](design/items.md) | Loại vật phẩm, độ hiếm, hệ binh khí, môn phái, vật phẩm mẫu |
| **tech/** | |
| [tech/architecture.md](tech/architecture.md) | Kiến trúc Unity + Java WebSocket, giao thức, cấu trúc thư mục |
| **art/** | |
| [art/art-pipeline.md](art/art-pipeline.md) | Quy trình tạo hình bằng ChatGPT, quy ước kích thước, prompt gốc |
| **worklog/** | Lịch sử làm việc theo ngày ([mẫu](worklog/_template.md)) |

## Tóm tắt nhanh

- **Thể loại:** auto-battler xếp túi đồ (lưới ô), võ hiệp, 2D hình tĩnh.
- **Nền tảng:** PC. **Client:** Unity 6. **Server:** Java + WebSocket. **Hình ảnh:** ChatGPT.
- **Chế độ:** PvE *Hành Tẩu Giang Hồ* (đi ải theo Hồi) và PvP *Luận Kiếm Đài* (xếp hạng theo mùa).
- **Vật phẩm giữ lâu dài** (phương án B): đồ kiếm ở PvE mang vào PvP.
