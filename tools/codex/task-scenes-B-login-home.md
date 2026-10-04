# Task B (2/4): structured LoginScene + HomeScene ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read first: `docs/decisions.md` #20, `docs/tech/ui-flow.md` ("Luồng", "Scene và prefab"), `Assets/_Game/Editor/Building/README.md` (task A foundation: `UiBuild`, `AssetGuard`, `SceneBuild`, common prefabs in `Prefabs/UI/Common/`, `UiButton`, "Build All Missing" registration).

Goal: `Scenes/LoginScene.unity` and `Scenes/HomeScene.unity` are **complete scenes**: opening them in the Editor shows the whole UI hierarchy; `LoginController` / `HomeController` only hold `[SerializeField]` references and logic — no UI creation at runtime.

## Hard rules
- Modify only: `Scripts/UI/Login/`, `Scripts/UI/Home/`, `Scenes/LoginScene.unity`, `Scenes/HomeScene.unity`, new `Editor/Building/LoginHomeSceneBuilder.cs`, `Prefabs/UI/Home/` (new), tests under `Tests/EditMode/UI/`. Another Codex is doing Gameplay/Bag/Replay in parallel: do NOT touch `Scripts/UI/Gameplay`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Scripts/Bag`, `Scripts/Battle`, `Editor/GameSceneBuilder.cs` (except leaving it compiling), `Resources/UI/Popups` (use popups via `PopupManager` only).
- English code, C# 9, existing style. No git. You cannot run Unity; Claude runs your builder in batch (`-executeMethod`, no dialogs in batch mode).
- Keep current behaviour exactly (read the current `Build()` code first): Login = name input + "Vào Giang Hồ" (saved via `GameSession`), Home = header with name/sect, menu buttons (Hành Tẩu Giang Hồ, Luận Kiếm Đài, Bao Phục → BagPopup, Tàng Bảo Các / Vạn Bảo Phổ / Thương Hội as "coming soon" MessagePopup, Cài đặt → SettingsPopup, Thoát), scene-ready signal to `SceneFlow`.

## Build
- Builder methods `BuildLoginScene(bool overwrite)` / `BuildHomeScene(bool overwrite)` + menus "One Bag Jianghu/Build/Login Scene", "Home Scene" (+ Overwrite twins), using `AssetGuard` (never overwrite by default). Scenes must follow the tree in ui-flow.md with meaningful names (e.g. `Canvas/Background`, `Canvas/Header/PlayerName`, `Canvas/Menu/Btn_Pve` …), buttons are instances of the common button prefabs, repeated menu entries may be a small prefab in `Prefabs/UI/Home/`.
- Since the current scene files are empty shells, the builder must replace them (treat "has only the old single controller object" as missing, or document that Claude will run the Overwrite menu once).
- `EditorBuildSettings` scene order stays Login, Home, Gameplay.

## Tests (EditMode)
Open each scene with `EditorSceneManager.OpenScene` (additive or single, restore afterwards) and assert: exactly one EventSystem, a Canvas with CanvasScaler 1920×1080 / match .5, the controller exists and has no null `[SerializeField]` Object references, expected buttons exist.

Finish with: files created/changed, the exact `-executeMethod` names Claude must run, anything unsure.

## Notes from task A (done)
- Do NOT edit `Editor/Building/BuildMenu.cs` (another Codex runs in parallel): Claude will register your builder there. Just tell Claude the method to register.
- The Editor assembly now references `UnityEngine.UI`, `Unity.InputSystem`, `OneBagJianghu.Engine`.
- Batch tool: `OneBagJianghu.EditorTools.Building.BuildMenu.BuildAllMissing`. Common prefabs: `Prefabs/UI/Common/{Button_Primary,Button_Secondary,Panel_Paper,Text_Title,Text_Body,InputField,Toggle,Slider,ScrollList}`; services in `Resources/Core/Services.prefab`.
