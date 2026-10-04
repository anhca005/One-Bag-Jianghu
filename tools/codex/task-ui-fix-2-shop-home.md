# Codex task UF2: Thương Hội tabs and header, reputation shop, exchange, Home currency refresh

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. **Follow `docs/tech/code-style.md`** (one statement per line, ≤120 chars, Allman braces, no inline if/for bodies, named helpers). This also applies to builder code you touch.

Parallel tasks:
- UF1 owns `UiBuild.cs`, `CommonUiPrefabBuilder.cs`, `GameControllerPrefabBuilder.cs`. It fixes the common `Dropdown.prefab` template and adds `SlicedAutoFit`.
- UF3 owns Bao Phục, replay and PvE.

**Do not edit UF1's or UF3's files.** You may *call* existing `UiBuild` helpers.

## Your files (only these)
- `Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs`
- `Assets/_Game/Editor/Building/LoginHomeSceneBuilder.cs` (Home HUD only)
- `Assets/_Game/Scripts/UI/Popups/ShopPopup.cs`, `ShopCard.cs`, `StoragePopup.cs` (only if needed for the currency row)
- `Assets/_Game/Scripts/UI/Home/HomeController.cs`
- `Assets/_Game/Scripts/UI/Common/CurrencyArt.cs` (if that is where the icon + text pairing lives)
- tests under `Assets/_Game/Tests/EditMode/` and `Assets/_Game/Tests/PlayMode/` (new files, or files that test these screens)

## Bugs (user screenshots)

### 1. Shop header: Bạc / Danh Vọng icons are detached from their numbers
In the Thương Hội header, the silver icon sits at the far left edge and the reputation icon sits alone in the middle. The text "Bạc: 1,200 Danh Vọng: 0 Miễn phí sau 05:59:59" is one string in the centre.

Make proper pairs, each with the icon directly left of its own value, as on Home and Tàng Bảo Các:
- `[icon_bac] 1,200`
- `[icon_danh_vong] 0`
- then "Miễn phí sau 05:59:59".

Use a HorizontalLayoutGroup, or the same currency-entry helper the Home HUD uses (`CurrencyArt` or similar). Do not hand-place icons with fixed offsets.

### 2. Tab highlight does not follow the active tab
Clicking "Đổi nguyên liệu" or "Tiệm Danh Vọng" switches the content, but "Thương Hội" keeps the red (primary) style. In `ShowTab`, set the active tab to the primary style and the others to secondary. Swap sprite/colours through the existing `UiButton` style API if there is one.

### 3. Exchange tab (Đổi nguyên liệu)
- The two dropdowns show raw ids such as `han_thiet`. Show the Vietnamese material name (from the material catalog), with an icon if the dropdown supports one.
- The two dropdowns must be the **common `Prefabs/UI/Common/Dropdown.prefab`**, linked as in `BagPrefabBuilder.Dropdown` (`UiBuild.InstantiateLinked<Dropdown>(CommonUiPrefabBuilder.Folder + "/Dropdown.prefab", parent)`). Do not build a private dropdown in this builder. UF1 fixes the template, so the open list stops being empty.
  - Do the same for **every** dropdown built in `EconomyPopupPrefabBuilder` (sect select, Tàng Bảo Các filter and sort, upgrade item and rarity…).
- The status line "Lựa chọn không hợp lệ" is clipped at the left ("'a chọn không hợp lệ"). Give it a full-width rect with centred alignment.
- Add a short label above each dropdown ("Đưa vào", "Nhận về") so the user understands the exchange.

### 4. Tiệm Danh Vọng cards
On each card the item name appears twice: a faded grey copy in the icon area, then the real name. The faded copy is the icon placeholder text.
- Look up the icon first:
  - **materials:** `Textures2D/Materials/<id>.png`; `chi_ton_ngoc_tuy` already exists there, so use the same library or lookup the storage view uses for materials;
  - **items:** the item icon library.
  - Show the placeholder text only when no sprite exists.
- The two chests (`catalyst_box_random`, `tier_3_material_box`) have no art yet; ChatGPT is drawing it.
  - Load from `Textures2D/Materials/<id>.png` (or a `Textures2D/Shop/` folder if you prefer; say which in your result), so dropping the files in later just works.
  - Until then, show a neutral placeholder: a small generic chest/box glyph or the plain "◆" alone, **without** the name copy.
- The "Mua" button has red text overlapping it, probably "Không đủ Danh Vọng". Move that reason to its own line above the button, or show it only as a tooltip or toast. It must never overlap the button.

### 5. Home HUD Bạc does not update after spending
Silver is spent in Thương Hội (or Tàng Bảo Các), the popup closes, and the Home HUD still shows the old value until the scene reloads.
- Refresh the HUD currency texts whenever a popup closes.
- **Better:** subscribe to a profile-changed or currency-changed event if `GameSession` has one. If it does not, add a minimal `event Action CurrenciesChanged` raised from the currency mutation points you can find in `PlayerProfile`/economy services, and say where.
- Do not touch engine code.

### 6. `HomeController` readability (from the last formatting pass)
In the currency block (around lines 70–92), `CurrencyArt.Apply(...)` calls are split with the string argument at column 0. The code also writes `"       Bạc "` with leading spaces and then trims them.
- Rewrite this block clearly: one helper `SetCurrency(Text label, string iconKey, string caption, long value)` called four times.
- Keep the behaviour the same, apart from the fix in point 5.

## Tests
- **PlayMode:**
  - open the shop and switch tabs; the active tab button uses the primary style and the others secondary;
  - buy something that costs Bạc, close the popup, and the Home HUD silver text shows the new value without a scene reload.
- **EditMode:**
  - the exchange dropdown options show names, not ids;
  - every `Dropdown` inside the economy prefabs is a prefab instance of `Common/Dropdown.prefab` (`PrefabUtility.GetCorrespondingObjectFromSource`).

## Check
- `py tools/lint/check_style.py <every file you changed>` → 0 problems.
- C# compiles.
- No git. Do not run Unity; Claude rebuilds, tests and screenshots.

## Result
List the files changed, the currency-changed mechanism you used, and the icon path the chest art should use.
