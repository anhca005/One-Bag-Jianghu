# Báo Cáo Kiểm Kê Dữ Liệu Hồ Sơ Người Chơi và Các Thao Tác Client (Chuyển Giao Quyền Quyết Định Lên Server - E6)

- **Ngày lập:** 2026-10-21
- **Người thực hiện:** Gemini (Kiến trúc sư hệ thống One Bag Jianghu)
- **Mục tiêu:** Cung cấp cho Claude danh sách đầy đủ, chi tiết và chính xác đến từng file:dòng về mọi trường dữ liệu trong hồ sơ người chơi (`PlayerProfile`), toàn bộ các thao tác client đang tự đọc/sửa trên máy, phân tích logic thời gian, thống kê dữ liệu tĩnh JSON cần chia sẻ, đề xuất nhóm tin nhắn giao thức mạng, và nhận diện các rủi ro kỹ thuật khi thực hiện Quyết định [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7) (chuyển mọi quyền quyết định dữ liệu lên Java server).
- **Tập tin quy chiếu chính:**
  - [`decisions.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md) (#68, #70)
  - [`protocol.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/tech/protocol.md)
  - [`PlayerProfile.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs)
  - [`StorageGrid.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs)
  - [`GameSession.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs)
  - [`UpgradeService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs)
  - [`MergeService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs)
  - [`SellService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs)
  - [`ShopService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs)
  - [`LevelService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs)
  - [`PveProgress.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs)
  - [`PveRewardService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs)
  - [`QuestService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs)
  - [`TutorialService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs)
  - [`StoryProgress.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs)
  - [`ArenaMatchLogic.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaMatchLogic.cs)
  - [`StoragePopup.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs)
  - [`BagLoadoutPanel.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs)
  - [`DevCheatPanel.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs)
  - `server/game-server/src/main/java/**`

---

## 1. Bảng Dữ Liệu Hồ Sơ Người Chơi (`PlayerProfile` và các lớp con)

Theo Quyết định [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7), không lưu thông tin người chơi trên máy. Toàn bộ kho đồ, Bao Phục, tiền tệ, cấp độ, Tâm Pháp, môn phái, Thể Lực, tiến độ PvE, nhiệm vụ, hướng dẫn và cốt truyện đều phải chuyển lên server quản lý. Client chỉ lưu token đăng nhập (khi bật "Ghi nhớ") và cấu hình âm thanh / hiển thị (`GameSettings`).

Dưới đây là kiểm kê chi tiết từng trường trong [`PlayerProfile.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs) và các lớp con/lớp phụ thuộc trực tiếp:

### 1.1. Lớp gốc `PlayerProfile` ([`PlayerProfile.cs:107-150`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L107-L150))

| Tên trường | Kiểu dữ liệu | Ý nghĩa | Đối tượng đọc (File:Dòng) | Đối tượng ghi (File:Dòng) | Phân loại |
|---|---|---|---|---|---|
| `Version` | `int` | Phiên bản schema của hồ sơ (hiện tại là 9). Dùng để migrate dữ liệu cũ. | [`PlayerProfile.cs:248`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L248), [`GameSession.cs:336`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L336) | [`PlayerProfile.cs:423`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L423) | **Lên Server** (Metadata DB) |
| `HeroName` | `string` | Tên Hiệp Khách hiển thị của người chơi. | [`GameSession.cs:190`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L190), [`GameSession.cs:427`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L427), [`GameplayController.cs:201`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L201), [`ShopPopup.cs:103`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L103) | [`GameSession.cs:194`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L194), [`LoginController.cs:41`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs#L41), [`StarterKit.cs:36`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StarterKit.cs#L36) | **Lên Server** (Bảng `accounts`) |
| `HeroLevel` | `int` | Cấp độ nhân vật (1–30). Ảnh hưởng Khí Huyết cơ bản và mở khóa phái (cấp 10). | [`GameSession.cs:220`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L220), [`GameSession.cs:398`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L398), [`BagLoadoutPanel.cs:306`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L306), [`LevelService.cs:83`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L83), [`SectChoiceService.cs:15`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L15), [`QuestService.cs:66`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L66), [`HomeController.cs:286`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L286) | [`LevelService.cs:98, 108`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L98), [`GameSession.cs:224`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L224), [`DevCheatPanel.cs:186`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L186) | **Lên Server** (Bắt buộc) |
| `HeroExp` | `long` | Điểm kinh nghiệm hiện tại của cấp đang xét. Khi đầy thì lên cấp, tại max cấp (30) thì bằng 0. | [`LevelService.cs:87`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L87), [`HomeController.cs:141, 262`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L141), [`DevCheatPanel.cs:187`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L187) | [`LevelService.cs:99, 107`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L99), [`DevCheatPanel.cs:193`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L193) | **Lên Server** (Bắt buộc) |
| `SectId` | `string` | Định danh môn phái đã chọn (ví dụ: `kim_cang`, `thai_thanh`...). Chọn vĩnh viễn ở cấp 10. | [`GameSession.cs:200`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L200), [`GameSession.cs:431`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L431), [`BagLoadoutPanel.cs:139`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L139), [`SectChoiceService.cs:21`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L21), [`QuestService.cs:69`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L69), [`SectSelectPopup.cs:73`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L73) | [`SectChoiceService.cs:33`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L33), [`StarterKit.cs:37`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StarterKit.cs#L37), [`DevCheatPanel.cs:201`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L201) | **Lên Server** (Bắt buộc) |
| `TamPhapLevel` | `int` | Tầng Tâm Pháp của môn phái (1–9). Mặc định là 1 khi chọn phái. | [`GameSession.cs:210`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L210), [`GameSession.cs:432`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L432), [`BagLoadoutPanel.cs:139`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L139), [`QuestService.cs:72`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L72), [`SectSelectPopup.cs:139`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L139) | [`SectChoiceService.cs:34`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L34), [`DevCheatPanel.cs:202`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L202) | **Lên Server** (Bắt buộc) |
| `SectPromptShown` | `bool` | Cờ ghi nhớ đã tự động bật popup mở môn phái ở cấp 10 hay chưa. | [`HomeController.cs:118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L118) | [`HomeController.cs:120`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L120), [`DevCheatPanel.cs:203`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L203), [`PlayerProfile.cs:396`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L396) | **Lên Server** (Đồng bộ UI flag) |
| `IntroSeen` | `bool` | Cờ ghi nhớ đã xem qua hoạt cảnh mở đầu (5 khung truyện Intro) hay chưa. | [`OnboardingRuntime.cs:25`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/OnboardingRuntime.cs#L25) | [`IntroPopup.cs:132`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/IntroPopup.cs#L132), [`PlayerProfile.cs:402`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L402) | **Lên Server** (Tránh xem lại khi đổi máy) |
| `CompletedTutorials` | `List<string>` | Danh sách các chuỗi hướng dẫn tân thủ đã hoàn thành (`first_battle`, `shop`, `upgrade`, `sect`). | [`TutorialService.cs:32, 91`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L32), [`HomeController.cs:124`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L124) | [`TutorialService.cs:92`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L92), [`TutorialService.cs:82`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L82), [`PlayerProfile.cs:403`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L403) | **Lên Server** (Bắt buộc) |
| `SeenStoryChapters` | `List<int>` | Danh sách chỉ số các Hồi cốt truyện đã xem xong trong Giang Hồ Bách Khoa. | [`StoryProgress.cs:14, 23`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L14) | [`StoryProgress.cs:24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L24), [`PlayerProfile.cs:409`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L409) | **Lên Server** (Bắt buộc) |
| `PendingStoryChapters` | `List<int>` | Danh sách các Hồi cốt truyện đã mở khóa sau khi hạ Boss nhưng chưa mở ra xem. | [`StoryProgress.cs:14, 22`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L14) | [`StoryProgress.cs:16, 22`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L16), [`PlayerProfile.cs:410`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L410) | **Lên Server** (Bắt buộc) |
| `Currencies` | `Dictionary<string, long>` | Số dư tiền tệ (`bac`, `danh_vong`, `nguyen_bao`). | Rất nhiều nơi: [`PlayerProfile.cs:225`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L225), [`UpgradeService.cs:94`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L94), [`ShopService.cs:97, 120, 176, 214`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L97), [`HomeController.cs:382`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L382)... | [`PlayerProfile.cs:219, 228`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L219), [`UpgradeService.cs:85`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L85), [`MergeService.cs:203`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L203), [`SellService.cs:74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L74), [`ShopService.cs:100, 132, 192, 223`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L100), [`OnlineModels.cs:472`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Net/OnlineModels.cs#L472)... | **Lên Server** (Bắt buộc) |
| `Materials` | `Dictionary<string, int>` | Tổng số lượng từng loại nguyên liệu/chất dẫn sở hữu (phản chiếu đồng bộ từ các stack trong kho). | [`PlayerProfile.cs:207, 349`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L207), [`UpgradeService.cs:103`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L103), [`MergeService.cs:98`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L98), [`ShopService.cs:175`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L175) | [`StorageGrid.cs:283-294`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L283-L294) (đồng bộ từ stacks) | **Lên Server** (Bắt buộc) |
| `Items` | `List<OwnedItem>` | Danh sách mọi vật phẩm người chơi sở hữu (gồm cả đang đeo trên Bao Phục lẫn trong kho). | [`PlayerProfile.cs:236, 243, 297`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L236), [`MergeService.cs:118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L118), [`StoragePopup.cs:403`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L403), [`QuestService.cs:185`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L185) | [`PlayerProfile.cs:174, 193`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L174), [`MergeService.cs:145-147, 167-169`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L145), [`SellService.cs:73`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L73), [`ShopService.cs:134`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L134), [`PveRewardService.cs:119`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L119) | **Lên Server** (Bắt buộc) |
| `Loadout` | `List<LoadoutPlacement>` | Danh sách vị trí trang bị trong Bao Phục (lưới 5x4). | [`GameSession.cs:306, 396, 416`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L306), [`BagLoadoutPanel.cs:228`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L228), [`StoragePopup.cs:293`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L293), [`MergeService.cs:197`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L197) | [`BagLoadoutPanel.cs:209`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L209), [`GameSession.cs:484`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L484), [`PlayerProfile.cs:194`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L194), [`StarterKit.cs:69`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StarterKit.cs#L69) | **Lên Server** (Bắt buộc) |
| `FirstClears` | `List<string>` | Legacy list (trước khi có `PveProfileState.Stages`). | [`PlayerProfile.cs:260`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L260) | Không còn ghi | Bỏ hoặc giữ null |
| `Shop` | `ShopState` | Trạng thái toàn bộ các ô hàng Thương Hội và lịch sử mua. | Xem bảng con 1.4 | Xem bảng con 1.4 | **Lên Server** (Bắt buộc) |
| `Storage` | `StorageState` | Không gian lưới Tàng Bảo Các (10x20), vị trí đồ, stacks nguyên liệu và overflow. | Xem bảng con 1.3 | Xem bảng con 1.3 | **Lên Server** (Bắt buộc) |
| `Pve` | `PveProfileState` | Tiến độ vượt ải PvE, Thể Lực và mốc hồi Thể Lực. | Xem bảng con 1.5 | Xem bảng con 1.5 | **Lên Server** (Bắt buộc) |
| `Quest` | `QuestProfileState` | Tiến trình chuỗi nhiệm vụ cốt truyện Hành Tẩu Lục. | Xem bảng con 1.6 | Xem bảng con 1.6 | **Lên Server** (Bắt buộc) |

---

### 1.2. Lớp `OwnedItem` và `LoadoutPlacement` ([`PlayerProfile.cs:33-55`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L33-L55))

- **`OwnedItem`**: Đại diện cho 1 thể hiện vật phẩm cụ thể người chơi sở hữu.
  - `Uid` (`string`): Mã định danh duy nhất ngẫu nhiên (UUID hex) phân biệt các bản sao của cùng một món. **Lên Server**.
  - `ItemId` (`string`): ID trong catalog (`items.json`). **Lên Server**.
  - `Tier` (`int`): Bậc Trọng của món đồ (1–9). **Lên Server**.
  - `Rarity` (`string`): Phẩm chất của món đồ (`common`, `good`, `rare`, `epic`, `legendary`, `supreme`). **Lên Server**.
  - `AcquiredAt` (`string`): ISO-8601 UTC timestamp lúc nhận món. **Lên Server**.
- **`LoadoutPlacement`**: Vị trí ô của món đồ trong Bao Phục (lưới 5x4).
  - `Uid` (`string`): Tham chiếu đến `OwnedItem.Uid`. **Lên Server**.
  - `X` (`int`): Tọa độ cột (0–4). **Lên Server**.
  - `Y` (`int`): Tọa độ hàng (0–3). **Lên Server**.

---

### 1.3. Lớp `StorageState`, `StorageItemPlacement`, `MaterialStack` ([`StorageGrid.cs:13-47`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L13-L47))

- **`StorageState`**: Trạng thái lưới Tàng Bảo Các.
  - `Cols` (`int` = 10), `Rows` (`int` = 20): Kích thước lưới kho. **Lên Server**.
  - `Items` (`List<StorageItemPlacement>`): Danh sách tọa độ các món đồ trong kho. `Uid`, `X`, `Y`. Các món đã trang bị trong `Loadout` sẽ không nằm ở đây ([#27](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L64), [`StorageGrid.cs:123`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L123)). **Lên Server**.
  - `MaterialStacks` (`List<MaterialStack>`): Tọa độ và số lượng từng ô nguyên liệu (mỗi ô kích thước 1x1, tối đa `MaxStack = 999`). Gồm `MaterialId`, `Count`, `X`, `Y`. **Lên Server**.
  - `Overflow` (`List<string>`): Danh sách chuỗi lưu tạm các món đồ/nguyên liệu bị tràn khi nâng cấp dữ liệu mà kho không còn ô trống. **Lên Server**.

---

### 1.4. Lớp `ShopState` và `ShopSlotState` ([`PlayerProfile.cs:57-91`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L57-L91))

- **`ShopSlotState`**: Trạng thái 1 ô hàng trong 6 ô Thương Hội.
  - `Kind` (`string`): Loại hàng (`"item"` hoặc `"material"`). **Lên Server**.
  - `ContentId` (`string`): ID vật phẩm hoặc nguyên liệu. **Lên Server**.
  - `Rarity` (`string`): Phẩm chất (nếu là item). **Lên Server**.
  - `Quantity` (`int`): Số lượng bán (item luôn là 1, nguyên liệu có thể là 1, 2, 5). **Lên Server**.
  - `Price` (`long`): Giá bán sau khi tính hệ số loại đồ và giảm giá. **Lên Server**.
  - `Bought` (`bool`): Đã mua hay chưa. **Lên Server**.
- **`ShopState`**:
  - `Slots` (`List<ShopSlotState>`): 6 ô hàng hiện tại. **Lên Server**.
  - `LastFreeRefreshUtc` (`string`): Mốc thời gian ISO-8601 làm mới miễn phí gần nhất. **Lên Server**.
  - `PaidRefreshesToday` (`int`): Số lượt làm mới có phí đã dùng trong ngày. **Lên Server**.
  - `StarterFreeRefreshesUsed` (`int`): Số lượt làm mới miễn phí khởi đầu đã dùng (tối đa 3). **Lên Server**.
  - `DayKey` (`string`): Chuỗi ngày hiện tại `yyyy-MM-dd` để kiểm tra qua ngày mới. **Lên Server**.
  - `WeekKey` (`string`): Chuỗi tuần hiện tại `yyyy-MM-dd` (thứ Hai) để kiểm tra qua tuần mới. **Lên Server**.
  - `ReputationPurchases` (`Dictionary<string, int>`): Số lượt đã mua trong tuần của từng mặt hàng Tiệm Danh Vọng. **Lên Server**.
  - `Generation` (`int`): Số đếm chu kỳ sinh hàng (dùng để làm salt cho PRNG). **Lên Server**.

---

### 1.5. Lớp `PveProfileState` và `PveStageProgress` ([`PlayerProfile.cs:8-31`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L8-L31))

- **`PveStageProgress`**:
  - `Cleared` (`bool`): Trạng thái ải đã qua. **Lên Server**.
  - `BestStars` (`int`): Legacy chấm sao (đã bỏ theo [#63](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L123)). Giữ để tương thích ngược.
- **`PveProfileState`**:
  - `Stages` (`Dictionary<string, PveStageProgress>`): Tiến độ từng ải trong 5 Hồi (tổng 30 ải: `hoi_1_01` -> `hoi_5_10`). **Lên Server**.
  - `Stamina` (`int`): Thể Lực hiện tại (-1 lúc chưa khởi tạo, tối đa 60). **Lên Server**.
  - `LastStaminaRegenUtc` (`string`): Mốc thời gian ISO-8601 hồi điểm Thể Lực gần nhất. **Lên Server**.
  - `DailyFirstClears` (`List<string>`): Danh sách ải đã vượt lần đầu trong ngày (phục vụ reset). **Lên Server**.
  - `DailyFirstClearDate` (`string`): Chuỗi ngày `yyyy-MM-dd` để reset danh sách `DailyFirstClears`. **Lên Server**.

---

### 1.6. Lớp `QuestProfileState` ([`PlayerProfile.cs:93-105`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L93-L105))

- `ActiveIndex` (`int`): Thứ tự nhiệm vụ đang kích hoạt trong danh sách nhiệm vụ tuyến tính (`quests.json`). **Lên Server**.
- `ActiveId` (`string`): ID nhiệm vụ đang làm (`quest_1`, `quest_2`...). **Lên Server**.
- `ClaimedIds` (`List<string>`): Danh sách ID các nhiệm vụ đã nhận thưởng xong. **Lên Server**.
- `ActionCounters` (`Dictionary<string, int>`): Bộ đếm tổng lũy kế các hành vi đã thực hiện (`pve_win`, `item_upgrade`, `item_merge`, `shop_buy`). **Lên Server**.
- `ActionBaseline` (`Dictionary<string, int>`): Mốc snapshot của bộ đếm tại thời điểm bắt đầu nhiệm vụ hiện tại. **Lên Server**.

---

### 1.7. Danh Sách Trạng Thái Client-Only (Chỉ Giữ Ở Máy Khách)

Các trạng thái sau đây hoàn toàn là cục bộ, không liên quan đến kinh tế hay dữ liệu thi đấu, tiếp tục giữ nguyên tại client:
1. **Cấu hình game (`GameSettings.cs`)**: `MasterVolume`, `MusicVolume`, `SfxVolume`, `Muted`, `Fullscreen`, `ResolutionWidth`, `ResolutionHeight`, `VSync`, `FrameRateLimit`. Lưu trong `PlayerPrefs` (`OneBagJianghu.MasterVolume`...).
2. **Phiên đăng nhập**: Chuỗi token 32 ký tự ngẫu nhiên của server trả về khi `auth_ok`, lưu trong `PlayerPrefs` khi người dùng chọn "Ghi nhớ đăng nhập".
3. **Cấu hình mạng**: `OneBagJianghu.ServerUrl` trong `PlayerPrefs`.
4. **Trạng thái UI tạm thời trong phiên (Ephemeral UI State)**:
   - Ngăn xếp popup đang hiển thị (`PopupStack.cs`).
   - Tab đang chọn trong màn hình: Tab Thương Mại / Đổi Nguyên Liệu / Danh Vọng trong `ShopPopup`; Tab Xếp Hạng / Lịch Sử / Thưởng / Tiệm trong `ArenaLobbyPopup`; Bộ lọc loại trang bị `typeFilter` trong `StoragePopup`.
   - Vị trí scroll của danh sách kho và lịch sử đấu.
   - Thao tác kéo thả đồ dở dang (con trỏ chuột đang giữ item nào, ô lưới đang highlight xanh/đỏ).
   - Tiến độ phát lại trận đấu (Replay clock, thanh tiến trình trận đấu, trạng thái pause/play, tốc độ x1/x2).

---

## 2. Bảng Thao Tác Làm Thay Đổi Hồ Sơ

Dưới đây là bảng thống kê đầy đủ toàn bộ 26 thao tác đang làm thay đổi hồ sơ người chơi trong mã nguồn C#:

| Tên thao tác | Lớp & Hàm thực hiện (File:Dòng) | Màn hình / UI gọi | Đầu vào & Trường hồ sơ bị thay đổi | Có dùng ngẫu nhiên? (Nguồn seed) | Phụ thuộc JSON |
|---|---|---|---|---|---|
| **1. Nâng Trọng** | `UpgradeService.Apply` ([`UpgradeService.cs:80-90`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L80-L90)) | `StoragePopup.Upgrade` ([`StoragePopup.cs:454-465`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L454-L465)) | **Vào:** `uid` của trang bị.<br>**Đổi:**<br>- `Currencies["bac"]` giảm theo phí nâng Trọng;<br>- `Storage.MaterialStacks` & `Materials` giảm theo nguyên liệu/chất dẫn;<br>- `item.Tier` tăng +1 (tối đa 9);<br>- `Quest.ActionCounters["item_upgrade"]` tăng +1. | **Không** (Xác định 100%) | `upgrade_costs.json`, `materials.json`, `items.json` |
| **2. Hợp Thành** | `MergeService.Apply` ([`MergeService.cs:126-172`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L126-L172)) | `StoragePopup.ChooseMergeCandidate` ([`StoragePopup.cs:517-525`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L517-L525)) | **Vào:** `firstUid` (món chính giữ Trọng), `secondUid` (món nguyên liệu bị tiêu hao).<br>**Đổi:**<br>- `Currencies["bac"]` giảm theo phí Hợp Thành;<br>- `Storage.MaterialStacks` & `Materials` giảm theo phụ liệu `ExtraMaterials`;<br>- Hoàn trả 50% nguyên liệu đã nâng Trọng của món thứ 2 (`RefundMaterials`);<br>- Món chính lên phẩm mới (`Rarity`), giữ nguyên Trọng theo [#67](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L78);<br>- Món thứ 2 bị xóa khỏi `Items`, `Loadout`, `Storage.Items`;<br>- `Quest.ActionCounters["item_merge"]` tăng +1. | **Không** | `merge.json`, `upgrade_costs.json`, `materials.json`, `items.json` |
| **3. Bán vật phẩm** | `SellService.Apply` ([`SellService.cs:67-78`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L67-L78)) | `StoragePopup.Sell` ([`StoragePopup.cs:528-544`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L528-L544)) | **Vào:** `uid` món đồ trong kho.<br>**Đổi:**<br>- Món bị xóa khỏi `Items` và `Storage.Items`;<br>- `Currencies["bac"]` tăng (30% giá gốc + 15% Bạc đã nâng Trọng);<br>- `Storage.MaterialStacks` & `Materials` nhận lại 50% nguyên liệu/chất dẫn đã nâng Trọng. | **Không** | `upgrade_costs.json`, `materials.json`, `items.json` |
| **4. Mua hàng Thương Hội** | `ShopService.Buy` ([`ShopService.cs:112-140`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L112-L140)) | `ShopPopup.ConfirmBuy` ([`ShopPopup.cs:326-344`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L326-L344)) | **Vào:** Chỉ số ô `index` (0–5).<br>**Đổi:**<br>- `Currencies["bac"]` giảm theo `slot.Price`;<br>- Thêm trang bị mới vào `Items` & `Storage.Items` (Trọng 1) HOẶC thêm nguyên liệu vào `Storage.MaterialStacks` & `Materials`;<br>- `slot.Bought = true`;<br>- `Quest.ActionCounters["shop_buy"]` tăng +1. | **Không** lúc mua (Ngẫu nhiên lúc sinh ô) | `shop.json`, `materials.json`, `items.json` |
| **5. Làm mới Thương Hội** | `ShopService.Refresh` ([`ShopService.cs:81-104`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L81-L104)) & `ShopService.Ensure` ([`ShopService.cs:47-53`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L47-L53)) | `ShopPopup.RefreshShop` ([`ShopPopup.cs:363-371`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L363-L371)) | **Vào:** `free` (bool).<br>**Đổi:**<br>- Nếu miễn phí: `Shop.StarterFreeRefreshesUsed++` (nếu dùng lượt khởi đầu) hoặc cập nhật `Shop.LastFreeRefreshUtc = now`;<br>- Nếu trả phí: `Currencies["bac"] -= 200`, `Shop.PaidRefreshesToday++`;<br>- Cả hai: `Shop.Generation++`, tạo mới hoàn toàn 6 ô `Shop.Slots`. | **Có** (PRNG: `seed = HeroName.GetHashCode()`, `salt = Shop.Generation`, thuật toán `seed * 486187739 + salt`) | `shop.json`, `materials.json`, `items.json` |
| **6. Đổi nguyên liệu Thương Hội** | `ShopService.Exchange` ([`ShopService.cs:151-194`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L151-L194)) | `ShopPopup.Exchange` ([`ShopPopup.cs:373-378`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L373-L378)) | **Vào:** `inputId`, `targetId`.<br>**Đổi:**<br>- Trừ nguyên liệu vào (`inputId`): 3 nếu cùng loại lên bậc, 2 nếu khác loại ngang bậc;<br>- Thêm 1 nguyên liệu ra (`targetId`) vào kho;<br>- `Currencies["bac"]` giảm theo phí đổi quy định. | **Không** | `shop.json`, `materials.json` |
| **7. Mua Tiệm Danh Vọng** | `ShopService.BuyReputation` ([`ShopService.cs:206-228`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L206-L228)) | `ShopPopup.ConfirmReputation` ([`ShopPopup.cs:346-362`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L346-L362)) | **Vào:** `offerId` (`chi_ton_ngoc_tuy`, `catalyst_box_random`, `tier_3_material_box`...).<br>**Đổi:**<br>- `Currencies["danh_vong"] -= offer.Price`;<br>- `Storage.MaterialStacks` & `Materials` nhận nguyên liệu tương ứng;<br>- `Shop.ReputationPurchases[offerId]++`. | **Có** nếu mở hộp ngẫu nhiên (PRNG: `seed * 486187739 + Generation * 397 + StableHash(offerId)`) | `shop.json`, `materials.json` |
| **8. Làm mới chu kỳ ngày/tuần Shop** | `ShopService.ResetPeriods` ([`ShopService.cs:343-360`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L343-L360)) | Tự động gọi mỗi khi mở Shop hoặc kiểm tra mua | **Vào:** `clock.UtcNow`.<br>**Đổi:**<br>- Qua ngày: `Shop.DayKey = day`, `Shop.PaidRefreshesToday = 0`;<br>- Qua tuần (thứ Hai): `Shop.WeekKey = week`, `Shop.ReputationPurchases.Clear()`. | **Không** | `shop.json` |
| **9. Kéo thả & Sắp xếp Tàng Bảo Các** | `StorageGrid.TryMoveItem`, `TryMoveStack` ([`StorageGrid.cs:174-205`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L174-L205)), `StoragePopup.Repack` ([`StoragePopup.cs:397-441`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L397-L441)) | `StoragePopup.CommitDrop` & nút "Dọn dẹp" (`Repack`) | **Vào:** Tọa độ đích `(x, y)` hoặc kích hoạt dọn kho.<br>**Đổi:**<br>- Tọa độ `X, Y` của `Storage.Items` và `Storage.MaterialStacks`;<br>- Dọn kho: gom các stack nguyên liệu rời rạc, gộp tràn `Overflow`. | **Không** | `items.json`, `materials.json` |
| **10. Xếp Bao Phục** | `BagLoadoutPanel.TryApplyToProfile` ([`BagLoadoutPanel.cs:198-223`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L198-L223)) | `BagPopup.SaveAndClose` ([`BagPopup.cs:59-69`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L59-L69)) | **Vào:** Lưới trang bị 5x4 trên UI.<br>**Đổi:**<br>- `Loadout` được ghi đè danh sách `LoadoutPlacement (uid, x, y)`;<br>- Món vào Bao Phục bị gỡ khỏi `Storage.Items`; món tháo ra được trả về ô trống `Storage.Items`. | **Không** | `items.json` |
| **11. Gia nhập Môn Phái** | `SectChoiceService.TryChoose` ([`SectChoiceService.cs:13-37`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L13-L37)) | `SectSelectPopup.CompleteJoin` ([`SectSelectPopup.cs:214-229`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L214-L229)) | **Vào:** `sectId` (khi nhân vật >= cấp 10).<br>**Đổi:**<br>- `profile.SectId = sectId`;<br>- `profile.TamPhapLevel = 1`. | **Không** | `sects.json` |
| **12. Cờ đã nhắc chọn phái** | Gán trực tiếp ([`HomeController.cs:120`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L120)) | `HomeController.Update` (khi đạt cấp 10) | **Vào:** Cấp độ >= 10.<br>**Đổi:** `profile.SectPromptShown = true`. | **Không** | Không |
| **13. Hồi Thể Lực theo thời gian** | `StaminaService.Refresh` ([`PveProgress.cs:66-89`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L66-L89)) | `HomeController.RefreshStamina` ([`HomeController.cs:402`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L402)), `StageMapPopup.RefreshStamina` ([`StageMapPopup.cs:311`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L311)) | **Vào:** `DateTimeOffset.UtcNow`.<br>**Đổi:**<br>- `Pve.Stamina` tăng theo thời gian trôi qua (+1 điểm mỗi 6 phút, trần 60);<br>- `Pve.LastStaminaRegenUtc` cập nhật mốc thời gian. | **Không** | `Pve/stamina.json` |
| **14. Tiêu hao Thể Lực vào ải** | `StaminaService.TrySpend` ([`PveProgress.cs:91-102`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L91-L102)) | `StageMapPopup.OnStageClicked` ([`StageMapPopup.cs:426`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L426)) | **Vào:** `stage.StaminaCost` (5 Thể Lực theo [#59](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L163)).<br>**Đổi:**<br>- `Pve.Stamina -= 5`;<br>- `Pve.LastStaminaRegenUtc` cập nhật mốc. | **Không** | `Pve/stamina.json`, `Pve/chapters.json` |
| **15. Vượt ải & Nhận thưởng PvE** | `GameplayController.ShowPveResult` ([`GameplayController.cs:255-297`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L255-L297)), gọi `PveRewardService.Grant` ([`PveRewardService.cs:32-50`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L32-L50)) | Kết thúc trận PvE trong `GameplayController` | **Vào:** `stage`, `firstClear`, `battleSeed`.<br>**Đổi:**<br>- `Pve.Stages[stage.Id].Cleared = true`;<br>- `Pve.DailyFirstClears` cập nhật ải;<br>- `Currencies["bac"]` cộng Bạc thưởng;<br>- `HeroExp`, `HeroLevel` tăng qua `LevelService.Grant`;<br>- `Storage.MaterialStacks` & `Materials` cộng nguyên liệu;<br>- `Items` & `Storage.Items` cộng trang bị rơi (nếu có);<br>- `Quest.ActionCounters["pve_win"]++`;<br>- `PendingStoryChapters` thêm Hồi mới nếu hạ Boss lần đầu. | **Có** (PRNG LCG: `state = state * 1664525 + 1013904223`, seed lấy từ `battleSeed` trận đấu để tính rơi thêm nguyên liệu 15% và tỉ lệ rơi đồ) | `Pve/chapters.json`, `items.json`, `levels.json` |
| **16. Nhận thưởng Luận Kiếm PvP** | `ArenaMatchReceipt.ApplyRewardOnce` ([`ArenaMatchLogic.cs:15-24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaMatchLogic.cs#L15-L24)), `OnlineProtocol.ApplyReward` ([`OnlineModels.cs:463-481`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Net/OnlineModels.cs#L463-L481)) | `ArenaLobbyPopup.StartMatchmaking` khi nhận gói `arena_result` ([`ArenaLobbyPopup.cs:342-343`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L342-L343)) | **Vào:** Gói tin `arena_result.Reward`.<br>**Đổi:**<br>- `Currencies["danh_vong"] += reward.Reputation`;<br>- `Storage.MaterialStacks` & `Materials` cộng nguyên liệu (ví dụ `chi_ton_ngoc_tuy`). | Do Server quyết định | Không |
| **17. Cộng EXP & Lên cấp** | `LevelService.Grant` ([`LevelService.cs:89-117`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L89-L117)) | Gọi từ `PveRewardService`, `QuestService`, `DevCheatPanel` | **Vào:** `amount` (lượng EXP thưởng).<br>**Đổi:**<br>- `HeroExp += amount`;<br>- `HeroLevel` tăng lên nếu đủ `ExpToNext` (tối đa 30, max cấp thì `HeroExp = 0`). | **Không** | `levels.json` |
| **18. Nhận thưởng Nhiệm vụ** | `QuestService.Claim` ([`QuestService.cs:87-111`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L87-L111)) | `QuestLogPopup.Claim` ([`QuestLogPopup.cs:72`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L72)), `QuestTracker.Claim` ([`QuestTracker.cs:74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/QuestTracker.cs#L74)) | **Vào:** Nhiệm vụ hiện tại `Quest.ActiveIndex`.<br>**Đổi:**<br>- `Currencies["bac"]` cộng Bạc thưởng;<br>- `HeroExp`, `HeroLevel` tăng qua `LevelService.Grant`;<br>- `Storage.MaterialStacks` & `Materials` cộng nguyên liệu thưởng;<br>- `Quest.ClaimedIds.Add(quest.Id)`;<br>- `Quest.ActiveIndex++`, `Quest.ActiveId = next.Id`;<br>- `Quest.ActionBaseline = copy(ActionCounters)`. | **Không** | `quests.json`, `levels.json`, `materials.json`, `items.json` |
| **19. Ghi nhận hành động Nhiệm vụ** | `PlayerProfile.RecordQuestAction` ([`PlayerProfile.cs:178-183`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L178-L183)) | Gọi tự động sau khi nâng Trọng, Hợp Thành, mua Shop, thắng PvE | **Vào:** Tên hành động `action`.<br>**Đổi:** `Quest.ActionCounters[action]++`. | **Không** | Không |
| **20. Hoàn thành Hướng dẫn** | `TutorialService.Complete` ([`TutorialService.cs:88-95`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L88-L95)) | `OnboardingRuntime.Publish` ([`OnboardingRuntime.cs:36-42`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/OnboardingRuntime.cs#L36-L42)) | **Vào:** `sequenceId`.<br>**Đổi:** `CompletedTutorials.Add(sequenceId)`. | **Không** | `tutorial.json` |
| **21. Đánh dấu đã xem Intro** | `IntroPopup.Finish` ([`IntroPopup.cs:128-138`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/IntroPopup.cs#L128-L138)) | Nút "Bỏ qua" hoặc xem hết 5 khung tranh Intro | **Vào:** Không.<br>**Đổi:** `profile.IntroSeen = true`. | **Không** | `intro.json` |
| **22. Tiến trình Cốt truyện Hồi** | `StoryProgress.TryQueueBossClear` & `MarkSeen` ([`StoryProgress.cs:9-25`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L9-L25)) | `GameplayController.ShowPveResult` & mở xem truyện trong Giang Hồ Bách Khoa | **Vào:** `chapterIndex`.<br>**Đổi:**<br>- Thắng Boss lần đầu: `PendingStoryChapters.Add(chapter)`;<br>- Đã mở xem: Xóa khỏi `PendingStoryChapters`, thêm vào `SeenStoryChapters`. | **Không** | `story.json`, `Pve/chapters.json` |
| **23. Đổi tên nhân vật ở Login** | `GameSession.HeroName` setter ([`GameSession.cs:190-196`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L190-L196)), `LoginController.StartGame` ([`LoginController.cs:41`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs#L41)) | Màn hình Login khi bấm "Vào Giang Hồ" | **Vào:** Tên nhập trong InputField.<br>**Đổi:** `profile.HeroName = value`. | **Không** | Không |
| **24. Khởi tạo hồ sơ ban đầu** | `StarterKit.CreateProfile` ([`StarterKit.cs:32-74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StarterKit.cs#L32-L74)) | `ProfileStore.LoadOrCreate` ([`GameSession.cs:128`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L128)) khi chưa có file save | **Vào:** `heroName`, `sectId`.<br>**Đổi:** Tạo mới toàn bộ `PlayerProfile`:<br>- `Currencies["bac"] = 500` ([#48](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L290));<br>- Sinh Thanh Phong Kiếm + Ô Tàm Y vào `Items` & `Loadout`;<br>- Khởi tạo rỗng `Storage`, `Shop`, `Pve`, `Quest`. | **Không** | `starter.json` |
| **25. Migration hồ sơ cũ (v1→v9)** | `PlayerProfile.FromJson` ([`PlayerProfile.cs:360-438`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L360-L438)) | `ProfileStore.Read` ([`GameSession.cs:157-174`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L157-L174)) khi đọc save cũ từ đĩa | **Vào:** Chuỗi JSON cũ.<br>**Đổi:** Tự động điền các trường thiếu qua 9 bước nâng cấp (rarity, shop, storage, pve, sectPromptShown, onboarding, story, quest). | **Không** | Không |
| **26. Bảng cheat nhà phát triển** | Toàn bộ các hàm trong `DevCheatPanel.cs` ([`DevCheatPanel.cs:94-232`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L94-L232)) | `DevCheatPanel` (chỉ bật ở `UNITY_EDITOR` hoặc `DEVELOPMENT_BUILD`) | **Vào:** Các nút trên panel.<br>**Đổi:** Can thiệp trực tiếp bộ nhớ `profile`: thêm item, thêm 10/100 nguyên liệu, +10.000 Bạc, +1.000 Danh Vọng, làm mới shop, nhảy 7 ngày, mở hết ải, hồi Thể Lực, +cấp, +EXP, reset phái, reset profile, xóa sạch save đĩa. | **Không** | Nhiều file JSON |

> [!NOTE]
> **Về tính năng Nâng Tâm Pháp:** Hiện tại trong `SectSelectPopup.cs:172, 248-251` nút này hiển thị nhãn `"Nâng Tâm Pháp (Sắp mở)"` và hàm `OpenTamPhapUpgrade` chỉ là hook dự phòng rỗng. Tuy nhiên trường `profile.TamPhapLevel` đã tồn tại trong schema và đã được mô phỏng chiến đấu sử dụng. Khi chuyển lên server, cần chuẩn bị sẵn thông điệp giao thức cho tính năng này.

---

## 3. Logic Thời Gian và Giờ Máy Chủ

Toàn bộ các vị trí đang sử dụng giờ máy tính (`DateTime.UtcNow`, `DateTimeOffset.UtcNow`, `DateTime.Now`) trong client C#:

| STT | Vị trí (File:Dòng) | Mã lệnh cụ thể | Ý nghĩa nghiệp vụ | Phải chuyển sang giờ Server? | Lý do kỹ thuật |
|---|---|---|---|---|---|
| **1** | [`PlayerProfile.cs:172`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L172) | `DateTimeOffset.UtcNow.ToString("O")` | Gán thời điểm sở hữu vật phẩm (`item.AcquiredAt`). | **Bắt buộc** | Server gán timestamp ISO-8601 khi tạo vật phẩm, tránh sai lệch múi giờ người dùng. |
| **2** | [`ShopService.cs:24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L24) | `DateTimeOffset.UtcNow` | Đồng hồ chuẩn của hệ thống cửa hàng Thương Hội (`IShopClock`). | **Bắt buộc** | Server là người quyết định thời gian hiện tại để tính toán ranh giới mua bán. |
| **3** | [`ShopService.cs:62-63`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L62-L63) | `Parse(Shop.LastFreeRefreshUtc, clock.UtcNow)` | Tính thời gian chờ còn lại cho lượt làm mới shop miễn phí tiếp theo (`UntilFreeRefresh`). | **Bắt buộc** | Chặn gian lận chỉnh giờ máy tính về tương lai để làm mới shop miễn phí vô tận. |
| **4** | [`ShopService.cs:244`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L244) | `Shop.LastFreeRefreshUtc = clock.UtcNow.ToString("O")` | Lưu mốc thời gian làm mới miễn phí shop. | **Bắt buộc** | Mốc thời gian này phải do server ghi và lưu trong DB. |
| **5** | [`ShopService.cs:345, 352`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L345) | `clock.UtcNow.UtcDateTime.ToString("yyyy-MM-dd")` & `Date.AddDays(-back)` | Reset số lượt làm mới có phí (`PaidRefreshesToday = 0`) khi sang ngày mới; reset giới hạn mua Tiệm Danh Vọng (`ReputationPurchases.Clear()`) khi sang tuần mới. | **Bắt buộc** | **Quan trọng:** Client đang dùng UTC thuần túy (`UtcNow`), trong khi Server (`ArenaCalendar.java:13`) chạy múi giờ **UTC+7** (`Asia/Ho_Chi_Minh`). Nếu không chuyển lên server, giờ reset ngày/tuần giữa Shop và Luận Kiếm Đài sẽ bị lệch nhau đúng 7 tiếng! |
| **6** | [`PveProgress.cs:66, 75, 84, 87`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L66) | `StaminaService.Refresh(PveProfileState, DateTimeOffset now)` | Tính toán lượng Thể Lực hồi phục theo thời gian trôi qua (1 điểm / 6 phút) và cập nhật `LastStaminaRegenUtc`. | **Bắt buộc** | Trọng điểm gian lận: Nếu để client tính, người chơi chỉnh giờ máy là có Thể Lực đầy ngay tức khắc. Server phải quản lý hoàn toàn Thể Lực. |
| **7** | [`PveProgress.cs:91, 100`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L91) | `StaminaService.TrySpend` | Tiêu Thể Lực trước khi vào ải. | **Bắt buộc** | Server kiểm tra Thể Lực còn đủ không và trừ trước khi chạy trận. |
| **8** | [`PveProgress.cs:104, 114`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L104) | `SecondsToNext`, `SecondsToFull` | Tính số giây còn lại để hồi 1 Thể Lực / đầy bình. | **Giữ ở Client (UI)** | Dùng để render chữ đếm ngược trơn tru trên UI (`StageMapPopup.cs:288, 326`, `HomeController.cs:419, 429`). Server chỉ cần trả về Thể Lực hiện tại và số giây tới điểm tiếp theo làm mốc ban đầu. |
| **9** | [`GameplayController.cs:267`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L267) | `System.DateTimeOffset.UtcNow.UtcDateTime.ToString("yyyy-MM-dd")` | Ghi nhận ngày vượt ải đầu tiên (`DailyFirstClearDate`) để reset danh sách `DailyFirstClears`. | **Bắt buộc** | Server kiểm soát để tránh gian lận nhận thưởng đánh lại/thưởng ngày. Mốc ngày phải tính theo UTC+7 của Server. |
| **10** | [`HomeController.cs:401-405`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L401-L405) | `staminaService.Refresh(..., DateTimeOffset.UtcNow)` | Vòng lặp `Update` mỗi giây trên Home tự động hồi Thể Lực và tự gọi `SaveProfile()`. | **Bắt buộc bỏ** | Client không được tự chạy vòng lặp sửa hồ sơ và lưu đĩa nữa. Server gửi push update hoặc client chỉ tính giờ để render text. |
| **11** | [`StageMapPopup.cs:311, 426`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L311) | `stamina.Refresh`, `stamina.TrySpend` | Cập nhật Thể Lực và trừ Thể Lực khi bấm nút vào ải. | **Bắt buộc bỏ** | Chuyển thành gửi tin nhắn yêu cầu vào ải lên server. |
| **12** | [`ShopPopup.cs:15-22`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L15-L22) | `ShopRuntimeClock` (`offset += TimeSpan.FromDays`) | Đồng hồ ảo cho phép nút cheat "Sang tuần sau" hoạt động. | **Bỏ hoặc Server-only** | Mã debug của client, không áp dụng cho production online. |
| **13** | [`ArenaHistoryRow.cs:43`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaHistoryRow.cs#L43) | `TimeSpan age = DateTimeOffset.Now - at;` | Hiển thị thời gian tương đối ("5 phút trước", "2 giờ trước") cho lịch sử đấu. | **Giữ ở Client (UI)** | Thuần túy định dạng giao diện hiển thị, mốc `at` đã do server trả về. |
| **14** | [`ArenaLobbyPopup.cs:250`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L250) | `value.SeasonEndsAt - DateTimeOffset.Now` | Đếm ngược thời gian kết thúc Kỳ Luận Kiếm. | **Giữ ở Client (UI)** | Thuần túy đếm ngược hiển thị, mốc `SeasonEndsAt` do server trả về. |

---

## 4. Dữ Liệu Tĩnh Client và Server Cần Dùng Chung

Theo Quyết định [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7): *"Dữ liệu game (vật phẩm, nguyên liệu, ải, nhiệm vụ, Thương Hội, sức mạnh) vẫn là các file JSON có sẵn. Server đọc cùng bộ file đó, nên client và server không lệch số."*

Hiện tại, thư mục `server/battle-engine/src/main/resources/data/` mới chỉ có `items.json` và `sects.json`. Toàn bộ các file kinh tế và tiến trình khác trên Java server vẫn **chưa có**.

Bảng kiểm kê toàn bộ file JSON trong [`One-Bag-Jianghu/Assets/_Game/Resources/Data`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data):

| Tên file JSON | Lớp C# đọc trong Client | Hiện trạng Server Java | Server cần dùng để làm gì? |
|---|---|---|---|
| **1. `Data/items.json`** | `OneBagJianghu.Bag.ItemCatalog`<br>([`ItemCatalog.cs:61`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/ItemCatalog.cs#L61)) | **Đã có** (`com.onebagjianghu.battle.data.ItemCatalog`) | Xác thực ID món đồ, kích thước ô width/height, loại đồ (vũ khí, giáp, công pháp...), cấp mở Đột Phá, chỉ số engine. |
| **2. `Data/sects.json`** | `OneBagJianghu.Bag.SectCatalog`<br>([`SectCatalog.cs:193`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectCatalog.cs#L193)) | **Đã có** (`com.onebagjianghu.battle.data.SectCatalog`) | Xác thực môn phái hợp lệ, chỉ số nội tại, chiêu thức Tuyệt Kỹ và nạp Tâm Pháp trong engine chiến đấu. |
| **3. `Data/materials.json`** | `OneBagJianghu.Economy.MaterialCatalog`<br>([`GameSession.cs:403`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L403)) | **Chưa có** (Cần đưa vào server) | Định nghĩa toàn bộ nguyên liệu: nhóm loại (`weapon`, `armor`...), bậc (1–3), chất dẫn (`isCatalyst`), cờ `pvpOnly` (`chi_ton_ngoc_tuy`). Phục vụ nâng Trọng, đổi đồ, thưởng trận. |
| **4. `Data/upgrade_costs.json`** | `OneBagJianghu.Economy.UpgradeCatalog`<br>([`EconomyRuntime.cs:79`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/EconomyRuntime.cs#L79)) | **Chưa có** (Cần đưa vào server) | Bảng chi phí nâng Trọng 1–9 cho từng phẩm chất: lượng Bạc, bậc nguyên liệu, số lượng nguyên liệu thường, số lượng chất dẫn. Dùng để server trừ chi phí khi nâng Trọng và tính tiền/nguyên liệu hoàn lại khi bán hoặc Hợp Thành. |
| **5. `Data/merge.json`** | `OneBagJianghu.Economy.MergeCatalog`<br>([`EconomyRuntime.cs:81`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/EconomyRuntime.cs#L81)) | **Chưa có** (Cần đưa vào server) | Bảng quy tắc Hợp Thành: phẩm đầu vào -> phẩm đầu ra (`toRarity`), phí Bạc, phụ liệu yêu cầu (`extraMaterials`). Server kiểm tra và trừ chi phí khi Hợp Thành. |
| **6. `Data/shop.json`** | `OneBagJianghu.Economy.ShopCatalog`<br>([`EconomyRuntime.cs:99`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/EconomyRuntime.cs#L99)) | **Chưa có** (Cần đưa vào server) | Bảng giá gốc theo phẩm (`itemBasePrices`), hệ số loại đồ (`itemTypePriceMultipliers`, vũ khí x2 theo [#53](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L222)), 6 quy tắc sinh ô (`slots`), phí làm mới (`paidRefreshCost = 200`), quy tắc đổi nguyên liệu (`exchange`), danh mục Tiệm Danh Vọng (`reputationOffers`). Server dùng để sinh ô hàng, mua bán, đổi nguyên liệu. |
| **7. `Data/levels.json`** | `OneBagJianghu.Economy.LevelConfig`<br>([`LevelService.cs:41`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L41)) | **Chưa có** (Cần đưa vào server) | Cấp tối đa (`maxLevel = 30`), bảng EXP cần để lên từng cấp (`expToNext`). Server dùng để cộng EXP và tính cấp độ người chơi. |
| **8. `Data/starter.json`** | `OneBagJianghu.Economy.StarterKit`<br>([`GameSession.cs:64, 382`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Core/GameSession.cs#L64)) | **Chưa có** (Cần đưa vào server) | Cấu hình bộ khởi đầu cho tài khoản mới đăng ký: 500 Bạc ([#48](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L290)), Thanh Phong Kiếm + Ô Tàm Y (Trọng 1, Phàm) xếp sẵn trong Bao Phục. Server dùng khi tạo tài khoản mới trong DB. |
| **9. `Data/power.json`** | `OneBagJianghu.Economy.PowerCatalog`<br>([`PowerCatalog.cs:39`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PowerCatalog.cs#L39)) | **Chưa có** (Cần đưa vào server) | Công thức Lực Chiến v4 theo số ô ([#61](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L141)). Server dùng để tự tính Chiến Uy của ghost khi người chơi khiêu chiến Luận Kiếm (không tin Chiến Uy client gửi nữa theo [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L118)). |
| **10. `Data/quests.json`** | `OneBagJianghu.Quests.QuestCatalog`<br>([`QuestCatalog.cs:71`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestCatalog.cs#L71)) | **Chưa có** (Cần đưa vào server) | Chuỗi nhiệm vụ Hành Tẩu Lục (điều kiện hoàn thành, loại mục tiêu, phần thưởng Bạc, EXP, nguyên liệu). Server dùng để kiểm tra điều kiện hoàn thành và phát thưởng. |
| **11. `Data/Pve/chapters.json`** | `OneBagJianghu.Pve.PveCatalog`<br>([`PveCatalog.cs:205`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveCatalog.cs#L205)) | **Chưa có** (Cần đưa vào server) | Dữ liệu toàn bộ 5 Hồi (30 ải): thông tin quái từng ải (loadout đồ, cấp độ, Tâm Pháp quái), phần thưởng lần đầu, tỉ lệ thưởng đánh lại (15%), danh sách đồ rơi và tỉ lệ. Server dùng để chạy mô phỏng trận PvE và trao thưởng. |
| **12. `Data/Pve/stamina.json`** | `OneBagJianghu.Pve.StaminaConfig`<br>([`PveProgress.cs:17`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L17)) | **Chưa có** (Cần đưa vào server) | Thể Lực tối đa (`max = 60`), tốc độ hồi phục (`regenPerMinute = 0.16666667` tức 1 điểm / 6 phút). Server dùng để tính hồi Thể Lực. |
| **13. `Data/arena.json`** | `OneBagJianghu.UI.Arena.ArenaRanks`<br>([`ArenaRanks.cs:43`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaRanks.cs#L43)) | Đang hardcode trong Java | Các mốc điểm Elo và tên Bậc Luận Kiếm (1000: Vô Danh, 1100: Tân Tú, 1250: Thiếu Hiệp, 1400: Đại Hiệp, 1600: Danh Túc, 1800: Tông Sư). Cần đồng bộ file chung. |
| **14. `Data/intro.json`, `story.json`, `tutorial.json`** | `OnboardingCatalogs.cs`<br>([`OnboardingCatalogs.cs:25, 67, 133`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/OnboardingCatalogs.cs#L25)) | **Chưa có** | Server chỉ cần danh sách ID để xác thực client gửi sự kiện hoàn thành hướng dẫn hoặc xem truyện có hợp lệ không. Nội dung text/thoại giữ ở client. |
| **15. `Data/Samples/*.json`** | Samples đội hình | **Đã có** (`server/battle-engine/samples/`) | 24 đội hình mẫu PvP / bot test. |
| **16. `Data/Audio/*`** | `Audio/SfxData.cs` | Không cần | Dữ liệu âm thanh thuần túy client. |

---

## 5. Đề Xuất Nhóm Tin Nhắn Giao Thức (C↔S) và Cơ Chế Đồng Bộ

Tiếp nối phong bì JSON chuẩn của giao thức hiện có trong [`docs/tech/protocol.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/tech/protocol.md) (`{ "type": "...", "id": n, "payload": { ... } }`), các thao tác client được gom thành 7 nhóm tin nhắn mạch lạc:

### 5.1. Bảng Nhóm Tin Nhắn Đề Xuất

```
1. Nhóm Đăng Nhập & Hồ Sơ (Auth & Sync)
   C→S: login / register / resume (đã có ở #68)
   C→S: profile_get         Payload: {}
   S→C: profile_sync        Payload: <FullProfile>

2. Nhóm Bao Phục & Tàng Bảo Các (Loadout & Storage)
   C→S: loadout_save        Payload: { "placements": [ { "uid": "...", "x": 0, "y": 0 } ] }
   S→C: loadout_save_ok     Payload: { "profile": <FullProfile> }
   C→S: storage_move        Payload: { "kind": "item"|"material", "uid": "...", "materialId": "...", "fromX": 0, "fromY": 0, "toX": 1, "toY": 2 }
   S→C: storage_move_ok     Payload: { "profile": <FullProfile> }
   C→S: storage_repack      Payload: {}
   S→C: storage_repack_ok   Payload: { "profile": <FullProfile> }
   C→S: item_upgrade        Payload: { "uid": "..." }
   S→C: item_upgrade_ok     Payload: { "profile": <FullProfile>, "upgradedUid": "...", "nextTier": 4 }
   C→S: item_merge          Payload: { "firstUid": "...", "secondUid": "..." }
   S→C: item_merge_ok       Payload: { "profile": <FullProfile>, "resultUid": "...", "refund": { "materials": {...} } }
   C→S: item_sell           Payload: { "uid": "..." }
   S→C: item_sell_ok        Payload: { "profile": <FullProfile>, "silverEarned": 450, "refund": { "materials": {...} } }

3. Nhóm Thương Hội (Shop)
   C→S: shop_status         Payload: {}
   S→C: shop_status_ok      Payload: { "shop": <ShopState>, "secondsToFreeRefresh": 7200, "paidRefreshCost": 200 }
   C→S: shop_refresh        Payload: { "free": true|false }
   S→C: shop_refresh_ok     Payload: { "profile": <FullProfile> }
   C→S: shop_buy            Payload: { "slotIndex": 0 }
   S→C: shop_buy_ok         Payload: { "profile": <FullProfile>, "slotIndex": 0 }
   C→S: shop_exchange       Payload: { "inputId": "...", "targetId": "..." }
   S→C: shop_exchange_ok    Payload: { "profile": <FullProfile> }
   C→S: shop_buy_reputation Payload: { "offerId": "..." }
   S→C: shop_buy_reputation_ok Payload: { "profile": <FullProfile> }

4. Nhóm Trận PvE & Thể Lực (PvE Battle & Progression)
   C→S: pve_battle_start    Payload: { "stageId": "hoi_1_01" }
   S→C: pve_battle_result   Payload: { 
                               "stageId": "hoi_1_01", 
                               "result": <BattleResult>, 
                               "won": true, 
                               "firstClear": false, 
                               "reward": { "silver": 15, "heroExp": 9, "materials": { "han_thiet": 1 }, "items": [] },
                               "profile": <FullProfile> 
                            }
   S→C: stamina_sync (chủ động hoặc kèm tin): Payload: { "stamina": 55, "max": 60, "secondsToNext": 180 }

5. Nhóm Môn Phái & Tâm Pháp (Sect & Tam Phap)
   C→S: sect_choose         Payload: { "sectId": "kim_cang" }
   S→C: sect_choose_ok      Payload: { "profile": <FullProfile> }
   C→S: tam_phap_upgrade    Payload: {} (dự phòng)
   S→C: tam_phap_upgrade_ok Payload: { "profile": <FullProfile> }

6. Nhóm Nhiệm Vụ, Cốt Truyện, Hướng Dẫn (Quests & Onboarding)
   C→S: quest_claim         Payload: { "questId": "quest_1" }
   S→C: quest_claim_ok      Payload: { "profile": <FullProfile>, "reward": { ... } }
   C→S: tutorial_complete   Payload: { "sequenceId": "first_battle" }
   S→C: tutorial_complete_ok Payload: { "profile": <FullProfile> }
   C→S: intro_complete      Payload: {}
   S→C: intro_complete_ok   Payload: { "profile": <FullProfile> }
   C→S: story_mark_seen     Payload: { "chapter": 1 }
   S→C: story_mark_seen_ok  Payload: { "profile": <FullProfile> }

7. Nhóm Luận Kiếm Đài PvP (Arena - đã có ở #68, #69, #70)
   C→S: arena_find / arena_cancel / arena_status / leaderboard / arena_history / arena_replay
   S→C: arena_searching / arena_result / arena_status_ok / leaderboard_ok / arena_history_ok / arena_replay_ok
   (Điểm thay đổi: arena_result sẽ trả kèm <FullProfile> thay vì client tự cộng reward như trước!).
```

---

### 5.2. Đề Xuất Cơ Chế Đồng Bộ: Toàn Bộ Hồ Sơ (`Full Profile`) hay Phần Thay Đổi (`Delta`)?

**Khuyến nghị dứt khoát của Gemini: Server nên trả TOÀN BỘ HỒ SƠ (`Full Profile`) sau mỗi thao tác.**

**Lý do phân tích chi tiết:**
1. **Dung lượng cực nhỏ (Payload Size):**
   - Một hồ sơ `PlayerProfile` đầy đủ của One Bag Jianghu gồm: 1 đối tượng hiệp khách (cấp, exp, phái), danh sách khoảng 20–40 món đồ (`items`), mảng Bao Phục 5x4 (tối đa 20 món), mảng kho 10x20 (`storage`), 6 ô shop, tiến độ 30 ải PvE và trạng thái quest.
   - Khi serialize thành JSON không thụt dòng (compact JSON), kích thước chỉ dao động từ **2.5 KB đến 4.5 KB**. Khi nén gzip/WebSocket permessage-deflate chỉ còn **dưới 1 KB**.
   - Băng thông này là hoàn toàn không đáng kể so với việc chạy WebSocket liên tục trong môi trường mạng hiện đại.
2. **Loại bỏ triệt để rủi ro lệch dữ liệu (Zero Desynchronization):**
   - Các thao tác kinh tế võ hiệp có hiệu ứng lây lan (cascade effect) rất phức tạp: ví dụ thao tác **Nâng Trọng** đồng thời thay đổi: trừ Bạc trong `Currencies`, trừ nguyên liệu ở nhiều stack khác nhau trong `Storage`, cập nhật `Materials`, tăng `Tier` của món trong `Items`, tăng bộ đếm `Quest.ActionCounters["item_upgrade"]`, có thể làm hoàn thành nhiệm vụ dẫn đến đổi `Quest.ActiveId`.
   - Nếu áp dụng cơ chế Delta (chỉ gửi phần thay đổi), client và server sẽ phải viết logic merge delta phức tạp cho từng mảng, từng dictionary. Chỉ cần một sai sót nhỏ hoặc mất gói/lệch thứ tự xử lý, kho đồ hoặc tiền tệ client sẽ bị lệch vĩnh viễn trong suốt phiên chơi.
   - Trả Full Profile đảm bảo tính **toàn vẹn tuyệt đối**: Server gửi gì, Client hiển thị đúng như vậy.
3. **Đơn giản hóa mã nguồn Client:**
   - Client chỉ cần đúng một hàm tiếp nhận: `session.ApplyProfile(newProfile)`. Sau đó phát sự kiện `PlayerProfile.NotifyProgressChanged()`, toàn bộ các màn hình UI (`HomeController`, `BagLoadoutPanel`, `StoragePopup`, `QuestTracker`...) tự động re-bind dữ liệu chuẩn xác 100%. Không cần viết hàng chục hàm xử lý delta phức tạp.
4. **Đúng tinh thần Quyết định [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7):** *"Client gửi yêu cầu, nhận hồ sơ mới rồi phát lại trận. Client không còn tự tính phần thưởng."*

---

## 6. Nhận Diện Rủi Ro Kỹ Thuật

### 6.1. Logic Phức Tạp Khó Chuyển Sang Java
1. **Thuật toán lưới không gian Tàng Bảo Các (`StorageGrid.cs`):**
   - Logic tìm ô trống đầu tiên (`TryFindFirst`), kiểm tra chồng lấn (`CanPlace`), xử lý kích thước đồ đa dạng (`width x height`: 1x1, 1x2, 2x2, 1x4...), dồn kho và gộp stack nguyên liệu (`MergeStacks`, `Repack`) với trần `MaxStack = 999`.
   - Cần viết lại toàn bộ lớp `StorageGrid` sang Java, đảm bảo thuật toán quét ô từ trên xuống dưới, từ trái sang phải (`row 0..rows, col 0..cols`) khớp từng ô với C#. Nếu server và client tính lệch nhau 1 ô, server sẽ từ chối giao dịch với lỗi "Tàng Bảo Các không đủ chỗ" gây ức chế cho người chơi.
2. **Hệ thống cửa hàng Thương Hội (`ShopService.cs`):**
   - Công thức tính giá vũ khí x2 ([#53](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L222)), làm tròn số thập phân (`Math.Round` theo chuẩn `MidpointRounding.AwayFromZero` hay chuẩn Java `BigDecimal`), tính chiết khấu giảm giá.
   - Thuật toán bốc thăm ngẫu nhiên theo trọng số (`Weighted`) cho phẩm chất và loại slot.
   - PRNG salt và hashing: Client đang dùng `seed * 486187739 + salt` với `seed = HeroName.GetHashCode()`. Trong Java, hàm `String.hashCode()` có công thức tính khác với .NET `string.GetHashCode()`. **Cần dùng thuật toán băm chuỗi ổn định độc lập nền tảng (`StableHash`)** thay vì gọi trực tiếp `GetHashCode()`.
3. **Mô phỏng trận PvE trên Server (`PveBattleService.java`):**
   - Hiện tại Java server mới chỉ có `BattleSimulator` (chạy trận giữa 2 loadout). Server chưa có lớp đọc `chapters.json` (`PveCatalog.java`) để nạp quái vật từng ải, chưa có logic bốc tỉ lệ thưởng đánh lại 15% (`PveRewardService.java`) và tỉ lệ rơi đồ (`RollDrops`).
   - Cần port toàn bộ `PveCatalog` và `PveRewardService` sang Java để server tự nạp quái, tự chạy seed, tự sinh `BattleResult` và tự tính gói thưởng `PveRewardResult`.

### 6.2. Các Chỗ Client Đang Giả Định Có Hồ Sơ Ngay (Đồng Bộ)
1. **Đóng túi lưu Bao Phục tức thì (`BagPopup.cs:59-69`):**
   - Hiện tại khi người chơi bấm nút đóng (X), bấm phím Escape, hoặc bấm ra vùng mờ (backdrop), hàm `SaveAndClose()` lập tức gọi `bagPanel.TryApplyToProfile()` rồi `session.SaveProfile()` đồng bộ ngay trong frame đó, sau đó đóng popup liền.
   - Khi chuyển sang online, việc lưu Bao Phục là một yêu cầu mạng bất đồng bộ (`loadout_save`). Nếu người chơi vừa bấm đóng túi rồi ấn ngay vào nút Đấu PvE hoặc Tìm Trận Luận Kiếm, client có thể gửi đội hình cũ nếu server chưa kịp phản hồi xong!
   - *Khắc phục:* Phải có cờ `isSaving` hoặc spinner chờ server xác nhận `loadout_save_ok` trước khi cho phép vào trận.
2. **Thao tác một chạm trong Tàng Bảo Các và Shop (`StoragePopup.cs`, `ShopPopup.cs`):**
   - Các nút Nâng Trọng, Hợp Thành, Bán, Mua Shop, Đổi Nguyên Liệu hiện tại đều thực thi đồng bộ, phát âm thanh SFX và cập nhật UI ngay lập tức.
   - Khi chuyển sang mạng, cần khóa (disable) các nút hành động trong thời gian chờ gói tin phản hồi từ server để chống người chơi spam click liên tục (double-spend / duplicate requests).
3. **Vòng lặp tự động hồi Thể Lực (`HomeController.cs:134-137, 395-406`):**
   - Hiện tại hàm `Update()` trên màn hình Home cứ mỗi 1 giây lại tự gọi `staminaService.Refresh` và tự gọi `session.SaveProfile()` ghi đĩa.
   - Khi lên server, hành vi này phải bị vô hiệu hóa hoàn toàn. Client chỉ dùng giờ máy để đếm ngược hiển thị text trên thanh trạng thái.

### 6.3. Danh Sách Các Bộ Kiểm Thử Sẽ Bị Ảnh Hưởng (Broken Tests)
Nếu các dịch vụ kinh tế trong `OneBagJianghu.Economy` bị xóa bỏ hoặc chuyển đổi thành network stub, các bộ unit test EditMode hiện có của Unity sẽ bị vỡ:
1. [`EconomyServicesTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs): Kiểm tra trực tiếp `UpgradeService`, `MergeService`, `SellService`, `RewardService` trên client.
2. [`ShopServiceTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/ShopServiceTests.cs): Kiểm tra logic mua bán, đổi nguyên liệu, tính giá của `ShopService`.
3. [`StorageGridTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/StorageGridTests.cs) & [`MaterialStackTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/MaterialStackTests.cs): Kiểm tra thuật toán xếp kho và chia/gộp stack.
4. [`PveSystemTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Pve/PveSystemTests.cs): Kiểm tra `StaminaService.Refresh`, `TrySpend`, và `PveRewardService.Grant`.
5. [`QuestSystemTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Quests/QuestSystemTests.cs): Kiểm tra `QuestService.Progress`, `Claim` và kiểm tra điều kiện nhiệm vụ.
6. [`PlayerProfileTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/PlayerProfileTests.cs): Kiểm tra serialization, validate và migration dữ liệu từ v1 đến v9 trên đĩa máy khách.

**Khuyến nghị chiến lược chuyển đổi kiểm thử:**
- **Không vội xóa các lớp Service C#**: Giữ lại các lớp `UpgradeService`, `MergeService`, `ShopService` ở dạng logic pure C# để làm cơ sở tính toán hiển thị trước (UI Preview / Client Preflight), nhờ đó giữ nguyên được các bộ Unit Test trên client.
- **Port toàn bộ logic sang Java Test**: Viết các test JUnit tương đương trong `server/game-server/src/test/java` để kiểm thử độc lập tính đúng đắn của logic kinh tế phía server.
- **Bổ sung Net Test**: Viết các bài test giả lập gửi tin nhắn JSON `C→S` và xác thực gói tin `S→C` trả về khớp chính xác trạng thái mong đợi.
