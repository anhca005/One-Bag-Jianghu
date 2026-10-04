# Codex task W2: hover tooltip in Bao Phục

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. This task follows task W (`tools/codex/task-bag-storage-grid-desc.md`), which added `ItemDescriber` and the 10×20 storage grid in Bao Phục. Read that task and its code first.

## Rules
Same as task W: edit only `Assets/_Game/Scripts/UI/**`, `Scripts/Bag/**`, `Editor/Building/**`, `Tests/**`. `Scripts/Engine/**` is read-only. Change the builders, not the prefabs by hand. **No git commands.**

## Request (from the user)
In Bao Phục, when the mouse hovers over an item, show a small popup with that item's description. **Text only**: no icon and no buttons.
- It works for items in the bag grid and in the storage grid. For material stacks, show the material description.
- The text comes from `ItemDescriber`, for the item's actual rarity and Trọng.
- It appears after a short delay (about 0.3 s) and hides on pointer exit, while dragging, and when the popup closes.
- It is a lightweight overlay, not a `PopupManager` popup, so it must not change the popup stack or dim the background. It must draw above the Bao Phục content (its own canvas with `overrideSorting`, above the popup's sorting order), and `raycastTarget = false` so it never blocks clicks or drags.
- Use the pale paper background (`frame_tooltip` sprite if it is available, otherwise a plain panel) with dark text. Width is about 360 px, and the height fits the content (ContentSizeFitter).
- Position it next to the cursor and keep it inside the screen: flip left/up near the edges.
- If task W made clicking an item show the description in a separate panel, keep hover as the main way and remove any duplicate panel that no longer makes sense. Say what you did in the result.
- Make it a reusable component (for example `UI/Common/ItemTooltip`) so Tàng Bảo Các and the battle screen can use it later.

## Tests
- PlayMode: open Bao Phục, send pointer-enter to an item view (via `ExecuteEvents`), wait past the delay, and assert the tooltip is active and its text contains the item name. Send pointer-exit and assert it hides. Start a drag and assert it hides.
- The tooltip canvas sorting order is greater than the Bao Phục popup canvas sorting order.

## Self-check
Close Unity first, then Build All Overwrite, EditMode and PlayMode as in task W. All tests must pass.

## Also finish from task W (it could not run Unity)
Task W did not add its PlayMode tests. Add them now:
- Open Bao Phục from Home. Move an item from storage into the bag (call the same handler a drag uses), save, and assert it is in the loadout and not in `profile.Storage`.
- Move it back, save, and assert the reverse.
- Assert that the storage grid RectTransform is at least 400 px wide and shows one view per storage item.
- Open Tàng Bảo Các, select a weapon, and assert the detail text contains "Lực Đạo" and "Phổ Công".

Note: Claude will run Unity (Build All + EditMode + PlayMode) after you finish. If your sandbox cannot run Unity, still write the tests and say which ones are untested.
