# Codex task ST4: the remaining visual bugs after ST3

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Same rules as ST–ST3:
- edit only `Scripts/**` (not `Scripts/Engine/**`), `Editor/**` and `Tests/**`;
- builders only;
- **no git; do not run Unity.**

Open the screenshots first: `Logs/Snapshots/StoragePopup_filled.png` and `Logs/Snapshots/Tooltip_filled.png`. They were taken after ST3.

## Test status (Claude ran it)
- EditMode 250/250. PlayMode 21/22. Only `RealItemTextFitsStorageBagAndShop` fails.
- **Claude changed `CheckTexts`:** for best-fit text, `preferredHeight` is measured at `fontSize`, so it overstated the height. Best-fit text is now checked by `cachedTextGenerator.characterCountVisible` vs `characterCount` (truncation at the minimum size). Keep this.
- **Remaining failures:**
  - `ShopPopup/.../Shop Card Prefab/Buy/Label`: height 26.0 > 24.5;
  - for every item: `tooltip <id> · viewport does not honor paper padding` and `text is outside its padded paper rect` (your own ST3 check).

## Visible bugs
1. **Tooltip is not content-sized.**
   - The paper stays about 320 px tall. The text is cut mid-line after "Phổ Công: Sát thương 100% Lực Đạo (xuyên", with no scrollbar.
   - ST3 promised height = content up to 70% of the screen.
   - Debug why. Likely causes:
     - the content height is read before the layout rebuild: call `LayoutRebuilder.ForceRebuildLayoutImmediate` on the content, then use `descriptionText.preferredHeight` for the wrap width;
     - or the panel height is set and then overwritten by an anchor/stretch or by the prefab size.
   - The padding failures above probably come from the same code.
   - Make the test assert that the tooltip paper height ≥ min(text preferred height + vertical insets, 70% of the screen).
2. **Storage cell badges** are now large grey rectangles (~60×45) covering the artwork. On 1-column tall items they stretch vertically.
   - The badge must be a **small fixed-size pill**, about 34×22, at the bottom-left inside the frame, independent of the item's size. Do not stretch it with anchors.
   - Copy the look of the Bao Phục tray badge: a dark rounded/translucent background with light text. That one looks right.
3. **Currency icons still overlap the first letter:**
   - storage header: "Bạc 1,000" shows as "ạc 1,000", the icon covers "B";
   - HUD: "Bạc 135,555" shows the same.

   Use a real layout per entry:
   - a child `Icon` Image with a fixed size and a `LayoutElement`;
   - a child `Text` with a `ContentSizeFitter`;
   - a `HorizontalLayoutGroup` with spacing 6 on the entry.

   No manual offsets. Apply this to Thể Lực, Bạc, Nguyên Bảo and Danh Vọng in the HUD, and to the storage header.
4. **Storage detail body text** still looks horizontally condensed (characters squeezed) next to the tooltip text, which uses the same font and looks normal.
   - Check every parent of the Detail text for a non-1 `localScale` and for any `SingleLineFit`, best-fit or `Outline`/`Shadow` that distorts it.
   - Check the font role. Fix the cause.
5. **Shop Buy label:** fit it (the box is 24.5 px high; the label wants 26).

## Tests
`TextOverflowTests` passes, including the stronger tooltip-height assertion.

## Result
For each item 1–5: the cause found and the fix, with file names.
