# Task E: PlayMode tests on the real structured scenes + full flow test ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Since decision #20 (`docs/decisions.md`) every scene is a complete, pre-built hierarchy and scripts no longer build UI at runtime. `Assets/_Game/Tests/PlayMode/DemoSmokeTests.cs` still does `new GameObject(typeof(BagDemoBootstrap))`, which now throws NullReferenceException (no serialized references) — the tests are outdated, not the game.

## Hard rules
- Modify only `Assets/_Game/Tests/PlayMode/` (tests + its asmdef if a reference is missing). Do not change game code; if the flow cannot be driven without a game change, stop and report exactly what is needed.
- English, C# 9, existing style. No git. You cannot run Unity; Claude runs the tests.
- **Known pitfall (hit 3 times already):** `bool ok = x != null && x.TryGet(k, out Sprite s); ... s ...` does not compile (CS0165). Declare `Sprite s = null;` first.
- Never block the main thread on async work (`.Result`, `.Wait()`, `GetAwaiter().GetResult()`): it deadlocks Unity. Use `yield return` loops with a timeout.

## Tests
Load scenes by path with `UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Single))` (inside `#if UNITY_EDITOR`; the PlayMode assembly runs in the Editor), so demo scenes need not be in Build Settings. Every wait has a timeout (e.g. 10 s) that fails with a clear message. Any unhandled exception/error log fails the test (default LogAssert behaviour — keep it).
1. `BagDemo`, `ReplayDemo`, `VfxLab`: load, run ~10 frames, assert key objects exist and are active (bag grid has item views; replay: find the play button via its component/serialized reference or `UiButton` label, play at 4×, run until the replay ends or 10 s; VfxLab: press "Play all" for 2 s).
2. **Full flow**: load `LoginScene` → set the name input (e.g. "Kiểm Thử") → click "Vào Giang Hồ" → wait for `HomeScene` active and the LoadingPanel hidden → click "Hành Tẩu Giang Hồ" → wait for `GameplayScene` → start the battle from the preparation phase (offline C# engine) → wait for the replay to finish at max speed (or skip to end if an API exists) → the result popup appears → click back to Home → `HomeScene` active again. Find UI through the controllers' serialized fields (reflection is fine in tests) or stable object names from the scene builders (`Editor/Building/*SceneBuilder.cs`), not through label text when avoidable.
3. The profile saved by the login step must not pollute the developer's real profile: back up and restore whatever `GameSession` uses (PlayerPrefs keys or file) in `[SetUp]/[TearDown]`.

Finish with: files created/changed, anything unsure.
