# Báo cáo Đánh giá Mã nguồn: Battle Engine (One Bag Jianghu)

**Ngày thực hiện:** 2026-09-27  
**Phạm vi đánh giá:** `server/battle-engine` (nguồn `main/java`, `test/java`, `resources/data/items.json`, đối chiếu với `docs/design/combat.md` và `docs/design/items.md`).  
**Mục tiêu:** Kiểm tra tính đúng đắn theo đặc tả, tính tất định (determinism), tính toàn vẹn của nhật ký sự kiện cho client replay, các lỗ hổng xác thực dữ liệu trong `ItemCatalog`, và độ bao phủ của bộ kiểm thử.

---

## 1. Lỗi tính đúng đắn (Correctness Bugs)

### [Mức độ: cao] `sourceMatches` trả về `true` khi `eventSource == null` khiến trigger `from: SELF/ADJACENT` bị kích hoạt sai
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:220`
- **Mô tả:** Trong hàm `sourceMatches`:
  ```java
  if (eventSource == null || from == TriggerSource.ANY) return true;
  ```
  Khi một sự kiện không gắn với vật phẩm cụ thể được phát ra (`eventSource == null`, ví dụ `BATTLE_START`, `HP_BELOW`, `PARRY_SUCCESS`), điều kiện này lập tức trả về `true`. Hệ quả là bất kỳ trigger nào được thiết lập với `from: SELF` hoặc `from: ADJACENT` đều bị kích hoạt dù không có vật phẩm nguồn tương ứng nào thỏa mãn.
  *Kịch bản:* Vật phẩm có trigger `{ "on": "hp_below", "from": "self", ... }`. Khi hiệp khách tụt máu, hàm `loseHp` gọi `fire(TriggerEvent.HP_BELOW, f, null, null, depth)` với `eventSource = null`. Hàm `sourceMatches` thấy `eventSource == null` liền trả về `true`, khiến trigger kích hoạt dù nó không phải là nguồn gây sát thương/sự kiện.
- **Đề xuất sửa:** Đổi logic kiểm tra thành:
  ```java
  if (from == TriggerSource.ANY) return true;
  if (eventSource == null) return false;
  ```

### [Mức độ: cao] Hàm `heal()` không kiểm tra `hp <= 0` cho phép đấu thủ đã tử trận hồi sinh trong cùng tick
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:378-383`
- **Mô tả:** Trong một tick tính toán (100ms), các hành động đã tích lũy đủ hồi chiêu đều được thực thi xen kẽ theo nguyên tắc đồng thời thật sự. Tuy nhiên, nếu Bên A ra đòn trước và hạ gục Bên B (`B.hp <= 0`), sau đó đòn đánh đã sẵn sàng của Bên B trong cùng tick kích hoạt và có hiệu ứng `HEAL`, hàm `heal` vẫn tính `actual = Math.min(amount, f.maxHp - f.hp)` và cộng máu trực tiếp vào `f.hp` đang âm của Bên B. Điều này đưa máu của Bên B trở lại dương trước khi bước kiểm tra tử trận `checkDeath()` chạy ở cuối tick, khiến Bên B hồi sinh từ cõi chết. Điều này mâu thuẫn với quy tắc thiết kế trong `fire`: *"A fighter already at 0 HP this tick cannot be saved by a pill"* và trong `secondTick`: `f.hp > 0`.
  *Kịch bản:* Bên B còn 40 HP. Bên A chém trước gây 100 sát thương (`B.hp = -60`). Cùng tick đó, B vung vũ khí có hiệu ứng `{ "op": "heal", "value": 150 }`. Hàm `heal` cộng 150 làm `B.hp = 90`. Đến cuối tick, `checkDeath()` thấy `B.hp > 0` nên Bên B không chết và trận đấu tiếp tục bình thường.
- **Đề xuất sửa:** Thêm kiểm tra ở đầu hàm `heal`: `if (f.hp <= 0) return;`.

### [Mức độ: trung bình] `REDUCE_MAX_HP` kéo tụt `hp` hiện tại nhưng không kích hoạt trigger `HP_BELOW`
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:267-268`
- **Mô tả:** Khi dính hiệu ứng Nội Thương (`REDUCE_MAX_HP`), máu tối đa giảm và máu hiện tại bị ép xuống theo `f.hp = Math.min(f.hp, f.maxHp)`. Tuy nhiên, mã nguồn không gọi `fire(TriggerEvent.HP_BELOW, ...)`. Theo `combat.md`, sự kiện `hp_below` xảy ra khi *"Khí huyết bên mình giảm và đang dưới ngưỡng hpBelow (còn sống)"*. Khi máu bị tụt do Nội Thương xuống dưới ngưỡng, các trang bị phản ứng như Bồi Nguyên Đan hoàn toàn không được kích hoạt.
  *Kịch bản:* Hiệp khách có 1000/1000 HP, mang Bồi Nguyên Đan (kích hoạt khi máu dưới 40% = 400). Địch gây Nội Thương giảm 700 max HP khiến `maxHp = 300` và `hp = 300` (< 40% ban đầu). Bồi Nguyên Đan không hề kích hoạt.
- **Đề xuất sửa:** Nếu `f.hp` bị giảm sau phép gán `Math.min`, kiểm tra và phát sự kiện `fire(TriggerEvent.HP_BELOW, f, null, null, ctx.depth)`.

### [Mức độ: trung bình] `setMeter` đặt về 0 trước khi phát `STATUS_APPLIED` làm sai lệch trạng thái tích điểm
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:317-321`
- **Mô tả:** Khi điểm trạng thái tích lũy chạm ngưỡng bộc phát (`meter >= threshold`), lệnh `setMeter(f, s, bursts ? 0 : meter)` đặt ngay meter về 0 trên thực thể `Fighter` trước khi gọi `fire(TriggerEvent.STATUS_APPLIED, ...)`. Nếu bất kỳ hiệu ứng nào lắng nghe `status_applied` truy vấn giá trị meter của nhân vật tại thời điểm đó, nó sẽ thấy 0 thay vì điểm số thực tế vừa đạt đến ngưỡng bộc phát (50).
  *Kịch bản:* Vũ khí tích 10 Liệt Hỏa đưa meter từ 40 lên 50. Code đặt meter = 0 ngay lập tức, sau đó mới gọi trigger `status_applied`. Trigger đọc `meter` thấy 0 điểm thay vì 50 điểm.
- **Đề xuất sửa:** Giữ nguyên meter ở giá trị chạm ngưỡng khi gọi `fire(TriggerEvent.STATUS_APPLIED)`, sau khi xử lý bộc phát (`burst`) xong mới thực hiện đặt `setMeter(f, s, 0)`.

---

## 2. Rủi ro tính tất định (Determinism Risks)

### [Mức độ: trung bình] Thứ tự thực thi `BATTLE_START` cố định Side 0 trước Side 1 thay vì tung coin ngẫu nhiên có seed
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:44-46`
- **Mô tả:** Trong vòng lặp mỗi tick, động cơ sử dụng `boolean aLeads = rng.nextInt(2) == 0;` để chọn ngẫu nhiên bên đi trước nhằm đảm bảo tính công bằng theo đúng `combat.md`: *"Thứ tự xen kẽ trong tick chỉ ảnh hưởng tài nguyên dùng chung (nội lực, Hóa Kình), và được chọn ngẫu nhiên có seed để không bên nào được ưu tiên cố định."* Tuy nhiên tại đầu trận (`BATTLE_START`), code lặp tuần tự `for (Fighter f : fighters)` khiến Side 0 luôn luôn được chạy trước Side 1. Nếu có các hiệu ứng tranh chấp tài nguyên chung đầu trận (như rút nội lực `DRAIN_QI` hoặc hiệu ứng bất lợi), Side 0 luôn có lợi thế vị trí tuyệt đối.
  *Kịch bản:* Cả 2 bên đều có trang bị kích hoạt lúc `BATTLE_START`: Bên A có hiệu ứng "hút 20 nội lực", Bên B có Thái Thanh Tâm Kinh ("nhận 20 nội lực"). Do Side 0 luôn chạy trước, Bên A hút từ Bên B khi B chưa kịp nhận nội lực (B có 0 nội lực, A không hút được gì). Nếu đổi vị trí loadout giữa A và B, Bên B chạy trước nhận 20 nội lực rồi Bên A mới hút được 20 nội lực. Kết quả trận đấu bị lệch theo thứ tự tham số thay vì seed.
- **Đề xuất sửa:** Sử dụng kết quả tung coin từ `rng` để quyết định thứ tự duyệt `fighters` khi kích hoạt `BATTLE_START`.

---

## 3. Vấn đề nhật ký sự kiện & Replay Client (Event-Log Problems)

### [Mức độ: cao] `BattleEvent` phát ra giá trị `hp` âm khi nhận sát thương vượt mức (overkill)
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:367`, `BattleEvent.java:66`
- **Mô tả:** Trong `loseHp`, phép tính `f.hp -= rest` làm cho `f.hp` có thể mang giá trị âm sâu (ví dụ -300). Hàm `BattleEvent.state(Fighter f)` sao chép trực tiếp `this.hp = f.hp`. Kết quả là client Unity nhận được các gói tin sự kiện với `hp` âm (ví dụ: `{"type": "DAMAGE", "hp": -250, ...}`). Trong khi đó, tài liệu đặc tả `combat.md` (dòng 122) minh họa sự kiện kết liễu có `"hp": 0`, và chính `BattleResult` cùng `LogFormatter` đã phải dùng `Math.max(0, ...)` để bọc lại giá trị này. Việc client nhận `hp` âm có thể gây lỗi hiển thị thanh máu UI hoặc làm sai lệch logic visual replay.
  *Kịch bản:* Hiệp khách còn 15 HP, dính một đòn bộc phát Liệt Hỏa 300 sát thương. Sự kiện `BURST` gửi cho client có `"hp": -285` thay vì `"hp": 0`.
- **Đề xuất sửa:** Trong `BattleEvent.state(Fighter f)`, gán `this.hp = Math.max(0, f.hp);` (hoặc clamp `f.hp = Math.max(0, f.hp);` khi nhân vật cạn máu).

### [Mức độ: thấp] Sự kiện `STATUS` khi tự giảm điểm mỗi giây (`decayPerSecond`) thiếu snapshot trạng thái `state(f)`
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/sim/Battle.java:138`
- **Mô tả:** Trong hàm `secondTick`, khi trạng thái tích điểm suy giảm theo thời gian, sự kiện được phát ra bằng `emit(ev(EventType.STATUS, f).kind(s.name()).value(next - meter).total(next));` mà không gọi `.state(f)`. Theo quy ước tại dòng 116 của `combat.md`, sự kiện tác động lên đấu thủ cần có snapshot trạng thái sau sự kiện (`hp, maxHp, shield, qi`) để client đồng bộ thanh trạng thái.
  *Kịch bản:* Nếu bật `decayPerSecond > 0` trong `BattleConfig`, client nhận được các sự kiện `STATUS` giảm điểm nhưng các trường `hp`, `maxHp`, `shield`, `qi` đều là `null`.
- **Đề xuất sửa:** Gọi thêm `.state(f)` trước khi đưa vào `emit(...)`.

---

## 4. Lỗ hổng kiểm tra dữ liệu trong ItemCatalog (Validation Gaps)

### [Mức độ: cao] Không kiểm tra `attack >= 0` khiến giá trị tấn công âm làm tăng máu và tăng khiên của đối thủ khi trúng đòn
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/ItemCatalog.java:104-106`
- **Mô tả:** `ItemCatalog.validate` kiểm tra `cooldown >= 0` và `crit in [0, 1]` nhưng hoàn toàn bỏ quên trường `attack`. Nếu một file JSON định nghĩa vũ khí có `attack < 0`, dữ liệu vẫn nạp thành công. Khi giao chiến, `base` và `amount` mang giá trị âm; trong `loseHp`, `f.shield -= Math.min(shield, amount)` sẽ trừ đi số âm khiến Cương Khí tăng lên, và `f.hp -= rest` khiến Khí Huyết tăng lên. Đòn tấn công biến thành đòn hồi phục cho đối thủ.
  *Kịch bản:* JSON cấu hình `"attack": -100`. Khi đánh trúng, đối thủ không mất máu mà được cộng thêm 100 máu và khiên.
- **Đề xuất sửa:** Thêm kiểm tra `if (def.attack < 0) errors.add("attack must be >= 0");` và đối với vũ khí bắt buộc `if (def.type == ItemType.WEAPON && def.attack <= 0) errors.add("weapon needs attack > 0");`.

### [Mức độ: cao] Cho phép vật phẩm không phải Binh Khí có `cooldown > 0` và kỹ năng `basic/ultimate`
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/ItemCatalog.java:106-116`
- **Mô tả:** Theo `items.md` dòng 85, chỉ có Binh Khí mới có thời gian hồi chiêu để ra đòn (`cooldown > 0`), các vật phẩm khác là bị động (`cooldown = 0`). Tuy nhiên, `ItemCatalog` chỉ kiểm tra chiều thuận: nếu là Binh Khí thì `cooldown > 0`. Mã nguồn không cấm Hộ Cụ, Tâm Pháp, Đan Dược hay Kỳ Trân có `cooldown > 0`. Nếu một món đồ bị động có `cooldown: 2.0` và khai báo `basic`, nó sẽ được đưa vào danh sách hồi chiêu và tự động tấn công mỗi 2 giây như một vũ khí thực thụ.
  *Kịch bản:* Một trang bị Hộ Cụ được viết nhầm `"type": "armor", "cooldown": 1.5, "basic": [{"op": "damage"}]`. `ItemCatalog` nạp bình thường và vào trận chiếc áo giáp này liên tục vung đòn gây sát thương lên địch.
- **Đề xuất sửa:** Thêm quy tắc xác thực: `if (def.type != ItemType.WEAPON && def.cooldown > 0) errors.add("non-weapon items must have cooldown = 0");`.

### [Mức độ: trung bình] `validateEffects` không kiểm tra giá trị tham số số học của các `EffectOp`
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/ItemCatalog.java:133-158`
- **Mô tả:** `validateEffects` chỉ kiểm tra tính hợp lệ của `op`, `status`, `duration/stacks` và `hits >= 1`. Tất cả các trường tham số số học khác đều bị bỏ lọt:
  - `DAMAGE`: không kiểm tra `pct > 0` hoặc `value > 0`.
  - `GAIN_QI`, `DRAIN_QI`, `SHIELD`, `HEAL`, `SHIELD_BREAK`, `REDUCE_MAX_HP`, `PARRY_DISCOUNT`, `ADD_ATTACK`: không kiểm tra `value > 0`.
  - `DMG_REDUCE`, `ADD_CRIT`: không kiểm tra `pct > 0` và `pct <= 1.0`.
  Nếu `HEAL` mang `value: -50`, hàm `heal()` sẽ thực hiện `f.hp += -50` trực tiếp trừ máu của nhân vật mà bỏ qua toàn bộ cơ chế đỡ đòn của Cương Khí hay Hộ Thân.
  *Kịch bản:* JSON khai báo `{ "op": "heal", "value": -100 }`. Khi kích hoạt, đối tượng bị trừ 100 máu trực tiếp và không kích hoạt trigger phản ứng.
- **Đề xuất sửa:** Bổ sung ràng buộc kiểm tra `value > 0` cho các hiệu ứng tăng/giảm chỉ số cố định và `pct > 0` cho các hiệu ứng tỉ lệ phần trăm.

### [Mức độ: trung bình] `validateEffects` chỉ kiểm tra `TargetKind` (`FIGHTER` vs `ITEM`) mà không kiểm tra phe (`SELF` vs `ENEMY`)
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/ItemCatalog.java:151-156`
- **Mô tả:** Kiểm tra `actual.kind != expected.kind` chỉ phân biệt mục tiêu tác động là Hiệp Khách (`FIGHTER`) hay Vật Phẩm (`ITEM`). Cả `SELF` và `ENEMY` đều thuộc `TargetKind.FIGHTER`. Do đó, nếu một vật phẩm gán `{ "op": "shield", "target": "enemy" }` hoặc `{ "op": "damage", "target": "self" }`, `ItemCatalog` hoàn toàn chấp nhận.
  *Kịch bản:* Vật phẩm có hiệu ứng khiên nhưng cấu hình nhầm `"target": "enemy"`. `ItemCatalog` không báo lỗi, khi kích hoạt nhân vật tự bơm khiên cho đối thủ.
- **Đề xuất sửa:** Xác thực rõ ràng danh sách `Target` được phép sử dụng cho từng `EffectOp` (ví dụ `SHIELD` chỉ được nhắm vào `SELF`, `DAMAGE` chỉ được nhắm vào `ENEMY`).

### [Mức độ: thấp] Trường `hits` được chấp nhận trên các hiệu ứng không hỗ trợ đa kích nhưng bị bỏ qua trong runtime
- **File:line:** `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/ItemCatalog.java:150`
- **Mô tả:** `validateEffects` kiểm tra `if (e.hits < 1) errors.add(ew + ": hits must be >= 1");` trên mọi hiệu ứng. Tuy nhiên, trong toàn bộ mã nguồn `Battle.java`, trường `hits` chỉ duy nhất được xử lý tại `EffectOp.DAMAGE`. Với tất cả các hiệu ứng khác (`HEAL`, `SHIELD`, `GAIN_QI`, `APPLY`), trường `hits` bị phớt lờ hoàn toàn mà không có cảnh báo.
  *Kịch bản:* Người thiết kế JSON viết `{ "op": "heal", "value": 50, "hits": 3 }` với kỳ vọng hồi 3 nhịp 50 máu (tổng 150), nhưng khi chạy nhân vật chỉ được hồi đúng 1 lần 50 máu.
- **Đề xuất sửa:** Bổ sung kiểm tra trong `ItemCatalog`: nếu `e.hits > 1` và `e.op != EffectOp.DAMAGE` thì báo lỗi cấu hình.

---

## 5. Các quy tắc quan trọng chưa có Unit Test (Important Untested Rules)

### [Mức độ: trung bình] Thiếu kiểm thử cho hàng loạt cơ chế chiến đấu và hiệu ứng kỹ năng cốt lõi
- **File:line:** `server/battle-engine/src/test/java/com/onebagjianghu/battle/sim/BattleSimulatorTest.java`
- **Mô tả:** File test hiện tại kiểm tra tính tất định và một số kịch bản cơ bản, nhưng bỏ sót rất nhiều cơ chế chiến đấu then chốt đã được chỉ định trong `combat.md`:
  1. `SHIELD_BREAK` (Phá Cương): Chưa có test nào kiểm tra việc trừ thẳng Cương Khí trước khi tính sát thương vũ khí.
  2. `REDUCE_MAX_HP` (Nội Thương): Chưa có test kiểm tra việc cắt giảm `maxHp` và kéo tụt `hp` theo.
  3. `REGEN` (Điều Tức): Chưa có test kiểm tra việc hồi máu định kỳ mỗi giây dựa trên số tầng Điều Tức.
  4. `SLOW` (Trì Trệ): Test `hasteDoublesAndSealPausesCooldown` chỉ kiểm tra Haste và Seal, bỏ qua cơ chế Slow (giảm 50% tốc độ xuất chiêu).
  5. `CHARGE` (Vận Khí): Chưa có test kiểm tra việc đẩy thanh tiến độ hồi chiêu và kích hoạt đòn đánh ở tick kế tiếp.
  6. `ADD_ATTACK` / `ADD_CRIT`: Chưa có test kiểm tra các bùa chú tăng Lực Đạo và tỉ lệ Hội Tâm lên vũ khí.
  7. Multi-hit (`hits > 1`): Chưa có test cho vũ khí đánh nhiều nhát trong một lượt (như Thanh Phong Kiếm: 3 nhát × 90%).
  8. Giới hạn hằng số biên:
     - Chưa kiểm tra sàn chi phí Hóa Kình `minParryCost = 20` (khi `parry_discount` vượt quá 40).
     - Chưa kiểm tra trần Hộ Thân `maxDmgReducePermille = 800` (80%).
  9. Các sự kiện Trigger còn thiếu: Chưa có test cho `ULTIMATE`, `PARRIED`, `STATUS_APPLIED`, `BURST`.
  10. Hệ số Trọng (`tierMultiplierPermille`): Chưa có test kiểm tra việc nhân hệ số Trọng vào `value`/`stacks` của các hiệu ứng kỹ năng (khiên, hồi máu, điểm trạng thái) ngoài thuộc tính `attack`.
  *Kịch bản:* Nếu một lập trình viên vô tình làm gãy logic của `SHIELD_BREAK`, `REDUCE_MAX_HP`, `REGEN` hay `CHARGE`, toàn bộ test suite hiện tại vẫn báo xanh 100%.
- **Đề xuất sửa:** Bổ sung các test case độc lập tương ứng trong `BattleSimulatorTest` cho từng cơ chế nói trên nhằm khóa chặt các quy tắc nghiệp vụ theo đặc tả `combat.md`.

---

## Đánh giá tổng thể (Overall Assessment)

Mô-đun `battle-engine` được thiết kế mạch lạc, cấu trúc mã nguồn gọn gàng, sử dụng thuật toán sinh số ngẫu nhiên SplitMix64 tốt và đảm bảo mô phỏng hoàn toàn trên số nguyên theo tick 100ms. Tuy nhiên, hệ thống vẫn tồn tại hai lỗi logic nghiêm trọng cần khắc phục ngay: lỗi lọt nguồn trong `sourceMatches` khiến các trigger có điều kiện `from: SELF/ADJACENT` tự động kích hoạt ngoài ý muốn trên các sự kiện không có nguồn, và lỗ hổng trong `heal()` cho phép đấu thủ đã chết được hồi sinh nếu tung chiêu hồi máu trong cùng tick. Bên cạnh đó, việc `BattleEvent` phát ra giá trị máu âm khi kết liễu sẽ gây trục trặc cho client Unity khi phát lại trận đấu, và bộ tiền kiểm tra `ItemCatalog` còn nhiều khoảng trống chưa chặn các giá trị số âm/sai loại mục tiêu. Khắc phục các điểm này và bổ sung các ca kiểm thử cho những cơ chế còn thiếu sẽ giúp động cơ chiến đấu đạt độ vững chắc cao, sẵn sàng phục vụ cho việc tích hợp máy chủ và kiểm thử cân bằng game.

---

## Kiểm chứng của Claude (2026-09-27)

Mỗi mục đã được đối chiếu với code thật. **7 đúng (đã sửa), 6 sai hoặc là thiết kế có chủ ý (không sửa).**

| # | Mục | Kết luận | Xử lý |
|---|---|---|---|
| 1.1 | `sourceMatches` với `eventSource == null` | **Một phần.** Không phải lỗi runtime: `from` vốn chỉ có nghĩa với sự kiện do một món gây ra (combat.md). Nhưng dữ liệu viết `from: self` cho `hp_below` sẽ bị lặng lẽ bỏ qua. Cách sửa Gemini đề xuất (trả `false`) lại làm trigger đó *không bao giờ* chạy | Chặn ở `ItemCatalog`: `from` ≠ `any` chỉ hợp lệ với `hit, crit, ultimate, parried, status_applied, burst` |
| 1.2 | `heal()` hồi sinh bên đã về 0 trong cùng tick | **Đúng** (mức trung bình). Kết quả còn phụ thuộc đồng xu chọn thứ tự vì trần máu tối đa | `heal()` bỏ qua bên có hp ≤ 0, nhất quán với Đan Dược và Điều Tức |
| 1.3 | `REDUCE_MAX_HP` không bắn `hp_below` | **Sai.** Sau khi cắt, hp = maxHp mới → tỉ lệ 100%, không thể dưới ngưỡng. Kịch bản của Gemini (300/300 < 40%) tính sai | Không sửa |
| 1.4 | Meter về 0 trước `status_applied` | **Sai / lý thuyết.** Không có hiệu ứng nào đọc meter; sự kiện `STATUS` đã ghi `total` = 50 trước khi reset | Không sửa |
| 2.1 | `BATTLE_START` luôn chạy bên 0 trước | **Đúng** (thấp). Hiện chưa món nào tranh tài nguyên lúc đầu trận nhưng sẽ có | Tung đồng xu có seed như trong tick |
| 3.1 | Nhật ký có `hp` âm | **Đúng** (thấp, không phải cao) | `BattleEvent.state` kẹp hp ≥ 0. File mẫu Unity cũ vẫn có -32, test Unity vẫn hợp lệ |
| 3.2 | Sự kiện giảm điểm thiếu snapshot | **Đúng** (thấp, chức năng đang tắt) | Thêm `.state(f)` |
| 4.1 | Không kiểm tra `attack` âm | **Đúng** | `attack ≥ 0`, binh khí `attack > 0` |
| 4.2 | Vật phẩm không phải binh khí có `cooldown > 0` | **Sai: có chủ ý.** Kỳ Trân / Pháp Bảo chủ động (như Búa Thiên Công trong game tham chiếu) cần thanh hồi chiêu | Không sửa |
| 4.3 | Không kiểm tra giá trị số của hiệu ứng | **Đúng** | `value > 0` cho qi/khiên/hồi máu/phá cương/nội thương/parry_discount/add_attack; `damage` cần `pct > 0` hoặc `value > 0`; `dmg_reduce`/`add_crit` cần `pct` trong (0, 1] |
| 4.4 | Không kiểm tra phe `self`/`enemy` | **Không sửa: có chủ ý linh hoạt.** Ví dụ hiệu ứng tự gây sát thương cho bản thân để đổi lấy sức mạnh là một kiểu thiết kế hợp lệ | Không sửa |
| 4.5 | `hits` bị bỏ qua ở hiệu ứng khác `damage` | **Đúng** (thấp) | Báo lỗi nếu `hits > 1` mà không phải `damage` |
| 5 | Thiếu test | **Đúng** | Thêm 8 test: Phá Cương, đa nhát, Điều Tức, Trì Trệ, Vận Khí, sàn chi phí Hóa Kình, trần Hộ Thân, không hồi máu khi đã về 0 + nhật ký không có hp âm; 2 test kiểm tra dữ liệu |

Sau khi sửa: 36/36 test pass; bảng đối đầu vẫn trong khoảng 40–61%.
