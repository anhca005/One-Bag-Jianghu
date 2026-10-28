Đã hoàn thành phần client hàng chờ ghép trận:

- Thêm `FindArenaMatch`, `CancelArenaSearch` và xử lý push `replyTo: 0`.
- Thêm model `ArenaSearch`, `searchId`.
- Cập nhật giao diện thời gian chờ, khoảng điểm, thời gian nới và nút Huỷ.
- Tự huỷ khi đóng sảnh; xử lý mất kết nối.
- Thêm 3 test transport giả theo yêu cầu.
- Style lint: 0 lỗi trên 5 file C#.
- Không chạy Unity theo yêu cầu.

Báo cáo đã ghi tại [-o](D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu\-o).Đã làm phần client hàng chờ ghép trận.

File sửa:
- Assets/_Game/Scripts/Net/OnlineModels.cs
- Assets/_Game/Scripts/Net/NetworkService.cs
- Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs
- Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs
- Assets/_Game/Tests/EditMode/Net/OnlineNetTests.cs

NetworkService đăng ký callback trước arena_find, xử lý chung phản hồi tức thời và tin chủ động
arena_searching/arena_result có replyTo: 0, rồi dọn lượt tìm khi có kết quả, huỷ hoặc mất kết nối.

Đã thêm test transport giả cho ghép ngay, searching rồi push result, và arena_cancel. Không chạy Unity theo yêu cầu.
Style lint: 0 lỗi trên cả 5 file C# đã sửa.
