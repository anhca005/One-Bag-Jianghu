# Nhật ký quyết định

Mỗi quyết định ghi: ngày, nội dung, lý do. Quyết định mới thêm lên **trên cùng**. Nếu thay đổi quyết định cũ thì ghi quyết định mới và đánh dấu quyết định cũ là *[đã thay thế bởi #n]*.

---

### #66 · 2026-10-11 · Ảnh UI giữ nguyên hình gốc: dùng Simple, không dùng Sliced
- Mọi Image dùng ảnh khung, nút, ô, nền đều để kiểu **Simple** và hiện đúng hình gốc. Không dùng 9-slice (Sliced/Tiled) nữa.
- Chỗ nào có tỷ lệ khác ảnh gốc thì **Codex vẽ ảnh riêng đúng tỷ lệ đó**. Không kéo giãn, không mượn tạm ảnh của chỗ khác.
- Thanh có độ dài thay đổi (máu, khí, tiến độ) dùng **Filled**: ảnh bị cắt bớt chứ không bị kéo giãn.
- Ảnh thiếu thì báo lỗi lúc build prefab. Không lặng lẽ thay bằng ảnh khác.
- **Chuẩn bị cho localize sau này:**
  - Không có chữ nào vẽ sẵn trong ảnh UI. Chữ luôn là Text riêng.
  - Khung có kích thước cố định. Chữ tự co lại cho vừa khung (best fit hoặc shrink), không bắt khung giãn theo chữ.
    Nút và nhãn cần chừa chỗ cho chữ dài hơn tiếng Việt khoảng 30%.

**Lý do:** Sliced làm méo hoa văn so với ảnh vẽ. Ảnh mượn chỗ khác thì sai tỷ lệ và trông tạm bợ.

### #65 · 2026-10-11 · Nhiệm vụ cốt truyện (Hành Tẩu Lục)
- Mỗi Hồi có một chuỗi nhiệm vụ nối tiếp. Nhiệm vụ chính đi theo cốt truyện; việc phụ dạy lối chơi.
- Mỗi lúc chỉ có một nhiệm vụ đang làm, nhận thưởng bằng tay.
- Thưởng gồm Bạc, EXP và nguyên liệu.
- Ở Home có khung theo dõi, kèm popup sổ Hành Tẩu Lục.

Chi tiết ở [design/quests.md](design/quests.md).
**Lý do:** người chơi cần biết bước tiếp theo, và cốt truyện cần sợi dây nối các ải.

### #64 · 2026-10-11 · Hợp Thành được cả món đang trang bị
Không cần gỡ món khỏi Bao Phục trước khi Hợp Thành. Các luật khác giữ nguyên: cùng món, cùng phẩm, cả hai từ 3 Trọng, chưa phải Chí Tôn.

Món mới nằm ở:
- ô của bản được chọn, nếu bản đó đang trang bị;
- ngược lại, ô của bản kia, nếu bản kia đang trang bị;
- ngược lại, Tàng Bảo Các.

Bản bị dùng mất được gỡ khỏi chỗ nó đang nằm.
**Lý do:** người chơi phải gỡ hai món ra, hợp thành, rồi xếp lại, quá vòng vèo.

### #63 · 2026-10-10 · Bỏ hệ thống sao ở PvE
Ải chỉ còn hai trạng thái: đã qua hoặc chưa qua. Hệ thống chấm 1–3 sao theo Khí Huyết còn lại bị bỏ hẳn:
- popup Thắng không còn sao;
- bản đồ không còn sao;
- không còn tiếng chấm sao.

Trường `bestStars` trong save được giữ lại nhưng không dùng, để save cũ vẫn đọc được.
**Lý do:** người dùng thấy sao không cần thiết. Phần thưởng trong popup Thắng giờ hiển thị bằng hình.

### #62 · 2026-10-10 · Quái không dùng đồ riêng; Hồi 1 dễ hơn
- **Bỏ đồ riêng của quái khỏi đồ quái:** 14 món ở 11 ải được thay bằng đồ người chơi mua được, cùng kích thước: Kịch Độc Chu Nha → Thất Tinh Tiêu, Heo Rừng Bắp Giáp → Ô Thiết Giáp, U Minh Quỷ Trảo → Tử Mẫu Song Đao, Bạch Hổ Cuồng Móng → Tử Kim Trảo (ải 4-3 dời một món 1 ô cho vừa). Các món này vẫn nằm trong `items.json` và trong danh sách `monsterItems`, nên shop không bán. Hồi 2–5 chưa chạy thử lại sau khi đổi.
- **Hồi 1 nhẹ hơn**, đo bằng túi gọn (Thanh Phong Kiếm + Ô Tàm Y + Thái Thanh Tâm Kinh, thêm Kim Sang Dược từ ải 5; Trọng 1 ở ải 1–4, Trọng 2 ở ải 5–7, Trọng 3 ở ải 8–10):
  - ải 3: quái xuống cấp 2;
  - ải 5 (tinh anh): Liệt Diễm Đao Trọng 2 thay Tử Mẫu Song Đao;
  - ải 8: còn 4 món Trọng 2 phẩm thường;
  - ải 10 (boss): còn 4 món.
  - Tỉ lệ thắng của túi gọn: ải thường 86–100%, tinh anh 83%, boss 72%. `recommendedPower` của các ải đã đổi được tính lại theo Lực Chiến v4.

### #61 · 2026-10-10 · Lực Chiến theo số ô; đồ quái xếp theo đường Lực Chiến *[thay #55 phần công thức]*
- **Lực Chiến v4 (công thức cộng):** mỗi ô đồ = 10 điểm × hệ số Trọng (1,0→1,4) × hệ số phẩm (1,0→1,6), đúng các hệ số engine dùng; điểm cấp = 30 + 3 mỗi cấp. Món 4 ô đúng bằng 4 lần món 1 ô. Bộ khởi đầu cấp 1 vẫn = 100. `power.json` `version: 1`, C# đã hỗ trợ sẵn.
- **Lực Chiến là con số thiết kế**, không còn đo bằng mô phỏng. Chỉ số thật của vật phẩm chưa đổi: người dùng tự chơi thử rồi quyết.
- **Đường Lực Chiến quái** (`tools/balance/pve_lc_curve.py`):
  - ải thường bám Lực Chiến túi người chơi tiêu biểu ở ải đó (Hồi 1: 85%, từ Hồi 2: 100%), không thấp hơn ải thường trước;
  - tinh anh = ải thường trước +25% (Hồi 1 +15%), boss = ải thường trước +55% (Hồi 1 +30%); đỉnh không cộng dồn, ải sau quay về bám ải thường trước đỉnh.
- **Đồ quái:** giữ vũ khí theo chủ đề; giáp và đồ hỗ trợ theo phe (dã thú, độc, lửa, băng, Kim Cang, Thái Thanh, Thiết Phủ, U Minh); trần Trọng và phẩm theo hồi; `recommendedPower` = Lực Chiến mục tiêu.
- **Túi người chơi tiêu biểu** (ít nhất 2 vũ khí, 1 giáp, tối đa 3 Công Pháp và 3 Đan Dược) là mốc, thay túi "tham" cũ.
- **Lý do:** túi "1 vũ khí + nhồi Công Pháp 1 ô" thắng túi tiêu biểu 79–97,5% ([báo cáo](reviews/2026-10-10-gemini-pve-kits-round5.md)); hai lần agent tự chỉnh chỉ số đều sai hướng. Người dùng chọn đặt Lực Chiến theo ô và tự kiểm bằng chơi thử.
- **Đã lùi phần đồ quái (cùng ngày):** quái dựng theo đường Lực Chiến v4 không thắng được (túi tiêu biểu thắng 0% kể cả khi Lực Chiến bằng nhau), vì Lực Chiến theo ô chưa khớp sức mạnh thật. Đồ quái trở về bản trước (Hồi 1 vòng 4, Hồi 2–5 vòng 3); giữ Lực Chiến v4 cho phần hiển thị và `recommendedPower` = Lực Chiến v4 của quái. Muốn dựng quái theo đường Lực Chiến thì phải chỉnh chỉ số vật phẩm cho đúng theo ô trước.
- **Còn mở:** công cụ đo `tools/balance/item_value.py` cho thấy Công Pháp 1 ô vẫn mạnh 1,1–2,4 lần mỗi ô so với chuẩn ([bảng](../tools/balance/out/item-value/baseline.csv)); nếu chơi thử thấy lệch thì chỉnh chỉ số sau.

### #60 · 2026-10-05 · Nhạc nền và tiếng giao diện
- **Nhạc nền:** bộ Richard Guo (được dùng thương mại, bắt buộc ghi "Music by Richard Guo"). Mỗi màn một bài:
  - Home: 糖葫芦.
  - Bản đồ Hồi 1–5: 碧波茗, 那边黑洞洞, 寒夜雪, 阳春, 黄雀在后.
  - Trận: 剑拔弩张; Thủ Lĩnh: 磅礴; Luận Kiếm Đài: 剑侠无双; Intro: 仙愁.
- **Nhạc ngắn khi thắng/thua:** lúc thắng cắt từ "10 Fanfares" (CC0); lúc thua cắt 6 giây đầu bài 仙愁.
- **Không dùng bộ Crow (乌鸦 Producer)**, vì giấy phép của bộ này cấm chỉnh sửa. Hồi 4 dùng 阳春 thay cho 画桥雨下.
- **File:** nhạc đã chuyển sang OGG trong `Audio/Music/` (khoảng 20 MB). File WAV gốc giữ ở `audio-src/`, thư mục này không đưa vào git. Ghi công trong `Audio/Licenses/music-credits.txt`.
- **Tiếng giao diện:** bỏ tiếng lật/gập sách và tiếng vải, thay bằng tiếng click ngắn (xem [audio/sfx-plan.md](audio/sfx-plan.md) mục 6).

### #59 · 2026-10-04 · Mỗi ải tốn 5 Thể Lực
- Người dùng chốt: mọi ải (cả tinh anh, Thủ Lĩnh) tốn **5** Thể Lực, thay cho 6/8/10/12. Thể Lực tối đa 60, hồi 1 điểm mỗi 6 phút (`Pve/stamina.json`) giữ nguyên → đầy bình đánh được 12 trận.
- Sửa kèm: vào trận không còn làm mất thời gian hồi đang chờ dở (`StaminaService.TrySpend`).

### #58 · 2026-10-04 · Mở đầu, hướng dẫn và Giang Hồ Bách Khoa
- Thiết kế: [design/onboarding.md](design/onboarding.md).
- **Intro** tranh cuộn 5 khung ("Tàn Nang"), bỏ qua được, xem lại trong Cài đặt.
- **Hướng dẫn** 4 chuỗi: xếp túi + ải 1 (kèm 3 gợi ý trong trận đầu), Thương Hội sau ải 1, nâng Trọng/Hợp Thành, Môn Phái ở cấp 10. Lục Tiều Phu dẫn, màn hình tối + khoanh sáng, chỉ bấm được đích; luôn có "Bỏ qua hướng dẫn".
- **Giang Hồ Bách Khoa** ở Home: tra thuật ngữ, chữ tĩnh, viết lại từ `glossary.md` cho người chơi.
- Ải 1 Hồi 1: quái chỉ cầm Thanh Phong Kiếm (đã như vậy từ lần dựng lại ải theo Thương Hội); bộ khởi đầu thắng 100%.

### #57 · 2026-10-04 · Âm thanh: bắt đầu bộ A theo `audio/sfx-plan.md`
- Codex làm hệ thống: `AudioManager` (pool, âm lượng theo `GameSettings`), `SfxLibrary`, `BattleSfxResolver` đọc `sfx-map.json`, `UiSfx` gắn qua builder, móc ở replay, kéo thả, Thương Hội, Tàng Bảo Các, bản đồ ải, hội thoại, kết quả.
- Claude tạo 28 tiếng bộ A (50 bản) bằng Runway, giữ MP3 (chưa chuẩn hóa). Bộ B/C, Tuyệt Kỹ riêng và âm thanh nền làm sau khi người dùng nghe bộ A.
- **Cập nhật cùng ngày:** người dùng nghe bộ Runway thấy rất khó nghe → bỏ; dùng **bộ thu âm thật CC0 của Kenney** (30 khóa, 44 file), khóa nào không có tiếng hợp thì để im. Chi tiết ở `audio/sfx-plan.md` mục 6.

### #56 · 2026-10-04 · PvE mở rộng tới ải 50 (Hồi 4–5); tiêu Bạc qua nâng Trọng
- Người dùng: game không dừng ở ải 50. Codex viết Hồi 4 "Xuân Vũ Kinh Kỳ" và Hồi 5 "Hạ Nhật Đế Kinh"; 3 nhân vật mới (Yến Vô Tâm, Lộ Tẫn Quang, Tào Huyền Kỳ); Giáo chủ U Minh còn tự do để truyện tiếp. Chỉ dùng vật phẩm có sẵn; cấp tối đa giữ 30.
- Đưa vào game ngay (13/20 ải trong dải), hình dùng dự phòng tới khi vẽ (prompt #50–60 trong `art/prompts-pve.md`).
- Mô hình tiến trình cho thấy túi đứng yên từ ải ~20 và Bạc dồn tới 51.070.
- **Kết quả mô hình có nâng Trọng** ([báo cáo](reviews/2026-10-04-codex-pve-upgrade.md)): Bạc không còn dồn (dư 620 trước ải 50 thay vì 51.070), Trọng TB túi 2,2 ở ải 20 → 3,8 ở ải 50; **không cần đổi dữ liệu**. Ải 15–50 dựng lại theo mô hình này (22/36 trong dải, đo trên seed mới) và đã vào game; ải 1–14 giữ nguyên.
- Điểm nghẽn nhỏ cuối game: thiếu nguyên liệu bậc 2 (ải 47–50). Đề xuất tăng ô nguyên liệu bậc 2 từ 5 lên 6 nếu người dùng thấy cần; chưa đổi. Người dùng chọn **nâng Trọng là nơi tiêu Bạc**. Luật nâng Trọng đã tốn Bạc + nguyên liệu; mô hình cũ bỏ qua bước này → Codex mô phỏng đúng rồi dựng lại ải 15–50; nếu vẫn tắc thì báo điểm nghẽn để người dùng quyết.

### #55 · 2026-10-04 · Lực Chiến: công thức kết hợp v2 + số đo v3; LC đề nghị lấy từ túi chuẩn
- **Kiểm chứng ba bản** trên cùng 100 cặp × 30 trận ([báo cáo v3](reviews/2026-10-04-codex-power-v3.md)):

  | Bản | Đúng bên thắng | Log-loss | Xếp đúng 6 vũ khí (Spearman) |
  |---|---|---|---|
  | v1 (cộng điểm) | 80% | 0,39 | thấp |
  | v2 (√(công × thủ), fit theo tỉ lệ thắng) | 87% | 0,34 | 0,28: 16 vũ khí bằng nhau, Song Đao đặt tay 140 |
  | v3 (đo DPS/chịu đòn từ engine) | 82% | 0,58 | 0,84 |
  | **Kết hợp** (chọn) | 81% | 0,37 | 0,84 |

- **Không bản nào đoán đúng trận túi nhỏ mạnh đấu túi quái nhiều món** (ải 1-7: dự đoán ≈10%, thực tế 60%), vì cộng điểm từng món nên nhiều món nhỏ bị tính quá cao.
- **Công thức kết hợp:** giữ v2, chỉ thay lực công của vũ khí = trung vị v2 × √(DPS đo ở v3 / trung vị v3). Không có số đặt tay. Tạo bằng `tools/balance/power_hybrid.py`; bộ khởi đầu cấp 1 = 100.
- **"Lực Chiến +N" của một món** = phần LC món đó cộng thêm vào bộ khởi đầu (Thanh Phong Kiếm + Ô Tàm Y), nên giáp cũng có giá trị.
- **LC đề nghị ở bảng ải** = LC của túi người chơi chuẩn (túi mua ở Thương Hội theo mô hình tiến trình) đã thắng ải đó khoảng 72/62/50% trong mô phỏng. Ghi vào `recommendedPower` của từng ải; so túi người chơi với túi người chơi chính xác hơn so với túi quái. Ải chưa có số thì dùng LC quái × 1,1 như cũ.
- **Lực Chiến vẫn chỉ là chỉ báo**, không quyết định trận.

### #54 · 2026-10-04 · Tử Mẫu Song Đao: công 16 → 15
- **Lý do:** người dùng thấy món này mạnh. Lực Chiến bản 1 và lần đo vũ khí trước cũng báo nó vượt trội.
- **Claude đo trong túi thật của người dùng** (cấp 9, đánh ải 1-7, 60 trận):

  | Vũ khí 4 ô | Thắng |
  |---|---|
  | Song Đao công 16 | 60% |
  | Chấn Nhạc Chùy | 60% |
  | Băng Phách Thương | 63% |

  Nghĩa là Song Đao ngang các vũ khí cùng cỡ, chỉ vượt trội khi so cả vũ khí 2–3 ô.
- **Song Đao rất nhạy theo công:**

  | Công | 15 | 14 | 13 | 11 |
  |---|---|---|---|---|
  | Thắng | 42% | 30% | 18% | 13% |

- **Người dùng chọn 15:** nhẹ hơn hai vũ khí 4 ô kia một chút.
- **Đã sửa:** cả `server/.../data/items.json` và `Resources/Data/items.json` (hai bản giống hệt).

### #53 · 2026-10-04 · Vũ khí trong Thương Hội giá ×2
- **Người dùng:** 200 Bạc cho một vũ khí là quá rẻ.
- **Chọn ×2 cho vũ khí:**

  | Phàm | Lương | Tinh | Tuyệt | Thần Binh |
  |---|---|---|---|---|
  | 400 | 1.000 | 2.400 | 6.000 | 16.000 |

  Loại đồ khác giữ giá theo phẩm.
- **Dữ liệu:** `shop.json` `"itemTypePriceMultipliers": {"weapon": 2.0}`.
  - `ShopService` tính `round(giá theo phẩm × hệ số loại)` rồi mới trừ giảm giá.
  - Loại không ghi thì hệ số 1.
- Dựng lại quái theo túi mua ở shop (#52) dùng giá này.

### #52 · 2026-10-04 · Ải PvE không thưởng vật phẩm; vật phẩm chỉ đến từ Thương Hội
- **Lý do:** quái không còn mang đồ riêng (#50) nhưng ải vẫn tặng đồ riêng đó, nên phần thưởng không khớp quái.
- **Người dùng chọn:**
  - bỏ **cả** vật phẩm thưởng lần đầu **và** đồ rơi ngẫu nhiên;
  - thưởng lần đầu mỗi ải **+100 Bạc** (bù ít).
  - Thưởng ải giờ chỉ còn Bạc, EXP và nguyên liệu. Đánh lại vẫn 15% (#47).
- **Claude áp dữ liệu:** `chapters.json` 30 ải (`items: []`, `drops: []`, `silver +100`). Viết lại 8 câu thoại nói việc nhận hoặc được tặng vật phẩm. Bản trước ở scratchpad.
- **Hệ quả:** người chơi chỉ có đồ từ shop. Đường độ khó #51 (dựng theo túi có đồ thưởng) không còn đúng, nên dựng lại quái theo túi mua ở shop bằng Bạc kiếm được.

### #51 · 2026-10-04 · Lực Chiến: chỉ số sức mạnh cho trang bị và cấp; dựng ải theo Lực Chiến
- **Công thức:**
  - `Lực Chiến = Σ (điểm gốc món × hệ số Trọng × hệ số phẩm) + điểm theo cấp`;
  - số liệu ở `Resources/Data/power.json`;
  - điểm gốc từng món và các hệ số do Codex **hiệu chỉnh bằng mô phỏng**, sao cho Lực Chiến bằng nhau ≈ 50% thắng;
  - bộ khởi đầu cấp 1 ≈ 100.
  - Môn phái và Tâm Pháp chưa tính (bản 1).
- **Hiển thị:**
  - banner Home;
  - Bao Phục (cập nhật khi xếp đồ);
  - tooltip vật phẩm ("Lực Chiến +N");
  - bảng thông tin ải: "Lực Chiến đề nghị" = Lực Chiến quái × 1,1, và Lực Chiến của bạn tô màu xanh / vàng / đỏ.
- **Chỉ là chỉ báo:** không đổi kết quả trận, không chặn vào ải. Thấp hơn đề nghị thì chỉ cảnh báo.
- **Dựng ải:** quái theo #50 (bản sao túi dự kiến). Lực Chiến của quái là thước đo độ khó và đường cong của từng ải.

### #50 · 2026-10-04 · Quái PvE dựng theo "bản sao" túi người chơi dự kiến *(thay cách dò của #49)*
- **Máy dò v2 chỉ đạt 1/30 ải.** Chỉnh phẩm hoặc Trọng cả túi quái thì nhảy thẳng từ 100% xuống 0%.
- **Claude thử bản sao** (túi không mua đồ đánh với chính nó, bớt dần đồ phụ; 20 trận):

  | | Bằng nhau | Kém 1 món | Kém 2 món |
  |---|---|---|---|
  | Người chơi thắng | 30–65% | 75–90% | 90–100% |

  Mỗi món lệch khoảng 30–40 điểm.
- **Người dùng chọn (2):** quái dùng **đúng loại đồ của túi người chơi dự kiến** ở ải đó (cùng món, Trọng, phẩm, vị trí). Tên quái, chân dung và thoại giữ nguyên.
  - **Ải thường:** bớt 1 món không phải vũ khí.
  - **Tinh anh:** đủ món, hạ phẩm 1–2 món.
  - **Thủ Lĩnh:** ngang bằng.
  - Ải lệch dải thì chỉnh một nấc; đo xác nhận 50 trận.
- **Lý do:** độ khó bám đúng sức người chơi nên không có vách; đòn bẩy (số món) đơn điệu và dễ đoán. Đánh đổi là quái ít khác biệt về đồ.

### #49 · 2026-10-04 · Túi quái lớn dần theo túi người chơi; đòn bẩy chính là phẩm/Trọng
- **Máy dò lần 1 (Codex) chỉ đạt 7/30 ải.** Claude đo lại và tìm ra hai nguyên nhân:
  - **Cấp gần như không có tác dụng:** mỗi cấp +5 Khí Huyết. Quái cấp 27 chỉ hơn người chơi cấp 7 khoảng 100 máu.
  - **Quái quá ít đồ:** mỗi ải thưởng 1 vật phẩm, nên tới ải 15 người chơi có 12 món, 3 vũ khí. Quái vẫn 5 món, thường 1 vũ khí.
    - Ví dụ ải 15: quái gây 256 sát thương mỗi trận, người chơi gây 1.065.
    - Nâng phẩm đồ phòng thủ của quái không bù được.
- **Người dùng chọn (a):** mỗi ải, quái mang số món bằng túi người chơi ở ải đó, số vũ khí cũng tương đương.
  - Đồ chọn theo chủ đề của quái (đồ quái đó và các quái cùng Hồi đang dùng).
  - Tên quái và lời thoại giữ nguyên.
  - Máy dò chỉnh **phẩm và Trọng của cả túi quái**; cấp quái để bằng cấp người chơi (Thủ Lĩnh +1).
- **Không chọn:**
  - (b) giảm vật phẩm thưởng;
  - (c) tăng sức mạnh theo cấp trong engine, vì ảnh hưởng PvP.

### #48 · 2026-10-04 · Bạc khởi đầu 500; cân ải bằng máy dò tự động
- **Codex thử thiết kế lại ải theo đề vòng 7, không đạt:**
  - tỉ lệ thắng nhảy giữa 2% và 100%;
  - mô hình túi đồ nhét mọi vật phẩm thưởng nên kín Bao Phục từ ải 9;
  - chỉnh tay quá ít lượt.
  - Báo cáo: `docs/reviews/2026-10-04-codex-pve-redesign.md`. Không tích hợp.
- **Người dùng chọn:**
  - **Bạc khởi đầu 1.000 → 500** (`starter.json`);
  - các số khác của shop giữ nguyên;
  - Codex viết **máy dò tự động**: chọn cấp quái từng ải để túi người chơi thực tế rơi vào dải mục tiêu. Mỗi lần đo 20 trận; bảng chốt 50 trận.
- **Lý do:** cấp quái là đòn bẩy đơn điệu (cấp cao hơn thì khó hơn) nên dò nhị phân rẻ. Đổi ít thứ cùng lúc để biết thay đổi nào có tác dụng.

### #47 · 2026-10-04 · Thưởng đánh lại ải = 15% thưởng lần đầu của chính ải đó
- **Trước:** đánh lại ải nào cũng nhận đúng một gói cố định (mục `win` của `rewards.json`: 60 Bạc, 20 EXP, 1 nguyên liệu Bậc 1, 5% chất xúc tác).
  - Gói này bằng 30% thưởng lần đầu ở ải 1-1 nhưng chỉ 2% ở ải 3-10, nên cày ải đầu lợi nhất.
- **Người dùng chọn:** thắng lại một ải nhận **15%** thưởng lần đầu của chính ải đó.
  - **Bạc và EXP:** 15%, làm tròn, tối thiểu 1.
  - **Nguyên liệu:** 15% số lượng. Phần lẻ thành tỉ lệ rơi thêm 1 món; ví dụ 3 Hàn Thiết thì 45% được 1.
  - **Vật phẩm thưởng lần đầu:** không trả lại.
  - **Đồ rơi của ải:** giữ nguyên.
  - Thua: không có thưởng.
- **Lý do:** thưởng tăng theo độ khó, cày ải cao đáng công hơn, nguyên liệu đúng Bậc của Hồi.
- **Đưa vào dữ liệu:** tỉ lệ là `repeatRewardPercent` trong `chapters.json`, không hardcode. Gemini vòng 7 tính thu nhập đánh lại theo luật này.

### #46 · 2026-10-04 · PvE cân lại theo sức mua thật của người chơi (hướng c)
- **Người dùng chơi thử:** bộ khởi đầu kẹt ở ải 2. Mua thêm 2 món Phàm 1 Trọng (Tử Mẫu Song Đao + Bồi Nguyên Đan) là qua hết Hồi 1.
- **Claude đo bằng `tools/balance/pve_bags.py`:**
  - chỉ bộ khởi đầu: ải 2 thắng 6%;
  - có 2 món mua thêm: 92–100% ở ải 1–9, nhưng ải 12 còn 38% và từ ải 14 gần như 0%;
  - túi dự kiến của vòng 4b có 5–6 món mà vẫn thua túi 4 món này.
- **Kết luận:** ải 2 là vách; đồ mua quá rẻ so với đường cong; Hồi 2 dốc gấp.
- **Người dùng chọn (c), kết hợp cả hai đòn bẩy:**
  - cân ải theo túi đồ người chơi thật sự có được: phần thưởng + Bạc;
  - giảm sức mua shop một phần.
- **Mục tiêu:** túi không mua đồ thắng ≥ 55% mọi ải, không có vách; túi mua khôn ≤ 90% (tinh anh/Thủ Lĩnh ≤ 75%).
- Lệnh Gemini vòng 7: `tools/gemini/task-round-7-pve-economy.md`.

### #45 · 2026-10-03 · Chữ hiển thị: số kiểu Việt, giữ tên dài, "Thủ Lĩnh"
Theo báo cáo rà chữ của Gemini (`docs/reviews/2026-10-08-gemini-text-audit.md`). **Người dùng chọn:**
- **Số:** phân cách hàng nghìn bằng dấu chấm (`1.200 Bạc`), dùng chung một hàm `UiFormat.Number`.
- **Tên dài:** giữ nguyên tên vật phẩm, ải, quái và nút. Giao diện tự co chữ trên một dòng (`SingleLineFit`). Không nhận các bản rút gọn tên của Gemini.
- **Ải cuối mỗi Hồi:** nhãn "Thủ Lĩnh" thay "BOSS"; thêm vào glossary.

**Claude áp 6 sửa cốt truyện/thuật ngữ trong `chapters.json`:** quái ải 1-7 đổi tên thành Thiết Bì Trư Thú cho khớp chân dung, vật phẩm và thoại; thoại bỏ "sinh lực", "tích nộ". Chuỗi trong code giao Codex TX.

**Không nhận:**
- "Thiên Khí" cho Thể Lực (glossary đã có Thể Lực);
- đổi "Côn Lôn Tuyết Liên": Côn Lôn là tên núi, không phải tên riêng của truyện.

### #44 · 2026-10-03 · Nhận vòng 6 (đội hình mẫu); tạm dừng cân bằng ma trận A *(mục tiêu ≤ 58 của #41 hoãn)*
- **Nhận:** 8 đội hình mẫu của Gemini vòng 6 vào `server/battle-engine/samples/` và `Resources/Data/Samples/`:
  - 6 đội hình làm lại có ghi trong báo cáo;
  - `huyen_doc` (xếp lại vị trí) và `huyen_doc_van_doc` (+ Dạ Hổ Y), đổi nhỏ không ghi trong báo cáo;
  - vật phẩm, hằng số và `chapters.json` không đổi; giữ `baseHp` 100 + 5/cấp.
- **Claude chạy lại, khớp báo cáo:**
  - ma trận A 105/192 (không đạt);
  - trận > 45 s ≤ 17,7% ở **mọi** Trọng, lần đầu đạt;
  - bước phẩm 82–87%, 31 ô 100%;
  - PvE 0/30;
  - trung bình hàng 33,8–69,3%, không có hard counter.
- **Người dùng chọn tạm dừng cân bằng:** ba vòng 4–6 đều kẹt quanh 100/192 dù đã đổi máu cơ bản, đội hình và trần Hộ Thân. Sẽ cân bằng lại khi có người chơi thử thật, hoặc khi thêm cơ chế khắc chế mềm vào engine.
- Đã sinh lại golden Parity theo đội hình mới.

### #43 · 2026-10-03 · Bộ font võ hiệp: Charm + Alegreya SC + Alegreya *(thay #37)*
- Claude thử 25 font OFL. Loại 4 font thiếu dấu tiếng Việt: Cinzel, Marcellus, Kaushan Script, Caveat Brush. Ảnh so sánh: `docs/art/font-preview/title-compare-2.png`, `body-compare-2.png`.
- **Người dùng chọn:**

  | Vai | Font | Dùng cho |
  |---|---|---|
  | Display (mới) | **Charm Bold** (bút lông) | tiêu đề lớn |
  | Title | **Alegreya SC Bold** (chữ khắc) | nút, biển hiệu |
  | Body | **Alegreya** Regular | thân bài |
  | Emphasis | Alegreya Bold | chữ nhấn |

- Alegreya chỉ có bản variable. Claude dùng fontTools tách bản tĩnh 400/700, vì Text cũ của Unity không chọn được độ đậm. OFL của Alegreya không có Reserved Font Name nên giữ được tên.
- File và giấy phép OFL nằm trong `Assets/_Game/Fonts/`. Codex F3 (`tools/codex/task-fonts-3.md`) nối vào `UiFonts`. Playfair Display và Literata sẽ gỡ sau khi kiểm xong.

### #42 · 2026-10-03 · Nhận PvE vòng 4b; vòng 5 không đổi gì; giữ trần Hộ Thân 80%
- **PvE vòng 4b** (`docs/reviews/2026-10-05-gemini-round-4b-pve.md`) vào game: `chapters.json` chỉ đổi phần quái.
  - Claude chạy lại `pve_check.py` 200 trận: **0/30 ải vi phạm**.
  - Band có lề 2 điểm; trận ≤ 40 s (ải 1–2 ≤ 30 s).
  - Boss = cấp đề nghị + 1; quái ±2 cấp; Trọng và phẩm bám theo người chơi.
  - Túi người chơi dự kiến chuẩn mới: `tools/gemini/out/round-4b/player/`. `pve_check.py` mặc định dùng thư mục này.
- **Vòng 5** (`docs/reviews/2026-10-06-gemini-round-5-matrix.md`): `patch.json` rỗng, ma trận A vẫn **103/192** (chưa đạt ≤ ~58). Gemini thử 3 hướng, đều làm hỏng chỉ số khác.
- **Trần Hộ Thân 60%: không có tác dụng.** Claude đo lại: ma trận giống hệt, vẫn 103/192. Đội cao nhất chỉ đạt khoảng 57% Hộ Thân. **Giữ trần 80%.**
- **Không build nào có tỉ lệ thắng trung bình dưới 15% hoặc trên 85%** (luật khắc chế cứng của #41 đạt).
- Người dùng chọn **mở đòn bẩy lớn hơn** để vẫn giữ mục tiêu ≤ ~58. Vòng 6 (`tools/gemini/task-round-6-matrix.md`) được phép thử:
  - máu cơ bản / máu theo cấp;
  - làm lại các đội hình mẫu lệch;
  - hệ số Tâm Pháp.
  - Thấp nhất: Thiết Phủ Liệt Diễm 31%.
  - Cao nhất: Kiếm Đạo Hóa Kình 75%.

### #41 · 2026-10-02 · Giữ mục tiêu ma trận A; đo thử trần Hộ Thân 60%
- **Giữ mục tiêu ma trận A ≤ ~58/192 ô ngoài 35–65%.** Hiện là 103/192. Không nới mục tiêu.
- **Giao Gemini đo thử trần Hộ Thân 60%** (`maxDmgReducePermille` 800 → 600) trước khi quyết. Trần hiện tại vẫn là 80%.
- **Thứ tự làm:**
  1. vòng 4b (PvE);
  2. vòng 5 (`tools/gemini/task-round-5-matrix.md`), giữ PvE 4b qua `pve_check.py`.

### #40 · 2026-10-02 · Nhận vòng 4 (Gemini): trận ngắn lại; PvE làm lại ở vòng 4b
Báo cáo: `docs/reviews/2026-10-04-gemini-round-4-pvp.md`. Claude đo lại bằng engine.
- **Nhận vào game:**
  - vật phẩm vòng 4 (`tools/gemini/out/round-4/items-final.json`): vũ khí tăng Công, hồi chiêu vũ khí nặng ngắn lại, Ngọc Cốt Phiến/Kiếm Tông nạp chiêu khi bạo kích yếu đi;
  - hằng số mới: Liệt Hỏa 80, Kịch Độc 32, Xuất Huyết 20 + 3,5% khí huyết tối đa, Điều Tức tối đa 3/giây.
- **Đo lại, từng trận một, ở 4 mốc Trọng:**
  - thời lượng trung bình 23–29 s (trước 33–38 s);
  - trận quá 45 s: 5–19% (trước 25–40%).
- **Ma trận A: 103/192 ô ngoài 35–65%** (trước 131), chưa đạt mục tiêu ≤ ~58.
- **Rarity-step:** trung bình cột 80,9–89,5%, nhưng **31 ô 100%** (trước 20).
- **Tier-step:** T2 > T1 61,1%.
- **`chapters.json` vòng 4 chỉ dùng tạm.** Ở 200 trận, 10/30 ải lệch band, và cấp quái nhảy lung tung:
  - quái lên tới cấp 36–37, vượt cấp tối đa 30;
  - boss không đúng "người chơi + 1 cấp".

  Gemini vòng 4b (`tools/gemini/task-round-4b-pve.md`) làm lại tiến trình quái theo luật cứng mà `tools/balance/pve_check.py` kiểm được.

### #39 · 2026-10-02 · Bộ khởi đầu: 1 kiếm + 1 giáp; không gợi ý trạng thái của địch; boss hơn 1 cấp
- Bộ khởi đầu: Thanh Phong Kiếm + Ô Tàm Y (T1, Phàm), bỏ Thiết Cốt Quyền Sáo.
- Không hiện gợi ý kiểu "Đối thủ dùng Kịch Độc" trước khi vào ải.
- Boss mỗi Hồi hơn người chơi dự kiến 1 cấp, dùng đồ Tuyệt phẩm: giữ.
- Đo: bộ 2 món thắng 0% quái ải 1–3 của vòng 3 → Claude tạm đổi quái ải 1 thành Thiết Cốt Quyền Sáo + Thiết Giáp Y (thắng 100%); Gemini vòng 4 cân lại ải 1–5 theo bộ khởi đầu mới.

### #38 · 2026-10-02 · Đưa số liệu Gemini vòng 3 vào game (Items v3, 30 ải thật)
- Người dùng chọn ghép ngay dù PvP chưa cân bằng: game đang lệch thang số (máu 100, sát thương thang 1200).
- Vào game:
  - `items.json` = `items-v3-final.json` (73 món, có `stats`), ở cả server và Unity;
  - hằng số engine theo `config.json` của Gemini: Liệt Hỏa 65, Kịch Độc 35, Xuất Huyết 15 + 3%, Kiệt Lực bước 5, Điều Tức tối đa 6;
  - 24 đội hình mẫu;
  - `levels.json`;
  - `chapters.json` 30 ải.
- Claude sửa khi ghép:
  - "Liệt Diệm" → "Liệt Diễm", "Vạn Hoa Nghịch Đồ" → "Bách Thảo Nghịch Đồ";
  - bỏ `cols/rows` của quái (luôn 5×4);
  - bỏ `characterId` của 26 quái thường không có trong `characters` (chân dung nằm trên quái);
  - thêm `recommendedLevel` cho mỗi ải (từ đội hình người chơi dự kiến) và hiện "Cấp đề nghị".
- Kiểm chứng:
  - PvE 30 ải chạy lại độc lập, khớp đúng báo cáo, mọi ải trong dải #34;
  - bước phẩm 80–89.5%, bước Trọng 58%.
- **Chưa đạt, giao Gemini vòng 4:** ma trận A 131/192 ô lệch; trận trung bình 37–42 giây, có trận tới 75 giây.

### #37 · *[đã thay thế bởi #43]* · 2026-10-02 · Font chữ: Playfair Display (tiêu đề, nút) + Literata (nội dung)
- Tiêu đề, nút, biển tên, tab, tên ải: **Playfair Display Bold**. Nội dung, tooltip, chỉ số, nhật ký trận, hội thoại: **Literata** (Regular, SemiBold để nhấn).
- Cả hai từ Google Fonts, giấy phép **SIL Open Font License 1.1** (dùng thương mại miễn phí, kèm file giấy phép khi phát hành). Đủ dấu tiếng Việt. Bản tĩnh tách từ bản biến thể chính thức (`github.com/google/fonts`) bằng fontTools, đặt ở `Assets/_Game/Fonts/` cùng `OFL-*.txt`.
- **Lý do:** người dùng chọn bộ A trong 3 bộ đề xuất (ảnh xem trước `docs/art/font-preview/`). Loại các font thư pháp Tây (khó đọc, tràn dòng), font có số kiểu cổ (khó đọc chỉ số).

### #36 · 2026-10-02 · Hệ số phẩm ×1.1 mỗi bậc (thay bảng phẩm ở #35)
- Phàm 1.0 · Lương 1.1 · Tinh 1.21 · Tuyệt 1.33 · Thần Binh 1.46 · Chí Tôn 1.6. Hệ số Trọng giữ 1.00 → 1.40.
- **Lý do:** hệ số nhân vào mọi chỉ số và hiệu ứng nên tác dụng cộng dồn. Đo bằng `rarity-step` trên bộ vật phẩm Gemini vòng 3 (Trọng 3, cấp 8, 40 trận/cặp): bảng 1.0→3.5 cho đội hơn 1 bậc thắng 97–99%; 1.0→2.0 thắng 89–97%; bảng ×1.1 thắng 80–89%. Người dùng chọn ×1.1: hơn phẩm vẫn là lợi thế rõ, nhưng xếp đồ khéo còn lật được.

### #35 · 2026-10-02 · Hệ chỉ số trang bị: máu cơ bản 100, chỉ số theo loại, Trọng tăng nhẹ, phẩm tăng mạnh
- **Khí Huyết cơ bản** = 100 + 5 × (cấp − 1), engine tự tính từ `level` của đội hình. Loadout **bỏ trường `hp`**; client không gửi máu nữa.
- **Mỗi vật phẩm có một dòng chỉ số riêng theo loại:**

  | Loại | Chỉ số | Ghi chú |
  |---|---|---|
  | Binh Khí | **Công** (= Lực Đạo của chính món đó) | Chỉ cộng cho món đó, không cộng sang binh khí khác |
  | Hộ Cụ | **Khí Huyết** | Nguồn máu chính |
  | Kỳ Trân | **Khí Huyết** | Ít hơn Hộ Cụ (người dùng bỏ Tốc Đánh vì trùng với Giảm Hồi Chiêu) |
  | Công Pháp | **Giảm Hồi Chiêu %** | Áp cho mọi vật phẩm có hồi chiêu của Hiệp Khách (binh khí và chiêu chủ động); trần 40% |
  | Đan Dược | **Hộ Thân %** | Giảm sát thương cố định cả trận, cộng chung trần 80% hiện có |

- **Hệ số, áp cho cả dòng chỉ số lẫn hiệu ứng** (khiên, hồi máu, điểm trạng thái…):
  - Trọng tăng nhẹ: 1.00 → 1.40 (+0.05 mỗi Trọng).
  - Phẩm tăng mạnh: Phàm 1.0 · Lương 1.3 · Tinh 1.7 · Tuyệt 2.2 · Thần Binh 2.8 · Chí Tôn 3.5. *[đã thay bởi #36: ×1.1 mỗi bậc]*
  - Đây là giá trị khởi điểm; Gemini đo bằng mô phỏng rồi đề xuất, người dùng chốt. Thay hệ số ở #30 và bảng Trọng cũ.
- **Quái PvE dùng cùng công thức:** máu theo `level` của quái + trang bị. Bỏ chỉnh máu riêng từng ải.
- Toàn bộ con số đang tính theo máu 1200 (hiệu ứng, bộc phát, Kiệt Lực) phải quy đổi lại. Phần cân bằng vật phẩm và độ khó ải của Gemini vòng 2 (`docs/reviews/2026-10-02-gemini-round-2.md`) bị thay thế. Giữ phần đặt tên, cốt truyện và bảng EXP.

### #34 · 2026-10-02 · Sau review Items v2 vòng 1 và nội dung PvE (Gemini)
- **Items v2 vòng 1** (`docs/reviews/2026-10-01-gemini-items-v2-balance-1.md`): Claude chạy lại khớp số (A 108 → 85/160 ô ngoài 35–65%). Mục tiêu ≤ 30% vẫn chưa đạt.
  - **Không nhận** việc hạ trần Hộ Thân toàn cục xuống 40%. Nó chỉ để vá lỗi 0.02 → 100% đã sửa trong converter; bỏ đi thì vẫn 86/160.
  - Nhận 20 đội hình mẫu mới và bản vá vật phẩm (trừ dòng `config:`) làm điểm xuất phát cho vòng 2. **Chưa đưa vào game.**
- **Luật engine mới:** trạng thái gây ra bởi trigger `STATUS_APPLIED` **không kích hoạt lại** trigger `STATUS_APPLIED`. Lý do: Vạn Độc Đỉnh tự lặp tới giới hạn độ sâu 8.
- **Độ khó PvE** ở sức mạnh dự kiến của người chơi: ải thường thắng 80–90%, tinh anh 60–70%, boss 50–60%. Giữ chấm sao ≥ 70% / ≥ 40% Khí Huyết.
- **Môn phái theo #21:** chọn khi Hiệp Khách đạt cấp 10. Đội hình "người chơi dự kiến" dưới cấp 10 không có `sect`. Kịch bản phải có nhịp gia nhập môn phái ở đúng mốc cấp đó.
- **Nội dung PvE vòng 1** (`docs/reviews/2026-10-01-gemini-pve-content.md`) giữ cốt truyện, bối cảnh 3 Hồi, nhân vật và danh sách hình, nhưng phải sửa:
  - "Độc Cô Hạnh" là họ gắn với truyện Kim Dung → đổi tên, khóa hình `npc_phong_loi`. Tên "Vạn Hoa Cốc" trùng game khác → đổi.
  - Đa số ải thắng 100%, quá dễ.
  - `weaponClass: dagger` không tồn tại.
  - Khóa hình gõ sai `hoa_ngiem_xa_cot` → `hoa_diem_xa_cot`.
  - Hai đội hình dùng vật phẩm quái quá yếu (9–20%).
- Prompt hình PvE đợt 2: `docs/art/prompts-pve.md` (#12–49).

### #33 · 2026-10-01 · Kế hoạch Hành Tẩu Giang Hồ (PvE)
Chi tiết: [design/pve.md](design/pve.md).
- Bản đầu có **3 Hồi × 10 ải** (ải 5 tinh anh, ải 10 boss). Cốt truyện tổng viết cho cả game, nhưng chỉ làm chi tiết 3 Hồi đầu.
- **Mỗi Hồi có kịch bản, giao diện và quái riêng:** bảng màu, tranh bản đồ chọn ải, nền trận, nhân vật.
- **Quái trộn:** phần lớn là võ nhân dùng vật phẩm có sẵn; một số ải và boss là quái thú. **Vật phẩm của quái dùng chung với người chơi:** nằm trong `items.json`, rơi từ ải, Thương Hội không bán.
- **Kể chuyện bằng hội thoại trước và sau ải**, có chân dung.
- Dữ liệu ở `Resources/Data/Pve/chapters.json`, theo định dạng trong pve.md.

### #32 · 2026-09-30 · Items v2: làm lại nội tại vật phẩm, thêm cơ chế engine
Người dùng thấy nội tại vật phẩm đơn điệu (86× `battle_start`, 40× `hp_below` trên 188 trigger). Tham khảo `docs/art/tham_khao/tong_hop_vat_pham_tham_khao.xlsx` **chỉ để lấy ý tưởng**, sáng tạo thêm, không chép. Gemini thiết kế lại: `docs/reviews/2026-09-30-gemini-items-v2.md` (65% ý tự nghĩ, 15% gần tham khảo).
- **Giữ nguyên 61 vật phẩm** (id, tên, loại, kích thước, hình) → **không vẽ lại ảnh**. Thanh Đồng Hộ Tâm Kính vẫn là Hộ Cụ 1×1.
- **Cơ chế mới được duyệt**:
  - Kỳ Trân/Công Pháp có hồi chiêu riêng (`cooldown` + `active`), chịu Khinh Công/Trì Trệ/Điểm Huyệt như Binh Khí.
  - Trigger `DAMAGED`, `HEALED`, `CONTROLLED`, `CRIT_TAKEN`, `ENEMY_ULTIMATE`, `QI_EMPTY`, `SHIELD_BROKEN` (bên mình phá hết Cương Khí địch), `BURST_TAKEN`.
  - Op `BUFF_STACK` (cộng dồn có trần), `CLEANSE` (xóa Trì Trệ/Điểm Huyệt; trừ điểm cố định với trạng thái tích điểm), `LIFESTEAL`, `RAISE_MAX_HP`.
  - Điều kiện Hậu Kình: hiệu ứng Tuyệt Kỹ chỉ chạy nếu Nội Lực còn ≥ N sau khi trả.
  - Liên Kích dùng trường `hits` có sẵn (mỗi nhát tính Hội Tâm và trigger riêng).
- **Không duyệt**: `retaliate` (thay bằng `damage` trên trigger `DAMAGED`), `status_threshold_reached` (Giải Độc Đan chuyển sang `BURST_TAKEN`).
- Giữ luật tích điểm bộc phát, không đổi sang "sát thương mỗi giây = số tầng".

### #31 · 2026-09-30 · Bản vá cân bằng vòng 3 (Gemini)
Review `docs/reviews/2026-09-30-gemini-balance-rarity-3.md`; Claude chạy lại khớp số (A 98 → 94/208, B Phàm 24 → 23, B Chí Tôn 23 → 22; trung bình rarity-step 65–72%).
- **Áp dụng**: Ô Thiết Giáp khiên đầu trận 500 → 300; Hộ Mạch Đan khiên cứu nguy 350 → 200; Tử Mẫu Song Đao Lực Đạo 22 → 27, hồi chiêu 2.7 → 2.2 s; Tử Kim Trảo Lực Đạo 6 → 7.
- Kết quả: Tử Mẫu Tốc Kích lên 41–63% từ T3; Kim Cang Cương Khí chỉ hạ ở T1 (69–73%), T3 trở lên vẫn 80–87%.
- **Chưa giải quyết** (vòng 4): Huyền Độc Môn 2–12% (đội hình mẫu mang 2 Hàn Ngọc Tiêu của Hàn Sơn); Huyền Độc Phệ Khí 5–20% (không kết liễu được, trận gương kéo tới Kiệt Lực rồi cùng gục → hòa ở Phàm–Tinh, 100% từ Tuyệt); Kim Cang Bách Luyện nhảy 100% ở rarity-step.
- **Người dùng chốt: giữ luật hòa** (hai bên cùng gục trong một tick = hòa, không phân định theo % Sinh Lực). Đội hình không kết liễu được phải sửa bằng vật phẩm/đội hình. Lệnh vòng 4: `tools/gemini/task-balance-rarity-4.md`.

### #30 · 2026-09-30 · Hệ số phẩm mới 1.00/1.06/1.12/1.19/1.26/1.34 + bản vá cân bằng vòng 2 (thay hệ số ở #25)
Review `docs/reviews/2026-09-30-gemini-balance-rarity-2.md`; Claude chạy lại khớp số (B Phàm 29 → 24/52).
- **Hệ số phẩm**: Phàm 1.00 · Lương 1.06 · Tinh 1.12 · Tuyệt 1.19 · Thần Binh 1.26 · Chí Tôn 1.34 (bước ~6%). Hơn 1 bậc phẩm thắng trung bình **≈ 70%** (64.5 / 66.7 / 74.1 / 70.1 / 75.1%). Trận tất định nên chênh lệch nhỏ đã quyết định thắng thua; chỉ số tuyệt đối của Chí Tôn chỉ hơn Phàm 34%, còn Trọng vẫn tới ×2.0.
- **Bản vá áp dụng**: Thiết Cốt Quyền Sáo Lực Đạo 11 → 9; Thất Tinh Tiêu độc 5 → 4, hồi chiêu 2.1 → 2.3 s; Liệt Diễm Đao Lực Đạo 78 → 70. Giảm số ô ngoài 35–65% ở cả 5 ma trận (A 103 → 98/208).
- Còn lệch lớn (vòng 3): Kim Cang Cương Khí ~90%, Tử Mẫu Tốc Kích 1–24%, Huyền Độc Môn ~10%, Huyền Độc Phệ Khí ~15%.

### #29 · 2026-09-30 · GameController là đối tượng quản lý cả game; log quan trọng bằng Debug.Log
- Người dùng tạo `GameController` ở gốc LoginScene. Nó thay prefab `Services` ẩn: prefab `Resources/Core/GameController.prefab` gồm các con `GameSession`, `SceneFlow`, `NetworkService`, `PopupManager`, `LoadingPanel`; đặt sẵn trong LoginScene, `DontDestroyOnLoad`, chỉ một bản. Chạy thẳng scene khác trong Editor thì tự tạo từ prefab.
- Log các sự kiện quan trọng bằng `Debug.Log` có tiền tố lọc: `[Game] [Scene] [Profile] [Shop] [Storage] [Bag] [Battle] [Reward] [Popup] [Cheat]`; thất bại dùng `LogWarning`.
- Giao Codex: `tools/codex/task-game-controller-logs.md`.

### #28 · 2026-09-30 · Sau review cân bằng theo phẩm (Gemini)
Review `docs/reviews/2026-09-30-gemini-balance-rarity.md`: bản vá của Gemini làm **tệ hơn** (Claude chạy lại: ô ngoài 35–65% ở T1/T9 TP1 tăng 23/52 → 31/52; báo cáo cũng ghi A 107 → 116/208) và phần mô tả lệch số trong patch → **không áp dụng**.
- **Mục tiêu 1 bậc phẩm:** hai đội giống hệt, chỉ khác 1 bậc phẩm → bên cao thắng **≈ 65–75%** (Gemini đo hiện tại 80–100%). Hệ số phẩm sẽ chọn bằng mô phỏng (công cụ cân bằng được thêm cách ghi đè hệ số), không đoán tay; đề xuất 1.00/1.15/1.32/1.52/1.75/2.00 của Gemini là điểm xuất phát.
- **Trần Cương Khí = 100% Sinh Lực tối đa** (phần vượt bị bỏ), giống trần Điều Tức.
- **Tâm Pháp:** Song Đao (`dual`) thuộc **Liệt Diễm**, Cung (`bow`) thuộc **Hàn Sơn**.

### #27 · 2026-09-30 · Tàng Bảo Các dạng lưới 10×20 = 200 ô; Thương Hội lưới 2×3
- **Tàng Bảo Các** bắt đầu với **200 ô**, lưới **10 cột × 20 hàng** (cuộn dọc), **xếp như Bao Phục**: mỗi món chiếm **đúng kích thước thật** (vd kiếm 1×3).
- Món mới vào kho **tự xếp** vào chỗ trống đầu tiên vừa kích thước; người chơi **kéo thả** để sắp lại; nút **Sắp xếp** tự gom gọn.
- **Nguyên liệu cũng chiếm ô**: mỗi loại 1 ô 1×1, cộng dồn tối đa 999 mỗi ô, quá thì sang ô mới.
- Đồ đang xếp trong **Bao Phục không nằm trong kho**; lấy khỏi túi thì cần chỗ trống trong kho để cất lại (không đủ chỗ → không cho lấy ra, báo lý do).
- Kho không còn chỗ vừa: không mua được món đó ở Thương Hội (nút Mua khóa, báo "Tàng Bảo Các không đủ chỗ"), phần thưởng không cất được thì báo. Mở rộng kho để sau (thay con số trong đặc tả kinh tế của Gemini).
- **Thương Hội**: 6 ô hàng hiển thị lưới **2 hàng × 3 cột**, mỗi ô một món.

### #26 · 2026-09-30 · Bố cục màn trận (Gameplay) mới
Theo bản phác của người dùng:
- Một dải ngang giữa màn hình, trái → phải: **avatar mình, túi đồ mình, khoảng trống cho đòn bay, túi đồ địch, avatar địch**. Nền trận vẫn thấy.
- **Sinh Lực, Cương Khí, Nội Lực** và biểu tượng trạng thái nằm **dưới avatar** mỗi bên. Hình trúng đòn, Bộc Phát, Hóa Kình, số sát thương hiện ở avatar; đòn bay đi từ vật phẩm sang avatar đối thủ.
- **Dải trên:** tên ải/đối thủ, đồng hồ trận, nút Thoát. **Dải dưới:** tạm dừng + tốc độ x1/x2/x4, nhật ký trận thu gọn (3 dòng, bấm để mở).
- Giao Codex: `tools/codex/task-gameplay-layout.md`.
- **Cấu trúc scene (bổ sung #20):** đối tượng đặt phẳng ở gốc scene, không bọc trong đối tượng cha mang tên scene: `Main Camera`, `EventSystem`, controller/bootstrap, `Canvas`. **Scene nào cũng có Main Camera** (tag MainCamera, có AudioListener). Chỉ một EventSystem hoạt động. Giao Codex: `tools/codex/task-scene-roots.md` (chạy sau task L vì cùng sửa builder màn trận).

### #25 · 2026-09-29 · Chốt hệ phẩm, gộp, 61 vật phẩm và tên mới (sau review Gemini)
Review `docs/reviews/2026-09-29-gemini-rarity-items.md` + bản sửa kinh tế:
- **Hệ số phẩm** (nhân chỉ số): Phàm 1.00 · Lương 1.25 · Tinh 1.55 · Tuyệt 1.90 · Thần Binh 2.30 · Chí Tôn 2.75.
- **Phẩm nhân đúng những chỉ số Trọng đang nhân**: Lực Đạo, `value`/`stacks` của hiệu ứng (khiên, hồi máu, số điểm trạng thái…). Không nhân nội lực, thời gian, chi phí, tỉ lệ. Chỉ số cuối = gốc × hệ số Trọng × hệ số phẩm (đường cong Trọng giữ nguyên như engine: 1.0 → 2.0).
- 21 món cũ quy về gốc Phàm **bằng script** theo đúng quy tắc trên (chia theo phẩm cũ), không dùng bản quy đổi tay của Gemini (bản đó chia cả nội lực và dùng sai đường cong Trọng). 40 món phi binh khí mới của Gemini giữ nguyên số.
- **Gộp**: Trọng ra = clamp(floor((Trọng A + Trọng B) / 3), 2, 7); hoàn 50% nguyên liệu; Bạc 500 / 1.500 / 4.000 / 10.000 / 25.000 cho 5 bước; bước Thần Binh → Chí Tôn cần thêm **5 Chí Tôn Ngọc Tủy** (`chi_ton_ngoc_tuy`), chỉ có từ Luận Kiếm Đài (mốc 10 thắng/tuần, đổi 500 Danh Vọng tối đa 1 viên/tuần, thưởng mùa). Giao diện cảnh báo nếu món gộp ra yếu hơn bản mạnh nhất đem gộp.
- **Đổi tên** (tránh trùng truyện/game có sẵn): Huyền Thiết → Ô Thiết (`o_thiet`), Huyền Thiết Giáp → Ô Thiết Giáp, Đại Hoàn Đan → Hồi Thiên Đan, Bắc Đẩu Trận Pháp → Tinh Đồ Trận Phổ, Trường Sinh Quyết → Dưỡng Sinh Quyết, Kim Ti Giáp → Kim Tuyến Y, Linh Tê Y → Nho Sam Y.
- Hiệu ứng Tâm Pháp cộng vào binh khí (tầng trạng thái, khiên…) cũng nhân theo phẩm của bản đó, giống như đang nhân theo Trọng. Vì vậy đội hình mẫu dùng đồ phẩm cao lệch vài điểm % so với trước; để đợt cân bằng xử lý.
- Mô phỏng Gemini (toàn bộ ở Phàm) còn 15/24 ô ngoài 35–65% → **cần một đợt cân bằng** sau khi engine đọc được phẩm; chưa chặn việc thử trong Unity.

### #24 · 2026-09-29 · Nâng phẩm bằng cách gộp 2 bản cùng món (thay phần "độ hiếm cố định" của #22)
Người dùng chọn "cách 2": **độ hiếm là thuộc tính của từng bản vật phẩm người chơi sở hữu**, không cố định theo món.
- Mỗi món trong `items.json` mang **chỉ số gốc ở Phàm Phẩm**; phẩm cao hơn nhân chỉ số theo bảng hệ số phẩm (giống cách Trọng nhân chỉ số). Đột Phá vẫn theo Trọng 3/6/9.
- **Gộp:** 2 bản **cùng món, cùng phẩm, đều ≥ Tam Trọng** → 1 bản phẩm cao hơn một bậc; Trọng giữ khoảng một nửa; hoàn một phần nguyên liệu đã nâng. Số cụ thể do Gemini đề xuất.
- Gộp thông thường (2 bản + nguyên liệu + Bạc) đi tới **Thần Binh**. Bước lên phẩm cao nhất **Chí Tôn** (Thần Binh → Chí Tôn) **bắt buộc** có thêm **nguyên liệu đặc biệt chỉ lấy từ Luận Kiếm Đài** (PvP); không có nguyên liệu này thì không gộp lên Chí Tôn được. Tên, số lượng và cách rơi do Gemini đề xuất.
- Nâng Trọng vẫn không thất bại; Thương Hội vẫn chỉ là NPC (phần còn lại của #22 giữ nguyên).
- Kỹ thuật: engine Java + C# thêm `rarity` vào vật phẩm trong đội hình, tạo lại bộ trận đối chiếu, cân bằng lại. Hình icon dùng chung, phẩm thể hiện bằng màu khung.

### #23 · 2026-09-29 · Chốt khung cấp Hiệp Khách + kinh tế; thêm vật phẩm phi binh khí
Sau khi review `docs/reviews/2026-09-29-gemini-hero-level.md` và `2026-09-29-gemini-economy.md`:
- **Túi đồ giữ cố định 5×4** cho mọi cấp (không mở rộng theo cấp). Phần thưởng lên cấp là mở tính năng / tiền / nguyên liệu, không phải ô túi.
- **Chưa dùng Thể Lực** ở bản offline (giữ sẵn trường dữ liệu, bật khi có server).
- **Tàng Bảo Các có ngay từ đầu** (kho chứa đồ ngoài túi và là nơi nâng Trọng).
- **Thêm vật phẩm phi binh khí**: mỗi loại Hộ Cụ, Công Pháp, Đan Dược, Kỳ Trân **tối thiểu 10 món** (hiện mỗi loại chỉ 1), để 4 nhóm nguyên liệu đều có đồ dùng. Gemini thiết kế, Claude kiểm chứng cân bằng.
- Nguyên liệu rơi theo tỉ trọng loại đồ; nhóm Công Pháp / Đan Dược–Kỳ Trân phải có nguồn rơi PvE/PvP.

### #22 · 2026-09-29 · Kinh tế: cửa hàng NPC, nguyên liệu nâng Trọng theo loại đồ, nâng không thất bại
- **Thương Hội chỉ là cửa hàng NPC**: bán vật phẩm và nguyên liệu, làm mới hàng bằng Bạc. **Không có chợ giữa người chơi.**
- **Nâng cấp = nâng Trọng 1 → 9** (giữ quyết định #12, Đột Phá ở 3/6/9). ~~Độ hiếm của mỗi món cố định, không nâng phẩm.~~ *[đã thay thế bởi #24: nâng phẩm bằng gộp]*
- **Nâng không thất bại**: đủ nguyên liệu + Bạc là lên Trọng.
- **Nguyên liệu chia theo loại đồ** (ví dụ binh khí dùng khoáng như Hàn Thiết; hộ cụ, công pháp, đan dược/kỳ trân dùng loại khác), mỗi loại có vài bậc theo khoảng Trọng; rơi từ PvE/PvP và bán ở Thương Hội.
- Số liệu (tên nguyên liệu, giá, bảng chi phí, nguồn rơi, nhịp độ) do Gemini thiết kế, Claude kiểm chứng; dữ liệu là JSON (data-driven), sau này server Java là bên quyết định.

### #21 · 2026-09-29 · Môn phái mở ở cấp 10; túi xếp ngoài trận; Home bố trí quanh tranh nền
Trả lời các câu hỏi mở trong đặc tả UI của Gemini (`docs/reviews/2026-09-27-gemini-ui-screens.md`).
- **Môn phái:** tài khoản mới **không có môn phái** (không có Tâm Pháp). Khi Hiệp Khách đạt **cấp 10** mới mở giao diện môn phái để chọn. Login chỉ nhập tên. Cần hệ thống cấp nhân vật (chưa có, ghi vào roadmap). Trong lúc chưa có, đội hình của người chơi không có `sect`; bộ chọn phái chỉ còn trong công cụ thử (BagDemo).
- **Túi đồ xếp ở ngoài (Home → Bao Phục).** GameplayScene **không còn giai đoạn chuẩn bị**: vào là đánh, rồi phát lại, rồi kết quả; không chỉnh túi trong trận.
- **Túi trống / không có binh khí:** ở Home, ngay **phía trên nút vào trận** hiện dòng chữ cảnh báo, nút không cho vào trận.
- **Home bố trí các lối vào quanh tranh nền** (điểm bấm rải quanh khung cảnh), không xếp một cột nút ở giữa.
- **Chưa cho xoay vật phẩm** ở MVP.
- **Hai cửa vào trận ở Home** (bổ sung cùng ngày): nút lớn ở đáy màn hình vào thẳng trận; điểm bấm trong tranh (con đường, lôi đài) sau này mở màn chọn ải / sảnh Luận Kiếm, hiện tạm cũng vào thẳng trận. Bố cục Home theo `docs/reviews/2026-09-29-gemini-home-layout.md`.

### #20 · 2026-09-29 · Scene và prefab có cấu trúc hoàn chỉnh, không dựng UI bằng code lúc chạy
Người dùng yêu cầu mọi scene phải là scene hoàn chỉnh, mở ra trong Editor thấy đủ cây đối tượng, không phải một object rỗng rồi Play mới sinh giao diện.
- **Phần tĩnh** (Canvas, EventSystem, khung, nút, chữ, panel) nằm sẵn trong scene hoặc prefab. Script chỉ giữ tham chiếu `[SerializeField]` và gắn logic, không tạo UI trong `Awake`/`Start`.
- **Phần động** (ô vật phẩm trong túi, dòng danh sách, hình VFX) được tạo lúc chạy **từ prefab** (`Instantiate`), không dựng từng component bằng code.
- Dịch vụ xuyên scene (LoadingPanel, gốc PopupManager) là prefab; `GameBootstrap` chỉ `Instantiate` prefab đó.
- Menu dựng scene/prefab trong Editor chỉ dùng để **tạo lần đầu**, không ghi đè khi đã có (bản trong scene/prefab là bản gốc để chỉnh tay). Muốn dựng lại phải chọn menu "ghi đè" riêng, có hộp xác nhận.
- Chi tiết quy ước: `docs/tech/ui-flow.md`, mục "Scene và prefab".

### #19 · 2026-09-28 · Chơi offline trước: chép battle-engine sang C#
Người dùng chọn làm phần offline trước, server để sau. Engine được chép sang C# (`Assets/_Game/Scripts/Engine/`, không phụ thuộc UnityEngine) để Unity tự tính trận từ đúng túi đồ của người chơi.
- **Engine Java vẫn là bản gốc**, và là bên quyết định khi chơi online (quyết định #3 giữ nguyên cho online).
- **Quy tắc song song:** mọi thay đổi engine Java phải chép sang C#; bộ trận đáp án do Java tạo (`tools/parity/make_golden.py`, 32 trận) phải khớp **từng sự kiện** trong test Unity.
- Module `server/game-server` (WebSocket, Codex đã làm, 51 test Java qua) để dành, chưa tích hợp.

### #18 · 2026-09-28 · Giao thức MVP: WebSocket + JSON
Bản MVP dùng JSON trong phong bì `{type, id/replyTo, payload}` qua WebSocket (`ws://localhost:8765/ws`) thay vì Protobuf như kế hoạch ban đầu: dễ gỡ lỗi, client đã có Newtonsoft, engine đã xuất JSON. Chuyển Protobuf khi cần tối ưu. Server ép hp/kích thước túi, tự chọn đối thủ và seed. Chi tiết: [tech/protocol.md](tech/protocol.md).

### #17 · 2026-09-27 · Màn phụ dùng popup (BasePopup + PopupManager)
Yêu cầu người dùng: các UI phụ là popup, khi cần mới gọi ra. Lớp nền `BasePopup` (Setup, Show, Hide/Close, OnEnable/OnDisable, OnShown/OnHidden, OnBackPressed, Refresh), `PopupManager` quản lý ngăn xếp, cache, nền mờ, phím Esc. Popup là prefab trong `Resources/UI/Popups/`. Chi tiết: [tech/ui-flow.md](tech/ui-flow.md#popup).

### #16 · 2026-09-27 · Luồng màn hình và độ phân giải
- Luồng: **LoginScene → LoadingPanel → HomeScene → GameplayScene** (yêu cầu người dùng). LoadingPanel không phải scene mà là một Canvas `DontDestroyOnLoad`, luôn trên cùng (`sortingOrder` 1000), hiện khi chuyển scene. Mọi lần chuyển scene đi qua `SceneFlow`.
- Độ phân giải chuẩn 1920×1080 (16:9), co giãn theo màn hình (match 0.5), tối thiểu 1280×720. Chi tiết: [tech/ui-flow.md](tech/ui-flow.md).

### #15 · 2026-09-27 · Bỏ hiệu ứng bộ môn phái; môn phái chỉ có Tâm Pháp
Yêu cầu từ người dùng.
- **Vật phẩm trung lập:** bỏ trường `sect` khỏi vật phẩm. Hiệp Khách phái nào cũng dùng được mọi binh khí.
- **Tâm Pháp** là nội tại của môn phái (không phải vật phẩm): buff các hệ binh khí của phái. Mỗi phái 1 Tâm Pháp, mạnh dần theo cấp với các mốc 1 / 3 / 6 / 9 cộng dồn.
- Loại vật phẩm `manual` trước gọi là Tâm Pháp nay đổi tên thành **Công Pháp** (mã giữ nguyên `manual`).
- Bỏ hiệu ứng bộ môn phái 2/4/6 món (thiết kế cũ trong items.md).
- Kỹ thuật: `sects.json` + `SectCatalog`; đội hình có `sect` và `tamPhapLevel`; `SectDef.applyTo` gộp Tâm Pháp vào định nghĩa binh khí sau Đột Phá, nên phần mô phỏng không phải sửa.

### #14 · 2026-09-27 · Điều Tức có mức tối đa; mục tiêu cân bằng "không build áp đảo"
- Điều Tức tối đa **30 khí huyết/giây**; phần cộng vượt mức bị bỏ. Lý do: cộng dồn không giới hạn làm đội có 2 Cầm thắng 99%.
- Mục tiêu cân bằng (chỉnh lại #13): không build nào có tỉ lệ thắng trung bình vượt ~70% ở mọi mốc Trọng. Build dưới 35% được chấp nhận nếu đó là cách xếp túi dở, miễn là mọi món đều xuất hiện trong ít nhất một build tốt.
- Cân bằng bằng bộ dò tự động trên 15 đội hình mẫu ở Trọng 1/3/6/9 (xem [design/balance.md](design/balance.md#đợt-3--2026-09-27)).

### #13 · 2026-09-27 · Giữ khắc chế giữa các build
Người dùng chọn giữ khắc chế lẫn nhau, không thêm ngẫu nhiên để làm mềm. Trận đấu vẫn gần như tất định: một build có thể thắng hẳn một build khác.
Mục tiêu cân bằng đổi thành: tỉ lệ thắng trung bình của mỗi build trong **35–65%**, và mỗi build phải có ít nhất một build khắc chế nó. Từng cặp riêng lẻ được phép lệch 0–100%.
**PvP ghép ngẫu nhiên:** không cho xem đối thủ trước trận, không đổi túi theo đối thủ. Gặp đối thủ khắc chế là một phần may rủi; điểm hạng phản ánh sức mạnh qua nhiều trận. Vì vậy tiêu chí trung bình 35–65% càng quan trọng: build yếu tổng thể sẽ tụt hạng dù có vài cặp thắng tốt.

### #12 · 2026-09-27 · Vật phẩm lên Trọng mở thêm hiệu ứng (Đột Phá)
Yêu cầu từ người dùng. Mỗi món vẫn là một vật phẩm riêng, có độ hiếm riêng, và có 9 Trọng nâng bằng tài nguyên PvE. Trọng 3 / 6 / 9 là **Đột Phá**, mở thêm hiệu ứng, cộng dồn các mốc trước. Hệ số Trọng chung vẫn giữ.
Kỹ thuật: trường `unlocks` trong items.json; `ItemDef.atTier(tier)` gộp các mốc ≤ Trọng thành định nghĩa dùng trong trận, nên phần mô phỏng không phải sửa. Chi tiết và bảng Đột Phá ở [design/items.md](design/items.md#đột-phá).

### #11 · 2026-09-27 · Phong cách hình ảnh: Thủy Mặc
Người dùng chọn sau khi thử hình [style board](art/reference/01-style-board-thuy-mac.png). Nét cọ mực, mảng mực loang trên nền giấy dó, son đỏ điểm nhấn, nhân vật nét anime kiếm hiệp. Bảng màu 6 màu và khóa phong cách ở [art/art-pipeline.md](art/art-pipeline.md). Các phong cách còn lại trong [style-options.md](art/style-options.md) chỉ để tham khảo.
Khóa phong cách viết theo hình thực tế: cho phép vệt mực và nét cọ khô, vẫn cấm dấu hiệu 3D (phản sáng, phát sáng, chiều sâu ống kính).

### #10 · 2026-09-27 · Đánh đồng thời, binh khí ≥ 2 ô, trạng thái tích điểm bộc phát
Yêu cầu từ người dùng.
- **Hai bên đánh đồng thời:** trong mỗi tick 100ms, mọi đòn đã ra của cả hai bên đều được thực hiện (xen kẽ A–B, bên dẫn đầu chọn bằng random có seed); tử trận chỉ xét cuối tick. Đòn ra cùng lúc với cú kết liễu vẫn trúng; hai bên cùng gục → hòa; Định Thân gây trong tick không hủy được đòn đối thủ đã ra trong cùng tick.
- **Binh khí tối thiểu 2 ô** (`ItemCatalog.MIN_WEAPON_CELLS`). Quyền Sáo, Trảo, Ám Khí chuyển sang 1x2.
- **Liệt Hỏa / Kịch Độc / Xuất Huyết tích điểm**, không gây sát thương theo thời gian. Đủ 50 điểm → bộc phát rồi reset về 0 (điểm dư bị bỏ). Liệt Hỏa 300 (Cương Khí đỡ được), Kịch Độc 240 (xuyên Cương Khí), Xuất Huyết 150 + 10% khí huyết tối đa (xuyên Cương Khí). Thêm trigger `burst`. Tất cả nằm trong `BattleConfig.Burst`, có sẵn tùy chọn điểm tự giảm theo giây (đang tắt).

### #9 · 2026-09-27 · Luật và quy tắc sau cân bằng đợt 1
Chi tiết và số liệu ở [design/balance.md](design/balance.md).
- *[đã thay thế bởi #10]* **Xuất Huyết** mất một nửa số tầng sau mỗi lần bộc phát. **Lý do:** cộng dồn vô hạn khiến sát thương tăng theo cấp số nhân với số lần đối thủ ra đòn.
- **Hóa Kình** gây Định Thân 1 giây (trước là 2). Thêm hiệu ứng `parry_discount`, chi phí Hóa Kình tối thiểu 20. **Lý do:** đội không có Tuyệt Kỹ biến thành "máy Hóa Kình" khóa đối thủ liên tục.
- *[đã thay thế bởi #10]* **Bên đi trước mỗi tick** chọn bằng random có seed thay cho đổi theo tick chẵn/lẻ. **Lý do:** lỗi, bên A luôn thắng khi hai bên ra đòn cùng lúc.
- **Quy tắc thiết kế vật phẩm:** ngân sách sát thương/giây theo ô × hệ số độ hiếm; binh khí không có Tuyệt Kỹ chỉ cho ≤ 1 nội lực/giây; khống chế toàn bộ đồ địch ≤ 1 giây mỗi lần.

### #8 · 2026-09-27 · Thiết kế battle-engine
- Tick 100 ms, thời gian và chỉ số dùng số nguyên (tỉ lệ theo ‰) để kết quả tất định.
- Random dùng SplitMix64 tự viết thay vì `java.util.Random`: chuỗi số không đổi giữa các bản JDK và dễ port sang C#.
- Vật phẩm hoàn toàn data-driven: hiệu ứng ghép từ ~12 `op` + trigger, không viết code riêng cho từng món.
- Nội lực là quỹ chung của Hiệp Khách. Bên đi trước đổi luân phiên mỗi tick *[đã thay thế bởi #9: chọn bằng random có seed]*.
- Hai bên cùng gục → hòa.

### #7 · 2026-09-27 · Server dùng Java 17 (thay cho 21 trong kế hoạch)
Máy dev đang có JDK 17 (LTS). Build bằng Maven Wrapper (`server/mvnw`), không cần cài Maven. Có thể nâng lên 21 sau mà không phải sửa code.

### #6 · 2026-09-27 · Tên game: One Bag Jianghu (Nhất Nang Giang Hồ)
"Cả giang hồ gói trong một túi", nói đúng lối chơi xếp túi đồ.

### #5 · 2026-09-27 · Thuật ngữ võ hiệp riêng, không dùng tên của game tham chiếu
Bảng chuẩn ở [design/glossary.md](design/glossary.md). Môn phái và vật phẩm dùng tên tự đặt, tránh tên riêng từ truyện Kim Dung để không vướng bản quyền.

### #4 · 2026-09-27 · Vật phẩm giữ lâu dài (phương án B)
Đồ kiếm được ở PvE được giữ và mang vào PvP, không reset theo lượt chơi.
**Lý do:** tạo động lực cày PvE và sưu tầm đồ.
**Rủi ro:** pay-to-win và chênh lệch trong PvP. Cách giảm thiểu xem [design/gdd.md](design/gdd.md#pvp-công-bằng).

### #3 · 2026-09-27 · Server có quyền quyết định, client chỉ phát lại trận
Server Java chạy mô phỏng trận và trả nhật ký sự kiện; Unity phát lại.
**Lý do:** client (C#) và server (Java) không dùng chung được code; đồ có giá trị lâu dài nên cần chống hack; phát lại và xem nhật ký có sẵn.

### #2 · 2026-09-27 · Công nghệ: Unity (client PC) + Java WebSocket (server)
Hình ảnh do ChatGPT tạo.

### #1 · 2026-09-27 · Thể loại: auto-battler xếp túi đồ, 2D hình tĩnh, có PvE và PvP
PvE đi ải theo chương để lấy tài nguyên và kinh nghiệm; PvP đua top để nhận đồ hiếm và trang phục.
