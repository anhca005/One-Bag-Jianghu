# Codex task: show battle rewards as pictures in the result popup

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.**

## Today
- `Scripts/UI/Popups/ResultPopup.cs` (+ `ResultPopupData`) shows rewards as one text line, e.g.
  "★★★  Thưởng: 45 Bạc · 8 EXP" (+ "Lên cấp! Cấp 13"), built in `Scripts/UI/Gameplay/GameplayController.cs`
  (~line 300) from `PveRewardResult` (`Scripts/Pve/PveRewardService.cs`: Silver, HeroExp, Materials, Old/NewLevel).
- Builder: `Editor/Building/PopupPrefabBuilder.cs` → `BuildResult` (popup 780x600). The middle box ("Reward Slot",
  `Kit/panel_inset`) now holds only that text and looks empty; the user wants pictures there.

## Do
- Extend `ResultPopupData` with a structured reward list (keep the text constructor for PvP/other callers):
  stars earned (0–3), and entries `{ icon sprite key, amount text, tooltip/name }`.
- In PvE (`GameplayController`), fill it from `PveRewardResult`: Bạc (`Textures2D/Pve/icon_bac.png`, the same icon
  Home uses), EXP (`Kit/icon_exp`), each material dropped (its icon from the item/material icon library used by the
  storage/shop UI) with "x N". Level-up stays a short text line ("Lên cấp! Cấp 13").
- Popup layout inside the middle box:
  - top row: 3 star images (`Textures2D/Pve/icon_star.png` earned / `icon_star_empty.png` not earned), ~44 px,
    centred; `Kit/star_burst` faintly behind earned stars;
  - below: a centred horizontal row of reward cards, each a **square** `Kit/reward_slot` (~84x84, never stretched
    non-uniformly) with the icon inside and the amount under it (ink text, readable); up to ~6 cards, wrap or
    shrink if more;
  - the level-up line under the cards; the old text field stays as fallback (used when the list is empty, e.g.
    PvP or "Thưởng chưa nhận: …" errors).
- Every sprite keeps a fallback when missing. Keep the tutorial target on the home button and the existing button
  behaviour.
- EditMode test: building the data from a sample `PveRewardResult` gives the right entries (silver, exp, materials,
  stars), and the popup shows one card per entry.

## Check
- `py tools/lint/check_style.py <every C# file you changed>` until clean; `dotnet build` the generated `.csproj` files.
- **Do not launch Unity.** No git. Do not touch LoginScene, the loading frame, `Map_hoi_1`, the replay builder.

## Report (`-o` output)
Files changed, the layout, the menu the user must run (Popup Prefabs (Overwrite)).
