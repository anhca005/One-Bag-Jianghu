# Codex task: draw the story cutscene images for ONE chapter

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. Use your built-in image generation skill (`image_gen`).

The first line of this message names the chapter to draw (e.g. "CHAPTER 2"). Draw **only that chapter's 5 images**.

## Inputs
- Prompts: `docs/art/prompts-story.md`, section of your chapter, files `story_<chapter>_1.png` … `story_<chapter>_5.png`.
- Story context: `docs/design/story.md` and the narration in `One-Bag-Jianghu/Assets/_Game/Resources/Data/story.json`.
- **Style references (attach / look at them for every image):**
  - `docs/art/reference/01-style-board-thuy-mac.png` (overall ink-wash style);
  - `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/intro_1.png` … `intro_5.png` (the existing cutscene look: match it);
  - the hero: `One-Bag-Jianghu/Assets/_Game/Textures2D/UI/hero_idle.png` (keep his look consistent);
  - allies and bosses when they appear, from `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/`: `npc_lao_tieu_phu.png`
    (Lục Tiều Phu), `npc_tieu_linh_nhi.png` (Bạch Linh Nhi), `npc_phong_loi.png` (Phong Vũ), `npc_yen_vo_tam.png`
    (Yến Vô Tâm), `boss_tao_huyen_ky.png` (Tào Huyền Kỳ), and other `boss_*` / `enemy_*` / `monster_*` files that match
    the chapter's bosses.

## Each image
- Landscape **1536x1024**, 2D Chinese ink-brush on aged rice paper, same palette as the intro images.
- **No text, letters, numbers, seals with characters, signatures or watermark.**
- Keep the lower 30% calm and low-contrast (the narration scroll covers it).
- Use the prompt from `prompts-story.md`, adjusted only to match the references (hero and characters' look).
- Save the final PNG to `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/story_<chapter>_<n>.png` (copy it from where the
  tool saves it). Exactly 1536x1024; resize/crop if the tool returns another size, keeping the subject centered.
- If an image comes out with text or a broken figure, redraw it once.

## Rules
- Write only those 5 PNG files. No code, no `.meta` files (Unity makes them), no git.
- Do not touch other chapters' files: other Codex runs draw them at the same time.

## Report (`-o` output)
- The 5 files written, and any image you had to redraw or that still has a problem.
