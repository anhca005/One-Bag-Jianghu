# Codex task UF2b: restore Tàng Bảo Các features lost in the UF2 rewrite

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. **Follow `docs/tech/code-style.md`.**

## Problem
UF2 reformatted and rewrote `Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs` (+522/−113 lines). In doing so it **dropped features** that earlier tasks (ST–ST4) had added to the StoragePopup (Tàng Bảo Các). The PlayMode test `TextOverflowTests.RealItemTextFitsStorageBagAndShop` now fails:
- "detail overlaps upgradeButton / mergeButton / sellButton / closeButton" for many items;
- `StoragePopup(Clone)/Panel/Cost` has height 166 > 117.

Example of a lost feature. In the committed version (`git show HEAD:One-Bag-Jianghu/Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs`, read-only use of git is allowed), the detail text lives in:
- a `Detail Viewport` with `RectMask2D` and a `ScrollRect`;
- a top-anchored `Content` with `VerticalLayoutGroup` + `ContentSizeFitter`;
- a `Detail Scrollbar`;
- rich text and wrapping.

The cost text uses best fit 16–20. The current version is just a plain `Text` anchored over the area.

## Do
1. Compare the **committed** builder with the current one, section by section. Cover every popup and card it builds: sect select, storage, storage item view, shop, shop card, exchange, upgrade/merge, and anything else.
2. For every component, setting or child object that existed in HEAD and is now missing or different, **restore it**. Keep the **UF2 changes** on top:
   - icon/value currency pairs;
   - tab primary/secondary styling;
   - common `Dropdown.prefab` instances;
   - exchange labels "Đưa vào" / "Nhận về" and the full-width status line;
   - reputation card icon lookup and placeholder;
   - the disabled-reason line above the buy button.

   Check the serialized-field wiring too (`UiBuild.Set(...)` calls). Every field set in HEAD must still be set.
3. List every restored item in the result. If a HEAD feature was deliberately replaced by an equivalent UF2 change, say so and do not restore it.

## Your files (only these)
- `Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs`
- `Assets/_Game/Scripts/UI/Popups/StoragePopup.cs`, `ShopPopup.cs`, `ShopCard.cs`, only if wiring requires it

## Check
- `py tools/lint/check_style.py <changed files>` → 0 problems for runtime scripts.
  - The builder must have **no new** problems in the code you write; legacy lines may remain.
  - No line may start at column 0 inside the namespace (`tools/lint/fix_continuation_indent.py --check`).
- C# compiles.
- No git writes. Do not run Unity; Claude rebuilds and runs `TextOverflowTests`.

## Result
Restored items (HEAD → now), items intentionally not restored and why, files changed.
