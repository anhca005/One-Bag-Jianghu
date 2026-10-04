# Codex task W: Bao Phục uses the 10×20 storage grid + item descriptions

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu` (Unity 6000.3.9f1, UGUI). Read `CLAUDE.md`, `docs/decisions.md` #27 and #29, and `docs/design/glossary.md` first.

## Rules
- You may edit/create files under `Assets/_Game/Scripts/UI/**`, `Assets/_Game/Scripts/Bag/**`, `Assets/_Game/Scripts/Economy/**`, `Assets/_Game/Editor/Building/**` and `Assets/_Game/Tests/**`.
- `Assets/_Game/Scripts/Engine/**` is read-only for you (sandbox). If you really need an engine change, describe it in your result instead.
- Do not edit data JSON, docs, or `tools/sheets/**` (another task is working there). **No git commands.**
- UI text is Vietnamese and uses the glossary terms. Code is English. Log with the existing `Debug.Log` tags (`[Bag]`, `[Storage]`).
- Generated prefabs and scenes come from the builders in `Editor/Building`: change the builders, not the prefab files by hand.

## Bug 1: Bao Phục shows the owned items squashed in a thin column
In `BagPopup` (prefab from `PopupPrefabBuilder`, content from `BagLoadoutPanel` + `ItemTrayView`), the right side is an `ItemTrayView`. The user's screenshot shows it about 20 px wide: the item icons are cut off and stacked vertically in a narrow strip.

The user wants the right side to be **the same 10×20 spatial grid as Tàng Bảo Các** (decision #27):
- Each owned item that is not in the bag is shown at its real storage position (`profile.Storage`, x/y) and its real size. Reuse `StorageGrid`, `StorageCell` and `StorageItemView` or their layout logic. Do not duplicate the grid math.
- The grid scrolls vertically. The cell size should match the bag cells if possible, otherwise use the largest cell size that fits 10 columns in the panel width.
- Material stacks also occupy storage cells. Show them dimmed and not draggable in Bao Phục, so the layout matches Tàng Bảo Các.
- **Drag from storage to bag:** the item leaves storage and its cells become free.
- **Drag from bag to storage:** the item is placed at the dropped cell if it fits, otherwise first-fit. If nothing fits, the item goes back to the bag and the popup shows "Tàng Bảo Các không đủ chỗ."
- **Lưu Bao Phục:** writes both the bag loadout and `profile.Storage` (items in the bag are not in storage). **Đóng** without saving discards the changes.
- Layout: bag on the left, storage grid on the right, both fully visible at 1920×1080 and at 1280×720, with no overlapping. Keep the Tâm Pháp / sect controls that `BagLoadoutPanel` already has.
- The developer mode of `BagLoadoutPanel` (`ItemSource.FullCatalogDeveloper`, used by the demo scene) may keep the old list tray. Only the profile-based Bao Phục must use the storage grid.

## Bug 2: selecting an item in Tàng Bảo Các shows no description
`StoragePopup.Select` only writes the name, rarity, Trọng, size and power %. The user expects a real item description.
- Create one shared formatter, for example `Scripts/UI/Common/ItemDescriber.cs`. It builds Vietnamese rich text from the engine item definition (read-only use of `Scripts/Engine` types, or the catalog JSON) for a given rarity and Trọng:
  - Type and weapon class.
  - For weapons: Lực Đạo, Tốc Xuất Chiêu (seconds) and Hội Tâm, **after** the Trọng × rarity multipliers (use `RarityRules` / the engine config; truncate like the engine).
  - Phổ Công and Tuyệt Kỹ (qi cost) effects.
  - Triggers ("Khi …: …").
  - Đột Phá milestones T3 / T6 / T9: unlocked ones normal, locked ones greyed with "(mở ở Trọng N)".
- Port the wording of `describe_effect` / `describe_trigger` / `describe_unlock` from `tools/sheets/export_xlsx.py` (read only), so the sheet and the game use the same phrasing. Status names must come from the glossary (Liệt Hỏa, Kịch Độc, Xuất Huyết, Cương Khí, …).
- Use it in the Tàng Bảo Các detail panel. The panel must be scrollable, or the text must auto-fit, so long weapon descriptions are fully readable. Keep the upgrade cost block and the buttons visible.
- For materials, show the material description from `materials.json` and what it is used for.
- In Bao Phục, clicking an item (in the bag or in storage) shows the same description in a small panel or tooltip.

## Tests (required)
- EditMode: `ItemDescriber` covers a weapon at Phàm 1 Trọng and at Chí Tôn 9 Trọng (numbers scaled), an item with triggers, and locked vs unlocked Đột Phá.
- PlayMode:
  - Open Bao Phục from Home.
  - Drag (or call the same handler as a drag) an item from storage into the bag, save, and assert the item is in the loadout and not in `profile.Storage`.
  - Move an item back from the bag to storage, save, and assert the reverse.
  - Assert that the storage grid RectTransform is at least 400 px wide.
  - Open Tàng Bảo Các, select a weapon, and assert the detail text contains "Lực Đạo" and "Phổ Công".
- Update any existing tests that depended on the old tray in Bao Phục.

## Self-check
Unity must be closed for batch runs. From the Unity project folder:
1. `"/c/Program Files/Unity/Hub/Editor/6000.3.9f1/Editor/Unity.exe" -batchmode -nographics -projectPath "D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu" -executeMethod OneBagJianghu.EditorTools.Building.BuildMenu.BuildAllOverwrite -quit -logFile Logs/BuildAll.log`
2. `-runTests -testPlatform EditMode -testResults Logs/EditMode.xml`, then the same with `PlayMode` → `Logs/PlayMode.xml`. All tests must pass.
3. If your sandbox cannot run Unity, say so in the result and list exactly what to run.
