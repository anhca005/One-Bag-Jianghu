# Codex task: battle feel — per-item hit feedback and visible item status

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. Code under `Assets/_Game/`. You cannot run Unity; Claude
builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory), `docs/design/combat.md` (statuses and what targets an item);
- `Scripts/Battle/Replay/View/ReplayScreen.cs` (`PresentEvent`, `PresentVfx`, `ApplyItemStatus`), `ReplayItemView.cs`,
  `ReplayVfxLayer.cs`, `ReplayStyle.cs`, `FighterPanelView.cs`;
- the event model in `Scripts/Battle/Replay/` (`BattleEvent`, kinds such as `SEAL`, `SLOW`, `HASTE`, `ULTIMATE`);
- status icons in `Textures2D/Status/` (`status_seal.png`, `status_slow.png`, `status_haste.png`, ...).

## User report
1. **Bug:** when one weapon attacks, every item in the bag flashes white once. Only the item that fired should react.
2. **Want:** the state of each item must be readable on the item itself. Example: an item hit by Điểm Huyệt (SEAL)
   must look sealed for the whole duration, not just show a small label.

## Do
1. **Find and fix the all-items flash.** Look at `Flash`, the flash overlay, shared materials/sprites (`fillSprite` is
   static), and any VFX that targets the whole bag. Report the root cause.
2. **Firing feedback (only the item that fired):**
   - a short scale punch (about 1.0 → 1.08 → 1.0, ~0.15 s) plus a brief light flash on that item only;
   - ULTIMATE: a stronger gold flash and a slightly longer punch;
   - unscaled by game speed? No: follow the replay clock so x2/x4 stay in sync.
3. **Item status visuals, for the whole duration, on the affected item:**
   - **SEAL (Điểm Huyệt):** item desaturated/greyed, a dark ink overlay, the seal status icon centered, and its
     cooldown fill frozen;
   - **SLOW (Trì Trệ):** a cool blue-grey tint and the slow icon in a corner;
   - **HASTE (Khinh Công):** a warm gold edge glow or tint and the haste icon in a corner;
   - when several apply, show all icons (small row) and the strongest tint (SEAL wins);
   - a remaining-time indication (a thin bar or ring that drains) is welcome if cheap;
   - clear exactly when the status ends, and on replay restart ("Chơi lại") and seek.
4. **Hit feedback on the target side:** when a damage event lands, a small shake of the target fighter panel (or the
   portrait) only. Keep it subtle; no full-screen shake.
5. Everything is built at runtime or through existing builders; no manual scene edits. If a prefab must change, change
   its builder and tell Claude which menu item rebuilds it. Prefer runtime so no rebuild is needed.

## Tests (EditMode, small)
- Firing one item flashes only that item's view (others unchanged).
- A SEAL status sets the sealed visual on that view and clears it after its end time.

## Rules
- **No git.** Never touch `LoginScene`, `BuildLoginScene` or the GameController prefab builder.
- Other Codex tasks run at the same time. **Only touch files under `Scripts/Battle/Replay/` and their tests**, plus
  `ReplayPrefabBuilder.cs` if really needed. Do not touch `GameplayController.cs`, `ItemTooltip.cs`, shop, home, PvE data.
- Lint every changed `.cs` file with `py tools/lint/check_style.py`; changed lines must be clean.

## Report (`-o` output)
- Root cause of the all-items flash.
- What each status looks like, the files changed, rebuild needs, lint result.
