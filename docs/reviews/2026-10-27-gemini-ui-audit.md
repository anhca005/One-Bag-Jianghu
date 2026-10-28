# Báo Cáo Rà Soát UI Còn Dở Hoặc Chưa Chỉn Chu (UI Audit) - Nhất Nang Giang Hồ

**Ngày:** 2026-10-27  
**Người thực hiện:** Gemini (UI/UX Reviewer)  
**Phạm vi:** Toàn bộ giao diện người dùng UGUI, mã nguồn controller (`Scripts/UI/**`, `Scripts/Onboarding/**`, `Scripts/Battle/Replay/View/**`), bộ dựng giao diện (`Editor/Building/*Builder.cs`), tài nguyên hình ảnh (`Textures2D/**`, `Resources/*Library.asset`) và tài liệu thiết kế (`docs/design/**`, `docs/decisions.md`).  
**Mục tiêu:** Chuẩn bị cho bản demo người chơi thật; đảm bảo toàn bộ màn hình hoạt động trơn tru, không có lỗi kẹt game, tuân thủ kiến trúc client-server (#70/#71), quy ước ảnh Simple (#66), và thuật ngữ `glossary.md`.

---

## I. Bảng Tổng Hợp Vấn Đề Theo Màn Hình

| Màn hình | Cao (Lỗi/Thiếu trầm trọng) | Vừa (Thiếu chỉn chu) | Thấp (Chi tiết nhỏ) | Tổng cộng |
|---|:---:|:---:|:---:|:---:|
| **1. Login & Tài khoản** | 1 | 2 | 2 | 5 |
| **2. Home (Sảnh chính)** | 1 | 2 | 2 | 5 |
| **3. Bao Phục (Túi đồ)** | 0 | 2 | 3 | 5 |
| **4. Tàng Bảo Các (Kho đồ)** | 1 | 2 | 2 | 5 |
| **5. Thương Hội (Cửa hàng)** | 1 | 2 | 2 | 5 |
| **6. Bản đồ ải (Hành Tẩu Giang Hồ)** | 2 | 1 | 2 | 5 |
| **7. Trận đấu (Chiến đấu & Replay)** | 1 | 3 | 0 | 4 |
| **8. Kết quả (Thắng/Thua)** | 1 | 1 | 1 | 3 |
| **9. Nhiệm vụ (Hành Tẩu Lục)** | 1 | 1 | 2 | 4 |
| **10. Chọn phái (Bái Sư)** | 1 | 2 | 0 | 3 |
| **11. Sảnh Luận Kiếm (Đấu Trường)** | 2 | 1 | 1 | 4 |
| **12. Cài đặt (Hệ Thống)** | 0 | 0 | 2 | 2 |
| **13. Bách Khoa (Vạn Bảo Phổ)** | 0 | 1 | 1 | 2 |
| **14. Hướng dẫn & Dẫn truyện** | 1 | 0 | 1 | 2 |
| **15. Popup chung & Toàn cục** | 0 | 1 | 1 | 2 |
| **Tổng cộng** | **13** | **21** | **22** | **56** |

---

## II. Chi Tiết Rà Soát Từng Màn Hình

### 1. Màn hình Đăng Nhập & Tài Khoản (Login & AccountPopup)

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs:68-69](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs#L68-L69)**
  - **Vấn đề:** Văn bản hướng dẫn ghi sai lệch kiến trúc dữ liệu: `"Tài khoản dùng cho Luận Kiếm Đài. Đồ đạc vẫn lưu trên máy này."`. Kể từ Quyết định #70/#71, toàn bộ tiến trình, đồ đạc và hồ sơ nhân vật đều được đồng bộ và lưu trữ tập trung trên server. Câu thông báo này sẽ khiến người chơi hoang mang khi chuyển thiết bị hoặc lo sợ mất dữ liệu.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Đổi câu thông báo thành `"Tài khoản giúp lưu trữ hành trang và tiến trình giang hồ an toàn trên máy chủ."`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs:62-73](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs#L62-L73)**
  - **Vấn đề:** Khi bấm nút "Vào Giang Hồ" hoặc đăng nhập tài khoản, coroutine `ConnectAndAuthenticate()` gọi kết nối mạng với thời gian chờ tối đa 3 giây nhưng không có vòng xoay tải (loading spinner), không đổi nhãn nút thành `"Đang kết nối..."` và không khoá nút `enterButton`. Người chơi dễ bấm liên tục (spam click) gây lỗi chồng yêu cầu kết nối mạng.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Vô hiệu hoá `enterButton.interactable = false`, đổi nhãn sang `"Đang kết nối..."` và bật biểu tượng chờ xoay tròn; nếu kết nối thất bại thì khôi phục lại trạng thái ban đầu.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs:59-60](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs#L59-L60)** & **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs:28](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/LoginController.cs#L28)**
  - **Vấn đề:** Trường nhập tên `heroName` được dựng dưới dạng InputField với gợi ý `"Tên Hiệp Khách"`, nhưng trong `LoginController.cs:28` lại bị gán `readOnly = true`. Người chơi click vào nhưng bàn phím không nhận, gây hiểu lầm là trường nhập liệu bị hỏng.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Nếu game chưa cho phép tự đặt tên mà dùng tên ngẫu nhiên/tên tài khoản, hãy chuyển component từ InputField sang nhãn Text hiển thị thông tin kèm nút xúc xắc "Đổi tên ngẫu nhiên", hoặc cho phép gõ trực tiếp nếu muốn người chơi tự đặt tên hiệp khách.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs:66-67](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs#L66-L67)**
  - **Vấn đề:** Hai nút bấm đặt cạnh nhau trong cùng một hàng của `AccountPopup` nhưng kích thước cọc cạch: nút "Đồng ý" kích thước `260x62`, trong khi nút "Để sau" là `240x57`. Giao diện trông thiếu cân xứng và không chỉn chu.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Đồng bộ kích thước cả hai nút về cùng một chuẩn kích thước (ví dụ `240x58` hoặc `260x60`).

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/AccountPopup.cs:113-116](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Login/AccountPopup.cs#L113-L116)**
  - **Vấn đề:** Khi server từ chối đăng ký/đăng nhập (ví dụ: tên tài khoản đã tồn tại, mật khẩu ngắn), thông báo lỗi gán thẳng vào `errorMessage.text` nhưng khung chữ hẹp (độ cao cố định, cỡ chữ 18). Khi chuỗi thông báo từ server dài hoặc mất mạng, chữ dễ bị cắt mép hoặc tràn khung.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Bật tính năng `verticalOverflow = Truncate` kèm `Best Fit` (min 13, max 18) hoặc mở rộng khung hiển thị thông báo lỗi.

---

### 2. Trang Chủ / Sảnh Chính (Home Scene)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs:78, 245-248](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L78)**
  - **Vấn đề:** Điểm chạm Vạn Bảo Phổ (`codexHotspot`) khi bấm vào chỉ bật một hộp thoại thông báo `"Sắp ra mắt"` (`Vạn Bảo Phổ đang được biên soạn, xin quay lại sau`). Tuy nhiên, trong mã nguồn đã hoàn thiện đầy đủ giao diện Bách Khoa `EncyclopediaPopup` hiển thị các mục Quy tắc, Trạng thái, Cốt truyện. Để dòng chữ "Sắp ra mắt" trong bản demo người chơi thật làm giảm độ hoàn thiện của sản phẩm.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Đổi lệnh gọi trong `OpenCodex()` từ hiển thị `MessagePopup` sang `PopupManager.Show<EncyclopediaPopup>()`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs:197-215](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L197-L215)**
  - **Vấn đề:** Hàm bấm vào Luận Kiếm Đài (`StartPvp`) thực hiện kết nối server bất đồng bộ `network.ConnectAsync(TimeSpan.FromSeconds(3))` nhưng không hiển thị bất kỳ chỉ báo đang tải nào và không vô hiệu hoá điểm chạm. Người chơi bấm nhiều lần liên tiếp sẽ kích hoạt nhiều tác vụ mạng song song.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Khoá tương tác giao diện và hiển thị loading indicator `"Đang kết nối Luận Kiếm Đài..."` trong khi chờ `ConnectAsync`.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs:115-135](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs#L115-L135)**
  - **Vấn đề:** Theo Quyết định thiết kế #21 (`docs/decisions.md`), màn hình Home cần có một nút lớn "Vào trận" đặt ở cạnh đáy màn hình và cảnh báo cấu hình túi đồ đặt ngay trên nút bấm. Bản dựng Home hiện tại chỉ có các điểm chạm (hotspot) gắn trên tranh vẽ nền, người chơi mới khó nhận biết ngay lối vào ải chính nếu không đọc kỹ hướng dẫn.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Bổ sung nút hành động chính (Primary CTA) "Vào Trận" hoặc "Hành Tẩu" nổi bật ở góc dưới bên phải màn hình theo đúng định hướng thiết kế #21.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs:137-140](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L137-L140)** vs **[HomeController.cs:295-297](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L295-L297)**
  - **Vấn đề:** Không nhất quán nhãn nút đóng của `MessagePopup`: thông báo mở khoá môn phái (dòng 140) dùng nhãn `"Đã hiểu"`, trong khi thông báo tính năng chưa mở (dòng 297) dùng nhãn `"Đóng"`.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Thống nhất dùng `"Đã hiểu"` cho toàn bộ các popup thông báo thông tin mang tính tiếp nhận trong game.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs:126-130](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs#L126-L130)**
  - **Vấn đề:** Chỉ số Thể Lực hiển thị dạng ghép chuỗi thô `staminaText.text = $"{stamina}/{max}";` mà không qua hàm định dạng số `UiFormat.Number`.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Sửa thành `staminaText.text = $"{UiFormat.Number(stamina)}/{UiFormat.Number(max)}";`.

---

### 3. Bao Phục (Túi Đồ & Xếp Đồ - BagPopup & BagLoadoutPanel)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs:70-88](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L70-L88)**
  - **Vấn đề:** Khi người chơi đóng túi đồ bằng nút đóng hoặc phím Esc, phương thức `Close()` kích hoạt việc lưu hành trang lên server (`SaveLoadout`). Nút đóng bị vô hiệu hoá (`closeButton.interactable = false`) nhưng không có chữ `"Đang lưu hành trang..."` hay biểu tượng xoay chờ. Khi mạng chậm 1–2 giây, người chơi tưởng game bị đơ hoặc nút bấm bị liệt.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Hiển thị dòng trạng thái nhỏ `"Đang lưu..."` hoặc icon loading cạnh nút đóng để báo hiệu cho người chơi biết hệ thống đang đồng bộ dữ liệu.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs:95](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BagPopup.cs#L95)**
  - **Vấn đề:** Khi server từ chối lưu túi đồ (ví dụ xếp đè ô, dữ liệu không hợp lệ), popup báo lỗi dùng tiêu đề chung chung là `"Thông báo"`, và hiển thị chuỗi thông báo kỹ thuật trực tiếp từ server.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Đổi tiêu đề popup thành `"Sắp Xếp Không Hợp Lệ"` hoặc `"Lỗi Lưu Hành Trang"` và dịch chuỗi phản hồi sang văn phong kiếm hiệp dễ hiểu.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs:282-291](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L282-L291)**
  - **Vấn đề:** Chỉ số Khí Huyết tối đa `stats.maxHp` hiển thị số trần không có dấu phân cách hàng nghìn (ví dụ: `1500` thay vì `1.500`), vi phạm quy chuẩn định dạng số của dự án.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Dùng `UiFormat.Number(stats.maxHp)` khi gán text Khí Huyết.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs:340](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L340)**
  - **Vấn đề:** Nhãn Lực Chiến ghi `"Lực Chiến: "` (có dấu hai chấm), trong khi trên giao diện Home (`HomeController.cs:122`) lại ghi `"Lực Chiến "` (không có dấu hai chấm). Quy cách hiển thị văn bản không đồng bộ giữa các màn hình.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Bỏ dấu hai chấm, chuẩn hoá thành `"Lực Chiến: {0}"` hoặc `"Lực Chiến {0}"` thống nhất toàn game theo `glossary.md`.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/BagPrefabBuilder.cs:160-175](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/BagPrefabBuilder.cs#L160-L175)**
  - **Vấn đề:** Lưới túi đồ và khay đồ dự phòng (`stash`) không có dòng chữ gợi ý phím tắt xoay đồ (phím `R`). Người chơi mới không biết cách xoay đồ để xếp vừa vặn vào lưới.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Thêm một dòng chữ nhỏ ở góc dưới túi đồ: `"Nhấn [R] khi kéo trang bị để xoay hướng"`.

---

### 4. Tàng Bảo Các (Kho Đồ, Nâng Trọng & Hợp Thành - StoragePopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs:567-573](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L567-L573)**
  - **Vấn đề:** **Lỗi nuốt mất thông báo lỗi (Bug nghiêm trọng):** Khi server báo lỗi thao tác (nâng Trọng, hợp thành hoặc bán đồ thất bại), hàm `FailNetwork(message)` gán `costText.text = message;` rồi ngay lập tức gọi `Refresh();`. Trong `Refresh()` (dòng 186-187), lệnh gọi `Select(selectedUid)` ghi đè lại `costText.text = BuildUpgradeCost(preview);`. Hậu quả là thông báo lỗi của server bị xoá sạch ngay trong cùng một khung hình, người chơi bấm nút nhưng không thấy có phản hồi nào và không thể biết tại sao thao tác bị từ chối!
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Không dùng `costText` để báo lỗi mạng tạm thời; hãy mở một popup thông báo lỗi rõ ràng bằng `PopupManager.Show<MessagePopup>(new MessagePopupData("Thất Bại", message, "Đã hiểu"))` hoặc giữ chuỗi lỗi trong một nhãn cảnh báo riêng biệt không bị `Refresh` ghi đè.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs:116-173](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L116-L173)**
  - **Vấn đề:** Khi kho đồ rỗng (người chơi đã chuyển hết trang bị vào túi hoặc vừa tạo tài khoản), lưới ô kho hoàn toàn trống trơn, không có dòng chữ hay hình ảnh hướng dẫn trạng thái rỗng ("Kho trống").
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Thêm nhãn trạng thái rỗng ở giữa lưới kho: `"Tàng Bảo Các hiện chưa có bảo vật. Hãy thu thập từ Hành Tẩu hoặc Thương Hội."`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs:541-564](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L541-L564)**
  - **Vấn đề:** Khi Nâng Trọng hoặc Hợp Thành thành công, hệ thống chỉ phát một âm thanh click nhẹ (`ui_click`), không có bất kỳ hiệu ứng thị giác nào (hiệu ứng ánh sáng, thông báo nổi hay popup chúc mừng món đồ đạt Phẩm/Trọng mới). Cảm giác thành tựu khi nâng cấp đồ bị mờ nhạt.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Thêm hiệu ứng rung/sáng nhẹ trên ô trang bị vừa nâng cấp và phát âm thanh thành công đặc trưng (`sfx_upgrade_success`), kèm thông báo nhỏ: `"Bảo vật đã đạt Trọng X!"`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs:181-185](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L181-L185)**
  - **Vấn đề:** Câu thông báo `"Cảnh báo: có dữ liệu đang chờ trong overflow."` chứa nguyên từ tiếng Anh kỹ thuật `"overflow"`, phá vỡ không gian kiếm hiệp cổ trang.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Đổi thành: `"Cảnh báo: Có vật phẩm trong hàng chờ nhận đồ do kho đầy."`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs:410-435](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L410-L435)**
  - **Vấn đề:** Khi người chơi thiếu Bạc hoặc thiếu nguyên liệu để Nâng Trọng / Đột Phá, nút `upgradeButton` bị tắt (`interactable = false`) nhưng dòng chữ chi phí không đổi màu đỏ cảnh báo thiếu nguyên liệu nào cụ thể, khiến người chơi phải tự tính nhẩm xem mình đang thiếu tài nguyên gì.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Đổi màu đỏ (`Color.red` hoặc màu mực son kiếm hiệp) đối với riêng loại nguyên liệu hoặc số Bạc mà người chơi hiện không đủ số lượng.

---

### 5. Thương Hội (Cửa Hàng Bạc, Tiệm Danh Vọng, Đổi Nguyên Liệu - ShopPopup & ShopCard)

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs:706](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs#L706)** & **[ShopCard.cs](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopCard.cs)**
  - **Vấn đề:** Trong prefab thẻ mua hàng `ShopCard.prefab`, biểu tượng tiền tệ của giá bán bị gắn cố định là sprite đồng tiền Bạc (`icon_bac.png`). Khi người chơi chuyển sang tab **Tiệm Danh Vọng**, thẻ bán vật phẩm bằng Danh Vọng vẫn hiển thị hình đồng tiền Bạc kế bên dòng chữ `"Danh Vọng 800"`. Người chơi sẽ tưởng đây là vật phẩm mua bằng Bạc.
  - **Mức độ:** **Cao** *(cần ảnh)*
  - **Đề xuất sửa:** Thêm trường `Image currencyIcon` vào `ShopCard.cs`; khi nạp thẻ mua bằng Danh Vọng thì hoán đổi sprite thành icon huy hiệu Danh Vọng tương ứng.
  - **Mô tả ảnh cần vẽ:** Biểu tượng phù hiệu Danh Vọng (`icon_danh_vong.png`), kích thước 32x32 hoặc 48x48, phong cách huân chương đồng/ngọc võ hiệp.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs:389-406](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L389-L406)**
  - **Vấn đề:** Trong tab Đổi Nguyên Liệu, danh sách chọn nguyên liệu trong Dropdown chỉ hiện tên món (ví dụ: `Huyền Thiết`) mà không hiển thị số lượng người chơi đang có trong túi/kho (ví dụ: `Huyền Thiết (Có: 0)`). Nút Đổi (`exchangeButton`) vẫn sáng và cho bấm kể cả khi người chơi không có nguyên liệu, dẫn đến việc bấm vào bị server từ chối kèm lỗi.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Hiển thị số lượng sở hữu ngay trong từng dòng dropdown: `"{Tên} (Có: {Số lượng})"`; tự động tắt nút Đổi (`interactable = false`) khi số lượng sở hữu nhỏ hơn tỷ lệ quy đổi.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs:302-308](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L302-L308)**
  - **Vấn đề:** Nút "Làm Mới" danh mục hàng hoá không hiển thị mức phí Bạc cần tiêu tốn trực tiếp trên nhãn nút hoặc bên cạnh nút. Người chơi bấm vào mới bị trừ tiền mà không được báo trước chi phí làm mới.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Đổi nhãn nút thành `"Làm Mới ({chi phí} Bạc)"` và tự động mờ nút khi số dư Bạc không đủ.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs:412](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L412)**
  - **Vấn đề:** Dòng chữ trạng thái giao dịch `statusText` (`"Giao dịch thành công."`) không được xoá trắng khi người chơi bấm chuyển đổi giữa các tab Thương Hội, dẫn đến thông tin cũ lưu cữu trên giao diện tab mới.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Xoá sạch nội dung `statusText.text = string.Empty;` trong hàm chuyển tab `ShowTab()`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs:220-224](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ShopPopup.cs#L220-L224)**
  - **Vấn đề:** Khi một món hàng đã được mua hết, thẻ hàng chỉ giảm độ mờ (`canvasGroup.alpha = 0.5f`) mà không có con dấu đóng "ĐÃ BÁN" phong cách kiếm hiệp, trông như món hàng bị vô hiệu hoá do lỗi hiển thị.
  - **Mức độ:** **Thấp** *(cần ảnh)*
  - **Đề xuất sửa:** Đóng một con dấu mực đỏ nghiêng góc 15 độ ghi chữ "ĐÃ BÁN" đè lên giữa thẻ hàng đã mua.
  - **Mô tả ảnh cần vẽ:** Con dấu triện cổ `"ĐÃ BÁN"` (`seal_sold_out.png`), mực chu sa đỏ kiếm hiệp, tỉ lệ vuông hoặc chữ nhật 120x60.

---

### 6. Bản Đồ Ải (Hành Tẩu Giang Hồ - StageMapPopup)

- **[One-Bag-Jianghu/Assets/_Game/Resources/PveArtLibrary.asset](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/PveArtLibrary.asset)** & **[StageMapPopup.cs:305-306](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L305-L306)**
  - **Vấn đề:** **Thiếu 18 ảnh chân dung địch (Lỗi tài nguyên ảnh nghiêm trọng):** Thư viện `PveArtLibrary.asset` hoàn toàn thiếu 18 key ảnh chân dung quái cho Hồi 4 (từ ải 31 đến 39, key `enemy_hoi_4_01` đến `enemy_hoi_4_09`) và Hồi 5 (từ ải 41 đến 49, key `enemy_hoi_5_01` đến `enemy_hoi_5_09`). Khi người chơi chọn các ải này trên bản đồ, dòng code 305-306 gán sprite `null` và vẽ một ô vuông màu xám xịt `Color(0.2f, 0.2f, 0.2f, 0.3f)`. Bản demo người chơi trải nghiệm đến Hồi 4 và 5 sẽ thấy khung ảnh quái bị tối đen không có hình vẽ!
  - **Mức độ:** **Cao** *(cần ảnh)*
  - **Đề xuất sửa:** Bổ sung 18 file ảnh chân dung tương ứng vào `Textures2D/Pve` và đăng ký vào `PveArtLibrary.asset`; trước mắt gán fallback tạm thời về các sprite quái cùng hệ phái của Hồi 2 và 3 để tránh hiển thị ô đen.
  - **Mô tả ảnh cần vẽ:** 18 ảnh chân dung kẻ địch và quái vật cho Hồi 4 (Miêu Cương/Tây Vực) và Hồi 5 (Tuyệt Đỉnh Hoa Sơn/Cấm Địa), kích thước 256x256 phong cách thủy mặc võ hiệp.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs:303-304](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L303-L304)**
  - **Vấn đề:** Dòng phần thưởng trên bảng thông tin ải luôn hiển thị: `"Lần đầu: {reward.Silver} Bạc · {reward.HeroExp} EXP"`, kể cả với các ải mà người chơi đã hoàn thành từ trước! Theo Quyết định thiết kế #47 (`docs/decisions.md`), khi đánh lại ải đã vượt thì chỉ nhận 15% phần thưởng; nhưng UI vẫn đề nhãn "Lần đầu" kèm đầy đủ 100% số thưởng, khiến người chơi tưởng mình bị trừ thưởng vô lý sau khi thắng trận.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Kiểm tra trạng thái vượt ải của người chơi: nếu là lần đầu thì hiện `"Thưởng lần đầu: ..."`, nếu đã vượt thì hiện `"Thưởng quét lại (15%): {reward.Silver * 0.15} Bạc · {reward.HeroExp * 0.15} EXP"`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs:355-366](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L355-L366)**
  - **Vấn đề:** Khi bấm nút "Khiêu Chiến" (`challengeButton`), hàm gọi `GameBootstrap.Network.StartPveBattle` bất đồng bộ nhưng không bật biểu tượng tải trận đấu và không vô hiệu hoá nút. Người chơi có thể bấm liên tiếp làm gửi nhiều gói tin bắt đầu trận.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Khoá nút `challengeButton.interactable = false`, đổi chữ nút thành `"Đang vào trận..."` và hiển thị vòng quay tải.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs:180-184](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L180-L184)**
  - **Vấn đề:** Nút chuyển sang Hồi kế tiếp (`nextButton`) bị tắt (`interactable = false`) khi Hồi đó chưa mở khoá, nhưng không có thông báo hay tooltip giải thích lý do (ví dụ: cần vượt ải bao nhiêu để mở Hồi sau).
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Cho phép bấm vào nút bị khoá để hiển thị tooltip/toast nhỏ: `"Cần hoàn thành Ải {X} Hồi {Y} để mở Hồi này."`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs:301-304](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs#L301-L304)**
  - **Vấn đề:** Điểm EXP, Thể Lực (`reward.HeroExp`, `stage.StaminaCost`, `staminaState.Stamina`) hiển thị dạng số trần, không qua hàm định dạng phân tách hàng nghìn `UiFormat.Number`.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Bổ sung `UiFormat.Number()` cho tất cả các giá trị số hiển thị trên popup bản đồ.

---

### 7. Trận Đấu (Chiến Đấu & Replay - GameplayController, ReplayScreen, FighterPanelView)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs:260-282](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs#L260-L282)**
  - **Vấn đề:** Chân dung đối thủ trong trận đấu (`ResolvePortrait`) chỉ băm mã hash mod 3 + 1 để lấy ngẫu nhiên 1 trong 3 ảnh chung `enemy_portrait_1..3`, hoặc fallback về một khối trắng 1x1 pixel (`CreateSilhouette()`, dòng 284-300). Toàn bộ ảnh chân dung đặc thù của Boss và quái vật trong `chapters.json` (`boss_bui_chen`, `monster_heo_rung`, `boss_dai_duong_chu`, v.v.) hoàn toàn bị bỏ qua trong màn hình chiến đấu! Cả trùm lớn lẫn quái nhỏ vào trận đều mang cùng một chân dung generique hoặc một khối bóng mờ trắng xám.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Truyền mã chân dung thực tế của đối thủ/Boss từ dữ liệu PVE hoặc PvP vào `FighterPanelView` và nạp ảnh từ `PveArtLibrary` thay vì băm ngẫu nhiên 3 ảnh mặc định.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/ReplayScreen.cs](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/ReplayScreen.cs)**
  - **Vấn đề:** Trong suốt quá trình diễn ra trận đấu tự động, màn hình replay không bắt sự kiện phím `Escape`. Người chơi trên PC có thói quen bấm Esc để tạm dừng trận đấu hoặc mở bảng xác nhận đầu hàng/thoát trận, nhưng bấm Esc hiện tại không có phản hồi gì.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Bắt phím Esc trong `Update()` của `ReplayScreen` để kích hoạt popup xác nhận đầu hàng/thoát nhanh về sảnh.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs:134-148](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs#L134-L148)**
  - **Vấn đề:** Thanh Khí Huyết và thanh Cương Khí (thanh vàng) không có chữ số hiển thị giá trị hiện tại / tối đa (ví dụ: `240/300`), người chơi chỉ thấy thanh co lại mà không theo dõi được chính xác lượng máu cụ thể trong giao tranh.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Thêm nhãn số Khí Huyết và Cương Khí đè lên giữa thanh đo với định dạng `UiFormat.Number(cur) + "/" + UiFormat.Number(max)`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/ReplayItemView.cs:100-112](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/ReplayItemView.cs#L100-L112)**
  - **Vấn đề:** Khi vũ khí/trang bị kích hoạt xuất chiêu, ô đồ chỉ nảy kích thước nhẹ, không có chữ số sát thương hoặc hồi phục nổi lên trên đầu trang bị (floating combat text), khiến việc quan sát hiệu quả của từng món đồ trong túi khi auto-battle rất khó khăn.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Bổ sung hiệu ứng số nổi (Floating Numbers) nhảy lên từ ô trang bị khi gây sát thương hoặc hồi phục để tăng cảm giác uy lực cho trận đánh.

---

### 8. Màn Hình Kết Quả Trận Đấu (ResultPopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs:226-227](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs#L226-L227)** & **[GameplayController.cs:361](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L361)**
  - **Vấn đề:** **Lỗi treo kẹt game vĩnh viễn (Game-Breaking Bug):** `ResultPopup` kế thừa `BasePopup` có `closeOnEscape = true`. Khi người chơi bấm phím `Esc` trên bàn phím tại màn hình kết quả trận đấu, `BasePopup.OnBackPressed()` sẽ gọi `Close()` mà không gán kết quả lựa chọn (`popup.Result` là `null`). Tại `GameplayController.cs:361`, dòng kiểm tra:
    ```csharp
    if (!(popup.Result is ResultPopupChoice choice)) return;
    ```
    Lập tức `return` mà không chuyển scene! Kết quả là popup kết quả biến mất, và **người chơi bị kẹt vĩnh viễn trên một màn hình GameplayScene trống rỗng**, không có nút bấm, không thể về Home, không thể về Bản đồ ải, buộc phải tắt cưỡng bức ứng dụng bằng Alt+F4.
  - **Mức độ:** **Cao (Tối khẩn cấp)**
  - **Đề xuất sửa:** Override lại phương thức `OnBackPressed()` trong `ResultPopup.cs` để khi người chơi bấm Esc sẽ mặc định chọn quay về Bản đồ ải (PVE) hoặc quay về Sảnh Luận Kiếm (PVP) thông qua `CloseWithResult(ResultPopupChoice.Map)`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs:122-125](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs#L122-L125)**
  - **Vấn đề:** Tiêu đề thắng thua chỉ hiển thị dòng chữ đơn giản `"ĐẮC THẮNG"` hoặc `"THẤT BẠI"` bằng font chữ thông thường, thiếu hình ảnh huy hiệu chiến thắng hoặc thư pháp kiếm hiệp hào hùng.
  - **Mức độ:** **Vừa** *(cần ảnh)*
  - **Đề xuất sửa:** Thay thế text chữ trơn bằng huy hiệu thư pháp võ hiệp "Đắc Thắng" (viền rồng/hoàng kim) và "Thất Bại" (mực xám/vỡ nứt).
  - **Mô tả ảnh cần vẽ:** 2 huy hiệu thư pháp khổ ngang 400x120: `badge_victory.png` và `badge_defeat.png`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs:150-165](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/ResultPopup.cs#L150-L165)**
  - **Vấn đề:** Số tiền Bạc và điểm EXP nhận được sau trận đấu hiện ra ngay lập tức dưới dạng số tĩnh, không có hoạt ảnh nhảy số đếm dần (counter animation) hay hiệu ứng ánh kim.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Cho số Bạc và EXP nhảy dần từ 0 lên giá trị đạt được trong 0.5 giây kèm âm thanh đếm tiền vui tai.

---

### 9. Nhiệm Vụ (Hành Tẩu Lục - QuestLogPopup & QuestTracker)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs:182-189](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L182-L189)**
  - **Vấn đề:** **Hiển thị ID kỹ thuật snake_case ra màn hình:** Phương thức `GiverName(string id)` chỉ xử lý trường hợp cho `"lao_tieu_phu"` và `"phong_loi"`. Trong tệp dữ liệu `quests.json` có nhiệm vụ được giao bởi `"tieu_linh_nhi"` và `"yen_vo_tam"`. Khi mở các nhiệm vụ này, giao diện in thẳng chữ thô `"tieu_linh_nhi"` và `"yen_vo_tam"` lên vị trí tên NPC giao nhiệm vụ!
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Bổ sung nhánh `case "tieu_linh_nhi": return "Tiểu Linh Nhi";` và `case "yen_vo_tam": return "Yến Vô Tâm";` vào hàm `GiverName()`.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/QuestUiBuilder.cs:63](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/QuestUiBuilder.cs#L63)**
  - **Vấn đề:** Nút mở sổ nhiệm vụ trên khung theo dõi ở Home dùng hàm `AddPlainButton` với ký tự thô `"☰"` và hình nút chữ nhật xám mặc định của Unity UI, hoàn toàn lệch tông với toàn bộ bộ giao diện cổ trang giấy da của game.
  - **Mức độ:** **Vừa** *(cần ảnh)*
  - **Đề xuất sửa:** Thay nút trơn bằng nút có sprite biểu tượng cuốn sổ/cuộn thẻ tre kiếm hiệp.
  - **Mô tả ảnh cần vẽ:** Icon nút mở Hành Tẩu Lục (`icon_quest_book.png`), hình thẻ tre hoặc quyển bí kíp thu nhỏ, kích thước 48x48.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogRow.cs:76-79](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogRow.cs#L76-L79)** & **[QuestLogPopup.cs:175-178](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L175-L178)**
  - **Vấn đề:** Số tiền Bạc thưởng (`reward.Silver.ToString()`), điểm EXP (`reward.HeroExp + " EXP"`) và tiến độ nhiệm vụ (`progress.Current + "/" + progress.Target`) đều không dùng định dạng số `UiFormat.Number`.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Bọc các biến số bằng `UiFormat.Number()`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs:115-135](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs#L115-L135)**
  - **Vấn đề:** Khi người chơi đã hoàn thành và nhận thưởng tất cả nhiệm vụ khả dụng, danh sách nhiệm vụ trống trơn, không có thông báo trạng thái rỗng để hướng dẫn bước tiếp theo.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Thêm thông báo rỗng: `"Đã hoàn thành toàn bộ công việc giang hồ hiện tại. Hãy tiếp tục vượt ải để mở nhiệm vụ mới!"`.

---

### 10. Chọn Phái (Bái Nhập Môn Phái - SectSelectPopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs:172, 269-272](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L172)**
  - **Vấn đề:** Khi người chơi đã gia nhập môn phái, nút hành động chính của popup hiển thị nhãn: `"Nâng Tâm Pháp (Sắp mở)"`. Khi bấm vào nút này, hàm xử lý `OpenTamPhapUpgrade()` là một hàm rỗng chỉ chứa ghi chú `// TODO: Open martial arts / heart law upgrade panel`. Chữ "Sắp mở" và nút bấm không có phản hồi vi phạm trực tiếp yêu cầu chỉn chu của bản demo.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Nếu tính năng Nâng Tâm Pháp chưa có trong bản demo, hãy ẩn nút này đi hoặc đổi nhãn thành `"Đã Nhập Phái"` và tắt `interactable = false`; nếu có popup nâng cấp thì liên kết mở giao diện.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs:187-188](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L187-L188)**
  - **Vấn đề:** Khi không tải được hình huy hiệu môn phái, code vẽ tạm một khối chữ nhật màu xám trơn `new Color(0.18f, 0.15f, 0.12f, 0.5f)` không viền, làm giao diện bị thô ráp.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Tạo một khung tranh mặc định hoa văn võ hiệp để làm nền khi thiếu sprite huy hiệu phái.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs:220-230](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs#L220-L230)**
  - **Vấn đề:** Khi bấm nút "Gia Nhập" môn phái, hệ thống gửi lệnh qua mạng `GameBootstrap.Network.JoinSect` nhưng không vô hiệu hoá các nút bấm chọn môn phái khác trên danh sách, cho phép người chơi bấm liên tục đổi phái trong khi gói tin cũ chưa phản hồi.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Khoá toàn bộ danh sách môn phái và hiển thị trạng thái `"Đang bái nhập môn phái..."` trong thời gian chờ server phản hồi.

---

### 11. Sảnh Luận Kiếm (Đấu Trường PVP - ArenaLobbyPopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs:391-392, 561-566](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L391-L392)**
  - **Vấn đề:** **Đối thủ dùng chân dung của người chơi:** Trong màn hình ghép trận đối đầu (`ShowVersusAfter`), cả hai khung chân dung nhân vật người chơi và nhân vật đối thủ đều gọi hàm `BindPortrait`:
    ```csharp
    BindPortrait(versusPlayerPortrait, "hero_portrait");
    BindPortrait(versusOpponentPortrait, "hero_portrait");
    ```
    Cả 2 bên đều nạp cùng chuỗi `"hero_portrait"`. Kết quả là trên võ đài, người chơi luôn thấy đối thủ có gương mặt y hệt nhân vật của chính mình, tạo cảm giác lỗi phân thân rất buồn cười và mất tính nhập vai.
  - **Mức độ:** **Cao** *(cần ảnh)*
  - **Đề xuất sửa:** Gán chân dung đối thủ theo môn phái của đối thủ hoặc dùng ảnh chân dung hiệp khách ngẫu nhiên/avatar đối thủ PVP riêng biệt.
  - **Mô tả ảnh cần vẽ:** Ít nhất 4 chân dung đối thủ Luận Kiếm đại diện cho các môn phái (`pvp_avatar_1..4.png`), kích thước 256x256.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs:164-166](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/OnlinePrefabBuilder.cs#L164-L166)**
  - **Vấn đề:** Văn bản hướng dẫn phần thưởng chứa thông tin sai lệch so với Quyết định thiết kế #72 (`docs/decisions.md`) và chứa chữ "sắp có":
    ```csharp
    "10 thắng tuần → 1 Chí Tôn Ngọc Tủy\nThưởng cuối mùa theo bậc\nTrao cuối mùa, sắp có"
    ```
    Quyết định #72 đã quy định rõ: mục tiêu tuần là **15 trận thắng**, và **huỷ bỏ cơ chế thưởng theo bậc phân hạng**, chỉ trao thưởng theo vị trí Top trên Bảng Xếp Hạng. Đoạn text tĩnh này vừa cung cấp sai thông tin cho người chơi vừa chứa chữ "sắp có" dở dang.
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Cập nhật nội dung theo đúng Quyết định #72: `"15 trận thắng tuần → 1 Chí Tôn Ngọc Tủy\nThưởng cuối mùa: Vinh danh Top 10 Bảng Xếp Hạng"`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs:277-287, 301-311](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L277-L287)**
  - **Vấn đề:** Bảng Xếp Hạng (`Leaderboard`) và Lịch Sử Đấu (`History`) khi chưa có dữ liệu (`Entries.Count == 0`) không có thông báo trạng thái rỗng, màn hình để lại một khoảng trống hoang vắng.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Hiển thị dòng trạng thái rỗng: `"Chưa có dữ liệu xếp hạng mùa giải này."` cho BXH và `"Chưa có chiến tích luận kiếm nào. Hãy tham gia khiêu chiến!"` cho Lịch sử.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs:205, 256-258, 393-396](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Arena/ArenaLobbyPopup.cs#L205)**
  - **Vấn đề:** Số lượng Chí Tôn Ngọc Tủy (`jadeText.text = jade.ToString()`), Điểm Đánh Giá Rating, Số trận Thắng/Thua và Lực Chiến trong màn đối đầu đều hiển thị số trần không phân cách hàng nghìn.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Dùng `UiFormat.Number()` cho tất cả các thông số điểm và tiền tệ trên sảnh Luận Kiếm.

---

### 12. Cài Đặt (Hệ Thống - SettingsPopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs:56-68](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs#L56-L68)**
  - **Vấn đề:** Dòng chữ ghi công âm thanh `AudioCredits` (`"Nhạc: Richard Guo · Tiếng động: Kenney (CC0)"`) được sinh động trong code runtime và ép neo cứng sát đáy màn hình (`anchoredPosition = (0, 8)`). Với các màn hình có tỉ lệ khác 16:9 (như 16:10 hoặc siêu rộng 21:9), dòng chữ này bị cấn sát vào cạnh viền hoặc bị che khuất.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Đưa dòng credits vào trực tiếp trong `PopupPrefabBuilder.cs` với neo cách đáy an toàn ít nhất 24px.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs:276-278](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SettingsPopup.cs#L276-L278)**
  - **Vấn đề:** Khi chưa đăng nhập (hoặc chạy ngắt mạng), nhãn hiển thị `"Chưa đăng nhập"` và nút ghi `"Đăng nhập"`. Bấm vào sẽ mở `AccountPopup` đè lên trên `SettingsPopup` tạo ra nhiều lớp popup chồng chéo phức tạp.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Khi mở `AccountPopup` từ Settings, hãy ẩn hoặc đóng `SettingsPopup` để tránh việc đóng mở popup lồng nhau.

---

### 13. Bách Khoa (Vạn Bảo Phổ - EncyclopediaPopup)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/EncyclopediaPopup.cs:121-135](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/EncyclopediaPopup.cs#L121-L135)**
  - **Vấn đề:** Khi người chơi mới chưa vượt qua Hồi nào để mở khoá cốt truyện (`stories.Count == 0`), tab "Cốt truyện" hoàn toàn trống trơn. Tên và nội dung chi tiết vẫn lưu lại dữ liệu của danh mục xem trước đó, đồng thời nút "Xem Lại" (`replayButton`) không được ẩn đi một cách an toàn.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Khi `stories.Count == 0`, ẩn `replayButton`, xoá trắng phần mô tả và hiển thị thông báo: `"Chưa có hồi ức cốt truyện nào được ghi chép. Hãy hành tẩu giang hồ để mở khoá!"`.

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/EncyclopediaPopup.cs:165-179](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/EncyclopediaPopup.cs#L165-L179)**
  - **Vấn đề:** Việc gán biểu tượng cho từng danh mục phụ thuộc vào phép so khớp chuỗi con tiếng Việt (`"Trạng thái"`, `"vật phẩm"`, `"Hệ thống"`). Nếu tệp `encyclopedia.json` bổ sung danh mục mới không khớp các từ khoá này, hàm trả về `null` và dòng danh mục đó bị thụt lề khác biệt so với các dòng có icon.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Cung cấp icon mặc định (như biểu tượng cuốn sách hoặc thẻ bài) khi danh mục không khớp từ khoá, đảm bảo lề chữ luôn thẳng hàng.

---

### 14. Hướng Dẫn & Dẫn Truyện (Intro, Story & Tutorial Overlay)

- **[One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/IntroPopup.cs:30, 52-55](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/IntroPopup.cs#L30)** & **[BasePopup.cs:20](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/BasePopup.cs#L20)**
  - **Vấn đề:** **Phím Esc gây lặp lại Intro và kẹt nhạc nền u ám (BGM Stuck):** `IntroPopup` kế thừa `BasePopup` có `closeOnEscape = true`. Khi người chơi nhấn phím `Esc` để bỏ qua đoạn dẫn chuyện Intro, `BasePopup.OnBackPressed()` kích hoạt `Close()`. Tuy nhiên, lệnh gửi trạng thái đã xem intro lên server (`SendOnboarding("intro_seen", ...)`) chỉ được gọi bên trong hàm `Finish()`. Kết quả là cờ `IntroSeen` không hề được lưu lên server, lần sau đăng nhập người chơi lại bị ép xem lại Intro! Nghiêm trọng hơn, hàm chuyển nhạc nền về nhạc Home (`AudioManager.PlayMusic(MusicKeys.Home)`) nằm trong `CompleteFinish()`, nên khi bấm Esc, nhạc nền của game bị kẹt vĩnh viễn ở bài nhạc u ám `bgm_intro` thay vì chuyển sang nhạc rộn rã của Home!
  - **Mức độ:** **Cao**
  - **Đề xuất sửa:** Ghi đè `OnBackPressed()` trong `IntroPopup.cs` để gọi thẳng hàm `Finish()` thay vì đóng chay; đảm bảo luôn lưu cờ `intro_seen` và chuyển đổi nhạc nền về `MusicKeys.Home`.

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/PopupPrefabBuilder.cs:255](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/PopupPrefabBuilder.cs#L255)** & **[TutorialOverlay.cs:30](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialOverlay.cs#L30)**
  - **Vấn đề:** Chân dung người hướng dẫn trong khung thoại Tutorial bị gán cứng là ảnh của `npc_lao_tieu_phu.png` (Lão Tiều Phu) ngay từ khâu dựng prefab, mã nguồn không có cơ chế đổi chân dung người nói theo từng bước hướng dẫn.
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Thêm thuộc tính `speaker` vào từng bước trong `tutorial.json` và cập nhật ảnh chân dung tương ứng trong `TutorialOverlay.Refresh()`.

---

### 15. Popup Chung & Quy Chuẩn Toàn Cục (Common Popups & Global Flow)

- **[One-Bag-Jianghu/Assets/_Game/Editor/Building/PopupPrefabBuilder.cs:60](file:///D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/PopupPrefabBuilder.cs#L60)**
  - **Vấn đề:** Nút đóng của `MessagePopup` được gán nhãn mặc định là `"Đóng"`, nhưng các nơi gọi lại truyền nhãn tuỳ ý (`"Đã hiểu"`, `"Đóng"`, `"Xác nhận"`).
  - **Mức độ:** **Thấp**
  - **Đề xuất sửa:** Chuẩn hoá nhãn mặc định thành `"Đã hiểu"` theo đúng tinh thần tiếp nhận thông điệp võ hiệp trong `glossary.md`.

- **Toàn cục các nút tương tác mạng (Network Debounce & Action Feedback)**
  - **Vấn đề:** Toàn bộ các nút gọi server chính trong game (`challengeButton` ở Ải, `buyButton` ở Shop, `upgradeButton` ở Tàng Bảo Các, `joinSectButton` ở Môn Phái) chưa có cơ chế chống bấm liên tục (debounce / throttle). Khi mạng lag, người chơi click liên tục có thể gây phát sinh nhiều transaction ngoài ý muốn.
  - **Mức độ:** **Vừa**
  - **Đề xuất sửa:** Tạo một component tiện ích `UiNetworkButton` hoặc bao bọc thao tác bấm bằng việc tắt `interactable` ngay lập tức và chỉ mở lại khi nhận được callback hoàn thành/thất bại từ server.

---

## III. Danh Sách Hình Ảnh Cần Vẽ Thêm

Dưới đây là bảng tổng hợp toàn bộ các hình ảnh cần bổ sung cho hoạ sĩ (Artist), được phân loại theo mức độ ưu tiên:

| Tên ảnh (Key) | Kích thước dự kiến | Định dạng / Kiểu | Màn hình sử dụng | Mô tả chi tiết |
|---|:---:|:---:|---|---|
| **18 Chân dung địch Hồi 4 & 5**<br>(`enemy_hoi_4_01` .. `4_09`,<br>`enemy_hoi_5_01` .. `5_09`) | 256 × 256 | Sprite Simple (không chữ) | Bản đồ ải (`StageMapPopup`) & Trận đấu | 9 chân dung thích khách/dị tộc Miêu Cương Tây Vực (Hồi 4) và 9 chân dung cao thủ Tuyệt Đỉnh Hoa Sơn (Hồi 5). Hiện đang bị ô vuông màu xám tối. |
| **Chân dung đối thủ Luận Kiếm**<br>(`pvp_avatar_1` .. `4`) | 256 × 256 | Sprite Simple (không chữ) | Sảnh Luận Kiếm (`ArenaLobbyPopup`) | 4 chân dung hiệp khách đại diện các môn phái đối thủ, giải quyết triệt để lỗi đối thủ mang cùng gương mặt với người chơi. |
| **Biểu tượng Danh Vọng**<br>(`icon_danh_vong.png`) | 48 × 48 | Sprite Simple (không chữ) | Thương Hội (`ShopCard`) | Huân chương phù hiệu Danh Vọng võ lâm để thay thế cho icon đồng tiền Bạc đang bị gán sai ở Tiệm Danh Vọng. |
| **Huy hiệu Đắc Thắng & Thất Bại**<br>(`badge_victory.png`, `badge_defeat.png`) | 400 × 120 | Sprite Simple | Màn hình Kết quả (`ResultPopup`) | Thư pháp chữ Hán/Việt cách điệu viền rồng hoàng kim (Đắc Thắng) và mực xám vết nứt (Thất Bại), tăng tính hoành tráng sau trận. |
| **Con dấu Đã Bán / Hết Hàng**<br>(`seal_sold_out.png`) | 140 × 70 | Sprite Simple | Thương Hội (`ShopPopup`) | Con dấu triện chu sa đỏ chữ "ĐÃ BÁN" nghiêng 15 độ, đóng đè lên thẻ hàng đã mua hết. |
| **Biểu tượng Hành Tẩu Lục**<br>(`icon_quest_book.png`) | 64 × 64 | Sprite Simple (không chữ) | Khung theo dõi Home (`QuestTracker`) | Cuốn bí kíp hoặc thẻ tre cổ phong thay cho nút chữ nhật xám có ký tự "☰" mặc định. |

---

## IV. Top 10 Việc Nên Làm Trước Nhất Cho Bản Demo

*Đây là 10 hạng mục có độ ảnh hưởng trực tiếp nhất tới trải nghiệm của người chơi thật, được chọn lọc để ưu tiên xử lý dứt điểm trước ngày mở demo:*

1. **Khắc phục lỗi treo kẹt game vĩnh viễn ở màn hình Kết quả trận đấu (`ResultPopup.cs:226-227` & `GameplayController.cs:361`):**  
   Override `OnBackPressed()` trong `ResultPopup.cs` để khi người chơi bấm phím `Esc`, hệ thống tự động trả về `ResultPopupChoice.Map` (hoặc `Arena`), ngăn chặn hoàn toàn việc người chơi bị kẹt lại trên màn hình trống rỗng.

2. **Sửa lỗi nuốt mất thông báo lỗi server trong Tàng Bảo Các (`StoragePopup.cs:567-573`):**  
   Tách biệt thông báo lỗi của `FailNetwork` ra khỏi `costText` hoặc không để `Refresh()` ghi đè xóa sạch chuỗi lỗi ngay trong frame, đảm bảo người chơi đọc được lý do tại sao nâng Trọng / hợp thành thất bại.

3. **Sửa lỗi phím Esc ở màn hình Intro gây lặp chuyện và kẹt nhạc nền (`IntroPopup.cs:30, 52-55`):**  
   Đảm bảo khi bấm Esc thoát Intro, hệ thống vẫn gửi cờ `intro_seen` lên server và đổi nhạc nền sang `MusicKeys.Home`.

4. **Sửa lỗi lọt tên NPC dạng mã kỹ thuật tiếng Anh trong Hành Tẩu Lục (`QuestLogPopup.cs:182-189`):**  
   Bổ sung tên hiển thị tiếng Việt cho `"tieu_linh_nhi"` ("Tiểu Linh Nhi") và `"yen_vo_tam"` ("Yến Vô Tâm") trong hàm `GiverName()`.

5. **Cấu hình ảnh fallback cho 18 chân dung địch Hồi 4 và Hồi 5 trong `PveArtLibrary.asset`:**  
   Gán ảnh tạm thời từ quái Hồi 2/3 để xoá bỏ triệt để các ô vuông màu xám xịt khi người chơi trải nghiệm các ải từ 31 đến 49.

6. **Sửa lỗi hiển thị phần thưởng ải Hành Tẩu Giang Hồ (`StageMapPopup.cs:303-304`):**  
   Hiển thị đúng mức thưởng quét lại (15% theo Quyết định #47) đối với các ải đã vượt thay vì luôn ghi `"Lần đầu: 100% thưởng"`.

7. **Sửa lỗi đối thủ mang cùng chân dung người chơi trong Sảnh Luận Kiếm (`ArenaLobbyPopup.cs:391-392`):**  
   Không nạp cùng một key `"hero_portrait"` cho cả hai phía; nạp ảnh chân dung theo phái của đối thủ hoặc avatar đối thủ ngẫu nhiên.

8. **Cập nhật nội dung bảng thưởng Luận Kiếm theo đúng Quyết định #72 (`OnlinePrefabBuilder.cs:164-166`):**  
   Sửa mốc thắng tuần thành 15 trận, xoá dòng chữ `"Thưởng cuối mùa theo bậc - sắp có"` để giao diện chuẩn xác và không còn chữ "sắp có".

9. **Kết nối màn hình Bách Khoa (`EncyclopediaPopup`) vào điểm chạm Vạn Bảo Phổ ở Home (`HomeController.cs:78, 245-248`):**  
   Mở trực tiếp giao diện Vạn Bảo Phổ thay vì hiện thông báo cụt hứng `"Sắp ra mắt"`.

10. **Bổ sung trạng thái đang tải (Loading state) và khoá nút chống spam click cho các tác vụ mạng chính:**  
    Áp dụng ngay cho nút Đăng Nhập (`LoginController`), Vào Luận Kiếm (`HomeController`), Khiêu Chiến ải (`StageMapPopup`) và Đóng Túi Đồ (`BagPopup`).
