# Gemini task: expand the story — story bible and full stage dialogue

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`. You write content only; no code.

## Read first
- `CLAUDE.md`, `docs/design/glossary.md` (exact in-game terms; **never** use names from Kim Dung novels or the reference
  games);
- your previous work: `docs/design/story.md` (main thread, chapter arcs, the 5 inconsistencies you found) and the
  narration now in `One-Bag-Jianghu/Assets/_Game/Resources/Data/story.json`;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/intro.json`;
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: `characters` (speaker ids and portraits), and for
  every stage its `id`, `name`, `kind`, `description`, `enemy.name`, `enemy.sect` and `dialogue`
  (`before` / `afterWin` / `afterLoss`, lists of `{ "speaker": "<character id>", "text": "..." }`);
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/encyclopedia.json`, `sects.json`.

## Goal (user request)
The story should be richer and more detailed. Keep the same main thread and chapter arcs you wrote; deepen them.

## Fixed points (do not change)
- The 25 cutscene panels in `story.json`: images are already being drawn from them. You may polish the wording of a
  panel's `text`, but each panel must keep the same scene and moment.
- Place names, stage names, enemy names, sects, the order of stages, and who the bosses are.
- Speaker ids must exist in `chapters.json` `characters`. If a new speaking character is really needed, list it
  separately (id, name, title, which existing portrait could fit) instead of using it in the dialogue.

## Write
1. **`docs/design/story-bible.md`** (Vietnamese), the reference for all future story work:
   - **The world:** Cửu Châu, the seven great sects and their stance, the court, U Minh Giáo's structure (Giáo chủ, Tả Sứ,
     Hữu Sứ, branches such as Huyền Độc Môn, allies inside Thiên Cơ Viện), the legend of Cửu Cực Càn Khôn Nang (what it
     is, why it broke, what the shell and the Lõi Càn Khôn do). Use the glossary terms.
   - **Characters:** for the hero and each ally (Lục Tiều Phu, Bạch Linh Nhi, Phong Vũ, Yến Vô Tâm) and each boss/named
     villain (Bùi Chấn, Hắc Độc Nương Tử, Dạ Sát Thiên, Lộ Tẫn Quang, Tào Huyền Kỳ, Giáo chủ U Minh): background,
     motivation, personality, way of speaking (2–3 sample lines), relationship to the hero, what changes in them.
   - **Chapter by chapter:** for each chapter, a beat for **every one of its 10 stages**: what happens there, why the
     enemy is there, what the hero learns or gains, and how it moves toward the boss. Resolve the 5 inconsistencies
     from `story.md` (one name "Lục Tiều Phu"; Phong Vũ's Hồi 1 duel as a test that sets up his alliance in Hồi 3;
     one term "Mảnh Tàn Nang" vs. "vỏ Thần Nang" vs. "Lõi Càn Khôn"; Dạ Sát Thiên's link to Hàn Sơn Phái; distinct
     Hồi 4–5 dialogue).
   - **After Hồi 5:** a short outline (5–8 lines) of where the story goes next (Bắc Minh tổng đàn), as a hook only.
2. **`tools/gemini/out/dialogue.json`**: new dialogue for **all 50 stages**, exactly this format:
   ```json
   { "stages": [
     { "id": "hoi_1_01",
       "dialogue": {
         "before":    [ { "speaker": "hero", "text": "..." } ],
         "afterWin":  [ { "speaker": "lao_tieu_phu", "text": "..." } ],
         "afterLoss": [ { "speaker": "lao_tieu_phu", "text": "..." } ] } }
   ] }
   ```
   - `before`: 2–4 lines; `afterWin`: 2–3 lines; `afterLoss`: 1–2 lines (a hint about the bag or a status is welcome,
     e.g. Hóa Kình, Kịch Độc, Điểm Huyệt, matching what that enemy does).
   - Each line at most ~25 words, natural wuxia Vietnamese, each character keeps the voice defined in the bible.
   - Every stage is specific to its place and enemy; no repeated template lines across stages.
   - Bosses (stage 10) get the strongest scenes and set up the chapter cutscene.
   - Keep the onboarding meaning of stage 1 (the first battle teaches arranging the bag).
3. **`tools/gemini/out/story-panels.json`**: only if you polished panel texts: the same format as `story.json`
   chapters 1–5, same image ids, each text at most 2 sentences / ~45 words. Skip this file if you changed nothing.

## Rules
- Write only the files above. Do not edit `chapters.json`, `story.json` or any other file. No git.
- Vietnamese with full diacritics; glossary terms exactly.

When done, reply with: the 5 inconsistencies and how you resolved each, any new character you propose, and the stage
count in `dialogue.json` (must be 50).
