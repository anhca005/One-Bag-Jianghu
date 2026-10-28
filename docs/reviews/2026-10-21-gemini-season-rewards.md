# Báo cáo Thiết Kế Thưởng Cuối Mùa Luận Kiếm Đài và Cơ Chế Đặt Lại Điểm

- **Ngày lập:** 2026-10-21
- **Người thực hiện:** Gemini (Nhà thiết kế số liệu One Bag Jianghu)
- **Tập tin liên quan:**
  - [`arena-lobby.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/arena-lobby.md)
  - [`decisions.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md) (#24, #25, #36, #68, #69, #70)
  - [`glossary.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/glossary.md)
  - [`gdd.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/design/gdd.md)
  - [`shop.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json)
  - [`arena.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/arena.json)
  - [`merge.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/merge.json)
  - [`season_sim.py`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/season_sim.py)

---

## 1. Phương Án Xác Định Bậc Nhận Thưởng Cuối Mùa

Một vấn đề cốt lõi trong thiết kế PvP là lựa chọn giữa **Bậc cao nhất đạt được trong mùa (Peak Tier)** hay **Bậc tại thời điểm chốt sổ (Final Tier)**.

### Lựa chọn của Gemini: Bậc cao nhất đạt được trong mùa (Peak Tier)
Đối với phần thưởng theo bậc, đề xuất tính theo **Bậc cao nhất mà người chơi từng đạt được trong suốt 28 ngày của Kỳ Luận Kiếm**.

**Lý do:**
1. **Loại bỏ hội chứng sợ mất hạng (Ladder Anxiety):**
   - Trong One Bag Jianghu, bóng kiếm (`ghost`) của người chơi lưu lại trên máy chủ và liên tục bị người chơi khác khiêu chiến khi offline ([#68](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L48), `updateOpponent` trong `ArenaService.java`).
   - Nếu tính thưởng theo bậc lúc chốt sổ, người chơi vừa chạm ngưỡng bậc cao (ví dụ vừa lên Tông Sư 1.800 ở tuần 3) sẽ có tâm lý sợ hãi, ngừng khiêu chiến trong tuần cuối để "bảo toàn rank", hoặc ức chế tột độ nếu bóng kiếm bị người khác đánh bại làm tụt hạng ngay trước giờ chốt mùa.
2. **Duy trì hoạt động của hàng chờ (Matchmaking Liquidity):**
   - Khi biết bậc cao nhất đã được hệ thống ghi nhận và bảo lưu, người chơi hoàn toàn tự tin tiếp tục đánh đủ 10 trận có thưởng mỗi ngày để gom Danh Vọng và hoàn thành mốc tuần mà không sợ mất phần thưởng mùa.
3. **Phù hợp với trải nghiệm casual võ hiệp:**
   - Người chơi cảm thấy công sức cày cuốc leo bậc được công nhận và tôn trọng, tránh cảm giác "công dã tràng" vì một chuỗi trận bị đối thủ khắc chế vào cuối mùa.

### Quy tắc riêng cho Top Bảng Xếp Hạng (Leaderboard Top 1, Top 2–3, Top 4–10):
- **Phải chốt theo Thứ hạng thực tế tại thời điểm chốt mùa (Snapshot Final Leaderboard).**
- *Lý do:* Bảng xếp hạng là cuộc đua cạnh tranh trực tiếp, có tính chất loại trừ (zero-sum). Danh hiệu "Thiên Hạ Đệ Nhất" và phần thưởng Top chỉ có thể trao cho người đang đứng ở vị trí vinh quang khi tiếng chuông bế mạc Kỳ Luận Kiếm vang lên.

---

## 2. Bảng Thưởng Cuối Mùa Luận Kiếm Đài

### A. Bảng thưởng theo Bậc (Tính theo Bậc cao nhất trong mùa)

| Bậc Luận Kiếm | Điểm yêu cầu | Danh Vọng | Chí Tôn Ngọc Tủy | Rương Chất Dẫn Ngẫu Nhiên | Rương Nguyên Liệu Bậc 3 |
|---|---|---|---|---|---|
| **Vô Danh** | 1.000–1.099 | 300 | 0 | 0 | 1 |
| **Tân Tú** | 1.100–1.249 | 600 | 0 | 1 | 2 |
| **Thiếu Hiệp** | 1.250–1.399 | 1.000 | 1 | 2 | 3 |
| **Đại Hiệp** | 1.400–1.599 | 1.500 | 2 | 3 | 4 |
| **Danh Túc** | 1.600–1.799 | 2.200 | 3 | 4 | 5 |
| **Tông Sư** | từ 1.800 | 3.000 | 4 | 5 | 6 |

*Phân tích cấu trúc phần thưởng:*
- **Chí Tôn Ngọc Tủy:** Chỉ bắt đầu trao từ bậc **Thiếu Hiệp** (1.250 điểm, tương đương trung vị người chơi hoạt động đều). Hai bậc khởi đầu Vô Danh và Tân Tú không nhận Ngọc Tủy nhằm ngăn chặn tài khoản phụ tạo ra chỉ để tích cóp nguyên liệu tối thượng.
- **Rương Chất Dẫn Ngẫu Nhiên (`catalyst_box_random`):** Rơi ngẫu nhiên 1 trong 4 loại chất dẫn Đột Phá mốc Trọng 3/6/9 (Binh Hồn Tinh Thạch, Huyền Giáp Hồn Thạch, Vấn Đạo Tâm Tinh, Cửu Chuyển Đan Sa). Cung cấp từ bậc Tân Tú để người chơi có tài nguyên Đột Phá đồ.
- **Rương Nguyên Liệu Bậc 3 (`tier_3_material_box`):** Mỗi rương chứa 3 nguyên liệu bậc 3 (Thiên Ngoại Vẫn Thiết, Cửu U Tàm Ti, Tằm Tơ Cổ Quyển, Côn Lôn Tuyết Liên) phục vụ nâng Trọng 7→9.

### B. Bảng thưởng thêm theo Thứ Hạng (Top Leaderboard Snapshot)

Thưởng thêm cộng dồn cùng phần thưởng bậc ở trên:

| Thứ hạng | Danh Vọng thêm | Chí Tôn Ngọc Tủy thêm | Danh hiệu độc quyền | Quyền lợi hiển thị |
|---|---|---|---|---|
| **Top 1** | +3.000 | +3 | **Thiên Hạ Đệ Nhất** | Chữ vàng kim rực rỡ, giữ trong 28 ngày mùa sau |
| **Top 2 – 3** | +2.000 | +2 | — | Dấu ấn Bạc bảng xếp hạng |
| **Top 4 – 10** | +1.200 | +1 | — | Dấu ấn Đồng bảng xếp hạng |
| **Top 11 – 50** | +600 | 0 | — | Vinh danh danh sách Top 50 Sảnh |

*Ví dụ:* Một hiệp khách đạt bậc Tông Sư và giữ vững ngôi vị **Top 1** khi mùa giải kết thúc sẽ nhận tổng cộng:
- Thưởng bậc Tông Sư: 3.000 Danh Vọng + 4 Ngọc Tủy + 5 Rương Chất Dẫn + 6 Rương Bậc 3.
- Thưởng Top 1: 3.000 Danh Vọng + 3 Ngọc Tủy + Danh hiệu *Thiên Hạ Đệ Nhất*.
- **Tổng nhận:** 6.000 Danh Vọng, 7 Chí Tôn Ngọc Tủy, 5 Rương Chất Dẫn, 6 Rương Bậc 3 và danh hiệu Đệ Nhất giang hồ.

---

## 3. Danh Hiệu "Thiên Hạ Đệ Nhất" và Đề Xuất Ngoại Trang

### Danh hiệu "Thiên Hạ Đệ Nhất" cho Hạng 1:
- **Thời hạn sở hữu:** Kéo dài trong suốt Kỳ Luận Kiếm tiếp theo (28 ngày). Nếu hết mùa sau người chơi không giữ được vị trí Top 1, danh hiệu sẽ chuyển giao cho quán quân mới.
- **Vị trí hiển thị trong game:**
  1. **Thẻ Hiệp Khách** (cột trái Sảnh Luận Kiếm Đài): Hiển thị nhãn danh hiệu font Display màu vàng kim `[Thiên Hạ Đệ Nhất]` ngay dưới tên Hiệp Khách.
  2. **Thẻ Đối Đầu** (Versus Popup khi tìm thấy trận): Đối thủ khi gặp bóng kiếm của Quán Quân mùa trước sẽ thấy danh hiệu hiển thị nổi bật trên thẻ đối thủ.
  3. **Bảng Xếp Hạng & Lịch Sử Đấu:** Dòng thông tin có gắn kèm huy hiệu ấn kiếm vàng cùng danh xưng Thiên Hạ Đệ Nhất.

### Đề xuất về Trang phục / Khung Avatar trong GDD:
- **ĐỀ XUẤT: CHƯA TRIỂN KHAI Ở ĐỢT NÀY (HOÃN SANG BẢN CẬP NHẬT TÍNH NĂNG NGOẠI TRANG SAU).**
- **Lý do:**
  1. *Phạm vi kỹ thuật:* Client hiện tại chưa có hệ thống tủ đồ Ngoại Trang (`costumes`), chưa có cấu trúc dữ liệu lưu khung avatar trong hồ sơ tài khoản và chưa có component hiển thị khung trong `AvatarView`.
  2. *Nguồn lực mỹ thuật:* Các bộ khung avatar và skin nhân vật Thủy Mặc đòi hỏi thời gian vẽ và bake texture riêng biệt ([#66](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L91)).
  3. *Tập trung trọng tâm:* Trọng tâm hiện tại của dự án theo [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7) là hoàn thiện kiến trúc mạng client-server, chống gian lận và cân bằng kinh tế. Giá trị của giải đấu đã được đảm bảo thông qua Danh Vọng, Ngọc Tủy, Rương Nguyên Liệu và Danh hiệu vinh danh.

---

## 4. Điều Kiện Nhận Thưởng Cuối Mùa

Để nhận thưởng cuối mùa (cả thưởng bậc và thưởng top), tài khoản phải thỏa mãn:
- **Số trận khiêu chiến tối thiểu trong mùa: 10 trận.**
- **Lý do:**
  - 10 trận đúng bằng định mức số trận có thưởng trong 1 ngày chơi thông thường.
  - Chặn đứng hoàn toàn vấn đề tài khoản "rác" hoặc tài khoản clone: Người dùng tạo hàng loạt tài khoản để nhận rating sàn 1.000 (Vô Danh) rồi bỏ mặc chờ hết 28 ngày để nhận thưởng Danh Vọng miễn phí.
  - Ngưỡng 10 trận là yêu cầu rất nhẹ nhàng cho người chơi thật (chỉ cần vào đấu 1 ngày trong cả 4 tuần), nhưng là rào cản ngăn chặn bot và tài khoản ma.

---

## 5. Cơ Chế Đặt Lại Điểm Sang Mùa Mới (Season Soft Reset)

### Công thức đề xuất:
$$R_{\text{new}} = \max\left(1000, 1000 + \text{round}\left((R_{\text{old}} - 1000) \times 0.5\right)\right)$$

Trong đó:
- $R_{\text{old}}$: Điểm Elo tại thời điểm đóng mùa giải.
- $R_{\text{new}}$: Điểm Elo khởi đầu mùa giải mới.
- Sàn 1.000 điểm luôn được bảo toàn tuyệt đối theo [#68](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L48).
- Làm tròn số nguyên bằng hàm chuẩn `round()`.

### Bảng đối chiếu mức điểm sau đặt lại:

| Bậc trước Reset | Điểm cũ ($R_{\text{old}}$) | Điểm mới ($R_{\text{new}}$) | Bậc sau Reset | Độ lệch ($\Delta$) | Trạng thái |
|---|---|---|---|---|---|
| **Vô Danh (Sàn)** | 1.000 | 1.000 | Vô Danh | 0 | Giữ nguyên tại sàn |
| **Vô Danh** | 1.060 | 1.030 | Vô Danh | -30 | Giữ ở bậc Vô Danh |
| **Tân Tú (Ngưỡng)** | 1.100 | 1.050 | Vô Danh | -50 | Khởi đầu lại từ Vô Danh |
| **Tân Tú (Cao)** | 1.200 | 1.100 | Tân Tú | -100 | Chạm đúng ngưỡng Tân Tú |
| **Thiếu Hiệp (Ngưỡng)** | 1.250 | 1.125 | Tân Tú | -125 | Bắt đầu ở Tân Tú |
| **Đại Hiệp (Ngưỡng)** | 1.400 | 1.200 | Tân Tú | -200 | Bắt đầu ở Tân Tú cao |
| **Danh Túc (Ngưỡng)** | 1.600 | 1.300 | Thiếu Hiệp | -300 | Bắt đầu ở Thiếu Hiệp |
| **Tông Sư (Ngưỡng)** | 1.800 | 1.400 | Đại Hiệp | -400 | Bắt đầu ở Đại Hiệp |
| **Tông Sư (Xuất sắc)**| 2.000 | 1.500 | Đại Hiệp | -500 | Bắt đầu ở Đại Hiệp cao |
| **Top 1 Server** | 2.300 | 1.650 | Danh Túc | -650 | Bắt đầu ở Danh Túc |

### Đánh giá tác động của hệ số 0.5:
1. **Không xảy ra tình trạng "đè bẹp người mới":** Nếu hard-reset toàn bộ về 1.000, các cao thủ Tông Sư mang trang bị khủng sẽ nghiền nát người mới ở tuần đầu tiên. Với Soft Reset 0.5, người chơi Tông Sư bắt đầu ở Đại Hiệp (1.400–1.500), cách xa dải ghép $\pm 50$ của người mới (1.000–1.050).
2. **Tạo cảm giác chinh phục lại bậc (Progression):** Cao thủ Tông Sư cần cày lại 400 điểm để lấy lại danh xưng Tông Sư, tạo không khí tranh tài sôi nổi ngay từ ngày đầu mùa mới.
3. **Thu hẹp khoảng cách điểm số:** Các tài khoản có điểm số phình cao (> 2.200) được kéo sát lại nhóm trên (1.600), tạo cơ hội cạnh tranh lại công bằng cho tất cả mọi người.

---

## 6. Kết Quả Mô Phỏng Số Liệu (`tools/gemini/season_sim.py`)

Công cụ mô phỏng được viết và thực thi tại [`season_sim.py`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/tools/gemini/season_sim.py), chạy mô phỏng 500 người chơi có phân bố trình độ thực tế $\mathcal{N}(1000, 150)$, thi đấu 28 ngày liên tục (10 trận/ngày/người), luật ghép trận $\pm 50$ nới dần theo [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L7), Elo K=32, sàn 1.000.

### A. Độ phình điểm (Rating Inflation do sàn 1.000)
Do quy tắc sàn 1.000: Mỗi khi một người chơi ở mốc 1.000 bị thua trận, điểm của họ không bị trừ âm (giữ nguyên 1.000), nhưng người thắng vẫn được cộng điểm ($+\Delta$). Điểm số này được "bơm" (minted) mới vào toàn bộ hệ sinh thái:
- **Mùa 1 (Bắt đầu tất cả ở 1.000):**
  - Tổng số điểm mới sinh ra: **+148.027 điểm** (trung bình $+296,1$ điểm/người chơi; sinh ra $\sim 5.286$ điểm/ngày).
  - Điểm trung bình hệ thống tăng từ 1.000 lên **1.296,1**; điểm trung vị đạt **1.252**; điểm cao nhất đạt **2.163**.
- **Mùa 2 (Sau Soft Reset 0.5):**
  - Số điểm mới sinh ra giảm còn: **+90.977 điểm** ($+182,0$ điểm/người chơi). Lý do: ít người bị kẹt sát sàn 1.000 hơn so với ngày đầu mùa 1.
- **Mùa 3 (Trạng thái cân bằng động - Steady State):**
  - Tỷ lệ sinh điểm ổn định ở mức: **+85.579 điểm** ($+171,2$ điểm/người chơi).
  - Điểm trung bình lúc chốt sổ ổn định ở mức **1.336,1**; trung vị **1.285**; điểm cao nhất **2.321**.

### B. Phân bố bậc lúc chốt sổ (Final Rating - Trạng thái ổn định Mùa 3)

```
  Vô Danh   (1000–1099) : [██████████] 19.0% (95 người)  <- Đáy sàn
  Tân Tú    (1100–1249) : [████████████] 23.8% (119 người)
  Thiếu Hiệp(1250–1399) : [████████████] 22.8% (114 người) <- Trung vị
  Đại Hiệp  (1400–1599) : [██████████] 18.6% (93 người)
  Danh Túc  (1600–1799) : [█████] 10.2% (51 người)
  Tông Sư   (>= 1800)   : [███] 5.6% (28 người)        <- Đỉnh tháp
```

### C. Phân bố bậc cao nhất trong mùa (Peak Rating - Trạng thái ổn định Mùa 3)

| Bậc Luận Kiếm | Tỷ lệ đạt Peak Rating | Nhận xét |
|---|---|---|
| **Vô Danh** | 2.2% | Chỉ 2.2% người chơi hoạt động cả mùa mà không vượt qua được 1.100 điểm |
| **Tân Tú** | 22.4% | Nhóm người chơi có tỷ lệ thắng dưới trung bình (~40–45%) |
| **Thiếu Hiệp** | 24.6% | Nhóm người chơi trung bình (win rate ~48–52%) |
| **Đại Hiệp** | 27.8% | Nhóm người chơi khá (win rate ~53–58%) |
| **Danh Túc** | 14.0% | Nhóm người chơi giỏi (win rate ~59–64%) |
| **Tông Sư** | 9.0% | Nhóm cao thủ hàng đầu (win rate $\ge 65\%$) |

**Kết luận số liệu:** Các mốc bậc định sẵn trong [`arena.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/arena.json) (1.100, 1.250, 1.400, 1.600, 1.800) kết hợp với $K=32$ và sàn 1.000 tạo ra một **kim tự tháp phân bố cực kỳ cân đối và chuẩn mực**:
- Đỉnh kim tự tháp (Tông Sư): 5–9% người chơi xuất sắc nhất.
- Tầng tinh anh (Danh Túc): 10–14%.
- Thân tháp chủ lực (Thiếu Hiệp + Đại Hiệp): chiếm $\sim 50\%$ người chơi.
- Tầng cơ bản (Tân Tú + Vô Danh): chiếm $\sim 30\%$.
- **Không cần thay đổi ngưỡng điểm bậc hay hệ số K=32.**

---

## 7. Kiểm Tra và Cân Bằng Kinh Tế (Danh Vọng & Chí Tôn Ngọc Tủy)

### A. Dòng thu nhập hiện tại (theo luật cũ chưa chỉnh)
Mỗi ngày 10 trận có thưởng: thắng +30, thua +10 Danh Vọng. Mùa dài 28 ngày (4 tuần):

1. **Người chơi đều đặn (Casual - Tỷ lệ thắng 50%, 10 trận/ngày):**
   - Số trận trong mùa: 280 trận (140 thắng, 140 thua).
   - Danh Vọng mỗi ngày: $5 \times 30 + 5 \times 10 = 200$ Danh Vọng/ngày.
   - Danh Vọng cả mùa: $28 \times 200 = 5.600$ Danh Vọng.
   - Tiến độ tuần: Thắng 35 trận/tuần $\ge 10$ trận thắng $\implies$ Hoàn thành 4/4 tuần, nhận **4 Chí Tôn Ngọc Tủy**.
   - Mua tại Tiệm Danh Vọng: Giá 500 DV/viên, giới hạn 1 viên/tuần. Tiêu tốn $4 \times 500 = 2.000$ Danh Vọng $\implies$ Mua đủ **4 Chí Tôn Ngọc Tủy** (vẫn còn dư $3.600$ Danh Vọng để mua nguyên liệu khác).
   - **Tổng thu nhập thường nhật:** Đạt **8 Chí Tôn Ngọc Tủy** mỗi mùa 28 ngày (chưa tính thưởng cuối mùa).

2. **Người chơi giỏi (Hardcore - Tỷ lệ thắng 65%, 10 trận/ngày):**
   - Danh Vọng mỗi ngày: $6.5 \times 30 + 3.5 \times 10 = 230$ Danh Vọng/ngày.
   - Danh Vọng cả mùa: $28 \times 230 = 6.440$ Danh Vọng.
   - Tiến độ tuần: Thắng 45.5 trận/tuần $\implies$ Nhận đủ **4 Chí Tôn Ngọc Tủy**.
   - Mua tại Tiệm Danh Vọng: Mua đủ **4 Chí Tôn Ngọc Tủy** (dư $4.440$ Danh Vọng).
   - **Tổng thu nhập thường nhật:** Đạt **8 Chí Tôn Ngọc Tủy** mỗi mùa (chưa tính thưởng cuối mùa).

### B. Thời gian tích lũy đủ 5 Ngọc Tủy để Hợp Thành 1 món Chí Tôn
Theo [`merge.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/merge.json), nâng từ Thần Binh lên Chí Tôn cần: 2 bản Thần Binh $\ge$ Trọng 3 + 25.000 Bạc + **5 Chí Tôn Ngọc Tủy**.
- Với tốc độ hiện tại (nhận 2 viên/tuần: 1 từ mốc tuần, 1 từ tiệm):
  - Tuần 1: 2 viên
  - Tuần 2: 2 viên (tổng 4)
  - Tuần 3: Vừa sang ngày đầu tuần 3 đã nhận thêm 1 viên $\implies$ **CHỈ MẤT 15 ĐẾN 17 NGÀY LÀ ĐÃ ĐỦ 5 VIÊN NGỌC TỦY!**
- Nếu cộng thêm phần thưởng cuối mùa (1–4 viên theo bậc):
  - Người chơi Thiếu Hiệp (50% win rate): Nhận $8 + 1 = 9$ Ngọc Tủy/mùa (~2 món Chí Tôn).
  - Người chơi Tông Sư (65% win rate): Nhận $8 + 4 = 12$ Ngọc Tủy/mùa (~2.4 món Chí Tôn).

### C. Đánh giá: Có quá nhanh so với Quyết định #24 và #25 không?
- **KẾT LUẬN: TỐC ĐỘ HIỆN TẠI LÀ QUÁ NHANH.**
- **Lý do:**
  1. *Số lượng vật phẩm trong Bao Phục hữu hạn:* Lưới túi là 5×4 = 20 ô. Một bộ đồ hoàn chỉnh của người chơi chỉ gồm khoảng 5 đến 7 món trang bị. Nếu mỗi mùa 28 ngày người chơi nâng được 2 món Chí Tôn, thì chỉ sau **3 đến 4 tháng**, người chơi đã "full đồ Chí Tôn" cả túi!
  2. *Mất giá trị của phẩm tối thượng:* Quyết định [#24](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L544) và [#36](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L436) khẳng định Chí Tôn là phẩm cao nhất trong game (hệ số 1.6), tượng trưng cho đỉnh cao võ học. Nếu kiếm quá dễ dãi sau 2 tuần, Chí Tôn sẽ trở thành món đồ phổ cập thay vì là mục tiêu theo đuổi dài hạn.
  3. *Mốc tuần 10 trận thắng quá dễ:* Người chơi đánh 10 trận/ngày tức 70 trận/tuần. Để đạt 10 trận thắng, họ chỉ cần tỷ lệ thắng vỏn vẹn **14%**! Điều này biến mốc tuần thành quà điểm danh miễn phí chứ không phản ánh nỗ lực thi đấu.

---

## 8. Danh Sách Đề Xuất Thay Đổi (Đánh số để Claude phê duyệt)

Nhằm đảm bảo kinh tế bền vững, tôn vinh phẩm Chí Tôn và vận hành mùa giải trơn tru, Gemini đề xuất danh sách 7 điều chỉnh cụ thể sau:

### [Đề xuất 1] Áp dụng Bảng Thưởng Cuối Mùa Luận Kiếm Đài
- **Nội dung:** Áp dụng bảng thưởng bậc theo **Bậc cao nhất trong mùa (Peak Tier)** và bảng thưởng Top Bảng Xếp Hạng theo **Snapshot thứ hạng lúc chốt sổ** như chi tiết tại Mục 2 của báo cáo này.
- **Lý do:** Khuyến khích người chơi liên tục thi đấu, xóa bỏ tâm lý ngại mất rank cuối mùa.

### [Đề xuất 2] Bổ sung điều kiện tối thiểu 10 trận đấu trong mùa
- **Nội dung:** Chỉ tài khoản đã tham gia khiêu chiến tối thiểu 10 trận trong Kỳ Luận Kiếm mới đủ tư cách nhận phần thưởng mùa (cả thưởng bậc và thưởng top).
- **Lý do:** Ngăn chặn tuyệt đối việc tạo tài khoản ảo/clone nhận điểm sàn 1.000 để trục lợi Danh Vọng miễn phí.

### [Đề xuất 3] Áp dụng công thức Soft Reset $R_{\text{new}} = \max(1000, 1000 + \text{round}((R_{\text{old}} - 1000) \times 0.5))$
- **Nội dung:** Khi sang mùa mới, đặt lại điểm Elo theo công thức thu hẹp 50% khoảng cách trên sàn 1.000.
- **Lý do:** Mô phỏng số liệu đã chứng minh công thức này đưa hệ thống về trạng thái cân bằng động (Steady State) hoàn hảo: điểm trung bình $\sim 1.336$, không dồn cao thủ về đè bẹp người mới, và tạo động lực leo rank 400 điểm cho nhóm Tông Sư.

### [Đề xuất 4] Điều chỉnh nguồn bán Chí Tôn Ngọc Tủy trong Tiệm Danh Vọng (`shop.json`)
- **Nội dung:** 
  - Tăng giá bán Chí Tôn Ngọc Tủy từ 500 Danh Vọng lên **800 Danh Vọng** (hoặc 1.000 Danh Vọng).
  - Điều chỉnh giới hạn mua từ 1 viên/tuần thành **1 viên mỗi 2 tuần** (tối đa 2 viên/mùa 28 ngày).
- **Lý do:** Ngăn chặn việc mua "sỉ" 4 viên Ngọc Tủy mỗi mùa với giá quá rẻ. Buộc người chơi phải cân nhắc phân bổ Danh Vọng giữa Ngọc Tủy và Rương Chất Dẫn.

### [Đề xuất 5] Nâng mốc tuần nhận Ngọc Tủy từ 10 thắng lên 15 trận thắng (`arena_weekly`)
- **Nội dung:** Nâng yêu cầu tích lũy chiến thắng tuần để nhận 1 Chí Tôn Ngọc Tủy từ 10 trận thắng lên **15 trận thắng/tuần**.
- **Lý do:** Với 70 trận đấu/tuần của người chơi tiêu chuẩn, 15 trận thắng yêu cầu tỷ lệ thắng $\approx 21.4\%$. Ngưỡng này vẫn rất dễ đạt với người chơi trung bình (50%), nhưng loại bỏ trường hợp người chơi chỉ vào đánh 1-2 ngày rồi bỏ bê.

### [Đề xuất 6] Tạm hoãn tính năng Trang phục / Khung avatar mùa giải
- **Nội dung:** Tạm thời không đưa trang phục và khung avatar vào gói thưởng mùa đợt này. Tập trung vinh danh hạng 1 bằng danh hiệu text và dấu ấn `[Thiên Hạ Đệ Nhất]`.
- **Lý do:** Hệ thống Ngoại Trang chưa được xây dựng trong mã nguồn và UI; việc hoãn giúp tập trung hoàn thiện ổn định kiến trúc server và luồng PvP đợt 2.

### [Đề xuất 7] Lưu trữ `peak_rating` trong cơ sở dữ liệu máy chủ (`ArenaService`)
- **Nội dung:** Bổ sung cột `peak_rating INTEGER NOT NULL DEFAULT 1000` vào bảng `arena_players`. Mỗi khi điểm số sau trận đấu vượt qua `peak_rating`, server cập nhật giá trị mới.
- **Lý do:** Cung cấp căn cứ kỹ thuật chính xác tuyệt đối để máy chủ tự động kết toán và phát thưởng bậc cao nhất cho người chơi khi mùa giải kết thúc.

---

## 9. Bảng Tiến Độ Sở Hữu Chí Tôn Sau Điều Chỉnh

Với gói điều chỉnh đề xuất ở trên, nhịp độ sở hữu Chí Tôn Ngọc Tủy sẽ trở nên lý tưởng:

| Nhóm người chơi | Ngọc Tủy tuần (4 tuần) | Ngọc Tủy Tiệm (2 viên/mùa) | Thưởng cuối mùa | Tổng Ngọc Tủy / mùa 28 ngày | Thời gian nâng 1 món Chí Tôn (5 viên) | Thời gian hoàn thiện Bao Phục (6 món) |
|---|---|---|---|---|---|---|
| **Người chơi bình thường (Win rate 50%, Bậc Thiếu Hiệp)** | 4 | 2 | 1 | **7 viên** | **~3 tuần** (hoặc 1 mùa) | **~8 – 9 tháng** |
| **Cao thủ xuất sắc (Win rate 65%, Bậc Tông Sư)** | 4 | 2 | 4 | **10 viên** | **~2 tuần** (2 món/mùa) | **~3 – 4 tháng** |
| **Thiên Hạ Đệ Nhất (Quán quân Top 1)** | 4 | 2 | 4 + 3 = 7 | **13 viên** | **~10 ngày** | **~2.5 tháng** |

Nhịp độ này giúp game duy trì được giá trị dài hạn của phẩm Chí Tôn, giữ chân người chơi trong nhiều tháng mà không tạo cảm giác bất khả thi.
