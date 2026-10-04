# Task R: Tàng Bảo Các as a spatial 10×20 grid (items at real size) ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Read `docs/decisions.md` **#27**, #24, #25, `Scripts/Bag/**` (bag grid placement, drag & drop — reuse this logic, do not write a second grid engine), `Scripts/Engine/Grid.cs`, `Scripts/UI/Popups/{StoragePopup,BagPopup,ShopPopup,ShopCard,DevCheatPanel}.cs`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Scripts/Economy/{PlayerProfile,ShopService,RewardService,MergeService,SellService}.cs`, `Scripts/Core/GameSession.cs`, `Editor/Building/{EconomyPopupPrefabBuilder,BagPrefabBuilder}.cs`, tests in `Tests/EditMode/{Economy,UI,Bag}` and `Tests/PlayMode/*`.

## Rules (decision #27)
- Storage is a **10 columns × 20 rows spatial grid** (200 cells), capacity stored in the profile (`storage.cols = 10`, `storage.rows = 20`) so it can grow later.
- **Every item copy occupies its real size** (`ItemDef.size`, same as in the bag; no rotation — decision #21). **Materials occupy cells too**: one 1×1 cell per material stack, max 999 per stack, overflow opens a new stack cell.
- Placement is saved in the profile: `storage.items` (uid → x, y) and `storage.materialStacks` (list of {materialId, count, x, y}). Profile version +1 with migration: existing items not in the loadout and all material counts are auto-placed (first-fit, row-major, top-left first); if something does not fit, keep it in an `overflow` list and show a warning in the popup (never delete data).
- **Items in the Bao Phục (loadout) are not in the storage grid.** Moving an item from the bag back to storage needs a free spot of its size (auto-place first-fit); no space → refuse with "Tàng Bảo Các không đủ chỗ.". Moving from storage to the bag frees its cells.
- New items (shop, rewards, merge output, dev cheat) auto-place first-fit; no space → the action fails atomically with "Tàng Bảo Các không đủ chỗ." (shop Mua disabled with that reason for that card). Materials: add to existing stacks first, then new 1×1 stacks; no space for a needed new stack → fail with the same message. Spending materials removes empty stacks. Merge: inputs are removed before the output is placed. Sell frees cells.
- Material counts shown elsewhere (costs, cheat) must equal the sum of stacks — keep `PlayerProfile.Materials` as the source of truth only if you keep it strictly in sync; simplest is to derive counts from the stacks (your choice, explain).

## UI (StoragePopup)
- Left ≈ 62%: the 10×20 grid inside a vertical `ScrollRect` (cells ≈ 80 px, mask, scrollbar). Items drawn at their real size (icon preserveAspect, rarity-coloured border, Trọng badge; placeholder silhouette + short name without art); material stacks show icon (from `Textures2D/Materials` when present) + count. Empty cells faint paper squares. Header: "Tàng Bảo Các  {usedCells}/200", type filter (dims non-matching items instead of hiding, so the layout stays spatial) and a **Sắp xếp** button (repack: items by type → rarity → Trọng, then materials, first-fit).
- **Drag & drop** inside the grid to move items/stacks (valid drop only where it fits; invalid → snap back, red highlight while hovering).
- Click selects → right detail panel (≈ 38%): as today (stats, cost list, Nâng Trọng / Hợp Thành with candidate choice / Bán). For a material stack: name, count, description.
- Build cell/item views as prefabs via the builder (`StorageCell.prefab`, `StorageItemView.prefab`); create the 200 cells once; re-bind item views on refresh.
- **BagPopup**: its tray = the items currently in storage (not in the loadout); placing into the bag removes them from the storage grid; removing from the bag re-places into storage (or refuses when full). Keep BagDemo's full-catalog developer mode unchanged.

## Hard rules
- Modify only: `Scripts/Economy/**`, `Scripts/Core/GameSession.cs`, `Scripts/Bag/**` (only additions/refactors that keep the bag behaviour), `Scripts/UI/Popups/**`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Editor/Building/{EconomyPopupPrefabBuilder,BagPrefabBuilder,PopupPrefabBuilder}.cs`, `Tests/EditMode/{Economy,UI,Bag}/**` (not Gameplay*), `Tests/PlayMode/**`. Do not touch the battle engine, replay or scenes' layout.
- UI from builders only; popups initialise lazily (PopupManager calls Setup before Awake). English code, Vietnamese UI with glossary terms. C# 9. No git. You cannot run Unity; re-read for compile errors (CS0165; fully qualify `UnityEngine.UI.Image.Type` in builders; test asmdefs reference every assembly they use).

## Tests
EditMode: first-fit placement by size, no rotation, material stacking/overflow at 999, spending removes empty stacks, migration auto-place + overflow list, shop buy / reward / cheat fail atomically when no space, merge places output after removing inputs, bag ↔ storage moves (refuse when no space), drag move validity. Prefab: 200 cells, ScrollRect vertical only, refs assigned. PlayMode: adapt StorageSmokeTests and the shop/bag flows; all green.

Finish with: files changed, builder menu items Claude must run (order), anything unsure.
