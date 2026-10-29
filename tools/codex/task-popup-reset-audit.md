# Task: popup buttons must be reset when a popup is reopened

Read `CLAUDE.md` and `docs/tech/code-style.md` first. Code must follow code-style.md.
Do NOT run any git command that changes the repo. Do NOT run Unity, scene builders or prefab builders.

## Background

Popups in `One-Bag-Jianghu/Assets/_Game/Scripts/UI/` are pooled by `PopupManager` and reused (`BasePopup`:
Setup -> OnSetup, OnShown, OnHidden). A bug just fixed: `BagPopup` disabled its close button and set
`isSaving = true` while saving, closed on success, and never reset them, so the reopened bag could not be
closed. Fix is `ResetCloseButton()` called from `OnSetup` (see `Scripts/UI/Popups/BagPopup.cs`).

## What to do

1. Audit every popup / panel script under `Scripts/UI/` (Popups, Arena, Economy, Online, Pve, Quest, Common, ...)
   for state that is changed during use and not restored on the next show:
   - `Button.interactable = false`, `UiButton.SetLabel(...)` "busy" labels ("Đang ...")
   - busy flags (`isSaving`, `isBusy`, `isSearching`, pending request ids, ...)
   - coroutines / timers stopped on hide but flags left set
   - network callbacks that arrive after the popup was closed and change it
2. For each real problem, fix it the same way as BagPopup: one small reset method called from
   `OnSetup` (or `OnShown` if that fits the popup better). Keep the fix minimal, match the surrounding code.
3. Do not change behaviour that is intentional (e.g. a button disabled because the player lacks currency
   and recomputed on bind).

## Allowed files

Only `.cs` files under `One-Bag-Jianghu/Assets/_Game/Scripts/UI/`. No prefabs, no scenes, no editor code.

## Self check

- `py tools/lint/check_style.py <each file you changed>` must report 0 problems.
- Make sure the code compiles by reading it carefully (no Unity run).

## Report

Write `tools/codex/out/popup-reset-audit.md` (Vietnamese): table of every popup checked, state found,
whether it was a bug, and what was changed (file:line). List suspicious cases you did not change.
