# Codex task: redesign the battle screen layout (HUD in the top banner)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Read `Assets/_Game/Editor/Building/README.md`.

## Today
- Builder: `Assets/_Game/Editor/Building/ReplayPrefabBuilder.cs` (`BuildScreen`, `BuildFighter`, `Bar`, `Status`,
  `Fighter`), prefabs `Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab` and `FighterPanel.prefab`.
- View: `Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs` (title, portrait, Khí Huyết / Cương Khí /
  Nội Lực bars + texts, status icons), `ReplayScreen.cs`.
- Current layout (see `One-Bag-Jianghu/Logs/Snapshots/GameplayScene.png`): a thin top strip with the mode title,
  timer and "Thoát"; on each side a tall panel with name, portrait, the three bars and status icons; the two bag
  grids in the middle; replay controls + battle log at the bottom.

## Goal (user request)
"Đưa máu, thông tin… lên top banner, bên dưới chỉ hiển thị avatar và tên."
- **Top banner** (full width, ~150–170 px tall, ink/paper style: use `Kit/fighter_panel` or `home_banner` style
  pieces already in `Textures2D/UI`): 
  - left half = player HUD, right half = enemy HUD, **mirrored** (enemy bars fill from right to left);
  - per side: name, a large Khí Huyết bar (`Kit/bar_frame` + `Kit/bar_fill_hp`) with "Khí Huyết x/y" text,
    Cương Khí shown as a thinner bar right under it (`bar_fill_shield`), Nội Lực bar (`bar_fill_qi`), and the
    status icon row (all existing statuses, same behavior);
  - center: timer, a small `Kit/vs_emblem` behind/under it, and the mode title in small text;
  - "Thoát" button stays reachable (top-right corner, compact).
- **Side panels below** keep only the **portrait (avatar) and name**, a bit larger than now, framed as today.
  The hit-shake and other feedback that targets the portrait keeps working; floating damage/heal numbers must still
  appear near the correct side.
- Bag grids: make the battle bag grid cells use `Kit/slot_cell` (with fallback to the current translucent cells).
  Grids may move down/resize slightly to make room, but must stay fully visible at 1920x1080 with the bottom
  controls; items and their overlays (cooldown, SEAL/SLOW/HASTE, tooltips) must still line up with cells.
- Keep the bottom replay controls and log as they are.

## How
- Prefer keeping `FighterPanelView` as the single view per side, with its bar/status references pointing into the
  banner and its portrait/title references into the side panel; or split into a HUD view + portrait view if
  cleaner. Either way `ReplayScreen` must bind both sides as today; keep serialized field names stable where tests
  use them.
- Every new sprite keeps a fallback when missing.
- Update/extend EditMode tests under `Tests/EditMode/Replay` if they reference moved objects; all must compile.

## Must not touch
LoginScene, GameController loading frame, `Map_hoi_1`, gameplay/battle logic, the engine.

## Check
- `py tools/lint/check_style.py <every C# file you changed>` until clean (the builder already has old dense lines;
  do not add new ones; you may clean the parts you rewrite).
- `dotnet build` the generated `.csproj` files: 0 errors.
- **Do not launch Unity** (the leader rebuilds and takes screenshots). No git.

## Report (`-o` output)
Files changed, the new layout (positions/sizes), how views are bound, test changes, menus to run.
