# Codex task: plan and draw the art for "all UI images Simple" (phase 2 of decision #66)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/`. Code in English, docs in Vietnamese.
**Read `docs/tech/code-style.md`.** Read decision #66 in `docs/decisions.md`. Use your built-in image generation
(`image_gen`) and Pillow.

## Input
- The audit `docs/art/ui-image-audit.md` and `tools/codex/out/ui-image-audit.csv`, measured at 1920x1080 in canvas
  units. "class C" means distorted.
- The existing kit art in `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/` and `UI/Kit/`.
- The prompts in `docs/art/prompts-ui-kit.md`: the style is Thủy Mặc ink and lacquer, matching the existing files.

## 1. Decide each distorted use: write `docs/art/ui-simple-plan.md` (Vietnamese)
For every class C group (sprite + rect size), choose the **first** option that works:
1. **Resize the rect to the art's aspect.** Allowed when the element is fixed-size and changing one dimension by at
   most about 12% does not collide with neighbours. Examples to consider: `count_badge` (20x14 vs art 2:1),
   `hero_portrait_frame`, `fighter_panel`, `button_*`, `plaque_label`, `dropdown_field`, `tier_pill`. Give the
   new size.
2. **Crop or pad the existing art** when the art has transparent or empty margin and the error is ≤ 10%. For
   example, `slot_cell` 1138x1070 into a square: crop it to square without cutting ornaments. Save it as a new
   file; do not overwrite the original.
3. **New art at the exact aspect.** Use this for long bars and panels: replay `TopBar` 1920x164 and `BottomBar`
   1920x120 (now `frame_panel`), `home_banner` 1920x132, `hud_panel` 780x148, `log_panel` 1300x96 and 1320x440,
   `timeline_bar` 1300x20, `frame_tooltip` sizes, `bag_board` 540x450, and anything else options 1–2 cannot fix.
   - Uses whose aspects are within 5% of each other share one image.
   - Name each file by its role, not its size, for example `replay_top_bar.png`, `replay_bottom_bar.png`,
     `log_panel_wide.png`.
- **Layout- or content-driven sizes** (tooltips, buttons whose width follows the text, list rows): propose a fixed
  size, or two or three fixed sizes, each with its own art. Text must shrink to fit (best fit). Leave about 30%
  headroom for later localization.
- **Scrollbars:** the handle length follows the content, so no fixed art fits. Use a plain flat-colour handle and
  track (no sprite, or a 1x1 white sprite tinted with the kit's ink colours).
- **Fills (slider, HP, qi, progress):** `Image.Type.Filled`, horizontal, so the art is cropped and never
  stretched. The art must be drawn at the full bar aspect.
- **Fallback chains** listed in the audit: say what replaces each. Either the wanted sprite already exists (then
  just drop the fallback), or name the new art.
- Ignore the 100x100 template entries (unsized component prefabs) unless they are used at that size.

The plan has one table: element (prefab/scene :: object), sprite now, rect now, decision (resize / crop / new art /
Filled / flat), new rect if resized, art file.

## 2. Produce the art
- For every crop or new-art row, create the file under `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/`.
- Pixel size: long side 2–4x the canvas size, at most 2048. The aspect must match the target rect within 1%.
- **No text, letters or numbers in any image**, so the UI can be localized later.
- Transparent background. Match the existing kit: palette, line weight, corner ornaments, and the paper and lacquer
  textures. Put the matching existing file next to the prompt as a reference.
- Ornaments must stay inside the image. A frame's inner area must stay plain enough for text or content.
- Check every image yourself:
  - correct size and aspect;
  - the alpha at all corners is 0 where the shape is not rectangular;
  - no text;
  - the style matches its reference.
  Redraw anything that fails, up to 2 times. Then build a contact sheet `tools/codex/out/ui-simple-art-sheet.png`:
  each new image at its target canvas size on the paper background, labelled underneath (the labels belong to the
  sheet only).
- Append each new prompt to `docs/art/prompts-ui-kit.md` as group J, numbered after the last one.

## Rules
- Only create or modify:
  - `docs/art/ui-simple-plan.md` and `docs/art/prompts-ui-kit.md`;
  - new PNG files in `Textures2D/UI/Kit/`;
  - `tools/codex/out/`.
- **Do not change C#, prefabs, scenes or existing PNGs** in this task. Do not create `.meta` files: Unity will.
- Do not launch Unity. **No git commands that change the repo.**

## Report (`-o` output)
- counts of resize / crop / new art / Filled / flat;
- the list of new files with their pixel size and target rect;
- anything you could not decide, and why;
- the art you are least happy with.
