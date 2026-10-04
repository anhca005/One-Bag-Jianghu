# Codex task FT2: fix SingleLineFit regressions after the font switch

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. This follows task FT (`tools/codex/task-fonts-and-fit.md`). Same rules: builders and scripts only (not `Scripts/Engine/**`), tests allowed, **no git**.

Claude fixed one bug already: `UiFontRegistry` was declared inside `UiFonts.cs`. A ScriptableObject class must live in a file of the same name, or the asset loses its script (`m_Script: {fileID: 0}`) and `Resources.Load` returns null. It now lives in `Scripts/UI/Common/UiFontRegistry.cs`. Do not move it back.

After Build All Overwrite, the snapshots (`Logs/Snapshots/HomeScene.png`, `GameplayScene.png`, `ShopPopup.png`; open them) show these problems:

1. **Labels shrunk far too small.**
   - Even short words on ornate buttons and plaques render at about the 12 px minimum: "Đóng", "Làm mới", "Thương Hội" tab, "Xác nhận đổi", every Home plaque, "Cài đặt", and the Home bottom buttons.
   - Likely cause: the fit runs while the rect width is still 0 or not laid out, at build time or on enable before layout, and the result is serialized.
   - Fix:
     - Never fit against a width ≤ 0; skip or defer until the rect has size (`OnRectTransformDimensionsChange`, or `Canvas.willRenderCanvases` once).
     - Store the authored size separately and **always fit from the authored size**, never from the current (already shrunk) size.
     - At build time, force a layout (`LayoutRebuilder.ForceRebuildLayoutImmediate` / `Canvas.ForceUpdateCanvases`) before fitting, or skip fitting in the builder and rely on runtime.
   - Expected: short labels keep their authored size; only labels that really overflow shrink.
2. **Battle control buttons** ("Tạm dừng", "x1", "x2", "x4", "Chơi lại", bottom-left in `GameplayScene`): the labels are still full size and overlap the cloud ornaments. Make sure these buttons get the `OrnateButton` preset and `SingleLineFit`. Find where they are built.
3. **The battle stat bars lost their text.** The fighter panel bars (Sinh Lực / Cương Khí / Nội Lực, e.g. "Sinh Lực 0/0") are empty. FT moved them to `Literata-SemiBold` (Emphasis). Check:
   - the role/font assignment;
   - colour and alpha;
   - whether SingleLineFit set the size to 0, or the rect collapsed;
   - vertical overflow with the new line spacing.
4. **Home top bar:** "Thể Lực" lost its second line "120/120". The two-line text is clipped, probably by line spacing 1.1 with the taller Literata metrics. Give it room (`verticalOverflow = Overflow`, or a taller rect), or put it on one line ("Thể Lực 120/120").
5. Check the same patterns everywhere else: any multi-line label clipped by the new font metrics, and any fitted label serialized at the minimum size. Grep the generated prefabs/scenes for `m_FontSize: 12` on `SingleLineFit` objects as a quick detector, and report what you find.

## Tests
- `SingleLineFit`:
  - a zero-width rect does not change the size;
  - refitting after a width change grows back to the authored size;
  - fitting is idempotent (running it twice gives the same size).
- Keep everything green.

## Result
The root cause of each item, the fix, and the files changed. Claude rebuilds and checks the snapshots.
