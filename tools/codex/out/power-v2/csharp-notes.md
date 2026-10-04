# PowerCatalog.cs — đọc power v2

- Rẽ nhánh bằng `version`; thiếu trường này nghĩa là v1.
- V2 đọc `tierMultiplier`, `rarityMultiplier`, `levelBaseDefense`,
  `levelDefensePerLevel`, `scale`; `items[id]` gồm `offense`, `defense`.
- Khởi tạo `O=0`, `D=levelBaseDefense + levelDefensePerLevel * (Max(1, level)-1)`.
- Mỗi món: `m=tierMultiplier[tier]*rarityMultiplier[rarity]`; cộng
  `offense*m` vào O và `defense*m` vào D.
- Kết quả: `Mathf.RoundToInt(scale * Mathf.Sqrt(O * D))`.
- Tooltip món phải tính chênh lệch LC của cả túi trước/sau; LC v2 không cộng được.
- `recommendedFactor` vẫn là 1.1; môn phái và Tâm Pháp không tham gia.
