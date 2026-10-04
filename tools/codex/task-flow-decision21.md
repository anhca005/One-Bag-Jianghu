# Task F: apply decision #21 — battle without preparation, bag warning on Home, no sect for new players ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read `docs/decisions.md` #20 and #21, `docs/tech/ui-flow.md`, `Assets/_Game/Editor/Building/README.md`.

## Hard rules
- Modify only under `One-Bag-Jianghu/Assets/_Game/`: `Scripts/UI/Gameplay`, `Scripts/UI/Home`, `Scripts/UI/Login`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Scripts/UI/Popups/BagPopup.cs`, `Scripts/Core/GameSession.cs`, the builders in `Editor/Building/` (Gameplay, LoginHome, Popup, Bag), `Tests/`. Do not touch `Scripts/Engine`, `Scripts/Battle/Replay` logic, VFX Lab, demo scenes (BagDemo keeps its sect selector as a dev tool).
- Everything stays pre-built (decision #20): change the **builders**, not runtime UI creation. Claude will re-run the builders with Overwrite (the user has not hand-edited these scenes yet). Tell Claude the exact methods to run.
- English, C# 9, existing style. No git. You cannot run Unity.
- **Known pitfalls:** (1) `bool ok = x != null && x.TryGet(k, out Sprite s);` then using `s` → CS0165; declare `Sprite s = null;` first. (2) Never block the main thread on async work.

## Changes
1. **GameplayScene has no preparation phase.** On scene ready: take the saved loadout from `GameSession`, run the battle (offline `LocalBattleService`, keep the online path untouched), then replay, then `ResultPopup`. Remove the Preparation root/prefab from the scene builder (delete `Prefabs/UI/Gameplay/Preparation.prefab` and its builder code if unused). If the loadout is invalid anyway (defensive), show a `MessagePopup` and return to Home.
2. **Home battle area:** PvE / PvP buttons with a `WarningText` object **right above them**. Validation from the saved loadout: empty bag → "Bao Phục đang trống"; no weapon → "Bao Phục chưa có binh khí"; otherwise hidden. When invalid the buttons are non-interactable. Re-validate after `BagPopup` closes (saved). Keep the current Home layout otherwise — a new "around the background" layout is being designed by Gemini and will come in a later task; keep object names stable (`Canvas/BattleArea/Btn_Pve`, `Btn_Pvp`, `WarningText`).
3. **No sect for new players:** Login only asks the name. A new profile has no sect (`sect` null, Tâm Pháp level ignored). `BagLoadoutPanel` gets a serialized option `showSectSelector` (false in the player's BagPopup, true in BagDemo — do not edit the BagDemo scene; the default for the prefab should keep BagDemo working, so set it false only on the BagPopup instance via the popup builder). Existing saved profiles that have a sect: keep loading them (do not crash), but the player UI does not show the selector. Add a TODO/placeholder field for hero level (`GameSession.HeroLevel`, default 1) — the level system itself is out of scope.
4. **Tests:** update `Tests/PlayMode/DemoSmokeTests.cs` full-flow test (no preparation step: Home → Btn_Pve → Gameplay replays automatically → result → Home), and add a PlayMode or EditMode test for the Home warning (empty bag → warning visible + buttons disabled; bag with a weapon → hidden + enabled). All existing tests stay green.

Finish with: files created/changed, the exact builder methods (with Overwrite) Claude must run in order, anything unsure.
