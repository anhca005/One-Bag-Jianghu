# Codex task (experiment only, round 2): redraw the chibi hero as clean cut-out parts

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/`. Code in English. **Read
`docs/tech/code-style.md`.** Use your built-in image generation (`image_gen`). Throwaway experiment: write only under
`One-Bag-Jianghu/Assets/_Sandbox/ChibiTest/` and `tools/codex/out/`. Nothing in `Assets/_Game/` may change.

## What went wrong in round 1 (`tools/codex/out/chibi-test.md`)
Parts were cut out of one finished painting (`Art/chibi_full.png`, dark background): overlapping pieces came out
incomplete (the sword hand covered the torso, hair covered the shoulders), every part kept a dark/grey halo from the
background, and in the scene the sword and front arm cover most of the body. The user's screenshot shows a blob.

## Do
1. **Art, redrawn for cut-out animation** (keep the same character design as `Art/chibi_full.png`: chibi, head ~40%
   of height, long dark hair in a high ponytail with red ribbon, grey-blue robe with white layers, red sash, straw hat
   on the back, dark trousers, red-wrapped boots; Thủy Mặc ink style; side view facing right):
   - first a **T-pose reference** on a **transparent** background: arms held out level, legs slightly apart, no
     sword in hand, nothing overlapping;
   - then **each part as its own image, drawn whole** (also the parts normally hidden behind others), transparent
     background, same scale and light: `head` (face + hair mass, no ponytail), `hair_tail` (ponytail + ribbons),
     `torso` (robe body + sash, without arms; skirt of the robe included), `upper_arm_front`, `forearm_front` (with
     hand, open grip), `upper_arm_back`, `forearm_back`, `thigh_front`, `shin_front` (with boot), `thigh_back`,
     `shin_back`, `sword` (straight jian, horizontal, hilt left), `hat` (straw hat seen from behind, on the back);
   - every joint piece has **extra overlap** at the joint end (round cap) so rotation never shows a gap;
   - **clean alpha**: no halo. After removing any background, erode/defringe the edge (e.g. remove pixels whose colour
     is close to the old background, then 1 px alpha erosion) and check on both a white and a black background;
   - consistent pixel scale across parts (head about 420 px wide at source).
   Save to `Art/v2/` (keep round 1 files). Write `Art/v2/rig.json`: per part the pivot (normalized), the parent part,
   the local offset of its pivot relative to the parent's pivot (in source pixels), and sorting order.
2. **Rig and animations**: update `Editor/ChibiTestBuilder.cs` to build the hero from `Art/v2` + `rig.json`, with
   the two-bone arms/legs (upper → lower), sword parented to the front hand. Sorting (back to front): hat, back
   arm, back leg, hair_tail, torso, head, front leg, front arm, sword (sword behind the front hand's fingers if you
   can split, otherwise just above the forearm). Re-key the four clips for the new rig: `Idle` (breath, hair and
   ribbon sway), `Run` (thigh/shin and upper/forearm swing with knee/elbow bend, body bob, lean), `Jump` (crouch with
   knee bend, rise, tuck, land), `Attack` (raise the sword behind, fast diagonal slash with the arm extending, follow
   through, recover). Character about 3 units tall, standing on the ground line, centred.
3. Save a preview sheet `tools/codex/out/chibi_v2_preview.png`: the assembled hero in rest pose plus 3 key poses
   (run contact, jump tuck, attack slash) composed with Pillow from the parts using `rig.json`, on paper and on
   black, so the leader can check halos and joints without Unity.

## Check
- `py tools/lint/check_style.py <your C# files>` until clean; `dotnet build Assembly-CSharp-Editor.csproj` and
  `Assembly-CSharp.csproj` (the controller already uses the Input System; keep it).
- **Do not launch Unity. No git.**

## Report (`-o` output)
Parts (size, pivot), what you did against halos, the clips, the menu to run (same: "One Bag Jianghu/Sandbox/Build
Chibi Test Scene", which must overwrite the old scene/clips).
