# Luồng màn hình và kiến trúc UI (client Unity)

Quyết định #16. Tài liệu này là khung chung cho Codex (code), Gemini (thiết kế nội dung từng màn) và ChatGPT (hình nền, khung UI).

## Luồng

```
LoginScene ──► LoadingPanel ──► HomeScene ──► LoadingPanel ──► GameplayScene
                                   ▲                                 │
                                   └──────── LoadingPanel ◄──────────┘ (thoát trận / hết trận)
```

- **LoginScene** (scene khởi động, index 0 trong Build Settings): logo, đăng nhập. Chưa có server tài khoản nên tạm là nhập tên Hiệp Khách + nút "Vào Giang Hồ", lưu cục bộ. **Không chọn môn phái** (mở ở cấp 10, quyết định #21).
- **LoadingPanel**: **không phải scene**. Một Canvas duy nhất, `DontDestroyOnLoad`, luôn nằm trên cùng mọi UI (`sortingOrder` cao nhất, ví dụ 1000). Hiện khi chuyển scene: nền che toàn màn hình, thanh tiến độ, câu dẫn/mẹo ngẫu nhiên, mờ dần khi xong. Chặn mọi thao tác (raycast) khi đang hiện.
- **HomeScene**: sảnh chính. Vào các chế độ và các màn phụ (dạng panel trong cùng scene, không tách scene): Hành Tẩu Giang Hồ (PvE), Luận Kiếm Đài (PvP), Bao Phục (xếp túi), Tàng Bảo Các (kho), Vạn Bảo Phổ, Thương Hội, cài đặt.
- **GameplayScene**: một trận, **không chỉnh túi** (quyết định #21): lấy đội hình đã xếp ở Home → tính trận (offline: engine C#) → phát lại → bảng kết quả → về Home hoặc trận tiếp.

## Kiến trúc code (`Assets/_Game/Scripts/`)

| Thành phần | Vai trò |
|---|---|
| `Core/GameBootstrap` | Tạo các dịch vụ tồn tại suốt game (LoadingPanel, SceneFlow, dữ liệu phiên) một lần duy nhất, dù Play từ scene nào trong Editor |
| `Core/SceneFlow` | API duy nhất để chuyển scene: `SceneFlow.Go(GameScene.Home)`. Hiện LoadingPanel → `LoadSceneAsync` → chờ scene mới báo sẵn sàng → ẩn LoadingPanel. Không nơi nào khác gọi `SceneManager.LoadScene` |
| `UI/Loading/LoadingPanel` | Canvas `DontDestroyOnLoad`, tiến độ 0..1, `Show()` / `Hide()` có mờ dần |
| `UI/Login`, `UI/Home`, `UI/Gameplay` | Controller từng scene. Mỗi scene có một bộ điều khiển gốc, báo `SceneFlow` khi đã dựng xong |
| `Bag/`, `Battle/Replay/` | Tái dùng trong GameplayScene và panel Bao Phục của Home |

Truyền dữ liệu giữa các scene qua một đối tượng phiên (`GameSession`: tên Hiệp Khách, môn phái, đội hình hiện tại, chế độ trận vừa chọn), không dùng biến tĩnh rải rác.

## Popup

Quyết định #17. Mọi màn phụ dùng **popup**: chỉ tạo khi cần gọi, không dựng sẵn trong scene.

- Mỗi popup là một **prefab** trong `Assets/_Game/Resources/UI/Popups/<TênLớp>.prefab`, lớp kế thừa `BasePopup`. Prefab được sinh bằng menu editor từ code để chỉnh tiếp trong Editor khi có art.
- `PopupManager` (dịch vụ trong gốc `DontDestroyOnLoad`, có Canvas riêng `sortingOrder` 100–199):
  - `PopupManager.Show<T>(data)` nạp prefab lần đầu, các lần sau dùng lại bản đã ẩn (cache), gọi `Setup(data)` rồi `Show()`.
  - Quản lý **ngăn xếp**: popup mới nằm trên popup cũ; nền mờ dùng chung nằm ngay dưới popup trên cùng.
  - Phím **Esc** / nút Back đóng popup trên cùng (nếu popup cho phép).
  - Khi chuyển scene thì đóng hết popup.
- Vòng đời `BasePopup`: `Setup(data)` → `Show()` (hiện + hiệu ứng) → `OnShown` → … → `Close()` / `Hide()` → `OnHidden` → sự kiện `Closed`. Có `OnEnable` / `OnDisable` để đăng ký/huỷ sự kiện, `OnBackPressed()` quyết định Esc có đóng không, `Refresh()` để vẽ lại khi dữ liệu đổi.
- Popup hiện có / sẽ có: `MessagePopup` (thông báo 1 nút), `ConfirmPopup` (Đồng ý / Huỷ), `SettingsPopup`, `BagPopup` (Bao Phục), `ResultPopup` (kết quả trận). LoadingPanel **không** phải popup.

## Scene và prefab (quyết định #20)

- **Mở scene trong Editor là thấy đủ giao diện.** Mỗi scene có cây rõ ràng:
  ```
  <Tên>Scene
  ├── Main Camera (nếu cần)
  ├── EventSystem              (InputSystemUIInputModule)
  ├── <Tên>Controller          (script điều khiển, chỉ tham chiếu)
  └── Canvas                   (Screen Space Overlay, 1920×1080, match 0.5, sortingOrder 0)
      ├── Background
      ├── Header / Content / Footer ... (các vùng của màn)
      └── ...
  ```
- **Script không dựng UI.** Controller/View dùng `[SerializeField]` để trỏ tới nút, chữ, panel có sẵn; gắn sự kiện trong `Awake`/`OnEnable`. Không `new GameObject`, không `AddComponent<Image/Text/Button>` lúc chạy.
- **Phần động từ prefab:** ô vật phẩm, dòng danh sách, hình VFX, dòng nhật ký… mỗi loại một prefab (`Assets/_Game/Prefabs/UI/...`), code chỉ `Instantiate` rồi gọi `Bind(data)`.
- **Phần dùng chung:** nút, khung, tiêu đề theo phong cách Thủy Mặc là prefab dùng lại (`Prefabs/UI/Common/`), để khi có art chỉ thay một chỗ.
- **Popup:** prefab trong `Resources/UI/Popups/` chứa **đủ nội dung** (không sinh thêm lúc chạy).
- **Dịch vụ xuyên scene:** `Resources/Core/Services.prefab` (LoadingPanel canvas, PopupManager canvas, SceneFlow, GameSession, NetworkService). `GameBootstrap` chỉ `Instantiate` prefab này một lần.
- **Menu dựng trong Editor** chỉ tạo khi còn thiếu; bản đã có là bản gốc, không bị ghi đè. Menu ghi đè riêng có hộp xác nhận.
- Scene demo/công cụ (BagDemo, ReplayDemo, VfxLab) theo cùng quy ước.

## Hiển thị

- **Độ phân giải chuẩn 1920×1080 (16:9)**. CanvasScaler "Scale With Screen Size", `matchWidthOrHeight = 0.5` để màn 16:10 chỉ thêm lề, không méo. Tối thiểu 1280×720. Mặc định toàn màn hình, có tùy chọn cửa sổ.
- **Thứ tự lớp** (`sortingOrder`): nội dung scene 0–99, popup 100–199 (mỗi popup trong ngăn xếp +2), thông báo 200–299, **LoadingPanel 1000**.
- Phong cách Thủy Mặc (quyết định #11): nền giấy, khung mực, son đỏ nhấn. Chữ dùng font hỗ trợ tiếng Việt đầy đủ dấu (chọn font ở bước sau; hiện dùng font mặc định).

## Chưa làm
- Đăng nhập thật (server tài khoản), âm thanh, hiệu ứng chuyển cảnh cầu kỳ, bản địa hóa.
