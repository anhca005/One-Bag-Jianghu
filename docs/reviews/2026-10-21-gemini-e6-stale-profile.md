# Báo Cáo Rà Soát Client Giữ Hồ Sơ Cũ Sau Khi Server Gửi Hồ Sơ Mới (E6)

- **Ngày lập:** 2026-10-21
- **Người thực hiện:** Gemini
- **Mục tiêu:** Rà soát toàn bộ codebase client (`One-Bag-Jianghu/Assets/_Game/Scripts/**`) để phát hiện các vị trí giữ tham chiếu `PlayerProfile` cũ sau khi `GameSession.ApplyServerProfile` tạo object mới, các vị trí còn tự ý sửa hồ sơ ở client, các màn hình UI không vẽ lại khi nhận hồ sơ mới, và các thao tác mạng không khoá nút / không xử lý lỗi `rejected`.
- **Nguyên tắc bối cảnh (E6 - #70, #71):**
  - Mọi phản hồi thành công `*_ok` từ server gửi kèm toàn bộ hồ sơ (`profile`) và revision mới.
  - `GameSession.ApplyServerProfile` (`One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs:194-206`) phân giải JSON thành **một object `PlayerProfile` hoàn toàn mới**, gán vào `GameSession.Profile`, sau đó kích hoạt `PlayerProfile.NotifyProgressChanged()`.
  - Mọi thành phần giữ tham chiếu tới object cũ hoặc không lắng nghe `ProgressChanged` đều có nguy cơ đọc/ghi dữ liệu rác, gây sai lệch hiển thị hoặc lỗi luồng nghiệp vụ.

---

## 1. Bảng Tổng Hợp Vấn Đề (Sắp Xếp Mức Độ Cao Lên Đầu)

| File:Dòng | Vấn đề | Mức độ | Cách sửa gợi ý |
|---|---|---|---|
| [`ShopPopup.cs:344-345`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L344-L345) | Trong `ConfirmBuy`, lệnh kiểm tra `Slots[index].Bought` chạy đồng bộ ngay sau khi gọi `BuyShopSlot` bất đồng bộ, trước khi server trả về `shop_buy_ok`. Tại thời điểm này `Bought` vẫn bằng `false` trên hồ sơ cũ, khiến `OnboardingRuntime.Publish("shop_item_purchased")` không bao giờ được gọi, làm kẹt hoàn toàn chuỗi hướng dẫn tân thủ Thương Hội. | **Cao** | Chuyển lệnh kiểm tra `Bought` và gọi `OnboardingRuntime.Publish` vào trong callback `onOk` của `BuyShopSlot` (sau khi `ApplyProfilePayload` đã cập nhật hồ sơ mới từ server). |
| [`QuestLogPopup.cs:98-100, 114-118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L98-L100) | `Claim()` gọi `Refresh()` ngay khi `ClaimActive` vừa phát request mạng, lúc này hồ sơ chưa cập nhật nên vẽ lại trạng thái chưa nhận thưởng. Khi server phản hồi `quest_claim_ok`, `QuestLogPopup` không nghe `ProgressChanged` và `HandleClaimed` không gọi `Refresh()`, khiến UI giữ nguyên trạng thái cũ. Nút `ClaimButton` cũng không bị khoá trong lúc chờ mạng, cho phép người chơi spam nhận thưởng liên tục. | **Cao** | Vô hiệu hoá `ClaimButton` ngay khi bấm nhận thưởng; bỏ `Refresh()` sớm ở dòng 99 và gọi `Refresh()` bên trong callback thành công `onOk` (hoặc đăng ký nghe `ProgressChanged`). |
| [`QuestService.cs:101-103`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L101-L103) | `QuestService.Claim(PlayerProfile profile)` được `QuestLogPopup.ClaimActive` gọi với bản sao thử nghiệm `preview`, nhưng bên trong hàm lại phát các sự kiện toàn cục tĩnh `PlayerProfile.NotifyProgressChanged()`, `Changed?.Invoke()` và `OnboardingRuntime.Publish("quest_claimed")`. Điều này khiến UI bên ngoài (như `QuestTracker`) bị kích hoạt vẽ lại sớm trên hồ sơ thật chưa đổi, và tutorial nhận thưởng được đánh dấu hoàn thành trước khi server phê duyệt. | **Cao** | Tách riêng hàm kiểm tra/xem trước (preflight) thuần tuý không phát sự kiện tĩnh; chỉ kích hoạt `NotifyProgressChanged` và `OnboardingRuntime.Publish` sau khi server phản hồi `quest_claim_ok`. |
| [`TutorialService.cs:9, 31-36, 90, 108`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L9) & [`OnboardingRuntime.cs:24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/OnboardingRuntime.cs#L24) | `TutorialService` nhận `PlayerProfile` trong constructor và lưu vĩnh viễn vào trường `private readonly PlayerProfile profile;`. `OnboardingRuntime.Start` khởi tạo service này một lần duy nhất. Khi server gửi hồ sơ mới, `IsCompleted` vẫn đọc `profile.CompletedTutorials` từ object cũ; `Reset()` xoá dữ liệu trên object cũ; `OnboardingRuntime` không nghe `ProgressChanged` nên không kích hoạt lại các chuỗi hướng dẫn khi đủ điều kiện từ server (qua ải 1, đạt cấp 10). | **Cao** | Bỏ trường `profile` trong `TutorialService`, truy xuất động qua `GameBootstrap.Session.Profile`; cho `OnboardingRuntime` đăng ký nghe `ProgressChanged` để đánh giá lại các chuỗi hướng dẫn đủ điều kiện. |
| [`StageMapPopup.cs:346-355, 418-428`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L346-L355) | `PveStageFlow.TryStart` phát request `StartPveBattle` nhưng không khoá nút `challengeButton`, cho phép người chơi spam gửi nhiều lệnh vào trận. Trong `StageMapPopup.StartStage`, cờ `leavingForBattle = true;` được gán trước cuộc gọi mạng; nếu server từ chối (`rejected`), cờ không được hoàn tác về `false`, khiến `RestoreHomeMusic` không chạy khi đóng popup và làm mất nhạc nền Home. | **Cao** | Disable `challengeButton` khi gửi `StartPveBattle`; trong callback báo lỗi mạng, khôi phục `leavingForBattle = false` và bật lại tương tác cho nút. |
| [`SectSelectPopup.cs:32, 55, 221-232`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L32) | Lưu `profile` trong trường private từ `OnSetup` mà không nghe `ProgressChanged`. Khi bấm xác nhận vào phái (`CompleteJoin`), bản sao `preview` được tạo từ trường `profile` cũ này thay vì `GameBootstrap.Session.Profile`. Cuộc gọi mạng `ChooseSect` không khoá nút `primaryButton` ("Gia Nhập") hay `closeButton`, cho phép spam click mở nhiều popup xác nhận hoặc gửi nhiều request lên server. | **Cao** | Disable `primaryButton` và `closeButton` ngay khi bắt đầu gửi `ChooseSect`, đồng thời đọc trực tiếp từ `GameBootstrap.Session.Profile` khi tạo `preview`. |
| [`ShopPopup.cs:427-432, 330-369`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L427-L432) | `SetActionPending(true)` chỉ vô hiệu hoá `refreshButton` và `exchangeButton`. Toàn bộ nút mua trên các ô hàng Thương Hội (`tradeCards`), Tiệm Danh Vọng (`reputationCards`), nút `closeButton`, và các nút chuyển tab (`tradeTab`, `exchangeTab`, `reputationTab`) không bị khoá. Người chơi có thể bấm mua liên tục nhiều ô hoặc spam cùng một ô trong khi giao dịch trước đang chờ server xử lý. | **Cao** | Cập nhật `SetActionPending` để vô hiệu hoá toàn bộ nút mua trên các thẻ hàng đang hiển thị, khoá nút đóng và các nút chuyển tab cho tới khi nhận phản hồi từ server. |
| [`BagLoadoutPanel.cs:51, 137, 203-211`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L51) & [`ItemTrayView.cs:23-24, 58, 85-101`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L23-L24) | `BagLoadoutPanel` lưu `boundProfile`, `ItemTrayView` lưu `draftProfile` và `storageGrid` (giữ `draftProfile`). Không thành phần nào nghe `ProgressChanged`. Trong `TryApplyToProfile` (dòng 203-211), client vẫn trực tiếp gọi `trayView.ApplyStorage(boundProfile.Storage)`, xoá và gán lại `boundProfile.Loadout` trên máy thay vì chỉ gửi dữ liệu lên server. | **Cao** | Nghe `ProgressChanged` để làm mới nếu hồ sơ server thay đổi; loại bỏ việc tự ghi đè `Storage` và `Loadout` ở client, chuyển sang mô hình chỉ gửi placements và áp dụng hồ sơ mới từ server trả về. |
| [`BagPopup.cs:70-77, 80-85`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L70-L77) | Khi lưu Bao Phục qua `SaveLoadout`, nếu server từ chối (`rejected`), hàm `HandleSaveError` chỉ hiển thị thông báo lỗi và mở lại `closeButton`. Bố cục trang bị trên panel không được khôi phục về trạng thái hợp lệ của `GameBootstrap.Session.Profile`, dẫn đến lần đóng kế tiếp tiếp tục gửi lại dữ liệu lỗi đã bị từ chối. | **Vừa** | Trong `HandleSaveError`, gọi `bagPanel.Bind(GameBootstrap.Session.Profile)` để hoàn tác toàn bộ vị trí trang bị trên lưới về đúng trạng thái gần nhất của server. |
| [`StoragePopup.cs:62-65, 105, 542-544, 552-557`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L62-L65) | `StoragePopup` lưu `grid` (`StorageGrid`) và `selected` (`OwnedItem`) dạng trường nhưng không nghe `ProgressChanged`. Khi thao tác kho bị lỗi hoặc bị server từ chối (`FailNetwork`), popup không gọi `Refresh()`, khiến UI không được hoàn tác về dữ liệu server. Sau khi nâng cấp, `selected` trỏ vào item của hồ sơ cũ cho tới khi `Select` chạy lại. Nút đóng `closeButton` và bộ lọc loại không bị khoá khi đang có thao tác mạng. | **Vừa** | Đăng ký nghe `ProgressChanged` để tự động vẽ lại; gọi `Refresh()` trong `FailNetwork`; lưu `selectedUid` dạng string thay vì giữ object `OwnedItem`; khoá `closeButton` khi `actionPending == true`. |
| [`HomeController.cs:96-102, 148-151, 351-359`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L96-L102) | `HomeController` không hề nghe `ProgressChanged`. Các nhãn tiền tệ (Bạc, Nguyên Bảo, Danh Vọng) và Lực Chiến chỉ được làm mới ở `Start()` hoặc khi đóng popup con. Nếu tiền tệ thay đổi trực tiếp trên Home (như bấm nhận thưởng ở `QuestTracker`), HUD không tự vẽ lại. Ngoài ra dòng 148-151 phải dùng polling trong `Update()` để kiểm tra cấp độ/kinh nghiệm. | **Vừa** | Cho `HomeController` đăng ký nghe `PlayerProfile.ProgressChanged` để gọi `RefreshQuestAndResources()`, đồng thời loại bỏ đoạn code kiểm tra thủ công trong `Update()`. |
| [`ArenaLobbyPopup.cs:108, 196-207, 458-462`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L108) | `ArenaLobbyPopup` không nghe `ProgressChanged`. Khi mở Tiệm Danh Vọng (`OpenReputationShop`), popup không đăng ký callback `Closed` của `ShopPopup`. Người chơi tiêu Danh Vọng hoặc mua Chí Tôn Ngọc Tủy rồi quay lại sảnh đấu trường thì số dư tiền tệ và vật phẩm trên sảnh vẫn giữ nguyên giá trị cũ. | **Vừa** | Đăng ký `PlayerProfile.ProgressChanged += BindProfile` khi mở popup sảnh đấu trường và huỷ đăng ký khi đóng. |
| [`StageMapPopup.cs:120-204`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L120-L204) | Không nghe `ProgressChanged`. Danh sách nút ải (`nodes`) chỉ được sinh một lần trong `DrawChapter()`. Nếu tiến độ PvE thay đổi qua server (mở ải mới hoặc qua ải), giao diện bản đồ không tự vẽ lại trạng thái các nút ải cho tới khi người chơi chuyển chương hoặc mở lại popup. | **Vừa** | Đăng ký nghe `ProgressChanged` để vẽ lại trạng thái mở khoá/vượt ải của các node trên bản đồ khi hồ sơ cập nhật. |
| [`StorageGrid.cs:54, 68`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L54) | `StorageGrid` lưu `private readonly PlayerProfile profile;`. Mọi instance sống lâu dài (như trong `ItemTrayView`) đều trỏ vào hồ sơ cũ. Ngoài ra tại dòng 68, constructor tự xoá `profile.Materials.Clear()` để migrate dữ liệu cũ sang stack, vi phạm nguyên tắc server quản lý dữ liệu. | **Vừa** | Không lưu trữ lâu dài instance `StorageGrid`; xoá bỏ logic migration tự sửa `Materials` ở constructor client vì server đã chuẩn hóa dữ liệu. |
| [`ArenaLobbyPopup.cs:422-425`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L422-L425) | `RequestReplay` gọi `GameBootstrap.Network.RequestArenaReplay` khi người chơi bấm xem lại trận đấu trong lịch sử nhưng không khoá nút hay hiển thị trạng thái đang tải, cho phép bấm liên tục gửi nhiều request xem lại. | **Thấp** | Disable tương tác của dòng lịch sử hoặc hiển thị trạng thái chờ khi đang tải dữ liệu trận xem lại. |
| [`SettingsPopup.cs:262`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs#L262) | `HandleLoginConfirmation` gọi `GameBootstrap.Network.Logout` mà không vô hiệu hoá nút `loginButton`, cho phép người chơi bấm đăng xuất nhiều lần trong khi request đang bay. | **Thấp** | Vô hiệu hoá `loginButton` trước khi gọi `Logout` và bật lại trong callback. |
| [`DevCheatPanel.cs:168-174`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L168-L174) | Các nút cheat gọi `SendCheat` không có cơ chế khoá nút hay chống spam, có thể gửi trùng lặp nhiều request cheat lên server. | **Thấp** | Thêm cờ chờ xử lý để tạm thời vô hiệu hoá các nút bấm cheat khi request đang bay. |
| [`SectChoiceService.cs:33-34`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L33-L34) | Phương thức `TryChoose` tự gán `profile.SectId = sectId;` và `profile.TamPhapLevel = 1;` trực tiếp trên tham số `profile` truyền vào thay vì chỉ làm nhiệm vụ kiểm tra điều kiện (validate). | **Thấp** | Chuyển `TryChoose` thành hàm kiểm tra điều kiện thuần tuý không làm biến đổi trạng thái của object truyền vào. |
| [`StoryProgress.cs:16, 22-24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L16) | Các hàm `TryQueueBossClear` và `MarkSeen` tự ý thao tác thêm/bớt trên `profile.PendingStoryChapters` và `profile.SeenStoryChapters` ở client. Đây là code thừa từ phiên bản offline cũ. | **Thấp** | Đánh dấu obsolete hoặc xoá bỏ các hàm này; client chỉ hiển thị cốt truyện dựa trên danh sách chapters do server trả về. |

---

## 2. Chi Tiết Rà Soát Theo 4 Yêu Cầu Cốt Lõi

### 2.1. Giữ `PlayerProfile` Lâu Dài
Danh sách các vị trí lưu tham chiếu `PlayerProfile` vào trường, thuộc tính, hoặc service:

1. **`TutorialService`** ([`TutorialService.cs:9, 31-36`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L9)):
   - **Tham chiếu:** Trường `private readonly PlayerProfile profile;` nhận từ constructor `TutorialService(catalog, profile)`.
   - **Khởi tạo:** Được khởi tạo một lần duy nhất tại [`OnboardingRuntime.cs:24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/OnboardingRuntime.cs#L24) trong `Start()`.
   - **Làm mới khi `ProgressChanged` hoặc mở lại popup?** Không làm mới khi `ProgressChanged`. `OnboardingRuntime` là component sống lâu dài nên không khởi tạo lại.
   - **Đọc dữ liệu cũ sau khi chờ server?** Có. Hàm `IsCompleted(sequenceId)` ([`TutorialService.cs:108`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L108)) luôn kiểm tra danh sách `CompletedTutorials` trên object cũ. Nếu server đồng bộ tiến độ hướng dẫn mới, `TutorialService` hoàn toàn không nhận biết.

2. **`StorageGrid`** ([`StorageGrid.cs:54, 57-64`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L54)):
   - **Tham chiếu:** Trường `private readonly PlayerProfile profile;`.
   - **Nơi nắm giữ lâu dài:**
     - [`ItemTrayView.cs:24, 77`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L24): Trường `private StorageGrid storageGrid;` được khởi tạo với `draftProfile` trong `InitializeStorage`.
     - [`StoragePopup.cs:62, 105`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L62): Trường `private StorageGrid grid;` được gán trong `Refresh()`.
   - **Làm mới khi `ProgressChanged` hoặc mở lại popup?** Không làm mới khi `ProgressChanged`. Chỉ làm mới khi mở lại popup hoặc khi gọi lại `Refresh()`.
   - **Đọc dữ liệu cũ sau khi chờ server?** Trong `StoragePopup`, nếu server đẩy hồ sơ mới (hoặc thao tác mạng bị lỗi `FailNetwork` không gọi `Refresh()`), `grid` vẫn giữ hồ sơ cũ. Trong `ItemTrayView`, nếu Bao Phục mở liên tục qua các lần cập nhật server, `storageGrid` giữ `draftProfile` cũ.

3. **`BagLoadoutPanel`** ([`BagLoadoutPanel.cs:51, 137`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L51)):
   - **Tham chiếu:** Trường `private PlayerProfile boundProfile;`.
   - **Khởi tạo:** Gán trong `Bind(PlayerProfile profile)`.
   - **Làm mới khi `ProgressChanged` hoặc mở lại popup?** Không làm mới khi `ProgressChanged`. Chỉ làm mới khi `BagPopup.OnSetup` gọi lại `Bind(session.Profile)`.
   - **Đọc dữ liệu cũ sau khi chờ server?** Nếu hồ sơ server thay đổi khi `BagPopup` đang mở, `boundProfile` sẽ chứa dữ liệu cũ, khiến `RefreshStats()`, `RefreshPower()`, và hàm tìm item [`boundProfile.FindItem(uid)`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L97) đọc thông tin cũ.

4. **`ItemTrayView`** ([`ItemTrayView.cs:23-24, 58`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L23)):
   - **Tham chiếu:** Trường `private PlayerProfile draftProfile;`.
   - **Khởi tạo:** Tạo mới bằng cách sao chép dữ liệu từ profile ở [`InitializeStorage`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L58).
   - **Làm mới khi `ProgressChanged` hoặc mở lại popup?** Không làm mới khi `ProgressChanged`. Chỉ làm mới khi mở lại Bao Phục.
   - **Đọc dữ liệu cũ sau khi chờ server?** Có, nếu nhận hồ sơ mới trong lúc Bao Phục mở.

5. **`SectSelectPopup`** ([`SectSelectPopup.cs:32, 55`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L32)):
   - **Tham chiếu:** Trường `private PlayerProfile profile;`.
   - **Khởi tạo:** Gán trong `OnSetup`: `profile = GameBootstrap.Session.Profile;`.
   - **Làm mới khi `ProgressChanged` hoặc mở lại popup?** Không làm mới khi `ProgressChanged`. Làm mới khi mở lại popup (`OnSetup`).
   - **Đọc dữ liệu cũ sau khi chờ server?** Có. Khi người chơi ấn xác nhận gia nhập phái, dòng 221 dùng chính `profile` cũ này để clone ra bản sao kiểm tra: `PlayerProfile preview = PlayerProfile.FromJson(profile.ToJson());`.

6. **`StoragePopup`** ([`StoragePopup.cs:63, 542`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L63)):
   - **Tham chiếu:** Trường `private OwnedItem selected;`.
   - **Hành vi sau cuộc gọi mạng:** Tại dòng 542: `bool keepSelection = action.StartsWith("upgrade", StringComparison.Ordinal); if (!keepSelection) selected = null;`. Khi nâng cấp thành công, `selected` không bị xoá, nghĩa là nó giữ một object `OwnedItem` thuộc về hồ sơ cũ trước khi server gửi hồ sơ mới.

---

### 2.2. Vẫn Tự Sửa Hồ Sơ Ở Client (Client Mutations)
Dù đã áp dụng server-authoritative E6, một số vị trí vẫn đang trực tiếp can thiệp và sửa đổi dữ liệu trên object thật:

1. **`BagLoadoutPanel.TryApplyToProfile`** ([`BagLoadoutPanel.cs:203-211`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L203-L211)):
   - Gọi `trayView.ApplyStorage(boundProfile.Storage);` (xoá và nạp lại toàn bộ danh sách item kho).
   - Gọi `boundProfile.Loadout.Clear();` và thêm từng `LoadoutPlacement` vào `boundProfile.Loadout`.
   - Gọi `boundProfile.Validate();`.
   - *Đánh giá:* Đây là phương thức client-side mutation hoàn chỉnh. Hiện tại `BagPopup` lưu thông qua `NetworkService.SaveLoadout`, nhưng phương thức này vẫn tồn tại công khai và sửa thẳng vào `boundProfile`.

2. **`StorageGrid..ctor`** ([`StorageGrid.cs:68`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L68)):
   - Gọi `profile.Materials.Clear();` ngay trong constructor khi khởi tạo lưới kho nếu phát hiện `MaterialStacks` rỗng và `Materials` có dữ liệu cũ.
   - *Đánh giá:* Sửa đổi trực tiếp từ điển nguyên liệu của hồ sơ tại client.

3. **`TutorialService.Reset`** ([`TutorialService.cs:90`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L90)):
   - Gọi `profile.CompletedTutorials.Clear();`.
   - *Đánh giá:* Trực tiếp can thiệp vào danh sách tutorial của object `profile` được lưu trong service.

4. **`SectChoiceService.TryChoose`** ([`SectChoiceService.cs:33-34`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L33-L34)):
   - Thực hiện gán `profile.SectId = sectId;` và `profile.TamPhapLevel = 1;`.
   - *Đánh giá:* Dù `SectSelectPopup` truyền bản sao `preview`, bản thân hàm này là logic nghiệp vụ có tác dụng phụ (side-effect) thay vì kiểm tra điều kiện thuần tuý.

5. **`QuestService.Claim`** ([`QuestService.cs:96-100`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L96-L100)):
   - Gọi `ApplyReward(profile, quest.Reward)`, `profile.Quest.ClaimedIds.Add(quest.Id);`, `profile.Quest.ActiveIndex++;`, `SetBaseline(profile);`.
   - Trong `ApplyReward` ([`QuestService.cs:166-169`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L166-L169)): gọi `storage.TryAddMaterial(...)`, `profile.AddCurrency(...)`, `LevelService.Grant(...)`.
   - *Đánh giá:* Dù `QuestLogPopup.ClaimActive` truyền `preview`, phương thức này chứa toàn bộ logic tự cộng thưởng trên máy khách. Đặc biệt, nó bắn sự kiện toàn cục tĩnh `NotifyProgressChanged()` dù đang chạy trên bản sao.

6. **`StoryProgress.TryQueueBossClear` & `MarkSeen`** ([`StoryProgress.cs:16, 22-24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L16)):
   - Sửa `profile.PendingStoryChapters` và `profile.SeenStoryChapters` trực tiếp tại client.

---

### 2.3. UI Không Vẽ Lại Sau Khi Nhận Hồ Sơ Mới
Toàn bộ codebase hiện tại **chỉ có duy nhất một lớp** lắng nghe sự kiện `PlayerProfile.ProgressChanged` là [`QuestTracker.cs:40`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/QuestTracker.cs#L40). Tất cả các UI Controller và Popup khác đều không đăng ký sự kiện này:

1. **`QuestLogPopup`** ([`QuestLogPopup.cs:48-77, 98-100`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L48-L77)):
   - Không nghe `ProgressChanged`.
   - Khi bấm nhận thưởng `Claim()`, hàm gọi `Refresh()` ngay lập tức ở dòng 99 trong khi request mạng `ClaimQuest` mới bắt đầu gửi đi.
   - Khi server trả lời `quest_claim_ok` và nạp hồ sơ mới, popup không hề vẽ lại, nút nhận thưởng vẫn giữ nguyên trạng thái cũ cho đến khi người chơi tắt popup đi mở lại.

2. **`ShopPopup`** ([`ShopPopup.cs:168-195`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L168-L195)):
   - Không nghe `ProgressChanged`.
   - Khi mua hàng ở dòng 344-345, code kiểm tra `Slots[index].Bought` trước khi server trả lời, dẫn đến sự kiện hoàn thành hướng dẫn bị mất hoàn toàn.
   - Nếu tiền tệ thay đổi từ server do sự kiện khác (ví dụ nhận thưởng nhiệm vụ nền), Thương Hội đang mở không cập nhật số dư Bạc và Danh Vọng.

3. **`HomeController`** ([`HomeController.cs:86-102, 351-359`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L86-L102)):
   - Không nghe `ProgressChanged`.
   - `RefreshCurrencies()` và `RefreshPower()` chỉ chạy ở `Start()` và khi đóng popup con.
   - Khi người chơi nhận thưởng nhiệm vụ từ `QuestTracker` ngay trên màn hình Home (không mở popup), tiền tệ hiển thị trên đỉnh màn hình (Bạc, Nguyên Bảo, Danh Vọng) và Lực Chiến không được cập nhật.

4. **`ArenaLobbyPopup`** ([`ArenaLobbyPopup.cs:108, 196-207, 458-462`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L108)):
   - Không nghe `ProgressChanged`.
   - Khi mở Tiệm Danh Vọng qua `OpenReputationShop()`, không bắt sự kiện đóng popup. Sau khi tiêu Danh Vọng hoặc mua Chí Tôn Ngọc Tủy, quay lại sảnh đấu trường thì Danh Vọng và Ngọc Tủy vẫn hiển thị số cũ.

5. **`StageMapPopup`** ([`StageMapPopup.cs:120-204`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L120-L204)):
   - Không nghe `ProgressChanged`.
   - Các nút ải trên bản đồ không được cập nhật nếu có thay đổi tiến độ vượt ải từ server trừ khi đổi chương hoặc mở lại popup.

6. **`StoragePopup`** ([`StoragePopup.cs:98-180`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L98-L180)):
   - Không nghe `ProgressChanged`.
   - Nếu có gói tin cập nhật hồ sơ từ bên ngoài, popup kho không tự động vẽ lại.

7. **`BagPopup` & `BagLoadoutPanel`** ([`BagPopup.cs:27-33`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L27-L33)):
   - Không nghe `ProgressChanged`.
   - Bao Phục không tự vẽ lại nếu hồ sơ thay đổi khi đang mở.

8. **`SectSelectPopup`** ([`SectSelectPopup.cs:51-59`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L51-L59)):
   - Không nghe `ProgressChanged`.
   - Thẻ môn phái và trạng thái đã gia nhập không cập nhật nếu hồ sơ thay đổi từ bên ngoài.

---

### 2.4. Gọi Thao Tác Mạng Không Khoá Nút Hoặc Không Xử Lý Lỗi `rejected`

1. **`ShopPopup.ConfirmBuy` & `ConfirmReputation`** ([`ShopPopup.cs:330-369, 427-432`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L330-L369)):
   - `SetActionPending(true)` chỉ tắt tương tác của `refreshButton` và `exchangeButton`.
   - Nút mua trên các thẻ hàng (`ShopCard`) trong danh sách hoàn toàn không bị khoá. Người chơi có thể bấm liên tục vào nút mua của một ô hoặc các ô khác trong khi request mua trước đó đang chờ phản hồi từ server.
   - Nút `closeButton` và các tab cũng không bị vô hiệu hoá.

2. **`QuestLogPopup.ClaimActive`** ([`QuestLogPopup.cs:103-118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L103-L118)):
   - Không hề khoá nút `ClaimButton` trên dòng nhiệm vụ khi gửi lệnh `ClaimQuest`.
   - Người chơi có thể bấm liên tiếp nhiều lần gây duplicate request.
   - Khi server trả về lỗi `rejected`, popup hiển thị `MessagePopup` thông báo lỗi nhưng danh sách nhiệm vụ vẫn ở trạng thái cũ, không được refresh lại.

3. **`PveStageFlow.TryStart` & `StageMapPopup.StartStage`** ([`StageMapPopup.cs:346-355, 418-428`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L346-L355)):
   - `TryStart` gửi `StartPveBattle` mà không khoá nút `challengeButton`.
   - Trong `StartStage`: `leavingForBattle = true;` được bật trước khi gọi mạng. Khi server trả lời lỗi `rejected`, cờ `leavingForBattle` vẫn giữ nguyên là `true`. Khi người chơi bấm đóng bản đồ, hàm `RestoreHomeMusic` ([`StageMapPopup.cs:236`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L236)) tưởng nhầm người chơi đang vào trận chiến nên không phát lại nhạc nền Home, gây mất tiếng game.

4. **`SectSelectPopup.CompleteJoin`** ([`SectSelectPopup.cs:228-232`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L228-L232)):
   - Không vô hiệu hoá nút `primaryButton` ("Gia Nhập") hay `closeButton` trong lúc gửi `ChooseSect`.
   - Người chơi có thể click liên tục mở thêm các popup xác nhận hoặc gửi nhiều request gia nhập phái.

5. **`BagPopup.SaveAndClose`** ([`BagPopup.cs:67, 80-85`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L67)):
   - Nút `closeButton` bị khoá trong khi lưu, nhưng khi server trả về lỗi `rejected` (`HandleSaveError`), popup chỉ mở lại nút đóng và hiện thông báo lỗi. Bố cục vật phẩm trên màn hình không được khôi phục về trạng thái hợp lệ của server, khiến lần đóng tiếp theo gửi lại đúng payload không hợp lệ.

6. **`StoragePopup.FailNetwork`** ([`StoragePopup.cs:552-557`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L552-L557)):
   - Khi các thao tác sắp xếp, kéo thả, nâng cấp, hợp thành hoặc bán bị server từ chối (`rejected`), `FailNetwork` chỉ gán thông báo lỗi vào `costText.text` mà KHÔNG gọi `Refresh()`, dẫn đến giao diện kho không được hoàn tác đồng bộ với server.
   - Nút `closeButton` và `typeFilter` không bị vô hiệu hoá khi `actionPending == true`.

7. **`ArenaLobbyPopup.RequestReplay`** ([`ArenaLobbyPopup.cs:422-425`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L422-L425)):
   - Không vô hiệu hoá tương tác hay hiển thị trạng thái đang tải khi gọi `RequestArenaReplay`.

8. **`SettingsPopup.HandleLoginConfirmation`** ([`SettingsPopup.cs:262`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs#L262)):
   - Không khoá `loginButton` trong khi yêu cầu đăng xuất đang được gửi lên server.

9. **`DevCheatPanel.SendCheat`** ([`DevCheatPanel.cs:168-174`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L168-L174)):
   - Không khoá các nút bấm cheat khi đang chờ phản hồi từ server.

---

## 3. Đề Xuất Lộ Trình Sửa Chữa (Action Plan)

1. **Chuẩn hoá cơ chế lắng nghe `PlayerProfile.ProgressChanged`:**
   - Tạo mẫu (pattern) đăng ký và huỷ đăng ký `PlayerProfile.ProgressChanged` đồng nhất trên tất cả các UI Controller và BasePopup (`HomeController`, `BagPopup`, `StoragePopup`, `ShopPopup`, `SectSelectPopup`, `QuestLogPopup`, `StageMapPopup`, `ArenaLobbyPopup`).
   - Đảm bảo khi `ApplyServerProfile` phát sinh object mới, mọi màn hình đang mở đều lập tức gọi `Refresh()` để vẽ lại dữ liệu mới nhất.

2. **Khắc phục ngay các lỗi chặn luồng (Mức Cao):**
   - **`ShopPopup.cs:344`:** Đưa logic kiểm tra mua hàng và gọi `OnboardingRuntime.Publish("shop_item_purchased")` vào callback `onOk` của `BuyShopSlot`.
   - **`ShopPopup.cs:427`:** Nâng cấp `SetActionPending` để disable toàn bộ các nút mua trên thẻ hàng, nút đóng và tab chuyển đổi.
   - **`QuestLogPopup.cs:98, 114`:** Khoá nút nhận thưởng khi bấm; bỏ lệnh `Refresh()` trước mạng; vẽ lại sau khi `ClaimQuest` hoàn thành; tách logic preflight trong `QuestService.Claim` để không phát `NotifyProgressChanged` sớm.
   - **`TutorialService.cs`:** Bỏ trường lưu `PlayerProfile` cố định, đọc động qua `GameBootstrap.Session.Profile`.
   - **`StageMapPopup.cs:346`:** Khoá `challengeButton` khi vào trận; hoàn tác cờ `leavingForBattle = false` trong callback báo lỗi mạng.
   - **`SectSelectPopup.cs:228`:** Khoá nút `primaryButton` và `closeButton` khi gửi yêu cầu chọn môn phái.

3. **Hoàn tác UI khi gặp lỗi mạng (`rejected`):**
   - Trong `BagPopup.HandleSaveError`: gọi lại `bagPanel.Bind(GameBootstrap.Session.Profile)` để đưa trang bị về vị trí server đã lưu.
   - Trong `StoragePopup.FailNetwork`: gọi `Refresh()` để làm mới lại lưới kho đồ và thông tin vật phẩm về đúng trạng thái máy chủ.
