# Codex task: show Thể Lực regen (countdown and tooltip)

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/Pve/PveProgress.cs`: `StaminaConfig` and `StaminaService`. The rules are max 60 and 1 point every 6 minutes,
  from `Resources/Data/Pve/stamina.json`.
- `Scripts/UI/Home/HomeController.cs` `RefreshCurrencies`;
- `Scripts/UI/Pve/StageMapPopup.cs`;
- `Scripts/UI/Common/HoverTooltipTarget.cs`, the hover tooltip already used in the shop.

## Problem
- **The user can't see how stamina regenerates.**
- **Home never calls `StaminaService.Refresh`,** so the banner number stays stale until the stage map opens.

## Do
1. **Home banner stamina slot**
   - Every second while Home is visible (unscaled time), call `StaminaService.Refresh` on the profile.
   - Update the number.
   - Save the profile only when the value changed. Use the existing save path; do not save every second.
2. **Hover tooltip on the stamina slot**, using `HoverTooltipTarget` or the same tooltip style:
   - `Thể Lực: 42/60`
   - `Hồi 1 điểm mỗi 6 phút` (computed from config, not hardcoded)
   - `+1 sau 04:12` and `Đầy sau 1:48:00`, or `Đã đầy` when full.
3. **Stage map popup**
   - Show current stamina and the `+1 sau mm:ss` countdown next to the cost line, refreshed every second.
   - When not enough: `Không đủ Thể Lực, đủ sau mm:ss`.
4. **Pure helper for testability:**
   - `StaminaService.SecondsToNext(state, now)`;
   - `StaminaService.SecondsToFull(state, now)`.

## Tests (EditMode)
- **`SecondsToNext` / `SecondsToFull`:** full, partial progress, and empty.
- **Regression:** `TrySpend` keeps partial regen progress.
  - Example: stamina 50/60, last regen 4 minutes ago. Spend 6. The next point arrives 2 minutes later, not 6.
  - Claude already fixed `TrySpend` this way. Add the test only.
- **Formatting:** `mm:ss` and `h:mm:ss`.

## Allowed files
- `Scripts/Pve/PveProgress.cs`: add the helpers only;
- `Scripts/UI/Home/HomeController.cs`;
- `Scripts/UI/Pve/StageMapPopup.cs`;
- `Editor/Building/LoginHomeSceneBuilder.cs`: the Home part only, if a tooltip target must be added by the builder.
  **Never touch `BuildLoginScene` or `LoginScene`.**
- `Editor/Building/PvePrefabBuilder.cs`, if a label is needed;
- tests under `Tests/EditMode/Pve/`.

## Rules
- **No git.**
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; they must be clean for the changed lines.

## Report (`-o` output)
- The files changed.
- The lint result.
