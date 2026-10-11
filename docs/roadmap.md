# Lộ trình

Trạng thái: ⬜ chưa làm · 🟨 đang làm · ✅ xong

| # | Giai đoạn | Nội dung | Ước lượng | Trạng thái |
|---|---|---|---|---|
| 0 | Tài liệu thiết kế | Chốt thuật ngữ, công thức sát thương, 50 vật phẩm MVP trên Sheets, file `.proto` | 2 tuần | 🟨 |
| 1 | battle-engine (Java) | Mô phỏng trận tất định, event bus, unit test, công cụ tự đấu để cân bằng | 3 tuần | 🟨 |
| 2 | Unity: phát lại trận + Bao Phục | Kết nối WebSocket, lưới kéo thả, trình phát lại nhật ký trận | 3–4 tuần | 🟨 |
| 3 | Hành Tẩu Giang Hồ (PvE) | Hồi 1–3, phần thưởng, cường hóa, Tàng Bảo Các, Thương Hội | 4 tuần | ⬜ |
| 4 | Luận Kiếm Đài (PvP) | Ghost, ghép trận theo điểm + Chiến Uy, mùa giải, bảng xếp hạng Redis | 3–4 tuần | ⬜ |
| 5 | Nội dung + hình ảnh | 100+ vật phẩm, 7 môn phái, chơi thử kín | liên tục | ⬜ |

## Giai đoạn 0: việc cụ thể

- [x] Chốt thể loại, công nghệ, phương án đồ giữ lâu dài
- [x] Chốt bộ thuật ngữ võ hiệp
- [x] Đặt tên game, tạo Unity project
- [x] Tạo thư mục docs và quy trình ghi worklog
- [x] Khởi tạo git cho repo
- [ ] Chốt công thức sát thương và các con số cơ bản (nội lực tối đa, chi phí Hóa Kình, thời gian Kiệt Lực…). Đã có đề xuất chạy được trong engine
- [ ] Danh sách 50 vật phẩm MVP (Sheets). Hiện có 14 món trong `items.json`
- [ ] Thiết kế file `.proto` cho giao thức client–server

## Giai đoạn 1: việc cụ thể

- [x] Project Maven `server/` + Maven Wrapper, Java 17
- [x] Mô hình dữ liệu vật phẩm (op, trigger, target) + nạp và kiểm tra JSON
- [x] Lưới Bao Phục: đặt đồ, chồng lấn, liền kề
- [x] Vòng lặp trận: tick, Phổ Công / Tuyệt Kỹ, Hóa Kình, 8 trạng thái, Kiệt Lực, Trọng
- [x] Nhật ký sự kiện (JSON) cho client phát lại
- [x] CLI: chạy 1 trận, tỉ lệ thắng N seed, bảng đối đầu
- [x] 25 unit test
- [x] CLI phân tích nguồn sát thương (`--runs`)
- [x] Cân bằng đợt 1: 5 đội hình mẫu đều trong khoảng 42–57% ([balance.md](design/balance.md))
- [x] Đánh đồng thời thật sự, binh khí ≥ 2 ô, trạng thái tích điểm bộc phát (decisions #10)
- [x] Cân bằng đợt 2: 5 đội hình mẫu trong khoảng 42–60%; 28 unit test
- [x] ~~Hiệu ứng bộ môn phái~~ → bỏ, thay bằng Tâm Pháp (decisions #15)
- [x] Tâm Pháp: `sects.json`, `SectCatalog`, đội hình có `sect` + `tamPhapLevel`; mốc 1 cho 7 phái
- [x] Tâm Pháp mốc 3 / 6 / 9 cho 7 phái (Gemini thiết kế, Claude kiểm chứng: ≤ 70% ở mọi cấp)
- [x] 7 binh khí cho các hệ còn trống (Gemini thiết kế, Claude sửa 3 lỗi và kiểm chứng); bộ mẫu 20 đội
- [ ] Cân bằng tổ hợp Trọng 9 + Tâm Pháp 9 (vài đội ~70–77%)
- [x] Cân bằng vòng 4–6 (Gemini): trận ngắn (> 45 s ≤ 18% mọi Trọng), PvE 30 ải đạt band, bước phẩm/Trọng đạt; ma trận A kẹt ~105/192 → **tạm dừng** đến khi có người chơi thử (#44)
- [x] Icon 7 binh khí mới (ChatGPT) — đủ 21/21 icon vật phẩm
- [x] Hiệu ứng tấn công trong màn phát lại (Codex; hình tạm khi thiếu art)
- [x] Hình tấn công đủ 39 tấm (17 Phổ Công, 17 Tuyệt Kỹ, 5 chung), tách nền bằng `tools/art/ink_to_alpha.py`; xem thử bằng scene `VfxLab` (menu Create VFX Lab Scene)
- [ ] Cơ chế còn thiếu: Côn cắt Tuyệt Kỹ, Cung phá Đan Dược, xoay vật phẩm
- [ ] Hệ thống cấp Hiệp Khách (quy đổi cấp → cấp Tâm Pháp)
- [x] Quyết định về khắc chế: giữ khắc chế giữa các build, PvP ghép ngẫu nhiên (decisions #13)
- [x] Đột Phá: vật phẩm mở thêm hiệu ứng ở Trọng 3/6/9 (decisions #12)

## Giai đoạn 2: việc cụ thể

- [x] Tầng dữ liệu phát lại trận (C#, `Assets/_Game/Scripts/Battle/Replay/`): đọc JSON nhật ký, timeline, trạng thái hai bên tại thời điểm bất kỳ; 13 test EditMode (Codex viết, Claude review)
- [x] Màn hình phát lại trận, bản thử (`Assets/_Game/Scripts/Battle/Replay/View/`): hai bảng Hiệp Khách, hai Bao Phục, số bay, x1/x2/x4, tua, nhật ký tiếng Việt; nhật ký trận có thêm vị trí món trong túi. Codex viết, Claude review; 29 test EditMode
- [x] Lưới Bao Phục kéo thả, bản thử (`Assets/_Game/Scripts/Bag/`): đặt/di chuyển/gỡ, kiểm tra chồng ô và liền kề như server, lưu/đọc đội hình JSON, giao diện dựng bằng code (`BagDemoBootstrap`); 8 test EditMode (Codex viết, Claude review). Chưa có hình, chưa xoay
- [x] Đồng bộ dữ liệu: menu "One Bag Jianghu → Sync Data From Server" (items.json, sects.json)
- [x] Icon thật trong túi đồ và màn phát lại (`ItemIconLibrary`, menu "Rebuild Icon Library"); chọn môn phái + cấp Tâm Pháp trong demo túi đồ
- [x] Luồng màn hình Login → Loading → Home → Gameplay, bản thử (Codex, decisions #16): SceneFlow, LoadingPanel DontDestroyOnLoad, GameSession, 3 scene + Build Settings; trận tạm dùng nhật ký mẫu
- [x] Bảng Cài đặt (âm thanh lưu sẵn, hiển thị, tài khoản/thoát) + banner Home vẽ mới; thưởng đánh lại ải 15% (decisions #47) — 2026-10-04
- [x] Hệ thống popup (BasePopup, PopupManager, 5 popup prefab; decisions #17)
- [x] Scene và prefab có cấu trúc hoàn chỉnh (decisions #20): Services, 5 popup, Login/Home/Gameplay, BagDemo/ReplayDemo/VfxLab; menu Build All Missing
- [x] Test PlayMode tự động cho luồng màn hình (Login → Home → trận offline → kết quả → Home) + 3 scene demo
- [x] Thiết kế chi tiết từng màn (Gemini, `docs/reviews/2026-09-27-gemini-ui-screens.md`)
- [x] Áp dụng quyết định #21: vào trận không chuẩn bị, cảnh báo túi ở Home, bỏ chọn phái cho người mới
- [x] Home bố trí quanh tranh nền (Gemini thiết kế, Codex dựng; còn chờ hình từ ChatGPT)
- [ ] Hệ thống cấp Hiệp Khách (mở Môn Phái ở cấp 10) — Gemini đang thiết kế (`tools/gemini/task-hero-level.md`)
- [~] Hình nền/khung UI: bộ UI kit nhóm A–I do Codex vẽ đã gắn vào game (màn trận, popup, Thương Hội thẻ ngang, Môn Phái, Hành Tẩu Lục) — 2026-10-10/11; còn rà màn Home
- [x] Nhiệm vụ cốt truyện Hành Tẩu Lục (decisions #65, [design/quests.md](design/quests.md)): 45 nhiệm vụ Hồi 1–5, khung theo dõi Home có luồng sáng, sổ nhiệm vụ, hướng dẫn trong tutorial — 2026-10-11; chờ người dùng chơi thử
- [x] Bỏ hệ thống sao PvE (#63); Hợp Thành được món đang trang bị (#64) — 2026-10-10/11
- [x] Chơi offline: engine C# khớp Java từng sự kiện (decisions #19), 32/32 trận đối chiếu qua trong Unity; Gameplay dùng engine C# khi không có mạng
- [~] Âm thanh: hệ thống phát (Codex) + bộ A 28 tiếng (Runway) đã vào game (decisions #57); còn người dùng nghe duyệt, bộ B/C, âm thanh nền. Kế hoạch [audio/sfx-plan.md](audio/sfx-plan.md). Nhạc nền gác ([audio/prompts-music.md](audio/prompts-music.md))
- [~] PvE Hồi 4–5 (ải 31–50) đã vào game (decisions #56); chờ vẽ 11 hình (prompt #50–60) và dựng lại độ khó theo mô hình nâng Trọng
- [~] Kết nối WebSocket (JSON, decisions #18): game-server Java + client Unity đã có code, **tạm hoãn** tích hợp

## Kinh tế và nâng Trọng (quyết định #22)

- [x] E1. Gemini thiết kế kinh tế, hệ phẩm, gộp, 40 vật phẩm phi binh khí → Claude review, chốt #25 (`docs/reviews/2026-09-29-gemini-{economy,rarity-items}.md`). Còn: một đợt cân bằng sau khi engine đọc phẩm
- [x] E2. Codex: hồ sơ người chơi + kho đồ + tiền tệ, lưu file JSON, bộ đồ khởi đầu; túi đồ chỉ dùng đồ đang sở hữu (`tools/codex/task-economy-1-inventory.md`, làm được song song với E1)
- [x] E2b. Codex (`tools/codex/task-rarity-engine.md`): engine Java + C# đọc `rarity` từng bản (hệ số phẩm), công cụ cân bằng, tạo lại trận đối chiếu (#24)
- [~] E3. Codex (`tools/codex/task-storage-upgrade-merge.md`, kèm thưởng sau trận + bảng cheat F1): màn Tàng Bảo Các + nâng Trọng + gộp nâng phẩm (chọn món, xem chi phí, xem trước Đột Phá, xác nhận) — bản đầu chạy được, test qua; còn: chọn bản thứ hai khi Hợp Thành, dropdown đẹp hơn, test PlayMode, người dùng thử
- [~] E4. Codex (`tools/codex/task-shop.md`; thưởng sau trận đã xong ở E3; bản đầu chạy được, test qua, chờ người dùng thử): Thương Hội (danh sách hàng, mua, làm mới, bán lại) + nhận thưởng sau trận (bảng kết quả hiện phần thưởng)
- [ ] E5. ChatGPT: prompt đã sẵn ở `docs/art/prompts-items-nonweapon.md` (40 món + 17 nguyên liệu); còn icon, Bạc/Danh Vọng, khung quầy hàng (Claude viết prompt sau khi chốt tên ở E1)
- [ ] E6. Sau này: chuyển logic kinh tế lên server Java (server quyết định), giữ dữ liệu JSON dùng chung
