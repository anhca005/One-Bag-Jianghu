# Codex task MP: one hand-editable prefab per chapter map (stage node positions)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Read first:
- `Scripts/UI/Pve/StageMapPopup.cs`, `PveStageNode.cs`;
- `Scripts/Pve/PveCatalog.cs` (`MapPos`), `Scripts/Pve/PveArtLibrary.cs`;
- `Editor/Building/PvePrefabBuilder.cs`, `Editor/Building/BuildMenu.cs`, `AssetGuard`;
- `Resources/Data/Pve/chapters.json` (`theme.mapBackground`, `stages[].mapPos`);
- the PvE PlayMode/EditMode tests.

**No git. Do not run Unity**; Claude runs it. Another Codex task is editing `DevCheatPanel`/`GameSession`/`SceneFlow` at the same time: do not touch those files.

## Problem
The stage nodes are placed from `mapPos` (normalized 0..1) in `chapters.json` on top of the map art. They do not sit on the painted road. The user wants **one prefab per chapter map** to move the nodes by hand in the Unity editor and see the result immediately.

## Do
1. **Map prefabs**: `Assets/_Game/Prefabs/UI/Pve/Maps/Map_<chapterId>.prefab` (`Map_hoi_1`, `Map_hoi_2`, `Map_hoi_3`), each containing:
   - the map background `Image` with the chapter's `mapBackground` sprite, at the **same size/aspect** the popup shows it;
   - a child `Nodes` with one empty `RectTransform` anchor per stage, named exactly the stage id (`hoi_1_01`…), at the current `mapPos`;
   - a small editor-only visual on each anchor so it is visible and draggable in Prefab Mode: a gizmo or a semi-transparent marker `Image` showing the stage index (and E/B for elite/boss), disabled at runtime or replaced by the real node;
   - optionally a `MapLayout` component listing the anchors, for validation.
2. **Protect manual edits:**
   - the builder creates a map prefab **only if it does not exist**, including in `BuildAllOverwrite`. Overwriting the user's node positions is the one thing that must never happen;
   - if a prefab exists but lacks an anchor for a stage (new stage added), add the missing anchor at its `mapPos` without moving the existing ones;
   - add a separate, explicit menu item **"One Bag Jianghu/PvE/Reset Map Prefabs from chapters.json (discards manual edits)"** with a confirmation dialog.
3. **Runtime (`StageMapPopup`)**:
   - for the open chapter, instantiate `Map_<chapterId>` (load via a serialized list in the popup or a `Resources` path) into the map area;
   - place each `PveStageNode` **exactly at its anchor**: parent it to the anchor, or copy the anchor's position;
   - fall back to `mapPos` only when the prefab or an anchor is missing, and log a warning once;
   - switching chapters swaps the map prefab;
   - keep everything else as it is: the selection, the locked/cleared states, the detail panel.
4. **Optional, small:** a menu item **"Write map node positions back to chapters.json"**. It converts the anchors to normalized `mapPos` and rewrites only those fields, so the data stays in sync for tools. Rewrite only the `mapPos` arrays and keep the JSON formatting and order. If that is risky, skip it and say so.

## Tests
- EditMode:
  - each chapter in `chapters.json` has a map prefab with exactly one anchor per stage id, and no extra or duplicate ids;
  - running the normal builder (`BuildAllOverwrite` path) on an existing map prefab does **not** change a moved anchor's position (move one in the test, rebuild, assert, restore).
- PlayMode: open the map for Hồi 1; each node's world position equals its anchor's world position (±1 px).

## Rules
- Edit only:
  - `Assets/_Game/Scripts/**`, except `Scripts/Engine/**`;
  - `Assets/_Game/Editor/**`;
  - `Assets/_Game/Tests/**`.
- The new prefabs are created by the builder (Claude runs it).
- No git.

## Result
- The changed files.
- How the user edits a map: which prefab to open, what to drag, what to avoid.
- What is protected from rebuilds.
