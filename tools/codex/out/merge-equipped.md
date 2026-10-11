Implemented both requested changes.

Files changed:

- `Scripts/Economy/MergeService.cs`
- `Scripts/UI/Popups/StoragePopup.cs`
- `Scripts/UI/Popups/StorageItemView.cs`
- `Scripts/Bag/View/BagViewStyle.cs`
- `Editor/Building/EconomyPopupPrefabBuilder.cs`
- `Editor/Building/BagPrefabBuilder.cs`
- `Tests/EditMode/Economy/EconomyServicesTests.cs`

Loadout behavior:

- Selected equipped copy keeps its UID, position, and becomes the merged result.
- If only the second copy is equipped, its UID and position are retained.
- If both are equipped, the selected copy remains and the second loadout slot is freed.
- Equipped results require no storage cell.
- Storage placements and consumed loadout entries are removed without dangling references.
- Because the retained UID now points to the upgraded item data, server battle-loadout serialization receives the merged item.

Tests added/updated:

- Equipped candidates are accepted.
- Equipped + storage.
- Storage + equipped.
- Equipped + equipped, with one slot freed.
- Full storage with an equipped source.
- Assertions verify result UID, loadout coordinates, rarity, and consumed-copy removal.

Visual changes:

- Storage items receive an inset, fully opaque paper backing tinted 18% toward rarity.
- Bao Phục items receive the same solid backing.

Validation:

- Style checker: `0 problem(s)` across all seven changed files.
- All generated `.csproj` builds succeeded.
- Unity was not launched and no serialized prefab/scene files were touched.

Run these Unity menus to regenerate the affected prefabs:

1. `One Bag Jianghu > Build > Economy Popup Prefabs (Overwrite)`
2. `One Bag Jianghu > Build > Bag Prefabs (Overwrite)`