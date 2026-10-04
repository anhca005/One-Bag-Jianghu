# Codex task: layout fixes for the Home banner and Settings popup (round 2)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity; Claude rebuilds and takes snapshots.

**Read `docs/tech/code-style.md`** (mandatory). **No git.** Do not edit `.unity`, `.prefab` or `.asset` files by hand. Do not touch the LoginScene build path.

## What the snapshots show
Snapshots are in `One-Bag-Jianghu/Logs/Snapshots/`: `HomeScene.png` and `SettingsPopup.png`, taken at 1920×1080. Look at them.

**Home banner (`Editor/Building/LoginHomeSceneBuilder.cs` `BuildHomeScene` and helpers, `Scripts/UI/Home/HomeController.cs`)**

1. **The currency icons are white squares.**
   - **Cause:** `CurrencyEntry` calls `UiBuild.ApplyUiSprite(icon, "icon_bac")`, which looks in `Textures2D/UI`, but the currency icons live in `Textures2D/Pve`.
   - **Second bug:** at runtime, `HomeController.SetCurrency` → `CurrencyArt.Apply(label, key)` does not find the builder's "Icon" sibling. That only works when the label's parent has a `HorizontalLayoutGroup`. So it creates a second 26×26 "Currency Icon" under the label.
   - **Fix:**
     - In the builder, load the sprite the same way `CurrencyArt` does: `PveArtLibrary`, then `ItemIconLibrary`, by key. A small editor helper is fine, or load the asset at `Assets/_Game/Textures2D/Pve/<key>.png`.
     - In `HomeController`, set the sprite of the existing sibling "Icon" image instead of calling `CurrencyArt.Apply`. Add a small public helper in `CurrencyArt` such as `TryGetSprite(string key, out Sprite sprite)`; `Scripts/UI/Common/UiKit.cs` may be edited for that only.
     - Make sure no "Currency Icon" child is created under these labels.
2. **The level seal is too small to read ("Cấp 1" inside 52 px).**
   - Make the seal 60×60.
   - Show **only the number** in it: font 26, Paper colour, bold Title font, best-fit min 14.
   - `HomeController.RefreshProgress` sets `levelText.text = displayedLevel.ToString()`.
   - Put the full "Cấp N" in the EXP text instead: `Cấp N · EXP x/y`, or `Cấp N · EXP Tối đa` at max level.
   - Update any test that expects the old strings.
3. **The EXP bar is tiny** because `preserveAspect` shrinks the 6:1 `progress_bar` art inside 300×30.
   - Size the bar 312×52, the art's aspect, with `preserveAspect` off.
   - Keep the fill area anchors (0.12–0.89 × 0.30–0.71).
   - The EXP text overlays the whole bar: font 15, Ink colour, `SingleLineFit`, min 10.
4. **The banner is too thin for its content;** the avatar sticks out of the paper band.
   - Make the header 132 px tall and set `pixelsPerUnitMultiplier = 390f / 132f`.
   - Vertically centre the left group inside the paper band, which is roughly the middle 64% of the height:
     - avatar 100×100 at x=64, y=16;
     - name at x=180, y=18 (28 pt);
     - seal at x=180 + name width, or a fixed x=440, y=14;
     - EXP bar at x=180, y=62.
   - The currency row and settings button are vertically centred in the 132 px header. Settings is 76×76 at the right, clear of the right scroll roller (the roller occupies about the last 60 px).
   - Update `TopStretch` calls and the test if it checks the height.
5. **The hero name** has a stray cloud ornament overlapping its left edge. Moving it to x=180 should clear it; make sure nothing overlaps.

**Settings popup (`Editor/Building/PopupPrefabBuilder.cs` `BuildSettings` + `Scripts/UI/Popups/SettingsPopup.cs`)**

6. **Rows are misaligned and cramped.** The Hiển thị dropdown labels sit right-aligned in the middle while the toggles are on the left. The Tài khoản buttons overlap the divider. The bottom buttons touch the frame.
   - Make the panel 960×900.
   - Use **one two-column grid for every row**:
     - label column at x 8%–40% of the panel width, left-aligned, 24 pt;
     - control column at x 44%–86%;
     - value column (the `%` text) at x 87%–94%.
   - **Row height 54, section header height 48, 12 px gap before each header.**
   - **Toggles** sit in the control column, left-aligned, 40×40. The toggle label goes in the **label column**, like every other row. Example: label "Toàn màn hình", toggle on the right.
   - **Dropdowns** fill the control column.
   - **Tài khoản:**
     - one row "Hiệp khách" with the name in the control column;
     - a row holding the two buttons "Về màn đăng nhập" and "Thoát game", side by side inside the control column, each 200×58, under the divider.
   - **Bottom row** "Mặc định" and "Đóng", each 220×64, centred at 34% / 66% of the width, 48 px above the bottom edge.
7. **Sliders have no handle,** so they look like flat grey boxes.
   - Add a handle to the common Slider in `Editor/Building/CommonUiPrefabBuilder.cs` `ConfigureSlider`:
     - a `Handle Slide Area` with a 28×28 Vermilion knob;
     - assign `slider.handleRect`;
     - the fill keeps working.
   - Track height in Settings rows: 22 px, centred in the row.
   - Only Settings uses the common Slider. Grep to confirm; if something else uses it, make sure it still looks right.
8. **Toggles are plain white squares.**
   - In `ConfigureToggle`, give the background Image the colour Paper darkened (Ink at 18% alpha over Paper, like the slider track) and a 2 px Ink outline (`Outline` component).
   - The checkmark stays Vermilion with 8 px inset.
9. **Dropdown captions are empty** in the snapshot. That is expected, because `OnSetup` did not run. Leave it, but make sure `OnSetup` fills the options and caption. In `SceneSnapshot`, capture the Settings popup **after** calling its setup path, if the other popups are captured that way.

## Allowed files
- `Editor/Building/LoginHomeSceneBuilder.cs` (Home only);
- `Editor/Building/PopupPrefabBuilder.cs` (`BuildSettings` only);
- `Editor/Building/CommonUiPrefabBuilder.cs` (`ConfigureSlider`, `ConfigureToggle` only);
- `Editor/Building/SceneSnapshot.cs`;
- `Scripts/UI/Home/HomeController.cs`;
- `Scripts/UI/Popups/SettingsPopup.cs`;
- `Scripts/UI/Common/UiKit.cs` (the `CurrencyArt` helper only);
- `Tests/EditMode/**`.

**`UiBuild.LabelFitPreset.None`** now exists: it adds shrink-to-fit and keeps the label's rect.

**Lint:** run `py tools/lint/check_style.py <changed files>` from `D:/CONG VIEC/One-Bag-Jianghu`.
- New or changed code must be clean.
- Pre-existing violations elsewhere in the big builder files are out of scope.

## Report
Write to the `-o` output: files changed, decisions made, and the lint result for the code you touched.
