# Codex task: draw the sect and Tâm Pháp images

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Use your built-in image generation skill (`image_gen`).

## Inputs
- Prompts and sizes: `docs/art/prompts-sects.md` (section A: 7 sect images, section B: 7 Tâm Pháp images).
- Lore: `docs/design/story-bible.md`, "Cửu Châu và Bảy Đại Môn Phái".
- Style references (look at them for every image): `docs/art/reference/01-style-board-thuy-mac.png`,
  `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/intro_1.png` … `intro_5.png`.

## Each image
- Use the prompt from `prompts-sects.md`.
- **No text, letters, numbers, seals with characters, signatures or watermark.**
- Save to `One-Bag-Jianghu/Assets/_Game/Textures2D/Sects/` (create the folder):
  - `sect_<id>.png`, exactly 1536x1024;
  - `tam_phap_<id>.png`, exactly 1024x1024.
  Ids: `kim_cang`, `thai_thanh`, `liet_diem`, `huyen_doc`, `han_son`, `thiet_phu`, `bich_thuy`.
- Resize/crop if the tool returns another size, keeping the subject centered.
- If an image has text or a broken subject, redraw it once.
- Draw the 7 Tâm Pháp images first, then the 7 sect images.

## Rules
- Write only those 14 PNG files. No code, no `.meta` files, no git.

## Report (`-o` output)
The files written, and any image that was redrawn or still has a problem.
