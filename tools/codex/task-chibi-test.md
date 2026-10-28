# Codex task (experiment only): chibi hero + idle/run/jump/attack animations in a new TestScene

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, URP 2D, UGUI).
Code in English. **Read `docs/tech/code-style.md` and follow it.** Use your built-in image generation (`image_gen`).

**This is a throwaway experiment. Nothing in the game may reference it or change because of it.** Put every file
under `Assets/_Sandbox/ChibiTest/` (create it). Do not touch anything under `Assets/_Game/`, Build Settings, project
settings, other scenes or prefabs.

## 1. Art: the hero, chibi, as cut-out parts
Reference: the main character `Assets/_Game/Textures2D/UI/hero_idle.png` and `hero_portrait.png` (young swordsman,
long dark hair tied up, grey-blue robe, red sash, straw hat on the back, sword). Same Thủy Mặc ink style as the
game (`docs/art/reference/01-style-board-thuy-mac.png`), but **chibi proportions** (big head ~40% of height, small
body), side view facing right.

Frame-by-frame sprites from image generation would not stay consistent, so draw the hero as **separate parts** for
cut-out animation, all in the same pose/scale, transparent backgrounds, trimmed:
`head.png` (with hair and hair tie), `hair_tail.png` (the ponytail, separate so it can sway), `torso.png` (robe and
sash), `arm_front.png`, `arm_back.png`, `leg_front.png`, `leg_back.png`, `sword.png`, `hat.png` (on the back,
optional). First generate one full chibi reference image (`chibi_full.png`), then generate/cut each part to match it
(you may cut parts out of the full image with Pillow and clean the edges, if that keeps them consistent). Each part
must have sensible pivots (shoulder for arms, hip for legs, neck for head, hilt for sword): record them.
Save under `Assets/_Sandbox/ChibiTest/Art/`.

## 2. Scene `Assets/_Sandbox/ChibiTest/TestScene.unity`
Create it with an editor script `Assets/_Sandbox/ChibiTest/Editor/ChibiTestBuilder.cs` and a menu item
"One Bag Jianghu/Sandbox/Build Chibi Test Scene" (the user runs it; you do not launch Unity):
- a 2D orthographic camera, a plain light paper-coloured background and a ground line;
- the hero as a hierarchy of `SpriteRenderer`s (root → torso → head/hair_tail/arms/legs, sword in the front hand),
  sorting orders set so back limbs are behind;
- an `Animator` with a controller and **four `AnimationClip`s created by the script**: `Idle` (breathing bob, hair
  sway, loop ~1.2 s), `Run` (leg/arm swing, body bob, slight lean, loop ~0.5 s), `Jump` (crouch, rise, hang, land,
  ~0.9 s, root moves up and down), `Attack` (wind-up, fast sword slash ~120°, recover, ~0.6 s, back to Idle);
- a small UI (or keyboard: 1 Idle, 2 Run, 3 Jump, 4 Attack; Space = Jump; J = Attack) to switch states, with the
  current state name shown.

## Check
- `py tools/lint/check_style.py <your C# files>` until clean; `dotnet build` (temporary `.csproj` copy that includes
  your new files if Unity has not listed them; delete the copy and its `obj/` caches afterwards). If `Assets/_Sandbox`
  needs its own asmdef for the editor script, add `Assets/_Sandbox/ChibiTest/Editor/ChibiTest.Editor.asmdef`
  (Editor only).
- **Do not launch Unity. No git.** Write only under `Assets/_Sandbox/ChibiTest/` and `tools/codex/out/`.

## Report (`-o` output)
The parts drawn (size, pivot), a preview image of the assembled chibi saved to `tools/codex/out/chibi_preview.png`
(compose the parts with Pillow at their pivots), the clips and their timings, the menu to run.
