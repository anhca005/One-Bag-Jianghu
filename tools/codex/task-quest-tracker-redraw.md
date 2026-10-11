# Codex task: redraw `quest_tracker.png` (art only)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Use your built-in image generation (`image_gen`).

The current `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/quest_tracker.png` (440x128) has a thin plaque: the paper
face is only ~45 px tall and most of the image is empty, so the two text lines (quest title ~23 px, objective ~18 px)
do not fit. The game stretches the image whole over a 440x128 rect.

Redraw it, same design family (look at the current file, `Kit/quest_log_row.png`, `Kit/shop_card_wide.png`,
`Kit/price_tag.png`, `docs/art/reference/01-style-board-thuy-mac.png`):
- **Final file 880x256 (same 55:16 ratio), the plaque filling it** (≤ 3% transparent padding): rice-paper face at
  least **75% of the height**, thin dark-lacquer border with ochre-gold cloud corners, a round slot on the left
  (about 70% of the height, for a scroll icon), plain even face to the right of the slot for two text lines;
  ribbons/tassels, if any, must stay inside the image and not cover the face.
- Transparent background with real alpha (`py tools/art/remove_checker.py` if needed). No text, letters, numbers.
- Overwrite `quest_tracker.png`; keep its `.meta` file untouched. Update the row #110 prompt in
  `docs/art/prompts-ui-kit.md` with the prompt you used.
- Save a preview with the image scaled to 440x128 on mid-grey to `tools/codex/out/quest_tracker_preview.png`.

Rules: only those files. No code, no git.

Report (`-o` output): final size, alpha yes/no, face height in px, slot centre x and diameter in px (at 880x256), so
the leader can place the icon and text.
