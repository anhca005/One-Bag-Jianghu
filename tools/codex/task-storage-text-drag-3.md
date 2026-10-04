# Codex task ST3: last overflow fixes (after ST2)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Same rules as ST/ST2:
- edit only `Scripts/**` (not `Scripts/Engine/**`), `Editor/**` and `Tests/**`;
- builders only;
- **no git; do not run Unity.**

Look at the new screenshots `Logs/Snapshots/StoragePopup_filled.png` and `Logs/Snapshots/Tooltip_filled.png`.

## Claude ran ST2
- All 4 drag tests pass. The interrupted-drag test now waits for the close animation to finish (Claude's change).
- The Home scene test expects the currency texts under `Header/Currency Row/` (Claude updated the paths).
- **Two false positives in `TextOverflowTests`, both fixed by Claude:**
  - the raw-word check flagged the Vietnamese word "tung". It now flags only snake_case and known engine ids;
  - the width check flagged wrapped text. It now checks width only for `HorizontalWrapMode.Overflow` text with no fitter.

## Still wrong
1. **Storage cell `Badge`** (the "T1"…"T9" label): preferred height 20.8 > 13.7 rect, on every item. It also sits on the frame's corner ornament.
   - Give it a real box (≥ 22 px high) placed inside the inner area, below and left of the corner ornament.
   - Use a small dark pill background like the Bao Phục tray badge, which looks good.
2. **Storage cell `Name`** for items without an icon: preferred height 75.3 > 54.7 ("Thiên Niên Quy…" with an ellipsis). Either:
   - let it fit: a smaller minimum font, 3 lines, or a bigger box inside the frame;
   - or, if an ellipsis is intended, give the component a clear marker (e.g. an `EllipsisText` component) and have `CheckTexts` skip height for texts that carry it.

   Prefer fitting when the box allows it.
3. **Storage header currencies** (top right, "Bạc:1,000  Danh Vọng:0"):
   - a stray silver icon still floats alone at about (1220, 100);
   - the Danh Vọng icon overlaps the "D" of "Danh".

   Use the same icon + number pair layout as the HUD, with no colon ("Bạc 1,000"). Remove the orphan icon.
4. **HUD "Nguyên Bảo 0"** wraps onto two lines and the "0" is cut. Make every HUD currency entry single-line (`SingleLineFit`, or a width from its content via `ContentSizeFitter` inside the `HorizontalLayoutGroup`).
5. **Tooltip:**
   - the text starts at the paper's left edge; the 16 px padding is not applied on the left and right;
   - the red seal ornament at the top-left still overlaps "Bình"/"Công";
   - the tooltip is not content-sized: it stays about 320 px tall and the last lines are cut ("ngẫu nhiên của địch" is cut at the bottom), while ST2 asked for up to 70% of the screen height.

   Fix the padding, which should account for the paper sprite's border. Keep the seal out of the text area, either by moving the text down or the seal out. Grow the height to fit the content up to the cap.
6. **Storage detail:** the body text looks condensed (tight character spacing). Use the normal Literata body font role at 18–20 px, with no horizontal scaling. Check that `SingleLineFit`/best-fit is not applied to the multi-line description.

## Tests
- `TextOverflowTests` must pass with your fixes. Keep Claude's two filter fixes.
- **Add checks:**
  - the tooltip's text rect stays inside its paper rect, inset by the padding;
  - every tooltip is fully on screen.

## Result
For each item 1–6: what you changed, with file names.
