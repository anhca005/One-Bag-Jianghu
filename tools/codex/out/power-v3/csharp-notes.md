# PowerCatalog.cs — thay đổi chính xác cho v3

Giữ nguyên nhánh đọc/tính v1. Nhánh hiện dùng cho v2 cũng dùng được cho v3 vì schema tính điểm giống nhau.

1. Cho phép `version == 3` tại chỗ kiểm tra phiên bản; không đổi fallback v1.
2. DTO gốc cần các trường sau (tên JSON giữ nguyên):
   - `int version`
   - `float recommendedFactor`
   - `Dictionary<string, float> tierMultiplier`
   - `Dictionary<string, float> rarityMultiplier`
   - `float levelBaseDefense`
   - `float levelDefensePerLevel`
   - `float scale`
   - `Dictionary<string, PowerItemV2> items`
3. `PowerItemV2` tiếp tục chứa đúng hai `float`: `offense`, `defense`; có thể đổi tên thành `PowerItemV3`
   nếu đồng thời cập nhật kiểu của dictionary. Không thêm bảng hay số riêng cho từng loại vật phẩm.
4. Với v2/v3, tính:

```csharp
float offense = 0f;
float defense = data.levelBaseDefense
    + data.levelDefensePerLevel * (Mathf.Max(1, fighter.Level) - 1);

foreach (PlacedItem item in fighter.Items)
{
    float multiplier = data.tierMultiplier[item.Tier.ToString(CultureInfo.InvariantCulture)];
    multiplier *= data.rarityMultiplier[item.Rarity.ToString().ToLowerInvariant()];
    PowerItemV2 measured = data.items[item.Id];
    offense += measured.offense * multiplier;
    defense += measured.defense * multiplier;
}

return Mathf.RoundToInt(data.scale * Mathf.Sqrt(Mathf.Max(0f, offense * defense)));
```

5. Tooltip của một món vẫn phải tính theo đúng hàm catalog hiện có. Không cộng `levelBaseDefense` vào giá trị
   `offense`/`defense` của món và không áp hệ số phẩm/Trọng lần thứ hai.
6. `recommendedFactor` vẫn là 1,1 và chỉ dùng cho Lực Chiến đề nghị; logistic `k` là dữ liệu kiểm định,
   không cần đưa vào runtime/UI.

