# Đề xuất Cơ chế Mới cho Battle Engine (Part A - Items v2)

Tài liệu này đề xuất **10 cơ chế mới** (tối đa theo yêu cầu) nhằm giải quyết triệt để tình trạng nội tại vật phẩm bị trùng lặp (trước đây 86× `battle_start`, 40× `hp_below`, hiệu ứng chủ yếu là `gain_qi`, `shield`, `heal`).

Trong 10 cơ chế này:
- **6 cơ chế độc quyền sáng tạo (original)** bắt nguồn từ phong vị võ hiệp, dòng chảy Nội Lực, tâm lý chiến Hóa Kình và hệ thống ô vị trí của One Bag Jianghu (≥ 50% theo yêu cầu).
- **4 cơ chế kế thừa tinh hoa (inspired)** từ tài liệu tham khảo (`tong_hop_vat_pham_tham_khao.xlsx`) nhưng đã được chuyển hóa hoàn toàn sang hệ thống quy chuẩn của game.

---

## 1. Bảng Tổng Hợp 10 Cơ Chế Đề Xuất

| STT | Mã Cơ Chế (Engine ID) | Phân Loại | Nguồn Gốc | Mức Độ | Tóm Tắt Ý Nghĩa |
| :---: | :--- | :--- | :--- | :---: | :--- |
| **1** | `COOLDOWN_NON_WEAPON` | Hệ Thống / ItemDef | `inspired` | **Bắt buộc** | Cho phép Kỳ Trân (và Công Pháp chọn lọc) có hồi chiêu riêng, kích hoạt chủ động theo chu kỳ thay vì chỉ thụ động. |
| **2** | `TRIGGER: DAMAGED` | TriggerEvent | `original` | **Bắt buộc** | Kích hoạt khi Hiệp Khách nhận sát thương từ đòn đánh/bộc phát của đối phương. Tạo bản sắc phản kích cho Hộ Cụ. |
| **3** | `TRIGGER: HEALED` | TriggerEvent | `inspired` | **Bắt buộc** | Kích hoạt khi Hiệp Khách được hồi máu (từ Liệu Thương hoặc Điều Tức). Tạo trục tương tác cho Bích Thủy Am và lối chơi thủ hộ. |
| **4** | `OP: BUFF_STACK` | EffectOp | `original` | **Bắt buộc** | Tích lũy chỉ số (Lực Đạo, Hội Tâm, Hộ Thân) theo từng tầng có giới hạn trần (`maxStacks`), giúp trận đấu có nhịp leo thang sức mạnh. |
| **5** | `OP: CLEANSE` | EffectOp | `inspired` | **Bắt buộc** | Tẩy trừ hiệu ứng khống chế (`slow`, `seal`) và/hoặc xóa bớt điểm tích lũy trạng thái (`burn`, `poison`, `bleed`). Khắc chế lối chơi dồn debuff. |
| **6** | `OP: MULTI_HIT` | EffectOp / Weapon | `original` | **Bắt buộc** | Đòn đánh Phổ Công tung ra $N$ nhát liên hoàn liên tiếp. Phù hợp hoàn hảo với Song Đao, Ám Khí, Cung. |
| **7** | `POST_ULTIMATE_QI` | Điều kiện / Ultimate | `original` | **Bắt buộc** | Kiểm tra Nội Lực còn lại sau khi xuất Tuyệt Kỹ: nếu vẫn $\ge X$, nhận thêm hiệu ứng bộc phá. Tạo chiều sâu tâm lý với Hóa Kình. |
| **8** | `TRIGGER: CONTROLLED` | TriggerEvent | `original` | Khuyến nghị | Kích hoạt khi một đạo cụ phe ta bị Điểm Huyệt/Trì Trệ hoặc bản thân bị Định Thân. Mở ra cơ chế phản khống chế. |
| **9** | `OP: LIFESTEAL` | EffectOp / Stat | `inspired` | Khuyến nghị | Chuyển đổi % sát thương thực tế gây ra thành Khí Huyết. Tăng tính bền bỉ cho vũ khí cận chiến tốc độ cao. |
| **10** | `OP: RAISE_MAX_HP` | EffectOp | `original` | Khuyến nghị | Tăng Khí Huyết tối đa của bản thân (kèm hồi lượng máu tương ứng). Khắc chế Nội Thương và củng cố bản sắc môn phái Kim Cang. |

---

## 2. Chi Tiết Kỹ Thuật Từng Cơ Chế

### 1. `COOLDOWN_NON_WEAPON` (Kỳ Trân Khởi Động Chu Kỳ)
- **Mã Engine:** Bổ sung trường `cooldown` và danh sách hiệu ứng kích hoạt chu kỳ `active` (hoặc tái sử dụng `triggers: [{on: "interval", interval: X}]`) vào `ItemDef` dành cho `type: RELIC` hoặc `MANUAL`.
- **Cơ chế & Thứ tự thực thi:**
  - Chạy thanh hồi chiêu ở Bước 1 của tick loop giống như Binh Khí.
  - Chịu ảnh hưởng bởi Khinh Công (tốc độ x2) và Trì Trệ (tốc độ x0.5).
  - Bị tạm dừng khi dính Điểm Huyệt (`seal`) hoặc Định Thân (`stun`).
  - Khi thanh hồi chiêu đầy, kích hoạt hiệu ứng ở Bước 2 cùng nhịp với đòn đánh.
- **Văn bản UI (Tiếng Việt):** *"Mỗi [X] giây kích hoạt: [Hiệu ứng]"*
- **Giá trị gameplay:** Phá vỡ hoàn toàn thế bị động của 44 món phi binh khí; biến Kỳ Trân thành những cỗ máy tạo nhịp chiến thuật độc lập.
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 2. `TRIGGER: DAMAGED` (Khi Chịu Đòn / Bị Đả Thương)
- **Mã Engine:** `TriggerEvent.DAMAGED`
- **Cơ chế & Thứ tự thực thi:**
  - Phát sinh tại Bước 2 ngay sau khi một đòn `DAMAGE` hoặc sát thương bộc phát `BURST` từ đối phương trừ vào Cương Khí hoặc Khí Huyết của phe mình.
  - Phân biệt với `hp_below`: `hp_below` chỉ kích hoạt khi tụt qua ngưỡng máu, còn `DAMAGED` kích hoạt mỗi khi nhận đòn (có thể quy định `minDamage` hoặc hồi chiêu nội bộ để chống vòng lặp).
  - Không kích hoạt nếu toàn bộ sát thương bị Hóa Kình hóa giải hoàn toàn.
- **Văn bản UI (Tiếng Việt):** *"Khi chịu sát thương: [Hiệu ứng]"*
- **Giá trị gameplay:** Tạo nên bản sắc phòng ngự phản công chân thực cho Hộ Cụ (giáp gai, phản chấn, tích cương khí khi bị đánh).
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 3. `TRIGGER: HEALED` (Khi Được Liệu Thương / Điều Tức)
- **Mã Engine:** `TriggerEvent.HEALED`
- **Cơ chế & Thứ tự thực thi:**
  - Phát sinh khi Khí Huyết thực tế tăng lên từ nguồn `HEAL` (Liệu Thương) hoặc nhịp nhảy mỗi giây của `REGEN` (Điều Tức).
  - Nếu Khí Huyết đã đầy 100% (lượng hồi thực tế = 0), trigger không kích hoạt để tránh lạm dụng.
- **Văn bản UI (Tiếng Việt):** *"Khi được hồi phục Khí Huyết: [Hiệu ứng]"*
- **Giá trị gameplay:** Cung cấp trục kích hoạt tự nhiên cho Bích Thủy Am, biến các món hồi phục thụ động thành đòn bẩy tăng tốc độ xuất chiêu hoặc gia tăng Cương Khí.
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 4. `OP: BUFF_STACK` (Tụ Kình / Cộng Dồn Thuộc Tính Có Giới Hạn)
- **Mã Engine:** `EffectOp.BUFF_STACK`, tham số: `stat` (`attack`, `crit`, `dmg_reduce`), `value`, `maxStacks`.
- **Cơ chế & Thứ tự thực thi:**
  - Gắn bộ đếm số tầng nguyên `currentStacks` lên vật phẩm đích hoặc Hiệp Khách.
  - Mỗi lần kích hoạt: `currentStacks = min(currentStacks + 1, maxStacks)`.
  - Chỉ số gia tăng = `currentStacks * value`. Tồn tại xuyên suốt trận đấu.
  - Tuân thủ nguyên tắc số nguyên tất định, không gây tràn số hay tăng cấp số nhân.
- **Văn bản UI (Tiếng Việt):** *"Tụ Kình: Mỗi lần kích hoạt tăng [X] Lực Đạo (tối đa cộng dồn [N] tầng)."*
- **Giá trị gameplay:** Giúp các vũ khí tốc độ nhanh hoặc các thế trận cù cưa tích lũy sức mạnh về cuối trận thay vì giữ nguyên chỉ số tĩnh.
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 5. `OP: CLEANSE` (Tẩy Trừ / Thanh Thân Khử Độc)
- **Mã Engine:** `EffectOp.CLEANSE`, mục tiêu mặc định: `SELF` hoặc `SELF_ALL_ITEMS`. Tham số tùy chọn `status`.
- **Cơ chế & Thứ tự thực thi:**
  - Nếu `status == null`: Giải trừ tất cả các bùa hại có thời hạn (`SLOW`, `SEAL`).
  - Nếu chỉ định trạng thái tích điểm (`BURN`, `POISON`, `BLEED`): Khấu trừ `value` điểm đang tích lũy (ví dụ đang có 35 điểm Độc, tẩy trừ 20 điểm thì còn 15 điểm, ngăn chặn bộc phát ngưỡng 50).
- **Văn bản UI (Tiếng Việt):** *"Tẩy Trừ: Hóa giải trạng thái Trì Trệ/Điểm Huyệt (hoặc trừ [X] điểm tích lũy Kịch Độc/Liệt Hỏa)."*
- **Giá trị gameplay:** Công cụ giải độc/giải khống chế sống còn, tạo khắc chế rõ rệt với Huyền Độc Môn và Hàn Sơn Phái.
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 6. `OP: MULTI_HIT` (Liên Kích / Phân Thân Kích)
- **Mã Engine:** `EffectOp.MULTI_HIT` hoặc nâng cấp tham số `hits` trong `EffectOp.DAMAGE`.
- **Cơ chế & Thứ tự thực thi:**
  - Khi Phổ Công kích hoạt, thực hiện $N$ nhát chém liên tiếp trong cùng 1 tick hoặc cách nhau 1 tick (100ms).
  - Mỗi nhát tính độc lập Hội Tâm và trừ vào Cương Khí đối thủ.
  - Mỗi nhát trúng đều có thể kích hoạt các trigger phụ thuộc nhát chém (`HIT`, `CRIT`, bôi độc/hỏa).
- **Văn bản UI (Tiếng Việt):** *"Liên Kích [N] nhát: Mỗi nhát gây [X]% Lực Đạo."*
- **Giá trị gameplay:** Tạo sự khác biệt vượt bậc về nhịp điệu giữa vũ khí nặng (Khai Sơn Phủ chém 1 phát cực mạnh) và vũ khí nhẹ (Tử Mẫu Song Đao, Thất Tinh Tiêu chém x2, x3 liên hoàn).
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 7. `POST_ULTIMATE_QI` (Hậu Kình Dư Khí)
- **Mã Engine:** `TriggerDef` hoặc `UltimateDef` với trường điều kiện: `condition: "qi_remaining"`, `minValue: 30`.
- **Cơ chế & Thứ tự thực thi:**
  - Kiểm tra ngay tại Bước thi triển Tuyệt Kỹ sau khi đã khấu trừ chi phí xuất chiêu.
  - Nếu lượng Nội Lực hiện tại của Hiệp Khách vẫn $\ge minValue$, kích hoạt thêm danh sách hiệu ứng phụ.
  - Nếu Tuyệt Kỹ bị Hóa Kình (`PARRIED`), nhánh Hậu Kình không kích hoạt.
- **Văn bản UI (Tiếng Việt):** *"Hậu Kình: Nếu sau khi thi triển Tuyệt Kỹ Nội Lực còn $\ge$ [X], nhận thêm [Hiệu ứng]."*
- **Giá trị gameplay:** Đưa yếu tố "quản lý khí công" lên đỉnh cao: người chơi phải tính toán tích thừa nội lực để kích hoạt hiệu ứng mạnh, đồng thời phải cân não giữ lại nội lực để Hóa Kình đòn đánh của địch.
- **Trạng thái:** **Bắt buộc (Required)**.

---

### 8. `TRIGGER: CONTROLLED` (Nghịch Cảnh / Phản Khống Chế)
- **Mã Engine:** `TriggerEvent.CONTROLLED`
- **Cơ chế & Thứ tự thực thi:**
  - Phát sinh khi một vật phẩm của bản thân bị dính `SEAL` (Điểm Huyệt), `SLOW` (Trì Trệ) hoặc bản thân bị `STUN` (Định Thân).
- **Văn bản UI (Tiếng Việt):** *"Khi bản thân hoặc đạo cụ bị khống chế: [Hiệu ứng]"*
- **Giá trị gameplay:** Giúp người chơi xây dựng các đội hình "thế chân vạc", biến điểm yếu khi bị đối thủ khóa chiêu thành cơ hội phản đòn bộc phá.
- **Trạng thái:** Khuyến nghị (Nice-to-have).

---

### 9. `OP: LIFESTEAL` (Hút Huyết Thao Quang)
- **Mã Engine:** `EffectOp.LIFESTEAL` (hoặc trường `lifestealPct` trong `EffectDef`).
- **Cơ chế & Thứ tự thực thi:**
  - Hồi Khí Huyết cho bản thân bằng `floor(damage_dealt * pct)`.
  - Áp dụng trên lượng sát thương thực tế gây ra vào Cương Khí và Khí Huyết đối phương.
  - Không vượt quá Khí Huyết tối đa.
- **Văn bản UI (Tiếng Việt):** *"Hút Huyết [X]%: Chuyển [X]% sát thương gây ra thành Khí Huyết hồi phục."*
- **Giá trị gameplay:** Củng cố bản sắc cho các vũ khí hung bạo (Tử Kim Trảo, Xích Sa Chưởng, Thanh Đằng Tiên).
- **Trạng thái:** Khuyến nghị (Nice-to-have).

---

### 10. `OP: RAISE_MAX_HP` (Cố Bản Bồi Nguyên)
- **Mã Engine:** `EffectOp.RAISE_MAX_HP`, tham số `value`.
- **Cơ chế & Thứ tự thực thi:**
  - Tăng vĩnh viễn giới hạn `maxHp` của Hiệp Khách thêm `value` điểm, đồng thời cộng ngay lượng Khí Huyết hiện tại tương ứng.
  - Khắc chế trực tiếp trạng thái Nội Thương (`reduce_max_hp`).
- **Văn bản UI (Tiếng Việt):** *"Cố Bản: Tăng [X] điểm Khí Huyết tối đa."*
- **Giá trị gameplay:** Cực kỳ quan trọng cho các loại Đan Dược thượng phẩm và Hộ Cụ hạng nặng của Kim Cang Tự.
- **Trạng thái:** Khuyến nghị (Nice-to-have).
