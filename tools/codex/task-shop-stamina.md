# Codex task: fixed shop refresh price, one free refresh for new players, stamina regen timer

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. Code under `Assets/_Game/`. You cannot run Unity; Claude
builds and tests.

**Read first:** `docs/tech/code-style.md` (mandatory); `Scripts/Economy/ShopService.cs`, `ShopCatalog.cs`,
`PlayerProfile.cs` (`ShopState`, profile migration/version); `Resources/Data/shop.json`; `Scripts/UI/Popups/ShopPopup.cs`;
`Scripts/UI/Home/HomeController.cs` (stamina text, `BuildStaminaTooltip`); `Scripts/Pve/` stamina service and
`Resources/Data/Pve/stamina.json`; `Scripts/UI/Pve/StageMapPopup.cs` (stamina display, if any).

## 1. Shop refresh (user decision)
- **Paid refresh costs a fixed 50 Bạc every time.** No daily escalation. Data-driven: in `shop.json` replace
  `paidRefreshCosts` by `"paidRefreshCost": 50` (keep reading the old array as a fallback for old data if simple).
- **One free refresh for a new player.** Replace the 6-hour timed free refresh:
  - `shop.json`: `"freeRefreshHours": 0` means "no timed free refresh"; add `"starterFreeRefreshes": 1`.
  - `ShopState` gets a counter of starter free refreshes used. A new or old profile without it has 1 available.
  - The shop's free-refresh button shows "Làm mới miễn phí" while one is left, then becomes the paid button
    "Làm mới (50 Bạc)". Never show a countdown when timed refresh is off.
  - Do not break the first-shop tutorial (`thai_thanh_tam_kinh` in slot 1 on generation 0, `shop_item_purchased`).
- Bump the profile version only if a migration is needed; keep old saves loading.

## 2. Stamina regen time (Thể Lực)
- On the Home top bar next to the stamina value, show the time until the next point, e.g. `55/60  04:32`. When full,
  show only `60/60`.
- The existing hover tooltip also shows the time until full ("Đầy sau 50:00").
- Updates once per second (Home already refreshes stamina each second). Use the stamina service, not a new formula.
- If the stage map popup shows stamina, show the same timer there.
- If the label does not fit, shrink with the existing fit helpers; do not overlap the bar art.

## Tests (EditMode)
- Paid refresh charges exactly 50 each time (three in a row → 150).
- A fresh profile gets exactly one free refresh; after using it, free refresh fails and paid works.
- Stamina: at 55/60 with a known clock, the "next point" time is correct; at max it reports none.

## Rules
- **No git.** Never touch `LoginScene`, `BuildLoginScene` or the GameController prefab builder.
- Other Codex tasks run at the same time. **Only touch** shop/economy/stamina files, `ShopPopup.cs`, `HomeController.cs`,
  `StageMapPopup.cs`, `shop.json`, `stamina.json` and their tests. Do not touch replay, encyclopedia or PvE chapter data.
- Lint every changed `.cs` file with `py tools/lint/check_style.py`; changed lines must be clean.

## Report (`-o` output)
- Files changed, the new `shop.json` fields, how old saves are handled, rebuild needs, lint result.
