# Codex task: chapter-end story cutscenes and a story replay in Giang Hồ Bách Khoa

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. Code under `Assets/_Game/`. You cannot run Unity; Claude
builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory), `docs/design/onboarding.md`, `docs/design/pve.md`;
- the intro: `Resources/Data/intro.json`, `Scripts/Onboarding/IntroPopup.cs` (full-screen panels, click/space to
  advance, scroll frame for the text, zoom on show), how it resolves art through `PveArtLibrary`
  (`Textures2D/Pve/intro_1..5`), and `BuildIntro` in `Editor/Building/PopupPrefabBuilder.cs`;
- `Scripts/Onboarding/EncyclopediaPopup.cs`, `Resources/Data/encyclopedia.json`, `BuildEncyclopedia`;
- `Scripts/UI/Gameplay/GameplayController.cs` (PvE result flow, `HandleResultClosed`), `PveRules.IsCleared`,
  `Resources/Data/Pve/chapters.json` (5 chapters, 10 stages each, stage 10 is the boss).

## Goal (user request)
- At the end of each chapter, a short cutscene tells that chapter's story.
- The player can re-watch the story of each chapter later, from Giang Hồ Bách Khoa.

## Do
1. **Data:** new `Resources/Data/story.json`:
   ```json
   { "chapters": [
     { "chapter": 0, "title": "Mở đầu", "panels": [ { "image": "intro_1", "text": "..." } ] },
     { "chapter": 1, "title": "Hồi 1 · Thanh Khê Trấn", "panels": [ { "image": "story_1_1", "text": "..." } ] }
   ] }
   ```
   - Chapter 0 is the existing intro; move or mirror `intro.json` so the intro keeps working exactly as today.
   - Chapters 1–5: 4–6 panels each. **Placeholder text** for now from each chapter's `summary` in `chapters.json`,
     split into short sentences. Gemini is writing the real script; Claude will replace the text and images later, so
     keep the format simple and documented.
   - Images `story_<chapter>_<n>` live in `Textures2D/Pve/` and resolve through `PveArtLibrary`. Until the art exists,
     fall back to the chapter's map/background art or an intro image; never show an empty frame.
2. **Player:** generalise `IntroPopup` into a story player (or a `StoryPopup` that shares its code) that takes a list of
   panels and a title. Same look and controls as the intro: full screen, click/space/enter to advance, Esc to skip,
   title card first.
3. **Trigger:** the first time the player clears a chapter's boss (stage 10), after the result popup closes and
   before returning to the map, play that chapter's cutscene once. Save "seen" in the profile so it never replays by
   itself. Do not break the onboarding chain after stage 1 (`first_battle_won`, `result_closed`, map/shop steps).
4. **Bách Khoa:** add a category **"Cốt truyện"**. One row per chapter the player has reached: Mở đầu always; chapter N
   once its boss is cleared. Selecting a row shows the chapter title and a short summary plus a **"Xem lại"** button
   that plays the cutscene. Locked chapters show as "Chưa mở khóa" or are hidden (pick one, say which).
5. **Builder:** prefab changes go through `PopupPrefabBuilder` (story popup and the encyclopedia button). Tell Claude
   the menu item(s) to rebuild. Keep `Encyclopedia Popup (Overwrite)` working.

## Tests (EditMode)
- `story.json` parses; chapters 0–5 exist; every panel has text.
- Clearing stage 10 of chapter 1 for the first time marks the cutscene pending once; a second clear does not.
- The Bách Khoa story list contains Mở đầu plus exactly the cleared chapters.

## Rules
- **No git.** Never touch `LoginScene`, `BuildLoginScene` or the GameController prefab builder.
- Other Codex tasks run at the same time. **Do not touch** `Scripts/Battle/Replay/`, shop/economy/stamina files,
  `HomeController.cs`, `StageMapPopup.cs` or `chapters.json`. You may touch `GameplayController.cs` only in the result
  flow.
- Lint every changed `.cs` file with `py tools/lint/check_style.py`; changed lines must be clean.

## Report (`-o` output)
- The `story.json` format, the files changed, where the trigger lives, the rebuild menu items, lint result.
