# Task N: flat scene hierarchy + Main Camera in every scene ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Read `docs/decisions.md` #20 (addendum 2026-09-30), `Editor/Building/*` (+ README), `Tests/EditMode/Scenes/**`, `Tests/PlayMode/**`.

## Rule (owner's request)
Every scene is laid out flat at the root, **no wrapper GameObject named after the scene**:

```
LoginScene
  Main Camera
  EventSystem
  LoginController      (or the scene's controller / bootstrap)
  Canvas               (the scene UI; more root canvases only if really needed)
```

- **Every scene has a `Main Camera`**: tag `MainCamera`, `Camera` (orthographic, clear flags Solid Color with the ink/paper background colour used today, depth -1) and one `AudioListener`. Canvases stay Screen Space Overlay unless a scene already needs Camera space (then assign this camera).
- One `EventSystem` (with `InputSystemUIInputModule`) per scene at the root. The persistent `Services` prefab (`Resources/Core/Services.prefab`, DontDestroyOnLoad) must not add a second EventSystem/AudioListener that causes the "There can be only one active Event System" warning — check `GameBootstrap`/Services and fix so only one is active at a time (e.g. Services owns none, or scenes' EventSystem is skipped when one exists — pick the simplest, explain).
- Applies to all builders: `SceneBuild.CreateStandard`, `LoginHomeSceneBuilder` (Login + Home), `GameplaySceneBuilder`, `DemoSceneBuilder` (BagDemo, ReplayDemo, VfxLab). Remove the scene-named roots and parent objects directly to the scene root; keep all serialized references wired.

## Hard rules
- Modify only `Editor/Building/**`, `Scripts/Core/GameBootstrap.cs` (+ Services-related scripts only if needed for the EventSystem fix), `Tests/EditMode/Scenes/**`, `Tests/PlayMode/**` (only where tests look up objects by the old root path). Another task has just changed `GameplaySceneBuilder.cs` and `ReplayPrefabBuilder.cs` for a new battle layout — keep those changes, only change the root structure/camera there.
- English, C# 9, existing style. No git. You cannot run Unity; re-read for compile errors.

## Tests
EditMode scene test for each of the 6 scenes (open the built scene asset): no root named like the scene, exactly one root `Main Camera` with tag MainCamera + Camera + AudioListener, exactly one EventSystem, controller/bootstrap and Canvas are root objects. Keep all existing tests green.

Finish with: files changed, builder menu items Claude must run (order), anything unsure.
