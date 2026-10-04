# Codex task ST2: fix what the ST check and the screenshots found

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. This continues task ST (`tools/codex/task-storage-text-drag.md`); same rules:
- edit only `Scripts/**` (not `Scripts/Engine/**`), `Editor/**` and `Tests/**`;
- builders only;
- **no git; do not run Unity.**

## Claude ran your tests (PlayMode, real Unity)
Claude made two changes first:
- **The hang:** `RealItemTextFitsStorageBagAndShop` hung forever, because `WaitForEndOfFrame` never resumes in `-batchmode`. Claude rewrote `TextOverflowTests.Capture`: overlay canvases are switched to a camera that renders into a RenderTexture. Keep that approach.
- **A compile error:** `DragOutsideGridTests` used an `out` parameter inside a lambda. Claude fixed it.

Results: **2/5 pass.** The screenshots are `Logs/Snapshots/StoragePopup_filled.png` and `Logs/Snapshots/Tooltip_filled.png` (1920×1080). Open and look at them.

### A. Real drag bugs (fix in the game code)
1. **`InterruptedDragRestoresAndDraggedImageIsCentredOnPointer`:** "A drag ghost survived popup disable", expected 0 `ItemView`s after the close but found 1.
2. **`StorageDropOutsideRestoresPlacementAndCount`:** `[Error] Cannot set the parent of the GameObject 'thanh_phong_kiem' while activating or deactivating the parent GameObject 'BagPopup(Clone)'`. The stack is `ItemView.OnDisable` → `RestoreOrigin` (ItemView.cs:79/84), called from `BasePopup.Hide` ← `PopupManager.CloseAll` ← `SceneFlow.TransitionTo`.
   - You cannot reparent during deactivation. Defer the restore, for example by marking it pending and restoring in `OnEnable`/next frame, or by destroying the drag proxy instead of moving the real view.
   - Make sure no error is logged on any popup close or scene change while dragging, or while not dragging.

### B. Overflows your check reported (unique elements)

| Element | Measured | Count |
|---|---|---|
| `StoragePopup/.../StorageItemView/Name` | preferred width up to 77.9 > 69.9, and height 20.8 > 19.8 | every item |
| `StoragePopup/.../StorageItemView/Badge` | height 20.8 > 19.8 | every item |
| `StoragePopup/Panel_Paper/Cost` | height up to 433.9 > 137.2 | 64 items |
| `ShopPopup/.../Shop Card Prefab/Name` | height 36.4 > 30 | — |
| `ShopPopup/.../Shop Card Prefab/Detail` | height 246.8 > 33 | — |
| `ShopPopup/.../Shop Card Prefab/Buy/Label` | height 31.2 > 24.5 | — |
| `BagLoadoutPanel/ItemTray/.../<item>/Name` | width up to 114.3 > 44 | every item |

The fitter clearly does not shrink enough. Either its minimum size is too large for these boxes, or the label boxes are too small for a readable font. Fix the layout so the text is readable **and** fits.

### C. Visible problems in the screenshots that the check missed
1. **Storage detail panel:**
   - The description is cut after the first ability. "Tuyệt Kỹ" and the triggers are not visible, and there is no visible scrollbar or cue. Give the description most of the panel height, with a visible scrollbar.
   - The **Cost** block ("Nâng lên 2 Trọng", "Bạc 1,000/100", "Hàn Thiết 0/3", …) is rendered at a huge font (~40 px) and runs underneath the "Nâng Trọng"/"Hợp Thành" buttons, where a line is cut in half. Make it one compact block at about 20 px, above the buttons, in a fixed area that never overlaps them. Show insufficient amounts in red.
2. **Raw identifier:** "Binh Khí · bow". The weapon class is shown as the raw id. Localize every `weaponClass` (sword → Kiếm, bow → Cung, …; check the glossary `docs/design/glossary.md` and the sheet exporter `tools/sheets/export_xlsx.py` for the names). Extend the describer test: no ASCII-only lowercase word may appear in any description.
3. **Rarity "Phàm"** in the detail and the tooltip is drawn in near-white on paper and is unreadable. Use a darker variant of each rarity colour for text on paper (keep the frame colours).
4. **Storage header** "Bạc:1,000 Danh Vọng:0":
   - the currency icons overlap the numbers;
   - a stray silver icon floats at about (1180, 100);
   - there is no spacing.

   Lay it out as icon + number pairs with gaps, using a `HorizontalLayoutGroup`.
5. **Storage cells:**
   - the item name at the bottom is tiny and unreadable, and it overlaps the rarity frame;
   - the "N Trọng" badge overlaps the frame corner;
   - on 1-column-wide items the name is a smear.

   Proposal:
   - show only a short "T3" badge inside the frame's inner area;
   - drop the name label from the cell, since the name is in the detail and the tooltip, or show it only when the cell is at least 2 cells wide;
   - for items with no icon, show the name centred, wrapped, inside the inner area.
6. **Bao Phục tray** (right side): the same issues as item 5, with names overlapping icons ("Cương Trư Bì" in a large font, others overlapping). Apply the same rules as the storage cells.
7. **Tooltip:**
   - it was placed partly off-screen (top edge cut, over the HUD) and its text touches the paper edges with no padding;
   - the red seal ornament overlaps "Công";
   - a scrolling tooltip cannot be scrolled, because the mouse is on the item.

   Instead:
   - size the tooltip to its content, up to ~70% of the screen height;
   - use 16 px padding;
   - clamp it fully on screen, preferring the right of the pointer and flipping left or up when needed;
   - if the text is still too tall, reduce the font to a minimum of 16 before truncating.
8. **HUD top bar** (behind both popups):
   - the currency icons overlap their numbers ("Bạc 135,55" plus an icon over the digits, and the same for Nguyên Bảo);
   - the EXP bar text is cramped.

   Use icon + number pairs with a layout group. Add the HUD root to the overflow check.
9. **Wording:** Bao Phục stats now read "Hộ Thân: 0% (tối đa 80%)" and "Giảm Hồi Chiêu: 0% (≤ 40%)". Use one style for both: "(≤ 80%)" and "(≤ 40%)".

## Tests
- Keep both test files.
- Add the HUD to `CheckTexts`.
- Add a check that every `Text` in the storage detail/cost area does not overlap the action buttons: rect intersection in screen space.
- Make all 5 tests pass in your reasoning. Claude will run them and look at the new screenshots.

## Result
For each item in A–C: what you changed, with file names.
