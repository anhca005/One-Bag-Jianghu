# Codex task: item tooltip on hover during battle

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/UI/Common/ItemTooltip.cs` and `HoverTooltipTarget.cs`: the tooltip already used in Bao Phục and Thương Hội;
- how `BagLoadoutPanel.Describe`, `ShopPopup` and `ItemView.SetTooltip` build the text (`ItemDescriber`,
  `ItemTooltip.WithPower`);
- `Scripts/Battle/Replay/View/ReplayItemView.cs`, `ReplayScreen.cs`, `ReplayUi.cs`;
- `Scripts/UI/Gameplay/GameplayController.cs`, which starts the replay;
- the replay part of `Editor/Building/ReplayPrefabBuilder.cs` and `GameplaySceneBuilder.cs`.

## User request
- **Today:** in battle, hovering an item on either side shows nothing.
- **Want:** hovering an item, the player's or the enemy's, shows the same tooltip as in Bao Phục: name, rarity, Trọng
  (tier), stats and description, and Lực Chiến if the bag tooltip shows it.

## Do
1. **Data:**
   - Each `ReplayItemView` must know its item id, rarity and tier.
   - Find where the replay views are created from the loadouts, and pass these values in. Use what the battle start
     data already carries; do not invent new server fields. If a value is missing for the enemy, describe what is
     known.
2. **Tooltip:**
   - Reuse `ItemTooltip` and `HoverTooltipTarget`. Do not write a second tooltip.
   - One tooltip instance for the battle screen. It must render above the item grids, the fighter panels and the VFX
     layer (`EnsureAbove` or an overlay canvas, as the bag does).
   - Show it after the same delay as in the bag. Hide it on pointer exit, when the replay ends, and when the result
     popup opens.
   - Keep it inside the screen. `ItemTooltip` already clamps; check that it does for both sides.
   - Hovering must not block the battle or other clicks. Speed buttons keep working while the tooltip is shown.
3. **Raycast:** the item views or their images must receive pointer events. Check `raycastTarget` on the item image and
   that no overlay (VFX, cooldown, status) blocks the hover. Overlays should not be raycast targets.
4. **Builder:**
   - If a scene or prefab object is needed, add it through the existing builders so a rebuild keeps it.
   - If possible, create it at runtime instead, so the user needs no rebuild.
   - Never touch `LoginScene`, `BuildLoginScene` or the GameController prefab builder.

## Tests (EditMode or PlayMode, keep it small)
- A `ReplayItemView` bound with an item id, rarity and tier produces non-empty tooltip text that contains the item
  name.

## Rules
- **No git.**
- Do not touch `StoragePopup.cs`. Claude is editing it.
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; changed lines must be clean.

## Report (`-o` output)
- The files changed.
- Where the tooltip instance lives and how it stays on top.
- Whether the user must rebuild any prefab or scene, and through which menu item.
- The lint result.
