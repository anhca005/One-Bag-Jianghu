# Codex task F3: switch to the new wuxia font set (Charm / Alegreya SC / Alegreya)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Read first:
- `Scripts/UI/Common/UiFonts.cs`, `UiFontRegistry.cs`, `SingleLineFit.cs`;
- `Editor/Building/UiBuild.cs` (`FitLabel` presets) and every builder that calls `UiFonts`/`UiFontRole`: grep `UiFontRole.`;
- the builder that creates `Resources/UiFonts.asset`;
- `Tests/PlayMode/TextOverflowTests.cs`;
- `docs/decisions.md` #37.

**Builders only. No git. Do not run Unity**; Claude runs it.

## Decision (user, #43)
The font files are already in `Assets/_Game/Fonts/`, with their OFL licences:

| Role | Font | Use |
|---|---|---|
| **Display** (new) | `Charm-Bold.ttf` (brush calligraphy) | Big headings only: game title on Login, popup titles ("Bao Phục", "Tàng Bảo Các", "Thương Hội"…), chapter title/name on the map, stage name in the map detail, result popup headline (Thắng/Thua), dialogue speaker name. |
| **Title** | `AlegreyaSC-Bold.ttf` (carved small caps) | Buttons, plaques, tabs, section headers, HUD hero name and level. |
| **Body** | `Alegreya-Regular.ttf` | All running text: descriptions, tooltips, stats, logs, dialogue lines, numbers in the HUD. |
| **Emphasis** | `Alegreya-Bold.ttf` | Bold inline labels (names inside the tooltip/detail, "Phổ Công:"), and wherever `Emphasis` is used today. |

The current fonts (Playfair Display, Literata) are no longer used. **Do not delete them**; Claude removes them after verification.

## Do
1. Add `UiFontRole.Display` and a `display` field to `UiFontRegistry`. The builder that creates `Resources/UiFonts.asset` assigns the four fonts above. If the asset exists, it updates the fields.
2. Go through every `UiFontRole.Title` use (about 21) and pick Display or Title according to the table above. Leave body and emphasis uses on the new fonts through the registry.
3. **Metrics:**
   - Charm is a slanted script: give Display texts a little extra width or height where needed, and check the line spacing of multi-line Display text;
   - Alegreya is about 15% narrower than Literata. Check that `SingleLineFit`/`FitLabel` minimum sizes still read well; Alegreya at 18 px reads about like Literata at 16;
   - set sensible `lineSpacing` for the body (about 1.05–1.15).
4. Ornate buttons: Alegreya SC caps are taller. Keep the label inside the 9-slice inset (`UiBuild.FitLabel` with `OrnateBorderClearance`).
5. Update any test that asserts a font name.

## Tests
- `TextOverflowTests` must still pass.
- Add an EditMode test: the registry has all 4 fonts assigned, and each font contains the glyphs of a Vietnamese sample string (`Font.HasCharacter`) for "Nhất Nang Giang Hồ ạảấầẩẫậắằẳẵặẹẻẽếềểễệỉịọỏốồổỗộớờởỡợụủứừửữựỳỵỷỹđơư ≤ •".

## Rules
- Edit only:
  - `Assets/_Game/Scripts/**`, except `Scripts/Engine/**`;
  - `Assets/_Game/Editor/**`;
  - `Assets/_Game/Tests/**`.
- No git.

## Result
- Which texts got Display vs. Title (a list by popup or scene).
- The metric adjustments.
- What Claude should check in screenshots.
