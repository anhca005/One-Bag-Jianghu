# Codex task: audit every UI Image that distorts its art (phase 1 of "all UI images Simple")

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English.
**Read `docs/tech/code-style.md` and follow it.** Decision #66 in `docs/decisions.md` is the goal: every framed UI
image shows its art as drawn (Image.Type.Simple, rect aspect = art aspect). Variable-length bars use Filled.
Missing proportions get their own new art later. No borrowed fallback sprites.

This phase **only measures**. Do not change any builder, prefab, scene or art yet.

## 1. Audit tool
Create `One-Bag-Jianghu/Assets/_Game/Editor/Tools/UiImageAudit.cs`:
- Add the menu "One Bag Jianghu/Audit UI Images" and a static `RunBatch()` for `-executeMethod`.
- For every prefab under `Assets/_Game/Prefabs` (recursive):
  - create a new empty scene in memory;
  - add a Screen Space Overlay Canvas with a CanvasScaler: Scale With Screen Size, reference 1920x1080, match the
    one the game uses (check `UiBuild` or the scenes);
  - instantiate the prefab under it, stretched to fill;
  - run `Canvas.ForceUpdateCanvases()` and `LayoutRebuilder.ForceRebuildLayoutImmediate` on the root (twice).
- For every scene in `Assets/_Game/Scenes`:
  - open it with `OpenSceneMode.Single`;
  - **never save**;
  - force the same layout pass.
- Record every `Image` whose sprite is a texture under `Assets/_Game/Textures2D/` and where at least one of these
  holds:
  - the type is Sliced or Tiled;
  - the type is Simple with `preserveAspect == false` and the aspect error is above 3%.

  Columns:
  - prefab or scene, and the GameObject path;
  - sprite asset path;
  - Image type, `preserveAspect`, `pixelsPerUnitMultiplier`;
  - rect width and height in canvas units at 1920x1080;
  - sprite pixel size;
  - rect aspect, sprite aspect, and the aspect error as `abs(ln(rectAspect / spriteAspect))` in %;
  - whether the rect size is driven by a LayoutGroup or ContentSizeFitter. A driven size is variable, so note it.
  - inactive objects too, flagged.
- Write `tools/codex/out/ui-image-audit.csv` and a grouped Markdown report `docs/art/ui-image-audit.md`. Group by
  sprite. For each sprite list its uses, with the rect sizes and errors.
- Errors: catch per prefab or scene, log them, and continue.

## 2. Code-created images (not in prefabs)
Some Images are made at runtime: for example `Scripts/UI/Popups/ResultPopup.cs`, `QuestTracker`, `UiBorderRunner`,
`ItemTooltip`, `UiKit`, `VfxLabBootstrap`.
- Grep `Assets/_Game/Scripts` for `Image.Type.Sliced`, `Image.Type.Tiled`, `.type =`, `pixelsPerUnitMultiplier` and
  `SlicedAutoFit`.
- List every site in the report under "Runtime": file:line, sprite key, the intended size if the code shows it.

## 3. Borrowed fallback sprites
- List every place where a builder or runtime script uses one sprite when another is missing, for example:
  - `if (!UiBuild.ApplyUiSprite(a, "Kit/x")) UiBuild.ApplyUiSprite(a, "Kit/y")`;
  - `result_victory` → `seal_title`;
  - `hud_panel` → `fighter_panel`;
  - `shop_card_wide` → `shop_card_frame`.
- Put them in the report under "Fallbacks": file:line, wanted sprite, fallback, and whether the wanted file
  exists.

## 4. Proposal (in the report, do not implement)
Classify each use:
- **A. Switch to Simple as is**: error ≤ 5%.
- **B. Filled**: progress, HP, qi or slider fills, where the art should be cropped, not stretched.
- **C. Needs new art**: give the target pixel size. Long side about 2–4x the canvas size, but at most 2048. Give
  a suggested file name.
  - Uses within 5% of each other share one new image.
  - A size driven by layout or content (tooltips, rows in a scroll list, buttons whose width follows text):
    propose fixing the size, or a small set of fixed sizes, and say which.
- **D. Unclear**: explain.

End with a summary table: sprite → class → new art files needed.

## Check
- Run `py tools/lint/check_style.py <your C# file>` until it is clean.
- Run `dotnet build` with temporary csproj copies that include the new file (Unity has not listed it), then delete
  the copies and their `obj/` caches.
- **Run the audit once in batch.** Unity is closed:
  `"<Unity 6000.3.9f1 Editor path>/Unity.exe" -batchmode -quit -projectPath "D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu" -executeMethod OneBagJianghu.Editor.Tools.UiImageAudit.RunBatch -logFile tools/codex/out/ui-audit.log`
  - Find the editor path from Unity Hub's install (for example `C:/Program Files/Unity/Hub/Editor/6000.3.9f1`).
  - Use the namespace convention of the other editor scripts.
  - If `tasklist` shows `Unity.exe` running, do not start a batch and do not kill it. Report instead.
- `git diff --stat` after the batch must show no changed `.prefab`, `.unity` or `.meta` files. If the batch changed
  any, say which. Do not revert them yourself.
- **No git commands that change the repo.** You may only create `UiImageAudit.cs` (and its `.meta` if Unity makes
  one), `docs/art/ui-image-audit.md` and the files in `tools/codex/out/`.

## Report (`-o` output)
- the counts: uses by class A/B/C/D, new images needed;
- the five worst distortions;
- the fallbacks found;
- anything that could not be measured.
