# Codex task: allow Hợp Thành on equipped items + solid backing for storage items

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Paths below are under `Assets/_Game/`.

## 1. Merge (Hợp Thành) works on items that are in the Bao Phục
Today `Scripts/Economy/MergeService.cs` refuses when either copy is equipped (`IsEquipped` in `Candidates` and in the
validation: "Gỡ khỏi Bao Phục để Hợp Thành"). The designer decided to allow it:
- Either copy, or both, may be equipped. Other rules stay (same item, same rarity, both ≥ 3 Trọng, not supreme, silver
  cost, material refund).
- **Where the result goes:**
  - if the **selected (first) copy** is equipped, the result takes its place in the Bao Phục (same loadout position
    and rotation, same uid kept if the code keeps the first uid; otherwise update the loadout entry to the new uid);
  - else if only the second copy is equipped, the result takes the second copy's place in the Bao Phục;
  - else (neither equipped) as today: into the Tàng Bảo Các, failing with "Tàng Bảo Các không đủ chỗ." if no room.
  - The consumed copy is removed from wherever it was (storage grid or Bao Phục loadout), never left dangling.
  - A result placed in the Bao Phục must not need a storage cell (do not fail on a full Tàng Bảo Các in that case).
- Find how the Bao Phục loadout stores items (search the profile/loadout code for the `IsEquipped` implementation) and
  keep it consistent; the battle loadout sent to the server must contain the merged item afterwards.
- UI: `Scripts/UI/Popups/StoragePopup.cs` `BeginMerge` currently blocks equipped items; remove that block. Make sure an
  equipped item can be selected as the source and as the candidate from wherever equipped items are shown in the
  storage popup (the "Đang trang bị" row) as well as from the grid. Refresh both the grid and the equipped row after.
- EditMode tests in `Tests/EditMode/Economy/EconomyServicesTests.cs` (or a new file next to it): equipped+storage,
  storage+equipped, equipped+equipped (one slot freed), full Tàng Bảo Các with an equipped source still succeeds, and
  the loadout afterwards holds the result uid. Update any test that asserted the old refusal.

## 2. Storage items need a solid backing
In the Tàng Bảo Các grid, an item view is only a frame + icon, so on items bigger than one cell (e.g. a 1x3 sword) the
empty grid cells show through and it looks hollow. Builder: `Editor/Building/EconomyPopupPrefabBuilder.cs`
`BuildStorageItem` (prefab `Resources/UI/Popups/StorageItemView.prefab`), view `StorageItemView`.
- Add an opaque backing image behind the frame, inset a few px so it does not poke past the frame's corners, covering
  the whole item footprint: a warm paper colour lightly tinted by rarity (e.g. lerp the paper colour 15–20% toward
  the rarity colour from `BagViewStyle.Rarity`), set at runtime in the view when the item binds. Fully opaque.
- Check the Bao Phục item view (`Editor/Building/BagPrefabBuilder.cs`, `ItemView`) for the same hollow look; if it has
  it, give it the same backing.

## Check
- Record nothing in docs (the leader does). `py tools/lint/check_style.py <every C# file you changed>` until clean;
  `dotnet build` the generated `.csproj` files (a new file missing from a `.csproj`: build with a temporary copy that
  includes it, then delete the copy and its `obj/` caches).
- **Do not launch Unity.** No git. Do not touch LoginScene, the loading frame, `Map_hoi_1`, the replay builder, any
  `.prefab`/`.unity` file.

## Report (`-o` output)
Files changed, how the loadout is updated on merge, the tests added, the menu the user must run.
