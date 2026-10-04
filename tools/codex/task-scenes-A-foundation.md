# Task A (1/4): structured scenes & prefabs — foundation ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI, Input System, legacy `UnityEngine.UI.Text`).

## Why
Today every scene holds ONE empty GameObject and all UI is created by code at Play (`UiKit`, `PopupBuildUi`, `ReplayUi`, controllers' `Build()`). The user wants **complete scenes/prefabs**: open a scene or prefab in the Editor and see the full hierarchy, edit it by hand, scripts only reference it. Read the rules first: `docs/decisions.md` #20 and `docs/tech/ui-flow.md` section "Scene và prefab". This task builds the foundation; tasks B (Login/Home), C (Gameplay/Bag/Replay), D (demo & tool scenes) come after and will reuse what you make.

## Hard rules
- Create/modify only under `One-Bag-Jianghu/Assets/_Game/`. No Packages/, ProjectSettings/, docs/, no git.
- English code, C# 9, match existing style. Editor code only in the Editor assembly. Must compile in Unity 6. The 104 EditMode tests keep passing (adapt tests only if they built UI through code you removed, keep their intent).
- You cannot run Unity (licensing fails in your sandbox). Claude will run your editor menus in batch mode, so every builder must be callable as `public static` method via `-executeMethod`, with no dialogs in batch mode (`Application.isBatchMode` → skip confirmation).
- Do NOT touch: `Scripts/UI/Login`, `Scripts/UI/Home`, `Scripts/UI/Gameplay`, `Scripts/Bag/View`, `Scripts/Battle/Replay/View` (tasks B/C/D). `UiKit` may stay for them to migrate later; do not delete it.

## 1. Editor infrastructure — `Editor/Building/`
- `UiBuild` (editor-only helpers to author UI in prefabs/scenes: create RectTransform child, Image, Text with the Vietnamese-capable default font, Button, layout helpers, stretch/anchor helpers, **instantiate a prefab as a linked prefab instance** via `PrefabUtility.InstantiatePrefab`). All static UI creation code lives here from now on (moved out of runtime).
- `AssetGuard`: `bool ShouldBuild(string assetPath, bool overwrite)` — returns true when missing; when it exists and `overwrite` is true, asks `EditorUtility.DisplayDialog` (auto-yes in batch mode); otherwise logs "exists, skipped" and returns false.
- `SceneBuild`: helper to create a scene with the standard tree from ui-flow.md (EventSystem with `InputSystemUIInputModule`, Canvas 1920×1080 ScaleWithScreenSize match .5 + GraphicRaycaster, named root objects) and save it.
- Menus under **One Bag Jianghu/Build/**: "Common UI Prefabs", "Services Prefab", "Popup Prefabs", and "Build All Missing" (calls every builder, including those tasks B–D will add — expose a simple registration: a static list/partial method or just call methods by name; keep it easy for B–D to add one line). Each has an "(Overwrite)" twin.

## 2. Common UI prefabs — `Assets/_Game/Prefabs/UI/Common/`
Thủy Mặc style using current colors from `UiKit` (paper, ink, gold, vermilion): `Button_Primary`, `Button_Secondary`, `Panel_Paper` (paper panel with gold outline), `Text_Title`, `Text_Body`, `InputField`, `Toggle`, `Slider`, `ScrollList` (ScrollRect + viewport + content with VerticalLayoutGroup + ContentSizeFitter). A small runtime component `UiButton` (on the button prefabs) exposing `SetLabel(string)` and `onClick` passthrough so code never digs into children by name.

## 3. Services prefab — `Assets/_Game/Resources/Core/Services.prefab`
Contains: `GameSession`, `NetworkService`, `SceneFlow`, `PopupManager` with its own child Canvas (sortingOrder 100, dimmer/backdrop object pre-made), `LoadingPanel` with its full child Canvas (sortingOrder 1000: background, title, progress bar, tip text, CanvasGroup) and an `EventSystem` child. Refactor `LoadingPanel`, `PopupManager`, `SceneFlow`, `GameBootstrap` so that:
- `LoadingPanel`/`PopupManager` use `[SerializeField]` references, no UI creation at runtime (keep behaviour: fade, progress, tips, stacking, dimmer, Esc to close, sorting +2 per level).
- `GameBootstrap` (still `RuntimeInitializeOnLoadMethod BeforeSceneLoad`) loads and `Instantiate`s `Resources/Core/Services`, `DontDestroyOnLoad`, wires references; if the prefab is missing, log a clear error telling to run "One Bag Jianghu/Build/Build All Missing". If a scene already contains an EventSystem, the services' EventSystem must not create a duplicate (disable one).

## 4. Popups — full prefabs
`Resources/UI/Popups/{Message,Confirm,Settings,Bag,Result}Popup.prefab` must contain their **entire** content (panel, title, body, buttons, fields) built by the editor builder using the common prefabs; popup classes get `[SerializeField]` references and drop runtime `PopupBuildUi` construction. Delete `PopupBuildUi` if nothing uses it anymore. `BagPopup` content that depends on the bag view (task C) may keep a placeholder `RectTransform BagHost` where task C will put the bag panel prefab — do not implement the bag view yourself.
Replace `Editor/PopupPrefabBuilder` with the new builder (keep the old menu path working or remove it — then update `GameSceneBuilder`/`DemoSceneBuilder` calls that referenced it so everything compiles).

## 5. Tests
- EditMode: Services prefab loads from Resources and has all components + both canvases with the right sorting orders; each popup prefab has its component and no null `[SerializeField]` references (reflection over serialized fields of type `UnityEngine.Object`).
- Keep `PopupStackTests` etc. green.

## 6. Report
Write `Assets/_Game/Editor/Building/README.md` (Vietnamese, ≤ 30 lines): menus, "create only when missing", how B–D register their builders. Finish with files created/changed and anything unsure.
