# Codex task: redo the 14 failed Simple UI images (phase 2, round 2)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Read `docs/tech/code-style.md` (for Python too: one statement per
line, ≤120 chars). Read decision #66 in `docs/decisions.md` and your plan `docs/art/ui-simple-plan.md`. Use
`image_gen` and Pillow.

## What went wrong in round 1 (rejected)
`tools/codex/out/build_ui_simple_art.py` shrank each generated image with `thumbnail()` and centred it on a
transparent canvas. The canvas had the right aspect, but the art did not fill it. Measured content bounding box
(alpha > 16) as a share of the canvas:

| File | Fill (width × height) |
|---|---|
| `timeline_bar_thin` | 2% × 13% |
| `log_panel_wide` | 9% × 21% |
| `replay_bottom_bar` | 18% × 33% |
| `home_banner_simple` | 19% × 44% |
| `replay_top_bar` | 23% × 26% |
| `input_field_wide` | 27% × 58% |
| `dropdown_field_medium` | 42% × 38% |
| `panel_paper_strip` | 47% × 58% |
| `tooltip_field` | 47% × 88% |
| `hud_panel_simple` | 53% × 55% |
| `button_replay_control` | 95% × 57% |
| `log_panel_expanded` | 94% × 64% |
| `count_badge_compact` | 96% × 66% |
| `tier_pill_compact` | 93% × 35% |

The validation said "aspect within 1%", which was true of the canvas only. That hid the defect. **Padding a
canvas to fake an aspect is not allowed.**

Keep the other 11 files from round 1. They pass.

## Method for wide or tall shapes
`image_gen` only outputs about 1:1, 3:2 or 2:3, so a 15:1 bar cannot be drawn in one go. Build it from drawn
pieces, scaling uniformly only:
1. Generate one source image of the element drawn as a **long bar filling the whole image width**, in the kit's
   style, using the matching existing kit file as the reference. Generate a separate one if the end caps and a
   plain middle are easier to get that way. Transparent or flat chroma background.
2. Cut it into three parts:
   - the **left cap** (with its corner ornament);
   - a **middle segment** that tiles seamlessly; check it by placing two copies side by side; cross-fade a few
     pixels if needed;
   - the **right cap**.
   For a symmetric design, the right cap may be the mirrored left cap.
3. Scale all three parts **uniformly** (the same factor on x and y) so the height equals the target pixel height.
   Then place: left cap, then the middle repeated as many times as needed (crop the last repeat to fit), then the
   right cap.
4. Never resize on one axis only. Never stretch a single image to a different aspect.

Elements with a moderate aspect (≤ 2.5:1, or near-square) may be generated directly, then **cropped tight** to the
shape and scaled uniformly to the target. Example: `count_badge_compact` at 640x448.

Tall elements: use the same caps-and-middle method vertically.

## Acceptance (automated, required)
Write `tools/codex/out/check_ui_simple_art.py`. For every one of the 25 files in the plan, it prints and checks:
- size and aspect versus the target, within 1%;
- **content bounding box** (alpha > 16) at least 97% of the width **and** at least 97% of the height. A soft drop
  shadow beyond the shape is allowed;
- corner alpha;
- for composed files, a log line naming the parts and the scale factor.

Run it and make every file pass. Include its full output in the report. Then rebuild the contact sheet
`tools/codex/out/ui-simple-art-sheet.png` with each image at its target canvas size. Also add a 3x zoom of each bar
end, so the leader can check the ornaments are not squashed.

## Style
- Match the existing kit: palette, line weight, paper and lacquer textures, gold cloud corner ornaments.
- No text, letters or numbers anywhere in the images.
- Thin bars at small heights (`timeline_bar_thin` 1300x20 canvas): keep the end ornaments simple enough to read
  at 20 px. If a caps-and-middle bar at that height is unreadable, propose a better target size in the plan
  (for example 1300x28) and say so.

## Rules
- Only overwrite the 14 failed PNGs in `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/`.
- You may also edit:
  - the scripts and outputs in `tools/codex/out/`;
  - the notes and sizes in `docs/art/ui-simple-plan.md`;
  - the group J prompts in `docs/art/prompts-ui-kit.md`.
- No C#, prefab, scene or `.meta` changes.
- Do not launch Unity. No git commands that change the repo.

## Report (`-o` output)
- the checker output;
- the method used per file (direct + crop, or caps + middle);
- the files you are least sure about.
