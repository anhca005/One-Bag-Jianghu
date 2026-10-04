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
| [design/balance.md](design/balance.md) | Quy tắc cân bằng (ngân sách theo ô, kinh tế nội lực), cách đo, kết quả các đợt |
| `tools/sheets/export_xlsx.py` | Xuất dữ liệu game ra Excel nhiều tab để nhập vào Google Sheets (`exports/`) |
| [../tools/balance/](../tools/balance/README.md) | Công cụ cân bằng: bảng đối đầu theo Trọng/Tâm Pháp, thử thay đổi số (`whatif`), tự dò (`tune`) |
| **tech/** | |
| [tech/architecture.md](tech/architecture.md) | Kiến trúc Unity + Java WebSocket, giao thức, cấu trúc thư mục |
| **art/** | |
| [art/art-pipeline.md](art/art-pipeline.md) | Quy trình tạo hình bằng ChatGPT, quy ước kích thước, prompt gốc |
| [art/style-options.md](art/style-options.md) | 7 phong cách đã thử (đã chốt Thủy Mặc), giữ để tham khảo |
| [art/prompts-items-mvp.md](art/prompts-items-mvp.md) | Prompt ChatGPT vẽ icon vật phẩm MVP (đã xong 14/14) |
| [tech/protocol.md](tech/protocol.md) | Giao thức WebSocket + JSON giữa Unity và server (MVP) |
| [tech/ui-flow.md](tech/ui-flow.md) | Luồng màn hình Login → Loading → Home → Gameplay, kiến trúc UI, độ phân giải |
| [art/prompts-ui.md](art/prompts-ui.md) | Prompt ChatGPT vẽ nền màn hình và khung UI |
| [art/prompts-attack-vfx.md](art/prompts-attack-vfx.md) | Prompt ChatGPT vẽ 25 hình tấn công / bộc phát (quy ước tên file cho code) |
| [art/reference/](art/reference/) | Hình tham chiếu đã duyệt, đính kèm mỗi lần tạo hình |
| **worklog/** | Lịch sử làm việc theo ngày ([mẫu](worklog/_template.md)) |

## Tóm tắt nhanh

- **Thể loại:** auto-battler xếp túi đồ (lưới ô), võ hiệp, 2D hình tĩnh.
- **Nền tảng:** PC. **Client:** Unity 6. **Server:** Java + WebSocket. **Hình ảnh:** ChatGPT.
- **Chế độ:** PvE *Hành Tẩu Giang Hồ* (đi ải theo Hồi) và PvP *Luận Kiếm Đài* (xếp hạng theo mùa).
- **Vật phẩm giữ lâu dài** (phương án B): đồ kiếm ở PvE mang vào PvP.
