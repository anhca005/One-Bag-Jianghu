# Codex task: Simple UI art, round 4 (7 files whose borders were cropped off)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Read `docs/tech/code-style.md`, decision #66 and
`docs/art/ui-simple-plan.md`. Continue in `tools/codex/out/build_ui_simple_art.py` and `check_ui_simple_art.py`.

## Round 3 result
**18 files pass. Do not touch them:**
- the replay bars, `home_banner_simple`, `hud_panel_simple`, `button_replay_control`;
- `panel_paper_strip`, `tooltip_field`, `tooltip_panel`, `input_field_wide`, `hero_portrait_frame_wide`;
- `bag_board_landscape`, `dropdown_field_medium`, `log_panel_wide`, `log_panel_expanded`, `timeline_bar_thin`;
- the three `plaque_label_*` files.

**Rejected, because "uniform cover" scaled the art past the canvas and cut the overflow off:**
- `slot_cell_square`, `slot_cell_highlight_square`, `slot_cell_invalid_square`: the right and bottom borders and
  both lower corners are cut off.
- `frame_panel_square`: the right edge is cut.
- `fighter_panel_replay`: the bottom and left borders are cut.
- `count_badge_compact` and `tier_pill_compact`: only the inner fill is left. The rim and end caps are gone; it is
  a plain red or black rectangle.

## Required algorithm (no cover scaling, no cropping into the frame)
For each rejected frame:
1. Take the **untouched source** (the original kit file or the image_gen source). **Tight-crop** it to its alpha
   bounding box (alpha > 16). This is the only crop allowed.
2. Pick one uniform scale `s`, then build the target canvas from **9 pieces** of the tight-cropped source:
   - `s` is chosen so the corners fit: the corner size times `s` is at most 35% of the target's short side;
   - corners are copied whole, scaled by `s`, and placed flush in the 4 canvas corners;
   - the top and bottom edge pieces are scaled by `s` and **tiled** horizontally between the corners, with a
     cross-fade at the joins;
   - the left and right edge pieces are scaled by `s` and tiled vertically;
   - the centre is a plain patch of the source's inner texture, scaled by `s` and tiled with cross-fades. Or, for
     the slot cells, it is the source centre scaled uniformly to cover the inner area, **clipped to the inner area
     only**.
   The corner size is the ornament's extent, measured on the source. Pick it so each corner contains the whole
   corner ornament, and log it.
3. The 3 slot cells use **identical geometry**: the same corner sizes, the same `s`, the same positions. Only the
   textures differ.
4. `count_badge_compact` (target 80x56) and `tier_pill_compact` (168x96):
   - generate a new small source with `image_gen`, prompted as a **small pill or badge with a clearly visible gold
     rim and rounded or cloud end caps**, in the kit style, no text;
   - tight-crop it and scale it **uniformly to fit inside** (contain) the target;
   - if the aspect differs, extend the plain middle horizontally with the caps + middle method.
   - The rim must be visible on all four sides.

## Acceptance (extend the checker)
In addition to the round 3 checks:
- **Corner check**: for each 9-piece file, compare each canvas corner patch with the source corner scaled by `s`.
  The mean absolute RGBA difference must be below 6. This proves the corners are whole and not cut.
- **Rim check** for every rejected file: in each of the 4 edge strips (outer 4% of the canvas), at least 85% of the
  pixels along the strip have alpha > 128. This proves no border is missing.
- Print a PASS or FAIL line per file. All 25 must pass.

Rebuild `tools/codex/out/ui-simple-art-review.png` with all 25 files. Also make
`tools/codex/out/ui-simple-art-fixed.png`: the 7 fixed files large (about 400 px high each), each next to its
source, so the borders can be compared.

## Rules
- Only overwrite these 7 PNGs in `Textures2D/UI/Kit/`.
- You may also edit:
  - the scripts and outputs in `tools/codex/out/`;
  - the plan notes;
  - the group J prompts.
- No text in images. No C#, prefab, scene or `.meta` changes.
- Do not launch Unity. No git commands that change the repo.

## Report (`-o` output)
- the checker output;
- per file: the corner size, `s`, and the source used.
