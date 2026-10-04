# Codex task Q2: PvE art wiring (icons, frames, per-chapter art)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read `docs/design/pve.md`, `docs/art/prompts-pve.md` (the file names are the contract), `docs/decisions.md` #33–#34, and the PvE code (`Scripts/Pve/**`, `Scripts/UI/Pve/**`, `Editor/Building/PvePrefabBuilder.cs`).

## Rules
- Edit only `Assets/_Game/Scripts/Pve/**`, `Assets/_Game/Scripts/UI/**`, `Assets/_Game/Editor/**`, `Assets/_Game/Tests/**`.
- Do not touch `Scripts/Engine/**` (another task is staging engine changes), the data JSON files, or docs. **No git.**
- Change the builders, not the generated prefabs. The art does not exist yet. **Everything must work with zero, some or all of the images present**, falling back to the current look.

## Changes
1. **`PveArtLibrary`:**
   - Stop hardcoding 5 keys. The builder scans `Assets/_Game/Textures2D/Pve/**` and `Assets/_Game/Textures2D/UI/**` and registers every sprite by file name. `ItemIconImporter` has the sprite import settings pattern: reuse it so new PNGs import as sprites.
   - Add a menu item and a batch method `OneBagJianghu.EditorTools.Building.PvePrefabBuilder.RebuildArtLibrary` so Claude can refresh it after new art arrives.
   - Make `BuildAllOverwrite` call it too.
2. **Stage map** (`StageMapPopup`, `PveStageNode`):
   - Node sprite by kind: normal → `theme.nodeIcon` (the data will use `node_hoi_N`), then `node_stage`; elite → `node_elite`; boss → `theme.bossNodeIcon`, then `node_boss`. When no sprite exists, keep the current coloured frame.
   - Locked nodes are greyed and show `icon_lock`.
   - Stars use `icon_star` / `icon_star_empty` images (3 small images under the node). If missing, fall back to the current ★ text.
   - The Thể Lực display uses `icon_stamina`, and currency displays in PvE panels use `icon_bac` / `icon_nguyen_bao` / `icon_danh_vong` when present.
   - The stage detail panel uses `frame_stage_card` (9-sliced) when present.
3. **Dialogue:** `DialoguePopup` uses `frame_dialogue` (9-sliced) for the text box when present.
4. **Portraits everywhere:** dialogue, stage panel and the battle `FighterPanelView`.
   - Resolve a portrait key from the PvE art library first, then the item icon library, then the existing fallbacks.
   - The battle must show the stage enemy's `portrait` key (for example `boss_bui_chen`). Only mirror and ink-tint when it falls back to hero art, as `FighterPanelView` does today.
5. **Currency icons outside PvE (small, safe):** the Home top bar and the Thương Hội / Tàng Bảo Các currency labels show the icon next to the number when the sprite exists.
6. **Import settings:** sprites in `Textures2D/Pve` are imported like the item icons. Frames with "frame_" in the name get 9-slice borders of about 12% of each side; set them in the importer.

## Tests
- EditMode:
  - With a temporary sprite registered under `node_hoi_1`, a normal stage node of chapter 1 uses it. Without it, the fallback frame is used.
  - Library rebuild registers a sprite placed in a temp `Textures2D/Pve` subfolder. Clean it up afterwards.
- PlayMode: the stage map opens with no art present and shows no errors (the current placeholder data).

## Self-check
Claude runs Unity (Build All, EditMode, PlayMode) afterwards. List anything you could not verify.
