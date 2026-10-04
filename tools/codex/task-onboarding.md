# Codex task: intro, tutorial and Giang Hồ Bách Khoa (decision #58)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `../docs/design/onboarding.md`: the full design. Follow it exactly.
- `docs/tech/code-style.md` (mandatory).
- `../docs/design/glossary.md`: the source for the encyclopedia text.
- `Scripts/UI/Pve/DialoguePopup.cs`, which is reused for the guide's lines.
- `Scripts/Economy/PlayerProfile.cs` and profile save/migration.
- Home: `Scripts/UI/Home/*`, and `Editor/Building/LoginHomeSceneBuilder.cs`. **Only the Home part:** never touch
  `BuildLoginScene` or `LoginScene`.
- Popups and builders: `Editor/Building/PopupPrefabBuilder.cs`, `UiBuild.cs`, `UiKit`.
- The bag, storage, shop, stage map, replay and sect code, where the step-completion events come from.
- `Scripts/Audio/AudioManager.cs`: play `ui_open`/`ui_click` where natural.

## Build
1. **Data** (you write it; Claude reviews the Vietnamese text):
   - **`Resources/Data/intro.json`:** the 5 panels from the design (image name + text).
   - **`Resources/Data/tutorial.json`:** the 4 sequences from the design. Each step has:
     - the guide's line;
     - the target UI id;
     - the completion event.
   - **`Resources/Data/encyclopedia.json`:** all terms in `glossary.md`, rewritten for players.
     - No code names and no decision numbers.
     - Each entry has: name, category, and 1–3 sentences in natural Vietnamese.
   - **Validation:** parse each file in a catalog class.
2. **Intro.**
   - An `IntroPopup` or overlay: full-screen image, a paper strip with the text, click or Space for next, 0.4 s fade,
     a slight slow zoom, and a Skip button.
   - **Missing images** (`Textures2D/Intro/intro_N` will arrive later): fall back to the chapter 1 map or a plain paper
     background, with no errors.
   - Shown once, on first Home entry after character creation (profile flag).
   - **Settings:** add a "Xem lại mở đầu" button, and also "Đặt lại hướng dẫn". Keep the 2×2 button grid tidy: if you add
     a row, keep the same column positions.
3. **Tutorial.**
   - A `TutorialService` (pure logic, testable) holds the active sequence and step, listens for game events, and
     persists completed sequences in the profile. Add a profile field with a safe default for old profiles.
   - **`TutorialOverlay`:**
     - a dim layer with a hole around the target rect (a 4-quad mask or a shader-free approach);
     - only the target receives clicks; for drag steps, the source and the destination;
     - the guide's dialogue box with Lục Tiều Phu's portrait;
     - a "Bỏ qua hướng dẫn" button.
   - **Target lookup:** give the needed UI objects stable ids. Use a small `TutorialTarget` component with an id string,
     added by the **builders**.
   - **Sequence triggers:**
     - `first_battle` after the intro;
     - `shop` on the first Home return after the first clear of stage 1;
     - `upgrade` when an owned item first becomes affordable to upgrade;
     - `sect` on reaching level 10.
   - **In-battle hints** for the first battle only: 3 short lines at the moments in the design. Non-blocking: the replay
     keeps running.
   - If a target is missing (UI changed), skip that step and log once. **Never soft-lock the player.**
4. **Giang Hồ Bách Khoa.**
   - A Home entry, through a builder, placed so it does not overlap existing hotspots or the banner. Check positions in
     `LoginHomeSceneBuilder`.
   - **Two-column popup:** categories on the left; on the right, the entries list and the selected entry's text.
     Scrollable.
   - Optional search field.

## Tests
- **EditMode:**
  - the three catalogs parse, and every tutorial target id used exists as a `TutorialTarget` in the built prefabs or
    scene (or list those that are created at runtime);
  - `TutorialService` step flow, skip, persistence, and an old profile without the field;
  - the encyclopedia covers every glossary term name;
  - the intro fallback with no images.
- **PlayMode:** a new profile goes through intro skip → tutorial step 1 visible → skip tutorial → no overlay remains.
  - Keep `PlayerDataGuard` protecting the user's data.

## Rules
- **No git.**
- **Do not touch:**
  - `LoginScene` or `BuildLoginScene`;
  - balance data;
  - `tools/**`, `docs/**`.
- Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`; it must be clean for
  new files and changed lines.

## Report (`-o` output)
- The files changed.
- The profile fields added.
- The target ids and where each is attached.
- Anything in the design you could not do.
- The lint result.
