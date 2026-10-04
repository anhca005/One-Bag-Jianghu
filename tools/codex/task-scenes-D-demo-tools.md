# Task D (4/4): structured demo & tool scenes — BagDemo, ReplayDemo, VfxLab ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read first: `docs/decisions.md` #20, `docs/tech/ui-flow.md` ("Scene và prefab"), `Assets/_Game/Editor/Building/README.md`, and the prefabs made by tasks A (common UI) and C (`Prefabs/UI/Bag/`, `Prefabs/UI/Replay/`).

Goal: `Scenes/BagDemo.unity`, `Scenes/ReplayDemo.unity`, `Scenes/VfxLab.unity` are **complete scenes** with the full hierarchy visible in the Editor, reusing the bag/replay prefabs (prefab instances, not copies). `BagDemoBootstrap`, `ReplayDemoBootstrap`, `VfxLabBootstrap` keep only `[SerializeField]` references and logic.

## Hard rules
- Modify only: `Scripts/Bag/View/BagDemoBootstrap.cs`, `Scripts/Battle/Replay/View/ReplayDemoBootstrap.cs`, `Scripts/Battle/Replay/View/VfxLab/` (all), `Scenes/BagDemo.unity`, `Scenes/ReplayDemo.unity`, `Scenes/VfxLab.unity`, `Prefabs/UI/Tools/` (new, e.g. VFX Lab catalog row, tuning panel), `Editor/DemoSceneBuilder.cs` (move to `Editor/Building/`, use `AssetGuard`), tests under `Tests/EditMode/VfxLab/` and a new `Tests/EditMode/Scenes/`.
- Do not change the bag/replay prefabs or views from task C; if something is missing, report it instead.
- Every VFX Lab feature must keep working: catalog list with ✓/✗/✎ marks and count, stage with two fighters + `ReplayVfxLayer`, play/loop/side/speed/play all/prev/next/background/hit-after, inspector with in-game size box + metrics, tuning panel with sliders/toggles/reset/save/unsaved mark, keyboard shortcuts. Catalog rows are instantiated from a row prefab at runtime (dynamic), everything else is in the scene.
- English code, C# 9, existing style. No git. You cannot run Unity; Claude runs your builders in batch.

## Tests (EditMode)
For each of the three scenes: one EventSystem, Canvas scaler 1920×1080/.5, bootstrap present with no null `[SerializeField]` Object references. Existing VfxCatalog/VfxTuning tests stay green.

Finish with: files created/changed, exact `-executeMethod` names Claude must run, anything unsure.

## Notes from task A (done)
- Do NOT edit `Editor/Building/BuildMenu.cs` (another Codex runs in parallel): Claude will register your builder there. Just tell Claude the method to register.
- The Editor assembly now references `UnityEngine.UI`, `Unity.InputSystem`, `OneBagJianghu.Engine`.
- Batch tool: `OneBagJianghu.EditorTools.Building.BuildMenu.BuildAllMissing`. Common prefabs: `Prefabs/UI/Common/{Button_Primary,Button_Secondary,Panel_Paper,Text_Title,Text_Body,InputField,Toggle,Slider,ScrollList}`; services in `Resources/Core/Services.prefab`.
