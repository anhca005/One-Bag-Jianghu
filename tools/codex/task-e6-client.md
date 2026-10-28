# Task: E6 client — hồ sơ từ server, đăng nhập khi vào game, mọi thao tác gửi lên server

Bạn là Codex, làm việc trong Unity project One Bag Jianghu (Unity 6000.3.9f1, UGUI, Input System).

## Đọc trước
- `docs/decisions.md` #70, #71. `docs/tech/protocol.md`, mục **"Hồ sơ trên server (E6, #70)"**. Đây là hợp đồng:
  làm đúng tên tin và tên trường.
- `docs/reviews/2026-10-21-gemini-e6-inventory.md`, nhất là mục 2 (26 thao tác, có file:dòng), mục 3 (giờ máy) và mục 6
  (rủi ro).
- `docs/tech/code-style.md`: mỗi dòng một câu lệnh, tối đa 120 ký tự, không viết thân `if`/`for` cùng dòng.
- `Scripts/Net/**` (`NetworkService`, `OnlineModels`), `Scripts/Core/GameSession.cs`, `Scripts/UI/Login/LoginController.cs`,
  `Scripts/UI/Popups/AccountPopup.cs`, `Scripts/UI/Arena/**`.

## Việc cần làm
1. **NetworkService:**
   - một hàm chung gửi thao tác hồ sơ: `SendProfileAction(type, payload, onOk, onError)`;
   - `onOk` nhận payload, còn hồ sơ trong payload được áp vào `GameSession` trước khi gọi `onOk`;
   - các hàm có kiểu cho từng tin trong bảng của protocol;
   - `auth_ok` có hồ sơ thì áp vào session;
   - lưu độ lệch `serverTime` để có `ServerClock.UtcNow`.
2. **GameSession:**
   - giữ hồ sơ trong bộ nhớ, `ApplyServerProfile(json, rev)` dùng `PlayerProfile.FromJson` rồi phát
     `ProgressChanged`;
   - **bỏ đọc/ghi `profile.json`** và bỏ `SaveProfile` (sửa mọi chỗ gọi);
   - `HeroName`, `SectId`… chỉ đọc từ hồ sơ server;
   - token (PlayerPrefs) và `GameSettings` vẫn giữ trên máy.
   - Không xoá các lớp luật C# (`UpgradeService`, `ShopService`…) và test của chúng. UI vẫn dùng chúng để **xem
     trước** chi phí, nhưng không gọi `Apply`/`Buy` để đổi hồ sơ nữa.
3. **Đăng nhập khi vào game** (`LoginController`; **không sửa `LoginScene.unity`**, người dùng sửa tay):
   - `Start` kết nối server;
   - có token thì `resume`;
   - không có token hoặc `resume` hỏng thì mở `AccountPopup` bằng code, không cho đóng khi chưa đăng nhập;
   - đăng nhập xong thì ô tên hiện tên Hiệp Khách của tài khoản và không sửa được, rồi bật nút Vào Giang Hồ;
   - không kết nối được thì hiện thông báo kèm nút Thử lại. Không có chế độ offline.
   - Đang chơi mà mất kết nối: hiện popup "Mất kết nối" chặn thao tác, tự `resume` lại. Hỏng thì về Login.
4. **Chuyển mọi thao tác sang gửi server** (danh sách đủ ở mục 2 báo cáo Gemini):
   - Tàng Bảo Các: kéo thả, dồn kho, nâng Trọng, Hợp Thành, bán;
   - Bao Phục: lưu khi đóng. Chưa nhận `loadout_save_ok` thì chưa cho vào trận;
   - Thương Hội: làm mới, mua, đổi nguyên liệu, Tiệm Danh Vọng;
   - chọn phái, nhận thưởng nhiệm vụ, intro, hướng dẫn, cốt truyện, cờ nhắc chọn phái;
   - **PvE:** bấm ải thì gửi `pve_battle`, nhận kết quả, Gameplay phát lại `result`, rồi màn kết quả hiện `reward` từ
     server. Bỏ trừ Thể Lực, cộng thưởng và lên cấp ở client;
   - **Luận Kiếm:** `arena_find` gửi `{}`. Bỏ `ApplyRewardOnce`/`ApplyReward` ở client, vì hồ sơ đến kèm `arena_result`;
   - Thể Lực: Home/StageMap chỉ đếm ngược bằng `ServerClock`, không tự hồi rồi lưu;
   - `DevCheatPanel`: gửi `dev_cheat`. Server báo `forbidden` thì hiện thông báo.
   Trong lúc chờ trả lời: khoá nút hành động (chống bấm 2 lần). Lỗi `rejected` thì hiện `message` như câu lỗi hiện nay.
   Âm thanh và hiệu ứng thành công chỉ chạy khi nhận `*_ok`.
5. **Test EditMode** (transport giả như `OnlineNetTests`):
   - `auth_ok` có hồ sơ thì session có hồ sơ;
   - mỗi nhóm thao tác gửi đúng `type`/payload và áp hồ sơ trả về;
   - `rejected` không đổi hồ sơ;
   - không còn file `profile.json` nào được ghi;
   - luồng Login: có token thì `resume`, không có thì mở popup.
   Test cũ ghi/đọc `profile.json` (`ProfileStore`, migration) thì chuyển sang chỉ test `FromJson`, hoặc xoá nếu không
   còn ý nghĩa. Ghi rõ đã xoá test nào.

## Được phép sửa
- `One-Bag-Jianghu/Assets/_Game/Scripts/**`, `Tests/EditMode/**`, `Editor/Building/OnlinePrefabBuilder.cs` (nếu cần
  popup "Mất kết nối", dùng helper `Place`).
- **Cấm:** mọi lệnh git; sửa `server/**` (một task khác đang làm); chạy Unity; sửa scene (`*.unity`), prefab có sẵn
  ngoài builder, ảnh; đặt tên hàm trùng tên kiểu Unity (ví dụ `Image`); dùng `UnityEngine.Input` (dùng Input System);
  chạy coroutine trong `Setup` của popup (chạy trong `OnShown`).

## Tự kiểm tra
- Mọi file đã sửa phải qua `py tools/lint/check_style.py <file>`, chạy ở gốc repo.
- Claude sẽ build Unity, chạy test và chơi thử với server thật.

## Báo cáo
Viết ngắn gọn bằng tiếng Việt ở file `-o`:
- file đã sửa;
- thao tác nào đã chuyển;
- chỗ nào chưa chuyển được và vì sao;
- test đã thêm hoặc xoá.
