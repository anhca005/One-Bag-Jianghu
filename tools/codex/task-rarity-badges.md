# Codex task: rarity badges on shop cards and item views

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/UI/Popups/ShopCard.cs` (it shows rarity only as a border color now);
- `Scripts/UI/Bag/` item views (`ItemView`) and storage item views;
- `BagViewStyle.Rarity`;
- `Editor/ItemIconImporter.cs`, which handles UI sprite import;
- the builders that create the shop card and item view prefabs: `EconomyPopupPrefabBuilder`, `BagPrefabBuilder`.

## Art (already in place)
- `Textures2D/UI/rarity_common.png`, `rarity_uncommon`, `rarity_rare`, `rarity_epic`, `rarity_legendary`,
  `rarity_supreme`.
- All are 512×512, transparent, round medallions.
- **Import:** make sure the importer handles them as UI sprites with alpha and no 9-slice. If `ItemIconImporter` needs
  a rule, add one.
- **Loading:** load them through the same art lookup the UI already uses. If a badge is missing, show no badge and
  raise no error.

## Show the badge
1. **Shop card (Thương Hội):**
   - A badge in the top-left corner of the item art, about 34×34 px. Keep the border color too.
   - The card's hover tooltip already shows details. Make sure it names the rarity: `Phàm Phẩm`, `Lương Phẩm`,
     `Tinh Phẩm`, `Tuyệt Phẩm`, `Thần Binh` or `Chí Tôn`. Use the existing display names if there is a helper.
2. **Item views in Bao Phục and Tàng Bảo Các:**
   - A small badge, about 20×20, in the top-left corner of each placed item.
   - It must not block drag or raycasts: set `raycastTarget = false`.
   - It must not cover the Trọng label if there is one. Check where it is and pick the free corner.
3. **Replay item views:** do not add badges there; the screen is too busy.
4. **Builders:** add the badge `Image` through the builders so prefab rebuilds keep it. Never touch `LoginScene` or
   `BuildLoginScene`.

## Tests (EditMode)
- The rarity → sprite name mapping covers all 6 rarities, plus an unknown rarity → none.
- The badge image has `raycastTarget` false: test it on the built prefab if your existing tests already load prefabs.

## Rules
- **No git.**
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; they must be clean for the changed lines.

## Report (`-o` output)
- The files changed.
- Where the badge sits on each view.
- The lint result.
