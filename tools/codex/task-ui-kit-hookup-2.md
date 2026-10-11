# Codex task: hook the UI kit (round 2) into the game, part 2: battle screen, result, tutorial, encyclopedia

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Read `One-Bag-Jianghu/Assets/_Game/Editor/Building/README.md`.

Part 1 (already done, see `tools/codex/out/ui-kit-hookup-1.md`) set up import + 9-slice for every
`Textures2D/UI/Kit/*.png` and applied groups A–D. Use `UiBuild.ApplyUiSprite(image, "Kit/<name>", type)`. What each
file is for: `docs/art/prompts-ui-kit.md`. **Every switch keeps a fallback** when the file is missing.

## 0. Redraw two icons first
Use your image generation skill: `icon_play.png` and `icon_pause.png` in `Textures2D/UI/Kit/` are solid black, while
`icon_speed.png` / `icon_skip.png` are a thick ink outline with a pale paper fill. Redraw play (one right triangle)
and pause (two vertical bars) **in exactly the outlined paper-fill style of `icon_speed.png`** (look at it), no text,
transparent background, trimmed like the others (~2% padding). Overwrite the two files.

## 1. Battle screen (group E) — `Editor/Building/ReplayPrefabBuilder.cs` and replay views
- Khí Huyết / Cương Khí / Nội Lực bars: `bar_frame` around, `bar_fill_hp` / `bar_fill_shield` / `bar_fill_qi` as the
  fill images (keep the fill logic; switch Image type to Filled or keep the current sizing method, whichever the view
  uses).
- `fighter_panel` behind each side's panel; `timeline_bar` for the replay timeline; `vs_emblem` in the middle between
  the two sides (small, behind nothing important; show it at battle start and keep it faint during the fight, or
  just static if animation is not already supported).
- Replay controls: `icon_play` / `icon_pause` / `icon_speed` / `icon_skip` on the existing control buttons (keep labels
  like "x2" readable; icon left of label or icon only with a tooltip if the button already has one).

## 2. Result popup (group F)
`result_victory` / `result_defeat` behind the result title (pick by outcome at runtime), `reward_slot` behind each
reward entry, `star_burst` behind earned stars if stars are shown.

## 3. Tutorial and encyclopedia (group G)
- `TutorialOverlay`: replace the yellow rectangle highlight with `tutorial_ring` stretched around the target (keep
  the dark shade with the hole), and add `tutorial_pointer` pointing at the target from the side with most room;
  a gentle bob animation is welcome if cheap (unscaled time).
- Encyclopedia category tabs: `encyclopedia_icon_items` / `_status` / `_story` / `_rules` next to the matching
  category names (match by category; unknown categories get no icon).

## Must not touch
LoginScene, GameController loading frame, `Map_hoi_1`. No gameplay logic changes. Do not undo part 1.

## Check
- Existing EditMode tests for replay/tutorial must still compile (`Tests/EditMode/Replay`, `Tests/EditMode/Onboarding`).
- `py tools/lint/check_style.py <every C# file you changed>` until clean; `dotnet build` the generated `.csproj`.
- **Do not launch Unity.** No git.

## Report (`-o` output)
Files changed, where each sprite is used, which builder menus the user must run (Overwrite), anything skipped.
