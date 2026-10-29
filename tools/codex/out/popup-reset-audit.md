# Rà soát reset trạng thái popup/panel

Ngày rà soát: 2026-10-29

Phạm vi: toàn bộ lớp có tên `Popup`/`Panel` dưới
`One-Bag-Jianghu/Assets/_Game/Scripts/UI/`, cùng hạ tầng pooling liên quan.

> Lưu ý: sandbox cho phép tạo báo cáo này nhưng từ chối mọi lần `apply_patch` vào các file `.cs`
> dưới Unity project. Vì vậy các lỗi dưới đây đã được xác định nhưng bốn bản sửa dự kiến chưa thể ghi vào mã nguồn.

| Popup/panel đã kiểm tra | Trạng thái thay đổi trong lúc dùng | Lỗi khi mở lại? | Thay đổi / kết luận (file:dòng) |
|---|---|---|---|
| `AccountPopup` | Ba nút bị khóa và nhãn xác nhận thành “Đang kết nối…” trong khi gửi đăng nhập/đăng ký | Có | Cần `ResetSubmitState()` gọi sau `ShowLogin()` trong `OnSetup`; hàm gọi `SetBusy(false)`. `Popups/AccountPopup.cs:54`, `:160` |
| `BagPopup` | `isSaving`, nút Đóng bị khóa, nhãn “Đang lưu…” | Đã sửa trước audit | `ResetCloseButton()` đã được gọi từ `OnSetup`; không đổi. `Popups/BagPopup.cs:27`, `:93` |
| `ConfirmPopup` | Callback và nhãn hai nút thay theo dữ liệu | Không | `OnSetup` gán lại toàn bộ; `OnHidden` xử lý callback chưa trả lời. `Popups/ConfirmPopup.cs:64` |
| `MessagePopup` | Tiêu đề, nội dung, nhãn nút | Không | Được bind lại hoàn toàn trong `OnSetup`. `Popups/MessagePopup.cs:44` |
| `ResultPopup` | Nhãn, callback và trạng thái nút chơi lại | Không | Được bind lại từ `ResultPopupData`, gồm `interactable`. `Popups/ResultPopup.cs:231` |
| `SettingsPopup` | Dropdown và nút tài khoản thay theo thiết lập/profile | Không | `OnSetup` nạp lại settings và recompute trạng thái có chủ đích. `Popups/SettingsPopup.cs:53`, `:110`, `:263` |
| `SectSelectPopup` | `actionPending`; nút chính, đóng và các thẻ môn phái bị khóa | Có | Cần `ResetActionState()` gọi đầu `OnSetup`, dùng `SetActionPending(false)` trước khi bind lại điều kiện gia nhập. `Popups/SectSelectPopup.cs:52`, `:247` |
| `ShopPopup` | `actionPending`; nút đóng/tab/dropdown/thẻ hàng bị khóa trong request | Có | Cần `ResetActionState()` gọi sau `EnsureInitialized()` trong `OnSetup`, dùng `SetActionPending(false)` rồi `Refresh()` recompute tiền/đã mua. `Popups/ShopPopup.cs:131`, `:471` |
| `StoragePopup` | `actionPending`; nút và filter bị khóa; còn giữ `selectedUid`, `selectedStack`, `mergeSourceUid` | Có | Cần `ResetActionState()` trong `OnSetup`: xóa pending/lựa chọn/chế độ Hợp Thành, mở lại Sort/Đóng/filter, gọi `ClearSelection()`; `Refresh()` sau đó recompute action. `Popups/StoragePopup.cs:97`, `:617`, `:643` |
| `QuestLogPopup` | `claimPending`, các nút nhận thưởng bị khóa | Không ở vòng đời bình thường | `OnHidden` đã reset `claimPending`; `OnShown` dựng lại danh sách và trạng thái nút. `Popups/QuestLogPopup.cs:31`, `:44` |
| `VanBaoPhoPopup` | Filter/tab và danh sách thẻ | Không | `OnSetup` bind lại danh mục, sở hữu và filter; không có busy state. `Popups/VanBaoPhoPopup.cs:38` |
| `DevCheatPanel` | Nội dung debug/profile | Không | Cả bản development và stub non-development đều dựng lại từ `OnSetup`; không có request/pending. `Popups/DevCheatPanel.cs:89`, `:179` |
| `ArenaLobbyPopup` | Matchmaking, `awaitingSearch`, `activeSearch`, `pendingResult`, lớp matching/versus, coroutine tự vào trận | Không thấy trạng thái kẹt trực tiếp | `OnSetup` ẩn hai lớp và lấy lại pending result từ session; `OnHidden` gọi `CancelMatch`. Trạng thái nút tìm được recompute theo network. `Arena/ArenaLobbyPopup.cs:105`, `:154`, `:379` |
| `DialoguePopup` | Chỉ số câu, `complete`, coroutine gõ chữ | Không | `OnSetup` reset index/nội dung/complete; coroutine Unity dừng khi object bị disable và `ShowLine` thay thế handle cũ. `Pve/DialoguePopup.cs:51`, `:71`, `:108` |
| `StageMapPopup` | `leavingForBattle`, nút Khiêu Chiến bị khóa khi gửi start request | Không thấy trạng thái kẹt trực tiếp | `OnSetup` reset `leavingForBattle`; `DrawChapter`/`SelectStage` recompute nút theo mở khóa, thể lực và đội hình. `Pve/StageMapPopup.cs:77`, `:309`, `:358` |
| `LoadingPanel` | Text/progress loading | Không thuộc pool popup | Panel sống qua scene và mọi lần hiển thị nhận lại nội dung/progress; không có busy button. `Loading/LoadingPanel.cs:7` |
| `BagLoadoutPanel` | Profile bind, selection và các view vật phẩm | Không độc lập; được popup bind lại | `BagPopup.OnSetup` gọi `Bind` mỗi lần mở; trạng thái view được dựng lại theo profile. `Common/BagLoadoutPanel.cs:19`, `Popups/BagPopup.cs:34` |
| `BasePopup` / `BasePopup<TData>` | Animation coroutine, `isClosing`, CanvasGroup | Không | `Show` reset `isClosing`; `HideImmediate` dừng animation và vô hiệu CanvasGroup; đây là hạ tầng pooling. `Popups/BasePopup.cs:84`, `:120` |
| `PopupManager` / `PopupStack` | Pool và stack popup đang mở | Không | Manager luôn gọi `Setup` trước `Show`; stack được cập nhật khi popup đóng. `Popups/PopupManager.cs`, `Popups/PopupStack.cs` |

## Các trường hợp đáng ngờ chưa đổi

- `AccountPopup`: callback của request cũ có thể đến sau khi popup đã đóng rồi mở lại; success có thể đóng
  phiên popup mới, error có thể ghi lỗi/đổi nút của phiên mới (`AccountPopup.cs:143-157`). Nên thêm token theo
  mỗi lần `OnSetup` nếu tầng network không hủy callback khi đóng.
- `SectSelectPopup`: callback `ChooseSect` cũ có thể gọi `CloseWithResult` hoặc mở lỗi trên phiên mới
  (`SectSelectPopup.cs:235-244`).
- `ShopPopup` và `StoragePopup`: Escape/backdrop vẫn có thể đóng popup trong lúc nút Đóng bị khóa. Callback
  cũ đến sau lần mở mới có thể xóa `actionPending` của request mới hoặc refresh UI không đúng phiên
  (`ShopPopup.cs:442-469`; `StoragePopup.cs:552-588`).
- `QuestLogPopup`: callback nhận thưởng cũ gọi `HandleClaimFinished()` không kiểm tra `IsShowing`; nếu trùng
  đúng một request mới, nó có thể xóa `claimPending` sớm (`QuestLogPopup.cs:103-151`).
- `ArenaLobbyPopup`: ba callback tải status/rank/history và callback replay không mang generation của lần mở;
  response cũ có thể bind vào lần mở mới (`ArenaLobbyPopup.cs:192-204`, `:452-462`).
- `StageMapPopup`: response start PvE giữ tham chiếu popup và có thể đóng cùng instance sau khi người chơi đã
  đóng/mở lại bản đồ (`StageMapPopup.cs:358-381`, `:478-503`).

Các trường hợp trên cần một quy ước chung (generation/token cho mỗi lần `Setup`, hoặc hủy request ở tầng
network). Không nên vá riêng bằng kiểm tra `IsShowing`, vì callback cũ vẫn có thể đến lúc cùng instance đang
hiển thị cho một phiên mới.

## Tự kiểm tra

- Không chạy Unity, scene builder hay prefab builder.
- Không chạy lệnh git.
- Chưa thể chạy style checker cho bản sửa vì sandbox không cho ghi bốn file `.cs`; mã nguồn hiện chưa thay đổi.
