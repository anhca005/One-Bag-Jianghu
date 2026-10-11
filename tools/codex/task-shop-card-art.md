# Codex task: draw one landscape shop card (`shop_card_wide.png`)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Use your built-in image generation skill (`image_gen`).

## Why
The shop item card in game is **landscape, 420x300 (7:5)**. The current art `shop_card.png` is portrait, so
stretching it sideways warps the paper, the border and the footer band. We need a card drawn at the real shape, so the
game can stretch it uniformly (no 9-slice).

## Inputs
- Prompt: `docs/art/prompts-ui-kit.md`, item **#109** (`shop_card_wide.png`). Prepend the "Tin mở đầu" style text.
- Style references (look at them): `docs/art/reference/01-style-board-thuy-mac.png` and, in
  `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/`, `shop_card.png` (match its paper, lacquer border, gold fret
  corners and darker footer band: the new card is the same design, only landscape), `shop_card_frame.png`,
  `shop_card_knot.png` (the cord is a separate image in game: do **not** draw a cord or knot), `price_tag.png`,
  `button_primary.png`.

## The image
- Generate landscape (1536x1024), then crop to **exactly 7:5** (e.g. 1400x1000) around the card with ~1.5% padding.
- Card fills the frame; straight, even edges; fret corner brackets small (each about 8–10% of the card height).
- Footer band along the bottom, about **18–20% of the card height**, a little darker parchment, separated by a thin
  gold line with small fret ends. It holds a button, so keep it plain.
- Face plain and even (item picture, name, stats and a price plate go on it).
- **Transparent background** (real alpha) outside the card. If the tool returns a solid or checkerboard background,
  use `py tools/art/remove_checker.py <file>` (read its docstring); check alpha and that the card is intact.
- No text, letters, numbers, seals, signatures, watermark, cord or knot. Redraw once if any appear or if the card is
  not a clean rectangle.
- Save to `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/Kit/shop_card_wide.png`. Also save a preview with the card
  scaled to 420x300 on a mid-grey background to `tools/codex/out/shop_card_wide_preview.png`.

## Rules
Write only those two PNG files. No code, no `.meta`, no git, no other images touched.

## Report (`-o` output)
Final pixel size, aspect, has alpha, footer band height in pixels (top of its gold divider line to the bottom edge
of the card), border thickness in pixels, redrawn yes/no, problems left.
