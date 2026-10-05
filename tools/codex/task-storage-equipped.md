# Codex task: Tàng Bảo Các shows equipped items too, so they can be upgraded

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/UI/Popups/StoragePopup.cs` (selection, Nâng Trọng, Hợp Thành);
- `Scripts/UI/Popups/StorageItemView.cs`;
- `Scripts/Economy/` `UpgradeService`, `MergeService`, `PlayerProfile` (Loadout and Storage);
- the storage part of `Editor/Building/EconomyPopupPrefabBuilder.cs`.

## User request
- **Today:** Tàng Bảo Các only shows items in storage. To upgrade an item that is in the Bao Phục, the player must take
  it out first.
- **Want:** also show the **equipped items** in Tàng Bảo Các so they can be upgraded directly.

## Do
1. **Panel:**
   - Add an **"Đang trang bị"** panel or strip, laid out as a small read-only copy of the bag grid, in the free space
     of the storage popup. Look at the current layout first and pick the cleanest spot. If there is no room, a
     horizontal row of item cards also works.
   - Show every loadout item with its art, Trọng and rarity badge, the same as storage items.
   - Equipped items cannot be dragged here. The bag is arranged in Bao Phục.
2. **Selection:**
   - Clicking an equipped item selects it like a storage item: detail text, upgrade preview, and the Nâng Trọng button.
   - Upgrading an equipped item works exactly like a storage item. The uid stays the same, so the bag keeps it and its
     new Trọng.
   - After upgrading, refresh both views and the Lực Chiến shown on Home, as the existing flow already does for storage.
3. **Hợp Thành with equipped items:**
   - Do not consume an equipped item.
   - If the selected item is equipped, disable Hợp Thành and show the reason "Gỡ khỏi Bao Phục để Hợp Thành".
   - If a storage item's merge candidates include equipped copies, skip them.
   - Keep the existing tutorial target ids (`storage_upgrade`, `storage_merge`) working.
4. **Builder:** add the new panel through `EconomyPopupPrefabBuilder` so prefab rebuilds keep it. Touch only the storage
   popup part.

## Tests (EditMode)
- Upgrading an item that is in the loadout raises its tier and keeps it in the loadout. This is a service-level test if
  the popup is hard to test.
- Merge candidates exclude equipped uids.

## Rules
- **No git.** Never touch `LoginScene` or `BuildLoginScene`.
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; changed lines must be clean.
- **Another Codex task is editing the encyclopedia popup at the same time.** Do not touch `EncyclopediaPopup.cs` or
  `BuildEncyclopedia` in `PopupPrefabBuilder.cs`.

## Report (`-o` output)
- The files changed.
- Where the panel sits.
- How Hợp Thành treats equipped items.
- The lint result.
