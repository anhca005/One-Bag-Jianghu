# Codex task TG: PlayMode tests must never leave the player's profile changed

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`. Follow `docs/tech/code-style.md` (one statement per line, ≤120 chars, Allman braces, no inline if/for bodies).

## Problem
The user plays the game in the Editor. After a full PlayMode run, their `profile.json` (in `Application.persistentDataPath`) changed: `heroLevel` 2 → 10 and `sectPromptShown` false → true.

Only `DemoSmokeTests`, `PveResultFlowTests` and `StorageSmokeTests` back up and restore the profile, each with copy-pasted SetUp/TearDown code. Other PlayMode tests save through `GameSession` without restoring, for example:
- `LevelSectFlowTests`;
- `PveThemeSmokeTests`;
- `BagTooltipSmokeTests`;
- `DragOutsideGridTests`;
- `TextOverflowTests`;
- `DevWipeSmokeTests`.

## Do
1. Add `Tests/PlayMode/PlayerDataGuard.cs`: an NUnit `[SetUpFixture]` that covers the whole PlayMode test assembly. If the assembly uses namespaces, use one that encloses all PlayMode test classes, or no namespace.
   - **`[OneTimeSetUp]`** snapshots:
     - every file the game writes in `persistentDataPath`: `profile.json`, `profile.json.bak`, `profile.json.tmp`, `loadout.json`. Grep `Scripts/` for other file names written there and include them;
     - every `PlayerPrefs` key the game uses. Grep `Scripts/` for `PlayerPrefs.Set*` key constants; `"OneBagJianghu.*"` keys at least.
     - Keep the snapshot in memory. Also write a copy to `persistentDataPath/test-guard/` so a killed run can be recovered by hand. Do not delete that copy on the next start if it already exists; overwrite it.
   - **`[OneTimeTearDown]`** restores everything exactly:
     - files that existed get their bytes back;
     - files that did not exist are deleted;
     - prefs the same way;
     - then `PlayerPrefs.Save()`.
2. Remove the duplicated per-class backup/restore code from the three test classes, unless a test needs a clean profile per test. In that case keep only the "start fresh" part, and leave restoring to the guard.
3. Do not change game code under `Scripts/`.

## Check
- `py tools/lint/check_style.py <changed files>` → 0 problems.
- C# compiles. No git. Do not run Unity; Claude runs the tests and compares the profile md5 before and after.

## Result
List the files changed, the files and keys the guard covers, and where each was found.
