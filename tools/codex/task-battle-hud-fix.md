# Codex task: fix the battle screen after the user's review (uses the new group H art)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.**

Builder: `Assets/_Game/Editor/Building/ReplayPrefabBuilder.cs`; views `Scripts/Battle/Replay/View/` (`ReplayScreen.cs`,
`FighterPanelView.cs`, `ReplayItemView.cs`); tests `Tests/EditMode/UI/GameplayPrefabTests.cs`, `Tests/EditMode/Replay/`.
Current look: `One-Bag-Jianghu/Logs/Snapshots/Battle_mid.png` and `Battle_log.png` (1920x1080, mid-fight).

New art (just drawn, transparent, trimmed) in `Assets/_Game/Textures2D/UI/Kit/`, see `docs/art/prompts-ui-kit.md`
group H (#99–108): `hud_panel`, `gauge_frame_large`, `gauge_frame_small`, `status_slot`, `count_badge`, `log_panel`,
`button_exit`, `name_plate`, `clock_plate`, `button_square`. Add their import/9-slice entries next to the other Kit
entries (`Editor/ItemIconImporter.cs` border table, `UiBuild.SlicedPixelsPerUnit`); measure the ornament size on each
PNG with Pillow. Every use keeps a fallback when a file is missing.

## The user's list (fix all)
1. **Player bag grid overlaps the left avatar panel** (the enemy side does not). Lay out both sides symmetrically
   with a clear gap between avatar panel and grid at 1920x1080.
2. **Portrait: the black backing pokes out past the frame's corners.** The backing must stay inside the frame's
   inner opening (inset it, or clip it to the opening), so no corner sticks out.
3. **Remove the "Chơi lại" (restart) button** from the battle screen (keep the method if tests need it; just no
   button). Re-space the remaining controls.
4. **Battle log**: stop using the stretched button image as its background. Use `log_panel` for both the collapsed
   log and the expanded log; text must stay inside (collapsed: the 2 newest lines, wrap + truncate, padded clear
   of the corners; expanded: rows sized by the layout, readable). Clicking the collapsed log still toggles the
   expanded one.
5. **Status icons and the names overflow**: the chip cell size and the icon art do not match. Give each status a
   `status_slot` socket of a fixed size with the icon inside and the stack count on a `count_badge` at its
   bottom-right, all inside the HUD. Names in the HUD and on the avatar plate must stay inside their boxes (one line,
   shrink to fit; avatar plate may wrap to two lines).
6. **HP / Cương Khí / Nội Lực frames look warped** (they used the loading bar art): use `gauge_frame_large` for Khí
   Huyết and `gauge_frame_small` for Cương Khí and Nội Lực, sized to their real aspect so nothing stretches badly; the
   fills (`bar_fill_*`) sit inside the inner channel; value text readable (outline/shadow).
7. **Exit button hard to see**: use `button_exit` (round, ~64 px) with a small "Thoát" label under it, placed where
   it does not cover anything and is easy to spot.
8. **Clock and "Hành Tẩu Giang Hồ" hard to read**: put them on `clock_plate` and give both texts an `Outline`
   (dark) in addition to the shadow.
9. HUD backgrounds: use `hud_panel` instead of the stretched `fighter_panel`; avatar name strip uses `name_plate`.
10. Small square controls in this screen (speed buttons if they are short labels like "x2") may use
    `button_square` if it reads better; keep the play/pause button as is.

## Check
- Update the EditMode tests that assert the hierarchy (restart button removed, new objects); all must compile.
- `py tools/lint/check_style.py <every C# file you changed>` until clean; `dotnet build` the generated `.csproj`
  files (if a `.csproj` lists a missing temp file `BatchUiKitRebuild.cs`/`BatchReplayCapture.cs`, ignore it or use a
  stub you delete afterwards).
- **Do not launch Unity**; the leader rebuilds and screenshots. No git. Do not touch LoginScene, the loading frame,
  `Map_hoi_1`, gameplay logic.

## Report (`-o` output)
Files changed, the new positions/sizes, the 9-slice entries added, test changes, anything skipped.
