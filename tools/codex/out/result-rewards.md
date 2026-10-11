Đã hoàn tất hiển thị phần thưởng trận đấu bằng hình ảnh.

Files changed:

- `Assets/_Game/Scripts/UI/Popups/ResultPopup.cs`
- `Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs`
- `Assets/_Game/Editor/Building/PopupPrefabBuilder.cs`
- `Assets/_Game/Tests/EditMode/UI/ResultPopupRewardTests.cs`

Layout mới:

- 3 sao ở hàng trên, gồm sao đạt/chưa đạt và hiệu ứng `star_burst`.
- Hàng card phần thưởng căn giữa, card vuông tối đa `84x84`.
- Card tự thu nhỏ và xuống hàng khi có hơn 6 phần thưởng.
- Icon giữ đúng tỷ lệ; Bạc, EXP và vật liệu dùng thư viện icon chung.
- Số lượng nằm dưới icon; tên phần thưởng hiện qua tooltip.
- Dòng lên cấp nằm dưới các card.
- Text cũ tiếp tục làm fallback cho PvP và lỗi nhận thưởng.
- Nút Home vẫn giữ tutorial target và hành vi cũ.

Kiểm tra:

- Style checker: `0 problem(s)` trên cả 4 file.
- `dotnet build One-Bag-Jianghu.sln --no-restore`: thành công, `0 errors`.
- Còn 2 cảnh báo obsolete có sẵn, không liên quan thay đổi này.
- Không mở Unity, không dùng git.

Trong Unity, chạy:

`One Bag Jianghu → Build → Popup Prefabs (Overwrite)`