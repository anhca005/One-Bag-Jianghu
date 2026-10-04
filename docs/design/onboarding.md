# Mở đầu, hướng dẫn và Giang Hồ Bách Khoa

Quyết định #58. Người dùng chọn:
- **Intro:** tranh cuộn 4–6 khung.
- **Hướng dẫn:** 4 chuỗi (xếp túi + ải 1, Thương Hội, nâng Trọng, Môn Phái). Lục Tiều Phu dẫn, màn hình tối và khoanh sáng nút cần bấm.
- **Bách khoa:** một mục ở Home để tra thuật ngữ, chỉ cần chữ tĩnh.

## 1. Intro: "Tàn Nang"

**Khi nào hiện:**
- lần đầu vào Home sau khi tạo nhân vật;
- xem lại được trong Cài đặt → "Xem lại mở đầu".

**Cách hiện:**
- Mỗi khung là một tranh toàn màn hình (1536×1024, Thủy Mặc) kèm 1–2 câu chữ trên dải giấy ở đáy.
- Bấm hoặc nhấn Space để sang khung sau.
- Chuyển khung bằng mờ dần 0,4 s, tranh trôi chậm (Ken Burns nhẹ).
- Nút **Bỏ qua** ở góc phải trên.
- Xong intro thì vào thẳng chuỗi hướng dẫn 1.

| # | Tranh (`Textures2D/Intro/`) | Chữ |
|---|---|---|
| 1 | `intro_1` | Cửu Châu loạn thế. Bảy đại phái tranh hùng, còn bảo vật *Cửu Cực Càn Khôn Nang* thì vỡ thành nhiều mảnh, lưu lạc khắp giang hồ. |
| 2 | `intro_2` | Một đêm mưa lửa, U Minh Giáo tràn vào sơn trang nhà ngươi, chỉ để tìm một chiếc túi gấm cũ. |
| 3 | `intro_3` | Trước khi ngã xuống, phụ thân trao lại cho ngươi chiếc tàn nang: "Túi nhỏ mà chứa được cả giang hồ… nhớ lấy, xếp đặt mới là võ học." |
| 4 | `intro_4` | Ba năm sau, thiếu niên mang tàn nang rời núi. Trong túi chỉ có một thanh kiếm và một tấm áo. |
| 5 | `intro_5` | Bến Thanh Khê, mùa thu lá đỏ. Con đường giang hồ bắt đầu từ đây. |

**Prompt hình** (theo quy ước `docs/art/prompts-pve.md`, landscape 1536x1024, no text):
1. `Wide ink-wash panorama of ancient China in turmoil: seven mountain sect banners on distant peaks, a shattered glowing brocade pouch whose fragments scatter like stars across the night sky. Muted ink tones with faint gold.`
2. `A mountain manor attacked on a rainy night, burning roofs, dark-robed cultists with ghostly masks climbing the walls, red fire reflected in rain, ink-wash style, figures small and silhouetted, no gore.`
3. `Close interior scene by candlelight: a wounded father's hand placing a worn embroidered brocade pouch into a young boy's hands, rain outside the paper window, emotional and quiet, ink-wash style, faces partly hidden, no blood.`
4. `A young swordsman in a plain grey-blue robe walking down a misty mountain path at dawn, a small brocade pouch at his waist, a sword on his back, seen from behind, vast ink-wash mountains ahead.`
5. `An autumn riverside ferry town at morning: red maple trees, a wooden pier, a ferry boat, an old woodcutter with firewood waiting by the road, the young swordsman arriving, warm hopeful mood, ink-wash style.`

## 2. Hướng dẫn

**Hình thức:**
- Hộp thoại dùng lại `DialoguePopup` với chân dung **Lục Tiều Phu**.
- Một lớp tối phủ màn hình, có lỗ sáng quanh ô cần bấm. Chỉ ô đó nhận click; ô kéo thả thì chỉ ô nguồn và ô đích nhận.
- Có nút **Bỏ qua hướng dẫn** ở mỗi bước: bỏ qua thì đánh dấu chuỗi đó đã xong.
- Tiến độ lưu theo hồ sơ (cờ theo từng chuỗi), không hiện lại khi đã xong.
- Cài đặt có nút "Đặt lại hướng dẫn".

**Dữ liệu:** `Resources/Data/tutorial.json`. Mỗi chuỗi gồm id, điều kiện kích hoạt và danh sách bước. Mỗi bước gồm câu thoại, đích khoanh sáng (id đối tượng UI) và điều kiện xong bước (sự kiện game). Code không viết cứng câu chữ.

### Chuỗi 1: `first_battle` (sau intro, hồ sơ mới)
1. "Tiểu hữu, trước khi ra trận phải biết xếp túi. Mở **Bao Phục** ra xem." → khoanh nút Bao Phục; xong khi popup mở.
2. "Binh khí tự ra đòn theo nhịp, Hộ Cụ cho Cương Khí đỡ đòn. Kéo **Thanh Phong Kiếm** vào túi." → khoanh món kiếm trong kho và ô đích; xong khi đặt vào túi.
3. "Giờ đến **Ô Tàm Y**." → tương tự.
4. "Lực Chiến cho biết túi mạnh cỡ nào. Đóng túi lại, ta đi **Hành Tẩu Giang Hồ**." → khoanh nút Đóng, rồi nút Hành Tẩu.
5. "Ải đầu tiên: **Trấn Khẩu Sơn Đạo**. Xem Lực Chiến đề nghị rồi vào trận." → khoanh ải 1, rồi nút vào trận.
6. **Trong trận**, 3 câu ngắn hiện lần lượt, chỉ ở trận đầu:
   - lúc bắt đầu: "Đòn thường cộng **Nội Lực**; đủ Nội Lực sẽ tung **Tuyệt Kỹ**.";
   - lúc có Cương Khí lần đầu: "Thanh vàng là **Cương Khí**, bị trừ trước Khí Huyết.";
   - lúc có Hóa Kình lần đầu, hoặc 10 s nếu chưa có: "Đủ 60 Nội Lực, ngươi tự **Hóa Kình** gạt Tuyệt Kỹ của địch.".
   - Trận vẫn chạy tiếp, không cần bấm.
7. **Sau trận thắng:** "Thắng ải lần đầu được Bạc, EXP và nguyên liệu. Đánh lại được 15%." → xong.

### Chuỗi 2: `shop` (về Home sau khi thắng ải 1 lần đầu)
1. "Có Bạc rồi. Ghé **Thương Hội** xem hàng." → khoanh Thương Hội.
2. "Rê chuột lên món đồ để xem chi tiết. Binh khí giá gấp đôi, nhưng là thứ quyết định trận." → khoanh ô hàng đầu tiên; xong khi rê chuột hoặc mua.
3. "Mua một món, rồi vào Bao Phục xếp nó vào túi." → xong khi món mới được đặt vào túi.

### Chuỗi 3: `upgrade` (lần đầu đủ nguyên liệu và Bạc để nâng Trọng một món đang có)
1. "Nguyên liệu dùng để **nâng Trọng**. Vào **Tàng Bảo Các**." → khoanh Tàng Bảo Các.
2. "Chọn một món, bấm Nâng Trọng. Mỗi Trọng tăng chỉ số của món. Trọng 3, 6 và 9 cần thêm chất dẫn để **Đột Phá**." → khoanh nút Nâng; xong khi nâng thành công.
3. "Hai bản cùng món, cùng phẩm, từ Tam Trọng trở lên thì **Hợp Thành** được ra phẩm cao hơn." → xong.

### Chuỗi 4: `sect` (lần đầu đạt cấp 10)
1. "Cấp 10, đủ tư cách bái nhập **Môn Phái**." → khoanh nút Môn Phái.
2. "Mỗi phái có **Tâm Pháp** riêng, tăng sức cho các hệ binh khí của phái. Chọn phái hợp với túi của ngươi." → xong khi chọn phái hoặc đóng.

## 3. Giang Hồ Bách Khoa

- **Nút ở Home:** điểm nóng hoặc nút "Bách Khoa" cạnh Vạn Bảo Phổ.
- **Popup hai cột:**
  - bên trái là danh mục: Hệ thống, Loại vật phẩm, Độ hiếm, Nâng cấp & nguyên liệu, Chỉ số & cơ chế, Trạng thái;
  - bên phải là danh sách mục, bấm vào thì hiện tên và mô tả.
- **Dữ liệu:** `Resources/Data/encyclopedia.json`, nội dung lấy từ [glossary.md](glossary.md), viết lại cho người chơi đọc.
  - Bỏ mã code, bỏ số quyết định.
  - Mỗi mục có tên, danh mục và 1–3 câu mô tả.
  - Chỉ có chữ tĩnh, không cần hình.
- **Ô tìm kiếm:** nên có, nhưng không bắt buộc.

## 4. Phân công
- **Codex (code):** intro player, `TutorialService` + lớp phủ khoanh sáng, các móc sự kiện, Bách Khoa popup, builder, test. Đề bài: `tools/codex/task-onboarding.md`.
- **Codex (vẽ):** 5 tranh intro theo prompt trên.
- **Claude:** viết `tutorial.json`/`encyclopedia.json` cùng Codex (Codex soạn, Claude duyệt chữ), build, test, chụp màn.
