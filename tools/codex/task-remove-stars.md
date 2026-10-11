# Codex task: remove the star rating system entirely

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.**

The game designer decided: PvE stages no longer give 0–3 stars. A stage is just cleared or not cleared.

## Do (paths under `Assets/_Game/`)
- `Scripts/Pve/PveProgress.cs`: delete `PveRules.Stars`.
- `Scripts/Economy/PlayerProfile.cs`: keep the `bestStars` JSON field on `PveStageProgress` so old saves still load,
  mark it `[Obsolete]`-free but unused (comment "legacy, unused"); drop it from validation (any value is accepted).
  Nothing may read or write it any more.
- `Scripts/UI/Gameplay/GameplayController.cs`: no star computation, no `BestStars`, no "star_rating" sfx, no stars in
  the reward data or the log line.
- `Scripts/UI/Popups/ResultPopup.cs` (+ `ResultPopupData`): remove `Stars`, the stars row, `CreateStar`,
  `star_burst`. The reward cards now use the space the stars row had (centre them vertically in the middle box,
  level-up line under them).
- `Editor/Building/PopupPrefabBuilder.cs` `BuildResult`: remove the "Stars" object; let the cards area take its
  place.
- `Scripts/UI/Pve/PveStageNode.cs`: `Bind` takes `bool cleared` instead of `int stars`; the node colour uses
  `cleared`; the star icons and stars text are always hidden (keep the serialized fields so existing prefabs, which
  the user hand-edits, still load; just `SetActive(false)` them if present).
- `Scripts/UI/Pve/StageMapPopup.cs`: pass `progress.Cleared`. The recommended-level icon uses `icon_star`; switch it
  to a non-star icon that already exists (e.g. `Kit/icon_exp` or the level icon Home uses), with fallback.
- `Editor/Building/PvePrefabBuilder.cs`: stop creating the "Stars" text and "Star 1..3" images for new nodes.
  **Do not run it, and do not touch any map prefab / `Map_hoi_1`.**
- `Scripts/UI/Popups/DevCheatPanel.cs`: "Qua ải 3 sao" becomes "Qua ải" (marks Cleared only).
- The "star_rating" sfx: remove it from wherever it is registered/used (keep the audio file); update
  `Tests/EditMode/Audio/SfxSystemTests.cs`.
- Tests: remove `PveSystemTests.StarsFollowRemainingHp`; update `PveArtWiringTests`, `ResultPopupRewardTests`,
  `Tests/PlayMode/DevWipeSmokeTests.cs`, `Tests/PlayMode/PveThemeSmokeTests.cs` (no `BestStars`). Add one EditMode
  assertion that the result popup shows no star objects.
- Grep the whole project (C#, JSON in `Resources/Data`, tutorial texts) for any other star wording ("sao", "★",
  "Stars") tied to stage rating and remove it. Do not touch unrelated words (e.g. "start", "status").

## Check
- `py tools/lint/check_style.py <every C# file you changed>` until clean; `dotnet build` the generated `.csproj`
  files.
- **Do not launch Unity.** No git. Do not touch LoginScene, the loading frame, `Map_hoi_1`, the replay builder, any
  `.prefab`/`.unity` file.

## Report (`-o` output)
Files changed, what remains of `bestStars` and why, the menu the user must run.
