# Lệnh cho Gemini (Antigravity): thiết kế màn hình (UX/UI)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`. Nếu việc thiết kế 7 binh khí (`task-new-weapons.md`) chưa làm thì làm việc đó trước.

---

You are the UX/UI designer of "One Bag Jianghu" (2D wuxia backpack auto-battler, PC, 1920×1080). Produce the screen specifications that a programmer will implement in Unity (procedural UGUI) and an artist will paint.

## Hard rules
- Create ONLY `docs/reviews/2026-09-27-gemini-ui-screens.md` (Vietnamese, ≤ 400 lines). Do not modify any other file. Do not run git.
- Game terms exactly as in `docs/design/glossary.md`. No names from Kim Dung novels or other games.

## Read first
`docs/tech/ui-flow.md` (scene flow and UI architecture — authoritative), `docs/design/gdd.md`, `docs/design/glossary.md`, `docs/design/combat.md`, `docs/design/items.md` (Tâm Pháp, Đột Phá, bag rules), `docs/art/art-pipeline.md` (Thủy Mặc style, palette), the images in `docs/art/reference/`, and the current Unity code in `One-Bag-Jianghu/Assets/_Game/Scripts/` (Bag/, Battle/Replay/, UI/ if present) to see what already exists.

## Deliver, for each of: LoadingPanel, LoginScene, HomeScene (+ its panels: Bao Phục, Hành Tẩu Giang Hồ chapter/stage select, Luận Kiếm Đài lobby, Cài đặt), GameplayScene (phases: Chuẩn bị, Giao đấu, Kết quả)
1. Purpose and entry/exit transitions.
2. **Wireframe as an ASCII layout on a 1920×1080 grid** with approximate pixel rects for each block (x, y, w, h), anchoring (which corner/edge), and what happens at 16:10.
3. Element list: type (button, text, bar, list, grid, icon…), content/label in Vietnamese, states (normal/hover/pressed/disabled/selected), data source (which JSON/session field).
4. Interactions: click/drag/hover/tooltip/keyboard shortcuts (e.g. Space = pause replay, 1/2/4 = speed, Esc = back).
5. Feedback: sounds (describe only), animations, empty states, error states (e.g. bag has no weapon → cannot start battle).
6. Art needs: list of images per screen (backgrounds, frames, buttons, ornaments, icons) with target pixel size — this list will be turned into ChatGPT prompts.

Also include:
- A shared **UI kit** section: button styles (primary vermilion, secondary ink, disabled), panel frames, tooltip style for items (name, rarity color, size, stats, Phổ Công/Tuyệt Kỹ, Đột Phá at tiers 3/6/9, which Tâm Pháp buffs it), font sizes (title/heading/body/caption at 1920×1080), spacing grid (8 px), z-order layers (content 0–99, popup 100–199, toast 200–299, LoadingPanel 1000).
- A priority list: which screens/elements are needed for a first playable (MVP) vs later.
- Max 5 open questions for the product owner.
