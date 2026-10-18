Đã triển khai phần lớn client online:

- Mạng: tài khoản, token/resume, arena status/battle/leaderboard, DTO, dịch lỗi.
- UI runtime: `AccountPopup`, `ArenaPopup`, `LeaderboardRow`.
- Luồng Home, PvP server-only, cộng thưởng trước replay và kết quả PvP.
- Builder: menu `One Bag Jianghu/Build/Online Popup Prefabs`.
- Test EditMode dùng transport giả.

Tuy nhiên chưa thể bàn giao hoàn chỉnh vì công cụ filesystem đột ngột mất quyền ghi sau lượt kiểm tra. Các việc còn dang dở:

- Sửa 43 lỗi style, chủ yếu auto-property một dòng trong `OnlineModels.cs`.
- Cập nhật `Net/README.md`.
- Chạy lại lint sau khi sửa.
- `dotnet build` ngoài Unity chưa thấy file mới vì `.csproj` do Unity sinh chưa được refresh; chưa xác nhận compile cuối.
- Chưa chạy Unity/test theo đúng yêu cầu.
- Không phát hiện vị trí cần ảnh mới; builder dùng ảnh Kit có tỷ lệ tương ứng.

Giả định API: lỗi server có `payload.code` và `payload.message`; các payload còn lại đúng nguyên dạng trong `protocol.md`.