# Task U: GameController as the game-wide manager object + important Debug.Log calls ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Read `docs/decisions.md` #20, #26 (flat scenes, Main Camera) and **#29**, `Scripts/Core/*` (GameBootstrap, GameSession, SceneFlow, NetworkService…), `Scripts/UI/Popups/PopupManager.cs`, `Scripts/UI/Loading/LoadingPanel.cs`, `Editor/Building/{ServicesPrefabBuilder,LoginHomeSceneBuilder,SceneBuild,GameplaySceneBuilder,DemoSceneBuilder,BuildMenu}.cs` (+ README), `Tests/EditMode/{Scenes,UI,Core}/**`, `Tests/PlayMode/*`.

## 1. GameController (decision #29)
The owner created an empty root object `GameController` in `LoginScene` and wants it to be **the manager object of the whole game**, instead of the hidden `Resources/Core/Services.prefab` that `GameBootstrap` instantiates at runtime.
- Replace the Services prefab by a prefab **`Resources/Core/GameController.prefab`** (built by a builder; rename `ServicesPrefabBuilder` → `GameControllerPrefabBuilder` or equivalent) with this structure:
  ```
  GameController            (GameController component: DontDestroyOnLoad, single instance, exposes Session/Flow/Popups/Loading/Network)
    GameSession             (GameSession component)
    SceneFlow               (SceneFlow component)
    NetworkService          (NetworkService component)
    PopupManager            (PopupManager + its overlay Canvas, backdrop, popup root)
    LoadingPanel            (LoadingPanel + its Canvas)
  ```
  Each manager on its own child object with a clear name; the popup/loading canvases keep their sorting orders (popups ≥ 100, loading ≥ 1000). Keep the fix in `PopupManager.RefreshStack` (`nestedCanvas.overrideSorting = true`).
- `LoginSceneBuilder` places a **prefab instance** of `GameController.prefab` at the LoginScene root (flat hierarchy: Main Camera, EventSystem, GameController, LoginController, Canvas). It must replace the owner's hand-made empty `GameController` object (same name, now the prefab instance).
- `GameController.Awake`: if another instance exists → destroy self; else `DontDestroyOnLoad`, initialise managers in order (Session.LoadProfile, Popups.Initialize, Flow.Initialize) and log it.
- **Starting Play from another scene in the Editor** (Home, Gameplay, demos): `GameBootstrap` (RuntimeInitializeOnLoadMethod) instantiates `Resources/Core/GameController` only if no instance exists yet, so every scene still works standalone. Remove the old Services prefab and references (`GameBootstrap.Session` etc. keep working, now backed by the GameController; keep the static accessors to avoid touching every caller).
- Other scenes do **not** contain a GameController object.

## 2. Debug.Log for important events
Use `UnityEngine.Debug.Log` (warnings `Debug.LogWarning`, errors `Debug.LogError`) with a short tag prefix so the owner can filter in the Console: `[Game]`, `[Scene]`, `[Profile]`, `[Shop]`, `[Storage]`, `[Bag]`, `[Battle]`, `[Reward]`, `[Popup]`, `[Cheat]`. Messages in Vietnamese or English, one line, with the key values. At least:
- `[Game]` GameController created / duplicate destroyed; bootstrap from a non-Login scene.
- `[Scene]` scene change requested / loaded (from → to).
- `[Profile]` load (path, version, migrated from v?, item count, material stacks, Bạc), save (path), reset to starter kit, overflow items after migration (warning).
- `[Shop]` slots generated (generation, contents), buy (slot, item/material, price, result), paid/free refresh (cost), exchange, reputation purchase; every failure as warning with the reason.
- `[Storage]` upgrade (item, rarity, Trọng a→b, cost), merge (inputs → output rarity/Trọng, refund), sell (price), move/drag (from → to), repack; failures as warnings.
- `[Bag]` loadout saved (item list with Trọng/rarity), bag ↔ storage transfer refused (warning).
- `[Battle]` start (mode, seed, player/opponent loadout summary), end (winner, duration, events count).
- `[Reward]` granted (Bạc, materials, EXP).
- `[Popup]` show/close (type, stack depth).
- `[Cheat]` every dev cheat action.
Economy code is pure C# (no UnityEngine): log in the UI/Core callers using the returned result objects, not inside `Scripts/Economy`. Keep logs out of per-frame code.

## Hard rules
- Modify only under `One-Bag-Jianghu/Assets/_Game/`: `Scripts/Core/**`, `Scripts/UI/**`, `Scripts/Battle/Replay/View/ReplayScreen.cs` (battle end log only if needed), `Editor/Building/**`, `Resources/Core/**`, `Tests/EditMode/**` (not Engine), `Tests/PlayMode/**`. **Do not touch `Scripts/Engine/**` or `Scripts/Economy/**`.**
- UI/structure from builders only. English code. C# 9. No git. You cannot run Unity; re-read for compile errors.

## Tests
EditMode: LoginScene has a root `GameController` prefab instance with the 5 manager children and no `Services`; other scenes have none; GameController prefab refs assigned. PlayMode: existing 7 tests stay green; add one that starts from HomeScene directly and asserts exactly one GameController exists and survives a scene change; LogAssert must not see unexpected errors.

Finish with: files changed, builder menu items Claude must run (order), anything unsure.
