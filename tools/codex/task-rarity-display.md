# Codex task RD: show the owned rarity (colour + frame) everywhere an item is displayed

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Read `docs/decisions.md` #36, #35 and #27, and `docs/design/glossary.md` (rarity names).

The user reports that items do not show their rarity colour and frame in several places. Claude's audit found:

| Where | Problem |
|---|---|
| Bao Phục: `Bag/View/ItemView.cs` (`Bind`, `SetAdjacent`) for placed items and for the tray (`ItemTrayView`) | Uses `ItemInfo.Rarity`, the catalog's **base** rarity (almost always common), instead of the owned copy's rarity (`OwnedItem.Rarity`, looked up by uid in the profile). |
| Replay: `Battle/Replay/View/ReplayScreen.cs` (~lines 100 and 184), `ReplayItemView`, `FighterPanelView` | Uses the catalog rarity. `BattleItemRef` has no rarity. The real rarity of both sides is in the loadouts that started the battle: the player's from the profile, the enemy's from the PvE stage / sample loadout. |
| Tàng Bảo Các: `UI/Popups/StoragePopup.cs` (`RarityColor`) | Uses the owned rarity correctly, but with its **own palette**, which differs from `BagViewStyle.Rarity`. |
| Thương Hội: `ShopCard` | OK (`BagViewStyle.Rarity`). |

## Rules
- Edit `Assets/_Game/Scripts/**` **except** `Scripts/Engine/**`, plus `Assets/_Game/Editor/**` and `Assets/_Game/Tests/**`.
- If a prefab needs a new frame image or component, change the **builder** in `Editor/Building/**`, not the prefab.
- **No git.**

## Build
1. **One source of truth:**
   - `BagViewStyle.Rarity(rarity)` (frame/border colour) and `BagViewStyle.Fill(rarity)` (background tint) are the palette. Remove `StoragePopup.RarityColor` and any other private palette; route everything through `BagViewStyle`.
   - Add `BagViewStyle.RarityName(rarity)` (Phàm, Lương, Tinh, Tuyệt, Thần Binh, Chí Tôn) if there is no shared helper yet, and use it instead of the local copies.
   - Make the frames clearly visible: a ≥ 2 px border in the rarity colour on every item tile. Keep the ink-wash style: for common, use the existing grey.
2. **Bao Phục:**
   - `ItemView` takes the owned rarity: an explicit `rarity` parameter, or a lookup by uid. Wire it through `BagLoadoutPanel` / `BagGridView` / `ItemTrayView`, where the owned uid is known.
   - The adjacency highlight must restore the **owned** rarity colour.
   - Tiles without a uid (demo scenes, test grids) fall back to the catalog rarity.
3. **Replay:**
   - Carry the rarity per item uid into the replay. Simplest: the code that builds the replay from the loadouts already knows both loadouts' items in order (uid = side × 100 + index), so add `Rarity` to `BattleItemRef`, filled when the log is created locally, or map it in `ReplayScreen` from the loadouts.
   - Server logs that lack it fall back to the catalog rarity.
   - Apply it to the replay item tiles and to anything in `FighterPanelView` that shows items.
4. **Audit the rest.** Grep every place that shows an item icon or name (`ItemIconLibrary` users: `GameplayController`, `ResultPopup`, `StageMapPopup`, `DialoguePopup`, `UiKit`, `ShopPopup`, the item tooltip and the storage detail panel). Wherever a specific owned or dropped copy is shown, give it the rarity frame, and show the rarity name in the detail/tooltip text, coloured. List every site you checked in your result, with OK / fixed / not applicable.

## Tests
- EditMode:
  - an owned Thần Binh item placed in the bag gets the legendary colour on its `ItemView`, and keeps it after adjacency on/off;
  - a replay built from loadouts with mixed rarities maps each uid to its rarity;
  - `BagViewStyle` is the only palette: no `new Color(` palette arrays for rarity remain outside it. A simple grep-style test is fine, or just state it in the result.
- Keep the existing tests green. Claude runs Unity afterwards.

## Result
- The site audit table.
- The changed files.
- Anything you could not verify visually.
