# Codex task: redo the Giang Hồ Bách Khoa popup layout

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/Onboarding/EncyclopediaPopup.cs`;
- `BuildEncyclopedia` and `AddScrollColumn` in `Editor/Building/PopupPrefabBuilder.cs`;
- `Resources/Data/encyclopedia.json`: 70 entries, each with a category;
- the shared UI kit: `UiBuild`, `UiKit`, `UiFonts` / `UiFontRole`, `UiButton`, `Button_Secondary.prefab` and the tab
  style used by `ShopPopup` or `StoragePopup`. Reuse what exists.

## Bug (user screenshot)
- **Buttons are ~50 px wide:** category and entry buttons are created at runtime from a raw `GameObject` with no
  `LayoutElement` or width, so the labels are clipped ("Hệ thống" shows as "Hệ / ống").
- **Wrong font:** they use Unity's default Arial instead of the game font.
- **Empty columns:** most of each column's width is blank.

## Do
1. **Layout** (popup size can stay 1180×820):
   - **Left column, categories:** full-width rows, one line each.
   - **Middle column, entries of the selected category:** full-width rows, scrollable, mouse wheel works.
   - **Right, detail:** the entry name as a heading, then the description in body text, top-aligned.
   - All text uses the game fonts through the existing font roles: Display for the title, Body for the rest. Ink color
     on paper.
2. **Rows:**
   - Built from a prefab or a builder-made template inside the popup, not raw runtime objects. The look must be
     consistent with other list rows in the game.
   - Each row has a `LayoutElement` with a fixed preferred height (about 52) and stretches to the column width.
   - Long names: one line with best-fit shrinking (`UiBuild.FitLabel` or `SingleLineFit`), never clipped.
3. **Selection:**
   - The selected category and the selected entry are highlighted (vermilion or gold tint, like the existing tabs).
   - Opening the popup selects the first category and its first entry.
4. **Click sound:** use the existing `UiSfx` click cue, or the `ui_click` call already there.
5. **Builder only:**
   - Change `BuildEncyclopedia` (and helpers it alone uses).
   - Add a menu item `One Bag Jianghu/Build/Encyclopedia Popup (Overwrite)` that rebuilds just this prefab.
   - Do not touch other popups.

## Tests (EditMode)
- Extend the existing onboarding tests if they cover the popup. Otherwise add one test: after setup, the category and
  entry rows have a width > 200 in the built prefab's layout, or at least a LayoutElement with flexibleWidth > 0.
  Keep it simple.

## Rules
- **No git.** Never touch `LoginScene` or `BuildLoginScene`.
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; changed lines must be clean.

## Report (`-o` output)
- The files changed, the menu item, and the lint result.
