# Codex task (experiment only, round 3): fix the chibi cut-out rig

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/`. Code in English. **Read
`docs/tech/code-style.md`.** Use your built-in image generation (`image_gen`) and Pillow. Throwaway experiment: write
only under `One-Bag-Jianghu/Assets/_Sandbox/ChibiTest/` and `tools/codex/out/`. Nothing in `Assets/_Game/` changes.

## Round 2 result (`tools/codex/out/chibi_v2_preview.png`, report `chibi-test-2.md`): rejected
Halos are gone and the head/hair are good, but:
1. **Grey discs at every joint** (shoulder, elbow, hip, knee): the "overlap caps" were painted as grey circles.
2. **Rectangular blocks** appear when legs bend: some parts have opaque rectangular areas / hard rectangular crops.
3. **Sword blade cut off**: only the hilt shows; in the attack pose it points the wrong way.
4. **Arms far too long** for a chibi body; the rest pose looks like reaching forward.
5. Separately drawn parts do not share scale/colour exactly.

## Approach this time (consistency first)
1. Generate **one** full-body chibi **T-pose** image on a plain flat background (then make it transparent): same
   design as `Art/v2` head/hair (keep `Art/v2/head.png` and `hair_tail.png` if they fit), arms out level and
   **short** (chibi: hand reaches about mid-thigh when hanging; upper arm ≈ forearm ≈ 0.6× head height), legs short
   and slightly apart, sword **not** in hand. Check proportions before going on; regenerate if wrong.
2. **Cut all limb parts out of that one T-pose image** (Pillow masks along the joint lines), so scale, colour and line
   weight match. Make each joint end **round by extending the part's own pixels**: grow the cut edge outward 10–15%
   by mirroring/smearing the sleeve or trouser texture under the neighbouring part, never by painting a separate
   cap. **No grey, no flat-colour discs.**
3. Draw the **sword** alone (full straight jian, blade fully inside the image, horizontal, hilt left), and the hat.
4. Alpha: every pixel outside the silhouette alpha 0; no opaque rectangles. Verify programmatically: for each part,
   the bounding box of alpha>0 must not touch the image border on more than the joint side, and corners of the
   image must be alpha 0. Defringe as in round 2.
5. Update `Art/v3/` + `rig.json` (pivots on the joint centres) and `Editor/ChibiTestBuilder.cs` to use v3; keep the
   clip timings, re-key angles so poses read clearly at chibi proportions: Run (knees bend, arms swing ±35°), Jump
   (crouch, tuck, land), Attack (sword raised behind the head, then a diagonal slash down-forward ending in front of
   the body, blade fully visible, then recover).
6. **Self-check loop (required):** render `tools/codex/out/chibi_v3_preview.png` (rest, run contact, run passing,
   jump tuck, attack wind-up, attack slash; on paper and on black; canvas large enough that nothing is clipped).
   Look at it. If any of problems 1–5 is still visible, fix and render again (up to 3 iterations). Report honestly
   what is still wrong.

## Check
- `py tools/lint/check_style.py <your C# files>` until clean; `dotnet build Assembly-CSharp-Editor.csproj` and
  `Assembly-CSharp.csproj`. Keep the Input System controller.
- **Do not launch Unity. No git.**

## Report (`-o` output)
Iterations done, what changed each time, remaining defects (be specific), parts table, menu to run.
