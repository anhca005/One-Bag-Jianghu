# Codex task: hover tooltips on Thương Hội cards + card text overlap fix

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity; Claude builds, tests and takes snapshots.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/UI/Popups/ShopPopup.cs`, `Scripts/UI/Popups/ShopCard.cs`;
- `Editor/Building/EconomyPopupPrefabBuilder.cs` (`BuildShopCard`, `BindCard`, and the shop popup build);
- the existing tooltip pieces:
  - `Scripts/UI/Common/ItemTooltip.cs`: `ShowDelayed`, `Hide`, `EnsureAbove`, `WithPower`;
  - `Scripts/UI/Common/ItemDescriber.cs`: `Describe(id, rarity, tier)`, `DescribeMaterial(info, count)`;
  - `Scripts/Bag/View/ItemTrayView.cs`: `MaterialTooltipTarget`, an example of hover → tooltip;
  - `Scripts/UI/Common/BagLoadoutPanel.cs` around line 88: how an item description gets "Lực Chiến +N";
  - how `StoragePopup` obtains its `ItemTooltip` instance. Reuse the same approach.

## 1. Hover tooltip on every shop card
In all three tabs (Thương Hội, Đổi nguyên liệu, Tiệm Danh Vọng), hovering a card shows the shared `ItemTooltip` with full details:
- **Item offers:**
  - `ItemDescriber.Describe(id, rarity, tier)`, passed through `ItemTooltip.WithPower(...)` so it shows "Lực Chiến +N";
  - use the offer's actual tier and rarity.
- **Material offers** (`×5` cards, such as Hoàng Tuyền Chỉ, Tử Linh Chi, Chu Sa Thảo): `ItemDescriber.DescribeMaterial(info, count)`, with the offer count.
- **Reputation-shop chests or other offer kinds:** a short description with name, type and what it contains, if the data has it. Otherwise name and type only.

**Behaviour**
- Exit hides the tooltip.
- Refresh, purchase and tab switch also hide it, so no stale tooltip stays on screen.
- The tooltip must render above the popup (`EnsureAbove`).
- Hovering the **Mua** button still counts as the card, or at least must not break the tooltip.

**Implementation:** prefer a small reusable component, for example `HoverTooltipTarget : IPointerEnterHandler, IPointerExitHandler` with `Initialize(ItemTooltip, Func<string> text)`, in `Scripts/UI/Common/`.
- `ShopCard` exposes `SetTooltip(ItemTooltip, Func<string>)`.
- `ShopPopup` sets it when it fills each card.
- You may refactor `MaterialTooltipTarget` to use the new component **only** if that is trivial and safe. Otherwise leave it.

## 2. Fix the card text overlap (see the user's screenshot)
On item cards the stat lines overlap the price label. For example, "Phàm • 1 Trọng" / "Khí Huyết +20" sits on top of "Bạc 200", and "Lương • 1 Trọng" / "Giảm Hồi Chiêu +5.5%" overlaps "Bạc 500".
- Lay the card out in non-overlapping bands, top to bottom:
  1. icon;
  2. name;
  3. a line with tier/rarity on the left and type on the right;
  4. one short stat line, single line, `SingleLineFit`, ellipsis allowed;
  5. a price row: price, strikethrough old price, discount %;
  6. a "Không đủ Bạc" reason line;
  7. the Mua button.
- Since the full details now live in the tooltip, the card may show **at most one** stat line. Drop the second line.
- Keep the existing serialized references working; rebuild through the builder.
- Material cards (`×5`) keep their count label, which must not collide with the price either.

## Tests (EditMode)
- **`HoverTooltipTarget`:** enter shows and exit hides. Use a fake or minimal tooltip if `ItemTooltip` is hard to instantiate; at least test that the text provider is called on enter.
- **`ShopCard`:** after `SetTooltip`, a target component exists.
- Update any existing shop UI tests that assert the old detail layout.

## Allowed files
- `Scripts/UI/Popups/ShopPopup.cs`, `Scripts/UI/Popups/ShopCard.cs`;
- new `Scripts/UI/Common/HoverTooltipTarget.cs`;
- `Scripts/Bag/View/ItemTrayView.cs`, only for the optional `MaterialTooltipTarget` reuse;
- `Editor/Building/EconomyPopupPrefabBuilder.cs`: the shop card and shop popup parts only. Do **not** touch the storage-popup parts; they were just restored after a regression;
- tests under `Tests/EditMode/`.

## Rules
- **No git.**
- Do not edit `.prefab` files by hand.
- UI text is Vietnamese; code is English.
- Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`. New and changed code must be clean; pre-existing builder debt is out of scope.

## Report (`-o` output)
- The files changed.
- How the tooltip instance is obtained.
- The lint result.
