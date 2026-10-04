# Task O: polish the new battle screen ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Task L built the layout of decision #26 (`tools/codex/task-gameplay-layout.md`). Claude rendered it (`One-Bag-Jianghu/Logs/Snapshots/GameplayScene.png`, open it) and found these problems. Read `Editor/Building/ReplayPrefabBuilder.cs`, `GameplaySceneBuilder.cs`, `Scripts/Battle/Replay/View/*`, `Scripts/Battle/Replay/*` (replay state), `Scripts/UI/Gameplay/*`, tests under `Tests/EditMode/UI` and `Tests/EditMode/Replay`.

## Fix
1. **Bags too small, centre gap too wide**: bags are ~330 px wide with a ~690 px gap. Make each 5×4 bag about 560–620 px wide (square cells), keep a centre gap ≈ 260–320 px, keep avatars fully on screen at 1920×1080 and 16:10 (1920×1200 / 1680×1050 — CanvasScaler match 0.5).
2. **Readability**: stage name (top-left), fighter names above portraits and bar labels are hard to read over the painted background. Give the top bar and the avatar blocks a readable backing (paper panel with ~85% opacity or a soft dark ink plate with light text), text outline/shadow, font sizes ≥ 22 for names, ≥ 18 for bar numbers. Bars: label + "current/max" number inside or beside the bar.
3. **Portrait art**: portrait keys by priority — player: `hero_portrait` → `hero_idle`; enemy: `enemy_portrait_{n}` (n = 1..3, stable per opponent, e.g. hash of the opponent name) → `hero_idle` mirrored + ink tint. Set the sprite at build time when the art exists (so it shows in the Editor) and at runtime for the enemy choice. Missing art → a plain ink silhouette placeholder, never an empty frame.
4. **Bottom bar**: speed/pause/replay buttons are small ornate buttons (≈ 130×56) — make them ≈ 170×64 with readable labels, the active speed highlighted. The collapsed log shows the last 3 lines in light text on the dark strip, font ≥ 18; clicking expands a scrollable panel (≈ 40% height) above the bottom bar.
5. **Hồi Phục (regen) chip**: task L said regen could not be reconstructed on seek. You may now extend the replay state (`Scripts/Battle/Replay/**`) to track regen from STATUS/HEAL events so the chip shows correct stacks, also after seeking/restart.
6. Status chips: icon from `Textures2D/Status` (`status_*`), stack number, max 6 chips per side, wrap to a second row.

## Hard rules
- Modify only: `Editor/Building/ReplayPrefabBuilder.cs`, `Editor/Building/GameplaySceneBuilder.cs`, `Scripts/Battle/Replay/**` (not `View/VfxLab/`, not `VfxTuning.cs`), `Scripts/UI/Gameplay/**`, `Tests/EditMode/UI/Gameplay*`, `Tests/EditMode/Replay/**`. Another agent is editing Economy/Popups/Home in parallel — do not touch them.
- Scenes stay flat at the root with a Main Camera (decision #26 addendum); UI only from builders. English code, Vietnamese UI strings. C# 9. No git. You cannot run Unity; re-read for compile errors (CS0165; fully qualify `UnityEngine.UI.Image.Type` in builders).

## Tests
Keep all tests green; add: bag rect widths within the target range, portrait image never null, regen chip after seek. Finish with: files changed, builder menu items Claude must run (order), anything unsure.
