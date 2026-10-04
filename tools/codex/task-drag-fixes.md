# Codex task DR: item drag fixes in Bao Phục and Tàng Bảo Các

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read `docs/decisions.md` #27 (items in the bag are never in storage), then the drag code:
- `Scripts/Bag/View/ItemView.cs` (`OnBeginDrag`, `Follow`, `OnEndDrag`), `BagGridView.cs`, `ItemTrayView.cs`, `UI/Common/BagLoadoutPanel.cs`, `UI/Popups/BagPopup.cs`;
- `UI/Popups/StoragePopup.cs`, `StorageItemView.cs`, `StorageCell.cs`, `Economy/StorageGrid.cs`.

## Bugs reported by the user
1. **Dropping an item outside the grid loses it.** In both Bao Phục (bag grid and tray) and Tàng Bảo Các, releasing a dragged item anywhere that is not a valid target makes the item disappear.
   - Required behaviour: a drop on an invalid spot **cancels** the drag. The item goes back exactly where it came from: same grid cell, or same tray / storage slot. Nothing is removed from the profile, the bag or the storage.
   - Valid moves stay as they are:
     - bag cell → bag cell;
     - tray → bag;
     - bag → tray (the item returns to storage, #27);
     - storage → storage.
   - Also cover:
     - a drop over a non-grid UI element;
     - a drop outside the window;
     - a drop while a popup closes;
     - a drag cancelled by `OnEndDrag` never firing (for example the object is disabled mid-drag). Make sure there is no orphan dragged object left on the canvas, and no item missing from the data.
   - Find the real cause (for example the `Destroy(gameObject)` path in `ItemView.OnEndDrag` for placed items, or a storage remove-before-place) and fix it there. State the cause in the result.
2. **Grab at the centre.** When the user presses on an item and drags, the item must be centred on the cursor, not offset by where it was grabbed.
   - The dragged visual is centred on the pointer.
   - Placement uses the item's centre: the target cell is computed from the pointer as the item's centre cell. For even sizes, use the cell left of / above the centre consistently.
   - The highlight preview (`ShowTarget`) matches exactly where the item will land.
   - Apply the same behaviour in Tàng Bảo Các.

## Rules
- Edit `Assets/_Game/Scripts/**` (not `Scripts/Engine/**`), `Assets/_Game/Editor/**` (builders only, if needed) and `Assets/_Game/Tests/**`. **No git.**

## Tests
- EditMode or PlayMode tests:
  - Simulate drops on invalid positions for a placed bag item, a tray item and a storage item. After each drop, the profile (`Loadout`, `Storage.Items`, `Items`) is unchanged and the views show the item in its original place.
  - A valid move still works.
  - The centre-grab target-cell maths for 1×1, 1×2, 2×2 and 1×3 items.
- Keep the existing tests green. Claude runs Unity afterwards.

## Result
The root cause of each bug, the fix, the changed files, and anything you could not verify without hands-on play.
