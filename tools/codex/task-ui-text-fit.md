# Codex task TF: text must fit inside its art (buttons, plaques, dropdowns, panels)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Prefabs and scenes are **generated** by the builders in `Assets/_Game/Editor/Building/**` (Build All Overwrite). Fix the builders and shared helpers, never the prefabs. Read `Editor/Building/README.md` and `UiBuild.cs` first.

## Problem (user report + Claude's snapshot review, `Logs/Snapshots/*.png`)
Many labels are stretched to the full rect of their image ("pivot/anchors full"), so text runs over the ornaments or edges:
1. **Ornate buttons** (`UiBuild.Button`, sprite with cloud scrolls at both ends): the label spans the whole width and overlaps the scrolls.
   - Seen in: "Đổi nguyên liệu", "Tiệm Danh Vọng" (Thương Hội tabs), "Hành Tẩu Giang Hồ" (Home), "Cài đặt", "Nâng Trọng"/"Hợp Thành" (tight), "Thoát", "Tạm dừng", "Chơi lại" (battle).
   - Fix: the label gets an inner rect that leaves the ornament zone free (about 18–22% of the width on each side, measured from the sprites in `Textures2D/UI`), with `resizeTextForBestFit` (min ~14, max = current size). The text stays centred, horizontally and vertically.
2. **Home hotspot plaques** (`Hotspot.prefab`, built in `LoginHomeSceneBuilder` / `CommonUiPrefabBuilder`): labels sit against the top edge of the plaque ("Thương Hội", "Tàng Bảo Các", "Vạn Bảo Phô", "Bao Phục", "Luận Kiếm Đài", "Hành Tẩu Giang Hồ"). Centre them inside the plaque's paper area, padded, with best fit.
3. **Dropdowns / input boxes** with the ink-brush frame and red seal: the caption text touches the left edge and the seal ("Tất cả" in Tàng Bảo Các).
   - Add left/right padding past the seal and the arrow.
   - In Thương Hội "Đổi nguyên liệu", the dropdowns show the raw English placeholders "input" and "target". Use Vietnamese captions: "Nguyên liệu bỏ ra" and "Nguyên liệu nhận".
   - Also check the Bao Phục sect dropdown (white background; it should use the shared `Dropdown.prefab` style).
4. **Battle avatar block** (`ReplayPrefabBuilder`, `FighterPanel`): the title "Hiệp Khách" overlaps the top ornament of the portrait frame. Move it above the frame or onto its own strip, inside the panel.
5. **Home top bar:**
   - An empty grey box sits next to the name (the EXP bar? It needs a fill and a "EXP x/y" label, or should be hidden when empty).
   - Two bare numbers "0" "0" have no icon or label. Use the currency icons/names (`UiKit.CurrencyArt` and glossary names) like "Bạc 8,800".
   - The "Môn Phái · Cấp 10" hotspot is faint and unreadable: make the locked state readable (dimmed art, legible text).

## Make it systematic
- Add a helper in `UiBuild` (e.g. `FitLabel(Text text, RectOffset padding)` or padding presets per sprite kind: `OrnateButton`, `Plaque`, `InkBox`) and use it everywhere those sprites are used. Do not patch each call site with magic numbers.
- Sweep **all** builders for `Text` children stretched over sprite images, and apply the right preset. List each place in the result.
- Keep fonts and sizes as they are, except best-fit shrinking.

## Rules
- Edit only `Assets/_Game/Editor/**`, plus `Assets/_Game/Scripts/**` (not `Scripts/Engine/**`) where runtime code sets text or rects.
- Keep the existing tests green; add an EditMode test if useful (e.g. every button label's rect is inset from its parent).
- **No git.**

## Result
List the presets with their padding values, every call site changed, and anything you could not judge without seeing it. Claude will rebuild, take snapshots and check visually.
