# Codex task: Simple UI art, round 3 (fix resolution, seams and the remaining files)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Read `docs/tech/code-style.md`, decision #66 and
`docs/art/ui-simple-plan.md`. Continue from your round 2 scripts: `tools/codex/out/build_ui_simple_art.py` and
`check_ui_simple_art.py`. Use `image_gen` and Pillow.

## Round 2 review: what passed
Caps + middle composition with uniform scaling. The replay bars, `home_banner_simple`, `hud_panel_simple` and
`log_panel_wide` fill their canvas and the end ornaments are not squashed. Keep this method.

## Fix
1. **Resolution.** Files were written at 1x canvas size: `count_badge_compact` is 20x14 px and
   `button_replay_control` is 170x64. Write every file at **2x the target canvas size**, or larger. The long side
   may not exceed 2048; if 2x would exceed it, use the largest scale that fits. Examples:
   - `count_badge` → 80x56 (4x is fine for tiny ones);
   - `tier_pill` → 168x96;
   - `button_replay_control` → 340x128;
   - replay bars → 2048 wide.
   Compose at the final pixel size from the source pieces. **Never upscale an already-downscaled file.**
2. **Tile seams.** The `replay_top_bar` middle shows visible repeating blocks at 3x zoom.
   - Pick a plainer middle segment, wider (at least 1.5x its height).
   - Cross-fade the joins (about 15% of the segment width) so no vertical seam or repeating pattern shows.
   - Check every caps + middle file at 3x on the sheet.
3. **`log_panel_expanded` (1320x440).** It is currently a horizontal strip with the left and right borders cut off.
   It needs a full frame: four corners with ornaments, plus edges.
   - Compose it in 2D from one source frame: four corner pieces, top and bottom edge segments tiled horizontally,
     left and right edge segments tiled vertically, and the centre fill tiled or a plain matching texture.
   - Scale uniformly only.
4. **`tier_pill_compact` (42x24).** It is now a plain dark rectangle. Make it a small lacquer pill with gold rim and
   end caps that reads at 42x24 canvas.
5. **The 11 round 1 files that kept their padding:** `tooltip_panel`, `hero_portrait_frame_wide`,
   `bag_board_landscape`, `fighter_panel_replay`, `plaque_label_compact`, `plaque_label_medium`,
   `plaque_label_tall`, `frame_panel_square`, `slot_cell_square`, `slot_cell_highlight_square`,
   `slot_cell_invalid_square`. Their content fills only 77–96% of the canvas, so the frame would show smaller than
   its rect. Rebuild each one so it fills at least 97% on both axes.
   - Tight-crop the drawn frame. Then either it already has the target aspect within 1% (scale uniformly), or
     compose it in 2D (corners + tiled edges + centre) to the exact target.
   - Ornaments that stick out beyond the frame (`bag_board` corner knots, `plaque_label` ropes, the portrait's red
     ribbon) count as content. The rect is the outer bounding box. The builder will place it so the frame body sits
     where it should; note in the plan, per file, the inset of the frame body (left/top/right/bottom, in % of the
     canvas), so the layout can align content inside it.
   - The 3 `slot_cell_*_square` files must stay pixel-aligned with each other: same frame geometry, different fill.

## Acceptance (your checker, extended)
For all 25 files:
- size ≥ 2x target, or the 2048 cap;
- aspect within 1% of the target;
- content fill ≥ 97% on both axes;
- corner alpha 0 where the shape is not rectangular;
- for composed files, a log line with the parts, the scale, and the cross-fade width.

Rebuild the contact sheet. Show each file at its target canvas size, plus a 3x crop of each end, or of each corner
for 2D frames. Also make `tools/codex/out/ui-simple-art-review.png`, a compact sheet that fits about 2000x2000 px
with all 25 at target size, for the user to review.

## Rules
- Overwrite only the 25 plan files in `Textures2D/UI/Kit/`.
- You may also edit:
  - the scripts and outputs in `tools/codex/out/`;
  - `docs/art/ui-simple-plan.md`, including the frame body insets;
  - the group J prompts in `docs/art/prompts-ui-kit.md`.
- No text in images. No C#, prefab, scene or `.meta` changes.
- Do not launch Unity. No git commands that change the repo.

## Report (`-o` output)
- the checker output;
- per file: the method and the final pixel size;
- the files you are least sure about.
