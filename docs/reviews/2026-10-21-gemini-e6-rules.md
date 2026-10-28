# Chuẩn Đối Chiếu Luật Nghiệp Vụ Hồ Sơ Người Chơi (C# Ground Truth cho Server Java - E6)

- **Ngày lập:** 2026-10-21
- **Tác giả:** Gemini (Kiến trúc sư hệ thống One Bag Jianghu)
- **Mục đích:** Trích xuất chi tiết, nguyên văn và chính xác tuyệt đối toàn bộ luật chơi từ mã nguồn C# và dữ liệu JSON làm chuẩn đặc tả duy nhất để đối chiếu, cài đặt và kiểm thử (review) đợt 2 cho server Java theo Quyết định [#70](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md) và [#71](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md).
- **Nguyên tắc:** Dẫn chứng chính xác `File:Dòng` cho mọi điều kiện từ chối, câu thông báo lỗi, phép tính, thứ tự biến đổi hồ sơ và ca kiểm thử.

---

## Mục Lục
1. [Nâng Trọng](#1-nâng-trọng)
2. [Hợp Thành](#2-hợp-thành)
3. [Bán](#3-bán)
4. [Xếp Tàng Bảo Các](#4-xếp-tàng-bảo-các)
5. [Xếp Bao Phục](#5-xếp-bao-phục)
6. [Thương Hội](#6-thương-hội)
7. [Thể Lực](#7-thể-lực)
8. [Trận PvE](#8-trận-pve)
9. [EXP và Lên Cấp](#9-exp-và-lên-cấp)
10. [Nhiệm Vụ](#10-nhiệm-vụ)
11. [Chọn Phái](#11-chọn-phái)
12. [Hướng Dẫn, Intro, Cốt Truyện](#12-hướng-dẫn-intro-cốt-truyện)
13. [Chiến Uy (PowerCatalog)](#13-chiến-uy-powercatalog)
14. [Bộ Khởi Đầu](#14-bộ-khởi-đầu)
15. [Bảng Cheat](#15-bảng-cheat)
16. [Chỗ Dễ Chép Sai Sang Java](#16-chỗ-dễ-chép-sai-sang-java)
17. [Chỗ Luật C# Có Vẻ Là Lỗi](#17-chỗ-luật-c-có-vẻ-là-lỗi)

---

## 1. Nâng Trọng

- **Tập tin quy chiếu:**
  - Logic: [`UpgradeService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L1-L125)
  - Cấu trúc & Catalog: [`UpgradeCatalog.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeCatalog.cs#L8-L60)
  - Dữ liệu chi phí: [`Resources/Data/upgrade_costs.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/upgrade_costs.json#L1-L7)
  - Dữ liệu nguyên liệu: [`Resources/Data/materials.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/materials.json#L1-L19)
  - Test chuẩn: [`EconomyServicesTests.cs:98-112, 158-175`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L98)

### 1.1. Điều kiện từ chối và câu lỗi nguyên văn
1. Không tìm thấy vật phẩm theo `uid`:
   - Kiểm tra: `profile.FindItem(uid) == null`
   - Câu lỗi: `"Không tìm thấy vật phẩm."` ([`UpgradeService.cs:47`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L47))
2. Vật phẩm đã đạt cấp Trọng tối đa (Cửu Trọng):
   - Kiểm tra: `item.Tier >= 9`
   - Câu lỗi: `"Vật phẩm đã đạt Cửu Trọng."` ([`UpgradeService.cs:52`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L52))
3. Không đủ Bạc:
   - Kiểm tra: `silver < preview.Silver` (với `silver` lấy từ `profile.Currencies["bac"]`)
   - Câu lỗi: `$"Không đủ Bạc ({silver}/{preview.Silver})."` ([`UpgradeService.cs:97`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L97))
4. Không đủ nguyên liệu hoặc chất dẫn:
   - Kiểm tra: `owned < cost.Value` (với `owned` lấy từ `profile.Materials[cost.Key]`)
   - Câu lỗi: `$"Không đủ {materials.Get(cost.Key).Name} ({owned}/{cost.Value})."` ([`UpgradeService.cs:106`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L106))
   - *Lưu ý:* Phải lấy tên tiếng Việt từ `materials.json` (ví dụ: `Hàn Thiết`, `Binh Hồn Tinh Thạch`), không dùng id thô.

### 1.2. Công thức từng bước
1. **Xác định nhóm nguyên liệu:**
   - Hàm `UpgradeService.Group(string itemType)`: nếu `itemType == "pill" || itemType == "relic"` thì trả về `"pill_relic"`, ngược lại giữ nguyên `itemType` (`"weapon"`, `"armor"`, `"manual"`). ([`UpgradeService.cs:123`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L123))
2. **Tra cứu bước nâng (`UpgradeStep`):**
   - Đọc từ `upgrade_costs.json` theo `rarity` của item và `fromTier = item.Tier` (1–8).
   - `Silver`: kiểu `long`.
   - `MaterialTier`: kiểu `int` (1–3).
   - `MaterialCount`: kiểu `int`.
   - `CatalystCount`: kiểu `int` (0 nếu không cần chất dẫn).
3. **Xác định ID nguyên liệu:**
   - Nguyên liệu thường: duyệt `materials.json` tìm item thỏa `ItemType == group && Tier == step.MaterialTier && IsCatalyst == false`. ([`UpgradeService.cs:118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L118))
   - Chất dẫn: nếu `step.CatalystCount > 0`, tìm item thỏa `ItemType == group && Tier == 0 && IsCatalyst == true`. ([`UpgradeService.cs:118`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L118))
4. **Kiểm tra Đột Phá:**
   - `NextTier = item.Tier + 1`.
   - Kiểm tra `items.GetUnlockTiers(item.ItemId)`: nếu danh sách unlock tiers chứa `NextTier` thì `UnlocksBreakthrough = true` (thường là Trọng 3, 6, 9 đối với vũ khí có Đột Phá). ([`UpgradeService.cs:66-70`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L66-L70))

### 1.3. Trường hồ sơ bị thay đổi (thứ tự chính xác)
1. `profile.Currencies["bac"] -= preview.Silver;` ([`UpgradeService.cs:85`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L85))
2. Trừ nguyên liệu kho: gọi `new StorageGrid(profile, catalog).SpendMaterials(preview.Materials, out _)`. ([`UpgradeService.cs:86`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L86))
   - Duyệt ngược danh sách `profile.Storage.MaterialStacks` từ cuối về đầu (`i = count - 1` xuống `0`), trừ dần `Count`. Nếu stack về 0 thì xóa stack khỏi list. ([`StorageGrid.cs:238-248`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L238-L248))
   - Đồng bộ lại `profile.Materials` từ các stack còn lại. ([`StorageGrid.cs:306-314`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L306-L314))
3. Tăng Trọng vật phẩm: `profile.RequireItem(uid).Tier++;` ([`UpgradeService.cs:87`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L87))
4. **Ghi nhận nhiệm vụ:** Có. Gọi `profile.RecordQuestAction("item_upgrade")` -> tăng `profile.Quest.ActionCounters["item_upgrade"]` thêm 1. ([`UpgradeService.cs:88`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeService.cs#L88), [`PlayerProfile.cs:178-183`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L178-L183))

### 1.4. Ngẫu nhiên
- **Không có.** Nâng Trọng thành công 100%, chi phí xác định hoàn toàn.

### 1.5. Ca biên
- **Vật phẩm đang trang bị trong Bao Phục (`Loadout`):** Nâng Trọng bình thường. Vật phẩm giữ nguyên vị trí trong `profile.Loadout`, `Tier` tăng lên 1 ([`EconomyServicesTests.cs:158-175`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L158-L175)).
- **Tiền/Nguyên liệu vừa đủ:** Được phép (điều kiện từ chối là `<`).
- **Trọng đã là 9:** Từ chối ngay lập tức với câu lỗi `"Vật phẩm đã đạt Cửu Trọng."`.

### 1.6. Ca thử
- **Ca 1 (Từ test `UpgradeIsAtomicAndStopsAtNine`):**
  - Đầu vào: Kiếm phẩm Phàm (`common`), Trọng 2. Hồ sơ có 100.000 Bạc, 1.000 nguyên liệu các loại.
  - Kết quả mong đợi: `NextTier = 3`, `UnlocksBreakthrough = true`. Nâng thành công, Bạc và nguyên liệu bị trừ, `item.Tier = 3`. Nếu gán `item.Tier = 9`, gọi Apply tiếp theo thất bại, giữ nguyên `item.Tier = 9`.
- **Ca 2 (Tự tính tay theo `upgrade_costs.json`):**
  - Đầu vào: Thanh Phong Kiếm (`thanh_phong_kiem`), phẩm Phàm (`common`), Trọng 1. Bạc sở hữu = 100, Hàn Thiết (`han_thiet`) sở hữu = 3.
  - Tra cứu: `fromTier: 1`: `silver: 100`, `materialTier: 1`, `materialCount: 3`, `catalystCount: 0`.
  - Kết quả mong đợi: Thành công. Bạc còn 0, Hàn Thiết còn 0, `item.Tier` lên 2, `ActionCounters["item_upgrade"]` tăng 1.

---

## 2. Hợp Thành

- **Tập tin quy chiếu:**
  - Logic: [`MergeService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L1-L239)
  - Catalog: [`UpgradeCatalog.cs:74-105`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/UpgradeCatalog.cs#L74-L105)
  - Dữ liệu quy tắc: [`Resources/Data/merge.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/merge.json#L1)
  - Test chuẩn: [`EconomyServicesTests.cs:114-156, 177-331`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L114)

### 2.1. Điều kiện từ chối và câu lỗi nguyên văn
1. Không tìm thấy 1 trong 2 món:
   - Kiểm tra: `first == null || second == null`
   - Câu lỗi: `"Không tìm thấy bản sao để Hợp Thành."` ([`MergeService.cs:183`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L183))
2. Chọn trùng 1 món làm cả 2 đầu vào:
   - Kiểm tra: `a == b`
   - Câu lỗi: `"Phải chọn hai bản sao khác nhau."` ([`MergeService.cs:185`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L185))
3. Hai món khác loại (`ItemId`):
   - Kiểm tra: `first.ItemId != second.ItemId`
   - Câu lỗi: `"Hai bản sao phải là cùng một món."` ([`MergeService.cs:187`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L187))
4. Hai món khác phẩm chất (`Rarity`):
   - Kiểm tra: `first.Rarity != second.Rarity`
   - Câu lỗi: `"Hai bản sao phải cùng phẩm."` ([`MergeService.cs:189`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L189))
5. Món đã ở phẩm cao nhất (Chí Tôn - `supreme`):
   - Kiểm tra: `first.Rarity == "supreme"`
   - Câu lỗi: `"Phẩm Chí Tôn không thể Hợp Thành cao hơn."` ([`MergeService.cs:191`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L191))
6. Không đủ Bạc:
   - Kiểm tra: `silver < result.Silver`
   - Câu lỗi: `$"Không đủ Bạc ({silver}/{result.Silver})."` ([`MergeService.cs:92`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L92))
7. Không đủ phụ liệu đặc biệt (ví dụ Chí Tôn Ngọc Tủy):
   - Kiểm tra: `owned < extra.Value`
   - Câu lỗi: `$"Không đủ {materials.Get(extra.Key).Name} ({owned}/{extra.Value})."` ([`MergeService.cs:102`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L102))
8. Kho Tàng Bảo Các không đủ chỗ (chỉ xảy ra khi cả 2 món đều nằm trong kho):
   - Kiểm tra: `!storage.TryFindFirst(width, height, out _, out _)` sau khi đã gỡ tạm 2 món cũ ([`MergeService.cs:160`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L160))
   - Câu lỗi: `"Tàng Bảo Các không đủ chỗ."` ([`MergeService.cs:163`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L163))

### 2.2. Công thức từng bước
1. **Quy tắc Trọng đầu ra (#67):**
   - Món thứ nhất (`firstUid`) là **món chính** và giữ nguyên Trọng của nó: `outputTier = first.Tier;` ([`MergeService.cs:65`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L65)). Mọi cấp Trọng đều được phép Hợp Thành (kể cả Nhất Trọng).
2. **Quy tắc phẩm đầu ra & phí Bạc:**
   - Tra bước trong `merge.json` theo `first.Rarity`:
     - `common` -> `uncommon`: 500 Bạc, phụ liệu `{}`
     - `uncommon` -> `rare`: 1.500 Bạc, phụ liệu `{}`
     - `rare` -> `epic`: 4.000 Bạc, phụ liệu `{}`
     - `epic` -> `legendary`: 10.000 Bạc, phụ liệu `{}`
     - `legendary` -> `supreme`: 25.000 Bạc, phụ liệu `{"chi_ton_ngoc_tuy": 5}`
3. **Hoàn trả nguyên liệu từ bản sao thứ 2 (`second`):**
   - Tính tổng toàn bộ nguyên liệu và chất dẫn đã tiêu tốn để nâng món `second` từ Trọng 1 lên `second.Tier`:
     - Duyệt `current = 1` đến `second.Tier - 1`, cộng dồn `step.MaterialCount` và `step.CatalystCount` từ `upgrade_costs.json`. ([`MergeService.cs:213-219`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L213-L219))
   - Hoàn trả 50%: `int refund = value.Value / 2;` (chia nguyên `int / 2`, cắt bỏ phần thập phân). ([`MergeService.cs:83`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L83))
   - Nếu `refund > 0`, đưa vào `RefundMaterials`.
4. **Cảnh báo suy giảm sức mạnh (`WeakerThanBestInput`):**
   - `Strength(rarity, tier) = RarityRules.PowerPermille(rarity, tier) = TierMultiplierPermille(tier) * MultiplierPermille(rarity) / 1000`. ([`RarityRules.cs:33`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/RarityRules.cs#L33))
   - Cờ bật `true` nếu `Strength(step.ToRarity, outputTier) < Math.Max(Strength(first.Rarity, first.Tier), Strength(second.Rarity, second.Tier))`. ([`MergeService.cs:72-76`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L72-L76))

### 2.3. Trường hồ sơ bị thay đổi (thứ tự chính xác)
- **Trường hợp A: Có ít nhất 1 món đang trang bị trên Bao Phục (`Loadout`):**
  1. `equippedResult = IsEquipped(firstUid) ? first : second;`
  2. `consumed = ReferenceEquals(equippedResult, first) ? second : first;`
  3. Gỡ cả hai khỏi vị trí kho (nếu có): `storage.RemoveItem(firstUid); storage.RemoveItem(secondUid);`
  4. Xóa món bị tiêu hao: `profile.RemoveItem(consumed.Uid);` (xóa khỏi `Items`, `Loadout`, `Storage.Items`)
  5. Cập nhật món giữ lại: `equippedResult.Rarity = preview.OutputRarity; equippedResult.Tier = preview.OutputTier;`
  6. Trừ Bạc & phụ liệu, cộng hoàn trả:
     - `profile.Currencies["bac"] -= preview.Silver;`
     - `storage.SpendMaterials(preview.ExtraMaterials, out _);`
     - Duyệt `refund`: `storage.TryAddMaterial(refund.Key, refund.Value);`
  7. `profile.RecordQuestAction("item_merge");`
- **Trường hợp B: Cả 2 món đều nằm trong kho (`Storage`):**
  1. Gỡ 2 món khỏi kho: `storage.RemoveItem(firstUid); storage.RemoveItem(secondUid);`
  2. Trừ Bạc & phụ liệu, cộng hoàn trả:
     - `profile.Currencies["bac"] -= preview.Silver;`
     - `storage.SpendMaterials(preview.ExtraMaterials, out _);`
     - Duyệt `refund`: `storage.TryAddMaterial(refund.Key, refund.Value);`
  3. Xóa vĩnh viễn cả 2 món cũ: `profile.RemoveItem(firstUid); profile.RemoveItem(secondUid);`
  4. Tạo vật phẩm mới: `OwnedItem newItem = profile.AddItem(itemId, preview.OutputTier, rarity: preview.OutputRarity);` (tạo `uid` ngẫu nhiên mới!).
  5. Đặt món mới vào kho: `storage.TryPlaceItem(newItem);`
  6. **Ghi nhận nhiệm vụ:** Có. Gọi `profile.RecordQuestAction("item_merge")` -> tăng `ActionCounters["item_merge"]` thêm 1. ([`MergeService.cs:170`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L170))

### 2.4. Ngẫu nhiên
- **Không có.** (Trừ việc sinh chuỗi `Guid.NewGuid().ToString("N")` cho `uid` của món mới khi hợp thành từ 2 món trong kho).

### 2.5. Ca biên
- **Cả 2 món cùng đang đeo trên Bao Phục:** Món `first` được giữ lại đúng vị trí ô cũ, món `second` bị gỡ khỏi Bao Phục và xóa. Ô của `second` trên Bao Phục trở thành ô trống ([`EconomyServicesTests.cs:246-267`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L246-L267)).
- **Món chính đang đeo trên Bao Phục và kho Tàng Bảo Các đã đầy 100%:** Vẫn Hợp Thành thành công vì món kết quả nằm trên Bao Phục, không cần ô trống kho ([`EconomyServicesTests.cs:270-292`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L270-L292)).
- **Món thứ 2 là Nhất Trọng (`Tier = 1`):** Hoàn trả nguyên liệu rỗng (`RefundMaterials` trống) vì chưa từng nâng Trọng.
- **Hợp Thành Thần Binh -> Chí Tôn:** Bắt buộc phải có đủ 5 `chi_ton_ngoc_tuy` trong kho ([`merge.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/merge.json#L1), [`EconomyServicesTests.cs:316-331`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L316-L331)).

### 2.6. Ca thử
- **Ca 1 (Từ test `MergeKeepsMainTierAndRefundsHalfOfMaterialCopy`):**
  - Đầu vào: Kiếm A Trọng 8, Kiếm B Trọng 3 (cùng phẩm Phàm).
  - Kết quả mong đợi: `OutputTier = 8`. `WeakerThanBestInput = false`. Hoàn trả nguyên liệu bằng đúng 50% chi phí nâng từ Trọng 1 lên Trọng 3 của Kiếm B. Nếu đảo lại B làm món chính, A làm phụ: `OutputTier = 3`, `WeakerThanBestInput = true`.
- **Ca 2 (Từ test `LegendaryMergeRequiresFivePvpJadeAndRefundNeverNegative`):**
  - Đầu vào: 2 kiếm phẩm Thần Binh (`legendary`), Trọng 3. Hồ sơ có 4 `chi_ton_ngoc_tuy`.
  - Kết quả: Bị từ chối vì thiếu 1 Ngọc Tủy. Thêm viên thứ 5: Thành công, trừ 25.000 Bạc và 5 Ngọc Tủy, ra kiếm Chí Tôn (`supreme`) Trọng 3.

---

## 3. Bán

- **Tập tin quy chiếu:**
  - Logic: [`SellService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L1-L94)
  - Test chuẩn: [`EconomyServicesTests.cs:333-345`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L333-L345)

### 3.1. Điều kiện từ chối và câu lỗi nguyên văn
1. Không tìm thấy vật phẩm theo `uid`:
   - Kiểm tra: `profile.FindItem(uid) == null`
   - Câu lỗi: `"Không tìm thấy vật phẩm để bán."` ([`SellService.cs:46`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L46))

### 3.2. Công thức từng bước
1. **Giá gốc theo phẩm (`BasePrices`):**
   - Mảng tra cứu ([`SellService.cs:21-29`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L21-L29)):
     - `common` (Phàm): 200
     - `uncommon` (Ưu): 500
     - `rare` (Hiếm): 1.200
     - `epic` (Cực): 3.000
     - `legendary` (Thần Binh): 8.000
     - `supreme` (Chí Tôn): 15.000
2. **Tính Bạc và nguyên liệu đã nâng Trọng (`investedSilver`, `Materials`):**
   - Duyệt `tier = 1` đến `item.Tier - 1`:
     - Lấy `step = upgrades.Get(item.Rarity, tier);`
     - `investedSilver += step.Silver;`
     - Nguyên liệu thường hoàn lại: `step.MaterialCount / 2` (chia nguyên cắt phần thập phân). ([`SellService.cs:58`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L58))
     - Chất dẫn hoàn lại: `step.CatalystCount / 2` (chia nguyên cắt phần thập phân). ([`SellService.cs:59`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L59))
3. **Tính tổng Bạc nhận được:**
   - Phép tính số học số nguyên kiểu `long` ([`SellService.cs:62`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L62)):
     ```csharp
     result.Silver = BasePrices[RarityRules.Index(item.Rarity)] * 30 / 100 + investedSilver * 15 / 100;
     ```
     *(30% giá gốc của phẩm + 15% tổng Bạc đã đổ vào nâng Trọng; cả hai phép nhân rồi chia nguyên 100 cắt phần dư).*

### 3.3. Trường hồ sơ bị thay đổi (thứ tự chính xác)
1. `storage.RemoveItem(uid);` ([`SellService.cs:72`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L72))
2. `profile.RemoveItem(uid);` ([`SellService.cs:73`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L73)) -> xóa khỏi `Items`, xóa khỏi `Loadout` (nếu đang trang bị), xóa khỏi `Storage.Items`.
3. `profile.AddCurrency("bac", preview.Silver);` ([`SellService.cs:74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L74))
4. Duyệt `preview.Materials`: gọi `storage.TryAddMaterial(value.Key, value.Value);` ([`SellService.cs:76`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L76))
5. **Ghi nhận nhiệm vụ:** Không ghi nhận nhiệm vụ nào.

### 3.4. Ngẫu nhiên
- **Không có.**

### 3.5. Ca biên
- **Vật phẩm đang đeo trên Bao Phục (`Loadout`):** C# cho phép bán! Khi bán, món đồ bị xóa sạch khỏi cả `Items` lẫn `Loadout`, ô trên Bao Phục được giải phóng ([`EconomyServicesTests.cs:334-345`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/EconomyServicesTests.cs#L334-L345)).
- **Món đồ Nhất Trọng (`Tier = 1`):** `investedSilver = 0`, không hoàn lại nguyên liệu. Bạc nhận được đúng bằng `BasePrices * 30 / 100`.

### 3.6. Ca thử
- **Ca 1 (Từ test `SellRemovesEquippedCopy`):**
  - Đầu vào: Kiếm phẩm Phàm (`common`), Trọng 3, đang gắn trong `Loadout`.
  - Kết quả: Bán thành công, `profile.FindItem("a") == null`, `profile.Loadout` rỗng, Bạc và nguyên liệu hoàn trả được cộng.
- **Ca 2 (Tự tính tay):**
  - Đầu vào: Kiếm phẩm Phàm (`common`), Trọng 2.
  - Phí nâng 1->2: 100 Bạc, 3 Hàn Thiết, 0 Chất dẫn.
  - Giá Bạc nhận: `200 * 30 / 100 + 100 * 15 / 100 = 60 + 15 = 75` Bạc.
  - Nguyên liệu hoàn trả: Hàn Thiết = `3 / 2 = 1`.
  - Kết quả mong đợi: `Silver = 75`, `Materials = {"han_thiet": 1}`.

---

## 4. Xếp Tàng Bảo Các

- **Tập tin quy chiếu:**
  - Logic lưới & stack: [`StorageGrid.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L1-L359)
  - Logic dọn kho (Repack): [`StoragePopup.cs:397-452`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L397-L452)
  - Test chuẩn: [`StorageGridTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/StorageGridTests.cs#L1-L60), [`MaterialStackTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/MaterialStackTests.cs#L1-L104)

### 4.1. Thông số lưới Tàng Bảo Các
- `Cols = 10`, `Rows = 20` (tổng cộng 200 ô). ([`StorageGrid.cs:38-40`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L38-L40))
- Giới hạn stack nguyên liệu: `MaxStack = 999`. Mỗi stack nguyên liệu chiếm đúng **1 ô 1x1**. ([`StorageGrid.cs:52`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L52))

### 4.2. Các thao tác và điều kiện từ chối

#### 4.2.1. Di chuyển vật phẩm (`TryMoveItem(uid, x, y)`)
- Điều kiện từ chối:
  - `placement == null || item == null` -> `false`
  - Vị trí `(x, y)` vượt quá biên lưới (`x < 0 || x + width > Cols || y < 0 || y + height > Rows`) -> `false`
  - Trùng đè lên bất kỳ vật phẩm nào khác trong `Storage.Items` hoặc đè lên ô của bất kỳ `MaterialStack` nào -> `false`
- Khi thành công: gán `placement.X = x; placement.Y = y;`.

#### 4.2.2. Di chuyển stack nguyên liệu (`TryMoveStack(stack, x, y)`)
- Điều kiện từ chối: `stack == null` hoặc không nằm trong `Storage.MaterialStacks`, hoặc ô `(x, y)` bị ra ngoài biên hoặc bị chiếm bởi đồ/stack khác -> `false`.
- Khi thành công: gán `stack.X = x; stack.Y = y;`.

#### 4.2.3. Thêm nguyên liệu (`TryAddMaterial(id, amount)`)
- Điều kiện từ chối: `amount <= 0` hoặc kho không đủ ô trống để tạo các stack mới -> `false`.
- Công thức tính số ô cần:
  - Tính tổng sức chứa còn lại trong các stack hiện có của loại `id`: `freeInStacks = sum(999 - stack.Count)`.
  - Số ô 1x1 mới cần thêm: `cells = Math.Max(0, amount - freeInStacks + 999 - 1) / 999;` (chia làm tròn lên). ([`StorageGrid.cs:185`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StorageGrid.cs#L185))
  - Thử tìm `cells` ô trống 1x1 theo thứ tự quét hàng trước cột sau (`row = 0..Rows-1, col = 0..Cols-1`). Nếu không tìm đủ -> từ chối, trả `false`.
- Khi thành công:
  - Dồn vào các stack hiện có của loại đó cho tới khi đạt 999.
  - Lượng còn dư tạo các `MaterialStack` mới đặt vào các ô trống tìm được, `Count` tối đa 999 mỗi ô.
  - Gọi `SyncMaterials()` để đồng bộ `profile.Materials`.

#### 4.2.4. Tiêu hao nguyên liệu (`SpendMaterials(costs, out error)`)
- Kiểm tra trước: Duyệt toàn bộ `costs`, nếu `MaterialCount(id) < count` thì dừng ngay, trả `error = $"Không đủ {name}."`.
- Thực hiện trừ:
  - Duyệt ngược danh sách `state.MaterialStacks` từ cuối lên đầu (`i = count - 1` về 0).
  - Trừ `take = Math.Min(remaining, stack.Count)`. Nếu `stack.Count == 0`, xóa hẳn stack khỏi danh sách (`RemoveAt(i)`).
  - Gọi `SyncMaterials()`.

#### 4.2.5. Dọn kho / Sắp xếp Tàng Bảo Các (`Repack`)
- Các bước thực hiện ([`StoragePopup.cs:397-441`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/StoragePopup.cs#L397-L441)):
  1. Lấy toàn bộ `OwnedItem` trong `Storage.Items`.
  2. Sắp xếp danh sách item theo hàm `CompareItems`:
     - So sánh loại item theo chuỗi mã ASCII/Ordinal tăng dần: `string.Compare(a.Type, b.Type, StringComparison.Ordinal)`.
     - Nếu cùng loại, so sánh phẩm chất giảm dần: `RarityRules.Index(b.Rarity).CompareTo(RarityRules.Index(a.Rarity))`.
     - Nếu cùng phẩm, so sánh Trọng giảm dần: `b.Tier.CompareTo(a.Tier)`.
  3. Gom tổng số lượng từng loại nguyên liệu theo thứ tự xuất hiện đầu tiên (`order` và `totals`).
  4. Xóa sạch: `Storage.Items.Clear()`, `Storage.MaterialStacks.Clear()`, `profile.Materials.Clear()`.
  5. Duyệt danh sách item đã sắp xếp: lần lượt gọi `grid.TryPlaceItem(item)` (tìm ô trống đầu tiên từ trên xuống dưới, trái qua phải theo kích thước thật của item, không xoay).
  6. Duyệt danh sách nguyên liệu theo thứ tự `order`: gọi `grid.TryAddMaterial(id, total)`. Nếu kho hết chỗ, phần dư đẩy vào `Storage.Overflow ("material:" + id + ":" + total)`.

### 4.3. Trường hồ sơ bị thay đổi
- Tọa độ `X, Y` trong `Storage.Items` và `Storage.MaterialStacks`.
- Danh sách `Storage.MaterialStacks` và dictionary `profile.Materials`.
- Không ghi nhận nhiệm vụ nào.

### 4.4. Ca thử
- **Ca 1 (Từ test `FirstFitUsesRealSizeRowMajorWithoutRotation`):**
  - Item "wide" (2x1), Item "tall" (1x3).
  - Kết quả: "wide" đặt tại `(0, 0)`, "tall" đặt tại `(2, 0)`.
- **Ca 2 (Từ test `MaterialsSplitAt999AndSpendingRemovesEmptyStacks`):**
  - Thêm 1000 quặng `ore`. Kết quả: Stack 1 tại `(0, 0)` có 999, Stack 2 tại `(1, 0)` có 1. `profile.Materials["ore"] = 1000`.
  - Tiêu hao 1 `ore`: Stack 2 bị trừ về 0 và bị xóa. Còn lại đúng 1 stack 999.

---

## 5. Xếp Bao Phục

- **Tập tin quy chiếu:**
  - Logic UI và lưu: [`BagLoadoutPanel.cs:198-240`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Common/BagLoadoutPanel.cs#L198-L240)
  - Điều phối chuyển đổi: [`ItemTrayView.cs:85-130`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L85-L130)
  - Kiểm tra tính toàn vẹn: [`PlayerProfile.cs:317-334`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L317-L334)
  - Giao thức: [`docs/tech/protocol.md:159`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/tech/protocol.md#L159) (`loadout_save`)

### 5.1. Thông số Bao Phục
- Lưới kích thước cố định: **5 cột x 4 hàng** (`Cols = 5`, `Rows = 4`).

### 5.2. Điều kiện từ chối khi server nhận `loadout_save`
1. Vật phẩm không tồn tại trong hồ sơ: `uid` không có trong `profile.Items`.
2. Trùng lặp vật phẩm: Cùng một `uid` xuất hiện từ 2 lần trở lên trong danh sách `placements`.
3. Vị trí ra ngoài lưới Bao Phục: `x < 0 || x + width > 5 || y < 0 || y + height > 4`.
4. Chồng đè ô: Các hình chữ nhật chiếm chỗ của các món đồ trong Bao Phục bị đè lên nhau.
5. Kho không đủ chỗ khi tháo đồ: Món đồ cũ nằm trong Bao Phục nhưng không có trong `placements` mới (bị tháo ra), nếu kho Tàng Bảo Các không còn vị trí trống vừa với kích thước món đó (`!storage.TryFindFirst(w, h, out _, out _)`).
   - Câu lỗi: `"Tàng Bảo Các không đủ chỗ."` ([`ItemTrayView.cs:125`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/View/ItemTrayView.cs#L125))

### 5.3. Trường hồ sơ bị thay đổi
1. Các món được trang bị vào Bao Phục bị gỡ khỏi `profile.Storage.Items`.
2. Các món bị tháo khỏi Bao Phục được thêm vào `profile.Storage.Items` với tọa độ ô trống tìm được.
3. `profile.Loadout` được ghi đè toàn bộ bằng danh sách `LoadoutPlacement (uid, x, y)`.
4. Gọi `profile.Validate()` kiểm tra tính nhất quán toàn bộ hồ sơ.
5. Không gọi `RecordQuestAction`, nhưng nhiệm vụ loại `"equip_count"` sẽ đọc trực tiếp `profile.Loadout.Count`.

### 5.4. Ca thử
- **Ca thử tự tính:**
  - Hồ sơ có Thanh Phong Kiếm (kích thước 1x3) và Ô Tàm Y (kích thước 2x2).
  - Client gửi `loadout_save` với placements:
    - Kiếm: `x = 0, y = 0` (chiếm các ô (0,0), (0,1), (0,2) - hợp lệ).
    - Giáp: `x = 1, y = 0` (chiếm các ô (1,0), (1,1), (2,0), (2,1) - hợp lệ).
  - Kết quả mong đợi: `loadout_save_ok`, `profile.Loadout` có 2 phần tử, `profile.Storage.Items` không còn 2 món này.

---

## 6. Thương Hội

- **Tập tin quy chiếu:**
  - Logic dịch vụ: [`ShopService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L1-L387)
  - Catalog: [`ShopCatalog.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopCatalog.cs)
  - Cấu hình dữ liệu: [`Resources/Data/shop.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json#L1-L21)
  - Test chuẩn: [`ShopServiceTests.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/ShopServiceTests.cs)

### 6.1. Chu kỳ Ngày / Tuần (`ResetPeriods`)
- **Reset ngày:** So sánh `profile.Shop.DayKey` với ngày UTC hiện tại (`clock.UtcNow.UtcDateTime.ToString("yyyy-MM-dd")`). Nếu khác:
  - `profile.Shop.DayKey = day;`
  - `profile.Shop.PaidRefreshesToday = 0;` ([`ShopService.cs:348-350`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L348-L350))
- **Reset tuần (thứ Hai hàng tuần):**
  - Tính ngày đầu tuần (thứ Hai): `int back = ((int)date.DayOfWeek + 6) % 7; string week = date.AddDays(-back).ToString("yyyy-MM-dd");`
  - Nếu `profile.Shop.WeekKey != week`:
    - `profile.Shop.WeekKey = week;`
    - `profile.Shop.ReputationPurchases.Clear();` ([`ShopService.cs:356-359`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L356-L359))

### 6.2. Làm mới Thương Hội (`Refresh`)
- **Làm mới miễn phí (`free = true`):**
  - Ưu tiên lượt khởi đầu: Nếu `StarterFreeRefreshesUsed < config.StarterFreeRefreshes` (1 lượt), tăng `StarterFreeRefreshesUsed++` và cho phép làm mới ngay. ([`ShopService.cs:90-91`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L90-L91))
  - Thời gian chờ định kỳ: `FreeRefreshHours = 3` giờ. Nếu chưa hết 3 giờ tính từ `LastFreeRefreshUtc` -> từ chối với câu lỗi:
    `"Chưa đến lượt làm mới miễn phí."` ([`ShopService.cs:89`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L89))
  - Khi thành công: cập nhật `LastFreeRefreshUtc = clock.UtcNow.ToString("O")`.
- **Làm mới trả phí (`free = false`):**
  - Chi phí: `PaidRefreshCost = 50` Bạc ([`shop.json:4`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json#L4)).
  - Kiểm tra Bạc: nếu `silver < 50` -> từ chối với câu lỗi:
    `$"Không đủ Bạc ({silver}/{cost})."` ([`ShopService.cs:99`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L99))
  - Khi thành công: trừ 50 Bạc, tăng `profile.Shop.PaidRefreshesToday++`.
- **Cả 2 trường hợp:** Gọi `Generate(profile)` để sinh lại 6 ô hàng mới.

### 6.3. Quy tắc sinh 6 ô hàng (`Generate` & `CreateSlot`)
- PRNG khởi tạo bằng: `new Random(unchecked(seed * 486187739 + profile.Shop.Generation++))` ([`ShopService.cs:237, 331`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L237)).
- **Quy tắc đặc biệt ô đầu tiên cho tài khoản mới (`Generation == 0`):**
  - Ô số 0 luôn được cố định là Thái Thanh Tâm Kinh: `FirstShopItemId = "thai_thanh_tam_kinh"`, phẩm `common`, giảm giá 0% ([`ShopService.cs:241-242`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L241-L242)).
- **Cấu hình 6 ô theo `shop.json`:**
  - **Ô 0 (`item`):** Vật phẩm phẩm Phàm/Ưu/Hiếm với trọng số [45, 40, 15].
  - **Ô 1 (`item`):** Vật phẩm phẩm Ưu/Hiếm/Cực với trọng số [50, 35, 15].
  - **Ô 2 (`material`):** Nguyên liệu Bậc 1, số lượng 5, giá 250 Bạc.
  - **Ô 3 (`material`):** Nguyên liệu Bậc 2, số lượng 5, giá 750 Bạc.
  - **Ô 4 (`catalyst_or_tier3`):** Quay `random.Next(100)`:
    - Nếu `< 40` (40%): Chất dẫn (Bậc 0, `isCatalyst: true`, trừ Ngọc Tủy), số lượng 1, giá 1.200 Bạc.
    - Nếu `>= 40` (60%): Nguyên liệu Bậc 3 thường, số lượng 2, giá 800 Bạc.
  - **Ô 5 (`discount`):** Quay `random.Next(100)`:
    - Nếu `>= 30` (70%): Vật phẩm giảm giá 20% (phẩm Ưu/Hiếm với trọng số [70, 30]).
    - Nếu `< 30` (30%): Nguyên liệu nhóm `pill_relic` Bậc 1, số lượng 5, giá `250 * (100 - 20) / 100 = 200` Bạc.
- **Công thức tính giá bán vật phẩm:**
  - Tra giá gốc `config.ItemBasePrices[rarity]`: common=200, uncommon=500, rare=1200, epic=3000, legendary=8000.
  - Hệ số loại đồ `config.ItemTypePriceMultipliers`: `weapon = 2.0` (vũ khí nhân đôi), các loại khác = 1.0.
  - Làm tròn giá trước giảm: `long price = (long)Math.Round(basePrice * multiplier);` (**Banker's rounding!**). ([`ShopService.cs:280`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L280))
  - Áp dụng giảm giá: `if (discountPercent > 0) price = price * (100 - discountPercent) / 100;` (chia nguyên).

### 6.4. Mua hàng Thương Hội (`Buy`)
- Điều kiện từ chối:
  - `index < 0 || index >= slots.Count` -> `"Ô hàng không hợp lệ."` ([`ShopService.cs:116`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L116))
  - `slot.Bought` -> `"Ô hàng này đã mua."` ([`ShopService.cs:119`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L119))
  - `silver < slot.Price` -> `$"Không đủ Bạc ({silver}/{slot.Price})."` ([`ShopService.cs:122`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L122))
  - Kho Tàng Bảo Các không đủ chỗ: `"Tàng Bảo Các không đủ chỗ."` ([`ShopService.cs:128, 131`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L128))
- Thay đổi hồ sơ:
  1. `Currencies["bac"] -= slot.Price;`
  2. Thêm đồ: nếu là item -> `storage.TryPlaceItem(profile.AddItem(slot.ContentId, 1, rarity: slot.Rarity));` (luôn là Trọng 1); nếu là nguyên liệu -> `storage.TryAddMaterial(slot.ContentId, slot.Quantity);`.
  3. `slot.Bought = true;`
  4. **Ghi nhận nhiệm vụ:** Có. Gọi `profile.RecordQuestAction("shop_buy")` -> tăng `ActionCounters["shop_buy"]` thêm 1. ([`ShopService.cs:138`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L138))

### 6.5. Đổi nguyên liệu Thương Hội (`Exchange`)
- Điều kiện từ chối:
  - ID không có trong `materials.json` -> `"Nguyên liệu không hợp lệ."` ([`ShopService.cs:161`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L161))
  - Đổi chất dẫn hoặc Ngọc Tủy: `input.IsCatalyst || target.IsCatalyst || input.Id == "chi_ton_ngoc_tuy" || target.Id == "chi_ton_ngoc_tuy"` -> `"Không thể đổi chất dẫn hoặc Chí Tôn Ngọc Tủy."` ([`ShopService.cs:168`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L168))
  - Sai quy tắc bậc/nhóm: chỉ cho phép cùng nhóm lên 1 bậc (`same = input.ItemType == target.ItemType && target.Tier == input.Tier + 1`) HOẶC ngang bậc khác nhóm (`cross = input.ItemType != target.ItemType && target.Tier == input.Tier`). Nếu không thỏa -> `"Chỉ đổi lên một bậc cùng nhóm hoặc ngang bậc khác nhóm."` ([`ShopService.cs:172`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L172))
  - Không đủ nguyên liệu nộp: `owned < count` -> `$"Không đủ {input.Name} ({owned}/{count})."` ([`ShopService.cs:178`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L178))
    - Cùng nhóm lên bậc: cần 3 nguyên liệu vào (`sameInput = 3`). Phí Bạc: Bậc 1 lên 2 = 50 Bạc; Bậc 2 lên 3 = 150 Bạc.
    - Khác nhóm ngang bậc: cần 2 nguyên liệu vào (`crossInput = 2`). Phí Bạc: Bậc 1 = 25 Bạc; Bậc 2 = 75 Bạc; Bậc 3 = 200 Bạc.
  - Không đủ Bạc: `silver < fee` -> `$"Không đủ Bạc ({silver}/{fee})."` ([`ShopService.cs:180`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L180))
  - Kho không đủ chỗ nhận 1 nguyên liệu ra: `"Tàng Bảo Các không đủ chỗ."` (đồng thời hoàn lại nguyên liệu vừa trừ). ([`ShopService.cs:189`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L189))
- Thay đổi hồ sơ:
  - Trừ nguyên liệu vào từ `Storage.MaterialStacks` (đồng bộ `Materials`).
  - Thêm 1 nguyên liệu ra vào `Storage.MaterialStacks` (đồng bộ `Materials`).
  - `Currencies["bac"] -= fee;`
  - Không ghi nhận nhiệm vụ.

### 6.6. Tiệm Danh Vọng (`BuyReputation`)
- Danh mục theo `shop.json:16-20`:
  1. `chi_ton_ngoc_tuy`: Giá 500 Danh Vọng, giới hạn 1 lần/tuần, nhận 1 viên.
  2. `catalyst_box_random`: Giá 250 Danh Vọng, giới hạn 3 lần/tuần, nhận 1 chất dẫn ngẫu nhiên (trừ Ngọc Tủy).
  3. `tier_3_material_box`: Giá 100 Danh Vọng, giới hạn 5 lần/tuần, nhận 3 nguyên liệu Bậc 3 thường ngẫu nhiên.
- Điều kiện từ chối:
  - Offer không tồn tại -> `"Món Danh Vọng không hợp lệ."` ([`ShopService.cs:211`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L211))
  - Đã hết lượt mua trong tuần -> `"Đã hết lượt mua trong tuần."` ([`ShopService.cs:213`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L213))
  - Không đủ Danh Vọng -> `$"Không đủ Danh Vọng ({value}/{offer.Price})."` ([`ShopService.cs:216`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L216))
- PRNG mở hộp: `Rng(profile.Shop.Generation * 397 + StableHash(offerId))`.
- Thay đổi hồ sơ:
  - `Currencies["danh_vong"] -= offer.Price;`
  - `profile.AddMaterial(reward, offer.Quantity);`
  - `profile.Shop.ReputationPurchases[offerId]++;`
  - Không ghi nhận nhiệm vụ.

---

## 7. Thể Lực

- **Tập tin quy chiếu:**
  - Logic dịch vụ: [`PveProgress.cs:57-145`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L57-L145) (`StaminaService`)
  - Cấu hình: [`Resources/Data/Pve/stamina.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/stamina.json#L1-L4)
  - Test chuẩn: [`PveSystemTests.cs:265-381`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Pve/PveSystemTests.cs#L265-L381)

### 7.1. Cấu hình Thể Lực
- `max = 60`
- `regenPerMinute = 0.1666666667f` (tức đúng **1 điểm mỗi 6 phút**, hay 360 giây / 1 điểm).

### 7.2. Hồi phục Thể Lực (`Refresh`)
- Nếu `state.Stamina < 0` (chưa khởi tạo): gán `state.Stamina = config.Max` (60).
- Nếu `state.Stamina >= config.Max`:
  - `state.Stamina = config.Max;`
  - `state.LastStaminaRegenUtc = now.ToString("O");`
  - Trả về 60.
- Nếu `state.Stamina < config.Max`:
  - `double perPoint = 1d / config.RegenPerMinute;` (6.0 phút).
  - `int gained = (int)((now - last).TotalMinutes / perPoint);` (ép kiểu cắt phần thập phân). ([`PveProgress.cs:80`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L80))
  - Nếu `gained > 0`:
    - `state.Stamina = Math.Min(config.Max, state.Stamina + gained);`
    - Cập nhật mốc: `state.LastStaminaRegenUtc = last.AddMinutes(gained * perPoint).ToString("O");` (bảo tồn tiến độ lẻ của điểm kế tiếp).

### 7.3. Tiêu hao Thể Lực (`TrySpend`)
- Luôn gọi `Refresh(state, now)` trước khi kiểm tra số dư.
- Nếu `amount < 0`: ném `ArgumentOutOfRangeException`.
- Nếu `state.Stamina < amount`: trả về `false`.
- Nếu đủ: `state.Stamina -= amount; return true;` (tiến độ lẻ của điểm tiếp theo vẫn được giữ nguyên qua mốc `LastStaminaRegenUtc`).

### 7.4. Đếm ngược hiển thị
- Số giây tới điểm kế tiếp: `SecondsToNext = perPoint - elapsed % perPoint;` (trả về 0 nếu Thể Lực đã đầy).
- Số giây tới khi đầy bình: `SecondsToFull = Math.Max(0d, (config.Max - state.Stamina) * perPoint - elapsed);`.

### 7.5. Ca thử
- **Ca 1 (Từ test `StaminaRegeneratesAndSpends`):**
  - Thể Lực = 5, mốc `2026-01-01T00:00:00Z`, cấu hình Max=10, 0.5 điểm/phút (2 phút/điểm).
  - Lúc `00:04:01Z` (trôi qua 4 phút 1 giây): Hồi được `4.016 / 2 = 2` điểm -> Thể Lực = 7.
  - Tiêu hao 6 điểm -> Thể Lực còn 1. Thử tiêu hao tiếp 2 điểm -> Thất bại (`false`).
- **Ca 2 (Từ test `TrySpendKeepsPartialRegenProgress`):**
  - Thể Lực = 50, mốc T0, sau 4 phút tiêu hao 6 điểm.
  - Kết quả: Thể Lực còn 44, `SecondsToNext` còn đúng 120 giây (vì 6 phút - 4 phút = 2 phút = 120s).

---

## 8. Trận PvE

- **Tập tin quy chiếu:**
  - Logic thưởng: [`PveRewardService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L1-L137)
  - Điều kiện mở ải: [`PveProgress.cs:30-55`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L30-L55) (`PveRules`)
  - Điều phối kết quả: [`GameplayController.cs:255-297`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L255-L297) (`ShowPveResult`)
  - Dữ liệu ải & quái: [`Resources/Data/Pve/chapters.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json)
  - Test chuẩn: [`PveSystemTests.cs:41-51, 130-263`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Pve/PveSystemTests.cs#L41)

### 8.1. Điều kiện vào ải
1. **Kiểm tra mở Hồi (`IsChapterUnlocked`):**
   - Hồi 1 luôn mở.
   - Hồi sau yêu cầu ải cuối cùng (ải 10 - Boss) của Hồi trước phải có `Cleared == true`. ([`PveProgress.cs:32-41`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L32-L41))
2. **Kiểm tra mở Ải (`IsStageUnlocked`):**
   - Ải đầu tiên của Hồi (index 0) mở khi Hồi đã mở.
   - Các ải tiếp theo yêu cầu ải liền trước trong cùng Hồi phải có `Cleared == true`. ([`PveProgress.cs:43-49`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveProgress.cs#L43-L49))
3. **Thể Lực:** Tiêu hao `stage.StaminaCost` = 5 Thể Lực qua `StaminaService.TrySpend`. Nếu thiếu Thể Lực -> từ chối vào trận.

### 8.2. Dữ liệu Quái trong ải
- Đọc từ `stage.Enemy`:
  - `Name`, `Portrait`, `Sect`, `TamPhapLevel`, `Level`.
  - Danh sách trang bị `Items`: từng món gồm `id`, tọa độ `x, y`, `tier`, `rarity`.
  - Máu quái trong engine: `maxHp = 1200 + 5 * (level - 1)`. ([`PveSystemTests.cs:50`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Pve/PveSystemTests.cs#L50))

### 8.3. Thưởng Vượt Ải Lần Đầu (`FirstClear`)
- Bạc: `stage.Rewards.FirstClear.Silver` (`long`).
- EXP: `stage.Rewards.FirstClear.HeroExp` (`long`).
- Nguyên liệu: Dictionary `stage.Rewards.FirstClear.Materials`.
- Trang bị: `stage.Rewards.FirstClear.Items` (rỗng theo Quyết định #52).

### 8.4. Thưởng Đánh Lại (`Repeat`)
- Tỉ lệ: `RepeatRewardPercent = 15`%.
- **Bạc đánh lại:**
  ```csharp
  decimal exact = firstClearAmount * (decimal)repeatRewardPercent / 100m;
  return Math.Max(1, (long)Math.Round(exact, MidpointRounding.AwayFromZero));
  ```
  *(Làm tròn số bán nguyên ra xa số 0: ví dụ 7.5 làm tròn thành 8. Tối thiểu là 1 nếu Bạc gốc > 0).* ([`PveRewardService.cs:95-96`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L95-L96))
- **EXP đánh lại:** Tương tự công thức Bạc (nhân 15%, làm tròn `AwayFromZero`, tối thiểu 1).
- **Nguyên liệu đánh lại (xác suất LCG):**
  - Sắp xếp danh sách material ID theo thứ tự Ordinal: `materialIds.Sort(StringComparer.Ordinal);` ([`PveRewardService.cs:76`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L76))
  - Seed LCG khởi đầu: `uint state = unchecked((uint)seed) ^ 0xA5A5DE40u;` ([`PveRewardService.cs:74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L74))
  - Với mỗi nguyên liệu:
    - `state = unchecked(state * 1664525u + 1013904223u);`
    - `long scaled = reward.Materials[materialId] * (long)repeatRewardPercent;`
    - `int count = (int)(scaled / 100);`
    - `if ((state >> 16) % 100 < scaled % 100) count++;`
    - Nếu `count > 0`, trao `count` nguyên liệu đó.

### 8.5. Rơi Đồ Ngẫu Nhiên (`Drops`)
- Áp dụng cho cả lần đầu lẫn đánh lại:
  - Seed LCG: `uint state = unchecked((uint)seed) ^ 0x85EBCA6Bu;` ([`PveRewardService.cs:101`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L101))
  - Duyệt danh sách `stage.Rewards.Drops`:
    - `state = unchecked(state * 1664525u + 1013904223u);`
    - Nếu `state % 1000 < drop.ChancePermille`: rơi 1 món đồ `drop.Id`, Trọng 1, phẩm `drop.Rarity`.

### 8.6. Kiểm tra chỗ trống và cập nhật hồ sơ khi thắng
- **Preflight check:** Tạo bản sao giả định hồ sơ. Thử xếp toàn bộ nguyên liệu và đồ rơi vào kho. Nếu không đủ chỗ:
  - Dừng lại, trả về lỗi: `"Tàng Bảo Các không đủ chỗ."` ([`PveRewardService.cs:43`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Pve/PveRewardService.cs#L43))
  - Hồ sơ thật giữ nguyên 100%.
- **Khi đủ chỗ, thứ tự cập nhật hồ sơ thật:**
  1. `profile.Pve.Stages[stage.Id].Cleared = true;` ([`GameplayController.cs:265`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L265))
  2. `profile.RecordQuestAction("pve_win");` -> tăng `ActionCounters["pve_win"]` thêm 1. ([`GameplayController.cs:266`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L266))
  3. Reset ngày thưởng: nếu `DailyFirstClearDate != today` (chuỗi `yyyy-MM-dd`), xóa `DailyFirstClears` và gán lại date. Thêm `stage.Id` vào `DailyFirstClears`. ([`GameplayController.cs:267-275`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Gameplay/GameplayController.cs#L267-L275))
  4. Thêm trang bị rơi vào `Items` & `Storage.Items`.
  5. Thêm nguyên liệu vào `Storage.MaterialStacks` (đồng bộ `Materials`).
  6. Thêm Bạc vào `Currencies["bac"]`.
  7. Cộng EXP qua `LevelService.Grant(profile, HeroExp)`.
  8. Mở Hồi cốt truyện: nếu là lần đầu hạ Boss (`firstClear && stage.Index == 10 && stage.Kind == "boss"`), thêm `stage.Chapter.Index` vào `profile.PendingStoryChapters`. ([`StoryProgress.cs:9-18`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L9-L18))

### 8.7. Ca thử
- **Ca 1 (Từ test `FirstClearRewardRemainsUnscaled`):**
  - Ải `hoi_1_01`, lần đầu.
  - Kết quả: 300 Bạc, 50 EXP, 3 Hàn Thiết, 0 đồ rơi.
- **Ca 2 (Từ test `ReplayUsesFifteenPercentOfStageFirstClearReward`):**
  - Ải `hoi_1_01`, đánh lại, seed = 1.
  - Bạc: `300 * 0.15 = 45`.
  - EXP: `50 * 0.15 = 7.5` -> làm tròn `AwayFromZero` = 8.
  - Hàn Thiết: `3 * 15 = 45%` xác suất rơi 1 viên.
- **Ca 3 (Từ test `ReplayRoundsPositiveSilverUpToMinimumOne`):**
  - Ải có thưởng lần đầu 3 Bạc. Đánh lại: `3 * 0.15 = 0.45` -> làm tròn = 0, nhưng qua `Math.Max(1, ...)` ra 1 Bạc.

---

## 9. EXP và Lên Cấp

- **Tập tin quy chiếu:**
  - Logic dịch vụ: [`LevelService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L1-L117)
  - Cấu hình: [`Resources/Data/levels.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/levels.json#L1-L37)
  - Test chuẩn: [`LevelServiceTests.cs:12-31`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/LevelServiceTests.cs#L12-L31)

### 9.1. Cấu hình
- `MaxLevel = 30`
- `ExpToNext`: mảng 29 phần tử cho các cấp 1 đến 29:
  `[30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 210, 240, 280, 320, 370, 420, 480, 550, 630, 720, 820, 930, 1050, 1180, 1320, 1480, 1650]`.
- Cấp 30 cần 0 EXP.

### 9.2. Công thức cộng EXP (`Grant`)
1. Nếu `amount < 0`: ném `ArgumentOutOfRangeException`.
2. Nếu `profile.HeroLevel >= MaxLevel`:
   - Gán `profile.HeroLevel = 30;`
   - Gán `profile.HeroExp = 0;`
   - Dừng lại, không cộng thêm. ([`LevelService.cs:98-100`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L98-L100))
3. Cộng dồn: `profile.HeroExp = checked(profile.HeroExp + amount);`
4. Vòng lặp thăng cấp:
   ```csharp
   while (profile.HeroLevel < config.MaxLevel && profile.HeroExp >= ExpToNext(profile.HeroLevel))
   {
       int before = profile.HeroLevel;
       profile.HeroExp -= ExpToNext(before);
       profile.HeroLevel++;
   }
   ```
5. Khi chạm trần cấp 30: `if (profile.HeroLevel == config.MaxLevel) profile.HeroExp = 0;` (toàn bộ EXP dư thừa bị hủy bỏ). ([`LevelService.cs:112-113`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/LevelService.cs#L112-L113))

### 9.3. Trường hồ sơ bị thay đổi
1. `profile.HeroExp`
2. `profile.HeroLevel`
- Không trực tiếp ghi nhận nhiệm vụ, nhưng nhiệm vụ loại `"hero_level"` đọc trực tiếp `profile.HeroLevel`.

### 9.4. Ca thử
- **Ca 1 (Từ test `Grant_CarriesAcrossSeveralLevels`):**
  - Đường cong mẫu: `expToNext: [100, 200, 300]`, cấp tối đa 4.
  - Ban đầu: Cấp 1, 0 EXP. Nhận 350 EXP.
  - Lên cấp 2: trừ 100 còn 250. Lên cấp 3: trừ 200 còn 50.
  - Kết quả: `HeroLevel = 3`, `HeroExp = 50`.
- **Ca 2 (Từ test `Grant_AtCap_DiscardsRemainingExp`):**
  - Nhận 10.000 EXP: Đạt cấp 4 (max), `HeroExp = 0`.
  - Nhận thêm 50 EXP: `HeroLevel = 4`, `HeroExp = 0`.

---

## 10. Nhiệm Vụ

- **Tập tin quy chiếu:**
  - Logic dịch vụ: [`QuestService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L1-L202)
  - Catalog: [`QuestCatalog.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestCatalog.cs)
  - Cấu hình chuỗi nhiệm vụ: [`Resources/Data/quests.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/quests.json)
  - Test chuẩn: [`QuestSystemTests.cs:69-171`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Quests/QuestSystemTests.cs#L69)

### 10.1. Cấu trúc tuyến tính
- Toàn bộ 45 nhiệm vụ (9 nhiệm vụ mỗi Hồi x 5 Hồi) được kích hoạt tuần tự qua chỉ số `ActiveIndex` (0–44).
- Người chơi chỉ có duy nhất **1 nhiệm vụ kích hoạt** tại một thời điểm (`ActiveId`).

### 10.2. Các loại mục tiêu và cách tính tiến độ (`Progress`)
- **Nhóm 1: Mục tiêu theo trạng thái hồ sơ (State Objectives):**
  - `"stage_clear"`: `current = PveRules.IsCleared(profile, stage) ? 1 : 0`, mục tiêu = 1.
  - `"hero_level"`: `current = profile.HeroLevel`, mục tiêu = `objective.Level`.
  - `"sect_joined"`: `current = !string.IsNullOrWhiteSpace(profile.SectId) ? 1 : 0`, mục tiêu = 1.
  - `"tam_phap_level"`: `current = profile.TamPhapLevel`, mục tiêu = `objective.Level`.
  - `"equip_count"`: `current = profile.Loadout.Count`, mục tiêu = `objective.Count`.
  - `"item_tier"`: `current = HighestTier(profile)` (duyệt toàn bộ `Items` tìm `Tier` cao nhất), mục tiêu = `objective.Tier`.
  *(Nếu đã đạt từ trước khi nhận nhiệm vụ, hoàn thành ngay lập tức).*
- **Nhóm 2: Mục tiêu theo hành động (Action Objectives):**
  - Các loại hành động: `"shop_buy"`, `"item_upgrade"`, `"item_merge"`, `"pve_win"`.
  - Công thức: `current = profile.Quest.ActionCounters[type] - profile.Quest.ActionBaseline[type]`. ([`QuestService.cs:81`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L81))
  - Mục tiêu: `objective.Count`.
  *(Chỉ tính các hành vi phát sinh **sau thời điểm nhiệm vụ được kích hoạt**, nhờ mốc snapshot `ActionBaseline`).*

### 10.3. Nhận thưởng nhiệm vụ (`Claim`)
- Điều kiện từ chối:
  - `quest == null || !Progress(profile, quest).Done`:
    `"Nhiệm vụ chưa hoàn thành."` ([`QuestService.cs:91`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L91))
  - Kho Tàng Bảo Các không đủ chỗ nhận nguyên liệu thưởng (kiểm tra qua clone thử nghiệm):
    `"Tàng Bảo Các không đủ chỗ."` ([`QuestService.cs:94`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L94))
- Thay đổi hồ sơ khi nhận thưởng:
  1. Thêm nguyên liệu thưởng vào `Storage.MaterialStacks` (đồng bộ `Materials`).
  2. Thêm Bạc thưởng vào `Currencies["bac"]`.
  3. Cộng EXP thưởng qua `LevelService.Grant(profile, reward.HeroExp)`.
  4. Đánh dấu đã nhận: `profile.Quest.ClaimedIds.Add(quest.Id);` ([`QuestService.cs:98`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L98))
  5. Chuyển nhiệm vụ kế tiếp: `profile.Quest.ActiveIndex++;` ([`QuestService.cs:99`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L99))
  6. Chụp lại mốc: `SetBaseline(profile)` -> gán `ActiveId` của nhiệm vụ mới và sao chép toàn bộ `ActionCounters` sang `ActionBaseline`. ([`QuestService.cs:155-156`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Quests/QuestService.cs#L155-L156))

### 10.4. Ca thử
- **Ca 1 (Từ test `ActionObjective_CountsOnlyAfterActivation`):**
  - Người chơi mua shop 1 lần (`ActionCounters["shop_buy"] = 1`).
  - Kích hoạt nhiệm vụ mua shop 1 lần. Mốc `Baseline = 1`.
  - Kiểm tra tiến độ: `1 - 1 = 0/1` (chưa xong). Mua thêm 1 lần -> `2 - 1 = 1/1` (hoàn thành).
- **Ca 2 (Từ test `Claim_WhenStorageIsFullPaysNothing`):**
  - Kho 1x1 đầy kín. Nhiệm vụ q1_01 hoàn thành. Bấm nhận thưởng -> Báo lỗi `"Tàng Bảo Các không đủ chỗ."`, Bạc không tăng, `ActiveIndex` vẫn là 0.

---

## 11. Chọn Phái

- **Tập tin quy chiếu:**
  - Logic: [`SectChoiceService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L1-L39)
  - Catalog: [`Resources/Data/sects.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/sects.json)
  - Test chuẩn: [`LevelServiceTests.cs:48-58`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/LevelServiceTests.cs#L48-L58)

### 11.1. Điều kiện từ chối và câu lỗi nguyên văn
1. Nhân vật chưa đạt cấp 10 (`SectUnlockLevel = 10`):
   - Kiểm tra: `profile.HeroLevel < 10`
   - Câu lỗi: `"Đạt cấp 10 để gia nhập môn phái"` ([`SectChoiceService.cs:17`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L17))
2. Đã chọn phái từ trước:
   - Kiểm tra: `!string.IsNullOrWhiteSpace(profile.SectId)`
   - Câu lỗi: `"Môn phái đã chọn không thể thay đổi."` ([`SectChoiceService.cs:23`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L23))
3. ID môn phái không có trong `sects.json`:
   - Kiểm tra: `!catalog.TryGet(sectId, out _)`
   - Câu lỗi: `"Môn phái không hợp lệ."` ([`SectChoiceService.cs:29`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L29))

### 11.2. Trường hồ sơ bị thay đổi
1. `profile.SectId = sectId;` ([`SectChoiceService.cs:33`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L33))
2. `profile.TamPhapLevel = 1;` ([`SectChoiceService.cs:34`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectChoiceService.cs#L34))
- Không gọi `RecordQuestAction`, nhưng nhiệm vụ loại `"sect_joined"` sẽ hoàn thành ngay khi `SectId` có giá trị.

### 11.3. Ca thử
- **Ca 1 (Từ test `Sect_CannotBeChosenBelowTenOrTwice`):**
  - Cấp 1 chọn phái -> Thất bại.
  - Gán Cấp 10, chọn `kim_cang` -> Thành công, `TamPhapLevel = 1`.
  - Chọn lại `thai_thanh` -> Thất bại, vẫn là `kim_cang`.

---

## 12. Hướng Dẫn, Intro, Cốt Truyện

- **Tập tin quy chiếu:**
  - Logic Hướng dẫn: [`TutorialService.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L1-L97)
  - Logic Cốt truyện: [`StoryProgress.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L1-L47)
  - Test chuẩn: [`OnboardingTests.cs:42-75, 95-125`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Onboarding/OnboardingTests.cs#L42)
  - Giao thức: [`docs/tech/protocol.md:171`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/tech/protocol.md#L171) (`onboarding`)

### 12.1. Thao tác và biến đổi hồ sơ
1. **Xem xong Intro (`intro_seen`):**
   - Đổi: `profile.IntroSeen = true;`
2. **Hoàn thành chuỗi hướng dẫn (`tutorial_done`):**
   - 4 định danh chuỗi hợp lệ: `"first_battle"`, `"shop"`, `"upgrade"`, `"sect"`.
   - Đổi: nếu chưa có thì `profile.CompletedTutorials.Add(id);`. ([`TutorialService.cs:91-92`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/TutorialService.cs#L91-L92))
3. **Mở xem Hồi cốt truyện (`story_seen`):**
   - Nhận `chapterIndex` (1–5).
   - Đổi: `profile.PendingStoryChapters.Remove(chapter);` và thêm vào `profile.SeenStoryChapters.Add(chapter);` (nếu chưa có). ([`StoryProgress.cs:22-24`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Onboarding/StoryProgress.cs#L22-L24))
4. **Đã hiện nhắc nhở môn phái (`sect_prompt_shown`):**
   - Đổi: `profile.SectPromptShown = true;`

### 12.2. Ca thử
- **Ca 1 (Từ test `FirstBossClearQueuesStoryOnlyOnce`):**
  - Hạ Boss `hoi_1_10` lần đầu: `PendingStoryChapters` có `[1]`. Hạ lần 2 không thêm lại.
- **Ca 2 (Từ test `StoryListContainsIntroAndExactlyClearedChapters`):**
  - Đã qua Boss Hồi 1 và 3: Danh sách mở khóa gồm Hồi 0 (Intro), Hồi 1, Hồi 3.

---

## 13. Chiến Uy (PowerCatalog)

- **Tập tin quy chiếu:**
  - Logic: [`PowerCatalog.cs`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PowerCatalog.cs#L1-L187)
  - Cấu hình: [`Resources/Data/power.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/power.json#L1-L106)
  - Test chuẩn: [`PowerCatalogTests.cs:36-93`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Economy/PowerCatalogTests.cs#L36-L93)

### 13.1. Công thức Lực Chiến v4 cộng dồn (Version 1 theo Quyết định #61)
- `power.json` đang chạy `version: 1`. Server tính Chiến Uy của người chơi và ghost theo công thức này:
  ```csharp
  double total = LevelBase + LevelPoints * (Mathf.Max(1, level) - 1);
  foreach (OwnedItem item in equippedItems)
  {
      double basePower = Items.TryGetValue(item.ItemId, out var val) ? (double)val : 25.0;
      double tierFactor = TierFactor[item.Tier.ToString()];
      double rarityFactor = RarityFactor[item.Rarity];
      total += basePower * tierFactor * rarityFactor;
  }
  return (int)Math.Round(total, MidpointRounding.ToEven);
  ```
- **Hệ số cấu hình:**
  - `LevelBase = 30`, `LevelPoints = 3` mỗi cấp trên cấp 1.
  - `TierFactor`: `1: 1.0`, `2: 1.05`, `3: 1.1`, `4: 1.15`, `5: 1.2`, `6: 1.25`, `7: 1.3`, `8: 1.35`, `9: 1.4`.
  - `RarityFactor`: `common: 1.0`, `uncommon: 1.1`, `rare: 1.21`, `epic: 1.33`, `legendary: 1.46`, `supreme: 1.6`.
  - Món không rõ ID: lấy điểm gốc = `25.0`.
- **Làm tròn:** `Math.Round(total, MidpointRounding.ToEven)` (**Banker's rounding!**). ([`PowerCatalog.cs:146`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PowerCatalog.cs#L146))

### 13.2. Ca thử
- **Bộ khởi đầu Cấp 1:**
  - Điểm cấp = 30.
  - Thanh Phong Kiếm (3 ô) = `30 * 1.0 * 1.0 = 30`.
  - Ô Tàm Y (4 ô) = `40 * 1.0 * 1.0 = 40`.
  - Tổng = `30 + 30 + 40 = 100` Lực Chiến. (Khớp quyết định [#61](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L154)).

---

## 14. Bộ Khởi Đầu

- **Tập tin quy chiếu:**
  - Logic: [`StarterKit.cs:32-74`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/StarterKit.cs#L32-L74)
  - Cấu hình: [`Resources/Data/starter.json`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/starter.json#L1-L22)
  - Khởi tạo kho: [`PlayerProfile.cs:440-471`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/PlayerProfile.cs#L440-L471) (`EnsureStorage`)

### 14.1. Giá trị khởi tạo khi đăng ký mới
- `HeroName = heroName`
- `HeroLevel = 1`, `HeroExp = 0`
- `SectId = null`, `TamPhapLevel = 1`, `SectPromptShown = false`
- `IntroSeen = false`, `CompletedTutorials = []`
- `SeenStoryChapters = []`, `PendingStoryChapters = []`
- `Currencies`:
  - `bac = 500` (theo Quyết định [#48](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/decisions.md#L309))
  - `nguyen_bao = 0`
  - `danh_vong = 0`
- `Items`:
  - 1 Thanh Phong Kiếm: `itemId = "thanh_phong_kiem"`, `tier = 1`, `rarity = "common"`, `uid = "starter_sword"`
  - 1 Ô Tàm Y: `itemId = "o_tam_y"`, `tier = 1`, `rarity = "common"`, `uid = "starter_armor"`
- `Loadout`: `[]` (danh sách rỗng).
- `Storage`:
  - `Cols = 10`, `Rows = 20`.
  - Khi qua `EnsureStorage`: cả 2 món Thanh Phong Kiếm và Ô Tàm Y được xếp vào `Storage.Items` (Thanh Phong Kiếm 1x3 tại `(0,0)`, Ô Tàm Y 2x2 tại `(1,0)`).
  - `MaterialStacks = []`, `Overflow = []`.
- `Shop`: `Generation = 0`, `Slots = []`, `StarterFreeRefreshesUsed = 0`, `PaidRefreshesToday = 0`.
- `Pve`: `Stamina = 60`, `Stages = {}`, `DailyFirstClears = []`.
- `Quest`: `ActiveIndex = 0`, `ActiveId = ""`, `ClaimedIds = []`, `ActionCounters = {}`, `ActionBaseline = {}`.

---

## 15. Bảng Cheat

- **Tập tin quy chiếu:** [`DevCheatPanel.cs:94-232`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/DevCheatPanel.cs#L94-L232)
- Áp dụng khi server chạy cờ `--dev`, xử lý qua gói `dev_cheat { "action": "..." }`:

| Tên Action | Tham số | Thao tác tương đương trên C# | Xử lý lỗi / Từ chối |
|---|---|---|---|
| `"add_item"` | `itemId`, `tier` (1–9), `rarity` | `profile.AddItem(id, tier, rarity: rarity)` rồi gọi `storage.TryPlaceItem(item)`. | Nếu kho hết chỗ: xóa item vừa add khỏi `Items`, trả lỗi `"Tàng Bảo Các không đủ chỗ."`. |
| `"add_material"` | `materialId`, `amount` (10 hoặc 100) | `storage.TryAddMaterial(id, amount)`. | Nếu kho hết chỗ: trả lỗi `"Tàng Bảo Các không đủ chỗ."`. |
| `"add_silver"` | `amount` (mặc định 10.000) | `profile.AddCurrency("bac", amount)`. | Không từ chối. |
| `"add_reputation"` | `amount` (mặc định 1.000) | `profile.AddCurrency("danh_vong", amount)`. | Không từ chối. |
| `"refresh_shop"` | Không | `shopService.ForceFreeRefresh(profile)`. | Luôn sinh 6 ô mới bất kể giờ. |
| `"advance_days"` | `days` (7) | Đẩy đồng hồ ảo qua 7 ngày, trigger reset ngày/tuần. | Không từ chối. |
| `"unlock_pve"` | Không | Duyệt toàn bộ ải trong 5 Hồi gán `Pve.Stages[id].Cleared = true`. | Không từ chối. |
| `"refill_stamina"` | Không | `staminaService.Refill(Pve, UtcNow)` -> gán Thể Lực = 60. | Không từ chối. |
| `"clear_stage"` | `stageId` (nếu null lấy ải đầu) | Gán `Pve.Stages[stageId].Cleared = true`. | Không từ chối. |
| `"add_level"` | Không | `grant = ExpToNext(level) - HeroExp; levelService.Grant(profile, grant);`. | Nếu đã max cấp (30) thì bỏ qua. |
| `"add_exp"` | `amount` (100) | `levelService.Grant(profile, 100);`. | Tự thăng cấp nếu đủ. |
| `"reset_sect"` | Không | `profile.SectId = null; profile.TamPhapLevel = 1; profile.SectPromptShown = false;`. | Cho phép chọn lại môn phái. |
| `"reset_profile"` | Không | Khởi tạo lại toàn bộ hồ sơ từ `starter.json`. | |

---

## 16. Chỗ Dễ Chép Sai Sang Java

Dưới đây là các khác biệt kỹ thuật tinh vi giữa runtime C# (.NET) và Java mà lập trình viên rất dễ mắc lỗi:

### 16.1. Quy tắc làm tròn số (`Math.Round` vs `Math.round`)
- **Trong C#:** `Math.Round(double)` mặc định sử dụng **Banker's Rounding** (`MidpointRounding.ToEven` - làm tròn về số chẵn gần nhất khi ở điểm giữa 0.5):
  - Ví dụ: `2.5 -> 2`, `3.5 -> 4`.
  - Được dùng trong `ShopService.cs:280` (tính giá vật phẩm sau hệ số loại đồ) và `PowerCatalog.cs:146` (tính Lực Chiến).
- **Trong Java:** `Math.round(double a)` thực hiện `(long)Math.floor(a + 0.5d)` (làm tròn lên phía dương):
  - Ví dụ: `Math.round(2.5) -> 3`. **Sẽ gây lệch giá shop và Lực Chiến giữa Client C# và Server Java!**
- **Giải pháp trên Java:** Để tái hiện chuẩn xác `MidpointRounding.ToEven` của C#, Java phải dùng:
  ```java
  BigDecimal.valueOf(val).setScale(0, RoundingMode.HALF_EVEN).longValue();
  ```
- **Làm tròn `MidpointRounding.AwayFromZero`:**
  - Dùng trong `PveRewardService.cs:96` (tính Bạc và EXP thưởng đánh lại).
  - Phía Java tương ứng với:
    ```java
    BigDecimal.valueOf(exact).setScale(0, RoundingMode.HALF_UP).longValue();
    ```

### 16.2. Phép dịch bit và số nguyên không dấu (`unsigned`)
- Trong C#: `uint state` dịch phải `(state >> 16)` giữ nguyên không dấu.
- Trong Java: Không có kiểu `uint`. Nếu dùng `int` có dấu, bắt buộc phải dùng **toán tử dịch phải không dấu `>>>`**:
  ```java
  int roll = (state >>> 16) % 100;
  ```
  Nếu viết nhầm thành `>> 16`, khi bit dấu là 1, kết quả `% 100` sẽ ra số âm, phá hủy hoàn toàn logic rơi nguyên liệu PvE!

### 16.3. Thứ tự lặp của Bảng Tra (`Dictionary` vs `HashMap`)
- Trong C#, `Dictionary<K, V>` lưu trữ theo mảng bucket và giữ thứ tự thêm vào (nếu không có xóa).
- Trong Java, `HashMap` **hoàn toàn không giữ thứ tự**.
- **Điểm chí mạng:** Trong `PveRewardService.cs:76`, C# đã chủ động gọi `materialIds.Sort(StringComparer.Ordinal);` trước khi chạy vòng lặp PRNG. Java server **bắt buộc** phải sort danh sách ID nguyên liệu theo mã ký tự ASCII (`Collections.sort(materialIds)`) trước khi quay thưởng đánh lại, nếu không chuỗi RNG sẽ bị sai lệch hoàn toàn.

### 16.4. So sánh chuỗi (`StringComparison.Ordinal`)
- Trong C#, `string.Compare(a, b, StringComparison.Ordinal)` so sánh trực tiếp byte UTF-16.
- Trong Java, `a.compareTo(b)` cũng so sánh theo mã ký tự char (Unicode code point), tương thích với Ordinal. Tuyệt đối không dùng `Collator.getInstance()` vì sẽ phụ thuộc vào Locale của hệ điều hành server.

### 16.5. Thuật toán PRNG của Cửa Hàng Thương Hội
- Trong `ShopService.cs:331`:
  ```csharp
  private Random Rng(int salt) => new Random(unchecked(seed * 486187739 + salt));
  ```
  Lớp `System.Random` của C# dùng thuật toán Subtractive Random Number Generator (Knuth), trong khi `java.util.Random` dùng 48-bit Linear Congruential Formula.
- Tuy nhiên, theo mục 179 của [`protocol.md`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/docs/tech/protocol.md#L179): *"Ngẫu nhiên (Thương Hội, rơi đồ, seed trận PvE) do server bốc bằng RNG của nó. Client không cần tính trùng."* Do đó Server Java có thể tự sinh ô hàng bằng RNG nội bộ của server mà không cần mô phỏng thuật toán nội bộ của class `System.Random`.

### 16.6. Phép cộng kiểm tra tràn số (`checked`)
- C# dùng `checked(a + b)` trong `LevelService.cs:103`, `StorageGrid.cs:312`, `MergeService.cs:236` để quăng `OverflowException`.
- Java mặc định tràn số nguyên sẽ tự động quay vòng (wrap-around). Trên Java nên dùng `Math.addExact(a, b)` nếu muốn đảm bảo chặn tràn số.

---

## 17. Chỗ Luật C# Có Vẻ Là Lỗi

Các điểm dưới đây được phát hiện trong quá trình trích xuất mã nguồn C#. **Chỉ nêu để đội ngũ nắm bắt rủi ro, không tự ý sửa code:**

1. **Gán nhầm Trọng khi Hợp Thành mà bản sao Candidate đang đeo trên người ([`MergeService.cs:135-147`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L135-L147)):**
   - Đoạn code:
     ```csharp
     OwnedItem equippedResult = IsEquipped(profile, firstUid) ? first : IsEquipped(profile, secondUid) ? second : null;
     if (equippedResult != null)
     {
         OwnedItem consumed = ReferenceEquals(equippedResult, first) ? second : first;
         ...
         equippedResult.Rarity = preview.OutputRarity;
         equippedResult.Tier = preview.OutputTier; // OutputTier lấy từ first.Tier!
     ```
   - **Hiện tượng:** Nếu món thứ nhất (`firstUid` - món chính) ở trong kho có Trọng 8, còn món thứ hai (`secondUid` - món làm nguyên liệu) đang đeo trên người có Trọng 3:
     `equippedResult` sẽ nhận giá trị là `second`. Sau đó dòng 147 gán `second.Tier = preview.OutputTier` (là 8). Món `second` đang nằm trên người bị biến thành món kết quả có Trọng 8, còn món `first` trong kho bị xóa. Dù kết quả chỉ số cuối cùng vẫn đúng, nhưng bản chất món đồ trên người đã bị tráo đổi định danh.
2. **Không nhất quán UID khi Hợp Thành đồ trong kho vs đồ trên người ([`MergeService.cs:146, 169`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/MergeService.cs#L146)):**
   - Khi Hợp Thành 2 món trong kho, code gọi `profile.AddItem(...)` tạo ra một vật phẩm mới toanh với UID ngẫu nhiên mới.
   - Nhưng khi có 1 món đang đeo trên người, code lại giữ nguyên UID cũ của món đang đeo.
3. **Bán đồ bỏ qua kiểm tra đầy kho khi hoàn trả nguyên liệu ([`SellService.cs:75-76`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/SellService.cs#L75-L76)):**
   - `SellService.Apply` gọi `storage.TryAddMaterial(value.Key, value.Value);` nhưng hoàn toàn không kiểm tra giá trị trả về (`bool`). Nếu kho Tàng Bảo Các hết ô trống, lượng nguyên liệu hoàn trả này sẽ bị mất trắng mà không được lưu vào `Overflow`.
4. **Mua Tiệm Danh Vọng văng ngoại lệ `InvalidOperationException` thay vì trả lỗi nghiệp vụ ([`ShopService.cs:224`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L224)):**
   - `BuyReputation` gọi `profile.AddMaterial(...)`. Hàm này trong `PlayerProfile.cs:202` ném thẳng ngoại lệ `throw new InvalidOperationException("Tàng Bảo Các không đủ chỗ.")` làm sập luồng nếu kho đầy, thay vì trả về `EconomyResult.Fail` nhẹ nhàng như hàm mua thường.
5. **Lệch giá Bạc làm mới Thương Hội giữa tài liệu và code ([`shop.json:4`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json#L4)):**
   - Bản kiểm kê trước ghi phí làm mới là 200 Bạc, nhưng file dữ liệu thật `Resources/Data/shop.json` đang đặt giá trị là `50`. Server Java phải đọc đúng file JSON (50 Bạc).
6. **Lệch múi giờ reset giữa Thương Hội và Luận Kiếm Đài ([`ShopService.cs:345, 352`](file:///d:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Economy/ShopService.cs#L345)):**
   - `ShopService` của client đang gọi `clock.UtcNow` (UTC+0), trong khi Luận Kiếm Đài trên server (`ArenaCalendar.java`) chạy theo giờ Việt Nam `Asia/Ho_Chi_Minh` (UTC+7). Khi chuyển quyền lên server, server cần chuẩn hóa tất cả các mốc reset ngày/tuần về cùng múi giờ UTC+7.
