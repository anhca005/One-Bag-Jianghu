# Gemini task: story script for the 5 chapters and their end-of-chapter cutscenes

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. You write content only; no code.

## Read first
- `CLAUDE.md` and `docs/design/glossary.md` (in-game terms; **never** use names from Kim Dung novels or from the
  reference games);
- `docs/design/gdd.md`, `docs/design/pve.md`, `docs/design/onboarding.md`;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/intro.json` (the opening: Cửu Cực Càn Khôn Nang, U Minh Giáo, the
  father's last words, Bến Thanh Khê);
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: for each chapter its `title`, `name`, `summary`, and
  for each stage the enemy `name`, `description` and `dialogue`. The story must agree with these: same places, same
  allies (Lão Tiều Phu / Lục Tiều Phu, Bạch Linh Nhi, Phong Vũ, Yến Vô Tâm), same villains (Ba Hổ Trại, Huyền Độc Môn,
  Hàn Sơn Phái's inner demon, U Minh Tả Sứ Dạ Sát Thiên, U Minh Hữu Sứ, Thiên Cơ Viện, Tào Huyền Kỳ, Giáo chủ U Minh).
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/encyclopedia.json` (lore entries).

## Problem
The user finds the story unclear. Each chapter needs a clear arc, and the end of each chapter gets a short cutscene
(still images + narration, like the intro) that tells what happened and points to the next chapter.

## Write
1. **`docs/design/story.md`** (Vietnamese):
   - the main thread in one paragraph: the hero, the broken Cửu Cực Càn Khôn Nang, what U Minh Giáo wants, how each
     chapter recovers a piece or a clue, the ending hook after Hồi 5 (the Giáo chủ carried the core away);
   - per chapter: goal, ally met, villain, the turning point at the boss, what the hero gains (a piece of the bag, a
     clue, a technique), and the hook into the next chapter. 6–10 lines each;
   - a list of any **inconsistencies** you found in `chapters.json` dialogue/summaries, with a suggested fix (do not
     edit that file).
2. **`tools/gemini/out/story.json`**, exactly this format:
   ```json
   { "chapters": [
     { "chapter": 1, "title": "Hồi 1 · Thanh Khê Trấn",
       "panels": [ { "image": "story_1_1", "text": "..." } ] }
   ] }
   ```
   - chapters 1–5, **5 panels each** (image ids `story_<chapter>_<n>`, n = 1..5);
   - each `text` is narration of **at most 2 sentences, about 25–45 words**, written in the tone of `intro.json`
     (literary wuxia Vietnamese, second person "ngươi" for the hero is fine as in the intro);
   - panel 1 recalls the chapter's situation, panels 2–4 the climax at the boss, panel 5 the outcome and the hook.
3. **`docs/art/prompts-story.md`**: one English image prompt per panel (25 prompts) for an image model, same style as
   the intro art: 2D Chinese ink-brush (shuimo) on aged rice paper, black brush outlines, flat ink washes, vermilion and
   ochre-gold accents, landscape 1536x1024, **no text, no letters, no watermark**, keep the lower 30% calmer because the
   narration scroll covers it. Name the file each prompt produces (`story_1_1.png` …). Describe the hero consistently
   (young wanderer, grey-blue travelling robe, bamboo hat, cloth bundle, plain sword) and each ally/villain
   consistently across panels.

## Rules
- Write only the three files above. Do not edit any other file. No git.
- Vietnamese with full diacritics. Use glossary terms exactly.

When done, reply with a short summary: the main thread in 3 lines, and the inconsistencies found.
