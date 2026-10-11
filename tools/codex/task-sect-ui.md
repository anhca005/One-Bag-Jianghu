# Codex task: redesign the Môn Phái (sect) popup

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). UI text in
Vietnamese, code in English. **Read `docs/tech/code-style.md` first and follow it** (one statement per line, ≤ 120
chars, no one-line `if`/`for` bodies). Use game terms from `docs/design/glossary.md`.

## Today
- `Scripts/UI/Popups/SectSelectPopup.cs` + builder `Editor/Building/EconomyPopupPrefabBuilder.cs` → `BuildSect`:
  a dropdown and a plain text block that lists raw ids ("flute, spear, brush, bow"). Ugly and English.
- `HomeController.OpenSect()` opens it only before joining; after joining it shows a MessagePopup.
- Data: `Resources/Data/sects.json` (7 sects: id, name, tamPhap, weaponClasses, levels 1/3/6/9 with bonuses),
  `Scripts/Bag/SectCatalog.cs`, `SectChoiceService.cs`; the player's level is `profile.TamPhapLevel` (1–9).
- Lore (one line per sect): `docs/design/story-bible.md` "Cửu Châu và Bảy Đại Môn Phái".
- Bug: the sect tutorial (`Resources/Data/tutorial.json`, sequence `sect`) waits for events `sect_opened` and
  `sect_selected_or_closed`, but nothing publishes them, so the hint line stays on screen.

## Art (drawn by another Codex run at the same time; may be missing while you work)
`Assets/_Game/Textures2D/Sects/sect_<id>.png` (1536x1024 landscape, subject centered) and
`Assets/_Game/Textures2D/Sects/tam_phap_<id>.png` (1024x1024). See `docs/art/prompts-sects.md`. Load them through a
small asset the builder fills (follow the `PveArtLibrary` pattern: ScriptableObject under `Resources/`, keys
`sect_<id>` / `tam_phap_<id>`), and make sure those textures import as Sprites (check how `Textures2D/Pve` is set up,
e.g. an AssetPostprocessor, and extend it to `Textures2D/Sects`). **Every image must have a fallback** (ink-tinted
panel with the sect's accent color) so the popup works with no art at all.

## Build: one "Môn Phái" popup, two modes
Layout (popup ~1600x920, same frame/buttons/fonts as ShopPopup/StoragePopup):
- **Left column: 7 sect cards** stacked (art cropped to a strip + sect name + its weapon classes in Vietnamese,
  e.g. "Tiêu · Thương · Bút · Cung"; reuse the class-name mapping in `UI/Common/ItemDescriber.cs`). Selected card
  highlighted; the player's own sect marked "Bổn môn" after joining.
- **Right: detail of the selected sect**
  - big sect art at the top with the sect name over a soft gradient;
  - the lore line;
  - **"Binh khí sở trường"**: the weapon classes;
  - **Tâm Pháp panel**: the tâm pháp image, its name, current level ("Tầng 3/9" after joining, nothing before), and the
    four milestone rows (Tầng 1, 3, 6, 9) each with a readable Vietnamese description of its bonuses (attackPct →
    "+10% Lực Đạo", cooldownPct, crit, ultimateCost, extra ultimate/basic effects, triggers). Reuse
    `ItemDescriber` helpers for effects/triggers where they fit; unlocked rows normal, locked rows greyed.
- **Bottom buttons**
  - before joining (level ≥ 10, no sect): "Gia Nhập" (opens a ConfirmPopup "Lựa chọn này là vĩnh viễn. Gia nhập
    <tên phái>?", then `SectChoiceService.TryChoose`) + "Để sau";
  - after joining: viewing another sect shows no join button; viewing own sect shows **"Nâng Tâm Pháp"** disabled
    with the label "Sắp mở" (tâm pháp upgrades come later: keep a clear method/hook for it, no logic now) + "Đóng".
- `HomeController.OpenSect()`: level < 10 keeps today's message; otherwise always opens this popup (own sect
  preselected after joining).
- Tutorial: publish `OnboardingRuntime.Publish("sect_opened")` when the popup opens and
  `"sect_selected_or_closed"` when it closes (joined or not). Keep a `TutorialTarget` "sect_choices" on the card
  column.
- Builder: rebuild `BuildSect` (keep the prefab name/path `Resources/UI/Popups/SectSelectPopup.prefab` or rename the
  class consistently), add a menu item "One Bag Jianghu/Build/Sect Popup (Overwrite)" like the existing
  "Story Popup (Overwrite)". Must not touch LoginScene, GameController loading frame or `Map_hoi_1`.
- Text must fit: no overflow at 1920x1080 (use best-fit / fixed sizes like other popups).

## Tests
EditMode tests (new file under `Tests/EditMode/`): sect level description strings for each sect are non-empty and
contain no raw English ids; joining from the popup flow sets `SectId`/`TamPhapLevel`; the own-sect view has the
upgrade button disabled.

## Rules
- **No git.** Do not launch Unity (the user has it open); the leader builds and runs tests afterwards. Do not write
  PNG files or `.meta` files for the art.
- Files you may touch: `SectSelectPopup.cs` (or its replacement), `SectCatalog.cs` (add fields if needed),
  `HomeController.cs` (`OpenSect` only), `EconomyPopupPrefabBuilder.cs` (`BuildSect` and helpers), the art library
  script + its builder/postprocessor, new test file, `docs/design/` note if you add data fields.
- Run `py tools/lint/check_style.py <every C# file you changed>` until clean.

## Report (`-o` output)
Files changed, how art is loaded/fallback, how to rebuild the prefab, anything left undone.
