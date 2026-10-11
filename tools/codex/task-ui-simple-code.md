# Codex task: switch the UI to Simple images (phase 3 of decision #66)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English.
**Read `docs/tech/code-style.md` and follow it.** Read decision #66 in `docs/decisions.md`. The user approved the art.

## Inputs
- `docs/art/ui-simple-plan.md`: the per-element decisions (resize / crop / new art / Filled / flat). It has new rect
  sizes and the frame-body insets for art with ornaments that stick out.
- `docs/art/ui-image-audit.md` and `tools/codex/out/ui-image-audit.csv`: every distorted use, including the 76
  class A uses (error ≤ 5%), which simply become Simple.
- The 25 new PNGs in `Assets/_Game/Textures2D/UI/Kit/` (listed in the plan). Unity has not imported them yet, so
  there are no `.meta` files. Do not create any.

## 1. Builders (`Assets/_Game/Editor/Building/*.cs`, `UiBuild.cs`)
- Every framed Image becomes `Image.Type.Simple` with the plan's sprite.
  - Remove `Image.Type.Sliced` and `Tiled` from all builders and from `UiBuild.ApplyUiSprite` defaults.
  - Remove the `pixelsPerUnitMultiplier` tables and the `SlicedAutoFit` usage.
  - Keep `SlicedAutoFit.cs` itself only if something else still needs it; otherwise delete the script **and** say so.
- Apply the plan's **new rect sizes** for resized elements.
- Where art has ornaments sticking out (portrait ribbon, plaque ropes, `bag_board` knots), place content inside the
  frame body using the plan's insets.
- **Fills** (slider fill, HP/qi/shield bar fills, progress): `Image.Type.Filled`, Horizontal, origin left, art
  drawn at the full bar aspect. If the fill art's aspect does not match its bar rect, list it in the report.
  Do not stretch it.
- **Scrollbars**: flat colour. Handle and track use a built-in white sprite (or none) tinted with the kit's ink
  colours. No kit art.
- **Fallback chains** (12, listed in the audit's "Fallbacks"): remove them all. `UiBuild.ApplyUiSprite` (and any
  similar helper) must `Debug.LogError` with the missing path when the sprite is not found and leave the image
  without a sprite. It must never substitute another sprite.
- **Text for localization**:
  - every Text inside a fixed frame (buttons, plaques, labels, tooltips, fields) uses best fit with a sensible
    min/max, so a ~30% longer string still fits;
  - frames do not resize to their text;
  - remove any ContentSizeFitter that drives a framed image's size, and replace it with a fixed size from the plan.
- `Editor/ItemIconImporter.cs`: the 9-slice border table is no longer needed for new art. Leave existing borders
  alone; do not seed borders for the new files.

## 2. Runtime code (`Assets/_Game/Scripts`)
Apply the same rules to every runtime site in the audit's "Runtime" section: no Sliced or Tiled, no borrowed
fallback sprites, and Filled for bars. This covers `UiKit`, `ItemTooltip`, `QuestTracker`, `UiBorderRunner`,
`ResultPopup`, `VfxLabBootstrap`, etc.
- `UiBorderRunner` draws a light path around a frame. If it relied on sliced geometry, keep it working on a
  Simple frame.
- `ResultPopup` reward cards are already Simple; keep them that way.

## 3. Hand-edited scenes: patch, do not rebuild
`LoginScene` and `HomeScene` are hand-edited and must **never** be rebuilt by their builders. Write
`Assets/_Game/Editor/Building/SimpleImageScenePatch.cs`:
- menu "One Bag Jianghu/Patch Scenes To Simple Images" and a static `RunBatch()`;
- it opens each of the two scenes and, **only for the objects named in the plan**, sets the Image to Simple, sets
  the plan's sprite, and sets the new size where the plan resizes. Examples: Login `Btn_Enter`, `HeroNameInput`;
  Home `Header`, `Stamina Tooltip/Paper`, `Hotspot_*/Plaque`. Also patch any other Sliced Image in these two scenes
  whose sprite is in the plan or in class A;
- it changes nothing else: no positions except the stated size, no other components;
- it saves the scene and logs every change as `scene :: path : old -> new`.

The other scenes (`GameplayScene`, `ReplayDemo`, `BagDemo`, `VfxLab`) contain prefab instances. List in the report
which of them still hold scene-only Sliced images after the prefabs are rebuilt, and patch those the same way in
the same script.

## 4. Art fix
In `Kit/button_replay_control.png`, the dark middle shows blocky light/dark seams from the tiled segment. Rebuild
it with your caps + middle builder:
- use a plainer, wider middle segment;
- cross-fade the joins;
- keep 680x256, full fill, no text.
Overwrite only this PNG.

## 5. Tests
Update `Tests/EditMode/UI/CommonUiPrefabTests.cs`, `StructuredPrefabTests.cs` and any other test that expects
Sliced. Add one EditMode test that scans the built prefabs under `Assets/_Game/Prefabs/UI` and fails when:
- an Image with a sprite under `Textures2D/UI` is Sliced or Tiled; or
- a Simple Image without `preserveAspect` has an aspect error above 5%.
Fills and flat images are exempt.

## Check
- Run `py tools/lint/check_style.py <every C# file you changed or created>` until it is clean.
- Run `dotnet build` on the Game, Editor and Tests projects. Use temporary csproj copies that include new files;
  delete the copies and their `obj/` caches afterwards.
- **Do not launch Unity.** In this sandbox it cannot license and fails anyway. The leader runs the builders.
- **No git commands that change the repo.** Do not edit `.prefab`, `.unity` or `.meta` files by hand. The
  builders and the patch script produce them.

## Report (`-o` output)
- the files changed;
- the **exact list of builder menus / `-executeMethod` calls to run, in order**, and which prefabs each rebuilds.
  These must not include LoginScene, HomeScene, the GameController loading frame or `Map_hoi_1`. Say if any
  builder would touch them;
- the fill art aspect mismatches;
- anything in the plan you could not apply.
