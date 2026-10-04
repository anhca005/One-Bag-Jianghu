# Codex task: shop price multiplier per item type (weapons ×2)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity; Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/Economy/ShopCatalog.cs`, `Scripts/Economy/ShopService.cs` (price around line 249);
- `Resources/Data/shop.json`;
- the existing shop tests under `Tests/EditMode/Economy/`.

## What
The user decided that weapons in Thương Hội cost **2×** the rarity base price. Every other item type keeps the current price.

| Weapon rarity | Price |
|---|---|
| Phàm | 400 |
| Lương | 1.000 |
| Tinh | 2.400 |
| Tuyệt | 6.000 |
| Thần Binh | 16.000 |

**Data-driven:** add to `shop.json`
```json
"itemTypePriceMultipliers": { "weapon": 2.0 }
```
- Types not listed use 1.0.
- Type keys are the item `type` values from `items.json`: weapon, armor, manual, pill, relic.

**Code**
- `ShopCatalog`: parse the new field. If it is absent, use an empty map. Validate that every multiplier is > 0.
- `ShopService`: item price = `round(ItemBasePrices[rarity] × multiplier[itemType])`, then apply any existing slot discount exactly as now.
  - Find how the service knows the item's type: `ItemCatalog` or `ItemInfo`.
- Any UI that shows a price must read the computed price, not recompute from base prices. Grep `ItemBasePrices` across `Scripts/` and fix any direct use.

## Tests (EditMode)
- **Weapon common** offer price = 400; **armor common** = 200.
- **A discounted weapon** gets the discount applied after the multiplier.
- **Missing field** → old prices.
- **Catalog rejects a multiplier ≤ 0.**

## Allowed files
- `Resources/Data/shop.json`: add the one field only, with minimal text edits;
- `Scripts/Economy/ShopCatalog.cs`, `Scripts/Economy/ShopService.cs`;
- any `Scripts/**` file that uses `ItemBasePrices` directly;
- tests under `Tests/EditMode/`.

**Do not touch `tools/**`;** another job may be editing there.

## Rules
- **No git.**
- Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`; it must be clean.

## Report (`-o` output)
- The files changed.
- Where the item type comes from.
- The lint result.
