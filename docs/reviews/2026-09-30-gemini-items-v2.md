# Báo Cáo Thiết Kế Lại Hệ Thống Vật Phẩm (Items v2)

**Người thực hiện:** Gemini (Game Designer)  
**Ngày:** 30/09/2026  
**Mục tiêu:** Tái thiết kế toàn diện 61 vật phẩm trong `items.json`, xóa bỏ triệt để sự trùng lặp (trước đây 86× `battle_start`, 40× `hp_below`, quá nhiều khiên/nội lực tĩnh), xác lập bản sắc rõ nét cho từng phân loại và gắn kết chặt chẽ với 7 môn phái.

---

## 1. Tóm Tắt Kết Quả Cốt Lõi

- **Đột phá đa dạng hóa:** Tỉ lệ kích hoạt đầu trận (`battle_start`) giảm từ **45.7% (86)** xuống **21.1% (51)** (đạt mục tiêu $\le 25\%$). Tỉ lệ kích hoạt dưới máu (`hp_below`) giảm từ **21.3% (40)** xuống **9.9% (24)** (đạt mục tiêu $\le 12\%$).
- **Hệ thống kích hoạt phong phú:** Bổ sung các nhịp kích hoạt mới: `crit` (34), `parry_success` (28), `burst` (24), `ultimate` (19), `hit` (18), `healed` (12), `damaged` (11), `crit_taken` (5), `controlled` (3), v.v.
- **Tỉ lệ nguồn gốc ý tưởng (Origin):**
  - **Sáng tạo độc quyền (`original`):** **40 vật phẩm (65.6%)** (vượt xa chỉ tiêu $\ge 50\%$).
  - **Kế thừa chọn lọc (`inspired`):** **12 vật phẩm (19.7%)**.
  - **Phỏng tác sát bản gốc (`adapted`):** **9 vật phẩm (14.8%)** (nằm trong trần $\le 20\%$).
- **Không vẽ lại hình ảnh:** Giữ nguyên 100% định danh (`id`), tên gọi, phân loại (`type`), kích thước ô (`size`), hệ binh khí (`weaponClass`) và hình vẽ Thủy Mặc.
- **Đã kiểm chứng kỹ thuật:** Bản rút gọn không dùng cơ chế mới `items-v2-existing-only.json` (19 vật phẩm) đã nạp và mô phỏng trận đấu thành công 100% trên `BattleCli` Java (`Exit code: 0`).

---

## 2. Bảng 10 Cơ Chế Mới Đề Xuất (Part A)

Chi tiết đầy đủ xem tại [`tools/gemini/out/items-v2/mechanics.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/out/items-v2/mechanics.md).

| STT | Mã Cơ Chế | Loại | Nguồn Gốc | Mức Độ | Tóm Tắt Ngữ Nghĩa & Giao Diện Tiếng Việt |
| :---: | :--- | :--- | :--- | :---: | :--- |
| **1** | `COOLDOWN_NON_WEAPON` | ItemDef | `inspired` | **Bắt buộc** | Cho phép Kỳ Trân/Công Pháp có hồi chiêu riêng. *"Mỗi [X] giây kích hoạt: ..."* |
| **2** | `TRIGGER: DAMAGED` | Event | `original` | **Bắt buộc** | Kích hoạt khi nhận sát thương từ đối phương. *"Khi chịu sát thương: ..."* |
| **3** | `TRIGGER: HEALED` | Event | `inspired` | **Bắt buộc** | Kích hoạt khi được hồi máu thực tế. *"Khi được hồi phục Khí Huyết: ..."* |
| **4** | `OP: BUFF_STACK` | EffectOp | `original` | **Bắt buộc** | Tụ Kình tích lũy chỉ số có trần (`maxStacks`). *"Tụ Kình: Mỗi lần tăng [X], tối đa [N] tầng."* |
| **5** | `OP: CLEANSE` | EffectOp | `inspired` | **Bắt buộc** | Hóa giải bùa hại (`slow`/`seal`) hoặc trừ điểm tích lũy (`burn`/`poison`). *"Tẩy Trừ: ..."* |
| **6** | `OP: MULTI_HIT` | EffectOp | `original` | **Bắt buộc** | Phổ Công tung $N$ nhát liên hoàn liên tiếp. *"Liên Kích [N] nhát: Mỗi nhát [X]% Lực Đạo."* |
| **7** | `POST_ULTIMATE_QI` | Điều kiện | `original` | **Bắt buộc** | Kiểm tra Nội Lực còn lại sau Tuyệt Kỹ: nếu $\ge X$ nhận thêm hiệu ứng. *"Hậu Kình: ..."* |
| **8** | `TRIGGER: CONTROLLED` | Event | `original` | Khuyến nghị | Kích hoạt khi bản thân/đạo cụ dính Điểm Huyệt/Trì Trệ/Định Thân. *"Khi bị khống chế: ..."* |
| **9** | `OP: LIFESTEAL` | EffectOp | `inspired` | Khuyến nghị | Hút máu theo % sát thương thực tế gây ra. *"Hút Huyết [X]%: Chuyển thành Khí Huyết."* |
| **10** | `OP: RAISE_MAX_HP` | EffectOp | `original` | Khuyến nghị | Tăng vĩnh viễn giới hạn Khí Huyết tối đa. Khắc chế Nội Thương. *"Cố Bản: Tăng [X] Khí Huyết tối đa."* |

---

## 3. So Sánh Phân Bổ Trigger & Hiệu Ứng Trước và Sau Cải Tiến

### Phân Bổ Sự Kiện Kích Hoạt (Triggers)
Tổng số trigger tăng từ **188** lên **242** nhờ hệ thống Đột Phá đa tầng:

| Sự Kiện Kích Hoạt (Trigger) | Trước Đây | Tỉ Lệ Trước | Sau Cải Tiến | Tỉ Lệ Sau | Đánh Giá Mục Tiêu |
| :--- | :---: | :---: | :---: | :---: | :--- |
| **`battle_start` (Đầu trận)** | **86** | **45.7%** | **51** | **21.1%** | **Đạt tiêu chuẩn** ($\le 25\%$) |
| **`hp_below` (Máu tụt dưới ngưỡng)** | **40** | **21.3%** | **24** | **9.9%** | **Đạt tiêu chuẩn** ($\le 12\%$) |
| `crit` (Đánh Hội Tâm) | 17 | 9.0% | 34 | 14.0% | Tăng mạnh tương tác Hội Tâm |
| `parry_success` (Hóa Kình thành công) | 12 | 6.4% | 28 | 11.6% | Thưởng phản xạ Hóa Kình |
| `burst` (Trạng thái bộc phát) | 14 | 7.4% | 24 | 9.9% | Tạo nhịp bùng nổ theo đợt |
| `ultimate` (Xuất Tuyệt Kỹ) | 7 | 3.7% | 19 | 7.9% | Tăng liên kết xuất chiêu |
| `hit` (Đánh trúng đối thủ) | 10 | 5.3% | 18 | 7.4% | Bổ trợ nhịp đánh nhanh |
| `healed` (Được hồi máu) | 0 | 0.0% | 12 | 5.0% | Bản sắc phái Bích Thủy |
| `damaged` (Chịu sát thương) | 0 | 0.0% | 11 | 4.5% | Bản sắc giáp phản kích |
| `crit_taken` (Bị đánh chí mạng) | 0 | 0.0% | 5 | 2.1% | Cơ chế bảo hiểm chống dồn sốc |
| Khác (`controlled`, `qi_empty`, ...) | 2 | 1.1% | 17 | 7.0% | Nhánh chiến thuật đặc thù |
| **Tổng cộng** | **188** | **100%** | **242** | **100%** | **Đa dạng hóa toàn diện** |

---

## 4. Bản Sắc Rõ Nét Theo Từng Phân Loại Vật Phẩm

1. **Binh Khí (`weapon`, 17 món):** 
   - Đóng vai trò hạt nhân gây sát thương với tiết tấu rõ rệt: Khai Sơn Phủ/Chấn Nhạc Chùy bổ nặng phá Cương Khí; Tử Mẫu Song Đao/Thất Tinh Tiêu kích hoạt **Liên Kích (Multi-hit)** x2, x3; Thanh Phong Kiếm vận dụng **Hậu Kình (Post-Ultimate Qi)**; Tử Kim Trảo kết hợp **Hút Huyết (Lifesteal)** và bộc phát Xuất Huyết.
2. **Hộ Cụ (`armor`, 11 món):** 
   - Xóa bỏ việc chỉ là "bình Cương Khí di động". Trở thành trang bị phòng ngự phản kích chủ động: Thiết Giáp Y/Phục Ma Giáp phản chấn sát thương và Tụ Kình khi nhận đòn (`damaged`); Băng Tâm Giáp phản hàn khí làm Trì Trệ kẻ tấn công; Kim Tuyến Y tản lực khi bị đánh Hội Tâm; Thanh Đồng Hộ Tâm Kính Tẩy Trừ bùa hại khi ngàn cân treo sợi tóc.
3. **Công Pháp (`manual`, 11 món):** 
   - Thay đổi luật chơi và dẫn dắt phong cách môn phái: Bách Luyện Chân Kinh tăng công và Vận Khí khi đập vỡ khiên địch; Dưỡng Sinh Quyết chuyển hóa lượng hồi máu thành Hộ Thân; Kiếm Tông Tâm Yếu ban khả năng bỏ qua Cương Khí cho các nhát đâm chí mạng kề bên; Thuần Dương Công bộc phát uy lực khi giữ lượng dư khí dồi dào.
4. **Đan Dược (`pill`, 11 món):** 
   - Đúng nghĩa "bảo mệnh đan dược dùng một lần" (`maxUses: 1`): Không còn rập khuôn chờ máu tụt; Cuồng Hỏa Đan bộc phát Khinh Công khi xuất Tuyệt Kỹ đầu trận; Vận Khí Đan kích hoạt khi cạn khí; Giải Độc Đan tự động khai mở khi điểm tích lũy độc/lửa chạm ngưỡng nguy hiểm; Cửu Chuyển Hoàn Hồn Đan tăng máu tối đa (Cố Bản).
5. **Kỳ Trân (`relic`, 11 món):** 
   - Sở hữu hồi chiêu riêng (`COOLDOWN_NON_WEAPON`) hoặc trở thành cỗ máy vận hành thế trận (engine): Túy Tiên Hồ Lô uống rượu theo chu kỳ để hồi phục sinh lực và nội khí; Trầm Hương Lư định kỳ xua tan tà khí độc hỏa; Hàn Băng Châu định kỳ phát tán giá lạnh Điểm Huyệt vũ khí nhanh nhất của địch; Âm Dương Ngọc chuyển hóa công thành máu, thủ thành khiên.

---

## 5. Bảng Phủ Kín 7 Môn Phái (Tối Thiểu 2 Phi Binh Khí / Phái)

| Môn Phái | Lối Chơi Trọng Tâm | Binh Khí Tiêu Biểu | Đạo Cụ Phi Binh Khí Hỗ Trợ Đặc Thù (Tối thiểu 2) |
| :--- | :--- | :--- | :--- |
| **Kim Cang Tự** | Cương Khí, Định Thân, Thể Phách, Cận Chiến | Thiết Cốt Quyền Sáo, Phục Hổ Côn | 1. `bach_luyen_chan_kinh` (Công Pháp: Vận Khí khi Phá Cương)<br>2. `thiet_giap_y` (Hộ Cụ: Tụ Kình Hộ Thân khi chịu đòn)<br>3. `tu_kim_bat` (Kỳ Trân: Chuông Phật tạo khiên & Vận Khí chu kỳ)<br>4. `cuu_chuyen_hoan_hon_dan` (Đan Dược: Cố Bản tăng máu tối đa & Định Thân) |
| **Thái Thanh Quán** | Hóa Kình, Kiếm Khí, Hội Tâm, Điều Hòa Khí | Thanh Phong Kiếm, Ngọc Cốt Phiến | 1. `thai_thanh_tam_kinh` (Công Pháp: Giảm tiêu hao & hồi khí khi Hóa Kình)<br>2. `kiem_tong_tam_yeu` (Công Pháp: Bổ trợ Kiếm/Phiến tăng Hội Tâm & xuyên giáp)<br>3. `bach_ngoc_boi` (Kỳ Trân: Tĩnh tâm tăng bạo kích chu kỳ)<br>4. `thien_tam_chuoi` (Kỳ Trân: Hóa Kình thành công hồi khí & Định Thân) |
| **Liệt Diễm Đường** | Liệt Hỏa, Bộc Phát, Vận Khí, Tấn Công Áp Đảo | Liệt Diễm Đao, Xích Sa Chưởng, Tử Mẫu Song Đao | 1. `liet_hoa_tam_kinh` (Công Pháp: Gây cháy tăng bạo kích, bộc phát Vận Khí)<br>2. `cuong_hoa_dan` (Đan Dược: Tuyệt Kỹ đầu trận mở Khinh Công & cuồng nộ)<br>3. `chu_sa_ho_lo` (Kỳ Trân: Phun sương găm Liệt Hỏa chu kỳ)<br>4. `da_ho_y` (Hộ Cụ: Chịu đòn dồn Lực Đạo cho vũ khí kề bên) |
| **Huyền Độc Môn** | Kịch Độc, Xuất Huyết, Hút Nội Lực, Bào Mòn | Thất Tinh Tiêu, Tử Kim Trảo | 1. `u_minh_bi_luc` (Công Pháp: Bộc phát độc/máu thì hút khí địch & hồi máu)<br>2. `van_doc_dinh` (Kỳ Trân: Đỉnh tích độc phụ trội & hút khí khi nổ độc)<br>3. `huyet_ngoc_thach` (Kỳ Trân: Hút tinh huyết nuôi vũ khí kề bên khi nổ máu)<br>4. `giai_doc_dan` (Đan Dược: Tự động Tẩy Trừ khi bị đối phương dồn độc/lửa) |
| **Hàn Sơn Phái** | Trì Trệ, Điểm Huyệt, Khống Chế, Hàn Băng | Hàn Ngọc Tiêu, Phán Quan Bút, Băng Phách Thương, Truy Hồn Cung | 1. `bang_tam_giap` (Hộ Cụ: Phản hàn khí làm Trì Trệ vũ khí kẻ tấn công)<br>2. `han_bang_chau` (Kỳ Trân: Tỏa hàn khí cực độ Điểm Huyệt vũ khí nhanh nhất)<br>3. `nho_sam_y` (Hộ Cụ: Địch xuất Tuyệt Kỹ thì hút khí & tạo Cương Khí)<br>4. `thanh_tam_dan` (Đan Dược: Tẩy Trừ khống chế & khai thông kinh mạch) |
| **Thiết Phủ Bang** | Phá Cương, Sát Thương Thuần, Cường Lực, Rìu/Chùy | Khai Sơn Phủ, Chấn Nhạc Chùy | 1. `o_thiet_giap` (Hộ Cụ: Cương Khí khổng lồ, Hóa Kình hồi giáp tăng công)<br>2. `phuc_ma_giap` (Hộ Cụ: Phục Ma phản chấn sát thương trực tiếp)<br>3. `da_ho_y` (Hộ Cụ: Càng bị thương càng tăng sát thương cho vũ khí) |
| **Bích Thủy Am** | Điều Tức, Liệu Thương, Dẻo Dai, Dưỡng Sinh | Bích Thủy Cầm, Thanh Đằng Tiên | 1. `duong_sinh_quyet` (Công Pháp: Hồi máu tích lũy Hộ Thân & Cương Khí)<br>2. `tram_huong_lo` (Kỳ Trân: Xông hương Tẩy Trừ độc hỏa & cấp khiên chu kỳ)<br>3. `kim_sang_duoc` (Đan Dược: Trị Liệu tức thì & cầm máu khi chịu đòn hiểm)<br>4. `tue_nguyet_bao_y` (Hộ Cụ: Khi được hồi máu tích thêm khiên & nội khí) |

---

## 6. Đánh Giá Khách Quan: Những Vật Phẩm Cảm Thấy Hơi Khiên Cưỡng

Trong quá trình thiết kế bám sát hình ảnh gốc (art prompts), có 4 vật phẩm tạo cảm giác hơi gượng gạo về quy chuẩn hệ thống:

1. **`thanh_dong_ho_tam_kinh` (Thanh Đồng Hộ Tâm Kính - Hộ Cụ 1x1):**
   - *Vấn đề:* Tất cả Hộ Cụ khác đều là áo giáp kích thước 2x1 hoặc 2x2. Chiếc gương đồng hộ tâm kích thước 1x1 này thực chất là một phụ kiện/trang sức đeo trước ngực. Đưa nó vào nhóm Hộ Cụ khiến chỉ số khiên của nó bị khống chế bởi diện tích 1 ô, tính chất giống một Kỳ Trân hơn là một bộ giáp.
2. **`xich_sa_chuong` (Xích Sa Chưởng - Binh Khí 1x2):**
   - *Vấn đề:* Hình ảnh chỉ vẽ một dải băng quấn lòng bàn tay màu đỏ sẫm. Khác với các vũ khí rèn đúc cụ thể (kiếm, đao, rìu), chưởng pháp vốn là chiêu thức nội công. Việc quy nó thành vũ khí độc lập mang tốc độ đánh và lực đạo riêng đòi hỏi người chơi phải hiểu đây là "găng vải quấn tay tăng lực chưởng".
3. **`truy_hon_cung` (Truy Hồn Cung - Binh Khí 1x3):**
   - *Vấn đề:* Game auto-battler quản lý túi đồ dạng 2D không có cơ chế quản lý ống tên rời. Cung bắn liên tục không tốn đạn dược trông hơi kỳ lạ nếu xét về tính chân thực vật lý, dù cơ chế xạ thủ tầm xa xuyên khiên (`ignoreShield`) vận hành rất tốt.
4. **`tu_kim_bat` (Tử Kim Bát - Kỳ Trân 2x1):**
   - *Vấn đề:* Bát khất thực của nhà sư thường có dạng hình tròn đồng tâm (1x1), nhưng trong game lại có kích thước chữ nhật nằm ngang 2x1 (để phù hợp với tranh vẽ chiếc bát đặt trên tấm vải xếp). Diện tích 2 ô khiến người chơi kỳ vọng nó có công năng của một cỗ máy hỗ trợ lớn thay vì một chiếc bát ăn thông thường.

---

## 7. 5 Câu Hỏi Mở Cần Người Dùng Chốt Ý Kiến

1. **Về cơ chế Hồi chiêu trên Kỳ Trân (`COOLDOWN_NON_WEAPON`):**
   - Người dùng có đồng ý cho phép các món Kỳ Trân (như Túy Tiên Hồ Lô, Trầm Hương Lư, Hàn Băng Châu) sở hữu thanh hồi chiêu hoạt động độc lập theo chu kỳ (tương tự như Binh Khí nhưng không xuất chiêu đánh thường mà phát động hiệu ứng hỗ trợ) hay giữ nguyên quy tắc "chỉ Binh Khí mới có hồi chiêu"? *(Gemini đề xuất: Rất cần thiết để phá thế thụ động).*
2. **Về phân loại của Thanh Đồng Hộ Tâm Kính:**
   - Có nên chuyển `thanh_dong_ho_tam_kinh` sang nhóm `relic` (Kỳ Trân) và đổi một vật phẩm 2x1 khác làm Hộ Cụ (ví dụ bổ sung Giáp Mây/Khảm Giáp), hay giữ nguyên là Hộ Cụ 1x1 duy nhất trong game?
3. **Về cơ chế Tẩy Trừ (`CLEANSE`):**
   - Tẩy Trừ nên xóa đứt điểm toàn bộ trạng thái hay chỉ trừ một lượng điểm cố định (ví dụ trừ 20/50 điểm Độc/Hỏa đang tích lũy)? *(Gemini đề xuất: Trừ điểm số cố định với trạng thái tích điểm, và xóa hẳn với bùa hại có thời hạn như Trì Trệ/Điểm Huyệt để đảm bảo tính cân bằng).*
4. **Về đòn Liên Kích (`MULTI_HIT`):**
   - Với các vũ khí tấn công nhiều nhát trong một lượt (như Song Đao x2 nhát, Thất Tinh Tiêu x3 nhát), các hiệu ứng bôi độc/hỏa hay đòn hút máu nên tính trên từng nhát chém riêng rẽ hay chỉ tính một lần cho cả đợt xuất chiêu? *(Gemini đề xuất: Tính riêng từng nhát nhưng giảm số điểm bôi mỗi nhát để tạo bản sắc cho vũ khí tốc đánh cao).*
5. **Về lộ trình hiện thực hóa Code:**
   - Trong 10 cơ chế Part A, người dùng muốn Claude/Codex ưu tiên viết code engine cho nhóm **Bắt buộc** (6 cơ chế: `COOLDOWN_NON_WEAPON`, `DAMAGED`, `HEALED`, `BUFF_STACK`, `CLEANSE`, `POST_ULTIMATE_QI`) trước để đưa vào mô phỏng cân bằng vòng kế tiếp, hay có điều chỉnh gì thêm trong danh sách này?

---

### Danh Mục File Sản Phẩm Kèm Theo:
- **Đặc tả 10 cơ chế Part A:** [`tools/gemini/out/items-v2/mechanics.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/out/items-v2/mechanics.md)
- **Danh mục 61 vật phẩm JSON đầy đủ:** [`tools/gemini/out/items-v2/items-v2.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/out/items-v2/items-v2.json)
- **Bảng đối chiếu thay đổi (CSV):** [`tools/gemini/out/items-v2/changes.csv`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/out/items-v2/changes.csv)
- **Danh mục rút gọn kiểm chứng engine hiện tại:** [`tools/gemini/out/items-v2/items-v2-existing-only.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/out/items-v2/items-v2-existing-only.json)
