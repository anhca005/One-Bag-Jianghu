GROUPS: H

# Codex task: draw the UI kit images (round 2) for the groups named on the first line

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Use your built-in image generation skill (`image_gen`).

The first line of this message names your groups (e.g. "GROUPS: C, D"). Draw **only those groups' images**. Other
Codex runs draw the other groups at the same time.

## Inputs
- List, file names and prompts: `docs/art/prompts-ui-kit.md` (groups A–G, items #44–98). Prepend the "Tin mở đầu"
  style text to every prompt (and for group D the extra icon sentence written above its table).
- Style references (look at them for every image): `docs/art/reference/01-style-board-thuy-mac.png`, and the existing
  UI pieces in `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/` (`frame_panel.png`, `button_primary.png`,
  `button_secondary.png`, `frame_tooltip.png`, `plaque_label.png`, `progress_bar.png`, `rarity_*.png`). New pieces
  must look like the same family: same ink line weight, same paper, vermilion and ochre-gold.
- Keep pieces within a group consistent with each other (same border, same proportions for pairs like
  tab_active/tab_idle, slot_cell variants, bar fills).

## Each image
- Generate at the size written in the table.
- **Transparent background** (real alpha). If the tool returns a solid or checkerboard background, remove it with
  `py tools/art/remove_checker.py <file>` (read its docstring); check the result has alpha and the piece is intact.
- **Trim** the transparent margin to the piece's bounding box plus about 2% padding (Pillow), so thin pieces (bars,
  tabs, ribbons, dividers, rows) come out tightly cropped. Variants of one piece (e.g. `slot_cell`,
  `slot_cell_highlight`, `slot_cell_invalid`; `tab_active` / `tab_idle`; `bar_fill_*`; `toggle_on` / `toggle_off`;
  `list_row` / `list_row_selected`) must end with **identical pixel sizes**: resize the variants to the first one.
- Pieces marked 9-slice: check the straight edges are plain and even; redraw once if the edges carry ornaments or
  the center is not plain.
- **No text, letters, numbers, seals with characters, signatures or watermark.** Redraw once if any appear.
- Save to `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/<file>` (create the folder).

## Rules
- Write only your groups' PNG files. No code, no `.meta` files, no git. Do not modify other images.

## Report (`-o` output)
A table: file, final pixel size, has alpha (yes/no), redrawn (yes/no), problems left.
